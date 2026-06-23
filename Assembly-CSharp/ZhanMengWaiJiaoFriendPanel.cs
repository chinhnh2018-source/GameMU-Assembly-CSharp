using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengWaiJiaoFriendPanel : UserControl
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

	private void InitTextInPrefabs()
	{
		this.zhanmengwaijiao[0].text = Global.GetLang("战盟名称");
		this.zhanmengwaijiao[1].text = Global.GetLang("战盟等级");
		this.zhanmengwaijiao[2].text = Global.GetLang("战盟首领");
		this.zhanmengwaijiao[3].text = Global.GetLang("状态");
		this.zhanmengwaijiao[4].text = Global.GetLang("操作");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.itemList.ItemsSource;
	}

	public void InitData(List<AllyData> dataList)
	{
		if (dataList == null || dataList.Count == 0)
		{
			return;
		}
		int count = dataList.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanMengWaiJiaoFriendItem zhanMengWaiJiaoFriendItem = U3DUtils.NEW<ZhanMengWaiJiaoFriendItem>();
			AllyData value = dataList[i];
			zhanMengWaiJiaoFriendItem.SetValue(value);
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				this.SetButtonUnenable(zhanMengWaiJiaoFriendItem.btnCancelMengYue);
				this.SetButtonUnenable(zhanMengWaiJiaoFriendItem.btnCancelRequest);
			}
			UIPanel component = zhanMengWaiJiaoFriendItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, zhanMengWaiJiaoFriendItem.gameObject, true);
			zhanMengWaiJiaoFriendItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -1)
				{
					if (this.jieChuMengYouOb.activeSelf)
					{
						return;
					}
					this.PopCancelJieMengWindow(e.IDType, e.Title);
				}
				else if (e.ID == 0)
				{
					GameInstance.Game.SendCancelJieMengRequest(e.IDType);
				}
			};
			this.ItemCollection.AddNoUpdate(zhanMengWaiJiaoFriendItem);
		}
	}

	private void PopCancelJieMengWindow(int id, string zhanMengName)
	{
		this.jieChuMengYouOb.SetActive(true);
		this.jieChuMengYueWindow.SetContent(zhanMengName);
		this.jieChuMengYueWindow.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			GameInstance.Game.SendJieChuJieMengRequest(id);
		};
	}

	public void RefreshUI(int result)
	{
		if (result == 40)
		{
			Super.HintMainText(Global.GetLang("解除结盟失败"), 10, 3);
		}
		else if (result == 41)
		{
			this.jieChuMengYouOb.SetActive(false);
			Super.HintMainText(Global.GetLang("解除结盟成功"), 10, 3);
		}
	}

	private void SetButtonUnenable(GButton btn)
	{
		btn.isEnabled = false;
	}

	public void ClearData()
	{
		this.ItemCollection.Clear();
		int childCount = this.itemList.transform.childCount;
		if (childCount <= 0)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			Object.Destroy(this.itemList.transform.GetChild(i).gameObject);
		}
	}

	public void IsShow(bool isShow)
	{
		base.gameObject.SetActive(isShow);
	}

	public UILabel[] zhanmengwaijiao;

	public ListBox itemList;

	public GameObject jieChuMengYouOb;

	public ZhanMengJieChuMengYueWindow jieChuMengYueWindow;

	private ObservableCollection _ItemCollection;
}
