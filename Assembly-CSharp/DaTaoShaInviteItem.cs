using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class DaTaoShaInviteItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void InitValue(EscapeBattleJoinRoleInfo data)
	{
		if (data == null)
		{
			this.LblName.Text = string.Empty;
			this.LblLevel.Text = string.Empty;
			this.LblZhanLi.Text = string.Empty;
			this.LblStatus.Text = string.Empty;
			this.LblStatus.Label.color = Color.white;
		}
		else
		{
			this.LblName.Text = data.Name;
			this.LblLevel.Text = string.Concat(new object[]
			{
				data.ChangeLevel,
				Global.GetLang("转"),
				data.Level,
				Global.GetLang("级")
			});
			this.LblZhanLi.Text = data.CombatForce.ToString();
			this.LblStatus.Text = ((!data.Join) ? Global.GetLang("未加入") : Global.GetLang("已加入"));
			this.LblStatus.Label.color = ((!data.Join) ? Color.white : Color.green);
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblName;

	public TextBlock LblLevel;

	public TextBlock LblZhanLi;

	public TextBlock LblStatus;
}
