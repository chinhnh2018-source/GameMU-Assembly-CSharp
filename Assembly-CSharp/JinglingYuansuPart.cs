using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class JinglingYuansuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnGet.Text = Global.GetLang("获取元素");
		this.BtnReset.Text = Global.GetLang("整理");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBag();
		this.TextMoneyNum.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.YuansuFenmo).ToString();
		this.RefreshUserMoney();
		this.PanelPos = this.Panel.transform.localPosition;
		this.PanelClip = this.Panel.clipRange;
		this.BtnGet.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BagTypes == YuansuBagTypes.Normal)
			{
				this.SetBagMode(YuansuBagTypes.Getting);
			}
			else if (this.BagTypes == YuansuBagTypes.Getting || this.BagTypes == YuansuBagTypes.Qianghua)
			{
				if (this.BagTypes == YuansuBagTypes.Qianghua)
				{
					this.CancelSelectIcon();
				}
				this.SetBagMode(YuansuBagTypes.Normal);
			}
		};
		this.BtnReset.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.YuansuGoodsDataList == null || Global.Data.YuansuGoodsDataList.Count <= 0)
			{
				return;
			}
			this.ShowModalDialog();
			GameInstance.Game.SpriteResetYuansuBag();
		};
		this.UIDragPanel.onDragFinished = delegate()
		{
			float num = Mathf.Round(this.Panel.clipRange.y / 78f) * 78f;
			SpringPanel.Begin(this.Panel.gameObject, new Vector3(-8f, -num, 0f), 10f);
		};
		GameInstance.Game.SpriteQueryGetYuanInfo();
	}

	public void InitPartData(YuansuBagTypes mode = YuansuBagTypes.Normal)
	{
		this.SetBagMode(mode);
	}

	protected override void OnDestroy()
	{
		if (JinglingYuansuPart.SelectedGoodsDataDict != null)
		{
			JinglingYuansuPart.SelectedGoodsDataDict.Clear();
			JinglingYuansuPart.SelectedGoodsDataDict = null;
		}
	}

	private void SetMoney(int money)
	{
		this.TextMoneyNum.Text = money.ToString();
	}

	public void RefreshUserMoney()
	{
		if (null == this.TextMoneyDiamondNum)
		{
			return;
		}
		this.TextMoneyDiamondNum.Text = Global.Data.roleData.UserMoney.ToString();
	}

	public void SetBtnGet()
	{
		if (this.BagTypes == YuansuBagTypes.Normal)
		{
			this.BtnGet.Text = Global.GetLang("获取元素");
			this.BtnGet.normalSprite = this.BtnSpriteNames[0];
			this.BtnGet.hoverSprite = this.BtnSpriteNames[0];
		}
		else if (this.BagTypes == YuansuBagTypes.Getting || this.BagTypes == YuansuBagTypes.Qianghua)
		{
			this.BtnGet.Text = Global.GetLang("返回");
			this.BtnGet.normalSprite = this.BtnSpriteNames[1];
			this.BtnGet.hoverSprite = this.BtnSpriteNames[1];
		}
		this.BtnGet.Refresh();
	}

	private void SetBagMode(YuansuBagTypes mode)
	{
		this.BagTypes = mode;
		if (mode == YuansuBagTypes.Normal)
		{
			if (null == this.jinglingYuansuPropsPart)
			{
				this.jinglingYuansuPropsPart = U3DUtils.NEW<JinglingYuansuPropsPart>();
				U3DUtils.AddChild(this.ModalPart, this.jinglingYuansuPropsPart.gameObject, true);
				this.QueryGoodsDataList(3001);
			}
			this.jinglingYuansuPropsPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				GoodsData zhuZhuangBei = e.ZhuZhuangBei;
				if (e.IDType == 2)
				{
					this.ToUseGoods(zhuZhuangBei, ModYuansuEquipTypes.EquipUnload);
				}
				else if (e.IDType == 3)
				{
					this.ToQianghua(zhuZhuangBei);
				}
			};
		}
		else if (mode == YuansuBagTypes.Getting)
		{
			if (null == this.jinglingYuansuGetPart)
			{
				this.jinglingYuansuGetPart = U3DUtils.NEW<JinglingYuansuGetPart>();
				U3DUtils.AddChild(this.ModalPart, this.jinglingYuansuGetPart.gameObject, true);
			}
			this.jinglingYuansuGetPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					GoodsData goodsData = Global.PopGoodsDataFromTempQueue();
					if (goodsData != null)
					{
						this.RefreshGoodsData(goodsData, ModYuansuEquipTypes.EquipAdd);
					}
				}
				else if (e.IDType == 2)
				{
					this.TextMoneyNum.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.YuansuFenmo).ToString();
				}
			};
		}
		else if (mode == YuansuBagTypes.Qianghua)
		{
			if (null == this.jinglingYuansuQianghuaPart)
			{
				this.jinglingYuansuQianghuaPart = U3DUtils.NEW<JinglingYuansuQianghuaPart>();
				U3DUtils.AddChild(this.ModalPart, this.jinglingYuansuQianghuaPart.gameObject, true);
				if (JinglingYuansuPart.SelectedGoodsDataDict == null)
				{
					JinglingYuansuPart.SelectedGoodsDataDict = new Dictionary<int, GoodsData>();
				}
			}
			this.jinglingYuansuQianghuaPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				GoodsQuality idtype = (GoodsQuality)e.IDType;
				bool state = Convert.ToBoolean(e.ID);
				int dbID = -1;
				if (null != this.jinglingYuansuQianghuaPart)
				{
					dbID = this.jinglingYuansuQianghuaPart.EquipDbID;
				}
				this.SelectIconByColor(idtype, state, dbID);
			};
		}
		if (null != this.jinglingYuansuPropsPart)
		{
			this.jinglingYuansuPropsPart.gameObject.SetActive(mode == YuansuBagTypes.Normal);
			if (mode == YuansuBagTypes.Normal && this.CurrentGoodData != null)
			{
				this.jinglingYuansuPropsPart.ModEquip(this.CurrentGoodData, ModYuansuEquipTypes.EquipMod);
				this.CurrentGoodData = null;
			}
		}
		if (null != this.jinglingYuansuGetPart)
		{
			this.jinglingYuansuGetPart.gameObject.SetActive(mode == YuansuBagTypes.Getting);
		}
		if (null != this.jinglingYuansuQianghuaPart)
		{
			this.jinglingYuansuQianghuaPart.gameObject.SetActive(mode == YuansuBagTypes.Qianghua);
		}
		this.SetBtnGet();
	}

	private void InitBag()
	{
		if (this.BagOrient == BagOrientTypes.Vertical && !this.IsPage)
		{
			this.BakTrans.localScale = new Vector3(390f, 1560f, 0f);
			this.BakTrans.transform.localPosition = new Vector3(-39f, -741f, 0f);
			this.UIDragPanel.dragEffect = 2;
			this.UIDragPanel.momentumAmount = 35f;
			this.UIDragPanel.scale = Vector3.up;
		}
		else if (this.BagOrient == BagOrientTypes.Horizontal && this.IsPage)
		{
			this.BakTrans.localScale = new Vector3(0f, 1560f, 0f);
			this.BakTrans.transform.localPosition = new Vector3(-39f, -741f, 0f);
			this.UIDragPanel.dragEffect = 0;
			this.UIDragPanel.momentumAmount = 0f;
			this.UIDragPanel.scale = Vector3.right;
		}
		Global.YuansuBagMaxGridCount = 100;
		this.GoodsBox.RowCount = 20;
		this.GoodsBox.ColCount = 5;
		this.GoodsBox.InitBox();
		this.QueryGoodsDataList(3000);
	}

	public void RefreshGoodsData(GoodsData gd, ModYuansuEquipTypes modType)
	{
		this.CloseModalDialog();
		if (gd == null)
		{
			return;
		}
		if (modType == ModYuansuEquipTypes.EquipLoad)
		{
			int num = this.GoodsBox.FindByGoodsDbID(gd.Id);
			if (gd.GCount <= 0 || gd.Site != 3000)
			{
				if (num != -1)
				{
					this.GoodsBox.RemoveGoodsIcon(num);
					Global.AddYuansuGoodsData(gd, true);
					Global.RemoveYuansuGoodsData(gd, false);
				}
				return;
			}
		}
		else if (modType == ModYuansuEquipTypes.EquipUnload)
		{
			GGoodIcon ggoodIcon = this.AddIcon(gd, false);
			this.GoodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			Global.AddYuansuGoodsData(gd, false);
			Global.RemoveYuansuGoodsData(gd, true);
		}
		else if (modType == ModYuansuEquipTypes.EquipAdd)
		{
			GGoodIcon ggoodIcon2 = this.AddIcon(gd, false);
			this.GoodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon2);
			ggoodIcon2.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
		}
		else if (modType == ModYuansuEquipTypes.EquipRemove)
		{
			if (gd.GCount > 0)
			{
				if (gd.Site == 3000)
				{
					int num2 = this.GoodsBox.FindByGoodsDbID(gd.Id);
					Global.RemoveYuansuGoodsData(gd, false);
					this.GoodsBox.RemoveGoodsIcon(num2);
				}
				else if (gd.Site == 3001)
				{
					Global.RemoveYuansuGoodsData(gd, true);
				}
			}
		}
		else if (modType == ModYuansuEquipTypes.EquipMod)
		{
			if (gd.Site == 3000)
			{
				GGoodIcon ggoodIcon3 = this.AddIcon(gd, true);
				this.GoodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon3);
				ggoodIcon3.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
				Global.ReplaceYuansuGoodsData(gd, null, false);
				if (this.BagTypes == YuansuBagTypes.Qianghua)
				{
					this.CurrentGoodIcon = ggoodIcon3;
				}
			}
			else if (gd.Site == 3001)
			{
				Global.ReplaceYuansuGoodsData(gd, null, true);
			}
		}
	}

	public void RefreshGoodsDataList(List<GoodsData> tmpGoodsDataList, bool resort = false, bool isStep = false, RefreshYuansuBagTypes type = RefreshYuansuBagTypes.None)
	{
		this.CloseModalDialog();
		if (tmpGoodsDataList == null && (type == RefreshYuansuBagTypes.Reset || type == RefreshYuansuBagTypes.Body))
		{
			return;
		}
		if (type == RefreshYuansuBagTypes.Body && null != this.jinglingYuansuPropsPart)
		{
			this.jinglingYuansuPropsPart.InitEquipsByUsed(tmpGoodsDataList);
			Global.Data.YuansuGoodsDataListByUseing = tmpGoodsDataList;
			return;
		}
		Global.Data.YuansuGoodsDataList = tmpGoodsDataList;
		if (tmpGoodsDataList != null && resort)
		{
			tmpGoodsDataList.Sort(new Comparison<GoodsData>(this.SortGoodsDataList));
		}
		base.StartCoroutine<bool>(this.ShowPage(this.CurrentSelectedPage, isStep));
	}

	private IEnumerator ShowPage(int pageIndex, bool isStep = false)
	{
		this.MaxPageCount = Global.GetJinDanBagCapacity() / 100;
		Dictionary<int, int> indexDict = new Dictionary<int, int>();
		if (Global.Data.YuansuGoodsDataList != null)
		{
			int count = Global.Data.YuansuGoodsDataList.Count;
			for (int subIndex = 0; subIndex < count; subIndex++)
			{
				GoodsData gd = Global.Data.YuansuGoodsDataList[subIndex];
				if (gd != null)
				{
					if (indexDict.ContainsKey(gd.BagIndex))
					{
						indexDict[gd.BagIndex] = subIndex;
					}
					else
					{
						indexDict.Add(gd.BagIndex, subIndex);
					}
				}
			}
		}
		int counter = 0;
		int refreshCount = 5;
		if (!isStep)
		{
			refreshCount = 20;
		}
		for (int i = 0; i < 100; i++)
		{
			counter++;
			int dataIndex = -1;
			if (indexDict.TryGetValue(i, ref dataIndex))
			{
				GoodsData gd2 = Global.Data.YuansuGoodsDataList[dataIndex];
				GGoodIcon icon = this.AddIcon(gd2, false);
				this.GoodsBox.SetGoodsIcon(this.Getindex(i), icon);
				icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
				bool canUseGoods = Global.CanUseGoods(gd2.GoodsID, false, true);
				Global.SetEquipGoodsZhanLiStat(icon, gd2);
			}
			else
			{
				this.GoodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
			if (counter % refreshCount == 0)
			{
				yield return null;
			}
		}
		yield return null;
		yield break;
	}

	private void RefreshBagPageText()
	{
	}

	private GGoodIcon AddIcon(GoodsData goodsData, bool isDoing = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = 15;
			icon.gameObject.AddComponent<UIDragPanelContents>();
			if (isDoing)
			{
				icon.BackSpriteName2 = "iconState_qianghua";
			}
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 1)
				{
					this.ToUseGoods(icon.ItemObject as GoodsData, ModYuansuEquipTypes.EquipLoad);
				}
				else if (ev.IDType == 2)
				{
					this.ToUseGoods(icon.ItemObject as GoodsData, ModYuansuEquipTypes.EquipUnload);
				}
				else if (ev.IDType == 3)
				{
					this.ToQianghua(icon);
				}
			};
			Super.InitYuansuGoodsGIcon(icon, goodsData);
			return icon;
		}
		return null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPartQuHui, HelpStateEvents.None, -1);
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (this.BagTypes == YuansuBagTypes.Qianghua)
		{
			int dbID = -1;
			if (null != this.jinglingYuansuQianghuaPart)
			{
				dbID = this.jinglingYuansuQianghuaPart.EquipDbID;
			}
			this.SelectIcon(ggoodIcon, dbID);
		}
		else
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.YuansuBagTip, GoodsOwnerTypes.YuansuBag, goodData);
		}
	}

	private int SortGoodsDataList(GoodsData x, GoodsData y)
	{
		return x.BagIndex - y.BagIndex;
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.BagOrient == BagOrientTypes.Vertical && !this.IsPage)
		{
			result = bagIndex;
		}
		else if (this.BagOrient == BagOrientTypes.Horizontal && this.IsPage)
		{
			int num = 5;
			int num2 = 20;
			int num3 = 0;
			this.GoodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	private void SelectIcon(GGoodIcon icon, int dbID = -1)
	{
		if (icon != null)
		{
			GoodsData goodsData = icon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			int id = goodsData.Id;
			if (dbID > 0 && dbID == id)
			{
				icon.BackSpriteName2 = "iconState_qianghua";
				return;
			}
			bool state = false;
			if (JinglingYuansuPart.SelectedGoodsDataDict.ContainsKey(id))
			{
				state = false;
				this.SelectIconExe(state, icon);
			}
			else
			{
				GoodsQuality goodsQuality = (GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID);
				if (goodsQuality >= GoodsQuality.Purple && Super.MessageBoxIsHint[3] == 0)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("被吞噬的元素之心中有{ff00ff}紫色{-}、{ffaa00}橙色{-}品质，确定要吞噬？"), 2, null, MessBoxIsHintTypes.YuansuQianghuaSelectNeedHint);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							state = true;
							this.SelectIconExe(state, icon);
						}
						return true;
					};
				}
				else
				{
					state = true;
					this.SelectIconExe(state, icon);
				}
			}
		}
	}

	private void SelectIconExe(bool state, GGoodIcon icon)
	{
		if (null == icon || JinglingYuansuPart.SelectedGoodsDataDict == null)
		{
			return;
		}
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		int id = goodsData.Id;
		if (!state)
		{
			JinglingYuansuPart.SelectedGoodsDataDict.Remove(id);
			icon.BackSpriteName2 = string.Empty;
			icon.BackgroundSprite2.gameObject.SetActive(false);
		}
		else
		{
			JinglingYuansuPart.SelectedGoodsDataDict.Add(id, goodsData);
			icon.BackSpriteName2 = "iconState_selected";
		}
		this.AfterSelectedIcon();
	}

	private void SelectIconByColor(GoodsQuality goodColor, bool state, int dbID = -1)
	{
		ObservableCollection itemsSource = this.GoodsBox.listBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(itemsSource[i]);
			if (null != ggoodIcon)
			{
				GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
				int id = goodsData.Id;
				if (dbID > 0 && dbID == id)
				{
					ggoodIcon.BackSpriteName2 = "iconState_qianghua";
				}
				else
				{
					GoodsQuality goodsQuality = (GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID);
					if (goodsQuality == goodColor)
					{
						if (state)
						{
							ggoodIcon.BackSpriteName2 = "iconState_selected";
							if (!JinglingYuansuPart.SelectedGoodsDataDict.ContainsKey(id))
							{
								JinglingYuansuPart.SelectedGoodsDataDict.Add(id, goodsData);
							}
						}
						else
						{
							ggoodIcon.BackSpriteName2 = string.Empty;
							ggoodIcon.BackgroundSprite2.gameObject.SetActive(false);
							if (JinglingYuansuPart.SelectedGoodsDataDict.ContainsKey(id))
							{
								JinglingYuansuPart.SelectedGoodsDataDict.Remove(id);
							}
						}
					}
				}
			}
		}
		this.AfterSelectedIcon();
	}

	public void CancelSelectIcon()
	{
		if (null != this.CurrentGoodIcon)
		{
			this.CurrentGoodIcon.BackSpriteName2 = string.Empty;
			this.CurrentGoodIcon.BackgroundSprite2.gameObject.SetActive(false);
		}
		if (JinglingYuansuPart.SelectedGoodsDataDict == null || JinglingYuansuPart.SelectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.GoodsBox.listBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(itemsSource[i]);
			if (null != ggoodIcon)
			{
				ggoodIcon.BackSpriteName2 = string.Empty;
				ggoodIcon.BackgroundSprite2.gameObject.SetActive(false);
			}
		}
		JinglingYuansuPart.SelectedGoodsDataDict.Clear();
		this.AfterSelectedIcon();
	}

	private void AfterSelectedIcon()
	{
		if (null != this.jinglingYuansuQianghuaPart)
		{
			this.jinglingYuansuQianghuaPart.RefreshPropsAndExp2();
		}
	}

	public void RemoveSelectIcon()
	{
		if (JinglingYuansuPart.SelectedGoodsDataDict == null || JinglingYuansuPart.SelectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		foreach (GoodsData gd in JinglingYuansuPart.SelectedGoodsDataDict.Values)
		{
			this.RefreshGoodsData(gd, ModYuansuEquipTypes.EquipRemove);
		}
		JinglingYuansuPart.SelectedGoodsDataDict.Clear();
	}

	private void ToUseGoods(GoodsData goodsData, ModYuansuEquipTypes modType)
	{
		if (goodsData == null)
		{
			return;
		}
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 810)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("无法佩戴"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (modType == ModYuansuEquipTypes.EquipLoad)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			if (JinglingYuansuPropsPart.UsedGoodsTypeDict.ContainsKey(goodsCatetoriy))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("相同属性的元素之心只能佩戴1个"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (JinglingYuansuPropsPart.UsedGoodsTypeDict.Count >= JinglingYuansuPropsPart.MaxEquipNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("元素之心佩戴已满"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteModYuansuEquip(goodsData.Id, (int)modType);
		}
		else if (modType == ModYuansuEquipTypes.EquipUnload)
		{
			if (Global.GetYuansuBagEmptyGridCount() < 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("元素背包至少需要1个空格子"), new object[0]), 0, -1, -1, 0);
				return;
			}
			this.ShowModalDialog();
			GameInstance.Game.SpriteModYuansuEquip(goodsData.Id, (int)modType);
		}
	}

	private void ToQianghua(GGoodIcon icon)
	{
		if (this.BagTypes != YuansuBagTypes.Qianghua)
		{
			this.SetBagMode(YuansuBagTypes.Qianghua);
		}
		if (null != icon && null != this.jinglingYuansuQianghuaPart)
		{
			this.CurrentGoodIcon = icon;
			icon.BackSpriteName2 = "iconState_qianghua";
			this.jinglingYuansuQianghuaPart.AddEquip(icon.ItemObject as GoodsData);
		}
	}

	private void ToQianghua(GoodsData goodsData)
	{
		if (this.BagTypes != YuansuBagTypes.Qianghua)
		{
			this.SetBagMode(YuansuBagTypes.Qianghua);
		}
		if (goodsData != null && null != this.jinglingYuansuQianghuaPart)
		{
			this.CurrentGoodData = goodsData;
			this.jinglingYuansuQianghuaPart.AddEquip(goodsData);
		}
	}

	private void QueryGoodsDataList(int site)
	{
		this.ShowModalDialog();
		if (site == 3000)
		{
			GameInstance.Game.SpritQueryYuansuBag(site);
		}
		else if (site == 3001)
		{
			GameInstance.Game.SpritQueryYuansuBagByUsed(site);
		}
	}

	public void NotifyToUseGoodsResult(int result, GoodsData gd, ModYuansuEquipTypes modType)
	{
		this.CloseModalDialog();
		if (result == 0)
		{
			this.RefreshGoodsData(gd, modType);
			if (null != this.jinglingYuansuPropsPart)
			{
				this.jinglingYuansuPropsPart.ModEquip(gd, modType);
			}
		}
		else if (result == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("道具不存在"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备栏已满"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == 7)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包格子不足"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void NotifyQianghuaResult(int result, GoodsData gd, ModYuansuEquipTypes modType)
	{
		this.CloseModalDialog();
		if (result == 0)
		{
			if (gd == null)
			{
				return;
			}
			this.RefreshGoodsData(gd, modType);
			this.RemoveSelectIcon();
			if (null != this.jinglingYuansuQianghuaPart)
			{
				this.jinglingYuansuQianghuaPart.NotifyResult(result, gd);
			}
		}
		else if (result == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("道具不存在"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == 11)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedYuansuFenmo, null, string.Empty, Global.GetLang("元素粉末"));
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("强化时发生错误,错误码{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void NotifyResetBag()
	{
		if (this.BagTypes == YuansuBagTypes.Getting || this.BagTypes == YuansuBagTypes.Qianghua)
		{
			if (this.BagTypes == YuansuBagTypes.Qianghua)
			{
				this.CancelSelectIcon();
			}
			this.SetBagMode(YuansuBagTypes.Normal);
		}
		this.SpringPanelByBag.enabled = false;
		this.Panel.transform.localPosition = this.PanelPos;
		this.Panel.clipRange = this.PanelClip;
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	private const int MaxGridCount = 100;

	private const int RowsInPage = 20;

	private const int ColumnsInPage = 5;

	private const int Columns = 0;

	private const int BagSizeAPage = 100;

	private const int AGridSize = 78;

	public GButton BtnGet;

	public GButton BtnReset;

	public TextBlock TextMoneyNum;

	public TextBlock TextMoneyDiamondNum;

	public GameObject ModalPart;

	[HideInInspector]
	public JinglingYuansuPropsPart jinglingYuansuPropsPart;

	[HideInInspector]
	public JinglingYuansuGetPart jinglingYuansuGetPart;

	[HideInInspector]
	public JinglingYuansuQianghuaPart jinglingYuansuQianghuaPart;

	private YuansuBagTypes BagTypes;

	public static Dictionary<int, GoodsData> SelectedGoodsDataDict;

	private GGoodIcon CurrentGoodIcon;

	private GoodsData CurrentGoodData;

	public GGoodsBox GoodsBox;

	public Transform BakTrans;

	public UIPanel Panel;

	public UIDraggablePanel UIDragPanel;

	public UISprite[] Pages;

	public SpringPanel SpringPanelByBag;

	private Vector3 PanelPos = Vector3.zero;

	private Vector4 PanelClip = Vector4.zero;

	private BagOrientTypes BagOrient = BagOrientTypes.Vertical;

	private bool IsPage;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private string[] BtnSpriteNames = new string[]
	{
		"button2",
		"light"
	};
}
