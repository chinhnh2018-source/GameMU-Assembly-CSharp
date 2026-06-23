using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	public class MerlinMagicBookManager
	{
		public static MerlinMagicBookManager getInstance()
		{
			return MerlinMagicBookManager.instance;
		}

		public TCPProcessCmdResults ProcessInsertMerlinDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MerlinGrowthSaveDBData merlinGrowthSaveDBData = null;
			int num = -1;
			bool flag = false;
			try
			{
				num = BitConverter.ToInt32(data, 0);
				merlinGrowthSaveDBData = DataHelper.BytesToObject<MerlinGrowthSaveDBData>(data, 4, count - 4);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd<bool>(nID, flag);
					return TCPProcessCmdResults.RESULT_OK;
				}
				DateTime now = DateTime.Now;
				string addTime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long addTime2 = now.Ticks / 10000L;
				flag = MerlinDBOperate.InsertMerlinData(dbMgr, merlinGrowthSaveDBData, addTime);
				if (!flag)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的梅林魔法书失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.MerlinData)
						{
							dbroleInfo.MerlinData = new MerlinGrowthSaveDBData();
						}
						dbroleInfo.MerlinData._RoleID = merlinGrowthSaveDBData._RoleID;
						dbroleInfo.MerlinData._Occupation = merlinGrowthSaveDBData._Occupation;
						dbroleInfo.MerlinData._Level = merlinGrowthSaveDBData._Level;
						dbroleInfo.MerlinData._StarNum = merlinGrowthSaveDBData._StarNum;
						dbroleInfo.MerlinData._StarExp = merlinGrowthSaveDBData._StarExp;
						dbroleInfo.MerlinData._LuckyPoint = merlinGrowthSaveDBData._LuckyPoint;
						dbroleInfo.MerlinData._ToTicks = merlinGrowthSaveDBData._ToTicks;
						dbroleInfo.MerlinData._AddTime = addTime2;
						dbroleInfo.MerlinData._ActiveAttr = merlinGrowthSaveDBData._ActiveAttr;
						dbroleInfo.MerlinData._UnActiveAttr = merlinGrowthSaveDBData._UnActiveAttr;
						MerlinRankManager.getInstance().createMerlinData(num);
					}
				}
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, flag);
			return TCPProcessCmdResults.RESULT_OK;
		}

		public TCPProcessCmdResults ProcessUpdateMerlinDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			bool flag = false;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 15)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd<bool>(nID, flag);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd<bool>(nID, flag);
					return TCPProcessCmdResults.RESULT_OK;
				}
				long toTicks = 0L;
				if (array[6] != "*")
				{
					toTicks = Convert.ToInt64(array[6]);
				}
				flag = MerlinDBOperate.UpdateMerlinData(dbMgr, num, array, 1);
				if (!flag)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新梅林魔法书失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null != dbroleInfo)
					{
						lock (dbroleInfo)
						{
							dbroleInfo.MerlinData._Level = DataHelper.ConvertToInt32(array[1], dbroleInfo.MerlinData._Level);
							dbroleInfo.MerlinData._LevelUpFailNum = DataHelper.ConvertToInt32(array[2], dbroleInfo.MerlinData._LevelUpFailNum);
							dbroleInfo.MerlinData._StarNum = DataHelper.ConvertToInt32(array[3], dbroleInfo.MerlinData._StarNum);
							dbroleInfo.MerlinData._StarExp = DataHelper.ConvertToInt32(array[4], dbroleInfo.MerlinData._StarExp);
							dbroleInfo.MerlinData._LuckyPoint = DataHelper.ConvertToInt32(array[5], dbroleInfo.MerlinData._LuckyPoint);
							if (array[6] != "*")
							{
								dbroleInfo.MerlinData._ToTicks = toTicks;
							}
							if (array[7] != "*")
							{
								dbroleInfo.MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(array[7], 10) / 100);
							}
							if (array[8] != "*")
							{
								dbroleInfo.MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(array[8], 10) / 100);
							}
							if (array[9] != "*")
							{
								dbroleInfo.MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(array[9], 10) / 100);
							}
							if (array[10] != "*")
							{
								dbroleInfo.MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(array[10], 10) / 100);
							}
							if (array[11] != "*")
							{
								dbroleInfo.MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(array[11], 10) / 100);
							}
							if (array[12] != "*")
							{
								dbroleInfo.MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(array[12], 10) / 100);
							}
							if (array[13] != "*")
							{
								dbroleInfo.MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(array[13], 10) / 100);
							}
							if (array[14] != "*")
							{
								dbroleInfo.MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(array[14], 10) / 100);
							}
						}
						MerlinRankingInfo merlinData = MerlinRankManager.getInstance().getMerlinData(num);
						if (null != merlinData)
						{
							if (merlinData.nLevel != dbroleInfo.MerlinData._Level || merlinData.nStarNum != dbroleInfo.MerlinData._StarNum)
							{
								merlinData.nLevel = dbroleInfo.MerlinData._Level;
								merlinData.nStarNum = dbroleInfo.MerlinData._StarNum;
								MerlinRankManager.getInstance().ModifyMerlinRankData(merlinData, false);
							}
						}
					}
				}
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, flag);
			return TCPProcessCmdResults.RESULT_OK;
		}

		public TCPProcessCmdResults ProcessQueryMerlinDataCmd(DBManager dbMgr, TCPOutPacketPool pool, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MerlinGrowthSaveDBData merlinGrowthSaveDBData = null;
			int nRoleID = -1;
			string value = null;
			try
			{
				value = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				nRoleID = Convert.ToInt32(value);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						merlinGrowthSaveDBData = dbroleInfo.MerlinData;
					}
				}
				else
				{
					merlinGrowthSaveDBData = MerlinDBOperate.QueryMerlinData(dbMgr, nRoleID);
				}
				if (null != merlinGrowthSaveDBData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(merlinGrowthSaveDBData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				merlinGrowthSaveDBData = new MerlinGrowthSaveDBData();
				for (int i = 0; i < 4; i++)
				{
					merlinGrowthSaveDBData._ActiveAttr[i] = 0.0;
					merlinGrowthSaveDBData._UnActiveAttr[i] = 0.0;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(merlinGrowthSaveDBData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static MerlinMagicBookManager instance = new MerlinMagicBookManager();
	}
}
