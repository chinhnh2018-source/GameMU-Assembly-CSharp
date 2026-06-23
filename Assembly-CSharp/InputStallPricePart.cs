using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class InputStallPricePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.TextBlock1);
		this.TextBlock1.Foreground = new SolidColorBrush(4280732698U);
		Canvas.SetLeft(this.TextBlock1, 30);
		Canvas.SetTop(this.TextBlock1, 32);
		this.TextBlock1.FontSize = HSTextField.defaultFontSize;
		this.Container.Children.Add(this.TextBlock2);
		this.TextBlock2.Foreground = new SolidColorBrush(4292690945U);
		Canvas.SetLeft(this.TextBlock2, 130);
		Canvas.SetTop(this.TextBlock2, 32);
		this.TextBlock2.FontSize = HSTextField.defaultFontSize;
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int GoodsDbId
	{
		get
		{
			return this._GoodsDbId;
		}
		set
		{
			this._GoodsDbId = value;
		}
	}

	public int Price
	{
		get
		{
			int result = 0;
			try
			{
				result = Convert.ToInt32(this.textBlock.Text.Text);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				result = 0;
			}
			return result;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.textBlock = U3DUtils.NEW<GTextBlock>();
		this.textBlock.BodyWidth = 209.0;
		this.textBlock.BodyHeight = 21.0;
		this.textBlock.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 209.0, 21.0, 3.0, 2.0));
		this.textBlock.Onlydouble = true;
		this.textBlock.Text.Text = string.Empty;
		this.textBlock.Text.FontSize = HSTextField.defaultFontSize;
		this.textBlock.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.textBlock, 29);
		Canvas.SetTop(this.textBlock, 59);
		this.Container.Children.Add(this.textBlock);
		this.textBlock.Text.Focus();
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 42.0;
		gicon.Height = 19.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn10_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn10_hover.png"));
		gicon.MouseLeftButtonDown = delegate(object sender, MouseEvent e)
		{
		};
		gicon.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.textBlock.Text.Text == string.Empty)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("商品单价不能为空!"), 0, -1, -1, 0);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 1
				});
			}
		};
		Canvas.SetLeft(gicon, 146);
		Canvas.SetTop(gicon, 94);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 42.0;
		gicon.Height = 19.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn11_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn11_hover.png"));
		gicon.MouseLeftButtonDown = delegate(object sender, MouseEvent e)
		{
		};
		gicon.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 196);
		Canvas.SetTop(gicon, 94);
		this.Container.Children.Add(gicon);
	}

	private GTextBlock textBlock;

	private TextBlock TextBlock1 = new TextBlock();

	private TextBlock TextBlock2 = new TextBlock();

	private SpriteSL thisCtrl = new SpriteSL();

	private int _GoodsDbId;

	public DPSelectedItemEventHandler DPSelectedItem;
}
