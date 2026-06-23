using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class WaitingQueue : UserControl
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

	public string TextFiled_1
	{
		set
		{
			if (null != this.textField_1)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.textField_1.Text = string.Empty;
				}
				else
				{
					this.textField_1.Text = Global.GetColorStringForNGUIText(new object[]
					{
						this.fontColor_normal,
						value
					});
				}
			}
		}
	}

	public string TextFiled_2
	{
		set
		{
			if (null != this.textField_2)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.textField_2.Text = string.Empty;
				}
				else
				{
					this.textField_2.text = Global.GetColorStringForNGUIText(new object[]
					{
						this.fontColor_highlight,
						value
					});
				}
			}
		}
	}

	public string TextFiled_3
	{
		set
		{
			if (null != this.textField_3)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.textField_3.Text = string.Empty;
				}
				else
				{
					this.textField_3.text = Global.GetColorStringForNGUIText(new object[]
					{
						this.fontColor_highlight,
						value
					});
				}
			}
		}
	}

	public int WaitingSeconds
	{
		set
		{
			if (value > 0)
			{
				this.longTicks = (long)(1000 * value) + Global.GetCorrectLocalTime();
				base.CancelInvoke("TickProc");
				base.InvokeRepeating("TickProc", 0f, 1f);
			}
			else
			{
				this.TextFiled_3 = string.Empty;
			}
		}
	}

	public int WaitingSeconds_Fixed
	{
		set
		{
			this.longTicks = (long)value;
			this.Tick();
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
		this.TextFiled_1 = Global.GetLang("正在刷新排队信息...");
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void RefreshWaitingQueue(int toWaitFor, int leftSeconds)
	{
		this.TextFiled_1 = Global.GetLang("服务器人数已满，请耐心等待...");
		this.TextFiled_2 = Global.GetLang("您现在排在第") + toWaitFor + Global.GetLang("位");
		this.WaitingSeconds_Fixed = leftSeconds;
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.longTicks;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (num2 > 0)
			{
				this.TextFiled_3 = Global.GetLang("预计等待时间：") + Global.GetTimeStrBySecEx((double)num2, true, -1);
			}
		}
		else
		{
			this.TextFiled_3 = string.Empty;
			base.CancelInvoke("TickProc");
		}
	}

	protected void Tick()
	{
		string timeField = this.GetTimeField((int)this.longTicks);
		this.TextFiled_3 = Global.GetLang("预计排队时间") + timeField;
	}

	protected string GetTimeField(int seconds)
	{
		string lang = Global.GetLang("分钟");
		string result = string.Empty;
		if (seconds <= 60)
		{
			result = "<1" + lang;
		}
		else if (seconds <= 180 && seconds > 60)
		{
			result = "<3" + lang;
		}
		else if (seconds <= 300 && seconds > 180)
		{
			result = "<5" + lang;
		}
		else if (seconds <= 600 && seconds > 300)
		{
			result = "<10" + lang;
		}
		else if (seconds <= 1200 && seconds > 600)
		{
			result = "<20" + lang;
		}
		else if (seconds <= 1800 && seconds > 1200)
		{
			result = "<30" + lang;
		}
		else if (seconds > 1800)
		{
			result = ">30" + lang;
		}
		return result;
	}

	private void OKBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.buttonClick != null)
		{
			this.buttonClick.Invoke(this, EventArgs.Empty);
		}
	}

	public TextBlock title;

	public TextBlock textField_1;

	public TextBlock textField_2;

	public TextBlock textField_3;

	public GButton okBtn;

	private string fontColor_normal = "e3b36c";

	private string fontColor_highlight = "00ff00";

	private long longTicks;

	public EventHandler buttonClick;
}
