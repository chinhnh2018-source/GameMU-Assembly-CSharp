using System;
using HSGameEngine.GameEngine.SilverLight;

public class GImageEx : GTipSprite
{
	public GImageEx()
	{
		base.mouseChildren = false;
		GTipService.HookTip(this, this);
	}

	protected override void InitializeComponent()
	{
		base.Children.Add(this.Body);
		this.Body.Stretch = global::StretchSL.None;
		Canvas.SetZIndex(this.Body, 0.0);
	}

	public URLImage Body = new URLImage();
}
