using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	public class TCPCmdDispatcher
	{
		private TCPCmdDispatcher()
		{
		}

		public static TCPCmdDispatcher getInstance()
		{
			return TCPCmdDispatcher.instance;
		}

		public void initialize()
		{
		}

		public void registerProcessor(int cmdId, short paramNum, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, paramNum, paramNum, processor, TCPCmdFlags.IsStringArrayParams);
		}

		public void registerProcessorEx(int cmdId, short minParamCount, short maxParamCount, ICmdProcessor processor, TCPCmdFlags cmdFlags = TCPCmdFlags.IsStringArrayParams)
		{
			Debug.Assert(processor != null);
			CmdHandler value = new CmdHandler
			{
				CmdFlags = (uint)cmdFlags,
				MinParamCount = minParamCount,
				MaxParamCount = maxParamCount,
				CmdProcessor = processor
			};
			this.cmdProcesserMapping.Add(cmdId, value);
		}

		public void registerStreamProcessorEx(int cmdId, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, -1, -1, processor, TCPCmdFlags.IsBinaryStreamParams);
		}

		private CmdHandler GetCmdHandler(int cmdID)
		{
			CmdHandler cmdHandler;
			CmdHandler result;
			if (this.cmdProcesserMapping.TryGetValue(cmdID, out cmdHandler))
			{
				result = cmdHandler;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public TCPProcessCmdResults transmission(TMSKSocket socket, int nID, byte[] data, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				byte[] array = Global.SendAndRecvData<byte[]>(nID, data, socket.ServerId, 0);
				if (null == array)
				{
					LogManager.WriteLog(2, string.Format("与DBServer通讯失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = BitConverter.ToInt32(array, 0);
				ushort packetCmdID = BitConverter.ToUInt16(array, 4);
				TCPOutPacket tcpoutPacket = TCPOutPacketPool.getInstance().Pop();
				tcpoutPacket.PacketCmdID = packetCmdID;
				tcpoutPacket.FinalWriteData(array, 6, num - 2);
				TCPManager.getInstance().MySocketListener.SendData(socket, tcpoutPacket, true);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults dispathProcessor(TMSKSocket socket, int nID, byte[] data, int count)
		{
			try
			{
				CmdHandler cmdHandler;
				if ((cmdHandler = this.GetCmdHandler(nID)) == null)
				{
					return TCPProcessCmdResults.RESULT_UNREGISTERED;
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (null == gameClient)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ICmdProcessorEx cmdProcessorEx = cmdHandler.CmdProcessor as ICmdProcessorEx;
				if ((cmdHandler.CmdFlags & 2U) > 0U)
				{
					string @string = new UTF8Encoding().GetString(data, 0, count);
					string[] array = @string.Split(new char[]
					{
						':'
					});
					if (array.Length < (int)cmdHandler.MinParamCount || array.Length > (int)cmdHandler.MaxParamCount)
					{
						LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					if (null == cmdProcessorEx)
					{
						if (!cmdHandler.CmdProcessor.processCmd(gameClient, array))
						{
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					else if (!cmdProcessorEx.processCmdEx(gameClient, nID, data, array))
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					return TCPProcessCmdResults.RESULT_OK;
				}
				else
				{
					if (null == cmdProcessorEx)
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					if (!cmdProcessorEx.processCmdEx(gameClient, nID, data, null))
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					return TCPProcessCmdResults.RESULT_OK;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		private Dictionary<int, CmdHandler> cmdProcesserMapping = new Dictionary<int, CmdHandler>();
	}
}
