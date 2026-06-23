using System;

public class ChatBoxFriendItem : UserControl
{
	public string FriendName
	{
		get
		{
			return this.m_strFriendName;
		}
		set
		{
			this.m_strFriendName = value;
		}
	}

	public UILabel LblFriendName;

	public UILabel LblFriendLevel;

	public UISprite _FriendFcae;

	protected string m_strFriendName = string.Empty;
}
