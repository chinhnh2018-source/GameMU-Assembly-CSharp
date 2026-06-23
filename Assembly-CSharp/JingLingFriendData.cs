using System;
using Server.Data;

public class JingLingFriendData : FriendData
{
	public void InitWith(FriendData data)
	{
		this.DbID = data.DbID;
		this.OtherRoleID = data.OtherRoleID;
		this.OtherRoleName = data.OtherRoleName;
		this.OtherLevel = data.OtherLevel;
		this.Occupation = data.Occupation;
		this.OnlineState = data.OnlineState;
		this.Position = data.Position;
		this.FriendType = data.FriendType;
		this.FriendChangeLifeLev = data.FriendChangeLifeLev;
		this.FriendCombatForce = data.FriendCombatForce;
		this.SpouseId = data.SpouseId;
		this.YaoSaiBossState = data.YaoSaiBossState;
		this.YaoSaiJianYuState = data.YaoSaiJianYuState;
	}
}
