using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class BattleEndYongZhePart : UserControl
{
	private void SetMVP(string name, int occupation, int roleSex)
	{
		if (!string.IsNullOrEmpty(name))
		{
			NGUITools.SetActive(this.mMvp.gameObject, true);
			this.mName.Text = name;
			string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(occupation),
				roleSex
			});
			this.mTouXiang.URL = url;
		}
		else
		{
			NGUITools.SetActive(this.mName.gameObject, false);
			NGUITools.SetActive(this.mMvp.gameObject, false);
			NGUITools.SetActive(this.mTouXiang.transform.parent.gameObject, false);
		}
	}

	private void InitMVP()
	{
		NGUITools.SetActive(this.mMvp.gameObject, false);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitMVP();
		if (this._Bak)
		{
			this._Bak.transform.localScale = Super.GetScreenSize();
		}
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.Text = Global.GetLang("领取奖励");
		UIEventListener.Get(this.BtnClose.gameObject).onClick = delegate(GameObject s1)
		{
			this.CloseWindow();
		};
	}

	protected void Close(object sender, MouseEvent e)
	{
		if (this._Submit.Text == Global.GetLang("领取奖励"))
		{
			TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_AWARD_GET.SendDataUseRoleID();
		}
		this.CloseWindow();
	}

	public void InitPartData(string Title, bool success, int score, long expr, int chengjiu, int JiaoTuanScore, int mengJunScore, int moJing, int paiMing, string awardsGoods, int JinBi, YongZheZhanChangAwardsData resultData)
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
		taskAwardsData.Experienceaward = expr;
		taskAwardsData.RongYuaward = chengjiu;
		taskAwardsData.Moneyaward = JinBi;
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
		int num = ConfigSystemParam.GetSystemParamByName("WarriorBattleLowestJiFen", true).SafeToInt32(0);
		this._JiaoTuanScore.Text = string.Format(Global.GetLang("教团总分：{0}"), JiaoTuanScore);
		this._MengJunScore.Text = string.Format(Global.GetLang("盟军总分：{0}"), mengJunScore);
		this._AwardsTitle.Text = ((score < num) ? (Global.GetLang("战斗奖励：") + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("您的积分"),
			"fd010c",
			string.Format(Global.GetLang("低于{0}"), num),
			"dac7ae",
			Global.GetLang(",无法获得奖励")
		})) : Global.GetLang("战斗奖励："));
		if (score < num)
		{
			this._Submit.Text = Global.GetLang("关闭");
		}
		if (awardsGoods != string.Empty)
		{
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(awardsGoods, 0, int.MaxValue);
			UIHelper.AddAwardGoods2(this._AwardsGoodsList.ItemsSource, goodsList, null, false, "bagGrid4_bak");
		}
		this.TimeLimit = 20;
		this._Time.Text = string.Format("{0}", this.TimeLimit);
		if (success)
		{
			this.AnimWin.gameObject.SetActive(true);
		}
		else
		{
			this.AnimLose.gameObject.SetActive(true);
		}
		if (resultData != null)
		{
			this.SetMVP(resultData.MvpRoleName, resultData.MvpOccupation, resultData.MvpRoleSex);
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

	private void CloseWindow()
	{
		if (PlayZone.GlobalPlayZone.YongzheZhanChang != null)
		{
			Super.CloseChildWindow(PlayZone.GlobalPlayZone.Children, PlayZone.GlobalPlayZone.YongzheZhanChang);
		}
	}

	public UISprite _Bak;

	public GButton _Submit;

	public UIButton BtnClose;

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

	public TextBlock mName;

	public ShowNetImage mTouXiang;

	public UISprite mMvp;

	public DPSelectedItemEventHandler DPSelectedItem;
}
