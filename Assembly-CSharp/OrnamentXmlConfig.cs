using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class OrnamentXmlConfig
{
	public OrnamentXmlConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Ornament.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ornament");
			if (xelementList != null && 0 < xelementList.Count)
			{
				int count = xelementList.Count;
				for (int i = 0; i < count; i++)
				{
					if (xelementList[i] != null)
					{
						OrnamentXmlData ornamentXmlData = new OrnamentXmlData(xelementList[i]);
						this.dic_OrnamentXmlData.Add(ornamentXmlData.GoodsID, ornamentXmlData);
					}
				}
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				string.Format("xml配置有误{0}表得到失败", "Config/Ornament.xml")
			});
		}
	}

	public OrnamentXmlData GetXmlByGoodsId(int GoodsId)
	{
		if (this.dic_OrnamentXmlData.ContainsKey(GoodsId))
		{
			return this.dic_OrnamentXmlData[GoodsId];
		}
		return null;
	}

	public List<OrnamentXmlData> GetOrnamentXmlDataLst()
	{
		List<OrnamentXmlData> list = new List<OrnamentXmlData>();
		foreach (KeyValuePair<int, OrnamentXmlData> keyValuePair in this.dic_OrnamentXmlData)
		{
			if (keyValuePair.Value != null)
			{
				List<OrnamentXmlData> list2 = list;
				Dictionary<int, OrnamentXmlData>.Enumerator enumerator;
				KeyValuePair<int, OrnamentXmlData> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		return list;
	}

	public List<OrnamentXmlData> GetOrnamentXmlDataLst(int Type)
	{
		List<OrnamentXmlData> list = new List<OrnamentXmlData>();
		foreach (KeyValuePair<int, OrnamentXmlData> keyValuePair in this.dic_OrnamentXmlData)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, OrnamentXmlData>.Enumerator enumerator;
				KeyValuePair<int, OrnamentXmlData> keyValuePair2 = enumerator.Current;
				if (Type == keyValuePair2.Value.Type)
				{
					List<OrnamentXmlData> list2 = list;
					KeyValuePair<int, OrnamentXmlData> keyValuePair3 = enumerator.Current;
					list2.Add(keyValuePair3.Value);
				}
			}
		}
		return list;
	}

	public List<OrnamentXmlData> GetOrnamentXmlDataLst(List<int> List)
	{
		List<OrnamentXmlData> list = new List<OrnamentXmlData>();
		try
		{
			foreach (KeyValuePair<int, OrnamentXmlData> keyValuePair in this.dic_OrnamentXmlData)
			{
				if (keyValuePair.Value != null)
				{
					Dictionary<int, OrnamentXmlData>.Enumerator enumerator;
					KeyValuePair<int, OrnamentXmlData> keyValuePair2 = enumerator.Current;
					if (List.Contains(keyValuePair2.Value.GoalType))
					{
						List<OrnamentXmlData> list2 = list;
						KeyValuePair<int, OrnamentXmlData> keyValuePair3 = enumerator.Current;
						list2.Add(keyValuePair3.Value);
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
		return list;
	}

	private Dictionary<int, OrnamentXmlData> dic_OrnamentXmlData = new Dictionary<int, OrnamentXmlData>();
}
