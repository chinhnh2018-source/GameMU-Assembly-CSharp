using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TarotDataAndXmlData
{
	public TarotCardData data
	{
		get
		{
			return this._data;
		}
		set
		{
			this._data = value;
			if (this.xmlData != null)
			{
				this.xmlData.Level = this._data.Level;
			}
		}
	}

	public string GetNextLevelNeesGoods()
	{
		string result = string.Empty;
		if (this.xmlData != null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Tarot.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "Tarot", "GoodsID", this.xmlData.GoodsID.ToString());
				if (xelementList != null)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Level");
						if (this.xmlData.Level + 1 == xelementAttributeInt)
						{
							result = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
							break;
						}
					}
				}
			}
		}
		return result;
	}

	public int GetGoodsID()
	{
		int result = 0;
		if (this._data != null)
		{
			result = this._data.GoodId;
		}
		else if (this.xmlData != null)
		{
			result = this.xmlData.GoodsID;
		}
		return result;
	}

	private TarotCardData _data;

	public TarotXmlData xmlData;
}
