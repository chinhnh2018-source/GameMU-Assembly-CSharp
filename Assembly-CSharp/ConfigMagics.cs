using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigMagics
{
	private static void initDict(int OccupationID)
	{
		if (ConfigMagics.MagicsNodeDict.Count <= 0)
		{
			string xmlName = string.Format("Config/Magics/Magics_{0}.xml", OccupationID);
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Config Maigcs is null"
				});
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Magic");
			if (xelementList == null || xelementList.Count <= 0)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Config Maigcs is null"
				});
				return;
			}
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				List<XElement> xelementList2 = Global.GetXElementList(xelement, "JiNeng");
				if (xelementList2 != null && !ConfigMagics.MagicsNodeDict.ContainsKey(xelementAttributeInt))
				{
					Dictionary<int, MagicItemVO> dictionary = new Dictionary<int, MagicItemVO>();
					ConfigMagics.MagicsNodeDict[xelementAttributeInt] = dictionary;
					int count2 = xelementList2.Count;
					for (int j = 0; j < count2; j++)
					{
						XElement xml = xelementList2[j];
						MagicItemVO magicItemVO = new MagicItemVO();
						magicItemVO.CopyFrom(xml);
						int level = magicItemVO.Level;
						if (!dictionary.ContainsKey(level))
						{
							dictionary[level] = magicItemVO;
						}
					}
				}
			}
		}
	}

	public static MagicItemVO GetMagicItemVO(int OccupationID, int magicId, int jinengLevel)
	{
		ConfigMagics.initDict(OccupationID);
		MagicItemVO result = null;
		if (ConfigMagics.MagicsNodeDict != null && ConfigMagics.MagicsNodeDict.ContainsKey(magicId))
		{
			Dictionary<int, MagicItemVO> dictionary = ConfigMagics.MagicsNodeDict[magicId];
			if (dictionary != null && dictionary.ContainsKey(jinengLevel))
			{
				result = dictionary[jinengLevel];
			}
		}
		return result;
	}

	public static void ClearData()
	{
		ConfigMagics.MagicsNodeDict.Clear();
	}

	public const string GAME_CONFIG_MAGIC_0 = "Config/Magics/Magics_0.xml";

	public const string GAME_CONFIG_MAGIC_1 = "Config/Magics/Magics_1.xml";

	public const string GAME_CONFIG_MAGIC_2 = "Config/Magics/Magics_2.xml";

	public const string GAME_CONFIG_MAGIC_3 = "Config/Magics/Magics_3.xml";

	public const string GAME_CONFIG_MAGIC_5 = "Config/Magics/Magics_5.xml";

	public static Dictionary<int, Dictionary<int, MagicItemVO>> MagicsNodeDict = new Dictionary<int, Dictionary<int, MagicItemVO>>();
}
