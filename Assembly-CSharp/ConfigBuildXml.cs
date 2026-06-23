using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigBuildXml
{
	public ConfigBuildXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Manor/Build.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Build");
			byte b = 0;
			while ((int)b < xelementList.Count)
			{
				BuildVO buildVO = new BuildVO();
				buildVO.CopyForm(xelementList[(int)b]);
				this.Dic[buildVO.ID] = buildVO;
				b += 1;
			}
		}
	}

	public BuildVO GetBuildVOByBuildID(int buildID)
	{
		if (this.Dic.ContainsKey(buildID))
		{
			return this.Dic[buildID];
		}
		return null;
	}

	public int[] GetBuildLevelArrayByBuildID(int buildID)
	{
		if (this.Dic.ContainsKey(buildID))
		{
			return this.Dic[buildID].DignLevel;
		}
		return null;
	}

	public string[] GetBuildPicArrayByBuildID(int buildID)
	{
		if (this.Dic.ContainsKey(buildID))
		{
			return this.Dic[buildID].PicArray;
		}
		return null;
	}

	public string GetBuildNameByBuildID(int BuildID)
	{
		if (this.Dic.ContainsKey(BuildID))
		{
			return this.Dic[BuildID].Name;
		}
		return null;
	}

	public int GetBuildMaxLevelByBuildID(int BuildID)
	{
		if (this.Dic.ContainsKey(BuildID))
		{
			return this.Dic[BuildID].MaxLevel;
		}
		return 1;
	}

	public List<BuildVO> GetBuildVOList()
	{
		List<BuildVO> list = new List<BuildVO>();
		Dictionary<int, BuildVO>.Enumerator enumerator = this.Dic.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<BuildVO> list2 = list;
			KeyValuePair<int, BuildVO> keyValuePair = enumerator.Current;
			list2.Add(keyValuePair.Value);
		}
		return list;
	}

	private Dictionary<int, BuildVO> Dic = new Dictionary<int, BuildVO>();
}
