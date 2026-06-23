using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhongShenZhengBaPart_Tishi : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.PlayerJion.Label != null)
		{
			this.PlayerJion.Text = Global.GetLang("立即战斗");
		}
		if (this.JingXiangJion.Label != null)
		{
			this.JingXiangJion.Text = Global.GetLang("镜像出战");
		}
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("提示")
		});
		this.Miaoshu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("已匹配到对手，请进入赛场比武")
		});
		this.PlayerJion.GetComponentInChildren<UILabel>().text = Global.GetLang("立即战斗");
		this.JingXiangJion.GetComponentInChildren<UILabel>().text = Global.GetLang("镜像出战");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.PlayerJion.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.PlayerJionMouseButtonEvent);
		this.JingXiangJion.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.JingXiangJionMouseButtonEvent);
	}

	public void ServerDataHandleEvent(int gameid, int day, int changci, int hasMirror)
	{
		this.CountDown();
		this.gameID = gameid;
		this.Changci.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			string.Format(Global.GetLang("众神争霸第{0}天，第{1}轮"), day, changci)
		});
		if (hasMirror == 0)
		{
			this.JingXiangJion.isEnabled = false;
		}
		else
		{
			this.JingXiangJion.isEnabled = true;
		}
	}

	private void PlayerJionMouseButtonEvent(object s, MouseEvent e)
	{
		GameInstance.Game.GetZhengBaEnter(this.gameID, 1);
		this.CloseHandler(this, new DPSelectedItemEventArgs());
	}

	private void JingXiangJionMouseButtonEvent(object s, MouseEvent e)
	{
		GameInstance.Game.GetZhengBaEnter(this.gameID, 2);
		this.CloseHandler(this, new DPSelectedItemEventArgs());
	}

	private void CountDown()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	private IEnumerator TickProc()
	{
		for (;;)
		{
			if (this.Time)
			{
				if (this.count < 1)
				{
					base.StopCoroutine("TickProc");
					GameInstance.Game.GetZhengBaEnter(this.gameID, 2);
					this.CloseHandler(this, new DPSelectedItemEventArgs());
				}
				this.Time.text = string.Format(Global.GetLang("{0}秒"), Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.count
				}));
				this.count--;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Close;

	public GButton PlayerJion;

	public GButton JingXiangJion;

	public UILabel Title;

	public UILabel Changci;

	public UILabel Time;

	public UILabel Miaoshu;

	private int count = 15;

	private int gameID;
}
