using System;
using Server.Data;

public class SoulCometStoneGroupGoodsItemAttribute
{
	public SoulCometStoneGroupGoodsItemAttribute(bool gray, GoodsData goodsData)
	{
		this.gray = gray;
		this.goodsData = goodsData;
	}

	public bool gray;

	public GoodsData goodsData;
}
