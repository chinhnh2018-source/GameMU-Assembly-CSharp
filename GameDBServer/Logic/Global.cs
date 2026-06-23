using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using MySQLDriverCS;
using ProtoBuf;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class Global
	{
		public static string GetXElementNodePath(XElement element)
		{
			string result;
			try
			{
				string text = element.Name.ToString();
				element = element.Parent;
				while (null != element)
				{
					text = element.Name.ToString() + "/" + text;
					element = element.Parent;
				}
				result = text;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		public static XElement GetXElement(XElement XML, string newroot)
		{
			return XML.DescendantsAndSelf(newroot).Single<XElement>();
		}

		public static XElement GetSafeXElement(XElement XML, string newroot)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single<XElement>();
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0} 失败, xml节点名: {1}", newroot, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
		{
			return XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
		}

		public static XElement GetSafeXElement(XElement XML, string newroot, string attribute, string value)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0}/{1}={2} 失败, xml节点名: {3}", new object[]
				{
					newroot,
					attribute,
					value,
					Global.GetXElementNodePath(XML)
				}));
			}
			return result;
		}

		public static XAttribute GetSafeAttribute(XElement XML, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute xattribute = XML.Attribute(attribute);
				if (null == xattribute)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
				result = xattribute;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		public static string GetSafeAttributeStr(XElement XML, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, attribute);
			return (string)safeAttribute;
		}

		public static long GetSafeAttributeLong(XElement XML, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, attribute);
			string text = (string)safeAttribute;
			long result;
			if (text == null || text == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(text);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		public static double GetSafeAttributeDouble(XElement XML, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, attribute);
			string text = (string)safeAttribute;
			double result;
			if (text == null || text == "")
			{
				result = 0.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(text);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		public static XAttribute GetSafeAttribute(XElement XML, string root, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute xattribute = XML.Element(root).Attribute(attribute);
				if (null == xattribute)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
				result = xattribute;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, root, attribute);
			return (string)safeAttribute;
		}

		public static long GetSafeAttributeLong(XElement XML, string root, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, root, attribute);
			string text = (string)safeAttribute;
			long result;
			if (text == null || text == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(text);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		public static double GetSafeAttributeDouble(XElement XML, string root, string attribute)
		{
			XAttribute safeAttribute = Global.GetSafeAttribute(XML, root, attribute);
			string text = (string)safeAttribute;
			double result;
			if (text == null || text == "")
			{
				result = -1.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(text);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		public static void DBRoleInfo2RoleDataEx(DBRoleInfo dbRoleInfo, RoleDataEx roleDataEx)
		{
			lock (dbRoleInfo)
			{
				roleDataEx.PTID = dbRoleInfo.PTID;
				roleDataEx.WorldRoleID = dbRoleInfo.WorldRoleID;
				roleDataEx.Channel = dbRoleInfo.Channel;
				roleDataEx.RoleID = dbRoleInfo.RoleID;
				roleDataEx.RoleName = dbRoleInfo.RoleName;
				roleDataEx.RoleSex = dbRoleInfo.RoleSex;
				roleDataEx.Occupation = dbRoleInfo.Occupation;
				roleDataEx.SubOccupation = dbRoleInfo.SubOccupation;
				roleDataEx.OccupationList = dbRoleInfo.OccupationList;
				roleDataEx.Level = dbRoleInfo.Level;
				roleDataEx.Faction = dbRoleInfo.Faction;
				roleDataEx.Money1 = dbRoleInfo.Money1;
				roleDataEx.Money2 = dbRoleInfo.Money2;
				roleDataEx.Experience = dbRoleInfo.Experience;
				roleDataEx.PKMode = dbRoleInfo.PKMode;
				roleDataEx.PKValue = dbRoleInfo.PKValue;
				string[] array = dbRoleInfo.Position.Split(new char[]
				{
					':'
				});
				if (array.Length == 4)
				{
					roleDataEx.MapCode = Convert.ToInt32(array[0]);
					roleDataEx.RoleDirection = Convert.ToInt32(array[1]);
					roleDataEx.PosX = Convert.ToInt32(array[2]);
					roleDataEx.PosY = Convert.ToInt32(array[3]);
				}
				roleDataEx.LifeV = 0;
				roleDataEx.MagicV = 0;
				roleDataEx.OldTasks = dbRoleInfo.OldTasks;
				roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
				roleDataEx.RolePic = dbRoleInfo.RolePic;
				roleDataEx.BagNum = dbRoleInfo.BagNum;
				roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
				roleDataEx.GoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 0);
				roleDataEx.RebornGoodsStoreList = Global.GetGoodsDataListBySite(dbRoleInfo, 15001);
				roleDataEx.OtherName = dbRoleInfo.OtherName;
				roleDataEx.MainQuickBarKeys = dbRoleInfo.MainQuickBarKeys;
				roleDataEx.OtherQuickBarKeys = dbRoleInfo.OtherQuickBarKeys;
				roleDataEx.LoginNum = dbRoleInfo.LoginNum;
				roleDataEx.LeftFightSeconds = dbRoleInfo.LeftFightSeconds;
				roleDataEx.FriendDataList = dbRoleInfo.FriendDataList;
				roleDataEx.HorsesDataList = dbRoleInfo.HorsesDataList;
				roleDataEx.HorseDbID = dbRoleInfo.HorseDbID;
				roleDataEx.PetsDataList = dbRoleInfo.PetsDataList;
				roleDataEx.PetDbID = dbRoleInfo.PetDbID;
				roleDataEx.InterPower = dbRoleInfo.InterPower;
				roleDataEx.JingMaiDataList = dbRoleInfo.JingMaiDataList;
				roleDataEx.DJPoint = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.DJPoint : 0);
				roleDataEx.DJTotal = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.Total : 0);
				roleDataEx.DJWincnt = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.Wincnt : 0);
				roleDataEx.TotalOnlineSecs = Math.Max(0, dbRoleInfo.TotalOnlineSecs);
				roleDataEx.AntiAddictionSecs = Math.Max(0, dbRoleInfo.AntiAddictionSecs);
				roleDataEx.BiGuanTime = dbRoleInfo.BiGuanTime;
				roleDataEx.YinLiang = dbRoleInfo.YinLiang;
				roleDataEx.SkillDataList = dbRoleInfo.SkillDataList;
				roleDataEx.TotalJingMaiExp = dbRoleInfo.TotalJingMaiExp;
				roleDataEx.JingMaiExpNum = dbRoleInfo.JingMaiExpNum;
				roleDataEx.RegTime = DataHelper.ConvertToTicks(dbRoleInfo.RegTime);
				roleDataEx.LastHorseID = dbRoleInfo.LastHorseID;
				roleDataEx.SaleGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, -1);
				roleDataEx.DefaultSkillID = dbRoleInfo.DefaultSkillID;
				roleDataEx.AutoLifeV = dbRoleInfo.AutoLifeV;
				roleDataEx.AutoMagicV = dbRoleInfo.AutoMagicV;
				roleDataEx.BufferDataList = dbRoleInfo.BufferDataList;
				roleDataEx.MyDailyTaskDataList = dbRoleInfo.MyDailyTaskDataList;
				roleDataEx.MyDailyJingMaiData = dbRoleInfo.MyDailyJingMaiData;
				roleDataEx.NumSkillID = dbRoleInfo.NumSkillID;
				roleDataEx.MyPortableBagData = dbRoleInfo.MyPortableBagData;
				roleDataEx.RebornGirdData = dbRoleInfo.RebornGirdData;
				roleDataEx.MyHuodongData = dbRoleInfo.MyHuodongData;
				roleDataEx.FuBenDataList = dbRoleInfo.FuBenDataList;
				roleDataEx.MainTaskID = dbRoleInfo.MainTaskID;
				roleDataEx.PKPoint = dbRoleInfo.PKPoint;
				roleDataEx.LianZhan = dbRoleInfo.LianZhan;
				roleDataEx.MyRoleDailyData = dbRoleInfo.MyRoleDailyData;
				roleDataEx.KillBoss = dbRoleInfo.KillBoss;
				roleDataEx.MyYaBiaoData = dbRoleInfo.MyYaBiaoData;
				roleDataEx.BattleNameStart = dbRoleInfo.BattleNameStart;
				roleDataEx.BattleNameIndex = dbRoleInfo.BattleNameIndex;
				roleDataEx.CZTaskID = dbRoleInfo.CZTaskID;
				roleDataEx.BattleNum = dbRoleInfo.BattleNum;
				roleDataEx.HeroIndex = dbRoleInfo.HeroIndex;
				roleDataEx.ZoneID = dbRoleInfo.ZoneID;
				roleDataEx.BHName = dbRoleInfo.BHName;
				roleDataEx.BHVerify = dbRoleInfo.BHVerify;
				roleDataEx.BHZhiWu = dbRoleInfo.BHZhiWu;
				roleDataEx.BGDayID1 = dbRoleInfo.BGDayID1;
				roleDataEx.BGMoney = dbRoleInfo.BGMoney;
				roleDataEx.BGDayID2 = dbRoleInfo.BGDayID2;
				roleDataEx.BGGoods = dbRoleInfo.BGGoods;
				roleDataEx.BangGong = dbRoleInfo.BangGong;
				roleDataEx.HuangHou = dbRoleInfo.HuangHou;
				roleDataEx.PaiHangPosDict = PaiHangManager.CalcPaiHangPosDictRoleID(dbRoleInfo.RoleID);
				roleDataEx.JieBiaoDayID = dbRoleInfo.JieBiaoDayID;
				roleDataEx.JieBiaoDayNum = dbRoleInfo.JieBiaoDayNum;
				roleDataEx.VipDailyDataList = dbRoleInfo.VipDailyDataList;
				roleDataEx.YangGongBKDailyJiFen = dbRoleInfo.YangGongBKDailyJiFen;
				roleDataEx.LastMailID = dbRoleInfo.LastMailID;
				roleDataEx.OnceAwardFlag = dbRoleInfo.OnceAwardFlag;
				roleDataEx.Gold = dbRoleInfo.Gold;
				roleDataEx.BanChat = dbRoleInfo.BanChat;
				roleDataEx.BanLogin = dbRoleInfo.BanLogin;
				roleDataEx.IsFlashPlayer = dbRoleInfo.IsFlashPlayer;
				roleDataEx.ChangeLifeCount = dbRoleInfo.ChangeLifeCount;
				roleDataEx.AdmiredCount = dbRoleInfo.AdmiredCount;
				roleDataEx.CombatForce = dbRoleInfo.CombatForce;
				roleDataEx.AutoAssignPropertyPoint = dbRoleInfo.AutoAssignPropertyPoint;
				roleDataEx.GoodsLimitDataList = dbRoleInfo.GoodsLimitDataList;
				roleDataEx.RoleParamsDict = dbRoleInfo.RoleParamsDict;
				roleDataEx.MyPortableBagData.GoodsUsedGridNum = Global.GetGoodsDataCountBySite(dbRoleInfo, -1000);
				roleDataEx.RebornGirdData.GoodsUsedGridNum = Global.GetGoodsDataCountBySite(dbRoleInfo, 15001);
				if (dbRoleInfo.RebornBagNum < 50)
				{
					roleDataEx.RebornBagNum = 50;
					DBManager instance = DBManager.getInstance();
					DBWriter.UpdateRoleRebornBagNum(instance, roleDataEx.RoleID, 50);
				}
				else
				{
					roleDataEx.RebornBagNum = dbRoleInfo.RebornBagNum;
				}
				if (dbRoleInfo.RebornGirdData.ExtGridNum < 60)
				{
					roleDataEx.RebornGirdData.ExtGridNum = 60;
					DBManager instance = DBManager.getInstance();
					DBWriter.UpdateRoleRebornStorageInfo(instance, roleDataEx.RoleID, 60);
				}
				roleDataEx.RebornShowEquip = dbRoleInfo.RebornShowEquip;
				roleDataEx.RebornShowModel = dbRoleInfo.RebornShowModel;
				roleDataEx.ZhanDuiID = dbRoleInfo.ZhanDuiID;
				roleDataEx.ZhanDuiZhiWu = dbRoleInfo.ZhanDuiZhiWu;
				long num = DateTime.Now.Ticks / 10000L;
				int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("anti-addiction-restart", 18000);
				if ((num - dbRoleInfo.LogOffTime) / 1000L >= (long)gameConfigItemInt)
				{
					roleDataEx.AntiAddictionSecs = 0;
				}
				roleDataEx.LastOfflineTime = dbRoleInfo.LogOffTime;
				dbRoleInfo.UpdateDBPositionTicks = num;
				dbRoleInfo.UpdateDBTimeTicks = num;
				dbRoleInfo.UpdateDBInterPowerTimeTicks = num;
				roleDataEx.MyWingData = dbRoleInfo.MyWingData;
				roleDataEx.RolePictureJudgeReferInfo = dbRoleInfo.PictureJudgeReferInfo;
				roleDataEx.RoleStarConstellationInfo = dbRoleInfo.StarConstellationInfo;
				roleDataEx.VIPLevel = Global.GetRoleParamsInt32(dbRoleInfo, "VIPExp");
				roleDataEx.ElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, 3000);
				roleDataEx.UsingElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, 3001);
				roleDataEx.PetList = Global.GetGoodsDataListBySite(dbRoleInfo, 4000);
				roleDataEx.DamonGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 5000);
				roleDataEx.Store_Yinliang = dbRoleInfo.store_yinliang;
				roleDataEx.Store_Money = dbRoleInfo.store_money;
				roleDataEx.LingYuDict = dbRoleInfo.LingYuDict;
				roleDataEx.MagicSwordParam = dbRoleInfo.MagicSwordParam;
				roleDataEx.MyMarriageData = dbRoleInfo.MyMarriageData;
				roleDataEx.MyMarryPartyJoinList = dbRoleInfo.MyMarryPartyJoinList;
				roleDataEx.GroupMailRecordList = dbRoleInfo.GroupMailRecordList;
				roleDataEx.MyGuardStatueDetail = dbRoleInfo.MyGuardStatueDetail;
				roleDataEx.MyHolyItemDataDic = dbRoleInfo.MyHolyItemDataDic;
				roleDataEx.MyTalentData = dbRoleInfo.MyTalentData;
				roleDataEx.TianTiData = dbRoleInfo.TianTiData;
				roleDataEx.FashionGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 6000);
				roleDataEx.OrnamentGoodsList = Global.GetGoodsDataListBySite(dbRoleInfo, 9000);
				roleDataEx.OrnamentDataDict = dbRoleInfo.OrnamentDataDict;
				roleDataEx.MerlinData = dbRoleInfo.MerlinData;
				if (null == roleDataEx.FluorescentGemData)
				{
					roleDataEx.FluorescentGemData = new FluorescentGemData();
				}
				roleDataEx.FluorescentGemData.GemBagList = Global.GetGoodsDataListBySite(dbRoleInfo, 7000);
				roleDataEx.FluorescentGemData.GemEquipList = dbRoleInfo.FluorescentGemData.GemEquipList;
				roleDataEx.FluorescentPoint = dbRoleInfo.FluorescentPoint;
				roleDataEx.BuildingDataList = dbRoleInfo.BuildingDataList;
				roleDataEx.SevenDayActDict = dbRoleInfo.SevenDayActDict;
				roleDataEx.SoulStonesInBag = Global.GetGoodsDataListBySite(dbRoleInfo, 8000);
				roleDataEx.SoulStonesInUsing = Global.GetGoodsDataListBySite(dbRoleInfo, 8001);
				roleDataEx.BanTradeToTicks = dbRoleInfo.BanTradeToTicks;
				roleDataEx.SpecActInfoDict = dbRoleInfo.SpecActInfoDict;
				roleDataEx.TarotData = dbRoleInfo.TarotData;
				roleDataEx.JunTuanZhiWu = dbRoleInfo.JunTuanZhiWu;
				roleDataEx.PaiZhuDamonGoodsDataList = Global.GetGoodsDataListBySiteRange(dbRoleInfo, 10000, 10001);
				roleDataEx.ShenJiDict = dbRoleInfo.ShenJiDict;
				roleDataEx.EverydayActInfoDict = dbRoleInfo.EverydayActInfoDict;
				roleDataEx.HuiJiData = dbRoleInfo.HuiJiData;
				roleDataEx.ArmorData = dbRoleInfo.ArmorData;
				roleDataEx.UserID = dbRoleInfo.UserID;
				roleDataEx.FuWenTabList = dbRoleInfo.FuWenTabList;
				roleDataEx.FuWenGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 11000);
				roleDataEx.AlchemyInfo = dbRoleInfo.AlchemyInfo;
				roleDataEx.JingLingYuanSuJueXingData = dbRoleInfo.JingLingYuanSuJueXingData;
				roleDataEx.BianShenData = dbRoleInfo.BianShenData;
				roleDataEx.RebornGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 15000);
				roleDataEx.RebornCount = Global.GetRoleParamsInt32(dbRoleInfo, "10240");
				roleDataEx.RebornLevel = Global.GetRoleParamsInt32(dbRoleInfo, "10241");
				roleDataEx.RebornExperience = Global.GetRoleParamsInt64(dbRoleInfo, "10242");
				roleDataEx.RebornYinJi = RebornStampManager.GetUserRebornInfoFromCached(dbRoleInfo.RoleID);
				roleDataEx.SpecPriorityActInfoDict = dbRoleInfo.SpecPriorityActInfoDict;
				roleDataEx.HolyGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 16000);
				roleDataEx.RebornEquipHoleData = RebornEquip.GetRebornEquipHoleData(dbRoleInfo);
				roleDataEx.MazingerStoreDataInfo = RebornEquip.GetMazingerStoreData(dbRoleInfo);
			}
		}

		public static void DBRoleInfo2RoleData4Selector(DBManager dbMgr, DBRoleInfo dbRoleInfo, RoleData4Selector roleData4Selector)
		{
			lock (dbRoleInfo)
			{
				roleData4Selector.RoleID = dbRoleInfo.RoleID;
				roleData4Selector.RoleName = dbRoleInfo.RoleName;
				roleData4Selector.RoleSex = dbRoleInfo.RoleSex;
				roleData4Selector.Occupation = dbRoleInfo.Occupation;
				roleData4Selector.Level = dbRoleInfo.Level;
				roleData4Selector.Faction = dbRoleInfo.Faction;
				roleData4Selector.MyWingData = dbRoleInfo.MyWingData;
				roleData4Selector.GoodsDataList = Global.GetUsingGoodsDataList(dbRoleInfo);
				roleData4Selector.OtherName = dbRoleInfo.OtherName;
				roleData4Selector.CombatForce = dbRoleInfo.CombatForce;
				roleData4Selector.AdmiredCount = dbRoleInfo.AdmiredCount;
				roleData4Selector.ZoneId = dbRoleInfo.ZoneID;
				roleData4Selector.OccupationList = dbRoleInfo.OccupationList;
				if (!int.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "FashionWingsID"), out roleData4Selector.FashionWingsID))
				{
					roleData4Selector.FashionWingsID = 0;
				}
				if (!long.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "SettingBitFlags"), out roleData4Selector.SettingBitFlags))
				{
					roleData4Selector.SettingBitFlags = 0L;
				}
				if (!int.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "10213"), out roleData4Selector.SubOccupation))
				{
					roleData4Selector.SubOccupation = 0;
				}
				roleData4Selector.HuiJiData = dbRoleInfo.HuiJiData;
			}
		}

		public static LineData LineItemToLineData(LineItem lineItem)
		{
			return new LineData
			{
				LineID = lineItem.LineID,
				GameServerIP = lineItem.GameServerIP,
				GameServerPort = lineItem.GameServerPort,
				OnlineCount = lineItem.OnlineCount
			};
		}

		public static int SafeConvertToInt32(string str, int fromBase = 10)
		{
			str = str.Trim();
			int result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0;
			}
			else
			{
				try
				{
					return Convert.ToInt32(str, fromBase);
				}
				catch (Exception)
				{
				}
				result = 0;
			}
			return result;
		}

		public static long SafeConvertToInt64(string str, int fromBase = 10)
		{
			str = str.Trim();
			long result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0L;
			}
			else
			{
				try
				{
					return Convert.ToInt64(str, fromBase);
				}
				catch (Exception)
				{
				}
				result = 0L;
			}
			return result;
		}

		public static DateTime SafeConvertToDateTime(string str, DateTime defValue)
		{
			str = str.Trim();
			DateTime result;
			if (string.IsNullOrEmpty(str))
			{
				result = defValue;
			}
			else
			{
				try
				{
					DateTime result2;
					if (DateTime.TryParse(str, out result2))
					{
						return result2;
					}
				}
				catch (Exception)
				{
				}
				result = defValue;
			}
			return result;
		}

		public static double GetOffsetSecond(DateTime date)
		{
			double totalMilliseconds = (date - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return totalMilliseconds / 1000.0;
		}

		public static long GetOffsetMinute(DateTime date)
		{
			return (long)(Global.GetOffsetSecond(date) / 60.0);
		}

		public static int GetOffsetDay(DateTime now)
		{
			double totalMilliseconds = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(totalMilliseconds / 1000.0 / 60.0 / 60.0 / 24.0);
		}

		public static string GetDayStartTime(DateTime now)
		{
			return Global.GetDateTimeString(now.Date);
		}

		public static string GetDayEndTime(DateTime now)
		{
			return Global.GetDateTimeString(now.Date.AddDays(1.0));
		}

		public static string GetDateTimeString(DateTime now)
		{
			return now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime GetRealDate(int day)
		{
			DateTime dateTime = DateTime.Parse("2011-11-11");
			return Global.GetAddDaysDataTime(dateTime, day, true);
		}

		public static string GetSocketRemoteEndPoint(Socket s)
		{
			try
			{
				return string.Format("{0} ", s.RemoteEndPoint);
			}
			catch (Exception)
			{
			}
			return "";
		}

		public static string GetDebugHelperInfo(Socket socket)
		{
			string result;
			if (null == socket)
			{
				result = "socket为null, 无法打印错误信息";
			}
			else
			{
				string text = "";
				try
				{
					text += string.Format("IP={0} ", Global.GetSocketRemoteEndPoint(socket));
				}
				catch (Exception)
				{
				}
				result = text;
			}
			return result;
		}

		public static List<GoodsData> GetGoodsDataListBySite(DBRoleInfo dbRoleInfo, int site)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site == site)
					{
						list.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = list;
			}
			return result;
		}

		public static List<GoodsData> GetGoodsDataListBySiteRange(DBRoleInfo dbRoleInfo, int siteBegin, int siteEnd)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site >= siteBegin && dbRoleInfo.GoodsDataList[i].Site <= siteEnd)
					{
						list.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetGoodsDataCountBySite(DBRoleInfo dbRoleInfo, int site)
		{
			int result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site == site)
					{
						num++;
					}
				}
				result = num;
			}
			return result;
		}

		public static GoodsData GetGoodsDataByDbID(DBRoleInfo dbRoleInfo, int goodsDbID)
		{
			lock (dbRoleInfo)
			{
				if (null != dbRoleInfo.GoodsDataList)
				{
					for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
					{
						if (dbRoleInfo.GoodsDataList[i].Id == goodsDbID)
						{
							return dbRoleInfo.GoodsDataList[i];
						}
					}
				}
			}
			return null;
		}

		public static List<GoodsData> GetUsingGoodsDataList(DBRoleInfo dbRoleInfo)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Using > 0 && dbRoleInfo.GoodsDataList[i].Site != 15000)
					{
						list.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = list;
			}
			return result;
		}

		public static void UpdateGoodsLimitByID(DBRoleInfo dbRoleInfo, int goodsID, int dayID, int usedNum)
		{
			lock (dbRoleInfo)
			{
				if (dbRoleInfo.GoodsLimitDataList == null)
				{
					dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
				}
				int num = -1;
				for (int i = 0; i < dbRoleInfo.GoodsLimitDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsLimitDataList[i].GoodsID == goodsID)
					{
						num = i;
						dbRoleInfo.GoodsLimitDataList[i].DayID = dayID;
						dbRoleInfo.GoodsLimitDataList[i].UsedNum = usedNum;
						break;
					}
				}
				if (-1 == num)
				{
					GoodsLimitData item = new GoodsLimitData
					{
						GoodsID = goodsID,
						DayID = dayID,
						UsedNum = usedNum
					};
					dbRoleInfo.GoodsLimitDataList.Add(item);
				}
			}
		}

		public static HorseData GetHorseDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
		{
			HorseData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.HorsesDataList)
					{
						for (int i = 0; i < dbRoleInfo.HorsesDataList.Count; i++)
						{
							if (dbRoleInfo.HorsesDataList[i].DbID == dbID)
							{
								return dbRoleInfo.HorsesDataList[i];
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static PetData GetPetDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
		{
			PetData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.PetsDataList)
					{
						for (int i = 0; i < dbRoleInfo.PetsDataList.Count; i++)
						{
							if (dbRoleInfo.PetsDataList[i].DbID == dbID)
							{
								return dbRoleInfo.PetsDataList[i];
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static DJPointData GetRoleDJPointData(DBRoleInfo dbRoleInfo)
		{
			DJPointData result = null;
			lock (dbRoleInfo)
			{
				result = new DJPointData
				{
					DbID = dbRoleInfo.RoleDJPointData.DbID,
					RoleID = dbRoleInfo.RoleDJPointData.RoleID,
					DJPoint = dbRoleInfo.RoleDJPointData.DJPoint,
					Total = dbRoleInfo.RoleDJPointData.Total,
					Wincnt = dbRoleInfo.RoleDJPointData.Wincnt,
					Yestoday = dbRoleInfo.RoleDJPointData.Yestoday,
					Lastweek = dbRoleInfo.RoleDJPointData.Lastweek,
					Lastmonth = dbRoleInfo.RoleDJPointData.Lastmonth,
					Dayupdown = dbRoleInfo.RoleDJPointData.Dayupdown,
					Weekupdown = dbRoleInfo.RoleDJPointData.Weekupdown,
					Monthupdown = dbRoleInfo.RoleDJPointData.Monthupdown
				};
			}
			return result;
		}

		public static int GetRoleOnlineState(DBRoleInfo dbRoleInfo)
		{
			int result;
			if (null == dbRoleInfo)
			{
				result = 0;
			}
			else if (dbRoleInfo.ServerLineID <= 0)
			{
				result = 0;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		public static int GetUserOnlineState(DBUserInfo dbUserInfo)
		{
			int result;
			if (null == dbUserInfo)
			{
				result = 0;
			}
			else
			{
				result = UserOnlineManager.GetUserOnlineState(dbUserInfo.UserID);
			}
			return result;
		}

		public static DBRoleInfo FindOnlineRoleInfoByUserInfo(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			DBRoleInfo result;
			if (null == dbUserInfo)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
				{
					int num = dbUserInfo.ListRoleIDs[i];
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null != dbroleInfo)
					{
						if (dbroleInfo.ServerLineID > 0)
						{
							return dbroleInfo;
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static bool IsGameServerClientOnline(int lineId)
		{
			GameServerClient gameServerClient = LineManager.GetGameServerClient(lineId);
			return gameServerClient != null && gameServerClient.CurrentSocket != null && gameServerClient == TCPManager.getInstance().getClient(gameServerClient.CurrentSocket);
		}

		public static string FormatRoleName(DBRoleInfo dbRoleInfo)
		{
			return Global.FormatRoleName(dbRoleInfo.ZoneID, dbRoleInfo.RoleName);
		}

		public static string FormatRoleName(string zoneID, string roleName)
		{
			return roleName;
		}

		public static string FormatRoleName(int zoneID, string roleName)
		{
			return roleName;
		}

		public static string FormatBangHuiName(int zoneID, string bhName)
		{
			return bhName;
		}

		public static int GetDBRoleInfoByZhiWu(List<BangHuiMgrItemData> bangHuiMgrItemDataList, int zhiWu)
		{
			int result;
			if (bangHuiMgrItemDataList == null || bangHuiMgrItemDataList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < bangHuiMgrItemDataList.Count; i++)
				{
					if (bangHuiMgrItemDataList[i].BHZhiwu == zhiWu)
					{
						return bangHuiMgrItemDataList[i].RoleID;
					}
				}
				result = 0;
			}
			return result;
		}

		public static void WriteRoleInfoLog(DBManager dbMgr, DBRoleInfo dbRoleInfo)
		{
			try
			{
				if (null != dbRoleInfo)
				{
					string userID = dbRoleInfo.UserID;
					DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
					if (null != dbuserInfo)
					{
						int money = dbuserInfo.Money;
					}
					if (!string.IsNullOrEmpty(dbRoleInfo.Position))
					{
						string[] array = dbRoleInfo.Position.Split(new char[]
						{
							':'
						});
						if (array.Length == 4)
						{
							int num = Convert.ToInt32(array[0]);
						}
					}
					string text = new DateTime(dbRoleInfo.LastTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string text2 = new DateTime(dbRoleInfo.LogOffTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("将角色的数据写入二进制日志出错，RoleID={0}", dbRoleInfo.RoleID), null, true);
			}
		}

		public static void LoadLangDict()
		{
			XElement xelement = null;
			try
			{
				xelement = XElement.Load("Language.xml");
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				if (null != xelement)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						dictionary[Global.GetSafeAttributeStr(xml, "ChineseText")] = Global.GetSafeAttributeStr(xml, "OtherLangText");
					}
					Global.LangDict = dictionary;
				}
			}
			catch (Exception)
			{
			}
		}

		public static string GetLang(string chineseText)
		{
			string result;
			if (null == Global.LangDict)
			{
				result = chineseText;
			}
			else
			{
				string text = "";
				if (!Global.LangDict.TryGetValue(chineseText, out text))
				{
					result = chineseText;
				}
				else if (string.IsNullOrEmpty(text))
				{
					result = chineseText;
				}
				else
				{
					result = text;
				}
			}
			return result;
		}

		public static List<MailData> LoadUserMailItemDataList(DBManager dbMgr, int rid)
		{
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref rid);
			if (null != dbroleInfo)
			{
				dbroleInfo.LastMailID = 0;
			}
			return DBQuery.GetMailItemDataList(dbMgr, rid);
		}

		public static int LoadUserMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
		{
			return DBQuery.GetMailItemDataCount(dbMgr, rid, excludeReadState, limitCount);
		}

		public static MailData LoadMailItemData(DBManager dbMgr, int rid, int mailID)
		{
			MailData mailItemData = DBQuery.GetMailItemData(dbMgr, rid, mailID);
			if (null != mailItemData)
			{
				if (mailItemData.IsRead != 1)
				{
					DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
					mailItemData.IsRead = 1;
					mailItemData.ReadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			return mailItemData;
		}

		public static bool UpdateHasReadMailFlag(DBManager dbMgr, int rid, int mailID)
		{
			bool result = true;
			DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
			return result;
		}

		public static bool UpdateHasFetchMailGoodsStat(DBManager dbMgr, int rid, int mailID)
		{
			bool result = DBWriter.UpdateMailHasFetchGoodsFlag(dbMgr, mailID, rid);
			DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
			return result;
		}

		public static bool DeleteMail(DBManager dbMgr, int rid, string mailIDs)
		{
			bool result = false;
			string[] array = mailIDs.Split(new char[]
			{
				'|'
			});
			foreach (string s in array)
			{
				try
				{
					int mailID = int.Parse(s);
					bool flag = DBWriter.DeleteMailDataItemExcludeGoodsList(dbMgr, mailID, rid);
					if (flag)
					{
						DBWriter.DeleteMailIDInMailTemp(dbMgr, mailID);
						DBWriter.DeleteMailGoodsList(dbMgr, mailID);
						result = flag;
					}
				}
				catch
				{
				}
			}
			return result;
		}

		public static int AddMail(DBManager dbMgr, string[] fields, out int addGoodsCount)
		{
			int senderrid = Convert.ToInt32(fields[0]);
			string text = fields[1];
			int num = Convert.ToInt32(fields[2]);
			string text2 = fields[3];
			string text3 = fields[4];
			string text4 = fields[5];
			int yinliang = Convert.ToInt32(fields[6]);
			int tongqian = Convert.ToInt32(fields[7]);
			int yuanbao = Convert.ToInt32(fields[8]);
			string text5 = fields[9];
			if (text2 == "")
			{
				string text6 = "";
				Global.GetRoleNameAndUserID(dbMgr, num, out text2, out text6);
			}
			text = text.Replace('$', ':');
			text2 = text2.Replace('$', ':');
			text3 = text3.Replace('$', ':');
			text4 = text4.Replace('$', ':');
			addGoodsCount = 0;
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref senderrid);
			if (null != dbroleInfo)
			{
				text = Global.FormatRoleName(dbroleInfo);
			}
			int num2 = DBWriter.AddMailBody(dbMgr, senderrid, text, num, text2, text3, text4, yinliang, tongqian, yuanbao);
			if (num2 >= 0)
			{
				addGoodsCount = Global.AddMailGoods(dbMgr, num2, text5.Split(new char[]
				{
					'|'
				}));
				DBWriter.UpdateLastScanMailID(dbMgr, num, num2);
			}
			return num2;
		}

		private static int AddMailGoods(DBManager dbMgr, int mailid, string[] goodsArr)
		{
			int result;
			if (goodsArr == null || goodsArr.Length <= 0)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < goodsArr.Length; i++)
				{
					string[] array = goodsArr[i].Split(new char[]
					{
						'_'
					});
					if (16 == array.Length)
					{
						if (DBWriter.AddMailGoodsDataItem(dbMgr, mailid, int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), array[3], int.Parse(array[4]), int.Parse(array[5]), int.Parse(array[6]), array[7], int.Parse(array[8]), int.Parse(array[9]), int.Parse(array[10]), int.Parse(array[11]), int.Parse(array[12]), int.Parse(array[13]), int.Parse(array[14]), int.Parse(array[15])))
						{
							num++;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static int FindDBRoleID(DBManager dbMgr, string roleName)
		{
			int num = dbMgr.DBRoleMgr.FindDBRoleID(roleName);
			if (num < 0)
			{
				try
				{
					string roleName2 = "";
					int num2 = -1;
					Global.GetRoleRealNameAndZoneID(roleName, out roleName2, out num2);
					if (num2 >= 0)
					{
						num = DBQuery.GetRoleIDByRoleName(dbMgr, roleName2, num2);
					}
				}
				catch
				{
				}
			}
			return num;
		}

		public static void GetRoleRealNameAndZoneID(string inRoleName, out string outRoleName, out int zoneID)
		{
			outRoleName = "";
			zoneID = -1;
			if (inRoleName.IndexOf('[') == 0 && inRoleName.IndexOf(']') >= 1)
			{
				string input = inRoleName.Substring(1);
				Regex regex = new Regex("^[\\d]+");
				MatchCollection matchCollection = regex.Matches(input);
				if (matchCollection.Count > 0)
				{
					zoneID = int.Parse(matchCollection[0].Value);
				}
				int startIndex = inRoleName.IndexOf(']') + 1;
				outRoleName = inRoleName.Substring(startIndex);
			}
		}

		public static int TransMoneyToYuanBao(int money)
		{
			int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
			return money * gameConfigItemInt;
		}

		public static string GetHuoDongKeyString(string fromDate, string toDate)
		{
			return string.Format("{0}_{1}", fromDate, toDate);
		}

		public static bool IsInActivityPeriod(string fromDate, string toDate)
		{
			try
			{
				if (DateTime.Now >= DateTime.Parse(fromDate) && DateTime.Now <= DateTime.Parse(toDate))
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		public static bool GetUserMaxLevelRole(DBManager dbMgr, string userID, out string maxLevelRoleName, out int maxLevelRoleZoneID)
		{
			maxLevelRoleName = "";
			maxLevelRoleZoneID = 1;
			DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
			if (null != dbuserInfo)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				for (int i = 0; i < dbuserInfo.ListRoleChangeLifeCount.Count; i++)
				{
					if (dbuserInfo.ListRoleChangeLifeCount[i] > num3)
					{
						num3 = dbuserInfo.ListRoleChangeLifeCount[i];
						num = dbuserInfo.ListRoleLevels[i];
						num2 = i;
					}
					else if (dbuserInfo.ListRoleChangeLifeCount[i] == num3)
					{
						if (num < dbuserInfo.ListRoleLevels[i])
						{
							num = dbuserInfo.ListRoleLevels[i];
							num2 = i;
						}
					}
				}
				if (num2 >= 0 && num2 < dbuserInfo.ListRoleNames.Count)
				{
					maxLevelRoleName = dbuserInfo.ListRoleNames[num2];
				}
				if (num2 >= 0 && num2 < dbuserInfo.ListRoleZoneIDs.Count)
				{
					maxLevelRoleZoneID = dbuserInfo.ListRoleZoneIDs[num2];
				}
			}
			return true;
		}

		public static bool GetRoleNameAndUserID(DBManager dbMgr, int rid, out string maxLevelRoleName, out string userID)
		{
			maxLevelRoleName = "";
			userID = "";
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref rid);
			if (null != dbroleInfo)
			{
				maxLevelRoleName = Global.FormatRoleName(dbroleInfo);
				userID = dbroleInfo.UserID;
			}
			return true;
		}

		public static List<HuoDongPaiHangData> GetPaiHangItemListByHuoDongLimit(DBManager dbMgr, List<int> minGateValueList, int activityType, string midDate, int maxPaiHang = 10)
		{
			List<HuoDongPaiHangData> activityPaiHangListNearMidTime = DBQuery.GetActivityPaiHangListNearMidTime(dbMgr, activityType, midDate, maxPaiHang);
			List<HuoDongPaiHangData> list = new List<HuoDongPaiHangData>();
			int num = 0;
			for (int i = 0; i < activityPaiHangListNearMidTime.Count; i++)
			{
				HuoDongPaiHangData huoDongPaiHangData = activityPaiHangListNearMidTime[i];
				huoDongPaiHangData.PaiHang = -1;
				for (int j = num; j < minGateValueList.Count; j++)
				{
					if (huoDongPaiHangData.PaiHangValue >= minGateValueList[j])
					{
						huoDongPaiHangData.PaiHang = j + 1;
						list.Add(huoDongPaiHangData);
						num = huoDongPaiHangData.PaiHang;
						break;
					}
				}
				if (huoDongPaiHangData.PaiHang < 0 || huoDongPaiHangData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			return list;
		}

		public static List<InputKingPaiHangData> GetInputKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
		{
			RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, minGateValueList);
			RankData rankData = GameDBManager.RankCacheMgr.GetRankData(key, minGateValueList, maxPaiHang);
			List<InputKingPaiHangData> result;
			if (null == rankData)
			{
				result = null;
			}
			else
			{
				List<InputKingPaiHangData> list = GameDBManager.RankCacheMgr.GetRankDataList(rankData);
				if (null == list)
				{
					list = new List<InputKingPaiHangData>();
				}
				result = list;
			}
			return result;
		}

		public static List<InputKingPaiHangData> GetUsedMoneyKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
		{
			RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, minGateValueList);
			RankData rankData = GameDBManager.RankCacheMgr.GetRankData(key, minGateValueList, maxPaiHang);
			List<InputKingPaiHangData> result;
			if (null == rankData)
			{
				result = null;
			}
			else
			{
				List<InputKingPaiHangData> list = GameDBManager.RankCacheMgr.GetRankDataList(rankData);
				if (null == list)
				{
					list = new List<InputKingPaiHangData>();
				}
				result = list;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessHuoDongForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int num = Convert.ToInt32(fields[0]);
			string fromDate = fields[1].Replace('$', ':');
			string text = fields[2].Replace('$', ':');
			string[] array = fields[3].Split(new char[]
			{
				'_'
			});
			List<int> list = new List<int>();
			foreach (string str in array)
			{
				list.Add(Global.SafeConvertToInt32(str, 10));
			}
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
			TCPProcessCmdResults result;
			if (null == dbroleInfo)
			{
				string data = string.Format("{0}:{1}:0", -1001, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				string midDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, text))
				{
					midDate = text;
				}
				List<HuoDongPaiHangData> paiHangItemListByHuoDongLimit = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, list, activityType, midDate, 10);
				int num2 = -1;
				for (int j = 0; j < paiHangItemListByHuoDongLimit.Count; j++)
				{
					if (paiHangItemListByHuoDongLimit[j] != null && dbroleInfo.RoleID == paiHangItemListByHuoDongLimit[j].RoleID)
					{
						num2 = paiHangItemListByHuoDongLimit[j].PaiHang;
						break;
					}
				}
				if (num2 <= 0)
				{
					string data = string.Format("{0}:{1}:0", -10007, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, text);
					int num3 = 0;
					string text2 = "";
					string data;
					lock (dbroleInfo)
					{
						DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, activityType, huoDongKeyString, out num3, out text2);
						if (num3 > 0)
						{
							data = string.Format("{0}:{1}:0", -10005, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int num4 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, activityType, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num4 < 0)
						{
							data = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					data = string.Format("{0}:{1}:{2}", 1, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}

		public static TCPProcessCmdResults GetHuoDongPaiHangForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int num = Convert.ToInt32(fields[0]);
			string fromDate = fields[1].Replace('$', ':');
			string text = fields[2].Replace('$', ':');
			string[] array = fields[3].Split(new char[]
			{
				'_'
			});
			List<int> list = new List<int>();
			foreach (string str in array)
			{
				list.Add(Global.SafeConvertToInt32(str, 10));
			}
			string midDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (!Global.IsInActivityPeriod(fromDate, text))
			{
				midDate = text;
			}
			List<HuoDongPaiHangData> paiHangItemListByHuoDongLimit = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, list, activityType, midDate, 10);
			tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HuoDongPaiHangData>>(paiHangItemListByHuoDongLimit, pool, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static void LogAndExitProcess(string error)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			File.AppendAllText("error.log", error + "\r\n");
			Console.WriteLine(error);
			Console.WriteLine("本程序将自动退出");
			Console.ForegroundColor = ConsoleColor.White;
			for (int i = 30; i > 0; i--)
			{
				Console.Write("\b\b" + i.ToString("00"));
				Thread.Sleep(600);
			}
			Process.GetCurrentProcess().Kill();
		}

		public static bool InitDBAutoIncrementValues(DBManager dbManger)
		{
			int num = GameDBManager.ZoneID * GameDBManager.DBAutoIncreaseStepValue;
			bool result;
			if (num < 0)
			{
				result = false;
			}
			else
			{
				int val = DBQuery.GetMaxRoleID(dbManger) + 1;
				int num2 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_roles", Math.Max(num, val));
				val = DBQuery.GetMaxMailID(dbManger) + 1;
				int num3 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_mail", Math.Max(num, val));
				val = DBQuery.GetMaxBangHuiID(dbManger) + 1;
				int num4 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_banghui", Math.Max(num, val));
				if (0 != num2)
				{
					Console.WriteLine("更新数据库表t_roles自增长字段出错");
				}
				if (0 != num3)
				{
					Console.WriteLine("更新数据库表t_mail自增长字段出错");
				}
				if (0 != num4)
				{
					Console.WriteLine("更新数据库表t_banghui自增长字段出错");
				}
				result = (num2 == 0 && num3 == 0 && 0 == num4);
			}
			return result;
		}

		public static void UpdateRoleParamByName(DBManager dbMgr, DBRoleInfo dbRoleInfo, string name, string value, RoleParamType roleParamType = null)
		{
			if (roleParamType == null)
			{
				roleParamType = RoleParamNameInfo.GetRoleParamType(name, value);
			}
			bool flag = DBWriter.UpdateRoleParams(dbMgr, dbRoleInfo.RoleID, name, value, roleParamType);
			lock (dbRoleInfo)
			{
				RoleParamsData roleParamsData = null;
				if (!dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
				{
					roleParamsData = new RoleParamsData
					{
						ParamName = name,
						ParamValue = value,
						ParamType = roleParamType
					};
					dbRoleInfo.RoleParamsDict[name] = roleParamsData;
				}
				else
				{
					roleParamsData.ParamValue = value;
				}
				if (flag)
				{
					roleParamsData.UpdateFaildTicks = 0L;
				}
				else
				{
					roleParamsData.UpdateFaildTicks = TimeUtil.NOW();
				}
				if (name == "20017" || name == "10213")
				{
					DBRoleInfo.InitFromRoleParams(dbRoleInfo);
				}
			}
		}

		public static long ModifyRoleParamLongByName(DBManager dbMgr, DBRoleInfo dbRoleInfo, string name, long value, RoleParamType roleParamType = null)
		{
			value += Global.GetRoleParamsInt64(dbRoleInfo, name);
			Global.UpdateRoleParamByName(dbMgr, dbRoleInfo, name, value.ToString(), roleParamType);
			return value;
		}

		public static string GetRoleParamByName(DBRoleInfo dbRoleInfo, string name)
		{
			lock (dbRoleInfo)
			{
				RoleParamsData roleParamsData = null;
				if (dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
				{
					return roleParamsData.ParamValue;
				}
			}
			return null;
		}

		public static int GetRoleParamsInt32(DBRoleInfo dbRoleInfo, string name)
		{
			string roleParamByName = Global.GetRoleParamByName(dbRoleInfo, name);
			int result;
			if (roleParamByName == null || roleParamByName.Length <= 0)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(roleParamByName, 10);
			}
			return result;
		}

		public static long GetRoleParamsInt64(DBRoleInfo dbRoleInfo, string name)
		{
			string roleParamByName = Global.GetRoleParamByName(dbRoleInfo, name);
			long result;
			if (roleParamByName == null || roleParamByName.Length <= 0)
			{
				result = 0L;
			}
			else
			{
				result = Global.SafeConvertToInt64(roleParamByName, 10);
			}
			return result;
		}

		public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId, int actStartDay)
		{
			lock (Global.QiangGouMutex)
			{
				int num = DBWriter.AddNewQiangGouBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney, qiangGouId, actStartDay);
				if (num < 0)
				{
					return -10001;
				}
			}
			return 1;
		}

		public static void QueryQiangGouBuyItemInfo(DBManager dbMgr, int roleID, int goodsID, int qiangGouId, int random, int actStartDay, out int roleBuyNum, out int totalBuyNum)
		{
			lock (Global.QiangGouMutex)
			{
				roleBuyNum = DBQuery.QueryQiangGouBuyItemNumByRoleID(dbMgr, roleID, goodsID, qiangGouId, random, actStartDay);
				totalBuyNum = DBQuery.QueryQiangGouBuyItemNum(dbMgr, goodsID, qiangGouId, random, actStartDay);
			}
		}

		public static int GetBitValue(int whichOne)
		{
			return (int)Math.Pow(2.0, (double)(whichOne - 1));
		}

		public static long GetBitRangeValue(long value, int startBit, int count)
		{
			long result;
			if (count > 63)
			{
				result = 0L;
			}
			else if (startBit < 0)
			{
				result = 0L;
			}
			else if (startBit < 0)
			{
				result = 0L;
			}
			else
			{
				long num = (long)Math.Pow(2.0, (double)count) - 1L;
				long maxValue = long.MaxValue;
				if (startBit != 0)
				{
					value >>= 1;
					value &= maxValue;
					value >>= startBit - 1;
				}
				result = (value & num);
			}
			return result;
		}

		public static DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
		{
			if (roundDay)
			{
				dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			}
			return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
		}

		public static WingData GetWingData(DBRoleInfo dbRoleInfo)
		{
			WingData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					result = dbRoleInfo.MyWingData;
				}
			}
			return result;
		}

		public static TCPProcessCmdResults SaveConsumeLog(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Global.SafeConvertToInt32(array[0], 10);
				int num2 = Global.SafeConvertToInt32(array[1], 10);
				string cdate = DateTime.Now.ToString("yyyy-MM-dd");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateCityInfoItem(dbMgr, dbroleInfo.LastIP, dbroleInfo.UserID, "usedmoney", num2);
				int num3 = DBWriter.SaveConsumeLog(dbMgr, num, cdate, num2);
				if (0 <= num3)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					Global.AddTotalUsedMoney(dbMgr, num, num2);
					GameDBManager.RankCacheMgr.OnUserDoSomething(num, RankType.Consume, num2);
				}
				else
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static string GetLocalAddressIPs()
		{
			string text = "";
			try
			{
				foreach (IPAddress ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
				{
					if (ipaddress.AddressFamily.ToString() == "InterNetwork")
					{
						if (text == "")
						{
							text = ipaddress.ToString();
						}
						else
						{
							text = text + "_" + ipaddress.ToString();
						}
					}
				}
			}
			catch
			{
			}
			return text;
		}

		public static string GetInternalIP()
		{
			string result = "?";
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ipaddress in hostEntry.AddressList)
				{
					if (ipaddress.AddressFamily.ToString() == "InterNetwork")
					{
						string[] array = ipaddress.ToString().Split(new char[]
						{
							'.'
						});
						if (array[0] == "10" || array[0] == "192")
						{
							result = ipaddress.ToString();
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		public static int GetTotalUsedMoney(DBManager dbMgr, int roleID)
		{
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
			int result;
			if (null == dbroleInfo)
			{
				result = 0;
			}
			else
			{
				string roleParamByName = DBQuery.GetRoleParamByName(dbMgr, roleID, "TotalCostMoney");
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				int num;
				if (array == null || array.Length != 2)
				{
					num = DBQuery.GetUserUsedMoney(dbMgr, roleID, dbroleInfo.RegTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					string value = string.Format("{0},{1}", 1, num);
					DBWriter.UpdateRoleParams(dbMgr, roleID, "TotalCostMoney", value, null);
				}
				else
				{
					num = Convert.ToInt32(array[1]);
				}
				result = num;
			}
			return result;
		}

		public static void AddTotalUsedMoney(DBManager dbMgr, int roleID, int addMoney)
		{
			DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
			if (null != dbroleInfo)
			{
				lock (Global.addtotalusedmoney_mutex)
				{
					string roleParamByName = DBQuery.GetRoleParamByName(dbMgr, roleID, "TotalCostMoney");
					string[] array = roleParamByName.Split(new char[]
					{
						','
					});
					if (array != null && array.Length == 2)
					{
						int num = Convert.ToInt32(array[1]);
						string value = string.Format("{0},{1}", 1, num + addMoney);
						DBWriter.UpdateRoleParams(dbMgr, roleID, "TotalCostMoney", value, null);
					}
				}
			}
		}

		public static List<uint> ParseRoleparamStreamValueToList(string value)
		{
			List<uint> list = new List<uint>();
			List<uint> result;
			if (string.IsNullOrEmpty(value))
			{
				result = list;
			}
			else
			{
				byte[] bytes = Convert.FromBase64String(value);
				value = Encoding.GetEncoding("latin1").GetString(bytes);
				int num = 0;
				for (int i = 0; i < value.Length; i += 4)
				{
					byte[] bytes2 = Encoding.GetEncoding("latin1").GetBytes(value.Substring(num, 4));
					list.Add(BitConverter.ToUInt32(bytes2, 0));
					num += 4;
				}
				result = list;
			}
			return result;
		}

		public static string ParseListToRoleparamStreamValue(List<uint> lsUint)
		{
			string text = "";
			for (int i = 0; i < lsUint.Count; i++)
			{
				byte[] bytes = BitConverter.GetBytes(lsUint[i]);
				text += Encoding.GetEncoding("latin1").GetString(bytes);
			}
			byte[] bytes2 = Encoding.GetEncoding("latin1").GetBytes(text);
			return Convert.ToBase64String(bytes2);
		}

		public static int[] StringArray2IntArray(string[] sa)
		{
			int[] array = new int[sa.Length];
			for (int i = 0; i < sa.Length; i++)
			{
				string text = sa[i].Trim();
				text = (string.IsNullOrEmpty(text) ? "0" : text);
				array[i] = Convert.ToInt32(text);
			}
			return array;
		}

		public static List<GoodsData> ParseGoodsDataList(string strGoodIDs)
		{
			string[] array = strGoodIDs.Split(new char[]
			{
				'|'
			});
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("ParseGMailGoodsDataList解析{0}中第{1}个的奖励项时失败, 物品配置项个数错误", strGoodIDs, i), null, true);
				}
				else
				{
					int[] array3 = Global.StringArray2IntArray(array2);
					GoodsData item = new GoodsData
					{
						GoodsID = array3[0],
						GCount = array3[1],
						Forge_level = array3[3],
						Binding = array3[2],
						Lucky = array3[5],
						ExcellenceInfo = array3[6],
						AppendPropLev = array3[4]
					};
					list.Add(item);
				}
			}
			return list;
		}

		public static Encoding GetSysEncoding()
		{
			Encoding result;
			if ("utf8" == DBConnections.dbNames.ToLower())
			{
				result = Encoding.UTF8;
			}
			else
			{
				result = Encoding.Default;
			}
			return result;
		}

		public static void CheckCodes()
		{
			bool flag = true;
			flag &= Global.CheckAllProtoBufContract();
			flag &= Global.CheckDuplicateEnum(typeof(TCPGameServerCmds));
			flag &= Global.CheckDuplicateEnum(typeof(SaleGoodsConsts));
			flag &= Global.CheckDuplicateEnum(typeof(ActivityTypes));
			if (!(flag & Global.CheckDuplicateFieldValue(typeof(RoleParamName))))
			{
				Console.WriteLine("代码检查发现问题,请尽快反馈给研发部\n\n");
			}
		}

		public static bool CheckDuplicateEnum(Type type)
		{
			bool result = true;
			HashSet<int> hashSet = new HashSet<int>();
			Array enumValues = type.GetEnumValues();
			foreach (object obj in enumValues)
			{
				int num = (int)obj;
				if (!hashSet.Add(num) && !Global.DontCheckEnumNames.Contains(obj.ToString()))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("枚举类型[{0}]定义常量值[{1}]重复", type.ToString(), num), null, true);
					result = false;
				}
			}
			return result;
		}

		public static bool CheckDuplicateFieldValue(Type type)
		{
			bool result = true;
			HashSet<string> hashSet = new HashSet<string>();
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				string text = fieldInfo.GetValue(null).ToString();
				if (!hashSet.Add(text))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("类型[{0}]定义常量值[{1}]重复", type.ToString(), text), null, true);
					result = false;
				}
			}
			return result;
		}

		public static bool CheckAllProtoBufContract()
		{
			bool result = true;
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				bool flag = false;
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				object[] customAttributes = type.GetCustomAttributes(typeof(ProtoContractAttribute), false);
				if (customAttributes.Length > 0)
				{
					foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
					{
						if (memberInfo.MemberType == MemberTypes.Constructor)
						{
							ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
							if (null != constructorInfo)
							{
								if (constructorInfo.GetParameters().Length == 0)
								{
									flag = true;
								}
							}
						}
						else if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
						{
							object[] customAttributes2 = memberInfo.GetCustomAttributes(typeof(ProtoMemberAttribute), false);
							foreach (object obj in customAttributes2)
							{
								ProtoMemberAttribute protoMemberAttribute = obj as ProtoMemberAttribute;
								if (null != protoMemberAttribute)
								{
									string a;
									if (dictionary.TryGetValue(protoMemberAttribute.Tag, out a) && a != memberInfo.Name)
									{
										result = false;
										Console.WriteLine("Protobuf定义{2}的Tag值{0}有重复:{1}", protoMemberAttribute.Tag, memberInfo.Name, type.ToString());
										break;
									}
									dictionary.Add(protoMemberAttribute.Tag, memberInfo.Name);
								}
							}
						}
					}
					if (!flag)
					{
						result = false;
						Console.WriteLine("Protobuf要求{0}结构必须有默认构造函数", type.ToString());
					}
				}
			}
			return result;
		}

		public static string GCC(int formatId, params object[] args)
		{
			string format = Global.FormatArray[formatId];
			string text = string.Format(format, args);
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = "";
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				MD5 md = MD5.Create();
				byte[] array = md.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder(32);
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("X2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static bool CCC(string cc, int formatId, params object[] args)
		{
			string format = Global.FormatArray[formatId];
			string text = string.Format(format, args);
			string b = Global.GCC(formatId, args);
			return cc == b;
		}

		public static bool AddCC(string cc)
		{
			bool result;
			lock (Global.CCHashSet1)
			{
				result = Global.CCHashSet1.Add(cc);
			}
			return result;
		}

		public static bool ProcessGMMsg(string[] args)
		{
			bool result;
			if (args[0] == "-charge")
			{
				if (args.Length >= 6)
				{
					UserMoneyMgr.GMAddCharge(args[1], args[2], args[3], args[4], args[5].Replace('$', ':'));
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public const int MaxFuMoMailNum = 50;

		public static object BangHuiMutex = new object();

		private static Dictionary<string, string> LangDict = null;

		private static Dictionary<string, List<InputKingPaiHangData>> dictCachingInputMoneyKingPaiHangListByHuoDongLimit = new Dictionary<string, List<InputKingPaiHangData>>();

		private static Dictionary<string, long> dictInputMoneyKingPaiHangListByHuoDongLimitHour = new Dictionary<string, long>();

		private static object CachingInputMoneyKingPaiHangLock = new object();

		private static Dictionary<string, List<InputKingPaiHangData>> dictCachingUsedMoneyKingPaiHangListByHuoDongLimit = new Dictionary<string, List<InputKingPaiHangData>>();

		private static Dictionary<string, long> dictUsedMoneyKingPaiHangListByHuoDongLimitHour = new Dictionary<string, long>();

		private static object CachingUsedMoneyKingPaiHangLock = new object();

		public static object QiangGouMutex = new object();

		private static object addtotalusedmoney_mutex = new object();

		public static string[] DontCheckEnumNames = new string[]
		{
			"Max",
			"Max_Configed"
		};

		private static readonly string[] FormatArray = new string[]
		{
			"",
			"j0U8l>.fjoean13fw16d2lf3s13e5.*{0}XX",
			"jOU81>.fjoean13fw16d2lf3s13e5.*{0}YY{1}",
			"jOU8l>.fjofw16d21f3s13e5.*{0}YY{1}.ean13{2}",
			"jOU81>.fjoeanl3fw16d21f.*{0}YY{1}3sl3e5.{2}={3}",
			"jOU81>.fjoeanl3fw16d2.*{0}YY{1}.{2}={3}1f3sl3e5-{4}"
		};

		private static HashSet<string> CCHashSet1 = new HashSet<string>();

		public delegate T SQLDelegate<T>(MySQLDataReader reader);
	}
}
