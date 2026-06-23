using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class YaoSaiBossManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static YaoSaiBossManager getInstance()
		{
			return YaoSaiBossManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1851, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1852, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1853, 1, 1, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1854, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1855, 3, 3, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1857, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1858, 1, 1, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1851:
					return this.ProcessGetBossMiniInfoCmd(client, nID, bytes, cmdParams);
				case 1852:
					return this.ProcessZhaoHuanBossCmd(client, nID, bytes, cmdParams);
				case 1853:
					return this.ProcessTaoFaBossCmd(client, nID, bytes, cmdParams);
				case 1854:
					return this.ProcessGetBossFightInFoCmd(client, nID, bytes, cmdParams);
				case 1855:
					return this.ProcessBossFightExcuteCmd(client, nID, bytes, cmdParams);
				case 1857:
					return this.ProcessGetBossFightLogCmd(client, nID, bytes, cmdParams);
				case 1858:
					return this.ProcessGiveBossKillAwardCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public bool ProcessGetBossMiniInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				YaoSaiBossData roleBossData = this.GetRoleBossData(num);
				int zhanDouCount = this.GetZhanDouCount(rid, num);
				YaoSaiBossMainData cmdData = new YaoSaiBossMainData
				{
					BossInfo = roleBossData,
					HaveZhaoHuanCount = Global.GetRoleParamsInt32FromDB(client, "10176"),
					TaoFaCount = Global.GetRoleParamsInt32FromDB(client, "10179"),
					ZhaoHuanBossID = Global.GetRoleParamsInt32FromDB(client, "10177"),
					OtherID = num,
					FightCount = zhanDouCount
				};
				client.sendCmd<YaoSaiBossMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZhaoHuanBossCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int num2 = 0;
				DateTime t = TimeUtil.NowDateTime();
				int num3 = 0;
				int num4 = 0;
				int num5 = Global.GetRoleParamsInt32FromDB(client, "10176");
				if (num < 0 || num > 3)
				{
					num3 = 12;
				}
				else
				{
					if (num > 1)
					{
						num2 = num;
					}
					else
					{
						num2 = ((num5 >= YaoSaiBossManager.PuTongZhaoHuanCount) ? 1 : 0);
					}
					YaoSaiBossData roleBossData = this.GetRoleBossData(rid);
					if (null != roleBossData)
					{
						if (roleBossData.LifeV == 0.0 || roleBossData.DeadTime < t)
						{
							num3 = 2;
						}
						else
						{
							num3 = 3;
						}
					}
					else
					{
						int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10179");
						if (roleParamsInt32FromDB >= YaoSaiBossManager.TaoFaCount)
						{
							num3 = 11;
						}
						else
						{
							int randomNumber = Global.GetRandomNumber(1, 100000);
							foreach (KeyValuePair<int, PetBossItem> keyValuePair in this.PetBossXmlDict)
							{
								switch (num2)
								{
								case 0:
									if (randomNumber >= keyValuePair.Value.FreeStartValue && randomNumber <= keyValuePair.Value.FreeEndValue)
									{
										num4 = keyValuePair.Key;
									}
									break;
								case 1:
									if (randomNumber >= keyValuePair.Value.ZuanShiStartValue && randomNumber <= keyValuePair.Value.ZuanShiEndValue)
									{
										num4 = keyValuePair.Key;
									}
									break;
								case 2:
								case 3:
									if (keyValuePair.Value.Star == 5 && randomNumber <= keyValuePair.Value.FreeEndValue)
									{
										num4 = keyValuePair.Key;
									}
									break;
								}
								if (num4 > 0)
								{
									break;
								}
							}
							if (num4 < 1)
							{
								num3 = 6;
							}
							else
							{
								switch (num2)
								{
								case 0:
									if (num5 >= YaoSaiBossManager.PuTongZhaoHuanCount)
									{
										num3 = 4;
									}
									else
									{
										num5++;
										Global.SaveRoleParamsInt32ValueToDB(client, "10176", num5, true);
									}
									break;
								case 1:
									if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiBossManager.ZuanShiZhaoHuanCost, "要塞Boss召唤", true, true, false, DaiBiSySType.JingLingYaoSaiPuTongZhaoHuan))
									{
										num3 = 5;
									}
									break;
								case 2:
								{
									bool flag;
									bool flag2;
									if (Global.UseGoodsBindOrNot(client, YaoSaiBossManager.FiveStartNeedGoods, YaoSaiBossManager.FiveStartNeedNums, true, out flag, out flag2) < 1)
									{
										num3 = 13;
									}
									break;
								}
								case 3:
								{
									bool flag;
									bool flag2;
									if (Global.UseGoodsBindOrNot(client, YaoSaiBossManager.FiveStartNeedGoods, YaoSaiBossManager.FiveStartNeedNums, true, out flag, out flag2) < 1)
									{
										if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiBossManager.FiveStartNeedZuan, "要塞Boss五星召唤", true, true, false, DaiBiSySType.JingLingYaoSaiZhaoHuanBoss))
										{
											num3 = 5;
										}
									}
									break;
								}
								}
							}
						}
					}
				}
				if (num3 == 0)
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "10177", num4, true);
				}
				string cmdData = string.Concat(new object[]
				{
					num3,
					":",
					num4,
					":",
					num5
				});
				client.sendCmd(nID, cmdData, false);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss召唤", "类型=" + num2, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessTaoFaBossCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10177");
				int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "10179");
				DateTime t = TimeUtil.NowDateTime();
				int num2 = 0;
				PetBossItem petBossItem = null;
				double lifeV = 0.0;
				YaoSaiBossData yaoSaiBossData = this.GetRoleBossData(num);
				if (null != yaoSaiBossData)
				{
					if (yaoSaiBossData.LifeV == 0.0 || yaoSaiBossData.DeadTime < t)
					{
						num2 = 2;
					}
					else
					{
						num2 = 3;
					}
				}
				else if (roleParamsInt32FromDB2 >= YaoSaiBossManager.TaoFaCount)
				{
					num2 = 11;
				}
				else if (!this.PetBossXmlDict.TryGetValue(roleParamsInt32FromDB, out petBossItem))
				{
					num2 = 6;
				}
				else
				{
					Monster monsterByMonsterID = GameManager.MonsterZoneMgr.GetMonsterByMonsterID(petBossItem.MonsterID);
					if (null == monsterByMonsterID)
					{
						LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 讨伐boss失败 不存在配置的boss MonsterID bossID={0}", roleParamsInt32FromDB), null, true);
						num2 = 6;
					}
					else
					{
						lifeV = monsterByMonsterID.VLife;
					}
				}
				if (num2 != 0)
				{
					client.sendCmd<int>(nID, num2, false);
					return true;
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10177", 0, true);
				yaoSaiBossData = new YaoSaiBossData
				{
					BossID = roleParamsInt32FromDB,
					LifeV = lifeV,
					DeadTime = t.AddSeconds((double)petBossItem.Time),
					OwnerID = num
				};
				client.sendCmd<int>(nID, num2, false);
				Global.SaveRoleParamsInt32ValueToDB(client, "10179", roleParamsInt32FromDB2 + 1, true);
				this.SaveAndBroadcastUpdateYaoSaiBoss(client.ClientData.RoleID, yaoSaiBossData, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss讨伐", "bossID=" + roleParamsInt32FromDB, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetBossFightInFoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int otherID = Global.SafeConvertToInt32(cmdParams[1]);
				YaoSaiBossFightData bossFightData = this.GetBossFightData(client, otherID);
				client.sendCmd<YaoSaiBossFightData>(nID, bossFightData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessBossFightExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				YaoSaiBossFightData bossFightData = this.GetBossFightData(client, num2);
				DateTime t = TimeUtil.NowDateTime();
				double num3 = 0.0;
				bool needNotifyAward = true;
				int num4;
				if (bossFightData.BossMiniInfo == null || bossFightData.BossMiniInfo.LifeV <= 0.0 || t >= bossFightData.BossMiniInfo.DeadTime)
				{
					num4 = 7;
				}
				else if (!this.CanFight(client, cmdParams[2]))
				{
					num4 = 8;
				}
				else if (num != num2 && bossFightData.HaveFightTime >= YaoSaiBossManager.XieZhuFightCount)
				{
					num4 = 9;
				}
				else if (bossFightData.ZuanShiFightCost > client.ClientData.UserMoney)
				{
					num4 = 5;
				}
				else
				{
					num3 = this.GetInjure(client, bossFightData.BossMiniInfo.BossID, cmdParams[2]);
					if (bossFightData.BossMiniInfo.LifeV < num3)
					{
						num3 = bossFightData.BossMiniInfo.LifeV;
					}
					num4 = this.ExcuteInjure(client, bossFightData.BossMiniInfo, num3);
					if (num4 == 0)
					{
						if (bossFightData.ZuanShiFightCost > 0)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bossFightData.ZuanShiFightCost, "要塞boss战斗消耗_" + bossFightData.BossMiniInfo.BossID, true, true, false, DaiBiSySType.None);
						}
						this.SaveAndBroadcastUpdateYaoSaiBoss(client.ClientData.RoleID, bossFightData.BossMiniInfo, true);
						bossFightData.HaveFightTime++;
						Global.SaveRoleParamsStringToDB(client, "37", cmdParams[2], true);
						if (num != num2)
						{
							int num5 = Global.GetRoleParamsInt32FromDB(client, "10178");
							if (num5 >= YaoSaiBossManager.XieZhuAwardCount)
							{
								needNotifyAward = false;
								goto IL_244;
							}
							num5++;
							Global.SaveRoleParamsInt32ValueToDB(client, "10178", num5, true);
						}
						this.GiveFightAward(client, num2, num3);
					}
				}
				IL_244:
				YaoSaiBossFightResultData cmdData = new YaoSaiBossFightResultData
				{
					Result = num4,
					FightLife = Convert.ToInt32(num3),
					BossInfo = this.GetRoleBossData(num2),
					NeedNotifyAward = needNotifyAward
				};
				client.sendCmd<YaoSaiBossFightResultData>(nID, cmdData, false);
				if (num4 == 0)
				{
					string strTarEnvName = (num == num2) ? "自己" : "协助";
					string strOptType = (bossFightData.BossMiniInfo.LifeV > 0.0) ? "存活" : "击杀";
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss战斗", "bossID=" + bossFightData.BossMiniInfo.BossID, client.ClientData.RoleName, strTarEnvName, strOptType, Convert.ToInt32(num3), client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetBossFightLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				List<YaoSaiBossFightLog> list = this.GetFightLogList(rid, num);
				if (list != null && list.Count > 0)
				{
					list.Sort((YaoSaiBossFightLog x, YaoSaiBossFightLog y) => y.FightLife - x.FightLife);
					list = list.Take(25).ToList<YaoSaiBossFightLog>();
				}
				YaoSaiBossFightLogInfo cmdData = new YaoSaiBossFightLogInfo
				{
					OtherRid = num,
					BossFightLogList = list
				};
				client.sendCmd<YaoSaiBossFightLogInfo>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGiveBossKillAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = 0;
				DateTime t = TimeUtil.NowDateTime();
				YaoSaiBossData yaoSaiBossData = null;
				PetBossItem petBossItem = null;
				yaoSaiBossData = this.GetRoleBossData(num);
				if (null == yaoSaiBossData)
				{
					num2 = 7;
				}
				else if (yaoSaiBossData.LifeV != 0.0 && yaoSaiBossData.DeadTime > t)
				{
					num2 = 3;
				}
				else if (!this.PetBossXmlDict.TryGetValue(yaoSaiBossData.BossID, out petBossItem))
				{
					num2 = 6;
				}
				else
				{
					double vlife = GameManager.MonsterZoneMgr.GetMonsterByMonsterID(petBossItem.MonsterID).VLife;
					double num3 = vlife - yaoSaiBossData.LifeV;
					if (num3 * 10.0 < vlife)
					{
						num3 = vlife / 10.0;
					}
					string[] array = petBossItem.KillAward.Split(new char[]
					{
						'|'
					});
					string[] array2 = petBossItem.KillExtraAward.Split(new char[]
					{
						'|'
					});
					int num4 = 0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(14);
						if (null != config)
						{
							num4 += (int)config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num4 += (int)specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_YaoSaiBoss);
					}
					num4 = Math.Max(1, num4);
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < array.Length; i++)
					{
						if (!(array[i] == ""))
						{
							string[] array3 = array[i].Split(new char[]
							{
								','
							});
							if (array3.Length == 7)
							{
								GoodsData goodsData = new GoodsData
								{
									Id = -1,
									GoodsID = Convert.ToInt32(array3[0]),
									Using = 0,
									Forge_level = Convert.ToInt32(array3[3]),
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									GCount = Convert.ToInt32(Math.Floor((double)Convert.ToInt32(array3[1]) * num3 / vlife)) * num4,
									Binding = Convert.ToInt32(array3[2]),
									BagIndex = 0,
									Lucky = Convert.ToInt32(array3[5]),
									ExcellenceInfo = Convert.ToInt32(array3[6]),
									AppendPropLev = Convert.ToInt32(array3[4])
								};
								SystemXmlItem systemXmlItem = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
								{
									string textMsg = string.Format("系统中不存在{0}", goodsData.GoodsID);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
								}
								else
								{
									list.Add(goodsData);
								}
							}
						}
					}
					if (yaoSaiBossData.LifeV <= 0.0)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							if (!(array2[i] == ""))
							{
								string[] array3 = array2[i].Split(new char[]
								{
									','
								});
								if (array3.Length == 7)
								{
									GoodsData goodsData = new GoodsData
									{
										Id = -1,
										GoodsID = Convert.ToInt32(array3[0]),
										Using = 0,
										Forge_level = Convert.ToInt32(array3[3]),
										Starttime = "1900-01-01 12:00:00",
										Endtime = "1900-01-01 12:00:00",
										Site = 0,
										GCount = Convert.ToInt32(Math.Floor((double)Convert.ToInt32(array3[1]) * num3 / vlife)) * num4,
										Binding = Convert.ToInt32(array3[2]),
										BagIndex = 0,
										Lucky = Convert.ToInt32(array3[5]),
										ExcellenceInfo = Convert.ToInt32(array3[6]),
										AppendPropLev = Convert.ToInt32(array3[4])
									};
									SystemXmlItem systemXmlItem = null;
									if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
									{
										string textMsg = string.Format("系统中不存在{0}", goodsData.GoodsID);
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
									}
									else
									{
										list.Add(goodsData);
									}
								}
							}
						}
					}
					int num5 = 0;
					foreach (GoodsData goodsData2 in list)
					{
						if (goodsData2.GCount > 0)
						{
							num5 += (int)Math.Ceiling((double)goodsData2.GCount / (double)Global.GetGoodsGridNumByID(goodsData2.GoodsID));
						}
					}
					if (!Global.CanAddGoodsNum(client, num5))
					{
						num2 = 10;
					}
					else
					{
						foreach (GoodsData goodsData2 in list)
						{
							if (goodsData2.GCount >= 1)
							{
								SystemXmlItem systemXmlItem2 = null;
								GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData2.GoodsID, out systemXmlItem2);
								string stringValue = systemXmlItem2.GetStringValue("Title");
								LogManager.WriteLog(3, string.Format("要塞boss击杀奖励{0} {1}", client.ClientData.RoleID, stringValue), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", true, 1, "要塞boss击杀奖励", "1900-01-01 12:00:00", 0, 0, goodsData2.Lucky, 0, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, 0, null, null, 0, true);
							}
						}
						lock (this.RunTimeData.Mutex)
						{
							this.RunTimeData.RoleBossCacheDict.Remove(num);
							this.RunTimeData.BossZhanDouLogDict.Remove(num);
						}
						GameManager.DBCmdMgr.AddDBCmd(20310, string.Format("{0}", yaoSaiBossData.OwnerID), null, client.ServerId);
					}
				}
				client.sendCmd<int>(nID, num2, false);
				if (num2 == 0)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss击杀结果=" + yaoSaiBossData.LifeV, string.Concat(new object[]
					{
						"角色ID=",
						yaoSaiBossData.OwnerID,
						"bossID=",
						yaoSaiBossData.BossID
					}), client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 领取击杀boss奖励信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public YaoSaiBossData GetRoleBossData(int rid)
		{
			YaoSaiBossData yaoSaiBossData = null;
			YaoSaiBossData result;
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					this.RunTimeData.RoleBossCacheDict.TryGetValue(rid, out yaoSaiBossData);
				}
				result = yaoSaiBossData;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取角色boss信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
				result = null;
			}
			return result;
		}

		public int GetZhanDouCount(int rid, int otherid)
		{
			try
			{
				YaoSaiBossData roleBossData = this.GetRoleBossData(otherid);
				if (null == roleBossData)
				{
					return 0;
				}
				lock (this.RunTimeData.Mutex)
				{
					Dictionary<int, List<YaoSaiBossFightLog>> dictionary = null;
					if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(otherid, out dictionary))
					{
						return 0;
					}
					List<YaoSaiBossFightLog> list = null;
					if (!dictionary.TryGetValue(rid, out list))
					{
						return 0;
					}
					return list.Count;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取boss战斗次数出错 rid={0},otherid={2},ex={3}", rid, otherid, ex.Message), null, true);
			}
			return 0;
		}

		public int ExcuteInjure(GameClient client, YaoSaiBossData bossData, double injure)
		{
			try
			{
				if (bossData.LifeV < injure)
				{
					injure = bossData.LifeV;
				}
				bossData.LifeV -= injure;
				int num = 0;
				if (client.ClientData.RoleID != bossData.OwnerID)
				{
					RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bossData.OwnerID), 0);
					if (roleDataEx.Faction > 0)
					{
						List<BangHuiMemberData> list = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", bossData.OwnerID, roleDataEx.Faction), 0);
						bool flag;
						if (list != null)
						{
							flag = (list.Find((BangHuiMemberData x) => x.RoleID == client.ClientData.RoleID) == null);
						}
						else
						{
							flag = true;
						}
						if (!flag)
						{
							num = 2;
							goto IL_105;
						}
					}
					num = 1;
				}
				IL_105:
				lock (this.RunTimeData.Mutex)
				{
					Dictionary<int, List<YaoSaiBossFightLog>> dictionary = null;
					List<YaoSaiBossFightLog> list2 = null;
					if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(bossData.OwnerID, out dictionary))
					{
						dictionary = new Dictionary<int, List<YaoSaiBossFightLog>>();
						this.RunTimeData.BossZhanDouLogDict[bossData.OwnerID] = dictionary;
					}
					if (!dictionary.TryGetValue(client.ClientData.RoleID, out list2))
					{
						list2 = new List<YaoSaiBossFightLog>();
						dictionary[client.ClientData.RoleID] = list2;
					}
					list2.Add(new YaoSaiBossFightLog
					{
						OtherRid = client.ClientData.RoleID,
						OtherRname = client.ClientData.RoleName,
						InviteType = num,
						FightLife = Convert.ToInt32(injure)
					});
				}
				GameManager.DBCmdMgr.AddDBCmd(20309, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					bossData.OwnerID,
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					num,
					injure
				}), null, client.ServerId);
				return 0;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 执行boss战斗信息出错 rid={0},ex={3}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return -1;
		}

		public YaoSaiBossFightData GetBossFightData(GameClient client, int otherID)
		{
			try
			{
				YaoSaiBossManager.<>c__DisplayClass13 CS$<>8__locals1 = new YaoSaiBossManager.<>c__DisplayClass13();
				int roleID = client.ClientData.RoleID;
				int zhanDouCount = this.GetZhanDouCount(roleID, otherID);
				int zuanShiFightCost = 0;
				if (roleID == otherID)
				{
					int num = zhanDouCount - YaoSaiBossManager.ZhaoHuanFightCount;
					if (num >= 0)
					{
						if (num >= YaoSaiBossManager.EWaiFightCost.Count)
						{
							num = YaoSaiBossManager.EWaiFightCost.Count - 1;
						}
						zuanShiFightCost = YaoSaiBossManager.EWaiFightCost[num];
					}
				}
				string text = Global.GetRoleParamByName(client, "37");
				if (string.IsNullOrEmpty(text))
				{
					text = "0|0|0";
				}
				CS$<>8__locals1.realJingLing = text.Split(new char[]
				{
					'|'
				});
				int i;
				for (i = 0; i < CS$<>8__locals1.realJingLing.Length; i++)
				{
					if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(CS$<>8__locals1.realJingLing[i])))
					{
						CS$<>8__locals1.realJingLing[i] = "0";
					}
					text = string.Join("|", CS$<>8__locals1.realJingLing);
				}
				return new YaoSaiBossFightData
				{
					BossMiniInfo = this.GetRoleBossData(otherID),
					JingLingZhenRong = text,
					HaveFightTime = zhanDouCount,
					ZuanShiFightCost = zuanShiFightCost
				};
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取boss战斗信息出错 rid={0},otherid={2},ex={3}", client.ClientData.RoleID, otherID, ex.Message), null, true);
			}
			return null;
		}

		public List<YaoSaiBossFightLog> GetFightLogList(int rid, int otherid)
		{
			List<YaoSaiBossFightLog> list = new List<YaoSaiBossFightLog>();
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					Dictionary<int, List<YaoSaiBossFightLog>> dictionary = null;
					if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(otherid, out dictionary))
					{
						return list;
					}
					foreach (KeyValuePair<int, List<YaoSaiBossFightLog>> keyValuePair in dictionary)
					{
						list.AddRange(keyValuePair.Value);
					}
				}
				return list;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取战斗记录列表失败。rid={0},otherid={1},ex={2}", rid, otherid, ex.Message), null, true);
			}
			return list;
		}

		public bool CanFight(GameClient client, string jingLing)
		{
			bool flag = false;
			bool result;
			if (string.IsNullOrEmpty(jingLing))
			{
				result = false;
			}
			else
			{
				string[] array = jingLing.Split(new char[]
				{
					'|'
				});
				if (array.Length != 3)
				{
					result = false;
				}
				else
				{
					List<string> list = new List<string>();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string item = array2[i];
						if (!(item == "0"))
						{
							if (list.Contains(item))
							{
								return false;
							}
							list.Add(item);
							if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(item)))
							{
								return false;
							}
							flag = true;
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public double GetInjure(GameClient client, int bossID, string jingLing)
		{
			string[] array = jingLing.Split(new char[]
			{
				'|'
			});
			List<GoodsData> list = new List<GoodsData>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string item = array2[i];
				if (!(item == "0"))
				{
					GoodsData item2 = client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Id == Convert.ToInt32(item));
					list.Add(item2);
				}
			}
			PetBossItem petBossItem = null;
			double result;
			if (!this.PetBossXmlDict.TryGetValue(bossID, out petBossItem))
			{
				result = 0.0;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				foreach (GoodsData goodsData in list)
				{
					int num3 = 1 + (1 + goodsData.Forge_level) / petBossItem.PetLevelStep;
					int num4 = num3 * petBossItem.PetLevelStepNum[0];
					int num5 = num3 * petBossItem.PetLevelStepNum[1];
					int num6 = 1 + Global.GetEquipExcellencePropNum(goodsData) / petBossItem.ExcellentStep;
					int num7 = num6 * petBossItem.ExcellentStepNum[0];
					int num8 = num6 * petBossItem.ExcellentStepNum[1];
					num += num4 + num7;
					num2 += num5 + num8;
				}
				double num9 = (double)Global.GetRandomNumber(num, num2 + 1);
				int num10 = 1;
				bool flag = true;
				for (int j = 0; j < petBossItem.PetSuit.Count; j++)
				{
					int k;
					for (k = 0; k < list.Count; k++)
					{
						if (petBossItem.PetSuit[j] == list[k].GoodsID)
						{
							break;
						}
					}
					if (k < list.Count)
					{
						list.RemoveAt(k);
						num10 += petBossItem.PetRate[j];
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					num10 += petBossItem.SuitRate;
				}
				num9 *= (double)num10;
				result = num9;
			}
			return result;
		}

		public void OnLogin(GameClient client, bool isNewDay)
		{
			if (isNewDay)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10176", 0, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10178", 0, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10179", 0, true);
			}
		}

		public void SaveAndBroadcastUpdateYaoSaiBoss(int roleID, YaoSaiBossData bossData, bool writeDB = true)
		{
			try
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				if (writeDB)
				{
					lock (this.RunTimeData.Mutex)
					{
						this.RunTimeData.RoleBossCacheDict[bossData.OwnerID] = bossData;
					}
				}
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(bossData.OwnerID);
				if (null != gameClient2)
				{
					gameClient2.sendCmd<YaoSaiBossData>(1856, bossData, false);
				}
				if (writeDB)
				{
					GameManager.DBCmdMgr.AddDBCmd(20307, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						bossData.OwnerID,
						bossData.BossID,
						bossData.LifeV,
						bossData.DeadTime.ToString().Replace(':', '$')
					}), null, GameCoreInterface.getinstance().GetLocalServerId());
				}
				List<FriendData> list = Global.sendToDB<List<FriendData>, string>(142, string.Format("{0}", bossData.OwnerID), GameCoreInterface.getinstance().GetLocalServerId());
				foreach (FriendData friendData in list)
				{
					if (friendData.OnlineState > 0 && friendData.FriendType == 0)
					{
						GameClient gameClient3 = GameManager.ClientMgr.FindClient(friendData.OtherRoleID);
						if (null != gameClient3)
						{
							if (Global.GetRoleParamsInt64FromDB(gameClient3, "10184") != 0L)
							{
								gameClient3.sendCmd<YaoSaiBossData>(1856, bossData, false);
							}
						}
					}
				}
				RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bossData.OwnerID), 0);
				if (roleDataEx.Faction > 0)
				{
					List<BangHuiMemberData> list2 = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", bossData.OwnerID, roleDataEx.Faction), 0);
					foreach (BangHuiMemberData bangHuiMemberData in list2)
					{
						if (bangHuiMemberData.OnlineState > 0)
						{
							GameClient gameClient3 = GameManager.ClientMgr.FindClient(bangHuiMemberData.RoleID);
							if (null != gameClient3)
							{
								if (Global.GetRoleParamsInt64FromDB(gameClient3, "10184") != 0L)
								{
									gameClient3.sendCmd<YaoSaiBossData>(1856, bossData, false);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 广播更新boss信息错误。ex:{0}", ex.Message), null, true);
			}
		}

		public void GiveFightAward(GameClient client, int otherid, double injure)
		{
			try
			{
				YaoSaiBossData roleBossData = this.GetRoleBossData(otherid);
				PetBossItem petBossItem = null;
				if (this.PetBossXmlDict.TryGetValue(roleBossData.BossID, out petBossItem))
				{
					int num = 0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(14);
						if (null != config)
						{
							num += (int)config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num += (int)specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_YaoSaiBoss);
					}
					num = Math.Max(1, num);
					string[] array = petBossItem.FightAward.Split(new char[]
					{
						'|'
					});
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < array.Length; i++)
					{
						if (!(array[i] == ""))
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 7)
							{
								GoodsData goodsData = new GoodsData
								{
									Id = -1,
									GoodsID = Convert.ToInt32(array2[0]),
									Using = 0,
									Forge_level = Convert.ToInt32(array2[3]),
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									GCount = Convert.ToInt32(Convert.ToInt32(array2[1])) * num,
									Binding = Convert.ToInt32(array2[2]),
									BagIndex = 0,
									Lucky = Convert.ToInt32(array2[5]),
									ExcellenceInfo = Convert.ToInt32(array2[6]),
									AppendPropLev = Convert.ToInt32(array2[4])
								};
								SystemXmlItem systemXmlItem = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
								{
									string textMsg = string.Format("系统中不存在{0}", goodsData.GoodsID);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
								}
								else
								{
									list.Add(goodsData);
								}
							}
						}
					}
					int num2 = 0;
					foreach (GoodsData goodsData2 in list)
					{
						if (goodsData2.GCount > 0)
						{
							num2 += (int)Math.Ceiling((double)goodsData2.GCount / (double)Global.GetGoodsGridNumByID(goodsData2.GoodsID));
						}
					}
					if (!Global.CanAddGoodsNum(client, num2))
					{
						Global.UseMailGivePlayerAward2(client, list, GLang.GetLang(2622, new object[0]), GLang.GetLang(2622, new object[0]), 0, 0, 0);
					}
					else
					{
						foreach (GoodsData goodsData2 in list)
						{
							SystemXmlItem systemXmlItem2 = null;
							GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData2.GoodsID, out systemXmlItem2);
							string stringValue = systemXmlItem2.GetStringValue("Title");
							LogManager.WriteLog(3, string.Format("要塞Boss战斗奖励{0} {1}", client.ClientData.RoleID, stringValue), null, true);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", true, 1, "要塞Boss战斗奖励", "1900-01-01 12:00:00", 0, 0, goodsData2.Lucky, 0, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, 0, null, null, 0, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 发放战斗奖励失败。ex:{0}", ex.Message), null, true);
			}
		}

		public int GetRoleBossState(int rid, int otherid)
		{
			try
			{
				DateTime t = TimeUtil.NowDateTime();
				if (Global.SafeConvertToInt64(Global.GetRoleParamsFromDBByRoleID(rid, "10184", 0)) == 0L)
				{
					return 0;
				}
				lock (this.RunTimeData.Mutex)
				{
					YaoSaiBossData yaoSaiBossData = null;
					if (!this.RunTimeData.RoleBossCacheDict.TryGetValue(rid, out yaoSaiBossData))
					{
						return 1;
					}
					if (yaoSaiBossData.LifeV <= 0.0 || t > yaoSaiBossData.DeadTime)
					{
						return 1;
					}
					Dictionary<int, List<YaoSaiBossFightLog>> dictionary = null;
					if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(rid, out dictionary))
					{
						return 2;
					}
					List<YaoSaiBossFightLog> list = null;
					if (!dictionary.TryGetValue(otherid, out list))
					{
						return 2;
					}
					return (list.Count < YaoSaiBossManager.XieZhuFightCount) ? 2 : 3;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取显示boss状态错误。rid={0},otherid={1},ex={2}", rid, otherid, ex.Message), null, true);
			}
			return 0;
		}

		public void LoadRunTimeDataFromDB()
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				Dictionary<int, YaoSaiBossData> dictionary = Global.sendToDB<Dictionary<int, YaoSaiBossData>, int>(20306, 0, GameCoreInterface.getinstance().GetLocalServerId());
				lock (this.RunTimeData.Mutex)
				{
					if (null != dictionary)
					{
						this.RunTimeData.RoleBossCacheDict = dictionary;
					}
				}
				Dictionary<int, List<YaoSaiBossFightLog>> dictionary2 = Global.sendToDB<Dictionary<int, List<YaoSaiBossFightLog>>, int>(20308, 0, GameCoreInterface.getinstance().GetLocalServerId());
				Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>> dictionary3 = new Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>>();
				foreach (KeyValuePair<int, List<YaoSaiBossFightLog>> keyValuePair in dictionary2)
				{
					Dictionary<int, List<YaoSaiBossFightLog>> dictionary4 = new Dictionary<int, List<YaoSaiBossFightLog>>();
					foreach (YaoSaiBossFightLog yaoSaiBossFightLog in keyValuePair.Value)
					{
						List<YaoSaiBossFightLog> list = null;
						if (!dictionary4.TryGetValue(yaoSaiBossFightLog.OtherRid, out list))
						{
							list = new List<YaoSaiBossFightLog>();
							dictionary4[yaoSaiBossFightLog.OtherRid] = list;
						}
						list.Add(yaoSaiBossFightLog);
					}
					dictionary3[keyValuePair.Key] = dictionary4;
				}
				lock (this.RunTimeData.Mutex)
				{
					this.RunTimeData.BossZhanDouLogDict = dictionary3;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 获取数据库数据失败。ex:{0}", ex.Message), null, true);
			}
		}

		public void LoadConfig()
		{
			this.LoadSystemParams();
			this.LoadPetBossXml();
			this.LoadRunTimeDataFromDB();
		}

		public void LoadSystemParams()
		{
			try
			{
				string[] array = GameManager.systemParamsList.GetParamValueByName("ManorBossCall").Split(new char[]
				{
					','
				});
				if (array.Length < 3)
				{
					LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 配置表配置出错 ManorBossCall", new object[0]), null, true);
				}
				else
				{
					YaoSaiBossManager.PuTongZhaoHuanCount = Global.SafeConvertToInt32(array[0]);
					YaoSaiBossManager.ZuanShiZhaoHuanCost = Global.SafeConvertToInt32(array[1]);
					YaoSaiBossManager.TaoFaCount = Global.SafeConvertToInt32(array[2]);
					string[] array2 = GameManager.systemParamsList.GetParamValueByName("ManorBossFight").Split(new char[]
					{
						','
					});
					if (array2.Length < 4)
					{
						LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 配置表配置出错 ManorBossCall", new object[0]), null, true);
					}
					else
					{
						YaoSaiBossManager.ZhaoHuanFightCount = Global.SafeConvertToInt32(array2[0]);
						YaoSaiBossManager.XieZhuFightCount = Global.SafeConvertToInt32(array2[1]);
						YaoSaiBossManager.XieZhuAwardCount = Global.SafeConvertToInt32(array2[2]);
						for (int i = 3; i < array2.Length; i++)
						{
							YaoSaiBossManager.EWaiFightCost.Add(Global.SafeConvertToInt32(array2[i]));
						}
						string[] array3 = GameManager.systemParamsList.GetParamValueByName("ManorSuperBoss").Split(new char[]
						{
							',',
							'|'
						});
						if (array3.Length < 3)
						{
							LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 配置表配置出错 ManorSuperBoss", new object[0]), null, true);
						}
						else
						{
							YaoSaiBossManager.FiveStartNeedGoods = Global.SafeConvertToInt32(array3[0]);
							YaoSaiBossManager.FiveStartNeedNums = Global.SafeConvertToInt32(array3[1]);
							YaoSaiBossManager.FiveStartNeedZuan = Global.SafeConvertToInt32(array3[2]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", "SystemParms.xml", ex.Message), ex, true);
			}
		}

		public void LoadPetBossXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath("Config\\PetBoss.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					this.PetBossXmlDict.Clear();
					foreach (XElement xml in enumerable)
					{
						int num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
						string[] array = Global.GetDefAttributeStr(xml, "FreeRate", "").Split(new char[]
						{
							','
						});
						string[] array2 = Global.GetDefAttributeStr(xml, "ZuanRate", "").Split(new char[]
						{
							','
						});
						if (array.Length < 2)
						{
							array = new string[]
							{
								"0",
								"0"
							};
						}
						if (array2.Length < 2)
						{
							array2 = new string[]
							{
								"0",
								"0"
							};
						}
						string[] array3 = Global.GetDefAttributeStr(xml, "PetLevelStepNum", "").Split(new char[]
						{
							','
						});
						int[] petLevelStepNum;
						if (array3.Length != 2)
						{
							int[] array4 = new int[2];
							petLevelStepNum = array4;
						}
						else
						{
							petLevelStepNum = new int[]
							{
								Global.SafeConvertToInt32(array3[0]),
								Global.SafeConvertToInt32(array3[1])
							};
						}
						string[] array5 = Global.GetDefAttributeStr(xml, "ExcellentStepNum", "").Split(new char[]
						{
							','
						});
						int[] excellentStepNum;
						if (array5.Length != 2)
						{
							int[] array4 = new int[2];
							excellentStepNum = array4;
						}
						else
						{
							excellentStepNum = new int[]
							{
								Global.SafeConvertToInt32(array5[0]),
								Global.SafeConvertToInt32(array5[1])
							};
						}
						string[] array6 = Global.GetDefAttributeStr(xml, "PetSuit", "").Split(new char[]
						{
							','
						});
						List<int> list = new List<int>();
						foreach (string str in array6)
						{
							list.Add(Global.SafeConvertToInt32(str));
						}
						string[] array8 = Global.GetDefAttributeStr(xml, "PetRate", "").Split(new char[]
						{
							','
						});
						List<int> list2 = new List<int>();
						foreach (string str in array8)
						{
							list2.Add(Global.SafeConvertToInt32(str));
						}
						for (int j = list.Count; j < list2.Count; j++)
						{
							list2.Add(1);
						}
						this.PetBossXmlDict[num] = new PetBossItem
						{
							ID = num,
							MonsterID = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "MonsterID", "0")),
							Star = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "Star", "0")),
							FreeStartValue = Global.SafeConvertToInt32(array[0]),
							FreeEndValue = Global.SafeConvertToInt32(array[1]),
							ZuanShiStartValue = Global.SafeConvertToInt32(array2[0]),
							ZuanShiEndValue = Global.SafeConvertToInt32(array2[1]),
							Time = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "Time", "0")),
							FightAward = Global.GetDefAttributeStr(xml, "FightAward", ""),
							KillAward = Global.GetDefAttributeStr(xml, "KillAward", ""),
							KillExtraAward = Global.GetDefAttributeStr(xml, "KillExtraAward", ""),
							PetLevelStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "PetLevelStep", "1")),
							PetLevelStepNum = petLevelStepNum,
							ExcellentStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "ExcellentStep", "1")),
							ExcellentStepNum = excellentStepNum,
							PetSuit = list,
							PetRate = list2,
							SuitRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "SuitRate", "1"))
						};
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", text, ex.Message), ex, true);
			}
		}

		public void YaoSaiBossTimer_Work()
		{
			try
			{
				long num = TimeUtil.NOW();
				lock (this.RunTimeData.Mutex)
				{
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiBoss :: 定时器错误 ：ex:{0}", ex.Message), null, true);
			}
		}

		public YaoSaiBossRunTimeData RunTimeData = new YaoSaiBossRunTimeData();

		public static int PuTongZhaoHuanCount = 0;

		public static int ZuanShiZhaoHuanCost = 0;

		public static int TaoFaCount = 0;

		public static int ZhaoHuanFightCount = 0;

		public static int XieZhuFightCount = 0;

		public static int XieZhuAwardCount = 0;

		public static int FiveStartNeedGoods = 0;

		public static int FiveStartNeedNums = 0;

		public static int FiveStartNeedZuan = 0;

		public static List<int> EWaiFightCost = new List<int>();

		public Dictionary<int, PetBossItem> PetBossXmlDict = new Dictionary<int, PetBossItem>();

		private static YaoSaiBossManager instance = new YaoSaiBossManager();
	}
}
