using System;

public class RepairPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected void OnEnable()
	{
	}

	public UISprite _Bak;

	public GButton _Close;

	public GButton _Submit;

	public GButton _Help;

	public TextBlock _Text;
}
