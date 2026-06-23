using System;
using System.Collections;
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
using XMLCreater;

public class GEquipTips : UserControl
{
	private void InitTextInPrefabs()
	{
		this.txtPeidaiMode.Pivot = 5;
		this.PeidaiIcon.Text = Global.GetLang("佩戴");
		this.XiexiaIcon.Text = Global.GetLang("卸下");
		this.JiagongIcon.Text = Global.GetLang("加工");
		this.ChushouIcon.Text = Global.GetLang("出售");
		this.FangruIcon.Text = Global.GetLang("放入");
		this.QuhuiIcon.Text = Global.GetLang("取回");
		this.m_BtnShangJia.Text = Global.GetLang("上架");
		this.m_BtnCuiHui.Text = Global.GetLang("摧毁");
		this.BuyNumIcon.Text = Global.GetLang("购买");
		this.m_BtnMaxNum.Text = Global.GetLang("最大");
		this.BtnPaiZhu.Text = Global.GetLang("派驻");
		this.BtnReset.Text = Global.GetLang("重置");
		this.txtXuanCaiName.Text = Global.GetLang("[炫彩宝石]");
		this.txtChongShengName.Text = Global.GetLang("[重生宝石]");
		this.txtCuiLianName.Text = Global.GetLang("[淬炼属性]");
		this.ChaKanIcon.Text = Global.GetLang("查看");
		this.ShiYongIcon.Text = Global.GetLang("入库");
		this.txtSalePrice.Text = string.Empty;
		if (this.ConstTexts != null && this.ConstTexts.Length == 10)
		{
			this.ConstTexts[0].Text = Global.GetLang("[基础属性]");
			this.ConstTexts[1].Text = Global.GetLang("[追加属性]");
			this.ConstTexts[2].Text = Global.GetLang("[转生属性]");
			this.ConstTexts[3].Text = Global.GetLang("[培养属性]");
			this.ConstTexts[4].Text = Global.GetLang("[卓越属性]");
			this.ConstTexts[5].Text = Global.GetLang("[幸运属性]");
			this.ConstTexts[6].Text = Global.GetLang("战斗力");
			this.ConstTexts[7].Text = Global.GetLang("数量:");
			this.ConstTexts[8].Text = Global.GetLang("[套装属性]");
			this.ConstTexts[9].Text = Global.GetLang("[觉醒属性]");
		}
		try
		{
			if (this.PropsJuHun.gameObject.transform.GetChild(0).name.Equals("txtName"))
			{
				this.PropsJuHun.gameObject.transform.GetChild(0).GetComponent<TextBlock>().Label.text = Global.GetLang("[聚魂]");
			}
			if (this.m_JingLingSkillobj.transform.GetChild(0).name.Equals("txtName"))
			{
				this.m_JingLingSkillobj.transform.GetChild(0).gameObject.GetComponent<UILabel>().text = Global.GetLang("[技能]");
			}
			if (this.PropsFuMo.gameObject.transform.GetChild(0).name.Equals("txtName"))
			{
				this.PropsFuMo.gameObject.transform.GetChild(0).GetComponent<TextBlock>().Label.text = Global.GetLang("[附魔属性]");
			}
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南东南亚英文--聚魂/技能赋值出错"
			});
		}
		this.txtJueXingValue.MaxWidth = 300.0;
		this.txtJuHunValue.MaxWidth = 300.0;
	}

	private bool SaleEquipNeedTiShi(GoodsData gd)
	{
		bool result = false;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID.Categoriy == 9)
		{
			result = true;
		}
		if (goodsXmlNodeByID.Categoriy == 10 && goodsXmlNodeByID.SuitID >= 4)
		{
			result = true;
		}
		if (Global.GetZhuoyueAttributeCount(gd) > 0)
		{
			result = true;
		}
		if (goodsXmlNodeByID.SuitID >= 5 && Global.GetZhuoyueAttributeCount(gd) >= 2)
		{
			result = true;
		}
		if (goodsXmlNodeByID.SuitID < 5 && Global.GetZhuoyueAttributeCount(gd) >= 4)
		{
			result = true;
		}
		if (gd.Forge_level >= 7)
		{
			result = true;
		}
		if (gd.AppendPropLev >= 20)
		{
			result = true;
		}
		if (goodsXmlNodeByID.Categoriy == 340 && 3 <= Super.GetHorseQuality(gd))
		{
			result = true;
		}
		if (40 <= goodsXmlNodeByID.Categoriy && 45 >= goodsXmlNodeByID.Categoriy && 3 <= Super.GetHorseQuality(gd))
		{
			result = true;
		}
		if (Super.MessageBoxIsHint[1] != 0)
		{
			result = false;
		}
		return result;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.GoodIcon.Width = 64.0;
		this.GoodIcon.Height = 64.0;
		this.GoodIcon.isAutoSize = true;
		Vector3 zero = Vector3.zero;
		zero.x = (float)this.GoodIcon.Width + 5f;
		zero.y = (float)this.GoodIcon.Height + 5f;
		this.GoodIcon.BackgroundSprite15.transform.localScale = zero;
		this.QianghuaProgressBar.ItemWidth = 21f;
		this.QianghuaProgressBar.MaxLevel = 15;
		this.QianghuaProgressBar1.ItemWidth = 21f;
		this.QianghuaProgressBar1.MaxLevel = 5;
		this.PropsDraggablePanel = this.PropsPanel.gameObject.GetComponent<UIDraggablePanel>();
		this.PropsDraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.OnPropsPanelDragfinished);
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
		};
		if (null != this.m_BtnCuiHui)
		{
			this.m_BtnCuiHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (Application.platform == 7)
				{
					GameInstance.Game.DestroyGoods(this.m_nDBID);
					this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
				}
			};
		}
		this.PeidaiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Peidai, 0, HandTypes.None);
		};
		this.XiexiaIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.Occupation == 3)
			{
				Global.currentMJSType = Global.GetMJSType();
			}
			this.TipsCallBack(TipsOperationTypes.Xiexia, 0, HandTypes.None);
		};
		this.JiagongIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int nDBID = this.m_nDBID;
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(nDBID, null);
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LianLu, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.LianLu, trigger, param, param2, true);
				return;
			}
			if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
			{
				if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.Horese, trigger, param, param2, true);
					return;
				}
				if (!Global.HorseEquipOpen(goodsXmlNodeByID.ID))
				{
					Super.HintMainText(Global.GetLang("本功能未开启"), 10, 3);
					return;
				}
			}
			this.TipsCallBack(TipsOperationTypes.Jiagong, 0, HandTypes.None);
			if (goodsXmlNodeByID != null)
			{
				if (Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
				{
					if (Global.Data.roleData.RebornCount <= 0)
					{
						Super.HintMainText(Global.GetLang("重生后才可使用该功能"), 10, 3);
						return;
					}
					if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
					{
						Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
						return;
					}
					PlayZone.GlobalPlayZone.OpenChongShengLianLuWindow(ChongShenLianLuTypes.JinJie);
				}
				else if (goodsXmlNodeByID.Categoriy == 980)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1537,
						Flag = goodsDataByDbID.Id
					});
				}
				else
				{
					(Super.GData.PlayZoneRoot as PlayZone).ShowLianluWindow(0, goodsXmlNodeByID.Categoriy);
				}
			}
			else
			{
				(Super.GData.PlayZoneRoot as PlayZone).ShowLianluWindow(0, 0);
			}
			if (PlayZone.GlobalPlayZone.gamePayerRolePart != null)
			{
				PlayZone.GlobalPlayZone.CloseGamePayerRoleWindow();
			}
		};
		this.ChushouIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChushouIcon.BtnTag == string.Empty)
			{
				this.TipsCallBack(TipsOperationTypes.Chushou, 0, HandTypes.None);
			}
			else
			{
				int dbID = this.ChushouIcon.BtnTag.SafeToInt32(0);
				GoodsData gd = Global.GetGoodsDataByDbID(dbID, null);
				GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (gd != null)
				{
					if (Global.IsRebornGood(goodVO))
					{
						this.ChuShouChongShengGood(gd, goodVO, this.ChushouIcon.BtnTag);
						return;
					}
					if ((24 <= goodVO.Categoriy && 27 >= goodVO.Categoriy) || goodVO.ChangeJinYuan == 0)
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
						this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
						return;
					}
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao) && Global.IsShengqi(gd))
					{
						Super.HintMainText(string.Format(Global.GetLang("需要开启【{0}】系统才能回收"), GongnengYugaoMgr.GetGongNengName(GongNengIDs.ZaiZao)), 10, 3);
						return;
					}
					if (this.SaleEquipNeedTiShi(gd))
					{
						if (goodVO.Categoriy == 9 || goodVO.Categoriy == 10)
						{
							string[] awardDescribe = new string[]
							{
								Global.GetLang("精灵本体回收"),
								Global.GetLang("精灵等级回收"),
								Global.GetLang("精灵技能回收")
							};
							int[] jingLingRecoverAward = Global.GetJingLingRecoverAward(gd);
							Super.ShowJingLingHuiShouMessageBox(Global.GetLang("精灵回收"), awardDescribe, jingLingRecoverAward, delegate(object x, DPSelectedItemEventArgs c)
							{
								if (c.ID == 0)
								{
									GameInstance.Game.SpriteOneKeyQuickSaleOut(3, this.ChushouIcon.BtnTag);
								}
								this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
								return true;
							}, null);
						}
						else if (goodVO.Categoriy == 340)
						{
							if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
							{
								GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认回收？"), 2, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
								messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
								{
									int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
									Super.CloseMessageBox(this.Container, messageBoxWindow);
									if (messageBoxReturn == 0)
									{
										if (gd.Forge_level > 0)
										{
											Super.HintMainText(Global.GetLang("只有将坐骑重置至1阶才能回收"), 10, 3);
										}
										else
										{
											GameInstance.Game.SpriteOneKeyQuickSaleOut(5, this.ChushouIcon.BtnTag);
										}
										this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
									}
									return true;
								};
							}
						}
						else if (40 <= goodVO.Categoriy && 45 >= goodVO.Categoriy)
						{
							GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认回收？"), 2, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
							messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
							{
								int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
								Super.CloseMessageBox(this.Container, messageBoxWindow);
								if (messageBoxReturn == 0)
								{
									GameInstance.Game.SpriteOneKeyQuickSaleOut(5, this.ChushouIcon.BtnTag);
									this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
								}
								return true;
							};
						}
						else
						{
							GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此装备比较贵重，是否确认回收？"), 2, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
							messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
							{
								int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
								Super.CloseMessageBox(this.Container, messageBoxWindow);
								if (messageBoxReturn == 0)
								{
									if (Global.IsRebornGood(goodVO))
									{
										GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(this.ChushouIcon.BtnTag);
									}
									else if (goodVO.Categoriy == 9 || goodVO.Categoriy == 10)
									{
										GameInstance.Game.SpriteOneKeyQuickSaleOut(3, this.ChushouIcon.BtnTag);
									}
									else
									{
										GameInstance.Game.SpriteOneKeyQuickSaleOut(2, this.ChushouIcon.BtnTag);
									}
									this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
								}
								return true;
							};
						}
					}
					else
					{
						if (goodVO.Categoriy == 9 || goodVO.Categoriy == 10)
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(3, this.ChushouIcon.BtnTag);
						}
						else if (goodVO.Categoriy == 340)
						{
							if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
							{
								if (gd.Forge_level > 0)
								{
									Super.HintMainText(Global.GetLang("只有将坐骑重置至1阶才能回收"), 10, 3);
								}
								else
								{
									GameInstance.Game.SpriteOneKeyQuickSaleOut(5, this.ChushouIcon.BtnTag);
								}
							}
						}
						else if (40 <= goodVO.Categoriy && 45 >= goodVO.Categoriy)
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(5, this.ChushouIcon.BtnTag);
						}
						else if (Global.IsRebornGood(goodVO))
						{
							GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(this.ChushouIcon.BtnTag);
						}
						else
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(2, this.ChushouIcon.BtnTag);
						}
						this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
					}
				}
			}
		};
		this.FangruIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Fangru, 0, HandTypes.None);
		};
		this.QuhuiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Quhui, 0, HandTypes.None);
		};
		if (null != this.m_BtnShangJia)
		{
			this.m_BtnShangJia.MouseLeftButtonUp = delegate(object sim, MouseEvent e)
			{
				this.TipsCallBack(TipsOperationTypes.ShangJia, 0, HandTypes.None);
			};
		}
		this.DPSelectedItemNum = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			int length = id.ToString().Length;
			if (id > this.MaxBuyNum)
			{
				if (this.PriceTypeIndex == 0 || this.PriceTypeIndex == 4)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				}
				else if (this.PriceTypeIndex == 2)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
				}
			}
			else if (id < 1)
			{
				Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
			}
			else
			{
				this.BuyNum = id;
			}
			this.FootBuyTxtNum.text = this.BuyNum.ToString();
			this.RefreshPrice();
		};
		UIEventListener.Get(this.FootBuyTxtNumContainer).onClick = delegate(GameObject s)
		{
			if (this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
			{
				return;
			}
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DPSelectedItemNum, this.FootBuyTxtNum, 0, -100);
		};
		this.BuyNumIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = ConvertExt.SafeConvertToInt32(this.FootBuyTxtNum.text);
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
			}
			else if (num > 0)
			{
				if (this.BuyNumIcon.BtnTag == TipsOperationTypes.JiaoYiShuoGouMai.ToString())
				{
					this.TipsCallBack(TipsOperationTypes.JiaoYiShuoGouMai, num, HandTypes.None);
				}
				else
				{
					this.TipsCallBack(TipsOperationTypes.Goumai, num, HandTypes.None);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("输入的数量不正确！"), 10, 3);
			}
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
		this.SwitchHandIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._HandTypes == HandTypes.ZuoShou)
			{
				this.TipsCallBack(TipsOperationTypes.SwitchHand, 0, HandTypes.YouShou);
			}
			else if (this._HandTypes == HandTypes.YouShou)
			{
				this.TipsCallBack(TipsOperationTypes.SwitchHand, 0, HandTypes.ZuoShou);
			}
		};
		if (this.callback != null)
		{
			this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
		}
		if (null != this.m_JingLingSkillobj)
		{
			NGUITools.SetActive(this.m_JingLingSkillobj, false);
		}
		if (null != this.m_SkillsLst)
		{
			this.m_Collection = this.m_SkillsLst.ItemsSource;
		}
	}

	private void ChuShouChongShengGood(GoodsData gd, GoodVO goodVO, string ids)
	{
		if (Global.IsRebornGood(goodVO))
		{
			if (Global.IsRebornEquip(goodVO.Categoriy))
			{
				bool flag = Global.GetZhuoyueAttributeCount(gd) > 5;
				long num = (long)Global.GetGoodsSaleToNpcChongShengExp(gd);
				long num2 = Global.Data.roleData.MoneyData[149];
				long num3 = 0L;
				if (num > num2)
				{
					num3 = num - num2;
				}
				string text = string.Empty;
				if (Global.Data.roleData.RebornCount <= 0)
				{
					text = Global.GetLang("您尚未重生，回收不会获得重生经验，是否确认要进行回收？");
				}
				else if (flag && num3 > 0L)
				{
					text = StringUtil.substitute(Global.GetLang("本次回收有贵重物品，且已达到今日回收经验上限，将有{0}经验溢出，是否确认回收？"), new object[]
					{
						num3
					});
				}
				else if (flag)
				{
					text = Global.GetLang("回收物品中有贵重物品，是否确认要进行回收？");
				}
				else if (num3 > 0L)
				{
					text = StringUtil.substitute(Global.GetLang("本次回收已达到今日回收经验上限，将有{0}经验溢出，是否确认回收？"), new object[]
					{
						num3
					});
				}
				if (string.IsNullOrEmpty(text))
				{
					GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(ids);
					this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
					return;
				}
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), text, 1, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(ids);
						this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
					}
					return true;
				};
			}
			else if (Global.IsRebornBaoShi(goodVO.Categoriy) || Global.IsRebornXuanCai(goodVO.Categoriy))
			{
				if (Global.IsNeedSaleRebornTiShi(gd))
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此物品比较贵重，是否确认回收？"), 1, null, MessBoxIsHintTypes.EquipHuishouNeedHint);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendChongShengPiLiangFenJie(ids);
							this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
						}
						return true;
					};
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
						this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
					}
					return true;
				};
			}
			else
			{
				GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(ids);
				this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
			}
		}
	}

	public void RenderTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData, bool selfBagOnly, string priceInfo = null, int handTypes = -1, byte showBtns = 1)
	{
		this.m_nDBID = goodsData.Id;
		GGoodTips.LastGoodsID = goodsData.GoodsID;
		if (NGUITools.GetActive(this.CloseBtn) != (1 == showBtns))
		{
			this.CloseBtn.gameObject.SetActive(1 == showBtns);
		}
		try
		{
			this.SetText(goodsOwner, goodsData, priceInfo, handTypes, showBtns);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		try
		{
			this.SetMenus(goodsOwner, goodsData, selfBagOnly, showBtns);
		}
		catch (Exception ex2)
		{
			MUDebug.LogException(ex2);
		}
		try
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
			{
				if (goodsData.ElementhrtsProps != null)
				{
					bool flag = false;
					for (int i = 0; i < goodsData.ElementhrtsProps.Count; i++)
					{
						if (i % 3 == 2 && goodsData.ElementhrtsProps[i] != 0)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						NGUITools.SetActive(this.m_JingLingSkillobj, true);
						this.m_Collection.Clear();
						List<jinglingPart.SkillData> list = new List<jinglingPart.SkillData>();
						int j = 0;
						int num = 1;
						int num2 = 2;
						int num3 = 0;
						while (j < goodsData.ElementhrtsProps.Count)
						{
							list.Add(new jinglingPart.SkillData(1 == goodsData.ElementhrtsProps[j], goodsData.ElementhrtsProps[num], goodsData.ElementhrtsProps[num2], false));
							j = num2 + 1;
							num = j + 1;
							num2 = num + 1;
							num3++;
						}
						for (int k = 0; k < list.Count; k++)
						{
							if (list[k].IsOpen && list[k].Id != 0)
							{
								JingLingSkillsIcon jingLingSkillsIcon = U3DUtils.NEW<JingLingSkillsIcon>();
								jingLingSkillsIcon.InitData(list[k].Id, list[k].Lev);
								this.m_Collection.AddNoUpdate(jingLingSkillsIcon);
								UIPanel component = jingLingSkillsIcon.GetComponent<UIPanel>();
								if (null != component)
								{
									Object.Destroy(component);
								}
							}
						}
						this.m_SkillsLst.repositionNow = true;
					}
					else
					{
						if (this.m_Collection != null)
						{
							this.m_Collection.Clear();
						}
						if (null != this.m_JingLingSkillobj)
						{
							NGUITools.SetActive(this.m_JingLingSkillobj, false);
						}
					}
				}
				else
				{
					if (this.m_Collection != null)
					{
						this.m_Collection.Clear();
					}
					if (null != this.m_JingLingSkillobj)
					{
						NGUITools.SetActive(this.m_JingLingSkillobj, false);
					}
				}
			}
			else if (goodsXmlNodeByID.Categoriy == 340)
			{
				HorseSkillData horseSkillData = new HorseSkillData(goodsData);
				if (0 < horseSkillData.SkillID)
				{
					NGUITools.SetActive(this.m_JingLingSkillobj, true);
					this.m_Collection.Clear();
					JingLingSkillsIcon jingLingSkillsIcon2 = U3DUtils.NEW<JingLingSkillsIcon>();
					jingLingSkillsIcon2.InitHorseSkillData(horseSkillData.SkillID, horseSkillData.SkillLevel);
					this.m_Collection.AddNoUpdate(jingLingSkillsIcon2);
					UIPanel component2 = jingLingSkillsIcon2.GetComponent<UIPanel>();
					if (null != component2)
					{
						Object.Destroy(component2);
					}
					this.m_SkillsLst.repositionNow = true;
				}
				else
				{
					if (this.m_Collection != null)
					{
						this.m_Collection.Clear();
					}
					if (null != this.m_JingLingSkillobj)
					{
						NGUITools.SetActive(this.m_JingLingSkillobj, false);
					}
				}
			}
			else
			{
				NGUITools.SetActive(this.m_JingLingSkillobj, false);
				this.m_Collection.Clear();
			}
		}
		catch (Exception ex3)
		{
			MUDebug.LogException(ex3);
		}
	}

	private void SetText(GoodsOwnerTypes goodsOwner, GoodsData goodsData, string priceInfo = null, int handTypes = -1, byte showBtns = 0)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = "FFFFFF";
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (goodsXmlNodeByID.Categoriy == 340)
		{
			int horseQuality = Super.GetHorseQuality(goodsData);
			if (horseQuality == 4)
			{
				text = "FF08FF";
				text2 += UIHelper.ZuoyueTitleNames[2];
			}
			else if (horseQuality == 3)
			{
				text = "FF08FF";
				text2 += UIHelper.ZuoyueTitleNames[2];
			}
			else if (horseQuality == 2)
			{
				text = "0099FF";
				text2 += UIHelper.ZuoyueTitleNames[1];
			}
			else if (horseQuality == 1)
			{
				text = "00FF00";
				text2 += UIHelper.ZuoyueTitleNames[0];
			}
		}
		else if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text = "00FF00";
				text2 += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text = "0099FF";
				text2 += UIHelper.ZuoyueTitleNames[1];
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text = "ff08ff";
				text2 += UIHelper.ZuoyueTitleNames[2];
			}
		}
		else if (goodsXmlNodeByID.Categoriy == 980)
		{
			int suitID = goodsXmlNodeByID.SuitID;
			if (suitID <= 3)
			{
				text = "00FF00";
				text2 += UIHelper.ZuoyueTitleNames[0];
			}
			else if (suitID == 4)
			{
				text = "0099FF";
				text2 += UIHelper.ZuoyueTitleNames[1];
			}
			else if (suitID > 4 && suitID <= 6)
			{
				text = "ff08ff";
				text2 += UIHelper.ZuoyueTitleNames[2];
			}
		}
		else
		{
			int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount2 > 0 && zhuoyueAttributeCount2 <= 2)
			{
				text = "00FF00";
				text2 += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount2 >= 3 && zhuoyueAttributeCount2 <= 4)
			{
				text = "0099FF";
				text2 += UIHelper.ZuoyueTitleNames[1];
			}
			else if (zhuoyueAttributeCount2 >= 5 && zhuoyueAttributeCount2 <= 6)
			{
				text = "FF08FF";
				text2 += UIHelper.ZuoyueTitleNames[2];
			}
		}
		text2 += goodsXmlNodeByID.Title;
		if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
		{
			text = goodsXmlNodeByID.GoodsColor;
			text2 = goodsXmlNodeByID.Title;
		}
		text2 = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			text2
		});
		if (goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 10 && goodsXmlNodeByID.Categoriy != 23 && goodsXmlNodeByID.Categoriy != 24 && goodsXmlNodeByID.Categoriy != 340 && !Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
		{
			if (goodsData.Forge_level > 0)
			{
				text3 += string.Format("+{0}", goodsData.Forge_level);
			}
			if (goodsData.AppendPropLev > 0)
			{
				text3 += string.Format(Global.GetLang("追{0}"), goodsData.AppendPropLev);
			}
		}
		if (goodsXmlNodeByID.Categoriy == 7)
		{
			this.txtTitle.Text = string.Format("{0}", text2);
		}
		else
		{
			this.txtTitle.Text = string.Format("{0} {1}", text2, text3);
		}
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 24 && goodsOwner == GoodsOwnerTypes.LookOther)
		{
			this.txtZhandouli.Text = string.Empty;
			this.ConstTexts[6].Text = string.Empty;
		}
		else
		{
			this.ConstTexts[6].Text = Global.GetLang("战斗力");
			this.txtZhandouli.Text = Global.GetGoodsDataZhanLi(goodsData).ToString();
		}
		if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10 || goodsXmlNodeByID.Categoriy == 340)
		{
			this.txtSuitID.Visibility = false;
		}
		else if (goodsXmlNodeByID.Categoriy == 980)
		{
			this.txtSuitID.Visibility = true;
			this.txtSuitID.textColor = Global.ParseStringColorToUint("FFFFFF");
			this.txtSuitID.Text = string.Format(Global.GetLang("Lv：{0}"), goodsData.ElementhrtsProps[0]);
		}
		else if (goodsXmlNodeByID.SuitID > 0)
		{
			this.txtSuitID.Visibility = true;
			this.txtSuitID.textColor = Global.ParseStringColorToUint(text);
			this.txtSuitID.Text = string.Format(Global.GetLang("{0}阶装备"), goodsXmlNodeByID.SuitID);
		}
		else
		{
			this.txtSuitID.Visibility = false;
		}
		if (!Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
		{
			int toZhuanSheng = goodsXmlNodeByID.ToZhuanSheng;
			int toLevel = goodsXmlNodeByID.ToLevel;
			text = "FDF7DD";
			if (Global.Data.roleData.ChangeLifeCount == toZhuanSheng)
			{
				if (Global.Data.roleData.Level < toLevel)
				{
					text = "FF0000";
				}
			}
			else if (Global.Data.roleData.ChangeLifeCount < toZhuanSheng)
			{
				text = "FF0000";
			}
			if (toZhuanSheng > 0)
			{
				this.txtLevel.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"C39550",
					Global.GetLang("等级: "),
					text,
					string.Format(Global.GetLang("{0}[{1}转]"), toLevel, toZhuanSheng)
				});
			}
			else
			{
				this.txtLevel.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"C39550",
					Global.GetLang("等级: "),
					text,
					Global.GMax(toLevel, 1)
				});
			}
		}
		else
		{
			this.txtLevel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("佩戴需求: "),
				"C39550",
				string.Format(Global.GetLang("重生{0}阶{1}级"), goodsXmlNodeByID.ToReborn, goodsXmlNodeByID.ToRebornLevel)
			});
		}
		text = "FDF7DD";
		if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
		{
			this.txtOcc.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("职业: "),
				text,
				Global.GetLang("通用")
			});
		}
		else if (goodsXmlNodeByID.ToOccupation >= 0)
		{
			if ((1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) == 0)
			{
				text = "FF0000";
			}
			string occupationStrByGoods = Global.GetOccupationStrByGoods(goodsXmlNodeByID.ToOccupation);
			this.txtOcc.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("职业: "),
				text,
				Global.GetLang(occupationStrByGoods)
			});
		}
		else
		{
			this.txtOcc.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("职业: "),
				text,
				Global.GetLang("通用")
			});
		}
		int categoriy = goodsXmlNodeByID.Categoriy;
		this.txtType.Text = Global.GetGoodsType(categoriy);
		if (goodsOwner == GoodsOwnerTypes.SelfBag)
		{
			if (goodsData.Using == 1)
			{
				this.SetSwitchHandIcon(-1);
			}
			else if (goodsData.Using == 0)
			{
				this.SetSwitchHandIcon(handTypes);
			}
		}
		else
		{
			this.SetSwitchHandIcon(-1);
		}
		this.txtPeidaiMode.Text = string.Empty;
		if (categoriy >= 11 && categoriy < 25)
		{
			int goodsActionNameByID = (int)Global.GetGoodsActionNameByID(goodsData.GoodsID);
			if (goodsActionNameByID == -1)
			{
				this.txtPeidaiMode.Text = string.Empty;
			}
			else if (goodsActionNameByID == 1 || goodsActionNameByID == 4)
			{
				this.txtPeidaiMode.Text = Global.GetLang("单手");
			}
			else
			{
				this.txtPeidaiMode.Text = Global.GetLang("双手");
			}
		}
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
		this.GoodIcon.BackSpriteName0 = "bagGrid2_bak";
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 980)
		{
			this.GoodIcon.isAutoSize = false;
		}
		this.GoodIcon.BodyURL = new ImageURL(value, false, 0);
		this.GoodIcon.ItemCategory = categoriy;
		this.GoodIcon.ItemCode = goodsData.GoodsID;
		this.GoodIcon.ItemObject = goodsData;
		bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
		Super.InitGoodsGIcon(this.GoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
		if (Global.IsShengqi(goodsData))
		{
			Vector3 localPosition = this.GoodIcon.GoodImg.transform.localPosition;
			localPosition.z = -0.1f;
			this.GoodIcon.GoodImg.transform.localPosition = localPosition;
		}
		int num = 145;
		int num2 = 0;
		text2 = string.Empty;
		if (this.txtOcc.Text.Length != 0)
		{
			num -= (int)this.txtOcc.ActualHeight + num2 + 10;
		}
		if (goodsXmlNodeByID.Strength > 0)
		{
			text = "FDF7DD";
			int num3 = (int)Global.GetCurrentRoleProp(1, 0);
			if (num3 < goodsXmlNodeByID.Strength)
			{
				text = "FF0000";
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需力量: {0}/{1}\n"), num3, goodsXmlNodeByID.Strength)
				});
			}
			else
			{
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需力量: {0}\n"), goodsXmlNodeByID.Strength)
				});
			}
		}
		if (goodsXmlNodeByID.Dexterity > 0)
		{
			text = "FDF7DD";
			int num4 = (int)Global.GetCurrentRoleProp(1, 2);
			if (num4 < goodsXmlNodeByID.Dexterity)
			{
				text = "FF0000";
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需敏捷: {0}/{1}\n"), num4, goodsXmlNodeByID.Dexterity)
				});
			}
			else
			{
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需敏捷: {0}\n"), goodsXmlNodeByID.Dexterity)
				});
			}
		}
		if (goodsXmlNodeByID.Intelligence > 0)
		{
			text = "FDF7DD";
			int num5 = (int)Global.GetCurrentRoleProp(1, 1);
			if (num5 < goodsXmlNodeByID.Intelligence)
			{
				text = "FF0000";
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需智力: {0}/{1}\n"), num5, goodsXmlNodeByID.Intelligence)
				});
			}
			else
			{
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需智力: {0}\n"), goodsXmlNodeByID.Intelligence)
				});
			}
		}
		if (goodsXmlNodeByID.Constitution > 0)
		{
			text = "FDF7DD";
			int num6 = (int)Global.GetCurrentRoleProp(1, 3);
			if (num6 < goodsXmlNodeByID.Constitution)
			{
				text = "FF0000";
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需体力: {0}/{1}\n"), num6, goodsXmlNodeByID.Constitution)
				});
			}
			else
			{
				text2 += Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("所需体力: {0}\n"), goodsXmlNodeByID.Constitution)
				});
			}
		}
		if (text2.Length > 0)
		{
			text2 = this.ProcessStr(text2);
			this.txtTiaojian.Visibility = true;
			this.txtTiaojian.Text = text2;
			num -= (int)this.txtTiaojian.ActualHeight + num2;
		}
		else
		{
			this.txtTiaojian.Visibility = false;
		}
		if (categoriy == 24)
		{
			FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(ItemCategories.Fashion, goodsData.GoodsID, goodsData.Forge_level);
			if (fashionLevelupVO != null)
			{
				this.txtInfo.Text = fashionLevelupVO.Description;
			}
		}
		else
		{
			this.txtInfo.Text = goodsXmlNodeByID.Description;
		}
		if (string.IsNullOrEmpty(this.txtInfo.Text))
		{
			this.txtInfo.Visibility = false;
		}
		else
		{
			this.txtInfo.Visibility = true;
			Transform transform = this.txtInfo.transform;
			transform.localPosition = new Vector3(transform.localPosition.x, (float)num, transform.localPosition.z);
			num -= (int)this.txtInfo.ActualHeight + num2;
		}
		this.SetPropsPanel(goodsOwner, num);
		this.SetRebornBaoShiInfo(goodsOwner, goodsData);
		this.SetCuiLianValue(goodsData, goodsXmlNodeByID, goodsOwner);
		if (goodsData.JuHunID <= 0)
		{
			this.PropsJuHun.Visibility = false;
		}
		else
		{
			this.PropsJuHun.Visibility = true;
			this.txtJuHunValue.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format("{0}{1}%", Global.GetLang("强化、追加、培养属性提高"), ParseJuHunConfig.GetJuHunDataById(goodsData.JuHunID).GrowProportion * 100f)
			});
		}
		if ((goodsXmlNodeByID.Categoriy >= 0 && goodsXmlNodeByID.Categoriy <= 21 && goodsXmlNodeByID.Categoriy != 7 && goodsXmlNodeByID.Categoriy != 8 && goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 10) || (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy < 45))
		{
			if (goodsData.ElementhrtsProps == null || goodsData.ElementhrtsProps.Count <= 0)
			{
				this.PropsFuMo.Visibility = false;
			}
			else if (goodsData.ElementhrtsProps[1] <= 0)
			{
				this.PropsFuMo.Visibility = false;
			}
			else
			{
				this.PropsFuMo.Visibility = true;
				List<int> elementhrtsProps = goodsData.ElementhrtsProps;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < elementhrtsProps.Count; i += 2)
				{
					if (!ConfigExtPropIndexes.GetPercentByID(elementhrtsProps[i]))
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(elementhrtsProps[i], false) + ":"
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							elementhrtsProps[i + 1] / 1000
						}) + Environment.NewLine);
					}
					else
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(elementhrtsProps[i], false) + ":"
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							elementhrtsProps[i + 1] / 10 + "%"
						}) + Environment.NewLine);
					}
				}
				this.txtFuMoValue.Text = stringBuilder.ToString();
			}
		}
		else
		{
			this.PropsFuMo.Visibility = false;
		}
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		if (categoriy == 7)
		{
			if (goodsData.Forge_level <= 0)
			{
				goodsData.Forge_level = 1;
				goodsData.AppendPropLev = 0;
			}
			double num7 = Global.SafeConvertToDouble(ConfigSystemParam.GetSystemParamByName("GoodWillXiShu", true));
			for (int j = 0; j < goodsEquipPropsDoubleList.Length; j++)
			{
				goodsEquipPropsDoubleList[j] *= (double)(1 + (goodsData.Forge_level - 1) * 2) + (double)goodsData.AppendPropLev * num7;
			}
			this.PropsZhuijia.Visibility = false;
		}
		for (int k = 0; k < this.mUpSpList.Count; k++)
		{
			this.mUpSpList[k].SetActive(false);
		}
		if (categoriy == 9 || categoriy == 10)
		{
			List<string> petAttribute = Global.GetPetAttribute(goodsData, 0);
			this.txtBaseValue.Text = string.Empty;
			for (int l = 0; l < petAttribute.Count; l++)
			{
				TextBlock textBlock = this.txtBaseValue;
				textBlock.Text = textBlock.Text + petAttribute[l] + '\n';
			}
			this.QianghuaProgressBar.Visibility = false;
			this.QianghuaProgressBar1.Visibility = false;
		}
		else
		{
			this.txtBaseValue.Text = this.GetBaseAttributeStr(goodsData, goodsEquipPropsDoubleList, categoriy, goodsOwner);
			this.PropsBase.Visibility = !string.IsNullOrEmpty(this.txtBaseValue.Text);
		}
		Transform transBaseValue = this.txtBaseValue.gameObject.transform;
		if (categoriy == 22 || categoriy == 9 || categoriy == 10 || categoriy == 7 || categoriy == 23 || categoriy == 24)
		{
			this.PropsZhuijia.Visibility = false;
			transBaseValue.localPosition = new Vector3(transBaseValue.localPosition.x, -28f, transBaseValue.localPosition.z);
		}
		else
		{
			Vector3 localPosition2 = this.txtBaseValue.transform.localPosition;
			if (this.QianghuaProgressBar.Visibility)
			{
				localPosition2.y = -50f;
			}
			else
			{
				localPosition2.y = -20f;
			}
			this.txtBaseValue.transform.localPosition = localPosition2;
			this.txtZhuijiaValue.Text = this.GetZhuijiaAttributeStr(goodsData, goodsEquipPropsDoubleList);
			if (string.IsNullOrEmpty(this.txtZhuijiaValue.Text))
			{
				this.PropsZhuijia.Visibility = false;
				this.txtZhuijiaValue.Visibility = false;
			}
			else
			{
				this.txtZhuijiaValue.Visibility = true;
				this.PropsZhuijia.Visibility = true;
			}
		}
		this.txtXilianValue.Text = this.GetXilianAttributeStr(goodsData, null);
		if (string.IsNullOrEmpty(this.txtXilianValue.Text))
		{
			this.PropsXianlian.Visibility = false;
		}
		else
		{
			this.txtXilianValue.textColor = Global.GetColorByGoodsData(goodsData);
			this.PropsXianlian.Visibility = true;
		}
		this.txtZhuanshengValue.Text = this.GetZhuanshengAttributeStr(goodsData, goodsEquipPropsDoubleList);
		if (string.IsNullOrEmpty(this.txtZhuanshengValue.Text))
		{
			this.PropsZhuansheng.Visibility = false;
		}
		else
		{
			this.PropsZhuansheng.Visibility = true;
		}
		if (categoriy == 340)
		{
			string text4 = string.Empty;
			if (GTipServiceEx.IsBaoXiangOpen)
			{
				text4 = Global.GetLang("获得时，随机生成卓越属性");
			}
			else if (goodsData.WashProps != null && 0 < goodsData.WashProps.Count)
			{
				Dictionary<string, int> dictionary = Global.MaxZhuoYurZuoQi(goodsData.GoodsID);
				for (int m = 0; m < goodsData.WashProps.Count; m += 2)
				{
					if (m < goodsData.WashProps.Count - 1 && 0 < goodsData.WashProps[m] && 0 < goodsData.WashProps[1])
					{
						if (ConfigExtPropIndexes.GetPercentByID(goodsData.WashProps[m]))
						{
							string text5 = text4;
							text4 = string.Concat(new string[]
							{
								text5,
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(goodsData.WashProps[m], false),
								Global.GetLang("："),
								((float)goodsData.WashProps[m + 1] / 1000f * 100f).ToString("f1"),
								"%\n"
							});
						}
						else
						{
							string text5 = text4;
							text4 = string.Concat(new object[]
							{
								text5,
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(goodsData.WashProps[m], false),
								Global.GetLang("："),
								(float)goodsData.WashProps[m + 1] / 1000f,
								"\n"
							});
						}
						if (ConfigExtPropIndexes.GetExtPropIndexesVoByID(goodsData.WashProps[m]) != null)
						{
							string word = ConfigExtPropIndexes.GetExtPropIndexesVoByID(goodsData.WashProps[m]).Word;
							if (dictionary.ContainsKey(word) && dictionary[word] <= goodsData.WashProps[m + 1])
							{
								GameObject gameObject;
								if (m / 2 <= this.mUpSpList.Count - 1)
								{
									gameObject = this.mUpSpList[m / 2];
								}
								else
								{
									gameObject = Object.Instantiate<GameObject>(this.mUpSp.gameObject);
									gameObject.transform.SetParent(this.txtZhuoyueValue.transform.parent.transform, false);
									this.mUpSpList.Add(gameObject);
								}
								gameObject.transform.localPosition = new Vector3(220f, this.txtZhuoyueValue.transform.localPosition.y - 5f - (float)(21 * (m / 2)), -1f);
								gameObject.gameObject.SetActive(true);
							}
						}
					}
				}
			}
			this.txtZhuoyueValue.Text = string.Empty;
			this.txtZhuoyueName.Text = string.Empty;
			if (!string.IsNullOrEmpty(text4))
			{
				this.txtZhuoyueValue.Text = this.ProcessStr(text4);
				this.txtZhuoyueName.Text = Global.GetLang("[卓越属性]");
				this.m_SpriteZhuoYueBak.gameObject.SetActive(true);
			}
		}
		else if (Global.IsRebornEquip(categoriy))
		{
			string text6 = string.Empty;
			if (GTipServiceEx.IsBaoXiangOpen)
			{
				text6 = Global.GetLang("获得时，随机生成卓越属性");
			}
			else if (goodsData.WashProps != null && 0 < goodsData.WashProps.Count)
			{
				for (int n = 0; n < goodsData.WashProps.Count; n += 2)
				{
					if (n < goodsData.WashProps.Count - 1 && 0 < goodsData.WashProps[n] && 0 < goodsData.WashProps[1])
					{
						if (ConfigExtPropIndexes.GetPercentByID(goodsData.WashProps[n]))
						{
							string text5 = text6;
							text6 = string.Concat(new string[]
							{
								text5,
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(goodsData.WashProps[n], false),
								Global.GetLang("："),
								((float)goodsData.WashProps[n + 1] / 1000f * 100f).ToString("f1"),
								"%\n"
							});
						}
						else
						{
							string text5 = text6;
							text6 = string.Concat(new object[]
							{
								text5,
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(goodsData.WashProps[n], false),
								Global.GetLang("："),
								(float)goodsData.WashProps[n + 1] / 1000f,
								"\n"
							});
						}
					}
				}
			}
			this.txtZhuoyueValue.Text = string.Empty;
			this.txtZhuoyueName.Text = string.Empty;
			if (!string.IsNullOrEmpty(text6))
			{
				this.txtZhuoyueValue.Text = this.ProcessStr(text6);
				this.txtZhuoyueName.Text = Global.GetLang("[卓越属性]");
				this.m_SpriteZhuoYueBak.gameObject.SetActive(true);
			}
		}
		else
		{
			this.txtZhuoyueValue.Text = this.GetZhuoyueAttributeStr(goodsData);
			if (string.Empty != this.txtZhuoyueValue.Text)
			{
				if (null != this.m_SpriteZhuoYueBak)
				{
					if (categoriy == 9 || categoriy == 10)
					{
						this.txtZhuoyueName.Text = Global.GetLang("[天赋属性]");
					}
					else if (categoriy == 980)
					{
						this.txtZhuoyueName.Text = Global.GetLang("[卓越属性]");
						int suitID2 = goodsXmlNodeByID.SuitID;
						if (suitID2 <= 3)
						{
							this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[0];
						}
						else if (suitID2 == 4)
						{
							this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[1];
						}
						else if (suitID2 > 4 && suitID2 <= 6)
						{
							this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[2];
						}
					}
					else
					{
						this.txtZhuoyueName.Text = Global.GetLang("[卓越属性]");
						this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[Global.GetZhuoYueAddIndex(goodsData.ExcellenceInfo)];
					}
					if (GTipServiceEx.IsBaoXiangOpen)
					{
						this.txtZhuoyueValue.Text = Global.GetLang("获得时，随机生成卓越属性");
					}
					this.m_SpriteZhuoYueBak.gameObject.SetActive(true);
				}
			}
			else if (categoriy == 980)
			{
				this.txtZhuoyueName.Text = Global.GetLang("[卓越属性]");
				int suitID3 = goodsXmlNodeByID.SuitID;
				if (suitID3 <= 3)
				{
					this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[0];
				}
				else if (suitID3 == 4)
				{
					this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[1];
				}
				else if (suitID3 > 4 && suitID3 <= 6)
				{
					this.m_SpriteZhuoYueBak.spriteName = this.zuoyueSpriteNames[2];
				}
				else if (null != this.m_SpriteZhuoYueBak)
				{
					this.m_SpriteZhuoYueBak.gameObject.SetActive(false);
				}
			}
			else if (null != this.m_SpriteZhuoYueBak)
			{
				this.m_SpriteZhuoYueBak.gameObject.SetActive(false);
			}
		}
		if (string.IsNullOrEmpty(this.txtZhuoyueValue.Text))
		{
			this.PropsZhuoyue.Visibility = false;
		}
		else
		{
			this.PropsZhuoyue.Visibility = true;
		}
		this.txtXingxunValue.Text = this.GetXingyunAttributeStr(goodsData);
		if (string.IsNullOrEmpty(this.txtXingxunValue.Text))
		{
			this.PropsXingxun.Visibility = false;
		}
		else if (categoriy == 24)
		{
			this.PropsXingxun.Visibility = false;
		}
		else
		{
			this.PropsXingxun.Visibility = true;
		}
		this.txtTaozhuangValue.Text = this.GetTaoZhuangPropsStr(goodsData, goodsOwner, -1);
		if (string.IsNullOrEmpty(this.txtTaozhuangValue.Text))
		{
			this.PropsTaozhuang.Visibility = false;
		}
		else
		{
			this.PropsTaozhuang.Visibility = true;
		}
		this.txtJueXingValue.Text = this.GetJueXingStr(goodsData, goodsOwner, -1);
		if (string.IsNullOrEmpty(this.txtJueXingValue.Text))
		{
			this.PropsJueXing.Visibility = false;
		}
		else
		{
			this.PropsJueXing.Visibility = true;
		}
		if (NGUITools.GetActive(this._YiZhuangBei))
		{
			this._YiZhuangBei.SetActive(false);
		}
		this.PropsList.repositionNow = true;
		this.PropsList.onReposition = delegate()
		{
			if (showBtns == 0)
			{
				this.StartCoroutine<bool>(UIHelper.DoActionOnEndFrame(delegate
				{
					this._YiZhuangBei.SetActive(true);
					this._YiZhuangBei.transform.localPosition = new Vector3(244f, transBaseValue.localPosition.y, 0f);
				}));
			}
		};
		if (categoriy == 980)
		{
			this.FootIntroTxtNum.Text = string.Empty;
		}
		else
		{
			string text7 = string.Empty;
			if (goodsData.Strong >= (int)goodsEquipPropsDoubleList[0])
			{
				text7 = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("(已破损，请修理)")
				});
			}
			else
			{
				text7 = string.Format("{0}/{1}", (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue) - goodsData.Strong / Global.MaxNotifyEquipStrongValue, (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue));
			}
			this.FootIntroTxtNum.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"C39550",
				Global.GetLang("耐久: "),
				"ffffff",
				text7
			});
			this.FootIntroTxtNum.transform.localPosition = new Vector3((float)(110 - (int)this.FootIntroTxtNum.ActualWidth), -8f, 0f);
		}
		if (goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.NPCSale || goodsOwner == GoodsOwnerTypes.OtherOnSale || goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
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
				string text8 = GTipServiceEx.PriceInfoUnits[this.PriceTypeIndex];
				this.BuyOnePrice = Convert.ToInt32(array[1]);
				this.MaxBuyNum = this.GetMaxBuyNum(this.PriceTypeIndex, this.BuyOnePrice);
				this.BuyNum = 1;
				if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
				{
					this.FootBuyTxtNum.text = "1";
				}
				else
				{
					this.FootBuyTxtNum.text = this.BuyNum.ToString();
				}
				this.RefreshPrice();
			}
		}
		else if (goodsOwner == GoodsOwnerTypes.jingLingYaoSai || goodsOwner == GoodsOwnerTypes.SelfBag || goodsOwner == GoodsOwnerTypes.ChatGoods || goodsOwner == GoodsOwnerTypes.OtherRole || goodsOwner == GoodsOwnerTypes.SysGifts || goodsOwner == GoodsOwnerTypes.Lianlu || goodsOwner == GoodsOwnerTypes.ChongShengLianlu)
		{
			int num8 = 0;
			bool flag = false;
			bool flag2 = false;
			byte b = 0;
			if (goodsData != null)
			{
				if (categoriy == 9 || categoriy == 10)
				{
					num8 = Global.GetPetPrice(goodsData);
				}
				else if (categoriy == 340 || (categoriy >= 40 && categoriy <= 45))
				{
					if (goodsXmlNodeByID.SuitID <= 6)
					{
						num8 = Global.GetHorsePrice(goodsData, out b);
					}
					else
					{
						num8 = Global.GetZaiZaoDian(goodsData);
						flag2 = true;
					}
				}
				else if (Global.IsRebornEquip(categoriy))
				{
					num8 = Global.GetGoodsSaleToNpcChongShengExp(goodsData);
				}
				else
				{
					int zaiZaoDian = Global.GetZaiZaoDian(goodsData);
					if (0 < zaiZaoDian)
					{
						num8 = zaiZaoDian;
						flag2 = true;
					}
					else
					{
						num8 = Global.GetGoodsSaleToNpJiFen(goodsData);
					}
				}
				if (num8 > 0)
				{
					flag = !flag2;
				}
				else
				{
					num8 = Global.GetGoodsSaleToNpcPrice(goodsData);
				}
			}
			if (num8 > 0)
			{
				this.txtSalePrice.gameObject.SetActive(true);
				this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"C39550",
					Global.GetLang("售价: "),
					"ffffff",
					num8
				});
				this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 15f, -7f, 0f);
				if (Global.IsRebornEquip(categoriy))
				{
					this.IconSprite.spriteName = "rebornExp";
				}
				else if (flag)
				{
					if (categoriy == 9 || categoriy == 10)
					{
						this.IconSprite.spriteName = "mohe";
					}
					else if (categoriy == 340 || (categoriy >= 40 && categoriy <= 45))
					{
						if (b == 0)
						{
							this.IconSprite.spriteName = "moneyMojing";
						}
						else
						{
							this.IconSprite.spriteName = "ZuoQiHunJing";
						}
					}
					else
					{
						this.IconSprite.spriteName = "moneyMojing";
					}
				}
				else if (flag2)
				{
					this.IconSprite.spriteName = "zaizao";
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
			else
			{
				this.txtSalePrice.gameObject.SetActive(false);
				this.IconSprite.spriteName = "none";
			}
		}
		else if (goodsOwner == GoodsOwnerTypes.SysGifts || goodsOwner == GoodsOwnerTypes.SelfStall || goodsOwner == GoodsOwnerTypes.None)
		{
			this.txtSalePrice.Text = string.Empty;
			this.IconSprite.spriteName = "none";
		}
		if (categoriy == 7)
		{
			this.FootIntroTxtNum.Text = string.Empty;
		}
		else if (categoriy == 24)
		{
			this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("不可出售")
			});
			this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -8f, 0f);
		}
	}

	private void SetPropsPanel(GoodsOwnerTypes goodsOwner, int posY)
	{
		int num = 0;
		int num2 = 0;
		if (goodsOwner == GoodsOwnerTypes.NPCSale || goodsOwner == GoodsOwnerTypes.mallSale || goodsOwner == GoodsOwnerTypes.OtherOnSale || goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
		{
			UISprite component = this.AddIcon.tweenTarget.GetComponent<UISprite>();
			UISprite component2 = this.SubIcon.tweenTarget.GetComponent<UISprite>();
			if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
			{
				this.BuyNumIcon.BtnTag = TipsOperationTypes.JiaoYiShuoGouMai.ToString();
				this.AddIcon.isEnabled = false;
				this.SubIcon.isEnabled = false;
				component.spriteName = this.addIconSpriteNames[1];
				component2.spriteName = this.subIconSpriteNames[1];
			}
			else
			{
				this.BuyNumIcon.BtnTag = string.Empty;
				this.AddIcon.isEnabled = true;
				this.SubIcon.isEnabled = true;
				component.spriteName = this.addIconSpriteNames[0];
				component2.spriteName = this.subIconSpriteNames[0];
			}
			num2 = 35;
			this.FootBuy.Visibility = true;
			if (goodsOwner == GoodsOwnerTypes.OtherOnSale)
			{
				this.AddIcon.gameObject.SetActive(false);
				this.SubIcon.gameObject.SetActive(false);
			}
		}
		else
		{
			num = 40;
			this.FootBuy.Visibility = false;
		}
		this.PropsPanel.clipRange = new Vector4(155f, -100f - (float)(num / 2), 336f, 360f + (float)num);
		this.PropsPanel.transform.localPosition = new Vector3(-136f, 71f, 0f);
		Transform transform = this.PropsList.gameObject.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, (float)(posY - 5), transform.localPosition.z);
		this.IconScrollBar.transform.localPosition = new Vector3(-51f, 18f + (float)num2, 0f);
		this.NeedRefreshBarState = true;
		this.LateUpdateCount = 0;
	}

	private void LateUpdate()
	{
		if (this.NeedRefreshBarState && this.LateUpdateCount++ >= 2)
		{
			this.ShowTopOrBottomBar = this.PropsDraggablePanel.shouldMoveVertically;
			if (this.ShowTopOrBottomBar)
			{
				this.IconScrollBar.gameObject.SetActive(true);
				this.IconScrollBar.spriteName = "down";
			}
			else
			{
				this.IconScrollBar.gameObject.SetActive(false);
			}
			this.NeedRefreshBarState = false;
			this.LateUpdateCount = 0;
		}
	}

	private void OnPropsPanelDragfinished()
	{
		if (this.ShowTopOrBottomBar)
		{
			bool flag = false;
			bool flag2 = false;
			this.PropsDraggablePanel.IsOnTopOrBottom(ref flag, ref flag2);
			if (flag2)
			{
				this.IconScrollBar.spriteName = "up";
			}
			else
			{
				this.IconScrollBar.spriteName = "down";
			}
		}
	}

	private void SetRebornBaoShiInfo(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (!Global.IsRebornEquip(goodsXmlNodeByID.Categoriy) || string.IsNullOrEmpty(goodsData.Props))
		{
			this.PropsXuanCai.gameObject.SetActive(false);
			this.PropsChongSheng.gameObject.SetActive(false);
			return;
		}
		this.SetXuanCai(goodsOwner, goodsData);
		this.SetChongShengBaoShi(goodsOwner, goodsData);
	}

	private void SetXuanCai(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		int xuanCaiBaoShiId = this.GetXuanCaiBaoShiId(goodsData);
		if (xuanCaiBaoShiId <= 0)
		{
			this.PropsXuanCai.gameObject.SetActive(false);
		}
		else
		{
			this.PropsXuanCai.gameObject.SetActive(true);
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xuanCaiBaoShiId);
			if (goodsXmlNodeByID != null)
			{
				this.txtXuanCaiBaoShiName.text = goodsXmlNodeByID.Title;
				this.imgXuanCaiIcon.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", goodsXmlNodeByID.IconCode);
			}
			string text = "16E53B";
			string text2 = "786F6F";
			List<XuanCaiShuXingVO> xuanCaiShuXingByGoodId = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiShuXingByGoodId(xuanCaiBaoShiId);
			if (xuanCaiShuXingByGoodId != null && xuanCaiShuXingByGoodId.Count == 3)
			{
				int roleId = Global.Data.roleData.RoleID;
				if (goodsOwner == GoodsOwnerTypes.SelfBag || goodsOwner == GoodsOwnerTypes.ChongShengLianlu)
				{
					roleId = Global.Data.roleData.RoleID;
				}
				else if (goodsOwner == GoodsOwnerTypes.OtherRole || goodsOwner == GoodsOwnerTypes.LookOther)
				{
					roleId = Super.GData.OtherRoleData.RoleID;
				}
				else
				{
					roleId = -1;
				}
				List<bool> list;
				if (goodsData.Using == 0)
				{
					list = new List<bool>();
					list.Add(false);
					list.Add(false);
					list.Add(false);
				}
				else
				{
					list = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetLevelXuanCai(roleId, xuanCaiBaoShiId);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < 3; i++)
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
							string text3 = (!list[i]) ? text2 : text;
							if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
							{
								stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
								{
									text3,
									string.Format("{0} + {1}%\n\r", extPropIndexesVOByWord.Description, (float.Parse(array2[1]) * 100f).ToString("f1"))
								}));
							}
							else
							{
								stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
								{
									text3,
									string.Format("{0} + {1}\n\r", extPropIndexesVOByWord.Description, array2[1])
								}));
							}
						}
					}
				}
				this.txtXuanCaiValue.Text = stringBuilder.ToString();
			}
			else
			{
				this.PropsXuanCai.gameObject.SetActive(false);
			}
		}
	}

	private void SetChongShengBaoShi(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		List<string> chongShengBaoShiId = this.GetChongShengBaoShiId(goodsData);
		if (chongShengBaoShiId.Count < 1)
		{
			this.PropsChongSheng.gameObject.SetActive(false);
		}
		else
		{
			this.PropsChongSheng.gameObject.SetActive(true);
		}
		Dictionary<int, double[]> systemParamIntDoubleDictByName = ConfigSystemParam.GetSystemParamIntDoubleDictByName("DaKongShuXing", '|', ',');
		for (int i = 0; i < this.lstChongShengBaoShi.Count; i++)
		{
			if (i < chongShengBaoShiId.Count)
			{
				int num = chongShengBaoShiId[i].Split(new char[]
				{
					'_'
				})[1].SafeToInt32(0);
				int num2 = chongShengBaoShiId[i].Split(new char[]
				{
					'_'
				})[0].SafeToInt32(0);
				double num3 = 0.0;
				string text = (i != chongShengBaoShiId.Count - 1) ? "\n\r" : string.Empty;
				if (systemParamIntDoubleDictByName.ContainsKey(num2))
				{
					num3 = systemParamIntDoubleDictByName[num2][1];
				}
				ChongShengBaoShiVO chongShengBaoShiById = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetChongShengBaoShiById(num);
				this.lstChongShengBaoShi[i].SetActive(true);
				GameObject gameObject = this.lstChongShengBaoShi[i];
				ShowNetImage component = gameObject.transform.FindChild("icon").GetComponent<ShowNetImage>();
				TextBlock component2 = gameObject.transform.FindChild("txtValue").GetComponent<TextBlock>();
				float num4 = Mathf.Lerp(30f, 20f, (float)chongShengBaoShiById.Level / 10f);
				component.transform.localScale = new Vector3(num4, num4, 1f);
				component.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num);
				string[] array = chongShengBaoShiById.ShuXing.Split(new char[]
				{
					'|'
				});
				string[] array2 = null;
				if (goodsXmlNodeByID.Categoriy >= 30 && goodsXmlNodeByID.Categoriy <= 34)
				{
					array2 = array[1].Split(new char[]
					{
						','
					});
				}
				else if (goodsXmlNodeByID.Categoriy >= 35 && goodsXmlNodeByID.Categoriy <= 38)
				{
					array2 = array[0].Split(new char[]
					{
						','
					});
				}
				ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]);
				if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
				{
					component2.text = string.Format("{0} + {1}%{2}", extPropIndexesVOByWord.Description, ((double)(float.Parse(array2[1]) * 100f) * num3).ToString("f1"), text);
				}
				else
				{
					component2.text = string.Format("{0} + {1}{2}", extPropIndexesVOByWord.Description, ((double)array2[1].SafeToInt32(0) * num3).ToString("f0"), text);
				}
			}
			else
			{
				this.lstChongShengBaoShi[i].SetActive(false);
			}
		}
	}

	private int GetXuanCaiBaoShiId(GoodsData goodsData)
	{
		string[] array = goodsData.Props.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'_'
				});
				int num = array2[0].SafeToInt32(0);
				if (num == -1)
				{
					return array2[2].SafeToInt32(0);
				}
			}
		}
		return -1;
	}

	private List<string> GetChongShengBaoShiId(GoodsData goodsData)
	{
		List<string> list = new List<string>();
		string[] array = goodsData.Props.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'_'
				});
				int num = array2[0].SafeToInt32(0);
				if (num > -1)
				{
					int num2 = array2[1].SafeToInt32(0);
					int num3 = array2[2].SafeToInt32(0);
					if (num3 > 0)
					{
						string text = num2.ToString() + "_" + num3.ToString();
						list.Add(text);
					}
				}
			}
		}
		return list;
	}

	private void SetCuiLianValue(GoodsData goodsData, GoodVO goodVO, GoodsOwnerTypes goodsOwner)
	{
		if (goodsData.Using == 0 || !Global.IsRebornEquip(goodVO.Categoriy))
		{
			this.PropsCuiLian.gameObject.SetActive(false);
			return;
		}
		this.PropsCuiLian.gameObject.SetActive(true);
		ItemCategories categoriy = (ItemCategories)goodVO.Categoriy;
		EquipCuiLianXmlData equipByCategoriy = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipByCategoriy(categoriy, (HandTypes)goodsData.BagIndex);
		if (equipByCategoriy == null)
		{
			this.PropsCuiLian.gameObject.SetActive(false);
			return;
		}
		int roleID = Global.Data.roleData.RoleID;
		if (goodsOwner == GoodsOwnerTypes.SelfBag || goodsOwner == GoodsOwnerTypes.SysGifts)
		{
			roleID = Global.Data.roleData.RoleID;
		}
		else if (goodsOwner == GoodsOwnerTypes.OtherRole || goodsOwner == GoodsOwnerTypes.LookOther)
		{
			roleID = Super.GData.OtherRoleData.RoleID;
		}
		else
		{
			roleID = -1;
		}
		RebornEquipData levelByEquipKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetLevelByEquipKey(equipByCategoriy.CateCuiLian, roleID);
		if (levelByEquipKey == null)
		{
			this.PropsCuiLian.gameObject.SetActive(false);
			return;
		}
		int level = levelByEquipKey.Level;
		if (level <= 0)
		{
			this.PropsCuiLian.gameObject.SetActive(false);
			return;
		}
		this.PropsCuiLian.gameObject.SetActive(true);
		EquipQuenchingVO equipQuenchingVODataByLevelAndKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipQuenchingVODataByLevelAndKey(equipByCategoriy.CateCuiLian, level);
		if (equipQuenchingVODataByLevelAndKey == null)
		{
			this.PropsCuiLian.gameObject.SetActive(false);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = equipQuenchingVODataByLevelAndKey.EverLelAttribute.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string word = array[i].Split(new char[]
			{
				','
			})[0];
			ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(word);
			if (ConfigExtPropIndexes.GetPercentByWord(word))
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"c39550",
					Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord.Description, (float.Parse(array[i].Split(new char[]
					{
						','
					})[1]) * 100f).ToString("f2")) + Environment.NewLine)
				}));
			}
			else
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"c39550",
					Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord.Description, array[i].Split(new char[]
					{
						','
					})[1]) + Environment.NewLine)
				}));
			}
		}
		this.txtCuiLianValue.text = stringBuilder.ToString();
	}

	private string GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, int categoriy = -1, GoodsOwnerTypes owner = GoodsOwnerTypes.SelfBag)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (categoriy == 980)
		{
			this.QianghuaProgressBar.Visibility = false;
			this.QianghuaProgressBar1.Visibility = false;
			foreach (KeyValuePair<string, double> keyValuePair in IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(gd.GoodsID, gd.ElementhrtsProps[0]))
			{
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair.Key))
				{
					string text3 = text;
					string text4 = "{0}: {1}%";
					Dictionary<string, double>.Enumerator enumerator;
					KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
					object extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair2.Key, false);
					KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
					text = text3 + string.Format(text4, extPropIndexesDescriptionByWord, (int)(keyValuePair3.Value * 100.0));
				}
				else
				{
					string text5 = text;
					string text6 = "{0}: {1}";
					Dictionary<string, double>.Enumerator enumerator;
					KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
					object extPropIndexesDescriptionByWord2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair4.Key, false);
					KeyValuePair<string, double> keyValuePair5 = enumerator.Current;
					text = text5 + string.Format(text6, extPropIndexesDescriptionByWord2, (int)keyValuePair5.Value);
				}
				text += "\n";
			}
			return text;
		}
		if ((categoriy > 6 && categoriy < 10) || (categoriy > 19 && categoriy != 21 && categoriy < 40) || categoriy > 45 || categoriy == 9 || categoriy == 7 || Global.IsRebornEquip(categoriy))
		{
			this.QianghuaProgressBar.Visibility = false;
			this.QianghuaProgressBar1.Visibility = false;
		}
		else if (Global.MaxForgeLevel == 20)
		{
			this.QianghuaProgressBar.Visibility = true;
			this.QianghuaProgressBar1.Visibility = true;
		}
		else if (gd.Forge_level <= 15)
		{
			this.QianghuaProgressBar.Visibility = true;
			this.QianghuaProgressBar1.Visibility = false;
		}
		else
		{
			this.QianghuaProgressBar.Visibility = true;
			this.QianghuaProgressBar1.Visibility = true;
			this.QianghuaProgressBar1.MaxLevel = gd.Forge_level - 15;
		}
		if (gd.Forge_level >= 0 && gd.Forge_level <= 15)
		{
			this.QianghuaProgressBar.Level = gd.Forge_level;
			this.QianghuaProgressBar1.Level = 0;
		}
		else
		{
			this.QianghuaProgressBar.Level = 15;
			this.QianghuaProgressBar1.Level = gd.Forge_level - 15;
		}
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
		double num = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd);
		double num2 = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd);
		int i = 1;
		while (i <= 10)
		{
			text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false);
			if (categoriy != 340)
			{
				goto IL_326;
			}
			if (i != 8 && i != 9 && i != 10 && i != 4 && i != 5 && i != 6)
			{
				if (i == 7)
				{
					text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[45]);
				}
				if (i == 3)
				{
					text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[46]);
				}
				num = 0.0;
				num2 = 0.0;
				goto IL_326;
			}
			IL_651:
			i += 2;
			continue;
			IL_326:
			if (i == 1)
			{
				if (equipFields_1[i] != 0.0)
				{
					text += string.Format("{0}: {1}%", text2, (int)equipFields_1[i]);
					text += "\n";
				}
				goto IL_651;
			}
			int num3 = i;
			int num4 = i + 1;
			if (equipFields_1[num3] != 0.0 || equipFields_1[num4] != 0.0)
			{
				double num5 = equipFields_1[num3];
				double num6 = equipFields_1[num4];
				byte b = 0;
				if ((owner == GoodsOwnerTypes.LookOther || owner == GoodsOwnerTypes.OtherRole || owner == GoodsOwnerTypes.OtherRole2) && categoriy == 340)
				{
					b = 1;
					float num7 = 0f;
					float num8 = 0f;
					if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(gd.Strong))
					{
						num7 = IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[gd.Strong].AdvancedEffect;
					}
					if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(gd.GoodsID, gd.Forge_level + 1) != null)
					{
						num8 = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(gd.GoodsID, gd.Forge_level + 1).AdvancedEffectFloat;
					}
					if (0f >= num7)
					{
						num7 = 0f;
					}
					num5 += (double)num7 * num5 + (double)num8 * num5;
					num6 += (double)num7 * num6 + (double)num8 * num6;
					num5 = Math.Round(num5, 0);
					num6 = Math.Round(num6, 0);
				}
				double num9 = 0.0;
				double num10 = 0.0;
				double num11 = 0.0;
				if (b == 0)
				{
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
					num9 = Global.GetEquipForgeAddBaseValue(gd, num4);
					num10 = Math.Max(num5 * num9, 3.0);
					num11 = Math.Max(num6 * num9, 3.0);
				}
				string text7 = string.Empty;
				string text8 = string.Empty;
				if (num9 > 0.0)
				{
					if (categoriy != 7 && categoriy != 23)
					{
						text7 = Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							string.Format("(+{0})", (int)num10)
						});
						text8 = Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							string.Format("(+{0})", (int)num11)
						});
						text += string.Format("{0}: {1}{2} - {3}{4}", new object[]
						{
							text2,
							(int)num5,
							text7,
							(int)num6,
							text8
						});
					}
					else
					{
						text += string.Format("{0}: {1} - {2}", text2, (int)num5, (int)num6);
					}
				}
				else
				{
					text += string.Format("{0}: {1} - {2}", text2, (int)num5, (int)num6);
				}
				text += "\n";
				goto IL_651;
			}
			goto IL_651;
		}
		string text9 = string.Empty;
		for (i = 11; i < 177; i++)
		{
			if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 24)
			{
				FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(ItemCategories.Fashion, gd.GoodsID, gd.Forge_level);
				if (fashionLevelupVO != null)
				{
					double num12 = 0.0;
					if (fashionLevelupVO.AttDic.TryGetValue(i, ref num12))
					{
						if (ConfigExtPropIndexes.GetPercentByID(i))
						{
							text += string.Format("{0}: {1}%", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false), (int)(num12 * 100.0));
						}
						else
						{
							text += string.Format("{0}: {1}", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false), (int)num12);
						}
						text += "\n";
					}
				}
			}
			else if (equipFields_1[i] != 0.0)
			{
				text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false);
				if (categoriy == 340)
				{
					if (i == 8 || i == 9 || i == 10 || i == 4 || i == 5 || i == 6)
					{
						goto IL_A5D;
					}
					if (i == 7)
					{
						text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(45, false);
					}
					if (i == 3)
					{
						text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(46, false);
					}
				}
				double num13 = equipFields_1[i];
				double num14 = Global.GetEquipForgeAddBaseValue(gd, i);
				if ((owner == GoodsOwnerTypes.LookOther || owner == GoodsOwnerTypes.OtherRole || owner == GoodsOwnerTypes.OtherRole2) && categoriy == 340 && IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(gd.Strong))
				{
					num14 += (double)IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[gd.Strong].AdvancedEffect;
				}
				if (ConfigExtPropIndexes.GetPercentByID(i))
				{
					if (categoriy != 8 || i != 24)
					{
						if (num14 > 0.0)
						{
							if (categoriy != 7 && categoriy != 23)
							{
								text9 = Global.GetColorStringForNGUIText(new object[]
								{
									"00FF00",
									string.Format("(+{0}%)", (int)num14 * 100)
								});
								text += string.Format("{0}: {1}%{2}", text2, (int)(num13 * 100.0), text9);
							}
							else
							{
								text += string.Format("{0}: {1}%", text2, (int)(num13 * 100.0));
							}
						}
						else
						{
							text += string.Format("{0}: {1}%", text2, (int)(num13 * 100.0));
						}
					}
				}
				else if (Global.IsRebornEquip(categoriy))
				{
					if (i == 122 || i == 123 || i == 129 || i == 130 || i == 136 || i == 137 || i == 143 || i == 144 || i == 150 || i == 151 || i == 157 || i == 158)
					{
						int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(gd);
						num13 = (double)ChongShengData.GetEnlargeByZhuoYue(zhuoyueAttributeCount2) * num13;
					}
					text += string.Format("{0}: {1}", text2, (int)num13);
				}
				else if (num14 > 0.0)
				{
					if (categoriy != 7 && categoriy != 23)
					{
						text9 = Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							string.Format("(+{0})", (int)(num13 * num14))
						});
						text += string.Format("{0}: {1}{2}", text2, (int)num13, text9);
					}
					else
					{
						text += string.Format("{0}: {1}", text2, (int)num13);
					}
				}
				else
				{
					text += string.Format("{0}: {1}", text2, (int)num13);
				}
				text += "\n";
			}
			IL_A5D:;
		}
		if (categoriy == 340)
		{
			HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(gd.GoodsID);
			if (horsePokedexByHorseID != null)
			{
				string text10 = text;
				text = string.Concat(new object[]
				{
					text10,
					Global.GetLang("移动速度："),
					(int)(horsePokedexByHorseID.HorseSpeed * 100f),
					"%",
					Environment.NewLine
				});
			}
		}
		return this.ProcessStr(text);
	}

	private string GetZhuijiaAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (gd.AppendPropLev <= 0)
		{
			this.ZhuijiaProgressBar.Percent = 0.0;
			this.ZhuijiaProgressBar.uiLabel.text = string.Format("{0}/{1}", gd.AppendPropLev, Global.GetMaxZhuijiaLevelByGoodsData(gd));
			return text;
		}
		int maxZhuijiaLevelByGoodsData = Global.GetMaxZhuijiaLevelByGoodsData(gd);
		this.ZhuijiaProgressBar.Percent = (double)((float)gd.AppendPropLev / (float)maxZhuijiaLevelByGoodsData);
		this.ZhuijiaProgressBar.uiLabel.text = string.Format("{0}/{1}", gd.AppendPropLev, maxZhuijiaLevelByGoodsData);
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
		double num = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd);
		double num2 = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd);
		for (int i = 8; i <= 13; i++)
		{
			text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
			double num3 = equipFields_1[i];
			if (num3 != 0.0)
			{
				double equipZhuijiaAddBaseValue = Global.GetEquipZhuijiaAddBaseValue(gd, i);
				if (i == 8 || i == 10)
				{
					num3 += num * num3;
				}
				int num4 = (int)Math.Ceiling(num3 * equipZhuijiaAddBaseValue);
				if (num4 > 0)
				{
					text += string.Format("{0}: +{1}", text2, num4);
					text += "\n";
				}
			}
		}
		return this.ProcessStr(text);
	}

	private string GetXilianAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		string text = string.Empty;
		int num = 0;
		int num2 = 5;
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
		if (zhuoyueAttributeCount <= 0)
		{
			return text;
		}
		Dictionary<int, int> xilianPropsUpLimitDict = Global.GetXilianPropsUpLimitDict(gd);
		float xilianPropsUpFactor = Global.GetXilianPropsUpFactor(gd);
		if (xilianPropsUpLimitDict == null)
		{
			return text;
		}
		if (gd.WashProps == null)
		{
			foreach (int num3 in xilianPropsUpLimitDict.Keys)
			{
				if (xilianPropsUpLimitDict[num3] > 0)
				{
					if (ExtPropIndexes.ExtPropIndexPercents[num3] == 1)
					{
						text += string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num3]), 0);
					}
					else if (ExtPropIndexes.ExtPropIndexPercents[num3] == 0)
					{
						text += string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num3]), 0);
					}
					text += this.GetSpaceString(Mathf.Max(num2 - num.ToString().Length, 0));
					text += this.GetUpLimitValueStr(xilianPropsUpLimitDict, xilianPropsUpFactor, num3, 0);
					text += "\n";
				}
			}
		}
		else
		{
			for (int i = 0; i < gd.WashProps.Count; i += 2)
			{
				int num4 = gd.WashProps[i];
				num = gd.WashProps[i + 1];
				if (ExtPropIndexes.ExtPropIndexPercents[num4] == 1)
				{
					text += string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num);
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[num4] == 0)
				{
					text += string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num);
				}
				text += this.GetSpaceString(Mathf.Max(num2 - num.ToString().Length, 0));
				text += this.GetUpLimitValueStr(xilianPropsUpLimitDict, xilianPropsUpFactor, num4, num);
				text += "\n";
			}
		}
		return this.ProcessStr(text);
	}

	private string GetUpLimitValueStr(Dictionary<int, int> dict, float factor, int key, int currentValue)
	{
		int num = 0;
		string text = string.Empty;
		if (dict != null && dict.TryGetValue(key, ref num))
		{
			num = (int)((float)num * factor);
			text += "     ";
			if (currentValue >= num)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"808080",
					Global.GetLang("已达上限")
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"808080",
					string.Format(Global.GetLang("最高上限 +{0}"), num)
				});
			}
		}
		return text;
	}

	private string GetZhuanshengAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (gd.ChangeLifeLevForEquip <= 0)
		{
			return text;
		}
		this.txtZhuanshengNum.Text = string.Format(Global.GetLang("[转生属性] {0}转"), gd.ChangeLifeLevForEquip);
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
		double num = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd);
		double num2 = (zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd);
		for (int i = 4; i <= 10; i += 2)
		{
			text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
			double num3 = equipFields_1[i];
			double equipZhuanshengAddBaseValue = Global.GetEquipZhuanshengAddBaseValue(gd, i);
			if (i == 4 || i == 6)
			{
				num3 += (double)((int)(num2 * num3));
			}
			else if (i == 8 || i == 10)
			{
				num3 += (double)((int)(num * num3));
			}
			int num4 = (int)Math.Ceiling(num3 * equipZhuanshengAddBaseValue);
			if (num4 > 0)
			{
				text += string.Format("{0}: +{1}", text2, num4);
				text += "\n";
			}
		}
		return this.ProcessStr(text);
	}

	public string GetZhuoyuePropStr(int flag)
	{
		string result = string.Empty;
		string text = string.Empty;
		string text2 = ZhuoyuePropIndexes.ZhuoyuePropIndexChineseNames[flag];
		if (flag == 1 || flag == 2 || flag == 9)
		{
			text = string.Format(Global.GetLang("人物等级/{0}"), ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag]);
		}
		else
		{
			text = ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag].ToString();
		}
		if (ZhuoyuePropIndexes.ZhuoyuePropIndexPercents[flag] == 1)
		{
			result = string.Format("{0}: +{1}%", text2, text);
		}
		else
		{
			result = string.Format("{0}: +{1}", text2, text);
		}
		return result;
	}

	private string GetZhuoyueAttributeStr(GoodsData gd)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 980)
		{
			List<int> washProps = gd.WashProps;
			if (washProps != null)
			{
				int i = 0;
				while (i < washProps.Count)
				{
					int id = washProps[++i];
					double num = (double)washProps[++i] / 1000.0;
					i++;
					text += ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(id, true);
					if (ConfigExtPropIndexes.GetPercentByID(id))
					{
						text = text + (num * 100.0).ToString("f1") + "%\n";
					}
					else
					{
						text = text + num.ToString("f1") + "\n";
					}
				}
			}
			return this.ProcessStr(text);
		}
		int num2 = 32;
		for (int j = 0; j < num2; j++)
		{
			if (Global.GetIntSomeBit(gd.ExcellenceInfo, j) == 1)
			{
				text2 = this.GetZhuoyuePropStr(j);
				if (!string.IsNullOrEmpty(text2))
				{
					text += text2;
					text += "\n";
				}
			}
		}
		return this.ProcessStr(text);
	}

	private string GetXingyunAttributeStr(GoodsData gd)
	{
		string text = string.Empty;
		if (gd.Lucky <= 0)
		{
			return text;
		}
		text += Global.GetLang("幸运一击概率: +5%");
		text += "\n";
		return this.ProcessStr(text);
	}

	public void InitTaoZhuangPropsXmlDataDict()
	{
		if (this.TaoZhuangPropsXmlDataDict != null)
		{
			return;
		}
		this.TaoZhuangPropsXmlDataDict = new Dictionary<int, TaoZhuangPropsXmlData>();
		XElement gameResXml = Global.GetGameResXml("Config/TaoZhuangProps.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			TaoZhuangPropsXmlData taoZhuangPropsXmlData = new TaoZhuangPropsXmlData();
			taoZhuangPropsXmlData.ID = Global.GetXElementAttributeInt(xelement, "ID");
			taoZhuangPropsXmlData.Name = Global.GetXElementAttributeStr(xelement, "Name");
			taoZhuangPropsXmlData.GoodsID = Global.GetXElementAttributeStr(xelement, "GoodsID");
			taoZhuangPropsXmlData.Multi = Global.GetXElementAttributeInt(xelement, "Multi");
			taoZhuangPropsXmlData.TaoZhuangProps = Global.GetXElementAttributeStr(xelement, "TaoZhuangProps");
			int id = taoZhuangPropsXmlData.ID;
			if (!this.TaoZhuangPropsXmlDataDict.ContainsKey(id))
			{
				this.TaoZhuangPropsXmlDataDict.Add(id, taoZhuangPropsXmlData);
			}
		}
	}

	public string GetTaoZhuangPropsStr(GoodsData goodsData, GoodsOwnerTypes goodsOwner = GoodsOwnerTypes.SelfBag, int roid = -1)
	{
		string text = string.Empty;
		if (goodsData == null)
		{
			return text;
		}
		if (this.TaoZhuangPropsXmlDataDict == null)
		{
			this.InitTaoZhuangPropsXmlDataDict();
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return text;
		}
		if (!Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
		{
			int num = 0;
			if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorseZaiZaoSuitID", ',');
				num = Mathf.Min(systemParamIntArrayByName);
			}
			else if (goodsXmlNodeByID.Categoriy == 980)
			{
				int leiXingByGoodsID = IConfigbase<ConfigShengYinShengJi>.Instance.GetLeiXingByGoodsID(goodsXmlNodeByID.ID);
				int num2 = 0;
				if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.HolyGoodsDataList != null)
				{
					for (int i = 0; i < Global.Data.roleData.HolyGoodsDataList.Count; i++)
					{
						GoodsData goodsData2 = Global.Data.roleData.HolyGoodsDataList[i];
						if (goodsData2 != null && goodsData2.Using == 1 && leiXingByGoodsID == IConfigbase<ConfigShengYinShengJi>.Instance.GetLeiXingByGoodsID(goodsData2.GoodsID))
						{
							num2++;
						}
					}
				}
				if (0 <= num2)
				{
					ShengYinTaoZhuangVO shengYinTaoZhuangVODataByLeiXing = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengYinTaoZhuangVODataByLeiXing(leiXingByGoodsID);
					if (!string.IsNullOrEmpty(shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr2))
					{
						text += Global.GetColorStringForNGUIText(new object[]
						{
							(2 > num2) ? "786F6F" : "16E53B",
							Global.GetLang("2件") + "\n" + shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr2
						});
					}
					if (!string.IsNullOrEmpty(shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr4))
					{
						text += Global.GetColorStringForNGUIText(new object[]
						{
							(4 > num2) ? "786F6F" : "16E53B",
							Global.GetLang("4件") + "\n" + shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr4
						});
					}
					if (!string.IsNullOrEmpty(shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr6))
					{
						text += Global.GetColorStringForNGUIText(new object[]
						{
							(num2 != 6) ? "786F6F" : "16E53B",
							Global.GetLang("6件") + "\n" + shengYinTaoZhuangVODataByLeiXing.TaoZhuangStr6
						});
					}
				}
			}
			else
			{
				num = Global.ShenqiZaizaoSuit;
			}
			if (goodsXmlNodeByID.SuitID <= num)
			{
				return text;
			}
		}
		TaoZhuangPropsXmlData taoZhuangPropsXmlData = null;
		if (this.TaoZhuangPropsXmlDataDict.TryGetValue(goodsXmlNodeByID.ShouShiSuitID, ref taoZhuangPropsXmlData))
		{
			Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
			if (goodsOwner == GoodsOwnerTypes.OtherRole || goodsOwner == GoodsOwnerTypes.LookOther)
			{
				for (int j = 0; j < Super.GData.OtherRoleData.GoodsDataList.Count; j++)
				{
					GoodsData goodsData3 = Super.GData.OtherRoleData.GoodsDataList[j];
					if (!dictionary.ContainsKey(goodsData3.Id))
					{
						dictionary.Add(goodsData3.Id, goodsData3);
					}
					else
					{
						dictionary[goodsData3.Id] = goodsData3;
					}
				}
			}
			else
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
				if (40 <= categoriyByGoodsID && 45 >= categoriyByGoodsID)
				{
					if (Global.Data.roleData.GoodsDataList != null)
					{
						for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
						{
							if (Global.Data.roleData.GoodsDataList[k].Using > 0)
							{
								GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[k].GoodsID);
								if (goodsXmlNodeByID2 != null && goodsXmlNodeByID2.Categoriy >= 40 && goodsXmlNodeByID2.Categoriy <= 45)
								{
									if (dictionary == null)
									{
										dictionary = new Dictionary<int, GoodsData>();
									}
									dictionary.Add(Global.Data.roleData.GoodsDataList[k].GoodsID, Global.Data.roleData.GoodsDataList[k]);
								}
							}
						}
					}
				}
				else if (Global.IsRebornEquip(categoriyByGoodsID))
				{
					dictionary = ChongShengData.GetUsingChongShengGoodsDataList();
				}
				else
				{
					dictionary = Global.GetUsingGoodsDataList();
				}
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<int, GoodsData>();
			}
			string[] array = taoZhuangPropsXmlData.GoodsID.Split(new char[]
			{
				','
			});
			if (array == null)
			{
				return text;
			}
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			int num3 = 0;
			bool isALl = Convert.ToBoolean(taoZhuangPropsXmlData.Multi);
			foreach (string strNum in array)
			{
				int num4 = strNum.SafeToInt32(0);
				text2 = "786F6F";
				goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num4);
				int num5 = 0;
				if (this.IsExistByGoodsID(dictionary, num4, isALl, ref num5))
				{
					text2 = "16E53B";
					if (num5 > 0)
					{
						num3 += num5;
					}
					else
					{
						num3++;
					}
				}
				text3 += "    ";
				text3 += Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					goodsXmlNodeByID.Title
				});
				text3 += "\n";
			}
			int num6 = 0;
			text4 = this.GetFormatPropsStr(taoZhuangPropsXmlData.TaoZhuangProps, num3, out num6);
			text += string.Format("{0} ({1}/{2})", taoZhuangPropsXmlData.Name, num3, num6);
			text += "\n";
			text += text3;
			text += text4;
		}
		return this.ProcessStr(text);
	}

	public string GetJueXingStr(GoodsData goodsData, GoodsOwnerTypes goodsOwner = GoodsOwnerTypes.SelfBag, int roid = -1)
	{
		RoleData roleData = Global.Data.roleData;
		if (goodsOwner == GoodsOwnerTypes.SelfBag)
		{
			roleData = Global.Data.roleData;
			if (goodsData.Using != 1)
			{
				return string.Empty;
			}
		}
		else
		{
			if (goodsOwner != GoodsOwnerTypes.OtherRole)
			{
				return string.Empty;
			}
			roleData = Super.GData.OtherRoleData;
			if (roleData == null)
			{
				return string.Empty;
			}
			if (goodsData.Using != 1)
			{
				return string.Empty;
			}
		}
		if (roleData.JueXingData == null)
		{
			return string.Empty;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		JueXingPositionType jueXingPositin = JueXingData.GetJueXingPositin(goodsXmlNodeByID.Categoriy, goodsData.BagIndex == 0);
		TaoZhuangDesSettingInfo taoZhuangDesSettingInfo = new TaoZhuangDesSettingInfo();
		taoZhuangDesSettingInfo.shuXingNameColor = "16E53B";
		taoZhuangDesSettingInfo.shuXingValuecolor = "16E53B";
		taoZhuangDesSettingInfo.notVisableColor = "786F6F";
		taoZhuangDesSettingInfo.weaponEffectjiange = "    ";
		taoZhuangDesSettingInfo.beShowTaoZhuangName = true;
		taoZhuangDesSettingInfo.roleData = roleData;
		MUAwakenSuitDetail awakenSuitDetailById;
		if (jueXingPositin >= JueXingPositionType.WuQi && jueXingPositin <= JueXingPositionType.YouJieZhi)
		{
			awakenSuitDetailById = JueXingData.GetAwakenSuitDetailById(roleData.JueXingData.AttackEquip);
		}
		else
		{
			if (jueXingPositin < JueXingPositionType.TouKui || jueXingPositin > JueXingPositionType.XieZi)
			{
				return string.Empty;
			}
			awakenSuitDetailById = JueXingData.GetAwakenSuitDetailById(roleData.JueXingData.DefenseEquip);
		}
		if (awakenSuitDetailById == null)
		{
			return string.Empty;
		}
		return JueXingData.GetTaoZhuangShuXingDes(awakenSuitDetailById, taoZhuangDesSettingInfo);
	}

	private bool IsExistByGoodsID(Dictionary<int, GoodsData> dict, int goodsID, bool isALl, ref int allCount)
	{
		if (dict == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, GoodsData> keyValuePair in dict)
		{
			GoodsData value = keyValuePair.Value;
			if (value.GoodsID == goodsID)
			{
				if (!isALl)
				{
					return true;
				}
				allCount++;
			}
		}
		return allCount > 0;
	}

	private string GetFormatPropsStr(string propsStr, int count, out int maxCount)
	{
		maxCount = 0;
		string text = string.Empty;
		if (string.IsNullOrEmpty(propsStr))
		{
			return text;
		}
		string[] array = propsStr.Split(new char[]
		{
			'|'
		});
		string text2 = string.Empty;
		if (array != null)
		{
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2 != null)
				{
					int num2 = array2[0].SafeToInt32(0);
					text2 = ((count < num2) ? "786F6F" : "16E53B");
					if (num2 != num)
					{
						text += Global.GetColorStringForNGUIText(new object[]
						{
							text2,
							string.Format(Global.GetLang("({0})件"), num2)
						});
						text += "\n";
						num = num2;
					}
					text += "    ";
					if (array2[1].Equals("Mdefense") || array2[1].Equals("Attack") || array2[1].Equals("MaxLifeV") || array2[1].Equals("Defense") || array2[1].Equals("Mattack") || array2[1].Equals("LifeSteal") || array2[1].Equals("AddAttack"))
					{
						int num3 = ExtPropIndexes.ExtPropIndexNames.IndexOf(array2[1].ToLower());
						text += Global.GetColorStringForNGUIText(new object[]
						{
							text2,
							string.Format(Global.GetLang("◆{0}: "), Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[num3])),
							text2,
							string.Format("{0}", array2[2])
						});
					}
					else
					{
						ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[1]);
						if (extPropIndexesVOByWord != null)
						{
							if (ConfigExtPropIndexes.GetPercentByID(extPropIndexesVOByWord.ID))
							{
								text += Global.GetColorStringForNGUIText(new object[]
								{
									text2,
									string.Format(Global.GetLang("◆{0}: "), Global.GetLang(extPropIndexesVOByWord.Description)),
									text2,
									string.Format("{0}%", (float.Parse(array2[2]) * 100f).ToString("f1"))
								});
							}
							else
							{
								text += Global.GetColorStringForNGUIText(new object[]
								{
									text2,
									string.Format(Global.GetLang("◆{0}: "), Global.GetLang(extPropIndexesVOByWord.Description)),
									text2,
									string.Format("{0}", array2[2])
								});
							}
						}
					}
					text += "\n";
					if (maxCount < num2)
					{
						maxCount = num2;
					}
				}
			}
		}
		return text;
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
			num = Global.GetRoleOwnNumByMoneyType(13);
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
		if (price <= 0)
		{
			return 0;
		}
		return num / price;
	}

	private void RefreshPrice()
	{
		string text = (this.BuyNum > this.MaxBuyNum) ? "ff0000" : "ffffff";
		this.txtSalePrice.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"C39550",
			Global.GetLang("总价: "),
			text,
			this.BuyOnePrice * this.BuyNum
		});
		this.IconSprite.spriteName = GTipServiceEx.PriceIconUnits[this.PriceTypeIndex];
		this.IconSprite.transform.localPosition = new Vector3(this.txtSalePrice.transform.localPosition.x + (float)((int)this.txtSalePrice.ActualWidth) + 10f, -8f, 0f);
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private string GetChineseNum(int num)
	{
		string text = string.Empty;
		int num2 = num / 10;
		if (num2 > 1)
		{
			text += string.Format("{0}{1}", this.ChineseNum[num2], this.ChineseNum[10]);
		}
		else if (num2 == 1)
		{
			text += string.Format("{0}", this.ChineseNum[10]);
		}
		int num3 = num % 10;
		if (num3 > 0)
		{
			text += this.ChineseNum[num3];
		}
		return text;
	}

	private List<string[]> GetPropsStrArr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		List<string[]> list = new List<string[]>();
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			list.Add(array2);
		}
		return list;
	}

	private string GetSpaceString(int num)
	{
		string text = string.Empty;
		while (num > 0)
		{
			text += "  ";
			num--;
		}
		return text;
	}

	private void SetMenus(GoodsOwnerTypes goodsOwner, GoodsData goodsData, bool selfBagOnly, byte showBtns)
	{
		this.RefreshBtnsPos();
		if (goodsData != null && ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).Categoriy == 7)
		{
			NGUITools.SetActiveChildren(this.Menus.gameObject, false);
			return;
		}
		this.JiagongIcon.Text = Global.GetLang("加工");
		NGUITools.SetActiveChildren(this.Menus.gameObject, false);
		if (showBtns == 0)
		{
			return;
		}
		this.m_BtnMaxNum.isEnabled = false;
		if (goodsOwner == GoodsOwnerTypes.SelfBag)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (goodsData.Using == 1)
			{
				if ((categoriy == 9 || categoriy == 10) && goodsData.Site != 10000)
				{
					this.ChaKanIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.TipsCallBack(TipsOperationTypes.CheckPet, 0, HandTypes.None);
					};
					NGUITools.SetActive(this.ChaKanIcon.gameObject, true);
				}
				NGUITools.SetActive(this.XiexiaIcon.gameObject, true);
				NGUITools.SetActive(this.JiagongIcon.gameObject, categoriy != 9 && categoriy != 22 && categoriy != 10 && categoriy != 23 && categoriy != 24);
				this.MenusList.repositionNow = true;
				return;
			}
			if (!selfBagOnly)
			{
				NGUITools.SetActive(this.FangruIcon.gameObject, true);
				NGUITools.SetActive(this.ChushouIcon.gameObject, true);
				if (goodsOwner == GoodsOwnerTypes.SelfStall)
				{
					NGUITools.SetActive(this.FangruIcon.gameObject, false);
					NGUITools.SetActive(this.ChushouIcon.gameObject, false);
					NGUITools.SetActive(this.m_BtnShangJia, true);
				}
			}
			else
			{
				if (categoriy == 9 || categoriy == 10)
				{
					this.ShiYongIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.TipsCallBack(TipsOperationTypes.UsePet, 0, HandTypes.None);
					};
					NGUITools.SetActive(this.ShiYongIcon.gameObject, true);
					if (goodsData.Site == 0)
					{
						if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LingDI))
						{
							this.BtnPaiZhu.Text = Global.GetLang("派驻");
							this.BtnPaiZhu.MouseLeftButtonUp = delegate(object e, MouseEvent s)
							{
								if (ConfigSystemParam.GetSystemParamIntByName("ManorPetMax") > (long)Global.GetRolePaiPets(false).Count)
								{
									this.TipsCallBack(TipsOperationTypes.PaiZhuPet, 0, HandTypes.None);
								}
								else
								{
									Super.HintMainText(Global.GetLang("位置已满，无法派驻"), 10, 3);
									this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
								}
							};
							this.BtnPaiZhu.transform.localPosition = new Vector3(40f, 697f, 0f);
							NGUITools.SetActive(this.BtnPaiZhu.gameObject, true);
						}
					}
					else if (goodsData.Site == 10000)
					{
						this.BtnPaiZhu.Text = Global.GetLang("撤回");
						this.BtnPaiZhu.MouseLeftButtonUp = delegate(object e, MouseEvent s)
						{
							this.TipsCallBack(TipsOperationTypes.PaiZhuPet, 0, HandTypes.None);
						};
						NGUITools.SetActive(this.BtnPaiZhu.gameObject, true);
					}
				}
				else if (categoriy == 340)
				{
					this.ShiYongIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.TipsCallBack(TipsOperationTypes.UseHorse, 0, HandTypes.None);
					};
					NGUITools.SetActive(this.ShiYongIcon.gameObject, true);
					if (0.0 <= ConfigSystemParam.GetSystemParamDoubleByName("HorseReturnNum"))
					{
						this.BtnReset.MouseLeftButtonUp = delegate(object s, MouseEvent e)
						{
							int num = goodsData.Forge_level + 1;
							if (num < 2)
							{
								Super.HintMainText(Global.GetLang("坐骑阶数必须大于2阶"), 10, 3);
								return;
							}
							Dictionary<int, int> dictionary = new Dictionary<int, int>();
							for (int k = 2; k <= num; k++)
							{
								HorseAdvancedVO horseAdvancedVOByID = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsData.GoodsID, k);
								if (horseAdvancedVOByID != null)
								{
									for (int l = 0; l < horseAdvancedVOByID.NeedGoodsLst.Count; l++)
									{
										if (dictionary.ContainsKey(horseAdvancedVOByID.NeedGoodsLst[l].GoodsID))
										{
											if (0 > dictionary[horseAdvancedVOByID.NeedGoodsLst[l].GoodsID])
											{
												dictionary[horseAdvancedVOByID.NeedGoodsLst[l].GoodsID] = 0;
											}
											Dictionary<int, int> dictionary3;
											Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
											int num3;
											int num2 = num3 = horseAdvancedVOByID.NeedGoodsLst[l].GoodsID;
											num3 = dictionary3[num3];
											dictionary2[num2] = num3 + horseAdvancedVOByID.NeedGoodsLst[l].GCount;
										}
										else
										{
											dictionary[horseAdvancedVOByID.NeedGoodsLst[l].GoodsID] = horseAdvancedVOByID.NeedGoodsLst[l].GCount;
										}
									}
								}
							}
							double num4 = ConfigSystemParam.GetSystemParamDoubleByName("HorseReturnNum");
							string text = Global.GetLang("重置后坐骑阶数将降低为1级，返还") + (num4 * 100.0).ToString("f0") + Global.GetLang("%升阶材料") + Global.GetLang("（");
							if (0 < dictionary.Count)
							{
								if (1.0 < num4)
								{
									num4 = 1.0;
								}
								if (0.0 > num4)
								{
									num4 = 0.0;
								}
								Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
								byte b = 0;
								while (enumerator.MoveNext())
								{
									string text2 = text;
									object[] array = new object[4];
									array[0] = text2;
									int num5 = 1;
									KeyValuePair<int, int> keyValuePair = enumerator.Current;
									array[num5] = Global.GetGoodsNameByID(keyValuePair.Key, true);
									array[2] = "*";
									int num6 = 3;
									KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
									array[num6] = Super.ToInt((double)keyValuePair2.Value * num4, 1);
									text = string.Concat(array);
									if ((int)(b += 1) < dictionary.Count)
									{
										text += Global.GetLang("、");
									}
								}
							}
							text += ")";
							string[] buttons = new string[]
							{
								Global.GetLang("确定"),
								Global.GetLang("取消")
							};
							GChildWindow gchildWindow = Super.ShowMessageBoxEx(Global.GetLang("提示"), text, delegate(object s1, DPSelectedItemEventArgs e1)
							{
								if (e1.ID == 0)
								{
									GameInstance.Game.SendRidePetReset(goodsData.Id);
									this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
								}
							}, buttons);
						};
						NGUITools.SetActive(this.BtnReset.gameObject, true);
					}
				}
				bool flag = categoriy != 9 && categoriy != 10;
				if (categoriy == 24)
				{
					List<GoodsData> roleFashionList = Global.GetRoleFashionList(ItemCategories.Fashion);
					if (0 < roleFashionList.Count)
					{
						for (int i = 0; i < roleFashionList.Count; i++)
						{
							if (goodsData.GoodsID == roleFashionList[i].GoodsID)
							{
								flag = false;
								break;
							}
						}
					}
				}
				else if (categoriy == 340)
				{
					flag = false;
				}
				else if (categoriy == 27)
				{
					List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
					if (0 < roleHorseFashionList.Count)
					{
						for (int j = 0; j < roleHorseFashionList.Count; j++)
						{
							if (goodsData.GoodsID == roleHorseFashionList[j].GoodsID)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if ((categoriy == 24 || categoriy == 27) && flag)
				{
					this.PeidaiIcon.Text = Global.GetLang("使用");
				}
				else if (categoriy == 23)
				{
					if (!Global.GetDecorationHaveActive(goodsData.GoodsID))
					{
						this.PeidaiIcon.Text = Global.GetLang("激活");
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					this.PeidaiIcon.Text = Global.GetLang("佩戴");
				}
				NGUITools.SetActive(this.PeidaiIcon.gameObject, flag);
				NGUITools.SetActive(this.JiagongIcon.gameObject, categoriy != 9 && categoriy != 22 && categoriy != 10 && (categoriy != 23 && categoriy != 24) && categoriy != 340);
				NGUITools.SetActive(this.ChushouIcon.gameObject, categoriy != 23 && categoriy != 24);
				if (goodsXmlNodeByID.Categoriy == 23)
				{
					NGUITools.SetActive(this.ChushouIcon.gameObject, Global.GetDecorationHaveActive(goodsData.GoodsID));
				}
				if (Global.g_bIsTipsShowCuiHuiBtn)
				{
					this.m_BtnCuiHui.gameObject.SetActive(true);
				}
			}
		}
		if (goodsOwner == GoodsOwnerTypes.JiaoYiShuo)
		{
			this.BuyNumIcon.gameObject.SetActive(true);
		}
		else if (goodsOwner == GoodsOwnerTypes.Exchange || goodsOwner == GoodsOwnerTypes.SelfPet)
		{
			NGUITools.SetActive(this.QuhuiIcon.gameObject, true);
		}
		else if (goodsOwner == GoodsOwnerTypes.Lianlu || goodsOwner == GoodsOwnerTypes.ChongShengLianlu)
		{
			NGUITools.SetActive(this.FangruIcon.gameObject, true);
		}
		else if (goodsOwner != GoodsOwnerTypes.SysGifts && goodsOwner != GoodsOwnerTypes.ChatGoods && goodsOwner != GoodsOwnerTypes.OtherRole && goodsOwner != GoodsOwnerTypes.OtherRole2 && goodsOwner != GoodsOwnerTypes.SelfBagNoMenu)
		{
			if (goodsOwner == GoodsOwnerTypes.SelfStall)
			{
				if (null != this.m_BtnShangJia && 0 >= goodsData.Binding && !Global.IsTimeLimitGoods(goodsData))
				{
					NGUITools.SetActive(this.m_BtnShangJia.gameObject, true);
				}
			}
			else if (goodsOwner == GoodsOwnerTypes.jingLingYaoSai)
			{
				if (goodsData.Site == 10000)
				{
					this.BtnPaiZhu.Text = Global.GetLang("撤回");
					this.BtnPaiZhu.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						this.TipsCallBack(TipsOperationTypes.PaiZhuPet, 0, HandTypes.None);
					};
					NGUITools.SetActive(this.BtnPaiZhu.gameObject, true);
					this.ShiYongIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.TipsCallBack(TipsOperationTypes.UsePet, 0, HandTypes.None);
					};
					NGUITools.SetActive(this.ShiYongIcon.gameObject, true);
				}
			}
			else if (goodsOwner == GoodsOwnerTypes.ZuoQiCangKu)
			{
				NGUITools.SetActive(this.QuhuiIcon.gameObject, true);
			}
			else if (goodsOwner == GoodsOwnerTypes.HolyBagShengJi || goodsOwner == GoodsOwnerTypes.HolyBagShengJiXuanZhong)
			{
				this.JiagongIcon.gameObject.SetActive(true);
				if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 980)
				{
					this.JiagongIcon.Text = Global.GetLang("升级");
				}
			}
			else if (goodsOwner == GoodsOwnerTypes.HolyBag && Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 980)
			{
				this.JiagongIcon.gameObject.SetActive(true);
				this.JiagongIcon.Text = Global.GetLang("升级");
				if (goodsData.Using == 1)
				{
					this.PeidaiIcon.gameObject.SetActive(false);
					this.XiexiaIcon.gameObject.SetActive(true);
				}
				else
				{
					this.PeidaiIcon.gameObject.SetActive(true);
					this.XiexiaIcon.gameObject.SetActive(false);
				}
			}
		}
		if (this.ChushouIcon.gameObject.activeSelf)
		{
			this.ChushouIcon.Text = Global.GetLang("回收");
			this.ChushouIcon.BtnTag = goodsData.Id.ToString();
		}
		this.MenusList.onReposition = new UITable.OnReposition(this.OnBtnReposition);
		this.MenusList.repositionNow = true;
		if (goodsData != null && ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).Categoriy == 7)
		{
			NGUITools.SetActiveChildren(this.Menus.gameObject, false);
		}
	}

	private void OnBtnReposition()
	{
		Vector3 localPosition = this.MenusList.transform.localPosition;
		localPosition.y = this.Foot.localPosition.y;
		this.MenusList.transform.localPosition = localPosition;
	}

	private void RefreshBtnsPos()
	{
		byte b = 0;
		try
		{
			Transform transform = this.BtnPaiZhu.transform;
			float x = this.BtnPaiZhu.transform.localPosition.x;
			int[] menuPosY = this.MenuPosY;
			byte b2 = b;
			b = b2 + 1;
			transform.localPosition = new Vector3(x, menuPosY[(int)b2], this.BtnPaiZhu.transform.localPosition.z);
			Transform transform2 = this.m_BtnCuiHui.transform;
			float x2 = this.m_BtnCuiHui.transform.localPosition.x;
			int[] menuPosY2 = this.MenuPosY;
			byte b3 = b;
			b = b3 + 1;
			transform2.localPosition = new Vector3(x2, menuPosY2[(int)b3], this.m_BtnCuiHui.transform.localPosition.z);
			Transform transform3 = this.m_BtnShangJia.transform;
			float x3 = this.m_BtnShangJia.transform.localPosition.x;
			int[] menuPosY3 = this.MenuPosY;
			byte b4 = b;
			b = b4 + 1;
			transform3.localPosition = new Vector3(x3, menuPosY3[(int)b4], this.m_BtnShangJia.transform.localPosition.z);
			Transform transform4 = this.PeidaiIcon.transform;
			float x4 = this.PeidaiIcon.transform.localPosition.x;
			int[] menuPosY4 = this.MenuPosY;
			byte b5 = b;
			b = b5 + 1;
			transform4.localPosition = new Vector3(x4, menuPosY4[(int)b5], this.PeidaiIcon.transform.localPosition.z);
			Transform transform5 = this.ChaKanIcon.transform;
			float x5 = this.ChaKanIcon.transform.localPosition.x;
			int[] menuPosY5 = this.MenuPosY;
			byte b6 = b;
			b = b6 + 1;
			transform5.localPosition = new Vector3(x5, menuPosY5[(int)b6], this.ChaKanIcon.transform.localPosition.z);
			Transform transform6 = this.ShiYongIcon.transform;
			float x6 = this.ShiYongIcon.transform.localPosition.x;
			int[] menuPosY6 = this.MenuPosY;
			byte b7 = b;
			b = b7 + 1;
			transform6.localPosition = new Vector3(x6, menuPosY6[(int)b7], this.ShiYongIcon.transform.localPosition.z);
			Transform transform7 = this.JiagongIcon.transform;
			float x7 = this.JiagongIcon.transform.localPosition.x;
			int[] menuPosY7 = this.MenuPosY;
			byte b8 = b;
			b = b8 + 1;
			transform7.localPosition = new Vector3(x7, menuPosY7[(int)b8], this.JiagongIcon.transform.localPosition.z);
			Transform transform8 = this.XiexiaIcon.transform;
			float x8 = this.XiexiaIcon.transform.localPosition.x;
			int[] menuPosY8 = this.MenuPosY;
			byte b9 = b;
			b = b9 + 1;
			transform8.localPosition = new Vector3(x8, menuPosY8[(int)b9], this.XiexiaIcon.transform.localPosition.z);
			Transform transform9 = this.FangruIcon.transform;
			float x9 = this.FangruIcon.transform.localPosition.x;
			int[] menuPosY9 = this.MenuPosY;
			byte b10 = b;
			b = b10 + 1;
			transform9.localPosition = new Vector3(x9, menuPosY9[(int)b10], this.FangruIcon.transform.localPosition.z);
			Transform transform10 = this.QuhuiIcon.transform;
			float x10 = this.QuhuiIcon.transform.localPosition.x;
			int[] menuPosY10 = this.MenuPosY;
			byte b11 = b;
			b = b11 + 1;
			transform10.localPosition = new Vector3(x10, menuPosY10[(int)b11], this.QuhuiIcon.transform.localPosition.z);
			Transform transform11 = this.BtnReset.transform;
			float x11 = this.BtnReset.transform.localPosition.x;
			int[] menuPosY11 = this.MenuPosY;
			byte b12 = b;
			b = b12 + 1;
			transform11.localPosition = new Vector3(x11, menuPosY11[(int)b12], this.BtnReset.transform.localPosition.z);
			Transform transform12 = this.ChushouIcon.transform;
			float x12 = this.ChushouIcon.transform.localPosition.x;
			int[] menuPosY12 = this.MenuPosY;
			byte b13 = b;
			b = b13 + 1;
			transform12.localPosition = new Vector3(x12, menuPosY12[(int)b13], this.ChushouIcon.transform.localPosition.z);
			Transform transform13 = this.BuyNumIcon.transform;
			float x13 = this.BuyNumIcon.transform.localPosition.x;
			int[] menuPosY13 = this.MenuPosY;
			byte b14 = b;
			b = b14 + 1;
			transform13.localPosition = new Vector3(x13, menuPosY13[(int)b14], this.BuyNumIcon.transform.localPosition.z);
		}
		catch (Exception ex)
		{
		}
	}

	public void AddNum()
	{
		int length = this.BuyNum.ToString().Length;
		this.BuyNum = Math.Min(this.BuyNum + (int)Math.Pow(10.0, (double)(length - 1)), this.MaxBuyNum);
		this.FootBuyTxtNum.text = this.BuyNum.ToString();
		this.RefreshPrice();
	}

	public void SubNum()
	{
		int length = this.BuyNum.ToString().Length;
		this.BuyNum = Math.Max(this.BuyNum - (int)Math.Pow(10.0, (double)Math.Max(length - 2, 0)), 1);
		this.FootBuyTxtNum.text = this.BuyNum.ToString();
		this.RefreshPrice();
	}

	private void TipsCallBack(TipsOperationTypes type, int id = 0, HandTypes handTypes = HandTypes.None)
	{
		bool flag = false;
		if (handTypes == HandTypes.None)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
			flag = true;
		}
		else
		{
			this.SetSwitchHandIcon((int)handTypes);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 9,
				Flag = (int)handTypes,
				MyID = this.m_nDBID
			});
		}
		if (type == TipsOperationTypes.UsePet || type == TipsOperationTypes.CheckPet || type == TipsOperationTypes.PaiZhuPet)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id
			});
		}
		if (type == TipsOperationTypes.UseHorse)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id
			});
		}
		if (type > TipsOperationTypes.Close && type < TipsOperationTypes.SwitchHand)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id
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
		if (type == TipsOperationTypes.JiaoYiShuoGouMai)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id
			});
		}
		if (flag)
		{
			GTipServiceEx.ClearChildWindow();
		}
	}

	private void SetSwitchHandIcon(int handTypes)
	{
		if (handTypes == -1)
		{
			NGUITools.SetActive(this.SwitchHandIcon.gameObject, false);
			this._HandTypes = HandTypes.None;
		}
		else
		{
			NGUITools.SetActive(this.SwitchHandIcon.gameObject, true);
			this._HandTypes = (HandTypes)handTypes;
			if (handTypes == 1)
			{
				this.SwitchHandIcon.Text = Global.GetLang("对比左手");
			}
			else if (handTypes == 0)
			{
				this.SwitchHandIcon.Text = Global.GetLang("对比右手");
			}
		}
	}

	internal void RenderTipsCanUse(bool CanUse)
	{
		if (null != this.GoodIcon)
		{
			base.StartCoroutine<bool>(this.DoAction(delegate
			{
				if (CanUse == NGUITools.GetActive(this.GoodIcon.NoUseSprite))
				{
					this.GoodIcon.NoUseSpriteVisible = !CanUse;
				}
			}));
		}
	}

	private IEnumerator DoAction(Action action)
	{
		yield return new WaitForEndOfFrame();
		action.Invoke();
		yield break;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public UISprite Bak;

	public GButton CloseBtn;

	public ListBox m_SkillsLst;

	private ObservableCollection m_Collection;

	public GameObject m_JingLingSkillobj;

	public SpriteSL Menus;

	public UITable MenusList;

	public GButton PeidaiIcon;

	public GButton XiexiaIcon;

	public GButton JiagongIcon;

	public GButton ChushouIcon;

	public GButton FangruIcon;

	public GButton QuhuiIcon;

	public GButton m_BtnShangJia;

	public GButton m_BtnCuiHui;

	public GButton m_BtnMaxNum;

	public GButton ShiYongIcon;

	public GButton ChaKanIcon;

	public GButton BtnPaiZhu;

	public GButton BtnReset;

	public UISprite IconScrollBar;

	public GButton BuyNumIcon;

	public UIButton AddIcon;

	public UIButton SubIcon;

	private int[] MenuPosY = new int[]
	{
		751,
		697,
		643,
		573,
		511,
		449,
		387,
		325,
		263,
		201,
		139,
		77,
		15,
		-57
	};

	public TextBlock txtTitle;

	public TextBlock txtZhandouli;

	public TextBlock txtPeidaiMode;

	public TextBlock txtLevel;

	public TextBlock txtOcc;

	public TextBlock txtType;

	public TextBlock txtZhuansheng;

	public TextBlock txtSuitID;

	public TextBlock txtTiaojian;

	public GGoodIcon GoodIcon;

	public UISprite ZhandouliCompareFlag;

	public GButton SwitchHandIcon;

	private string[] zuoyueSpriteNames = new string[]
	{
		"tips_ZhuoYue",
		"tips_ZhuoYue1",
		"tips_ZhuoYue2"
	};

	public Transform Body;

	public UIPanel PropsPanel;

	public TextBlock txtInfo;

	public UITable PropsList;

	public SpriteSL PropsJuHun;

	public SpriteSL PropsFuMo;

	public SpriteSL PropsBase;

	public SpriteSL PropsXuanCai;

	public SpriteSL PropsChongSheng;

	public SpriteSL PropsZhuijia;

	public SpriteSL PropsXianlian;

	public SpriteSL PropsZhuansheng;

	public SpriteSL PropsZhuoyue;

	public SpriteSL PropsXingxun;

	public SpriteSL PropsTaozhuang;

	public SpriteSL PropsJueXing;

	public SpriteSL PropsCuiLian;

	public TextBlock txtJuHunValue;

	public TextBlock txtFuMoValue;

	public TextBlock txtBaseValue;

	public TextBlock txtXuanCaiName;

	public TextBlock txtXuanCaiBaoShiName;

	public TextBlock txtChongShengName;

	public TextBlock txtCuiLianName;

	public TextBlock txtXuanCaiValue;

	public TextBlock txtChongShengValue;

	public TextBlock txtCuiLianValue;

	public ShowNetImage imgXuanCaiIcon;

	public List<GameObject> lstChongShengBaoShi;

	public TextBlock txtZhuijiaValue;

	public TextBlock txtXilianValue;

	public TextBlock txtZhuanshengNum;

	public TextBlock txtZhuanshengValue;

	public TextBlock txtZhuoyueName;

	public TextBlock txtZhuoyueValue;

	public TextBlock txtXingxunValue;

	public TextBlock txtTaozhuangValue;

	public TextBlock txtJueXingValue;

	public GImgProgressBar QianghuaProgressBar;

	public GImgProgressBar QianghuaProgressBar1;

	public GImgProgressBar ZhuijiaProgressBar;

	[SerializeField]
	private UISprite mUpSp;

	[SerializeField]
	private GameObject _YiZhuangBei;

	private List<GameObject> mUpSpList = new List<GameObject>();

	public Transform Foot;

	public SpriteSL FootIntro;

	public TextBlock FootIntroTxtNum;

	public SpriteSL FootBuy;

	public TextBlock txtSalePrice;

	public UISprite IconSprite;

	public UILabel FootBuyTxtNum;

	public GameObject FootBuyTxtNumContainer;

	private DPSelectedItemEventHandler DPSelectedItemNum;

	public UISprite m_SpriteZhuoYueBak;

	private int BuyNum;

	private int BuyOnePrice;

	private int PriceTypeIndex;

	private int MaxBuyNum;

	private int m_nDBID;

	private UIDraggablePanel PropsDraggablePanel;

	private bool ShowTopOrBottomBar;

	private bool NeedRefreshBarState;

	private int LateUpdateCount;

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

	public Dictionary<int, TaoZhuangPropsXmlData> TaoZhuangPropsXmlDataDict;

	private string[] ChineseNum = new string[]
	{
		Global.GetLang("零"),
		Global.GetLang("一"),
		Global.GetLang("二"),
		Global.GetLang("三"),
		Global.GetLang("四"),
		Global.GetLang("五"),
		Global.GetLang("六"),
		Global.GetLang("七"),
		Global.GetLang("八"),
		Global.GetLang("九"),
		Global.GetLang("十")
	};

	private HandTypes _HandTypes = HandTypes.None;
}
