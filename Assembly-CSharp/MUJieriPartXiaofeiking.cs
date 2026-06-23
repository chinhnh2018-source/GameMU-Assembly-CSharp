using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartXiaofeiking : UserControl
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

	public string ThisXmlName
	{
		get
		{
			return this.thisXmlName;
		}
		set
		{
			this.thisXmlName = value;
			GameInstance.Game.SpriteQueryJieriXiaoFeiKing();
			Super.ShowNetWaiting(null);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.rewardList.ItemsSource;
	}

	private void InitData(string strXML)
	{
		this.ItemCollection.Clear();
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
		if (Global.InLimitTimeRange(this.awardStartStr, this.awardEndStr))
		{
			this.isInLingqu = true;
		}
		else
		{
			this.isInLingqu = false;
		}
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间："),
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
			Global.GetLang("领取时间:"),
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
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr5
		});
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList3.Count; i++)
		{
			XElement xelement4 = xelementList3[i];
			if (xelement4 == null)
			{
				return;
			}
			MUJieriPartXiaofeikingItem mujieriPartXiaofeikingItem = U3DUtils.NEW<MUJieriPartXiaofeikingItem>();
			this.ItemCollection.Add(mujieriPartXiaofeikingItem);
			this.listItem.Add(mujieriPartXiaofeikingItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "ID");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "MinYuanBao");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr7))
			{
				goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
			}
			else
			{
				goodsIDs = xelementAttributeStr6;
			}
			mujieriPartXiaofeikingItem.GoodsIDs = goodsIDs;
			mujieriPartXiaofeikingItem.Id = xelementAttributeInt2;
			mujieriPartXiaofeikingItem.Need = Global.GetColorStringForNGUIText(new object[]
			{
				"f2e1bd",
				string.Format(Global.GetLang("最低消费{0}钻石"), xelementAttributeInt3)
			});
			mujieriPartXiaofeikingItem.RoleName = this.GetKingNameByPaiHang(i + 1);
			mujieriPartXiaofeikingItem.IsInLingqu = this.isInLingqu;
			UIPanel component = mujieriPartXiaofeikingItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void setBtnState(JieriCZKingData result)
	{
		Super.HideNetWaiting();
		if (result != null)
		{
			this.state = result;
			this.InitData(this.ThisXmlName);
			this.setState();
		}
		else
		{
			this.InitData(this.ThisXmlName);
			this.setNullState();
		}
	}

	private string GetKingNameByPaiHang(int paiHang)
	{
		string result = Global.GetLang("无");
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHang = this.state.ListPaiHang;
		if (listPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHang.Count; i++)
		{
			if (listPaiHang[i] != null && paiHang == listPaiHang[i].PaiHang)
			{
				result = Global.FormatRoleName(listPaiHang[i].MaxLevelRoleZoneID, listPaiHang[i].MaxLevelRoleName);
				break;
			}
		}
		return result;
	}

	private bool IsMeByPaiHang()
	{
		bool result = false;
		if (this.state == null)
		{
			return result;
		}
		this.listKingPaiHang = this.state.ListPaiHang;
		if (this.listKingPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < this.listKingPaiHang.Count; i++)
		{
			if (this.listKingPaiHang[i] != null && Global.Data.RoleID.ToString() == this.listKingPaiHang[i].UserID)
			{
				result = true;
				this.rank = i;
			}
		}
		return result;
	}

	private void setState()
	{
		bool flag = this.IsMeByPaiHang();
		int paiHang = this.listKingPaiHang[this.rank].PaiHang;
		MUJieriPartXiaofeikingItem mujieriPartXiaofeikingItem = this.listItem[paiHang - 1];
		mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
		for (int i = 0; i < this.listItem.Count; i++)
		{
			if (flag && i == paiHang - 1)
			{
				if (this.state.State > 0)
				{
					mujieriPartXiaofeikingItem = this.listItem[paiHang - 1];
					mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				if (this.state.State <= 0)
				{
					mujieriPartXiaofeikingItem = this.listItem[paiHang - 1];
					mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			if (i != paiHang - 1)
			{
				mujieriPartXiaofeikingItem = this.listItem[i];
				mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
		}
	}

	private void setNullState()
	{
		for (int i = 0; i < this.listItem.Count; i++)
		{
			MUJieriPartXiaofeikingItem mujieriPartXiaofeikingItem = this.listItem[i];
			mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
		}
	}

	public void setCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriPartXiaofeikingItem mujieriPartXiaofeikingItem = this.listItem[position - 1];
		mujieriPartXiaofeikingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public TextBlock descText;

	public ListBox rewardList;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private bool isInLingqu = true;

	private List<MUJieriPartXiaofeikingItem> listItem = new List<MUJieriPartXiaofeikingItem>();

	private JieriCZKingData state;

	private List<InputKingPaiHangData> listKingPaiHang = new List<InputKingPaiHangData>();

	private int rank;

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
