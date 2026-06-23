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

public class ZhuLing_Part : UserControl
{
	private void InitTextInPrefabs()
	{
		this.StaticStr0.text = Global.GetLang("翅膀注灵");
		this.StaticStr1.text = Global.GetLang("翅膀注灵属性加成");
		this.StaticStr2.text = Global.GetLang("翅膀注灵消耗");
		this.StaticStr3.text = Global.GetLang("消耗金币:");
		this.StaticStr4.text = Global.GetLang("注灵");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		XElement gameResXml = Global.GetGameResXml("Config/ZhuLingType.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[0], "GoodsID");
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
		this.NeedMoney = Global.GetXElementAttributeInt(xelementList[0], "CostBandJinBi");
		XElement gameResXml2 = Global.GetGameResXml("Config/WinZhuLing.xml");
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "ZhuLing");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList2[i], "TypeID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList2[i], "Occupation");
			if (xelementAttributeInt == 1 && Global.Data.roleData.Occupation == xelementAttributeInt2)
			{
				this.intTotalMaxAttackV = Global.GetXElementAttributeInt(xelementList2[i], "MaxAttackV");
				this.intTotalMaxMAttackV = Global.GetXElementAttributeInt(xelementList2[i], "MaxMAttackV");
				this.intTotalMaxDefenseV = Global.GetXElementAttributeInt(xelementList2[i], "MaxDefenseV");
				this.intTotalMaxMDefenseV = Global.GetXElementAttributeInt(xelementList2[i], "MaxMDefenseV");
				this.intTotalMaxLifeV = Global.GetXElementAttributeInt(xelementList2[i], "LifeV");
				this.intTotalHitVV = Global.GetXElementAttributeInt(xelementList2[i], "HitV");
				this.intTotalDodgeV = Global.GetXElementAttributeInt(xelementList2[i], "DodgeV");
			}
		}
		XElement gameResXml3 = Global.GetGameResXml("Config/MaxWinZhuLing.xml");
		List<XElement> xelementList3 = Global.GetXElementList(gameResXml3, "ZhuLing");
		for (int j = 0; j < xelementList3.Count; j++)
		{
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList3[j], "SuitID");
			if (xelementAttributeInt3 == Global.Data.roleData.MyWingData.WingID)
			{
				this.ZhuLingMaxTimes = Global.GetXElementAttributeInt(xelementList3[j], "PlainZhuLing");
			}
		}
		this.ZhuLingTotalMaxTimes = Global.GetXElementAttributeInt(xelementList3[xelementList3.Count - 1], "PlainZhuLing");
		this.ZhuLingTimes = Global.Data.roleData.MyWingData.ZhuLingNum;
		this.UpdataData();
		this.Zhuhun.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ZhuLingTimes >= this.ZhuLingMaxTimes)
			{
				Super.HintMainText(Global.GetLang("注灵已到达最大值"), 10, 3);
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
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZhuLing, null, string.Empty, Global.GetLang("洛克之羽"));
				return;
			}
			GameInstance.Game.StartZhuLing();
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
		Global.Data.roleData.MyWingData.ZhuLingNum++;
		this.UpdataData();
		GameObject gameObject = Object.Instantiate<GameObject>(this.Effect.gameObject);
		gameObject.transform.parent = this.Zhuling;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
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
		this.GoodIcon.ContentText.text = this.TotalNum.ToString() + "/" + this.NeedNum.ToString();
		if (0 < this.BindingNum)
		{
			this.GoodIcon.BindingSprite.gameObject.SetActive(true);
		}
		else
		{
			this.GoodIcon.BindingSprite.gameObject.SetActive(false);
		}
		if (this.intTotalMaxAttackV != 0)
		{
			if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
			{
				this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}最大物攻{{-}} {0}"), this.intTotalMaxAttackV * this.ZhuLingTimes);
				this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}最大魔攻{{-}} {0}"), this.intTotalMaxMAttackV * this.ZhuLingTimes);
				this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}最大物防{{-}} {0}"), this.intTotalMaxDefenseV * this.ZhuLingTimes);
				this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}最大魔防{{-}} {0}"), this.intTotalMaxMDefenseV * this.ZhuLingTimes);
				this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV * this.ZhuLingTimes);
				this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV * this.ZhuLingTimes);
				this.Attribute[6].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV * this.ZhuLingTimes);
			}
			else
			{
				this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}最大物攻{{-}} {0}"), this.intTotalMaxAttackV * this.ZhuLingTimes);
				this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}最大物防{{-}} {0}"), this.intTotalMaxDefenseV * this.ZhuLingTimes);
				this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}最大魔防{{-}} {0}"), this.intTotalMaxMDefenseV * this.ZhuLingTimes);
				this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV * this.ZhuLingTimes);
				this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV * this.ZhuLingTimes);
				this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV * this.ZhuLingTimes);
			}
		}
		else
		{
			this.Attribute[0].text = string.Format(Global.GetLang("{{c39550}}最大魔攻{{-}} {0}"), this.intTotalMaxMAttackV * this.ZhuLingTimes);
			this.Attribute[1].text = string.Format(Global.GetLang("{{c39550}}最大物防{{-}} {0}"), this.intTotalMaxDefenseV * this.ZhuLingTimes);
			this.Attribute[2].text = string.Format(Global.GetLang("{{c39550}}最大魔防{{-}} {0}"), this.intTotalMaxMDefenseV * this.ZhuLingTimes);
			this.Attribute[3].text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV * this.ZhuLingTimes);
			this.Attribute[4].text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV * this.ZhuLingTimes);
			this.Attribute[5].text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV * this.ZhuLingTimes);
		}
		if (this.ZhuLingTimes < this.ZhuLingTotalMaxTimes)
		{
			if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
			{
				for (int j = 0; j < this.Arrow.Length; j++)
				{
					this.Arrow[j].gameObject.SetActive(true);
				}
			}
			else
			{
				for (int k = 0; k < this.Arrow.Length - 1; k++)
				{
					this.Arrow[k].gameObject.SetActive(true);
				}
			}
			if (this.intTotalMaxAttackV != 0)
			{
				if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
				{
					this.IncreaseAttribute[0].text = this.intTotalMaxAttackV.ToString();
					this.IncreaseAttribute[1].text = this.intTotalMaxMAttackV.ToString();
					this.IncreaseAttribute[2].text = this.intTotalMaxDefenseV.ToString();
					this.IncreaseAttribute[3].text = this.intTotalMaxMDefenseV.ToString();
					this.IncreaseAttribute[4].text = this.intTotalHitVV.ToString();
					this.IncreaseAttribute[5].text = this.intTotalDodgeV.ToString();
					this.IncreaseAttribute[6].text = this.intTotalMaxLifeV.ToString();
				}
				else
				{
					this.IncreaseAttribute[0].text = this.intTotalMaxAttackV.ToString();
					this.IncreaseAttribute[1].text = this.intTotalMaxDefenseV.ToString();
					this.IncreaseAttribute[2].text = this.intTotalMaxMDefenseV.ToString();
					this.IncreaseAttribute[3].text = this.intTotalHitVV.ToString();
					this.IncreaseAttribute[4].text = this.intTotalDodgeV.ToString();
					this.IncreaseAttribute[5].text = this.intTotalMaxLifeV.ToString();
				}
			}
			else
			{
				this.IncreaseAttribute[0].text = this.intTotalMaxMAttackV.ToString();
				this.IncreaseAttribute[1].text = this.intTotalMaxDefenseV.ToString();
				this.IncreaseAttribute[2].text = this.intTotalMaxMDefenseV.ToString();
				this.IncreaseAttribute[3].text = this.intTotalHitVV.ToString();
				this.IncreaseAttribute[4].text = this.intTotalDodgeV.ToString();
				this.IncreaseAttribute[5].text = this.intTotalMaxLifeV.ToString();
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
		this.Zhuru.text = Global.GetLang("已注入") + ((float)this.ZhuLingTimes / (float)this.ZhuLingTotalMaxTimes * 100f).ToString() + "%";
		this.Water.GetComponent<Renderer>().material.SetFloat("_Mask", (float)this.ZhuLingTimes / (float)this.ZhuLingTotalMaxTimes);
		if (this.ZhuLingTimes >= this.ZhuLingMaxTimes)
		{
			this.ZhulingFull.gameObject.SetActive(true);
		}
		else
		{
			this.ZhulingFull.gameObject.SetActive(false);
		}
	}

	public UILabel StaticStr0;

	public UILabel StaticStr1;

	public UILabel StaticStr2;

	public UILabel StaticStr3;

	public UILabel StaticStr4;

	public UILabel Money;

	public GGoodIcon GoodIcon;

	public GButton Zhuhun;

	public GButton Close;

	public UILabel Zhuru;

	public UILabel[] Attribute;

	public UILabel[] IncreaseAttribute;

	public UISprite[] Arrow;

	public Transform Effect;

	public Transform Zhuling;

	public Transform ZhulingFull;

	public GameObject Water;

	public int ZhuLingTimes;

	private int TotalNum;

	private int BindingNum;

	private int NeedGoodID;

	private int NeedNum;

	private int NeedMoney;

	private int intTotalMaxAttackV;

	private int intTotalMaxMAttackV;

	private int intTotalMaxDefenseV;

	private int intTotalMaxMDefenseV;

	private int intTotalMaxLifeV;

	private int intTotalHitVV;

	private int intTotalDodgeV;

	private int ZhuLingMaxTimes;

	private int ZhuLingTotalMaxTimes;

	public DPSelectedItemEventHandler DPSelectedItem;
}
