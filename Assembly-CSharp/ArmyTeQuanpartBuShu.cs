using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class ArmyTeQuanpartBuShu : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
		this.InitConfigValue();
		this.m_ProgressBar.gameObject.SetActive(false);
	}

	private void InitTextInPrefabs()
	{
		try
		{
			this.m_FanRongDu.Text = Global.GetLang("消耗繁荣度：");
			this.m_BtnBuShu.Text = Global.GetLang("部署");
			this.m_FanRongDu.Pivot = 2;
			this.m_FanRongDu.X = 40.0;
			this.m_FanRongDu.Y = 40.0;
			this.m_FanRongDu.MaxWidth = 110.0;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "越南东南亚英文：预制可能报空了</color>"
			});
		}
	}

	private void InitBtnCallBack()
	{
		this.m_BtnBuShu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.RefreshIconCallBack != null)
			{
				this.RefreshIconCallBack(null, new DPSelectedItemEventArgs
				{
					ID = this.m_Index,
					Level = this.GetCostValue(),
					IDType = this.m_Type
				});
			}
		};
	}

	private void InitConfigValue()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("GuardCost", ',');
		this.m_BuShuGuardCost = systemParamIntArrayByName[0];
		this.m_FuHuoGuardCost = systemParamIntArrayByName[1];
		this.mFuHuoSumTime = systemParamIntArrayByName[2];
		this.m_FanRongDuValue.Text = this.m_BuShuGuardCost.ToString();
	}

	private int GetCostValue()
	{
		int result = 0;
		int type = this.m_Type;
		if (type != 1)
		{
			if (type != 2)
			{
				MUDebug.Log<string>(new string[]
				{
					"类型出错！"
				});
			}
			else
			{
				result = this.currentZuanShiCost;
			}
		}
		else
		{
			result = this.m_BuShuGuardCost;
		}
		return result;
	}

	public void InitTypeValue(int type, DateTime relifeTime, int costDiamond)
	{
		base.CancelInvoke("TickProcs");
		this.m_Type = type;
		switch (this.m_Type)
		{
		case 1:
			this.InitConfigValue();
			this.m_DiamondIcon.gameObject.SetActive(false);
			this.m_TimeLabel.gameObject.SetActive(false);
			this.m_FanRongDu.Text = Global.GetLang("消耗繁荣度：");
			this.m_BtnBuShu.Text = Global.GetLang("部署");
			this.m_BtnBuShu.target.spriteName = "btn_normal1";
			this.m_BtnBuShu.normalSprite = "btn_normal1";
			this.m_BtnBuShu.hoverSprite = "btn_normal1";
			this.m_BtnBuShu.pressedSprite = "btn_press";
			this.m_BtnBuShu.disabledSprite = "btn_press";
			break;
		case 2:
		{
			this.mCostDiamond = costDiamond;
			this.InitConfigValue();
			this.m_FanRongDuValue.Text = this.mCostDiamond.ToString();
			this.m_BtnBuShu.isEnabled = true;
			this.m_DiamondIcon.gameObject.SetActive(true);
			this.m_TimeLabel.gameObject.SetActive(true);
			this.m_FanRongDu.Text = Global.GetLang("消耗钻石    ：");
			this.m_BtnBuShu.Text = Global.GetLang("立即复活");
			this.m_BtnBuShu.target.spriteName = "btn_normal2";
			this.m_BtnBuShu.normalSprite = "btn_normal2";
			this.m_BtnBuShu.hoverSprite = "btn_normal2";
			this.m_BtnBuShu.pressedSprite = "btn_press";
			this.m_BtnBuShu.disabledSprite = "btn_press";
			DateTime correctDateTime = Global.GetCorrectDateTime();
			long num = relifeTime.Ticks - correctDateTime.Ticks;
			this.secondsCountDown = (int)(num / 10000000L);
			this.maskCountDown = (float)this.secondsCountDown;
			if (!this.m_ProgressBar.gameObject.activeSelf)
			{
				this.m_ProgressBar.gameObject.SetActive(true);
			}
			base.InvokeRepeating("TickProcs", 0f, 1f);
			break;
		}
		case 3:
			this.InitConfigValue();
			this.m_DiamondIcon.gameObject.SetActive(false);
			this.m_TimeLabel.gameObject.SetActive(false);
			this.m_ProgressBar.gameObject.SetActive(false);
			break;
		default:
			MUDebug.Log<string>(new string[]
			{
				"类型出错！"
			});
			break;
		}
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
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3),
				(int)(sec % (double)num % (double)num2 % (double)num3)
			}));
		}
		return string.Format("{0:T}", StringUtil.substitute(Global.GetLang("{0}:{1}:{2}:{3}"), new object[]
		{
			(int)(sec / (double)num),
			(int)(sec % (double)num / (double)num2),
			(int)(sec % (double)num % (double)num2 / (double)num3),
			(int)(sec % (double)num % (double)num2 % (double)num3)
		}));
	}

	private void TickProcs()
	{
		this.m_TimeLabel.text = string.Format(Global.GetLang("{0}"), this.GetTimeStrBySec((double)this.secondsCountDown, false));
		this.secondsCountDown--;
		if (this.secondsCountDown > 0)
		{
			float num = (float)this.secondsCountDown / (float)this.mFuHuoSumTime;
			this.currentZuanShiCost = (int)((float)this.m_FuHuoGuardCost * num + 1f);
			this.m_FanRongDuValue.Text = this.currentZuanShiCost.ToString();
		}
		if (this.secondsCountDown < 0)
		{
			base.CancelInvoke("TickProcs");
			this.m_TimeLabel.gameObject.SetActive(false);
			this.SetButtonDisable();
			this.m_ProgressBar.gameObject.SetActive(false);
			if (this.RefreshIconMaskCallBack != null)
			{
				this.RefreshIconMaskCallBack(null, new DPSelectedItemEventArgs
				{
					ID = this.m_Index,
					Flag = (int)this.maskCountDown,
					Level = 0
				});
			}
		}
	}

	public void SetButtonDisable()
	{
		this.InitConfigValue();
		this.m_FanRongDu.Text = Global.GetLang("消耗繁荣度：");
		this.m_BtnBuShu.Text = Global.GetLang("部署");
		this.m_DiamondIcon.gameObject.SetActive(false);
		this.m_TimeLabel.gameObject.SetActive(false);
		this.m_BtnBuShu.isEnabled = false;
	}

	public void CloseCountDown()
	{
		base.CancelInvoke("TickProcs");
		this.m_TimeLabel.gameObject.SetActive(false);
		this.m_ProgressBar.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("TickProcs");
		this.m_Index = 0;
		this.m_Type = 1;
		this.RefreshIconCallBack = null;
		this.RefreshIconMaskCallBack = null;
		this.m_FanRongDu = null;
		this.m_FanRongDuValue = null;
		this.m_BtnBuShu = null;
		this.m_BuShuGuardCost = 0;
		this.m_FuHuoGuardCost = 0;
		this.m_TimeLabel = null;
		this.m_DiamondIcon = null;
	}

	[Tooltip("设置当前部署按钮的Index")]
	public int m_Index;

	public DPSelectedItemEventHandler RefreshIconCallBack;

	public DPSelectedItemEventHandler RefreshIconMaskCallBack;

	public TextBlock m_FanRongDu;

	public TextBlock m_FanRongDuValue;

	public GButton m_BtnBuShu;

	private int m_BuShuGuardCost;

	private int m_FuHuoGuardCost;

	private int mFuHuoSumTime;

	public TextBlock m_TimeLabel;

	public UISprite m_DiamondIcon;

	private int m_Type = 1;

	public UISprite m_ProgressBar;

	private int mCostDiamond;

	private int currentZuanShiCost;

	private int secondsCountDown;

	private float maskCountDown;
}
