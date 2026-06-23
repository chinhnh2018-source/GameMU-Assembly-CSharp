using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ArmyTeamActivity : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.TabBtnOBC = this.ListTabBtn.ItemsSource;
		this.InitBtnItem();
		this.ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		this.ListTabBtn.SelectedIndex = 0;
	}

	private void InitBtnItem()
	{
		if (this.BtnClose != null)
		{
			this.BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		XElement gameResXml = Global.GetGameResXml("Config/LegionsHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				FamilyActivityBtnitem familyActivityBtnitem = U3DUtils.NEW<FamilyActivityBtnitem>();
				familyActivityBtnitem.label.text = xelementAttributeStr;
				familyActivityBtnitem.label.color = NGUIMath.HexToColorEx(8350293U);
				familyActivityBtnitem.TipIcon.gameObject.SetActive(false);
				familyActivityBtnitem.Id = xelementAttributeInt;
				this.TabBtnOBC.AddNoUpdate(familyActivityBtnitem);
			}
		}
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		FamilyActivityBtnitem familyActivityBtnitem = U3DUtils.AS<FamilyActivityBtnitem>(this.ListTabBtn.SelectedItem);
		if (null == familyActivityBtnitem)
		{
			return;
		}
		if (this.familyBtnItem != null && this.familyBtnItem != familyActivityBtnitem)
		{
			this.familyBtnItem.Bak.spriteName = "btn_left_normal";
			this.familyBtnItem.label.color = NGUIMath.HexToColorEx(8350293U);
		}
		if (familyActivityBtnitem == this.familyBtnItem)
		{
			return;
		}
		this.familyBtnItem = familyActivityBtnitem;
		this.familyBtnItem.Bak.spriteName = "btn_left_selected";
		familyActivityBtnitem.label.color = NGUIMath.HexToColorEx(15461355U);
		this.ShowPage(familyActivityBtnitem);
	}

	public void ShowPage(FamilyActivityBtnitem item)
	{
		this.SprPnlContent.Clear();
		this.territoryFightPart = null;
		int id = item.Id;
		if (id == 1)
		{
			this.territoryFightPart = U3DUtils.NEW<TerritoryFightPart>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.territoryFightPart.gameObject, true);
		}
	}

	public void ResTerritoryFightState(string state, string name1, string name2)
	{
		if (null == this.territoryFightPart)
		{
			return;
		}
		this.territoryFightPart.state = int.Parse(state);
		this.territoryFightPart.SetLingDiZhanLingName(name1, name2);
	}

	private new void OnDestroy()
	{
	}

	public GameObject PnlContent;

	public GameObject BtnItem;

	public SpriteSL SprPnlContent;

	private ObservableCollection TabBtnOBC;

	public ListBox ListTabBtn;

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TerritoryFightPart territoryFightPart;

	private FamilyActivityBtnitem familyBtnItem;
}
