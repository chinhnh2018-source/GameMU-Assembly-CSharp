using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class FamilyBuildItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnGet.Text = Global.GetLang("领取");
		this.SpriteUI.transform.localPosition = new Vector3(100f, -20f, 0f);
		this.lblNeedZhangong.transform.localPosition = new Vector3(-75f, -20f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnGet.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._BuildingType == 1)
			{
				Global.SendEvent("710", Global.GetLang("战盟军旗BUFF领取次数"));
			}
			if (this._BuildingType == 2)
			{
				Global.SendEvent("712", Global.GetLang("战盟祭坛BUFF领取次数"));
			}
			if (this._BuildingType == 3)
			{
				Global.SendEvent("714", Global.GetLang("战盟军械BUFF领取次数"));
			}
			if (this._BuildingType == 4)
			{
				Global.SendEvent("716", Global.GetLang("战盟光环BUFF领取次数"));
			}
			GameInstance.Game.SpriteZhanMengBuildGetBufferCmd(Global.Data.roleData.Faction, this.BuildingType, this.CurrentLevel);
		};
	}

	public int BuffID
	{
		get
		{
			return this._BuffID;
		}
		set
		{
			this._BuffID = value;
		}
	}

	public int BuildingType
	{
		get
		{
			return this._BuildingType;
		}
		set
		{
			switch (value)
			{
			}
			this._BuildingType = value;
		}
	}

	public int CurrentLevel
	{
		get
		{
			return this._currentLevel;
		}
		set
		{
			this._currentLevel = value;
			this.lblCurrentLevel.text = string.Empty;
			switch (this._BuildingType)
			{
			case 1:
				this.lblCurrentLevel.text = Global.GetLang("战盟旗帜   LV") + value;
				Global.zhanmengLevel = value;
				break;
			case 2:
				this.lblCurrentLevel.text = Global.GetLang("战盟祭坛   LV") + value;
				break;
			case 3:
				this.lblCurrentLevel.text = Global.GetLang("战盟军械   LV") + value;
				break;
			case 4:
				this.lblCurrentLevel.text = Global.GetLang("战盟光环   LV") + value;
				break;
			}
		}
	}

	public int NeedZhangong
	{
		get
		{
			return this._needZhangong;
		}
		set
		{
			this._needZhangong = value;
			this.lblNeedZhangong.text = Global.GetLang("消耗战功 : {FFFFFF}") + value + "{-}";
		}
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (value)
			{
				this.itemBak.spriteName = "stillItem2_bak";
			}
			else
			{
				this.itemBak.spriteName = "stillItem_bak";
			}
		}
	}

	public void SetAsSelected(bool isSelected)
	{
		if (isSelected)
		{
			this.itemBak.spriteName = "stillItem2_bak";
		}
		else
		{
			this.itemBak.spriteName = "stillItem_bak";
		}
	}

	public void SetIcon(string url)
	{
		this.icon.ImageURL = url;
		this.icon.ForceShow();
	}

	public UISprite itemBak;

	public UILabel lblCurrentLevel;

	public UILabel lblNeedZhangong;

	public GButton btnGet;

	public UISprite SpriteUI;

	private int _BuffID = -1;

	private int _BuildingType = -1;

	private int _currentLevel = -1;

	private int _needZhangong = -1;

	private bool _SelectedState;

	public ShowNetImage icon;
}
