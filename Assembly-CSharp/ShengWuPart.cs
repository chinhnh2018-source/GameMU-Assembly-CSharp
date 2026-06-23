using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ShengWuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_JinbiCount.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.m_BangjinCount.text = Global.Data.roleData.Money1.ToString();
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"FCED00",
			Global.Data.roleData.CombatForce
		});
		this.RoleZhandouli.Text = colorStringForNGUIText;
		this.chengjiulv.pivot = 5;
		this.xiaohaojinbiwenben.pivot = 5;
		this.xiaohaosuipianwenben.pivot = 5;
		this.m_Suipianxiaohao.pivot = 3;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.MaxJie = (int)ConfigSystemParam.GetSystemParamIntByName("ShengWuMax");
		for (int i = 0; i < this.m_Bujianstype.Length; i++)
		{
			this.m_Bujianstype[i].text = string.Empty;
		}
		for (int j = 0; j < this.m_BujianJieshu.Length; j++)
		{
			this.m_BujianJieshu[j].text = string.Empty;
		}
		for (int k = 0; k < this.m_Selectbtn.Length; k++)
		{
			this.m_Selectbtn[k].SetActive(false);
		}
		int l = 0;
		int num = this.m_TiXing.Length;
		while (l < num)
		{
			this.m_TiXing[l].SetActive(false);
			l++;
		}
		this.SuiPianTeXiao.gameObject.SetActive(false);
		this.ModelTexiao.gameObject.SetActive(false);
		this.SetBtnStat(this.m_ShengBei, true);
		this.SetBtnStat(this.m_ShengGuan, false);
		this.SetBtnStat(this.m_Shengjian, false);
		this.SetBtnStat(this.m_ShengDian, false);
		this.NotifyShengWuData(1);
		this.ShowModle((int)this.m_stype);
		this.GetPartAttribute((int)this.m_stype, 1, this.m_SuitID[0]);
		this.ShengWuTip();
		this.m_Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.m_ShengBei.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if ((int)this.m_stype != 1)
			{
				this.InitShengWuStype(1);
			}
		};
		this.m_ShengGuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if ((int)this.m_stype != 2)
			{
				this.InitShengWuStype(2);
			}
		};
		this.m_Shengjian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if ((int)this.m_stype != 3)
			{
				this.InitShengWuStype(3);
			}
		};
		this.m_ShengDian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if ((int)this.m_stype != 4)
			{
				this.InitShengWuStype(4);
			}
		};
		this.m_BuJianBut[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 1)
			{
				this.InitSuitAttribute(1, 0);
			}
		};
		this.m_BuJianBut[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 2)
			{
				this.InitSuitAttribute(2, 1);
			}
		};
		this.m_BuJianBut[2].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 3)
			{
				this.InitSuitAttribute(3, 2);
			}
		};
		this.m_BuJianBut[3].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 4)
			{
				this.InitSuitAttribute(4, 3);
			}
		};
		this.m_BuJianBut[4].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 5)
			{
				this.InitSuitAttribute(5, 4);
			}
		};
		this.m_BuJianBut[5].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.check != 6)
			{
				this.InitSuitAttribute(6, 5);
			}
		};
		this.m_Jiacheng.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.JiaCheng_MouseLeftButtonUp);
		this.m_Zonglan.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Zonglan_MouseLeftButtonUp);
		this.m_Tisheng.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Tisheng_MouseLeftButtonUp);
		UIEventListener.Get(this.m_Bangzhu.gameObject).onClick = delegate(GameObject s)
		{
			this.m_BossModal.Show = false;
			GChildWindow window = U3DUtils.NEW<GChildWindow>();
			window.IsShowModal = true;
			window.Z = 200.0;
			ShengWuShuoMing shengWuShuoMing = U3DUtils.NEW<ShengWuShuoMing>();
			Super.InitChildWindow(window, "FallItemsWindow");
			Super.GData.GlobalPlayZone.Children.Add(window);
			window.ModalType = ChildWindowModalType.Translucent;
			window.SetContent(window.BodyPresenter, shengWuShuoMing, 0.0, 0.0, true);
			window.ChildWindowClose = delegate(object a, EventArgs e)
			{
				Super.CloseChildWindow(this, window);
				return true;
			};
			shengWuShuoMing.DPSelectedItem = delegate(object sender, DPSelectedItemEventArgs e1)
			{
				this.m_BossModal.Show = true;
				Super.CloseChildWindow(this, window);
			};
		};
	}

	private void InitShengWuStype(int index)
	{
		if (index == 0)
		{
			return;
		}
		this.SetBtnStat(this.m_ShengBei, index == 1);
		this.SetBtnStat(this.m_ShengGuan, index == 2);
		this.SetBtnStat(this.m_Shengjian, index == 3);
		this.SetBtnStat(this.m_ShengDian, index == 4);
		this.modeltexiao = 0;
		this.ResGetHolyData();
		this.ResGetPartData();
		this.NotifyShengWuData((sbyte)index);
		this.ShowModle((int)this.m_stype);
		this.GetPartAttribute((int)this.m_stype, 1, this.m_SuitID[0]);
	}

	private void InitSuitAttribute(int index, int suitID)
	{
		if (this.m_SuitID.Count <= 0)
		{
			return;
		}
		this.ResGetPartData();
		this.GetPartAttribute((int)this.m_stype, index, this.m_SuitID[suitID]);
		this.check = index;
	}

	private void Zonglan_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.m_BossModal.Show = false;
		GChildWindow window = U3DUtils.NEW<GChildWindow>();
		window.IsShowModal = true;
		window.Z = 200.0;
		ShuXingZongLan shuXingZongLan = U3DUtils.NEW<ShuXingZongLan>();
		Super.InitChildWindow(window, "FallItemsWindow");
		Super.GData.GlobalPlayZone.Children.Add(window);
		window.ModalType = ChildWindowModalType.Translucent;
		window.SetContent(window.BodyPresenter, shuXingZongLan, 0.0, 0.0, true);
		shuXingZongLan.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e1)
		{
			this.m_BossModal.Show = true;
			Super.CloseChildWindow(this, window);
		};
		switch ((int)this.m_stype)
		{
		case 1:
			shuXingZongLan.Title1.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("圣杯属性总览")
			}));
			break;
		case 2:
			shuXingZongLan.Title1.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("圣冠属性总览")
			}));
			break;
		case 3:
			shuXingZongLan.Title1.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("圣剑属性总览")
			}));
			break;
		case 4:
			shuXingZongLan.Title1.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("圣典属性总览")
			}));
			break;
		}
		this.LinShiCunChu.Clear();
		for (int i = 0; i < this.ShuXingZongLan.Count; i++)
		{
			this.LinShiCunChu.Add(this.ShuXingZongLan[i]);
		}
		int num = 0;
		int num2 = 0;
		while (!this.LinShiCunChu.IsNullOrEmpty<string>())
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = this.LinShiCunChu[0].Split(new char[]
			{
				':'
			})[0];
			int num3 = int.Parse(this.LinShiCunChu[0].Split(new char[]
			{
				':'
			})[1]);
			this.LinShiCunChu.RemoveAt(0);
			for (int j = 0; j < this.LinShiCunChu.Count; j++)
			{
				if (text.Equals(this.LinShiCunChu[j].Split(new char[]
				{
					':'
				})[0]))
				{
					num3 += int.Parse(this.LinShiCunChu[j].Split(new char[]
					{
						':'
					})[1]);
					this.LinShiCunChu.RemoveAt(j);
					j--;
				}
			}
			text2 = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				text
			})) + string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				" :  "
			})) + string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				num3
			}));
			shuXingZongLan.Shuzhi[num2].transform.gameObject.SetActive(true);
			shuXingZongLan.Shuzhi[num2].text = text2;
			num2++;
			num++;
		}
	}

	private void JiaCheng_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.m_BossModal.Show = false;
		GChildWindow window = U3DUtils.NEW<GChildWindow>();
		window.IsShowModal = true;
		window.Z = 200.0;
		EWaiJiaCheng ewaiJiaCheng = U3DUtils.NEW<EWaiJiaCheng>();
		Super.InitChildWindow(window, "FallItemsWindow");
		Super.GData.GlobalPlayZone.Children.Add(window);
		window.ChildWindowClose = delegate(object a, EventArgs b)
		{
			Super.CloseChildWindow(this, window);
			return true;
		};
		window.ModalType = ChildWindowModalType.Translucent;
		window.SetContent(window.BodyPresenter, ewaiJiaCheng, 0.0, 0.0, true);
		ewaiJiaCheng.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e1)
		{
			this.m_BossModal.Show = true;
			Super.CloseChildWindow(this, window);
		};
		string text = string.Empty;
		switch ((int)this.m_stype)
		{
		case 1:
			ewaiJiaCheng.Title.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("圣杯额外加成")
			}));
			break;
		case 2:
			ewaiJiaCheng.Title.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("圣冠额外加成")
			}));
			break;
		case 3:
			ewaiJiaCheng.Title.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("圣剑额外加成")
			}));
			break;
		case 4:
			ewaiJiaCheng.Title.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("圣典额外加成")
			}));
			break;
		}
		if (this.m_maxsuit == 0)
		{
			ewaiJiaCheng.Curtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("当前效果 ：无")
			});
		}
		else
		{
			ewaiJiaCheng.Curtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(Global.GetLang("当前效果 ：所有部件达到{0}阶"), this.m_maxsuit)
			});
		}
		if (this.m_maxsuit == this.MaxJie)
		{
			ewaiJiaCheng.Lowtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下级效果 ：无")
			});
		}
		else
		{
			ewaiJiaCheng.Lowtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下级效果 ：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format(Global.GetLang("所有部件达到{0}阶"), this.m_maxsuit + 1)
			});
		}
		XElement gameResXml = Global.GetGameResXml("Config/ExtPropIndexes.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ExtPropIndexes");
		if (!this.m_ewaishuxing.Equals("-1"))
		{
			for (int i = 0; i < this.m_ewaishuxing.Split(new char[]
			{
				'|'
			}).Length; i++)
			{
				string text2 = this.m_ewaishuxing.Split(new char[]
				{
					'|'
				})[i].Split(new char[]
				{
					','
				})[0];
				for (int j = 0; j < xelementList.Count; j++)
				{
					XElement xelement = xelementList[j];
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Word");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Description");
					if (text2.ToUpper().Equals(xelementAttributeStr.ToUpper()))
					{
						if (this.m_ewaishuxing.Split(new char[]
						{
							'|'
						}).Length == 1)
						{
							if (float.Parse(this.m_ewaishuxing.Split(new char[]
							{
								','
							})[1]) < 10f)
							{
								text = string.Concat(new object[]
								{
									xelementAttributeStr2,
									": + ",
									float.Parse(this.m_ewaishuxing.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								});
							}
							else
							{
								text = xelementAttributeStr2 + ": + " + this.m_ewaishuxing.Split(new char[]
								{
									','
								})[1];
							}
						}
						else if (float.Parse(this.m_ewaishuxing.Split(new char[]
						{
							'|'
						})[i].Split(new char[]
						{
							','
						})[1]) < 10f)
						{
							text = string.Concat(new object[]
							{
								xelementAttributeStr2,
								": + ",
								float.Parse(this.m_ewaishuxing.Split(new char[]
								{
									'|'
								})[i].Split(new char[]
								{
									','
								})[1]) * 100f,
								"%"
							});
						}
						else
						{
							text = xelementAttributeStr2 + ": + " + this.m_ewaishuxing.Split(new char[]
							{
								'|'
							})[i].Split(new char[]
							{
								','
							})[1];
						}
						ewaiJiaCheng.wenben1[i].text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							text
						}));
					}
				}
			}
		}
		if (!this.m_nextewaijiacheng.Equals("-1"))
		{
			for (int k = 0; k < this.m_nextewaijiacheng.Split(new char[]
			{
				'|'
			}).Length; k++)
			{
				string text3 = this.m_nextewaijiacheng.Split(new char[]
				{
					'|'
				})[k].Split(new char[]
				{
					','
				})[0];
				for (int l = 0; l < xelementList.Count; l++)
				{
					XElement xelement2 = xelementList[l];
					string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "Word");
					string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "Description");
					if (text3.ToUpper().Equals(xelementAttributeStr3.ToUpper()))
					{
						if (this.m_nextewaijiacheng.Split(new char[]
						{
							'|'
						}).Length == 1)
						{
							if (float.Parse(this.m_nextewaijiacheng.Split(new char[]
							{
								','
							})[1]) < 10f)
							{
								text = string.Concat(new object[]
								{
									xelementAttributeStr4,
									": + ",
									float.Parse(this.m_nextewaijiacheng.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								});
							}
							else
							{
								text = xelementAttributeStr4 + ": + " + this.m_nextewaijiacheng.Split(new char[]
								{
									','
								})[1];
							}
						}
						else if (float.Parse(this.m_nextewaijiacheng.Split(new char[]
						{
							'|'
						})[k].Split(new char[]
						{
							','
						})[1]) < 10f)
						{
							text = string.Concat(new object[]
							{
								xelementAttributeStr4,
								": + ",
								float.Parse(this.m_nextewaijiacheng.Split(new char[]
								{
									'|'
								})[k].Split(new char[]
								{
									','
								})[1]) * 100f,
								"%"
							});
						}
						else
						{
							text = xelementAttributeStr4 + ": + " + this.m_nextewaijiacheng.Split(new char[]
							{
								'|'
							})[k].Split(new char[]
							{
								','
							})[1];
						}
						ewaiJiaCheng.wenben2[k].text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							text
						}));
					}
				}
			}
		}
	}

	private void ResGetPartData()
	{
		for (int i = 0; i < this.m_BujianShuXing.Length; i++)
		{
			this.m_BujianShuXing[i].text = string.Empty;
		}
		for (int j = 0; j < this.m_BuJian[6].transform.childCount; j++)
		{
			GameObject gameObject = this.m_BuJian[6].transform.GetChild(j).gameObject;
			Object.Destroy(gameObject);
		}
		for (int k = 0; k < this.m_Selectbtn.Length; k++)
		{
			this.m_Selectbtn[k].SetActive(false);
		}
		this.m_needgoods = string.Empty;
		this.m_failcost = string.Empty;
		this.costbandjinbi = 0;
		this.currentSlot = 0;
	}

	private void ResGetHolyData()
	{
		int i = 0;
		int num = this.m_TiXing.Length;
		while (i < num)
		{
			this.m_TiXing[i].SetActive(false);
			i++;
		}
		for (int j = 0; j < this.m_BuJian.Length - 1; j++)
		{
			for (int k = 0; k < this.m_BuJian[j].transform.childCount; k++)
			{
				GameObject gameObject = this.m_BuJian[j].transform.GetChild(k).gameObject;
				Object.Destroy(gameObject);
			}
		}
		this.m_SuitID.Clear();
		this.m_stype = 0;
		this.ShuXingZongLan.Clear();
		this.check = -1;
		this.CurrentBujianID.Clear();
		this.m_suipianCount.Clear();
	}

	private void Tisheng_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.SuiPianTeXiao.gameObject.SetActive(false);
		this.ModelTexiao.gameObject.SetActive(false);
		if (this.currentSuit == this.MaxJie)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("当前部件已达到最高阶！"), 0, -1, -1, 0);
			return;
		}
		if (this.costbandjinbi != -1)
		{
			string text = this.m_needgoods.Split(new char[]
			{
				','
			})[1];
			if (this.m_suipianCount[(int)this.currentSlot - 1] < int.Parse(text))
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedSuiPian, null, string.Empty, Global.GetLang("碎片"));
				return;
			}
			if (!string.IsNullOrEmpty(this.mNeedItem))
			{
				string[] array = this.mNeedItem.Split(new char[]
				{
					','
				});
				if (array.Length >= 2)
				{
					int goodsID = Global.SafeConvertToInt32(array[0]);
					int num = Global.SafeConvertToInt32(array[1]);
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
					if (totalGoodsCountByID < num)
					{
						Super.HintMainText(Global.GetLang("物品不足"), 10, 3);
						return;
					}
				}
			}
			int num2 = Global.GetRoleOwnNumByMoneyType(8) + Global.Data.roleData.Money1;
			if (num2 < this.costbandjinbi)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, Global.GetLang("金币"));
				return;
			}
		}
		GameInstance.Game.GetShengWuInfo(this.m_stype, this.currentSlot);
	}

	private void GetPartAttribute(int nType, int nSlot, int nSuit)
	{
		this.currentSlot = (sbyte)nSlot;
		this.m_Selectbtn[nSlot - 1].SetActive(true);
		this.currentSuit = nSuit;
		int bujianID = Global.GetBujianID(nType, nSlot, nSuit);
		string color = Global.dic_holyPartAttr[bujianID].Color;
		string picture = Global.dic_holyPartAttr[bujianID].Picture;
		string text = string.Empty;
		string text2 = string.Empty;
		if (Global.dic_holyPartAttr[bujianID].SuitID == 0)
		{
			this.m_Tishenglab.text = Global.GetLang("激活");
		}
		else
		{
			this.m_Tishenglab.text = Global.GetLang("进阶");
		}
		text2 = Global.dic_holyPartAttr[bujianID].Property;
		float successProbability = Global.dic_holyPartAttr[bujianID].SuccessProbability;
		this.m_needgoods = Global.dic_holyPartAttr[bujianID].NeedGoods;
		this.m_failcost = Global.dic_holyPartAttr[bujianID].FailCost;
		this.costbandjinbi = Global.dic_holyPartAttr[bujianID].CostBandJianBi;
		this.mNeedItem = Global.dic_holyPartAttr[bujianID].NeedItem;
		this.mFailureConsumption = Global.dic_holyPartAttr[bujianID].FailureConsumption;
		this.m_Bujianstype[6].text = Global.GetColorStringForNGUIText(new object[]
		{
			string.Format("{0}", color),
			Global.dic_holyPartAttr[bujianID].name
		});
		this.m_BujianJieshu[6].text = Global.GetColorStringForNGUIText(new object[]
		{
			string.Format("{0}", color),
			string.Format(Global.GetLang("{0}阶"), Global.dic_holyPartAttr[bujianID].SuitID)
		});
		if (this.m_needgoods.Equals("-1"))
		{
			this.m_Chengjiulv1.transform.parent.gameObject.SetActive(false);
			this.m_Xiaohaojinbi1.transform.parent.gameObject.SetActive(false);
			this.m_Suipianxiaohao.transform.parent.gameObject.SetActive(false);
			this.GoodIconObj.SetActive(false);
			this.m_Tisheng.gameObject.SetActive(false);
			this.Maxdengji.gameObject.SetActive(true);
			this.GoodIconObj.SetActive(false);
			this.Maxdengji.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("已达到最高级别！")
			});
		}
		else
		{
			this.m_Chengjiulv1.transform.parent.gameObject.SetActive(true);
			this.m_Xiaohaojinbi1.transform.parent.gameObject.SetActive(true);
			this.m_Suipianxiaohao.transform.parent.gameObject.SetActive(true);
			this.m_Tisheng.gameObject.SetActive(true);
			this.Maxdengji.gameObject.SetActive(false);
			this.GoodIconObj.SetActive(true);
			this.chengjiulv.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("成功率：")
			});
			this.xiaohaojinbiwenben.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("消耗金币：")
			});
			this.m_Chengjiulv1.text = successProbability * 100f + "%";
			int num = Global.GetRoleOwnNumByMoneyType(8) + Global.Data.roleData.Money1;
			if (num < this.costbandjinbi)
			{
				this.m_Xiaohaojinbi1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"EA0C0C",
					this.costbandjinbi.ToString()
				});
			}
			else
			{
				this.m_Xiaohaojinbi1.text = this.costbandjinbi.ToString();
			}
			this.xiaohaosuipianwenben.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("消耗")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("【{0}碎片】{1}"), Global.dic_holyPartAttr[bujianID].name, Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(":")
				}))
			});
			if (this.m_suipianCount[nSlot - 1] < this.m_needgoods.Split(new char[]
			{
				','
			})[1].SafeToInt32(0))
			{
				this.m_Suipianxiaohao.text = Global.GetColorStringForNGUIText(new object[]
				{
					"EA0C0C",
					this.m_suipianCount[nSlot - 1] + "/" + this.m_needgoods.Split(new char[]
					{
						','
					})[1]
				});
			}
			else
			{
				this.m_Suipianxiaohao.text = this.m_suipianCount[nSlot - 1] + "/" + this.m_needgoods.Split(new char[]
				{
					','
				})[1];
			}
			string goodItemName = this.GetGoodItemName(Global.dic_holyPartAttr[bujianID].NeedItem);
			if (!string.IsNullOrEmpty(goodItemName))
			{
				this.mLblGoodsName.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("消耗")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}碎片】{1}"), goodItemName, Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang(":")
					}))
				});
			}
			else
			{
				this.mLblGoodsName.text = string.Empty;
			}
			this.mLblGoodsCount.Text = this.GetCountDes(Global.dic_holyPartAttr[bujianID].NeedItem);
		}
		for (int i = 0; i < text2.Split(new char[]
		{
			'|'
		}).Length; i++)
		{
			for (int j = 0; j < ExtPropIndexes.ShengWuIndexNames.Length; j++)
			{
				string text3 = text2.Split(new char[]
				{
					'|'
				})[i].Split(new char[]
				{
					','
				})[0];
				if (ExtPropIndexes.ShengWuIndexNames[j].ToUpper() == text3.ToUpper())
				{
					text = ExtPropIndexes.ShengWuChineseNames[j] + "  :  " + text2.Split(new char[]
					{
						'|'
					})[i].Split(new char[]
					{
						','
					})[1];
					this.m_BujianShuXing[i].text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
					{
						"E3cca3",
						text
					}));
				}
			}
		}
		this.GGoodIconAdd(bujianID, 6);
	}

	private string GetGoodItemName(string goodId)
	{
		if (!string.IsNullOrEmpty(goodId))
		{
			string[] array = goodId.Split(new char[]
			{
				','
			});
			if (array.Length >= 2)
			{
				int id = Global.SafeConvertToInt32(array[0]);
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
				if (goodsXmlNodeByID != null)
				{
					return goodsXmlNodeByID.Title;
				}
			}
		}
		return string.Empty;
	}

	private string GetCountDes(string goodId)
	{
		if (!string.IsNullOrEmpty(goodId))
		{
			string[] array = goodId.Split(new char[]
			{
				','
			});
			if (array.Length >= 2)
			{
				int goodsID = Global.SafeConvertToInt32(array[0]);
				int num = Global.SafeConvertToInt32(array[1]);
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (totalGoodsCountByID < num)
				{
					return Global.GetColorStringForNGUIText(new object[]
					{
						"EA0C0C",
						totalGoodsCountByID + "/" + num
					});
				}
				return totalGoodsCountByID + "/" + num;
			}
		}
		return string.Empty;
	}

	public void ShengWuTip()
	{
		for (int i = 0; i < this.m_TiShi.Length; i++)
		{
			this.m_TiShi[i].SetActive(false);
		}
		for (int j = 1; j < 5; j++)
		{
			HolyItemData holyItemData = null;
			if (Global.dic_holyItem.ContainsKey((sbyte)j))
			{
				Global.dic_holyItem.TryGetValue((sbyte)j, ref holyItemData);
				if (holyItemData != null)
				{
					Global.m_PartArray = holyItemData.m_PartArray;
				}
			}
			for (int k = 0; k < 6; k++)
			{
				HolyItemPartData value = Global.m_PartArray.GetValue((sbyte)(k + 1));
				int bujianID = Global.GetBujianID(j, k + 1, (int)value.m_sSuit);
				int num;
				if (!Global.dic_holyPartAttr[bujianID].NeedGoods.Equals("-1"))
				{
					num = int.Parse(Global.dic_holyPartAttr[bujianID].NeedGoods.Split(new char[]
					{
						','
					})[1]);
				}
				else
				{
					num = 0;
				}
				int costBandJianBi = Global.dic_holyPartAttr[bujianID].CostBandJianBi;
				int num2 = Global.Data.roleData.Money1 + Global.GetRoleOwnNumByMoneyType(8);
				int num3 = (int)value.m_sSuit;
				if (!string.IsNullOrEmpty(Global.dic_holyPartAttr[bujianID].NeedItem))
				{
					string[] array = Global.dic_holyPartAttr[bujianID].NeedItem.Split(new char[]
					{
						','
					});
					int num4 = int.Parse(array[1]);
					int goodsID = int.Parse(array[0]);
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
					if (num <= value.m_nSlice && costBandJianBi <= num2 && num3 < this.MaxJie - 1 && num4 <= totalGoodsCountByID)
					{
						this.m_TiShi[j - 1].SetActive(true);
					}
				}
				else if (num <= value.m_nSlice && costBandJianBi <= num2 && num3 < this.MaxJie - 1)
				{
					this.m_TiShi[j - 1].SetActive(true);
				}
			}
		}
	}

	private void NotifyShengWuData(sbyte stype)
	{
		this.m_stype = stype;
		HolyItemData holyItemData = null;
		if (Global.dic_holyItem != null && Global.dic_holyItem.Keys.Count > 0 && Global.dic_holyItem.ContainsKey(stype))
		{
			Global.dic_holyItem.TryGetValue(stype, ref holyItemData);
			if (holyItemData != null)
			{
				Global.m_PartArray = holyItemData.m_PartArray;
			}
		}
		sbyte b = 0;
		while ((int)b < this.m_BuJian.Length - 1)
		{
			HolyItemPartData value = Global.m_PartArray.GetValue((sbyte)((int)b + 1));
			int bujianID = Global.GetBujianID((int)stype, (int)b + 1, (int)value.m_sSuit);
			this.CurrentBujianID.Add(bujianID);
			Global.GetHolyPartByID(bujianID);
			string property = Global.dic_holyPartAttr[bujianID].Property;
			string text = string.Empty;
			string color = Global.dic_holyPartAttr[bujianID].Color;
			int num = 0;
			if (!Global.dic_holyPartAttr[bujianID].NeedGoods.Equals("-1"))
			{
				num = int.Parse(Global.dic_holyPartAttr[bujianID].NeedGoods.Split(new char[]
				{
					','
				})[1]);
			}
			int costBandJianBi = Global.dic_holyPartAttr[bujianID].CostBandJianBi;
			this.m_suipianCount.Add(value.m_nSlice);
			this.m_Bujianstype[(int)b].text = Global.GetColorStringForNGUIText(new object[]
			{
				string.Format("{0}", color),
				Global.dic_holyPartAttr[bujianID].name
			});
			this.m_BujianJieshu[(int)b].text = Global.GetColorStringForNGUIText(new object[]
			{
				string.Format("{0}", color),
				string.Format(Global.GetLang("{0}阶"), Global.dic_holyPartAttr[bujianID].SuitID)
			});
			this.m_SuitID.Add(Global.dic_holyPartAttr[bujianID].SuitID);
			if (!property.Equals("-1"))
			{
				for (int i = 0; i < property.Split(new char[]
				{
					'|'
				}).Length; i++)
				{
					for (int j = 0; j < ExtPropIndexes.ShengWuIndexNames.Length; j++)
					{
						string text2 = property.Split(new char[]
						{
							'|'
						})[i].Split(new char[]
						{
							','
						})[0];
						if (ExtPropIndexes.ShengWuIndexNames[j].ToUpper() == text2.ToUpper())
						{
							text = ExtPropIndexes.ShengWuChineseNames[j] + "  :  " + property.Split(new char[]
							{
								'|'
							})[i].Split(new char[]
							{
								','
							})[1];
							this.ShuXingZongLan.Add(text);
						}
					}
				}
			}
			int num2 = Global.Data.roleData.Money1 + Global.GetRoleOwnNumByMoneyType(8);
			int num3 = (int)value.m_sSuit;
			if (!string.IsNullOrEmpty(Global.dic_holyPartAttr[bujianID].NeedItem))
			{
				string[] array = Global.dic_holyPartAttr[bujianID].NeedItem.Split(new char[]
				{
					','
				});
				int num4 = int.Parse(array[1]);
				int goodsID = int.Parse(array[0]);
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (num <= value.m_nSlice && costBandJianBi <= num2 && num3 < this.MaxJie - 1 && num4 <= totalGoodsCountByID)
				{
					this.m_TiXing[(int)b].SetActive(true);
				}
			}
			else if (num <= value.m_nSlice && costBandJianBi <= num2 && num3 < this.MaxJie - 1)
			{
				this.m_TiXing[(int)b].SetActive(true);
			}
			this.GGoodIconAdd(bujianID, (int)b);
			b += 1;
		}
	}

	private void GGoodIconAdd(int BuJianID, int i)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.transform.parent = this.m_BuJian[i].transform;
		ggoodIcon.transform.localPosition = new Vector3(0f, 0f, 0f);
		ggoodIcon.transform.localScale = new Vector3(1f, 1f, 0f);
		ggoodIcon.Width = 64.0;
		ggoodIcon.Height = 64.0;
		string picture = Global.dic_holyPartAttr[BuJianID].Picture;
		ggoodIcon.BodyURL = new ImageURL(string.Format(Global.GetLang("NetImages/GameRes/Images/ShengWu/{0}.png"), picture), false, 0);
		if (Global.dic_holyPartAttr[BuJianID].SuitID == 0)
		{
			ggoodIcon.GoodImg.GetComponent<UITexture>().shader = Shader.Find("Unlit/Gray");
		}
		if (Global.dic_holyPartAttr[BuJianID].SuitID == 2 || Global.dic_holyPartAttr[BuJianID].SuitID == 3)
		{
			ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
		}
		else if (Global.dic_holyPartAttr[BuJianID].SuitID == 4 || Global.dic_holyPartAttr[BuJianID].SuitID == 5)
		{
			ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
		}
		else if (Global.dic_holyPartAttr[BuJianID].SuitID == 6 || Global.dic_holyPartAttr[BuJianID].SuitID == 7)
		{
			ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
		}
		else if (Global.dic_holyPartAttr[BuJianID].SuitID == 8 || Global.dic_holyPartAttr[BuJianID].SuitID == 9)
		{
			ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			ggoodIcon.TeXiao.gameObject.SetActive(true);
		}
		else if (9 < Global.dic_holyPartAttr[BuJianID].SuitID && Global.dic_holyPartAttr[BuJianID].SuitID <= this.MaxJie)
		{
			ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			ggoodIcon.TeXiao.gameObject.SetActive(true);
		}
	}

	private int GetMinSuitID()
	{
		if (this.m_SuitID.Count <= 0)
		{
			return 0;
		}
		int num = this.m_SuitID[0];
		for (int i = 1; i < this.m_SuitID.Count; i++)
		{
			if (this.m_SuitID[i] < num)
			{
				num = this.m_SuitID[i];
			}
		}
		return num;
	}

	private int GetMOdelID(int data)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShengWu.xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		int minSuitID = this.GetMinSuitID();
		this.m_maxsuit = minSuitID;
		this.currentID = Global.GetShengwuID(minSuitID, data);
		int result = 0;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (this.currentID == xelement.AttributeInt("ID"))
			{
				result = xelement.AttributeInt("ResName");
			}
			if (data == xelement.AttributeInt("Tyep"))
			{
				int num = this.m_maxsuit + 1;
				if (num <= this.MaxJie)
				{
					if (num == xelement.AttributeInt("BuJianSuit"))
					{
						this.m_nextewaijiacheng = xelement.AttributeStr("ExtraProperty");
					}
				}
				else if (this.m_maxsuit == xelement.AttributeInt("BuJianSuit"))
				{
					this.m_nextewaijiacheng = "-1";
				}
				if (this.m_maxsuit == xelement.AttributeInt("BuJianSuit"))
				{
					this.m_ewaishuxing = xelement.AttributeStr("ExtraProperty");
				}
			}
		}
		return result;
	}

	private void ShowModle(int data)
	{
		int minSuitID = this.GetMinSuitID();
		if (minSuitID == this.desmodel && this.desmodeltype == data)
		{
			return;
		}
		base.StartCoroutine<bool>(this.Load3DModel(data));
	}

	public override void Destroy()
	{
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
			this.resourceLoader = null;
		}
		base.Destroy();
	}

	private IEnumerator Load3DModel(int data)
	{
		int m_minSuitID = this.GetMinSuitID();
		this.desmodel = m_minSuitID;
		this.desmodeltype = data;
		int modelID = this.GetMOdelID(data);
		this.ModalDestory();
		this.m_BossModal = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.holy3DModel, this.m_BossModal.gameObject, false);
		Transform trans = this.m_BossModal.transform;
		trans.localPosition = new Vector3(-135f, -125f, 0f);
		trans.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(this.m_BossModal.transform);
		this.modeltexiao++;
		this.PlayUpgradeEffect();
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
		this.resourceLoader = UIHelper.LoadModelResource(this.m_BossModal, modelID, 1f, delegate(object s, DPSelectedItemEventArgs e)
		{
			if (m_minSuitID == 0)
			{
				UIModel_Type componentInChildren = (s as GameObject).GetComponentInChildren<UIModel_Type>();
				if (null != componentInChildren)
				{
					int num = componentInChildren.effect.Length;
					if (num != 0)
					{
						for (int i = 0; i < num; i++)
						{
							componentInChildren.effect[i].SetActive(false);
						}
					}
					int num2 = componentInChildren.uiModel.Length;
					if (num2 != 0)
					{
						Shader shader = Shader.Find("Custom/ViewSpecular - Gray");
						for (int j = 0; j < num2; j++)
						{
							int num3 = componentInChildren.uiModel[j].GetComponent<SkinnedMeshRenderer>().materials.Length;
							for (int k = 0; k < num3; k++)
							{
								componentInChildren.uiModel[j].GetComponent<SkinnedMeshRenderer>().materials[k].shader = shader;
							}
						}
					}
				}
			}
		});
		yield return null;
		yield break;
	}

	private void ModalDestory()
	{
		if (null != this.m_BossModal)
		{
			Object.Destroy(this.m_BossModal.gameObject);
			this.m_BossModal = null;
		}
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	public void PartInfo(int stype, int part, int suit)
	{
		this.m_JinbiCount.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.m_BangjinCount.text = Global.Data.roleData.Money1.ToString();
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"FCED00",
			Global.Data.roleData.CombatForce
		});
		this.RoleZhandouli.Text = colorStringForNGUIText;
		this.ResGetPartData();
		this.ResGetHolyData();
		this.NotifyShengWuData((sbyte)stype);
		this.GetPartAttribute(stype, part, suit);
		this.ShengWuTip();
	}

	public void TiShengHuiDiao(int data)
	{
		if (data == 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("该功能还没有开启！"), 0, -1, -1, 0);
			return;
		}
		if (data == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("非常规出错！"), 0, -1, -1, 0);
			return;
		}
		if (data == 2)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, Global.GetLang("金币"));
			return;
		}
		if (data == 3)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedSuiPian, null, string.Empty, Global.GetLang("碎片"));
			return;
		}
		if (data == 4)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要升级的部件已经满级！"), 0, -1, -1, 0);
			return;
		}
		if (data == 6)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("物品不足"), 0, -1, -1, 0);
			return;
		}
		if (data == 0)
		{
			this.SuiPianTeXiao.localPosition = new Vector3(this.SuipianTeXiaoPoint[(int)this.currentSlot - 1].localPosition.x, this.SuipianTeXiaoPoint[(int)this.currentSlot - 1].localPosition.y - 26f, this.SuipianTeXiaoPoint[(int)this.currentSlot - 1].localPosition.z - 10f);
			this.SuiPianTeXiao.gameObject.SetActive(true);
			this.ShowModle((int)this.m_stype);
			this._LianluAnim[0].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[0], new ActiveAnimation.OnFinished(this.PlayFinished));
			return;
		}
		if (data == 1)
		{
			this._LianluAnim[1].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[1], new ActiveAnimation.OnFinished(this.PlayFinished));
			return;
		}
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	private void PlayUpgradeEffect()
	{
		if (this.modeltexiao > 1)
		{
			this.ModelTexiao.localPosition = new Vector3(-135f, -20f, -700f);
			this.ModelTexiao.gameObject.SetActive(false);
			this.ModelTexiao.gameObject.SetActive(true);
		}
	}

	public GButton m_ShengBei;

	public GButton m_ShengGuan;

	public GButton m_Shengjian;

	public GButton m_ShengDian;

	public GButton m_Tisheng;

	public UIButton m_Bangzhu;

	public GButton m_Jiacheng;

	public GButton m_Zonglan;

	public GButton m_Close;

	public GButton[] m_BuJianBut;

	public GameObject[] m_BuJian;

	public GameObject m_Shuomingweizhi;

	public UILabel[] m_Bujianstype;

	public UILabel[] m_BujianJieshu;

	public GameObject[] m_TiXing;

	public GameObject[] m_TiShi;

	public GameObject[] m_Selectbtn;

	public UILabel m_Chengjiulv1;

	public UILabel m_Xiaohaojinbi1;

	public UILabel m_Suipianxiaohao;

	public UILabel m_Tishenglab;

	public UILabel[] m_BujianShuXing;

	public UILabel m_JinbiCount;

	public UILabel m_BangjinCount;

	public UILabel chengjiulv;

	public UILabel xiaohaojinbiwenben;

	public UILabel xiaohaosuipianwenben;

	public UILabel Maxdengji;

	public Transform SuiPianTeXiao;

	public Transform[] SuipianTeXiaoPoint;

	public Transform ModelTexiao;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Animation[] _LianluAnim;

	public GameObject TanKuangWeiZhi;

	public GameObject holy3DModel;

	private Modal3DShow m_BossModal;

	public string m_needgoods = string.Empty;

	public string m_failcost = string.Empty;

	public string m_ewaishuxing = string.Empty;

	public string m_nextewaijiacheng = string.Empty;

	public List<int> m_suipianCount = new List<int>();

	public int costbandjinbi;

	public int currentID;

	public int m_maxsuit;

	public TextBlock RoleZhandouli;

	public GameObject GoodIconObj;

	public TextBlock mLblGoodsCount;

	public TextBlock mLblGoodsName;

	public GGoodIcon mGoodIcon;

	public string mNeedItem;

	public string mFailureConsumption;

	private List<string> LinShiCunChu = new List<string>();

	private List<string> ShuXingZongLan = new List<string>();

	private List<int> m_SuitID = new List<int>();

	private List<int> CurrentBujianID = new List<int>();

	private sbyte m_stype;

	private sbyte currentSlot;

	private int currentSuit;

	private int check;

	private int desmodel = -1;

	private int desmodeltype = -1;

	private int modeltexiao;

	private int MaxJie = -1;

	private ResourceResLoader resourceLoader;
}
