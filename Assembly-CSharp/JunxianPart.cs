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

public class JunxianPart : JingjiPlayerInfoBase
{
	private void InitTextInPrefabs()
	{
		this.m_TishengBtn.Text = Global.GetLang("提升军衔");
		this.mTips.Text = Global.GetLang("升级军衔需求：");
		if (this.m_ConstAttr1 != null && this.m_ConstAttr1.Length == 7)
		{
			this.m_ConstAttr1[0].Text = Global.GetLang("物理攻击:");
			this.m_ConstAttr1[1].Text = Global.GetLang("魔法攻击:");
			this.m_ConstAttr1[2].Text = Global.GetLang("物理防御:");
			this.m_ConstAttr1[3].Text = Global.GetLang("魔法防御:");
			this.m_ConstAttr1[4].Text = Global.GetLang("生命上限:");
			this.m_ConstAttr1[5].Text = Global.GetLang("伤害加成:");
			this.m_ConstAttr1[6].Text = Global.GetLang("命       中:");
		}
		if (this.m_ConstAttr2 != null && this.m_ConstAttr2.Length == 7)
		{
			this.m_ConstAttr2[0].Text = Global.GetLang("物理攻击:");
			this.m_ConstAttr2[1].Text = Global.GetLang("魔法攻击:");
			this.m_ConstAttr2[2].Text = Global.GetLang("物理防御:");
			this.m_ConstAttr2[3].Text = Global.GetLang("魔法防御:");
			this.m_ConstAttr2[4].Text = Global.GetLang("生命上限:");
			this.m_ConstAttr2[5].Text = Global.GetLang("伤害加成:");
			this.m_ConstAttr2[6].Text = Global.GetLang("命       中:");
		}
		this.m_JunxianLabel.X = -340.0;
		this.m_RankingLabel.X = -350.0;
		this.m_LianshengLabel.X = -340.0;
		this.m_ShengwangLabel.X = -340.0;
		this.m_ShengjishengwangLabel.X = 335.0;
		for (int i = 0; i <= 6; i++)
		{
			this.m_ConstAttr1[i].Pivot = 5;
			this.m_ConstAttr2[i].Pivot = 5;
			this.m_ConstAttr1[i].X = -12.0;
			this.m_ConstAttr2[i].X = -12.0;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (this.GoodIconObj != null)
		{
			this.GoodIconObj.gameObject.SetActive(false);
		}
		this.m_ListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.m_TishengBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsJunxianMaxLevel(this.m_JunxianLevel))
			{
				string lang = Global.GetLang("已经达到最大军衔级别！");
				string[] buttons = new string[]
				{
					Global.GetLang("确定")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
				{
				}, buttons);
			}
			else
			{
				Global.SendEvent("1401", Global.GetLang("军衔提升次数"));
				if (this.NeedGoodsID > 0)
				{
					if (this.IsGoodsEnough())
					{
						GameInstance.Game.SpriteGetJingJiJunxianLevelupCmd();
					}
					else
					{
						Super.HintMainText(Global.GetLang("物品不足"), 10, 3);
					}
				}
				else
				{
					GameInstance.Game.SpriteGetJingJiJunxianLevelupCmd();
				}
			}
		};
		this.m_FutiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			XElement elementByLevel = base.getElementByLevel(this.m_JunxianLevel);
			int xelementAttributeInt = Global.GetXElementAttributeInt(elementByLevel, "XiaoHaoShengWang");
			if (this.m_Shengwang > xelementAttributeInt)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(87);
				if (bufferDataByID == null || Global.IsBufferDataOver(bufferDataByID, 0L, false))
				{
					GameInstance.Game.SpriteGetJingJiRankingBuffCmd(false);
				}
				else
				{
					string lang = Global.GetLang("当前人物身上已激活军衔BUFF，是否要覆盖当前BUFF");
					string[] buttons = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
					{
						if (e1.ID == 0)
						{
							GameInstance.Game.SpriteGetJingJiRankingBuffCmd(true);
						}
					}, buttons);
				}
			}
			else
			{
				string lang2 = Global.GetLang("声望不足，无法完成操作！");
				string[] buttons2 = new string[]
				{
					Global.GetLang("确定")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, delegate(object s1, DPSelectedItemEventArgs e1)
				{
				}, buttons2);
			}
		};
		this.agendaBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.openAdendaViewEventHandler != null)
			{
				this.openAdendaViewEventHandler(this, new DPSelectedItemEventArgs
				{
					Type = 1
				});
			}
		};
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		if (roleCommonUseParamsValue >= 4)
		{
			this.agendaBtn.gameObject.SetActive(true);
		}
	}

	public void init(int ranking, int winCount, List<XElement> list)
	{
		base.initPlayerInfo(ranking, winCount, list);
		this.initItems();
		this.refLeftAttrPart();
		this.refRightAttrPart(this.m_JunxianLevel + 1, true);
	}

	public void refAttrPart()
	{
		this.m_Shengwang = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
		this.m_ShengwangLabel.Text = string.Empty + this.m_Shengwang;
		this.m_JunxianLevel = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		if (this.m_JunxianLevel <= 0)
		{
			this.m_JunxianLabel.Text = Global.GetLang("无");
		}
		else
		{
			GameObject at = this.collection.GetAt(this.m_JunxianLevel - 1);
			JunxianItem component = at.GetComponent<JunxianItem>();
			component.Icon.ToGrayBitmap = false;
			component.Bkg.spriteName = "jingjiChangJunxianIcon_bak";
			this.m_JunxianLabel.Text = Global.GetXElementAttributeStr(base.getElementByLevel(this.m_JunxianLevel), "Name");
			this.m_ListBox.SelectedIndex = this.m_JunxianLevel - 1;
			if (this.m_JunxianLevel == 1)
			{
				this.refRightAttrPart(this.m_JunxianLevel + 1, true);
			}
		}
		this.refLeftAttrPart();
		for (int i = 0; i < this.collection.Count; i++)
		{
			JunxianItem junxianItem = U3DUtils.AS<JunxianItem>(this.collection[i]);
			if (junxianItem != null && junxianItem.Level == this.m_JunxianLevel)
			{
				this.m_ListBox.SelectedIndex = i;
				break;
			}
		}
	}

	private void initItems()
	{
		int count = this.JunxianXmlList.Count;
		int num = -1;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.JunxianXmlList[i];
			if (xelement != null)
			{
				JunxianItem junxianItem = U3DUtils.NEW<JunxianItem>();
				this.collection.Add(junxianItem);
				junxianItem.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
				if (xelementAttributeInt > this.m_JunxianLevel)
				{
					junxianItem.Icon.ToGrayBitmap = true;
					junxianItem.Bkg.spriteName = "jingjiChangJunxianIconGray_bak";
				}
				junxianItem.Icon.ImageURL = StringUtil.substitute("NetImages/GameRes/Images/Junxianwenzi/{0}.jpg", new object[]
				{
					xelementAttributeInt
				});
				junxianItem.Icon.ForceShow();
				junxianItem.Level = xelementAttributeInt;
				if (xelementAttributeInt == this.m_JunxianLevel)
				{
					this.m_ListBox.SelectedIndex = i;
					junxianItem.Selected.gameObject.SetActive(true);
					num = this.m_ListBox.SelectedIndex;
				}
				UIPanel component = junxianItem.gameObject.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}
		if (this.m_JunxianLevel <= 0)
		{
			JunxianItem junxianItem2 = U3DUtils.AS<JunxianItem>(this.m_ListBox[0]);
			junxianItem2.Selected.gameObject.SetActive(true);
			this.m_ListBox.SelectedIndex = 0;
		}
		if (num >= 0)
		{
			this.ShowNextNeedGood(num);
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		JunxianItem junxianItem = U3DUtils.AS<JunxianItem>(this.m_ListBox.SelectedItem);
		if (null == junxianItem)
		{
			return;
		}
		if (this.tempitem != null && this.tempitem != junxianItem)
		{
			this.tempitem.Selected.gameObject.SetActive(false);
		}
		this.tempitem = junxianItem;
		this.tempitem.Selected.gameObject.SetActive(true);
		if ((this.m_JunxianLevel <= 0 && junxianItem.Level == 1) || junxianItem.Level == this.m_JunxianLevel)
		{
			this.refRightAttrPart(this.m_JunxianLevel + 1, true);
			this.ShowNextNeedGood(this.m_ListBox.SelectedIndex);
		}
		else
		{
			this.refRightAttrPart(this.tempitem.Level, false);
			this.ShowNextNeedGood(this.m_ListBox.SelectedIndex);
		}
	}

	private void ShowNextNeedGood(int tIndex)
	{
		int num = tIndex + 1;
		if (num < this.collection.Count)
		{
			GameObject at = this.collection.GetAt(num);
			if (at != null)
			{
				JunxianItem component = at.GetComponent<JunxianItem>();
				if (component != null)
				{
					if (!string.IsNullOrEmpty(component.NeedGoods))
					{
						if (this.GoodIconObj != null)
						{
							this.GoodIconObj.gameObject.SetActive(true);
						}
						string[] array = component.NeedGoods.Split(new char[]
						{
							','
						});
						if (array.Length >= 2)
						{
							this.NeedGoodsID = Global.SafeConvertToInt32(array[0]);
							this.NeedGoodsCount = Global.SafeConvertToInt32(array[1]);
						}
						this.AddGoodsIcon(this.goodIcon, this.NeedGoodsID, this.NeedGoodsCount);
					}
					else
					{
						this.NeedGoodsID = -1;
						this.NeedGoodsCount = -1;
						if (this.GoodIconObj != null)
						{
							this.GoodIconObj.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		else if (this.GoodIconObj != null)
		{
			this.GoodIconObj.gameObject.SetActive(false);
		}
	}

	private void refLeftAttrPart()
	{
		if (this.m_JunxianLevel < 1)
		{
			this.m_FutiBtn.isEnabled = false;
			return;
		}
		XElement elementByLevel = base.getElementByLevel(this.m_JunxianLevel);
		int xelementAttributeInt = Global.GetXElementAttributeInt(elementByLevel, "XiaoHaoShengWang");
		this.m_Futishengwang1Label.Text = xelementAttributeInt + string.Empty;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunXianBufferGoodsIDs", ',');
		if (systemParamIntArrayByName.Length <= 0)
		{
			return;
		}
		this.m_FutishijianLabel.Text = "60" + Global.GetLang("分钟");
		int num = this.m_JunxianLevel - 1;
		if (num >= systemParamIntArrayByName.Length)
		{
			num = systemParamIntArrayByName.Length - 1;
		}
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(Convert.ToInt32(systemParamIntArrayByName[num]));
		this.m_Attr1[0].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[7],
			goodsEquipPropsDoubleList[8]
		});
		this.m_Attr1[1].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[9],
			goodsEquipPropsDoubleList[10]
		});
		this.m_Attr1[2].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[3],
			goodsEquipPropsDoubleList[4]
		});
		this.m_Attr1[3].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[5],
			goodsEquipPropsDoubleList[6]
		});
		this.m_Attr1[4].Text = StringUtil.substitute("{0}", new object[]
		{
			goodsEquipPropsDoubleList[13]
		});
		this.m_Attr1[5].Text = StringUtil.substitute("{0}%", new object[]
		{
			goodsEquipPropsDoubleList[26] * 100.0
		});
		this.m_Attr1[6].Text = StringUtil.substitute("{0}", new object[]
		{
			goodsEquipPropsDoubleList[18]
		});
		if (this.m_Shengwang < xelementAttributeInt)
		{
			this.m_FutiBtn.isEnabled = false;
		}
		else
		{
			this.m_FutiBtn.isEnabled = true;
		}
	}

	private void refRightAttrPart(int level, bool next)
	{
		if (level < 0)
		{
			level = 0;
		}
		XElement elementByLevel = base.getElementByLevel(level);
		if (elementByLevel == null)
		{
			this.m_Futishengwang2Label.Text = string.Empty;
			this.m_ShengjishengwangLabel.Text = string.Empty;
			this.m_RightTitle.spriteName = "XiaJiJunXian";
			for (int i = 0; i < this.m_Attr2.Length; i++)
			{
				this.m_Attr2[i].Text = string.Empty;
			}
			return;
		}
		this.m_Futishengwang2Label.Text = Global.GetXElementAttributeStr(elementByLevel, "XiaoHaoShengWang");
		string xelementAttributeStr = Global.GetXElementAttributeStr(elementByLevel, "NeedShengWang");
		this.m_ShengjishengwangLabel.Text = xelementAttributeStr;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunXianBufferGoodsIDs", ',');
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(systemParamIntArrayByName[level - 1]);
		this.m_Attr2[0].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[7],
			goodsEquipPropsDoubleList[8]
		});
		this.m_Attr2[1].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[9],
			goodsEquipPropsDoubleList[10]
		});
		this.m_Attr2[2].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[3],
			goodsEquipPropsDoubleList[4]
		});
		this.m_Attr2[3].Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			goodsEquipPropsDoubleList[5],
			goodsEquipPropsDoubleList[6]
		});
		this.m_Attr2[4].Text = StringUtil.substitute("{0}", new object[]
		{
			goodsEquipPropsDoubleList[13]
		});
		this.m_Attr2[5].Text = StringUtil.substitute("{0}%", new object[]
		{
			goodsEquipPropsDoubleList[26] * 100.0
		});
		this.m_Attr2[6].Text = StringUtil.substitute("{0}", new object[]
		{
			goodsEquipPropsDoubleList[18]
		});
		if (this.m_Shengwang < Convert.ToInt32(xelementAttributeStr))
		{
			this.m_TishengBtn.isEnabled = false;
		}
		else
		{
			this.m_TishengBtn.isEnabled = true;
		}
		if (next)
		{
			this.m_TishengBtn.gameObject.SetActive(true);
			this.m_RightTitle.spriteName = "XiaJiJunXian";
		}
		else
		{
			this.m_TishengBtn.gameObject.SetActive(false);
			this.m_RightTitle.spriteName = "ChaKanJunXian";
		}
	}

	private void OnEnable()
	{
	}

	private bool IsJunxianMaxLevel(int currentLevel)
	{
		XElement xelement = this.JunxianXmlList[this.JunxianXmlList.Count - 1];
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
		return currentLevel == xelementAttributeInt;
	}

	private bool IsGoodsEnough()
	{
		bool result = false;
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(this.NeedGoodsID);
		if (totalGoodsCountByID >= this.NeedGoodsCount)
		{
			result = true;
		}
		return result;
	}

	private void AddGoodsIcon(GGoodIcon tsGoodIcons, int goodsID, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			tsGoodIcons.Width = 78.0;
			tsGoodIcons.Height = 78.0;
			tsGoodIcons.BackgroundSprite0.transform.localScale = new Vector3(78f, 78f, 0f);
			tsGoodIcons.isAutoSize = true;
			tsGoodIcons.BackSpriteName0 = backSpriteName;
			tsGoodIcons.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			tsGoodIcons.TipType = 1;
			tsGoodIcons.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			tsGoodIcons.ItemNum = iNeedNub;
			tsGoodIcons.ItemCode = goodsID;
			tsGoodIcons.ItemObject = dummyGoodsData;
			tsGoodIcons.BoxTypes = 5;
			tsGoodIcons.Text = iNeedNub.ToString();
			tsGoodIcons.TextShadowColor = 4278190080U;
			tsGoodIcons.TextColor = 16777215U;
			tsGoodIcons.DisableTextColor = 8421504U;
			tsGoodIcons.TextHorizontalAlignment = global::Layout.Right;
			tsGoodIcons.TextVerticalAlignment = global::Layout.Bottom;
			tsGoodIcons.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(tsGoodIcons, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			tsGoodIcons.Text = string.Empty;
			this.mNeedGoodsCount.Text = string.Format("{0}/{1}", totalGoodsCountByID, iNeedNub);
			tsGoodIcons.EnableIcon = true;
			tsGoodIcons.TextColor = 16777215U;
			tsGoodIcons.TeXiao.gameObject.SetActive(false);
			if (totalGoodsCountByID >= iNeedNub)
			{
				this.mNeedGoodsCount.textColor = 16777215U;
			}
			else
			{
				this.mNeedGoodsCount.textColor = 16711680U;
			}
			tsGoodIcons.TextShadowColor = 4278190080U;
			tsGoodIcons.TextHorizontalAlignment = global::Layout.Right;
			tsGoodIcons.TextVerticalAlignment = global::Layout.Bottom;
			UIPanel component = tsGoodIcons.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			tsGoodIcons.transform.localPosition = new Vector3(162f, -245f, -1f);
			tsGoodIcons.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
			tsGoodIcons.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
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
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public TextBlock m_Futishengwang1Label;

	public TextBlock m_FutishijianLabel;

	public TextBlock m_ShengjishengwangLabel;

	public TextBlock m_Futishengwang2Label;

	public TextBlock[] m_Attr1;

	public TextBlock[] m_Attr2;

	public TextBlock[] m_ConstAttr1;

	public TextBlock[] m_ConstAttr2;

	public GButton m_FutiBtn;

	public GButton m_TishengBtn;

	public DPSelectedItemEventHandler openAdendaViewEventHandler;

	public GButton agendaBtn;

	public UISprite m_RightTitle;

	public GGoodIcon goodIcon;

	public GameObject GoodIconObj;

	public TextBlock mTips;

	public TextBlock mNeedGoodsCount;

	private JunxianItem tempitem;

	private int NeedGoodsID = -1;

	private int NeedGoodsCount;
}
