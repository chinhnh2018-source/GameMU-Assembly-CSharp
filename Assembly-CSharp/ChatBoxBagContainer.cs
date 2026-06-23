using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChatBoxBagContainer : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.dragPanel.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.m_soltWidth = (int)this.dragPanel.GetComponent<UIPanel>().clipRange.z;
		this.InitBeiBaoList();
	}

	private void InitBeiBaoList()
	{
		if (this.beRebornBg)
		{
			this.lstBeiBaoWuPin = this.GetRebirthBaoGuoGoodsDatalist();
		}
		else
		{
			this.lstBeiBaoWuPin = this.GetBeiBaoWuPin();
		}
		if (this.lstBeiBaoWuPin == null)
		{
			this.lstBeiBaoWuPin = new List<GoodsData>();
		}
		this.ShowPage();
	}

	private void ShowPage()
	{
		if (this.lstBeiBaoWuPin == null)
		{
			return;
		}
		if (this.lstBeiBaoWuPin.Count <= 0)
		{
			this.m_pageNum = 1;
			this.m_realColCount = this.ColCount;
		}
		else
		{
			this.m_pageNum = (this.lstBeiBaoWuPin.Count - 1) / (this.ColCount * this.RowCount) + 1;
			this.m_realColCount = this.m_pageNum * this.ColCount;
		}
		this.pageController.SetPageNum(this.m_pageNum, 0);
		if (null != this.lstBeiBao)
		{
			this.lstBeiBao.ColCount = this.m_realColCount;
			this.lstBeiBao.RowCount = this.RowCount;
			this.lstBeiBao.listBox.maxPerLine = this.m_realColCount;
			this.lstBeiBao.InitBox();
			this.imgBg.transform.localScale = new Vector3((float)(this.m_realColCount * 82), 164f, 1f);
			this.lstBeiBao.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lstBeiBao_SelectionChanged);
		}
		this.LoadPageData(0);
	}

	private void LoadPageData(int page)
	{
		if (this.m_loadedDataPage >= page)
		{
			return;
		}
		int num = this.ColCount * this.RowCount * page;
		if (num >= this.lstBeiBaoWuPin.Count)
		{
			return;
		}
		for (int i = num; i < this.lstBeiBaoWuPin.Count; i++)
		{
			if (this.lstBeiBaoWuPin[i] != null)
			{
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(this.lstBeiBaoWuPin[i], true, i < this.m_equiedNum);
				if (!(goodsItemIcon == null))
				{
					this.lstBeiBao.SetGoodsIcon(this.GetBoxIndex(i), goodsItemIcon);
				}
			}
		}
		this.m_loadedDataPage = page;
	}

	private int GetBoxIndex(int i)
	{
		int num = i / (this.RowCount * this.ColCount);
		int num2 = i % (this.RowCount * this.ColCount);
		int num3 = num2 / this.ColCount;
		int num4 = num2 % this.ColCount;
		return num4 + num3 * this.m_realColCount + num * this.ColCount;
	}

	private void lstBeiBao_SelectionChanged(object sender, object e)
	{
		GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.lstBeiBao.listBox.SelectedItem);
		if (null != ggoodIcon && ggoodIcon.Tip != null && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2,
				Title = ggoodIcon.Tip
			});
		}
	}

	private void onDragFinished()
	{
		if (this.m_pageNum == 1)
		{
			return;
		}
		if (Math.Abs(Math.Abs(this.dragPanel.transform.localPosition.x) - (float)(this.m_soltWidth * this.m_currentSelectedPage)) > (float)this.pageOffSize)
		{
			if (this.dragPanel.transform.localPosition.x > (float)((0 - this.m_soltWidth) * this.m_currentSelectedPage))
			{
				this.m_currentSelectedPage--;
				if (this.m_currentSelectedPage <= 0)
				{
					this.m_currentSelectedPage = 0;
				}
			}
			else
			{
				this.m_currentSelectedPage++;
				if (this.m_currentSelectedPage >= this.m_pageNum)
				{
					this.m_currentSelectedPage = this.m_pageNum - 1;
				}
			}
		}
		this.dragPanel.target.x = (float)((0 - this.m_soltWidth) * this.m_currentSelectedPage);
		this.dragPanel.enabled = true;
		this.LoadPageData(this.m_currentSelectedPage);
		this.pageController.SetCurrentPage(this.m_currentSelectedPage);
	}

	private GGoodIcon GetGoodsItemIcon(GoodsData goodsData, bool isDrag, bool beEquiped = false)
	{
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			string textColoeByGoodsIDHTML = ChatBoxBagContainer.GetTextColoeByGoodsIDHTML(goodsData);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = "none";
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = textColoeByGoodsIDHTML;
			ggoodIcon.EquipedSprite.gameObject.SetActive(beEquiped);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
			UIEventListener.Get(ggoodIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSelectItem);
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private void OnSelectItem(GameObject go)
	{
		GGoodIcon component = go.GetComponent<GGoodIcon>();
		if (null != component && component.Tip != null && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2,
				Title = component.Tip
			});
		}
	}

	private List<GoodsData> GetBeiBaoWuPin()
	{
		List<GoodsData> list = new List<GoodsData>();
		if (list == null || Global.Data.roleData.GoodsDataList == null)
		{
			return list;
		}
		Global.GetUsingGoodsDataList();
		if (Super.GData.RoleUsingGoodsDataList != null)
		{
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				if (value != null)
				{
					if (Global.GetCategoriyByGoodsID(value.GoodsID) != 23)
					{
						list.Add(value);
						this.m_equiedNum++;
					}
				}
			}
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.GoodsDataList[i].Using == 0)
			{
				list.Add(Global.Data.roleData.GoodsDataList[i]);
			}
		}
		return list;
	}

	private List<GoodsData> GetRebirthBaoGuoGoodsDatalist()
	{
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> list2 = new List<GoodsData>();
		if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.RebornGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[i] != null)
				{
					if (Global.Data.roleData.RebornGoodsDataList[i].Using == 1)
					{
						list.Add(Global.Data.roleData.RebornGoodsDataList[i]);
					}
					else
					{
						list2.Add(Global.Data.roleData.RebornGoodsDataList[i]);
					}
				}
			}
		}
		this.m_equiedNum = list.Count;
		list.AddRange(list2);
		return list;
	}

	public static string GetTextColoeByGoodsIDHTML(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		string text2 = "{FFFFFF}";
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 10 || Global.GetGoodsCatetoriy(goodsData.GoodsID) == 9)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (zhuoyueAttributeCount != 0 || 1 < goodsXmlNodeByID.SuitID)
			{
				text2 = "{ff08ff}";
			}
			else
			{
				text2 = "{0099ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) >= 0 && Global.GetGoodsCatetoriy(goodsData.GoodsID) <= 22 && zhuoyueAttributeCount > 0)
		{
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.IsRebornEquip(Global.GetGoodsCatetoriy(goodsData.GoodsID)) && goodsData.ExcellenceInfo > 0)
		{
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) >= 40 && Global.GetGoodsCatetoriy(goodsData.GoodsID) <= 45 && goodsData.ExcellenceInfo > 0)
		{
			int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount2 > 0 && zhuoyueAttributeCount2 <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount2 >= 3 && zhuoyueAttributeCount2 <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount2 >= 5 && zhuoyueAttributeCount2 <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 340)
		{
			int horseQuality = Super.GetHorseQuality(goodsData);
			if (horseQuality == 1)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (horseQuality == 2)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (horseQuality >= 3)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (goodsData.Lucky > 0)
		{
			text2 = "{0099FF}";
		}
		string text3 = text + Global.GetGoodsNameByID(goodsData.GoodsID, false);
		string text4 = string.Format("{0}[{1}]{2}", text2, text3, "{-}");
		return string.Concat(new object[]
		{
			Global.GetLang("｛"),
			goodsData.GoodsID,
			"_",
			goodsData.Id,
			"_",
			Global.Data.roleData.RoleID,
			Global.GetLang("｝"),
			text4,
			Global.GetLang("｛-｝")
		});
	}

	public GGoodsBox lstBeiBao;

	public UISprite imgBg;

	public int ColCount = 10;

	public int RowCount = 2;

	public int pageOffSize = 100;

	public SpringPanel dragPanel;

	public ChatBoxPageController pageController;

	public bool beRebornBg;

	private int m_currentSelectedPage;

	private int m_pageNum = 1;

	private int m_loadedDataPage = -1;

	private int m_soltWidth = 820;

	private List<GoodsData> lstBeiBaoWuPin;

	private int m_realColCount;

	private int m_equiedNum;

	public DPSelectedItemEventHandler DPSelectedItem;
}
