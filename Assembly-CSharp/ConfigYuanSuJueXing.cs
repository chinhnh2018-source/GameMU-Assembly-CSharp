using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigYuanSuJueXing : IConfigbase<ConfigYuanSuJueXing>, ConfigBase
{
	public ConfigYuanSuJueXing()
	{
		this.XmlClearType = ClearType.ClearOnChangeSceneAndOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.ClearData();
	}

	public static ConfigYuanSuJueXing instance
	{
		get
		{
			return IConfigbase<ConfigYuanSuJueXing>.Instance;
		}
	}

	public void ClearDataChangeScene()
	{
		this.m_TypeGongJi = YuanSuJueXingType.none;
		this.m_TypeChuanTou = YuanSuJueXingType.none;
		this.m_DicJingLingYuanSuGongJiVO.Clear();
		this.m_DicJingLingYuanSuChanTouVO.Clear();
		this.m_DicJingLingYuanSuVO.Clear();
		this.m_DicJingLingYuanSuShuXingSumVO.Clear();
	}

	public void ClearData()
	{
		this.m_TypeGongJi = YuanSuJueXingType.none;
		this.m_TypeChuanTou = YuanSuJueXingType.none;
		this.m_DicJingLingYuanSuGongJiVO.Clear();
		this.m_DicJingLingYuanSuChanTouVO.Clear();
		this.m_DicJingLingYuanSuVO.Clear();
		this.m_DicJingLingYuanSuShuXingSumVO.Clear();
	}

	private void AddJingLingYuanSuVO()
	{
		XElement gameResXml = Global.GetGameResXml(this.JINGLINGYUANSU_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			JingLingYuanSuVO jingLingYuanSuVO = new JingLingYuanSuVO();
			jingLingYuanSuVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			jingLingYuanSuVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			jingLingYuanSuVO.XianShiLevel = Global.GetXElementAttributeStr(xelementList[i], "XianShiLevel");
			jingLingYuanSuVO.YuanSuType = Global.GetXElementAttributeInt(xelementList[i], "YuanSuType");
			jingLingYuanSuVO.ShuXingType = Global.GetXElementAttributeInt(xelementList[i], "ShuXingType");
			jingLingYuanSuVO.QiangHuaLevel = Global.GetXElementAttributeInt(xelementList[i], "QiangHuaLevel");
			jingLingYuanSuVO.JieXingCurrency = Global.GetXElementAttributeInt(xelementList[i], "JieXingCurrency");
			jingLingYuanSuVO.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
			jingLingYuanSuVO.Failtofail = Global.GetXElementAttributeStr(xelementList[i], "Failtofail");
			jingLingYuanSuVO.Success = Global.GetXElementAttributeFloat(xelementList[i], "Success");
			jingLingYuanSuVO.Attribute = Global.GetXElementAttributeStr(xelementList[i], "Attribute");
			if (this.m_DicJingLingYuanSuVO.ContainsKey(jingLingYuanSuVO.ID))
			{
				this.m_DicJingLingYuanSuVO[jingLingYuanSuVO.ID] = jingLingYuanSuVO;
			}
			else
			{
				this.m_DicJingLingYuanSuVO.Add(jingLingYuanSuVO.ID, jingLingYuanSuVO);
			}
			if (jingLingYuanSuVO.ShuXingType == 1)
			{
				if (this.m_DicJingLingYuanSuGongJiVO.ContainsKey(jingLingYuanSuVO.YuanSuType))
				{
					if (this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType].ContainsKey(jingLingYuanSuVO.QiangHuaLevel))
					{
						this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType][jingLingYuanSuVO.QiangHuaLevel] = jingLingYuanSuVO;
					}
					else
					{
						this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType].Add(jingLingYuanSuVO.QiangHuaLevel, jingLingYuanSuVO);
					}
				}
				else
				{
					this.m_DicJingLingYuanSuGongJiVO.Add(jingLingYuanSuVO.YuanSuType, new Dictionary<int, JingLingYuanSuVO>());
					if (this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType].ContainsKey(jingLingYuanSuVO.QiangHuaLevel))
					{
						this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType][jingLingYuanSuVO.QiangHuaLevel] = jingLingYuanSuVO;
					}
					else
					{
						this.m_DicJingLingYuanSuGongJiVO[jingLingYuanSuVO.YuanSuType].Add(jingLingYuanSuVO.QiangHuaLevel, jingLingYuanSuVO);
					}
				}
			}
			else if (jingLingYuanSuVO.ShuXingType == 2)
			{
				if (this.m_DicJingLingYuanSuChanTouVO.ContainsKey(jingLingYuanSuVO.YuanSuType))
				{
					if (this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType].ContainsKey(jingLingYuanSuVO.QiangHuaLevel))
					{
						this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType][jingLingYuanSuVO.QiangHuaLevel] = jingLingYuanSuVO;
					}
					else
					{
						this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType].Add(jingLingYuanSuVO.QiangHuaLevel, jingLingYuanSuVO);
					}
				}
				else
				{
					this.m_DicJingLingYuanSuChanTouVO.Add(jingLingYuanSuVO.YuanSuType, new Dictionary<int, JingLingYuanSuVO>());
					if (this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType].ContainsKey(jingLingYuanSuVO.QiangHuaLevel))
					{
						this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType][jingLingYuanSuVO.QiangHuaLevel] = jingLingYuanSuVO;
					}
					else
					{
						this.m_DicJingLingYuanSuChanTouVO[jingLingYuanSuVO.YuanSuType].Add(jingLingYuanSuVO.QiangHuaLevel, jingLingYuanSuVO);
					}
				}
			}
		}
	}

	private void AddJingLingYuanSuShuXingVO()
	{
		XElement gameResXml = Global.GetGameResXml(this.JINGLINGYUANSUSHUXING_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Rose");
		int[] array = new int[xelementList.Count];
		Dictionary<int, JingLingYuanSuShuXingVO> dictionary = new Dictionary<int, JingLingYuanSuShuXingVO>();
		Dictionary<int, JingLingYuanSuShuXingVO> dictionary2 = new Dictionary<int, JingLingYuanSuShuXingVO>();
		Dictionary<int, JingLingYuanSuShuXingVO> dictionary3 = new Dictionary<int, JingLingYuanSuShuXingVO>();
		for (int i = 0; i < xelementList.Count; i++)
		{
			JingLingYuanSuShuXingVO jingLingYuanSuShuXingVO = new JingLingYuanSuShuXingVO();
			jingLingYuanSuShuXingVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			jingLingYuanSuShuXingVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			jingLingYuanSuShuXingVO.Tipe = Global.GetXElementAttributeInt(xelementList[i], "Tipe");
			jingLingYuanSuShuXingVO.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			jingLingYuanSuShuXingVO.AcetiveElement = Global.GetXElementAttributeStr(xelementList[i], "AcetiveElement");
			jingLingYuanSuShuXingVO.tips = Global.GetXElementAttributeStr(xelementList[i], "tips");
			jingLingYuanSuShuXingVO.JingLingSpecial = Global.GetXElementAttributeStr(xelementList[i], "JingLingSpecial");
			if (jingLingYuanSuShuXingVO.Tipe == 1)
			{
				if (dictionary.ContainsKey(jingLingYuanSuShuXingVO.Level))
				{
					dictionary[jingLingYuanSuShuXingVO.Level] = jingLingYuanSuShuXingVO;
				}
				else
				{
					dictionary.Add(jingLingYuanSuShuXingVO.Level, jingLingYuanSuShuXingVO);
				}
			}
			if (jingLingYuanSuShuXingVO.Tipe == 2)
			{
				if (dictionary2.ContainsKey(jingLingYuanSuShuXingVO.Level))
				{
					dictionary2[jingLingYuanSuShuXingVO.Level] = jingLingYuanSuShuXingVO;
				}
				else
				{
					dictionary2.Add(jingLingYuanSuShuXingVO.Level, jingLingYuanSuShuXingVO);
				}
			}
			if (jingLingYuanSuShuXingVO.Tipe == 3)
			{
				if (dictionary3.ContainsKey(jingLingYuanSuShuXingVO.Level))
				{
					dictionary3[jingLingYuanSuShuXingVO.Level] = jingLingYuanSuShuXingVO;
				}
				else
				{
					dictionary3.Add(jingLingYuanSuShuXingVO.Level, jingLingYuanSuShuXingVO);
				}
			}
			array[i] = jingLingYuanSuShuXingVO.Level;
		}
		this.m_DicJingLingYuanSuShuXingSumVO.Add(1, dictionary);
		this.m_DicJingLingYuanSuShuXingSumVO.Add(2, dictionary2);
		this.m_DicJingLingYuanSuShuXingSumVO.Add(3, dictionary3);
		this.m_MaxLevelTeShu = Mathf.Max(array);
	}

	private Dictionary<int, JingLingYuanSuVO> DicJingLingYuanSuVO
	{
		get
		{
			if (this.m_DicJingLingYuanSuVO == null)
			{
				this.m_DicJingLingYuanSuVO = new Dictionary<int, JingLingYuanSuVO>();
			}
			if (this.m_DicJingLingYuanSuVO.Count <= 0)
			{
				this.AddJingLingYuanSuVO();
			}
			return this.m_DicJingLingYuanSuVO;
		}
	}

	private Dictionary<int, JingLingYuanSuVO> DicJingLingYuanSuGongJiVO(YuanSuJueXingType type)
	{
		if (this.m_DicJingLingYuanSuGongJiVO.Count <= 0)
		{
			this.AddJingLingYuanSuVO();
		}
		if (this.m_DicJingLingYuanSuChanTouVO.ContainsKey((int)type))
		{
			return this.m_DicJingLingYuanSuGongJiVO[(int)type];
		}
		return null;
	}

	private Dictionary<int, JingLingYuanSuVO> DicJingLingYuanSuChanTouVO(YuanSuJueXingType type)
	{
		if (this.m_DicJingLingYuanSuChanTouVO.Count <= 0)
		{
			this.AddJingLingYuanSuVO();
		}
		if (this.m_DicJingLingYuanSuChanTouVO.ContainsKey((int)type))
		{
			return this.m_DicJingLingYuanSuChanTouVO[(int)type];
		}
		return null;
	}

	public int Goodid(YuanSuJueXingType type)
	{
		string[] array = this.DicJingLingYuanSuChanTouVO(type)[0].NeedGoods.Split(new char[]
		{
			'|'
		});
		return array[0].Split(new char[]
		{
			','
		})[0].SafeToInt32(0);
	}

	public Dictionary<int, Dictionary<int, JingLingYuanSuShuXingVO>> DicJingLingYuanSuShuXingVO
	{
		get
		{
			if (this.m_DicJingLingYuanSuShuXingSumVO == null)
			{
				this.m_DicJingLingYuanSuShuXingSumVO = new Dictionary<int, Dictionary<int, JingLingYuanSuShuXingVO>>();
			}
			if (this.m_DicJingLingYuanSuShuXingSumVO.Count <= 0)
			{
				this.AddJingLingYuanSuShuXingVO();
			}
			return this.m_DicJingLingYuanSuShuXingSumVO;
		}
	}

	public JingLingYuanSuVO GetYuanSuShuXingGongJiLevel(int level)
	{
		if (this.DicJingLingYuanSuGongJiVO((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType).ContainsKey(level))
		{
			return this.DicJingLingYuanSuGongJiVO((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType)[level];
		}
		return null;
	}

	public JingLingYuanSuVO GetYuanSuShuXingChuanTouLevel(int level)
	{
		if (this.DicJingLingYuanSuChanTouVO((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType).ContainsKey(level))
		{
			return this.DicJingLingYuanSuChanTouVO((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType)[level];
		}
		return null;
	}

	public string[] GetAddYuanSuShuXing(YuanSuJueXingQiangHuaType type, int level)
	{
		YuanSuJueXingType activeType = (YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType;
		if (type == YuanSuJueXingQiangHuaType.GongJi)
		{
			if (this.DicJingLingYuanSuGongJiVO(activeType).ContainsKey(level) && this.DicJingLingYuanSuGongJiVO(activeType).ContainsKey(level + 1))
			{
				if (level <= 0)
				{
					return this.DicJingLingYuanSuGongJiVO(activeType)[level + 1].Attribute.Split(new char[]
					{
						'|'
					});
				}
				string[] array = new string[this.DicJingLingYuanSuGongJiVO(activeType)[level].Attribute.Split(new char[]
				{
					'|'
				}).Length];
				string[] array2 = this.DicJingLingYuanSuGongJiVO(activeType)[level].Attribute.Split(new char[]
				{
					'|'
				});
				string[] array3 = this.DicJingLingYuanSuGongJiVO(activeType)[level + 1].Attribute.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!ConfigExtPropIndexes.GetPercentByWord(array2[i].Split(new char[]
					{
						','
					})[0]))
					{
						array[i] = string.Format("{0},{1}", array3[i].Split(new char[]
						{
							','
						})[0], (array3[i].Split(new char[]
						{
							','
						})[1].SafeToInt32(0) - array2[i].Split(new char[]
						{
							','
						})[1].SafeToInt32(0)).ToString());
					}
					else
					{
						array[i] = string.Format("{0},{1}", array3[i].Split(new char[]
						{
							','
						})[0], (float.Parse(array3[i].Split(new char[]
						{
							','
						})[1]) - float.Parse(array2[i].Split(new char[]
						{
							','
						})[1])).ToString());
					}
				}
				return array;
			}
		}
		else if (type == YuanSuJueXingQiangHuaType.ChuanTou && this.DicJingLingYuanSuChanTouVO(activeType).ContainsKey(level) && this.DicJingLingYuanSuChanTouVO(activeType).ContainsKey(level + 1))
		{
			if (level <= 0)
			{
				return this.DicJingLingYuanSuGongJiVO(activeType)[level + 1].Attribute.Split(new char[]
				{
					'|'
				});
			}
			string[] array4 = new string[this.DicJingLingYuanSuChanTouVO(activeType)[level].Attribute.Split(new char[]
			{
				'|'
			}).Length];
			string[] array5 = this.DicJingLingYuanSuChanTouVO(activeType)[level].Attribute.Split(new char[]
			{
				'|'
			});
			string[] array6 = this.DicJingLingYuanSuChanTouVO(activeType)[level + 1].Attribute.Split(new char[]
			{
				'|'
			});
			for (int j = 0; j < array4.Length; j++)
			{
				if (!ConfigExtPropIndexes.GetPercentByWord(array5[j].Split(new char[]
				{
					','
				})[0]))
				{
					array4[j] = string.Format("{0},{1}", array6[j].Split(new char[]
					{
						','
					})[0], (array6[j].Split(new char[]
					{
						','
					})[1].SafeToInt32(0) - array5[j].Split(new char[]
					{
						','
					})[1].SafeToInt32(0)).ToString());
				}
				else
				{
					array4[j] = string.Format("{0},{1}", array6[j].Split(new char[]
					{
						','
					})[0], (float.Parse(array6[j].Split(new char[]
					{
						','
					})[1]) - float.Parse(array5[j].Split(new char[]
					{
						','
					})[1])).ToString());
				}
			}
			return array4;
		}
		return null;
	}

	public string[] GetAddTeShuShuXing(int level)
	{
		YuanSuJueXingType yuanSuJueXingType = YuanSuJueXingType.none;
		if (Global.Data.roleData.JingLingYuanSuJueXingData != null)
		{
			yuanSuJueXingType = (YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType;
		}
		if (!this.DicJingLingYuanSuShuXingVO.ContainsKey((int)yuanSuJueXingType))
		{
			return null;
		}
		if (this.DicJingLingYuanSuShuXingVO[(int)yuanSuJueXingType].ContainsKey(level))
		{
			return this.DicJingLingYuanSuShuXingVO[(int)yuanSuJueXingType][level].AcetiveElement.Split(new char[]
			{
				'|'
			});
		}
		return null;
	}

	public JingLingYuanSuShuXingVO GetJingLingYuanSuShuXingLevel(int level)
	{
		YuanSuJueXingType yuanSuJueXingType = YuanSuJueXingType.none;
		if (Global.Data.roleData.JingLingYuanSuJueXingData != null)
		{
			yuanSuJueXingType = (YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType;
		}
		if (!this.DicJingLingYuanSuShuXingVO.ContainsKey((int)yuanSuJueXingType))
		{
			return null;
		}
		if (this.DicJingLingYuanSuShuXingVO[(int)yuanSuJueXingType].ContainsKey(level))
		{
			return this.DicJingLingYuanSuShuXingVO[(int)yuanSuJueXingType][level];
		}
		return null;
	}

	public Dictionary<int, JingLingYuanSuShuXingVO> GetJingLingYuanSuShuXingType(int type)
	{
		if (this.DicJingLingYuanSuShuXingVO.ContainsKey(type))
		{
			return this.DicJingLingYuanSuShuXingVO[type];
		}
		return null;
	}

	public JingLingYuanSuShuXingVO GetJingLingYuanSuShuXinKeyType(int[] data, int type, out bool JiHuoBool)
	{
		JiHuoBool = true;
		if (data == null)
		{
			return null;
		}
		int num = -1;
		int num2 = -1;
		if (type == 1)
		{
			if (this.DicJingLingYuanSuVO.ContainsKey(data[0]))
			{
				num = this.DicJingLingYuanSuVO[data[0]].QiangHuaLevel;
			}
			if (this.DicJingLingYuanSuVO.ContainsKey(data[1]))
			{
				num2 = this.DicJingLingYuanSuVO[data[1]].QiangHuaLevel;
			}
		}
		else if (type == 2)
		{
			if (this.DicJingLingYuanSuVO.ContainsKey(data[2]))
			{
				num = this.DicJingLingYuanSuVO[data[2]].QiangHuaLevel;
			}
			if (this.DicJingLingYuanSuVO.ContainsKey(data[3]))
			{
				num2 = this.DicJingLingYuanSuVO[data[3]].QiangHuaLevel;
			}
		}
		else if (type == 3)
		{
			if (this.DicJingLingYuanSuVO.ContainsKey(data[4]))
			{
				num = this.DicJingLingYuanSuVO[data[4]].QiangHuaLevel;
			}
			if (this.DicJingLingYuanSuVO.ContainsKey(data[5]))
			{
				num2 = this.DicJingLingYuanSuVO[data[5]].QiangHuaLevel;
			}
		}
		if (this.DicJingLingYuanSuShuXingVO.ContainsKey(type))
		{
			int num3 = Mathf.Min(num, num2);
			int num4 = num3 / 4;
			if (num3 <= 0)
			{
				JiHuoBool = false;
			}
			else
			{
				JiHuoBool = true;
			}
			if (num4 <= 0)
			{
				num4 = 1;
			}
			if (this.DicJingLingYuanSuShuXingVO[type].ContainsKey(num4))
			{
				this.DicJingLingYuanSuShuXingVO[type][num4].MinLevel = num3;
				return this.DicJingLingYuanSuShuXingVO[type][num4];
			}
		}
		return null;
	}

	public JingLingYuanSuVO GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType QianghuaType)
	{
		if (Global.Data.roleData.JingLingYuanSuJueXingData == null)
		{
			return null;
		}
		if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 1)
		{
			if (QianghuaType == YuanSuJueXingQiangHuaType.GongJi)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[0] <= 0)
				{
					return this.GetYuanSuShuXingGongJiLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[0]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[0]];
				}
			}
			else if (QianghuaType == YuanSuJueXingQiangHuaType.ChuanTou)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[1] <= 0)
				{
					return this.GetYuanSuShuXingChuanTouLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[1]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[1]];
				}
			}
		}
		else if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 2)
		{
			if (QianghuaType == YuanSuJueXingQiangHuaType.GongJi)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[2] <= 0)
				{
					return this.GetYuanSuShuXingGongJiLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[2]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[2]];
				}
			}
			else if (QianghuaType == YuanSuJueXingQiangHuaType.ChuanTou)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[3] <= 0)
				{
					return this.GetYuanSuShuXingChuanTouLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[3]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[3]];
				}
			}
		}
		else if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 3)
		{
			if (QianghuaType == YuanSuJueXingQiangHuaType.GongJi)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[4] <= 0)
				{
					return this.GetYuanSuShuXingGongJiLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[4]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[4]];
				}
			}
			else if (QianghuaType == YuanSuJueXingQiangHuaType.ChuanTou)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[5] <= 0)
				{
					return this.GetYuanSuShuXingChuanTouLevel(0);
				}
				if (this.DicJingLingYuanSuVO.ContainsKey(Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[5]))
				{
					return this.DicJingLingYuanSuVO[Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs[5]];
				}
			}
		}
		return null;
	}

	public int MaxLevelGongJi(YuanSuJueXingType type)
	{
		if (this.m_TypeGongJi != type)
		{
			int[] array = new int[this.DicJingLingYuanSuGongJiVO(type).Count];
			int num = 0;
			Dictionary<int, JingLingYuanSuVO>.Enumerator enumerator = this.DicJingLingYuanSuGongJiVO(type).GetEnumerator();
			while (enumerator.MoveNext())
			{
				int[] array2 = array;
				int num2 = num;
				KeyValuePair<int, JingLingYuanSuVO> keyValuePair = enumerator.Current;
				array2[num2] = keyValuePair.Value.QiangHuaLevel;
			}
			this.m_TypeGongJi = type;
			this.m_MaxLevelGongJi = Mathf.Max(array);
		}
		return this.m_MaxLevelGongJi;
	}

	public int MaxLevelChuanTou(YuanSuJueXingType type)
	{
		if (this.m_TypeChuanTou != type)
		{
			int[] array = new int[this.DicJingLingYuanSuGongJiVO(type).Count];
			int num = 0;
			Dictionary<int, JingLingYuanSuVO>.Enumerator enumerator = this.DicJingLingYuanSuGongJiVO(type).GetEnumerator();
			while (enumerator.MoveNext())
			{
				int[] array2 = array;
				int num2 = num;
				KeyValuePair<int, JingLingYuanSuVO> keyValuePair = enumerator.Current;
				array2[num2] = keyValuePair.Value.QiangHuaLevel;
			}
			this.m_TypeChuanTou = type;
			this.m_MaxLevelChuanTou = Mathf.Max(array);
		}
		return this.m_MaxLevelChuanTou;
	}

	public int MaxLevelTeShu
	{
		get
		{
			if (this.m_MaxLevelTeShu <= 0)
			{
				this.AddJingLingYuanSuShuXingVO();
			}
			return this.m_MaxLevelTeShu;
		}
	}

	public ClearType XmlClearType { get; set; }

	private string JINGLINGYUANSU_PATH = "Config/JingLingYuanSu.xml";

	private string JINGLINGYUANSUSHUXING_PATH = "Config/JingLingYuanSuShuXing.xml";

	private Dictionary<int, Dictionary<int, JingLingYuanSuVO>> m_DicJingLingYuanSuGongJiVO = new Dictionary<int, Dictionary<int, JingLingYuanSuVO>>();

	private Dictionary<int, Dictionary<int, JingLingYuanSuVO>> m_DicJingLingYuanSuChanTouVO = new Dictionary<int, Dictionary<int, JingLingYuanSuVO>>();

	private Dictionary<int, JingLingYuanSuVO> m_DicJingLingYuanSuVO = new Dictionary<int, JingLingYuanSuVO>();

	private Dictionary<int, Dictionary<int, JingLingYuanSuShuXingVO>> m_DicJingLingYuanSuShuXingSumVO = new Dictionary<int, Dictionary<int, JingLingYuanSuShuXingVO>>();

	private int m_MaxLevelGongJi = -1;

	private int m_MaxLevelChuanTou = -1;

	private int m_MaxLevelTeShu = -1;

	private YuanSuJueXingType m_TypeGongJi;

	private YuanSuJueXingType m_TypeChuanTou;
}
