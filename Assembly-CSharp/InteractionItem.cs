using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class InteractionItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.ItemButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickHandler);
	}

	public void OnClickHandler(GameObject ob)
	{
		if (this.DPSelectedItem != null && this.IsVIPOpen)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				Title = this.ActionName
			});
		}
	}

	public void RefreshItem(XElement xmlItem)
	{
		this.m_xmlItem = xmlItem;
		int xelementAttributeInt = Global.GetXElementAttributeInt(this.m_xmlItem, "ID");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(this.m_xmlItem, "VIP");
		this.ActionName = Global.GetXElementAttributeStr(this.m_xmlItem, "FileName");
		this.ItemImage.URL = Global.GetGameResInteractionString(string.Format("interaction_{0}.png", xelementAttributeInt));
		if (xelementAttributeInt2 > Global.Data.roleData.VIPLevel)
		{
			this.ItemMask.gameObject.SetActive(true);
			this.ItemMask.URL = Global.GetGameResInteractionString("mask_bak.png");
			this.TextValue.text = string.Format(Global.GetLang("V{0}开启"), xelementAttributeInt2);
			this.IsVIPOpen = false;
		}
		else
		{
			this.IsVIPOpen = true;
			this.ItemMask.gameObject.SetActive(false);
			this.TextValue.text = string.Empty;
		}
	}

	public ShowNetImage ItemImage;

	public ShowNetImage ItemMask;

	public UIButton ItemButton;

	public GTextBlockOutLine TextValue;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int id;

	private XElement m_xmlItem;

	private string ActionName = string.Empty;

	private bool IsVIPOpen;
}
