using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChongShengChushouPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ChuShouBtn.Text = Global.GetLang("立即回收");
		this.checkBox1.Text = Global.GetLang("蓝装");
		this.checkBox2.Text = Global.GetLang("紫装");
		this.checkBox3.Text = Global.GetLang("紫闪");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Items = this.goodsListBox.Items;
		this.checkBox1.isChecked = true;
		this.checkBox2.isChecked = false;
		this.checkBox3.isChecked = false;
		this.InitHuiShou();
		this.m_HuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelHuiShou.gameObject.SetActive(!this.m_PanelHuiShou.gameObject.activeSelf);
		};
		this.btnEquip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.EquipmentChongSheng, false);
		};
		this.btnBaoShi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.BaoShiChongSheng, false);
		};
		this.btnXuanCai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HuiShouBtnOnClick(HuiShouType.XuanCaiChongSheng, false);
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
			if (this.huiShouType == HuiShouType.EquipmentChongSheng)
			{
				this.ChuShouEquip();
			}
			else if (this.huiShouType == HuiShouType.BaoShiChongSheng || this.huiShouType == HuiShouType.XuanCaiChongSheng)
			{
				this.ChuShouBaoShi();
			}
		};
		this.checkBox1.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.checkBox2.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.checkBox3.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.LoadAllWhiteEquip();
		};
		this.InitDict();
	}

	public void InitHuiShou()
	{
		this.m_ListFuMo.Clear();
		if (this.btnXuanCai.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.btnXuanCai);
		}
		if (this.btnBaoShi.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.btnBaoShi);
		}
		if (this.btnEquip.gameObject.activeSelf)
		{
			this.m_ListFuMo.Add(this.btnEquip);
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
		this.HuiShouBtnOnClick(HuiShouType.EquipmentChongSheng, true);
	}

	private void ChuShouEquip()
	{
		bool flag = false;
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
				if (Global.GetZhuoyueAttributeCount(goodsData) >= 5)
				{
					flag = true;
				}
				list.Add(goodsData);
			}
		}
		if (goodslist != string.Empty)
		{
			if (this.huiShouType == HuiShouType.EquipmentChongSheng && Global.Data.roleData.RebornCount <= 0)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("您尚未重生，回收不会获得重生经验，是否确认要进行回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(goodslist);
						this.ResetMoenyData();
						this.Items.Clear();
						Super.goodDBIdDict.Clear();
					}
					return true;
				};
				return;
			}
			if (flag)
			{
				string message = string.Empty;
				if (this.m_expFull <= 0L)
				{
					message = Global.GetLang("回收物品中有贵重物品，是否确认要进行回收？");
				}
				else
				{
					message = StringUtil.substitute(Global.GetLang("本次回收有贵重物品，且已达到今日回收经验上限，将有{0}经验溢出，是否确认回收？"), new object[]
					{
						this.m_expFull
					});
				}
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), message, 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(goodslist);
						this.ResetMoenyData();
						this.Items.Clear();
						Super.goodDBIdDict.Clear();
					}
					return true;
				};
			}
			else if (this.m_expFull > 0L)
			{
				string message2 = StringUtil.substitute(Global.GetLang("本次回收已达到今日回收经验上限，将有{0}经验溢出，是否确认回收？"), new object[]
				{
					this.m_expFull
				});
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), message2, 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(goodslist);
						this.ResetMoenyData();
						this.Items.Clear();
						Super.goodDBIdDict.Clear();
					}
					return true;
				};
			}
			else
			{
				GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(goodslist);
				this.ResetMoenyData();
				this.Items.Clear();
				Super.goodDBIdDict.Clear();
			}
		}
	}

	private void ChuShouBaoShi()
	{
		bool flag = false;
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
				if (Global.IsNeedSaleRebornTiShi(goodsData))
				{
					flag = true;
				}
				list.Add(goodsData);
			}
		}
		if (goodslist != string.Empty)
		{
			if (flag)
			{
				string lang = Global.GetLang("回收物品中有贵重物品，是否确认要进行回收？");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), lang, 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendChongShengPiLiangFenJie(goodslist);
						this.ResetMoenyData();
						this.Items.Clear();
						Super.goodDBIdDict.Clear();
					}
					return true;
				};
			}
			else
			{
				GameInstance.Game.SendChongShengPiLiangFenJie(goodslist);
				this.ResetMoenyData();
				this.Items.Clear();
				Super.goodDBIdDict.Clear();
			}
		}
	}

	public void HuiShouBtnOnClick(HuiShouType type, bool strBool = false)
	{
		Vector3 localPosition = this.m_GameOnBack.transform.localPosition;
		if (type == HuiShouType.EquipmentChongSheng)
		{
			this.btnEquip.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("重生装备回收")
			});
			this.btnBaoShi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("重生宝石回收")
			});
			this.btnXuanCai.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("炫彩宝石回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("重生装备回收")
			});
			localPosition.y = this.btnEquip.transform.localPosition.y;
		}
		else if (type == HuiShouType.BaoShiChongSheng)
		{
			this.btnEquip.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("重生装备回收")
			});
			this.btnBaoShi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("重生宝石回收")
			});
			this.btnXuanCai.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("炫彩宝石回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("重生宝石回收")
			});
			localPosition.y = this.btnBaoShi.transform.localPosition.y;
		}
		else if (type == HuiShouType.XuanCaiChongSheng)
		{
			this.btnEquip.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("重生装备回收")
			});
			this.btnBaoShi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"aa8669",
				Global.GetLang("重生宝石回收")
			});
			this.btnXuanCai.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffbca",
				Global.GetLang("炫彩宝石回收")
			});
			this.m_HuiShouBtn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2db97",
				Global.GetLang("炫彩宝石回收")
			});
			localPosition.y = this.btnXuanCai.transform.localPosition.y;
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
		this.checkBox1.Text = Global.GetLang("蓝装");
		this.checkBox2.Text = Global.GetLang("紫装");
		this.checkBox3.Text = Global.GetLang("紫闪");
		this.checkBox1.Check = false;
		this.checkBox2.Check = false;
		this.checkBox3.Check = false;
		if (this.huiShouType == HuiShouType.EquipmentChongSheng)
		{
			this.checkBox1.Text = Global.GetLang("蓝装");
			this.checkBox2.Text = Global.GetLang("紫装");
			this.checkBox3.Text = Global.GetLang("紫闪");
			this.huoBiType.text = Global.GetLang("重生经验:");
			this.checkBox1.Check = true;
			this.huoBiSprite.gameObject.SetActive(true);
			this.expMaxNumText.gameObject.SetActive(true);
			if (Global.Data.roleData.RebornCount > 0)
			{
				RebornLevelVO rebornLevelVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornLevelVOByID(Global.Data.roleData.RebornLevel);
				if (rebornLevelVOByID != null)
				{
					long num = (long)rebornLevelVOByID.MaxOfGoods;
				}
			}
			this.ResetChongShengExp();
		}
		else if (this.huiShouType == HuiShouType.BaoShiChongSheng)
		{
			this.checkBox1.Text = Global.GetLang("3级及以下");
			this.checkBox2.Text = Global.GetLang("4级宝石");
			this.checkBox3.Text = Global.GetLang("5级宝石");
			this.huoBiType.text = Global.GetLang("魔晶:");
			this.checkBox1.Check = true;
			this.huoBiSprite.gameObject.SetActive(false);
			this.expMaxNumText.gameObject.SetActive(false);
			this.goExp.SetActive(false);
			this.goChongShengHuoBi.SetActive(true);
		}
		else if (this.huiShouType == HuiShouType.XuanCaiChongSheng)
		{
			this.checkBox1.Text = Global.GetLang("1级宝石");
			this.checkBox2.Text = Global.GetLang("2级宝石");
			this.checkBox3.Text = Global.GetLang("3级宝石");
			this.huoBiType.text = Global.GetLang("魔晶:");
			this.checkBox1.Check = true;
			this.huoBiSprite.gameObject.SetActive(false);
			this.expMaxNumText.gameObject.SetActive(false);
			this.goExp.SetActive(false);
			this.goChongShengHuoBi.SetActive(true);
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

	private void ResetChongShengExp()
	{
		long num = 0L;
		if (Global.Data.roleData.RebornCount > 0)
		{
			RebornLevelVO rebornLevelVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornLevelVOByID(Global.Data.roleData.RebornLevel);
			if (rebornLevelVOByID != null)
			{
				num = (long)rebornLevelVOByID.MaxOfGoods;
			}
		}
		long num2 = Global.Data.roleData.MoneyData[149];
		long num3 = num - num2;
		if (num3 < 0L)
		{
			num3 = num;
		}
		string text = string.Concat(new object[]
		{
			Global.GetLang("回收上限 : "),
			num3,
			"/",
			num
		});
		if (num3 == num)
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				text
			});
		}
		this.expMaxNumText.text = text;
		this.expMaxNumText.text = string.Concat(new object[]
		{
			Global.GetLang("回收上限 : "),
			num3,
			"/",
			num
		});
		this.goExp.SetActive(true);
		this.goChongShengHuoBi.SetActive(false);
	}

	public void ResetMoenyData()
	{
		if (this.huiShouType == HuiShouType.EquipmentChongSheng)
		{
			this.TongqianNumText.text = "+0";
			this.SetPrice();
		}
		else if (this.huiShouType == HuiShouType.XuanCaiChongSheng || this.huiShouType == HuiShouType.BaoShiChongSheng)
		{
			this.SetPrice();
		}
	}

	private void SetPrice()
	{
		this.expNum = 0L;
		if (this.huiShouType == HuiShouType.EquipmentChongSheng)
		{
			for (int i = 0; i < this.goodsListBox.Count(); i++)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
				this.expNum += (long)Global.GetGoodsSaleToNpcChongShengExp(goodsData);
			}
			long num = 0L;
			if (Global.Data.roleData.RebornCount > 0)
			{
				RebornLevelVO rebornLevelVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornLevelVOByID(Global.Data.roleData.RebornLevel);
				if (rebornLevelVOByID != null)
				{
					num = (long)rebornLevelVOByID.MaxOfGoods;
				}
			}
			else
			{
				this.expNum = 0L;
			}
			long num2 = Global.Data.roleData.MoneyData[149];
			if (num2 > num)
			{
				num2 = 0L;
			}
			if (this.expNum > num2)
			{
				this.m_expFull = this.expNum - num2;
				this.expNum = num2;
			}
			else
			{
				this.m_expFull = 0L;
			}
			long num3 = num - (num2 - this.expNum);
			if (num3 < 0L)
			{
				num3 = num;
			}
			string text = string.Concat(new object[]
			{
				Global.GetLang("回收上限 : "),
				num3,
				"/",
				num
			});
			if (num3 == num)
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					text
				});
			}
			this.expMaxNumText.text = text;
			this.TongqianNumText.Text = " + {00FF00}" + this.expNum.ToString() + "{-}";
		}
		else if (this.huiShouType == HuiShouType.BaoShiChongSheng || this.huiShouType == HuiShouType.XuanCaiChongSheng)
		{
			long num4 = 0L;
			long num5 = 0L;
			long num6 = 0L;
			for (int j = 0; j < this.goodsListBox.Count(); j++)
			{
				GoodsData goodsData2 = U3DUtils.AS<GGoodIcon>(this.Items[j]).ItemObject as GoodsData;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
				num4 += (long)(goodsXmlNodeByID.ChangeFengYingJingShi * goodsData2.GCount);
				num5 += (long)(goodsXmlNodeByID.ChangeChongShengJingShi * goodsData2.GCount);
				num6 += (long)(goodsXmlNodeByID.ChangeXuanCaiJingShi * goodsData2.GCount);
			}
			this.fengYinJingShiNum.Text = string.Concat(new object[]
			{
				Global.GetRoleOwnNumByMoneyType(155),
				" + {00FF00}",
				num4.ToString(),
				"{-}"
			});
			this.chongShengJingShiNum.Text = string.Concat(new object[]
			{
				Global.GetRoleOwnNumByMoneyType(156),
				" + {00FF00}",
				num5.ToString(),
				"{-}"
			});
			this.xuanCaiJingShiNum.Text = string.Concat(new object[]
			{
				Global.GetRoleOwnNumByMoneyType(157),
				" + {00FF00}",
				num6.ToString(),
				"{-}"
			});
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
		List<GoodsData> chongShengGoodsDatas = ChongShengData.GetChongShengGoodsDatas();
		if (chongShengGoodsDatas == null)
		{
			return;
		}
		for (int i = 0; i < chongShengGoodsDatas.Count; i++)
		{
			GoodsData goodsData = chongShengGoodsDatas[i];
			if (goodsData != null)
			{
				if (goodsData.GCount > 0)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (this.huiShouType == HuiShouType.EquipmentChongSheng)
					{
						if (Super.CanSaleOutGoods(goodsData))
						{
							if (ChongShengData.IsChongShengEquip(categoriyByGoodsID))
							{
								if (goodsData.Using <= 0)
								{
									if (this.Items.Count >= 20)
									{
										break;
									}
									if (ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID) != null)
									{
										int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
										if (this.checkBox1.isChecked && zhuoyueAttributeCount < 5)
										{
											this.AddGGoodIcon(goodsData);
										}
										if (this.checkBox2.isChecked && zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount < 6)
										{
											this.AddGGoodIcon(goodsData);
										}
										if (this.checkBox3.isChecked && zhuoyueAttributeCount >= 6)
										{
											this.AddGGoodIcon(goodsData);
										}
									}
								}
							}
						}
					}
					else if (this.huiShouType == HuiShouType.BaoShiChongSheng)
					{
						if (this.Items.Count >= 20)
						{
							break;
						}
						if (Super.CanSaleOutGoods(goodsData))
						{
							if (Global.IsRebornBaoShi(categoriyByGoodsID))
							{
								if (goodsData.Using <= 0)
								{
									GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
									if (goodsXmlNodeByID != null)
									{
										if (this.checkBox1.isChecked && goodsXmlNodeByID.SuitID < 4)
										{
											this.AddGGoodIcon(goodsData);
										}
										if (this.checkBox2.isChecked && goodsXmlNodeByID.SuitID == 4)
										{
											this.AddGGoodIcon(goodsData);
										}
										if (this.checkBox3.isChecked && goodsXmlNodeByID.SuitID == 5)
										{
											this.AddGGoodIcon(goodsData);
										}
									}
								}
							}
						}
					}
					else if (this.huiShouType == HuiShouType.XuanCaiChongSheng && Global.IsRebornXuanCai(categoriyByGoodsID))
					{
						if (goodsData.Using <= 0)
						{
							if (this.Items.Count >= 20)
							{
								break;
							}
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							if (goodsXmlNodeByID2 != null)
							{
								int suitID = goodsXmlNodeByID2.SuitID;
								if (this.checkBox1.isChecked && suitID == 1)
								{
									this.AddGGoodIcon(goodsData);
								}
								if (this.checkBox2.isChecked && suitID == 2)
								{
									this.AddGGoodIcon(goodsData);
								}
								if (this.checkBox3.isChecked && suitID == 3)
								{
									this.AddGGoodIcon(goodsData);
								}
							}
						}
					}
				}
			}
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
			if (this.parcelChongShengPart != null)
			{
				Super.SetBgGIconShouStat(this.parcelChongShengPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), true);
			}
			Global.SetEquipGoodsZhanLiStat(icon, goodsData);
			this.SetPrice();
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.Items.Remove(icon.gameObject);
				if (this.parcelChongShengPart != null)
				{
					Super.SetBgGIconShouStat(this.parcelChongShengPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
				}
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
					if (this.parcelChongShengPart != null)
					{
						Super.SetBgGIconShouStat(this.parcelChongShengPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
					}
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

	public void AddChuShouGoods(GoodsData gd)
	{
		if (gd != null)
		{
			if (!Super.CanSaleOutGoods(gd))
			{
				Super.HintMainText(Global.GetLang("此物品不可出售！"), 10, 3);
			}
			else if (!this.FindGoodDataByID(gd.Id))
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
				if (this.parcelChongShengPart != null)
				{
					Super.SetBgGIconShouStat(this.parcelChongShengPart.FindGoodsIconByDbID(goodsData.Id), false);
				}
			}
		}
		Super.goodDBIdDict.Clear();
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
		List<GoodsData> chongShengGoodsDatas = ChongShengData.GetChongShengGoodsDatas();
		for (int i = 0; i < this.Items.Count; i++)
		{
			GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
			if (goodsData != null)
			{
				for (int j = 0; j < chongShengGoodsDatas.Count; j++)
				{
					if (chongShengGoodsDatas[j].Id == goodsData.Id)
					{
						break;
					}
					if (j >= chongShengGoodsDatas.Count - 1)
					{
						this.Items.RemoveAt(i);
					}
				}
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private long expNum;

	public HuiShouType huiShouType;

	public TextBlock TongqianNumText;

	public TextBlock expMaxNumText;

	public TextBlock BangdingJinBiText;

	public UISprite huoBiSprite;

	public UILabel huoBiType;

	public ListBox goodsListBox;

	public TextBlock TypeText;

	public GButton CloseBtn;

	public GButton ChuShouBtn;

	public GCheckBox checkBox1;

	public GCheckBox checkBox2;

	public GCheckBox checkBox3;

	public TextBlock fengYinJingShiNum;

	public TextBlock chongShengJingShiNum;

	public TextBlock xuanCaiJingShiNum;

	public GameObject goExp;

	public GameObject goChongShengHuoBi;

	public long m_expFull;

	[HideInInspector]
	public ParcelPart parcelChongShengPart;

	public GButton m_HuiShouBtn;

	public UIPanel m_PanelHuiShou;

	public GButton btnEquip;

	public GButton btnBaoShi;

	public GButton btnXuanCai;

	public UISprite m_SpFenGeXian;

	public UISprite m_SpFuMoBack;

	public GameObject m_GameOnBack;

	private List<GButton> m_ListFuMo = new List<GButton>();

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
