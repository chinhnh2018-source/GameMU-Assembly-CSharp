using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TaoZhuangZhanShiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnGoto.Text = Global.GetLang("获得途径");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.equipIcon[11] = this.wuqizuoIcon;
		this.equipIcon[36] = this.wuqiyouIcon;
		this.equipIcon[0] = this.toukuiIcon;
		this.equipIcon[1] = this.kaijiaIcon;
		this.equipIcon[2] = this.hushouIcon;
		this.equipIcon[3] = this.hutuiIcon;
		this.equipIcon[4] = this.xueziIcon;
		this.equipIcon[5] = this.xianglianIcon;
		this.equipIcon[6] = this.jiezhizuoIcon;
		this.equipIcon[31] = this.jiezhiyouIcon;
		this.equipIcon[7] = this.zuoJiIcon;
		this.equipIcon[8] = this.chibangIcon;
		this.equipIcon[9] = this.shouhuchongIcon;
		this.equipIcon[22] = this.hufuIcon;
		if (null != this.m_BtnGoto)
		{
			this.m_BtnGoto.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!string.IsNullOrEmpty(this.LinkIDs))
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedCustom, this.callback, this.LinkIDs, this.TaoZhuangName);
				}
			};
		}
		this.TaoZhuangXML = Global.GetGameResXml("TaoZhuangZhanShi");
		List<XElement> xelementList = Global.GetXElementList(this.TaoZhuangXML, "ZhanShi");
		this.TaoZhuangList = this.GetDataListByOccupation(xelementList);
		this.TabFileList = new List<string>();
		int count = this.TaoZhuangList.Count;
		for (int i = 0; i < count; i++)
		{
			this.TabFileList.Add(string.Format(Global.GetLang("{0}阶套装"), this.TaoZhuangList[i].GetXElementAttrStr("SuitID")));
		}
		this.InitBtnProc();
		this.SetTabBtnIndex(0);
		UIHelper.SetModalPosZ(this.m_BossModel.transform);
	}

	private List<XElement> GetDataListByOccupation(List<XElement> taoZhuangListTotal)
	{
		List<XElement> list = new List<XElement>();
		int count = taoZhuangListTotal.Count;
		int occupation = Global.Data.roleData.Occupation;
		MJSSkillType mjstypeByAttr = Global.GetMJSTypeByAttr();
		for (int i = 0; i < count; i++)
		{
			XElement xelement = taoZhuangListTotal[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Occupation");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				if (occupation == Global.SafeConvertToInt32(array[0]) && mjstypeByAttr == (MJSSkillType)Global.SafeConvertToInt32(array[1]))
				{
					list.Add(xelement);
				}
			}
			else if (occupation == Global.SafeConvertToInt32(array[0]))
			{
				list.Add(xelement);
			}
		}
		return list;
	}

	private XElement GetXElementBySuidID(int suitID)
	{
		XElement result = null;
		int count = this.TaoZhuangList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.TaoZhuangList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "SuitID");
			if (suitID == xelementAttributeInt)
			{
				result = xelement;
				break;
			}
		}
		return result;
	}

	private void InitBtnProc()
	{
		this.m_TabBtnOBC = this.m_ListTabBtn.ItemsSource;
		int count = this.TabFileList.Count;
		for (int i = 0; i < count; i++)
		{
			string text = this.TabFileList[i];
			BtnObjForBianQiang btnObjForBianQiang = U3DUtils.NEW<BtnObjForBianQiang>();
			btnObjForBianQiang.m_BtnItem.Label.text = text;
			this.m_TabBtnOBC.AddNoUpdate(btnObjForBianQiang);
		}
		this.m_TabBtnOBC.DelayUpdate();
		this.m_ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnSelectChange);
	}

	private void TabBtnSelectChange(object sender, object e)
	{
		if (this.m_ListTabBtn.SelectedIndex >= 0)
		{
			this.SetTabBtnIndex(this.m_ListTabBtn.SelectedIndex);
		}
	}

	private void SetTabBtnIndex(int nIndex)
	{
		int xelementAttributeInt = Global.GetXElementAttributeInt(this.TaoZhuangList[nIndex], "SuitID");
		XElement xelementBySuidID = this.GetXElementBySuidID(xelementAttributeInt);
		if (xelementBySuidID == null)
		{
			return;
		}
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementBySuidID, "MinZhuanSheng");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementBySuidID, "MinLevel");
		if (xelementAttributeInt2 * 100 + xelementAttributeInt3 > Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level)
		{
			Super.HintMainText(string.Format(Global.GetLang("需要等级达到{0}转{1}级后可查看"), xelementAttributeInt2, xelementAttributeInt3), 10, 3);
			return;
		}
		this.m_nTabBtnIndex = nIndex;
		GameObject at = this.m_TabBtnOBC.GetAt(nIndex);
		BtnObjForBianQiang component = at.GetComponent<BtnObjForBianQiang>();
		this.SetBtnActieve(component.m_BtnItem);
		this.ResetGoodsListIcon(xelementAttributeInt);
		this.Load3DModel();
		this.m_LabelGainDes.text = string.Format(Global.GetLang("产出:{0}"), this.GainDes);
		this.zhandouli.text = this.TaoZhuangZhanLi.ToString();
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
	}

	private void Load3DModel()
	{
		if (null != this.m_BossModel)
		{
			this.m_BossModel.Clear();
		}
		int occupation = Global.Data.roleData.Occupation;
		WingData wingData = null;
		if (this.WingID != -1)
		{
			wingData = new WingData();
			wingData.WingID = this.WingID;
			wingData.Using = 1;
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		this.roleResLoader = UIHelper.LoadRoleRes(this.m_BossModel, 0L, occupation, 0, string.Empty, this.usingGoodsList, null, wingData, 1.7f, 0, null, false);
	}

	public void SetBtnActieve(GButton btn)
	{
		if (null != btn)
		{
			if (btn == this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15790320U);
				return;
			}
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnLastSelect = this.m_BtnCurrentSelect;
				this.m_BtnCurrentSelect = btn;
			}
			this.m_BtnCurrentSelect = btn;
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15790320U);
				this.m_BtnCurrentSelect.normalSprite = "btn_left_selected";
				this.m_BtnCurrentSelect.Refresh();
			}
			if (null != this.m_BtnLastSelect)
			{
				this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(10323559U);
				this.m_BtnLastSelect.normalSprite = "btn_left_normal";
				this.m_BtnLastSelect.Refresh();
			}
		}
	}

	public void ResetGoodsListIcon(int suitID)
	{
		XElement xelementBySuidID = this.GetXElementBySuidID(suitID);
		this.WingID = Global.GetXElementAttributeInt(xelementBySuidID, "Wing");
		this.GainDes = Global.GetXElementAttributeStr(xelementBySuidID, "Origin");
		this.LinkIDs = Global.GetXElementAttributeStr(xelementBySuidID, "Avenue");
		this.TaoZhuangName = Global.GetXElementAttributeStr(xelementBySuidID, "Name");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelementBySuidID, "Goods");
		this.ResetGoodsList(xelementAttributeStr);
		for (int i = 0; i < 25; i++)
		{
			this.SetEquipIcon(i);
		}
	}

	private void ResetGoodsList(string goodStr)
	{
		this.TaoZhuangZhanLi = 0.0;
		this.usingGoodsList.Clear();
		string[] array = goodStr.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				ItemCategories categoriyByGoodsID = (ItemCategories)Global.GetCategoriyByGoodsID(Convert.ToInt32(array2[0]));
				if (categoriyByGoodsID != ItemCategories.Decoration)
				{
					int bagIndex = 0;
					if (i == 9 || i == 6)
					{
						bagIndex = 1;
					}
					this.AddGoodsData(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[6]), bagIndex);
				}
			}
		}
	}

	private void AddGoodsData(int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0, int bagIndex = 0)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			dummyGoodsDataMu.Using = 1;
			dummyGoodsDataMu.BagIndex = bagIndex;
			this.usingGoodsList.Add(dummyGoodsDataMu);
			this.TaoZhuangZhanLi += Global.GetGoodsDataZhanLi(dummyGoodsDataMu);
		}
	}

	private void SetEquipIcon(int equipCategory)
	{
		if (this.usingGoodsList == null)
		{
			return;
		}
		int count = this.usingGoodsList.Count;
		for (int i = 0; i < count; i++)
		{
			GoodsData goodsData = this.usingGoodsList[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (equipCategory == categoriy)
			{
				GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
				icon.isAutoSize = true;
				icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
				{
					Super.GetIconCode(goodsXmlNodeByID)
				}), false, 0);
				icon.TipType = 1;
				icon.ItemCategory = goodsXmlNodeByID.Categoriy;
				icon.ItemCode = goodsData.GoodsID;
				icon.ItemObject = goodsData;
				icon.BoxTypes = 0;
				icon.TextSize = 20;
				icon.Tag = goodsData.ExcellenceInfo;
				this.InitGoodIconSize(icon, icon.ItemCategory);
				if (categoriy == 9 || categoriy == 10)
				{
					if (goodsData.ExcellenceInfo != 0)
					{
						icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
						icon.TeXiao.gameObject.SetActive(true);
					}
					else if (goodsXmlNodeByID.SuitID == 1)
					{
						icon.BackSpriteName1 = "iconState_zuoyue1";
					}
					else
					{
						icon.BackSpriteName1 = "iconState_zuoyue2";
					}
				}
				else if (goodsData.ExcellenceInfo > 0 || categoriy == 22)
				{
					this.SetExcellenceStat(icon, categoriy);
				}
				this.SetEquipBorderBySuitID(icon, goodsData);
				icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
				{
					this.SetBoxCollider(icon);
				};
				int handType = goodsXmlNodeByID.HandType;
				this.SetZhuangBeiPeiDai(icon, equipCategory, handType, goodsData.BagIndex);
			}
		}
	}

	public void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBagNoMenu, goodsData);
	}

	public void SetZhuangBeiPeiDai(GGoodIcon icon, int categoriy, int handType, int iBagIndex)
	{
		if (categoriy >= 11 && categoriy <= 21)
		{
			int num = 11;
			int num2 = 36;
			if (handType == 1)
			{
				if (this.equipIcon[num].Count() > 0)
				{
					this.equipIcon[num].RemoveAt(0, true, false);
				}
				this.equipIcon[num].Add(icon);
			}
			else if (handType == 0)
			{
				if (this.equipIcon[num2].Count() > 0)
				{
					this.equipIcon[num2].RemoveAt(0, true, false);
				}
				this.equipIcon[num2].Add(icon);
			}
			else if (handType == 2)
			{
				if (iBagIndex == 0)
				{
					if (this.equipIcon[num].Count() > 0)
					{
						this.equipIcon[num].RemoveAt(0, true, false);
					}
					this.equipIcon[num].Add(icon);
				}
				else if (iBagIndex == 1)
				{
					if (this.equipIcon[num2].Count() > 0)
					{
						this.equipIcon[num2].RemoveAt(0, true, false);
					}
					this.equipIcon[num2].Add(icon);
				}
			}
		}
		else if (categoriy == 6)
		{
			if (iBagIndex == 0)
			{
				if (this.equipIcon[categoriy].Count() > 0)
				{
					this.equipIcon[categoriy].RemoveAt(0, true, false);
				}
				this.equipIcon[categoriy].Add(icon);
			}
			else if (iBagIndex == 1)
			{
				if (this.equipIcon[25 + categoriy].Count() > 0)
				{
					this.equipIcon[25 + categoriy].RemoveAt(0, true, false);
				}
				this.equipIcon[25 + categoriy].Add(icon);
			}
		}
		else if (categoriy == 9 || categoriy == 10)
		{
			if (this.equipIcon[9].Count() > 0)
			{
				this.equipIcon[9].RemoveAt(0, true, false);
			}
			this.equipIcon[9].Add(icon);
		}
		else
		{
			if (this.equipIcon[categoriy].Count() > 0)
			{
				this.equipIcon[categoriy].RemoveAt(0, true, false);
			}
			this.equipIcon[categoriy].Add(icon);
		}
	}

	public void SetBoxCollider(GGoodIcon icon)
	{
		bool flag = Global.CanUseGoodsAttr(icon.ItemCode, false);
		if ((icon.ItemCategory >= 11 && icon.ItemCategory <= 36 && icon.ItemCategory != 22) || icon.ItemCategory == 1)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(83f, 129f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(83f, 129f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 0 || icon.ItemCategory == 2 || icon.ItemCategory == 3 || icon.ItemCategory == 4 || icon.ItemCategory == 22 || icon.ItemCategory == 7)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(80f, 80f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(80f, 80f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 5 || icon.ItemCategory == 9 || icon.ItemCategory == 6 || icon.ItemCategory == 31)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(53f, 53f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(53f, 53f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 8)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(115f, 78f, 0f);
		}
		Super.InitEquipGIcon(icon, icon.ItemObject as GoodsData, true, IconTextTypes.Qianghua);
		icon.ContentText.Pivot = 2;
		icon.ContentText.X = (double)(icon.GetComponent<BoxCollider>().size.x / 2f);
		icon.ContentText.Y = (double)(icon.GetComponent<BoxCollider>().size.y / 2f);
	}

	public void InitGoodIconSize(GGoodIcon icon, int iCategoriy)
	{
		int num = 64;
		int num2 = 64;
		if ((iCategoriy >= 11 && iCategoriy <= 21) || iCategoriy == 1)
		{
			num = 83;
			num2 = 129;
		}
		else if (iCategoriy == 8)
		{
			num = 115;
			num2 = 78;
		}
		else if (iCategoriy == 6 || iCategoriy == 5 || iCategoriy == 9 || iCategoriy == 10)
		{
			num = 53;
			num2 = 53;
		}
		else if (iCategoriy == 7 || iCategoriy == 0 || iCategoriy == 2 || iCategoriy == 4 || iCategoriy == 3 || iCategoriy == 22)
		{
			num = 80;
			num2 = 80;
		}
		icon.Width = (double)num;
		icon.Height = (double)num2;
	}

	public void SetExcellenceStat(GGoodIcon icon, int iCategoriy)
	{
		if ((iCategoriy >= 11 && iCategoriy <= 21) || iCategoriy == 1)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(83f, 129f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xiongjia");
		}
		else if (iCategoriy == 8)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(115f, 78f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_chibang");
		}
		else if (iCategoriy == 6 || iCategoriy == 5 || iCategoriy == 9 || iCategoriy == 10)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(53f, 53f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xianglian");
		}
		else if (iCategoriy == 7 || iCategoriy == 0 || iCategoriy == 2 || iCategoriy == 4 || iCategoriy == 3 || iCategoriy == 22)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(80f, 80f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_toukui");
		}
		icon.BackgroundSprite1.gameObject.SetActive(true);
	}

	private void SetExcellence(GGoodIcon icon, string zhuoyueTeXiaoPrefab)
	{
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		if (goodsData.ExcellenceInfo > 0)
		{
			if (Global.GetZhuoyueAttributeCount(goodsData) >= 6)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) < 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) >= 3 && Global.GetZhuoyueAttributeCount(goodsData) < 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) == 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
		}
		else if (icon.ItemCategory == 22 || icon.ItemCategory == 9)
		{
			int goodsQuality = Super.GetGoodsQuality(goodsData.GoodsID);
			if (goodsQuality == 1)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (goodsQuality == 2)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (goodsQuality == 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
			else if (goodsQuality == 4)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (goodsQuality == 6)
			{
			}
		}
		icon.BindingSprite.gameObject.SetActive(goodsData.Binding > 0);
		Vector3 localScale = icon.BackgroundSprite1.transform.localScale;
		icon.BindingSprite.transform.localPosition = this.Pos(localScale, -(localScale.x / 2f - 12f), -(localScale.y / 2f - 12f), -0.03f);
	}

	private void SetEquipBorderBySuitID(GGoodIcon icon, GoodsData goodsData)
	{
		if (null == icon)
		{
			return;
		}
		if (goodsData == null)
		{
			return;
		}
		if (!Global.IsShengqi(goodsData))
		{
			return;
		}
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		Transform transform = icon.BackgroundSprite15.transform;
		if ((categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21) || categoriyByGoodsID == 1)
		{
			transform.localScale = new Vector3(93f, 139f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 8)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(125f, 88f);
			transform.localPosition = new Vector3(0.5f, 0f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 6 || categoriyByGoodsID == 5 || categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(63f, 63f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 7 || categoriyByGoodsID == 0 || categoriyByGoodsID == 2 || categoriyByGoodsID == 4 || categoriyByGoodsID == 3 || categoriyByGoodsID == 22)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(90f, 90f);
		}
		Vector3 vector = icon.BackgroundSprite15.transform.localScale;
		vector += new Vector3(2f, 2f, 0f);
		icon.BackSpriteName15 = "iconStateGold";
		icon.BackgroundSprite15.transform.localScale = vector;
	}

	private Vector3 Pos(Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		return v;
	}

	public GGoodIcon zuoJiIcon;

	public GGoodIcon toukuiIcon;

	public GGoodIcon chibangIcon;

	public GGoodIcon wuqizuoIcon;

	public GGoodIcon wuqiyouIcon;

	public GGoodIcon xianglianIcon;

	public GGoodIcon kaijiaIcon;

	public GGoodIcon shouhuchongIcon;

	public GGoodIcon hushouIcon;

	public GGoodIcon jiezhizuoIcon;

	public GGoodIcon jiezhiyouIcon;

	public GGoodIcon hutuiIcon;

	public GGoodIcon xueziIcon;

	public GGoodIcon hufuIcon;

	public TextBlock zhandouli;

	public Dictionary<int, GGoodIcon> equipIcon = new Dictionary<int, GGoodIcon>();

	private List<GoodsData> usingGoodsList = new List<GoodsData>();

	private int WingID = -1;

	public GButton m_BtnLastSelect;

	public GButton m_BtnCurrentSelect;

	public int m_nTabBtnIndex;

	public GameObject m_Btn;

	public ListBox m_ListTabBtn = new ListBox();

	private List<string> TabFileList;

	private ObservableCollection m_TabBtnOBC;

	private XElement TaoZhuangXML;

	private List<XElement> TaoZhuangList;

	public Modal3DShow m_BossModel;

	private double TaoZhuangZhanLi;

	private string GainDes = string.Empty;

	private string LinkIDs = string.Empty;

	private string TaoZhuangName = string.Empty;

	public TextBlock m_LabelGainDes;

	public GButton m_BtnGoto;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	private RoleResLoader roleResLoader;
}
