using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class BuyGoodsPartItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_TabBtnOBC = this.m_ListTabBtn.ItemsSource;
		UIEventListener.Get(this.BtnItem).onClick = delegate(GameObject s)
		{
			if (this.m_TabBtnOBC == null || this.m_TabBtnOBC.Count == 0)
			{
				this.InitBtnProc();
			}
			this.ItemIndexChild = 0;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			this.SetAllBtnInactieve();
		};
	}

	private void TabBtnSelectChange(object sender, object e)
	{
		if (this.m_ListTabBtn != null && this.m_ListTabBtn.SelectedIndex >= 0)
		{
			this.SetUIState(this.m_ListTabBtn.SelectedIndex);
		}
	}

	public void SetUIState(int nIndex)
	{
		this.SetTabBtnIndex(nIndex);
	}

	private void InitBtnProc()
	{
		if (this.BShowChild == 1 && (this.m_TabBtnOBC == null || this.m_TabBtnOBC.Count == 0))
		{
			int count = this.TabFileList.Count;
			for (int i = 0; i < count; i++)
			{
				XElement xelement = this.TabFileList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Type");
				if (xelementAttributeInt == this._ItemIndex)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
					BtnObjForBuyGood btnObjForBuyGood = U3DUtils.NEW<BtnObjForBuyGood>();
					btnObjForBuyGood.TextLabel.text = xelementAttributeStr;
					btnObjForBuyGood.IDValue = Global.GetXElementAttributeInt(xelement, "ID");
					this.m_TabBtnOBC.AddNoUpdate(btnObjForBuyGood);
				}
			}
			this.m_TabBtnOBC.DelayUpdate();
			this.m_ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnSelectChange);
		}
	}

	private void SetTabBtnIndex(int nIndex)
	{
		this.m_nTabBtnIndex = nIndex;
		GameObject at = this.m_TabBtnOBC.GetAt(nIndex);
		BtnObjForBuyGood component = at.GetComponent<BtnObjForBuyGood>();
		this.SetBtnActieve(component);
		this.ItemIndexChild = component.IDValue;
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		}
	}

	public void SetAllBtnInactieve()
	{
		if (this.m_BtnCurrentSelect != null)
		{
			this.m_BtnCurrentSelect.BackgroundSprite.gameObject.SetActive(false);
		}
		if (this.m_BtnLastSelect != null)
		{
			this.m_BtnLastSelect.BackgroundSprite.gameObject.SetActive(false);
		}
		this.m_BtnCurrentSelect = null;
		this.m_BtnLastSelect = null;
	}

	public void SetBtnActieve(BtnObjForBuyGood btnobj)
	{
		if (null != btnobj)
		{
			if (btnobj == this.m_BtnCurrentSelect)
			{
				return;
			}
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnLastSelect = this.m_BtnCurrentSelect;
				this.m_BtnCurrentSelect = btnobj;
			}
			this.m_BtnCurrentSelect = btnobj;
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.BackgroundSprite.gameObject.SetActive(true);
			}
			if (null != this.m_BtnLastSelect)
			{
				this.m_BtnLastSelect.BackgroundSprite.gameObject.SetActive(false);
			}
		}
	}

	public bool ToggleState
	{
		get
		{
			return this._ToggleState;
		}
		set
		{
			if (this._ToggleState != value)
			{
				this._ToggleState = value;
				this.ButtonTween.Play(this._ToggleState);
				if (this._ToggleState)
				{
					this.BackgroundSprite.spriteName = "btn_subtract";
					this.TextName.color = NGUIMath.HexToColorEx(15790320U);
				}
				else
				{
					this.BackgroundSprite.spriteName = "btn_add";
					this.TextName.color = NGUIMath.HexToColorEx(10323559U);
				}
			}
		}
	}

	public int ItemIndex
	{
		get
		{
			return this._ItemIndex;
		}
		set
		{
			this._ItemIndex = value;
		}
	}

	public int BShowChild
	{
		get
		{
			return this._bShowChild;
		}
		set
		{
			this._bShowChild = value;
		}
	}

	public int ItemIndexChild
	{
		get
		{
			return this._ItemIndexChild;
		}
		set
		{
			if (value != this._ItemIndexChild)
			{
				this._ItemIndexChild = value;
			}
		}
	}

	public UIButtonTween ButtonTween;

	public TweenScale ItemTweenScale;

	public SpriteSL Content;

	public GameObject BtnItem;

	public UISprite BackgroundSprite;

	public UILabel TextName;

	private ObservableCollection m_TabBtnOBC;

	public ListBox m_ListTabBtn = new ListBox();

	public int m_nTabBtnIndex;

	public BtnObjForBuyGood m_BtnLastSelect;

	public BtnObjForBuyGood m_BtnCurrentSelect;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int ListIndex;

	public List<XElement> TabFileList;

	private bool _ToggleState;

	private int _ItemIndex = -1;

	private int _bShowChild;

	private int _ItemIndexChild = -1;
}
