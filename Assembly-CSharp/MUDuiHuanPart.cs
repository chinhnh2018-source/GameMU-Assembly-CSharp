using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class MUDuiHuanPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		MUDuiHuanPart.Instance = null;
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id < 1000)
			{
				DuiHuanItem duiHuanItem = U3DUtils.AS<DuiHuanItem>(this._ItemList.ItemsSource.GetAt(id));
				if (null != duiHuanItem)
				{
					SystemHelpPart.SetMask(duiHuanItem.duihuanBtn, default(Vector4));
				}
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this._Close, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected override void InitializeComponent()
	{
		int num = 0;
		if (Global.isFanbei(12))
		{
			num = 2;
		}
		if (Global.isFanbei(251))
		{
			double num2 = 0.0;
			if (double.TryParse(Global.JieriFanbeiInfo[251].ExtArg1, ref num2))
			{
				num += (int)num2;
			}
		}
		if (num == 0)
		{
			num = 1;
		}
		int num3 = 20 * num;
		this.hintText.text = Global.GetLang("每次幸运之星祈福可获得") + num3 + Global.GetLang("积分");
		this.hintText.Pivot = 5;
		this.ItemCollectionType = this._TypeList.ItemsSource;
		this.ItemCollectionItemList = this._ItemList.ItemsSource;
		this._TypeList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		NGUITools.SetActive(this.m_TreasureXueZuanObj, false);
		this._Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.MUDuiHuanPart, HelpStateEvents.Inactived, 1);
		};
		MUDuiHuanPart.Instance = this;
	}

	public void InitPartData(int Type, int zhanMengLevel)
	{
		this.duHuanType = Type;
		this.ZhanMengLevel = zhanMengLevel;
		if (this.duHuanType == 0)
		{
			this.title.spriteName = "mojinDuihuan";
			this.typeSprite.spriteName = "moJing";
			this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 1)
		{
			GameInstance.Game.SpriteGetZaJinDanJiFen();
			this.title.spriteName = "jifenDuihuan";
			this.typeSprite.spriteName = "qiFuJiFen";
			this.valueText.Text = "0";
			this.hintText.gameObject.SetActive(true);
		}
		else if (this.duHuanType == 2)
		{
			this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.PetJiFen).ToString();
			this.title.spriteName = "jifenDuihuan";
			this.typeSprite.spriteName = "jingLingJiFen";
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 3)
		{
			this.title.spriteName = "zhanggongshop";
			this.typeSprite.spriteName = "zhangGong";
			this.valueText.Text = Global.Data.roleData.BangGong.ToString();
			this.hintText.gameObject.SetActive(false);
			if (this.ZhanMengLevel <= 0)
			{
				GameInstance.Game.SpriteQueryBangHuiDetail(Global.Data.roleData.Faction);
			}
		}
		else if (this.duHuanType == 4)
		{
			this.SpriteType1.enabled = false;
			this.SpriteType2.enabled = true;
			this.Title1.enabled = false;
			this.Title2.enabled = true;
			this.hintText.text = string.Empty;
			this.valueText.text = Global.Data.roleData.TianTiRongYao + string.Empty;
		}
		else if (this.duHuanType == 5)
		{
			this.SpriteType1.enabled = true;
			this.SpriteType2.enabled = false;
			this.Title1.enabled = true;
			this.Title2.enabled = false;
			NGUITools.SetActive(this.hintText, false);
			NGUITools.SetActive(this.m_TreasureXueZuanObj, true);
			this.SpriteType1.spriteName = "CangBaoJiFen";
			this.Title1.spriteName = "CangBaoMiJIngTitle";
			this.valueText.text = Global.GetRoleCommonUseParamsValue(32).ToString();
			this.m_TreasureXueZuanValue.text = Global.GetRoleCommonUseParamsValue(33).ToString();
		}
		else if (this.duHuanType == 6)
		{
			this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhengBaPoint).ToString();
			this.title.spriteName = "wudaoShangDian";
			this.typeSprite.spriteName = "zhengbaDianShu";
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 7)
		{
			this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.BHMatchGuessJiFen).ToString();
			this.title.spriteName = "huangjinliansai";
			this.typeSprite.spriteName = "liansaijifen";
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 9)
		{
			this.valueText.Text = ShiLiData.GetSelfCompPoint().ToString();
			this.title.spriteName = "shilishangdian";
			this.typeSprite.spriteName = "gongxian";
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 8)
		{
			this.UpDataHorsePoint();
			this.title.spriteName = "jifenDuihuan";
			this.typeSprite.spriteName = "zuoqijifen";
			this.hintText.gameObject.SetActive(false);
		}
		else if (this.duHuanType == 10)
		{
			this.SpriteType1.enabled = true;
			this.SpriteType2.enabled = false;
			this.Title1.enabled = false;
			this.Title2.enabled = true;
			this.SpriteType1.spriteName = "ZhanDuiJingJiHuoBi";
			this.hintText.gameObject.SetActive(false);
			this.valueText.Text = Global.Data.roleData.MoneyData[160].ToString();
		}
		else if (this.duHuanType == 11)
		{
			this.SpriteType1.enabled = true;
			this.SpriteType2.enabled = false;
			this.Title1.enabled = false;
			this.Title2.enabled = true;
			this.SpriteType1.spriteName = "zhanduijingcaidian";
			this.hintText.gameObject.SetActive(false);
			this.valueText.Text = Global.Data.roleData.MoneyData[162].ToString();
		}
		if (this.duHuanType != 3 || this.ZhanMengLevel > 0)
		{
			GameInstance.Game.SpriteGetBindDiamondExchangeInfoCmd();
		}
	}

	public void NotifyByZhanMengLevelInfo(int zhanMengLevel)
	{
		if (this.duHuanType == 3)
		{
			this.ZhanMengLevel = zhanMengLevel;
			GameInstance.Game.SpriteGetBindDiamondExchangeInfoCmd();
		}
	}

	private List<int> GetCompBattleShopID()
	{
		int baseStoreId = ShiLiData.GetBaseStoreId();
		int bastStoreId = ShiLiData.GetBastStoreId();
		List<int> list = new List<int>();
		list.Add(baseStoreId);
		CompData selfCompData = ShiLiData.GetSelfCompData();
		if (selfCompData == null)
		{
			return list;
		}
		if (selfCompData.kfCompData == null)
		{
			return list;
		}
		if (selfCompData.kfCompData.compBattleBaseData == null)
		{
			return list;
		}
		CompBattleGameStates compBattleStates = (CompBattleGameStates)selfCompData.kfCompData.CompBattleStates;
		if (compBattleStates == CompBattleGameStates.Start || compBattleStates == CompBattleGameStates.NotJoin || compBattleStates == CompBattleGameStates.Analysis)
		{
			return list;
		}
		if (selfCompData.kfCompData.CompTypeBattle != Global.Data.roleData.CompType)
		{
			return list;
		}
		if (selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList == null)
		{
			return list;
		}
		if (selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList.Count < 3)
		{
			return list;
		}
		int num = selfCompData.kfCompData.CompTypeBattle - 1;
		if (selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList[num].OwnCityList == null)
		{
			return list;
		}
		bool flag = true;
		int count = selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList[num].OwnCityList.Count;
		if (count == 0)
		{
			flag = false;
		}
		else
		{
			for (int i = 0; i < selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList.Count; i++)
			{
				if (i != num)
				{
					if (selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList[i].OwnCityList != null)
					{
						if (selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList[i].OwnCityList.Count >= count)
						{
							flag = false;
							break;
						}
					}
				}
			}
		}
		List<int> ownCityList = selfCompData.kfCompData.compBattleBaseData.CompBattleOwnCityList[num].OwnCityList;
		for (int j = 0; j < ownCityList.Count; j++)
		{
			MUForceCraft forceCraftByID = ShiLiData.GetForceCraftByID(ownCityList[j]);
			if (forceCraftByID != null)
			{
				list.Add(forceCraftByID.DuiHuanType);
			}
		}
		if (flag)
		{
			list.Add(bastStoreId);
		}
		return list;
	}

	public void RefreshDate(Dictionary<int, int> dict)
	{
		this.dayDuiHuanNumDict = dict;
		if (!this.Flag)
		{
			this.RefreshItem(this.tempitem.DuiHuanType, this.tempitem.SaleType);
			return;
		}
		this.Flag = false;
		List<int> list = null;
		if (this.duHuanType == 9)
		{
			list = this.GetCompBattleShopID();
		}
		XElement isolateResXml = Global.GetIsolateResXml("Config/DuiHuanType.xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "DuiHuan"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (this.duHuanType == Global.GetXElementAttributeInt(xelement, "SaleType"))
				{
					if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == Global.GetXElementAttributeInt(xelement, "OccupCondition") || Global.GetXElementAttributeInt(xelement, "OccupCondition") == -1)
					{
						if (this.duHuanType != 9 || list.IndexOf(Global.GetXElementAttributeInt(xelement, "ID")) >= 0)
						{
							string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "PubStartTime");
							string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "PubEndTime");
							if (Global.InLimitTimeRange(xelementAttributeStr, xelementAttributeStr2))
							{
								MUDuiHuanTypeItem muduiHuanTypeItem = U3DUtils.NEW<MUDuiHuanTypeItem>();
								muduiHuanTypeItem.image.URL = StringUtil.substitute("NetImages/GameRes/Images/Hybrid/{0}.png", new object[]
								{
									Global.GetXElementAttributeStr(xelement, "Image")
								});
								muduiHuanTypeItem.title.Text = Global.GetXElementAttributeStr(xelement, "Name");
								muduiHuanTypeItem.info.Text = Global.GetXElementAttributeStr(xelement, "Description");
								muduiHuanTypeItem.DuiHuanType = Global.GetXElementAttributeInt(xelement, "ID");
								muduiHuanTypeItem.SaleType = Global.GetXElementAttributeInt(xelement, "SaleType");
								if (muduiHuanTypeItem.SaleType == 5)
								{
									muduiHuanTypeItem.image.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
									{
										Global.GetXElementAttributeStr(xelement, "Image")
									});
								}
								this.ItemCollectionType.Add(muduiHuanTypeItem);
							}
						}
					}
				}
			}
		}
		this._TypeList.SelectedIndex = 0;
		SystemHelpMgr.OnAction(UIObjIDs.MUDuiHuanPart, HelpStateEvents.Actived, 1);
	}

	public void OnDuiHuanResult(int result, int goodsDbID, int jingYuan, double experience)
	{
		if (result < 1)
		{
			if (result == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该装备不能兑换到任何精元和经验"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备等级不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精元置换时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精元置换成功"), new object[0]), 0, -1, -1, 0);
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		MUDuiHuanTypeItem muduiHuanTypeItem = U3DUtils.AS<MUDuiHuanTypeItem>(this._TypeList.SelectedItem);
		if (null == muduiHuanTypeItem)
		{
			return;
		}
		if (null == muduiHuanTypeItem)
		{
			return;
		}
		if (this.tempitem != null && this.tempitem != muduiHuanTypeItem)
		{
			this.tempitem.bak.spriteName = "lianluEquipItem_bak";
		}
		this.tempitem = muduiHuanTypeItem;
		this.tempitem.bak.spriteName = "lianluEquipItem_bak2";
		this.UIDragPl.target.x = 159f;
		this.UIDragPl.target.y = -37f;
		this.UIDragPl.enabled = true;
		this.ShowIteminfo(muduiHuanTypeItem.DuiHuanType, muduiHuanTypeItem.SaleType);
	}

	private void ShowIteminfo(int type, int SaleType)
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/DuiHuanItems.xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "Items"), "*");
		this.ItemCollectionItemList.Clear();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (type == Global.GetXElementAttributeInt(xelement, "DuiHuanType"))
				{
					DuiHuanItem item = null;
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedZhanMengLevel");
					item = U3DUtils.NEW<DuiHuanItem>();
					this.ItemCollectionItemList.Add(item);
					item.itemID = Global.GetXElementAttributeInt(xelement, "ID");
					item.SourceGoods.BackSpriteName0 = "bagGrid2_bak";
					if (SaleType == 0)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/mojing.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "MoJing");
					}
					else if (SaleType == 1)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/jifen.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "QiFuJiFen");
					}
					else if (SaleType == 2)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/jingLingJiFen.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "PetJiFen");
					}
					else if (SaleType == 3)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/zhangGong.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "ZhanGong");
					}
					else if (SaleType == 4)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/RongYao.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "RongYao");
					}
					else if (SaleType == 5)
					{
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "DuiHuanType");
						item.SourceGoods.GoodImg.URL = ((xelementAttributeInt2 != 7002) ? "NetImages/GameRes/Images/Goods/8028.png" : "NetImages/GameRes/Images/Goods/8029.png");
						item.SourceGoods.ContentText.Text = ((xelementAttributeInt2 != 7002) ? Global.GetXElementAttributeInt(xelement, "TreasureXueZuan") : Global.GetXElementAttributeInt(xelement, "TreasureJiFen")).ToString();
					}
					else if (SaleType == 6)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8033.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "ZhengBaDianShu");
					}
					else if (SaleType == 7)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8034.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "LeagueNum");
					}
					else if (SaleType == 9)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8036.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "CompHonor");
					}
					else if (SaleType == 8)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8035.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "HuntNum");
					}
					else if (SaleType == 10)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8037.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "TeamRongYao");
					}
					else if (SaleType == 11)
					{
						item.SourceGoods.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/8038.png";
						item.SourceGoods.ContentText.Text = Global.GetXElementAttributeStr(xelement, "TeamPoint");
					}
					int num = Global.GetXElementAttributeInt(xelement, "DayDuiHuanNum");
					if (SaleType == 9 && type == ShiLiData.GetBaseStoreId())
					{
						num = ShiLiData.GetSelfTotalDuiHuanNum(num);
					}
					item.iDayTotalDuiHuanNum = num;
					if (num != -1)
					{
						if (this.dayDuiHuanNumDict != null)
						{
							if (this.dayDuiHuanNumDict.ContainsKey(item.itemID))
							{
								int num2 = num - this.dayDuiHuanNumDict.GetValue(item.itemID);
								num2 = ((num2 >= 0) ? num2 : 0);
								item.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), num2);
								item.iDayDuiHuanNum = num2;
							}
							else
							{
								item.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), num);
								item.iDayDuiHuanNum = num;
							}
						}
						else
						{
							item.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), num);
							item.iDayDuiHuanNum = num;
						}
					}
					else
					{
						item.iDayDuiHuanNum = -1;
					}
					this.GetIcon(Global.GetXElementAttributeStr(xelement, "NewGoods"), item.TargetGoods);
					if (Global.GetXElementAttributeStr(xelement, "NeedGoods") == string.Empty)
					{
						item.SourceGoods2.gameObject.SetActive(false);
					}
					else
					{
						item.SourceGoods2.gameObject.SetActive(true);
						item.strNeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
						this.GetIcon(Global.GetXElementAttributeStr(xelement, "NeedGoods"), item.SourceGoods2);
						if (Global.GetTotalGoodsCountByRequire(Global.GetXElementAttributeStr(xelement, "NeedGoods")) < 1)
						{
							item.SourceGoods2.GoodImg.ToGrayBitmap = true;
							item.duihuanBtn.isEnabled = false;
						}
					}
					if (SaleType == 5)
					{
						int num3 = (Global.GetXElementAttributeInt(xelement, "DuiHuanType") != 7002) ? Global.SafeConvertToInt32(this.m_TreasureXueZuanValue.text) : Global.SafeConvertToInt32(this.valueText.text);
						int num4 = Global.SafeConvertToInt32(item.SourceGoods.ContentText.Text);
						item.duihuanBtn.isEnabled = (num3 >= num4 && item.iDayDuiHuanNum > 0);
						item.SourceGoods.GoodImg.ToGrayBitmap = (num3 < num4);
					}
					else if (Global.SafeConvertToInt32(this.valueText.Text) < Global.SafeConvertToInt32(item.SourceGoods.ContentText.Text) || item.iDayDuiHuanNum == 0)
					{
						item.duihuanBtn.isEnabled = false;
					}
					if (this.ZhanMengLevel < xelementAttributeInt)
					{
						item.duihuanBtn.isEnabled = false;
						item.xianzhiText.Text = string.Format(Global.GetLang("需要战盟等级{0}"), xelementAttributeInt);
						item.xianzhiText.Label.color = NGUIMath.HexToColorEx(11141120U);
					}
					item.duihuanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						SystemHelpMgr.OnAction(UIObjIDs.MUDuiHuanPartListItemBtn, HelpStateEvents.Clicked, 1);
						this.tempDuiHuanItem = item;
						if (Global.IsBagFull())
						{
							Super.HintMainText(Global.GetLang("兑换失败，背包已满!"), 10, 3);
						}
						else
						{
							if (this.duHuanType == 0)
							{
								if (this._TypeList.SelectedIndex == 0)
								{
									Global.SendEvent("200", Global.GetLang("魔晶兑换护身符次数"));
								}
								else if (this._TypeList.SelectedIndex == 1)
								{
									Global.SendEvent("201", Global.GetLang("魔晶兑换珍贵道具次数"));
								}
								else if (this._TypeList.SelectedIndex == 2)
								{
									Global.SendEvent("202", Global.GetLang("魔晶兑换装备次数"));
								}
							}
							this.ShowModalDialog();
							this.StartCoroutine<bool>(this.SendSpriteExchange(new Action<int>(GameInstance.Game.SpriteGetExchangeMoJingAndQiFuCmd), item.itemID));
						}
					};
				}
			}
		}
	}

	private IEnumerator SendSpriteExchange(Action<int> method, int itemID)
	{
		yield return new WaitForSeconds(0.4f);
		method.Invoke(itemID);
		yield break;
	}

	private void RefreshItem(int type, int SaleType)
	{
		if (this.ItemCollectionItemList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollectionItemList.Count; i++)
		{
			DuiHuanItem duiHuanItem = U3DUtils.AS<DuiHuanItem>(this.ItemCollectionItemList[i]);
			if (null != duiHuanItem && duiHuanItem.duihuanBtn.isEnabled)
			{
				if (SaleType == 5)
				{
					int num = (type != 7002) ? Global.SafeConvertToInt32(this.m_TreasureXueZuanValue.text) : Global.SafeConvertToInt32(this.valueText.text);
					int num2 = Global.SafeConvertToInt32(duiHuanItem.SourceGoods.ContentText.Text);
					duiHuanItem.duihuanBtn.isEnabled = (num >= num2 && duiHuanItem.iDayDuiHuanNum > 0);
					duiHuanItem.SourceGoods.GoodImg.ToGrayBitmap = (num < num2);
				}
				else
				{
					duiHuanItem.SourceGoods.GoodImg.ToGrayBitmap = (Global.SafeConvertToInt32(duiHuanItem.SourceGoods.ContentText.Text) > Global.SafeConvertToInt32(this.valueText.Text));
					duiHuanItem.duihuanBtn.isEnabled = (Global.SafeConvertToInt32(duiHuanItem.SourceGoods.ContentText.Text) <= Global.SafeConvertToInt32(this.valueText.Text));
				}
				if (duiHuanItem.iDayTotalDuiHuanNum != -1)
				{
					if (this.dayDuiHuanNumDict != null)
					{
						if (this.dayDuiHuanNumDict.ContainsKey(duiHuanItem.itemID))
						{
							int num3 = duiHuanItem.iDayTotalDuiHuanNum - this.dayDuiHuanNumDict.GetValue(duiHuanItem.itemID);
							num3 = ((num3 >= 0) ? num3 : 0);
							duiHuanItem.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), num3);
							duiHuanItem.iDayDuiHuanNum = num3;
						}
						else
						{
							duiHuanItem.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), duiHuanItem.iDayTotalDuiHuanNum);
							duiHuanItem.iDayDuiHuanNum = duiHuanItem.iDayTotalDuiHuanNum;
						}
					}
					if (duiHuanItem.iDayDuiHuanNum == 0)
					{
						duiHuanItem.SourceGoods.GoodImg.ToGrayBitmap = true;
						duiHuanItem.duihuanBtn.isEnabled = false;
					}
				}
				if (duiHuanItem.strNeedGoods == string.Empty)
				{
					duiHuanItem.SourceGoods2.gameObject.SetActive(false);
				}
				else
				{
					duiHuanItem.SourceGoods2.gameObject.SetActive(true);
					if (Global.GetTotalGoodsCountByRequire(duiHuanItem.strNeedGoods) < 1)
					{
						duiHuanItem.SourceGoods2.GoodImg.ToGrayBitmap = true;
						duiHuanItem.duihuanBtn.isEnabled = false;
					}
				}
			}
		}
	}

	private GGoodIcon GetIcon(string NewGoods, GGoodIcon icon)
	{
		string[] array = NewGoods.Split(new char[]
		{
			','
		});
		int num = Global.SafeConvertToInt32(array[0]);
		int gcount = Global.SafeConvertToInt32(array[1]);
		int binding = Global.SafeConvertToInt32(array[2]);
		int forgeLevel = Global.SafeConvertToInt32(array[3]);
		int zhuijiaLevel = Global.SafeConvertToInt32(array[4]);
		int lucky = Global.SafeConvertToInt32(array[5]);
		int zhuoyueIndex = Global.SafeConvertToInt32(array[6]);
		GoodsData gd = Global.GetDummyGoodsDataMu(num, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, "NetImages/GameRes/");
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		icon.Width = 64.0;
		icon.Height = 64.0;
		icon.TipType = 1;
		icon.ItemCode = num;
		icon.ItemObject = gd;
		icon.BoxTypes = -1;
		icon.BackSpriteName0 = "bagGrid2_bak";
		icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
		};
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		return icon;
	}

	public void UpdateJifenZhi(int jifen)
	{
		this.valueText.Text = jifen.ToString();
	}

	public void UpdateZhangMengInfo(int zhangong, int zhanMengLevel)
	{
		this.valueText.Text = zhangong.ToString();
	}

	public void UpdateZhanMengLianSaiJiFenInfo()
	{
		if (this.duHuanType == 7)
		{
			this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.BHMatchGuessJiFen).ToString();
		}
		else if (this.duHuanType == 9)
		{
			this.valueText.Text = ShiLiData.GetSelfCompPoint().ToString();
		}
		else if (this.duHuanType == 10)
		{
			this.valueText.Text = Global.Data.roleData.MoneyData[160].ToString();
		}
		else if (this.duHuanType == 11)
		{
			this.valueText.Text = Global.Data.roleData.MoneyData[162].ToString();
			if (TeamCompeteDataManager.RefreshJingCaiDianCallBack != null)
			{
				TeamCompeteDataManager.RefreshJingCaiDianCallBack.Invoke();
			}
		}
	}

	public void UpDataHorsePoint()
	{
		this.valueText.Text = Global.GetRoleOwnNumByMoneyType(140).ToString();
	}

	public void UpdateMojingZhi(int result)
	{
		this.CloseModalDialog();
		if (result < 0)
		{
			if (result == -1)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMojing, null, string.Empty, string.Empty);
			}
			else if (result == -2)
			{
				Super.HintMainText(Global.GetLang("兑换失败，当前积分值不足!"), 10, 3);
			}
			else if (result == -3)
			{
				Super.HintMainText(Global.GetLang("兑换失败，兑所需背包中的物品不足！"), 10, 3);
			}
			else if (result == -4)
			{
				Super.HintMainText(Global.GetLang("兑换失败，物品扣除失败！"), 10, 3);
			}
			else if (result == -5)
			{
				Super.HintMainText(Global.GetLang("兑换失败，背包空间不足！"), 10, 3);
			}
			else if (result == -10)
			{
				Super.HintMainText(Global.GetLang("今日兑换次数已使用完！"), 10, 3);
			}
			return;
		}
		if (result > 0)
		{
			if (this.duHuanType == 0)
			{
				this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
			}
			if (this.duHuanType == 1)
			{
				GameInstance.Game.SpriteGetZaJinDanJiFen();
			}
			if (this.duHuanType == 2)
			{
				this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.PetJiFen).ToString();
			}
			if (this.duHuanType == 3)
			{
				this.valueText.Text = Global.Data.roleData.BangGong.ToString();
			}
			else if (this.duHuanType == 5)
			{
				this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TreasureJiFen).ToString();
				this.m_TreasureXueZuanValue.text = Global.GetRoleCommonUseParamsValue(33).ToString();
			}
			else if (this.duHuanType == 6)
			{
				this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhengBaPoint).ToString();
			}
			else if (this.duHuanType == 7)
			{
				this.valueText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.BHMatchGuessJiFen).ToString();
			}
			else if (this.duHuanType == 8)
			{
				this.valueText.Text = Global.GetRoleOwnNumByMoneyType(140).ToString();
			}
			else if (this.duHuanType == 9)
			{
				this.valueText.Text = ShiLiData.GetSelfCompPoint().ToString();
			}
			else if (this.duHuanType == 10)
			{
				this.valueText.Text = Global.Data.roleData.MoneyData[160].ToString();
			}
			else if (this.duHuanType == 11)
			{
				this.valueText.Text = Global.Data.roleData.MoneyData[162].ToString();
			}
			if (this.tempDuiHuanItem.iDayDuiHuanNum != -1)
			{
				int num = (this.tempDuiHuanItem.iDayDuiHuanNum - 1 >= 0) ? (this.tempDuiHuanItem.iDayDuiHuanNum - 1) : 0;
				this.tempDuiHuanItem.xianzhiText.Text = string.Format(Global.GetLang("今日可兑: {0}"), num);
				this.tempDuiHuanItem.iDayDuiHuanNum = num;
				if (this.dayDuiHuanNumDict == null)
				{
					this.dayDuiHuanNumDict = new Dictionary<int, int>();
				}
				if (this.dayDuiHuanNumDict.Count > 0)
				{
					if (this.dayDuiHuanNumDict.ContainsKey(this.tempDuiHuanItem.itemID))
					{
						this.dayDuiHuanNumDict[this.tempDuiHuanItem.itemID] = this.dayDuiHuanNumDict.GetValue(this.tempDuiHuanItem.itemID) + 1;
					}
					else
					{
						this.dayDuiHuanNumDict.Add(this.tempDuiHuanItem.itemID, 1);
					}
				}
				else
				{
					this.dayDuiHuanNumDict.Add(this.tempDuiHuanItem.itemID, 1);
				}
			}
			this.RefreshItem(this.tempitem.DuiHuanType, this.tempitem.SaleType);
			this.ShowIteminfo(this.tempitem.DuiHuanType, this.tempitem.SaleType);
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public static MUDuiHuanPart Instance;

	public ListBox _TypeList;

	public ListBox _ItemList;

	public GButton _Close;

	public UISprite title;

	public UISprite typeSprite;

	public TextBlock valueText;

	public TextBlock hintText;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public SpringPanel UIDragPl;

	public UILabel m_TreasureXueZuanValue;

	public GameObject m_TreasureXueZuanObj;

	private ObservableCollection ItemCollectionType;

	private ObservableCollection ItemCollectionItemList;

	public int duHuanType;

	private int ZhanMengLevel;

	private Dictionary<int, int> dayDuiHuanNumDict;

	public UISprite SpriteType1;

	public UISprite SpriteType2;

	public UISprite Title1;

	public UISprite Title2;

	private bool Flag = true;

	private MUDuiHuanTypeItem tempitem;

	private DuiHuanItem tempDuiHuanItem;
}
