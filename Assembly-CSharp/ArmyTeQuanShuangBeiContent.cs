using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyTeQuanShuangBeiContent : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
	}

	private void OnEnable()
	{
		base.CancelInvoke("TickProcs");
		this.SendDoubleCaiJiTimeRequest();
	}

	private void SendDoubleCaiJiTimeRequest()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetDoubleCaiJiTime();
	}

	public void ShowDoubleCaiJiTimeByServer(LingZhuOpenData data)
	{
		if (data == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		this.SetDetailTime(data.BeginTime, data.EndTime);
		this.ServerMsgTips(data.OpenType);
		string chineseText = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("本周剩余开启次数：")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			data.LeftCount
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("次")
		}));
		this.m_Times.Text = Global.GetLang(chineseText);
		if (data.OpenType == -4)
		{
			this.SetButtonStatus(Global.GetLang("已开启"), 2);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			long num = data.DoubleOpenEndTime.Ticks - correctDateTime.Ticks;
			this.secondsCountDown = (int)(num / 10000000L);
			base.InvokeRepeating("TickProcs", 0f, 1f);
		}
	}

	protected void TickProcs()
	{
		this.m_Time.text = string.Format(Global.GetLang("剩余时间： {0}"), Global.GetTimeStrBySecEx((double)this.secondsCountDown, true, -1));
		this.secondsCountDown--;
		if (this.secondsCountDown < 0)
		{
			base.CancelInvoke("TickProcs");
			this.m_Time.gameObject.SetActive(false);
			this.SetButtonStatus(Global.GetLang("已结束"), 0);
		}
	}

	public void OpenDoubleCaiJiResultByServer(int result, int times = 0)
	{
	}

	private void ServerMsgTips(int result)
	{
		this.m_Time.gameObject.SetActive(false);
		switch (result + 9)
		{
		case 0:
			Super.HintMainText(Global.GetLang("不在领地地图"), 10, 3);
			return;
		case 1:
			Super.HintMainText(Global.GetLang("不存在领地数据"), 10, 3);
			return;
		case 2:
			Super.HintMainText(Global.GetLang("存库失败"), 10, 3);
			return;
		case 3:
			Super.HintMainText(Global.GetLang("领主数据不存在"), 10, 3);
			return;
		case 4:
			this.SetButtonStatus(Global.GetLang("已结束"), 0);
			return;
		case 5:
			this.SetButtonStatus(Global.GetLang("已开启"), 2);
			this.m_Time.gameObject.SetActive(true);
			return;
		case 6:
			Super.HintMainText(Global.GetLang("不在开启时间内"), 10, 3);
			return;
		case 7:
			Super.HintMainText(Global.GetLang("不是领主"), 10, 3);
			return;
		case 8:
			Super.HintMainText(Global.GetLang("跨服服务器连接错误"), 10, 3);
			return;
		case 10:
			this.SetButtonStatus(Global.GetLang("开启双倍"), 1);
			return;
		}
		MUDebug.Log(new object[]
		{
			"军团特权未处理错误类型：",
			result
		});
	}

	private void InitTextInPrefabs()
	{
		this.m_TimeTips.Text = Global.GetLang("双倍采集可开启时间段：12:00~24:00，周日不能开启双倍  ");
		this.m_BtnOpen.Text = Global.GetLang("开启双倍");
		this.m_Times.Text = Global.GetLang("本周剩余开启次数：2次");
	}

	private void SetDetailTime(TimeSpan begin, TimeSpan end)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("每天")
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			begin
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			"~"
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			end
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("可开启双倍采集，")
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			Global.GetLang("周日")
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("不能开启双倍，每次双倍时间持续 ")
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			1
		}));
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("小时")
		}));
		this.m_TimeTips.Text = stringBuilder.ToString();
	}

	private void InitBtnCallBack()
	{
		this.m_BtnOpen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.ShowNetWaiting(null);
		};
	}

	public void InitValue(string content)
	{
		this.m_TimeTips.Text = Global.GetLang(content);
		this.SetActivity(this.IsSunday());
	}

	public void SetButtonStatus(string content, int type)
	{
		this.m_BtnOpen.Text = Global.GetLang(content);
		switch (type)
		{
		case 0:
			this.m_BtnOpen.enabled = false;
			this.m_BtnOpen.target.spriteName = "btn_press2";
			this.m_BtnOpen.GetComponent<BoxCollider>().enabled = false;
			break;
		case 1:
			this.m_BtnOpen.enabled = true;
			this.m_BtnOpen.target.spriteName = "btn_normal";
			this.m_BtnOpen.GetComponent<BoxCollider>().enabled = true;
			break;
		case 2:
			this.m_BtnOpen.enabled = false;
			this.m_BtnOpen.target.spriteName = "btn_press2";
			this.m_BtnOpen.GetComponent<BoxCollider>().enabled = false;
			break;
		}
	}

	private bool IsSunday()
	{
		return Global.GetCorrectDateTime().DayOfWeek == 0;
	}

	private void SetActivity(bool isShow)
	{
		this.m_BtnOpen.target.color = ((!isShow) ? Color.gray : Color.white);
		this.m_BtnOpen.gameObject.GetComponent<BoxCollider>().enabled = isShow;
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("TickProcs");
		this.m_TimeTips = null;
		this.m_Time = null;
		this.m_BtnOpen = null;
	}

	public TextBlock m_TimeTips;

	public TextBlock m_Time;

	public GButton m_BtnOpen;

	public TextBlock m_Times;

	private int secondsCountDown;
}
