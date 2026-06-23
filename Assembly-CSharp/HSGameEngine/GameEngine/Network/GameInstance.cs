using System;
using Server.Data;

namespace HSGameEngine.GameEngine.Network
{
	public static class GameInstance
	{
		public static TCPGame Game
		{
			get
			{
				return GameInstance._Game;
			}
		}

		public static TCPGame CreateNewTCPGame()
		{
			if (GameInstance._Game.CurrentSession.roleData != null)
			{
				GameInstance._SessionBakup = GameInstance._Game.CurrentSession;
			}
			GameInstance._Game = new TCPGame();
			if (GameInstance._SessionBakup == null)
			{
				GameInstance._SessionBakup = GameInstance._Game.CurrentSession;
			}
			return GameInstance._Game;
		}

		public static RoleData ValidRoleData
		{
			get
			{
				if (GameInstance._Game.CurrentSession.roleData == null)
				{
					return GameInstance._SessionBakup.roleData;
				}
				return GameInstance._Game.CurrentSession.roleData;
			}
		}

		private static TCPGame _Game = new TCPGame();

		private static Session _SessionBakup = new Session();
	}
}
