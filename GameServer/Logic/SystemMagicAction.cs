using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class SystemMagicAction
	{
		static SystemMagicAction()
		{
			for (MagicActionIDs magicActionIDs = MagicActionIDs.FOREVER_ADDHIT; magicActionIDs < MagicActionIDs.MAX; magicActionIDs++)
			{
				string key = magicActionIDs.ToString().ToLower();
				SystemMagicAction.MagicActionIDsDict.Add(key, magicActionIDs);
			}
		}

		private static void PrintMaigcActionDictUsage(string name, Dictionary<string, MagicActionIDs> dict)
		{
			Console.WriteLine(string.Format("{0}个数{1}", name, dict.Count));
			foreach (KeyValuePair<string, MagicActionIDs> keyValuePair in dict)
			{
				Console.WriteLine(string.Format("{0} {1}", keyValuePair.Key, keyValuePair.Value));
			}
			Console.WriteLine("\r\n");
		}

		public static void PrintMaigcActionUsage()
		{
			SystemMagicAction.PrintMaigcActionDictUsage("MagicActionIDsDict", SystemMagicAction.MagicActionIDsDict);
		}

		private int FindIDByName(string name)
		{
			MagicActionIDs magicActionIDs;
			int result;
			if (SystemMagicAction.MagicActionIDsDict.TryGetValue(name.ToLower(), out magicActionIDs))
			{
				result = (int)magicActionIDs;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private MagicActionItem ParseParams(string item)
		{
			int num = item.IndexOf('(');
			string name;
			string text;
			if (-1 != num)
			{
				int num2 = item.IndexOf(')', num + 1);
				if (-1 == num2)
				{
					return null;
				}
				name = item.Substring(0, num);
				text = item.Substring(num + 1, num2 - num - 1);
			}
			else if ((num = item.IndexOf(',')) != -1)
			{
				name = item.Substring(0, num);
				text = item.Substring(num + 1, item.Length - num - 1);
			}
			else
			{
				name = item;
				text = "";
			}
			int num3 = this.FindIDByName(name);
			MagicActionItem result;
			if (num3 < 0)
			{
				result = null;
			}
			else
			{
				double[] array = null;
				if (text != "")
				{
					string[] array2 = text.Split(new char[]
					{
						','
					});
					array = new double[array2.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						if (char.IsDigit(array2[i], 0) || array2[i][0] == '-')
						{
							array[i] = Global.SafeConvertToDouble(array2[i]);
						}
						else
						{
							array[i] = (double)this.FindIDByName(array2[i]);
						}
					}
				}
				result = new MagicActionItem
				{
					MagicActionID = (MagicActionIDs)num3,
					MagicActionParams = array
				};
			}
			return result;
		}

		public List<MagicActionItem> ParseActionsInterface(string actions)
		{
			return this.ParseActions(actions);
		}

		private List<MagicActionItem> ParseActions(string actions)
		{
			List<MagicActionItem> list = new List<MagicActionItem>();
			string[] array = actions.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string item = array[i].Trim();
				MagicActionItem magicActionItem = this.ParseParams(item);
				if (null != magicActionItem)
				{
					list.Add(magicActionItem);
				}
			}
			return list;
		}

		private void ParseMagicActions(Dictionary<int, List<MagicActionItem>> dict, int id, string actions)
		{
			actions = actions.Trim();
			if (!("" == actions))
			{
				List<MagicActionItem> value = this.ParseActions(actions);
				dict[id] = value;
			}
		}

		public List<MagicActionItem> ParseActionsOutUse(string strAction)
		{
			return this.ParseActions(strAction);
		}

		public Dictionary<int, List<MagicActionItem>> MagicActionsDict
		{
			get
			{
				return this._MagicActionsDict;
			}
		}

		public Dictionary<int, int> MagicActionRelationDic
		{
			get
			{
				return this._MagicActionRelationDic;
			}
		}

		public void ParseMagicActions(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				string stringValue = systemMagicsMgr.SystemXmlItemDict[num].GetStringValue("MagicScripts");
				if (null != stringValue)
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._MagicActionsDict = dictionary;
		}

		public void ParseMagicActionRelations(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (int num in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				int intValue = systemMagicsMgr.SystemXmlItemDict[num].GetIntValue("NextMagicID", -1);
				if (-1 != intValue)
				{
					dictionary[intValue] = num;
				}
			}
			this._MagicActionRelationDic = dictionary;
		}

		public void ParseMagicActions2(SystemXmlItems systemMagicsMgr)
		{
		}

		public void ParseScanTypeActions2(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				string stringValue = systemMagicsMgr.SystemXmlItemDict[num].GetStringValue("ScanType");
				if (null != stringValue)
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._MagicActionsDict = dictionary;
		}

		public Dictionary<int, List<MagicActionItem>> GoodsActionsDict
		{
			get
			{
				return this._GoodsActionsDict;
			}
		}

		public void ParseGoodsActions(SystemXmlItems systemGoodsMgr)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemGoodsMgr.SystemXmlItemDict.Keys)
			{
				string stringValue = systemGoodsMgr.SystemXmlItemDict[num].GetStringValue("ExecMagic");
				if (!string.IsNullOrEmpty(stringValue))
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._GoodsActionsDict = dictionary;
		}

		public Dictionary<int, List<MagicActionItem>> NPCScriptActionsDict
		{
			get
			{
				return this._NPCScriptActionsDict;
			}
		}

		public void ParseNPCScriptActions(SystemXmlItems systemNPCScripts)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemNPCScripts.SystemXmlItemDict.Keys)
			{
				string stringValue = systemNPCScripts.SystemXmlItemDict[num].GetStringValue("ExecMagic");
				if (null != stringValue)
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._NPCScriptActionsDict = dictionary;
		}

		public Dictionary<int, List<MagicActionItem>> BossAIActionsDict
		{
			get
			{
				return this._BossAIActionsDict;
			}
		}

		public void ParseBossAIActions(SystemXmlItems systemBossAI)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemBossAI.SystemXmlItemDict.Keys)
			{
				string stringValue = systemBossAI.SystemXmlItemDict[num].GetStringValue("Action");
				if (!string.IsNullOrEmpty(stringValue))
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._BossAIActionsDict = dictionary;
		}

		public Dictionary<int, List<MagicActionItem>> ExtensionPropsActionsDict
		{
			get
			{
				return this._ExtensionPropsActionsDict;
			}
		}

		public void ParseExtensionPropsActions(SystemXmlItems systemExtensionProps)
		{
			Dictionary<int, List<MagicActionItem>> dictionary = new Dictionary<int, List<MagicActionItem>>();
			foreach (int num in systemExtensionProps.SystemXmlItemDict.Keys)
			{
				string stringValue = systemExtensionProps.SystemXmlItemDict[num].GetStringValue("MagicScripts");
				if (!string.IsNullOrEmpty(stringValue))
				{
					this.ParseMagicActions(dictionary, num, stringValue);
				}
			}
			this._ExtensionPropsActionsDict = dictionary;
		}

		private static Dictionary<string, MagicActionIDs> MagicActionIDsDict = new Dictionary<string, MagicActionIDs>();

		private Dictionary<int, List<MagicActionItem>> _MagicActionsDict = null;

		private Dictionary<int, int> _MagicActionRelationDic = null;

		private Dictionary<int, List<MagicActionItem>> _GoodsActionsDict = null;

		private Dictionary<int, List<MagicActionItem>> _NPCScriptActionsDict = null;

		private Dictionary<int, List<MagicActionItem>> _BossAIActionsDict = null;

		private Dictionary<int, List<MagicActionItem>> _ExtensionPropsActionsDict = null;
	}
}
