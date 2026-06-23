using System;
using System.Text;
using GameDBServer.DB;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class ZhanMengBuildGetBufferCmdProcessor : ICmdProcessor
	{
		private ZhanMengBuildGetBufferCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(602, this);
		}

		public static ZhanMengBuildGetBufferCmdProcessor getInstance()
		{
			return ZhanMengBuildGetBufferCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] array = text.Split(new char[]
			{
				':'
			});
			if (array.Length != 5)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				int num4 = Convert.ToInt32(array[4]);
				DBManager dbmanager = DBManager.getInstance();
				DBRoleInfo dbroleInfo = dbmanager.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbmanager, bhid);
					if (null == bangHuiDetailData)
					{
						string cmdData = string.Format("{0}", -1000);
						client.sendCmd(nID, cmdData);
					}
					else
					{
						switch (num2)
						{
						case 1:
							if (num4 > bangHuiDetailData.QiLevel)
							{
								num4 = -1;
							}
							break;
						case 2:
							if (num4 > bangHuiDetailData.JiTan)
							{
								num4 = -1;
							}
							break;
						case 3:
							if (num4 > bangHuiDetailData.JunXie)
							{
								num4 = -1;
							}
							break;
						case 4:
							if (num4 > bangHuiDetailData.GuangHuan)
							{
								num4 = -1;
							}
							break;
						default:
							num4 = -1;
							break;
						}
						if (num4 < 0)
						{
							string cmdData = string.Format("{0}", -1110);
							client.sendCmd(nID, cmdData);
						}
						else if (dbroleInfo.BangGong < Math.Abs(num3))
						{
							string cmdData = string.Format("{0}", -1110);
							client.sendCmd(nID, cmdData);
						}
						else if (!DBWriter.UpdateRoleBangGong(dbmanager, num, dbroleInfo.BGDayID1, dbroleInfo.BGMoney, dbroleInfo.BGDayID2, dbroleInfo.BGGoods, dbroleInfo.BangGong - num3))
						{
							string cmdData = string.Format("{0}", -1110);
							client.sendCmd(nID, cmdData);
						}
						else
						{
							dbroleInfo.BangGong -= Math.Abs(num3);
							string cmdData = string.Format("{0}", 1);
							client.sendCmd(nID, cmdData);
						}
					}
				}
			}
		}

		private static ZhanMengBuildGetBufferCmdProcessor instance = new ZhanMengBuildGetBufferCmdProcessor();
	}
}
