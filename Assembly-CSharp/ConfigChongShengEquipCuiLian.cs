using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ConfigChongShengEquipCuiLian : IConfigbase<ConfigChongShengEquipCuiLian>, ConfigBase
{
	public ConfigChongShengEquipCuiLian()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_DictEquipQuenchingVOCfg.Clear();
	}

	private void ParseEquipQuenchingVOXML()
	{
		if (this.m_DictEquipQuenchingVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.EquipQuenchingVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EquipQuenching");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EquipQuenchingVO equipQuenchingVO = new EquipQuenchingVO(xelementList[i]);
			this.m_DictEquipQuenchingVOCfg.Add(equipQuenchingVO.ID, equipQuenchingVO);
			if (!this.m_DicMaxLevel.ContainsKey((EquipCuiLian)equipQuenchingVO.PerfusionName))
			{
				this.m_DicMaxLevel.Add((EquipCuiLian)equipQuenchingVO.PerfusionName, equipQuenchingVO.PerfusionLevel);
			}
			else
			{
				this.m_DicMaxLevel[(EquipCuiLian)equipQuenchingVO.PerfusionName] = Mathf.Max(equipQuenchingVO.PerfusionLevel, this.m_DicMaxLevel[(EquipCuiLian)equipQuenchingVO.PerfusionName]);
			}
		}
	}

	public EquipQuenchingVO GetEquipQuenchingVODataById(int id)
	{
		this.ParseEquipQuenchingVOXML();
		EquipQuenchingVO result = null;
		if (this.m_DictEquipQuenchingVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetEquipQuenchingVODataById() 有误! id " + id
		});
		return null;
	}

	private Dictionary<int, EquipQuenchingVO> DictEquipQuenchingVOCfg
	{
		get
		{
			if (this.m_DictEquipQuenchingVOCfg.Count <= 0)
			{
				this.ParseEquipQuenchingVOXML();
			}
			return this.m_DictEquipQuenchingVOCfg;
		}
	}

	public string GetEquipName(EquipCuiLian equip)
	{
		string result = string.Empty;
		switch (equip)
		{
		case EquipCuiLian.touKui:
			result = Global.GetLang("头盔");
			break;
		case EquipCuiLian.huJia:
			result = Global.GetLang("护甲");
			break;
		case EquipCuiLian.huShou:
			result = Global.GetLang("护手");
			break;
		case EquipCuiLian.huTui:
			result = Global.GetLang("护腿");
			break;
		case EquipCuiLian.xieZi:
			result = Global.GetLang("鞋子");
			break;
		case EquipCuiLian.jieZhi1:
			result = Global.GetLang("戒指1");
			break;
		case EquipCuiLian.jieZhi2:
			result = Global.GetLang("戒指2");
			break;
		case EquipCuiLian.xiangLian:
			result = Global.GetLang("项链");
			break;
		case EquipCuiLian.shengWu:
			result = Global.GetLang("圣物");
			break;
		case EquipCuiLian.shengQi:
			result = Global.GetLang("圣器");
			break;
		}
		return result;
	}

	public ItemCategories GetEquipCateXml(EquipCuiLian equip)
	{
		ItemCategories result = ItemCategories.TouKui_ChongSheng;
		switch (equip)
		{
		case EquipCuiLian.touKui:
			result = ItemCategories.TouKui_ChongSheng;
			break;
		case EquipCuiLian.huJia:
			result = ItemCategories.KaiJia_ChongSheng;
			break;
		case EquipCuiLian.huShou:
			result = ItemCategories.HuShou_ChongSheng;
			break;
		case EquipCuiLian.huTui:
			result = ItemCategories.HuTui_ChongSheng;
			break;
		case EquipCuiLian.xieZi:
			result = ItemCategories.XueZi_ChongSheng;
			break;
		case EquipCuiLian.jieZhi1:
			result = ItemCategories.JieZhi_ChongSheng;
			break;
		case EquipCuiLian.jieZhi2:
			result = ItemCategories.JieZhi_ChongSheng;
			break;
		case EquipCuiLian.xiangLian:
			result = ItemCategories.XiangLian_ChongSheng;
			break;
		case EquipCuiLian.shengWu:
			result = ItemCategories.ShengWu_ChongSheng;
			break;
		case EquipCuiLian.shengQi:
			result = ItemCategories.ShengQi_ChongSheng;
			break;
		}
		return result;
	}

	public List<EquipCuiLianXmlData> ListEquipCuiLianCateData
	{
		get
		{
			if (this.m_ListEquipCuiLianXmlData.Count <= 0)
			{
				for (int i = 1; i < 11; i++)
				{
					EquipCuiLianXmlData equipCuiLianXmlData = new EquipCuiLianXmlData();
					equipCuiLianXmlData.CategoriesXml = this.GetEquipCateXml((EquipCuiLian)i);
					equipCuiLianXmlData.Name = this.GetEquipName((EquipCuiLian)i);
					equipCuiLianXmlData.CateCuiLian = (EquipCuiLian)i;
					this.m_ListEquipCuiLianXmlData.Add(equipCuiLianXmlData);
				}
			}
			return this.m_ListEquipCuiLianXmlData;
		}
	}

	public int GetMaxLevelByEquipKey(EquipCuiLian key)
	{
		if (this.m_DicMaxLevel.Count <= 0)
		{
			this.ParseEquipQuenchingVOXML();
		}
		if (this.m_DicMaxLevel.ContainsKey(key))
		{
			return this.m_DicMaxLevel[key];
		}
		return 99;
	}

	public RebornEquipData GetLevelByEquipKey(EquipCuiLian key, int roleID)
	{
		Dictionary<int, RebornEquipData> dictionary = null;
		if (Global.Data != null && roleID == Global.Data.RoleID)
		{
			dictionary = Global.Data.roleData.RebornEquipHoleData;
		}
		else if (Global.Data != null && Global.Data.OtherRoles != null && Global.Data.OtherRoles.ContainsKey(roleID))
		{
			dictionary = Global.Data.OtherRoles[roleID].RebornEquipHoleData;
		}
		if (dictionary != null && dictionary.ContainsKey((int)key))
		{
			return dictionary[(int)key];
		}
		return null;
	}

	public EquipQuenchingVO GetEquipQuenchingVODataByLevelAndKey(EquipCuiLian key, int level)
	{
		foreach (KeyValuePair<int, EquipQuenchingVO> keyValuePair in this.DictEquipQuenchingVOCfg)
		{
			if (keyValuePair.Value.PerfusionLevel == level)
			{
				Dictionary<int, EquipQuenchingVO>.Enumerator enumerator;
				KeyValuePair<int, EquipQuenchingVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.PerfusionName == (int)key)
				{
					KeyValuePair<int, EquipQuenchingVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public EquipCuiLianXmlData GetEquipByCategoriy(ItemCategories categories, HandTypes type)
	{
		EquipCuiLianXmlData result = null;
		for (int i = 0; i < this.ListEquipCuiLianCateData.Count; i++)
		{
			if (this.ListEquipCuiLianCateData[i].CategoriesXml == categories)
			{
				if (categories != ItemCategories.JieZhi_ChongSheng)
				{
					return this.ListEquipCuiLianCateData[i];
				}
				if (type == HandTypes.ZuoShou && this.ListEquipCuiLianCateData[i].CateCuiLian == EquipCuiLian.jieZhi1)
				{
					return this.ListEquipCuiLianCateData[i];
				}
				if (type == HandTypes.YouShou && this.ListEquipCuiLianCateData[i].CateCuiLian == EquipCuiLian.jieZhi2)
				{
					return this.ListEquipCuiLianCateData[i];
				}
			}
		}
		return result;
	}

	public string ErrorCuiLianString(RebornPerfusionOpcode e)
	{
		string lang = Global.GetLang("未知错误");
		switch (e)
		{
		case RebornPerfusionOpcode.NotStart:
			lang = Global.GetLang("功能未开启");
			break;
		case RebornPerfusionOpcode.NotExsit:
			lang = Global.GetLang("不存在当前槽位信息");
			break;
		case RebornPerfusionOpcode.MakeAbleErr:
			lang = Global.GetLang("系统出错");
			break;
		case RebornPerfusionOpcode.InsertDataErr:
			lang = Global.GetLang("数据出错");
			break;
		case RebornPerfusionOpcode.PerfusionNumErr:
			lang = Global.GetLang("灌注次数出错");
			break;
		case RebornPerfusionOpcode.PerfusionInfoErr:
			lang = Global.GetLang("灌注信息出错");
			break;
		case RebornPerfusionOpcode.UpdateDataErr:
			lang = Global.GetLang("更新数据出错");
			break;
		case RebornPerfusionOpcode.SuccNotVoll:
			lang = Global.GetLang("成功率不足");
			break;
		case RebornPerfusionOpcode.AbschreckenFail:
			lang = Global.GetLang("淬炼失败");
			break;
		case RebornPerfusionOpcode.AbschreckenXmlErr:
			lang = Global.GetLang("淬炼系统出错");
			break;
		case RebornPerfusionOpcode.MaxLevel:
			lang = Global.GetLang("已经到达最大等级");
			break;
		case RebornPerfusionOpcode.MaxAble:
			lang = Global.GetLang("灌注已达最大值");
			break;
		}
		return lang;
	}

	private string EquipQuenchingVOXMLPath = "Config/EquipQuenching.xml";

	private Dictionary<int, EquipQuenchingVO> m_DictEquipQuenchingVOCfg = new Dictionary<int, EquipQuenchingVO>();

	private List<EquipCuiLianXmlData> m_ListEquipCuiLianXmlData = new List<EquipCuiLianXmlData>();

	private Dictionary<EquipCuiLian, int> m_DicMaxLevel = new Dictionary<EquipCuiLian, int>();
}
