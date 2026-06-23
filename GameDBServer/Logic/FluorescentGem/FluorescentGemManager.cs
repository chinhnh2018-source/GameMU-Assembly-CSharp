using System;
using System.Text;
using System.Threading;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.FluorescentGem
{
	public class FluorescentGemManager
	{
		public static FluorescentGemManager getInstance()
		{
			return FluorescentGemManager.instance;
		}

		public TCPProcessCmdResults ProcessResetBagDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			client.sendCmd<bool>(nID, false);
			return TCPProcessCmdResults.RESULT_OK;
		}

		public TCPProcessCmdResults ProcessModifyFluorescentPointCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			bool cmdData = false;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, cmdData);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd<bool>(nID, cmdData);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd<bool>(nID, cmdData);
					return TCPProcessCmdResults.RESULT_OK;
				}
				lock (dbroleInfo)
				{
					dbroleInfo.FluorescentPoint += num2;
					cmdData = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, num, dbroleInfo.FluorescentPoint);
				}
				client.sendCmd<bool>(nID, cmdData);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, cmdData);
			return TCPProcessCmdResults.RESULT_OK;
		}

		public TCPProcessCmdResults ProcessUpdateFluorescentPointCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			bool cmdData = false;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, cmdData);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd<bool>(nID, cmdData);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd<bool>(nID, cmdData);
					return TCPProcessCmdResults.RESULT_OK;
				}
				cmdData = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, num, num2);
				lock (dbroleInfo)
				{
					dbroleInfo.FluorescentPoint = num2;
				}
				client.sendCmd<bool>(nID, cmdData);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, cmdData);
			return TCPProcessCmdResults.RESULT_OK;
		}

		public TCPProcessCmdResults ProcessEquipGemCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			bool flag = false;
			FluorescentGemSaveDBData fluorescentGemSaveDBData = null;
			try
			{
				fluorescentGemSaveDBData = DataHelper.BytesToObject<FluorescentGemSaveDBData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref fluorescentGemSaveDBData._RoleID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, fluorescentGemSaveDBData._RoleID), null, true);
					client.sendCmd<bool>(nID, flag);
					return TCPProcessCmdResults.RESULT_OK;
				}
				flag = FluorescentGemDBOperate.EquipFluorescentGem(dbMgr, fluorescentGemSaveDBData);
				if (flag)
				{
					lock (dbroleInfo)
					{
						GoodsData goodsData = new GoodsData();
						goodsData.GoodsID = fluorescentGemSaveDBData._GoodsID;
						goodsData.GCount = 1;
						goodsData.Binding = fluorescentGemSaveDBData._Bind;
						goodsData.Site = 7001;
						goodsData.BagIndex = this.GenerateBagIndex(fluorescentGemSaveDBData._Position, fluorescentGemSaveDBData._GemType);
						dbroleInfo.FluorescentGemData.GemEquipList.Add(goodsData);
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

		public TCPProcessCmdResults ProcessUnEquipGemCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			bool flag = false;
			FluorescentGemSaveDBData fluorescentGemSaveDBData = null;
			try
			{
				fluorescentGemSaveDBData = DataHelper.BytesToObject<FluorescentGemSaveDBData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref fluorescentGemSaveDBData._RoleID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, fluorescentGemSaveDBData._RoleID), null, true);
					client.sendCmd<bool>(nID, flag);
					return TCPProcessCmdResults.RESULT_OK;
				}
				flag = FluorescentGemDBOperate.UnEquipFluorescentGem(dbMgr, fluorescentGemSaveDBData);
				if (flag)
				{
					bool flag2 = false;
					try
					{
						DBRoleInfo obj;
						Monitor.Enter(obj = dbroleInfo, ref flag2);
						int slot = this.GenerateBagIndex(fluorescentGemSaveDBData._Position, fluorescentGemSaveDBData._GemType);
						dbroleInfo.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == slot);
					}
					finally
					{
						if (flag2)
						{
							DBRoleInfo obj;
							Monitor.Exit(obj);
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

		public void ParsePosAndType(int slot, out int pos, out int type)
		{
			pos = slot / 100;
			type = slot % 100;
		}

		public int GenerateBagIndex(int pos, int type)
		{
			return pos * 100 + type;
		}

		private static FluorescentGemManager instance = new FluorescentGemManager();
	}
}
