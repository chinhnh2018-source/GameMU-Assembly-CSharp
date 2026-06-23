using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class CGetOldResourceManager
	{
		public static XElement xmlData
		{
			get
			{
				lock (CGetOldResourceManager._xmlDataMutex)
				{
					if (CGetOldResourceManager._xmlData != null)
					{
						return CGetOldResourceManager._xmlData;
					}
				}
				XElement xmlData = null;
				try
				{
					string uri = "Config/ZiYuanZhaoHui.xml";
					xmlData = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				}
				catch (Exception ex)
				{
					xmlData = null;
					LogManager.WriteException(ex.ToString());
				}
				lock (CGetOldResourceManager._xmlDataMutex)
				{
					CGetOldResourceManager._xmlData = xmlData;
				}
				return CGetOldResourceManager._xmlData;
			}
		}

		public static double[] ExpGold
		{
			get
			{
				double[] exp;
				if (CGetOldResourceManager._Exp != null)
				{
					exp = CGetOldResourceManager._Exp;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiExp", ',');
					lock (CGetOldResourceManager._ExpMutex)
					{
						CGetOldResourceManager._Exp = paramValueDoubleArrayByName;
					}
					exp = CGetOldResourceManager._Exp;
				}
				return exp;
			}
		}

		public static double[] BondGold
		{
			get
			{
				double[] bondGold;
				if (CGetOldResourceManager._BondGold != null)
				{
					bondGold = CGetOldResourceManager._BondGold;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiBandGold", ',');
					lock (CGetOldResourceManager._BondGoldMutex)
					{
						CGetOldResourceManager._BondGold = paramValueDoubleArrayByName;
					}
					bondGold = CGetOldResourceManager._BondGold;
				}
				return bondGold;
			}
		}

		public static double[] MoJing
		{
			get
			{
				double[] moJing;
				if (CGetOldResourceManager._MoJing != null)
				{
					moJing = CGetOldResourceManager._MoJing;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiMoJing", ',');
					lock (CGetOldResourceManager._MoJingMutex)
					{
						CGetOldResourceManager._MoJing = paramValueDoubleArrayByName;
					}
					moJing = CGetOldResourceManager._MoJing;
				}
				return moJing;
			}
		}

		public static double[] ShengWang
		{
			get
			{
				double[] shengWang;
				if (CGetOldResourceManager._ShengWang != null)
				{
					shengWang = CGetOldResourceManager._ShengWang;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiShengWang", ',');
					lock (CGetOldResourceManager._ShengWangMutex)
					{
						CGetOldResourceManager._ShengWang = paramValueDoubleArrayByName;
					}
					shengWang = CGetOldResourceManager._ShengWang;
				}
				return shengWang;
			}
		}

		public static double[] ChengJiu
		{
			get
			{
				double[] chengJiu;
				if (CGetOldResourceManager._ChengJiu != null)
				{
					chengJiu = CGetOldResourceManager._ChengJiu;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiChengJiu", ',');
					lock (CGetOldResourceManager._ChengJiuMutex)
					{
						CGetOldResourceManager._ChengJiu = paramValueDoubleArrayByName;
					}
					chengJiu = CGetOldResourceManager._ChengJiu;
				}
				return chengJiu;
			}
		}

		public static double[] ZhanGong
		{
			get
			{
				double[] zhanGong;
				if (CGetOldResourceManager._ZhanGong != null)
				{
					zhanGong = CGetOldResourceManager._ZhanGong;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiZhanGong", ',');
					lock (CGetOldResourceManager._ZhanGongMutex)
					{
						CGetOldResourceManager._ZhanGong = paramValueDoubleArrayByName;
					}
					zhanGong = CGetOldResourceManager._ZhanGong;
				}
				return zhanGong;
			}
		}

		public static double[] BangZuan
		{
			get
			{
				double[] bangZuan;
				if (CGetOldResourceManager._BangZuan != null)
				{
					bangZuan = CGetOldResourceManager._BangZuan;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiBindZuan", ',');
					lock (CGetOldResourceManager._BangZuanMutex)
					{
						CGetOldResourceManager._BangZuan = paramValueDoubleArrayByName;
					}
					bangZuan = CGetOldResourceManager._BangZuan;
				}
				return bangZuan;
			}
		}

		public static double[] XingHun
		{
			get
			{
				double[] xingHun;
				if (CGetOldResourceManager._XingHun != null)
				{
					xingHun = CGetOldResourceManager._XingHun;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiXingHun", ',');
					lock (CGetOldResourceManager._XingHunMutex)
					{
						CGetOldResourceManager._XingHun = paramValueDoubleArrayByName;
					}
					xingHun = CGetOldResourceManager._XingHun;
				}
				return xingHun;
			}
		}

		public static double[] YuanSuFenMo
		{
			get
			{
				double[] yuanSuFenMo;
				if (CGetOldResourceManager._YuanSuFenMo != null)
				{
					yuanSuFenMo = CGetOldResourceManager._YuanSuFenMo;
				}
				else
				{
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiYuanSuFenMo", ',');
					lock (CGetOldResourceManager._YuanSuFenMoMutex)
					{
						CGetOldResourceManager._YuanSuFenMo = paramValueDoubleArrayByName;
					}
					yuanSuFenMo = CGetOldResourceManager._YuanSuFenMo;
				}
				return yuanSuFenMo;
			}
		}

		public static double RoleChangelifeRate(int count)
		{
			try
			{
				if (CGetOldResourceManager._changelifeRate == null)
				{
					CGetOldResourceManager._changelifeRate = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuanShengExpXiShu", ',');
				}
				if (CGetOldResourceManager._changelifeRate != null && CGetOldResourceManager._changelifeRate.Length > count)
				{
					return CGetOldResourceManager._changelifeRate[count];
				}
				return 1.0;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "获取经验各项产出根据转生对应的经验系数 error count=" + count, false, false);
			}
			return 1.0;
		}

		public static List<OldResourceInfo> GetOldResourceInfo(GameClient client)
		{
			List<OldResourceInfo> list = new List<OldResourceInfo>();
			if (client.ClientData.OldResourceInfoDict != null)
			{
				foreach (OldResourceInfo oldResourceInfo in client.ClientData.OldResourceInfoDict.Values)
				{
					if (oldResourceInfo != null && oldResourceInfo.leftCount > 0)
					{
						list.Add(oldResourceInfo);
					}
				}
			}
			return list;
		}

		public static bool HasOldResource(GameClient client)
		{
			return CGetOldResourceManager.GetOldResourceInfo(client).Count > 0;
		}

		private static bool RoleCando(GameClient client, XElement item)
		{
			int num = (int)Global.GetSafeAttributeLong(item, "Type");
			string[] array = Global.GetSafeAttributeStr(item, "MinLevel").Split(new char[]
			{
				','
			});
			string[] array2 = Global.GetSafeAttributeStr(item, "MaxLevel").Split(new char[]
			{
				','
			});
			int num2 = Global.SafeConvertToInt32(array[0]);
			int num3 = Global.SafeConvertToInt32(array[1]);
			string safeAttributeStr = Global.GetSafeAttributeStr(item, "NeedRenWu");
			bool flag = false;
			if (Global.SafeConvertToInt32(array[0]) < client.ClientData.ChangeLifeCount && client.ClientData.ChangeLifeCount < Global.SafeConvertToInt32(array2[0]))
			{
				flag = true;
			}
			if (Global.SafeConvertToInt32(array[0]) == client.ClientData.ChangeLifeCount)
			{
				if (Global.SafeConvertToInt32(array[1]) <= client.ClientData.Level)
				{
					flag = true;
				}
			}
			if (Global.SafeConvertToInt32(array2[0]) == client.ClientData.ChangeLifeCount)
			{
				if (Global.SafeConvertToInt32(array2[1]) >= client.ClientData.Level)
				{
					flag = true;
				}
			}
			bool flag2;
			if (string.IsNullOrEmpty(safeAttributeStr))
			{
				flag2 = true;
			}
			else
			{
				int taskID = Global.SafeConvertToInt32(safeAttributeStr);
				flag2 = CGetOldResourceManager.TaskHasDone(client, taskID);
			}
			return flag && flag2;
		}

		private static bool IsCanCalcOldResource(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				DateTime regTime = Global.GetRegTime(client.ClientData);
				DateTime dateTime = TimeUtil.NowDateTime();
				if (regTime.Year == dateTime.Year)
				{
					if (regTime.DayOfYear == dateTime.DayOfYear)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		private static bool TaskHasDone(GameClient client, int taskID)
		{
			return client.ClientData.MainTaskID >= taskID;
		}

		public static FuBenData GetOldFubenData(GameClient client, int fuBenID)
		{
			FuBenData result;
			if (null == client.ClientData.OldFuBenDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.OldFuBenDataList)
				{
					for (int i = 0; i < client.ClientData.OldFuBenDataList.Count; i++)
					{
						if (client.ClientData.OldFuBenDataList[i].FuBenID == fuBenID)
						{
							return client.ClientData.OldFuBenDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static int GetVIPActiveNumByType(GameClient client, int activeId)
		{
			int vipLevel = client.ClientData.VipLevel;
			string name;
			switch (activeId)
			{
			case 1:
				name = "VIPEnterBloodCastleCountAddValue";
				break;
			case 2:
				name = "VIPEnterDaimonSquareCountAddValue";
				break;
			default:
				return 0;
			}
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName(name, ',');
			if (vipLevel > 0 && paramValueIntArrayByName != null && paramValueIntArrayByName[vipLevel] > 0)
			{
				int num = paramValueIntArrayByName[vipLevel];
			}
			return 0;
		}

		private static void CalcOldResourceInfo(int leftnum, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			if (leftnum > 0)
			{
				if (inInfo == null)
				{
					outInfo = new OldResourceInfo();
					outInfo.bandmoney = 0;
					outInfo.chengjiu = 0;
					outInfo.exp = 0;
					outInfo.mojing = 0;
					outInfo.zhangong = 0;
					outInfo.shengwang = 0;
					outInfo.leftCount = 0;
					outInfo.bandDiamond = 0;
					outInfo.xinghun = 0;
					outInfo.yuanSuFenMo = 0;
				}
				outInfo.bandmoney += data.bandmoney * leftnum;
				outInfo.chengjiu += data.chengjiu * leftnum;
				outInfo.exp += data.exp * leftnum;
				outInfo.mojing += data.mojing * leftnum;
				outInfo.zhangong += data.zhangong * leftnum;
				outInfo.shengwang += data.shengwang * leftnum;
				outInfo.bandDiamond += data.bandDiamond * leftnum;
				outInfo.xinghun += data.xinghun * leftnum;
				outInfo.yuanSuFenMo += data.yuanSuFenMo * leftnum;
				outInfo.leftCount += leftnum;
				outInfo.type = data.type;
			}
		}

		private static void CalcOldResourceInfo(int oldday, int oldnum, int total, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			int num = 0;
			int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
			if (oldday >= 0 && oldnum >= 0)
			{
				if (dayOfYear == oldday)
				{
					num = total - oldnum;
				}
				else
				{
					num = total;
				}
				num = ((num > 0) ? num : 0);
			}
			CGetOldResourceManager.CalcOldResourceInfo(num, data, inInfo, out outInfo);
		}

		private static void GetFubenResourceInfo(GameClient client, int copyId, int total, bool needFinish, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			if (total >= 1)
			{
				FuBenData oldFubenData = CGetOldResourceManager.GetOldFubenData(client, copyId);
				int oldday;
				int oldnum;
				if (null != oldFubenData)
				{
					oldday = oldFubenData.DayID;
					oldnum = (needFinish ? oldFubenData.FinishNum : oldFubenData.EnterNum);
				}
				else
				{
					oldday = 0;
					oldnum = 0;
				}
				CGetOldResourceManager.CalcOldResourceInfo(oldday, oldnum, total, data, inInfo, out outInfo);
			}
		}

		private static void ComputeResourceByType(GameClient client, int type, Dictionary<int, List<CGetResData>> getRestDataDict, out OldResourceInfo outInfo)
		{
			outInfo = null;
			if (getRestDataDict.ContainsKey(type))
			{
				List<CGetResData> list = getRestDataDict[type];
				foreach (CGetResData cgetResData in list)
				{
					if (cgetResData.copyId != -1)
					{
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(cgetResData.copyId, out systemXmlItem))
						{
							continue;
						}
						bool needFinish = true;
						int intValue = systemXmlItem.GetIntValue("FinishNumber", -1);
						if (intValue < 0)
						{
							needFinish = false;
							intValue = systemXmlItem.GetIntValue("EnterNumber", -1);
						}
						OldResourceInfo oldResourceInfo = null;
						CGetOldResourceManager.GetFubenResourceInfo(client, cgetResData.copyId, intValue, needFinish, cgetResData, oldResourceInfo, out oldResourceInfo);
						if (oldResourceInfo != null)
						{
							int num = oldResourceInfo.leftCount;
							CGetOldResourceManager.CalcOldResourceInfo(oldResourceInfo.leftCount, cgetResData, outInfo, out outInfo);
						}
					}
					if (cgetResData.activeId != -1)
					{
						int num = 0;
						switch (cgetResData.type)
						{
						case 5:
						{
							int vipactiveNumByType = CGetOldResourceManager.GetVIPActiveNumByType(client, cgetResData.activeId);
							int key = Global.GetDaimonSquareCopySceneIDForRole(client);
							DaimonSquareDataInfo daimonSquareDataInfo = null;
							Data.DaimonSquareDataInfoList.TryGetValue(key, out daimonSquareDataInfo);
							if (null == daimonSquareDataInfo)
							{
								daimonSquareDataInfo = Data.DaimonSquareDataInfoList.FirstOrDefault<KeyValuePair<int, DaimonSquareDataInfo>>().Value;
								if (daimonSquareDataInfo == null)
								{
									break;
								}
							}
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 2);
							if (num2 < 0)
							{
								num2 = 0;
							}
							num = daimonSquareDataInfo.MaxEnterNum + vipactiveNumByType - num2;
							break;
						}
						case 6:
						{
							int vipactiveNumByType = CGetOldResourceManager.GetVIPActiveNumByType(client, cgetResData.activeId);
							int key = Global.GetBloodCastleCopySceneIDForRole(client);
							BloodCastleDataInfo bloodCastleDataInfo = null;
							Data.BloodCastleDataInfoList.TryGetValue(key, out bloodCastleDataInfo);
							if (null == bloodCastleDataInfo)
							{
								bloodCastleDataInfo = Data.BloodCastleDataInfoList.FirstOrDefault<KeyValuePair<int, BloodCastleDataInfo>>().Value;
								if (bloodCastleDataInfo == null)
								{
									break;
								}
							}
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 1);
							if (num2 < 0)
							{
								num2 = 0;
							}
							num = bloodCastleDataInfo.MaxEnterNum + vipactiveNumByType - num2;
							break;
						}
						case 7:
						{
							List<string> list2 = GameManager.AngelTempleMgr.m_AngelTempleData.BeginTime;
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 5);
							if (num2 < 0)
							{
								num2 = 0;
							}
							num = list2.Count - num2;
							break;
						}
						case 9:
						{
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 6);
							if (num2 < 0)
							{
								num2 = 0;
							}
							num = 1 - num2;
							break;
						}
						case 10:
						{
							SystemXmlItem systemXmlItem2 = null;
							if (!GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemXmlItem2))
							{
								return;
							}
							string[] array = null;
							string stringValue = systemXmlItem2.GetStringValue("TimePoints");
							if (stringValue != null && stringValue != "")
							{
								array = stringValue.Split(new char[]
								{
									','
								});
							}
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 3);
							if (num2 < 0)
							{
								num2 = 0;
							}
							if (array != null)
							{
								num = array.Length - num2;
							}
							break;
						}
						case 11:
						{
							SystemXmlItem systemXmlItem2 = null;
							if (!GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemXmlItem2))
							{
								return;
							}
							List<string> list2 = new List<string>();
							string[] array = null;
							string stringValue = systemXmlItem2.GetStringValue("TimePoints");
							if (stringValue != null && stringValue != "")
							{
								array = stringValue.Split(new char[]
								{
									','
								});
							}
							int dayOfYear = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 4);
							if (num2 < 0)
							{
								num2 = 0;
							}
							if (array != null)
							{
								num = array.Length - num2;
							}
							break;
						}
						}
						num = ((num > 0) ? num : 0);
						CGetOldResourceManager.CalcOldResourceInfo(num, cgetResData, outInfo, out outInfo);
					}
					if (cgetResData.type == 13)
					{
						int num3 = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiFuBenID", -1);
						SystemXmlItem systemXmlItem3 = null;
						GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(num3, out systemXmlItem3);
						int intValue = systemXmlItem3.GetIntValue("EnterNumber", -1);
						CGetOldResourceManager.GetFubenResourceInfo(client, num3, intValue, false, cgetResData, outInfo, out outInfo);
					}
					else if (cgetResData.type != 8)
					{
						if (cgetResData.type == 1)
						{
							int oldday = -1;
							int num4 = -1;
							if (client.ClientData.YesterdayDailyTaskData != null)
							{
								oldday = DateTime.Parse(client.ClientData.YesterdayDailyTaskData.RecTime).DayOfYear;
								num4 = client.ClientData.YesterdayDailyTaskData.RecNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, num4, 10, cgetResData, outInfo, out outInfo);
						}
						else if (cgetResData.type == 15)
						{
							int oldday = -1;
							int num4 = -1;
							if (client.ClientData.YesterdayTaofaTaskData != null)
							{
								oldday = DateTime.Parse(client.ClientData.YesterdayTaofaTaskData.RecTime).DayOfYear;
								num4 = client.ClientData.YesterdayTaofaTaskData.RecNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, num4, Global.MaxTaofaTaskNumForMU, cgetResData, outInfo, out outInfo);
						}
						else if (cgetResData.type == 16)
						{
							int oldday = -1;
							int num4 = -1;
							if (client.ClientData.OldCrystalCollectData != null)
							{
								oldday = client.ClientData.OldCrystalCollectData.OldDay;
								num4 = client.ClientData.OldCrystalCollectData.OldNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, num4, CaiJiLogic.DailyNum, cgetResData, outInfo, out outInfo);
						}
						else if (cgetResData.type == 19)
						{
							int oldday = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessDayId");
							int num4 = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessCount");
							int num = 3;
							int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("TempleMirageWinNum", ',');
							if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length == 2)
							{
								num = paramValueIntArrayByName[0];
							}
							CGetOldResourceManager.CalcOldResourceInfo(num - num4, cgetResData, outInfo, out outInfo);
						}
					}
				}
			}
		}

		public static void InitRoleOldResourceInfo(GameClient client, bool isFirstLogin)
		{
			if (CGetOldResourceManager.IsCanCalcOldResource(client))
			{
				if (!isFirstLogin)
				{
					client.ClientData.OldResourceInfoDict = CGetOldResourceManager.ReadResourceGetfromDB(client);
				}
				else if (CGetOldResourceManager.xmlData != null)
				{
					Dictionary<int, List<CGetResData>> dictionary = new Dictionary<int, List<CGetResData>>();
					IEnumerable<XElement> enumerable = CGetOldResourceManager.xmlData.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (CGetOldResourceManager.RoleCando(client, xelement))
						{
							int num = (int)Global.GetSafeAttributeLong(xelement, "Type");
							int copyId = (int)Global.GetSafeAttributeLong(xelement, "CodeID");
							int activeId = (int)Global.GetSafeAttributeLong(xelement, "HuoDongID");
							int num2 = (int)Global.GetSafeAttributeLong(xelement, "ExpAward");
							int num3 = (int)Global.GetSafeAttributeLong(xelement, "BandMoneyAward");
							int num4 = (int)Global.GetSafeAttributeLong(xelement, "ShengWangAward");
							int num5 = (int)Global.GetSafeAttributeLong(xelement, "ZhanGongAward");
							int num6 = (int)Global.GetSafeAttributeLong(xelement, "MoJingAward");
							int num7 = (int)Global.GetSafeAttributeLong(xelement, "ChengJiuAward");
							int num8 = (int)Global.GetSafeAttributeLong(xelement, "BindZuanAward");
							int num9 = (int)Global.GetSafeAttributeLong(xelement, "XingHunAward");
							int num10 = (int)Global.GetSafeAttributeLong(xelement, "YuanSuFenMo");
							CGetResData cgetResData = new CGetResData();
							cgetResData.type = num;
							cgetResData.copyId = copyId;
							cgetResData.activeId = activeId;
							cgetResData.exp = ((num2 > 0) ? num2 : 0);
							cgetResData.bandmoney = ((num3 > 0) ? num3 : 0);
							cgetResData.shengwang = ((num4 > 0) ? num4 : 0);
							cgetResData.zhangong = ((num5 > 0) ? num5 : 0);
							cgetResData.mojing = ((num6 > 0) ? num6 : 0);
							cgetResData.chengjiu = ((num7 > 0) ? num7 : 0);
							cgetResData.bandDiamond = ((num8 > 0) ? num8 : 0);
							cgetResData.xinghun = ((num9 > 0) ? num9 : 0);
							cgetResData.yuanSuFenMo = ((num10 > 0) ? num10 : 0);
							if (!dictionary.ContainsKey(num))
							{
								dictionary[num] = new List<CGetResData>();
							}
							dictionary[num].Add(cgetResData);
						}
					}
					Dictionary<int, OldResourceInfo> dictionary2 = new Dictionary<int, OldResourceInfo>();
					List<int> list = dictionary.Keys.ToList<int>();
					foreach (int num in list)
					{
						OldResourceInfo oldResourceInfo = null;
						int num;
						CGetOldResourceManager.ComputeResourceByType(client, num, dictionary, out oldResourceInfo);
						if (oldResourceInfo != null)
						{
							dictionary2[num] = oldResourceInfo;
							dictionary2[num].roleId = client.ClientData.RoleID;
						}
					}
					client.ClientData.OldResourceInfoDict = dictionary2;
					CGetOldResourceManager.ReplaceDataToDB(client);
				}
			}
		}

		public static int GiveRoleOldResource(GameClient client, int actType, int goldorZuanshi, int getModel)
		{
			int num = 0;
			OldResourceInfo oldResourceInfo = null;
			OldResourceInfo oldResourceInfo2 = null;
			int num2 = 0;
			double num3 = CGetOldResourceManager.RoleChangelifeRate(client.ClientData.ChangeLifeCount);
			if (getModel == 0)
			{
				if (client.ClientData.OldResourceInfoDict == null || !client.ClientData.OldResourceInfoDict.TryGetValue(actType, out oldResourceInfo))
				{
					LogManager.WriteLog(2, string.Format("CGetOldResource:资源获取失败, dict={0}, actType={1}", client.ClientData.OldResourceInfoDict, actType), null, true);
					return -3;
				}
				if (oldResourceInfo.leftCount == 0)
				{
					LogManager.WriteLog(2, string.Format("CGetOldResource:资源数量获取异常, leftCount={0}", oldResourceInfo.leftCount), null, true);
					return -3;
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BondGold.Length)
				{
					if (CGetOldResourceManager.BondGold[goldorZuanshi] != 0.0)
					{
						num2 = (int)((double)oldResourceInfo.bandmoney / CGetOldResourceManager.BondGold[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ExpGold.Length)
				{
					if (!num3.Equals(0.0) && CGetOldResourceManager.ExpGold[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.exp / (num3 * CGetOldResourceManager.ExpGold[goldorZuanshi]));
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ChengJiu.Length)
				{
					if (CGetOldResourceManager.ChengJiu[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.chengjiu / CGetOldResourceManager.ChengJiu[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ShengWang.Length)
				{
					if (CGetOldResourceManager.ShengWang[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.shengwang / CGetOldResourceManager.ShengWang[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.MoJing.Length)
				{
					if (CGetOldResourceManager.MoJing[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.mojing / CGetOldResourceManager.MoJing[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ZhanGong.Length)
				{
					if (CGetOldResourceManager.ZhanGong[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.zhangong / CGetOldResourceManager.ZhanGong[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BangZuan.Length)
				{
					if (CGetOldResourceManager.BangZuan[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.bandDiamond / CGetOldResourceManager.BangZuan[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.XingHun.Length)
				{
					if (CGetOldResourceManager.XingHun[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.xinghun / CGetOldResourceManager.XingHun[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.YuanSuFenMo.Length)
				{
					if (CGetOldResourceManager.YuanSuFenMo[goldorZuanshi] != 0.0)
					{
						num2 += (int)((double)oldResourceInfo.yuanSuFenMo / CGetOldResourceManager.YuanSuFenMo[goldorZuanshi]);
					}
				}
				oldResourceInfo2 = oldResourceInfo;
			}
			else
			{
				oldResourceInfo2 = new OldResourceInfo();
				int num4 = 0;
				List<int> list = client.ClientData.OldResourceInfoDict.Keys.ToList<int>();
				foreach (int key in list)
				{
					if (client.ClientData.OldResourceInfoDict.TryGetValue(key, out oldResourceInfo))
					{
						if (oldResourceInfo.leftCount != 0)
						{
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BondGold.Length)
							{
								if (CGetOldResourceManager.BondGold[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.bandmoney / CGetOldResourceManager.BondGold[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ExpGold.Length)
							{
								if (!num3.Equals(0.0) && CGetOldResourceManager.ExpGold[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.exp / (num3 * CGetOldResourceManager.ExpGold[goldorZuanshi]));
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ChengJiu.Length)
							{
								if (CGetOldResourceManager.ChengJiu[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.chengjiu / CGetOldResourceManager.ChengJiu[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ShengWang.Length)
							{
								if (CGetOldResourceManager.ShengWang[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.shengwang / CGetOldResourceManager.ShengWang[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.MoJing.Length)
							{
								if (CGetOldResourceManager.MoJing[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.mojing / CGetOldResourceManager.MoJing[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ZhanGong.Length)
							{
								if (CGetOldResourceManager.ZhanGong[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.zhangong / CGetOldResourceManager.ZhanGong[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BangZuan.Length)
							{
								if (CGetOldResourceManager.BangZuan[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.bandDiamond / CGetOldResourceManager.BangZuan[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.XingHun.Length)
							{
								if (CGetOldResourceManager.XingHun[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.xinghun / CGetOldResourceManager.XingHun[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.YuanSuFenMo.Length)
							{
								if (CGetOldResourceManager.YuanSuFenMo[goldorZuanshi] != 0.0)
								{
									num2 += (int)((double)oldResourceInfo.yuanSuFenMo / CGetOldResourceManager.YuanSuFenMo[goldorZuanshi]);
								}
							}
							oldResourceInfo2.bandmoney += oldResourceInfo.bandmoney;
							oldResourceInfo2.exp += oldResourceInfo.exp;
							oldResourceInfo2.chengjiu += oldResourceInfo.chengjiu;
							oldResourceInfo2.shengwang += oldResourceInfo.shengwang;
							oldResourceInfo2.mojing += oldResourceInfo.mojing;
							oldResourceInfo2.zhangong += oldResourceInfo.zhangong;
							oldResourceInfo2.bandDiamond += oldResourceInfo.bandDiamond;
							oldResourceInfo2.xinghun += oldResourceInfo.xinghun;
							oldResourceInfo2.yuanSuFenMo += oldResourceInfo.yuanSuFenMo;
							num4++;
						}
					}
				}
				if (num4 == 0)
				{
					return -3;
				}
			}
			int result;
			if (num2 <= 0)
			{
				LogManager.WriteLog(2, string.Format("CGetOldResource:资源消耗结算异常, leftCount={0}", num2), null, true);
				result = -3;
			}
			else
			{
				switch (goldorZuanshi)
				{
				case 0:
					if (num2 > client.ClientData.Money1 + client.ClientData.YinLiang)
					{
						LogManager.WriteLog(2, string.Format("CGetOldResource:消耗物资不足, cost={0},money1={1},yinliang={2}", num2, client.ClientData.Money1, client.ClientData.YinLiang), null, true);
						return -1;
					}
					if (Global.SubBindTongQianAndTongQian(client, num2, "资源找回"))
					{
						if (oldResourceInfo2.exp > 0)
						{
							long num5 = (long)((float)oldResourceInfo2.exp * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.ProcessRoleExperience(client, num5, true, true, false, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
							{
								num5
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (oldResourceInfo2.mojing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)((float)oldResourceInfo2.mojing * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true, false);
						}
						if (oldResourceInfo2.shengwang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, (int)((float)oldResourceInfo2.shengwang * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true);
						}
						if (oldResourceInfo2.bandmoney > 0)
						{
							int num6 = (int)((float)oldResourceInfo2.bandmoney * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num6, "金币资源找回", true);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(31, new object[0]), new object[]
							{
								num6
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (oldResourceInfo2.chengjiu > 0)
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)((float)oldResourceInfo2.chengjiu * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true);
						}
						if (oldResourceInfo2.zhangong > 0)
						{
							int num7 = (int)((float)oldResourceInfo2.zhangong * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref num7, AddBangGongTypes.None, 0);
						}
						if (oldResourceInfo2.bandDiamond > 0)
						{
							GameManager.ClientMgr.AddUserGold(client, (int)((float)oldResourceInfo2.bandDiamond * CGetOldResourceManager.GoldRate), "资源找回获得绑钻");
						}
						if (oldResourceInfo2.xinghun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, (int)((float)oldResourceInfo2.xinghun * CGetOldResourceManager.GoldRate), "资源找回获得星魂", true, true);
						}
						if (oldResourceInfo2.yuanSuFenMo > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)((float)oldResourceInfo2.yuanSuFenMo * CGetOldResourceManager.GoldRate), "资源找回获得元素粉末", true, false);
						}
					}
					break;
				case 1:
					if (num2 > client.ClientData.UserMoney)
					{
						return -2;
					}
					if (GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "资源找回", true, true, false, DaiBiSySType.None))
					{
						if (oldResourceInfo2.exp > 0)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, (long)oldResourceInfo2.exp, true, true, false, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
							{
								oldResourceInfo2.exp
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (oldResourceInfo2.mojing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, oldResourceInfo2.mojing, "资源找回(钻石)", false, true, false);
						}
						if (oldResourceInfo2.shengwang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, oldResourceInfo2.shengwang, "资源找回(钻石)", false, true);
						}
						if (oldResourceInfo2.bandmoney > 0)
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, oldResourceInfo2.bandmoney, "钻石资源找回", true);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(31, new object[0]), new object[]
							{
								oldResourceInfo2.bandmoney
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (oldResourceInfo2.chengjiu > 0)
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, oldResourceInfo2.chengjiu, "资源找回(钻石)", false, true);
						}
						if (oldResourceInfo2.zhangong > 0)
						{
							GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref oldResourceInfo2.zhangong, AddBangGongTypes.None, 0);
						}
						if (oldResourceInfo2.bandDiamond > 0)
						{
							GameManager.ClientMgr.AddUserGold(client, oldResourceInfo2.bandDiamond, "资源找回获得绑钻");
						}
						if (oldResourceInfo2.xinghun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, oldResourceInfo2.xinghun, "资源找回获得星魂", true, true);
						}
						if (oldResourceInfo2.yuanSuFenMo > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, oldResourceInfo2.yuanSuFenMo, "资源找回获得元素粉末", true, false);
						}
					}
					break;
				}
				lock (client)
				{
					if (getModel == 0)
					{
						if (client.ClientData.OldResourceInfoDict != null && client.ClientData.OldResourceInfoDict.ContainsKey(actType))
						{
							client.ClientData.OldResourceInfoDict.Remove(actType);
						}
					}
					else if (client.ClientData.OldResourceInfoDict != null)
					{
						client.ClientData.OldResourceInfoDict.Clear();
					}
				}
				CGetOldResourceManager.ReplaceDataToDB(client);
				result = num;
			}
			return result;
		}

		public static void ReplaceDataToDB(GameClient client)
		{
			Dictionary<int, Dictionary<int, OldResourceInfo>> dictionary = new Dictionary<int, Dictionary<int, OldResourceInfo>>();
			dictionary[client.ClientData.RoleID] = client.ClientData.OldResourceInfoDict;
			Global.sendToDB<int, byte[]>(10164, DataHelper.ObjectToBytes<Dictionary<int, Dictionary<int, OldResourceInfo>>>(dictionary), client.ServerId);
		}

		public static Dictionary<int, OldResourceInfo> ReadResourceGetfromDB(GameClient client)
		{
			Dictionary<int, OldResourceInfo> dictionary = new Dictionary<int, OldResourceInfo>();
			byte[] array = null;
			Dictionary<int, OldResourceInfo> result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10163, string.Format("{0}", client.ClientData.RoleID), out array, client.ServerId))
			{
				result = dictionary;
			}
			else if (array == null || array.Length <= 6)
			{
				result = dictionary;
			}
			else
			{
				int num = BitConverter.ToInt32(array, 0);
				dictionary = DataHelper.BytesToObject<Dictionary<int, OldResourceInfo>>(array, 6, num - 2);
				result = dictionary;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessOldResourceCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				switch (nID)
				{
				case 642:
				{
					if (array.Length != 1)
					{
						LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					List<OldResourceInfo> oldResourceInfo = CGetOldResourceManager.GetOldResourceInfo(gameClient);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<OldResourceInfo>>(oldResourceInfo, pool, nID);
					break;
				}
				case 643:
				{
					if (array.Length != 4)
					{
						LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					int num2 = Global.SafeConvertToInt32(array[1]);
					int goldorZuanshi = Global.SafeConvertToInt32(array[2]);
					int num3 = Global.SafeConvertToInt32(array[3]);
					int num4 = CGetOldResourceManager.GiveRoleOldResource(gameClient, num2, goldorZuanshi, num3);
					if (num4 == 0)
					{
						gameClient._IconStateMgr.CheckZiYuanZhaoHui(gameClient);
						gameClient._IconStateMgr.SendIconStateToClient(gameClient);
					}
					string data2 = string.Format("{0}:{1}:{2}", num4, num2, num3);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					break;
				}
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "QueryOldResource", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static float GoldRate = 0.75f;

		private static object _xmlDataMutex = new object();

		private static XElement _xmlData = null;

		private static double[] _Exp = null;

		private static object _ExpMutex = new object();

		private static double[] _BondGold = null;

		private static object _BondGoldMutex = new object();

		private static double[] _MoJing = null;

		private static object _MoJingMutex = new object();

		private static double[] _ShengWang = null;

		private static object _ShengWangMutex = new object();

		private static double[] _ChengJiu = null;

		private static object _ChengJiuMutex = new object();

		private static double[] _ZhanGong = null;

		private static object _ZhanGongMutex = new object();

		private static double[] _BangZuan = null;

		private static object _BangZuanMutex = new object();

		private static double[] _XingHun = null;

		private static object _XingHunMutex = new object();

		private static double[] _YuanSuFenMo = null;

		private static object _YuanSuFenMoMutex = new object();

		private static double[] _changelifeRate = null;
	}
}
