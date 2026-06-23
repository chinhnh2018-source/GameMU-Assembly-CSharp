using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ZiYuanZhaoHuiPartGiftItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnByGold.Text = Global.GetLang("金币找回");
		this.m_BtnByDiamond.Text = Global.GetLang("钻石找回");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this._ItemCollection = this.m_ListWuPin.ItemsSource;
		}
		if (null != this.m_BtnByGold)
		{
			this.m_BtnByGold.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = this.ResourceInfo.type;
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(e, new DPSelectedItemEventArgs
					{
						IDType = 1,
						ID = type,
						Data = this.ResourceInfo
					});
				}
			};
		}
		if (null != this.m_BtnByDiamond)
		{
			this.m_BtnByDiamond.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = this.ResourceInfo.type;
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(e, new DPSelectedItemEventArgs
					{
						IDType = 2,
						ID = type,
						Data = this.ResourceInfo
					});
				}
			};
		}
	}

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

	public void Refresh(OldResourceInfo resourceInfo)
	{
		if (resourceInfo != null)
		{
			this.ResourceInfo = resourceInfo;
			this.LoadGiftItemList();
			XElement gameResXml = Global.GetGameResXml("ZiYuanZhaoHuiType");
			this.ItemElement = Global.GetXElement(gameResXml, "ZhaoHui", "ID", resourceInfo.type.ToString());
			string xelementAttributeStr = Global.GetXElementAttributeStr(this.ItemElement, "Name");
			this.m_Title.text = xelementAttributeStr;
			this.TextRemainCount.text = string.Format(Global.GetLang("昨日未完成次数:{0}"), resourceInfo.leftCount);
		}
	}

	public void LoadGiftItemList()
	{
		this.ItemCollection.Clear();
		int exp = this.ResourceInfo.exp;
		if (exp > 0)
		{
			this.AddGiftExceptGoodsIcon("ExpAward", exp);
		}
		int bandmoney = this.ResourceInfo.bandmoney;
		if (bandmoney > 0)
		{
			this.AddGiftExceptGoodsIcon("BandMoneyAward", bandmoney);
		}
		int mojing = this.ResourceInfo.mojing;
		if (mojing > 0)
		{
			this.AddGiftExceptGoodsIcon("MoJingAward", mojing);
		}
		int chengjiu = this.ResourceInfo.chengjiu;
		if (chengjiu > 0)
		{
			this.AddGiftExceptGoodsIcon("ChengJiuAward", chengjiu);
		}
		int shengwang = this.ResourceInfo.shengwang;
		if (shengwang > 0)
		{
			this.AddGiftExceptGoodsIcon("ShengWangAward", shengwang);
		}
		int zhangong = this.ResourceInfo.zhangong;
		if (zhangong > 0)
		{
			this.AddGiftExceptGoodsIcon("ZhanGongAward", zhangong);
		}
		int bandDiamond = this.ResourceInfo.bandDiamond;
		if (bandDiamond > 0)
		{
			this.AddGiftExceptGoodsIcon("BindZuanAward", bandDiamond);
		}
		int xinghun = this.ResourceInfo.xinghun;
		if (xinghun > 0)
		{
			this.AddGiftExceptGoodsIcon("XingHunAward", xinghun);
		}
		int yuanSuFenMo = this.ResourceInfo.yuanSuFenMo;
		if (yuanSuFenMo > 0)
		{
			this.AddGiftExceptGoodsIcon("YuanSuFenMo", yuanSuFenMo);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGiftExceptGoodsIcon(string name, int count)
	{
		JinRiKeZuoGiftItemIcon icon = U3DUtils.NEW<JinRiKeZuoGiftItemIcon>();
		icon.NetImage.Width = 27.0;
		icon.NetImage.Height = 27.0;
		string url = string.Empty;
		if (name != null)
		{
			if (ZiYuanZhaoHuiPartGiftItem.<>f__switch$mapB == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(9);
				dictionary.Add("ExpAward", 0);
				dictionary.Add("BandMoneyAward", 1);
				dictionary.Add("MoJingAward", 2);
				dictionary.Add("ChengJiuAward", 3);
				dictionary.Add("ShengWangAward", 4);
				dictionary.Add("ZhanGongAward", 5);
				dictionary.Add("BindZuanAward", 6);
				dictionary.Add("XingHunAward", 7);
				dictionary.Add("YuanSuFenMo", 8);
				ZiYuanZhaoHuiPartGiftItem.<>f__switch$mapB = dictionary;
			}
			int num;
			if (ZiYuanZhaoHuiPartGiftItem.<>f__switch$mapB.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					url = "NetImages/GameRes/Images/Unit/exp.png";
					icon.NetImage.Width = 43.0;
					icon.NetImage.Height = 43.0;
					break;
				case 1:
					url = "NetImages/GameRes/Images/Unit/bindmoney.png";
					break;
				case 2:
					url = "NetImages/GameRes/Images/Unit/mojing.png";
					break;
				case 3:
					url = "NetImages/GameRes/Images/Unit/chengjiu.png";
					break;
				case 4:
					url = "NetImages/GameRes/Images/Unit/shengwang.png";
					break;
				case 5:
					url = "NetImages/GameRes/Images/Unit/zhangong.png";
					break;
				case 6:
					url = "NetImages/GameRes/Images/Unit/binddiamond.png";
					break;
				case 7:
					url = "NetImages/GameRes/Images/Unit/xinghun.png";
					break;
				case 8:
					url = "NetImages/GameRes/Images/Unit/yuansu.png";
					break;
				}
			}
		}
		icon.NetImage.URL = url;
		icon.NetImage.ImageDownloaded = delegate(object s)
		{
			if (icon.NetImage.Texture != null)
			{
				icon.NetImage.Texture.MakePixelPerfect();
			}
		};
		icon.Text.text = count.ToString();
		this.ItemCollection.AddNoUpdate(icon);
		icon.gameObject.AddComponent<UIDragPanelContents>();
	}

	public GTextBlockOutLine m_Title;

	public GButton m_BtnByGold;

	public GButton m_BtnByDiamond;

	public GTextBlockOutLine TextRemainCount;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	private XElement ItemElement;

	public OldResourceInfo ResourceInfo;
}
