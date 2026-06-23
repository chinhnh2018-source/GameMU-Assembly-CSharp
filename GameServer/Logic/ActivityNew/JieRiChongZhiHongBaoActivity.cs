using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiChongZhiHongBaoActivity : Activity
	{
		public static JieRiChongZhiHongBaoActivity getInstance()
		{
			return JieRiChongZhiHongBaoActivity.instance;
		}

		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiChongZhiHongBao.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiChongZhiHongBao.xml"));
				if (null == xelement)
				{
					return false;
				}
				lock (this.Mutex)
				{
					this.HongBaoDict.Clear();
					XElement xelement2 = xelement.Element("Activities");
					if (null != xelement2)
					{
						this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
						this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
						this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
						this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
						this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
					}
					xelement2 = xelement.Element("GiftList");
					if (null != xelement2)
					{
						IEnumerable<XElement> enumerable = xelement2.Elements();
						foreach (XElement xelement3 in enumerable)
						{
							if (null != xelement3)
							{
								JieRiChongZhiHongBaoInfo jieRiChongZhiHongBaoInfo = new JieRiChongZhiHongBaoInfo();
								jieRiChongZhiHongBaoInfo.ID = (int)Global.GetSafeAttributeLong(xelement3, "ID");
								jieRiChongZhiHongBaoInfo.RechargeDifference = (int)Global.GetSafeAttributeLong(xelement3, "RechargeDifference");
								jieRiChongZhiHongBaoInfo.PlatformID = Global.GetSafeAttributeStr(xelement3, "PlatformID");
								jieRiChongZhiHongBaoInfo.RedPacketSize = (int)Global.GetSafeAttributeLong(xelement3, "RedPacketSize");
								jieRiChongZhiHongBaoInfo.Interval = Global.GetSafeAttributeIntArray(xelement3, "Interval", -1, ',');
								jieRiChongZhiHongBaoInfo.DurationTime = (int)Global.GetSafeAttributeLong(xelement3, "DurationTime");
								this.HongBaoDict.Add(jieRiChongZhiHongBaoInfo.ID, jieRiChongZhiHongBaoInfo);
							}
						}
					}
					base.PredealDateTime();
					this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public string GetKeyStr()
		{
			return this.ActivityKeyStr;
		}

		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		private static JieRiChongZhiHongBaoActivity instance = new JieRiChongZhiHongBaoActivity();

		private object Mutex = new object();

		private HashSet<long> recvDict = new HashSet<long>();

		private SortedDictionary<int, JieRiChongZhiHongBaoInfo> HongBaoDict = new SortedDictionary<int, JieRiChongZhiHongBaoInfo>();
	}
}
