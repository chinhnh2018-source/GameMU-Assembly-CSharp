using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class SkillPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.upSkillLevBtn.Text = Global.GetLang("升级技能");
		this.upSkillSLDBtn.Text = Global.GetLang("提升熟练度");
		this.ConstText_NowXiaoGuo.Text = Global.GetLang("本级效果");
		this.ConstText_NextXiaoGuo.Text = Global.GetLang("下级效果");
		this.shifangfanweiText.Text = string.Empty;
		this.xiaohaoText.Text = string.Empty;
		this.cdText.Text = string.Empty;
		this.cdText.X = 90.0;
		this.cdText.Y = -200.0;
		this.shifangfanweiText.Y = -230.0;
		this.nowXiaoGuoText.Y = -32.0;
		this.nextXiaoGuoText.Y = -50.0;
		this.ConstText_NextXiaoGuo.Y = -30.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.upSkillLevBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.temp.skillLeve == 0)
			{
				Super.HintMainText(Global.GetLang("此技能尚未激活！"), 10, 3);
			}
			else
			{
				GameInstance.Game.SpriteUpSkillLevel(this.temp.skillDBid, 3);
				this.Anim[0].SetActive(false);
			}
		};
		this.upSkillSLDBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.temp.skillLeve == 0)
			{
				Super.HintMainText(Global.GetLang("此技能尚未激活！"), 10, 3);
			}
			else
			{
				GameInstance.Game.SpriteUpSkillLevel(this.temp.skillDBid, 1);
			}
		};
		this.upSkillSLDBtn.gameObject.SetActive(false);
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void AutoUpSkillLevel(int skillID)
	{
	}

	public void RefreshSkillItems()
	{
		this.ItemCollection.Clear();
		List<MagicInfoVO> skillListByOccupation = this.GetSkillListByOccupation();
		if (skillListByOccupation == null || skillListByOccupation.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < skillListByOccupation.Count; i++)
		{
			MagicInfoVO magicInfoVO = skillListByOccupation[i];
			int id = magicInfoVO.ID;
			int magicIcon = magicInfoVO.MagicIcon;
			if (magicIcon < 0)
			{
			}
			int actionIndex = magicInfoVO.ActionIndex;
			if (actionIndex < 1000)
			{
				int parentMagicID = magicInfoVO.ParentMagicID;
				if (parentMagicID <= 0)
				{
					SkillData skillDataByID = Global.GetSkillDataByID(id);
					SkillItem skillItem = U3DUtils.NEW<SkillItem>();
					this.ItemCollection.Add(skillItem);
					skillItem.InitSkill(magicInfoVO, id, skillDataByID);
					this.SkillIconDict[id] = skillItem;
					if (skillDataByID == null)
					{
						skillItem.skillUsedNum = 0;
						skillItem.skillLeve = 0;
						skillItem.skillDBid = -1;
						skillItem.icon.ToGrayBitmap = true;
					}
					else
					{
						skillItem.skillUsedNum = skillDataByID.UsedNum;
						skillItem.skillLeve = skillDataByID.SkillLevel;
						skillItem.skillDBid = skillDataByID.DbID;
					}
					UIPanel component = skillItem.gameObject.GetComponent<UIPanel>();
					if (null != component)
					{
						Object.Destroy(component);
					}
					skillItem.UpLevStat.gameObject.SetActive(this.GetUpLeveConditions(skillItem.ID, skillItem.skillLeve, skillItem.skillUsedNum, false));
					if (!skillItem.icon.ToGrayBitmap)
					{
						skillItem.transform.GetChild(1).GetComponent<UITexture>().material.shader = Shader.Find("Unlit/Transparent Colored");
					}
				}
			}
		}
		if (this.ItemCollection.Count < this.skillMaxShowCount)
		{
			for (int j = 0; j < this.skillMaxShowCount - this.ItemCollection.Count; j++)
			{
				SkillItem skillItem2 = U3DUtils.NEW<SkillItem>();
				this.ItemCollection.Add(skillItem2);
				skillItem2.skillDBid = -1;
				skillItem2.icon.URL = "NetImages/GameRes/Images/Hybrid/Lock_bak.png";
				skillItem2.ItemName.textColor = 16711680U;
				UIPanel component2 = skillItem2.gameObject.GetComponent<UIPanel>();
				if (null != component2)
				{
					Object.Destroy(component2);
				}
				skillItem2.transform.GetChild(1).GetComponent<UITexture>().material.shader = Shader.Find("Unlit/Transparent Colored");
			}
			this.uIDraggablePanel.GetComponent<UIDraggablePanel>().enabled = false;
			this.Arrows.SetActive(false);
		}
		else if (this.ItemCollection.Count > this.skillMaxShowCount)
		{
			this.Arrows.SetActive(true);
		}
		this.ItemCollection.DelayUpdate();
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteGetSkillInfoCmd();
	}

	public void RefreshDate()
	{
		this.RefreshSkillItems();
		int selectedIndex = 0;
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			SkillItem skillItem = U3DUtils.AS<SkillItem>(this.ItemCollection[i]);
			if (skillItem.UpLevStat.gameObject.activeSelf)
			{
				selectedIndex = i;
				break;
			}
		}
		this.listBox.SelectedIndex = selectedIndex;
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this);
	}

	public void GetNewData(bool onlyOnce = false)
	{
	}

	private List<MagicInfoVO> GetSkillListByOccupation()
	{
		if (!ConfigMagicInfos.IsValid())
		{
			return null;
		}
		List<MagicInfoVO> list = new List<MagicInfoVO>();
		foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
		{
			MagicInfoVO value = keyValuePair.Value;
			if (value.ToOcuupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void NotifyAddSkillData(int skillDbID, int skillID, int skillLevel)
	{
		this.RefreshSkillItems();
	}

	public void NotifyUpSkillLevel(int retCode, int skillDbID, int skillLevel, int usedNum)
	{
		SkillItem skillItem = null;
		for (int i = 0; i < this.listBox.Count(); i++)
		{
			skillItem = U3DUtils.AS<SkillItem>(this.ItemCollection[i]);
			if (skillItem.skillDBid == skillDbID)
			{
				bool flag = skillLevel > skillItem.skillLeve;
				if (flag)
				{
					this.Anim[0].transform.position = skillItem.transform.position;
					this.Anim[0].SetActive(true);
				}
				skillItem.skillUsedNum = usedNum;
				skillItem.skillLeve = skillLevel;
				skillItem.SetSkillLevel(skillLevel);
				break;
			}
		}
		this.NotifySkillUsedNum(-1, skillItem.ID, usedNum, skillLevel);
		this.SetSkillinfo(skillItem.ID, skillLevel, usedNum);
		this.temp.UpLevStat.gameObject.SetActive(false);
		this.SetUpLevelState(skillItem.ID, skillLevel, usedNum);
		this.SetUpLevelButtnState(skillItem.ID, skillLevel);
	}

	public void NotifyChgNumSkillID(int role, int skillID)
	{
	}

	public void NotifySkillUsedNum(int role, int skillID, int usedNum, int skillLevel)
	{
		SkillData skillDataByID = Global.GetSkillDataByID(skillID);
		if (skillDataByID == null)
		{
			return;
		}
		skillDataByID.UsedNum = usedNum;
		skillDataByID.SkillLevel = skillLevel;
		SkillItem skillItem = null;
		if (this.SkillIconDict.ContainsKey(skillDataByID.SkillID))
		{
			this.SkillIconDict.TryGetValue(skillDataByID.SkillID, ref skillItem);
			if (null != skillItem)
			{
				skillItem.SetSkillLevel(skillLevel);
			}
		}
	}

	public void NotifyDefaultSkillUpLevel()
	{
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		SkillItem skillItem = U3DUtils.AS<SkillItem>(this.listBox.SelectedItem);
		if (null == skillItem)
		{
			return;
		}
		if (this.temp != null && this.temp != skillItem)
		{
			this.temp.selectStat = "jinengItem";
		}
		this.temp = skillItem;
		skillItem.selectStat = "jinengItemSelect";
		this.m_bIsBtnIsEnable = true;
		this.SetSkillinfo(skillItem.ID, skillItem.skillLeve, skillItem.skillUsedNum);
		this.SetUpLevelState(skillItem.ID, skillItem.skillLeve, skillItem.skillUsedNum);
		this.SetUpLevelButtnState(skillItem.ID, skillItem.skillLeve);
	}

	public bool m_bIsBtnIsEnable
	{
		get
		{
			return false;
		}
		set
		{
			this.upSkillLevBtn.isEnabled = value;
		}
	}

	private void SetUpLevelState(int nID, int nLevel, int nSkillNum)
	{
	}

	private void SetUpLevelButtnState(int nID, int nLevel)
	{
		if (this.IsUpLevelEnable(nID, nLevel))
		{
			this.upSkillLevBtn.isEnabled = true;
		}
		else
		{
			this.upSkillLevBtn.isEnabled = false;
		}
	}

	private bool IsUpLevelEnable(int skillid, int skilllev)
	{
		MagicItemVO magicItemVO = ConfigMagics.GetMagicItemVO(Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation), skillid, skilllev);
		return magicItemVO != null && skilllev < 100 && Global.Data.roleData.ChangeLifeCount >= magicItemVO.NeedZhuanSheng && (Global.Data.roleData.ChangeLifeCount != magicItemVO.NeedZhuanSheng || Global.Data.roleData.Level >= magicItemVO.NeedRoleLevel);
	}

	private void SetSkillinfo(int skillID, int skllleve, int skillUsedNum)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return;
		}
		if (maigcInfoVOByCode.ID == skillID && maigcInfoVOByCode.NextMagicID != -1)
		{
			MagicInfoVO maigcInfoVOByCode2 = ConfigMagicInfos.GetMaigcInfoVOByCode(maigcInfoVOByCode.NextMagicID);
			this.SetXiaoGuoInfo(maigcInfoVOByCode, maigcInfoVOByCode2, skillID, skllleve + Global.getSkillAddPoin(skillID));
		}
		else
		{
			this.SetXiaoGuoInfo(maigcInfoVOByCode, skillID, skllleve + Global.getSkillAddPoin(skillID));
		}
		this.skiilNameText.Text = maigcInfoVOByCode.Name;
		this.shifangfanweiText.Text = string.Format(Global.GetLang("{{c39550}}释放范围:{{-}}{{00ff00}}{0}{{-}}"), maigcInfoVOByCode.FanWeiDescription);
		this.xiaohaoText.Text = string.Format(Global.GetLang("{{c39550}}魔法消耗:{{-}}{{00ff00}}{0}%魔法上限{{-}}"), maigcInfoVOByCode.BaseMagic);
		this.cdText.Text = string.Format(Global.GetLang("{{c39550}}技能冷却:{{-}}{{00ff00}}{0}{{-}}秒"), maigcInfoVOByCode.CDTime);
		this.SetUpLeveConditions(skillID, skllleve, skillUsedNum);
	}

	private bool GetUpLeveConditions(int skillid, int skilllev, int skillUsedNum, bool isBtn = false)
	{
		double num = Convert.ToDouble(skillUsedNum);
		MagicItemVO magicItemVO = ConfigMagics.GetMagicItemVO(Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation), skillid, skilllev);
		return magicItemVO != null && skilllev < 100 && num >= (double)magicItemVO.ShuLianDu && magicItemVO.NeedJinBi <= Global.GetRoleOwnNumByMoneyType(8) + Global.GetRoleOwnNumByMoneyType(1) && Global.Data.roleData.ChangeLifeCount >= magicItemVO.NeedZhuanSheng && (Global.Data.roleData.ChangeLifeCount != magicItemVO.NeedZhuanSheng || Global.Data.roleData.Level >= magicItemVO.NeedRoleLevel);
	}

	private void SetUpLeveConditions(int skillid, int skilllev, int skillUsedNum)
	{
		if (skilllev <= 0)
		{
			skilllev = 1;
		}
		double num = Convert.ToDouble(skillUsedNum);
		MagicItemVO magicItemVO = ConfigMagics.GetMagicItemVO(Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation), skillid, skilllev);
		if (magicItemVO == null)
		{
			this.LevProgBar.Percent = 0.0;
			this.needLeveText.Text = string.Format(Global.GetLang("{0}级"), 0);
			this.needJifenText.Text = "0";
			this.needJinbiText.Text = "0";
			this.progBarText.Text = "0%";
			return;
		}
		int num2 = (int)(((double)magicItemVO.ShuLianDu - num) / 5.0);
		if (num2 <= 0)
		{
			num2 = 0;
		}
		else if (num2 < 1)
		{
			num2 = 1;
		}
		this.needJifenText.Text = Convert.ToString(num2);
		this.needJifenText.textColor = 14599836U;
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < num2)
		{
			this.needJifenText.textColor = 16711680U;
			this.upSkillSLDBtn.isEnabled = false;
		}
		else if (skillUsedNum >= magicItemVO.ShuLianDu)
		{
			this.upSkillSLDBtn.isEnabled = false;
		}
		else
		{
			this.upSkillSLDBtn.isEnabled = true;
		}
		this.needJinbiText.Text = magicItemVO.NeedJinBi.ToString();
		this.needJinbiText.textColor = 14599836U;
		if (magicItemVO.NeedJinBi > Global.GetRoleOwnNumByMoneyType(8) + Global.GetRoleOwnNumByMoneyType(1))
		{
			this.needJinbiText.textColor = 16711680U;
		}
		this.needLeveText.textColor = 14599836U;
		if (magicItemVO.NeedZhuanSheng != 0)
		{
			this.needLeveText.Text = string.Format(Global.GetLang("{0}转{1}级"), magicItemVO.NeedZhuanSheng, magicItemVO.NeedRoleLevel);
		}
		else
		{
			this.needLeveText.Text = string.Format(Global.GetLang("{0}级"), magicItemVO.NeedRoleLevel);
		}
		if (Global.Data.roleData.ChangeLifeCount < magicItemVO.NeedZhuanSheng)
		{
			this.needLeveText.textColor = 16711680U;
		}
		else if (Global.Data.roleData.ChangeLifeCount == magicItemVO.NeedZhuanSheng && Global.Data.roleData.Level < magicItemVO.NeedRoleLevel)
		{
			this.needLeveText.textColor = 16711680U;
		}
		this.skillValue = skilllev;
		double num3 = (double)this.skillValue;
		this.LevProgBar.Percent = num3 / 100.0;
		this.progBarText.Label.text = string.Format("{0}%", this.skillValue);
		MUDebug.Log<string>(new string[]
		{
			"iUsedNum=" + num
		});
		MUDebug.Log<string>(new string[]
		{
			"skillValue=" + this.skillValue
		});
		MUDebug.Log<string>(new string[]
		{
			"LevProgBar.Percent=" + this.LevProgBar.Percent
		});
		Global.ShowSkillRedPoint();
	}

	private void SetXiaoGuoInfo(MagicInfoVO xmlElement, int skillid, int skillLev)
	{
		string description = xmlElement.Description;
		string magicScripts = xmlElement.MagicScripts;
		string[] array = magicScripts.Split(new char[]
		{
			'|'
		});
		string[] magicScriptsValue = this.GetMagicScriptsValue(array[0]);
		this.nowXiaoGuoText.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), skillLev), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), skillLev));
		this.nextXiaoGuoText.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), skillLev + 1), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), skillLev + 1));
	}

	private void SetXiaoGuoInfo(MagicInfoVO xmlElement, MagicInfoVO nextxmlElement, int skillid, int skillLev)
	{
		string description = xmlElement.Description;
		string magicScripts = xmlElement.MagicScripts;
		string magicScripts2 = nextxmlElement.MagicScripts;
		string[] array = magicScripts.Split(new char[]
		{
			'|'
		});
		string[] magicScriptsValue = this.GetMagicScriptsValue(array[0]);
		string[] array2 = magicScripts2.Split(new char[]
		{
			'|'
		});
		string[] magicScriptsValue2 = this.GetMagicScriptsValue(array2[0]);
		this.nowXiaoGuoText.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), skillLev, Convert.ToDouble(magicScriptsValue2[0])), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), skillLev, Convert.ToInt32(magicScriptsValue2[1])));
		this.nextXiaoGuoText.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), skillLev + 1, Convert.ToDouble(magicScriptsValue2[0])), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), skillLev + 1, Convert.ToInt32(magicScriptsValue2[1])));
	}

	private string[] GetMagicScriptsValue(string str)
	{
		if (str == string.Empty)
		{
			return null;
		}
		if (str.Contains("MU_FIRE_WALL_Y") || str.Contains("ZHONGDU"))
		{
			int num = str.IndexOf('(') + 1;
			int num2 = str.IndexOf(')') - num;
			string text = str.Substring(num, num2);
			string[] array = text.Split(new char[]
			{
				','
			});
			return new string[]
			{
				array[2],
				array[3]
			};
		}
		int num3 = str.IndexOf('(') + 1;
		int num4 = str.IndexOf(')') - num3;
		return str.Substring(num3, num4).Split(new char[]
		{
			','
		});
	}

	private string GetProportion(double jichubi, int sklev)
	{
		double num = jichubi + jichubi / 200.0 * (double)sklev;
		return string.Format("{{00ff00}}{0}%{{-}}", (num * 100.0).ToString("0.00"));
	}

	private string GetProportion(double jichubi, int sklev, double nextjichubi)
	{
		double num = jichubi + jichubi / 200.0 * (double)sklev + (nextjichubi + nextjichubi / 200.0 * (double)sklev);
		return string.Format("{{00ff00}}{0}%{{-}}", (num * 100.0).ToString("0.00"));
	}

	private string GetGuDingzhi(int jichubi, int sklev)
	{
		return string.Format("{{00ff00}}{0}{{-}}", (jichubi + jichubi * sklev).ToString());
	}

	private string GetGuDingzhi(int jichubi, int sklev, int nexjichubi)
	{
		return string.Format("{{00ff00}}{0}{{-}}", (jichubi + jichubi * sklev + (nexjichubi + nexjichubi * sklev)).ToString());
	}

	private string GetChufalv(double chufalv, int sklev)
	{
		if (sklev >= 100)
		{
			sklev = 100;
		}
		double num = chufalv + chufalv / 200.0 * (double)sklev;
		return string.Format("{{00ff00}}{0}%{{-}}", (num * 100.0).ToString("0.00"));
	}

	private Dictionary<int, SkillItem> SkillIconDict = new Dictionary<int, SkillItem>();

	public ListBox listBox;

	public GButton upSkillLevBtn;

	public GButton upSkillSLDBtn;

	public TextBlock needJifenText;

	public TextBlock needJinbiText;

	public TextBlock needLeveText;

	public TextBlock skiilNameText;

	public TextBlock nowXiaoGuoText;

	public TextBlock nextXiaoGuoText;

	public TextBlock cdText;

	public TextBlock shifangfanweiText;

	public TextBlock xiaohaoText;

	public GImgProgressBar LevProgBar;

	public TextBlock progBarText;

	public UIDraggablePanel uIDraggablePanel;

	public GameObject[] Anim;

	public GameObject Arrows;

	public int skillMaxShowCount = 7;

	public TextBlock ConstText_NowXiaoGuo;

	public TextBlock ConstText_NextXiaoGuo;

	private ObservableCollection ItemCollection;

	private int skillValue;

	private SkillItem temp;
}
