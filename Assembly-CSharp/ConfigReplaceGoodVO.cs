using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ConfigReplaceGoodVO
{
	public static int GetReplaceGoodCount(int sourceGoodId, string toType, ref bool isBind, long toProperty = 0L)
	{
		isBind = false;
		ConfigReplaceGoodVO.InitReplaceGoodsXmlNodeDict();
		int num = 0;
		if (ConfigReplaceGoodVO.ReplaceGoodsDict != null)
		{
			foreach (KeyValuePair<int, ReplaceGoodVO> keyValuePair in ConfigReplaceGoodVO.ReplaceGoodsDict)
			{
				ReplaceGoodVO value = keyValuePair.Value;
				if (value.OldGoods == sourceGoodId && StringUtil.isEqualIgnoreCase(value.ToType, toType))
				{
					int newGoods = value.NewGoods;
					if (ConvertExt.SafeConvertToInt64(value.ToTypeProperty) <= toProperty)
					{
						int totalGoodsCountByID = Global.GetTotalGoodsCountByID(newGoods);
						if (totalGoodsCountByID > 0)
						{
							GoodsData goodsDataByID = Global.GetGoodsDataByID(newGoods);
							if (goodsDataByID != null && goodsDataByID.Binding == 1)
							{
								isBind = true;
							}
							num += totalGoodsCountByID;
						}
					}
				}
			}
		}
		return num;
	}

	public static void InitReplaceGoodsXmlNodeDict()
	{
		if (ConfigReplaceGoodVO.ReplaceGoodsDict == null)
		{
			ConfigReplaceGoodVO.ReplaceGoodsDict = new Dictionary<int, ReplaceGoodVO>();
			XElement gameResXml = Global.GetGameResXml("Config/ReplaceGoods.Xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "ReplaceGoods");
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					ReplaceGoodVO replaceGoodVO = new ReplaceGoodVO();
					replaceGoodVO.CopyFrom(xelement);
					ConfigReplaceGoodVO.ReplaceGoodsDict[replaceGoodVO.ID] = replaceGoodVO;
				}
			}
		}
	}

	public static void ClearData()
	{
		if (ConfigReplaceGoodVO.ReplaceGoodsDict != null)
		{
			ConfigReplaceGoodVO.ReplaceGoodsDict.Clear();
		}
	}

	public static Dictionary<int, ReplaceGoodVO> ReplaceGoodsDict;
}
