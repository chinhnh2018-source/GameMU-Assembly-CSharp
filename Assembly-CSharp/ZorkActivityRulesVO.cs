using System;
using Tmsk.Xml;

public class ZorkActivityRulesVO
{
	public ZorkActivityRulesVO()
	{
	}

	public ZorkActivityRulesVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.MapCode = xe.GetAttributeInt("MapCode", -1);
		this.TimePoints = xe.GetAttributeStr("TimePoints");
		this.BattleSignSecs = xe.GetAttributeInt("BattleSignSecs", -1);
		this.SignCondition = xe.GetAttributeStr("SignCondition");
		this.MaxEnterNum = xe.GetAttributeInt("MaxEnterNum", -1);
		this.EnterCD = xe.GetAttributeInt("EnterCD", -1);
		this.PrepareSecs = xe.GetAttributeInt("PrepareSecs", -1);
		this.FightingSecs = xe.GetAttributeInt("FightingSecs", -1);
		this.ClearRolesSecs = xe.GetAttributeInt("ClearRolesSecs", -1);
		this.SeasonFightDay = xe.GetAttributeInt("SeasonFightDay", -1);
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public int MapCode { get; set; }

	public string TimePoints { get; set; }

	public int BattleSignSecs { get; set; }

	public string SignCondition { get; set; }

	public int MaxEnterNum { get; set; }

	public int EnterCD { get; set; }

	public int PrepareSecs { get; set; }

	public int FightingSecs { get; set; }

	public int ClearRolesSecs { get; set; }

	public int SeasonFightDay { get; set; }
}
