using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class MyMessageBoxExPart : UserControl
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
				this.HintTitle_Label.text = value;
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

	private void InitTextInPrefabs()
	{
		this.OkBtn.Text = Global.GetLang("确定");
		this.CancelBtn.Text = Global.GetLang("取消");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OkBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkBtn_MouseLeftButtonUp);
		this.CancelBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
		this.CloseBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CloseBtn_MouseLeftButtonUp);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void InitPartData(string caption, string message, params string[] buttons)
	{
		this.HintTitle = caption;
		this.HintText = message;
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

	public void SetCloseBtnEnable(bool show)
	{
		this.CloseBtn.gameObject.SetActive(show);
	}

	public void RefreshMessage(string msg)
	{
		this.HintText = msg;
	}

	public UISprite _Bak;

	public UILabel HintText_Label;

	public UILabel HintTitle_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public GButton CloseBtn;

	public EventHandler ButtonClick;

	private int _MyMessageBoxPartReturn = -1;
}
