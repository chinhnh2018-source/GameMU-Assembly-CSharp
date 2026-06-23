using System;
using System.Collections.Generic;
using System.Text;

namespace HSGameEngine.GameEngine.Network
{
	public class TCPCmdHandler
	{
		public static bool ProcessServerCmd(TCPClient client, int nID, byte[] data, int count)
		{
			if (nID == 23 || nID == 112)
			{
				TCPPing.RecordRecCmd(nID);
			}
			bool flag;
			if (nID != 1 && nID != 20)
			{
				flag = MUSocketConnectEventArgs.NotifyRecvDataStream(client, nID, data, count, false, 1);
			}
			else
			{
				flag = TCPCmdHandler.ProcessUserInfoCmd(client, nID, data, count);
			}
			if (!flag)
			{
				flag = true;
				MUDebug.LogError<string>(new string[]
				{
					"客户端未处理的指令ID=" + (TCPGameServerCmds)nID
				});
			}
			return flag;
		}

		private static bool ProcessUserInfoCmd(TCPClient client, int nID, byte[] data, int count)
		{
			string @string = new UTF8Encoding().GetString(data, 0, count);
			string[] array = @string.Split(new char[]
			{
				':'
			});
			if (array.Length < 2)
			{
				return false;
			}
			client.NotifyRecvData(new MUSocketConnectEventArgs
			{
				RemoteEndPoint = client.GetRemoteEndPoint(),
				Error = "Success",
				NetSocketType = 4,
				CmdID = nID,
				fields = array
			});
			return true;
		}

		public static Dictionary<int, TCPCmdHandler.UnmarshalFun> ProtoUnmarshalFuncs;

		public delegate object UnmarshalFun(byte[] bytes, int offset, int count);
	}
}
