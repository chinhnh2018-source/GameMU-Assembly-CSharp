using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class DailyPrivilegeItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.list_goods)
		{
			this.initPos = this.list_goods.transform.localPosition;
		}
	}

	public int dailyPrivilegeID
	{
		get
		{
			return this._dailyPrivilegeID;
		}
		set
		{
			this._dailyPrivilegeID = value;
			this.LoadDailyData();
		}
	}

	public int completeCount
	{
		set
		{
			this._completCount = value;
			this.SetCompeteCount(this.sum);
		}
	}

	public bool highlight
	{
		set
		{
			this._highlight = value;
			this.SetHighlight(value);
		}
	}

	public bool UnFinished()
	{
		return this._completCount < this.sum;
	}

	private void LoadDailyData()
	{
		if (this._dailyPrivilegeID <= 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("JianFu");
		XElement xelement = Global.GetXElement(gameResXml, "JianFu", "ID", this._dailyPrivilegeID.ToString());
		if (xelement == null)
		{
			return;
		}
		this.LoadAwardValueList(xelement);
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
		this.title.text = xelementAttributeStr;
		this.sum = Global.GetXElementAttributeInt(xelement, "Num");
		this.SetCompeteCount(this.sum);
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Goods");
		this.LoadGoodsList(xelementAttributeStr2);
	}

	private void SetHighlight(bool selected = true)
	{
		if (null != this.highlightMark)
		{
			this.highlightMark.SetActive(selected);
		}
	}

	private void SetCompeteCount(int sum)
	{
		if (null != this.completeCountLabel)
		{
			this.completeCountLabel.Text = string.Format("({0}/{1})", this._completCount, sum);
		}
		if (null != this.signMark)
		{
			this.signMark.SetActive(this._completCount >= sum);
		}
	}

	private int GetExtraVIPPrivilegeCount()
	{
		int result = 0;
		if (this._dailyPrivilegeID / 100 == 1)
		{
			result = Global.GetSystemParamVipLeveValue("VIPJinYanFuBenNum");
		}
		else if (this._dailyPrivilegeID / 100 == 2)
		{
			result = Global.GetSystemParamVipLeveValue("VIPJinBiFuBenNum");
		}
		return result;
	}

	private void LoadAwardValueList(XElement itemAttribute)
	{
		int xelementAttributeInt = Global.GetXElementAttributeInt(itemAttribute, "Exp");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(itemAttribute, "BandJinBi");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(itemAttribute, "MoJing");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(itemAttribute, "ChengJiu");
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(itemAttribute, "ShengWang");
		int xelementAttributeInt6 = Global.GetXElementAttributeInt(itemAttribute, "ZhanGong");
		int xelementAttributeInt7 = Global.GetXElementAttributeInt(itemAttribute, "BandZuanShi");
		int xelementAttributeInt8 = Global.GetXElementAttributeInt(itemAttribute, "XingHun");
		int xelementAttributeInt9 = Global.GetXElementAttributeInt(itemAttribute, "YuanSuFenMo");
		int xelementAttributeInt10 = Global.GetXElementAttributeInt(itemAttribute, "ShouHuDianShu");
		int xelementAttributeInt11 = Global.GetXElementAttributeInt(itemAttribute, "ZaiZao");
		int xelementAttributeInt12 = Global.GetXElementAttributeInt(itemAttribute, "LingJing");
		int xelementAttributeInt13 = Global.GetXElementAttributeInt(itemAttribute, "RongYao");
		int num = 4;
		int num2 = 0;
		if (xelementAttributeInt > 0)
		{
			num2++;
			this.AddAwardValueIcon("ExpAward", xelementAttributeInt);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt2 > 0)
		{
			num2++;
			this.AddAwardValueIcon("BandMoneyAward", xelementAttributeInt2);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt3 > 0)
		{
			num2++;
			this.AddAwardValueIcon("MoJingAward", xelementAttributeInt3);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt4 > 0)
		{
			num2++;
			this.AddAwardValueIcon("ChengJiuAward", xelementAttributeInt4);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt5 > 0)
		{
			num2++;
			this.AddAwardValueIcon("ShengWangAward", xelementAttributeInt5);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt6 > 0)
		{
			num2++;
			this.AddAwardValueIcon("ZhanGongAward", xelementAttributeInt6);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt7 > 0)
		{
			num2++;
			this.AddAwardValueIcon("BindZuanAward", xelementAttributeInt7);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt8 > 0)
		{
			num2++;
			this.AddAwardValueIcon("XingHunAward", xelementAttributeInt8);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt9 > 0)
		{
			num2++;
			this.AddAwardValueIcon("YuanSuFenMo", xelementAttributeInt9);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt10 > 0)
		{
			num2++;
			this.AddAwardValueIcon("ShouHuDianShu", xelementAttributeInt10);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt11 > 0)
		{
			num2++;
			this.AddAwardValueIcon("ZaiZao", xelementAttributeInt11);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt12 > 0)
		{
			num2++;
			this.AddAwardValueIcon("LingJing", xelementAttributeInt12);
		}
		if (num2 >= num)
		{
			return;
		}
		if (xelementAttributeInt13 > 0)
		{
			num2++;
			this.AddAwardValueIcon("RongYao", xelementAttributeInt13);
		}
		if (num2 >= num)
		{
			return;
		}
	}

	private void AddAwardValueIcon(string name, int count)
	{
		JinRiKeZuoGiftItemIcon icon = U3DUtils.NEW<JinRiKeZuoGiftItemIcon>();
		icon.NetImage.Width = 27.0;
		icon.NetImage.Height = 27.0;
		string text = string.Empty;
		string url = string.Empty;
		if (name != null)
		{
			if (DailyPrivilegeItem.<>f__switch$map2 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ExpAward", 0);
				dictionary.Add("BandMoneyAward", 1);
				dictionary.Add("MoJingAward", 2);
				dictionary.Add("ChengJiuAward", 3);
				dictionary.Add("ShengWangAward", 4);
				dictionary.Add("ZhanGongAward", 5);
				dictionary.Add("BindZuanAward", 6);
				dictionary.Add("XingHunAward", 7);
				dictionary.Add("YuanSuFenMo", 8);
				dictionary.Add("ShouHuDianShu", 9);
				dictionary.Add("ZaiZao", 10);
				dictionary.Add("LingJing", 11);
				dictionary.Add("RongYao", 12);
				DailyPrivilegeItem.<>f__switch$map2 = dictionary;
			}
			int num;
			if (DailyPrivilegeItem.<>f__switch$map2.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					text = "exp";
					icon.NetImage.Width = 43.0;
					icon.NetImage.Height = 43.0;
					break;
				case 1:
					text = "bindmoney";
					break;
				case 2:
					text = "mojing";
					break;
				case 3:
					text = "chengjiu";
					break;
				case 4:
					text = "shengwang";
					break;
				case 5:
					text = "zhangong";
					break;
				case 6:
					text = "binddiamond";
					break;
				case 7:
					text = "xinghun";
					break;
				case 8:
					text = "yuansu";
					break;
				case 9:
					text = "shouhu";
					break;
				case 10:
					text = "zaizao";
					break;
				case 11:
					text = "lingjing";
					break;
				case 12:
					text = "rongyao";
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			url = "NetImages/GameRes/Images/Unit/" + text + ".png";
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
		ObservableCollection itemsSource = this.list_values.ItemsSource;
		itemsSource.AddNoUpdate(icon);
		icon.gameObject.AddComponent<UIDragPanelContents>();
	}

	private void LoadGoodsList(string goodsStr)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		int num = 3;
		int num2 = 84;
		float num3 = (float)num2 * 0.8f * 0.5f;
		ObservableCollection itemsSource = this.list_goods.ItemsSource;
		int num4 = Math.Min(array.Length, num);
		for (int i = 0; i < num4; i++)
		{
			string goodsStr2 = array[i];
			this.AddGoodsIcon(goodsStr2, itemsSource);
		}
		float num5 = this.initPos.x + (float)(num - itemsSource.Count) * num3;
		this.list_goods.transform.localPosition = new Vector3(num5, this.initPos.y, this.initPos.z);
	}

	private void AddGoodsIcon(string goodsStr, ObservableCollection itemCollection)
	{
		if (string.IsNullOrEmpty(goodsStr))
		{
			return;
		}
		string[] array = goodsStr.Split(new char[]
		{
			','
		});
		if (array.Length != 7)
		{
			return;
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		UIHelper.AddGoodsIcon3(itemCollection, dummyGoodsDataMu, true, false, false, "bagGrid4_bak");
	}

	public GTextBlockOutLine title;

	public GTextBlockOutLine completeCountLabel;

	public ListBox list_values;

	public ListBox list_goods;

	private Vector3 initPos = Vector3.zero;

	private int sum;

	public GameObject highlightMark;

	public GameObject signMark;

	private int _dailyPrivilegeID;

	private int _completCount;

	private bool _highlight;
}
