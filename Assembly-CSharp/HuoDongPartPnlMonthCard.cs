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

public class HuoDongPartPnlMonthCard : UserControl
{
	private void InitPrefabs()
	{
		this.RemainTimes.X = -100.0;
		this.RemainTimes.text = string.Empty;
		if (this.GetAwardBtn != null)
		{
			this.GetAwardBtn.Text = Global.GetLang("领取");
			this.GetAwardBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				Super.ShowNetWaiting(string.Empty);
				GameInstance.Game.SpriteGetMonthCardAwardCard(this.yueKaData.CurrDay);
			};
		}
		if (this.ReChargeBtn != null)
		{
			this.ReChargeBtn.Text = Global.GetLang("充值");
			this.ReChargeBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			};
		}
		if (this.AwardsTitle != null)
		{
			this.AwardsTitle.text = Global.GetLang("奖励内容");
			this.AwardsTitle.color = NGUIMath.HexToColorEx(16434701U);
		}
		if (this.ScrollView != null)
		{
		}
		this.MonthCards = this.MonthCardDayList.ItemsSource;
		if (this.GetRangeCtrl != null)
		{
			this.GetRangeCtrl.cam = Global.UICamera;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.HaveInit = false;
		this.InitPrefabs();
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.SpriteMonthCardInfo();
	}

	private void OnEnable()
	{
		if (this.HaveInit)
		{
			Super.ShowNetWaiting(string.Empty);
			GameInstance.Game.SpriteMonthCardInfo();
		}
	}

	public void SetData(YueKaData awardData)
	{
		this.yueKaData = awardData;
		this.RemainTimes.text = string.Format(Global.GetLang("（剩余月卡天数：{0}天）"), this.yueKaData.RemainDay);
		if (base.gameObject.active && base.gameObject.activeInHierarchy)
		{
			if (!this.HaveInit)
			{
				this.InitAwardList();
			}
			this.SetAwardListState();
		}
	}

	private void InitAwardList()
	{
		this.MonthCards.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/Activity/Card.Xml");
		if (gameResXml == null)
		{
			MUDebug.LogWarning<string>(new string[]
			{
				"Card XML IS NULL"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Card");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			MonthCardAwardDayBtn monthCardAwardDayBtn = U3DUtils.NEW<MonthCardAwardDayBtn>();
			monthCardAwardDayBtn.DiamondNum = Global.GetXElementAttributeStr(xelement, "BandZuanShiAward");
			monthCardAwardDayBtn.GoodsOne = Global.GetXElementAttributeStr(xelement, "GoodsOne");
			monthCardAwardDayBtn.Day = Global.GetXElementAttributeStr(xelement, "Day");
			monthCardAwardDayBtn.GoodsTwo = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
			this.MonthAwardBtns.Add(Global.SafeConvertToInt32(monthCardAwardDayBtn.Day), monthCardAwardDayBtn);
			monthCardAwardDayBtn.SetDayLabel(Global.SafeConvertToInt32(monthCardAwardDayBtn.Day));
			monthCardAwardDayBtn.PassIcon.SetActive(false);
			monthCardAwardDayBtn.PassMask.SetActive(false);
			GGoodIcon ggoodIcon = this.SetGoodIcon(monthCardAwardDayBtn, monthCardAwardDayBtn.GetGoodsData(MonthCardAwardDayBtn.GoodsType.GoodsOne));
			U3DUtils.AddChild(monthCardAwardDayBtn.gameObject, ggoodIcon.gameObject, true);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			monthCardAwardDayBtn.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			this.MonthCards.AddNoUpdate(monthCardAwardDayBtn.gameObject);
			this.MonthCards.DelayUpdate();
			monthCardAwardDayBtn.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			UIPanel component = monthCardAwardDayBtn.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		this.HaveInit = true;
	}

	private GGoodIcon SetGoodIcon(MonthCardAwardDayBtn target, GoodsData goodsData)
	{
		if (target.AwardItemBtn == null)
		{
			GGoodIcon ggoodIcon;
			if (goodsData != null)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				int categoriy = goodsXmlNodeByID.Categoriy;
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
				NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
				ggoodIcon.BackgroundSprite0.MakePixelPerfect();
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsData.GoodsID;
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
				ggoodIcon.gameObject.transform.localPosition = new Vector3(0f, 0f, -0.1f);
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
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			target.AwardItemBtn = ggoodIcon;
			return ggoodIcon;
		}
		bool canUse2 = Global.CanUseGoods(goodsData.GoodsID, false, true);
		Super.InitGoodsGIcon(target.AwardItemBtn, goodsData, canUse2, IconTextTypes.Qianghua);
		return target.AwardItemBtn;
	}

	private void SetCurrDayTexiao(GameObject target, bool showObj)
	{
		if (this.yueKaTex == null)
		{
			this.yueKaTex = (Object.Instantiate(Resources.Load("UITeXiao/YueKaJieMian/YueKa_LingQu_effect")) as GameObject);
			this.yueKaTex.transform.localPosition = new Vector3(0f, 0f, -3f);
		}
		this.yueKaTex.SetActive(showObj);
		U3DUtils.AddChild(target, this.yueKaTex, true);
		this.GetRangeCtrl.BoxChip(this.yueKaTex.transform.GetChild(0).gameObject);
		this.GetRangeCtrl.BoxChip(this.yueKaTex.transform.GetChild(1).GetChild(0).GetChild(0).gameObject);
	}

	private void RemoveCurrDayTexiao()
	{
		if (this.yueKaTex != null)
		{
			Object.Destroy(this.yueKaTex);
		}
		this.yueKaTex = null;
	}

	private void SetAwardListState()
	{
		if (this.yueKaData.HasYueKa)
		{
			this.ReChargeBtn.gameObject.SetActive(false);
			this.GetAwardBtn.gameObject.SetActive(true);
			this.AwardItem1.SetGoodsData(this.MonthAwardBtns[this.yueKaData.CurrDay].GetGoodsData(MonthCardAwardDayBtn.GoodsType.GoodsOne));
			this.AwardItem2.SetDiamondNum(this.MonthAwardBtns[this.yueKaData.CurrDay].DiamondNum);
			this.AwardItem1.ItemIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			this.CurrDay = this.yueKaData.CurrDay;
			char[] array = this.yueKaData.AwardInfo.ToCharArray();
			for (int i = 1; i < array.Length; i++)
			{
				int state = Global.SafeConvertToInt32(array[i - 1].ToString());
				this.MonthAwardBtns[i].SetState(state);
			}
			for (int j = array.Length + 1; j <= this.MonthAwardBtns.Values.Count; j++)
			{
				this.MonthAwardBtns[j].SetState(2);
			}
			int num = Global.SafeConvertToInt32(array[this.yueKaData.CurrDay - 1].ToString());
			if (num == 1)
			{
				this.MonthAwardBtns[this.yueKaData.CurrDay].SetState(num);
				this.SetCurrDayTexiao(this.MonthAwardBtns[this.yueKaData.CurrDay].gameObject, true);
				this.GetAwardBtn.isEnabled = false;
			}
			else
			{
				this.MonthAwardBtns[this.yueKaData.CurrDay].SetState(2);
				this.GetAwardBtn.isEnabled = true;
				this.SetCurrDayTexiao(this.MonthAwardBtns[this.yueKaData.CurrDay].gameObject, true);
			}
		}
		else
		{
			this.AwardItem1.SetGoodsData(this.MonthAwardBtns[1].GetGoodsData(MonthCardAwardDayBtn.GoodsType.GoodsOne));
			this.AwardItem2.SetDiamondNum(this.MonthAwardBtns[1].DiamondNum);
			this.AwardItem1.ItemIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			this.ReChargeBtn.gameObject.SetActive(true);
			this.GetAwardBtn.gameObject.SetActive(false);
			for (int k = 1; k <= this.MonthAwardBtns.Values.Count; k++)
			{
				this.MonthAwardBtns[k].SetState(2);
			}
			this.CurrDay = 1;
			this.SetCurrDayTexiao(this.MonthAwardBtns[1].gameObject, true);
		}
	}

	public void SetCurrDayState(int state)
	{
		this.GetAwardBtn.isEnabled = false;
		if (this.yueKaData.CurrDay == this.yueKaData.AwardInfo.Length)
		{
			this.yueKaData.AwardInfo = this.yueKaData.AwardInfo.Substring(0, this.yueKaData.AwardInfo.Length - 1) + state;
			this.MonthAwardBtns[this.yueKaData.CurrDay].SetState(state);
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			ggoodIcon = evt.target.GetComponent<MonthCardAwardDayBtn>().AwardItemBtn;
		}
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

	private void SetShowAwards(GoodsData showData, string diamondNum)
	{
		if (this.AwardItem1 != null)
		{
			this.AwardItem1.SetGoodsData(showData);
		}
	}

	public ListBox MonthCardDayList = new ListBox();

	public GButton GetAwardBtn;

	public GButton ReChargeBtn;

	public UIDraggablePanel ScrollView;

	public UIPanel ClipPanel;

	public UILabel AwardsTitle;

	public MonthCardAwardItem AwardItem1;

	public MonthCardAwardItem AwardItem2;

	public GetRange GetRangeCtrl;

	public TextBlock RemainTimes;

	private ObservableCollection MonthCards;

	private YueKaData yueKaData;

	private bool HaveInit;

	private GameObject yueKaTex;

	private Dictionary<int, MonthCardAwardDayBtn> MonthAwardBtns = new Dictionary<int, MonthCardAwardDayBtn>();

	private int CurrDay;
}
