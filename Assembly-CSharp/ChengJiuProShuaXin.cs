using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ChengJiuProShuaXin : MonoBehaviour
{
	private void Start()
	{
		this.UITopLabelTitle.text = Global.GetLang("钻石消耗:");
		this.UIBottomLabelTitle.text = Global.GetLang("成就消耗:");
		this.UITopLabelTitle.pivot = 5;
		this.UITopLabelTitle.transform.localPosition = new Vector3(30f, this.UITopLabelTitle.transform.localPosition.y, this.UITopLabelTitle.transform.localPosition.z);
		this.UIBottomLabelTitle.pivot = 5;
		this.UIBottomLabelTitle.transform.localPosition = new Vector3(30f, this.UIBottomLabelTitle.transform.localPosition.y, this.UIBottomLabelTitle.transform.localPosition.z);
	}

	public void Init()
	{
		this.DicPro.Clear();
		this.DicPro.Add("hp", this.HP);
		this.DicPro.Add("att", this.Att);
		this.DicPro.Add("def", this.Def);
		this.DicPro.Add("hit", this.Hit);
		this.barLabel.text = "0/100";
		this.bar.BodyHeight = 16.0;
		this.bar.BodyWidth = 262.0;
		this.bar.Percent = 0.0;
	}

	public void refreshUI(int id, int addValue = 0)
	{
		if (this.DicPro == null || this.DicPro.Count <= 0)
		{
			return;
		}
		ShuXingTiSheng shuXingTiSheng = this.DicPro["hp"];
		ShuXingTiSheng shuXingTiSheng2 = this.DicPro["att"];
		ShuXingTiSheng shuXingTiSheng3 = this.DicPro["def"];
		ShuXingTiSheng shuXingTiSheng4 = this.DicPro["hit"];
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuFuWen.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "ChengJiuFuWen", "ID", id.ToString());
		XElement xelement = xelementList[0];
		Global.GetXElementAttributeInt(xelement, "LifeV");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Dodge");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "AddDefense");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "AddAttack");
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "LifeV");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Picture");
		string[] array = Global.GetXElementAttributeStr(xelement, "QiangHua").ToString().Split(new char[]
		{
			'|'
		});
		string[] array2 = array[0].Split(new char[]
		{
			','
		});
		int num = xelementAttributeInt4;
		int wcd = 100;
		int wcd2 = 100;
		int wcd3 = 100;
		int wcd4 = 100;
		if (id == Global.Data.ChengjiuFuWen.RuneID)
		{
			int lifeAdd = Global.Data.ChengjiuFuWen.LifeAdd;
			num = Global.Data.ChengjiuFuWen.AttackAdd;
			int defenseAdd = Global.Data.ChengjiuFuWen.DefenseAdd;
			int dodgeAdd = Global.Data.ChengjiuFuWen.DodgeAdd;
			wcd = lifeAdd / xelementAttributeInt5 * 100;
			wcd2 = num / xelementAttributeInt4 * 100;
			wcd3 = defenseAdd / xelementAttributeInt3 * 100;
			wcd4 = dodgeAdd / xelementAttributeInt2 * 100;
			this.UseStoneChengJiu(Global.Data.ChengjiuFuWen.Diamond, Global.Data.ChengjiuFuWen.Achievement);
			shuXingTiSheng.setAddPro(lifeAdd.ToString(), int.Parse(array2[1]), 0, 0, this.strMax);
			shuXingTiSheng2.setAddPro(num.ToString(), int.Parse(array2[2]), 0, 0, this.strMaxAtt);
			shuXingTiSheng3.setAddPro(defenseAdd.ToString(), int.Parse(array2[3]), 0, 0, this.strMaxDif);
			shuXingTiSheng4.setAddPro(dodgeAdd.ToString(), int.Parse(array2[4]), 0, 0, this.strMaxHit);
			this.setStarBar(this.getFuWenWCD((float)xelementAttributeInt2, (float)xelementAttributeInt3, (float)xelementAttributeInt4, (float)xelementAttributeInt5), this.getFuWenStarWCD((float)xelementAttributeInt2, (float)xelementAttributeInt3, (float)xelementAttributeInt4, (float)xelementAttributeInt5));
			if (this.dingji.activeSelf)
			{
				this.dingji.SetActive(false);
			}
		}
		else
		{
			if (!this.dingji.activeSelf)
			{
				this.dingji.SetActive(true);
			}
			this.UseStoneChengJiu(0, 0);
			this.setStarBar(100f, 1f);
			shuXingTiSheng.setAddPro(xelementAttributeInt5.ToString(), 0, -1, wcd, this.strMax);
			shuXingTiSheng2.setAddPro(num.ToString(), 0, -1, wcd2, this.strMaxAtt);
			shuXingTiSheng3.setAddPro(xelementAttributeInt3.ToString(), 0, -1, wcd3, this.strMaxDif);
			shuXingTiSheng4.setAddPro(xelementAttributeInt2.ToString(), 0, -1, wcd4, this.strMaxHit);
		}
		this.refreshIcon(xelementAttributeStr2, xelementAttributeStr);
	}

	public void refreshIcon(string IconName, string Name)
	{
		string[] array = IconName.Split(new char[]
		{
			'.'
		});
		this.HeadSp.spriteName = array[0];
		this.HeadSp.MakePixelPerfect();
	}

	public void UseStoneChengJiu(int stone, int cj)
	{
		this.stoneObje.SetActive(stone != 0 || 0 != cj);
		this.stoneLabel.text = ((stone > 0) ? stone.ToString() : Global.GetLang("免费"));
		if (cj <= 0)
		{
			this.ChengJiuObje.SetActive(false);
		}
		else
		{
			this.ChengJiuObje.SetActive(true);
		}
		this.ChengJiuLabel.text = cj.ToString();
	}

	public void CreatePre(int strIndex)
	{
		if (this.baoji.Count == 0 || this.baoji.Count <= strIndex)
		{
			return;
		}
		if (this.baoji[strIndex].activeSelf)
		{
			this.baoji[strIndex].SetActive(false);
		}
		this.baoji[strIndex].SetActive(true);
	}

	public void DesPre(object game)
	{
		TweenAlpha tweenAlpha = game as TweenAlpha;
		Object.Destroy(tweenAlpha.gameObject);
	}

	private void setFuWenBar(float wcd)
	{
		if (wcd >= 1f)
		{
			wcd = 1f;
		}
		float num = wcd * 100f;
		int num2 = (int)num;
		this.barLabel.text = string.Format("{0}%", num2);
		float num3 = wcd;
		this.bar.Percent = (double)num3;
	}

	public void setStarVisible(bool isok, int index = -1)
	{
		if (index == -1)
		{
			for (int i = 0; i < this.FuWenStar.Count; i++)
			{
				this.FuWenStar[i].SetActive(isok);
			}
		}
		else
		{
			for (int j = 0; j < index; j++)
			{
				this.FuWenStar[index].SetActive(isok);
			}
		}
	}

	private void setStarBar(float wcd, float starWcd)
	{
		if (wcd == 100f)
		{
			this.setStarVisible(true, -1);
			this.setFuWenBar(1f);
			return;
		}
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("DianLiangBiaoZhi", ',');
		for (int i = 0; i < systemParamFloatArrayByName.Length; i++)
		{
			float num = systemParamFloatArrayByName[i];
			if (num > wcd)
			{
				break;
			}
			if (!this.FuWenStar[i].activeSelf)
			{
				this.FuWenStar[i].SetActive(true);
			}
		}
		this.setFuWenBar(starWcd);
	}

	private float getFuWenWCD(float dodge, float defense, float attack, float lifev)
	{
		float num = (float)Global.Data.ChengjiuFuWen.DodgeAdd / dodge * 0.25f;
		float num2 = (float)Global.Data.ChengjiuFuWen.DefenseAdd / defense * 0.25f;
		float num3 = (float)Global.Data.ChengjiuFuWen.AttackAdd / attack * 0.25f;
		float num4 = (float)Global.Data.ChengjiuFuWen.LifeAdd / lifev * 0.25f;
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

	private float getFuWenStarWCD(float dodge, float defense, float attack, float lifev)
	{
		float num = (float)Global.Data.ChengjiuFuWen.DodgeAdd / dodge;
		float num2 = (float)Global.Data.ChengjiuFuWen.DefenseAdd / defense;
		float num3 = (float)Global.Data.ChengjiuFuWen.AttackAdd / attack;
		float num4 = (float)Global.Data.ChengjiuFuWen.LifeAdd / lifev;
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

	public ShuXingTiSheng HP;

	public ShuXingTiSheng Att;

	public ShuXingTiSheng Def;

	public ShuXingTiSheng Hit;

	public Dictionary<string, ShuXingTiSheng> DicPro = new Dictionary<string, ShuXingTiSheng>();

	public UISprite HeadSp;

	public UILabel HeadName;

	public GameObject stoneObje;

	public GameObject ChengJiuObje;

	public UILabel stoneLabel;

	public UILabel ChengJiuLabel;

	public GameObject fontAddition;

	private string strMax = Global.GetLang("生命上限  ：{0}");

	private string strMaxAtt = Global.GetLang("攻击力       : {0}");

	private string strMaxDif = Global.GetLang("防御力       : {0}");

	private string strMaxHit = Global.GetLang("闪避：{0}");

	public GImgProgressBar bar;

	public UILabel barLabel;

	public List<GameObject> baoji = new List<GameObject>();

	public List<GameObject> FuWenStar = new List<GameObject>();

	public GameObject baoshifaguang;

	private int starIndex;

	public GameObject dingji;

	public UILabel UITopLabelTitle;

	public UILabel UIBottomLabelTitle;
}
