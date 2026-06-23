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

public class JingLingCangKuPart : UserControl
{
	public BaoGuoMode BaoGuoMode
	{
		get
		{
			return this.mBaoGuoMode;
		}
		set
		{
			this.mBaoGuoMode = value;
			if (this.mBaoGuoMode == BaoGuoMode.horseCangKu)
			{
				this.mTitleSp.spriteName = "zuoQiCangKu";
			}
			else
			{
				this.mTitleSp.spriteName = "jinglingChangku";
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.CaozuoBtn.Text = Global.GetLang("批量提取");
		this.ZhengliBtn.Text = Global.GetLang("整 理");
		this.huiShouBtn.Text = Global.GetLang("一键回收");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.UIDragPl.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
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
		this.CaozuoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isPiliang)
			{
				this.SetPiliangCaozuo(false);
			}
			else
			{
				this.SetPiliangCaozuo(true);
			}
		};
		this.ZhengliBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
			{
				GameInstance.Game.SpriteResetJingLingBag();
			}
			else
			{
				GameInstance.Game.SendRoleRideCangKuZhengLi();
			}
		};
		this.huiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("是否确定回收仓库内所有蓝色精灵？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						string goodslist = string.Empty;
						if (this.GoodsDataList == null)
						{
							return true;
						}
						bool flag = false;
						for (int i = 0; i < this.GoodsDataList.Count; i++)
						{
							if (this.GoodsDataList[i].ElementhrtsProps != null)
							{
								for (int j = 0; j < this.GoodsDataList[i].ElementhrtsProps.Count; j++)
								{
									if (j % 3 == 2 && this.GoodsDataList[i].ElementhrtsProps[j] != 0)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (!flag)
						{
							GoodVO goodV0;
							for (int k = 0; k < this.GoodsDataList.Count; k++)
							{
								goodV0 = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsDataList[k].GoodsID);
								if (goodV0.SuitID == 1 && this.GoodsDataList[k].ExcellenceInfo == 0)
								{
									goodslist += this.GoodsDataList[k].Id;
									goodslist += ",";
								}
							}
							GameInstance.Game.SpriteOneKeyQuickSaleOut(4, goodslist);
							GameInstance.Game.SpriteResetJingLingBag();
						}
						else
						{
							string[] awardDescribe = new string[]
							{
								Global.GetLang("精灵本体回收"),
								Global.GetLang("精灵等级回收"),
								Global.GetLang("精灵技能回收")
							};
							int[] jingLingSRecoverAward = Global.GetJingLingSRecoverAward(this.GoodsDataList);
							GoodVO goodV0;
							Super.ShowJingLingHuiShouMessageBox(Global.GetLang("精灵回收"), awardDescribe, jingLingSRecoverAward, delegate(object x, DPSelectedItemEventArgs c)
							{
								for (int l = 0; l < this.GoodsDataList.Count; l++)
								{
									goodV0 = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsDataList[l].GoodsID);
									if (goodV0.SuitID == 1 && this.GoodsDataList[l].ExcellenceInfo == 0)
									{
										goodslist += this.GoodsDataList[l].Id;
										goodslist += ",";
									}
								}
								GameInstance.Game.SpriteOneKeyQuickSaleOut(4, goodslist);
								GameInstance.Game.SpriteResetJingLingBag();
								return true;
							}, null);
						}
					}
					return true;
				};
			}
			else if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("是否确定回收仓库内所有蓝色坐骑？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						string text = string.Empty;
						if (this.GoodsDataList == null)
						{
							return true;
						}
						for (int i = 0; i < this.GoodsDataList.Count; i++)
						{
							int categoriyByGoodsID = Global.GetCategoriyByGoodsID(this.GoodsDataList[i].GoodsID);
							if (categoriyByGoodsID == 340 || (40 <= categoriyByGoodsID && 45 >= categoriyByGoodsID))
							{
								if ((long)this.GoodsDataList[i].WashProps.Count < ConfigSystemParam.GetSystemParamIntByName("HorseRecoveryLimit") * 2L)
								{
									int horseQuality = Super.GetHorseQuality(this.GoodsDataList[i]);
									if (horseQuality < 3)
									{
										text += this.GoodsDataList[i].Id;
										text += ",";
									}
								}
							}
						}
						if (!string.IsNullOrEmpty(text))
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(6, text);
							GameInstance.Game.SendRoleRideCangKuZhengLi();
						}
					}
					return true;
				};
			}
		};
	}

	private void SetPiliangCaozuo(bool bl)
	{
		this.isPiliang = bl;
		this.selectImage.Visibility = bl;
		this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
		Super._ParcelPart.isShowTips = !bl;
		if (bl)
		{
			this.CaozuoBtn.Text = Global.GetLang("取消操作");
		}
		else
		{
			this.CaozuoBtn.Text = Global.GetLang("批量操作");
		}
	}

	private void onDragFinished()
	{
		if (Math.Abs(Math.Abs(this.UIDragPl.transform.localPosition.x) - (float)(468 * this.CurrentSelectedPage)) > 30f)
		{
			if (SystemHelpPart.IsMaskShowing())
			{
				return;
			}
			if (this.UIDragPl.transform.localPosition.x > (float)(-468 * this.CurrentSelectedPage))
			{
				this.CurrentSelectedPage--;
				if (this.CurrentSelectedPage <= 0)
				{
					this.CurrentSelectedPage = 0;
				}
			}
			else
			{
				this.CurrentSelectedPage++;
				if (this.CurrentSelectedPage >= 10)
				{
					this.CurrentSelectedPage = 9;
				}
			}
		}
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPageText();
	}

	public void CleanUpChildWindows()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		Global.InitBagParams();
	}

	public void ResetGetNewData()
	{
	}

	public IEnumerator GetNewData()
	{
		this.goodsBox.RowCount = 4;
		this.goodsBox.ColCount = this.BagSizeAPage / 4;
		this.goodsBox.InitBox();
		yield return new WaitForSeconds(0.02f);
		if (Super._ParcelPart == null)
		{
			ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
			parcelPart.iBaoGuoMode = (int)this.BaoGuoMode;
			Super._ParcelPart = parcelPart;
		}
		Super._ParcelPart.iBaoGuoMode = (int)this.BaoGuoMode;
		Super._ParcelPart.Visibility = true;
		this.baoGuoCanvas.Children.Add(Super._ParcelPart);
		yield return new WaitForSeconds(0.02f);
		Super._ParcelPart.InitPartData();
		this.SetPiliangCaozuo(true);
		yield return new WaitForSeconds(0.1f);
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
		{
			GameInstance.Game.SpriteGetJingLingGoodsDataList();
		}
		else
		{
			List<GoodsData> lst = Global.Data.roleData.MountStoreList;
			if (lst != null && 0 < lst.Count)
			{
				this.RefreshGoodsDataList(lst, false, false);
			}
		}
		yield break;
	}

	public void RefreshGoodsUsing()
	{
	}

	public void RefreshGoodsDataList(List<GoodsData> tmpGoodsDataList, bool resort = false, bool isStep = false)
	{
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu && Global.Data.IsDoingJingLingZhaoHuan)
		{
			return;
		}
		if (tmpGoodsDataList != null && resort)
		{
			tmpGoodsDataList.Sort(new Comparison<GoodsData>(this.goodsDataList_Sort));
		}
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
		{
			SystemHelpPart.HideMask();
			Global.Data.JingLingGoodsDataList = tmpGoodsDataList;
		}
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
		{
			List<GoodsData> list = this.FindAppendNewGoodsArr();
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					GoodsData gd = list[i];
					this.RefreshJingLingBag(gd);
				}
			}
			else
			{
				GoodsData gd2 = null;
				this.RefreshJingLingBag(gd2);
			}
		}
		else
		{
			GoodsData gd3 = null;
			this.RefreshJingLingBag(gd3);
		}
		if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
		{
			this.GoodsDataList = tmpGoodsDataList;
		}
		else
		{
			this.GoodsDataList = Global.Data.JingLingGoodsDataList;
		}
		base.StartCoroutine<bool>(this.ShowPageEx(this.CurrentSelectedPage, isStep));
		if (tmpGoodsDataList == null || tmpGoodsDataList.Count <= 0 || this.ForceShowNavi)
		{
			this.ForceShowNavi = false;
		}
		this.OldGoodsArrayList.Clear();
		if (this.GoodsDataList != null)
		{
			for (int j = 0; j < this.GoodsDataList.Count; j++)
			{
				this.OldGoodsArrayList.Add(this.GoodsDataList[j]);
			}
		}
	}

	public void ResetJingLingbag()
	{
		List<GoodsData> thisPartGoodsList = this.GetThisPartGoodsList();
		if (thisPartGoodsList != null)
		{
			for (int i = thisPartGoodsList.Count; i < this.BagSizeAPage; i++)
			{
				this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
		}
	}

	private List<GoodsData> GetThisPartGoodsList()
	{
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
		{
			return Global.Data.JingLingGoodsDataList;
		}
		if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
		{
			return Global.Data.roleData.MountStoreList;
		}
		return null;
	}

	protected List<GoodsData> FindAppendNewGoodsArr()
	{
		if (this.OldGoodsArrayList == null || this.OldGoodsArrayList.Count == 0)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> thisPartGoodsList = this.GetThisPartGoodsList();
		if (thisPartGoodsList != null)
		{
			for (int i = 0; i < thisPartGoodsList.Count; i++)
			{
				GoodsData goodsData = thisPartGoodsList[i];
				if (!this.ExisitGoods(goodsData))
				{
					list.Add(goodsData);
				}
			}
		}
		return list;
	}

	protected bool ExisitGoods(GoodsData obj)
	{
		if (this.OldGoodsArrayList == null)
		{
			return false;
		}
		for (int i = 0; i < this.OldGoodsArrayList.Count; i++)
		{
			GoodsData goodsData = this.OldGoodsArrayList[i];
			if (goodsData.Id == obj.Id && goodsData.GoodsID == obj.GoodsID)
			{
				return true;
			}
		}
		return false;
	}

	public void RefreshJingLingBag(GoodsData gd = null)
	{
		if (gd == null)
		{
			if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
			{
				if (this.GoodsDataList != null)
				{
					this.SortGoodsBagIndex();
				}
			}
			else if (Global.Data.JingLingGoodsDataList != null)
			{
				this.SortGoodsBagIndex();
			}
		}
		else
		{
			this.TryToModifyBagIndex(gd);
			GGoodIcon ggoodIcon = this.AddIcon(gd);
			this.goodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			Global.SetEquipGoodsZhanLiStat(ggoodIcon, gd);
		}
	}

	private int goodsDataList_Sort(GoodsData x, GoodsData y)
	{
		return x.BagIndex - y.BagIndex;
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.GoodsDataList = null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (this.selectImage.Visibility)
		{
			this.rgoodsDBid = (ggoodIcon.ItemObject as GoodsData).BagIndex;
			Global.MoveGoodsToParcel(ggoodIcon.ItemObject as GoodsData, 0);
			if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu && this.GetGoodsIconIndex(ggoodIcon.ItemObject as GoodsData) == 0)
			{
				SystemHelpMgr.OnAction(UIObjIDs.JinglingCangKuDetailPart, HelpStateEvents.Inactived, -1);
			}
		}
		else
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
			{
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.ZuoQiCangKu, goodData);
			}
			else
			{
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfPet, goodData);
			}
		}
	}

	private void ShowDisableGrid(int pageIndex)
	{
		for (int i = 0; i < this.DisableImagesList.Count; i++)
		{
			Image image = this.DisableImagesList[i];
			image.removeEventListener("click", new MouseEventHandler(this.OnClickDisableGrid));
			this.Container.Children.Remove(this.DisableImagesList[i], true);
		}
		this.DisableImagesList.Clear();
		int disableGridStartIndexOfCurrentPage = this.GetDisableGridStartIndexOfCurrentPage(pageIndex);
		for (int j = disableGridStartIndexOfCurrentPage; j < this.BagSizeAPage; j++)
		{
		}
	}

	private int GetDisableGridStartIndexOfCurrentPage(int pageIndex)
	{
		int result = 0;
		if (this.MaxPageCount > 0)
		{
			int num = Global.GetJinDanBagCapacity() - pageIndex * this.BagSizeAPage;
			if (num < 0)
			{
				result = 0;
			}
			else if (num <= this.BagSizeAPage)
			{
				result = num;
			}
			else
			{
				result = this.BagSizeAPage;
			}
		}
		return result;
	}

	private void OnClickDisableGrid(MouseEvent evt)
	{
		Image image = evt.currentTarget as Image;
		if (null == image)
		{
			return;
		}
		int num = this.DisableImagesList.IndexOf(image);
		if (num < 0)
		{
			return;
		}
		int num2 = num + 1;
		int num3 = num2 * Global.OneJinDanGridYuanBao;
		string message = StringUtil.substitute(Global.GetLang("您确定消耗{0}钻石开启{1}个背包栏位吗?"), new object[]
		{
			num3,
			num2
		});
		GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("扩展格子"), message, Super.GetChildLeft(630, 316), Super.GetChildTop(321, 128), 630, 321, 0.01, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
			}
			return true;
		};
	}

	public void RemoveGoodsEX(int BgIndex)
	{
		if (this.GoodsDataList == null)
		{
			return;
		}
		this.goodsBox.RemoveGoodsIcon(this.Getindex(BgIndex));
		this.BagCapacity.Text = string.Format(Global.GetLang("{{00FF00}}容量：{{-}}{0}/{1}"), (this.GoodsDataList != null) ? this.GoodsDataList.Count : 0, Global.GetJinDanBagCapacity());
	}

	public void RemoveGoods(int goodsDbID)
	{
		if (this.GoodsDataList == null)
		{
			return;
		}
		this.goodsBox.RemoveGoodsIcon(this.Getindex(this.rgoodsDBid));
		this.BagCapacity.Text = string.Format(Global.GetLang("{{00FF00}}容量：{{-}}{0}/{1}"), (this.GoodsDataList != null) ? this.GoodsDataList.Count : 0, Global.GetJinDanBagCapacity());
	}

	public void RemoveGoods(GoodsData gd)
	{
		if (gd == null || gd.GCount < 1)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		string title = goodsXmlNodeByID.Title;
		if (gd.Binding > 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("绑定的物品不可丢弃, 建议您将闲置的物品村存放到龙城【仓库管理员】处.或直接【摧毁】"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int goodsDataPrice = Global.GetGoodsDataPrice(gd, true);
		GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("确认要售出{0}:【{1}】？\n \n售出后你将得到：{2} 绑定金币"), new object[]
		{
			(gd.GCount <= 1) ? string.Empty : Global.GetLang("这些"),
			title,
			goodsDataPrice
		}), (int)((this.Width - 253.0) / 2.0), (int)((this.Height - 171.0) / 2.0), (int)this.Width, (int)this.Height, 0.01, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SpriteModGoods(0, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
			}
			return true;
		};
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon icon = Global.GetNewGoodIcon();
			icon.Width = 80.0;
			icon.Height = 80.0;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = 14;
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 6)
				{
					this.rgoodsDBid = (icon.ItemObject as GoodsData).BagIndex;
					Global.MoveGoodsToParcel(icon.ItemObject as GoodsData, 0);
				}
			};
			UIEventListener.Get(icon.gameObject).onDrag = delegate(GameObject go, Vector2 delta)
			{
				this.deltaX += delta.x;
			};
			return icon;
		}
		return null;
	}

	public IEnumerator ShowPageEx(int pageIndex, bool isStep = false)
	{
		this.MaxPageCount = Global.GetJinDanBagCapacity() / this.BagSizeAPage;
		List<GoodsData> lst = this.GetThisPartGoodsList();
		Dictionary<int, int> indexDict = new Dictionary<int, int>();
		if (lst != null)
		{
			int count = lst.Count;
			for (int subIndex = 0; subIndex < count; subIndex++)
			{
				GoodsData gd = lst[subIndex];
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
		int refreshCount = 6;
		if (!isStep)
		{
			refreshCount = 24;
		}
		for (int i = 0; i < this.BagSizeAPage; i++)
		{
			counter++;
			int dataIndex = -1;
			if (indexDict.TryGetValue(i, ref dataIndex))
			{
				if (dataIndex < lst.Count)
				{
					GoodsData gd2 = lst[dataIndex];
					GGoodIcon icon = this.AddIcon(gd2);
					this.goodsBox.SetGoodsIcon(this.Getindex(i), icon);
					icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
					bool canUseGoods = Global.CanUseGoods(gd2.GoodsID, false, true);
					Super.InitGoodsGIcon(icon, gd2, canUseGoods, IconTextTypes.Qianghua);
					Global.SetEquipGoodsZhanLiStat(icon, gd2);
				}
				else
				{
					this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
				}
			}
			else
			{
				this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
			if (counter % refreshCount == 0)
			{
				yield return null;
			}
		}
		yield return null;
		if (Global.GetJinDanBagCapacity() <= 0)
		{
			this.Page.Text = "0/0";
		}
		this.RefreshBagPageText();
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		if (this.BaoGuoMode == BaoGuoMode.JingLingCangKu)
		{
			SystemHelpMgr.OnAction(UIObjIDs.JinglingCangKuPart, HelpStateEvents.Actived, 1);
		}
		else
		{
			SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPart, HelpStateEvents.Actived, 1);
		}
		yield break;
	}

	protected void RefreshBagPageText()
	{
		if (this.tempPaneStat != null)
		{
			this.tempPaneStat.spriteName = "selectState_normal2";
		}
		this.Pages[this.CurrentSelectedPage].spriteName = "selectState_hover2";
		this.tempPaneStat = this.Pages[this.CurrentSelectedPage];
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int dex)
	{
		int num = this.BagSizeAPage / 4;
		this.goodsBox.listBox.maxPerLine = num;
		int num2 = dex / 6 / 4;
		int num3 = dex % 24;
		int num4 = num3 % 6;
		int num5 = num3 / 6 % 4;
		return num4 + num5 * num + num2 * 6;
	}

	private void ExchangeGoodsDataPosition(GoodsData gd1, GoodsData gd2, int toIndex)
	{
		int num = this.goodsBox.FindByGoodsDbID(gd1.Id);
		if (num >= 0)
		{
			gd2.BagIndex = num;
			gd1.BagIndex = toIndex;
		}
	}

	private void MoveGoodsDataPosition(GoodsData gd1, int toIndex)
	{
		int num = this.goodsBox.FindByGoodsDbID(gd1.Id);
		if (num >= 0)
		{
		}
	}

	protected bool CanShowGoodsInPacel(GoodsData gd)
	{
		return gd != null && gd.Using <= 0 && gd.GCount > 0;
	}

	protected void SortGoodsBagIndex()
	{
		List<GoodsData> thisPartGoodsList = this.GetThisPartGoodsList();
		thisPartGoodsList.Sort(delegate(GoodsData x, GoodsData y)
		{
			if (x.BagIndex != y.BagIndex)
			{
				return x.BagIndex - y.BagIndex;
			}
			if (x.GoodsID != y.GoodsID)
			{
				return x.GoodsID - y.GoodsID;
			}
			return x.GCount - y.GCount;
		});
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> list2 = new List<GoodsData>();
		List<GoodsData> list3 = new List<GoodsData>();
		for (int i = 0; i < thisPartGoodsList.Count; i++)
		{
			GoodsData goodsData = thisPartGoodsList[i];
			if (!this.CanShowGoodsInPacel(goodsData))
			{
				list3.Add(goodsData);
			}
			else if (!this.ExistBagIndex(list, goodsData.BagIndex))
			{
				list.Add(goodsData);
			}
			else
			{
				list2.Add(goodsData);
			}
		}
		int j;
		for (j = 0; j < list2.Count; j++)
		{
			GoodsData goodsData2 = list2[j];
			int bagIndex = goodsData2.BagIndex;
			int num = this.GenerateNewBagIndex(list, 0);
			if (num < 0)
			{
				break;
			}
			goodsData2.BagIndex = num;
			if (goodsData2.BagIndex != bagIndex)
			{
				GoodsData goodsData3 = goodsData2;
				GameInstance.Game.SpriteModGoods(3, goodsData3.Id, goodsData3.GoodsID, goodsData3.Using, goodsData3.Site, goodsData3.GCount, goodsData3.BagIndex, string.Empty);
			}
			list.Add(goodsData2);
			list.Sort(delegate(GoodsData x, GoodsData y)
			{
				if (x.BagIndex != y.BagIndex)
				{
					return x.BagIndex - y.BagIndex;
				}
				if (x.GoodsID != y.GoodsID)
				{
					return x.GoodsID - y.GoodsID;
				}
				return x.GCount - y.GCount;
			});
		}
		if (j < list2.Count)
		{
		}
		for (int k = 0; k < list3.Count; k++)
		{
			GoodsData goodsData4 = list3[k];
			list.Add(goodsData4);
		}
		if (this.BaoGuoMode == BaoGuoMode.horseCangKu)
		{
			this.GoodsDataList = list;
		}
		else
		{
			Global.Data.JingLingGoodsDataList = list;
		}
	}

	protected bool ExistBagIndex(List<GoodsData> tmpList, int bagIndex)
	{
		for (int i = 0; i < tmpList.Count; i++)
		{
			if (tmpList[i].BagIndex == bagIndex)
			{
				return true;
			}
		}
		return false;
	}

	protected int GenerateNewBagIndexThanMinValue(int minIndex)
	{
		List<GoodsData> thisPartGoodsList = this.GetThisPartGoodsList();
		return this.GenerateNewBagIndex(thisPartGoodsList, minIndex);
	}

	protected int GenerateNewBagIndex(List<GoodsData> existSortedList, int minIndex = 0)
	{
		if (existSortedList == null)
		{
			return minIndex;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < existSortedList.Count; i++)
		{
			GoodsData goodsData = existSortedList[i];
			if (this.CanShowGoodsInPacel(goodsData))
			{
				int bagIndex = goodsData.BagIndex;
				if (list.IndexOf(bagIndex) < 0)
				{
					list.Add(bagIndex);
				}
			}
		}
		for (int j = minIndex; j < Global.GetJinDanBagCapacity(); j++)
		{
			if (list.IndexOf(j) < 0)
			{
				return j;
			}
		}
		return -1;
	}

	protected bool IsGoodsBagIndexRepeat(GoodsData goodsData)
	{
		List<GoodsData> thisPartGoodsList = this.GetThisPartGoodsList();
		for (int i = 0; i < thisPartGoodsList.Count; i++)
		{
			GoodsData goodsData2 = thisPartGoodsList[i];
			if (this.CanShowGoodsInPacel(goodsData2))
			{
				if (goodsData2.BagIndex == goodsData.BagIndex && goodsData2.Id != goodsData.Id)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected void TryToModifyBagIndex(GoodsData goodsData)
	{
		if (goodsData.BagIndex < 0)
		{
			goodsData.BagIndex = 0;
		}
		if (goodsData.BagIndex < this.CurrentSelectedPage * this.BagSizeAPage || goodsData.BagIndex >= (this.CurrentSelectedPage + 1) * this.BagSizeAPage || this.IsGoodsBagIndexRepeat(goodsData))
		{
			int num = this.GenerateNewBagIndexThanMinValue(this.CurrentSelectedPage * this.BagSizeAPage);
			if (num < 0)
			{
				num = this.GenerateNewBagIndexThanMinValue(0);
			}
			if (num >= 0 && num != goodsData.BagIndex)
			{
				goodsData.BagIndex = num;
				GameInstance.Game.SpriteModGoods(3, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
			}
		}
	}

	internal void NoticeRideZhengLiCallBack(List<GoodsData> NewMountStore)
	{
		if (NewMountStore != null)
		{
			for (int i = NewMountStore.Count - 1; i >= 0; i--)
			{
				if (0 >= NewMountStore[i].GCount)
				{
					NewMountStore.RemoveAt(i);
				}
			}
			Global.Data.roleData.MountStoreList = NewMountStore;
			this.RefreshGoodsDataList(Global.Data.roleData.MountStoreList, true, false);
			this.ResetJingLingbag();
		}
	}

	internal void ShowHelpAnim(int p, int param)
	{
		if (p == 607)
		{
			GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(0);
			if (null != goodsIcon)
			{
				SystemHelpPart.SetMask(goodsIcon, default(Vector4));
			}
		}
	}

	private const int bgRows = 4;

	private const int bgColumn = 6;

	public TextBlock Page;

	public UISprite[] Pages;

	public TextBlock ContentText;

	public TextBlock BagCapacity;

	public GButton CloseBtn;

	public GButton CaozuoBtn;

	public GButton ZhengliBtn;

	public GButton huiShouBtn;

	public Canvas baoGuoCanvas;

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL selectImage;

	public SpriteSL selectBaoGuoImage;

	[SerializeField]
	private UISprite mTitleSp;

	private bool isPiliang;

	public GGoodsBox goodsBox;

	private int rgoodsDBid;

	private int BagSizeAPage = 240;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	public GChildWindow PetActiveWindow;

	private LoadingWindow LoadingWin;

	private List<GoodsData> GoodsDataList;

	private List<Image> DisableImagesList = new List<Image>();

	private int[] equiplistMenuItemIDs = new int[]
	{
		3
	};

	private string[] equiplistMenuItemNames = new string[]
	{
		Global.GetLang("售出")
	};

	public bool ForceShowNavi;

	private float deltaX;

	public SpringPanel UIDragPl;

	private BaoGuoMode mBaoGuoMode = BaoGuoMode.JingLingCangKu;

	private List<GoodsData> OldGoodsArrayList = new List<GoodsData>();

	private UISprite tempPaneStat;
}
