using System;

public class PrivateRoleInfo
{
	public string RoleName
	{
		get
		{
			return this.m_roleName;
		}
		set
		{
			this.m_roleName = value;
		}
	}

	public int RoleZone
	{
		get
		{
			return this.m_roleZone;
		}
		set
		{
			this.m_roleZone = value;
		}
	}

	public int UnReadMessageNum
	{
		get
		{
			return this.m_unReadMessageNum;
		}
		set
		{
			this.m_unReadMessageNum = value;
		}
	}

	public long LastChatTicks
	{
		get
		{
			return this.m_lastChatTicks;
		}
		set
		{
			this.m_lastChatTicks = value;
		}
	}

	private string m_roleName;

	private int m_roleZone;

	private int m_unReadMessageNum;

	private long m_lastChatTicks;
}
