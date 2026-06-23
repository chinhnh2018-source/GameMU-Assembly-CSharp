using System;

namespace HSGameEngine.GameEngine.Network
{
	public delegate bool ProcessServerCmdHandler(TCPClient client, int nID, byte[] data, int count);
}
