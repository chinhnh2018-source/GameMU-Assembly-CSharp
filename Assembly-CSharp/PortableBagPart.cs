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

public class PortableBagPart : UserControl
{
	public bool IsRebornPortableBag
	{
		get
		{
			return this.m_isRebornPortableBag;
		}
		set
		{
			this.m_isRebornPortableBag = value;
		}
	}

	public List<GoodsData> SelfPortableGoods
	{
		get
		{
			if (this.m_isRebornPortableBag)
			{
				return Global.Data.roleData.RebornGoodsStoreList;
			}
			return Global.Data.PortableGoodsDataList;
		}
		set
		{
			if (this.m_isRebornPortableBag)
			{
				Global.Data.roleData.RebornGoodsStoreList = value;
			}
			else
			{
				Global.Data.PortableGoodsDataList = value;
			}
		}
	}

	protected virtual int SelfPortableBagNum
	{
		get
		{
			if (this.m_isRebornPortableBag)
			{
				return ChongShengData.GetChongShengPortableBagNum();
			}
			return Global.Data.roleData.MyPortableBagData.ExtGridNum;
		}
		set
		{
			if (this.m_isRebornPortableBag)
			{
				ChongShengData.SetChongShengPortableBagNum(value);
			}
			else
			{
				Global.Data.roleData.MyPortableBagData.ExtGridNum = value;
			}
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public bool ForceShowNavi
	{
		get
		{
			return this._ForceShowNavi;
		}
		set
		{
			this._ForceShowNavi = value;
		}
	}

	private void InitPage()
	{
		if (null == this._PageListBox)
		{
		}
		if (null != this._PageSp)
		{
			ObservableCollection itemsSource = this._PageListBox.ItemsSource;
			for (byte b = 0; b < this.m_PageCount; b += 1)
			{
				UISprite uisprite = Object.Instantiate<UISprite>(this._PageSp);
				if (null != uisprite)
				{
					itemsSource.AddNoUpdate(uisprite);
					this.m_ListPage.Add(uisprite);
					uisprite.transform.localScale = Vector3.one * 18f;
				}
			}
			Object.Destroy(this._PageSp.gameObject);
		}
	}

	public void RefreshMoney()
	{
		this.bindMoney.text = Global.Data.roleData.StoreMoney.ToString();
		this.Money.text = Global.Data.roleData.StoreYinLiang.ToString();
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
				if (this.CurrentSelectedPage >= (int)this.m_PageCount)
				{
					this.CurrentSelectedPage = (int)(this.m_PageCount - 1);
				}
			}
		}
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPageText();
	}

