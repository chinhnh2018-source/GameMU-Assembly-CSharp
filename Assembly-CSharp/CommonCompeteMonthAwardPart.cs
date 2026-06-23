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

public class CommonCompeteMonthAwardPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		try
		{
			if (this.m_BtnLingQuJiangLi.transform.GetChild(1).name.Equals("Label"))
			{
				this.m_BtnLingQuJiangLi.transform.GetChild(1).GetComponent<UILabel>().text = Global.GetLang("领取奖励");
			}
			this.m_LabelTitle.text = Global.GetLang("月度排名奖励");
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"越南东南亚英文调试用,报空！"
			});
		}
		UIEventListener.Get(this.m_BtnClose.gameObject).onClick = delegate(GameObject s)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, null);
			}
		};
		UIEventListener.Get(this.m_BtnLingQuJiangLi.gameObject).onClick = delegate(GameObject s)
		{
			this.RequestAcceptAward();
		};
	}

	public void InitTeamCompeteAward(int rank)
	{
		this.mAwardType = ECommonCompeteMonthAwardType.TeamCompepte;
		this.RankInfo(rank);
		this.ParseAwardByXml(rank);
	}

	private void ParseAwardByXml(int rank)
	{
		string empty = string.Empty;
		XElement gameResXml = Global.GetGameResXml(this.GetXmlName(out empty));
		XElement xelement = Enumerable.ToList<XElement>(gameResXml.Elements(empty)).Find((XElement s) => s.AttributeInt("StarRank") <= rank && s.AttributeInt("EndRank") >= rank);
		if (xelement == null)
		{
			xelement = Enumerable.ToList<XElement>(gameResXml.Elements(empty)).Find((XElement s) => int.Parse(s.GetXElementAttrStr("EndRank")) == -1);
		}
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
			if (goodsItemIconEx != null)
			{
				this.m_Items.Items.Add(goodsItemIconEx);
			}
		}
	}

	public void InitDaTaoShaAward(int rank)
	{
		this.mAwardType = ECommonCompeteMonthAwardType.DaYaoSha;
		this.m_LabelTitle.text = Global.GetLang("魔界大逃杀排名奖励");
		this.m_LabelPaiMing.text = Global.GetLang("上赛季排名：") + rank;
		this.ParseAwardByXml(rank);
	}

	private string GetXmlName(out string rootName)
	{
		string result = string.Empty;
		rootName = string.Empty;
		switch (this.mAwardType)
		{
		case ECommonCompeteMonthAwardType.TeamCompepte:
			result = "Config/TeamDuanWeiAward.xml";
			rootName = "TeamDuanWeiAward";
			break;
		case ECommonCompeteMonthAwardType.DaYaoSha:
			result = "Config/EscapeRankAward.xml";
			rootName = "EscapeRankAward";
			break;
		}
		return result;
	}

	private void RankInfo(int rank)
	{
		this.m_LabelPaiMing.text = Global.GetLang("您的月度排名为:") + rank;
	}

	private void RequestAcceptAward()
	{
		switch (this.mAwardType)
		{
		case ECommonCompeteMonthAwardType.TeamCompepte:
			GameInstance.Game.RequestZhanDuiAcceptAward();
			break;
		case ECommonCompeteMonthAwardType.DaYaoSha:
			GameInstance.Game.SendScceptDaTaoShaAwardData();
			break;
		}
	}

	public void RespondAcceptAward(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num >= 0)
		{
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, null);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
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

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel m_LabelPaiMing;

	public UILabel m_LabelTitle;

	public UIButton m_BtnClose;

	public UIButton m_BtnLingQuJiangLi;

	public ListBox m_Items;

	private ECommonCompeteMonthAwardType mAwardType;
}
