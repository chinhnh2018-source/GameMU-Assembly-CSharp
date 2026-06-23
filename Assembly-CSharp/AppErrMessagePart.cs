using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.SilverLight;

public class AppErrMessagePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Panel.Background = new SolidColorBrush(16777215U);
		this.Panel.Orientation = global::Layout.Vertical;
		this.Panel.Width = this.ScrollViewer1.Width;
		this.Container.Children.Add(this.ScrollViewer1);
		this.ScrollViewer1.Width = 280.0;
		this.ScrollViewer1.Height = 315.0;
		Canvas.SetLeft(this.ScrollViewer1, 0);
		Canvas.SetTop(this.ScrollViewer1, 0);
		this.ScrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer1.Viewer = this.Panel;
		this.Panel.Children.Add(this.textBox1);
		Canvas.SetLeft(this.textBox1, 5);
		Canvas.SetTop(this.textBox1, 5);
		this.textBox1.Height = 23.0;
		this.textBox1.Width = 120.0;
		this.textBox1.TextWrapping = true;
		this.textBox1.AcceptsReturn = true;
		this.textBox1.border = false;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		Canvas.SetLeft(this.textBox1, 0);
		Canvas.SetTop(this.textBox1, 0);
		this.textBox1.Background = new SolidColorBrush(16777215U);
		this.textBox1.Foreground = new SolidColorBrush(uint.MaxValue);
		this.textBox1.Width = this.Container.Width - 20.0;
		this.textBox1.Height = this.Container.Height;
		this.Container.BackgroundAlpha = 0.8;
		this.Container.BackgroundColor = 2175545U;
	}

	public void InitPartData()
	{
		this.textBox1.Text = string.Empty;
		List<string> errMsgList = GError.GetErrMsgList();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < errMsgList.Count; i++)
		{
			stringBuilder.AppendLine(errMsgList[i]);
		}
		this.textBox1.Text = stringBuilder.ToString();
		this.textBox1.Height = (double)this.textBox1.textHeight;
		this.ScrollViewer1.ResetScrollView();
	}

	private TextBox textBox1 = new TextBox();

	private StackPanel Panel = new StackPanel();

	private GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);
}
