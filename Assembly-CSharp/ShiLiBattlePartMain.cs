using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartMain : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblInfoTitle.text = Global.GetLang("领地占领情况");
		this.lblJiaoTuan.text = Global.GetLang("神圣教团:");
		this.lblTongMeng.text = Global.GetLang("自由同盟:");
		this.lblXieHui.text = Global.GetLang("自由协会:");
		this.lblRewardTitle.text = Global.GetLang("奖励预览");
		this.btnReward.Text = Global.GetLang("领取奖励");
		Global.SetSprite(base.gameObject);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			ShiLiBattlePartMain.OpenHelpWindow();
		};
		this.btnReward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenRewardWindow();
		};
		for (int i = 0; i < this.lstCity.Count; i++)
		{
			this.lstCity[i].OnSelectCity = new Action<ShiLiBattlePartCity>(this.OnSelectCity);
		}
		this.LoadRewards();
		this.SendGetBattleData();
	}

	private void OnSelectCity(ShiLiBattlePartCity city)
	{
		if (city.CityInfo != null)
		{
			this.OpenCityInfoWindow(city.CityInfo);
		}
	}

	private void SetShiLiNum(ShiLiType type, int num)
	{
		if (type == ShiLiType.None)
		{
			return;
		}
		List<GameObject> list = null;
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			list = this.lstJiaoTuan;
			if (num == 0)
			{
				this.lblJiaoTuan.text = Global.GetLang("神圣教团:未占领城池");
			}
			else
			{
				this.lblJiaoTuan.text = Global.GetLang("神圣教团:");
			}
			break;
		case ShiLiType.ZiYouTongMeng:
			list = this.lstTongMeng;
			if (num == 0)
			{
				this.lblTongMeng.text = Global.GetLang("自由同盟:未占领城池");
			}
			else
			{
				this.lblTongMeng.text = Global.GetLang("自由同盟:");
			}
			break;
		case ShiLiType.ZhiMengXieHui:
			list = this.lstXieHui;
			if (num == 0)
			{
				this.lblXieHui.text = Global.GetLang("织梦协会:未占领城池");
			}
			else
			{
				this.lblXieHui.text = Global.GetLang("织梦协会:");
			}
			break;
		}
		for (int i = 0; i < 5; i++)
		{
			if (i < num)
			{
				list[i].gameObject.SetActive(true);
			}
			else
			{
				list[i].gameObject.SetActive(false);
			}
		}
	}

	private ShiLiBattlePartCity GetCityById(int cityId)
	{
		return this.lstCity.Find((ShiLiBattlePartCity info) => info.cityID == cityId);
	}

	private void SetBattleData(CompBattleBaseData battleData)
	{
		if (battleData == null)
		{
			return;
		}
		if (battleData.CompBattleOwnCityList == null)
		{
			return;
		}
		for (int i = 0; i < battleData.CompBattleOwnCityList.Count; i++)
		{
			ShiLiType shiLiType = i + ShiLiType.ShenShengJiaoTuan;
			if (battleData.CompBattleOwnCityList[i].OwnCityList == null)
			{
				this.SetShiLiNum(shiLiType, 0);
			}
			else
			{
				for (int j = 0; j < battleData.CompBattleOwnCityList[i].OwnCityList.Count; j++)
				{
					ShiLiBattlePartCity cityById = this.GetCityById(battleData.CompBattleOwnCityList[i].OwnCityList[j]);
					if (cityById != null)
					{
						cityById.ShiLiType = shiLiType;
					}
				}
				this.SetShiLiNum(shiLiType, battleData.CompBattleOwnCityList[i].OwnCityList.Count);
			}
		}
	}

	private void SetBattleState(CompBattleGameStates state)
	{
		if (state == CompBattleGameStates.Awards)
		{
			this.btnReward.Text = Global.GetLang("领取奖励");
			this.btnReward.isEnabled = true;
		}
		else if (state == CompBattleGameStates.Analysis)
		{
			this.btnReward.Text = Global.GetLang("结算中");
			this.btnReward.isEnabled = false;
			base.StartCoroutine<bool>(this.ReGetState());
		}
		else
		{
			this.btnReward.Text = Global.GetLang("奖励预览");
			this.btnReward.isEnabled = true;
		}
		if (state == CompBattleGameStates.Start)
		{
			this.SetAllCityState(true);
		}
		else
		{
			this.SetAllCityState(false);
		}
	}

	private IEnumerator ReGetState()
	{
		yield return new WaitForSeconds(5f);
		GameInstance.Game.ShiLiGetCompBattleState();
		yield break;
	}

	private void SetAllCityState(bool beInBattle)
	{
		for (int i = 0; i < this.lstCity.Count; i++)
		{
			ShiLiBattlePartCity shiLiBattlePartCity = this.lstCity[i];
			shiLiBattlePartCity.BeInBattle = beInBattle;
		}
	}

	private void LoadRewards()
	{
		int[] craftReward = ShiLiData.GetCraftReward();
		for (int i = 0; i < craftReward.Length; i++)
		{
			GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(craftReward[i], true);
			ggoodIcon.transform.SetParent(this.gridReward.transform);
			ggoodIcon.transform.localScale = new Vector3(1f, 1f, 1f);
			ggoodIcon.transform.localPosition = Vector3.zero;
		}
		this.gridReward.Reposition();
		float num = (float)(3 - craftReward.Length) * this.gridReward.cellWidth / 2f;
		this.gridReward.transform.localPosition = new Vector3(num, 0f, 0f);
	}

	private void OnOpenRewardWindow()
	{
		if (this.m_rewardWindow == null)
		{
			this.m_rewardWindow = U3DUtils.NEW<GChildWindow>();
			this.m_rewardWindow.IsShowModal = true;
			this.m_rewardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_rewardWindow, Global.GetLang("排行"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_rewardWindow);
		}
		if (this.m_rewardPart == null)
		{
			this.m_rewardPart = U3DUtils.NEW<ShiLiBattlePartReward>();
			this.m_rewardPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRewardWindow();
			};
			this.m_rewardPart.InitInfos(ShiLiBattlePartReward.ShiLiTypeEX.ShiLiBattle);
		}
		this.m_rewardWindow.SetContent(this.m_rewardWindow.BodyPresenter, this.m_rewardPart, 0.0, 0.0, true);
	}

	private void CloseRewardWindow()
	{
		if (null != this.m_rewardPart)
		{
			this.m_rewardPart.transform.parent = null;
			Object.Destroy(this.m_rewardPart.gameObject);
			this.m_rewardPart = null;
		}
		if (null != this.m_rewardWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_rewardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_rewardWindow, true);
			this.m_rewardWindow = null;
		}
	}

	private void OpenCityInfoWindow(MUForceCraft cityInfo)
	{
		if (this.m_cityInfoWindow == null)
		{
			this.m_cityInfoWindow = U3DUtils.NEW<GChildWindow>();
			this.m_cityInfoWindow.IsShowModal = true;
			this.m_cityInfoWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_cityInfoWindow, Global.GetLang("城池详情"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_cityInfoWindow);
		}
		if (this.m_cityInfoPart == null)
		{
			this.m_cityInfoPart = U3DUtils.NEW<ShiLiBattlePartCityDeatil>();
			this.m_cityInfoPart.SetmCityInfo(cityInfo);
			this.m_cityInfoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseCityInfoWindow();
			};
		}
		this.m_cityInfoWindow.SetContent(this.m_cityInfoWindow.BodyPresenter, this.m_cityInfoPart, 0.0, 0.0, true);
	}

	private void CloseCityInfoWindow()
	{
		if (null != this.m_cityInfoPart)
		{
			this.m_cityInfoPart.transform.parent = null;
			Object.Destroy(this.m_cityInfoPart.gameObject);
			this.m_cityInfoPart = null;
		}
		if (null != this.m_cityInfoWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_cityInfoWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_cityInfoWindow, true);
			this.m_cityInfoWindow = null;
		}
	}

	public static void OpenHelpWindow()
	{
		ChangeableRulePart.RuleXml compBattleHelpData = ShiLiData.GetCompBattleHelpData();
		if (compBattleHelpData == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到相关配置"
			});
			return;
		}
		if (ShiLiBattlePartMain.m_helpWindow == null)
		{
			ShiLiBattlePartMain.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiBattlePartMain.m_helpWindow.IsShowModal = true;
			ShiLiBattlePartMain.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiBattlePartMain.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiBattlePartMain.m_helpWindow);
		}
		if (ShiLiBattlePartMain.m_helpPart == null)
		{
			ShiLiBattlePartMain.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			ShiLiBattlePartMain.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiBattlePartMain.CloseHelpWindow();
			};
		}
		ShiLiBattlePartMain.m_helpWindow.SetContent(ShiLiBattlePartMain.m_helpWindow.BodyPresenter, ShiLiBattlePartMain.m_helpPart, 0.0, 0.0, true);
		ShiLiBattlePartMain.m_helpPart.SetHelpInfo(compBattleHelpData.list);
	}

	private static void CloseHelpWindow()
	{
		if (null != ShiLiBattlePartMain.m_helpPart)
		{
			ShiLiBattlePartMain.m_helpPart.transform.parent = null;
			Object.Destroy(ShiLiBattlePartMain.m_helpPart.gameObject);
			ShiLiBattlePartMain.m_helpPart = null;
		}
		if (null != ShiLiBattlePartMain.m_helpWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShiLiBattlePartMain.m_helpWindow);
			ShiLiBattlePartMain.m_helpWindow = null;
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
		MUEventManager.AddEventListener<CompBattleBaseData>("CMD_SPR_COMP_BATTLE_BASE_DATA", new Action<CompBattleBaseData>(this.ServerGetBattleData));
		MUEventManager.AddEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompBattleBaseData>("CMD_SPR_COMP_BATTLE_BASE_DATA", new Action<CompBattleBaseData>(this.ServerGetBattleData));
		MUEventManager.RemoveEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
	}

	private void SendGetBattleData()
	{
		GameInstance.Game.ShiLiGetCompBattleData();
		Super.ShowNetWaiting(null);
	}

	private void SendGetBattleState()
	{
		GameInstance.Game.ShiLiGetCompBattleState();
		Super.ShowNetWaiting(null);
	}

	private void ServerGetBattleData(CompBattleBaseData battleData)
	{
		this.SetBattleData(battleData);
		this.SendGetBattleState();
	}

	private void ServerGetBattleState(CompBattleGameStates state)
	{
		this.SetBattleState(state);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton btnClose;

	public GButton btnHelp;

	public UILabel lblInfoTitle;

	public UILabel lblJiaoTuan;

	public UILabel lblTongMeng;

	public UILabel lblXieHui;

	public UILabel lblRewardTitle;

	public GButton btnReward;

	public List<GameObject> lstJiaoTuan;

	public List<GameObject> lstTongMeng;

	public List<GameObject> lstXieHui;

	public List<ShiLiBattlePartCity> lstCity;

	public UIGrid gridReward;

	protected GChildWindow m_rewardWindow;

	protected ShiLiBattlePartReward m_rewardPart;

	protected GChildWindow m_cityInfoWindow;

	protected ShiLiBattlePartCityDeatil m_cityInfoPart;

	protected static GChildWindow m_helpWindow;

	protected static CommonHelpWindow m_helpPart;
}