	private void InitTextInPrefabs()
	{
		this.zhengLiBtn.Text = Global.GetLang("整 理");
		this.kuozhangBtn.Text = Global.GetLang("确定");
		this.quxiaoBtn.Text = Global.GetLang("取消");
		this.cunQuMoneyBtn.Text = Global.GetLang("存取金币");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		Global.InitBagParams();
		this.BagSizeAPage = Global.DefaultPortableGridNum;
		this.UIDragPl.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.caoZuoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (ObjectClickGetingMgr.IsType(6))
			{
				ObjectClickGetingMgr.CancelClickGetThing(6);
				this.caoZuoBtn.Text = Global.GetLang("批量操作");
				this.selectImage.Visibility = false;
				this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
				if (this.IsRebornPortableBag)
				{
					Super._ParcelRebornPart.isShowTips = true;
				}
				else
				{
					Super._ParcelPart.isShowTips = true;
				}
			}
			else
			{
				ObjectClickGetingMgr.StartClickGetThing(6, e);
				this.caoZuoBtn.Text = Global.GetLang("取消操作");
				this.selectImage.Visibility = true;
				this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
				if (this.IsRebornPortableBag)
				{
					Super._ParcelRebornPart.isShowTips = false;
				}
				else
				{
					Super._ParcelPart.isShowTips = false;
				}
			}
		};
		this.zhengLiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_isRebornPortableBag)
			{
				GameInstance.Game.SpriteResetRebornPortableBag();
			}
			else
			{
				GameInstance.Game.SpriteResetPortableBag();
			}
		};
		this.returnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.PortableBagPart, HelpStateEvents.Inactived, -1);
		};
		this.kuozhangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.extWindow.gameObject.SetActive(false);
			this.modalBak.gameObject.SetActive(false);
			if (this.m_isRebornPortableBag)
			{
				GameInstance.Game.SpriteExtRebornGridByYuanBao((double)this.wantToAddGridsNum);
			}
			else
			{
				GameInstance.Game.SpriteExtGridByYuanBao((double)this.wantToAddGridsNum, 2, this.needZSText.Text.SafeToInt32(0));
			}
		};
		this.quxiaoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.extWindow.gameObject.SetActive(false);
			this.modalBak.gameObject.SetActive(false);
		};
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.extWindow.gameObject.SetActive(false);
			this.modalBak.gameObject.SetActive(false);
		};
		this.cunQuMoneyBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2
				});
			}
		};
		this.m_PageCount = (byte)(Global.MaxPortableGridNum / 4 / 6);
		this.InitPage();
		this.goodsBox.listBox.maxPerLine = (int)(this.m_PageCount * 6);
		this.goodsBox.RowCount = 4;
		this.goodsBox.ColCount = Global.MaxPortableGridNum / 4;
		this.goodsBox.InitBox();
		this.RefreshBagPageText();
		this.CheckBagShowCount();
	}

	public void CleanUpChildWindows()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.CheckBagShowCount();
		this.RefreshMoney();
		this.goMoney.SetActive(!this.IsRebornPortableBag);
		this.cunQuMoneyBtn.gameObject.SetActive(!this.IsRebornPortableBag);
		if (this.IsRebornPortableBag)
		{
			this.caoZuoBtn.transform.localPosition = this.cunQuMoneyBtn.transform.localPosition;
		}
		if (!this.IsRebornPortableBag)
		{
			if (Super._ParcelPart == null)
			{
				ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
				parcelPart.iBaoGuoMode = 2;
				parcelPart.InitPartData();
				Super._ParcelPart = parcelPart;
			}
			Super._ParcelPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 2 || e.ID == 11)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -3
					});
				}
				return true;
			};
			Super._ParcelPart.iBaoGuoMode = 2;
			Super._ParcelPart.Visibility = true;
			this.baoGuoCanvas.Children.Add(Super._ParcelPart);
			ObjectClickGetingMgr.StartClickGetThing(6, null);
			this.caoZuoBtn.Text = Global.GetLang("取消操作");
			this.selectImage.Visibility = true;
			this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
			Super._ParcelPart.isShowTips = false;
		}
		else
		{
			if (Super._ParcelRebornPart == null)
			{
				ParcelPart parcelPart2 = U3DUtils.NEW<ParcelPart>();
				parcelPart2.IsRebornParcel = true;
				parcelPart2.iBaoGuoMode = 11;
				parcelPart2.InitPartData();
				Super._ParcelRebornPart = parcelPart2;
			}
			Super._ParcelRebornPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 2 || e.ID == 11)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -3
					});
				}
				return true;
			};
			Super._ParcelRebornPart.iBaoGuoMode = 11;
			Super._ParcelRebornPart.Visibility = true;
			this.baoGuoCanvas.Children.Add(Super._ParcelRebornPart);
			ObjectClickGetingMgr.StartClickGetThing(6, null);
			this.caoZuoBtn.Text = Global.GetLang("取消操作");
			this.selectImage.Visibility = true;
			this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
			Super._ParcelRebornPart.isShowTips = false;
		}
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (this.m_isRebornPortableBag)
		{
			base.StartCoroutine<bool>(this.RefreshGoodsDataList(this.SelfPortableGoods, true, true));
		}
		else
		{
			if (!this.FirstGetNewData)
			{
				if (this.ForceShowNavi)
				{
					this.ForceShowNavi = false;
				}
				return;
			}
			this.FirstGetNewData = false;
			GameInstance.Game.SpriteGetGoodsDataListBySite(-1000);
		}
	}

	public void RefreshExtGridNum()
	{
		if (null != this.petAddBagNumsPart)
		{
			this.petAddBagNumsPart.RefreshData(0);
		}
	}

	public IEnumerator RefreshGoodsDataList(List<GoodsData> tmpGoodsDataList, bool resort = false, bool isStep = false)
	{
		if (tmpGoodsDataList != null && resort)
		{
			tmpGoodsDataList.Sort(new Comparison<GoodsData>(this.goodsDataList_Sort));
		}
		this.SelfPortableGoods = tmpGoodsDataList;
		yield return new WaitForSeconds(0.02f);
		GoodsData newGoodsObj = this.FindAppendNewGoods();
		this.addNewGoods(newGoodsObj);
		if (newGoodsObj == null)
		{
			this.RefreshPortableBag(newGoodsObj);
			base.StartCoroutine<bool>(this.ShowPageEx(0, isStep));
		}
		if (tmpGoodsDataList == null || tmpGoodsDataList.Count <= 0 || this.ForceShowNavi)
		{
			this.ForceShowNavi = false;
		}
		this.OldGoodsArrayList.Clear();
		if (this.SelfPortableGoods != null)
		{
			for (int i = 0; i < this.SelfPortableGoods.Count; i++)
			{
				this.OldGoodsArrayList.Add(this.SelfPortableGoods[i]);
			}
		}
		SystemHelpMgr.OnAction(UIObjIDs.PortableBagPart, HelpStateEvents.Actived, 1);
		yield break;
	}

	protected GoodsData FindAppendNewGoods()
	{
		if (this.OldGoodsArrayList == null || this.OldGoodsArrayList.Count == 0)
		{
			return null;
		}
		if (this.SelfPortableGoods != null)
		{
			for (int i = 0; i < this.SelfPortableGoods.Count; i++)
			{
				GoodsData goodsData = this.SelfPortableGoods[i];
				if (!this.ExisitGoods(goodsData))
				{
					return goodsData;
				}
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

	public void RefreshPortableBag(GoodsData gd = null)
	{
		if (this.SelfPortableGoods != null)
		{
			this.SortGoodsBagIndex();
		}
	}

	private int goodsDataList_Sort(GoodsData x, GoodsData y)
	{
		return x.BagIndex - y.BagIndex;
	}

	public override void Destroy()
	{
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		if (this.selectImage.Visibility)
		{
			if (this.m_isRebornPortableBag)
			{
				Global.MoveGoodsToChongShengParcel(ggoodIcon.ItemObject as GoodsData, 0);
			}
			else
			{
				Global.MoveGoodsToParcel(ggoodIcon.ItemObject as GoodsData, 0);
				SystemHelpMgr.OnAction(UIObjIDs.PortableBagPartGetGoods, HelpStateEvents.Clicked, 1);
			}
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
		int num = (int)(24 * this.m_PageCount);
		for (int i = this.SelfPortableBagNum; i < num; i++)
		{
			GGoodIcon ggoodIcon = this.ShowDisableGGoodIcon(i, false);
			this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.OnClickDisableGrid));
		}
		this.SetFristDisableGrid();
	}

	private int GetDisableGridStartIndexOfCurrentPage(int pageIndex)
	{
		int result = 0;
		if (this.MaxPageCount > 0)
		{
			int num = this.SelfPortableBagNum - pageIndex * 216;
			if (num < 0)
			{
				result = 0;
			}
			else if (num <= 216)
			{
				result = num;
			}
			else
			{
				result = 216;
			}
		}
		return result;
	}

	private bool OpenBagCanOnTime()
	{
		return 120 >= this.SelfPortableBagNum + 1;
	}

	private void OnClickDisableGrid(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		int num = (int)ggoodIcon.Tag;
		if (num < 0)
		{
			return;
		}
		this.wantToAddGridsNum = num;
		this.wantToAddGridsNum = num - this.SelfPortableBagNum + 1;
		if (120 <= num)
		{
			if (!this.OpenBagCanOnTime())
			{
				if (this.m_isRebornPortableBag)
				{
					this.ShowRebornNeedWindow();
				}
				else
				{
					this.ShowExtWindow(-99);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("请先开启背包的前5页"), 10, 3);
			}
		}
		else if (this.m_isRebornPortableBag)
		{
			this.ShowRebornNeedWindow();
		}
		else
		{
			GameInstance.Game.SpriteQueryOpenPortableGridTickCmd();
		}
	}

	public void ShowExtWindow(int onlineExtTime)
	{
		if (0 <= onlineExtTime)
		{
			BaoGuoMode mode = (!this.m_isRebornPortableBag) ? BaoGuoMode.CangKu : BaoGuoMode.ChongShengCangKu;
			int num;
			int extBagGridNeedYuanBao = Global.GetExtBagGridNeedYuanBao(this.wantToAddGridsNum, onlineExtTime, mode, out num);
			this.needZSText.Text = ((extBagGridNeedYuanBao >= 0) ? extBagGridNeedYuanBao.ToString() : "0");
			this.ExtGridNumText.Text = this.wantToAddGridsNum.ToString();
			int num2 = num;
			num2 = ((num2 >= 0) ? num2 : 0);
			this.needTimeText.Text = string.Format(Global.GetLang("{0}小时{1}分钟"), num2 / 3600, num2 % 3600 / this.BagSizeAPage);
			if (!SystemHelpPart.IsMaskShowing())
			{
				this.extWindow.gameObject.SetActive(true);
				this.modalBak.gameObject.SetActive(true);
				Vector3 localPosition = this.modalBak.transform.localPosition;
				localPosition.z = this.modalBak.transform.parent.localPosition.z - 4f;
				this.modalBak.transform.localPosition = localPosition;
				localPosition.z -= 0.1f;
				this.extWindow.transform.localPosition = localPosition;
			}
			Transform transform = this.extWindow.transform.FindChild("bak2");
			if (null != transform)
			{
				ShowNetImage component = transform.GetComponent<ShowNetImage>();
				if (null != component)
				{
					component.URL = "NetImages/GameRes/Images/Plate/bagExpandBagGrid_bak.jpg";
				}
			}
		}
		else if (onlineExtTime == -99)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ExtraStoragePrice", ',');
			if (systemParamIntArrayByName.Length == 2)
			{
				int num3 = systemParamIntArrayByName[1] * this.wantToAddGridsNum;
				this.needZSText.Text = ((num3 >= 0) ? num3.ToString() : "0");
				this.ExtGridNumText.Text = this.wantToAddGridsNum.ToString();
				this.needTimeText.gameObject.SetActive(false);
				if (!SystemHelpPart.IsMaskShowing())
				{
					this.extWindow.gameObject.SetActive(true);
					this.modalBak.gameObject.SetActive(true);
					Vector3 localPosition2 = this.modalBak.transform.localPosition;
					localPosition2.z = this.modalBak.transform.parent.localPosition.z - 4f;
					this.modalBak.transform.localPosition = localPosition2;
					localPosition2.z -= 0.1f;
					this.extWindow.transform.localPosition = localPosition2;
				}
			}
			Transform transform2 = this.extWindow.transform.FindChild("bak2");
			if (null != transform2)
			{
				ShowNetImage component2 = transform2.GetComponent<ShowNetImage>();
				if (null != component2)
				{
					component2.URL = "NetImages/GameRes/Images/Plate/bagExpandBagGrid_bak1.jpg";
				}
			}
		}
	}

	public void ShowRebornNeedWindow()
	{
		BaoGuoMode mode = BaoGuoMode.None;
		if (this.m_isRebornPortableBag)
		{
			mode = BaoGuoMode.ChongShengCangKu;
		}
		int rebornBagGridNeedYuanBao = Global.GetRebornBagGridNeedYuanBao(this.wantToAddGridsNum, mode);
		this.needZSText.Text = ((rebornBagGridNeedYuanBao >= 0) ? rebornBagGridNeedYuanBao.ToString() : "0");
		this.ExtGridNumText.Text = this.wantToAddGridsNum.ToString();
		this.needTimeText.Text = string.Empty;
		Transform transform = this.extWindow.transform.FindChild("bak2");
		if (null != transform)
		{
			ShowNetImage component = transform.GetComponent<ShowNetImage>();
			if (null != component)
			{
				component.URL = "NetImages/GameRes/Images/Plate/bagExpandBagGrid_bak1.jpg";
			}
		}
		this.extWindow.gameObject.SetActive(true);
		this.modalBak.gameObject.SetActive(true);
		Vector3 localPosition = this.modalBak.transform.localPosition;
		localPosition.z = -2.25f;
		this.modalBak.transform.localPosition = localPosition;
		Vector3 localPosition2 = this.extWindow.transform.localPosition;
		localPosition2.z = -2.35f;
		this.extWindow.transform.localPosition = localPosition2;
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	public void RemoveGoods(params object[] args)
	{
		int num;
		if (args[0] is int)
		{
			num = (int)args[0];
		}
		else
		{
			num = ((GoodsData)args[0]).Id;
		}
		int num2 = this.goodsBox.FindByGoodsDbID(num);
		if (num2 >= 0)
		{
			this.goodsBox.RemoveGoodsIcon(num2);
		}
		for (int i = 0; i < this.SelfPortableGoods.Count; i++)
		{
			GoodsData goodsData = this.SelfPortableGoods[i];
			if (goodsData.Id == num)
			{
				this.goodsBox.RemoveGoodsIcon(this.GetGoodsIconIndex(goodsData));
				break;
			}
		}
		if (args[0] is int)
		{
			this.RemoveGoods((int)args[0]);
		}
		else if (args[0] is object)
		{
			this.RemoveGoods((GoodsData)args[0]);
		}
	}

	public void RemoveGoods(int goodsDbID)
	{
		int num = this.goodsBox.FindByGoodsDbID(goodsDbID);
		if (num != -1)
		{
			this.goodsBox.RemoveGoodsIcon(num);
		}
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
		return Global.CanPortableAddGoods((this.SelfPortableGoods != null) ? this.SelfPortableGoods.Count : 0);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (!clickGetThingEventArgs.Cancel || ObjectClickGetingMgr.IsType(6))
		{
		}
		if (clickGetThingEventArgs.ClickGetThingType != 6)
		{
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		GGoodIcon icon = sender as GGoodIcon;
		this.MoveGoodsBetweenParcelAndPortableBag(icon);
		clickGetThingEventArgs.NextClick = true;
	}

	private void MoveGoodsBetweenParcelAndPortableBag(GGoodIcon icon)
	{
		if (null == icon)
		{
			return;
		}
		if (icon.BoxTypes == 11)
		{
			Global.MoveGoodsToParcel(icon.ItemObject as GoodsData, 0);
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
		}
	}

	private void listBox2_MouseLeftButtonDown(object sender, MouseEvent e)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
		}
	}

	public void RefreshGoodsStates()
	{
		for (int i = 0; i < this.ItemCollection.Length; i++)
		{
			bool canUse = Global.CanUseGoods(U3DUtils.AS<GGoodIcon>(this.ItemCollection[i]).ItemCode, false, true);
			Super.InitGoodsGIcon(U3DUtils.AS<GGoodIcon>(this.ItemCollection[i]), U3DUtils.AS<GGoodIcon>(this.ItemCollection[i]).ItemObject as GoodsData, canUse, IconTextTypes.Qianghua);
		}
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
			icon.TextShadowColor = 4278190080U;
			icon.BoxTypes = 11;
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 6)
				{
					if (this.IsRebornPortableBag)
					{
						Global.MoveGoodsToChongShengParcel(icon.ItemObject as GoodsData, 0);
					}
					else
					{
						Global.MoveGoodsToParcel(icon.ItemObject as GoodsData, 0);
					}
				}
			};
			return icon;
		}
		return null;
	}

	public void addNewEmptyGrid(int istart, int iend, bool IsTrue = true)
	{
		base.StartCoroutine<bool>(this.AddNewEmptyGrid(istart, iend, IsTrue));
	}

	private IEnumerator AddNewEmptyGrid(int istart, int iend, bool IsTrue)
	{
		if (IsTrue)
		{
			Super.ShowNetWaiting(null);
		}
		byte index = 0;
		for (int subIndex = istart; subIndex < iend; subIndex++)
		{
			byte b = 6;
			byte b2;
			index = (b2 = index) + 1;
			if (b <= b2)
			{
				index = 0;
				yield return null;
			}
			this.goodsBox.RemoveGoodsIcon(this.Getindex(subIndex));
		}
		Super.HideNetWaiting();
		if (IsTrue)
		{
			this.SelfPortableBagNum = iend;
			this.CheckBagShowCount();
			this.SetFristDisableGrid();
			this.ShowDisableGrid(0);
		}
		yield break;
	}

	public void addNewGoods(GoodsData gd)
	{
		if (gd != null)
		{
			GGoodIcon ggoodIcon = this.AddIcon(gd);
			this.goodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			Global.SetEquipGoodsZhanLiStat(ggoodIcon, gd);
		}
	}

	public IEnumerator ShowPage(int pageIndex)
	{
		this.MaxPageCount = Global.MaxPortableGridNum / 216;
		int startIndex = pageIndex * 216;
		int disableGridStart = this.GetDisableGridStartIndexOfCurrentPage(pageIndex) + startIndex;
		if (this.SelfPortableGoods != null)
		{
			int counter = 0;
			for (int subIndex = 0; subIndex < this.SelfPortableGoods.Count; subIndex++)
			{
				GoodsData gd = this.SelfPortableGoods[subIndex];
				if (gd != null && gd.GCount > 0 && gd.Using == 0 && gd.Site == -1000)
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
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.ShowDisableGrid(pageIndex);
		yield break;
	}

	private void CheckBagShowCount()
	{
		bool flag;
		if (this.OpenBagCanOnTime())
		{
			flag = false;
			this.m_PageCount = 5;
		}
		else
		{
			this.m_PageCount = (byte)(Global.MaxPortableGridNum / 4 / 6);
			flag = true;
		}
		byte b = 0;
		while ((int)b < this.m_ListPage.Count)
		{
			NGUITools.SetActive(this.m_ListPage[(int)b], flag || b < 5);
			b += 1;
		}
		if (!flag)
		{
			this.addNewEmptyGrid(120, Global.MaxPortableGridNum, false);
		}
		Vector3 localPosition = this._PageListBox.transform.localPosition;
		localPosition.x = -225f - (float)(29 * (this.m_PageCount - 1)) / 2f;
		this._PageListBox.transform.localPosition = localPosition;
		this._PageListBox.repositionNow = true;
		Vector3 localScale = this._BagBG.transform.localScale;
		localScale.x = (float)(468 * (int)this.m_PageCount);
		this._BagBG.transform.localScale = localScale;
	}

	public IEnumerator ShowPageEx(int pageIndex, bool isStep = false)
	{
		this.MaxPageCount = Global.MaxPortableGridNum / 216;
		Dictionary<int, int> indexDict = new Dictionary<int, int>();
		if (this.SelfPortableGoods != null)
		{
			int count = this.SelfPortableGoods.Count;
			for (int subIndex = 0; subIndex < count; subIndex++)
			{
				GoodsData gd = this.SelfPortableGoods[subIndex];
				if (gd != null)
				{
					SaleGoodsConsts site = (!this.m_isRebornPortableBag) ? SaleGoodsConsts.PortableGoodsID : SaleGoodsConsts.RebornStore;
					if (gd.GCount > 0 && gd.Using == 0 && gd.Site == (int)site)
					{
						indexDict[gd.BagIndex] = subIndex;
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
		for (int i = 0; i < this.SelfPortableBagNum; i++)
		{
			counter++;
			int dataIndex = -1;
			if (indexDict.TryGetValue(i, ref dataIndex))
			{
				GoodsData gd2 = this.SelfPortableGoods[dataIndex];
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
		this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.ShowDisableGrid(pageIndex);
		yield break;
	}

	private void RefreshBagPageText()
	{
		if (this.tempPaneStat != null)
		{
			this.tempPaneStat.spriteName = "selectState_normal2";
		}
		if (NGUITools.GetActive(this.m_ListPage[this.CurrentSelectedPage].gameObject))
		{
			this.m_ListPage[this.CurrentSelectedPage].spriteName = "selectState_hover2";
			this.tempPaneStat = this.m_ListPage[this.CurrentSelectedPage];
		}
	}

	private void SetFristDisableGrid()
	{
		if (this.SelfPortableBagNum < Global.MaxPortableGridNum)
		{
			GGoodIcon ggoodIcon = this.ShowDisableGGoodIcon(this.SelfPortableBagNum, true);
			this.goodsBox.SetGoodsIcon(this.Getindex(this.SelfPortableBagNum), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.OnClickDisableGrid));
		}
	}

	private void ShowEmptyGrid(int pageIndex)
	{
		for (int i = pageIndex; i < Global.GetPortableBagCapacity(); i++)
		{
			GGoodIcon emptyGGoodIcon = this.GetEmptyGGoodIcon();
			this.ItemCollection.Add(emptyGGoodIcon.gameObject);
		}
	}

	private GGoodIcon GetEmptyGGoodIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 64.0;
		ggoodIcon.Height = 64.0;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private GGoodIcon ShowDisableGGoodIcon(int index, bool bFirst = false)
	{
		GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
		newGoodIcon.isAutoSize = true;
		newGoodIcon.Width = 78.0;
		newGoodIcon.Height = 78.0;
		if (bFirst)
		{
			if (this.IsRebornPortableBag)
			{
				newGoodIcon.BackSpriteName1 = "bagGridLock_bak";
			}
			else
			{
				newGoodIcon.BackSpriteName1 = ((120 <= index) ? "bagGridLock_bak" : "kaiqizhong");
			}
		}
		else
		{
			newGoodIcon.BackSpriteName1 = "bagGridLock_bak";
		}
		newGoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		newGoodIcon.Tag = index;
		return newGoodIcon;
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int dex)
	{
		int num = Global.MaxPortableGridNum / 4;
		int num2 = dex / 6 / 4;
		int num3 = dex % 24;
		int num4 = num3 % 6;
		int num5 = num3 / 6 % 4;
		return num4 + num5 * num + num2 * 6;
	}

	private void ClearIconEvents()
	{
		this.ItemsList.Clear();
		this.goodIdDict.Clear();
	}

	public GGoodIcon FindGoodsIconByDbID(int dbID)
	{
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			GoodsData goodsData = this.ItemsList[i].ItemObject as GoodsData;
			if (dbID == goodsData.Id)
			{
				return this.ItemsList[i];
			}
		}
		return null;
	}

	protected bool CanShowGoodsInPacel(GoodsData gd)
	{
		return gd != null && gd.Using <= 0 && gd.GCount > 0;
	}

	protected void SortGoodsBagIndex()
	{
		List<GoodsData> selfPortableGoods = this.SelfPortableGoods;
		selfPortableGoods.Sort(delegate(GoodsData x, GoodsData y)
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
		for (int i = 0; i < selfPortableGoods.Count; i++)
		{
			GoodsData goodsData = selfPortableGoods[i];
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
		this.SelfPortableGoods = list;
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
		return this.GenerateNewBagIndex(this.SelfPortableGoods, minIndex);
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
		for (int j = minIndex; j < Global.GetPortableBagCapacity(); j++)
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
		List<GoodsData> selfPortableGoods = this.SelfPortableGoods;
		for (int i = 0; i < selfPortableGoods.Count; i++)
		{
			GoodsData goodsData2 = selfPortableGoods[i];
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
		if (goodsData.BagIndex < this.CurrentSelectedPage * 216 || goodsData.BagIndex >= (this.CurrentSelectedPage + 1) * 216 || this.IsGoodsBagIndexRepeat(goodsData))
		{
			int num = this.GenerateNewBagIndexThanMinValue(this.CurrentSelectedPage * 216);
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

	public GGoodIcon LocationFristGoodsIcon()
	{
		if (this.SelfPortableGoods != null)
		{
			for (int i = 0; i < this.SelfPortableGoods.Count; i++)
			{
				GoodsData goodsData = this.SelfPortableGoods[i];
				GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(this.Getindex(goodsData.BagIndex));
				if (goodsIcon != null)
				{
					this.CurrentSelectedPage = goodsData.BagIndex / 24;
					this.UIDragPl.target.x = (float)(-468 * this.CurrentSelectedPage);
					this.UIDragPl.enabled = true;
					return goodsIcon;
				}
			}
		}
		this.RefreshBagPageText();
		this.CheckBagShowCount();
		return null;
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				GGoodIcon ggoodIcon = this.LocationFristGoodsIcon();
				if (null != ggoodIcon)
				{
					SystemHelpPart.SetMask(ggoodIcon, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.PortableBagPartGetGoods, HelpStateEvents.Clicked, 1);
				}
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.returnBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private const int bgRows = 4;

	private const int bgColumn = 6;

	public GButton zhengLiBtn;

	public GButton caoZuoBtn;

	public GButton cunQuMoneyBtn;

	public GButton returnBtn;

	public UILabel bindMoney;

	public UILabel Money;

	public GameObject goMoney;

	public GGoodsBox goodsBox;

	public Canvas baoGuoCanvas;

	public SpriteSL selectImage;

	public SpriteSL selectBaoGuoImage;

	public TextBlock Page;

	public UISprite _PageSp;

	public ListBox _PageListBox;

	private List<UISprite> m_ListPage = new List<UISprite>();

	private byte m_PageCount;

	public UISprite _BagBG;

	public SpringPanel UIDragPl;

	private int MaxPageCount;

	private List<GGoodIcon> ItemsList = new List<GGoodIcon>();

	private int BagSizeAPage = 60;

	private int CurrentSelectedPage;

	private PetAddBagNumsPart petAddBagNumsPart;

	private List<Image> DisableImagesList = new List<Image>();

	private List<GoodsData> OldGoodsArrayList = new List<GoodsData>();

	private Dictionary<int, int> goodIdDict = new Dictionary<int, int>();

	private bool FirstGetNewData = true;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private Canvas PlaceHolder;

	private NoBorderWindow MenuWindow;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	private ObservableCollection ItemCollection;

	private int wantToAddGridsNum = 1;

	public GameObject extWindow;

	public TextBlock needZSText;

	public TextBlock ExtGridNumText;

	public TextBlock needTimeText;

	public GButton kuozhangBtn;

	public GButton quxiaoBtn;

	public GButton closeBtn;

	public UISprite modalBak;

	private bool m_isRebornPortableBag;

	private bool _ForceShowNavi;

	private UISprite tempPaneStat;
}
