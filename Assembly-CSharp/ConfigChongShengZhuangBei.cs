using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ConfigChongShengZhuangBei : IConfigbase<ConfigChongShengZhuangBei>, ConfigBase
{
	public ConfigChongShengZhuangBei()
	{
		this.XmlClearType = ClearType.ClearOnChangeSceneAndOnLondConfigNoDispose;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		if (clearType == 1)
		{
			this.ClearDataChangeScene();
		}
		else
		{
			this.ClearData();
		}
	}

	public void ClearDataChangeScene()
	{
		this.m_DicZhuangBeiDaKongVO.Clear();
		this.m_DicChongShengBaoShiVO.Clear();
		this.m_DicXuanCaiShuXingVO.Clear();
		this.m_DicXuanCaiHeChengVO.Clear();
	}

	public void ClearData()
	{
		this.m_DicZhuangBeiDaKongVO.Clear();
		this.m_DicChongShengBaoShiVO.Clear();
		this.m_DicXuanCaiShuXingVO.Clear();
		this.m_DicXuanCaiHeChengVO.Clear();
	}

	private void AddDataZhuangBeiDaKongVO()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ZhuangBeiDaKong.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZhuangBeiDaKongVO zhuangBeiDaKongVO = new ZhuangBeiDaKongVO();
			zhuangBeiDaKongVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			zhuangBeiDaKongVO.ZhuangBeiDengJie = Global.GetXElementAttributeInt(xelementList[i], "ZhuangBeiDengJie");
			zhuangBeiDaKongVO.ZhuangBeiPinZhi = Global.GetXElementAttributeInt(xelementList[i], "ZhuangBeiPinZhi");
			zhuangBeiDaKongVO.DaKongShuLiang = Global.GetXElementAttributeInt(xelementList[i], "DaKongShuLiang");
			zhuangBeiDaKongVO.XiaoHaoDaoJu = Global.GetXElementAttributeStr(xelementList[i], "XiaoHaoDaoJu");
			zhuangBeiDaKongVO.GaiLv = Global.GetXElementAttributeStr(xelementList[i], "GaiLv");
			if (this.m_DicZhuangBeiDaKongVO.ContainsKey(zhuangBeiDaKongVO.ID))
			{
				this.m_DicZhuangBeiDaKongVO[zhuangBeiDaKongVO.ID] = zhuangBeiDaKongVO;
			}
			else
			{
				this.m_DicZhuangBeiDaKongVO.Add(zhuangBeiDaKongVO.ID, zhuangBeiDaKongVO);
			}
		}
	}

	private Dictionary<int, ZhuangBeiDaKongVO> DicZhuangBeiDaKongVO
	{
		get
		{
			if (this.m_DicZhuangBeiDaKongVO == null)
			{
				this.m_DicZhuangBeiDaKongVO = new Dictionary<int, ZhuangBeiDaKongVO>();
			}
			if (this.m_DicZhuangBeiDaKongVO.Count <= 0)
			{
				this.AddDataZhuangBeiDaKongVO();
			}
			return this.m_DicZhuangBeiDaKongVO;
		}
	}

	private void AddDataChongShengBaoShiVO()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ChongShengBaoShi.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ChongShengBaoShiVO chongShengBaoShiVO = new ChongShengBaoShiVO();
			chongShengBaoShiVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			chongShengBaoShiVO.BaoShiID = Global.GetXElementAttributeInt(xelementList[i], "BaoShiID");
			chongShengBaoShiVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			chongShengBaoShiVO.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			chongShengBaoShiVO.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			chongShengBaoShiVO.ShuXing = Global.GetXElementAttributeStr(xelementList[i], "ShuXing");
			chongShengBaoShiVO.FengYinJingShi = Global.GetXElementAttributeInt(xelementList[i], "FengYinJingShi");
			chongShengBaoShiVO.ChongShengJingShi = Global.GetXElementAttributeInt(xelementList[i], "ChongShengJingShi");
			chongShengBaoShiVO.XuanCaiJingShi = Global.GetXElementAttributeInt(xelementList[i], "XuanCaiJingShi");
			if (this.m_DicChongShengBaoShiVO.ContainsKey(chongShengBaoShiVO.BaoShiID))
			{
				this.m_DicChongShengBaoShiVO[chongShengBaoShiVO.BaoShiID] = chongShengBaoShiVO;
			}
			else
			{
				this.m_DicChongShengBaoShiVO.Add(chongShengBaoShiVO.BaoShiID, chongShengBaoShiVO);
			}
		}
	}

	private void AddDataXuanCaiShuXingVO()
	{
		XElement gameResXml = Global.GetGameResXml("Config/XuanCaiShuXing.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XuanCaiShuXingVO xuanCaiShuXingVO = new XuanCaiShuXingVO();
			xuanCaiShuXingVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			xuanCaiShuXingVO.DaoJuID = Global.GetXElementAttributeInt(xelementList[i], "DaoJuID");
			xuanCaiShuXingVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			xuanCaiShuXingVO.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			xuanCaiShuXingVO.JiHuoShuXing = Global.GetXElementAttributeStr(xelementList[i], "JiHuoShuXing");
			xuanCaiShuXingVO.Tips = Global.GetXElementAttributeStr(xelementList[i], "Tips");
			xuanCaiShuXingVO.JiHuoTiaoJian = Global.GetXElementAttributeStr(xelementList[i], "JiHuoTiaoJian");
			xuanCaiShuXingVO.FenJieHuoDe = Global.GetXElementAttributeInt(xelementList[i], "FenJieHuoDe");
			if (this.m_DicXuanCaiShuXingVO.ContainsKey(xuanCaiShuXingVO.ID))
			{
				this.m_DicXuanCaiShuXingVO[xuanCaiShuXingVO.ID] = xuanCaiShuXingVO;
			}
			else
			{
				this.m_DicXuanCaiShuXingVO.Add(xuanCaiShuXingVO.ID, xuanCaiShuXingVO);
			}
		}
	}

	private void AddDataXuanCaiHeChengVO()
	{
		XElement gameResXml = Global.GetGameResXml("Config/XuanCaiHeCheng.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XuanCaiHeChengVO xuanCaiHeChengVO = new XuanCaiHeChengVO();
			xuanCaiHeChengVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			xuanCaiHeChengVO.BaoShiID = Global.GetXElementAttributeInt(xelementList[i], "BaoShiID");
			xuanCaiHeChengVO.Name = Global.GetXElementAttributeInt(xelementList[i], "Name");
			xuanCaiHeChengVO.HeChengBaoShiId = Global.GetXElementAttributeInt(xelementList[i], "HeChengBaoShiId");
			xuanCaiHeChengVO.HeChengXiaoHao = Global.GetXElementAttributeStr(xelementList[i], "HeChengXiaoHao");
			if (this.m_DicXuanCaiHeChengVO.ContainsKey(xuanCaiHeChengVO.BaoShiID))
			{
				this.m_DicXuanCaiHeChengVO[xuanCaiHeChengVO.BaoShiID] = xuanCaiHeChengVO;
			}
			else
			{
				this.m_DicXuanCaiHeChengVO.Add(xuanCaiHeChengVO.BaoShiID, xuanCaiHeChengVO);
			}
		}
	}

	private Dictionary<int, XuanCaiHeChengVO> DicXuanCaiHeChengVO
	{
		get
		{
			if (this.m_DicXuanCaiHeChengVO == null)
			{
				this.m_DicXuanCaiHeChengVO = new Dictionary<int, XuanCaiHeChengVO>();
			}
			if (this.m_DicXuanCaiHeChengVO.Count <= 0)
			{
				this.AddDataXuanCaiHeChengVO();
			}
			return this.m_DicXuanCaiHeChengVO;
		}
	}

	public Dictionary<int, XuanCaiShuXingVO> DicXuanCaiShuXingVO
	{
		get
		{
			if (this.m_DicXuanCaiShuXingVO == null)
			{
				this.m_DicXuanCaiShuXingVO = new Dictionary<int, XuanCaiShuXingVO>();
			}
			if (this.m_DicXuanCaiShuXingVO.Count <= 0)
			{
				this.AddDataXuanCaiShuXingVO();
			}
			return this.m_DicXuanCaiShuXingVO;
		}
	}

	public Dictionary<int, ChongShengBaoShiVO> DicChongShengBaoShiVO
	{
		get
		{
			if (this.m_DicChongShengBaoShiVO == null)
			{
				this.m_DicChongShengBaoShiVO = new Dictionary<int, ChongShengBaoShiVO>();
			}
			if (this.m_DicChongShengBaoShiVO.Count <= 0)
			{
				this.AddDataChongShengBaoShiVO();
			}
			return this.m_DicChongShengBaoShiVO;
		}
	}

	public ZhuangBeiDaKongVO GetDaKongDataByJieShu(int jieShu, int pinZhi)
	{
		foreach (KeyValuePair<int, ZhuangBeiDaKongVO> keyValuePair in this.DicZhuangBeiDaKongVO)
		{
			if (keyValuePair.Value.ZhuangBeiDengJie == jieShu)
			{
				Dictionary<int, ZhuangBeiDaKongVO>.Enumerator enumerator;
				KeyValuePair<int, ZhuangBeiDaKongVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.ZhuangBeiPinZhi == pinZhi)
				{
					KeyValuePair<int, ZhuangBeiDaKongVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public List<ChongShengBaoShiVO> GetChongShengBaoShi(int level)
	{
		Dictionary<int, ChongShengBaoShiVO>.Enumerator enumerator = this.DicChongShengBaoShiVO.GetEnumerator();
		List<ChongShengBaoShiVO> list = new List<ChongShengBaoShiVO>();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, ChongShengBaoShiVO> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.Level == level)
			{
				List<ChongShengBaoShiVO> list2 = list;
				KeyValuePair<int, ChongShengBaoShiVO> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		return list;
	}

	public ChongShengBaoShiVO GetChongShengBaoShiById(int goodsId)
	{
		if (this.DicChongShengBaoShiVO.ContainsKey(goodsId))
		{
			return this.DicChongShengBaoShiVO[goodsId];
		}
		return null;
	}

	public List<XuanCaiShuXingVO> GetXuanCaiShuXing(int level)
	{
		Dictionary<int, XuanCaiShuXingVO>.Enumerator enumerator = this.DicXuanCaiShuXingVO.GetEnumerator();
		List<XuanCaiShuXingVO> list = new List<XuanCaiShuXingVO>();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, XuanCaiShuXingVO> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.Level == level)
			{
				List<XuanCaiShuXingVO> list2 = list;
				KeyValuePair<int, XuanCaiShuXingVO> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		return list;
	}

	public XuanCaiHeChengVO GetXuanCaiHeCheng(int Goodsid)
	{
		if (this.DicXuanCaiHeChengVO.ContainsKey(Goodsid))
		{
			return this.DicXuanCaiHeChengVO[Goodsid];
		}
		return null;
	}

	public List<XuanCaiShuXingVO> GetXuanCaiShuXingByGoodId(int goodsId)
	{
		Dictionary<int, XuanCaiShuXingVO>.Enumerator enumerator = this.DicXuanCaiShuXingVO.GetEnumerator();
		List<XuanCaiShuXingVO> list = new List<XuanCaiShuXingVO>();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, XuanCaiShuXingVO> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.DaoJuID == goodsId)
			{
				List<XuanCaiShuXingVO> list2 = list;
				KeyValuePair<int, XuanCaiShuXingVO> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		return list;
	}

	public string PinZhiKuang(int level)
	{
		if (level == 3)
		{
			return "kuang_lan";
		}
		if (level == 4)
		{
			return "kuang_zi";
		}
		if (level == 5)
		{
			return "kuang_zishan";
		}
		return string.Empty;
	}

	public int GetDaKongPingZhi(int zuoyue)
	{
		if (zuoyue <= 4)
		{
			return 3;
		}
		if (zuoyue == 5)
		{
			return 4;
		}
		if (zuoyue > 5)
		{
			return 5;
		}
		return 0;
	}

	public string PinZhiXiangQian(int level)
	{
		if (level == 3)
		{
			return "jiahao_lan";
		}
		if (level == 4)
		{
			return "jiahao_zishan";
		}
		if (level == 5)
		{
			return "jiahao_zishan";
		}
		return string.Empty;
	}

	public List<bool> GetLevelXuanCai(int roleId, int goodsId)
	{
		List<bool> list = new List<bool>();
		list.Add(false);
		list.Add(false);
		list.Add(false);
		if (roleId == -1)
		{
			return list;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		List<GoodsData> list2 = null;
		if (roleId == Global.Data.RoleID)
		{
			list2 = Global.Data.roleData.RebornGoodsDataList;
		}
		else if (Global.Data.OtherRoles.ContainsKey(roleId))
		{
			list2 = Global.Data.OtherRoles[roleId].RebornGoodsDataList;
		}
		if (list2 != null)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(list2[i].GoodsID);
				if (goodsXmlNodeByID == null)
				{
					return list;
				}
				if (goodsXmlNodeByID.Categoriy >= 30 && goodsXmlNodeByID.Categoriy <= 38 && list2[i].Using == 1)
				{
					string[] array = list2[i].Props.Split(new char[]
					{
						'|'
					});
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							if (!string.IsNullOrEmpty(array[j]))
							{
								int num = array[j].Split(new char[]
								{
									'_'
								})[0].SafeToInt32(0);
								int num2 = array[j].Split(new char[]
								{
									'_'
								})[2].SafeToInt32(0);
								if (num != 0 && IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(num2))
								{
									if (dictionary.ContainsKey(IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num2].Type))
									{
										Dictionary<int, int> dictionary3;
										Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
										int num4;
										int num3 = num4 = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num2].Type;
										num4 = dictionary3[num4];
										dictionary2[num3] = num4 + IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num2].Level;
									}
									else
									{
										dictionary.Add(IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num2].Type, IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num2].Level);
									}
								}
							}
						}
					}
				}
			}
		}
		List<XuanCaiShuXingVO> xuanCaiShuXingByGoodId = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiShuXingByGoodId(goodsId);
		for (int k = 0; k < xuanCaiShuXingByGoodId.Count; k++)
		{
			string[] array2 = xuanCaiShuXingByGoodId[k].JiHuoTiaoJian.Split(new char[]
			{
				'|'
			});
			for (int l = 0; l < array2.Length; l++)
			{
				if (!string.IsNullOrEmpty(array2[l]))
				{
					int num5 = array2[l].Split(new char[]
					{
						','
					})[0].SafeToInt32(0);
					int num6 = array2[l].Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
					int num7 = 0;
					if (dictionary.ContainsKey(num5))
					{
						num7 = dictionary[num5];
					}
					if (num7 < num6)
					{
						list[k] = false;
						break;
					}
					if (l >= array2.Length - 1)
					{
						list[k] = true;
					}
				}
			}
		}
		return list;
	}

	private const string ZHUANGBEIDAKONG_PATH = "Config/ZhuangBeiDaKong.xml";

	private const string CHONGSHENGBAOSHI_PATH = "Config/ChongShengBaoShi.xml";

	private const string XUANCAISHUXING_PATH = "Config/XuanCaiShuXing.xml";

	private const string XUANCAIHECHENG_PATH = "Config/XuanCaiHeCheng.xml";

	private Dictionary<int, ZhuangBeiDaKongVO> m_DicZhuangBeiDaKongVO = new Dictionary<int, ZhuangBeiDaKongVO>();

	private Dictionary<int, ChongShengBaoShiVO> m_DicChongShengBaoShiVO = new Dictionary<int, ChongShengBaoShiVO>();

	private Dictionary<int, XuanCaiShuXingVO> m_DicXuanCaiShuXingVO = new Dictionary<int, XuanCaiShuXingVO>();

	private Dictionary<int, XuanCaiHeChengVO> m_DicXuanCaiHeChengVO = new Dictionary<int, XuanCaiHeChengVO>();
}
