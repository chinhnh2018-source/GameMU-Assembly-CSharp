using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class ChatChannelFilterPart : UserControl
{
	public ChatChannelFilterPart()
	{
		this.textBlock1.Text = Global.GetLang("选择频道(多选)");
		this.CheckBox1.Content = Global.GetLang("综合");
		this.CheckBox2.Content = Global.GetLang("世界");
		this.CheckBox3.Content = Global.GetLang("战盟");
		this.CheckBox4.Content = Global.GetLang("队伍");
		this.CheckBox5.Content = Global.GetLang("私聊");
	}

	protected override void InitializeComponent()
	{
		this.Container.Width = 113.0;
		this.Container.Height = 131.0;
		this.Container.Background = new SolidColorBrush(4278190080U);
		this.Container.Children.Add(this.textBlock1);
		this.textBlock1.Height = 23.0;
		Canvas.SetLeft(this.textBlock1, 4);
		Canvas.SetTop(this.textBlock1, 4);
		this.textBlock1.FontSize = HSTextField.defaultFontSize;
		this.textBlock1.Text = Global.GetLang("选择频道(多选)");
		this.textBlock1.Width = 100.0;
		this.Container.Children.Add(this.StackPanel1);
		Canvas.SetLeft(this.StackPanel1, 4);
		Canvas.SetTop(this.StackPanel1, 27);
		this.StackPanel1.Children.Add(this.CheckBox1);
		this.CheckBox1.Content = Global.GetLang("综合");
		this.CheckBox1.Height = 16;
		this.CheckBox1.Foreground = new SolidColorBrush(uint.MaxValue);
		this.StackPanel1.Children.Add(this.CheckBox2);
		this.CheckBox2.Content = Global.GetLang("世界");
		this.CheckBox2.Height = 16;
		this.CheckBox2.Foreground = new SolidColorBrush(uint.MaxValue);
		this.StackPanel1.Children.Add(this.CheckBox3);
		this.CheckBox3.Content = Global.GetLang("战盟");
		this.CheckBox3.Height = 16;
		this.CheckBox3.Foreground = new SolidColorBrush(uint.MaxValue);
		this.StackPanel1.Children.Add(this.CheckBox4);
		this.CheckBox4.Content = Global.GetLang("队伍");
		this.CheckBox4.Height = 16;
		this.CheckBox4.Foreground = new SolidColorBrush(uint.MaxValue);
		this.StackPanel1.Children.Add(this.CheckBox5);
		this.CheckBox5.Content = Global.GetLang("私聊");
		this.CheckBox5.Height = 16;
		this.CheckBox5.Foreground = new SolidColorBrush(uint.MaxValue);
	}

	private void checkBox_Click(object sender, MouseEvent e)
	{
	}

	private TextBlock textBlock1 = new TextBlock();

	private StackPanel StackPanel1 = new StackPanel();

	private GCheckBox CheckBox1 = new GCheckBox();

	private GCheckBox CheckBox2 = new GCheckBox();

	private GCheckBox CheckBox3 = new GCheckBox();

	private GCheckBox CheckBox4 = new GCheckBox();

	private GCheckBox CheckBox5 = new GCheckBox();
}
