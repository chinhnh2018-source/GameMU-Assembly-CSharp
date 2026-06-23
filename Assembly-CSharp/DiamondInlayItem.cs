using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DiamondInlayItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			this.OnButtonClicked(-1);
		};
	}

	private void OnButtonClicked(int shapeType)
	{
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = this.index,
			Data = shapeType
		});
	}

	public void SetDiamondIconByType(int shapeID, int goodsID)
	{
		if (shapeID < 1 || shapeID > 3)
		{
			return;
		}
		string url = null;
		if (goodsID > 0)
		{
			url = Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID));
		}
		switch (shapeID)
		{
		case 1:
			this.icon_diamond_1.URL = url;
			break;
		case 2:
			this.icon_diamond_2.URL = url;
			break;
		case 3:
			this.icon_diamond_3.URL = url;
			break;
		}
	}

	public bool isDiamondUpgradable
	{
		set
		{
			this.isUpgradable = value;
			this.HideTipsIcon(!this.isUpgradable);
		}
	}

	private void HideTipsIcon(bool hide = true)
	{
		if (null != this.tipsIcon)
		{
			this.tipsIcon.SetActive(!hide);
		}
	}

	public ShowNetImage icon_diamond_1;

	public ShowNetImage icon_diamond_2;

	public ShowNetImage icon_diamond_3;

	public int index;

	public GameObject tipsIcon;

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool isUpgradable;
}
