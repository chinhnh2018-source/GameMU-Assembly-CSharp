using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace Remoting
{
	public class Zork5v5Service
	{
		public static Zork5v5Service Instance()
		{
			return Zork5v5Service._instance;
		}

		public void InitConfig()
		{
			try
			{
				lock (this.MutexConfig)
				{
					DateTime.TryParse(KuaFuServerManager.systemParamsList.GetParamValueByName("ZorkStartTime"), out this.ZorkStartTime);
					this.SceneDataDict.Clear();
					string fileName = "Config/ZorkActivityRules.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						ZorkBattleSceneInfo zorkBattleSceneInfo = new ZorkBattleSceneInfo();
						int num = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						int mapCode = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MapCode", 0L);
						zorkBattleSceneInfo.Id = num;
						zorkBattleSceneInfo.MapCode = mapCode;
						zorkBattleSceneInfo.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MaxEnterNum", 0L);
						zorkBattleSceneInfo.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "PrepareSecs", 0L);
						zorkBattleSceneInfo.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "FightingSecs", 0L);
						zorkBattleSceneInfo.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ClearRolesSecs", 0L);
						zorkBattleSceneInfo.BattleSignSecs = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "BattleSignSecs", 0L);
						zorkBattleSceneInfo.SeasonFightRound = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "SeasonFightDay", 0L);
						string[] array = xelement2.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < array.Length; i += 3)
						{
							TimeSpan ts = new TimeSpan(Convert.ToInt32(array[i]), 0, 0, 0);
							TimeSpan item = DateTime.Parse(array[i + 1]).TimeOfDay.Add(ts);
							TimeSpan item2 = DateTime.Parse(array[i + 2]).TimeOfDay.Add(ts);
							zorkBattleSceneInfo.TimePoints.Add(item);
							zorkBattleSceneInfo.TimePoints.Add(item2);
						}
						for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(zorkBattleSceneInfo.TimePoints[i].Hours, zorkBattleSceneInfo.TimePoints[i].Minutes, zorkBattleSceneInfo.TimePoints[i].Seconds);
							zorkBattleSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(zorkBattleSceneInfo.TimePoints[i].Hours, zorkBattleSceneInfo.TimePoints[i].Minutes, zorkBattleSceneInfo.TimePoints[i].Seconds);
							zorkBattleSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						this.SceneDataDict[num] = zorkBattleSceneInfo;
						this.SeasonWeeks = Math.Max(this.SeasonWeeks, (int)Math.Ceiling((double)zorkBattleSceneInfo.SeasonFightRound / (double)(zorkBattleSceneInfo.TimePoints.Count / 2)));
					}
					this.ZorkLevelRangeList.Clear();
					fileName = "Config/ZorkDanAward.xml";
					resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					xelement = ConfigHelper.Load(resourcePath);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						ZorkBattleAwardConfig zorkBattleAwardConfig = new ZorkBattleAwardConfig();
						zorkBattleAwardConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						zorkBattleAwardConfig.RankValue = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "RankValue", 0L);
						zorkBattleAwardConfig.WinRankValue = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "WinRankValue", 0L);
						zorkBattleAwardConfig.LoseRankValue = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "LoseRankValue", 0L);
						this.ZorkLevelRangeList.Add(zorkBattleAwardConfig);
						this.ZorkLevelRangeList.Sort(delegate(ZorkBattleAwardConfig left, ZorkBattleAwardConfig righit)
						{
							int result;
							if (left.ID > righit.ID)
							{
								result = -1;
							}
							else if (left.ID > righit.ID)
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
				lock (this.Mutex)
				{
					this.CurrentSeasonID = this.Persistence.LoadZorkSeasonID();
					bool flag2 = this.CurrentSeasonID == 0;
					this.CurrentSeasonID = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
					if (flag2)
					{
						this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
					}
					this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
					this.TopZhanDui = this.Persistence.LoadZorkTopZhanDui();
					this.UpdateTopZhanDuiInfo();
					this.TopKiller = this.Persistence.LoadZorkTopKiller();
					this.ReloadRankInfo(0, this.ZorkBattleRankInfoDict);
					this.ReloadRankInfo(1, this.ZorkBattleRankInfoDict);
					this.InitFuBenManagerData(now);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.LoadDatabase failed!", ex, true);
			}
		}

		public bool CheckOpenState(DateTime now)
		{
			return !(now < this.ZorkStartTime);
		}

		private void UpdateTopZhanDuiInfo()
		{
			this.TopZhanDuiName = "";
			if (this.TopZhanDui > 0)
			{
				TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(this.TopZhanDui);
				if (null != zhanDuiData)
				{
					this.TopZhanDuiName = KuaFuServerManager.FormatName(zhanDuiData.ZoneID, zhanDuiData.ZhanDuiName);
				}
			}
		}

		private int GetCurrentRoundByTime(DateTime now, int CurrentSeasonID)
		{
			int result;
			if (!this.CheckOpenState(now))
			{
				result = 0;
			}
			else if (!KuaFuServerManager.IsGongNengOpened(114))
			{
				result = 0;
			}
			else
			{
				lock (this.MutexConfig)
				{
					ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
					if (null == zorkBattleSceneInfo)
					{
						result = 0;
					}
					else
					{
						DateTime seasonDateTm = ZorkBattleUtils.GetSeasonDateTm(CurrentSeasonID);
						if (now < seasonDateTm)
						{
							result = 1;
						}
						else
						{
							TimeSpan t = new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second);
							if (t.Days == 0)
							{
								t += new TimeSpan(7, 0, 0, 0);
							}
							int num = 0;
							for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
							{
								TimeSpan timeSpan = zorkBattleSceneInfo.TimePoints[i + 1];
								if (timeSpan.Days == 0)
								{
									timeSpan += new TimeSpan(7, 0, 0, 0);
								}
								if (t > timeSpan)
								{
									num++;
								}
							}
							int num2 = (now - seasonDateTm).Days % (7 * this.SeasonWeeks) / 7;
							int val = num2 * zorkBattleSceneInfo.TimePoints.Count / 2 + num + 1;
							result = Math.Min(val, zorkBattleSceneInfo.SeasonFightRound + 1);
						}
					}
				}
			}
			return result;
		}

		private int ComputeCurrentSeasonID(DateTime now, int CurrentSeasonID)
		{
			int result;
			if (!this.CheckOpenState(now))
			{
				result = 0;
			}
			else if (!KuaFuServerManager.IsGongNengOpened(114))
			{
				result = 0;
			}
			else
			{
				lock (this.MutexConfig)
				{
					DateTime dateTime = ZorkBattleUtils.GetSeasonDateTm(CurrentSeasonID);
					if (dateTime == DateTime.MinValue)
					{
						TimeSpan timeSpan = TimeSpan.MaxValue;
						foreach (ZorkBattleSceneInfo zorkBattleSceneInfo in this.SceneDataDict.Values)
						{
							for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
							{
								TimeSpan timeSpan2 = zorkBattleSceneInfo.TimePoints[i];
								if (timeSpan2.Days == 0)
								{
									timeSpan2 += new TimeSpan(7, 0, 0, 0);
								}
								if (timeSpan2 < timeSpan)
								{
									timeSpan = timeSpan2;
								}
							}
						}
						timeSpan -= new TimeSpan(1, 0, 0, 0);
						int num = TimeUtil.NowDateTime().DayOfWeek - DayOfWeek.Monday;
						num = ((num >= 0) ? (-num) : (-(7 + num)));
						TimeSpan t = new TimeSpan(Math.Abs(num), now.Hour, now.Minute, now.Second);
						if (t < timeSpan)
						{
							dateTime = TimeUtil.NowDateTime().AddDays((double)num);
						}
						else
						{
							dateTime = TimeUtil.NowDateTime().AddDays((double)(num + 7));
						}
					}
					else if ((now - dateTime).Days >= this.SeasonWeeks * 7)
					{
						int num = TimeUtil.NowDateTime().DayOfWeek - DayOfWeek.Monday;
						num = ((num >= 0) ? (-num) : (-(7 + num)));
						dateTime = TimeUtil.NowDateTime().AddDays((double)num);
					}
					result = ZorkBattleUtils.MakeSeason(dateTime);
				}
			}
			return result;
		}

		public bool ReloadRankInfo(int rankType, KuaFuData<Dictionary<int, List<KFZorkRankInfo>>> ZorkBattleRankInfoDict)
		{
			bool result = true;
			List<KFZorkRankInfo> list = new List<KFZorkRankInfo>();
			if (!ZorkBattleRankInfoDict.V.TryGetValue(rankType, out list))
			{
				list = (ZorkBattleRankInfoDict.V[rankType] = new List<KFZorkRankInfo>());
			}
			else
			{
				list.Clear();
			}
			try
			{
				if (rankType == 0)
				{
					TianTi5v5Service.CalZorkBattleRankTeamJiFen(list);
				}
				else
				{
					result = this.Persistence.LoadZorkBattleRankInfo(rankType, list);
				}
				TimeUtil.AgeByNow(ref ZorkBattleRankInfoDict.Age);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return result;
		}

		private void InitFuBenManagerData(DateTime now)
		{
			this.LastUpdateTime = now;
			this.FuBenDataDict.Clear();
			this.ZhanDuiIDVsGameIDDict.Clear();
			this.BybZhanDuiIDSet.Clear();
			this.StateMachine = new Zork5v5StateMachine();
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.RankAnalyse, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), new Action<DateTime, int>(this.MS_RankAnalyse_Update), null));
			this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.Init, TimeUtil.NowDateTime(), 0);
			this.StateMachine.Tick(now, 0);
		}

		private void MS_Init_Update(DateTime now, int param)
		{
			if (this.CheckOpenState(now))
			{
				if (KuaFuServerManager.IsGongNengOpened(114))
				{
					ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
					Zork5v5StateMachine.StateType stateType = Zork5v5StateMachine.StateType.Init;
					for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
					{
						if (now.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days)
						{
							int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
							int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
							for (int j = 0; j < num2; j++)
							{
								int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
								int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
								int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
								if (now.TimeOfDay.TotalSeconds >= (double)num3 && now.TimeOfDay.TotalSeconds < (double)num4)
								{
									stateType = Zork5v5StateMachine.StateType.SignUp;
								}
								else if (now.TimeOfDay.TotalSeconds >= (double)num4 && now.TimeOfDay.TotalSeconds < (double)num5)
								{
									stateType = Zork5v5StateMachine.StateType.GameStart;
								}
							}
						}
					}
					if (this.CurrentSeasonID > 0)
					{
						if (this.CurrentSeasonID != this.ComputeCurrentSeasonID(now, this.CurrentSeasonID))
						{
							stateType = Zork5v5StateMachine.StateType.RankAnalyse;
						}
					}
					else
					{
						this.CurrentSeasonID = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
						this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
						this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
					}
					if (stateType != Zork5v5StateMachine.StateType.Init)
					{
						this.StateMachine.SetCurrState(stateType, now, param);
						LogManager.WriteLog(5, string.Format("Zork::MS_Init_Update To:{0} SeasonID:{1} Round:{2}", stateType, this.CurrentSeasonID, this.CurrentRound), null, true);
					}
				}
			}
		}

		private void MS_SignUp_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType stateType = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days)
				{
					int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
					int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
					for (int j = 0; j < num2; j++)
					{
						int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
						int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
						int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)num4 && now.TimeOfDay.TotalSeconds >= (double)num4)
						{
							stateType = Zork5v5StateMachine.StateType.PrepareGame;
						}
					}
				}
			}
			if (stateType == Zork5v5StateMachine.StateType.PrepareGame)
			{
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_SignUp_Update To:{0} SeasonID:{1} Round:{2}", stateType, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		private void MS_PrepareGame_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			List<KF5v5PiPeiTeam> list = this.PiPeiDict.Values.ToList<KF5v5PiPeiTeam>();
			if (list.Count < zorkBattleSceneInfo.MaxEnterNum - 1)
			{
				this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_PrepareGame_Update To:{0} SeasonID:{1} Round:{2}", Zork5v5StateMachine.StateType.NotifyEnter, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
			else
			{
				List<KF5v5PiPeiTeam> list2 = new List<KF5v5PiPeiTeam>(list);
				if (list2.Count > 0)
				{
					Random random = new Random((int)now.Ticks);
					int i = 0;
					while (list2.Count > 0 && i < list2.Count * 2)
					{
						int index = random.Next(0, list2.Count);
						int index2 = random.Next(0, list2.Count);
						KF5v5PiPeiTeam value = list2[index];
						list2[index] = list2[index2];
						list2[index2] = value;
						i++;
					}
				}
				int num = 0;
				for (int i = 0; i < list2.Count / zorkBattleSceneInfo.MaxEnterNum; i++)
				{
					int num2 = 0;
					int side = 0;
					KuaFu5v5FuBenData kuaFu5v5FuBenData = new KuaFu5v5FuBenData();
					for (int j = num; j < num + zorkBattleSceneInfo.MaxEnterNum; j++)
					{
						KF5v5PiPeiTeam kf5v5PiPeiTeam = list2[j];
						if (kuaFu5v5FuBenData.AddZhanDuiWithName(kf5v5PiPeiTeam.TeamID, kf5v5PiPeiTeam.ZhanDuiName, ref num2, ref side))
						{
							TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(kf5v5PiPeiTeam.TeamID);
							if (null != zhanDuiData)
							{
								foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
								{
									KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
									{
										ServerId = kf5v5PiPeiTeam.ServerID,
										RoleId = tianTi5v5ZhanDuiRoleData.RoleID,
										Side = side
									};
									kuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, kf5v5PiPeiTeam.TeamID);
								}
							}
						}
					}
					num += zorkBattleSceneInfo.MaxEnterNum;
					this.AssginKfFuben(kuaFu5v5FuBenData);
				}
				if (num > 0)
				{
					list2.RemoveRange(0, num);
				}
				if (list2.Count >= zorkBattleSceneInfo.MaxEnterNum - 1)
				{
					int num2 = 0;
					int side = 0;
					KuaFu5v5FuBenData kuaFu5v5FuBenData = new KuaFu5v5FuBenData();
					for (int i = 0; i < list2.Count; i++)
					{
						KF5v5PiPeiTeam kf5v5PiPeiTeam = list2[i];
						if (kuaFu5v5FuBenData.AddZhanDuiWithName(kf5v5PiPeiTeam.TeamID, kf5v5PiPeiTeam.ZhanDuiName, ref num2, ref side))
						{
							TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(kf5v5PiPeiTeam.TeamID);
							if (null != zhanDuiData)
							{
								foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
								{
									KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
									{
										ServerId = kf5v5PiPeiTeam.ServerID,
										RoleId = tianTi5v5ZhanDuiRoleData.RoleID,
										Side = side
									};
									kuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, kf5v5PiPeiTeam.TeamID);
								}
							}
						}
					}
					this.AssginKfFuben(kuaFu5v5FuBenData);
				}
				else if (list2.Count > 0)
				{
					foreach (KF5v5PiPeiTeam kf5v5PiPeiTeam2 in list2)
					{
						this.BybZhanDuiIDSet.Add(kf5v5PiPeiTeam2.TeamID);
					}
					string arg = string.Join<int>("|", this.BybZhanDuiIDSet.ToArray<int>());
					LogManager.WriteLog(5, string.Format("Zork::轮空 SeasonID:{0} Round:{1} zhanduiId:{2} ", this.CurrentSeasonID, this.CurrentRound, arg), null, true);
				}
				this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_PrepareGame_Update To:{0} SeasonID:{1} Round:{2}", Zork5v5StateMachine.StateType.NotifyEnter, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		private void AssginKfFuben(KuaFu5v5FuBenData fubenData)
		{
			int num = 0;
			int num2 = 0;
			num2 = TianTiPersistence.Instance.GetNextGameId();
			int roleNum = fubenData.ZhanDuiDict.Count * 5;
			if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)num2, roleNum, out num))
			{
				fubenData.ServerId = num;
				fubenData.GameId = num2;
				fubenData.GameType = this.GameType;
				fubenData.LoginInfo = KuaFuServerManager.GetKuaFuLoginInfo(0, num);
				this.FuBenDataDict[num2] = fubenData;
				foreach (int key in fubenData.ZhanDuiDict.Keys)
				{
					this.ZhanDuiIDVsGameIDDict[key] = num2;
					KF5v5PiPeiTeam kf5v5PiPeiTeam;
					if (this.PiPeiDict.TryGetValue(key, out kf5v5PiPeiTeam))
					{
						kf5v5PiPeiTeam.GameId = fubenData.GameId;
					}
				}
				string text = string.Join<int>("|", fubenData.ZhanDuiDict.Keys.ToArray<int>());
				LogManager.WriteLog(5, string.Format("Zork::分组 SeasonID:{0} gameId:{1} zhanduiId:{2} Round:{3}", new object[]
				{
					this.CurrentSeasonID,
					num2,
					text,
					this.CurrentRound
				}), null, true);
			}
			else
			{
				string text = string.Join<int>("|", fubenData.ZhanDuiDict.Keys.ToArray<int>());
				LogManager.WriteLog(5, string.Format("Zork::分配游戏服务器失败 SeasonID:{0} gameId:{1} zhanduiId:{2} Round:{3}", new object[]
				{
					this.CurrentSeasonID,
					num2,
					text,
					this.CurrentRound
				}), null, true);
			}
		}

		private void MS_NotifyEnter_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType stateType = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days)
				{
					int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
					int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
					for (int j = 0; j < num2; j++)
					{
						int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
						int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
						int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
						if ((double)num4 <= now.TimeOfDay.TotalSeconds && now.TimeOfDay.TotalSeconds < (double)num5)
						{
							stateType = Zork5v5StateMachine.StateType.GameStart;
						}
					}
				}
			}
			if (stateType == Zork5v5StateMachine.StateType.GameStart)
			{
				foreach (KuaFu5v5FuBenData kuaFu5v5FuBenData in this.FuBenDataDict.Values)
				{
					KuaFu5v5FuBenData kuaFu5v5FuBenData2 = kuaFu5v5FuBenData;
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(10036, new object[]
					{
						kuaFu5v5FuBenData2
					}), 0);
				}
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_NotifyEnter_Update To:{0} SeasonID:{1} Round:{2}", stateType, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		private void MS_GameStart_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType stateType = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days)
				{
					int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
					int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
					for (int j = 0; j < num2; j++)
					{
						int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
						int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
						int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)num5 && (double)num5 <= now.TimeOfDay.TotalSeconds)
						{
							stateType = Zork5v5StateMachine.StateType.RankAnalyse;
						}
					}
				}
			}
			if (stateType == Zork5v5StateMachine.StateType.RankAnalyse)
			{
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_GameStart_Update To:{0} SeasonID:{1} Round:{2}", stateType, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		private void MS_RankAnalyse_Enter(DateTime now, int param)
		{
			this.PiPeiDict.Clear();
			this.BybZhanDuiIDSet.Clear();
		}

		private void MS_RankAnalyse_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType stateType = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days)
				{
					int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
					int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
					for (int j = 0; j < num2; j++)
					{
						int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
						int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
						int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
						int num6 = num5 + zorkBattleSceneInfo.BattleSignSecs / 2;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)num6 && (double)num6 <= now.TimeOfDay.TotalSeconds)
						{
							if (now.TimeOfDay.TotalSeconds > zorkBattleSceneInfo.SecondsOfDay[i + 1])
							{
								stateType = Zork5v5StateMachine.StateType.Init;
							}
							else
							{
								stateType = Zork5v5StateMachine.StateType.SignUp;
							}
						}
					}
				}
			}
			int num7 = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
			if (this.CurrentSeasonID != num7)
			{
				stateType = Zork5v5StateMachine.StateType.Init;
			}
			if (Zork5v5StateMachine.StateType.None != stateType)
			{
				this.HandleUnCompleteFuBenData();
				if (this.CurrentSeasonID != num7)
				{
					this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
					this.CurrentSeasonID = num7;
					TianTi5v5Service.ClearAllZhanDuiZorkData();
					this.ZorkBattleRankInfoDict.V.Clear();
					TimeUtil.AgeByNow(ref this.ZorkBattleRankInfoDict.Age);
				}
				else
				{
					this.ReloadRankInfo(0, this.ZorkBattleRankInfoDict);
					this.ReloadRankInfo(1, this.ZorkBattleRankInfoDict);
				}
				this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
				ZorkBattleSceneInfo zorkBattleSceneInfo2 = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (this.CurrentRound > zorkBattleSceneInfo2.SeasonFightRound)
				{
					List<KFZorkRankInfo> list;
					if (this.ZorkBattleRankInfoDict.V.TryGetValue(0, out list) && list.Count != 0)
					{
						this.TopZhanDui = list[0].Key;
					}
					else
					{
						this.TopZhanDui = 0;
					}
					this.Persistence.SaveZorkTopZhanDui(this.TopZhanDui);
					this.UpdateTopZhanDuiInfo();
					if (this.ZorkBattleRankInfoDict.V.TryGetValue(1, out list) && list.Count != 0)
					{
						this.TopKiller = list[0].Key;
					}
					else
					{
						this.TopKiller = 0;
					}
					this.Persistence.SaveZorkTopKiller(this.TopKiller);
				}
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("Zork::MS_RankAnalyse_Update To:{0} SeasonID:{1} Round:{2}", stateType, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		private void HandleUnCompleteFuBenData()
		{
			foreach (KeyValuePair<int, KuaFu5v5FuBenData> keyValuePair in this.FuBenDataDict)
			{
				KuaFu5v5FuBenData value = keyValuePair.Value;
				ClientAgentManager.Instance().RemoveKfFuben(this.GameType, value.ServerId, (long)value.GameId);
			}
			this.FuBenDataDict.Clear();
			this.ZhanDuiIDVsGameIDDict.Clear();
		}

		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int gameid)
		{
			KuaFu5v5FuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					this.FuBenDataDict.TryGetValue(gameid, out result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.GetFuBenDataByGameId_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			KuaFu5v5FuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					int key = 0;
					this.ZhanDuiIDVsGameIDDict.TryGetValue(ZhanDuiId, out key);
					this.FuBenDataDict.TryGetValue(key, out result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.GetFuBenDataByZhanDuiId_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			ZorkBattleSyncData zorkBattleSyncData = new ZorkBattleSyncData();
			try
			{
				lock (this.Mutex)
				{
					zorkBattleSyncData.CurSeasonID = this.CurrentSeasonID;
					zorkBattleSyncData.CurRound = this.CurrentRound;
					zorkBattleSyncData.TopZhanDui = this.TopZhanDui;
					zorkBattleSyncData.TopZhanDuiName = this.TopZhanDuiName;
					zorkBattleSyncData.TopKiller = this.TopKiller;
					zorkBattleSyncData.DiffKFCenterSeconds = (int)(gsTicks - TimeUtil.NOW()) / 1000;
					zorkBattleSyncData.ZorkBattleRankInfoDict.Age = this.ZorkBattleRankInfoDict.Age;
					if (ageRank != this.ZorkBattleRankInfoDict.Age)
					{
						zorkBattleSyncData.ZorkBattleRankInfoDict = this.ZorkBattleRankInfoDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.SyncData_ZorkBattle failed!", ex, true);
			}
			return zorkBattleSyncData;
		}

		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			string result = "";
			try
			{
				lock (this.Mutex)
				{
					int num = -4034;
					if (!this.PiPeiDict.ContainsKey(zhanduiID))
					{
						num = -4035;
					}
					else if (this.StateMachine.GetCurrState() == Zork5v5StateMachine.StateType.NotifyEnter || this.StateMachine.GetCurrState() == Zork5v5StateMachine.StateType.GameStart)
					{
						if (this.BybZhanDuiIDSet.Contains(zhanduiID))
						{
							num = -4036;
						}
						else if (!this.ZhanDuiIDVsGameIDDict.ContainsKey(zhanduiID))
						{
							num = -4006;
						}
					}
					result = string.Format("{0}", num);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.GetKuaFuGameState_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					DateTime now = TimeUtil.NowDateTime();
					if (!this.CheckOpenState(now))
					{
						result = -11004;
						return result;
					}
					if (this.StateMachine.GetCurrState() != Zork5v5StateMachine.StateType.SignUp && this.StateMachine.GetCurrState() != Zork5v5StateMachine.StateType.RankAnalyse)
					{
						result = -2001;
						return result;
					}
					TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(zhanduiID);
					if (null == zhanDuiData)
					{
						result = -5;
						return result;
					}
					if (this.PiPeiDict.ContainsKey(zhanduiID))
					{
						result = -5;
						return result;
					}
					KF5v5PiPeiTeam value = new KF5v5PiPeiTeam
					{
						TeamID = zhanduiID,
						ServerID = serverID,
						GroupIndex = this.CalDuanWeiByJiFen(zhanDuiData.ZorkJiFen),
						ZhanDouLi = zhanDuiData.ZhanDouLi,
						ZorkJiFen = zhanDuiData.ZorkJiFen,
						ZhanDuiName = KuaFuServerManager.FormatName(zhanDuiData.ZoneID, zhanDuiData.ZhanDuiName)
					};
					this.PiPeiDict[zhanduiID] = value;
					LogManager.WriteLog(5, string.Format("Zork::比赛报名 SeasonID:{0} Round:{1} ZhanDuiID:{2}", this.CurrentSeasonID, this.CurrentRound, zhanduiID), null, true);
				}
				return result;
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(2, "Zork5v5Service.SignUp_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					KuaFu5v5FuBenData kuaFu5v5FuBenData;
					if (!this.FuBenDataDict.TryGetValue(data.GameId, out kuaFu5v5FuBenData))
					{
						result = -4000;
						return result;
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, kuaFu5v5FuBenData.ServerId, (long)data.GameId);
					this.FuBenDataDict.Remove(data.GameId);
					foreach (KeyValuePair<int, int> keyValuePair in kuaFu5v5FuBenData.ZhanDuiDict)
					{
						this.ZhanDuiIDVsGameIDDict.Remove(keyValuePair.Key);
					}
					foreach (KeyValuePair<int, TianTi5v5ZhanDuiData> keyValuePair2 in data.ZhanDuiDict)
					{
						TianTi5v5ZhanDuiData value = keyValuePair2.Value;
						TianTi5v5Service.UpdateZorkZhanDuiData(value);
					}
					foreach (ZorkBattleRoleInfo roleData in data.ClientContextDataList)
					{
						this.Persistence.UpdateZorkBattleRoleData(roleData, true);
					}
					string text = string.Join<int>("|", data.ZhanDuiDict.Keys.ToArray<int>());
					string text2 = string.Format("Zork::GameFuBenComplete_ZorkBattle SeasonID:{0} GameID:{1} ZhanDuiIDWin:{2} ZhanDuiID:{3} Round:{2} ZhanDuiInfo:", new object[]
					{
						this.CurrentSeasonID,
						data.GameId,
						data.ZhanDuiIDWin,
						text,
						this.CurrentRound
					});
					foreach (KeyValuePair<int, TianTi5v5ZhanDuiData> keyValuePair2 in data.ZhanDuiDict)
					{
						TianTi5v5ZhanDuiData value = keyValuePair2.Value;
						text2 += string.Format(" [ZhanDuiID:{0} JiFen:{1}]", keyValuePair2.Key, value.ZorkJiFen);
					}
					LogManager.WriteLog(5, text2, null, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(2, "Zork5v5Service.GameFuBenComplete_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		public int CalDuanWeiByJiFen(int jifen)
		{
			int num = 0;
			lock (this.MutexConfig)
			{
				foreach (ZorkBattleAwardConfig zorkBattleAwardConfig in this.ZorkLevelRangeList)
				{
					if ((zorkBattleAwardConfig.RankValue < 0 || jifen >= zorkBattleAwardConfig.RankValue) && zorkBattleAwardConfig.ID > num)
					{
						num = zorkBattleAwardConfig.ID;
					}
				}
			}
			return num;
		}

		public void Update(DateTime now)
		{
			try
			{
				if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
				{
					this.UpdateFrameCount += 1U;
					lock (this.Mutex)
					{
						this.StateMachine.Tick(now, 0);
					}
					this.LastUpdateTime = now;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "Zork5v5Service.Update failed!", ex, true);
			}
		}

		public const int RankShowNum = 30;

		private static Zork5v5Service _instance = new Zork5v5Service();

		public readonly GameTypes GameType = 36;

		public readonly GameTypes EvItemGameType = 2;

		private object Mutex = new object();

		private object MutexConfig = new object();

		public Dictionary<int, ZorkBattleSceneInfo> SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();

		public List<ZorkBattleAwardConfig> ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();

		public DateTime ZorkStartTime;

		public TianTiPersistence Persistence = TianTiPersistence.Instance;

		private Zork5v5StateMachine StateMachine = new Zork5v5StateMachine();

		public Dictionary<int, KF5v5PiPeiTeam> PiPeiDict = new Dictionary<int, KF5v5PiPeiTeam>();

		private Dictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new Dictionary<int, KuaFu5v5FuBenData>();

		private Dictionary<int, int> ZhanDuiIDVsGameIDDict = new Dictionary<int, int>();

		private HashSet<int> BybZhanDuiIDSet = new HashSet<int>();

		private KuaFuData<Dictionary<int, List<KFZorkRankInfo>>> ZorkBattleRankInfoDict = new KuaFuData<Dictionary<int, List<KFZorkRankInfo>>>();

		private int CurrentSeasonID = 0;

		private int CurrentRound = 0;

		private int TopZhanDui = 0;

		private string TopZhanDuiName;

		private int TopKiller = 0;

		private int SeasonWeeks = 1;

		private uint UpdateFrameCount = 0U;

		private DateTime LastUpdateTime = DateTime.MinValue;
	}
}
