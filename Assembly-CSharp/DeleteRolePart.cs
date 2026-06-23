using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DeleteRolePart : UserControl
{
	private void OnBtnSureClick(object sender, MouseEvent e)
	{
		this.ItemEventHandler(this, new DPSelectedItemEventArgs
		{
			ID = 0
		});
	}

	private void OnBtnCancelClick(object sender, MouseEvent e)
	{
		this.ItemEventHandler(this, new DPSelectedItemEventArgs
		{
			ID = 1
		});
	}

	private void InitTextInPrefabs()
	{
		this.m_BtnSure.Text = Global.GetLang("确定");
		this.m_BtnCancel.Text = Global.GetLang("取消");
		this.m_InPut.text = Global.GetLang("请输入验证码");
		this.m_InPut.defaultText = Global.GetLang("请输入验证码");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ShowYanZhengMa();
		this.InitCtrl();
	}

	private void InitCtrl()
	{
		if (null != this.m_BtnSure)
		{
			this.m_BtnSure.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnSureClick);
		}
		if (null != this.m_BtnCancel)
		{
			this.m_BtnCancel.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnCancelClick);
		}
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnCancelClick);
		}
	}

	private void ShowYanZhengMa()
	{
		if (null != this.m_LblYanZhengMa)
		{
			string text = this.MakeYanZhengMa();
			if (text == string.Empty)
			{
				this.m_LblYanZhengMa.text = "3A4B";
			}
			else
			{
				this.m_LblYanZhengMa.text = text;
			}
		}
	}

	private string MakeYanZhengMa()
	{
		string text = "ABCDEFGHJKMNPQRSTUXYZ23456789";
		string text2 = string.Empty;
		int length = text.Length;
		for (int i = 0; i < 4; i++)
		{
			int num = Random.Range(1, length);
			if (0 < num)
			{
				string text3 = text.Substring(num, 1);
				text2 += text3;
			}
		}
		return text2;
	}

	public RoleInfo m_RoleInfo;

	public UILabel m_LblYanZhengMa;

	public UIInput m_InPut;

	public GButton m_BtnSure;

	public GButton m_BtnCancel;

	public GButton m_BtnClose;

	public UILabel m_RoleName;

	public UILabel m_RoleOccups;

	public UILabel m_RolLevel;

	public DPSelectedItemEventHandler ItemEventHandler;
}
