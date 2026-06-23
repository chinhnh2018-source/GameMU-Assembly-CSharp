using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class BoCaiStorePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.btnHeiDi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(this.daiBiNumber);
		this.labHuoBi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			totalGoodsCountByID
		});
		GameInstance.Game.SendBoCaiShangChengData();
	}

	public void SetDataList(BoCaiShopInfo data)
	{
		if (data != null && data.Info != 0)
		{
			Super.HintMainText(IConfigbase<ConfigCaiShuZi>.Instance.ErrorString((BocaiSysMsgErr)data.Info), 10, 3);
			return;
		}
		this.obser = this.listBox.ItemsSource;
		this.obser.Clear();
		Dictionary<int, DuiHuanShangChengVO>.Enumerator enumerator = IConfigbase<ConfigCaiShuZi>.Instance.DicDuiHuanData().GetEnumerator();
		while (enumerator.MoveNext())
		{
			BoCaiStoreItem boCaiStoreItem = U3DUtils.NEW<BoCaiStoreItem>();
			this.obser.AddNoUpdate(boCaiStoreItem);
			KeyValuePair<int, DuiHuanShangChengVO> keyValuePair = enumerator.Current;
			DuiHuanShangChengVO value = keyValuePair.Value;
			string[] array = value.WuPinID.Split(new char[]
			{
				','
			});
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			BoCaiStoreItem boCaiStoreItem2 = boCaiStoreItem;
			GoodsData gd = dummyGoodsDataMu;
			BoCaiTypeEnum type = BoCaiTypeEnum.Bocai_Dice;
			KeyValuePair<int, DuiHuanShangChengVO> keyValuePair2 = enumerator.Current;
			boCaiStoreItem2.SetData(gd, type, keyValuePair2.Value);
			if (boCaiStoreItem.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(boCaiStoreItem.GetComponent<UIPanel>());
			}
			if (boCaiStoreItem.GetComponent<BoxCollider>() != null)
			{
				boCaiStoreItem.GetComponent<BoxCollider>().size = new Vector3(270f, 100f, 1f);
			}
			KeyValuePair<int, DuiHuanShangChengVO> keyValuePair3 = enumerator.Current;
			int num = keyValuePair3.Value.MeiRiShangXianDan;
			if (num == -1)
			{
				num = 99;
			}
			else if (data != null && data.ItemList != null)
			{
				for (int i = 0; i < data.ItemList.Count; i++)
				{
					string wuPinID = data.ItemList[i].WuPinID;
					KeyValuePair<int, DuiHuanShangChengVO> keyValuePair4 = enumerator.Current;
					if (wuPinID.Equals(keyValuePair4.Value.WuPinID))
					{
						KeyValuePair<int, DuiHuanShangChengVO> keyValuePair5 = enumerator.Current;
						num = keyValuePair5.Value.MeiRiShangXianDan - data.ItemList[i].BuyNum;
					}
				}
			}
			boCaiStoreItem.Number = num;
		}
		this.listBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			this.OnItem = this.listBox.SelectedItem.GetComponent<BoCaiStoreItem>();
			if (this.OnItem == null)
			{
				return;
			}
			int num2 = Global.GetTotalGoodsCountByID(this.daiBiNumber) / this.OnItem.XmlVo.DaiBiJiaGe;
			if (num2 <= 0)
			{
				Super.HintMainText(Global.GetLang("欢乐代币不足"), 10, 3);
				return;
			}
			if (this.OnItem.XmlVo.MeiRiShangXianDan == -1)
			{
				num2 = Mathf.Min(num2, 99);
			}
			else
			{
				num2 = Mathf.Min(num2, this.OnItem.Number);
			}
			if (num2 <= 0)
			{
				Super.HintMainText(Global.GetLang("今日购买上限"), 10, 3);
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.OnItem.Gd.GoodsID);
			HuiGuiData.OpenBuyItemWindow(this.OnItem.XmlVo.WuPinID.SafeToInt32(0), goodsXmlNodeByID, this.OnItem.XmlVo.DaiBiJiaGe, num2, new Action<int, GoodVO, int>(this.OnBuyConfig));
			HuiGuiData.m_commonBuyPart.imgIcon.spriteName = "huanlebi";
		};
	}

	public void RefreshList(string[] str)
	{
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(this.daiBiNumber);
		this.labHuoBi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			totalGoodsCountByID
		});
		if (str[3].SafeToInt32(0) != 0)
		{
			Super.HintMainText(IConfigbase<ConfigCaiShuZi>.Instance.ErrorString((BocaiSysMsgErr)str[3].SafeToInt32(0)), 10, 3);
			return;
		}
		int num = str[0].SafeToInt32(0);
		int num2 = str[1].SafeToInt32(0);
		for (int i = 0; i < this.obser.Count; i++)
		{
			BoCaiStoreItem component = this.obser.GetAt(i).GetComponent<BoCaiStoreItem>();
			if (component != null && component.XmlVo.ID == num)
			{
				if (component.XmlVo.MeiRiShangXianDan == -1)
				{
					component.Number = -1;
				}
				else
				{
					component.Number = component.XmlVo.MeiRiShangXianDan - num2;
				}
			}
		}
	}

	private BoCaiStoreItem OnItem { get; set; }

	private void OnBuyConfig(int buyId, GoodVO vo, int count)
	{
		HuiGuiData.CloseBuyItemWindow();
		GameInstance.Game.SendBoCaiShangChengBuy(this.OnItem.XmlVo.ID, count, this.OnItem.XmlVo.WuPinID);
	}

	public GButton btnClose;

	public ListBox listBox;

	public UILabel labHuoBi;

	public GButton btnHeiDi;

	private int daiBiNumber = 5440;

	private int maxCount = 99;

	private ObservableCollection obser;

	public DPSelectedItemEventHandler handlerClose;
}
