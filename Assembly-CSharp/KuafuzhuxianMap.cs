using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Xml;
using UnityEngine;

public class KuafuzhuxianMap : UserControl
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

	public int MapLineID
	{
		get
		{
			return this.maplineid;
		}
		set
		{
			this.maplineid = value;
			this.InitBtnItem(this.maplineid);
		}
	}

	public bool BComeFromTTaskBoxMini
	{
		get
		{
			return this.m_BComeFromTTaskBoxMini;
		}
		set
		{
			this.m_BComeFromTTaskBoxMini = value;
		}
	}

	public int BossID
	{
		get
		{
			return this.bossID;
		}
		set
		{
			this.bossID = value;
		}
	}

	public int TeleportID
	{
		get
		{
			return this.teleportID;
		}
		set
		{
			this.teleportID = value;
		}
	}

	public int MapCode
	{
		get
		{
			return this.mapCode;
		}
		set
		{
			this.mapCode = value;
			GameInstance.Game.GetMapInfo(this.mapCode);
		}
	}

	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("请选择线路")
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._ItemCollection = this.rewardList.ItemsSource;
		if (this.Btnclose != null)
		{
			this.Btnclose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectItem(this, new DPSelectedItemEventArgs
				{
					Type = -10
				});
			};
		}
		this.InitXml();
	}

	private void InitXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/MapLine.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "MapLine");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			MapLine mapLine = new MapLine();
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MaxNum");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Line");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Name");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
				mapLine.ID = xelementAttributeInt2;
				mapLine.Name = xelementAttributeStr2;
				mapLine.Line = xelementAttributeStr;
				mapLine.MaxNum = xelementAttributeInt;
				if (!this.dic_MapLine.ContainsKey(xelementAttributeInt2))
				{
					this.dic_MapLine.Add(xelementAttributeInt2, mapLine);
				}
			}
		}
	}

	private void InitBtnItem(int id)
	{
		if (this.dic_MapLine == null || this.dic_MapLine.Keys.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<int, MapLine> keyValuePair in this.dic_MapLine)
		{
			MapLine value = keyValuePair.Value;
			if (id == value.ID)
			{
				string[] lines = value.Line.Split(new char[]
				{
					'|'
				});
				this.InitBtnItems(lines);
			}
		}
	}

	private void InitBtnItems(string[] Lines)
	{
		for (int i = 0; i < Lines.Length; i++)
		{
			int num = int.Parse(Lines[i].Split(new char[]
			{
				','
			})[1]);
			int num2 = int.Parse(Lines[i].Split(new char[]
			{
				','
			})[0]);
			KuafuzhuxianMapItem kuafuzhuxianMapItem = U3DUtils.NEW<KuafuzhuxianMapItem>();
			kuafuzhuxianMapItem.BossID = this.BossID;
			kuafuzhuxianMapItem.TeleportID = this.TeleportID;
			kuafuzhuxianMapItem.DPS = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.FindRoad(this, new DPSelectedItemEventArgs());
			};
			kuafuzhuxianMapItem.BtnMap.GetComponent<UIDragPanelContents>().enabled = false;
			this._ItemCollection.Add(kuafuzhuxianMapItem);
			UIPanel component = kuafuzhuxianMapItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void SetBtnState(List<KuaFuLineData> data)
	{
		if (data == null)
		{
			return;
		}
		int count = this._ItemCollection.Count;
		int stateValue = -2;
		for (int i = 0; i < count; i++)
		{
			this.mapItem = U3DUtils.AS<KuafuzhuxianMapItem>(this._ItemCollection[i]);
			if (i > data.Count - 1)
			{
				this.mapItem.StateValue = -2;
				this.mapItem.MapCode = 50;
				this.mapItem.LineNum = i + 1;
			}
			else
			{
				this.GetStateValue(data[i], out stateValue);
				this.mapItem.StateValue = stateValue;
				this.mapItem.MapCode = data[i].MapCode;
				this.mapItem.LineNum = data[i].LineID;
			}
		}
		if (this.BAutoFindOneRode)
		{
			if (0 < data.Count)
			{
				int onlineCount = data[0].OnlineCount;
				for (int j = 0; j < data.Count; j++)
				{
					if (onlineCount > data[j].OnlineCount)
					{
						onlineCount = data[j].OnlineCount;
						int num = data[j].LineID;
					}
				}
			}
			for (int k = 0; k < count; k++)
			{
				this.mapItem = U3DUtils.AS<KuafuzhuxianMapItem>(this._ItemCollection[k]);
				if (this.lineID == this.mapItem.LineNum)
				{
					this.mapItem.DPS(this.mapCode, new DPSelectedItemEventArgs());
				}
			}
			this.BAutoFindOneRode = false;
		}
	}

	private void GetStateValue(KuaFuLineData data, out int stateValue)
	{
		stateValue = -2;
		if (data.State != 1)
		{
			return;
		}
		if ((double)data.OnlineCount == (double)data.MaxOnlineCount * 0.8)
		{
			stateValue = 0;
			return;
		}
		stateValue = (((double)data.OnlineCount >= (double)data.MaxOnlineCount * 0.8) ? -1 : 1);
	}

	public void PromptInfo(int error)
	{
		if (error == 0)
		{
			return;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足进入条件！"), new object[0]), 0, -1, -1, 0);
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public DPSelectedItemEventHandler FindRoad;

	public GButton Btnclose;

	public UILabel Title;

	public ListBox rewardList;

	public KuafuzhuxianMapItem mapItem;

	public Dictionary<int, MapLine> dic_MapLine = new Dictionary<int, MapLine>();

	private int lineID;

	private ObservableCollection _ItemCollection;

	private int maplineid;

	public bool BAutoFindOneRode;

	private bool m_BComeFromTTaskBoxMini;

	private int bossID;

	private int teleportID;

	private int mapCode;
}
