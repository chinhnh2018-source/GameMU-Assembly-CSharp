using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class HuiJiIcon : UserControl
{
	public void RefreshTime(int EmblemID, long timeCD, long ContinuedTicks)
	{
		if (ConfigSystemParam.GetSystemParamIntByName("EmblemProp") != (long)EmblemID)
		{
			MUDebug.Log<string>(new string[]
			{
				"EmblemID错误：" + EmblemID
			});
			return;
		}
		if (timeCD <= 0L && ContinuedTicks <= 0L)
		{
			this.Percent = 1f;
			this.mBool = false;
			this.mGameTeXiao.SetActive(true);
			base.StopCoroutine("TimerHuijiLast");
			return;
		}
		if (this.mGameTeXiao.activeSelf)
		{
			this.mGameTeXiao.SetActive(false);
		}
		this.mMaxCDTime = timeCD;
		this.mMaxChiXuTime = ContinuedTicks;
		this.mTime = timeCD + ContinuedTicks;
		if (!this.mBool)
		{
			this.mBool = true;
			this.SetTime(timeCD + ContinuedTicks);
		}
	}

	public void SetTime(long startTime = -1L)
	{
		this.mTime = startTime;
		this.mBool = true;
		MUDebug.Log<string>(new string[]
		{
			"TimerHuijiLast开启了"
		});
		if (UICtrlBar.singleton.gameObject.activeSelf && NGUITools.GetActive(base.gameObject))
		{
			base.StartCoroutine("TimerHuijiLast");
		}
		this.Percent = 0f;
		this.mGameTeXiao.SetActive(false);
	}

	private IEnumerator TimerHuijiLast()
	{
		do
		{
			this.UpdateLastUI();
			yield return new WaitForSeconds(0.2f);
		}
		while (this.mBool);
		yield break;
	}

	protected void OnEnable()
	{
		if (this.mBool && Global.Data.roleData != null)
		{
			EmblemCoolDownItem emblemItem = Global.GetEmblemItem();
			if (emblemItem != null)
			{
				this.mTime = emblemItem.StartTicks + emblemItem.CDTicks + emblemItem.ContinuedTicks - Global.GetCorrectLocalTime();
				this.mMaxCDTime = emblemItem.CDTicks;
				this.mMaxChiXuTime = emblemItem.ContinuedTicks;
			}
			if (UICtrlBar.singleton.gameObject.activeSelf && NGUITools.GetActive(base.gameObject))
			{
				base.StartCoroutine("TimerHuijiLast");
			}
		}
	}

	private void UpdateLastUI()
	{
		if (this.mBool)
		{
			if (this.mTime > 0L && this.mMaxCDTime > 0L)
			{
				this.Percent = 1f - (float)this.mTime / (float)this.mMaxCDTime;
			}
			else
			{
				this.Percent = 1f;
				this.mBool = false;
				this.mGameTeXiao.SetActive(true);
				base.StopCoroutine("TimerHuijiLast");
			}
		}
	}

	public override void Update()
	{
		if (this.mBool && this.mTime >= 0L)
		{
			this.mTime -= TmskTime.DeltaMills();
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

	public void RefreshIcon()
	{
		try
		{
			int roleEmblemLevel = Global.GetRoleEmblemLevel(Global.Data.roleData.RoleID);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	[ContextMenu("sss")]
	private void sss()
	{
		if (UICtrlBar.singleton.gameObject.activeSelf && NGUITools.GetActive(base.gameObject))
		{
			base.StartCoroutine("TimerHuijiLast");
		}
	}

	[SerializeField]
	private UISprite mSprForeground;

	[SerializeField]
	private GameObject mGameTeXiao;

	private long mTime = -1L;

	public bool mBool;

	private long mMaxCDTime = -1L;

	private long mMaxChiXuTime = -1L;
}
