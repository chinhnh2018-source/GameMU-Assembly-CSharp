using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, UseSynchronizationContext = true)]
	public class JunTuanService : MarshalByRefObject, IJunTuanService, IExecCommand
	{
		private object Mutex
		{
			get
			{
				return this.Persistence.Mutex;
			}
		}

		private KuaFuCmdData JunTuanBaseDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanBaseDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanBaseDataListCmdData = value;
			}
		}

		private KuaFuCmdData JunTuanMiniDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanMiniDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanMiniDataListCmdData = value;
			}
		}

		private KuaFuCmdData JunTuanRankDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanRankDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanRankDataListCmdData = value;
			}
		}

		private Dictionary<int, JunTuanDetailData> JunTuanAllDataDict
		{
			get
			{
				return this.Persistence.JunTuanAllDataDict;
			}
			set
			{
				this.Persistence.JunTuanAllDataDict = value;
			}
		}

		private Dictionary<int, int> BangHuiJunTuanIdDict
		{
			get
			{
				return this.Persistence.BangHuiJunTuanIdDict;
			}
			set
			{
				this.Persistence.BangHuiJunTuanIdDict = value;
			}
		}

		private Dictionary<int, JunTuanBangHuiData> JunTuanBangHuiDataDict
		{
			get
			{
				return this.Persistence.JunTuanBangHuiDataDict;
			}
			set
			{
				this.Persistence.JunTuanBangHuiDataDict = value;
			}
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public JunTuanService()
		{
			JunTuanService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		~JunTuanService()
		{
			this.BackgroundThread.Abort();
		}

		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					Global.UpdateNowTime(dateTime);
					if (dateTime.TimeOfDay.TotalMinutes < 5.0)
					{
						this.Persistence.CheckJunTuanPoint();
					}
					if (dateTime > this.CheckTime20)
					{
						this.CheckTime20 = dateTime.AddSeconds(20.0);
						this.Persistence.UpdateJunTuanTaskList();
						this.CheckRoleTimerProc(dateTime);
						JunTuanEraService.Instance().HandleChangeEraID(dateTime, true);
					}
					if (dateTime > this.CheckTimer120)
					{
						this.CheckTimer120 = dateTime.AddSeconds(120.0);
						this.Persistence.CheckJunTuanBangHuiList();
						int num = 12;
						lock (this.Mutex)
						{
							foreach (KeyValuePair<int, int> keyValuePair in this.BangHuiJunTuanIdDict)
							{
								if (keyValuePair.Value > 0)
								{
									int num2;
									if (!this.BangHuiId2RoleDataUpdateDayIdDict.TryGetValue(keyValuePair.Key, out num2) || num2 != dateTime.Day)
									{
										this.BangHuiId2RoleDataUpdateDayIdDict[keyValuePair.Key] = dateTime.Day;
										ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(25, new object[]
										{
											keyValuePair.Key
										}), 0);
										if (num-- <= 0)
										{
											break;
										}
									}
								}
							}
							this.CheckGameFuBenTimerProc(dateTime);
						}
					}
					if (this.ExecPaiHang)
					{
						this.ExecPaiHang = false;
						this.Persistence.UpdateJunTuanRankDataList();
					}
					if (dateTime > this.CheckTimer3600)
					{
						this.CheckTimer3600 = dateTime.AddSeconds(3600.0);
						this.Persistence.UpdateJunTuanRankDataList();
						this.Persistence.UpdateJunTuanBaseDataList();
						YaoSaiService.Instance().CheckYaoSaiPrisonTimerProc(dateTime);
					}
					this.Persistence.DelayWriteDataProc();
					int num3 = (int)(TimeUtil.NowDateTime() - dateTime).TotalMilliseconds;
					this.Persistence.SaveCostTime(num3);
					num3 = 1000 - num3;
					if (num3 < 50)
					{
						num3 = 50;
					}
					Thread.Sleep(num3);
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		public AsyncDataItem[] GetClientCacheItems(int serverId, long dataAge)
		{
			AsyncDataItem[] array = ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
			AsyncDataItem hongBaoDataList = HongBaoManager_K.getInstance().GetHongBaoDataList(dataAge);
			AsyncDataItem[] result;
			if (hongBaoDataList != null)
			{
				if (null != array)
				{
					AsyncDataItem[] array2 = new AsyncDataItem[array.Length + 1];
					array.CopyTo(array2, 1);
					array2[0] = hongBaoDataList;
					result = array2;
				}
				else
				{
					result = new AsyncDataItem[]
					{
						hongBaoDataList
					};
				}
			}
			else
			{
				result = array;
			}
			return result;
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 21 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1}", clientInfo.ServerId, clientInfo.ClientId));
				result = -11003;
			}
			return result;
		}

		public int CreateJunTuan(byte[] cmdBytes)
		{
			try
			{
				JunTuanRequestData junTuanRequestData = DataHelper2.BytesToObject<JunTuanRequestData>(cmdBytes, 0, cmdBytes.Length);
				string junTuanName = junTuanRequestData.JunTuanName;
				lock (this.Mutex)
				{
					int num;
					if (this.BangHuiJunTuanIdDict.TryGetValue(junTuanRequestData.BhId, out num) && num > 0)
					{
						return -1020;
					}
					JunTuanBangHuiData junTuanBangHuiData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(junTuanRequestData.BhId, out junTuanBangHuiData))
					{
						junTuanBangHuiData = new JunTuanBangHuiData
						{
							BhId = junTuanRequestData.BhId
						};
						this.JunTuanBangHuiDataDict[junTuanBangHuiData.BhId] = junTuanBangHuiData;
					}
					junTuanBangHuiData.BhId = junTuanRequestData.BhId;
					junTuanBangHuiData.BhName = junTuanRequestData.BhName;
					junTuanBangHuiData.BhZoneId = junTuanRequestData.BhZoneId;
					junTuanBangHuiData.LeaderName = junTuanRequestData.LeaderName;
					junTuanBangHuiData.LeaderZoneId = junTuanRequestData.LeaderZoneId;
					junTuanBangHuiData.RoleNum = junTuanRequestData.RoleNum;
					junTuanBangHuiData.ZhanLi = junTuanRequestData.ZhanLi;
					junTuanBangHuiData.LeaderOccupation = junTuanRequestData.Occupation;
					junTuanBangHuiData.LeaderRoleId = junTuanRequestData.LeaderRoleId;
					long num2 = TimeUtil.NOW();
					if (num2 - junTuanBangHuiData.LastCreateTicks < (long)(this.Persistence.RuntimeData.LegionsCreateCD * 3600000))
					{
						return -2007;
					}
					int num3 = this.Persistence.RuntimeData.LegionProsperityCost[0];
					num = (int)this.Persistence.CreateJunTuan(junTuanName, "", junTuanRequestData.BhZoneId, junTuanRequestData.LeaderName, num3, TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime()));
					if (num <= 0)
					{
						return -1023;
					}
					junTuanBangHuiData.JuTuanZhiWu = 1;
					if (this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, num) < 0)
					{
						return -15;
					}
					junTuanBangHuiData.LastCreateTicks = num2;
					JunTuanDetailData detailData = this.JuntuanAdd(new JunTuanData
					{
						JunTuanId = num,
						JunTuanName = junTuanName,
						Point = num3,
						LeaderName = junTuanRequestData.LeaderName,
						LeaderZoneId = junTuanRequestData.LeaderZoneId,
						BangHuiNum = 1
					}, junTuanBangHuiData);
					string message = "";
					JunTuanEventLog logData = new JunTuanEventLog
					{
						EventType = 0,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(detailData, logData);
					JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData
					{
						BhId = junTuanRequestData.BhId,
						JunTuanId = num,
						JunTuanName = junTuanName,
						JunTuanZhiWu = 1,
						JunTuanChanged = 1
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
					{
						junTuanBangHuiMiniData
					}), 0);
					return num;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		public int ChangeJunTuanBulltin(int bhid, int junTuanId, string bulltin)
		{
			lock (this.Mutex)
			{
				int num;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
				{
					return -1020;
				}
				JunTuanDetailData junTuanDetailData;
				bool flag2;
				if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
				{
					flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return -1024;
				}
				junTuanDetailData.JunTuanData.V.Bulletin = bulltin;
				TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanData.Age);
				DbHelperMySQL.ExecuteSql(string.Format("update t_juntuan set bulletin='{1}' where juntuanid={0}", junTuanId, bulltin));
			}
			return 0;
		}

		public int ChangeJunTuanGVoicePrioritys(int bhid, string prioritys)
		{
			lock (this.Mutex)
			{
				int key;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					return -1025;
				}
				JunTuanDetailData junTuanDetailData;
				bool flag2;
				if (this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
				{
					flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return -1024;
				}
				string b = junTuanDetailData.JunTuanData.V.LeaderRoleId.ToString();
				HashSet<string> hashSet = new HashSet<string>();
				if (!string.IsNullOrEmpty(prioritys))
				{
					foreach (string text in prioritys.Split(new char[]
					{
						','
					}))
					{
						if (text != b)
						{
							hashSet.Add(text);
						}
					}
				}
				prioritys = string.Join(",", hashSet);
				junTuanDetailData.JunTuanData.V.GVoicePrioritys = prioritys;
				this.Persistence.AddDelayWriteSql(string.Format("update t_juntuan set voice='{0}';", prioritys));
				GMCmdData data = new GMCmdData
				{
					Fields = new string[]
					{
						"-gvoicepriority",
						2.ToString(),
						key.ToString(),
						prioritys
					}
				};
				this.BroadcastGMCmdData(data, 2);
			}
			return 0;
		}

		public string GetJunTuanGVoicePrioritys(int bhid)
		{
			string result;
			lock (this.Mutex)
			{
				int key;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					result = null;
				}
				else
				{
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = null;
					}
					else
					{
						result = junTuanDetailData.JunTuanData.V.GVoicePrioritys;
					}
				}
			}
			return result;
		}

		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			lock (this.Mutex)
			{
				if (serverFlag == 1)
				{
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(21, new AsyncDataItem(9996, new object[]
					{
						data
					}));
				}
				else
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(21, new AsyncDataItem(9996, new object[]
					{
						data
					}), 0);
				}
			}
		}

		public int UpdateJunTuanLingDi(int junTuanId, int lingdi)
		{
			lock (this.Mutex)
			{
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					if (junTuanDetailData.JunTuanId == junTuanId)
					{
						junTuanDetailData.JunTuanData.V.LingDi |= lingdi;
						TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanData.Age);
						this.Persistence.AddJuntuanEventLog(junTuanDetailData, new JunTuanEventLog
						{
							EventType = 5,
							Time = TimeUtil.NowDateTime(),
							Message = lingdi.ToString()
						});
					}
					else if ((junTuanDetailData.JunTuanData.V.LingDi & lingdi) != 0)
					{
						junTuanDetailData.JunTuanData.V.LingDi &= ~lingdi;
						TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanData.Age);
					}
				}
				this.Persistence.UpdateJunTuanLingDi(junTuanId, lingdi);
				this.Persistence.UpdateJunTuanMiniDataList();
			}
			return 0;
		}

		public int QuitJunTuan(int bhid, int junTuanId, int otherBhid)
		{
			try
			{
				lock (this.Mutex)
				{
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
					{
						return -1020;
					}
					JunTuanDetailData junTuanDetailData;
					if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						return -1024;
					}
					int num2;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out num2) && num2 != junTuanId)
					{
						return -1025;
					}
					int num3 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0) ? 1 : 2;
					if (otherBhid != bhid && this.Persistence.RuntimeData.RolePermissionDict.Value[1].Manager == 0)
					{
						return -1024;
					}
					JunTuanBangHuiData junTuanBangHuiData = junTuanDetailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					junTuanBangHuiData.JuTuanZhiWu = 0;
					this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, 0);
					string message = KuaFuServerManager.FormatName(junTuanBangHuiData.BhZoneId, junTuanBangHuiData.BhName);
					JunTuanEventLog logData = new JunTuanEventLog
					{
						EventType = 3,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(junTuanDetailData, logData);
					int num4 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == otherBhid) == 0) ? 1 : 0;
					if (num4 == 1)
					{
						this.JunTuanRemoveBangHui(junTuanDetailData, otherBhid);
						JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData
						{
							BhId = junTuanDetailData.LeaderBhId,
							JunTuanId = junTuanId,
							JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
							JunTuanZhiWu = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
						{
							junTuanBangHuiMiniData
						}), 0);
					}
					else
					{
						this.JunTuanRemoveBangHui(junTuanDetailData, otherBhid);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		private void JunTuanAddBangHui(JunTuanDetailData detailData, JunTuanBangHuiData data)
		{
			this.BangHuiJunTuanIdDict[data.BhId] = detailData.JunTuanId;
			int num = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == data.BhId);
			if (num >= 0)
			{
				detailData.JunTuanBangHuiList.V[num] = data;
			}
			else
			{
				detailData.JunTuanBangHuiList.V.Add(data);
			}
			TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiList.Age);
			this.Persistence.JunTuanUpdateBhList(detailData, true, false);
		}

		public int DestroyJunTuan(int bhid, int junTuanId)
		{
			try
			{
				lock (this.Mutex)
				{
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
					{
						return -1020;
					}
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					if (junTuanDetailData.JunTuanData.V.LingDi > 0)
					{
						return -1032;
					}
					this.Persistence.DestroyJunTuan(junTuanDetailData);
					this.Persistence.AddJunTuanEventLog(junTuanDetailData.JunTuanId, new JunTuanEventLog
					{
						EventType = 6,
						Message = "Destroy",
						Time = TimeUtil.NowDateTime()
					});
					JunTuanEraService.Instance().OnJunTuanDestroy(junTuanDetailData.JunTuanId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		public void JunTuanChat(int serverId, byte[] bytes)
		{
			try
			{
				ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(26, new object[]
				{
					bytes
				}), serverId);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public int JoinJunTuan(byte[] cmdBytes)
		{
			try
			{
				JunTuanRequestData data = DataHelper2.BytesToObject<JunTuanRequestData>(cmdBytes, 0, cmdBytes.Length);
				lock (this.Mutex)
				{
					int num;
					if (this.BangHuiJunTuanIdDict.TryGetValue(data.BhId, out num) && num > 0)
					{
						return -1020;
					}
					JunTuanBangHuiData junTuanBangHuiData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(data.BhId, out junTuanBangHuiData))
					{
						junTuanBangHuiData = new JunTuanBangHuiData();
						junTuanBangHuiData.BhId = data.BhId;
						this.JunTuanBangHuiDataDict[junTuanBangHuiData.BhId] = junTuanBangHuiData;
					}
					junTuanBangHuiData.BhName = data.BhName;
					junTuanBangHuiData.BhZoneId = data.BhZoneId;
					junTuanBangHuiData.LeaderName = data.LeaderName;
					junTuanBangHuiData.LeaderZoneId = data.LeaderZoneId;
					junTuanBangHuiData.RoleNum = data.RoleNum;
					junTuanBangHuiData.ZhanLi = data.ZhanLi;
					junTuanBangHuiData.LeaderOccupation = data.Occupation;
					junTuanBangHuiData.LeaderRoleId = data.LeaderRoleId;
					long num2 = TimeUtil.NOW();
					if (num2 - junTuanBangHuiData.LastRequestJoinTicks < (long)(this.Persistence.RuntimeData.LegionsJionCD * 60000))
					{
						return -2007;
					}
					JunTuanDetailData junTuanDetailData;
					if (!this.JunTuanAllDataDict.TryGetValue(data.JunTuanId, out junTuanDetailData))
					{
						return -1022;
					}
					if (junTuanDetailData.JunTuanData.V.BangHuiNum >= 4)
					{
						return -1031;
					}
					if (junTuanDetailData.JoinDataList.V.Exists((JunTuanRequestData x) => x.BhId == data.BhId))
					{
						return -1021;
					}
					if (this.Persistence.AddJunTuanJoinData(data) < 0L)
					{
						return -15;
					}
					junTuanBangHuiData.LastRequestJoinTicks = num2;
					if (junTuanDetailData.JoinDataList == null)
					{
						junTuanDetailData.JoinDataList = new KuaFuData<List<JunTuanRequestData>>();
						junTuanDetailData.JoinDataList.Age += 1L;
						junTuanDetailData.JoinDataList.V = new List<JunTuanRequestData>();
					}
					junTuanDetailData.JoinDataList.V.Add(data);
					this.Persistence.UpdateRequestDataListCmdData(junTuanDetailData);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(27, new object[]
					{
						data.JunTuanId,
						1
					}), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		public KuaFuCmdData GetJunTuanList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (this.JunTuanMiniDataListCmdData.Age != dataAge)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanMiniDataListCmdData.Age,
						Bytes0 = this.JunTuanMiniDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanMiniDataListCmdData.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetJunTuanData(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int key;
				JunTuanDetailData junTuanDetailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					result = null;
				}
				else if (!this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
				{
					result = null;
				}
				else if (dataAge != junTuanDetailData.JunTuanData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = junTuanDetailData.JunTuanData.Age,
						Bytes0 = DataHelper2.ObjectToBytes<JunTuanData>(junTuanDetailData.JunTuanData.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = junTuanDetailData.JunTuanData.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetJunTuanBaseData(int bhid, long dataAge)
		{
			throw new NotImplementedException();
		}

		public KuaFuCmdData GetJunTuanBaseDataList(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (this.JunTuanBaseDataListCmdData.Age != dataAge)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanBaseDataListCmdData.Age,
						Bytes0 = this.JunTuanBaseDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanBaseDataListCmdData.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetJunTuanRequestList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int key;
				JunTuanDetailData junTuanDetailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
				{
					if (dataAge == junTuanDetailData.RequestDataListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.RequestDataListCmdData.Age
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.RequestDataListCmdData.Age,
							Bytes0 = junTuanDetailData.RequestDataListCmdData.Bytes0
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public int JoinJunTuanResponse(int bhid, int junTuanId, int otherBhid, bool accept)
		{
			try
			{
				lock (this.Mutex)
				{
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
					{
						return -1026;
					}
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					JunTuanRequestData junTuanRequestData = junTuanDetailData.JoinDataList.V.Find((JunTuanRequestData x) => x.BhId == otherBhid);
					if (junTuanRequestData == null)
					{
						return -1001;
					}
					if (accept)
					{
						if (junTuanDetailData.JunTuanData.V.BangHuiNum >= 4)
						{
							return -1031;
						}
					}
					foreach (JunTuanDetailData junTuanDetailData2 in this.JunTuanAllDataDict.Values)
					{
						JunTuanRequestData junTuanRequestData2 = junTuanDetailData2.JoinDataList.V.FirstOrDefault((JunTuanRequestData x) => x.BhId == otherBhid);
						if (null != junTuanRequestData2)
						{
							this.Persistence.DeleteJunTuanRequestData(junTuanDetailData2, junTuanRequestData2);
							this.Persistence.UpdateRequestDataListCmdData(junTuanDetailData2);
						}
					}
					if (accept)
					{
						int num2;
						if (this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out num2) && num2 != 0)
						{
							return -1020;
						}
						JunTuanBangHuiData data;
						if (!this.JunTuanBangHuiDataDict.TryGetValue(otherBhid, out data))
						{
							return -1001;
						}
						this.JunTuanAddBangHui(junTuanDetailData, data);
						this.Persistence.UpdateJunTuanBangHuiData(data, junTuanId);
						this.Persistence.ReloadJunTuanRoleDataList(junTuanDetailData);
						JunTuanBangHuiData junTuanBangHuiData = junTuanDetailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
						string message = KuaFuServerManager.FormatName(junTuanBangHuiData.BhZoneId, junTuanBangHuiData.BhName);
						JunTuanEventLog logData = new JunTuanEventLog
						{
							EventType = 1,
							Message = message,
							Time = TimeUtil.NowDateTime()
						};
						this.Persistence.AddJuntuanEventLog(junTuanDetailData, logData);
						JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData
						{
							BhId = otherBhid,
							JunTuanId = junTuanId,
							JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
							JunTuanChanged = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
						{
							junTuanBangHuiMiniData
						}), 0);
					}
					if (junTuanDetailData.JoinDataList.V.Count == 0)
					{
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(27, new object[]
						{
							junTuanId,
							0
						}), 0);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		public int RemoveBangHui(int otherBhid)
		{
			try
			{
				lock (this.Mutex)
				{
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out num) || num <= 0)
					{
						Dictionary<JunTuanDetailData, JunTuanRequestData> dictionary = new Dictionary<JunTuanDetailData, JunTuanRequestData>();
						foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
						{
							JunTuanRequestData junTuanRequestData = junTuanDetailData.JoinDataList.V.FirstOrDefault((JunTuanRequestData x) => x.BhId == otherBhid);
							if (null != junTuanRequestData)
							{
								dictionary[junTuanDetailData] = junTuanRequestData;
							}
						}
						foreach (KeyValuePair<JunTuanDetailData, JunTuanRequestData> keyValuePair in dictionary)
						{
							this.Persistence.DeleteJunTuanRequestData(keyValuePair.Key, keyValuePair.Value);
						}
						return 1;
					}
					JunTuanDetailData junTuanDetailData2;
					if (!this.JunTuanAllDataDict.TryGetValue(num, out junTuanDetailData2))
					{
						return 1;
					}
					JunTuanBangHuiData junTuanBangHuiData = junTuanDetailData2.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					if (null == junTuanBangHuiData)
					{
						return 1;
					}
					junTuanBangHuiData.JuTuanZhiWu = 0;
					this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, 0);
					string message = KuaFuServerManager.FormatName(junTuanBangHuiData.BhZoneId, junTuanBangHuiData.BhName);
					JunTuanEventLog logData = new JunTuanEventLog
					{
						EventType = 3,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(junTuanDetailData2, logData);
					int num2 = (junTuanDetailData2.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == otherBhid) == 0) ? 1 : 0;
					if (num2 == 1)
					{
						this.JunTuanRemoveBangHui(junTuanDetailData2, otherBhid);
						JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData
						{
							BhId = junTuanDetailData2.LeaderBhId,
							JunTuanId = num,
							JunTuanName = junTuanDetailData2.JunTuanData.V.JunTuanName,
							JunTuanZhiWu = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
						{
							junTuanBangHuiMiniData
						}), 0);
					}
					else
					{
						this.JunTuanRemoveBangHui(junTuanDetailData2, otherBhid);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		public int ChangeBangHuiName(int bhid, string bhName)
		{
			try
			{
				lock (this.Mutex)
				{
					JunTuanBangHuiData junTuanBangHuiData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(bhid, out junTuanBangHuiData))
					{
						return 3;
					}
					junTuanBangHuiData.BhName = bhName;
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num <= 0)
					{
						this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, 0);
						return 1;
					}
					JunTuanDetailData junTuanDetailData;
					if (!this.JunTuanAllDataDict.TryGetValue(num, out junTuanDetailData))
					{
						return 1;
					}
					junTuanBangHuiData = junTuanDetailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == bhid);
					if (null == junTuanBangHuiData)
					{
						return 1;
					}
					this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, num);
					this.Persistence.JunTuanUpdateBhList(junTuanDetailData, true, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		public KuaFuCmdData GetJunTuanBangHuiList(int bhid, int junTuanId, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int num;
				JunTuanDetailData junTuanDetailData;
				if (bhid > 0 && (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || junTuanId != num))
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
				{
					if (dataAge != junTuanDetailData.JunTuanBangHuiListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanBangHuiListCmdData.Age,
							Bytes0 = junTuanDetailData.JunTuanBangHuiListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanBangHuiListCmdData.Age
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public int JunTuanChangeBangHuiZhiWu(int bhid, int junTuanId, int otherBhid, int zhiWu)
		{
			try
			{
				lock (this.Mutex)
				{
					int num;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
					{
						return -1026;
					}
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					int num2;
					if (this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out num2) && num2 != junTuanId)
					{
						return -1025;
					}
					JunTuanBangHuiData junTuanBangHuiData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(otherBhid, out junTuanBangHuiData))
					{
						return -4;
					}
					if (junTuanDetailData.JunTuanData.V.LingDi > 0)
					{
						return -1032;
					}
					this.JunTuanChangeBangHui(junTuanDetailData, bhid, otherBhid);
					JunTuanBangHuiData junTuanBangHuiData2 = junTuanDetailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					string message = KuaFuServerManager.FormatName(junTuanBangHuiData2.BhZoneId, junTuanBangHuiData2.BhName);
					JunTuanEventLog logData = new JunTuanEventLog
					{
						EventType = 2,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(junTuanDetailData, logData);
					JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData
					{
						BhId = otherBhid,
						JunTuanId = junTuanId,
						JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
						JunTuanZhiWu = zhiWu
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
					{
						junTuanBangHuiMiniData
					}), 0);
					JunTuanBangHuiMiniData junTuanBangHuiMiniData2 = new JunTuanBangHuiMiniData
					{
						BhId = bhid,
						JunTuanId = junTuanId,
						JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
						JunTuanZhiWu = 0
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(22, new object[]
					{
						junTuanBangHuiMiniData2
					}), 0);
					return 0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		public KuaFuCmdData GetJunTuanRoleList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int num;
				JunTuanDetailData junTuanDetailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num <= 0)
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(num, out junTuanDetailData))
				{
					if (dataAge != junTuanDetailData.JunTuanRoleDataListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanRoleDataListCmdData.Age,
							Bytes0 = junTuanDetailData.JunTuanRoleDataListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanRoleDataListCmdData.Age
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public int UpdateRoleDataList(int bhid, KuaFuCmdData listCmdData)
		{
			try
			{
				List<JunTuanRoleData> list = null;
				int num = 0;
				long num2 = 0L;
				Dictionary<int, JunTuanRoleData> dictionary = new Dictionary<int, JunTuanRoleData>();
				if (listCmdData != null && listCmdData.Bytes0 != null && listCmdData.Bytes0.Length > 0)
				{
					list = DataHelper2.BytesToObject<List<JunTuanRoleData>>(listCmdData.Bytes0, 0, listCmdData.Bytes0.Length);
					if (null != list)
					{
						foreach (JunTuanRoleData junTuanRoleData in list)
						{
							num++;
							num2 += (long)junTuanRoleData.ZhanLi;
							dictionary[junTuanRoleData.RoleId] = junTuanRoleData;
						}
					}
				}
				long num3 = TimeUtil.NOW();
				bool flag = false;
				lock (this.Mutex)
				{
					int num4;
					if (this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num4))
					{
						JunTuanDetailData junTuanDetailData;
						if (this.JunTuanAllDataDict.TryGetValue(num4, out junTuanDetailData))
						{
							JunTuanBangHuiData junTuanBangHuiData = junTuanDetailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == bhid);
							if (null != junTuanBangHuiData)
							{
								bool flag3 = false;
								if (junTuanBangHuiData.RoleNum != num || junTuanBangHuiData.ZhanLi != num2)
								{
									if (junTuanBangHuiData.NextUpdateTicks < num3)
									{
										flag3 = true;
										junTuanBangHuiData.NextUpdateTicks = num3 + 300000L;
									}
								}
								junTuanBangHuiData.RoleNum = num;
								junTuanBangHuiData.ZhanLi = num2;
								this.Persistence.UpdateJunTuanBangHuiData(junTuanBangHuiData, num4);
								foreach (JunTuanRoleData junTuanRoleData2 in dictionary.Values)
								{
									if (junTuanRoleData2.JuTuanZhiWu == 2 || junTuanRoleData2.JuTuanZhiWu == 1)
									{
										if (junTuanRoleData2.BhId == junTuanDetailData.LeaderBhId)
										{
											if (junTuanDetailData.JunTuanData.V.LeaderName != junTuanRoleData2.RoleName || junTuanDetailData.JunTuanData.V.LeaderZoneId != junTuanRoleData2.ZoneId)
											{
												flag = true;
											}
											junTuanRoleData2.JuTuanZhiWu = 1;
											junTuanDetailData.JunTuanData.V.LeaderZoneId = junTuanRoleData2.ZoneId;
											junTuanDetailData.JunTuanData.V.LeaderName = junTuanRoleData2.RoleName;
											junTuanDetailData.JunTuanData.V.LeaderRoleId = junTuanRoleData2.RoleId;
											TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanData.Age);
										}
										else
										{
											junTuanRoleData2.JuTuanZhiWu = 2;
										}
										if (junTuanBangHuiData.LeaderName != junTuanRoleData2.RoleName || junTuanBangHuiData.LeaderZoneId != junTuanRoleData2.ZoneId || junTuanBangHuiData.LeaderOccupation != junTuanRoleData2.Occu)
										{
											flag3 = true;
											junTuanBangHuiData.LeaderName = junTuanRoleData2.RoleName;
											junTuanBangHuiData.LeaderZoneId = junTuanRoleData2.ZoneId;
											junTuanBangHuiData.LeaderOccupation = junTuanRoleData2.Occu;
										}
									}
								}
								junTuanDetailData.JunTuanRoleDataList.Age = TimeUtil.AgeByNow(junTuanDetailData.JunTuanRoleDataList.Age);
								junTuanDetailData.JunTuanRoleDataList.V.RemoveAll((JunTuanRoleData x) => x.BhId == bhid);
								junTuanDetailData.JunTuanRoleDataList.V.AddRange(list);
								junTuanDetailData.JunTuanRoleDataListCmdData.Age = junTuanDetailData.JunTuanRoleDataList.Age;
								junTuanDetailData.JunTuanRoleDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRoleData>>(junTuanDetailData.JunTuanRoleDataList.V);
								if (flag3)
								{
									TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanBangHuiListCmdData.Age);
									junTuanDetailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(junTuanDetailData.JunTuanBangHuiList.V);
								}
							}
						}
					}
				}
				this.Persistence.UpdateJuntuanRoleDataList(bhid, dictionary);
				if (flag)
				{
					this.Persistence.UpdateJunTuanMiniDataList();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		public KuaFuCmdData GetJunTuanTaskAllData(int bhid, int junTuanId, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int num;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num == 0)
				{
					result = new KuaFuCmdData
					{
						Age = -1025L
					};
				}
				else
				{
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(num, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = new KuaFuCmdData
						{
							Age = -1025L
						};
					}
					else if (dataAge != junTuanDetailData.JunTuanTaskAllData.Age)
					{
						junTuanDetailData.JunTuanTaskAllDataCmdData.Age = junTuanDetailData.JunTuanTaskAllData.Age;
						junTuanDetailData.JunTuanTaskAllDataCmdData.Bytes0 = DataHelper2.ObjectToBytes<JunTuanTaskAllData>(junTuanDetailData.JunTuanTaskAllData.V);
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanTaskAllDataCmdData.Age,
							Bytes0 = junTuanDetailData.JunTuanTaskAllDataCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.JunTuanTaskAllData.Age
						};
					}
				}
			}
			return result;
		}

		public int JunTuanChangeTaskValue(int bhid, int junTuanId, int taskId, int addValue, long ticks)
		{
			DateTime time = new DateTime(ticks);
			int result;
			lock (this.Mutex)
			{
				int key;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					result = -1025;
				}
				else
				{
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = -1025;
					}
					else
					{
						JunTuanTaskData junTuanTaskData = junTuanDetailData.JunTuanTaskAllData.V.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
						JunTuanTaskInfo junTuanTaskInfo;
						if (null == junTuanTaskData)
						{
							result = -1029;
						}
						else if (junTuanTaskData.TaskState != 0L)
						{
							result = 3;
						}
						else if (junTuanTaskData.WeekDay != TimeUtil.GetWeekStartDayIdNow())
						{
							result = 3;
						}
						else if (!this.Persistence.RuntimeData.TaskList.Value.TryGetValue(taskId, out junTuanTaskInfo))
						{
							result = -1029;
						}
						else
						{
							DateTime taskLastTime = TimeUtil.NowDateTime();
							junTuanTaskData.TaskValue += Math.Min(addValue, junTuanTaskInfo.NumInterval - junTuanTaskData.TaskValue);
							TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanTaskAllData.Age);
							if (junTuanTaskData.TaskValue >= junTuanTaskInfo.NumInterval)
							{
								junTuanTaskData.TaskState = 1L;
								junTuanDetailData.JunTuanTaskAllData.V.TaskLastTime = taskLastTime;
								this.Persistence.AddJunTuanPoint(junTuanDetailData, junTuanTaskInfo.Score, false);
								this.Persistence.AddJuntuanEventLog(junTuanDetailData, new JunTuanEventLog
								{
									EventType = 4,
									Message = junTuanTaskInfo.Name,
									Time = time
								});
								this.Persistence.UpdateJunTuanTaskData(junTuanId, junTuanTaskData, time, junTuanTaskInfo.Score);
							}
							else
							{
								this.Persistence.UpdateJunTuanTaskData(junTuanId, junTuanTaskData);
							}
							result = junTuanTaskData.TaskValue;
						}
					}
				}
			}
			return result;
		}

		public KuaFuCmdData GetJunTuanRankingData(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (dataAge != this.JunTuanRankDataListCmdData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanRankDataListCmdData.Age,
						Bytes0 = this.JunTuanRankDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanRankDataListCmdData.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetJunTuanLogList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int key;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					result = null;
				}
				else
				{
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(key, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = null;
					}
					else if (dataAge != junTuanDetailData.EventLogListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.EventLogListCmdData.Age,
							Bytes0 = junTuanDetailData.EventLogListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = junTuanDetailData.EventLogListCmdData.Age
						};
					}
				}
			}
			return result;
		}

		private void CheckRoleTimerProc(DateTime now)
		{
			if (this.KarenFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					foreach (KuaFuData<KarenFuBenData> kuaFuData in this.KarenFuBenDataDict.Values)
					{
						List<int> list = new List<int>(new int[kuaFuData.V.EnterGameRoleCount.Count]);
						List<int> list2 = new List<int>();
						foreach (KarenFuBenRoleData karenFuBenRoleData in kuaFuData.V.RoleDict.Values)
						{
							if (karenFuBenRoleData.State == 4)
							{
								if (karenFuBenRoleData.StateEndTime < now)
								{
									karenFuBenRoleData.State = 0;
									list2.Add(karenFuBenRoleData.RoleId);
									LogManager.WriteLog(2, string.Format("阿卡伦战场角色状态数据清除 rid={0} endtm={1} state={2}", karenFuBenRoleData.RoleId, karenFuBenRoleData.StateEndTime, karenFuBenRoleData.State), null, true);
								}
								else
								{
									List<int> list3;
									int index;
									(list3 = list)[index = karenFuBenRoleData.Side - 1] = list3[index] + 1;
								}
							}
						}
						kuaFuData.V.EnterGameRoleCount = list;
						foreach (int key in list2)
						{
							kuaFuData.V.RoleDict.Remove(key);
						}
						kuaFuData.Age += 1L;
					}
				}
			}
		}

		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.KarenFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, KuaFuData<KarenFuBenData>> keyValuePair in this.KarenFuBenDataDict)
					{
						if (keyValuePair.Value.V.EndTime < now)
						{
							list.Add(keyValuePair.Key);
							LogManager.WriteLog(2, string.Format("阿卡伦战场副本数据清除 gameId={0} state={1} endtm={2}", keyValuePair.Value.V.GameId, keyValuePair.Value.V.State, keyValuePair.Value.V.EndTime), null, true);
							ClientAgentManager.Instance().RemoveKfFuben(keyValuePair.Value.V.GameType, keyValuePair.Value.V.ServerId, (long)keyValuePair.Value.V.GameId);
						}
					}
					foreach (int key in list)
					{
						this.KarenFuBenDataDict.Remove(key);
					}
				}
			}
		}

		public void UpdateKuaFuMapClientCount(int serverId, int gameId, List<int> mapClientCountList)
		{
			lock (this.Mutex)
			{
				if (mapClientCountList != null && mapClientCountList.Count > 0)
				{
					ClientAgent currentClientAgent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (null != currentClientAgent)
					{
						KuaFuData<KarenFuBenData> kuaFuData = null;
						if (this.KarenFuBenDataDict.TryGetValue(gameId, out kuaFuData))
						{
							kuaFuData.V.RoleCountSideList = mapClientCountList;
							kuaFuData.Age += 1L;
						}
					}
				}
			}
		}

		public KuaFuCmdData GetKarenKuaFuFuBenData(int gameType, int mapCode, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				JunTuanBattleMiniInfo junTuanBattleMiniInfo = JunTuanPersistence.Instance.RuntimeData.KarenBattleMapList.Value.Find((JunTuanBattleMiniInfo x) => x.MapCode == mapCode);
				if (null == junTuanBattleMiniInfo)
				{
					result = null;
				}
				else
				{
					KuaFuData<KarenFuBenData> kuaFuData = null;
					if (!this.KarenFuBenDataDict.TryGetValue(mapCode, out kuaFuData))
					{
						kuaFuData = new KuaFuData<KarenFuBenData>();
						kuaFuData.V.GameId = mapCode;
						kuaFuData.V.State = 0;
						kuaFuData.V.EndTime = Global.NowTime.AddMinutes(65.0);
						kuaFuData.V.RoleCountSideList = new List<int>(new int[junTuanBattleMiniInfo.LegionsMax]);
						kuaFuData.V.EnterGameRoleCount = new List<int>(new int[junTuanBattleMiniInfo.LegionsMax]);
						kuaFuData.V.RoleDict = new Dictionary<int, KarenFuBenRoleData>();
						kuaFuData.V.GameType = gameType;
						if (!ClientAgentManager.Instance().SpecialKfFuben(gameType, (long)mapCode, junTuanBattleMiniInfo.MaxEnterNum * junTuanBattleMiniInfo.LegionsMax, out kuaFuData.V.ServerId))
						{
							LogManager.WriteLog(2, string.Format("阿卡伦分配游戏服务器失败 gameType={0}, mapCode={1}", gameType, mapCode), null, true);
							return null;
						}
						this.KarenFuBenDataDict[mapCode] = kuaFuData;
						kuaFuData.Age += 1L;
					}
					if (dataAge != kuaFuData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KarenFuBenData>(kuaFuData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = kuaFuData.Age
						};
					}
				}
			}
			return result;
		}

		public KarenFuBenRoleData GetKarenFuBenRoleData(int gameId, int roleId)
		{
			KarenFuBenRoleData result;
			lock (this.Mutex)
			{
				KuaFuData<KarenFuBenData> kuaFuData = null;
				if (this.KarenFuBenDataDict == null || !this.KarenFuBenDataDict.TryGetValue(gameId, out kuaFuData))
				{
					result = null;
				}
				else
				{
					KarenFuBenRoleData karenFuBenRoleData = null;
					if (!kuaFuData.V.RoleDict.TryGetValue(roleId, out karenFuBenRoleData))
					{
						result = null;
					}
					else
					{
						result = karenFuBenRoleData;
					}
				}
			}
			return result;
		}

		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int side, int state)
		{
			lock (this.Mutex)
			{
				JunTuanBattleMiniInfo junTuanBattleMiniInfo = JunTuanPersistence.Instance.RuntimeData.KarenBattleMapList.Value.Find((JunTuanBattleMiniInfo x) => x.MapCode == gameId);
				if (null == junTuanBattleMiniInfo)
				{
					return -11003;
				}
				KuaFuData<KarenFuBenData> kuaFuData = null;
				if (this.KarenFuBenDataDict == null || !this.KarenFuBenDataDict.TryGetValue(gameId, out kuaFuData))
				{
					return -11003;
				}
				KarenFuBenRoleData karenFuBenRoleData = null;
				if (!kuaFuData.V.RoleDict.TryGetValue(roleId, out karenFuBenRoleData))
				{
					karenFuBenRoleData = new KarenFuBenRoleData
					{
						ServerId = serverId,
						RoleId = roleId,
						KuaFuServerId = kuaFuData.V.ServerId,
						KuaFuMapCode = kuaFuData.V.GameId,
						Side = side,
						State = 0
					};
					kuaFuData.V.RoleDict[roleId] = karenFuBenRoleData;
				}
				if (4 == state)
				{
					if (kuaFuData.V.GetRoleCountWithEnter(side) >= junTuanBattleMiniInfo.MaxEnterNum)
					{
						return -22;
					}
					if (karenFuBenRoleData.State == null || karenFuBenRoleData.State == 7)
					{
						List<int> list;
						int index;
						(list = kuaFuData.V.EnterGameRoleCount)[index = side - 1] = list[index] + 1;
						karenFuBenRoleData.StateEndTime = Global.NowTime.AddMinutes(1.0);
					}
					kuaFuData.V.State = 2;
				}
				else if (5 == state)
				{
					List<int> list;
					int index;
					if (karenFuBenRoleData.State == 4)
					{
						(list = kuaFuData.V.EnterGameRoleCount)[index = side - 1] = list[index] - 1;
					}
					(list = kuaFuData.V.RoleCountSideList)[index = side - 1] = list[index] + 1;
				}
				else if (7 == state)
				{
					List<int> list;
					int index;
					(list = kuaFuData.V.RoleCountSideList)[index = side - 1] = list[index] - 1;
				}
				else if (6 == state)
				{
					kuaFuData.V.State = 3;
				}
				karenFuBenRoleData.Side = side;
				karenFuBenRoleData.State = state;
				kuaFuData.Age += 1L;
			}
			return state;
		}

		private JunTuanDetailData JuntuanAdd(JunTuanData data, JunTuanBangHuiData bhData)
		{
			JunTuanDetailData result;
			lock (this.Mutex)
			{
				int junTuanId = data.JunTuanId;
				JunTuanDetailData junTuanDetailData;
				if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
				{
					junTuanDetailData = new JunTuanDetailData
					{
						JunTuanId = data.JunTuanId
					};
					this.JunTuanAllDataDict[junTuanId] = junTuanDetailData;
				}
				junTuanDetailData.JunTuanData.V = data;
				this.JunTuanAddBangHui(junTuanDetailData, bhData);
				result = junTuanDetailData;
			}
			return result;
		}

		private void JunTuanChangeBangHui(JunTuanDetailData detailData, int bhid1, int bhid2)
		{
			int num = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid1);
			int num2 = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid2);
			if (num >= 0 && num2 >= 0)
			{
				JunTuanBangHuiData value = detailData.JunTuanBangHuiList.V[num];
				detailData.JunTuanBangHuiList.V[num] = detailData.JunTuanBangHuiList.V[num2];
				detailData.JunTuanBangHuiList.V[num2] = value;
				for (int i = 0; i < detailData.JunTuanBangHuiList.V.Count; i++)
				{
					if (i == 0)
					{
						detailData.JunTuanBangHuiList.V[i].JuTuanZhiWu = 1;
					}
					else
					{
						detailData.JunTuanBangHuiList.V[i].JuTuanZhiWu = 0;
					}
					this.Persistence.UpdateJunTuanBangHuiData(detailData.JunTuanBangHuiList.V[i], detailData.JunTuanId);
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiList.Age);
				this.Persistence.JunTuanUpdateBhList(detailData, true, true);
				foreach (JunTuanRoleData junTuanRoleData in detailData.JunTuanRoleDataList.V)
				{
					if (junTuanRoleData.JuTuanZhiWu == 1 || junTuanRoleData.JuTuanZhiWu == 2)
					{
						if (junTuanRoleData.BhId == bhid2)
						{
							junTuanRoleData.JuTuanZhiWu = 1;
							detailData.JunTuanData.V.LeaderZoneId = junTuanRoleData.ZoneId;
							detailData.JunTuanData.V.LeaderName = junTuanRoleData.RoleName;
							detailData.JunTuanData.V.LeaderRoleId = junTuanRoleData.RoleId;
						}
						else
						{
							junTuanRoleData.JuTuanZhiWu = 2;
						}
						this.Persistence.UpdateJuntuanRoleData(junTuanRoleData);
					}
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				this.Persistence.UpdateRoleDataListCmdData(detailData);
			}
		}

		private void JunTuanRemoveBangHui(JunTuanDetailData detailData, int bhid)
		{
			this.BangHuiJunTuanIdDict[bhid] = 0;
			if (null != detailData.JunTuanBangHuiList.V)
			{
				detailData.JunTuanBangHuiList.V.RemoveAll((JunTuanBangHuiData x) => x.BhId == bhid);
				detailData.JunTuanBangHuiList.Age += 1L;
			}
			if (null != detailData.JunTuanBaseData.V)
			{
				detailData.JunTuanBaseData.V.BhList.Remove(bhid);
				detailData.JunTuanBaseData.Age += 1L;
			}
			this.Persistence.JunTuanUpdateBhList(detailData, true, false);
			if (detailData.JunTuanBangHuiList.V.Count == 0)
			{
				this.Persistence.DestroyJunTuan(detailData);
			}
		}

		public int GetJunTuanPoint(int bhid, int junTuanId)
		{
			int result;
			lock (this.Mutex)
			{
				int num;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num != junTuanId)
				{
					result = -1020;
				}
				else
				{
					JunTuanDetailData junTuanDetailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						flag2 = (junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = -1024;
					}
					else
					{
						result = junTuanDetailData.JunTuanData.V.Point;
					}
				}
			}
			return result;
		}

		public int ExecCommand(string[] args)
		{
			int result = -1;
			try
			{
				if (string.Compare(args[0], "reload") == 0 && 0 == string.Compare(args[1], "paihang"))
				{
					this.ExecPaiHang = true;
				}
				else if (0 == string.Compare(args[0], "load"))
				{
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public KuaFuCmdData GetYaoSaiPrisonRoleData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonRoleData(rid, dataAge);
		}

		public KuaFuCmdData GetYaoSaiFuLuListData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiFuLuListData(rid, dataAge);
		}

		public KuaFuCmdData GetYaoSaiPrisonLogData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonLogData(rid, dataAge);
		}

		public KuaFuCmdData GetYaoSaiPrisonJingJiData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonJingJiData(rid, dataAge);
		}

		public KuaFuCmdData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			return YaoSaiService.Instance().SearchYaoSaiFuLu(rid, unionlev, faction, frindSet);
		}

		public int YaoSaiPrisonOpt(int srcrid, int otherrid, int type, bool success)
		{
			return YaoSaiService.Instance().YaoSaiPrisonOpt(srcrid, otherrid, type, success);
		}

		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole data)
		{
			return YaoSaiService.Instance().UpdateYaoSaiPrisonRoleData(data);
		}

		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			return YaoSaiService.Instance().YaoSaiPrisonHuDong(ownerid, fuluid, type, param0, param1, param2);
		}

		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			return YaoSaiService.Instance().UpdateYaoSaiPrisonLogData(rid, id, state);
		}

		public KuaFuCmdData GetEraRankData(long dataAge)
		{
			return JunTuanEraService.Instance().GetEraRankData(dataAge);
		}

		public KuaFuCmdData GetEraData(int juntuanid, long dataAge)
		{
			return JunTuanEraService.Instance().GetEraData(juntuanid, dataAge);
		}

		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			return JunTuanEraService.Instance().EraDonate(juntuanid, taskid, var1, var2, var3);
		}

		public int SetDoubleOpenTime(int roleId, int lingDiType, DateTime openTime, int openSeconds)
		{
			return LingDiCaiJiService.Instance().SetDoubleOpenTime(roleId, lingDiType, openTime, openSeconds);
		}

		public int SetShouWeiTime(int roleId, int bhid, int lingDiType, DateTime openTime, int index, int junTuanPointCost)
		{
			int num;
			int result;
			if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num))
			{
				result = -1025;
			}
			else
			{
				int junTuanPoint = this.GetJunTuanPoint(bhid, num);
				JunTuanDetailData junTuanDetailData;
				if (junTuanPoint < junTuanPointCost)
				{
					result = -1030;
				}
				else if (!this.JunTuanAllDataDict.TryGetValue(num, out junTuanDetailData) || junTuanDetailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) < 0)
				{
					result = -1025;
				}
				else
				{
					this.Persistence.AddJunTuanPoint(junTuanDetailData, -junTuanPointCost, false);
					result = LingDiCaiJiService.Instance().SetShouWeiTime(roleId, lingDiType, openTime, index);
				}
			}
			return result;
		}

		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			return LingDiCaiJiService.Instance().CanEnterKuaFuMap(roleId, lingDiType);
		}

		public List<LingDiData> GetLingDiData()
		{
			return LingDiCaiJiService.Instance().GetLingDiData();
		}

		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			return LingDiCaiJiService.Instance().SetLingZhu(roleId, lingDiType, junTuanId, junTuanName, zhiWu, roledata);
		}

		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			return LingDiCaiJiService.Instance().SetShouWei(lingDiType, shouWeiList);
		}

		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			return LingDiCaiJiService.Instance().UpdateMapRoleNum(lingDiType, roleNum, serverId);
		}

		public int GetLingDiRoleNum(int lingDiType)
		{
			return LingDiCaiJiService.Instance().GetLingDiRoleNum(lingDiType);
		}

		public bool GetClientCacheItems(int serverId)
		{
			return LingDiCaiJiService.Instance().GetClientCacheItems(serverId);
		}

		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			return HongBaoManager_K.getInstance().GetHongBaoDataList(dataAge);
		}

		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			return HongBaoManager_K.getInstance().OpenHongBao(hongBaoId, rid, zoneid, userid, rname);
		}

		public void AddServerTotalCharge(string keyStr, long addCharge)
		{
			HongBaoManager_K.getInstance().AddServerTotalCharge(keyStr, addCharge);
		}

		private const double SaveServerStateProcInterval = 30.0;

		public static JunTuanService Instance = null;

		public readonly GameTypes GameType = 21;

		private DateTime CheckTime20;

		private DateTime CheckTimer120;

		private DateTime CheckTimer3600;

		private bool ExecPaiHang = false;

		private int LastUpdateRankHour = -1;

		public JunTuanPersistence Persistence = JunTuanPersistence.Instance;

		private Dictionary<int, int> BangHuiId2RoleDataUpdateDayIdDict = new Dictionary<int, int>();

		private Dictionary<int, KuaFuData<KarenFuBenData>> KarenFuBenDataDict = new Dictionary<int, KuaFuData<KarenFuBenData>>();

		public Thread BackgroundThread;
	}
}
