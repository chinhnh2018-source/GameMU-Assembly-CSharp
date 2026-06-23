using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TianFuItem : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(this._Image.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this._ID
			});
		};
	}

	public void SetIcon(string name)
	{
		this._ShowNetImage.ImageURL = name;
	}

	private new void Start()
	{
	}

	public string setHuang
	{
		set
		{
			this._Huang.spriteName = value;
		}
	}

	public bool ShowNetImage
	{
		set
		{
			this._ShowNetImage.ToGrayBitmap = value;
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

	public new string Name
	{
		get
		{
			return this._Name;
		}
		set
		{
			this._Name = value;
		}
	}

	public string Image
	{
		set
		{
			this._Image.spriteName = value;
		}
	}

	public void setLabel(int currLv, int maxLv)
	{
		this._CurrLv = currLv;
		this._MaxLv = maxLv;
		if (!this._Active)
		{
			this.label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format("{0}/{1}", currLv, maxLv)
			});
		}
		else if (currLv == maxLv)
		{
			this.label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				string.Format("{0}/{1}", currLv, maxLv)
			});
		}
		else
		{
			this.label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format("{0}/{1}", currLv, maxLv)
			});
		}
	}

	public int getTipsType()
	{
		if (!this._Active)
		{
			this._TipsType = 4;
			return this._TipsType;
		}
		if (this._CurrLv == 0)
		{
			this._TipsType = 2;
			return this._TipsType;
		}
		if (this._MaxLv > this._CurrLv)
		{
			this._TipsType = 1;
			return this._TipsType;
		}
		if (this._CurrLv == this._MaxLv)
		{
			this._TipsType = 3;
			return this._TipsType;
		}
		return this._TipsType;
	}

	private int _ID;

	public int _CurrLv;

	public int _MaxLv;

	public UILabel label;

	private string _Name;

	public UISprite _Huang;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite _Image;

	public int _ItemType = 1;

	public string iconStr;

	public bool _Active;

	private int _TipsType;

	public string _CurrSkillStr;

	public string _LastSkillStr;

	public string _SkillTiaoJian_1;

	public string _SkillTiaoJian_2;

	public UITexture _UITexture;

	public ShowNetImage _ShowNetImage;

	public GameObject _Visible;

	public int _NeedTianFu;

	public int _NeedTianFuLevel;
}
