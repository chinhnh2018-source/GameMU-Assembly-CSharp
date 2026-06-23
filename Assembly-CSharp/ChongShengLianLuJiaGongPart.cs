using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ChongShengLianLuJiaGongPart : UserControl
{
	private int ChongShengNumber
	{
		get
		{
			return this.m_ChongShengNumber;
		}
		set
		{
			this.m_ChongShengNumber = value;
			this.m_BtnChongShengNumber.Text = this.m_ChongShengNumber.ToString();
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		this.InitPage(BaoShiType.XuanCai);
		this.RefreshCuiPian();
		this.m_VecXuanCaiLevel = this.m_SpringXuanCai.transform.localPosition;
		this.m_VecXuanCaiEquip = this.m_DragBaoShiPanel.transform.localPosition;
		this.m_PanelNumber.gameObject.SetActive(false);
		this.m_Obser = this.m_ListBox.ItemsSource;
		this.m_ObserXuanCai = this.m_ListBoxXuanCaiEquip.ItemsSource;
		this.m_ListBoxXuanCaiEquip.SelectionChanged = new MouseLeftButtonUpEventHandler(this.EquipOnClick);
		this.m_ListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectionChange);
		this.InitOnclick();
	}

	private void RefreshCuiPian()
	{
		this.m_LabFengYinJingShiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetRoleOwnNumByMoneyType(155)
		});
		this.m_LabChongShengJingShiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetRoleOwnNumByMoneyType(156)
		});
		this.m_LabXuanCaiJingShiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetRoleOwnNumByMoneyType(157)
		});
	}

	private void InitOnclick()
	{
		this.m_TabBtnType.TabClick += delegate(GameObject s, int e)
		{
			if (e == 0)
			{
				this.m_PanelXuanCai.gameObject.SetActive(true);
				this.m_PanelChongSheng.gameObject.SetActive(false);
				int num = -1;
				if (Global.Data.roleData.RebornGoodsDataList != null)
				{
					for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.RebornGoodsDataList[i].GoodsID);
						if (goodsXmlNodeByID.Categoriy == 960)
						{
							if (num <= 0)
							{
								num = goodsXmlNodeByID.SuitID;
							}
							else
							{
								num = Mathf.Min(num, goodsXmlNodeByID.SuitID);
							}
						}
					}
				}
				if (num <= 0)
				{
					this.m_TabBtnXuanCaiLevel.TabIndex = 1;
				}
				else
				{
					this.m_TabBtnXuanCaiLevel.TabIndex = num;
					Vector3 vecXuanCaiLevel = this.m_VecXuanCaiLevel;
					vecXuanCaiLevel.x -= (float)(80 * (num - 1));
					float num2 = 0f;
					UIPanel component = this.m_SpringXuanCai.GetComponent<UIPanel>();
					if (component != null)
					{
						num2 = -((float)(80 * this.maxLevelXuanCai - 20) - component.clipRange.z - this.m_VecXuanCaiLevel.x);
					}
					if (vecXuanCaiLevel.x <= num2)
					{
						vecXuanCaiLevel.x = num2;
					}
					this.m_SpringXuanCai.target = vecXuanCaiLevel;
					this.m_SpringXuanCai.enabled = true;
				}
				this.m_DPSelectedItemEventHandler(this, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
			else if (e == 1)
			{
				this.m_PanelXuanCai.gameObject.SetActive(false);
				this.m_PanelChongSheng.gameObject.SetActive(true);
				this.m_TabBtnLevel.TabIndex = 1;
				this.m_DPSelectedItemEventHandler(this, new DPSelectedItemEventArgs
				{
					IDType = 1
				});
			}
		};
		this.m_TabBtnLevel.TabClick += delegate(GameObject s, int e)
		{
			this.AddList(e);
		};
		this.m_TabBtnXuanCaiLevel.TabClick += delegate(GameObject s, int e)
		{
			this.AddListBoxXuanCai(e);
		};
		this.m_BtnTipsClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelTips.gameObject.SetActive(false);
		};
		this.M_ListBtnClose[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseEquip(0);
		};
		this.M_ListBtnClose[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseEquip(1);
		};
		this.M_ListBtnClose[2].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseEquip(2);
		};
		this.M_ListBtnClose[3].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseEquip(3);
		};
		this.m_BtnChongShengNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object es, DPSelectedItemEventArgs s2)
			{
				this.RefreshNumber(s2.ID);
			}, null, 0, -100);
		};
		this.m_BtnJianNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshNumber(--this.ChongShengNumber);
		};
		this.m_BtnJiaNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshNumber(++this.ChongShengNumber);
		};
		this.m_BtnXuanCaiHeCheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendXuanCaiHeCheng();
		};
		this.m_CheckBinDing.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.RefreshNumber(this.ChongShengNumber);
		};
	}

	private void InitText()
	{
		this.m_BtnTipsHeCheng.Text = Global.GetLang("加工");
		this.m_BtnTipsFenJie.Text = Global.GetLang("分解");
		this.m_BtnXuanCaiHeCheng.Text = Global.GetLang("合成");
		this.m_TabBtn1.text = Global.GetLang("炫彩宝石");
		this.m_TabBtn2.text = Global.GetLang("重生宝石");
		this.m_LabLevelTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9D8667",
			Global.GetLang("等级筛选：")
		});
		this.m_LabNullNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("当前没有该等级炫彩宝石")
		});
		this.m_LabTipsTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"98d66c",
			Global.GetLang("宝石加工与分解")
		});
		this.m_LabHeChengSuiPianTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("合成需要:")
		});
		this.m_LabFenJieSuiPianTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("分解获得:")
		});
		this.m_CheckBinDing.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("只分解绑定宝石")
		});
		this.m_PanelXuanCai.gameObject.SetActive(true);
		this.m_PanelChongSheng.gameObject.SetActive(false);
		this.m_PanelTips.gameObject.SetActive(false);
	}

	public void InitPage(BaoShiType type = BaoShiType.XuanCai)
	{
		int num = 0;
		int[] array = new int[IConfigbase<ConfigChongShengZhuangBei>.Instance.DicXuanCaiShuXingVO.Count];
		Dictionary<int, XuanCaiShuXingVO>.Enumerator enumerator = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicXuanCaiShuXingVO.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int[] array2 = array;
			int num2 = num;
			KeyValuePair<int, XuanCaiShuXingVO> keyValuePair = enumerator.Current;
			array2[num2] = keyValuePair.Value.Level;
			num++;
		}
		this.maxLevelXuanCai = Mathf.Max(array);
		for (int i = 0; i < this.m_ListBtnXuanCai.Count; i++)
		{
			Object.Destroy(this.m_ListBtnXuanCai[i].gameObject);
		}
		for (int j = 0; j < this.maxLevelXuanCai; j++)
		{
			GameObject gameObject = U3DUtils.Clone(this.m_TabBtnXuanCaiLevel.gameObject, this.m_BtnLevelXuanCai.gameObject);
			gameObject.gameObject.SetActive(true);
			GButton component = gameObject.GetComponent<GButton>();
			component.transform.localPosition = new Vector3((float)(j * 80), 0f, 0f);
			if (component.GetComponent<BoxCollider>() != null)
			{
				component.GetComponent<BoxCollider>().size = new Vector3(80f, 42f, 1f);
				component.GetComponent<BoxCollider>().center = new Vector3(20f, 0f, 0f);
			}
			if (component != null)
			{
				component.TagIndex = j + 1;
				if (component.BtnState.GetComponent<UISprite>() != null)
				{
					component.BtnState.GetComponent<UISprite>().spriteName = (j + 1).ToString();
				}
			}
			if (gameObject.GetComponent<UIDragPanelContents>() == null)
			{
				gameObject.AddComponent<UIDragPanelContents>();
			}
			gameObject.GetComponent<UIDragPanelContents>().draggablePanel = this.m_DragLevelPanel;
			this.m_ListBtnXuanCai.Add(component);
		}
		this.m_TabBtnXuanCaiLevel.enabled = true;
		int num3 = 0;
		int count = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.Count;
		int[] array3 = new int[count];
		Dictionary<int, ChongShengBaoShiVO>.Enumerator enumerator2 = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			int[] array4 = array3;
			int num4 = num3;
			KeyValuePair<int, ChongShengBaoShiVO> keyValuePair2 = enumerator2.Current;
			array4[num4] = keyValuePair2.Value.Level;
			num3++;
		}
		int num5 = Mathf.Max(array3);
		for (int k = 0; k < this.m_ListBtn.Count; k++)
		{
			Object.Destroy(this.m_ListBtn[k].gameObject);
		}
		this.m_ListBtn.Clear();
		for (int l = 0; l < num5; l++)
		{
			GameObject gameObject2 = U3DUtils.Clone(this.m_TabBtnLevel.gameObject, this.m_BtnLevel.gameObject);
			gameObject2.gameObject.SetActive(true);
			GButton component2 = gameObject2.GetComponent<GButton>();
			component2.transform.localPosition = new Vector3((float)(l * 100), 0f, 0f);
			if (component2 != null)
			{
				component2.TagIndex = l + 1;
				component2.Text = Global.GetLang(string.Format(Global.GetLang("{0}级"), l + 1));
			}
			this.m_ListBtn.Add(component2);
		}
	}

	private void RefreshNumber(int count)
	{
		if (this.m_ChongShengTipsType == ChongShengLianLuJiaGongPart.ChongShengTipsType.GouMai)
		{
			ChongShengBaoShiVO chongShengBaoShiVO = null;
			if (IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(this.m_OnClickGd.GoodsID))
			{
				chongShengBaoShiVO = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[this.m_OnClickGd.GoodsID];
			}
			if (chongShengBaoShiVO == null)
			{
				return;
			}
			if (Global.GetRoleOwnNumByMoneyType(155) < chongShengBaoShiVO.FengYinJingShi * count || Global.GetRoleOwnNumByMoneyType(156) < chongShengBaoShiVO.ChongShengJingShi * count || Global.GetRoleOwnNumByMoneyType(157) < chongShengBaoShiVO.XuanCaiJingShi * count)
			{
				this.m_BtnJiaNumber.isEnabled = false;
				int[] array = new int[]
				{
					1,
					1,
					1
				};
				if (chongShengBaoShiVO.FengYinJingShi != 0)
				{
					array[0] = Global.GetRoleOwnNumByMoneyType(155) / chongShengBaoShiVO.FengYinJingShi;
				}
				else
				{
					array[0] = count;
				}
				if (chongShengBaoShiVO.ChongShengJingShi != 0)
				{
					array[1] = Global.GetRoleOwnNumByMoneyType(156) / chongShengBaoShiVO.ChongShengJingShi;
				}
				else
				{
					array[1] = count;
				}
				if (chongShengBaoShiVO.XuanCaiJingShi != 0)
				{
					array[2] = Global.GetRoleOwnNumByMoneyType(157) / chongShengBaoShiVO.XuanCaiJingShi;
				}
				else
				{
					array[2] = count;
				}
				this.ChongShengNumber = Mathf.Min(array);
			}
			else
			{
				int num = count + 1;
				if (Global.GetRoleOwnNumByMoneyType(155) < chongShengBaoShiVO.FengYinJingShi * num || Global.GetRoleOwnNumByMoneyType(156) < chongShengBaoShiVO.ChongShengJingShi * num || Global.GetRoleOwnNumByMoneyType(157) < chongShengBaoShiVO.XuanCaiJingShi * num)
				{
					this.m_BtnJiaNumber.isEnabled = false;
				}
				else
				{
					this.m_BtnJiaNumber.isEnabled = true;
				}
				this.ChongShengNumber = count;
			}
			if (this.ChongShengNumber <= 0)
			{
				this.m_BtnEnd.isEnabled = false;
				this.m_BtnJianNumber.isEnabled = false;
			}
			else if (this.ChongShengNumber == 1)
			{
				this.m_BtnJianNumber.isEnabled = false;
				this.m_BtnEnd.isEnabled = true;
			}
			else if (this.ChongShengNumber > 1 && this.ChongShengNumber < 999)
			{
				this.m_BtnJianNumber.isEnabled = true;
				this.m_BtnEnd.isEnabled = true;
			}
			else
			{
				if (this.ChongShengNumber >= 999)
				{
					this.ChongShengNumber = 999;
				}
				this.m_BtnJianNumber.isEnabled = true;
				this.m_BtnEnd.isEnabled = true;
				this.m_BtnJiaNumber.isEnabled = false;
			}
			this.m_LabEndSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.m_OnClickItem.BaoShiData.FengYinJingShi * this.ChongShengNumber
			});
			this.m_LabEndSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.m_OnClickItem.BaoShiData.ChongShengJingShi * this.ChongShengNumber
			});
			this.m_LabEndSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.m_OnClickItem.BaoShiData.XuanCaiJingShi * this.ChongShengNumber
			});
		}
		else if (this.m_ChongShengTipsType == ChongShengLianLuJiaGongPart.ChongShengTipsType.FenJie)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.m_OnClickGd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int num2 = 0;
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
				{
					if (Global.Data.roleData.RebornGoodsDataList[i].GoodsID == this.m_OnClickGd.GoodsID)
					{
						if (this.m_CheckBinDing.isChecked)
						{
							if (Global.Data.roleData.RebornGoodsDataList[i].Binding == 1)
							{
								num2 += Global.Data.roleData.RebornGoodsDataList[i].GCount;
							}
						}
						else
						{
							num2 += Global.Data.roleData.RebornGoodsDataList[i].GCount;
						}
					}
				}
			}
			else
			{
				this.ChongShengNumber = 0;
			}
			if (num2 <= 0)
			{
				this.ChongShengNumber = 0;
				this.m_BtnJiaNumber.isEnabled = false;
			}
			else if (count >= num2)
			{
				this.ChongShengNumber = num2;
				this.m_BtnJiaNumber.isEnabled = false;
			}
			else
			{
				this.ChongShengNumber = count;
				this.m_BtnJiaNumber.isEnabled = true;
			}
			if (this.ChongShengNumber <= 0)
			{
				this.m_BtnEnd.isEnabled = false;
				this.m_BtnJianNumber.isEnabled = false;
			}
			else if (this.ChongShengNumber == 1)
			{
				this.m_BtnJianNumber.isEnabled = false;
				this.m_BtnEnd.isEnabled = true;
			}
			else if (this.ChongShengNumber > 1 && this.ChongShengNumber < 9999)
			{
				this.m_BtnJianNumber.isEnabled = true;
				this.m_BtnEnd.isEnabled = true;
			}
			else
			{
				if (this.ChongShengNumber >= 9999)
				{
					this.ChongShengNumber = 9999;
				}
				this.m_BtnJianNumber.isEnabled = true;
				this.m_BtnEnd.isEnabled = true;
			}
			this.m_LabEndSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				goodsXmlNodeByID.ChangeFengYingJingShi * this.ChongShengNumber
			});
			this.m_LabEndSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				goodsXmlNodeByID.ChangeChongShengJingShi * this.ChongShengNumber
			});
			this.m_LabEndSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				goodsXmlNodeByID.ChangeXuanCaiJingShi * this.ChongShengNumber
			});
		}
	}

	private void RefreshBtn()
	{
		int num = 0;
		if (Global.Data.roleData.RebornGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[i].GoodsID == this.m_OnClickGd.GoodsID)
				{
					num += Global.Data.roleData.RebornGoodsDataList[i].GCount;
				}
			}
			if (num <= 0)
			{
				this.m_BtnTipsFenJie.isEnabled = false;
			}
			else
			{
				this.m_BtnTipsFenJie.isEnabled = true;
			}
		}
		else
		{
			this.m_BtnTipsFenJie.isEnabled = false;
		}
		ChongShengBaoShiVO chongShengBaoShiVO = null;
		if (IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(this.m_OnClickGd.GoodsID))
		{
			chongShengBaoShiVO = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[this.m_OnClickGd.GoodsID];
		}
		if (chongShengBaoShiVO == null)
		{
			return;
		}
		if (chongShengBaoShiVO.FengYinJingShi <= Global.GetRoleOwnNumByMoneyType(155) && chongShengBaoShiVO.ChongShengJingShi <= Global.GetRoleOwnNumByMoneyType(156) && chongShengBaoShiVO.XuanCaiJingShi <= Global.GetRoleOwnNumByMoneyType(157))
		{
			this.m_BtnTipsHeCheng.isEnabled = true;
		}
		else
		{
			this.m_BtnTipsHeCheng.isEnabled = false;
		}
	}

	private void SelectionChange(object sender, object e)
	{
		ChongShengLianLuJiaGongItem chongShengLianLuJiaGongItem = U3DUtils.AS<ChongShengLianLuJiaGongItem>(this.m_ListBox.SelectedItem);
		if (chongShengLianLuJiaGongItem == null)
		{
			return;
		}
		if (this.m_GameGoodsParnet.GetComponentInChildren<GGoodIcon>() != null)
		{
			Object.Destroy(this.m_GameGoodsParnet.GetComponentInChildren<GGoodIcon>());
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(chongShengLianLuJiaGongItem.BaoShiData.BaoShiID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		this.StartTipsPanel(chongShengLianLuJiaGongItem, dummyGoodsDataMu);
		this.RefreshBtn();
	}

	private void StartTipsPanel(ChongShengLianLuJiaGongItem item, GoodsData gd)
	{
		this.m_PanelTips.gameObject.SetActive(true);
		this.m_OnClickItem = item;
		this.m_OnClickGd = gd;
		gd.GCount = item.Number;
		if (this.m_GameGoodsParnet.GetComponentInChildren<GGoodIcon>() != null)
		{
			Object.Destroy(this.m_GameGoodsParnet.GetComponentInChildren<GGoodIcon>().gameObject);
		}
		GGoodIcon icon = Global.CreateGoodsIcon(gd, false, true);
		icon.transform.SetParent(this.m_GameGoodsParnet.transform, false);
		this.m_LabTipsName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"98d66c",
			Global.GetLang(item.BaoShiName)
		});
		if (item.Number > 0)
		{
			this.m_LabTipsIsHuoDe.text = Global.GetColorStringForNGUIText(new object[]
			{
				"98d66c",
				Global.GetLang(string.Format(Global.GetLang("拥有：{0}"), item.Number))
			});
		}
		else
		{
			this.m_LabTipsIsHuoDe.text = Global.GetColorStringForNGUIText(new object[]
			{
				"98d66c",
				Global.GetLang("未拥有")
			});
		}
		icon.MouseLeftButtonUp = delegate(object s2, MouseEvent e2)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
			GTipServiceEx.GGoodTipsPart.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), gd.GCount);
		};
		this.m_LabTipsContentTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("宝石属性")
		});
		string[] array = item.BaoShiData.ShuXing.Split(new char[]
		{
			'|'
		});
		StringBuilder stringBuilder = new StringBuilder();
		ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[0].Split(new char[]
		{
			','
		})[0]);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("攻击装备佩戴") + Environment.NewLine
		}));
		if (ConfigExtPropIndexes.GetPercentByWord(array[0].Split(new char[]
		{
			','
		})[0]))
		{
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord.Description, (float.Parse(array[0].Split(new char[]
				{
					','
				})[1]) * 100f).ToString("f1")) + Environment.NewLine)
			}));
		}
		else
		{
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord.Description, array[0].Split(new char[]
				{
					','
				})[1]) + Environment.NewLine)
			}));
		}
		ExtPropIndexesVO extPropIndexesVOByWord2 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[1].Split(new char[]
		{
			','
		})[0]);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("防御装备佩戴") + Environment.NewLine
		}));
		if (ConfigExtPropIndexes.GetPercentByWord(array[1].Split(new char[]
		{
			','
		})[0]))
		{
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord2.Description, (float.Parse(array[1].Split(new char[]
				{
					','
				})[1]) * 100f).ToString("f1")) + Environment.NewLine)
			}));
		}
		else
		{
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord2.Description, array[1].Split(new char[]
				{
					','
				})[1]) + Environment.NewLine)
			}));
		}
		this.m_LabTipsContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"98d66c",
			stringBuilder.ToString()
		});
		this.m_LabHeChengSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			item.BaoShiData.FengYinJingShi
		});
		this.m_LabHeChengSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			item.BaoShiData.ChongShengJingShi
		});
		this.m_LabHeChengSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			item.BaoShiData.XuanCaiJingShi
		});
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		this.m_BtnTipsHeCheng.MouseLeftButtonUp = delegate(object s, MouseEvent eve)
		{
			this.RefreshNumberGouMaiOrFenJie(1, gd, item.BaoShiName);
		};
		this.m_BtnTipsFenJie.MouseLeftButtonUp = delegate(object s, MouseEvent eve)
		{
			this.RefreshNumberGouMaiOrFenJie(2, gd, item.BaoShiName);
		};
		this.m_LabFenJieSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			goodsXmlNodeByID.ChangeFengYingJingShi
		});
		this.m_LabFenJieSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			goodsXmlNodeByID.ChangeChongShengJingShi
		});
		this.m_LabFenJieSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			goodsXmlNodeByID.ChangeXuanCaiJingShi
		});
		if (Global.GetRoleOwnNumByMoneyType(155) >= item.BaoShiData.FengYinJingShi)
		{
			this.m_LabHeChengSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				item.BaoShiData.FengYinJingShi
			});
		}
		else
		{
			this.m_LabHeChengSuiPian[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				item.BaoShiData.FengYinJingShi
			});
		}
		if (Global.GetRoleOwnNumByMoneyType(156) >= item.BaoShiData.ChongShengJingShi)
		{
			this.m_LabHeChengSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				item.BaoShiData.ChongShengJingShi
			});
		}
		else
		{
			this.m_LabHeChengSuiPian[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				item.BaoShiData.ChongShengJingShi
			});
		}
		if (Global.GetRoleOwnNumByMoneyType(157) >= item.BaoShiData.XuanCaiJingShi)
		{
			this.m_LabHeChengSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				item.BaoShiData.XuanCaiJingShi
			});
		}
		else
		{
			this.m_LabHeChengSuiPian[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				item.BaoShiData.XuanCaiJingShi
			});
		}
	}

	private void RefreshNumberGouMaiOrFenJie(int key, GoodsData gd, string name)
	{
		this.m_CheckBinDing.isChecked = true;
		this.m_PanelNumber.gameObject.SetActive(true);
		this.m_ChongShengTipsType = (ChongShengLianLuJiaGongPart.ChongShengTipsType)key;
		this.RefreshNumber(1);
		if (this.m_GameEndGoodParent.GetComponentInChildren<GGoodIcon>() != null)
		{
			Object.Destroy(this.m_GameEndGoodParent.GetComponentInChildren<GGoodIcon>().gameObject);
		}
		GGoodIcon ggoodIcon = Global.CreateGoodsIcon(gd, false, true);
		ggoodIcon.transform.SetParent(this.m_GameEndGoodParent.transform, false);
		if (gd.GCount > 0)
		{
			this.m_LabEndNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("拥有数量:{0}"), gd.GCount))
			});
		}
		else
		{
			this.m_LabEndNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("拥有数量:未拥有")
			});
		}
		this.m_LabEndName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(name)
		});
		if (key == 1)
		{
			this.m_CheckBinDing.gameObject.SetActive(false);
			this.m_LabEndASuiPianTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang("合成需要:")
			});
			this.m_BtnEnd.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("加工")
			});
			this.m_BtnEnd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.SendChongShengBuy(this.m_OnClickGd.GoodsID, this.ChongShengNumber);
				this.m_PanelNumber.gameObject.SetActive(false);
			};
		}
		else if (key == 2)
		{
			this.m_CheckBinDing.gameObject.SetActive(true);
			this.m_LabEndASuiPianTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang("分解获得:")
			});
			this.m_BtnEnd.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("分解")
			});
			this.m_BtnEnd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int isCheck = (!this.m_CheckBinDing.isChecked) ? 0 : 1;
				if (IConfigbase<ConfigChongShengZhuangBei>.Instance.GetChongShengBaoShiById(gd.GoodsID) != null && IConfigbase<ConfigChongShengZhuangBei>.Instance.GetChongShengBaoShiById(gd.GoodsID).Level > 6)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("此物品比较贵重，是否确认回收？")
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendChongShengFenJie(this.m_OnClickGd.GoodsID, this.ChongShengNumber, isCheck);
							this.m_PanelNumber.gameObject.SetActive(false);
						}
						return true;
					};
					return;
				}
				GameInstance.Game.SendChongShengFenJie(this.m_OnClickGd.GoodsID, this.ChongShengNumber, isCheck);
				this.m_PanelNumber.gameObject.SetActive(false);
			};
		}
		this.m_BtnEndColse.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelNumber.gameObject.SetActive(false);
			this.StartTipsPanel(this.m_OnClickItem, this.m_OnClickGd);
		};
		this.m_PanelTips.gameObject.SetActive(false);
	}

	private void AddList(int level)
	{
		this.m_Obser.Clear();
		foreach (KeyValuePair<int, ChongShengBaoShiVO> keyValuePair in IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO)
		{
			if (level == keyValuePair.Value.Level)
			{
				ChongShengLianLuJiaGongItem chongShengLianLuJiaGongItem = U3DUtils.NEW<ChongShengLianLuJiaGongItem>();
				this.m_Obser.AddNoUpdate(chongShengLianLuJiaGongItem);
				int num = 0;
				Dictionary<int, ChongShengBaoShiVO>.Enumerator enumerator;
				if (Global.Data.roleData.RebornGoodsDataList != null)
				{
					for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
					{
						int goodsID = Global.Data.roleData.RebornGoodsDataList[i].GoodsID;
						KeyValuePair<int, ChongShengBaoShiVO> keyValuePair2 = enumerator.Current;
						if (goodsID == keyValuePair2.Value.BaoShiID)
						{
							num += Global.Data.roleData.RebornGoodsDataList[i].GCount;
						}
					}
				}
				chongShengLianLuJiaGongItem.Number = num;
				ChongShengLianLuJiaGongItem chongShengLianLuJiaGongItem2 = chongShengLianLuJiaGongItem;
				KeyValuePair<int, ChongShengBaoShiVO> keyValuePair3 = enumerator.Current;
				chongShengLianLuJiaGongItem2.SetDataChongSheng(keyValuePair3.Value);
				if (chongShengLianLuJiaGongItem.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(chongShengLianLuJiaGongItem.GetComponent<UIPanel>());
				}
			}
		}
	}

	private void AddListBoxXuanCai(int level)
	{
		this.m_LevelXuanCai = level;
		List<XuanCaiShuXingVO> xuanCaiShuXing = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiShuXing(level);
		this.m_ObserXuanCai.Clear();
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		int i = 0;
		while (i < xuanCaiShuXing.Count)
		{
			if (!(this.M_ListIcon[0] != null))
			{
				goto IL_D6;
			}
			GoodsData goodsData = this.M_ListIcon[0].ItemObject as GoodsData;
			if (goodsData == null)
			{
				break;
			}
			XuanCaiHeChengVO xuanCaiHeCheng = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiHeCheng(goodsData.GoodsID);
			if (xuanCaiHeCheng == null)
			{
				return;
			}
			string[] array = xuanCaiHeCheng.HeChengXiaoHao.Split(new char[]
			{
				','
			});
			bool flag = false;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].SafeToInt32(0) == xuanCaiShuXing[i].DaoJuID)
				{
					flag = true;
				}
			}
			if (flag)
			{
				goto IL_D6;
			}
			IL_254:
			i++;
			continue;
			IL_D6:
			if (Global.Data.roleData.RebornGoodsDataList == null)
			{
				goto IL_254;
			}
			int num = 0;
			int num2 = 0;
			for (int k = 0; k < Global.Data.roleData.RebornGoodsDataList.Count; k++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[k].GoodsID == xuanCaiShuXing[i].DaoJuID)
				{
					if (Global.Data.roleData.RebornGoodsDataList[k].Binding == 1)
					{
						num += Global.Data.roleData.RebornGoodsDataList[k].GCount;
					}
					else
					{
						num2 += Global.Data.roleData.RebornGoodsDataList[k].GCount;
					}
				}
			}
			if (num <= 0 && num2 <= 0)
			{
				goto IL_254;
			}
			num -= this.GetNumber(xuanCaiShuXing[i].DaoJuID, 1);
			num2 -= this.GetNumber(xuanCaiShuXing[i].DaoJuID, 0);
			if (dictionary.ContainsKey(xuanCaiShuXing[i].DaoJuID))
			{
				goto IL_254;
			}
			dictionary.Add(xuanCaiShuXing[i].DaoJuID, true);
			if (num > 0)
			{
				this.SetItem(xuanCaiShuXing[i].DaoJuID, num, 1);
			}
			if (num2 > 0)
			{
				this.SetItem(xuanCaiShuXing[i].DaoJuID, num2, 0);
				goto IL_254;
			}
			goto IL_254;
		}
		if (this.m_ObserXuanCai.Count <= 0)
		{
			this.m_LabNullNumber.gameObject.SetActive(true);
		}
		else
		{
			this.m_LabNullNumber.gameObject.SetActive(false);
		}
		this.m_SpringPanelXuanCai.target = this.m_VecXuanCaiEquip;
		this.m_SpringPanelXuanCai.enabled = true;
	}

	private void SetItem(int goodid, int count, int binDing)
	{
		if (count > 0)
		{
			ChongShengLianluEquipItem chongShengLianluEquipItem = U3DUtils.NEW<ChongShengLianluEquipItem>();
			this.m_ObserXuanCai.AddNoUpdate(chongShengLianluEquipItem);
			chongShengLianluEquipItem.SetDataXuanCai(goodid, count, binDing);
			if (chongShengLianluEquipItem.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(chongShengLianluEquipItem.GetComponent<UIPanel>());
			}
			if (chongShengLianluEquipItem.GetComponent<UIDragPanelContents>() == null)
			{
				chongShengLianluEquipItem.gameObject.AddComponent<UIDragPanelContents>();
			}
			chongShengLianluEquipItem.GetComponent<UIDragPanelContents>().draggablePanel = this.m_DragBaoShiPanel;
			chongShengLianluEquipItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.AddEquip(e.ZhuZhuangBei);
			};
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (this.m_LevelXuanCai >= this.maxLevelXuanCai)
		{
			Super.HintMainText(Global.GetLang("当前宝石已经是最高等级，无法进行合成"), 10, 3);
			return;
		}
		if (Global.Data.roleData.RebornGoodsDataList == null)
		{
			return;
		}
		if (this.M_ListIcon[0] == null)
		{
			XuanCaiHeChengVO xuanCaiHeCheng = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiHeCheng(gd.GoodsID);
			if (xuanCaiHeCheng == null)
			{
				return;
			}
			string[] array = xuanCaiHeCheng.HeChengXiaoHao.Split(new char[]
			{
				','
			});
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.RebornGoodsDataList[i];
				int gcount = gd.GCount;
				for (int j = 0; j < array.Length; j++)
				{
					if (goodsData.GoodsID == array[j].SafeToInt32(0) && gcount > 0 && this.M_ListIcon[0] == null && goodsData.GoodsID == gd.GoodsID && goodsData.Binding == gd.Binding)
					{
						GoodsData gdNew = Global.GetDummyGoodsDataMu(goodsData.GoodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
						gdNew.Id = goodsData.Id;
						gdNew.Binding = goodsData.Binding;
						GGoodIcon icon = Global.CreateGoodsIcon(gdNew, false, true);
						icon.SecondText.gameObject.SetActive(false);
						this.M_ListIcon[0] = icon;
						icon.transform.SetParent(this.M_ListParent[0].transform, false);
						icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
						{
							gdNew.GCount = gd.GCount;
							GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gdNew);
							GTipServiceEx.GGoodTipsPart.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), gd.GCount);
						};
						this.M_ListBtnClose[0].gameObject.SetActive(true);
						break;
					}
				}
			}
			if (this.M_ListIcon[3] != null)
			{
				Object.Destroy(this.M_ListIcon[3].gameObject);
				this.M_ListIcon[3] = null;
			}
			if (this.M_ListIcon[0].ItemObject is GoodsData)
			{
				GoodsData gdNew = Global.GetDummyGoodsDataMu(xuanCaiHeCheng.HeChengBaoShiId, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				gdNew.Binding = this.IsBindOfNextEquip();
				GGoodIcon iconNew = Global.CreateGoodsIcon(gdNew, false, true);
				iconNew.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					GTipServiceEx.ShowTip(iconNew, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gdNew);
				};
				this.M_ListIcon[3] = iconNew;
				iconNew.transform.SetParent(this.M_ListParent[3].transform, false);
			}
			this.AddListBoxXuanCai(this.m_LevelXuanCai);
		}
		else
		{
			if (this.M_ListIcon[1] == null)
			{
				for (int k = 0; k < Global.Data.roleData.RebornGoodsDataList.Count; k++)
				{
					if (Global.Data.roleData.RebornGoodsDataList[k].GoodsID == gd.GoodsID && Global.Data.roleData.RebornGoodsDataList[k].Binding == gd.Binding)
					{
						GoodsData goodsData2 = Global.Data.roleData.RebornGoodsDataList[k];
						int gcount2 = gd.GCount;
						if (gcount2 > 0)
						{
							GoodsData gdNew = Global.GetDummyGoodsDataMu(gd.GoodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
							gdNew.Id = goodsData2.Id;
							gdNew.Binding = goodsData2.Binding;
							GGoodIcon iconFu = Global.CreateGoodsIcon(gdNew, false, true);
							this.M_ListIcon[1] = iconFu;
							iconFu.transform.SetParent(this.M_ListParent[1].transform, false);
							this.M_ListBtnClose[1].gameObject.SetActive(true);
							iconFu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
							{
								gdNew.GCount = gd.GCount;
								GTipServiceEx.ShowTip(iconFu, TipTypes.GoodsText, GoodsOwnerTypes.None, gdNew);
								GTipServiceEx.GGoodTipsPart.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), gd.GCount);
							};
							this.AddListBoxXuanCai(this.m_LevelXuanCai);
							return;
						}
					}
				}
			}
			if (this.M_ListIcon[2] == null)
			{
				for (int l = 0; l < Global.Data.roleData.RebornGoodsDataList.Count; l++)
				{
					if (Global.Data.roleData.RebornGoodsDataList[l].GoodsID == gd.GoodsID && Global.Data.roleData.RebornGoodsDataList[l].Binding == gd.Binding)
					{
						GoodsData goodsData3 = Global.Data.roleData.RebornGoodsDataList[l];
						int gcount3 = gd.GCount;
						if (gcount3 > 0)
						{
							GoodsData gdNew = Global.GetDummyGoodsDataMu(gd.GoodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
							gdNew.Id = goodsData3.Id;
							gdNew.Binding = goodsData3.Binding;
							GGoodIcon iconFu = Global.CreateGoodsIcon(gdNew, false, true);
							this.M_ListIcon[2] = iconFu;
							iconFu.transform.SetParent(this.M_ListParent[2].transform, false);
							this.M_ListBtnClose[2].gameObject.SetActive(true);
							iconFu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
							{
								gdNew.GCount = gd.GCount;
								GTipServiceEx.ShowTip(iconFu, TipTypes.GoodsText, GoodsOwnerTypes.None, gdNew);
								GTipServiceEx.GGoodTipsPart.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), gd.GCount);
							};
							this.AddListBoxXuanCai(this.m_LevelXuanCai);
							return;
						}
					}
				}
			}
		}
	}

	public int GetNumber(int goodid, int binDing)
	{
		int num = 0;
		if (this.M_ListIcon[0] != null)
		{
			GoodsData goodsData = this.M_ListIcon[0].ItemObject as GoodsData;
			if (goodsData.Id > 0 && goodsData.Binding == binDing && goodsData.GoodsID == goodid)
			{
				num++;
			}
		}
		if (this.M_ListIcon[1] != null)
		{
			GoodsData goodsData = this.M_ListIcon[1].ItemObject as GoodsData;
			if (goodsData.Id > 0 && goodsData.Binding == binDing && goodsData.GoodsID == goodid)
			{
				num++;
			}
		}
		if (this.M_ListIcon[2] != null)
		{
			GoodsData goodsData = this.M_ListIcon[2].ItemObject as GoodsData;
			if (goodsData.Id > 0 && goodsData.Binding == binDing && goodsData.GoodsID == goodid)
			{
				num++;
			}
		}
		return num;
	}

	public void CloseEquip(int count)
	{
		if (count == 3 || count == 0)
		{
			for (int i = 0; i < this.M_ListIcon.Length; i++)
			{
				if (this.M_ListIcon[i] != null)
				{
					Object.Destroy(this.M_ListIcon[i].gameObject);
					this.M_ListIcon[i] = null;
					this.M_ListBtnClose[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			Object.Destroy(this.M_ListIcon[count].gameObject);
			this.M_ListIcon[count] = null;
			this.M_ListBtnClose[count].gameObject.SetActive(false);
		}
		this.AddListBoxXuanCai(this.m_LevelXuanCai);
	}

	private void EquipOnClick(object sender, MouseEvent e)
	{
		if (this.m_ListBoxXuanCaiEquip.SelectedItem == null)
		{
			return;
		}
		ChongShengLianluEquipItem component = this.m_ListBoxXuanCaiEquip.SelectedItem.GetComponent<ChongShengLianluEquipItem>();
		if (component == null)
		{
			return;
		}
		this.AddEquip(component.GoodsData);
		this.AddListBoxXuanCai(this.m_LevelXuanCai);
	}

	public int IsBindOfNextEquip()
	{
		for (int i = 0; i < this.M_ListIcon.Length - 1; i++)
		{
			if (this.M_ListIcon[i] != null && (this.M_ListIcon[i].ItemObject as GoodsData).Binding == 1)
			{
				return 1;
			}
		}
		return 0;
	}

	public void RefreshChongSheng()
	{
		this.RefreshCuiPian();
		for (int i = 0; i < this.m_Obser.Count; i++)
		{
			ChongShengLianLuJiaGongItem component = this.m_Obser.GetAt(i).GetComponent<ChongShengLianLuJiaGongItem>();
			if (component != null && component.GoodId == this.m_OnClickGd.GoodsID)
			{
				if (Global.Data.roleData.RebornGoodsDataList != null)
				{
					int num = 0;
					for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
					{
						if (Global.Data.roleData.RebornGoodsDataList[j].GoodsID == this.m_OnClickGd.GoodsID)
						{
							num += Global.Data.roleData.RebornGoodsDataList[j].GCount;
						}
					}
					component.Number = num;
					this.m_OnClickGd.GCount = num;
				}
				else
				{
					this.m_OnClickGd.GCount = 0;
					component.Number = 0;
				}
			}
		}
	}

	private void SendXuanCaiHeCheng()
	{
		if (this.M_ListIcon[0] == null)
		{
			Super.HintMainText(Global.GetLang("请放入要合成的宝石"), 10, 3);
			return;
		}
		if (this.M_ListIcon[1] == null || this.M_ListIcon[2] == null)
		{
			Super.HintMainText(Global.GetLang("合成材料不足"), 10, 3);
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		GoodsData goodsData = this.M_ListIcon[0].ItemObject as GoodsData;
		if (goodsData != null && goodsData.Id > 0)
		{
			dictionary.Add(goodsData.Id, 1);
		}
		GoodsData goodsData2 = this.M_ListIcon[1].ItemObject as GoodsData;
		if (goodsData2 != null && goodsData2.Id > 0)
		{
			if (!dictionary.ContainsKey(goodsData2.Id))
			{
				dictionary.Add(goodsData2.Id, 1);
			}
			else
			{
				Dictionary<int, int> dictionary3;
				Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
				int num2;
				int num = num2 = goodsData2.Id;
				num2 = dictionary3[num2];
				dictionary2[num] = num2 + 1;
			}
		}
		GoodsData goodsData3 = this.M_ListIcon[2].ItemObject as GoodsData;
		if (goodsData3 != null && goodsData3.Id > 0)
		{
			if (!dictionary.ContainsKey(goodsData3.Id))
			{
				dictionary.Add(goodsData3.Id, 1);
			}
			else
			{
				Dictionary<int, int> dictionary5;
				Dictionary<int, int> dictionary4 = dictionary5 = dictionary;
				int num2;
				int num3 = num2 = goodsData3.Id;
				num2 = dictionary5[num2];
				dictionary4[num3] = num2 + 1;
			}
		}
		if (goodsData.Id <= 0)
		{
			Super.HintMainText(Global.GetLang("请放入要合成的宝石"), 10, 3);
			return;
		}
		if (goodsData2.Id <= 0 || goodsData3.Id <= 0)
		{
			Super.HintMainText(Global.GetLang("合成材料不足"), 10, 3);
			return;
		}
		int[] data = new int[6];
		int num4 = 0;
		Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int[] data3 = data;
			int num5 = num4;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			data3[num5] = keyValuePair.Key;
			num4++;
			int[] data2 = data;
			int num6 = num4;
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			data2[num6] = keyValuePair2.Value;
			num4++;
		}
		if (this.IsBindOfNextEquip() == 1 && (goodsData.Binding == 0 || goodsData2.Binding == 0 || goodsData3.Binding == 0))
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("合成材料含有绑定宝石，合成宝石为绑定，是否合成")
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.SenXuanCaiLevelMax(data[0], data[1], data[2], data[3], data[4], data[5]);
				}
				return true;
			};
			return;
		}
		this.SenXuanCaiLevelMax(data[0], data[1], data[2], data[3], data[4], data[5]);
	}

	private void SenXuanCaiLevelMax(int DBID1, int num1, int DBID2, int num2, int DBID3, int num3)
	{
		GoodsData goodsData = this.M_ListIcon[3].ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		List<bool> levelXuanCai = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetLevelXuanCai(Global.Data.RoleID, goodsData.GoodsID);
		bool flag = true;
		for (int i = 0; i < levelXuanCai.Count; i++)
		{
			flag = (levelXuanCai[i] && flag);
		}
		if (Global.GetZuanShi(ZuanShiPartClass.ChongShengXuanCaiHeCheng) && !flag)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("请查看全身的重生宝石是否可激活该炫彩宝石属性")
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
			{
				messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
			}
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					Global.SetZuanShi(ZuanShiPartClass.ChongShengXuanCaiHeCheng, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
				}
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendXuanCaiHeCheng(DBID1, num1, DBID2, num2, DBID3, num3);
				}
				return true;
			};
			return;
		}
		GameInstance.Game.SendXuanCaiHeCheng(DBID1, num1, DBID2, num2, DBID3, num3);
	}

	public void RefreshXuanCai()
	{
		for (int i = 0; i < this.M_ListIcon.Length; i++)
		{
			if (this.M_ListIcon[i] != null)
			{
				Object.Destroy(this.M_ListIcon[i].gameObject);
				this.M_ListIcon[i] = null;
				this.M_ListBtnClose[i].gameObject.SetActive(false);
			}
		}
		this.AddListBoxXuanCai(this.m_LevelXuanCai);
	}

	public UITab m_TabBtnType;

	public UILabel m_TabBtn1;

	public UILabel m_TabBtn2;

	[SerializeField]
	private ListBox m_ListBox;

	public UITab m_TabBtnLevel;

	[SerializeField]
	private GButton m_BtnLevel;

	[SerializeField]
	private GButton m_BtnLevelXuanCai;

	public List<GButton> m_ListBtn;

	[SerializeField]
	private UIPanel m_PanelChongSheng;

	[SerializeField]
	private UIPanel m_PanelXuanCai;

	[SerializeField]
	private UILabel m_LabLevelTitle;

	[SerializeField]
	private UIPanel m_PanelTips;

	[SerializeField]
	private UILabel m_LabTipsTitle;

	[SerializeField]
	private UILabel m_LabTipsName;

	[SerializeField]
	private UILabel m_LabTipsIsHuoDe;

	[SerializeField]
	private UILabel m_LabTipsContentTitle;

	[SerializeField]
	private UILabel m_LabTipsContent;

	[SerializeField]
	private GameObject m_GameGoodsParnet;

	[SerializeField]
	private GButton m_BtnTipsHeCheng;

	[SerializeField]
	private GButton m_BtnTipsFenJie;

	[SerializeField]
	private GButton m_BtnTipsClose;

	public GButton m_BtnChongShengNumber;

	[SerializeField]
	private GButton m_BtnJianNumber;

	[SerializeField]
	private GButton m_BtnJiaNumber;

	[SerializeField]
	private UILabel m_LabFengYinJingShiNumber;

	[SerializeField]
	private UILabel m_LabChongShengJingShiNumber;

	[SerializeField]
	private UILabel m_LabXuanCaiJingShiNumber;

	[SerializeField]
	private UILabel m_LabHeChengSuiPianTitle;

	[SerializeField]
	private UILabel m_LabFenJieSuiPianTitle;

	[SerializeField]
	private UILabel[] m_LabHeChengSuiPian;

	[SerializeField]
	private UILabel[] m_LabFenJieSuiPian;

	[SerializeField]
	private UIPanel m_PanelNumber;

	[SerializeField]
	private UILabel[] m_LabEndSuiPian;

	[SerializeField]
	private GButton m_BtnEnd;

	[SerializeField]
	private GButton m_BtnEndColse;

	[SerializeField]
	private UILabel m_LabEndNumber;

	[SerializeField]
	private UILabel m_LabEndName;

	[SerializeField]
	private UILabel m_LabEndASuiPianTitle;

	[SerializeField]
	private GameObject m_GameEndGoodParent;

	[SerializeField]
	private GCheckBox m_CheckBinDing;

	public List<GButton> m_ListBtnXuanCai;

	public UITab m_TabBtnXuanCaiLevel;

	public UIDraggablePanel m_DragLevelPanel;

	public UIDraggablePanel m_DragBaoShiPanel;

	public SpringPanel m_SpringPanelXuanCai;

	public ListBox m_ListBoxXuanCaiEquip;

	public GButton m_BtnXuanCaiHeCheng;

	public UILabel m_LabNullNumber;

	public SpringPanel m_SpringXuanCai;

	private ObservableCollection m_ObserXuanCai;

	public DPSelectedItemEventHandler m_DPSelectedItemEventHandler;

	private GGoodIcon[] M_ListIcon = new GGoodIcon[4];

	public GameObject[] M_ListParent = new GameObject[4];

	public GButton[] M_ListBtnClose = new GButton[4];

	private ObservableCollection m_Obser;

	private Vector3 m_VecXuanCaiEquip;

	private Vector3 m_VecXuanCaiLevel;

	private GoodsData m_OnClickGd;

	private ChongShengLianLuJiaGongItem m_OnClickItem;

	private ChongShengLianLuJiaGongPart.ChongShengTipsType m_ChongShengTipsType;

	private int m_LevelXuanCai;

	private int m_ChongShengNumber;

	private int maxLevelXuanCai;

	public enum ChongShengTipsType
	{
		None,
		GouMai,
		FenJie
	}
}
