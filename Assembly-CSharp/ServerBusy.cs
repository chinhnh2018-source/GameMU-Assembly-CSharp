using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class ServerBusy : UserControl
{
	public string Title
	{
		set
		{
			if (null != this.title)
			{
				this.title.Text = value;
			}
		}
	}

	public string TextFiled
	{
		set
		{
			if (null != this.textField)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.textField.Text = string.Empty;
				}
				else
				{
					this.textField.Text = Global.GetColorStringForNGUIText(new object[]
					{
						this.fontColor_normal,
						value
					});
				}
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.okBtn.Text = Global.GetLang("取消");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.okBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OKBtn_MouseLeftButtonUp);
		this.Title = Global.GetLang("提示");
		this.TextFiled = Global.GetLang("服务器排队人数已达上限，请稍后再试！");
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void OKBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.buttonClick != null)
		{
			this.buttonClick.Invoke(this, EventArgs.Empty);
		}
	}

	public TextBlock title;

	public TextBlock textField;

	public GButton okBtn;

	private string fontColor_normal = "e3b36c";

	public EventHandler buttonClick;
}
