using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengWaiJiaoLogPanel : UserControl
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
	}

	public void InitData(List<AllyLogData> dataList)
	{
		if (dataList == null || dataList.Count == 0)
		{
			return;
		}
		int count = dataList.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanMengWaiJiaoInfoItem zhanMengWaiJiaoInfoItem = U3DUtils.NEW<ZhanMengWaiJiaoInfoItem>();
			AllyLogData value = dataList[i];
			zhanMengWaiJiaoInfoItem.SetValue(value);
			UIPanel component = zhanMengWaiJiaoInfoItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, zhanMengWaiJiaoInfoItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(zhanMengWaiJiaoInfoItem);
		}
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

	public ListBox itemList;

	private ObservableCollection _ItemCollection;
}
