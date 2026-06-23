using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ActivityGoodsListPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ListBox.ItemsSource;
		}
	}

	protected override void InitializeComponent()
	{
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs());
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
		this._ListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		ActivityGoodsListPartItem activityGoodsListPartItem = U3DUtils.AS<ActivityGoodsListPartItem>(this._ListBox.SelectedItem);
		if (null != activityGoodsListPartItem)
		{
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = activityGoodsListPartItem.GoodsID;
			GTipServiceEx.ShowTip(activityGoodsListPartItem._Icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		}
	}

	public void InitPartData(string goodsItemList)
	{
		if (string.IsNullOrEmpty(goodsItemList))
		{
			return;
		}
		string[] array = goodsItemList.Split(new char[]
		{
			'|'
		});
		int num = 0;
		foreach (string strNum in array)
		{
			int num2 = strNum.SafeToInt32(0);
			ActivityGoodsListPartItem activityGoodsListPartItem = U3DUtils.NEW<ActivityGoodsListPartItem>();
			if (null != activityGoodsListPartItem)
			{
				this.ItemCollection.AddNoUpdate(activityGoodsListPartItem);
				activityGoodsListPartItem.Init();
				activityGoodsListPartItem.name = num++.ToString("0000");
				activityGoodsListPartItem.GoodsID = num2;
				string goodsIconString = Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(num2));
				activityGoodsListPartItem._Icon.BodyURL = new ImageURL(goodsIconString, false, 0);
				activityGoodsListPartItem._Label.text = Global.GetGoodsNameByID(num2, true);
				Super.InitGoodsGIcon(activityGoodsListPartItem._Icon, Global.GetFakeEquipGoodsData(num2, 0, 0), true, IconTextTypes.Qianghua);
				UIDragObject component = activityGoodsListPartItem.gameObject.GetComponent<UIDragObject>();
				if (null != component)
				{
					component.target = this._ListBox.transform;
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	public GButton _Close;

	public ListBox _ListBox;

	public ShowNetImage _bak;

	public DPSelectedItemEventHandler DPSelectedItem;
}
