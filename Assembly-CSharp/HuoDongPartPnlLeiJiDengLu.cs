using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HuoDongPartPnlLeiJiDengLu : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		GameInstance.Game.SpriteQueryTotalLoginInfoCmd();
		if (null != this.m_ListLeiJiDengLu)
		{
			this.m_ListLeiJiDengLuObC = this.m_ListLeiJiDengLu.ItemsSource;
		}
	}

	private void LingJiang(int nEventID)
	{
		string text = string.Format(Global.GetLang("领取第{0}天"), nEventID);
		GameInstance.Game.SpriteGetTotalLoginInfoCmd(nEventID);
	}

	public void LingJiangWanCheng(int nIndex)
	{
		nIndex--;
		GameObject at = this.m_ListLeiJiDengLuObC.GetAt(nIndex);
		LianDengEveryDayItem component = at.gameObject.GetComponent<LianDengEveryDayItem>();
		if (null != component)
		{
			if (null != component.m_BtnLingJiang)
			{
				component.m_BtnLingJiang.gameObject.SetActive(false);
			}
			component.m_SpriteState.gameObject.SetActive(true);
			component.m_SpriteState.spriteName = "yilingqu";
			component.m_LblDayNum.gameObject.SetActive(false);
		}
	}

	public void InitEveryDayList(int nLeiJiTianShu = 0, int nLingJiangSate = 0)
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/NewHuoDongLoginNumGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.m_ListLeiJiDengLuObC.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Gift");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "TimeOl");
			int num = Convert.ToInt32(xelementAttributeStr);
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ID");
			LianDengEveryDayItem item = U3DUtils.NEW<LianDengEveryDayItem>();
			item.m_ListWuPinObC.Clear();
			item.m_nEventID = Convert.ToInt32(xelementAttributeStr2);
			item.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (Global.GetBaoGuoSpaceCount() > item.m_ListWuPinObC.Count)
				{
					this.LingJiang(e.ID);
				}
				else
				{
					Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
				}
			};
			if (null != item.m_LblDayNum)
			{
				item.m_LblDayNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"CC7432",
					string.Format(Global.GetLang("第{0}天"), xelementAttributeStr)
				});
			}
			if (null != item.m_LblDayNumLingQv)
			{
				item.m_LblDayNumLingQv.text = string.Format(Global.GetLang("第{0}天"), xelementAttributeStr);
			}
			bool bEnable = true;
			if (num <= nLeiJiTianShu)
			{
				int mask = Convert.ToInt32(xelementAttributeStr2);
				if (Global.GetIntSomeBit(nLingJiangSate, mask) == 1)
				{
					MUDebug.Log<string>(new string[]
					{
						"存在"
					});
					if (null != item.m_BtnLingJiang)
					{
						item.m_BtnLingJiang.gameObject.SetActive(false);
					}
					item.m_LblDayNum.gameObject.SetActive(false);
					bEnable = false;
					item.m_SpriteState.spriteName = "yilingqu";
					item.m_SpriteState.gameObject.SetActive(true);
				}
			}
			else
			{
				if (null != item.m_BtnLingJiang)
				{
					item.m_BtnLingJiang.gameObject.SetActive(false);
				}
				item.m_LblDayNumLingQv.gameObject.SetActive(false);
				item.m_SpriteState.gameObject.SetActive(true);
				item.m_LblDayNum.gameObject.SetActive(true);
				item.m_SpriteState.spriteName = "weidacheng";
			}
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "GoodsID1");
			this.AddIconToList(ref item.m_ListWuPinObC, xelementAttributeStr3, bEnable, false);
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "GoodsID2");
			this.AddIconToList(ref item.m_ListWuPinObC, xelementAttributeStr4, bEnable, true);
			item.m_ListWuPinObC.DelayUpdate();
			this.m_ListLeiJiDengLuObC.AddNoUpdate(item);
			this.m_ListLeiJiDengLuObC.DelayUpdate();
		}
	}

	private void AddIconToList(ref ObservableCollection listOBC, string strGoods, bool bEnable, bool bOccupation = false)
	{
		if (listOBC == null || string.Empty == strGoods)
		{
			return;
		}
		if (strGoods.IndexOf("|") != -1)
		{
			string[] array = strGoods.Split(new char[]
			{
				'|'
			});
			foreach (string text in array)
			{
				string text2 = text;
				if (string.Empty != text2)
				{
					string[] array3 = text2.Split(new char[]
					{
						','
					});
					string text3 = array3[0];
					if (this.IsGoodsToOccupation(Convert.ToInt32(text3)))
					{
						GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(array3, false, bEnable);
						listOBC.AddNoUpdate(goodsItemIconEx);
						goodsItemIconEx.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
					}
				}
			}
		}
		else if (string.Empty != strGoods)
		{
			string[] array4 = strGoods.Split(new char[]
			{
				','
			});
			string text4 = array4[0];
			if (this.IsGoodsToOccupation(Convert.ToInt32(text4)))
			{
				GGoodIcon goodsItemIconEx2 = this.GetGoodsItemIconEx(array4, false, bEnable);
				listOBC.AddNoUpdate(goodsItemIconEx2);
				goodsItemIconEx2.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			}
		}
	}

	private void SetGoodsIconImageGray(GGoodIcon icon)
	{
		if (null != icon)
		{
			UITexture component = icon.GoodImg.gameObject.gameObject.GetComponent<UITexture>();
			if (null != component)
			{
				component.shader = Shader.Find("Unlit/Gray");
			}
		}
	}

	private bool IsGoodsToOccupation(int nGoodsID)
	{
		if (0 >= nGoodsID)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		int mainOccupation = goodsXmlNodeByID.MainOccupation;
		return mainOccupation == -1 || (mainOccupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) && (mainOccupation != 3 || (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Strength > goodsXmlNodeByID.Intelligence) || (Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength)));
	}

	private GGoodIcon GetGoodsItemIcon(string strID, bool isDrag = false, int nLock = 0, bool bEnable = true, int nQiangHua = -1, int nZhuoYue = -1)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(strID), 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		dummyGoodsDataMu.Binding = nLock;
		dummyGoodsDataMu.ExcellenceInfo = nZhuoYue;
		dummyGoodsDataMu.Forge_level = nQiangHua;
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
			ggoodIcon.Width = 72.0;
			ggoodIcon.Height = 72.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.BackgroundSprite0.MakePixelPerfect();
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		if (!bEnable)
		{
		}
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private GGoodIcon GetGoodsItemIconEx(string[] goods, bool isDrag = false, bool bEnable = true)
	{
		if (goods.Length != 7)
		{
			return null;
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(goods[0]), Convert.ToInt32(goods[3]), Convert.ToInt32(goods[4]), Convert.ToInt32(goods[6]), Convert.ToInt32(goods[5]), Convert.ToInt32(goods[2]), Convert.ToInt32(goods[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemCode = dummyGoodsDataMu.GoodsID;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		if (!bEnable)
		{
		}
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.ItemCode);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			if (goodsData != null)
			{
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.ItemCode);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public ListBox m_ListLeiJiDengLu = new ListBox();

	private ObservableCollection m_ListLeiJiDengLuObC;
}
