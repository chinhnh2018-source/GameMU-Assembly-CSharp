using System;
using HSGameEngine.GameEngine.SilverLight;

public class ActivitiesGiftItem : UserControl
{
	public ActivitiesGiftItem()
	{
		this.ActivitiesInfoText2.TextFontWrapping = TextWrapping.Wrap;
		this.GoodsInfoText.TextFontWrapping = TextWrapping.Wrap;
		this.GoodsInfoText.mouseEnabled = false;
	}

	protected override void InitializeComponent()
	{
		this.StackPanel1.Children.Add(this.Rectangle2);
		this.Rectangle2.Height = 5.0;
		this.Rectangle2.Width = 180.0;
		this.Rectangle2.HorizontalAlignment = global::Layout.Left;
		this.Rectangle2.VerticalAlignment = global::Layout.Top;
		this.Rectangle2.IsHitTestVisible = false;
		this.StackPanel1.Children.Add(this.ActivitiesInfoText2);
		this.ActivitiesInfoText2.Text = string.Empty;
		this.ActivitiesInfoText2.FontSize = HSTextField.defaultFontSize;
		this.ActivitiesInfoText2.TextColor = new SolidColorBrush(7448500U);
		this.ActivitiesInfoText2.Width = 330.0;
		this.ActivitiesInfoText2.TextWidth = 330.0;
		this.StackPanel1.Children.Add(this.Rectangle3);
		this.Rectangle3.Height = 5.0;
		this.Rectangle3.Width = 180.0;
		this.Rectangle3.HorizontalAlignment = global::Layout.Left;
		this.Rectangle3.VerticalAlignment = global::Layout.Top;
		this.Rectangle3.IsHitTestVisible = false;
		this.StackPanel1.Children.Add(this.GoodsInfoText);
		Canvas.SetLeft(this.StackPanel1, 5);
		this.Container.Children.Add(this.StackPanel1);
		this.GoodsInfoText.Text = string.Empty;
		this.GoodsInfoText.FontSize = HSTextField.defaultFontSize;
		this.GoodsInfoText.TextWidth = 330.0;
		this.GoodsInfoText.BodyWidth = 330.0;
		this.GoodsInfoText.HorizontalAlignment = global::Layout.Left;
		this.GoodsInfoText.VerticalAlignment = global::Layout.Top;
		this.StackPanel1.Children.Add(this.GoodsInfoText);
	}

	public StackPanel StackPanel1 = new StackPanel();

	public RectangleSL Rectangle2 = new RectangleSL();

	public GTextBlockOutLine ActivitiesInfoText2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public RectangleSL Rectangle3 = new RectangleSL();

	public GTextBlockEx GoodsInfoText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
}
