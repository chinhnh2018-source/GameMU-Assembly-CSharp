using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class KuafuDaojishiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 1
						});
						KuafuJoinPart.IsBaoMing = 0;
					}
				}, -1);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
				KuafuJoinPart.IsBaoMing = 0;
			}
		};
		this.BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
			KuafuJoinPart.IsBaoMing = 0;
		};
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	private void InitTextInPrefabs()
	{
		this.BtnOK.Text = Global.GetLang("立即进入");
		this.BtnCancel.Text = Global.GetLang("取 消");
		this.staticText[0].text = Global.GetLang("您已获得进入幻影寺院的资格，是否立刻进入！");
		this.staticText[1].text = Global.GetLang("后自动取消");
		this.labTime.X = -70.0;
	}

	public void SetTextInPrefabs(string btnOKText, string btnCancelText, string staticText0, string staticText1, bool IsNeedDaojishi)
	{
		this.BtnOK.Text = btnOKText;
		this.BtnCancel.Text = btnCancelText;
		this.staticText[0].text = staticText0;
		this.staticText[1].text = staticText1;
		if (!IsNeedDaojishi)
		{
			base.CancelInvoke("TickProc");
			this.labTime.Text = string.Empty;
		}
	}

	protected void TickProc()
	{
		if (this.waitTime >= 0)
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
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		this.waitTime--;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock[] staticText;

	public TextBlock labTime;

	public GButton BtnOK;

	public GButton BtnCancel;

	private int waitTime = 15;
}
