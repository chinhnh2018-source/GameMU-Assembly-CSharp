using System;
using System.Text.RegularExpressions;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class VerifySecondPasswordPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetLang("二级密码");
		this.ConfirmIcon.Text = Global.GetLang("确定");
		this.Label.text = Global.GetLang("请输入二级密码");
		this.Hint.text = Global.GetLang("如忘记密码，请联系平台客服进行找回");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.SubmitVerifySecondPassword);
		this.Close.MouseLeftButtonUp = delegate(object sender, MouseEvent o)
		{
			Super.CloseVerifySecondPasswordWindow();
		};
	}

	private void PasswordLostFocus(object sender, EventArgs e)
	{
		this.InputPassword.label.password = true;
	}

	private void SubmitVerifySecondPassword(object sender, MouseEvent e)
	{
		if (string.IsNullOrEmpty(this.InputPassword.text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("您未输入二级密码"), -1, -1, -1, -1, false);
			return;
		}
		if (!Regex.IsMatch(this.InputPassword.text, "^[a-zA-Z0-9_]+$"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码只能由英文、数字、下划线组成"), -1, -1, -1, -1, false);
			return;
		}
		if (this.InputPassword.text.Length < 6 || this.InputPassword.text.Length > 8)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码需要6-8个字符"), -1, -1, -1, -1, false);
			return;
		}
		VerifySecondPassword verifySecondPassword = new VerifySecondPassword
		{
			UserID = GameInstance.Game.CurrentSession.UserID,
			SecPwd = SecondPasswordRC4.Encrypt(this.InputPassword.text)
		};
		byte[] data = DataHelper.ObjectToBytes<VerifySecondPassword>(verifySecondPassword);
		GameInstance.Game.VerifySecondPassword(data);
	}

	public TextBox InputPassword;

	public GButton ConfirmIcon;

	public GButton Close;

	public UILabel Title;

	public UILabel Label;

	public UILabel Hint;

	[HideInInspector]
	public int VerifiedRoleID;
}
