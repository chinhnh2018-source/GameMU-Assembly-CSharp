using System;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TeamCompeteListItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void InitValue(TianTi5v5ZhanDuiMiniData data)
	{
		this.LblTeamName.Text = data.DuiZhangName;
		this.LblLeaderName.Text = data.Name;
		if (data.MemberList == null || data.MemberList.Count <= 0)
		{
			this.LblLeaderCount.Text = "1/5";
		}
		else
		{
			this.LblLeaderCount.Text = data.MemberList.Count.ToString() + "/5";
		}
		this.LblLeaderBattleValue.Text = data.ZhanDouLi.ToString();
		this.LblDuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID((int)data.DuanWeiID);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTeamName;

	public TextBlock LblLeaderName;

	public TextBlock LblLeaderCount;

	public TextBlock LblLeaderBattleValue;

	public TextBlock LblDuanWei;
}
