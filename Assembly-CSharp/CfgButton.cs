using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class CfgButton : GIcon
{
	public CfgButton(XElement node) : base(0, -1)
	{
	}

	public string TagKey
	{
		get
		{
			return this._TagKey;
		}
		set
		{
			this._TagKey = value;
		}
	}

	public string BindValue
	{
		get
		{
			return this._BindValue;
		}
		set
		{
			this._BindValue = value;
		}
	}

	public void Init(XElement node)
	{
		base.InitIcon(1, 0);
		this.InitUI(node);
	}

	protected void InitUI(XElement node)
	{
		this.ImageSource = Global.GetXElementAttributeInt(node, "ImageSource");
		if (this.ImageSource <= 0)
		{
			this.ImageSource = 2;
		}
		Canvas.SetLeft(this, Global.GetXElementAttributeInt(node, "Left"));
		Canvas.SetTop(this, Global.GetXElementAttributeInt(node, "Top"));
		base.Width = (double)Global.GetXElementAttributeInt(node, "Width");
		base.Height = (double)Global.GetXElementAttributeInt(node, "Height");
		this.SetBodySource(Global.GetXElementAttributeStr(node, "BodySource"));
		this.SetNewSource(Global.GetXElementAttributeStr(node, "NewSource"));
		this.SetEnableHint(Global.GetXElementAttributeStr(node, "EnableHint"));
		this._TagKey = Global.GetXElementAttributeStr(node, "Tag");
		this._BindValue = Global.GetXElementAttributeStr(node, "BindValue");
		string xelementAttributeStr = Global.GetXElementAttributeStr(node, "Text");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			base.Text = xelementAttributeStr;
		}
		xelementAttributeStr = Global.GetXElementAttributeStr(node, "TextColor");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			base.TextColor = Super.ParseStringColor(xelementAttributeStr);
		}
		base.TipType = Global.GetXElementAttributeInt(node, "TipType");
		base.Tip = Global.GetXElementAttributeStr(node, "Tip");
	}

	private void SetEnableHint(string strValue)
	{
		if ("true" == strValue.ToLowerInvariant())
		{
			base.HintDecoType = 8;
		}
		else
		{
			base.HintDecoType = -1;
		}
	}

	private void SetBodySource(string strValue)
	{
		if (!string.IsNullOrEmpty(strValue))
		{
			base.BodySource = new ImageBrush(this.GetResImage(strValue));
		}
	}

	private void SetNewSource(string strValue)
	{
		if (!string.IsNullOrEmpty(strValue))
		{
			base.NewSource = new ImageBrush(this.GetResImage(strValue));
		}
	}

	private BitmapData GetResImage(string strValue)
	{
		if (this.ImageSource == 1)
		{
			return Global.GetGameResImage(strValue);
		}
		return Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage(strValue), 80.0, 21.0, 3.0, 2.0);
	}

	private string _TagKey = string.Empty;

	private string _BindValue = string.Empty;

	private int ImageSource = 1;
}
