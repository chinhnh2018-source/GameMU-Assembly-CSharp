using System;
using Tmsk.Xml;

public class TeamMatchVo
{
	public TeamMatchVo(XElement element)
	{
		this.ID = element.GetAttributeInt("ID", -1);
		this.Name = element.GetAttributeStr("Name");
		this.MapCode = element.GetAttributeInt("MapCode", -1);
		this.TimePoints = element.GetAttributeStr("TimePoints");
		this.LotteryTime = element.GetAttributeStr("LotteryTime");
		this.LotteryMoney = element.GetAttributeInt("LotteryMoney", -1);
	}

	public int ID;

	public string Name;

	public int MapCode;

	public string TimePoints;

	public int EnterCD;

	public int PrepareSecs;

	public int FightingSecs;

	public int ClearRolesSecs;

	public string LotteryTime;

	public int LotteryMoney;
}
