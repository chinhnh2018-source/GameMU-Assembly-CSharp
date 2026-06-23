using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class AvailableGoods : UserControl
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
		if (null != this.itemGoodsIcon)
		{
			this.itemGoodsIcon.Width = 64.0;
			this.itemGoodsIcon.Height = 64.0;
			this.itemGoodsIcon.OutSizeX = 78;
			this.itemGoodsIcon.OutSizeY = 78;
			this.itemGoodsIcon.BackSpriteName0 = "bagGrid4_bak";
		}
	}

	protected override void OnDestroy()
	{
	}

	public GoodsType goodsType
	{
		set
		{
			this.goodType = value;
			GoodsType goodsType = this.goodType;
			if (goodsType == GoodsType.GoodsType_FluorescentDiamond)
			{
				this.equipBtn.Text = Global.GetLang("镶嵌");
			}
		}
	}

	public void InitAvailableGoods(List<GoodsData> goodsList)
	{
		if (goodsList != null && goodsList.Count > 0)
		{
			int num = (goodsList.Count + 3 - 1) / 3;
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
		if (goodsList == null || goodsList.Count <= 0)
		{
			return;
		}
		this.RefreshGoodsList(goodsList);
		this.currentGoodIcon = this.goodsBox.GetGoodsIcon(0);
		GoodsData goodsData = goodsList[0];
		this.SetIconHighlight(this.currentGoodIcon);
		this.lastIcon = this.currentGoodIcon;
		this.RefreshSelectedIcon(goodsData);
		this.SelectIcon(this.currentGoodIcon);
	}

	private void RefreshGoodsList(List<GoodsData> goodsDataList)
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
			if (null != ggoodIcon.GetComponent<UIPanel>())
			{
				Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
			}
		}
	}

	private void RefreshSelectedIcon(GoodsData goodsData)
	{
		if (null == this.itemGoodsIcon)
		{
			return;
		}
		if (null != this.itemGoodsIcon)
		{
			this.itemGoodsIcon.BackSpriteName0 = "bagGrid4_bak";
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				this.itemGoodsIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode)), false, 0);
				if (goodsData.Site == 7000)
				{
					this.itemGoodsIcon.TextColor = 15793920U;
					this.itemGoodsIcon.ContentText.Text = "Lv" + Global.GetDiamondLevelByGoodsID(goodsData.GoodsID);
					int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(goodsData.GoodsID);
					this.itemGoodsIcon.TeXiao.gameObject.SetActive(diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 10);
					if (diamondLevelByGoodsID >= 2 && diamondLevelByGoodsID <= 3)
					{
						this.itemGoodsIcon.BackSpriteName1 = "iconState_zuoyue";
					}
					if (diamondLevelByGoodsID >= 4 && diamondLevelByGoodsID <= 5)
					{
						this.itemGoodsIcon.BackSpriteName1 = "iconState_zuoyue1";
					}
					if (diamondLevelByGoodsID >= 6 && diamondLevelByGoodsID <= 7)
					{
						this.itemGoodsIcon.BackSpriteName1 = "iconState_zuoyue2";
					}
					if (diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 10)
					{
						this.itemGoodsIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
					}
				}
			}
			this.SetGoodsName(goodsXmlNodeByID.Title);
		}
	}

	private void SetGoodsName(string name)
	{
		if (null != this.goodsName)
		{
			this.goodsName.Text = name;
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
			if (goodsData.Site == 7000)
			{
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
			}
			else if (goodsData.Site == 8000)
			{
				ggoodIcon.TextColor = 15793920U;
				int num = 0;
				ggoodIcon.ContentText.Text = "Lv" + Global.GetSoulCometStoneLevel(goodsData, out num);
				int equipGoodsSuitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
				if (equipGoodsSuitID == 1)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
				}
				if (equipGoodsSuitID == 2)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
				}
				if (equipGoodsSuitID == 3)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
				}
				if (equipGoodsSuitID >= 4 && equipGoodsSuitID <= 10)
				{
					ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
					ggoodIcon.TeXiao.gameObject.SetActive(true);
				}
			}
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
			this.lastIcon.BackSpriteName15 = "none";
		}
		if (null != icon)
		{
			icon.BackSpriteName15 = "iconState_highlight";
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
		int level = 1;
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		if (categoriyByGoodsID >= 910 && categoriyByGoodsID <= 928)
		{
			int num = 0;
			level = Global.GetSoulCometStoneLevel(goodsData, out num);
		}
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		this.baseValue.text = AvailableGoods.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, level, 0);
	}

	private int SortGoodsDataList(GoodsData x, GoodsData y)
	{
		if (y.GoodsID == x.GoodsID)
		{
			return y.GCount - x.GCount;
		}
		return y.GoodsID - x.GoodsID;
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

	public static string GetBaseAttributeStrFromPropertyList(double[] equipFields, int level = 1, int fillCount = 0)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = "e3b36c";
		string text4 = string.Empty;
		string text5 = " ";
		for (int i = 0; i < fillCount; i++)
		{
			text4 += text5;
		}
		for (int j = 1; j <= 10; j += 2)
		{
			if (equipFields[j] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					text2 + ": " + text4
				});
				double num = equipFields[j];
				if (j == 1)
				{
					if (equipFields[j] != 0.0)
					{
						text += string.Format("{0}{1}%", text2, (int)(num * (double)level));
						text += "\n";
					}
				}
				else
				{
					int num2 = j;
					int num3 = j + 1;
					if (equipFields[num2] != 0.0 || equipFields[num3] != 0.0)
					{
						double num4 = equipFields[num2];
						double num5 = equipFields[num3];
						text += string.Format("{0}{1}", text2, (int)(num5 * (double)level));
					}
				}
				text += "\n";
			}
		}
		for (int j = 11; j < 177; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					text2 + ": " + text4
				});
				double num6 = equipFields[j];
				if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
				{
					double num7 = num6 * (double)level * 100.0;
					text += string.Format("{0}{1}%", text2, Math.Round(num7, 2));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
				{
					text += string.Format("{0}{1}", text2, (int)(num6 * (double)level));
				}
				text += "\n";
			}
		}
		return Global.ProcessStr(text);
	}

	private const int columnsInPage = 3;

	private const int columns = 0;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton equipBtn;

	public GButton closeBtn;

	public GGoodsBox goodsBox;

	public GGoodIcon itemGoodsIcon;

	private GGoodIcon currentGoodIcon;

	public UIPanel bagPanel;

	public UIDraggablePanel dragPanel;

	private Vector3 panelPos = Vector3.zero;

	private Vector4 panelClip = Vector4.zero;

	public Transform background;

	public TextBlock goodsName;

	public TextBlock baseValue;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	private bool isPage;

	private int rowsInPage = 4;

	private GGoodIcon lastIcon;

	private GoodsType goodType = GoodsType.GoodsType_Default;
}
