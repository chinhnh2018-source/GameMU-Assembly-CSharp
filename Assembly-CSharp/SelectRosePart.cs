using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class SelectRosePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitPrefabTxt();
		this.ReadXMLData();
		this.OnSelectIndex(1);
		this.OkBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.SelectId
				});
			}
			PlayZone.GlobalPlayZone.CloseSelectRoseWindow();
		};
		this.Rose1Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectIndex(1);
		};
		this.Rose2Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectIndex(2);
		};
		this.Rose3Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectIndex(3);
		};
		this.Rose4Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectIndex(4);
		};
		this.Rose5Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectIndex(5);
		};
	}

	public void OnSelectIndex(int selectIndex)
	{
		if (selectIndex != this.PreSelectIndex)
		{
			if (this.PreSelectIndex != 0)
			{
				this.Btns[this.PreSelectIndex - 1].target.gameObject.SetActive(false);
			}
			this.PreSelectIndex = selectIndex;
			this.Btns[this.PreSelectIndex - 1].target.gameObject.SetActive(true);
			this.SelectId = this.RoseDatas[selectIndex - 1].GoodsID;
			this.RoseName.text = this.RoseDatas[selectIndex - 1].Name;
			this.Effect.text = Global.GetLang("奉献度：+") + this.RoseDatas[selectIndex - 1].GoodWill;
		}
	}

	private void InitPrefabTxt()
	{
		this.OkBtn.Text = Global.GetLang("确定");
		this.Effect.text = string.Empty;
		this.RoseName.text = string.Empty;
	}

	private void ReadXMLData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/GiveRose.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Rose");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			SelectRosePart.RoseData roseData = new SelectRosePart.RoseData();
			roseData.ID = Global.GetXElementAttributeInt(xelement, "ID");
			roseData.GoodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
			roseData.Name = Global.GetXElementAttributeStr(xelement, "Name");
			roseData.GoodWill = Global.GetXElementAttributeInt(xelement, "GoodWill");
			roseData.SetGoodIcon(this.Btns[i].gameObject);
			this.RoseDatas.Add(roseData);
		}
	}

	public UILabel RoseName;

	public UILabel Effect;

	public GButton Rose1Btn;

	public GButton Rose2Btn;

	public GButton Rose3Btn;

	public GButton Rose4Btn;

	public GButton Rose5Btn;

	public GButton[] Btns;

	public GButton OkBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int SelectId;

	private int PreSelectIndex;

	private List<SelectRosePart.RoseData> RoseDatas = new List<SelectRosePart.RoseData>();

	private class RoseData
	{
		public void SetGoodIcon(GameObject parent)
		{
			if (this.Icon == null)
			{
				GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(this.GoodsID, 0, 0);
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsID);
				int categoriy = goodsXmlNodeByID.Categoriy;
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				this.Icon = U3DUtils.NEW<GGoodIcon>();
				this.Icon.GoodsID = this.GoodsID;
				this.Icon.Width = 78.0;
				this.Icon.Height = 78.0;
				this.Icon.ItemCategory = categoriy;
				this.Icon.ItemObject = fakeEquipGoodsData;
				this.Icon.isAutoSize = false;
				this.Icon.BackSpriteName0 = "bagGrid4_bak";
				this.Icon.BackgroundSprite0.MakePixelPerfect();
				this.Icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				this.Icon.Tip = Global.GetGoodsNameByID(this.GoodsID, false);
				bool canUse = Global.CanUseGoods(this.GoodsID, false, true);
				Super.InitGoodsGIcon(this.Icon, fakeEquipGoodsData, canUse, IconTextTypes.Qianghua);
				this.Icon.SecondText.Text = string.Format("{0}", Global.GetTotalGoodsCountByID(this.GoodsID));
				U3DUtils.AddChild(parent, this.Icon.gameObject, true);
				this.Icon.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
				BoxCollider component = this.Icon.GetComponent<BoxCollider>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
		}

		public void RefreshIcon()
		{
			if (this.Icon != null)
			{
				this.Icon.SecondText.Text = string.Format("{0}", Global.GetTotalGoodsCountByID(this.GoodsID));
			}
		}

		public int ID;

		public int GoodsID;

		public string Name;

		public int GoodWill;

		public GGoodIcon Icon;
	}
}
