using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public class PopupWinMgr
	{
		public static void LoadPopupWinItemList()
		{
			XElement xelement = null;
			string text = "Config/PopupWin.xml";
			try
			{
				xelement = XElement.Load(Global.GameResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			List<PopupWinItem> popupWinItemList = new List<PopupWinItem>();
			IEnumerable<XElement> enumerable = xelement.Elements();
			foreach (XElement xmlnode in enumerable)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = xmlnode
				};
				PopupWinMgr.ParseXmlItem(systemXmlItem, popupWinItemList);
			}
			PopupWinMgr.PopupWinItemList = popupWinItemList;
		}

		private static void ParseXmlItem(SystemXmlItem systemXmlItem, List<PopupWinItem> popupWinItemList)
		{
			int intValue = systemXmlItem.GetIntValue("ID", -1);
			int intValue2 = systemXmlItem.GetIntValue("HintFileID", -1);
			string stringValue = systemXmlItem.GetStringValue("Times");
			if (!string.IsNullOrEmpty(stringValue))
			{
				PopupWinTimeItem[] array = PopupWinMgr.ParsePopupWinTimeItems(stringValue);
				if (null == array)
				{
					LogManager.WriteLog(2, string.Format("解析弹窗配置表中的时间项为数组时失败, ID={0}", intValue), null, true);
				}
				else
				{
					PopupWinItem item = new PopupWinItem
					{
						ID = intValue,
						HintFileID = intValue2,
						Times = array
					};
					popupWinItemList.Add(item);
				}
			}
		}

		private static PopupWinTimeItem[] ParsePopupWinTimeItems(string times)
		{
			string[] array = times.Split(new char[]
			{
				'|'
			});
			PopupWinTimeItem[] result;
			if (array.Length <= 0)
			{
				result = null;
			}
			else
			{
				PopupWinTimeItem[] array2 = new PopupWinTimeItem[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (string.IsNullOrEmpty(text))
					{
						return null;
					}
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					if (array3 == null || array3.Length != 2)
					{
						return null;
					}
					array2[i] = new PopupWinTimeItem
					{
						Hour = Global.SafeConvertToInt32(array3[0]),
						Minute = Global.SafeConvertToInt32(array3[1])
					};
				}
				result = array2;
			}
			return result;
		}

		private static bool CanPopupWin(PopupWinItem popupWinItem, PopupWinTimeItem lastPopupWinTimeItem, int hour, int minute)
		{
			bool result;
			if (null == popupWinItem.Times)
			{
				result = false;
			}
			else
			{
				int num = lastPopupWinTimeItem.Hour * 60 + lastPopupWinTimeItem.Minute;
				int num2 = hour * 60 + minute;
				for (int i = 0; i < popupWinItem.Times.Length; i++)
				{
					int num3 = popupWinItem.Times[i].Hour * 60 + popupWinItem.Times[i].Minute;
					if (num3 > num)
					{
						if (num2 >= num3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public static void ProcessPopupWins()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfYear = dateTime.DayOfYear;
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			if (dayOfYear != PopupWinMgr.LastPopupWinDay)
			{
				PopupWinMgr.LastPopupWinDay = dayOfYear;
				PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
				PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
			}
			else if (hour != PopupWinMgr.LastPopupWinTimeItem.Hour || minute != PopupWinMgr.LastPopupWinTimeItem.Minute)
			{
				List<PopupWinItem> popupWinItemList = PopupWinMgr.PopupWinItemList;
				if (popupWinItemList == null || popupWinItemList.Count <= 0)
				{
					PopupWinMgr.LastPopupWinDay = dayOfYear;
					PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
					PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
				}
				else
				{
					for (int i = 0; i < popupWinItemList.Count; i++)
					{
						if (PopupWinMgr.CanPopupWin(popupWinItemList[i], PopupWinMgr.LastPopupWinTimeItem, hour, minute))
						{
							string strcmd = string.Format("{0}", popupWinItemList[i].HintFileID);
							GameManager.ClientMgr.NotifyAllPopupWinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, strcmd);
						}
					}
					PopupWinMgr.LastPopupWinDay = dayOfYear;
					PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
					PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
				}
			}
		}

		public static void ProcessClientPopupWins(GameClient client)
		{
			List<PopupWinItem> popupWinItemList = PopupWinMgr.PopupWinItemList;
			if (popupWinItemList != null && popupWinItemList.Count > 0)
			{
				if (popupWinItemList[0].Times.Length > 0)
				{
					string strcmd = string.Format("{0}", popupWinItemList[0].HintFileID);
					GameManager.ClientMgr.NotifyPopupWinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd);
				}
			}
		}

		private static List<PopupWinItem> PopupWinItemList = null;

		private static int LastPopupWinDay = TimeUtil.NowDateTime().DayOfYear;

		private static PopupWinTimeItem LastPopupWinTimeItem = new PopupWinTimeItem
		{
			Hour = TimeUtil.NowDateTime().Hour,
			Minute = TimeUtil.NowDateTime().Minute
		};
	}
}
