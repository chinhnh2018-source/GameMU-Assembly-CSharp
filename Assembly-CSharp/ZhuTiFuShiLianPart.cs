using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuShiLianPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("立即前往");
		this.m_LabJiangLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("活动奖励")
		});
		this.m_ListOBC = this.m_ListBox.ItemsSource;
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
			int num = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
			if (num >= this.m_StarTime && num <= this.m_EndTime)
			{
				if (num >= this.m_StarTime && num <= this.m_HuoDongChiXu + this.m_StarTime)
				{
					MUDebug.Log<float>(new float[]
					{
						(float)(num - this.m_StarTime) / 60f
					});
					GameInstance.Game.SpriteEnterFuBen(Global.GetFuBenIdKeyMap(this.m_FuBenId));
					PlayZone.GlobalPlayZone.CloseZhuTiFuPartWindow();
				}
				else
				{
					Super.HintMainText(Global.GetLang("战斗已经开始无法进入副本"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("当前不在活动时间内"), 10, 3);
			}
		};
	}

	private void AddBangZhuPanel()
	{
		this.m_PanelBangZhu.gameObject.SetActive(false);
		XElement gameResXml = Global.GetGameResXml("Config/ThemeActivityIntroZhuanSheng.xml");
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

	public void SetData(string xmlList)
	{
		this.AddXmlThemeActivityZhuanSheng(xmlList);
		int num = Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 10000;
		foreach (KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair in this.m_DicThemeActivityZhuanSheng)
		{
			int num2 = keyValuePair.Value.MinLevel.Split(new char[]
			{
				'|'
			})[0].SafeToInt32(0) * 10000;
			Dictionary<int, ThemeActivityZhuanSheng>.Enumerator enumerator;
			KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair2 = enumerator.Current;
			int num3 = num2 + keyValuePair2.Value.MinLevel.Split(new char[]
			{
				'|'
			})[1].SafeToInt32(0);
			KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair3 = enumerator.Current;
			int num4 = keyValuePair3.Value.MaxLevel.Split(new char[]
			{
				'|'
			})[0].SafeToInt32(0) * 10000;
			KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair4 = enumerator.Current;
			int num5 = num4 + keyValuePair4.Value.MaxLevel.Split(new char[]
			{
				'|'
			})[1].SafeToInt32(0);
			if (num >= num3 && num <= num5)
			{
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair5 = enumerator.Current;
				this.m_HuoDongChiXu = keyValuePair5.Value.ReadyTime;
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair6 = enumerator.Current;
				this.m_FuBenId = keyValuePair6.Value.MapCode;
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair7 = enumerator.Current;
				this.m_TypeID = keyValuePair7.Value.ID;
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair8 = enumerator.Current;
				if (keyValuePair8.Value.TimePoints.Split(new char[]
				{
					'-'
				}).Length == 2)
				{
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair9 = enumerator.Current;
					int num6 = keyValuePair9.Value.TimePoints.Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[0].SafeToInt32(0) * 60 * 60;
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair10 = enumerator.Current;
					this.m_StarTime = num6 + keyValuePair10.Value.TimePoints.Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[1].SafeToInt32(0) * 60;
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair11 = enumerator.Current;
					int num7 = keyValuePair11.Value.TimePoints.Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[0].SafeToInt32(0) * 60 * 60;
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair12 = enumerator.Current;
					this.m_EndTime = num7 + keyValuePair12.Value.TimePoints.Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[1].SafeToInt32(0) * 60;
				}
				int num8 = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
				if (num8 >= this.m_StarTime && num8 <= this.m_EndTime)
				{
					this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("已开启")
					});
				}
				else
				{
					UILabel labTime = this.m_LabTime;
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("活动时间：")
					});
					object[] array = new object[2];
					array[0] = "dac7ae";
					int num9 = 1;
					string lang = Global.GetLang("每天：");
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair13 = enumerator.Current;
					array[num9] = lang + keyValuePair13.Value.TimePoints;
					labTime.text = colorStringForNGUIText + Global.GetColorStringForNGUIText(array);
				}
				string goodsIDs = string.Empty;
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair14 = enumerator.Current;
				goodsIDs = keyValuePair14.Value.GoodsList;
				Super.LoadGoodsList(goodsIDs, this.m_ListOBC);
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair15 = enumerator.Current;
				int mapCode = keyValuePair15.Value.MapCode;
				KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair16 = enumerator.Current;
				int monstersID = keyValuePair16.Value.MonstersID;
				if (ConfigMonsters.MonsterXmlNode.ContainsKey(monstersID))
				{
					this.m_LabName.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						Global.GetLang(ConfigMonsters.MonsterXmlNode[monstersID].SName)
					});
					mapCode = ConfigMonsters.MonsterXmlNode[monstersID].MapCode;
					Modal3DShow 3Dmodel = this.m_3Dmodel;
					int monsterResID = monstersID;
					KeyValuePair<int, ThemeActivityZhuanSheng> keyValuePair17 = enumerator.Current;
					MonsterNPCResLoader monsterNPCResLoader = UIHelper.LoadMonsterRes(3Dmodel, monsterResID, keyValuePair17.Value.Scale);
				}
				return;
			}
		}
	}

	private void AddXmlThemeActivityZhuanSheng(string xmlList)
	{
		XElement xelement = XElement.Parse(xmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityZhuanSheng");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityZhuanSheng themeActivityZhuanSheng = new ThemeActivityZhuanSheng();
			themeActivityZhuanSheng.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityZhuanSheng.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
			themeActivityZhuanSheng.MapCode = Global.GetXElementAttributeInt(xelementList[i], "MapID");
			themeActivityZhuanSheng.MinLevel = Global.GetXElementAttributeStr(xelementList[i], "MinLevel");
			themeActivityZhuanSheng.MaxLevel = Global.GetXElementAttributeStr(xelementList[i], "MaxLevel");
			themeActivityZhuanSheng.ZhanLi = Global.GetXElementAttributeInt(xelementList[i], "ZhanLi");
			themeActivityZhuanSheng.GoodsList = Global.GetXElementAttributeStr(xelementList[i], "GoodsList");
			themeActivityZhuanSheng.Scale = Global.GetXElementAttributeFloat(xelementList[i], "Scale");
			themeActivityZhuanSheng.TimePoints = Global.GetXElementAttributeStr(xelementList[i], "TimePoints");
			themeActivityZhuanSheng.ReadyTime = Global.GetXElementAttributeInt(xelementList[i], "ReadyTime");
			themeActivityZhuanSheng.FightingSecs = Global.GetXElementAttributeInt(xelementList[i], "FightingSecs");
			themeActivityZhuanSheng.ClearRolesSecs = Global.GetXElementAttributeInt(xelementList[i], "ClearRolesSecs");
			themeActivityZhuanSheng.MaxEnterNum = Global.GetXElementAttributeInt(xelementList[i], "MaxEnterNum");
			if (!this.m_DicThemeActivityZhuanSheng.ContainsKey(themeActivityZhuanSheng.ID))
			{
				this.m_DicThemeActivityZhuanSheng.Add(themeActivityZhuanSheng.ID, themeActivityZhuanSheng);
			}
		}
	}

	public UILabel m_LabName;

	public UILabel m_LabJiangLi;

	public UILabel m_LabTime;

	public GButton m_Btn;

	public ListBox m_ListBox;

	public Modal3DShow m_3Dmodel;

	public UIPanel m_PanelBangZhuList;

	private UIDraggablePanel mDragPanelContent;

	public UIFont m_Font;

	public UIPanel m_PanelBangZhu;

	public UILabel m_LabBangZhuContent;

	public UILabel m_LabBangZhuTitle;

	public GButton m_BtnBnagZhuOpen;

	public GButton m_BtnBnagZhuClose;

	private ObservableCollection m_ListOBC;

	private int m_TypeID = -1;

	private int m_StarTime = -1;

	private int m_EndTime = -1;

	private int m_FuBenId = -1;

	private int m_HuoDongChiXu;

	private Dictionary<int, ThemeActivityZhuanSheng> m_DicThemeActivityZhuanSheng = new Dictionary<int, ThemeActivityZhuanSheng>();

	private Dictionary<int, ZhuTiFuBossHorseIntro> m_DicIntro = new Dictionary<int, ZhuTiFuBossHorseIntro>();
}
