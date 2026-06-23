using System;
using System.Linq;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TianTiLingQuJiangLiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		try
		{
			this.m_BtnLingQuJiangLi.GetComponentInChildren<UILabel>().text = Global.GetLang("领取奖励");
		}
		catch
		{
		}
		if (this.staticText != null)
		{
			this.staticText.text = Global.GetLang("领取奖励");
		}
		UIEventListener.Get(this.m_BtnClose.gameObject).onClick = delegate(GameObject s)
		{
			TianTiArenaPart.Instance.m_RankingGroup.gameObject.SetActive(true);
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(0f, 10f, 0f);
			Object.Destroy(this.gameObject);
		};
		this.m_LabelTitle.text = Global.GetLang("月度排名奖励");
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/DuanWeiRankAward.xml", new object[0]));
		int rank = 0;
		if (TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag != null && TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag.TianTiData != null)
		{
			rank = TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag.TianTiData.MonthDuanWeiRank;
			this.m_LabelPaiMing.text = Global.GetLang("您的月度排名为:") + rank;
			XElement xelement = Enumerable.ToList<XElement>(gameResXml.Elements("Award")).Find((XElement s) => s.AttributeInt("StarRank") <= rank && s.AttributeInt("EndRank") >= rank);
			if (xelement == null)
			{
				xelement = Enumerable.ToList<XElement>(gameResXml.Elements("Award")).Find((XElement s) => int.Parse(s.GetXElementAttrStr("ID")) == 17);
			}
			Debug.Log(xelement);
			string[] array = xelement.GetXElementAttrStr("Award").Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(array[i].Split(new char[]
				{
					','
				}), false, true);
				this.m_Items.Items.Add(goodsItemIconEx);
			}
		}
		UIEventListener.Get(this.m_BtnLingQuJiangLi.gameObject).onClick = delegate(GameObject s)
		{
			TCPGameServerCmds.CMD_SPR_TIANTI_GET_PAIMING_AWARDS.SendDataUseRoleID();
		};
	}

	public void aciton_CMD_SPR_TIANTI_GET_PAIMING_AWARDS(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(0f, 10f, 0f);
		Object.Destroy(base.gameObject);
		TianTiArenaPart.Instance.m_RankingGroup.gameObject.SetActive(true);
		if (num < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(num, false, false), 0, -1, -1, 0);
		}
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
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowGoodsTip(s);
		};
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

	public UILabel m_LabelPaiMing;

	public UILabel m_LabelTitle;

	public UIButton m_BtnClose;

	public UIButton m_BtnLingQuJiangLi;

	public ListBox m_Items;

	public UILabel staticText;
}
