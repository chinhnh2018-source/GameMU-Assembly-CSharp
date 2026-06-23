using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class BattleEndPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (this._Bak)
		{
			this._Bak.transform.localScale = Super.GetScreenSize();
		}
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.Text = Global.GetLang("确定");
		this._AwardsTitle.X = -220.0;
		this._JiaoTuanScore.X = -288.0;
		this._MyScore.X = -288.0;
	}

	protected void Close(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void InitPartData(string Title, bool success, int score, int expr, int chengjiu, int JiaoTuanScore, int mengJunScore, int moJing, int paiMing, string awardsGoods)
	{
		string text = (paiMing >= 5) ? Global.GetLang("无") : (paiMing + 1).ToString();
		this._MyPaiMing.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("我的排名："),
			"dac7ae",
			text
		});
		this._MyScore.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("我的得分："),
			"dac7ae",
			score
		});
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		taskAwardsData.MoJingaward = moJing;
		taskAwardsData.Experienceaward = (long)expr;
		taskAwardsData.RongYuaward = chengjiu;
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
		this._JiaoTuanScore.Text = string.Format(Global.GetLang("教团总分：{0}"), JiaoTuanScore);
		this._MengJunScore.Text = string.Format(Global.GetLang("盟军总分：{0}"), mengJunScore);
		this._AwardsTitle.Text = Global.GetLang("战斗奖励：");
		List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(awardsGoods, 0, int.MaxValue);
		UIHelper.AddAwardGoods2(this._AwardsGoodsList.ItemsSource, goodsList, null, false, "bagGrid4_bak");
		this.TimeLimit = 20;
		this._Time.Text = string.Format("{0}", this.TimeLimit);
		base.InvokeRepeating("TickProc", 1f, 1f);
		if (success)
		{
			this.AnimWin.gameObject.SetActive(true);
		}
		else
		{
			this.AnimLose.gameObject.SetActive(true);
		}
	}

	private void TickProc()
	{
		if (this.TimeLimit-- > 0)
		{
			this._Time.Text = string.Format(Global.GetLang("{0}秒后关闭"), this.TimeLimit);
		}
		else
		{
			this.Close(this, MouseEvent.Empty);
		}
	}

	public UISprite _Bak;

	public GButton _Submit;

	public TextBlock _MengJunScore;

	public TextBlock _JiaoTuanScore;

	public ListBox _AwardsGoodsList;

	public ListBox _AwardsList;

	public TextBlock _AwardsTitle;

	public TextBlock _Desc;

	public TextBlock _Time;

	public TextBlock _MyScore;

	public TextBlock _MyPaiMing;

	public Animator AnimWin;

	public Animator AnimLose;

	private int TimeLimit = 20;

	public DPSelectedItemEventHandler DPSelectedItem;
}
