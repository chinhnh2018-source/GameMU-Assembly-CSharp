using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	public class SevenDayActivityMgr : SingletonTemplate<SevenDayActivityMgr>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		private SevenDayActivityMgr()
		{
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1310, 2, 2, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1311, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1312, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1310:
				result = this.HandleClientQuery(client, nID, bytes, cmdParams);
				break;
			case 1311:
				result = this.HandleGetAward(client, nID, bytes, cmdParams);
				break;
			case 1312:
				result = this.HandleClientBuy(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 32)
			{
				SevenDayGoalEventObject sevenDayGoalEventObject = eventObject as SevenDayGoalEventObject;
				try
				{
					if (sevenDayGoalEventObject != null && this.IsInActivityTime(sevenDayGoalEventObject.Client))
					{
						this.GoalAct.HandleEvent(sevenDayGoalEventObject);
						if (sevenDayGoalEventObject.Client.ClientSocket.session.SocketTime[4] > 0L)
						{
							this.CheckSendIconState(sevenDayGoalEventObject.Client);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, "SevenDayActivityMgr.processEvent [SevenDayGoal]", ex, true);
				}
				finally
				{
					SevenDayGoalEvPool.Free(sevenDayGoalEventObject);
				}
			}
		}

		public void LoadConfig()
		{
			this.LoginAct.LoadConfig();
			this.ChargeAct.LoadConfig();
			this.BuyAct.LoadConfig();
			this.GoalAct.LoadConfig();
		}

		public void OnLogin(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(7))
			{
				if (this.IsInActivityTime(client))
				{
					this.LoginAct.Update(client);
					this.ChargeAct.Update(client);
					this.GoalAct.Update(client);
					this.CheckSendIconState(client);
				}
				else if (TimeUtil.NowDateTime() > Global.GetRegTime(client.ClientData).AddDays(14.0))
				{
					lock (client.ClientData.SevenDayActDict)
					{
						if (client.ClientData.SevenDayActDict.Count > 0)
						{
							if (!Global.sendToDB<bool, int>(13221, client.ClientData.RoleID, client.ServerId))
							{
								LogManager.WriteLog(2, string.Format("玩家超过七日活动结束后7天了，删除数据失败,roleid={0}", client.ClientData.RoleID), null, true);
							}
							else
							{
								client.ClientData.SevenDayActDict.Clear();
							}
						}
					}
				}
			}
		}

		public void OnNewDay(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(7))
			{
				if (this.IsInActivityTime(client))
				{
					this.LoginAct.Update(client);
					this.ChargeAct.Update(client);
					this.CheckSendIconState(client);
				}
			}
		}

		public void OnCharge(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(7))
			{
				if (this.IsInActivityTime(client))
				{
					this.ChargeAct.Update(client);
					this.CheckSendIconState(client);
				}
			}
		}

		private void CheckSendIconState(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(7))
			{
				if (client != null)
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = this.LoginAct.HasAnyAwardCanGet(client);
					flag = (flag || flag3);
					flag2 |= client._IconStateMgr.AddFlushIconState(17001, flag3);
					flag3 = this.ChargeAct.HasAnyAwardCanGet(client);
					flag = (flag || flag3);
					flag2 |= client._IconStateMgr.AddFlushIconState(17002, flag3);
					flag3 = this.BuyAct.HasAnyCanBuy(client);
					flag = (flag || flag3);
					flag2 |= client._IconStateMgr.AddFlushIconState(17004, flag3);
					bool[] array = null;
					flag3 = this.GoalAct.HasAnyAwardCanGet(client, out array);
					flag2 |= client._IconStateMgr.AddFlushIconState(17005, array[0]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17006, array[1]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17007, array[2]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17008, array[3]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17009, array[4]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17010, array[5]);
					flag2 |= client._IconStateMgr.AddFlushIconState(17011, array[6]);
					flag = (flag || flag3);
					flag2 |= client._IconStateMgr.AddFlushIconState(17003, flag3);
					flag2 |= client._IconStateMgr.AddFlushIconState(17000, flag);
					if (flag2)
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		private bool HandleClientQuery(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(cmdParams[1]);
				SevenDayActQueryData sevenDayActQueryData = new SevenDayActQueryData();
				sevenDayActQueryData.ActivityType = num;
				sevenDayActQueryData.ItemDict = null;
				TCPOutPacket tcpoutPacket = null;
				Dictionary<int, SevenDayItemData> activityData = this.GetActivityData(client, (ESevenDayActType)num);
				if (activityData == null)
				{
					tcpoutPacket = DataHelper.ObjectToTCPOutPacket<SevenDayActQueryData>(sevenDayActQueryData, TCPOutPacketPool.getInstance(), nID);
				}
				else
				{
					lock (activityData)
					{
						sevenDayActQueryData.ItemDict = activityData;
						if (num == 2)
						{
							sevenDayActQueryData.ItemDict = new Dictionary<int, SevenDayItemData>();
							foreach (KeyValuePair<int, SevenDayItemData> keyValuePair in activityData)
							{
								sevenDayActQueryData.ItemDict.Add(keyValuePair.Key, new SevenDayItemData
								{
									AwardFlag = keyValuePair.Value.AwardFlag,
									Params1 = Global.TransMoneyToYuanBao(keyValuePair.Value.Params1),
									Params2 = keyValuePair.Value.Params2
								});
							}
						}
						tcpoutPacket = DataHelper.ObjectToTCPOutPacket<SevenDayActQueryData>(sevenDayActQueryData, TCPOutPacketPool.getInstance(), nID);
					}
				}
				if (tcpoutPacket != null)
				{
					client.sendCmd(tcpoutPacket, true);
				}
				result = true;
			}
			return result;
		}

		private bool HandleGetAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = Convert.ToInt32(cmdParams[2]);
				ESevenDayActErrorCode esevenDayActErrorCode = ESevenDayActErrorCode.NotInActivityTime;
				if (!this.IsInActivityTime(client))
				{
					esevenDayActErrorCode = ESevenDayActErrorCode.NotInActivityTime;
				}
				else if (num == 1)
				{
					esevenDayActErrorCode = this.LoginAct.HandleGetAward(client, num2);
				}
				else if (num == 2)
				{
					esevenDayActErrorCode = this.ChargeAct.HandleGetAward(client, num2);
				}
				else if (num == 3)
				{
					esevenDayActErrorCode = this.GoalAct.HandleGetAward(client, num2);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", (int)esevenDayActErrorCode, num, num2), false);
				if (esevenDayActErrorCode == ESevenDayActErrorCode.Success)
				{
					this.CheckSendIconState(client);
				}
				result = true;
			}
			return result;
		}

		private bool HandleClientBuy(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = Convert.ToInt32(cmdParams[2]);
				ESevenDayActErrorCode esevenDayActErrorCode;
				if (!this.IsInActivityTime(client))
				{
					esevenDayActErrorCode = ESevenDayActErrorCode.NotInActivityTime;
				}
				else
				{
					esevenDayActErrorCode = this.BuyAct.HandleClientBuy(client, num, num2);
				}
				if (esevenDayActErrorCode == ESevenDayActErrorCode.Success)
				{
					this.CheckSendIconState(client);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", (int)esevenDayActErrorCode, num, num2), false);
				result = true;
			}
			return result;
		}

		public bool IsInActivityTime(GameClient client)
		{
			int num;
			return !GameFuncControlManager.IsGameFuncDisabled(7) && this.IsInActivityTime(client, out num);
		}

		public bool IsInActivityTime(GameClient client, out int currDay)
		{
			currDay = 0;
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = false;
			}
			else if (client == null)
			{
				result = false;
			}
			else
			{
				DateTime dateTime = Global.GetRegTime(client.ClientData);
				dateTime -= dateTime.TimeOfDay;
				DateTime dateTime2 = TimeUtil.NowDateTime();
				dateTime2 -= dateTime2.TimeOfDay;
				currDay = (dateTime2 - dateTime).Days + 1;
				result = (currDay >= 1 && currDay <= 7);
			}
			return result;
		}

		public Dictionary<int, SevenDayItemData> GetActivityData(GameClient client, ESevenDayActType actType)
		{
			Dictionary<int, SevenDayItemData> result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = null;
			}
			else if (client == null)
			{
				result = null;
			}
			else
			{
				Dictionary<int, SevenDayItemData> dictionary = null;
				lock (client.ClientData.SevenDayActDict)
				{
					if (!client.ClientData.SevenDayActDict.TryGetValue((int)actType, out dictionary))
					{
						dictionary = new Dictionary<int, SevenDayItemData>();
						client.ClientData.SevenDayActDict[(int)actType] = dictionary;
					}
				}
				result = dictionary;
			}
			return result;
		}

		public bool UpdateDb(int roleid, ESevenDayActType actType, int id, SevenDayItemData itemData, int serverId)
		{
			bool result;
			if (!Global.sendToDB<bool, SevenDayUpdateDbData>(13220, new SevenDayUpdateDbData
			{
				RoleId = roleid,
				ActivityType = (int)actType,
				Id = id,
				Data = itemData
			}, serverId))
			{
				LogManager.WriteLog(2, string.Format("七日活动更新玩家数据失败, roleid={0}, act={1}, id={2}", roleid, actType, id), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool GiveAward(GameClient client, AwardItem item, ESevenDayActType type)
		{
			bool result;
			if (client == null || null == item)
			{
				result = false;
			}
			else
			{
				if (item.GoodsDataList != null)
				{
					for (int i = 0; i < item.GoodsDataList.Count; i++)
					{
						int goodsID = item.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, goodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsDataList[i].GoodsID, item.GoodsDataList[i].GCount, item.GoodsDataList[i].Quality, "", item.GoodsDataList[i].Forge_level, item.GoodsDataList[i].Binding, 0, "", true, 1, this.GetActivityChineseName(type), "1900-01-01 12:00:00", item.GoodsDataList[i].AddPropIndex, item.GoodsDataList[i].BornIndex, item.GoodsDataList[i].Lucky, item.GoodsDataList[i].Strong, item.GoodsDataList[i].ExcellenceInfo, item.GoodsDataList[i].AppendPropLev, item.GoodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				result = true;
			}
			return result;
		}

		public string GetActivityChineseName(ESevenDayActType type)
		{
			string result = type.ToString();
			if (type == ESevenDayActType.Login)
			{
				result = "七日登录";
			}
			else if (type == ESevenDayActType.Charge)
			{
				result = "七日充值";
			}
			else if (type == ESevenDayActType.Goal)
			{
				result = "七日目标";
			}
			else if (type == ESevenDayActType.Buy)
			{
				result = "七日抢购";
			}
			return result;
		}

		public bool GiveEffectiveTimeAward(GameClient client, AwardItem item, ESevenDayActType type)
		{
			bool result;
			if (client == null || null == item)
			{
				result = false;
			}
			else
			{
				if (item.GoodsDataList != null)
				{
					for (int i = 0; i < item.GoodsDataList.Count; i++)
					{
						int goodsID = item.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, goodsID))
						{
							Global.AddEffectiveTimeGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsDataList[i].GoodsID, item.GoodsDataList[i].GCount, item.GoodsDataList[i].Quality, "", item.GoodsDataList[i].Forge_level, item.GoodsDataList[i].Binding, 0, "", false, 1, this.GetActivityChineseName(type), item.GoodsDataList[i].Starttime, item.GoodsDataList[i].Endtime, item.GoodsDataList[i].AddPropIndex, item.GoodsDataList[i].BornIndex, item.GoodsDataList[i].Lucky, item.GoodsDataList[i].Strong, item.GoodsDataList[i].ExcellenceInfo, item.GoodsDataList[i].AppendPropLev, item.GoodsDataList[i].ChangeLifeLevForEquip, null, null);
						}
					}
				}
				result = true;
			}
			return result;
		}

		public void On_GM(GameClient client, string[] cmdFields)
		{
			if (cmdFields != null && cmdFields.Length >= 2)
			{
				if (cmdFields[1] == "reload")
				{
					SingletonTemplate<SevenDayActivityMgr>.Instance().LoadConfig();
				}
				else if (cmdFields[1] == "get" && client != null)
				{
					if (cmdFields.Length >= 4)
					{
						this.HandleGetAward(client, 1311, null, new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2],
							cmdFields[3]
						});
					}
				}
				else if (cmdFields[1] == "buy" && client != null)
				{
					if (cmdFields.Length >= 4)
					{
						this.HandleClientBuy(client, 1312, null, new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2],
							cmdFields[3]
						});
					}
				}
			}
		}

		private SevenDayLoginAct LoginAct = new SevenDayLoginAct();

		private SevenDayChargeAct ChargeAct = new SevenDayChargeAct();

		private SevenDayBuyAct BuyAct = new SevenDayBuyAct();

		private SevenDayGoalAct GoalAct = new SevenDayGoalAct();
	}
}
