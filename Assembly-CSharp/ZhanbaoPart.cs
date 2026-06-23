using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class ZhanbaoPart : JingjiPlayerInfoBase
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.request();
		this.m_JunxianLabel.X = -360.0;
		this.m_RankingLabel.X = -370.0;
		this.m_LianshengLabel.X = -290.0;
		this.m_ShengwangLabel.X = -360.0;
		this.m_DangqianxiaoguoLabel.X = -313.0;
	}

	public void init(int ranking, int winCount, List<XElement> list)
	{
		base.initPlayerInfo(ranking, winCount, list);
	}

	public void refresh(List<JingJiChallengeInfoData> list)
	{
		if (list == null)
		{
			this.canRefresh = false;
			return;
		}
		string text = "00FF00";
		string text2 = "FF0000";
		string text3 = "F9F702";
		for (int i = 0; i < list.Count; i++)
		{
			JingJiChallengeInfoData jingJiChallengeInfoData = list[i];
			string text4 = string.Empty;
			switch (jingJiChallengeInfoData.zhanbaoType)
			{
			case 0:
				text4 = string.Format(Global.GetLang("你挑战{0},{1},排名上升至{2}"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					Global.GetLang("你胜利了")
				}), (jingJiChallengeInfoData.value >= 0) ? jingJiChallengeInfoData.value.ToString() : Global.GetLang("500名后"));
				break;
			case 1:
				text4 = string.Format(Global.GetLang("你挑战{0},{1},排名不变。"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Global.GetLang("你失败了")
				}));
				break;
			case 2:
				text4 = string.Format(Global.GetLang("{0}挑战你,{1},排名不变。"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					Global.GetLang("你胜利了")
				}));
				break;
			case 3:
				text4 = string.Format(Global.GetLang("{0}挑战你,{1},排名下降至{2}"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Global.GetLang("你失败了")
				}), (jingJiChallengeInfoData.value >= 0) ? jingJiChallengeInfoData.value.ToString() : Global.GetLang("500名后"));
				break;
			case 4:
				text4 = string.Format(Global.GetLang("{0}连胜次数达到了{1}次,勇不可挡！"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					jingJiChallengeInfoData.value
				}));
				break;
			}
			ZhanbaoItem zhanbaoItem = U3DUtils.NEW<ZhanbaoItem>();
			this.collection.Add(zhanbaoItem);
			zhanbaoItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			zhanbaoItem.m_Text.Text = text4;
		}
		this.currPage++;
		base.Invoke("Continue", 1f);
	}

	private void Continue()
	{
		this.canRefresh = true;
	}

	private IEnumerator CheckBarValue()
	{
		for (;;)
		{
			if (this.canRefresh && this.scrollBar.scrollValue > 0.98f)
			{
				this.request();
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private void request()
	{
		this.canRefresh = false;
		if (this.currPage > 2)
		{
			return;
		}
		GameInstance.Game.SpriteGetChallengeInfoListCmd(this.currPage);
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(base.TimeProc());
		base.StartCoroutine<bool>(this.CheckBarValue());
	}

	public UIScrollBar scrollBar;

	private int currPage;

	private bool canRefresh;
}
