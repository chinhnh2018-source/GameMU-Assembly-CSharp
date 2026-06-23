using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class CompetitionResultPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.m_ConfirmBtn.Text = Global.GetLang("离开");
		if (this.ConstTexts != null && this.ConstTexts.Length > 0)
		{
			this.SetConstTextName();
		}
	}

	protected override void InitializeComponent()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.InitTextInPrefabs();
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		this.m_ConfirmBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Exit();
		};
		base.InvokeRepeating("TimeProc", 0f, 1f);
	}

	private void TimeProc()
	{
		if (this.countDown < 0)
		{
			this.m_TimeLabel.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
			this.Exit();
		}
		this.m_TimeLabel.Text = StringUtil.substitute("{0}" + Global.GetLang("秒后关闭"), new object[]
		{
			this.countDown
		});
		this.countDown--;
	}

	private void Exit()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
	}

	private void SetConstTextName()
	{
		try
		{
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			if (mapSceneUIClass != SceneUIClasses.KuaFuTeamCompete)
			{
				if (mapSceneUIClass != SceneUIClasses.KuaFuTeamCompeteZhengBa)
				{
					if (mapSceneUIClass == SceneUIClasses.WanMoXiaGu)
					{
						this.ConstTexts[0].Text = Global.GetLang("奖励经验:");
						this.ConstTexts[1].Text = Global.GetLang("奖励绑金:");
						this.ConstTexts[2].Text = Global.GetLang("战斗用时:");
						this.trans[0].localPosition = new Vector3(-165f, 40f, 0f);
						this.trans[1].localPosition = new Vector3(41f, 40f, 0f);
						this.trans[2].localPosition = new Vector3(191f, 40f, 0f);
					}
				}
				else
				{
					this.ConstTexts[0].Text = Global.GetLang(string.Empty);
					this.ConstTexts[1].Text = Global.GetLang(string.Empty);
					this.ConstTexts[2].Text = Global.GetLang(string.Empty);
					this.ValueTexts[0].Text = Global.GetLang(string.Empty);
					this.ValueTexts[1].Text = Global.GetLang(string.Empty);
					this.ValueTexts[2].Text = Global.GetLang(string.Empty);
					this.ValueTexts[2].Pivot = 4;
					this.trans[0].localPosition = new Vector3(-108f, 40f, 0f);
					this.trans[1].localPosition = new Vector3(172f, 40f, 0f);
					this.trans[2].localPosition = new Vector3(-52f, 7f, 0f);
				}
			}
			else
			{
				this.ConstTexts[0].Text = Global.GetLang("奖励荣耀:");
				this.ConstTexts[1].Text = Global.GetLang("奖励积分:");
				this.ConstTexts[2].Text = Global.GetLang("段位:");
				this.trans[0].localPosition = new Vector3(-108f, 40f, 0f);
				this.trans[1].localPosition = new Vector3(172f, 40f, 0f);
				this.trans[2].localPosition = new Vector3(-45f, -23f, 0f);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogError<Exception>(new Exception[]
			{
				ex
			});
		}
	}

	private string GetTime(int UsedSecs)
	{
		double num = (double)UsedSecs;
		return StringUtil.substitute(Global.GetLang("{0}分{1}秒"), new object[]
		{
			(int)(num / (double)this.sMinu),
			(int)(num % (double)this.sMinu)
		});
	}

	public void InitWanMoXiaGuData(WanMoXiaGuAwardsData data)
	{
		this.ValueTexts[0].Pivot = 3;
		this.ValueTexts[1].Pivot = 3;
		this.ValueTexts[2].Pivot = 3;
		this.ValueTexts[0].X = -60.0;
		this.ValueTexts[1].X = 0.0;
		this.ValueTexts[2].X = 120.0;
		this.ValueTexts[0].Text = data.Exp.ToString();
		this.ValueTexts[1].Text = data.Money.ToString();
		this.ValueTexts[2].Text = this.GetTime(data.UsedSecs);
		if (data.Success == 0)
		{
			this.m_TitleLabel.textColor = 16711680U;
			this.AnimLose.gameObject.SetActive(true);
		}
		else if (data.Success == 1)
		{
			this.m_TitleLabel.textColor = 16381698U;
			this.AnimWin.gameObject.SetActive(true);
		}
		if (data.AwardsGoods != null && data.AwardsGoods.Count > 0)
		{
			for (int i = 0; i < data.AwardsGoods.Count; i++)
			{
				UIHelper.AddGoodsIcon(this.ItemCollection, data.AwardsGoods[i], null, false, "bagGrid4_bak");
			}
			this.mListBox.transform.localPosition = new Vector3((float)((data.AwardsGoods.Count - 1) * -80), this.mListBox.transform.localPosition.y, this.mListBox.transform.localPosition.z);
		}
	}

	public void InitTeamCompeteData(TianTi5v5AwardsData data)
	{
		this.ValueTexts[0].Text = data.RongYao.ToString();
		this.ValueTexts[1].Text = (data.LianShengJiFen + data.DuanWeiJiFen).ToString();
		this.ValueTexts[2].Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		if (data.Success == 0)
		{
			this.m_TitleLabel.textColor = 16711680U;
			this.AnimLose.gameObject.SetActive(true);
		}
		else if (data.Success == 1)
		{
			this.m_TitleLabel.textColor = 16381698U;
			this.AnimWin.gameObject.SetActive(true);
		}
	}

	public void InitTeamCompeteZhengBaData(ZhanDuiZhengBaAwardsData data)
	{
		this.trans[0].gameObject.SetActive(false);
		this.trans[1].gameObject.SetActive(false);
		if (data.Success == 0)
		{
			this.m_TitleLabel.textColor = 16711680U;
			this.AnimLose.gameObject.SetActive(true);
			this.ValueTexts[2].Text = this.GetDes(data.NewGrade, false);
		}
		else if (data.Success == 1)
		{
			this.m_TitleLabel.textColor = 16381698U;
			this.AnimWin.gameObject.SetActive(true);
			this.ValueTexts[2].Text = this.GetDes(data.NewGrade, true);
		}
	}

	private string GetDes(int grade, bool isSuccess)
	{
		if (grade == 1)
		{
			return (!isSuccess) ? Global.GetLang("获得亚军") : Global.GetLang("成功夺得荣耀之王");
		}
		if (grade == 2)
		{
			return (!isSuccess) ? Global.GetLang("获得亚军") : Global.GetLang("成功晋级决赛");
		}
		return (!isSuccess) ? Global.GetString(new object[]
		{
			Global.GetLang("止步"),
			grade,
			Global.GetLang("强")
		}) : Global.GetString(new object[]
		{
			Global.GetLang("成功晋级"),
			grade,
			Global.GetLang("强")
		});
	}

	public GButton m_ConfirmBtn;

	public TextBlock m_TimeLabel;

	public TextBlock m_TitleLabel;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblPaiming;

	public Animator AnimWin;

	public Animator AnimLose;

	public TextBlock[] ConstTexts;

	public TextBlock[] ValueTexts;

	private int countDown = 10;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public Transform[] trans;

	public Transform bak;

	private int sMinu = 60;
}
