using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class OlympicsMatchBallPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.winCount.Text = string.Format("{0}{1}", OlympicsDataManage.GetMatchData()[2].WinJiFen, Global.GetLang("积分"));
		this.loseCount.Text = string.Format("{0}{1}", OlympicsDataManage.GetMatchData()[2].LoseJiFen, Global.GetLang("积分"));
		this.winLabel.text = Global.GetLang("胜利获得：");
		this.loseLabel.text = Global.GetLang("失败获得：");
		this.currentScoreLabel.text = Global.GetLang("进球个数：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.totalScore = OlympicsDataManage.GetMatchData()[2].WinNum;
		this.totalBulletCount = (float)OlympicsDataManage.GetMatchData()[2].GameNum;
		this.InitTextureBg(OlympicsMatchBallPart.Direction.None);
		this.canPlayBall = true;
		this.ButtonClickEvent();
		this.InitIdleAnim();
		this.isWinTitleLabel.gameObject.SetActive(false);
		this.intervalTime = this.frameCount;
		this.PlayGoalkeeperIdleAnim();
	}

	private void ButtonClickEvent()
	{
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
		this.btnLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.canPlayBall)
			{
				if (this.gameTimesIsFinish)
				{
					return;
				}
				this.currentTimes--;
				this.RefreshBallCount(this.currentTimes);
				this.canPlayBall = false;
				this.IsDisableButton(false);
				GameInstance.Game.SendOlympicsSingleMatchRequest(2, 1);
				this.direction = OlympicsMatchBallPart.Direction.Left;
			}
		};
		this.btnMiddle.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.canPlayBall)
			{
				if (this.gameTimesIsFinish)
				{
					return;
				}
				this.currentTimes--;
				this.RefreshBallCount(this.currentTimes);
				this.canPlayBall = false;
				this.IsDisableButton(false);
				GameInstance.Game.SendOlympicsSingleMatchRequest(2, 2);
				this.direction = OlympicsMatchBallPart.Direction.Middle;
			}
		};
		this.btnRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.canPlayBall)
			{
				if (this.gameTimesIsFinish)
				{
					return;
				}
				this.currentTimes--;
				this.RefreshBallCount(this.currentTimes);
				this.canPlayBall = false;
				this.IsDisableButton(false);
				GameInstance.Game.SendOlympicsSingleMatchRequest(2, 3);
				this.direction = OlympicsMatchBallPart.Direction.Right;
			}
		};
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

	public void InitScoreData(int matchTimes, int timesInGame, int score)
	{
		for (int i = 0; i < this.bulletCount.Length; i++)
		{
			this.bulletCount[i].gameObject.SetActive(false);
		}
		this.SetScoreLabel(score);
		this.currentSumScore = score;
		int num = (int)(this.totalBulletCount - (float)timesInGame);
		this.currentTimes = num;
		if (num <= 0)
		{
			num = 0;
			this.gameTimesIsFinish = true;
		}
		for (int j = 0; j < num; j++)
		{
			this.bulletCount[j].gameObject.SetActive(true);
		}
	}

	private void RefreshBallCount(int times)
	{
		for (int i = 0; i < this.bulletCount.Length; i++)
		{
			this.bulletCount[i].gameObject.SetActive(false);
		}
		if (times <= 0)
		{
			times = 0;
			this.gameTimesIsFinish = true;
		}
		for (int j = 0; j < times; j++)
		{
			this.bulletCount[j].gameObject.SetActive(true);
		}
	}

	private void SetScoreLabel(int score)
	{
		this.currentScore.text = string.Format("{0}/{1}", score, this.totalScore);
		this.SetProgressBar((float)score);
	}

	public void NotifyScoreLabel(int score)
	{
		this.currentSumScore += score;
		this.InitDirection();
		base.StartCoroutine(this.PlayGoalkeeperAnim(score));
	}

	private IEnumerator PlayGoalkeeperAnim(int score)
	{
		this.StopGoalkeeperIdleAnim();
		yield return new WaitForSeconds(this.goalkeeperGetBallTime);
		if (score == 0)
		{
			this.InitTextureBg(this.direction);
		}
		else if (this.direction == OlympicsMatchBallPart.Direction.Left)
		{
			if (this.GetRandom() == 0)
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Middle);
			}
			else
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Right);
			}
		}
		else if (this.direction == OlympicsMatchBallPart.Direction.Middle)
		{
			if (this.GetRandom() == 0)
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Left);
			}
			else
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Right);
			}
		}
		else if (this.direction == OlympicsMatchBallPart.Direction.Right)
		{
			if (this.GetRandom() == 0)
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Left);
			}
			else
			{
				this.InitTextureBg(OlympicsMatchBallPart.Direction.Middle);
			}
		}
		this.SetScoreLabel(this.currentSumScore);
		yield break;
	}

	public int GetRandom()
	{
		Random random = new Random();
		return random.Next(0, 2);
	}

	public void NotifyFinishGame(int isWin, int grade, int score)
	{
		if (isWin == 1)
		{
			this.isWinTitleLabel.Text = "Win";
		}
		this.SetScoreLabel(grade);
		this.isWin = isWin;
		this.grade = grade;
		this.score = score;
	}

	private void InitDirection()
	{
		if (this.direction == OlympicsMatchBallPart.Direction.Right)
		{
			this.to = this.right;
		}
		else if (this.direction == OlympicsMatchBallPart.Direction.Left)
		{
			this.to = this.left;
		}
		else if (this.direction == OlympicsMatchBallPart.Direction.Middle)
		{
			this.to = this.middle;
		}
		this.isYes = true;
	}

	private new void Update()
	{
		if (this.isPlayIdleAnim)
		{
			this.intervalTime -= Time.deltaTime;
			if (this.intervalTime <= 0f)
			{
				this.intervalTime = this.frameCount;
				this.index++;
				if (this.index > this.animClipLength)
				{
					this.index = 1;
				}
				this.ReloadTexture(this.index);
			}
		}
		if (this.isYes)
		{
			this.mFloat += Time.deltaTime * this.speed;
			Vector3 vector = (this.from.position + this.to.position) * 0.5f;
			if (this.direction == OlympicsMatchBallPart.Direction.Right)
			{
				vector -= new Vector3(this.offsetY, this.offsetX, 0f);
			}
			else if (this.direction == OlympicsMatchBallPart.Direction.Left)
			{
				vector += new Vector3(this.offsetY, this.offsetX, 0f);
			}
			this.ballTranform.position = Vector3.Lerp(this.from.position, this.to.position, this.mFloat);
			this.ballTranform.localScale = Vector3.Lerp(new Vector3((float)this.ballOrigionScale, (float)this.ballOrigionScale, 1f), new Vector3((float)this.ballChangeScale, (float)this.ballChangeScale, 1f), this.mFloat);
			if (this.ballTranform.position == this.to.position)
			{
				this.isYes = false;
				if (this.currentTimes <= 0)
				{
					if (this.isFinish)
					{
						return;
					}
					this.isFinish = true;
					this.btnLeft.isEnabled = false;
					this.btnMiddle.isEnabled = false;
					this.btnRight.isEnabled = false;
					this.canPlayBall = false;
					base.StartCoroutine(this.ShowResultWindow());
				}
				base.StartCoroutine(this.Destroy());
			}
		}
	}

	private IEnumerator ShowResultWindow()
	{
		yield return new WaitForSeconds(1f);
		if (null == this.olympicsMatchResultWindow)
		{
			this.olympicsMatchResultWindow = U3DUtils.NEW<GChildWindow>();
			this.olympicsMatchResultWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.olympicsMatchResultWindow, "OlympicsMatchResultWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchResultWindow);
		}
		this.olympicsMatchResult = U3DUtils.NEW<OlympicsMatchResult>();
		this.olympicsMatchResultWindow.Body.Add(this.olympicsMatchResult);
		this.olympicsMatchResult.SetContent(this.isWin, this.grade, 2, this.score);
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
		yield break;
	}

	private void InitTextureBg(OlympicsMatchBallPart.Direction dir)
	{
		switch (dir + 1)
		{
		case OlympicsMatchBallPart.Direction.Left:
			this.goalkeeper.URL = "NetImages/GameRes/Images/Olympics/Goalkeeper_idle.png";
			this.goalkeeper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			this.goalkeeper.transform.parent.localPosition = new Vector3(0f, (float)this.goalkeeperY, 2f);
			break;
		case OlympicsMatchBallPart.Direction.Middle:
			this.goalkeeper.URL = "NetImages/GameRes/Images/Olympics/Goalkeeper_left.png";
			this.goalkeeper.transform.localEulerAngles = new Vector3(0f, 0f, (float)this.leftGoalkeeperRotationZ);
			this.goalkeeper.transform.parent.localPosition = new Vector3((float)this.leftGoalkeeperX, (float)this.goalkeeperY, this.goalkeeperZ);
			break;
		case OlympicsMatchBallPart.Direction.Right:
			this.goalkeeper.URL = "NetImages/GameRes/Images/Olympics/Goalkeeper_middle.png";
			this.goalkeeper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			this.goalkeeper.transform.parent.localPosition = new Vector3(0f, (float)this.goalkeeperY, this.goalkeeperZ);
			break;
		case (OlympicsMatchBallPart.Direction)3:
			this.goalkeeper.URL = "NetImages/GameRes/Images/Olympics/Goalkeeper_right.png";
			this.goalkeeper.transform.localEulerAngles = new Vector3(0f, 0f, (float)this.rightGoalkeeperRotationZ);
			this.goalkeeper.transform.parent.localPosition = new Vector3((float)this.rightGoalkeeperX, (float)this.goalkeeperY, this.goalkeeperZ);
			break;
		}
		NGUITools.MakePixelPerfect(this.goalkeeper.transform);
	}

	private void InitIdleAnim()
	{
		this.idleAnim = new Texture2D[this.animClipLength];
		for (int i = 0; i < this.animClipLength; i++)
		{
			base.StartCoroutine(this.InitImage(i));
		}
	}

	private IEnumerator InitImage(int index)
	{
		this.ImageURL = string.Format("{0}{1}{2}", "NetImages/GameRes/Images/Olympics/OlympicsAnimTexture/", index + 1, ".png");
		WWW www = new WWW(PathUtils.WebPath(this.ImageURL + ".qj"));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			www = new WWW(PathUtils.WebPath(this.ImageURL));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
		}
		if (null == www.assetBundle)
		{
			this.idleAnim[index] = www.textureNonReadable;
		}
		else
		{
			this.idleAnim[index] = (www.assetBundle.mainAsset as Texture2D);
		}
		www.Dispose();
		www = null;
		yield break;
	}

	private void ReloadTexture(int index)
	{
		this.idle.mainTexture = this.idleAnim[index - 1];
	}

	private void StopGoalkeeperIdleAnim()
	{
		this.isPlayIdleAnim = false;
		this.index = 0;
		this.intervalTime = this.frameCount;
		this.idle.transform.parent.gameObject.SetActive(false);
		this.goalkeeper.transform.parent.localPosition = new Vector3(0f, (float)this.goalkeeperY, this.goalkeeperZ);
		Resources.UnloadUnusedAssets();
	}

	private void PlayGoalkeeperIdleAnim()
	{
		this.isPlayIdleAnim = true;
		this.index = 0;
		this.intervalTime = this.frameCount;
		this.idle.transform.parent.gameObject.SetActive(true);
		this.goalkeeper.transform.parent.localPosition = new Vector3(0f, (float)this.goalkeeperY, 2f);
	}

	private new IEnumerator Destroy()
	{
		yield return new WaitForSeconds(0.5f);
		this.mFloat = 0f;
		this.ballTranform.position = this.from.position;
		this.ballTranform.localScale = new Vector3((float)this.ballOrigionScale, (float)this.ballOrigionScale, 1f);
		this.direction = OlympicsMatchBallPart.Direction.None;
		this.canPlayBall = true;
		this.IsDisableButton(true);
		this.InitTextureBg(OlympicsMatchBallPart.Direction.None);
		this.PlayGoalkeeperIdleAnim();
		yield break;
	}

	private void IsDisableButton(bool isDis)
	{
		this.btnLeft.isEnabled = isDis;
		this.btnMiddle.isEnabled = isDis;
		this.btnRight.isEnabled = isDis;
	}

	private new void OnDestroy()
	{
		this.isFinish = false;
		base.StopCoroutine(this.Destroy());
		base.StopCoroutine(this.PlayGoalkeeperAnim(0));
	}

	public GButton btnClose;

	public TextBlock winLabel;

	public TextBlock winCount;

	public TextBlock loseLabel;

	public TextBlock loseCount;

	public TextBlock isWinTitleLabel;

	public TextBlock currentScoreLabel;

	public TextBlock currentScore;

	public UISprite[] bulletCount;

	public GameObject tipsObj;

	public TextBlock tipsContent;

	public GButton btnLeft;

	public GButton btnMiddle;

	public GButton btnRight;

	public UISprite ball;

	public DPSelectedItemEventHandler Hander;

	public GImgProgressBar progressBar;

	private int totalScore;

	private float totalBulletCount;

	private OlympicsMatchResult olympicsMatchResult;

	private GChildWindow olympicsMatchResultWindow;

	public Transform from;

	private Transform to;

	public Transform right;

	public Transform middle;

	public Transform left;

	public float offsetX;

	public float offsetY;

	private float mFloat;

	private bool isYes;

	public float speed;

	public OlympicsMatchBallPart.Direction direction = OlympicsMatchBallPart.Direction.None;

	public Transform ballTranform;

	public int leftGoalkeeperX = -133;

	public int rightGoalkeeperX = 118;

	public int goalkeeperY = 80;

	public float goalkeeperZ = -0.3f;

	public int leftGoalkeeperRotationZ = 30;

	public int rightGoalkeeperRotationZ = -30;

	public int ballOrigionScale = 68;

	public int ballChangeScale = 34;

	public float goalkeeperGetBallTime = 1f;

	public ShowNetImage goalkeeper;

	private bool canPlayBall;

	private int currentTimes;

	private bool gameTimesIsFinish;

	public float frameCount = 0.1f;

	public UITexture idle;

	private float intervalTime = 0.1f;

	private int index = 1;

	public bool isPlayIdleAnim;

	public int animClipLength = 2;

	private Texture2D[] idleAnim;

	private string ImageURL;

	public int currentSumScore;

	private bool isFinish;

	private int isWin;

	private int grade;

	private int score;

	public enum Direction
	{
		None = -1,
		Left,
		Middle,
		Right
	}
}
