using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class OlympicsMatchTips : UserControl
{
	private void InitTextInPrefabs()
	{
		this.content.Text = Global.GetLang("您上次比赛没有完成，是否继续\n？");
		this.btnConfirm.Text = Global.GetLang("确定");
		this.btnCancel.Text = Global.GetLang("取消");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.btnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void SetValue(int type, int timesInGame, int score)
	{
		if (type == 1)
		{
			Super.HintMainText(string.Format("{0}{1}", new object[]
			{
				Global.GetLang("上次剩余射击次数："),
				OlympicsDataManage.GetMatchData()[1].GameNum - timesInGame,
				Global.GetLang("上次的得分："),
				score
			}), 10, 3);
		}
		else if (type == 2)
		{
			Super.HintMainText(string.Format("{0}{1}", new object[]
			{
				Global.GetLang("上次剩余踢球次数："),
				OlympicsDataManage.GetMatchData()[2].GameNum - timesInGame,
				Global.GetLang("上次的得分："),
				score
			}), 10, 3);
		}
	}

	public DPSelectedItemEventHandler Hander;

	public TextBlock content;

	public GButton btnConfirm;

	public GButton btnCancel;
}
