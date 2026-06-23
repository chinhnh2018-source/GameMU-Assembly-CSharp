using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class ThemeActivityConfig
	{
		public bool InList(int type)
		{
			return this.ConfigDict.ContainsKey(type);
		}

		public string GetFileName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ConfigDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public string GetActivityName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ActivityNameDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int GetEndData(int type)
		{
			int result;
			if (this.EndDataDict.ContainsKey(type))
			{
				result = this.EndDataDict[type];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public XElement GetFilterXElement()
		{
			string uri = "Config/ThemeActivityType.xml";
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			XElement result;
			if (null == xelement)
			{
				result = null;
			}
			else
			{
				XElement xelement2 = new XElement(xelement);
				List<XElement> list = new List<XElement>();
				IEnumerable<XElement> enumerable = xelement2.Elements();
				foreach (XElement xelement3 in enumerable)
				{
					int type = Convert.ToInt32(Global.GetSafeAttributeStr(xelement3, "Type"));
					int endData = this.GetEndData(type);
					if (endData > 0 && TimeUtil.NowDateTime() > Global.GetKaiFuTime().AddDays((double)endData))
					{
						list.Add(xelement3);
					}
				}
				list.ForEach(delegate(XElement x)
				{
					x.Remove();
				});
				result = xelement2;
			}
			return result;
		}

		public Dictionary<int, string> ConfigDict = new Dictionary<int, string>();

		public Dictionary<int, string> ActivityNameDict = new Dictionary<int, string>();

		public Dictionary<int, int> EndDataDict = new Dictionary<int, int>();

		public int ActivityOpenVavle;

		public List<int> openList = new List<int>();
	}
}
