using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigDaiBiShiYong : IConfigbase<ConfigDaiBiShiYong>, ConfigBase
{
	public ConfigDaiBiShiYong()
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
		this.m_DictDaiBiShiYongVOCfg.Clear();
	}

	public int buyZuanShi { get; set; }

	private void ParseDaiBiShiYongVOXML()
	{
		if (this.m_DictDaiBiShiYongVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.DaiBiShiYongVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuanLeDaiBi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			DaiBiShiYongVO daiBiShiYongVO = new DaiBiShiYongVO(xelementList[i]);
			this.m_DictDaiBiShiYongVOCfg.Add(daiBiShiYongVO.XiTongMingCheng, daiBiShiYongVO);
		}
	}

	public string DaiBiKaiGuan(string strKey)
	{
		string result = "diamond";
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			result = "huanlebi";
		}
		return result;
	}

	public void RefreshSp(UISprite sp, string strKey, int count, string urlOff = "")
	{
		bool flag = false;
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			flag = true;
		}
		this.huanLeBi = (long)Global.GetTotalGoodsCountByID(this.daiBiGoodId);
		if (flag && this.huanLeBi >= (long)count)
		{
			sp.spriteName = "huanlebi";
		}
		else if (!string.IsNullOrEmpty(urlOff) && IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan(strKey))
		{
			sp.spriteName = urlOff;
		}
		else
		{
			sp.spriteName = "diamond";
		}
	}

	public void RefreshShowImg(ShowNetImage img, string strKey, int count, string urlOff = "")
	{
		bool flag = false;
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			flag = true;
		}
		if (img.URL.Equals("NetImages/GameRes/Images/Unit/diamond.png") || img.URL.Equals("NetImages/GameRes/Images/Unit/huanlebi.png") || string.IsNullOrEmpty(img.URL) || img.URL.Equals(urlOff))
		{
			this.huanLeBi = (long)Global.GetTotalGoodsCountByID(this.daiBiGoodId);
			if (flag && this.huanLeBi >= (long)count)
			{
				img.URL = "NetImages/GameRes/Images/Unit/huanlebi.png";
			}
			else if (!string.IsNullOrEmpty(urlOff) && IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan(strKey))
			{
				img.URL = urlOff;
			}
			else
			{
				img.URL = "NetImages/GameRes/Images/Unit/diamond.png";
			}
		}
	}

	public string RefreshString(string name, string strKey, int count)
	{
		bool flag = false;
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			flag = true;
		}
		this.huanLeBi = (long)Global.GetTotalGoodsCountByID(this.daiBiGoodId);
		if (flag && this.huanLeBi >= (long)count)
		{
			name = Global.GetLang("代币");
		}
		if (!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan(strKey))
		{
			name = Global.GetLang("钻石");
		}
		return name;
	}

	public bool CloseZiDong(string strKey, int zuanshi)
	{
		bool flag = false;
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			flag = true;
		}
		this.huanLeBi = (long)Global.GetTotalGoodsCountByID(this.daiBiGoodId);
		return this.FaSongFlag && flag && this.huanLeBi < (long)zuanshi;
	}

	public bool SendHuoBiNumber(string strKey, int zuanshi, bool pp = false)
	{
		bool flag = false;
		this.ParseDaiBiShiYongVOXML();
		if (this.m_DictDaiBiShiYongVOCfg.ContainsKey(strKey) && this.m_DictDaiBiShiYongVOCfg[strKey].DaiBiKaiGuan == 1)
		{
			flag = true;
		}
		this.huanLeBi = (long)Global.GetTotalGoodsCountByID(this.daiBiGoodId);
		if (flag && this.huanLeBi >= (long)zuanshi)
		{
			this.FaSongFlag = true;
		}
		else if (IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan(strKey) && Global.GetRoleOwnNumByMoneyType(163) >= zuanshi)
		{
			this.FaSongFlag = true;
		}
		else if (pp && !IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan(strKey))
		{
			this.FaSongFlag = true;
		}
		else
		{
			this.FaSongFlag = false;
		}
		return this.FaSongFlag;
	}

	private string DaiBiShiYongVOXMLPath = "Config/DaiBiShiYong.xml";

	private Dictionary<string, DaiBiShiYongVO> m_DictDaiBiShiYongVOCfg = new Dictionary<string, DaiBiShiYongVO>();

	public int daiBiGoodId = 5440;

	private bool FaSongFlag;

	private long huanLeBi;
}
