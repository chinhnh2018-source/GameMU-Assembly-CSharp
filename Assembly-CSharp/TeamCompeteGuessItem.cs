using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TeamCompeteGuessItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblZhanDuiName.Text = Global.GetLang(string.Empty);
		this.LblZhanLi.Text = Global.GetLang("战力：");
		this.LblDuanWei.Text = Global.GetLang("段位：");
		this.LblChengJi.Text = Global.GetLang("战队成绩：");
		this.LblFanHuaiDian.Text = Global.GetLang("返还竞猜点:");
		this.Lbl1.Text = Global.GetLang(string.Empty);
		this.Lbl2.Text = Global.GetLang(string.Empty);
		this.Lbl3.Text = Global.GetLang(string.Empty);
		this.LblXiaZhuJinBi.Text = Global.GetLang("下注金币:");
		this.LblMoney.Text = Global.GetLang("0");
	}

	private void InitEvent()
	{
	}

	public void InitValue(ZhanDuiZhengBaZhanDuiData data)
	{
		if (data == null)
		{
			return;
		}
		this.LblZhanDuiName.Text = TeamCompeteDataManager.ServerTeamName(data.ZoneId, data.ZhanDuiName);
		this.LblZhanLi.Text = Global.GetLang("战力：") + data.ZhanLi;
		this.LblDuanWei.Text = Global.GetLang("段位：") + TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		this.LblChengJi.Text = Global.GetLang("战队成绩：") + this.GetGrade(data.Grade);
		this.LblMoney.Text = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchGuessCostJinBi().ToString();
		int occu = 0;
		if (data.MemberList != null && data.MemberList.Count > 0)
		{
			occu = data.MemberList[0].Occupation;
		}
		this.ShowTouXiang(TeamCompeteDataManager.GetTouXiangPathByOccu(occu));
		this.DisplayYaZhuInfo(data);
	}

	private string GetGrade(int rank)
	{
		if (rank == 1)
		{
			return Global.GetLang("冠军");
		}
		if (rank == 2)
		{
			return Global.GetLang("亚军");
		}
		return rank + Global.GetLang("强");
	}

	private string GetStatus(int state)
	{
		if (state == 1)
		{
			return Global.GetLang("晋级");
		}
		if (state == 2)
		{
			return Global.GetLang("淘汰");
		}
		return Global.GetLang("暂无");
	}

	private void ShowTouXiang(string path)
	{
		this.img.URL = path;
	}

	private void DisplayYaZhuInfo(ZhanDuiZhengBaZhanDuiData data)
	{
		foreach (KeyValuePair<int, TeamMatchAwardVo> keyValuePair in IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchAwardDict())
		{
			if (keyValuePair.Key == 1)
			{
				TextBlock lbl = this.Lbl3;
				object lang = Global.GetLang("冠军 ");
				Dictionary<int, TeamMatchAwardVo>.Enumerator enumerator;
				KeyValuePair<int, TeamMatchAwardVo> keyValuePair2 = enumerator.Current;
				lbl.Text = lang + keyValuePair2.Value.TeamPoint + Global.GetLang("点");
			}
			else
			{
				Dictionary<int, TeamMatchAwardVo>.Enumerator enumerator;
				KeyValuePair<int, TeamMatchAwardVo> keyValuePair3 = enumerator.Current;
				if (keyValuePair3.Key == 2)
				{
					TextBlock lbl2 = this.Lbl2;
					object lang2 = Global.GetLang("亚军 ");
					KeyValuePair<int, TeamMatchAwardVo> keyValuePair4 = enumerator.Current;
					lbl2.Text = lang2 + keyValuePair4.Value.TeamPoint + Global.GetLang("点");
				}
				else
				{
					KeyValuePair<int, TeamMatchAwardVo> keyValuePair5 = enumerator.Current;
					if (keyValuePair5.Key == 3)
					{
						TextBlock lbl3 = this.Lbl1;
						object lang3 = Global.GetLang("4强 ");
						KeyValuePair<int, TeamMatchAwardVo> keyValuePair6 = enumerator.Current;
						lbl3.Text = lang3 + keyValuePair6.Value.TeamPoint + Global.GetLang("点");
					}
				}
			}
		}
		this.LblMoney.Text = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchGuessCostJinBi().ToString();
		bool flag = data.Grade == 4;
		bool flag2 = data.Grade == 2;
		bool flag3 = data.Grade == 1;
		this.Lbl3.textColor = ((!flag3) ? Global.ParseStringColorToUint(this.grayColor) : Global.ParseStringColorToUint("#B1986FFF"));
		this.Lbl2.textColor = ((!flag2) ? Global.ParseStringColorToUint(this.grayColor) : Global.ParseStringColorToUint("#B1986FFF"));
		this.Lbl1.textColor = ((!flag) ? Global.ParseStringColorToUint(this.grayColor) : Global.ParseStringColorToUint("#B1986FFF"));
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

	public TextBlock LblZhanDuiName;

	public TextBlock LblZhanLi;

	public TextBlock LblDuanWei;

	public TextBlock LblChengJi;

	public TextBlock LblFanHuaiDian;

	public TextBlock Lbl1;

	public TextBlock Lbl2;

	public TextBlock Lbl3;

	public TextBlock LblXiaZhuJinBi;

	public TextBlock LblMoney;

	public ShowNetImage img;

	private string grayColor = "#808081";
}
