using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class RoleExpProgressBar : GBasePart
{
	protected override void InitializeComponent()
	{
		this.EXPBar.uiLabel.gameObject.SetActive(false);
		UIEventListener.Get(this.ProgGameobject.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (b)
			{
				this.EXPBar.uiLabel.gameObject.SetActive(true);
			}
			else
			{
				this.EXPBar.uiLabel.gameObject.SetActive(false);
			}
		};
		UIEventListener.Get(this.ProgGameobject1.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (b)
			{
				this.EXPBar.uiLabel.gameObject.SetActive(true);
			}
			else
			{
				this.EXPBar.uiLabel.gameObject.SetActive(false);
			}
		};
		UIEventListener.Get(this.ProgGameobjectEX.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (b)
			{
				this.EXPBarEX.uiLabel.gameObject.SetActive(true);
			}
			else
			{
				this.EXPBarEX.uiLabel.gameObject.SetActive(false);
			}
		};
	}

	public double Left
	{
		get
		{
			return Canvas.GetLeft(this);
		}
		set
		{
			Canvas.SetLeft(this, value);
		}
	}

	public double Top
	{
		get
		{
			return Canvas.GetTop(this);
		}
		set
		{
			Canvas.SetTop(this, value);
		}
	}

	public int ZIndex
	{
		get
		{
			return (int)Canvas.GetZIndex(this);
		}
		set
		{
			Canvas.SetZIndex(this, (double)value);
		}
	}

	public double ProgressTotalWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	public double ProgressWidth { get; set; }

	public string ProgressTip
	{
		set
		{
		}
	}

	public double BackWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	public void Init()
	{
	}

	public int RoleID
	{
		set
		{
		}
	}

	public GSpriteTypes SpriteType
	{
		set
		{
		}
	}

	public string ProgressText
	{
		set
		{
			if (null != this.ProgressValue)
			{
				this.ProgressValue.Text = value;
			}
		}
	}

	public double EXPPercent
	{
		set
		{
			this.EXPBar.Percent = value;
			this.ProgressText = string.Format("{0}%", (int)(value * 100.0));
		}
	}

	public double EXPPercentEX
	{
		set
		{
			this.EXPBarEX.Percent = value;
		}
	}

	public string ExpValue
	{
		set
		{
			this.EXPBar.ProgessText = value;
		}
	}

	public string ExpValueEX
	{
		set
		{
			this.EXPBarEX.ProgessText = value;
		}
	}

	public void RefreshTime()
	{
		if (null != this.TxtTimeNow)
		{
			this.TxtTimeNow.Text = Global.GetCorrectDateTime().ToLongTimeString();
		}
	}

	private void FixedUpdate()
	{
		if (this.state)
		{
			this.onceTicks += Time.fixedDeltaTime;
			if (this.onceTicks >= 10f)
			{
				this.showText(9999f);
			}
		}
	}

	private void showText(float ticks)
	{
		if (ticks <= 300f)
		{
			this.strColor = "17e43e";
		}
		else if (ticks >= 301f && ticks <= 800f)
		{
			this.strColor = "fac60d";
		}
		else if (ticks >= 801f)
		{
			this.strColor = "ff0000";
		}
		if (FPSCounter.showFps)
		{
			this.labPingValues.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.strColor,
				string.Format("PING: {0}ms FPS:{1}", ticks, FPSCounter.fps.ToString("f" + Mathf.Clamp(1, 0, 10)))
			});
		}
		else
		{
			this.labPingValues.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.strColor,
				string.Format("PING: {0}ms", ticks)
			});
		}
	}

	public void SendPing()
	{
		this.sendTick = Global.GetCorrectLocalTime();
		this.onceTicks = 0f;
		this.state = true;
	}

	public void ReceivePing()
	{
		this.state = false;
		this.serviceRespondTick = RoleExpProgressBar.receiveTick - this.sendTick;
		this.serviceRespondTick = ((this.serviceRespondTick <= 0L) ? 20L : this.serviceRespondTick);
		this.showText(20f);
	}

	public GImgProgressBar EXPBar;

	public GImgProgressBar EXPBarEX;

	public BoxCollider ProgGameobjectEX;

	public TextBlock ProgressValue;

	public BoxCollider ProgGameobject1;

	public GameObject ProgGameobject;

	public TextBlock TxtTimeNow;

	public TextBlock labPingValues;

	private bool state;

	private float onceTicks;

	private string strColor = string.Empty;

	public static long receiveTick;

	private long sendTick;

	private long serviceRespondTick = 20L;
}
