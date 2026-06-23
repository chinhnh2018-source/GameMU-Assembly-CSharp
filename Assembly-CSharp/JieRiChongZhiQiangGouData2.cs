using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class JieRiChongZhiQiangGouData2
{
	public JieRiChongZhiQiangGouData2(XElement ele)
	{
		if (ele != null)
		{
			this.Description = Global.GetXElementAttributeStr(ele, "Description");
			List<XElement> xelementList = Global.GetXElementList(ele, "Award");
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (xelementList != null)
				{
					this._GiftList.Add(new JieRiChongZhiQiangGouData3(xelementList[i]));
				}
			}
		}
	}

	public List<JieRiChongZhiQiangGouData3> _GiftList = new List<JieRiChongZhiQiangGouData3>();

	public string Description;
}
