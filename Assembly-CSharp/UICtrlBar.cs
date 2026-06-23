using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class UICtrlBar : UserControl
{
	public bool IsShowSkill
	{
		get
		{
			return this.Ctrl1.Visibility;
		}
		set
		{
			this.Ctrl1.Visibility = value;
			this.BianShenIcon.gameObject.SetActive(value);
		}
	}

	public static UICtrlBar singleton
	{
		get
		{
			return UICtrlBar.ms_Singleton;
		}
	}

	public bool IsSafeRegion
	{
		set
		{
			this.m_beInSafe = value;
			this.m_beCanSendBianShen = true;
			if (value)
			{
				this.BianShenTabs.SetActive(false);
				this.NormalSkillTabs.SetActive(false);
				this.Ctrl1.Visibility = false;
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu)
				{
					this.Ctrl0.Visibility = false;
				}
				else
				{
					this.Ctrl0.Visibility = true;
				}
				if (this.Ctrl0.Visibility && !SceneUIClasses.RebornMap.IsTheScene() && 0 < this.mAnNiuPosDic.Count)
				{
					for (int i = 0; i < this.Ctrl0.Children.Count(); i++)
					{
						GameObject childAt = this.Ctrl0.Children.getChildAt(i);
						if (null != childAt && !childAt.name.Equals("DuihuaIcon") && this.mAnNiuPosDic.ContainsKey(childAt.name))
						{
							childAt.transform.localPosition = this.mAnNiuPosDic[childAt.name];
						}
					}
				}
			}
			else
			{
				this.Ctrl0.Visibility = false;
				this.Ctrl1.Visibility = true;
				if (!ShenHunData.IsInBianShenState())
				{
					this.BianShenTabs.SetActive(false);
					this.NormalSkillTabs.SetActive(true);
					this.RefreshSkillIcon();
					this.RefershBianShenButton();
				}
				else
				{
					this.BianShenTabs.SetActive(true);
					this.NormalSkillTabs.SetActive(false);
					this.InitBianShenSkill();
				}
			}
		}
	}

	public void RefershBianShenButton()
	{
		if (!UICtrlBar.IsShenHunFuTiOpened() || !UICtrlBar.IsMapCanBianShen() || Global.IsInDaTaoShaPrepare() || (Global.IsInDaTaoSha() && DaTaoShaDataManager.EBattleStatus <= EscapeBattleGameSceneStatuses.STATUS_BEGIN))
		{
			this.BianShenIcon.gameObject.SetActive(false);
		}
		else
		{
			this.BianShenIcon.UpdateInBianShenState();
			this.BianShenIcon.gameObject.SetActive(true);
			this.BianShenIcon.UpdateNumInfo();
		}
	}

	public static bool IsShenHunFuTiOpened()
	{
		return ShenHunData.IsShenHunOpen();
	}

	public static bool IsMapCanBianShen()
	{
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
		return settingMapVOByCode.Transfiguration == 1;
	}

	public void FreshPlayerHideMoreIcon(bool show)
	{
	}

	private void RepeatSkillIcon()
	{
		if (null != this.SkillIconPressed)
		{
			if (U3DUtils.ComponentIsEnabled(this.SkillIconPressed) && this.SkillIconPressed.IsPressed)
			{
				if (Input.acceleration.sqrMagnitude < 60f)
				{
					this.PressSkillIcon(this.SkillIconPressed);
				}
			}
			else
			{
				this.CancelAutoUseSkill();
			}
		}
		else
		{
			this.CancelAutoUseSkill();
		}
	}

	private void PressSkillIcon(GSkillIcon skillIcon)
	{
		long skillCoolDownTicks = Global.GetSkillCoolDownTicks(skillIcon.TagCode);
		if (skillCoolDownTicks > 0L)
		{
			return;
		}
		if (ConfigMagicInfos.IsHorseSkill(skillIcon.TagCode))
		{
			List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
			if (roleHorseSkillData != null && 0 < roleHorseSkillData.Count)
			{
				int i = 0;
				int count = roleHorseSkillData.Count;
				while (i < count)
				{
					long skillCoolDownTicks2 = Global.GetSkillCoolDownTicks(roleHorseSkillData[i].SkillID);
					if (0L < skillCoolDownTicks2)
					{
						return;
					}
					i++;
				}
			}
		}
		if (skillIcon.TagCode < 0)
		{
			this.ShowSkillSelectPart(skillIcon.TagIndex);
			return;
		}
		if (skillIcon.TagIndex != 4 || skillIcon.EnableState)
		{
			if (this.UICtrlBarNotify != null)
			{
				if (ShenHunData.IsInBianShenState())
				{
					this.UICtrlBarNotify(UICtrlBarTypes.BianShenSkillButton, skillIcon.TagIndex);
				}
				else
				{
					this.UICtrlBarNotify(UICtrlBarTypes.SkillButton, skillIcon.TagIndex);
				}
			}
			if (SystemHelpMgr.HintTextForSkillIcon >= 0)
			{
				if (SystemHelpMgr.HintTextForSkillIcon == 2)
				{
					if (skillIcon.TagIndex == 2)
					{
						this.ShowHelpAnim(1, 1);
					}
				}
				else if (SystemHelpMgr.HintTextForSkillIcon == 1)
				{
					if (skillIcon.TagIndex == 1)
					{
						SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
						this.ShowHelpAnim(-1, 0);
					}
				}
				else if (SystemHelpMgr.HintTextForSkillIcon == skillIcon.TagIndex)
				{
					SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
					this.ShowHelpAnim(-1, 0);
				}
			}
			return;
		}
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese))
		{
			Super.HintMainText(Global.GetLang("坐骑系统开启后才可以使用坐骑技能"), 10, 3);
			return;
		}
		if (Global.GetRoleHorseSkillData(Global.Data.RoleID).Count <= 0)
		{
			Super.HintMainText(Global.GetLang("入库坐骑后才可以使用坐骑技能"), 10, 3);
		}
	}

	protected override void InitializeComponent()
	{
		for (int i = 0; i < 5; i++)
		{
			int index = i;
			GSkillIcon skillIcon = this.SkillIcon[i];
			skillIcon.TagIndex = index;
			skillIcon.StillIcon.OnPress = delegate(GameObject go, bool state)
			{
				if (state)
				{
					this.SkillIconPressed = skillIcon;
					this.InvokeRepeating("RepeatSkillIcon", 0.24f, 0.2f);
				}
				else
				{
					this.CancelAutoUseSkill();
				}
			};
			skillIcon.StillIcon.MouseLeftButtonUp = delegate(object go, MouseEvent state)
			{
				this.PressSkillIcon(skillIcon);
			};
			skillIcon.onDrag = delegate(GameObject go, Vector2 delta)
			{
				if (delta.magnitude > 50f)
				{
					this.CancelAutoUseSkill();
					this.ShowSkillSelectPart(index);
				}
			};
		}
		this.SkillIcon[4].EnableState = false;
		this.HuiJiIconSkill = this.HuiJiIcon.GetComponent<HuiJiIcon>();
		UIEventListener.Get(this.HuiJiIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.HuiJiIcon_Click);
		UIEventListener.Get(this.BianShenIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.BianShenIcon_Click);
		UIEventListener.Get(this.BaitanIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.BaitanIcon_Click);
		UIEventListener.Get(this.BaoxiangIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.BaoxiangIcon_Click);
		UIEventListener.Get(this.BianqiangIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.BianqiangIcon_Click);
		UIEventListener.Get(this.DuihuaIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.DuihuaIcon_Click);
		UIEventListener.Get(this.LianzhiIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.LianzhiIcon_Click);
		UIEventListener.Get(this.BianQiangNewIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.BianQiangNewIcon_Click);
		UIEventListener.Get(this.XingYunChouJiangIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.XingyunChoujiang_Click);
		if (SceneUIClasses.RebornMap.IsTheScene())
		{
			for (int j = 0; j < this.Ctrl0.Children.Count(); j++)
			{
				GameObject childAt = this.Ctrl0.Children.getChildAt(j);
				if (null != childAt && !childAt.name.Equals("DuihuaIcon"))
				{
					this.mAnNiuPosDic[childAt.name] = childAt.transform.localPosition;
					childAt.transform.localPosition = new Vector3(-2000f, -2000f, 0f);
				}
			}
		}
		this.RefreshSkillIcon();
		UICtrlBar.ms_Singleton = this;
		ActivityTipManager.RegActivityTipItem(7000, delegate(int s, ActivityTipItem e)
		{
			this._MeiRiTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(8000, delegate(int s, ActivityTipItem e)
		{
			this._QiFuTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(18002, delegate(int s, ActivityTipItem e)
		{
			this._XingyUNcChoujiangTipIcon.gameObject.SetActive(e.IsActive);
		});
		UIEventListener.Get(this.youKeDengLu.gameObject).onClick = delegate(GameObject s)
		{
			VideoSystem.GetInstance().CloseVideoView();
			PlatSDKMgr.ShowUserCenter();
			this.Ctrl2.Visibility = false;
		};
		this.Ctrl2.Visibility = PlatSDKMgr.IsGuestk();
		Global.ChangeEmblemCoolDownData(Global.GetFangZhiJiaSuTime(), 0L);
		EmblemCoolDownItem emblemItem = Global.GetEmblemItem();
		if (emblemItem != null)
		{
			this.RefreshEmblemCoodDown(emblemItem.ID, emblemItem.GetCDTicks(), emblemItem.GetContinuedTicks());
		}
	}

	private void RefreshHuiJiIconPos()
	{
		Vector3 localPosition = this.HuiJiIcon.transform.localPosition;
		if (!this.SkillIcon[4].EnableState)
		{
			localPosition.y = 120f;
		}
		else
		{
			localPosition.y = 120f;
		}
		this.HuiJiIcon.transform.localPosition = localPosition;
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		ActivityTipManager.RegActivityTipItem(7000, null);
		ActivityTipManager.RegActivityTipItem(8000, null);
		ActivityTipManager.RegActivityTipItem(18002, null);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (id < 1000)
		{
			SystemHelpMgr.HintTextForSkillIcon = id;
			if (id >= 0 && id < 4)
			{
				Super.PlayYinDaoSound("xinshouyindao4.mp3", false, false);
				this.AnimHand.SetActive(true);
				this.AnimHighlight.Visibility = true;
				this.AnimHand.transform.localPosition = this.SkillIcon[id].transform.localPosition + new Vector3(0f, 0f, -0.3f);
				if (id == 1)
				{
					this.AnimHighlight.transform.localScale = Vector3.one;
				}
				else
				{
					this.AnimHighlight.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				}
			}
			if (id == 2)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("点击增益技能技能提高角色能力"), 3);
			}
			else if (id == 1)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("点击技能键攻击敌人"), 3);
			}
			else if (id == 0)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("使用新技能击破大门"), 3);
			}
			else if (id == 3)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("使用新技能消灭巫师"), 3);
			}
			else
			{
				SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
			}
			this.AnimHand.gameObject.SetActive(state > 0);
		}
		else if (id == 1000)
		{
			SystemHelpPart.SetMaskEx(this.SkillIcon[3], new Vector4(0f, 0f), 1);
		}
	}

	private void InitSkillIcon(GSkillIcon SkillIcon, QuickKeyItem quickKeyItem)
	{
		SkillData skillDataByID = Global.GetSkillDataByID(quickKeyItem.ID);
		if (skillDataByID == null)
		{
			SkillIcon.BodyURL = null;
			return;
		}
		int skillID = skillDataByID.SkillID;
		SkillIcon.TagCode = skillID;
		MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
		if (skillXmlNode == null)
		{
			SkillIcon.BodyURL = null;
			return;
		}
		int magicIcon = skillXmlNode.MagicIcon;
		if (magicIcon > 0)
		{
			SkillIcon.BodyURL = Global.GetSkillIconString(magicIcon);
		}
		if (ConfigMagicInfos.IsHorseSkill(skillID))
		{
			HorseSkillData horseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID).Find((HorseSkillData e) => 0L < Global.GetSkillCoolDownTicks(e.SkillID));
			if (horseSkillData != null)
			{
				long skillCoolDownTicks = Global.GetSkillCoolDownTicks(horseSkillData.SkillID);
				if (skillCoolDownTicks > 0L)
				{
					MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(horseSkillData.SkillID);
					if (maigcInfoVOByCode != null)
					{
						SkillIcon.MyStart(skillCoolDownTicks, false, 0L, true, true, (long)(maigcInfoVOByCode.CDTime * 1000));
					}
					else
					{
						SkillIcon.MyStart(skillCoolDownTicks, false, 0L, true, true, -1L);
					}
				}
			}
		}
		else
		{
			long skillCoolDownTicks2 = Global.GetSkillCoolDownTicks(skillID);
			if (skillCoolDownTicks2 > 0L)
			{
				SkillIcon.MyStart(skillCoolDownTicks2, false, 0L, true, true, -1L);
			}
		}
	}

	public void RefreshHorseSkillIcon()
	{
		List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
		if (0 < roleHorseSkillData.Count)
		{
			this.SkillIcon[4].EnableState = true;
			this.SkillIcon[4].TagCode = -1;
			if (0 < Global.Data.RoleUseHorseSkillId)
			{
				HorseSkillData horseSkillData = roleHorseSkillData.Find((HorseSkillData e) => e.SkillID == Global.Data.RoleUseHorseSkillId);
				if (horseSkillData != null)
				{
					this.InitSkillIcon(this.SkillIcon[4], new QuickKeyItem
					{
						ID = horseSkillData.SkillID
					});
				}
				else
				{
					this.SkillIcon[4].BodyURL = null;
				}
			}
		}
		else
		{
			this.SkillIcon[4].EnableState = false;
		}
	}

	public void RefreshSkillIcon()
	{
		Super.InitMainQuickKeys();
		Super.ParseMainQuickKeys(Global.Data.roleData.MainQuickBarKeys, false);
		for (int i = 0; i < 4; i++)
		{
			this.SkillIcon[i].TagCode = -1;
			QuickKeyItem quickKeyItem = Super.GData.MainQuickKeyItems[i];
			if (quickKeyItem == null)
			{
				this.SkillIcon[i].BodyURL = null;
			}
			else
			{
				this.InitSkillIcon(this.SkillIcon[i], quickKeyItem);
			}
		}
		this.RefreshHorseSkillIcon();
		this.RefreshHuiJiIconPos();
		this.RefershBianShenButton();
		if (Global.Data.roleData.IsFlashPlayer == 0)
		{
			int num = 0;
			for (int j = 0; j < Global.Data.roleData.SkillDataList.Count; j++)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(Global.Data.roleData.SkillDataList[j].SkillID);
				if (skillXmlNode != null)
				{
					int actionIndex = skillXmlNode.ActionIndex;
					if (actionIndex < 1000)
					{
						num++;
					}
				}
			}
			this.SetSkillIconCount(num);
		}
	}

	public void ShowSkillSelectPart(int index)
	{
		if (index < 0 || index > 5 || Global.Data.roleData.IsFlashPlayer != 0)
		{
			return;
		}
		if (index != 4 || this.SkillIcon[index].EnableState)
		{
			int tagCode = this.SkillIcon[index].TagCode;
			GChildWindow window = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(window, "Skill Select");
			this.Container.Children.Add(window);
			SkillSelectPart skillSelectPart = SkillSelectPart.GShow();
			window.SetContent(window.BodyPresenter, skillSelectPart, 0.0, 0.0, true);
			skillSelectPart.InitPartData(index, tagCode, delegate(object s, DPSelectedItemEventArgs e)
			{
				Super.CloseChildWindow(this, window);
				int index2 = -1;
				int skillID = -1;
				if (e != null)
				{
					skillID = e.ID;
					index2 = e.Index;
				}
				this.SelectSkillEnd(index2, skillID);
			}, false);
			return;
		}
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese))
		{
			Super.HintMainText(Global.GetLang("坐骑系统开启后才可以使用坐骑技能"), 10, 3);
			return;
		}
		if (Global.GetRoleHorseSkillData(Global.Data.RoleID).Count <= 0)
		{
			Super.HintMainText(Global.GetLang("入库坐骑后才可以使用坐骑技能"), 10, 3);
		}
	}

	public void AddTempSkill(int start, int count)
	{
		int num = Global.CalcOriginalOccupationID(Global.CheckRoleOcc(Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation));
		int num2 = start;
		while (num2 < start + count && num2 < 4)
		{
			int num3 = UICtrlBar.TempSkillList[num, num2, 0];
			Global.AddSkillData(UICtrlBar.TempSkillList[num, num2, 1], num3, UICtrlBar.TempSkillList[num, num2, 2]);
			int i = num3;
			while (i > 0)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(i);
				if (skillXmlNode == null)
				{
					break;
				}
				i = skillXmlNode.NextMagicID;
				if (i > 0)
				{
					Global.AddSkillData(-1, i, 1);
				}
			}
			num2++;
		}
		this.SetSkillIconCount(num2);
		Global.Data.roleData.MainQuickBarKeys = UICtrlBar.TempQuickBarKeys[num];
		Global.Data.GameScene.SetDefaultSkillID(UICtrlBar.TempSkillList[num, 0, 0]);
		this.RefreshSkillIcon();
	}

	public void SetSkillIconCount(int count)
	{
		this.SkillIcon[1].EnableState = (count > 0);
		this.SkillIcon[2].EnableState = (count > 1);
		this.SkillIcon[0].EnableState = (count > 2);
		this.SkillIcon[3].EnableState = (count > 3);
	}

	public void ResetSkillBar()
	{
		List<SkillData> list = new List<SkillData>();
		for (int i = 0; i < Global.Data.roleData.SkillDataList.Count; i++)
		{
			SkillData skillData = Global.Data.roleData.SkillDataList[i];
			int num = skillData.SkillID % 100;
			if (num == 0 || num > 60)
			{
				list.Add(skillData);
			}
		}
		Global.Data.roleData.SkillDataList = list;
		Global.Data.roleData.MainQuickBarKeys = null;
		this.RefreshSkillIcon();
		GameInstance.Game.SpriteModKeys(0, Global.Data.roleData.MainQuickBarKeys);
	}

	public void SelectSkillEnd(int index, int skillID)
	{
		if (index >= 4 || index < 0)
		{
			if (index != 4 || 0 >= Global.GetRoleHorseSkillData(Global.Data.RoleID).Count)
			{
				return;
			}
		}
		this.AddLearnedSkillReal(skillID, index, 0);
	}

	public void AddLearnedSkillReal(int skillID, int index, int magicType)
	{
		if (4 > index)
		{
			foreach (QuickKeyItem quickKeyItem in Super.GData.MainQuickKeyItems.Clone() as QuickKeyItem[])
			{
				if (quickKeyItem != null && quickKeyItem.ID == skillID)
				{
					quickKeyItem.ItemType = -1;
					quickKeyItem.ID = 0;
				}
			}
			QuickKeyItem[] array;
			array[index] = new QuickKeyItem();
			array[index].ItemType = 0;
			array[index].ID = skillID;
			string quickKeys = Super.GetQuickKeys(array);
			Global.Data.roleData.MainQuickBarKeys = quickKeys;
			Super.ParseMainQuickKeys(Global.Data.roleData.MainQuickBarKeys, false);
			GameInstance.Game.SpriteModKeys(0, quickKeys);
			if ((magicType == 1 || magicType == 2) && ConfigMagicInfos.CanSkillByBangDing(skillID, false))
			{
				Global.Data.GameScene.SetDefaultSkillID(skillID);
			}
		}
		else if (index == 4)
		{
			List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
			if (0 < roleHorseSkillData.Count)
			{
				for (int j = 0; j < roleHorseSkillData.Count; j++)
				{
					if (skillID == roleHorseSkillData[j].SkillID)
					{
						GameInstance.Game.SendZuoQiSkill(skillID);
						break;
					}
				}
			}
		}
	}

	public void AddLearnedSkill(int skillID)
	{
		object skillDataByID = Global.GetSkillDataByID(skillID);
		if (skillDataByID != null)
		{
			int num = this.FindBlankItem();
			if (num >= 0)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
				int magicType = skillXmlNode.MagicType;
				int actionIndex = skillXmlNode.ActionIndex;
				if (magicType >= 0 && magicType < 3 && actionIndex < 1000)
				{
					this.AddLearnedSkillReal(skillID, num, magicType);
				}
			}
		}
	}

	private int FindBlankItem()
	{
		QuickKeyItem[] mainQuickKeyItems = Super.GData.MainQuickKeyItems;
		if (mainQuickKeyItems[1] == null || mainQuickKeyItems[1].ItemType < 0)
		{
			return 1;
		}
		if (mainQuickKeyItems[2] == null || mainQuickKeyItems[2].ItemType < 0)
		{
			return 2;
		}
		if (mainQuickKeyItems[0] == null || mainQuickKeyItems[0].ItemType < 0)
		{
			return 0;
		}
		if (mainQuickKeyItems[3] == null || mainQuickKeyItems[3].ItemType < 0)
		{
			return 3;
		}
		return -1;
	}

	protected void OnEnable()
	{
		this.RefreshHuiJiIconPos();
	}

	public void RefreshEmblemIcon()
	{
		if (null != this.HuiJiIconSkill)
		{
			this.HuiJiIconSkill.RefreshIcon();
		}
	}

	public void RefreshEmblemCoodDown(int EmblemID, long CDTicks, long ContinuedTicks)
	{
		this.HuiJiIconSkill.RefreshTime(EmblemID, CDTicks, ContinuedTicks);
	}

	public void RefreshCoolDown(int skillID, long ticks)
	{
		for (int i = 0; i < 5; i++)
		{
			if (skillID == this.SkillIcon[i].TagCode)
			{
				if (ticks > 0L)
				{
					this.SkillIcon[i].MyStart(ticks, true, 0L, true, true, -1L);
				}
				else
				{
					this.SkillIcon[i].StopCoolDown();
				}
			}
		}
		if (ShenHunData.IsInBianShenState())
		{
			for (int j = 0; j < 5; j++)
			{
				if (skillID == this.BianShenSkillIcon[j].TagCode)
				{
					if (ticks > 0L)
					{
						this.BianShenSkillIcon[j].MyStart(ticks, true, 0L, true, true, -1L);
					}
					else
					{
						this.BianShenSkillIcon[j].StopCoolDown();
					}
				}
			}
		}
	}

	private void BaitanIcon_Click(GameObject go)
	{
		if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
		{
			return;
		}
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 0);
		}
	}

	private void DuihuaIcon_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 1);
		}
	}

	private void BaoxiangIcon_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 2);
		}
	}

	private void BianqiangIcon_Click(GameObject go)
	{
		if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
		{
			return;
		}
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 3);
		}
	}

	private void LianzhiIcon_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 4);
		}
	}

	private void BianQiangNewIcon_Click(GameObject go)
	{
		if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
		{
			return;
		}
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 5);
		}
	}

	private void XingyunChoujiang_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.NormalButton, 6);
		}
	}

	private void HuiJiIcon_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null && this.HuiJiIconSkill != null && !this.HuiJiIconSkill.mBool)
		{
			this.UICtrlBarNotify(UICtrlBarTypes.HuiJiButton, 0);
		}
	}

	private void BianShenIcon_Click(GameObject go)
	{
		if (this.UICtrlBarNotify != null && !ShenHunData.IsInBianShenState() && !this.BianShenIcon.BeInCDTime)
		{
			if (ShenHunData.IsBianShenNumEnough())
			{
				if (this.m_beCanSendBianShen)
				{
					this.UICtrlBarNotify(UICtrlBarTypes.BianShenButton, 0);
					this.m_beCanSendBianShen = false;
					base.Invoke("ResetBianShenSendTime", 2f);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("变身道具不足"), 10, 3);
			}
		}
	}

	private void ResetBianShenSendTime()
	{
		this.m_beCanSendBianShen = true;
	}

	public void CancelAutoUseSkill()
	{
		base.CancelInvoke("RepeatSkillIcon");
		this.SkillIconPressed = null;
	}

	public static bool IsPress()
	{
		return UICtrlBar.singleton != null && UICtrlBar.singleton.IsInvoking("RepeatSkillIcon");
	}

	public void SetChoujiangIconState()
	{
		NGUITools.SetActive(this.XingYunChouJiangIcon.gameObject, HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.XingyunChoujiang));
	}

	public void BianShen()
	{
		this.IsSafeRegion = this.m_beInSafe;
		this.InitBianShenSkill();
		this.BianShenIcon.BianShenStart();
		PlayZone.GlobalPlayZone.StopCaiji(true);
	}

	public void BianShenEnd()
	{
		this.IsSafeRegion = this.m_beInSafe;
		this.BianShenIcon.BianShenEnd();
		PlayZone.GlobalPlayZone.StopCaiji(true);
	}

	private void InitBianShenSkill()
	{
		long freeBianShenNum = ShenHunData.GetFreeBianShenNum();
		List<int> selfBianShenSkill = ShenHunData.GetSelfBianShenSkill();
		for (int i = 0; i < selfBianShenSkill.Count; i++)
		{
			GSkillIcon skillIcon = this.BianShenSkillIcon[i];
			skillIcon.TagIndex = i;
			skillIcon.TagCode = -1;
			skillIcon.EnableState = true;
			QuickKeyItem quickKeyItem = new QuickKeyItem
			{
				ID = selfBianShenSkill[i]
			};
			this.InitSkillIcon(skillIcon, quickKeyItem);
			skillIcon.StillIcon.MouseLeftButtonUp = delegate(object go, MouseEvent state)
			{
				this.PressSkillIcon(skillIcon);
			};
		}
		for (int j = selfBianShenSkill.Count; j < this.BianShenSkillIcon.Length; j++)
		{
			this.BianShenSkillIcon[j].TagIndex = 99;
			this.BianShenSkillIcon[j].EnableState = false;
			this.BianShenSkillIcon[j].StillIcon.MouseLeftButtonUp = null;
		}
	}

	private const float HuiJiIconPosY1 = 120f;

	private const float HuiJiIconPosY2 = 120f;

	public SpriteSL Ctrl0;

	public UIButton BaitanIcon;

	public UIButton DuihuaIcon;

	public UIButton BaoxiangIcon;

	public UIButton BianqiangIcon;

	public UIButton LianzhiIcon;

	public UIButton BianQiangNewIcon;

	public Transform _MeiRiTipIcon;

	public Transform _QiFuTipIcon;

	public UIButton XingYunChouJiangIcon;

	public Transform _XingyUNcChoujiangTipIcon;

	public SpriteSL Ctrl1;

	public GSkillIcon[] SkillIcon;

	public GameObject AnimHand;

	public CAnimation AnimHighlight;

	public SpriteSL Ctrl2;

	public UIButton youKeDengLu;

	public UIButton HuiJiIcon;

	private HuiJiIcon HuiJiIconSkill;

	public GameObject BianShenTabs;

	public GameObject NormalSkillTabs;

	public BianShenIcon BianShenIcon;

	public GSkillIcon[] BianShenSkillIcon;

	private bool m_beInSafe;

	private bool m_beCanSendBianShen = true;

	public UICtrlBarNotifyHandler UICtrlBarNotify;

	private static UICtrlBar ms_Singleton = null;

	private GSkillIcon SkillIconPressed;

	private Dictionary<string, Vector3> mAnNiuPosDic = new Dictionary<string, Vector3>();

	private static int[,,] TempSkillList = new int[,,]
	{
		{
			{
				100,
				102,
				1,
				0
			},
			{
				104,
				104,
				1,
				1
			},
			{
				120,
				120,
				1,
				3
			},
			{
				102,
				105,
				1,
				2
			}
		},
		{
			{
				200,
				203,
				1,
				1
			},
			{
				205,
				205,
				1,
				0
			},
			{
				204,
				204,
				1,
				3
			},
			{
				203,
				206,
				1,
				2
			}
		},
		{
			{
				300,
				307,
				1,
				3
			},
			{
				304,
				304,
				1,
				2
			},
			{
				305,
				305,
				1,
				1
			},
			{
				307,
				301,
				1,
				0
			}
		}
	};

	private static string[] TempQuickBarKeys = new string[]
	{
		"0@120|1@100|0@104|0@102",
		"0@204|0@200|0@205|1@203",
		"0@305|1@300|0@304|0@307"
	};
}
