using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class WolfSoulField_Rule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Texture.URL = "NetImages/GameRes/Images/Plate/langhunRule.png";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs());
		};
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton Close;

	public ShowNetImage Texture;
}
