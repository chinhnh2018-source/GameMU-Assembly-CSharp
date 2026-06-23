using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ZuduiFubenPart : UserControl
{
	public int MemberCountMax
	{
		get
		{
			if (this.FubenID == 70200)
			{
				return 10;
			}
			return Global.TeamMaxCount;
		}
	}

	public bool IsAutoStart
	{
		get
		{
			return this.isAutoStart;
		}
	}

	public bool DragTeamListBoxEnable
	{
		get
		{
			return Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.TeamRoles != null && (Global.Data.CurrentCopyTeamData.TeamRoles.Count >= 6 || this.FubenID == 70200);
		}
	}

	private ListBox MemberListBox
	{
		get
		{
			if (this.DragTeamListBoxEnable)
			{
				return this.memberListDragableBox;
			}
			return this.memberListBox;
		}
	}

	private ObservableCollection MemberItemCollection
	{
		get
		{
			if (this.DragTeamListBoxEnable)
			{
				return this.memberItemDragableCollection;
			}
			return this.memberItemCollection;
		}
	}

	private int FubenID
	{
		get
		{
			if (this.fubenListBox.Count() <= 0)
			{
				return 0;
			}
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
			if (zuduiFubtnItem)
			{
				return zuduiFubtnItem.FubenID;
			}
			return 0;
		}
	}

	public static bool IsKuaFuFuBen
	{
		get
		{
			return ActivityCategorys.KuaFuFuBen == ZuduiFubenPart.eCurActivityCategorys;
		}
	}

	public static int ItemCount
	{
		get
		{
			return ZuduiFubenPart.mItemCount;
		}
		set
		{
			ZuduiFubenPart.mItemCount = value;
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.zuduiFuBenPart = null;
		}
		base.OnDestroy();
	}

	private void InitTextInPrefabs()
	{
		this.btnCreate.Text = Global.GetLang("创建房间");
		this.btnQuictJoin.Text = Global.GetLang("快速加入");
		this.lblPrompt.text = Global.GetLang("当前无可加入的房间，您可以创建一个新的房间");
		this.chkForce.Text = Global.GetLang("战力达到:");
		this.chkAutoStart.Text = Global.GetLang("房间人满并全部准备后，自动开始");
		this.ConstText.text = Global.GetLang("的玩家可加入");
		this.btnCreateConfirm.Text = Global.GetLang("确定");
		this.btnQuit.Text = Global.GetLang("退出房间");
		this.ConstText.transform.localPosition = new Vector3(125f, 16f, 0f);
		this.forceInput.transform.localPosition = new Vector3(-75f, 17f, -1f);
		this.m_labRule.X = -160.0;
		this.mCheckBoxAutoStart._Lable.lineWidth = 125;
		this.mCheckBoxAutoKick._Lable.lineWidth = 125;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.thisCtrl = this;
		GameInstance.Game.GetJieriFanbeiInfo();
		this.InitTextInPrefabs();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.zuduiFuBenPart = this;
		}
		this.InitListbox();
		this.InitBtns();
		this.m_BtnRuleClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_rulePart.gameObject.SetActive(false);
		};
		this._SetMemberLstDragVisiable(false);
		if (null != this.fubenSpringPanel)
		{
			this.fubenPanelStartPosX = this.fubenSpringPanel.target.x;
		}
		ZuduiFubenKuaFuPart.ReInitMoRiShenPan();
		this.InitCheckBoxEventHandle();
	}

	private bool IsTeamleader
	{
		get
		{
			return Global.Data != null && Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.RoleID;
		}
	}

	private void InitCheckBoxEventHandle()
	{
		this.mCheckBoxAutoStart._Lable.text = Global.GetLang("人满后自动开始");
		this.mCheckBoxAutoKick._Lable.text = Global.GetLang("30s不准备自动踢");
		if (Global.Data != null && Global.Data.CurrentCopyTeamData != null)
		{
			this.isNewAutoStart = Global.Data.CurrentCopyTeamData.AutoStart;
			this.isAutoKick = (Global.Data.CurrentCopyTeamData.AutoKick == 1);
		}
		this.isAutoStart = this.isNewAutoStart;
		this.ChangeAutoStatus(this.mCheckBoxAutoStart, this.isNewAutoStart);
		if (this.IsTeamleader)
		{
			GameInstance.Game.SpriteCopyTeam(TeamCmds.AutoStart, 0L, 0, (!this.isNewAutoStart) ? 0 : 1, 0);
		}
		this.mCheckBoxAutoStart.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				this.isNewAutoStart = !this.isNewAutoStart;
				this.ChangeAutoStatus(this.mCheckBoxAutoStart, this.isNewAutoStart);
				GameInstance.Game.SpriteCopyTeam(TeamCmds.AutoStart, 0L, 0, (!this.isNewAutoStart) ? 0 : 1, 0);
			}
		};
		this.ChangeAutoStatus(this.mCheckBoxAutoKick, this.isAutoKick);
		if (this.IsTeamleader)
		{
			GameInstance.Game.SpriteCopyTeam(TeamCmds.ChangeKickFlag, 0L, 0, 0, (!this.isAutoKick) ? 0 : 1);
		}
		this.mCheckBoxAutoKick.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				this.isAutoKick = !this.isAutoKick;
				this.ChangeAutoStatus(this.mCheckBoxAutoKick, this.isAutoKick);
				GameInstance.Game.SpriteCopyTeam(TeamCmds.ChangeKickFlag, 0L, 0, 0, (!this.isAutoKick) ? 0 : 1);
			}
		};
	}

	private void ChangeAutoStatus(GCheckBox checkBos, bool isAuto)
	{
		checkBos.checkSprite.spriteName = ((!isAuto) ? "checkbox_cancel" : "checkbox_ok");
		if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.RoleID)
		{
			checkBos._Lable.color = this.GetColor(isAuto);
		}
		else
		{
			checkBos._Lable.color = this.GetColor(false);
		}
	}

	private Color GetColor(bool flag)
	{
		return (!flag) ? NGUIMath.HexToColorEx(8421505U) : NGUIMath.HexToColorEx(14462560U);
	}

	private void ShowAutoStartAndOut(bool isOk)
	{
		this.mCheckBoxAutoStart.gameObject.SetActive(isOk);
		this.mCheckBoxAutoKick.gameObject.SetActive(isOk);
	}

	public void SetCheckState(int index, long sta)
	{
		if (index != 15)
		{
			if (index == 16)
			{
				this.ChangeAutoStatus(this.mCheckBoxAutoStart, sta == 1L);
				Global.Data.CurrentCopyTeamData.AutoStart = (sta == 1L);
			}
		}
		else
		{
			this.ChangeAutoStatus(this.mCheckBoxAutoKick, sta == 1L);
			Global.Data.CurrentCopyTeamData.AutoKick = (int)sta;
		}
	}

	private void InitFubenSpringPanel()
	{
		if (null == this.fubenSpringPanel)
		{
			return;
		}
		this.fubenSpringPanel.onFinished = delegate()
		{
			if (this.isInitFubenList)
			{
				this.fubenSpringPanel.target.x = this.fubenPanelStartPosX;
				this.fubenSpringPanel.enabled = true;
				this.isInitFubenList = false;
			}
		};
	}

	public void InitPartData(ActivityCategorys category)
	{
		if (null != this.fubenSpringPanel)
		{
			this.fubenSpringPanel.target = new Vector3(-369f, 70f, 0f);
		}
		ZuduiFubenPart.eCurActivityCategorys = category;
		Global.CurrentActivityCategorys = category;
		this.InitFubenTabConf(category);
		if (this.fubenTabDict.Count > 0)
		{
			this.InitFubenConf();
			this.InitFubenMapConf();
			this.InitFubenItem(category);
			this.CheckAllItem();
			this.btnCreate.isEnabled = true;
			this.btnQuictJoin.isEnabled = false;
			if (Global.Data.CurrentCopyTeamData == null)
			{
				ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
				if (this.CouldEnterFuben(zuduiFubtnItem))
				{
					this.btnCreate.isEnabled = true;
					GameInstance.Game.SpriteRegCopyTeamEventNotify(zuduiFubtnItem.FubenID, 1);
				}
				else
				{
					this.btnCreate.isEnabled = false;
					this.btnQuictJoin.isEnabled = false;
				}
			}
			else
			{
				ZuduiFubtnItem zuduiFubtnItem2 = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
				if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.SceneIndex != zuduiFubtnItem2.FubenID && this.eLastActivityCategorys == ZuduiFubenPart.eCurActivityCategorys)
				{
					return;
				}
				if (category == this.eLastActivityCategorys)
				{
					this.NotifyCopyTeamMember();
				}
				else
				{
					ZuduiFubtnItem zuduiFubtnItem3 = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
					if (this.CouldEnterFuben(zuduiFubtnItem3))
					{
						this.btnCreate.isEnabled = true;
						GameInstance.Game.SpriteRegCopyTeamEventNotify(zuduiFubtnItem3.FubenID, 1);
					}
					else
					{
						this.btnCreate.isEnabled = false;
						this.btnQuictJoin.isEnabled = false;
					}
				}
			}
		}
		else
		{
			this.btnCreate.isEnabled = false;
			this.btnQuictJoin.isEnabled = false;
			string lang = Global.GetLang("暂无多人副本！");
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
			}, buttons);
		}
		this.eLastActivityCategorys = category;
	}

	private void InitListbox()
	{
		this.FubenItemCollection = this.fubenListBox.ItemsSource;
		this.fubenListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.FuBenSelectedChange);
		this.TeamItemCollection = this.teamListBox.ItemsSource;
		this.memberItemCollection = this.memberListBox.ItemsSource;
		this.memberItemDragableCollection = this.memberListDragableBox.ItemsSource;
		this.memberListDragableBox.ItemPerPage = 4;
	}

	private void InitBtns()
	{
		this.btnQuictJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
			{
				return;
			}
			if (ZuduiFubenPart.IsKuaFuFuBen)
			{
				ZuduiFubenPart.ItemCount = this.fubenListBox.SelectedIndex;
				int trigger = 0;
				int param = 0;
				int param2 = 0;
				if (this.fubenListBox.SelectedIndex == 1 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZuDuiFuBen, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.ZuDuiFuBen, trigger, param, param2, true);
					return;
				}
				if (this.fubenListBox.SelectedIndex == 2)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuansuShiLian, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.YuansuShiLian, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 3)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuWanMoTa, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuWanMoTa, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 4)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoriShenPan, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.MoriShenPan, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 5 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LangHunYaoSai, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.LangHunYaoSai, trigger, param, param2, true);
					return;
				}
			}
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					ZuduiFubtnItem fubenItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
					int zhanLi = 0;
					if (!Global.CanEnterFuBenByZhanLi(fubenItem.FubenID, out zhanLi))
					{
						PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
						PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s3, DPSelectedItemEventArgs e3)
						{
							if (e3.ID == 1)
							{
								PlayZone.GlobalPlayZone.CloseFuBenTiShiPartWindow();
								GameInstance.Game.SpriteCopyTeam(TeamCmds.QuickJoinTeam, (long)fubenItem.FubenID, 0, 0, 0);
							}
						};
						return;
					}
					GameInstance.Game.SpriteCopyTeam(TeamCmds.QuickJoinTeam, (long)fubenItem.FubenID, 0, 0, 0);
				}
			}, -1);
		};
		this.btnCreate.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
			{
				return;
			}
			if (ZuduiFubenPart.IsKuaFuFuBen)
			{
				int trigger = 0;
				int param = 0;
				int param2 = 0;
				ZuduiFubenPart.ItemCount = this.fubenListBox.SelectedIndex;
				if (this.fubenListBox.SelectedIndex == 1 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZuDuiFuBen, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.ZuDuiFuBen, trigger, param, param2, true);
					return;
				}
				if (this.fubenListBox.SelectedIndex == 2)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuansuShiLian, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.YuansuShiLian, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 3)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuWanMoTa, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuWanMoTa, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 4)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoriShenPan, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.MoriShenPan, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 5)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoriShenPan, ref trigger, ref param, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.MoriShenPan, trigger, param, param2, true);
						return;
					}
				}
				else if (this.fubenListBox.SelectedIndex == 5 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LangHunYaoSai, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.LangHunYaoSai, trigger, param, param2, true);
					return;
				}
			}
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
			if (!zuduiFubtnItem.CanSelect)
			{
				return;
			}
			int zhanLi = 0;
			if (!Global.CanEnterFuBenByZhanLi(zuduiFubtnItem.FubenID, out zhanLi))
			{
				PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
				PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s3, DPSelectedItemEventArgs e3)
				{
					if (e3.ID == 1)
					{
						this.CreateTeamWin.SetActive(true);
						this.forceInput.text = string.Empty + Global.Data.roleData.CombatForce;
						PlayZone.GlobalPlayZone.CloseFuBenTiShiPartWindow();
					}
				};
				return;
			}
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					this.CreateTeamWin.SetActive(true);
					this.forceInput.text = string.Empty + Global.Data.roleData.CombatForce;
				}
			}, -1);
		};
		this.btnQuit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
			this.NotifyQuitTeam();
		};
		this.btnReady.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data != null && Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				this.StartFuben(false);
			}
			else
			{
				GameInstance.Game.SpriteCopyTeam(TeamCmds.Ready, (!this.isReady) ? 1L : 0L, 0, 0, 0);
				this.isReady = !this.isReady;
				this.btnReady.Label.text = ((!this.isReady) ? Global.GetLang("准备") : Global.GetLang("取消准备"));
			}
		};
		this.chkForce.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			int num;
			if (this.chkForce.isChecked && this.forceInput.text.Length > 0)
			{
				num = int.Parse(this.forceInput.text);
			}
			else
			{
				num = Global.Data.roleData.CombatForce;
			}
			if (num > Global.Data.roleData.CombatForce)
			{
				Super.HintMainText(Global.GetLang("最低战斗力不能高于自身战斗力"), 10, 3);
				num = Global.Data.roleData.CombatForce;
			}
			this.forceInput.text = string.Empty + num;
		};
		this.btnCreateConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = 0;
			if (this.chkForce.isChecked && this.forceInput.text.Length > 0)
			{
				num = int.Parse(this.forceInput.text);
			}
			if (num > Global.Data.roleData.CombatForce)
			{
				Super.HintMainText(Global.GetLang("最低战斗力不能高于自身战斗力"), 10, 3);
				num = Global.Data.roleData.CombatForce;
				this.forceInput.text = string.Empty + num;
			}
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
			GameInstance.Game.SpriteCopyTeam(TeamCmds.Create, (long)zuduiFubtnItem.FubenID, num, (!this.chkAutoStart.isChecked) ? 0 : 1, (!this.isAutoKick) ? 0 : 1);
			this.isAutoStart = this.chkAutoStart.isChecked;
			this.isNewAutoStart = this.isAutoStart;
			this.CreateTeamWin.SetActive(false);
		};
		this.btnCloseCreateWin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CreateTeamWin.SetActive(false);
		};
		UIEventListener.Get(this.forceInputBak.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DPSelectedForceNum, null, 0, -100);
			if (PlayZone.GlobalPlayZone.InstanceNumKeyboardPart() != null)
			{
				PlayZone.GlobalPlayZone.InstanceNumKeyboardPart().RefreshUI(this.forceInput.label.text);
			}
		};
		this.DPSelectedForceNum = delegate(object s, DPSelectedItemEventArgs e)
		{
			int num = e.ID;
			if (num > Global.Data.roleData.CombatForce)
			{
				Super.HintMainText(Global.GetLang("最低战斗力不能高于自身战斗力"), 10, 3);
				num = Global.Data.roleData.CombatForce;
			}
			this.forceInput.text = string.Empty + num;
		};
	}

	private void InitFubenTabConf(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		this.fubenTabDict.Clear();
		ZuduiFubenPart.FubenTabConf fubenTabConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "FuBenType");
			if (xelementAttributeInt2 == (int)category)
			{
				fubenTabConf = null;
				if (!this.fubenTabDict.TryGetValue(xelementAttributeInt, ref fubenTabConf))
				{
					fubenTabConf = new ZuduiFubenPart.FubenTabConf();
					this.fubenTabDict.Add(xelementAttributeInt, fubenTabConf);
				}
				fubenTabConf.name = Global.GetXElementAttributeStr(xelement, "Name");
				fubenTabConf.Preview = Global.GetXElementAttributeStr(xelement, "Preview");
				fubenTabConf.fuBenType = xelementAttributeInt2;
			}
		}
	}

	private void InitFubenConf()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		this.fubenDict.Clear();
		ZuduiFubenPart.FubenConf fubenConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			if (this.fubenTabDict.ContainsKey(xelementAttributeInt))
			{
				fubenConf = null;
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
				if (!this.fubenDict.TryGetValue(xelementAttributeInt2, ref fubenConf))
				{
					fubenConf = new ZuduiFubenPart.FubenConf();
					this.fubenDict.Add(xelementAttributeInt2, fubenConf);
				}
				fubenConf.TabID = Global.GetXElementAttributeInt(xelement, "TabID");
				fubenConf.zhuanshengLevelNeed = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				fubenConf.levelNeed = Global.GetXElementAttributeInt(xelement, "MinLevel");
				fubenConf.finishNumber = Global.GetXElementAttributeInt(xelement, "FinishNumber");
				fubenConf.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
			}
		}
	}

	private void InitFubenMapConf()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
		if (gameResXml == null)
		{
			return;
		}
		this.fubenMapDict.Clear();
		ZuduiFubenPart.FubenMapConf fubenMapConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "CopyID");
			if (this.fubenDict.ContainsKey(xelementAttributeInt))
			{
				fubenMapConf = null;
				if (!this.fubenMapDict.TryGetValue(xelementAttributeInt, ref fubenMapConf))
				{
					fubenMapConf = new ZuduiFubenPart.FubenMapConf();
					fubenMapConf.Experienceaward = Global.GetXElementAttributeInt(xelement, "Experienceaward");
					fubenMapConf.Moneyaward = Global.GetXElementAttributeInt(xelement, "Moneyaward");
					fubenMapConf.GoodIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
					fubenMapConf.Fenmoaward = Global.GetXElementAttributeInt(xelement, "YuanSuFenMoaward");
					fubenMapConf.YingGuangaward = Global.GetXElementAttributeInt(xelement, "YingGuangaward");
					fubenMapConf.LangHunaward = Global.GetXElementAttributeInt(xelement, "LangHunaward");
					fubenMapConf.TuiJianZhanLi = this.fubenDict[xelementAttributeInt].ZhanLi;
					this.fubenMapDict.Add(xelementAttributeInt, fubenMapConf);
				}
			}
		}
	}

	private void InitFubenItem(ActivityCategorys category)
	{
		this.FubenItemCollection.Clear();
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
		zuduiFubtnItem.SetBakUrl("-1");
		zuduiFubtnItem.ShowFanbei = false;
		zuduiFubtnItem.CanSelect = false;
		zuduiFubtnItem.bak.enabled = false;
		this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
		zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
		UIPanel component = zuduiFubtnItem.GetComponent<UIPanel>();
		if (component)
		{
			Object.Destroy(component);
		}
		if (category == ActivityCategorys.KuaFuFuBen)
		{
			Dictionary<int, ZuduiFubenPart.FubenTabConf> dictionary = new Dictionary<int, ZuduiFubenPart.FubenTabConf>();
			Dictionary<int, ZuduiFubenPart.FubenConf> dictionary2 = new Dictionary<int, ZuduiFubenPart.FubenConf>();
			for (int i = 0; i < this.specialKuaFuFuBenIDs.Length; i++)
			{
				int num = this.specialKuaFuFuBenIDs[i];
				if (this.fubenDict.ContainsKey(num))
				{
					dictionary2.Add(num, this.fubenDict[num]);
				}
			}
			for (int j = 0; j < this.specialKuaFuFuBenTabIDs.Length; j++)
			{
				int num2 = this.specialKuaFuFuBenTabIDs[j];
				if (this.fubenTabDict.ContainsKey(num2))
				{
					dictionary.Add(num2, this.fubenTabDict[num2]);
				}
			}
			if (dictionary2.Count > 0 && dictionary.Count > 0)
			{
				foreach (int num3 in dictionary.Keys)
				{
					foreach (int num4 in dictionary2.Keys)
					{
						if (dictionary2[num4].TabID == num3)
						{
							zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
							zuduiFubtnItem.SetBakUrl(this.fubenTabDict[num3].Preview);
							zuduiFubtnItem.FubenID = num4;
							zuduiFubtnItem.NeedLevel = string.Concat(new object[]
							{
								"[",
								dictionary2[num4].zhuanshengLevelNeed,
								Global.GetLang("转]"),
								dictionary2[num4].levelNeed
							});
							zuduiFubtnItem.FinishNum = string.Empty + ActivityTipManager.GetFubenItemData(num4).FinishNum;
							zuduiFubtnItem.FinishNumLimit = "/" + dictionary2[num4].finishNumber;
							zuduiFubtnItem.RewardExp = string.Empty + this.fubenMapDict[num4].Experienceaward;
							zuduiFubtnItem.RewardMoney = string.Empty + this.fubenMapDict[num4].Moneyaward;
							zuduiFubtnItem.RewardFenMo = this.fubenMapDict[num4].Fenmoaward;
							zuduiFubtnItem.RewardGoods = this.fubenMapDict[num4].GoodIDs;
							zuduiFubtnItem.YingGuangaward = this.fubenMapDict[num4].YingGuangaward;
							zuduiFubtnItem.LangHunaward = this.fubenMapDict[num4].LangHunaward;
							if (dictionary[num3].fuBenType == 1)
							{
								zuduiFubtnItem.Type = 1;
								zuduiFubtnItem.ShowFanbei = Global.isFanbei(5);
							}
							zuduiFubtnItem.CanSelect = true;
							if (dictionary[num3].fuBenType == 9)
							{
								zuduiFubtnItem.m_lblZhanLiStatic.enabled = true;
								zuduiFubtnItem.m_lblZhanLi.text = string.Empty + this.fubenMapDict[num4].TuiJianZhanLi;
								if (Global.Data.roleData.CombatForce < this.fubenMapDict[num4].TuiJianZhanLi)
								{
									zuduiFubtnItem.m_lblZhanLi.text = "{F70101}" + zuduiFubtnItem.m_lblZhanLi.text + "{-}";
								}
								if (zuduiFubtnItem.FubenID != 4000)
								{
									zuduiFubtnItem.btnHelp.gameObject.SetActive(true);
									zuduiFubtnItem.btnHelp.name = zuduiFubtnItem.FubenID.ToString();
									UIEventListener.Get(zuduiFubtnItem.btnHelp.gameObject).onClick = delegate(GameObject s)
									{
										int fubenID = ConvertExt.SafeConvertToInt32(s.name);
										this.ShowRulePart(fubenID);
									};
								}
							}
							else
							{
								zuduiFubtnItem.btnHelp.gameObject.SetActive(false);
							}
							this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
							zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
							zuduiFubtnItem.bak.enabled = false;
							UIPanel component2 = zuduiFubtnItem.GetComponent<UIPanel>();
							if (component2)
							{
								Object.Destroy(component2);
							}
						}
					}
				}
			}
		}
		else
		{
			foreach (int num5 in this.fubenTabDict.Keys)
			{
				foreach (int num6 in this.fubenDict.Keys)
				{
					if (this.fubenDict[num6].TabID == num5)
					{
						zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
						zuduiFubtnItem.SetBakUrl(this.fubenTabDict[num5].Preview);
						zuduiFubtnItem.FubenID = num6;
						zuduiFubtnItem.NeedLevel = string.Concat(new object[]
						{
							"[",
							StringUtil.substitute(Global.GetLang("{0}转"), new object[]
							{
								this.fubenDict[num6].zhuanshengLevelNeed
							}),
							" Lv.",
							this.fubenDict[num6].levelNeed,
							"]"
						});
						zuduiFubtnItem.FinishNum = string.Empty + ActivityTipManager.GetFubenItemData(num6).FinishNum;
						zuduiFubtnItem.FinishNumLimit = "/" + this.fubenDict[num6].finishNumber;
						zuduiFubtnItem.RewardExp = string.Empty + this.fubenMapDict[num6].Experienceaward;
						zuduiFubtnItem.RewardMoney = string.Empty + this.fubenMapDict[num6].Moneyaward;
						zuduiFubtnItem.RewardFenMo = this.fubenMapDict[num6].Fenmoaward;
						zuduiFubtnItem.RewardGoods = this.fubenMapDict[num6].GoodIDs;
						zuduiFubtnItem.YingGuangaward = this.fubenMapDict[num6].YingGuangaward;
						zuduiFubtnItem.LangHunaward = this.fubenMapDict[num6].LangHunaward;
						if (this.fubenTabDict[num5].fuBenType == 1)
						{
							zuduiFubtnItem.Type = 1;
							zuduiFubtnItem.ShowFanbei = Global.isFanbei(5);
						}
						zuduiFubtnItem.CanSelect = true;
						if (this.fubenTabDict[num5].fuBenType == 9)
						{
							zuduiFubtnItem.Type = 9;
							zuduiFubtnItem.m_lblZhanLiStatic.enabled = true;
							zuduiFubtnItem.m_lblZhanLi.text = string.Empty + this.fubenMapDict[num6].TuiJianZhanLi;
							if (Global.Data.roleData.CombatForce < this.fubenMapDict[num6].TuiJianZhanLi)
							{
								zuduiFubtnItem.m_lblZhanLi.text = "{F70101}" + zuduiFubtnItem.m_lblZhanLi.text + "{-}";
							}
							if (zuduiFubtnItem.FubenID != 4000)
							{
								zuduiFubtnItem.btnHelp.gameObject.SetActive(true);
								zuduiFubtnItem.btnHelp.name = zuduiFubtnItem.FubenID.ToString();
								UIEventListener.Get(zuduiFubtnItem.btnHelp.gameObject).onClick = delegate(GameObject s)
								{
									int fubenID = ConvertExt.SafeConvertToInt32(s.name);
									this.ShowRulePart(fubenID);
								};
							}
						}
						else
						{
							zuduiFubtnItem.btnHelp.gameObject.SetActive(false);
						}
						this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
						zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
						zuduiFubtnItem.bak.enabled = false;
						UIPanel component3 = zuduiFubtnItem.GetComponent<UIPanel>();
						if (component3)
						{
							Object.Destroy(component3);
						}
					}
				}
			}
		}
		zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
		zuduiFubtnItem.SetBakUrl("-1");
		zuduiFubtnItem.ShowFanbei = false;
		zuduiFubtnItem.CanSelect = false;
		this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
		zuduiFubtnItem.bak.enabled = false;
		zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
		for (int k = 0; k < 4; k++)
		{
			ZuduiFubtnItem zuduiFubtnItem2 = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[k]);
			zuduiFubtnItem2.bak.enabled = false;
			UIPanel component4 = zuduiFubtnItem2.GetComponent<UIPanel>();
			if (component4)
			{
				Object.Destroy(component4);
			}
		}
		UIPanel component5 = zuduiFubtnItem.GetComponent<UIPanel>();
		if (component5)
		{
			Object.Destroy(component5);
		}
		this.fubenListBox.sorted = true;
		this.fubenListBox.repositionNow = true;
		this.fubenListBox.SelectedIndex = 1;
		this.isInitFubenList = true;
	}

	private void IsNotifyCopyTeamMember(int fubenID)
	{
		if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.SceneIndex == fubenID)
		{
			this.NotifyCopyTeamMember();
		}
	}

	public void NotifyCopyTeamMember()
	{
		this.ResetTeamOrMemberListPabel(false);
		this.lblPrompt.gameObject.SetActive(false);
		if (Global.Data.CurrentCopyTeamData != null)
		{
			this.ShowAutoStartAndOut(true);
			this.SetCheckState(15, (long)Global.Data.CurrentCopyTeamData.AutoKick);
			this.SetCheckState(16, (!Global.Data.CurrentCopyTeamData.AutoStart) ? 0L : 1L);
			this.MemberItemCollection.Clear();
			bool flag = Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID;
			if (flag)
			{
				this.mCheckBoxAutoStart.gameObject.GetComponent<BoxCollider>().enabled = true;
				this.mCheckBoxAutoKick.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				this.mCheckBoxAutoStart.gameObject.GetComponent<BoxCollider>().enabled = false;
				this.mCheckBoxAutoKick.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
			foreach (CopyTeamMemberData copyTeamMemberData in Global.Data.CurrentCopyTeamData.TeamRoles)
			{
				ZuduiMemberItem zuduiMemberItem = U3DUtils.NEW<ZuduiMemberItem>();
				this.MemberItemCollection.AddNoUpdate(zuduiMemberItem);
				zuduiMemberItem.RoleID = copyTeamMemberData.RoleID;
				zuduiMemberItem.MemberName = this.TeamMemberNameCut(copyTeamMemberData.RoleName);
				zuduiMemberItem.MemberForce = string.Empty + copyTeamMemberData.CombatForce;
				zuduiMemberItem.IsCap = (Global.Data.CurrentCopyTeamData.LeaderRoleID == copyTeamMemberData.RoleID);
				zuduiMemberItem.IsReady = copyTeamMemberData.IsReady;
				zuduiMemberItem.FaceImg = StringUtil.substitute("NetImages/RS_Face/{0}.png", new object[]
				{
					Global.CalcOriginalOccupationID(copyTeamMemberData.Occupation)
				});
				if (copyTeamMemberData.RoleID == Global.Data.roleData.RoleID)
				{
					if (Global.Data.roleData.RoleID == Global.Data.CurrentCopyTeamData.LeaderRoleID)
					{
						this.isReady = copyTeamMemberData.IsReady;
						this.btnReady.Label.text = Global.GetLang("开始");
					}
					else
					{
						this.isReady = copyTeamMemberData.IsReady;
						this.btnReady.Label.text = ((!this.isReady) ? Global.GetLang("准备") : Global.GetLang("取消准备"));
					}
				}
				if (this.DragTeamListBoxEnable)
				{
					UIDragObject component = zuduiMemberItem.GetComponent<UIDragObject>();
					component.target = this.memberListDragableBox.transform;
				}
				UIPanel component2 = zuduiMemberItem.GetComponent<UIPanel>();
				if (component2)
				{
					Object.Destroy(component2);
				}
			}
			if (Global.Data.roleData.RoleID == Global.Data.CurrentCopyTeamData.LeaderRoleID && this.MemberItemCollection.Count < this.MemberCountMax)
			{
				ZuduiMemberItem zuduiMemberItem = U3DUtils.NEW<ZuduiMemberItem>();
				zuduiMemberItem.RoleID = -1;
				this.MemberItemCollection.AddNoUpdate(zuduiMemberItem);
				if (this.DragTeamListBoxEnable)
				{
					UIDragObject component3 = zuduiMemberItem.GetComponent<UIDragObject>();
					component3.target = this.memberListDragableBox.transform;
				}
				UIPanel component4 = zuduiMemberItem.GetComponent<UIPanel>();
				if (component4)
				{
					Object.Destroy(component4);
				}
				zuduiMemberItem.ShowTrans = ZuduiMemberItem.ShowType.eYaoQingZuDui;
			}
			if (this.DragTeamListBoxEnable)
			{
				this.memberItemDragableCollection.DelayUpdate();
				long num = ConfigSystemParam.GetSystemParamIntByName("LangHunYaoSaiTeamMaxNum");
				if (num <= 0L)
				{
					num = 10L;
				}
				this.lblMemNum.text = string.Format("{0}/{1}", this.TeamRolesNumber(), num);
			}
		}
		if (Global.Data.roleData != null && Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
		{
			this.StartFuben(true);
		}
	}

	private int TeamRolesNumber()
	{
		if (Global.Data.roleData != null && Global.Data.CurrentCopyTeamData != null)
		{
			return Global.Data.CurrentCopyTeamData.TeamRoles.Count;
		}
		return this.MemberItemCollection.Count;
	}

	private void DebugDicts()
	{
	}

	public void NotifyCopyTeamData(CopySearchTeamData searchTeamData)
	{
		if (this.MemberItemCollection.Count > 0)
		{
			return;
		}
		if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.SceneIndex == U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox.SelectedItem).FubenID)
		{
			this.lblPrompt.gameObject.SetActive(false);
			return;
		}
		this.ResetTeamOrMemberListPabel(true);
		if (searchTeamData != null && searchTeamData.TeamDataList != null)
		{
			this.TeamItemCollection.Clear();
			foreach (CopyTeamData copyTeamData in searchTeamData.TeamDataList)
			{
				ZuduiTeamItem zuduiTeamItem = U3DUtils.NEW<ZuduiTeamItem>();
				this.TeamItemCollection.AddNoUpdate(zuduiTeamItem);
				zuduiTeamItem.CopyTeamID = copyTeamData.TeamID;
				zuduiTeamItem.TeamName = this.TeamMemberNameCut(copyTeamData.TeamName) + Global.GetLang("的房间");
				zuduiTeamItem.ForceRequire = string.Empty + copyTeamData.MinZhanLi;
				zuduiTeamItem.MemberCount = string.Empty + copyTeamData.MemberCount;
				zuduiTeamItem.MemberCountMax = string.Empty + this.MemberCountMax;
				zuduiTeamItem.FuBenID = copyTeamData.SceneIndex;
				UIPanel component = zuduiTeamItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
		else
		{
			this.TeamItemCollection.Clear();
		}
		bool flag = this.teamListBox.Count() > 0;
		if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
		{
			this.lblPrompt.gameObject.SetActive(false);
		}
		else
		{
			this.lblPrompt.gameObject.SetActive(!flag);
		}
		this.btnQuictJoin.isEnabled = flag;
		if (searchTeamData == null || searchTeamData.TeamDataList == null || searchTeamData.TeamDataList.Count <= 0)
		{
			this.lblPrompt.gameObject.SetActive(true);
		}
	}

	public void NotifyCopyTeamData(int fubenId, long teamId, string leaderName, int memberCount, int forceRequire)
	{
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
		if (zuduiFubtnItem.FubenID == fubenId)
		{
			int num = this.teamListBox.Count();
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				ZuduiTeamItem zuduiTeamItem = U3DUtils.AS<ZuduiTeamItem>(this.teamListBox[i]);
				if (zuduiTeamItem.CopyTeamID == teamId)
				{
					if (memberCount <= 0)
					{
						this.TeamItemCollection.RemoveAt(i);
					}
					else
					{
						zuduiTeamItem.MemberCount = string.Empty + memberCount;
						zuduiTeamItem.FuBenID = fubenId;
						zuduiTeamItem.MemberCountMax = string.Empty + this.MemberCountMax;
					}
					flag = true;
					break;
				}
			}
			if (!flag && memberCount > 0)
			{
				ZuduiTeamItem zuduiTeamItem2 = U3DUtils.NEW<ZuduiTeamItem>();
				this.TeamItemCollection.AddNoUpdate(zuduiTeamItem2);
				zuduiTeamItem2.CopyTeamID = teamId;
				zuduiTeamItem2.TeamName = this.TeamMemberNameCut(leaderName) + Global.GetLang("的房间");
				zuduiTeamItem2.ForceRequire = string.Empty + forceRequire;
				zuduiTeamItem2.MemberCount = string.Empty + memberCount;
				zuduiTeamItem2.MemberCountMax = string.Empty + this.MemberCountMax;
				zuduiTeamItem2.FuBenID = fubenId;
				UIPanel component = zuduiTeamItem2.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
		bool flag2 = this.teamListBox.Count() > 0;
		if (Global.Data.CurrentCopyTeamData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
		{
			this.lblPrompt.gameObject.SetActive(false);
		}
		else
		{
			this.lblPrompt.gameObject.SetActive(!flag2);
		}
		this.btnQuictJoin.isEnabled = flag2;
		if (!flag2)
		{
			this.ShowAutoStartAndOut(false);
		}
	}

	public void NotifyCreateTeam(long teamId)
	{
	}

	public void NotifyQuitTeam()
	{
		this.ResetTeamOrMemberListPabel(true);
		this.ShowAutoStartAndOut(false);
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
		GameInstance.Game.SpriteRegCopyTeamEventNotify(zuduiFubtnItem.FubenID, 1);
	}

	public void NotifyJoinTeam()
	{
	}

	public void NotifyKickOut()
	{
		this.ResetTeamOrMemberListPabel(true);
		this.ShowAutoStartAndOut(false);
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
		GameInstance.Game.SpriteRegCopyTeamEventNotify(zuduiFubtnItem.FubenID, 1);
	}

	public void NotifyTeamMemberReadyState(int roleId, bool isReady)
	{
		int i = 0;
		bool flag = false;
		while (i < Global.Data.CurrentCopyTeamData.TeamRoles.Count)
		{
			if (roleId == Global.Data.CurrentCopyTeamData.TeamRoles[i].RoleID)
			{
				Global.Data.CurrentCopyTeamData.TeamRoles[i].IsReady = isReady;
				flag = true;
				break;
			}
			i++;
		}
		if (flag)
		{
			ZuduiMemberItem zuduiMemberItem = U3DUtils.AS<ZuduiMemberItem>(this.MemberListBox[i]);
			zuduiMemberItem.IsReady = isReady;
		}
		if (Global.Data.roleData != null && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
		{
			this.StartFuben(true);
		}
	}

	public void ResetTeamPanel()
	{
		this.MemberItemCollection.Clear();
		this.ShowAutoStartAndOut(false);
		this.ResetTeamOrMemberListPabel(true);
		this.lblPrompt.gameObject.SetActive(true);
		this.btnQuictJoin.isEnabled = false;
	}

	private void ResetTeamOrMemberListPabel(bool bTeamLstVisiable)
	{
		this._SetTeamLstVisiable(bTeamLstVisiable);
		this._SetMemberLstDefaultVisiable(!bTeamLstVisiable);
		this._SetMemberLstDragVisiable(!bTeamLstVisiable);
		this._SetMemberDefautButtonsVisiable(!bTeamLstVisiable);
		if (!bTeamLstVisiable)
		{
			if (this.DragTeamListBoxEnable)
			{
				this._SetMemberLstDefaultVisiable(false);
				this._SetMemberLstDragVisiable(true);
				this._SetMemberDefautButtonsVisiable(true);
			}
			else
			{
				this._SetMemberLstDefaultVisiable(true);
				this._SetMemberLstDragVisiable(false);
				this._SetMemberDefautButtonsVisiable(true);
			}
		}
		else
		{
			this.btnCreate.isEnabled = true;
			this.btnQuictJoin.isEnabled = false;
			if (Global.Data.CurrentCopyTeamData == null)
			{
				ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
				if (this.CouldEnterFuben(zuduiFubtnItem))
				{
					this.btnCreate.isEnabled = true;
					this.btnQuictJoin.isEnabled = true;
				}
				else
				{
					this.btnCreate.isEnabled = false;
					this.btnQuictJoin.isEnabled = false;
				}
			}
		}
		BoxCollider component = this.panelFuben.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = !bTeamLstVisiable;
		}
	}

	private void _SetTeamLstVisiable(bool visiable)
	{
		this.teamListBox.gameObject.SetActive(visiable);
		this.btnCreate.gameObject.SetActive(visiable);
		this.btnQuictJoin.gameObject.SetActive(visiable);
	}

	private void _SetMemberDefautButtonsVisiable(bool visiable)
	{
		this.btnQuit.gameObject.SetActive(visiable);
		this.btnQuit.isEnabled = true;
		this.btnReady.gameObject.SetActive(visiable);
	}

	private void _SetMemberLstDefaultVisiable(bool visiable)
	{
		this.memberListBox.gameObject.SetActive(visiable);
	}

	private void _SetMemberLstDragVisiable(bool visiable)
	{
		this.pnlDragTeamMember.gameObject.SetActive(visiable);
	}

	public void OnCoutDownStart()
	{
		if (ZuduiFubenPart.IsKuaFuFuBen && this.btnQuit != null)
		{
			this.btnQuit.isEnabled = false;
		}
	}

	private bool CanStartFuben()
	{
		bool flag = false;
		foreach (CopyTeamMemberData copyTeamMemberData in Global.Data.CurrentCopyTeamData.TeamRoles)
		{
			flag = copyTeamMemberData.IsReady;
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	private bool StartFuben(bool autoStart = false)
	{
		if (autoStart && !this.IsAutoStart)
		{
			return false;
		}
		if (autoStart && Global.Data.CurrentCopyTeamData.TeamRoles.Count != this.MemberCountMax)
		{
			return false;
		}
		if (this.CanStartFuben())
		{
			if (Global.Data.CurrentCopyTeamData.AutoStart)
			{
				ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
				Global.SendEvent("2900", Global.GetLang("多人副本挑战次数"));
				GameInstance.Game.SpriteEnterFuBen(zuduiFubtnItem.FubenID);
			}
			else if (!autoStart)
			{
				ZuduiFubtnItem zuduiFubtnItem2 = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
				Global.SendEvent("2900", Global.GetLang("多人副本挑战次数"));
				GameInstance.Game.SpriteEnterFuBen(zuduiFubtnItem2.FubenID);
			}
			return true;
		}
		if (!autoStart)
		{
			string lang = Global.GetLang("所有人均为准备状态才能开启副本！");
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
				}
			}, buttons);
		}
		return false;
	}

	private bool CouldEnterFuben(ZuduiFubtnItem fubenItem)
	{
		int fubenID = fubenItem.FubenID;
		bool flag = false;
		bool flag2 = false;
		if (this.fubenDict.ContainsKey(fubenID))
		{
			ZuduiFubenPart.FubenConf fubenConf = this.fubenDict[fubenID];
			if (Global.Data.roleData.ChangeLifeCount > fubenConf.zhuanshengLevelNeed)
			{
				flag = true;
			}
			else if (Global.Data.roleData.ChangeLifeCount == fubenConf.zhuanshengLevelNeed && Global.Data.roleData.Level >= fubenConf.levelNeed)
			{
				flag = true;
			}
			else
			{
				fubenItem.NeedLevel = "{ff0000}" + fubenItem.NeedLevel + "{-}";
			}
			if (ActivityTipManager.GetFubenItemData(fubenID).FinishNum < this.fubenDict[fubenID].finishNumber)
			{
				flag2 = true;
			}
			else
			{
				fubenItem.FinishNum = "{ff0000}" + fubenItem.FinishNum + "{-}";
				fubenItem.FinishNumLimit = "{ff0000}" + fubenItem.FinishNumLimit + "{-}";
			}
		}
		return flag && flag2;
	}

	public static void ProcessErrorCode(ZuduiFubenPart.ZuduiErrCode err, bool bShowMessageBoxEx = true)
	{
		string text = string.Empty;
		switch (err + 18)
		{
		case (ZuduiFubenPart.ZuduiErrCode)0:
			text = Global.GetLang("服务器繁忙");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)2:
			text = Global.GetLang("对方不在本房间");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)3:
			text = Global.GetLang("房间已经开始游戏");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)4:
			text = Global.GetLang("跨服中心服务器异常");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)5:
			text = Global.GetLang("服务器异常");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)6:
			text = Global.GetLang("被踢出房间");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)7:
			text = Global.GetLang("离开房间");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)11:
			text = Global.GetLang("没有合适的房间");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)12:
			text = Global.GetLang("未达到房间的最低战力要求");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)13:
			text = Global.GetLang("房间已满");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)14:
			text = Global.GetLang("你不是房主(房主才能执行)");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)15:
			text = Global.GetLang("已有房间");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)16:
			text = Global.GetLang("房间已解散");
			break;
		case (ZuduiFubenPart.ZuduiErrCode)17:
			text = Global.GetLang("你已经不在房间中");
			break;
		}
		if (bShowMessageBoxEx)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), text, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
				}
			}, buttons);
		}
		else if (err != ZuduiFubenPart.ZuduiErrCode.ERR_SUCCESS)
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	public void QuitTeam()
	{
		GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
		this.NotifyQuitTeam();
		this.teamListBox.Clear();
	}

	private void FuBenSelectedChange(object sender, MouseEvent e)
	{
		this.SelectetedFubenItem();
	}

	private void SelectetedFubenItem()
	{
		this.MemberItemCollection.Clear();
		this.ShowAutoStartAndOut(false);
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox.SelectedItem);
		if (null == zuduiFubtnItem)
		{
			return;
		}
		if (!zuduiFubtnItem.CanSelect)
		{
			this.btnCreate.gameObject.SetActive(true);
			this.btnCreate.isEnabled = false;
			this.btnQuictJoin.gameObject.SetActive(true);
			this.btnQuictJoin.isEnabled = false;
			this.btnReady.gameObject.SetActive(false);
			this.btnQuit.gameObject.SetActive(false);
			this.currentFubenIdx = this.fubenListBox.SelectedIndex;
			this.lblPrompt.gameObject.SetActive(true);
			this.TeamItemCollection.Clear();
			this.preFubenIdx = 0;
			this._SetMemberLstDragVisiable(false);
			return;
		}
		if (this.fubenItem != null && this.fubenItem != zuduiFubtnItem)
		{
			this.fubenItem.bak.enabled = false;
		}
		this.fubenItem = zuduiFubtnItem;
		this.fubenItem.bak.enabled = true;
		if (CommonFlagX.HaveFlagData("team"))
		{
			CommonFlagX.CommonFlagData flagData = CommonFlagX.GetFlagData("team");
			if (flagData != null || flagData.refdata != null)
			{
				this.checkFubenItem(this.fubenListBox.SelectedIndex);
				this.UpdateBtnState(zuduiFubtnItem);
				this.IsNotifyCopyTeamMember(zuduiFubtnItem.FubenID);
				return;
			}
		}
		this.checkFubenItem(this.fubenListBox.SelectedIndex);
		this.UpdateBtnState(zuduiFubtnItem);
		this.IsNotifyCopyTeamMember(zuduiFubtnItem.FubenID);
	}

	public void UpdateBtnState(ZuduiFubtnItem item)
	{
		if (Global.Data.CurrentCopyTeamData != null && item.FubenID != Global.Data.CurrentCopyTeamData.SceneIndex)
		{
			this.ShowTeamListWidget(true);
		}
	}

	private void ShowTeamListWidget(bool show)
	{
		this.MemberItemCollection.Clear();
		this.ShowAutoStartAndOut(false);
		this.teamListBox.gameObject.SetActive(show);
		this.memberListBox.gameObject.SetActive(!show);
		this.btnCreate.gameObject.SetActive(show);
		this.btnQuictJoin.gameObject.SetActive(show);
		this.btnQuit.gameObject.SetActive(!show);
		this.btnReady.gameObject.SetActive(!show);
		this.ShowAutoStartAndOut(!show);
		BoxCollider component = this.panelFuben.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = !show;
		}
	}

	private void checkFubenItem(int selected)
	{
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[selected]);
		if (this.CouldEnterFuben(zuduiFubtnItem))
		{
			this.currentFubenIdx = selected;
			this.btnCreate.isEnabled = true;
			this.currentFubenIdx = selected;
			if (this.preFubenIdx != this.currentFubenIdx)
			{
				this.preFubenIdx = this.currentFubenIdx;
				GameInstance.Game.SpriteRegCopyTeamEventNotify(zuduiFubtnItem.FubenID, 1);
			}
		}
		else
		{
			if (this.IsOutOfCount(zuduiFubtnItem))
			{
				this.currentFubenIdx = selected;
			}
			this.preFubenIdx = selected;
			this.TeamItemCollection.Clear();
			this.btnCreate.isEnabled = false;
			this.btnQuictJoin.isEnabled = false;
		}
	}

	private bool IsOutOfCount(ZuduiFubtnItem fubenItem)
	{
		int fubenID = fubenItem.FubenID;
		bool result = true;
		if (this.fubenDict.ContainsKey(fubenID))
		{
			ZuduiFubenPart.FubenConf fubenConf = this.fubenDict[fubenID];
			if (ActivityTipManager.GetFubenItemData(fubenID).FinishNum < this.fubenDict[fubenID].finishNumber)
			{
				result = false;
			}
		}
		return result;
	}

	private void CheckAllItem()
	{
		for (int i = 0; i < this.fubenListBox.Count(); i++)
		{
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[i]);
			this.CouldEnterFuben(zuduiFubtnItem);
		}
	}

	private string TeamMemberNameCut(string strName)
	{
		if (ZuduiFubenPart.IsKuaFuFuBen)
		{
			string text = strName;
			int num = text.IndexOf(']');
			if (num > 0 && num < text.Length)
			{
				text = text.Insert(num + 1, "\r\n");
			}
			return text;
		}
		return strName;
	}

	private void ShowRulePart(int fubenID)
	{
		if (fubenID == 4000)
		{
			this.fubenItem.btnHelp.gameObject.SetActive(false);
			return;
		}
		this.m_labRule.transform.localScale = Vector3.one * 18f;
		if (fubenID == 70000)
		{
			this.m_labRule.Text = Global.GetLang("{FFD35F}【规则介绍】{-}\r\n1、副本4个角落会依次出现{00FF00}天启四骑士{-}，四骑士\r\n全部被击杀后 在中间会召唤最终BOSS-{00FF00}末日审判{-}.\r\n击杀末日审判副本通关。\r\n2、每在{00FF00}规定时间{-}内击杀一个骑士，最终奖励\r\n{00FF00}提升50%{-}，但是最终BOSS末日审判的攻击、防御、\r\n生命也会相应提高，{FF0000}需要挑战者们量力而行{-}。");
			this.m_labRule.Text = "  {FFD35F}[Quy tắc]{-}\r\n    1, Bốn góc phó bản lần lượt xuất hiện {00FF00}4 kỵ sĩ tận thế{-}, tất cả\r\n       4 kỵ sĩ bị kích bại, ở giữa sẽ triệu hồi BOSS -{00FF00}Phán Quyết{-}.\r\n       Kích sát Boss Phán Quyết để vượt ải.\r\n    2, Trong mỗi {00FF00}thời gian quy định{-} kích sát 1 kỵ sĩ, quà cuối cùng\r\n       {00FF00}tăng 50%{-}, nhưng BOSS Phán Quyết công kích, phòng ngự,\r\n       HP cũng tăng tương ứng, {FF0000}cần năng lực của các Dũng Sĩ{-}.";
		}
		else if (fubenID == 70100)
		{
			this.m_labRule.Text = Global.GetLang("{FFD35F}【规则介绍】{-}\\r\\n\r\n1、战斗开始后副本中会{00FF00}不断刷新怪物{-}，在规定\r\n时间内杀光一波刷新下一波，若{00FF00}用时较少{-}波数\r\n可一次提升{00FF00}1至4波{-}，规定时间内{FF0000}未全部击杀{-}则\r\n副本结束。\r\n2、每新刷一波时间会被{00FF00}重置{-}，波数提高怪物的\r\n属性也随之提高，副本最大为{00FF00}30{-}波，{00FF00}波数越多\r\n最终的奖励越高{-}。\r\n{00FF00}提示： {-}元素恐惧者有{FF0000}极高的防御{-}，荧石的{00FF00}元素攻击{-}\r\n可对其造成大量伤害。");
			this.m_labRule.Text = "  {FFD35F}[Quy tắc]{-}\r\n    1, Phó bản sẽ {00FF00}không ngừng làm mới quái{-}, trong thời gian quy \r\n       định giết hết 1 đợt sẽ được làm mới, nếu {00FF00}thời gian dùng ít {-}\r\n       số đợt có thể tăng {00FF00}1 đến 4 đợt{-}, trong thời gian quy định\r\n       {FF0000}chưa kích sát hết {-}thì phó bản sẽ kết thúc.\r\n    2, Mỗi đợt mới thời gian sẽ {00FF00}thiết lập lại{-}, số đợt tăng thuộc tính \r\n       quái cũng tăng, cao nhất là {00FF00}30{-} đợt, {00FF00}số đợt càng cao\r\n       quà cuối cùng càng cao{-}.\r\n       {00FF00}Cảnh báo: {-}Kẻ sợ hãi có {FF0000}tăng phòng ngự{-}, Huỳnh Quang của\r\n       {00FF00}Kẻ tấn công{-} có thể tạo sát thương lớn.";
		}
		else if (fubenID == 70200)
		{
			this.m_labRule.Text = Global.GetLang("{FFD35F}【规则介绍】{-}1、战斗开始后副本中会有3个点不断{00FF00}刷新怪物{-}，共计15波怪物，怪物会{FF0000}攻击狼魂要塞{-}。2、玩家需要{00FF00}保护狼魂要塞{-}，狼魂要塞被击破则副本结束。3、根据个人{00FF00}击杀怪物个数{-}、{00FF00}副本用时{-}、{00FF00}狼魂要塞剩余血量{-}计算最终积分，积分越多最终奖励越好。");
		}
		else if (fubenID == 70300)
		{
			this.m_labRule.Text = "  {FFD35F}[Giới thiệu quy tắc]{-}\r\n    1, Sau khi trận chiến bắt đầu, trong PB sẽ có 3 điểm liên tục \r\n       {00FF00}làm mới quái{-}, tổng cộng 15 đợt quái, quái sẽ {FF0000}tấn công \r\n       cứ điểm quan trọng Hồn Sói{-}.\r\n    2, Người chơi cần {00FF00}bảo vệ cứ điểm quan trọng Hồn Sói{-}, Pháo\r\n       Đài Sói bị đánh phá thì PB sẽ kết thúc.\r\n    3, Dựa vào {00FF00}số quái cá nhân tiêu diệt{-}, {00FF00}thời gian dùng PB{-},\r\n       {00FF00}HP Pháo Đài Sói còn{-} sẽ tính ra số điểm cuối cùng, điểm\r\n       càng nhiều thưởng cuối càng nhiều.";
		}
		else
		{
			this.m_labRule.Text = string.Empty;
		}
		this.m_rulePart.gameObject.SetActive(true);
	}

	public void InitFanbei()
	{
		for (int i = 0; i < this.fubenListBox.Count(); i++)
		{
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[i]);
			if (zuduiFubtnItem.Type == 1)
			{
				zuduiFubtnItem.ShowFanbei = Global.isFanbei(5);
			}
		}
	}

	internal void ShowPageByFubenID(int fubenID)
	{
		ZuduiFubtnItem zuduiFubtnItem = null;
		for (int i = 0; i < this.fubenListBox.Count(); i++)
		{
			zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[i]);
			if (zuduiFubtnItem.FubenID == fubenID)
			{
				this.fubenListBox.SelectedIndex = i;
				this.currentFubenIdx = i;
				break;
			}
			zuduiFubtnItem = null;
		}
		if (zuduiFubtnItem == null)
		{
			return;
		}
		this.fubenSpringPanel.target.x = this.fubenPanelStartPosX - (float)((this.fubenListBox.SelectedIndex - 1) * 438);
		this.fubenSpringPanel.enabled = true;
		this.isInitFubenList = false;
	}

	public static void SendSpriteCopyTeam(TeamCmds teamCmdType, long ID, int forceRequire, int autoStart)
	{
		GameInstance.Game.SpriteCopyTeam(teamCmdType, ID, forceRequire, autoStart, 0);
	}

	public const int DRAG_TEAMMEMBER_NUM_MIN = 6;

	private DPSelectedItemEventHandler DPSelectedForceNum;

	private SpriteSL thisCtrl;

	public GButton btnQuictJoin;

	public GButton btnCreate;

	public GButton btnQuit;

	public GButton btnReady;

	public UIPanel panelFuben;

	public SpringPanel fubenSpringPanel;

	private bool isInitFubenList;

	public UILabel lblPrompt;

	public ListBox fubenListBox;

	public ListBox teamListBox;

	public ListBox memberListBox;

	private ObservableCollection FubenItemCollection;

	private ObservableCollection TeamItemCollection;

	private ObservableCollection memberItemCollection;

	public GameObject CreateTeamWin;

	public GButton btnCreateConfirm;

	public GButton btnCloseCreateWin;

	public GCheckBox chkForce;

	public GCheckBox chkAutoStart;

	public UILabel ConstText;

	public UIInput forceInput;

	public UISprite forceInputBak;

	public GButton m_BtnRuleClose;

	public TextBlock m_labRule;

	public Transform m_rulePart;

	public UIPanel pnlDragTeamMember;

	public ListBox memberListDragableBox;

	private ObservableCollection memberItemDragableCollection;

	public UILabel lblMemNum;

	public GCheckBox mCheckBoxAutoStart;

	public GCheckBox mCheckBoxAutoKick;

	private bool isAutoStart;

	public UICenterOnChild centerController;

	private int currentFubenIdx;

	private int preFubenIdx;

	private float fubenPanelStartPosX;

	private bool isReady;

	public ActivityCategorys eLastActivityCategorys = ActivityCategorys.RiChangFuBen;

	private static ActivityCategorys eCurActivityCategorys = ActivityCategorys.ZuDuiFuBen;

	private static int mItemCount;

	private Dictionary<int, ZuduiFubenPart.FubenTabConf> fubenTabDict = new Dictionary<int, ZuduiFubenPart.FubenTabConf>();

	private Dictionary<int, ZuduiFubenPart.FubenConf> fubenDict = new Dictionary<int, ZuduiFubenPart.FubenConf>();

	private Dictionary<int, ZuduiFubenPart.FubenMapConf> fubenMapDict = new Dictionary<int, ZuduiFubenPart.FubenMapConf>();

	private bool isNewAutoStart = true;

	private bool isAutoKick = true;

	private int[] specialKuaFuFuBenIDs = new int[]
	{
		4000,
		70100,
		70300,
		70000,
		70200
	};

	private int[] specialKuaFuFuBenTabIDs = new int[]
	{
		701,
		801,
		803,
		800,
		802
	};

	private ZuduiFubtnItem fubenItem;

	public enum ZuduiErrCode
	{
		ERR_SUCCESS = 1,
		ERR_NO_TEAM = -1,
		ERR_TEAM_IS_DESTORYED = -2,
		ERR_ALLREADY_HAS_TEAM = -3,
		ERR_NOT_TEAM_LEADER = -4,
		ERR_TEAM_IS_FULL = -5,
		ERR_FORCE_LOW = -6,
		ERR_NO_ACCEPTABLE_TEAM = -7,
		ERR_LEAVE_TEAM = -11,
		ERR_KICK_OUT = -12,
		ServerException = -13,
		CenterServerFailed = -14,
		TeamAlreadyStart = -15,
		NotInMyTeam = -16,
		MemeberNotReady = -17,
		KFServerIsBusy = -18
	}

	private class FubenTabConf
	{
		public string name = string.Empty;

		public string Preview = string.Empty;

		public int fuBenType;
	}

	private class FubenConf
	{
		public int TabID;

		public int zhuanshengLevelNeed;

		public int levelNeed;

		public int finishNumber;

		public int ZhanLi;
	}

	private class FubenMapConf
	{
		public int Moneyaward;

		public int Experienceaward;

		public string GoodIDs;

		public int Fenmoaward = -1;

		public int TuiJianZhanLi;

		public int YingGuangaward;

		public int LangHunaward;
	}
}
