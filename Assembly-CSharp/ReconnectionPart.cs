using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ReconnectionPart : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.ConstTexts != null && this.ConstTexts.Length == 5)
		{
			this.ConstTexts[0].text = Global.GetLang("系统提示");
			this.ConstTexts[1].text = Global.GetLang("可能出现以下原因：");
			this.ConstTexts[2].text = Global.GetLang("1、帐号重复登入了");
			this.ConstTexts[3].text = Global.GetLang("2、您网络不稳定,掉线了");
			this.ConstTexts[4].text = Global.GetLang("3、服务器进入维护中");
		}
		this.ConstTexts[2].pivot = 3;
		this.ConstTexts[3].pivot = 3;
		this.ConstTexts[4].pivot = 3;
		this.ConstTexts[2].transform.localPosition = new Vector3(-140f, 9f, 0f);
		this.ConstTexts[3].transform.localPosition = new Vector3(-140f, -19f, 0f);
		this.ConstTexts[4].transform.localPosition = new Vector3(-140f, -47f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		Global.Data.GameScene.CancelAutoFight(0, true);
		Global.Data.GameScene.CancelAutoFindRoad(true);
	}

	public void SetWindowState()
	{
		if (this.m_nWindowType == 0)
		{
			if (null != this.m_BtnReConn)
			{
				this.m_BtnReConn.Label.text = Global.GetLang("立即重连");
			}
			base.StartCoroutine("TickProc");
		}
		if (this.m_nWindowType == 1)
		{
			if (null != this.m_BtnReConn)
			{
				this.m_BtnReConn.Label.text = Global.GetLang("立即刷新");
			}
			if (null != this.m_LblHint)
			{
				this.m_LblHint.text = string.Format(Global.GetLang("连接已经断开,请点击{0}"), Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("立即刷新")
				}));
			}
		}
		this.InitContrl();
	}

	private void InitContrl()
	{
		Global.g_nReconnTimes++;
		if (null != this.m_BtnReConn)
		{
			this.m_BtnReConn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (MUVoiceManager.GetInstance().IsJunTuanRealTimeMap() || MUVoiceManager.GetInstance().IsZhanMengRealTimeMap())
				{
					MUVoiceManager.GetInstance().ExitRealTimeScene();
				}
				if (this.m_nWindowType == 0)
				{
					this.m_DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1
					});
				}
				if (this.m_nWindowType == 1)
				{
					this.m_DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2
					});
				}
				base.StopCoroutine("TickProc");
			};
		}
	}

	protected virtual void OnEnable()
	{
		if (this.m_nWindowType == 0)
		{
		}
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			if (null != this.m_LblHint)
			{
				if (1 >= this.m_nTick)
				{
					this.m_DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1
					});
					base.StopCoroutine("TickProc");
				}
				if (115 <= this.m_nTick)
				{
					this.m_BtnReConn.isEnabled = false;
				}
				else
				{
					this.m_BtnReConn.isEnabled = true;
				}
				this.m_LblHint.text = string.Format(Global.GetLang("连接已经断开,{0}{1}自动重连"), Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.m_nTick
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("秒")
				}));
				this.m_nTick--;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public DPSelectedItemBoolEventHandler m_DPSelectedItem;

	public GButton m_BtnReConn;

	public UILabel m_LblHint;

	public int m_nReconnTimes;

	public UILabel[] ConstTexts;

	private int m_nTick = 120;

	public int m_nWindowType;
}
