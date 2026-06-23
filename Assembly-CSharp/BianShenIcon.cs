using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class BianShenIcon : UserControl
{
	public bool BeInCDTime
	{
		get
		{
			return this.m_beInCDTime;
		}
	}

	public void ServerSynCD(long endTime)
	{
		long num = endTime - Global.GetCorrectLocalTime();
		if (num > 0L)
		{
			this.RefreshCoolInfo(Global.GetCorrectLocalTime(), num);
			this.RefreserNowTime();
		}
	}

	private void RefreshCoolInfo(long startTime, long timeCD)
	{
		this.m_cdStartTime = startTime;
		this.m_cdTotalTime = timeCD;
	}

	private void RefreserNowTime()
	{
		long num = Global.GetCorrectLocalTime() - this.m_cdStartTime;
		this.m_cdStartTime = Global.GetCorrectLocalTime();
		this.m_cdTotalTime -= num;
		this.lblTime.text = string.Empty;
		if (this.m_cdTotalTime <= 0L)
		{
			this.m_cdTotalTime = 1L;
			this.FinishCD();
		}
		else
		{
			this.m_beInCDTime = true;
			if (!this.m_beInBianShen)
			{
				this.UpdateCDState();
			}
		}
	}

	public void UpdateNumInfo()
	{
		if (!ShenHunData.IsShenHunOpen())
		{
			return;
		}
		int num = (int)ShenHunData.GetFreeBianShenNum();
		if (num > 0)
		{
			this.lblOwnNum.color = Color.yellow;
			this.lblOwnNum.text = num.ToString();
			return;
		}
		int bianShenGoodsNum = ShenHunData.GetBianShenGoodsNum();
		if (bianShenGoodsNum > 99)
		{
			this.lblOwnNum.text = "99+";
		}
		else
		{
			this.lblOwnNum.text = bianShenGoodsNum.ToString();
		}
		this.lblOwnNum.color = Color.red;
	}

	public void BianShenStart()
	{
		this.m_beInBianShen = true;
		this.Percent = 0f;
		this.mGameTeXiao.SetActive(true);
		this.UpdateNumInfo();
	}

	public void BianShenEnd()
	{
		this.m_beInBianShen = false;
		this.mGameTeXiao.SetActive(false);
		this.Percent = 1f;
		this.RefreserNowTime();
	}

	private void UpdateCDState()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (this.m_cdTotalTime <= 0L)
		{
			this.FinishCD();
		}
		long num = this.m_cdTotalTime - (correctLocalTime - this.m_cdStartTime);
		if (num > 0L)
		{
			this.lblCDTime.text = (num / 1000L + 1L).ToString();
			this.Percent = (float)num * 1f / (float)this.m_cdTotalTime;
			if (this.mGameTeXiao.activeSelf)
			{
				this.mGameTeXiao.SetActive(false);
			}
		}
		else
		{
			this.lblCDTime.text = string.Empty;
			this.FinishCD();
		}
	}

	private void FinishCD()
	{
		this.Percent = 0f;
		this.m_beInCDTime = false;
	}

	private void UpdateBianShenState()
	{
		BufferData bufferDataByID = Global.GetBufferDataByID(121);
		if (bufferDataByID != null)
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			long num = (long)bufferDataByID.BufferSecs * 1000L;
			long num2 = num - (correctLocalTime - bufferDataByID.StartTime);
			if (num2 > 0L)
			{
				this.lblTime.text = (num2 / 1000L + 1L).ToString();
				this.lblCDTime.text = string.Empty;
			}
			else
			{
				this.RefreserNowTime();
				this.lblTime.text = string.Empty;
				this.m_beInBianShen = false;
			}
		}
	}

	public void UpdateInBianShenState()
	{
		this.m_beInBianShen = ShenHunData.IsInBianShenState();
	}

	private IEnumerator UpdateInfo()
	{
		for (;;)
		{
			this.UpdateLastUI();
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	protected void OnEnable()
	{
		base.StartCoroutine<bool>(this.UpdateInfo());
	}

	protected void OnDisable()
	{
		base.StopAllCoroutines();
	}

	private void UpdateLastUI()
	{
		if (this.m_beInBianShen)
		{
			this.UpdateBianShenState();
		}
		else if (this.m_beInCDTime)
		{
			this.UpdateCDState();
		}
		if (this.m_dayNum != Global.GetCorrectDateTime().Day)
		{
			this.UpdateNumInfo();
			this.m_dayNum = Global.GetCorrectDateTime().Day;
		}
	}

	public float Percent
	{
		set
		{
			if (null != this.mSprForeground)
			{
				this.mSprForeground.fillAmount = value;
			}
		}
	}

	public UILabel lblOwnNum;

	public UILabel lblTime;

	public UILabel lblCDTime;

	[SerializeField]
	private UISprite mSprForeground;

	[SerializeField]
	private GameObject mGameTeXiao;

	private long m_cdStartTime;

	private long m_cdTotalTime;

	private bool m_beInBianShen;

	private bool m_beInCDTime;

	private int m_dayNum;
}
