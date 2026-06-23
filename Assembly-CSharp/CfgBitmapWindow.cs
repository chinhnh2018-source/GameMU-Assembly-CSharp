using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;
using Tmsk.Xml;

public class CfgBitmapWindow : GBitmapWindow
{
	public CfgBitmapWindow(UserControl parent, int left = 0, int top = 0)
	{
		this.ParentCanvas = parent.Container;
		base.Left = (double)left;
		base.Top = (double)top;
		this.Z += 1.0;
		Canvas.SetZIndex(this, this.Z);
		this.ParentCanvas.Children.Add(this);
		this.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.OnClose();
			return true;
		};
	}

	protected virtual void OnClose()
	{
		this.ParentCanvas.Children.Remove(this, true);
	}

	public void TryAdjustPosition()
	{
		if (this.ActualRightSpace > 0)
		{
			Canvas.SetLeft(this, Global.GlobalMainWindow.ActualWidth - (double)this.ActualRightSpace);
		}
	}

	protected bool InitUIByXmlCfg(XElement xmlUI)
	{
		if (xmlUI == null)
		{
			return false;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xmlUI, "Left");
		if (xelementAttributeInt >= 0)
		{
			base.Left = (double)xelementAttributeInt;
		}
		else
		{
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xmlUI, "RightSpace");
			if (xelementAttributeInt2 > 0)
			{
				this.ActualRightSpace = xelementAttributeInt2;
				base.Left = (double)Math.Max((int)(Global.GlobalMainWindow.ActualWidth - (double)xelementAttributeInt2), 0);
			}
		}
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xmlUI, "Top");
		if (xelementAttributeInt3 >= 0)
		{
			base.Top = (double)xelementAttributeInt3;
		}
		base.BodyLeft = (double)Global.GetXElementAttributeInt(xmlUI, "BodyLeft");
		base.BodyTop = (double)Global.GetXElementAttributeInt(xmlUI, "BodyTop");
		base.BodyWidth = (double)Global.GetXElementAttributeInt(xmlUI, "BodyWidth");
		base.BodyHeight = (double)Global.GetXElementAttributeInt(xmlUI, "BodyHeight");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xmlUI, "WinBackgroundURL");
		if (!StringUtil.isWhitespace(xelementAttributeStr) && xelementAttributeStr.Length > 0)
		{
			base.WinBackgroundURL = new ImageURL(Global.GetGameResImageURL(xelementAttributeStr), false, 0);
		}
		base.CloseButtonWidth = (double)Global.GetXElementAttributeInt(xmlUI, "CloseButtonWidth");
		base.CloseButtonHeight = (double)Global.GetXElementAttributeInt(xmlUI, "CloseButtonHeight");
		base.CloseButtonLeft = (double)Global.GetXElementAttributeInt(xmlUI, "CloseButtonLeft");
		base.CloseButtonTop = (double)Global.GetXElementAttributeInt(xmlUI, "CloseButtonTop");
		xelementAttributeStr = Global.GetXElementAttributeStr(xmlUI, "CloseButtonFill");
		if (!StringUtil.isWhitespace(xelementAttributeStr) && xelementAttributeStr.Length > 0)
		{
			base.CloseButtonFill = Global.GetLoginResImage(xelementAttributeStr);
		}
		xelementAttributeStr = Global.GetXElementAttributeStr(xmlUI, "CloseButtonTransformFill");
		if (!StringUtil.isWhitespace(xelementAttributeStr) && xelementAttributeStr.Length > 0)
		{
			base.CloseButtonTransformFill = Global.GetLoginResImage(xelementAttributeStr);
		}
		return true;
	}

	public int ActualRightSpace = -1;

	protected Canvas ParentCanvas;
}
