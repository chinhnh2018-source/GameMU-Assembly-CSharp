using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CrusadeWorldXML
{
	public int RecommendCount
	{
		get
		{
			return this.mRecommendCount;
		}
		set
		{
			this.mRecommendCount = value;
		}
	}

	private void initXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/CrusadeWorld.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "CrusadeWorld");
			if (xelementList != null && 0 < xelementList.Count)
			{
				int i = 0;
				int count = xelementList.Count;
				while (i < count)
				{
					CrusadeWorldVO crusadeWorldVO = new CrusadeWorldVO();
					crusadeWorldVO.CopyForm(xelementList[i]);
					if (!this.dic.ContainsKey(crusadeWorldVO.ID))
					{
						this.dic[crusadeWorldVO.ID] = crusadeWorldVO;
					}
					else
					{
						this.dic.Add(crusadeWorldVO.ID, crusadeWorldVO);
					}
					i++;
				}
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=red>CrusadeWorldXML  初始化出错 Config/CrusadeWorld.xml</color>"
			});
		}
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	public CrusadeWorldVO GetVOById(int ID)
	{
		if (0 >= this.dic.Count)
		{
			this.initXml();
		}
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		MUDebug.Log<string>(new string[]
		{
			"<color=red>CrusadeWorldVO  加载出错误ID " + ID + "</color>"
		});
		return null;
	}

	private const string Path = "Config/CrusadeWorld.xml";

	private int mRecommendCount;

	private Dictionary<int, CrusadeWorldVO> dic = new Dictionary<int, CrusadeWorldVO>();
}
