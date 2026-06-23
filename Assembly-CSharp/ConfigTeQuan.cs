using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ConfigTeQuan : IConfigbase<ConfigTeQuan>, ConfigBase
{
	public ConfigTeQuan()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public void GetDataFormSever(DPSelectedItemEventHandler getDataCallBack)
	{
		this.GetDataCallBack = getDataCallBack;
		if (this.mJieriXmlData == null)
		{
			GameInstance.Game.SendGetRoleTeQuanActivityXMLData(-1);
		}
		else
		{
			GameInstance.Game.SendGetRoleTeQuanActivityXMLData(this.mJieriXmlData.Version);
		}
	}

	public void NoticeGetDataCallBack(JieriXmlData xmlData)
	{
		byte b = 0;
		if (this.mJieriXmlData == null)
		{
			this.mJieriXmlData = xmlData;
			b = 1;
		}
		else if (this.mJieriXmlData.Version != xmlData.Version)
		{
			this.mJieriXmlData = xmlData;
			b = 1;
		}
		if (b == 1)
		{
			this.ClearXMLData(0);
			if (this.mJieriXmlData.XmlList != null)
			{
				for (int i = 0; i < this.mJieriXmlData.XmlList.Count; i++)
				{
					if (i < this.XMLName.Length)
					{
						if (this.mXMLData.ContainsKey(this.XMLName[i]))
						{
							this.mXMLData[this.XMLName[i]] = this.mJieriXmlData.XmlList[i];
						}
						else
						{
							this.mXMLData.Add(this.XMLName[i], this.mJieriXmlData.XmlList[i]);
						}
					}
				}
			}
		}
		if (this.GetDataCallBack != null)
		{
			this.GetDataCallBack(null, null);
		}
	}

	public XElement GetTeQuanXml(string path)
	{
		string onlyFileName = XmlManager.GetOnlyFileName(path);
		if (!string.IsNullOrEmpty(onlyFileName) && this.mXMLData.ContainsKey(onlyFileName) && !string.IsNullOrEmpty(this.mXMLData[onlyFileName]))
		{
			return XElement.Parse(this.mXMLData[onlyFileName]);
		}
		return Global.GetGameResXml(path);
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
		if (this.mTeQuanJianLiXML != null)
		{
			this.mTeQuanJianLiXML = null;
		}
		if (this.mTeQuanJiHuoXML != null)
		{
			this.mTeQuanJiHuoXML = null;
		}
		if (this.mTeQuanShangChengXML != null)
		{
			this.mTeQuanShangChengXML = null;
		}
		if (this.mTeQuanTiaoJianXml != null)
		{
			this.mTeQuanTiaoJianXml = null;
		}
		if (this.mTeQuanZhiGouXML != null)
		{
			this.mTeQuanZhiGouXML = null;
		}
		if (this.mTeQuanBuffXML != null)
		{
			this.mTeQuanBuffXML = null;
		}
	}

	private void initTeQuanTiaoJianXml()
	{
		if (this.mTeQuanTiaoJianXml == null)
		{
			this.mTeQuanTiaoJianXml = new TeQuanTiaoJianXml();
		}
	}

	public BetterList<TeQuanTiaoJianVO> GetTeQuanTiaoJianVOList()
	{
		this.initTeQuanTiaoJianXml();
		return this.mTeQuanTiaoJianXml.GetTeQuanTiaoJianVOList();
	}

	public TeQuanTiaoJianVO GetTeQuanTiaoJianVOByID(int ID)
	{
		this.initTeQuanTiaoJianXml();
		return this.mTeQuanTiaoJianXml.GetTeQuanTiaoJianVOByID(ID);
	}

	private void initTeQuanJiHuoXML()
	{
		if (this.mTeQuanJiHuoXML == null)
		{
			this.mTeQuanJiHuoXML = new TeQuanJiHuoXML();
		}
	}

	public TeQuanJiHuoVO GetTeQuanJiHuoVOByID(int ID)
	{
		this.initTeQuanJiHuoXML();
		return this.mTeQuanJiHuoXML.GetTeQuanJiHuoVOByID(ID);
	}

	private void InitTeQuanShangChengXML()
	{
		if (this.mTeQuanShangChengXML == null)
		{
			this.mTeQuanShangChengXML = new TeQuanShangChengXML();
		}
	}

	public BetterList<TeQuanShangChengVO> GetShangChengVOItemsBuyTeQuanID(int TeQuanID)
	{
		this.InitTeQuanShangChengXML();
		return this.mTeQuanShangChengXML.GetShangChengVOItemsBuyTeQuanID(TeQuanID);
	}

	private void InitTeQuanJiangLi()
	{
		if (this.mTeQuanJianLiXML == null)
		{
			this.mTeQuanJianLiXML = new TeQuanJianLiXML();
		}
	}

	public int GetZhongJiJiangLiTiaoJianByID(int id)
	{
		this.InitTeQuanJiangLi();
		TeQuanJianLiVO itemById = this.mTeQuanJianLiXML.GetItemById(id);
		if (itemById != null)
		{
			return itemById.LingQuTiaoJian;
		}
		return 0;
	}

	public List<GoodsData> GetZhongJiJiangLiGoods(int ID)
	{
		List<GoodsData> list = new List<GoodsData>();
		this.InitTeQuanJiangLi();
		TeQuanJianLiVO itemById = this.mTeQuanJianLiXML.GetItemById(ID);
		if (itemById != null && !string.IsNullOrEmpty(itemById.WuPinID))
		{
			string[] array = itemById.WuPinID.Split(new char[]
			{
				'|'
			});
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array[i], 0);
						if (goodsDataByStr != null)
						{
							list.Add(goodsDataByStr);
						}
					}
				}
			}
		}
		return list;
	}

	private void InitTeQuanZhiGouXML()
	{
		if (this.mTeQuanZhiGouXML == null)
		{
			this.mTeQuanZhiGouXML = new TeQuanZhiGouXML();
		}
	}

	public BetterList<TeQuanZhiGouVO> GetZhiGouVOItems()
	{
		this.InitTeQuanZhiGouXML();
		return this.mTeQuanZhiGouXML.GetItems();
	}

	public TeQuanZhiGouVO GetZhiGouVOItemByID(int ID)
	{
		this.InitTeQuanZhiGouXML();
		return this.mTeQuanZhiGouXML.GetItem(ID);
	}

	public BetterList<TeQuanZhiGouVO> GetZhiGouVOItems(int TeQuanID)
	{
		this.InitTeQuanZhiGouXML();
		return this.mTeQuanZhiGouXML.GetItemsByTeQuanID(TeQuanID);
	}

	private void InitTeQuanBuffXML()
	{
		if (this.mTeQuanBuffXML == null)
		{
			this.mTeQuanBuffXML = new TeQuanBuffXML();
		}
	}

	public BetterList<TeQuanBuffVO> GetTeQuanBuffVOsByTeQuanId(int TeQuanID)
	{
		this.InitTeQuanBuffXML();
		return this.mTeQuanBuffXML.GetTeQuanBuffVOItemsBuyTeQuanID(TeQuanID);
	}

	public DPSelectedItemEventHandler GetDataCallBack;

	private JieriXmlData mJieriXmlData;

	private string[] XMLName = new string[]
	{
		"TeQuanTiaoJian",
		"TeQuanJiHuo",
		"TeQuanBoss",
		"TeQuanJianLi",
		"TeQuanZhiGou",
		"TeQuanShangCheng"
	};

	private Dictionary<string, string> mXMLData = new Dictionary<string, string>();

	private TeQuanTiaoJianXml mTeQuanTiaoJianXml;

	private TeQuanJiHuoXML mTeQuanJiHuoXML;

	private TeQuanShangChengXML mTeQuanShangChengXML;

	private TeQuanJianLiXML mTeQuanJianLiXML;

	private TeQuanZhiGouXML mTeQuanZhiGouXML;

	private TeQuanBuffXML mTeQuanBuffXML;
}
