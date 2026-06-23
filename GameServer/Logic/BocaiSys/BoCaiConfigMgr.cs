using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	public class BoCaiConfigMgr
	{
		public static int LoadConfig(bool isReload = true)
		{
			try
			{
				List<CaiShuZiConfig> caiShuZiCfgList;
				BoCaiConfigMgr.LoadCaiShuZi(out caiShuZiCfgList);
				List<CaiDaXiaoConfig> caiDaXiaoCfgList;
				BoCaiConfigMgr.LoadCaiDaXiao(out caiDaXiaoCfgList);
				List<BoCaiConfigMgr.DaiBiShiYongData> daiBiShiYongCfgList;
				BoCaiConfigMgr.LoadDaiBiShiYong(out daiBiShiYongCfgList);
				List<DuiHuanShangChengConfig> duiHuanShangChengCgfList;
				BoCaiConfigMgr.Load_DuiHuanShangCheng(out duiHuanShangChengCgfList);
				lock (BoCaiConfigMgr.CaiShuZiCfgList)
				{
					BoCaiConfigMgr.CaiShuZiCfgList = caiShuZiCfgList;
				}
				lock (BoCaiConfigMgr.CaiDaXiaoCfgList)
				{
					BoCaiConfigMgr.CaiDaXiaoCfgList = caiDaXiaoCfgList;
				}
				lock (BoCaiConfigMgr.DaiBiShiYongCfgList)
				{
					BoCaiConfigMgr.DaiBiShiYongCfgList = daiBiShiYongCfgList;
				}
				lock (BoCaiConfigMgr.DuiHuanShangChengCgfList)
				{
					BoCaiConfigMgr.DuiHuanShangChengCgfList = duiHuanShangChengCgfList;
				}
				if (isReload)
				{
					BoCaiCaiDaXiao.GetInstance().BigTimeUpData(true);
					BoCaiCaiShuZi.GetInstance().BigTimeUpData(true);
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		public static DuiHuanShangChengConfig GetBoCaiShopConfig(int ID, string WuPinID)
		{
			DuiHuanShangChengConfig result = null;
			try
			{
				result = BoCaiConfigMgr.DuiHuanShangChengCgfList.Find((DuiHuanShangChengConfig x) => x.ID == ID && x.WuPinID == WuPinID);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return result;
		}

		public static bool CanReplaceMoney(DaiBiSySType type)
		{
			try
			{
				BoCaiConfigMgr.DaiBiShiYongData daiBiShiYongData = BoCaiConfigMgr.DaiBiShiYongCfgList.Find((BoCaiConfigMgr.DaiBiShiYongData x) => x.XiTongMingCheng.Equals(type.ToString()));
				if (null != daiBiShiYongData)
				{
					return daiBiShiYongData.IsOpen;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private static bool LoadCaiShuZi(out List<CaiShuZiConfig> _CaiShuZiCfgList)
		{
			_CaiShuZiCfgList = new List<CaiShuZiConfig>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/CaiShuZi.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/CaiShuZi.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						CaiShuZiConfig caiShuZiConfig = new CaiShuZiConfig();
						caiShuZiConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						caiShuZiConfig.XiaoHaoDaiBi = (int)Global.GetSafeAttributeLong(xelement2, "XiaoHaoDaiBi");
						caiShuZiConfig.ChuFaBiZhong = (int)Global.GetSafeAttributeLong(xelement2, "ChuFaBiZhong");
						caiShuZiConfig.BuChongTiaoJian = (int)Global.GetSafeAttributeLong(xelement2, "BuChongTiaoJian");
						caiShuZiConfig.XiTongChouCheng = Global.GetSafeAttributeDouble(xelement2, "XiTongChouCheng");
						caiShuZiConfig.ShangChengKaiGuan = (int)Global.GetSafeAttributeLong(xelement2, "ShangChengKaiGuan");
						caiShuZiConfig.AnNiuList = new List<CaiShuZiAnNiu>();
						foreach (string text in Global.GetSafeAttributeStr(xelement2, "ZhongJiangFanLi").Split(new char[]
						{
							'|'
						}))
						{
							string[] array2 = text.Split(new char[]
							{
								','
							});
							if (array2.Length < 2)
							{
								LogManager.WriteLog(1000, string.Format("{0}解析出现 AnNiuName err, myData.ID={1}", "Config/CaiShuZi.xml", caiShuZiConfig.ID), null, true);
							}
							else
							{
								CaiShuZiAnNiu caiShuZiAnNiu = new CaiShuZiAnNiu();
								caiShuZiAnNiu.NO = Convert.ToInt32(array2[0]);
								caiShuZiAnNiu.Percent = Convert.ToDouble(array2[1]);
								caiShuZiConfig.AnNiuList.Add(caiShuZiAnNiu);
								if (caiShuZiAnNiu.NO != caiShuZiConfig.AnNiuList.Count)
								{
									LogManager.WriteLog(1000, string.Format("{0}解析出现 d.NO != myData.AnNiuList.Count  myData.ID={1}", "Config/CaiShuZi.xml", caiShuZiConfig.ID), null, true);
								}
							}
						}
						if (caiShuZiConfig.AnNiuList.Count < 3)
						{
							LogManager.WriteLog(1000, string.Format("{0}解析出现 myData.AnNiuList.Count < 3 err, myData.ID={1}", "Config/CaiShuZi.xml", caiShuZiConfig.ID), null, true);
						}
						else
						{
							caiShuZiConfig.JieShuShiJian = Global.GetSafeAttributeStr(xelement2, "JieShuShiJian");
							caiShuZiConfig.KaiQiShiJian = Global.GetSafeAttributeStr(xelement2, "KaiQiShiJian");
							DateTime.Parse(caiShuZiConfig.KaiQiShiJian);
							caiShuZiConfig.KaiJiangShiJian = Global.GetSafeAttributeStr(xelement2, "KaiJiangShiJian");
							DateTime.Parse(caiShuZiConfig.KaiJiangShiJian);
							_CaiShuZiCfgList.Add(caiShuZiConfig);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/CaiShuZi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private static bool Load_DuiHuanShangCheng(out List<DuiHuanShangChengConfig> _DuiHuanShangChengCgfList)
		{
			_DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/DuiHuanShangCheng.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/DuiHuanShangCheng.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						DuiHuanShangChengConfig duiHuanShangChengConfig = new DuiHuanShangChengConfig();
						duiHuanShangChengConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						duiHuanShangChengConfig.DaiBiJiaGe = (int)Global.GetSafeAttributeLong(xelement2, "DaiBiJiaGe");
						duiHuanShangChengConfig.MeiRiShangXianDan = (int)Global.GetSafeAttributeLong(xelement2, "MeiRiShangXianDan");
						duiHuanShangChengConfig.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						duiHuanShangChengConfig.WuPinID = Global.GetSafeAttributeStr(xelement2, "WuPinID");
						if (null == GlobalNew.ParseGoodsData(duiHuanShangChengConfig.WuPinID))
						{
							LogManager.WriteLog(1000, string.Format("{0}解析 WuPinID={1} err", "Config/DuiHuanShangCheng.xml", duiHuanShangChengConfig.WuPinID), null, true);
						}
						else
						{
							_DuiHuanShangChengCgfList.Add(duiHuanShangChengConfig);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/DuiHuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private static bool LoadCaiDaXiao(out List<CaiDaXiaoConfig> _CaiDaXiaoCfgList)
		{
			_CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/CaiDaXiao.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/CaiDaXiao.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						CaiDaXiaoConfig caiDaXiaoConfig = new CaiDaXiaoConfig();
						caiDaXiaoConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						caiDaXiaoConfig.XiaoHaoDaiBi = (int)Global.GetSafeAttributeLong(xelement2, "XiaoHaoDaiBi");
						caiDaXiaoConfig.ShangChengKaiGuan = (int)Global.GetSafeAttributeLong(xelement2, "ShangChengKaiGuan");
						caiDaXiaoConfig.ZhuShuShangXian = (int)Global.GetSafeAttributeLong(xelement2, "ZhuShuShangXian");
						caiDaXiaoConfig.HuoDongJieSu = Global.GetSafeAttributeStr(xelement2, "HuoDongJieSu");
						caiDaXiaoConfig.HuoDongKaiQi = Global.GetSafeAttributeStr(xelement2, "HuoDongKaiQi");
						DateTime.Parse(caiDaXiaoConfig.HuoDongKaiQi);
						caiDaXiaoConfig.MeiRiKaiQi = Global.GetSafeAttributeStr(xelement2, "MeiRiKaiQi");
						DateTime.Parse(caiDaXiaoConfig.MeiRiKaiQi);
						caiDaXiaoConfig.MeiRiJieSu = Global.GetSafeAttributeStr(xelement2, "MeiRiJieSu");
						DateTime.Parse(caiDaXiaoConfig.MeiRiJieSu);
						_CaiDaXiaoCfgList.Add(caiDaXiaoConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/CaiDaXiao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private static bool LoadDaiBiShiYong(out List<BoCaiConfigMgr.DaiBiShiYongData> _DaiBiShiYongCfgList)
		{
			_DaiBiShiYongCfgList = new List<BoCaiConfigMgr.DaiBiShiYongData>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/DaiBiShiYong.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/DaiBiShiYong.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						BoCaiConfigMgr.DaiBiShiYongData daiBiShiYongData = new BoCaiConfigMgr.DaiBiShiYongData();
						daiBiShiYongData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						daiBiShiYongData.IsOpen = ((int)Global.GetSafeAttributeLong(xelement2, "DaiBiKaiGuan") > 0);
						daiBiShiYongData.XiTongMingCheng = Global.GetSafeAttributeStr(xelement2, "XiTongMingCheng").Trim();
						daiBiShiYongData.ZhongWenMingCheng = Global.GetSafeAttributeStr(xelement2, "ZhongWenMingCheng");
						_DaiBiShiYongCfgList.Add(daiBiShiYongData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/DuiHuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string CaiShuZi = "Config/CaiShuZi.xml";

		private const string CaiDaXiao = "Config/CaiDaXiao.xml";

		private const string DaiBiShiYong = "Config/DaiBiShiYong.xml";

		private const string DuiHuanShangCheng = "Config/DuiHuanShangCheng.xml";

		public const string StrHuanLeDuiHuanOpen = "HuanLeDuiHuan";

		private static List<CaiShuZiConfig> CaiShuZiCfgList = new List<CaiShuZiConfig>();

		private static List<CaiDaXiaoConfig> CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();

		private static List<BoCaiConfigMgr.DaiBiShiYongData> DaiBiShiYongCfgList = new List<BoCaiConfigMgr.DaiBiShiYongData>();

		private static List<DuiHuanShangChengConfig> DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();

		public class DaiBiShiYongData
		{
			public int ID;

			public bool IsOpen;

			public string XiTongMingCheng;

			public string ZhongWenMingCheng;
		}
	}
}
