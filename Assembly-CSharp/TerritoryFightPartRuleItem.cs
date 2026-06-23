using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TerritoryFightPartRuleItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BG.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang.png.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.TabBtnOBC = this.ListItems.ItemsSource;
		this.InitRuleItem();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
	}

	private void InitRuleItem()
	{
		this.ItemXi = U3DUtils.NEW<TerritoryFightPartRuleListItem>();
		this.ItemXi.WestOrEast = 2;
		this.ItemXi.ImageName = "akalunximap.jpg.qj";
		this.TabBtnOBC.AddNoUpdate(this.ItemXi);
		this.ItemDong = U3DUtils.NEW<TerritoryFightPartRuleListItem>();
		this.ItemDong.WestOrEast = 1;
		this.ItemDong.ImageName = "akalundongmap.jpg.qj";
		this.TabBtnOBC.AddNoUpdate(this.ItemDong);
		UIPanel component = this.ItemDong.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		this.SprPnlContent.GetComponent<UIDraggablePanel>().onDragFinished = delegate()
		{
			if (this.canHover)
			{
				int num = (int)this.SprPnlContent.gameObject.transform.localPosition.x;
				if (num > -470)
				{
					this.SprPnlContent.target = new Vector3(0f, 0f, 0f);
					this.SpriteRight.gameObject.SetActive(true);
					this.SpriteLeft.gameObject.SetActive(false);
				}
				else
				{
					this.SprPnlContent.target = new Vector3(-946f, 0f, 0f);
					this.SpriteRight.gameObject.SetActive(false);
					this.SpriteLeft.gameObject.SetActive(true);
				}
				this.SprPnlContent.enabled = true;
			}
		};
		UIPanel component2 = this.ItemXi.GetComponent<UIPanel>();
		if (component2 != null)
		{
			Object.Destroy(component2);
		}
	}

	public void SetSelectedIndex(int index)
	{
		if (index == 0)
		{
			this.SprPnlContent.target = new Vector3(-946f, 0f, 0f);
			this.SpriteRight.gameObject.SetActive(false);
			this.SpriteLeft.gameObject.SetActive(true);
		}
		else
		{
			this.SprPnlContent.target = new Vector3(0f, 0f, 0f);
			this.SpriteRight.gameObject.SetActive(true);
			this.SpriteLeft.gameObject.SetActive(false);
		}
		this.SprPnlContent.enabled = true;
		base.InvokeRepeating("CanOnHover", 1f, 1f);
	}

	private void CanOnHover()
	{
		this.canHover = true;
	}

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedClose;

	public SpringPanel SprPnlContent;

	public ShowNetImage BG;

	public ListBox ListItems;

	public UISprite SpriteLeft;

	public UISprite SpriteRight;

	private ObservableCollection TabBtnOBC;

	private TerritoryFightPartRuleListItem ItemXi;

	private TerritoryFightPartRuleListItem ItemDong;

	private bool canHover;
}
