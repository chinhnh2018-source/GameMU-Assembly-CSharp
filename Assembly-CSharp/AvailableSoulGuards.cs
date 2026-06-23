using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class AvailableSoulGuards : UserControl
{
	private void InitTextInPrefabs()
	{
		this.equipBtn.Text = Global.GetLang("装备");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.panelPos = this.bagPanel.transform.localPosition;
		this.panelClip = this.bagPanel.clipRange;
		this.equipBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Data = this.currentGoodIcon.ItemObject
			});
		};
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.dragPanel.onDragFinished = delegate()
		{
			float num = Mathf.Round(this.bagPanel.clipRange.y / 78f) * 78f;
			SpringPanel.Begin(this.bagPanel.gameObject, new Vector3(-8f, -num, 0f), 10f);
		};
		if (null != this.soulItemGoodsIcon)
		{
			this.soulItemGoodsIcon.Width = 64.0;
			this.soulItemGoodsIcon.Height = 64.0;
			this.soulItemGoodsIcon.OutSizeX = 78;
			this.soulItemGoodsIcon.OutSizeY = 78;
			this.soulItemGoodsIcon.BackSpriteName0 = "bagGrid4_bak";
		}
	}

	protected override void OnDestroy()
	{
	}

	public void InitAvailableSouls(List<GoodsData> soulList)
	{
		if (soulList != null && soulList.Count > 0)
		{
			int num = (soulList.Count + 3 - 1) / 3;
			this.rowsInPage = ((num <= 4) ? 4 : num);
		}
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			this.background.localScale = new Vector3(234f, (float)(78 * this.rowsInPage), 0f);
			this.background.transform.localPosition = new Vector3(-39f, (float)(-(float)(78 * this.rowsInPage / 2 - 39)), 0f);
			this.dragPanel.dragEffect = 2;
			this.dragPanel.momentumAmount = 35f;
			this.dragPanel.scale = Vector3.up;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			this.background.localScale = new Vector3(0f, (float)(78 * this.rowsInPage), 0f);
			this.background.transform.localPosition = new Vector3(-39f, (float)(-(float)(78 * this.rowsInPage / 2 - 39)), 0f);
			this.dragPanel.dragEffect = 0;
			this.dragPanel.momentumAmount = 0f;
			this.dragPanel.scale = Vector3.right;
		}
		this.goodsBox.RowCount = this.rowsInPage;
		this.goodsBox.ColCount = 3;
		this.goodsBox.InitBox();
		if (soulList == null || soulList.Count <= 0)
		{
			return;
		}
		this.RefreshSoulGuardList(soulList);
		this.currentGoodIcon = this.goodsBox.GetGoodsIcon(0);
		GoodsData goodsData = soulList[0];
		this.SetIconHighlight(this.currentGoodIcon);
		this.lastIcon = this.currentGoodIcon;
		this.RefreshSelectedIcon(goodsData);
		this.SelectIcon(this.currentGoodIcon);
	}

	private void RefreshSoulGuardList(List<GoodsData> goodsDataList)
	{
		if (goodsDataList == null || goodsDataList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < goodsDataList.Count; i++)
		{
			GoodsData goodsData = goodsDataList[i];
			GGoodIcon ggoodIcon = this.AddIcon(goodsData, null);
			this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
		}
	}

	private void RefreshSelectedIcon(GoodsData goodsData)
	{
		if (null == this.soulItemGoodsIcon)
		{
			return;
		}
		if (null != this.soulItemGoodsIcon)
		{
			this.soulItemGoodsIcon.BackSpriteName0 = "bagGrid4_bak";
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				this.soulItemGoodsIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode)), false, 0);
			}
			this.SetSoulName(goodsXmlNodeByID.Title);
		}
	}

	private void SetSoulName(string name)
	{
		if (null != this.soulName)
		{
			this.soulName.Text = name;
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
			};
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		this.SetIconHighlight(ggoodIcon);
		this.lastIcon = ggoodIcon;
		this.currentGoodIcon = ggoodIcon;
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		this.RefreshSelectedIcon(goodsData);
		this.SelectIcon(ggoodIcon);
	}

	private void SetIconHighlight(GGoodIcon icon)
	{
		if (null != this.lastIcon)
		{
			this.lastIcon.BackSpriteName1 = "none";
		}
		if (null != icon)
		{
			icon.BackSpriteName1 = "iconState_highlight";
		}
	}

	private void SelectIcon(GGoodIcon icon)
	{
		if (null == icon)
		{
			return;
		}
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		this.baseValue.text = Global.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, false, 0);
	}

	private int SortGoodsDataList(SoulRecyclingItem x, SoulRecyclingItem y)
	{
		return x.recyclingPoint - y.recyclingPoint;
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			result = bagIndex;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			int num = 3;
			int num2 = this.rowsInPage;
			int num3 = 0 / this.rowsInPage;
			this.goodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	private const int columnsInPage = 3;

	private const int columns = 0;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton equipBtn;

	public GButton closeBtn;

	public GGoodsBox goodsBox;

	public GGoodIcon soulItemGoodsIcon;

	private GGoodIcon currentGoodIcon;

	public UIPanel bagPanel;

	public UIDraggablePanel dragPanel;

	private Vector3 panelPos = Vector3.zero;

	private Vector4 panelClip = Vector4.zero;

	public Transform background;

	public TextBlock soulName;

	public TextBlock baseValue;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	private bool isPage;

	private int rowsInPage = 4;

	private GGoodIcon lastIcon;
}
