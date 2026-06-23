using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriZengsongZengKingPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.depositBtn.Text = Global.GetLang("充值");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
		this.labNum.gameObject.transform.localPosition = new Vector3(230f, 28f, -1f);
		this.depositBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			MUDebug.Log<string>(new string[]
			{
				"充值界面"
			});
		};
	}

	public void InitData(string strXML, int _type)
	{
		this.ItemCollection.Clear();
		this.ActivityType = _type;
		string attributeName = string.Empty;
		if (_type == 3)
		{
			attributeName = "ID";
			NGUITools.SetActive(this.labNum.gameObject, false);
		}
		else
		{
			attributeName = "Ranking";
		}
		NGUITools.SetActive(this.depositBtn.gameObject, 3 == _type);
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
		if (_type == 3)
		{
			this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("领奖方式: "),
				"ffffff",
				Global.GetLang("奖励将会在活动结束由客服人员统计后使用邮件发放")
			});
			this.lingquEndtime.text = string.Empty;
		}
		else
		{
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
		}
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
			MUJieriZengsongZengKingItem mujieriZengsongZengKingItem = U3DUtils.NEW<MUJieriZengsongZengKingItem>();
			this.ItemCollection.Add(mujieriZengsongZengKingItem);
			this.listItem.Add(mujieriZengsongZengKingItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, attributeName);
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "MinYuanBao");
			string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement4, "GoodsThr");
			string xelementAttributeStr9 = Global.GetXElementAttributeStr(xelement4, "EffectiveTime");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr7))
			{
				goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
			}
			else
			{
				goodsIDs = xelementAttributeStr6;
			}
			mujieriZengsongZengKingItem.Id = xelementAttributeInt2;
			mujieriZengsongZengKingItem.Need = xelementAttributeInt3;
			mujieriZengsongZengKingItem.RoleName = Global.GetLang("无");
			mujieriZengsongZengKingItem.IsInLingqu = this.isInLingqu;
			mujieriZengsongZengKingItem.ActivityType = _type;
			if (_type == 3)
			{
				mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.NotNeedGain;
			}
			else
			{
				mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
			mujieriZengsongZengKingItem.SetLabText(xelementAttributeInt3, _type);
			Super.LoadGoodsList(goodsIDs, mujieriZengsongZengKingItem.ItemCollection);
			Super.LoadOtherGoodsList(xelementAttributeStr8, mujieriZengsongZengKingItem.ItemCollection, xelementAttributeStr9);
			for (int j = 0; j < mujieriZengsongZengKingItem.ItemCollection.Count; j++)
			{
				GGoodIcon component = mujieriZengsongZengKingItem.ItemCollection.GetAt(j).GetComponent<GGoodIcon>();
				if (component && component.GetComponent<UIPanel>())
				{
					Object.Destroy(component.GetComponent<UIPanel>());
				}
			}
			if (!this.DicRankToItem.ContainsKey(xelementAttributeInt2))
			{
				this.DicRankToItem.Add(xelementAttributeInt2, mujieriZengsongZengKingItem);
			}
			UIPanel component2 = mujieriZengsongZengKingItem.GetComponent<UIPanel>();
			if (component2)
			{
				Object.Destroy(component2);
			}
		}
	}

	public void GetDataInfo()
	{
		if (this.ActivityType == 1)
		{
			GameInstance.Game.GetJieriZengsongKingInfoCmd();
		}
		else if (this.ActivityType == 2)
		{
			GameInstance.Game.GetJieriShouliKingInfoCmd();
		}
		else if (this.ActivityType == 3)
		{
			GameInstance.Game.GetJieriPingTaiKingCmd();
		}
		Super.ShowNetWaiting(null);
	}

	public void SetBak(string bakName)
	{
		this.bak.ImageURL = string.Format("NetImages/GameRes/Images/Plate/{0}.jpg", bakName);
		this.bak.gameObject.SetActive(false);
		this.bak.gameObject.SetActive(true);
	}

	public void GetZengKingData(object ZengKingData)
	{
		Super.HideNetWaiting();
		if (ZengKingData == null)
		{
			return;
		}
		if (ZengKingData is JieriGiveKingData)
		{
			JieriGiveKingData jieriGiveKingData = ZengKingData as JieriGiveKingData;
			if (jieriGiveKingData.RankingList != null)
			{
				this.RankingList = jieriGiveKingData.RankingList;
			}
			if (jieriGiveKingData.MyData != null)
			{
				this.MyData = jieriGiveKingData.MyData;
			}
			this.SetBtnState();
		}
		if (ZengKingData is JieriRecvKingData)
		{
			JieriRecvKingData jieriRecvKingData = ZengKingData as JieriRecvKingData;
			if (jieriRecvKingData.RankingList != null)
			{
				this.RankingListRec = jieriRecvKingData.RankingList;
			}
			if (jieriRecvKingData.MyData != null)
			{
				this.MyDataRec = jieriRecvKingData.MyData;
			}
			this.SetBtnStateRec();
		}
	}

	private void SetBtnStateRec()
	{
		this.labNum.text = string.Format(Global.GetLang("已收取总数量: {0}"), (this.MyDataRec != null) ? this.MyDataRec.TotalRecv : 0);
		if (this.RankingListRec == null || this.listItem == null)
		{
			return;
		}
		MUJieriZengsongZengKingItem mujieriZengsongZengKingItem = null;
		int count = this.listItem.Count;
		for (int i = 0; i < this.RankingListRec.Count; i++)
		{
			JieriRecvKingItemData jieriRecvKingItemData = this.RankingListRec[i];
			this.DicRankToItem.TryGetValue(jieriRecvKingItemData.Rank, ref mujieriZengsongZengKingItem);
			mujieriZengsongZengKingItem.RoleName = jieriRecvKingItemData.Rolename;
			if (jieriRecvKingItemData.RoleID == Global.Data.RoleID)
			{
				if (jieriRecvKingItemData.GetAwardTimes > 0)
				{
					mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else
				{
					mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
		}
	}

	private void SetBtnState()
	{
		this.labNum.text = string.Format(Global.GetLang("已赠送总数量: {0}"), (this.MyData != null) ? this.MyData.TotalGive : 0);
		if (this.RankingList == null || this.listItem == null)
		{
			return;
		}
		MUJieriZengsongZengKingItem mujieriZengsongZengKingItem = null;
		int count = this.listItem.Count;
		for (int i = 0; i < this.RankingList.Count; i++)
		{
			JieriGiveKingItemData jieriGiveKingItemData = this.RankingList[i];
			this.DicRankToItem.TryGetValue(jieriGiveKingItemData.Rank, ref mujieriZengsongZengKingItem);
			mujieriZengsongZengKingItem.RoleName = jieriGiveKingItemData.Rolename;
			if (jieriGiveKingItemData.RoleID == Global.Data.RoleID)
			{
				if (jieriGiveKingItemData.GetAwardTimes > 0)
				{
					mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else
				{
					mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
		}
	}

	public void GetPingTaiKingData(List<InputKingPaiHangData> listData)
	{
		Super.HideNetWaiting();
		if (listData == null)
		{
			return;
		}
		MUJieriZengsongZengKingItem mujieriZengsongZengKingItem = null;
		for (int i = 0; i < listData.Count; i++)
		{
			InputKingPaiHangData inputKingPaiHangData = listData[i];
			if (this.DicRankToItem.TryGetValue(inputKingPaiHangData.PaiHang, ref mujieriZengsongZengKingItem))
			{
				ZtBuffServerInfo ztBuffServerInfo = null;
				int maxLevelRoleZoneID = inputKingPaiHangData.MaxLevelRoleZoneID;
				string maxLevelRoleName = inputKingPaiHangData.MaxLevelRoleName;
				if (maxLevelRoleZoneID > 0 && Global.GetNowServerIsZhuTiFu(maxLevelRoleZoneID, out ztBuffServerInfo))
				{
					mujieriZengsongZengKingItem.RoleName = Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, maxLevelRoleName, 0);
				}
				else
				{
					mujieriZengsongZengKingItem.RoleName = Global.GetColorStringForNGUIText(new object[]
					{
						"ffffff",
						"S" + maxLevelRoleZoneID,
						"99ccff",
						maxLevelRoleName
					});
				}
			}
		}
	}

	public void SetResultState(int result, int ID)
	{
		MUJieriZengsongZengKingItem mujieriZengsongZengKingItem = null;
		switch (result)
		{
		case 0:
			this.DicRankToItem.TryGetValue(ID, ref mujieriZengsongZengKingItem);
			mujieriZengsongZengKingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			break;
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

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public TextBlock descText;

	public ListBox rewardList;

	public ShowNetImage bak;

	public TextBlock labNum;

	public GButton depositBtn;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private bool isInLingqu;

	private List<MUJieriZengsongZengKingItem> listItem = new List<MUJieriZengsongZengKingItem>();

	private Dictionary<int, MUJieriZengsongZengKingItem> DicRankToItem = new Dictionary<int, MUJieriZengsongZengKingItem>();

	private ObservableCollection _ItemCollection;

	private int ActivityType = 1;

	private List<JieriGiveKingItemData> RankingList;

	private JieriGiveKingItemData MyData;

	private List<JieriRecvKingItemData> RankingListRec;

	private JieriRecvKingItemData MyDataRec;
}
