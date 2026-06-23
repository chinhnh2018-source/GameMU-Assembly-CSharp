using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class PKModePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		this._Bak.URL = Global.GetGameResImageString("PKMode_bak.png");
		if (Global.Data != null)
		{
			this.PKMode = Global.Data.roleData.PKMode;
		}
		switch (this.PKMode)
		{
		case 0:
			this._Normal.Check = true;
			break;
		case 1:
			this._Whole.Check = true;
			break;
		case 2:
			this._Faction.Check = true;
			break;
		case 3:
			this._Team.Check = true;
			break;
		case 4:
			this._Kind.Check = true;
			break;
		case 7:
			this._ArmyGroup.Check = true;
			break;
		}
		this._Normal.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this._Normal.Check)
			{
				this.PKModeChanged(0);
			}
			this.Close();
		};
		this._Team.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this._Team.Check)
			{
				this.PKModeChanged(3);
			}
			this.Close();
		};
		this._Faction.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this._Faction.Check)
			{
				this.PKModeChanged(2);
			}
			this.Close();
		};
		this._Whole.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this._Whole.Check)
			{
				this.PKModeChanged(1);
			}
			this.Close();
		};
		this._Kind.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this._Kind.Check)
			{
				this.PKModeChanged(4);
			}
			this.Close();
		};
		this._ArmyGroup.CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (this._ArmyGroup.Check)
			{
				this.PKModeChanged(7);
			}
			this.Close();
		};
	}

	protected void Close()
	{
		if (this._Close.MouseLeftButtonUp != null)
		{
			this._Close.MouseLeftButtonUp(null, null);
		}
	}

	public void PKModeChanged(int mode)
	{
		if (mode != 0)
		{
			this._Normal.Check = false;
		}
		if (mode != 3)
		{
			this._Team.Check = false;
		}
		if (mode != 2)
		{
			this._Faction.Check = false;
		}
		if (mode != 1)
		{
			this._Whole.Check = false;
		}
		if (mode != 4)
		{
			this._Kind.Check = false;
		}
		if (mode != 7)
		{
			this._ArmyGroup.Check = false;
		}
		this.PKMode = mode;
		if (this.PKMode != Global.Data.roleData.PKMode)
		{
			if (Global.IsBattleMap())
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("阵营战只能是全体模式!"), new object[0]), 0, -1, -1, 0);
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前场景只能是战盟模式!"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.ShiLian)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前场景只能是组队模式!"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteUpdatePKMode(this.PKMode);
		}
	}

	public static PKModePart GGetInstance()
	{
		if (null == PKModePart._Instance)
		{
			PKModePart._Instance = U3DUtils.NEW<PKModePart>();
		}
		return PKModePart._Instance;
	}

	public static PKModePart GShow()
	{
		if (null == PKModePart._Instance)
		{
			PKModePart._Instance = U3DUtils.NEW<PKModePart>();
		}
		PKModePart._Instance.gameObject.SetActive(true);
		return PKModePart._Instance;
	}

	public static void GClose()
	{
		if (null != PKModePart._Instance)
		{
			Object.Destroy(PKModePart._Instance.gameObject);
			PKModePart._Instance = null;
		}
	}

	public static void GHide()
	{
		if (null != PKModePart._Instance)
		{
			PKModePart._Instance.gameObject.SetActive(false);
			PKModePart._Instance = null;
		}
	}

	public static PKModePart _Instance;

	public ShowNetImage _Bak;

	public GButton _Close;

	public GCheckBox _Normal;

	public GCheckBox _Team;

	public GCheckBox _Faction;

	public GCheckBox _Whole;

	public GCheckBox _Kind;

	public GCheckBox _ArmyGroup;

	private int PKMode;
}
