using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.OnePiece
{
	public class OnePieceManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static OnePieceManager getInstance()
		{
			return OnePieceManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1600, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1601, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1605, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1602, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1604, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1606, 2, 2, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!GlobalNew.IsGongNengOpened(client, 72, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(506, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(8))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(507, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1600:
					return this.ProcessOnePieceGetInfoCmd(client, nID, bytes, cmdParams);
				case 1601:
					return this.ProcessOnePieceRollCmd(client, nID, bytes, cmdParams);
				case 1602:
					return this.ProcessOnePieceTriggerEventCmd(client, nID, bytes, cmdParams);
				case 1604:
					return this.ProcessOnePieceMoveCmd(client, nID, bytes, cmdParams);
				case 1605:
					return this.ProcessOnePieceRollMiracleCmd(client, nID, bytes, cmdParams);
				case 1606:
					return this.ProcessOnePieceDiceBuyCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public void GetOnePieceTreasureData(GameClient client, OnePieceTreasureData myTreasureData)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			DateTime dateTime = TimeUtil.NowDateTime();
			int rollNumNormal = 0;
			int rollNumMiracle = 0;
			int posID = this.GenPosID(1, 0);
			int eventID = 0;
			string name = "TreasureData";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			if (null != roleParamByName)
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (5 == array.Length)
				{
					int num = Convert.ToInt32(array[0]);
					rollNumNormal = Convert.ToInt32(array[1]);
					rollNumMiracle = Convert.ToInt32(array[2]);
					posID = Convert.ToInt32(array[3]);
					eventID = Convert.ToInt32(array[4]);
				}
			}
			myTreasureData.PosID = posID;
			myTreasureData.EventID = eventID;
			myTreasureData.RollNumNormal = rollNumNormal;
			myTreasureData.RollNumMiracle = rollNumMiracle;
			string s = dateTime.ToString("yyyy-MM-dd");
			DateTime dateTime2;
			if (DateTime.TryParse(s, out dateTime2))
			{
				int num2 = (int)(DayOfWeek.Monday - dateTime.DayOfWeek);
				num2 = ((num2 <= 0) ? (7 + num2) : num2);
				dateTime2 = dateTime2.AddDays((double)num2);
				myTreasureData.ResetPosTicks = TimeUtil.TimeDiff(dateTime2.Ticks, dateTime.Ticks);
			}
		}

		public void JudgeResetOnePieceTreasureData(GameClient client)
		{
			if (client.ClientData.OnePieceMoveLeft == 0)
			{
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				DateTime dateTime = TimeUtil.NowDateTime();
				string text = "TreasureData";
				string roleParamByName = Global.GetRoleParamByName(client, text);
				int num = 0;
				if (null != roleParamByName)
				{
					string[] array = roleParamByName.Split(new char[]
					{
						','
					});
					if (5 == array.Length)
					{
						num = Convert.ToInt32(array[0]);
					}
				}
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				int num2 = dateTime.DayOfWeek - DayOfWeek.Monday;
				num2 = ((num2 >= 0) ? (num2 + 1) : 7);
				if (num != 0 && offsetDay - num >= num2)
				{
					onePieceTreasureData.PosID = this.ResetRolePos(client);
					onePieceTreasureData.EventID = 0;
				}
				if (offsetDay != num && offsetDay > num)
				{
					this.HandleDicePassDay(client, offsetDay, num, onePieceTreasureData);
					num = offsetDay;
				}
				string valueString = string.Format("{0},{1},{2},{3},{4}", new object[]
				{
					num,
					onePieceTreasureData.RollNumNormal,
					onePieceTreasureData.RollNumMiracle,
					onePieceTreasureData.PosID,
					onePieceTreasureData.EventID
				});
				Global.SaveRoleParamsStringToDB(client, text, valueString, true);
			}
		}

		public void HandleDicePassDay(GameClient client, int currday, int lastday, OnePieceTreasureData myOnePieceData)
		{
			int num = 1;
			if (lastday != 0)
			{
				num = currday - lastday;
			}
			if (num > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 0, this.SystemParamsTreasureFreeNum * num);
				this.ModifyOnePieceDice(client, myOnePieceData, 1, this.SystemParamsTreasureMiracleNum * num);
			}
		}

		public int ResetRolePos(GameClient client)
		{
			int num = this.GenPosID(1, 0);
			this.TryGiveOnePieceBoxListAward(client);
			string cmdData = string.Format("{0}:{1}", 13, num);
			client.sendCmd(1604, cmdData, false);
			return num;
		}

		public void ModifyOnePieceTreasureData(GameClient client, OnePieceTreasureData myOnePieceData)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			DateTime dateTime = TimeUtil.NowDateTime();
			string text = "TreasureData";
			string roleParamByName = Global.GetRoleParamByName(client, text);
			int num = offsetDay;
			if (null != roleParamByName)
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (5 == array.Length)
				{
					num = Convert.ToInt32(array[0]);
				}
			}
			string valueString = string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				num,
				myOnePieceData.RollNumNormal,
				myOnePieceData.RollNumMiracle,
				myOnePieceData.PosID,
				myOnePieceData.EventID
			});
			Global.SaveRoleParamsStringToDB(client, text, valueString, true);
		}

		public int GenPosID(int floor, int cell)
		{
			return 1000 * floor + cell;
		}

		public int GetFloorByPosID(int posid)
		{
			return posid / 1000;
		}

		public int FilterPosIDChangeFloor(GameClient client, int posid)
		{
			int result;
			if (client.ClientData.OnePieceMoveDir == 0)
			{
				result = posid;
			}
			else if (this.IfHaveOnePieceBoxListAward(client))
			{
				result = posid;
			}
			else
			{
				bool flag = client.ClientData.OnePieceMoveDir > 0;
				if (posid % 1000 != 0 && posid % 1000 != 30)
				{
					result = posid;
				}
				else
				{
					int num;
					if (flag)
					{
						num = (this.GetFloorByPosID(posid) + 1) * 1000;
					}
					else
					{
						num = posid - 1;
					}
					OnePieceTreasureMapConfig onePieceTreasureMapConfig = null;
					if (!this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig))
					{
						num = this.GenPosID(1, 0);
					}
					result = num;
				}
			}
			return result;
		}

		public int GetNextPosIDForEvent(int posid, bool foward = true)
		{
			int num;
			if (foward)
			{
				num = posid + 1;
			}
			else
			{
				num = posid - 1;
			}
			OnePieceTreasureMapConfig onePieceTreasureMapConfig = null;
			int result;
			if (this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig) && posid % 1000 != 30 && num % 1000 != 0)
			{
				result = num;
			}
			else
			{
				if (foward)
				{
					num = (this.GetFloorByPosID(posid) + 1) * 1000 + 1;
					if (!this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig))
					{
						num = this.GenPosID(1, 1);
					}
				}
				else
				{
					num = (this.GetFloorByPosID(posid) - 1) * 1000 + 30;
					if (!this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig))
					{
						num = this.GenPosID(1, 0);
					}
				}
				result = num;
			}
			return result;
		}

		public int RollMoveNum()
		{
			int num = 0;
			int result;
			if (this.OnePiece_FakeRollNum_GM != 0)
			{
				num = this.OnePiece_FakeRollNum_GM;
				result = num;
			}
			else
			{
				double num2 = (double)Global.GetRandomNumber(1, 101) / 100.0;
				double num3 = 0.0;
				for (int i = 0; i < this.SystemParamsTreasureDice.Count; i++)
				{
					num3 += this.SystemParamsTreasureDice[i];
					if (num2 <= num3)
					{
						num = i + 1;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public int RandomTreasureEvent(List<OnePieceRandomEvent> LisRandomEvent)
		{
			int num = 0;
			int result;
			if (LisRandomEvent == null || LisRandomEvent.Count == 0)
			{
				result = num;
			}
			else
			{
				double num2 = (double)Global.GetRandomNumber(1, 101) / 100.0;
				double num3 = 0.0;
				for (int i = 0; i < LisRandomEvent.Count; i++)
				{
					num3 += LisRandomEvent[i].Rate;
					if (num2 <= num3)
					{
						num = LisRandomEvent[i].EventID;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public void SyncOnePieceEvent(GameClient client, int EventID, int EventValue = 0, int ErrCode = 0, List<int> BoxIDList = null)
		{
			OnePieceTreasureEvent onePieceTreasureEvent = new OnePieceTreasureEvent
			{
				EventID = EventID,
				EventValue = EventValue,
				BoxIDList = BoxIDList,
				ErrCode = ErrCode
			};
			byte[] buffer = DataHelper.ObjectToBytes<OnePieceTreasureEvent>(onePieceTreasureEvent);
			GameManager.ClientMgr.SendToClient(client, buffer, 1603);
		}

		public void ModifyOnePieceDice(GameClient client, OnePieceTreasureData myOnePieceData, int diceType, int num)
		{
			bool flag = false;
			int num2;
			int num3;
			if (diceType == 0)
			{
				num2 = myOnePieceData.RollNumNormal;
				myOnePieceData.RollNumNormal += num;
				if (myOnePieceData.RollNumNormal > 99)
				{
					myOnePieceData.RollNumNormal = 99;
					flag = true;
				}
				num3 = myOnePieceData.RollNumNormal;
			}
			else
			{
				if (diceType != 1)
				{
					return;
				}
				num2 = myOnePieceData.RollNumMiracle;
				myOnePieceData.RollNumMiracle += num;
				if (myOnePieceData.RollNumMiracle > 99)
				{
					myOnePieceData.RollNumMiracle = 99;
					flag = true;
				}
				num3 = myOnePieceData.RollNumMiracle;
			}
			string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				0,
				diceType,
				num3,
				num2
			});
			client.sendCmd(1607, cmdData, false);
			if (flag)
			{
				cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					14,
					diceType,
					num3,
					num2
				});
				client.sendCmd(1607, cmdData, false);
			}
		}

		public int GiveCopyMapGift(GameClient client, int fuBenID)
		{
			OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
			this.GetOnePieceTreasureData(client, onePieceTreasureData);
			int result;
			if (client.ClientData.OnePieceTempEventID == 0)
			{
				result = 0;
			}
			else
			{
				OnePieceTreasureEventConfig onePieceTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(client.ClientData.OnePieceTempEventID, out onePieceTreasureEventConfig))
				{
					result = 0;
				}
				else if (onePieceTreasureEventConfig.FuBenID != fuBenID)
				{
					result = 0;
				}
				else
				{
					int onePieceTempEventID = client.ClientData.OnePieceTempEventID;
					this.GiveOnePieceEventAward(client, onePieceTreasureData, onePieceTreasureEventConfig);
					client.ClientData.OnePieceTempEventID = 0;
					result = onePieceTempEventID;
				}
			}
			return result;
		}

		public OnePieceTreasureErrorCode GiveOnePieceEventAward(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode result = OnePieceTreasureErrorCode.OnePiece_Success;
			List<GoodsData> list = Global.ConvertToGoodsDataList(myTreasureEventConfig.GoodsList.Items, -1);
			if (!Global.CanAddGoodsDataList(client, list))
			{
				if (myTreasureEventConfig.Type == TreasureEventType.ETET_Excharge)
				{
					return OnePieceTreasureErrorCode.OnePiece_ErrorBagNotEnough;
				}
				foreach (GoodsData goodsData in list)
				{
					Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(508, new object[0]), GLang.GetLang(508, new object[0]), 1.0);
				}
				result = OnePieceTreasureErrorCode.OnePiece_ErrorCheckMail;
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					GoodsData goodsData2 = list[i];
					if (null != goodsData2)
					{
						goodsData2.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, "获得藏宝秘境奖励", goodsData2.Endtime, goodsData2.AddPropIndex, goodsData2.BornIndex, goodsData2.Lucky, goodsData2.Strong, 0, 0, 0, null, null, 0, true);
					}
				}
			}
			if (myTreasureEventConfig.NewValue.Type != MoneyTypes.None)
			{
				int type = (int)myTreasureEventConfig.NewValue.Type;
				if (type <= 8)
				{
					if (type != 1)
					{
						if (type == 8)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", false);
						}
					}
					else
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", false);
					}
				}
				else if (type != 40)
				{
					if (type != 50)
					{
						switch (type)
						{
						case 110:
							GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", true);
							break;
						case 111:
							GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, myTreasureEventConfig.NewValue.Num, true, true);
							break;
						}
					}
					else
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励");
					}
				}
				else
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", ActivityTypes.None, "");
				}
			}
			if (myTreasureEventConfig.NewDiec > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 0, myTreasureEventConfig.NewDiec);
			}
			if (myTreasureEventConfig.NewSuperDiec > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 1, myTreasureEventConfig.NewSuperDiec);
			}
			return result;
		}

		public OnePieceTreasureErrorCode TriggerEventAward(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode onePieceTreasureErrorCode = this.GiveOnePieceEventAward(client, myOnePieceData, myTreasureEventConfig);
			OnePieceTreasureErrorCode result;
			if (onePieceTreasureErrorCode != OnePieceTreasureErrorCode.OnePiece_Success && onePieceTreasureErrorCode != OnePieceTreasureErrorCode.OnePiece_ErrorCheckMail)
			{
				result = onePieceTreasureErrorCode;
			}
			else
			{
				this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, 0, (int)onePieceTreasureErrorCode, null);
				result = OnePieceTreasureErrorCode.OnePiece_Success;
			}
			return result;
		}

		public OnePieceTreasureErrorCode TriggerEventExcharge(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			int i = 0;
			while (i < myTreasureEventConfig.NeedGoods.Count)
			{
				SystemXmlItem systemXmlItem = null;
				OnePieceTreasureErrorCode result;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(myTreasureEventConfig.NeedGoods[i]._NeedGoodsID, out systemXmlItem))
				{
					result = OnePieceTreasureErrorCode.OnePiece_ErrorNeedGoodsID;
				}
				else if (myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount <= 0)
				{
					result = OnePieceTreasureErrorCode.OnePiece_ErrorNeedGoodsCount;
				}
				else
				{
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, myTreasureEventConfig.NeedGoods[i]._NeedGoodsID);
					if (totalGoodsCountByID >= myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount)
					{
						i++;
						continue;
					}
					result = OnePieceTreasureErrorCode.OnePiece_ErrorGoodsNotEnough;
				}
				return result;
			}
			if (0 > Global.IsRoleHasEnoughMoney(client, myTreasureEventConfig.NeedValue.Num, (int)myTreasureEventConfig.NeedValue.Type))
			{
				return OnePieceTreasureErrorCode.OnePiece_ErrorNeedMoneyNotEnough;
			}
			OnePieceTreasureErrorCode onePieceTreasureErrorCode = this.GiveOnePieceEventAward(client, myOnePieceData, myTreasureEventConfig);
			if (OnePieceTreasureErrorCode.OnePiece_Success != onePieceTreasureErrorCode)
			{
				return onePieceTreasureErrorCode;
			}
			for (i = 0; i < myTreasureEventConfig.NeedGoods.Count; i++)
			{
				bool flag = false;
				bool flag2 = false;
				GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NeedGoods[i]._NeedGoodsID, myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount, false, out flag, out flag2, false);
			}
			Global.SubRoleMoneyForGoods(client, myTreasureEventConfig.NeedValue.Num, (int)myTreasureEventConfig.NeedValue.Type, "藏宝秘境");
			return OnePieceTreasureErrorCode.OnePiece_Success;
		}

		public OnePieceTreasureErrorCode TriggerEventMove(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode result;
			if (myTreasureEventConfig.MoveRange == null || myTreasureEventConfig.MoveRange.Count == 0)
			{
				result = OnePieceTreasureErrorCode.OnePiece_ErrorMoveRange;
			}
			else
			{
				int randomNumber = Global.GetRandomNumber(0, myTreasureEventConfig.MoveRange.Count);
				int num = myTreasureEventConfig.MoveRange[randomNumber];
				client.ClientData.OnePieceMoveLeft = num;
				client.ClientData.OnePieceMoveDir = num;
				this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, num, 0, null);
				result = OnePieceTreasureErrorCode.OnePiece_Success;
			}
			return result;
		}

		public OnePieceTreasureErrorCode TriggerEventCombat(GameClient client, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			SystemXmlItem systemXmlItem = null;
			OnePieceTreasureErrorCode result;
			if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(myTreasureEventConfig.FuBenID, out systemXmlItem))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(509, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("MapCode", -1);
				string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (array == null || array.Length < 2)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(510, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
				}
				else
				{
					int fuBenSeqID = Global.SafeConvertToInt32(array[1]);
					Global.UpdateFuBenData(client, myTreasureEventConfig.FuBenID, 1, 0);
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(intValue, out gameMap))
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(511, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
					}
					else
					{
						client.ClientData.FuBenSeqID = fuBenSeqID;
						client.ClientData.FuBenID = myTreasureEventConfig.FuBenID;
						FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, client.ClientData.FuBenSeqID, 0, myTreasureEventConfig.FuBenID);
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, intValue, -1, -1, -1, 0);
						client.ClientData.OnePieceTempEventID = myTreasureEventConfig.ID;
						result = OnePieceTreasureErrorCode.OnePiece_Success;
					}
				}
			}
			return result;
		}

		public int GiveOnePieceBoxAward(GameClient client, OnePieceTreasureBoxConfig myBoxConfig)
		{
			int result = 0;
			if (myBoxConfig.Type == TeasureBoxType.ETBT_Goods)
			{
				List<GoodsData> list = Global.ConvertToGoodsDataList(myBoxConfig.Goods.Items, -1);
				if (!Global.CanAddGoodsDataList(client, list))
				{
					foreach (GoodsData goodsData in list)
					{
						Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(508, new object[0]), GLang.GetLang(508, new object[0]), 1.0);
					}
					result = 16;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						GoodsData goodsData2 = list[i];
						if (null != goodsData2)
						{
							goodsData2.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, "获得藏宝秘境奖励", goodsData2.Endtime, goodsData2.AddPropIndex, goodsData2.BornIndex, goodsData2.Lucky, goodsData2.Strong, 0, 0, 0, null, null, 0, true);
						}
					}
				}
			}
			else if (myBoxConfig.Type == TeasureBoxType.ETBT_BaoZangJiFen)
			{
				GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myBoxConfig.Num, "获得藏宝秘境奖励", true);
			}
			else if (myBoxConfig.Type == TeasureBoxType.ETBT_BaoZangXueZuan)
			{
				GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, myBoxConfig.Num, true, true);
			}
			return result;
		}

		public int TryGiveOnePieceBoxListAward(GameClient client)
		{
			List<int> onePieceBoxIDList = client.ClientData.OnePieceBoxIDList;
			int result;
			if (onePieceBoxIDList == null)
			{
				result = 4;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < onePieceBoxIDList.Count; i++)
				{
					int key = onePieceBoxIDList[i] / 1000;
					int num2 = onePieceBoxIDList[i] % 1000;
					List<OnePieceTreasureBoxConfig> list = null;
					if (this.TreasureBoxConfig.TryGetValue(key, out list))
					{
						if (num2 > 0 && num2 <= list.Count)
						{
							num = this.GiveOnePieceBoxAward(client, list[num2 - 1]);
						}
					}
				}
				client.ClientData.OnePieceBoxIDList = null;
				result = num;
			}
			return result;
		}

		public OnePieceTreasureErrorCode TriggerEventTreasureBox(GameClient client, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < myTreasureEventConfig.BoxList.Count; i++)
			{
				int openNum = myTreasureEventConfig.BoxList[i].OpenNum;
				List<OnePieceTreasureBoxConfig> list2 = null;
				if (this.TreasureBoxConfig.TryGetValue(myTreasureEventConfig.BoxList[i].BoxID, out list2))
				{
					for (int j = 0; j < openNum; j++)
					{
						int beginNum = list2[0].BeginNum;
						int maxV = list2[list2.Count - 1].EndNum + 1;
						int randomNumber = Global.GetRandomNumber(beginNum, maxV);
						for (int k = 0; k < list2.Count; k++)
						{
							if (randomNumber <= list2[k].EndNum)
							{
								int item = myTreasureEventConfig.BoxList[i].BoxID * 1000 + list2[k].ID;
								list.Add(item);
								break;
							}
						}
					}
				}
			}
			client.ClientData.OnePieceBoxIDList = list;
			this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, 0, 0, list);
			return OnePieceTreasureErrorCode.OnePiece_Success;
		}

		public OnePieceTreasureErrorCode TriggerEvent(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode result;
			switch (myTreasureEventConfig.Type)
			{
			case TreasureEventType.ETET_Award:
				result = this.TriggerEventAward(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Excharge:
				result = this.TriggerEventExcharge(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Move:
				result = this.TriggerEventMove(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Combat:
				result = this.TriggerEventCombat(client, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_TreasureBox:
				result = this.TriggerEventTreasureBox(client, myTreasureEventConfig);
				break;
			default:
				result = OnePieceTreasureErrorCode.OnePiece_ErrorNotHaveEvent;
				break;
			}
			return result;
		}

		public bool OnePieceMoveTrigger(GameClient client, ref OnePieceTreasureData myOnePieceData, OnePieceTreasureMapConfig myTreasureMapConfig, TriggerType Trigger)
		{
			bool result;
			if (Trigger != myTreasureMapConfig.Trigger)
			{
				result = false;
			}
			else
			{
				if (myTreasureMapConfig.Score > 0 && Trigger == TriggerType.ETT_Stay)
				{
					GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myTreasureMapConfig.Score, "获得藏宝秘境奖励", true);
				}
				int num = this.RandomTreasureEvent(myTreasureMapConfig.LisRandomEvent);
				OnePieceTreasureEventConfig onePieceTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(num, out onePieceTreasureEventConfig))
				{
					result = false;
				}
				else
				{
					if (onePieceTreasureEventConfig.Type == TreasureEventType.ETET_Combat || onePieceTreasureEventConfig.Type == TreasureEventType.ETET_Excharge)
					{
						myOnePieceData.EventID = num;
						this.SyncOnePieceEvent(client, onePieceTreasureEventConfig.ID, 0, 0, null);
					}
					else
					{
						this.TriggerEvent(client, myOnePieceData, onePieceTreasureEventConfig);
					}
					result = true;
				}
			}
			return result;
		}

		public void HandleRoleLogout(GameClient client)
		{
			if (this.IfCanContinueMove(client))
			{
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				for (int i = 0; i < this.SystemParamsTreasureDice.Count + 1; i++)
				{
					this.TryGiveOnePieceBoxListAward(client);
					this.HandleOnePieceTreasureMove(client, client.ClientData.OnePieceMoveLeft, onePieceTreasureData);
					if (!this.IfCanContinueMove(client))
					{
						break;
					}
				}
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
			}
		}

		public int CalculateMoveCellToNextEvent(GameClient client, int MoveCellNum, OnePieceTreasureData myOnePieceData)
		{
			int result;
			if (MoveCellNum == 0)
			{
				result = myOnePieceData.PosID;
			}
			else
			{
				OnePieceTreasureMapConfig onePieceTreasureMapConfig = null;
				int num = myOnePieceData.PosID;
				for (int i = 0; i < Math.Abs(MoveCellNum); i++)
				{
					if (MoveCellNum > 0)
					{
						num = this.GetNextPosIDForEvent(num, true);
					}
					else
					{
						num = this.GetNextPosIDForEvent(num, false);
					}
					if (!this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig))
					{
						break;
					}
					if (onePieceTreasureMapConfig.Trigger == TriggerType.ETT_Pass)
					{
						return num;
					}
				}
				result = num;
			}
			return result;
		}

		public void HandleOnePieceTreasureMove(GameClient client, int MoveCellNum, OnePieceTreasureData myOnePieceData)
		{
			OnePieceTreasureMapConfig onePieceTreasureMapConfig = null;
			int num = myOnePieceData.PosID;
			for (int i = 0; i < Math.Abs(MoveCellNum); i++)
			{
				if (MoveCellNum > 0)
				{
					num = this.GetNextPosIDForEvent(myOnePieceData.PosID, true);
				}
				else
				{
					num = this.GetNextPosIDForEvent(myOnePieceData.PosID, false);
				}
				if (!this.TreasureMapConfig.TryGetValue(num, out onePieceTreasureMapConfig))
				{
					break;
				}
				myOnePieceData.PosID = num;
				if (MoveCellNum > 0)
				{
					client.ClientData.OnePieceMoveLeft--;
				}
				else
				{
					client.ClientData.OnePieceMoveLeft++;
				}
				if (onePieceTreasureMapConfig.Trigger == TriggerType.ETT_Pass)
				{
					break;
				}
			}
			if (onePieceTreasureMapConfig != null && MoveCellNum != 0)
			{
				this.OnePieceMoveTrigger(client, ref myOnePieceData, onePieceTreasureMapConfig, TriggerType.ETT_Pass);
			}
			if (onePieceTreasureMapConfig != null && client.ClientData.OnePieceMoveLeft == 0 && MoveCellNum != 0)
			{
				this.OnePieceMoveTrigger(client, ref myOnePieceData, onePieceTreasureMapConfig, TriggerType.ETT_Stay);
			}
			myOnePieceData.PosID = this.FilterPosIDChangeFloor(client, myOnePieceData.PosID);
		}

		public void GM_SetDice(GameClient client, int diceType, int newNum)
		{
			OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
			this.GetOnePieceTreasureData(client, onePieceTreasureData);
			if (diceType == 0)
			{
				this.ModifyOnePieceDice(client, onePieceTreasureData, diceType, newNum - onePieceTreasureData.RollNumNormal);
			}
			else
			{
				if (diceType != 1)
				{
					return;
				}
				this.ModifyOnePieceDice(client, onePieceTreasureData, diceType, newNum - onePieceTreasureData.RollNumMiracle);
			}
			this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
		}

		public void GM_SetPosID(GameClient client, int posid)
		{
			OnePieceTreasureMapConfig onePieceTreasureMapConfig = null;
			if (this.TreasureMapConfig.TryGetValue(posid, out onePieceTreasureMapConfig))
			{
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				onePieceTreasureData.PosID = posid;
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				byte[] buffer = DataHelper.ObjectToBytes<OnePieceTreasureData>(onePieceTreasureData);
				GameManager.ClientMgr.SendToClient(client, buffer, 1600);
			}
		}

		public void GM_PrintTreasureData(GameClient client)
		{
			OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
			OnePieceManager.getInstance().GetOnePieceTreasureData(client, onePieceTreasureData);
			string textMsg = string.Format("藏宝秘境 位置PosID[{0}] MoveLeft[{1}] RollNumNormal[{2}] RollNumMiracle[{3}] JiFen[{4}] XueZuan[{5}]", new object[]
			{
				onePieceTreasureData.PosID,
				client.ClientData.OnePieceMoveLeft,
				onePieceTreasureData.RollNumNormal,
				onePieceTreasureData.RollNumMiracle,
				GameManager.ClientMgr.GetTreasureJiFen(client),
				GameManager.ClientMgr.GetTreasureXueZuan(client)
			});
			GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
		}

		public void UpdateOnePieceTreasureLogDB(GameClient client, OnePieceTreasureLogType LogType, int addValue = 1)
		{
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.Building, LogRecordType.IntValue, new object[]
			{
				LogType,
				addValue
			});
		}

		public bool ProcessOnePieceGetInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				this.HandleRoleLogout(client);
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData myTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myTreasureData);
				byte[] buffer = DataHelper.ObjectToBytes<OnePieceTreasureData>(myTreasureData);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOnePieceRollMiracleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				string cmdData;
				if (this.IfCanContinueMove(client))
				{
					num = 5;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				if (num2 <= 0 || num2 > this.SystemParamsTreasureDice.Count)
				{
					num = 3;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				if (onePieceTreasureData.RollNumMiracle < 1)
				{
					num = 15;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				onePieceTreasureData.EventID = 0;
				client.ClientData.OnePieceMoveLeft = num2;
				client.ClientData.OnePieceMoveDir = num2;
				int num3 = this.CalculateMoveCellToNextEvent(client, num2, onePieceTreasureData);
				onePieceTreasureData.RollNumMiracle--;
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				cmdData = string.Format("{0}:{1}", num, num2);
				client.sendCmd(nID, cmdData, false);
				cmdData = string.Format("{0}:{1}", num, num3);
				client.sendCmd(1604, cmdData, false);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_Role, 1);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_MoveNum, num2);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOnePieceRollCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				string cmdData;
				if (this.IfCanContinueMove(client))
				{
					num = 5;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				if (onePieceTreasureData.RollNumNormal < 1)
				{
					num = 15;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				onePieceTreasureData.EventID = 0;
				int num2 = this.RollMoveNum();
				client.ClientData.OnePieceMoveLeft = num2;
				client.ClientData.OnePieceMoveDir = num2;
				int num3 = this.CalculateMoveCellToNextEvent(client, num2, onePieceTreasureData);
				onePieceTreasureData.RollNumNormal--;
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				cmdData = string.Format("{0}:{1}", num, num2);
				client.sendCmd(nID, cmdData, false);
				cmdData = string.Format("{0}:{1}", num, num3);
				client.sendCmd(1604, cmdData, false);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_Role, 1);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_MoveNum, num2);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOnePieceDiceBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string cmdData = "";
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				if (num3 <= 0 || num3 > 99)
				{
					num = 14;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				int num4;
				if (num2 == 0)
				{
					if (onePieceTreasureData.RollNumNormal + num3 > 99)
					{
						num = 14;
						cmdData = string.Format("{0}:{1}:{2}", num, num2, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					num4 = num3 * this.SystemParamsTreasurePrice;
				}
				else
				{
					if (num2 != 1)
					{
						num = 3;
						cmdData = string.Format("{0}:{1}:{2}", num, num2, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					if (onePieceTreasureData.RollNumMiracle + num3 > 99)
					{
						num = 14;
						cmdData = string.Format("{0}:{1}:{2}", num, num2, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					num4 = num3 * this.SystemParamsTreasureSuperPrice;
				}
				if (client.ClientData.UserMoney < num4)
				{
					num = 1;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (num4 > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num4, "藏宝秘境买骰子", true, true, false, DaiBiSySType.None))
					{
						num = 1;
						cmdData = string.Format("{0}:{1}", num, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				if (num2 == 0)
				{
					this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_BuyDice, num3);
					onePieceTreasureData.RollNumNormal += num3;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, onePieceTreasureData.RollNumNormal);
				}
				else if (num2 == 1)
				{
					this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_BuySuperDice, num3);
					onePieceTreasureData.RollNumMiracle += num3;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, onePieceTreasureData.RollNumMiracle);
				}
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool IfHaveOnePieceBoxListAward(GameClient client)
		{
			return client.ClientData.OnePieceBoxIDList != null;
		}

		public bool IfCanContinueMove(GameClient client)
		{
			return client.ClientData.OnePieceMoveLeft != 0 || this.IfHaveOnePieceBoxListAward(client);
		}

		public bool ProcessOnePieceMoveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				string cmdData;
				if (!this.IfCanContinueMove(client))
				{
					num = 12;
					cmdData = string.Format("{0}:{1}", num, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				if (this.IfHaveOnePieceBoxListAward(client))
				{
					num = this.TryGiveOnePieceBoxListAward(client);
					onePieceTreasureData.PosID = this.FilterPosIDChangeFloor(client, onePieceTreasureData.PosID);
				}
				else
				{
					this.HandleOnePieceTreasureMove(client, client.ClientData.OnePieceMoveLeft, onePieceTreasureData);
				}
				int num2 = onePieceTreasureData.PosID;
				if (!this.IfHaveOnePieceBoxListAward(client))
				{
					num2 = this.CalculateMoveCellToNextEvent(client, client.ClientData.OnePieceMoveLeft, onePieceTreasureData);
				}
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				cmdData = string.Format("{0}:{1}", num, num2);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOnePieceTriggerEventCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num;
				string cmdData;
				if (client.ClientData.OnePieceMoveLeft != 0)
				{
					num = 5;
					cmdData = string.Format("{0}:{1}", num, -1);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				OnePieceTreasureData onePieceTreasureData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, onePieceTreasureData);
				if (onePieceTreasureData.EventID == 0)
				{
					num = 6;
					cmdData = string.Format("{0}:{1}", num, -1);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				OnePieceTreasureEventConfig onePieceTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(onePieceTreasureData.EventID, out onePieceTreasureEventConfig))
				{
					num = 6;
					cmdData = string.Format("{0}:{1}", num, -1);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				num = (int)this.TriggerEvent(client, onePieceTreasureData, onePieceTreasureEventConfig);
				if (num == 0 || num == 16)
				{
					onePieceTreasureData.EventID = 0;
				}
				this.ModifyOnePieceTreasureData(client, onePieceTreasureData);
				cmdData = string.Format("{0}:{1}", num, (int)onePieceTreasureEventConfig.Type);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("TreasureDice");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						this.SystemParamsTreasureDice.Add(Global.SafeConvertToDouble(array2[1]));
					}
				}
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("TreasureFreeNum");
			if (!string.IsNullOrEmpty(paramValueByName2))
			{
				string[] array = paramValueByName2.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					this.SystemParamsTreasureFreeNum = Global.SafeConvertToInt32(array[0]);
					this.SystemParamsTreasureMiracleNum = Global.SafeConvertToInt32(array[1]);
				}
			}
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("TreasurePrice");
			if (!string.IsNullOrEmpty(paramValueByName3))
			{
				this.SystemParamsTreasurePrice = Global.SafeConvertToInt32(paramValueByName3);
			}
			string paramValueByName4 = GameManager.systemParamsList.GetParamValueByName("TreasureSuperPrice");
			if (!string.IsNullOrEmpty(paramValueByName4))
			{
				this.SystemParamsTreasureSuperPrice = Global.SafeConvertToInt32(paramValueByName4);
			}
			return this.LoadOnePieceTreasureMapFile() && this.LoadOnePieceTreasureEventFile() && this.LoadOnePieceTreasureBoxFile();
		}

		public bool LoadOnePieceTreasureMapFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureMap.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						OnePieceTreasureMapConfig onePieceTreasureMapConfig = new OnePieceTreasureMapConfig();
						onePieceTreasureMapConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						onePieceTreasureMapConfig.Num = (int)Global.GetSafeAttributeLong(xelement2, "Num");
						onePieceTreasureMapConfig.Floor = (int)Global.GetSafeAttributeLong(xelement2, "Floor");
						onePieceTreasureMapConfig.Trigger = (TriggerType)Global.GetSafeAttributeLong(xelement2, "Trigger");
						onePieceTreasureMapConfig.Score = (int)Global.GetSafeAttributeLong(xelement2, "Score");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Event");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取TreasureMap.xml中的Event失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split(new char[]
								{
									','
								});
								if (array2.Length == 2)
								{
									OnePieceRandomEvent onePieceRandomEvent = new OnePieceRandomEvent();
									onePieceRandomEvent.EventID = Global.SafeConvertToInt32(array2[0]);
									onePieceRandomEvent.Rate = Global.SafeConvertToDouble(array2[1]);
									onePieceTreasureMapConfig.LisRandomEvent.Add(onePieceRandomEvent);
								}
							}
						}
						this.TreasureMapConfig[onePieceTreasureMapConfig.ID] = onePieceTreasureMapConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "TreasureMap.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadOnePieceTreasureEventFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureEvent.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						OnePieceTreasureEventConfig onePieceTreasureEventConfig = new OnePieceTreasureEventConfig();
						onePieceTreasureEventConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						onePieceTreasureEventConfig.Type = (TreasureEventType)Global.GetSafeAttributeLong(xelement2, "Type");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "NewGoods");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							ConfigParser.ParseAwardsItemList(safeAttributeStr, ref onePieceTreasureEventConfig.GoodsList, '|', ',');
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "NewValue");
						if (!string.IsNullOrEmpty(safeAttributeStr2))
						{
							string[] array = safeAttributeStr2.Split(new char[]
							{
								','
							});
							if (array.Length == 2)
							{
								onePieceTreasureEventConfig.NewValue.Type = (MoneyTypes)Global.SafeConvertToInt32(array[0]);
								onePieceTreasureEventConfig.NewValue.Num = Global.SafeConvertToInt32(array[1]);
							}
						}
						if (string.IsNullOrEmpty(safeAttributeStr) && string.IsNullOrEmpty(safeAttributeStr2))
						{
							LogManager.WriteLog(1, string.Format("读取TreasureEvent.xml奖励配置项1失败", new object[0]), null, true);
						}
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "NeedGoods");
						if (!string.IsNullOrEmpty(safeAttributeStr2))
						{
							string[] array = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split(new char[]
								{
									','
								});
								if (array2.Length == 2)
								{
									OnePieceGoodsPair onePieceGoodsPair = new OnePieceGoodsPair();
									onePieceGoodsPair._NeedGoodsID = Global.SafeConvertToInt32(array2[0]);
									onePieceGoodsPair._NeedGoodsCount = Global.SafeConvertToInt32(array2[1]);
									onePieceTreasureEventConfig.NeedGoods.Add(onePieceGoodsPair);
								}
							}
						}
						string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement2, "NeedValue");
						if (!string.IsNullOrEmpty(safeAttributeStr4))
						{
							string[] array = safeAttributeStr4.Split(new char[]
							{
								','
							});
							if (array.Length == 2)
							{
								onePieceTreasureEventConfig.NeedValue.Type = (MoneyTypes)Global.SafeConvertToInt32(array[0]);
								onePieceTreasureEventConfig.NeedValue.Num = Global.SafeConvertToInt32(array[1]);
							}
						}
						string safeAttributeStr5 = Global.GetSafeAttributeStr(xelement2, "Move");
						if (!string.IsNullOrEmpty(safeAttributeStr5))
						{
							string[] array = safeAttributeStr5.Split(new char[]
							{
								','
							});
							for (int i = 0; i < array.Length; i++)
							{
								onePieceTreasureEventConfig.MoveRange.Add(Global.SafeConvertToInt32(array[i]));
							}
						}
						onePieceTreasureEventConfig.NewDiec = (int)Global.GetSafeAttributeLong(xelement2, "NewDiec");
						onePieceTreasureEventConfig.NewSuperDiec = (int)Global.GetSafeAttributeLong(xelement2, "NewSuperDiec");
						onePieceTreasureEventConfig.FuBenID = (int)Global.GetSafeAttributeLong(xelement2, "FuBenID");
						string safeAttributeStr6 = Global.GetSafeAttributeStr(xelement2, "Box");
						if (!string.IsNullOrEmpty(safeAttributeStr6))
						{
							string[] array = safeAttributeStr6.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array.Length; i++)
							{
								string[] array3 = array[i].Split(new char[]
								{
									','
								});
								if (array3.Length == 2)
								{
									OnePieceTreasureBoxPair onePieceTreasureBoxPair = new OnePieceTreasureBoxPair();
									onePieceTreasureBoxPair.BoxID = Global.SafeConvertToInt32(array3[0]);
									onePieceTreasureBoxPair.OpenNum = Global.SafeConvertToInt32(array3[1]);
									onePieceTreasureEventConfig.BoxList.Add(onePieceTreasureBoxPair);
								}
							}
						}
						this.TreasureEventConfig[onePieceTreasureEventConfig.ID] = onePieceTreasureEventConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "TreasureEvent.xml", ex.Message), null, true);
			}
			return true;
		}

		public bool LoadOnePieceTreasureBoxFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureBox.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						List<OnePieceTreasureBoxConfig> list = new List<OnePieceTreasureBoxConfig>();
						int key = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						IEnumerable<XElement> enumerable2 = xelement2.Elements();
						foreach (XElement xml in enumerable2)
						{
							OnePieceTreasureBoxConfig onePieceTreasureBoxConfig = new OnePieceTreasureBoxConfig();
							onePieceTreasureBoxConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
							onePieceTreasureBoxConfig.Type = (TeasureBoxType)Global.GetSafeAttributeLong(xml, "Type");
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Goods");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取TreasureBox.xml奖励配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									','
								});
								if (array.Length != 1)
								{
									ConfigParser.ParseAwardsItemList(safeAttributeStr, ref onePieceTreasureBoxConfig.Goods, '|', ',');
								}
								else
								{
									onePieceTreasureBoxConfig.Num = Global.SafeConvertToInt32(safeAttributeStr);
								}
							}
							onePieceTreasureBoxConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
							onePieceTreasureBoxConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
							list.Add(onePieceTreasureBoxConfig);
						}
						this.TreasureBoxConfig[key] = list;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "TreasureBox.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string OnePiece_TreasureMapfileName = "Config/Treasure/TreasureMap.xml";

		private const string OnePiece_TreasureEventfileName = "Config/Treasure/TreasureEvent.xml";

		private const string OnePiece_TreasureBoxfileName = "Config/Treasure/TreasureBox.xml";

		private const int OnePiece_FloorHashNum = 1000;

		private const int OnePiece_FloorCellNum = 30;

		private const int OnePiece_DiceMaxNum = 99;

		public List<double> SystemParamsTreasureDice = new List<double>();

		public int SystemParamsTreasureFreeNum = 0;

		public int SystemParamsTreasureMiracleNum = 0;

		public int SystemParamsTreasurePrice = 0;

		public int SystemParamsTreasureSuperPrice = 0;

		public int OnePiece_FakeRollNum_GM = 0;

		private static OnePieceManager instance = new OnePieceManager();

		public Dictionary<int, OnePieceTreasureMapConfig> TreasureMapConfig = new Dictionary<int, OnePieceTreasureMapConfig>();

		public Dictionary<int, OnePieceTreasureEventConfig> TreasureEventConfig = new Dictionary<int, OnePieceTreasureEventConfig>();

		public Dictionary<int, List<OnePieceTreasureBoxConfig>> TreasureBoxConfig = new Dictionary<int, List<OnePieceTreasureBoxConfig>>();
	}
}
