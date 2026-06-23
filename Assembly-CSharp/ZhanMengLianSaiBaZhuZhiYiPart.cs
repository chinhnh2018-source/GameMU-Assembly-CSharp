using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhanMengLianSaiBaZhuZhiYiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
		int num = ConfigSystemParam.GetSystemParamByName("LeagueWing", true).SafeToInt32(0);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (Global.GetXElementAttributeInt(xelement, "Goods") == num)
			{
				this.wingGoodsId = Global.GetXElementAttributeInt(xelement, "ID");
				this.tabID = Global.GetXElementAttributeInt(xelement, "Tab");
				break;
			}
		}
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(num);
		string baseAttributeStr = this.GetBaseAttributeStr(Global.GetEmptyGoodsData(num, 1, 1, 1, 1, 0, 0, 0, 0), goodsEquipPropsDoubleList, -1);
		this.mLabelAtt.text = baseAttributeStr;
		this.mLabelAtt._CharMargin.y = 8f;
		BoxCollider componentInParent = this.mLabelAtt.transform.GetComponentInParent<BoxCollider>();
		if (null != componentInParent)
		{
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.mLabelAtt.transform);
			componentInParent.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
			componentInParent.center = new Vector3(0f, bounds.size.y / 2f, 0f);
		}
	}

	private void InitPrefabText()
	{
		try
		{
			string goodsNameByID = Global.GetGoodsNameByID(ConfigSystemParam.GetSystemParamByName("LeagueWing", true).SafeToInt32(0), false);
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				goodsNameByID
			});
			bool flag = false;
			int num = ConfigSystemParam.GetSystemParamByName("LeagueWing", true).SafeToInt32(0);
			if (Global.Data.fashionAndTitleList != null)
			{
				for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
				{
					if (Global.Data.fashionAndTitleList[i] != null && num == Global.Data.fashionAndTitleList[i].GoodsID)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				this.mBtnLoad.gameObject.SetActive(true);
				if (this.wingGoodsId == Global.Data.roleData.RoleCommonUseIntPamams[26])
				{
					this.mBtnLoad.Label.text = Global.GetLang("卸下");
				}
				else
				{
					this.mBtnLoad.Label.text = Global.GetLang("佩戴");
				}
			}
			else
			{
				this.mBtnLoad.gameObject.SetActive(false);
			}
		}
		catch (Exception ex)
		{
		}
	}

	private string GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, int categoriy = -1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (0 <= gd.ExcellenceInfo)
		{
			double num = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd.ExcellenceInfo);
			double num2 = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd.ExcellenceInfo);
			for (int i = 1; i <= 10; i += 2)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				if (i == 1)
				{
					if (equipFields_1[i] != 0.0)
					{
						text = text + Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							text2 + Global.GetLang("：")
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							(int)equipFields_1[i] + "% "
						});
						text += "\n";
					}
				}
				else
				{
					int num3 = i;
					int num4 = i + 1;
					if (equipFields_1[num3] != 0.0 || equipFields_1[num4] != 0.0)
					{
						double num5 = equipFields_1[num3];
						double num6 = equipFields_1[num4];
						if (i == 3 || i == 5)
						{
							num5 += num2 * num5;
							num6 += num2 * num6;
						}
						else if (i == 7 || i == 9)
						{
							num5 += num * num5;
							num6 += num * num6;
						}
						double equipForgeAddBaseValue = Global.GetEquipForgeAddBaseValue(gd, num4);
						double num7 = Math.Max(num5 * equipForgeAddBaseValue, 3.0);
						double num8 = Math.Max(num6 * equipForgeAddBaseValue, 3.0);
						text = text + Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							text2 + Global.GetLang("：")
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							(int)num5 + " - " + (int)num6
						});
						text += "\n";
					}
				}
			}
		}
		for (int i = 11; i < ConfigExtPropIndexes.GetExtPropIndexesVOByWord("MAX").ID; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false);
				double num9 = equipFields_1[i];
				double equipForgeAddBaseValue2 = Global.GetEquipForgeAddBaseValue(gd, i);
				if (ConfigExtPropIndexes.GetPercentByID(i))
				{
					text = text + Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text2 + Global.GetLang("：")
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(int)(num9 * 100.0) + "% "
					});
				}
				else
				{
					text = text + Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text2 + Global.GetLang("：")
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(int)num9
					});
				}
				text += "\n";
			}
		}
		return this.ProcessStr(text);
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private void InitTexture()
	{
		try
		{
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this.mBtnLoad.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					if (this.wingGoodsId == Global.Data.roleData.RoleCommonUseIntPamams[26])
					{
						GameInstance.Game.UploadLuoLanWing(this.tabID, this.wingGoodsId, 2);
						this.Hander(this, new DPSelectedItemEventArgs
						{
							Type = 1,
							ID = 1
						});
					}
					else
					{
						if (Global.Data.roleData.MyWingData.Using == 0)
						{
							string goodsNameByID = Global.GetGoodsNameByID(ConfigSystemParam.GetSystemParamByName("LeagueWing", true).SafeToInt32(0), false);
							Super.HintMainText(Global.GetLang("需要佩戴翅膀才能佩戴") + goodsNameByID, 10, 3);
							return;
						}
						GameInstance.Game.UploadLuoLanWing(this.tabID, this.wingGoodsId, 1);
						this.Hander(this, new DPSelectedItemEventArgs
						{
							Type = 1,
							ID = 2
						});
					}
				}
			};
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
	}

	[SerializeField]
	private TextBlock mLabelAtt;

	[SerializeField]
	private GButton mBtnLoad;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UILabel mTitleLabel;

	private int wingGoodsId;

	private int tabID;

	public DPSelectedItemEventHandler Hander;
}
