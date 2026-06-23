using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class GGoodTips : UserControl
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		if (this.goodsID == SystemHelpMgr.ActiveGoodsID)
		{
			SystemHelpMgr.OnAction(UIObjIDs.GGoodTips, HelpStateEvents.Inactived, this.goodsID);
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.ShiyongIcon, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.BuyNumIcon, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void OnMenuListRepisition()
	{
		if (this.goodsID == SystemHelpMgr.ActiveGoodsID)
		{
			SystemHelpMgr.OnAction(UIObjIDs.GGoodTips, HelpStateEvents.Actived, this.goodsID);
		}
	}

	private void InitTextInPrefabs()
	{
		this.txtSalePrice.Text = string.Empty;
		this.ShiyongIcon.Text = Global.GetLang("使用");
		this.ChushouIcon.Text = Global.GetLang("出售");
		this.FangruIcon.Text = Global.GetLang("放入");
		this.QuhuiIcon.Text = Global.GetLang("取回");
		this.m_BtnShangJia.Text = Global.GetLang("上架");
		this.m_BtnCuiHui.Text = Global.GetLang("摧毁");
		this.BuyNumIcon.Text = Global.GetLang("购买");
		this.m_BtnJuanXian.Text = Global.GetLang("捐献");
		this.m_BtnShouJi.Text = Global.GetLang("收集");
		this._GoodsObtainBtn.Text = Global.GetLang("前往获得");
		if (this.ConstTexts != null && this.ConstTexts.Length == 1)
		{
			this.ConstTexts[0].Text = Global.GetLang("数量");
		}
		this.txtType.Pivot = 3;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.GoodIcon.Width = 64.0;
		this.GoodIcon.Height = 64.0;
		this.GoodIcon.isAutoSize = true;
		this.MenusList.onReposition = new UITable.OnReposition(this.OnMenuListRepisition);
		this.DPSelectedItemNum = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.BuyNumIcon.BtnTag == TipsOperationTypes.Goumai.ToString())
			{
				int num = e.ID;
				if (num <= 0)
				{
					Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
					return;
				}
				if (num > this.MaxBuyNum)
				{
					if (this.isMallOrNPCSale)
					{
						num = this.MaxBuyNum;
						this.BuyNum = num;
						this.BuyNum = Math.Min(this.BuyNum, this.MaxBuyNum);
						this.BuyNum = Math.Min(this.BuyNum, Global.GetGoodsGridNumByID(this._goodsData.GoodsID));
						this.FootBuyTxtNum.text = this.BuyNum.ToString();
						this.RefreshPrice();
					}
					else if (this.PriceTypeIndex == 0 || this.PriceTypeIndex == 4)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
					}
					else if (this.PriceTypeIndex == 2)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
					}
					else if (this.PriceTypeIndex == 5)
					{
						num = this.MaxBuyNum;
						this.BuyNum = num;
						this.BuyNum = Math.Min(this.BuyNum, this.MaxBuyNum);
						this.FootBuyTxtNum.text = this.BuyNum.ToString();
						this.RefreshPrice();
					}
					else
					{
						Super.HintMainText(string.Format(Global.GetLang("{0}不足"), GTipServiceEx.PriceInfoUnits[this.PriceTypeIndex]), 10, 3);
					}
					return;
				}
				this.BuyNum = num;
				this.BuyNum = Math.Min(this.BuyNum, this.MaxBuyNum);
				this.BuyNum = Math.Min(this.BuyNum, Global.GetGoodsGridNumByID(this._goodsData.GoodsID));
				this.FootBuyTxtNum.text = this.BuyNum.ToString();
				this.RefreshPrice();
			}
			else if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.Caifen.ToString())
			{
				int id = e.ID;
				if (id <= 0)
				{
					Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
				}
				else if (id > this.MaxCaifenNum)
				{
					Super.HintMainText(Global.GetLang("超过了最大可拆分的数量！"), 10, 3);
				}
				else
				{
					this.CaifenNum = id;
				}
				this.CaifenNum = Math.Min(this.CaifenNum, this.MaxCaifenNum);
				this.FootBuyTxtNum.text = this.CaifenNum.ToString();
			}
		};
		if (this.callback != null)
		{
			this.TipsCallBack(TipsOperationTypes.Close, 0);
		}
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Close, 0);
		};
		if (null != this.m_BtnCuiHui)
		{
			this.m_BtnCuiHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (Application.platform == 7)
				{
					GameInstance.Game.DestroyGoods(this._goodsData.Id);
					this.TipsCallBack(TipsOperationTypes.Close, 0);
				}
			};
		}
		if (null != this.m_BtnJuanXian)
		{
			this.m_BtnJuanXian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (PlayZone.GlobalPlayZone.m_JiYuanHuoDongPart == null)
				{
					PlayZone.GlobalPlayZone.OpenJiYuanHuoDongWindow(2);
					this.TipsCallBack(TipsOperationTypes.Close, 0);
				}
			};
		}
		if (null != this.m_BtnShouJi)
		{
			this.m_BtnShouJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (PlayZone.GlobalPlayZone.m_JiYuanHuoDongPart != null)
				{
					PlayZone.GlobalPlayZone.m_JiYuanHuoDongPart.SetChuanSong(this._goodsData.GoodsID);
					this.TipsCallBack(TipsOperationTypes.Close, 0);
				}
			};
		}
		this.ShiyongIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._goodsData != null && Global.IsCanUseGoodsByGongnengID(this._goodsData.GoodsID))
			{
				this.TipsCallBack(TipsOperationTypes.Close, 0);
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this._goodsData.GoodsID);
				bool isBatchUse = !(goodsXmlNodeByID.PiLiangUse == "0");
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				if (goodsXmlNodeByID.Categoriy == 24 || goodsXmlNodeByID.Categoriy == 8 || goodsXmlNodeByID.Categoriy == 25 || goodsXmlNodeByID.Categoriy == 26)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("时装使用后可在衣橱中穿戴，放入衣橱后无法再取出 ？"), delegate(object s1, DPSelectedItemEventArgs e1)
					{
						if (e1.ID == 0)
						{
							Global.ToUseGoods(this._goodsData, true, isBatchUse);
						}
					}, buttons);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.ToUseGoods(this._goodsData, true, isBatchUse);
						}
						return true;
					};
				}
				else if (goodsXmlNodeByID.Categoriy == 28)
				{
					string[] buttons2 = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					GChildWindow messageBoxWindow = Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("时装使用后可在变身中穿戴，放入后无法再取出 ？"), delegate(object s1, DPSelectedItemEventArgs e1)
					{
						if (e1.ID == 0)
						{
							Global.ToUseGoods(this._goodsData, true, isBatchUse);
						}
					}, buttons2);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.ToUseGoods(this._goodsData, true, isBatchUse);
						}
						return true;
					};
				}
				else if (goodsXmlNodeByID.Categoriy == 27)
				{
					string[] buttons3 = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("时装使用后可在坐骑时装界面穿戴，使用后无法再取出 ？"), delegate(object s1, DPSelectedItemEventArgs e1)
						{
							if (e1.ID == 0)
							{
								Global.ToUseGoods(this._goodsData, true, isBatchUse);
							}
						}, buttons3);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								Global.ToUseGoods(this._goodsData, true, isBatchUse);
							}
							return true;
						};
					}
				}
				else
				{
					Global.ToUseGoods(this._goodsData, true, isBatchUse);
				}
				SystemHelpMgr.OnAction(UIObjIDs.UseGoodsBtn, HelpStateEvents.Clicked, -1);
			}
		};
		this.ChushouIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._goodsData != null)
			{
				GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(this._goodsData.GoodsID);
				if (Global.IsRebornGood(goodVO))
				{
					if (Super.IsImportantGoods(this._goodsData.GoodsID) && Super.MessageBoxIsHint[2] == 0)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认出售？"), 2, null, MessBoxIsHintTypes.ImportantGoodsSaleNeedHint);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								this.ChuShouChongShengGood(this._goodsData, goodVO, this._goodsData.Id.ToString());
							}
							return true;
						};
					}
					else
					{
						this.ChuShouChongShengGood(this._goodsData, goodVO, this._goodsData.Id.ToString());
					}
					return;
				}
				if (Super.IsImportantGoods(this._goodsData.GoodsID) && Super.MessageBoxIsHint[2] == 0)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认出售？"), 2, null, MessBoxIsHintTypes.ImportantGoodsSaleNeedHint);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.TipsCallBack(TipsOperationTypes.Chushou, 0);
						}
						return true;
					};
				}
				else
				{
					this.TipsCallBack(TipsOperationTypes.Chushou, 0);
				}
			}
		};
		this.FangruIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Fangru, 0);
		};
		this.QuhuiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Quhui, 0);
		};
		if (null != this.m_BtnShangJia)
		{
			this.m_BtnShangJia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.TipsCallBack(TipsOperationTypes.ShangJia, 0);
			};
		}
		this.m_BtnMaxNum.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = ConvertExt.SafeConvertToInt32(this.FootBuyTxtNum.text);
			if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.Caifen.ToString())
			{
				if (num > this.MaxCaifenNum)
				{
					Super.HintMainText(Global.GetLang("超过了最大可拆分的数量！"), 10, 3);
				}
				else if (num > 0)
				{
					this.TipsCallBack(TipsOperationTypes.Caifen, num);
				}
				else
				{
					Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
				}
			}
			if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.Max.ToString())
			{
				int num2 = Global.GetGoodsGridNumByID(this._goodsData.GoodsID);
				if (this._goodsOwner == GoodsOwnerTypes.NPCSale || this._goodsOwner == GoodsOwnerTypes.mallSale)
				{
					int tempBuyNum;
					if (num2 < this.MaxBuyNum)
					{
						this.FootBuyTxtNum.text = Convert.ToString(num2);
						tempBuyNum = num2;
					}
					else
					{
						this.FootBuyTxtNum.text = Convert.ToString(this.MaxBuyNum);
						tempBuyNum = this.MaxBuyNum;
					}
					this.RefreshTempPrice(tempBuyNum, true);
				}
				else
				{
					if (num2 < this.MaxBuyNum)
					{
						this.FootBuyTxtNum.text = Convert.ToString(num2);
						this.FootBuyTxtNum.color = Color.white;
					}
					else
					{
						this.FootBuyTxtNum.text = Convert.ToString(num2);
						this.FootBuyTxtNum.color = Color.red;
					}
					if (this._goodsOwner == GoodsOwnerTypes.WangZheShangCheng)
					{
						num2 = this.MaxBuyNum;
						this.FootBuyTxtNum.text = Convert.ToString(num2);
						this.FootBuyTxtNum.color = Color.white;
					}
					this.RefreshTempPrice(num2, false);
				}
			}
			if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
			{
			}
		};
		this.BuyNumIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = ConvertExt.SafeConvertToInt32(this.FootBuyTxtNum.text);
			if (this.BuyNumIcon.BtnTag == TipsOperationTypes.Goumai.ToString() || this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
			{
				if (this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
				{
					num = 1;
				}
				if (9999 < num)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("输入数量错误！"), new object[0]), 0, -1, -1, 0);
					this.FootBuyTxtNum.text = Global.GetLang("1");
					return;
				}
				if (num > this.MaxBuyNum)
				{
					if (this.PriceTypeIndex == 0 || this.PriceTypeIndex == 4)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
					}
					else if (this.PriceTypeIndex == 2)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
					}
					else
					{
						Super.HintMainText(string.Format(Global.GetLang("{0}不足"), GTipServiceEx.PriceInfoUnits[this.PriceTypeIndex]), 10, 3);
					}
				}
				else if (num > 0)
				{
					if (this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
					{
						this.TipsCallBack(TipsOperationTypes.JiaoYiShuoGouMai, num);
					}
					else
					{
						GoodsPriceUnitTypes goodsPriceUnitTypes = GoodsPriceUnitTypes.None;
						if (!string.IsNullOrEmpty(this._priceInfo))
						{
							goodsPriceUnitTypes = (GoodsPriceUnitTypes)this._priceInfo.Split(new char[]
							{
								','
							})[0].SafeToInt32(0);
						}
						if (this._goodsOwner == GoodsOwnerTypes.mallSale && goodsPriceUnitTypes == GoodsPriceUnitTypes.Zhuanshi)
						{
							GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.BuyOnePrice * num)
							}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
							messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
							{
								int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
								Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
								if (messageBoxReturn == 0)
								{
									this.TipsCallBack(TipsOperationTypes.Goumai, num);
								}
								return true;
							};
							return;
						}
						this.TipsCallBack(TipsOperationTypes.Goumai, num);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
				}
			}
			else if (this.BuyNumIcon.BtnTag == TipsOperationTypes.OtherStallGouMai.ToString())
			{
				this.TipsCallBack(TipsOperationTypes.OtherStallGouMai, 0);
			}
		};
		UIEventListener.Get(this.FootBuyTxtNumContainer).onClick = delegate(GameObject s)
		{
			if (this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
			{
				return;
			}
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DPSelectedItemNum, this.FootBuyTxtNum, 0, -100);
		};
		UIEventListener.Get(this.AddIcon.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (!this.AddIcon.isEnabled)
			{
				return;
			}
			if (b)
			{
				base.InvokeRepeating("AddNum", 0.1f, 0.1f);
			}
			else
			{
				base.CancelInvoke("AddNum");
			}
		};
		UIEventListener.Get(this.SubIcon.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (!this.SubIcon.isEnabled)
			{
				return;
			}
			if (b)
			{
				base.InvokeRepeating("SubNum", 0.1f, 0.1f);
			}
			else
			{
				base.CancelInvoke("SubNum");
			}
		};
		UIEventListener.Get(this.AddIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.AddIcon.isEnabled)
			{
				return;
			}
			this.AddNum();
		};
		UIEventListener.Get(this.SubIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.SubIcon.isEnabled)
			{
				return;
			}
			this.SubNum();
		};
		this._GoodsObtainBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (!ConfigGoodsObtain.GetGoodsObtainCanShowOnTipsByGoodsID(this._goodsData.GoodsID))
			{
				MUDebug.Log<string>(new string[]
				{
					"GoodsID = " + this.goodsID + Global.GetLang("不在TIPS上显示获得途径")
				});
				return;
			}
			Super.ShowGoodsGuideForGoodsTips(this._goodsData.GoodsID, null);
			this.TipsCallBack(TipsOperationTypes.Close, 0);
		};
	}

	private void ChuShouChongShengGood(GoodsData gd, GoodVO goodVO, string ids)
	{
		if (Global.IsRebornGood(goodVO))
		{
			if (Global.IsRebornBaoShi(goodVO.Categoriy) || Global.IsRebornXuanCai(goodVO.Categoriy))
			{
				if (Global.IsNeedSaleRebornTiShi(gd) && Super.MessageBoxIsHint[14] != 1)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认回收？"), 2, null, MessBoxIsHintTypes.ChongShengBaoShi);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendChongShengPiLiangFenJie(ids);
							this.TipsCallBack(TipsOperationTypes.Close, 0);
						}
						return true;
					};
				}
				else
				{
					GameInstance.Game.SendChongShengPiLiangFenJie(ids);
					this.TipsCallBack(TipsOperationTypes.Close, 0);
				}
			}
			else if (Global.IsNeedSaleRebornTiShi(gd))
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认回收？"), 2, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(ids);
						this.TipsCallBack(TipsOperationTypes.Close, 0);
					}
					return true;
				};
			}
			else
			{
				GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(ids);
				this.TipsCallBack(TipsOperationTypes.Close, 0);
			}
		}
	}

	public void RenderTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData, bool selfBagOnly, string priceInfo = null)
	{
		this.goodsID = goodsData.GoodsID;
		this._goodsData = goodsData;
		this._goodsOwner = goodsOwner;
		GGoodTips.LastGoodsID = this.goodsID;
		if (priceInfo != null)
		{
			this._priceInfo = priceInfo;
		}
		this.SetText(goodsOwner, goodsData, priceInfo);
		this.SetMenus(goodsOwner, goodsData, selfBagOnly);
		this.SetPropsPanel(goodsOwner, goodsData);
	}

	private void SetPropsPanel(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		int num = 185;
		int num2 = 36;
		int num3 = 20;
		int num4 = -95;
		int num5 = 47;
		if (this.txtActiveTime.gameObject.activeSelf || this.txtUseLimited.gameObject.activeSelf)
		{
			if (this.txtActiveTime.gameObject.activeSelf)
			{
				num += 13;
				num5 += -14;
				num4 += -14;
				this.Body.transform.localPosition = new Vector3(this.Body.transform.localPosition.x, (float)num5, 0f);
				this.Foot.transform.localPosition = new Vector3(this.Foot.transform.localPosition.x, (float)num4, 0f);
			}
			if (this.txtUseLimited.gameObject.activeSelf)
			{
				if (!this.txtActiveTime.gameObject.activeSelf)
				{
					num += 13;
				}
				num += 13;
				num5 += -14;
				num4 += -14;
				this.Body.transform.localPosition = new Vector3(this.Body.transform.localPosition.x, (float)num5, 0f);
				this.Foot.transform.localPosition = new Vector3(this.Foot.transform.localPosition.x, (float)num4, 0f);
			}
		}
		else
		{
			this.Body.transform.localPosition = new Vector3(this.Body.transform.localPosition.x, (float)num5, 0f);
			this.Foot.transform.localPosition = new Vector3(this.Foot.transform.localPosition.x, (float)num4, 0f);
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		Dictionary<string, int> goodsToTypeLimitString = Global.GetGoodsToTypeLimitString(goodsData.GoodsID);
		if (goodsOwner == GoodsOwnerTypes.NPCSale || goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.OtherOnSale || goodsOwner == GoodsOwnerTypes.JiaoYiShuo || goodsOwner == GoodsOwnerTypes.WangZheShangCheng || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
		{
			UISprite component = this.AddIcon.tweenTarget.GetComponent<UISprite>();
			UISprite component2 = this.SubIcon.tweenTarget.GetComponent<UISprite>();
			if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
			{
				this.BuyNumIcon.BtnTag = TipsOperationTypes.JiaoYiShuoGouMai.ToString();
				this.m_BtnMaxNum.Label.text = Global.GetLang("最大");
				this.m_BtnMaxNum.isEnabled = false;
				this.AddIcon.isEnabled = false;
				this.SubIcon.isEnabled = false;
				component.spriteName = this.addIconSpriteNames[1];
				component2.spriteName = this.subIconSpriteNames[1];
			}
			else
			{
				this.AddIcon.isEnabled = true;
				this.SubIcon.isEnabled = true;
				component.spriteName = this.addIconSpriteNames[0];
				component2.spriteName = this.subIconSpriteNames[0];
				if (goodsOwner == GoodsOwnerTypes.OtherOnSale)
				{
					this.BuyNumIcon.BtnTag = TipsOperationTypes.OtherStallGouMai.ToString();
					this.AddIcon.gameObject.SetActive(false);
					this.SubIcon.gameObject.SetActive(false);
				}
				else
				{
					this.BuyNumIcon.BtnTag = TipsOperationTypes.Goumai.ToString();
				}
			}
			this.BuyNumIcon.Text = Global.GetLang("购买");
			if (goodsOwner != GoodsOwnerTypes.JiaoYiShuo)
			{
				this.m_BtnMaxNum.BtnTag = TipsOperationTypes.Max.ToString();
				this.m_BtnMaxNum.Text = Global.GetLang("最大");
			}
			if (goodsXmlNodeByID != null && goodsXmlNodeByID.Categoriy == 121)
			{
				this.FootBuy.Visibility = false;
			}
			else if (goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
			{
				this.FootBuy.Visibility = false;
			}
			else
			{
				this.FootBuy.Visibility = true;
				num2 = 100;
			}
		}
		else if (goodsXmlNodeByID != null && (goodsOwner == GoodsOwnerTypes.SelfBag || goodsOwner == GoodsOwnerTypes.HolyBagShengJi || goodsOwner == GoodsOwnerTypes.HolyBag) && goodsData.GCount > 1 && goodsToTypeLimitString.Count == 0 && (goodsData.Endtime == null || goodsData.Endtime == "1900-01-01 12:00:00") && goodsXmlNodeByID.Categoriy != 121)
		{
			this.BuyNumIcon.BtnTag = TipsOperationTypes.Caifen.ToString();
			num2 = 100;
			this.m_BtnMaxNum.BtnTag = TipsOperationTypes.Caifen.ToString();
			this.m_BtnMaxNum.Text = Global.GetLang("拆分");
			this.m_BtnMaxNum.isEnabled = true;
			this.MaxCaifenNum = goodsData.GCount - 1;
			this.CaifenNum = 1;
			this.FootBuyTxtNum.text = this.CaifenNum.ToString();
			this.FootBuy.Visibility = true;
			UISprite component3 = this.AddIcon.tweenTarget.GetComponent<UISprite>();
			UISprite component4 = this.SubIcon.tweenTarget.GetComponent<UISprite>();
			this.AddIcon.isEnabled = true;
			this.SubIcon.isEnabled = true;
			component3.spriteName = this.addIconSpriteNames[0];
			component4.spriteName = this.subIconSpriteNames[0];
		}
		else if (goodsXmlNodeByID != null && goodsOwner == GoodsOwnerTypes.SelfBag && goodsData.GCount > 1 && (goodsData.Endtime == null || goodsData.Endtime == "1900-01-01 12:00:00") && goodsXmlNodeByID.Categoriy != 121)
		{
			bool flag = false;
			foreach (KeyValuePair<string, int> keyValuePair in goodsToTypeLimitString)
			{
				if (keyValuePair.Value == 1 && goodsXmlNodeByID != null && !string.IsNullOrEmpty(goodsXmlNodeByID.ToType.ToString()))
				{
					string[] array = goodsXmlNodeByID.ToType.Split(new char[]
					{
						','
					});
					if (0 < array.Length && "NeedTask" == array[0])
					{
						flag = true;
						this.BuyNumIcon.BtnTag = TipsOperationTypes.Caifen.ToString();
						num2 = 100;
						this.m_BtnMaxNum.BtnTag = TipsOperationTypes.Caifen.ToString();
						this.m_BtnMaxNum.Text = Global.GetLang("拆分");
						this.m_BtnMaxNum.isEnabled = true;
						this.MaxCaifenNum = goodsData.GCount - 1;
						this.CaifenNum = 1;
						this.FootBuyTxtNum.text = this.CaifenNum.ToString();
						this.FootBuy.Visibility = true;
						UISprite component5 = this.AddIcon.tweenTarget.GetComponent<UISprite>();
						UISprite component6 = this.SubIcon.tweenTarget.GetComponent<UISprite>();
						this.AddIcon.isEnabled = true;
						this.SubIcon.isEnabled = true;
						component5.spriteName = this.addIconSpriteNames[0];
						component6.spriteName = this.subIconSpriteNames[0];
						break;
					}
				}
			}
			if (!flag)
			{
				this.FootBuy.Visibility = false;
			}
		}
		else
		{
			this.FootBuy.Visibility = false;
		}
		if (!Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !Global.IsInShiLiZhengBaMap() && ConfigGoodsObtain.GetGoodsObtainCanShowOnTipsByGoodsID(this._goodsData.GoodsID))
		{
			float num6 = 0f;
			if (this.FootBuy.Visibility)
			{
				num6 += 88f;
			}
			else
			{
				num6 = 44f;
			}
			this._GoodsObtainBtn.transform.localPosition = new Vector3(this._GoodsObtainBtn.transform.localPosition.x, num6, this._GoodsObtainBtn.transform.localPosition.z);
			this._GoodsObtainBtn.gameObject.SetActive(true);
		}
		else
		{
			this._GoodsObtainBtn.gameObject.SetActive(false);
		}
		int num7 = (int)this.txtInfo.ActualHeight + 10 + Mathf.FloorToInt(Mathf.Abs(this.txtInfo.transform.localPosition.y) - 20f) + ((!this._GoodsObtainBtn.gameObject.activeSelf) ? 0 : 88);
		if (!string.IsNullOrEmpty(this.txtInfoConTent.Text))
		{
			num7 += 80;
		}
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(num + num3 + num7 + num2), 1f);
		if (this.FootBuy.Visibility)
		{
			this.Foot.localPosition = new Vector3(this.Foot.localPosition.x, this.Body.localPosition.y - (float)num7 - (float)num3 - (float)(num2 / 2), 0f);
			NGUITools.SetActive(this.FootIntroTxtNum.gameObject, false);
		}
		else
		{
			this.Foot.localPosition = new Vector3(this.Foot.localPosition.x, this.Body.localPosition.y - (float)num7 - (float)num3, 0f);
			NGUITools.SetActive(this.FootIntroTxtNum.gameObject, true);
		}
		base.transform.localPosition = new Vector3(0f, (float)((int)((450f - this.Bak.transform.localScale.y) * -0.5f)), 0f);
		this.MenusList.transform.localPosition = new Vector3(this.Menus.transform.localPosition.x, this.Bak.transform.localPosition.y - this.Bak.transform.localScale.y + 20f, 0f);
	}

	private void ResetText()
	{
		this.txtSalePrice.Text = string.Empty;
		this.IconSprite.spriteName = "none";
	}

	private void SetText(GoodsOwnerTypes goodsOwner, GoodsData goodsData, string priceInfo = null)
	{
		this.ResetText();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		this.txtTitle.Text = Global.GetColorStringForNGUIText(new object[]
		{
			goodsXmlNodeByID.GoodsColor,
			goodsXmlNodeByID.Title
		});
		int toZhuanSheng = goodsXmlNodeByID.ToZhuanSheng;
		int num = Global.GMax(goodsXmlNodeByID.ToLevel, 1);
		string text = string.Empty;
		if (toZhuanSheng > 0)
		{
			text = string.Format(Global.GetLang("{0}转{1}级"), toZhuanSheng, num);
		}
		else
		{
			text = string.Format(Global.GetLang("{0}级"), num);
		}
		string text2 = "FFFFFF";
		if (Global.Data.roleData.ChangeLifeCount == toZhuanSheng)
		{
			if (Global.Data.roleData.Level < num)
			{
				text2 = "FF0000";
			}
		}
		else if (Global.Data.roleData.ChangeLifeCount < toZhuanSheng)
		{
			text2 = "FF0000";
		}
		this.txtLevel.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"FFFFFF",
			Global.GetLang("使用等级: "),
			text2,
			text
		});
		text2 = "FFFFFF";
		if (goodsXmlNodeByID.ToOccupation >= 0)
		{
			if ((1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) == 0)
			{
				text2 = "FF0000";
			}
			string occupationStrByGoods = Global.GetOccupationStrByGoods(goodsXmlNodeByID.ToOccupation);
			this.txtOcc.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				Global.GetLang("适用职业: "),
				text2,
				Global.GetLang(occupationStrByGoods)
			});
		}
		else
		{
			this.txtOcc.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				Global.GetLang("适用职业: "),
				text2,
				Global.GetLang("通用")
			});
		}
		this.DateTime_Endtime_Enable = false;
		if (goodsData.Endtime != null && goodsData.Endtime != "1900-01-01 12:00:00")
		{
			string text3 = string.Empty;
			string text4 = string.Empty;
			if (goodsData.Endtime.Contains("-"))
			{
				this.DateTime_Endtime_Enable = true;
				this.DateTime_Endtime = DateTime.Parse(goodsData.Endtime);
				TimeSpan timeSpan = this.DateTime_Endtime - DateTime.Now;
				if (timeSpan.TotalHours > 1.0)
				{
					text3 = string.Concat(new object[]
					{
						(int)timeSpan.TotalDays,
						Global.GetLang("天"),
						timeSpan.Hours,
						Global.GetLang("小时"),
						timeSpan.Minutes,
						Global.GetLang("分钟")
					});
				}
				else
				{
					text3 = string.Concat(new object[]
					{
						(int)timeSpan.TotalMinutes,
						Global.GetLang("分钟"),
						timeSpan.Seconds,
						Global.GetLang("秒")
					});
				}
				text4 = Global.GetLang("有效时间") + Global.GetLang("：");
			}
			else
			{
				text3 = goodsData.Endtime + Global.GetLang("分钟");
				text4 = Global.GetLang("领取后有效时间：");
			}
			this.txtActiveTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				text4,
				"00FF00",
				text3
			});
			this.txtActiveTime.gameObject.SetActive(true);
		}
		else
		{
			this.txtActiveTime.gameObject.SetActive(false);
		}
		this.txtUseLimited.Text = "{FFFFFF}" + Global.GetLang("使用限制") + Global.GetLang("：{-}");
		if (goodsData != null)
		{
			Dictionary<string, int> goodsToTypeLimitString = Global.GetGoodsToTypeLimitString(goodsData.GoodsID);
			foreach (KeyValuePair<string, int> keyValuePair in goodsToTypeLimitString)
			{
				if (keyValuePair.Value == 1)
				{
					TextBlock textBlock = this.txtUseLimited;
					textBlock.Text = textBlock.Text + "{00FF00}" + keyValuePair.Key + "{-}  ";
				}
				else
				{
					TextBlock textBlock2 = this.txtUseLimited;
					textBlock2.Text = textBlock2.Text + "{FF0000}" + keyValuePair.Key + "{-}  ";
				}
			}
			if (goodsToTypeLimitString.Count == 0)
			{
				this.txtUseLimited.gameObject.SetActive(false);
			}
			else
			{
				this.txtUseLimited.gameObject.SetActive(true);
			}
		}
		int categoriy = goodsXmlNodeByID.Categoriy;
		this.txtType.Text = Global.GetGoodsType(categoriy);
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite1.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite15.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite2.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BindingSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.EndTimeSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.NoUseSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.TeXiao.gameObject, false);
		string value = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodsXmlNodeByID)
		});
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 981)
		{
			this.GoodIcon.isAutoSize = false;
		}
		this.GoodIcon.BackSpriteName0 = "bagGrid2_bak";
		this.GoodIcon.BodyURL = new ImageURL(value, false, 0);
		this.GoodIcon.ItemCategory = categoriy;
		this.GoodIcon.ItemCode = goodsData.GoodsID;
		this.GoodIcon.ItemObject = goodsData;
		bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
		Super.InitGoodsGIcon(this.GoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
		if (categoriy != 703 && categoriy != 704)
		{
			if (goodsData.GoodsID != 703000)
			{
				goto IL_68C;
			}
		}
		try
		{
			Vector3 localPosition = this.GoodIcon.TeXiao.transform.localPosition;
			localPosition.z = -0.015f;
			this.GoodIcon.TeXiao.transform.localPosition = localPosition;
		}
		catch (Exception ex)
		{
		}
		IL_68C:
		this.txtInfo.Text = string.Empty;
		this.txtInfoConTent.Text = string.Empty;
		if (goodsXmlNodeByID.Categoriy == 121)
		{
			this.txtInfo.Text = goodsXmlNodeByID.Description + "\r\n ";
		}
		else if (goodsXmlNodeByID.Categoriy == 950)
		{
			this.txtInfoConTent.Text = this.SetChongShengBaoShiContent(goodsXmlNodeByID.ID);
		}
		else if (goodsXmlNodeByID.Categoriy == 960)
		{
			this.txtInfoConTent.Text = this.SetXuanCaiBaoShiContent(goodsXmlNodeByID.ID);
		}
		else
		{
			this.txtInfo.Text = goodsXmlNodeByID.Description;
		}
		if (string.IsNullOrEmpty(this.txtInfo.Text))
		{
			this.txtInfo.Visibility = false;
			SpringPanel component = this.txtInfoConTent.transform.parent.GetComponent<SpringPanel>();
			if (component != null)
			{
				component.target = new Vector3(0f, 39f, 0f);
				component.enabled = true;
			}
		}
		else
		{
			if (1.0 < this.txtUseLimited.ActualHeight / (double)this.txtUseLimited.FontSize)
			{
				float num2 = (float)this.txtUseLimited.ActualHeight - (float)this.txtUseLimited.FontSize;
				this.txtInfo.transform.localPosition = new Vector3(this.txtInfo.transform.localPosition.x, -20f - num2, this.txtInfo.transform.localPosition.z);
			}
			this.txtInfo.Visibility = true;
		}
		int num3 = goodsData.GCount;
		if (goodsData.Id < 0 && goodsOwner == GoodsOwnerTypes.None)
		{
			num3 = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
		}
		if (goodsXmlNodeByID.UsingNum > 1)
		{
			this.FootIntroTxtNum.Text = string.Format(Global.GetLang("剩余次数: {0}"), num3);
		}
		else if (goodsOwner != GoodsOwnerTypes.SysGifts)
		{
			this.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), num3);
		}
		else
		{
			this.FootIntroTxtNum.Text = string.Empty;
		}
		this.FootBuyTxtNum.color = Color.white;
		if (goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.NPCSale || goodsOwner == GoodsOwnerTypes.OtherOnSale || goodsOwner == GoodsOwnerTypes.JiaoYiShuo || goodsOwner == GoodsOwnerTypes.WangZheShangCheng || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
		{
			if (priceInfo != null)
			{
				string[] array = priceInfo.Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					return;
				}
				this.PriceTypeIndex = Convert.ToInt32(array[0]);
				string text5 = GTipServiceEx.PriceInfoUnits[this.PriceTypeIndex];
				this.BuyOnePrice = Convert.ToInt32(array[1]);
				if (goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.NPCSale)
				{
					this.isMallOrNPCSale = true;
					this.MaxBuyNum = this.GetMaxBuyNumForNPCSaleAndMallSale();
				}
				else if (goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
				{
					this.isMallOrNPCSale = true;
					this.MaxBuyNum = this.GetMaxBuyNum(this.PriceTypeIndex, this.BuyOnePrice);
				}
				else
				{
					this.isMallOrNPCSale = false;
					this.MaxBuyNum = this.GetMaxBuyNum(this.PriceTypeIndex, this.BuyOnePrice);
				}
				this.BuyNum = 1;
				if (goodsOwner == GoodsOwnerTypes.WangZheShangCheng)
				{
					MallItem mallItem = null;
					if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.kuaFuWangZheMallPart != null)
					{
						mallItem = PlayZone.GlobalPlayZone.kuaFuWangZheMallPart.GetMallItemByGoodID(this._goodsData.GoodsID);
					}
					if (mallItem != null)
					{
						this.MaxBuyNum = mallItem.nXianGouLeft;
					}
				}
				if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
				{
					this.FootBuyTxtNum.text = goodsData.GCount.ToString();
				}
				else if (goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
				{
					this.FootBuyTxtNum.text = this.BuyNum.ToString();
				}
				else
				{
					this.FootBuyTxtNum.text = this.BuyNum.ToString();
					if (goodsOwner == GoodsOwnerTypes.OtherOnSale)
					{
						if (goodsData.GoodsID == 50200)
						{
							this.FootBuyTxtNum.text = Global.GetLang(Convert.ToString(goodsData.Quality));
						}
						else
						{
							this.FootBuyTxtNum.text = Global.GetLang(Convert.ToString(goodsData.GCount));
						}
					}
				}
				this.RefreshPrice();
			}
		}
		else if (goodsOwner == GoodsOwnerTypes.SelfBag || goodsOwner == GoodsOwnerTypes.ChatGoods)
		{
			int num4 = 0;
			if (goodsData != null)
			{
				num4 = Global.GetGoodsSaleToNpcPrice(goodsData);
				if (num4 <= 0)
				{
					num4 = Global.GetGoodsSaleToNpcZaizao(goodsData);
					this.IconSprite.spriteName = "zaizao";
				}
				else if (categoriy == 23)
				{
					num4 = Global.GetDecorationSaleToNpcPrice(goodsData.GoodsID);
					this.IconSprite.spriteName = "LovePoints";
				}
				else if (goodsData.Binding > 0)
				{
					this.IconSprite.spriteName = "moneyBindJinbi";
				}
				else
				{
					this.IconSprite.spriteName = "moneyJinbi";
				}
			}
			if (num4 > 0)
			{
				this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"C39550",
					Global.GetLang("售价: "),
					"ffffff",
					num4
				});
				this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -15f, 0f);
			}
			else
			{
				this.IconSprite.spriteName = "none";
			}
			if (goodsXmlNodeByID.NoSaleOut == 1)
			{
				this.txtSalePrice.Text = Global.GetLang("{FF0000}不可出售{-}");
			}
		}
		else if (goodsOwner == GoodsOwnerTypes.SysGifts || goodsOwner == GoodsOwnerTypes.SelfStall || goodsOwner == GoodsOwnerTypes.None)
		{
			this.txtSalePrice.Text = string.Empty;
			this.IconSprite.spriteName = "none";
		}
	}

	private new void Update()
	{
		if (this.DateTime_Endtime_Enable && this.txtActiveTime.gameObject.activeInHierarchy && (this.txtActiveTime.text.Contains(Global.GetLang("秒")) || this.txtActiveTime.text.Contains(Global.GetLang("分钟"))))
		{
			TimeSpan timeSpan = this.DateTime_Endtime - Global.GetCorrectDateTime();
			if (timeSpan.Ticks <= 0L)
			{
				return;
			}
			string text = string.Concat(new object[]
			{
				(int)timeSpan.TotalMinutes,
				Global.GetLang("分钟"),
				timeSpan.Seconds,
				Global.GetLang("秒")
			});
			string text2 = Global.GetLang("有效时间") + Global.GetLang("：");
			this.txtActiveTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				text2,
				"00FF00",
				text
			});
		}
	}

	private int GetMaxBuyNumForNPCSaleAndMallSale()
	{
		return ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("MaxPurchase", true));
	}

	private bool IsEnoughMoneyColorForNPCSaleAndMallSale(int goodsPriceUnit, int price, int count)
	{
		int num = 0;
		if (goodsPriceUnit == 0)
		{
			num = Global.Data.roleData.YinLiang;
		}
		else if (goodsPriceUnit == 1)
		{
			num = Global.Data.roleData.Money1;
		}
		else if (goodsPriceUnit == 2)
		{
			num = Global.Data.roleData.UserMoney;
		}
		else if (goodsPriceUnit == 3)
		{
			num = Global.Data.roleData.Gold;
		}
		else if (goodsPriceUnit == 4)
		{
			num = Global.Data.roleData.Money1;
		}
		else if (goodsPriceUnit == 5)
		{
			num = Global.GetRoleOwnNumByMoneyType(112);
		}
		else if (goodsPriceUnit == 6)
		{
			num = Global.GetRoleOwnNumByMoneyType(132);
		}
		return num - price * count >= 0;
	}

	private int GetMaxBuyNum(int goodsPriceUnit, int price)
	{
		int num = 0;
		if (goodsPriceUnit == 0)
		{
			num = Global.Data.roleData.YinLiang;
		}
		else if (goodsPriceUnit == 1)
		{
			num = Global.Data.roleData.Money1;
		}
		else if (goodsPriceUnit == 2)
		{
			num = Global.Data.roleData.UserMoney;
		}
		else if (goodsPriceUnit == 3)
		{
			num = Global.Data.roleData.Gold;
		}
		else if (goodsPriceUnit == 4)
		{
			num = Global.Data.roleData.Money1;
		}
		else if (goodsPriceUnit == 5)
		{
			num = Global.GetRoleOwnNumByMoneyType(112);
		}
		else if (goodsPriceUnit == 6)
		{
			num = Global.GetRoleOwnNumByMoneyType(132);
		}
		if (price == 0)
		{
			return 0;
		}
		return num / price;
	}

	private void RefreshPrice()
	{
		if (this.isMallOrNPCSale)
		{
			bool flag = this.IsEnoughMoneyColorForNPCSaleAndMallSale(this.PriceTypeIndex, this.BuyOnePrice, this.BuyNum);
			string text = (!flag) ? "ff0000" : "ffffff";
			this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("总价: "),
				text,
				this.BuyOnePrice * this.BuyNum
			});
			this.IconSprite.spriteName = GTipServiceEx.PriceIconUnits[this.PriceTypeIndex];
			this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -15f, 0f);
			this.FootBuyTxtNum.color = ((!flag) ? Color.red : Color.white);
		}
		else
		{
			string text2 = (this.BuyNum > this.MaxBuyNum) ? "ff0000" : "ffffff";
			this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("总价: "),
				text2,
				this.BuyOnePrice * this.BuyNum
			});
			this.IconSprite.spriteName = GTipServiceEx.PriceIconUnits[this.PriceTypeIndex];
			this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -15f, 0f);
			if (this.BuyNum <= this.MaxBuyNum)
			{
				this.FootBuyTxtNum.color = Color.white;
			}
			else
			{
				this.FootBuyTxtNum.color = Color.red;
			}
		}
	}

	private void RefreshTempPrice(int tempBuyNum, bool isSpecial = false)
	{
		if (isSpecial)
		{
			bool flag = this.IsEnoughMoneyColorForNPCSaleAndMallSale(this.PriceTypeIndex, this.BuyOnePrice, tempBuyNum);
			string text = (!flag) ? "ff0000" : "ffffff";
			this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("总价: "),
				text,
				this.BuyOnePrice * tempBuyNum
			});
			this.IconSprite.spriteName = GTipServiceEx.PriceIconUnits[this.PriceTypeIndex];
			this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -15f, 0f);
			this.FootBuyTxtNum.color = ((!flag) ? Color.red : Color.white);
		}
		else
		{
			string text2 = (this.BuyNum > this.MaxBuyNum) ? "ff0000" : "ffffff";
			this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("总价: "),
				text2,
				this.BuyOnePrice * this.BuyNum
			});
			this.IconSprite.spriteName = GTipServiceEx.PriceIconUnits[this.PriceTypeIndex];
			this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -15f, 0f);
			if (this.BuyNum <= this.MaxBuyNum)
			{
				this.FootBuyTxtNum.color = Color.white;
			}
			else
			{
				this.FootBuyTxtNum.color = Color.red;
			}
		}
	}

	private string SetChongShengBaoShiContent(int goodsid)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetChongShengBaoShiById(goodsid).ShuXing.Split(new char[]
		{
			'|'
		});
		if (array.Length != 2)
		{
			return null;
		}
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
		return stringBuilder.ToString();
	}

	private string SetXuanCaiBaoShiContent(int goodsid)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<XuanCaiShuXingVO> xuanCaiShuXingByGoodId = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiShuXingByGoodId(goodsid);
		for (int i = 0; i < xuanCaiShuXingByGoodId.Count; i++)
		{
			string[] array = xuanCaiShuXingByGoodId[i].JiHuoShuXing.Split(new char[]
			{
				'|'
			});
			for (int j = 0; j < array.Length; j++)
			{
				if (!string.IsNullOrEmpty(array[j]))
				{
					string[] array2 = array[j].Split(new char[]
					{
						','
					});
					ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]);
					if (j <= 0)
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang(string.Format(Global.GetLang("属性{0}："), i + 1))
						}));
					}
					else
					{
						stringBuilder.Append("              ");
					}
					if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang(string.Format("{0}{1}%", extPropIndexesVOByWord.Description, (float.Parse(array2[1]) * 100f).ToString("f1")) + Environment.NewLine)
						}));
					}
					else
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang(string.Format("{0}{1}", extPropIndexesVOByWord.Description, array2[1]) + Environment.NewLine)
						}));
					}
				}
			}
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(xuanCaiShuXingByGoodId[i].Tips + Environment.NewLine)
			}));
		}
		return stringBuilder.ToString();
	}

	private void SetMenus(GoodsOwnerTypes goodsOwner, GoodsData goodsData, bool selfBagOnly)
	{
		NGUITools.SetActiveChildren(this.Menus.gameObject, false);
		if (goodsOwner == GoodsOwnerTypes.SelfBag)
		{
			if (!selfBagOnly)
			{
				NGUITools.SetActive(this.FangruIcon.gameObject, true);
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy == 350)
			{
				this.m_BtnJuanXian.gameObject.SetActive(true);
				this.m_BtnJuanXian.Text = Global.GetLang("捐献");
				this.ShiyongIcon.gameObject.SetActive(false);
			}
			if (categoriy == 24 || categoriy == 8 || (25 <= categoriy && 27 >= categoriy) || categoriy == 28)
			{
				bool flag = true;
				List<GoodsData> list = null;
				XElement xelement = null;
				string newroot = string.Empty;
				if (categoriy == 24)
				{
					list = Global.GetRoleFashionList(ItemCategories.Fashion);
					xelement = Global.GetGameResXml("ShiZhuangLevelup.xml");
					newroot = "ShiZhuangLevelup";
				}
				else if (categoriy == 8)
				{
					list = Global.GetRoleFashionList(ItemCategories.ChiBang);
					xelement = Global.GetGameResXml("FashionWings.xml");
					newroot = "SpecialTitle";
				}
				else if (categoriy == 27)
				{
					list = Global.GetRoleFashionList(ItemCategories.ShiZhuang_ZuoQi);
					xelement = Global.GetGameResXml("HorseFashion.xml");
					newroot = "HorseFashion";
				}
				else if (categoriy == 25)
				{
					list = Global.GetRoleFashionList(ItemCategories.ShiZhuang_WuQi);
					xelement = Global.GetGameResXml("WuQiShiZhuangShengJi.xml");
					newroot = "JingLing";
				}
				else if (categoriy == 26)
				{
					list = Global.GetRoleFashionList(ItemCategories.ShiZhuang_JiaoYin);
					xelement = Global.GetGameResXml("JiaoYinShiZhuangShengJi.xml");
					newroot = "JingLing";
				}
				else if (categoriy == 28)
				{
					list = Global.GetRoleFashionList(ItemCategories.BianShen_ShiZhuang);
				}
				if (list != null && 0 < list.Count)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (goodsData.GoodsID == list[i].GoodsID)
						{
							flag = false;
							break;
						}
					}
				}
				if (xelement != null)
				{
					XElement xelement2 = Global.GetXElement(xelement, newroot, "GoodsID", goodsData.GoodsID.ToString());
					if (xelement2 != null)
					{
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Time");
						if ("-1" != xelementAttributeStr)
						{
							flag = true;
						}
					}
				}
				NGUITools.SetActive(this.ShiyongIcon.gameObject, flag);
			}
			else if (Global.CanDirectUseGoods(goodsData.GoodsID, false))
			{
				if (categoriy == 23)
				{
					if (Global.GetDecorationHaveActive(goodsXmlNodeByID.ID))
					{
						NGUITools.SetActive(this.ShiyongIcon.gameObject, false);
					}
					else
					{
						this.ShiyongIcon.Label.text = Global.GetLang("激活");
						NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
					}
				}
				else if (categoriy == 350)
				{
					NGUITools.SetActive(this.ShiyongIcon.gameObject, false);
				}
				else
				{
					this.ShiyongIcon.Label.text = Global.GetLang("使用");
					NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
				}
				int categoriy2 = goodsXmlNodeByID.Categoriy;
			}
			else
			{
				int glUI = goodsXmlNodeByID.GlUI;
				if (glUI > 0)
				{
					if (categoriy == 23)
					{
						if (Global.GetDecorationHaveActive(goodsXmlNodeByID.ID))
						{
							NGUITools.SetActive(this.ShiyongIcon.gameObject, false);
						}
						else
						{
							this.ShiyongIcon.Label.text = Global.GetLang("激活");
							NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
						}
					}
					else
					{
						this.ShiyongIcon.Label.text = Global.GetLang("使用");
						NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
					}
				}
			}
			if (Super.CanSaleOutGoods(goodsData))
			{
				if (categoriy == 23)
				{
					NGUITools.SetActive(this.ChushouIcon.gameObject, Global.GetDecorationHaveActive(goodsXmlNodeByID.ID));
				}
				else
				{
					NGUITools.SetActive(this.ChushouIcon.gameObject, true);
				}
			}
			else
			{
				NGUITools.SetActive(this.ChushouIcon.gameObject, false);
			}
			if (Global.IsRebornBaoShi(categoriy) || Global.IsRebornXuanCai(categoriy))
			{
				this.ChushouIcon.Text = Global.GetLang("回收");
			}
			else
			{
				this.ChushouIcon.Text = Global.GetLang("出售");
			}
			if (Global.g_bIsTipsShowCuiHuiBtn)
			{
				this.m_BtnCuiHui.gameObject.SetActive(true);
			}
		}
		else if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
		{
			this.m_BtnMaxNum.isEnabled = false;
			this.BuyNumIcon.gameObject.SetActive(true);
		}
		else if (goodsOwner == GoodsOwnerTypes.JiYuanShouJi)
		{
			this.m_BtnShouJi.gameObject.SetActive(true);
			this.m_BtnShouJi.Text = Global.GetLang("收集");
			this.m_BtnShouJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (PlayZone.GlobalPlayZone.m_JiYuanHuoDongPart != null)
				{
					PlayZone.GlobalPlayZone.m_JiYuanHuoDongPart.SetChuanSong(this._goodsData.GoodsID);
					this.TipsCallBack(TipsOperationTypes.Close, 0);
				}
			};
		}
		else if (goodsOwner == GoodsOwnerTypes.QiangGou)
		{
			this._GoodsObtainBtn.gameObject.SetActive(false);
			this.BuyNumIcon.gameObject.SetActive(false);
		}
		else if (goodsOwner == GoodsOwnerTypes.NPCSale || goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.WangZheShangCheng || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop || goodsOwner == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
		{
			this.BuyNumIcon.gameObject.SetActive(true);
			this.m_BtnMaxNum.isEnabled = true;
			this.m_BtnMaxNum.gameObject.SetActive(true);
			this._GoodsObtainBtn.gameObject.SetActive(false);
		}
		else if (goodsOwner == GoodsOwnerTypes.Exchange || goodsOwner == GoodsOwnerTypes.SelfPet)
		{
			NGUITools.SetActive(this.QuhuiIcon.gameObject, true);
		}
		else if (goodsOwner != GoodsOwnerTypes.SelfBagNoMenu)
		{
			if (goodsOwner == GoodsOwnerTypes.SelfStall)
			{
				if (null != this.m_BtnShangJia && 0 >= goodsData.Binding && !Global.IsTimeLimitGoods(goodsData))
				{
					GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					int categoriy2 = goodsXmlNodeByID2.Categoriy;
					if (categoriy2 != 50)
					{
						NGUITools.SetActive(this.m_BtnShangJia.gameObject, true);
					}
				}
			}
			else if (goodsOwner == GoodsOwnerTypes.Guide)
			{
				if (Global.CanDirectUseGoods(goodsData.GoodsID, false))
				{
					NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
				}
				else
				{
					GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					int categoriy2 = goodsXmlNodeByID3.Categoriy;
					if (categoriy2 == 301 || categoriy2 == 302)
					{
						NGUITools.SetActive(this.ShiyongIcon.gameObject, true);
					}
				}
			}
			else if (goodsOwner == GoodsOwnerTypes.OtherRole)
			{
				GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				if (goodsXmlNodeByID4.Categoriy == 24)
				{
					this.Menus.Visibility = false;
				}
			}
		}
		this.MenusList.repositionNow = true;
	}

	public void AddNum()
	{
		if (this.BuyNumIcon.BtnTag == TipsOperationTypes.Goumai.ToString())
		{
			int length = this.BuyNum.ToString().Length;
			int num;
			if (length >= 3)
			{
				num = (int)Math.Pow(10.0, (double)(length - 2));
			}
			else
			{
				num = (int)Math.Pow(10.0, (double)(length - 1));
			}
			this.BuyNum = Math.Min(this.BuyNum + num, this.MaxBuyNum);
			this.FootBuyTxtNum.text = this.BuyNum.ToString();
			this.RefreshPrice();
		}
		else if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.Caifen.ToString())
		{
			this.CaifenNum = Math.Min(++this.CaifenNum, this.MaxCaifenNum);
			this.FootBuyTxtNum.text = this.CaifenNum.ToString();
		}
	}

	public void SubNum()
	{
		if (this.BuyNumIcon.BtnTag == TipsOperationTypes.Goumai.ToString())
		{
			int length = this.BuyNum.ToString().Length;
			this.BuyNum = Math.Max(this.BuyNum - (int)Math.Pow(10.0, (double)Math.Max(length - 2, 0)), 1);
			this.FootBuyTxtNum.text = this.BuyNum.ToString();
			this.RefreshPrice();
		}
		else if (this.m_BtnMaxNum.BtnTag == TipsOperationTypes.Caifen.ToString())
		{
			this.CaifenNum = Math.Max(--this.CaifenNum, 1);
			this.FootBuyTxtNum.text = this.CaifenNum.ToString();
		}
	}

	private void TipsCallBack(TipsOperationTypes type, int id = 0)
	{
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			IDType = 0,
			ID = 0
		});
		if (type > TipsOperationTypes.Close && type < TipsOperationTypes.Max && null != GTipServiceEx.TipsSender && GTipServiceEx.TipsSender.DPSelectedItem != null)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id,
				MyID = this.goodsID
			});
		}
		if (type == TipsOperationTypes.ShangJia)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 12
			});
		}
		if (type != TipsOperationTypes.OtherStallGouMai || null != GTipServiceEx.TipsSender)
		{
		}
		GTipServiceEx.ClearChildWindow();
	}

	public static int LastGoodsID;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public UISprite Bak;

	public GButton CloseBtn;

	public CAnimation[] _HelpAnim;

	public SpriteSL Menus;

	public UITable MenusList;

	public GButton ShiyongIcon;

	public GButton ChushouIcon;

	public GButton FangruIcon;

	public GButton QuhuiIcon;

	public GButton m_BtnShangJia;

	public GButton m_BtnCuiHui;

	public GButton BuyNumIcon;

	public GButton m_BtnMaxNum;

	public UIButton AddIcon;

	public UIButton SubIcon;

	public GButton m_BtnJuanXian;

	public GButton m_BtnShouJi;

	public GButton _GoodsObtainBtn;

	public TextBlock txtTitle;

	public TextBlock txtLevel;

	public TextBlock txtOcc;

	public TextBlock txtType;

	public GGoodIcon GoodIcon;

	public TextBlock txtActiveTime;

	public TextBlock txtUseLimited;

	public Transform Body;

	public TextBlock txtInfo;

	public TextBlock txtInfoConTent;

	public Transform Foot;

	public SpriteSL FootIntro;

	public TextBlock FootIntroTxtNum;

	public SpriteSL FootBuy;

	public TextBlock txtSalePrice;

	public UISprite IconSprite;

	public UILabel FootBuyTxtNum;

	public GameObject FootBuyTxtNumContainer;

	private DPSelectedItemEventHandler DPSelectedItemNum;

	private int BuyNum;

	private int BuyOnePrice;

	private int PriceTypeIndex;

	private int MaxBuyNum;

	private int CaifenNum;

	private int MaxCaifenNum;

	private int goodsID;

	private GoodsData _goodsData;

	private GoodsOwnerTypes _goodsOwner = GoodsOwnerTypes.None;

	private string _priceInfo = string.Empty;

	private bool isMallOrNPCSale;

	public TextBlock[] ConstTexts;

	private string[] addIconSpriteNames = new string[]
	{
		"roleJiadianBtn",
		"roleJiadianBtn_disable"
	};

	private string[] subIconSpriteNames = new string[]
	{
		"tipsSubBtn",
		"tipsSubBtn_disable"
	};

	private DateTime DateTime_Endtime = default(DateTime);

	private bool DateTime_Endtime_Enable;
}
