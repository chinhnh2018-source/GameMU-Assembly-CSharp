using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class RiChangFuBenItem : UserControl
{
	public string ItemName
	{
		get
		{
			return this._ItemName.text;
		}
		set
		{
			this._ItemName.text = value;
		}
	}

	public bool ShowDetail
	{
		set
		{
			this._ItemName.gameObject.SetActive(value);
			this.Star_Bak.gameObject.SetActive(value);
			this.Title_Bak.gameObject.SetActive(value);
			for (int i = 0; i < this._RewardLevel.Length; i++)
			{
				GImgProgressBar gimgProgressBar = this._RewardLevel[i];
				gimgProgressBar.gameObject.SetActive(value);
			}
			for (int j = 0; j < this._RewardType.Length; j++)
			{
				TextBlock textBlock = this._RewardType[j];
				textBlock.gameObject.SetActive(value);
			}
		}
	}

	public int ShowType
	{
		set
		{
			if (value == 0)
			{
			}
		}
	}

	public Bounds bounds
	{
		set
		{
			this._Bounds.transform.localScale = value.size;
			this._Bounds.transform.localPosition = value.center;
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			if (null != component)
			{
				component.size = value.size;
				component.center = value.center;
			}
		}
	}

	public int Mode
	{
		set
		{
			if (this.m_Mode != value)
			{
				this.m_Mode = value;
				this.ChangeModeSprite(this.m_Mode, this._SelectedState);
			}
		}
	}

	public int MyTopTime { get; set; }

	public string TopName { get; set; }

	public int TopTime { get; set; }

	public bool GrayTexture
	{
		set
		{
			this._GrayIfInactive = true;
		}
	}

	public bool IsEnabeld
	{
		get
		{
			return this._IsEnabeld;
		}
		set
		{
			this._IsEnabeld = value;
			this._Preview.ToGrayBitmap = (this._IsGray || !this._IsEnabeld);
		}
	}

	public bool IsGray
	{
		get
		{
			return this._IsGray;
		}
		set
		{
			this._IsGray = value;
			this._Preview.ToGrayBitmap = (this._IsGray || !this._IsEnabeld);
		}
	}

	public bool SelectedState
	{
		set
		{
			if (this._SelectedState != value)
			{
				this._SelectedState = value;
				this.SelectedRect.gameObject.SetActive(value);
			}
			if (this._GrayIfInactive && !this.bIsOpen)
			{
				this._Preview.ToGrayBitmap = !value;
			}
		}
	}

	private void ChangeModeSprite(int mode, bool selected)
	{
		if (mode >= 0)
		{
			this._Mode.spriteName = string.Format("{0}{1}", mode, (!selected) ? 0 : 1);
		}
		else
		{
			this._Mode.spriteName = null;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._RewardLevel[0].transform.localPosition = new Vector3(-22f, -130f, -0.1f);
		this._RewardLevel[1].transform.localPosition = new Vector3(-22f, -166f, -0.1f);
		this._ItemName.Z = -0.1;
		this._RewardType[0].transform.localScale = new Vector3(20f, 20f, 1f);
		this._RewardType[1].transform.localScale = new Vector3(20f, 20f, 1f);
	}

	public void Init()
	{
		Transform parent = base.transform.parent;
		ListBox listBox = NGUITools.FindInParents<ListBox>(base.gameObject);
		if (null != listBox)
		{
			parent = listBox.transform.parent;
		}
		UIDragObject uidragObject = base.gameObject.GetComponent<UIDragObject>();
		if (null == uidragObject)
		{
			uidragObject = base.gameObject.AddComponent<UIDragObject>();
		}
		uidragObject.scale = Vector3.right;
		uidragObject.target = base.transform.parent;
		uidragObject.restrictWithinPanel = true;
		if (this.CopyID <= 0)
		{
			ActivityTipManager.RegActivityTipItem(ActivityTipManager.GetActivityTipTypeByFuBenTabID(this.TabID), delegate(int s, ActivityTipItem e)
			{
				this.IsGray = !e.IsActive;
			});
		}
	}

	private new void OnDestroy()
	{
		if (this.CopyID <= 0)
		{
			ActivityTipManager.RegActivityTipItem(ActivityTipManager.GetActivityTipTypeByFuBenTabID(this.TabID), null);
		}
	}

	public TextBlock _ItemName;

	public ShowNetImage _Preview;

	public TextBlock[] _RewardType;

	public GImgProgressBar[] _RewardLevel;

	public UISprite SelectedRect;

	public UISprite _Mode;

	public UITexture _Bounds;

	public GameObject WeiKaiQi;

	public TextBlock RenWuTitle;

	public TextBlock NeedText;

	public UISprite Star_Bak;

	public UISprite Title_Bak;

	private int m_Mode = -1;

	public string Level;

	public string CopyType = string.Empty;

	public int FuBenType;

	public int CopyID;

	public int TabID;

	public int MapCode;

	public int DisplayID;

	public int MinLevel;

	public int MaxLevel;

	public int MinZhuanSheng;

	public int MaxZhuanSheng;

	public int MaxEnterNum;

	public int EnterGoods;

	public int GoodsNumber;

	public int MinSaoDangTime;

	public int NeedYuanBao;

	public int ZhanLi;

	public int EnterNum;

	public int FinishNum;

	public int MaxFinishNum;

	public bool bIsOpen;

	public bool LevelAllow;

	public bool SaoDangAllow;

	public bool NumberAllow;

	public bool ZhanLiAllow;

	public string RewardExplains = string.Empty;

	public string RewardGoods = string.Empty;

	private bool _GrayIfInactive;

	private bool _IsEnabeld;

	private bool _IsGray;

	private bool _SelectedState = true;
}
