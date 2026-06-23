using System;
using System.Text.RegularExpressions;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChangeSecondPasswordPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetLang("修改密码");
		this.ConfirmIcon.Text = Global.GetLang("确定");
		this.OldPassword.text = Global.GetLang("原二级密码");
		this.NewPassword.text = Global.GetLang("新二级密码");
		this.NextNewPassword.text = Global.GetLang("再次输入二级密码");
		this.Hint.text = Global.GetLang("请牢记自己的二级密码,并且不要将二级密码透露给任何人");
		this.OldPassword.pivot = 5;
		this.NewPassword.pivot = 5;
		this.NextNewPassword.pivot = 5;
		this.OldPassword.transform.localPosition = new Vector3(-90f, this.OldPassword.transform.localPosition.y, this.OldPassword.transform.localPosition.z);
		this.NewPassword.transform.localPosition = new Vector3(-90f, this.NewPassword.transform.localPosition.y, this.NewPassword.transform.localPosition.z);
		this.NextNewPassword.transform.localPosition = new Vector3(-90f, this.NextNewPassword.transform.localPosition.y, this.NextNewPassword.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OldInputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.NewInputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.NextNewInputPassword.LostFocus = new EventHandler(this.PasswordLostFocus);
		this.ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.SubmitNewSecondPassword);
		this.Close.MouseLeftButtonUp = delegate(object sender, MouseEvent o)
		{
			(Super.GData.PlayZoneRoot as PlayZone).CloseChangeSecondPasswordWindow();
		};
	}

	private void PasswordLostFocus(object sender, EventArgs e)
	{
		this.OldInputPassword.label.password = true;
		this.NewInputPassword.label.password = true;
		this.NextNewInputPassword.label.password = true;
	}

	private void SubmitNewSecondPassword(object sender, MouseEvent e)
	{
		if (string.IsNullOrEmpty(this.OldInputPassword.text) || string.IsNullOrEmpty(this.NewInputPassword.text) || string.IsNullOrEmpty(this.NextNewInputPassword.text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("您未输入二级密码"), -1, -1, -1, -1, false);
			return;
		}
		if (!Regex.IsMatch(this.OldInputPassword.text, "^[a-zA-Z0-9_]+$") || !Regex.IsMatch(this.NewInputPassword.text, "^[a-zA-Z0-9_]+$") || !Regex.IsMatch(this.NextNewInputPassword.text, "^[a-zA-Z0-9_]+$"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码只能由英文、数字、下划线组成"), -1, -1, -1, -1, false);
			return;
		}
		if (this.OldInputPassword.text.Length < 6 || this.OldInputPassword.text.Length > 8 || this.NewInputPassword.text.Length < 6 || this.NewInputPassword.text.Length > 8 || this.NextNewInputPassword.text.Length < 6 || this.NextNewInputPassword.text.Length > 8)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码需要6-8个字符"), -1, -1, -1, -1, false);
			return;
		}
		if (this.NewInputPassword.text != this.NextNewInputPassword.text)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("两次输入的密码不一致，请重新输入"), -1, -1, -1, -1, false);
			return;
		}
		SetSecondPassword setSecondPassword = new SetSecondPassword
		{
			RoleID = Global.Data.roleData.RoleID,
			OldSecPwd = SecondPasswordRC4.Encrypt(this.OldInputPassword.text),
			NewSecPwd = SecondPasswordRC4.Encrypt(this.NewInputPassword.text)
		};
		byte[] secondPassword = DataHelper.ObjectToBytes<SetSecondPassword>(setSecondPassword);
		GameInstance.Game.SetSecondPassword(secondPassword);
	}

	public TextBox OldInputPassword;

	public TextBox NewInputPassword;

	public TextBox NextNewInputPassword;

	public GButton ConfirmIcon;

	public GButton Close;

	public UILabel Title;

	public UILabel OldPassword;

	public UILabel NewPassword;

	public UILabel NextNewPassword;

	public UILabel Hint;
}
