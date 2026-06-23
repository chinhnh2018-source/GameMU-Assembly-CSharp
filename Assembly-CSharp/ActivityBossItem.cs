using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class ActivityBossItem : UserControl
{
	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public int ItemTag
	{
		get
		{
			return this.itemTag;
		}
		set
		{
			this.itemTag = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public double MaxEnterNum
	{
		get
		{
			return this.MaxNum;
		}
		set
		{
			this.MaxNum = value;
		}
	}

	public bool IsExisted
	{
		set
		{
			this._Icon.GoodImg.ToGrayBitmap = !value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void Init(MouseLeftButtonUpEventHandler handler = null)
	{
		GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(this, null, handler, false);
		if (null != ggoodIcon)
		{
			this._Icon = ggoodIcon;
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.OutSizeX = 92;
			ggoodIcon.OutSizeY = 92;
			ggoodIcon.BackSpriteName0 = "bagGrid2_bak";
			ggoodIcon.isAutoSize = false;
			ggoodIcon.BodyURL = new ImageURL(Global.GetBossIconString(ConfigMonsters.GetMonsterPicCodeByID(this.MonsterID)), false, 0);
		}
		Transform parent = base.transform.parent;
		ListBox listBox = NGUITools.FindInParents<ListBox>(base.gameObject);
		if (null != listBox)
		{
			parent = listBox.transform.parent;
		}
		UIDragObject uidragObject = this._Icon.gameObject.GetComponent<UIDragObject>();
		if (null == uidragObject)
		{
			uidragObject = this._Icon.gameObject.AddComponent<UIDragObject>();
		}
		uidragObject.scale = Vector3.right;
		uidragObject.target = base.transform.parent;
		uidragObject.restrictWithinPanel = true;
	}

	public void SetIconHighlighted(bool highlight = true)
	{
		this.highlightState.gameObject.SetActive(highlight);
	}

	public string MapName;

	public string ZhanLi;

	public string BossState;

	public string BossLastKiller;

	public int BirthType;

	public string TimeList;

	public string FallItemsList;

	public GGoodIcon _Icon;

	public UISprite _IsExisted;

	public UISprite highlightState;

	public int MonsterID;

	public int NPCID;

	public int SpriteID;

	public int MapCode;

	public int TargetType;

	public int TargetID;

	public int Level;

	public float Scale;

	public string BossName;

	public int Show;

	private int itemTag;

	private double MaxNum;
}
