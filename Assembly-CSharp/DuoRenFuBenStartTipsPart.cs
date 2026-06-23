using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class DuoRenFuBenStartTipsPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	private void InitTextInPrefabs()
	{
		this.BtnOK.Text = Global.GetLang("立即进入");
		this.BtnCancel.Text = Global.GetLang("取 消");
		this.staticText[0].text = Global.GetLang("副本队伍已经集合完毕，是否立刻进入？");
		this.staticText[1].text = Global.GetLang("后自动进入");
		this.labTime.Pivot = 5;
		this.labTime.X = -70.0;
	}

	public void InitData(int countdown)
	{
		this.waitTime = countdown;
		if (countdown > 0)
		{
			base.CancelInvoke("TickProc");
			base.InvokeRepeating("TickProc", 0f, 1f);
		}
		else
		{
			this.labTime.Text = string.Empty;
			this.staticText[1].text = string.Empty;
		}
	}

	protected void TickProc()
	{
		if (this.waitTime > 0)
		{
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				this.waitTime.ToString() + Global.GetLang("秒")
			});
		}
		else
		{
			base.CancelInvoke("TickProc");
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		}
		this.waitTime--;
	}

	public override void Destroy()
	{
		base.CancelInvoke("TickProc");
		base.Destroy();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock[] staticText;

	public TextBlock labTime;

	public GButton BtnOK;

	public GButton BtnCancel;

	private int waitTime = 5;
}
