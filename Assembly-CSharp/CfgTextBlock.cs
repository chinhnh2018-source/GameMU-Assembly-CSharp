using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class CfgTextBlock : GTextBlockOutLine
{
	public CfgTextBlock(XElement nodeCfg = null, string _text = "", int fontcolor = -1, int bgcolor = -1, int border = -1, int fontsize = -1) : base(_text, fontcolor, bgcolor, border, fontsize, 0)
	{
		if (nodeCfg != null)
		{
			this.InitTextCfgInfo(nodeCfg);
		}
	}

	protected bool InitTextCfgInfo(XElement node)
	{
		string xelementAttributeStr = Global.GetXElementAttributeStr(node, "Color");
		base.TextColor = Super.ParseStringColor(xelementAttributeStr);
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "Underline");
		this.SetUnderLine(xelementAttributeStr);
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "Tag");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			this.Tag = xelementAttributeStr;
		}
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "Text");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			base.Text = xelementAttributeStr;
		}
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "TextWrap");
		this.SetWrap(xelementAttributeStr);
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "FontSize");
		this.SetFont(xelementAttributeStr);
		Canvas.SetLeft(this, Global.GetXElementAttributeInt(node, "Left"));
		Canvas.SetTop(this, Global.GetXElementAttributeInt(node, "Top"));
		this.Width = (double)Global.GetXElementAttributeInt(node, "Width");
		this.Height = (double)Global.GetXElementAttributeInt(node, "Height");
		return true;
	}

	private void SetUnderLine(string strValue)
	{
		if ("true" == strValue.ToLowerInvariant())
		{
			base.underLine = true;
		}
		else
		{
			base.underLine = false;
		}
	}

	private void SetWrap(string strValue)
	{
		if ("true" == strValue.ToLowerInvariant())
		{
			base.TextFontWrapping = HSGameEngine.GameEngine.SilverLight.TextWrapping.Wrap;
		}
		else
		{
			base.TextFontWrapping = HSGameEngine.GameEngine.SilverLight.TextWrapping.NoWrap;
		}
	}

	private void SetFont(string strValue)
	{
		if (!string.IsNullOrEmpty(strValue))
		{
			int num = Global.TryGetInt(strValue);
			if (num != 268435455)
			{
				base.FontSize = num;
			}
			else
			{
				base.FontSize = HSTextField.defaultFontSize;
			}
		}
	}
}
