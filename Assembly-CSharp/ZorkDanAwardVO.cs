using System;
using Tmsk.Xml;

public class ZorkDanAwardVO
{
	public ZorkDanAwardVO()
	{
	}

	public ZorkDanAwardVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.RankLevel = xe.GetAttributeStr("RankLevel");
		this.RankValue = xe.GetAttributeInt("RankValue", -1);
		this.WinRankValue = xe.GetAttributeInt("WinRankValue", -1);
		this.LoseRankValue = xe.GetAttributeInt("LoseRankValue", -1);
		string attributeStr = xe.GetAttributeStr("FirstBattleReward");
		this.FirstBattleReward = attributeStr.Split(new char[]
		{
			'|'
		});
		string attributeStr2 = xe.GetAttributeStr("SeasonReward");
		this.SeasonReward = attributeStr2.Split(new char[]
		{
			'|'
		});
		string attributeStr3 = xe.GetAttributeStr("WinRankReward");
		this.WinRankReward = attributeStr3.Split(new char[]
		{
			'|'
		});
		string attributeStr4 = xe.GetAttributeStr("LoseRankReward");
		this.LoseRankReward = attributeStr4.Split(new char[]
		{
			'|'
		});
	}

	public int ID { get; set; }

	public string RankLevel { get; set; }

	public int RankValue { get; set; }

	public int WinRankValue { get; set; }

	public int LoseRankValue { get; set; }

	public string[] FirstBattleReward { get; set; }

	public string[] SeasonReward { get; set; }

	public string[] WinRankReward { get; set; }

	public string[] LoseRankReward { get; set; }
}
