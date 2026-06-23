using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
	{
		private ZhanMengBuildUpLevelCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(601, 4, this);
		}

		public static ZhanMengBuildUpLevelCmdProcessor getInstance()
		{
			return ZhanMengBuildUpLevelCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			int num3 = Global.SafeConvertToInt32(cmdParams[2]);
			int num4 = Math.Max(2, Global.SafeConvertToInt32(cmdParams[3]));
			int num5 = 601;
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
			else if (num4 > Global.MaxBangHuiFlagLevel)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-2,
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
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(num, num2, 0);
					int totalMoney = bangHuiDetailData.TotalMoney;
					int intValue = zhanMengBuildItem.GetIntValue("LevelupCost", -1);
					string stringValue = zhanMengBuildItem.GetStringValue("NeedGoods");
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
					{
						num,
						num2,
						num3,
						intValue,
						num4,
						Global.GetZhanMengInitCoin(),
						stringValue
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
						if (num6 >= 0)
						{
							Global.BroadcastZhanMengBuildUpLevelHint(client, num3, num4);
							cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								num4,
								num,
								num2,
								num3,
								intValue
							});
							JunQiManager.NotifySyncBangHuiJunQiItemsDict(client);
							GameManager.ClientMgr.NotifyBangHuiUpLevel(num2, client.ServerId, num4, client.ClientSocket.IsKuaFuLogin);
							BangHuiDetailData bangHuiDetailData2 = Global.GetBangHuiDetailData(num, num2, 0);
							int totalMoney2 = bangHuiDetailData2.TotalMoney;
							string resList = EventLogManager.NewResPropString(ResLogType.BangHuiMoney, new object[]
							{
								-intValue,
								totalMoney,
								totalMoney2
							});
							EventLogManager.AddBangHuiBuildUpEvent(client, num2, num3, num4, resList);
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
			return result;
		}

		private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();
	}
}
