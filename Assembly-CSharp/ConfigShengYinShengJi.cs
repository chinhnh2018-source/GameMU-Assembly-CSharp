using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ConfigShengYinShengJi : IConfigbase<ConfigShengYinShengJi>, ConfigBase
{
	public ConfigShengYinShengJi()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_DictShengYinTaoZhuangVOCfg.Clear();
		this.m_DictShengYinShengJiVOCfg.Clear();
	}

	private void ParseShengYinShengJiVOXML()
	{
		if (this.m_DictShengYinShengJiVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ShengYinShengJiVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShengYinShengJi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ShengYinShengJiVO shengYinShengJiVO = new ShengYinShengJiVO(xelementList[i]);
			this.m_DictShengYinShengJiVOCfg.Add(shengYinShengJiVO.ID, shengYinShengJiVO);
		}
	}

	public ShengYinShengJiVO GetShengYinShengJiVODataByGoodsId(int id)
	{
		this.ParseShengYinShengJiVOXML();
		foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.DaoJuID == id)
				{
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public ShengYinShengJiVO GetShengYinShengJiVODataByGoodsIdAndLevel(int id, int level)
	{
		this.ParseShengYinShengJiVOXML();
		foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.DaoJuID == id)
				{
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
					if (keyValuePair3.Value.DengJi == -1)
					{
						KeyValuePair<int, ShengYinShengJiVO> keyValuePair4 = enumerator.Current;
						return keyValuePair4.Value;
					}
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair5 = enumerator.Current;
					if (keyValuePair5.Value.DengJi == level)
					{
						KeyValuePair<int, ShengYinShengJiVO> keyValuePair6 = enumerator.Current;
						return keyValuePair6.Value;
					}
				}
			}
		}
		return null;
	}

	public ShengYinShengJiVO GetShengYinShengJiVODataById(int id)
	{
		this.ParseShengYinShengJiVOXML();
		ShengYinShengJiVO result = null;
		if (this.m_DictShengYinShengJiVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetShengYinShengJiVODataById() 有误! id " + id
		});
		return null;
	}

	public Dictionary<int, ShengYinShengJiVO>.Enumerator GetShengYinShengJiEnumerator()
	{
		this.ParseShengYinShengJiVOXML();
		return this.m_DictShengYinShengJiVOCfg.GetEnumerator();
	}

	public int GetCaoWeiIndexByGoodsID(int GoodsID)
	{
		this.ParseShengYinShengJiVOXML();
		ShengYinShengJiVO shengYinShengJiVODataByGoodsId = this.GetShengYinShengJiVODataByGoodsId(GoodsID);
		if (shengYinShengJiVODataByGoodsId != null)
		{
			return shengYinShengJiVODataByGoodsId.BuWei;
		}
		return 0;
	}

	public bool GetSoltISOpenByRideLevel(int SlotId)
	{
		this.ParseShengYinShengJiVOXML();
		if (SlotId == 1)
		{
			return true;
		}
		if (this.mZuoQiMainData != null)
		{
			int soltOpenLevel = this.GetSoltOpenLevel(SlotId);
			if (this.mZuoQiMainData.MountLevel + 1 >= soltOpenLevel)
			{
				return true;
			}
		}
		return false;
	}

	public int GetSoltOpenLevel(int SlotId)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ShengYinJieSuo", '|');
		int result = 0;
		byte b = 0;
		while ((int)b < systemParamStringArrayByName.Length)
		{
			if (systemParamStringArrayByName[(int)b].Split(new char[]
			{
				','
			})[0].SafeToInt32(0) == SlotId)
			{
				result = systemParamStringArrayByName[(int)b].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
				break;
			}
			b += 1;
		}
		return result;
	}

	public void NoticeGetZuoQiDataCallBack(ZuoQiMainData data)
	{
		this.mZuoQiMainData = data;
		if (this.Hander != null)
		{
			this.Hander(null, null);
		}
	}

	public void GetData(DPSelectedItemEventHandler hander)
	{
		this.Hander = hander;
		GameInstance.Game.GetRidePetMainData();
	}

	public int GetShengJiNeedXEPByGoodsIDAndLevel(int GoodsID, int Level)
	{
		this.ParseShengYinShengJiVOXML();
		foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.DaoJuID == GoodsID)
				{
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
					if (keyValuePair3.Value.DengJi == Level)
					{
						KeyValuePair<int, ShengYinShengJiVO> keyValuePair4 = enumerator.Current;
						return keyValuePair4.Value.ShengJiJingYan;
					}
				}
			}
		}
		return 0;
	}

	public Dictionary<string, double> GetPropByGoodsIDAndLevel(int GoodsID, int Level)
	{
		this.ParseShengYinShengJiVOXML();
		foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.DaoJuID == GoodsID)
				{
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
					if (keyValuePair3.Value.DengJi == Level)
					{
						KeyValuePair<int, ShengYinShengJiVO> keyValuePair4 = enumerator.Current;
						return keyValuePair4.Value.JiChuProp;
					}
				}
			}
		}
		return null;
	}

	public int GetMaxLevelByGoodsID(int GoodsID)
	{
		this.ParseShengYinShengJiVOXML();
		if (0 >= this.mMaxLevel)
		{
			foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
			{
				if (keyValuePair.Value != null)
				{
					Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value.DaoJuID == GoodsID)
					{
						int num = this.mMaxLevel;
						KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
						if (num < keyValuePair3.Value.DengJi)
						{
							KeyValuePair<int, ShengYinShengJiVO> keyValuePair4 = enumerator.Current;
							this.mMaxLevel = keyValuePair4.Value.DengJi;
						}
					}
				}
			}
		}
		return this.mMaxLevel;
	}

	public int GetLeiXingByGoodsID(int GoodsID)
	{
		this.ParseShengYinShengJiVOXML();
		foreach (KeyValuePair<int, ShengYinShengJiVO> keyValuePair in this.m_DictShengYinShengJiVOCfg)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<int, ShengYinShengJiVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.DaoJuID == GoodsID)
				{
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value.LeiXing;
				}
			}
		}
		return -1;
	}

	private void ParseShengYinTaoZhuangVOXML()
	{
		if (this.m_DictShengYinTaoZhuangVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ShengYinTaoZhuangVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShengYinTaoZhuang");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ShengYinTaoZhuangVO shengYinTaoZhuangVO = new ShengYinTaoZhuangVO(xelementList[i]);
			this.m_DictShengYinTaoZhuangVOCfg.Add(shengYinTaoZhuangVO.ID, shengYinTaoZhuangVO);
		}
	}

	public ShengYinTaoZhuangVO GetShengYinTaoZhuangVODataById(int id)
	{
		this.ParseShengYinTaoZhuangVOXML();
		ShengYinTaoZhuangVO result = null;
		if (this.m_DictShengYinTaoZhuangVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetShengYinTaoZhuangVODataById() 有误! id " + id
		});
		return null;
	}

	public ShengYinTaoZhuangVO GetShengYinTaoZhuangVODataByLeiXing(int LeiXing)
	{
		this.ParseShengYinTaoZhuangVOXML();
		foreach (KeyValuePair<int, ShengYinTaoZhuangVO> keyValuePair in this.m_DictShengYinTaoZhuangVOCfg)
		{
			if (keyValuePair.Value.LeiXing == LeiXing)
			{
				Dictionary<int, ShengYinTaoZhuangVO>.Enumerator enumerator;
				KeyValuePair<int, ShengYinTaoZhuangVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		return null;
	}

	public List<int> GetTaoZhuangLeiXingList()
	{
		List<int> list = new List<int>();
		this.ParseShengYinTaoZhuangVOXML();
		Dictionary<int, ShengYinTaoZhuangVO>.Enumerator enumerator = this.m_DictShengYinTaoZhuangVOCfg.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<int> list2 = list;
			KeyValuePair<int, ShengYinTaoZhuangVO> keyValuePair = enumerator.Current;
			if (!list2.Contains(keyValuePair.Value.LeiXing))
			{
				List<int> list3 = list;
				KeyValuePair<int, ShengYinTaoZhuangVO> keyValuePair2 = enumerator.Current;
				list3.Add(keyValuePair2.Value.LeiXing);
			}
		}
		return list;
	}

	private string ShengYinShengJiVOXMLPath = "Config/ShengYinShengJi.xml";

	private Dictionary<int, ShengYinShengJiVO> m_DictShengYinShengJiVOCfg = new Dictionary<int, ShengYinShengJiVO>();

	public DPSelectedItemEventHandler Hander;

	private int mMaxLevel;

	private ZuoQiMainData mZuoQiMainData;

	private string ShengYinTaoZhuangVOXMLPath = "Config/ShengYinTaoZhuang.xml";

	private Dictionary<int, ShengYinTaoZhuangVO> m_DictShengYinTaoZhuangVOCfg = new Dictionary<int, ShengYinTaoZhuangVO>();
}
