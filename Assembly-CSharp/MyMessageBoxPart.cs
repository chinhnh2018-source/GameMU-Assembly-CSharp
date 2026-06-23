using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class MyMessageBoxPart : UserControl
{
	public int BoxType
	{
		set
		{
			if (value == 0)
			{
				this.CheckBox.gameObject.SetActive(false);
				this.CancelBtn.gameObject.SetActive(false);
				this.OkBtn.gameObject.SetActive(true);
				this.OkBtn.gameObject.transform.localPosition = this.CancelBtn.transform.localPosition;
			}
			else if (value == 1)
			{
				this.CheckBox.gameObject.SetActive(false);
				this.OkBtn.gameObject.SetActive(true);
				this.CancelBtn.gameObject.SetActive(true);
			}
			else if (value == 2)
			{
				this.CheckBox.gameObject.SetActive(true);
				this.OkBtn.gameObject.SetActive(true);
				this.CancelBtn.gameObject.SetActive(true);
			}
		}
	}

	public string HintTitle
	{
		set
		{
			if (null != this.HintTitle_Label)
			{
				this.HintTitle_Label.text = Global.GetLang(value);
			}
		}
	}

	public string HintText
	{
		set
		{
			if (null != this.HintText_Label)
			{
				this.HintText_Label.text = Global.GetLang(value);
			}
		}
	}

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
		this.CheckBox.Text = Global.GetLang("本次登录不再提示");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OkBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkBtn_MouseLeftButtonUp);
		this.CancelBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
		this.CloseBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
	}

	public override void Destroy()
	{
		base.Destroy();
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

	public TextBlock HintText_Label;

	public UILabel HintTitle_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public GButton CloseBtn;

	public GCheckBox CheckBox;

	public EventHandler ButtonClick;

	private int _MyMessageBoxPartReturn = -1;
}
