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

public class DiamondBag : UserControl
{
	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	private void onDragFinished()
	{
		int num = 11;
		int num2 = 390;
		if (Math.Abs(Math.Abs(this.springPanel.transform.localPosition.x) - (float)(num2 * this.currentSelectedPage)) > 30f)
		{
			if (this.springPanel.transform.localPosition.x > (float)(-(float)num2 * this.currentSelectedPage))
			{
				this.currentSelectedPage--;
				if (this.currentSelectedPage <= 0)
				{
					this.currentSelectedPage = 0;
				}
			}
			else
			{
				this.currentSelectedPage++;
				if (this.currentSelectedPage >= num)
				{
					this.currentSelectedPage = num - 1;
				}
			}
		}
		this.springPanel.target.x = (float)(-(float)num2 * this.currentSelectedPage);
		this.springPanel.enabled = true;
		this.RefreshBagPageText();
	}

	private void InitTextInPrefabs()
	{
		this.resortBtn.Text = Global.GetLang("整 理");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.springPanel.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.resortBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SortDiamondBagRequest();
		};
		this.goodsBox.RowCount = 4;
		this.goodsBox.ColCount = 55;
		this.goodsBox.InitBox();
		this.RefreshBagPageText();
	}

	public void InitBag()
	{
		Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
		if (bagDiamondList == null || bagDiamondList.Count <= 0)
		{
			base.StartCoroutine<bool>(this.ClearBag());
			return;
		}
		base.StartCoroutine<bool>(this.RefreshGoodsList(bagDiamondList));
	}

	public override void Destroy()
	{
	}

	private void RefreshBagPageText()
	{
		if (this.tempPaneStat != null)
		{
			this.tempPaneStat.spriteName = "normal_page";
		}
		this.Pages[this.currentSelectedPage].spriteName = "select_page";
		this.tempPaneStat = this.Pages[this.currentSelectedPage];
	}

	private IEnumerator RefreshGoodsList(Dictionary<int, GoodsData> dic_goods)
	{
		yield return base.StartCoroutine<bool>(this.ClearBag());
		if (dic_goods == null || dic_goods.Count <= 0)
		{
			yield break;
		}
		int counter = 0;
		foreach (KeyValuePair<int, GoodsData> keyValuePair in dic_goods)
		{
			int bagIndex = keyValuePair.Key;
			GoodsData gd = null;
			if (dic_goods.TryGetValue(bagIndex, ref gd) && gd != null)
			{
				GGoodIcon icon = this.AddIcon(gd, null);
				this.goodsBox.SetGoodsIcon(this.Getindex(bagIndex), icon);
				icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
				if (null != icon.GetComponent<UIPanel>())
				{
					Object.Destroy(icon.GetComponent<UIPanel>());
				}
				counter++;
				if (counter % 5 == 0)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	private IEnumerator ClearBag()
	{
		int counter = 0;
		for (int i = 0; i < 220; i++)
		{
			GGoodIcon icon = this.AddEmptyIcon();
			this.goodsBox.SetGoodsIcon(this.Getindex(i), icon);
			counter++;
			if (counter % 100 == 0)
			{
				yield return null;
			}
		}
		yield break;
	}

	public void AddGoods(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		this.ReplaceGoodsIconWithType(gd, BagModifyType.BagModifyType_Replace);
	}

	public void RemoveGoods(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		this.ReplaceGoodsIconWithType(gd, BagModifyType.BagModifyType_Destroy);
	}

	public void ReplaceGoods(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		this.ReplaceGoodsIconWithType(gd, BagModifyType.BagModifyType_Replace);
	}

	private void ReplaceGoodsIconWithType(GoodsData gd, BagModifyType type)
	{
		if (gd == null)
		{
			return;
		}
		GoodsData bagDiamondGoodsDataAtIndex = Global.GetBagDiamondGoodsDataAtIndex(gd.BagIndex);
		GGoodIcon icon;
		if (bagDiamondGoodsDataAtIndex != null)
		{
			icon = this.AddIcon(gd, null);
		}
		else
		{
			icon = this.AddEmptyIcon();
		}
		this.ReplaceGoodsIconAtIndex(gd.BagIndex, icon);
	}

	private void ReplaceGoodsIconAtIndex(int bagIndex, GGoodIcon icon)
	{
		if (null != this.goodsBox)
		{
			int num = this.Getindex(bagIndex);
			if (null != icon)
			{
				this.goodsBox.SetGoodsIcon(num, icon);
				icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			}
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.TextColor = 15793920U;
			ggoodIcon.ContentText.Text = "Lv" + Global.GetDiamondLevelByGoodsID(goodsData.GoodsID);
			int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(goodsData.GoodsID);
			if (diamondLevelByGoodsID >= 2 && diamondLevelByGoodsID <= 3)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
			}
			if (diamondLevelByGoodsID >= 4 && diamondLevelByGoodsID <= 5)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
			}
			if (diamondLevelByGoodsID >= 6 && diamondLevelByGoodsID <= 7)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
			}
			if (diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 12)
			{
				ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
				ggoodIcon.TeXiao.gameObject.SetActive(true);
			}
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 3)
				{
					this.UplevelDiamond(this.currentSelectedGoodsData);
				}
				else if (ev.IDType == 19)
				{
					this.PulverizeDiamond(this.currentSelectedGoodsData);
				}
			};
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private GGoodIcon AddEmptyIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		this.currentGoodIcon = ggoodIcon;
		if (null != ggoodIcon)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			this.currentSelectedGoodsData = goodsData;
			string text = UIHelper.FormatGoodsName(goodsData, false, false, false);
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.FluorescentDiamondBagTip, GoodsOwnerTypes.FluorescentDiamondBag, goodsData);
		}
	}

	private int Getindex(int bagIndex)
	{
		int num = 5;
		int num2 = 4;
		int num3 = 55;
		this.goodsBox.listBox.maxPerLine = num3;
		int num4 = bagIndex / num / num2;
		int num5 = bagIndex % (num * num2);
		int num6 = num5 % num;
		int num7 = num5 / num % num2;
		return num6 + num7 * num3 + num4 * num;
	}

	private void UplevelDiamond(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		GoodsData targetGoodsData = Global.CloneGoodsData(gd, false);
		PlayZone.GlobalPlayZone.ShowDiamondUplevelWindow(targetGoodsData, 0, null);
	}

	private void PulverizeDiamond(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(gd.GoodsID);
		if (diamondLevelByGoodsID < 6 || 0 != Super.MessageBoxIsHint[11])
		{
			this.PulverizeFluorescentDiamond(gd);
			return;
		}
		GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("此道具比较贵重，是否确认分解？"), 2, null, MessBoxIsHintTypes.PulverizeDiamondEventHint);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				this.PulverizeFluorescentDiamond(gd);
			}
			return true;
		};
	}

	private void PulverizeFluorescentDiamond(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		if (gd.GCount <= 1)
		{
			this.PulverizeDiamondRequest(gd);
			return;
		}
		GoodsData targetGoodsData = Global.CloneGoodsData(gd, false);
		PlayZone.GlobalPlayZone.ShowDiamondPulverizationWindow(targetGoodsData);
	}

	public void SetBagSortResult(Dictionary<int, GoodsData> dic_diamond)
	{
		Global.SetBagDiamondList(dic_diamond);
		base.StartCoroutine<bool>(this.RefreshGoodsList(dic_diamond));
	}

	private void SortDiamondBagRequest()
	{
		GameInstance.Game.SortDiamondBag();
	}

	private void PulverizeDiamondRequest(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		GameInstance.Game.PulverizeDiamond(gd.BagIndex, gd.GCount);
	}

	private const int maxGridCount = 220;

	private const int rowsInPage = 4;

	private const int columnsInPage = 5;

	private const int bagSizeAPage = 220;

	private const int aGridSize = 78;

	public GButton resortBtn;

	public GGoodsBox goodsBox;

	public UISprite[] Pages;

	public SpringPanel springPanel;

	public UIPanel bagPanel;

	public UIDraggablePanel dragPanel;

	private int currentSelectedPage;

	private GGoodIcon currentGoodIcon;

	private GoodsData currentSelectedGoodsData;

	private UISprite tempPaneStat;
}
