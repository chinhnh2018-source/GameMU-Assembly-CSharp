using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

namespace HSGameEngine.GameEngine.Logic
{
	public class ActivityTipManager
	{
		private static bool CanShowIconByTime(int startDay, DateTime kaiFuDayTime, XElement item)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (startDay <= 0)
			{
				return false;
			}
			if (correctDateTime.DayOfYear - kaiFuDayTime.DayOfYear >= startDay)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(item, "ShowType");
				return xelementAttributeInt <= 0 || correctDateTime.DayOfYear - kaiFuDayTime.DayOfYear < startDay + xelementAttributeInt;
			}
			return false;
		}

		private static void LoadActivityTipCfg()
		{
			if (ActivityTipManager.DictActivityTip != null)
			{
				return;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.KaiFuStartDay))
			{
				return;
			}
			DateTime dateTime = default(DateTime);
			string[] array = Global.Data.roleData.KaiFuStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 01:01:01", ref dateTime);
			DateTime serverMergeTime = Global.GetServerMergeTime();
			DateTime jieriTime = Global.GetJieriTime();
			ActivityTipManager.DictActivityTip = new Dictionary<int, List<ActivityTime>>();
			ActivityTipManager.DictAcitvityMinLevle = new Dictionary<int, int>();
			XElement gameResXml = Global.GetGameResXml("Config/Activity/ActivityTip.Xml");
			if (gameResXml == null)
			{
				return;
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Tip");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					ActivityTipManager.DictAcitvityMinLevle.Add(xelementAttributeInt, UIHelper.GetUnionZhuanShengLevel(xelementAttributeInt2, xelementAttributeInt3));
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowTimes");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "WeekDays");
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						','
					});
					int dayOfWeek = correctDateTime.DayOfWeek;
					if (StringUtil.trim(xelementAttributeStr2).Length == 0 || Enumerable.ToList<string>(array2).IndexOf(StringUtil.substitute("{0}", new object[]
					{
						dayOfWeek
					})) >= 0)
					{
						int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "TimeType");
						DateTime kaiFuDayTime = dateTime;
						if (xelementAttributeInt4 == 2)
						{
							kaiFuDayTime = serverMergeTime;
						}
						else if (xelementAttributeInt4 == 3)
						{
							kaiFuDayTime = jieriTime;
						}
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "StartDay");
						if (!string.IsNullOrEmpty(xelementAttributeStr3))
						{
							bool flag = true;
							string[] array3 = xelementAttributeStr3.Split(new char[]
							{
								','
							});
							for (int j = 0; j < array3.Length; j++)
							{
								int startDay = Global.SafeConvertToInt32(array3[j]);
								if (ActivityTipManager.CanShowIconByTime(startDay, kaiFuDayTime, xelement))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								goto IL_32F;
							}
						}
						List<ActivityTime> list = new List<ActivityTime>();
						string[] array4 = xelementAttributeStr.Split(new char[]
						{
							'|'
						});
						foreach (string text in array4)
						{
							string[] array6 = text.Split(new char[]
							{
								'-'
							});
							if (array6.Length == 2)
							{
								string text2 = correctDateTime.ToString("yyyy-MM-dd ") + StringUtil.trim(array6[0]) + string.Empty;
								string text3 = correctDateTime.ToString("yyyy-MM-dd ") + StringUtil.trim(array6[1]) + string.Empty;
								DateTime dateTimeLeft = default(DateTime);
								if (DateTime.TryParse(text2, ref dateTimeLeft))
								{
									DateTime dateTimeRight = default(DateTime);
									if (DateTime.TryParse(text3, ref dateTimeRight))
									{
										list.Add(new ActivityTime
										{
											DateTimeLeft = dateTimeLeft,
											DateTimeRight = dateTimeRight
										});
									}
								}
							}
						}
						ActivityTipManager.DictActivityTip.Add(xelementAttributeInt, list);
					}
				}
				IL_32F:;
			}
		}

		private static void LoadFuBenConfig()
		{
			int num = -1;
			int num2 = -1;
			ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.Clear();
			ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.Clear();
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Display");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
				RiChangFuBenItemData riChangFuBenItemData = new RiChangFuBenItemData();
				riChangFuBenItemData.NeedYuanBao = Global.GetXElementAttributeInt(xelement, "NeedYuanBao");
				riChangFuBenItemData.MinZhuanSheng = xelementAttributeInt5;
				riChangFuBenItemData.MaxZhuanSheng = xelementAttributeInt6;
				riChangFuBenItemData.MinLevel = xelementAttributeInt3;
				riChangFuBenItemData.MaxLevel = xelementAttributeInt4;
				riChangFuBenItemData.CopyID = Global.GetXElementAttributeInt(xelement, "ID");
				riChangFuBenItemData.TabID = Global.GetXElementAttributeInt(xelement, "TabID");
				riChangFuBenItemData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				riChangFuBenItemData.DisplayID = xelementAttributeInt2;
				riChangFuBenItemData.ItemName = Global.GetXElementAttributeStr(xelement, "CopyName");
				riChangFuBenItemData.CopyType = ((Global.GetXElementAttributeInt(xelement, "CopyType") > 0) ? Global.GetLang("组队") : Global.GetLang("个人"));
				riChangFuBenItemData.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "EnterNumber");
				if (riChangFuBenItemData.MaxEnterNum < 0)
				{
					riChangFuBenItemData.MaxEnterNum = int.MaxValue;
				}
				riChangFuBenItemData.MaxFinishNum = Global.GetXElementAttributeInt(xelement, "FinishNumber");
				if (riChangFuBenItemData.MaxFinishNum < 0)
				{
					riChangFuBenItemData.MaxFinishNum = int.MaxValue;
				}
				riChangFuBenItemData.EnterGoods = Global.GetXElementAttributeInt(xelement, "EnterGoods");
				riChangFuBenItemData.GoodsNumber = Global.GetXElementAttributeInt(xelement, "GoodsNumber");
				riChangFuBenItemData.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
				riChangFuBenItemData.Remind = Global.GetXElementAttributeInt(xelement, "Remind");
				riChangFuBenItemData.RewardGoods = Global.GetXElementAttributeStr(xelement, "RewardGoods");
				riChangFuBenItemData.MinSaoDangTime = Global.GetFuBenMapMinSaoDangTime(riChangFuBenItemData.MapCode) * 60;
				riChangFuBenItemData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5, xelementAttributeInt6);
				ActivityTipManager.RiChangFuBenItemDataDict[riChangFuBenItemData.CopyID] = riChangFuBenItemData;
				if (!ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.ContainsKey(riChangFuBenItemData.TabID))
				{
					num++;
					ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.Add(riChangFuBenItemData.TabID, num);
				}
				if (!ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.ContainsKey(riChangFuBenItemData.CopyID))
				{
					num2++;
					ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.Add(riChangFuBenItemData.CopyID, num2);
				}
				int activityTipTypeByFuBenTabID = ActivityTipManager.GetActivityTipTypeByFuBenTabID(riChangFuBenItemData.TabID);
				ActivityTipItem activityTipItem = null;
				if (!ActivityTipManager.ActivityTipItemDict.ContainsKey(activityTipTypeByFuBenTabID))
				{
					int fuBenTypeByCopyID = ActivityTipManager.GetFuBenTypeByCopyID(riChangFuBenItemData.CopyID);
					if (fuBenTypeByCopyID == 0)
					{
						activityTipItem = ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenTabID, 2001);
						riChangFuBenItemData.PreQuery = true;
					}
					else if (fuBenTypeByCopyID == 1)
					{
						activityTipItem = ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenTabID, 2002);
						activityTipItem.Hide = true;
						riChangFuBenItemData.PreQuery = true;
					}
					else if (fuBenTypeByCopyID == 2)
					{
						activityTipItem = ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenTabID, 2003);
						riChangFuBenItemData.PreQuery = true;
					}
					else if (fuBenTypeByCopyID == 9)
					{
						activityTipItem = ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenTabID, 2004);
						riChangFuBenItemData.PreQuery = true;
					}
					else
					{
						activityTipItem = ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenTabID, 0);
						activityTipItem.Hide = true;
					}
				}
				if (activityTipItem == null)
				{
					activityTipItem = ActivityTipManager.GetActivityTipItem(activityTipTypeByFuBenTabID);
				}
				if (activityTipItem != null)
				{
					activityTipItem.NeedLevel = Global.GetFuBenTabNeedLevel(riChangFuBenItemData.TabID);
					activityTipItem.NeedTaskID = Global.GetFuBenTabNeedCompleteTask(riChangFuBenItemData.TabID);
					if (!activityTipItem.Hide)
					{
						activityTipItem.Hide = (riChangFuBenItemData.Remind <= 0);
					}
					if (!riChangFuBenItemData.PreQuery)
					{
						riChangFuBenItemData.PreQuery = (riChangFuBenItemData.Remind > 0);
					}
				}
				int activityTipTypeByFuBenID = ActivityTipManager.GetActivityTipTypeByFuBenID(riChangFuBenItemData.CopyID);
				if (!ActivityTipManager.ActivityTipItemDict.ContainsKey(activityTipTypeByFuBenID))
				{
					ActivityTipManager.AddActivityTipItem(activityTipTypeByFuBenID, activityTipTypeByFuBenTabID);
				}
			}
		}

		private static void LoadFuBenTabConfig()
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenTab.Xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "FuBenType");
				ActivityTipManager.RiChangFuBenTabTypeDict[xelementAttributeInt] = xelementAttributeInt2;
			}
		}

		public static bool IsActivityTipInShowTime(int type)
		{
			ActivityTipManager.LoadActivityTipCfg();
			if (ActivityTipManager.DictActivityTip == null || ActivityTipManager.DictAcitvityMinLevle == null)
			{
				return false;
			}
			if (!ActivityTipManager.DictActivityTip.ContainsKey(type))
			{
				return false;
			}
			if (!ActivityTipManager.DictAcitvityMinLevle.ContainsKey(type))
			{
				return false;
			}
			int unionZhuanShengLevel = UIHelper.GetUnionZhuanShengLevel(Global.Data.roleData.Level, Global.Data.roleData.ChangeLifeCount);
			if (unionZhuanShengLevel < ActivityTipManager.DictAcitvityMinLevle[type])
			{
				return false;
			}
			List<ActivityTime> list = ActivityTipManager.DictActivityTip[type];
			if (list.Count < 1)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				DateTime dateTimeLeft = list[i].DateTimeLeft;
				DateTime dateTimeRight = list[i].DateTimeRight;
				DateTime correctDateTime = Global.GetCorrectDateTime();
				long ticks = correctDateTime.Ticks;
				if (dateTimeLeft.Year != correctDateTime.Year)
				{
					ActivityTipManager.DictActivityTip.Clear();
					ActivityTipManager.DictActivityTip = null;
					return false;
				}
				if (ticks >= dateTimeLeft.Ticks && ticks < dateTimeRight.Ticks)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetActivityTipCountInShowTime()
		{
			if (Global.Data.roleData == null)
			{
				return 0;
			}
			int num = 0;
			ActivityTipManager.LoadActivityTipCfg();
			if (ActivityTipManager.DictActivityTip == null || ActivityTipManager.DictAcitvityMinLevle == null)
			{
				return 0;
			}
			foreach (KeyValuePair<int, int> keyValuePair in ActivityTipManager.DictAcitvityMinLevle)
			{
				int unionZhuanShengLevel = UIHelper.GetUnionZhuanShengLevel(Global.Data.roleData.Level, Global.Data.roleData.ChangeLifeCount);
				if (unionZhuanShengLevel >= ActivityTipManager.DictAcitvityMinLevle[keyValuePair.Key])
				{
					if (ActivityTipManager.DictActivityTip.ContainsKey(keyValuePair.Key))
					{
						List<ActivityTime> list = ActivityTipManager.DictActivityTip[keyValuePair.Key];
						if (list.Count >= 1)
						{
							for (int i = 0; i < list.Count; i++)
							{
								DateTime dateTimeLeft = list[i].DateTimeLeft;
								DateTime dateTimeRight = list[i].DateTimeRight;
								DateTime correctDateTime = Global.GetCorrectDateTime();
								long ticks = correctDateTime.Ticks;
								if (dateTimeLeft.Year != correctDateTime.Year)
								{
									ActivityTipManager.DictActivityTip.Clear();
									ActivityTipManager.DictActivityTip = null;
									break;
								}
								if (ticks >= dateTimeLeft.Ticks && ticks < dateTimeRight.Ticks)
								{
									num++;
								}
							}
						}
					}
				}
			}
			return num;
		}

		public static void InitActivityItemTree()
		{
			ActivityTipItem activityTipItem = ActivityTipManager.AddActivityTipItem(1000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1001, 1000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1006, 1001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1007, 1001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1005, 1000);
			activityTipItem.LevelRelative = true;
			activityTipItem = ActivityTipManager.AddActivityTipItem(1002, 1000);
			activityTipItem.LevelRelative = true;
			activityTipItem = ActivityTipManager.AddActivityTipItem(13000, 1000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1100, 1000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(2000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(2001, 2000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(2003, 2000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(2002, 2000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(2004, 2000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(5000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(5001, 5000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(5002, 5000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3001, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3008, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3007, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3006, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3009, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3010, 3000);
			if (Global.IsYueKaOpen())
			{
				activityTipItem = ActivityTipManager.AddActivityTipItem(3013, 3000);
			}
			activityTipItem = ActivityTipManager.AddActivityTipItem(3002, 3001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3003, 3001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3004, 3001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3005, 3001);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3011, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3012, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3011, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3012, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(4000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(4001, 4000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(4002, 4000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(4003, 4000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6001, 6000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6002, 6000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6003, 6000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6004, 6000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(6005, 6000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(7000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(7001, 7000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(8000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(9000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(10000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(10001, 10000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(11000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12001, 12000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12002, 12000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12003, 12000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12004, 12000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(12005, 12000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14001, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14002, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14003, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14004, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14005, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14006, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14007, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14008, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14009, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14010, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14011, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14012, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14013, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14014, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14015, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14016, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14017, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14018, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14020, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14021, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14019, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14023, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14028, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14032, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14110, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14114, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15052, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15053, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(11502, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(11501, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15054, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15051, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14033, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14034, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14035, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14027, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(1515, 14000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15001, 15000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15002, 15000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15003, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14111, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14112, 14111);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14113, 14111);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15004, 15000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(16000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(16001, 16000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14100, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14101, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14102, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14103, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14104, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14115, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14105, 3000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(18000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17001, 17000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17002, 17000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17003, 17000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17004, 17000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17005, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17006, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17007, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17008, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17009, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17010, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(17011, 17003);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15050, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15005, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14109, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14107, 14109);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14108, 14109);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14106, 14109);
			activityTipItem = ActivityTipManager.AddActivityTipItem(20000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(20001, 20000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(20002, 20000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15010, 1100);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15011, 1100);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15012, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(18002, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3014, 3000);
			ActivityTipManager.LoadFuBenTabConfig();
			ActivityTipManager.LoadFuBenConfig();
			activityTipItem = ActivityTipManager.AddActivityTipItem(18003, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(30000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(30001, 30000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(31000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(31001, 31000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(31002, 31000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(31003, 31000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(32000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(32001, 32000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(32002, 32000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(32003, 32000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(19000, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(32001, 32000);
			activityTipItem = ActivityTipManager.AddActivityTipItem(7500, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3015, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3036, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(90001, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3037, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(3035, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15055, 0);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15055, 15056);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15055, 15057);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15055, 15058);
			activityTipItem = ActivityTipManager.AddActivityTipItem(15055, 15059);
			activityTipItem = ActivityTipManager.AddActivityTipItem(14115, 0);
		}

		public static int GetFuBenTypeByCopyID(int CopyID)
		{
			int result = 0;
			RiChangFuBenItemData riChangFuBenItemData;
			int num;
			if (ActivityTipManager.RiChangFuBenItemDataDict.TryGetValue(CopyID, ref riChangFuBenItemData))
			{
				num = riChangFuBenItemData.TabID;
			}
			else
			{
				num = -2;
			}
			if (!ActivityTipManager.RiChangFuBenTabTypeDict.TryGetValue(num, ref result))
			{
				result = -1;
			}
			return result;
		}

		public static ActivityTipItem RegActivityTipItem(int type, ActivityTipEventHandler handler)
		{
			ActivityTipItem activityTipItem = null;
			if (ActivityTipManager.ActivityTipItemDict.TryGetValue(type, ref activityTipItem))
			{
				if (handler != null)
				{
					ActivityTipItem activityTipItem2 = activityTipItem;
					activityTipItem2.Handler = (ActivityTipEventHandler)Delegate.Combine(activityTipItem2.Handler, handler);
				}
				else
				{
					activityTipItem.Handler = null;
				}
				if (activityTipItem.Handler != null)
				{
					activityTipItem.Handler(activityTipItem.type, activityTipItem);
				}
			}
			return activityTipItem;
		}

		public static ActivityTipItem UnRegActivityTipItem(int type, ActivityTipEventHandler handler)
		{
			ActivityTipItem activityTipItem = null;
			if (ActivityTipManager.ActivityTipItemDict.TryGetValue(type, ref activityTipItem))
			{
				ActivityTipItem activityTipItem2 = activityTipItem;
				activityTipItem2.Handler = (ActivityTipEventHandler)Delegate.Remove(activityTipItem2.Handler, handler);
				if (activityTipItem.Handler != null)
				{
					activityTipItem.Handler(activityTipItem.type, activityTipItem);
				}
			}
			return activityTipItem;
		}

		public static ActivityTipItem SetActivityTipItemActive(int type, bool active)
		{
			ActivityTipItem activityTipItem = null;
			ActivityCustomTipManager.ExcuteActivityEvent(type, active);
			if (ActivityTipManager.ActivityTipItemDict.TryGetValue(type, ref activityTipItem))
			{
				activityTipItem.IsActive = active;
				if (activityTipItem.Handler != null)
				{
					activityTipItem.Handler(activityTipItem.type, activityTipItem);
				}
				ActivityTipItem parent = activityTipItem.Parent;
				if (parent != null)
				{
					int num = 0;
					for (int i = 0; i < parent.Children.Count; i++)
					{
						if (parent.Children[i].IsActive)
						{
							num++;
							break;
						}
					}
					ActivityTipManager.ChangeItemData(parent, num, false);
				}
			}
			return activityTipItem;
		}

		public static ActivityTipItem GetActivityTipItem(int type)
		{
			ActivityTipItem result = null;
			if (ActivityTipManager.ActivityTipItemDict.TryGetValue(type, ref result))
			{
				return result;
			}
			return null;
		}

		public static RiChangFuBenItemData GetFubenItemData(int fubenID)
		{
			RiChangFuBenItemData result;
			if (ActivityTipManager.RiChangFuBenItemDataDict.TryGetValue(fubenID, ref result))
			{
				return result;
			}
			return new RiChangFuBenItemData();
		}

		public static ActivityTipItem AddActivityTipItem(int type, int parent = 0)
		{
			ActivityTipItem activityTipItem = null;
			if (ActivityTipManager.ActivityTipItemDict.TryGetValue(type, ref activityTipItem))
			{
				return activityTipItem;
			}
			activityTipItem = new ActivityTipItem();
			activityTipItem.type = type;
			ActivityTipManager.ActivityTipItemDict.Add(type, activityTipItem);
			if (parent != 0)
			{
				ActivityTipManager.AddChildItem(parent, activityTipItem);
			}
			return activityTipItem;
		}

		public static void AddChildItem(int parent, ActivityTipItem item)
		{
			ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(parent);
			if (activityTipItem != null)
			{
				item.Parent = activityTipItem;
				if (activityTipItem.Children.Find((ActivityTipItem x) => x.type == item.type) == null)
				{
					activityTipItem.Children.Add(item);
				}
			}
		}

		private static ActivityTipItem GetActivityTipItemByFuBenID(int copyID)
		{
			ActivityTipItem result = null;
			int num;
			if (ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.TryGetValue(copyID, ref num))
			{
				result = ActivityTipManager.GetActivityTipItem(2200 + num);
			}
			return result;
		}

		private static ActivityTipItem GetActivityTipItemByFuBenTab(int tabID)
		{
			ActivityTipItem result = null;
			int num;
			if (ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.TryGetValue(tabID, ref num))
			{
				result = ActivityTipManager.GetActivityTipItem(2100 + num);
			}
			return result;
		}

		public static int GetActivityTipTypeByFuBenID(int copyID)
		{
			int num;
			if (!ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.TryGetValue(copyID, ref num))
			{
				num = 0;
			}
			return 2200 + num;
		}

		public static int GetActivityTipTypeByFuBenTabID(int tabID)
		{
			int num;
			if (!ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.TryGetValue(tabID, ref num))
			{
				num = 0;
			}
			return 2100 + num;
		}

		public static void Reset()
		{
			ActivityTipManager.EnterGameRoleID = 0;
		}

		public static void OnEnterGame()
		{
			if (ActivityTipManager.EnterGameRoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetBossInfoDictData();
				GameInstance.Game.SpriteGetMeditateTimeInfoCmd();
				ActivityTipManager.EnterGameRoleID = Global.Data.roleData.RoleID;
				foreach (RiChangFuBenItemData riChangFuBenItemData in ActivityTipManager.RiChangFuBenItemDataDict.Values)
				{
					if (riChangFuBenItemData.TabID > 0 && riChangFuBenItemData.PreQuery)
					{
						GameInstance.Game.SpriteQureyFuBenInfo(riChangFuBenItemData.MapCode, riChangFuBenItemData.CopyID);
					}
				}
			}
		}

		public static int OnSkillDataListUpdate()
		{
			List<MagicInfoVO> skillListByOccupation = Global.GetSkillListByOccupation(Global.Data.roleData.Occupation);
			if (skillListByOccupation == null || skillListByOccupation.Count <= 0)
			{
				return -1;
			}
			for (int i = 0; i < skillListByOccupation.Count; i++)
			{
				MagicInfoVO magicInfoVO = skillListByOccupation[i];
				int id = magicInfoVO.ID;
				int magicIcon = magicInfoVO.MagicIcon;
				if (magicIcon < 0)
				{
				}
				int actionIndex = magicInfoVO.ActionIndex;
				if (actionIndex < 1000)
				{
					SkillData skillDataByID = Global.GetSkillDataByID(id);
					if (skillDataByID != null)
					{
						if (skillDataByID.UsedNum == 0 && skillDataByID.SkillID == Global.GetBaseSkillID(Global.Data.roleData.Occupation))
						{
							skillDataByID.UsedNum = Global.Data.roleData.NumSkillID;
						}
						if (Global.GetUpLeveConditions(skillDataByID.SkillID, skillDataByID.SkillLevel, skillDataByID.UsedNum, false))
						{
							return skillDataByID.SkillID;
						}
					}
				}
			}
			return -1;
		}

		public static void ServerUpdateIconStateData(ActivityIconStateData data)
		{
			if (data.arrIconState != null)
			{
				foreach (ushort num in data.arrIconState)
				{
					int type = num >> 1;
					int activeCount = (int)(num & 1);
					ActivityTipManager.OnChangeItemCountValue(type, activeCount);
				}
			}
		}

		public static void OnTimer(long ticks)
		{
			if (ticks - ActivityTipManager.LastSecsTicks30 < 30000L)
			{
				return;
			}
			ActivityTipManager.LastSecsTicks30 = ticks;
			int num = ActivityTipManager.OnSkillDataListUpdate();
			if (num > 0)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventAddHintIcon", new int[]
				{
					8,
					num
				}));
			}
		}

		public static void OnLevelChanged(int oldLevel, int newLevel)
		{
			if (Global.Data.roleData.IsFlashPlayer != 0)
			{
				return;
			}
			foreach (KeyValuePair<int, RiChangFuBenItemData> keyValuePair in ActivityTipManager.RiChangFuBenItemDataDict)
			{
				RiChangFuBenItemData value = keyValuePair.Value;
				if (value.TabID > 0)
				{
					value.LevelAllow = (0 == UIHelper.AvalidLevel(value.MinLevel, value.MaxLevel, value.MinZhuanSheng, value.MaxZhuanSheng));
					value.ZhanLiAllow = (value.ZhanLi <= Global.Data.roleData.CombatForce);
					ActivityTipItem activityTipItemByFuBenID = ActivityTipManager.GetActivityTipItemByFuBenID(value.CopyID);
					if (activityTipItemByFuBenID != null)
					{
						int activeCount = ((value.MaxEnterNum >= 0 && value.MaxEnterNum <= value.EnterNum) || (value.MaxFinishNum >= 0 && value.MaxFinishNum <= value.FinishNum) || !value.LevelAllow) ? 0 : 1;
						ActivityTipManager.ChangeItemData(activityTipItemByFuBenID, activeCount, true);
					}
				}
			}
			ActivityTipManager.OnGetBossInfoDict(null, false);
		}

		public static void OnCompleteTask()
		{
			foreach (ActivityTipItem activityTipItem in ActivityTipManager.ActivityTipItemDict.Values)
			{
				if (!activityTipItem.IsActive)
				{
					ActivityTipManager.ChangeItemData(activityTipItem, activityTipItem.ActiveChildCount, true);
				}
			}
		}

		public static void OnQueryFuBenInfoResult(int CopyID, int nClientSec, int nEnterNum, string strName, int nBestTimer, int nFinishNum, int dayID)
		{
			RiChangFuBenItemData riChangFuBenItemData = null;
			int dayOfYear = Global.GetCorrectDateTime().DayOfYear;
			if (dayID != dayOfYear && (dayID < dayOfYear || Math.Abs(dayID - dayOfYear) > 300))
			{
				nEnterNum = 0;
				nFinishNum = 0;
			}
			if (ActivityTipManager.RiChangFuBenItemDataDict.TryGetValue(CopyID, ref riChangFuBenItemData))
			{
				riChangFuBenItemData.EnterNum = nEnterNum;
				riChangFuBenItemData.FinishNum = nFinishNum;
				riChangFuBenItemData.MyTopTime = nClientSec;
				if (nBestTimer > 0)
				{
					riChangFuBenItemData.TopName = strName;
					riChangFuBenItemData.TopTime = nBestTimer;
				}
				riChangFuBenItemData.LevelAllow = (0 == UIHelper.AvalidLevel(riChangFuBenItemData.MinLevel, riChangFuBenItemData.MaxLevel, riChangFuBenItemData.MinZhuanSheng, riChangFuBenItemData.MaxZhuanSheng));
				riChangFuBenItemData.ZhanLiAllow = (riChangFuBenItemData.ZhanLi <= Global.Data.roleData.CombatForce);
				ActivityTipItem activityTipItemByFuBenID = ActivityTipManager.GetActivityTipItemByFuBenID(riChangFuBenItemData.CopyID);
				if (activityTipItemByFuBenID != null)
				{
					int activeCount = ((riChangFuBenItemData.MaxEnterNum >= 0 && riChangFuBenItemData.MaxEnterNum <= riChangFuBenItemData.EnterNum) || (riChangFuBenItemData.MaxFinishNum >= 0 && riChangFuBenItemData.MaxFinishNum <= riChangFuBenItemData.FinishNum) || !riChangFuBenItemData.LevelAllow) ? 0 : 1;
					ActivityTipManager.ChangeItemData(activityTipItemByFuBenID, activeCount, true);
				}
			}
		}

		public static void OnChangeItemCountValue(int type, int activeCount)
		{
			ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(type);
			if (activityTipItem != null)
			{
				ActivityTipManager.ChangeItemData(activityTipItem, activeCount, false);
			}
		}

		public static void OnGetBossInfoDict(Dictionary<int, BossData> bossInfoDict, bool clearOld = true)
		{
			if (Global.Data == null || bossInfoDict == null)
			{
				return;
			}
			Global.Data.BossInfoLastRefreshTime = Global.GetCorrectDateTime();
			if (clearOld)
			{
				Global.Data.BossInfoDict = bossInfoDict;
			}
			else if (bossInfoDict != null)
			{
				foreach (KeyValuePair<int, BossData> keyValuePair in bossInfoDict)
				{
					Global.Data.BossInfoDict[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (Global.Data.ShiJieBossSet == null)
			{
				Global.Data.ShiJieBossSet = ConfigSystemParam.GetSystemParamIntSetByName("BossStaticDataIDForChengJiu", ',');
				Global.Data.HuangJinBossSet = ConfigSystemParam.GetSystemParamIntSetByName("HuangJinBoss", ',');
			}
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<int, BossData> keyValuePair2 in Global.Data.BossInfoDict)
			{
				if (Global.Data.ShiJieBossSet.Contains(keyValuePair2.Key) && string.IsNullOrEmpty(keyValuePair2.Value.NextTime))
				{
					if (Global.GetBossUnionLevel(keyValuePair2.Value.ExtensionID, 0) <= Global.GetUnionLevel(-1, -1))
					{
						num++;
					}
				}
				else if (Global.Data.HuangJinBossSet.Contains(keyValuePair2.Key) && string.IsNullOrEmpty(keyValuePair2.Value.NextTime) && Global.GetBossUnionLevel(keyValuePair2.Value.ExtensionID, 0) <= Global.GetUnionLevel(-1, -1))
				{
					num2++;
				}
			}
			Global.Data.AlivedBossCount = num;
			Global.Data.AlivedHuangJinBossCount = num2;
			int activeCount = Global.Data.AlivedBossCount;
			ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(1002);
			if (activityTipItem != null)
			{
				ActivityTipManager.ChangeItemData(activityTipItem, activeCount, false);
			}
			activeCount = Global.Data.AlivedHuangJinBossCount;
			activityTipItem = ActivityTipManager.GetActivityTipItem(1005);
			if (activityTipItem != null)
			{
				ActivityTipManager.ChangeItemData(activityTipItem, activeCount, false);
			}
		}

		public static void ChangeItemData(ActivityTipItem activityTipItem, int activeCount, bool force = false)
		{
			activityTipItem.ActiveChildCount = activeCount;
			bool flag = !activityTipItem.Hide && (activityTipItem.NeedLevel <= 0 || activityTipItem.NeedLevel <= Global.GetUnionLevel(-1, -1)) && activityTipItem.NeedTaskID <= Global.Data.roleData.CompletedMainTaskID && activityTipItem.NeedVIPLevel <= Global.Data.roleData.VIPLevel && activityTipItem.ActiveChildCount > 0;
			if (activityTipItem.IsActive == flag)
			{
				if (!force)
				{
					return;
				}
			}
			else
			{
				activityTipItem.IsActive = flag;
				if (activityTipItem.Handler != null)
				{
					activityTipItem.Handler(activityTipItem.type, activityTipItem);
				}
			}
			ActivityCustomTipManager.ExcuteActivityEvent(activityTipItem.type, activityTipItem.IsActive);
			ActivityTipItem parent = activityTipItem.Parent;
			if (parent != null)
			{
				int num = 0;
				foreach (ActivityTipItem activityTipItem2 in parent.Children)
				{
					if (activityTipItem2.IsActive)
					{
						num++;
						break;
					}
				}
				ActivityTipManager.ChangeItemData(parent, num, false);
			}
		}

		public static void ClearData()
		{
			ActivityTipManager.RiChangFuBenTabTypeDict.Clear();
			ActivityTipManager.RiChangFuBenItemSequenceTabIDDict.Clear();
			ActivityTipManager.RiChangFuBenItemSequenceCopyIDDict.Clear();
		}

		public static ActivityTipTypes[] ActivityTipFlags = new ActivityTipTypes[]
		{
			ActivityTipTypes.JuQingFuBen,
			ActivityTipTypes.ZuDuiFuBen,
			ActivityTipTypes.RiChangFuBen,
			ActivityTipTypes.RiChangHuoDong,
			ActivityTipTypes.ShiJieBoss,
			ActivityTipTypes.VIPHuoDong
		};

		public static ActivityTipItem ActivityTipItemRoot = new ActivityTipItem();

		public static Dictionary<int, ActivityTipItem> ActivityTipItemDict = new Dictionary<int, ActivityTipItem>();

		private static Dictionary<int, List<ActivityTime>> DictActivityTip = null;

		private static Dictionary<int, int> DictAcitvityMinLevle = null;

		public static Dictionary<int, RiChangFuBenItemData> RiChangFuBenItemDataDict = new Dictionary<int, RiChangFuBenItemData>();

		private static Dictionary<int, int> RiChangFuBenTabTypeDict = new Dictionary<int, int>();

		private static Dictionary<int, int> RiChangFuBenItemSequenceTabIDDict = new Dictionary<int, int>();

		private static Dictionary<int, int> RiChangFuBenItemSequenceCopyIDDict = new Dictionary<int, int>();

		private static int EnterGameRoleID = 0;

		private static long LastSecsTicks30;
	}
}
