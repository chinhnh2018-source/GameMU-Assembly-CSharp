using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class DBBufferBoxItem : UserControl
{
	public CDCoolDown CoolDown
	{
		get
		{
			return this.cdCoolDown;
		}
	}

	public GIcon ItemIcon
	{
		get
		{
			return this._ItemIcon;
		}
		set
		{
			if (null != this._ItemIcon)
			{
				this.Container.Children.Remove(this._ItemIcon, true);
			}
			if (null != this.cdCoolDown)
			{
				this.cdCoolDown.CoodDownComplete = new EventHandler(this.CoodDownComplete);
				this.Container.Children.Remove(this.cdCoolDown, true);
				this.cdCoolDown = null;
			}
			this._ItemIcon = value;
			if (null != this._ItemIcon)
			{
				this.Container.Children.Add(this._ItemIcon);
				Canvas.SetZIndex(this._ItemIcon, 998.0);
				this.cdCoolDown = U3DUtils.NEW<CDCoolDown>();
				this.cdCoolDown.Width = this.Width - 2.0;
				this.cdCoolDown.Height = this.Height - 2.0;
				this.cdCoolDown.BodyColor = new SolidColorBrush(ColorSL.FromArgb(150, 0, 0, 0));
				this.cdCoolDown.Opacity = 1.0;
				this.cdCoolDown.TextFontSize = FontSizeMgr.CDCollDownFontSize;
				this.cdCoolDown.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
				this.cdCoolDown.IsHitTestVisible = false;
				this.cdCoolDown.MaskAlpha = 0.7;
				this.cdCoolDown.CoodDownComplete = new EventHandler(this.CoodDownComplete);
				Canvas.SetLeft(this.cdCoolDown, 1);
				Canvas.SetTop(this.cdCoolDown, 1);
				Canvas.SetZIndex(this.cdCoolDown, 999.0);
				this.Container.Children.Add(this.cdCoolDown);
				CDCoolDown cdcoolDown = U3DUtils.NEW<CDCoolDown>();
				cdcoolDown.Width = this.Width - 2.0;
				cdcoolDown.Height = this.Height - 2.0;
				cdcoolDown.BodyColor = new SolidColorBrush(ColorSL.FromArgb(150, 0, 0, 0));
				cdcoolDown.Opacity = 1.0;
				cdcoolDown.TextFontSize = FontSizeMgr.CDCollDownFontSize;
				cdcoolDown.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
				cdcoolDown.IsHitTestVisible = false;
				cdcoolDown.BackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/buf_bak.png"));
				cdcoolDown.MaskAlpha = 0.7;
				cdcoolDown.CoodDownComplete = new EventHandler(this.CoodDownComplete);
				Canvas.SetLeft(cdcoolDown, 1);
				Canvas.SetTop(cdcoolDown, 1);
				Canvas.SetZIndex(cdcoolDown, 997.0);
				this.Container.Children.Add(cdcoolDown);
			}
		}
	}

	protected override void InitializeComponent()
	{
	}

	private void CoodDownComplete(object sender, object e)
	{
	}

	private GIcon _ItemIcon;

	private CDCoolDown cdCoolDown;
}
