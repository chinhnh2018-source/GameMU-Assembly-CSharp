using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MingXiangAwardBar : UserControl
{
	public string Time
	{
		set
		{
			this._Time.text = value;
		}
	}

	public string Expr
	{
		set
		{
			this._Expr.text = value;
		}
	}

	public string XingHun
	{
		set
		{
			if (null != this._XingHun)
			{
				this._XingHun.text = value;
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this._BtnLingQu.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._XingHunAwards = UIHelper.AddTextAwards(this._AwardList.ItemsSource, AwardsTypes.XingHun, 0L, "CTextAwards2");
		this._XingHunAwards.TextColor = new Color32(218, 199, 174, byte.MaxValue);
		this._XingHunAwards._Text.pivot = 5;
		this._XingHunAwards._Text.transform.localPosition = new Vector3(100f, 0f, 0f);
		this._XingHunAwards._Text.transform.localScale = new Vector3(16f, 16f, 1f);
		this._XingHunAwards._Icon.transform.localPosition = new Vector3(-12f, 0f, 0f);
		this._ExpAwards = UIHelper.AddTextAwards(this._AwardList.ItemsSource, AwardsTypes.Exp, 0L, "CTextAwards2");
		this._ExpAwards.TextColor = new Color32(218, 199, 174, byte.MaxValue);
		this._ExpAwards._Text.pivot = 5;
		this._ExpAwards._Text.transform.localPosition = new Vector3(100f, 0f, 0f);
		this._ExpAwards._Text.transform.localScale = new Vector3(16f, 16f, 1f);
		this._ExpAwards._Icon.transform.localPosition = new Vector3(-3f, 0f, 0f);
		this._BtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Handler != null)
			{
				this.Handler(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		this._BtnShowWindow.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Handler != null)
			{
				this.Handler(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		UIEventListener.Get(this.mCancel.gameObject).onClick = delegate(GameObject s)
		{
			GameInstance.Game.SpriteStartMeditateCmd(0);
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.CloseMingXiangAwardBar();
			}
		};
	}

	public void InitPartData(int IconCode)
	{
	}

	public void SetAwardInfo(long secs1, long secs2)
	{
		int num;
		long mingXiangExpr = Global.GetMingXiangExpr(out num);
		long num2 = (secs1 + secs2) / 60L * 60L;
		long num3 = (long)((int)(secs1 / 60L)) * mingXiangExpr;
		num3 += (long)((int)(secs2 / 60L)) * mingXiangExpr;
		int num4 = (int)(secs1 / 60L) * num;
		num4 += (int)(secs2 / 60L) * num;
		string text = UIHelper.FormatSecsShort(num2, Global.GetLang("0分钟"));
		if (num2 >= 43200L)
		{
			this.Time = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("收益中:  "),
				"dac7ae",
				Global.GetLang("12小时"),
				"17e43e",
				Global.GetLang("\n(已满)")
			});
		}
		else
		{
			this.Time = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("收益中:  "),
				"dac7ae",
				text
			});
		}
		this._ExpAwards._Text.text = this.ConvertToValue(num3);
		this._XingHunAwards._Text.text = this.ConvertToValue((long)num4);
	}

	public string ConvertToValue(long number)
	{
		string empty = string.Empty;
		int length = number.ToString().Length;
		if (length >= 6)
		{
			return string.Format("{0}{1}", number / 10000L, Global.GetLang("万"));
		}
		return number.ToString();
	}

	public DPSelectedItemEventArgs args = new DPSelectedItemEventArgs();

	public DPSelectedItemEventHandler Handler;

	public TextBlock _Time;

	public TextBlock _Expr;

	public TextBlock _JinBi;

	public UISprite mCancel;

	public TextBlock _XingHun;

	public ListBox _AwardList;

	public GButton _BtnLingQu;

	public GButton _BtnShowWindow;

	private CText _ExpAwards;

	private CText _XingHunAwards;
}
