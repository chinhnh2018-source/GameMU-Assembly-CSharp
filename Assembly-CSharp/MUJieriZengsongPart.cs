using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriZengsongPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitPerfabText()
	{
		this.labNum.Text = string.Empty;
		this.InputBox.label.text = Global.GetLang("输入好友昵称");
		this.InputBox.defaultText = Global.GetLang("输入好友昵称");
		this.btnZengsong.Text = Global.GetLang("赠送礼物");
		this.labNum.text = Global.GetLang("今日已赠送数量: {0}");
		this.InputBox.label.transform.localPosition = new Vector3(30f, this.InputBox.label.transform.localPosition.y, this.InputBox.label.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPerfabText();
		this.labNum.gameObject.transform.localPosition = new Vector3(130f, 90f, 0f);
		this.ItemCollection = this.rewardList.ItemsSource;
		UIEventListener.Get(this.FriendIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowFriendWindow();
		};
		this.btnZengsong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.InputBox.text == string.Empty)
			{
				Super.HintMainText(Global.GetLang("请输入赠送好友昵称！"), 10, 3);
			}
			else
			{
				this.ShowPiliangWindow();
			}
		};
	}

	public void InitData(string strXML)
	{
		XElement xelement = XElement.Parse(strXML);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间: "),
			"ffffff",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("领取时间: "),
			"ffffff",
			this.awardStartStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.awardEndStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			XElement xelement3 = xelementList2[i];
			if (xelement3 == null)
			{
				return;
			}
			MUJieriZengsongItem mujieriZengsongItem = U3DUtils.NEW<MUJieriZengsongItem>();
			this.ItemCollection.Add(mujieriZengsongItem);
			this.listItem.Add(mujieriZengsongItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement3, "ID");
			this.goodID = Global.GetXElementAttributeStr(xelement3, "Goods");
			string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "GoodsOne");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement3, "GoodsTwo");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement3, "GoodsThr");
			string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement3, "EffectiveTime");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement3, "Num");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr6))
			{
				goodsIDs = xelementAttributeStr5 + "@" + xelementAttributeStr6;
			}
			else
			{
				goodsIDs = xelementAttributeStr5;
			}
			mujieriZengsongItem.Id = xelementAttributeInt2;
			mujieriZengsongItem.Need = xelementAttributeInt3;
			mujieriZengsongItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			Super.LoadGoodsList(goodsIDs, mujieriZengsongItem.ItemCollection);
			Super.LoadOtherGoodsList(xelementAttributeStr7, mujieriZengsongItem.ItemCollection, xelementAttributeStr8);
			UIPanel component = mujieriZengsongItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
		this.ADDGoodsICON(Global.SafeConvertToInt32(this.goodID));
	}

	private void ADDGoodsICON(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.isAutoSize = true;
			icon.BackSpriteName0 = backSpriteName;
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			icon.ItemCode = goodsID;
			icon.ItemObject = dummyGoodsData;
			icon.BoxTypes = 5;
			icon.TextShadowColor = 4278190080U;
			icon.TextColor = 16777215U;
			icon.DisableTextColor = 8421504U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			icon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			icon.SecondText.text = totalGoodsCountByID.ToString();
			icon.SecondText.gameObject.SetActive(true);
			icon.TextShadowColor = 4278190080U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			this.goodIcon = icon;
			this.GiftGood.Add(icon);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = icon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			icon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
		}
	}

	private void ShowFriendWindow()
	{
		if (this.FriendWindow != null)
		{
			this.CloseChildWindow(this.FriendWindow);
			this.FriendWindow = null;
			this._FriendPart = null;
		}
		if (this.FriendWindow == null && this._FriendPart == null)
		{
			this.FriendWindow = U3DUtils.NEW<GChildWindow>();
			this.FriendWindow.IsShowModal = true;
			this.InitChildWindow(this.FriendWindow, "jieriZengsongFriend", false);
			this.Container.Add(this.FriendWindow);
			this.FriendWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.FriendWindow);
				return true;
			};
			this.FriendWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.FriendWindow);
				return true;
			};
			this.FriendWindow.ModalType = ChildWindowModalType.TransBak;
			this._FriendPart = U3DUtils.NEW<MUJieriZengsongFriendPart>();
			this._FriendPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.InputBox.text = e.Title;
				this.CloseChildWindow(this.FriendWindow);
				this.FriendWindow = null;
				this._FriendPart = null;
			};
			this.FriendWindow.SetContent(this.FriendWindow.BodyPresenter, this._FriendPart, 0.0, 0.0, true);
		}
	}

	private void CloseFriendWindow()
	{
		if (this.FriendWindow != null)
		{
			this.CloseChildWindow(this.FriendWindow);
			this.FriendWindow = null;
			this._FriendPart = null;
		}
	}

	private void ShowPiliangWindow()
	{
		if (this.PiliangWindow != null)
		{
			this.CloseChildWindow(this.PiliangWindow);
			this.PiliangWindow = null;
			this._MUJieriZengsongPiliangPart = null;
		}
		if (this.PiliangWindow == null && this._MUJieriZengsongPiliangPart == null)
		{
			this.PiliangWindow = U3DUtils.NEW<GChildWindow>();
			this.PiliangWindow.IsShowModal = true;
			this.InitChildWindow(this.PiliangWindow, "jierizengsongPiliang", false);
			this.Container.Add(this.PiliangWindow);
			this.PiliangWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.PiliangWindow);
				return true;
			};
			this.PiliangWindow.ModalType = ChildWindowModalType.Translucent;
			this._MUJieriZengsongPiliangPart = U3DUtils.NEW<MUJieriZengsongPiliangPart>();
			this._MUJieriZengsongPiliangPart.GoodsID = this.goodID;
			this._MUJieriZengsongPiliangPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.CloseChildWindow(this.PiliangWindow);
					this.PiliangWindow = null;
					this._MUJieriZengsongPiliangPart = null;
				}
				if (e.ID == 1)
				{
					int idtype = e.IDType;
					string text = this.InputBox.text;
					this.CloseChildWindow(this.PiliangWindow);
					GameInstance.Game.SendJieriZengsongCmd(text, Global.SafeConvertToInt32(this.goodID), idtype);
				}
			};
			this.PiliangWindow.SetContent(this.PiliangWindow.BodyPresenter, this._MUJieriZengsongPiliangPart, 0.0, 0.0, true);
		}
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		Super.CloseChildWindow(this.Container, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
	}

	public void GetDataInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetJieriZengsongInfoCmd();
	}

	public void setBtnState(int totalGive, int totalRecv, int flag, int result = 100)
	{
		Super.HideNetWaiting();
		this.labNum.text = string.Format(Global.GetLang("今日已赠送数量: {0}"), totalGive);
		int count = this.listItem.Count;
		for (int i = 0; i < count; i++)
		{
			MUJieriZengsongItem mujieriZengsongItem = this.listItem[i];
			if (totalGive >= mujieriZengsongItem.Need)
			{
				if (Global.GetIntSomeBit(flag, i + 1) == 0)
				{
					mujieriZengsongItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
				else
				{
					mujieriZengsongItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
			}
		}
		if (result == 0)
		{
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(Global.SafeConvertToInt32(this.goodID));
			this.goodIcon.SecondText.text = totalGoodsCountByID.ToString();
			Super.HintMainText(Global.GetLang("赠送成功"), 10, 3);
		}
		else if (result == 1)
		{
			Super.HintMainText(Global.GetLang("不是活动时间"), 10, 3);
		}
		else if (result == 6)
		{
			Super.HintMainText(Global.GetLang("接收者不能是自己"), 10, 3);
		}
		else if (result == 3)
		{
			Super.HintMainText(Global.GetLang("赠送物品错误"), 10, 3);
		}
		else if (result == 4)
		{
			Super.HintMainText(Global.GetLang("赠送物品不足"), 10, 3);
		}
		else if (result == 7)
		{
			Super.HintMainText(Global.GetLang("服务器异常"), 10, 3);
		}
		else if (result == 5)
		{
			Super.HintMainText(Global.GetLang("接收者不存在"), 10, 3);
		}
	}

	public void SetLingquResult(int result, int index)
	{
		switch (result)
		{
		case 0:
		{
			MUJieriZengsongItem mujieriZengsongItem = this.listItem[index - 1];
			mujieriZengsongItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			break;
		}
		case 1:
			Super.HintMainText(Global.GetLang("活动未开启"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("不是领奖时间"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("数据库出错"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("服务器配置出错"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("背包不足"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("不满足领奖条件"), 10, 3);
			break;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite FriendIcon;

	public SpriteSL GiftGood;

	public TextBox InputBox;

	public GButton btnZengsong;

	public ListBox rewardList;

	public TextBlock labNum;

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	public GChildWindow FriendWindow;

	public MUJieriZengsongFriendPart _FriendPart;

	public GChildWindow PiliangWindow;

	public MUJieriZengsongPiliangPart _MUJieriZengsongPiliangPart;

	private List<MUJieriZengsongItem> listItem = new List<MUJieriZengsongItem>();

	private string goodID = string.Empty;

	private GGoodIcon goodIcon;

	private ObservableCollection _ItemCollection;
}
