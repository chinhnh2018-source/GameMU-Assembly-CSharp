using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	public class ZhanMengBuildGetBufferCmdProcessor : ICmdProcessor
	{
		private ZhanMengBuildGetBufferCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(602, 4, this);
		}

		public static ZhanMengBuildGetBufferCmdProcessor getInstance()
		{
			return ZhanMengBuildGetBufferCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			int num3 = Global.SafeConvertToInt32(cmdParams[2]);
			int num4 = Global.SafeConvertToInt32(cmdParams[3]);
			int num5 = 602;
			bool result;
			if (client.ClientData.Faction != num2)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-1,
					num,
					num2,
					num3,
					0
				});
				client.sendCmd(num5, cmdData, false);
				result = true;
			}
			else
			{
				SystemXmlItem zhanMengBuildItem = Global.GetZhanMengBuildItem(num3, num4);
				if (null == zhanMengBuildItem)
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-3,
						num,
						num2,
						num3,
						0
					});
					client.sendCmd(num5, cmdData, false);
					result = true;
				}
				else
				{
					int intValue = zhanMengBuildItem.GetIntValue("BuffTime", -1);
					int intValue2 = zhanMengBuildItem.GetIntValue("ConvertCost", -1);
					if (client.ClientData.BangGong < intValue2)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1110,
							num,
							num2,
							num3,
							0
						});
						client.sendCmd(num5, cmdData, false);
						result = true;
					}
					else
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							num,
							num2,
							num3,
							intValue2,
							num4
						});
						string[] array = Global.ExecuteDBCmd(num5, strcmd, client.ServerId);
						if (array == null || array.Length != 1)
						{
							LogManager.WriteLog(2, string.Format("升级帮旗等级时失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)num5, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), num), null, true);
							result = false;
						}
						else
						{
							int num6 = Global.SafeConvertToInt32(array[0]);
							string cmdData;
							if (num6 > 0)
							{
								client.ClientData.BangGong -= Math.Abs(intValue2);
								this.installBuff(client, num4 - 1, num3, intValue);
								cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									1,
									num,
									num2,
									num3,
									intValue2
								});
							}
							else
							{
								cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									num6,
									num,
									num2,
									num3,
									0
								});
							}
							client.sendCmd(num5, cmdData, false);
							result = true;
						}
					}
				}
			}
			return result;
		}

		private void installBuff(GameClient client, int nNewBufferGoodsIndexID, int buildType, int secs)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 88 + buildType - 1);
			double[] actionParams = new double[]
			{
				(double)secs,
				(double)nNewBufferGoodsIndexID
			};
			Global.UpdateBufferData(client, BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI + buildType - 1, actionParams, 0, true);
			GameManager.ClientMgr.NotifySelfBangGongChange(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		private static ZhanMengBuildGetBufferCmdProcessor instance = new ZhanMengBuildGetBufferCmdProcessor();
	}
}
