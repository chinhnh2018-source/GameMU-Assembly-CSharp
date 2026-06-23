using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class LogFilterConfig
	{
		public static bool InitConfig()
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			HashSet<int> hashSet3 = new HashSet<int>();
			HashSet<int> hashSet4 = new HashSet<int>();
			string text = null;
			try
			{
				text = Global.IsolateResPath("config\\Monitoring.xml");
				XElement xelement = ConfigHelper.Load(text);
				IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "Monitoring");
				if (null == xelements)
				{
					LogManager.WriteLog(1000, string.Format("未找到配置文件{0},请联系策划负责人获取", text), null, true);
					return false;
				}
				foreach (XElement xelement2 in xelements)
				{
					int num = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Type", 0L);
					int item = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Code", 0L);
					if (num == 1)
					{
						hashSet.Add(item);
					}
					else if (num == 2)
					{
						hashSet2.Add(item);
					}
					else if (num == 3)
					{
						hashSet3.Add(item);
					}
					else if (num == 4)
					{
						hashSet4.Add(item);
					}
					else
					{
						LogManager.WriteLog(1, string.Format("警告：配置文件{0},配置了未定义的类型!,{1}", text, xelement2.ToString()), null, true);
					}
				}
				LogFilterConfig.NeedLogGoodsIdHashSet = hashSet;
				LogFilterConfig.NeedLogMoneyTypeHashSet = hashSet2;
				LogFilterConfig.NoLogGameHashSet = hashSet3;
				LogFilterConfig.NoLogOperatorHashSet = hashSet4;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("读取配置{0}发生失败,{1}", text, ex.Message), ex, true);
				return false;
			}
			return true;
		}

		public static bool LogGoodsIdLog(int goodsId)
		{
			HashSet<int> needLogGoodsIdHashSet = LogFilterConfig.NeedLogGoodsIdHashSet;
			return null == needLogGoodsIdHashSet || needLogGoodsIdHashSet.Contains(goodsId);
		}

		public static bool LogMoneyTypeLog(int moneyType)
		{
			HashSet<int> needLogMoneyTypeHashSet = LogFilterConfig.NeedLogMoneyTypeHashSet;
			return null == needLogMoneyTypeHashSet || needLogMoneyTypeHashSet.Contains(moneyType);
		}

		public static bool LogGameLog(int type)
		{
			HashSet<int> noLogGameHashSet = LogFilterConfig.NoLogGameHashSet;
			return null != noLogGameHashSet && !noLogGameHashSet.Contains(type);
		}

		public static bool LogOperatorLog(int type)
		{
			HashSet<int> noLogOperatorHashSet = LogFilterConfig.NoLogOperatorHashSet;
			return null != noLogOperatorHashSet && !noLogOperatorHashSet.Contains(type);
		}

		private static HashSet<int> NeedLogGoodsIdHashSet;

		private static HashSet<int> NeedLogMoneyTypeHashSet;

		private static HashSet<int> NoLogOperatorHashSet;

		private static HashSet<int> NoLogGameHashSet;
	}
}
