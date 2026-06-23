using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

internal class BtnHander
{
	public BtnHander(GameObject obj)
	{
		obj.SetActive(true);
		this.ThisObj = obj;
		this.TiShiSp = obj.transform.FindChild("TiShi").GetComponent<UISprite>();
		this._BakSp = obj.transform.FindChild("Background").GetComponent<UISprite>();
		this._ContentSp = obj.transform.FindChild("ContentSp").GetComponent<UISprite>();
	}

	public string BakSpName
	{
		set
		{
			this._BakSp.spriteName = value;
		}
	}

	public ChatChannelIndexes ChatChannelIndexes
	{
		get
		{
			return this.mChatChannelIndexes;
		}
		set
		{
			this.mChatChannelIndexes = value;
			this.RefreshSp();
		}
	}

	public string Text
	{
		get
		{
			string[] array = new string[]
			{
				Global.GetLang("综合"),
				Global.GetLang("世界"),
				Global.GetLang("战盟"),
				Global.GetLang("队伍"),
				Global.GetLang("私聊"),
				Global.GetLang("系统"),
				Global.GetLang("阵营"),
				Global.GetLang("副本"),
				Global.GetLang("军团"),
				Global.GetLang("势力"),
				Global.GetLang("平台"),
				Global.GetLang("战队")
			};
			return array[(int)this.mChatChannelIndexes];
		}
		set
		{
			this.label.text = value;
		}
	}

	public Color TextColor
	{
		set
		{
			if (null == this.label)
			{
			}
		}
	}

	public bool BActive
	{
		get
		{
			return NGUITools.GetActive(this.ThisObj);
		}
		set
		{
			this.ThisObj.SetActive(value);
		}
	}

	public bool Bselect
	{
		set
		{
			this.mBselect = value;
			this.RefreshSp();
		}
	}

	private void RefreshSp()
	{
		if (this.mBselect)
		{
			this.BakSpName = "chatTab_hover";
		}
		else
		{
			this.BakSpName = "chatTab_normal";
		}
		this._ContentSp.spriteName = this.mChatChannelIndexes.ToString() + (this.mBselect ? "_S" : "_N");
		NGUITools.MakePixelPerfect(this._ContentSp.transform);
	}

	public bool ActiveTiShi
	{
		set
		{
			if (this.mChatChannelIndexes != ChatChannelIndexes.All)
			{
				this.TiShiSp.enabled = value;
			}
			else
			{
				this.TiShiSp.enabled = false;
			}
		}
	}

	private UISprite _BakSp;

	private UISprite _ContentSp;

	private UILabel label;

	private UISprite TiShiSp;

	private GameObject ThisObj;

	private ChatChannelIndexes mChatChannelIndexes;

	private bool mBselect;
}
