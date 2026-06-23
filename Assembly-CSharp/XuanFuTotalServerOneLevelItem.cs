using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class XuanFuTotalServerOneLevelItem : UserControl
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
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			this.SetAllBtnInactieve();
		};
		this.ItemTweenScale.eventReceiver = base.gameObject;
		this.ItemTweenScale.callWhenFinished = "ReceiveTweenMessage";
	}

	public void ReceiveTweenMessage()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3
			});
		}
	}

	public void RefreshUI()
	{
		if (this.FirstLevelListData != null)
		{
			this.TextName.text = string.Format(Global.GetLang("{0}"), this.FirstLevelListData.strFirstLevelServerName);
		}
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
		this.BShowChild = 1;
		if (this.BShowChild == 1 && (this.m_TabBtnOBC == null || this.m_TabBtnOBC.Count == 0))
		{
			int count = this.FirstLevelListData.listServerData.Count;
			for (int i = 0; i < count; i++)
			{
				XuanFuTotalServerItem xuanFuTotalServerItem = U3DUtils.NEW<XuanFuTotalServerItem>();
				SecondLevelServerListData secondLevelServerListData = this.FirstLevelListData.listServerData[i];
				xuanFuTotalServerItem.secondLevelListData = secondLevelServerListData;
				xuanFuTotalServerItem.m_Title.text = secondLevelServerListData.strSecondtLevelServerName;
				this.m_TabBtnOBC.AddNoUpdate(xuanFuTotalServerItem);
			}
			this.m_TabBtnOBC.DelayUpdate();
			this.m_ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnSelectChange);
		}
	}

	private void SetTabBtnIndex(int nIndex)
	{
		this.m_nTabBtnIndex = nIndex;
		GameObject at = this.m_TabBtnOBC.GetAt(nIndex);
		XuanFuTotalServerItem component = at.GetComponent<XuanFuTotalServerItem>();
		this.SetBtnActieve(component);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2,
				Data = component.secondLevelListData
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

	public void SetBtnActieve(XuanFuTotalServerItem btnobj)
	{
		if (null != btnobj)
		{
			if (btnobj == this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.BackgroundSprite.gameObject.SetActive(true);
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
					this.BackgroundSprite.spriteName = "tongyongBtn2_normal";
					this.TextName.color = NGUIMath.HexToColorEx(15790320U);
				}
				else
				{
					this.BackgroundSprite.spriteName = "tongyongBtn2_hover";
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

	public UIButtonTween ButtonTween;

	public TweenScale ItemTweenScale;

	public SpriteSL Content;

	public GameObject BtnItem;

	public UISprite BackgroundSprite;

	public UILabel TextName;

	private ObservableCollection m_TabBtnOBC;

	public ListBox m_ListTabBtn = new ListBox();

	public int m_nTabBtnIndex;

	public XuanFuTotalServerItem m_BtnLastSelect;

	public XuanFuTotalServerItem m_BtnCurrentSelect;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int ListIndex;

	public FistLevelServerListData FirstLevelListData;

	public List<XElement> TabFileList;

	private bool _ToggleState;

	private int _ItemIndex = -1;

	private int _bShowChild;
}
