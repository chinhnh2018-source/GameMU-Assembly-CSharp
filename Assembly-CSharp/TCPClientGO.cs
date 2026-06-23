using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;

public static class TCPClientGO
{
	public static bool SendData(this TCPGameServerCmds _TCPGameServerCmds, string strcmd)
	{
		return GameInstance.Game.GameClient.SendData(TCPOutPacket.MakeTCPOutPacket(GameInstance.Game.GameClient.OutPacketPool, strcmd, (int)_TCPGameServerCmds));
	}

	public static bool SendDataUseRoleID(this TCPGameServerCmds _TCPGameServerCmds)
	{
		return GameInstance.Game.GameClient.SendData(TCPOutPacket.MakeTCPOutPacket(GameInstance.Game.GameClient.OutPacketPool, Global.Data.roleData.RoleID.ToString(), (int)_TCPGameServerCmds));
	}
}
