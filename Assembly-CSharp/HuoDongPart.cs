using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuoDongPart : UserControl
{
	public HuoDongPart()
	{
		this.ItemCollection = this.HuoDongList.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.WindowLblTitle.text = Global.GetLang("福利");
		this.WindowLblTitle.transform.localPosition = new Vector3(0f, 235f, -0.1f);
		this.WindowLblTitle.transform.localScale = new Vector3(30f, 30f, 1f);
		this.InitBtnProc();
	}

	private void TabBtnSelectChange(object sender, object e)
	{
		if (Global.g_bIsYaoQingCeShi && this.m_ListTabBtn.SelectedIndex == 0)
		{
			Super.HintMainText(Global.GetLang("此系统暂时未开放！"), 10, 3);
			return;
		}
		if (this.m_ListTabBtn.SelectedIndex >= 0 && this.m_ListTabBtn.SelectedItem != null)
		{
			BtnObj component = this.m_ListTabBtn.SelectedItem.GetComponent<BtnObj>();
			this.SetUIState(component.SelectIndex, 0);
		}
	}

	private void InitBtnProc()
	{
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		this.m_TabBtnOBC = this.m_ListTabBtn.ItemsSource;
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("LiPinDuiHuan_Andrid", true);
		for (int i = 0; i < 9; i++)
		{
			string btnNameByIndex = this.GetBtnNameByIndex(i);
			if ((!(btnNameByIndex == Global.GetLang("禮品兌換")) && !(btnNameByIndex == Global.GetLang("礼品兑换"))) || !Context.IsAPPVerify)
			{
				if (!(btnNameByIndex == Global.GetLang("超值回馈")) || (Global.VersionCode >= 1.4f && !Context.IsAPPVerify))
				{
					if (Global.VersionCode >= 2.3f || !(btnNameByIndex == Global.GetLang("战斗力奖励")))
					{
						BtnObj btnObj = U3DUtils.NEW<BtnObj>();
						btnObj.m_BtnItem.Label.text = btnNameByIndex;
						btnObj.SelectIndex = (HuoDongPart.HuoDongType)i;
						this.BtnObjs.Add((int)btnObj.SelectIndex, btnObj);
						this.m_TabBtnOBC.AddNoUpdate(btnObj);
					}
				}
			}
		}
		if (ConfigLaoWanJiaZhaoHui.IsInPlayerRecallActivity())
		{
			BtnObj btnObj2 = U3DUtils.NEW<BtnObj>();
			btnObj2.m_BtnItem.Label.text = Global.GetLang("战友召回");
			btnObj2.Children.Add(this._ActivityTipIcons[7]);
			btnObj2.SelectIndex = HuoDongPart.HuoDongType.ZhanYouCallBack;
			this.BtnObjs.Add((int)btnObj2.SelectIndex, btnObj2);
			this.m_TabBtnOBC.AddNoUpdate(btnObj2);
			ActivityTipManager.RegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
			if (TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.PlayerRecallActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.PlayerRecallActivity])
			{
				this._ActivityTipIcons[7].SetActive(true);
			}
		}
		else
		{
			this._ActivityTipIcons[7].SetActive(false);
		}
		this.m_TabBtnOBC.DelayUpdate();
		this.m_ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnSelectChange);
		if (Global.g_bIsYaoQingCeShi)
		{
		}
		GameObject gameObject = this.BtnObjs[0].gameObject;
		BtnObj componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[0]);
		ActivityTipManager.RegActivityTipItem(3001, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[0].SetActive(e.IsActive);
		});
		if (!Context.IsAPPVerify)
		{
			gameObject = this.BtnObjs[1].gameObject;
			componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
			componentInChildren.Children.Add(this._ActivityTipIcons[1]);
			ActivityTipManager.RegActivityTipItem(3013, delegate(int s, ActivityTipItem e)
			{
				this._ActivityTipIcons[1].SetActive(e.IsActive);
			});
		}
		else
		{
			this._ActivityTipIcons[1].SetActive(false);
		}
		gameObject = this.BtnObjs[2].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[2]);
		ActivityTipManager.RegActivityTipItem(3006, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[2].SetActive(e.IsActive);
		});
		gameObject = this.BtnObjs[3].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[3]);
		ActivityTipManager.RegActivityTipItem(3010, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[3].SetActive(e.IsActive);
		});
		gameObject = this.BtnObjs[5].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[4]);
		ActivityTipManager.RegActivityTipItem(3007, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[4].SetActive(e.IsActive);
		});
		gameObject = this.BtnObjs[6].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[5]);
		ActivityTipManager.RegActivityTipItem(3008, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[5].SetActive(e.IsActive);
		});
		gameObject = this.BtnObjs[7].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[6]);
		ActivityTipManager.RegActivityTipItem(3009, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[6].SetActive(e.IsActive);
		});
		gameObject = this.BtnObjs[4].gameObject;
		componentInChildren = gameObject.GetComponentInChildren<BtnObj>();
		componentInChildren.Children.Add(this._ActivityTipIcons[8]);
		ActivityTipManager.RegActivityTipItem(3014, delegate(int s, ActivityTipItem e)
		{
			this._ActivityTipIcons[8].SetActive(e.IsActive);
		});
	}

	private new void OnDestroy()
	{
		if (null != this.m_meiRiZaiXian)
		{
			this.m_meiRiZaiXian.Destroy();
			this.m_meiRiZaiXian = null;
		}
		if (null != this.m_dengLuHaoLi)
		{
			this.m_dengLuHaoLi.Destroy();
			this.m_dengLuHaoLi = null;
		}
		if (null != this.m_LeiJiDengLu)
		{
			this.m_LeiJiDengLu.Destroy();
			this.m_LeiJiDengLu = null;
		}
		if (null != this.m_ChengJiuPart)
		{
			this.m_ChengJiuPart.Destroy();
			this.m_ChengJiuPart = null;
		}
		if (null != this.m_ChongzhiHuikuiPart)
		{
			this.m_ChongzhiHuikuiPart.Destroy();
			this.m_ChongzhiHuikuiPart = null;
		}
		if (null != this.m_HuoyuePart)
		{
			this.m_HuoyuePart.Destroy();
			this.m_HuoyuePart = null;
		}
		if (null != this.m_LiPinDuiHuanPart)
		{
			this.m_LiPinDuiHuanPart.Destroy();
			this.m_LiPinDuiHuanPart = null;
		}
		if (null != this.m_UpLevelAwardPart)
		{
			this.m_UpLevelAwardPart.Destroy();
			this.m_UpLevelAwardPart = null;
		}
		if (null != this.m_ZhanDouLiAwardPart)
		{
			this.m_ZhanDouLiAwardPart.Destroy();
			this.m_ZhanDouLiAwardPart = null;
		}
		if (ConfigLaoWanJiaZhaoHui.IsInPlayerRecallActivity())
		{
			if (null != this.playerRecall)
			{
				this.playerRecall.Destroy();
				this.playerRecall = null;
			}
			ActivityTipManager.UnRegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
		}
		if (null != this.nochongzhi)
		{
			this.nochongzhi.Destroy();
			this.nochongzhi = null;
		}
		ActivityTipManager.RegActivityTipItem(3001, null);
		ActivityTipManager.RegActivityTipItem(3008, null);
		ActivityTipManager.RegActivityTipItem(3007, null);
		ActivityTipManager.RegActivityTipItem(3006, null);
		ActivityTipManager.RegActivityTipItem(3009, null);
		ActivityTipManager.RegActivityTipItem(3010, null);
		ActivityTipManager.RegActivityTipItem(3013, null);
		ActivityTipManager.RegActivityTipItem(3014, null);
	}

	public void SetUIState(HuoDongPart.HuoDongType nIndex, int nItem = 0)
	{
		this.SetPnlActive(nIndex, nItem);
		this.SetTabBtnIndex(nIndex);
	}

	private string GetBtnNameByIndex(int nIndex)
	{
		string chineseText;
		switch (nIndex)
		{
		case 0:
			chineseText = "充值回馈";
			break;
		case 1:
			chineseText = "超值回馈";
			break;
		case 2:
			chineseText = "每日活跃";
			break;
		case 3:
			chineseText = "等级奖励";
			break;
		case 4:
			chineseText = "战斗力奖励";
			break;
		case 5:
			chineseText = "连续登录";
			break;
		case 6:
			chineseText = "累计登录";
			break;
		case 7:
			chineseText = "每日在线";
			break;
		case 8:
			chineseText = Global.GetLang("礼品兑换");
			break;
		default:
			chineseText = Global.GetLang("异常按钮");
			break;
		}
		return Global.GetLang(chineseText);
	}

	public void SetBtnActieve(GButton btn)
	{
		if (null != btn)
		{
			if (btn == this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15461355U);
				return;
			}
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnLastSelect = this.m_BtnCurrentSelect;
				this.m_BtnCurrentSelect = btn;
			}
			this.m_BtnCurrentSelect = btn;
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15461355U);
				this.m_BtnCurrentSelect.normalSprite = "chatTab_hover";
				this.m_BtnCurrentSelect.Refresh();
			}
			if (null != this.m_BtnLastSelect)
			{
				this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(8350293U);
				this.m_BtnLastSelect.normalSprite = "chatTab_normal";
				this.m_BtnLastSelect.Refresh();
			}
		}
	}

	private void SetPnlActive(HuoDongPart.HuoDongType nActivePage, int nItem = 0)
	{
		this.InitPnl(nActivePage);
		if (null != this.m_ChongzhiHuikuiPart)
		{
			this.m_ChongzhiHuikuiPart.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.ChongZhiBack);
		}
		if (null != this.m_HuoyuePart)
		{
			bool flag = nActivePage == HuoDongPart.HuoDongType.EveryDayActive;
			this.m_HuoyuePart.gameObject.SetActive(flag);
			if (flag)
			{
				GameInstance.Game.SpriteGetDailyActiveInforCmd();
			}
		}
		if (null != this.m_UpLevelAwardPart)
		{
			this.m_UpLevelAwardPart.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.LevelAward);
			if (nActivePage == HuoDongPart.HuoDongType.LevelAward)
			{
				this.m_UpLevelAwardPart.QueryServerInfo();
			}
		}
		if (null != this.m_ZhanDouLiAwardPart)
		{
			this.m_ZhanDouLiAwardPart.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.ZhanDouLiAward);
			if (nActivePage == HuoDongPart.HuoDongType.ZhanDouLiAward)
			{
				this.m_ZhanDouLiAwardPart.SendQueryAwardRequest();
			}
		}
		if (null != this.m_dengLuHaoLi)
		{
			this.m_dengLuHaoLi.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.LianXuLogin);
		}
		if (null != this.m_LeiJiDengLu)
		{
			this.m_LeiJiDengLu.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.LeiJiLogin);
		}
		if (null != this.m_meiRiZaiXian)
		{
			this.m_meiRiZaiXian.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.EveryDayOnline);
		}
		if (null != this.m_LiPinDuiHuanPart)
		{
			this.m_LiPinDuiHuanPart.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.GiftChage);
		}
		if (null != this.m_MonthCardPart)
		{
			this.m_MonthCardPart.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.YueKaBack);
		}
		if (null != this.playerRecall)
		{
			this.playerRecall.gameObject.SetActive(HuoDongPart.HuoDongType.ZhanYouCallBack == nActivePage);
		}
		if (null != this.nochongzhi)
		{
			this.nochongzhi.gameObject.SetActive(nActivePage == HuoDongPart.HuoDongType.ChongZhiBack || nActivePage == HuoDongPart.HuoDongType.GiftChage || nActivePage == HuoDongPart.HuoDongType.YueKaBack || nActivePage == HuoDongPart.HuoDongType.ZhanYouCallBack);
		}
	}

	private void SetTabBtnIndex(HuoDongPart.HuoDongType nIndex)
	{
		this.m_nTabBtnIndex = (int)nIndex;
		BtnObj btnObj = this.BtnObjs[(int)nIndex];
		this.SetBtnActieve(btnObj.m_BtnItem);
	}

	private void InitPnl(HuoDongPart.HuoDongType type)
	{
		switch (type)
		{
		case HuoDongPart.HuoDongType.ChongZhiBack:
			if (Global.IsOperateUnPermitInKuaFuMapCheck(false, false))
			{
				if (null == this.nochongzhi)
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					this.nochongzhi = U3DUtils.NEW<NoChongZhiPart>();
					U3DUtils.AddChild(this.m_Pnl.gameObject, this.nochongzhi.gameObject, true);
					this.nochongzhi.gameObject.SetActive(false);
				}
				return;
			}
			if (null == this.m_ChongzhiHuikuiPart)
			{
				this.m_ChongzhiHuikuiPart = U3DUtils.NEW<HuodongPartChongzhiHuikuiPart>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ChongzhiHuikuiPart.gameObject, true);
				this.m_ChongzhiHuikuiPart.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.YueKaBack:
			if (Global.IsOperateUnPermitInKuaFuMapCheck(false, false))
			{
				if (null == this.nochongzhi)
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					this.nochongzhi = U3DUtils.NEW<NoChongZhiPart>();
					U3DUtils.AddChild(this.m_Pnl.gameObject, this.nochongzhi.gameObject, true);
					this.nochongzhi.gameObject.SetActive(false);
				}
				return;
			}
			if (null == this.m_MonthCardPart)
			{
				this.m_MonthCardPart = U3DUtils.NEW<HuoDongPartPnlMonthCard>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_MonthCardPart.gameObject, true);
				this.m_MonthCardPart.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.EveryDayActive:
			if (null == this.m_HuoyuePart)
			{
				this.m_HuoyuePart = U3DUtils.NEW<HuodongPartHuoyuePart>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_HuoyuePart.gameObject, true);
				this.m_HuoyuePart.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.LevelAward:
			if (null == this.m_UpLevelAwardPart)
			{
				this.m_UpLevelAwardPart = U3DUtils.NEW<UpLevelAwardPart>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_UpLevelAwardPart.gameObject, true);
				this.m_UpLevelAwardPart.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.ZhanDouLiAward:
			if (null == this.m_ZhanDouLiAwardPart)
			{
				this.m_ZhanDouLiAwardPart = U3DUtils.NEW<ZhanDouLiAwardPart>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ZhanDouLiAwardPart.gameObject, true);
				this.m_ZhanDouLiAwardPart.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.LianXuLogin:
			if (null == this.m_dengLuHaoLi)
			{
				this.m_dengLuHaoLi = U3DUtils.NEW<HuoDongPartPnlDengLuHaoLi>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_dengLuHaoLi.gameObject, true);
				this.m_dengLuHaoLi.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.LeiJiLogin:
			if (null == this.m_LeiJiDengLu)
			{
				this.m_LeiJiDengLu = U3DUtils.NEW<HuoDongPartPnlLeiJiDengLu>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_LeiJiDengLu.gameObject, true);
				this.m_LeiJiDengLu.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.EveryDayOnline:
			if (null == this.m_meiRiZaiXian)
			{
				this.m_meiRiZaiXian = U3DUtils.NEW<HuoDongPartPnlMeiRiZaiXian>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_meiRiZaiXian.gameObject, true);
				this.m_meiRiZaiXian.gameObject.SetActive(false);
			}
			break;
		case HuoDongPart.HuoDongType.GiftChage:
			if (Global.IsOperateUnPermitInKuaFuMapCheck(false, false))
			{
				if (null == this.nochongzhi)
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					this.nochongzhi = U3DUtils.NEW<NoChongZhiPart>();
					U3DUtils.AddChild(this.m_Pnl.gameObject, this.nochongzhi.gameObject, true);
					this.nochongzhi.gameObject.SetActive(false);
				}
				return;
			}
			if (null == this.m_LiPinDuiHuanPart)
			{
				this.m_LiPinDuiHuanPart = U3DUtils.NEW<HuoDongPartLiPinDuiHuan>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_LiPinDuiHuanPart.gameObject, true);
				this.m_LiPinDuiHuanPart.gameObject.SetActive(false);
			}
			break;
		}
		if (ConfigLaoWanJiaZhaoHui.IsInPlayerRecallActivity() && type == HuoDongPart.HuoDongType.ZhanYouCallBack)
		{
			if (Global.IsOperateUnPermitInKuaFuMapCheck(false, false))
			{
				if (null == this.nochongzhi)
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					this.nochongzhi = U3DUtils.NEW<NoChongZhiPart>();
					U3DUtils.AddChild(this.m_Pnl.gameObject, this.nochongzhi.gameObject, true);
					this.nochongzhi.gameObject.SetActive(false);
				}
				return;
			}
			if (null == this.playerRecall)
			{
				this.playerRecall = U3DUtils.NEW<PlayerRecall>();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.playerRecall.gameObject, true);
				this.playerRecall.InitRecallData();
				this.playerRecall.gameObject.SetActive(false);
			}
		}
	}

	private void PlayerRecallActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (args == null)
		{
			return;
		}
		if (null != this._ActivityTipIcons[7])
		{
			TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
			this._ActivityTipIcons[7].SetActive(args.IsActive);
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.partCanvas);
		this.partCanvas.Background = new SolidColorBrush(16777215U);
		this.partCanvas.Width = 607.0;
		this.partCanvas.Height = 475.0;
		Canvas.SetLeft(this.partCanvas, 86);
		Canvas.SetTop(this.partCanvas, 3);
		this.Container.Children.Add(this.HuoDongList);
		this.HuoDongList.Width = 80.0;
		this.HuoDongList.Height = 479.0;
		this.HuoDongList.ItemMargin = new Thickness(0.0, 8.0, 0.0, 0.0);
		Canvas.SetLeft(this.HuoDongList, 8);
		Canvas.SetLeft(this.HuoDongList, 8);
		Canvas.SetTop(this.HuoDongList, 9);
		this.HuoDongList.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.selectItemImage);
		this.selectItemImage.Width = 80.0;
		this.selectItemImage.Height = 21.0;
		Canvas.SetLeft(this.selectItemImage, 8);
		this.selectItemImage.Source = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 80.0, 21.0, 3.0, 2.0));
		this.selectItemImage.mouseEnabled = false;
	}

	public void InitPartData(int showPage = 0)
	{
		this.LoadHuoDongList();
		this.SetHuoDongListPage(showPage);
		GameInstance.Game.SpriteFetchActivtData(0);
	}

	public void SetHuoDongListPage(int showPage = 0)
	{
		this.SetHuoDongListPart(showPage);
	}

	private void SetBackground(int showPage = 0)
	{
		switch (showPage)
		{
		case 0:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_XinQuHuoDong_bak.png"), false, 0);
			break;
		case 1:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_XinShouKaLiBao_bak.png"), false, 0);
			break;
		case 2:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_XinShouKaLiBao_bak.png"), false, 0);
			break;
		case 3:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_JianMianYouLi_bak.png"), false, 0);
			break;
		case 4:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_DengLuYouLi_bak.png"), false, 0);
			break;
		case 5:
			base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_ZaiXianYouLi_bak.png"), false, 0);
			break;
		}
	}

	private void LoadHuoDongList()
	{
		this.ItemCollection.Clear();
		if (HuoDongPart.HuoDongItemNames == null)
		{
			return;
		}
		for (int i = 0; i < HuoDongPart.HuoDongItemNames.Length; i++)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 80.0;
			gicon.Height = 25.0;
			gicon.Name = i.ToString();
			gicon.TextColor = new SolidColorBrush(11394222U);
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
			gicon.Cursor = Cursors.Hand;
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetHuoDongListPart(Convert.ToInt32((s as GIcon).Name));
			};
			gicon.Text = HuoDongPart.HuoDongItemNames[i];
			this.ItemCollection.AddNoUpdate(gicon);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void SetHuoDongListPart(int showPage)
	{
		this.SetBackground(showPage);
		this.SetselectItemImage(showPage);
		this.partCanvas.Clear();
		switch (showPage)
		{
		case 0:
			if (null == this.xinQuHuoDongPart)
			{
				this.xinQuHuoDongPart = U3DUtils.NEW<XinQuHuoDongPart>();
				this.xinQuHuoDongPart.InitPartSize(607, 475);
				this.xinQuHuoDongPart.InitPartData();
				this.xinQuHuoDongPart.SetXinQuHuoDongListPage(0);
			}
			this.partCanvas.Add(this.xinQuHuoDongPart);
			Canvas.SetLeft(this.xinQuHuoDongPart, 0);
			Canvas.SetTop(this.xinQuHuoDongPart, 0);
			break;
		case 1:
			if (null == this.chongZhiDaLiPrt)
			{
				this.chongZhiDaLiPrt = U3DUtils.NEW<ChongZhiDaliPart>();
				this.chongZhiDaLiPrt.InitPartSize(607, 475);
				this.chongZhiDaLiPrt.InitPartData();
			}
			else
			{
				this.chongZhiDaLiPrt.ResetGetNewData();
				this.chongZhiDaLiPrt.GetNewData();
			}
			this.partCanvas.Add(this.chongZhiDaLiPrt);
			Canvas.SetLeft(this.chongZhiDaLiPrt, 0);
			Canvas.SetTop(this.chongZhiDaLiPrt, 0);
			break;
		case 2:
			if (null == this.xinShouKaPart)
			{
				this.xinShouKaPart = U3DUtils.NEW<XinShouKaPart>();
				this.xinShouKaPart.InitPartSize(607, 475);
				this.xinShouKaPart.InitPartData();
			}
			this.partCanvas.Add(this.xinShouKaPart);
			Canvas.SetLeft(this.xinShouKaPart, 0);
			Canvas.SetTop(this.xinShouKaPart, 0);
			break;
		case 3:
			if (null == this.jianMianYouLiPart)
			{
				this.jianMianYouLiPart = U3DUtils.NEW<JianMianYouLiPart>();
				this.jianMianYouLiPart.InitPartSize(607, 475);
				this.jianMianYouLiPart.InitPartData();
			}
			this.partCanvas.Add(this.jianMianYouLiPart);
			Canvas.SetLeft(this.jianMianYouLiPart, 0);
			Canvas.SetTop(this.jianMianYouLiPart, 0);
			break;
		case 4:
			if (null == this.dengLuYouLiPart)
			{
				this.dengLuYouLiPart = U3DUtils.NEW<DengLuYouLiPart>();
				this.dengLuYouLiPart.InitPartSize(607, 475);
				this.dengLuYouLiPart.InitPartData();
			}
			this.partCanvas.Add(this.dengLuYouLiPart);
			Canvas.SetLeft(this.dengLuYouLiPart, 0);
			Canvas.SetTop(this.dengLuYouLiPart, 0);
			break;
		case 5:
			if (null == this.zaiXianYouLiPart)
			{
				this.zaiXianYouLiPart = U3DUtils.NEW<ZaiXianYouLiPart>();
				this.zaiXianYouLiPart.InitPartSize(607, 475);
				this.zaiXianYouLiPart.InitPartData();
			}
			this.partCanvas.Add(this.zaiXianYouLiPart);
			Canvas.SetLeft(this.zaiXianYouLiPart, 0);
			Canvas.SetTop(this.zaiXianYouLiPart, 0);
			break;
		}
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void SetselectItemImage(int Type)
	{
		int num = Type + 1;
		if (num == 1)
		{
			Canvas.SetTop(this.selectItemImage, 17);
		}
		else
		{
			Canvas.SetTop(this.selectItemImage, 21 * num + 12 * (num - 1) - 4);
		}
	}

	public GameObject m_Btn;

	public GameObject m_Pnl;

	private ObservableCollection m_TabBtnOBC;

	public ListBox m_ListTabBtn = new ListBox();

	public GButton m_BtnLastSelect;

	public GButton m_BtnCurrentSelect;

	public GButton m_BtnClose;

	public int m_nTabBtnIndex;

	public HuoDongPartPnlMeiRiZaiXian m_meiRiZaiXian;

	public HuoDongPartPnlDengLuHaoLi m_dengLuHaoLi;

	public HuoDongPartPnlLeiJiDengLu m_LeiJiDengLu;

	public ChengJiuPart m_ChengJiuPart;

	public HuodongPartChongzhiHuikuiPart m_ChongzhiHuikuiPart;

	public HuodongPartHuoyuePart m_HuoyuePart;

	public HuoDongPartLiPinDuiHuan m_LiPinDuiHuanPart;

	public UpLevelAwardPart m_UpLevelAwardPart;

	public ZhanDouLiAwardPart m_ZhanDouLiAwardPart;

	public HuoDongPartPnlMonthCard m_MonthCardPart;

	public GameObject[] _ActivityTipIcons;

	public Dictionary<int, BtnObj> BtnObjs = new Dictionary<int, BtnObj>();

	public DPSelectedItemEventHandler DPSelectedItem;

	public PlayerRecall playerRecall;

	public NoChongZhiPart nochongzhi;

	public UILabel WindowLblTitle;

	private LoadingWindow LoadingWin;

	public Image selectItemImage = new Image();

	private ListBox HuoDongList = new ListBox();

	private static string[] HuoDongItemNames = new string[]
	{
		Global.GetLang("新区活动"),
		Global.GetLang("充值大礼"),
		Global.GetLang("新手卡礼包"),
		Global.GetLang("见面有礼"),
		Global.GetLang("登陆有礼"),
		Global.GetLang("在线有礼")
	};

	private Canvas partCanvas = new Canvas();

	public XinQuHuoDongPart xinQuHuoDongPart;

	public XinShouKaPart xinShouKaPart;

	public ChongZhiDaliPart chongZhiDaLiPrt;

	private JianMianYouLiPart jianMianYouLiPart;

	public DengLuYouLiPart dengLuYouLiPart;

	public ZaiXianYouLiPart zaiXianYouLiPart;

	private ObservableCollection _ItemCollection;

	public enum HuoDongType
	{
		ChongZhiBack,
		YueKaBack,
		EveryDayActive,
		LevelAward,
		ZhanDouLiAward,
		LianXuLogin,
		LeiJiLogin,
		EveryDayOnline,
		GiftChage,
		ZhanYouCallBack,
		MaxValue
	}
}
