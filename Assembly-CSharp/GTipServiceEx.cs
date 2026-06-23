using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GTipServiceEx
{
	public static GGoodTips GGoodTipsPart
	{
		get
		{
			return GTipServiceEx.GoodTips;
		}
	}

	public static bool EquipTipWindowVisiable
	{
		get
		{
			return null != GTipServiceEx.EquipTipWindow && NGUITools.GetActive(GTipServiceEx.EquipTipWindow.gameObject);
		}
	}

	public static void RefreshEquipNoUse(bool CanUse)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (null != GTipServiceEx.EquipTips)
		{
			GTipServiceEx.EquipTips.RenderTipsCanUse(CanUse);
		}
	}

	public static void ShowTip(GGoodIcon sender, TipTypes tipType, GoodsOwnerTypes goodsOwner, GoodsData goodData)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (null != sender)
		{
			GTipServiceEx.TipsSender = sender;
		}
		if (goodData == null)
		{
			return;
		}
		if (tipType == TipTypes.SoulGuardTip)
		{
			GTipServiceEx.ShowSoulGuardTipsWindow(goodsOwner, goodData);
			return;
		}
		if (tipType == TipTypes.FluorescentDiamondBagTip || tipType == TipTypes.SoulCometStoneBagTip)
		{
			GTipServiceEx.ShowFluorescentDiamondTipsWindow(goodsOwner, goodData);
			return;
		}
		if (tipType != TipTypes.YuansuBagTip)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodData.GoodsID);
			if ((categoriyByGoodsID >= 0 && categoriyByGoodsID < 25) || categoriyByGoodsID == 700 || categoriyByGoodsID == 702)
			{
				if ((23 <= categoriyByGoodsID && 27 >= categoriyByGoodsID) || categoriyByGoodsID == 8)
				{
					if (categoriyByGoodsID == 24 && goodsOwner == GoodsOwnerTypes.LookOther)
					{
						GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
					}
					else
					{
						GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodData, null);
					}
				}
				else
				{
					GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
				}
			}
			else if (ConfigBaoXiangTips.IsBaoXiangID(goodData.GoodsID) && Global.IsBaoXiangTips((int)goodsOwner) && Super._ParcelPart == null && goodsOwner != GoodsOwnerTypes.SelfBag)
			{
				GTipServiceEx.ShowBaoXiangTipWindow(goodsOwner, goodData, null);
			}
			else if (categoriyByGoodsID == 340)
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
			}
			else if (categoriyByGoodsID >= 40 && categoriyByGoodsID <= 45)
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
			}
			else if (categoriyByGoodsID == 980)
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
			}
			else if (Global.IsRebornEquip(categoriyByGoodsID))
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodData, null);
			}
			else
			{
				GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodData, null);
			}
		}
		else if (tipType == TipTypes.YuansuBagTip)
		{
			GTipServiceEx.ShowYuansuTipsWindow(goodsOwner, goodData);
		}
	}

	public static void ShowTip(GGoodIcon sender, TipTypes tipType, GoodsOwnerTypes goodsOwner, GoodsPriceUnitTypes goodsPriceUnit, int price, int goodsID, int goodsDbID = -1, int roleID = -1, MarriageData MarriageData = null)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (null != sender)
		{
			GTipServiceEx.TipsSender = sender;
		}
		GoodsData goodsData;
		if (goodsOwner == GoodsOwnerTypes.ChatGoods)
		{
			if (roleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetGoodsByDbID(roleID, goodsDbID);
				GTipServiceEx.IsWaitting = true;
				return;
			}
			goodsData = Global.GetGoodsDataByDbID(goodsDbID, null);
			if (goodsData == null)
			{
				for (int i = 0; i < Global.Data.equipPet.Count; i++)
				{
					if (Global.Data.equipPet[i].Id == goodsDbID)
					{
						goodsData = Global.Data.equipPet[i];
					}
				}
				if (goodsData == null)
				{
					List<GoodsData> roleFashionList = Global.GetRoleFashionList(ItemCategories.Fashion);
					for (int j = 0; j < roleFashionList.Count; j++)
					{
						if (roleFashionList[j].Id == goodsDbID)
						{
							goodsData = roleFashionList[j];
							break;
						}
					}
				}
				if (goodsData == null)
				{
					List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
					for (int k = 0; k < roleDecorationList.Count; k++)
					{
						if (roleDecorationList[k].Id == goodsDbID)
						{
							goodsData = roleDecorationList[k];
							break;
						}
					}
				}
			}
		}
		else
		{
			if (goodsOwner == GoodsOwnerTypes.ZhuanpanGonggao)
			{
				GameInstance.Game.SpriteGetGoodsByDbID(roleID, goodsDbID);
				GTipServiceEx.IsWaitting = true;
				return;
			}
			goodsData = Super.GetShowGoodsDataBy(goodsDbID, (int)goodsOwner, goodsID);
		}
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		if (tipType != TipTypes.NormalText)
		{
			if (tipType != TipTypes.SkillText)
			{
				if (tipType == TipTypes.GoodsText)
				{
					string priceInfo = null;
					if (goodsPriceUnit != GoodsPriceUnitTypes.None)
					{
						priceInfo = string.Format("{0},{1}", (int)goodsPriceUnit, price);
					}
					if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
					{
						if ((23 <= categoriyByGoodsID && 27 >= categoriyByGoodsID) || categoriyByGoodsID == 8)
						{
							GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodsData, null);
						}
						else
						{
							GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, null);
						}
					}
					else if (categoriyByGoodsID == 340)
					{
						GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, priceInfo);
					}
					else if (categoriyByGoodsID >= 40 && categoriyByGoodsID <= 45)
					{
						GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, null);
					}
					else if (Global.IsRebornEquip(categoriyByGoodsID))
					{
						GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, null);
					}
					else
					{
						GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodsData, priceInfo);
					}
				}
				else if (tipType != TipTypes.YuansuBagTip)
				{
					if (tipType != TipTypes.ExternalTip)
					{
						if (tipType != TipTypes.BufferTip)
						{
							if (tipType != TipTypes.ExperienceTip)
							{
								if (tipType != TipTypes.LingLiTip)
								{
									if (tipType != TipTypes.LifeSliderTip)
									{
										if (tipType != TipTypes.MagicSliderTip)
										{
											if (tipType == TipTypes.BonusTip)
											{
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static void ShowTip(GGoodIcon sender, TipTypes tipType, GoodsOwnerTypes goodsOwner, GoodsPriceUnitTypes goodsPriceUnit, int price, GoodsData goodsData)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (null != sender)
		{
			GTipServiceEx.TipsSender = sender;
		}
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		string priceInfo = null;
		if (goodsPriceUnit != GoodsPriceUnitTypes.None)
		{
			priceInfo = string.Format("{0},{1}", (int)goodsPriceUnit, price);
		}
		if (tipType == TipTypes.StallTip)
		{
		}
		if (tipType == TipTypes.GoodsText)
		{
			if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
			{
				if ((23 <= categoriyByGoodsID && 27 >= categoriyByGoodsID) || categoriyByGoodsID == 8)
				{
					GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodsData, priceInfo);
				}
				else
				{
					GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, priceInfo);
				}
			}
			else if (categoriyByGoodsID == 340)
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, priceInfo);
			}
			else if (categoriyByGoodsID >= 40 && categoriyByGoodsID <= 45)
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, priceInfo);
			}
			else if (Global.IsRebornEquip(categoriyByGoodsID))
			{
				GTipServiceEx.ShowEquipTipWindow(goodsOwner, goodsData, priceInfo);
			}
			else
			{
				GTipServiceEx.ShowGoodsTipWindow(goodsOwner, goodsData, priceInfo);
			}
		}
		else if (tipType == TipTypes.YuansuBagTip)
		{
		}
	}

	public static void ShowTip(Vector3 senderPos, TipTypes tipType, int bufferID)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (tipType == TipTypes.BufferTip)
		{
			if (bufferID <= 0)
			{
				return;
			}
			GTipServiceEx.ShowBuffTipWindow(senderPos, bufferID);
		}
	}

	public static void ShowTip(string titleStr, string contentStr, TipTypes tipType = TipTypes.NormalText, bool DestroyWindow = false)
	{
		if (GTipServiceEx.IsWaitting)
		{
			return;
		}
		if (tipType == TipTypes.NormalText)
		{
			GTipServiceEx.ShowNormalTipWindow(titleStr, contentStr, DestroyWindow);
		}
	}

	private static void CloseGoodsTipWindow()
	{
		if (null != GTipServiceEx.GoodTipWindow)
		{
			GTipServiceEx.GoodTipWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.GoodTipWindow);
		}
	}

	private static void ShowGoodsTipWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData, string priceInfo = null)
	{
		GTipServiceEx._GoodData = GoodData;
		if (null != GTipServiceEx.GoodTipWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.GoodTipWindow, Global.GetLang("GoodTips"));
			GTipServiceEx.GoodTips.RenderTips(goodsOwner, GoodData, GTipServiceEx.SelfBagOnly, priceInfo);
			GTipServiceEx.GoodTipWindow.Visibility = true;
			return;
		}
		GTipServiceEx.GoodTipWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.GoodTipWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.GoodTipWindow.IsCache = true;
		GTipServiceEx.GoodTipWindow.IsShowModal = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.GoodTipWindow, Global.GetLang("GoodTips"));
		GTipServiceEx.GoodTipWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseGoodsTipWindow();
			return true;
		};
		GTipServiceEx.GoodTipWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseGoodsTipWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (null == GTipServiceEx.GoodTips)
		{
			GTipServiceEx.GoodTips = U3DUtils.NEW<GGoodTips>();
			GTipServiceEx.GoodTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 12)
				{
					PlayZone.GlobalPlayZone.OpenWuPinShangJiaWindow(GTipServiceEx._GoodData, 0);
				}
				if (e.IDType == 0)
				{
					if (e.ID == 0)
					{
						GTipServiceEx.CloseGoodsTipWindow();
					}
				}
				else if (e.IDType == 1)
				{
					if (e.ID != 0)
					{
						if (e.ID != 1)
						{
							if (e.ID != 2)
							{
								if (e.ID == 3)
								{
								}
							}
						}
					}
				}
			};
			GTipServiceEx.GoodTips.RenderTips(goodsOwner, GoodData, GTipServiceEx.SelfBagOnly, priceInfo);
			GTipServiceEx.GoodTipWindow.SetContent(GTipServiceEx.GoodTipWindow.BodyPresenter, GTipServiceEx.GoodTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.GoodTipWindow);
		}
	}

	private static void CloseEquipsTipWindow()
	{
		if (GTipServiceEx.CloseHander != null)
		{
			GTipServiceEx.CloseHander(null, new DPSelectedItemEventArgs
			{
				Type = 0,
				ID = 1
			});
		}
		if (null != GTipServiceEx.EquipTipWindow)
		{
			if (null != GTipServiceEx.mGModalTips)
			{
				GTipServiceEx.mGModalTips.OnActive(false);
				GTipServiceEx.mGModalTips.Visibility = false;
			}
			GTipServiceEx.EquipTipWindow.Visibility = false;
			if (null != GTipServiceEx.EquipTipsEX)
			{
				GTipServiceEx.EquipTipsEX.Visibility = false;
			}
			GTipServiceEx.CloseChildWindow(GTipServiceEx.EquipTipWindow);
		}
	}

	private static void ShowEquipTipWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData, string priceInfo = null)
	{
		GTipServiceEx._goodsOwner = goodsOwner;
		GTipServiceEx._GoodData = GoodData;
		Dictionary<int, int> tipsResultDict = Global.GetCompareAttributeInfo(GoodData, HandTypes.None);
		bool tipsResultState = GTipServiceEx.IsShowTipsResultWindow(tipsResultDict, goodsOwner, GoodData.GoodsID) && GoodData.Using == 0;
		int num = -1;
		if (tipsResultState)
		{
			tipsResultDict.TryGetValue(GTipServiceEx.HandKey, ref num);
		}
		GTipServiceEx.HandValue = num;
		if (null != GTipServiceEx.EquipTipWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.EquipTipWindow, Global.GetLang("EquipTips"));
			GTipServiceEx.EquipTips.RenderTips(goodsOwner, GoodData, GTipServiceEx.SelfBagOnly, priceInfo, num, 1);
			GTipServiceEx.EquipTipWindow.Visibility = true;
			GTipServiceEx.ShowTipsResultWindow(tipsResultState, tipsResultDict);
			GTipServiceEx.ShowModalTipWindow(goodsOwner, GoodData, tipsResultState);
			return;
		}
		GTipServiceEx.EquipTipWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.EquipTipWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.EquipTipWindow.IsCache = true;
		GTipServiceEx.EquipTipWindow.IsShowModal = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.EquipTipWindow, Global.GetLang("EquipTips"));
		GTipServiceEx.EquipTipWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseEquipsTipWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		GTipServiceEx.EquipTipWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseEquipsTipWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (null == GTipServiceEx.EquipTips)
		{
			GTipServiceEx.EquipTips = U3DUtils.NEW<GEquipTips>();
			GTipServiceEx.EquipTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					GTipServiceEx.CloseEquipsTipWindow();
				}
				else if (e.IDType == 9)
				{
					tipsResultDict.Clear();
					GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(e.MyID, null);
					if (goodsDataByDbID != null)
					{
						tipsResultDict = Global.GetCompareAttributeInfo(goodsDataByDbID, (HandTypes)e.Flag);
						bool tipsResultState = GTipServiceEx.IsShowTipsResultWindow(tipsResultDict, GTipServiceEx._goodsOwner, goodsDataByDbID.GoodsID);
						GTipServiceEx.HandValue = e.Flag;
						tipsResultState = true;
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataByDbID.GoodsID);
						if (categoriyByGoodsID == 24 || categoriyByGoodsID == 23)
						{
							tipsResultState = false;
						}
						GTipServiceEx.ShowTipsResultWindow(tipsResultState, tipsResultDict);
						GTipServiceEx.ShowModalTipWindow(goodsOwner, goodsDataByDbID, tipsResultState);
					}
					else
					{
						tipsResultDict = Global.GetCompareAttributeInfo(GTipServiceEx._GoodData, (HandTypes)e.Flag);
						bool tipsResultState = GTipServiceEx.IsShowTipsResultWindow(tipsResultDict, GTipServiceEx._goodsOwner, GTipServiceEx._GoodData.GoodsID);
						GTipServiceEx.HandValue = e.Flag;
						bool tipsResultState2 = true;
						int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(GoodData.GoodsID);
						if (categoriyByGoodsID2 == 24 || categoriyByGoodsID2 == 23)
						{
							tipsResultState2 = false;
						}
						GTipServiceEx.ShowTipsResultWindow(tipsResultState2, tipsResultDict);
						GTipServiceEx.ShowModalTipWindow(goodsOwner, GoodData, tipsResultState2);
					}
				}
				else if (e.IDType == 12)
				{
					PlayZone.GlobalPlayZone.OpenWuPinShangJiaWindow(GTipServiceEx._GoodData, 0);
				}
			};
			GTipServiceEx.EquipTips.RenderTips(goodsOwner, GoodData, GTipServiceEx.SelfBagOnly, priceInfo, num, 1);
			GTipServiceEx.EquipTipWindow.SetContent(GTipServiceEx.EquipTipWindow.BodyPresenter, GTipServiceEx.EquipTips, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, 40f, 0f);
			GTipServiceEx.EquipTips.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.EquipTipWindow);
		}
		GTipServiceEx.ShowTipsResultWindow(tipsResultState, tipsResultDict);
		GTipServiceEx.ShowModalTipWindow(goodsOwner, GoodData, tipsResultState);
	}

	private static void ShowModalTipWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData, bool tipsResultState)
	{
		if (GoodData == null)
		{
			if (null != GTipServiceEx.mGModalTips)
			{
				GTipServiceEx.mGModalTips.Visibility = false;
			}
			if (null != GTipServiceEx.EquipTips)
			{
				GTipServiceEx.EquipTips.X = 0.0;
			}
		}
		else if (Global.GetGoodsCatetoriy(GoodData.GoodsID) == 340)
		{
			if (null != GTipServiceEx.mGModalTips)
			{
				GTipServiceEx.mGModalTips.Visibility = true;
			}
			else
			{
				GTipServiceEx.mGModalTips = U3DUtils.NEW<GModalTips>();
				GTipServiceEx.EquipTipWindow.SetContent(GTipServiceEx.EquipTipWindow.BodyPresenter, GTipServiceEx.mGModalTips, 0.0, 0.0, true);
			}
			GTipServiceEx.mGModalTips.RefreshTips(goodsOwner, GoodData);
			if (null != GTipServiceEx.EquipTips)
			{
				GTipServiceEx.EquipTips.X = 160.0;
				GTipServiceEx.mGModalTips.X = -160.0;
			}
		}
		else
		{
			if (null != GTipServiceEx.mGModalTips)
			{
				GTipServiceEx.mGModalTips.Visibility = false;
			}
			if (null != GTipServiceEx.EquipTips)
			{
				GTipServiceEx.EquipTips.X = (double)((!tipsResultState) ? 0 : 115);
			}
		}
	}

	private static void ShowTipsResultWindow(bool tipsResultState, Dictionary<int, int> tipsResultDict)
	{
		int num = 115;
		int num2 = -159;
		int num3 = 40;
		if (Global.GetCategoriyByGoodsID(GTipServiceEx._GoodData.GoodsID) == 980)
		{
			if (null != GTipServiceEx.EquipTipsResult)
			{
				GTipServiceEx.EquipTipsResult.Visibility = false;
			}
			if (tipsResultState)
			{
				if (null != GTipServiceEx.EquipTips)
				{
					GTipServiceEx.EquipTips.X = 0.0;
				}
				GoodsData roleRidePetShenShenYinJiSlotUsingGoodsData = Global.GetRoleRidePetShenShenYinJiSlotUsingGoodsData(Global.Data.roleData.HolyGoodsDataList, IConfigbase<ConfigShengYinShengJi>.Instance.GetCaoWeiIndexByGoodsID(GTipServiceEx._GoodData.GoodsID));
				if (roleRidePetShenShenYinJiSlotUsingGoodsData != null)
				{
					if (null == GTipServiceEx.EquipTipsEX)
					{
						GTipServiceEx.EquipTipsEX = U3DUtils.NEW<GEquipTips>();
						GTipServiceEx.EquipTipsEX.RenderTips(GTipServiceEx._goodsOwner, roleRidePetShenShenYinJiSlotUsingGoodsData, true, null, -1, 0);
						GTipServiceEx.EquipTipWindow.SetContent(GTipServiceEx.EquipTipWindow.BodyPresenter, GTipServiceEx.EquipTipsEX, 0.0, 0.0, true);
					}
					else
					{
						GTipServiceEx.EquipTipsEX.Visibility = true;
						GTipServiceEx.EquipTipsEX.RenderTips(GTipServiceEx._goodsOwner, roleRidePetShenShenYinJiSlotUsingGoodsData, true, null, -1, 0);
					}
					GTipServiceEx.EquipTipsEX.transform.localPosition = new Vector3(-224f, (float)num3, 0f);
					GTipServiceEx.EquipTips.X = (double)num;
				}
			}
			else if (null != GTipServiceEx.EquipTipsEX)
			{
				GTipServiceEx.EquipTipsEX.Visibility = false;
			}
		}
		else
		{
			if (tipsResultState)
			{
				if (null == GTipServiceEx.EquipTipsResult)
				{
					GTipServiceEx.EquipTipsResult = U3DUtils.NEW<GEquipTipsResult>();
					GTipServiceEx.EquipTipsResult.RenderTips(tipsResultDict);
					GTipServiceEx.EquipTipWindow.SetContent(GTipServiceEx.EquipTipWindow.BodyPresenter, GTipServiceEx.EquipTipsResult, 0.0, 0.0, true);
				}
				else
				{
					GTipServiceEx.EquipTipsResult.Visibility = true;
					GTipServiceEx.EquipTipsResult.RenderTips(tipsResultDict);
				}
				GTipServiceEx.EquipTips.X = (double)num;
				GTipServiceEx.EquipTipsResult.X = (double)num2;
				GTipServiceEx.EquipTipsResult.Y = (double)num3;
			}
			else
			{
				if (null != GTipServiceEx.EquipTipsResult)
				{
					GTipServiceEx.EquipTipsResult.Visibility = false;
				}
				if (null != GTipServiceEx.EquipTips)
				{
					GTipServiceEx.EquipTips.X = 0.0;
				}
			}
			if (null != GTipServiceEx.EquipTipsEX)
			{
				GTipServiceEx.EquipTipsEX.Visibility = false;
			}
		}
	}

	private static bool IsShowTipsResultWindow(Dictionary<int, int> resultDict, GoodsOwnerTypes goodsOwner, int goodsID)
	{
		if (resultDict == null)
		{
			return false;
		}
		if (!resultDict.ContainsKey(1))
		{
			return false;
		}
		if (goodsOwner != GoodsOwnerTypes.SelfBag && goodsOwner != GoodsOwnerTypes.HolyBag)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
		{
			return true;
		}
		int num = -1;
		if (goodsXmlNodeByID != null)
		{
			num = goodsXmlNodeByID.ToOccupation;
		}
		return goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 10 && goodsXmlNodeByID.Categoriy != 24 && goodsXmlNodeByID.Categoriy != 23 && (1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & num) != 0 && !Global.GetDecorationHaveActive(goodsXmlNodeByID.ID);
	}

	private static void CloseBuffTipWindow()
	{
		if (null != GTipServiceEx.BuffTipWindow)
		{
			GTipServiceEx.BuffTipWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.BuffTipWindow);
		}
	}

	private static void ShowBuffTipWindow(Vector3 senderPos, int bufferID)
	{
		if (null != GTipServiceEx.BuffTipWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.BuffTipWindow, Global.GetLang("BuffTips"));
			GTipServiceEx.BuffTips.RenderTips(bufferID);
			GTipServiceEx.BuffTipWindow.Visibility = true;
			GTipServiceEx.setWinPartPos(GTipServiceEx.BuffTips.gameObject, senderPos);
			return;
		}
		GTipServiceEx.BuffTipWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.BuffTipWindow.ModalType = ChildWindowModalType.TransBak;
		GTipServiceEx.InitChildWindow(GTipServiceEx.BuffTipWindow, Global.GetLang("BuffTips"));
		GTipServiceEx.setModalExSprite(GTipServiceEx.BuffTipWindow, true);
		if (null == GTipServiceEx.BuffTips)
		{
			GTipServiceEx.BuffTips = U3DUtils.NEW<GBuffTips>();
			GTipServiceEx.BuffTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0 && e.ID == 0)
				{
					GTipServiceEx.CloseBuffTipWindow();
				}
			};
			GTipServiceEx.BuffTips.RenderTips(bufferID);
			GTipServiceEx.BuffTipWindow.SetContent(GTipServiceEx.BuffTipWindow.BodyPresenter, GTipServiceEx.BuffTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.BuffTipWindow);
			GTipServiceEx.setWinPartPos(GTipServiceEx.BuffTips.gameObject, senderPos);
		}
	}

	private static void setWinPartPos(GameObject obj, Vector3 pos)
	{
		Vector3 position;
		position..ctor(pos.x, pos.y, obj.transform.position.z);
		obj.transform.position = position;
	}

	private static void setModalExSprite(GChildWindow childWindow, bool isTrue)
	{
		if (isTrue)
		{
			UISprite component = childWindow.ModalBak.GetComponent<UISprite>();
			if (null != component)
			{
				component.spriteName = "none";
			}
			UIEventListener.Get(childWindow.ModalBak.gameObject).onClick = delegate(GameObject s)
			{
				GTipServiceEx.CloseBuffTipWindow();
			};
		}
	}

	private static void ClosNormalTipWindow()
	{
		if (null != GTipServiceEx.NormalTipWindow)
		{
			GTipServiceEx.NormalTipWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.NormalTipWindow);
		}
	}

	private static void ShowNormalTipWindow(string titleStr, string contentStr, bool DestroyWindow = false)
	{
		if (null != GTipServiceEx.NormalTipWindow)
		{
			GTipServiceEx.NormalTipWindow.ModalType = ChildWindowModalType.Translucent;
			GTipServiceEx.InitChildWindow(GTipServiceEx.NormalTipWindow, Global.GetLang("NormalTips"));
			GTipServiceEx.NormalTips.RenderTips(titleStr, contentStr);
			GTipServiceEx.NormalTipWindow.Visibility = true;
			return;
		}
		GTipServiceEx.NormalTipWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.NormalTipWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.InitChildWindow(GTipServiceEx.NormalTipWindow, Global.GetLang("NormalTips"));
		GTipServiceEx.NormalTipWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.ClosNormalTipWindow();
			return true;
		};
		if (null == GTipServiceEx.NormalTips)
		{
			GTipServiceEx.NormalTips = U3DUtils.NEW<GNormalTips>();
			GTipServiceEx.NormalTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0 && e.ID == 0)
				{
					GTipServiceEx.ClosNormalTipWindow();
					if (DestroyWindow)
					{
						Object.Destroy(GTipServiceEx.NormalTipWindow.gameObject);
					}
				}
			};
			GTipServiceEx.NormalTips.RenderTips(titleStr, contentStr);
			GTipServiceEx.NormalTipWindow.SetContent(GTipServiceEx.NormalTipWindow.BodyPresenter, GTipServiceEx.NormalTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.NormalTipWindow);
		}
	}

	private static void CloseYuansuTipsWindow()
	{
		if (null != GTipServiceEx.YuansuTipsWindow)
		{
			GTipServiceEx.YuansuTipsWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.YuansuTipsWindow);
		}
	}

	private static void ShowYuansuTipsWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData)
	{
		if (null != GTipServiceEx.YuansuTipsWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.YuansuTipsWindow, Global.GetLang("YuansuTips"));
			GTipServiceEx.YuansuTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.YuansuTipsWindow.Visibility = true;
			return;
		}
		GTipServiceEx.YuansuTipsWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.YuansuTipsWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.YuansuTipsWindow.IsCache = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.YuansuTipsWindow, Global.GetLang("YuansuTips"));
		GTipServiceEx.YuansuTipsWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseYuansuTipsWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (null == GTipServiceEx.YuansuTips)
		{
			GTipServiceEx.YuansuTips = U3DUtils.NEW<GYuansuTips>();
			GTipServiceEx.YuansuTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					GTipServiceEx.CloseYuansuTipsWindow();
				}
			};
			GTipServiceEx.YuansuTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.YuansuTipsWindow.SetContent(GTipServiceEx.YuansuTipsWindow.BodyPresenter, GTipServiceEx.YuansuTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.YuansuTipsWindow);
		}
	}

	private static void CloseSoulGuardTipsWindow()
	{
		if (null != GTipServiceEx.soulGuardTipsWindow)
		{
			GTipServiceEx.soulGuardTipsWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.soulGuardTipsWindow);
		}
	}

	private static void ShowSoulGuardTipsWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData)
	{
		if (null != GTipServiceEx.soulGuardTipsWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.soulGuardTipsWindow, Global.GetLang("SoulGuardTips"));
			GTipServiceEx.soulGuardTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.soulGuardTipsWindow.Visibility = true;
			return;
		}
		GTipServiceEx.soulGuardTipsWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.soulGuardTipsWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.soulGuardTipsWindow.IsCache = true;
		GTipServiceEx.soulGuardTipsWindow.IsShowModal = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.soulGuardTipsWindow, Global.GetLang("SoulGuardTips"));
		GTipServiceEx.soulGuardTipsWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseSoulGuardTipsWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (null == GTipServiceEx.soulGuardTips)
		{
			GTipServiceEx.soulGuardTips = U3DUtils.NEW<GSoulGuardTips>();
			GTipServiceEx.soulGuardTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					GTipServiceEx.CloseSoulGuardTipsWindow();
				}
			};
			GTipServiceEx.soulGuardTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.soulGuardTipsWindow.SetContent(GTipServiceEx.soulGuardTipsWindow.BodyPresenter, GTipServiceEx.soulGuardTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.soulGuardTipsWindow);
		}
	}

	private static void ShowFluorescentDiamondTipsWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData)
	{
		if (null != GTipServiceEx.fluorescentDiamondWindow)
		{
			GTipServiceEx.InitChildWindow(GTipServiceEx.fluorescentDiamondWindow, Global.GetLang("fluorescentDiamondTips"));
			GTipServiceEx.fluorescentDiamondTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.fluorescentDiamondWindow.Visibility = true;
			return;
		}
		GTipServiceEx.fluorescentDiamondWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.fluorescentDiamondWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.fluorescentDiamondWindow.IsCache = true;
		GTipServiceEx.fluorescentDiamondWindow.IsShowModal = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.fluorescentDiamondWindow, Global.GetLang("fluorescentDiamondTips"));
		GTipServiceEx.fluorescentDiamondWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseFluorescentDiamondTipsWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (null == GTipServiceEx.fluorescentDiamondTips)
		{
			GTipServiceEx.fluorescentDiamondTips = U3DUtils.NEW<GFluorescentDiamondTips>();
			GTipServiceEx.fluorescentDiamondTips.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					GTipServiceEx.CloseFluorescentDiamondTipsWindow();
				}
			};
			GTipServiceEx.fluorescentDiamondTips.RenderTips(goodsOwner, GoodData);
			GTipServiceEx.fluorescentDiamondWindow.SetContent(GTipServiceEx.fluorescentDiamondWindow.BodyPresenter, GTipServiceEx.fluorescentDiamondTips, 0.0, 0.0, false);
			Super.GData.PlayZoneRoot.Children.Add(GTipServiceEx.fluorescentDiamondWindow);
		}
	}

	private static void CloseFluorescentDiamondTipsWindow()
	{
		if (null != GTipServiceEx.fluorescentDiamondWindow)
		{
			GTipServiceEx.fluorescentDiamondWindow.Visibility = false;
			GTipServiceEx.CloseChildWindow(GTipServiceEx.fluorescentDiamondWindow);
		}
	}

	public static void NotifyGetGoodsDataByDBID(GoodsData goodsData)
	{
		GTipServiceEx.IsWaitting = false;
		GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.ChatGoods, goodsData);
	}

	public static void ClearChildWindow()
	{
		GTipServiceEx.TipsSender = null;
	}

	private static void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
	}

	private static void CloseChildWindow(GChildWindow childWindow)
	{
		Super.HideChildWindow(childWindow);
	}

	public static void ResetTipsLayer(GoodsData goodData, string layer)
	{
	}

	private static void ShowBaoXiangTipWindow(GoodsOwnerTypes goodsOwner, GoodsData GoodData, DPSelectedItemEventHandler DPSelectedItem = null)
	{
		if (GTipServiceEx.BaoXiangTipWindow != null)
		{
			GTipServiceEx.CloseBaoXiangTipWindow();
		}
		GTipServiceEx.BaoXiangTipWindow = U3DUtils.NEW<GChildWindow>();
		GTipServiceEx.BaoXiangTipWindow.ModalType = ChildWindowModalType.Translucent;
		GTipServiceEx.BaoXiangTipWindow.IsCache = true;
		GTipServiceEx.BaoXiangTipWindow.IsShowModal = true;
		GTipServiceEx.InitChildWindow(GTipServiceEx.BaoXiangTipWindow, Global.GetLang("BaoXiangUIPart"));
		Super.GData.GlobalPlayZone.Children.Add(GTipServiceEx.BaoXiangTipWindow);
		GTipServiceEx.BaoXiangTipWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseBaoXiangTipWindow();
			return true;
		};
		GTipServiceEx.BaoXiangTipWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			GTipServiceEx.CloseBaoXiangTipWindow();
			GTipServiceEx.ClearChildWindow();
			return true;
		};
		if (GTipServiceEx.BaoXiangTips == null)
		{
			GTipServiceEx.BaoXiangTips = U3DUtils.NEW<BaoXiangUIPart>();
			if (GTipServiceEx.BaoXiangTips)
			{
				GTipServiceEx.IsBaoXiangOpen = true;
				GTipServiceEx.BaoXiangTips.RefreshTipsGoods(GoodData);
				GTipServiceEx.BaoXiangTipWindow.SetContent(GTipServiceEx.BaoXiangTipWindow.BodyPresenter, GTipServiceEx.BaoXiangTips, 0.0, 0.0, false);
				GTipServiceEx.BaoXiangTips.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 0)
					{
						GTipServiceEx.CloseBaoXiangTipWindow();
						GTipServiceEx.ClearChildWindow();
					}
				};
			}
		}
		else
		{
			GTipServiceEx.CloseBaoXiangTipWindow();
			GTipServiceEx.ClearChildWindow();
			MUDebug.Log<string>(new string[]
			{
				"BaoXiangUIPart != null"
			});
		}
	}

	public static void CloseBaoXiangTipWindow()
	{
		GTipServiceEx.TipsSender = null;
		GTipServiceEx.IsBaoXiangOpen = false;
		if (GTipServiceEx.BaoXiangTips != null)
		{
			Object.Destroy(GTipServiceEx.BaoXiangTips.gameObject);
			GTipServiceEx.BaoXiangTips = null;
		}
		if (GTipServiceEx.BaoXiangTipWindow != null)
		{
			GTipServiceEx.CloseChildWindow(GTipServiceEx.BaoXiangTipWindow);
			Object.Destroy(GTipServiceEx.BaoXiangTipWindow.gameObject);
			GTipServiceEx.BaoXiangTipWindow = null;
		}
	}

	public static bool SelfBagOnly = true;

	public static GGoodIcon TipsSender = null;

	private static GGoodTips GoodTips = null;

	private static GChildWindow GoodTipWindow = null;

	private static GEquipTips EquipTips = null;

	private static GEquipTipsResult EquipTipsResult = null;

	private static GChildWindow EquipTipWindow = null;

	private static GBuffTips BuffTips = null;

	private static GChildWindow BuffTipWindow = null;

	private static GNormalTips NormalTips = null;

	private static GChildWindow NormalTipWindow = null;

	private static GYuansuTips YuansuTips = null;

	private static GChildWindow YuansuTipsWindow = null;

	private static GSoulGuardTips soulGuardTips = null;

	private static GChildWindow soulGuardTipsWindow = null;

	private static GChildWindow fluorescentDiamondWindow = null;

	private static GFluorescentDiamondTips fluorescentDiamondTips = null;

	private static GEquipTips EquipTipsEX = null;

	private static GModalTips mGModalTips = null;

	public static int HandKey = 180;

	public static int HandValue = -1;

	private static GoodsData _GoodData = null;

	private static GoodsOwnerTypes _goodsOwner = GoodsOwnerTypes.None;

	private static bool IsWaitting = false;

	public static DPSelectedItemEventHandler CloseHander = null;

	public static string[] PriceInfoUnits = new string[]
	{
		Global.GetLang("金币"),
		Global.GetLang("积分"),
		Global.GetLang("钻石"),
		Global.GetLang("绑定钻石"),
		Global.GetLang("绑定金币"),
		Global.GetLang("王者点数"),
		Global.GetLang("觉醒点")
	};

	public static string[] PriceIconUnits = new string[]
	{
		"moneyJinbi",
		"moneyJifen",
		"moneyZhuanshi",
		"moneyBindZhuanshi",
		"moneyBindJinbi",
		"wangzheDianshu",
		"MoneyLueDuoPoint"
	};

	private static GChildWindow BaoXiangTipWindow = null;

	private static BaoXiangUIPart BaoXiangTips = null;

	public static bool IsBaoXiangOpen = false;
}
