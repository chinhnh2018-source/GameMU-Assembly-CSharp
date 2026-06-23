using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class VersionSystemOpenManager
	{
		public void LoadVersionSystemOpenData()
		{
			lock (this._VersionSystemOpenMutex)
			{
				string text = "Config/VersionSystemOpen.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
				if (null == xelement)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
				}
				else
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					this.VersionSystemOpenDict.Clear();
					this.SystemOpenDict.Clear();
					foreach (XElement xml in enumerable)
					{
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "SystemName");
						int num = (int)Global.GetSafeAttributeLong(xml, "IsOpen");
						this.VersionSystemOpenDict[safeAttributeStr] = num;
						int num2 = (int)Global.GetSafeAttributeLong(xml, "ID");
						if (num2 >= 100000)
						{
							if (num2 >= 100000 && num2 < 120000)
							{
								this.SystemOpenDict[num2 - 100000] = (num > 0);
							}
							else
							{
								this.SystemOpenDict[num2] = (num > 0);
							}
						}
					}
				}
			}
		}

		public bool IsVersionSystemOpen(string key)
		{
			int num = 0;
			bool result = false;
			lock (this._VersionSystemOpenMutex)
			{
				if (this.VersionSystemOpenDict.TryGetValue(key, out num))
				{
					result = (num == 1);
				}
			}
			return result;
		}

		public bool IsVersionSystemOpen(int id)
		{
			bool result = false;
			lock (this._VersionSystemOpenMutex)
			{
				if (!this.SystemOpenDict.TryGetValue(id, out result))
				{
					return true;
				}
			}
			return result;
		}

		private object _VersionSystemOpenMutex = new object();

		private Dictionary<string, int> VersionSystemOpenDict = new Dictionary<string, int>();

		private Dictionary<int, bool> SystemOpenDict = new Dictionary<int, bool>();
	}
}
