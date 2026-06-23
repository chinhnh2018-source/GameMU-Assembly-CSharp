using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;

public class PopWinPart : UserControl
{
	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public static uint ToColor(string value)
	{
		value = Global.StringReplaceAll(value, "#", string.Empty);
		value = "0x" + value;
		return (uint)value.SafeToInt32(0);
	}

	public double realHeight
	{
		get
		{
			return (double)this._txtHeight;
		}
	}

	public void InitPartDataEx(XElement xml)
	{
		if (xml == null)
		{
			return;
		}
		try
		{
			this._url = Global.GetXElementAttributeStr(xml, "URL");
			List<XElement> xelementList = Global.GetXElementList(xml, "Item");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				TextBlock textBlock = new TextBlock();
				TextFormat textFormat = new TextFormat();
				textFormat.font = HSTextField.fontName;
				textFormat.color = PopWinPart.ToColor(Global.GetXElementAttributeStr(xelement, "color"));
				textFormat.size = Global.GetXElementAttributeInt(xelement, "fontsize");
				textFormat.underline = false;
				textFormat.bold = (Global.GetXElementAttributeStr(xelement, "fontweight") == "true");
				textBlock.Width = 220.0;
				textBlock.HorizontalAlignment = global::Layout.Center;
				textBlock.htmlText = Global.GetXElementAttributeStr(xelement, "text");
				textBlock.TextWrapping = true;
				textBlock.selectable = false;
				textBlock.TextFontWrapping = true;
				textBlock.setTextFormat(textFormat);
				textBlock.defaultTextFormat = textFormat;
				Canvas.SetLeft(textBlock, 5);
				Canvas.SetTop(textBlock, this._txtHeight);
				this.Container.Children.Add(textBlock);
				this._txtHeight += (int)textBlock.ActualHeight + 5;
				this.Container.buttonMode = true;
				this.Container.mouseChildren = false;
				this.Container.addEventListener("mouseUp", new MouseEventHandler(this.MouseUp));
				this.Container.addEventListener("mouseOver", new MouseEventHandler(this.HyperlinkButton_MouseEnter));
				this.Container.addEventListener("mouseOut", new MouseEventHandler(this.HyperlinkButton_MouseLeave));
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void MouseUp(MouseEvent evt)
	{
		this.navigateToURL(new URLRequest(this._url), "_Blank");
	}

	private void SetUnderline(bool b)
	{
		TextFormat textFormat = new TextFormat();
		textFormat.underline = b;
		for (int i = 0; i < this.Container.Children.numChildren; i++)
		{
			if (this.Container.getChildAt(i).SafeGetComponent<TextBlock>() is TextBlock)
			{
				TextBlock textBlock = this.Container.getChildAt(i).SafeGetComponent<TextBlock>();
				textBlock.setTextFormat(textFormat);
			}
		}
	}

	private void HyperlinkButton_MouseEnter(MouseEvent evt)
	{
		this.SetUnderline(true);
	}

	private void HyperlinkButton_MouseLeave(MouseEvent evt)
	{
		this.SetUnderline(false);
	}

	public void navigateToURL(URLRequest urlRequest, string target)
	{
	}

	private int _txtHeight = 5;

	private string _url = string.Empty;
}
