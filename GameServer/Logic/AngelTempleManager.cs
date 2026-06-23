using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.GoldAuction;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class AngelTempleManager
	{
		public void InitAngelTemple()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.AngelTemple);
			this.AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleMonsterUpgradeNumber"));
			this.AngelTempleMinHurt = GameManager.systemParamsList.GetParamValueIntByName("AngelTempleMinHurt", -1);
			double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("AngelTempleBossUpgrade", ',');
			if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length == 4)
			{
				this.AngelTempleBossUpgradeTime = (int)paramValueDoubleArrayByName[0];
				this.AngelTempleBossUpgradeParam1 = paramValueDoubleArrayByName[1];
				this.AngelTempleBossUpgradeParam2 = paramValueDoubleArrayByName[2];
				this.AngelTempleBossUpgradeParam3 = paramValueDoubleArrayByName[3];
			}
			this.m_sKillBossRoleName = GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleRole");
			for (int i = 0; i < 5; i++)
			{
				AngelTemplePointInfo angelTemplePointInfo = new AngelTemplePointInfo();
				angelTemplePointInfo.m_RoleID = 0;
				angelTemplePointInfo.m_DamagePoint = 0L;
				angelTemplePointInfo.m_GetAwardFlag = 0;
				angelTemplePointInfo.m_RoleName = "";
				this.m_PointInfoArray[i] = angelTemplePointInfo;
			}
			this.m_BossHP = 10000L;
			SystemXmlItem systemXmlItem = null;
			GameManager.systemAngelTempleData.SystemXmlItemDict.TryGetValue(1, out systemXmlItem);
			if (systemXmlItem == null)
			{
				throw new Exception("AngelTemple Scene ERROR");
			}
			this.m_AngelTempleData.MapCode = systemXmlItem.GetIntValue("MapCode", -1);
			this.m_AngelTempleData.MinChangeLifeNum = systemXmlItem.GetIntValue("MinZhuangSheng", -1);
			this.m_AngelTempleData.MinLevel = systemXmlItem.GetIntValue("MinLevel", -1);
			List<string> list = new List<string>();
			string stringValue = systemXmlItem.GetStringValue("TimePoints");
			if (stringValue != null && stringValue != "")
			{
				string[] array = stringValue.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i].Trim());
				}
			}
			this.m_AngelTempleData.BeginTime = list;
			this.m_AngelTempleData.PrepareTime = Global.GMax(systemXmlItem.GetIntValue("PrepareSecs", -1), systemXmlItem.GetIntValue("WaitingEnterSecs", -1));
			this.m_AngelTempleData.DurationTime = systemXmlItem.GetIntValue("FightingSecs", -1);
			this.m_AngelTempleData.LeaveTime = systemXmlItem.GetIntValue("ClearRolesSecs", -1);
			this.m_AngelTempleData.MinPlayerNum = systemXmlItem.GetIntValue("MinRequestNum", -1);
			this.m_AngelTempleData.MaxPlayerNum = systemXmlItem.GetIntValue("MaxEnterNum", -1);
			this.m_AngelTempleData.BossID = systemXmlItem.GetIntValue("BossID", -1);
			this.m_AngelTempleData.BossPosX = systemXmlItem.GetIntValue("BossPosX", -1);
			this.m_AngelTempleData.BossPosY = systemXmlItem.GetIntValue("BossPosY", -1);
		}

		public void GMSetHuoDongStartNow()
		{
			this.InitAngelTemple();
			this.m_AngelTempleData.BeginTime = new List<string>
			{
				TimeUtil.NowDateTime().ToString("HH:mm")
			};
			lock (this.m_AngelTempleScene)
			{
				this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
			}
		}

		public void OnLoadDynamicMonsters(Monster monster)
		{
			if (monster.MonsterInfo.ExtensionID == this.m_AngelTempleData.BossID)
			{
				this.LastMinDamage = 0L;
				this.m_AngelTempleBoss = monster;
				if (0.0 == this.BossBaseHP)
				{
					this.BossBaseHP = monster.MonsterInfo.VLifeMax;
				}
				if (this.AngelTempleMonsterUpgradePercent <= 0.0)
				{
					this.AngelTempleMonsterUpgradePercent = 1.0;
				}
				this.AngelTempleMonsterUpgradePercent = Global.Clamp(this.AngelTempleMonsterUpgradePercent, 0.001, 1000.0);
				monster.MonsterInfo.VLifeMax = this.BossBaseHP * this.AngelTempleMonsterUpgradePercent;
				monster.VLife = monster.MonsterInfo.VLifeMax;
				this.m_BossHP = (long)monster.MonsterInfo.VLifeMax;
			}
		}

		public void SetTotalPointInfo(string sName, long nPoint)
		{
			this.m_sTotalDamageName = sName;
			this.m_nTotalDamageValue = nPoint;
		}

		public void SendTimeInfoToAll(long ticks)
		{
			int nTimer;
			int eStatus;
			lock (this.m_AngelTempleScene)
			{
				nTimer = (int)((this.m_AngelTempleScene.m_lStatusEndTime - ticks) / 1000L);
				eStatus = (int)this.m_AngelTempleScene.m_eStatus;
			}
			GameManager.ClientMgr.NotifyAngelTempleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode, 570, null, eStatus, nTimer, 0, 0, 0, 0.0);
		}

		public void OnEnterScene(GameClient client)
		{
			this.SetLeaveFlag(client, false);
			this.SendTimeInfoToClient(client);
			this.NotifyInfoToClient(client);
			if (null != this.m_AngelTempleBoss)
			{
				this.NotifyInfoToAllClient(this.m_AngelTempleBoss.VLife);
			}
		}

		public void SendTimeInfoToClient(GameClient client)
		{
			long num = TimeUtil.NOW();
			int num2;
			int eStatus;
			lock (this.m_AngelTempleScene)
			{
				num2 = (int)((this.m_AngelTempleScene.m_lStatusEndTime - num) / 1000L);
				eStatus = (int)this.m_AngelTempleScene.m_eStatus;
			}
			string cmdData = string.Format("{0}:{1}", eStatus, num2);
			client.sendCmd(570, cmdData, false);
		}

		public bool ChangeToNextStatus(out AngelTempleStatus newStatus)
		{
			bool result = false;
			long num = TimeUtil.NOW();
			lock (this.m_AngelTempleScene)
			{
				if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_NULL)
				{
					if (this.CanEnterAngelTempleOnTime())
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_PREPARE;
						this.m_AngelTempleScene.m_lPrepareTime = num;
						this.m_AngelTempleScene.m_lStatusEndTime = num + (long)(this.m_AngelTempleData.PrepareTime * 1000);
						result = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_PREPARE)
				{
					if (num >= this.m_AngelTempleScene.m_lStatusEndTime)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_BEGIN;
						this.m_AngelTempleScene.m_lBeginTime = num;
						this.m_AngelTempleScene.m_lStatusEndTime = num + (long)(this.m_AngelTempleData.DurationTime * 1000);
						result = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_BEGIN)
				{
					if (num >= this.m_AngelTempleScene.m_lStatusEndTime || this.m_AngelTempleScene.m_bEndFlag != 0)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_END;
						this.m_AngelTempleScene.m_lEndTime = num;
						this.m_AngelTempleScene.m_lStatusEndTime = num + (long)(this.m_AngelTempleData.LeaveTime * 1000);
						result = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_END)
				{
					if (num >= this.m_AngelTempleScene.m_lStatusEndTime)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
						result = true;
					}
				}
				newStatus = this.m_AngelTempleScene.m_eStatus;
			}
			return result;
		}

		public void HeartBeatAngelTempleScene()
		{
			long num = TimeUtil.NOW();
			AngelTempleStatus angelTempleStatus;
			if (this.ChangeToNextStatus(out angelTempleStatus))
			{
				switch (angelTempleStatus)
				{
				case AngelTempleStatus.FIGHT_STATUS_NULL:
				{
					List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.m_AngelTempleData.MapCode);
					if (mapClients != null)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								if (gameClient.ClientData.MapCode == this.m_AngelTempleData.MapCode)
								{
									int num2 = GameManager.MainMapCode;
									int maxX = -1;
									int mapY = -1;
									if (MapTypes.Normal == Global.GetMapType(gameClient.ClientData.LastMapCode))
									{
										if (GameManager.BattleMgr.BattleMapCode != gameClient.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != gameClient.ClientData.LastMapCode)
										{
											num2 = gameClient.ClientData.LastMapCode;
											maxX = gameClient.ClientData.LastPosX;
											mapY = gameClient.ClientData.LastPosY;
										}
									}
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
									{
										gameClient.ClientData.bIsInAngelTempleMap = false;
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num2, maxX, mapY, -1, 0);
									}
								}
							}
						}
					}
					this.CleanUpAngelTempleScene();
					if (num >= this.m_AngelTempleScene.m_lEndTime + (long)(this.m_AngelTempleData.LeaveTime * 20000))
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
					}
					break;
				}
				case AngelTempleStatus.FIGHT_STATUS_PREPARE:
					Global.AddFlushIconStateForAll(1007, true);
					break;
				case AngelTempleStatus.FIGHT_STATUS_BEGIN:
				{
					lock (this.m_AngelTempleScene)
					{
						this.bBossKilled = false;
						this.m_AngelTempleScene.m_bEndFlag = 0;
					}
					this.SendTimeInfoToAll(num);
					int bossID = this.m_AngelTempleData.BossID;
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(this.m_AngelTempleData.MapCode, out gameMap))
					{
						LogManager.WriteLog(2, string.Format("天使神殿报错 地图配置 ID = {0}", this.m_AngelTempleData.MapCode), null, true);
						return;
					}
					int gridX = gameMap.CorrectWidthPointToGridPoint(this.m_AngelTempleData.BossPosX) / gameMap.MapGridWidth;
					int gridY = gameMap.CorrectHeightPointToGridPoint(this.m_AngelTempleData.BossPosY) / gameMap.MapGridHeight;
					this.AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleMonsterUpgradeNumber"));
					GameManager.MonsterZoneMgr.AddDynamicMonsters(this.m_AngelTempleData.MapCode, bossID, -1, 1, gridX, gridY, 1, 0, 0, null, null);
					break;
				}
				case AngelTempleStatus.FIGHT_STATUS_END:
					Global.AddFlushIconStateForAll(1007, false);
					this.SendTimeInfoToAll(num);
					if (!this.bBossKilled && this.m_AngelTempleBoss != null)
					{
						MonsterData monsterData = this.m_AngelTempleBoss.GetMonsterData();
						double num3 = 0.0;
						if (monsterData.MaxLifeV != monsterData.LifeV)
						{
							num3 = Global.Clamp(monsterData.MaxLifeV - monsterData.LifeV, monsterData.MaxLifeV / 10.0, monsterData.MaxLifeV);
							this.AngelTempleMonsterUpgradePercent *= num3 * 0.8 / monsterData.MaxLifeV;
							Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", this.AngelTempleMonsterUpgradePercent.ToString("0.00"));
						}
						GameManager.MonsterMgr.AddDelayDeadMonster(this.m_AngelTempleBoss);
						GameManager.ClientMgr.NotifyAngelTempleMsgBossDisappear(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode);
						LogManager.WriteLog(3, string.Format("天使神殿Boss未死亡,血量减少百分比{0:P} ,Boss生命值比例成长为{1}", num3 / monsterData.MaxLifeV, this.AngelTempleMonsterUpgradePercent), null, true);
						this.m_AngelTempleBoss = null;
					}
					this.GiveAwardAngelTempleScene(this.bBossKilled);
					break;
				}
			}
			if (angelTempleStatus == AngelTempleStatus.FIGHT_STATUS_BEGIN)
			{
			}
		}

		public void NotifyInfoToAllClient(double nBossHP)
		{
			lock (this.m_PointDamageInfoMutex)
			{
				GameManager.ClientMgr.NotifyAngelTempleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode, 572, this.m_PointInfoArray, 0, 0, 0, 0, 0, nBossHP);
			}
		}

		public void NotifyInfoToClient(GameClient client)
		{
			string arg = Global.FormatRoleName(client, client.ClientData.RoleName);
			double num = Math.Round((double)client.ClientData.AngelTempleCurrentPoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
			string strCmd = string.Format("{0}:{1}", arg, num);
			GameManager.ClientMgr.SendToClient(client, strCmd, 573);
		}

		public void ProcessAttackBossInAngelTempleScene(GameClient client, Monster monster, int nDamage)
		{
			if (nDamage > 0)
			{
				AngelTemplePointInfo angelTemplePointInfo;
				lock (this.m_PointDamageInfoMutex)
				{
					if (!this.m_RoleDamageAngelValue.TryGetValue(client.ClientData.RoleID, out angelTemplePointInfo))
					{
						angelTemplePointInfo = new AngelTemplePointInfo();
						angelTemplePointInfo.m_RoleID = client.ClientData.RoleID;
						angelTemplePointInfo.m_DamagePoint = (long)nDamage;
						angelTemplePointInfo.m_GetAwardFlag = 0;
						angelTemplePointInfo.m_RoleName = Global.FormatRoleName(client, client.ClientData.RoleName);
						this.m_RoleDamageAngelValue.Add(client.ClientData.RoleID, angelTemplePointInfo);
					}
					else
					{
						angelTemplePointInfo.m_DamagePoint += (long)nDamage;
					}
					this.AddRoleAuctionData(client, nDamage);
					if (angelTemplePointInfo.m_DamagePoint > this.LastMinDamage)
					{
						if (angelTemplePointInfo.Ranking < 0)
						{
							this.m_PointInfoArray[5] = angelTemplePointInfo;
							angelTemplePointInfo.Ranking = 1;
							Array.Sort<AngelTemplePointInfo>(this.m_PointInfoArray, angelTemplePointInfo);
						}
						else
						{
							Array.Sort<AngelTemplePointInfo>(this.m_PointInfoArray, 0, 5, angelTemplePointInfo);
						}
						if (null != this.m_PointInfoArray[5])
						{
							this.m_PointInfoArray[5].Ranking = -1;
						}
						this.LastMinDamage = ((this.m_PointInfoArray[4] != null) ? this.m_PointInfoArray[4].m_DamagePoint : 0L);
					}
				}
				client.ClientData.AngelTempleCurrentPoint = angelTemplePointInfo.m_DamagePoint;
				if (client.ClientData.AngelTempleCurrentPoint > client.ClientData.AngelTempleTopPoint)
				{
					client.ClientData.AngelTempleTopPoint = client.ClientData.AngelTempleCurrentPoint;
				}
				if (angelTemplePointInfo.m_DamagePoint > this.m_nTotalDamageValue)
				{
					string sName = Global.FormatRoleName(client, client.ClientData.RoleName);
					this.SetTotalPointInfo(sName, angelTemplePointInfo.m_DamagePoint);
				}
				long num = TimeUtil.NOW();
				int num2 = (int)(100.0 * monster.VLife / (double)this.m_BossHP);
				if (num >= client.ClientData.m_NotifyInfoTickForSingle + this.m_NotifyInfoDelayTick)
				{
					client.ClientData.m_NotifyInfoTickForSingle = num;
					this.NotifyInfoToClient(client);
				}
				if (num >= this.m_NotifyInfoTickForSingle + this.m_NotifyInfoDelayTick || num2 != this.m_LastNotifyBossHPPercent)
				{
					this.m_LastNotifyBossHPPercent = num2;
					this.m_NotifyInfoTickForSingle = num;
					this.NotifyInfoToAllClient(monster.VLife);
				}
			}
		}

		public void GiveAwardAngelTempleScene(bool bBossKilled)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.m_AngelTempleData.MapCode);
			if (null != mapClients)
			{
				int num = 0;
				List<AngelTemplePointInfo> list = new List<AngelTemplePointInfo>();
				lock (this.m_PointDamageInfoMutex)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						if (mapClients[i] is GameClient)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							AngelTemplePointInfo angelTemplePointInfo;
							if (!this.m_RoleDamageAngelValue.TryGetValue(gameClient.ClientData.RoleID, out angelTemplePointInfo))
							{
								this.SendAngelTempleAwardMsg(gameClient, -1, 0, 0, GLang.GetLang(6, new object[0]), "", bBossKilled);
							}
							else if (!angelTemplePointInfo.LeaveScene)
							{
								if (Interlocked.CompareExchange(ref angelTemplePointInfo.m_GetAwardFlag, 1, 0) == 0)
								{
									if (angelTemplePointInfo.m_DamagePoint < this.AngelTempleMinHurt)
									{
										this.SendAngelTempleAwardMsg(gameClient, -1, 0, 0, GLang.GetLang(6, new object[0]), "", bBossKilled);
									}
									else
									{
										num++;
										list.Add(angelTemplePointInfo);
									}
								}
							}
						}
					}
				}
				list.Sort(new Comparison<AngelTemplePointInfo>(AngelTemplePointInfo.Compare_static));
				if (bBossKilled)
				{
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.AngelTempleAward.SystemXmlItemDict)
					{
						if (null != keyValuePair.Value)
						{
							int intValue = keyValuePair.Value.GetIntValue("ID", -1);
							int num2 = keyValuePair.Value.GetIntValue("MinPaiMing", -1);
							int num3 = keyValuePair.Value.GetIntValue("MaxPaiMing", -1);
							int intValue2 = keyValuePair.Value.GetIntValue("ShengWang", -1);
							int intValue3 = keyValuePair.Value.GetIntValue("Gold", -1);
							string stringValue = keyValuePair.Value.GetStringValue("Goods");
							num2 = Global.GMax(0, num2 - 1);
							num3 = Global.GMin(10000, num3 - 1);
							int i = num2;
							while (i <= num3 && i < num)
							{
								list[i].m_AwardPaiMing = i + 1;
								list[i].m_AwardShengWang += intValue2;
								list[i].m_AwardGold += intValue3;
								list[i].GoodsList.AddNoRepeat(stringValue);
								i++;
							}
						}
					}
					int[] array = new int[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = i;
					}
					int num4 = 0;
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.AngelTempleLuckyAward.SystemXmlItemDict)
					{
						if (null != keyValuePair.Value)
						{
							int intValue4 = keyValuePair.Value.GetIntValue("ID", -1);
							int intValue5 = keyValuePair.Value.GetIntValue("Number", -1);
							string lang = Global.GetLang(keyValuePair.Value.GetStringValue("Name"));
							string stringValue2 = keyValuePair.Value.GetStringValue("Goods");
							int num5 = 0;
							while (num5 < intValue5 && num4 < num)
							{
								int randomNumber = Global.GetRandomNumber(num4, num);
								int num6 = array[num4];
								array[num4] = array[randomNumber];
								array[randomNumber] = num6;
								int index = array[num4];
								list[index].m_LuckPaiMingName = lang;
								list[index].GoodsList.AddNoRepeat(stringValue2);
								num5++;
								num4++;
							}
						}
					}
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.AngelTempleAward.SystemXmlItemDict)
					{
						if (null != keyValuePair.Value)
						{
							systemXmlItem = keyValuePair.Value;
						}
					}
					if (null != systemXmlItem)
					{
						int intValue = systemXmlItem.GetIntValue("ID", -1);
						int intValue2 = systemXmlItem.GetIntValue("ShengWang", -1);
						int intValue3 = systemXmlItem.GetIntValue("Gold", -1);
						string stringValue = systemXmlItem.GetStringValue("Goods");
						for (int i = 0; i < num; i++)
						{
							list[i].m_AwardPaiMing = -1;
							list[i].m_LuckPaiMingName = GLang.GetLang(6, new object[0]);
							list[i].m_AwardShengWang = intValue2;
							list[i].m_AwardGold = intValue3;
							list[i].GoodsList.AddNoRepeat(stringValue);
						}
					}
				}
				double num7 = 0.0;
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(1);
					if (null != config)
					{
						num7 += config.GetMult();
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					num7 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_AngelTemple);
				}
				num7 = Math.Max(1.0, num7);
				if (num7 > 1.0)
				{
					foreach (AngelTemplePointInfo angelTemplePointInfo2 in list)
					{
						angelTemplePointInfo2.m_AwardGold = (int)((double)angelTemplePointInfo2.m_AwardGold * num7);
						angelTemplePointInfo2.m_AwardShengWang = (int)((double)angelTemplePointInfo2.m_AwardShengWang * num7);
						foreach (AwardsItemData awardsItemData in angelTemplePointInfo2.GoodsList.Items)
						{
							awardsItemData.GoodsNum = (int)((double)awardsItemData.GoodsNum * num7);
						}
					}
				}
				foreach (AngelTemplePointInfo angelTemplePointInfo2 in list)
				{
					GameClient gameClient2 = GameManager.ClientMgr.FindClient(angelTemplePointInfo2.m_RoleID);
					if (null != gameClient2)
					{
						ProcessTask.ProcessAddTaskVal(gameClient2, TaskTypes.AngelTemple, -1, 1, new object[0]);
						if (angelTemplePointInfo2.m_AwardGold > 0)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient2, angelTemplePointInfo2.m_AwardGold, "天使神殿奖励", false);
						}
						if (angelTemplePointInfo2.m_AwardShengWang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(gameClient2, angelTemplePointInfo2.m_AwardShengWang, "天使神殿", true, true);
						}
						foreach (AwardsItemData awardsItemData in angelTemplePointInfo2.GoodsList.Items)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient2, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "天使神殿奖励物品", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
						}
						this.SendAngelTempleAwardMsg(gameClient2, angelTemplePointInfo2.m_AwardPaiMing, angelTemplePointInfo2.m_AwardGold, angelTemplePointInfo2.m_AwardShengWang, angelTemplePointInfo2.m_LuckPaiMingName, angelTemplePointInfo2.GoodsList.ToString(), bBossKilled);
					}
				}
			}
		}

		private void SendAngelTempleAwardMsg(GameClient client, int paiMing, int awardGold, int awardShengWang, string luckPaiMingName, string goodsString, bool success)
		{
			string strCmd;
			if (client.CodeRevision >= 2)
			{
				strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					paiMing,
					awardGold,
					awardShengWang,
					luckPaiMingName,
					goodsString,
					success ? 1 : 0
				});
			}
			else
			{
				strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					paiMing,
					awardGold,
					awardShengWang,
					luckPaiMingName,
					goodsString
				});
			}
			GameManager.ClientMgr.SendToClient(client, strCmd, 571);
		}

		private void SetLeaveFlag(GameClient client, bool leaveFlag)
		{
			AngelTemplePointInfo angelTemplePointInfo = null;
			lock (this.m_PointDamageInfoMutex)
			{
				if (this.m_RoleDamageAngelValue.TryGetValue(client.ClientData.RoleID, out angelTemplePointInfo))
				{
					angelTemplePointInfo.LeaveScene = leaveFlag;
				}
			}
		}

		public void LeaveAngelTempleScene(GameClient client, bool logout = false)
		{
			this.SetLeaveFlag(client, true);
			if (client.ClientData.MapCode == this.m_AngelTempleData.MapCode || client.ClientData.bIsInAngelTempleMap)
			{
				Interlocked.Decrement(ref this.m_AngelTempleScene.m_nPlarerCount);
				client.ClientData.bIsInAngelTempleMap = false;
				if (logout)
				{
					client.ClientData.MapCode = client.ClientData.LastMapCode;
					client.ClientData.PosX = client.ClientData.LastPosX;
					client.ClientData.PosY = client.ClientData.LastPosY;
				}
			}
		}

		public bool CanEnterAngelTempleOnTime()
		{
			lock (this.m_AngelTempleScene)
			{
				if (this.m_AngelTempleScene.m_eStatus >= AngelTempleStatus.FIGHT_STATUS_PREPARE && this.m_AngelTempleScene.m_eStatus < AngelTempleStatus.FIGHT_STATUS_END)
				{
					return true;
				}
			}
			DateTime t = TimeUtil.NowDateTime();
			string b = t.ToString("HH:mm");
			List<string> beginTime = this.m_AngelTempleData.BeginTime;
			bool result;
			if (null == beginTime)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < beginTime.Count; i++)
				{
					DateTime t2 = DateTime.Parse(beginTime[i]);
					DateTime t3 = t2.AddMinutes((double)(this.m_AngelTempleData.PrepareTime / 60));
					if (beginTime[i] == b || (t > t2 && t <= t3))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool AddBuffer(GameClient client, BufferItemTypes buffID, double[] newParams, bool notifyPropsChanged)
		{
			if (buffID == BufferItemTypes.MU_ANGELTEMPLEBUFF1)
			{
				Global.RemoveBufferData(client, 86);
			}
			else if (buffID == BufferItemTypes.MU_ANGELTEMPLEBUFF2)
			{
				Global.RemoveBufferData(client, 85);
			}
			int num = 0;
			int num2 = -1;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)buffID);
			if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				num2 = (int)bufferDataByID.BufferVal;
			}
			bool result;
			if (num2 == num)
			{
				result = false;
			}
			else
			{
				double[] array = new double[2];
				Global.UpdateBufferData(client, buffID, newParams, 1, notifyPropsChanged);
				if (notifyPropsChanged)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				result = true;
			}
			return result;
		}

		public void KillAngelBoss(GameClient client, Monster monster)
		{
			if (this.m_AngelTempleData.BossID == monster.MonsterInfo.ExtensionID)
			{
				lock (this.m_AngelTempleScene)
				{
					this.bBossKilled = true;
					this.m_AngelTempleScene.m_bEndFlag = 1;
				}
				string sKillBossRoleName = Global.FormatRoleName(client, client.ClientData.RoleName);
				this.m_sKillBossRoleName = sKillBossRoleName;
				Global.UpdateDBGameConfigg("AngelTempleRole", this.m_sKillBossRoleName);
				this.NotifyInfoToClient(client);
				this.NotifyInfoToAllClient(monster.VLife);
				this.m_AngelTempleScene.m_nKillBossRole = client.ClientData.RoleID;
				this.m_sKillBossRoleID = client.ClientData.RoleID;
				double num = (double)((TimeUtil.NOW() - this.m_AngelTempleScene.m_lBeginTime) / 1000L);
				double num2 = Global.Clamp(num, (double)(this.m_AngelTempleData.DurationTime / 10), (double)this.m_AngelTempleData.DurationTime);
				this.AngelTempleMonsterUpgradePercent *= (double)this.m_AngelTempleData.DurationTime * 0.8 / num2;
				Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", this.AngelTempleMonsterUpgradePercent.ToString("0.00"));
				LogManager.WriteLog(3, string.Format("天使神殿Boss被击杀,用时{0}秒 ,Boss生命值比例成长为{1}", num, this.AngelTempleMonsterUpgradePercent), null, true);
				GlodAuctionProcessCmdEx.getInstance().KillBossAddAuction(this.m_AngelTempleScene.m_nKillBossRole, this.m_BossHP, this.m_RoleAuctionData.Values.ToList<AuctionRoleData>(), AuctionEnum.AngelTemple);
				this.m_AngelTempleBoss = null;
			}
		}

		private void AddRoleAuctionData(GameClient client, int nDamage)
		{
			try
			{
				AuctionRoleData auctionRoleData;
				if (!this.m_RoleAuctionData.TryGetValue(client.ClientData.RoleID, out auctionRoleData))
				{
					auctionRoleData = new AuctionRoleData();
					auctionRoleData.m_RoleID = client.ClientData.RoleID;
					auctionRoleData.m_RoleName = Global.FormatRoleName(client, client.ClientData.RoleName);
					auctionRoleData.ZoneID = client.ClientData.ZoneID;
					auctionRoleData.strUserID = client.strUserID;
					auctionRoleData.ServerId = client.ServerId;
					auctionRoleData.Value = (long)nDamage;
					this.m_RoleAuctionData.Add(auctionRoleData.m_RoleID, auctionRoleData);
				}
				else
				{
					auctionRoleData.Value += (long)nDamage;
				}
			}
			catch
			{
			}
		}

		public void CleanUpAngelTempleScene()
		{
			this.m_RoleAuctionData.Clear();
			this.m_AngelTempleScene.CleanAll();
			lock (this.m_PointDamageInfoMutex)
			{
				this.m_RoleDamageAngelValue.Clear();
				for (int i = 0; i < this.m_PointInfoArray.Length; i++)
				{
					if (null != this.m_PointInfoArray[i])
					{
						this.m_PointInfoArray[i] = new AngelTemplePointInfo();
					}
				}
			}
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_sTotalDamageName) && this.m_sTotalDamageName == oldName)
				{
					this.m_sTotalDamageName = newName;
				}
			}
		}

		private const int PaiHangArrayLength = 6;

		public AngelTempleSceneInfo m_AngelTempleScene = new AngelTempleSceneInfo();

		public AngelTempleData m_AngelTempleData = new AngelTempleData();

		public Dictionary<int, AngelTemplePointInfo> m_RoleDamageAngelValue = new Dictionary<int, AngelTemplePointInfo>();

		public Dictionary<int, AuctionRoleData> m_RoleAuctionData = new Dictionary<int, AuctionRoleData>();

		public object m_PointDamageInfoMutex = new object();

		private long LastMinDamage;

		public AngelTemplePointInfo[] m_PointInfoArray = new AngelTemplePointInfo[6];

		public Monster m_AngelTempleBoss = null;

		public bool bBossKilled = false;

		public long m_BossHP = 0L;

		public long m_nTotalDamageValue = -1L;

		public string m_sTotalDamageName = "";

		public int m_sKillBossRoleID = 0;

		public string m_sKillBossRoleName = "";

		public long m_NotifyInfoTickForAll = 0L;

		public long m_NotifyInfoTickForSingle = 0L;

		public int m_LastNotifyBossHPPercent = -1;

		public long m_NotifyInfoDelayTick = 3000L;

		public long AngelTempleMinHurt = 0L;

		private int AngelTempleBossUpgradeTime = 0;

		private double AngelTempleBossUpgradeParam1 = 0.0;

		private double AngelTempleBossUpgradeParam2 = 0.0;

		private double AngelTempleBossUpgradeParam3 = 0.0;

		private double AngelTempleMonsterUpgradePercent = 0.0;

		private double BossBaseHP = 0.0;
	}
}
