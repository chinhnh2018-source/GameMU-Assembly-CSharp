using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class FuWenItemManager : MonoBehaviour
{
	private void Start()
	{
	}

	public void init()
	{
		for (int i = 0; i < this.listFW.Count; i++)
		{
			this.listFW[i].DPSelectedItem = this.DPSelectedItem;
		}
	}

	public int typeFw
	{
		get
		{
			return this._typeFw;
		}
		set
		{
			this.typeFw = value;
		}
	}

	public void initUI()
	{
		AchievementRuneData chengjiuFuWen = Global.Data.ChengjiuFuWen;
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuFuWen.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		foreach (XElement xelement in xelementList)
		{
			int num = int.Parse(Global.GetXElementAttributeStr(xelement, "ID"));
			float dodge = float.Parse(Global.GetXElementAttributeStr(xelement, "Dodge"));
			float defense = float.Parse(Global.GetXElementAttributeStr(xelement, "AddDefense"));
			float attack = float.Parse(Global.GetXElementAttributeStr(xelement, "AddAttack"));
			float lifev = float.Parse(Global.GetXElementAttributeStr(xelement, "LifeV"));
			int num2 = num - 1;
			if (num < chengjiuFuWen.RuneID)
			{
				if (this.listFW.Count == num)
				{
					this.listFW[num2].setFuWenProgress(100f, true, true);
				}
				else
				{
					this.listFW[num2].setFuWenProgress(100f, true, false);
				}
			}
			else if (num == chengjiuFuWen.RuneID)
			{
				float fuWenWCD = this.getFuWenWCD(dodge, defense, attack, lifev);
				this.listFW[num2].setFuWenProgress(fuWenWCD, true, true);
			}
			else
			{
				this.listFW[num2].setFuWenProgress(0f, false, false);
			}
		}
	}

	public void showKaiQiFuWenLine()
	{
		int num = Global.Data.ChengjiuFuWen.RuneID - 1;
		if (Global.Data.ChengjiuFuWen.UpResultType == 3)
		{
			num = 5;
			this.listSp[this.listSp.Count - 1].spriteName = "guang_1";
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.listSp[i].spriteName = "guang_1";
			}
		}
	}

	public void UpdateUI(int id)
	{
		XElement xmle = this.getXmle(id);
		int num = int.Parse(Global.GetXElementAttributeStr(xmle, "ID"));
		float dodge = float.Parse(Global.GetXElementAttributeStr(xmle, "Dodge"));
		float defense = float.Parse(Global.GetXElementAttributeStr(xmle, "AddDefense"));
		float attack = float.Parse(Global.GetXElementAttributeStr(xmle, "AddAttack"));
		float lifev = float.Parse(Global.GetXElementAttributeStr(xmle, "LifeV"));
		float fuWenWCD = this.getFuWenWCD(dodge, defense, attack, lifev);
		this.listFW[id - 1].setFuWenProgress(fuWenWCD, true, true);
	}

	public bool isFuWen(int id)
	{
		int num = id - 1;
		return this.listFW[num].isOK;
	}

	public GameObject getLocalPosition(int index)
	{
		int num = index - 1;
		if (num < 0 || this.listFW.Count <= num)
		{
			return null;
		}
		return this.listFW[num].Gbtn.gameObject;
	}

	private float getFuWenWCD(float dodge, float defense, float attack, float lifev)
	{
		float num = (float)Global.Data.ChengjiuFuWen.DodgeAdd / dodge * 0.25f;
		float num2 = (float)Global.Data.ChengjiuFuWen.DefenseAdd / defense * 0.25f;
		float num3 = (float)Global.Data.ChengjiuFuWen.AttackAdd / attack * 0.25f;
		float num4 = (float)Global.Data.ChengjiuFuWen.LifeAdd / lifev * 0.25f;
		float num5 = (num + num2 + num3 + num4) * 100f;
		return (float)((int)num5);
	}

	public void CreatePre(string str = "")
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.fontAddition);
		gameObject.transform.parent = this.fontAddition.transform.parent;
		gameObject.transform.localScale = new Vector3(28f, 28f, 0f);
		if (str != string.Empty)
		{
			gameObject.GetComponent<UILabel>().text = str;
		}
		gameObject.SetActive(true);
	}

	public void DesPre(object game)
	{
		TweenAlpha tweenAlpha = game as TweenAlpha;
		Object.Destroy(tweenAlpha.gameObject);
	}

	private XElement getXmle(int id)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuFuWen.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		foreach (XElement xelement in xelementList)
		{
			int num = int.Parse(Global.GetXElementAttributeStr(xelement, "ID"));
			if (id == num)
			{
				return xelement;
			}
		}
		return null;
	}

	public void setBtnChoise(int oldType, int newType)
	{
		if (oldType == newType)
		{
			return;
		}
		int num = oldType - 1;
		int num2 = newType - 1;
		this.listFW[num].choise.SetActive(false);
		this.listFW[num].nomal.SetActive(true);
		this.listFW[num2].choise.SetActive(true);
		this.listFW[num2].nomal.SetActive(false);
	}

	public List<FuWenItem> listFW = new List<FuWenItem>();

	private int _typeFw = 1;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject fontAddition;

	public List<UISprite> listSp = new List<UISprite>();
}
