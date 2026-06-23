using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class SystemSettingTab1 : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "HideOtherRoles";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("屏蔽其他玩家造型显示");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.HideOtherRoles;
		Canvas.SetLeft(gcheckBox, 5);
		Canvas.SetTop(gcheckBox, 33);
		this.Container.Children.Add(gcheckBox);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("当您的机器非常卡的时候\n推荐您屏蔽其他玩家的造型");
		gtextBlockOutLine.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(gtextBlockOutLine, 21);
		Canvas.SetTop(gtextBlockOutLine, 50);
		this.Container.Children.Add(gtextBlockOutLine);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "HideChatPopupWin";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("屏蔽聊天泡泡显示");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.HideChatPopupWin;
		Canvas.SetLeft(gcheckBox, 5);
		Canvas.SetTop(gcheckBox, 106);
		this.Container.Children.Add(gcheckBox);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "HideTeamMembersFace";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("屏蔽组队队员信息窗口的显示");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.HideTeamMembersFace;
		Canvas.SetLeft(gcheckBox, 5);
		Canvas.SetTop(gcheckBox, 131);
		this.Container.Children.Add(gcheckBox);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "CloseGameMusic";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("关闭背景音乐");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.CloseGameMusic;
		Canvas.SetLeft(gcheckBox, 213);
		Canvas.SetTop(gcheckBox, 33);
		this.Container.Children.Add(gcheckBox);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("快捷开关键：Y");
		gtextBlockOutLine.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(gtextBlockOutLine, 229);
		Canvas.SetTop(gtextBlockOutLine, 50);
		this.Container.Children.Add(gtextBlockOutLine);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "CloseGameAudio";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("关闭动作音效");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.CloseGameAudio;
		Canvas.SetLeft(gcheckBox, 213);
		Canvas.SetTop(gcheckBox, 75);
		this.Container.Children.Add(gcheckBox);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("快捷开关键：CTRL+Y");
		gtextBlockOutLine.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(gtextBlockOutLine, 229);
		Canvas.SetTop(gtextBlockOutLine, 92);
		this.Container.Children.Add(gtextBlockOutLine);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "RefuseTeamRequest";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("拒绝组队邀请");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.RefuseTeamRequest;
		Canvas.SetLeft(gcheckBox, 213);
		Canvas.SetTop(gcheckBox, 117);
		this.Container.Children.Add(gcheckBox);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("拒绝后不再收到组队邀请提示");
		gtextBlockOutLine.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(gtextBlockOutLine, 229);
		Canvas.SetTop(gtextBlockOutLine, 134);
		this.Container.Children.Add(gtextBlockOutLine);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "RefusePrivateChat";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("拒绝他人向我发起私聊");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.RefusePrivateChat;
		Canvas.SetLeft(gcheckBox, 213);
		Canvas.SetTop(gcheckBox, 159);
		this.Container.Children.Add(gcheckBox);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("绝拒后不再收到私聊消息");
		gtextBlockOutLine.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(gtextBlockOutLine, 229);
		Canvas.SetTop(gtextBlockOutLine, 176);
		this.Container.Children.Add(gtextBlockOutLine);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "RefuseExchangeRequest";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("拒绝他人向我发起交易");
		gcheckBox.TextColor = new SolidColorBrush(11394222U);
		gcheckBox.Check = Global.Data.SysSetting.RefuseExchangeRequest;
		Canvas.SetLeft(gcheckBox, 213);
		Canvas.SetTop(gcheckBox, 201);
		this.Container.Children.Add(gcheckBox);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = Global.GetLang("绝拒后不再收到交易提示");
		gtextBlockOutLine.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(gtextBlockOutLine, 229);
		Canvas.SetTop(gtextBlockOutLine, 218);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void SaveSystemSettings()
	{
		Global.Data.SysSetting.HideOtherRoles = U3DUtils.AS<GCheckBox>(this.Container.FindName("HideOtherRoles")).Check;
		Global.Data.SysSetting.HideChatPopupWin = U3DUtils.AS<GCheckBox>(this.Container.FindName("HideChatPopupWin")).Check;
		Global.Data.SysSetting.HideTeamMembersFace = U3DUtils.AS<GCheckBox>(this.Container.FindName("HideTeamMembersFace")).Check;
		Global.Data.SysSetting.RefuseTeamRequest = U3DUtils.AS<GCheckBox>(this.Container.FindName("RefuseTeamRequest")).Check;
		Global.Data.SysSetting.RefusePrivateChat = U3DUtils.AS<GCheckBox>(this.Container.FindName("RefusePrivateChat")).Check;
		Global.Data.SysSetting.RefuseExchangeRequest = U3DUtils.AS<GCheckBox>(this.Container.FindName("RefuseExchangeRequest")).Check;
		Global.Data.SysSetting.CloseGameMusic = U3DUtils.AS<GCheckBox>(this.Container.FindName("CloseGameMusic")).Check;
		Global.Data.SysSetting.CloseGameAudio = U3DUtils.AS<GCheckBox>(this.Container.FindName("CloseGameAudio")).Check;
	}

	public void LoadSystemSettings()
	{
		U3DUtils.AS<GCheckBox>(this.Container.FindName("HideOtherRoles")).Check = Global.Data.SysSetting.HideOtherRoles;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("HideChatPopupWin")).Check = Global.Data.SysSetting.HideChatPopupWin;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("HideTeamMembersFace")).Check = Global.Data.SysSetting.HideTeamMembersFace;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("RefuseTeamRequest")).Check = Global.Data.SysSetting.RefuseTeamRequest;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("RefusePrivateChat")).Check = Global.Data.SysSetting.RefusePrivateChat;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("RefuseExchangeRequest")).Check = Global.Data.SysSetting.RefuseExchangeRequest;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("CloseGameMusic")).Check = Global.Data.SysSetting.CloseGameMusic;
		U3DUtils.AS<GCheckBox>(this.Container.FindName("CloseGameAudio")).Check = Global.Data.SysSetting.CloseGameAudio;
	}
}
