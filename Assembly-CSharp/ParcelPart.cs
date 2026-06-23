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

public class ParcelPart : UserControl
{
	public bool IsRebornParcel
	{
		get
		{
			return this.m_isRebornParcel;
		}
		set
		{
			this.m_isRebornParcel = value;
			if (this.m_isRebornParcel)
			{
				this.ChongShengCangKuBtn.Text = Global.GetLang("背包");
			}
			else
			{
				this.ChongShengCangKuBtn.Text = Global.GetLang("重生背包");
			}
		}
	}

	public int iBaoGuoMode
	{
		get
		{
			return this._iBaoGuoMode;
		}
		set
		{
			this._iBaoGuoMode = value;
			if (this._iBaoGuoMode == 0 || this._iBaoGuoMode == 5)
			{
				this.ShangjiaBtn.gameObject.SetActive(false);
				this.ChuShouBtn.gameObject.SetActive(true);
				this.ChuShouBtn.isEnabled = true;
				this.ChuShowAnim.SetActive(HintQueueIcon.HintBagFull);
				this.jingLingChuShouBtn.gameObject.SetActive(false);
				this.jingLingChuShouBtn.isEnabled = false;
				this.ChongShengCangKuBtn.gameObject.SetActive(false);
			}
			else
			{
				this.ChuShowAnim.SetActive(false);
				if (this._iBaoGuoMode == 7 || this._iBaoGuoMode == 8)
				{
					this.ChuShouBtn.gameObject.SetActive(false);
					this.ShangjiaBtn.gameObject.SetActive(true);
					if (ChongShengData.IsChongShengBgOpen())
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(true);
					}
					else
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(false);
					}
					if (this._iBaoGuoMode == 8)
					{
						float num = -146f;
						Vector3 localPosition = this.ShangjiaBtn.transform.localPosition;
						this.ShangjiaBtn.transform.localPosition = new Vector3(localPosition.x, num, localPosition.z);
						Vector3 localPosition2 = this.ZhengLiBtn.transform.localPosition;
						this.ChongShengCangKuBtn.transform.localPosition = new Vector3(localPosition.x + 135f, num, localPosition.z);
						this.ZhengLiBtn.transform.localPosition = new Vector3(localPosition.x + 270f, num, localPosition2.z);
						this.TextContainer.gameObject.SetActive(false);
						if (this.m_isRebornParcel)
						{
							this.ChongShengCangKuBtn.Text = Global.GetLang("背包");
						}
						else
						{
							this.ChongShengCangKuBtn.Text = Global.GetLang("重生背包");
						}
					}
					else
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(false);
					}
				}
				else if (this._iBaoGuoMode == 11 || this._iBaoGuoMode == 2)
				{
					if (ChongShengData.IsChongShengBgOpen())
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(true);
						this.ChuShouBtn.gameObject.SetActive(false);
						this.ChuShouBtn.isEnabled = false;
					}
					else
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(false);
						this.ChuShouBtn.gameObject.SetActive(true);
						this.ChuShouBtn.isEnabled = false;
					}
					if (this.m_isRebornParcel)
					{
						this.ChongShengCangKuBtn.Text = Global.GetLang("背包");
					}
					else
					{
						this.ChongShengCangKuBtn.Text = Global.GetLang("重生背包");
					}
				}
				else if (this._iBaoGuoMode == 3)
				{
					if (ChongShengData.IsChongShengBgOpen())
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(true);
						this.ChuShouBtn.gameObject.SetActive(false);
						this.ChuShouBtn.isEnabled = false;
					}
					else
					{
						this.ChongShengCangKuBtn.gameObject.SetActive(false);
						this.ChuShouBtn.gameObject.SetActive(true);
						this.ChuShouBtn.isEnabled = false;
					}
					if (this.m_isRebornParcel)
					{
						this.ChongShengCangKuBtn.Text = Global.GetLang("背包");
					}
					else
					{
						this.ChongShengCangKuBtn.Text = Global.GetLang("重生背包");
					}
				}
				else
				{
					this.ShangjiaBtn.gameObject.SetActive(false);
					this.ChuShouBtn.isEnabled = false;
					this.jingLingChuShouBtn.isEnabled = false;
				}
			}
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected virtual List<GoodsData> SelfParcelGoods
	{
		get
		{
			if (this.m_isRebornParcel)
			{
				return ChongShengData.GetChongShengGoodsDatas();
			}
			return Global.Data.roleData.GoodsDataList;
		}
		set
		{
			if (this.m_isRebornParcel)
			{
				Global.Data.roleData.RebornGoodsDataList = value;
			}
			else
			{
				Global.Data.roleData.GoodsDataList = value;
			}
		}
	}

	protected virtual int SelfBagNum
	{
		get
		{
			if (this.m_isRebornParcel)
			{
				return ChongShengData.GetChongShengBagNum();
			}
			return Global.Data.roleData.BagNum;
		}
	}

	protected int BagSite
	{
		get
		{
			if (this.m_isRebornParcel)
			{
				return 15000;
			}
			return 0;
		}
	}

	private void onDragFinished()
	{
		if (Math.Abs(Math.Abs(this.UIDragPl.transform.localPosition.x) - (float)(390 * this.CurrentSelectedPage)) > 30f)
		{
			if (this.UIDragPl.transform.localPosition.x > (float)(-390 * this.CurrentSelectedPage))
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
				if (this.CurrentSelectedPage >= 5)
				{
					this.CurrentSelectedPage = 4;
				}
			}
		}
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPageText();
	}

	private void InitTextInPrefabs()
	{
		this.ZhengLiBtn.Text = Global.GetLang("整 理");
		this.ChuShouBtn.Text = Global.GetLang("回 收");
		this.ShangjiaBtn.Text = Global.GetLang("出售金币");
		this.kuozhangBtn.Text = Global.GetLang("确定");
		this.quxiaoBtn.Text = Global.GetLang("取消");
		if (this.m_isRebornParcel)
		{
			this.ChongShengCangKuBtn.Text = Global.GetLang("背包");
		}
		else
		{
			this.ChongShengCangKuBtn.Text = Global.GetLang("重生背包");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		Super.goodDBIdDict.Clear();
		this.UIDragPl.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.ZhengLiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_isRebornParcel)
			{
				GameInstance.Game.SpriteResetChongShengBag();
			}
			else
			{
				GameInstance.Game.SpriteResetBag();
			}
		};
		this.ChuShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Huishou(false);
		};
		this.jingLingChuShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.JingLingHuiShou(false);
		};
		this.kuozhangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.extWindow.gameObject.SetActive(false);
			this.modalBak.gameObject.SetActive(false);
			this.gridCount = this.SelfBagNum;
			if (this.m_isRebornParcel)
			{
				GameInstance.Game.SpriteExtRebornBagNumByYuanBao((double)this.wantToAddGridsNum);
			}
			else
			{
				GameInstance.Game.SpriteExtBagNumByYuanBao((double)this.wantToAddGridsNum, 2, this.needZSText.Text.SafeToInt32(0));
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
		this.ChongShengCangKuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._iBaoGuoMode == 8)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = this._iBaoGuoMode
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this._iBaoGuoMode,
					IDType = this._iBaoGuoMode
				});
			}
		};
		this.ChongShengCangKuBtn.gameObject.SetActive(false);
		Vector3 localPosition = this.closeBtn.transform.localPosition;
		localPosition.x = 210f;
		localPosition.y = 154f;
		this.closeBtn.transform.localPosition = localPosition;
		this.closeBtn.Width = 52f;
		this.closeBtn.Height = 52f;
		this.closeBtn.transform.localScale = Vector3.one * 0.8f;
		this.ShangjiaBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 7,
					IDType = 7
				});
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 8,
					IDType = 8
				});
			}
		};
		this.goodsBox.RowCount = 4;
		this.goodsBox.ColCount = 25;
		this.goodsBox.InitBox();
		if (this.GuanLianItem != null)
		{
		}
	}

	public void JingLingHuiShou(bool isReset = false)
	{
	}

	public void Huishou(bool isReset = false)
	{
		int num;
		if (this.ChuShouBtn.Text == Global.GetLang("取 消") || isReset)
		{
			this.ChuShouBtn.Text = Global.GetLang("回收");
			num = 101;
			this.iBaoGuoMode = 0;
			this.BindTongQianText.gameObject.SetActive(true);
			this.YinLiangText.gameObject.SetActive(true);
			this.Money3Text.gameObject.SetActive(true);
			this.Money4Text.gameObject.SetActive(true);
			this.ChuShouBtn.normalSprite = this.BtnSpriteNames[0];
			this.ChuShouBtn.target.spriteName = this.BtnSpriteNames[0];
		}
		else
		{
			this.JingLingHuiShou(true);
			this.ChuShouBtn.Text = Global.GetLang("取 消");
			num = 10;
			this.iBaoGuoMode = 5;
			this.BindTongQianText.gameObject.SetActive(false);
			this.YinLiangText.gameObject.SetActive(false);
			this.Money3Text.gameObject.SetActive(false);
			this.Money4Text.gameObject.SetActive(false);
			this.ChuShouBtn.normalSprite = this.BtnSpriteNames[1];
			this.ChuShouBtn.target.spriteName = this.BtnSpriteNames[1];
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRoleDetailPart, HelpStateEvents.Clicked, -1);
		}
		if (num == 101)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = num,
					IDType = 0
				});
			}
		}
		else
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 10,
				IDType = 0
			});
		}
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int dex)
	{
		int num = 25;
		this.goodsBox.listBox.maxPerLine = num;
		int num2 = dex / 5 / 4;
		int num3 = dex % 20;
		int num4 = num3 % 5;
		int num5 = num3 / 5 % 4;
		return num4 + num5 * num + num2 * 5;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		if (this.iBaoGuoMode == 0 || this.iBaoGuoMode == 4 || this.iBaoGuoMode == 6 || this.iBaoGuoMode == 9)
		{
			GTipServiceEx.SelfBagOnly = true;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
		else if (this.iBaoGuoMode == 1)
		{
			if (this.isShowTips)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = 1,
					Tag = ggoodIcon.ItemObject
				});
			}
		}
		else if (this.iBaoGuoMode == 2)
		{
			if (this.isShowTips)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
			}
			else
			{
				Global.MoveGoodsToPortableBag(goodsData, 0);
			}
		}
		else if (this.iBaoGuoMode == 11)
		{
			if (this.isShowTips)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
			}
			else
			{
				Global.MoveGoodsToPortableChongShengBag(goodsData, 0);
			}
		}
		else if (this.iBaoGuoMode == 3)
		{
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
		else if (this.iBaoGuoMode == 5)
		{
			if (this.isShowTips)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = 5,
					Tag = ggoodIcon.ItemObject
				});
			}
		}
		else if (this.iBaoGuoMode == 7 || this.iBaoGuoMode == 8)
		{
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.StallTip, GoodsOwnerTypes.SelfStall, goodsData);
		}
		else if (this.iBaoGuoMode == 10)
		{
			GTipServiceEx.SelfBagOnly = true;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
	}

	protected void RefreshBagCapacityText()
	{
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public GGoodIcon FindGoodsIconByDbID(int dbID)
	{
		return this.goodsBox.GetGoodsIcon(this.goodsBox.FindByGoodsDbID(dbID));
	}

	public void RefreshGoods1(GoodsData gd)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (this.SelfParcelGoods != null)
		{
			int num = this.goodsBox.FindByGoodsDbID(gd.Id);
			if (gd.GCount <= 0 || gd.Using > 0 || gd.Site != this.BagSite)
			{
				if (num != -1)
				{
					this.goodsBox.RemoveGoodsIcon(num);
				}
				return;
			}
		}
		this.RefreshGoods3(gd);
		this.RefreshBagCapacityText();
	}

	public void RefreshGoods3(GoodsData gd)
	{
		int num = this.goodsBox.FindByGoodsDbID(gd.Id);
		if (num != -1)
		{
			GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(num);
			if (null != goodsIcon)
			{
				bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
				Super.InitGoodsGIcon(goodsIcon, gd, canUse, IconTextTypes.Qianghua);
				goodsIcon.RedTipObject.gameObject.SetActive(Super.ShowGoodRedTip(gd));
			}
		}
	}

	public void RefreshGoodsUsing()
	{
		if (this.SelfParcelGoods != null)
		{
			for (int i = 0; i < this.SelfParcelGoods.Count; i++)
			{
				GoodsData goodsData = this.SelfParcelGoods[i];
				if (goodsData != null && goodsData.GCount > 0 && goodsData.Using == 0 && goodsData.Site == this.BagSite)
				{
					Global.SetEquipGoodsZhanLiStat(this.goodsBox.GetGoodsIcon(this.Getindex(goodsData.BagIndex)), goodsData);
				}
			}
		}
	}

	public void RemoveGoodsRedPointByDelete(GoodsData dta)
	{
		GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(this.Getindex(dta.BagIndex));
		if (goodsIcon != null)
		{
			goodsIcon.RedTipObject.gameObject.SetActive(false);
		}
	}

	public void RefreshGoodsRedPoint(int lastGoods)
	{
		if (Global.Data.roleData != null && this.SelfParcelGoods != null)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsLimitData> itrGoodsLimitDataList = Global.Data.roleData.GoodsLimitDataList;
			if (itrGoodsLimitDataList != null && itrGoodsLimitDataList.Count > 0)
			{
				int i;
				for (i = 0; i < itrGoodsLimitDataList.Count; i++)
				{
					GoodsData goodsData = this.SelfParcelGoods.Find((GoodsData result) => result.GoodsID == itrGoodsLimitDataList[i].GoodsID);
					if (goodsData != null && !list.Contains(goodsData))
					{
						list.Add(goodsData);
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				for (int j = 0; j < list.Count; j++)
				{
					GoodsData goodsData2 = list[j];
					if (goodsData2 != null)
					{
						GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(this.Getindex(goodsData2.BagIndex));
						if (goodsIcon != null)
						{
							goodsIcon.RedTipObject.gameObject.SetActive(Super.ShowGoodRedTip(goodsData2));
						}
					}
				}
			}
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GGoodIcon icon = null;
		icon = Global.GetNewGoodIcon();
		icon.gameObject.AddComponent<UIDragPanelContents>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodsData.GoodsID)
		});
		icon.ItemCode = goodsData.GoodsID;
		icon.ItemObject = goodsData;
		icon.ItemCategory = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		if ((this._iBaoGuoMode == 5 || this._iBaoGuoMode == 1) && Super.goodDBIdDict.ContainsKey(goodsData.Id))
		{
			Super.SetBgGIconShouStat(icon, true);
		}
		if (this.iBaoGuoMode == 0 || this.iBaoGuoMode == 1 || this.iBaoGuoMode == 2)
		{
			icon.RedTipObject.gameObject.SetActive(Super.ShowGoodRedTip(goodsData));
		}
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
			if (ev.IDType == 1)
			{
				Global.ToUseGoods(icon.ItemObject as GoodsData, true, false);
			}
			else if (ev.IDType != 2)
			{
				if (ev.IDType == 4)
				{
					if (!this.m_isRebornParcel)
					{
						if (Global.GetZhuoyueAttributeCount(icon.ItemObject as GoodsData) > 0)
						{
							Super.HintMainText(Global.GetLang("卓越及以上装备可回收获得魔晶!"), 10, 3);
							return;
						}
						string text = (icon.ItemObject as GoodsData).Id.ToString();
						if (string.Empty != text)
						{
							GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID((icon.ItemObject as GoodsData).GoodsID);
							if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao) && goodsXmlNodeByID.SuitID >= Global.ShenqiZaizaoSuit)
							{
								Super.HintMainText(string.Format(Global.GetLang("需要开启【{0}】系统才能回收"), GongnengYugaoMgr.GetGongNengName(GongNengIDs.ZaiZao)), 10, 3);
								return;
							}
							if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(3, text);
							}
							else
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(1, text);
							}
						}
					}
					else
					{
						string goodsDbIds = (icon.ItemObject as GoodsData).Id.ToString();
						GameInstance.Game.SpriteChongShengOneKeyQuickSaleOut(goodsDbIds);
					}
				}
				else if (ev.IDType == 7)
				{
					Global.ToUseGoods(icon.ItemObject as GoodsData, true, false);
				}
				else if (ev.IDType == 10)
				{
					if (ev.ID > 0)
					{
						GoodsData goodsData2 = icon.ItemObject as GoodsData;
						if (Global.CanAddGoods(goodsData2.GoodsID, ev.ID, goodsData2.Binding, goodsData2.Endtime, false))
						{
							GameInstance.Game.SpriteSplitGoods(goodsData2.Id, goodsData2.Site, goodsData2.GoodsID, ev.ID);
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再拆分物品..."), new object[0]), 1, -1, -1, 0);
						}
					}
				}
				else if (ev.IDType == 5)
				{
					if (this.iBaoGuoMode == 1)
					{
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = -1,
								IDType = 1,
								Tag = icon.ItemObject
							});
						}
					}
					else if (this.iBaoGuoMode == 2)
					{
						Global.MoveGoodsToPortableBag(icon.ItemObject as GoodsData, 0);
					}
					else if (this.iBaoGuoMode == 11)
					{
						Global.MoveGoodsToPortableChongShengBag(icon.ItemObject as GoodsData, 0);
					}
					else if (this.iBaoGuoMode == 3)
					{
						GoodsData goodsData3 = icon.ItemObject as GoodsData;
						if (goodsData3.Site == this.BagSite && goodsData3.Using <= 0)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData3.GoodsID);
							if (goodsXmlNodeByID2 != null)
							{
								int categoriy = goodsXmlNodeByID2.Categoriy;
								bool flag = Global.IsRebornEquip(categoriy);
								if (flag && ChongShengData.beContainBaoShi(goodsData3))
								{
									Super.HintMainText(Global.GetLang("装备中镶嵌宝石，不能交易"), 10, 3);
									return;
								}
							}
							if (goodsData3.Binding > 0)
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("已经绑定的物品不能交易"), 0, -1, -1, 0);
								return;
							}
							if (Global.IsTimeLimitGoods(goodsData3))
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("限时的物品不能交易"), 0, -1, -1, 0);
								return;
							}
							if (this.DPSelectedItem != null)
							{
								this.DPSelectedItem(this, new DPSelectedItemEventArgs
								{
									ID = -1,
									IDType = 3,
									Tag = icon.ItemObject
								});
							}
						}
					}
					else if (this.iBaoGuoMode == 5 && this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = -1,
							IDType = 5,
							Tag = icon.ItemObject
						});
					}
				}
				else if (ev.IDType == 15)
				{
					GameInstance.Game.SpriteModGoods(3, goodsData.Id, goodsData.GoodsID, goodsData.Using, 5000, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
				else if (ev.IDType == 17)
				{
					GameInstance.Game.SpriteModGoods(3, goodsData.Id, goodsData.GoodsID, goodsData.Using, 10000, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
				else if (ev.IDType == 22)
				{
					if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
					{
						return;
					}
					GameInstance.Game.SpriteModGoods(3, goodsData.Id, goodsData.GoodsID, goodsData.Using, 13000, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
		};
		return icon;
	}

	public void RefreshGoods2(GoodsData gd = null, bool isInit = false, bool isStep = true)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (null != base.gameObject && !base.gameObject.activeSelf)
		{
			return;
		}
		if (gd == null)
		{
			if (this.SelfParcelGoods != null)
			{
				this.SortGoodsBagIndex();
			}
			base.StartCoroutine<bool>(this.ShowPageEx(0, isStep));
		}
		else
		{
			GGoodIcon ggoodIcon = this.AddIcon(gd);
			this.goodsBox.SetGoodsIcon(this.GetGoodsIconIndex(gd), ggoodIcon);
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			Global.SetEquipGoodsZhanLiStat(ggoodIcon, gd);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
		this.RefreshBagCapacityText();
	}

	public void RefreshMoney()
	{
		this.BindTongQianText.Text = Global.Data.roleData.Money1.ToString();
		this.Money3Text.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
		this.YinLiangText.Text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.Money4Text.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.Gold
		});
	}

	public void RefreshYinLiang()
	{
		this.YinLiangText.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.YinLiang
		});
	}

	public void RefreshSaleGoodsPart(GoodsData goodData, bool toAdd = true)
	{
		if (null != this.saleGoodsPart)
		{
			this.saleGoodsPart.RefreshData(goodData, toAdd);
		}
	}

	public void NotifySaleGoodsPartResult(int result)
	{
		if (null != this.saleGoodsPart)
		{
			this.saleGoodsPart.NotifyError(result);
		}
	}

	public void InitPartData()
	{
		Global.InitBagParams();
		this.CurrentSelectedPage = 0;
		this.RefreshGoods2(null, true, true);
		this.BindTongQianText.Text = Global.Data.roleData.Money1.ToString();
		this.Money3Text.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
		this.YinLiangText.Text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.Money4Text.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.Gold
		});
		if (0 < SystemHelpMgr.ActiveHintID)
		{
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
		}
	}

	public void CheckInitedPartData()
	{
		if (!this.IsItemFinished)
		{
			if (this.IndexItemFinished < 0)
			{
				this.IndexItemFinished = 0;
			}
			base.StartCoroutine<bool>(this.ShowPageEx(this.IndexItemFinished, true));
		}
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 14)
		{
			return;
		}
		if (clickGetThingEventArgs.Cancel)
		{
			this.DestroyIcon.Text = Global.GetLang("摧毁");
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		GIcon gicon = sender as GIcon;
		if (gicon.BoxTypes != 1)
		{
			return;
		}
		GoodsData gd = gicon.ItemObject as GoodsData;
		if (gd != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				if (Super.CanSaleOutGoods(gd))
				{
					if (!Super.CanDirectSaleOutGoods(gd))
					{
						string title = goodsXmlNodeByID.Title;
						GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("确认要摧毁{0}吗？"), new object[]
						{
							title
						}), (int)((this.Width - 253.0) / 2.0), (int)((this.Height - 171.0) / 2.0), (int)this.Width, (int)this.Height, 0.01, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								Global.DestroyGoods(gd);
							}
							return true;
						};
					}
					else
					{
						Global.DestroyGoods(gd);
					}
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此物品不可摧毁"), new object[0]), 0, -1, -1, 0);
				}
			}
		}
		clickGetThingEventArgs.NextClick = true;
	}

	public void OpenAgain()
	{
		this.HintGiftUpLevelGoods();
		this.HintBagFull();
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.DestroyIcon.Text = Global.GetLang("摧  毁");
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("背包UI"), null);
		ObjectClickGetingMgr.CancelClickGetThing(14);
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void ShowDisableGrid()
	{
		int num = 100;
		for (int i = this.SelfBagNum; i < num; i++)
		{
			GGoodIcon ggoodIcon = this.ShowDisableGGoodIcon(i, false);
			this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.OnClickDisableGrid));
		}
		this.SetFristDisableGrid();
	}

	private void SetFristDisableGrid()
	{
		if (this.SelfBagNum < Global.MaxBagGridNum)
		{
			GGoodIcon ggoodIcon = this.ShowDisableGGoodIcon(this.SelfBagNum, true);
			this.goodsBox.SetGoodsIcon(this.Getindex(this.SelfBagNum), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.OnClickDisableGrid));
		}
	}

	private GGoodIcon ShowDisableGGoodIcon(int index, bool bFirst = false)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		if (bFirst)
		{
			if (this.IsRebornParcel)
			{
				ggoodIcon.BackSpriteName1 = "bagGridLock_bak";
			}
			else
			{
				ggoodIcon.BackSpriteName1 = "kaiqizhong";
			}
		}
		else
		{
			ggoodIcon.BackSpriteName1 = "bagGridLock_bak";
		}
		ggoodIcon.Tag = index;
		return ggoodIcon;
	}

	private void OnClickDisableGrid(MouseEvent evt)
	{
		int num = this.SelfBagNum / 20;
		if (this.CurrentSelectedPage < num)
		{
			return;
		}
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		int num2 = (int)ggoodIcon.Tag;
		if (num2 < 0)
		{
			return;
		}
		this.wantToAddGridsNum = num2 - this.SelfBagNum + 1;
		if (this.m_isRebornParcel)
		{
			this.ShowRebornNeedWindow();
		}
		else
		{
			GameInstance.Game.SpriteQueryOpenGridTickCmd();
		}
	}

	public void ShowExtWindow(int onlineExtTime)
	{
		BaoGuoMode mode = BaoGuoMode.None;
		if (this.IsRebornParcel)
		{
			mode = BaoGuoMode.horseCangKu;
		}
		int num;
		int extBagGridNeedYuanBao = Global.GetExtBagGridNeedYuanBao(this.wantToAddGridsNum, onlineExtTime, mode, out num);
		this.needZSText.Text = ((extBagGridNeedYuanBao >= 0) ? extBagGridNeedYuanBao.ToString() : "0");
		this.ExtGridNumText.Text = this.wantToAddGridsNum.ToString();
		int num2 = num;
		num2 = ((num2 >= 0) ? num2 : 0);
		this.needTimeText.Text = string.Format(Global.GetLang("{0}小时{1}分钟"), num2 / 3600, num2 % 3600 / 60);
		this.extWindow.gameObject.SetActive(true);
		this.modalBak.gameObject.SetActive(true);
		Vector3 localPosition = this.modalBak.transform.localPosition;
		localPosition.z = -2.25f;
		this.modalBak.transform.localPosition = localPosition;
		Vector3 localPosition2 = this.extWindow.transform.localPosition;
		localPosition2.z = -2.35f;
		this.extWindow.transform.localPosition = localPosition2;
	}

	public void ShowRebornNeedWindow()
	{
		BaoGuoMode mode = BaoGuoMode.None;
		if (this.IsRebornParcel)
		{
			mode = BaoGuoMode.horseCangKu;
		}
		int rebornBagGridNeedYuanBao = Global.GetRebornBagGridNeedYuanBao(this.wantToAddGridsNum, mode);
		this.needZSText.Text = ((rebornBagGridNeedYuanBao >= 0) ? rebornBagGridNeedYuanBao.ToString() : "0");
		this.ExtGridNumText.Text = this.wantToAddGridsNum.ToString();
		this.needTimeText.Text = string.Empty;
		Transform transform = this.extWindow.transform.FindChild("bak");
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

	public void ExpandGrid()
	{
		for (int i = this.gridCount; i < this.SelfBagNum; i++)
		{
			this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
		}
		this.SetFristDisableGrid();
		this.gridCount = this.SelfBagNum;
	}

	protected bool CanShowGoodsInPacel(GoodsData gd)
	{
		return gd != null && gd.Using <= 0 && gd.GCount > 0;
	}

	protected void SortGoodsBagIndex()
	{
		List<GoodsData> selfParcelGoods = this.SelfParcelGoods;
		selfParcelGoods.Sort(delegate(GoodsData x, GoodsData y)
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
		for (int i = 0; i < selfParcelGoods.Count; i++)
		{
			GoodsData goodsData = selfParcelGoods[i];
			if (goodsData.GoodsID == 1040605 && goodsData.BagIndex == -1)
			{
				goodsData.BagIndex = 0;
			}
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
			GoodsData goodsData3 = list3[k];
			list.Add(goodsData3);
		}
		this.SelfParcelGoods = list;
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
		List<GoodsData> selfParcelGoods = this.SelfParcelGoods;
		return this.GenerateNewBagIndex(selfParcelGoods, minIndex);
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
		for (int j = minIndex; j < Global.GetTotalMaxBagGridCount(); j++)
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
		List<GoodsData> selfParcelGoods = this.SelfParcelGoods;
		for (int i = 0; i < selfParcelGoods.Count; i++)
		{
			GoodsData goodsData2 = selfParcelGoods[i];
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
		if (goodsData.BagIndex < this.CurrentSelectedPage * 100 || goodsData.BagIndex >= (this.CurrentSelectedPage + 1) * 100 || this.IsGoodsBagIndexRepeat(goodsData))
		{
			int num = this.GenerateNewBagIndexThanMinValue(0);
			if (num < 0)
			{
				num = this.GenerateNewBagIndexThanMinValue(0);
			}
			if (num >= 0 && num != goodsData.BagIndex)
			{
				goodsData.BagIndex = num;
			}
		}
	}

	public IEnumerator ShowPageEx(int startIndex = 0, bool isStep = false)
	{
		this.MaxPageCount = Global.MaxBagGridNum / 100;
		Dictionary<int, int> indexDict = new Dictionary<int, int>();
		if (this.SelfParcelGoods != null)
		{
			int count = this.SelfParcelGoods.Count;
			for (int subIndex = 0; subIndex < count; subIndex++)
			{
				GoodsData gd = this.SelfParcelGoods[subIndex];
				if (gd != null && gd.GCount > 0 && gd.Using == 0 && gd.Site == this.BagSite)
				{
					indexDict.Add(gd.BagIndex, subIndex);
				}
			}
		}
		int counter = 0;
		for (int i = startIndex; i < this.SelfBagNum; i++)
		{
			counter++;
			int dataIndex = -1;
			if (indexDict.TryGetValue(i, ref dataIndex))
			{
				GoodsData gd2 = this.SelfParcelGoods[dataIndex];
				GGoodIcon icon = this.AddIcon(gd2);
				this.goodsBox.SetGoodsIcon(this.Getindex(i), icon);
				icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				bool canUseGoods = Global.CanUseGoods(gd2.GoodsID, false, true);
				Super.InitGoodsGIcon(icon, gd2, canUseGoods, IconTextTypes.Qianghua);
				if (this.iBaoGuoMode == 0 || this.iBaoGuoMode == 1 || this.iBaoGuoMode == 2)
				{
					icon.RedTipObject.gameObject.SetActive(Super.ShowGoodRedTip(gd2));
				}
				if (gd2.GCount > 0 && gd2.Using == 0 && gd2.Site == this.BagSite)
				{
					Global.SetEquipGoodsZhanLiStat(icon, gd2);
				}
			}
			else
			{
				this.goodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
			this.IndexItemFinished = i;
			if (isStep && counter % 5 == 0)
			{
				yield return null;
			}
		}
		this.IndexItemFinished = this.SelfBagNum - 1;
		if (isStep)
		{
			yield return null;
		}
		this.ShowDisableGrid();
		this.IsItemFinished = true;
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPageText();
		yield break;
	}

	public void RefreshGoodsStates()
	{
	}

	public void HintGiftUpLevelGoods()
	{
		int num = Super.CanOpenUpLevelGiftBag();
		if (num <= 0)
		{
			return;
		}
	}

	public void RefreshGold()
	{
		this.Money4Text.Text = Global.Data.roleData.Gold.ToString();
	}

	public void HintBagFull()
	{
		if (Global.IsBagFull())
		{
		}
	}

	public GGoodIcon LocationGoodsIcon(int goodsID, bool isZuoyue = true)
	{
		if (this.SelfParcelGoods != null)
		{
			for (int i = 0; i < this.SelfParcelGoods.Count; i++)
			{
				GoodsData goodsData = this.SelfParcelGoods[i];
				if (goodsID == goodsData.GoodsID)
				{
					if (!isZuoyue)
					{
						GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(this.Getindex(goodsData.BagIndex));
						if (goodsIcon != null)
						{
							this.CurrentSelectedPage = goodsData.BagIndex / 20;
							this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
							this.UIDragPl.enabled = true;
						}
						return goodsIcon;
					}
					if (Global.GetZhuoyueAttributeCount(goodsData) > 0)
					{
						GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(this.Getindex(goodsData.BagIndex));
						if (goodsIcon != null)
						{
							this.CurrentSelectedPage = goodsData.BagIndex / 20;
							this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
							this.UIDragPl.enabled = true;
						}
						return goodsIcon;
					}
				}
			}
		}
		this.RefreshBagPageText();
		return null;
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 701 || id == 702)
			{
				SystemHelpPart.SetMask(this.ChuShouBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private const int bgRows = 4;

	private const int bgColumn = 5;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	public GGoodsBox goodsBox;

	public GameObject TextContainer;

	public TextBlock BindTongQianText;

	public TextBlock YinLiangText;

	public TextBlock Money3Text;

	public TextBlock Money4Text;

	public TextBlock Page;

	public GButton ZhengLiBtn;

	public GButton ChuShouBtn;

	public GButton ChongShengCangKuBtn;

	public GButton jingLingChuShouBtn;

	public GButton ShangjiaBtn;

	public SpringPanel UIDragPl;

	public SpriteSL ModalPart;

	public UISprite[] Pages;

	private List<GGoodIcon> ItemsList = new List<GGoodIcon>();

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private int[] goodsMenuItemIDs = new int[]
	{
		default(int),
		1,
		2,
		5
	};

	private GIcon DestroyIcon;

	private ObservableCollection Items;

	private Canvas PlaceHolder;

	private SaleGoodsPart saleGoodsPart;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private Canvas Root;

	public DPSelectedItemEventHandler GuanLianItem;

	public Dictionary<int, int> goodIdDict = new Dictionary<int, int>();

	public bool isShowTips = true;

	private int _iBaoGuoMode = -1;

	private int wantToAddGridsNum = 1;

	public GameObject extWindow;

	public TextBlock needZSText;

	public TextBlock ExtGridNumText;

	public TextBlock needTimeText;

	public GButton kuozhangBtn;

	public GButton quxiaoBtn;

	public GButton closeBtn;

	public UISprite modalBak;

	private int gridCount;

	public bool IsItemFinished;

	public int IndexItemFinished = -1;

	public GameObject ChuShowAnim;

	private bool m_isRebornParcel;

	private string[] BtnSpriteNames = new string[]
	{
		"tongyongBtn3_normal",
		"tongyongBtn6_normal"
	};

	private UISprite tempPaneStat;
}
