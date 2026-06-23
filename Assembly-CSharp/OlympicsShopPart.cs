using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OlympicsShopPart : UserControl
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
		base.InitializeComponent();
	}

	public void InitData()
	{
		if (this.tmpDataList == null || this.tmpDataList.Count <= 0)
		{
			return;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < this.tmpDataList.Count; i++)
		{
			OlympicsShopItem olympicsShopItem = U3DUtils.NEW<OlympicsShopItem>();
			OlympicsShopData value = this.tmpDataList[i];
			olympicsShopItem.SetValue(value);
			olympicsShopItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendOlympicsBuyGoodsRequest(e.ID, 1);
			};
			UIPanel component = olympicsShopItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, olympicsShopItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(olympicsShopItem);
		}
		Super.HideNetWaiting();
	}

	public void RefreshData()
	{
		this.tmpDataList = OlympicsDataManage.GetShopData();
		if (this.tmpDataList == null || this.tmpDataList.Count <= 0)
		{
			return;
		}
		this.InitData();
	}

	public void RefreshData(int id, int ownedBuyCount, int totalBuyCount)
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			OlympicsShopItem olympicsShopItem = U3DUtils.AS<OlympicsShopItem>(this.ItemCollection[i]);
			if (olympicsShopItem.currentShopData.ID == id)
			{
				olympicsShopItem.currentShopData.NumSingleBuy = ownedBuyCount;
				olympicsShopItem.currentShopData.NumFullBuy = totalBuyCount;
				olympicsShopItem.SetValue(olympicsShopItem.currentShopData);
			}
		}
		Super.HideNetWaiting();
	}

	public ListBox itemList;

	private ObservableCollection _ItemCollection;

	private List<OlympicsShopData> tmpDataList;
}
