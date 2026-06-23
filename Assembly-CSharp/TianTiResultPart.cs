using System;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TianTiResultPart : UserControl
{
	private void InitTextInPrefabs()
	{
		if (null != this.staticText)
		{
			this.staticText.text = Global.GetLang("物品奖励");
		}
		this.m_ConfirmBtn.Text = Global.GetLang("离开");
		if (this.ConstTexts != null && this.ConstTexts.Length == 2)
		{
			this.ConstTexts[0].Text = Global.GetLang("奖励荣耀:");
			this.ConstTexts[1].Text = Global.GetLang("奖励积分:");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		this.m_ConfirmBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			Object.Destroy(base.gameObject);
		};
		base.InvokeRepeating("TimeProc", 0f, 1f);
	}

	protected void TimeProc()
	{
		if (this.countDown < 0)
		{
			base.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
		}
		this.m_TimeLabel.Text = StringUtil.substitute("{0}" + Global.GetLang("秒后关闭"), new object[]
		{
			this.countDown
		});
		this.countDown--;
	}

	public void InnitData(TianTiAwardsData _TianTiAwardsData)
	{
		if (_TianTiAwardsData.Success == 0)
		{
			this.m_TitleLabel.Text = Global.GetLang("竞技场战斗失败");
			this.m_TitleLabel.textColor = 16711680U;
			this.AnimLose.gameObject.SetActive(true);
			this.m_JingjiJLabel.text = Global.GetLang("扣除积分") + ":" + (Math.Abs(_TianTiAwardsData.DuanWeiJiFen) + _TianTiAwardsData.LianShengJiFen);
			this.m_JingliSLabel.text = Global.GetLang("奖励荣耀") + ":" + _TianTiAwardsData.RongYao;
		}
		else if (_TianTiAwardsData.Success == 1)
		{
			this.m_TitleLabel.Text = Global.GetLang("竞技场战斗胜利");
			this.m_TitleLabel.textColor = 16381698U;
			this.AnimWin.gameObject.SetActive(true);
			this.m_JingjiJLabel.text = Global.GetLang("奖励积分") + ":" + (Math.Abs(_TianTiAwardsData.DuanWeiJiFen) + _TianTiAwardsData.LianShengJiFen);
			this.m_JingliSLabel.text = Global.GetLang("奖励荣耀") + ":" + _TianTiAwardsData.RongYao;
		}
		this.m_LabelPaiming.text = Global.GetLang("段位") + ":" + TianTiArenaPart.getDuanWeiByID(_TianTiAwardsData.DuanWeiId + string.Empty);
	}

	public void InnitData(FuBenTongGuanData fuBenTongGuanData)
	{
		if (fuBenTongGuanData.ResultMark == 0)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		this.m_GoodsP.gameObject.SetActive(true);
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/FuBenMap.xml", new object[0]));
		XElement xelement = Enumerable.ToList<XElement>(gameResXml.Elements("Copy")).Find((XElement s) => s.AttributeStr("MapCode") == Global.Data.roleData.MapCode.ToString());
		GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(xelement.AttributeStr("GoodsIDs").Split(new char[]
		{
			','
		}), 1);
		goodsItemIconEx.transform.parent = this.m_GoodsP;
		goodsItemIconEx.transform.localScale = Vector3.one * 0.9f;
		goodsItemIconEx.transform.localPosition = Vector3.zero;
		this.m_GoodCount.text = ((int)(fuBenTongGuanData.AwardRate * (double)int.Parse(xelement.AttributeStr("GoodsIDs").Split(new char[]
		{
			','
		})[1]))).ToString();
	}

	public void InnitData(ElementWarAwardsData result)
	{
		this.AnimWin.gameObject.SetActive(true);
		this.AnimLose.gameObject.SetActive(false);
		this.m_JingjiJLabel.text = Global.GetLang("经验") + ":" + result.Exp;
		if (result.ysfm > 0)
		{
			this.m_JingliSLabel.text = Global.GetLang("元素粉末") + ":" + result.ysfm;
		}
		else
		{
			this.m_LabelJinBi.transform.localPosition = this.m_JingliSLabel.transform.localPosition;
		}
		this.m_LabelPaiming.text = string.Format(Global.GetLang("达成{0}层元素试炼"), result.Wave);
		this.m_LabelJinBi.text = Global.GetLang("金币") + ":" + result.Money;
	}

	public void InnitData(CopyWolfAwardsData result)
	{
		if (ZuduiFuBen_LangHunYaoSai.FortLifeNow > 0)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		this.m_JingliSLabel.Text = string.Format(string.Concat(new string[]
		{
			Global.GetLang("狼魂积分:{0}"),
			Environment.NewLine,
			Global.GetLang("奖励经验:{1}"),
			Environment.NewLine,
			Global.GetLang("狼魂粉末:{2}")
		}), result.RoleScore, result.Exp, result.WolfMoney);
		if (result.Money > 0)
		{
			this.m_JingjiJLabel.Text = string.Format(Global.GetLang("奖励金币:{0}"), result.Money);
		}
		this.m_LabelPaiming.text = string.Empty;
		this.m_LabelJinBi.text = string.Empty;
	}

	private GGoodIcon GetGoodsItemIconEx(string[] goods, int count = 0)
	{
		GoodsData goodsData = new GoodsData();
		goodsData.GoodsID = int.Parse(goods[0]);
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			ggoodIcon.SecondText.Text = goods[1];
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
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

	public GButton m_ConfirmBtn;

	public TextBlock m_JingyanLabel;

	public TextBlock m_TimeLabel;

	public TextBlock m_ShengwangLabel;

	public TextBlock m_TitleLabel;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock m_LabelPaiming;

	public TextBlock m_JingjiJLabel;

	public TextBlock m_LabelJinBi;

	public TextBlock m_LabelYuanSu;

	public TextBlock m_JingliSLabel;

	public UILabel lblPaiming;

	public Animator AnimWin;

	public Animator AnimLose;

	public TextBlock[] ConstTexts;

	public Transform m_GoodsP;

	public Transform m_Goods;

	public UILabel m_GoodCount;

	public Transform bak;

	private int countDown = 10;

	public UILabel staticText;
}
