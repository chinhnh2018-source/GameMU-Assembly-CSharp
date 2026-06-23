using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class OlympicsMatchPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		base.InitializeComponent();
		this.btnShoot.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (Global.IsInOlympicsAwardActivity())
			{
				Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
				return;
			}
			GameInstance.Game.SendBeginMatchRequest(1);
		};
		this.btnPlayBall.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (Global.IsInOlympicsAwardActivity())
			{
				Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
				return;
			}
			GameInstance.Game.SendBeginMatchRequest(2);
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnShoot.Text = Global.GetLang("参加");
		this.btnPlayBall.Text = Global.GetLang("参加");
		this.shootFreeLabel.Text = Global.GetLang("免费剩余次数：");
		this.shootPayLabel.Text = Global.GetLang("付费剩余次数：");
		this.playBallFreeLabel.Text = Global.GetLang("免费剩余次数：");
		this.playBallPayLabel.Text = Global.GetLang("付费剩余次数：");
	}

	public void RefreshData(int tmpShootTimes, int tmpPlayBallTimes)
	{
		this.shootData = OlympicsDataManage.GetMatchData()[1];
		this.shootGameDiamondTimes = this.shootData.needDiamondsList.Count;
		OlympicsDataManage.shootTimes = this.shootGameDiamondTimes + this.shootData.FreeNum - tmpShootTimes;
		if (OlympicsDataManage.shootTimes <= 0)
		{
			OlympicsDataManage.shootTimes = 0;
		}
		this.playBallData = OlympicsDataManage.GetMatchData()[2];
		this.playBallGameDiamondTimes = this.playBallData.needDiamondsList.Count;
		OlympicsDataManage.playBallTimes = this.playBallGameDiamondTimes + this.playBallData.FreeNum - tmpPlayBallTimes;
		if (OlympicsDataManage.playBallTimes <= 0)
		{
			OlympicsDataManage.playBallTimes = 0;
		}
		if (tmpShootTimes < this.shootData.FreeNum)
		{
			NGUITools.SetActive(this.shootFreeLabel.gameObject, true);
			NGUITools.SetActive(this.shootPayLabel.gameObject, false);
			NGUITools.SetActive(this.shootDiamond.gameObject, false);
			NGUITools.SetActive(this.shootCanJiaLabel.gameObject, true);
			NGUITools.SetActive(this.shootDiamongSprite.gameObject, false);
			this.howManyShoot.Text = string.Format("{0}/{1}", this.shootData.FreeNum - tmpShootTimes, this.shootData.FreeNum);
		}
		else
		{
			NGUITools.SetActive(this.shootFreeLabel.gameObject, false);
			NGUITools.SetActive(this.shootPayLabel.gameObject, true);
			NGUITools.SetActive(this.shootDiamond.gameObject, true);
			NGUITools.SetActive(this.shootCanJiaLabel.gameObject, false);
			NGUITools.SetActive(this.shootDiamongSprite.gameObject, true);
			this.shootDiamond.Text = this.shootData.needDiamondsList[this.GetCurrentPayGameTimes(OlympicsDataManage.shootTimes, this.shootGameDiamondTimes)].ToString() + Global.GetLang(" 参加");
			this.howManyShoot.Text = string.Format("{0}/{1}", this.shootData.FreeNum + this.shootGameDiamondTimes - tmpShootTimes, this.shootGameDiamondTimes);
		}
		if (tmpPlayBallTimes < this.playBallData.FreeNum)
		{
			NGUITools.SetActive(this.playBallFreeLabel.gameObject, true);
			NGUITools.SetActive(this.playBallPayLabel.gameObject, false);
			NGUITools.SetActive(this.playBallDiamond.gameObject, false);
			NGUITools.SetActive(this.playBallCanJiaLabel.gameObject, true);
			NGUITools.SetActive(this.playBallDiamongSprite.gameObject, false);
			this.howManyPlayBall.Text = string.Format("{0}/{1}", this.playBallData.FreeNum - tmpPlayBallTimes, this.playBallData.FreeNum);
		}
		else
		{
			NGUITools.SetActive(this.playBallFreeLabel.gameObject, false);
			NGUITools.SetActive(this.playBallPayLabel.gameObject, true);
			NGUITools.SetActive(this.playBallDiamond.gameObject, true);
			NGUITools.SetActive(this.playBallCanJiaLabel.gameObject, false);
			NGUITools.SetActive(this.playBallDiamongSprite.gameObject, true);
			this.playBallDiamond.Text = this.playBallData.needDiamondsList[this.GetCurrentPayGameTimes(OlympicsDataManage.playBallTimes, this.playBallGameDiamondTimes)].ToString() + Global.GetLang(" 参加");
			this.howManyPlayBall.Text = string.Format("{0}/{1}", this.playBallData.FreeNum + this.playBallGameDiamondTimes - tmpPlayBallTimes, this.playBallGameDiamondTimes);
		}
	}

	public void OpenShootGame(int matchTimes, int timesInGame, int score)
	{
		if (null == this.olympicsMatchShootPartWind)
		{
			this.olympicsMatchShootPartWind = U3DUtils.NEW<GChildWindow>();
			this.olympicsMatchShootPartWind.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.olympicsMatchShootPartWind, "OlympicsMatchShootPartWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchShootPartWind);
		}
		this.olympicsMatchShootPart = U3DUtils.NEW<OlympicsMatchShootPart>();
		this.olympicsMatchShootPartWind.Body.Add(this.olympicsMatchShootPart);
		this.olympicsMatchShootPart.InitScoreData(matchTimes, timesInGame, score);
		this.olympicsMatchShootPart.Hander = delegate(object s, DPSelectedItemEventArgs args)
		{
			if (args.ID == 0 && null != this.olympicsMatchShootPart)
			{
				Object.Destroy(this.olympicsMatchShootPart.gameObject);
				this.olympicsMatchShootPart = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchShootPartWind);
			}
		};
		this.olympicsMatchShootPartWind.SetContent(this.olympicsMatchShootPartWind.BodyPresenter, this.olympicsMatchShootPart, 0.0, 0.0, true);
	}

	public void OpenPlayBallGame(int matchTimes, int timesInGame, int score)
	{
		if (null == this.olympicsMatchBallPartWind)
		{
			this.olympicsMatchBallPartWind = U3DUtils.NEW<GChildWindow>();
			this.olympicsMatchBallPartWind.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.olympicsMatchBallPartWind, "OlympicsMatchBallPartWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchBallPartWind);
		}
		this.olympicsMatchBallPart = U3DUtils.NEW<OlympicsMatchBallPart>();
		this.olympicsMatchBallPartWind.Body.Add(this.olympicsMatchBallPart);
		this.olympicsMatchBallPart.InitScoreData(matchTimes, timesInGame, score);
		this.olympicsMatchBallPart.Hander = delegate(object s1, DPSelectedItemEventArgs args)
		{
			if (args.ID == 0 && null != this.olympicsMatchBallPart)
			{
				Object.Destroy(this.olympicsMatchBallPart.gameObject);
				this.olympicsMatchBallPart = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchBallPartWind);
			}
		};
		this.olympicsMatchBallPartWind.SetContent(this.olympicsMatchBallPartWind.BodyPresenter, this.olympicsMatchBallPart, 0.0, 0.0, true);
	}

	private int GetCurrentPayGameTimes(int times, int sumCounts)
	{
		int result;
		if (times == 0)
		{
			result = sumCounts - 1;
		}
		else
		{
			result = sumCounts - times;
		}
		return result;
	}

	public TextBlock shootFreeLabel;

	public TextBlock shootPayLabel;

	public TextBlock howManyShoot;

	public TextBlock shootDiamond;

	public TextBlock shootCanJiaLabel;

	public UISprite shootDiamongSprite;

	public TextBlock playBallFreeLabel;

	public TextBlock playBallPayLabel;

	public TextBlock howManyPlayBall;

	public TextBlock playBallDiamond;

	public TextBlock playBallCanJiaLabel;

	public UISprite playBallDiamongSprite;

	public GButton btnShoot;

	public GButton btnPlayBall;

	private OlympicsMatchData shootData;

	private OlympicsMatchData playBallData;

	private int shootGameDiamondTimes;

	private int playBallGameDiamondTimes;

	[HideInInspector]
	public OlympicsMatchShootPart olympicsMatchShootPart;

	private GChildWindow olympicsMatchShootPartWind;

	[HideInInspector]
	public OlympicsMatchBallPart olympicsMatchBallPart;

	private GChildWindow olympicsMatchBallPartWind;
}
