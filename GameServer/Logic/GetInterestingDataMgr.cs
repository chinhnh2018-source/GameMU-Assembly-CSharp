using System;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	internal class GetInterestingDataMgr : SingletonTemplate<GetInterestingDataMgr>
	{
		private GetInterestingDataMgr()
		{
		}

		public void LoadConfig()
		{
			GetInterestingDataMgr.GetIntervalMs = GameManager.systemParamsList.GetParamValueIntByName("GetIntervalMs", -1);
			if (GetInterestingDataMgr.GetIntervalMs < 30000L)
			{
				GetInterestingDataMgr.GetIntervalMs = 180000L;
			}
		}

		public void Update(GameClient client)
		{
			if (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("GetInterestingData"))
			{
				if (client != null)
				{
					int num = 0;
					int num2 = 0;
					long num3 = TimeUtil.NOW();
					lock (client.InterestingData)
					{
						if (client.ClientData.FirstPlayStart)
						{
							return;
						}
						for (int i = 0; i < client.InterestingData.itemArray.Length; i++)
						{
							InterestingData.Item item = client.InterestingData.itemArray[i];
							if (item != null)
							{
								num += item.RequestCount - item.ResponseCount;
								num2 += item.InvalidCount;
								if (item.RequestCount >= 2 || item.LastRequestMs + GetInterestingDataMgr._FirstGetIntervalMs <= num3)
								{
									if (item.RequestCount < 2 || item.LastRequestMs + GetInterestingDataMgr.GetIntervalMs <= num3)
									{
										if (!client.ClientSocket.session.IsGM)
										{
											if (i == 1)
											{
												RobotTaskValidator.getInstance().SendTaskListKey(client);
											}
											client.sendCmd(14004, string.Format("{0}", i), false);
											item.LastRequestMs = num3;
											item.RequestCount++;
										}
									}
								}
							}
						}
					}
					if (num > 10)
					{
					}
					if (num2 > 10)
					{
					}
				}
			}
		}

		public TCPProcessCmdResults OnResponse(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string[] array = new UTF8Encoding().GetString(data, 0, count).Split(new char[]
				{
					':'
				});
				if (array == null || array.Length <= 2)
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (num2 < 0 || num2 >= 2)
				{
					LogManager.WriteLog(2, string.Format("角色返回敏感数据索引错误,roleid={0}, rolename={1}, index={2}", num, gameClient.ClientData.RoleName, num2), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				lock (gameClient.InterestingData)
				{
					InterestingData.Item item = gameClient.InterestingData.itemArray[num2];
					if (item != null)
					{
						item.LastResponseMs = TimeUtil.NOW();
						item.ResponseCount++;
						if (num2 == 0)
						{
							this._CheckSpeed(gameClient, item, array);
						}
					}
				}
			}
			catch
			{
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		private void _CheckSpeed(GameClient client, InterestingData.Item item, string[] fields)
		{
			double num = Convert.ToDouble(fields[2]);
			client.InterestingData.Speed = num;
			double num2 = RoleAlgorithm.GetMoveSpeed(client);
			if (client.ClientData.HorseDbID > 0)
			{
				num2 += Global.GetHorseSpeed(client);
			}
			if (num > num2 * 1.4)
			{
				item.InvalidCount++;
			}
		}

		private static long GetIntervalMs = 180000L;

		private static long _FirstGetIntervalMs = 30000L;
	}
}
