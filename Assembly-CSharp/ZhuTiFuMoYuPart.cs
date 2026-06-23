using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuMoYuPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("立即前往");
		this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("BOSS掉落")
		});
		this.AddBangZhuPanel();
		this.m_BtnBnagZhuOpen.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_PanelBangZhu.gameObject.SetActive(true);
		};
		this.m_BtnBnagZhuClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_PanelBangZhu.gameObject.SetActive(false);
		};
		this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.Faction <= 0)
			{
				Super.HintMainText(string.Format("{0}", Global.GetLang("需加入战盟才能进入")), 10, 3);
				return;
			}
			Dictionary<int, ThemeActivityMoYu>.Enumerator enumerator = this.m_DicThemeActivityMoYu.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
				KeyValuePair<int, ThemeActivityMoYu> keyValuePair = enumerator.Current;
				XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(keyValuePair.Value.MapId, xmlName);
				if (gameMapSettingsXml == null)
				{
					object obj = "地图加载配置错误MapID:";
					KeyValuePair<int, ThemeActivityMoYu> keyValuePair2 = enumerator.Current;
					Super.HintMainText(Global.GetLang(obj + keyValuePair2.Value.MapId), 10, 3);
					return;
				}
				XElement xelement = XmlManager.GetXElement(gameMapSettingsXml, "Limits");
				int num = 0;
				int num2 = 0;
				if (xelement != null)
				{
					num = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					num2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				}
				int num3 = Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 10000;
				if (num3 < num * 10000 + num2)
				{
					Super.HintMainText(Global.GetLang("等级不足，无法进入魔域"), 10, 3);
				}
				KeyValuePair<int, ThemeActivityMoYu> keyValuePair3 = enumerator.Current;
				int monstersID = keyValuePair3.Value.MonstersID;
				KeyValuePair<int, ThemeActivityMoYu> keyValuePair4 = enumerator.Current;
				int mapId = keyValuePair4.Value.MapId;
				Point monsterPointByID = Global.GetMonsterPointByID(mapId, monstersID);
				KeyValuePair<int, ThemeActivityMoYu> keyValuePair5 = enumerator.Current;
				monsterPointByID.X = keyValuePair5.Value.X;
				KeyValuePair<int, ThemeActivityMoYu> keyValuePair6 = enumerator.Current;
				monsterPointByID.Y = keyValuePair6.Value.Y;
				if (mapId != -1)
				{
					GameInstance.Game.SpriteGotToMap(mapId);
					PlayZone.GlobalPlayZone.CloseZhuTiFuPartWindow();
				}
			}
		};
	}

	private void AddXmlThemeActivityMoYu(string xmlList)
	{
		XElement xelement = XElement.Parse(xmlList);
		Global.XmlMoYu = xmlList;
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityMoYu");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityMoYu themeActivityMoYu = new ThemeActivityMoYu();
			themeActivityMoYu.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityMoYu.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
			themeActivityMoYu.MapId = Global.GetXElementAttributeInt(xelementList[i], "MapId");
			themeActivityMoYu.X = Global.GetXElementAttributeInt(xelementList[i], "X");
			themeActivityMoYu.Y = Global.GetXElementAttributeInt(xelementList[i], "Y");
			themeActivityMoYu.Radius = Global.GetXElementAttributeInt(xelementList[i], "Radius");
			themeActivityMoYu.NpcID = Global.GetXElementAttributeInt(xelementList[i], "NpcID");
			themeActivityMoYu.ChengJiuAward = Global.GetXElementAttributeStr(xelementList[i], "ChengJiuAward");
			themeActivityMoYu.ShengWangAward = Global.GetXElementAttributeStr(xelementList[i], "ShengWangAward");
			themeActivityMoYu.Hurt = Global.GetXElementAttributeInt(xelementList[i], "Hurt");
			if (!this.m_DicThemeActivityMoYu.ContainsKey(themeActivityMoYu.ID))
			{
				this.m_DicThemeActivityMoYu.Add(themeActivityMoYu.ID, themeActivityMoYu);
			}
		}
	}

	public void SetXml(string xmlList)
	{
		this.AddXmlThemeActivityMoYu(xmlList);
	}

	public void SetData()
	{
		string text = string.Empty;
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ThemeActivityMoYuShowGoods", ',');
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			if (i < systemParamStringArrayByName.Length - 1)
			{
				text = text + systemParamStringArrayByName[i] + ",0,0,0,0,0,0|";
			}
			else
			{
				text = text + systemParamStringArrayByName[i] + ",0,0,0,0,0,0";
			}
		}
		this.m_ListOBC = this.m_ListGoods.ItemsSource;
		Super.LoadGoodsList(text, this.m_ListOBC);
	}

	private void AddBangZhuPanel()
	{
		this.m_PanelBangZhu.gameObject.SetActive(false);
		XElement gameResXml = Global.GetGameResXml("Config/ThemeActivityIntroMoYu.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZhuTiFuBossHorseIntro zhuTiFuBossHorseIntro = new ZhuTiFuBossHorseIntro();
			zhuTiFuBossHorseIntro.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			zhuTiFuBossHorseIntro.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
			zhuTiFuBossHorseIntro.Bold = Global.GetXElementAttributeInt(xelementList[i], "Bold");
			if (!this.m_DicIntro.ContainsKey(zhuTiFuBossHorseIntro.ID))
			{
				this.m_DicIntro.Add(zhuTiFuBossHorseIntro.ID, zhuTiFuBossHorseIntro);
			}
		}
		Dictionary<int, ZhuTiFuBossHorseIntro>.Enumerator enumerator = this.m_DicIntro.GetEnumerator();
		float num = 0f;
		bool flag = true;
		while (enumerator.MoveNext())
		{
			if (flag)
			{
				flag = false;
				UILabel labBangZhuTitle = this.m_LabBangZhuTitle;
				KeyValuePair<int, ZhuTiFuBossHorseIntro> keyValuePair = enumerator.Current;
				labBangZhuTitle.text = Global.GetLang(keyValuePair.Value.Intro);
				KeyValuePair<int, ZhuTiFuBossHorseIntro> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.Bold == 1)
				{
					this.m_LabBangZhuTitle.renderStyle = 1;
				}
				else
				{
					UILabel labBangZhuTitle2 = this.m_LabBangZhuTitle;
					KeyValuePair<int, ZhuTiFuBossHorseIntro> keyValuePair3 = enumerator.Current;
					labBangZhuTitle2.text = Global.GetLang(keyValuePair3.Value.Intro);
				}
			}
			else
			{
				UILabel uilabel = NGUITools.AddWidget<UILabel>(this.m_PanelBangZhuList.gameObject);
				uilabel.font = this.m_Font;
				uilabel.lineWidth = 525;
				uilabel.pivot = 0;
				uilabel.transform.localScale = Vector3.one * 18f;
				uilabel.transform.localPosition = new Vector3(0f, num, -0.001f);
				uilabel.color = new Color(218f, 199f, 174f);
				UILabel uilabel2 = uilabel;
				KeyValuePair<int, ZhuTiFuBossHorseIntro> keyValuePair4 = enumerator.Current;
				uilabel2.renderStyle = ((keyValuePair4.Value.Bold != 1) ? 0 : 1);
				TextBlock textBlock = uilabel.gameObject.AddComponent<TextBlock>();
				textBlock.Label = uilabel;
				textBlock._CharMargin = new Vector2(0f, 16f);
				TextBlock textBlock2 = textBlock;
				KeyValuePair<int, ZhuTiFuBossHorseIntro> keyValuePair5 = enumerator.Current;
				textBlock2.Text = Global.GetLang(keyValuePair5.Value.Intro);
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(uilabel.transform);
				num -= uilabel.relativeSize.y * uilabel.transform.localScale.y;
			}
		}
		BoxCollider boxCollider = this.m_PanelBangZhuList.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = this.m_PanelBangZhuList.gameObject.AddComponent<BoxCollider>();
		}
		UIEventListener.Get(this.m_PanelBangZhuList.gameObject);
		UIDragPanelContents uidragPanelContents = this.m_PanelBangZhuList.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = this.m_PanelBangZhuList.gameObject.AddComponent<UIDragPanelContents>();
		}
		uidragPanelContents.draggablePanel = this.mDragPanelContent;
	}

	public UILabel m_LabTitle;

	public ListBox m_ListGoods;

	public GButton m_Btn;

	private Dictionary<int, ThemeActivityMoYu> m_DicThemeActivityMoYu = new Dictionary<int, ThemeActivityMoYu>();

	public UIPanel m_PanelBangZhuList;

	private UIDraggablePanel mDragPanelContent;

	public UIFont m_Font;

	public UIPanel m_PanelBangZhu;

	public UILabel m_LabBangZhuContent;

	public UILabel m_LabBangZhuTitle;

	public GButton m_BtnBnagZhuOpen;

	public GButton m_BtnBnagZhuClose;

	private ObservableCollection m_ListOBC;

	private Dictionary<int, ZhuTiFuBossHorseIntro> m_DicIntro = new Dictionary<int, ZhuTiFuBossHorseIntro>();
}
