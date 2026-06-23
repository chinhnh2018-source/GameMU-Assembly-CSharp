using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public class BroadcastInfoMgr
	{
		public static void LoadBroadcastInfoItemList()
		{
			XElement xelement = null;
			string text = "Config/BroadcastInfos.xml";
			try
			{
				xelement = XElement.Load(Global.IsolateResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			List<BroadcastInfoItem> broadcastInfoItemList = new List<BroadcastInfoItem>();
			IEnumerable<XElement> enumerable = xelement.Elements("Infos").Elements<XElement>();
			foreach (XElement xmlnode in enumerable)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = xmlnode
				};
				BroadcastInfoMgr.ParseXmlItem(systemXmlItem, broadcastInfoItemList);
			}
			BroadcastInfoMgr.BroadcastInfoItemList = broadcastInfoItemList;
		}

		private static void ParseXmlItem(SystemXmlItem systemXmlItem, List<BroadcastInfoItem> broadcastInfoItemList)
		{
			int intValue = systemXmlItem.GetIntValue("ID", -1);
			int intValue2 = systemXmlItem.GetIntValue("InfoClass", -1);
			int intValue3 = systemXmlItem.GetIntValue("HintErrID", -1);
			int intValue4 = systemXmlItem.GetIntValue("TimeType", -1);
			int intValue5 = systemXmlItem.GetIntValue("StartDay", -1);
			int intValue6 = systemXmlItem.GetIntValue("ShowType", -1);
			string stringValue = systemXmlItem.GetStringValue("WeekDays");
			string stringValue2 = systemXmlItem.GetStringValue("Times");
			string stringValue3 = systemXmlItem.GetStringValue("Text");
			string stringValue4 = systemXmlItem.GetStringValue("OnlineNotice");
			int intValue7 = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
			int intValue8 = systemXmlItem.GetIntValue("MinLevel", -1);
			if (string.IsNullOrEmpty(stringValue2))
			{
				LogManager.WriteLog(2, string.Format("解析广播配置表中的时间项失败, ID={0}", intValue), null, true);
			}
			else
			{
				BroadcastTimeItem[] array = BroadcastInfoMgr.ParseBroadcastTimeItems(stringValue2);
				if (null == array)
				{
					LogManager.WriteLog(2, string.Format("解析广播配置表中的时间项为数组时失败, ID={0}", intValue), null, true);
				}
				else if (string.IsNullOrEmpty(stringValue3))
				{
					LogManager.WriteLog(2, string.Format("解析广播配置表中的时间项失败, ID={0}", intValue), null, true);
				}
				else
				{
					DateTimeRange[] onlineNoticeTimeRanges = Global.ParseDateTimeRangeStr(stringValue4);
					BroadcastInfoItem item = new BroadcastInfoItem
					{
						ID = intValue,
						InfoClass = intValue2,
						HintErrID = intValue3,
						TimeType = intValue4,
						KaiFuStartDay = intValue5,
						KaiFuShowType = intValue6,
						WeekDays = stringValue,
						Times = array,
						OnlineNoticeTimeRanges = onlineNoticeTimeRanges,
						Text = stringValue3.Replace(":", ""),
						MinZhuanSheng = intValue7,
						MinLevel = intValue8
					};
					broadcastInfoItemList.Add(item);
				}
			}
		}

		private static BroadcastTimeItem[] ParseBroadcastTimeItems(string times)
		{
			BroadcastTimeItem[] result;
			if (string.IsNullOrEmpty(times))
			{
				result = null;
			}
			else
			{
				string[] array = times.Split(new char[]
				{
					'|'
				});
				if (array.Length <= 0)
				{
					result = null;
				}
				else
				{
					BroadcastTimeItem[] array2 = new BroadcastTimeItem[array.Length];
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
						array2[i] = new BroadcastTimeItem
						{
							Hour = Global.SafeConvertToInt32(array3[0]),
							Minute = Global.SafeConvertToInt32(array3[1])
						};
					}
					result = array2;
				}
			}
			return result;
		}

		private static bool CanBroadcast(BroadcastInfoItem broadcastInfoItem, BroadcastTimeItem lastBroadcastTimeItem, int weekDayID, int hour, int minute)
		{
			bool result;
			if (null == broadcastInfoItem.Times)
			{
				result = false;
			}
			else
			{
				if (!string.IsNullOrEmpty(broadcastInfoItem.WeekDays))
				{
					if (-1 == broadcastInfoItem.WeekDays.IndexOf(weekDayID.ToString()))
					{
						return false;
					}
				}
				if (broadcastInfoItem.KaiFuStartDay > 0)
				{
					DateTime now = Global.GetKaiFuTime();
					if (2 == broadcastInfoItem.TimeType)
					{
						now = Global.GetHefuStartDay();
					}
					else if (3 == broadcastInfoItem.TimeType)
					{
						now = Global.GetJieriStartDay();
					}
					DateTime now2 = TimeUtil.NowDateTime();
					int offsetDay = Global.GetOffsetDay(now2);
					int offsetDay2 = Global.GetOffsetDay(now);
					if (offsetDay - offsetDay2 < broadcastInfoItem.KaiFuStartDay)
					{
						return false;
					}
					if (broadcastInfoItem.KaiFuShowType > 0)
					{
						if (offsetDay - offsetDay2 >= broadcastInfoItem.KaiFuStartDay + broadcastInfoItem.KaiFuShowType)
						{
							return false;
						}
					}
				}
				int num = lastBroadcastTimeItem.Hour * 60 + lastBroadcastTimeItem.Minute;
				int num2 = hour * 60 + minute;
				for (int i = 0; i < broadcastInfoItem.Times.Length; i++)
				{
					int num3 = broadcastInfoItem.Times[i].Hour * 60 + broadcastInfoItem.Times[i].Minute;
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

		public static void ProcessBroadcastInfos()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfWeek = (int)dateTime.DayOfWeek;
			int dayOfYear = dateTime.DayOfYear;
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			if (dayOfYear != BroadcastInfoMgr.LastBroadcastDay)
			{
				BroadcastInfoMgr.LastBroadcastDay = dayOfYear;
				BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
				BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
			}
			else if (hour != BroadcastInfoMgr.LastBroadcastTimeItem.Hour || minute != BroadcastInfoMgr.LastBroadcastTimeItem.Minute)
			{
				List<BroadcastInfoItem> broadcastInfoItemList = BroadcastInfoMgr.BroadcastInfoItemList;
				if (broadcastInfoItemList == null || broadcastInfoItemList.Count <= 0)
				{
					BroadcastInfoMgr.LastBroadcastDay = dayOfYear;
					BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
					BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
				}
				else
				{
					for (int i = 0; i < broadcastInfoItemList.Count; i++)
					{
						if (BroadcastInfoMgr.CanBroadcast(broadcastInfoItemList[i], BroadcastInfoMgr.LastBroadcastTimeItem, dayOfWeek, hour, minute))
						{
							if (broadcastInfoItemList[i].InfoClass <= 1)
							{
								Global.BroadcastRoleActionMsg(null, (broadcastInfoItemList[i].InfoClass == 0) ? RoleActionsMsgTypes.Bulletin : RoleActionsMsgTypes.HintMsg, broadcastInfoItemList[i].Text, false, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, 100, 100);
							}
							else if (3 == broadcastInfoItemList[i].InfoClass)
							{
								GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, broadcastInfoItemList[i].Text, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Math.Max(broadcastInfoItemList[i].HintErrID, 0), broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, 100, 100);
							}
						}
					}
					BroadcastInfoMgr.LastBroadcastDay = dayOfYear;
					BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
					BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
				}
			}
		}

		public static void LoginBroadcastInfos(GameClient client)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			List<BroadcastInfoItem> broadcastInfoItemList = BroadcastInfoMgr.BroadcastInfoItemList;
			if (broadcastInfoItemList != null && broadcastInfoItemList.Count > 0)
			{
				DateTime now = dateTime;
				int dayOfWeek = (int)dateTime.DayOfWeek;
				for (int i = 0; i < broadcastInfoItemList.Count; i++)
				{
					if (broadcastInfoItemList[i].InfoClass == 3)
					{
						if (null != broadcastInfoItemList[i].OnlineNoticeTimeRanges)
						{
							if (Global.GetUnionLevel(client, false) >= Global.GetUnionLevel(broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, false))
							{
								int num = 0;
								if (Global.JugeDateTimeInTimeRange(dateTime, broadcastInfoItemList[i].OnlineNoticeTimeRanges, out num, true))
								{
									if (!string.IsNullOrEmpty(broadcastInfoItemList[i].WeekDays))
									{
										if (-1 == broadcastInfoItemList[i].WeekDays.IndexOf(dayOfWeek.ToString()))
										{
											goto IL_246;
										}
									}
									if (broadcastInfoItemList[i].KaiFuStartDay > 0)
									{
										DateTime now2 = Global.GetKaiFuTime();
										if (2 == broadcastInfoItemList[i].TimeType)
										{
											now2 = Global.GetHefuStartDay();
										}
										else if (3 == broadcastInfoItemList[i].TimeType)
										{
											now2 = Global.GetJieriStartDay();
										}
										int offsetDay = Global.GetOffsetDay(now);
										int offsetDay2 = Global.GetOffsetDay(now2);
										if (offsetDay - offsetDay2 < broadcastInfoItemList[i].KaiFuStartDay)
										{
											goto IL_246;
										}
										if (broadcastInfoItemList[i].KaiFuShowType > 0)
										{
											if (offsetDay - offsetDay2 >= broadcastInfoItemList[i].KaiFuStartDay + broadcastInfoItemList[i].KaiFuShowType)
											{
												goto IL_246;
											}
										}
									}
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, broadcastInfoItemList[i].Text, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Math.Max(broadcastInfoItemList[i].HintErrID, 0));
								}
							}
						}
					}
					IL_246:;
				}
			}
		}

		private static List<BroadcastInfoItem> BroadcastInfoItemList = null;

		private static int LastBroadcastDay = TimeUtil.NowDateTime().DayOfYear;

		private static BroadcastTimeItem LastBroadcastTimeItem = new BroadcastTimeItem
		{
			Hour = TimeUtil.NowDateTime().Hour,
			Minute = TimeUtil.NowDateTime().Minute
		};
	}
}
