using System;
using Tmsk.Xml;

public class ZorkAchievementVO
{
	public ZorkAchievementVO()
	{
	}

	public ZorkAchievementVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.AchievementTarget = xe.GetAttributeInt("AchievementTarget", -1);
		this.TargetNum = xe.GetAttributeInt("TargetNum", -1);
		this.ResetType = xe.GetAttributeInt("ResetType", -1);
		this.AchievementReward = xe.GetAttributeStr("AchievementReward").Split(new char[]
		{
			'|'
		});
		this.AchievementTitle = xe.GetAttributeStr("AchievementTitle");
	}

	public int ID { get; set; }

	public int AchievementTarget { get; set; }

	public int TargetNum { get; set; }

	public int ResetType { get; set; }

	public string[] AchievementReward { get; set; }

	public string AchievementTitle { get; set; }
}
