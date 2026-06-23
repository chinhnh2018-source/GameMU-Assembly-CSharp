using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TianTiAwardPart : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.collection = this.m_ListBox.Items;
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		List<XElement> xmlList = Enumerable.ToList<XElement>(Global.GetGameResXml(string.Format("Config/DuanWeiRankAward.xml", new object[0])).Elements("Award"));
		this.init(xmlList);
	}

	public void init(List<XElement> xmlList)
	{
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement xelement = xmlList[i];
			if (xelement != null)
			{
				TianTiAwardItem component = U3DUtils.NEW<TianTiAwardItem>();
				this.collection.Add(component);
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "MinRank");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "MaxRank");
			}
		}
	}

	public void SetLingquInfo(long cd)
	{
	}

	public void SetRewardGoodIcon(List<GGoodIcon> iconList)
	{
		int num = 0;
		foreach (GGoodIcon ggoodIcon in iconList)
		{
			if (ggoodIcon != null)
			{
				U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
				Super.InitGoodsGIcon(ggoodIcon, (GoodsData)ggoodIcon.ItemObject, Global.CanUseGoods(ggoodIcon.ItemCode, false, true), IconTextTypes.Qianghua);
				ggoodIcon.transform.localPosition = new Vector3(-418f + (float)num++ * 90f, -100f, 0f);
			}
		}
	}

	public void SetRankingData(int ranking)
	{
	}

	public GButton m_BtnClose;

	public ListBox m_ListBox;

	private ObservableCollection collection;

	public UILabel m_LabelDuanWei;

	public UILabel m_LabelDuanWeiJiFen;

	public UILabel m_LabelMeiRiTiaoJian;

	public UIButton m_BtnDuanWei;

	public UIButton m_BtnMeiRiJiangLi;
}
