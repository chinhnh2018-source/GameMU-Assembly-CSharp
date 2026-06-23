using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigMengHuanSlateRewardPool : IConfigbase<ConfigMengHuanSlateRewardPool>, ConfigBase
{
	public ConfigMengHuanSlateRewardPool()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
	}

	public void ClearXMLData()
	{
		this.m_DictHuanMengSlateRewardPoolVOCfg.Clear();
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	private void ParseHuanMengSlateRewardPoolVOXML()
	{
		if (this.m_DictHuanMengSlateRewardPoolVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.HuanMengSlateRewardPoolVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuanMengSlateRewardPool");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HuanMengSlateRewardPoolVO huanMengSlateRewardPoolVO = new HuanMengSlateRewardPoolVO(xelementList[i]);
			this.m_DictHuanMengSlateRewardPoolVOCfg.Add(huanMengSlateRewardPoolVO.ID, huanMengSlateRewardPoolVO);
		}
	}

	public HuanMengSlateRewardPoolVO GetHuanMengSlateRewardPoolVODataById(int id)
	{
		this.ParseHuanMengSlateRewardPoolVOXML();
		HuanMengSlateRewardPoolVO result = null;
		if (this.m_DictHuanMengSlateRewardPoolVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetHuanMengSlateRewardPoolVODataById() 有误! id " + id
		});
		return null;
	}

	public HuanMengSlateRewardPoolVO GetDataByType(int type)
	{
		this.ParseHuanMengSlateRewardPoolVOXML();
		foreach (KeyValuePair<int, HuanMengSlateRewardPoolVO> keyValuePair in this.m_DictHuanMengSlateRewardPoolVOCfg)
		{
			string[] array = keyValuePair.Value.RandomPoolType.Split(new char[]
			{
				','
			});
			if (array.IndexOf(type.ToString()) != -1)
			{
				Dictionary<int, HuanMengSlateRewardPoolVO>.Enumerator enumerator;
				KeyValuePair<int, HuanMengSlateRewardPoolVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		return null;
	}

	public string GetError(int errorId)
	{
		string chineseText = string.Empty;
		switch (errorId)
		{
		case 2:
			chineseText = "参数错误";
			break;
		case 3:
			chineseText = "活动已结束，无法进行相应操作，请等待下次活动开启";
			break;
		case 4:
			chineseText = "配置错误";
			break;
		case 5:
			chineseText = "数据操作失败";
			break;
		case 6:
			chineseText = "没有获取到数据";
			break;
		case 7:
			chineseText = "抽取道具失败";
			break;
		case 8:
			chineseText = "无效的石板类型";
			break;
		case 9:
			chineseText = "幸运之星不足";
			break;
		case 10:
			chineseText = "背包不足";
			break;
		case 11:
			chineseText = "检查石板错误";
			break;
		case 12:
			chineseText = "系统不包含物品";
			break;
		case 13:
			chineseText = "抽取错误";
			break;
		}
		return Global.GetLang(chineseText);
	}

	private string HuanMengSlateRewardPoolVOXMLPath = "Config/HuanMengSlateRewardPool.xml";

	private Dictionary<int, HuanMengSlateRewardPoolVO> m_DictHuanMengSlateRewardPoolVOCfg = new Dictionary<int, HuanMengSlateRewardPoolVO>();
}
