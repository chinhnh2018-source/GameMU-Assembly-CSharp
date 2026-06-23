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

public class ZaDanCangKuPart : UserControl
{
	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id < 100)
			{
				GameObject at = this.goodsBox.listBox.ItemsSource.GetAt(id);
				if (null != at)
				{
					SystemHelpPart.SetMask(at.transform, default(Vector4));
				}
				else
				{
					SystemHelpPart.HideMask();
				}
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.CloseBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void InitTextInPrefabs()
	{
		this.ZhengliBtn.Text = Global.GetLang("整 理");
		this.CaozuoBtn.Text = Global.GetLang("取消操作");
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
			GameInstance.Game.SpriteResetJinDanBag();
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
			parcelPart.iBaoGuoMode = 6;
			Super._ParcelPart = parcelPart;
		}
		Super._ParcelPart.iBaoGuoMode = 6;
		Super._ParcelPart.Visibility = true;
		this.baoGuoCanvas.Children.Add(Super._ParcelPart);
		yield return new WaitForSeconds(0.02f);
		Super._ParcelPart.InitPartData();
		this.SetPiliangCaozuo(true);
		yield return new WaitForSeconds(0.1f);
		GameInstance.Game.SpriteGetJinDanGoodsDataList(2000);
		yield break;
	}

	public void RefreshGoodsUsing()
	{
	}

	public void RefreshGoodsDataList(List<GoodsData> tmpGoodsDataList, bool resort = false, bool isStep = false)
	{
		if (Global.Data.IsDoingZaJinDan)
		{
			return;
		}
		if (tmpGoodsDataList != null && resort)
		{
			tmpGoodsDataList.Sort(new Comparison<GoodsData>(this.goodsDataList_Sort));
		}
		Global.Data.JinDanGoodsDataList = tmpGoodsDataList;
		List<GoodsData> list = this.FindAppendNewGoodsArr();
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				GoodsData gd = list[i];
				this.RefreshJinDanBag(gd);
			}
		}
		else
		{
			GoodsData gd2 = null;
			this.RefreshJinDanBag(gd2);
		}
		this.GoodsDataList = Global.Data.JinDanGoodsDataList;
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

	public void ResetJinDanbag()
	{
		if (Global.Data.JinDanGoodsDataList != null)
		{
			for (int i = Global.Data.JinDanGoodsDataList.Count; i < this.BagSizeAPage; i++)
			{
				this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
		}
	}

	protected List<GoodsData> FindAppendNewGoodsArr()
	{
		if (this.OldGoodsArrayList == null || this.OldGoodsArrayList.Count == 0)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		if (Global.Data.JinDanGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.JinDanGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.JinDanGoodsDataList[i];
				if (!this.ExisitGoods(goodsData))
				{
					list.Add(goodsData);
				}
			}
		}
		return list;
	}

	protected GoodsData FindAppendNewGoods()
	{
		if (this.OldGoodsArrayList == null || this.OldGoodsArrayList.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < Global.Data.JinDanGoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.JinDanGoodsDataList[i];
			if (!this.ExisitGoods(goodsData))
			{
				return goodsData;
			}
		}
		return null;
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

	public void RefreshJinDanBag(GoodsData gd = null)
	{
		if (gd == null)
		{
			this.ClearIconEvents();
			if (Global.Data.JinDanGoodsDataList != null)
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
		ObjectClickGetingMgr.CancelClickGetThing(17);
		SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPart, HelpStateEvents.Inactived, 1);
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPartQuHui, HelpStateEvents.None, -1);
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (this.selectImage.Visibility)
		{
			this.rgoodsDBid = (ggoodIcon.ItemObject as GoodsData).BagIndex;
			Global.MoveGoodsToParcel(ggoodIcon.ItemObject as GoodsData, 0);
		}
		else
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfPet, goodData);
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

	private bool JugeCanAddGoods()
	{
		return Global.CanJinDanAddGoods((this.GoodsDataList != null) ? this.GoodsDataList.Count : 0);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (!clickGetThingEventArgs.Cancel || ObjectClickGetingMgr.IsType(17))
		{
		}
		if (clickGetThingEventArgs.ClickGetThingType != 17)
		{
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		GIcon gicon = sender as GIcon;
		if (null == gicon)
		{
			return;
		}
		if (gicon.BoxTypes == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("宝物仓库只能提取，不能存放"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.MoveGoodsBetweenParcelAndJinDanBag(gicon);
		clickGetThingEventArgs.NextClick = true;
	}

	private void MoveGoodsBetweenParcelAndJinDanBag(GIcon icon)
	{
		if (null == icon)
		{
			return;
		}
		if (icon.BoxTypes == 1)
		{
			Global.MoveGoodsToJinDanBag(icon.ItemObject as GoodsData, 0);
		}
		else if (icon.BoxTypes == 14)
		{
			Global.MoveGoodsToParcel(icon.ItemObject as GoodsData, 0);
		}
	}

	public void RefreshGoodsStates()
	{
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

	public IEnumerator ShowPage(int pageIndex)
	{
		this.MaxPageCount = Global.GetJinDanBagCapacity() / this.BagSizeAPage;
		int startIndex = pageIndex * this.BagSizeAPage;
		int disableGridStart = this.GetDisableGridStartIndexOfCurrentPage(pageIndex) + startIndex;
		if (Global.Data.JinDanGoodsDataList != null)
		{
			int counter = 0;
			for (int subIndex = 0; subIndex < Global.Data.JinDanGoodsDataList.Count; subIndex++)
			{
				GoodsData gd = Global.Data.JinDanGoodsDataList[subIndex];
				if (gd != null)
				{
					counter++;
					GGoodIcon icon = this.AddIcon(gd);
					this.goodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), icon);
					icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
					bool canUseGoods = Global.CanUseGoods(gd.GoodsID, false, true);
					Super.InitGoodsGIcon(icon, gd, canUseGoods, IconTextTypes.Qianghua);
					Global.SetEquipGoodsZhanLiStat(icon, gd);
					if (counter % 5 == 0)
					{
						yield return null;
					}
				}
			}
		}
		if (Global.GetJinDanBagCapacity() <= 0)
		{
			this.Page.Text = "0/0";
		}
		this.RefreshBagPageText();
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPart, HelpStateEvents.Actived, 1);
		yield break;
	}

	public IEnumerator ShowPageEx(int pageIndex, bool isStep = false)
	{
		this.MaxPageCount = Global.GetJinDanBagCapacity() / this.BagSizeAPage;
		Dictionary<int, int> indexDict = new Dictionary<int, int>();
		if (Global.Data.JinDanGoodsDataList != null)
		{
			int count = Global.Data.JinDanGoodsDataList.Count;
			for (int subIndex = 0; subIndex < count; subIndex++)
			{
				GoodsData gd = Global.Data.JinDanGoodsDataList[subIndex];
				if (gd != null)
				{
					indexDict.Add(gd.BagIndex, subIndex);
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
				GoodsData gd2 = Global.Data.JinDanGoodsDataList[dataIndex];
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
		SystemHelpMgr.OnAction(UIObjIDs.QiFuCangKuPart, HelpStateEvents.Actived, 1);
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

	private void PageIconMouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void ClearIconEvents()
	{
	}

	public GGoodIcon FindGoodsIconByDbID(int dbID)
	{
		return null;
	}

	protected bool CanShowGoodsInPacel(GoodsData gd)
	{
		return gd != null && gd.Using <= 0 && gd.GCount > 0;
	}

	protected void SortGoodsBagIndex()
	{
		List<GoodsData> jinDanGoodsDataList = Global.Data.JinDanGoodsDataList;
		jinDanGoodsDataList.Sort(delegate(GoodsData x, GoodsData y)
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
		for (int i = 0; i < jinDanGoodsDataList.Count; i++)
		{
			GoodsData goodsData = jinDanGoodsDataList[i];
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
		Global.Data.JinDanGoodsDataList = list;
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
		List<GoodsData> jinDanGoodsDataList = Global.Data.JinDanGoodsDataList;
		return this.GenerateNewBagIndex(jinDanGoodsDataList, minIndex);
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

	protected int GenerateNewBagIndexForPage(int page, int pageSize)
	{
		return 0;
	}

	protected bool IsGoodsBagIndexRepeat(GoodsData goodsData)
	{
		List<GoodsData> jinDanGoodsDataList = Global.Data.JinDanGoodsDataList;
		for (int i = 0; i < jinDanGoodsDataList.Count; i++)
		{
			GoodsData goodsData2 = jinDanGoodsDataList[i];
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

	private const int bgRows = 4;

	private const int bgColumn = 6;

	public TextBlock Page;

	public UISprite[] Pages;

	public TextBlock ContentText;

	public TextBlock BagCapacity;

	public GButton CloseBtn;

	public GButton CaozuoBtn;

	public GButton ZhengliBtn;

	public Canvas baoGuoCanvas;

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL selectImage;

	public SpriteSL selectBaoGuoImage;

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

	private string[] goodsMenuItemNames = new string[]
	{
		Global.GetLang("拆分"),
		Global.GetLang("售出")
	};

	private int[] equiplistMenuItemIDs = new int[]
	{
		3
	};

	private string[] equiplistMenuItemNames = new string[]
	{
		Global.GetLang("售出")
	};

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	public bool ForceShowNavi;

	private float deltaX;

	public SpringPanel UIDragPl;

	private List<GoodsData> OldGoodsArrayList = new List<GoodsData>();

	private UISprite tempPaneStat;
}
