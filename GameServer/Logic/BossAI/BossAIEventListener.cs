using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.BossAI
{
	public class BossAIEventListener : IEventListener
	{
		private BossAIEventListener()
		{
		}

		public static BossAIEventListener getInstance()
		{
			return BossAIEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			Monster monster = null;
			GameClient obj = null;
			List<BossAIItem> list = new List<BossAIItem>();
			if (eventObject.getEventType() == 16)
			{
				monster = (eventObject as MonsterBirthOnEventObject).getMonster();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 0);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									monster.RecBossAI(list2[i]);
									list.Add(list2[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 11)
			{
				monster = (eventObject as MonsterDeadEventObject).getMonster();
				obj = (eventObject as MonsterDeadEventObject).getAttacker();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 3);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									monster.RecBossAI(list2[i]);
									list.Add(list2[i]);
								}
							}
						}
					}
					List<BossAIItem> list3 = BossAICachingMgr.FindCachingItem(aiid, 5);
					if (null != list3)
					{
						for (int i = 0; i < list3.Count; i++)
						{
							if (monster.CanExecBossAI(list3[i]))
							{
								bool flag3 = false;
								List<int> monsterIDList = (list3[i].Condition as AllDeadCondition).MonsterIDList;
								for (int j = 0; j < monsterIDList.Count; j++)
								{
									List<object> list4 = GameManager.MonsterMgr.FindMonsterByExtensionID(monster.CurrentCopyMapID, monsterIDList[j]);
									if (list4.Count > 0)
									{
										flag3 = true;
										break;
									}
								}
								if (!flag3)
								{
									monster.RecBossAI(list3[i]);
									list.Add(list3[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 17)
			{
				monster = (eventObject as MonsterInjuredEventObject).getMonster();
				obj = (eventObject as MonsterInjuredEventObject).getAttacker();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 2);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									monster.RecBossAI(list2[i]);
									list.Add(list2[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 19)
			{
				monster = (eventObject as MonsterAttackedEventObject).getMonster();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 4);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									monster.RecBossAI(list2[i]);
									list.Add(list2[i]);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 18)
			{
				monster = (eventObject as MonsterBlooadChangedEventObject).getMonster();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 1);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									double num = monster.VLife / monster.MonsterInfo.VLifeMax;
									bool flag7 = num >= (list2[i].Condition as BloodChangedCondition).MinLifePercent && num <= (list2[i].Condition as BloodChangedCondition).MaxLifePercent;
									if (flag7)
									{
										monster.RecBossAI(list2[i]);
										list.Add(list2[i]);
									}
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 20)
			{
				monster = (eventObject as MonsterLivingTimeEventObject).getMonster();
				int aiid = monster.MonsterInfo.AIID;
				if (aiid > 0)
				{
					List<BossAIItem> list2 = BossAICachingMgr.FindCachingItem(aiid, 6);
					if (null != list2)
					{
						lock (monster.TriggerMutex)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (monster.CanExecBossAI(list2[i]))
								{
									bool flag7 = monster.GetMonsterLivingTicks() >= (list2[i].Condition as LivingTimeCondition).LivingMinutes * 60L * 1000L;
									if (flag7)
									{
										monster.RecBossAI(list2[i]);
										list.Add(list2[i]);
									}
								}
							}
						}
					}
				}
			}
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					BossAIItem bossAIItem = list[i];
					List<MagicActionItem> list5 = null;
					if (GameManager.SystemMagicActionMgr.BossAIActionsDict.TryGetValue(bossAIItem.ID, out list5) && null != list5)
					{
						for (int j = 0; j < list5.Count; j++)
						{
							MagicAction.ProcessAction(monster, obj, list5[j].MagicActionID, list5[j].MagicActionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							if (!string.IsNullOrEmpty(bossAIItem.Desc))
							{
								GameManager.ClientMgr.BroadSpecialHintText(monster.CurrentMapCode, monster.CurrentCopyMapID, bossAIItem.Desc);
							}
						}
					}
				}
			}
		}

		private static BossAIEventListener instance = new BossAIEventListener();
	}
}
