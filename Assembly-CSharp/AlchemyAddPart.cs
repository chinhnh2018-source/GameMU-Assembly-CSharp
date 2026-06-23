using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class AlchemyAddPart : UserControl
{
	private bool IsShowCheckBox
	{
		set
		{
			this.mCheckBox.gameObject.SetActive(value);
		}
	}

	private bool IsShowDaoJuGoodsIcon
	{
		set
		{
			this.goodsTexture1.gameObject.SetActive(value);
			this.goodsTexture2.gameObject.SetActive(value);
			this.goodsTexture3.gameObject.SetActive(value);
			this.DuiHuanWuPin.gameObject.SetActive(!value);
			this.ShengYuWuPin.gameObject.SetActive(!value);
			this.XiaoHaoType.gameObject.SetActive(!value);
		}
	}

	private bool IsShowHuoBiGoodsIcon
	{
		set
		{
			this.DuiHuanWuPin.gameObject.SetActive(value);
			this.ShengYuWuPin.gameObject.SetActive(value);
			this.XiaoHaoType.gameObject.SetActive(value);
			this.goodsTexture1.gameObject.SetActive(!value);
			this.goodsTexture2.gameObject.SetActive(!value);
			this.goodsTexture3.gameObject.SetActive(!value);
		}
	}

	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("灌注")
		});
		this.DuiHuanBiLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("兑换比例:")
		});
		this.ShiYongShangXianLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("今日使用上限：")
		});
		this.ShengYuWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("剩余{0}:"), this.DicConversion[this.CostType].Name)
		});
		this.XiaoHaoWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("消耗{0}:"), this.DicConversion[this.CostType].Name)
		});
		this.DuiHuanYuanSuLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("兑换数量:")
		});
		this.BtnGuanZhu.Label.text = Global.GetLang("灌注");
		this.mCheckBox._Lable.text = Global.GetLang("只灌注绑定道具");
		this.mBtnHuoBi.Label.text = Global.GetLang("货币");
		this.mBtnDaoJu.Label.text = Global.GetLang("道具");
		if (int.Parse(this.GetGoodValue(this.GoodType)) < this.DicConversion[this.GoodType].Unit)
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				this.GetGoodValue(this.GoodType)
			});
		}
		else
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.GetGoodValue(this.GoodType)
			});
		}
		this.Input.Text = this.InputNum.ToString();
		if (this.InputNum <= 0)
		{
			this.XScrollBar.scrollValue = 0f;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mOBC = this.BtnGoods.ItemsSource;
		this.BtnGoods.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		this.AddButtons(false, false);
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnGuanZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.MyAlchemyData != null && Global.Data.MyAlchemyData.ToDayCost.ContainsKey(this.CostType) && Global.Data.MyAlchemyData.ToDayCost[this.CostType] >= this.DicConversion[this.CostType].Limit)
			{
				Super.HintMainText(Global.GetLang("达到今日上限"), 10, 3);
				return;
			}
			if (this.CostNum < this.DicConversion[this.CostType].Unit)
			{
				if (this.mIsShowHuoBi)
				{
					Super.HintMainText(Global.GetLang("请添加货币后进行灌注"), 10, 3);
				}
				if (this.mIsShowDaoJu)
				{
					Super.HintMainText(Global.GetLang("请添加道具后进行灌注"), 10, 3);
				}
				return;
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string text = string.Format("{0}", Global.GetLang("{dac7ae}确定灌注{17e43e}{0}{3681f3}{1}{dac7ae},兑换{17e43e}{2}{3681f3}元素值{dac7ae}吗？"));
			string text2 = text.Replace("{0}", this.CostNum.ToString());
			text2 = text2.Replace("{1}", this.DicConversion[this.CostType].Name);
			text2 = text2.Replace("{2}", this.DuiHuanNum.ToString());
			Super.ShowMessageBoxEx(Global.GetLang("提示"), text2, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					GameInstance.Game.SendAlchemyAddElement(this.CostType, this.CostNum, this.mIsUseDaoJu);
					Super.ShowNetWaiting(null);
				}
			}, buttons);
		};
		this.XScrollBar.onChange = delegate(UIScrollBar sb)
		{
			if (int.Parse(this.GetGoodValue(this.GoodType)) < this.DicConversion[this.GoodType].Unit)
			{
				this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = false;
				this.XScrollBar.scrollValue = 0f;
				return;
			}
			this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = true;
			int num = Mathf.RoundToInt(sb.scrollValue * (float)this.MaxNum);
			num -= num % this.DicConversion[this.GoodType].Unit;
			this.InputNum = Mathf.Min(num, this.MaxNum);
			if (this.InputNum <= 0)
			{
				this.InputNum = 0;
			}
			this.Input.Text = this.InputNum.ToString();
			this.DuiHuanYuanSuNum.text = (this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit)).ToString();
			if (this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit) == 0)
			{
			}
			if (int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum < this.DicConversion[this.GoodType].Unit)
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			else
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			if (this.InputNum <= 0)
			{
				this.XScrollBar.scrollValue = 0f;
			}
			else
			{
				this.XScrollBar.scrollValue = (float)this.InputNum / (float)this.MaxNum;
			}
			this.CostNum = this.InputNum;
			this.DuiHuanNum = this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit);
		};
		UIEventListener.Get(this.AddIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (int.Parse(this.GetGoodValue(this.GoodType)) < this.DicConversion[this.GoodType].Unit)
			{
				this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = false;
				this.XScrollBar.scrollValue = 0f;
				return;
			}
			this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = true;
			this.InputNum = Mathf.Min(this.InputNum += this.DicConversion[this.GoodType].Unit, this.MaxNum);
			this.Input.Text = this.InputNum.ToString();
			this.DuiHuanYuanSuNum.text = (this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit)).ToString();
			if (int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum < this.DicConversion[this.GoodType].Unit)
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			else
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			this.XScrollBar.scrollValue = (float)this.InputNum / (float)this.MaxNum;
			this.CostNum = this.InputNum;
			this.DuiHuanNum = this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit);
		};
		UIEventListener.Get(this.SubIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (int.Parse(this.GetGoodValue(this.GoodType)) < this.DicConversion[this.GoodType].Unit)
			{
				this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = false;
				this.XScrollBar.scrollValue = 0f;
				return;
			}
			this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = true;
			this.InputNum = Mathf.Max(this.InputNum -= this.DicConversion[this.GoodType].Unit, 0);
			this.Input.Text = this.InputNum.ToString();
			this.DuiHuanYuanSuNum.text = (this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit)).ToString();
			if (int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum < this.DicConversion[this.GoodType].Unit)
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			else
			{
				this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					int.Parse(this.GetGoodValue(this.GoodType)) - this.InputNum
				});
			}
			if (this.InputNum <= 0)
			{
				this.XScrollBar.scrollValue = 0f;
			}
			else
			{
				this.XScrollBar.scrollValue = (float)this.InputNum / (float)this.MaxNum;
			}
			this.CostNum = this.InputNum;
			this.DuiHuanNum = this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit);
		};
		this.InitCategory();
	}

	private void InitCategory()
	{
		this.IsShowCategoryBtn(this.mBtnHuoBi, true);
		this.IsShowCategoryBtn(this.mBtnDaoJu, false);
		this.mIsShowHuoBi = true;
		this.mIsShowDaoJu = false;
		this.IsShowHuoBiGoodsIcon = true;
		this.mBtnHuoBi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mIsShowHuoBi)
			{
				return;
			}
			this.IsShowHuoBiGoodsIcon = true;
			this.AddButtons(false, false);
			this.IsShowCategoryBtn(this.mBtnHuoBi, true);
			this.IsShowCategoryBtn(this.mBtnDaoJu, false);
			this.mIsShowHuoBi = true;
			this.mIsShowDaoJu = false;
		};
		this.mBtnDaoJu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mIsShowDaoJu)
			{
				return;
			}
			this.IsShowDaoJuGoodsIcon = true;
			this.AddButtons(true, this.mCheckBox.Check);
			this.IsShowCategoryBtn(this.mBtnHuoBi, false);
			this.IsShowCategoryBtn(this.mBtnDaoJu, true);
			this.mIsShowHuoBi = false;
			this.mIsShowDaoJu = true;
		};
		this.mCheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.mIsUseDaoJu = ((!this.mCheckBox.Check) ? 0 : 1);
			this.InitAttr(this.CurrentGoodType);
		};
		this.mCheckBox.Check = true;
		this.mIsUseDaoJu = ((!this.mCheckBox.Check) ? 0 : 1);
	}

	private void IsShowCategoryBtn(GButton btn, bool isShow = false)
	{
		btn.normalSprite = ((!isShow) ? "++" : "--");
		btn.hoverSprite = ((!isShow) ? "++" : "--");
		btn.pressedSprite = ((!isShow) ? "++" : "--");
		btn.Refresh();
	}

	public void InitPartSize()
	{
		this.Input.Text = "1";
		this.XScrollBar.scrollValue = 0f;
	}

	private void SetMaxNum(int goodtype)
	{
		this.GoodType = goodtype;
		int num = int.Parse(this.GetGoodValue(goodtype));
		int num2 = 0;
		if (Global.Data.MyAlchemyData != null && Global.Data.MyAlchemyData.ToDayCost != null && Global.Data.MyAlchemyData.ToDayCost.ContainsKey(goodtype) && this.DicConversion.ContainsKey(goodtype))
		{
			num2 = this.DicConversion[goodtype].Limit - Global.Data.MyAlchemyData.ToDayCost[goodtype];
		}
		else if (this.DicConversion.ContainsKey(goodtype))
		{
			num2 = this.DicConversion[goodtype].Limit;
		}
		if (num >= num2)
		{
			this.MaxNum = num2;
		}
		else
		{
			this.MaxNum = num - num % this.DicConversion[goodtype].Unit;
		}
	}

	public void InitAttr(int goodType)
	{
		this.CurrentGoodType = goodType;
		this.WuPinLeft.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.DicConversion[goodType].Unit
		});
		this.YuanSu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.DicConversion[goodType].Element
		});
		this.ShengYuWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("剩余{0}:"), this.DicConversion[goodType].Name)
		});
		this.XiaoHaoWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("消耗{0}:"), this.DicConversion[goodType].Name)
		});
		this.DuiHuanWuPin.spriteName = goodType.ToString();
		this.ShengYuWuPin.spriteName = goodType.ToString();
		this.XiaoHaoType.spriteName = goodType.ToString();
		this.goodsTexture1.URL = "NetImages/GameRes/Images/Goods/" + goodType + ".png.qj";
		this.goodsTexture2.URL = "NetImages/GameRes/Images/Goods/" + goodType + ".png.qj";
		this.goodsTexture3.URL = "NetImages/GameRes/Images/Goods/" + goodType + ".png.qj";
		if (Global.Data.MyAlchemyData != null && Global.Data.MyAlchemyData.ToDayCost != null && Global.Data.MyAlchemyData.ToDayCost.ContainsKey(goodType) && this.DicConversion.ContainsKey(goodType))
		{
			string text = "e3b36c";
			if (Global.Data.MyAlchemyData.ToDayCost[goodType] >= this.DicConversion[goodType].Limit)
			{
				text = "FF0000";
			}
			this.ShiYongShangXianLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				Global.GetLang("今日使用上限：")
			});
			this.ShiYongShangXianNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format("{0}/{1}", Global.Data.MyAlchemyData.ToDayCost[goodType], this.DicConversion[goodType].Limit)
			});
		}
		else if (this.DicConversion.ContainsKey(goodType))
		{
			this.ShiYongShangXianNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("0/{0}", this.DicConversion[goodType].Limit)
			});
			this.ShiYongShangXianLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("今日使用上限：")
			});
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"DicConversion字典不包含此字段"
			});
		}
		if (int.Parse(this.GetGoodValue(goodType)) < this.DicConversion[goodType].Unit)
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				this.GetGoodValue(goodType)
			});
			if (this.CacheBtn != null && this.CacheBtn.Count > 0)
			{
				this.SetBtnSpriteState(goodType, this.CacheBtn[goodType]);
			}
		}
		else
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.GetGoodValue(goodType)
			});
			if (this.CacheBtn != null && this.CacheBtn.Count > 0)
			{
				this.SetBtnSpriteState(goodType, this.CacheBtn[goodType]);
			}
		}
		this.SetMaxNum(goodType);
		this.InputNum = 0;
		this.DuiHuanYuanSuNum.text = "0";
		this.Input.Text = this.InputNum.ToString();
		if (this.InputNum <= 0)
		{
			this.XScrollBar.scrollValue = 0f;
		}
		this.CostNum = this.InputNum;
		this.DuiHuanNum = 0;
		int num = this.DicConversion[goodType].ID - 1;
		if (num < this.mOBC.Count)
		{
			AlchemyAddPartBtn btn = U3DUtils.AS<AlchemyAddPartBtn>(this.mOBC[num].gameObject);
			this.SetBtnSpriteState(goodType, btn);
		}
	}

	private string GetGoodValue(int goodtype)
	{
		if (goodtype == 1)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
		}
		if (goodtype == 2)
		{
			return Global.Data.ChengJiuData.ChengJiuPoints.ToString();
		}
		if (goodtype == 3)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang).ToString();
		}
		if (goodtype == 4)
		{
			return Global.Data.roleData.StarSoulValue.ToString();
		}
		if (goodtype == 5)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
		}
		if (goodtype == 6)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.YuansuFenmo).ToString();
		}
		if (goodtype == 7)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian).ToString();
		}
		if (goodtype == 8)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.GuardStatue).ToString();
		}
		if (goodtype == 9)
		{
			return Global.Data.roleData.TianTiRongYao.ToString();
		}
		if (goodtype == 10)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.FluorescentPoint).ToString();
		}
		if (goodtype == 11)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.LangHunFenMo).ToString();
		}
		if (goodtype == 12)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShenLiJingHua).ToString();
		}
		if (goodtype == 13)
		{
			return Global.Data.roleData.BangGong.ToString();
		}
		if (goodtype == 14)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.CharmPoint).ToString();
		}
		if (Global.Data != null && Global.Data.roleData.GoodsDataList != null)
		{
			int num = 0;
			if (this.mIsUseDaoJu == 1)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList.Find((GoodsData result) => result.GoodsID == goodtype && result.Binding == 1);
				if (goodsData != null)
				{
					num = goodsData.GCount;
				}
			}
			else
			{
				GoodsData goodsData2 = Global.Data.roleData.GoodsDataList.Find((GoodsData result) => result.GoodsID == goodtype && result.Binding == 1);
				GoodsData goodsData3 = Global.Data.roleData.GoodsDataList.Find((GoodsData result) => result.GoodsID == goodtype && result.Binding == 0);
				int num2 = 0;
				int num3 = 0;
				if (goodsData2 != null)
				{
					num2 = goodsData2.GCount;
				}
				if (goodsData3 != null)
				{
					num3 = goodsData3.GCount;
				}
				num = num2 + num3;
			}
			return num.ToString();
		}
		return "0";
	}

	private void StartMoRenZhi()
	{
		if (this.mOBC.Count <= 0)
		{
			return;
		}
		AlchemyAddPartBtn alchemyAddPartBtn = U3DUtils.AS<AlchemyAddPartBtn>(this.mOBC[0].gameObject);
		this.InitAttr(alchemyAddPartBtn.Type);
		this.CostType = alchemyAddPartBtn.Type;
		this.BtnItem = alchemyAddPartBtn;
		alchemyAddPartBtn.bak.spriteName = "splitline";
		alchemyAddPartBtn.lable.color = NGUIMath.HexToColorEx(15461355U);
	}

	private void AddButtons(bool isShowDaoJu = false, bool isFilter = false)
	{
		this.mOBC.Clear();
		this.CacheBtn.Clear();
		this.IsShowCheckBox = isShowDaoJu;
		this.DicConversion = AlchemyPart.GetDicCurrencyConversion();
		if (isFilter)
		{
		}
		Dictionary<int, CurrencyConversion>.Enumerator enumerator = this.DicConversion.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			if (isShowDaoJu)
			{
				KeyValuePair<int, CurrencyConversion> keyValuePair = enumerator.Current;
				if (keyValuePair.Key < 100)
				{
					continue;
				}
			}
			else
			{
				KeyValuePair<int, CurrencyConversion> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Key >= 100)
				{
					continue;
				}
			}
			AlchemyAddPartBtn alchemyAddPartBtn = U3DUtils.NEW<AlchemyAddPartBtn>();
			AlchemyAddPartBtn alchemyAddPartBtn2 = alchemyAddPartBtn;
			KeyValuePair<int, CurrencyConversion> keyValuePair3 = enumerator.Current;
			alchemyAddPartBtn2.Type = keyValuePair3.Key;
			AlchemyAddPartBtn alchemyAddPartBtn3 = alchemyAddPartBtn;
			KeyValuePair<int, CurrencyConversion> keyValuePair4 = enumerator.Current;
			alchemyAddPartBtn3.BtnName = keyValuePair4.Value.Name;
			alchemyAddPartBtn.Index = num;
			Dictionary<int, AlchemyAddPartBtn> cacheBtn = this.CacheBtn;
			KeyValuePair<int, CurrencyConversion> keyValuePair5 = enumerator.Current;
			cacheBtn.Add(keyValuePair5.Key, alchemyAddPartBtn);
			KeyValuePair<int, CurrencyConversion> keyValuePair6 = enumerator.Current;
			this.SetBtnSpriteState(keyValuePair6.Key, alchemyAddPartBtn);
			alchemyAddPartBtn.bak.spriteName = "splitline";
			alchemyAddPartBtn.lable.color = NGUIMath.HexToColorEx(8350293U);
			this.mOBC.AddNoUpdate(alchemyAddPartBtn);
			UIPanel component = alchemyAddPartBtn.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			num++;
		}
		this.StartMoRenZhi();
	}

	private Dictionary<int, CurrencyConversion> FilterBindGoods(Dictionary<int, CurrencyConversion> dict)
	{
		Dictionary<int, CurrencyConversion> dictionary = new Dictionary<int, CurrencyConversion>();
		Dictionary<int, CurrencyConversion>.Enumerator itr = dict.GetEnumerator();
		while (itr.MoveNext())
		{
			if (Global.Data.roleData != null && Global.Data.roleData.GoodsDataList != null)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList.Find(delegate(GoodsData result)
				{
					int goodsID = result.GoodsID;
					KeyValuePair<int, CurrencyConversion> keyValuePair4 = itr.Current;
					return goodsID == keyValuePair4.Value.ID;
				});
				if (goodsData != null && goodsData.Binding == 1)
				{
					Dictionary<int, CurrencyConversion> dictionary2 = dictionary;
					KeyValuePair<int, CurrencyConversion> keyValuePair = itr.Current;
					if (!dictionary2.ContainsKey(keyValuePair.Key))
					{
						Dictionary<int, CurrencyConversion> dictionary3 = dictionary;
						KeyValuePair<int, CurrencyConversion> keyValuePair2 = itr.Current;
						int key = keyValuePair2.Key;
						KeyValuePair<int, CurrencyConversion> keyValuePair3 = itr.Current;
						dictionary3.Add(key, keyValuePair3.Value);
					}
				}
			}
		}
		return dictionary;
	}

	private void SetBtnSpriteState(int goodType, AlchemyAddPartBtn btn)
	{
		string goodValue = this.GetGoodValue(goodType);
		if (string.IsNullOrEmpty(goodValue))
		{
			btn.IsEnabled = false;
			return;
		}
		if (int.Parse(goodValue) >= this.DicConversion[goodType].Unit)
		{
			if (Global.Data.MyAlchemyData != null && Global.Data.MyAlchemyData.ToDayCost.ContainsKey(goodType) && Global.Data.MyAlchemyData.ToDayCost[goodType] >= this.DicConversion[goodType].Limit)
			{
				btn.IsEnabled = false;
			}
			else
			{
				btn.IsEnabled = true;
			}
		}
		else
		{
			btn.IsEnabled = false;
		}
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		AlchemyAddPartBtn alchemyAddPartBtn = U3DUtils.AS<AlchemyAddPartBtn>(this.BtnGoods.SelectedItem);
		if (null == alchemyAddPartBtn)
		{
			return;
		}
		this.InitAttr(alchemyAddPartBtn.Type);
		this.CostType = alchemyAddPartBtn.Type;
		this.ShengYuWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("剩余{0}:"), this.DicConversion[this.CostType].Name)
		});
		this.XiaoHaoWuPinLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("消耗{0}:"), this.DicConversion[this.CostType].Name)
		});
		this.InputNum = 0;
		this.Input.Text = this.InputNum.ToString();
		this.DuiHuanYuanSuNum.text = (this.DicConversion[this.GoodType].Element * (this.InputNum / this.DicConversion[this.GoodType].Unit)).ToString();
		if (int.Parse(this.GetGoodValue(alchemyAddPartBtn.Type)) < this.DicConversion[alchemyAddPartBtn.Type].Unit)
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				this.GetGoodValue(alchemyAddPartBtn.Type)
			});
		}
		else
		{
			this.ShengYuWuPinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.GetGoodValue(alchemyAddPartBtn.Type)
			});
		}
		if (this.InputNum <= 0)
		{
			this.XScrollBar.scrollValue = 0f;
		}
		if (this.BtnItem != null && this.BtnItem != alchemyAddPartBtn)
		{
			this.BtnItem.bak.spriteName = "splitline";
			this.BtnItem.lable.color = NGUIMath.HexToColorEx(8350293U);
		}
		if (alchemyAddPartBtn == this.BtnItem)
		{
			return;
		}
		this.BtnItem = alchemyAddPartBtn;
		this.BtnItem.bak.spriteName = "splitline";
		alchemyAddPartBtn.lable.color = NGUIMath.HexToColorEx(15461355U);
	}

	private const string OpenCategoryName = "--";

	private const string CloseCategoryName = "++";

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel Title;

	public UILabel DuiHuanBiLi;

	public UILabel WuPinLeft;

	public UILabel YuanSu;

	public UILabel ShiYongShangXianLab;

	public UILabel ShiYongShangXianNum;

	public UILabel ShengYuWuPinLab;

	public UILabel ShengYuWuPinNum;

	public UILabel XiaoHaoWuPinLab;

	public UISprite XiaoHaoType;

	public UILabel DuiHuanYuanSuLab;

	public UILabel DuiHuanYuanSuNum;

	public GButton BtnClose;

	public GButton BtnGuanZhu;

	public ListBox BtnGoods;

	public UISprite DuiHuanWuPin;

	public UISprite ShengYuWuPin;

	public UIButton AddIcon;

	public UIButton SubIcon;

	public TextBox Input;

	public UIScrollBar XScrollBar;

	private int MaxNum;

	private int InputNum;

	private int DuiHuanNum;

	private int GoodType = 1;

	private ObservableCollection mOBC;

	private Dictionary<int, CurrencyConversion> DicConversion;

	private int CostType = 1;

	private int CostNum;

	public GButton mBtnHuoBi;

	public GButton mBtnDaoJu;

	private bool mIsShowHuoBi;

	private bool mIsShowDaoJu;

	public int CurrentGoodType = -1;

	public GCheckBox mCheckBox;

	private int mIsUseDaoJu;

	public ShowNetImage goodsTexture1;

	public ShowNetImage goodsTexture2;

	public ShowNetImage goodsTexture3;

	private Dictionary<int, AlchemyAddPartBtn> CacheBtn = new Dictionary<int, AlchemyAddPartBtn>();

	private AlchemyAddPartBtn BtnItem;
}
