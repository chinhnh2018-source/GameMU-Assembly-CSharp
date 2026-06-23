using System;
using System.Collections.Generic;
using Tmsk.Xml;

public class ChongShengYinJiConfigManager : ParseXmlCfg
{
	public static ChongShengYinJiConfigManager GetInstance()
	{
		if (ChongShengYinJiConfigManager.instance == null)
		{
			ChongShengYinJiConfigManager.instance = new ChongShengYinJiConfigManager();
		}
		return ChongShengYinJiConfigManager.instance;
	}

	public Dictionary<int, ZhuYinJiCfgData> GetDictZhuYinJiCfgData()
	{
		if (this.DictZhuYinJiCfgData.Count <= 0)
		{
			this.LoadZhuYinJiXML();
		}
		return this.DictZhuYinJiCfgData;
	}

	public ZhuYinJiCfgData GetZhuYinJiCfgDataByIDAndType(int id, int type)
	{
		if (this.DictZhuYinJiCfgData.Count <= 0)
		{
			this.LoadZhuYinJiXML();
		}
		return this.GetZhuYinJiCfgDataByDict(id, type);
	}

	public ZhuYinJiCfgData GetZhuYinJiCfgDataByID(int id)
	{
		if (this.DictZhuYinJiCfgData.Count <= 0)
		{
			this.LoadZhuYinJiXML();
		}
		return this.GetZhuYinJiCfgDataByDict(id, -1);
	}

	private void LoadZhuYinJiXML()
	{
		List<XElement> xelements = base.GetXElements(this.ChongShengYinJiZhuCfgName, "Rose");
		if (xelements != null && xelements.Count > 0)
		{
			for (int i = 0; i < xelements.Count; i++)
			{
				ZhuYinJiCfgData zhuYinJiCfgData = new ZhuYinJiCfgData(xelements[i]);
				this.DictZhuYinJiCfgData.Add(zhuYinJiCfgData.ID, zhuYinJiCfgData);
			}
		}
	}

	private ZhuYinJiCfgData GetZhuYinJiCfgDataByDict(int id, int type = -1)
	{
		ZhuYinJiCfgData zhuYinJiCfgData = null;
		if (this.DictZhuYinJiCfgData.TryGetValue(id, ref zhuYinJiCfgData))
		{
			if (type == -1)
			{
				return zhuYinJiCfgData;
			}
			if (zhuYinJiCfgData.TypeZhu == type)
			{
				return zhuYinJiCfgData;
			}
		}
		return null;
	}

	public Dictionary<int, ZiYinJiCfgData> GetDictZiYinJiCfgData()
	{
		if (this.DictZiYinJiCfgData.Count <= 0)
		{
			this.LoadZiYinJiXML();
		}
		return this.DictZiYinJiCfgData;
	}

	public ZiYinJiCfgData GetZiYinJiCfgDataByIDAndType(int id, int type)
	{
		if (this.DictZiYinJiCfgData.Count <= 0)
		{
			this.LoadZiYinJiXML();
		}
		return this.GetZiYinJiCfgDataByDict(id, type);
	}

	public ZiYinJiCfgData GetZiYinJiCfgDataByID(int id)
	{
		if (this.DictZiYinJiCfgData.Count <= 0)
		{
			this.LoadZiYinJiXML();
		}
		return this.GetZiYinJiCfgDataByDict(id, -1);
	}

	private void LoadZiYinJiXML()
	{
		List<XElement> xelements = base.GetXElements(this.ChongShengYinJiZiCfgName, "Rose");
		if (xelements != null && xelements.Count > 0)
		{
			for (int i = 0; i < xelements.Count; i++)
			{
				ZiYinJiCfgData ziYinJiCfgData = new ZiYinJiCfgData(xelements[i]);
				this.DictZiYinJiCfgData.Add(ziYinJiCfgData.ID, ziYinJiCfgData);
			}
		}
	}

	private ZiYinJiCfgData GetZiYinJiCfgDataByDict(int id, int type = -1)
	{
		ZiYinJiCfgData ziYinJiCfgData = null;
		if (this.DictZiYinJiCfgData.TryGetValue(id, ref ziYinJiCfgData))
		{
			if (type == -1)
			{
				return ziYinJiCfgData;
			}
			if (ziYinJiCfgData.Type == type)
			{
				return ziYinJiCfgData;
			}
		}
		return null;
	}

	public void Clear()
	{
		this.DictZhuYinJiCfgData.Clear();
		this.DictZiYinJiCfgData.Clear();
	}

	private static ChongShengYinJiConfigManager instance;

	private string ChongShengYinJiZhuCfgName = "Config/ShenShengYinJiZhu.xml";

	private Dictionary<int, ZhuYinJiCfgData> DictZhuYinJiCfgData = new Dictionary<int, ZhuYinJiCfgData>();

	private string ChongShengYinJiZiCfgName = "Config/ShenShengYinJiZi.xml";

	private Dictionary<int, ZiYinJiCfgData> DictZiYinJiCfgData = new Dictionary<int, ZiYinJiCfgData>();
}
