using System;
using Tmsk.Xml;

public class ZorkSceneVO
{
	public ZorkSceneVO()
	{
	}

	public ZorkSceneVO(XElement xe)
	{
		this.BuffAreID = xe.GetAttributeInt("BuffAreID", -1);
		this.BuffArePlace = new int[2];
		string attributeStr = xe.GetAttributeStr("BuffArePlace");
		string[] array = attributeStr.Split(new char[]
		{
			'|'
		});
		if (array != null)
		{
			string[] array2 = array[0].Split(new char[]
			{
				','
			});
			if (array2.Length > 1)
			{
				for (int i = 0; i < 2; i++)
				{
					this.BuffArePlace[i] = array2[i].SafeToInt32(0);
				}
			}
		}
		this.ArmyType = xe.GetAttributeInt("ArmyType", -1);
		this.ArmyRefreshRange = xe.GetAttributeFloat("ArmyRefreshRange", -1f);
		this.ArmyGroupRound = xe.GetAttributeStr("ArmyGroupRound");
		this.GuardGroupID = xe.GetAttributeStr("GuardGroupID");
		this.FirstArmyTime = xe.GetAttributeInt("FirstArmyTime", -1);
		this.NextArmyRefresTime = xe.GetAttributeInt("NextArmyRefresTime", -1);
	}

	public int BuffAreID { get; set; }

	public int[] BuffArePlace { get; set; }

	public int ArmyType { get; set; }

	public float ArmyRefreshRange { get; set; }

	public string ArmyGroupRound { get; set; }

	public string GuardGroupID { get; set; }

	public int FirstArmyTime { get; set; }

	public int NextArmyRefresTime { get; set; }
}
