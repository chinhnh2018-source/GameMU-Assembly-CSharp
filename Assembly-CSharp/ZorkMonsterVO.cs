using System;
using Tmsk.Xml;

public class ZorkMonsterVO
{
	public ZorkMonsterVO()
	{
	}

	public ZorkMonsterVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.GroupID = xe.GetAttributeInt("GroupID", -1);
		this.MonsterType = xe.GetAttributeInt("MonsterType", -1);
		this.MonsterId = xe.GetAttributeInt("MonsterId", -1);
		this.MonsterNum = xe.GetAttributeInt("MonsterNum", -1);
		this.MonsterDropBuffId = xe.GetAttributeInt("MonsterDropBuffId", -1);
		this.BuffEffictId = xe.GetAttributeInt("BuffEffictId", -1);
		this.BuffEffictTime = xe.GetAttributeInt("BuffEffictTime", -1);
		this.RewardIntegral = xe.GetAttributeInt("RewardIntegral", -1);
		this.BossBlood = xe.GetAttributeFloat("BossBlood", -1f);
		this.BuffRefreshTime = xe.GetAttributeInt("BuffRefreshTime", -1);
		this.BossBuffGroup = xe.GetAttributeStr("BossBuffGroup");
		this.BossBuffRound = xe.GetAttributeStr("BossBuffRound");
		string attributeStr = xe.GetAttributeStr("BossLostSkill");
		this.BossLostSkill = attributeStr.Split(new char[]
		{
			'|'
		});
	}

	public int ID { get; set; }

	public int GroupID { get; set; }

	public int MonsterType { get; set; }

	public int MonsterId { get; set; }

	public int MonsterNum { get; set; }

	public int MonsterDropBuffId { get; set; }

	public int BuffEffictId { get; set; }

	public int BuffEffictTime { get; set; }

	public int RewardIntegral { get; set; }

	public float BossBlood { get; set; }

	public int BuffRefreshTime { get; set; }

	public string BossBuffGroup { get; set; }

	public string BossBuffRound { get; set; }

	public string[] BossLostSkill { get; set; }
}
