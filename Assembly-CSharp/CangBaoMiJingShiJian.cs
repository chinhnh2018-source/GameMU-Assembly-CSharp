using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class CangBaoMiJingShiJian : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.initXML();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void initXML()
	{
		this.xml_FuBen = Global.GetGameResXml("Config/FuBen.Xml");
		this.xml_Event = Global.GetGameResXml("GameRes/Config/Treasure/TreasureEvent.xml");
	}

	private void InitPrefabText()
	{
	}

	private void InitHandler()
	{
		this.m_Mask.enabled = false;
		UIEventListener.Get(this.m_Mask.gameObject).onClick = delegate(GameObject e)
		{
			this.Btnhandler(e, new DPSelectedItemEventArgs
			{
				ID = 0,
				MyID = this.m_EventID
			});
		};
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent d)
		{
			this.Btnhandler(e, new DPSelectedItemEventArgs
			{
				ID = 0,
				MyID = this.m_EventID
			});
		};
		this.m_SureBtn.MouseLeftButtonUp = delegate(object e, MouseEvent d)
		{
			this.Btnhandler(e, new DPSelectedItemEventArgs
			{
				ID = 1,
				MyID = this.m_EventID
			});
		};
	}

	private string GetEventString(Data_CangBao_Event data)
	{
		return this.GetXMLItem(this.xml_Event, "TreasureEvent", "ID", "Descriptio", data.ID.ToString());
	}

	private string GetXMLItem(XElement xml, string NewNode1, string NewNode2, string NewNode3, string itemvalue)
	{
		if (xml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(xml, NewNode1);
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (itemvalue == Global.GetXElementAttributeStr(xelementList[i], NewNode2))
				{
					return Global.GetXElementAttributeStr(xelementList[i], NewNode3);
				}
			}
		}
		return string.Empty;
	}

	public void RefreshContent(Data_CangBao_Event data)
	{
		this.m_EventType = data.Type;
		this.m_EventID = data.ID;
		string empty = string.Empty;
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		switch (this.EventType)
		{
		case 2:
			empty = this.strThings1;
			text = this.GetEventString(data);
			text2 = this.m_Str_Title[0];
			text3 = this.m_Str_Btn[0];
			this.m_NPCSp.URL = "NetImages/GameRes/Images/CangBaoMiJing/NPC.png";
			break;
		case 4:
			empty = this.strThings3;
			text = this.GetEventString(data);
			text2 = this.m_Str_Title[1];
			text3 = this.m_Str_Btn[1];
			this.m_NPCSp.URL = "NetImages/GameRes/Images/CangBaoMiJing/BOSS.png";
			break;
		}
		this.m_Title.text = text2;
		this.m_SureBtn.Label.text = text3;
		this.m_ContentLabel1.text = empty;
		this.m_Contentlabel2.text = text;
	}

	public int EventType
	{
		get
		{
			return this.m_EventType;
		}
	}

	public ShowNetImage m_NPCSp;

	public UISprite m_Mask;

	public UILabel m_Title;

	public UILabel m_ContentLabel1;

	public UILabel m_Contentlabel2;

	public GButton m_CloseBtn;

	public GButton m_SureBtn;

	private string[] m_Str_Title = new string[]
	{
		Global.GetLang("交易兑换"),
		Global.GetLang("遭遇BOSS")
	};

	private string[] m_Str_Btn = new string[]
	{
		Global.GetLang("确定"),
		Global.GetLang("战斗")
	};

	private string strThings1 = Global.GetColorStringForNGUIText(new object[]
	{
		"e3b36c",
		Global.GetLang("冒险者，要不要做一笔超值的买卖？")
	});

	private string strThings3 = Global.GetColorStringForNGUIText(new object[]
	{
		"e3b36c",
		Global.GetLang("你竟敢闯进我的领地，你这是自寻死路！")
	});

	private int m_EventID;

	private int m_EventType;

	private XElement xml_FuBen;

	private XElement xml_Event;

	public DPSelectedItemEventHandler Btnhandler;

	public enum EventTitleType
	{
		ETET_Excharge,
		ETET_Combat
	}

	public enum TreasureEventType
	{
		ETET_Null,
		ETET_Award,
		ETET_Excharge,
		ETET_Move,
		ETET_Combat,
		ETET_TreasureBox
	}
}
