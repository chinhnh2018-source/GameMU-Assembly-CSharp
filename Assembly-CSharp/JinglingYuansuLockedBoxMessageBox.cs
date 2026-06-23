using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class JinglingYuansuLockedBoxMessageBox : UserControl
{
	public int BoxType
	{
		set
		{
		}
	}

	public string HintTitle
	{
		set
		{
			if (null != this.HintTitle_Label)
			{
				this.HintTitle_Label.text = string.Format(Global.GetLang("{0}"), value);
			}
		}
	}

	public string HintText
	{
		set
		{
			if (null != this.HintText_Label)
			{
				this.HintText_Label.text = value;
			}
		}
	}

	public int DefaultReturn { get; set; }

	public int MaxTime { get; set; }

	public int MyMessageBoxPartReturn
	{
		get
		{
			return this._MyMessageBoxPartReturn;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.OkBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkBtn_MouseLeftButtonUp);
		this.CancelBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
		this.CloseBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CloseBtn_MouseLeftButtonUp);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void InitPartData(string caption, string message, int defaultMoney, float delay, params string[] buttons)
	{
		this.HintTitle = caption;
		this.HintText = message;
		this.CostDiamond.text = defaultMoney.ToString();
		if (delay > 0f)
		{
			this.EndTicks = Global.GetCorrectLocalTime() + (long)((int)(delay * 1000f));
			base.InvokeRepeating("Timer_Tick", 0f, 0.12f);
		}
		else
		{
			delay = 0f;
		}
		if (buttons.Length == 1)
		{
			this.OkBtn.Text = buttons[0];
			Vector3 localPosition = this.OkBtn.transform.localPosition;
			localPosition.x = 0f;
			this.OkBtn.transform.localPosition = localPosition;
			this.CancelBtn.gameObject.SetActive(false);
		}
		if (buttons.Length == 2)
		{
			this.OkBtn.Text = buttons[0];
			this.CancelBtn.Text = buttons[1];
			Vector3 localPosition2 = this.OkBtn.transform.localPosition;
			localPosition2.x = -75f;
			this.OkBtn.transform.localPosition = localPosition2;
			this.CancelBtn.gameObject.SetActive(true);
		}
	}

	private void OkBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 0;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	private void CancelBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = -1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	private void Timer_Tick()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime < this.EndTicks)
		{
			this.HintText2_Label.text = string.Format(Global.GetLang("倒计时: {{ff0000}}{0}秒{{-}}"), Mathf.CeilToInt((float)(this.EndTicks - correctLocalTime) / 1000f));
		}
		else if (this.EndTicks != 0L)
		{
			this.EndTicks = 0L;
			this.HintText2_Label.text = string.Format(Global.GetLang("倒计时: {{ff0000}}{0}秒{{-}}"), 0);
			this.CloseBtn_MouseLeftButtonUp(this, MouseEvent.Empty);
		}
	}

	public UISprite _Bak;

	public UILabel HintText_Label;

	public UILabel HintText2_Label;

	public UILabel HintTitle_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public GButton CloseBtn;

	public UILabel CostDiamond;

	public EventHandler ButtonClick;

	private int _MyMessageBoxPartReturn = -1;

	private long EndTicks;
}
