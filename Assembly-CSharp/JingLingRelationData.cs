using System;
using Server.Data;

public class JingLingRelationData
{
	public string RoleName
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.OtherRoleName;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.RoleName;
			}
			return string.Empty;
		}
	}

	public int Lv
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.OtherLevel;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.Level;
			}
			return 1;
		}
	}

	public int ChangeLife
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.FriendChangeLifeLev;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.BangHuiMemberChangeLifeLev;
			}
			return 0;
		}
	}

	public int RoleID
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.OtherRoleID;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.RoleID;
			}
			return -1;
		}
	}

	public int YaoSaiBossState
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.YaoSaiBossState;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.YaoSaiBossState;
			}
			return 0;
		}
	}

	public int YaoSaiJianYuState
	{
		get
		{
			if (this.friendData != null)
			{
				return this.friendData.YaoSaiJianYuState;
			}
			if (this.banghuiData != null)
			{
				return this.banghuiData.YaoSaiJianYuState;
			}
			return 0;
		}
	}

	public bool IsOpenYaosai
	{
		get
		{
			return this.YaoSaiBossState > 0 && this.YaoSaiJianYuState > 0;
		}
	}

	internal void InitWith(FriendData data)
	{
		this.friendData = new JingLingFriendData();
		this.friendData.InitWith(data);
	}

	internal void InitWith(BangHuiMemberData data)
	{
		this.banghuiData = new JingLingBangHuiMemberData();
		this.banghuiData.InitWith(data);
	}

	public JingLingFriendData friendData;

	public JingLingBangHuiMemberData banghuiData;

	public YaoSaiBossData bossdata;
}
