using System;
using Tmsk.Xml;

public class EscapeDanListVo
{
	public EscapeDanListVo(XElement element)
	{
		this.ID = element.GetAttributeInt("ID", -1);
		this.RankLevelName = element.GetAttributeStr("RankLevelName");
		this.RankValue = element.GetAttributeInt("RankValue", -1);
		this.WinRankValue = element.GetAttributeStr("WinRankValue");
		this.LoseRankValue = element.GetAttributeStr("LoseRankValue");
		this.WinRankReward = element.GetAttributeStr("WinRankReward");
		this.LoseRankReward = element.GetAttributeStr("LoseRankReward");
		this.FirstWinRankReward = element.GetAttributeStr("FirstWinRankReward");
	}

	public int ID;

	public string RankLevelName;

	public int RankValue;

	public string WinRankValue;

	public string LoseRankValue;

	public string WinRankReward;

	public string LoseRankReward;

	public string FirstWinRankReward;
}
