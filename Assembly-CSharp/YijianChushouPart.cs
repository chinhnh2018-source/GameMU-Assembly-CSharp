using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YijianChushouPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ChuShouBtn.Text = Global.GetLang("立即回收");
		this.DuiHuanBtn.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f3f2f2",
			Global.GetLang("魔晶兑换")
		});
		this.baicheckBox.Text = Global.GetLang("白装");
		this.lvcheckBox.Text = Global.GetLang("绿装");
		this.lancheckBox.Text = Global.GetLang("蓝装");
		this.zicheckBox.Text = Global.GetLang("紫装");
		this.jingLinglancheckBox.GetComponentInChildren<UILabel>().text = Global.GetLang("蓝色");
		this.jingLingzicheckBox.GetComponentInChildren<UILabel>().text = Global.GetLang("紫色");
		this.lancheckBox._Lable.lineWidth = 105;
		this.baicheckBox._Lable.lineWidth = 95;
		this.m_HorseHuiShouBtn.Text = Global.GetLang("坐骑回收");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Items = this.goodsListBox.Items;
		this.baicheckBox.isChecked = true;
		this.lvcheckBox.isChecked = false;
		this.lancheckBox.isChecked = false;
		this.zicheckBox.isChecked = false;
		this.jingLinglancheckBox.isChecked = true;
		this.jingLingzicheckBox.isChecked = false;
		this.InitHuiShou();
		this.m_HuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelHuiShou.gameObject.SetActive(!this.m_PanelHuiShou.gameObject.activeSelf);
		};
		this.m_RoleHuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.Equipment, false);
		};
		this.m_PetHuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.Pet, false);
		};
		this.m_HorseHuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.horse, false);
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.ChuShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (0 < SystemHelpMgr.ActiveHelpID)
			{
				SystemHelpMgr.OnAction(UIObjIDs.HuiShouDetailPatr, HelpStateEvents.Inactived, -1);
			}
			if (this.Items.Count <= 0)
			{
				return;
			}
			bool flag = false;
			int categoriy = 0;
			string goodslist = string.Empty;
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < this.Items.Count; i++)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
				if (goodsData != null)
				{
					if (goodslist.Length > 0)
					{
						goodslist += ",";
					}
					goodslist += goodsData.Id;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if ((goodsXmlNodeByID.SuitID >= 5 && Global.GetZhuoyueAttributeCount(goodsData) >= 2) || (goodsXmlNodeByID.SuitID < 5 && Global.GetZhuoyueAttributeCount(goodsData) >= 4) || goodsData.Forge_level >= 7 || goodsData.AppendPropLev >= 20)
					{
						flag = true;
					}
					categoriy = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if ((categoriy == 9 || categoriy == 10) && goodsXmlNodeByID.SuitID >= 4)
					{
						flag = true;
					}
					if (categoriy == 340 || (categoriy >= 40 && categoriy <= 45))
					{
						int horseQuality = Super.GetHorseQuality(goodsData);
						if (3 <= horseQuality)
						{
							flag = true;
						}
					}
					list.Add(goodsData);
				}
			}
			if (goodslist != string.Empty)
			{
				if (flag)
				{
					if (categoriy == 9 || categoriy == 10)
					{
						string[] awardDescribe = new string[]
						{
							Global.GetLang("精灵本体回收"),
							Global.GetLang("精灵等级回收"),
							Global.GetLang("精灵技能回收")
						};
						int[] jingLingSRecoverAward = Global.GetJingLingSRecoverAward(list);
						Super.ShowJingLingHuiShouMessageBox(Global.GetLang("精灵回收"), awardDescribe, jingLingSRecoverAward, delegate(object x, DPSelectedItemEventArgs c)
						{
							if (c.ID == 0)
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(3, goodslist);
								this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
								this.Items.Clear();
								Super.goodDBIdDict.Clear();
							}
							return true;
						}, null);
					}
					else if (categoriy == 340 || (categoriy >= 40 && categoriy <= 45))
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("回收物品中有已升阶的坐骑或贵重的坐骑装备，是否确认要进行回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(5, goodslist);
								this.TongqianNumText.Text = Global.GetRoleOwnNumByMoneyType(139).ToString();
								this._MoJingLabel.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
								this.Items.Clear();
								Super.goodDBIdDict.Clear();
							}
							return true;
						};
					}
					else
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("回收物品中有贵重物品，是否确认要进行回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								if (categoriy == 9 || categoriy == 10)
								{
									GameInstance.Game.SpriteOneKeyQuickSaleOut(3, goodslist);
									this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
								}
								else
								{
									GameInstance.Game.SpriteOneKeyQuickSaleOut(2, goodslist);
									this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
									this.ZaizaoNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian).ToString();
								}
								this.Items.Clear();
								Super.goodDBIdDict.Clear();
							}
							return true;
						};
					}
				}
				else
				{
					if (categoriy == 9 || categoriy == 10)
					{
						GameInstance.Game.SpriteOneKeyQuickSaleOut(3, goodslist);
						this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
					}
					else if (categoriy == 340 || (categoriy >= 40 && categoriy <= 45))
					{
						GameInstance.Game.SpriteOneKeyQuickSaleOut(5, goodslist);
						this.TongqianNumText.Text = Global.GetRoleOwnNumByMoneyType(139).ToString();
						this._MoJingLabel.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
						this.Items.Clear();
						Super.goodDBIdDict.Clear();
					}
					else
					{
						GameInstance.Game.SpriteOneKeyQuickSaleOut(2, goodslist);
						this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
						this.ZaizaoNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian).ToString();
					}
					this.Items.Clear();
					Super.goodDBIdDict.Clear();
				}
			}
		};
		this.DuiHuanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.huiShouType == HuiShouType.Equipment)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2
				});
			}
			else if (this.huiShouType == HuiShouType.horse)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -4
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -3
				});
			}
		};
		this.baicheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
			if (0 < SystemHelpMgr.ActiveHelpID)
			{
				SystemHelpMgr.OnAction(UIObjIDs.HuiShouDetailPatr, HelpStateEvents.Clicked, -1);
			}
		};
		this.lvcheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.lancheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.zicheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
			if (0 < SystemHelpMgr.ActiveHelpID)
			{
				SystemHelpMgr.OnAction(UIObjIDs.HuiShouDetailPatr, HelpStateEvents.Clicked, -1);
			}
		};
		this.jingLinglancheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.jingLingzicheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.InitDict();
	}

	public void InitHuiShou()
	{
		this.m_ListFuMo.Clear();
		if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese))
		{
			this.m_HorseHuiShouBtn.gameObject.SetActive(true);
		}
		else
		{
			this.m_HorseHuiShouBtn.gameObject.SetActive(false);
		}
		if (this.m_RoleHuiShouBtn.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.m_RoleHuiShouBtn);
		}
		if (this.m_PetHuiShouBtn.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.m_PetHuiShouBtn);
		}
		if (this.m_HorseHuiShouBtn.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.m_HorseHuiShouBtn);
		}
		this.m_SpFuMoBack.transform.localScale = new Vector3(148f, (float)(this.m_ListFuMo.Count * 40), 1f);
		Vector3 localPosition;
		localPosition..ctor(-118f, -157f, -0.2f);
		for (int i = this.m_ListFuMo.Count - 1; i >= 0; i--)
		{
			this.m_ListFuMo[i].transform.localPosition = localPosition;
			localPosition.y += 20f;
			if (i > 0)
			{
				GameObject gameObject = U3DUtils.Clone(this.m_PanelHuiShou.gameObject, this.m_SpFenGeXian.gameObject);
				gameObject.transform.localPosition = localPosition;
				gameObject.SetActive(true);
				localPosition.y += 20f;
			}
		}
		this.HuiShouBtnOnClick(HuiShouType.Equipment, true);
	}

	public void HuiShouBtnOnClick(HuiShouType type, bool strBool = false)
	{
		Vector3 localPosition = this.m_GameOnBack.transform.localPosition;
		if (type == HuiShouType.Equipment)
		{
			this.m_RoleHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("装备回收")
			});
			this.m_PetHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("精灵回收")
			});
			this.m_HorseHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("坐骑回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("装备回收")
			});
			localPosition.y = this.m_RoleHuiShouBtn.transform.localPosition.y;
		}
		else if (type == HuiShouType.Pet)
		{
			this.m_RoleHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("装备回收")
			});
			this.m_PetHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("精灵回收")
			});
			this.m_HorseHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("坐骑回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("精灵回收")
			});
			localPosition.y = this.m_PetHuiShouBtn.transform.localPosition.y;
		}
		else if (type == HuiShouType.horse)
		{
			this.m_RoleHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("装备回收")
			});
			this.m_PetHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("精灵回收")
			});
			this.m_HorseHuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("坐骑回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("坐骑回收")
			});
			localPosition.y = this.m_HorseHuiShouBtn.transform.localPosition.y;
		}
		this.m_GameOnBack.transform.localPosition = localPosition;
		this.m_PanelHuiShou.gameObject.SetActive(false);
		if (!strBool)
		{
			this.huiShouType = type;
			this.InitPartData();
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.goodsListBox.AutoCollider = true;
	}

	public void InitPartData()
	{
		this.baicheckBox.Text = Global.GetLang("白装");
		this.lvcheckBox.Text = Global.GetLang("绿装");
		this.lancheckBox.Text = Global.GetLang("蓝装");
		this.zicheckBox.Text = Global.GetLang("紫装");
		this.baicheckBox.transform.localPosition = new Vector3(-178f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
		this.lancheckBox.transform.localPosition = new Vector3(30f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
		this.zicheckBox.transform.localPosition = new Vector3(136f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
		if (this.huiShouType == HuiShouType.Equipment)
		{
			this.lancheckBox.Text = Global.GetLang("蓝装及以下");
			this.zicheckBox.Text = Global.GetLang("紫装");
			this.baicheckBox.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("非本职业")
			});
			this.baicheckBox.transform.localPosition = new Vector3(98f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
			this.lancheckBox.transform.localPosition = new Vector3(-178f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
			this.zicheckBox.transform.localPosition = new Vector3(-24f, this.lancheckBox.transform.localPosition.y, this.lancheckBox.transform.localPosition.z);
			this.lvcheckBox.gameObject.SetActive(false);
			this.ZaizaodianInfo.SetActive(GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao));
			this.baicheckBox.gameObject.SetActive(true);
			this.lancheckBox.gameObject.SetActive(true);
			this.zicheckBox.gameObject.SetActive(true);
			this.jingLinglancheckBox.gameObject.SetActive(false);
			this.jingLingzicheckBox.gameObject.SetActive(false);
			this.DuiHuanBtn.Text = Global.GetLang("魔晶兑换");
			this.huoBiSprite.spriteName = "mojing";
			this.huoBiType.text = Global.GetLang("魔晶:");
			this.lancheckBox.Check = true;
			this.baicheckBox.Check = false;
		}
		else if (this.huiShouType == HuiShouType.horse)
		{
			this.baicheckBox.Text = Global.GetLang("白色");
			this.lvcheckBox.Text = Global.GetLang("绿色");
			this.lancheckBox.Text = Global.GetLang("蓝色");
			this.zicheckBox.Text = Global.GetLang("紫色");
			this.ZaizaodianInfo.SetActive(true);
			this.baicheckBox.gameObject.SetActive(true);
			this.lvcheckBox.gameObject.SetActive(true);
			this.lancheckBox.gameObject.SetActive(true);
			this.zicheckBox.gameObject.SetActive(true);
			this.jingLinglancheckBox.gameObject.SetActive(false);
			this.jingLingzicheckBox.gameObject.SetActive(false);
			this.DuiHuanBtn.Text = Global.GetLang("查看坐骑");
			this.huoBiSprite.spriteName = "mohe";
		}
		else
		{
			this.ZaizaodianInfo.SetActive(false);
			this.baicheckBox.gameObject.SetActive(false);
			this.lvcheckBox.gameObject.SetActive(false);
			this.lancheckBox.gameObject.SetActive(false);
			this.zicheckBox.gameObject.SetActive(false);
			this.jingLinglancheckBox.gameObject.SetActive(true);
			this.jingLingzicheckBox.gameObject.SetActive(true);
			this.DuiHuanBtn.Text = Global.GetLang("查看精灵");
			this.huoBiSprite.spriteName = "mohe";
			this.huoBiType.text = Global.GetLang("灵晶:");
		}
		this.LoadAllWhiteEquip();
		this.SetPrice();
		ObjectClickGetingMgr.StartClickGetThing(18, null);
		this.selectImage.Visibility = false;
		this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
		if (0 < SystemHelpMgr.ActiveHelpID)
		{
			SystemHelpMgr.OnAction(UIObjIDs.HuiShouPart, HelpStateEvents.Actived, -1);
		}
	}

	public void CleanUpChildWindows()
	{
	}

	public void GetNewData()
	{
	}

	private void SetPrice()
	{
		this._MoJingObj.SetActive(false);
		this.TongqianNum = 0;
		this.ZaizaoNum = 0;
		int num = 0;
		if (this.huiShouType == HuiShouType.Equipment)
		{
			for (int i = 0; i < this.goodsListBox.Count(); i++)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
				this.TongqianNum += Global.GetGoodsSaleToNpJiFen(goodsData);
				this.ZaizaoNum += Global.GetGoodsSaleToNpcZaizao(goodsData);
			}
		}
		else if (this.huiShouType == HuiShouType.horse)
		{
			for (int j = 0; j < this.goodsListBox.Count(); j++)
			{
				GoodsData goodsData2 = U3DUtils.AS<GGoodIcon>(this.Items[j]).ItemObject as GoodsData;
				if (goodsData2 != null)
				{
					byte b = 0;
					int horsePrice = Global.GetHorsePrice(goodsData2, out b);
					num += ((Global.GetGoodsSaleToNpJiFen(goodsData2) != 1) ? Global.GetGoodsSaleToNpJiFen(goodsData2) : 0);
					if (b != 0)
					{
						this.TongqianNum += horsePrice;
					}
					this.ZaizaoNum += Global.GetGoodsSaleToNpcZaizao(goodsData2);
				}
			}
		}
		else
		{
			for (int k = 0; k < this.goodsListBox.Count(); k++)
			{
				GoodsData goodsData3 = U3DUtils.AS<GGoodIcon>(this.Items[k]).ItemObject as GoodsData;
				this.TongqianNum += Global.GetPetPrice(goodsData3);
			}
		}
		if (this.huiShouType == HuiShouType.Equipment)
		{
			this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString() + " + {00FF00}" + this.TongqianNum.ToString() + "{-}";
			this.ZaizaoNumText.Text = string.Format("{0} +{1}", Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.ZaizaoNum.ToString()
			}));
		}
		else if (this.huiShouType == HuiShouType.horse)
		{
			this._MoJingObj.SetActive(true);
			this.TongqianNumText.Text = string.Concat(new object[]
			{
				Global.GetRoleOwnNumByMoneyType(139),
				" + {00FF00}",
				this.TongqianNum.ToString(),
				"{-}"
			});
			this.ZaizaoNumText.Text = string.Format("{0} +{1}", Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.ZaizaoNum.ToString()
			}));
			this._MoJingLabel.text = string.Format("{0} +{1}", Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				num.ToString()
			}));
			this.huoBiSprite.spriteName = "HunJing";
		}
		else
		{
			this.TongqianNumText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString() + " + {00FF00}" + this.TongqianNum.ToString() + "{-}";
		}
	}

	private void InitDict()
	{
		for (int i = 0; i < this.YaopinGoodsID.Length; i++)
		{
			if (!this.GoodsIDDict.ContainsKey(this.YaopinGoodsID[i]))
			{
				this.GoodsIDDict[this.YaopinGoodsID[i]] = 0;
			}
		}
	}

	private void LoadAllWhiteEquip()
	{
		this.ClearBgShouStat();
		this.Items.Clear();
		if (Global.Data.roleData.GoodsDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
			if (goodsData != null)
			{
				if (goodsData.GCount > 0)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (this.huiShouType == HuiShouType.Equipment)
					{
						if (Super.CanSaleOutGoods(goodsData))
						{
							if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25 && categoriyByGoodsID != 9 && categoriyByGoodsID != 10)
							{
								if (goodsData.Using <= 0)
								{
									if (this.Items.Count >= 20)
									{
										break;
									}
									GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
									if (goodsXmlNodeByID != null)
									{
										if (goodsXmlNodeByID.ChangeJinYuan != 0)
										{
											if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao))
											{
												if (goodsXmlNodeByID.SuitID >= Global.ShenqiZaizaoSuit)
												{
													goto IL_42C;
												}
											}
											else if (goodsXmlNodeByID.SuitID > Global.ShenqiZaizaoSuit)
											{
												goto IL_42C;
											}
											int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
											if (this.lvcheckBox.isChecked && zhuoyueAttributeCount > 0 && zhuoyueAttributeCount < 3)
											{
												if (this.baicheckBox.isChecked && (1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) != 0)
												{
													goto IL_42C;
												}
												this.AddGGoodIcon(goodsData);
											}
											if (this.lancheckBox.isChecked && zhuoyueAttributeCount < 5)
											{
												if (this.baicheckBox.isChecked && (1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) != 0)
												{
													goto IL_42C;
												}
												this.AddGGoodIcon(goodsData);
											}
											if (this.zicheckBox.isChecked && zhuoyueAttributeCount >= 5)
											{
												if (!this.baicheckBox.isChecked || (1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) == 0)
												{
													this.AddGGoodIcon(goodsData);
												}
											}
										}
									}
								}
							}
						}
					}
					else if (this.huiShouType == HuiShouType.horse)
					{
						if (this.Items.Count >= 20)
						{
							break;
						}
						if (Super.CanSaleOutGoods(goodsData))
						{
							if (categoriyByGoodsID == 340 || (categoriyByGoodsID >= 40 && categoriyByGoodsID <= 45))
							{
								if (goodsData.Using <= 0 && 0 >= goodsData.Forge_level)
								{
									if (!Global.IsShengqi(goodsData))
									{
										GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
										if (goodsXmlNodeByID2 != null)
										{
											if (goodsXmlNodeByID2.SuitID < Global.ShenqiZaizaoSuit)
											{
												int horseQuality = Super.GetHorseQuality(goodsData);
												if (horseQuality == 0)
												{
													if (this.baicheckBox.isChecked)
													{
														this.AddGGoodIcon(goodsData);
													}
												}
												else if (horseQuality == 1)
												{
													if (this.lvcheckBox.isChecked)
													{
														this.AddGGoodIcon(goodsData);
													}
												}
												else if (horseQuality == 2)
												{
													if (this.lancheckBox.isChecked)
													{
														this.AddGGoodIcon(goodsData);
													}
												}
												else if (horseQuality >= 3 && this.zicheckBox.isChecked)
												{
													this.AddGGoodIcon(goodsData);
												}
											}
										}
									}
								}
							}
						}
					}
					else if (categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
					{
						if (goodsData.Using <= 0)
						{
							if (this.Items.Count >= 20)
							{
								break;
							}
							GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							if (goodsXmlNodeByID3 != null)
							{
								int suitID = goodsXmlNodeByID3.SuitID;
								if (this.jingLinglancheckBox.isChecked && suitID == 1)
								{
									this.AddGGoodIcon(goodsData);
								}
								if (this.jingLingzicheckBox.isChecked && suitID > 1)
								{
									this.AddGGoodIcon(goodsData);
								}
							}
						}
					}
				}
			}
			IL_42C:;
		}
		this.Items.DelayUpdate();
		this.SetPrice();
	}

	private void AddGGoodIcon(GoodsData goodsData)
	{
		GGoodIcon icon = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao) && Global.IsShengqi(goodsData))
			{
				Super.HintMainText(string.Format(Global.GetLang("需要开启【{0}】系统才能回收"), GongnengYugaoMgr.GetGongNengName(GongNengIDs.ZaiZao)), 10, 3);
				return;
			}
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int categoriy = goodsXmlNodeByID.Categoriy;
			try
			{
				icon = U3DUtils.NEW<GGoodIcon>();
				icon.Width = 64.0;
				icon.Height = 64.0;
				icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				icon.TipType = 1;
				icon.ItemCategory = categoriy;
				icon.ItemCode = goodsData.GoodsID;
				icon.ItemObject = goodsData;
				icon.TextSize = 20;
				icon.TextShadowColor = 4278190080U;
				icon.BoxTypes = 1;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			this.Items.Add(icon.gameObject);
			if (!Super.goodDBIdDict.ContainsKey(goodsData.Id))
			{
				Super.goodDBIdDict.Add(goodsData.Id, goodsData.GoodsID);
			}
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, goodsData, canUse, IconTextTypes.Qianghua);
			Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), true);
			Global.SetEquipGoodsZhanLiStat(icon, goodsData);
			this.SetPrice();
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.Items.Remove(icon.gameObject);
				Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
				this.SetPrice();
				if (Super.goodDBIdDict.ContainsKey(goodsData.Id))
				{
					Super.goodDBIdDict.Remove((icon.ItemObject as GoodsData).Id);
				}
			};
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 6)
				{
					this.Items.Remove(icon.gameObject);
					Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
					this.SetPrice();
					if (Super.goodDBIdDict.ContainsKey(goodsData.Id))
					{
						Super.goodDBIdDict.Remove((icon.ItemObject as GoodsData).Id);
					}
				}
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>DPSelectedItem</color>"
				});
			};
		}
	}

	public void AddHuiShouGoods(GoodsData gd)
	{
		if (gd != null)
		{
			if (Global.GetGoodsSaleToNpJiFen(gd) <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (this.FindGoodDataByID(gd.Id))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("物品已存在"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (this.goodsListBox.Count() >= 24)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("回收空间已经满，请先回收后，再进行添加操作！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			if (goodsXmlNodeByID.ChangeJinYuan == 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (goodsXmlNodeByID.Categoriy < 25)
			{
				if ((goodsXmlNodeByID.SuitID >= 5 && Global.GetZhuoyueAttributeCount(gd) >= 2) || (goodsXmlNodeByID.SuitID < 5 && Global.GetZhuoyueAttributeCount(gd) >= 4) || gd.Forge_level >= 7 || gd.AppendPropLev >= 20)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("此装备比较贵重，是否确认回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.AddGGoodIcon(gd);
							this.Items.DelayUpdate();
							this.SetPrice();
						}
						return true;
					};
				}
				else
				{
					this.AddGGoodIcon(gd);
					this.Items.DelayUpdate();
					this.SetPrice();
				}
			}
			else if (40 <= goodsXmlNodeByID.Categoriy && 45 >= goodsXmlNodeByID.Categoriy)
			{
				if (3 <= Super.GetHorseQuality(gd) || 0 < gd.Forge_level)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("此装备比较贵重，是否确认回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.AddGGoodIcon(gd);
							this.Items.DelayUpdate();
							this.SetPrice();
						}
						return true;
					};
				}
				else
				{
					this.AddGGoodIcon(gd);
					this.Items.DelayUpdate();
					this.SetPrice();
				}
			}
			else
			{
				if (goodsXmlNodeByID.Categoriy != 340)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
					return;
				}
				if (3 <= Super.GetHorseQuality(gd))
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("此装备比较贵重，是否确认回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.AddGGoodIcon(gd);
							this.Items.DelayUpdate();
							this.SetPrice();
						}
						return true;
					};
				}
				else if (0 < gd.Forge_level)
				{
					Super.HintMainText(Global.GetLang("只有将坐骑重置至1阶才能回收"), 10, 3);
				}
				else
				{
					this.AddGGoodIcon(gd);
					this.Items.DelayUpdate();
					this.SetPrice();
				}
			}
		}
	}

	public void AddChuShouGoods(GoodsData gd)
	{
		if (gd != null)
		{
			if (!Super.CanSaleOutGoods(gd))
			{
				Super.HintMainText(Global.GetLang("此物品不可出售！"), 10, 3);
			}
			else
			{
				if (gd.ExcellenceInfo > 0)
				{
					Super.HintMainText(Global.GetLang("卓越及以上装备可回收获得魔晶!"), 10, 3);
					return;
				}
				if (!this.FindGoodDataByID(gd.Id))
				{
					if (this.goodsListBox.Count() < 20)
					{
						this.AddGGoodIcon(gd);
						this.Items.DelayUpdate();
						this.SetPrice();
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("一键出售的空间已经满，请先出售后，再进行添加操作"), new object[0]), 0, -1, -1, 0);
						Super.HintMainText(Global.GetLang("一键出售的空间已经满！"), 10, 3);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("物品已存在！"), 10, 3);
				}
			}
		}
	}

	private bool FindGoodDataByID(int id)
	{
		if (this.goodsListBox.Count() <= 0)
		{
			return false;
		}
		for (int i = 0; i < this.goodsListBox.Count(); i++)
		{
			this.goodsListBox.SelectedIndex = i;
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.goodsListBox.SelectedItem);
			if (!(null == ggoodIcon))
			{
				GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
				if (goodsData != null)
				{
					if (goodsData.Id == id)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void ClearBgShouStat()
	{
		for (int i = 0; i < this.Items.Count; i++)
		{
			GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
			if (goodsData != null)
			{
				Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID(goodsData.Id), false);
			}
		}
		Super.goodDBIdDict.Clear();
	}

	internal void RefreshMoJingPrice()
	{
		this._MoJingLabel.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
	}

	internal void RefreshEquipmentPrice()
	{
		this.SetPrice();
	}

	internal void RefreshListBox()
	{
		if (this.Items == null || this.Items.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.Items.Count; i++)
		{
			GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
			if (goodsData != null)
			{
				for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
				{
					if (Global.Data.roleData.GoodsDataList[j].Id == goodsData.Id)
					{
						break;
					}
					if (j >= Global.Data.roleData.GoodsDataList.Count - 1)
					{
						this.Items.RemoveAt(i);
					}
				}
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int TongqianNum;

	private int ZaizaoNum;

	public HuiShouType huiShouType;

	public TextBlock TongqianNumText;

	public TextBlock BangdingJinBiText;

	public TextBlock ZaizaoNumText;

	public UISprite huoBiSprite;

	public UILabel huoBiType;

	public ListBox goodsListBox;

	public TextBlock TypeText;

	public GButton CloseBtn;

	public GButton ChuShouBtn;

	public GButton DuiHuanBtn;

	public GCheckBox baicheckBox;

	public GCheckBox lvcheckBox;

	public GCheckBox lancheckBox;

	public GCheckBox zicheckBox;

	public GCheckBox jingLinglancheckBox;

	public GCheckBox jingLingzicheckBox;

	public GameObject ZaizaodianInfo;

	public GButton m_HuiShouBtn;

	public UIPanel m_PanelHuiShou;

	public GButton m_RoleHuiShouBtn;

	public GButton m_PetHuiShouBtn;

	public GButton m_HorseHuiShouBtn;

	public UISprite m_SpFenGeXian;

	public UISprite m_SpFuMoBack;

	public GameObject m_GameOnBack;

	private List<GButton> m_ListFuMo = new List<GButton>();

	[SerializeField]
	private GameObject _MoJingObj;

	[SerializeField]
	private UILabel _MoJingLabel;

	public SpriteSL selectImage;

	public SpriteSL selectBaoGuoImage;

	private ObservableCollection Items;

	private int[] YaopinGoodsID = new int[]
	{
		20000,
		20001,
		20002,
		20003,
		20004,
		20005,
		20006,
		20007,
		20010,
		20011,
		20012,
		20013,
		20014,
		20015,
		20016,
		20017,
		20020,
		20021,
		20022,
		20023,
		1000040,
		1100040,
		1200040
	};

	private Dictionary<int, int> GoodsIDDict = new Dictionary<int, int>();
}
