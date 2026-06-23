using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class PopWin : UserControl
{
	public void InitPartSizeEx(XElement xml)
	{
		PopWinPart popWinPart = U3DUtils.NEW<PopWinPart>();
		popWinPart.BackgroundColor = 1845296U;
		popWinPart.BackgroundAlpha = 0.9;
		popWinPart.InitPartDataEx(xml);
		int num = (int)popWinPart.realHeight + 30 + 9;
		this.PopWindow.Left = 0.0;
		this.PopWindow.Top = 0.0;
		this.PopWindow.HeadLeft = 0.0;
		this.PopWindow.HeadTop = 0.0;
		this.PopWindow.HeadWidth = 253.0;
		this.PopWindow.HeadHeight = 27.0;
		this.PopWindow.BodyLeft = 0.0;
		this.PopWindow.BodyTop = 27.0;
		this.PopWindow.BodyWidth = 253.0;
		this.PopWindow.BodyHeight = (double)((num <= 143) ? 143 : num);
		Super.InitChildWindow2(this.PopWindow, Global.GetLang("温馨提示"));
		popWinPart.InitPartSize((int)this.PopWindow.BodyWidth - 18, (int)this.PopWindow.BodyHeight - 9);
		this.PopWindow.SetContent(this.PopWindow.BodyPresenter, popWinPart, 9.0, 0.0, true);
		this.Container.Children.Add(this.PopWindow);
		this.Container.Height = this.PopWindow.BodyHeight + this.PopWindow.HeadHeight;
		this.Container.Width = this.PopWindow.BodyWidth;
	}

	public void InitPartData()
	{
		Canvas.SetTop(this.PopWindow, 0);
	}

	private GChildWindow PopWindow = U3DUtils.NEW<GChildWindow>();
}
