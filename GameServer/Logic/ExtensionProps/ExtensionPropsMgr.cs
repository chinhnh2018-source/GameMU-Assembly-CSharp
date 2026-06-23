using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.ExtensionProps
{
	public class ExtensionPropsMgr
	{
		public static ExtensionPropItem FindCachingItem(int id)
		{
			ExtensionPropItem extensionPropItem = null;
			ExtensionPropItem result;
			if (!ExtensionPropsMgr._ExtensionPropsCachingDict.TryGetValue(id, out extensionPropItem))
			{
				result = null;
			}
			else
			{
				result = extensionPropItem;
			}
			return result;
		}

		private static Dictionary<int, byte> ParseDict(string str)
		{
			Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
			string[] array = str.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				dictionary[Global.SafeConvertToInt32(array[i])] = 1;
			}
			return dictionary;
		}

		private static ExtensionPropItem ParseCachingItem(SystemXmlItem systemXmlItem)
		{
			return new ExtensionPropItem
			{
				ID = systemXmlItem.GetIntValue("ID", -1),
				PrevTuoZhanShuXing = ExtensionPropsMgr.ParseDict(systemXmlItem.GetStringValue("PrevTuoZhanShuXing")),
				TargetType = systemXmlItem.GetIntValue("TargetTyp", -1),
				ActionType = systemXmlItem.GetIntValue("ActionType", -1),
				Probability = (int)(systemXmlItem.GetDoubleValue("Probability") * 100.0),
				NeedSkill = ExtensionPropsMgr.ParseDict(systemXmlItem.GetStringValue("NeedSkill")),
				Icon = systemXmlItem.GetIntValue("Icon", -1),
				TargetDecoration = systemXmlItem.GetIntValue("TargetDecoration", -1),
				DelayDecoration = systemXmlItem.GetIntValue("DelayDecoration", -1)
			};
		}

		public static void LoadCachingItems(SystemXmlItems systemExtensionProps)
		{
			Dictionary<int, ExtensionPropItem> dictionary = new Dictionary<int, ExtensionPropItem>();
			foreach (int key in systemExtensionProps.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemExtensionProps.SystemXmlItemDict[key];
				ExtensionPropItem extensionPropItem = ExtensionPropsMgr.ParseCachingItem(systemXmlItem);
				if (null != extensionPropItem)
				{
					dictionary[extensionPropItem.ID] = extensionPropItem;
				}
			}
			ExtensionPropsMgr._ExtensionPropsCachingDict = dictionary;
		}

		public static List<int> ProcessExtensionProps(List<int> extensionPropsIDList, int skillID, int actionType)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (null == extensionPropsIDList)
			{
				result = list;
			}
			else
			{
				Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
				for (int i = 0; i < extensionPropsIDList.Count; i++)
				{
					int num = extensionPropsIDList[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(num);
					if (null != extensionPropItem)
					{
						if (extensionPropItem.ActionType == actionType)
						{
							if (extensionPropItem.NeedSkill.Count > 0)
							{
								if (!extensionPropItem.NeedSkill.ContainsKey(skillID))
								{
									goto IL_C2;
								}
							}
							int randomNumber = Global.GetRandomNumber(0, 101);
							if (randomNumber <= extensionPropItem.Probability)
							{
								list.Add(num);
								dictionary[num] = 1;
							}
						}
					}
					IL_C2:;
				}
				List<int> list2 = new List<int>();
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(num);
					if (null != extensionPropItem)
					{
						if (extensionPropItem.PrevTuoZhanShuXing.Count > 0)
						{
							foreach (int key in extensionPropItem.PrevTuoZhanShuXing.Keys)
							{
								if (!dictionary.ContainsKey(key))
								{
								}
							}
						}
						list2.Add(num);
					}
				}
				result = list2;
			}
			return result;
		}

		public static void ExecuteExtensionPropsActions(List<int> list, IObject self, IObject obj)
		{
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(num);
					if (null != extensionPropItem)
					{
						IObject @object;
						if (0 == extensionPropItem.ActionType)
						{
							@object = self;
							if (0 != extensionPropItem.TargetType)
							{
								@object = obj;
							}
						}
						else
						{
							@object = obj;
							if (0 != extensionPropItem.TargetType)
							{
								@object = self;
							}
						}
						List<MagicActionItem> list2 = null;
						if (GameManager.SystemMagicActionMgr.BossAIActionsDict.TryGetValue(extensionPropItem.ID, out list2) && null != list2)
						{
							for (int j = 0; j < list2.Count; j++)
							{
								MagicAction.ProcessAction(self, @object, list2[j].MagicActionID, list2[j].MagicActionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							}
						}
						GameManager.ClientMgr.NotifySpriteExtensionPropsHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self, @object.GetObjectID(), (int)@object.CurrentPos.X, (int)@object.CurrentPos.Y, num);
					}
				}
			}
		}

		private static Dictionary<int, ExtensionPropItem> _ExtensionPropsCachingDict = new Dictionary<int, ExtensionPropItem>();
	}
}
