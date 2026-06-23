using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class zhengDuoService
	{
		public static zhengDuoService Instance()
		{
			return zhengDuoService._instance;
		}

		public void InitConfig()
		{
			if (!this._config.Load(KuaFuServerManager.GetResourcePath("Config\\PlunderLands.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\PlunderLandsMonster.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\PlunderLandsRebirth.xml", KuaFuServerManager.ResourcePathTypes.GameRes)))
			{
				LogManager.WriteLog(2, string.Format("加载[{0}]时出错!!!", "争夺之地"), null, true);
			}
			else
			{
				this.LoadDBConfig();
			}
		}

		private void LoadDBConfig()
		{
			lock (this.Mutex)
			{
				int num;
				for (;;)
				{
					num = this._persistence.DBWeekAndStepGet(32);
					if (num >= 0)
					{
						break;
					}
					Thread.Sleep(2000);
				}
				this.StepProcessEnd = num;
				DateTime weekStartTimeNow = TimeUtil.GetWeekStartTimeNow();
				DateTime d = TimeUtil.NowDateTime();
				TimeSpan t = d - weekStartTimeNow;
				if (t >= this._config.FirstStartTime)
				{
					this.RankTime = weekStartTimeNow;
				}
				else
				{
					this.RankTime = weekStartTimeNow.AddDays(-7.0);
				}
				int offsetDay = TimeUtil.GetOffsetDay(this.RankTime);
				this.ReloadRankDatas(offsetDay);
				this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
			}
		}

		private void ReloadRankDatas(int weekDay)
		{
			Dictionary<int, ZhengDuoRankData> dictionary;
			for (;;)
			{
				dictionary = this._persistence.DBRankList(weekDay);
				if (dictionary != null)
				{
					break;
				}
				Thread.Sleep(2000);
			}
			this.RankDict = dictionary;
			Array.Clear(this.SyncData.RankDatas, 0, this.SyncData.RankDatas.Length);
			for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
			{
				ZhengDuoRankData zhengDuoRankData;
				if (this.RankDict.TryGetValue(i, out zhengDuoRankData) && zhengDuoRankData.Bhid > 0)
				{
					this.SyncData.RankDatas[i] = zhengDuoRankData;
				}
			}
		}

		private ZhengDuoSceneInfo GetCurrentZhengDuoSceneInfo(TimeSpan timeOfWeek)
		{
			foreach (ZhengDuoSceneInfo zhengDuoSceneInfo in this._config.SceneDataDict.Values)
			{
				if (timeOfWeek >= zhengDuoSceneInfo.TimeBegin && timeOfWeek < zhengDuoSceneInfo.NextTime)
				{
					return zhengDuoSceneInfo;
				}
			}
			return null;
		}

		public int GetSuccessRank(EZhengDuoStep step)
		{
			int result;
			switch (step)
			{
			case 2:
				result = 8;
				break;
			case 3:
				result = 4;
				break;
			case 4:
				result = 2;
				break;
			case 5:
				result = 1;
				break;
			default:
				result = 16;
				break;
			}
			return result;
		}

		public ZhengDuoSyncData ZhengDuoSync(int serverID, long version)
		{
			ZhengDuoSyncData result;
			if (version < this.SyncData.Age)
			{
				result = this.SyncData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int ZhengDuoSign(int serverID, int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
			lock (this.Mutex)
			{
				if (this.SyncData.WeekDay == weekStartDayIdNow && this.SyncData.ZhengDuoStep == 1)
				{
					List<ZhengDuoRankData> list = new List<ZhengDuoRankData>();
					int num = 0;
					for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
					{
						ZhengDuoRankData zhengDuoRankData = this.SyncData.RankDatas[i];
						if (zhengDuoRankData != null && zhengDuoRankData.Bhid != bhid)
						{
							list.Add(zhengDuoRankData);
							if (zhengDuoRankData.UsedMillisecond < usedTime)
							{
								num = list.Count;
							}
						}
					}
					if (num < 16)
					{
						ZhengDuoRankData zhengDuoRankData2 = new ZhengDuoRankData
						{
							Bhid = bhid,
							UsedMillisecond = usedTime,
							ServerID = serverID,
							ZoneId = zoneId,
							BhName = bhName,
							BhLevel = bhLevel,
							ZhanLi = bhZhanLi,
							Week = weekStartDayIdNow,
							State = 1
						};
						zhengDuoRankData2.Rank1 = num;
						zhengDuoRankData2.Rank2 = 16;
						list.Insert(num, zhengDuoRankData2);
						LogManager.WriteLog(0, string.Format("争夺之地提交海选结果#bhid={0},usedTime={1},week={2}", bhid, usedTime, weekStartDayIdNow), null, true);
						for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
						{
							if (i < list.Count)
							{
								if (this.SyncData.RankDatas[i] != list[i])
								{
									list[i].Rank1 = i;
									this._persistence.DBRankUpdata(list[i]);
								}
								this.SyncData.RankDatas[i] = list[i];
							}
							else
							{
								this.SyncData.RankDatas[i] = null;
							}
						}
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
				}
				else
				{
					LogManager.WriteLog(2, string.Format("争夺之地提交海选结果失败，非海选时间拒绝提交", new object[0]), null, true);
				}
			}
			return 0;
		}

		private List<ZhengDuoRankData> GetListByGroup(int rank1, int step)
		{
			List<ZhengDuoRankData> list = new List<ZhengDuoRankData>();
			List<ZhengDuoRankData> result;
			if (step < 1 || step > 5)
			{
				result = list;
			}
			else
			{
				bool flag = false;
				int successRank = this.GetSuccessRank(step);
				int num = 16 / successRank;
				for (int i = 0; i < successRank; i++)
				{
					list.Clear();
					for (int j = 0; j < num; j++)
					{
						int num2 = KFZhengDuoConfig.GroupInfo[i * num + j];
						if (num2 == rank1)
						{
							flag = true;
						}
						ZhengDuoRankData zhengDuoRankData = this.SyncData.RankDatas[num2];
						if (null != zhengDuoRankData)
						{
							list.Add(zhengDuoRankData);
						}
					}
					if (flag)
					{
						break;
					}
				}
				result = list;
			}
			return result;
		}

		private ZhengDuoRankData GetEnemy(ZhengDuoRankData rankData0, int step)
		{
			int num = step - 2;
			ZhengDuoRankData result;
			if (num < 0)
			{
				result = null;
			}
			else
			{
				int successRank = this.GetSuccessRank(step);
				int successRank2 = this.GetSuccessRank(step - 1);
				lock (this.Mutex)
				{
					if (rankData0 == null || rankData0.Rank2 != successRank2)
					{
						return null;
					}
					int num2 = rankData0.Rank1 >> num;
					for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
					{
						ZhengDuoRankData zhengDuoRankData = this.SyncData.RankDatas[i];
						if (zhengDuoRankData != null && zhengDuoRankData.Rank2 == successRank2)
						{
							int num3 = zhengDuoRankData.Rank1 >> num;
							if (num2 + num3 == successRank2)
							{
								return zhengDuoRankData;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			int result;
			if (bhids == null || bhids.Length < 2)
			{
				result = -18;
			}
			else
			{
				int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
				lock (this.Mutex)
				{
					if (this.SyncData.WeekDay == weekStartDayIdNow && this.SyncData.ZhengDuoStep > 1)
					{
						int num = int.MaxValue;
						int rank = this.GetSuccessRank(this.SyncData.ZhengDuoStep);
						List<ZhengDuoRankData> list = new List<ZhengDuoRankData>();
						for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
						{
							ZhengDuoRankData zhengDuoRankData = this.SyncData.RankDatas[i];
							if (zhengDuoRankData != null && bhids.Contains(zhengDuoRankData.Bhid))
							{
								list.Add(zhengDuoRankData);
								if (bhidSuccess > 0)
								{
									if (bhidSuccess == zhengDuoRankData.Bhid)
									{
										num = zhengDuoRankData.Rank1;
									}
								}
								else if (num > zhengDuoRankData.Rank1)
								{
									num = zhengDuoRankData.Rank1;
								}
							}
						}
						if (list.Any((ZhengDuoRankData x) => x.Rank2 == rank))
						{
							LogManager.WriteLog(0, string.Format("争夺之地提交淘汰赛结果失败，已过期#successbhid={0},bhid0={1},bhid1={2},week={3}", new object[]
							{
								bhidSuccess,
								bhids[0],
								bhids[1],
								weekStartDayIdNow
							}), null, true);
							return 0;
						}
						if (num >= 16)
						{
							return -18;
						}
						ZhengDuoFuBenData zhengDuoFuBenData;
						if (list.Count > 0 && this.Bhid2FuBenDict.TryGetValue(list[0].Bhid, out zhengDuoFuBenData))
						{
							zhengDuoFuBenData.State = 3;
						}
						LogManager.WriteLog(0, string.Format("争夺之地提交淘汰赛结果#successbhid={0},bhid0={1},bhid1={2},week={3}", new object[]
						{
							bhidSuccess,
							bhids[0],
							bhids[1],
							weekStartDayIdNow
						}), null, true);
						foreach (ZhengDuoRankData zhengDuoRankData2 in list)
						{
							zhengDuoRankData2.State = 0;
							if (zhengDuoRankData2.Rank1 == num)
							{
								zhengDuoRankData2.Rank2 = this.GetSuccessRank(this.SyncData.ZhengDuoStep);
								zhengDuoRankData2.Enemy = 0;
							}
							else
							{
								zhengDuoRankData2.Lose = 1;
							}
							this._persistence.DBRankUpdata(zhengDuoRankData2);
						}
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
					else
					{
						LogManager.WriteLog(2, string.Format("争夺之地提交海选结果失败，非海选时间拒绝提交", new object[0]), null, true);
					}
				}
				result = 0;
			}
			return result;
		}

		private void ProcessZhengDuoRank(int step, bool notify = true)
		{
			if (step >= 1)
			{
				int successRank = this.GetSuccessRank(step);
				int successRank2 = this.GetSuccessRank(step - 1);
				int successRank3 = this.GetSuccessRank(step + 1);
				lock (this.Mutex)
				{
					bool flag2 = false;
					int i;
					if (step >= 2)
					{
						for (i = 0; i < successRank; i++)
						{
							bool flag3 = false;
							int num = int.MaxValue;
							List<ZhengDuoRankData> listByGroup = this.GetListByGroup(i, step);
							foreach (ZhengDuoRankData zhengDuoRankData in listByGroup)
							{
								if (zhengDuoRankData.Rank2 == successRank)
								{
									flag3 = true;
								}
								else if (zhengDuoRankData.Rank2 == successRank2 && zhengDuoRankData.Rank1 < num)
								{
									num = zhengDuoRankData.Rank1;
								}
							}
							if (!flag3 && num < 16)
							{
								int[] array = new int[2];
								foreach (ZhengDuoRankData zhengDuoRankData in listByGroup)
								{
									zhengDuoRankData.State = 0;
									if (zhengDuoRankData.Rank1 == num)
									{
										array[0] = zhengDuoRankData.Bhid;
										zhengDuoRankData.Rank2 = successRank;
										zhengDuoRankData.Enemy = 0;
									}
									else
									{
										array[1] = zhengDuoRankData.Bhid;
										zhengDuoRankData.Lose = 1;
									}
									this._persistence.DBRankUpdata(zhengDuoRankData);
								}
								flag2 = true;
								LogManager.WriteLog(0, string.Format("争夺之地提交淘汰赛结果，无战斗结果，自动判定#successbhid={0},otherBhid={1},week={2}", array[0], array[1], this.SyncData.WeekDay), null, true);
							}
						}
					}
					i = 0;
					while (i < successRank3)
					{
						bool flag3 = false;
						List<int> list = new List<int>();
						List<ZhengDuoRankData> listByGroup = this.GetListByGroup(i, step + 1);
						foreach (ZhengDuoRankData zhengDuoRankData in listByGroup)
						{
							if (zhengDuoRankData.Rank2 == successRank)
							{
								list.Add(zhengDuoRankData.Rank1);
							}
							else
							{
								zhengDuoRankData.Lose = 1;
								this._persistence.DBRankUpdata(zhengDuoRankData);
							}
						}
						if (list.Count == 2)
						{
							this.SyncData.RankDatas[list[0]].Enemy = this.SyncData.RankDatas[list[1]].Bhid;
							this.SyncData.RankDatas[list[1]].Enemy = this.SyncData.RankDatas[list[0]].Bhid;
							LogManager.WriteLog(0, string.Format("争夺之地分配对手#{0}<==>{1}", this.SyncData.RankDatas[list[0]].Bhid, this.SyncData.RankDatas[list[1]].Bhid), null, true);
							goto IL_3B9;
						}
						if (list.Count == 1)
						{
							this.SyncData.RankDatas[list[0]].Enemy = 0;
							LogManager.WriteLog(0, string.Format("争夺之地分配对手#{0}无对手,直接晋级", this.SyncData.RankDatas[list[0]].Bhid), null, true);
							goto IL_3B9;
						}
						IL_410:
						i++;
						continue;
						IL_3B9:
						foreach (int num2 in list)
						{
							ZhengDuoRankData data = this.SyncData.RankDatas[num2];
							flag2 = true;
							this._persistence.DBRankUpdata(data);
						}
						goto IL_410;
					}
					if (notify && flag2)
					{
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
				}
			}
		}

		public int GmCommand(string[] args, byte[] data)
		{
			if (args.Length > 0)
			{
				if (args[0] == "-zhengduo")
				{
					int num;
					int num2;
					if (args.Length >= 3 && int.TryParse(args[1], out num) && int.TryParse(args[2], out num2))
					{
						if (num == 10)
						{
							lock (this.Mutex)
							{
								this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
							}
						}
						else
						{
							lock (this.Mutex)
							{
								this.SyncData.ZhengDuoStep = num;
								this.SyncData.State = num2;
								if (num2 > 0)
								{
									this.ProcessZhengDuoRank(this.SyncData.ZhengDuoStep - 1, true);
								}
								else
								{
									this.ProcessZhengDuoRank(this.SyncData.ZhengDuoStep, true);
								}
								this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
							}
						}
					}
				}
			}
			return 0;
		}

		private void ClearZhengDuoFuBenData()
		{
			lock (this.Mutex)
			{
				if (this.SyncData.State == 0)
				{
					foreach (ZhengDuoFuBenData zhengDuoFuBenData in this.FuBenDict.Values)
					{
						try
						{
							ClientAgentManager.Instance().RemoveKfFuben(17, zhengDuoFuBenData.ServerId, zhengDuoFuBenData.GameId);
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			ZhengDuoFuBenData zhengDuoFuBenData = null;
			lock (this.Mutex)
			{
				if (this.SyncData.ZhengDuoStep < 2 || this.SyncData.State == 0)
				{
					return null;
				}
				if (this.Bhid2FuBenDict.TryGetValue(bhid, out zhengDuoFuBenData) && zhengDuoFuBenData.WeekDay == this.SyncData.WeekDay && zhengDuoFuBenData.GroupIndex == this.SyncData.ZhengDuoStep)
				{
					return zhengDuoFuBenData;
				}
				zhengDuoFuBenData = null;
				int zhengDuoStep = this.SyncData.ZhengDuoStep;
				int successRank = this.GetSuccessRank(zhengDuoStep - 1);
				List<ZhengDuoRankData> list = new List<ZhengDuoRankData>();
				for (int i = 0; i < successRank; i++)
				{
					List<ZhengDuoRankData> listByGroup = this.GetListByGroup(i, zhengDuoStep);
					foreach (ZhengDuoRankData zhengDuoRankData in listByGroup)
					{
						if (zhengDuoRankData.Rank2 == successRank)
						{
							if (zhengDuoRankData.Bhid == bhid || zhengDuoRankData.Enemy == bhid)
							{
								list.Add(zhengDuoRankData);
							}
						}
					}
					if (list.Count >= 1)
					{
						int num = 0;
						long num2 = this._persistence.CreateZhengDuoFuBen(17, num);
						if (num2 > 0L && ClientAgentManager.Instance().AssginKfFuben(17, num2, 60, out num))
						{
							zhengDuoFuBenData = new ZhengDuoFuBenData
							{
								GameId = num2,
								ServerId = num,
								GroupIndex = this.SyncData.ZhengDuoStep,
								State = 2,
								WeekDay = this.SyncData.WeekDay,
								PlayerDict = new Dictionary<int, int>()
							};
							this.FuBenDict[zhengDuoFuBenData.GameId] = zhengDuoFuBenData;
							zhengDuoFuBenData.PlayerDict[list[0].Bhid] = 1;
							this.Bhid2FuBenDict[list[0].Bhid] = zhengDuoFuBenData;
							if (list.Count >= 2)
							{
								zhengDuoFuBenData.PlayerDict[list[1].Bhid] = 2;
								this.Bhid2FuBenDict[list[1].Bhid] = zhengDuoFuBenData;
							}
							LogManager.WriteLog(0, string.Format("争夺之地分配副本#gameId={0},serverId={1},{2}<==>{3}", new object[]
							{
								num2,
								num,
								list[0].Bhid,
								(list.Count >= 2) ? list[1].Bhid : 0
							}), null, true);
							break;
						}
					}
				}
			}
			return zhengDuoFuBenData;
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			lock (this.Mutex)
			{
				ZhengDuoFuBenData result;
				if (this.FuBenDict.TryGetValue(gameId, out result))
				{
					return result;
				}
			}
			return null;
		}

		public void Update(DateTime now)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(11))
			{
				bool flag = false;
				int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
				TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow();
				lock (this.Mutex)
				{
					if (this.SyncData.WeekDay != weekStartDayIdNow)
					{
						this.SyncData.WeekDay = weekStartDayIdNow;
						flag = true;
					}
					if (timeOfWeekNow < this._config.FirstStartTime)
					{
						this.CurrentSceneInfo = null;
						if (this.SyncData.ZhengDuoStep != 0 || this.SyncData.State > 0)
						{
							this.SyncData.ZhengDuoStep = 0;
							this.SyncData.State = 0;
							flag = true;
						}
					}
					else if (this.CurrentSceneInfo == null)
					{
						ZhengDuoSceneInfo currentZhengDuoSceneInfo = this.GetCurrentZhengDuoSceneInfo(timeOfWeekNow);
						if (null == currentZhengDuoSceneInfo)
						{
							return;
						}
						int num = this._persistence.DBWeekAndStepGet(31);
						if (currentZhengDuoSceneInfo.Id == 1)
						{
							this.SyncData.ZhengDuoStep = 1;
							this.StepProcessEnd = 0;
							this.ReloadRankDatas(weekStartDayIdNow);
							LogManager.WriteLog(0, string.Format("争夺之地,进入海选阶段", new object[0]), null, true);
						}
						else if (num == currentZhengDuoSceneInfo.Id - 1 || num == currentZhengDuoSceneInfo.Id)
						{
							LogManager.WriteLog(0, string.Format("争夺之地,进入{0}阶段", currentZhengDuoSceneInfo.Id), null, true);
							this.SyncData.ZhengDuoStep = currentZhengDuoSceneInfo.Id;
						}
						else
						{
							this.SyncData.ZhengDuoStep = 0;
							this.SyncData.State = 0;
							LogManager.WriteLog(0, string.Format("争夺之地,因为前阶段的海选或淘汰赛未开启，本期活动不开启#current={0},last={1}", currentZhengDuoSceneInfo.Id, num), null, true);
						}
						if (this.SyncData.ZhengDuoStep > 0)
						{
							while (this.StepProcessEnd < this.SyncData.ZhengDuoStep - 1)
							{
								this.StepProcessEnd++;
								this.ProcessZhengDuoRank(this.StepProcessEnd, false);
								this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
							}
							this._persistence.DBWeekAndStepSet(31, this.SyncData.ZhengDuoStep);
							if (timeOfWeekNow >= currentZhengDuoSceneInfo.TimeBegin && timeOfWeekNow < currentZhengDuoSceneInfo.TimeEnd)
							{
								this.SyncData.State = 1;
								int successRank = this.GetSuccessRank(this.SyncData.ZhengDuoStep - 1);
								foreach (ZhengDuoRankData zhengDuoRankData in this.SyncData.RankDatas)
								{
									if (zhengDuoRankData != null && zhengDuoRankData.Rank2 == successRank && zhengDuoRankData.Lose == 0)
									{
										zhengDuoRankData.State = 1;
									}
								}
							}
							else
							{
								this.SyncData.State = 0;
							}
						}
						flag = true;
						this.CurrentSceneInfo = currentZhengDuoSceneInfo;
					}
					else
					{
						if (this.SyncData.ZhengDuoStep != this.CurrentSceneInfo.Id)
						{
							return;
						}
						if (timeOfWeekNow < this.CurrentSceneInfo.TimeEnd)
						{
							if (this.SyncData.State == 0)
							{
								this.SyncData.State = 1;
								flag = true;
							}
						}
						else if (timeOfWeekNow < this.CurrentSceneInfo.TimeProcessEnd)
						{
							if (this.SyncData.State == 1)
							{
								LogManager.WriteLog(0, string.Format("争夺之地,结束战斗状态#step={0}", this.CurrentSceneInfo.Id), null, true);
								this.SyncData.State = 0;
								flag = true;
							}
							if (this.StepProcessEnd < this.CurrentSceneInfo.Id)
							{
								bool flag3 = true;
								foreach (ZhengDuoRankData zhengDuoRankData2 in this.SyncData.RankDatas)
								{
									if (zhengDuoRankData2 != null && zhengDuoRankData2.State > 0)
									{
										flag3 = false;
										break;
									}
								}
								if (flag3)
								{
									this.ClearZhengDuoFuBenData();
									this.StepProcessEnd++;
									this.ProcessZhengDuoRank(this.StepProcessEnd, false);
									this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
									flag = true;
								}
							}
						}
						else if (timeOfWeekNow < this.CurrentSceneInfo.NextTime)
						{
							if (this.SyncData.State == 1)
							{
								this.SyncData.State = 0;
								flag = true;
							}
							if (this.StepProcessEnd < this.CurrentSceneInfo.Id)
							{
								this.ClearZhengDuoFuBenData();
								this.StepProcessEnd++;
								this.ProcessZhengDuoRank(this.StepProcessEnd, false);
								this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
								flag = true;
							}
							long num2 = TimeUtil.NOW();
							if (num2 - this.SyncData.Age > 75000L && timeOfWeekNow < this.CurrentSceneInfo.TimeProcessEnd)
							{
								flag = true;
							}
						}
						else
						{
							this.CurrentSceneInfo = null;
						}
					}
					if (flag)
					{
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(20, new object[]
						{
							this.SyncData
						}), 0);
					}
				}
			}
		}

		private const GameTypes GameType = 17;

		private static zhengDuoService _instance = new zhengDuoService();

		private ZhengDuoPersistence _persistence = ZhengDuoPersistence.Instance;

		public readonly GameTypes EvItemGameType = 2;

		public Dictionary<int, ZhengDuoRankData> RankDict = new Dictionary<int, ZhengDuoRankData>();

		public Dictionary<long, ZhengDuoFuBenData> FuBenDict = new Dictionary<long, ZhengDuoFuBenData>();

		public Dictionary<int, ZhengDuoFuBenData> Bhid2FuBenDict = new Dictionary<int, ZhengDuoFuBenData>();

		public ZhengDuoSceneInfo CurrentSceneInfo;

		public ZhengDuoSyncData SyncData = new ZhengDuoSyncData();

		public DateTime RankTime;

		private int StepProcessEnd;

		public object Mutex = new object();

		private KFZhengDuoConfig _config = new KFZhengDuoConfig();
	}
}
