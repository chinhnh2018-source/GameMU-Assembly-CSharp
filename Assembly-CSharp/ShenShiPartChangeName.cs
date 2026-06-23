using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShenShiPartChangeName : UserControl
{
	public new string Name
	{
		set
		{
			this.input.Text = Global.GetLang(value);
		}
	}

	public int TabID
	{
		get
		{
			return this.tabid;
		}
		set
		{
			this.tabid = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("改名")
		});
		this.miaoshu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("符文页名字最多5个字")
		});
		this.Btnsure.Label.text = Global.GetLang("确定");
		this.Btnquxiao.Label.text = Global.GetLang("取消");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Btnclose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.Btnsure.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ConfirmIcon_MouseLeftButtonUp);
		this.Btnquxiao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	private void ConfirmIcon_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.newName = this.input.text;
		if (Global.IncludeReplaceFilterFileds(this.newName))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的符文页名称当中含有敏感词汇，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (this.newName.IndexOf(" ") != -1)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,符文页名称当中不允许包含空格，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		this.newName = Global.StringReplaceAll(this.newName, "'", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, "|", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, "$", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, ":", string.Empty);
		this.newName = Global.ReplaceFilterFileds(this.newName);
		if (this.newName.Contains("{") || this.newName.Contains("}"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的符文页名称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (string.IsNullOrEmpty(this.newName) || Global.StringTrim(this.newName) == string.Empty)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,请输入符文页名称!"), -1, -1, -1, -1, false);
			return;
		}
		GameInstance.Game.GetFuWenChangeName(this.TabID, this.newName);
		Super.ShowNetWaiting(null);
		this.CloseHandler(this, new DPSelectedItemEventArgs());
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel title;

	public UILabel miaoshu;

	public TextBox input;

	public GButton Btnclose;

	public GButton Btnsure;

	public GButton Btnquxiao;

	private string newName;

	private int tabid;
}
