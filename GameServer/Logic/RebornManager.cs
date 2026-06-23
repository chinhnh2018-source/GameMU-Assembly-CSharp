using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.MUWings;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class RebornManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener
	{
		public static RebornManager getInstance()
		{
			return RebornManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig(false);
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("RebornManager.TimerProc", new EventHandler(this.TimerProc)), 2000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1710, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1712, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1713, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1714, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2030, 4, 4, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2031, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2032, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2033, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2046, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2047, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2051, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2053, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2059, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2050, 4, 4, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2054, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2055, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2056, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2057, 6, 6, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2058, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2060, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2095, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2096, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(14, RebornManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(14, RebornManager.getInstance());
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
			bool flag = GlobalNew.IsGongNengOpened(client, 105, false);
			if (2046 == nID || 2060 == nID)
			{
				flag = true;
			}
			bool result;
			if (!flag)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1710:
					return this.ProcessRebornUpgradeCmd(client, nID, bytes, cmdParams);
				case 1711:
					break;
				case 1712:
					return this.ProcessRebornAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1713:
					return this.ProcessRebornAdmireCmd(client, nID, bytes, cmdParams);
				case 1714:
					return this.ProcessRebornRankDataCmd(client, nID, bytes, cmdParams);
				default:
					switch (nID)
					{
					case 2030:
						if (cmdParams == null || cmdParams.Length != 4)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int stampID = Convert.ToInt32(cmdParams[1]);
							int stampType = Convert.ToInt32(cmdParams[2]);
							int upNum = Convert.ToInt32(cmdParams[3]);
							int num3;
							int num4;
							int num5;
							int num2 = Convert.ToInt32(RebornStamp.ProcessRebornYinJiLevelUp(client, num, stampID, stampType, upNum, out num3, out num4, out num5));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								num2,
								num3,
								num4,
								num5
							}), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_YINJI_LEVELUP", false, false);
						}
						break;
					case 2031:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							RebornStampData rebornStampData;
							int num2 = Convert.ToInt32(RebornStamp.ProcessRebornYinJiGetInfo(client, num, out rebornStampData));
							client.ClientData.RebornYinJi = rebornStampData;
							client.sendCmd<RebornStampData>(nID, rebornStampData, false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_YINJI_GETINFO", false, false);
						}
						break;
					case 2032:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int num2 = Convert.ToInt32(RebornStamp.ProcessRebornYinJiReset(client, num));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_YINJI_RESET", false, false);
						}
						break;
					case 2033:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int stampType2 = Convert.ToInt32(cmdParams[1]);
							int stampType3 = Convert.ToInt32(cmdParams[2]);
							int num2 = Convert.ToInt32(RebornStamp.ProcessRebornYinJiChoose(client, num, stampType2, stampType3));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_YINJI_CHOOSE", false, false);
						}
						break;
					case 2034:
					case 2035:
					case 2036:
					case 2037:
					case 2038:
					case 2039:
					case 2040:
					case 2041:
					case 2042:
					case 2043:
					case 2044:
					case 2045:
					case 2048:
					case 2049:
					case 2052:
						break;
					case 2046:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							string strGoodsID = cmdParams[1];
							int num2 = Convert.ToInt32(RebornEquip.SaleRebornEquipProcess(client, num, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_SALEONE", false, false);
						}
						break;
					case 2047:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							string strGoodsID = cmdParams[1];
							int num2 = Convert.ToInt32(RebornEquip.SaleStoreRebornEquipProcess(client, num, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_SALEMORE", false, false);
						}
						break;
					case 2050:
						if (cmdParams == null || cmdParams.Length != 4)
						{
							return false;
						}
						try
						{
							int dbid = Convert.ToInt32(cmdParams[0]);
							int number = Convert.ToInt32(cmdParams[1]);
							int bind = Convert.ToInt32(cmdParams[2]);
							int isReset = Convert.ToInt32(cmdParams[3]);
							string arg;
							int num6;
							int num2 = Convert.ToInt32(RebornStone.ProessMakeRebornEquipHold(client, dbid, bind, isReset, number, out arg, out num6));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, arg, num6), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_DAKONG", false, false);
						}
						break;
					case 2051:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int num2 = Convert.ToInt32(RebornEquip.RebornEquipShowProcess(client, num));
							client.sendCmd(nID, string.Format("{0}:{1}", num2, client.ClientData.RebornShowEquip), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_SHOW", false, false);
						}
						break;
					case 2053:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int dbid = Convert.ToInt32(cmdParams[1]);
							int num2 = Convert.ToInt32(RebornEquip.RebornEquipAdvanceProcess(client, num, dbid));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_UPDATA", false, false);
						}
						break;
					case 2054:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int equipDBID = Convert.ToInt32(cmdParams[0]);
							int stoneDBID = Convert.ToInt32(cmdParams[1]);
							int number = Convert.ToInt32(cmdParams[2]);
							string arg2 = "";
							int num6;
							int num2 = Convert.ToInt32(RebornStone.ProessRebornStoneInlayHold(client, equipDBID, stoneDBID, number, out arg2, out num6));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, arg2, num6), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_INLAY", false, false);
						}
						break;
					case 2055:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int equipDBID = Convert.ToInt32(cmdParams[0]);
							int number2 = Convert.ToInt32(cmdParams[1]);
							string arg2 = "";
							int num2 = Convert.ToInt32(RebornStone.ProessRebornStoneDisInlayHold(client, equipDBID, number2, out arg2));
							client.sendCmd(nID, string.Format("{0}:{1}", num2, arg2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_DEMOUNT", false, false);
						}
						break;
					case 2056:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int goodID = Convert.ToInt32(cmdParams[0]);
							int count = Convert.ToInt32(cmdParams[1]);
							int num2 = Convert.ToInt32(RebornStone.ProessRebornStoneComplex(client, goodID, count));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_STONECOMPLEX", false, false);
						}
						break;
					case 2057:
						if (cmdParams == null || cmdParams.Length != 6)
						{
							return false;
						}
						try
						{
							int dbid2 = Convert.ToInt32(cmdParams[0]);
							int num7 = Convert.ToInt32(cmdParams[1]);
							int dbid3 = Convert.ToInt32(cmdParams[2]);
							int num8 = Convert.ToInt32(cmdParams[3]);
							int dbid4 = Convert.ToInt32(cmdParams[4]);
							int num9 = Convert.ToInt32(cmdParams[5]);
							int num2 = Convert.ToInt32(RebornStone.RebornXuanCaiComplexStone(client, dbid2, num7, dbid3, num8, dbid4, num9));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_XUANCAICOMPLEX", false, false);
						}
						break;
					case 2058:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int goodsID = Convert.ToInt32(cmdParams[0]);
							int count = Convert.ToInt32(cmdParams[1]);
							int bind = Convert.ToInt32(cmdParams[2]);
							int num2 = Convert.ToInt32(RebornStone.RebornStoneResolve(client, goodsID, count, bind));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_STONERESOLVE", false, false);
						}
						break;
					case 2059:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int num = Convert.ToInt32(cmdParams[0]);
							int num2 = Convert.ToInt32(RebornEquip.RebornEquipShowModelProcess(client, num));
							client.sendCmd(nID, string.Format("{0}:{1}", num2, client.ClientData.RebornShowModel), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_SHOWMODEL", false, false);
						}
						break;
					case 2060:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							string strGoodsID = cmdParams[0];
							int num2 = Convert.ToInt32(RebornStone.SaleRebornStoneProcess(client, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", num2), false);
						}
						catch (Exception e)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORN_BATCH_STONERESOLVE", false, false);
						}
						break;
					default:
						switch (nID)
						{
						case 2095:
							if (cmdParams == null || cmdParams.Length != 1)
							{
								return false;
							}
							try
							{
								int holeSite = Convert.ToInt32(cmdParams[0]);
								int num10;
								int num2 = Convert.ToInt32(RebornEquip.RebornEquipHolePerfusionProcess(client, holeSite, out num10));
								client.sendCmd(nID, string.Format("{0}:{1}", num2, num10), false);
							}
							catch (Exception e)
							{
								client.sendCmd(nID, "-1", false);
								DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORNHOLE_PERFUSION", false, false);
							}
							break;
						case 2096:
							if (cmdParams == null || cmdParams.Length != 1)
							{
								return false;
							}
							try
							{
								int holeSite = Convert.ToInt32(cmdParams[0]);
								int num10;
								int num11;
								int num2 = Convert.ToInt32(RebornEquip.RebornEquipHoleAbschreckenProcess(client, holeSite, out num11, out num10));
								client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, num11, num10), false);
							}
							catch (Exception e)
							{
								client.sendCmd(nID, "-1", false);
								DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REBORNHOLE_ABSCHRECKEN", false, false);
							}
							break;
						}
						break;
					}
					break;
				}
				result = true;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
		}

		public bool ProcessRebornUpgradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				Dictionary<int, RebornStageInfo> rebornStageInfoDict = this.RebornStageInfoDict;
				string cmdData;
				if (client.ClientData.RebornCount >= rebornStageInfoDict.Count)
				{
					cmdData = string.Format("{0}:{1}:{2}", -12, num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (client.ClientData.HideGM > 0)
				{
					cmdData = string.Format("{0}:{1}:{2}", -12, num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int rebornCount = client.ClientData.RebornCount;
				string resList = "";
				RebornStageInfo rebornStageInfo;
				if (!rebornStageInfoDict.TryGetValue(client.ClientData.RebornCount + 1, out rebornStageInfo))
				{
					cmdData = string.Format("{0}:{1}:{2}", -30, num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int num2 = this.CheckRebornUpgradeLimit(client);
				if (num2 != 0)
				{
					cmdData = string.Format("{0}:{1}:{2}", -1, num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				KuaFuWorldRoleData kuaFuWorldRoleData = new KuaFuWorldRoleData
				{
					LocalRoleID = client.ClientData.LocalRoleID,
					UserID = client.strUserID,
					WorldRoleID = client.ClientData.WorldRoleID,
					Channel = client.ClientData.Channel,
					PTID = client.ClientData.ServerPTID,
					ServerID = client.ServerId,
					ZoneID = client.ClientData.ZoneID
				};
				int num3 = KuaFuWorldClient.getInstance().RegPTKuaFuRoleData(ref kuaFuWorldRoleData);
				if (num3 < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num3, num, client.ClientData.RebornCount), false);
					return true;
				}
				num3 = KuaFuWorldClient.getInstance().Reborn_RoleReborn(client.ClientData.ServerPTID, client.ClientData.RoleID, client.ClientData.RoleName, 1);
				if (num3 < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num3, num, client.ClientData.RebornCount), false);
					return true;
				}
				if (0 == client.ClientData.RebornCount)
				{
					client.ClientData.RebornLevel = 1;
				}
				client.ClientData.RebornCount++;
				lock (client.ClientData.PropPointMutex)
				{
					if (rebornStageInfo.RebornPoint > 0)
					{
						GameManager.ClientMgr.ModifyRebornYinJiPointValue(client, rebornStageInfo.RebornPoint, "重生", true, true, false);
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10240", client.ClientData.RebornCount, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10241", client.ClientData.RebornLevel, true);
				long rebornExperience = client.ClientData.RebornExperience;
				client.ClientData.RebornExperience = 0L;
				if (rebornExperience <= 0L)
				{
					this.NotifySelfExperience(client, rebornExperience);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
				}
				else
				{
					this.ProcessRoleExperience(client, rebornExperience, MoneyTypes.RebornExp, false, true, false, "none");
				}
				if (1 == client.ClientData.RebornCount)
				{
					this.AutoGiveRebornInitGoods(client);
				}
				this.InitPlayerRebornPorperty(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				if (rebornStageInfo.AwardGoods != null && rebornStageInfo.AwardGoods.Items != null)
				{
					foreach (AwardsItemData awardsItemData in rebornStageInfo.AwardGoods.Items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "重生", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				EventLogManager.AddRebornEvent(client, rebornCount, client.ClientData.ChangeLifeCount, resList);
				cmdData = string.Format("{0}:{1}:{2}", 1, num, client.ClientData.RebornCount);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessRebornAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				Dictionary<int, List<KFRebornRankInfo>> v;
				lock (this.RebornSyncDataCache)
				{
					v = this.RebornSyncDataCache.RebornRankDict.V;
				}
				Dictionary<int, RebornRankAdmireData> dictionary = new Dictionary<int, RebornRankAdmireData>();
				for (int i = 0; i <= 3; i++)
				{
					RebornRankAdmireData rebornRankAdmireData = new RebornRankAdmireData();
					List<KFRebornRankInfo> list = null;
					if (v.TryGetValue(i, out list) && list != null && list.Count > 0)
					{
						KFRebornRankInfo kfrebornRankInfo = list[0];
						KFRebornRoleData kfrebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(kfrebornRankInfo.PtID, kfrebornRankInfo.Key);
						if (kfrebornRoleData != null && null != kfrebornRoleData.RoleData4Selector)
						{
							rebornRankAdmireData.RoleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(kfrebornRoleData.RoleData4Selector, 0, kfrebornRoleData.RoleData4Selector.Length);
							rebornRankAdmireData.Value = kfrebornRankInfo.Value;
							rebornRankAdmireData.PtID = kfrebornRankInfo.PtID;
							rebornRankAdmireData.Param = kfrebornRankInfo.Param2;
						}
					}
					rebornRankAdmireData.AdmireCount = this.GetRebornAdmireCount(client, i);
					dictionary[i] = rebornRankAdmireData;
				}
				client.sendCmd<Dictionary<int, RebornRankAdmireData>>(nID, dictionary, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessRebornAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				string cmdData;
				if (this.EveryDayMaxRebornExp == null || num2 < 0 || num2 >= this.EveryDayMaxRebornExp.Length)
				{
					cmdData = string.Format("{0}:{1}", -2, num2);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (this.GetRebornAdmireCount(client, num2) >= 1)
				{
					cmdData = string.Format("{0}:{1}", -3, num2);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				this.ProcessRoleExperience(client, (long)this.EveryDayMaxRebornExp[num2], MoneyTypes.RebornExp, true, true, false, "膜拜");
				this.ProcessIncreaseRebornAdmireCount(client, num2);
				cmdData = string.Format("{0}:{1}", 1, num2);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessRebornRankDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				Dictionary<int, List<KFRebornRankInfo>> v;
				lock (this.RebornSyncDataCache)
				{
					v = this.RebornSyncDataCache.RebornRankDict.V;
				}
				RebornRankInfoToClient rebornRankInfoToClient = new RebornRankInfoToClient();
				List<KFRebornRankInfo> list = null;
				if (v.TryGetValue(num2, out list) && null != list)
				{
					foreach (KFRebornRankInfo kfrebornRankInfo in list)
					{
						rebornRankInfoToClient.rankList.Add(new RebornRankInfo
						{
							Key = kfrebornRankInfo.Key,
							Value = kfrebornRankInfo.Value,
							Param1 = kfrebornRankInfo.Param1,
							Param2 = kfrebornRankInfo.Param2,
							UserPtID = Data.GetUserPtIDByUserID(kfrebornRankInfo.UserID)
						});
					}
				}
				rebornRankInfoToClient.RankType = num2;
				client.sendCmd<RebornRankInfoToClient>(nID, rebornRankInfoToClient, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void AutoGiveRebornInitGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法给与重生初始装备", new object[0]), null, true);
			}
			else
			{
				int roleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> list = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("RebornInitialEquip"), true, '|', ',');
					if (null == list)
					{
						LogManager.WriteLog(2, string.Format("重生初始化装备默认数据报错.RoleID{0}", roleID), null, true);
					}
					else if (list.Count <= 0)
					{
						LogManager.WriteLog(2, string.Format("重生初始化装备数量为空.RoleID{0}", roleID), null, true);
					}
					else
					{
						bool flag = false;
						for (int i = 0; i < list.Count; i++)
						{
							int num = list[i][0];
							int num2 = list[i][1];
							int binding = list[i][2];
							int forgeLevel = list[i][3];
							int nAppendPropLev = list[i][4];
							int lucky = list[i][5];
							int excellenceProperty = list[i][6];
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
							{
								LogManager.WriteLog(2, string.Format("重生初始化装备数量ID不存在:RoleID{0},GoodsID={1}", roleID, num), null, true);
							}
							else if (!Global.IsRoleOccupationMatchGoods(client, num))
							{
								LogManager.WriteLog(2, string.Format("重生初始化装备与职业不符RoleID{0}, 物品id{1}.", roleID, num), null, true);
							}
							else if (1 != num2)
							{
								LogManager.WriteLog(2, string.Format("重生初始化装备数量必须为1件RoleID{0}, 数量{1}.", roleID, num2), null, true);
							}
							else
							{
								int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num, num2, 0, "", forgeLevel, binding, 15000, "", false, 1, "自动给于重生装备", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceProperty, nAppendPropLev, 0, null, null, 0, true);
								if (num3 <= 0)
								{
									LogManager.WriteLog(2, string.Format("重生初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", roleID), null, true);
								}
								else
								{
									GoodsData rebornGoodsByDbID = Global.GetRebornGoodsByDbID(client, num3);
									if (null == rebornGoodsByDbID)
									{
										LogManager.WriteLog(2, string.Format("重生初始化装备数量[GetGoodsByID]失败.RoleID{0}", roleID), null, true);
									}
									else
									{
										int num4 = 0;
										int goodsCatetoriy = Global.GetGoodsCatetoriy(rebornGoodsByDbID.GoodsID);
										if (goodsCatetoriy == 36 && flag)
										{
											num4++;
										}
										string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
										{
											client.ClientData.RoleID,
											1,
											rebornGoodsByDbID.Id,
											rebornGoodsByDbID.GoodsID,
											1,
											rebornGoodsByDbID.Site,
											rebornGoodsByDbID.GCount,
											num4,
											""
										});
										TCPProcessCmdResults tcpprocessCmdResults = Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
										if (TCPProcessCmdResults.RESULT_FAILED == tcpprocessCmdResults)
										{
											LogManager.WriteLog(2, string.Format("重生初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", roleID), null, true);
										}
										else if (goodsCatetoriy == 36)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		public bool CheckRebornUpgradeIcon(GameClient client)
		{
			return this.CheckRebornUpgradeLimit(client) == 0;
		}

		public int CheckRebornUpgradeLimit(GameClient client)
		{
			int num = 0;
			Dictionary<int, RebornStageInfo> rebornStageInfoDict = this.RebornStageInfoDict;
			RebornStageInfo rebornStageInfo;
			int result;
			if (!rebornStageInfoDict.TryGetValue(client.ClientData.RebornCount + 1, out rebornStageInfo))
			{
				num = -3;
				result = num;
			}
			else if (rebornStageInfo.NeedZhuanSheng.Length == 2 && Global.GetUnionLevel2(client) < Global.GetUnionLevel2(rebornStageInfo.NeedZhuanSheng[0], rebornStageInfo.NeedZhuanSheng[1]))
			{
				num = -3;
				result = num;
			}
			else if (client.ClientData.RebornLevel < rebornStageInfo.NeedRebornLevel)
			{
				num = -12;
				result = num;
			}
			else if (client.ClientData.CombatForce < rebornStageInfo.NeedZhanLi)
			{
				num = -12;
				result = num;
			}
			else
			{
				if (rebornStageInfo.NeedMaxWing.Length == 5)
				{
					if ((double)client.ClientData.MyWingData.WingID < rebornStageInfo.NeedMaxWing[0] || ((double)client.ClientData.MyWingData.WingID == rebornStageInfo.NeedMaxWing[0] && (double)client.ClientData.MyWingData.ForgeLevel < rebornStageInfo.NeedMaxWing[1]))
					{
						return -12;
					}
					if ((double)LingYuManager.GetTotalLevel(client) < rebornStageInfo.NeedMaxWing[2])
					{
						return -12;
					}
					if (ZhuLingZhuHunManager.GetZhuLingPct(client) < rebornStageInfo.NeedMaxWing[3] || ZhuLingZhuHunManager.GetZhuHunPct(client) < rebornStageInfo.NeedMaxWing[4])
					{
						return -12;
					}
				}
				if (rebornStageInfo.NeedChengJie > 0 && ChengJiuManager.GetChengJiuLevel(client) < rebornStageInfo.NeedChengJie)
				{
					num = -12;
					result = num;
				}
				else if (rebornStageInfo.NeedShengWang > 0 && GameManager.ClientMgr.GetShengWangLevelValue(client) < rebornStageInfo.NeedShengWang)
				{
					num = -12;
					result = num;
				}
				else
				{
					if (rebornStageInfo.NeedMagicBook.Length == 2)
					{
						if (client.ClientData.MerlinData._Level < rebornStageInfo.NeedMagicBook[0] || (client.ClientData.MerlinData._Level == rebornStageInfo.NeedMagicBook[0] && client.ClientData.MerlinData._StarNum < rebornStageInfo.NeedMagicBook[1]))
						{
							return -12;
						}
					}
					result = num;
				}
			}
			return result;
		}

		public int GetRebornAdmireCount(GameClient client, int rankType)
		{
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "151");
			if (roleParamsIntListFromDB.Count != 5)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 5; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (roleParamsIntListFromDB[0] != dayOfYear)
			{
				result = 0;
			}
			else
			{
				result = roleParamsIntListFromDB[rankType + 1];
			}
			return result;
		}

		public void ProcessIncreaseRebornAdmireCount(GameClient client, int rankType)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "151");
			if (roleParamsIntListFromDB.Count != 5)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 5; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			if (roleParamsIntListFromDB[0] == dayOfYear)
			{
				List<int> list;
				int index;
				(list = roleParamsIntListFromDB)[index = rankType + 1] = list[index] + 1;
			}
			else
			{
				roleParamsIntListFromDB[0] = dayOfYear;
				for (int i = 1; i < roleParamsIntListFromDB.Count; i++)
				{
					roleParamsIntListFromDB[i] = 0;
				}
				roleParamsIntListFromDB[rankType + 1] = 1;
			}
			Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "151", true);
		}

		public bool CheckRebornCountLevelValid(GameClient client, int count, int level)
		{
			Dictionary<int, RebornStageInfo> rebornStageInfoDict = this.RebornStageInfoDict;
			Dictionary<int, RebornLevelInfo> rebornLevelInfoDict = this.RebornLevelInfoDict;
			RebornStageInfo rebornStageInfo;
			return rebornStageInfoDict.TryGetValue(count, out rebornStageInfo) && (rebornStageInfo.MaxRebornLevel <= 0 || level <= rebornStageInfo.MaxRebornLevel);
		}

		public void OnLogin(GameClient client, bool login = false)
		{
			if (login)
			{
				this.InitPlayerRebornPorperty(client);
			}
			else
			{
				GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
				GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
			}
		}

		public void InitPlayerRebornPorperty(GameClient client)
		{
			if (client.ClientData.RebornCount > 0)
			{
				Dictionary<int, RebornStageInfo> rebornStageInfoDict = this.RebornStageInfoDict;
				for (int i = 1; i <= rebornStageInfoDict.Count; i++)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.Reborn,
						i,
						PropsCacheManager.ConstExtProps
					});
				}
				for (int i = 1; i <= client.ClientData.RebornCount; i++)
				{
					RebornStageInfo rebornStageInfo;
					if (rebornStageInfoDict.TryGetValue(i, out rebornStageInfo))
					{
						if (0 <= client.ClientData.Occupation && client.ClientData.Occupation < rebornStageInfo.extProps.Length)
						{
							if (null != rebornStageInfo.extProps[client.ClientData.Occupation])
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.Reborn,
									i,
									rebornStageInfo.extProps[client.ClientData.Occupation]
								});
							}
						}
					}
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		public int CalcRebornInjure(IObject attacker, IObject defender, double injurePercnet, double baseRate, ref int burst)
		{
			int num = 0;
			for (int i = 122; i <= 150; i += 7)
			{
				num += (int)RoleAlgorithm.CalRebornAttackInjureValue(attacker, defender, (ExtPropIndexes)i, ref burst);
			}
			if (attacker is GameClient && (defender is GameClient || defender is Robot))
			{
				num /= 2;
			}
			num = (int)((double)num * injurePercnet * baseRate);
			return Math.Max(0, num);
		}

		public int CalculateCombatForce(GameClient client)
		{
			CombatForceInfo rebornCombatForceData = this.RebornCombatForceData;
			double minAttackV = RoleAlgorithm.GetMinAttackV(client);
			double maxAttackV = RoleAlgorithm.GetMaxAttackV(client);
			double minADefenseV = RoleAlgorithm.GetMinADefenseV(client);
			double maxADefenseV = RoleAlgorithm.GetMaxADefenseV(client);
			double minMagicAttackV = RoleAlgorithm.GetMinMagicAttackV(client);
			double maxMagicAttackV = RoleAlgorithm.GetMaxMagicAttackV(client);
			double minMDefenseV = RoleAlgorithm.GetMinMDefenseV(client);
			double maxMDefenseV = RoleAlgorithm.GetMaxMDefenseV(client);
			double hitV = RoleAlgorithm.GetHitV(client);
			double dodgeV = RoleAlgorithm.GetDodgeV(client);
			double addAttackInjureValue = RoleAlgorithm.GetAddAttackInjureValue(client);
			double decreaseInjureValue = RoleAlgorithm.GetDecreaseInjureValue(client);
			double maxLifeV = RoleAlgorithm.GetMaxLifeV(client);
			double maxMagicV = RoleAlgorithm.GetMaxMagicV(client);
			double lifeStealV = RoleAlgorithm.GetLifeStealV(client);
			double num = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Fire);
			double num2 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Water);
			double num3 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Lightning);
			double num4 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Soil);
			double num5 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Ice);
			double num6 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Wind);
			double extProp = RoleAlgorithm.GetExtProp(client, 122);
			double extProp2 = RoleAlgorithm.GetExtProp(client, 123);
			double extProp3 = RoleAlgorithm.GetExtProp(client, 129);
			double extProp4 = RoleAlgorithm.GetExtProp(client, 130);
			double extProp5 = RoleAlgorithm.GetExtProp(client, 136);
			double extProp6 = RoleAlgorithm.GetExtProp(client, 137);
			double extProp7 = RoleAlgorithm.GetExtProp(client, 143);
			double extProp8 = RoleAlgorithm.GetExtProp(client, 144);
			double extProp9 = RoleAlgorithm.GetExtProp(client, 150);
			double extProp10 = RoleAlgorithm.GetExtProp(client, 151);
			if (null != rebornCombatForceData)
			{
				double num7 = (minAttackV / rebornCombatForceData.MinPhysicsAttackModulus + maxAttackV / rebornCombatForceData.MaxPhysicsAttackModulus) / 2.0 + (minADefenseV / rebornCombatForceData.MinPhysicsDefenseModulus + maxADefenseV / rebornCombatForceData.MaxPhysicsDefenseModulus) / 2.0 + (minMagicAttackV / rebornCombatForceData.MinMagicAttackModulus + maxMagicAttackV / rebornCombatForceData.MaxMagicAttackModulus) / 2.0 + (minMDefenseV / rebornCombatForceData.MinMagicDefenseModulus + maxMDefenseV / rebornCombatForceData.MaxMagicDefenseModulus) / 2.0 + addAttackInjureValue / rebornCombatForceData.AddAttackInjureModulus + decreaseInjureValue / rebornCombatForceData.DecreaseInjureModulus + hitV / rebornCombatForceData.HitValueModulus + dodgeV / rebornCombatForceData.DodgeModulus + maxLifeV / rebornCombatForceData.MaxHPModulus + maxMagicV / rebornCombatForceData.MaxMPModulus + lifeStealV / rebornCombatForceData.LifeStealModulus;
				num7 += num / rebornCombatForceData.FireAttack + num2 / rebornCombatForceData.WaterAttack + num3 / rebornCombatForceData.LightningAttack + num4 / rebornCombatForceData.SoilAttack + num5 / rebornCombatForceData.IceAttack + num6 / rebornCombatForceData.WindAttack;
				num7 += extProp / rebornCombatForceData.HolyAttack + extProp2 / rebornCombatForceData.HolyDefense + extProp3 / rebornCombatForceData.ShadowAttack + extProp4 / rebornCombatForceData.ShadowDefense + extProp5 / rebornCombatForceData.NatureAttack + extProp6 / rebornCombatForceData.NatureDefense + extProp7 / rebornCombatForceData.ChaosAttack + extProp8 / rebornCombatForceData.ChaosDefense + extProp9 / rebornCombatForceData.IncubusAttack + extProp10 / rebornCombatForceData.IncubusDefense;
				client.ClientData.RebornCombatForce = (int)num7;
			}
			return client.ClientData.RebornCombatForce;
		}

		public void EarnExperience(GameClient sprite, long experience)
		{
			Dictionary<int, RebornStageInfo> rebornStageInfoDict = this.RebornStageInfoDict;
			Dictionary<int, RebornLevelInfo> rebornLevelInfoDict = this.RebornLevelInfoDict;
			if (sprite.ClientData.RebornCount > 0 && sprite.ClientData.RebornCount <= rebornStageInfoDict.Count)
			{
				RebornStageInfo rebornStageInfo;
				if (rebornStageInfoDict.TryGetValue(sprite.ClientData.RebornCount, out rebornStageInfo))
				{
					RebornLevelInfo rebornLevelInfo = null;
					if (rebornLevelInfoDict.TryGetValue(sprite.ClientData.RebornLevel, out rebornLevelInfo))
					{
						long num = (long)rebornLevelInfo.NeedRebornExp;
						bool flag = false;
						if (rebornStageInfo.MaxRebornLevel > 0 && sprite.ClientData.RebornLevel >= rebornStageInfo.MaxRebornLevel)
						{
							flag = true;
						}
						if (!flag && sprite.ClientData.RebornLevel <= rebornLevelInfoDict.Count - 1 && sprite.ClientData.RebornExperience + experience >= num)
						{
							int rebornLevel = sprite.ClientData.RebornLevel;
							sprite.ClientData.RebornLevel++;
							experience = sprite.ClientData.RebornExperience + experience - num;
							sprite.ClientData.RebornExperience = 0L;
							this.EarnExperience(sprite, experience);
						}
						else
						{
							sprite.ClientData.RebornExperience += experience;
							sprite.ClientData.RebornExperience = Global.GMax(0L, sprite.ClientData.RebornExperience);
						}
					}
				}
			}
		}

		public void ProcessRoleExperience(GameClient client, long experience, MoneyTypes types, bool enableFilter = true, bool writeToDB = true, bool checkDead = false, string strFrom = "none")
		{
			if (types == MoneyTypes.RebornExpMonster || types == MoneyTypes.RebornExpSale || types == MoneyTypes.RebornExp)
			{
				if (client.ClientData.HideGM <= 0)
				{
					if (!checkDead || client.ClientData.CurrentLifeV > 0)
					{
						if (experience > 0L)
						{
							if (types != MoneyTypes.RebornExp)
							{
								experience = Math.Min(experience, this.GetRebornExpMaxValueLeft(client, types));
							}
							if (experience > 0L)
							{
								long rebornExperience = client.ClientData.RebornExperience;
								int unionLevel = Global.GetUnionLevel2(client.ClientData.RebornCount, client.ClientData.RebornLevel);
								EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.Awards, types, experience, -1L, strFrom);
								int rebornLevel = client.ClientData.RebornLevel;
								this.EarnExperience(client, experience);
								long nowTicks = TimeUtil.NOW();
								if (writeToDB || rebornLevel != client.ClientData.RebornLevel)
								{
									Dictionary<int, RebornLevelInfo> rebornLevelInfoDict = this.RebornLevelInfoDict;
									lock (client.ClientData.PropPointMutex)
									{
										for (int i = rebornLevel + 1; i <= client.ClientData.RebornLevel; i++)
										{
											RebornLevelInfo rebornLevelInfo;
											if (rebornLevelInfoDict.TryGetValue(i, out rebornLevelInfo) && rebornLevelInfo.RebornPoint > 0)
											{
												GameManager.ClientMgr.ModifyRebornYinJiPointValue(client, rebornLevelInfo.RebornPoint, "升级", true, true, false);
											}
										}
									}
									Global.SaveRoleParamsInt32ValueToDB(client, "10241", client.ClientData.RebornLevel, true);
									Global.SaveRoleParamsInt64ValueToDB(client, "10242", client.ClientData.RebornExperience, true);
									Global.SetLastDBRoleParamCmdTicks(client, "10241", nowTicks);
									Global.SetLastDBRoleParamCmdTicks(client, "10242", nowTicks);
								}
								else
								{
									Global.SaveRoleParamsInt64ValueToDB(client, "10242", client.ClientData.RebornExperience, false);
									Global.SetLastDBRoleParamCmdTicks(client, "10242", nowTicks);
								}
								if (rebornLevel != client.ClientData.RebornLevel)
								{
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									EventLogManager.AddRoleRebornUpgradeEvent(client, experience, rebornExperience, unionLevel, strFrom);
									KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 0, client.ClientData.RebornLevel, 0, "");
									if (client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.SendIconStateToClient(client);
									}
								}
								GameManager.ClientMgr.UpdateRoleDailyData_RebornExp(client, types, experience);
								this.NotifySelfExperience(client, experience);
								GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, strFrom, MoneyTypes.RebornExpMonster, false, true, false);
								GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, strFrom, MoneyTypes.RebornExpSale, false, true, false);
							}
						}
					}
				}
			}
		}

		public void NotifySelfExperience(GameClient client, long newExperience)
		{
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RebornExperience,
				client.ClientData.RebornLevel,
				newExperience,
				client.ClientData.RebornCount
			});
			client.sendCmd(1711, cmdData, false);
		}

		public long GetRebornExpMaxValue(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				long rebornExpMaxValueFix = this.GetRebornExpMaxValueFix(client, types);
				long rebornExpMaxAddValue = GameManager.ClientMgr.GetRebornExpMaxAddValue(client, types);
				long roleDailyData_RebornExp = GameManager.ClientMgr.GetRoleDailyData_RebornExp(client, types);
				result = Math.Max(0L, rebornExpMaxValueFix + rebornExpMaxAddValue);
			}
			return result;
		}

		public long GetRebornExpMaxValueLeft(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				long rebornExpMaxValueFix = this.GetRebornExpMaxValueFix(client, types);
				long rebornExpMaxAddValue = GameManager.ClientMgr.GetRebornExpMaxAddValue(client, types);
				long roleDailyData_RebornExp = GameManager.ClientMgr.GetRoleDailyData_RebornExp(client, types);
				result = Math.Max(0L, rebornExpMaxValueFix + rebornExpMaxAddValue - roleDailyData_RebornExp);
			}
			return result;
		}

		private long GetRebornExpMaxValueFix(GameClient client, MoneyTypes types)
		{
			Dictionary<int, RebornLevelInfo> rebornLevelInfoDict = this.RebornLevelInfoDict;
			RebornLevelInfo rebornLevelInfo = null;
			long result;
			if (!rebornLevelInfoDict.TryGetValue(client.ClientData.RebornLevel, out rebornLevelInfo))
			{
				result = 0L;
			}
			else
			{
				long num = 0L;
				if (types == MoneyTypes.RebornExpMonster)
				{
					num = (long)rebornLevelInfo.MaxOfMonsters;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					num = (long)rebornLevelInfo.MaxOfGoods;
				}
				result = num;
			}
			return result;
		}

		public int GetTodayLianZhanMax(GameClient client)
		{
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "152");
			if (roleParamsIntListFromDB.Count != 2)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 2; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (roleParamsIntListFromDB[0] != dayOfYear)
			{
				result = 0;
			}
			else
			{
				result = roleParamsIntListFromDB[1];
			}
			return result;
		}

		public void SetTodayLianZhanMax(GameClient client, int max)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "152");
			if (roleParamsIntListFromDB.Count != 2)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 2; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			if (roleParamsIntListFromDB[0] == dayOfYear)
			{
				roleParamsIntListFromDB[1] = max;
			}
			else
			{
				roleParamsIntListFromDB[0] = dayOfYear;
				roleParamsIntListFromDB[1] = max;
			}
			Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "152", true);
		}

		public void ProcessLianZhan(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			if (54 == mapSceneType)
			{
				if (client.ClientData.TempLianZhan > this.GetTodayLianZhanMax(client))
				{
					this.SetTodayLianZhanMax(client, client.ClientData.TempLianZhan);
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 3, client.ClientData.TempLianZhan, 0, "");
				}
			}
		}

		public void ProcessRebornMonsterFallGoods(GameClient client, Monster monster)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(monster.CurrentMapCode);
			if (54 == mapSceneType)
			{
				if (monster.MonsterType == 301)
				{
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 1, 1, 0, "");
				}
				else if (monster.MonsterType == 401)
				{
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 2, 1, 0, "");
				}
			}
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				string roleParamsFromDBByRoleID = Global.GetRoleParamsFromDBByRoleID(roleId, "10240", 0);
				int num = Global.SafeConvertToInt32(roleParamsFromDBByRoleID);
				if (num > 0)
				{
					KuaFuWorldClient.getInstance().Reborn_ChangeName(GameManager.PTID, roleId, newName);
				}
			}
		}

		public void PlatFormChat(GameClient client, string text)
		{
			KFPlatFormChat kfplatFormChat = new KFPlatFormChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, client.ClientData.UserPTID);
			lock (this.Mutex)
			{
				this.PlatFormChatList.Add(kfplatFormChat);
			}
			this.BroadcastPlatFormChatMsg(kfplatFormChat);
		}

		public void BroadcastPlatFormChatMsg(KFPlatFormChat kfChat)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient != null && gameClient.ClientData.UserPTID == kfChat.PtID)
				{
					gameClient.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFPlatFormChat> list = DataHelper.BytesToObject<List<KFPlatFormChat>>(data, 0, data.Length);
				if (null != list)
				{
					foreach (KFPlatFormChat kfChat in list)
					{
						this.BroadcastPlatFormChatMsg(kfChat);
					}
				}
			}
		}

		public bool InitConfig(bool reload = false)
		{
			bool result;
			if (!RebornStamp.ParseYinJiConfig())
			{
				result = false;
			}
			else if (!RebornEquip.ParseRebornEquipConfig())
			{
				result = false;
			}
			else if (!RebornStone.ParseRebornStoneConfig())
			{
				result = false;
			}
			else if (!this.LoadRebornStageConfigFile())
			{
				result = false;
			}
			else if (!this.LoadRebornCombatForceConfigFile())
			{
				result = false;
			}
			else
			{
				if (!reload)
				{
					if (!this.LoadRebornLevelConfigFile())
					{
						return false;
					}
				}
				this.EveryDayMaxRebornExp = GameManager.systemParamsList.GetParamValueIntArrayByName("EveryDayMaxRebornExp", ',');
				this.RebornMapPKMode = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornMapPK", ',');
				result = true;
			}
			return result;
		}

		public bool LoadRebornStageConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornStage));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornStage));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, RebornStageInfo> dictionary = new Dictionary<int, RebornStageInfo>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RebornStageInfo rebornStageInfo = new RebornStageInfo();
					rebornStageInfo.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					rebornStageInfo.NeedZhuanSheng = Global.GetSafeAttributeIntArray(xml, "NeedZhuanSheng", -1, '|');
					rebornStageInfo.NeedRebornLevel = (int)Global.GetSafeAttributeLong(xml, "NeedRebornLevel");
					rebornStageInfo.NeedZhanLi = (int)Global.GetSafeAttributeLong(xml, "NeedZhanLi");
					rebornStageInfo.NeedMaxWing = Global.GetSafeAttributeDoubleArray(xml, "NeedMaxWing", -1, '|');
					rebornStageInfo.NeedChengJie = (int)Global.GetSafeAttributeLong(xml, "NeedChengJie");
					rebornStageInfo.NeedShengWang = (int)Global.GetSafeAttributeLong(xml, "NeedShengWang");
					rebornStageInfo.NeedMagicBook = Global.GetSafeAttributeIntArray(xml, "NeedMagicBook", -1, '|');
					rebornStageInfo.MaxRebornLevel = (int)Global.GetSafeAttributeLong(xml, "MaxRebornLevel");
					rebornStageInfo.RebornPoint = (int)Global.GetSafeAttributeLong(xml, "RebornDian");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "ExtProp");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < 6; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array2[0]);
								rebornStageInfo.extProps[i] = new double[177];
								rebornStageInfo.extProps[i][(int)propIndexByPropName] = Global.SafeConvertToDouble(array2[1]);
							}
						}
					}
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "AwardGoods"), ref rebornStageInfo.AwardGoods, '|', ',');
					dictionary[rebornStageInfo.ID] = rebornStageInfo;
				}
				this.RebornStageInfoDict = dictionary;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornStage, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadRebornLevelConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornLevel));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornLevel));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, RebornLevelInfo> dictionary = new Dictionary<int, RebornLevelInfo>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RebornLevelInfo rebornLevelInfo = new RebornLevelInfo();
					rebornLevelInfo.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					rebornLevelInfo.NeedRebornExp = (int)Global.GetSafeAttributeLong(xml, "NeedRebornExp");
					rebornLevelInfo.MaxOfMonsters = (int)Global.GetSafeAttributeLong(xml, "MaxOfMonsters");
					rebornLevelInfo.MaxOfGoods = (int)Global.GetSafeAttributeLong(xml, "MaxOfGoods");
					rebornLevelInfo.RebornPoint = (int)Global.GetSafeAttributeLong(xml, "RebornDian");
					dictionary[rebornLevelInfo.ID] = rebornLevelInfo;
				}
				this.RebornLevelInfoDict = dictionary;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornLevel, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadRebornCombatForceConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornCombatForce));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornCombatForce));
				if (null == xelement)
				{
					return false;
				}
				CombatForceInfo combatForceInfo = new CombatForceInfo();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					combatForceInfo.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					combatForceInfo.MaxHPModulus = Global.GetSafeAttributeDouble(xelement2, "LifeV");
					combatForceInfo.MaxMPModulus = Global.GetSafeAttributeDouble(xelement2, "MagicV");
					combatForceInfo.MinPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xelement2, "MinDefenseV");
					combatForceInfo.MaxPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xelement2, "MaxDefenseV");
					combatForceInfo.MinMagicDefenseModulus = Global.GetSafeAttributeDouble(xelement2, "MinMDefenseV");
					combatForceInfo.MaxMagicDefenseModulus = Global.GetSafeAttributeDouble(xelement2, "MaxMDefenseV");
					combatForceInfo.MinPhysicsAttackModulus = Global.GetSafeAttributeDouble(xelement2, "MinAttackV");
					combatForceInfo.MaxPhysicsAttackModulus = Global.GetSafeAttributeDouble(xelement2, "MaxAttackV");
					combatForceInfo.MinMagicAttackModulus = Global.GetSafeAttributeDouble(xelement2, "MinMAttackV");
					combatForceInfo.MaxMagicAttackModulus = Global.GetSafeAttributeDouble(xelement2, "MaxMAttackV");
					combatForceInfo.HitValueModulus = Global.GetSafeAttributeDouble(xelement2, "HitV");
					combatForceInfo.DodgeModulus = Global.GetSafeAttributeDouble(xelement2, "Dodge");
					combatForceInfo.AddAttackInjureModulus = Global.GetSafeAttributeDouble(xelement2, "AddAttackInjure");
					combatForceInfo.DecreaseInjureModulus = Global.GetSafeAttributeDouble(xelement2, "DecreaseInjureValue");
					combatForceInfo.LifeStealModulus = Global.GetSafeAttributeDouble(xelement2, "LifeSteal");
					combatForceInfo.AddAttackModulus = Global.GetSafeAttributeDouble(xelement2, "AddAttack");
					combatForceInfo.AddDefenseModulus = Global.GetSafeAttributeDouble(xelement2, "AddDefense");
					combatForceInfo.FireAttack = Global.GetSafeAttributeDouble(xelement2, "FireAttack");
					combatForceInfo.WaterAttack = Global.GetSafeAttributeDouble(xelement2, "WaterAttack");
					combatForceInfo.LightningAttack = Global.GetSafeAttributeDouble(xelement2, "LightningAttack");
					combatForceInfo.SoilAttack = Global.GetSafeAttributeDouble(xelement2, "SoilAttack");
					combatForceInfo.IceAttack = Global.GetSafeAttributeDouble(xelement2, "IceAttack");
					combatForceInfo.WindAttack = Global.GetSafeAttributeDouble(xelement2, "WindAttack");
					combatForceInfo.ArmorMax = ConfigHelper.GetElementAttributeValueDouble(xelement2, "ArmorMax", 1.0);
					combatForceInfo.HolyAttack = Global.GetSafeAttributeDouble(xelement2, "HolyAttack");
					combatForceInfo.HolyDefense = Global.GetSafeAttributeDouble(xelement2, "HolyDefense");
					combatForceInfo.ShadowAttack = Global.GetSafeAttributeDouble(xelement2, "ShadowAttack");
					combatForceInfo.ShadowDefense = Global.GetSafeAttributeDouble(xelement2, "ShadowDefense");
					combatForceInfo.NatureAttack = Global.GetSafeAttributeDouble(xelement2, "NatureAttack");
					combatForceInfo.NatureDefense = Global.GetSafeAttributeDouble(xelement2, "NatureDefense");
					combatForceInfo.ChaosAttack = Global.GetSafeAttributeDouble(xelement2, "ChaosAttack");
					combatForceInfo.ChaosDefense = Global.GetSafeAttributeDouble(xelement2, "ChaosDefense");
					combatForceInfo.IncubusAttack = Global.GetSafeAttributeDouble(xelement2, "IncubusAttack");
					combatForceInfo.IncubusDefense = Global.GetSafeAttributeDouble(xelement2, "IncubusDefense");
				}
				this.RebornCombatForceData = combatForceInfo;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornCombatForce, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool IfSupportPKModeNotNormal(int mapCode)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(mapCode);
			bool result;
			if (54 != mapSceneType)
			{
				result = true;
			}
			else
			{
				List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
				if (null == list)
				{
					result = false;
				}
				else
				{
					KuaFuLineData kuaFuLineData = list.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
					result = (null != kuaFuLineData && this.RebornMapPKMode[kuaFuLineData.Line - 1] != 0);
				}
			}
			return result;
		}

		public void OnInitGame(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			if (54 == mapSceneType)
			{
				if (0 != client.ClientData.PKMode)
				{
					if (this.IfSupportPKModeNotNormal(client.ClientData.MapCode))
					{
						client.ClientData.PKMode = 2;
					}
					else
					{
						client.ClientData.PKMode = 0;
					}
				}
			}
		}

		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				RebornSyncData rebornSyncData = KuaFuWorldClient.getInstance().Reborn_SyncData(this.RebornSyncDataCache.RebornRankDict.Age, this.RebornSyncDataCache.BossRefreshDict.Age);
				if (null != rebornSyncData)
				{
					lock (this.RebornSyncDataCache)
					{
						if (this.RebornSyncDataCache.RebornRankDict.Age != rebornSyncData.RebornRankDict.Age && null != rebornSyncData.BytesRebornRankDict)
						{
							this.RebornSyncDataCache.RebornRankDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>>(rebornSyncData.BytesRebornRankDict, 0, rebornSyncData.BytesRebornRankDict.Length);
							if (!GameManager.IsKuaFuServer)
							{
								int i = 0;
								while (i <= 3)
								{
									List<KFRebornRankInfo> list = null;
									if (this.RebornSyncDataCache.RebornRankDict.V.TryGetValue(i, out list) && list != null && list.Count > 0)
									{
										KFRebornRankInfo kfrebornRankInfo = list[0];
										if (kfrebornRankInfo.PtID == GameManager.PTID)
										{
											RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, kfrebornRankInfo.Key), 0);
											if (roleDataEx != null && roleDataEx.RoleID > 0)
											{
												RoleData4Selector roleData4Selector = Global.RoleDataEx2RoleData4Selector(roleDataEx);
												KuaFuWorldClient.getInstance().Reborn_SetRoleData4Selector(kfrebornRankInfo.PtID, kfrebornRankInfo.Key, DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector));
											}
										}
									}
									IL_17F:
									i++;
									continue;
									goto IL_17F;
								}
							}
						}
						if (this.RebornSyncDataCache.BossRefreshDict.Age != rebornSyncData.BossRefreshDict.Age && null != rebornSyncData.BytesRebornBossRefreshDict)
						{
							this.RebornSyncDataCache.BossRefreshDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>>(rebornSyncData.BytesRebornBossRefreshDict, 0, rebornSyncData.BytesRebornBossRefreshDict.Length);
							if (null == this.RebornSyncDataCache.BossRefreshDict)
							{
								this.RebornSyncDataCache.BossRefreshDict = new KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>();
							}
						}
					}
					List<KFPlatFormChat> list2 = null;
					lock (this.Mutex)
					{
						if (this.PlatFormChatList.Count > 0)
						{
							list2 = new List<KFPlatFormChat>(this.PlatFormChatList);
							this.PlatFormChatList.Clear();
						}
					}
					if (null != list2)
					{
						KuaFuWorldClient.getInstance().Reborn_PlatFormChat(list2);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
		}

		public object Mutex = new object();

		private Dictionary<int, RebornStageInfo> RebornStageInfoDict = new Dictionary<int, RebornStageInfo>();

		private Dictionary<int, RebornLevelInfo> RebornLevelInfoDict = new Dictionary<int, RebornLevelInfo>();

		private CombatForceInfo RebornCombatForceData = new CombatForceInfo();

		private int[] EveryDayMaxRebornExp;

		private int[] RebornMapPKMode;

		public RebornSyncData RebornSyncDataCache = new RebornSyncData();

		public List<KFPlatFormChat> PlatFormChatList = new List<KFPlatFormChat>();

		private static RebornManager instance = new RebornManager();
	}
}
