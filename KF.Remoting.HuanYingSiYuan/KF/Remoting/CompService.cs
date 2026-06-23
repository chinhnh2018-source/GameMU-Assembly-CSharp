using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class CompService
	{
		public static CompService Instance()
		{
			return CompService._instance;
		}

		private KuaFuData<Dictionary<int, KFCompData>> CompDataDict
		{
			get
			{
				return this.Persistence.CompDataDict;
			}
			set
			{
				this.Persistence.CompDataDict = value;
			}
		}

		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianDict
		{
			get
			{
				return this.Persistence.CompRankJunXianDict;
			}
			set
			{
				this.Persistence.CompRankJunXianDict = value;
			}
		}

		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianLastDict
		{
			get
			{
				return this.Persistence.CompRankJunXianLastDict;
			}
			set
			{
				this.Persistence.CompRankJunXianLastDict = value;
			}
		}

		private KuaFuData<List<KFCompRankInfo>> CompRankBossDamageList
		{
			get
			{
				return this.Persistence.CompRankBossDamageList;
			}
			set
			{
				this.Persistence.CompRankBossDamageList = value;
			}
		}

		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankBattleJiFenDict
		{
			get
			{
				return this.Persistence.CompRankBattleJiFenDict;
			}
			set
			{
				this.Persistence.CompRankBattleJiFenDict = value;
			}
		}

		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankMineJiFenDict
		{
			get
			{
				return this.Persistence.CompRankMineJiFenDict;
			}
			set
			{
				this.Persistence.CompRankMineJiFenDict = value;
			}
		}

		private Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict
		{
			get
			{
				return this.Persistence.CompRoleDataDict;
			}
			set
			{
				this.Persistence.CompRoleDataDict = value;
			}
		}

		public void InitConfig()
		{
			try
			{
				string paramValueByName = KuaFuServerManager.systemParamsList.GetParamValueByName("CompReplaceAmerce");
				this.CompReplaceAmerce = Global.SafeConvertToDouble(paramValueByName);
				string paramValueByName2 = KuaFuServerManager.systemParamsList.GetParamValueByName("CompNumEveryDay");
				string[] array = paramValueByName2.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					this.CompBoomValueReduce = Global.SafeConvertToInt32(array[0]);
					this.CompBoomValueMin = Global.SafeConvertToInt32(array[1]);
				}
				lock (this.Mutex)
				{
					string fileName = "Config/Comp.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompConfigDict.Clear();
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						CompConfig compConfig = new CompConfig();
						compConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "CompID", 0L);
						compConfig.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapCode", 0L);
						compConfig.BossID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MonstersID", 0L);
						compConfig.MaxPlayer = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MaxPlayer", 0L);
						this.CompConfigDict[compConfig.ID] = compConfig;
					}
					fileName = "Config/ForceCraft.xml";
					resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompBattleConfigDict.Clear();
					xelement = ConfigHelper.Load(resourcePath);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						CompBattleConfig compBattleConfig = new CompBattleConfig();
						compBattleConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						compBattleConfig.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapCode", 0L);
						compBattleConfig.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MaxEnterNum", 0L);
						compBattleConfig.EnterCD = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EnterCD", 0L);
						compBattleConfig.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "PrepareSecs", 0L);
						compBattleConfig.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "FightingSecs", 0L);
						compBattleConfig.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ClearRolesSecs", 0L);
						string[] array2 = xelement2.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < array2.Length; i += 3)
						{
							TimeSpan ts = new TimeSpan(Convert.ToInt32(array2[i]), 0, 0, 0);
							TimeSpan item = DateTime.Parse(array2[i + 1]).TimeOfDay.Add(ts);
							TimeSpan item2 = DateTime.Parse(array2[i + 2]).TimeOfDay.Add(ts);
							compBattleConfig.TimePoints.Add(item);
							compBattleConfig.TimePoints.Add(item2);
						}
						for (int i = 0; i < compBattleConfig.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(compBattleConfig.TimePoints[i].Hours, compBattleConfig.TimePoints[i].Minutes, compBattleConfig.TimePoints[i].Seconds);
							compBattleConfig.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						this.CompBattleConfigDict[compBattleConfig.ID] = compBattleConfig;
					}
					fileName = "Config/CompMineWar.xml";
					resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompMineConfigDict.Clear();
					xelement = ConfigHelper.Load(resourcePath);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						CompMineConfig compMineConfig = new CompMineConfig();
						compMineConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						compMineConfig.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapCode", 0L);
						compMineConfig.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MaxEnterNum", 0L);
						compMineConfig.EnterCD = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EnterCD", 0L);
						compMineConfig.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "PrepareSecs", 0L);
						compMineConfig.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "FightingSecs", 0L);
						compMineConfig.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ClearRolesSecs", 0L);
						string[] array2 = xelement2.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < array2.Length; i += 3)
						{
							TimeSpan ts = new TimeSpan(Convert.ToInt32(array2[i]), 0, 0, 0);
							TimeSpan item = DateTime.Parse(array2[i + 1]).TimeOfDay.Add(ts);
							TimeSpan item2 = DateTime.Parse(array2[i + 2]).TimeOfDay.Add(ts);
							compMineConfig.TimePoints.Add(item);
							compMineConfig.TimePoints.Add(item2);
						}
						for (int i = 0; i < compMineConfig.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(compMineConfig.TimePoints[i].Hours, compMineConfig.TimePoints[i].Minutes, compMineConfig.TimePoints[i].Seconds);
							compMineConfig.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						this.CompMineConfigDict[compMineConfig.ID] = compMineConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void LoadDatabase(DateTime now)
		{
			try
			{
				this.Persistence.LoadDatabase();
				this.InitServerCompData();
				this.UpdateCompRankBattleJiFen(now);
				this.UpdateCompRankMineJiFen(now);
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
				this.LastUpdateHour = now.Hour;
				this.CompDataDayID = this.Persistence.GetCompDayID();
				this.CompDataWeekDayID = this.Persistence.GetCompWeekDayID();
				this.CompBattleWeekDayID = this.Persistence.GetCompBattleWeekDayID();
				this.CompMineWeekDayID = this.Persistence.GetCompMineWeekDayID();
				this.HandleChangeDay(now);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "CompService.LoadDatabase failed!", ex, true);
			}
		}

		public void OnStopServer()
		{
			this.Persistence.DelayWriteDataProc();
		}

		public void Update(DateTime now)
		{
			try
			{
				this.Persistence.DelayWriteDataProc();
				if (now > this.CheckTime10)
				{
					this.CheckTime10 = now.AddSeconds(10.0);
					this.CheckRoleTimerProc(now);
					this.CheckGameFuBenTimerProc(now);
					this.HandleCompMineLogicSomething(now);
					this.HandleCompBattleLogicSomething(now);
				}
				int hour = now.Hour;
				if (hour != this.LastUpdateHour)
				{
					this.HandleChangeHour(now);
					this.LastUpdateHour = hour;
				}
				int offsetDay = TimeUtil.GetOffsetDay(now);
				if (offsetDay != this.LastUpdateDayID)
				{
					this.HandleChangeDay(now);
					this.LastUpdateDayID = offsetDay;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "CompService.Update failed!", ex, true);
			}
		}

		private void InitServerCompData()
		{
			for (int i = 1; i <= 3; i++)
			{
				KFCompData kfcompData = null;
				if (!this.CompDataDict.V.TryGetValue(i, out kfcompData))
				{
					kfcompData = new KFCompData();
					kfcompData.InitPlunderResList();
					kfcompData.CompType = i;
					this.CompDataDict.V[i] = kfcompData;
					TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					this.Persistence.SaveCompData(kfcompData, false);
				}
			}
		}

		private void HandleChangeHour(DateTime now)
		{
			lock (this.Mutex)
			{
				for (int i = 1; i <= 3; i++)
				{
					this.Persistence.LoadCompRankInfo(1, i, this.CompRankJunXianDict, null);
				}
			}
		}

		private void HandleChangeDay(DateTime now)
		{
			int offsetDay = TimeUtil.GetOffsetDay(now);
			int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
			if (this.CompDataDayID == 0 || 0 == this.CompDataWeekDayID)
			{
				this.CompDataDayID = offsetDay;
				this.CompDataWeekDayID = weekStartDayIdNow;
				this.Persistence.SaveCompDayID(this.CompDataDayID);
				this.Persistence.SaveCompWeekDayID(this.CompDataWeekDayID);
			}
			else
			{
				lock (this.Mutex)
				{
					if (offsetDay != this.CompDataDayID)
					{
						foreach (KFCompData kfcompData in this.CompDataDict.V.Values)
						{
							if (0 != kfcompData.EnemyCompTypeSet)
							{
								kfcompData.EnemyCompType = kfcompData.EnemyCompTypeSet;
								kfcompData.EnemyCompTypeSet = 0;
							}
							kfcompData.YestdCrystal = kfcompData.Crystal;
							kfcompData.Crystal = 0;
							kfcompData.YestdBoss = kfcompData.Boss;
							kfcompData.Boss = 0;
							kfcompData.YestdBossKillCompType = kfcompData.BossKillCompType;
							kfcompData.BossKillCompType = 0;
							for (int i = 1; i <= 3; i++)
							{
								kfcompData.YestdPlunderResList[i - 1] = kfcompData.PlunderResList[i - 1];
								kfcompData.PlunderResList[i - 1] = 0;
							}
							if (kfcompData.BoomValue > this.CompBoomValueMin)
							{
								kfcompData.BoomValue -= Math.Min(this.CompBoomValueReduce, kfcompData.BoomValue - this.CompBoomValueMin);
							}
							kfcompData.YestdBoomValue = kfcompData.BoomValue;
							this.Persistence.SaveCompData(kfcompData, true);
						}
						TimeUtil.AgeByNow(ref this.CompDataDict.Age);
						this.CompDataDayID = offsetDay;
						this.Persistence.SaveCompDayID(this.CompDataDayID);
					}
					if (weekStartDayIdNow != this.CompDataWeekDayID)
					{
						for (int j = 1; j <= 3; j++)
						{
							this.Persistence.LoadCompRankInfo(1, j, this.CompRankJunXianDict, null);
						}
						foreach (KuaFuData<KFCompRoleData> kuaFuData in this.CompRoleDataDict.Values)
						{
							if (kuaFuData.V.JunXianLast != 0 || kuaFuData.V.JunXian != 0 || kuaFuData.V.CompTypeLast != kuaFuData.V.CompType)
							{
								kuaFuData.V.JunXianLast = kuaFuData.V.JunXian;
								kuaFuData.V.JunXian = 0;
								kuaFuData.V.CompTypeLast = kuaFuData.V.CompType;
								TimeUtil.AgeByNow(ref kuaFuData.Age);
								this.Persistence.SaveCompRoleData(kuaFuData.V, false, true, false, false);
							}
						}
						for (int j = 1; j <= 3; j++)
						{
							List<KFCompRankInfo> list = null;
							this.CompRankJunXianDict.V.TryGetValue(j, out list);
							this.CompRankJunXianLastDict.V[j] = new List<KFCompRankInfo>(list);
							list.Clear();
						}
						TimeUtil.AgeByNow(ref this.CompRankJunXianDict.Age);
						TimeUtil.AgeByNow(ref this.CompRankJunXianLastDict.Age);
						this.CompDataWeekDayID = weekStartDayIdNow;
						this.Persistence.SaveCompWeekDayID(this.CompDataWeekDayID);
					}
				}
			}
		}

		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			try
			{
				CompSyncData compSyncData = new CompSyncData();
				compSyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(27));
				compSyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(28));
				compSyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(29));
				lock (this.Mutex)
				{
					compSyncData.BytesCompMapDataDict = DataHelper2.ObjectToBytes<Dictionary<int, CompMapData>>(this.CompMapDataDict);
					compSyncData.CompDataDict.Age = this.CompDataDict.Age;
					compSyncData.CompRankJunXianDict.Age = this.CompRankJunXianDict.Age;
					compSyncData.CompRankJunXianLastDict.Age = this.CompRankJunXianLastDict.Age;
					compSyncData.CompRankBossDamageList.Age = this.CompRankBossDamageList.Age;
					compSyncData.CompRankBattleJiFenDict.Age = this.CompRankBattleJiFenDict.Age;
					compSyncData.CompRankMineJiFenDict.Age = this.CompRankMineJiFenDict.Age;
					if (ageComp != this.CompDataDict.Age)
					{
						compSyncData.BytesCompDataDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, KFCompData>>>(this.CompDataDict);
					}
					if (ageRankJX != this.CompRankJunXianDict.Age)
					{
						compSyncData.BytesCompRankJunXianDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(this.CompRankJunXianDict);
					}
					if (ageRankJXL != this.CompRankJunXianLastDict.Age)
					{
						compSyncData.BytesCompRankJunXianLastDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(this.CompRankJunXianLastDict);
					}
					if (ageRankBD != this.CompRankBossDamageList.Age)
					{
						compSyncData.BytesCompRankBossDamageList = DataHelper2.ObjectToBytes<KuaFuData<List<KFCompRankInfo>>>(this.CompRankBossDamageList);
					}
					if (ageRankBJF != this.CompRankBattleJiFenDict.Age)
					{
						compSyncData.CompBattleJoinRoleNum = this.CompBattleJoinRoleNum;
						compSyncData.BytesCompRankBattleJiFenDict = this.BytesCompRankBattleJiFenDict;
					}
					if (ageRankBJF != this.CompRankMineJiFenDict.Age)
					{
						compSyncData.CompMineJoinRoleNum = this.CompMineJoinRoleNum;
						compSyncData.BytesCompRankMineJiFenDict = this.BytesCompRankMineJiFenDict;
					}
					return compSyncData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		private int GetZhiWuByRankJunXianLast(int compType, int rid)
		{
			List<KFCompRankInfo> list = null;
			this.CompRankJunXianLastDict.V.TryGetValue(compType, out list);
			int result;
			if (list == null || list.Count == 0)
			{
				result = 0;
			}
			else
			{
				int num = list.FindIndex((KFCompRankInfo x) => x.Key == rid) + 1;
				result = ((num > 5) ? 0 : num);
			}
			return result;
		}

		public KuaFuCmdData GetCompRoleData(int roleId, long dataAge)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFCompRoleData> kuaFuData = null;
					if (!this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
					{
						return null;
					}
					if (dataAge != kuaFuData.Age)
					{
						kuaFuData.V.ZhiWu = this.GetZhiWuByRankJunXianLast(kuaFuData.V.CompType, roleId);
						return new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFCompRoleData>(kuaFuData.V)
						};
					}
					return new KuaFuCmdData
					{
						Age = kuaFuData.Age
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public void ChangeName(int roleId, string roleName)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFCompRoleData> kuaFuData = null;
					if (this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
					{
						kuaFuData.V.RoleName = roleName;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, false, false);
						for (int i = 1; i <= 3; i++)
						{
							List<KFCompRankInfo> list = null;
							if (this.CompRankJunXianDict.V.TryGetValue(i, out list))
							{
								foreach (KFCompRankInfo kfcompRankInfo in list)
								{
									if (kfcompRankInfo.Key == roleId)
									{
										kfcompRankInfo.Param1 = KuaFuServerManager.FormatName(roleName, kfcompRankInfo.tagInfo.V.ZoneID);
										TimeUtil.AgeByNow(ref this.CompRankJunXianDict.Age);
									}
								}
							}
						}
						for (int i = 1; i <= 3; i++)
						{
							List<KFCompRankInfo> list = null;
							if (this.CompRankJunXianLastDict.V.TryGetValue(i, out list))
							{
								foreach (KFCompRankInfo kfcompRankInfo in list)
								{
									if (kfcompRankInfo.Key == roleId)
									{
										kfcompRankInfo.Param1 = KuaFuServerManager.FormatName(roleName, kfcompRankInfo.tagInfo.V.ZoneID);
										TimeUtil.AgeByNow(ref this.CompRankJunXianLastDict.Age);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			int num = 0;
			int result;
			if (compType < 1 || compType > 3)
			{
				num = -12;
				result = num;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFCompRoleData> kuaFuData = null;
						if (this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
						{
							num = -12;
							return num;
						}
						kuaFuData = new KuaFuData<KFCompRoleData>();
						kuaFuData.V.RoleID = roleId;
						kuaFuData.V.ZoneID = zoneId;
						kuaFuData.V.RoleName = roleName;
						kuaFuData.V.CompType = compType;
						kuaFuData.V.CompTypeBattle = compType;
						kuaFuData.V.BattleJiFen = battleJiFen;
						this.CompRoleDataDict[roleId] = kuaFuData;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						this.Persistence.SaveCompRoleData(kuaFuData.V, true, true, false, false);
						LogManager.WriteLog(5, string.Format("Comp_JoinComp_Repair roleId={0} zoneId={1} roleName={2} compType={3} battleJiFen={4}", new object[]
						{
							roleId,
							zoneId,
							roleName,
							compType,
							battleJiFen
						}), null, true);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = num;
			}
			return result;
		}

		public int JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			int num = 0;
			int result;
			if (compType < 1 || compType > 3)
			{
				num = -12;
				result = num;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfcompData = null;
						if (!this.CompDataDict.V.TryGetValue(compType, out kfcompData))
						{
							kfcompData = new KFCompData();
							kfcompData.InitPlunderResList();
							kfcompData.CompType = compType;
							this.CompDataDict.V[compType] = kfcompData;
							TimeUtil.AgeByNow(ref this.CompDataDict.Age);
							this.Persistence.SaveCompData(kfcompData, true);
						}
						KuaFuData<KFCompRoleData> kuaFuData = null;
						if (!this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KFCompRoleData>();
							kuaFuData.V.RoleID = roleId;
							kuaFuData.V.ZoneID = zoneId;
							kuaFuData.V.RoleName = roleName;
							kuaFuData.V.CompType = compType;
							this.CompRoleDataDict[roleId] = kuaFuData;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.SaveCompRoleData(kuaFuData.V, true, true, false, false);
						}
						else
						{
							CompBattleGameStates compBattleGameStates = this.GetCompBattleGameStates(TimeUtil.NowDateTime());
							if (compBattleGameStates != 0)
							{
								num = -12;
								return num;
							}
							compBattleGameStates = this.GetCompMineGameStates(TimeUtil.NowDateTime());
							if (compBattleGameStates != 0)
							{
								num = -12;
								return num;
							}
							kuaFuData.V.RoleName = roleName;
							kuaFuData.V.CompType = compType;
							kuaFuData.V.JunXian = (int)((double)kuaFuData.V.JunXian * this.CompReplaceAmerce);
							kuaFuData.V.BattleJiFen = 0;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.SaveCompRoleData(kuaFuData.V, true, false, false, false);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = num;
			}
			return result;
		}

		public void CompOpt(int compType, int optType, int param1, int param2)
		{
			if (compType >= 1 && compType <= 3)
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfcompData = null;
						if (this.CompDataDict.V.TryGetValue(compType, out kfcompData))
						{
							switch (optType)
							{
							case 0:
								kfcompData.BoomValue += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							case 1:
							{
								KuaFuData<KFCompRoleData> kuaFuData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kuaFuData))
								{
									kuaFuData.V.JunXian += param2;
									TimeUtil.AgeByNow(ref kuaFuData.Age);
									this.Persistence.SaveCompRoleData(kuaFuData.V, true, false, false, false);
								}
								break;
							}
							case 2:
							{
								List<int> plunderResList;
								int index;
								(plunderResList = kfcompData.PlunderResList)[index = param1 - 1] = plunderResList[index] + param2;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							}
							case 3:
								kfcompData.EnemyCompTypeSet = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							case 4:
								kfcompData.BossKillCompType = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							case 5:
								kfcompData.Boss += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							case 6:
								kfcompData.Crystal += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								break;
							case 7:
							{
								kfcompData.BossDamageTop = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfcompData, true);
								List<KFCompRankInfo> v = this.CompRankBossDamageList.V;
								v[compType - 1].Value = param1;
								TimeUtil.AgeByNow(ref this.CompRankBossDamageList.Age);
								break;
							}
							case 8:
							{
								KuaFuData<KFCompRoleData> kuaFuData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kuaFuData))
								{
									kuaFuData.V.BattleJiFen += param2;
									kuaFuData.V.RankTmBJF = TimeUtil.NowDateTime();
									kuaFuData.V.CompTypeBattle = kuaFuData.V.CompType;
									if (!this.CompBattleJiFenRoleSet.Contains(param1))
									{
										this.CompBattleJiFenRoleSet.Add(param1);
										List<KFCompRankInfo> list = null;
										if (this.CompRankBattleJiFenDict.V.TryGetValue(kuaFuData.V.CompType, out list))
										{
											list.Add(new KFCompRankInfo
											{
												Key = param1,
												Param1 = KuaFuServerManager.FormatName(kuaFuData.V.RoleName, kuaFuData.V.ZoneID),
												tagInfo = kuaFuData
											});
										}
									}
									TimeUtil.AgeByNow(ref kuaFuData.Age);
									this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, true, false);
								}
								break;
							}
							case 9:
							{
								kfcompData.MineRes += param1;
								List<KFCompData> list2 = this.CompDataDict.V.Values.ToList<KFCompData>();
								list2.Sort(delegate(KFCompData left, KFCompData right)
								{
									int result;
									if (left.MineRes > right.MineRes)
									{
										result = -1;
									}
									else if (left.MineRes < right.MineRes)
									{
										result = 1;
									}
									else if (left.MineRank < right.MineRank)
									{
										result = -1;
									}
									else if (left.MineRank > right.MineRank)
									{
										result = 1;
									}
									else if (left.CompType < right.CompType)
									{
										result = -1;
									}
									else if (left.CompType > right.CompType)
									{
										result = 1;
									}
									else
									{
										result = 0;
									}
									return result;
								});
								for (int i = 0; i < list2.Count; i++)
								{
									KFCompData kfcompData2 = list2[i];
									kfcompData2.MineRank = i + 1;
									this.Persistence.SaveCompData(kfcompData2, true);
								}
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								break;
							}
							case 10:
							{
								KuaFuData<KFCompRoleData> kuaFuData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kuaFuData))
								{
									kuaFuData.V.MineJiFen += param2;
									kuaFuData.V.RankTmMJF = TimeUtil.NowDateTime();
									kuaFuData.V.CompTypeMine = kuaFuData.V.CompType;
									if (!this.CompMineJiFenRoleSet.Contains(param1))
									{
										this.CompMineJiFenRoleSet.Add(param1);
										List<KFCompRankInfo> list3 = null;
										if (this.CompRankMineJiFenDict.V.TryGetValue(kuaFuData.V.CompType, out list3))
										{
											list3.Add(new KFCompRankInfo
											{
												Key = param1,
												Param1 = string.Format("S{0}·{1}", kuaFuData.V.ZoneID, kuaFuData.V.RoleName),
												tagInfo = kuaFuData
											});
										}
									}
									TimeUtil.AgeByNow(ref kuaFuData.Age);
									this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, true, false);
								}
								break;
							}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		public void SetBulletin(int compType, string bulletin)
		{
			if (compType >= 1 && compType <= 3)
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfcompData = null;
						if (this.CompDataDict.V.TryGetValue(compType, out kfcompData))
						{
							kfcompData.Bulletin = bulletin;
							TimeUtil.AgeByNow(ref this.CompDataDict.Age);
							this.Persistence.SaveCompData(kfcompData, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		public void BroadCastCompNotice(int serverId, byte[] bytes)
		{
			try
			{
				ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(34, new object[]
				{
					bytes
				}), serverId);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void CompChat(int serverId, byte[] bytes)
		{
			try
			{
				AsyncDataItem evItem = new AsyncDataItem(35, new object[]
				{
					bytes
				});
				HashSet<int> hashSet = new HashSet<int>();
				hashSet.Add(KuaFuServerManager.GetSpecialLineId(27));
				hashSet.Add(KuaFuServerManager.GetSpecialLineId(28));
				hashSet.Add(KuaFuServerManager.GetSpecialLineId(29));
				foreach (KuaFuData<CompFuBenData> kuaFuData in this.CompFuBenDataDict.Values)
				{
					hashSet.Add(kuaFuData.V.ServerId);
				}
				foreach (int num in hashSet)
				{
					if (serverId != num)
					{
						ClientAgentManager.Instance().PostAsyncEvent(num, this.EvItemGameType, evItem);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void SetRoleData4Selector(int roleId, byte[] bytes)
		{
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(KuaFuServerManager.GetSpecialLineId(27));
			hashSet.Add(KuaFuServerManager.GetSpecialLineId(28));
			hashSet.Add(KuaFuServerManager.GetSpecialLineId(29));
			lock (this.Mutex)
			{
				KuaFuData<KFCompRoleData> kuaFuData = null;
				if (this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
				{
					kuaFuData.V.RoleData4Selector = bytes;
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, false, false);
					AsyncDataItem evItem = new AsyncDataItem(36, new object[]
					{
						roleId
					});
					foreach (int serverId in hashSet)
					{
						ClientAgentManager.Instance().PostAsyncEvent(serverId, this.EvItemGameType, evItem);
					}
				}
			}
		}

		public void UpdateMapRoleNum(int mapCode, int roleNum)
		{
			GameTypes gameType = 0;
			CompConfig compConfig = null;
			CompMapData compMapData = null;
			lock (this.Mutex)
			{
				compConfig = this.CompConfigDict.Values.ToList<CompConfig>().Find((CompConfig x) => x.MapCode == mapCode);
				if (null == compConfig)
				{
					return;
				}
				if (!this.CompMapDataDict.TryGetValue(mapCode, out compMapData))
				{
					compMapData = new CompMapData();
					this.CompMapDataDict[mapCode] = compMapData;
				}
				if (compConfig.ID == 1)
				{
					gameType = 27;
				}
				if (compConfig.ID == 2)
				{
					gameType = 28;
				}
				if (compConfig.ID == 3)
				{
					gameType = 29;
				}
			}
			int specialLineId = KuaFuServerManager.GetSpecialLineId(gameType);
			if (specialLineId != compMapData.ServerId)
			{
				if (0 != compMapData.ServerId)
				{
					ClientAgentManager.Instance().RemoveKfFuben(gameType, compMapData.ServerId, compMapData.GameId);
				}
				int serverId = 0;
				int nextGameId = TianTiPersistence.Instance.GetNextGameId();
				if (ClientAgentManager.Instance().AssginKfFuben(gameType, (long)nextGameId, compConfig.MaxPlayer, out serverId))
				{
					compMapData.GameId = (long)nextGameId;
					compMapData.Type = (byte)compConfig.ID;
					compMapData.ServerId = serverId;
				}
			}
			compMapData.roleNum = roleNum;
		}

		private CompBattleGameStates GetCompMineGameStates(DateTime now)
		{
			CompBattleGameStates compBattleGameStates = 0;
			CompMineConfig compMineConfig = this.CompMineConfigDict.Values.FirstOrDefault<CompMineConfig>();
			CompBattleGameStates result;
			if (null == compMineConfig)
			{
				result = compBattleGameStates;
			}
			else
			{
				for (int i = 0; i < compMineConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)compMineConfig.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= compMineConfig.SecondsOfDay[i] - (double)compMineConfig.PrepareSecs && now.TimeOfDay.TotalSeconds <= compMineConfig.SecondsOfDay[i + 1])
					{
						if (now.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i] + (double)(compMineConfig.PrepareSecs / 2))
						{
							compBattleGameStates = -1;
						}
						else if (now.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i] + (double)compMineConfig.PrepareSecs)
						{
							compBattleGameStates = -2;
						}
						else if (now.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i + 1] - (double)compMineConfig.ClearRolesSecs)
						{
							compBattleGameStates = 1;
						}
						else if (now.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i + 1])
						{
							compBattleGameStates = 4;
						}
					}
				}
				result = compBattleGameStates;
			}
			return result;
		}

		private CompBattleGameStates GetCompBattleGameStates(DateTime now)
		{
			CompBattleGameStates compBattleGameStates = 0;
			CompBattleConfig compBattleConfig = this.CompBattleConfigDict.Values.FirstOrDefault<CompBattleConfig>();
			CompBattleGameStates result;
			if (null == compBattleConfig)
			{
				result = compBattleGameStates;
			}
			else
			{
				for (int i = 0; i < compBattleConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)compBattleConfig.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= compBattleConfig.SecondsOfDay[i] - (double)compBattleConfig.PrepareSecs && now.TimeOfDay.TotalSeconds <= compBattleConfig.SecondsOfDay[i + 1])
					{
						if (now.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i] + (double)(compBattleConfig.PrepareSecs / 2))
						{
							compBattleGameStates = -1;
						}
						else if (now.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i] + (double)compBattleConfig.PrepareSecs)
						{
							compBattleGameStates = -2;
						}
						else if (now.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i + 1] - (double)compBattleConfig.ClearRolesSecs)
						{
							compBattleGameStates = 1;
						}
						else if (now.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i + 1])
						{
							compBattleGameStates = 4;
						}
					}
				}
				result = compBattleGameStates;
			}
			return result;
		}

		private void HandleCompMineLogicSomething(DateTime now)
		{
			CompBattleGameStates compMineGameStates = this.GetCompMineGameStates(now);
			if (0 != compMineGameStates)
			{
				int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
				if (-2 == compMineGameStates && this.CompMineWeekDayID != weekStartDayIdNow)
				{
					foreach (KuaFuData<KFCompRoleData> kuaFuData in this.CompRoleDataDict.Values)
					{
						if (kuaFuData.V.MineJiFen > 0 || kuaFuData.V.CompTypeMine != kuaFuData.V.CompType)
						{
							kuaFuData.V.MineRankNum = 0;
							kuaFuData.V.MineJiFen = 0;
							kuaFuData.V.CompTypeMine = kuaFuData.V.CompType;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, false, false);
						}
					}
					for (int i = 1; i <= 3; i++)
					{
						this.CompMineJoinRoleNum[i - 1] = 0;
						List<KFCompRankInfo> list = null;
						if (this.CompRankMineJiFenDict.V.TryGetValue(i, out list))
						{
							list.Clear();
						}
					}
					this.CompMineJiFenRoleSet.Clear();
					foreach (KFCompData kfcompData in this.CompDataDict.V.Values)
					{
						kfcompData.MineRes = 0;
					}
					TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					this.CompMineWeekDayID = weekStartDayIdNow;
					this.Persistence.SaveCompMineWeekDayID(this.CompMineWeekDayID);
					AsyncDataItem evItem = new AsyncDataItem(38, new object[0]);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, evItem, 0);
				}
				this.UpdateCompRankMineJiFen(now);
			}
		}

		private void HandleCompBattleLogicSomething(DateTime now)
		{
			CompBattleGameStates compBattleGameStates = this.GetCompBattleGameStates(now);
			if (0 != compBattleGameStates)
			{
				int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
				if (-2 == compBattleGameStates && this.CompBattleWeekDayID != weekStartDayIdNow)
				{
					foreach (KuaFuData<KFCompRoleData> kuaFuData in this.CompRoleDataDict.Values)
					{
						if (kuaFuData.V.BattleJiFen > 0 || kuaFuData.V.CompTypeBattle != kuaFuData.V.CompType)
						{
							kuaFuData.V.BattleRankNum = 0;
							kuaFuData.V.BattleJiFen = 0;
							kuaFuData.V.CompTypeBattle = kuaFuData.V.CompType;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.SaveCompRoleData(kuaFuData.V, false, false, false, false);
						}
					}
					for (int i = 1; i <= 3; i++)
					{
						this.CompBattleJoinRoleNum[i - 1] = 0;
						List<KFCompRankInfo> list = null;
						if (this.CompRankBattleJiFenDict.V.TryGetValue(i, out list))
						{
							list.Clear();
						}
					}
					this.CompBattleJiFenRoleSet.Clear();
					this.CompBattleWeekDayID = weekStartDayIdNow;
					this.Persistence.SaveCompBattleWeekDayID(this.CompBattleWeekDayID);
					AsyncDataItem evItem = new AsyncDataItem(37, new object[0]);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, evItem, 0);
				}
				this.UpdateCompRankBattleJiFen(now);
			}
		}

		private void UpdateCompRankMineJiFen(DateTime now)
		{
			KuaFuData<Dictionary<int, List<KFCompRankInfo>>> kuaFuData = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
			for (int i = 1; i <= 3; i++)
			{
				this.CompMineJoinRoleNum[i - 1] = 0;
				List<KFCompRankInfo> list = null;
				if (this.CompRankMineJiFenDict.V.TryGetValue(i, out list))
				{
					list.Sort(delegate(KFCompRankInfo left, KFCompRankInfo right)
					{
						int result;
						if (left.tagInfo.V.MineJiFen > right.tagInfo.V.MineJiFen)
						{
							result = -1;
						}
						else if (left.tagInfo.V.MineJiFen < right.tagInfo.V.MineJiFen)
						{
							result = 1;
						}
						else if (left.tagInfo.V.RankTmBJF < right.tagInfo.V.RankTmBJF)
						{
							result = -1;
						}
						else if (left.tagInfo.V.RankTmBJF > right.tagInfo.V.RankTmBJF)
						{
							result = 1;
						}
						else if (left.Key > right.Key)
						{
							result = -1;
						}
						else if (left.Key < right.Key)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int j = 0; j < list.Count; j++)
					{
						KFCompRankInfo kfcompRankInfo = list[j];
						KuaFuData<KFCompRoleData> kuaFuData2 = null;
						if (this.CompRoleDataDict.TryGetValue(kfcompRankInfo.Key, out kuaFuData2))
						{
							kfcompRankInfo.Value = kuaFuData2.V.MineJiFen;
							this.CompMineJiFenRoleSet.Add(kfcompRankInfo.Key);
							if (kuaFuData2.V.MineRankNum != j + 1)
							{
								kuaFuData2.V.MineRankNum = j + 1;
								TimeUtil.AgeByNow(ref kuaFuData2.Age);
							}
						}
					}
					this.CompMineJoinRoleNum[i - 1] = list.Count;
					kuaFuData.V[i] = list.GetRange(0, Math.Min(list.Count, 50));
				}
			}
			TimeUtil.AgeByNow(ref this.CompRankMineJiFenDict.Age);
			kuaFuData.Age = this.CompRankMineJiFenDict.Age;
			this.BytesCompRankMineJiFenDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(kuaFuData);
		}

		private void UpdateCompRankBattleJiFen(DateTime now)
		{
			KuaFuData<Dictionary<int, List<KFCompRankInfo>>> kuaFuData = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
			for (int i = 1; i <= 3; i++)
			{
				this.CompBattleJoinRoleNum[i - 1] = 0;
				List<KFCompRankInfo> list = null;
				if (this.CompRankBattleJiFenDict.V.TryGetValue(i, out list))
				{
					list.Sort(delegate(KFCompRankInfo left, KFCompRankInfo right)
					{
						int result;
						if (left.tagInfo.V.BattleJiFen > right.tagInfo.V.BattleJiFen)
						{
							result = -1;
						}
						else if (left.tagInfo.V.BattleJiFen < right.tagInfo.V.BattleJiFen)
						{
							result = 1;
						}
						else if (left.tagInfo.V.RankTmBJF < right.tagInfo.V.RankTmBJF)
						{
							result = -1;
						}
						else if (left.tagInfo.V.RankTmBJF > right.tagInfo.V.RankTmBJF)
						{
							result = 1;
						}
						else if (left.Key > right.Key)
						{
							result = -1;
						}
						else if (left.Key < right.Key)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int j = 0; j < list.Count; j++)
					{
						KFCompRankInfo kfcompRankInfo = list[j];
						KuaFuData<KFCompRoleData> kuaFuData2 = null;
						if (this.CompRoleDataDict.TryGetValue(kfcompRankInfo.Key, out kuaFuData2))
						{
							kfcompRankInfo.Value = kuaFuData2.V.BattleJiFen;
							this.CompBattleJiFenRoleSet.Add(kfcompRankInfo.Key);
							if (kuaFuData2.V.BattleRankNum != j + 1)
							{
								kuaFuData2.V.BattleRankNum = j + 1;
								TimeUtil.AgeByNow(ref kuaFuData2.Age);
							}
						}
					}
					this.CompBattleJoinRoleNum[i - 1] = list.Count;
					kuaFuData.V[i] = list.GetRange(0, Math.Min(list.Count, 50));
				}
			}
			TimeUtil.AgeByNow(ref this.CompRankBattleJiFenDict.Age);
			kuaFuData.Age = this.CompRankBattleJiFenDict.Age;
			this.BytesCompRankBattleJiFenDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(kuaFuData);
		}

		private void CheckRoleTimerProc(DateTime now)
		{
			if (this.CompFuBenDataDict != null && this.CompFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					foreach (KuaFuData<CompFuBenData> kuaFuData in this.CompFuBenDataDict.Values)
					{
						List<int> list = new List<int>(new int[kuaFuData.V.EnterGameRoleCount.Count]);
						List<int> list2 = new List<int>();
						foreach (CompFuBenRoleData compFuBenRoleData in kuaFuData.V.RoleDict.Values)
						{
							if (compFuBenRoleData.State == 4)
							{
								if (compFuBenRoleData.StateEndTime < now)
								{
									compFuBenRoleData.State = 0;
									list2.Add(compFuBenRoleData.RoleId);
								}
								else
								{
									List<int> list3;
									int index;
									(list3 = list)[index = compFuBenRoleData.Side - 1] = list3[index] + 1;
								}
							}
						}
						kuaFuData.V.EnterGameRoleCount = list;
						foreach (int key in list2)
						{
							kuaFuData.V.RoleDict.Remove(key);
						}
						TimeUtil.AgeByNow(ref kuaFuData.Age);
					}
				}
			}
		}

		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.CompFuBenDataDict != null && this.CompFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
					foreach (KeyValuePair<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> keyValuePair in this.CompFuBenDataDict)
					{
						if (keyValuePair.Value.V.EndTime < now)
						{
							list.Add(keyValuePair.Key);
							LogManager.WriteLog(2, string.Format("势力战场副本数据清除 gameType={0} gameId={1} state={2} endtm={3}", new object[]
							{
								keyValuePair.Key.Key,
								keyValuePair.Value.V.GameId,
								keyValuePair.Value.V.State,
								keyValuePair.Value.V.EndTime
							}), null, true);
							GameTypes key = keyValuePair.Key.Key;
							ClientAgentManager.Instance().RemoveKfFuben(key, keyValuePair.Value.V.ServerId, (long)keyValuePair.Value.V.GameId);
						}
					}
					foreach (KeyValuePair<int, int> key2 in list)
					{
						this.CompFuBenDataDict.Remove(key2);
					}
				}
			}
		}

		public void UpdateFuBenMapRoleNum(int gameType, CompFuBenData fubenItem)
		{
			try
			{
				lock (this.Mutex)
				{
					if (fubenItem.RoleCountSideList != null && fubenItem.RoleCountSideList.Count > 0)
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(gameType, fubenItem.GameId);
						KuaFuData<CompFuBenData> kuaFuData = null;
						if (this.CompFuBenDataDict.TryGetValue(key, out kuaFuData))
						{
							kuaFuData.V.RoleCountSideList = fubenItem.RoleCountSideList;
							kuaFuData.V.ZhuJiangRoleDict = fubenItem.ZhuJiangRoleDict;
							kuaFuData.V.MineTruckGo = fubenItem.MineTruckGo;
							kuaFuData.V.MineSafeArrived = fubenItem.MineSafeArrived;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			try
			{
				lock (this.Mutex)
				{
					if (shDataList.Count == 3)
					{
						for (int i = 1; i <= 3; i++)
						{
							KFCompData kfcompData = null;
							if (this.CompDataDict.V.TryGetValue(i, out kfcompData))
							{
								kfcompData.StrongholdDict[cityID] = shDataList[i - 1];
								this.Persistence.SaveCompData(kfcompData, true);
							}
						}
						TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public int GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			lock (this.Mutex)
			{
				CompBattleConfig compBattleConfig = null;
				if (!this.CompBattleConfigDict.TryGetValue(cityID, out compBattleConfig))
				{
					return -11003;
				}
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(gameType, cityID);
				KuaFuData<CompFuBenData> kuaFuData = null;
				if (this.CompFuBenDataDict == null || !this.CompFuBenDataDict.TryGetValue(key, out kuaFuData))
				{
					return -11003;
				}
				if (roleId < 0 && 6 == state)
				{
					kuaFuData.V.State = 3;
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					return state;
				}
				KuaFuData<KFCompRoleData> kuaFuData2 = null;
				if (!this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData2))
				{
					return -11003;
				}
				CompFuBenRoleData compFuBenRoleData = null;
				if (!kuaFuData.V.RoleDict.TryGetValue(roleId, out compFuBenRoleData))
				{
					compFuBenRoleData = new CompFuBenRoleData
					{
						ServerId = serverId,
						RoleId = roleId,
						KuaFuServerId = kuaFuData.V.ServerId,
						KuaFuMapCode = kuaFuData.V.GameId,
						Side = kuaFuData2.V.CompType,
						State = 0,
						ZhiWu = zhiwu
					};
					kuaFuData.V.RoleDict[roleId] = compFuBenRoleData;
				}
				if (4 == state)
				{
					if (kuaFuData.V.GetRoleCountWithEnter(compFuBenRoleData.Side) >= compBattleConfig.MaxEnterNum)
					{
						return -22;
					}
					if (compFuBenRoleData.State == null || compFuBenRoleData.State == 7)
					{
						List<int> list;
						int index;
						(list = kuaFuData.V.EnterGameRoleCount)[index = compFuBenRoleData.Side - 1] = list[index] + 1;
						compFuBenRoleData.StateEndTime = Global.NowTime.AddMinutes(1.0);
					}
					kuaFuData.V.State = 2;
				}
				else if (5 == state)
				{
					List<int> list;
					int index;
					if (compFuBenRoleData.State == 4)
					{
						(list = kuaFuData.V.EnterGameRoleCount)[index = compFuBenRoleData.Side - 1] = list[index] - 1;
					}
					(list = kuaFuData.V.RoleCountSideList)[index = compFuBenRoleData.Side - 1] = list[index] + 1;
					if (zhiwu > 0)
					{
						kuaFuData.V.ZhuJiangRoleDict[compFuBenRoleData.Side].Add(roleId);
					}
				}
				else if (7 == state)
				{
					List<int> list;
					int index;
					(list = kuaFuData.V.RoleCountSideList)[index = compFuBenRoleData.Side - 1] = list[index] - 1;
					if (zhiwu > 0)
					{
						kuaFuData.V.ZhuJiangRoleDict[compFuBenRoleData.Side].Remove(roleId);
					}
				}
				compFuBenRoleData.State = state;
				TimeUtil.AgeByNow(ref kuaFuData.Age);
			}
			return state;
		}

		public KuaFuCmdData GetKuaFuFuBenData(int gameType, int cityID, long dataAge)
		{
			KuaFuCmdData result;
			if (30 != gameType && 31 != gameType)
			{
				result = null;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						DateTime now = TimeUtil.NowDateTime();
						int num = 0;
						int num2 = 0;
						int num3 = 0;
						if (30 == gameType)
						{
							CompBattleGameStates compBattleGameStates = this.GetCompBattleGameStates(now);
							if (0 == compBattleGameStates)
							{
								return null;
							}
							CompBattleConfig compBattleConfig = null;
							if (!this.CompBattleConfigDict.TryGetValue(cityID, out compBattleConfig))
							{
								return null;
							}
							num = compBattleConfig.TotalSecs;
							num2 = compBattleConfig.MaxEnterNum;
							num3 = compBattleConfig.MapCode;
						}
						else if (31 == gameType)
						{
							CompBattleGameStates compBattleGameStates = this.GetCompMineGameStates(now);
							if (0 == compBattleGameStates)
							{
								return null;
							}
							CompMineConfig compMineConfig = null;
							if (!this.CompMineConfigDict.TryGetValue(cityID, out compMineConfig))
							{
								return null;
							}
							num = compMineConfig.TotalSecs;
							num2 = compMineConfig.MaxEnterNum;
							num3 = compMineConfig.MapCode;
						}
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(gameType, cityID);
						KuaFuData<CompFuBenData> kuaFuData = null;
						if (!this.CompFuBenDataDict.TryGetValue(key, out kuaFuData))
						{
							kuaFuData = new KuaFuData<CompFuBenData>();
							kuaFuData.V.Init();
							kuaFuData.V.GameId = cityID;
							kuaFuData.V.State = 0;
							kuaFuData.V.EndTime = Global.NowTime.AddMinutes((double)num);
							if (!ClientAgentManager.Instance().AssginKfFuben(gameType, (long)cityID, num2 * 3, out kuaFuData.V.ServerId))
							{
								LogManager.WriteLog(2, string.Format("势力战分配游戏服务器失败 gameType={0}, mapCode={1}", gameType, num3), null, true);
								return null;
							}
							this.CompFuBenDataDict[key] = kuaFuData;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
						}
						if (dataAge != kuaFuData.Age)
						{
							return new KuaFuCmdData
							{
								Age = kuaFuData.Age,
								Bytes0 = DataHelper2.ObjectToBytes<CompFuBenData>(kuaFuData.V)
							};
						}
						return new KuaFuCmdData
						{
							Age = kuaFuData.Age
						};
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = null;
			}
			return result;
		}

		private static CompService _instance = new CompService();

		public readonly GameTypes EvItemGameType = 2;

		private object Mutex = new object();

		private int LastUpdateDayID;

		private int LastUpdateHour;

		private int CompDataDayID;

		private int CompDataWeekDayID;

		private int CompBattleWeekDayID;

		private int CompMineWeekDayID;

		private DateTime CheckTime10;

		public CompPersistence Persistence = CompPersistence.Instance;

		private Dictionary<int, CompMapData> CompMapDataDict = new Dictionary<int, CompMapData>();

		private Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> CompFuBenDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>>();

		private HashSet<int> CompBattleJiFenRoleSet = new HashSet<int>();

		private HashSet<int> CompMineJiFenRoleSet = new HashSet<int>();

		private int[] CompBattleJoinRoleNum = new int[3];

		private int[] CompMineJoinRoleNum = new int[3];

		public byte[] BytesCompRankBattleJiFenDict = null;

		public byte[] BytesCompRankMineJiFenDict = null;

		public Dictionary<int, CompConfig> CompConfigDict = new Dictionary<int, CompConfig>();

		public Dictionary<int, CompBattleConfig> CompBattleConfigDict = new Dictionary<int, CompBattleConfig>();

		public Dictionary<int, CompMineConfig> CompMineConfigDict = new Dictionary<int, CompMineConfig>();

		private double CompReplaceAmerce;

		private int CompBoomValueReduce;

		private int CompBoomValueMin;
	}
}
