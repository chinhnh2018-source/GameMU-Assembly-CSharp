using System;
using GameDBServer.Logic.WanMoTa;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class ModifyWanMoTaCmdProcessor : ICmdProcessor
	{
		private ModifyWanMoTaCmdProcessor()
		{
		}

		public static ModifyWanMoTaCmdProcessor getInstance()
		{
			return ModifyWanMoTaCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ModifyWanMotaData modifyWanMotaData = DataHelper.BytesToObject<ModifyWanMotaData>(cmdParams, 0, count);
				if (null == modifyWanMotaData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数传输错误, CMD={0}, CmdData={2}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, cmdParams), null, true);
					client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
				}
				else
				{
					string strParams = modifyWanMotaData.strParams;
					string[] array = strParams.Split(new char[]
					{
						':'
					});
					if (array.Length != 6)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, array.Length, cmdParams), null, true);
						client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
					}
					else
					{
						array[4] = modifyWanMotaData.strSweepReward;
						int num = Convert.ToInt32(array[0]);
						WanMotaInfo wanMoTaData = WanMoTaManager.getInstance().getWanMoTaData(num);
						if (null == wanMoTaData)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("没有找到相应的万魔塔信息，CMD={0}, RoleID={1}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, num), null, true);
							client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
						}
						else
						{
							int num2 = WanMoTaManager.getInstance().updateWanMoTaData(num, array, 1);
							if (num2 < 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("数据库更新万魔塔数据失败，CMD={0}, RoleID={1}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, num), null, true);
							}
							else
							{
								bool flag = false;
								lock (wanMoTaData)
								{
									wanMoTaData.lFlushTime = DataHelper.ConvertToInt64(array[1], wanMoTaData.lFlushTime);
									int num3 = DataHelper.ConvertToInt32(array[2], wanMoTaData.nPassLayerCount);
									wanMoTaData.nSweepLayer = DataHelper.ConvertToInt32(array[3], wanMoTaData.nSweepLayer);
									wanMoTaData.strSweepReward = DataHelper.ConvertToStr(array[4], wanMoTaData.strSweepReward);
									wanMoTaData.lSweepBeginTime = DataHelper.ConvertToInt64(array[5], wanMoTaData.lSweepBeginTime);
									if (num3 != wanMoTaData.nPassLayerCount)
									{
										wanMoTaData.nPassLayerCount = num3;
										flag = true;
									}
								}
								if (flag)
								{
									WanMoTaManager.getInstance().ModifyWanMoTaPaihangData(wanMoTaData, false);
								}
							}
							string cmdData = string.Format("{0}:{1}", num, num2);
							client.sendCmd(10158, cmdData);
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
				string cmdData = string.Format("{0}:{1}", 0, -1);
			}
		}

		private static ModifyWanMoTaCmdProcessor instance = new ModifyWanMoTaCmdProcessor();
	}
}
