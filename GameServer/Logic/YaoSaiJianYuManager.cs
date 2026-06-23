using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class YaoSaiJianYuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		public static YaoSaiJianYuManager getInstance()
		{
			return YaoSaiJianYuManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1520, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1521, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1522, 2, 2, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1523, 3, 3, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1524, 2, 2, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1525, 3, 3, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1527, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1528, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(53, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(54, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, YaoSaiJianYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 44, YaoSaiJianYuManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(53, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(54, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, YaoSaiJianYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 44, YaoSaiJianYuManager.getInstance());
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
			if (!GlobalNew.IsGongNengOpened(client, 70, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1520:
					return this.ProcessGetYaoSaiMainDataCmd(client, nID, bytes, cmdParams);
				case 1521:
					return this.ProcessGetYaoSaiHuDongDataCmd(client, nID, bytes, cmdParams);
				case 1522:
					return this.ProcessYaoSaiRevoltCmd(client, nID, bytes, cmdParams);
				case 1523:
					return this.ProcessYaoSaiHuDongCmd(client, nID, bytes, cmdParams);
				case 1524:
					return this.ProcessYaoSaiFreeCmd(client, nID, bytes, cmdParams);
				case 1525:
					return this.ProcessYaoSaiOptCmd(client, nID, bytes, cmdParams);
				case 1527:
					return this.ProcessBuyZhengFuCountCmd(client, nID, bytes, cmdParams);
				case 1528:
					return this.ProcessGetYaoSaiDataCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 53)
			{
				JingJiChangWinEventObject jingJiChangWinEventObject = eventObject as JingJiChangWinEventObject;
				if (jingJiChangWinEventObject != null && jingJiChangWinEventObject.getType() == 1)
				{
					this.processWin(jingJiChangWinEventObject.getPlayer(), jingJiChangWinEventObject.getRobot());
				}
			}
			else if (eventObject.getEventType() == 54)
			{
				JingJiChangFailedEventObject jingJiChangFailedEventObject = eventObject as JingJiChangFailedEventObject;
				if (jingJiChangFailedEventObject != null && jingJiChangFailedEventObject.getType() == 1)
				{
					this.processFailed(jingJiChangFailedEventObject.getPlayer(), jingJiChangFailedEventObject.getRobot());
				}
			}
			else if (eventObject.getEventType() == 14)
			{
				GameClient player = (eventObject as PlayerInitGameEventObject).getPlayer();
				this.OnLogin(player);
			}
			else if (eventObject.getEventType() == 12)
			{
				GameClient player = (eventObject as PlayerLogoutEventObject).getPlayer();
				this.OnLogout(player);
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 29)
			{
				OnClientChangeMapEventObject onClientChangeMapEventObject = eventObject as OnClientChangeMapEventObject;
				if (null != onClientChangeMapEventObject)
				{
					bool flag = GlobalNew.IsGongNengOpened(onClientChangeMapEventObject.Client, 70, false);
					if (flag)
					{
						GameClient client = onClientChangeMapEventObject.Client;
						if (Global.CanRecordPos(client))
						{
							GameManager.DBCmdMgr.AddDBCmd(10001, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.MapCode,
								client.ClientData.RoleDirection,
								client.ClientData.PosX,
								client.ClientData.PosY
							}), null, client.ServerId);
						}
						KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
						if (null == yaoSaiPrisonRoleData)
						{
							this.UpdateYaoSaiPrisonRoleData(onClientChangeMapEventObject.Client);
							YaoSaiMissionManager.getInstance().RefReshMission(onClientChangeMapEventObject.Client);
						}
						onClientChangeMapEventObject.Result = true;
					}
					else
					{
						onClientChangeMapEventObject.Result = false;
					}
					onClientChangeMapEventObject.Handled = true;
				}
			}
		}

		public bool ProcessGetYaoSaiMainDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				PrisonMainData cmdData = this.BuildYaoSaiMainDataForClient(client);
				client.sendCmd<PrisonMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetYaoSaiHuDongDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				List<PrisonLogData> cmdData = this.BuildYaoSaiLogDataForClient(client);
				client.sendCmd<List<PrisonLogData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessYaoSaiRevoltCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.RoleID != num2)
				{
					return true;
				}
				if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
				{
					return true;
				}
				bool flag = TimeUtil.NOW() - this.GetRevoltCD(client) < (long)(60000 * this.FightCDTime);
				if (flag && num3 == 0)
				{
					num = -2007;
					client.sendCmd(nID, string.Format("{0}", num), false);
					return true;
				}
				KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
				if (yaoSaiPrisonRoleData == null || yaoSaiPrisonRoleData.OwnerID == 0)
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}", num), false);
					return true;
				}
				KFPrisonRoleData yaoSaiPrisonRoleData2 = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(yaoSaiPrisonRoleData.OwnerID);
				if (yaoSaiPrisonRoleData2 == null || yaoSaiPrisonRoleData2.RoleID == 0)
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}", num), false);
					return true;
				}
				if (flag && num3 == 1)
				{
					if (client.ClientData.UserMoney < this.FightCDClearCost)
					{
						num = -10;
						client.sendCmd(nID, string.Format("{0}", num), false);
						return true;
					}
				}
				int yaoSaiLevelID = this.GetYaoSaiLevelID(Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level));
				int yaoSaiLevelID2 = this.GetYaoSaiLevelID(yaoSaiPrisonRoleData2.UnionLevel);
				if (yaoSaiLevelID2 > yaoSaiLevelID)
				{
					int num4 = JunTuanClient.getInstance().YaoSaiPrisonOpt(yaoSaiPrisonRoleData.RoleID, yaoSaiPrisonRoleData2.RoleID, -1, true);
					if (num4 < 0)
					{
						num = -5;
						client.sendCmd(nID, string.Format("{0}", num), false);
						return true;
					}
					num = 1;
				}
				else
				{
					KFPrisonJingJiData yaoSaiPrisonJingJiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(yaoSaiPrisonRoleData.OwnerID);
					if (yaoSaiPrisonJingJiData == null || null == yaoSaiPrisonJingJiData.PlayerJingJiMirrorData)
					{
						num = -4;
						client.sendCmd(nID, string.Format("{0}", num), false);
						return true;
					}
					PlayerJingJiData beChallengerData = DataHelper.BytesToObject<PlayerJingJiData>(yaoSaiPrisonJingJiData.PlayerJingJiMirrorData, 0, yaoSaiPrisonJingJiData.PlayerJingJiMirrorData.Length);
					JingJiChangManager.getInstance().enterJingJiChang(client, beChallengerData, JingJiFuBenType.YAOSAI);
					client.ClientData.YaoSaiPrisonOptType = -1;
					this.UpdateYaoSaiPrisonRoleData(client);
				}
				if (flag && num3 == 1)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.FightCDClearCost, "要塞征服反抗CD", true, true, false, DaiBiSySType.None))
					{
						num = -10;
						client.sendCmd(nID, string.Format("{0}", num), false);
						return true;
					}
				}
				this.SetRevoltCD(client, TimeUtil.NOW());
				client.sendCmd(nID, string.Format("{0}", num), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessYaoSaiHuDongCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				if (client.ClientData.RoleID != num2)
				{
					return true;
				}
				Dictionary<int, YaoSaiJianYuManorCommandConfig> dictionary = null;
				List<FallGoodsItem> list = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.CommandAwardConfigDict;
					list = this.FallGoodsItemConfigList;
				}
				List<int> yaoSaiJianYuCount = this.GetYaoSaiJianYuCount(client);
				if (yaoSaiJianYuCount[2] >= this.FuLuHuDongCount)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						targetID,
						num3,
						this.FuLuHuDongCount
					}), false);
					return true;
				}
				YaoSaiJianYuManorCommandConfig yaoSaiJianYuManorCommandConfig = null;
				if (!dictionary.TryGetValue(num3, out yaoSaiJianYuManorCommandConfig))
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						targetID,
						num3,
						yaoSaiJianYuCount[2]
					}), false);
					return true;
				}
				List<KFPrisonRoleData> yaoSaiPrisonFuLuData = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(num2);
				if (null == yaoSaiPrisonFuLuData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						targetID,
						num3,
						yaoSaiJianYuCount[2]
					}), false);
					return true;
				}
				KFPrisonRoleData kfprisonRoleData = yaoSaiPrisonFuLuData.Find((KFPrisonRoleData x) => x.RoleID == targetID);
				if (null == kfprisonRoleData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						targetID,
						num3,
						yaoSaiJianYuCount[2]
					}), false);
					return true;
				}
				List<PrisonHuDongData> huDongData = this.GetHuDongData(client);
				PrisonHuDongData prisonHuDongData = huDongData.Find((PrisonHuDongData x) => x.RoleID == targetID);
				if (null != prisonHuDongData)
				{
					long num4 = TimeUtil.NOW() - prisonHuDongData.HuDongStartTicks;
					if (num4 < (long)(this.FuLuHuDongUseTime * 60000))
					{
						num = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							num,
							num2,
							targetID,
							num3,
							yaoSaiJianYuCount[2]
						}), false);
						return true;
					}
				}
				int unionLevel = Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level);
				int unionLevel2 = kfprisonRoleData.UnionLevel;
				double num5 = Math.Pow((double)(unionLevel + unionLevel2), 1.6);
				int num6 = (int)(num5 * yaoSaiJianYuManorCommandConfig.OwnerFactor);
				int num7 = (int)(num5 * yaoSaiJianYuManorCommandConfig.FuLuFactor);
				num6 -= ((num6 > 100) ? (num6 % 100) : 0);
				num7 -= ((num7 > 100) ? (num7 % 100) : 0);
				int num8 = JunTuanClient.getInstance().YaoSaiPrisonHuDong(num2, targetID, num3, (int)yaoSaiJianYuManorCommandConfig.AwardType, num6, num7);
				if (num8 < 0)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						targetID,
						num3,
						yaoSaiJianYuCount[2]
					}), false);
					return true;
				}
				this.GiveHuDongAward(client, yaoSaiJianYuManorCommandConfig.AwardType, num6);
				List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(list, list.Count, 0, 1.0);
				if (fallGoodsItemByPercent != null && fallGoodsItemByPercent.Count > 0)
				{
					List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
					for (int i = 0; i < goodsDataListFromFallGoodsItemList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataListFromFallGoodsItemList[i].GoodsID, goodsDataListFromFallGoodsItemList[i].GCount, goodsDataListFromFallGoodsItemList[i].Quality, goodsDataListFromFallGoodsItemList[i].Props, goodsDataListFromFallGoodsItemList[i].Forge_level, goodsDataListFromFallGoodsItemList[i].Binding, 0, "", true, 1, "要塞监狱互动", goodsDataListFromFallGoodsItemList[i].Endtime, goodsDataListFromFallGoodsItemList[i].AddPropIndex, goodsDataListFromFallGoodsItemList[i].BornIndex, goodsDataListFromFallGoodsItemList[i].Lucky, goodsDataListFromFallGoodsItemList[i].Strong, goodsDataListFromFallGoodsItemList[i].ExcellenceInfo, goodsDataListFromFallGoodsItemList[i].AppendPropLev, goodsDataListFromFallGoodsItemList[i].ChangeLifeLevForEquip, null, null, 0, true);
					}
				}
				List<int> list2;
				(list2 = yaoSaiJianYuCount)[2] = list2[2] + 1;
				this.SaveYaoSaiJianYuCount(client, yaoSaiJianYuCount);
				this.HuDongStart(client, targetID);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					targetID,
					num3,
					yaoSaiJianYuCount[2]
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessYaoSaiFreeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.RoleID != num2)
				{
					return true;
				}
				List<KFPrisonRoleData> yaoSaiPrisonFuLuData = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(num2);
				if (null == yaoSaiPrisonFuLuData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, targetID), false);
					return true;
				}
				KFPrisonRoleData kfprisonRoleData = yaoSaiPrisonFuLuData.Find((KFPrisonRoleData x) => x.RoleID == targetID);
				if (null == kfprisonRoleData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, targetID), false);
					return true;
				}
				int num3 = JunTuanClient.getInstance().YaoSaiPrisonOpt(num2, targetID, -2, true);
				if (num3 < 0)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, targetID), false);
					return true;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, targetID), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessYaoSaiOptCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				if (client.ClientData.RoleID != num2)
				{
					return true;
				}
				if (targetID == num2)
				{
					return true;
				}
				if (num3 != 0 && num3 != 1 && num3 != 2)
				{
					return true;
				}
				if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
				{
					return true;
				}
				KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(num2);
				KFPrisonJingJiData yaoSaiPrisonJingJiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(targetID);
				KFPrisonRoleData yaoSaiPrisonRoleData2 = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(targetID);
				if (yaoSaiPrisonRoleData == null || yaoSaiPrisonRoleData2 == null || yaoSaiPrisonJingJiData == null || null == yaoSaiPrisonJingJiData.PlayerJingJiMirrorData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						targetID,
						num3
					}), false);
					return true;
				}
				client.ClientData.YaoSaiPrisonOptType = num3;
				client.ClientData.YaoSaiPrisonTargetID = targetID;
				client.ClientData.YaoSaiPrisonTargetName = yaoSaiPrisonRoleData2.RoleName;
				if (num3 == 0 || num3 == 1)
				{
					if (yaoSaiPrisonRoleData.OwnerID == targetID)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
					int unionLevel = Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level);
					if (this.GetYaoSaiLevelID(unionLevel) != this.GetYaoSaiLevelID(yaoSaiPrisonRoleData2.UnionLevel))
					{
						num = -19;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
					List<KFPrisonRoleData> yaoSaiPrisonFuLuData = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(num2);
					bool flag;
					if (yaoSaiPrisonFuLuData != null)
					{
						if (yaoSaiPrisonFuLuData.Count < 4)
						{
							flag = !yaoSaiPrisonFuLuData.Exists((KFPrisonRoleData x) => x.RoleID == targetID);
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						num = -36;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
				}
				if (num3 == 2 || num3 == 1)
				{
					if (yaoSaiPrisonRoleData2.OwnerID == 0 || yaoSaiPrisonRoleData2.OwnerID == num2)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
					yaoSaiPrisonJingJiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(yaoSaiPrisonRoleData2.OwnerID);
					yaoSaiPrisonRoleData2 = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(yaoSaiPrisonRoleData2.OwnerID);
					if (yaoSaiPrisonRoleData2 == null || yaoSaiPrisonRoleData2.RoleID == 0 || yaoSaiPrisonJingJiData == null || null == yaoSaiPrisonJingJiData.PlayerJingJiMirrorData)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
				}
				List<int> list = null;
				List<int> list2 = null;
				lock (this.ConfigMutex)
				{
					list = this.ZhengFuCostList;
					list2 = this.HelpCostList;
				}
				int num4 = 0;
				List<int> yaoSaiJianYuCount = this.GetYaoSaiJianYuCount(client);
				if (num3 == 0 || num3 == 1)
				{
					List<int> list3;
					if (yaoSaiJianYuCount[4] > 0)
					{
						(list3 = yaoSaiJianYuCount)[4] = list3[4] - 1;
					}
					else
					{
						int num5 = yaoSaiJianYuCount[3] - list[0] + 1;
						if (num5 > 0)
						{
							num4 = ((num5 >= list.Count) ? list[list.Count - 1] : list[num5]);
						}
					}
					(list3 = yaoSaiJianYuCount)[3] = list3[3] + 1;
				}
				else
				{
					int num5 = yaoSaiJianYuCount[1] - list2[0] + 1;
					if (num5 > 0)
					{
						num4 = ((num5 >= list2.Count) ? list2[list2.Count - 1] : list2[num5]);
					}
					List<int> list3;
					(list3 = yaoSaiJianYuCount)[1] = list3[1] + 1;
				}
				if (num4 > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num4, "要塞征服抢夺解救", true, true, false, DaiBiSySType.None))
					{
						num = -10;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							targetID,
							num3
						}), false);
						return true;
					}
				}
				PlayerJingJiData beChallengerData = DataHelper.BytesToObject<PlayerJingJiData>(yaoSaiPrisonJingJiData.PlayerJingJiMirrorData, 0, yaoSaiPrisonJingJiData.PlayerJingJiMirrorData.Length);
				JingJiChangManager.getInstance().enterJingJiChang(client, beChallengerData, JingJiFuBenType.YAOSAI);
				this.UpdateYaoSaiPrisonRoleData(client);
				this.SaveYaoSaiJianYuCount(client, yaoSaiJianYuCount);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					targetID,
					num3
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuyZhengFuCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				if (client.ClientData.RoleID != num2)
				{
					return true;
				}
				List<int> yaoSaiJianYuCount = this.GetYaoSaiJianYuCount(client);
				if (yaoSaiJianYuCount[4] > 0)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, yaoSaiJianYuCount[4]), false);
					return true;
				}
				List<int> list = null;
				lock (this.ConfigMutex)
				{
					list = this.ZhengFuCostList;
				}
				int num3 = 0;
				int num4 = yaoSaiJianYuCount[3] - list[0] + 1;
				if (num4 > 0)
				{
					num3 = ((num4 >= list.Count) ? list[list.Count - 1] : list[num4]);
				}
				if (num3 > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, "要塞征服抢夺解救", true, true, false, DaiBiSySType.None))
					{
						num = -10;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, yaoSaiJianYuCount[4]), false);
						return true;
					}
				}
				List<int> list2;
				(list2 = yaoSaiJianYuCount)[4] = list2[4] + 1;
				this.SaveYaoSaiJianYuCount(client, yaoSaiJianYuCount);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, yaoSaiJianYuCount[4]), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetYaoSaiDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				if (num == 0)
				{
					YaoSaiWorldData yaoSaiWorldData = new YaoSaiWorldData();
					if (this.ManorSearchCost > 0 && Global.GetTotalBindTongQianAndTongQianVal(client) < this.ManorSearchCost)
					{
						client.sendCmd<YaoSaiWorldData>(nID, yaoSaiWorldData, false);
						return true;
					}
					HashSet<int> hashSet = new HashSet<int>();
					if (null != client.ClientData.FriendDataList)
					{
						foreach (FriendData friendData in client.ClientData.FriendDataList)
						{
							if (friendData.FriendType == 0)
							{
								hashSet.Add(friendData.OtherRoleID);
							}
						}
					}
					KFPrisonRoleData kfprisonRoleData = JunTuanClient.getInstance().SearchYaoSaiFuLu(client.ClientData.RoleID, Global.GetUnionLevel2(client), client.ClientData.Faction, hashSet);
					if (kfprisonRoleData != null && this.ManorSearchCost > 0)
					{
						if (!Global.SubBindTongQianAndTongQian(client, this.ManorSearchCost, "世界战役搜索"))
						{
							return true;
						}
					}
					this.BuildYaoSaiWolrdDataForClient(client, yaoSaiWorldData, kfprisonRoleData);
					client.sendCmd<YaoSaiWorldData>(nID, yaoSaiWorldData, false);
				}
				else
				{
					KFPrisonRoleData kfprisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(num);
					YaoSaiWorldData yaoSaiWorldData = new YaoSaiWorldData();
					this.BuildYaoSaiWolrdDataForClient(client, yaoSaiWorldData, kfprisonRoleData);
					client.sendCmd<YaoSaiWorldData>(nID, yaoSaiWorldData, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private void GiveHuDongAward(GameClient client, PrisonAwardType awardType, int addNum)
		{
			if (addNum > 0)
			{
				if (awardType == PrisonAwardType.mojing)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, addNum, "要塞征服互动奖励", true, true, false);
				}
				else if (awardType == PrisonAwardType.xinghun)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, addNum, "要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.chengjiu)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, addNum, "GM要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.shengwang)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, addNum, "要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.zhangong)
				{
					GameManager.ClientMgr.AddBangGong(client, ref addNum, AddBangGongTypes.YaoSaiJianYu, 0);
				}
			}
		}

		private void processWin(GameClient player, Robot robot)
		{
			if (player.ClientData.YaoSaiPrisonOptType == 2 || player.ClientData.YaoSaiPrisonOptType == 1)
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, player.ClientData.YaoSaiPrisonTargetID, player.ClientData.YaoSaiPrisonOptType, true);
			}
			else
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, robot.PlayerId, player.ClientData.YaoSaiPrisonOptType, true);
			}
			string arg;
			if (player.ClientData.YaoSaiPrisonOptType == 2)
			{
				arg = player.ClientData.YaoSaiPrisonTargetName;
			}
			else
			{
				arg = robot.getRoleDataMini().RoleName;
			}
			player.sendCmd(1526, string.Format("{0}:{1}:{2}", player.ClientData.YaoSaiPrisonOptType, 1, arg), false);
		}

		private void processFailed(GameClient player, Robot robot)
		{
			if (player.ClientData.YaoSaiPrisonOptType == 2 || player.ClientData.YaoSaiPrisonOptType == 1)
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, player.ClientData.YaoSaiPrisonTargetID, player.ClientData.YaoSaiPrisonOptType, false);
			}
			else
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, robot.PlayerId, player.ClientData.YaoSaiPrisonOptType, false);
			}
			string arg;
			if (player.ClientData.YaoSaiPrisonOptType == 2)
			{
				arg = player.ClientData.YaoSaiPrisonTargetName;
			}
			else
			{
				arg = robot.getRoleDataMini().RoleName;
			}
			player.sendCmd(1526, string.Format("{0}:{1}:{2}", player.ClientData.YaoSaiPrisonOptType, 0, arg), false);
		}

		public int GetYaoSaiJianYuState(int roleID, int unionLev = 0)
		{
			int result;
			if (this.ManorFriendListOpenUnionLev != 0 && unionLev > 0 && unionLev < this.ManorFriendListOpenUnionLev)
			{
				result = 1;
			}
			else
			{
				KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(roleID);
				if (null == yaoSaiPrisonRoleData)
				{
					result = 0;
				}
				else if (yaoSaiPrisonRoleData.OwnerID != 0)
				{
					result = 2;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}

		public void UpdateYaoSaiPrisonRoleData(GameClient client)
		{
			KFUpdatePrisonRole kfupdatePrisonRole = new KFUpdatePrisonRole();
			kfupdatePrisonRole.RoleID = client.ClientData.RoleID;
			kfupdatePrisonRole.RoleName = client.ClientData.RoleName;
			kfupdatePrisonRole.UnionLevel = Global.GetUnionLevel2(client);
			kfupdatePrisonRole.Faction = client.ClientData.Faction;
			kfupdatePrisonRole.RoleSex = (byte)client.ClientData.RoleSex;
			kfupdatePrisonRole.Occupation = (byte)client.ClientData.Occupation;
			kfupdatePrisonRole.CombatForce = client.ClientData.CombatForce;
			kfupdatePrisonRole.ZoneID = client.ClientData.ZoneID;
			PlayerJingJiData playerJingJiData = JingJiChangManager.getInstance().createJingJiData(client);
			kfupdatePrisonRole.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(playerJingJiData);
			JunTuanClient.getInstance().UpdateYaoSaiPrisonRoleData(kfupdatePrisonRole);
		}

		private void OnLogin(GameClient client)
		{
			this.UpdateYaoSaiLogData(client.ClientData.RoleID);
		}

		private void OnLogout(GameClient client)
		{
			JunTuanClient.getInstance().ClearYaoSaiPrisonData(client.ClientData.RoleID);
		}

		public void UpdateYaoSaiLogData(int roleID)
		{
			lock (this.ConfigMutex)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				if (null != gameClient)
				{
					List<KFPrisonLogData> yaoSaiPrisonLogData = JunTuanClient.getInstance().GetYaoSaiPrisonLogData(gameClient.ClientData.RoleID);
					if (null != yaoSaiPrisonLogData)
					{
						foreach (KFPrisonLogData kfprisonLogData in yaoSaiPrisonLogData)
						{
							if (kfprisonLogData.State == 0)
							{
								int num = JunTuanClient.getInstance().UpdateYaoSaiPrisonLogData(gameClient.ClientData.RoleID, kfprisonLogData.ID, 1);
								if (num < 0)
								{
									break;
								}
								this.GiveHuDongAward(gameClient, (PrisonAwardType)kfprisonLogData.Param1, kfprisonLogData.Param2);
							}
						}
					}
				}
			}
		}

		private long GetRevoltCD(GameClient client)
		{
			return Global.GetRoleParamsInt64FromDB(client, "10183");
		}

		private void SetRevoltCD(GameClient client, long time)
		{
			Global.SaveRoleParamsInt64ValueToDB(client, "10183", time, true);
		}

		public int GetYaoSaiLevelID(int unionlev)
		{
			Dictionary<int, YaoSaiJianYuManorLevelConfig> dictionary = null;
			lock (this.ConfigMutex)
			{
				dictionary = this.LevelConfigDict;
			}
			int result = 0;
			foreach (YaoSaiJianYuManorLevelConfig yaoSaiJianYuManorLevelConfig in dictionary.Values)
			{
				int num = yaoSaiJianYuManorLevelConfig.MinZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MinLevel;
				int num2 = yaoSaiJianYuManorLevelConfig.MaxZhuanSheng * 100 + yaoSaiJianYuManorLevelConfig.MaxLevel;
				if (unionlev >= num && unionlev <= num2)
				{
					result = yaoSaiJianYuManorLevelConfig.ID;
					break;
				}
			}
			return result;
		}

		private void FilterHuDongData(GameClient client, List<PrisonHuDongData> hudongDataList, List<KFPrisonRoleData> myFuLuListData)
		{
			List<int> list = new List<int>();
			using (List<PrisonHuDongData>.Enumerator enumerator = hudongDataList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PrisonHuDongData item = enumerator.Current;
					KFPrisonRoleData kfprisonRoleData = myFuLuListData.Find((KFPrisonRoleData x) => x.RoleID == item.RoleID);
					if (null == kfprisonRoleData)
					{
						list.Add(item.RoleID);
					}
				}
			}
			using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					int item = enumerator2.Current;
					hudongDataList.RemoveAll((PrisonHuDongData x) => x.RoleID == item);
				}
			}
			if (list.Count != 0)
			{
				this.SaveHuDongData(client, hudongDataList);
			}
		}

		private List<PrisonHuDongData> GetHuDongData(GameClient client)
		{
			List<PrisonHuDongData> list = new List<PrisonHuDongData>();
			List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "20021");
			for (int i = 0; i < roleParamsUlongListFromDB.Count - 1; i += 2)
			{
				PrisonHuDongData item = new PrisonHuDongData
				{
					RoleID = (int)roleParamsUlongListFromDB[i],
					HuDongStartTicks = (long)roleParamsUlongListFromDB[i + 1]
				};
				list.Add(item);
			}
			return list;
		}

		private void SaveHuDongData(GameClient client, List<PrisonHuDongData> hudongDataList)
		{
			List<ulong> list = new List<ulong>();
			foreach (PrisonHuDongData prisonHuDongData in hudongDataList)
			{
				list.Add((ulong)((long)prisonHuDongData.RoleID));
				list.Add((ulong)prisonHuDongData.HuDongStartTicks);
			}
			Global.SaveRoleParamsUlongListToDB(client, list, "20021", true);
		}

		private void HuDongStart(GameClient client, int fuluID)
		{
			long huDongStartTicks = TimeUtil.NOW();
			List<PrisonHuDongData> huDongData = this.GetHuDongData(client);
			PrisonHuDongData prisonHuDongData = huDongData.Find((PrisonHuDongData x) => x.RoleID == fuluID);
			if (null != prisonHuDongData)
			{
				prisonHuDongData.HuDongStartTicks = huDongStartTicks;
			}
			else
			{
				huDongData.Add(new PrisonHuDongData
				{
					RoleID = fuluID,
					HuDongStartTicks = huDongStartTicks
				});
			}
			this.SaveHuDongData(client, huDongData);
		}

		private List<int> GetYaoSaiJianYuCount(GameClient client)
		{
			List<int> list = null;
			lock (this.ConfigMutex)
			{
				list = this.ZhengFuCostList;
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "20020");
			if (roleParamsIntListFromDB.Count != 5)
			{
				roleParamsIntListFromDB.Clear();
				roleParamsIntListFromDB.Add(offsetDay);
				roleParamsIntListFromDB.Add(0);
				roleParamsIntListFromDB.Add(0);
				roleParamsIntListFromDB.Add(0);
				roleParamsIntListFromDB.Add(list[0]);
			}
			if (roleParamsIntListFromDB[0] != offsetDay)
			{
				roleParamsIntListFromDB[0] = offsetDay;
				roleParamsIntListFromDB[1] = 0;
				roleParamsIntListFromDB[2] = 0;
				roleParamsIntListFromDB[3] = 0;
				roleParamsIntListFromDB[4] = list[0];
			}
			return roleParamsIntListFromDB;
		}

		private void SaveYaoSaiJianYuCount(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "20020", true);
		}

		private List<PrisonLogData> BuildYaoSaiLogDataForClient(GameClient client)
		{
			List<PrisonLogData> list = new List<PrisonLogData>();
			List<KFPrisonLogData> yaoSaiPrisonLogData = JunTuanClient.getInstance().GetYaoSaiPrisonLogData(client.ClientData.RoleID);
			List<PrisonLogData> result;
			if (null == yaoSaiPrisonLogData)
			{
				result = list;
			}
			else
			{
				foreach (KFPrisonLogData kfprisonLogData in yaoSaiPrisonLogData)
				{
					PrisonLogData item = new PrisonLogData
					{
						ID = kfprisonLogData.IntroID,
						Name1 = kfprisonLogData.Name1,
						Name2 = kfprisonLogData.Name2,
						JiangLiType = kfprisonLogData.Param1,
						JiangLiCount = kfprisonLogData.Param2
					};
					list.Add(item);
				}
				result = list;
			}
			return result;
		}

		private YaoSaiWorldData BuildYaoSaiWolrdDataForClient(GameClient client, YaoSaiWorldData worldData, KFPrisonRoleData kfpRoleData)
		{
			List<int> yaoSaiJianYuCount = this.GetYaoSaiJianYuCount(client);
			worldData.jiejiuCount = yaoSaiJianYuCount[1];
			worldData.zhenfuCount = yaoSaiJianYuCount[3];
			worldData.zhenfuLeftCount = yaoSaiJianYuCount[4];
			YaoSaiWorldData result;
			if (null == kfpRoleData)
			{
				result = worldData;
			}
			else
			{
				worldData.state = ((kfpRoleData.OwnerID != 0) ? 1 : 0);
				this.TransKFPrisonRoleDataToPrisonRoleData(kfpRoleData, worldData.Mine);
				if (kfpRoleData.OwnerID != 0)
				{
					KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(kfpRoleData.OwnerID);
					this.TransKFPrisonRoleDataToPrisonRoleData(yaoSaiPrisonRoleData, worldData.Master);
				}
				List<KFPrisonRoleData> yaoSaiPrisonFuLuData = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(kfpRoleData.RoleID);
				if (null != yaoSaiPrisonFuLuData)
				{
					foreach (KFPrisonRoleData kfprisonRoleData in yaoSaiPrisonFuLuData)
					{
						PrisonFuLuData prisonFuLuData = new PrisonFuLuData();
						prisonFuLuData.RoleID = kfprisonRoleData.RoleID;
						prisonFuLuData.Name = kfprisonRoleData.RoleName;
						prisonFuLuData.Level = (kfprisonRoleData.UnionLevel - 1) % 100 + 1;
						prisonFuLuData.ChangeLevel = (kfprisonRoleData.UnionLevel - 1) / 100;
						prisonFuLuData.ZoneID = kfprisonRoleData.ZoneID;
						prisonFuLuData.Occupation = (int)kfprisonRoleData.Occupation;
						prisonFuLuData.RoleSex = (int)kfprisonRoleData.RoleSex;
						worldData.FuLuList.Add(prisonFuLuData);
					}
				}
				result = worldData;
			}
			return result;
		}

		private void TransKFPrisonRoleDataToPrisonRoleData(KFPrisonRoleData kfpRoleData, PrisonRoleData prisonRoleData)
		{
			if (kfpRoleData != null && null != prisonRoleData)
			{
				prisonRoleData.RoleID = kfpRoleData.RoleID;
				prisonRoleData.Name = kfpRoleData.RoleName;
				prisonRoleData.Level = (kfpRoleData.UnionLevel - 1) % 100 + 1;
				prisonRoleData.ChangeLevel = (kfpRoleData.UnionLevel - 1) / 100;
				prisonRoleData.ZoneID = kfpRoleData.ZoneID;
				prisonRoleData.Occupation = (int)kfpRoleData.Occupation;
				prisonRoleData.RoleSex = (int)kfpRoleData.RoleSex;
				prisonRoleData.CombatForce = kfpRoleData.CombatForce;
			}
		}

		private PrisonMainData BuildYaoSaiMainDataForClient(GameClient client)
		{
			PrisonMainData prisonMainData = new PrisonMainData();
			KFPrisonRoleData kfprisonRoleData = null;
			List<KFPrisonRoleData> list = null;
			KFPrisonRoleData yaoSaiPrisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
			if (yaoSaiPrisonRoleData != null && yaoSaiPrisonRoleData.OwnerID > 0)
			{
				kfprisonRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(yaoSaiPrisonRoleData.OwnerID);
			}
			if (yaoSaiPrisonRoleData != null && yaoSaiPrisonRoleData.RoleID == client.ClientData.RoleID)
			{
				list = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(client.ClientData.RoleID);
			}
			List<int> yaoSaiJianYuCount = this.GetYaoSaiJianYuCount(client);
			prisonMainData.JieJiuCount = yaoSaiJianYuCount[1];
			prisonMainData.ZhengFuCount = yaoSaiJianYuCount[3];
			prisonMainData.ZhengFuLeftCount = yaoSaiJianYuCount[4];
			prisonMainData.LaoDongCount = yaoSaiJianYuCount[2];
			long num = TimeUtil.NOW() - this.GetRevoltCD(client);
			long num2 = (long)(60000 * this.FightCDTime);
			prisonMainData.RevoltCD = ((num >= num2) ? 0L : (num2 - num));
			if (yaoSaiPrisonRoleData != null && yaoSaiPrisonRoleData.RoleID == client.ClientData.RoleID)
			{
				if (yaoSaiPrisonRoleData.OwnerID == 0 || kfprisonRoleData == null || 0 == kfprisonRoleData.RoleID)
				{
					this.TransKFPrisonRoleDataToPrisonRoleData(yaoSaiPrisonRoleData, prisonMainData.roleData);
					prisonMainData.MineFuLuState = 0;
				}
				else
				{
					this.TransKFPrisonRoleDataToPrisonRoleData(kfprisonRoleData, prisonMainData.roleData);
					prisonMainData.MineFuLuState = 1;
				}
			}
			if (null != list)
			{
				long num3 = TimeUtil.NOW();
				List<PrisonHuDongData> huDongData = this.GetHuDongData(client);
				this.FilterHuDongData(client, huDongData, list);
				list.Sort(delegate(KFPrisonRoleData left, KFPrisonRoleData right)
				{
					int result;
					if (left.FuLuTime < right.FuLuTime)
					{
						result = -1;
					}
					else if (left.FuLuTime > right.FuLuTime)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				foreach (KFPrisonRoleData kfprisonRoleData2 in list)
				{
					PrisonFuLuData fuluData = new PrisonFuLuData();
					fuluData.RoleID = kfprisonRoleData2.RoleID;
					fuluData.Name = kfprisonRoleData2.RoleName;
					fuluData.Level = (kfprisonRoleData2.UnionLevel - 1) % 100 + 1;
					fuluData.ChangeLevel = (kfprisonRoleData2.UnionLevel - 1) / 100;
					fuluData.ZoneID = kfprisonRoleData2.ZoneID;
					fuluData.Occupation = (int)kfprisonRoleData2.Occupation;
					fuluData.RoleSex = (int)kfprisonRoleData2.RoleSex;
					PrisonHuDongData prisonHuDongData = huDongData.Find((PrisonHuDongData x) => x.RoleID == fuluData.RoleID);
					if (null != prisonHuDongData)
					{
						long num4 = num3 - prisonHuDongData.HuDongStartTicks;
						if (num4 < (long)(this.FuLuHuDongUseTime * 60000))
						{
							fuluData.LaoDongState = 1;
							fuluData.LaoDongTime = (long)(this.FuLuHuDongUseTime * 60000) - num4;
						}
					}
					prisonMainData.fuLuDataList.Add(fuluData);
				}
			}
			return prisonMainData;
		}

		public int GetManorFriendListOpenUnionLev()
		{
			return this.ManorFriendListOpenUnionLev;
		}

		public bool InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ManorFriendListOpen");
			string[] array = paramValueByName.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.ManorFriendListOpenUnionLev = Global.GetUnionLevel2(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]));
			}
			List<int> list = new List<int>();
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("ManorCatch");
			string[] array2 = paramValueByName2.Split(new char[]
			{
				','
			});
			foreach (string str in array2)
			{
				list.Add(Global.SafeConvertToInt32(str));
			}
			lock (this.ConfigMutex)
			{
				this.ZhengFuCostList = list;
			}
			List<int> list2 = new List<int>();
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("ManorHelp");
			string[] array4 = paramValueByName3.Split(new char[]
			{
				','
			});
			foreach (string str in array4)
			{
				list2.Add(Global.SafeConvertToInt32(str));
			}
			lock (this.ConfigMutex)
			{
				this.HelpCostList = list2;
			}
			string paramValueByName4 = GameManager.systemParamsList.GetParamValueByName("ManorCommandAward");
			string[] array5 = paramValueByName4.Split(new char[]
			{
				','
			});
			if (array5.Length == 3)
			{
				this.FuLuHuDongCount = Global.SafeConvertToInt32(array5[0]);
				this.FuLuAwardCount = Global.SafeConvertToInt32(array5[1]);
				this.FuLuHuDongUseTime = Global.SafeConvertToInt32(array5[2]);
			}
			string paramValueByName5 = GameManager.systemParamsList.GetParamValueByName("ManorCommandAgainst");
			if (null != paramValueByName5)
			{
				string[] array6 = paramValueByName5.Split(new char[]
				{
					','
				});
				if (array6.Length == 2)
				{
					this.FightCDTime = Global.SafeConvertToInt32(array6[0]);
					this.FightCDClearCost = Global.SafeConvertToInt32(array6[1]);
				}
			}
			string paramValueByName6 = GameManager.systemParamsList.GetParamValueByName("ManorSearchCost");
			this.ManorSearchCost = Global.SafeConvertToInt32(paramValueByName6);
			this.LoadFallGoodsItemList();
			return this.LoadYaoSaiJianYuManorLevelConfig() && this.LoadYaoSaiJianYuCommandConfigFile();
		}

		public void LoadFallGoodsItemList()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ManorWorkAward");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				List<FallGoodsItem> list = new List<FallGoodsItem>();
				int num = 0;
				FallGoodsItem fallGoodsItem = null;
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (!(text == ""))
					{
						string[] array2 = text.Split(new char[]
						{
							','
						});
						if (array2.Length == 7)
						{
							fallGoodsItem = null;
							try
							{
								fallGoodsItem = new FallGoodsItem
								{
									GoodsID = Convert.ToInt32(array2[0]),
									BasePercent = num,
									SelfPercent = (int)(Convert.ToDouble(array2[1]) * 100000.0),
									Binding = Convert.ToInt32(array2[2]),
									LuckyRate = (int)Convert.ToDouble(array2[3]),
									FallLevelID = Convert.ToInt32(array2[4]),
									ZhuiJiaID = Convert.ToInt32(array2[5]),
									ExcellencePropertyID = Convert.ToInt32(array2[6])
								};
								num += fallGoodsItem.SelfPercent;
							}
							catch (Exception)
							{
								fallGoodsItem = null;
							}
							if (null == fallGoodsItem)
							{
								LogManager.WriteLog(2, string.Format("解析要塞监狱掉落项时发生错误", new object[0]), null, true);
							}
							else
							{
								list.Add(fallGoodsItem);
								if (num > 100000)
								{
									LogManager.WriteLog(2, string.Format("解析要塞监狱掉落项时发生概率溢出100000错误", new object[0]), null, true);
								}
							}
						}
					}
				}
				lock (this.ConfigMutex)
				{
					this.FallGoodsItemConfigList = list;
				}
			}
		}

		public bool LoadYaoSaiJianYuManorLevelConfig()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ManorLevel.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ManorLevel.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, YaoSaiJianYuManorLevelConfig> dictionary = new Dictionary<int, YaoSaiJianYuManorLevelConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					YaoSaiJianYuManorLevelConfig yaoSaiJianYuManorLevelConfig = new YaoSaiJianYuManorLevelConfig();
					yaoSaiJianYuManorLevelConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "MinLevel");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length == 2)
					{
						yaoSaiJianYuManorLevelConfig.MinZhuanSheng = Global.SafeConvertToInt32(array[0]);
						yaoSaiJianYuManorLevelConfig.MinLevel = Global.SafeConvertToInt32(array[1]);
					}
					safeAttributeStr = Global.GetSafeAttributeStr(xml, "MaxLevel");
					array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length == 2)
					{
						yaoSaiJianYuManorLevelConfig.MaxZhuanSheng = Global.SafeConvertToInt32(array[0]);
						yaoSaiJianYuManorLevelConfig.MaxLevel = Global.SafeConvertToInt32(array[1]);
					}
					dictionary[yaoSaiJianYuManorLevelConfig.ID] = yaoSaiJianYuManorLevelConfig;
				}
				lock (this.ConfigMutex)
				{
					this.LevelConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/ManorLevel.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadYaoSaiJianYuCommandConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ManorCommand.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ManorCommand.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, YaoSaiJianYuManorCommandConfig> dictionary = new Dictionary<int, YaoSaiJianYuManorCommandConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					YaoSaiJianYuManorCommandConfig yaoSaiJianYuManorCommandConfig = new YaoSaiJianYuManorCommandConfig();
					yaoSaiJianYuManorCommandConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Award");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length == 3)
					{
						yaoSaiJianYuManorCommandConfig.AwardType = (PrisonAwardType)Global.SafeConvertToInt32(array[0]);
						yaoSaiJianYuManorCommandConfig.OwnerFactor = Global.SafeConvertToDouble(array[1]);
						yaoSaiJianYuManorCommandConfig.FuLuFactor = Global.SafeConvertToDouble(array[2]);
					}
					dictionary[yaoSaiJianYuManorCommandConfig.ID] = yaoSaiJianYuManorCommandConfig;
				}
				lock (this.ConfigMutex)
				{
					this.CommandAwardConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/ManorCommand.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string YaoSaiJianYu_ManorCommandFileName = "Config/ManorCommand.xml";

		private const string YaoSaiJianYu_ManorLevelFileName = "Config/ManorLevel.xml";

		public const int YaoSaiPrisonFuLuMaxNum = 4;

		private object ConfigMutex = new object();

		private int ManorFriendListOpenUnionLev = 0;

		private List<int> ZhengFuCostList = new List<int>();

		private List<int> HelpCostList = new List<int>();

		private int FuLuHuDongCount = 20;

		private int FuLuAwardCount = 20;

		private int FuLuHuDongUseTime = 30;

		private int FightCDTime = 20;

		private int FightCDClearCost = 20;

		private int ManorSearchCost = 10000;

		private Dictionary<int, YaoSaiJianYuManorCommandConfig> CommandAwardConfigDict = new Dictionary<int, YaoSaiJianYuManorCommandConfig>();

		public Dictionary<int, YaoSaiJianYuManorLevelConfig> LevelConfigDict = new Dictionary<int, YaoSaiJianYuManorLevelConfig>();

		public List<FallGoodsItem> FallGoodsItemConfigList = new List<FallGoodsItem>();

		private static YaoSaiJianYuManager instance = new YaoSaiJianYuManager();
	}
}
