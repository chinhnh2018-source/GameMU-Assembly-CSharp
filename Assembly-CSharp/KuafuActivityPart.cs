using System;
using System.Collections.Generic;
using System.Globalization;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class KuafuActivityPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.activityOBC = this.activityList.ItemsSource;
		this.InitActivity();
		this.activityList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedItem);
	}

	private void InitActivity()
	{
		XElement gameResXml = Global.GetGameResXml("Config/KuaFuHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				KuafuActivityItem kuafuActivityItem = U3DUtils.NEW<KuafuActivityItem>();
				this.activityOBC.Add(kuafuActivityItem);
				kuafuActivityItem.InitConfig(xelement);
				UIDragObject component = kuafuActivityItem.GetComponent<UIDragObject>();
				component.target = this.activityList.transform;
				UIPanel component2 = kuafuActivityItem.GetComponent<UIPanel>();
				if (component2)
				{
					Object.Destroy(component2);
				}
			}
		}
	}

	private void SelectedItem(object sender, MouseEvent e)
	{
		KuafuActivityItem kuafuActivityItem = U3DUtils.AS<KuafuActivityItem>(this.activityList.SelectedItem);
		if (kuafuActivityItem == null)
		{
			return;
		}
		if (!(this.kuafuItem != null) || this.kuafuItem != kuafuActivityItem)
		{
		}
		this.kuafuItem = kuafuActivityItem;
		int trigger = 0;
		int param = 0;
		int param2 = 0;
		if (kuafuActivityItem.ID == 20000)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuafuHuanyingsiyuan, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.KuafuHuanyingsiyuan, trigger, param, param2, true);
			}
			this.ShowKuafuJoin(kuafuActivityItem);
		}
		else if (kuafuActivityItem.ID == 20001)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.TianTiJingSai, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.TianTiJingSai, trigger, param, param2, true);
			}
			PlayZone.GlobalPlayZone.ShowTianTiJingSaiWindow();
		}
		else if (kuafuActivityItem.ID == 20002)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YongZheZhanChang, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.YongZheZhanChang, trigger, param, param2, true);
				return;
			}
			this.ShowYongZheZhanChang(kuafuActivityItem);
		}
		else if (kuafuActivityItem.ID == 20003)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuafuHuodongBOSS, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.KuafuHuodongBOSS, trigger, param, param2, true);
				return;
			}
			this.ShowKuaFuBOSS(kuafuActivityItem);
		}
		else if (kuafuActivityItem.ID == 20004)
		{
			if (this.miniData != null && this.miniData.IsZhengBaOpened && this.miniData.IsThisMonthInActivity)
			{
				this.ShowZhongShenZhengBa(kuafuActivityItem);
			}
			else
			{
				string lang = Global.GetLang("此功能未开启！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		}
		else if (kuafuActivityItem.ID == 20005)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.PKLovers, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.PKLovers, trigger, param, param2, true);
				return;
			}
			if (GameInstance.Game.CurrentSession != null && GameInstance.Game.CurrentSession.MarriageData != null && (int)GameInstance.Game.CurrentSession.MarriageData.byMarrytype == -1)
			{
				Super.HintMainText(Global.GetLang("您当前为未结婚状态,不能参加此活动"), 10, 3);
				return;
			}
			if (Global.Data.roleData.OccupationList != null && Global.Data.roleData.Occupation != Global.Data.roleData.OccupationList[0])
			{
				string lang2 = Global.GetLang("此功能必须使用主职业");
				Super.HintMainText(lang2, 10, 3);
				return;
			}
			this.ShowPKLoversWindow();
		}
		else if (kuafuActivityItem.ID == 20006)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuWangZhe, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuWangZhe, trigger, param, param2, true);
				return;
			}
			this.ShowKuaFuWangZhe(kuafuActivityItem);
		}
		else if (kuafuActivityItem.ID == 20007)
		{
			this.ShowArmyCaiJiPart();
		}
		else if (kuafuActivityItem.ID == 20008)
		{
			if (this.mBangHuiMatchMainInfo == null)
			{
				Super.HintMainText(Global.GetLang("黄金联赛暂未开启无法查看"), 10, 3);
				return;
			}
			if (this.mBangHuiMatchMainInfo.seasonid > 0)
			{
				try
				{
					DateTime dateTime = DateTime.ParseExact(this.mBangHuiMatchMainInfo.seasonid.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture);
					if (dateTime <= Global.GetCorrectDateTime() && this.mBangHuiMatchMainInfo.round == 1)
					{
						PlayZone.GlobalPlayZone.OpenZhanMengLianSaiGuanZhanWindow(this.mBangHuiMatchMainInfo, false);
						return;
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogError<Exception>(new Exception[]
					{
						ex
					});
				}
			}
			if (this.mBangHuiMatchMainInfo.LastRoundPKInfo == null || this.mBangHuiMatchMainInfo.LastRoundPKInfo.Count <= 0)
			{
				PlayZone.GlobalPlayZone.OpenZhanMengLianSaiGuanZhanWindow(this.mBangHuiMatchMainInfo, true);
				return;
			}
			PlayZone.GlobalPlayZone.OpenZhanMengLianSaiGuanZhanWindow(this.mBangHuiMatchMainInfo, false);
		}
		else if (kuafuActivityItem.ID == 20009)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuPlunder, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuPlunder, trigger, param, param2, true);
				return;
			}
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = 1509
			});
		}
		else if (kuafuActivityItem.ID == 20010)
		{
			PlayZone.GlobalPlayZone.OpenTeamCompetePart(false);
		}
	}

	private void ShowKuafuJoin(KuafuActivityItem item)
	{
		if (this.kuafuJoinWindow != null)
		{
			this.CloseChildWindow(this.kuafuJoinWindow);
			this.kuafuJoinWindow = null;
			this.kuafuJoinPart = null;
		}
		if (this.kuafuJoinWindow == null && this.kuafuJoinPart == null)
		{
			this.kuafuJoinWindow = U3DUtils.NEW<GChildWindow>();
			this.kuafuJoinWindow.IsShowModal = true;
			this.InitChildWindow(this.kuafuJoinWindow, "kuafu", false);
			this.Container.Children.Add(this.kuafuJoinWindow);
			this.kuafuJoinWindow.ModalType = ChildWindowModalType.Translucent;
			this.kuafuJoinPart = U3DUtils.NEW<KuafuJoinPart>();
			PlayZone.GlobalPlayZone._KuafuJoinPart = this.kuafuJoinPart;
			this.kuafuJoinPart.InitData(item);
			this.kuafuJoinPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseChildWindow(this.kuafuJoinWindow);
				this.kuafuJoinPart = null;
				this.kuafuJoinWindow = null;
				PlayZone.GlobalPlayZone._KuafuJoinPart = null;
			};
			this.kuafuJoinWindow.SetContent(this.kuafuJoinWindow.BodyPresenter, this.kuafuJoinPart, 0.0, 0.0, true);
		}
	}

	private void ShowYongZheZhanChang(KuafuActivityItem item)
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		this.InitChildWindow(gchildWindow, "TianTiJingSai", false);
		this.Container.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.Translucent;
		KuafuYongZheZhanChangPart kuafuYongZheZhanChangPart = U3DUtils.NEW<KuafuYongZheZhanChangPart>();
		gchildWindow.SetContent(gchildWindow.BodyPresenter, kuafuYongZheZhanChangPart, 0.0, 0.0, true);
		kuafuYongZheZhanChangPart.InitData(item);
	}

	private void ShowKuaFuBOSS(KuafuActivityItem item)
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		this.InitChildWindow(gchildWindow, "KuafuBossPart", false);
		this.Container.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.Translucent;
		KuafuBossPart kuafuBossPart = U3DUtils.NEW<KuafuBossPart>();
		gchildWindow.SetContent(gchildWindow.BodyPresenter, kuafuBossPart, 0.0, 0.0, true);
		kuafuBossPart.InitData(item);
	}

	private void ShowKuaFuWangZhe(KuafuActivityItem item)
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		this.InitChildWindow(gchildWindow, "KuaFuWangZhePart", false);
		this.Container.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.Translucent;
		KuaFuWangZhePart kuaFuWangZhePart = U3DUtils.NEW<KuaFuWangZhePart>();
		gchildWindow.SetContent(gchildWindow.BodyPresenter, kuaFuWangZhePart, 0.0, 0.0, true);
		kuaFuWangZhePart.InitData(item);
	}

	private void ShowZhongShenZhengBa(KuafuActivityItem item)
	{
		PlayZone.GlobalPlayZone.ShowZhongShenZhengBaWindow();
	}

	private void ShowPKLoversWindow()
	{
		PlayZone.GlobalPlayZone.OpenPKLoversWindow();
	}

	private void ShowArmyCaiJiPart()
	{
		PlayZone.GlobalPlayZone.OpenArmyCaiJiActivityWindow();
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		Super.CloseChildWindow(this.Container, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
	}

	public void InitZhengBaTime(ZhengBaMiniStateData time)
	{
		KuafuActivityItem kuafuActivityItem = U3DUtils.AS<KuafuActivityItem>(this.activityOBC[4]);
		if (kuafuActivityItem == null)
		{
			return;
		}
		this.miniData = time;
		if (time.IsThisMonthInActivity && time.IsZhengBaOpened)
		{
			kuafuActivityItem.InitZhengBaTime(time);
			UITexture component = kuafuActivityItem.netBak.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Transparent Colored");
		}
		else
		{
			UITexture component2 = kuafuActivityItem.netBak.GetComponent<UITexture>();
			component2.shader = Shader.Find("Unlit/Gray");
			kuafuActivityItem.labTime.text = string.Empty;
			kuafuActivityItem.labType.text = Global.GetLang("活动未开启");
		}
	}

	public void InitArmyCaiJiTime(DateTime time)
	{
		KuafuActivityItem kuafuActivityItem = U3DUtils.AS<KuafuActivityItem>(this.activityOBC[7]);
		if (kuafuActivityItem == null)
		{
			return;
		}
		int num = Global.GetCorrectDateTime().CompareTo(time);
		if (num >= 0)
		{
			UITexture component = kuafuActivityItem.netBak.GetComponent<UITexture>();
			kuafuActivityItem.labTime.text = string.Empty;
			kuafuActivityItem.labType.text = Global.GetLang("未开启双倍时间");
		}
		else
		{
			kuafuActivityItem.InitCaiJiTime(time);
			UITexture component2 = kuafuActivityItem.netBak.GetComponent<UITexture>();
			kuafuActivityItem.labType.text = Global.GetLang("活动结束");
		}
	}

	public void InitZhanMengLianSaiJingCaiTimeState(BangHuiMatchMainInfo info)
	{
		KuafuActivityItem kuafuActivityItem = U3DUtils.AS<KuafuActivityItem>(this.activityOBC[8]);
		if (kuafuActivityItem == null)
		{
			return;
		}
		this.mBangHuiMatchMainInfo = info;
		if (this.mBangHuiMatchMainInfo == null)
		{
			return;
		}
		if (this.HaveDatas(this.mBangHuiMatchMainInfo.LastRoundPKInfo))
		{
			kuafuActivityItem.InitZhanMengLianSaiJingCaiData(this.mBangHuiMatchMainInfo);
			return;
		}
		if (this.HaveDatas(this.mBangHuiMatchMainInfo.CurRoundPKInfo) && !this.HaveDatas(this.mBangHuiMatchMainInfo.LastRoundPKInfo) && this.mBangHuiMatchMainInfo.seasonid > 0)
		{
			try
			{
				DateTime dateTime = DateTime.ParseExact(this.mBangHuiMatchMainInfo.seasonid.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture);
				if (!(dateTime > Global.GetCorrectDateTime()))
				{
					kuafuActivityItem.InitZhanMengLianSaiJingCaiData(this.mBangHuiMatchMainInfo);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					ex
				});
			}
		}
	}

	private bool HaveDatas(List<BangHuiMatchPKInfo> list)
	{
		return list != null && list.Count > 0;
	}

	public void RefreshBangHuiMatchMainInfoByGuessData(string[] feilds)
	{
		int num = Global.SafeConvertToInt32(feilds[0]);
		int bhid1 = Global.SafeConvertToInt32(feilds[1]);
		int bhid2 = Global.SafeConvertToInt32(feilds[2]);
		int num2 = Global.SafeConvertToInt32(feilds[3]);
		if (this.mBangHuiMatchMainInfo != null && num >= 0)
		{
			try
			{
				List<BangHuiMatchPKInfo> curRoundPKInfo = this.mBangHuiMatchMainInfo.CurRoundPKInfo;
				if (curRoundPKInfo != null && curRoundPKInfo.Count > 0)
				{
					BangHuiMatchPKInfo bangHuiMatchPKInfo = curRoundPKInfo.Find((BangHuiMatchPKInfo ret) => ret.bhid1 == bhid1 && ret.bhid2 == bhid2);
					if (bangHuiMatchPKInfo != null)
					{
						bangHuiMatchPKInfo.guess = (byte)num2;
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					ex
				});
			}
		}
	}

	public void NocticeKuaFuPlunderStateCallBack(KuaFuLueDuoGameStates State)
	{
		for (int i = 0; i < this.activityOBC.Count; i++)
		{
			KuafuActivityItem kuafuActivityItem = U3DUtils.AS<KuafuActivityItem>(this.activityOBC[i]);
			if (null != kuafuActivityItem && kuafuActivityItem.ID == 20009)
			{
				kuafuActivityItem.NocticeKuaFuPlunderStateCallBack(State);
				break;
			}
		}
	}

	public ListBox activityList;

	public GScrollBarPageList pageRadio;

	private ObservableCollection activityOBC;

	private GChildWindow kuafuJoinWindow;

	private KuafuJoinPart kuafuJoinPart;

	private ZhengBaMiniStateData miniData;

	private KuafuActivityItem kuafuItem;

	private BangHuiMatchMainInfo mBangHuiMatchMainInfo;
}
