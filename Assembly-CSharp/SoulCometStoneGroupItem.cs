using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SoulCometStoneGroupItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public SoulCometStoneGroupItemAttribute soulCometStoneGroupAttribute
	{
		set
		{
			this._soulCometStoneGroupAttribute = value;
			this.RefreshSoulCometGroup(this._soulCometStoneGroupAttribute);
		}
	}

	private void RefreshSoulCometGroup(SoulCometStoneGroupItemAttribute soulCometStoneGroupAttribute)
	{
		if (soulCometStoneGroupAttribute == null)
		{
			this.HideProperties(true);
			return;
		}
		this.RefreshStoneList(soulCometStoneGroupAttribute.list_groupItemAttr);
		this.SetStoneGroupAttributes(soulCometStoneGroupAttribute.properties, soulCometStoneGroupAttribute.perfect);
	}

	private void RefreshStoneList(List<SoulCometStoneGroupGoodsItemAttribute> list_groupAttribute)
	{
		if (null == this.stoneListBox)
		{
			return;
		}
		ObservableCollection itemsSource = this.stoneListBox.ItemsSource;
		itemsSource.Clear();
		if (list_groupAttribute == null || list_groupAttribute.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < list_groupAttribute.Count; i++)
		{
			SoulCometStoneGroupGoodsItemAttribute soulCometStoneGroupGoodsItemAttribute = list_groupAttribute[i];
			if (soulCometStoneGroupGoodsItemAttribute != null && soulCometStoneGroupGoodsItemAttribute.goodsData != null)
			{
				GGoodIcon ggoodIcon = this.AddIcon(soulCometStoneGroupGoodsItemAttribute.goodsData, soulCometStoneGroupGoodsItemAttribute.gray);
				itemsSource.AddNoUpdate(ggoodIcon);
				Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
				ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			}
		}
	}

	private void SetStoneGroupAttributes(string props, bool normalColor = true)
	{
		if (this.propertyGroups == null || this.propertyGroups.Length <= 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(props))
		{
			this.HideProperties(true);
			return;
		}
		string[] array = props.Split(new char[]
		{
			'|'
		});
		if (array == null || array.Length <= 0)
		{
			this.HideProperties(true);
			return;
		}
		int num = Math.Min(3, array.Length);
		int i;
		for (i = 0; i < num; i++)
		{
			if (!normalColor)
			{
				this.properties[i].textColor = (uint)NGUIMath.ColorToIntEx(Color.gray);
			}
			this.properties[i].Text = array[i];
		}
		while (i < 3)
		{
			this.propertyGroups[i].SetActive(false);
			i++;
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData, bool gray = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			ggoodIcon.EnableIcon = !gray;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.TextColor = 15793920U;
			int num = 0;
			int soulCometStoneLevel = Global.GetSoulCometStoneLevel(goodsData, out num);
			ggoodIcon.ContentText.Text = "Lv" + soulCometStoneLevel;
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
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.SoulCometStoneBagTip, GoodsOwnerTypes.SoulCometStoneBag, goodData);
	}

	private void HideProperties(bool hide = true)
	{
		if (this.propertyGroups == null || this.propertyGroups.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < this.propertyGroups.Length; i++)
		{
			this.propertyGroups[i].SetActive(!hide);
		}
	}

	private const int maxProperty = 4;

	public ListBox stoneListBox;

	public GameObject[] propertyGroups;

	public TextBlock[] properties;

	private SoulCometStoneGroupItemAttribute _soulCometStoneGroupAttribute;
}
