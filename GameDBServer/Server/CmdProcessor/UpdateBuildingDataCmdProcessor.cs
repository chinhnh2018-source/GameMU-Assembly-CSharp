using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class UpdateBuildingDataCmdProcessor : ICmdProcessor
	{
		private UpdateBuildingDataCmdProcessor()
		{
		}

		public static UpdateBuildingDataCmdProcessor getInstance()
		{
			return UpdateBuildingDataCmdProcessor.instance;
		}

		public void processCmdUpdateBuildLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (array.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				string keyTime = array[0];
				BuildingLogType logType = (BuildingLogType)Convert.ToInt32(array[1]);
				DBManager dbMgr = DBManager.getInstance();
				DBWriter.UpdateBuildingLog(dbMgr, keyTime, logType);
				string cmdData = string.Format("{0}", 0);
				client.sendCmd(nID, cmdData);
			}
		}

		public void processCmdUpdateBuildData(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (array.Length != 9)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				BuildingData buildingData = new BuildingData();
				int num = Convert.ToInt32(array[0]);
				buildingData.BuildId = Convert.ToInt32(array[1]);
				buildingData.BuildLev = Convert.ToInt32(array[2]);
				buildingData.BuildExp = Convert.ToInt32(array[3]);
				buildingData.TaskID_1 = Convert.ToInt32(array[5]);
				buildingData.TaskID_2 = Convert.ToInt32(array[6]);
				buildingData.TaskID_3 = Convert.ToInt32(array[7]);
				buildingData.TaskID_4 = Convert.ToInt32(array[8]);
				buildingData.BuildTime = array[4].Replace('$', ':');
				DBManager dbmanager = DBManager.getInstance();
				DBRoleInfo dbroleInfo = dbmanager.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.BuildingDataList)
						{
							dbroleInfo.BuildingDataList = new List<BuildingData>();
						}
						this.UpdateBuildData(dbroleInfo.BuildingDataList, buildingData);
					}
					DBWriter.UpdateBuildingData(dbmanager, num, buildingData);
					string cmdData = string.Format("{0}", 0);
					client.sendCmd(nID, cmdData);
				}
			}
		}

		public bool UpdateBuildData(List<BuildingData> BuildingDataList, BuildingData myBuildData)
		{
			bool flag = false;
			for (int i = 0; i < BuildingDataList.Count; i++)
			{
				if (BuildingDataList[i].BuildId == myBuildData.BuildId)
				{
					flag = true;
					BuildingDataList[i] = myBuildData;
					break;
				}
			}
			if (!flag)
			{
				BuildingDataList.Add(myBuildData);
			}
			return true;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13300:
				this.processCmdUpdateBuildData(client, nID, cmdParams, count);
				break;
			case 13301:
				this.processCmdUpdateBuildLog(client, nID, cmdParams, count);
				break;
			}
		}

		private static UpdateBuildingDataCmdProcessor instance = new UpdateBuildingDataCmdProcessor();
	}
}
