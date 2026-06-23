using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class HongBaoPaiHangPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
		base.InitializeComponent();
	}

	public void SetXml(string str = "")
	{
		XElement xelement;
		if (string.IsNullOrEmpty(str))
		{
			xelement = Global.GetGameResXml("Config/JieRiGifts/JieRiHongBaoBang.xml");
		}
		else
		{
			xelement = XElement.Parse(str);
		}
		try
		{
			XElement xelement2 = Global.GetXElement(xelement, "Activities");
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "RedPacketList");
			for (int i = 0; i < xelementList.Count; i++)
			{
				JieRiHongBaoBang jieRiHongBaoBang = new JieRiHongBaoBang();
				jieRiHongBaoBang.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				jieRiHongBaoBang.Ranking = Global.GetXElementAttributeInt(xelementList[i], "Ranking");
				jieRiHongBaoBang.MinNum = Global.GetXElementAttributeInt(xelementList[i], "Threshold");
				jieRiHongBaoBang.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
				jieRiHongBaoBang.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
				jieRiHongBaoBang.GoodsThr = Global.GetXElementAttributeStr(xelementList[i], "GoodsThr");
				jieRiHongBaoBang.EffectiveTime = Global.GetXElementAttributeStr(xelementList[i], "EffectiveTime");
				this.m_List.Add(jieRiHongBaoBang);
			}
			this.m_LabTime1.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("活动时间：")
			})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetXElementAttributeStr(xelement2, "FromDate") + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("至")
				}) + Global.GetXElementAttributeStr(xelement2, "ToDate")
			}));
			this.m_LabTime2.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("领奖时间：")
			})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetXElementAttributeStr(xelement2, "AwardStartDate") + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("至")
				}) + Global.GetXElementAttributeStr(xelement2, "AwardEndDate")
			}));
			this.m_LabContent.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("活动内容：")
			})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetXElementAttributeStr(Global.GetXElement(xelement, "GiftList"), "Description")
			}));
			if (Global.GetCorrectLocalTime() > Global.SafeConvertDateTime(Global.GetXElementAttributeStr(xelement2, "AwardStartDate")).Ticks / 10000L && Global.GetCorrectLocalTime() < Global.SafeConvertDateTime(Global.GetXElementAttributeStr(xelement2, "AwardEndDate")).Ticks / 10000L)
			{
				this.m_OnTimeBool = true;
			}
			else
			{
				this.m_OnTimeBool = false;
			}
			this.SetData();
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void AddItem(JieriHongBaoKingData data)
	{
		if (this.m_List.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.m_List.Count; i++)
		{
			bool flag = true;
			HongBaoPaiHangItem item = U3DUtils.NEW<HongBaoPaiHangItem>();
			this.m_ObservableCollection.AddNoUpdate(item);
			if (data.RankList != null)
			{
				for (int j = 0; j < data.RankList.Count; j++)
				{
					if (data.RankList[j].Rank == this.m_List[i].Ranking)
					{
						item.XmlData = this.m_List[i];
						item.LingQuData = data.RankList[j];
						flag = false;
					}
				}
			}
			if (flag)
			{
				item.XmlData = this.m_List[i];
				JieriHongBaoKingItemData jieriHongBaoKingItemData = new JieriHongBaoKingItemData();
				jieriHongBaoKingItemData.RoleID = -1;
				jieriHongBaoKingItemData.Rank = this.m_List[i].Ranking;
				jieriHongBaoKingItemData.Rolename = Global.GetLang("无");
				jieriHongBaoKingItemData.GetAwardTimes = 0;
				item.LingQuData = jieriHongBaoKingItemData;
			}
			UIPanel component = item.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			item.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (item.ID == Global.Data.RoleID)
				{
					GameInstance.Game.LingQuHongBaoPaiHang(s.ID);
				}
			};
			if (item.ID == Global.Data.RoleID && !this.m_OnTimeBool)
			{
				item.SetisEnabled(1);
			}
		}
	}

	private void SetData()
	{
		GameInstance.Game.SendHongBaoPaiHang();
	}

	public void Refresh(JieriHongBaoKingData data)
	{
		this.m_LabXianZhi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("已抢红包数量：") + data.SelfCount
		}));
		this.AddItem(data);
	}

	public void LingQuRefresh(int ret)
	{
		if (ret == 0)
		{
			for (int i = 0; i < this.m_ObservableCollection.Count; i++)
			{
				HongBaoPaiHangItem component = this.m_ObservableCollection[i].GetComponent<HongBaoPaiHangItem>();
				if (component.ID == Global.Data.RoleID)
				{
					component.SetisEnabled(0);
				}
			}
		}
		else if (ret == -2001)
		{
			Super.HintMainText(Global.GetLang("不在有效时间范围内"), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
	}

	public UILabel m_LabTime1;

	public UILabel m_LabTime2;

	public UILabel m_LabContent;

	public ListBox m_ListBox;

	public UILabel m_LabXianZhi;

	public bool m_OnTimeBool;

	private ObservableCollection m_ObservableCollection;

	private List<JieRiHongBaoBang> m_List = new List<JieRiHongBaoBang>();
}
