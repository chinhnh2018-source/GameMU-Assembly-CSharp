using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhongShenZhengBaPartResult : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnChouQu.Label.text = Global.GetLang("抽取编号");
		this.BtnClose.Label.text = Global.GetLang("关闭");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.CountDown();
		this.InitTextInPrefabs();
		this.BtnClose.gameObject.SetActive(false);
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnChouQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = "UITeXiao/Perfabs/zhongshenzhengba/zhongshenzhengba_bianhao";
			Object original = Resources.Load(text);
			GameObject gameObject = SpawnManager.Instantiate(original) as GameObject;
			GameObject gameObject2 = SpawnManager.Instantiate(original) as GameObject;
			gameObject.transform.SetParent(this.obj1.transform, false);
			gameObject2.transform.SetParent(this.obj2.transform, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			U3DUtils.ResetLayer(gameObject2, "MUUI");
			if (this.ResultData != null)
			{
				this.shiwei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					this.ResultData.RandGroup / 10
				});
				this.gewei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					this.ResultData.RandGroup % 10
				});
			}
			this.BtnChouQu.gameObject.SetActive(false);
			this.BtnClose.gameObject.SetActive(true);
		};
	}

	public void DisposeServerData(ZhengBaNtfPkResultData Data)
	{
		if (Data == null)
		{
			return;
		}
		this.shiwei.text = string.Empty;
		this.gewei.text = string.Empty;
		this.ResultData = Data;
		if (Data.RandGroup >= 1 && Data.RandGroup <= 16)
		{
			this.BtnChouQu.gameObject.SetActive(true);
			this.choujiangBg.gameObject.SetActive(true);
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
			this.shiwei.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				0
			});
			this.gewei.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				0
			});
			this.Miaoshu1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("恭喜您成功晋级众神争霸16强")
			});
			this.Miaoshu2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("请点击下方按钮抽取16强对战编号")
			});
			return;
		}
		if (Data.IsUpGrade)
		{
			this.BtnChouQu.gameObject.SetActive(false);
			this.choujiangBg.gameObject.SetActive(false);
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
			this.Miaoshu1.text = string.Empty;
			this.Miaoshu2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("恭喜您，成功晋级"), Data.LeftUpGradeNum)
			});
			return;
		}
		if (Data.LeftUpGradeNum > 0 && !Data.IsUpGrade)
		{
			this.BtnChouQu.gameObject.SetActive(false);
			this.choujiangBg.gameObject.SetActive(false);
			if (Data.IsWin)
			{
				this.AnimWin.gameObject.SetActive(true);
				this.AnimLose.gameObject.SetActive(false);
			}
			else
			{
				this.AnimWin.gameObject.SetActive(false);
				this.AnimLose.gameObject.SetActive(true);
			}
			this.Miaoshu1.text = string.Empty;
			this.Miaoshu2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("仍然还需要胜利{0}场才能晋级"), Data.StillNeedWin)
			});
			return;
		}
		if (Data.LeftUpGradeNum == 0 && !Data.IsUpGrade)
		{
			this.BtnChouQu.gameObject.SetActive(false);
			this.choujiangBg.gameObject.SetActive(false);
			if (Data.IsWin)
			{
				this.AnimWin.gameObject.SetActive(true);
				this.AnimLose.gameObject.SetActive(false);
			}
			else
			{
				this.AnimWin.gameObject.SetActive(false);
				this.AnimLose.gameObject.SetActive(true);
			}
			this.Miaoshu1.text = string.Empty;
			this.Miaoshu2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("非常遗憾，您没能晋级")
			});
			return;
		}
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

	public UILabel Time;

	public UILabel shiwei;

	public UILabel gewei;

	public UISprite choujiangBg;

	public GButton BtnChouQu;

	public GButton BtnClose;

	public GameObject obj1;

	public GameObject obj2;

	private int count = 10;

	private ZhengBaNtfPkResultData ResultData;
}
