using System;
using System.Text.RegularExpressions;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SetSecondPasswordPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetLang("设置密码");
		this.ConfirmIcon.Text = Global.GetLang("确定");
		this.NewPassword.text = Global.GetLang("新二级密码");
		this.NextNewPassword.text = Global.GetLang("再次输入二级密码");
		this.Hint.text = Global.GetLang("用于登陆角色、删除角色时使用");
		this.InputPassword.X = 80;
		this.NextInputPassword.X = 80;
		this.NewPassword.pivot = 5;
		this.NewPassword.transform.localPosition = new Vector3(-90f, 0f, -0.1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.NextInputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.SubmitNewSecondPassword);
		this.Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).CloseSetSecondPasswordWindow();
		};
	}

	private void PasswordLostFocus(object sender, EventArgs e)
	{
		this.InputPassword.label.password = true;
		this.NextInputPassword.label.password = true;
	}

	private void SubmitNewSecondPassword(object sender, MouseEvent e)
	{
		if (string.IsNullOrEmpty(this.InputPassword.text) || string.IsNullOrEmpty(this.NextInputPassword.text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("您未输入二级密码"), -1, -1, -1, -1, false);
			return;
		}
		if (!Regex.IsMatch(this.InputPassword.text, "^[a-zA-Z0-9_]+$") || !Regex.IsMatch(this.NextInputPassword.text, "^[a-zA-Z0-9_]+$"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码只能由英文、数字、下划线组成"), -1, -1, -1, -1, false);
			return;
		}
		if (this.InputPassword.text.Length < 6 || this.InputPassword.text.Length > 8 || this.NextInputPassword.text.Length < 6 || this.NextInputPassword.text.Length > 8)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码需要6-8个字符"), -1, -1, -1, -1, false);
			return;
		}
		if (this.InputPassword.text != this.NextInputPassword.text)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("两次输入的密码不一致，请重新输入"), -1, -1, -1, -1, false);
			return;
		}
		SetSecondPassword setSecondPassword = new SetSecondPassword
		{
			RoleID = Global.Data.roleData.RoleID,
			NewSecPwd = SecondPasswordRC4.Encrypt(this.InputPassword.text)
		};
		byte[] secondPassword = DataHelper.ObjectToBytes<SetSecondPassword>(setSecondPassword);
		GameInstance.Game.SetSecondPassword(secondPassword);
	}

	public TextBox InputPassword;

	public TextBox NextInputPassword;

	public GButton ConfirmIcon;

	public GButton Close;

	public UILabel Title;

	public UILabel NewPassword;

	public UILabel NextNewPassword;

	public UILabel Hint;
}
