using System;
using System.Collections;
using System.Runtime.InteropServices;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MySettingLockScreenMessageBoxPart : UserControl
{
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

	public int MyMessageBoxPartReturn
	{
		get
		{
			return this._MyMessageBoxPartReturn;
		}
	}

	private void InitTextInPrefabs()
	{
		this.OkBtn.Text = Global.GetLang("立即设置");
		this.CancelBtn.Text = Global.GetLang("取消");
		this.CheckBox.Text = Global.GetLang("本次登录不再提示");
		this.HintText_Label1.Text = Global.GetLang("应用流畅模式\r\n提升流畅度");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.CountDown();
		this.OkBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkBtn_MouseLeftButtonUp);
		this.CancelBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
		this.CloseBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void Update()
	{
		base.Update();
		if (Super.platformLogin != null && Super.platformLogin.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OkBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			Global.Data.SysSetting.ScreenLockSize = true;
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

	public static void ShowSettingScreenSizeTip()
	{
		if (!Global.Data.SysSetting.ScreenLockSizeWindowFinish)
		{
			Super.CloseChildWindow(Super.MainWindowRoot, "settingLockScreenMessage");
			MySettingLockScreenMessageBoxPart.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang(string.Empty), Global.GetLang("应用流畅模式\r\n提升流畅度"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			Global.Data.SysSetting.ScreenLockSizeWindowFinish = true;
		}
	}

	private static GChildWindow ShowMessageBox(Canvas root, int boxType, string caption, string message, int left = -1, int top = -1, int width = -1, int height = -1, double opacity = 0.7, [Optional] Vector3 pos, MouseLeftButtonUpEventHandler RegOkCallBack = null, string OkButtonText = null)
	{
		GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
		MySettingLockScreenMessageBoxPart mySettingLockScreenMessageBoxPart = U3DUtils.NEW<MySettingLockScreenMessageBoxPart>();
		Super.InitChildWindow(messageBoxWindow, "settingLockScreenMessage");
		messageBoxWindow.ModalType = ChildWindowModalType.None;
		mySettingLockScreenMessageBoxPart.HintTitle = caption;
		mySettingLockScreenMessageBoxPart.ButtonClick = delegate(object s, EventArgs e)
		{
			Global.SaveSystemSettings();
			Super.CloseChildWindow(root, messageBoxWindow);
		};
		GButton okBtn = mySettingLockScreenMessageBoxPart.OkBtn;
		okBtn.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(okBtn.MouseLeftButtonUp, delegate(object o, MouseEvent e)
		{
			Super.CloseChildWindow(root, messageBoxWindow);
		});
		messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, mySettingLockScreenMessageBoxPart, 9.0, 0.0, true);
		root.Children.Add(messageBoxWindow);
		mySettingLockScreenMessageBoxPart.transform.localPosition = new Vector3(135f, 0f, 0f);
		return messageBoxWindow;
	}

	private void CountDown()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	private IEnumerator TickProc()
	{
		for (;;)
		{
			if (this.count < 1)
			{
				Global.Data.SysSetting.ScreenLockSize = true;
				this._MyMessageBoxPartReturn = 0;
				this.ButtonClick.Invoke(this, EventArgs.Empty);
				base.StopCoroutine("TickProc");
			}
			this.count--;
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public UILabel HintTitle_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public GButton CloseBtn;

	public GCheckBox CheckBox;

	public TextBlock HintText_Label1;

	public TextBlock HintText_Label2;

	public EventHandler ButtonClick;

	private int _MyMessageBoxPartReturn = -1;

	private int count = 60;
}
