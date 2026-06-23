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

public class Zhuhun_Part : UserControl
{
	private void InitTextInPrefabs()
	{
		this.StaticStr0.text = Global.GetLang("翅膀注魂");
		this.StaticStr1.text = Global.GetLang("翅膀注魂属性加成");
		this.StaticStr2.text = Global.GetLang("翅膀注魂消耗");
		this.StaticStr3.text = Global.GetLang("消耗金币:");
		this.StaticStr4.text = Global.GetLang("注魂");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		XElement gameResXml = Global.GetGameResXml("Config/ZhuLingType.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[1], "GoodsID");
		string[] array = xelementAttributeStr.Split(new char[]
		{
			','
		});
		this.NeedGoodID = Convert.ToInt32(array[0]);
		this.GoodIcon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			this.NeedGoodID
		});
		this.NeedNum = Convert.ToInt32(array[1]);
		this.NeedMoney = Global.GetXElementAttributeInt(xelementList[1], "CostBandJinBi");
		XElement gameResXml2 = Global.GetGameResXml("Config/WinZhuLing.xml");
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "ZhuLing");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList2[i], "TypeID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList2[i], "Occupation");
			if (xelementAttributeInt == 2 && Global.Data.roleData.Occupation == xelementAttributeInt2)
			{
				this.increasePercent = Global.GetXElementAttributeDouble(xelementList2[i], "AllAttribute");
			}
		}
		XElement gameResXml3 = Global.GetGameResXml("Config/MaxWinZhuLing.xml");
		List<XElement> xelementList3 = Global.GetXElementList(gameResXml3, "ZhuLing");
		for (int j = 0; j < xelementList3.Count; j++)
		{
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList3[j], "SuitID");
			if (xelementAttributeInt3 == Global.Data.roleData.MyWingData.WingID)
			{
				this.ZhuLingMaxTimes = Global.GetXElementAttributeInt(xelementList3[j], "SeniorZhuLing");
			}
		}
		this.ZhuLingTotalMaxTimes = Global.GetXElementAttributeInt(xelementList3[xelementList3.Count - 1], "SeniorZhuLing");
		this.ZhuLingTimes = Global.Data.roleData.MyWingData.ZhuHunNum;
		this.UpdataData();
		this.Zhuhun.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ZhuLingTimes >= this.ZhuLingMaxTimes)
			{
				Super.HintMainText(Global.GetLang("注魂已到达最大值"), 10, 3);
				return;
			}
			long num = (long)Global.Data.roleData.Money1 + (long)Global.Data.roleData.YinLiang;
			if (num < (long)this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, Global.GetLang("金币"));
				return;
			}
			this.TotalNum = 0;
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
				{
					if (this.NeedGoodID == Global.Data.roleData.GoodsDataList[k].GoodsID)
					{
						this.TotalNum += Global.Data.roleData.GoodsDataList[k].GCount;
					}
				}
			}
			if (this.TotalNum < this.NeedNum)
			{
				Super.HintMainText(Global.GetLang("所需道具不足"), 10, 3);
				return;
			}
			GameInstance.Game.StartZhuHun();
		};
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		this.GoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = this.NeedGoodID;
			GTipServiceEx.ShowTip(this.GoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		};
	}

	public void LevelUp()
	{
		this.ZhuLingTimes++;
		Global.Data.roleData.MyWingData.ZhuHunNum++;
		this.UpdataData();
		GameObject gameObject = Object.Instantiate<GameObject>(this.Effect.gameObject);
		gameObject.transform.parent = this.ZhuHun;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private XElement GetWingStarXmlNode(string id, string starid, string occupation)
	{
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/WingStar_{0}.xml", occupation));
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "Wing", "ID", id);
		if (xelement == null)
		{
			return null;
		}
		xelement = Global.GetXElement(xelement, "Item", "Star", starid);
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	private void InitChiBangInfo()
	{
		if (Global.Data.roleData.MyWingData == null)
		{
			return;
		}
		string text = Global.Data.roleData.MyWingData.WingID.ToString();
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/Wing_{0}.xml", Global.Data.roleData.Occupation));
		XElement xelement = Global.GetXElement(gameResXml, "Level", "ID", text);
		if (xelement == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinAttackV");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxAttackV");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinMAttackV");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxMAttackV");
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MinDefenseV");
		int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "MaxDefenseV");
		int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement, "MinMDefenseV");
		int xelementAttributeInt8 = Global.GetXElementAttributeInt(xelement, "MaxMDefenseV");
		int xelementAttributeInt9 = Global.GetXElementAttributeInt(xelement, "HitV");
		int xelementAttributeInt10 = Global.GetXElementAttributeInt(xelement, "Dodge");
		int xelementAttributeInt11 = Global.GetXElementAttributeInt(xelement, "MaxLifeV");
		this.BasSubAttackInjurePercent = Global.GetXElementAttributeDouble(xelement, "SubAttackInjurePercent");
		this.BasAddAttackInjurePercent = Global.GetXElementAttributeDouble(xelement, "AddAttackInjurePercent");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int forgeLevel = Global.Data.roleData.MyWingData.ForgeLevel;
		XElement wingStarXmlNode = this.GetWingStarXmlNode(text, forgeLevel.ToString(), Global.Data.roleData.Occupation.ToString());
		if (wingStarXmlNode != null)
		{
			num = Global.GetXElementAttributeInt(wingStarXmlNode, "MinAttackV");
			num2 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxAttackV");
			num3 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinMAttackV");
			num4 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMAttackV");
			num5 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinDefenseV");
			num6 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxDefenseV");
			num7 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinMDefenseV");
			num8 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMDefenseV");
			num9 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxLifeV");
			num10 = Global.GetXElementAttributeInt(wingStarXmlNode, "HitV");
			num11 = Global.GetXElementAttributeInt(wingStarXmlNode, "Dodge");
		}
		this.intTotalMinAttackV = (int)((double)(xelementAttributeInt + num) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMaxAttackV = (int)((double)(xelementAttributeInt2 + num2) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMinMAttackV = (int)((double)(xelementAttributeInt3 + num3) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMaxMAttackV = (int)((double)(xelementAttributeInt4 + num4) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMinDefenseV = (int)((double)(xelementAttributeInt5 + num5) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMaxDefenseV = (int)((double)(xelementAttributeInt6 + num6) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMinMDefenseV = (int)((double)(xelementAttributeInt7 + num7) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMaxMDefenseV = (int)((double)(xelementAttributeInt8 + num8) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalMaxLifeV = (int)((double)(xelementAttributeInt11 + num9) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalHitVV = (int)((double)(xelementAttributeInt9 + num10) * (this.increasePercent * (double)this.ZhuLingTimes));
		this.intTotalDodgeV = (int)((double)(xelementAttributeInt10 + num11) * (this.increasePercent * (double)this.ZhuLingTimes));
		if (xelementAttributeInt != 0)
		{
			if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
			{
				this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}物理攻击{{-}} {0}-{1}"), this.intTotalMinAttackV, this.intTotalMaxAttackV);
				this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}魔法攻击{{-}} {0}-{1}"), this.intTotalMinMAttackV, this.intTotalMaxMAttackV);
				this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}物理防御{{-}} {0}-{1}"), this.intTotalMinDefenseV, this.intTotalMaxDefenseV);
				this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}魔法防御{{-}} {0}-{1}"), this.intTotalMinMDefenseV, this.intTotalMaxMDefenseV);
				this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV);
				this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV);
				this.Attribute[6].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV);
			}
			else
			{
				this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}物理攻击{{-}} {0}-{1}"), this.intTotalMinAttackV, this.intTotalMaxAttackV);
				this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}物理防御{{-}} {0}-{1}"), this.intTotalMinDefenseV, this.intTotalMaxDefenseV);
				this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}魔法防御{{-}} {0}-{1}"), this.intTotalMinMDefenseV, this.intTotalMaxMDefenseV);
				this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV);
				this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV);
				this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV);
			}
		}
		else
		{
			this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}魔法攻击{{-}} {0}-{1}"), this.intTotalMinMAttackV, this.intTotalMaxMAttackV);
			this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}物理防御{{-}} {0}-{1}"), this.intTotalMinDefenseV, this.intTotalMaxDefenseV);
			this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}魔法防御{{-}} {0}-{1}"), this.intTotalMinMDefenseV, this.intTotalMaxMDefenseV);
			this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV);
			this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV);
			this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV);
		}
		if (this.ZhuLingTimes < 50)
		{
			if (xelementAttributeInt != 0)
			{
				if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
				{
					this.IncreaseAttribute[0].text = ((int)((double)(xelementAttributeInt2 + num2) * this.increasePercent)).ToString();
					this.IncreaseAttribute[1].text = ((int)((double)(xelementAttributeInt4 + num4) * this.increasePercent)).ToString();
					this.IncreaseAttribute[2].text = ((int)((double)(xelementAttributeInt6 + num6) * this.increasePercent)).ToString();
					this.IncreaseAttribute[3].text = ((int)((double)(xelementAttributeInt8 + num8) * this.increasePercent)).ToString();
					this.IncreaseAttribute[4].text = ((int)((double)(xelementAttributeInt9 + num10) * this.increasePercent)).ToString();
					this.IncreaseAttribute[5].text = ((int)((double)(xelementAttributeInt10 + num11) * this.increasePercent)).ToString();
					this.IncreaseAttribute[6].text = ((int)((double)(xelementAttributeInt11 + num9) * this.increasePercent)).ToString();
					for (int i = 0; i < this.Arrow.Length; i++)
					{
						this.Arrow[i].gameObject.SetActive(true);
					}
				}
				else
				{
					this.IncreaseAttribute[0].text = ((int)((double)(xelementAttributeInt2 + num2) * this.increasePercent)).ToString();
					this.IncreaseAttribute[1].text = ((int)((double)(xelementAttributeInt6 + num6) * this.increasePercent)).ToString();
					this.IncreaseAttribute[2].text = ((int)((double)(xelementAttributeInt8 + num8) * this.increasePercent)).ToString();
					this.IncreaseAttribute[3].text = ((int)((double)(xelementAttributeInt9 + num10) * this.increasePercent)).ToString();
					this.IncreaseAttribute[4].text = ((int)((double)(xelementAttributeInt10 + num11) * this.increasePercent)).ToString();
					this.IncreaseAttribute[5].text = ((int)((double)(xelementAttributeInt11 + num9) * this.increasePercent)).ToString();
					for (int j = 0; j < this.Arrow.Length - 1; j++)
					{
						this.Arrow[j].gameObject.SetActive(true);
					}
				}
			}
			else
			{
				this.IncreaseAttribute[0].text = ((int)((double)(xelementAttributeInt4 + num4) * this.increasePercent)).ToString();
				this.IncreaseAttribute[1].text = ((int)((double)(xelementAttributeInt6 + num6) * this.increasePercent)).ToString();
				this.IncreaseAttribute[2].text = ((int)((double)(xelementAttributeInt8 + num8) * this.increasePercent)).ToString();
				this.IncreaseAttribute[3].text = ((int)((double)(xelementAttributeInt9 + num10) * this.increasePercent)).ToString();
				this.IncreaseAttribute[4].text = ((int)((double)(xelementAttributeInt10 + num11) * this.increasePercent)).ToString();
				this.IncreaseAttribute[5].text = ((int)((double)(xelementAttributeInt11 + num9) * this.increasePercent)).ToString();
				for (int k = 0; k < this.Arrow.Length - 1; k++)
				{
					this.Arrow[k].gameObject.SetActive(true);
				}
			}
		}
		else
		{
			for (int l = 0; l < this.Arrow.Length; l++)
			{
				this.Arrow[l].gameObject.SetActive(false);
				this.IncreaseAttribute[l].text = string.Empty;
			}
		}
	}

	public void UpdataData()
	{
		this.TotalNum = 0;
		this.BindingNum = 0;
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (this.NeedGoodID == Global.Data.roleData.GoodsDataList[i].GoodsID)
				{
					this.TotalNum += Global.Data.roleData.GoodsDataList[i].GCount;
					if (Global.Data.roleData.GoodsDataList[i].Binding == 1)
					{
						this.BindingNum += Global.Data.roleData.GoodsDataList[i].GCount;
					}
				}
			}
		}
		long num = (long)Global.Data.roleData.Money1 + (long)Global.Data.roleData.YinLiang;
		if (num > (long)this.NeedMoney)
		{
			this.Money.text = "{FFFFFF}";
		}
		else
		{
			this.Money.text = "{FF0000}";
		}
		UILabel money = this.Money;
		money.text = money.text + this.NeedMoney.ToString() + "{-}";
		if (this.NeedNum > this.TotalNum)
		{
			this.GoodIcon.EnableIcon = false;
			this.GoodIcon.ContentText.textColor = 16711680U;
		}
		else
		{
			this.GoodIcon.EnableIcon = true;
			this.GoodIcon.ContentText.textColor = 16777215U;
		}
		this.GoodIcon.ContentText.text = this.TotalNum.ToString() + "/" + this.NeedNum.ToString();
		if (0 < this.BindingNum)
		{
			this.GoodIcon.BindingSprite.gameObject.SetActive(true);
		}
		else
		{
			this.GoodIcon.BindingSprite.gameObject.SetActive(false);
		}
		if (this.ZhuLingTimes >= this.ZhuLingMaxTimes)
		{
			this.ZhuHunFull.gameObject.SetActive(true);
		}
		else
		{
			this.ZhuHunFull.gameObject.SetActive(false);
		}
		this.Zhuru.text = Global.GetLang("已注入") + ((float)this.ZhuLingTimes / (float)this.ZhuLingTotalMaxTimes * 100f).ToString() + "%";
		this.Water.GetComponent<Renderer>().material.SetFloat("_Mask", (float)this.ZhuLingTimes / (float)this.ZhuLingTotalMaxTimes);
		this.InitChiBangInfo();
	}

	public UILabel StaticStr0;

	public UILabel StaticStr1;

	public UILabel StaticStr2;

	public UILabel StaticStr3;

	public UILabel StaticStr4;

	public GGoodIcon GoodIcon;

	public GButton Zhuhun;

	public GButton Close;

	public UILabel Zhuru;

	public UILabel Tisheng;

	public UILabel Money;

	public UILabel[] Attribute;

	public UILabel[] IncreaseAttribute;

	public UISprite[] Arrow;

	public Transform Effect;

	public Transform ZhuHun;

	public Transform ZhuHunFull;

	public GameObject Water;

	public int ZhuLingTimes;

	private int ZhuLingMaxTimes;

	private int ZhuLingTotalMaxTimes;

	private double increasePercent;

	private int TotalNum;

	private int BindingNum;

	private int NeedGoodID;

	private int NeedNum;

	private int NeedMoney;

	private int intTotalMinAttackV;

	private int intTotalMaxAttackV;

	private int intTotalMinMAttackV;

	private int intTotalMaxMAttackV;

	private int intTotalMinDefenseV;

	private int intTotalMaxDefenseV;

	private int intTotalMinMDefenseV;

	private int intTotalMaxMDefenseV;

	private int intTotalMaxLifeV;

	private int intTotalHitVV;

	private int intTotalDodgeV;

	private double BasSubAttackInjurePercent;

	private double BasAddAttackInjurePercent;

	public DPSelectedItemEventHandler DPSelectedItem;
}
