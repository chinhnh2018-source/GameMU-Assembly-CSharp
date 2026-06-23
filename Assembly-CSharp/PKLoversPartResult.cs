using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class PKLoversPartResult : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnClose.Label.text = Global.GetLang("离开");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.CountDown();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void DisposeServerData(CoupleArenaPkResultData Data)
	{
		if (Data == null)
		{
			return;
		}
		if (Data.PKResult == 0)
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(false);
		}
		else if (Data.PKResult == 1)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else if (Data.PKResult == 2)
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		this.Miaoshu1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("奖励荣耀：{0}"), Data.GetRongYao)
		});
		this.Miaoshu2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("奖励积分：{0}"), Data.GetJiFen)
		});
		this.Miaoshu4.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format("{0}", PKLoversPart.GetCoupleDuanWeiTypeDic()[Data.DuanWeiType].Name)
		});
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
					this.CloseHandler(this, new DPSelectedItemEventArgs());
				}
				this.Time.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("{0}秒"), this.count)
				});
				this.count--;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public Animator AnimWin;

	public Animator AnimLose;

	public UILabel Miaoshu1;

	public UILabel Miaoshu2;

	public UILabel Miaoshu4;

	public UILabel Time;

	public GButton BtnClose;

	private int count = 5;
}
