using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class WebOldPlayerManager
	{
		public static WebOldPlayerManager getInstance()
		{
			lock (WebOldPlayerManager.Mutex)
			{
				if (WebOldPlayerManager.instance != null)
				{
					return WebOldPlayerManager.instance;
				}
				WebOldPlayerManager.instance = new WebOldPlayerManager();
			}
			WebOldPlayerManager.LoadWebZhaoHuiXml();
			return WebOldPlayerManager.instance;
		}

		public static void ReloadXml()
		{
			WebOldPlayerManager.LoadWebZhaoHuiXml();
		}

		public static void LoadWebZhaoHuiXml()
		{
			try
			{
				string uri = "Config/WebOldPlayer.xml";
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(uri), true);
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					DateTime dateTime = TimeUtil.NowDateTime();
					Dictionary<int, WebOldPlayerManager.WebZhaoHuiData> dictionary = new Dictionary<int, WebOldPlayerManager.WebZhaoHuiData>();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int key = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							DateTime begionTime = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "BeginTime", ""));
							DateTime endTime = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "EndTime", ""));
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "GoodsOne", "");
							List<GoodsData> list = new List<GoodsData>();
							List<GoodsData> list2 = new List<GoodsData>();
							if (defAttributeStr != "")
							{
								string[] array = defAttributeStr.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < array.Length; i++)
								{
									if (!(array[i] == ""))
									{
										string[] array2 = array[i].Split(new char[]
										{
											','
										});
										if (array2.Length == 7)
										{
											int[] array3 = Global.StringArray2IntArray(array2);
											GoodsData newGoodsData = Global.GetNewGoodsData(array3[0], array3[1], 0, array3[3], array3[2], 0, array3[5], 0, array3[6], array3[4], 0);
											SystemXmlItem systemXmlItem = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(newGoodsData.GoodsID, out systemXmlItem))
											{
												LogManager.WriteLog(2, string.Format("系统中不存在{0}", newGoodsData.GoodsID), null, true);
											}
											else
											{
												list.Add(newGoodsData);
											}
										}
									}
								}
							}
							defAttributeStr = Global.GetDefAttributeStr(xelement2, "GoodsTwo", "");
							if (defAttributeStr != "")
							{
								string[] array = defAttributeStr.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < array.Length; i++)
								{
									if (!(array[i] == ""))
									{
										string[] array2 = array[i].Split(new char[]
										{
											','
										});
										if (array2.Length == 7)
										{
											int[] array3 = Global.StringArray2IntArray(array2);
											GoodsData newGoodsData = Global.GetNewGoodsData(array3[0], array3[1], 0, array3[3], array3[2], 0, array3[5], 0, array3[6], array3[4], 0);
											SystemXmlItem systemXmlItem = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(newGoodsData.GoodsID, out systemXmlItem))
											{
												LogManager.WriteLog(2, string.Format("系统中不存在{0}", newGoodsData.GoodsID), null, true);
											}
											else
											{
												list2.Add(newGoodsData);
											}
										}
									}
								}
							}
							string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "MaitTitle", "");
							string defAttributeStr3 = Global.GetDefAttributeStr(xelement2, "MailContent", "");
							WebOldPlayerManager.WebZhaoHuiData value = new WebOldPlayerManager.WebZhaoHuiData
							{
								BegionTime = begionTime,
								EndTime = endTime,
								GoodsOne = list,
								GoodsTwo = list2,
								MailTitle = defAttributeStr2,
								MailContent = defAttributeStr3
							};
							dictionary[key] = value;
						}
					}
					lock (WebOldPlayerManager.Mutex)
					{
						WebOldPlayerManager.RunTimeZhaoHuiData = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/WebZhaoHui.xml解析出现异常", ex, true);
			}
		}

		public bool ChouJiangAddCheck(int roleID, int chouJiangType)
		{
			DateTime t = TimeUtil.NowDateTime();
			lock (WebOldPlayerManager.Mutex)
			{
				if (WebOldPlayerManager.RunTimeZhaoHuiData == null || WebOldPlayerManager.RunTimeZhaoHuiData.Count < 1 || t > WebOldPlayerManager.RunTimeZhaoHuiData[0].EndTime || t < WebOldPlayerManager.RunTimeZhaoHuiData[0].BegionTime)
				{
					return false;
				}
			}
			int num = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(roleID, "10167", 0));
			bool result;
			if (num < 1)
			{
				result = false;
			}
			else
			{
				string cmdText = string.Format("{0}:{1}:{2}", roleID, chouJiangType, t.Date.ToString().Replace(':', '$'));
				GameManager.DBCmdMgr.AddDBCmd(20305, cmdText, null, GameCoreInterface.getinstance().GetLocalServerId());
				result = true;
			}
			return result;
		}

		public void WebOldPlayerCheck(int roleID, int awardID)
		{
			try
			{
				DateTime t = TimeUtil.NowDateTime();
				WebOldPlayerManager.WebZhaoHuiData webZhaoHuiData = null;
				lock (WebOldPlayerManager.Mutex)
				{
					if (WebOldPlayerManager.RunTimeZhaoHuiData == null || WebOldPlayerManager.RunTimeZhaoHuiData.Count < 1)
					{
						return;
					}
					if (!WebOldPlayerManager.RunTimeZhaoHuiData.TryGetValue(awardID, out webZhaoHuiData))
					{
						return;
					}
					if (t > webZhaoHuiData.EndTime || t < webZhaoHuiData.BegionTime)
					{
						return;
					}
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				int num;
				if (null == gameClient)
				{
					num = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(roleID, "10167", 0));
					if (num < 1)
					{
						GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", roleID, "10167", 1), null, GameCoreInterface.getinstance().GetLocalServerId());
					}
				}
				else
				{
					num = Global.GetRoleParamsInt32FromDB(gameClient, "10167");
					if (num < 1)
					{
						Global.SaveRoleParamsInt32ValueToDB(gameClient, "10167", 1, true);
					}
				}
				if (num < 1)
				{
					List<GoodsData> list = new List<GoodsData>();
					string chineseText = "";
					string chineseText2 = "";
					lock (WebOldPlayerManager.Mutex)
					{
						foreach (GoodsData item in webZhaoHuiData.GoodsOne)
						{
							list.Add(item);
						}
						foreach (GoodsData item in webZhaoHuiData.GoodsTwo)
						{
							list.Add(item);
						}
						chineseText = webZhaoHuiData.MailTitle;
						chineseText2 = webZhaoHuiData.MailContent;
					}
					Global.UseMailGivePlayerAward3(roleID, list, Global.GetLang(chineseText), Global.GetLang(chineseText2), 0, 0, 0);
				}
			}
			catch
			{
			}
		}

		public static Dictionary<int, WebOldPlayerManager.WebZhaoHuiData> RunTimeZhaoHuiData = new Dictionary<int, WebOldPlayerManager.WebZhaoHuiData>();

		private static object Mutex = new object();

		private static WebOldPlayerManager instance = null;

		public class WebZhaoHuiData
		{
			public DateTime BegionTime;

			public DateTime EndTime;

			public List<GoodsData> GoodsOne;

			public List<GoodsData> GoodsTwo;

			public string MailTitle;

			public string MailContent;
		}
	}
}
