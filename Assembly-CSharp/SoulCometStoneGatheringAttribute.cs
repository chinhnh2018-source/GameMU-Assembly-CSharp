using System;

public class SoulCometStoneGatheringAttribute
{
	public SoulCometStoneGatheringAttribute()
	{
		this.list_extraGoodsNeed = new string[]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};
	}

	public int id;

	public int groupLevel;

	public int type;

	public int iconCode;

	public int needSoulCometPowder;

	public string[] list_extraGoodsNeed;
}
