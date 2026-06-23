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

public class jinglingPart : UserControl
{
	public jinglingSystemPart.BtnType PartType
	{
		get
		{
			return this.m_PartType;
		}
		set
		{
			this.m_PartType = value;
			this.m_JinglingContent.SetActive(jinglingSystemPart.BtnType.JingLing == value);
			this.JingLingSkillInfPart(value, false);
			bool active = jinglingSystemPart.BtnType.JingLing == value;
			this.buttonShuXingQiYuan.gameObject.SetActive(active);
			this.buttonJinglingQiYuan.gameObject.SetActive(active);
			if (ConfigVersionSystemOpen.IsVersionSystemOpen(100101) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuanSuJueXing))
			{
				this.yuanSuJueXingBtn.gameObject.SetActive(active);
			}
			this.chuZhanButton.gameObject.SetActive(active);
			this.HuntButton.gameObject.SetActive(active);
			this.LevelUP.gameObject.SetActive(active);
			this.m_NeedExp = ((!(null == this.m_NeedExp)) ? this.m_NeedExp : base.FindName("NeedExp"));
			this.m_NeedExp.SetActive(active);
			this.m_SkillObj.SetActive(this.m_JingLingSkillsIsOpen && (value == jinglingSystemPart.BtnType.JingLing || jinglingSystemPart.BtnType.JingLingSkills == value));
			this.m_JingLingInF = ((!(null == this.m_JingLingInF)) ? this.m_JingLingInF : base.FindName("JingLingInf"));
			this.m_JingLingInF.SetActive(jinglingSystemPart.BtnType.JingLing == value);
			this.level.gameObject.SetActive(active);
			this.openBag.gameObject.SetActive(active);
			this.model.gameObject.SetActive(jinglingSystemPart.BtnType.JingLingSkills != value);
		}
	}

	public void RefreshSkillAwakeCost(int cost)
	{
		if (null != this.m_JingLingSkillInfPart)
		{
			if (Global.Data.equipPet != null && 0 < Global.Data.equipPet.Count)
			{
				this.m_JingLingSkillInfPart.RefreshSkillAwakeCost(cost);
			}
			else
			{
				this.m_JingLingSkillInfPart.RefreshSkillAwakeCost(0);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	public void RefreshSkillUpData()
	{
		this.m_SkillSigns[this.SkillSelectIndex].Lev++;
		List<int> list = new List<int>();
		for (int i = 0; i < this.m_SkillSigns.Length; i++)
		{
			list.Add((!this.m_SkillSigns[i].IsOpen) ? 0 : 1);
			list.Add(this.m_SkillSigns[i].Lev);
			list.Add(this.m_SkillSigns[i].SkillId);
		}
		Global.Data.equipPet[this.currentSelect].ElementhrtsProps = list;
		if (null != this.m_JingLingSkillInfPart)
		{
			this.m_JingLingSkillInfPart.SetSkillInF(this.m_SkillSigns[this.SkillSelectIndex].SkillId, this.m_SkillSigns[this.SkillSelectIndex].Lev, this.TempJinglingData.Id, this.SkillSelectIndex, this.m_SkillSigns[this.SkillSelectIndex].IsOpen);
			this.m_JingLingSkillInfPart.RefreshSkillUp(this.SkillSelectIndex);
		}
	}

	public void RefreshSkillAwarkData(List<int> data)
	{
		for (int i = 0; i < this.m_isNew.Length; i++)
		{
			this.m_isNew[i] = false;
		}
		if (Global.Data.equipPet[this.currentSelect].ElementhrtsProps == null)
		{
			int num = 0;
			Global.Data.equipPet[this.currentSelect].ElementhrtsProps = data;
			for (int j = 0; j < data.Count; j++)
			{
				if (j % 3 == 2)
				{
					this.m_isNew[num++] = (data[j] != 0);
				}
			}
		}
		else
		{
			int num2 = 0;
			for (int k = 0; k < data.Count; k++)
			{
				if (k % 3 == 2)
				{
					this.m_isNew[num2++] = (Global.Data.equipPet[this.currentSelect].ElementhrtsProps[k] != data[k]);
				}
			}
			Global.Data.equipPet[this.currentSelect].ElementhrtsProps = data;
		}
		GameInstance.Game.SendGetJingLingSkillGraspNeedLingJing();
		this.InitData(Global.Data.equipPet[this.currentSelect]);
		this.JingLingSkillInfPart(this.m_PartType, true);
		for (int l = 0; l < this.m_isNew.Length; l++)
		{
			if (this.m_isNew[l])
			{
				this.RefreshSkillsTeXiao(l);
				break;
			}
		}
	}

	private void RefreshSkillsTeXiao(int NewSlot)
	{
		if (null != this.m_JingLingSkillInfPart)
		{
			this.m_JingLingSkillInfPart.RefreshSkillsTeXiao(NewSlot);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	private void ChangePutBagActive(bool bActive = true)
	{
		if (this.putBackBag != null)
		{
			if (!bActive)
			{
				for (int i = 0; i < this.putBackBag.Length; i++)
				{
					this.putBackBag[i].gameObject.SetActive(bActive);
				}
			}
			else
			{
				for (int j = 0; j < this.putBackBag.Length; j++)
				{
					this.putBackBag[j].gameObject.SetActive(false);
				}
				for (int k = 0; k < Global.Data.equipPet.Count; k++)
				{
					if (Global.Data.equipPet[k] != null)
					{
						this.putBackBag[k].gameObject.SetActive(true);
					}
				}
			}
		}
	}

	private void JingLingSkillInfPart(jinglingSystemPart.BtnType type, bool bFormAwark = false)
	{
		bool flag = type == jinglingSystemPart.BtnType.JingLingSkills || jinglingSystemPart.BtnType.SkillsGrasp == type;
		if (flag)
		{
			if (null == this.m_JingLingSkillInfPart)
			{
				this.m_JingLingSkillInfPart = U3DUtils.NEW<JingLingSkillsInFPart>();
				this.m_JingLingSkillInfPart.transform.SetParent(base.transform, false);
				this.m_JingLingSkillInfPart.gameObject.SetActive(flag);
			}
			this.m_JingLingSkillInfPart.Mode_z = (int)this.model.transform.localPosition.z - 300;
			if (type == jinglingSystemPart.BtnType.JingLingSkills)
			{
				this.m_JingLingSkillInfPart.SetPratType(JingLingSkillsInFPart.SkillPartInfType.Skills);
				if (this.TempJinglingData != null)
				{
					this.m_JingLingSkillInfPart.SetSkillInF(this.m_SkillSigns[this.SkillSelectIndex].SkillId, this.m_SkillSigns[this.SkillSelectIndex].Lev, this.TempJinglingData.Id, this.SkillSelectIndex, this.m_SkillSigns[this.SkillSelectIndex].IsOpen);
				}
				else
				{
					this.m_JingLingSkillInfPart.SetSkillInF(0, 0, 0, this.SkillSelectIndex, this.m_SkillSigns[this.SkillSelectIndex].IsOpen);
				}
				this.m_JingLingSkillInfPart.ChangeTeXiaoAngle(this.SkillSelectIndex);
			}
			else if (type == jinglingSystemPart.BtnType.SkillsGrasp)
			{
				this.m_JingLingSkillInfPart.SetPratType(JingLingSkillsInFPart.SkillPartInfType.SkillsGrasp);
			}
		}
		if (type == jinglingSystemPart.BtnType.JingLing)
		{
			Vector3[] array = (type != jinglingSystemPart.BtnType.JingLing) ? this.m_SkillsPos_Skills : this.m_SkillsPos_jingling;
			for (int i = 0; i < this.m_SkillSigns.Length; i++)
			{
				if (this.currentSelect < Global.Data.equipPet.Count && 0 <= this.currentSelect)
				{
					this.m_SkillSigns[i].SetSignType(jinglingSkillSignItem.SignType.jingLing, false);
				}
				else
				{
					this.m_SkillSigns[i].SetSignType(jinglingSkillSignItem.SignType.jingLing, true);
				}
				this.m_SkillSigns[i].transform.SetParent(this.m_SkillObj.transform, false);
				Global.RefreshUI(this.m_SkillSigns[i].gameObject);
			}
			if (this.currentSelect < Global.Data.equipPet.Count && 0 <= this.currentSelect)
			{
				this.InitData(Global.Data.equipPet[this.currentSelect]);
			}
		}
		else if (type == jinglingSystemPart.BtnType.JingLingSkills)
		{
			this.m_JingLingSkillInfPart.ChangeSkillsInfParent(this.m_SkillSigns);
			for (int j = 0; j < this.m_SkillSigns.Length; j++)
			{
				this.m_SkillSigns[j].SetSignType(jinglingSkillSignItem.SignType.SkillUp, false);
				this.m_SkillSigns[j].IsOpen = this.m_SkillSigns[j].IsOpen;
				this.m_SkillSigns[j].SkillId = this.m_SkillSigns[j].SkillId;
				this.m_SkillSigns[j].SkillStep = this.m_SkillSigns[j].SkillStep;
				this.m_SkillSigns[j].Lev = this.m_SkillSigns[j].Lev;
			}
		}
		else if (type == jinglingSystemPart.BtnType.SkillsGrasp)
		{
			if (this.currentSelect < Global.Data.equipPet.Count && 0 <= this.currentSelect)
			{
				this.InitData(Global.Data.equipPet[this.currentSelect]);
				if (this.TempJinglingData != null)
				{
					this.m_JingLingSkillInfPart.SetSkillGraspInf(this.m_SkillSigns, this.TempJinglingData.Id);
				}
				else
				{
					this.m_JingLingSkillInfPart.SetSkillGraspInf(this.m_SkillSigns, 0);
				}
			}
			else
			{
				this.m_JingLingSkillInfPart.SetSkillGraspInf(this.m_SkillSigns, 0);
			}
			if (!bFormAwark)
			{
				this.m_JingLingSkillInfPart.ClearLock();
			}
		}
		this.ChangePutBagActive(!flag);
		this.ActiveObj(this.m_JingLingSkillInfPart, flag);
		this.ChangeGoodsIconS_Z();
		this.ChangeBGTexture();
	}

	private void ActiveObj(Component o, bool bActive = true)
	{
		if (null != o)
		{
			NGUITools.SetActive(o, bActive);
		}
	}

	private void ChangeBGTexture()
	{
		this.m_BgTexture.URL = ((this.m_PartType != jinglingSystemPart.BtnType.JingLingSkills) ? "NetImages/GameRes/Images/Plate/Geniusback.jpg" : "NetImages/GameRes/Images/Plate/jingLingSkillUp.jpg");
	}

	private void ChangeGoodsIconS_Z()
	{
		float z = (this.m_PartType != jinglingSystemPart.BtnType.JingLingSkills && this.m_PartType != jinglingSystemPart.BtnType.SkillsGrasp) ? -0.03f : -0.01f;
		for (int i = 0; i < this.iconList.Count; i++)
		{
			GGoodIcon ggoodIcon = this.iconList[i];
			Vector3 localPosition = ggoodIcon.transform.localPosition;
			localPosition.z = z;
			ggoodIcon.transform.localPosition = localPosition;
		}
		float num = (this.m_PartType != jinglingSystemPart.BtnType.JingLingSkills && this.m_PartType != jinglingSystemPart.BtnType.SkillsGrasp) ? -0.03f : -2.03f;
		for (int j = 0; j < this.fightArr.Length; j++)
		{
			Vector3 localPosition2 = this.fightArr[j].transform.localPosition;
			localPosition2.z = z;
			this.fightArr[j].transform.localPosition = localPosition2;
		}
	}

	public void ShouHuChongLoaderComplete(ShouHuChongLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		if (this.model.ChildGameObjectList.Count != 0)
		{
			this.model.Clear();
		}
		U3DUtils.AddChild(this.model.gameObject, go, false);
		this.model.ChildGameObjectList.Add(go);
		this.model._Target = go;
		go.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (this.XMLPet == null)
		{
			this.XMLPet = Global.GetGameResXml("Config/PetHabitus.Xml");
		}
		if (this.XMLPetList == null)
		{
			this.XMLPetList = Global.GetXElementList(this.XMLPet, "Pet");
		}
		for (int i = 0; i < this.XMLPetList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(this.XMLPetList[i], "ID");
			if (xelementAttributeInt == Global.Data.equipPet[this.currentSelect].GoodsID)
			{
				float xelementAttributeFloat = Global.GetXElementAttributeFloat(this.XMLPetList[i], "Habitus");
				go.transform.localScale = new Vector3(xelementAttributeFloat, xelementAttributeFloat, xelementAttributeFloat);
				float xelementAttributeFloat2 = Global.GetXElementAttributeFloat(this.XMLPetList[i], "XPoint");
				float xelementAttributeFloat3 = Global.GetXElementAttributeFloat(this.XMLPetList[i], "YPoint");
				float xelementAttributeFloat4 = Global.GetXElementAttributeFloat(this.XMLPetList[i], "ZPoint");
				go.transform.localRotation = Quaternion.Euler(new Vector3(xelementAttributeFloat2, xelementAttributeFloat3, xelementAttributeFloat4));
				break;
			}
		}
		ShouHuChongController shouHuChongController = go.AddComponent<ShouHuChongController>();
		shouHuChongController.LoaderURL = loader.LoaderURL;
		shouHuChongController.UseType = 1;
		shouHuChongController.InitController(go, loader.parent.transform);
		shouHuChongController.Action = GPetActions.Idle;
		U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer("MUUI"), null);
		if (null != go.gameObject.GetComponent<Renderer>())
		{
			Material[] materials = go.gameObject.GetComponent<Renderer>().materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].renderQueue < 3000)
				{
					materials[j].renderQueue = 3000;
				}
			}
		}
		if (this.m_JingLingSkillsIsOpen)
		{
			this.ChangeSkillSginCollider_Z((int)this.model.transform.localPosition.z);
		}
		go.AddComponent<LoadUIShaderAgain>();
		LookAtCamera[] componentsInChildren = go.GetComponentsInChildren<LookAtCamera>();
		if (componentsInChildren != null)
		{
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				if (null != componentsInChildren[k])
				{
					componentsInChildren[k].enabled = false;
					LookAtCamera_UI lookAtCamera_UI = componentsInChildren[k].GetComponent<LookAtCamera_UI>();
					if (null == lookAtCamera_UI)
					{
						lookAtCamera_UI = componentsInChildren[k].gameObject.AddComponent<LookAtCamera_UI>();
					}
				}
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.level.text = string.Empty;
		this.petName.text = string.Empty;
		for (int i = 0; i < this.giftAttribute.Length; i++)
		{
			this.giftAttribute[i].text = string.Empty;
		}
		this.ownMoHe.text = string.Empty;
		this.castMoHe.text = string.Empty;
		this.titleAttr.text = Global.GetLang("基础属性");
		this.titleGiftAttr.text = Global.GetLang("天赋属性");
		this.xiaoHao.text = Global.GetLang("消耗");
		this.LevelUP.Text = Global.GetLang("升级");
		for (int j = 0; j < this.SelectArr.Length; j++)
		{
			this.SelectArr[j].gameObject.SetActive(false);
			this.fightArr[j].gameObject.SetActive(false);
			this.plusArr[j].gameObject.SetActive(true);
		}
	}

	private void InitSkillData()
	{
		if (this.m_SkillSigns == null)
		{
			jinglingSkillSignItem.SignType type = jinglingSkillSignItem.SignType.jingLing;
			if (this.m_PartType == jinglingSystemPart.BtnType.JingLing)
			{
				type = jinglingSkillSignItem.SignType.jingLing;
			}
			else if (this.m_PartType == jinglingSystemPart.BtnType.JingLingSkills)
			{
				type = jinglingSkillSignItem.SignType.SkillUp;
			}
			else if (this.m_PartType == jinglingSystemPart.BtnType.SkillsGrasp)
			{
				type = jinglingSkillSignItem.SignType.SkillGrasp;
			}
			this.m_SkillSigns = new jinglingSkillSignItem[4];
			for (byte b = 0; b < 4; b += 1)
			{
				jinglingSkillSignItem jinglingSkillSignItem = U3DUtils.NEW<jinglingSkillSignItem>();
				jinglingSkillSignItem.SetSignType(type, false);
				jinglingSkillSignItem.SlotIndex = (int)b;
				jinglingSkillSignItem.IsOpen = false;
				jinglingSkillSignItem.SkillId = 0;
				jinglingSkillSignItem.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					this.SkillSelectIndex = s.MyID;
					if (this.m_PartType == jinglingSystemPart.BtnType.JingLing)
					{
						if (this.m_SkillSigns[this.SkillSelectIndex].IsOpen)
						{
							this.ShowJingLingTips(this.SkillSelectIndex);
						}
						else
						{
							Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(this.m_SkillSigns[this.SkillSelectIndex].SlotIndex)), 10, 3);
						}
					}
					else if (this.m_PartType == jinglingSystemPart.BtnType.JingLingSkills && null != this.m_JingLingSkillInfPart)
					{
						if (this.TempJinglingData != null)
						{
							this.m_JingLingSkillInfPart.SetSkillInF(this.m_SkillSigns[this.SkillSelectIndex].SkillId, this.m_SkillSigns[this.SkillSelectIndex].Lev, this.TempJinglingData.Id, this.SkillSelectIndex, this.m_SkillSigns[this.SkillSelectIndex].IsOpen);
						}
						else
						{
							this.m_JingLingSkillInfPart.SetSkillInF(0, 0, 0, this.SkillSelectIndex, this.m_SkillSigns[this.SkillSelectIndex].IsOpen);
						}
						this.m_JingLingSkillInfPart.ChangeTeXiaoAngle(this.SkillSelectIndex);
						if (this.m_SkillSigns[this.SkillSelectIndex].IsOpen)
						{
							if (0 >= this.m_SkillSigns[this.SkillSelectIndex].SkillId)
							{
								PlayZone.GlobalPlayZone.ShowJingLingPart(jinglingSystemPart.BtnType.SkillsGrasp);
							}
						}
						else
						{
							Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(this.m_SkillSigns[this.SkillSelectIndex].SlotIndex)), 10, 3);
						}
					}
				};
				jinglingSkillSignItem.Collider_z = ((!(null == this.model)) ? ((int)(this.model.transform.localPosition.z - 10f)) : 0);
				jinglingSkillSignItem.transform.SetParent(this.m_SkillObj.transform, false);
				this.m_SkillSigns[(int)b] = jinglingSkillSignItem;
			}
		}
	}

	private void ShowJingLingTips(int id)
	{
		if (id < this.m_SkillSigns.Length)
		{
			if (this.m_SkillSigns[id].IsOpen)
			{
				if (this.m_SkillSigns[id].SkillId != 0)
				{
					this.m_SkillSigns[id].SelectEffectActive = true;
					GChildWindow w = U3DUtils.NEW<GChildWindow>();
					w.Modal = false;
					w.ModalBak.SetActive(true);
					w.ModalBakSprite.enabled = false;
					w.transform.localPosition = new Vector3(0f, 0f, this.model.transform.localPosition.z - 300f);
					w.transform.SetParent(base.transform, false);
					JingLingSkillTips jingLingSkillTips = U3DUtils.NEW<JingLingSkillTips>();
					jingLingSkillTips.SetSkillId(this.m_SkillSigns[id].SkillId, this.m_SkillSigns[id].Lev);
					w.ChildWindowModalBakClick = delegate(object s, EventArgs e)
					{
						Object.Destroy(w.gameObject, 0.01f);
						this.m_SkillSigns[id].SelectEffectActive = false;
						return true;
					};
					w.Body.Add(jingLingSkillTips);
				}
				else
				{
					Super.HintMainText(Global.GetLang("技能槽位没有技能"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(id)), 10, 3);
			}
		}
	}

	private void ChangeSkillSginCollider_Z(int z)
	{
		if (this.m_SkillSigns != null)
		{
			byte b = 0;
			while ((int)b < this.m_SkillSigns.Length)
			{
				if (null != this.m_SkillSigns[(int)b])
				{
					this.m_SkillSigns[(int)b].Collider_z = z;
				}
				b += 1;
			}
		}
		if (null != this.m_JingLingSkillInfPart)
		{
			this.m_JingLingSkillInfPart.Mode_z = (int)this.model.transform.localPosition.z - 300;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitSkillData();
		this.mPropOBC = this.PropListBox.ItemsSource;
		this.m_JingLingSkillsIsOpen = Global.GetJingLingSkillIsOpen();
		this.ownMoHe.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
		if (Global.Data.equipPet != null)
		{
			this.SetEquipPet();
		}
		this.chuZhanButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.currentSelect + 1 > Global.Data.equipPet.Count)
			{
				return;
			}
			for (int i = 0; i < this.fightArr.Length; i++)
			{
				this.fightArr[i].gameObject.SetActive(false);
			}
			if ("gofight" == this.chuZhanButton.target.spriteName)
			{
				this.chuZhanButton.hoverSprite = "goback";
				this.chuZhanButton.normalSprite = "goback";
				this.chuZhanButton.pressedSprite = "goback";
				this.chuZhanButton.disabledSprite = "goback";
				this.chuZhanButton.target.spriteName = "goback";
				this.fightArr[this.currentSelect].gameObject.SetActive(true);
				if (Global.Data.equipPet.Count != 0)
				{
					Global.ToUseGoods(Global.Data.equipPet[this.currentSelect], true, false);
				}
			}
			else
			{
				this.chuZhanButton.hoverSprite = "gofight";
				this.chuZhanButton.normalSprite = "gofight";
				this.chuZhanButton.pressedSprite = "gofight";
				this.chuZhanButton.disabledSprite = "gofight";
				this.chuZhanButton.target.spriteName = "gofight";
				this.fightArr[this.currentSelect].gameObject.SetActive(false);
				if (Global.Data.equipPet.Count != 0 && Global.Data.equipPet[this.currentSelect].Using == 1)
				{
					Global.Data.equipPet[this.currentSelect].Using = 0;
					GameInstance.Game.SpriteModGoods(2, Global.Data.equipPet[this.currentSelect].Id, Global.Data.equipPet[this.currentSelect].GoodsID, Global.Data.equipPet[this.currentSelect].Using, Global.Data.equipPet[this.currentSelect].Site, Global.Data.equipPet[this.currentSelect].GCount, Global.Data.equipPet[this.currentSelect].BagIndex, string.Empty);
				}
			}
		};
		this.putBackBag[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.BackBagHandle(0);
		};
		this.putBackBag[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.BackBagHandle(1);
		};
		this.putBackBag[2].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.BackBagHandle(2);
		};
		this.putBackBag[3].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.BackBagHandle(3);
		};
		this.buttonShuXingQiYuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GChildWindow JingLingShuXinQiyuanWindow = U3DUtils.NEW<GChildWindow>();
			JingLingShuXinQiyuanWindow.ModalType = ChildWindowModalType.TranslucentGUI;
			UIEventListener.Get(JingLingShuXinQiyuanWindow.ModalBak).onClick = delegate(GameObject go)
			{
				Object.Destroy(JingLingShuXinQiyuanWindow.gameObject, 0.01f);
			};
			base.Children.Add(JingLingShuXinQiyuanWindow);
			JingLingShuXinQiyuanPart jingLingShuXinQiyuanPart = U3DUtils.NEW<JingLingShuXinQiyuanPart>();
			JingLingShuXinQiyuanWindow.Children.Add(jingLingShuXinQiyuanPart.gameObject);
			JingLingShuXinQiyuanWindow.ZIndex = 900.0;
		};
		this.buttonJinglingQiYuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GChildWindow JingLingShuXinQiyuanWindow = U3DUtils.NEW<GChildWindow>();
			JingLingShuXinQiyuanWindow.ModalType = ChildWindowModalType.TranslucentGUI;
			UIEventListener.Get(JingLingShuXinQiyuanWindow.ModalBak).onClick = delegate(GameObject go)
			{
				Object.Destroy(JingLingShuXinQiyuanWindow.gameObject, 0.01f);
			};
			base.Children.Add(JingLingShuXinQiyuanWindow);
			JingLingQiyuanPart jingLingQiyuanPart = U3DUtils.NEW<JingLingQiyuanPart>();
			JingLingShuXinQiyuanWindow.Body.Children.Add(jingLingQiyuanPart.gameObject);
			JingLingShuXinQiyuanWindow.ZIndex = 900.0;
			jingLingQiyuanPart.btnClose.MouseLeftButtonUp = delegate(object h, MouseEvent k)
			{
				Object.Destroy(JingLingShuXinQiyuanWindow.gameObject, 0.01f);
			};
		};
		this.HuntButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LieQuYuanSu, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.LieQuYuanSu, trigger, param, param2, true);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.LevelUP.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.currentSelect + 1 > Global.Data.equipPet.Count)
			{
				return;
			}
			if (Global.Data.equipPet[this.currentSelect].Binding == 0)
			{
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				string lang = Global.GetLang("升级后您的宠物将变为绑定，确认要执行该操作吗?");
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					if (e1.ID == 0)
					{
						this.PetLevelUp();
					}
				}, buttons);
			}
			else
			{
				this.PetLevelUp();
			}
		};
		this.openBag.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = 703
			});
		};
		this.GetLingJing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLingJing, null, string.Empty, Global.GetLang("灵晶"));
		};
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100101) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuanSuJueXing))
		{
			this.yuanSuJueXingBtn.gameObject.SetActive(true);
			this.yuanSuJueXingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -10
				});
				PlayZone.GlobalPlayZone.OpenYuanSuJueXingWindow(true, -1);
			};
		}
		else
		{
			this.yuanSuJueXingBtn.gameObject.SetActive(false);
		}
		ActivityTipManager.RegActivityTipItem(16001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		UIHelper.SetModalPosZ(this.model.transform);
	}

	private void BackBagHandle(int index)
	{
		if (Global.Data.equipPet != null && index < Global.Data.equipPet.Count)
		{
			Super.ShowNetWaiting(string.Empty);
			if (Global.Data.equipPet[index].Using == 1)
			{
				Global.Data.equipPet[index].Using = 0;
				this.fightArr[index].gameObject.SetActive(false);
				this.chuZhanButton.hoverSprite = "gofight";
				this.chuZhanButton.normalSprite = "gofight";
				this.chuZhanButton.pressedSprite = "gofight";
				this.chuZhanButton.disabledSprite = "gofight";
				this.chuZhanButton.target.spriteName = "gofight";
				GameInstance.Game.SpriteModGoods(2, Global.Data.equipPet[index].Id, Global.Data.equipPet[index].GoodsID, Global.Data.equipPet[index].Using, Global.Data.equipPet[index].Site, Global.Data.equipPet[index].GCount, Global.Data.equipPet[index].BagIndex, string.Empty);
			}
			GameInstance.Game.SpriteModGoods(3, Global.Data.equipPet[index].Id, Global.Data.equipPet[index].GoodsID, Global.Data.equipPet[index].Using, 0, Global.Data.equipPet[index].GCount, Global.Data.equipPet[index].BagIndex, string.Empty);
			if (null != this.m_JingLingSkillInfPart)
			{
				this.m_JingLingSkillInfPart.ClearLock();
			}
		}
	}

	private void OnActivityStateChanged(int type, ActivityTipItem args)
	{
		if (type == 16001)
		{
			this.TipIconLieQu.SetActive(args.IsActive);
		}
	}

	private void PetLevelUp()
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.equipPet[this.currentSelect].GoodsID);
		if (this.TempCastMoHe <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue) && Global.Data.equipPet[this.currentSelect].Forge_level < goodsXmlNodeByID.SuitID * 10 + 9)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.effect);
			gameObject.transform.parent = this.effectPosition;
			U3DUtils.ReplaceLayerInChildren(gameObject, LayerMask.NameToLayer("MUUI"), null);
			gameObject.transform.localPosition = new Vector3(-167f, -122f, -300f);
			gameObject.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
			DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
			delayDestroy.delayTime = 1f;
		}
		this.ExecuteForgePet();
	}

	private void ClearData()
	{
		this.level.text = string.Empty;
		this.castMoHe.text = string.Empty;
		for (int i = 0; i < this.giftAttribute.Length; i++)
		{
			this.giftAttribute[i].text = string.Empty;
		}
		if ((this.m_PartType == jinglingSystemPart.BtnType.JingLing || this.m_PartType == jinglingSystemPart.BtnType.SkillsGrasp) && this.model.ChildGameObjectList.Count != 0)
		{
			this.model.Clear();
		}
	}

	private void InitData(GoodsData goodData)
	{
		this.TempJinglingData = goodData;
		this.ClearData();
		if (goodData.Using == 0)
		{
			this.chuZhanButton.hoverSprite = "gofight";
			this.chuZhanButton.normalSprite = "gofight";
			this.chuZhanButton.pressedSprite = "gofight";
			this.chuZhanButton.disabledSprite = "gofight";
			this.chuZhanButton.target.spriteName = "gofight";
		}
		else
		{
			this.chuZhanButton.hoverSprite = "goback";
			this.chuZhanButton.normalSprite = "goback";
			this.chuZhanButton.pressedSprite = "goback";
			this.chuZhanButton.disabledSprite = "goback";
			this.chuZhanButton.target.spriteName = "goback";
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			this.petName.text = "{" + goodsXmlNodeByID.GoodsColor + "}";
			UILabel uilabel = this.petName;
			uilabel.text += goodsXmlNodeByID.Title;
			UILabel uilabel2 = this.petName;
			uilabel2.text += "{-}";
		}
		this.level.text = Global.GetLang("{FAC60D}当前等级{-}:") + "{FFFFFF}" + (goodData.Forge_level + 1).ToString() + "{-}";
		if (this.XMLLevel == null)
		{
			this.XMLLevel = Global.GetGameResXml("Config/PetLevelUp.Xml");
		}
		if (this.XMLLevelList == null)
		{
			this.XMLLevelList = Global.GetXElementList(this.XMLLevel, "PetLevelUp");
		}
		for (int i = 0; i < this.XMLLevelList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(this.XMLLevelList[i], "Level");
			if (xelementAttributeInt == goodData.Forge_level + 2)
			{
				this.TempCastMoHe = Global.GetXElementAttributeInt(this.XMLLevelList[i], "NeedEXP");
				break;
			}
		}
		this.RefreshAttribute();
		if (this.m_PartType == jinglingSystemPart.BtnType.JingLing || this.m_PartType == jinglingSystemPart.BtnType.SkillsGrasp)
		{
			if (this.realShouHuChongLoader != null)
			{
				this.realShouHuChongLoader.Stop();
			}
			this.realShouHuChongLoader = new ShouHuChongResLoader(new ShouHuChongLoadData
			{
				parent = this.model.gameObject,
				data = new GoodsData(),
				data = 
				{
					GoodsID = goodData.GoodsID,
					Using = 1
				},
				ReplaceChildLayer = true,
				HideGameEffect = true,
				ToLayer = LayerMask.NameToLayer("GUI")
			}, new OnShouHuChongLoadComplete(this.ShouHuChongLoaderComplete));
		}
		if (this.TempCastMoHe > Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue))
		{
			this.LevelUP.isEnabled = false;
			this.castMoHe.text = "{FF0000}" + this.TempCastMoHe + "{-}";
		}
		else
		{
			this.LevelUP.isEnabled = true;
			this.castMoHe.text = "{FFFFFF}" + this.TempCastMoHe + "{-}";
		}
		if (this.m_JingLingSkillsIsOpen)
		{
			if (goodData.ElementhrtsProps != null)
			{
				bool[] array = this.CheckJingLingLev(goodData.Forge_level + 1);
				int num = 0;
				for (int j = 0; j < goodData.ElementhrtsProps.Count; j++)
				{
					if (j % 3 == 0)
					{
						goodData.ElementhrtsProps[j] = ((!array[num++]) ? 0 : 1);
					}
				}
			}
			this.InitSkillsData(goodData.ElementhrtsProps, goodData.Forge_level + 1);
			if (null != this.model)
			{
				this.ChangeSkillSginCollider_Z((int)this.model.transform.localPosition.z - 10);
			}
		}
	}

	private void RefreshAttribute()
	{
		this.mPropOBC.Clear();
		GoodsData tempJinglingData = this.TempJinglingData;
		List<string> petAttribute = Global.GetPetAttribute(tempJinglingData, 0);
		List<string> petAttribute2 = Global.GetPetAttribute(tempJinglingData, 1);
		try
		{
			for (int i = 0; i < petAttribute.Count; i++)
			{
				JingLingPropInF jingLingPropInF = U3DUtils.NEW<JingLingPropInF>();
				jingLingPropInF.LabelPropInfName.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					petAttribute[i]
				});
				if (i < petAttribute2.Count)
				{
					jingLingPropInF.LabelPropInfValue1.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						petAttribute2[i]
					});
				}
				jingLingPropInF.SpUp.gameObject.SetActive(true);
				this.mPropOBC.AddNoUpdate(jingLingPropInF);
				jingLingPropInF.MDragPanel = this.PropDraggablePanel;
			}
		}
		catch (Exception ex)
		{
		}
		List<string> zhuoYueAttribute = Global.GetZhuoYueAttribute(tempJinglingData);
		for (int j = 0; j < zhuoYueAttribute.Count; j++)
		{
			if (j >= this.giftAttribute.Length)
			{
				break;
			}
			this.giftAttribute[j].text = zhuoYueAttribute[j];
		}
	}

	private bool ChackSkillDataIsAllZore(List<int> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (data[0] != 0)
			{
				return false;
			}
		}
		return true;
	}

	private bool[] CheckJingLingLev(int Forge_level)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
		bool[] array = new bool[4];
		byte b = 0;
		while ((int)b < systemParamStringArrayByName.Length)
		{
			string[] array2 = systemParamStringArrayByName[(int)b].Split(new char[]
			{
				','
			});
			if (Forge_level >= Convert.ToInt32(array2[1]))
			{
				array[(int)b] = true;
			}
			b += 1;
		}
		return array;
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

	private void InitSkillsData(List<int> data, int Forge_level = 0)
	{
		this.InitSkillData();
		if (data == null || this.ChackSkillDataIsAllZore(data))
		{
			bool[] array = this.CheckJingLingLev(Forge_level);
			int num = 2;
			for (int i = 0; i < array.Length; i++)
			{
				this.m_SkillSigns[i].SelectEffectActive = false;
				this.m_SkillSigns[i].IsOpen = array[i];
				this.m_SkillSigns[i].Lev = 0;
				this.m_SkillSigns[i].SkillId = 0;
				this.m_SkillSigns[i].SlotIndex = i;
				int num2 = num + 1;
				int num3 = num2 + 1;
				num = num3 + 1;
			}
		}
		else if (0 < data.Count)
		{
			jinglingSkillSignItem.SignType type = jinglingSkillSignItem.SignType.jingLing;
			if (this.m_PartType == jinglingSystemPart.BtnType.JingLing)
			{
				type = jinglingSkillSignItem.SignType.jingLing;
			}
			else if (this.m_PartType == jinglingSystemPart.BtnType.JingLingSkills)
			{
				type = jinglingSkillSignItem.SignType.SkillUp;
			}
			else if (this.m_PartType == jinglingSystemPart.BtnType.SkillsGrasp)
			{
				type = jinglingSkillSignItem.SignType.SkillGrasp;
			}
			int j = 0;
			int num4 = 1;
			int num5 = 2;
			int num6 = 0;
			while (j < data.Count)
			{
				this.m_SkillSigns[num6].SetSignType(type, false);
				this.m_SkillSigns[num6].SlotIndex = num6;
				this.m_SkillSigns[num6].IsOpen = (1 == data[j]);
				this.m_SkillSigns[num6].SkillId = data[num5];
				this.m_SkillSigns[num6].Lev = data[num4];
				j = num5 + 1;
				num4 = j + 1;
				num5 = num4 + 1;
				num6++;
			}
		}
	}

	private void SelectPet(int index)
	{
		for (int i = 0; i < this.m_isNew.Length; i++)
		{
			this.m_isNew[i] = false;
		}
		this.SelectArr[this.currentSelect].gameObject.SetActive(false);
		this.InitData(Global.Data.equipPet[index]);
		this.currentSelect = index;
		this.SelectArr[index].gameObject.SetActive(true);
		if (!Global.GetRolePatHaveJingling() && this.m_SkillSigns != null)
		{
			for (int j = 0; j < this.m_SkillSigns.Length; j++)
			{
				this.m_SkillSigns[j].ClearData();
			}
		}
		this.JingLingSkillInfPart(this.m_PartType, false);
		if (null != this.m_JingLingSkillInfPart)
		{
			this.m_JingLingSkillInfPart.ClearLock();
		}
	}

	public void SetEquipPet()
	{
		for (int i = 0; i < this.iconList.Count; i++)
		{
			Object.Destroy(this.iconList[i].gameObject);
			Object.Destroy(this.m_LevLabelList[i].gameObject);
		}
		this.iconList.Clear();
		this.m_LevLabelList.Clear();
		for (int j = 0; j < this.plusArr.Length; j++)
		{
			this.plusArr[j].gameObject.SetActive(true);
			this.fightArr[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < this.putBackBag.Length; k++)
		{
			this.putBackBag[k].gameObject.SetActive(false);
		}
		if (Global.Data.equipPet == null)
		{
			return;
		}
		for (int l = 0; l < Global.Data.equipPet.Count; l++)
		{
			if (Global.Data.equipPet[l] != null)
			{
				GGoodIcon ggoodIcon = this.NewPetIcon(Global.Data.equipPet[l]);
				this.iconList.Add(ggoodIcon);
				this.plusArr[l].gameObject.SetActive(false);
				if (Global.Data.equipPet[l].Using != 0)
				{
					this.fightArr[l].gameObject.SetActive(true);
				}
				this.putBackBag[l].gameObject.SetActive(true);
				switch (l)
				{
				case 0:
					ggoodIcon.transform.localPosition = new Vector3(0f, -9f, -0.1f);
					ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.SelectPet(0);
					};
					break;
				case 1:
					ggoodIcon.transform.localPosition = new Vector3(0f, -107f, -0.1f);
					ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.SelectPet(1);
					};
					break;
				case 2:
					ggoodIcon.transform.localPosition = new Vector3(0f, -205f, -0.1f);
					ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.SelectPet(2);
					};
					break;
				case 3:
					ggoodIcon.transform.localPosition = new Vector3(0f, -304f, -0.1f);
					ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.SelectPet(3);
					};
					break;
				}
				this.SelectArr[this.currentSelect].gameObject.SetActive(false);
				if (Global.Data.equipPet.Count < this.currentSelect + 1)
				{
					this.currentSelect = 0;
				}
				GameObject gameObject = new GameObject();
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
				Object.Destroy(gameObject);
				gameObject2.transform.SetParent(this.jingLing, false);
				gameObject2.name = "petNewLabel" + l.ToString();
				UILabel uilabel = gameObject2.AddComponent<UILabel>();
				uilabel.font = ggoodIcon.petLevel.font;
				uilabel.transform.localScale = ggoodIcon.petLevel.transform.localScale;
				uilabel.color = ggoodIcon.petLevel.color;
				uilabel.text = "Lv" + (Global.Data.equipPet[l].Forge_level + 1).ToString();
				uilabel.pivot = ggoodIcon.petLevel.pivot;
				Vector3 localPosition = ggoodIcon.transform.localPosition;
				localPosition.x += ggoodIcon.petLevel.transform.localPosition.x;
				localPosition.y += ggoodIcon.petLevel.transform.localPosition.y;
				localPosition.z = ggoodIcon.transform.localPosition.z - 0.1f;
				uilabel.transform.localPosition = localPosition;
				this.m_LevLabelList.Add(uilabel);
			}
		}
		if (Global.Data.equipPet != null && Global.Data.equipPet.Count != 0)
		{
			this.SelectArr[this.currentSelect].gameObject.SetActive(true);
			this.InitData(Global.Data.equipPet[this.currentSelect]);
		}
		else
		{
			this.ClearData();
			List<int> data = null;
			this.InitSkillsData(data, 0);
		}
	}

	private GGoodIcon NewPetIcon(GoodsData goodsData)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.transform.parent = this.jingLing;
		ggoodIcon.transform.localScale = new Vector3(1f, 1f, 0f);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			this.petName.text = goodsXmlNodeByID.Title;
		}
		ggoodIcon.BindingSprite.gameObject.SetActive(goodsData.Binding > 0);
		if (goodsData.ExcellenceInfo != 0)
		{
			ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			ggoodIcon.TeXiao.gameObject.SetActive(true);
		}
		else if (goodsXmlNodeByID.SuitID == 1)
		{
			ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
		}
		else
		{
			ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
		}
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.GoodsID = goodsData.GoodsID;
		ggoodIcon.GoodsCount = goodsData.GCount;
		ggoodIcon.Binding = goodsData.Binding;
		ggoodIcon.Lucky = goodsData.Lucky;
		ggoodIcon.ForgeLevel = goodsData.Forge_level;
		ggoodIcon.ZhuijiaLevel = goodsData.AppendPropLev;
		ggoodIcon.ExcellenceInfo = goodsData.ExcellenceInfo;
		ggoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		ggoodIcon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodsData.GoodsID)
		});
		ggoodIcon.petLevel.text = string.Empty;
		ggoodIcon.BackgroundSprite1Visible = true;
		return ggoodIcon;
	}

	private void ExecuteForgePet()
	{
		Super.ShowNetWaiting(string.Empty);
		if (Global.Data.equipPet != null && Global.Data.equipPet[this.currentSelect] != null)
		{
			GameInstance.Game.SpriteForgeGoodsNew(Global.Data.equipPet[this.currentSelect].Id, Global.Data.equipPet[this.currentSelect].GoodsID, 0, 1, 0);
		}
	}

	protected override void OnDestroy()
	{
		if (this.realShouHuChongLoader != null)
		{
			this.realShouHuChongLoader.Stop();
			this.realShouHuChongLoader = null;
		}
		ActivityTipManager.UnRegActivityTipItem(16001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		base.OnDestroy();
	}

	public void ErrorHandle(EPetSkillState state)
	{
		string text = string.Empty;
		switch (state + 10)
		{
		case EPetSkillState.Default:
			text = Global.GetLang("没有技能可以领悟");
			break;
		case EPetSkillState.Success:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, Global.GetLang("钻石"));
			break;
		case (EPetSkillState)2:
			text = Global.GetLang("没有槽位可以觉醒");
			break;
		case (EPetSkillState)3:
			text = Global.GetLang("槽位未开放");
			break;
		case (EPetSkillState)4:
			text = Global.GetLang("槽位是最高级");
			break;
		case (EPetSkillState)5:
			text = Global.GetLang("槽位错误");
			break;
		case (EPetSkillState)6:
			text = Global.GetLang("灵晶不足");
			break;
		case (EPetSkillState)7:
			text = Global.GetLang("没有入库");
			break;
		case (EPetSkillState)8:
			text = Global.GetLang("宠物不存在");
			break;
		case (EPetSkillState)9:
			text = Global.GetLang("功能未开放");
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	public GameObject m_JinglingContent;

	public ShowNetImage m_BgTexture;

	public GameObject m_SkillObj;

	public UILabel level;

	public UILabel petName;

	public ListBox PropListBox;

	public UIDraggablePanel PropDraggablePanel;

	public UILabel[] giftAttribute = new UILabel[6];

	public UILabel ownMoHe;

	public UILabel castMoHe;

	public UILabel titleAttr;

	public UILabel titleGiftAttr;

	public UILabel xiaoHao;

	public Transform jingLing;

	public UISprite[] plusArr = new UISprite[4];

	public UISprite[] fightArr = new UISprite[4];

	public UISprite[] SelectArr = new UISprite[4];

	public GButton[] putBackBag = new GButton[4];

	private int currentSelect;

	public GButton chuZhanButton;

	public GButton HuntButton;

	public Modal3DShow model;

	public GButton LevelUP;

	public GButton openBag;

	public GButton yuanSuJueXingBtn;

	public GButton GetLingJing;

	public GameObject effect;

	public Transform effectPosition;

	public GameObject TipIconLieQu;

	private int TempCastMoHe;

	private List<GGoodIcon> iconList = new List<GGoodIcon>();

	private List<UILabel> m_LevLabelList = new List<UILabel>();

	public DPSelectedItemEventHandler DPSelectedItem;

	private XElement XMLLevel;

	private List<XElement> XMLLevelList;

	private XElement XMLPet;

	private List<XElement> XMLPetList;

	public GButton buttonShuXingQiYuan;

	public GButton buttonJinglingQiYuan;

	private jinglingSkillSignItem[] m_SkillSigns;

	private GoodsData TempJinglingData;

	private GameObject m_NeedExp;

	private GameObject m_JingLingInF;

	private jinglingSystemPart.BtnType m_PartType = jinglingSystemPart.BtnType.JingLing;

	private Vector3[] m_SkillsPos_jingling = new Vector3[]
	{
		new Vector3(-110f, 0f, 0f),
		new Vector3(-36.5f, 0f, 0f),
		new Vector3(36.5f, 0f, 0f),
		new Vector3(110f, 0f, 0f)
	};

	private Vector3[] m_SkillsPos_Skills = new Vector3[]
	{
		new Vector3(-85.3f, 2.7f, -1f),
		new Vector3(154.7f, 4.2f, -1f),
		new Vector3(-81.3f, -243.1f, -1f),
		new Vector3(157.5f, -239.9f, -1f)
	};

	private JingLingSkillsInFPart m_JingLingSkillInfPart;

	private bool[] m_isNew = new bool[4];

	private int SkillSelectIndex;

	private bool m_JingLingSkillsIsOpen;

	private Dictionary<int, int> m_DicSlotOpenLev = new Dictionary<int, int>();

	private ObservableCollection mPropOBC;

	private ShouHuChongResLoader realShouHuChongLoader;

	public class SkillData
	{
		public SkillData(bool isOpen, int lev, int id, bool isNew = false)
		{
			this.IsOpen = isOpen;
			this.Lev = lev;
			this.Id = id;
		}

		public bool IsOpen;

		public int Lev;

		public int Id;

		public bool IsNew;
	}
}
