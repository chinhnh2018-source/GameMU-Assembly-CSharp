using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuLiBaoPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ListCollection = this.m_ListGoods.ItemsSource;
		this.m_ShowTitle.URL = Global.GetZhuTiFuNetImg("Title", this.m_ShowTitle.URL);
	}

	private void Init()
	{
		this.m_BtnLingQu.Text = Global.GetLang("领取");
		string goodsIDs = string.Empty;
		if (!string.IsNullOrEmpty(this.m_ThemeActivityLiBao.GoodsTwo))
		{
			goodsIDs = this.m_ThemeActivityLiBao.GoodsOne + "@" + this.m_ThemeActivityLiBao.GoodsTwo;
		}
		else
		{
			goodsIDs = this.m_ThemeActivityLiBao.GoodsOne;
		}
		Super.LoadGoodsList(goodsIDs, this.m_ListCollection);
		this.m_BtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteFetchActivityAward(151, 0);
		};
	}

	public void GetData(string XmlList)
	{
		this.AddXmlThemeActivityLiBao(XmlList);
		this.Init();
	}

	public void RefreshData(bool mIsLingQu)
	{
		this.m_BtnLingQu.isEnabled = mIsLingQu;
		if (!mIsLingQu)
		{
			this.m_BtnLingQu.target.spriteName = "yilingqu";
			this.m_BtnLingQu.target.transform.localScale = new Vector3(90f, 66f, 1f);
			this.m_BtnLingQu.Text = string.Empty;
		}
	}

	private void AddXmlThemeActivityLiBao(string XmlList)
	{
		XElement xelement = XElement.Parse(XmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityLiBao");
		for (int i = 0; i < xelementList.Count; i++)
		{
			this.m_ThemeActivityLiBao = new ThemeActivityLiBao
			{
				ID = Global.GetXElementAttributeInt(xelementList[i], "ID"),
				GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne"),
				GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo")
			};
		}
	}

	public GButton m_BtnLingQu;

	public ShowNetImage m_ShowTitle;

	public ListBox m_ListGoods;

	private ObservableCollection m_ListCollection;

	private ThemeActivityLiBao m_ThemeActivityLiBao;
}
