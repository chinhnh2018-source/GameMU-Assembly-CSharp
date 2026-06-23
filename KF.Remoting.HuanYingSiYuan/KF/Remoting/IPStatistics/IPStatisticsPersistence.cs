using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting.IPStatistics
{
	public class IPStatisticsPersistence
	{
		private IPStatisticsPersistence()
		{
		}

		public void LoadConfig()
		{
			try
			{
				List<StatisticsControl> list = new List<StatisticsControl>();
				string fileName = "Config\\IPStaristicsConfig.xml";
				XElement xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null != xelement)
				{
					foreach (XElement xelement2 in xelement.Elements())
					{
						list.Add(new StatisticsControl
						{
							ID = Convert.ToInt32(xelement2.Attribute("ID").Value.ToString()),
							ParamType = Convert.ToInt32(xelement2.Attribute("ParamType").Value.ToString()),
							ParamLimit = Convert.ToInt32(xelement2.Attribute("ParamLimit").Value.ToString()),
							ComParamType = Convert.ToInt32(xelement2.Attribute("ComParamType").Value.ToString()),
							ComParamLimit = Convert.ToDouble(xelement2.Attribute("ComParamLimit").Value.ToString()),
							OperaType = Convert.ToInt32(xelement2.Attribute("OperaType").Value.ToString()),
							OperaParam = Convert.ToInt32(xelement2.Attribute("OperaParam").Value.ToString())
						});
					}
					fileName = "Config\\IPPassList.xml";
					xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes));
					if (null != xelement)
					{
						List<IPPassList> list2 = new List<IPPassList>();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							IPPassList ippassList = new IPPassList();
							ippassList.ID = Convert.ToInt32(xelement2.Attribute("ID").Value.ToString());
							string text = xelement2.Attribute("MinIP").Value.ToString();
							ippassList.MinIP = IpHelper.IpToInt(text);
							string text2 = xelement2.Attribute("MaxIP").Value.ToString();
							ippassList.MaxIP = IpHelper.IpToInt(text2);
							list2.Add(ippassList);
						}
						this._IPControlList = list;
						this._IPPassList = list2;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "IPStaristicsConfig.InitConfig failed!", ex, true);
			}
		}

		public bool isCanPassIP(long ipAsInt)
		{
			bool result;
			lock (this._IPPassList)
			{
				if (this._IPPassList == null || this._IPPassList.Count == 0)
				{
					result = false;
				}
				else
				{
					foreach (IPPassList ippassList2 in this._IPPassList)
					{
						if (ippassList2.MinIP <= ipAsInt && ippassList2.MaxIP >= ipAsInt)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public static readonly IPStatisticsPersistence Instance = new IPStatisticsPersistence();

		public List<StatisticsControl> _IPControlList = new List<StatisticsControl>();

		public List<IPPassList> _IPPassList = new List<IPPassList>();
	}
}
