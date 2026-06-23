using System;
using Server.Data;

namespace HSGameEngine.GameEngine.Network
{
	public class Session
	{
		public string UserID = string.Empty;

		public string UserName = string.Empty;

		public string UserToken = string.Empty;

		public int UserIsAdult;

		public int RoleRandToken = -1;

		public int RoleID = -1;

		public int LocalRoleID = -1;

		public int RoleSex;

		public string RoleName = string.Empty;

		public bool PlayGame;

		public RoleData roleData;

		public string RolePathString = string.Empty;

		public int GameServerID = 1;

		public string GamePingTaiID = "local";

		public MarriageData MarriageData;

		public MarriageData_EX OtherMarriageData;
	}
}
