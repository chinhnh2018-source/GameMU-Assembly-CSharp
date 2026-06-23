using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CrusadeStoreXml
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
		XElement gameResXml = Global.GetGameResXml("Config/CrusadeStore.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "CrusadeStore");
			if (xelementList != null && 0 < xelementList.Count)
			{
				int i = 0;
				int count = xelementList.Count;
				while (i < count)
				{
					CrusadeStoreVO crusadeStoreVO = new CrusadeStoreVO();
					crusadeStoreVO.CopyForm(xelementList[i]);
					if (!this.dic.ContainsKey(crusadeStoreVO.ID))
					{
						this.dic[crusadeStoreVO.ID] = crusadeStoreVO;
					}
					else
					{
						this.dic.Add(crusadeStoreVO.ID, crusadeStoreVO);
					}
					i++;
				}
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=red>CrusadeStoreXML  初始化出错 Config/CrusadeStore.xml</color>"
			});
		}
	}

	public CrusadeStoreVO GetVOByID(int ID)
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
			"<color=red>CrusadeStoreXML  ID 获取不到 " + ID + "</color>"
		});
		return null;
	}

	public Dictionary<int, CrusadeStoreVO>.Enumerator GetEnumerator()
	{
		if (0 >= this.dic.Count)
		{
			this.initXml();
		}
		return this.dic.GetEnumerator();
	}

	internal void ClearData()
	{
		this.dic.Clear();
	}

	private const string Path = "Config/CrusadeStore.xml";

	private int mRecommendCount;

	private Dictionary<int, CrusadeStoreVO> dic = new Dictionary<int, CrusadeStoreVO>();
}
