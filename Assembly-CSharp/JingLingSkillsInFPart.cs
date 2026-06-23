using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class JingLingSkillsInFPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.xml_Magics = Global.GetGameResXml("Config/Magics.xml");
		GameInstance.Game.SendGetJingLingSkillGraspNeedLingJing();
		if (null == this.m_SkillTeXiaoObj_Power)
		{
			this.m_SkillRoot = new Transform[4];
			this.m_SkillTeXiaoObj_Power = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongwujineng/chongwujineng_power", this.m_RootObj[0].transform);
			this.m_SkillTeXiaoObj_Power.transform.localPosition = new Vector3(-130f, -60f, -190f);
			for (int i = 0; i < this.m_SkillRoot.Length; i++)
			{
				string text = string.Format("UI_ceng/kuang/kuang{0}", i + 1);
				this.m_SkillRoot[i] = this.m_SkillTeXiaoObj_Power.transform.FindChild(text);
			}
		}
		this.m_SkillTeXiaoObj_PowerJ_Power = this.m_SkillTeXiaoObj_Power.transform.Find("fangkuang");
		this.m_SkillTeXiao_jiemian = this.m_SkillTeXiaoObj_Power.transform.Find("jiemian").gameObject;
	}

	private void SkillGraspItemStateChange()
	{
		for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
		{
			JingLingSkillGraspItem jingLingSkillGraspItem = this.m_JingLingSkillGraspItemLst[i];
			if (jingLingSkillGraspItem)
			{
				jingLingSkillGraspItem.EffectActive = false;
				jingLingSkillGraspItem.TeXiao_NewActive = false;
				jingLingSkillGraspItem.TeXiao_UpSkillActive = false;
				if (i == this.selectIndex && this.m_JingLingSkillGraspItemLst[i].SkillIsOpen && this.m_JingLingSkillGraspItemLst[i].SkillID != 0)
				{
					GChildWindow w = U3DUtils.NEW<GChildWindow>();
					w.ModalType = ChildWindowModalType.TransBak;
					w.ModalBakSprite.transform.localScale = new Vector3(2000f, 2000f, 1f);
					w.transform.localPosition = new Vector3(-50f, -70f, -800f);
					w.transform.SetParent(base.transform, false);
					JingLingSkillTips jingLingSkillTips = U3DUtils.NEW<JingLingSkillTips>();
					jingLingSkillTips.SetSkillId(this.m_JingLingSkillGraspItemLst[i].SkillID, this.m_JingLingSkillGraspItemLst[i].Lev);
					w.ChildWindowModalBakClick = delegate(object we, EventArgs fg)
					{
						Object.Destroy(w.gameObject, 0.01f);
						return true;
					};
					w.Body.Add(jingLingSkillTips);
				}
			}
		}
	}

	private void ListSelectChange(object s)
	{
		for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
		{
			if (s.Equals(this.m_JingLingSkillGraspItemLst[i]))
			{
				this.selectIndex = i;
			}
		}
	}

	private void SkillGraspItemCheckBoxStateChange(object e)
	{
		for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
		{
			JingLingSkillGraspItem jingLingSkillGraspItem = this.m_JingLingSkillGraspItemLst[i];
			if (jingLingSkillGraspItem.CheckBoxobject.Equals(e))
			{
				if (jingLingSkillGraspItem.IsLock)
				{
					if (2 > this.m_LStSkillLock.Count)
					{
						this.m_LStSkillLock.Add(i);
					}
					else
					{
						jingLingSkillGraspItem.IsLock = false;
					}
				}
				else if (this.m_LStSkillLock.Contains(i))
				{
					this.m_LStSkillLock.Remove(i);
				}
				else
				{
					MUDebug.LogError(new object[]
					{
						string.Format("{0}ID没存", new object[0]),
						jingLingSkillGraspItem.SkillID
					});
				}
				break;
			}
		}
		if (2 <= this.m_LStSkillLock.Count)
		{
			for (int j = 0; j < this.m_JingLingSkillGraspItemLst.Count; j++)
			{
				if (!this.m_JingLingSkillGraspItemLst[j].IsLock)
				{
					this.m_JingLingSkillGraspItemLst[j].CheckBoxActive = false;
				}
			}
		}
		else
		{
			for (int k = 0; k < this.m_JingLingSkillGraspItemLst.Count; k++)
			{
				if (this.m_JingLingSkillGraspItemLst[k].SkillIsOpen)
				{
					this.m_JingLingSkillGraspItemLst[k].CheckBoxActive = true;
				}
			}
		}
		this.Diamond = this.GetPetSkillAwarkCostDiamond();
	}

	private void InitPrefabText()
	{
		this.m_HelpContent[0].transform.localScale = new Vector3(18f, 18f, 1f);
		this.m_HelpContent[2].transform.localScale = new Vector3(18f, 18f, 1f);
		this.m_HelpContent[1].transform.localScale = new Vector3(16f, 16f, 1f);
		this.m_HelpContent[3].transform.localScale = new Vector3(16f, 16f, 1f);
		this.m_HelpContent[2].transform.localPosition = new Vector3(-40f, -165f, -2f);
		this.m_UpLevBtn.Text = Global.GetLang("升级");
		this.m_TopTitle.text = Global.GetLang("槽位技能");
		this.m_Bownlabel.text = Global.GetLang("槽位升级");
		this.m_GraspSkillBtn.Text = Global.GetLang("技能领悟");
		this.m_XiaoHao.text = Global.GetLang("消耗：");
		this.m_BownContent[1].text = Global.GetLang("消耗：");
	}

	private void InitTexture()
	{
	}

	private int GetPetSkillLevelupCost()
	{
		if (0 >= this.m_DicSoltUpCost.Count)
		{
			XElement gameResXml = Global.GetGameResXml("Config/PetSkillLevelup.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "Levelup");
				for (int i = 0; i < xelementList.Count; i++)
				{
					if (xelementList != null)
					{
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Level");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "Cost");
						if (this.m_MaxLev <= xelementAttributeInt)
						{
							this.m_MaxLev = xelementAttributeInt;
						}
						if (this.m_DicSoltUpCost.ContainsKey(xelementAttributeInt))
						{
							this.m_DicSoltUpCost[xelementAttributeInt] = xelementAttributeInt2;
						}
						else
						{
							this.m_DicSoltUpCost.Add(xelementAttributeInt, xelementAttributeInt2);
						}
					}
				}
			}
		}
		if (this.m_SlotLev >= this.m_MaxLev)
		{
			return -1;
		}
		if (this.m_DicSoltUpCost.ContainsKey(this.m_SlotLev + 1))
		{
			return this.m_DicSoltUpCost[this.m_SlotLev + 1];
		}
		return 0;
	}

	private int GetPetSkillAwarkCostDiamond()
	{
		if (0 >= this.m_DicSkillAwarkCostDiamond.Count)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostZuanShi", '|');
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				if (this.m_DicSkillAwarkCostDiamond.ContainsKey(num))
				{
					this.m_DicSkillAwarkCostDiamond[num] = num2;
				}
				else
				{
					this.m_DicSkillAwarkCostDiamond.Add(num, num2);
				}
			}
		}
		if (this.m_DicSkillAwarkCostDiamond.ContainsKey(this.m_LStSkillLock.Count))
		{
			return this.m_DicSkillAwarkCostDiamond[this.m_LStSkillLock.Count];
		}
		return 0;
	}

	private void Empty(object sender, MouseEvent e)
	{
		if (this.m_SkillID == 0 && this.m_IsOpen)
		{
			Super.HintMainText(Global.GetLang("槽位没有技能"), 10, 3);
		}
		else
		{
			Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(this.m_SoltId)), 10, 3);
		}
	}

	private void UpLevBtnHander(object sender, MouseEvent e)
	{
		if (0 > this.m_SoltId)
		{
			Super.HintMainText(Global.GetLang("请选择一个技槽位！"), 10, 3);
		}
		else if (this.GetPetSkillLevelupCost() <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue))
		{
			GameInstance.Game.SendJingLingSkillSlotUp(this.m_DbId, this.m_SoltId + 1);
			Super.ShowNetWaiting(null);
		}
		else
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLingJing, null, string.Empty, Global.GetLang("灵晶"));
		}
	}

	private int GetJingLingSlotOpenLev(int slotIidnex)
	{
		if (this.m_DicSlotOpenLev.Count == 0)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
			byte b = 0;
			while ((int)b < systemParamStringArrayByName.Length)
			{
				string[] array = systemParamStringArrayByName[(int)b].Split(new char[]
				{
					','
				});
				this.m_DicSlotOpenLev.Add((int)b, Convert.ToInt32(array[1]));
				b += 1;
			}
		}
		if (this.m_DicSlotOpenLev.ContainsKey(slotIidnex))
		{
			return this.m_DicSlotOpenLev[slotIidnex];
		}
		return 0;
	}

	private void InitHandler()
	{
		this.m_Helpbtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (null != this.m_HelpObj)
			{
				NGUITools.SetActive(this.m_HelpObj, true);
				this.m_HelpObj.transform.localPosition = new Vector3(0f, 0f, (float)(this.m_Mode_z - 20));
				this.m_HelpCloseBtn.MouseLeftButtonUp = delegate(object x, MouseEvent l)
				{
					NGUITools.SetActive(this.m_HelpObj, false);
				};
				this.m_HelpMasp.alpha = 0.8f;
				UIEventListener.Get(this.m_HelpMasp.gameObject).onClick = delegate(GameObject h)
				{
					NGUITools.SetActive(this.m_HelpObj, false);
				};
				this.m_HelpContent[0].text = this.str_Help[0];
				this.m_HelpContent[1].text = this.str_Help[1];
				this.m_HelpContent[2].text = this.str_Help[2];
				this.m_HelpContent[3].text = this.str_Help[3];
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"界面help的OBJ没赋值"
				});
			}
		};
		this.m_UpLevBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.UpLevBtnHander(e, s);
		};
		if (null != this.m_ListBox)
		{
			this.m_Collection = this.m_ListBox.ItemsSource;
			this.m_ListBox.SelectionChanged = delegate(object e, MouseEvent s)
			{
				this.selectIndex = ((this.m_ListBox.SelectedIndex < 0) ? -1 : this.m_ListBox.SelectedIndex);
				this.SkillGraspItemStateChange();
			};
			this.m_DraggablePanel = this.m_ListBox.Parent.GetComponent<UIDraggablePanel>();
		}
		this.m_GraspSkillBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (Global.Data.equipPet == null)
			{
				Super.HintMainText(Global.GetLang("精灵不存在"), 10, 3);
				return;
			}
			if (0 >= Global.Data.equipPet.Count)
			{
				Super.HintMainText(Global.GetLang("精灵不存在"), 10, 3);
				return;
			}
			for (int i = 0; i < Global.Data.equipPet.Count; i++)
			{
				GoodsData goodsData = Global.Data.equipPet[i];
				if (goodsData.Id == this.m_DbId && goodsData.Forge_level + 1 < this.GetJingLingSlotOpenLev(0))
				{
					Super.HintMainText(Global.GetLang("无技能槽位可用"), 10, 3);
					return;
				}
			}
			if (this.m_LStSkillLock.Count >= this.GetOpenSlotNum())
			{
				Super.HintMainText(Global.GetLang("技能槽位不能全锁"), 10, 3);
				return;
			}
			if (this.m_SkillAwarkCost <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue))
			{
				if (this.m_LStSkillLock.Count <= 2)
				{
					if (Global.Data.roleData.UserMoney >= this.GetPetSkillAwarkCostDiamond())
					{
						int[] lockId = new int[2];
						int j = 0;
						int count = this.m_LStSkillLock.Count;
						while (j < count)
						{
							lockId[j] = this.m_LStSkillLock[j] + 1;
							j++;
						}
						if (0 < this.GetPetSkillAwarkCostDiamond())
						{
							if (Super.MessageBoxIsHint[12] == 0)
							{
								Super.ZuanShiShowMessageBox(Global.GetLang("提示"), string.Format(Global.GetLang("本次操作将要花费{0}"), this.GetPetSkillAwarkCostDiamond()), 2, delegate(object z, DPSelectedItemEventArgs t)
								{
									if (t.ID == 0)
									{
										this.SendJingLingSkillGrasp(this.m_DbId, lockId[0], lockId[1]);
									}
								}, MessBoxIsHintTypes.JjingLingSkillAwarkHint, 0f, string.Empty, 0);
							}
							else
							{
								this.SendJingLingSkillGrasp(this.m_DbId, lockId[0], lockId[1]);
							}
						}
						else
						{
							this.SendJingLingSkillGrasp(this.m_DbId, lockId[0], lockId[1]);
						}
					}
					else
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, Global.GetLang("钻石"));
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("技能最多只能锁定2个！"), 10, 3);
				}
			}
			else
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLingJing, null, string.Empty, Global.GetLang("灵晶"));
			}
		};
		this.m_SkillPreView.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			GChildWindow w = U3DUtils.NEW<GChildWindow>();
			w.ModalType = ChildWindowModalType.Translucent;
			w.transform.localPosition = new Vector3(0f, 0f, (float)this.m_Mode_z);
			w.transform.SetParent(base.transform, false);
			JingLingSkillPreviewPart jingLingSkillPreviewPart = U3DUtils.NEW<JingLingSkillPreviewPart>();
			w.Body.Add(jingLingSkillPreviewPart.gameObject);
			w.ChildWindowModalBakClick = delegate(object x, EventArgs c)
			{
				Object.Destroy(w.gameObject, 0.01f);
				return true;
			};
			jingLingSkillPreviewPart.M_CloseBtn.MouseLeftButtonUp = delegate(object x, MouseEvent c)
			{
				Object.Destroy(w.gameObject, 0.01f);
			};
		};
		this.m_SkillChuanCheng.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LianLu, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.LianLu, trigger, param, param2, true);
				return;
			}
			if (PlayZone.GlobalPlayZone.gamePayerRolePart != null)
			{
				PlayZone.GlobalPlayZone.CloseGamePayerRoleWindow();
			}
			if (PlayZone.GlobalPlayZone.JingLingPart != null)
			{
				PlayZone.GlobalPlayZone.CloseJingLingPartWindow();
			}
			(Super.GData.PlayZoneRoot as PlayZone).ShowLianluWindow(12, 0);
		};
	}

	private void SendJingLingSkillGrasp(int dbId, int SkillSlotLockId0 = 0, int SkillSlotLockId1 = 0)
	{
		GameInstance.Game.SendJingLingSkillGrasp(dbId, SkillSlotLockId0, SkillSlotLockId1);
		Super.ShowNetWaiting(null);
	}

	private int GetOpenSlotNum()
	{
		int num = 0;
		for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
		{
			if (this.m_JingLingSkillGraspItemLst[i].SkillIsOpen)
			{
				num++;
			}
		}
		return num;
	}

	private void ClearSkillUpEffect()
	{
		Transform transform = this.m_SkillRoot[0].transform.parent.Find("chongwu_jineng_shengji");
		if (null != transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	public void ClearLock()
	{
		if (this.m_JingLingSkillGraspItemLst != null)
		{
			for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
			{
				JingLingSkillGraspItem jingLingSkillGraspItem = this.m_JingLingSkillGraspItemLst[i];
				jingLingSkillGraspItem.IsLock = false;
				this.m_JingLingSkillGraspItemLst[i].CheckBoxActive = this.m_JingLingSkillGraspItemLst[i].SkillIsOpen;
				jingLingSkillGraspItem.TeXiao_NewActive = false;
				jingLingSkillGraspItem.TeXiao_UpSkillActive = false;
			}
		}
		if (this.m_LStSkillLock != null)
		{
			this.m_LStSkillLock.Clear();
		}
		this.Diamond = this.GetPetSkillAwarkCostDiamond();
	}

	public void ChangeSkillSignBg(int SlotID, bool bOpen)
	{
	}

	public void ChangeTeXiaoAngle(int SlotID)
	{
		NGUITools.SetActive(this.m_SkillTeXiaoObj_PowerJ_Power, false);
		NGUITools.SetActive(this.m_SkillTeXiao_jiemian, false);
		Vector3 localPosition = this.m_SkillRoot[SlotID].localPosition;
		int num;
		if (localPosition.x > 0f)
		{
			num = ((0f >= localPosition.y) ? 90 : 0);
		}
		else
		{
			num = ((0f >= localPosition.y) ? 180 : 270);
		}
		Animator component = this.m_SkillTeXiaoObj_PowerJ_Power.GetComponent<Animator>();
		if (null != component)
		{
			component.enabled = true;
		}
		Animator component2 = this.m_SkillTeXiao_jiemian.GetComponent<Animator>();
		if (null != component2)
		{
			component2.enabled = true;
		}
		this.m_SkillTeXiaoObj_PowerJ_Power.localRotation = Quaternion.Euler(0f, 180f, (float)num);
		NGUITools.SetActive(this.m_SkillTeXiaoObj_PowerJ_Power, true);
		NGUITools.SetActive(this.m_SkillTeXiao_jiemian, true);
	}

	public void RefreshSkillUp(int SlotID)
	{
		Transform transform = this.m_SkillRoot[SlotID].transform.parent.Find("chongwu_jineng_shengji");
		if (null != transform)
		{
			Object.Destroy(transform.gameObject);
		}
		transform = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongwujineng/chongwu_jineng_shengji", this.m_SkillRoot[SlotID].transform.parent).transform;
		Vector3 localPosition = this.m_SkillRoot[SlotID].transform.localPosition;
		localPosition.z -= 10f;
		transform.localPosition = localPosition;
		transform.name = "chongwu_jineng_shengji";
		DelayDestroy delayDestroy = transform.gameObject.AddComponent<DelayDestroy>();
		delayDestroy.delayTime = 3.5f;
		transform.gameObject.SetActive(true);
	}

	public void RefreshSkillsTeXiao(int NewSlot)
	{
		for (int i = 0; i < this.m_JingLingSkillGraspItemLst.Count; i++)
		{
			this.m_JingLingSkillGraspItemLst[i].TeXiao_NewActive = false;
			this.m_JingLingSkillGraspItemLst[i].TeXiao_UpSkillActive = false;
		}
		for (int j = 0; j < this.m_JingLingSkillGraspItemLst.Count; j++)
		{
			this.m_JingLingSkillGraspItemLst[j].TeXiao_NewActive = (NewSlot == j);
			this.m_JingLingSkillGraspItemLst[j].TeXiao_UpSkillActive = (NewSlot == j);
		}
	}

	public void RefreshSkillAwakeCost(int cost)
	{
		this.m_SkillAwarkCost = cost;
		this.YuanSuXiaohao = this.m_SkillAwarkCost;
	}

	public void SetPratType(JingLingSkillsInFPart.SkillPartInfType type)
	{
		for (int i = 0; i < this.m_RootObj.Length; i++)
		{
			this.m_RootObj[i].SetActive(i == (int)type);
		}
		if (type != JingLingSkillsInFPart.SkillPartInfType.Skills)
		{
			if (type == JingLingSkillsInFPart.SkillPartInfType.SkillsGrasp)
			{
			}
		}
	}

	public void SetSkillGraspInf(jinglingSkillSignItem[] SignArray, int dbID)
	{
		this.m_DbId = dbID;
		bool flag = SignArray.Length > 4;
		this.selectIndex = -1;
		if (0 >= this.m_JingLingSkillGraspItemLst.Count)
		{
			for (int i = 0; i < SignArray.Length; i++)
			{
				JingLingSkillGraspItem jingLingSkillGraspItem = U3DUtils.NEW<JingLingSkillGraspItem>();
				jingLingSkillGraspItem.SlotID = SignArray[i].SlotIndex;
				jingLingSkillGraspItem.SkillIsOpen = SignArray[i].IsOpen;
				jingLingSkillGraspItem.SkillID = SignArray[i].SkillId;
				jingLingSkillGraspItem.Lev = SignArray[i].Lev;
				jingLingSkillGraspItem.CheckBoxActive = jingLingSkillGraspItem.SkillIsOpen;
				jingLingSkillGraspItem.Selecehandler = new ObjectEventHandler(this.ListSelectChange);
				if (!flag)
				{
					UIDragPanelContents component = jingLingSkillGraspItem.GetComponent<UIDragPanelContents>();
					if (null != component)
					{
						component.enabled = false;
					}
				}
				jingLingSkillGraspItem.EffectActive = false;
				UIDragPanelContents component2 = jingLingSkillGraspItem.GetComponent<UIDragPanelContents>();
				if (null != component2)
				{
					component2.draggablePanel = this.m_DraggablePanel;
				}
				jingLingSkillGraspItem.CheckBoxhandler = new ObjectEventHandler(this.SkillGraspItemCheckBoxStateChange);
				this.m_Collection.AddNoUpdate(jingLingSkillGraspItem);
				this.m_JingLingSkillGraspItemLst.Add(jingLingSkillGraspItem);
			}
			this.m_ListBox.repositionNow = true;
		}
		else
		{
			this.SkillGraspItemStateChange();
			for (int j = 0; j < SignArray.Length; j++)
			{
				JingLingSkillGraspItem jingLingSkillGraspItem2 = this.m_JingLingSkillGraspItemLst[j];
				jingLingSkillGraspItem2.SlotID = SignArray[j].SlotIndex;
				jingLingSkillGraspItem2.SkillIsOpen = SignArray[j].IsOpen;
				jingLingSkillGraspItem2.SkillID = SignArray[j].SkillId;
				jingLingSkillGraspItem2.Lev = SignArray[j].Lev;
			}
		}
		this.YuanSuXiaohao = this.m_SkillAwarkCost;
	}

	public void SetSkillInF(int skillID, int Lev, int dbId, int SlotID, bool IsOpen)
	{
		bool flag = false;
		this.m_SlotLev = Lev;
		this.m_SoltId = SlotID;
		this.m_DbId = dbId;
		this.m_SkillID = skillID;
		this.m_IsOpen = IsOpen;
		string text = string.Empty;
		string text2 = Lev.ToString();
		string text3 = string.Empty;
		string magicScripts = string.Empty;
		if (this.m_DidXML.ContainsKey(skillID))
		{
			text = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "Name");
			text3 = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "Description");
			magicScripts = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "MagicScripts");
			flag = true;
		}
		else if (this.xml_Magics != null)
		{
			XElement xelement = Global.GetXElementList(this.xml_Magics, "Magic").Find((XElement s) => s.Attribute("ID").Value == skillID.ToString());
			if (xelement != null)
			{
				this.m_DidXML.Add(skillID, xelement);
				text = Global.GetXElementAttributeStr(xelement, "Name");
				text3 = Global.GetXElementAttributeStr(xelement, "Description");
				magicScripts = Global.GetXElementAttributeStr(xelement, "MagicScripts");
				flag = true;
			}
		}
		if (flag)
		{
			this.m_TopContent[0].text = Global.GetJingLinfSkillName(skillID, true);
			this.m_TopContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("当前效果：")
			});
			string text4 = string.Format("{0}%", Global.GetJIngLingSkillAddAttack(magicScripts, Lev));
			this.m_TopContent[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(text3, text4)
			});
			if (0 >= this.m_MaxLev)
			{
				this.GetPetSkillLevelupCost();
			}
			if (this.m_MaxLev <= Lev)
			{
				this.m_TopContent[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("已达到最高等级")
				});
				this.m_TopContent[4].text = string.Empty;
			}
			else
			{
				this.m_TopContent[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("下级效果：")
				});
				string text5 = string.Format("{0}%", Global.GetJIngLingSkillAddAttack(magicScripts, Lev + 1));
				this.m_TopContent[4].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(text3, text5)
				});
			}
			if (this.GetPetSkillLevelupCost() == -1)
			{
				this.m_BownContent[0].gameObject.SetActive(true);
				this.m_BownContent[0].text = Global.GetLang("已达到最高等级");
				this.m_BownContent[1].transform.parent.gameObject.SetActive(false);
				this.m_UpLevBtn.gameObject.SetActive(false);
				Global.RefreshUI(this.m_UpLevBtn.gameObject);
			}
			else
			{
				this.m_BownContent[0].gameObject.SetActive(false);
				this.m_BownContent[1].transform.parent.gameObject.SetActive(true);
				this.SkillUpCost = this.GetPetSkillLevelupCost();
				this.m_UpLevBtn.gameObject.SetActive(true);
				this.m_UpLevBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.UpLevBtnHander);
			}
		}
		else
		{
			this.m_TopContent[0].text = string.Empty;
			this.m_TopContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("无技能效果")
			});
			this.m_TopContent[2].text = string.Concat(new string[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("通过")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("领悟")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("可以获得精灵技能")
				}),
				Environment.NewLine,
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("提升")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("技能槽位可增强技能效果")
				})
			});
			this.m_TopContent[3].text = string.Empty;
			this.m_TopContent[4].text = string.Empty;
			this.m_BownContent[0].gameObject.SetActive(false);
			this.m_BownContent[1].transform.parent.gameObject.SetActive(true);
			this.SkillUpCost = 0;
			this.m_UpLevBtn.gameObject.SetActive(true);
			this.m_UpLevBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Empty);
		}
		this.YuanSuXiaohao = this.m_SkillAwarkCost;
		this.ClearSkillUpEffect();
	}

	public void ChangeSkillsInfParent(jinglingSkillSignItem[] Skills)
	{
		for (int i = 0; i < Skills.Length; i++)
		{
			if (i < this.m_SkillRoot.Length)
			{
				Transform transform = this.m_SkillRoot[i];
				Vector3 localPosition = transform.localPosition;
				localPosition.z -= 9f;
				if (null != transform)
				{
					Skills[i].transform.SetParent(transform.parent, false);
					Skills[i].transform.localPosition = localPosition;
				}
			}
		}
	}

	public int SkillUpCost
	{
		set
		{
			if (this.m_IsOpen)
			{
				this.m_BownContent[2].text = ((value <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue)) ? string.Format("{0}", this.GetPetSkillLevelupCost()) : Global.GetColorStringForNGUIText(new object[]
				{
					"d02929",
					string.Format("{0}", this.GetPetSkillLevelupCost())
				}));
			}
			else
			{
				this.m_BownContent[2].text = string.Empty;
			}
		}
	}

	public int Mode_z
	{
		set
		{
			this.m_Mode_z = value;
		}
	}

	public int Diamond
	{
		get
		{
			return int.Parse(this.m_Money[1].text);
		}
		set
		{
			if (Global.GetRolePatHaveJingling())
			{
				if (value <= Global.Data.roleData.UserMoney)
				{
					this.m_Money[1].text = value.ToString();
				}
				else
				{
					this.m_Money[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						"d02929",
						value
					});
				}
			}
			else
			{
				this.m_Money[1].text = string.Empty;
			}
		}
	}

	public int YuanSuXiaohao
	{
		get
		{
			return int.Parse(this.m_Money[0].text);
		}
		set
		{
			if (Global.GetRolePatHaveJingling())
			{
				if (this.m_SkillAwarkCost <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue))
				{
					this.m_Money[0].text = value.ToString();
				}
				else
				{
					this.m_Money[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"d02929",
						value
					});
				}
			}
			else
			{
				this.m_Money[0].text = string.Empty;
			}
		}
	}

	public GButton m_UpLevBtn;

	public UILabel m_TopTitle;

	public UILabel m_Bownlabel;

	public UILabel[] m_TopContent;

	public UILabel[] m_BownContent;

	public GameObject[] m_RootObj;

	public ListBox m_ListBox;

	public UILabel m_XiaoHao;

	public UILabel[] m_Money;

	public GButton m_GraspSkillBtn;

	public GButton m_SkillPreView;

	public GButton m_Helpbtn;

	public UIScrollBar m_ScrollBar;

	public UILabel[] m_HelpContent;

	public UISprite m_HelpMasp;

	public GameObject m_HelpObj;

	public GButton m_HelpCloseBtn;

	public GButton m_SkillChuanCheng;

	private Dictionary<int, int> m_DicSkillAwarkCostDiamond = new Dictionary<int, int>();

	private Dictionary<int, int> m_DicSoltUpCost = new Dictionary<int, int>();

	private Dictionary<int, XElement> m_DidXML = new Dictionary<int, XElement>();

	private XElement xml_Magics;

	private ObservableCollection m_Collection;

	private UIDraggablePanel m_DraggablePanel;

	private int selectIndex = -1;

	private int m_SkillAwarkCost;

	private int m_SlotLev;

	private List<JingLingSkillGraspItem> m_JingLingSkillGraspItemLst = new List<JingLingSkillGraspItem>();

	private List<int> m_LStSkillLock = new List<int>();

	private int m_DbId;

	private int m_SkillID;

	private int m_SoltId;

	private int m_Mode_z = -10;

	private GameObject m_SkillTeXiaoObj_Power;

	private Transform m_SkillTeXiaoObj_PowerJ_Power;

	private GameObject m_SkillTeXiao_jiemian;

	private Transform[] m_SkillRoot;

	private int m_MaxLev;

	private bool m_IsOpen;

	private Dictionary<int, int> m_DicSlotOpenLev = new Dictionary<int, int>();

	private string[] str_Help = new string[]
	{
		Global.GetLang("技能领悟"),
		string.Concat(new string[]
		{
			Global.GetLang("1、精灵每达到"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("30")
			}),
			Global.GetLang("、"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("40")
			}),
			Global.GetLang("、"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("50")
			}),
			Global.GetLang("、"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("60")
			}),
			Global.GetLang("级都会开启一个技能槽"),
			Environment.NewLine,
			Global.GetLang("2、精灵通过"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("领悟")
			}),
			Global.GetLang("可以随机获得一个技能,新技能随机放在一个开启的技能槽里，"),
			Global.GetLang("如果该技能槽已经有技能，原技能"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"d02929",
				Global.GetLang("会被新技能顶替")
			}),
			Environment.NewLine,
			Global.GetLang("3、花费钻石可以"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("锁定技能")
			}),
			Global.GetLang("，被锁定的技能无法被新技能顶替")
		}),
		Global.GetLang("技能槽"),
		string.Concat(new string[]
		{
			Global.GetLang("1、提升"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("技能槽等级")
			}),
			Global.GetLang("可以提高精灵技能触发后的效果"),
			Environment.NewLine,
			Global.GetLang("2、技能槽中的技能被替换，技能槽"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("等级不会变")
			}),
			Environment.NewLine,
			Global.GetLang("3、精灵被回收时，提升技能槽所消耗的灵晶会被返还")
		})
	};

	public enum SkillPartInfType
	{
		Skills,
		SkillsGrasp
	}
}
