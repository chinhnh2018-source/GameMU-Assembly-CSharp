using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class RebirthBossItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void Init(MouseLeftButtonUpEventHandler handler = null)
	{
		GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(this, null, handler, false);
		if (null != ggoodIcon)
		{
			this.mIcon = ggoodIcon;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.OutSizeX = 66;
			ggoodIcon.OutSizeY = 66;
			ggoodIcon.BackSpriteName0 = "bagGrid2_bak";
			ggoodIcon.isAutoSize = false;
			ggoodIcon.BodyURL = new ImageURL(Global.GetBossIconString(ConfigMonsters.GetMonsterPicCodeByID(this.BossID)), false, 0);
		}
		Transform parent = base.transform.parent;
		ListBox listBox = NGUITools.FindInParents<ListBox>(base.gameObject);
		if (null != listBox)
		{
			parent = listBox.transform.parent;
		}
		UIDragObject uidragObject = this.mIcon.gameObject.GetComponent<UIDragObject>();
		if (null == uidragObject)
		{
			uidragObject = this.mIcon.gameObject.AddComponent<UIDragObject>();
		}
		uidragObject.scale = Vector3.right;
		uidragObject.target = base.transform.parent;
		uidragObject.restrictWithinPanel = true;
		this.Select = false;
	}

	public int MapCode { get; set; }

	public string MapName { get; set; }

	public float Scale { get; set; }

	public string ZhanLi { get; set; }

	public int BossID { get; set; }

	public bool Select
	{
		get
		{
			return this.mSelect;
		}
		set
		{
			this.mSelect = value;
			this._SelectSP.enabled = this.mSelect;
		}
	}

	public bool IsLock
	{
		get
		{
			return this.mIsLock;
		}
		set
		{
			this.mIsLock = value;
			this.mIcon.GoodImg.ToGrayBitmap = this.mIsLock;
		}
	}

	[SerializeField]
	private UISprite _SpBak;

	[SerializeField]
	private UISprite _SelectSP;

	[SerializeField]
	private Transform _GoodsRoot;

	private GGoodIcon mIcon;

	private bool mSelect;

	private bool mIsLock;
}
