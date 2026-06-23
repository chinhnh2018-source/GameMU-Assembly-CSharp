using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhanMengShenDianPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Back.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang.png.qj";
		this.AcquiescentInit();
		this.Btn[0].Text = Global.GetLang("战盟神殿");
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.Btn[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ShenDianShouHu == null)
			{
				this.OpenShenDianShouHu();
				this.ChangeBtn(0);
			}
		};
		GameInstance.Game.SendShenDianInfo();
		Super.ShowNetWaiting(null);
	}

	public void analyzedShenDianServerInfo(UnionPalaceData data)
	{
		if (data == null)
		{
			return;
		}
		this.ZhanGongNum.text = data.ZhanGongLeft.ToString();
		if (this.ShenDianShouHu)
		{
			this.ShenDianShouHu.analyzedShenDianServerInfo(data);
		}
	}

	private void AcquiescentInit()
	{
		this.OpenShenDianShouHu();
		this.ChangeBtn(0);
	}

	private void ChangeBtn(int n)
	{
		for (int i = 0; i < this.Btn.Length; i++)
		{
			if (i == n)
			{
				this.Btn[i].Pressed = true;
				this.Btn[i].Label.color = NGUIMath.HexToColorEx(15790320U);
			}
			else
			{
				this.Btn[i].Pressed = false;
				this.Btn[i].Label.color = NGUIMath.HexToColorEx(10323559U);
			}
		}
	}

	private void OpenShenDianShouHu()
	{
		this.CloseAll();
		if (this.ShenDianShouHu == null)
		{
			this.ShenDianShouHu = U3DUtils.NEW<ZhanMengShenDianPartItem>();
			this.ShenDianShouHu.transform.parent = this.obj.transform;
			this.ShenDianShouHu.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private void CloseAll()
	{
		if (null != this.ShenDianShouHu)
		{
			this.ShenDianShouHu.transform.parent = null;
			Object.Destroy(this.ShenDianShouHu.gameObject);
			this.ShenDianShouHu = null;
		}
	}

	public static void ClearXMLData()
	{
		if (0 < ZhanMengShenDianPart.dicShenDianScale.Count)
		{
			ZhanMengShenDianPart.dicShenDianScale.Clear();
		}
		if (0 < ZhanMengShenDianPart.dicShenDianLevelUp.Count)
		{
			ZhanMengShenDianPart.dicShenDianLevelUp.Clear();
		}
		if (0 < ZhanMengShenDianPart.dicShenDianExtra.Count)
		{
			ZhanMengShenDianPart.dicShenDianExtra.Clear();
		}
		if (0 < ZhanMengShenDianPart.dicShenDianTab.Count)
		{
			ZhanMengShenDianPart.dicShenDianTab.Clear();
		}
	}

	public static Dictionary<int, ShenDianScale> GetDicShenDianScale()
	{
		if (ZhanMengShenDianPart.dicShenDianScale.Count > 0)
		{
			return ZhanMengShenDianPart.dicShenDianScale;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenDianScale.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenDian");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenDianScale shenDianScale = new ShenDianScale();
			shenDianScale.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			shenDianScale.Scale = Global.GetXElementAttributeStr(xelementList[i], "Scale");
			if (!ZhanMengShenDianPart.dicShenDianScale.ContainsKey(shenDianScale.Level))
			{
				ZhanMengShenDianPart.dicShenDianScale.Add(shenDianScale.Level, shenDianScale);
			}
			i++;
		}
		return ZhanMengShenDianPart.dicShenDianScale;
	}

	public static Dictionary<int, ShenDianLevelUp> GetDicShenDianLevelUp()
	{
		if (ZhanMengShenDianPart.dicShenDianLevelUp.Count > 0)
		{
			return ZhanMengShenDianPart.dicShenDianLevelUp;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenDianLevelUp.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenDian");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenDianLevelUp shenDianLevelUp = new ShenDianLevelUp();
			shenDianLevelUp.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			shenDianLevelUp.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			shenDianLevelUp.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			shenDianLevelUp.NeedZhanMengLevel = Global.GetXElementAttributeInt(xelementList[i], "NeedZhanMengLevel");
			shenDianLevelUp.NeedStatueLevel = Global.GetXElementAttributeStr(xelementList[i], "NeedStatueLevel");
			shenDianLevelUp.MaxLifeV = Global.GetXElementAttributeInt(xelementList[i], "MaxLifeV");
			shenDianLevelUp.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
			shenDianLevelUp.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
			shenDianLevelUp.AddAttackInjure = Global.GetXElementAttributeInt(xelementList[i], "AddAttackInjure");
			shenDianLevelUp.Effect = Global.GetXElementAttributeStr(xelementList[i], "Effect");
			shenDianLevelUp.Model = Global.GetXElementAttributeStr(xelementList[i], "Model");
			if (!ZhanMengShenDianPart.dicShenDianLevelUp.ContainsKey(shenDianLevelUp.ID))
			{
				ZhanMengShenDianPart.dicShenDianLevelUp.Add(shenDianLevelUp.ID, shenDianLevelUp);
			}
			i++;
		}
		return ZhanMengShenDianPart.dicShenDianLevelUp;
	}

	public static Dictionary<int, ShenDianExtra> GetDicShenDianExtra()
	{
		if (ZhanMengShenDianPart.dicShenDianExtra.Count > 0)
		{
			return ZhanMengShenDianPart.dicShenDianExtra;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenDianExtra.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenDian");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenDianExtra shenDianExtra = new ShenDianExtra();
			shenDianExtra.StatueLevel = Global.GetXElementAttributeInt(xelementList[i], "StatueLevel");
			shenDianExtra.ZhanMengLevel = Global.GetXElementAttributeInt(xelementList[i], "ZhanMengLevel");
			shenDianExtra.MaxLifePercent = Global.GetXElementAttributeDouble(xelementList[i], "MaxLifePercent");
			if (!ZhanMengShenDianPart.dicShenDianExtra.ContainsKey(shenDianExtra.StatueLevel))
			{
				ZhanMengShenDianPart.dicShenDianExtra.Add(shenDianExtra.StatueLevel, shenDianExtra);
			}
			i++;
		}
		return ZhanMengShenDianPart.dicShenDianExtra;
	}

	public static Dictionary<int, ShenDianTab> GetDicShenDianTab()
	{
		if (ZhanMengShenDianPart.dicShenDianTab.Count > 0)
		{
			return ZhanMengShenDianPart.dicShenDianTab;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenDianTab.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenDian");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenDianTab shenDianTab = new ShenDianTab();
			shenDianTab.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			shenDianTab.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			if (!ZhanMengShenDianPart.dicShenDianTab.ContainsKey(shenDianTab.Type))
			{
				ZhanMengShenDianPart.dicShenDianTab.Add(shenDianTab.Type, shenDianTab);
			}
			i++;
		}
		return ZhanMengShenDianPart.dicShenDianTab;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Close;

	public GButton[] Btn;

	public ShowNetImage Back;

	public UILabel ZhanGongNum;

	public GameObject obj;

	protected ZhanMengShenDianPartItem ShenDianShouHu;

	private static Dictionary<int, ShenDianScale> dicShenDianScale = new Dictionary<int, ShenDianScale>();

	private static Dictionary<int, ShenDianLevelUp> dicShenDianLevelUp = new Dictionary<int, ShenDianLevelUp>();

	private static Dictionary<int, ShenDianExtra> dicShenDianExtra = new Dictionary<int, ShenDianExtra>();

	private static Dictionary<int, ShenDianTab> dicShenDianTab = new Dictionary<int, ShenDianTab>();
}
