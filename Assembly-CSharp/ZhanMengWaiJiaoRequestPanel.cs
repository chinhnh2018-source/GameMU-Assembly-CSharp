using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengWaiJiaoRequestPanel : UserControl
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
		this.ItemCollection = this.itemList.ItemsSource;
		this.zhanmengwaijiaoRe[0].text = Global.GetLang("战盟名称");
		this.zhanmengwaijiaoRe[1].text = Global.GetLang("战盟等级");
		this.zhanmengwaijiaoRe[2].text = Global.GetLang("战盟首领");
		this.zhanmengwaijiaoRe[3].text = Global.GetLang("操作");
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
			ZhanMengWaiJiaoRequestItem zhanMengWaiJiaoRequestItem = U3DUtils.NEW<ZhanMengWaiJiaoRequestItem>();
			AllyData value = dataList[i];
			zhanMengWaiJiaoRequestItem.SetValue(value);
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				this.SetButtonUnenable(zhanMengWaiJiaoRequestItem.btnAgree);
				this.SetButtonUnenable(zhanMengWaiJiaoRequestItem.btnCancel);
			}
			UIPanel component = zhanMengWaiJiaoRequestItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, zhanMengWaiJiaoRequestItem.gameObject, true);
			zhanMengWaiJiaoRequestItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -1)
				{
					GameInstance.Game.SendAgreeOrRejectRequest(e.IDType, e.Flag);
				}
				else if (e.ID == 0)
				{
					GameInstance.Game.SendAgreeOrRejectRequest(e.IDType, e.Flag);
				}
			};
			this.ItemCollection.AddNoUpdate(zhanMengWaiJiaoRequestItem);
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

	public UILabel[] zhanmengwaijiaoRe;

	public ListBox itemList;

	private ObservableCollection _ItemCollection;
}
