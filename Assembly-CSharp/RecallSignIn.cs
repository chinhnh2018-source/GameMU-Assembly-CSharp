using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RecallSignIn : RecallGoodsEx
{
	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.activityInfo)
		{
			this.AddXmlHuoDong();
			DateTime dateTime = DateTime.Parse(this.dicHuoDongZhaoHui[1].NotLoggedInBegin);
			DateTime dateTime2 = DateTime.Parse(this.dicHuoDongZhaoHui[1].NotLoggedInFinish);
			TimeSpan timeSpan = dateTime2 - dateTime;
			this.activityInfo.text = string.Concat(new string[]
			{
				Global.GetLang(DateTime.Parse(this.dicHuoDongZhaoHui[1].NotLoggedInFinish).AddDays(1.0).ToString("yyyy-MM-dd") + Global.GetLang("前连续")),
				(timeSpan.Days + 1).ToString(),
				Global.GetLang("天未登陆，获得回归资格"),
				Environment.NewLine,
				Global.GetLang("回归用户累积登陆领取(每个账号只能领取1次)")
			});
		}
	}

	public void AddXmlHuoDong()
	{
		XElement xelement = null;
		if (RecallGoodsEx.m_UserReturnXmlData != null)
		{
			for (int i = 0; i < RecallGoodsEx.m_UserReturnXmlData.XmlNameList.Count; i++)
			{
				if (RecallGoodsEx.m_UserReturnXmlData.XmlNameList[i].Equals("HuoDongZhaoHui.xml"))
				{
					xelement = XElement.Parse(RecallGoodsEx.m_UserReturnXmlData.XmlList[i]);
				}
			}
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "HuoDongZhaoHui");
		for (int j = 0; j < xelementList.Count; j++)
		{
			HuoDongZhaoHui huoDongZhaoHui = new HuoDongZhaoHui();
			huoDongZhaoHui.ID = Global.GetXElementAttributeInt(xelementList[j], "ID");
			huoDongZhaoHui.HuoDongID = Global.GetXElementAttributeInt(xelementList[j], "HuoDongID");
			huoDongZhaoHui.BeginTime = Global.GetXElementAttributeStr(xelementList[j], "BeginTime");
			huoDongZhaoHui.FinishTime = Global.GetXElementAttributeStr(xelementList[j], "FinishTime");
			huoDongZhaoHui.NotLoggedInBegin = Global.GetXElementAttributeStr(xelementList[j], "NotLoggedInBegin");
			huoDongZhaoHui.NotLoggedInFinish = Global.GetXElementAttributeStr(xelementList[j], "NotLoggedInFinish");
			huoDongZhaoHui.Level = Global.GetXElementAttributeStr(xelementList[j], "Level");
			huoDongZhaoHui.VIP = Global.GetXElementAttributeInt(xelementList[j], "VIP");
			if (!this.dicHuoDongZhaoHui.ContainsKey(huoDongZhaoHui.ID))
			{
				this.dicHuoDongZhaoHui.Add(huoDongZhaoHui.ID, huoDongZhaoHui);
			}
		}
	}

	public override void OnDragFinished()
	{
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		this.ShowSignInListNew();
		return true;
	}

	private void ShowSignInListNew()
	{
		if (base.xml == null)
		{
			return;
		}
		if (base.xmlList == null)
		{
			return;
		}
		this._itemCollection.Clear();
		int unionLevel = Global.GetUnionLevel(-1, -1);
		if (ServerBufferZhaoHui.IsValid())
		{
			unionLevel = ServerBufferZhaoHui.Instance.UnionLevel;
			RecallGoodsEx.__lwj_log_andmore_etc.Add("[Use Role cur Level show SignIn list]");
		}
		else
		{
			RecallGoodsEx.__lwj_log_andmore_etc.Add("[Use Server Return Level show SignIn list]");
		}
		for (int i = 0; i < base.xmlList.Count; i++)
		{
			XElement xelement = base.xmlList[i];
			int xmlID = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "ID"));
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Level");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "TimeOl");
			int day = Convert.ToInt32(xelementAttributeStr2);
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					',',
					'|'
				});
				if (array.Length != 4)
				{
					return;
				}
				int num = Global.SafeConvertToInt32(array[0]);
				int num2 = Global.SafeConvertToInt32(array[1]);
				int num3 = Global.SafeConvertToInt32(array[2]);
				int num4 = Global.SafeConvertToInt32(array[3]);
				int unionLevel2 = Global.GetUnionLevel(num, num2);
				int unionLevel3 = Global.GetUnionLevel(num3, num4);
				if (unionLevel >= unionLevel2 && unionLevel <= unionLevel3)
				{
					RecallSignInItem signInItem = null;
					signInItem = U3DUtils.NEW<RecallSignInItem>();
					signInItem.observer.Clear();
					signInItem.xmlID = xmlID;
					signInItem.minLv = num2;
					signInItem.minZh = num;
					signInItem.maxZh = num3;
					signInItem.maxLv = num4;
					signInItem.day = day;
					signInItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
					{
						if (Global.GetBaoGuoSpaceCount() > signInItem.observer.Count)
						{
							this.SendOpenAward(signInItem.xmlID);
						}
						else
						{
							Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
						}
					};
					if (null != signInItem.signDay_TopLayer)
					{
						signInItem.signDay_TopLayer.text = string.Format(Global.GetLang("第{0}天"), xelementAttributeStr2);
						signInItem.signDay_TopLayer.gameObject.SetActive(false);
					}
					if (null != signInItem.signDay_BottomLayer)
					{
						signInItem.signDay_BottomLayer.text = string.Format(Global.GetLang("第{0}天"), xelementAttributeStr2);
						signInItem.signDay_TopLayer.gameObject.SetActive(true);
					}
					if (null != signInItem.signBtn)
					{
						signInItem.signBtn.gameObject.SetActive(false);
					}
					if (null != signInItem)
					{
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "GoodsID1");
						this.AddIconToList(ref signInItem.observer, xelementAttributeStr3, true, false);
						string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "GoodsID2");
						this.AddIconToList(ref signInItem.observer, xelementAttributeStr4, true, true);
						signInItem.observer.DelayUpdate();
						this._itemCollection.AddNoUpdate(signInItem);
						this._itemCollection.DelayUpdate();
					}
					GGoodIcon[] componentsInChildren = signInItem.GetComponentsInChildren<GGoodIcon>();
					if (componentsInChildren != null)
					{
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							if (j > 3)
							{
								Object.Destroy(componentsInChildren[j].gameObject);
							}
						}
					}
					if (signInItem.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(signInItem.GetComponent<UIPanel>());
					}
				}
			}
		}
		base._UpdateRewardsListStatus();
	}

	private void AddIconToList(ref ObservableCollection observer, string goods, bool iconEnable, bool isOccupation = false)
	{
		if (observer == null || string.Empty == goods)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		if (goods.IndexOf("|") != -1)
		{
			string[] array = goods.Split(new char[]
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
					if (isOccupation)
					{
						if (MUJieripartChongzhiKingItem.IsTongGuo(array3[0], roleOcc))
						{
							goto IL_DA;
						}
					}
					GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(array3, false, iconEnable);
					observer.AddNoUpdate(goodsItemIconEx);
					goodsItemIconEx.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				}
				IL_DA:;
			}
		}
		else if (string.Empty != goods)
		{
			string[] array4 = goods.Split(new char[]
			{
				','
			});
			string text4 = array4[0];
			if (isOccupation)
			{
				if (!MUJieripartChongzhiKingItem.IsTongGuo(array4[0], roleOcc))
				{
					GGoodIcon goodsItemIconEx2 = this.GetGoodsItemIconEx(array4, false, iconEnable);
					observer.AddNoUpdate(goodsItemIconEx2);
					goodsItemIconEx2.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				}
			}
			else
			{
				GGoodIcon goodsItemIconEx3 = this.GetGoodsItemIconEx(array4, false, iconEnable);
				observer.AddNoUpdate(goodsItemIconEx3);
				goodsItemIconEx3.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
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
			ggoodIcon.isAutoSize = true;
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
		if (string.IsNullOrEmpty(text))
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
		if (string.IsNullOrEmpty(text))
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

	public Dictionary<int, HuoDongZhaoHui> dicHuoDongZhaoHui = new Dictionary<int, HuoDongZhaoHui>();
}
