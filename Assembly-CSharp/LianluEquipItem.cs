using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class LianluEquipItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ConstTxtHint.Text = Global.GetLang("点击放入");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.UpgradeStat = false;
	}

	public bool SelectStat
	{
		get
		{
			return this.SelectBak.Visibility;
		}
		set
		{
			this.SelectBak.Visibility = value;
		}
	}

	public bool UpgradeStat
	{
		get
		{
			return this.UpgradeStatSprite.Visibility;
		}
		set
		{
			this.UpgradeStatSprite.Visibility = value;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite Bak;

	public SpriteSL SelectBak;

	public TextBlock TxtEquipName;

	public GGoodIcon EquipIcon;

	public TextBlock ConstTxtHint;

	public SpriteSL UpgradeStatSprite;
}
