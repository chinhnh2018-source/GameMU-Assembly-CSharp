using System;
using System.Text;
using Server.Tools;

namespace HSGameEngine.GameEngine.Network
{
	public class MUSocketConnectEventArgs : SocketConnectEventArgs
	{
		public string[] fields
		{
			get
			{
				if (this.fields == null && this.bytesData != null)
				{
					string @string = new UTF8Encoding().GetString(this.bytesData, 0, this._bytecout);
					this.fields = @string.Split(new char[]
					{
						':'
					});
				}
				return this.fields;
			}
			set
			{
				this.fields = value;
			}
		}

		public static bool NotifyRecvDataStream(TCPClient client, int nID, byte[] data, int count, bool isString = false, int version = 1)
		{
			string[] fields = null;
			TCPCmdHandler.UnmarshalFun unmarshalFun;
			if (TCPCmdHandler.ProtoUnmarshalFuncs != null && TCPCmdHandler.ProtoUnmarshalFuncs.TryGetValue(nID, ref unmarshalFun))
			{
				client.NotifyRecvData(new MUSocketConnectEventArgs
				{
					RemoteEndPoint = client.GetRemoteEndPoint(),
					Error = "Success",
					NetSocketType = 4,
					CmdID = nID,
					objectData = unmarshalFun(data, 0, count)
				});
			}
			else
			{
				byte[] array = new byte[count];
				DataHelper.CopyBytes(array, 0, data, 0, count);
				client.NotifyRecvData(new MUSocketConnectEventArgs
				{
					RemoteEndPoint = client.GetRemoteEndPoint(),
					Error = "Success",
					NetSocketType = 4,
					CmdID = nID,
					fields = fields,
					bytesData = array,
					_bytecout = count
				});
			}
			return true;
		}

		private int _bytecout;

		public object objectData;
	}
}
