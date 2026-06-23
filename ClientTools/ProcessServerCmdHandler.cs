using System;

namespace ClientTools
{
	public delegate bool ProcessServerCmdHandler(TCPClient client, int nID, byte[] data, int count);
}
