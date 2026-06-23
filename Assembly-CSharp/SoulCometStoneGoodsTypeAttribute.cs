using System;

public class SoulCometStoneGoodsTypeAttribute
{
	public SoulCometStoneGoodsTypeAttribute(int id, string type, int defaultGoodsID)
	{
		this.id = id;
		this.type = type;
		this.defaultGoodsID = defaultGoodsID;
	}

	public int id;

	public string type;

	public int defaultGoodsID;
}
