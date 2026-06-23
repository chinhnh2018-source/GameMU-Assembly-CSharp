using System;
using Tmsk.Xml;

public class EscapeActivityRulesVO
{
	public EscapeActivityRulesVO()
	{
	}

	public EscapeActivityRulesVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.MapCode = xe.GetAttributeStr("MapCode");
		this.TimePoints = xe.GetAttributeStr("TimePoints");
		this.BattleSignSecs = xe.GetAttributeInt("BattleSignSecs", -1);
		this.SignCondition = xe.GetAttributeStr("SignCondition");
		this.MatchTeamNum = xe.GetAttributeInt("MatchTeamNum", -1);
		this.EnterBattleNum = xe.GetAttributeInt("EnterBattleNum", -1);
		this.PrepareSecs = xe.GetAttributeInt("PrepareSecs", -1);
		this.SafeSecs = xe.GetAttributeInt("SafeSecs", -1);
		this.FanaticismStartNum = xe.GetAttributeInt("FanaticismStartNum", -1);
		this.BattleEndTime = xe.GetAttributeInt("BattleEndTime", -1);
		this.RankRefreshTime = xe.GetAttributeInt("RankRefreshTime", -1);
		this.RewardView = xe.GetAttributeStr("RewardView");
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public string MapCode { get; set; }

	public string TimePoints { get; set; }

	public int BattleSignSecs { get; set; }

	public string SignCondition { get; set; }

	public int MatchTeamNum { get; set; }

	public int EnterBattleNum { get; set; }

	public int PrepareSecs { get; set; }

	public int SafeSecs { get; set; }

	public int FanaticismStartNum { get; set; }

	public int BattleEndTime { get; set; }

	public int RankRefreshTime { get; set; }

	public string RewardView { get; set; }
}
