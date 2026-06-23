using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuBossPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("艾欧里亚")
		});
		this.m_LabDiaoLuo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("BOSS掉落")
		});
		this.m_ListOBC = this.m_ListBox.ItemsSource;
	}

	public void AddXmlThemeActivityBOSS(string xmlList)
	{
		XElement xelement = XElement.Parse(xmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityBOSS");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityBOSS themeActivityBOSS = new ThemeActivityBOSS();
			themeActivityBOSS.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityBOSS.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
			themeActivityBOSS.MaxLevel = Global.GetXElementAttributeStr(xelementList[i], "MaxLevel");
			themeActivityBOSS.ZhanLi = Global.GetXElementAttributeInt(xelementList[i], "ZhanLi");
			themeActivityBOSS.GoodsList = Global.GetXElementAttributeStr(xelementList[i], "GoodsList");
			themeActivityBOSS.Scale = Global.GetXElementAttributeFloat(xelementList[i], "Scale");
			themeActivityBOSS.MapCode = Global.GetXElementAttributeInt(xelementList[i], "MapCode");
			themeActivityBOSS.X = Global.GetXElementAttributeInt(xelementList[i], "X");
			themeActivityBOSS.Y = Global.GetXElementAttributeInt(xelementList[i], "Y");
			themeActivityBOSS.Radius = Global.GetXElementAttributeInt(xelementList[i], "Radius");
			themeActivityBOSS.Num = Global.GetXElementAttributeInt(xelementList[i], "Num");
			themeActivityBOSS.TimePoints = Global.GetXElementAttributeStr(xelementList[i], "TimePoints");
			if (!this.m_DicThemeActivityBOSS.ContainsKey(themeActivityBOSS.ID))
			{
				this.m_DicThemeActivityBOSS.Add(themeActivityBOSS.ID, themeActivityBOSS);
			}
		}
	}

	public void SetData(int actId, int bossState)
	{
		if (this.m_DicThemeActivityBOSS.ContainsKey(actId))
		{
			ThemeActivityBOSS data = this.m_DicThemeActivityBOSS[actId];
			if (bossState == 0)
			{
				this.m_LabZhuangTai.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("未刷新")
				});
			}
			else if (bossState == 1)
			{
				this.m_LabZhuangTai.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("已刷新")
				});
			}
			else if (bossState == 2)
			{
				this.m_LabZhuangTai.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("已死亡")
				});
			}
			this.m_LabTimeContent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("每天{0}"), data.TimePoints.Split(new char[]
				{
					'|'
				})[0]) + Environment.NewLine + data.TimePoints.Split(new char[]
				{
					'|'
				})[1]
			});
			this.m_LabDiTu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("所在地图：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				ConfigSettings.GetMapNameByCode(data.MapCode, false)
			});
			string goodsIDs = string.Empty;
			goodsIDs = data.GoodsList;
			Super.LoadGoodsList(goodsIDs, this.m_ListOBC);
			GGoodIcon[] componentsInChildren = this.m_ListBox.GetComponentsInChildren<GGoodIcon>();
			if (componentsInChildren != null)
			{
				this.m_ListBox.transform.localPosition = new Vector3(this.m_ListBox.transform.localPosition.x - this.m_ListBox.cellWidth / 2f * (float)(componentsInChildren.Length - 1), this.m_ListBox.transform.localPosition.y, -0.1f);
			}
			if (ConfigMonsters.MonsterXmlNode.ContainsKey(data.MonstersID))
			{
				this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang(ConfigMonsters.MonsterXmlNode[data.MonstersID].SName)
				});
				Point pt = new Point(data.X, data.Y);
				this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (Global.IsGoToKuaFuMap(data.MapCode))
					{
						PlayZone.GlobalPlayZone.OpenKuafuMapView(2, -1, data.MonstersID, data.MapCode, pt.X, pt.Y, false, 0, 0, false, false);
						return;
					}
					if (data.MapCode != -1)
					{
						Global.Data.GameScene.AutoFindRoad(data.MapCode, pt, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
						PlayZone.GlobalPlayZone.CloseZhuTiFuPartWindow();
					}
				};
			}
			if (this._BossModal != null)
			{
				if (!this._BossModal.IsTarget(this.m_DicThemeActivityBOSS[actId].MonstersID))
				{
					this._BossModal.Clear();
					this._BossModal.MonsterID = this.m_DicThemeActivityBOSS[actId].MonstersID;
					if (this.monsterNPCResLoader != null)
					{
						this.monsterNPCResLoader.Stop();
					}
					this.monsterNPCResLoader = UIHelper.LoadMonsterRes(this._BossModal, this.m_DicThemeActivityBOSS[actId].MonstersID, this.m_DicThemeActivityBOSS[actId].Scale);
				}
				UIHelper.SetModalPosZ(this._BossModal.transform);
			}
		}
	}

	public UILabel m_LabTitle;

	public UILabel m_LabDiTu;

	public UILabel m_LabZhuangTai;

	public UILabel m_LabTimeTitle;

	public UILabel m_LabTimeContent;

	public UILabel m_LabDiaoLuo;

	public Modal3DShow _BossModal;

	public GButton m_Btn;

	public ListBox m_ListBox;

	private MonsterNPCResLoader monsterNPCResLoader;

	private ObservableCollection m_ListOBC;

	private Dictionary<int, ThemeActivityBOSS> m_DicThemeActivityBOSS = new Dictionary<int, ThemeActivityBOSS>();
}
