using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SystemNaviBox : UserControl
{
	protected override void InitializeComponent()
	{
		this.IsHitTestVisible = false;
	}

	public double CenterX
	{
		get
		{
			return this._CenterX;
		}
		set
		{
			this._CenterX = value;
		}
	}

	public double CenterY
	{
		get
		{
			return this._CenterY;
		}
		set
		{
			this._CenterY = value;
		}
	}

	public string Text
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public int DelayTicks
	{
		get
		{
			return this._DelayTicks;
		}
		set
		{
			this._DelayTicks = value;
			if (this._DelayTicks > 0)
			{
				if (this.DelayTimer != null)
				{
					this.DelayTimer.Tick = null;
					this.DelayTimer.Stop();
					this.DelayTimer = null;
				}
				this.DelayTimer = new DispatcherTimer("SystemNaviBox_DelayTimer");
				this.DelayTimer.Interval = TimeSpan.FromMilliseconds((double)this._DelayTicks);
				this.DelayTimer.Tick = new DispatcherTimerEventHandler(this.DelayTimer_Tick);
				this.DelayTimer.Start();
			}
			else if (this.DelayTimer != null)
			{
				this.DelayTimer.Tick = null;
				this.DelayTimer.Stop();
				this.DelayTimer = null;
			}
		}
	}

	public int DecoCode
	{
		get
		{
			return this._DecoCode;
		}
		set
		{
			this._DecoCode = value;
			if (this._DecoCode >= 0)
			{
				base.InitHintDecoration(this._DecoCode, new Point(0, 0), null);
			}
			else
			{
				this.ClearHintDecoration();
			}
		}
	}

	private void DelayTimer_Tick(object sender, object e)
	{
		Super.RemoveSystemNaviBoxByName(null, this.Name, null);
		if (this.DelayTimer != null)
		{
			this.DelayTimer.Tick = null;
			this.DelayTimer.Stop();
			this.DelayTimer = null;
		}
	}

	public override void Destroy()
	{
		this.ClearHintDecoration();
		Object.Destroy(base.gameObject);
	}

	private int _DelayTicks;

	private DispatcherTimer DelayTimer;

	private double _CenterX;

	private double _CenterY;

	private int _DecoCode = -1;
}
