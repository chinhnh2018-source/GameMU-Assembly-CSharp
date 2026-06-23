using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class GStillIcon : GTipSprite
{
	public GStillIcon.GSillIconType IconType
	{
		set
		{
			this._IconType = value;
			if (value == GStillIcon.GSillIconType.Skill)
			{
				this.StillIcon.normalSprite = "mainStillBorder_normal";
				this.StillIcon.hoverSprite = "mainStillBorder_normal";
				this.StillIcon.pressedSprite = "mainStillBorder_hover";
				this.StillIcon.disabledSprite = "mainStillBorder_diable";
			}
			else if (value == GStillIcon.GSillIconType.Goods)
			{
				this.StillIcon.normalSprite = "mainStillBorder_normal";
				this.StillIcon.hoverSprite = "mainStillBorder_hover";
				this.StillIcon.pressedSprite = "mainStillBorder_normal";
				this.StillIcon.disabledSprite = "mainStillBorder_normal";
				this.StillIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(base.NormalGoodsTipsHandler);
			}
			else if (value == GStillIcon.GSillIconType.Buff)
			{
				this.StillIcon.normalSprite = "buffGrid_bak";
				this.StillIcon.hoverSprite = "buffGrid_bak";
				this.StillIcon.pressedSprite = "buffGrid_bak";
				this.StillIcon.disabledSprite = "buffGrid_bak";
				this.StillIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(base.NormalBuffTipsHandler);
			}
			this.StillIcon.Refresh();
		}
	}

	private bool _IsFromLightToDark
	{
		set
		{
			this._MaskImage.invert = !value;
		}
	}

	public float Percent
	{
		set
		{
			if (null != this._MaskImage)
			{
				if (this.ShowCDMask)
				{
					this._MaskImage.fillAmount = 1f - value;
				}
				else
				{
					this._MaskImage.fillAmount = 0f;
				}
			}
		}
	}

	public string SkillText
	{
		set
		{
			this._SkillText.text = value;
		}
	}

	public new double Width
	{
		get
		{
			return this._Width;
		}
		set
		{
			this._Width = value;
			this.ReSize();
		}
	}

	public new double Height
	{
		get
		{
			return this._Height;
		}
		set
		{
			this._Height = value;
			this.ReSize();
		}
	}

	public void MyStart(long ticks, bool showText = true, int timerTicks = 100, long startTicks = 0L, bool isFromLightToDark = true, bool isDrawTicks = true)
	{
		this._IsFromLightToDark = isFromLightToDark;
		this._isDrawTicks = isDrawTicks;
		this._timerTicks = Mathf.Max(50, timerTicks);
		this._totalTicks = ticks;
		this._leftTicks = this._totalTicks - Math.Max(Global.GetCorrectLocalTime() - startTicks, 0L);
		this._currentInterval = Math.Max(200L, this._totalTicks / 1000L);
		this._startTicks = startTicks;
		this._Interval = Mathf.Clamp((float)this._currentInterval / 1000f, (float)this._timerTicks / 1000f, 1f);
	}

	public void UpdateUI()
	{
		if (this._leftTicks <= 0L)
		{
			if (this.CoodDownComplete != null)
			{
				this.CoodDownComplete.Invoke(this, EventArgs.Empty);
			}
			return;
		}
		int num = (int)(this._leftTicks / 1000L);
		double num2 = Math.Max(this.Width, this.Height);
		float percent = 1f - (float)this._leftTicks / (float)this._totalTicks;
		this.Percent = percent;
		if (this.ShowText && this._IconType == GStillIcon.GSillIconType.Buff)
		{
			if (this._leftTicks >= 50L)
			{
				this.SkillText = UIHelper.FormatSecs1(this._leftTicks / 1000L, "-");
			}
			else
			{
				this.SkillText = null;
			}
		}
		this._leftTicks -= this._currentInterval;
	}

	public string BodyURL
	{
		get
		{
			return this._BodyURL;
		}
		set
		{
			this._BodyURL = value;
			if (value != null)
			{
				this.StillImg.URL = this._BodyURL;
			}
		}
	}

	protected override void InitializeComponent()
	{
		if (this._IconType != GStillIcon.GSillIconType.Skill)
		{
			this.IconType = this._IconType;
		}
	}

	protected void OnEnable()
	{
		base.StartCoroutine<bool>(this.TimerProc());
	}

	private IEnumerator TimerProc()
	{
		for (;;)
		{
			if (this._isDrawTicks)
			{
				this.UpdateUI();
				yield return new WaitForSeconds(this._Interval);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		yield break;
	}

	protected virtual void ReSize()
	{
		if (!double.IsNaN(this._Width) && !double.IsNaN(this._Height))
		{
			this.StillImg.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			this._MaskImage.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
		}
	}

	public ShowNetImage StillImg;

	public GButton StillIcon;

	public UISprite _MaskImage;

	public UILabel _SkillText;

	public bool ShowText;

	public bool ShowCDMask = true;

	[HideInInspector, SerializeField]
	public GStillIcon.GSillIconType _IconType;

	public EventHandler CoodDownComplete;

	protected float _Interval = 1f;

	private bool _isDrawTicks = true;

	private int _timerTicks;

	private long _startTicks;

	private long _totalTicks;

	private long _leftTicks;

	private long _currentInterval = 200L;

	private string _BodyURL;

	public enum GSillIconType
	{
		Skill,
		Goods,
		Buff
	}
}
