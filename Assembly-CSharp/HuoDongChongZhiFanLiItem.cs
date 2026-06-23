using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuoDongChongZhiFanLiItem : UserControl
{
	private int FullNum { get; set; }

	private int BoughtFullNum { get; set; }

	protected override void InitializeComponent()
	{
		this.InitPrefabLable();
	}

	private void InitPrefabLable()
	{
		this.mLblTipTime.Text = Global.GetLang("距离活动开始");
		this.mLblTip.Text = Global.GetLang("00:00\n即将开始");
		this.LeftNum = 0;
		this.mLblDescribe.Text = Global.GetLang("充值任意金额均可获得双倍钻石");
	}

	private int LeftNum
	{
		set
		{
			this.mLblLeftNum.Text = string.Format(Global.GetLang("本轮抢购剩余{0}份"), value);
		}
	}

	public string ResetLblTipTime
	{
		set
		{
			this.mLblTipTime.Text = Global.GetLang(value);
		}
	}

	public void InitData(ChongZhiFanLiData data)
	{
		base.CancelInvoke("BeginCountDown");
		base.CancelInvoke("EndCountDown");
		if (data == null)
		{
			return;
		}
		this.mLblDescribe.Text = data.Description;
		this.ItemConfigData = data;
		this.tBegin = ChongChiFanLiUtils.GetBeginTime(this.ItemConfigData.Data, this.ItemConfigData.BeginTime);
		this.tEnd = ChongChiFanLiUtils.GetEndTime(this.ItemConfigData.Data, this.ItemConfigData.EndTime);
		this.CurState = ChongChiFanLiUtils.GetCurState(this.tBegin, this.tEnd, 0, null, false, false);
		if (this.CurState != ChongZhiFanLiState.Selling)
		{
			this.ProgressBarPercent = (float)this.ItemConfigData.FullPurchase / (float)this.ItemConfigData.FullPurchase;
		}
		else
		{
			int num = this.FullNum - this.BoughtFullNum;
			this.LeftNum = num;
			this.ProgressBarPercent = (float)num / (((float)this.FullNum != 0f) ? ((float)this.FullNum) : 1f);
		}
	}

	public ChongZhiFanLiState CurState
	{
		get
		{
			return this.mCurState;
		}
		set
		{
			this.mCurState = value;
			switch (this.mCurState)
			{
			case ChongZhiFanLiState.WillBegin:
				this.mLblTip.Text = string.Format(Global.GetLang("{0}\n即将开始"), this.ItemConfigData.BeginTime.Hour.ToString("00") + ":" + this.ItemConfigData.BeginTime.Second.ToString("00"));
				this.LeftNum = this.ItemConfigData.FullPurchase;
				this.BgTitleName = "btn_lv1";
				this.StopTimer();
				base.CancelInvoke("BeginCountDown");
				this.CountDown();
				break;
			case ChongZhiFanLiState.Selling:
				this.mLblTip.Text = string.Format(Global.GetLang("{0}\n正在进行"), this.ItemConfigData.BeginTime.Hour.ToString("00") + ":" + this.ItemConfigData.BeginTime.Second.ToString("00"));
				this.BgTitleName = "btn_hong1";
				this.LeftNum = this.FullNum - this.BoughtFullNum;
				this.StopTimer();
				this.StartUITimer();
				base.CancelInvoke("EndCountDown");
				this.CountDown();
				break;
			case ChongZhiFanLiState.End:
			{
				this.mLblTip.Text = Global.GetLang("已结束");
				base.CancelInvoke("BeginCountDown");
				base.CancelInvoke("EndCountDown");
				this.BgTitleName = "btn_lv1";
				int num = this.FullNum - this.BoughtFullNum;
				this.LeftNum = num;
				this.StopTimer();
				if (this.EndOrSellOutHandler != null)
				{
					if (num == 0)
					{
						this.ResetLblTipTime = Global.GetLang("今日活动已结束");
						this.EndOrSellOutHandler(null, new DPSelectedItemEventArgs
						{
							Flag = 1
						});
					}
					else
					{
						this.EndOrSellOutHandler(null, new DPSelectedItemEventArgs
						{
							Flag = 2
						});
					}
				}
				break;
			}
			}
		}
	}

	private string BgTitleName
	{
		set
		{
			this.mBgTitle.spriteName = value;
		}
	}

	public void RespondLeftNum(int fullNum, int boughtFullNum)
	{
		this.FullNum = fullNum;
		this.BoughtFullNum = boughtFullNum;
		int num = this.FullNum - this.BoughtFullNum;
		this.LeftNum = num;
		this.ProgressBarPercent = (float)num / (((float)this.FullNum != 0f) ? ((float)this.FullNum) : 1f);
		if (num == 0)
		{
			this.CurState = ChongZhiFanLiState.End;
		}
	}

	public float ProgressBarPercent
	{
		set
		{
			this.mProgressBar.Percent = (double)value;
			this.mProgressBar.foreground.localScale = new Vector3(value * 344f, this.mProgressBar.foreground.localScale.y, this.mProgressBar.foreground.localScale.z);
			this.mProgressBar.ProgessText = Global.GetLang("剩余") + value.ToString("p0");
		}
	}

	private void CountDown()
	{
		switch (this.CurState)
		{
		case ChongZhiFanLiState.WillBegin:
			base.InvokeRepeating("BeginCountDown", 0f, 1f);
			break;
		case ChongZhiFanLiState.Selling:
			base.InvokeRepeating("EndCountDown", 0f, 1f);
			break;
		case ChongZhiFanLiState.End:
			this.mLblTip.Text = Global.GetLang("已结束");
			base.CancelInvoke("BeginCountDown");
			base.CancelInvoke("EndCountDown");
			if (this.EndOrSellOutHandler != null)
			{
				this.EndOrSellOutHandler(null, null);
			}
			break;
		}
	}

	private void BeginCountDown()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		long num = this.tBegin.Ticks - correctDateTime.Ticks;
		int num2 = (int)(num / 10000000L);
		num2--;
		if (num2 < 0)
		{
			this.CurState = ChongZhiFanLiState.Selling;
			this.RequestData();
			base.CancelInvoke("BeginCountDown");
		}
		else
		{
			this.mLblTipTime.Text = Global.GetLang("距离活动开始 ") + ChongChiFanLiUtils.GetStrTime((double)num2);
		}
	}

	private void EndCountDown()
	{
		long num = this.tEnd.Ticks - Global.GetCorrectDateTime().Ticks;
		int num2 = (int)(num / 10000000L);
		num2--;
		if (num2 < 0)
		{
			this.CurState = ChongZhiFanLiState.End;
			base.CancelInvoke("EndCountDown");
		}
		else
		{
			this.mLblTipTime.Text = Global.GetLang("距离活动结束 ") + ChongChiFanLiUtils.GetStrTime((double)num2);
		}
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("HuoDongChongZhiFanLiItemTimer");
			this.UITimer.Interval = TimeSpan.FromSeconds(10.0);
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		this.RequestData();
	}

	private void RequestData()
	{
		bool flag = HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi);
		if (flag)
		{
			GameInstance.Game.SendDataHuoDongFanLi();
		}
	}

	public override void Destroy()
	{
		base.CancelInvoke("BeginCountDown");
		base.CancelInvoke("EndCountDown");
		this.StopTimer();
		base.Destroy();
	}

	private const string DisableSpriteName = "btn_lv1";

	private const string NormalSpriteName = "btn_hong1";

	public DPSelectedItemEventHandler EndOrSellOutHandler;

	public ChongZhiFanLiData ItemConfigData;

	public TextBlock mLblTipTime;

	public TextBlock mLblTip;

	public TextBlock mLblActivityTime;

	public TextBlock mLblLeftNum;

	public TextBlock mLblDescribe;

	public GImgProgressBar mProgressBar;

	public UISprite mBgTitle;

	private ChongZhiFanLiState mCurState;

	private DateTime tBegin;

	private DateTime tEnd;

	private DispatcherTimer UITimer;
}
