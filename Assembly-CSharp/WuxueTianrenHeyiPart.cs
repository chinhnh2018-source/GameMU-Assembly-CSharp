using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class WuxueTianrenHeyiPart : UserControl
{
	public WuxueTianrenHeyiPart()
	{
		this.wuxue11_normal = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxue11_normal.png"));
		this.wuxue11_hover = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxue11_hover.png"));
		this.wuxue12_normal = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxue12_normal.png"));
		this.wuxue12_hover = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxue12_hover.png"));
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Wuxue11 = U3DUtils.NEW<GIcon>();
		this.Wuxue11.Width = 97.0;
		this.Wuxue11.Height = 92.0;
		this.Wuxue11.BodySource = this.wuxue11_hover;
		this.Wuxue11.DisableBodySource = this.wuxue11_normal;
		this.Container.Children.Add(this.Wuxue11);
		Canvas.SetLeft(this.Wuxue11, 45);
		Canvas.SetTop(this.Wuxue11, 26);
		this.Wuxue11.IsHitTestVisible = false;
		this.Wuxue12 = U3DUtils.NEW<GIcon>();
		this.Wuxue12.Width = 97.0;
		this.Wuxue12.Height = 92.0;
		this.Wuxue12.BodySource = this.wuxue12_hover;
		this.Wuxue12.DisableBodySource = this.wuxue12_normal;
		this.Container.Children.Add(this.Wuxue12);
		Canvas.SetLeft(this.Wuxue12, 169);
		Canvas.SetTop(this.Wuxue12, 26);
		this.Wuxue12.IsHitTestVisible = false;
	}

	public void InitPartData(int index)
	{
		if (index == 11)
		{
			this.Wuxue11.EnableIcon = true;
			this.Wuxue12.EnableIcon = false;
			this.Wuxue11.HintDecoType = 6;
			this.Wuxue12.HintDecoType = -1;
		}
		else if (index == 12)
		{
			this.Wuxue11.EnableIcon = true;
			this.Wuxue12.EnableIcon = true;
			this.Wuxue11.HintDecoType = 6;
			this.Wuxue12.HintDecoType = 7;
		}
		else
		{
			this.Wuxue11.EnableIcon = false;
			this.Wuxue12.EnableIcon = false;
			this.Wuxue11.HintDecoType = -1;
			this.Wuxue12.HintDecoType = -1;
		}
	}

	public override void Destroy()
	{
		this.Wuxue11.HintDecoType = -1;
		this.Wuxue12.HintDecoType = -1;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GIcon Wuxue11;

	private GIcon Wuxue12;

	private ImageBrush wuxue11_normal;

	private ImageBrush wuxue11_hover;

	private ImageBrush wuxue12_normal;

	private ImageBrush wuxue12_hover;
}
