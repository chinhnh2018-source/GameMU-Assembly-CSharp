using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShenHunPartFuTi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblInfoWord.text = Global.GetLang("基础信息");
		this.btnShengJi.Text = Global.GetLang("升级");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_selectType = ShenHunFuTiType.TianShen;
		Global.SetButtonSprite(this.btnChange, "btnshenbing");
		this.InitData();
		this.skillDetailWindow.gameObject.SetActive(false);
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			ShenHunPartFuTi.OpenHelpWindow();
		};
		this.btnLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(this.m_selectModelLevel);
			MUTransfigurationLevel formerNewModelLevel = this.GetFormerNewModelLevel(selfLevelInfo);
			if (formerNewModelLevel != null)
			{
				this.m_selectModelLevel = formerNewModelLevel.Level;
				this.Load3DModelByLevel(this.m_selectModelLevel, this.m_selectType);
			}
		};
		this.btnYou.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(this.m_selectModelLevel);
			MUTransfigurationLevel nextNewModelLevel = this.GetNextNewModelLevel(selfLevelInfo);
			if (nextNewModelLevel != null)
			{
				this.m_selectModelLevel = nextNewModelLevel.Level;
				this.Load3DModelByLevel(this.m_selectModelLevel, this.m_selectType);
			}
		};
		this.btnShengJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BeMaterialsEnough())
			{
				this.SendBianShenLevelUp();
			}
			else
			{
				Super.HintMainText(Global.GetLang("所需材料不足"), 10, 3);
			}
		};
		this.btnChange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_selectType == ShenHunFuTiType.TianShen)
			{
				this.SetSelectState(ShenHunFuTiType.ShenBing);
			}
			else
			{
				this.SetSelectState(ShenHunFuTiType.TianShen);
			}
		};
		this.bianShenModel.CanRotate = true;
		this.weaponModel.CanRotate = false;
	}

	private void InitData()
	{
		int bianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
		int num = bianShen + 1;
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(bianShen);
		if (selfLevelInfo == null)
		{
			Debug.LogError(Global.GetLang("读取神魂附体等级数据错误"));
			return;
		}
		this.sliderExp.sliderValue = (float)ShenHunData.GetSelfShenHunLevel().Exp * 1f / (float)selfLevelInfo.UpExp;
		this.lblLevelNow.text = "Lv" + bianShen;
		MUTransfigurationLevel selfLevelInfo2 = this.GetSelfLevelInfo(num);
		bool flag = selfLevelInfo2 == null;
		if (flag)
		{
			this.goNextArrow.SetActive(false);
			this.lblLevelNext.text = string.Empty;
		}
		else
		{
			this.goNextArrow.SetActive(true);
			this.lblLevelNext.text = "Lv" + num;
		}
		this.LoadSkill(selfLevelInfo);
		this.lstInfoNowWord[0].text = Global.GetLang("持续时间");
		this.lstInfoNowValue[0].text = selfLevelInfo.Duration + Global.GetLang("秒");
		if (flag)
		{
			this.lstInfoAdd[0].gameObject.SetActive(false);
			this.lstInfoAddSprite[0].gameObject.SetActive(false);
		}
		else
		{
			int num2 = selfLevelInfo2.Duration - selfLevelInfo.Duration;
			if (num2 <= 0)
			{
				this.lstInfoAdd[0].gameObject.SetActive(false);
				this.lstInfoAddSprite[0].gameObject.SetActive(false);
			}
			else
			{
				this.lstInfoAdd[0].text = num2 + Global.GetLang("秒");
			}
		}
		List<MUPropInfo> proPerty = selfLevelInfo.ProPerty;
		for (int i = 0; i < this.lstInfoNowWord.Count - 1; i++)
		{
			if (i < proPerty.Count)
			{
				this.lstInfoNowWord[i + 1].gameObject.SetActive(true);
				this.lstInfoNowValue[i + 1].gameObject.SetActive(true);
				this.lstInfoAdd[i + 1].gameObject.SetActive(true);
				this.lstInfoAddSprite[i + 1].gameObject.SetActive(true);
				string chinesePropName = proPerty[i].ChinesePropName;
				this.lstInfoNowWord[i + 1].text = chinesePropName;
				if (proPerty[i].BePercent)
				{
					this.lstInfoNowValue[i + 1].text = Mathf.RoundToInt(proPerty[i].PropNum * 100f) + "%";
				}
				else
				{
					this.lstInfoNowValue[i + 1].text = Mathf.RoundToInt(proPerty[i].PropNum).ToString();
				}
				if (flag)
				{
					this.lstInfoAdd[i + 1].gameObject.SetActive(false);
					this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
				}
				else
				{
					List<MUPropInfo> proPerty2 = selfLevelInfo2.ProPerty;
					if (proPerty[i].BePercent)
					{
						int num3 = Mathf.RoundToInt((proPerty2[i].PropNum - proPerty[i].PropNum) * 100f);
						if (num3 <= 0)
						{
							this.lstInfoAdd[i + 1].gameObject.SetActive(false);
							this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
						}
						else
						{
							this.lstInfoAdd[i + 1].text = num3 + "%";
						}
					}
					else
					{
						int num4 = Mathf.RoundToInt(proPerty2[i].PropNum - proPerty[i].PropNum);
						if (num4 <= 0)
						{
							this.lstInfoAdd[i + 1].gameObject.SetActive(false);
							this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
						}
						else
						{
							this.lstInfoAdd[i + 1].text = num4.ToString();
						}
					}
				}
			}
			else
			{
				this.lstInfoNowWord[i + 1].gameObject.SetActive(false);
				this.lstInfoAdd[i + 1].gameObject.SetActive(false);
				this.lstInfoNowValue[i + 1].gameObject.SetActive(false);
				this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
			}
		}
		this.LoadRewards(selfLevelInfo.NeedGoods);
		this.Load3DModelByLevel(selfLevelInfo.Level, this.m_selectType);
		if (flag)
		{
			this.btnShengJi.gameObject.SetActive(false);
		}
		else
		{
			this.btnShengJi.gameObject.SetActive(true);
		}
	}

	private MUTransfigurationLevel GetSelfLevelInfo(int level)
	{
		return ShenHunData.GetTransfigurationLevelByOccupationLevel(Global.Data.roleData.Occupation, level);
	}

	private MUTransfigurationLevel GetNextNewModelLevel(MUTransfigurationLevel level)
	{
		int bianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(bianShen);
		List<MUTransfigurationLevel> allTransfigurationLevelByOccupation = IConfigbase<ConfigShenHun>.Instance.GetAllTransfigurationLevelByOccupation(Global.Data.roleData.Occupation);
		int num = allTransfigurationLevelByOccupation.IndexOf(level);
		if (num < 0)
		{
			return null;
		}
		int i = num;
		while (i < allTransfigurationLevelByOccupation.Count)
		{
			MUTransfigurationLevel mutransfigurationLevel = allTransfigurationLevelByOccupation[i];
			if (level.GodMod != mutransfigurationLevel.GodMod || level.WeaponMOd != mutransfigurationLevel.WeaponMOd)
			{
				if (selfLevelInfo != null && mutransfigurationLevel.GodMod == selfLevelInfo.GodMod && mutransfigurationLevel.WeaponMOd == selfLevelInfo.WeaponMOd)
				{
					return selfLevelInfo;
				}
				return mutransfigurationLevel;
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	private MUTransfigurationLevel GetFormerNewModelLevel(MUTransfigurationLevel level)
	{
		int bianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(bianShen);
		List<MUTransfigurationLevel> allTransfigurationLevelByOccupation = IConfigbase<ConfigShenHun>.Instance.GetAllTransfigurationLevelByOccupation(Global.Data.roleData.Occupation);
		int num = allTransfigurationLevelByOccupation.IndexOf(level);
		if (num < 0)
		{
			return null;
		}
		MUTransfigurationLevel mutransfigurationLevel = null;
		for (int i = num; i > -1; i--)
		{
			MUTransfigurationLevel mutransfigurationLevel2 = allTransfigurationLevelByOccupation[i];
			if (level.GodMod != mutransfigurationLevel2.GodMod || level.WeaponMOd != mutransfigurationLevel2.WeaponMOd)
			{
				if (mutransfigurationLevel == null)
				{
					mutransfigurationLevel = mutransfigurationLevel2;
					if (selfLevelInfo != null && mutransfigurationLevel.GodMod == selfLevelInfo.GodMod && mutransfigurationLevel.WeaponMOd == selfLevelInfo.WeaponMOd)
					{
						return selfLevelInfo;
					}
				}
				else
				{
					if (mutransfigurationLevel.GodMod != mutransfigurationLevel2.GodMod || mutransfigurationLevel.WeaponMOd != mutransfigurationLevel2.WeaponMOd)
					{
						return mutransfigurationLevel;
					}
					mutransfigurationLevel = mutransfigurationLevel2;
				}
			}
		}
		return mutransfigurationLevel;
	}

	private void SetSelectState(ShenHunFuTiType type)
	{
		if (this.m_selectType != type)
		{
			this.m_formerModelId = 0;
			this.m_formerWeaponId = 0;
			this.m_selectType = type;
			this.m_selectModelLevel = ShenHunData.GetSelfShenHunLevel().BianShen;
			if (this.m_selectType == ShenHunFuTiType.ShenBing)
			{
				this.bgImg.URL = "NetImages/GameRes/Images/ShenHun/wuqi_bg.png.qj";
				Global.SetButtonSprite(this.btnChange, "btntianshen");
			}
			else if (this.m_selectType == ShenHunFuTiType.TianShen)
			{
				this.bgImg.URL = "NetImages/GameRes/Images/ShenHun/shenhun_bg.png.qj";
				Global.SetButtonSprite(this.btnChange, "btnshenbing");
			}
			this.Load3DModelByLevel(this.m_selectModelLevel, this.m_selectType);
		}
	}

	private void SetLeftRightBtnstate()
	{
		this.btnYou.gameObject.SetActive(true);
		this.btnLeft.gameObject.SetActive(true);
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(this.m_selectModelLevel);
		MUTransfigurationLevel nextNewModelLevel = this.GetNextNewModelLevel(selfLevelInfo);
		if (this.GetFormerNewModelLevel(selfLevelInfo) == null)
		{
			this.btnLeft.gameObject.SetActive(false);
		}
		if (this.m_selectModelLevel > ShenHunData.GetSelfShenHunLevel().BianShen)
		{
			this.btnYou.gameObject.SetActive(false);
		}
		else if (nextNewModelLevel == null)
		{
			this.btnYou.gameObject.SetActive(false);
		}
	}

	private void LoadSkill(MUTransfigurationLevel nowLevelInfo)
	{
		List<int> allSkills = ShenHunData.GetAllSkills(Global.Data.roleData.Occupation);
		for (int i = 0; i < allSkills.Count; i++)
		{
			ShenHunPartSkill shenHunPartSkill = this.lstSkills[i];
			shenHunPartSkill.transform.SetParent(this.gridSkill.transform);
			shenHunPartSkill.OnClickSkill = new Action<ShenHunPartSkill>(this.OnClickSkill);
			int jieSuoSkillLevel = ShenHunData.GetJieSuoSkillLevel(Global.Data.roleData.Occupation, i);
			shenHunPartSkill.InitSkill(allSkills[i], nowLevelInfo.Level, jieSuoSkillLevel, -1);
			shenHunPartSkill.transform.localScale = Vector3.one;
			shenHunPartSkill.transform.localPosition = Vector3.zero;
		}
		this.gridSkill.Reposition();
	}

	private void OnClickSkill(ShenHunPartSkill skill)
	{
		this.skillDetailWindow.gameObject.SetActive(true);
		this.skillDetailWindow.InitSkill(skill.SkillInfo, skill.SkillLevel);
	}

	private void LoadRewards(List<MUMaterialInfo> materials)
	{
		for (int i = 0; i < this.gridIcon.transform.childCount; i++)
		{
			Object.Destroy(this.gridIcon.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < materials.Count; j++)
		{
			if (materials[j].MaterialId > 0)
			{
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(materials[j].MaterialId, true);
				ggoodIcon.transform.SetParent(this.gridIcon.transform);
				ggoodIcon.transform.localScale = new Vector3(1f, 1f, 1f);
				ggoodIcon.transform.localPosition = new Vector3(this.gridIcon.cellWidth * (float)j, 0f, 0f);
				int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(materials[j].MaterialId);
				ggoodIcon.SecondText.Label.supportEncoding = true;
				if (roleGoodsNumberCountByGoodsID >= materials[j].Num)
				{
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, materials[j].Num)
					});
				}
				else
				{
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, materials[j].Num)
					});
				}
			}
		}
		float num = (float)(1 - materials.Count) * this.gridIcon.cellWidth / 2f;
		this.gridIcon.transform.localPosition = new Vector3(num, 0f, 0f);
	}

	private bool BeMaterialsEnough()
	{
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(ShenHunData.GetSelfShenHunLevel().BianShen);
		List<MUMaterialInfo> needGoods = selfLevelInfo.NeedGoods;
		for (int i = 0; i < needGoods.Count; i++)
		{
			int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(needGoods[i].MaterialId);
			if (roleGoodsNumberCountByGoodsID < needGoods[i].Num)
			{
				return false;
			}
		}
		return true;
	}

	private void Load3DModelByLevel(int level, ShenHunFuTiType type)
	{
		MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(level);
		if (selfLevelInfo == null)
		{
			Debug.LogError(Global.GetLang("获取等级信息错误 Level :") + level);
			return;
		}
		this.m_selectModelLevel = level;
		if (type == ShenHunFuTiType.TianShen)
		{
			this.Load3DModel(selfLevelInfo.GodMod, selfLevelInfo.WeaponMOd);
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(selfLevelInfo.GodMod);
			this.lblName.text = monsterXmlNodeByID.SName + " Lv" + level;
		}
		else if (type == ShenHunFuTiType.ShenBing)
		{
			this.LoadWeaponData(selfLevelInfo.WeaponMOd);
			string goodsNameByID = Global.GetGoodsNameByID(selfLevelInfo.WeaponMOd, false);
			this.lblName.text = goodsNameByID + " Lv" + level;
		}
		this.SetLeftRightBtnstate();
	}

	private void Load3DModel(int modelId, int weaponId)
	{
		if (this.m_formerModelId == modelId && this.m_formerWeaponId == weaponId && this.bianShenModel.transform.childCount > 0)
		{
			this.bianShenModel.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			return;
		}
		this.m_formerModelId = modelId;
		this.m_formerWeaponId = weaponId;
		this.bianShenModel.transform.parent.gameObject.SetActive(true);
		this.weaponModel.transform.parent.gameObject.SetActive(false);
		this.bianShenModel.Clear();
		if (this.resLoader != null)
		{
			this.resLoader.Stop();
		}
		this.resLoader = UIHelper.LoadBianShenRes(this.bianShenModel, modelId, weaponId);
	}

	private void LoadWeaponData(int modelId)
	{
		if (this.m_formerWeaponId == modelId)
		{
			return;
		}
		this.m_formerWeaponId = modelId;
		this.bianShenModel.transform.parent.gameObject.SetActive(false);
		this.weaponModel.transform.parent.gameObject.SetActive(true);
		this.weaponModel.Clear();
		UIHelper.LoadGoodsRes(this.weaponModel, modelId, 1f, 0.005f, 0, "Equip", true);
		this.weaponModel.CanRotate = false;
	}

	public static void OpenHelpWindow()
	{
		ChangeableRulePart.RuleXml shenHunHelpData = IConfigbase<ConfigShenHun>.Instance.GetShenHunHelpData();
		if (shenHunHelpData == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到相关配置"
			});
			return;
		}
		if (ShenHunPartFuTi.m_helpWindow == null)
		{
			ShenHunPartFuTi.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			ShenHunPartFuTi.m_helpWindow.IsShowModal = true;
			ShenHunPartFuTi.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShenHunPartFuTi.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(ShenHunPartFuTi.m_helpWindow);
		}
		if (ShenHunPartFuTi.m_helpPart == null)
		{
			ShenHunPartFuTi.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			ShenHunPartFuTi.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShenHunPartFuTi.CloseHelpWindow();
			};
		}
		ShenHunPartFuTi.m_helpWindow.SetContent(ShenHunPartFuTi.m_helpWindow.BodyPresenter, ShenHunPartFuTi.m_helpPart, 0.0, 0.0, true);
		ShenHunPartFuTi.m_helpPart.SetHelpInfo(shenHunHelpData.list);
	}

	private static void CloseHelpWindow()
	{
		if (null != ShenHunPartFuTi.m_helpPart)
		{
			ShenHunPartFuTi.m_helpPart.transform.parent = null;
			Object.Destroy(ShenHunPartFuTi.m_helpPart.gameObject);
			ShenHunPartFuTi.m_helpPart = null;
		}
		if (null != ShenHunPartFuTi.m_helpWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShenHunPartFuTi.m_helpWindow);
			ShenHunPartFuTi.m_helpWindow = null;
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
		if (this.m_selectType == ShenHunFuTiType.TianShen && this.m_selectModelLevel > 0)
		{
			MUTransfigurationLevel selfLevelInfo = this.GetSelfLevelInfo(this.m_selectModelLevel);
			if (selfLevelInfo == null)
			{
				Debug.LogError(Global.GetLang("获取等级信息错误 Level :") + this.m_selectModelLevel);
				return;
			}
			this.m_formerModelId = 0;
			this.m_formerWeaponId = 0;
			this.Load3DModel(selfLevelInfo.GodMod, selfLevelInfo.WeaponMOd);
		}
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<BianShenUpdateResultData>("CMD_DB_BIANSHEN_LEVEL_UP", new Action<BianShenUpdateResultData>(this.ServerBianShenLevelUpReslut));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<BianShenUpdateResultData>("CMD_DB_BIANSHEN_LEVEL_UP", new Action<BianShenUpdateResultData>(this.ServerBianShenLevelUpReslut));
	}

	private void SendBianShenLevelUp()
	{
		Super.ShowNetWaiting(null);
		BianShenUpdateResultData bianShenUpdateResultData = new BianShenUpdateResultData();
		bianShenUpdateResultData.Type = 0;
		bianShenUpdateResultData.Result = 0;
		bianShenUpdateResultData.BianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
		bianShenUpdateResultData.Exp = ShenHunData.GetSelfShenHunLevel().Exp;
		bianShenUpdateResultData.Auto = 0;
		bianShenUpdateResultData.ZuanShi = 0;
		this.m_lastBianShenData = bianShenUpdateResultData;
		GameInstance.Game.SendBianShenUpLevel(bianShenUpdateResultData);
	}

	private void ServerBianShenLevelUpReslut(BianShenUpdateResultData resultData)
	{
		this.InitData();
		int num = resultData.BianShen - this.m_lastBianShenData.BianShen;
		int num2 = resultData.Exp - this.m_lastBianShenData.Exp;
		if (num > 0)
		{
			Super.HintMainText(Global.GetLang("等级提升"), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang("经验+") + num2, 10, 3);
		}
	}

	public UILabel lblLevelNow;

	public UILabel lblLevelNext;

	public GameObject goNextArrow;

	public UILabel lblInfoWord;

	public UILabel lblName;

	public GButton btnHelp;

	public GButton btnLeft;

	public GButton btnYou;

	public GButton btnShengJi;

	public GButton btnChange;

	public UISlider sliderExp;

	public ShowNetImage bgImg;

	public List<UILabel> lstInfoNowWord;

	public List<UILabel> lstInfoNowValue;

	public List<UILabel> lstInfoAdd;

	public List<UISprite> lstInfoAddSprite;

	public UIGrid gridIcon;

	public UIGrid gridSkill;

	public List<ShenHunPartSkill> lstSkills;

	public Modal3DShow bianShenModel;

	public Modal3DShow weaponModel;

	[HideInInspector]
	public ShenHunPartSkillDetail skillDetailWindow;

	private ShenHunFuTiType m_selectType;

	private int m_selectModelLevel;

	private MonsterNPCResLoader resLoader;

	private int m_formerModelId;

	private int m_formerWeaponId;

	protected static GChildWindow m_helpWindow;

	protected static CommonHelpWindow m_helpPart;

	private BianShenUpdateResultData m_lastBianShenData;
}
