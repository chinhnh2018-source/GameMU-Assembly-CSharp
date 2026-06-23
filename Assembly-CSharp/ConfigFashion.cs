using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigFashion
{
	private static void InitFashion()
	{
		if (0 < ConfigFashion.mFashionLevelupData.Count)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("ShiZhuangLevelup.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShiZhuangLevelup");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				FashionLevelupVO fashionLevelupVO = new FashionLevelupVO();
				fashionLevelupVO.ID = Global.GetXElementAttributeInt(xelement, "ID");
				fashionLevelupVO.GoodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				fashionLevelupVO.Name = Global.GetXElementAttributeStr(xelement, "Name");
				fashionLevelupVO.level = Global.GetXElementAttributeInt(xelement, "level");
				fashionLevelupVO.ProPerty = Global.GetXElementAttributeStr(xelement, "ProPerty");
				fashionLevelupVO.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				fashionLevelupVO.Time = Global.GetXElementAttributeInt(xelement, "Time");
				fashionLevelupVO.Description = Global.GetXElementAttributeStr(xelement, "Description");
				if (ConfigFashion.mFashionLevelupData.ContainsKey(fashionLevelupVO.ID))
				{
					ConfigFashion.mFashionLevelupData[fashionLevelupVO.ID] = fashionLevelupVO;
				}
				else
				{
					ConfigFashion.mFashionLevelupData.Add(fashionLevelupVO.ID, fashionLevelupVO);
				}
			}
		}
	}

	private static void InitMoXingVO()
	{
		if (0 < ConfigFashion.dic.Count)
		{
			return;
		}
		string text = "Config/WuQiShiZhuangMoXing.xml";
		string text2 = "GameRes";
		try
		{
			text = XmlManager.GetOnlyFileName(text);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 缓存中没找到 {0}"), text2));
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
				}
				else
				{
					string text3 = textAsset.text;
					XElement xelement = XElement.Parse(text3);
					if (xelement == null)
					{
						GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
					}
					else
					{
						List<XElement> xelementList = Global.GetXElementList(xelement, "JingLing");
						if (xelementList == null || xelementList.Count <= 0)
						{
							GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
						}
						else
						{
							int count = xelementList.Count;
							for (int i = 0; i < count; i++)
							{
								WuQiShiZhuangMoXingVO wuQiShiZhuangMoXingVO = new WuQiShiZhuangMoXingVO();
								wuQiShiZhuangMoXingVO.CopyForm(xelementList[i]);
								ConfigFashion.dic[wuQiShiZhuangMoXingVO.ID] = wuQiShiZhuangMoXingVO;
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
	}

	private static void InitWing()
	{
		if (0 < ConfigFashion.mWingLevelupData.Count)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/FashionWings.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "SpecialTitle");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				FashionLevelupVO fashionLevelupVO = new FashionLevelupVO();
				fashionLevelupVO.ID = Global.GetXElementAttributeInt(xelement, "ID");
				fashionLevelupVO.GoodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				fashionLevelupVO.Name = Global.GetXElementAttributeStr(xelement, "Name");
				fashionLevelupVO.level = Global.GetXElementAttributeInt(xelement, "level");
				fashionLevelupVO.ProPerty = Global.GetXElementAttributeStr(xelement, "ProPerty");
				fashionLevelupVO.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				fashionLevelupVO.Time = Global.GetXElementAttributeInt(xelement, "Time");
				fashionLevelupVO.MOD = Global.GetXElementAttributeStr(xelement, "MOD");
				if (ConfigFashion.mWingLevelupData.ContainsKey(fashionLevelupVO.ID))
				{
					ConfigFashion.mWingLevelupData[fashionLevelupVO.ID] = fashionLevelupVO;
				}
				else
				{
					ConfigFashion.mWingLevelupData.Add(fashionLevelupVO.ID, fashionLevelupVO);
				}
				if (!ConfigFashion.mFashionGoodIdList.Contains(fashionLevelupVO.GoodsID))
				{
					ConfigFashion.mFashionGoodIdList.Add(fashionLevelupVO.GoodsID);
				}
			}
		}
	}

	private static void InitWuQi()
	{
		if (0 < ConfigFashion.mWuQiLevelupData.Count)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/WuQiShiZhuangShengJi.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				FashionLevelupVO fashionLevelupVO = new FashionLevelupVO();
				fashionLevelupVO.ID = Global.GetXElementAttributeInt(xelement, "ID");
				fashionLevelupVO.GoodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				fashionLevelupVO.Name = Global.GetXElementAttributeStr(xelement, "Name");
				fashionLevelupVO.level = Global.GetXElementAttributeInt(xelement, "level");
				fashionLevelupVO.ProPerty = Global.GetXElementAttributeStr(xelement, "ProPerty");
				fashionLevelupVO.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				fashionLevelupVO.Time = Global.GetXElementAttributeInt(xelement, "Time");
				fashionLevelupVO.MOD = Global.GetXElementAttributeStr(xelement, "MOD");
				fashionLevelupVO.Previev = Global.GetXElementAttributeInt(xelement, "Previev");
				fashionLevelupVO.Scene = Global.GetXElementAttributeInt(xelement, "Scene");
				if (ConfigFashion.mWuQiLevelupData.ContainsKey(fashionLevelupVO.ID))
				{
					ConfigFashion.mWuQiLevelupData[fashionLevelupVO.ID] = fashionLevelupVO;
				}
				else
				{
					ConfigFashion.mWuQiLevelupData.Add(fashionLevelupVO.ID, fashionLevelupVO);
				}
				if (!ConfigFashion.mFashionGoodIdList.Contains(fashionLevelupVO.GoodsID))
				{
					ConfigFashion.mFashionGoodIdList.Add(fashionLevelupVO.GoodsID);
				}
			}
		}
	}

	private static void InitJiaoYin()
	{
		if (0 < ConfigFashion.mJiaoYinLevelupData.Count)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/JiaoYinShiZhuangShengJi.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				FashionLevelupVO fashionLevelupVO = new FashionLevelupVO();
				fashionLevelupVO.ID = Global.GetXElementAttributeInt(xelement, "ID");
				fashionLevelupVO.GoodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				fashionLevelupVO.Name = Global.GetXElementAttributeStr(xelement, "Name");
				fashionLevelupVO.level = Global.GetXElementAttributeInt(xelement, "level");
				fashionLevelupVO.ProPerty = Global.GetXElementAttributeStr(xelement, "ProPerty");
				fashionLevelupVO.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				fashionLevelupVO.Time = Global.GetXElementAttributeInt(xelement, "Time");
				fashionLevelupVO.MOD = Global.GetXElementAttributeStr(xelement, "MOD");
				fashionLevelupVO.Previev = Global.GetXElementAttributeInt(xelement, "Previev");
				fashionLevelupVO.Scene = Global.GetXElementAttributeInt(xelement, "Scene");
				if (ConfigFashion.mJiaoYinLevelupData.ContainsKey(fashionLevelupVO.ID))
				{
					ConfigFashion.mJiaoYinLevelupData[fashionLevelupVO.ID] = fashionLevelupVO;
				}
				else
				{
					ConfigFashion.mJiaoYinLevelupData.Add(fashionLevelupVO.ID, fashionLevelupVO);
				}
				if (!ConfigFashion.mFashionGoodIdList.Contains(fashionLevelupVO.GoodsID))
				{
					ConfigFashion.mFashionGoodIdList.Add(fashionLevelupVO.GoodsID);
				}
			}
		}
	}

	public static FashionLevelupVO Get(ItemCategories Cate, int GoodsID, int Level)
	{
		Dictionary<int, FashionLevelupVO> dictionary = null;
		if (Cate == ItemCategories.Fashion)
		{
			ConfigFashion.InitFashion();
			dictionary = ConfigFashion.mFashionLevelupData;
		}
		else if (Cate == ItemCategories.ChiBang)
		{
			ConfigFashion.InitWing();
			dictionary = ConfigFashion.mWingLevelupData;
		}
		else if (Cate == ItemCategories.ShiZhuang_JiaoYin)
		{
			ConfigFashion.InitJiaoYin();
			dictionary = ConfigFashion.mJiaoYinLevelupData;
		}
		else if (Cate == ItemCategories.ShiZhuang_WuQi)
		{
			ConfigFashion.InitWuQi();
			dictionary = ConfigFashion.mWuQiLevelupData;
		}
		else if (Cate == ItemCategories.ShiZhuang_WuQi)
		{
		}
		if (dictionary != null)
		{
			foreach (KeyValuePair<int, FashionLevelupVO> keyValuePair in dictionary)
			{
				if (GoodsID == keyValuePair.Value.GoodsID)
				{
					Dictionary<int, FashionLevelupVO>.Enumerator enumerator;
					KeyValuePair<int, FashionLevelupVO> keyValuePair2 = enumerator.Current;
					if (Level == keyValuePair2.Value.level)
					{
						KeyValuePair<int, FashionLevelupVO> keyValuePair3 = enumerator.Current;
						return keyValuePair3.Value;
					}
				}
			}
		}
		return null;
	}

	public static WuQiShiZhuangMoXingVO GetWuQiShiZhuangMoXingVOByID(int id)
	{
		ConfigFashion.InitMoXingVO();
		if (ConfigFashion.dic.ContainsKey(id))
		{
			return ConfigFashion.dic[id];
		}
		return null;
	}

	public static WuQiShiZhuangMoXingVO GetWuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion(int GoodsID, int occupayion)
	{
		ConfigFashion.InitMoXingVO();
		foreach (KeyValuePair<int, WuQiShiZhuangMoXingVO> keyValuePair in ConfigFashion.dic)
		{
			WuQiShiZhuangMoXingVO value = keyValuePair.Value;
			if (GoodsID == value.GoodsID && occupayion == value.Occupayion)
			{
				return value;
			}
		}
		return null;
	}

	public static bool IsFashion(int GoodsID, ItemCategories cate)
	{
		if (cate == ItemCategories.ChiBang)
		{
			ConfigFashion.InitWing();
		}
		else if (cate == ItemCategories.ShiZhuang_JiaoYin)
		{
			ConfigFashion.InitJiaoYin();
		}
		else if (cate == ItemCategories.ShiZhuang_WuQi)
		{
			ConfigFashion.InitWuQi();
		}
		for (int i = 0; i < ConfigFashion.mFashionGoodIdList.Count; i++)
		{
			if (GoodsID == ConfigFashion.mFashionGoodIdList[i])
			{
				return true;
			}
		}
		return false;
	}

	public static void ClearData()
	{
		ConfigFashion.dic.Clear();
		ConfigFashion.mFashionLevelupData.Clear();
		ConfigFashion.mWingLevelupData.Clear();
		ConfigFashion.mJiaoYinLevelupData.Clear();
		ConfigFashion.mWuQiLevelupData.Clear();
	}

	private static Dictionary<int, WuQiShiZhuangMoXingVO> dic = new Dictionary<int, WuQiShiZhuangMoXingVO>();

	private static Dictionary<int, FashionLevelupVO> mFashionLevelupData = new Dictionary<int, FashionLevelupVO>();

	private static Dictionary<int, FashionLevelupVO> mWingLevelupData = new Dictionary<int, FashionLevelupVO>();

	private static Dictionary<int, FashionLevelupVO> mJiaoYinLevelupData = new Dictionary<int, FashionLevelupVO>();

	private static Dictionary<int, FashionLevelupVO> mWuQiLevelupData = new Dictionary<int, FashionLevelupVO>();

	private static List<int> mFashionGoodIdList = new List<int>();
}
