using System;
using System.Collections.Generic;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic.BossAI
{
	public static class BossAICachingMgr
	{
		public static List<BossAIItem> FindCachingItem(int AIID, int triggerType)
		{
			string key = string.Format("{0}_{1}", AIID, triggerType);
			List<BossAIItem> list = null;
			List<BossAIItem> result;
			if (!BossAICachingMgr._BossAICachingDict.TryGetValue(key, out list))
			{
				result = null;
			}
			else
			{
				result = list;
			}
			return result;
		}

		private static ITriggerCondition ParseCondition(int ID, int triggerType, string condition)
		{
			ITriggerCondition triggerCondition = null;
			if (triggerType == 0)
			{
				triggerCondition = new BirthOnCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 1)
			{
				triggerCondition = new BloodChangedCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				string[] array = condition.Split(new char[]
				{
					'-'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("服务器端配置的Boss AI项，条件配置错误 ID={0}", ID), null, true);
					return null;
				}
				(triggerCondition as BloodChangedCondition).MinLifePercent = Global.SafeConvertToDouble(array[0]);
				(triggerCondition as BloodChangedCondition).MaxLifePercent = Global.SafeConvertToDouble(array[1]);
			}
			else if (triggerType == 2)
			{
				triggerCondition = new InjuredCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 3)
			{
				triggerCondition = new DeadCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 4)
			{
				triggerCondition = new AttackedCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
			}
			else if (triggerType == 5)
			{
				triggerCondition = new AllDeadCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				string[] array = condition.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					(triggerCondition as AllDeadCondition).MonsterIDList.Add(Global.SafeConvertToInt32(array[i]));
				}
			}
			else if (triggerType == 6)
			{
				triggerCondition = new LivingTimeCondition
				{
					TriggerType = (BossAITriggerTypes)triggerType
				};
				(triggerCondition as LivingTimeCondition).LivingMinutes = (long)Global.SafeConvertToInt32(condition);
			}
			return triggerCondition;
		}

		private static BossAIItem ParseBossAICachingItem(SystemXmlItem systemXmlItem)
		{
			BossAIItem bossAIItem = new BossAIItem
			{
				ID = systemXmlItem.GetIntValue("ID", -1),
				AIID = systemXmlItem.GetIntValue("AIID", -1),
				TriggerNum = systemXmlItem.GetIntValue("TriggerNum", -1),
				TriggerCD = systemXmlItem.GetIntValue("TriggerCD", -1),
				TriggerType = systemXmlItem.GetIntValue("TriggerType", -1),
				Desc = systemXmlItem.GetStringValue("Description")
			};
			bossAIItem.Condition = BossAICachingMgr.ParseCondition(bossAIItem.ID, bossAIItem.TriggerType, systemXmlItem.GetStringValue("Condition"));
			BossAIItem result;
			if (null == bossAIItem.Condition)
			{
				result = null;
			}
			else
			{
				result = bossAIItem;
			}
			return result;
		}

		public static void LoadBossAICachingItems(SystemXmlItems systemBossAI)
		{
			Dictionary<string, List<BossAIItem>> dictionary = new Dictionary<string, List<BossAIItem>>();
			foreach (int key in systemBossAI.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemBossAI.SystemXmlItemDict[key];
				BossAIItem bossAIItem = BossAICachingMgr.ParseBossAICachingItem(systemXmlItem);
				if (null != bossAIItem)
				{
					string key2 = string.Format("{0}_{1}", bossAIItem.AIID, bossAIItem.TriggerType);
					List<BossAIItem> list = null;
					if (!dictionary.TryGetValue(key2, out list))
					{
						list = new List<BossAIItem>();
						dictionary[key2] = list;
					}
					list.Add(bossAIItem);
				}
			}
			BossAICachingMgr._BossAICachingDict = dictionary;
		}

		private static Dictionary<string, List<BossAIItem>> _BossAICachingDict = new Dictionary<string, List<BossAIItem>>();
	}
}
