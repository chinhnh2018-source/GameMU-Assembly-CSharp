using System;
using UnityEngine;

public class LianluZhuangbeiXilianPropsItem : UserControl
{
	public int FlagState
	{
		set
		{
			Vector3 localPosition = this.TxtChangeValue.transform.localPosition;
			if (value == 0)
			{
				this.ChangeFlag.gameObject.SetActive(false);
				this.TxtChangeValue.Text = string.Empty;
			}
			else
			{
				this.ChangeFlag.gameObject.SetActive(true);
				if (value > 0)
				{
					this.ChangeFlag.spriteName = this.FlagNames[0];
				}
				else if (value < 0)
				{
					this.ChangeFlag.spriteName = this.FlagNames[1];
				}
			}
		}
	}

	public bool IsCompare
	{
		get
		{
			return this._IsCompare;
		}
		set
		{
			this._IsCompare = value;
			this.ChangeFlag.gameObject.SetActive(this._IsCompare);
			this.TxtChangeValue.gameObject.SetActive(this._IsCompare);
		}
	}

	public bool RadioVisible
	{
		get
		{
			return this.Bak.gameObject.activeSelf;
		}
		set
		{
		}
	}

	public int PropsID
	{
		get
		{
			return this._PropsID;
		}
		set
		{
			this._PropsID = value;
		}
	}

	public int ID
	{
		get
		{
			return this._ID;
		}
		set
		{
			this._ID = value;
		}
	}

	public TextBlock TxtValue;

	public TextBlock TxtChangeValue;

	public TextBlock TxtRangeValue;

	public UISprite ChangeFlag;

	public UISprite Bak;

	private string[] FlagNames = new string[]
	{
		"up",
		"down"
	};

	private bool _IsCompare;

	private int _PropsID = -1;

	public int _ID = -1;
}
