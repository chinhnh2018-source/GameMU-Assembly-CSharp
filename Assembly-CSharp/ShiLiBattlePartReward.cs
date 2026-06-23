using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartReward : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("奖励排行");
		this.lblTitlePaiMing.text = Global.GetLang("排名");
		this.lblReward.text = Global.GetLang("争霸奖励");
		this.lblJunXian.text = Global.GetLang("军衔");
		this.lblGongXian.text = Global.GetLang("贡献度");
		this.lblMyPaiHangWord.text = Global.GetLang("我的排行:");
		this.lblMyJiFenWord.text = Global.GetLang("我的积分:");
		this.lblCityNumWord.text = Global.GetLang("争霸占领城市:");
		this.btnLingQu.Text = Global.GetLang("领  奖");
		this.lblMyPaiHangWord.pivot = 5;
		this.lblMyPaiHangWord.transform.localPosition = new Vector3(120f, -172f, this.lblMyPaiHangWord.transform.localPosition.z);
		this.lblMyJiFenWord.pivot = 5;
		this.lblMyJiFenWord.transform.localPosition = new Vector3(120f, -195f, this.lblMyJiFenWord.transform.localPosition.z);
		this.lblMyPaiHang.pivot = 3;
		this.lblMyPaiHang.transform.localPosition = new Vector3(120f, -172f, this.lblMyPaiHang.transform.localPosition.z);
		this.lblMyJiFen.pivot = 3;
		this.lblMyJiFen.transform.localPosition = new Vector3(120f, -195f, this.lblMyJiFen.transform.localPosition.z);
		this.lblCityNum.pivot = 3;
		this.lblCityNum.transform.localPosition = new Vector3(-165f, -172f, this.lblCityNum.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mShiLiTypeEX == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle)
			{
				this.SendCompBattleAwardGet();
			}
			else if (this.mShiLiTypeEX == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiMiDong)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendShiLiGetMiDongAwardGet();
			}
		};
		this.btnLingQu.gameObject.SetActive(false);
	}

	public void InitInfos(ShiLiBattlePartReward.ShiLiTypeEX shiLiType = ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle)
	{
		if (null != this._HelpLabel)
		{
			this._HelpLabel.text = string.Empty;
		}
		this.mShiLiTypeEX = shiLiType;
		List<MUForceCraftReward> list = null;
		if (shiLiType == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle)
		{
			list = ShiLiData.GetAllCraftReward().ForceCraftRewards;
		}
		else if (shiLiType == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiMiDong)
		{
			this.lblCityNum.text = string.Empty;
			this.lblMyJiFen.text = string.Empty;
			this.lblMyPaiHang.text = string.Empty;
			this.lblMyPaiHangWord.text = string.Empty;
			this.lblMyJiFenWord.text = string.Empty;
			list = IConfigbase<ConfigShiLiMiDong>.Instance.GetRewardAllAllData().ForceCraftRewards;
			this.lblCityNumWord.text = string.Empty;
			if (null == this._HelpLabel)
			{
				this._HelpLabel = NGUITools.AddWidget<UILabel>(base.gameObject);
				this._HelpLabel.font = this.lblTitle.font;
			}
			this._HelpLabel.transform.localScale = Vector3.one * 16f;
			this._HelpLabel.transform.localPosition = new Vector3(0f, -170f, -1f);
			this._HelpLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ddd2bd",
				Global.GetLang("按势力运输资源排名奖励会变化，详情见活动帮助")
			});
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				ShiLiBattlePartRewardItem shiLiBattlePartRewardItem = U3DUtils.NEW<ShiLiBattlePartRewardItem>();
				shiLiBattlePartRewardItem.gameObject.name = "item" + i;
				shiLiBattlePartRewardItem.transform.SetParent(this.gridRewardItems.transform);
				shiLiBattlePartRewardItem.transform.localScale = Vector3.one;
				shiLiBattlePartRewardItem.transform.localPosition = Vector3.zero;
				shiLiBattlePartRewardItem.InitInfo(list[i]);
				this.m_lstRewardItem.Add(shiLiBattlePartRewardItem);
			}
			this.gridRewardItems.Reposition();
		}
		this.SendCompBattleAward(shiLiType);
	}

	private void InitSelfInfo(CompBattleAwardsData award)
	{
		this.lblMyJiFen.text = Global.Data.roleData.MoneyData[143].ToString();
		this.lblCityNum.text = award.WinNum.ToString();
		if (award.RankNum <= 0)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
			this.btnLingQu.isEnabled = false;
			this.m_myCraftReward = null;
			return;
		}
		this.m_myCraftReward = ShiLiData.GetAllCraftReward().GetForceCraftRewardByID(award.AwardID);
		this.m_awardData = award;
		if (this.m_myCraftReward == null)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
		}
		else if (this.m_myCraftReward.Rank < 0)
		{
			this.lblMyPaiHang.text = Global.GetLang("前") + Mathf.RoundToInt(this.m_myCraftReward.RankRate * 100f) + "%";
		}
		else
		{
			this.lblMyPaiHang.text = this.m_myCraftReward.Rank.ToString();
		}
		ShiLiBattlePartRewardItem rewardItemById = this.GetRewardItemById(award.AwardID);
		if (rewardItemById != null)
		{
			rewardItemById.SetSelected(true);
		}
	}

	private ShiLiBattlePartRewardItem GetRewardItemById(int id)
	{
		return this.m_lstRewardItem.Find((ShiLiBattlePartRewardItem info) => info.CraftReward.ID == id);
	}

	private void SetBattleState(CompBattleGameStates state)
	{
		if (this.m_myCraftReward == null)
		{
			this.btnLingQu.gameObject.SetActive(false);
			return;
		}
		if (state == CompBattleGameStates.Awards)
		{
			this.btnLingQu.gameObject.SetActive(true);
		}
		else
		{
			this.btnLingQu.gameObject.SetActive(false);
		}
	}

	private void OnOpenAwardWindow(List<GoodsData> goodsData)
	{
		if (this.m_awardWindow == null)
		{
			this.m_awardWindow = U3DUtils.NEW<GChildWindow>();
			this.m_awardWindow.IsShowModal = true;
			this.m_awardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_awardWindow, Global.GetLang("领取奖励"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_awardWindow);
		}
		if (this.m_awardPart == null)
		{
			this.m_awardPart = U3DUtils.NEW<ShiLiAwardGoodsTips>();
			this.m_awardPart.ShowGoodsIcon(goodsData);
			string contentInfo = string.Format(Global.GetLang("本次活动中{0}势力获得了{1}场势力争霸战的胜利，你在活动中获得了{2}的积分，奖励如下"), Global.GetColorStringForNGUIText(new object[]
			{
				"FAC60D",
				ShiLiData.GetShiLiNameByType((ShiLiType)ShiLiData.GetSelfCompType())
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"FAC60D",
				this.m_awardData.WinNum
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"FAC60D",
				Global.Data.roleData.MoneyData[143]
			}));
			this.m_awardPart.SetContentInfo(contentInfo);
			this.m_awardPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAwardWindow();
			};
		}
		this.m_awardWindow.SetContent(this.m_awardWindow.BodyPresenter, this.m_awardPart, 0.0, 0.0, true);
		this.m_awardWindow.transform.localPosition = new Vector3(0f, 0f, -5000f);
	}

	private void CloseAwardWindow()
	{
		if (null != this.m_awardPart)
		{
			this.m_awardPart.transform.parent = null;
			Object.Destroy(this.m_awardPart.gameObject);
			this.m_awardPart = null;
		}
		if (null != this.m_awardWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_awardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_awardWindow, true);
			this.m_awardWindow = null;
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
		MUEventManager.AddEventListener<CompBattleAwardsData>("CMD_SPR_COMP_BATTLE_AWARD", new Action<CompBattleAwardsData>(this.ServerBattleAward));
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_BATTLE_AWARD_GET", new Action<int>(this.ServerBattleAwardGet));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
		MUEventManager.RemoveEventListener<CompBattleAwardsData>("CMD_SPR_COMP_BATTLE_AWARD", new Action<CompBattleAwardsData>(this.ServerBattleAward));
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_BATTLE_AWARD_GET", new Action<int>(this.ServerBattleAwardGet));
	}

	private void SendCompBattleAward(ShiLiBattlePartReward.ShiLiTypeEX shiLiType = ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle)
	{
		Super.ShowNetWaiting(null);
		if (shiLiType == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle)
		{
			GameInstance.Game.ShiLiGetCompBattleAward();
		}
		else if (shiLiType == ShiLiBattlePartReward.ShiLiTypeEX.ShiLiMiDong)
		{
			GameInstance.Game.SendShiLiGetCompMiDongStates();
		}
	}

	private void ServerBattleAward(CompBattleAwardsData Award)
	{
		this.InitSelfInfo(Award);
		this.SendGetBattleState();
	}

	private void SendGetBattleState()
	{
		GameInstance.Game.ShiLiGetCompBattleState();
		Super.ShowNetWaiting(null);
	}

	private void ServerGetBattleState(CompBattleGameStates state)
	{
		this.SetBattleState(state);
	}

	private void SendCompBattleAwardGet()
	{
		GameInstance.Game.ShiLiGetCompBattleAwardGet();
	}

	private void ServerBattleAwardGet(int ret)
	{
		if (ret >= 0 && this.m_myCraftReward != null)
		{
			this.btnLingQu.gameObject.SetActive(false);
			GameInstance.Game.ShiLiGetCompBattleState();
			float craftRewardRate = ShiLiData.GetCraftRewardRate(this.m_awardData.WinNum);
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < this.m_myCraftReward.LstGoodsOne.Count; i++)
			{
				GoodsData goodsDataByStr = Global.GetGoodsDataByStr(this.m_myCraftReward.LstGoodsOne[i], 0);
				if (goodsDataByStr != null)
				{
					goodsDataByStr.GCount = (int)((float)goodsDataByStr.GCount * craftRewardRate);
					list.Add(goodsDataByStr);
				}
			}
			for (int j = 0; j < this.m_myCraftReward.LstGoodsTwo.Count; j++)
			{
				GoodsData goodsDataByStr2 = Global.GetGoodsDataByStr(this.m_myCraftReward.LstGoodsTwo[j], 0);
				if (goodsDataByStr2 != null)
				{
					goodsDataByStr2.GCount = (int)((float)goodsDataByStr2.GCount * craftRewardRate);
					list.Add(goodsDataByStr2);
				}
			}
			this.OnOpenAwardWindow(list);
		}
	}

	internal void NoticeGetStatesCallBask(int states)
	{
		if (states == 2)
		{
			this.btnLingQu.gameObject.SetActive(true);
			this._HelpLabel.transform.localPosition = new Vector3(0f, -210f, -1f);
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendShiLiGetCompMiDongAwardInf();
		}
		else
		{
			this.lblCityNum.text = string.Empty;
			this.lblMyJiFen.text = string.Empty;
			this.lblMyPaiHang.text = string.Empty;
			this.lblMyPaiHangWord.text = string.Empty;
			this.lblMyJiFenWord.text = string.Empty;
			this.btnLingQu.gameObject.SetActive(false);
			this._HelpLabel.transform.localPosition = new Vector3(0f, -170f, -1f);
		}
	}

	internal void NoticeGetBaseAwardDataCallBack(CompMineAwardsData award)
	{
		this.lblMyJiFen.text = Global.Data.roleData.MoneyData[145].ToString();
		this.lblCityNum.text = string.Empty;
		if (award == null || award.RankNum <= 0)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
			this.btnLingQu.isEnabled = false;
			this.m_myCraftReward = null;
			return;
		}
		this.lblMyPaiHangWord.text = Global.GetLang("我的排行:");
		this.lblMyJiFenWord.text = Global.GetLang("我的积分:");
		this.m_myCraftReward = ShiLiData.GetAllCraftReward().GetForceCraftRewardByID(award.AwardID);
		this.m_CompMineAwardsData = award;
		if (this.m_myCraftReward == null)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
		}
		else if (this.m_myCraftReward.Rank < 0)
		{
			this.lblMyPaiHang.text = Global.GetLang("前") + Mathf.RoundToInt(this.m_myCraftReward.RankRate * 100f) + "%";
		}
		else
		{
			this.lblMyPaiHang.text = this.m_myCraftReward.Rank.ToString();
		}
		ShiLiBattlePartRewardItem rewardItemById = this.GetRewardItemById(award.AwardID);
		if (rewardItemById != null)
		{
			rewardItemById.SetSelected(true);
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTitlePaiMing;

	public UILabel lblReward;

	public UILabel lblJunXian;

	public UILabel lblGongXian;

	public UILabel lblMyPaiHangWord;

	public UILabel lblMyJiFenWord;

	public UILabel lblMyPaiHang;

	public UILabel lblMyJiFen;

	public UILabel lblCityNum;

	public UILabel lblCityNumWord;

	public GButton btnClose;

	public GButton btnLingQu;

	public UIGrid gridRewardItems;

	private UILabel _HelpLabel;

	private MUForceCraftReward m_myCraftReward;

	private CompBattleAwardsData m_awardData;

	private CompMineAwardsData m_CompMineAwardsData;

	private List<ShiLiBattlePartRewardItem> m_lstRewardItem = new List<ShiLiBattlePartRewardItem>();

	private ShiLiBattlePartReward.ShiLiTypeEX mShiLiTypeEX;

	protected GChildWindow m_awardWindow;

	protected ShiLiAwardGoodsTips m_awardPart;

	public enum ShiLiTypeEX
	{
		ShiLiBattle,
		ShiLiMiDong
	}
}
