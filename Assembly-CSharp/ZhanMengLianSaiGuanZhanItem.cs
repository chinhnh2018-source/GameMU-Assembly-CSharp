using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiGuanZhanItem : UserControl
{
	private int ZhanMengID { get; set; }

	public int Bhid1 { get; set; }

	public int Bhid2 { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.mBtnClick.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.hideGM <= 0)
			{
				if (this.OpenJingCaiPart != null)
				{
					this.OpenJingCaiPart.Invoke(this.mBangHuiMatchPKInfoData);
				}
			}
			else
			{
				GameInstance.Game.SendZhanMengLianSaiCompetitionEnter(this.ZhanMengID);
			}
		};
	}

	private void InitValue()
	{
		NGUITools.SetActive(this.mVictoryImg.gameObject, false);
	}

	public void InitData(BangHuiMatchPKInfo data, int type, BangHuiMatchGameStates gameState, bool isLastRound = false)
	{
		this.ZhanMengID = data.bhid1;
		this.Bhid1 = data.bhid1;
		this.Bhid2 = data.bhid2;
		this.mLblName1.Text = data.bhname1;
		this.mLblName2.Text = data.bhname2;
		this.mGameStates = gameState;
		if (type == 1)
		{
			NGUITools.SetActive(this.mBtnClick.gameObject, false);
			this.mVsImg.spriteName = "vs_dark";
			switch (data.result)
			{
			case 0:
				this.mLblName1.Label.color = Color.gray;
				this.mLblName2.Label.color = Color.gray;
				break;
			case 1:
				this.mLblName2.Label.color = Color.gray;
				break;
			case 2:
				this.mLblName1.Label.color = Color.gray;
				break;
			}
			if (data.guess == 1)
			{
				NGUITools.SetActive(this.mVictoryImg.gameObject, true);
				this.mVictoryImg.transform.localPosition = new Vector3(this.mVictoryImg.transform.localPosition.x, 20f, this.mVictoryImg.transform.localPosition.z);
				this.mVictoryImg.spriteName = ((data.result != 1) ? "ya2" : "ya1");
			}
			else if (data.guess == 2)
			{
				NGUITools.SetActive(this.mVictoryImg.gameObject, true);
				this.mVictoryImg.transform.localPosition = new Vector3(this.mVictoryImg.transform.localPosition.x, -20f, this.mVictoryImg.transform.localPosition.z);
				this.mVictoryImg.spriteName = ((data.result != 2) ? "ya2" : "ya1");
			}
		}
		else if (type == 2)
		{
			this.mVsImg.spriteName = "vs";
			if (Global.Data != null)
			{
				this.hideGM = Global.Data.roleData.HideGM;
			}
			this.mBtnClick.Text = ((this.hideGM <= 0) ? Global.GetLang("竞\n猜") : Global.GetLang("观\n战"));
			if (this.mGameStates == 3 || this.mGameStates == null)
			{
				NGUITools.SetActive(this.mBtnClick.gameObject, false);
			}
			if (this.hideGM > 0)
			{
				NGUITools.SetActive(this.mBtnClick.gameObject, true);
			}
			this.mBangHuiMatchPKInfoData = data;
			if (data.guess > 0)
			{
				this.RefreshYaZhuIcon((int)data.guess);
			}
			if (isLastRound)
			{
				NGUITools.SetActive(this.mBtnClick.gameObject, false);
			}
		}
	}

	public void HideJingCaiBtn()
	{
		NGUITools.SetActive(this.mBtnClick.gameObject, false);
	}

	public void RefreshYaZhuIcon(int result)
	{
		NGUITools.SetActive(this.mVictoryImg.gameObject, true);
		if (result == 1)
		{
			this.mVictoryImg.transform.localPosition = new Vector3(this.mVictoryImg.transform.localPosition.x, 20f, this.mVictoryImg.transform.localPosition.z);
			this.mVictoryImg.spriteName = ((result != 1) ? "ya2" : "ya1");
		}
		else if (result == 2)
		{
			this.mVictoryImg.transform.localPosition = new Vector3(this.mVictoryImg.transform.localPosition.x, -20f, this.mVictoryImg.transform.localPosition.z);
			this.mVictoryImg.spriteName = ((result != 2) ? "ya2" : "ya1");
		}
		NGUITools.SetActive(this.mBtnClick.gameObject, false);
	}

	protected override void OnDestroy()
	{
		this.mLblName1 = null;
		this.mLblName2 = null;
		this.mVictoryImg = null;
		this.mBtnClick = null;
	}

	public Action<BangHuiMatchPKInfo> OpenJingCaiPart;

	public TextBlock mLblName1;

	public TextBlock mLblName2;

	public UISprite mVictoryImg;

	public UISprite mVsImg;

	public GButton mBtnClick;

	private int hideGM;

	private BangHuiMatchPKInfo mBangHuiMatchPKInfoData;

	private BangHuiMatchGameStates mGameStates = 5;
}
