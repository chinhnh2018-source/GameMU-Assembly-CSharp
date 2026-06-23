using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class MoYuZhanDouEnd : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblClose.text = Global.GetLang("点击空白处关闭");
		this.lblZhanDuiScoreWord.text = Global.GetLang("战队得分 :");
		this.lblZhanDuiRankWord.text = Global.GetLang(string.Empty);
		this.lblSelfScoreWord.text = Global.GetLang("自己得分 :");
		this.lblJiangLiWord.text = Global.GetLang("获得奖励 :");
		this.lblBossWord.text = Global.GetLang("击杀头领奖励 :");
		this.lblSelfScoreWord.pivot = 5;
		this.lblSelfScoreWord.transform.localPosition = new Vector3(190f, this.lblSelfScoreWord.transform.localPosition.y, this.lblSelfScoreWord.transform.localPosition.z);
		this.lblSelfScore.pivot = 3;
		this.lblSelfScore.transform.localPosition = new Vector3(190f, this.lblSelfScore.transform.localPosition.y, this.lblSelfScore.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.goClose).onClick = new UIEventListener.VoidDelegate(this.OnClose);
	}

	private void OnClose(GameObject go)
	{
		if (this.ChildWindowClose != null)
		{
			this.ChildWindowClose(this, null);
		}
		if (this.CoseHander != null)
		{
			this.CoseHander.Invoke();
		}
	}

	public void SetInfo(int zhanDuiScore, bool beWin, int selfScore, List<GoodsData> reward, List<GoodsData> boss, bool isFirst = false)
	{
		if (isFirst)
		{
			this.lblJiangLiWord.text = Global.GetLang("首胜奖励 :");
		}
		this.lblZhanDuiScore.text = zhanDuiScore.ToString();
		this.lblZhanDuiRankWord.text = ((!beWin) ? Global.GetLang("失败") : Global.GetLang("胜利"));
		this.lblSelfScore.text = selfScore.ToString();
		if (reward != null)
		{
			Global.LoadReward(reward, this.gridReward, 70f, true);
		}
		if (boss != null)
		{
			if (boss.Count == 0)
			{
				this.goBoss.SetActive(false);
			}
			else
			{
				this.goBoss.SetActive(true);
				Global.LoadReward(boss, this.gridBoss, 70f, true);
			}
		}
		else
		{
			this.goBoss.SetActive(false);
		}
	}

	public Action CoseHander;

	public UILabel lblClose;

	public UILabel lblZhanDuiScoreWord;

	public UILabel lblZhanDuiScore;

	public UILabel lblZhanDuiRank;

	public UILabel lblZhanDuiRankWord;

	public UILabel lblSelfScoreWord;

	public UILabel lblSelfScore;

	public UILabel lblJiangLiWord;

	public GameObject goBoss;

	public UILabel lblBossWord;

	public UIGrid gridReward;

	public UIGrid gridBoss;

	public GameObject goClose;
}
