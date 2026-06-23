using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class HintQueueIcon : UserControl
{
	public int GoodsDbID
	{
		get
		{
			return this._GoodsDbID;
		}
		set
		{
			this._GoodsDbID = value;
		}
	}

	public ObservableCollection arrayItemCollection
	{
		get
		{
			return null;
		}
	}

	public string SpriteName
	{
		set
		{
		}
	}

	public void SetIcon(HintQueueIcon.HintTypes type)
	{
		switch (type)
		{
		case HintQueueIcon.HintTypes.BuChang:
			this._HintIcon.spriteName = "hintIconBuchang";
			break;
		case HintQueueIcon.HintTypes.ZhuanSheng:
			this._HintIcon.spriteName = "hintIconZhuansheng";
			break;
		case HintQueueIcon.HintTypes.YaoPinTip:
			this._HintIcon.spriteName = "hintIconYaopinBuzhu";
			break;
		case HintQueueIcon.HintTypes.YaoPinTipMaigc:
			this._HintIcon.spriteName = "hintIconYaopinBuzhu2";
			break;
		case HintQueueIcon.HintTypes.EquipInvalid:
			this._HintIcon.spriteName = "hintIconEquipInvalid";
			break;
		case HintQueueIcon.HintTypes.FixEquip:
			this._HintIcon.spriteName = "hintIconNaijiu";
			break;
		case HintQueueIcon.HintTypes.AddPoint:
			this._HintIcon.spriteName = "hintIconJiadian";
			break;
		case HintQueueIcon.HintTypes.BagFull:
			this._HintIcon.spriteName = "hintIconBag";
			break;
		case HintQueueIcon.HintTypes.SkillUsedNumFull:
			this._HintIcon.spriteName = "hintIconJinengJiadian";
			break;
		case HintQueueIcon.HintTypes.ZhuanZhi:
			this._HintIcon.spriteName = "hintIconZhuanzhi";
			break;
		}
		this._HintIcon.MakePixelPerfect();
	}

	public void SetIcon(SystemWizardTypes systemWizardType)
	{
		this.SystemWizardType = systemWizardType;
		switch (systemWizardType)
		{
		case SystemWizardTypes.HintBiGuan:
			this.SpriteName = "HintBar_BiGuanIconBtn";
			break;
		default:
			if (systemWizardType == SystemWizardTypes.HintReloadXml)
			{
				this.SpriteName = "HintBar_GiftHorseUpdateIconBtn";
			}
			break;
		case SystemWizardTypes.GiftUpLevel:
			this.SpriteName = "HintBar_GiftUpLevelIconBtn";
			if (Global.Data.roleData.Level < 60)
			{
				Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("图标向导UI"), 690000, 0, 1);
			}
			break;
		case SystemWizardTypes.GiftLogin:
			this.SpriteName = "HintBar_GiftLoginIconBtn";
			break;
		case SystemWizardTypes.GiftOnline:
			this.SpriteName = "HintBar_GiftOnlineIconBtn";
			break;
		case SystemWizardTypes.LingLiFull:
			this.SpriteName = "HintBar_LingLiFullIconBtn";
			break;
		}
	}

	public void SetIconUrl(HintQueueIcon.HintTypes type)
	{
		string text = string.Empty;
		switch (type)
		{
		case HintQueueIcon.HintTypes.BuChang:
			text = "hintIconBuchang.png";
			break;
		case HintQueueIcon.HintTypes.ZhuanSheng:
			text = "hintIconZhuansheng.png";
			break;
		case HintQueueIcon.HintTypes.YaoPinTip:
			text = "hintIconYaopinBuzhu.png";
			break;
		case HintQueueIcon.HintTypes.YaoPinTipMaigc:
			text = "hintIconYaopinBuzhu2.png";
			break;
		case HintQueueIcon.HintTypes.EquipInvalid:
			text = "hintIconEquipInvalid.png";
			break;
		case HintQueueIcon.HintTypes.FixEquip:
			text = "hintIconNaijiu.png";
			break;
		case HintQueueIcon.HintTypes.AddPoint:
			text = "hintIconJiadian.png";
			break;
		case HintQueueIcon.HintTypes.BagFull:
			text = "hintIconBag.png";
			break;
		case HintQueueIcon.HintTypes.SkillUsedNumFull:
			text = "hintIconJinengJiadian.png";
			break;
		case HintQueueIcon.HintTypes.ZhuanZhi:
			text = "hintIconZhuanzhi.png";
			break;
		case HintQueueIcon.HintTypes.KuaFuTeamCompete:
			text = "KuaFuTeamCompete.png";
			break;
		case HintQueueIcon.HintTypes.Rebirth:
			text = "IconRebrith.png";
			break;
		}
		this._HintIconUrl.DestroyImmediateTexture();
		this._HintIconUrl.URL = string.Format("NetImages/GameRes/Images/HintIcon/{0}", text);
		this._HintIconUrl.xMakePerfect();
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		SystemWizardTypes systemWizardType = this.SystemWizardType;
		switch (systemWizardType)
		{
		case SystemWizardTypes.HintBiGuan:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 3
				});
			}
			break;
		default:
			if (systemWizardType == SystemWizardTypes.HintReloadXml)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 1
					});
				}
			}
			break;
		case SystemWizardTypes.GiftUpLevel:
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("图标向导UI"), null);
			break;
		case SystemWizardTypes.GiftLogin:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 8
				});
			}
			break;
		case SystemWizardTypes.GiftOnline:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 9
				});
			}
			break;
		case SystemWizardTypes.LingLiFull:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 5
				});
			}
			break;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
	}

	public void AddNewHint(HintQueueIcon.HintTypes type, params object[] param)
	{
		if (SceneUIClasses.RebornMap.IsTheScene())
		{
			if (type != HintQueueIcon.HintTypes.Chart)
			{
				return;
			}
		}
		if (type >= HintQueueIcon.HintTypes.ZhuanSheng && type < HintQueueIcon.HintTypes.ZhuanZhi && (Global.IsInKuaFuPlunderMainMap() || Global.IsCompMiDongMap()))
		{
			return;
		}
		if (type == HintQueueIcon.HintTypes.ZhuanSheng && Global.Data.ZSPSetting.DontPromptingZhuanSheng)
		{
			return;
		}
		if (type < HintQueueIcon.HintTypes.MaxQueuedHintMsg)
		{
			int num = 0;
			foreach (HintQueueIcon.HintData hintData in this.HintList)
			{
				if (hintData.Type < type)
				{
					num++;
				}
				else
				{
					if (hintData.Type == type)
					{
						return;
					}
					break;
				}
			}
			HintQueueIcon.HintData hintData2 = new HintQueueIcon.HintData();
			hintData2.Type = type;
			hintData2.Active = true;
			if (type != HintQueueIcon.HintTypes.FixEquip)
			{
				if (type != HintQueueIcon.HintTypes.AddPoint)
				{
				}
			}
			this.HintList.Insert(num, hintData2);
			this.LastCheckHintQueueTick = 0L;
		}
		else
		{
			HintQueueIcon.HintData hintData3;
			if (!this.HintDict.TryGetValue((int)type, ref hintData3))
			{
				hintData3 = new HintQueueIcon.HintData();
				this.HintDict.Add((int)type, hintData3);
			}
			switch (type)
			{
			}
		}
	}

	public void CheckEquipBroken()
	{
		GoodsData anyBrokenEquip = Global.GetAnyBrokenEquip();
		this.UpdateHintState(HintQueueIcon.HintTypes.FixEquip, anyBrokenEquip != null);
	}

	public void CheckBagItemChange()
	{
		HintQueueIcon.HintBagFull = Global.IsBagFull();
		this.UpdateHintState(HintQueueIcon.HintTypes.BagFull, HintQueueIcon.HintBagFull);
	}

	public void CheckEquipInvalid()
	{
		HintQueueIcon.HintEquipInvalid = Global.IsAnyEquipInvalid();
		this.UpdateHintState(HintQueueIcon.HintTypes.EquipInvalid, HintQueueIcon.HintEquipInvalid);
	}

	public void CheckKuaFuTeamCompeteState(bool isHint = false)
	{
		this.UpdateHintState(HintQueueIcon.HintTypes.KuaFuTeamCompete, isHint);
	}

	private void UpdateHintState(HintQueueIcon.HintTypes type, bool toHint)
	{
		if (toHint)
		{
			if (this.activeHint == null || this.activeHint.Type != type)
			{
				this.AddNewHint(type, new object[0]);
			}
		}
		else
		{
			this.RemoveHint(type, true);
		}
		this.ProcessHintQueue(true);
	}

	public void OnAttributeChanged()
	{
		if (Global.Data.roleData.MapCode == 6090)
		{
			return;
		}
		int num = (int)Global.GetCurrentRoleProp(0, 1);
		this.UpdateHintState(HintQueueIcon.HintTypes.AddPoint, num >= 50);
	}

	public void OnLevelChanged()
	{
		if (Global.Data.ZSPSetting.DontPromptingZhuanSheng)
		{
			this.UpdateHintState(HintQueueIcon.HintTypes.ZhuanSheng, false);
		}
		else
		{
			this.UpdateHintState(HintQueueIcon.HintTypes.ZhuanSheng, Global.CanHintZhuanSheng(true));
		}
	}

	public void OnRebirthCheck()
	{
		if (!SceneUIClasses.RebornMap.IsTheScene())
		{
			if (this.mMeiLanDataBag == null)
			{
				TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
			}
			if (this.mLingyuList == null)
			{
				TCPGameServerCmds.CMD_SPR_GET_LINGYU_LIST.SendDataUseRoleID();
			}
			byte[] array = new byte[3];
			byte[] array2 = new byte[3];
			int id = Global.Data.roleData.RebornCount + 1;
			RebornStageVO rebornStageVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornStageVOByID(id);
			if (rebornStageVOByID != null)
			{
				if (rebornStageVOByID.NeedZhuanShengInf[0] != -1 && rebornStageVOByID.NeedZhuanShengInf[1] != -1)
				{
					bool flag = false;
					if (rebornStageVOByID.NeedZhuanShengInf[0] < Global.Data.roleData.ChangeLifeCount)
					{
						flag = true;
					}
					else if (rebornStageVOByID.NeedZhuanShengInf[0] == Global.Data.roleData.ChangeLifeCount && rebornStageVOByID.NeedZhuanShengInf[1] <= Global.Data.roleData.Level)
					{
						flag = true;
					}
					if (flag)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedRebornLevel != -1)
				{
					if (rebornStageVOByID.NeedRebornLevel <= Global.Data.roleData.RebornLevel)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedZhanLi != -1)
				{
					if (rebornStageVOByID.NeedZhanLi <= Global.Data.roleData.CombatForce)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedMaxWingInf[0] != -1 && rebornStageVOByID.NeedMaxWingInf[1] != -1 && rebornStageVOByID.NeedMaxWingInf[2] != -1 && rebornStageVOByID.NeedMaxWingInf[3] != -1 && rebornStageVOByID.NeedMaxWingInf[4] != -1)
				{
					if (Global.Data.roleData.MyWingData != null)
					{
						int num = 0;
						if (this.mLingyuList != null && 0 < this.mLingyuList.Count)
						{
							for (int i = 0; i < this.mLingyuList.Count; i++)
							{
								if (this.mLingyuList[i].Level > 0)
								{
									num += this.mLingyuList[i].Level;
								}
							}
						}
						bool flag2 = false;
						bool flag3 = false;
						if (rebornStageVOByID.NeedMaxWingInf[0] < Global.Data.roleData.MyWingData.WingID)
						{
							flag3 = true;
						}
						else if (rebornStageVOByID.NeedMaxWingInf[0] == Global.Data.roleData.MyWingData.WingID && rebornStageVOByID.NeedMaxWingInf[1] <= Global.Data.roleData.MyWingData.ForgeLevel)
						{
							flag3 = true;
						}
						if (flag3)
						{
							XElement gameResXml = Global.GetGameResXml("Config/MaxWinZhuLing.xml");
							List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZhuLing");
							int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[xelementList.Count - 1], "PlainZhuLing");
							int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[xelementList.Count - 1], "SeniorZhuLing");
							if (rebornStageVOByID.NeedMaxWingInf[2] <= num && (double)rebornStageVOByID.NeedMaxWingInf[3] <= (double)Global.Data.roleData.MyWingData.ZhuLingNum / (double)xelementAttributeInt && (double)rebornStageVOByID.NeedMaxWingInf[3] <= (double)Global.Data.roleData.MyWingData.ZhuHunNum / (double)xelementAttributeInt2)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedChengJie != -1)
				{
					if (rebornStageVOByID.NeedChengJie <= Global.GetChengJiuLevel(0))
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedShengWang != -1)
				{
					if (rebornStageVOByID.NeedShengWang <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel))
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedMagicBookInf[0] != -1 && rebornStageVOByID.NeedMagicBookInf[1] != -1)
				{
					if (this.mMeiLanDataBag != null)
					{
						if (rebornStageVOByID.NeedMagicBookInf[0] < this.mMeiLanDataBag._Level)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
						else if (rebornStageVOByID.NeedMagicBookInf[0] == this.mMeiLanDataBag._Level && rebornStageVOByID.NeedMagicBookInf[1] <= this.mMeiLanDataBag._StarNum)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
					}
					array[this.GetZoneIndex(array)] = 1;
				}
			}
			if (array2[0] == 1 && array2[1] == 1 && array2[2] == 1 && !Global.IsKuaFuMap(Global.Data.roleData.MapCode, true))
			{
				this.UpdateHintState(HintQueueIcon.HintTypes.Rebirth, true);
			}
			else
			{
				this.RemoveHint(HintQueueIcon.HintTypes.Rebirth, true);
				this.ProcessHintQueue(true);
			}
		}
	}

	private int GetZoneIndex(byte[] index)
	{
		int result = 0;
		if (index != null)
		{
			for (byte b = 0; b < 3; b += 1)
			{
				if (index[(int)b] == 0)
				{
					result = (int)b;
					break;
				}
			}
		}
		return result;
	}

	public void OnSkillUsedNumFull(int skillID)
	{
		this.SkillID = skillID;
		if (skillID > 0)
		{
			this.UpdateHintState(HintQueueIcon.HintTypes.SkillUsedNumFull, true);
		}
		else
		{
			this.RemoveHint(HintQueueIcon.HintTypes.SkillUsedNumFull, true);
			this.ProcessHintQueue(true);
		}
	}

	public void RemoveHint(HintQueueIcon.HintTypes type, bool removeAll)
	{
		HintQueueIcon.HintData hintData;
		if (type < HintQueueIcon.HintTypes.MaxQueuedHintMsg)
		{
			if (removeAll)
			{
				this.HintList.RemoveAll((HintQueueIcon.HintData h) => h.Type == type);
			}
			else
			{
				hintData = this.HintList.Find((HintQueueIcon.HintData h) => h.Type == type);
				if (hintData != null)
				{
					this.HintList.Remove(hintData);
				}
			}
		}
		else if (this.HintDict.TryGetValue((int)type, ref hintData))
		{
			hintData.Active = false;
		}
		if (this.activeHint != null && type == this.activeHint.Type)
		{
			this.activeHint.Showing = false;
			this.activeHint = null;
		}
	}

	public void RemoveHint(HintQueueIcon.HintData hint, bool allSame = false)
	{
		HintQueueIcon.HintTypes type = hint.Type;
		this.RemoveHint(type, allSame);
	}

	public HintQueueIcon.HintData GetNoQueuedHint(HintQueueIcon.HintTypes type)
	{
		HintQueueIcon.HintData hintData;
		if (!this.HintDict.TryGetValue((int)type, ref hintData))
		{
			hintData = new HintQueueIcon.HintData();
			hintData.Type = type;
			hintData.Active = false;
			this.HintDict.Add((int)type, hintData);
		}
		return hintData;
	}

	public HintQueueIcon.HintData PeekHintQueue()
	{
		if (this.HintList.Count > 0)
		{
			return this.HintList[0];
		}
		return null;
	}

	public void ProcessHintQueue(bool force = false)
	{
		if (this.m_beOtherHintOpen && UIHelper.AvalidLevel(this.m_openLevel, this.m_openZhuanSheng, false))
		{
			this.otherQueue.UpdateState();
			if (this.BeShouldRePosition())
			{
				this.RePosition();
			}
		}
		if (!this.IsShowHintQueueIcon())
		{
			this._HintPanel.gameObject.SetActive(false);
			return;
		}
		if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon.GetHongBaoCount() > 0)
		{
			this._HintPanel.gameObject.SetActive(false);
			return;
		}
		if (null == this._HintPanel)
		{
			return;
		}
		if (!force)
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastCheckHintQueueTick < 10000L)
			{
				return;
			}
			this.LastCheckHintQueueTick = correctLocalTime;
		}
		else
		{
			this.LastCheckHintQueueTick = Global.GetCorrectLocalTime();
		}
		HintQueueIcon.HintData hintData = this.PeekHintQueue();
		if (hintData == null)
		{
			this._HintPanel.gameObject.SetActive(false);
			this.activeHint = null;
		}
		else if (this.activeHint == null)
		{
			this.activeHint = hintData;
			this.SetIconUrl(hintData.Type);
			if (!this._HintPanel.gameObject.activeSelf)
			{
				this._HintPanel.gameObject.SetActive(true);
			}
		}
		else
		{
			if (hintData.Type == this.activeHint.Type)
			{
				return;
			}
			if (hintData.Type < this.activeHint.Type)
			{
				this.activeHint = hintData;
				this.SetIconUrl(hintData.Type);
			}
		}
	}

	public void HideIconHint()
	{
		this._HintPanel.gameObject.SetActive(false);
	}

	public void ShowIconHint()
	{
		this._HintPanel.gameObject.SetActive(true);
	}

	private bool BeShouldRePosition()
	{
		bool flag = false;
		if (this._HintPanel.gameObject.activeSelf || (PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon.gameObject.activeSelf))
		{
			flag = true;
		}
		bool flag2 = false;
		if (PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon.gameObject.activeSelf)
		{
			flag2 = true;
		}
		return this.m_beFirstShow != flag || this.m_beSecondShow != flag2 || this.m_showOtherNum != this.otherQueue.ShowNum;
	}

	private void RePosition()
	{
		int num = 0;
		Vector3 zero = Vector3.zero;
		Vector3 localPosition = Vector3.zero;
		if (this._HintPanel != null)
		{
			if (this._HintPanel.gameObject.activeSelf || (PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon.gameObject.activeSelf))
			{
				zero..ctor(64f, 0f, 0f);
				num++;
				this.m_beFirstShow = true;
			}
			else
			{
				zero = Vector3.zero;
				this.m_beFirstShow = false;
			}
		}
		if (PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon.gameObject.activeSelf)
		{
			PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon.SetHintPanelPosition(zero);
			localPosition = zero + new Vector3(64f, 0f, 0f);
			num++;
			this.m_beSecondShow = true;
		}
		else
		{
			localPosition = zero;
			this.m_beSecondShow = false;
		}
		this.otherQueue.transform.localPosition = localPosition;
		this.m_showOtherNum = this.otherQueue.ShowNum;
		num += this.otherQueue.ShowNum;
		if (num > this.m_formerShowNum)
		{
			this.ReShoAnimation();
		}
		this.m_formerShowNum = num;
	}

	private void ReShoAnimation()
	{
		if (this._HintPanel.gameObject.activeSelf)
		{
			Animation componentInChildren = this._HintPanel.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.Stop();
				componentInChildren.Play();
			}
		}
		if (PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon.gameObject.activeSelf)
		{
			Animation componentInChildren2 = PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon.gameObject.GetComponentInChildren<Animation>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.Stop();
				componentInChildren2.Play();
			}
		}
		if (PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon != null && PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon.gameObject.activeSelf)
		{
			Animation componentInChildren3 = PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon.gameObject.GetComponentInChildren<Animation>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.Stop();
				componentInChildren3.Play();
			}
		}
		if (this.otherQueue.ShowNum > 0)
		{
			Animation[] componentsInChildren = this.otherQueue.gameObject.GetComponentsInChildren<Animation>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
				componentsInChildren[i].Play();
			}
		}
	}

	public void OnTimer(long ticks)
	{
		if (ticks < this.LastCheckHintConditionTick + 4700L)
		{
			return;
		}
		this.LastCheckHintConditionTick = ticks;
		this.OnLevelChanged();
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (this.activeHint == null)
		{
			return;
		}
		if (this.activeHint.Type != this.lastHintType)
		{
			return;
		}
		if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu)
		{
			Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
			this.activeHint.Showing = false;
			return;
		}
		if (args.ID == 0)
		{
			switch (this.activeHint.Type)
			{
			case HintQueueIcon.HintTypes.ZhuanSheng:
				PlayZone.GlobalPlayZone.ShowZhuanShengPart();
				break;
			case HintQueueIcon.HintTypes.YaoPinTip:
				PlayZone.GlobalPlayZone.OpenYaoPinTipWindow();
				break;
			case HintQueueIcon.HintTypes.YaoPinTipMaigc:
				PlayZone.GlobalPlayZone.OpenYaoPinTipWindow();
				break;
			case HintQueueIcon.HintTypes.EquipInvalid:
				PlayZone.GlobalPlayZone.ShowMallWindow(false, 0, 2);
				break;
			case HintQueueIcon.HintTypes.FixEquip:
				if (Global.Data.roleData.VIPLevel >= 0)
				{
					GameInstance.Game.SpriteFetchVipDailyAward(4001);
				}
				else
				{
					Super.AutoFindRoad(1, 3, 111, 1);
				}
				break;
			case HintQueueIcon.HintTypes.AddPoint:
				HintQueueIcon.HintAddPoint = true;
				PlayZone.GlobalPlayZone.ShowGamePayerRoleWindow(GamePayerRolePart_PartID.GamePayerRolePart_ShuXing);
				PlayZone.GlobalPlayZone.gamePayerRolePart.roleAttributePart.NotifyResult(0, 0);
				break;
			case HintQueueIcon.HintTypes.BagFull:
				PlayZone.GlobalPlayZone.ShowGamePayerRoleWindow(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao);
				break;
			case HintQueueIcon.HintTypes.SkillUsedNumFull:
				HintQueueIcon.HintSkillUsedNumFull = true;
				PlayZone.GlobalPlayZone.ShowGamePayerRoleWindow(GamePayerRolePart_PartID.GamePayerRolePart_JiNeng);
				break;
			case HintQueueIcon.HintTypes.ZhuanZhi:
				Super.AutoFindRoad(1, 3, 116, 1);
				break;
			case HintQueueIcon.HintTypes.KuaFuTeamCompete:
				Super.HintMainText(Global.GetLang("暂无邀请"), 10, 3);
				break;
			case HintQueueIcon.HintTypes.Rebirth:
				if (!SceneUIClasses.RebornMap.IsTheScene())
				{
					PlayZone.GlobalPlayZone.ShowRebirthFirstPartWind();
				}
				else
				{
					Super.HintMainText(Global.GetLang("不能使用此功能"), 10, 3);
				}
				break;
			}
			this.RemoveHint(this.activeHint, true);
			this.ProcessHintQueue(true);
		}
		else if (args.ID == 1 || args.ID == -1)
		{
			this.activeHint.Showing = false;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_beOtherHintOpen = (ConfigSystemParam.GetSystemParamIntByName("IsMainHintOpen") == 1L);
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("IsMainHintOpenLevel", ',');
		if (systemParamIntArrayByName.Length > 1)
		{
			this.m_openZhuanSheng = systemParamIntArrayByName[0];
			this.m_openLevel = systemParamIntArrayByName[1];
		}
		UIEventListener.Get(this._HintPanel.gameObject).onPress = new UIEventListener.BoolDelegate(this.HintIcon_Clicked);
		ActivityTipManager.RegActivityTipItem(11000, delegate(int s, ActivityTipItem e)
		{
			this.UpdateHintState(HintQueueIcon.HintTypes.BuChang, e.IsActive);
		});
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(11000, null);
		base.OnDestroy();
	}

	protected virtual void OnEnable()
	{
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			if (this._HintPanel.gameObject.activeInHierarchy)
			{
				float scale = (Time.time % 2f <= 1f) ? 1f : 1.1f;
				TweenScale.Begin(this._HintPanel.gameObject, 1f, new Vector3(scale, scale, this._HintPanel.transform.localScale.z));
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void HintIcon_Clicked(GameObject go, bool pressed)
	{
		if (this.activeHint == null)
		{
			return;
		}
		if (this.activeHint.Type != this.lastHintType && this.activeHint.Showing)
		{
			this.activeHint.Showing = false;
		}
		if (!this.activeHint.Showing && pressed)
		{
			this.activeHint.Showing = true;
			this.ShowMessageBox(this.activeHint);
		}
	}

	protected void ShowMessageBox(HintQueueIcon.HintData hint)
	{
		this.lastHintType = hint.Type;
		if (hint.Type == HintQueueIcon.HintTypes.FixEquip)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("立即修理"),
				Global.GetLang("取消")
			};
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("您的装备耐久度过低，请立即修理")
			});
			Super.ShowMessageBoxEx(Global.GetLang("提示"), colorStringForNGUIText, new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
		}
		else if (hint.Type == HintQueueIcon.HintTypes.AddPoint)
		{
			string[] buttons2 = new string[]
			{
				Global.GetLang("立即分配"),
				Global.GetLang("取消")
			};
			string text = ColorCode.EncodingText(Global.GetLang("未分配的点数"), "ff9d08");
			text = string.Format(Global.GetLang("您有大量{0}需要分配,分配属性点可以增强你的战斗力"), text);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), text, new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons2);
		}
		else if (hint.Type == HintQueueIcon.HintTypes.SkillUsedNumFull)
		{
			this.DPSelectItemHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		else if (hint.Type == HintQueueIcon.HintTypes.ZhuanSheng)
		{
			this.DPSelectItemHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		else if (hint.Type == HintQueueIcon.HintTypes.ZhuanZhi)
		{
			string[] buttons3 = new string[]
			{
				Global.GetLang("立即前往"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("您已达到转职条件,请及时前往萨维娜处进行转职"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons3);
		}
		else if (hint.Type == HintQueueIcon.HintTypes.BagFull)
		{
			this.DPSelectItemHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		else if (hint.Type == HintQueueIcon.HintTypes.YaoPinTip || hint.Type == HintQueueIcon.HintTypes.YaoPinTipMaigc)
		{
			this.DPSelectItemHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		else if (hint.Type == HintQueueIcon.HintTypes.BuChang)
		{
			hint.Showing = false;
			int num = 116;
			int npcID = 2130706432 + num;
			PlayZone.GlobalPlayZone.ShowNPCLuaDialogWindow(npcID, num, false);
		}
		else if (hint.Type == HintQueueIcon.HintTypes.EquipInvalid)
		{
			string[] buttons4 = new string[]
			{
				Global.GetLang("购买药剂")
			};
			string text2 = ColorCode.EncodingText(Global.GetLang("未分配的点数"), "ff9d08");
			text2 = string.Format(Global.GetLang("有装备因属性值不足而失效，为了保证您的战力，可通过使用药剂增加属性。"), text2);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), text2, new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons4);
		}
		else if (hint.Type == HintQueueIcon.HintTypes.Rebirth)
		{
			hint.Showing = false;
			PlayZone.GlobalPlayZone.ShowRebirthFirstPartWind();
		}
	}

	private bool IsShowHintQueueIcon()
	{
		bool result = true;
		if (Global.IsCompetitionGuanKan || Global.IsInZhanMengLianSaiCompetetionMap() || Global.IsInKuaFuPlunderBattleMap() || Global.IsInKuaFuPlunderMainMap() || Global.IsInShiLiZhengBaBattleMap() || Global.IsInKuaFuTeamCompete() || Global.IsInDaTaoSha())
		{
			result = false;
		}
		return result;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private SystemWizardTypes SystemWizardType;

	private int _GoodsDbID;

	public int SkillID;

	public static bool HintAddPoint;

	public static bool HintBagFull;

	public static bool HintNewPrivateMsg;

	public static bool HintSkillUsedNumFull;

	public static bool HintEquipInvalid;

	public List<HintQueueIcon.HintData> HintList = new List<HintQueueIcon.HintData>();

	public Dictionary<int, HintQueueIcon.HintData> HintDict = new Dictionary<int, HintQueueIcon.HintData>();

	public UIPanel _HintPanel;

	public UISprite _HintIcon;

	public ShowNetImage _HintIconUrl;

	public TextBlock _HintText;

	public SystemHintQueueManager otherQueue;

	private HintQueueIcon.HintTypes lastHintType;

	private bool m_beOtherHintOpen;

	private int m_openLevel = 100;

	private int m_openZhuanSheng = 100;

	public MerlinGrowthSaveDBData mMeiLanDataBag;

	public List<LingYuData> mLingyuList;

	private HintQueueIcon.HintData activeHint;

	private long LastCheckHintQueueTick = 700L;

	private int m_formerShowNum;

	private bool m_beFirstShow;

	private bool m_beSecondShow;

	private int m_showOtherNum = -1;

	private long LastCheckHintConditionTick = 700L;

	public class HintData
	{
		public HintQueueIcon.HintTypes Type;

		public int Count;

		public bool Active;

		public bool Showing;

		public bool Checked;

		public long LastTick;

		public string Text;

		public object Tag;
	}

	public enum HintTypes
	{
		BuChang,
		ZhuanSheng,
		YaoPinTip,
		YaoPinTipMaigc,
		EquipInvalid,
		FixEquip,
		AddPoint,
		BagFull,
		SkillUsedNumFull,
		ZhuanZhi,
		KuaFuTeamCompete,
		Rebirth,
		MaxQueuedHintMsg = 63,
		BangHui,
		TeamRequest,
		TeamInvite,
		Chart
	}
}
