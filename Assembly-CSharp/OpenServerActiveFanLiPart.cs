using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class OpenServerActiveFanLiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnLingQv.Text = Global.GetLang("领取");
		this.m_TxtName1.Text = Global.GetLang("无");
		this.m_TxtName2.Text = Global.GetLang("无");
		this.m_TxtName3.Text = Global.GetLang("无");
		this.m_TxtName4.Text = Global.GetLang("无");
		this.m_TxtName5.Text = Global.GetLang("无");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_BtnLingQv.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.m_BtnLingQv.isEnabled)
			{
				return;
			}
			GameInstance.Game.SpriteFetchNewZoneActivityAward(37, 0);
		};
		this.m_BtnLingQv.isEnabled = false;
	}

	public void InitDataFromServerInfo(NewZoneActiveData fanLiData)
	{
		this.m_TxtFanLi.text = ((!OpenServerActiveFanLiPart.IsBetweenGainTime()) ? "0" : fanLiData.YuanBao.ToString());
		if (fanLiData.Ranklist != null && fanLiData.Ranklist.Count > 0)
		{
			int count = fanLiData.Ranklist.Count;
			if (count == 1)
			{
				this.m_TxtName1.text = fanLiData.Ranklist[0].RoleName;
			}
			else if (count == 2)
			{
				this.m_TxtName1.text = fanLiData.Ranklist[0].RoleName;
				this.m_TxtName2.text = fanLiData.Ranklist[1].RoleName;
			}
			else if (count == 3)
			{
				this.m_TxtName1.text = fanLiData.Ranklist[0].RoleName;
				this.m_TxtName2.text = fanLiData.Ranklist[1].RoleName;
				this.m_TxtName3.text = fanLiData.Ranklist[2].RoleName;
			}
			else if (count == 4)
			{
				this.m_TxtName1.text = fanLiData.Ranklist[0].RoleName;
				this.m_TxtName2.text = fanLiData.Ranklist[1].RoleName;
				this.m_TxtName3.text = fanLiData.Ranklist[2].RoleName;
				this.m_TxtName4.text = fanLiData.Ranklist[3].RoleName;
			}
			else if (count == 5)
			{
				this.m_TxtName1.text = fanLiData.Ranklist[0].RoleName;
				this.m_TxtName2.text = fanLiData.Ranklist[1].RoleName;
				this.m_TxtName3.text = fanLiData.Ranklist[2].RoleName;
				this.m_TxtName4.text = fanLiData.Ranklist[3].RoleName;
				this.m_TxtName5.text = fanLiData.Ranklist[4].RoleName;
			}
		}
		this.m_BtnLingQv.isEnabled = false;
		if (fanLiData.YuanBao > 0 && OpenServerActiveFanLiPart.IsBetweenGainTime())
		{
			this.m_BtnLingQv.isEnabled = true;
		}
	}

	public static bool GetTotalCanGainState(NewZoneActiveData fanLiData)
	{
		bool result = false;
		if (fanLiData.YuanBao > 0 && OpenServerActiveFanLiPart.IsBetweenGainTime())
		{
			result = true;
		}
		return result;
	}

	public static bool IsBetweenGainTime()
	{
		bool result = false;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime huodongTimeDateTime = Global.GetHuodongTimeDateTime(OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 0], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 1], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 2], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 3]);
		DateTime huodongTimeDateTime2 = Global.GetHuodongTimeDateTime(OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 0], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 1], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 2], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 3]);
		if (correctDateTime.CompareTo(huodongTimeDateTime) >= 0 && correctDateTime.CompareTo(huodongTimeDateTime2) <= 0)
		{
			result = true;
		}
		return result;
	}

	public void LoadConfigFromXML(string xmlPath)
	{
		this.ShowActiveTime();
	}

	private void ShowActiveTime()
	{
		string huodongTimeStr = Global.GetHuodongTimeStr(0, 0, 0, 0);
		string huodongTimeStr2 = Global.GetHuodongTimeStr(OpenServerActiveFanLiPart.XinQuHuoDongItemTime[0, 0], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[0, 1], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[0, 2], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[0, 3]);
		string text = string.Empty;
		string text2 = string.Empty;
		this.m_TxtHuoDongShiJianStart.Text = huodongTimeStr;
		this.m_TxtHuoDongShiJianEnd.Text = huodongTimeStr2;
		text = Global.GetHuodongTimeStr(OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 0], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 1], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 2], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[2, 3]);
		text2 = Global.GetHuodongTimeStr(OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 0], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 1], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 2], OpenServerActiveFanLiPart.XinQuHuoDongItemTime[4, 3]);
		this.m_TxtHuoDongLingQuShiJianStart.Text = text;
		this.m_TxtHuoDongLingQuShiJianEnd.Text = text2;
	}

	public GTextBlockOutLine m_TxtHuoDongLingQuShiJianStart;

	public GTextBlockOutLine m_TxtHuoDongLingQuShiJianEnd;

	public GTextBlockOutLine m_TxtHuoDongShiJianStart;

	public GTextBlockOutLine m_TxtHuoDongShiJianEnd;

	public GTextBlockOutLine m_TxtFanLi;

	public GTextBlockOutLine m_TxtName1;

	public GTextBlockOutLine m_TxtName2;

	public GTextBlockOutLine m_TxtName3;

	public GTextBlockOutLine m_TxtName4;

	public GTextBlockOutLine m_TxtName5;

	public GButton m_BtnLingQv;

	public DPSelectedItemEventHandler DPSelectedItem;

	private static int[,] XinQuHuoDongItemTime = new int[,]
	{
		{
			6,
			23,
			59,
			59
		},
		{
			7,
			0,
			0,
			0
		},
		{
			1,
			0,
			0,
			0
		},
		{
			8,
			23,
			59,
			59
		},
		{
			7,
			23,
			59,
			59
		}
	};
}
