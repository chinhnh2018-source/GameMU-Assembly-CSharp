using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class YaoSaiService
	{
		public static YaoSaiService Instance()
		{
			return YaoSaiService._instance;
		}

		public void InitConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string paramValueByName = KuaFuServerManager.systemParamsList.GetParamValueByName("ManorCommandAward");
					string[] array = paramValueByName.Split(new char[]
					{
						','
					});
					if (array.Length == 3)
					{
						this.RuntimeData.FuLuHuDongTimes = Global.SafeConvertToInt32(array[0]);
						this.RuntimeData.FuLuAwardTimes = Global.SafeConvertToInt32(array[1]);
						this.RuntimeData.FuLuHuDongMinutes = Global.SafeConvertToInt32(array[2]);
					}
					string fileName = "Config/ManorLevel.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						YaoSaiJianYuManorLevelConfig yaoSaiJianYuManorLevelConfig = new YaoSaiJianYuManorLevelConfig();
						yaoSaiJianYuManorLevelConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "MinLevel", "");
						string[] array2 = elementAttributeValue.Split(new char[]
						{
							'|'
						});
						if (array2.Length == 2)
						{
							yaoSaiJianYuManorLevelConfig.MinZhuanSheng = Global.SafeConvertToInt32(array2[0]);
							yaoSaiJianYuManorLevelConfig.MinLevel = Global.SafeConvertToInt32(array2[1]);
						}
						elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "MaxLevel", "");
						array2 = elementAttributeValue.Split(new char[]
						{
							'|'
						});
						if (array2.Length == 2)
						{
							yaoSaiJianYuManorLevelConfig.MaxZhuanSheng = Global.SafeConvertToInt32(array2[0]);
							yaoSaiJianYuManorLevelConfig.MaxLevel = Global.SafeConvertToInt32(array2[1]);
						}
						this.RuntimeData.YaoSaiJianYuLevelDict[yaoSaiJianYuManorLevelConfig.ID] = yaoSaiJianYuManorLevelConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public KuaFuCmdData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				long num = TimeUtil.NOW();
				long num2 = long.MinValue;
				KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == yaoSaiPrisonRoleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonRoleAllData kfprisonRoleAllData = null;
					KuaFuData<KFPrisonRoleData> kfRoleData = yaoSaiPrisonRoleAllData.kfRoleData;
					int yaoSaiLevelID = this.GetYaoSaiLevelID(kfRoleData.V.UnionLevel);
					int yaoSaiLevelID2 = this.GetYaoSaiLevelID(unionlev);
					kfRoleData.V.Faction = faction;
					if (yaoSaiLevelID != yaoSaiLevelID2)
					{
						kfRoleData.V.UnionLevel = unionlev;
						this.UpdateYaoSaiPrisonSearchData(yaoSaiPrisonRoleAllData, yaoSaiLevelID);
						this.SaveYaoSaiPrisonRoleDataDB(kfRoleData.V, null);
					}
					List<KFPrisonRoleAllData> list;
					if (!this.YaoSaiSearchDataDict.TryGetValue(yaoSaiLevelID2, out list))
					{
						result = null;
					}
					else
					{
						for (int i = 0; i < 5; i++)
						{
							KFPrisonRoleAllData kfprisonRoleAllData2 = list[Global.GetRandomNumber(0, list.Count)];
							if (this.YaoSaiSearchItemCheck(kfRoleData.V, kfprisonRoleAllData2.kfRoleData.V, frindSet))
							{
								kfprisonRoleAllData = kfprisonRoleAllData2;
							}
						}
						if (null == kfprisonRoleAllData)
						{
							foreach (KFPrisonRoleAllData kfprisonRoleAllData2 in list)
							{
								if (this.YaoSaiSearchItemCheck(kfRoleData.V, kfprisonRoleAllData2.kfRoleData.V, frindSet))
								{
									long num3 = num - kfprisonRoleAllData2.SearchTimeStamp;
									if (num3 >= 60000L)
									{
										kfprisonRoleAllData = kfprisonRoleAllData2;
										break;
									}
									if (num3 > num2)
									{
										num2 = num3;
										kfprisonRoleAllData = kfprisonRoleAllData2;
									}
								}
							}
						}
						if (null != kfprisonRoleAllData)
						{
							kfprisonRoleAllData.SearchTimeStamp = num;
							result = new KuaFuCmdData
							{
								Age = kfprisonRoleAllData.kfRoleData.Age,
								Bytes0 = DataHelper2.ObjectToBytes<KFPrisonRoleData>(kfprisonRoleAllData.kfRoleData.V)
							};
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result;
		}

		public KuaFuCmdData GetYaoSaiPrisonRoleData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == yaoSaiPrisonRoleAllData)
				{
					result = null;
				}
				else if (dataAge != yaoSaiPrisonRoleAllData.kfRoleData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = yaoSaiPrisonRoleAllData.kfRoleData.Age,
						Bytes0 = DataHelper2.ObjectToBytes<KFPrisonRoleData>(yaoSaiPrisonRoleAllData.kfRoleData.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = yaoSaiPrisonRoleAllData.kfRoleData.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetYaoSaiFuLuListData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (0 == rid)
				{
					result = null;
				}
				else
				{
					KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(rid, true);
					if (null == yaoSaiPrisonFuLuAllData)
					{
						result = null;
					}
					else if (dataAge != yaoSaiPrisonFuLuAllData.fuluData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = yaoSaiPrisonFuLuAllData.fuluData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<List<KFPrisonRoleData>>(yaoSaiPrisonFuLuAllData.fuluData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = yaoSaiPrisonFuLuAllData.fuluData.Age
						};
					}
				}
			}
			return result;
		}

		public KuaFuCmdData GetYaoSaiPrisonLogData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == yaoSaiPrisonRoleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonLogAllData kfprisonLogAllData = null;
					if (!this.YaoSaiPrisonLogDataDict.TryGetValue(rid, out kfprisonLogAllData))
					{
						kfprisonLogAllData = new KFPrisonLogAllData();
						if (!this.LoadYaoSaiPrisonLogList(rid, kfprisonLogAllData.LogListData.V))
						{
							return null;
						}
						this.YaoSaiPrisonLogDataDict[rid] = kfprisonLogAllData;
						TimeUtil.AgeByNow(ref kfprisonLogAllData.LogListData.Age);
					}
					kfprisonLogAllData.LogDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
					if (dataAge != kfprisonLogAllData.LogListData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = kfprisonLogAllData.LogListData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<List<KFPrisonLogData>>(kfprisonLogAllData.LogListData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = kfprisonLogAllData.LogListData.Age
						};
					}
				}
			}
			return result;
		}

		public KuaFuCmdData GetYaoSaiPrisonJingJiData(int rid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(rid, true);
				if (null == yaoSaiPrisonRoleAllData)
				{
					result = null;
				}
				else
				{
					KFPrisonJingJiAllData kfprisonJingJiAllData = null;
					if (!this.YaoSaiPrisonJingJiDataDict.TryGetValue(rid, out kfprisonJingJiAllData))
					{
						kfprisonJingJiAllData = new KFPrisonJingJiAllData();
						if (!this.LoadYaoSaiPrisonJingJiData(rid, kfprisonJingJiAllData.JingJiData.V))
						{
							return null;
						}
						this.YaoSaiPrisonJingJiDataDict[rid] = kfprisonJingJiAllData;
						kfprisonJingJiAllData.JingJiDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						TimeUtil.AgeByNow(ref kfprisonJingJiAllData.JingJiData.Age);
					}
					if (dataAge != kfprisonJingJiAllData.JingJiData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = kfprisonJingJiAllData.JingJiData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFPrisonJingJiData>(kfprisonJingJiAllData.JingJiData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = kfprisonJingJiAllData.JingJiData.Age
						};
					}
				}
			}
			return result;
		}

		public int YaoSaiPrisonOpt(int srcrid, int targetid, int type, bool success)
		{
			int result;
			lock (this.Mutex)
			{
				if (type <= -3 || type >= 3)
				{
					result = -1;
				}
				else
				{
					KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(srcrid, true);
					if (null == yaoSaiPrisonRoleAllData)
					{
						result = -1;
					}
					else
					{
						KFPrisonRoleAllData yaoSaiPrisonRoleAllData2 = this.GetYaoSaiPrisonRoleAllData(targetid, true);
						if (null == yaoSaiPrisonRoleAllData2)
						{
							result = -1;
						}
						else
						{
							KuaFuData<KFPrisonRoleData> kfRoleData = yaoSaiPrisonRoleAllData.kfRoleData;
							KuaFuData<KFPrisonRoleData> kfRoleData2 = yaoSaiPrisonRoleAllData2.kfRoleData;
							if (-2 == type)
							{
								if (srcrid != kfRoleData2.V.OwnerID)
								{
									return -1;
								}
								KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
								if (null != yaoSaiPrisonFuLuAllData)
								{
									KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
									KFPrisonRoleData kfprisonRoleData = fuluData.V.Find((KFPrisonRoleData x) => x.RoleID == targetid);
									if (null != kfprisonRoleData)
									{
										fuluData.V.Remove(kfprisonRoleData);
										TimeUtil.AgeByNow(ref fuluData.Age);
									}
								}
								kfRoleData2.V.OwnerID = 0;
								TimeUtil.AgeByNow(ref kfRoleData2.Age);
								this.SaveYaoSaiPrisonRoleDataDB(kfRoleData2.V, null);
							}
							else if (-1 == type)
							{
								if (kfRoleData.V.OwnerID == 0)
								{
									return -1;
								}
								if (kfRoleData.V.OwnerID != targetid)
								{
									yaoSaiPrisonRoleAllData2 = this.GetYaoSaiPrisonRoleAllData(kfRoleData.V.OwnerID, true);
									if (null == yaoSaiPrisonRoleAllData2)
									{
										return -1;
									}
									kfRoleData2 = yaoSaiPrisonRoleAllData2.kfRoleData;
								}
								if (success)
								{
									KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(kfRoleData.V.OwnerID, false);
									if (null != yaoSaiPrisonFuLuAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluData2 = yaoSaiPrisonFuLuAllData.fuluData;
										fuluData2.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == srcrid);
										TimeUtil.AgeByNow(ref fuluData2.Age);
									}
									kfRoleData.V.OwnerID = 0;
									TimeUtil.AgeByNow(ref kfRoleData.Age);
									this.SaveYaoSaiPrisonRoleDataDB(kfRoleData.V, null);
								}
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data, false);
								KFPrisonLogData data2 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = kfRoleData2.V.RoleID,
									Name1 = kfRoleData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data2, false);
							}
							else if (0 == type)
							{
								if (kfRoleData.V.OwnerID == targetid)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, -1, success);
								}
								if (kfRoleData2.V.OwnerID != 0)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, 1, success);
								}
								if (success)
								{
									KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
									if (null != yaoSaiPrisonFuLuAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
										fuluData.V.Add(kfRoleData2.V);
										TimeUtil.AgeByNow(ref fuluData.Age);
									}
									kfRoleData2.V.OwnerID = srcrid;
									kfRoleData2.V.FuLuTime = TimeUtil.NowDateTime().Ticks;
									TimeUtil.AgeByNow(ref kfRoleData2.Age);
									this.SaveYaoSaiPrisonRoleDataDB(kfRoleData2.V, null);
								}
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = kfRoleData2.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data, false);
								KFPrisonLogData data2 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = kfRoleData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data2, false);
							}
							else if (1 == type)
							{
								if (srcrid == kfRoleData2.V.OwnerID)
								{
									return -1;
								}
								if (kfRoleData.V.OwnerID == targetid)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, -1, success);
								}
								if (kfRoleData2.V.OwnerID == 0)
								{
									return this.YaoSaiPrisonOpt(srcrid, targetid, 0, success);
								}
								KFPrisonLogData data3 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 2, success),
									RoleID = kfRoleData2.V.OwnerID,
									Name1 = kfRoleData.V.RoleName,
									Name2 = kfRoleData2.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data3, false);
								if (success)
								{
									KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(kfRoleData2.V.OwnerID, false);
									if (null != yaoSaiPrisonFuLuAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
										fuluData.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == targetid);
										TimeUtil.AgeByNow(ref fuluData.Age);
									}
									yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(srcrid, false);
									if (null != yaoSaiPrisonFuLuAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
										fuluData.V.Add(kfRoleData2.V);
										TimeUtil.AgeByNow(ref fuluData.Age);
									}
									kfRoleData2.V.OwnerID = srcrid;
									kfRoleData2.V.FuLuTime = TimeUtil.NowDateTime().Ticks;
									TimeUtil.AgeByNow(ref kfRoleData2.Age);
									this.SaveYaoSaiPrisonRoleDataDB(kfRoleData2.V, null);
								}
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = kfRoleData2.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data, false);
								KFPrisonLogData data2 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = kfRoleData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data2, false);
							}
							else if (2 == type)
							{
								if (srcrid == kfRoleData2.V.OwnerID)
								{
									return -1;
								}
								if (0 == kfRoleData2.V.OwnerID)
								{
									return -1;
								}
								KFPrisonLogData data3 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 2, success),
									RoleID = kfRoleData2.V.OwnerID,
									Name1 = kfRoleData.V.RoleName,
									Name2 = kfRoleData2.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data3, false);
								if (success)
								{
									KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(kfRoleData2.V.OwnerID, false);
									if (null != yaoSaiPrisonFuLuAllData)
									{
										KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
										fuluData.V.RemoveAll((KFPrisonRoleData x) => x.RoleID == targetid);
										TimeUtil.AgeByNow(ref fuluData.Age);
									}
									kfRoleData2.V.OwnerID = 0;
									TimeUtil.AgeByNow(ref kfRoleData2.Age);
									this.SaveYaoSaiPrisonRoleDataDB(kfRoleData2.V, null);
								}
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 0, success),
									RoleID = srcrid,
									Name1 = kfRoleData2.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data, false);
								KFPrisonLogData data2 = new KFPrisonLogData
								{
									IntroID = this.TransOptTypeToLogType(type, 1, success),
									RoleID = targetid,
									Name1 = kfRoleData.V.RoleName,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data2, false);
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}

		private void UpdateYaoSaiPrisonSearchData(KFPrisonRoleAllData allData, int oldLevID)
		{
			int yaoSaiLevelID = this.GetYaoSaiLevelID(allData.kfRoleData.V.UnionLevel);
			if (allData.SearchDataIndex == -1)
			{
				List<KFPrisonRoleAllData> list = null;
				if (!this.YaoSaiSearchDataDict.TryGetValue(yaoSaiLevelID, out list))
				{
					list = new List<KFPrisonRoleAllData>();
					this.YaoSaiSearchDataDict[yaoSaiLevelID] = list;
				}
				allData.SearchDataIndex = list.Count;
				list.Add(allData);
			}
			else if (oldLevID != yaoSaiLevelID)
			{
				List<KFPrisonRoleAllData> list2 = null;
				if (!this.YaoSaiSearchDataDict.TryGetValue(oldLevID, out list2))
				{
					LogManager.WriteLog(0, string.Format("要塞搜索数据异常 RoleID={0} OldLevID={1} NewUnionLev={2}", allData.kfRoleData.V.RoleID, oldLevID, allData.kfRoleData.V.UnionLevel), null, true);
				}
				else
				{
					List<KFPrisonRoleAllData> list3 = null;
					if (!this.YaoSaiSearchDataDict.TryGetValue(yaoSaiLevelID, out list3))
					{
						list3 = new List<KFPrisonRoleAllData>();
						this.YaoSaiSearchDataDict[yaoSaiLevelID] = list3;
					}
					list2[list2.Count - 1].SearchDataIndex = allData.SearchDataIndex;
					KFPrisonRoleAllData value = list2[allData.SearchDataIndex];
					list2[allData.SearchDataIndex] = list2[list2.Count - 1];
					list2[list2.Count - 1] = value;
					list2.RemoveAt(list2.Count - 1);
					allData.SearchDataIndex = list3.Count;
					list3.Add(allData);
				}
			}
		}

		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole updateData)
		{
			lock (this.Mutex)
			{
				if (this.GetYaoSaiLevelID(updateData.UnionLevel) == -1)
				{
					LogManager.WriteLog(0, string.Format("要塞异常数据更新 RoleID={0} RoleName={1} ZoneID={2} UnionLev={3}", new object[]
					{
						updateData.RoleID,
						updateData.RoleName,
						updateData.ZoneID,
						updateData.UnionLevel
					}), null, true);
					return -1;
				}
				KFPrisonRoleAllData kfprisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(updateData.RoleID, true);
				KuaFuData<KFPrisonRoleData> kfRoleData;
				if (null == kfprisonRoleAllData)
				{
					kfprisonRoleAllData = new KFPrisonRoleAllData();
					this.YaoSaiPrisonRoleDataDict[updateData.RoleID] = kfprisonRoleAllData;
					kfRoleData = kfprisonRoleAllData.kfRoleData;
				}
				else
				{
					kfRoleData = kfprisonRoleAllData.kfRoleData;
				}
				int yaoSaiLevelID = this.GetYaoSaiLevelID(kfRoleData.V.UnionLevel);
				kfRoleData.V.RoleID = updateData.RoleID;
				kfRoleData.V.RoleName = updateData.RoleName;
				kfRoleData.V.UnionLevel = updateData.UnionLevel;
				kfRoleData.V.Faction = updateData.Faction;
				kfRoleData.V.RoleSex = updateData.RoleSex;
				kfRoleData.V.Occupation = updateData.Occupation;
				kfRoleData.V.CombatForce = updateData.CombatForce;
				kfRoleData.V.ZoneID = updateData.ZoneID;
				kfRoleData.V.OptTime = TimeUtil.NowDateTime().Ticks;
				TimeUtil.AgeByNow(ref kfRoleData.Age);
				kfprisonRoleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
				KFPrisonJingJiAllData kfprisonJingJiAllData = null;
				if (this.YaoSaiPrisonJingJiDataDict.TryGetValue(updateData.RoleID, out kfprisonJingJiAllData))
				{
					kfprisonJingJiAllData.JingJiData.V.PlayerJingJiMirrorData = updateData.PlayerJingJiMirrorData;
					TimeUtil.AgeByNow(ref kfprisonJingJiAllData.JingJiData.Age);
				}
				this.UpdateYaoSaiPrisonSearchData(kfprisonRoleAllData, yaoSaiLevelID);
				if (kfRoleData.V.OwnerID != 0)
				{
					KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(kfRoleData.V.OwnerID, false);
					if (null != yaoSaiPrisonFuLuAllData)
					{
						KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
						TimeUtil.AgeByNow(ref fuluData.Age);
					}
				}
				this.SaveYaoSaiPrisonRoleDataDB(kfRoleData.V, updateData.PlayerJingJiMirrorData);
			}
			return 0;
		}

		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			int result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData yaoSaiPrisonRoleAllData = this.GetYaoSaiPrisonRoleAllData(ownerid, true);
				if (null == yaoSaiPrisonRoleAllData)
				{
					result = -1;
				}
				else
				{
					KFPrisonRoleAllData yaoSaiPrisonRoleAllData2 = this.GetYaoSaiPrisonRoleAllData(fuluid, true);
					if (null == yaoSaiPrisonRoleAllData2)
					{
						result = -1;
					}
					else
					{
						KuaFuData<KFPrisonRoleData> kfRoleData = yaoSaiPrisonRoleAllData.kfRoleData;
						KuaFuData<KFPrisonRoleData> kfRoleData2 = yaoSaiPrisonRoleAllData2.kfRoleData;
						if (kfRoleData2.V.OwnerID != ownerid)
						{
							result = -1;
						}
						else
						{
							int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
							if (offsetDay != kfRoleData2.V.CountDayID)
							{
								kfRoleData2.V.CountDayID = offsetDay;
								kfRoleData2.V.AwardCount = 0;
							}
							if (kfRoleData2.V.AwardCount >= this.RuntimeData.FuLuAwardTimes)
							{
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransHuDongTypeToLogType(type, false, false),
									RoleID = kfRoleData2.V.RoleID,
									State = 1
								};
								this.AddYaoSaiPrisonLogData(data, false);
							}
							else
							{
								KFPrisonLogData data = new KFPrisonLogData
								{
									IntroID = this.TransHuDongTypeToLogType(type, false, true),
									RoleID = kfRoleData2.V.RoleID,
									Param1 = param0,
									Param2 = param2,
									State = 0
								};
								this.AddYaoSaiPrisonLogData(data, true);
								kfRoleData2.V.AwardCount++;
							}
							KFPrisonLogData data2 = new KFPrisonLogData
							{
								IntroID = this.TransHuDongTypeToLogType(type, true, true),
								RoleID = ownerid,
								Name1 = kfRoleData2.V.RoleName,
								Param1 = param0,
								Param2 = param1,
								State = 1
							};
							this.AddYaoSaiPrisonLogData(data2, false);
							TimeUtil.AgeByNow(ref kfRoleData2.Age);
							this.SaveYaoSaiPrisonRoleDataDB(kfRoleData2.V, null);
							result = kfRoleData2.V.AwardCount;
						}
					}
				}
			}
			return result;
		}

		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			lock (this.Mutex)
			{
				string sqlCmd = string.Format("UPDATE `t_yaosai_prison_log` SET `state`={0} WHERE `id`={1}", state, id);
				this.ExecuteSqlNoQuery(sqlCmd);
				KFPrisonLogAllData kfprisonLogAllData = null;
				if (!this.YaoSaiPrisonLogDataDict.TryGetValue(rid, out kfprisonLogAllData))
				{
					return 0;
				}
				KFPrisonLogData kfprisonLogData = kfprisonLogAllData.LogListData.V.Find((KFPrisonLogData x) => x.ID == id);
				if (null != kfprisonLogData)
				{
					kfprisonLogData.State = state;
					TimeUtil.AgeByNow(ref kfprisonLogAllData.LogListData.Age);
				}
			}
			return 0;
		}

		private KFPrisonRoleAllData GetYaoSaiPrisonRoleAllData(int roleID, bool loadFromDB = true)
		{
			KFPrisonRoleAllData result;
			lock (this.Mutex)
			{
				KFPrisonRoleAllData kfprisonRoleAllData = null;
				if (this.YaoSaiPrisonRoleDataDict.TryGetValue(roleID, out kfprisonRoleAllData))
				{
					result = kfprisonRoleAllData;
				}
				else if (!loadFromDB)
				{
					result = kfprisonRoleAllData;
				}
				else
				{
					YaoSaiSearchParam loadParam = new YaoSaiSearchParam
					{
						roleID = roleID,
						loadFuLu = false
					};
					List<KFPrisonRoleData> list = this.LoadYaoSaiPrisonRoleList(loadParam);
					if (list == null || list.Count == 0)
					{
						result = null;
					}
					else
					{
						kfprisonRoleAllData = new KFPrisonRoleAllData();
						if (list[0].OwnerID != 0)
						{
							KFPrisonFuLuAllData yaoSaiPrisonFuLuAllData = this.GetYaoSaiPrisonFuLuAllData(list[0].OwnerID, false);
							if (null != yaoSaiPrisonFuLuAllData)
							{
								KuaFuData<List<KFPrisonRoleData>> fuluData = yaoSaiPrisonFuLuAllData.fuluData;
								KFPrisonRoleData kfprisonRoleData = fuluData.V.Find((KFPrisonRoleData x) => x.RoleID == roleID);
								if (null != kfprisonRoleData)
								{
									list[0] = kfprisonRoleData;
								}
							}
						}
						kfprisonRoleAllData.kfRoleData.V = list[0];
						TimeUtil.AgeByNow(ref kfprisonRoleAllData.kfRoleData.Age);
						kfprisonRoleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						this.YaoSaiPrisonRoleDataDict[roleID] = kfprisonRoleAllData;
						this.UpdateYaoSaiPrisonSearchData(kfprisonRoleAllData, this.GetYaoSaiLevelID(kfprisonRoleAllData.kfRoleData.V.UnionLevel));
						result = kfprisonRoleAllData;
					}
				}
			}
			return result;
		}

		private KFPrisonFuLuAllData GetYaoSaiPrisonFuLuAllData(int roleID, bool loadFromDB = true)
		{
			KFPrisonFuLuAllData result;
			lock (this.Mutex)
			{
				KFPrisonFuLuAllData kfprisonFuLuAllData = null;
				if (this.YaoSaiOwnerIDVsFuLuDict.TryGetValue(roleID, out kfprisonFuLuAllData))
				{
					result = kfprisonFuLuAllData;
				}
				else if (!loadFromDB)
				{
					result = kfprisonFuLuAllData;
				}
				else
				{
					YaoSaiSearchParam loadParam = new YaoSaiSearchParam
					{
						roleID = roleID,
						loadFuLu = true
					};
					List<KFPrisonRoleData> list = this.LoadYaoSaiPrisonRoleList(loadParam);
					if (list == null || list.Count == 0)
					{
						result = null;
					}
					else
					{
						kfprisonFuLuAllData = new KFPrisonFuLuAllData();
						foreach (KFPrisonRoleData kfprisonRoleData in list)
						{
							KFPrisonRoleAllData kfprisonRoleAllData = null;
							if (!this.YaoSaiPrisonRoleDataDict.TryGetValue(kfprisonRoleData.RoleID, out kfprisonRoleAllData))
							{
								kfprisonRoleAllData = new KFPrisonRoleAllData();
								kfprisonRoleAllData.kfRoleData.V = kfprisonRoleData;
								kfprisonRoleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
								TimeUtil.AgeByNow(ref kfprisonRoleAllData.kfRoleData.Age);
								this.YaoSaiPrisonRoleDataDict[kfprisonRoleData.RoleID] = kfprisonRoleAllData;
								this.UpdateYaoSaiPrisonSearchData(kfprisonRoleAllData, this.GetYaoSaiLevelID(kfprisonRoleAllData.kfRoleData.V.UnionLevel));
							}
							kfprisonFuLuAllData.fuluData.V.Add(kfprisonRoleAllData.kfRoleData.V);
						}
						kfprisonFuLuAllData.DataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						TimeUtil.AgeByNow(ref kfprisonFuLuAllData.fuluData.Age);
						this.YaoSaiOwnerIDVsFuLuDict[roleID] = kfprisonFuLuAllData;
						result = kfprisonFuLuAllData;
					}
				}
			}
			return result;
		}

		public void CheckYaoSaiPrisonTimerProc(DateTime now)
		{
			if (this.YaoSaiPrisonJingJiDataDict.Count != 0 || this.YaoSaiPrisonLogDataDict.Count != 0 || this.YaoSaiSearchDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, List<KFPrisonRoleAllData>> keyValuePair in this.YaoSaiSearchDataDict)
					{
						List<KFPrisonRoleAllData> value = keyValuePair.Value;
						if (value.Count <= 20000)
						{
							LogManager.WriteLog(0, string.Format("要塞搜索数据清理 LevelID={0}, TotalNum={1}, RemoveNum={2}, LeftNum={3}", new object[]
							{
								keyValuePair.Key,
								value.Count,
								0,
								value.Count
							}), null, true);
						}
						else
						{
							value.Sort(delegate(KFPrisonRoleAllData left, KFPrisonRoleAllData right)
							{
								int result;
								if (left.RoleDataEndTime > right.RoleDataEndTime)
								{
									result = -1;
								}
								else if (left.RoleDataEndTime < right.RoleDataEndTime)
								{
									result = 1;
								}
								else
								{
									result = 0;
								}
								return result;
							});
							for (int i = 0; i < value.Count; i++)
							{
								value[i].SearchDataIndex = i;
								if (i >= 20000)
								{
									list.Add(value[i].kfRoleData.V.RoleID);
								}
							}
							LogManager.WriteLog(0, string.Format("要塞搜索数据清理 LevelID={0}, TotalNum={1}, RemoveNum={2}, LeftNum={3}", new object[]
							{
								keyValuePair.Key,
								value.Count,
								value.Count - 20000,
								20000
							}), null, true);
							value.RemoveRange(20000, value.Count - 20000);
						}
					}
					foreach (int key in list)
					{
						this.YaoSaiPrisonRoleDataDict.Remove(key);
					}
					list.Clear();
					foreach (KeyValuePair<int, KFPrisonFuLuAllData> keyValuePair2 in this.YaoSaiOwnerIDVsFuLuDict)
					{
						if (keyValuePair2.Value.DataEndTime < now)
						{
							list.Add(keyValuePair2.Key);
						}
					}
					LogManager.WriteLog(0, string.Format("要塞俘虏数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiOwnerIDVsFuLuDict.Count, list.Count, this.YaoSaiOwnerIDVsFuLuDict.Count - list.Count), null, true);
					foreach (int key in list)
					{
						this.YaoSaiOwnerIDVsFuLuDict.Remove(key);
					}
					list.Clear();
					foreach (KeyValuePair<int, KFPrisonJingJiAllData> keyValuePair3 in this.YaoSaiPrisonJingJiDataDict)
					{
						if (keyValuePair3.Value.JingJiDataEndTime < now)
						{
							list.Add(keyValuePair3.Key);
						}
					}
					LogManager.WriteLog(0, string.Format("要塞竞技数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiPrisonJingJiDataDict.Count, list.Count, this.YaoSaiPrisonJingJiDataDict.Count - list.Count), null, true);
					foreach (int key in list)
					{
						this.YaoSaiPrisonJingJiDataDict.Remove(key);
					}
					list.Clear();
					foreach (KeyValuePair<int, KFPrisonLogAllData> keyValuePair4 in this.YaoSaiPrisonLogDataDict)
					{
						if (keyValuePair4.Value.LogDataEndTime < now)
						{
							list.Add(keyValuePair4.Key);
						}
					}
					LogManager.WriteLog(0, string.Format("要塞日志数据清理 TotalNum={0}, RemoveNum={1}, LeftNum={2}", this.YaoSaiPrisonLogDataDict.Count, list.Count, this.YaoSaiPrisonLogDataDict.Count - list.Count), null, true);
					foreach (int key in list)
					{
						this.YaoSaiPrisonLogDataDict.Remove(key);
					}
				}
			}
		}

		private bool YaoSaiSearchItemCheck(KFPrisonRoleData srcData, KFPrisonRoleData searchItem, HashSet<int> frindSet)
		{
			return srcData.RoleID != searchItem.RoleID && srcData.RoleID != searchItem.OwnerID && srcData.OwnerID != searchItem.RoleID && (srcData.Faction == 0 || srcData.Faction != searchItem.Faction) && (frindSet == null || !frindSet.Contains(searchItem.RoleID));
		}

		private void AddYaoSaiPrisonLogData(KFPrisonLogData data, bool broadCast = false)
		{
			lock (this.Mutex)
			{
				KFPrisonLogAllData kfprisonLogAllData = null;
				if (this.YaoSaiPrisonLogDataDict.TryGetValue(data.RoleID, out kfprisonLogAllData))
				{
					kfprisonLogAllData.LogListData.V.Add(data);
					TimeUtil.AgeByNow(ref kfprisonLogAllData.LogListData.Age);
					if (kfprisonLogAllData.LogListData.V.Count > 30)
					{
						int num = kfprisonLogAllData.LogListData.V.Count - 30;
						for (int i = 0; i < num; i++)
						{
							this.DeleteYaoSaiPrisonLogData(kfprisonLogAllData.LogListData.V[i]);
						}
						kfprisonLogAllData.LogListData.V.RemoveRange(0, num);
					}
				}
				this.InsertYaoSaiPrisonLogData(data);
				if (broadCast)
				{
					AsyncDataItem evItem = new AsyncDataItem(31, new object[]
					{
						data.RoleID
					});
					ClientAgentManager.Instance().BroadCastAsyncEvent(21, evItem, 0);
				}
			}
		}

		public int GetYaoSaiLevelID(int unionlev)
		{
			int result = -1;
			lock (this.RuntimeData.Mutex)
			{
				foreach (YaoSaiJianYuManorLevelConfig yaoSaiJianYuManorLevelConfig in this.RuntimeData.YaoSaiJianYuLevelDict.Values)
				{
					int num = yaoSaiJianYuManorLevelConfig.MinZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MinLevel;
					int num2 = yaoSaiJianYuManorLevelConfig.MaxZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MaxLevel;
					if (unionlev >= num && unionlev <= num2)
					{
						result = yaoSaiJianYuManorLevelConfig.ID;
						break;
					}
				}
			}
			return result;
		}

		public int TransOptTypeToLogType(int type, int src_tar_thr, bool success)
		{
			int result = 0;
			switch (type)
			{
			case -2:
				if (1 == src_tar_thr)
				{
				}
				break;
			case -1:
				if (1 == src_tar_thr)
				{
					if (success)
					{
						result = 19;
					}
					else
					{
						result = 18;
					}
				}
				else if (success)
				{
					result = 11;
				}
				else
				{
					result = 12;
				}
				break;
			case 0:
				if (1 == src_tar_thr)
				{
					if (success)
					{
						result = 1;
					}
					else
					{
						result = 2;
					}
				}
				else if (success)
				{
					result = 14;
				}
				else
				{
					result = 13;
				}
				break;
			case 1:
				if (0 == src_tar_thr)
				{
					if (success)
					{
						result = 14;
					}
					else
					{
						result = 13;
					}
				}
				else if (1 == src_tar_thr)
				{
					if (success)
					{
						result = 1;
					}
					else
					{
						result = 2;
					}
				}
				else if (2 == src_tar_thr)
				{
					if (success)
					{
						result = 21;
					}
					else
					{
						result = 20;
					}
				}
				break;
			case 2:
				if (0 == src_tar_thr)
				{
					if (success)
					{
						result = 23;
					}
					else
					{
						result = 22;
					}
				}
				else if (1 == src_tar_thr)
				{
					if (success)
					{
						result = 10;
					}
					else
					{
						result = 9;
					}
				}
				else if (2 == src_tar_thr)
				{
					if (success)
					{
						result = 24;
					}
					else
					{
						result = 25;
					}
				}
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public int TransHuDongTypeToLogType(int type, bool owner, bool success)
		{
			int result;
			switch (type)
			{
			case 1:
				if (!owner)
				{
					if (success)
					{
						result = 3;
					}
					else
					{
						result = 6;
					}
				}
				else
				{
					result = 15;
				}
				break;
			case 2:
				if (!owner)
				{
					if (success)
					{
						result = 4;
					}
					else
					{
						result = 7;
					}
				}
				else
				{
					result = 16;
				}
				break;
			case 3:
				if (!owner)
				{
					if (success)
					{
						result = 5;
					}
					else
					{
						result = 8;
					}
				}
				else
				{
					result = 17;
				}
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public void LoadYaoSaiData()
		{
			long num = TimeUtil.NOW();
			foreach (YaoSaiJianYuManorLevelConfig yaoSaiJianYuManorLevelConfig in this.RuntimeData.YaoSaiJianYuLevelDict.Values)
			{
				YaoSaiSearchParam loadParam = new YaoSaiSearchParam
				{
					MinUnionLev = yaoSaiJianYuManorLevelConfig.MinZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MinLevel,
					MaxUnionLev = yaoSaiJianYuManorLevelConfig.MaxZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MaxLevel,
					loadFuLu = false
				};
				List<KFPrisonRoleData> list = this.LoadYaoSaiPrisonRoleList(loadParam);
				if (list == null || list.Count == 0)
				{
					LogManager.WriteLog(0, string.Format("要塞监狱数据加载 LevelID={0} CountNum={1}", yaoSaiJianYuManorLevelConfig.ID, 0), null, true);
				}
				else
				{
					LogManager.WriteLog(0, string.Format("要塞监狱数据加载 LevelID={0} CountNum={1}", yaoSaiJianYuManorLevelConfig.ID, list.Count), null, true);
					foreach (KFPrisonRoleData kfprisonRoleData in list)
					{
						KFPrisonRoleAllData kfprisonRoleAllData = new KFPrisonRoleAllData();
						kfprisonRoleAllData.kfRoleData.V = kfprisonRoleData;
						TimeUtil.AgeByNow(ref kfprisonRoleAllData.kfRoleData.Age);
						kfprisonRoleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						this.YaoSaiPrisonRoleDataDict[kfprisonRoleData.RoleID] = kfprisonRoleAllData;
						this.UpdateYaoSaiPrisonSearchData(kfprisonRoleAllData, this.GetYaoSaiLevelID(kfprisonRoleData.UnionLevel));
					}
				}
			}
			LogManager.WriteLog(0, string.Format("要塞监狱数据加载 TakeTime={0}ms", TimeUtil.NOW() - num), null, true);
		}

		private bool LoadYaoSaiPrisonJingJiData(int roleID, KFPrisonJingJiData data)
		{
			try
			{
				string sqlstring = string.Format("SELECT roledata FROM t_yaosai_prison WHERE rid={0}", roleID);
				object single = DbHelperMySQL.GetSingle(sqlstring);
				if (null != single)
				{
					data.PlayerJingJiMirrorData = (single as byte[]);
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return false;
		}

		private List<KFPrisonRoleData> LoadYaoSaiPrisonRoleList(YaoSaiSearchParam loadParam)
		{
			List<KFPrisonRoleData> list = new List<KFPrisonRoleData>();
			MySqlDataReader mySqlDataReader = null;
			List<KFPrisonRoleData> result;
			try
			{
				string strSQL;
				if (loadParam.MinUnionLev != 0 || loadParam.MaxUnionLev != 0)
				{
					int num = 12000;
					strSQL = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE unionLevel>={0} AND unionLevel<={1} ORDER BY opttime DESC LIMIT 0,{2}", loadParam.MinUnionLev, loadParam.MaxUnionLev, num);
				}
				else if (!loadParam.loadFuLu)
				{
					strSQL = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE rid={0}", loadParam.roleID);
				}
				else
				{
					strSQL = string.Format("SELECT rid,rname,unionLevel,faction,sex,occupation,combatforce,zoneid,awardCount,countDayId,ownerid,fulutime,opttime FROM t_yaosai_prison WHERE ownerid={0}", loadParam.roleID);
				}
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num2 = 1;
				while (mySqlDataReader.Read())
				{
					KFPrisonRoleData kfprisonRoleData = new KFPrisonRoleData();
					kfprisonRoleData.RoleID = Convert.ToInt32(mySqlDataReader[0].ToString());
					kfprisonRoleData.RoleName = mySqlDataReader[1].ToString();
					kfprisonRoleData.UnionLevel = Convert.ToInt32(mySqlDataReader[2].ToString());
					kfprisonRoleData.Faction = Convert.ToInt32(mySqlDataReader[3].ToString());
					kfprisonRoleData.RoleSex = Convert.ToByte(mySqlDataReader[4].ToString());
					kfprisonRoleData.Occupation = Convert.ToByte(mySqlDataReader[5].ToString());
					kfprisonRoleData.CombatForce = Convert.ToInt32(mySqlDataReader[6].ToString());
					kfprisonRoleData.ZoneID = Convert.ToInt32(mySqlDataReader[7].ToString());
					kfprisonRoleData.AwardCount = Convert.ToInt32(mySqlDataReader[8].ToString());
					kfprisonRoleData.CountDayID = Convert.ToInt32(mySqlDataReader[9].ToString());
					kfprisonRoleData.OwnerID = Convert.ToInt32(mySqlDataReader[10].ToString());
					DateTime dateTime;
					DateTime.TryParse(mySqlDataReader[11].ToString(), out dateTime);
					kfprisonRoleData.FuLuTime = dateTime.Ticks;
					DateTime.TryParse(mySqlDataReader[12].ToString(), out dateTime);
					kfprisonRoleData.OptTime = dateTime.Ticks;
					list.Add(kfprisonRoleData);
					num2++;
				}
				result = list;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		private bool LoadYaoSaiPrisonLogList(int RoleID, List<KFPrisonLogData> LogListData)
		{
			MySqlDataReader mySqlDataReader = null;
			bool result;
			try
			{
				string strSQL = string.Format("SELECT id,roleid,introid,param1,param2,name1,name2,state \r\n                            FROM t_yaosai_prison_log WHERE roleid={0} ORDER BY id DESC LIMIT {1};", RoleID, 30);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					LogListData.Add(new KFPrisonLogData
					{
						ID = Convert.ToInt64(mySqlDataReader[0].ToString()),
						RoleID = Convert.ToInt32(mySqlDataReader[1].ToString()),
						IntroID = Convert.ToInt32(mySqlDataReader[2].ToString()),
						Param1 = Convert.ToInt32(mySqlDataReader[3].ToString()),
						Param2 = Convert.ToInt32(mySqlDataReader[4].ToString()),
						Name1 = mySqlDataReader[5].ToString(),
						Name2 = mySqlDataReader[6].ToString(),
						State = Convert.ToInt32(mySqlDataReader[7].ToString())
					});
					num++;
				}
				LogListData.Reverse();
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		private void SaveYaoSaiPrisonRoleDataDB(KFPrisonRoleData data, byte[] PlayerJingJiMirrorData = null)
		{
			DateTime dateTime = new DateTime(data.FuLuTime);
			DateTime dateTime2 = new DateTime(data.OptTime);
			if (null != PlayerJingJiMirrorData)
			{
				string text = string.Format("INSERT INTO `t_yaosai_prison` (`rid`,`rname`,`unionLevel`,`faction`,`sex`,`occupation`,`combatforce`,`zoneid`,`awardCount`,`countDayId`,`ownerid`,`fulutime`,`opttime`,`roledata`) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},'{11}','{12}',@content) on duplicate key update `rname`='{1}',`unionLevel`={2},`faction`={3},`sex`={4},`occupation`={5},`combatforce`={6},`zoneid`={7},`awardCount`={8}, `countDayId`={9},`ownerid`={10},`fulutime`='{11}',`opttime`='{12}',`roledata`=@content;", new object[]
				{
					data.RoleID,
					data.RoleName,
					data.UnionLevel,
					data.Faction,
					data.RoleSex,
					data.Occupation,
					data.CombatForce,
					data.ZoneID,
					data.AwardCount,
					data.CountDayID,
					data.OwnerID,
					dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					dateTime2.ToString("yyyy-MM-dd HH:mm:ss")
				});
				DbHelperMySQL.ExecuteSqlInsertImg(text, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", PlayerJingJiMirrorData)
				});
			}
			else
			{
				string text = string.Format("UPDATE `t_yaosai_prison` SET `rname`='{1}',`unionLevel`={2},`faction`={3},`sex`={4},`occupation`={5},`combatforce`={6},`zoneid`={7},`awardCount`={8},`countDayId`={9},`ownerid`={10},`fulutime`='{11}',`opttime`='{12}' WHERE rid={0}", new object[]
				{
					data.RoleID,
					data.RoleName,
					data.UnionLevel,
					data.Faction,
					data.RoleSex,
					data.Occupation,
					data.CombatForce,
					data.ZoneID,
					data.AwardCount,
					data.CountDayID,
					data.OwnerID,
					dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					dateTime2.ToString("yyyy-MM-dd HH:mm:ss")
				});
				this.ExecuteSqlNoQuery(text);
			}
		}

		private void InsertYaoSaiPrisonLogData(KFPrisonLogData data)
		{
			string sqlCmd = string.Format("INSERT INTO `t_yaosai_prison_log` (`roleid`,`introid`,`param1`,`param2`,`name1`,`name2`,`state`) VALUES ({0},{1},{2},{3},'{4}','{5}',{6});", new object[]
			{
				data.RoleID,
				data.IntroID,
				data.Param1,
				data.Param2,
				data.Name1,
				data.Name2,
				data.State
			});
			data.ID = this.ExecuteSqlGetIncrement(sqlCmd);
		}

		private void DeleteYaoSaiPrisonLogData(KFPrisonLogData data)
		{
			string sqlCmd = string.Format("DELETE FROM `t_yaosai_prison_log` WHERE `id`={0};", data.ID);
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(3, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		private long ExecuteSqlGetIncrement(string sqlCmd)
		{
			long result;
			try
			{
				LogManager.WriteLog(3, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSqlGetIncrement(sqlCmd, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1L;
			}
			return result;
		}

		private static YaoSaiService _instance = new YaoSaiService();

		private object Mutex = new object();

		private YaoSaiRuntimeData RuntimeData = new YaoSaiRuntimeData();

		private Dictionary<int, List<KFPrisonRoleAllData>> YaoSaiSearchDataDict = new Dictionary<int, List<KFPrisonRoleAllData>>();

		private Dictionary<int, KFPrisonRoleAllData> YaoSaiPrisonRoleDataDict = new Dictionary<int, KFPrisonRoleAllData>();

		private Dictionary<int, KFPrisonFuLuAllData> YaoSaiOwnerIDVsFuLuDict = new Dictionary<int, KFPrisonFuLuAllData>();

		private Dictionary<int, KFPrisonJingJiAllData> YaoSaiPrisonJingJiDataDict = new Dictionary<int, KFPrisonJingJiAllData>();

		private Dictionary<int, KFPrisonLogAllData> YaoSaiPrisonLogDataDict = new Dictionary<int, KFPrisonLogAllData>();
	}
}
