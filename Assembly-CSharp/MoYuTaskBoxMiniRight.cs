using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class MoYuTaskBoxMiniRight : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblScoreWord.text = string.Empty;
		this.lblTeam1.text = string.Empty;
		this.lblTeam1Score.text = string.Empty;
		this.lblTeam2.text = string.Empty;
		this.lblTeam2Score.text = string.Empty;
		this.lblTeam3.text = string.Empty;
		this.lblTeam3Score.text = string.Empty;
		this.lblScoreWord.text = Global.GetLang("战场积分");
		this.lstTeamName.Add(this.lblTeam1);
		this.lstTeamName.Add(this.lblTeam2);
		this.lstTeamName.Add(this.lblTeam3);
		this.lstTeamScore.Add(this.lblTeam1Score);
		this.lstTeamScore.Add(this.lblTeam2Score);
		this.lstTeamScore.Add(this.lblTeam3Score);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetContentShow(true);
		UIEventListener.Get(this.goSwitch).onClick = new UIEventListener.VoidDelegate(this.OnSwicth);
	}

	private void OnSwicth(GameObject go)
	{
		this.goContent.SetActive(!this.m_beShow);
		this.SetContentShow(!this.m_beShow);
	}

	private void SetContentShow(bool beShow)
	{
		this.m_beShow = beShow;
		if (beShow)
		{
			this.sprSwitch.spriteName = "Taskarrow1";
		}
		else
		{
			this.sprSwitch.spriteName = "Taskarrow_02";
		}
	}

	private void ShowAnimation(bool beShow)
	{
		this.m_beCanClick = false;
		if (beShow)
		{
			TweenPosition.Begin(this.goContent, 0.3f, new Vector3(42f, 26f, -0.02f), new Vector3(210f, 26f, -0.02f));
		}
		else
		{
			TweenPosition.Begin(this.goContent, 0.3f, new Vector3(210f, 26f, -0.02f), new Vector3(42f, 26f, -0.02f));
		}
		base.StartCoroutine<bool>(this.DealDoTime());
	}

	private IEnumerator DealDoTime()
	{
		yield return new WaitForSeconds(0.3f);
		this.m_beCanClick = true;
		yield break;
	}

	protected void UpdateInfo(List<string> teamNames, List<int> teamScores)
	{
		for (int i = 0; i < 3; i++)
		{
			if (i < teamNames.Count)
			{
				this.lstTeamObject[i].SetActive(true);
				this.lstTeamName[i].text = teamNames[i];
				this.lstTeamScore[i].text = teamScores[i].ToString();
			}
			else
			{
				this.lstTeamObject[i].SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<ZorkBattleSideScore>("CMD_SPR_ZORK_SIDE_SCORE", new Action<ZorkBattleSideScore>(this.GetInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleSideScore>("CMD_SPR_ZORK_SIDE_SCORE", new Action<ZorkBattleSideScore>(this.GetInfo));
	}

	private void GetInfo(ZorkBattleSideScore data)
	{
		if (data != null && data.ZorkBattleTeamList != null)
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < data.ZorkBattleTeamList.Count; i++)
			{
				list.Add(data.ZorkBattleTeamList[i].TeamName);
				list2.Add(data.ZorkBattleTeamList[i].JiFen);
			}
			this.UpdateInfo(list, list2);
		}
		else
		{
			List<string> teamNames = new List<string>();
			List<int> teamScores = new List<int>();
			this.UpdateInfo(teamNames, teamScores);
		}
	}

	public UILabel lblScoreWord;

	public UILabel lblTeam1;

	public UILabel lblTeam1Score;

	public UILabel lblTeam2;

	public UILabel lblTeam2Score;

	public UILabel lblTeam3;

	public UILabel lblTeam3Score;

	public GameObject goSwitch;

	public GameObject goContent;

	public UISprite sprSwitch;

	public List<GameObject> lstTeamObject = new List<GameObject>();

	private List<UILabel> lstTeamName = new List<UILabel>();

	private List<UILabel> lstTeamScore = new List<UILabel>();

	private bool m_beShow = true;

	private bool m_beCanClick = true;
}
