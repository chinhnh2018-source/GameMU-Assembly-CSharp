using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhuanZhiPart : UserControl
{
	public string Level
	{
		set
		{
			this._Level_Value.text = value;
		}
	}

	public long Money
	{
		set
		{
			string text = value.ToString();
			if (Global.Data != null && Global.Data.roleData != null)
			{
				this._Money_Value.text = ColorCode.EncodingText1((long)(Global.Data.roleData.YinLiang + Global.Data.roleData.Money1), value, "fd010c");
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClose);
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
		bool visibility = this.Refresh(false);
		this._HelpAnim[0].Visibility = visibility;
		if (null != this._Submit)
		{
			this._Submit.Text = Global.GetLang("转职");
		}
		if (null != this._Title)
		{
			this._Title.Text = Global.GetLang("转职");
		}
	}

	public bool Refresh(bool showNotify = false)
	{
		bool flag = true;
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return false;
		}
		int occupation = Global.Data.roleData.Occupation;
		int occup = Global.CalcChangeOccupationID(occupation);
		this._Occupation0.Text = Global.GetOccupationStr(occupation);
		this._Occupation1.Text = Global.GetLang("尚未开放");
		XElement gameResXml = Global.GetGameResXml("Config/Roles/ZhuanZhi.xml");
		if (gameResXml == null)
		{
			return false;
		}
		XElement xelement = Global.GetXElement(gameResXml, "ZhuanZhis");
		if (xelement == null)
		{
			return false;
		}
		XElement xelement2 = Global.GetXElement(xelement, "ZhuanZhi", "OccupationID", occup.ToString());
		if (xelement2 == null)
		{
			this._Submit.isEnabled = false;
			return false;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Level");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "MinChangeLife");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "NeedJinBi");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "OccupationName");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "NeedGoods");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "AwardShuXing");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardGoods");
		this.Money = (long)xelementAttributeInt3;
		this._Occupation1.Text = xelementAttributeStr;
		this._JiangLi_ShuXing.Text = ColorCode.EncodingText(xelementAttributeInt4, "00ff00");
		this._JiangLi_ChengZhang.Text = string.Format(Global.GetLang("每次{0}可获得{1}点属性"), Global.GetColorStringForNGUIText(new object[]
		{
			"00ff00",
			Global.GetLang("升级")
		}), ColorCode.EncodingText(Global.GetOccupationJiaDian(occup), "00ff00"));
		if (!UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, true))
		{
			this.Level = Global.GetColorStringForNGUIText(new object[]
			{
				"fd010c",
				UIHelper.FormatLevelLimit(xelementAttributeInt, xelementAttributeInt2)
			});
			flag = false;
			if (showNotify)
			{
				this.ShowErrorMsg(string.Format(Global.GetLang("角色等级不够{0}, 暂时无法转职"), xelementAttributeInt));
				showNotify = false;
			}
		}
		else
		{
			this.Level = UIHelper.FormatLevelLimit(xelementAttributeInt, xelementAttributeInt2);
		}
		if (xelementAttributeInt3 > 0 && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < xelementAttributeInt3)
		{
			flag = false;
			if (showNotify)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				showNotify = false;
			}
		}
		if (xelementAttributeStr2.Length > 3)
		{
			this._Need_ItemList.ItemsSource.Clear();
			List<GoodsData> list = UIHelper.ParseRewardGoodsList(xelementAttributeStr2, 0, int.MaxValue);
			foreach (GoodsData goodsData in list)
			{
				int goodsID = goodsData.GoodsID;
				int gcount = goodsData.GCount;
				string goodsNameByID = Global.GetGoodsNameByID(goodsID, false);
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(this._Need_ItemList.ItemsSource, goodsData, null, false, "bagGrid4_bak");
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID)), false, 0);
				ggoodIcon.Text = string.Format("{0}/{1}", totalGoodsCountByID, gcount);
				ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
				if (gcount > 0 && totalGoodsCountByID < gcount)
				{
					flag = false;
					if (showNotify)
					{
						this.ShowErrorMsg(string.Format(Global.GetLang("{0}不足{1}个,暂时无法转生"), goodsNameByID, gcount));
						showNotify = false;
					}
				}
			}
			this._Need_ItemList.ItemsSource.DelayUpdate();
		}
		else
		{
			this._NeedItem.SetActive(false);
		}
		if (xelementAttributeStr3.Length > 3)
		{
			this._JiangLi_ItemList.ItemsSource.Clear();
			List<GoodsData> list2 = UIHelper.ParseRewardGoodsList(xelementAttributeStr3, 0, int.MaxValue);
			foreach (GoodsData goodsData2 in list2)
			{
				int goodsID2 = goodsData2.GoodsID;
				int gcount2 = goodsData2.GCount;
				GGoodIcon ggoodIcon2 = UIHelper.AddGoodsIcon(this._JiangLi_ItemList.ItemsSource, goodsData2, null, false, "bagGrid4_bak");
				ggoodIcon2.isAutoSize = true;
				ggoodIcon2.BodyURL = new ImageURL(Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID2)), false, 0);
				ggoodIcon2.Text = gcount2.ToString();
				ggoodIcon2.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon2.TextVerticalAlignment = global::Layout.Bottom;
			}
			this._JiangLi_ItemList.ItemsSource.DelayUpdate();
		}
		this._Submit.isEnabled = flag;
		return flag;
	}

	private void ShowErrorMsg(string msg)
	{
		Super.HintMainText(msg, 10, 3);
	}

	private void OnClose(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this._Close, null);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		bool flag = this.Refresh(true);
		this._HelpAnim[0].Visibility = flag;
		if (flag)
		{
			GameInstance.Game.SpriteExecuteZhuanZhi();
		}
		SystemHelpMgr.OnAction(UIObjIDs.ZhuanZhiSubmit, HelpStateEvents.Clicked, -1);
	}

	public void NotifyResult(int ret, int roleID, int OccuID, int totalPoint)
	{
		switch (ret + 4)
		{
		case 0:
			this.ShowErrorMsg(string.Format(Global.GetLang("所需物品数量不足"), new object[0]));
			break;
		case 1:
			this.ShowErrorMsg(string.Format(Global.GetLang("没有所需物品"), new object[0]));
			break;
		case 2:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			break;
		case 3:
			this.ShowErrorMsg(string.Format(Global.GetLang("角色等级不够"), new object[0]));
			break;
		case 5:
		{
			Global.Data.roleData.Occupation = OccuID;
			if (Global.Data.CurrentRolePropFields != null && Global.Data.CurrentRolePropFields.Length >= 21)
			{
				Global.Data.CurrentRolePropFields[20] = (double)totalPoint;
			}
			XElement gameResXml = Global.GetGameResXml("Config/Roles/ZhuanZhi.xml");
			if (gameResXml != null)
			{
				XElement xelement = Global.GetXElement(gameResXml, "ZhuanZhis");
				if (xelement != null)
				{
					XElement xelement2 = Global.GetXElement(xelement, "ZhuanZhi", "OccupationID", OccuID.ToString());
					if (xelement2 != null)
					{
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "OccupationName");
						this.ShowErrorMsg(string.Format(Global.GetLang("成功转职为{0}"), xelementAttributeStr));
						this.OnClose(null, null);
					}
				}
			}
			break;
		}
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			SystemHelpPart.SetMask(this._Submit.transform, default(Vector4));
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		SystemHelpMgr.OnAction(UIObjIDs.ZhuanZhiPart, HelpStateEvents.Inactived, -1);
	}

	public TextBlock _Title;

	public TextBlock _Occupation0;

	public TextBlock _Occupation1;

	public ListBox _Need_ItemList;

	public ListBox _JiangLi_ItemList;

	public GTextBlockOutLine _Level_Value;

	public GTextBlockOutLine _Money_Value;

	public TextBlock _JiangLi_ShuXing;

	public TextBlock _JiangLi_ChengZhang;

	public GButton _Submit;

	public GButton _Close;

	public ShowNetImage _Background;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject _NeedItem;

	public CAnimation[] _HelpAnim;
}
