using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class FashionWardrobePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LiftFashionListView.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectChange);
		this.m_RightPropertyObservableCollection = this.m_ListBox_RightProperty.ItemsSource;
		this.m_FashionListObservableCollection = this.m_LiftFashionListView.ItemsSource;
		if (this._LiftPageObg != null)
		{
			this.m_ListTopBtns.Add(new FashionWardrobePart.BtnHander(this._LiftTopPageBtn[0], 0, new DPSelectedItemEventHandler(this.LiftTopBtnHander)));
			this.m_ListTopBtns.Add(new FashionWardrobePart.BtnHander(this._LiftTopPageBtn[1], 1, new DPSelectedItemEventHandler(this.LiftTopBtnHander)));
			if (0L < ConfigSystemParam.GetSystemParamIntByName("WuQiShiZhuangMainGame"))
			{
				this.m_ListTopBtns.Add(new FashionWardrobePart.BtnHander(this._LiftTopPageBtn[2], 2, new DPSelectedItemEventHandler(this.LiftTopBtnHander)));
			}
			else
			{
				this._LiftTopPageBtn[2].SetActive(false);
			}
			if (0L < ConfigSystemParam.GetSystemParamIntByName("JiaoYinShiZhuangMainGame"))
			{
				this.m_ListTopBtns.Add(new FashionWardrobePart.BtnHander(this._LiftTopPageBtn[3], 3, new DPSelectedItemEventHandler(this.LiftTopBtnHander)));
				if (0L >= ConfigSystemParam.GetSystemParamIntByName("WuQiShiZhuangMainGame"))
				{
					this._LiftTopPageBtn[3].transform.localPosition = this._LiftTopPageBtn[2].transform.localPosition;
				}
			}
			else
			{
				this._LiftTopPageBtn[3].SetActive(false);
			}
			for (int i = 0; i < this.m_ListTopBtns.Count; i++)
			{
				if (this.m_ListTopBtns[i] != null)
				{
					this.m_ListTopBtns[1].BChose = (0 == i);
				}
			}
		}
		base.StartCoroutine<bool>(this.StartInitUI());
	}

	private IEnumerator StartInitUI()
	{
		Super.ShowNetWaiting(null);
		this.InitPrefabText();
		this.InitTexture();
		yield return null;
		this.InitXml();
		yield return null;
		this.InitHandler();
		yield return null;
		NGUITools.SetActive(this.m_AllFullSp, false);
		this.InitFashionwardRobeData();
		GameInstance.Game.GetLuoLanChengZhuInfo();
		base.StartCoroutine<bool>(this.StartLoadMode());
		this.RoleModal.transform.localPosition = new Vector3(5f, -170f, -250f);
		Super.HideNetWaiting();
		yield break;
	}

	private void InitXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (Global.GetXElementAttributeInt(xelement, "Type") == 1)
			{
				this.cfgWingId = Global.GetXElementAttributeInt(xelement, "ID");
				this.cfgWingTabId = Global.GetXElementAttributeInt(xelement, "Tab");
				break;
			}
		}
	}

	private void InitPrefabText()
	{
		this.m_WearBtn.Label.text = Global.GetLang("穿戴");
		this.m_PropertyTitleLabel[0].text = Global.GetLang("时装属性");
		this.m_PropertyTitleLabel[1].text = Global.GetLang("升级消耗");
		this.m_FashionUpBtn.Text = Global.GetLang("升级");
	}

	private void InitTexture()
	{
		this.m_BgImage[1].URL = "NetImages/GameRes/Images/Fashionwardrobe/FashionwardrobeBg_Mode3D.jpg";
	}

	private void InitHandler()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		this.m_PropertyBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowPropertyPart();
		};
		this.m_WearBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (!this.WearBtnCanCheck)
			{
				return;
			}
			this.WearBtnCanCheck = false;
			base.StartCoroutine<bool>(this.ChangeBtnState());
			this.WearBtnHander();
		};
		this.m_FashionUpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.UpBtnHander_Fashion();
		};
		this.m_HelpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowHelpPart();
		};
		this.WearBtn_LuoLan.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (!this.WearBtnCanCheck)
			{
				return;
			}
			this.WearBtnCanCheck = false;
			base.StartCoroutine<bool>(this.ChangeBtnState());
			if (Global.GetLang("穿戴罗兰") == this.WearBtn_LuoLan.Text)
			{
				if (Global.Data.roleData.MyWingData.Using == 0)
				{
					Super.HintMainText(Global.GetLang("需要佩戴翅膀才能佩戴罗兰羽翼！"), 10, 3);
					return;
				}
				GameInstance.Game.UploadLuoLanWing(this.cfgWingTabId, this.cfgWingId, 1);
			}
			else
			{
				GameInstance.Game.UploadLuoLanWing(this.cfgWingTabId, this.cfgWingId, 2);
			}
		};
	}

	private void WearBtnHander()
	{
		ListBox liftFashionListView = this.m_LiftFashionListView;
		ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
		if (null != liftFashionListView.SelectedItem)
		{
			bool flag = Global.GetLang("穿戴") == this.m_WearBtn.Label.text;
			FashionItem component = liftFashionListView.SelectedItem.GetComponent<FashionItem>();
			if (null != component)
			{
				if (component.FashionTimeIsOver)
				{
					Super.HintMainText(Global.GetLang("时装已过期！"), 10, 3);
				}
				else
				{
					if (topBtnsSelectFashionCate == ItemCategories.ChiBang && Global.Data.roleData.MyWingData.Using != 1)
					{
						Super.HintMainText(Global.GetLang("请先佩戴翅膀！"), 10, 3);
						return;
					}
					this.ScendModFashion(flag);
					UILabel label = this.m_WearBtn.Label;
					string text;
					if (flag)
					{
						string lang = Global.GetLang("卸载");
						this.m_WearBtn.Label.text = lang;
						text = lang;
					}
					else
					{
						string lang = Global.GetLang("穿戴");
						this.m_WearBtn.Label.text = lang;
						text = lang;
					}
					label.text = text;
				}
			}
		}
		else
		{
			bool flag2 = false;
			List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
			if (fashionAndTitleList != null && 0 < fashionAndTitleList.Count)
			{
				for (int i = 0; i < fashionAndTitleList.Count; i++)
				{
					GoodsData goodsData = fashionAndTitleList[i];
					if (topBtnsSelectFashionCate == (ItemCategories)Global.GetCategoriyByGoodsID(goodsData.GoodsID) && goodsData.Using == 1)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					bool flag3 = Global.GetLang("穿戴") == this.m_WearBtn.Label.text;
					if (topBtnsSelectFashionCate == ItemCategories.ChiBang && Global.Data.roleData.MyWingData.Using != 1)
					{
						Super.HintMainText(Global.GetLang("请先佩戴翅膀！"), 10, 3);
						return;
					}
					if (topBtnsSelectFashionCate == ItemCategories.ShiZhuang_WuQi)
					{
						bool flag4 = false;
						Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
						if (usingGoodsDataList != null)
						{
							foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
							{
								int categoriyByGoodsID = Global.GetCategoriyByGoodsID(keyValuePair.Value.Id);
								if (11 <= categoriyByGoodsID && 21 >= categoriyByGoodsID)
								{
									flag4 = true;
									break;
								}
							}
						}
						if (!flag4)
						{
						}
					}
					this.ScendModFashion(flag3);
					UILabel label2 = this.m_WearBtn.Label;
					string text2;
					if (flag3)
					{
						string lang = Global.GetLang("卸载");
						this.m_WearBtn.Label.text = lang;
						text2 = lang;
					}
					else
					{
						string lang = Global.GetLang("穿戴");
						this.m_WearBtn.Label.text = lang;
						text2 = lang;
					}
					label2.text = text2;
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选择一套时装"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("当前无时装可选！"), 10, 3);
			}
		}
	}

	private void CheckWearBtnText(bool bChiBang)
	{
		List<GoodsData> list;
		if (bChiBang)
		{
			list = Global.GetRoleChiBangFashionList(true);
		}
		else
		{
			list = Global.GetRoleFashionList(ItemCategories.Fashion);
		}
		bool flag = false;
		if (0 < list.Count)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(list[i].GoodsID);
				if ((categoriyByGoodsID == 8 || categoriyByGoodsID == 24) && list[i].Using == 1)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.m_WearBtn.Label.text = Global.GetLang("卸载");
		}
		else
		{
			this.m_WearBtn.Label.text = Global.GetLang("穿戴");
		}
	}

	private void UpBtnHander_ChiBang()
	{
	}

	private void UpBtnHander_Fashion()
	{
		if (this.m_FashionSelectData != null)
		{
			ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
			FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(topBtnsSelectFashionCate, this.m_FashionSelectData.GoodsID, this.m_FashionSelectData.Forge_level + 1);
			if (fashionLevelupVO != null)
			{
				int num = 0;
				bool flag = 0 >= fashionLevelupVO.Time;
				string[] array = fashionLevelupVO.NeedGoods.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					num = Global.GetRoleGoodsNumberCountByGoodsID(int.Parse(array[0]));
				}
				if (flag)
				{
					int fashUpCost = this.GetFashUpCost(this.m_FashionSelectData.GoodsID, this.m_FashionSelectData.Forge_level + 1);
					if (fashUpCost <= num)
					{
						Super.ShowNetWaiting(null);
						GameInstance.Game.SendFashionUp(this.m_FashionSelectData.Id);
					}
					else
					{
						Super.HintMainText(Global.GetLang("道具不足"), 10, 3);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("所选时装不能升级"), 10, 3);
				}
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("请选择一套时装"), 10, 3);
		}
	}

	private bool CanShowFashionUi(byte type)
	{
		ItemCategories itemCategories = ItemCategories.Fashion;
		if (type == 0)
		{
			itemCategories = ItemCategories.Fashion;
		}
		else if (type == 1)
		{
			itemCategories = ItemCategories.ChiBang;
		}
		else if (type == 2)
		{
			itemCategories = ItemCategories.ShiZhuang_WuQi;
		}
		else if (type == 3)
		{
			itemCategories = ItemCategories.ShiZhuang_JiaoYin;
		}
		List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
		if (fashionAndTitleList != null)
		{
			int i = 0;
			int count = fashionAndTitleList.Count;
			while (i < count)
			{
				if (fashionAndTitleList[i] != null && itemCategories == (ItemCategories)Global.GetCategoriyByGoodsID(fashionAndTitleList[i].GoodsID))
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	private void LiftTopBtnHander(object sender, DPSelectedItemEventArgs args)
	{
		if (args != null)
		{
			this.mLastSelectTopsBtnID = this.mSelectTopsBtnID;
			byte type = (byte)args.ID;
			this.mSelectTopsBtnID = type;
			if (this.CanShowFashionUi(type))
			{
				this.ActiceFashionUI(type, 1);
			}
			else
			{
				this.mSelectTopsBtnID = this.mLastSelectTopsBtnID;
				this.ActiceFashionUI(this.mLastSelectTopsBtnID, 0);
				Super.HintMainText(Global.GetLang("此功能暂未开启"), 10, 3);
			}
		}
	}

	private void ActiceFashionUI(byte type, byte RefreshView = 0)
	{
		for (int i = 0; i < this.m_ListTopBtns.Count; i++)
		{
			if (this.m_ListTopBtns[i] != null)
			{
				if (this.m_ListTopBtns[i].ID == (int)type)
				{
					this.m_ListTopBtns[i].BChose = true;
				}
				else
				{
					this.m_ListTopBtns[i].BChose = false;
				}
			}
		}
		if (RefreshView == 1)
		{
			this.InitLiftView();
			this.RefreshLuoLanYuYiBtn();
		}
	}

	private void RefreshLuoLanYuYiBtn()
	{
		if (this.GetTopBtnsSelectFashionCate() == ItemCategories.ChiBang)
		{
			if (this.mRoleIsLuoLanChengZhu)
			{
				NGUITools.SetActive(this.WearBtn_LuoLan, true);
				if (Global.Data.roleData.RoleCommonUseIntPamams[26] == 1)
				{
					this.WearBtn_LuoLan.Text = Global.GetLang("卸下罗兰");
				}
				else
				{
					this.WearBtn_LuoLan.Text = Global.GetLang("穿戴罗兰");
				}
				Vector3 localPosition = this.m_WearBtn.transform.localPosition;
				localPosition.x = 70f;
				this.m_WearBtn.transform.localPosition = localPosition;
				localPosition.x = -70f;
				this.WearBtn_LuoLan.transform.localPosition = localPosition;
			}
			else
			{
				NGUITools.SetActive(this.WearBtn_LuoLan, false);
				Vector3 localPosition2 = this.m_WearBtn.transform.localPosition;
				localPosition2.x = 0f;
				this.m_WearBtn.transform.localPosition = localPosition2;
			}
		}
		else
		{
			NGUITools.SetActive(this.WearBtn_LuoLan, false);
			Vector3 localPosition3 = this.m_WearBtn.transform.localPosition;
			localPosition3.x = 0f;
			this.m_WearBtn.transform.localPosition = localPosition3;
		}
	}

	private IEnumerator ChangeBtnState()
	{
		yield return new WaitForSeconds(1.2f);
		this.WearBtnCanCheck = true;
		yield break;
	}

	private void ShowPropertyPart()
	{
		List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
		if (0 < selectTopBtnsTypeGoodsList.Count)
		{
			string[] content = new string[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("累计加成属性")
				}),
				this.GetProPertyStr(selectTopBtnsTypeGoodsList)
			};
			Global.ShowProPerty(0, content, null);
			FashionWardrobePart.ChangeTipsZ("ProPerty", -650);
		}
		else
		{
			Super.HintMainText(Global.GetLang("当前无已激活的时装"), 10, 3);
		}
	}

	private void ShowHelpPart()
	{
		Global.ShowHelpPart(this.str_Help[0], Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			this.str_Help[1]
		}), false, 0f);
		FashionWardrobePart.ChangeTipsZ("HelpPart", -650);
	}

	private void ScendModFashion(bool bDress)
	{
		List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
		if (bDress)
		{
			GoodsData goodsData = selectTopBtnsTypeGoodsList.Find((GoodsData s) => s.Using == 1);
			if (goodsData != null)
			{
				GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, 0, 6000, goodsData.GCount, goodsData.BagIndex, string.Empty);
			}
			GameObject at = this.m_FashionListObservableCollection.GetAt(this.m_LiftFashionListView.SelectedIndex);
			if (null != at)
			{
				FashionItem component = at.GetComponent<FashionItem>();
				if (null != component)
				{
					GoodsData goodsData2 = component._GoodsData;
					if (goodsData2 != null)
					{
						GameInstance.Game.SpriteModGoods(1, goodsData2.Id, goodsData2.GoodsID, 1, 6000, goodsData2.GCount, goodsData2.BagIndex, string.Empty);
					}
				}
			}
		}
		else
		{
			GoodsData goodsData3 = selectTopBtnsTypeGoodsList.Find((GoodsData s) => s.Using == 1);
			if (goodsData3 != null)
			{
				GameInstance.Game.SpriteModGoods(2, goodsData3.Id, goodsData3.GoodsID, 0, 6000, goodsData3.GCount, goodsData3.BagIndex, string.Empty);
			}
		}
	}

	private void InitFashionwardRobeData()
	{
		bool flag = true;
		for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(Global.Data.fashionAndTitleList[i].GoodsID);
			if ((24 <= categoriyByGoodsID && 27 >= categoriyByGoodsID) || categoriyByGoodsID == 8)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.GetDressList();
		}
		else
		{
			this.InitLiftView();
		}
	}

	private List<GoodsData> GetSelectTopBtnsTypeGoodsList()
	{
		ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
		{
			if (Global.Data.fashionAndTitleList[i] != null && topBtnsSelectFashionCate == (ItemCategories)Global.GetCategoriyByGoodsID(Global.Data.fashionAndTitleList[i].GoodsID))
			{
				if (Global.Data.fashionAndTitleList[i].Using == 1)
				{
					list.Insert(0, Global.Data.fashionAndTitleList[i]);
				}
				else
				{
					list.Add(Global.Data.fashionAndTitleList[i]);
				}
			}
		}
		return list;
	}

	private void InitLiftView()
	{
		this.m_FashionListObservableCollection.Clear();
		List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
		if (0 < selectTopBtnsTypeGoodsList.Count)
		{
			selectTopBtnsTypeGoodsList.Sort(delegate(GoodsData x, GoodsData y)
			{
				if (x.Using == 1 || y.Using == 1)
				{
					return y.Using - x.Using;
				}
				if (x.Forge_level == y.Forge_level)
				{
					return x.Id - y.Id;
				}
				return y.Forge_level - x.Forge_level;
			});
			ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
			for (int i = 0; i < selectTopBtnsTypeGoodsList.Count; i++)
			{
				FashionItem fashionItem = U3DUtils.NEW<FashionItem>();
				FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(topBtnsSelectFashionCate, selectTopBtnsTypeGoodsList[i].GoodsID, selectTopBtnsTypeGoodsList[i].Forge_level);
				if (fashionLevelupVO != null)
				{
					fashionItem.IsForeverFashion = (fashionLevelupVO.Time == -1);
				}
				fashionItem.RefreshFashionItemInf(selectTopBtnsTypeGoodsList[i]);
				fashionItem.DeletePanel();
				fashionItem.Hander = new DPSelectedItemEventHandler(this.FashionItemHander);
				fashionItem.DraggablePanel = this.m_DraggablePanel;
				this.m_FashionListObservableCollection.AddNoUpdate(fashionItem);
			}
			this.m_DraggablePanel.UpdateScrollbars(false);
			this.m_LiftFashionListView.repositionNow = true;
		}
		else
		{
			this.m_Propertylabel.text = string.Empty;
		}
		this.SelectChange(null, null);
		base.StartCoroutine<bool>(this.PressUIView());
	}

	private IEnumerator PressUIView()
	{
		yield return null;
		this.m_DraggablePanel.Press(false);
		yield break;
	}

	private void FashionItemHander(object sender, DPSelectedItemEventArgs args)
	{
		if (sender != null)
		{
			GameObject gameObject = sender as GameObject;
			if (null != gameObject)
			{
				FashionItem component = gameObject.GetComponent<FashionItem>();
				if (null != component)
				{
					component.FashionTimeIsOver = true;
				}
			}
		}
	}

	private void SelectChange(object sender, MouseEvent e)
	{
		int num = 0;
		ListBox liftFashionListView = this.m_LiftFashionListView;
		if (sender != null)
		{
			if (null != liftFashionListView.SelectedItem)
			{
				FashionItem component = liftFashionListView.SelectedItem.GetComponent<FashionItem>();
				num = component.GoodsID;
				this.m_FashionSelectData = component._GoodsData;
				this.RefreshFrontShowMode(num, false);
				if (null != component)
				{
					num = component.GoodsID;
					int lev = component.Lev;
					this.RefreshPropertyData(lev, num);
				}
			}
		}
		else
		{
			bool flag = false;
			if (null != liftFashionListView.SelectedItem)
			{
				GameObject selectedItem = liftFashionListView.SelectedItem;
				if (null != selectedItem)
				{
					FashionItem component2 = selectedItem.GetComponent<FashionItem>();
					if (null != component2)
					{
						num = component2.GoodsID;
						int lev2 = component2.Lev;
						this.RefreshPropertyData(lev2, num);
					}
				}
				else
				{
					this.RefreshPropertyData(0, num);
				}
			}
			else
			{
				List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
				for (int i = 0; i < selectTopBtnsTypeGoodsList.Count; i++)
				{
					GoodsData goodsData = selectTopBtnsTypeGoodsList[i];
					if (goodsData.Using == 1)
					{
						this.m_LiftFashionListView.SelectedIndex = i;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					liftFashionListView.SelectedIndex = 0;
				}
				GameObject at = this.m_FashionListObservableCollection.GetAt(liftFashionListView.SelectedIndex);
				if (null != at)
				{
					FashionItem component3 = at.GetComponent<FashionItem>();
					if (null != component3)
					{
						num = component3.GoodsID;
						int lev3 = component3.Lev;
						this.RefreshPropertyData(lev3, num);
					}
				}
				else
				{
					this.RefreshPropertyData(0, num);
				}
			}
		}
		List<GoodsData> selectTopBtnsTypeGoodsList2 = this.GetSelectTopBtnsTypeGoodsList();
		for (int j = 0; j < selectTopBtnsTypeGoodsList2.Count; j++)
		{
			GoodsData goodsData2 = selectTopBtnsTypeGoodsList2[j];
			if (goodsData2.GoodsID == num)
			{
				this.m_WearBtn.Label.text = ((goodsData2.Using != 1) ? Global.GetLang("穿戴") : Global.GetLang("卸载"));
				break;
			}
		}
		if (sender != null)
		{
			this.UpDataFashionItem();
		}
	}

	private void UpDataFashionItem()
	{
		ObservableCollection fashionListObservableCollection = this.m_FashionListObservableCollection;
		ListBox liftFashionListView = this.m_LiftFashionListView;
		if (fashionListObservableCollection != null)
		{
			for (int i = 0; i < fashionListObservableCollection.Count; i++)
			{
				GameObject at = fashionListObservableCollection.GetAt(i);
				if (null != at)
				{
					FashionItem component = at.GetComponent<FashionItem>();
					component.SetSelect(liftFashionListView.SelectedIndex == i);
				}
			}
		}
	}

	private IEnumerator StartLoadMode()
	{
		Super.ShowNetWaiting(null);
		yield return new WaitForSeconds(0.3f);
		Super.HideNetWaiting();
		this.RefreshFrontShowMode(-1, true);
		this.m_LiftFashionListView.SelectedIndex = 0;
		this.UpDataFashionItem();
		yield break;
	}

	private List<string> GetProPertyStrList(int Level, int GoodsId)
	{
		string empty = string.Empty;
		List<string> list = new List<string>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(this.GetTopBtnsSelectFashionCate(), GoodsId, Level);
		if (fashionLevelupVO != null)
		{
			string[] proPerty_Array = fashionLevelupVO.ProPerty_Array;
			for (int i = 0; i < proPerty_Array.Length; i++)
			{
				string[] array = proPerty_Array[i].Split(new char[]
				{
					','
				});
				string text = array[0];
				if (!dictionary.ContainsValue(text))
				{
					dictionary.Add(i, text);
				}
				list2.Add(double.Parse(array[1]));
			}
		}
		FashionLevelupVO fashionLevelupVO2 = ConfigFashion.Get(this.GetTopBtnsSelectFashionCate(), GoodsId, Level + 1);
		if (fashionLevelupVO2 != null)
		{
			string[] proPerty_Array2 = fashionLevelupVO2.ProPerty_Array;
			for (int j = 0; j < proPerty_Array2.Length; j++)
			{
				string[] array2 = proPerty_Array2[j].Split(new char[]
				{
					','
				});
				string text2 = array2[0];
				if (!dictionary.ContainsValue(text2))
				{
					if (dictionary.ContainsKey(j))
					{
						dictionary[j] = text2;
					}
					else
					{
						dictionary.Add(j, text2);
					}
				}
				list3.Add(double.Parse(array2[1]));
			}
		}
		if (0 < list3.Count)
		{
			for (int k = 0; k < list3.Count; k++)
			{
				if (k < list2.Count)
				{
					if (dictionary.ContainsKey(k))
					{
						if (ConfigExtPropIndexes.GetPercentByWord(dictionary[k]))
						{
							list2[k] = (double)Mathf.FloorToInt((float)list2[k] * 10000f);
							list3[k] = (double)Mathf.FloorToInt((float)list3[k] * 10000f);
							List<double> list5;
							List<double> list4 = list5 = list3;
							int num2;
							int num = num2 = k;
							double num3 = list5[num2];
							list4[num] = num3 - list2[k];
							if (0.0 > list3[k])
							{
								list3[k] = 0.0;
							}
							List<double> list7;
							List<double> list6 = list7 = list2;
							int num4 = num2 = k;
							num3 = list7[num2];
							list6[num4] = num3 / 100.0;
							List<double> list9;
							List<double> list8 = list9 = list3;
							int num5 = num2 = k;
							num3 = list9[num2];
							list8[num5] = num3 / 100.0;
							list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[k], true))
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								string.Format("{0}{1}", list2[k].ToString("f1"), "%")
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								string.Format("{0}{1}", list3[k].ToString("f1"), "%")
							})));
						}
						else
						{
							List<double> list11;
							List<double> list10 = list11 = list3;
							int num2;
							int num6 = num2 = k;
							double num7 = list11[num2];
							list10[num6] = num7 - list2[k];
							if (0.0 > list3[k])
							{
								list3[k] = 0.0;
							}
							list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[k], true))
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								list2[k].ToString()
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								list3[k].ToString()
							})));
						}
					}
				}
				else if (dictionary.ContainsKey(k))
				{
					if (ConfigExtPropIndexes.GetPercentByWord(dictionary[k]))
					{
						list3[k] = (double)Mathf.FloorToInt((float)list3[k] * 10000f);
						List<double> list13;
						List<double> list12 = list13 = list3;
						int num2;
						int num8 = num2 = k;
						double num9 = list13[num2];
						list12[num8] = num9 / 100.0;
						list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[k], true))
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format("{0}{1}", 0, "%")
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							string.Format("{0}{1}", list3[k].ToString(), "%")
						})));
					}
					else
					{
						list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[k], true))
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							"0"
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							string.Format("{0}{1}", list3[k].ToString(), string.Empty)
						})));
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < list2.Count; l++)
			{
				if (dictionary.ContainsKey(l))
				{
					if (ConfigExtPropIndexes.GetPercentByWord(dictionary[l]))
					{
						list2[l] = (double)Mathf.FloorToInt((float)list2[l] * 100f);
						list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[l], true))
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format("{0}{1}", list2[l], "%")
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							string.Format("{0}{1}", 0, "%")
						})));
					}
					else
					{
						list.Add(string.Format("{0}{1}|{2}", Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(dictionary[l], true))
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							list2[l].ToString()
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							0
						})));
					}
				}
			}
		}
		return list;
	}

	public override void Update()
	{
		base.Update();
	}

	private List<int> GetProperty(int Level, int GoodsId)
	{
		List<int> list = new List<int>();
		FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(this.GetTopBtnsSelectFashionCate(), GoodsId, Level);
		if (fashionLevelupVO != null)
		{
			string[] proPerty_Array = fashionLevelupVO.ProPerty_Array;
			for (int i = 0; i < proPerty_Array.Length; i++)
			{
				string[] array = proPerty_Array[i].Split(new char[]
				{
					','
				});
				list.Add(int.Parse(array[1]));
			}
		}
		return list;
	}

	private string GetProPertyStr(List<GoodsData> lst)
	{
		string empty = string.Empty;
		List<string> list = new List<string>();
		if (0 < lst.Count)
		{
			for (int i = 0; i < lst.Count; i++)
			{
				GoodsData goodsData = lst[i];
				FashionLevelupVO fashionLevelupVO = ConfigFashion.Get((ItemCategories)Global.GetCategoriyByGoodsID(goodsData.GoodsID), goodsData.GoodsID, goodsData.Forge_level);
				if (fashionLevelupVO != null)
				{
					string[] proPerty_Array = fashionLevelupVO.ProPerty_Array;
					for (int j = 0; j < proPerty_Array.Length; j++)
					{
						string[] array = proPerty_Array[j].Split(new char[]
						{
							','
						});
						list.Add(string.Format("{0}|{1}", array[0], array[1]));
					}
				}
			}
		}
		string text = string.Empty;
		Dictionary<string, double> dictionary = new Dictionary<string, double>();
		for (int k = 0; k < list.Count; k++)
		{
			string[] array2 = list[k].Split(new char[]
			{
				'|'
			});
			if (dictionary.ContainsKey(array2[0]))
			{
				double num = 0.0;
				if (double.TryParse(array2[1], ref num))
				{
					Dictionary<string, double> dictionary3;
					Dictionary<string, double> dictionary2 = dictionary3 = dictionary;
					string text3;
					string text2 = text3 = array2[0];
					double num2 = dictionary3[text3];
					dictionary2[text2] = num2 + num;
				}
			}
			else
			{
				double num3 = 0.0;
				if (double.TryParse(array2[1], ref num3))
				{
					dictionary.Add(array2[0], num3);
				}
			}
		}
		foreach (KeyValuePair<string, double> keyValuePair in dictionary)
		{
			if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair.Key))
			{
				string text4 = text;
				string lang = Global.GetLang("{0}：{1}%");
				Dictionary<string, double>.Enumerator enumerator;
				KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
				object extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair2.Key, true);
				KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
				text = text4 + string.Format(lang, extPropIndexesDescriptionByWord, keyValuePair3.Value * 100.0) + Environment.NewLine;
			}
			else
			{
				string text5 = text;
				string lang2 = Global.GetLang("{0}：{1}");
				Dictionary<string, double>.Enumerator enumerator;
				KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
				object extPropIndexesDescriptionByWord2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair4.Key, true);
				KeyValuePair<string, double> keyValuePair5 = enumerator.Current;
				text = text5 + string.Format(lang2, extPropIndexesDescriptionByWord2, keyValuePair5.Value) + Environment.NewLine;
			}
		}
		return text;
	}

	private void RefreshPropertyData(int Level, int GoodsId)
	{
		this.m_RightPropertyObservableCollection.Clear();
		if (GoodsId == 0)
		{
			NGUITools.SetActive(this.m_NotFullRoot, false);
			NGUITools.SetActive(this.m_AllFullSp, true);
			NGUITools.SetActive(this.m_UpNeedGoodsRoot, false);
		}
		else
		{
			this.m_Propertylabel.text = string.Empty;
			List<string> proPertyStrList = this.GetProPertyStrList(Level, GoodsId);
			int num = 0;
			if (proPertyStrList != null && proPertyStrList.Count > 0)
			{
				num = ((!("@@@!!!" == proPertyStrList[proPertyStrList.Count - 1])) ? proPertyStrList.Count : (proPertyStrList.Count - 1));
			}
			for (int i = 0; i < num; i++)
			{
				if (!string.IsNullOrEmpty(proPertyStrList[i]))
				{
					string[] array = proPertyStrList[i].Split(new char[]
					{
						'|'
					});
					FashionPropretyItem fashionPropretyItem = U3DUtils.NEW<FashionPropretyItem>();
					if (1 < array.Length)
					{
						string text = Super.ClearStringColor(array[1]);
						if ("0%" == text || "0" == text || "0.0%" == text || "0.0 " == text)
						{
							fashionPropretyItem.SetContent(array[0], string.Empty);
						}
						else
						{
							fashionPropretyItem.SetContent(array[0], array[1]);
						}
					}
					else
					{
						fashionPropretyItem.SetContent(array[0], string.Empty);
					}
					fashionPropretyItem.DraggablePanel = this.m_DraggablePanel_RightProperty;
					this.m_RightPropertyObservableCollection.AddNoUpdate(fashionPropretyItem);
				}
			}
		}
		this.FashionUpInfRefresh(GoodsId, Level + 1);
	}

	private void FashionUpInfRefresh(int GoodsId, int LevelNext)
	{
		FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(this.GetTopBtnsSelectFashionCate(), GoodsId, LevelNext);
		if (fashionLevelupVO != null)
		{
			for (int i = 0; i < this.m_UpNeedGoodsRoot.childCount; i++)
			{
				Transform child = this.m_UpNeedGoodsRoot.GetChild(i);
				if (null != child)
				{
					Object.Destroy(child.gameObject);
				}
			}
			if (fashionLevelupVO.Time == -1)
			{
				string[] array = fashionLevelupVO.NeedGoods.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(new char[]
					{
						','
					});
					int num = int.Parse(array2[0]);
					GGoodIcon ggoodIcon = FashionWardrobePart.initGood(Global.GetEmptyGoodsData(num, 0, 1, 0, 1, 1, 1, 1, 1), true);
					int num2 = 0;
					if (int.TryParse(array2[1], ref num2))
					{
						int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(num);
						ggoodIcon.SecondText.Label.supportEncoding = true;
						ggoodIcon.SText = string.Format("{1}/{0}", num2, (num2 > roleGoodsNumberCountByGoodsID) ? Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							roleGoodsNumberCountByGoodsID.ToString()
						}) : roleGoodsNumberCountByGoodsID.ToString());
					}
					this.m_UpneedGoodsNum = int.Parse(array2[1]);
					ggoodIcon.transform.SetParent(this.m_UpNeedGoodsRoot, false);
					ggoodIcon.ContentText.text = string.Empty;
					ggoodIcon.BindingSprite.gameObject.SetActive(false);
				}
			}
		}
		bool flag = null == fashionLevelupVO;
		NGUITools.SetActive(this.m_NotFullRoot, !flag);
		NGUITools.SetActive(this.m_AllFullSp, flag);
		if (flag)
		{
			if (fashionLevelupVO == null)
			{
				fashionLevelupVO = ConfigFashion.Get(this.GetTopBtnsSelectFashionCate(), GoodsId, 0);
			}
			if (fashionLevelupVO != null)
			{
				UISprite component = this.m_AllFullSp.GetComponent<UISprite>();
				if (null != component)
				{
					component.spriteName = ((fashionLevelupVO.Time == -1) ? "FullLevel" : "XianshiFashion");
				}
			}
		}
		NGUITools.SetActive(this.m_UpNeedGoodsRoot, !flag);
	}

	private int GetFashUpCost(int goodsId, int Level)
	{
		int result = 0;
		ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
		FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(topBtnsSelectFashionCate, goodsId, Level);
		if (fashionLevelupVO != null)
		{
			string[] array = fashionLevelupVO.NeedGoods.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				result = int.Parse(array[1]);
			}
		}
		return result;
	}

	private void ErrorLog(int ret)
	{
		string errMsg = StdErrorCode.GetErrMsg(ret, true, false);
		Super.HintMainText(Global.GetLang(errMsg), 10, 3);
	}

	private void UpDateFashionLst()
	{
		List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
		int num = -1;
		ObservableCollection fashionListObservableCollection = this.m_FashionListObservableCollection;
		if (0 < selectTopBtnsTypeGoodsList.Count)
		{
			if (selectTopBtnsTypeGoodsList.Count == fashionListObservableCollection.Count)
			{
				for (int i = 0; i < selectTopBtnsTypeGoodsList.Count; i++)
				{
					if (selectTopBtnsTypeGoodsList[i].Using == 1)
					{
						num = selectTopBtnsTypeGoodsList[i].GoodsID;
						break;
					}
				}
				for (int j = 0; j < fashionListObservableCollection.Count; j++)
				{
					GameObject at = fashionListObservableCollection.GetAt(j);
					if (null != at)
					{
						FashionItem component = at.GetComponent<FashionItem>();
						if (null != component)
						{
							component.IsUsing = (num == component.GoodsID);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < selectTopBtnsTypeGoodsList.Count; k++)
				{
					if (selectTopBtnsTypeGoodsList[k].Using == 1)
					{
						num = selectTopBtnsTypeGoodsList[k].GoodsID;
						break;
					}
				}
				for (int l = 0; l < fashionListObservableCollection.Count; l++)
				{
					GameObject at2 = fashionListObservableCollection.GetAt(l);
					if (null != at2)
					{
						FashionItem component2 = at2.GetComponent<FashionItem>();
						if (null != component2)
						{
							if (num == component2.GoodsID)
							{
								component2.IsUsing = true;
							}
							else
							{
								component2.IsUsing = false;
							}
						}
					}
				}
			}
		}
	}

	private void UpdateFashionWearBtnTextAndProperty()
	{
		GoodsData goodsData = null;
		if (null != this.m_LiftFashionListView.SelectedItem)
		{
			FashionItem component = this.m_LiftFashionListView.SelectedItem.GetComponent<FashionItem>();
			if (null != component)
			{
				goodsData = component._GoodsData;
			}
		}
		else
		{
			List<GoodsData> roleFashionList = Global.GetRoleFashionList(ItemCategories.Fashion);
			for (int i = 0; i < roleFashionList.Count; i++)
			{
				if (roleFashionList[i].Using == 1)
				{
					goodsData = roleFashionList[i];
					this.RefreshFrontShowMode(roleFashionList[i].GoodsID, false);
				}
			}
		}
		UILabel label = this.m_WearBtn.Label;
		string text;
		if (goodsData != null)
		{
			string lang = Global.GetLang("卸载");
			this.m_WearBtn.Label.text = lang;
			text = lang;
		}
		else
		{
			string lang = Global.GetLang("穿戴");
			this.m_WearBtn.Label.text = lang;
			text = lang;
		}
		label.text = text;
	}

	private ItemCategories GetTopBtnsSelectFashionCate()
	{
		ItemCategories result = ItemCategories.Fashion;
		if (this.mSelectTopsBtnID == 0)
		{
			result = ItemCategories.Fashion;
		}
		else if (this.mSelectTopsBtnID == 1)
		{
			result = ItemCategories.ChiBang;
		}
		else if (this.mSelectTopsBtnID == 2)
		{
			result = ItemCategories.ShiZhuang_WuQi;
		}
		else if (this.mSelectTopsBtnID == 3)
		{
			result = ItemCategories.ShiZhuang_JiaoYin;
		}
		return result;
	}

	public void RefreshLuoLanData(LuoLanChengZhuInfo item)
	{
		if (Global.Data != null && item != null && item.RoleInfoList != null && 0 < item.RoleInfoList.Count && item != null && item.RoleInfoList != null && 0 < item.RoleInfoList.Count && Global.Data.RoleID == item.RoleInfoList[0].RoleID)
		{
			this.mRoleIsLuoLanChengZhu = true;
		}
	}

	public void SetType(byte type = 0)
	{
		this.mSelectTopsBtnID = type;
		if (this.CanShowFashionUi(this.mSelectTopsBtnID))
		{
			this.LiftTopBtnHander(null, new DPSelectedItemEventArgs
			{
				ID = (int)type,
				Index = 1
			});
		}
		else
		{
			for (byte b = 0; b < 4; b += 1)
			{
				if (this.mSelectTopsBtnID != b && this.CanShowFashionUi(b))
				{
					this.LiftTopBtnHander(null, new DPSelectedItemEventArgs
					{
						ID = (int)b,
						Index = 1
					});
					break;
				}
			}
		}
	}

	public void RefreshFashionUp(string[] data)
	{
		int num = int.Parse(data[0]);
		if (num == 0)
		{
			int num2 = 0;
			int num3 = int.Parse(data[3]);
			List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
			for (int i = 0; i < selectTopBtnsTypeGoodsList.Count; i++)
			{
				GoodsData goodsData = selectTopBtnsTypeGoodsList[i];
				if (data[2] == goodsData.Id.ToString())
				{
					num2 = goodsData.GoodsID;
					goodsData.Forge_level = num3;
					this.RefreshPropertyData(goodsData.Forge_level, goodsData.GoodsID);
				}
			}
			ObservableCollection fashionListObservableCollection = this.m_FashionListObservableCollection;
			for (int j = 0; j < fashionListObservableCollection.Count; j++)
			{
				FashionItem component = fashionListObservableCollection.GetAt(j).GetComponent<FashionItem>();
				if (num2 == component.GoodsID)
				{
					component.Lev = num3;
				}
			}
			this.RefreshFrontShowMode(num2, false);
		}
		else
		{
			this.ErrorLog(num);
		}
	}

	public void RefreshRoleFashionMod(SCModGoods data)
	{
		if (data.ModType == 4)
		{
			List<GoodsData> selectTopBtnsTypeGoodsList = this.GetSelectTopBtnsTypeGoodsList();
			GoodsData goodsData = selectTopBtnsTypeGoodsList.Find((GoodsData e) => e.Id == data.ID);
			if (goodsData != null)
			{
				ObservableCollection fashionListObservableCollection = this.m_FashionListObservableCollection;
				for (int i = 0; i < fashionListObservableCollection.Count; i++)
				{
					GameObject at = fashionListObservableCollection.GetAt(i);
					if (null != at)
					{
						FashionItem component = at.GetComponent<FashionItem>();
						if (goodsData.GoodsID == component.GoodsID)
						{
							component.FashionTimeIsOver = true;
							break;
						}
					}
				}
			}
			GoodsData goodsData2 = selectTopBtnsTypeGoodsList.Find((GoodsData e) => e.Using == 1);
			if (goodsData2 == null)
			{
				this.m_WearBtn.Label.text = Global.GetLang("穿戴");
			}
			else if (goodsData2.Id == data.ID)
			{
				this.RefreshFrontShowMode(-1, false);
			}
		}
		else
		{
			this.m_LiftFashionListView.repositionNow = true;
			this.UpDateFashionLst();
			this.RefreshFrontShowMode(-1, false);
		}
	}

	public void RefreshRoleData()
	{
		Super.HideNetWaiting();
		this.InitLiftView();
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
	}

	public void RefreshFrontShowMode(int GoodsId = -1, bool BRelayLoad = false)
	{
		if (!BRelayLoad && this.m_BFristOpen)
		{
			this.m_BFristOpen = false;
			return;
		}
		ItemCategories topBtnsSelectFashionCate = this.GetTopBtnsSelectFashionCate();
		this.RefreshLuoLanYuYiBtn();
		WingData wingData = new WingData();
		wingData.AddDateTime = Global.Data.roleData.MyWingData.AddDateTime;
		wingData.DbID = Global.Data.roleData.MyWingData.DbID;
		wingData.ForgeLevel = Global.Data.roleData.MyWingData.ForgeLevel;
		wingData.JinJieFailedNum = Global.Data.roleData.MyWingData.JinJieFailedNum;
		wingData.StarExp = Global.Data.roleData.MyWingData.StarExp;
		wingData.Using = Global.Data.roleData.MyWingData.Using;
		wingData.WingID = Global.Data.roleData.MyWingData.WingID;
		wingData.ZhuHunNum = Global.Data.roleData.MyWingData.WingID;
		wingData.ZhuLingNum = Global.Data.roleData.MyWingData.ZhuLingNum;
		this.RoleModal.Clear();
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.FashionWingsID);
		UIHelper.SetModalPosZ(this.RoleModal.transform);
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
		if (GoodsId != -1)
		{
			if (fashionAndTitleList != null)
			{
				for (int i = 0; i < fashionAndTitleList.Count; i++)
				{
					GoodsData goodsData = Global.CloneGoodsData(fashionAndTitleList[i], true);
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if (topBtnsSelectFashionCate == (ItemCategories)goodsCatetoriy)
					{
						goodsData.Using = ((goodsData.GoodsID != GoodsId) ? 0 : 1);
					}
					list.Add(goodsData);
				}
				if (Global.Data.roleData.GoodsDataList != null)
				{
					for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
					{
						GoodsData goodsData2 = Global.CloneGoodsData(Global.Data.roleData.GoodsDataList[j], true);
						list.Add(goodsData2);
					}
				}
			}
		}
		else
		{
			if (fashionAndTitleList != null)
			{
				for (int k = 0; k < fashionAndTitleList.Count; k++)
				{
					GoodsData goodsData3 = Global.CloneGoodsData(fashionAndTitleList[k], true);
					list.Add(goodsData3);
				}
			}
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int l = 0; l < Global.Data.roleData.GoodsDataList.Count; l++)
				{
					GoodsData goodsData4 = Global.CloneGoodsData(Global.Data.roleData.GoodsDataList[l], true);
					list.Add(goodsData4);
				}
			}
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		if (roleCommonUseParamsValue > 0)
		{
			int num = 26;
			if (num < Global.Data.roleData.RoleCommonUseIntPamams.Count)
			{
				int fashionID = Global.Data.roleData.RoleCommonUseIntPamams[num];
				int fashionGoodsID = Global.GetFashionGoodsID(fashionID);
				this.roleResLoader = UIHelper.LoadRoleRes(this.RoleModal, Global.Data.roleData.SettingBitFlags, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, Global.Data.RoleName, list, Global.Data.equipPet, wingData, 1.5f, fashionGoodsID, new DPSelectedItemEventHandler(this.ModeLaderComplete), false);
			}
		}
		else
		{
			this.roleResLoader = UIHelper.LoadRoleRes(this.RoleModal, Global.Data.roleData.SettingBitFlags, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, Global.Data.RoleName, list, Global.Data.equipPet, wingData, 1.5f, 0, new DPSelectedItemEventHandler(this.ModeLaderComplete), false);
		}
	}

	private void ModeLaderComplete(object sender, DPSelectedItemEventArgs args)
	{
	}

	public static GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					FashionWardrobePart.ShowGoodsTip(e);
				};
			}
		}
		return ggoodIcon;
	}

	public static void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		FashionWardrobePart.ChangeTipsZ(Global.GetLang("GoodTips"), -650);
	}

	public static void ChangeTipsZ(string Title, int z)
	{
		WindowManage.Sort();
	}

	public static int FashionWardrobeOrder = 65;

	public GButton m_CloseBtn;

	public ListBox m_LiftFashionListView;

	public UIDraggablePanel m_DraggablePanel;

	public UIDraggablePanel m_DraggablePanel_RightProperty;

	public Modal3DShow RoleModal;

	public GButton m_PropertyBtn;

	public GButton m_WearBtn;

	public GButton WearBtn_LuoLan;

	public UILabel[] m_PropertyTitleLabel;

	public UILabel m_Propertylabel;

	public GButton m_FashionUpBtn;

	public Transform m_UpNeedGoodsRoot;

	public ShowNetImage[] m_BgImage;

	public ListBox m_ListBox_RightProperty;

	public GButton m_HelpBtn;

	public GameObject m_AllFullSp;

	public GameObject m_NotFullRoot;

	public GameObject[] _LiftTopPageBtn;

	public GameObject[] _LiftPageObg;

	public UIScrollBar _ScrollBar;

	private ObservableCollection m_FashionListObservableCollection;

	private ObservableCollection m_RightPropertyObservableCollection;

	private GoodsData m_FashionSelectData;

	private string[] str_Help = new string[]
	{
		Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("规则说明：")
		}),
		string.Concat(new string[]
		{
			Global.GetLang("1、背包中的时装放入衣柜后"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("不可再放入背包")
			}),
			Environment.NewLine,
			Global.GetLang("2、放入衣柜的时装属性会"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("全部叠加")
			}),
			Environment.NewLine,
			Global.GetLang("3、衣柜中积攒相同时装达到指定数量可升级"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("强")
			}),
			Environment.NewLine,
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("化时装")
			}),
			Environment.NewLine,
			Global.GetLang("4、"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("限时时装")
			}),
			Global.GetLang("同时只能放入1件，并且"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("不能强化")
			})
		})
	};

	private int m_UpneedGoodsNum;

	private bool m_BFristOpen = true;

	private List<FashionWardrobePart.BtnHander> m_ListTopBtns = new List<FashionWardrobePart.BtnHander>();

	private bool mRoleIsLuoLanChengZhu;

	private int cfgWingTabId;

	private int cfgWingId;

	private byte mSelectTopsBtnID;

	private byte mLastSelectTopsBtnID;

	private bool WearBtnCanCheck = true;

	private RoleResLoader roleResLoader;

	public DPSelectedItemEventHandler Hander;

	public class BtnHander
	{
		public BtnHander(GameObject btnObj, int tag, DPSelectedItemEventHandler Hander = null)
		{
			if (null != btnObj)
			{
				UIEventListener.Get(btnObj).onClick = delegate(GameObject o)
				{
					if (Hander != null)
					{
						Hander(o, new DPSelectedItemEventArgs
						{
							ID = tag
						});
					}
				};
				this.BtnObj = btnObj;
				this.BtnBak = btnObj.transform.FindChild("Bak").GetComponent<UISprite>();
				this.BtnText = btnObj.transform.FindChild("Label").GetComponent<UILabel>();
				this.m_tag = tag;
				this.BChose = false;
			}
		}

		public bool BChose
		{
			set
			{
				string text = string.Empty;
				string spriteName = string.Empty;
				Vector3 localPosition = this.BtnBak.transform.localPosition;
				Vector3 localPosition2 = this.BtnObj.transform.localPosition;
				if (value)
				{
					text = "fdf7dd";
					spriteName = "tongyongTabnormal";
					localPosition.y = 0f;
					localPosition2.y = 0f;
				}
				else
				{
					text = "9d8667";
					spriteName = "tongyonghover";
					localPosition.y = -4f;
					localPosition2.y = 0f;
				}
				this.BtnText.text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					this.str[this.m_tag]
				});
				this.BtnBak.spriteName = spriteName;
				this.BtnBak.transform.localPosition = localPosition;
				this.BtnObj.transform.localPosition = localPosition2;
			}
		}

		public int ID
		{
			get
			{
				return this.m_tag;
			}
		}

		public GameObject BtnObj;

		private UISprite BtnBak;

		private UILabel BtnText;

		private int m_tag;

		private string[] str = new string[]
		{
			Global.GetLang("装备"),
			Global.GetLang("翅膀"),
			Global.GetLang("武器"),
			Global.GetLang("脚印")
		};
	}
}
