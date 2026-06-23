using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class RebornService
	{
		public static RebornService Instance()
		{
			return RebornService._instance;
		}

		private Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict
		{
			get
			{
				return this.Persistence.RebornRoleDataDict;
			}
			set
			{
				this.Persistence.RebornRoleDataDict = value;
			}
		}

		public KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict
		{
			get
			{
				return this.Persistence.RebornRankDict;
			}
			set
			{
				this.Persistence.RebornRankDict = value;
			}
		}

		public void InitConfig()
		{
			try
			{
				lock (this.Mutex)
				{
					string fileName = "Config/RebornBoss.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RebornBossConfigDict.Clear();
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						int key = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapID", 0L);
						List<RebornBossConfig> list;
						if (!this.RebornBossConfigDict.TryGetValue(key, out list))
						{
							list = new List<RebornBossConfig>();
							this.RebornBossConfigDict[key] = list;
						}
						RebornBossConfig rebornBossConfig = new RebornBossConfig();
						rebornBossConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						rebornBossConfig.MapID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapID", 0L);
						rebornBossConfig.MonstersID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MonstersID", 0L);
						list.Add(rebornBossConfig);
					}
					foreach (List<RebornBossConfig> list2 in this.RebornBossConfigDict.Values)
					{
						list2.Sort(delegate(RebornBossConfig left, RebornBossConfig right)
						{
							int result;
							if (left.ID < right.ID)
							{
								result = -1;
							}
							else if (left.ID > right.ID)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
							return result;
						});
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
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
				this.LastUpdateHour = now.Hour;
				this.RebornDataDayID = this.Persistence.GetRebornDayID();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "RebornService.LoadDatabase failed!", ex, true);
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
				LogManager.WriteLog(2, "RebornService.Update failed!", ex, true);
			}
		}

		private void HandleChangeHour(DateTime now)
		{
			lock (this.Mutex)
			{
				this.Persistence.LoadRebornRankInfo(0, this.RebornRankDict);
			}
		}

		private void HandleChangeDay(DateTime now)
		{
			int offsetDay = TimeUtil.GetOffsetDay(now);
			if (0 == this.RebornDataDayID)
			{
				this.RebornDataDayID = offsetDay;
				this.Persistence.SaveRebornDayID(this.RebornDataDayID);
			}
			else
			{
				lock (this.Mutex)
				{
					if (offsetDay != this.RebornDataDayID)
					{
						foreach (KuaFuData<KFRebornRoleData> kuaFuData in this.RebornRoleDataDict.Values)
						{
							kuaFuData.V.RarityLast = kuaFuData.V.Rarity;
							kuaFuData.V.Rarity = 0;
							kuaFuData.V.BossLast = kuaFuData.V.Boss;
							kuaFuData.V.Boss = 0;
							kuaFuData.V.LianShaLast = kuaFuData.V.LianSha;
							kuaFuData.V.LianSha = 0;
							this.Persistence.UpdateRebornRoleData(kuaFuData.V, 84, false);
						}
						for (int i = 0; i <= 3; i++)
						{
							if (i != 0)
							{
								this.Persistence.LoadRebornRankInfo(i, this.RebornRankDict);
							}
						}
						this.RebornDataDayID = offsetDay;
						this.Persistence.SaveRebornDayID(this.RebornDataDayID);
					}
				}
			}
		}

		public void SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			lock (this.Mutex)
			{
				KuaFuData<KFRebornRoleData> kuaFuData = null;
				if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptId, roleId), out kuaFuData))
				{
					kuaFuData.V.RoleData4Selector = bytes;
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					this.Persistence.UpdateRebornRoleData4Selector(kuaFuData.V);
				}
			}
		}

		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			try
			{
				RebornSyncData rebornSyncData = new RebornSyncData();
				lock (this.Mutex)
				{
					rebornSyncData.RebornRankDict.Age = this.RebornRankDict.Age;
					rebornSyncData.BossRefreshDict.Age = this.BossRefreshDict.Age;
					if (ageRank != this.RebornRankDict.Age)
					{
						rebornSyncData.BytesRebornRankDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>>(this.RebornRankDict);
					}
					if (ageBoss != this.BossRefreshDict.Age)
					{
						rebornSyncData.BytesRebornBossRefreshDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>>(this.BossRefreshDict);
					}
				}
				return rebornSyncData;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public KuaFuCmdData GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFRebornRoleData> kuaFuData = null;
					if (!this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptId, roleId), out kuaFuData))
					{
						return null;
					}
					if (dataAge != kuaFuData.Age)
					{
						return new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFRebornRoleData>(kuaFuData.V)
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

		public int RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			int result = 0;
			try
			{
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
				KuaFuData<KFRebornRoleData> kuaFuData = null;
				if (!this.RebornRoleDataDict.TryGetValue(key, out kuaFuData))
				{
					kuaFuData = new KuaFuData<KFRebornRoleData>();
					kuaFuData.V.PtID = ptId;
					kuaFuData.V.RoleID = roleId;
					kuaFuData.V.RoleName = roleName;
					kuaFuData.V.Lev = level;
					this.RebornRoleDataDict[key] = kuaFuData;
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					this.Persistence.InsertRebornRoleData(kuaFuData.V);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public void ChangeName(int ptId, int roleId, string roleName)
		{
			try
			{
				lock (this.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
					KuaFuData<KFRebornRoleData> kuaFuData = null;
					if (this.RebornRoleDataDict.TryGetValue(key, out kuaFuData))
					{
						kuaFuData.V.RoleName = roleName;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						this.Persistence.UpdateRebornRoleDataRoleName(kuaFuData.V);
						bool flag2 = false;
						foreach (KeyValuePair<int, List<KFRebornRankInfo>> keyValuePair in this.RebornRankDict.V)
						{
							KFRebornRankInfo kfrebornRankInfo = keyValuePair.Value.Find((KFRebornRankInfo x) => x.PtID == ptId && x.Key == roleId);
							if (null != kfrebornRankInfo)
							{
								string worldRoleID = ConstData.FormatWorldRoleID(roleId, ptId);
								KuaFuWorldRoleData kuaFuWorldRoleData = TSingleton<KuaFuWorldManager>.getInstance().LoadKuaFuWorldRoleData(roleId, ptId, worldRoleID);
								if (null != kuaFuWorldRoleData)
								{
									kfrebornRankInfo.Param1 = KuaFuServerManager.FormatName(kfrebornRankInfo.tagInfo.V.RoleName, kuaFuWorldRoleData.ZoneID);
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							TimeUtil.AgeByNow(ref this.RebornRankDict.Age);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			try
			{
				lock (this.Mutex)
				{
					switch (optType)
					{
					case 0:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							kuaFuData.V.Lev = param1;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.UpdateRebornRoleData(kuaFuData.V, 1, true);
						}
						break;
					}
					case 1:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							kuaFuData.V.Rarity += param1;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.UpdateRebornRoleData(kuaFuData.V, 2, true);
						}
						break;
					}
					case 2:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							kuaFuData.V.Boss += param1;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.UpdateRebornRoleData(kuaFuData.V, 8, true);
						}
						break;
					}
					case 3:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							if (param1 > kuaFuData.V.LianSha)
							{
								kuaFuData.V.LianSha = param1;
								TimeUtil.AgeByNow(ref kuaFuData.Age);
								this.Persistence.UpdateRebornRoleData(kuaFuData.V, 32, true);
							}
						}
						break;
					}
					case 4:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							KFRebornBossAwardData myData = new KFRebornBossAwardData();
							myData.MapCodeID = param1;
							myData.LineID = param2;
							string[] array = param3.Split(new char[]
							{
								','
							});
							if (array.Length == 2)
							{
								myData.ExtensionID = Global.SafeConvertToInt32(array[0]);
								myData.RankNum = Global.SafeConvertToInt32(array[1]);
							}
							KFRebornBossAwardData kfrebornBossAwardData = kuaFuData.V.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == myData.MapCodeID && x.LineID == myData.LineID);
							if (null != kfrebornBossAwardData)
							{
								kfrebornBossAwardData.MapCodeID = myData.MapCodeID;
								kfrebornBossAwardData.LineID = myData.LineID;
								kfrebornBossAwardData.ExtensionID = myData.ExtensionID;
								kfrebornBossAwardData.RankNum = myData.RankNum;
							}
							else
							{
								kuaFuData.V.BossAwardList.Add(myData);
							}
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.UpdateRebornRoleDataBossAward(kuaFuData.V);
						}
						break;
					}
					case 5:
					{
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kuaFuData))
						{
							kuaFuData.V.BossAwardList.RemoveAll((KFRebornBossAwardData x) => x.MapCodeID == param1 && x.LineID == param2 && x.ExtensionID == Global.SafeConvertToInt32(param3));
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.UpdateRebornRoleDataBossAward(kuaFuData.V);
						}
						break;
					}
					case 6:
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(param1, param2);
						KFRebornBossRefreshData kfrebornBossRefreshData;
						if (!this.BossRefreshDict.V.TryGetValue(key, out kfrebornBossRefreshData))
						{
							kfrebornBossRefreshData = new KFRebornBossRefreshData();
							this.BossRefreshDict.V[key] = kfrebornBossRefreshData;
						}
						kfrebornBossRefreshData.MapCodeID = param1;
						kfrebornBossRefreshData.LineID = param2;
						string[] array = param3.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							kfrebornBossRefreshData.ExtensionID = Global.SafeConvertToInt32(array[0]);
							kfrebornBossRefreshData.NextTime = array[1];
						}
						TimeUtil.AgeByNow(ref this.BossRefreshDict.Age);
						break;
					}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void PlatFormChat(int serverId, byte[] bytes)
		{
			try
			{
				AsyncDataItem evItem = new AsyncDataItem(39, new object[]
				{
					bytes
				});
				HashSet<int> hashSet = new HashSet<int>();
				lock (this.Mutex)
				{
					foreach (KeyValuePair<int, List<RebornBossConfig>> keyValuePair in this.RebornBossConfigDict)
					{
						List<KuaFuLineData> kuaFuLineDataList = KuaFuServerManager.GetKuaFuLineDataList(keyValuePair.Key);
						if (null != kuaFuLineDataList)
						{
							foreach (KuaFuLineData kuaFuLineData in kuaFuLineDataList)
							{
								hashSet.Add(kuaFuLineData.ServerId);
							}
						}
					}
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

		private static RebornService _instance = new RebornService();

		public readonly GameTypes EvItemGameType = 32;

		private object Mutex = new object();

		private int LastUpdateDayID;

		private int LastUpdateHour;

		private int RebornDataDayID;

		public RebornPersistence Persistence = RebornPersistence.Instance;

		public KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>> BossRefreshDict = new KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>();

		public Dictionary<int, List<RebornBossConfig>> RebornBossConfigDict = new Dictionary<int, List<RebornBossConfig>>();
	}
}
