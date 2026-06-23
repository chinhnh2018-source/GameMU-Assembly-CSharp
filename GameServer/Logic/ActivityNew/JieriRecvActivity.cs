using System;

namespace GameServer.Logic.ActivityNew
{
	public class JieriRecvActivity : JieriGiveRecv_Base
	{
		public override string GetConfigFile()
		{
			return "Config/JieRiGifts/JieRiShouQu.xml";
		}

		public override string QueryActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = "0:0";
			}
			else
			{
				RoleGiveRecvInfo roleGiveRecvInfo = base.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				lock (roleGiveRecvInfo)
				{
					result = string.Format("{0}:{1}", roleGiveRecvInfo.TotalRecv, roleGiveRecvInfo.AwardFlag);
				}
			}
			return result;
		}

		public override void FlushIcon(GameClient client)
		{
			if (client != null && client._IconStateMgr.CheckJieriRecv(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		public override bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			return info != null && info.TotalRecv >= condValue;
		}

		public void OnRecv(int roleid, int goodsCnt)
		{
			if (this.InActivityTime())
			{
				bool flag;
				RoleGiveRecvInfo roleGiveRecvInfo = base.GetRoleGiveRecvInfo(roleid, out flag);
				if (roleGiveRecvInfo != null)
				{
					if (!flag)
					{
						lock (roleGiveRecvInfo)
						{
							roleGiveRecvInfo.TotalRecv += goodsCnt;
						}
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
					if (gameClient != null)
					{
						if (gameClient._IconStateMgr.CheckJieriRecv(gameClient))
						{
							gameClient._IconStateMgr.AddFlushIconState(14000, gameClient._IconStateMgr.IsAnyJieRiTipActived());
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
						}
					}
				}
			}
		}
	}
}
