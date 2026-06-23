using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengLianSaiGuanZhanRoleListPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mLblName.Text = Global.GetLang("名字");
		this.mLblZhiYe.Text = Global.GetLang("职业");
		this.mLblLevel.Text = Global.GetLang("等级");
		this.mLblZhiWu.Text = Global.GetLang("职务");
		this.mLblkill.Text = Global.GetLang("击杀");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.selectId > 0)
			{
				GameInstance.Game.SendGuanZhanTrackOtherPlayer(this.selectId);
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("请选择需要追踪的玩家！"), 10, 3);
			}
		};
		UIEventListener.Get(this.mLeftClick.gameObject).onClick = delegate(GameObject s)
		{
			this.RefreshRolelistInfo(1);
		};
		UIEventListener.Get(this.mRightClick.gameObject).onClick = delegate(GameObject s)
		{
			this.RefreshRolelistInfo(2);
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (null != selectedItem)
			{
				ZhanMengLianSaiGuanZhanRolelistItem component = selectedItem.GetComponent<ZhanMengLianSaiGuanZhanRolelistItem>();
				this.selectId = component.ID;
				component.Selected = true;
				if (this.mListBox.LastSelectedItem != null)
				{
					ZhanMengLianSaiGuanZhanRolelistItem component2 = this.mListBox.LastSelectedItem.GetComponent<ZhanMengLianSaiGuanZhanRolelistItem>();
					if (this.selectId != component2.ID)
					{
						component2.Selected = false;
					}
				}
			}
		};
	}

	public void InitData(GuanZhanData data, GameObject RolelistItemObj)
	{
		this.mGuanZhanData = data;
		this.mRolelistItemObj = RolelistItemObj;
		List<string> sideName = data.SideName;
		if (sideName != null && sideName.Count > 0)
		{
			this.mLblLeftZhenYingName.Text = sideName[0];
			this.mLblRightZhenYingName.Text = sideName[1];
		}
		this.RefreshRolelistInfo(1);
	}

	private void RefreshRolelistInfo(int key)
	{
		if (this.mGuanZhanData == null)
		{
			return;
		}
		Dictionary<int, List<GuanZhanRoleMiniData>> roleMiniDataDict = this.mGuanZhanData.RoleMiniDataDict;
		List<GuanZhanRoleMiniData> list = null;
		if (roleMiniDataDict.ContainsKey(key))
		{
			list = roleMiniDataDict[key];
		}
		if (list != null && list.Count > 0)
		{
			this.ItemCollection.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = NGUITools.AddChild(this.mListBox.gameObject, this.mRolelistItemObj);
				gameObject.SetActive(true);
				ZhanMengLianSaiGuanZhanRolelistItem component = gameObject.GetComponent<ZhanMengLianSaiGuanZhanRolelistItem>();
				component.InitData(list[i]);
				this.ItemCollection.AddNoUpdate(gameObject);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
		}
	}

	protected override void OnDestroy()
	{
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblName;

	public TextBlock mLblZhiYe;

	public TextBlock mLblLevel;

	public TextBlock mLblZhiWu;

	public TextBlock mLblkill;

	public GameObject mLeftClick;

	public TextBlock mLblLeftZhenYingName;

	public GameObject mRightClick;

	public TextBlock mLblRightZhenYingName;

	public GButton mBtnConfirm;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private GameObject mRolelistItemObj;

	private GuanZhanData mGuanZhanData;

	public int selectId;
}
