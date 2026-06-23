using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class SystemGoodsManager
	{
		public Dictionary<string, SystemXmlItem> GoodsItemsDict
		{
			get
			{
				return this._GoodsItemsDict;
			}
		}

		public void LoadGoodsItemsDict(SystemXmlItems systemGoodsMgr)
		{
			Dictionary<string, SystemXmlItem> dictionary = new Dictionary<string, SystemXmlItem>();
			foreach (int key in systemGoodsMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemGoodsMgr.SystemXmlItemDict[key];
				string stringValue = systemXmlItem.GetStringValue("Title");
				dictionary[stringValue] = systemXmlItem;
			}
			this._GoodsItemsDict = dictionary;
		}

		private Dictionary<string, SystemXmlItem> _GoodsItemsDict = null;
	}
}
