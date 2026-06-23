using System;
using System.Collections;
using UnityEngine;

public class GSkillIcon : GTipSprite
{
	public UIEventListener.VectorDelegate onDrag
	{
		set
		{
			UIEventListener.Get(this.StillIcon.gameObject).onDrag = value;
		}
	}

	public UIEventListener.VoidDelegate onClick
	{
		set
		{
			UIEventListener.Get(this.StillIcon.gameObject).onClick = value;
		}
	}

	public bool IsPressed
	{
		get
		{
			return this.StillIcon.IsPressed;
		}
	}

	public GSkillIcon.GSkillIconType IconType
	{
		set
		{
			this._IconType = value;
			if (value == GSkillIcon.GSkillIconType.Skill)
			{
				if (!this.beBigSkillIcon)
				{
					this.StillIcon.normalSprite = "mainStillBorder_normal";
					this.StillIcon.hoverSprite = "mainStillBorder_normal";
					this.StillIcon.pressedSprite = "mainStillBorder_hover";
					this.StillIcon.disabledSprite = "mainStillBorder_diable";
				}
				else
				{
					this.StillIcon.normalSprite = "mainStillBorder2";
					this.StillIcon.hoverSprite = "mainStillBorder2";
					this.StillIcon.pressedSprite = "mainStillBorder2";
					this.StillIcon.disabledSprite = "mainStillBorder_diable";
				}
			}
			else if (value == GSkillIcon.GSkillIconType.Goods)
			{
				this.StillIcon.normalSprite = "mainStillBorder_normal";
				this.StillIcon.hoverSprite = "mainStillBorder_hover";
				this.StillIcon.pressedSprite = "mainStillBorder_normal";
				this.StillIcon.disabledSprite = "mainStillBorder_normal";
				this.StillIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(base.NormalGoodsTipsHandler);
			}
			else if (value == GSkillIcon.GSkillIconType.Buff)
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
				this._MaskImage.fillAmount = value;
			}
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
			this.UpdateLayout();
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
			this.UpdateLayout();
			this.ReSize();
		}
	}

	public double InnerWidth
	{
		get
		{
			return this._InnerWidth;
		}
		set
		{
			this._InnerWidth = value;
			this.ReSize();
		}
	}

	public double InnerHeight
	{
		get
		{
			return this._InnerHeight;
		}
		set
		{
			this._InnerHeight = value;
			this.ReSize();
		}
	}

	public double OuterWidth
	{
		get
		{
			return this._OuterWidth;
		}
		set
		{
			this._OuterWidth = value;
			this.ReSize();
		}
	}

	public double OuterHeight
	{
		get
		{
			return this._OuterHeight;
		}
		set
		{
			this._OuterHeight = value;
			this.UpdateLayout();
			this.ReSize();
		}
	}

	public bool SelectState
	{
		set
		{
			this._SelectBorder.gameObject.SetActive(value);
		}
	}

	public new string Name
	{
		set
		{
			if (value != null)
			{
				this._Name.text = value;
			}
		}
	}

	public int NameSize
	{
		set
		{
			this._Name.transform.localScale = new Vector3((float)value, (float)value, 0f);
		}
	}

	public int Level
	{
		set
		{
			this._Level.text = value.ToString();
		}
	}

	public bool EnableState
	{
		get
		{
			return this.m_enableState;
		}
		set
		{
			this.m_enableState = value;
			if (this.TagIndex == 4 && !this.m_enableState)
			{
				this.StillIcon.normalSprite = "mainStillBorder_suo";
				this.StillIcon.hoverSprite = "mainStillBorder_suo";
				this.StillIcon.pressedSprite = "mainStillBorder_suo";
				this.StillIcon.disabledSprite = "mainStillBorder_suo";
			}
			else if (!this.beBigSkillIcon)
			{
				this.StillIcon.isEnabled = this.m_enableState;
				this.StillIcon.normalSprite = "mainStillBorder";
				this.StillIcon.hoverSprite = "mainStillBorder";
				this.StillIcon.pressedSprite = "mainStillBorder";
				this.StillIcon.disabledSprite = "mainStillBorder_suo";
			}
			else
			{
				this.StillIcon.isEnabled = this.m_enableState;
				this.StillIcon.normalSprite = "mainStillBorder2";
				this.StillIcon.hoverSprite = "mainStillBorder2";
				this.StillIcon.pressedSprite = "mainStillBorder2";
				this.StillIcon.disabledSprite = "mainStillBorder_suo";
			}
			if (!this.m_enableState)
			{
				this.BodyURL = null;
			}
		}
	}

	public int TagCode { get; set; }

	public int TagIndex { get; set; }

	private bool IsCooldown { get; set; }

	public void MyStart(long ticks, bool showText = true, long startTicks = 0L, bool isFromLightToDark = true, bool isDrawTicks = true, long totalTicks = -1L)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this._IsFromLightToDark = isFromLightToDark;
		this._isDrawTicks = isDrawTicks;
		this._startTicks = startTicks;
		this._leftTicks = ticks;
		this._totalTicks = ((totalTicks != -1L) ? totalTicks : ticks);
		this._currentInterval = Math.Max(40L, this._totalTicks / 1000L);
		this._Interval = (float)this._currentInterval / 1000f;
		if (!this.IsCooldown)
		{
			base.StartCoroutine("TimerProc");
			this.IsCooldown = true;
			this._eachCoroutineDelatTime = TmskTime.CurMills();
		}
	}

	public void StopCoolDown()
	{
		this._leftTicks = -1L;
		this.IsCooldown = false;
		this._eachCoroutineDelatTime = 0L;
	}

	public bool UpdateUI()
	{
		if (this._eachCoroutineDelatTime == 0L)
		{
			this._eachCoroutineDelatTime = TmskTime.CurMills();
		}
		this._leftTicks -= TmskTime.CurMills() - this._eachCoroutineDelatTime;
		this._eachCoroutineDelatTime = TmskTime.CurMills();
		if (this._leftTicks <= 0L || this._startTicks < 0L)
		{
			if (this.CoodDownComplete != null)
			{
				this.CoodDownComplete.Invoke(this, EventArgs.Empty);
			}
			this.Percent = 0f;
			this.IsCooldown = false;
			this._eachCoroutineDelatTime = 0L;
			base.StopCoroutine("TimerProc");
			this._CoolDown.text = null;
			return false;
		}
		this._CoolDown.text = string.Format("{0}", Mathf.CeilToInt((float)this._leftTicks / 1000f));
		float percent = (float)this._leftTicks / (float)this._totalTicks;
		this.Percent = percent;
		return true;
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
			this.StillImg.URL = this._BodyURL;
			if (string.IsNullOrEmpty(this._BodyURL))
			{
				this.StillImg.Texture.enabled = false;
				this.StopCoolDown();
			}
			else
			{
				this.StillImg.Texture.enabled = true;
			}
		}
	}

	protected override void InitializeComponent()
	{
		if (this._IconType != GSkillIcon.GSkillIconType.Skill)
		{
			this.IconType = this._IconType;
		}
	}

	protected void OnEnable()
	{
		base.StartCoroutine("TimerProc");
	}

	private IEnumerator TimerProc()
	{
		while (!this._isDrawTicks || this.UpdateUI())
		{
			yield return new WaitForSeconds(this._Interval);
		}
		yield break;
		yield break;
	}

	protected virtual void ReSize()
	{
		if (!double.IsNaN(this._Width) && !double.IsNaN(this._Height))
		{
			(this.StillIcon.GetComponent<Collider>() as BoxCollider).size = new Vector3((float)this._Width, (float)this._Height, 1f);
			this.StillIcon.target.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			this._MaskImage.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			this._SelectBorder.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			if (this._InnerWidth == 0.0 && this._InnerHeight == 0.0)
			{
				this.StillImg.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			}
			else
			{
				this.StillImg.transform.localScale = new Vector3((float)this._InnerWidth, (float)this._InnerHeight, 1f);
			}
			if (this._OuterWidth == 0.0 && this._OuterHeight == 0.0)
			{
				this._SelectBorder.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			}
			else
			{
				this._SelectBorder.transform.localScale = new Vector3((float)this._OuterWidth, (float)this._OuterHeight, 1f);
			}
			return;
		}
	}

	public ShowNetImage StillImg;

	public GButton StillIcon;

	public UISprite _MaskImage;

	public UISprite _SelectBorder;

	public UILabel _Level;

	public UILabel _CoolDown;

	public UILabel _Name;

	public bool beBigSkillIcon;

	[HideInInspector, SerializeField]
	public GSkillIcon.GSkillIconType _IconType;

	public EventHandler CoodDownComplete;

	private double _InnerWidth;

	private double _InnerHeight;

	private double _OuterWidth;

	private double _OuterHeight;

	private bool m_enableState = true;

	protected float _Interval = 1f;

	private bool _isDrawTicks = true;

	private long _startTicks;

	private long _leftTicks;

	private long _totalTicks;

	private long _currentInterval = 200L;

	private long _eachCoroutineDelatTime;

	private string _BodyURL;

	public enum GSkillIconType
	{
		Skill,
		Goods,
		Buff
	}
}
