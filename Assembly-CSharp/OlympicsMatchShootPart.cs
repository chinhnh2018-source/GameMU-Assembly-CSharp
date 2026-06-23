using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class OlympicsMatchShootPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.winLabel.text = Global.GetLang("胜利获得：");
		this.loseLabel.text = Global.GetLang("失败获得：");
		this.currentScoreLabel.text = Global.GetLang("当前环数：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.winCount.Text = string.Format("{0}{1}", OlympicsDataManage.GetMatchData()[1].WinJiFen, Global.GetLang("积分"));
		this.loseCount.Text = string.Format("{0}{1}", OlympicsDataManage.GetMatchData()[1].LoseJiFen, Global.GetLang("积分"));
		this.totalScore = OlympicsDataManage.GetMatchData()[1].WinNum;
		this.totalBulletCount = OlympicsDataManage.GetMatchData()[1].GameNum;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.shootBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isGameOver)
			{
				return;
			}
			if (this.isWaiteOneScenonds && this.isShoot)
			{
				Super.ShowNetWaiting(null);
				this.isClickShoot = true;
				this.CountDownBulletNum();
				base.StartCoroutine(this.StartShoot());
			}
		};
		this.gun.MakePixelPerfect();
		this.isWinTitleLabel.gameObject.SetActive(false);
		base.StartCoroutine(this.WaitForTimePlayGame());
	}

	private IEnumerator WaitForTimePlayGame()
	{
		yield return new WaitForSeconds((float)this.waitForTimePlayGame);
		this.isWaiteOneScenonds = true;
		this.IsShowBullet(false);
		this.BeginGame();
		yield break;
	}

	private void SetProgressBar(float currentCount)
	{
		if (currentCount >= (float)this.totalScore)
		{
			currentCount = (float)this.totalScore;
			if (!this.isWinTitleLabel.gameObject.activeSelf)
			{
				this.isWinTitleLabel.gameObject.SetActive(true);
				this.isWinTitleLabel.Text = "Win";
			}
		}
		this.progressBar.Percent = (double)(currentCount / (float)this.totalScore);
	}

	public void NotifyScoreLabel(int score)
	{
		this.BeginGame();
		this.isClickShoot = false;
		this.tipsObj.SetActive(false);
		this.isShoot = true;
		this.zhunXin.URL = "NetImages/GameRes/Images/Olympics/zhunXin01.png";
		this.SetScoreLabel(score);
	}

	public void NotifyFinishGame(int isWin, int grade, int score)
	{
		if (isWin == 1)
		{
			this.isWinTitleLabel.Text = "Win";
		}
		this.SetScoreLabel(grade);
		this.isShoot = false;
		this.isGameOver = true;
		Super.HintMainText(Global.GetLang("比赛结束"), 10, 3);
		if (null == this.olympicsMatchResultWindow)
		{
			this.olympicsMatchResultWindow = U3DUtils.NEW<GChildWindow>();
			this.olympicsMatchResultWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.olympicsMatchResultWindow, "OlympicsMatchResultWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchResultWindow);
		}
		this.olympicsMatchResult = U3DUtils.NEW<OlympicsMatchResult>();
		this.olympicsMatchResultWindow.Body.Add(this.olympicsMatchResult);
		this.olympicsMatchResult.SetContent(isWin, grade, 1, score);
		this.olympicsMatchResult.Hander = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args.ID == 0)
			{
				Object.Destroy(this.olympicsMatchResult.gameObject);
				this.olympicsMatchResult = null;
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 0
					});
				}
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchResultWindow);
			}
		};
	}

	private void SetScoreLabel(int score)
	{
		this.currentScore.text = string.Format("{0}/{1}", score, this.totalScore);
		this.SetProgressBar((float)score);
	}

	public void InitScoreData(int matchTimes, int timesInGame, int score)
	{
		for (int i = 0; i < this.bulletCountSprite.Length; i++)
		{
			this.bulletCountSprite[i].gameObject.SetActive(false);
		}
		this.SetScoreLabel(score);
		int num = this.totalBulletCount - timesInGame;
		this.bulletCount = num;
		if (num <= 0)
		{
			num = 0;
			this.isShoot = false;
			for (int j = 0; j < num; j++)
			{
				this.bulletCountSprite[j].gameObject.SetActive(true);
			}
			this.ShootOutOfLimitTimes();
			return;
		}
		this.isShoot = true;
		this.zhunXin.URL = "NetImages/GameRes/Images/Olympics/zhunXin01.png";
		for (int k = 0; k < num; k++)
		{
			this.bulletCountSprite[k].gameObject.SetActive(true);
		}
	}

	private void RefreshBulletCount(int times)
	{
		for (int i = 0; i < this.bulletCountSprite.Length; i++)
		{
			this.bulletCountSprite[i].gameObject.SetActive(false);
		}
		if (times <= 0)
		{
			times = 0;
			for (int j = 0; j < times; j++)
			{
				this.bulletCountSprite[j].gameObject.SetActive(true);
			}
		}
		else
		{
			this.isShoot = true;
			this.zhunXin.URL = "NetImages/GameRes/Images/Olympics/zhunXin01.png";
			for (int k = 0; k < times; k++)
			{
				this.bulletCountSprite[k].gameObject.SetActive(true);
			}
		}
	}

	private void IsShowBullet(bool isShow)
	{
		this.bulletHitPoint.gameObject.SetActive(isShow);
	}

	private void BeginGame()
	{
		this.InitData();
		base.StartCoroutine(this.StartMoveTarget());
	}

	private void InitData()
	{
		Random random = new Random();
		this.targetMoveSpeed = (float)random.Next(this.minRandom, this.maxRandom);
		this.tipsObj.SetActive(false);
		this.shootBtn.isEnabled = true;
		this.isMoveTarget = true;
		this.startPoint = this.startPosition.x;
		this.endPoint = this.endPosition.x;
		this.bullet.localPosition = this.bulletStartPosition;
		this.target.localRotation = Quaternion.identity;
		if (this.bulletCount <= 0)
		{
			this.ShootOutOfLimitTimes();
			return;
		}
	}

	private IEnumerator StartMoveTarget()
	{
		while (this.isMoveTarget)
		{
			Vector3 tmp = this.target.localPosition;
			tmp.x = Mathf.MoveTowards(this.target.localPosition.x, this.endPosition.x, Time.deltaTime * this.targetMoveSpeed);
			this.target.localPosition = tmp;
			this.lastPosition = this.target.localPosition;
			if (this.target.localPosition.x >= this.endPoint)
			{
				this.isShoot = false;
				base.StartCoroutine(this.StopMoveTarget());
			}
			yield return null;
		}
		yield break;
	}

	private IEnumerator StopMoveTarget()
	{
		this.StopMoveTargetImmediate();
		yield return new WaitForSeconds(2f);
		if (!this.isClickShoot)
		{
			GameInstance.Game.SendOlympicsSingleMatchRequest(1, 0);
			this.CountDownBulletNum();
		}
		else
		{
			this.InitData();
			base.StartCoroutine(this.StartMoveTarget());
			this.isShoot = true;
		}
		yield break;
	}

	private void StopMoveTargetImmediate()
	{
		this.isMoveTarget = false;
		this.target.localPosition = this.startPosition;
		this.lastPosition = Vector3.zero;
		base.StopCoroutine(this.StartMoveTarget());
	}

	private void CountDownBulletNum()
	{
		this.bulletCount--;
		this.RefreshBulletCount(this.bulletCount);
	}

	private IEnumerator StartShoot()
	{
		while (this.isShoot)
		{
			Vector3 tmp = this.bullet.localPosition;
			tmp.y = Mathf.MoveTowards(this.bullet.localPosition.y, 0f, Time.deltaTime * this.bulletMoveSpeed);
			this.bullet.localPosition = tmp;
			if (this.bullet.localPosition.y >= 0f)
			{
				this.CheckBulletShootTarget();
			}
			yield return null;
		}
		yield break;
	}

	private void CheckBulletShootTarget()
	{
		this.isShoot = false;
		this.StopShoot();
		this.score = this.GetScore(Mathf.Abs(this.target.localPosition.x));
		Super.HintMainText(Global.GetLang("枪支冷却"), 10, 3);
		this.zhunXin.URL = "NetImages/GameRes/Images/Olympics/zhunXin02.png";
		if (this.score >= 0)
		{
			if (this.score != 0)
			{
				this.IsShowBullet(true);
			}
			this.ShootTarget(this.score);
		}
	}

	private void StopShoot()
	{
		base.StopCoroutine(this.StartShoot());
	}

	private void ShootTarget(int score)
	{
		this.isMoveTarget = false;
		base.StopCoroutine(this.StartMoveTarget());
		base.StartCoroutine(this.PushTarget());
		base.StartCoroutine(this.ShowScore(score));
	}

	private IEnumerator ShowScore(int score)
	{
		yield return new WaitForSeconds(0.7f);
		this.tipsObj.SetActive(true);
		if (score >= 0)
		{
			this.tipsContent.Text = string.Format("{0}{1}", score, Global.GetLang("环"));
		}
		if (this.bulletCount <= 0)
		{
			this.ShootOutOfLimitTimes();
		}
		yield break;
	}

	private void ShootOutOfLimitTimes()
	{
		this.isShoot = false;
		this.shootBtn.isEnabled = false;
		this.StopMoveTargetImmediate();
		this.zhunXin.URL = "NetImages/GameRes/Images/Olympics/zhunXin02.png";
		Super.HintMainText(Global.GetLang("射击次数已达上限！"), 10, 3);
	}

	private IEnumerator PushTarget()
	{
		yield return new WaitForSeconds(2f);
		this.target.localPosition = this.startPosition;
		this.IsShowBullet(false);
		GameInstance.Game.SendOlympicsSingleMatchRequest(1, this.score);
		yield break;
	}

	private int GetScore(float distance)
	{
		int result;
		if (distance <= this.radius)
		{
			result = 10;
		}
		else if (distance <= this.radius * 2f)
		{
			result = 9;
		}
		else if (distance <= this.radius * 3f)
		{
			result = 8;
		}
		else if (distance <= this.radius * 4f)
		{
			result = 7;
		}
		else if (distance <= this.radius * 5f)
		{
			result = 6;
		}
		else if (distance <= this.radius * 6f)
		{
			result = 5;
		}
		else if (distance <= this.radius * 7f)
		{
			result = 4;
		}
		else if (distance <= this.radius * 8f)
		{
			result = 3;
		}
		else if (distance <= this.radius * 9f)
		{
			result = 2;
		}
		else if (distance <= this.radius * 10f)
		{
			result = 1;
		}
		else
		{
			result = 0;
		}
		return result;
	}

	private new void OnDestroy()
	{
		base.StopCoroutine(this.StartMoveTarget());
		base.StopCoroutine(this.PushTarget());
		base.StopCoroutine(this.StartShoot());
		this.isGameOver = false;
	}

	public GButton btnClose;

	public TextBlock winLabel;

	public TextBlock winCount;

	public TextBlock loseLabel;

	public TextBlock loseCount;

	public TextBlock isWinTitleLabel;

	public TextBlock currentScoreLabel;

	public TextBlock currentScore;

	public UISprite[] bulletCountSprite;

	public GameObject tipsObj;

	public TextBlock tipsContent;

	public GButton btnShoot;

	public ShowNetImage zhunXin;

	public DPSelectedItemEventHandler Hander;

	public GImgProgressBar progressBar;

	private int totalScore;

	private int totalBulletCount;

	private OlympicsMatchResult olympicsMatchResult;

	private GChildWindow olympicsMatchResultWindow;

	public UITexture gun;

	private bool isClickShoot;

	public Vector3 startPosition = Vector3.zero;

	public Vector3 endPosition = Vector3.zero;

	private Vector3 lastPosition = Vector3.zero;

	private float startPoint;

	private float endPoint;

	public Transform target;

	public Transform centerPoint;

	public Vector3 bulletStartPosition = Vector3.zero;

	public Transform bullet;

	public GButton shootBtn;

	private bool isShoot;

	public float targetMoveSpeed = 1f;

	public float bulletMoveSpeed = 1f;

	private bool isMoveTarget;

	public int bulletCount = 5;

	public float radius = 1f;

	public float dropTime = 3f;

	public int minRandom = 100;

	public int maxRandom = 150;

	public Transform bulletHitPoint;

	private bool isGameOver;

	public int waitForTimePlayGame = 1;

	private bool isWaiteOneScenonds;

	private int score;
}
