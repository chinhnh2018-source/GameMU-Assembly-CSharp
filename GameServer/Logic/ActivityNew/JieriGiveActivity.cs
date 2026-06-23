using System;

namespace GameServer.Logic.ActivityNew
{
	public class JieriGiveActivity : JieriGiveRecv_Base
	{
		public override string GetConfigFile()
		{
			return "Config/JieRiGifts/JieRiZengSong.xml";
		}

		public override string QueryActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = "0:0:0";
			}
			else
			{
				RoleGiveRecvInfo roleGiveRecvInfo = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				lock (roleGiveRecvInfo)
				{
					result = string.Format("{0}:{1}:{2}", roleGiveRecvInfo.TotalGive, roleGiveRecvInfo.TotalRecv, roleGiveRecvInfo.AwardFlag);
				}
			}
			return result;
		}

		public override void FlushIcon(GameClient client)
		{
			if (client != null && client._IconStateMgr.CheckJieriGive(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		public override bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			return info != null && info.TotalGive >= condValue;
		}

		public string ProcRoleGiveToOther(GameClient client, string receiverRolename, int goodsID, int goodsCnt)
		{
			int num = -1;
			JieriGiveErrorCode jieriGiveErrorCode = JieriGiveErrorCode.Success;
			if (!this.InActivityTime())
			{
				jieriGiveErrorCode = JieriGiveErrorCode.ActivityNotOpen;
			}
			else if (string.IsNullOrEmpty(receiverRolename) || receiverRolename == client.ClientData.RoleName)
			{
				jieriGiveErrorCode = JieriGiveErrorCode.ReceiverCannotSelf;
			}
			else if (!base.IsGiveGoodsID(goodsID))
			{
				jieriGiveErrorCode = JieriGiveErrorCode.GoodsIDError;
			}
			else if (goodsCnt <= 0 || Global.GetTotalGoodsCountByID(client, goodsID) < goodsCnt)
			{
				jieriGiveErrorCode = JieriGiveErrorCode.GoodsNotEnough;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					receiverRolename,
					goodsID,
					goodsCnt
				});
				string[] array = Global.ExecuteDBCmd(13200, strcmd, client.ServerId);
				if (array == null || array.Length != 1)
				{
					jieriGiveErrorCode = JieriGiveErrorCode.DBFailed;
				}
				else
				{
					num = Convert.ToInt32(array[0]);
					if (num == -1)
					{
						jieriGiveErrorCode = JieriGiveErrorCode.ReceiverNotExist;
					}
					else if (num <= 0)
					{
						jieriGiveErrorCode = JieriGiveErrorCode.DBFailed;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsCnt, false, out flag, out flag2, false))
						{
							jieriGiveErrorCode = JieriGiveErrorCode.GoodsNotEnough;
						}
						else
						{
							GameManager.logDBCmdMgr.AddMessageLog(0, Global.GetGoodsNameByID(goodsID), "节日赠送", client.ClientData.RoleName, receiverRolename, "日志", goodsCnt, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, "");
							jieriGiveErrorCode = JieriGiveErrorCode.Success;
						}
					}
				}
			}
			RoleGiveRecvInfo roleGiveRecvInfo = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
			if (jieriGiveErrorCode == JieriGiveErrorCode.Success)
			{
				lock (roleGiveRecvInfo)
				{
					roleGiveRecvInfo.TotalGive += goodsCnt;
				}
				if (client._IconStateMgr.CheckJieriGive(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
				JieRiGiveKingActivity jieriGiveKingActivity = HuodongCachingMgr.GetJieriGiveKingActivity();
				if (jieriGiveKingActivity != null)
				{
					jieriGiveKingActivity.OnGive(client, goodsID, goodsCnt);
				}
				JieriRecvActivity jieriRecvActivity = HuodongCachingMgr.GetJieriRecvActivity();
				if (jieriRecvActivity != null)
				{
					jieriRecvActivity.OnRecv(num, goodsCnt);
				}
				JieRiRecvKingActivity jieriRecvKingActivity = HuodongCachingMgr.GetJieriRecvKingActivity();
				if (jieriRecvKingActivity != null)
				{
					jieriRecvKingActivity.OnRecv(num, goodsID, goodsCnt, client.ServerId);
				}
			}
			string result;
			lock (roleGiveRecvInfo)
			{
				result = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)jieriGiveErrorCode,
					roleGiveRecvInfo.TotalGive,
					roleGiveRecvInfo.TotalRecv,
					roleGiveRecvInfo.AwardFlag
				});
			}
			return result;
		}
	}
}
