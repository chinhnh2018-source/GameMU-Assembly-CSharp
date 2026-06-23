using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RoleShiZhuangPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_TeShuTiShi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("还未获得特殊称号")
		}));
		this.m_PuTongTiShi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("还未获得普通称号")
		}));
		NGUITools.SetActive(this.m_TeShuTiShi.gameObject, false);
		NGUITools.SetActive(this.m_PuTongTiShi.gameObject, false);
		this.m_Btns.transform.localPosition = new Vector3(this.m_Btns.transform.localPosition.x, this.m_Btns.transform.localPosition.y, -0.5f);
		for (int i = 0; i < this.m_Tab.TabBtns.Count; i++)
		{
			this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_BtnsText[i]
			}));
		}
		this.m_Tab.TabBtns[0].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			this.m_BtnsText[0]
		}));
		this.m_Tab.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			this.m_PuTongTiShi.transform.localPosition = new Vector3(0f, 0f, -1f);
			if (s.ID == 0)
			{
				NGUITools.SetActive(this.PuTongScroll.gameObject, true);
				NGUITools.SetActive(this.TeShuScroll.gameObject, false);
			}
			else if (s.ID == 1)
			{
				if (PlayZone.GlobalPlayZone.gamePayerRolePart != null)
				{
					NGUITools.SetActive(this.BtnTip.gameObject, false);
					NGUITools.SetActive(PlayZone.GlobalPlayZone.gamePayerRolePart._ActivityTipIcons[6].gameObject, false);
					Global.Data.teshuTitileTipOne = 2;
				}
				NGUITools.SetActive(this.PuTongScroll.gameObject, false);
				NGUITools.SetActive(this.TeShuScroll.gameObject, true);
			}
			for (int j = 0; j < this.m_Tab.TabBtns.Count; j++)
			{
				this.m_Tab.TabBtns[j].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_BtnsText[j]
				}));
				if (j == s.ID)
				{
					this.m_Tab.TabBtns[j].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.m_BtnsText[j]
					}));
				}
			}
			return true;
		};
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.dic_dress = RoleShiZhuangPart.GetDressConfig();
		this.dic_fashionID = RoleShiZhuangPart.GetFashionIDs();
		UIEventListener.Get(this.Help.gameObject).onClick = delegate(GameObject s)
		{
			GChildWindow RuleWindow = U3DUtils.NEW<GChildWindow>();
			RuleWindow.IsShowModal = true;
			RuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(RuleWindow, "RuleWindow");
			Super.GData.GlobalPlayZone.Children.Add(RuleWindow);
			RoleShiZhuangHelp roleShiZhuangHelp = U3DUtils.NEW<RoleShiZhuangHelp>();
			roleShiZhuangHelp.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs e)
			{
				Super.CloseChildWindow(this, RuleWindow);
			};
			RuleWindow.SetContent(RuleWindow.BodyPresenter, roleShiZhuangHelp, 0.0, 0.0, true);
		};
		this.m_BtnLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshPage(-1);
		};
		this.m_BtnRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshPage(1);
		};
	}

	public void InitRoleDressData()
	{
		this.GetDressListRequest();
	}

	private int MaxPage
	{
		get
		{
			int result;
			if (this.list_title.Count % 20 > 0)
			{
				result = this.list_title.Count / 20 + 1;
			}
			else
			{
				result = this.list_title.Count / 20;
			}
			return result;
		}
	}

	private void RefreshPage(int count)
	{
		this.m_PageKey += count;
		if (this.m_PageKey <= 0)
		{
			return;
		}
		if (this.m_PageKey > this.MaxPage)
		{
			this.m_PageKey = this.MaxPage;
			return;
		}
		if (this.m_PageKey <= 1)
		{
			this.m_BtnLeft.isEnabled = false;
		}
		else
		{
			this.m_BtnLeft.isEnabled = true;
		}
		if (this.m_PageKey >= this.MaxPage)
		{
			this.m_BtnRight.isEnabled = false;
		}
		else
		{
			this.m_BtnRight.isEnabled = true;
		}
		this.m_LabPage.text = string.Format("{0}/{1}", this.m_PageKey, this.MaxPage);
		this.RefreshTitleList(this.list_title);
		if (count != 0)
		{
			this.m_Spring.target = Vector3.zero;
			this.m_Spring.enabled = true;
		}
	}

	private void SortListChengHao()
	{
		int id = Global.Data.roleData.RoleCommonUseIntPamams[30];
		int fashionGoodsID = this.GetFashionGoodsID(id);
		for (int i = 0; i < this.list_title.Count; i++)
		{
			GoodsData goodsData = this.list_title[i];
			if (goodsData.GoodsID == fashionGoodsID)
			{
				this.list_title.Insert(0, goodsData);
				this.list_title.RemoveAt(i + 1);
				break;
			}
		}
	}

	private bool IsTitleButtonEnabled(List<GoodsData> list_goods)
	{
		if (list_goods == null || list_goods.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < list_goods.Count; i++)
		{
			GoodsData goodsData = list_goods[i];
			int dressTabIDByGoodsID = Global.GetDressTabIDByGoodsID(goodsData.GoodsID);
			if (dressTabIDByGoodsID == 2)
			{
				return true;
			}
		}
		return false;
	}

	private void EquipTitle(int titleGoodsID)
	{
		if (titleGoodsID < 0)
		{
			return;
		}
		int num = Global.Data.roleData.RoleCommonUseIntPamams[30];
		int dressIDByGoodsID = this.GetDressIDByGoodsID(titleGoodsID);
		bool equip = num != dressIDByGoodsID;
		this.EquipDressItem(dressIDByGoodsID, equip);
	}

	private void RefreshTitleList(List<GoodsData> list_title)
	{
		if (list_title == null)
		{
			return;
		}
		if (this.titleListBox.ItemsSource.Count > 0)
		{
			this.titleListBox.ItemsSource.Clear();
		}
		int id = Global.Data.roleData.RoleCommonUseIntPamams[30];
		int fashionGoodsID = this.GetFashionGoodsID(id);
		for (int i = (this.m_PageKey - 1) * 20; i < list_title.Count; i++)
		{
			ShiZhuangItem shiZhuangItem = U3DUtils.NEW<ShiZhuangItem>();
			shiZhuangItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				int id2 = e.ID;
				this.EquipTitle(id2);
			};
			shiZhuangItem.goodsData = list_title[i];
			shiZhuangItem.adorned = (shiZhuangItem.goodsData.GoodsID == fashionGoodsID);
			this.titleListBox.ItemsSource.AddNoUpdate(shiZhuangItem);
			if (i >= 19 + (this.m_PageKey - 1) * 20)
			{
				break;
			}
		}
	}

	private void SeperateDressList(List<GoodsData> list_goods)
	{
		if (list_goods == null || list_goods.Count <= 0)
		{
			return;
		}
		if (this.dic_dress == null || this.dic_dress.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < list_goods.Count; i++)
		{
		}
		if (this.list_title == null)
		{
			this.list_title = new List<GoodsData>();
		}
		this.list_title.Clear();
		int id = Global.Data.roleData.RoleCommonUseIntPamams[30];
		int fashionGoodsID = this.GetFashionGoodsID(id);
		for (int j = 0; j < list_goods.Count; j++)
		{
			GoodsData goodsData = list_goods[j];
			if (this.dic_dress.ContainsKey(goodsData.GoodsID))
			{
				DressAttribute dressAttribute = this.dic_dress[goodsData.GoodsID];
				if (dressAttribute.tabID == 2)
				{
					if (dressAttribute.goodsID == fashionGoodsID)
					{
						this.list_title.Insert(0, goodsData);
					}
					else
					{
						this.list_title.Add(goodsData);
					}
				}
			}
		}
	}

	private void RemoveDressItemByGoodsID(int goodsID)
	{
		if (goodsID <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.titleListBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GameObject at = itemsSource.GetAt(i);
			ShiZhuangItem component = at.GetComponent<ShiZhuangItem>();
			if (null != component && goodsID == component.goodsData.Id)
			{
				itemsSource.RemoveAt(i);
				return;
			}
		}
	}

	private bool IsListEmpty()
	{
		return this.titleListBox.ItemsSource.Count <= 0;
	}

	private int GetDressIDByGoodsID(int goodsID)
	{
		int result = 0;
		if (this.dic_dress != null)
		{
			DressAttribute dressAttribute = null;
			this.dic_dress.TryGetValue(goodsID, ref dressAttribute);
			if (dressAttribute != null)
			{
				result = dressAttribute.id;
			}
		}
		return result;
	}

	private int GetDressTabIDByGoodsID(int goodsID)
	{
		int result = 0;
		if (this.dic_dress != null)
		{
			DressAttribute dressAttribute = null;
			this.dic_dress.TryGetValue(goodsID, ref dressAttribute);
			if (dressAttribute != null)
			{
				result = dressAttribute.tabID;
			}
		}
		return result;
	}

	private int GetFashionGoodsID(int id)
	{
		int result = 0;
		if (this.dic_fashionID != null)
		{
			this.dic_fashionID.TryGetValue(id, ref result);
		}
		return result;
	}

	private static Dictionary<int, DressAttribute> GetDressConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FashionTab.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
		if (xelementList == null || xelementList.Count < 2)
		{
			return null;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[0], "Categoriy");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[1], "Categoriy");
		XElement gameResXml2 = Global.GetGameResXml("Config/Fashion.xml");
		if (gameResXml2 == null)
		{
			return null;
		}
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "Fashion");
		if (xelementList2 == null || xelementList2.Count <= 0)
		{
			return null;
		}
		Dictionary<int, DressAttribute> dictionary = new Dictionary<int, DressAttribute>(xelementList2.Count);
		for (int i = 0; i < xelementList2.Count; i++)
		{
			XElement xelement = xelementList2[i];
			DressAttribute dressAttribute = new DressAttribute();
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Tab");
			if (xelementAttributeInt3 == 1)
			{
				dressAttribute.categoryID = xelementAttributeInt;
			}
			else if (xelementAttributeInt3 == 2)
			{
				dressAttribute.categoryID = xelementAttributeInt2;
			}
			dressAttribute.id = Global.GetXElementAttributeInt(xelement, "ID");
			dressAttribute.tabID = Global.GetXElementAttributeInt(xelement, "Tab");
			dressAttribute.goodsID = Global.GetXElementAttributeInt(xelement, "Goods");
			dressAttribute.name = Global.GetXElementAttributeStr(xelement, "Name");
			dressAttribute.type = Global.GetXElementAttributeInt(xelement, "Type");
			dressAttribute.timeLimit = Global.GetXElementAttributeInt(xelement, "Time");
			if (!dictionary.ContainsKey(dressAttribute.goodsID))
			{
				dictionary.Add(dressAttribute.goodsID, dressAttribute);
			}
		}
		return dictionary;
	}

	private static Dictionary<int, int> GetFashionIDs()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Goods");
			if (!dictionary.ContainsKey(xelementAttributeInt))
			{
				dictionary.Add(xelementAttributeInt, xelementAttributeInt2);
			}
		}
		return dictionary;
	}

	public void GetDressListRequest()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetDressList();
	}

	public void EquipDressItem(int titleID, bool equip = true)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.UploadLuoLanWing(2, titleID, (!equip) ? 2 : 1);
	}

	public void SetEquipedFashion(int status)
	{
		Super.HideNetWaiting();
		string msg = string.Empty;
		switch (status + 3)
		{
		case 0:
			msg = Global.GetLang("不符合佩戴条件");
			break;
		default:
			if (status != -3002)
			{
				if (status != -3001)
				{
					if (status != -20)
					{
						if (status == -12)
						{
							msg = Global.GetLang("服务器数据异常");
						}
					}
					else
					{
						msg = Global.GetLang("这不是一个可佩戴道具");
					}
				}
				else
				{
					msg = Global.GetLang("只有罗兰城主才能佩戴");
				}
			}
			else
			{
				msg = Global.GetLang("没有婚姻关系无法佩戴");
			}
			break;
		case 3:
			this.SetEquipedState();
			break;
		}
		if (status != 0)
		{
			Super.HintMainText(msg, 10, 3);
		}
	}

	public void SetDressList(List<GoodsData> list_goods)
	{
		Super.HideNetWaiting();
		if (Global.Data.teshuTitileBufferLst == null || Global.Data.teshuTitileBufferLst.Count <= 0)
		{
			NGUITools.SetActive(this.m_TeShuTiShi.gameObject, true);
		}
		this.AddItem();
		if (Global.Data.teshuTitileBufferLst.Count > 0 && Global.Data.BufferDataId <= 0 && Global.Data.teshuTitileTipOne == 1)
		{
			NGUITools.SetActive(this.BtnTip.gameObject, true);
		}
		if (list_goods == null || list_goods.Count <= 0)
		{
			this.m_BtnLeft.isEnabled = false;
			this.m_BtnRight.isEnabled = false;
			this.m_LabPage.text = string.Format("{0}/{1}", 0, 0);
			NGUITools.SetActive(this.m_PuTongTiShi.gameObject, true);
			return;
		}
		this.SeperateDressList(list_goods);
		if (this.list_title.Count <= 0)
		{
			this.m_BtnLeft.isEnabled = false;
			this.m_BtnRight.isEnabled = false;
			this.m_LabPage.text = string.Format("{0}/{1}", 0, 0);
			NGUITools.SetActive(this.m_PuTongTiShi.gameObject, true);
		}
		this.RefreshPage(0);
		NGUITools.SetActive(this.m_TeShuTitlePanel.gameObject, false);
	}

	private void SetEquipedState()
	{
		this.SortListChengHao();
		this.RefreshPage(0);
	}

	public void OnFashionEquipInvalid(int goodsID)
	{
		this.RemoveDressItemByGoodsID(goodsID);
	}

	public void OnFashionEquipAdded(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return;
		}
		int dressTabIDByGoodsID = this.GetDressTabIDByGoodsID(goodsData.GoodsID);
		if (dressTabIDByGoodsID < 2)
		{
			return;
		}
		if (this.IsListEmpty())
		{
			if (this.list_title == null)
			{
				this.list_title = new List<GoodsData>();
			}
			this.list_title.Clear();
			this.list_title.Add(goodsData);
		}
		else
		{
			this.list_title.Add(goodsData);
		}
		this.SortListChengHao();
		this.RefreshPage(0);
	}

	private void RefreshDic()
	{
	}

	private void AddItem()
	{
		if (Global.Data.teshuTitileBufferLst.Count <= 0)
		{
			NGUITools.SetActive(this.m_TeShuTiShi.gameObject, true);
			return;
		}
		NGUITools.SetActive(this.m_TeShuTitlePanel, true);
		ObservableCollection itemsSource = this.m_TeShuTitleListBox.ItemsSource;
		itemsSource.Clear();
		int num = -1;
		for (int i = 0; i < Global.Data.teshuTitileBufferLst.Count; i++)
		{
			for (int j = 0; j < Global.TeShuTitleListXml.Count; j++)
			{
				if (Global.TeShuTitleListXml[j].BuffID == Global.Data.teshuTitileBufferLst[i].BufferID)
				{
					TeShuChengHaoItem teShuChengHaoItem = U3DUtils.NEW<TeShuChengHaoItem>();
					itemsSource.AddNoUpdate(teShuChengHaoItem);
					SpecialTitle specialTitle = Global.TeShuTitleListXml[j];
					if (Global.Data.BufferDataId == specialTitle.BuffID)
					{
						num = i;
					}
					teShuChengHaoItem.BufferData = Global.Data.teshuTitileBufferLst[i];
					teShuChengHaoItem.SetRoleTitle(specialTitle.IconCode);
					teShuChengHaoItem.SetProperties(specialTitle.Describtion);
					teShuChengHaoItem.Index = i;
					teShuChengHaoItem.ID = specialTitle.ID;
					teShuChengHaoItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
					{
						this.GetDataTeShu(e.IDType, e.AllowAutoBuy, e.Index);
					};
				}
				NGUITools.SetActive(this.m_TeShuTitlePanel, false);
			}
		}
		if (num != -1)
		{
			this.RefreshItem(num);
		}
	}

	private void RefreshItem(int index)
	{
		Super.HideNetWaiting();
		ObservableCollection itemsSource = this.m_TeShuTitleListBox.ItemsSource;
		if (index == 0)
		{
			itemsSource.GetAt(0).GetComponent<TeShuChengHaoItem>().adorned = !itemsSource.GetAt(0).GetComponent<TeShuChengHaoItem>().adorned;
			return;
		}
		itemsSource.GetAt(0).GetComponent<TeShuChengHaoItem>().adorned = false;
		TeShuChengHaoItem component = itemsSource.GetAt(index).GetComponent<TeShuChengHaoItem>();
		if (component == null)
		{
			return;
		}
		TeShuChengHaoItem teShuChengHaoItem = U3DUtils.NEW<TeShuChengHaoItem>();
		teShuChengHaoItem.BufferData = component.BufferData;
		teShuChengHaoItem.SetRoleTitle(component.ImgName);
		teShuChengHaoItem.SetProperties(component.TitleData);
		teShuChengHaoItem.adorned = true;
		teShuChengHaoItem.ID = component.ID;
		itemsSource.Remove(index);
		itemsSource.Insert(0, teShuChengHaoItem);
		for (int i = 0; i < itemsSource.Count; i++)
		{
			int index2 = i;
			itemsSource.GetAt(index2).GetComponent<TeShuChengHaoItem>().Index = i;
			itemsSource.GetAt(index2).GetComponent<TeShuChengHaoItem>().DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.GetDataTeShu(e.IDType, e.AllowAutoBuy, e.Index);
			};
		}
	}

	private void GetDataTeShu(int ID, bool _equipState, int index)
	{
		Super.ShowNetWaiting(null);
		this.m_index = index;
		int mode;
		if (_equipState)
		{
			mode = 2;
		}
		else
		{
			mode = 1;
		}
		GameInstance.Game.GetGetTeShuChengHaoPeiDai(ID, mode);
	}

	public void GetDataPeiDai(string result)
	{
		Super.HideNetWaiting();
		if (result.SafeToInt32(0) == 0)
		{
			this.AddItem();
			NGUITools.SetActive(this.m_TeShuTitlePanel, true);
		}
	}

	public GameObject titlePanel;

	public ListBox titleListBox;

	public UIButton Help;

	public ListBox m_TeShuTitleListBox;

	public GameObject m_TeShuTitlePanel;

	public UILabel m_TeShuTiShi;

	public UILabel m_PuTongTiShi;

	public GameObject m_Btns;

	public GTab m_Tab;

	public GameObject PuTongScroll;

	public GameObject TeShuScroll;

	public GameObject BtnTip;

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public UILabel m_LabPage;

	public SpringPanel m_Spring;

	private Dictionary<int, DressAttribute> dic_dress;

	private Dictionary<int, int> dic_fashionID;

	private List<GoodsData> list_title;

	private int m_index = -1;

	private string[] m_BtnsText = new string[]
	{
		Global.GetLang("普通称号"),
		Global.GetLang("特殊称号")
	};

	private int m_PageKey = 1;
}
