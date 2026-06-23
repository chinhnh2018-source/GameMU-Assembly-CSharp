using System;
using System.Collections.Generic;

public class SoulCometStoneGroupItemAttribute
{
	public SoulCometStoneGroupItemAttribute()
	{
		this.list_groupItemAttr = new List<SoulCometStoneGroupGoodsItemAttribute>();
	}

	public bool perfect;

	public string properties = string.Empty;

	public List<SoulCometStoneGroupGoodsItemAttribute> list_groupItemAttr;
}
