using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic.Copy;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.WanMota;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class CopyMapManager
	{
		public int GetNewCopyMapID()
		{
			int result = 1;
			lock (this)
			{
				result = this.BaseCopyMapID++;
			}
			return result;
		}

		private void AddCopyID(int fuBenSeqID, int mapCode, int copyMapID)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				this.FuBenSeqID2CopyIDDict[key] = copyMapID;
			}
		}

		public void RemoveCopyID(int fuBenSeqID, int mapCode)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				this.FuBenSeqID2CopyIDDict.Remove(key);
			}
		}

		public int FindCopyID(int fuBenSeqID, int mapCode)
		{
			int result = -1;
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2CopyIDDict)
			{
				if (!this.FuBenSeqID2CopyIDDict.TryGetValue(key, out result))
				{
					result = -1;
				}
			}
			return result;
		}

		private void AddMonsterState(int fuBenSeqID, int mapCode, int monsterState)
		{
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2MonsterStateDict)
			{
				this.FuBenSeqID2MonsterStateDict[key] = monsterState;
			}
		}

		private int FindMonsterState(int fuBenSeqID, int mapCode)
		{
			int result = 0;
			string key = string.Format("{0}_{1}", fuBenSeqID, mapCode);
			lock (this.FuBenSeqID2MonsterStateDict)
			{
				if (!this.FuBenSeqID2MonsterStateDict.TryGetValue(key, out result))
				{
					result = 0;
				}
			}
			return result;
		}

		public void AddAwardState(int roleID, int fuBenSeqID, int mapCode, int awardState)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, fuBenSeqID, mapCode);
			lock (this.RoleIDFuBenSeqID2AwardStateDict)
			{
				this.RoleIDFuBenSeqID2AwardStateDict[key] = awardState;
			}
		}

		public int FindAwardState(int roleID, int fuBenSeqID, int mapCode)
		{
			int result = 0;
			string key = string.Format("{0}_{1}_{2}", roleID, fuBenSeqID, mapCode);
			lock (this.RoleIDFuBenSeqID2AwardStateDict)
			{
				if (!this.RoleIDFuBenSeqID2AwardStateDict.TryGetValue(key, out result))
				{
					result = 0;
				}
			}
			return result;
		}

		public CopyMap FindCopyMap(int mapCode, int fuBenSeqID)
		{
			CopyMap result = null;
			int num = this.FindCopyID(fuBenSeqID, mapCode);
			if (num > 0)
			{
				result = this.FindCopyMap(num);
			}
			return result;
		}

		public CopyMap GetCopyMap(GameClient client, MapTypes mapType)
		{
			CopyMap copyMap = null;
			int mapTotalMonsterNum = GameManager.MonsterZoneMgr.GetMapTotalMonsterNum(client.ClientData.MapCode, MonsterTypes.None, true);
			int mapTotalMonsterNum2 = GameManager.MonsterZoneMgr.GetMapTotalMonsterNum(client.ClientData.MapCode, MonsterTypes.Noraml, true);
			int totalBossNum = mapTotalMonsterNum - mapTotalMonsterNum2;
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			lock (this)
			{
				int num = this.FindCopyID(client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (num > 0)
				{
					copyMap = this.FindCopyMap(num);
				}
				int num2 = this.FindMonsterState(client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (null == copyMap)
				{
					copyMap = new CopyMap
					{
						CopyMapID = this.GetNewCopyMapID(),
						FuBenSeqID = client.ClientData.FuBenSeqID,
						MapCode = client.ClientData.MapCode,
						FubenMapID = client.ClientData.FuBenID,
						CopyMapType = mapType,
						IsInitMonster = (num2 > 0),
						InitTicks = TimeUtil.NOW(),
						TotalNormalNum = mapTotalMonsterNum2,
						TotalBossNum = totalBossNum,
						bStoryCopyMapFinishStatus = false
					};
					if (copyMap.FubenMapID < 0)
					{
						copyMap.FubenMapID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
						client.ClientData.FuBenID = copyMap.FubenMapID;
					}
					this.AddCopyID(client.ClientData.FuBenSeqID, client.ClientData.MapCode, copyMap.CopyMapID);
					this.AddCopyMap(copyMap);
					this.AddTeamCopyMap(copyMap);
					if (!copyMap.IsInitMonster)
					{
						copyMap.IsInitMonster = true;
						GameManager.MonsterZoneMgr.AddCopyMapMonsters(client.ClientData.MapCode, copyMap.CopyMapID);
					}
				}
				copyMap.AddGameClient(client);
				if (client.ClientData.MapCode == 6090)
				{
					copyMap.FreshPlayerCreateGateFlag = 0;
					FreshPlayerCopySceneManager.AddFreshPlayerListCopyMap(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (Global.IsInExperienceCopyScene(client.ClientData.MapCode))
				{
					ExperienceCopySceneManager.AddExperienceListCopyMap(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (client.ClientData.MapCode == 5100)
				{
					GlodCopySceneManager.AddGlodCopySceneList(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (client.ClientData.MapCode == EMoLaiXiCopySceneManager.EMoLaiXiCopySceneMapCode)
				{
					EMoLaiXiCopySceneManager.AddEMoLaiXiCopySceneList(client.ClientData.FuBenSeqID, copyMap);
				}
				else if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene2(client.ClientData.MapCode))
				{
					GameManager.BloodCastleCopySceneMgr.AddBloodCastleCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene2(client.ClientData.MapCode))
				{
					GameManager.DaimonSquareCopySceneMgr.AddDaimonSquareCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZhuanShengShiLian.AddCopyScenes(copyMap.FuBenSeqID, copyMap.FubenMapID, client.ClientData.MapCode, copyMap);
				}
				else if (Global.IsStoryCopyMapScene(client.ClientData.MapCode))
				{
					SystemXmlItem systemXmlItem = null;
					if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemXmlItem) && systemXmlItem != null)
					{
						int intValue = systemXmlItem.GetIntValue("BossID", -1);
						int mapMonsterNum = GameManager.MonsterZoneMgr.GetMapMonsterNum(client.ClientData.MapCode, intValue);
						if (mapMonsterNum == 0)
						{
							Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 1);
						}
						else
						{
							Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 2);
						}
					}
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
				}
				switch (mapSceneType)
				{
				case 25:
					HuanYingSiYuanManager.getInstance().AddHuanYingSiYuanCopyScenes(client, copyMap);
					goto IL_6B5;
				case 26:
					TianTiManager.getInstance().AddTianTiCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 27:
					YongZheZhanChangManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 28:
					ElementWarManager.getInstance().AddCopyScene(client, copyMap);
					goto IL_6B5;
				case 29:
					SingletonTemplate<MoRiJudgeManager>.Instance().AddCopyScene(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 31:
					KuaFuBossManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 34:
					CopyWolfManager.getInstance().AddCopyScene(client, copyMap);
					goto IL_6B5;
				case 35:
					LangHunLingYuManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 36:
					SingletonTemplate<ZhengBaManager>.Instance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 38:
					SingletonTemplate<CoupleArenaManager>.Instance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 39:
					KingOfBattleManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 41:
					KarenBattleManager_MapWest.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 42:
					KarenBattleManager_MapEast.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 45:
					BangHuiMatchManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 47:
					KuaFuLueDuoManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 52:
					CompBattleManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 53:
					CompMineManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				case 57:
					ZorkBattleManager.getInstance().AddCopyScenes(client, copyMap, mapSceneType);
					goto IL_6B5;
				}
				if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(copyMap.FubenMapID))
				{
					SingletonTemplate<CopyTeamManager>.Instance().AddCopyScenes(client, copyMap, mapSceneType);
				}
				IL_6B5:;
			}
			GlobalServiceManager.AddCopyScenes(client, copyMap, mapSceneType);
			return copyMap;
		}

		private void AddCopyMap(CopyMap copyMap)
		{
			lock (this._ListCopyMaps)
			{
				this._ListCopyMaps.Add(copyMap);
			}
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.Add(copyMap.CopyMapID, copyMap);
			}
		}

		public void RemoveCopyMap(CopyMap copyMap)
		{
			lock (this._ListCopyMaps)
			{
				this._ListCopyMaps.Remove(copyMap);
			}
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.Remove(copyMap.CopyMapID);
			}
		}

		public CopyMap FindCopyMap(int copyMapID)
		{
			CopyMap result = null;
			lock (this._DictCopyMaps)
			{
				this._DictCopyMaps.TryGetValue(copyMapID, out result);
			}
			return result;
		}

		public CopyMap GetNextCopyMap(int index)
		{
			CopyMap result = null;
			lock (this._ListCopyMaps)
			{
				if (index < this._ListCopyMaps.Count)
				{
					result = this._ListCopyMaps[index];
				}
			}
			return result;
		}

		public int GetCopyMapCount()
		{
			int result = 0;
			lock (this._ListCopyMaps)
			{
				result = this._ListCopyMaps.Count;
			}
			return result;
		}

		private bool CopyMapOverTime(CopyMap copyMap, long nowTicks, List<GameClient> clientsList)
		{
			int num = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
			FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num, copyMap.MapCode);
			bool result;
			if (null == fuBenMapItem)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
				}
				if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(num))
				{
					if (clientsList == null || 0 == clientsList.Count)
					{
						if (nowTicks - copyMap.GetLastLeaveClientTicks() >= 600000L)
						{
							return true;
						}
					}
				}
				if (fuBenMapItem.MaxTime <= 0)
				{
					result = false;
				}
				else if (nowTicks - copyMap.InitTicks < (long)fuBenMapItem.MaxTime * 60L * 1000L)
				{
					result = false;
				}
				else
				{
					if (null != clientsList)
					{
						int mainMapCode = GameManager.MainMapCode;
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(mainMapCode, out gameMap))
						{
							for (int i = 0; i < clientsList.Count; i++)
							{
								if (copyMap.MapCode == 6090)
								{
									int cmd = 543;
									string data = string.Format("{0}", clientsList[i].ClientData.RoleID);
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, cmd);
									Global._TCPManager.MySocketListener.SendData(clientsList[i].ClientSocket, tcpOutPacket, true);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, clientsList[i], mainMapCode, -1, -1, -1, 0);
									GameManager.LuaMgr.Error(clientsList[i], GLang.GetLang(99, new object[0]), 0);
								}
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		private bool CanRemoveCopyMap(CopyMap copyMap, long nowTicks)
		{
			bool result;
			if (copyMap.bNeedRemove)
			{
				result = true;
			}
			else
			{
				List<GameClient> clientsList = copyMap.GetClientsList();
				long num = 180000L;
				if (copyMap.IsKuaFuCopy)
				{
					if (copyMap.CanRemoveTicks > 0L)
					{
						return nowTicks > copyMap.CanRemoveTicks;
					}
					num = 30000L;
				}
				if (this.CopyMapOverTime(copyMap, nowTicks, clientsList))
				{
					result = true;
				}
				else if (clientsList != null && clientsList.Count > 0)
				{
					result = false;
				}
				else
				{
					long lastLeaveClientTicks = copyMap.GetLastLeaveClientTicks();
					result = (nowTicks - lastLeaveClientTicks >= num);
				}
			}
			return result;
		}

		public void ProcessRemoveCopyMap(CopyMap copyMap)
		{
			int leftMonsterByCopyMapID = Global.GetLeftMonsterByCopyMapID(copyMap.CopyMapID);
			int monsterState = 0;
			if (copyMap.IsInitMonster)
			{
				monsterState = ((leftMonsterByCopyMapID <= 0) ? 1 : 0);
			}
			this.AddMonsterState(copyMap.FuBenSeqID, copyMap.MapCode, monsterState);
			GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(copyMap.MapCode, copyMap.CopyMapID);
			this.RemoveCopyID(copyMap.FuBenSeqID, copyMap.MapCode);
			this.RemoveCopyMap(copyMap);
			this.RemoveTeamCopyMap(copyMap);
			SceneUIClasses mapSceneType = Global.GetMapSceneType(copyMap.MapCode);
			if (copyMap.MapCode == 6090)
			{
				FreshPlayerCopySceneManager.RemoveFreshPlayerListCopyMap(copyMap.FuBenSeqID, copyMap);
			}
			if (Global.IsInExperienceCopyScene(copyMap.MapCode))
			{
				ExperienceCopySceneManager.RemoveExperienceListCopyMap(copyMap.FuBenSeqID);
			}
			if (copyMap.MapCode == 5100)
			{
				GlodCopySceneManager.RemoveGlodCopySceneList(copyMap.FuBenSeqID);
			}
			else if (copyMap.MapCode == EMoLaiXiCopySceneManager.EMoLaiXiCopySceneMapCode)
			{
				EMoLaiXiCopySceneManager.RemoveEMoLaiXiCopySceneList(copyMap.FuBenSeqID, copyMap.CopyMapID);
			}
			else if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID))
			{
				GameManager.BloodCastleCopySceneMgr.RemoveBloodCastleListCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
			{
				GameManager.DaimonSquareCopySceneMgr.RemoveDaimonSquareListCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(copyMap.MapCode))
			{
				ZhuanShengShiLian.RemoveCopyScenes(copyMap, copyMap.FuBenSeqID, copyMap.FubenMapID);
			}
			switch (mapSceneType)
			{
			case 25:
				HuanYingSiYuanManager.getInstance().RemoveHuanYingSiYuanListCopyScenes(copyMap);
				goto IL_3A0;
			case 26:
				TianTiManager.getInstance().RemoveTianTiCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 27:
				YongZheZhanChangManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 28:
				ElementWarManager.getInstance().RemoveCopyScene(copyMap);
				goto IL_3A0;
			case 29:
				SingletonTemplate<MoRiJudgeManager>.Instance().DelCopyScene(copyMap);
				goto IL_3A0;
			case 31:
				KuaFuBossManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 34:
				CopyWolfManager.getInstance().RemoveCopyScene(copyMap);
				goto IL_3A0;
			case 35:
				LangHunLingYuManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 36:
				SingletonTemplate<ZhengBaManager>.Instance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 38:
				SingletonTemplate<CoupleArenaManager>.Instance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 39:
				KingOfBattleManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 41:
				KarenBattleManager_MapWest.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 42:
				KarenBattleManager_MapEast.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 45:
				BangHuiMatchManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 52:
				CompBattleManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 53:
				CompMineManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			case 57:
				ZorkBattleManager.getInstance().RemoveCopyScene(copyMap, mapSceneType);
				goto IL_3A0;
			}
			if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(copyMap.FubenMapID))
			{
				SingletonTemplate<CopyTeamManager>.Instance().RemoveCopyScene(copyMap, mapSceneType);
			}
			IL_3A0:
			GlobalServiceManager.RemoveCopyScene(copyMap, mapSceneType);
			SingletonTemplate<CopyTeamManager>.Instance().OnCopyRemove(copyMap.FuBenSeqID);
			FuBenManager.RemoveFuBenInfoBySeqID(copyMap.FuBenSeqID);
			copyMap.bNeedRemove = false;
		}

		public void ProcessRemoveCopyMaps(List<CopyMap> listCopyMap, int FuBenSeqID, int FubenMapID)
		{
			foreach (CopyMap copyMap in listCopyMap)
			{
				int leftMonsterByCopyMapID = Global.GetLeftMonsterByCopyMapID(copyMap.CopyMapID);
				int monsterState = 0;
				if (copyMap.IsInitMonster)
				{
					monsterState = ((leftMonsterByCopyMapID <= 0) ? 1 : 0);
				}
				this.AddMonsterState(copyMap.FuBenSeqID, copyMap.MapCode, monsterState);
				GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(copyMap.MapCode, copyMap.CopyMapID);
				this.RemoveCopyID(copyMap.FuBenSeqID, copyMap.MapCode);
				this.RemoveCopyMap(copyMap);
				this.RemoveTeamCopyMap(copyMap);
				SingletonTemplate<CopyTeamManager>.Instance().OnCopyRemove(copyMap.FuBenSeqID);
			}
			if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(FubenMapID))
			{
				LuoLanFaZhenCopySceneManager.OnFubenOver(FuBenSeqID);
			}
			FuBenManager.RemoveFuBenInfoBySeqID(FuBenSeqID);
		}

		public void ProcessEndCopyMap()
		{
			long nowTicks = TimeUtil.NOW();
			int num = 0;
			CopyMap nextCopyMap = this.GetNextCopyMap(num);
			int num2 = 0;
			while (null != nextCopyMap)
			{
				if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(nextCopyMap.FubenMapID))
				{
					List<CopyMap> list = null;
					bool flag = true;
					List<int> list2 = FuBenManager.FindMapCodeListByFuBenID(nextCopyMap.FubenMapID);
					if (null != list2)
					{
						foreach (int mapCode in list2)
						{
							int num3 = this.FindCopyID(nextCopyMap.FuBenSeqID, mapCode);
							if (num3 >= 0)
							{
								CopyMap copyMap = this.FindCopyMap(num3);
								if (null != copyMap)
								{
									if (!this.CanRemoveCopyMap(copyMap, nowTicks))
									{
										flag = false;
										break;
									}
									if (null == list)
									{
										list = new List<CopyMap>();
									}
									list.Add(copyMap);
								}
							}
						}
					}
					if (!flag)
					{
						num++;
						nextCopyMap = this.GetNextCopyMap(num);
						continue;
					}
					if (flag && list != null && list.Count > 0)
					{
						this.ProcessRemoveCopyMaps(list, nextCopyMap.FuBenSeqID, nextCopyMap.FubenMapID);
						break;
					}
				}
				if (this.CanRemoveCopyMap(nextCopyMap, nowTicks))
				{
					this.ProcessRemoveCopyMap(nextCopyMap);
					GuildCopyMap guildCopyMap = GameManager.GuildCopyMapMgr.FindGuildCopyMapBySeqID(nextCopyMap.FuBenSeqID);
					if (null != guildCopyMap)
					{
						GameManager.GuildCopyMapMgr.RemoveGuildCopyMap(guildCopyMap.GuildID);
					}
					num2++;
					if (num2 >= GameManager.OnceDestroyCopyMapNum)
					{
						break;
					}
				}
				else
				{
					num++;
				}
				nextCopyMap = this.GetNextCopyMap(num);
			}
		}

		public void ProcessEndGuildCopyMapFlag()
		{
			if (GameManager.GuildCopyMapMgr.IsPrepareResetTime())
			{
				GameManager.GuildCopyMapMgr.ProcessEndFlag = true;
			}
		}

		public void ProcessEndGuildCopyMap(long ticks)
		{
			if (ticks - GameManager.GuildCopyMapMgr.lastProcessEndTicks >= 1000L)
			{
				GameManager.GuildCopyMapMgr.lastProcessEndTicks = ticks;
				if (!GameManager.GuildCopyMapMgr.IsPrepareResetTime())
				{
					if (GameManager.GuildCopyMapMgr.ProcessEndFlag)
					{
						GuildCopyMap guildCopyMap = GameManager.GuildCopyMapMgr.FindActiveGuildCopyMap();
						if (null == guildCopyMap)
						{
							GameManager.GuildCopyMapMgr.ProcessEndFlag = false;
						}
						else
						{
							GameManager.GuildCopyMapMgr.RemoveGuildCopyMap(guildCopyMap.GuildID);
							this.CloseGuildCopyMap(guildCopyMap.SeqID, guildCopyMap.MapCode);
						}
					}
				}
			}
		}

		public void CloseGuildCopyMap(int fuBenSeqID, int mapCode)
		{
			int num = this.FindCopyID(fuBenSeqID, mapCode);
			if (num > 0)
			{
				CopyMap copyMap = this.FindCopyMap(num);
				if (null != copyMap)
				{
					if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(copyMap.FubenMapID))
					{
						this.RemoveCopyMapAllPlayer(copyMap);
						this.ProcessRemoveCopyMap(copyMap);
					}
				}
			}
		}

		public void RemoveCopyMapAllPlayer(CopyMap copyMap)
		{
			List<GameClient> clientsList = copyMap.GetClientsList();
			if (clientsList != null)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null)
					{
						if (gameClient.ClientData.MapCode == copyMap.MapCode)
						{
							int num = GameManager.MainMapCode;
							int maxX = -1;
							int mapY = -1;
							if (MapTypes.Normal == Global.GetMapType(gameClient.ClientData.LastMapCode))
							{
								if (GameManager.BattleMgr.BattleMapCode != gameClient.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != gameClient.ClientData.LastMapCode)
								{
									num = gameClient.ClientData.LastMapCode;
									maxX = gameClient.ClientData.LastPosX;
									mapY = gameClient.ClientData.LastPosY;
								}
							}
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(num, out gameMap))
							{
								gameClient.ClientData.bIsInAngelTempleMap = false;
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num, maxX, mapY, -1, 0);
							}
						}
					}
				}
			}
		}

		public void AddTeamCopyMap(CopyMap copyMap)
		{
			if (SingletonTemplate<CopyTeamManager>.Instance().NeedRecordDamageInfoFuBenID(copyMap.FubenMapID))
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					if (!this.TeamCopyMapDict.Contains(copyMap))
					{
						this.RoleDamageDict.Add(copyMap.CopyMapID, new Dictionary<int, RoleDamage>());
						this.TeamCopyMapDict.Add(copyMap);
					}
				}
			}
		}

		public void RemoveTeamCopyMap(CopyMap copyMap)
		{
			if (SingletonTemplate<CopyTeamManager>.Instance().NeedRecordDamageInfoFuBenID(copyMap.FubenMapID))
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					this.RoleDamageDict.Remove(copyMap.CopyMapID);
					this.TeamCopyMapDict.Remove(copyMap);
				}
			}
		}

		public List<RoleDamage> GetCopyMapAllRoleDamages(int copyMapID)
		{
			List<RoleDamage> list = null;
			lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
			{
				Dictionary<int, RoleDamage> dictionary;
				if (this.RoleDamageDict.TryGetValue(copyMapID, out dictionary))
				{
					list = new List<RoleDamage>();
					foreach (RoleDamage item in dictionary.Values)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public void BroadcastCopyMapDamageInfo(CopyMap copyMap, int sendtoRoleId = -1)
		{
			lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
			{
				Dictionary<int, RoleDamage> dictionary;
				if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dictionary))
				{
					List<GameClient> clientsList = copyMap.GetClientsList();
					foreach (GameClient gameClient in clientsList)
					{
						long num = Interlocked.Exchange(ref gameClient.SumDamageForCopyTeam, 0L);
						int roleID = gameClient.ClientData.RoleID;
						RoleDamage roleDamage;
						if (dictionary.TryGetValue(roleID, out roleDamage))
						{
							roleDamage.Damage += num;
						}
						else
						{
							dictionary.Add(roleID, new RoleDamage(roleID, num, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), new int[0]));
						}
					}
					List<RoleDamage> copyMapAllRoleDamages = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
					foreach (GameClient gameClient in clientsList)
					{
						if (sendtoRoleId < 0 || sendtoRoleId == gameClient.ClientData.RoleID)
						{
							gameClient.sendCmd<List<RoleDamage>>(626, copyMapAllRoleDamages, false);
						}
					}
				}
			}
		}

		public void SendCopyMapMaxDamageInfo(GameClient client, CopyMap copyMap, int MaxCount)
		{
			if (MaxCount > 0)
			{
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					Dictionary<int, RoleDamage> dictionary;
					if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dictionary))
					{
						List<GameClient> clientsList = copyMap.GetClientsList();
						foreach (GameClient gameClient in clientsList)
						{
							long num = Interlocked.Exchange(ref gameClient.SumDamageForCopyTeam, 0L);
							int roleID = gameClient.ClientData.RoleID;
							RoleDamage roleDamage;
							if (dictionary.TryGetValue(roleID, out roleDamage))
							{
								roleDamage.Damage += num;
							}
							else
							{
								dictionary.Add(roleID, new RoleDamage(roleID, num, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), new int[0]));
							}
						}
						List<RoleDamage> list = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
						IEnumerable<RoleDamage> enumerable = from items in list
						orderby items.Damage descending
						select items;
						List<RoleDamage> list2 = new List<RoleDamage>();
						int num2 = 0;
						foreach (RoleDamage item in enumerable)
						{
							list2.Add(item);
							num2++;
							if (num2 >= GameManager.GuildCopyMapMgr.MaxDamageSendCount)
							{
								break;
							}
						}
						list = list2;
						client.sendCmd<List<RoleDamage>>(626, list, false);
					}
				}
			}
		}

		public void CheckCopyTeamDamage(long ticks, bool force = false)
		{
			if (ticks - this.LastNotifyTeamDamageTicks >= 2000L)
			{
				this.LastNotifyTeamDamageTicks = ticks;
				lock (this._RoleDamageDict_TeamCopyMapDict_Mutex)
				{
					foreach (CopyMap copyMap in this.TeamCopyMapDict)
					{
						Dictionary<int, RoleDamage> dictionary;
						if (this.RoleDamageDict.TryGetValue(copyMap.CopyMapID, out dictionary))
						{
							List<GameClient> clientsList = copyMap.GetClientsList();
							long num = 0L;
							foreach (GameClient gameClient in clientsList)
							{
								long num2 = Interlocked.Exchange(ref gameClient.SumDamageForCopyTeam, 0L);
								if (num2 > 0L)
								{
									int roleID = gameClient.ClientData.RoleID;
									RoleDamage roleDamage;
									if (dictionary.TryGetValue(roleID, out roleDamage))
									{
										roleDamage.Damage += num2;
									}
									else
									{
										dictionary.Add(roleID, new RoleDamage(roleID, num2, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), new int[0]));
									}
									num += num2;
								}
							}
							if (num > 0L || force)
							{
								List<RoleDamage> list = this.GetCopyMapAllRoleDamages(copyMap.CopyMapID);
								if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(copyMap.FubenMapID))
								{
									IEnumerable<RoleDamage> enumerable = from items in list
									orderby items.Damage descending
									select items;
									List<RoleDamage> list2 = new List<RoleDamage>();
									int num3 = 0;
									foreach (RoleDamage item in enumerable)
									{
										list2.Add(item);
										num3++;
										if (num3 >= GameManager.GuildCopyMapMgr.MaxDamageSendCount)
										{
											break;
										}
									}
									list = list2;
								}
								foreach (GameClient gameClient in clientsList)
								{
									gameClient.sendCmd<List<RoleDamage>>(626, list, false);
								}
							}
						}
					}
				}
			}
		}

		private bool IsHeroMapCode(int mapCode)
		{
			if (null == CopyMapManager.HeroMapCodeFileds)
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("HeroMapCodes");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					CopyMapManager.HeroMapCodeFileds = paramValueByName.Split(new char[]
					{
						','
					});
				}
			}
			bool result;
			if (CopyMapManager.HeroMapCodeFileds == null || CopyMapManager.HeroMapCodeFileds.Length <= 0)
			{
				result = false;
			}
			else
			{
				string b = mapCode.ToString();
				for (int i = 0; i < CopyMapManager.HeroMapCodeFileds.Length; i++)
				{
					if (CopyMapManager.HeroMapCodeFileds[i] == b)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public void CopyMapPassAward(GameClient client, CopyMap copyMap, bool anyAlive)
		{
			int num = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
			FuBenTongGuanData fuBenTongGuanData = null;
			if (num > 0)
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(num);
				if (null != fuBenInfoItem)
				{
					fuBenInfoItem.EndTicks = TimeUtil.NOW();
					int addNum = 1;
					if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
					{
						addNum = 0;
					}
					int num2 = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (num2 > 0)
					{
						int num3 = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
						int nLev = -1;
						string strName = null;
						SystemXmlItem systemXmlItem = null;
						if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(num2, out systemXmlItem))
						{
							nLev = systemXmlItem.GetIntValue("FuBenLevel", -1);
							strName = systemXmlItem.GetStringValue("CopyName");
						}
						Global.UpdateFuBenDataForQuickPassTimer(client, num2, num3, addNum);
						FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num2, client.ClientData.MapCode);
						if (fuBenMapItem != null && fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
						{
							int nMaxTime = fuBenMapItem.MaxTime * 60;
							long startTicks = fuBenInfoItem.StartTicks;
							long endTicks = fuBenInfoItem.EndTicks;
							int nFinishTimer = (int)(endTicks - startTicks) / 1000;
							int killedNormalNum = 0;
							int nDieCount = fuBenInfoItem.nDieCount;
							fuBenTongGuanData = Global.GiveCopyMapGiftForScore(client, num2, copyMap.MapCode, nMaxTime, nFinishTimer, killedNormalNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, strName);
						}
						GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							Global.FormatRoleName(client, client.ClientData.RoleName),
							num2,
							num3
						}), null, client.ServerId);
						bool bActiveChenJiu = true;
						if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID) && GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
						{
							bActiveChenJiu = false;
						}
						GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(client, 1, nLev, bActiveChenJiu);
					}
				}
			}
			GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
			if (fuBenTongGuanData != null)
			{
				byte[] bytesData = DataHelper.ObjectToBytes<FuBenTongGuanData>(fuBenTongGuanData);
				GameManager.ClientMgr.NotifyAllFuBenTongGuanJiangLi(copyMap, bytesData);
			}
		}

		public void CopyMapPassAwardForAll(GameClient client, CopyMap copyMap, bool anyAlive)
		{
			if (copyMap.CopyMapPassAwardFlag)
			{
				LogManager.WriteLog(2, string.Format("CopyMapPassAwardForAll: 组队副本{0}序列ID({1})完成并给过奖励,不应再次给予", copyMap.FubenMapID, copyMap.FuBenSeqID), null, true);
			}
			else
			{
				copyMap.CopyMapPassAwardFlag = true;
				int fuBenSeqID = copyMap.FuBenSeqID;
				List<GameClient> list = new List<GameClient>();
				if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(copyMap.FubenMapID))
				{
					List<int> list2 = FuBenManager.FindMapCodeListByFuBenID(copyMap.FubenMapID);
					if (null != list2)
					{
						foreach (int mapCode in list2)
						{
							int num = this.FindCopyID(fuBenSeqID, mapCode);
							if (num >= 0)
							{
								CopyMap copyMap2 = this.FindCopyMap(num);
								if (null != copyMap2)
								{
									list.AddRange(copyMap2.GetClientsList());
								}
							}
						}
					}
				}
				else
				{
					list.AddRange(copyMap.GetClientsList());
				}
				list = Global.DistinctGameClientList(list);
				if (null != list)
				{
					if (fuBenSeqID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							fuBenInfoItem.EndTicks = TimeUtil.NOW();
							int addNum = 1;
							if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
							{
								addNum = 0;
							}
							int num2 = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
							if (num2 > 0)
							{
								int num3 = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
								int nLev = -1;
								string strName = null;
								SystemXmlItem systemXmlItem = null;
								if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(num2, out systemXmlItem))
								{
									nLev = systemXmlItem.GetIntValue("FuBenLevel", -1);
									strName = systemXmlItem.GetStringValue("CopyName");
								}
								FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num2, copyMap.MapCode);
								if (fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
								{
									int nMaxTime = fuBenMapItem.MaxTime * 60;
									long startTicks = fuBenInfoItem.StartTicks;
									long endTicks = fuBenInfoItem.EndTicks;
									int nFinishTimer = (int)(endTicks - startTicks) / 1000;
									int killedNormalNum = 0;
									int nDieCount = fuBenInfoItem.nDieCount;
									for (int i = 0; i < list.Count; i++)
									{
										GameClient gameClient = list[i];
										if (null != gameClient)
										{
											FuBenTongGuanData fuBenTongGuanData = Global.GiveCopyMapGiftForScore(gameClient, num2, copyMap.MapCode, nMaxTime, nFinishTimer, killedNormalNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, strName);
											if (fuBenTongGuanData != null)
											{
												byte[] buffer = DataHelper.ObjectToBytes<FuBenTongGuanData>(fuBenTongGuanData);
												GameManager.ClientMgr.SendToClient(gameClient, buffer, 521);
											}
										}
									}
								}
								for (int i = 0; i < list.Count; i++)
								{
									GameClient gameClient = list[i];
									if (null != gameClient)
									{
										Global.UpdateFuBenDataForQuickPassTimer(gameClient, num2, num3, addNum);
										GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											gameClient.ClientData.RoleID,
											Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName),
											num2,
											num3
										}), null, gameClient.ServerId);
										bool bActiveChenJiu = true;
										if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID) && GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(copyMap.FubenMapID))
										{
											bActiveChenJiu = false;
										}
										GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(gameClient, 1, nLev, bActiveChenJiu);
									}
								}
							}
						}
					}
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(copyMap.FubenMapID))
					{
						GameManager.ClientMgr.NotifyAllMapFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
					}
					else
					{
						GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
					}
				}
			}
		}

		public void CopyMapFaildForAll(List<GameClient> objsList, CopyMap copyMap)
		{
			if (copyMap.CopyMapPassAwardFlag)
			{
				LogManager.WriteLog(2, string.Format("CopyMapPassAwardForAll: 组队副本{0}序列ID({1})完成并给过奖励,不应再次给予", copyMap.FubenMapID, copyMap.FuBenSeqID), null, true);
			}
			else
			{
				copyMap.CopyMapPassAwardFlag = true;
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				objsList = Global.DistinctGameClientList(objsList);
				if (null != objsList)
				{
					FuBenTongGuanData fuBenTongGuanData = null;
					if (fuBenSeqID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							fuBenInfoItem.EndTicks = TimeUtil.NOW();
							int num = FuBenManager.FindFuBenIDByMapCode(mapCode);
							if (num > 0)
							{
								int num2 = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
								fuBenTongGuanData = new FuBenTongGuanData();
								fuBenTongGuanData.FuBenID = copyMap.FubenMapID;
								fuBenTongGuanData.MapCode = mapCode;
								fuBenTongGuanData.ResultMark = 1;
							}
						}
					}
					foreach (GameClient gameClient in objsList)
					{
						GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, gameClient.ClientData.RoleID, false);
					}
					if (fuBenTongGuanData != null && objsList.Count > 0)
					{
						byte[] bytesData = DataHelper.ObjectToBytes<FuBenTongGuanData>(fuBenTongGuanData);
						GameManager.ClientMgr.NotifyAllFuBenTongGuanJiangLi(copyMap, bytesData);
					}
				}
			}
		}

		public void ProcessKilledMonster(GameClient client, Monster monster)
		{
			if (monster.CopyMapID > 0)
			{
				CopyMap copyMap = this.FindCopyMap(monster.CopyMapID);
				if (null != copyMap)
				{
					if (monster.ManagerType == 11)
					{
						copyMap.SetKilledDynamicMonsterDict(monster.UniqueID);
					}
					else if (monster.CurrentMapCode == SingletonTemplate<MoRiJudgeManager>.Instance().MapCode)
					{
						copyMap.SetKilledDynamicMonsterDict(monster.UniqueID);
					}
					else if (!copyMap.CustomPassAwards)
					{
						bool flag = false;
						SystemXmlItem systemXmlItem = null;
						if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemXmlItem))
						{
							int intValue = systemXmlItem.GetIntValue("KillAll", -1);
							if (intValue == 2)
							{
								flag = true;
							}
						}
						if (!monster.MonsterZoneNode.IsDynamicZone() || flag)
						{
							if (101 == monster.MonsterType)
							{
								copyMap.SetKilledNormalDict(monster.RoleID);
							}
							else
							{
								copyMap.SetKilledBossDict(monster.RoleID);
							}
							bool flag2 = false;
							bool flag3 = GameManager.MonsterMgr.IsAnyMonsterAliveByCopyMapID(monster.CopyMapID);
							if (flag && !copyMap.bStoryCopyMapFinishStatus)
							{
								int intValue2 = systemXmlItem.GetIntValue("BossID", -1);
								if (monster.MonsterInfo.ExtensionID == intValue2)
								{
									if (Global.IsInTeamCopyScene(client.ClientData.MapCode) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(monster.CurrentMapCode))
									{
										this.CopyMapPassAwardForAll(client, copyMap, true);
									}
									else
									{
										this.CopyMapPassAward(client, copyMap, true);
									}
									Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 3);
									copyMap.bStoryCopyMapFinishStatus = true;
									this.KillAllMonster(copyMap);
								}
							}
							if (!flag && ((copyMap.KilledNormalNum >= copyMap.TotalNormalNum && copyMap.KilledBossNum >= copyMap.TotalBossNum) || !flag3))
							{
								if (this.IsHeroMapCode(monster.MonsterZoneNode.MapCode))
								{
									int heroIndex = FuBenManager.FindMapCodeIndexByFuBenID(monster.MonsterZoneNode.MapCode);
									GameManager.ClientMgr.ChangeRoleHeroIndex(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, heroIndex, false);
								}
								int num = FuBenManager.FindNextMapCodeByFuBenID(monster.MonsterZoneNode.MapCode);
								if (-1 == num)
								{
									GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !flag3);
									if (WanMotaCopySceneManager.IsWanMoTaMapCode(monster.MonsterZoneNode.MapCode))
									{
										WanMotaCopySceneManager.SendMsgToClientForWanMoTaCopyMapAward(client, copyMap, flag3);
									}
									else if (Global.IsInTeamCopyScene(client.ClientData.MapCode) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(monster.CurrentMapCode))
									{
										this.CopyMapPassAwardForAll(client, copyMap, flag3);
									}
									else
									{
										this.CopyMapPassAward(client, copyMap, flag3);
									}
								}
								else
								{
									GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !flag3);
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyAllFuBenMonstersNum(copyMap, !flag3);
							}
							if (flag2)
							{
							}
						}
					}
				}
			}
		}

		public void KillAllMonster(CopyMap copyMap)
		{
			List<object> list = GameManager.MonsterMgr.GetCopyMapIDMonsterList(copyMap.CopyMapID);
			list = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, list, false);
			if (null != list)
			{
				int i = 0;
				while (i < list.Count)
				{
					Monster monster = list[i] as Monster;
					if (null != monster)
					{
						if (monster.MonsterType != 1001)
						{
							Global.SystemKillMonster(monster);
						}
					}
					IL_6A:
					i++;
					continue;
					goto IL_6A;
				}
			}
		}

		public string GetCopyMapStrInfo()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			CopyMap nextCopyMap = this.GetNextCopyMap(num3);
			while (null != nextCopyMap)
			{
				num2++;
				num3++;
				int num4 = 0;
				if (dictionary.TryGetValue(nextCopyMap.MapCode, out num4))
				{
					dictionary[nextCopyMap.MapCode] = num4 + 1;
				}
				else
				{
					dictionary[nextCopyMap.MapCode] = 1;
				}
				num += nextCopyMap.TotalNormalNum;
				num += nextCopyMap.TotalBossNum;
				nextCopyMap = this.GetNextCopyMap(num3);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(string.Format("当前总的副本数量 {0} 个 \r\n", num2), new object[0]);
			stringBuilder.AppendFormat(string.Format("当前总的副本怪物数量 {0} 个, 总的动态怪物 {1} 个 \r\n", num, Monster.GetMonsterCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingAddFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingAddFuBenMonsterQueueCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingDestroyFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingDestroyFuBenMonsterQueueCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingReloadFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingReloadFuBenMonsterQueueCount()), new object[0]);
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				stringBuilder.AppendFormat(string.Format("MapCode {0} 副本数量 {1} 个 \r\n", keyValuePair.Key, keyValuePair.Value), new object[0]);
			}
			return stringBuilder.ToString();
		}

		public string ListCopyMapStrInfo()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			CopyMap nextCopyMap = this.GetNextCopyMap(num3);
			StringBuilder stringBuilder = new StringBuilder();
			while (null != nextCopyMap)
			{
				num2++;
				num3++;
				int num4 = 0;
				if (dictionary.TryGetValue(nextCopyMap.MapCode, out num4))
				{
					dictionary[nextCopyMap.MapCode] = num4 + 1;
				}
				else
				{
					dictionary[nextCopyMap.MapCode] = 1;
				}
				num += nextCopyMap.TotalNormalNum;
				num += nextCopyMap.TotalBossNum;
				stringBuilder.AppendFormat(string.Format("{0,10} {1,10} {2,10} \r\n", nextCopyMap.FuBenSeqID, nextCopyMap.MapCode, nextCopyMap.GetClientsList().Count), new object[0]);
				nextCopyMap = this.GetNextCopyMap(num3);
			}
			stringBuilder.AppendFormat(string.Format("当前总的副本数量 {0} 个 \r\n", num2), new object[0]);
			stringBuilder.AppendFormat(string.Format("当前总的副本怪物数量 {0} 个, 总的动态怪物 {1} 个 \r\n", num, Monster.GetMonsterCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingAddFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingAddFuBenMonsterQueueCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingDestroyFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingDestroyFuBenMonsterQueueCount()), new object[0]);
			stringBuilder.AppendFormat(string.Format("WaitingReloadFuBenMonsterQueueCount {0} \r\n", GameManager.MonsterZoneMgr.WaitingReloadFuBenMonsterQueueCount()), new object[0]);
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				stringBuilder.AppendFormat(string.Format("MapCode {0} 副本数量 {1} 个 \r\n", keyValuePair.Key, keyValuePair.Value), new object[0]);
			}
			return stringBuilder.ToString();
		}

		public void AddGuangMuEvent(CopyMap copyMap, int guangMuId, int show)
		{
			copyMap.AddGuangMuEvent(guangMuId, show);
			GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, guangMuId, show);
		}

		private int BaseCopyMapID = 1;

		private Dictionary<string, int> FuBenSeqID2CopyIDDict = new Dictionary<string, int>();

		private Dictionary<string, int> FuBenSeqID2MonsterStateDict = new Dictionary<string, int>();

		private Dictionary<string, int> RoleIDFuBenSeqID2AwardStateDict = new Dictionary<string, int>();

		private List<CopyMap> _ListCopyMaps = new List<CopyMap>(300);

		private Dictionary<int, CopyMap> _DictCopyMaps = new Dictionary<int, CopyMap>(300);

		private object _RoleDamageDict_TeamCopyMapDict_Mutex = new object();

		private Dictionary<int, Dictionary<int, RoleDamage>> RoleDamageDict = new Dictionary<int, Dictionary<int, RoleDamage>>();

		private List<CopyMap> TeamCopyMapDict = new List<CopyMap>();

		private long LastNotifyTeamDamageTicks = 0L;

		private static string[] HeroMapCodeFileds = null;
	}
}
