using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class BaodianGuideItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnMain.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GButton gbutton = U3DUtils.AS<GButton>(s as GameObject);
		};
	}

	public GButton btnMain;

	public UISprite mainIcon;
}
