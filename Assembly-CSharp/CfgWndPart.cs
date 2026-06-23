using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;

public class CfgWndPart : UserControl
{
	public CfgWndPart(XElement xmlUI, GBitmapWindow owner)
	{
		this.MyOwner = owner;
		this.InitUI(xmlUI);
	}

	protected bool InitUI(XElement xmlUI)
	{
		this.Width = (double)Global.GetXElementAttributeInt(xmlUI, "BodyWidth");
		this.Height = (double)Global.GetXElementAttributeInt(xmlUI, "BodyHeight");
		this.Container.Width = base.width;
		this.Container.Height = base.height;
		List<XElement> xelementList = Global.GetXElementList(xmlUI, "Item");
		if (xelementList == null)
		{
			return false;
		}
		double num = 0.0;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if ("text" == Global.GetXElementAttributeStr(xelement, "Type").ToLower())
			{
				CfgTextBlock cfgTextBlock = U3DUtils.NEW<CfgTextBlock>();
				if (Canvas.GetTop(cfgTextBlock) < 0.0)
				{
					Canvas.SetTop(cfgTextBlock, num);
				}
				num += cfgTextBlock.RealSize.Height;
				this.Container.Children.Add(cfgTextBlock);
				cfgTextBlock.IsHitTestVisible = false;
			}
			else if ("goods" == Global.GetXElementAttributeStr(xelement, "Type").ToLower())
			{
				CfgGoodsItemList cfgGoodsItemList = U3DUtils.NEW<CfgGoodsItemList>();
				if (Canvas.GetTop(cfgGoodsItemList) < 0.0)
				{
					Canvas.SetTop(cfgGoodsItemList, num);
				}
				num += cfgGoodsItemList.Height;
				this.Container.Children.Add(cfgGoodsItemList);
			}
			else if ("button" == Global.GetXElementAttributeStr(xelement, "Type").ToLower())
			{
				CfgButton cfgButton = U3DUtils.NEW<CfgButton>();
				if (Canvas.GetTop(cfgButton) < 0.0)
				{
					Canvas.SetTop(cfgButton, num);
				}
				num += cfgButton.Height;
				this.Container.Children.Add(cfgButton);
				cfgButton.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnClick);
			}
		}
		return true;
	}

	protected void OnBtnClick(object sender, MouseEvent e)
	{
		if (this.OnAnyBtnClick != null)
		{
			CfgButton cfgButton = sender as CfgButton;
			if (null != cfgButton)
			{
				cfgButton.EnableHint = false;
			}
			this.OnAnyBtnClick(sender, e);
		}
	}

	public MouseLeftButtonUpEventHandler OnAnyBtnClick;

	private GBitmapWindow MyOwner;
}
