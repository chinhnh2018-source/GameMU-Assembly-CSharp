using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class JingjiResultPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_ConfirmBtn.Text = Global.GetLang("离开");
		if (this.ConstTexts != null && this.ConstTexts.Length == 2)
		{
			this.ConstTexts[0].Text = Global.GetLang("奖励声望:");
			this.ConstTexts[1].Text = Global.GetLang("奖励经验:");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		this.m_ConfirmBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteGetJingJiJunxianLeaveCmd();
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		};
		base.InvokeRepeating("TimeProc", 0f, 1f);
	}

	public void init(int isWin, int shengWangValue, int expValue, int paiming)
	{
		this.m_JingyanLabel.Text = string.Empty + expValue;
		this.m_ShengwangLabel.Text = string.Empty + shengWangValue;
		if (isWin == 0)
		{
			this.m_TitleLabel.Text = Global.GetLang("竞技场战斗失败");
			this.m_TitleLabel.textColor = 16711680U;
			this.lblPaiming.text = Global.GetLang("排名不变：") + ((paiming >= 0) ? (string.Empty + paiming) : Global.GetLang("500名后"));
			this.AnimLose.gameObject.SetActive(true);
		}
		else if (isWin == 1)
		{
			this.m_TitleLabel.Text = Global.GetLang("竞技场战斗胜利");
			this.m_TitleLabel.textColor = 16381698U;
			this.lblPaiming.text = Global.GetLang("排名提升至：") + ((paiming >= 0) ? (string.Empty + paiming) : Global.GetLang("500名后"));
			this.AnimWin.gameObject.SetActive(true);
		}
		Global.PlaySoundAudio("Audio/UI/JingJiChangSuccess", false);
	}

	protected void TimeProc()
	{
		if (this.countDown < 0)
		{
			this.m_TimeLabel.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
		}
		this.m_TimeLabel.Text = StringUtil.substitute("{0}" + Global.GetLang("秒后关闭"), new object[]
		{
			this.countDown
		});
		this.countDown--;
	}

	public GButton m_ConfirmBtn;

	public TextBlock m_JingyanLabel;

	public TextBlock m_TimeLabel;

	public TextBlock m_ShengwangLabel;

	public TextBlock m_TitleLabel;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UILabel lblPaiming;

	public Animator AnimWin;

	public Animator AnimLose;

	public TextBlock[] ConstTexts;

	public Transform bak;

	private int countDown = 10;
}
