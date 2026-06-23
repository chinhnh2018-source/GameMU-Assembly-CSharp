using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class AdendaPropertyViewController : MonoBehaviour
{
	private void InitTextInPrefabs()
	{
		this.levelupMinimumPrestige.text = Global.GetLang("消耗声望：");
		this.levelupMinimumDiamond.text = Global.GetLang("消耗：");
	}

	public void Init()
	{
		this.InitTextInPrefabs();
		this.propertyDic.Clear();
		this.propertyDic.Add("hp", this.hpItem);
		this.propertyDic.Add("att", this.attackItem);
		this.propertyDic.Add("def", this.defenceItem);
		this.propertyDic.Add("hit", this.hitItem);
		this.progressPercent.text = "0/100";
		this.progressBar.BodyHeight = 16.0;
		this.progressBar.BodyWidth = 276.0;
		this.progressBar.Percent = 0.0;
	}

	public void RefreshNewPropertiesByMedalID(int id)
	{
		if (this.propertyDic == null || this.propertyDic.Count <= 0)
		{
			return;
		}
		AdendaItemPropertyHelperController adendaItemPropertyHelperController = this.propertyDic["hp"];
		AdendaItemPropertyHelperController adendaItemPropertyHelperController2 = this.propertyDic["att"];
		AdendaItemPropertyHelperController adendaItemPropertyHelperController3 = this.propertyDic["def"];
		AdendaItemPropertyHelperController adendaItemPropertyHelperController4 = this.propertyDic["hit"];
		XElement xelementByID = this.GetXElementByID(id);
		string[] array = Global.GetXElementAttributeStr(xelementByID, "QiangHua").ToString().Split(new char[]
		{
			'|'
		});
		string[] array2 = array[0].Split(new char[]
		{
			','
		});
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementByID, "LifeV");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementByID, "AddAttack");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementByID, "AddDefense");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelementByID, "HitV");
		int num = xelementAttributeInt2;
		int percent = 100;
		int percent2 = 100;
		int percent3 = 100;
		int percent4 = 100;
		if (id == Global.Data.adendaData.MedalID)
		{
			int lifeAdd = Global.Data.adendaData.LifeAdd;
			num = Global.Data.adendaData.AttackAdd;
			int defenseAdd = Global.Data.adendaData.DefenseAdd;
			int hitAdd = Global.Data.adendaData.HitAdd;
			percent = lifeAdd / xelementAttributeInt * 100;
			percent2 = num / xelementAttributeInt2 * 100;
			percent3 = defenseAdd / xelementAttributeInt3 * 100;
			percent4 = hitAdd / xelementAttributeInt4 * 100;
			this.SetAvailableAttributeFields(Global.Data.adendaData.Diamond, Global.Data.adendaData.Prestige);
			adendaItemPropertyHelperController.SetAdditionProperty(lifeAdd.ToString(), int.Parse(array2[1]), 0, 0, Global.GetLang("生命上限：{0}"));
			adendaItemPropertyHelperController2.SetAdditionProperty(num.ToString(), int.Parse(array2[2]), 0, 0, Global.GetLang("攻击力     : {0}"));
			adendaItemPropertyHelperController3.SetAdditionProperty(defenseAdd.ToString(), int.Parse(array2[3]), 0, 0, Global.GetLang("防御力     : {0}"));
			adendaItemPropertyHelperController4.SetAdditionProperty(hitAdd.ToString(), int.Parse(array2[4]), 0, 0, Global.GetLang("命中       ：{0}"));
			this.setStarBar(this.GetMedalItemCompletePercent((float)xelementAttributeInt4, (float)xelementAttributeInt3, (float)xelementAttributeInt2, (float)xelementAttributeInt), this.GetTrismStarCompletePercent((float)xelementAttributeInt4, (float)xelementAttributeInt3, (float)xelementAttributeInt2, (float)xelementAttributeInt));
			this.hintReachedTheTop.SetActive(false);
		}
		else
		{
			this.hintReachedTheTop.SetActive(true);
			this.SetAvailableAttributeFields(0, 0);
			this.setStarBar(100f, 1f);
			adendaItemPropertyHelperController.SetAdditionProperty(xelementAttributeInt.ToString(), 0, -1, percent, Global.GetLang("生命上限：{0}"));
			adendaItemPropertyHelperController2.SetAdditionProperty(num.ToString(), 0, -1, percent2, Global.GetLang("攻击力     : {0}"));
			adendaItemPropertyHelperController3.SetAdditionProperty(xelementAttributeInt3.ToString(), 0, -1, percent3, Global.GetLang("防御力     : {0}"));
			adendaItemPropertyHelperController4.SetAdditionProperty(xelementAttributeInt4.ToString(), 0, -1, percent4, Global.GetLang("命中       ：{0}"));
		}
	}

	public void SetThumbnail(int id)
	{
		if (id < 1 || id > 6)
		{
			return;
		}
		if (null != this.thumbnail)
		{
			this.thumbnail.URL = "NetImages/GameRes/Images/Adenda/hornour_level_" + id + ".png";
		}
	}

	public void SetAvailableAttributeFields(int diamond, int prestige)
	{
		this.diamondField.SetActive(diamond != 0 || 0 != prestige);
		this.diamondLabel.text = ((diamond > 0) ? diamond.ToString() : Global.GetLang("免费"));
		if (prestige <= 0)
		{
			this.prestigeField.SetActive(false);
		}
		else
		{
			this.prestigeField.SetActive(true);
		}
		this.prestigeLabel.text = prestige.ToString();
	}

	public void PlayBurstEffectByType(int typeIndex)
	{
		if (this.burstEffects.Count == 0 || this.burstEffects.Count <= typeIndex)
		{
			return;
		}
		if (this.burstEffects[typeIndex].activeSelf)
		{
			this.burstEffects[typeIndex].SetActive(false);
		}
		this.burstEffects[typeIndex].SetActive(true);
	}

	public void DestroyBurstEffect(object game)
	{
		TweenAlpha tweenAlpha = game as TweenAlpha;
		Object.Destroy(tweenAlpha.gameObject);
	}

	private XElement GetXElementByID(int id)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShengWangXunZhang.xml");
		return Global.GetXElement(gameResXml, "ShengWangXunZhang", "ID", id.ToString());
	}

	private void SetMedalItemCompletePercent(float percent)
	{
		if (percent >= 1f)
		{
			percent = 1f;
		}
		float num = percent * 100f;
		this.progressPercent.text = Mathf.Floor(num) + "%";
		this.progressBar.Percent = (double)percent;
	}

	public void SetStarVisible(bool visible)
	{
		for (int i = 0; i < this.trismStar.Count; i++)
		{
			this.trismStar[i].SetActive(visible);
		}
	}

	private void setStarBar(float percent, float starPercent)
	{
		if (percent == 100f)
		{
			this.SetStarVisible(true);
			this.SetMedalItemCompletePercent(1f);
			return;
		}
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("DianLiangBiaoZhi", ',');
		for (int i = 0; i < systemParamFloatArrayByName.Length; i++)
		{
			float num = systemParamFloatArrayByName[i];
			if (num > percent)
			{
				break;
			}
			if (!this.trismStar[i].activeSelf)
			{
				this.trismStar[i].SetActive(true);
			}
		}
		this.SetMedalItemCompletePercent(starPercent);
	}

	private float GetMedalItemCompletePercent(float hitv, float defense, float attack, float lifev)
	{
		float num = (float)Global.Data.adendaData.HitAdd / hitv * 0.25f;
		float num2 = (float)Global.Data.adendaData.DefenseAdd / defense * 0.25f;
		float num3 = (float)Global.Data.adendaData.AttackAdd / attack * 0.25f;
		float num4 = (float)Global.Data.adendaData.LifeAdd / lifev * 0.25f;
		float num5 = (num + num2 + num3 + num4) * 100f;
		float num6 = Mathf.Floor(num5);
		this.starIndex = 0;
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("DianLiangBiaoZhi", ',');
		float num7 = systemParamFloatArrayByName[this.starIndex];
		foreach (float num7 in systemParamFloatArrayByName)
		{
			if (num7 <= num6)
			{
				this.starIndex++;
			}
		}
		return num6;
	}

	private float GetTrismStarCompletePercent(float hitv, float defense, float attack, float lifev)
	{
		float num = (float)Global.Data.adendaData.HitAdd / hitv;
		float num2 = (float)Global.Data.adendaData.DefenseAdd / defense;
		float num3 = (float)Global.Data.adendaData.AttackAdd / attack;
		float num4 = (float)Global.Data.adendaData.LifeAdd / lifev;
		float result;
		if (this.starIndex == 5)
		{
			result = ((num + num2 + num3 + num4) * 0.25f - (float)this.starIndex * 0.16f) / 0.2f;
		}
		else
		{
			result = ((num + num2 + num3 + num4) * 0.25f - (float)this.starIndex * 0.16f) / 0.16f;
		}
		return result;
	}

	public void RefreshAvailablePrestigeField()
	{
		if (Global.Data.adendaData != null)
		{
			this.leftPrestige.text = Global.Data.adendaData.PrestigeLeft.ToString();
		}
		else
		{
			this.leftPrestige.text = "0";
		}
	}

	public void RefreshAvailableDiamondField()
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ShengWangYinJi", Global.Data.adendaData.Diamond, string.Empty);
		if (Global.Data.roleData != null)
		{
			this.leftDiamond.text = Global.Data.roleData.UserMoney.ToString();
		}
		else
		{
			this.leftDiamond.text = "0";
		}
	}

	private const string netImagePath = "NetImages/GameRes/Images/Adenda/";

	private const string imagePrefix = "hornour_level_";

	private const string imageSuffix = ".png";

	public AdendaItemPropertyHelperController hpItem;

	public AdendaItemPropertyHelperController attackItem;

	public AdendaItemPropertyHelperController defenceItem;

	public AdendaItemPropertyHelperController hitItem;

	public ShowNetImage thumbnail;

	public GameObject diamondField;

	public GameObject prestigeField;

	public UILabel diamondLabel;

	public UILabel prestigeLabel;

	public UILabel leftDiamond;

	public UILabel leftPrestige;

	public UILabel levelupMinimumPrestige;

	public UILabel levelupMinimumDiamond;

	public GameObject fontAddition;

	public GImgProgressBar progressBar;

	public UILabel progressPercent;

	public List<GameObject> burstEffects = new List<GameObject>();

	public List<GameObject> trismStar = new List<GameObject>();

	public GameObject shinningEffect;

	private int starIndex;

	public GameObject hintReachedTheTop;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private Dictionary<string, AdendaItemPropertyHelperController> propertyDic = new Dictionary<string, AdendaItemPropertyHelperController>();
}
