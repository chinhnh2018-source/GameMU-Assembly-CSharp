using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class SystemMagicManager
	{
		public SystemMagicManager MagicItemsDict
		{
			get
			{
				return this;
			}
		}

		public void LoadMagicItemsDict(SystemXmlItems systemMagicMgr)
		{
			lock (this._MagicItemsDict)
			{
				foreach (int key in systemMagicMgr.SystemXmlItemDict.Keys)
				{
					SystemXmlItem systemXmlItem = systemMagicMgr.SystemXmlItemDict[key];
					int intValue = systemXmlItem.GetIntValue("ID", -1);
					this._MagicItemsDict[intValue] = systemXmlItem;
				}
			}
		}

		public bool TryGetValue(int intKey, out SystemXmlItem systemMagic)
		{
			bool result;
			lock (this._MagicItemsDict)
			{
				result = this._MagicItemsDict.TryGetValue(intKey, out systemMagic);
			}
			return result;
		}

		private Dictionary<int, SystemXmlItem> _MagicItemsDict = new Dictionary<int, SystemXmlItem>();
	}
}
