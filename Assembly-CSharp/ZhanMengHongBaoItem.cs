using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class ZhanMengHongBaoItem : UserControl
{
	public int HongBaoID
	{
		get
		{
			return this.mHongBaoID;
		}
		set
		{
			this.mHongBaoID = value;
		}
	}

	public int HongBaoType { get; set; }

	public int DiamondCount
	{
		get
		{
			return this.diamondCount;
		}
		set
		{
			this.diamondCount = value;
		}
	}

	public int DiamondSumCount
	{
		get
		{
			return this.diamondSumCount;
		}
		set
		{
			this.diamondSumCount = value;
		}
	}

	public string SenderName
	{
		set
		{
			this.mLblSenderName.Text = value.ToString();
		}
	}

	public string LblBottomContent
	{
		set
		{
			this.mLblBottomContent.Text = value.ToString();
		}
	}

	private void OnEnable()
	{
		base.CancelInvoke("CountDownTime");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mBtnQiangHongBao.Text = Global.GetLang("抢红包");
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.mBgClick).onClick = delegate(GameObject s)
		{
			if (!this.isChaKan)
			{
				return;
			}
			if (this.ChaHongBaoHandler != null)
			{
				this.ChaHongBaoHandler(null, new DPSelectedItemEventArgs
				{
					ID = this.HongBaoID,
					IDType = 2,
					Flag = this.HongBaoType
				});
			}
		};
		this.mBtnQiangHongBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.QiangHongBaoHandler != null)
			{
				this.QiangHongBaoHandler(null, new DPSelectedItemEventArgs
				{
					ID = this.HongBaoID,
					IDType = 1,
					Flag = this.HongBaoType
				});
			}
		};
	}

	private void InitValue()
	{
	}

	public void InitItemDataByServerData(int hongBaoYeQianType, HongBaoItemData data)
	{
		if (data == null)
		{
			return;
		}
		this.HongBaoID = data.hongBaoID;
		this.HongBaoType = data.type;
		int hongBaoStatus = data.hongBaoStatus;
		this.SenderName = data.sender;
		this.DiamondCount = data.diamondCount;
		this.DiamondSumCount = data.diamondSumCount;
		this.mBeginTime = data.beginTime;
		this.mEndTime = data.endTime;
		NGUITools.SetActive(this.mStatusObj.gameObject, hongBaoStatus == 0);
		NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, hongBaoStatus == 0);
		this.isChaKan = (hongBaoStatus != 0);
		switch (hongBaoStatus)
		{
		case 0:
		{
			NGUITools.SetActive(this.mShengYuObj, true);
			NGUITools.SetActive(this.mFaFangObj, false);
			NGUITools.SetActive(this.mHuoDeObj, false);
			NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, true);
			NGUITools.SetActive(this.mStatusObj, false);
			this.mHongBaoIcon.spriteName = "hongbao_weikai";
			DateTime correctDateTime = Global.GetCorrectDateTime();
			long num = this.mEndTime.Ticks - correctDateTime.Ticks;
			this.secondsCountDown = (int)(num / 10000000L);
			base.InvokeRepeating("CountDownTime", 0f, 1f);
			break;
		}
		case 1:
			NGUITools.SetActive(this.mShengYuObj, false);
			NGUITools.SetActive(this.mFaFangObj, false);
			NGUITools.SetActive(this.mHuoDeObj, true);
			NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, false);
			NGUITools.SetActive(this.mStatusObj, true);
			this.mHongBaoIcon.spriteName = "hongbao_yikai";
			this.mStatus.spriteName = "yiqiangguo";
			NGUITools.MakePixelPerfect(this.mStatus.transform);
			if (hongBaoYeQianType >= 1)
			{
				this.mLblHuoDeValue.Text = this.DiamondSumCount.ToString();
			}
			else
			{
				this.mLblHuoDeValue.Text = this.DiamondCount.ToString();
			}
			this.mLblTime.Text = this.GetFormatTime(this.mBeginTime);
			this.ChangeHongBaoNameDes(hongBaoYeQianType, this.mLblHuoDe);
			break;
		case 2:
			NGUITools.SetActive(this.mShengYuObj, false);
			NGUITools.SetActive(this.mFaFangObj, true);
			NGUITools.SetActive(this.mHuoDeObj, false);
			NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, false);
			NGUITools.SetActive(this.mStatusObj, true);
			this.mHongBaoIcon.spriteName = "hongbao_yikai";
			this.mStatus.spriteName = "guoqituihuan";
			NGUITools.MakePixelPerfect(this.mStatus.transform);
			if (hongBaoYeQianType >= 1)
			{
				this.mLblFaFangValue.Text = this.DiamondSumCount.ToString();
			}
			else
			{
				this.mLblFaFangValue.Text = this.DiamondCount.ToString();
			}
			this.mLblTime.Text = this.GetFormatTime(this.mBeginTime);
			this.ChangeHongBaoNameDes(hongBaoYeQianType, this.mLblFaFang);
			break;
		case 3:
			NGUITools.SetActive(this.mShengYuObj, false);
			NGUITools.SetActive(this.mFaFangObj, true);
			NGUITools.SetActive(this.mHuoDeObj, false);
			NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, false);
			NGUITools.SetActive(this.mStatusObj, true);
			this.mHongBaoIcon.spriteName = "hongbao_yikai";
			this.mStatus.spriteName = "yiqiangwan";
			NGUITools.MakePixelPerfect(this.mStatus.transform);
			if (hongBaoYeQianType >= 1)
			{
				this.mLblFaFangValue.Text = this.DiamondSumCount.ToString();
			}
			else
			{
				this.mLblFaFangValue.Text = this.DiamondCount.ToString();
			}
			this.mLblTime.Text = this.GetFormatTime(this.mBeginTime);
			this.ChangeHongBaoNameDes(hongBaoYeQianType, this.mLblFaFang);
			break;
		default:
			Super.HintMainText(Global.GetLang("红包状态有误"), 10, 3);
			break;
		}
	}

	private void ChangeHongBaoNameDes(int type, TextBlock label)
	{
		switch (type)
		{
		case 0:
			label.Text = Global.GetLang("获得：");
			break;
		case 1:
			label.Text = Global.GetLang("发放：");
			break;
		case 2:
			label.Text = Global.GetLang("发放：");
			break;
		}
	}

	private string GetFormatTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd");
	}

	private void CountDownTime()
	{
		this.LblBottomContent = string.Format("{0}{1}", Global.GetLang("剩余时间："), this.GetTimeStrBySec((double)this.secondsCountDown, false));
		this.secondsCountDown--;
		if (this.secondsCountDown < 0)
		{
			base.CancelInvoke("CountDownTime");
			NGUITools.SetActive(this.mShengYuObj, false);
			NGUITools.SetActive(this.mStatusObj.gameObject, true);
			this.mStatus.spriteName = "guoqituihuan";
			this.mLblTime.Text = this.mBeginTime.ToString("yyyy-MM-dd");
			NGUITools.SetActive(this.mFaFangObj, true);
			this.mLblFaFang.Text = Global.GetLang("获得：");
			this.mLblFaFangValue.Text = this.DiamondCount.ToString();
			NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, false);
			this.isChaKan = true;
		}
	}

	public void ChangeStatusToYiLingQu()
	{
		NGUITools.SetActive(this.mShengYuObj, false);
		NGUITools.SetActive(this.mFaFangObj, false);
		NGUITools.SetActive(this.mHuoDeObj, true);
		NGUITools.SetActive(this.mBtnQiangHongBao.gameObject, false);
		NGUITools.SetActive(this.mStatusObj, true);
		this.mHongBaoIcon.spriteName = "hongbao_yikai";
		this.mStatus.spriteName = "yulingqu";
		NGUITools.MakePixelPerfect(this.mStatus.transform);
		this.mLblHuoDeValue.Text = this.DiamondCount.ToString();
		this.mLblTime.Text = this.mBeginTime.ToString("yyyy-MM-dd");
	}

	public string GetTimeStrBySec(double sec, bool showDay = true)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		if (!showDay)
		{
			return string.Format("{0:T}", StringUtil.substitute(Global.GetLang("{0}:{1}:{2}"), new object[]
			{
				((int)(sec % (double)num / (double)num2)).ToString("00"),
				((int)(sec % (double)num % (double)num2 / (double)num3)).ToString("00"),
				((int)(sec % (double)num % (double)num2 % (double)num3)).ToString("00")
			}));
		}
		return string.Format("{0:T}", StringUtil.substitute(Global.GetLang("{0}:{1}:{2}:{3}"), new object[]
		{
			((int)(sec / (double)num)).ToString("00"),
			((int)(sec % (double)num / (double)num2)).ToString("00"),
			((int)(sec % (double)num % (double)num2 / (double)num3)).ToString("00"),
			((int)(sec % (double)num % (double)num2 % (double)num3)).ToString("00")
		}));
	}

	public void CancelInvoke()
	{
		base.CancelInvoke("CountDownTime");
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("CountDownTime");
		this.QiangHongBaoHandler = null;
		this.ChaHongBaoHandler = null;
		this.mSpriteHongBaoIcon = null;
		this.mLblSenderName = null;
		this.mLblBottomContent = null;
		this.mStatus = null;
		this.mLblTime = null;
		this.mBtnQiangHongBao = null;
		this.mBgClick = null;
	}

	private const string yiqiangguang = "yiqiangwan";

	private const string yiqiangguo = "yiqiangguo";

	private const string yilingqu = "yulingqu";

	private const string yiguoqi = "guoqituihuan";

	public DPSelectedItemEventHandler QiangHongBaoHandler;

	public DPSelectedItemEventHandler ChaHongBaoHandler;

	public UISprite mSpriteHongBaoIcon;

	public TextBlock mLblSenderName;

	public TextBlock mLblBottomContent;

	public GButton mBtnQiangHongBao;

	public GameObject mBgClick;

	public GameObject mShengYuObj;

	public TextBlock mLblShengYu;

	public TextBlock mLblShengYuTime;

	public GameObject mFaFangObj;

	public TextBlock mLblFaFang;

	public TextBlock mLblFaFangValue;

	public GameObject mHuoDeObj;

	public TextBlock mLblHuoDe;

	public TextBlock mLblHuoDeValue;

	public GameObject mStatusObj;

	public TextBlock mLblTime;

	public UISprite mStatus;

	public UISprite mHongBaoIcon;

	private DateTime mBeginTime;

	private DateTime mEndTime;

	private int mHongBaoID;

	private int diamondCount;

	private int diamondSumCount;

	public bool isChaKan;

	private int secondsCountDown;

	private enum EHongBaoStatus
	{
		ShengYu,
		YiLingQu,
		YiGuoQi,
		YiQiangGuang
	}
}
