using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class HunYanItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void SetData(MarryPartyData m_data)
	{
		this.Data = m_data;
		DateTime dateTime;
		dateTime..ctor(this.Data.StartTime * 10000L);
		XElement gameResXml = Global.GetGameResXml("Config/WeddingFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WeddingFeasttAward");
		XElement xelement = xelementList[this.Data.PartyType - 1];
		this.TotalNum = Global.GetXElementAttributeInt(xelement, "SumNum");
		this.CanUseNum = Global.GetXElementAttributeInt(xelement, "UseNum");
		this.Time.text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
		this.HuasbandName.text = Global.GetLang("{3e89dd}♂") + this.Data.HuasbandName + "{-}";
		this.WifeName.text = Global.GetLang("{dd36bc}♀") + this.Data.WifeName + "{-}";
		this.LeftNum.text = (this.TotalNum - this.Data.JoinCount).ToString();
		this.HaveJoinTotalNum = this.Data.JoinCount;
		this.GuiMoIcon.spriteName = string.Format("hunyan_0{0}", Global.GetXElementAttributeInt(xelement, "ID"));
		if (Global.Data.HunYanJointTimes.ContainsKey(this.Data.RoleID))
		{
			this.HaveUseNum = Global.Data.HunYanJointTimes[this.Data.RoleID];
			this.CanYuNum.text = this.HaveUseNum + "/" + this.CanUseNum;
			if (this.HaveUseNum == this.CanUseNum)
			{
				this.CanYuNum.color = new Color(1f, 0f, 0f);
			}
		}
		else
		{
			this.CanYuNum.text = "0/" + this.CanUseNum;
		}
	}

	public UISprite GuiMoIcon;

	public UILabel Time;

	public UILabel HuasbandName;

	public UILabel WifeName;

	public UILabel LeftNum;

	public UILabel CanYuNum;

	[HideInInspector]
	public MarryPartyData Data;

	public int CanUseNum;

	public int TotalNum;

	public int HaveUseNum;

	public int HaveJoinTotalNum;
}
