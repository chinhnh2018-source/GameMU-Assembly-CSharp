using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AlchemyPartAttr : UserControl
{
	public new string Name
	{
		set
		{
			this.name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				value
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject sb)
		{
			this.DPSHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public UILabel name;

	public DPSelectedItemEventHandler DPSHandler;
}
