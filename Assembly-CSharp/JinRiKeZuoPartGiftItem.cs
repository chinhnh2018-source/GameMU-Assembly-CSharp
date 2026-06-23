using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class JinRiKeZuoPartGiftItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnGoTo.Text = Global.GetLang("立即前往");
		this.m_Title.Pivot = 3;
		this.m_Title.X = -330.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this._ItemCollection = this.m_ListWuPin.ItemsSource;
		}
		if (null != this.m_BtnGoTo)
		{
			this.m_BtnGoTo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(this.ItemElement, "Link");
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = xelementAttributeInt
				});
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

	public void RefreshByID(int id, int remainCount = 0)
	{
		if (id > 0)
		{
			XElement isolateResXml = Global.GetIsolateResXml("JinRiKeZuo");
			this.ItemElement = Global.GetXElement(isolateResXml, "KeZuo", "ID", id.ToString());
			this.LoadGiftItemList();
			string xelementAttributeStr = Global.GetXElementAttributeStr(this.ItemElement, "Name");
			this.m_Title.text = xelementAttributeStr;
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(this.ItemElement, "Image");
			this.Bak.URL = "NetImages/GameRes/Images/Plate/" + xelementAttributeStr2 + ".jpg";
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(this.ItemElement, "Link");
			if (xelementAttributeStr3 != "1000")
			{
				this.TextRemainCount.text = string.Format(Global.GetLang("剩余次数:{0}次"), remainCount.ToString());
			}
			else
			{
				int num = remainCount / 60;
				this.TextRemainCount.text = string.Format(Global.GetLang("剩余时间:{0}分钟"), num.ToString());
			}
		}
		else
		{
			this.Bak.URL = "NetImages/GameRes/Images/Plate/kezuoitem_empty.jpg";
			this.m_Title.gameObject.SetActive(false);
			this.m_BtnGoTo.gameObject.SetActive(false);
			this.TextRemainCount.gameObject.SetActive(false);
			this.m_ListWuPin.gameObject.SetActive(false);
		}
	}

	public void LoadGiftItemList()
	{
		this.ItemCollection.Clear();
		int xelementAttributeInt = Global.GetXElementAttributeInt(this.ItemElement, "ExpAward");
		if (xelementAttributeInt > 0)
		{
			this.AddGiftExceptGoodsIcon("ExpAward", xelementAttributeInt);
		}
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(this.ItemElement, "BandMoneyAward");
		if (xelementAttributeInt2 > 0)
		{
			this.AddGiftExceptGoodsIcon("BandMoneyAward", xelementAttributeInt2);
		}
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(this.ItemElement, "MoJingAward");
		if (xelementAttributeInt3 > 0)
		{
			this.AddGiftExceptGoodsIcon("MoJingAward", xelementAttributeInt3);
		}
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(this.ItemElement, "ChengJiuAward");
		if (xelementAttributeInt4 > 0)
		{
			this.AddGiftExceptGoodsIcon("ChengJiuAward", xelementAttributeInt4);
		}
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(this.ItemElement, "ShengWangAward");
		if (xelementAttributeInt5 > 0)
		{
			this.AddGiftExceptGoodsIcon("ShengWangAward", xelementAttributeInt5);
		}
		int xelementAttributeInt6 = Global.GetXElementAttributeInt(this.ItemElement, "ZhanGongAward");
		if (xelementAttributeInt6 > 0)
		{
			this.AddGiftExceptGoodsIcon("ZhanGongAward", xelementAttributeInt6);
		}
		int xelementAttributeInt7 = Global.GetXElementAttributeInt(this.ItemElement, "BindZuanAward");
		if (xelementAttributeInt7 > 0)
		{
			this.AddGiftExceptGoodsIcon("BindZuanAward", xelementAttributeInt7);
		}
		int xelementAttributeInt8 = Global.GetXElementAttributeInt(this.ItemElement, "XingHunAward");
		if (xelementAttributeInt8 > 0)
		{
			this.AddGiftExceptGoodsIcon("XingHunAward", xelementAttributeInt8);
		}
		int xelementAttributeInt9 = Global.GetXElementAttributeInt(this.ItemElement, "YuanSuFenMo");
		if (xelementAttributeInt9 > 0)
		{
			this.AddGiftExceptGoodsIcon("YuanSuFenMo", xelementAttributeInt9);
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(this.ItemElement, "GoodsAward");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			string[] array = xelementAttributeStr.Split(new char[]
			{
				'|'
			});
			if (array.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length == 7)
				{
					this.AddGoodsIcon(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]));
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int goodsID, int gcount)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int goodsOccByID = Global.GetGoodsOccByID(goodsID);
			if (goodsOccByID != -1 && (goodsOccByID & 1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation)) == 0)
			{
				return;
			}
			JinRiKeZuoGiftItemIcon jinRiKeZuoGiftItemIcon = U3DUtils.NEW<JinRiKeZuoGiftItemIcon>();
			jinRiKeZuoGiftItemIcon.NetImage.URL = StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			});
			jinRiKeZuoGiftItemIcon.NetImage.Width = 32.0;
			jinRiKeZuoGiftItemIcon.NetImage.Height = 32.0;
			jinRiKeZuoGiftItemIcon.Text.text = "x" + gcount.ToString();
			this.ItemCollection.AddNoUpdate(jinRiKeZuoGiftItemIcon);
			jinRiKeZuoGiftItemIcon.gameObject.AddComponent<UIDragPanelContents>();
		}
	}

	private void AddGiftExceptGoodsIcon(string name, int count)
	{
		JinRiKeZuoGiftItemIcon icon = U3DUtils.NEW<JinRiKeZuoGiftItemIcon>();
		icon.NetImage.Width = 27.0;
		icon.NetImage.Height = 27.0;
		string url = string.Empty;
		if (name != null)
		{
			if (JinRiKeZuoPartGiftItem.<>f__switch$map4 == null)
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
				JinRiKeZuoPartGiftItem.<>f__switch$map4 = dictionary;
			}
			int num;
			if (JinRiKeZuoPartGiftItem.<>f__switch$map4.TryGetValue(name, ref num))
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

	public GButton m_BtnGoTo;

	public ShowNetImage Bak;

	public GTextBlockOutLine TextRemainCount;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	private XElement ItemElement;
}
