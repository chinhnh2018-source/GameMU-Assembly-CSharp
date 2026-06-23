using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB;
using GameDBServer.DB.DBController;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;
using Server.Data;

namespace GameDBServer.Logic
{
	public class JingJiChangManager : JingJiChangConstants, IManager
	{
		private JingJiChangManager()
		{
		}

		public static JingJiChangManager getInstance()
		{
			return JingJiChangManager.instance;
		}

		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		private void initCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10140, JingJiGetDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10141, JingJiGetChallengeDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10142, JingJiCreateDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10143, JingJiRequestChallengeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10144, JingJiChallengeEndCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10145, JingJiSaveDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10146, JingJiGetChallengeInfoDataCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10147, JingJiRemoveCDCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10148, JingJiGetRankingAndRewardTimeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10149, JingJiUpdateNextRewardTimeCmdProcessor.getInstance());
		}

		private void initData()
		{
			List<PlayerJingJiData> playerJingJiDataList = JingJiChangDBController.getInstance().getPlayerJingJiDataList();
			if (null != playerJingJiDataList)
			{
				foreach (PlayerJingJiData playerJingJiData in playerJingJiDataList)
				{
					playerJingJiData.convertObject();
					this.playerJingJiDatas.Add(playerJingJiData.roleId, playerJingJiData);
					this.rankingDatas.Add(playerJingJiData.getPlayerJingJiRankingData());
				}
				for (int i = 0; i < this.rankingDatas.Count; i++)
				{
					if (this.rankingDatas[i].ranking != i + 1)
					{
						this.rankingDatas[i].ranking = i + 1;
						JingJiChangDBController.getInstance().updateJingJiRanking(this.rankingDatas[i].roleId, this.rankingDatas[i].ranking);
					}
				}
			}
		}

		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, JingJiChangPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, JingJiChangPlayerLogoutEventListener.getInstnace());
		}

		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, JingJiChangPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, JingJiChangPlayerLogoutEventListener.getInstnace());
		}

		private void removeData()
		{
			if (null != this.playerJingJiDatas)
			{
				this.playerJingJiDatas.Clear();
			}
			this.playerJingJiDatas = null;
			if (null != this.rankingDatas)
			{
				this.rankingDatas.Clear();
			}
			this.rankingDatas = null;
			if (null != this.lockPlayerJingJiDatas)
			{
				this.lockPlayerJingJiDatas.Clear();
			}
			this.lockPlayerJingJiDatas = null;
			if (null != this.challengeInfos)
			{
				this.challengeInfos.Clear();
			}
			this.challengeInfos = null;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			this.removeListener();
			this.removeData();
			return true;
		}

		public bool createRobotData(PlayerJingJiData data)
		{
			lock (this.changeRankingLock)
			{
				if (this.playerJingJiDatas.ContainsKey(data.roleId))
				{
					return false;
				}
				data.isOnline = true;
				this.playerJingJiDatas.Add(data.roleId, data);
				this.challengeInfos.Add(data.roleId, new List<JingJiChallengeInfoData>());
				if (this.rankingDatas.Count >= JingJiChangConstants.RankingList_Max_Num)
				{
					data.ranking = -1;
				}
				else
				{
					data.ranking = this.rankingDatas.Count + 1;
					this.rankingDatas.Add(data.getPlayerJingJiRankingData());
				}
			}
			return JingJiChangDBController.getInstance().insertJingJiData(data);
		}

		public void onPlayerLogin(int roleId)
		{
			PlayerJingJiData playerJingJiData = null;
			lock (this.changeRankingLock)
			{
				if (this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData))
				{
					playerJingJiData.isOnline = true;
				}
				else
				{
					playerJingJiData = JingJiChangDBController.getInstance().getPlayerJingJiDataById(roleId);
					if (null != playerJingJiData)
					{
						playerJingJiData.convertObject();
						playerJingJiData.isOnline = true;
						this.playerJingJiDatas.Add(playerJingJiData.roleId, playerJingJiData);
					}
				}
				if (null != playerJingJiData)
				{
					List<JingJiChallengeInfoData> list = null;
					if (!this.challengeInfos.TryGetValue(roleId, out list))
					{
						list = JingJiChangZhaoBaoDBController.getInstnace().getChallengeInfoListByRoleId(roleId);
						if (null == list)
						{
							list = new List<JingJiChallengeInfoData>();
						}
						this.challengeInfos.Add(roleId, list);
					}
				}
			}
		}

		public void onPlayerLogout(int roleId)
		{
			PlayerJingJiData playerJingJiData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData);
				if (null != playerJingJiData)
				{
					playerJingJiData.isOnline = false;
					if (playerJingJiData.ranking == -1 && !this.lockPlayerJingJiDatas.ContainsKey(playerJingJiData.roleId))
					{
						this.playerJingJiDatas.Remove(playerJingJiData.roleId);
					}
				}
				this.challengeInfos.Remove(roleId);
			}
		}

		public void getRankingAndNextRewardTimeById(int roleId, out int ranking, out long nextRewardTime)
		{
			ranking = -2;
			nextRewardTime = 0L;
			PlayerJingJiData playerJingJiDataById = this.getPlayerJingJiDataById(roleId);
			if (null != playerJingJiDataById)
			{
				ranking = playerJingJiDataById.ranking;
				nextRewardTime = playerJingJiDataById.nextRewardTime;
			}
		}

		public bool updateNextRewardTime(int roleId, long nextRewardTime)
		{
			PlayerJingJiData playerJingJiData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData);
			}
			bool result;
			if (null != playerJingJiData)
			{
				playerJingJiData.nextRewardTime = nextRewardTime;
				result = JingJiChangDBController.getInstance().updateNextRewardTime(roleId, nextRewardTime);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public PlayerJingJiData getPlayerJingJiDataById(int roleId)
		{
			PlayerJingJiData playerJingJiData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData);
			}
			if (playerJingJiData != null)
			{
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleId);
				if (dbroleInfo != null)
				{
					playerJingJiData.AdmiredCount = dbroleInfo.AdmiredCount;
				}
			}
			return playerJingJiData;
		}

		public bool removeCD(int roleId)
		{
			PlayerJingJiData playerJingJiData = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData);
			}
			if (null != playerJingJiData)
			{
				playerJingJiData.nextChallengeTime = 0L;
			}
			return JingJiChangDBController.getInstance().updateNextChallengeTime(roleId, 0L);
		}

		public JingJiBeChallengeData requestChallenge(int challengerId, int beChallengerId, int beChallengerRanking)
		{
			JingJiBeChallengeData jingJiBeChallengeData = new JingJiBeChallengeData();
			PlayerJingJiData playerJingJiData = null;
			JingJiBeChallengeData result;
			lock (this.changeRankingLock)
			{
				if (!this.playerJingJiDatas.TryGetValue(challengerId, out playerJingJiData))
				{
					jingJiBeChallengeData.state = 0;
					result = jingJiBeChallengeData;
				}
				else if (TimeUtil.NOW() < playerJingJiData.nextChallengeTime)
				{
					jingJiBeChallengeData.state = -1;
					result = jingJiBeChallengeData;
				}
				else if ((playerJingJiData.ranking > 100 || playerJingJiData.ranking < 0) && beChallengerRanking <= 3)
				{
					jingJiBeChallengeData.state = -5;
					result = jingJiBeChallengeData;
				}
				else if (beChallengerRanking > this.rankingDatas.Count || beChallengerRanking < 1)
				{
					jingJiBeChallengeData.state = -2;
					result = jingJiBeChallengeData;
				}
				else
				{
					PlayerJingJiRankingData playerJingJiRankingData = this.rankingDatas[beChallengerRanking - 1];
					PlayerJingJiData playerJingJiData2 = null;
					if (!this.playerJingJiDatas.TryGetValue(playerJingJiRankingData.roleId, out playerJingJiData2))
					{
						jingJiBeChallengeData.state = 0;
						result = jingJiBeChallengeData;
					}
					else if (challengerId == playerJingJiRankingData.roleId)
					{
						jingJiBeChallengeData.state = -3;
						result = jingJiBeChallengeData;
					}
					else
					{
						BeChallengerCount beChallengerCount = null;
						this.lockPlayerJingJiDatas.TryGetValue(playerJingJiData2.roleId, out beChallengerCount);
						if (null == beChallengerCount)
						{
							beChallengerCount = new BeChallengerCount();
							beChallengerCount.nBeChallengerCount = 1;
							this.lockPlayerJingJiDatas.Add(playerJingJiData2.roleId, beChallengerCount);
						}
						else
						{
							beChallengerCount.nBeChallengerCount++;
						}
						jingJiBeChallengeData.state = 1;
						jingJiBeChallengeData.beChallengerData = playerJingJiData2;
						result = jingJiBeChallengeData;
					}
				}
			}
			return result;
		}

		public int onChallengeEnd(JingJiChallengeResultData result)
		{
			PlayerJingJiData playerJingJiData = null;
			PlayerJingJiData playerJingJiData2 = null;
			int ranking3;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(result.playerId, out playerJingJiData);
				this.playerJingJiDatas.TryGetValue(result.robotId, out playerJingJiData2);
				BeChallengerCount beChallengerCount = null;
				this.lockPlayerJingJiDatas.TryGetValue(result.robotId, out beChallengerCount);
				if (null != beChallengerCount)
				{
					beChallengerCount.nBeChallengerCount--;
				}
				if (result.isWin)
				{
					int ranking = playerJingJiData.ranking;
					int ranking2 = playerJingJiData2.ranking;
					if (ranking2 < 1 || ranking == ranking2)
					{
						return playerJingJiData.ranking;
					}
					if (ranking == -1)
					{
						playerJingJiData.ranking = ranking2;
						playerJingJiData2.ranking = ranking;
						this.rankingDatas.Remove(playerJingJiData2.getPlayerJingJiRankingData());
						this.rankingDatas.Add(playerJingJiData.getPlayerJingJiRankingData());
						this.rankingDatas.Sort();
						JingJiChangDBController.getInstance().updateJingJiRanking(playerJingJiData.roleId, playerJingJiData.ranking);
						JingJiChangDBController.getInstance().updateJingJiRanking(playerJingJiData2.roleId, playerJingJiData2.ranking);
					}
					else if (ranking > ranking2)
					{
						playerJingJiData.ranking = ranking2;
						playerJingJiData2.ranking = ranking;
						playerJingJiData2.getPlayerJingJiRankingData();
						playerJingJiData.getPlayerJingJiRankingData();
						this.rankingDatas.Sort();
						JingJiChangDBController.getInstance().updateJingJiRanking(playerJingJiData.roleId, playerJingJiData.ranking);
						JingJiChangDBController.getInstance().updateJingJiRanking(playerJingJiData2.roleId, playerJingJiData2.ranking);
					}
				}
				ranking3 = playerJingJiData.ranking;
			}
			return ranking3;
		}

		private void createChallengeWinChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = beChallengePlayer.roleName;
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Challenge_Win;
			playerZhanBaoData.value = challengePlayer.ranking;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			robotZhanBaoData = new JingJiChallengeInfoData();
			robotZhanBaoData.roleId = beChallengePlayer.roleId;
			robotZhanBaoData.challengeName = challengePlayer.roleName;
			robotZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Be_Challenge_Failed;
			robotZhanBaoData.value = beChallengePlayer.ranking;
			robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		private void createChallengeFailedChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = beChallengePlayer.roleName;
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Challenge_Failed;
			playerZhanBaoData.value = challengePlayer.ranking;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			robotZhanBaoData = new JingJiChallengeInfoData();
			robotZhanBaoData.roleId = beChallengePlayer.roleId;
			robotZhanBaoData.challengeName = challengePlayer.roleName;
			robotZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_Be_Challenge_Win;
			robotZhanBaoData.value = beChallengePlayer.ranking;
			robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		private void createLianShengChallengeInfo(PlayerJingJiData challengePlayer, out JingJiChallengeInfoData playerZhanBaoData)
		{
			playerZhanBaoData = new JingJiChallengeInfoData();
			playerZhanBaoData.roleId = challengePlayer.roleId;
			playerZhanBaoData.challengeName = "";
			playerZhanBaoData.zhanbaoType = JingJiChangConstants.ChallengeInfoType_LianSheng;
			playerZhanBaoData.value = challengePlayer.winCount;
			playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public void saveData(JingJiSaveData data, out int winCount)
		{
			winCount = 0;
			PlayerJingJiData playerJingJiData = null;
			PlayerJingJiData playerJingJiData2 = null;
			lock (this.changeRankingLock)
			{
				this.playerJingJiDatas.TryGetValue(data.roleId, out playerJingJiData);
				this.playerJingJiDatas.TryGetValue(data.robotId, out playerJingJiData2);
				if (data.isWin)
				{
					playerJingJiData.level = data.level;
					playerJingJiData.changeLiveCount = data.changeLiveCount;
					playerJingJiData.nextChallengeTime = data.nextChallengeTime;
					playerJingJiData.baseProps = data.baseProps;
					playerJingJiData.extProps = data.extProps;
					playerJingJiData.equipDatas = data.equipDatas;
					playerJingJiData.skillDatas = data.skillDatas;
					playerJingJiData.combatForce = data.combatForce;
					playerJingJiData.wingData = data.wingData;
					playerJingJiData.settingFlags = data.settingFlags;
					playerJingJiData.occupationId = data.Occupation;
					playerJingJiData.SubOccupation = data.SubOccupation;
					playerJingJiData.winCount++;
					playerJingJiData.shenShiEquipData = data.ShenShiEuipSkill;
					if (playerJingJiData.winCount > playerJingJiData.MaxWinCnt)
					{
						playerJingJiData.MaxWinCnt = playerJingJiData.winCount;
						JingJiChangDBController.getInstance().updateJingJiMaxWinCount(playerJingJiData.roleId, playerJingJiData.MaxWinCnt);
					}
					playerJingJiData.PassiveEffectList = data.PassiveEffectList;
					playerJingJiData.convertString();
					JingJiChangDBController.getInstance().updateJingJiDataForWin(playerJingJiData);
					JingJiChallengeInfoData jingJiChallengeInfoData;
					JingJiChallengeInfoData jingJiChallengeInfoData2;
					this.createChallengeWinChallengeInfoData(playerJingJiData, playerJingJiData2, out jingJiChallengeInfoData, out jingJiChallengeInfoData2);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(jingJiChallengeInfoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(jingJiChallengeInfoData2);
					JingJiChallengeInfoData jingJiChallengeInfoData3 = null;
					if (playerJingJiData.winCount >= 10 && playerJingJiData.winCount % 10 == 0)
					{
						winCount = playerJingJiData.winCount;
						this.createLianShengChallengeInfo(playerJingJiData, out jingJiChallengeInfoData3);
						JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(jingJiChallengeInfoData3);
					}
					List<JingJiChallengeInfoData> list = null;
					this.challengeInfos.TryGetValue(playerJingJiData.roleId, out list);
					if (null != jingJiChallengeInfoData3)
					{
						list.Insert(0, jingJiChallengeInfoData3);
						if (list.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							list.RemoveAt(list.Count - 1);
						}
					}
					list.Insert(0, jingJiChallengeInfoData);
					if (list.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
					{
						list.RemoveAt(list.Count - 1);
					}
					if (playerJingJiData2.isOnline)
					{
						List<JingJiChallengeInfoData> list2 = null;
						this.challengeInfos.TryGetValue(playerJingJiData2.roleId, out list2);
						list2.Insert(0, jingJiChallengeInfoData2);
						if (list2.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							list2.RemoveAt(list2.Count - 1);
						}
					}
					if (playerJingJiData2.winCount > 0)
					{
						playerJingJiData2.winCount = 0;
						JingJiChangDBController.getInstance().updateJingJiWinCount(playerJingJiData2.roleId, playerJingJiData2.winCount);
					}
				}
				else
				{
					if (playerJingJiData.winCount >= 10)
					{
						winCount = playerJingJiData.winCount;
					}
					playerJingJiData.winCount = 0;
					playerJingJiData.nextChallengeTime = data.nextChallengeTime;
					JingJiChangDBController.getInstance().updateJingJiDataForFailed(playerJingJiData.roleId, playerJingJiData.nextChallengeTime);
					playerJingJiData2.winCount++;
					if (playerJingJiData2.winCount > playerJingJiData2.MaxWinCnt)
					{
						playerJingJiData2.MaxWinCnt = playerJingJiData2.winCount;
						JingJiChangDBController.getInstance().updateJingJiMaxWinCount(playerJingJiData2.roleId, playerJingJiData2.MaxWinCnt);
					}
					JingJiChangDBController.getInstance().updateJingJiWinCount(playerJingJiData2.roleId, playerJingJiData2.winCount);
					JingJiChallengeInfoData jingJiChallengeInfoData;
					JingJiChallengeInfoData jingJiChallengeInfoData2;
					this.createChallengeFailedChallengeInfoData(playerJingJiData, playerJingJiData2, out jingJiChallengeInfoData, out jingJiChallengeInfoData2);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(jingJiChallengeInfoData);
					JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(jingJiChallengeInfoData2);
					List<JingJiChallengeInfoData> list = null;
					this.challengeInfos.TryGetValue(playerJingJiData.roleId, out list);
					list.Insert(0, jingJiChallengeInfoData);
					if (list.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
					{
						list.RemoveAt(list.Count - 1);
					}
					if (playerJingJiData2.isOnline)
					{
						List<JingJiChallengeInfoData> list2 = null;
						this.challengeInfos.TryGetValue(playerJingJiData2.roleId, out list2);
						list2.Insert(0, jingJiChallengeInfoData2);
						if (list2.Count > JingJiChangConstants.ChallengeInfo_Max_Num)
						{
							list2.RemoveAt(list2.Count - 1);
						}
					}
				}
				BeChallengerCount beChallengerCount = null;
				int num = 0;
				this.lockPlayerJingJiDatas.TryGetValue(playerJingJiData2.roleId, out beChallengerCount);
				if (null != beChallengerCount)
				{
					num = beChallengerCount.nBeChallengerCount;
					if (num <= 0)
					{
						this.lockPlayerJingJiDatas.Remove(playerJingJiData2.roleId);
					}
				}
				if (playerJingJiData2.ranking == -1 && num <= 0 && !playerJingJiData2.isOnline)
				{
					this.playerJingJiDatas.Remove(playerJingJiData2.roleId);
				}
			}
		}

		public List<PlayerJingJiMiniData> getChallengeData(int[] challengeRankings)
		{
			List<PlayerJingJiMiniData> list = new List<PlayerJingJiMiniData>();
			lock (this.changeRankingLock)
			{
				if (challengeRankings.Length > 1 && challengeRankings[0] < 0)
				{
					int num = Math.Min(this.rankingDatas.Count / 6, -challengeRankings[0]);
					if (num <= 2)
					{
						return list;
					}
					challengeRankings[0] = this.rankingDatas.Count - 1 - num;
					challengeRankings[1] = this.rankingDatas.Count - 1 - num * 2;
					challengeRankings[2] = this.rankingDatas.Count - 1 - num * 3;
				}
				int num2 = 0;
				while (num2++ < 6)
				{
					bool flag2 = false;
					foreach (int num3 in challengeRankings)
					{
						PlayerJingJiData playerJingJiData = null;
						if (num3 <= this.rankingDatas.Count)
						{
							PlayerJingJiRankingData playerJingJiRankingData = this.rankingDatas[num3 - 1];
							if (playerJingJiRankingData.ranking < 0)
							{
								flag2 = true;
								this.rankingDatas.Remove(playerJingJiRankingData);
								break;
							}
						}
					}
					if (!flag2)
					{
						break;
					}
					this.rankingDatas.Sort();
				}
				foreach (int num3 in challengeRankings)
				{
					PlayerJingJiData playerJingJiData = null;
					if (num3 <= this.rankingDatas.Count)
					{
						PlayerJingJiRankingData playerJingJiRankingData = this.rankingDatas[num3 - 1];
						if (this.playerJingJiDatas.TryGetValue(playerJingJiRankingData.roleId, out playerJingJiData))
						{
							list.Add(playerJingJiData.getPlayerJingJiMiniData());
						}
					}
				}
			}
			return list;
		}

		public List<JingJiChallengeInfoData> getChallengeInfoDataList(int roleId, int pageIndex)
		{
			List<JingJiChallengeInfoData> result;
			if (pageIndex >= JingJiChangConstants.ChallengeInfo_Max_Num)
			{
				result = null;
			}
			else
			{
				List<JingJiChallengeInfoData> list = null;
				if (!this.challengeInfos.TryGetValue(roleId, out list))
				{
					result = null;
				}
				else
				{
					int num = pageIndex * JingJiChangConstants.ChallengeInfo_PageShowNum;
					int num2 = JingJiChangConstants.ChallengeInfo_PageShowNum;
					if (num >= list.Count)
					{
						result = null;
					}
					else
					{
						if (num + num2 >= list.Count)
						{
							num2 = list.Count - num;
						}
						if (num2 == 0)
						{
							result = null;
						}
						else
						{
							result = list.GetRange(num, num2);
						}
					}
				}
			}
			return result;
		}

		public List<PaiHangItemData> getRankingList(int pageIndex)
		{
			int num = JingJiChangConstants.RankingList_PageShowNum;
			if (num > this.rankingDatas.Count)
			{
				num = this.rankingDatas.Count;
			}
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			lock (this.changeRankingLock)
			{
				for (int i = 0; i < num; i++)
				{
					list.Add(this.rankingDatas[i].getPaiHangItemData());
				}
			}
			return list;
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				try
				{
					lock (this.changeRankingLock)
					{
						PlayerJingJiData playerJingJiData = null;
						if (!this.playerJingJiDatas.TryGetValue(roleId, out playerJingJiData) || playerJingJiData == null)
						{
							return;
						}
						playerJingJiData.roleName = newName;
						playerJingJiData.name = newName;
					}
				}
				catch (Exception)
				{
				}
				JingJiChangDBController.getInstance().OnChangeName(roleId, oldName, newName);
			}
		}

		private static JingJiChangManager instance = new JingJiChangManager();

		private List<PlayerJingJiRankingData> rankingDatas = new List<PlayerJingJiRankingData>();

		private Dictionary<int, PlayerJingJiData> playerJingJiDatas = new Dictionary<int, PlayerJingJiData>();

		private Dictionary<int, BeChallengerCount> lockPlayerJingJiDatas = new Dictionary<int, BeChallengerCount>();

		private object changeRankingLock = new object();

		private Dictionary<int, List<JingJiChallengeInfoData>> challengeInfos = new Dictionary<int, List<JingJiChallengeInfoData>>();
	}
}
