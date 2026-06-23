using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class LianSaiLeagueSuperAwardVO
{
	public LianSaiLeagueSuperAwardVO(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.BeginNum = Global.GetXElementAttributeInt(xml, "BeginNum");
			this.EndNum = Global.GetXElementAttributeInt(xml, "EndNum");
			this.mGoodsOne = Global.GetXElementAttributeStr(xml, "GoodsOne");
		}
	}

	public List<GoodsData> Goods
	{
		get
		{
			List<GoodsData> list = new List<GoodsData>();
			if (!string.IsNullOrEmpty(this.mGoodsOne))
			{
				string[] array = this.mGoodsOne.Split(new char[]
				{
					'|'
				});
				if (array != null && 0 < array.Length)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 7)
							{
								GoodsData emptyGoodsData = Global.GetEmptyGoodsData(array2[0].SafeToInt32(0), 0, array2[2].SafeToInt32(0), array2[3].SafeToInt32(0), array2[1].SafeToInt32(0), array2[5].SafeToInt32(0), array2[6].SafeToInt32(0), 0, 0);
								if (emptyGoodsData != null)
								{
									list.Add(emptyGoodsData);
								}
							}
						}
					}
				}
			}
			return list;
		}
	}

	public int ID;

	public string Name;

	public int BeginNum;

	public int EndNum;

	private string mGoodsOne;
}
