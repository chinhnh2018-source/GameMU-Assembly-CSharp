using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ActivityPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this._GTabControl.TabBtns[this.RuntimeTabIndexes[7]], default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this._GTabControl.TabBtns[this.RuntimeTabIndexes[1]], default(Vector4));
			}
			else if (id == 1000)
			{
				SystemHelpPart.SetMask(this._RiChangFuBen._TaskList.GetItemByIndex(0).transform, default(Vector4));
			}
			else if (id == 1001)
			{
				SystemHelpPart.SetMask(this._JingYanFuBen._TaskList.GetItemByIndex(0).transform, default(Vector4));
			}
			else if (id == 1002)
			{
				SystemHelpPart.SetMask(this._JingYanFuBen._TaskList.GetItemByIndex(1).transform, default(Vector4));
			}
			else if (id == 2001)
			{
				SystemHelpPart.SetMask(this._ZuDuiFuBen.fubenListBox.GetItemByIndex(0).transform, default(Vector4));
			}
			else if (id == 2002)
			{
				SystemHelpPart.SetMask(this._GTabControl.TabBtns[this.RuntimeTabIndexes[1]], default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(2003, null);
		ActivityTipManager.RegActivityTipItem(2001, null);
		ActivityTipManager.RegActivityTipItem(2002, null);
		ActivityTipManager.RegActivityTipItem(2004, null);
		ActivityTipManager.RegActivityTipItem(1001, null);
		ActivityTipManager.RegActivityTipItem(1002, null);
		ActivityTipManager.RegActivityTipItem(1003, null);
		ActivityTipManager.RegActivityTipItem(1005, null);
		ActivityTipManager.RegActivityTipItem(13000, null);
		ActivityTipManager.RegActivityTipItem(1100, null);
		ActivityTipManager.RegActivityTipItem(15010, null);
		ActivityTipManager.RegActivityTipItem(15011, null);
		PlayZone.GlobalPlayZone.ShiJieBossPart = null;
		PlayZone.GlobalPlayZone.ShowFeiBossPart = null;
		DaTaoShaDataManager.DaTaoShaSwitchCallBak = null;
		Object.Destroy(this._PaTaFuBen);
		this._PaTaFuBen = null;
		base.OnDestroy();
	}

	public void InitPartData(int type = 0, int showPage = 0)
	{
		this.WindowType = type;
		for (int i = 0; i < 14; i++)
		{
			this.RuntimeTabIndexes[i] = -1;
		}
		this._GTabControl = U3DUtils.NEW<GTabControl>("ActivityTab");
		U3DUtils.AddChild(base.gameObject, this._GTabControl.gameObject, false);
		if (type == 1)
		{
			this._GTabControl.BtnsPosition = new Vector3(this._GTabControl.BtnsPosition.x, 177f, this._GTabControl.BtnsPosition.z);
			this._GTabControl.BtnsSize = new Vector3(this._GTabControl.BtnsSize.x, -51f, this._GTabControl.BtnsSize.z);
			this._GTabControl.arrangement = GTabControl.Arrangement.Vertical;
		}
		this._GTabControl.BeforeTabBtnClick = delegate(object s, MouseEvent e)
		{
			if (e.Index == this._GTabControl.SelectIndex)
			{
				return;
			}
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (e.Index == this.RuntimeTabIndexes[3] && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.PaTa, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.PaTa, trigger, param, param2, true);
				return;
			}
			if (e.Index == this.RuntimeTabIndexes[9] && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GuZhanChang, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.GuZhanChang, trigger, param, param2, true);
				return;
			}
			if (e.Index == this.RuntimeTabIndexes[11] && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShuiJingHuanJing, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ShuiJingHuanJing, trigger, param, param2, true);
				return;
			}
			this._GTabControl.SetTab(s as GameObject);
		};
		this._GTabControl.OnTabBtnClick = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Index == this.RuntimeTabIndexes[3])
			{
				if (null == this._PaTaFuBen)
				{
					this._PaTaFuBen = U3DUtils.NEW<RiChangPaTaPart>();
					this._GTabControl.AddPageContent(this._PaTaFuBen.gameObject, e.Index);
					this._Bak.URL = Global.GetGameResImageString("richangPata_bak.jpg");
					this._PaTaFuBen.InitPartData(PataSweepTypes.None);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[3]);
				this._PaTaFuBen = null;
			}
			if (e.Index == this.RuntimeTabIndexes[2] || e.Index == this.RuntimeTabIndexes[4])
			{
				if (null == this._ZuDuiFuBen)
				{
					this._ZuDuiFuBen = U3DUtils.NEW<ZuduiFubenPart>();
					this._GTabControl.AddPageContent(this._ZuDuiFuBen.gameObject, e.Index);
					if (e.Index == this.RuntimeTabIndexes[2])
					{
						this._ZuDuiFuBen.InitPartData(ActivityCategorys.ZuDuiFuBen);
					}
					else
					{
						this._ZuDuiFuBen.InitPartData(ActivityCategorys.KuaFuFuBen);
					}
					this._Bak.URL = Global.GetGameResImageString("zuduifuben_bak.jpg");
				}
				else if (this._GTabControl.SelectIndex_IfNotChangeHappen == this.RuntimeTabIndexes[2] && e.Index == this.RuntimeTabIndexes[4])
				{
					this._GTabControl.AddPageContent(this._ZuDuiFuBen.gameObject, e.Index);
					this._ZuDuiFuBen.InitPartData(ActivityCategorys.KuaFuFuBen);
					this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[2]);
				}
				else if (this._GTabControl.SelectIndex_IfNotChangeHappen == this.RuntimeTabIndexes[4] && e.Index == this.RuntimeTabIndexes[2])
				{
					this._GTabControl.AddPageContent(this._ZuDuiFuBen.gameObject, e.Index);
					this._ZuDuiFuBen.InitPartData(ActivityCategorys.ZuDuiFuBen);
					this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[4]);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[4]);
				this._ZuDuiFuBen = null;
			}
			if (e.Index == this.RuntimeTabIndexes[1])
			{
				if (null == this._JingYanFuBen)
				{
					this._JingYanFuBen = U3DUtils.NEW<RiChangFuBenPart>();
					this._GTabControl.AddPageContent(this._JingYanFuBen.gameObject, e.Index);
					this._JingYanFuBen.InitPartData(ActivityCategorys.RiChangFuBen);
					this._Bak.URL = Global.GetGameResImageString("richangHuoDongrichangFuBen_Bak.jpg");
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[1]);
				this._JingYanFuBen = null;
			}
			if (e.Index == this.RuntimeTabIndexes[0])
			{
				if (null == this._RiChangFuBen)
				{
					this._RiChangFuBen = U3DUtils.NEW<RiChangFuBenPart>();
					this._GTabControl.AddPageContent(this._RiChangFuBen.gameObject, e.Index);
					this._RiChangFuBen.InitPartData(ActivityCategorys.JuQingFuBen);
					this._Bak.URL = Global.GetGameResImageString("richangHuoDongrichangFuBen_Bak.jpg");
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[0]);
				this._RiChangFuBen = null;
			}
			if (e.Index == this.RuntimeTabIndexes[5])
			{
				if (null == this._HuoDongRiLi)
				{
					this._HuoDongRiLi = U3DUtils.NEW<HuoDongRiLiPart>();
					this._GTabControl.AddPageContent(this._HuoDongRiLi.gameObject, e.Index);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[5]);
				this._HuoDongRiLi = null;
			}
			if (e.Index == this.RuntimeTabIndexes[6])
			{
				if (null == this._RiChangHuoDong)
				{
					this._RiChangHuoDong = U3DUtils.NEW<RiChangHuoDongPart>();
					this._GTabControl.AddPageContent(this._RiChangHuoDong.gameObject, e.Index);
					this._RiChangHuoDong.InitPartData(ActivityCategorys.RiChangHuoDong);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[6]);
				this._RiChangHuoDong = null;
			}
			if (e.Index == this.RuntimeTabIndexes[7])
			{
				if (null == this._ShiJieBoss)
				{
					this._ShiJieBoss = U3DUtils.NEW<HuoDongBossPart>();
					this._GTabControl.AddPageContent(this._ShiJieBoss.gameObject, e.Index);
					this._ShiJieBoss.InitPartData(0);
					PlayZone.GlobalPlayZone.ShiJieBossPart = this._ShiJieBoss;
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[7]);
				this._ShiJieBoss = null;
				PlayZone.GlobalPlayZone.ShiJieBossPart = null;
			}
			if (e.Index == this.RuntimeTabIndexes[10])
			{
				if (null == this._HuangJinBoss)
				{
					this._HuangJinBoss = U3DUtils.NEW<HuoDongBossPart>();
					this._GTabControl.AddPageContent(this._HuangJinBoss.gameObject, e.Index);
					this._HuangJinBoss.InitPartData(1);
					PlayZone.GlobalPlayZone._HuangJinBossPart = this._HuangJinBoss;
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[10]);
				this._HuangJinBoss = null;
				PlayZone.GlobalPlayZone._HuangJinBossPart = null;
			}
			if (e.Index == this.RuntimeTabIndexes[8])
			{
				if (null == this._RiChangVIPHuoDong)
				{
					this._RiChangVIPHuoDong = U3DUtils.NEW<RiChangVIPHuoDongPart>();
					this._GTabControl.AddPageContent(this._RiChangVIPHuoDong.gameObject, e.Index);
					this._RiChangVIPHuoDong.InitPartData(ActivityCategorys.VIPHuoDong);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[8]);
				this._RiChangVIPHuoDong = null;
			}
			if (e.Index == this.RuntimeTabIndexes[9])
			{
				if (null == this._RiChangGuZhanChangPart)
				{
					this._RiChangGuZhanChangPart = U3DUtils.NEW<RiChangGuZhanChangPart>();
					this._GTabControl.AddPageContent(this._RiChangGuZhanChangPart.gameObject, e.Index);
					this._RiChangGuZhanChangPart.InitPartData(ActivityCategorys.GuZhanChang);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[9]);
				this._RiChangGuZhanChangPart = null;
			}
			if (e.Index == this.RuntimeTabIndexes[11])
			{
				if (null == this._RiChangShuiJingHuanJingPart)
				{
					this._RiChangShuiJingHuanJingPart = U3DUtils.NEW<RiChangShuiJingHuanJingPart>();
					this._GTabControl.AddPageContent(this._RiChangShuiJingHuanJingPart.gameObject, e.Index);
					this._RiChangShuiJingHuanJingPart.InitPartData(ActivityCategorys.ShuiJingHuanJing);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[11]);
				this._RiChangShuiJingHuanJingPart = null;
			}
			if (e.Index == this.RuntimeTabIndexes[12])
			{
				if (this._KuafuActivityPart == null)
				{
					this._KuafuActivityPart = U3DUtils.NEW<KuafuActivityPart>();
					this._GTabControl.AddPageContent(this._KuafuActivityPart.gameObject, e.Index);
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[12]);
				this._KuafuActivityPart = null;
			}
			if (e.Index == this.RuntimeTabIndexes[13])
			{
				if (this.mTeamCompeteActivityPart == null)
				{
					this.mTeamCompeteActivityPart = U3DUtils.NEW<TeamCompeteActivityPart>();
					this._GTabControl.AddPageContent(this.mTeamCompeteActivityPart.gameObject, e.Index);
					this.SetPanelPosition();
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[13]);
				this.mTeamCompeteActivityPart = null;
			}
			SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenDetailpart, HelpStateEvents.Clicked, this.RuntimeTabIndexes.IndexOf(e.Index));
		};
		int num = 0;
		if (type == 0)
		{
			this._GTabControl.AddTabPage(4);
			this._Title.text = Global.GetLang("副本");
			this._TitleSprite.spriteName = "fuben";
			this._Bak.URL = Global.GetGameResImageString("richangHuoDongrichangFuBen_Bak.jpg");
			this.RuntimeTabIndexes[0] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("剧情副本"), num);
			num++;
			ActivityTipManager.RegActivityTipItem(2001, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[0];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[0]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			this.RuntimeTabIndexes[1] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("日常副本"), num);
			if (!GongnengYugaoMgr.IsOpenedByCondition(ConfigSystemOpen.GetSystemOpenVOByID(211)))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.RiChangFuBen);
			}
			num++;
			ActivityTipManager.RegActivityTipItem(2003, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[1];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[1]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			this.RuntimeTabIndexes[3] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("万魔塔"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.PaTa))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.PaTa);
			}
			num++;
			this.RuntimeTabIndexes[4] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("跨服副本"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuFuBen))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.KuaFuFuBen);
			}
			num++;
			ActivityTipManager.RegActivityTipItem(2004, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[4];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[4]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
		}
		else
		{
			this._GTabControl.AddTabPage((!Global.g_bIsYaoQingCeShi) ? 9 : 8);
			this._Title.text = Global.GetLang("活动");
			this._TitleSprite.spriteName = "huodong";
			this._Bak.URL = Global.GetGameResImageString("richangHuoDongrichangFuBen_Bak.jpg");
			this.RuntimeTabIndexes[5] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("活动日历"), num);
			num++;
			this.RuntimeTabIndexes[6] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("日常活动"), num);
			num++;
			ActivityTipManager.RegActivityTipItem(1001, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[6];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[6]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			this.RuntimeTabIndexes[7] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("世界BOSS"), num);
			num++;
			ActivityTipManager.RegActivityTipItem(1002, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[7];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[7]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			this.RuntimeTabIndexes[10] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("黄金部队"), num);
			num++;
			ActivityTipManager.RegActivityTipItem(1005, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[10];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[10]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			if (!Global.g_bIsYaoQingCeShi)
			{
				this.RuntimeTabIndexes[8] = num;
				this._GTabControl.SetTabButtonName(Global.GetLang("VIP专属"), num);
				num++;
			}
			this.RuntimeTabIndexes[9] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("古战场"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GuZhanChang))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.GuZhanChang);
			}
			num++;
			this.RuntimeTabIndexes[11] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("水晶幻境"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShuiJingHuanJing))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.ShuiJingHuanJing);
			}
			num++;
			ActivityTipManager.RegActivityTipItem(13000, delegate(int s, ActivityTipItem e)
			{
				if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShuiJingHuanJing))
				{
					int num2 = this.RuntimeTabIndexes[11];
					this._ActivityTipIcons[num2].SetActive(e.IsActive);
					GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[11]];
					if (gbutton != null)
					{
						this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
						this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
					}
				}
			});
			this.RuntimeTabIndexes[12] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("跨服活动"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuHuoDong))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.KuaFuHuoDong);
			}
			num++;
			ActivityTipManager.RegActivityTipItem(1100, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[12];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[12]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			ActivityTipManager.RegActivityTipItem(15010, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[12];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[12]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
				}
			});
			ActivityTipManager.RegActivityTipItem(15011, delegate(int s, ActivityTipItem e)
			{
				int num2 = this.RuntimeTabIndexes[12];
				this._ActivityTipIcons[num2].SetActive(e.IsActive);
				GButton gbutton = this._GTabControl.TabBtns[this.RuntimeTabIndexes[12]];
				if (gbutton != null)
				{
					this._ActivityTipIcons[num2].transform.parent = gbutton.transform;
					this._ActivityTipIcons[num2].transform.localPosition = new Vector3(this._ActivityTipIcons[num2].transform.localPosition.x, 0f, this._ActivityTipIcons[num2].transform.localPosition.z);
				}
			});
			this.RuntimeTabIndexes[13] = num;
			this._GTabControl.SetTabButtonName(Global.GetLang("战队活动"), num);
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.TeamCompeteActivity))
			{
				this._GTabControl.SetMaskBtn(num, GongNengIDs.TeamCompeteActivity);
			}
			num++;
		}
		this.ShowPage(showPage, 0);
		SystemHelpMgr.OnAction(UIObjIDs.HuoDongPart, HelpStateEvents.Actived, 1);
	}

	public void ClickItemBySystem(int TabID = 0)
	{
		if (null != this._RiChangFuBen)
		{
			this._RiChangFuBen.ShowRiChangFuBenDetailPart(ActivityCategorys.JuQingFuBen, TabID);
		}
	}

	public void ShowPage(int site, int param = 0)
	{
		int num = site / 100;
		int num2 = this.RuntimeTabIndexes[num];
		if (num2 < 0)
		{
			this._GTabControl.SetActivePage(0);
			return;
		}
		this._GTabControl.SetActivePage(num2);
		int num3 = site % 100;
		if (num3 != 0)
		{
			num3--;
			if (num == 0)
			{
				this._RiChangFuBen.ShowPage(num3 / 4);
				this._RiChangFuBen.ShowFuBenDetailWindow(num3);
			}
			else if (num != 2)
			{
				if (num == 1)
				{
					this._JingYanFuBen.ShowPage(num3 / 4);
					this._JingYanFuBen.ShowFuBenDetailWindow(num3);
				}
				else if (num == 6)
				{
					this._RiChangHuoDong.ShowPage(num3 / 4);
					this._RiChangHuoDong.ShowHuoDongDetailWindow(num3);
				}
				else if (num == 8)
				{
					this._RiChangVIPHuoDong.ShowPage(num3 / 4);
					this._RiChangVIPHuoDong.ShowHuoDongDetailWindow(num3);
				}
			}
		}
	}

	public void NocticeKuaFuPlunderStateCallBack(KuaFuLueDuoStateData State)
	{
		if (null != this._KuafuActivityPart)
		{
			this._KuafuActivityPart.NocticeKuaFuPlunderStateCallBack(State.GameState);
		}
	}

	private void SetPanelPosition()
	{
	}

	public GButton _Close;

	public ShowNetImage _Bak;

	public UISprite _TitleSprite;

	public RiChangFuBenPart _RiChangFuBen;

	public RiChangFuBenPart _JingYanFuBen;

	public ZuduiFubenPart _ZuDuiFuBen;

	public ZuduiFubenKuaFuPart _ZuduiFubenKuaFuPart;

	public RiChangPaTaPart _PaTaFuBen;

	public HuoDongRiLiPart _HuoDongRiLi;

	public RiChangHuoDongPart _RiChangHuoDong;

	public RiChangVIPHuoDongPart _RiChangVIPHuoDong;

	public RiChangGuZhanChangPart _RiChangGuZhanChangPart;

	public RiChangShuiJingHuanJingPart _RiChangShuiJingHuanJingPart;

	public HuoDongBossPart _ShiJieBoss;

	public HuoDongBossPart _HuangJinBoss;

	public HuoDongBossPart _ShouFeiBoss;

	public KuafuActivityPart _KuafuActivityPart;

	public TeamCompeteActivityPart mTeamCompeteActivityPart;

	public GTabControl _GTabControl;

	public TextBlock _Title;

	public GameObject[] _ActivityTipIcons;

	private int WindowType;

	private int[] RuntimeTabIndexes = new int[14];

	public bool IsOpenZhanMengLianSaiJingCai;
}
