using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting.KFBoCai
{
	public class KFBoCaiConfigManager
	{
		public static int LoadConfig(bool isReload = true)
		{
			try
			{
				List<CaiShuZiConfig> caiShuZiCfgList;
				KFBoCaiConfigManager.LoadCaiShuZi(out caiShuZiCfgList);
				List<CaiDaXiaoConfig> caiDaXiaoCfgList;
				KFBoCaiConfigManager.LoadCaiDaXiao(out caiDaXiaoCfgList);
				List<DuiHuanShangChengConfig> duiHuanShangChengCgfList;
				KFBoCaiConfigManager.Load_DuiHuanShangCheng(out duiHuanShangChengCgfList);
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					KFBoCaiConfigManager.CaiShuZiCfgList = caiShuZiCfgList;
				}
				lock (KFBoCaiConfigManager.CaiDaXiaoCfgList)
				{
					KFBoCaiConfigManager.CaiDaXiaoCfgList = caiDaXiaoCfgList;
				}
				lock (KFBoCaiConfigManager.DuiHuanShangChengCgfList)
				{
					KFBoCaiConfigManager.DuiHuanShangChengCgfList = duiHuanShangChengCgfList;
				}
				if (isReload)
				{
					KFBoCaiCaiDaXiao.GetInstance().UpData(true);
					KFBoCaiCaiShuzi.GetInstance().UpData(true);
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		public static CaiShuZiConfig GetCaiShuZiConfig()
		{
			CaiShuZiConfig caiShuZiConfig = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					int count = KFBoCaiConfigManager.CaiShuZiCfgList.Count;
					int i = 0;
					while (i < count)
					{
						if (DateTime.Parse(KFBoCaiConfigManager.CaiShuZiCfgList[i].JieShuShiJian) < TimeUtil.NowDateTime())
						{
							KFBoCaiConfigManager.CaiShuZiCfgList.RemoveAt(i);
							count = KFBoCaiConfigManager.CaiShuZiCfgList.Count;
						}
						else
						{
							if (caiShuZiConfig == null || DateTime.Parse(caiShuZiConfig.KaiQiShiJian) > DateTime.Parse(KFBoCaiConfigManager.CaiShuZiCfgList[i].KaiQiShiJian))
							{
								caiShuZiConfig = KFBoCaiConfigManager.CaiShuZiCfgList[i];
							}
							i++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return caiShuZiConfig;
		}

		public static CaiShuZiConfig GetCaiShuZiConfig(int ID)
		{
			CaiShuZiConfig result = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					result = KFBoCaiConfigManager.CaiShuZiCfgList.Find((CaiShuZiConfig x) => x.ID == ID);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return result;
		}

		public static CaiDaXiaoConfig GetCaiDaXiaoConfig()
		{
			CaiDaXiaoConfig caiDaXiaoConfig = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiDaXiaoCfgList)
				{
					int count = KFBoCaiConfigManager.CaiDaXiaoCfgList.Count;
					int i = 0;
					while (i < count)
					{
						if (DateTime.Parse(KFBoCaiConfigManager.CaiDaXiaoCfgList[i].HuoDongJieSu) < TimeUtil.NowDateTime())
						{
							KFBoCaiConfigManager.CaiDaXiaoCfgList.RemoveAt(i);
							count = KFBoCaiConfigManager.CaiDaXiaoCfgList.Count;
						}
						else
						{
							if (caiDaXiaoConfig == null || DateTime.Parse(caiDaXiaoConfig.HuoDongKaiQi) > DateTime.Parse(KFBoCaiConfigManager.CaiDaXiaoCfgList[i].HuoDongKaiQi))
							{
								caiDaXiaoConfig = KFBoCaiConfigManager.CaiDaXiaoCfgList[i];
							}
							i++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return caiDaXiaoConfig;
		}

		private static bool LoadCaiShuZi(out List<CaiShuZiConfig> _CaiShuZiCfgList)
		{
			_CaiShuZiCfgList = new List<CaiShuZiConfig>();
			try
			{
				XElement xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/CaiShuZi.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/CaiShuZi.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					CaiShuZiConfig caiShuZiConfig = new CaiShuZiConfig();
					caiShuZiConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
					caiShuZiConfig.XiaoHaoDaiBi = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "XiaoHaoDaiBi", 0L);
					caiShuZiConfig.ChuFaBiZhong = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ChuFaBiZhong", 0L);
					caiShuZiConfig.BuChongTiaoJian = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "BuChongTiaoJian", 0L);
					caiShuZiConfig.XiTongChouCheng = ConfigHelper.GetElementAttributeValueDouble(xelement2, "XiTongChouCheng", 0.0);
					caiShuZiConfig.ShangChengKaiGuan = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ShangChengKaiGuan", 0L);
					caiShuZiConfig.AnNiuList = new List<CaiShuZiAnNiu>();
					foreach (string text in ConfigHelper.GetElementAttributeValue(xelement2, "ZhongJiangFanLi", "").Split(new char[]
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
						caiShuZiConfig.KaiQiShiJian = ConfigHelper.GetElementAttributeValue(xelement2, "KaiQiShiJian", "");
						DateTime.Parse(caiShuZiConfig.KaiQiShiJian);
						caiShuZiConfig.JieShuShiJian = ConfigHelper.GetElementAttributeValue(xelement2, "JieShuShiJian", "");
						DateTime.Parse(caiShuZiConfig.JieShuShiJian);
						caiShuZiConfig.KaiJiangShiJian = ConfigHelper.GetElementAttributeValue(xelement2, "KaiJiangShiJian", "");
						DateTime.Parse(caiShuZiConfig.KaiJiangShiJian);
						_CaiShuZiCfgList.Add(caiShuZiConfig);
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
				XElement xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/DuiHuanShangCheng.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/DuiHuanShangCheng.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					DuiHuanShangChengConfig duiHuanShangChengConfig = new DuiHuanShangChengConfig();
					duiHuanShangChengConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
					duiHuanShangChengConfig.DaiBiJiaGe = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "DaiBiJiaGe", 0L);
					duiHuanShangChengConfig.MeiRiShangXianDan = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "MeiRiShangXianDan", 0L);
					duiHuanShangChengConfig.Name = ConfigHelper.GetElementAttributeValue(xelement2, "Name", "");
					duiHuanShangChengConfig.WuPinID = ConfigHelper.GetElementAttributeValue(xelement2, "WuPinID", "");
					_DuiHuanShangChengCgfList.Add(duiHuanShangChengConfig);
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
				XElement xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/CaiDaXiao.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
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
						caiDaXiaoConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						caiDaXiaoConfig.XiaoHaoDaiBi = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "XiaoHaoDaiBi", 0L);
						caiDaXiaoConfig.ShangChengKaiGuan = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ShangChengKaiGuan", 0L);
						caiDaXiaoConfig.HuoDongKaiQi = ConfigHelper.GetElementAttributeValue(xelement2, "HuoDongKaiQi", "");
						caiDaXiaoConfig.ZhuShuShangXian = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ZhuShuShangXian", 0L);
						DateTime.Parse(caiDaXiaoConfig.HuoDongKaiQi);
						caiDaXiaoConfig.HuoDongJieSu = ConfigHelper.GetElementAttributeValue(xelement2, "HuoDongJieSu", "");
						DateTime.Parse(caiDaXiaoConfig.HuoDongJieSu);
						caiDaXiaoConfig.MeiRiKaiQi = ConfigHelper.GetElementAttributeValue(xelement2, "MeiRiKaiQi", "");
						DateTime.Parse(caiDaXiaoConfig.MeiRiKaiQi);
						caiDaXiaoConfig.MeiRiJieSu = ConfigHelper.GetElementAttributeValue(xelement2, "MeiRiJieSu", "");
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

		private const string CaiShuZi = "Config/CaiShuZi.xml";

		private const string CaiDaXiao = "Config/CaiDaXiao.xml";

		private const string DuiHuanShangCheng = "Config/DuiHuanShangCheng.xml";

		private static List<CaiShuZiConfig> CaiShuZiCfgList = new List<CaiShuZiConfig>();

		private static List<CaiDaXiaoConfig> CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();

		private static List<DuiHuanShangChengConfig> DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();
	}
}
