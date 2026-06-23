using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class LianShaShow : MonoBehaviour
{
	private void Start()
	{
		this._SPLabel.text = Global.GetLang("连斩");
		this._ValueLabel.transform.localPosition = new Vector3(-60f, -19f, -1f);
		this._MyUIAnchor.enabled = true;
	}

	private float GetPercent()
	{
		float num = 0f;
		if (this.mOverTicks != 0L && this.mBufferSecs != 0L)
		{
			long num2 = this.mOverTicks - Global.GetCorrectLocalTime();
			if (0L < num2)
			{
				num = (float)num2 / (float)this.mBufferSecs;
			}
		}
		return Mathf.Abs(num);
	}

	public void RefreshValue(int Number)
	{
		this._ValueLabel.text = Number.ToString();
	}

	private void RefreshProgerss(float Value)
	{
		Value = Mathf.Clamp01(Value);
		if (null != this._ProgressTrans)
		{
			this._ProgressTrans.fillAmount = Value;
		}
	}

	public void RefreshData(long OverTicks, long BufferSecs)
	{
		this.mOverTicks = OverTicks;
		this.mBufferSecs = BufferSecs;
	}

	public bool Active
	{
		get
		{
			return 0 == this.mSelfHaveRemove;
		}
	}

	public void ActiveSelf()
	{
		MUDebug.Log<string>(new string[]
		{
			"连杀 激活  mSelfHaveRemove =  " + this.mSelfHaveRemove
		});
		if (this.mSelfHaveRemove == 1)
		{
			UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren != null)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			this.mSelfHaveRemove = 0;
			this._MyUIAnchor.enabled = true;
		}
	}

	public void RemoveSelf()
	{
		MUDebug.Log<string>(new string[]
		{
			"连杀 关闭  mSelfHaveRemove =  " + this.mSelfHaveRemove
		});
		if (this.mSelfHaveRemove == 0)
		{
			this._MyUIAnchor.enabled = false;
			base.transform.localPosition = new Vector3(-2000f, -2000f, 0f);
			UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren != null)
				{
					componentsInChildren[i].enabled = false;
				}
			}
			this.mSelfHaveRemove = 1;
		}
	}

	public bool UpdateUI()
	{
		float percent = this.GetPercent();
		if (!Mathf.Approximately(0f, percent))
		{
			this.RefreshProgerss(percent);
			return true;
		}
		return false;
	}

	private const float ProgressWight = 189f;

	[SerializeField]
	private UILabel _ValueLabel;

	[SerializeField]
	private UILabel _SPLabel;

	[SerializeField]
	private UISprite _ProgressTrans;

	[SerializeField]
	private MyUIAnchor _MyUIAnchor;

	private long mOverTicks;

	private long mBufferSecs;

	private bool TimerProcHaveStart;

	private byte mSelfHaveRemove;
}
