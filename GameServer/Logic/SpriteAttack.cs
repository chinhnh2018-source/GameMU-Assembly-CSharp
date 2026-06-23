using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.UnionAlly;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	internal class SpriteAttack
	{
		private static int VerifyEnemyID(IObject attacker, int mapCode, int enemyID, int enemyX, int enemyY)
		{
			int num = enemyID;
			int result;
			if (-1 == enemyID)
			{
				result = num;
			}
			else
			{
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
				if (spriteType == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (!Global.CompareTwoPointGridXY(monster.MonsterZoneNode.MapCode, new Point((double)enemyX, (double)enemyY), monster.SafeCoordinate))
						{
							num = -1;
						}
					}
					else
					{
						num = -1;
					}
				}
				else if (spriteType == GSpriteTypes.NPC)
				{
					num = -1;
				}
				else if (spriteType == GSpriteTypes.Pet)
				{
					num = -1;
				}
				else if (spriteType == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						if (!Global.CompareTwoPointGridXY(biaoCheItem.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)biaoCheItem.PosX, (double)biaoCheItem.PosY)))
						{
							num = -1;
						}
					}
					else
					{
						num = -1;
					}
				}
				else if (spriteType == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						if (!Global.CompareTwoPointGridXY(junQiItem.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)junQiItem.PosX, (double)junQiItem.PosY)))
						{
							num = -1;
						}
					}
					else
					{
						num = -1;
					}
				}
				else if (spriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						if (!Global.CompareTwoPointGridXY(fakeRoleItem.MyRoleDataMini.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)fakeRoleItem.MyRoleDataMini.PosX, (double)fakeRoleItem.MyRoleDataMini.PosY)))
						{
							num = -1;
						}
					}
					else
					{
						num = -1;
					}
				}
				else
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null != gameClient)
					{
						if (!Global.CompareTwoPointGridXY(gameClient.ClientData.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY)))
						{
							num = -1;
						}
					}
					else
					{
						num = -1;
					}
				}
				result = num;
			}
			return result;
		}

		private static bool IsOpposition(IObject me, int mapCode, int enemyID)
		{
			bool flag = true;
			bool result;
			if (-1 == enemyID)
			{
				result = flag;
			}
			else
			{
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
				if (spriteType == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (me is GameClient)
						{
							flag = Global.IsOpposition(me as GameClient, monster);
						}
						else
						{
							flag = (me is Monster && Global.IsOpposition(me as Monster, monster));
						}
					}
				}
				else if (spriteType == GSpriteTypes.NPC)
				{
					flag = false;
				}
				else if (spriteType == GSpriteTypes.Pet)
				{
					flag = false;
				}
				else if (spriteType == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						if (me is GameClient)
						{
							flag = Global.IsOpposition(me as GameClient, biaoCheItem);
						}
						else
						{
							flag = (me is Monster && Global.IsOpposition(me as Monster, biaoCheItem));
						}
					}
					else
					{
						flag = false;
					}
				}
				else if (spriteType == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						if (me is GameClient)
						{
							flag = Global.IsOpposition(me as GameClient, junQiItem);
						}
						else
						{
							flag = (me is Monster && Global.IsOpposition(me as Monster, junQiItem));
						}
					}
					else
					{
						flag = false;
					}
				}
				else if (spriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						if (me is GameClient)
						{
							flag = Global.IsOpposition(me as GameClient, fakeRoleItem);
						}
						else
						{
							flag = (me is Monster && Global.IsOpposition(me as Monster, fakeRoleItem));
						}
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null != gameClient)
					{
						if (me is GameClient)
						{
							flag = Global.IsOpposition(me as GameClient, gameClient);
						}
						else
						{
							flag = (me is Monster && Global.IsOpposition(me as Monster, gameClient));
						}
					}
				}
				result = flag;
			}
			return result;
		}

		public static bool JugeMagicDistance(SystemXmlItem systemMagic, IObject attacker, int enemy, int enemyX, int enemyY, int magicCode, bool forceNotAttack = false)
		{
			int intValue = systemMagic.GetIntValue("AttackDistance", -1);
			Point point = new Point((double)enemyX, (double)enemyY);
			if (systemMagic.GetIntValue("MagicType", -1) == 1)
			{
				int intValue2 = systemMagic.GetIntValue("TargetType", -1);
				if (1 == intValue2)
				{
					return true;
				}
				if (2 == intValue2 || 3 == intValue2 || 4 == intValue2)
				{
					Point end = new Point((double)enemyX, (double)enemyY);
					if (-1 != enemy)
					{
						SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out end);
						if (Global.GetTwoPointDistance(point, end) < 300.0)
						{
							end = point;
						}
					}
					if (systemMagic.GetIntValue("ActionType", -1) == 0 && !forceNotAttack)
					{
						if (Global.GetTwoPointDistance(attacker.CurrentPos, end) > (double)intValue)
						{
							return false;
						}
					}
					else if (Global.GetTwoPointDistance(attacker.CurrentPos, end) > (double)intValue)
					{
						return false;
					}
				}
			}
			else
			{
				if (1 == systemMagic.GetIntValue("TargetPos", -1))
				{
					return true;
				}
				Point end2;
				if (2 == systemMagic.GetIntValue("TargetPos", -1))
				{
					end2 = new Point((double)enemyX, (double)enemyY);
					if (-1 != enemy)
					{
						if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out end2))
						{
							end2 = new Point((double)enemyX, (double)enemyY);
						}
						else if (Global.GetTwoPointDistance(point, end2) < 300.0)
						{
							end2 = point;
						}
					}
				}
				else
				{
					end2 = new Point((double)enemyX, (double)enemyY);
				}
				if (Global.GetTwoPointDistance(attacker.CurrentPos, end2) > (double)intValue)
				{
					return false;
				}
			}
			return true;
		}

		public static bool CanUseMaigc(GameClient client, int magicCode)
		{
			lock (client.ClientData.SkillIdHashSet)
			{
				if (client.ClientData.SkillIdHashSet.Contains(magicCode))
				{
					return true;
				}
				bool flag2;
				if (client.ClientData.SkillDataList != null)
				{
					flag2 = (null == client.ClientData.SkillDataList.Find((SkillData x) => x.SkillID == magicCode));
				}
				else
				{
					flag2 = true;
				}
				if (!flag2)
				{
					client.ClientData.SkillIdHashSet.Add(magicCode);
					return true;
				}
			}
			return false;
		}

		private static bool CheckEnemyClientPostion(GameClient client, ref int enemyID, int realEnemyX, int realEnemyY)
		{
			bool result;
			if (-1 == enemyID)
			{
				result = true;
			}
			else if (-1 == realEnemyX || -1 == realEnemyY)
			{
				result = true;
			}
			else
			{
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
				if (spriteType != GSpriteTypes.Other)
				{
					result = true;
				}
				else
				{
					bool flag = false;
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null == gameClient)
					{
						flag = true;
					}
					else if (gameClient.CurrentMapCode != client.CurrentMapCode)
					{
						flag = true;
					}
					else if (gameClient.CurrentCopyMapID > 0 && gameClient.CurrentCopyMapID != client.CurrentCopyMapID)
					{
						flag = true;
					}
					else if (Math.Abs(gameClient.ClientData.PosX - realEnemyX) > 1600 || Math.Abs(gameClient.ClientData.PosY - realEnemyY) > 1000)
					{
						flag = true;
					}
					if (flag)
					{
						enemyID = -1;
						client.sendCmd(127, string.Format("{0}:{1}", enemyID, 1), false);
					}
					result = true;
				}
			}
			return result;
		}

		private static bool CheckMonsterPostion(GameClient client, int enemyID, int realEnemyX, int realEnemyY)
		{
			bool result;
			if (-1 == enemyID)
			{
				result = true;
			}
			else if (-1 == realEnemyX || -1 == realEnemyY)
			{
				result = true;
			}
			else
			{
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
				if (spriteType != GSpriteTypes.Monster)
				{
					result = true;
				}
				else
				{
					Point p = new Point((double)realEnemyX, (double)realEnemyY);
					Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemyID);
					if (monster == null || !monster.Alive || (monster.CopyMapID > 0 && monster.CopyMapID != client.ClientData.CopyMapID))
					{
						GameManager.ClientMgr.NotifyMyselfLeaveMonsterByID(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyID);
						result = true;
					}
					else
					{
						Point point = new Point(monster.SafeCoordinate.X, monster.SafeCoordinate.Y);
						if (monster.VLife > 0.0 && monster.Alive)
						{
							if (Global.CompareTwoPointGridXY(monster.MonsterZoneNode.MapCode, p, point))
							{
								return true;
							}
						}
						if (!Global.JugePointAtClientGrids(client, monster, point))
						{
							List<object> list = new List<object>();
							list.Add(monster);
							GameManager.ClientMgr.NotifyMyselfLeaveMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, list);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public static bool CheckLastAttackTicks(GameClient client, bool recAttackTicks, int magicCode)
		{
			return recAttackTicks || true;
		}

		private static bool CanAutoUseZSSkill(GameClient client, int magicCode)
		{
			int num = Global.CalcOriginalOccupationID(client);
			bool result;
			if (0 != num)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
				{
					result = false;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("SkillType", -1);
					if (1 != intValue && 10 != intValue)
					{
						result = false;
					}
					else
					{
						int intValue2 = systemXmlItem.GetIntValue("MagicType", -1);
						result = (1 == intValue2 || 2 == intValue2);
					}
				}
			}
			return result;
		}

		private static bool CanRecordAttackTicks(GameClient client, int magicCode)
		{
			SystemXmlItem systemXmlItem = null;
			return !GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem) || 1 != systemXmlItem.GetIntValue("SkillType", -1);
		}

		public static bool AddManyAttackMagic(IObject obj, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			MagicsManyTimeDmageQueue extComponent = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			return null != extComponent && extComponent.AddManyTimeDmageQueueItemEx(enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode);
		}

		public static bool AddDelayMagic(IObject obj, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			MagicsManyTimeDmageQueue extComponent = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			return null != extComponent && extComponent.AddDelayMagicItemEx(enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode);
		}

		private static void ParseManyTimes(IObject obj, List<ManyTimeDmageItem> manyTimeDmageItemList, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			long num = TimeUtil.NOW();
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				for (int i = 0; i < manyTimeDmageItemList.Count; i++)
				{
					ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
					{
						ToExecTicks = num + manyTimeDmageItemList[i].InjuredSeconds,
						enemy = enemy,
						enemyX = enemyX,
						enemyY = enemyY,
						realEnemyX = realEnemyX,
						realEnemyY = realEnemyY,
						magicCode = magicCode,
						manyRangeIndex = i,
						manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
					};
					gameClient.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (monster.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() <= 0)
				{
					for (int i = 0; i < manyTimeDmageItemList.Count; i++)
					{
						ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
						{
							ToExecTicks = num + manyTimeDmageItemList[i].InjuredSeconds,
							enemy = enemy,
							enemyX = enemyX,
							enemyY = enemyY,
							realEnemyX = realEnemyX,
							realEnemyY = realEnemyY,
							magicCode = magicCode,
							manyRangeIndex = i,
							manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
						};
						monster.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
					}
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				for (int i = 0; i < manyTimeDmageItemList.Count; i++)
				{
					ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
					{
						ToExecTicks = num + manyTimeDmageItemList[i].InjuredSeconds,
						enemy = enemy,
						enemyX = enemyX,
						enemyY = enemyY,
						realEnemyX = realEnemyX,
						realEnemyY = realEnemyY,
						magicCode = magicCode,
						manyRangeIndex = i,
						manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
					};
					robot.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
				}
			}
		}

		public static void ExecMagicsManyTimeDmageQueue(IObject obj)
		{
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (gameClient.ClientData.CurrentLifeV > 0)
				{
					List<ManyTimeDmageQueueItem> canExecItems = gameClient.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < canExecItems.Count; i++)
					{
						SpriteAttack.ProcessAttack(gameClient, canExecItems[i].enemy, canExecItems[i].enemyX, canExecItems[i].enemyY, canExecItems[i].realEnemyX, canExecItems[i].realEnemyY, canExecItems[i].magicCode, canExecItems[i].manyRangeIndex, canExecItems[i].manyRangeInjuredPercent);
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (monster.VLife > 0.0)
				{
					List<ManyTimeDmageQueueItem> canExecItems = monster.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < canExecItems.Count; i++)
					{
						SpriteAttack.ProcessAttackByMonster(monster, canExecItems[i].enemy, canExecItems[i].enemyX, canExecItems[i].enemyY, canExecItems[i].realEnemyX, canExecItems[i].realEnemyY, canExecItems[i].magicCode, canExecItems[i].manyRangeIndex, canExecItems[i].manyRangeInjuredPercent);
					}
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (robot.VLife > 0.0)
				{
					List<ManyTimeDmageQueueItem> canExecItems = robot.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < canExecItems.Count; i++)
					{
						SpriteAttack.ProcessMagicAttackByJingJiRobot(robot, canExecItems[i].enemy, canExecItems[i].magicCode, canExecItems[i].manyRangeIndex, canExecItems[i].manyRangeInjuredPercent);
					}
				}
			}
		}

		public static void ExecMagicsManyTimeDmageQueueEx(IObject obj)
		{
			MagicsManyTimeDmageQueue extComponent = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			if (null != extComponent)
			{
				if (obj is GameClient)
				{
					GameClient gameClient = obj as GameClient;
					if (gameClient.ClientData.CurrentLifeV > 0)
					{
						ManyTimeDmageItem manyTimeDmageItem;
						ManyTimeDmageMagicItem canExecItemsEx;
						while (null != (canExecItemsEx = extComponent.GetCanExecItemsEx(out manyTimeDmageItem)))
						{
							try
							{
								SpriteAttack.ProcessAttack(gameClient, canExecItemsEx.enemy, canExecItemsEx.enemyX, canExecItemsEx.enemyY, canExecItemsEx.realEnemyX, canExecItemsEx.realEnemyY, canExecItemsEx.magicCode, manyTimeDmageItem.manyRangeIndex, manyTimeDmageItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
				else if (obj is Monster)
				{
					Monster monster = obj as Monster;
					if (monster.VLife > 0.0)
					{
						ManyTimeDmageItem manyTimeDmageItem;
						ManyTimeDmageMagicItem canExecItemsEx;
						while (null != (canExecItemsEx = extComponent.GetCanExecItemsEx(out manyTimeDmageItem)))
						{
							try
							{
								SpriteAttack.ProcessAttackByMonster(monster, canExecItemsEx.enemy, canExecItemsEx.enemyX, canExecItemsEx.enemyY, canExecItemsEx.realEnemyX, canExecItemsEx.realEnemyY, canExecItemsEx.magicCode, manyTimeDmageItem.manyRangeIndex, manyTimeDmageItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
				else if (obj is Robot)
				{
					Robot robot = obj as Robot;
					if (robot.VLife > 0.0)
					{
						ManyTimeDmageItem manyTimeDmageItem;
						ManyTimeDmageMagicItem canExecItemsEx;
						while (null != (canExecItemsEx = extComponent.GetCanExecItemsEx(out manyTimeDmageItem)))
						{
							try
							{
								SpriteAttack.ProcessMagicAttackByJingJiRobot(robot, canExecItemsEx.enemy, canExecItemsEx.magicCode, manyTimeDmageItem.manyRangeIndex, manyTimeDmageItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
			}
		}

		public static void ProcessAttack(GameClient client, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == manyRangeIndex)
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(magicCode, out systemXmlItem))
				{
					return;
				}
				if (!client.ClientData.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(2, string.Format("拒绝技能释放#CD未结束2,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
					return;
				}
				client.ClientData.MyMagicCoolDownMgr.AddSkillCoolDown(client, magicCode);
				if (SpriteAttack.AddManyAttackMagic(client, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(2, string.Format("记录技能释放#本次技能,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
					client.ClientData.LastSkillID = magicCode;
					return;
				}
			}
			client.passiveSkillModule.OnProcessMagic(client, enemy, enemyX, enemyY);
			if (manyRangeIndex <= 0)
			{
				client.UsingEquipMgr.AttackSomebody(client);
			}
			bool recAttackTicks = SpriteAttack.CanRecordAttackTicks(client, magicCode);
			if (manyRangeIndex > 0)
			{
				recAttackTicks = false;
			}
			SpriteAttack._ProcessAttack(client, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode, recAttackTicks, manyRangeIndex, manyRangeInjuredPercent);
		}

		public static bool IsParentActionDone(GameClient client, int magicCode)
		{
			int num = 0;
			if (GameManager.SystemMagicActionMgr.MagicActionRelationDic.TryGetValue(magicCode, out num))
			{
				if (num != client.ClientData.LastSkillID)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsMagicEnough(GameClient client, int magicCode)
		{
			SkillData skillDataByID = Global.GetSkillDataByID(client, magicCode);
			bool result;
			if (skillDataByID == null)
			{
				result = false;
			}
			else
			{
				int num = Global.GetNeedMagicV(client, magicCode, skillDataByID.SkillLevel);
				if (num > 0 && client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
				{
					int num2 = (int)RoleAlgorithm.GetMaxMagicV(client);
					num = (int)((double)num2 * ((double)num / 100.0));
					if (client.ClientData.CurrentMagicV - num < 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public static int CheckMagicScripts(GameClient client, int magicCode)
		{
			int result;
			if (-1 == magicCode)
			{
				result = magicCode;
			}
			else
			{
				List<MagicActionItem> list = null;
				if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out list) || null == list)
				{
					if (!GameManager.SystemMagicActionMgr2.MagicActionsDict.TryGetValue(magicCode, out list) || null == list)
					{
						return -1;
					}
				}
				result = magicCode;
			}
			return result;
		}

		public static int CheckMagicScripts2(GameClient client, int magicCode)
		{
			int result;
			if (-1 == magicCode)
			{
				result = magicCode;
			}
			else
			{
				List<MagicActionItem> list = null;
				if (!GameManager.SystemMagicActionMgr2.MagicActionsDict.TryGetValue(magicCode, out list) || null == list)
				{
					result = -1;
				}
				else
				{
					result = magicCode;
				}
			}
			return result;
		}

		private static void _ProcessAttack(GameClient client, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, bool recAttackTicks, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (!SpriteAttack.CheckEnemyClientPostion(client, ref enemy, realEnemyX, realEnemyY))
			{
				if (client.ClientSocket.session.gmPriority > 0)
				{
					LogManager.WriteLog(2, string.Format("拒绝技能释放#目标未找到,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
				}
			}
			else if (!SpriteAttack.CheckMonsterPostion(client, enemy, realEnemyX, realEnemyY))
			{
				if (client.ClientSocket.session.gmPriority > 0)
				{
					LogManager.WriteLog(2, string.Format("拒绝技能释放#目标距离太远,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
				}
			}
			else
			{
				magicCode = SpriteAttack.CheckMagicScripts(client, magicCode);
				if (!SpriteAttack.CheckLastAttackTicks(client, recAttackTicks, magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(2, string.Format("拒绝技能释放#攻击速度超限,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
				}
				else
				{
					client.CheckCheatData.LastMagicCode = magicCode;
					if (-1 == magicCode)
					{
						SpriteAttack.ProcessPhyAttack(client, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
					}
					else
					{
						SpriteAttack.ProcessMagicAttack(client, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
					}
				}
			}
		}

		private static bool IsFriend(GameClient me, int mapCode, int enemyID)
		{
			bool flag = false;
			bool result;
			if (-1 == enemyID)
			{
				result = flag;
			}
			else if (me.ClientData.RoleID == enemyID)
			{
				result = true;
			}
			else
			{
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
				if (spriteType == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (null != monster.OwnerClient)
						{
							if (monster.OwnerClient.ClientData.RoleID == me.ClientData.RoleID)
							{
								return true;
							}
						}
					}
				}
				else if (spriteType != GSpriteTypes.NPC)
				{
					if (spriteType != GSpriteTypes.Pet)
					{
						if (spriteType != GSpriteTypes.BiaoChe)
						{
							if (spriteType != GSpriteTypes.JunQi)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
								if (null != gameClient)
								{
									flag = SpriteAttack.IsFriend(me, gameClient);
								}
							}
						}
					}
				}
				result = flag;
			}
			return result;
		}

		private static bool IsFriend(GameClient me, GameClient enemy)
		{
			bool flag = false;
			bool result;
			if (me.ClientData.PKMode == 0 || 1 == me.ClientData.PKMode)
			{
				result = true;
			}
			else if (Global.IsBattleMap(me))
			{
				result = (me.ClientData.BattleWhichSide == enemy.ClientData.BattleWhichSide);
			}
			else
			{
				if (2 == me.ClientData.PKMode)
				{
					if (me.ClientData.ServerPTID != enemy.ClientData.ServerPTID)
					{
						return false;
					}
					if (me.ClientData.Faction > 0 && enemy.ClientData.Faction > 0 && (me.ClientData.Faction == enemy.ClientData.Faction || AllyManager.getInstance().UnionIsAlly(me, enemy.ClientData.Faction)))
					{
						return true;
					}
				}
				else if (3 == me.ClientData.PKMode)
				{
					if (me.ClientData.TeamID > 0 && me.ClientData.TeamID == enemy.ClientData.TeamID)
					{
						flag = true;
					}
				}
				result = flag;
			}
			return result;
		}

		private static bool GetEnemyPos(int mapCode, int enemyID, out Point pos)
		{
			bool result = false;
			pos = new Point(0.0, 0.0);
			GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
			if (spriteType == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
				if (null != monster)
				{
					result = true;
					pos = new Point(monster.SafeCoordinate.X, monster.SafeCoordinate.Y);
				}
			}
			else if (spriteType != GSpriteTypes.Pet)
			{
				if (spriteType == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						result = true;
						pos = new Point((double)biaoCheItem.PosX, (double)biaoCheItem.PosY);
					}
				}
				else if (spriteType == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						result = true;
						pos = new Point((double)junQiItem.PosX, (double)junQiItem.PosY);
					}
				}
				else if (spriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						result = true;
						pos = new Point((double)fakeRoleItem.MyRoleDataMini.PosX, (double)fakeRoleItem.MyRoleDataMini.PosY);
					}
				}
				else
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null != gameClient)
					{
						result = true;
						pos = new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY);
					}
				}
			}
			return result;
		}

		private static object GetEnemyObject(int mapCode, int enemyID)
		{
			object result = null;
			GSpriteTypes spriteType = Global.GetSpriteType((uint)enemyID);
			if (spriteType == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
				if (null != monster)
				{
					result = monster;
				}
			}
			else if (spriteType != GSpriteTypes.Pet)
			{
				if (spriteType == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						result = biaoCheItem;
					}
				}
				else if (spriteType == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						result = junQiItem;
					}
				}
				else if (spriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						result = fakeRoleItem;
					}
				}
				else
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null != gameClient)
					{
						result = gameClient;
					}
				}
			}
			return result;
		}

		private static void ProcessPhyAttack(GameClient client, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			enemy = SpriteAttack.VerifyEnemyID(client, client.ClientData.MapCode, enemy, enemyX, enemyY);
			int direction = client.ClientData.RoleDirection;
			if (-1 != enemyX && -1 != enemyY)
			{
				direction = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, (double)client.ClientData.PosX, (double)client.ClientData.PosY);
			}
			List<int> list = new List<int>();
			GameManager.ClientMgr.LookupEnemiesInCircleByAngle(client, direction, client.ClientData.MapCode, enemyX, enemyY, 200, list, 135.0, true);
			GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, client.ClientData.MapCode, client.ClientData.CopyMapID, enemyX, enemyY, 200, list, 125.0, true);
			BiaoCheManager.LookupAttackEnemyIDs(client, direction, list);
			JunQiManager.LookupAttackEnemyIDs(client, direction, list);
			FakeRoleManager.LookupAttackEnemyIDs(client, direction, list);
			if (list.Count > 0)
			{
				if (list.IndexOf(enemy) < 0)
				{
					int randomNumber = Global.GetRandomNumber(0, list.Count);
					enemy = list[randomNumber];
				}
			}
			else
			{
				enemy = -1;
			}
			if (-1 != enemy)
			{
				if (!SpriteAttack.IsOpposition(client, client.ClientData.MapCode, enemy))
				{
					enemy = -1;
				}
			}
			int burst = 0;
			int injure = 0;
			if (enemy != -1)
			{
				List<int> list2 = client.ClientData.ExtensionProps.GetIDs();
				if (null != list2)
				{
					list2 = ExtensionPropsMgr.ProcessExtensionProps(list2, magicCode, 0);
				}
				List<int> list3 = client.ClientData.ExtensionProps.GetIDs();
				if (null != list3)
				{
					list3 = ExtensionPropsMgr.ProcessExtensionProps(list3, magicCode, 1);
				}
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
				if (spriteType == GSpriteTypes.Monster)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemy);
					if (null != monster)
					{
						GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, client, monster);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, monster);
					}
				}
				else if (spriteType == GSpriteTypes.BiaoChe)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					BiaoCheManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else if (spriteType == GSpriteTypes.JunQi)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					JunQiManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else if (spriteType == GSpriteTypes.FakeRole)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					JunQiManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					GameClient gameClient = GameManager.ClientMgr.FindClient(enemy);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gameClient, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, client, gameClient);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, gameClient);
					}
				}
			}
		}

		private static void ProcessMagicAttack(GameClient client, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != magicCode)
			{
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
				{
					SkillData skillData = Global.GetSkillDataByID(client, magicCode);
					if (client.ClientData.IsFlashPlayer >= 1)
					{
						skillData = new SkillData
						{
							DbID = -1,
							SkillID = magicCode,
							SkillLevel = 1,
							UsedNum = 1
						};
					}
					if (null != skillData)
					{
						List<MagicActionItem> list = null;
						if (GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out list) && null != list)
						{
							List<MagicActionItem> list2 = null;
							if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out list2) || null == list2)
							{
							}
							MagicActionItem magicActionItem = null;
							if (list2 != null && list2.Count > 0)
							{
								magicActionItem = list2[0];
							}
							int intValue = systemXmlItem.GetIntValue("AttackDistance", -1);
							int num = systemXmlItem.GetIntValue("MaxNum", -1);
							MagicAction.MaxHitNum = num;
							if (!SpriteAttack.JugeMagicDistance(systemXmlItem, client, enemy, enemyX, enemyY, magicCode, false))
							{
								if (null == magicActionItem)
								{
									return;
								}
								if (magicActionItem != null && magicActionItem.MagicActionID != MagicActionIDs.SCAN_SQUARE && magicActionItem.MagicActionID != MagicActionIDs.FRONT_SECTOR && magicActionItem.MagicActionID != MagicActionIDs.ROUNDSCAN)
								{
									return;
								}
							}
							int num2 = 0;
							List<int> list3 = new List<int>();
							List<int> list4 = new List<int>();
							if (manyRangeIndex <= 0)
							{
								num2 = Global.GetNeedMagicV(client, magicCode, skillData.SkillLevel);
								if (num2 > 0 && client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									int num3 = (int)RoleAlgorithm.GetMaxMagicV(client);
									num2 = (int)((double)num3 * ((double)num2 / 100.0));
									if (client.ClientData.CurrentMagicV - num2 < 0)
									{
										return;
									}
								}
								int num4 = 1;
								double num5 = DBRoleBufferManager.ProcessDblSkillUp(client);
								num4 = (int)((double)num4 * num5);
								if (num4 > 0)
								{
									GameManager.ClientMgr.AddNumSkill(client, skillData, num4, false);
								}
								GameManager.ClientMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (double)num2);
								list3 = client.ClientData.ExtensionProps.GetIDs();
								if (null != list3)
								{
									list3 = ExtensionPropsMgr.ProcessExtensionProps(list3, skillData.SkillID, 0);
								}
								list4 = client.ClientData.ExtensionProps.GetIDs();
								if (null != list4)
								{
									list4 = ExtensionPropsMgr.ProcessExtensionProps(list4, skillData.SkillID, 1);
								}
							}
							int intValue2 = systemXmlItem.GetIntValue("TargetPlayingType", -1);
							int direction = 0;
							if (systemXmlItem.GetIntValue("MagicType", -1) == 1 || systemXmlItem.GetIntValue("MagicType", -1) == 3)
							{
								int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
								if (1 == intValue3)
								{
									if (systemXmlItem.GetIntValue("MagicType", -1) != 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
									}
									bool flag = false;
									int i = 0;
									while (i < list.Count)
									{
										if (list[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
										{
											goto IL_3FF;
										}
										if (Global.GetTwoPointDistance(new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY), new Point((double)enemyX, (double)enemyY)) <= (double)intValue)
										{
											goto IL_3FF;
										}
										IL_457:
										i++;
										continue;
										IL_3FF:
										flag |= MagicAction.ProcessAction(client, client, list[i].MagicActionID, list[i].MagicActionParams, enemyX, enemyY, num2, skillData.SkillLevel, skillData.SkillID, skillData.SkillID, client.ClientData.RoleDirection, -1, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										goto IL_457;
									}
									if (flag)
									{
										if (systemXmlItem.GetIntValue("MagicType", -1) == 3)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
										}
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, client);
								}
								else if (-1 == intValue3 || 2 == intValue3 || 3 == intValue3 || 4 == intValue3)
								{
									direction = client.ClientData.RoleDirection;
									if (-1 != enemyX && -1 != enemyY)
									{
										direction = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, (double)client.ClientData.PosX, (double)client.ClientData.PosY);
									}
									if (2 == intValue3)
									{
										if (-1 == enemy)
										{
											enemy = client.ClientData.RoleID;
										}
										else if (client.ClientData.RoleID != enemy)
										{
											if (!SpriteAttack.IsFriend(client, client.ClientData.MapCode, enemy))
											{
												enemy = client.ClientData.RoleID;
											}
										}
									}
									else if (4 == intValue3)
									{
										if (-1 == enemy)
										{
											enemy = client.ClientData.RoleID;
										}
										else if (client.ClientData.RoleID != enemy)
										{
										}
									}
									else if (-1 == intValue3 || 3 == intValue3)
									{
										if (-1 == enemy || enemy == client.ClientData.RoleID)
										{
											if (-1 == enemy)
											{
												Point point = new Point((double)enemyX, (double)enemyY);
												if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
												{
													point = new Point((double)enemyX, (double)enemyY);
												}
												else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
												{
													if (-1 != enemy)
													{
														if (!SpriteAttack.GetEnemyPos(client.ClientData.MapCode, enemy, out point))
														{
															point = new Point((double)enemyX, (double)enemyY);
														}
													}
													direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
												}
												else
												{
													direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
												}
												List<object> list5 = new List<object>();
												GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)point.X, (int)point.Y, 50, list5, -1);
												GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)point.X, (int)point.Y, 50, list5);
												if (list5.Count > 0)
												{
													int randomNumber = Global.GetRandomNumber(0, list5.Count);
													enemy = (list5[randomNumber] as IObject).GetObjectID();
												}
											}
										}
										if (enemy > 0)
										{
											if (!SpriteAttack.IsOpposition(client, client.ClientData.MapCode, enemy))
											{
												enemy = -1;
											}
										}
									}
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
									}
									if (-1 != enemy)
									{
										GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
										if (spriteType == GSpriteTypes.Monster)
										{
											Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemy);
											if (null != monster)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, monster.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, monster, list[i].MagicActionID, list[i].MagicActionParams, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, client, monster);
											}
										}
										else if (spriteType == GSpriteTypes.BiaoChe)
										{
											BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemy);
											if (null != biaoCheItem)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, biaoCheItem.BiaoCheID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, biaoCheItem, list[i].MagicActionID, list[i].MagicActionParams, biaoCheItem.PosX, biaoCheItem.PosY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else if (spriteType == GSpriteTypes.JunQi)
										{
											JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemy);
											if (null != junQiItem)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, junQiItem.JunQiID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, junQiItem, list[i].MagicActionID, list[i].MagicActionParams, junQiItem.PosX, junQiItem.PosY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else if (spriteType == GSpriteTypes.FakeRole)
										{
											FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemy);
											if (null != fakeRoleItem)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, fakeRoleItem.FakeRoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, fakeRoleItem, list[i].MagicActionID, list[i].MagicActionParams, fakeRoleItem.MyRoleDataMini.PosX, fakeRoleItem.MyRoleDataMini.PosY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else
										{
											GameClient gameClient = GameManager.ClientMgr.FindClient(enemy);
											if (null != gameClient)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gameClient.ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, gameClient, list[i].MagicActionID, list[i].MagicActionParams, gameClient.ClientData.PosX, gameClient.ClientData.PosY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, gameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, client, gameClient);
											}
										}
									}
								}
								else
								{
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < list.Count; i++)
									{
										MagicAction.ProcessAction(client, null, list[i].MagicActionID, list[i].MagicActionParams, enemyX, enemyY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, client.ClientData.RoleDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, client);
								}
							}
							else
							{
								int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
								direction = client.ClientData.RoleDirection;
								Point point;
								if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
								{
									point = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
								{
									point = new Point((double)enemyX, (double)enemyY);
									if (-1 != enemy)
									{
										if (!SpriteAttack.GetEnemyPos(client.ClientData.MapCode, enemy, out point))
										{
											point = new Point((double)enemyX, (double)enemyY);
										}
									}
									direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								else
								{
									if (magicActionItem != null && (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR || magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN))
									{
										point = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
									}
									else
									{
										point = new Point((double)enemyX, (double)enemyY);
									}
									direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								List<object> list6 = new List<object>();
								if (magicActionItem != null)
								{
									if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
									{
										GameManager.ClientMgr.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list6);
									}
									else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
									{
										GameManager.ClientMgr.LookupEnemiesInCircleByAngle(client, client.ClientData.RoleDirection, client.ClientData.MapCode, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6, magicActionItem.MagicActionParams[0], true);
									}
									else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
									{
										GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6, intValue3);
									}
								}
								else
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6, -1);
								}
								if (1 != intValue3)
								{
									if (2 == intValue3 || 4 == intValue3)
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < list.Count; i++)
										{
											MagicAction.ProcessAction(client, client, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										if (intValue2 == 1)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, client);
										int j = 0;
										while (j < list6.Count)
										{
											if (2 != intValue3)
											{
												goto IL_1224;
											}
											if (client.ClientData.TeamID > 0 && client.ClientData.TeamID == (list6[j] as GameClient).ClientData.TeamID)
											{
												goto IL_1224;
											}
											IL_132F:
											j++;
											continue;
											IL_1224:
											for (int i = 0; i < list.Count; i++)
											{
												MagicAction.ProcessAction(client, list6[j] as GameClient, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (intValue2 == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list6[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, list6[j] as GameClient);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, client, list6[j] as GameClient);
											if (--num <= 0)
											{
												break;
											}
											goto IL_132F;
										}
									}
									else if (-1 == intValue3 || 3 == intValue3)
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < list.Count; i++)
										{
											MagicAction.ProcessAction(client, null, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										List<object> list7 = new List<object>();
										if (list2 != null)
										{
											if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												GameManager.MonsterMgr.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list7);
											}
											else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircleByRoleAngle(client.ClientData.RoleYAngle, client.ClientData.MapCode, client.ClientData.CopyMapID, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, magicActionItem.MagicActionParams[0], true);
											}
											else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7);
											}
										}
										else
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7);
										}
										List<object> list8 = new List<object>();
										foreach (object obj in list7)
										{
											if (Global.IsOpposition(client, obj as Monster))
											{
												list8.Add(obj);
											}
										}
										foreach (object obj in list6)
										{
											if ((obj as GameClient).ClientData.RoleID != client.ClientData.RoleID)
											{
												if (Global.IsOpposition(client, obj as GameClient))
												{
													list8.Add(obj);
												}
											}
										}
										double magicCodeAddPercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(client, list8, magicCode);
										for (int j = 0; j < list6.Count; j++)
										{
											if ((list6[j] as GameClient).ClientData.RoleID != client.ClientData.RoleID)
											{
												if (Global.IsOpposition(client, list6[j] as GameClient))
												{
													if (intValue2 == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list6[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
													}
													for (int i = 0; i < list.Count; i++)
													{
														MagicAction.ProcessAction(client, list6[j] as GameClient, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, magicCodeAddPercent);
													}
													ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, list6[j] as GameClient);
													ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, client, list6[j] as GameClient);
													if (--num <= 0)
													{
														break;
													}
												}
											}
										}
										for (int j = 0; j < list7.Count; j++)
										{
											if (Global.IsOpposition(client, list7[j] as Monster))
											{
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list7[j] as Monster).RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, list7[j] as Monster, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, magicCodeAddPercent);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, list7[j] as Monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, client, list7[j] as Monster);
												if (--num <= 0)
												{
													break;
												}
											}
										}
										List<object> list9 = new List<object>();
										BiaoCheManager.LookupRangeAttackEnemies(client, (int)point.X, (int)point.Y, direction, systemXmlItem.GetStringValue("AttackDistance"), list9);
										for (int j = 0; j < list9.Count; j++)
										{
											if (intValue2 == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list9[j] as BiaoCheItem).BiaoCheID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < list.Count; i++)
											{
												MagicAction.ProcessAction(client, list9[j] as BiaoCheItem, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (--num <= 0)
											{
												break;
											}
										}
										List<object> list10 = new List<object>();
										JunQiManager.LookupRangeAttackEnemies(client, (int)point.X, (int)point.Y, direction, systemXmlItem.GetStringValue("AttackDistance"), list10);
										for (int j = 0; j < list10.Count; j++)
										{
											if (Global.IsOpposition(client, list10[j] as JunQiItem))
											{
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list10[j] as JunQiItem).JunQiID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, list10[j] as JunQiItem, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (--num <= 0)
												{
													break;
												}
											}
										}
										List<object> list11 = new List<object>();
										if (magicActionItem != null)
										{
											if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												FakeRoleManager.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list11);
											}
											else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												FakeRoleManager.LookupEnemiesInCircleByAngle(client, client.ClientData.RoleDirection, client.ClientData.MapCode, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list11, magicActionItem.MagicActionParams[0], true);
											}
											else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
											{
												FakeRoleManager.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list11);
											}
										}
										else
										{
											FakeRoleManager.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list11);
										}
										for (int j = 0; j < list11.Count; j++)
										{
											if (Global.IsOpposition(client, list11[j] as FakeRoleItem))
											{
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (list11[j] as FakeRoleItem).FakeRoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(client, list11[j] as FakeRoleItem, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (--num <= 0)
												{
													break;
												}
											}
										}
									}
									else
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < list.Count; i++)
										{
											MagicAction.ProcessAction(client, null, list[i].MagicActionID, list[i].MagicActionParams, enemyX, enemyY, num2, skillData.SkillLevel, skillData.SkillID, 0, 0, client.ClientData.RoleDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, client, client);
									}
								}
							}
						}
					}
				}
			}
		}

		public static void ProcessAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == manyRangeIndex && attacker.MagicFinish <= -1)
			{
				if (SpriteAttack.AddManyAttackMagic(attacker, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode))
				{
					attacker.MagicFinish = -2;
					return;
				}
			}
			bool recAttackTicks = false;
			SpriteAttack._ProcessAttackByMonster(attacker, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode, recAttackTicks, manyRangeIndex, manyRangeInjuredPercent);
		}

		public static void ProcessAttackByJingJiRobot(Robot attacker, IObject target, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == magicCode)
			{
				SpriteAttack.ProcessPhyAttackByMonster(attacker, target.GetObjectID(), (int)target.CurrentPos.X, (int)target.CurrentPos.Y, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
			else
			{
				SpriteAttack.ProcessMagicAttackByJingJiRobot(attacker, target, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
		}

		private static void ProcessMagicAttackByJingJiRobot(Robot attacker, int enemy, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != enemy)
			{
				IObject @object = null;
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
				if (spriteType == GSpriteTypes.Monster)
				{
					@object = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
				}
				else if (spriteType == GSpriteTypes.Other)
				{
					@object = GameManager.ClientMgr.FindClient(enemy);
				}
				if (null != @object)
				{
					SpriteAttack.ProcessMagicAttackByJingJiRobot(attacker, @object, magicCode, manyRangeIndex, manyRangeInjuredPercent);
				}
			}
		}

		private static void ProcessMagicAttackByJingJiRobot(Robot attacker, IObject target, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != magicCode)
			{
				if (-1 == manyRangeIndex)
				{
					if (SpriteAttack.AddManyAttackMagic(attacker, target.GetObjectID(), (int)target.CurrentPos.X, (int)target.CurrentPos.Y, (int)target.CurrentPos.X, (int)target.CurrentPos.Y, magicCode))
					{
						attacker.MyMagicCoolDownMgr.AddSkillCoolDown(attacker, magicCode);
						return;
					}
				}
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
				{
					int num = target.GetObjectID();
					int num2 = (int)target.CurrentPos.X;
					int num3 = (int)target.CurrentPos.Y;
					int intValue = systemXmlItem.GetIntValue("AttackDistance", -1);
					int num4 = systemXmlItem.GetIntValue("MaxNum", -1);
					if (SpriteAttack.JugeMagicDistance(systemXmlItem, attacker, num, num2, num3, magicCode, false))
					{
						List<MagicActionItem> list = null;
						if (GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out list) && null != list)
						{
							int num5 = 0;
							List<int> list2 = new List<int>();
							List<int> list3 = new List<int>();
							if (manyRangeIndex <= 0)
							{
								num5 = Global.GetNeedMagicV(attacker, magicCode, 1);
								if (num5 > 0)
								{
									int num6 = (int)attacker.MonsterInfo.VManaMax;
									int num7 = num6 * (num5 / 100);
									if (attacker.VMana - (double)num7 <= 0.0)
									{
										return;
									}
								}
								if (!attacker.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
								{
									return;
								}
								attacker.MyMagicCoolDownMgr.AddSkillCoolDown(attacker, magicCode);
								GameManager.MonsterMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (double)num5);
								list2 = attacker.ExtensionProps.GetIDs();
								if (null != list2)
								{
									list2 = ExtensionPropsMgr.ProcessExtensionProps(list2, magicCode, 0);
								}
								list3 = attacker.ExtensionProps.GetIDs();
								if (null != list3)
								{
									list3 = ExtensionPropsMgr.ProcessExtensionProps(list3, magicCode, 1);
								}
							}
							int intValue2 = systemXmlItem.GetIntValue("TargetPlayingType", -1);
							if (systemXmlItem.GetIntValue("MagicType", -1) == 1 || systemXmlItem.GetIntValue("MagicType", -1) == 3)
							{
								int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
								if (1 == intValue3)
								{
									if (systemXmlItem.GetIntValue("MagicType", -1) != 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), num2, num3, magicCode);
									}
									bool flag = false;
									int i = 0;
									while (i < list.Count)
									{
										if (list[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
										{
											goto IL_337;
										}
										if (Global.GetTwoPointDistance(attacker.CurrentPos, new Point((double)num2, (double)num3)) <= (double)intValue)
										{
											goto IL_337;
										}
										IL_389:
										i++;
										continue;
										IL_337:
										flag |= MagicAction.ProcessAction(attacker, attacker, list[i].MagicActionID, list[i].MagicActionParams, num2, num3, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										goto IL_389;
									}
									if (flag)
									{
										if (systemXmlItem.GetIntValue("MagicType", -1) == 3)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), num2, num3, magicCode);
										}
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, attacker);
								}
								else if (-1 == intValue3 || 2 == intValue3 || 3 == intValue3)
								{
									int direction = (int)attacker.CurrentDir;
									if (-1 != num2 && -1 != num3)
									{
										direction = (int)Global.GetDirectionByTan((double)num2, (double)num3, attacker.CurrentPos.X, attacker.CurrentPos.Y);
									}
									if (2 == intValue3)
									{
										if (-1 == num)
										{
											num = attacker.GetObjectID();
										}
										else if (attacker.GetObjectID() != num)
										{
											num = -1;
										}
									}
									else if (-1 == intValue3 || 3 == intValue3)
									{
										if (-1 == num || num == attacker.GetObjectID())
										{
											if (-1 == num)
											{
												Point point = new Point((double)num2, (double)num3);
												if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
												{
													point = new Point((double)num2, (double)num3);
												}
												else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
												{
													if (-1 != num)
													{
														if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, num, out point))
														{
															point = new Point((double)num2, (double)num3);
														}
													}
													direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
												}
												else
												{
													direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
												}
												List<object> list4 = new List<object>();
												List<MagicActionItem> list5 = null;
												if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out list5) || null == list5)
												{
												}
												MagicActionItem magicActionItem = null;
												if (list5 != null && list5.Count > 0)
												{
													magicActionItem = list5[0];
												}
												if (magicActionItem != null)
												{
													if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
													{
														GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list4);
													}
													else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
													{
														GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list4, magicActionItem.MagicActionParams[0], true);
													}
												}
												else
												{
													GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list4);
												}
												if (list4.Count > 0)
												{
													int randomNumber = Global.GetRandomNumber(0, list4.Count);
													num = (list4[randomNumber] as IObject).GetObjectID();
												}
											}
										}
										else if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, num))
										{
											num = -1;
										}
									}
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, num2, num3, magicCode);
									}
									if (-1 != num)
									{
										GSpriteTypes spriteType = Global.GetSpriteType((uint)num);
										if (spriteType == GSpriteTypes.Monster)
										{
											Monster monster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, num);
											if (null != monster)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, monster.RoleID, num2, num3, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(attacker, monster, list[i].MagicActionID, list[i].MagicActionParams, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, monster);
											}
										}
										else if (spriteType == GSpriteTypes.BiaoChe)
										{
											BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(num);
											if (null != biaoCheItem)
											{
											}
										}
										else if (spriteType == GSpriteTypes.JunQi)
										{
											JunQiItem junQiItem = JunQiManager.FindJunQiByID(num);
											if (null != junQiItem)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, junQiItem.GetObjectID(), num2, num3, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(attacker, junQiItem, list[i].MagicActionID, list[i].MagicActionParams, (int)junQiItem.CurrentPos.X, (int)junQiItem.CurrentPos.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, junQiItem);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, junQiItem);
											}
										}
										else if (spriteType == GSpriteTypes.FakeRole)
										{
											FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(num);
											if (null != fakeRoleItem)
											{
											}
										}
										else
										{
											GameClient gameClient = GameManager.ClientMgr.FindClient(num);
											if (null != gameClient)
											{
												if (1 == intValue2)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, gameClient.ClientData.RoleID, num2, num3, magicCode);
												}
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(attacker, gameClient, list[i].MagicActionID, list[i].MagicActionParams, gameClient.ClientData.PosX, gameClient.ClientData.PosY, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, gameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, gameClient);
											}
										}
									}
								}
								else
								{
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, num2, num3, magicCode);
									}
									for (int i = 0; i < list.Count; i++)
									{
										MagicAction.ProcessAction(attacker, null, list[i].MagicActionID, list[i].MagicActionParams, num2, num3, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, attacker);
								}
							}
							else
							{
								int direction = (int)attacker.CurrentDir;
								Point point;
								if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
								{
									point = attacker.CurrentPos;
								}
								else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
								{
									point = new Point((double)num2, (double)num3);
									if (-1 != num)
									{
										if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, num, out point))
										{
											point = new Point((double)num2, (double)num3);
										}
									}
									direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								else
								{
									point = new Point((double)num2, (double)num3);
									direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								List<object> list6 = new List<object>();
								List<MagicActionItem> list5 = null;
								if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out list5) || null == list5)
								{
								}
								MagicActionItem magicActionItem = null;
								if (list5 != null && list5.Count > 0)
								{
									magicActionItem = list5[0];
								}
								if (magicActionItem != null)
								{
									if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
									{
										GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list6);
									}
									else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
									{
										GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6, magicActionItem.MagicActionParams[0], true);
									}
								}
								else
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6);
								}
								int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
								if (1 != intValue3)
								{
									if (2 == intValue3)
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, num2, num3, magicCode);
										}
										for (int i = 0; i < list.Count; i++)
										{
											MagicAction.ProcessAction(attacker, attacker, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, attacker);
										for (int j = 0; j < list6.Count; j++)
										{
											if ((list6[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
											{
												if (!Global.IsOpposition(attacker, list6[j] as GameClient))
												{
													for (int i = 0; i < list.Count; i++)
													{
														MagicAction.ProcessAction(attacker, list6[j] as GameClient, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
													}
													if (intValue2 == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list6[j] as GameClient).ClientData.RoleID, num2, num3, magicCode);
													}
													ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, list6[j] as GameClient);
													ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, list6[j] as GameClient);
													if (--num4 <= 0)
													{
														break;
													}
												}
											}
										}
									}
									else if (-1 == intValue3 || 3 == intValue3)
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, num2, num3, magicCode);
										}
										for (int j = 0; j < list6.Count; j++)
										{
											if ((list6[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
											{
												if (Global.IsOpposition(attacker, list6[j] as GameClient))
												{
													for (int i = 0; i < list.Count; i++)
													{
														MagicAction.ProcessAction(attacker, list6[j] as GameClient, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
													}
													if (intValue2 == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list6[j] as GameClient).ClientData.RoleID, num2, num3, magicCode);
													}
													if (--num4 <= 0)
													{
														break;
													}
												}
											}
										}
										List<object> list7 = new List<object>();
										if (magicActionItem != null)
										{
											if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												GameManager.MonsterMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list7, 1);
											}
											else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, magicActionItem.MagicActionParams[0], true, 1);
											}
										}
										else
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, 1);
										}
										for (int j = 0; j < list7.Count; j++)
										{
											if (Global.IsOpposition(attacker, list7[j] as Monster))
											{
												for (int i = 0; i < list.Count; i++)
												{
													MagicAction.ProcessAction(attacker, list7[j] as Monster, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list7[j] as Monster).RoleID, num2, num3, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, list7[j] as Monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, list7[j] as Monster);
												if (--num4 <= 0)
												{
													break;
												}
											}
										}
										List<object> list8 = new List<object>();
										BiaoCheManager.LookupRangeAttackEnemies(attacker, (int)point.X, (int)point.Y, direction, systemXmlItem.GetStringValue("AttackDistance"), list8);
										for (int j = 0; j < list8.Count; j++)
										{
											for (int i = 0; i < list.Count; i++)
											{
												MagicAction.ProcessAction(attacker, list8[j] as BiaoCheItem, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (intValue2 == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list8[j] as BiaoCheItem).BiaoCheID, num2, num3, magicCode);
											}
											if (--num4 <= 0)
											{
												break;
											}
										}
										List<object> list9 = new List<object>();
										JunQiManager.LookupRangeAttackEnemies(attacker, (int)point.X, (int)point.Y, direction, systemXmlItem.GetStringValue("AttackDistance"), list9);
										for (int j = 0; j < list9.Count; j++)
										{
											for (int i = 0; i < list.Count; i++)
											{
												MagicAction.ProcessAction(attacker, list9[j] as JunQiItem, list[i].MagicActionID, list[i].MagicActionParams, (int)point.X, (int)point.Y, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (intValue2 == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list9[j] as JunQiItem).JunQiID, num2, num3, magicCode);
											}
											if (--num4 <= 0)
											{
												break;
											}
										}
									}
									else
									{
										if (intValue2 <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, num2, num3, magicCode);
										}
										for (int i = 0; i < list.Count; i++)
										{
											MagicAction.ProcessAction(attacker, null, list[i].MagicActionID, list[i].MagicActionParams, num2, num3, num5, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, attacker);
									}
								}
							}
						}
					}
				}
			}
		}

		private static void _ProcessAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, bool recAttackTicks, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 == magicCode)
			{
				SpriteAttack.ProcessPhyAttackByMonster(attacker, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
			else
			{
				bool bFindTarget = true;
				try
				{
					bFindTarget = SpriteAttack.ProcessMagicAttackByMonster(attacker, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
				}
				finally
				{
					SpriteAttack.ProcessManyAttackMagicFinish(bFindTarget, attacker);
				}
			}
		}

		private static void ProcessPhyAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			enemy = SpriteAttack.VerifyEnemyID(attacker, attacker.MonsterZoneNode.MapCode, enemy, enemyX, enemyY);
			if (-1 == enemy)
			{
				int direction = (int)attacker.Direction;
				if (-1 != enemyX && -1 != enemyY)
				{
					direction = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, attacker.SafeCoordinate.X, attacker.SafeCoordinate.Y);
				}
				List<int> list = new List<int>();
				GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY, 200, list, 135.0, true);
				GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY, 200, list, 125.0, true);
				if (list.Count > 0)
				{
					int randomNumber = Global.GetRandomNumber(0, list.Count);
					enemy = list[randomNumber];
				}
			}
			if (-1 != enemy)
			{
				if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, enemy))
				{
					enemy = -1;
				}
			}
			if (enemy != -1)
			{
				List<int> list2 = attacker.ExtensionProps.GetIDs();
				if (null != list2)
				{
					list2 = ExtensionPropsMgr.ProcessExtensionProps(list2, magicCode, 0);
				}
				List<int> list3 = attacker.ExtensionProps.GetIDs();
				if (null != list3)
				{
					list3 = ExtensionPropsMgr.ProcessExtensionProps(list3, magicCode, 1);
				}
				GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
				if (spriteType == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
					if (null != monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, monster);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, monster);
					}
				}
				else if (spriteType != GSpriteTypes.BiaoChe)
				{
					if (spriteType == GSpriteTypes.JunQi)
					{
						JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemy);
						if (junQiItem != null && null != attacker.OwnerClient)
						{
							JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, junQiItem, 0, 0, manyRangeInjuredPercent, attacker.MonsterInfo.AttackType, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, junQiItem);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, junQiItem);
						}
					}
					else
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(enemy);
						if (null != gameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, gameClient, 0, 0, manyRangeInjuredPercent, attacker.MonsterInfo.AttackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(list2, attacker, gameClient);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, gameClient);
						}
					}
				}
			}
		}

		private static bool ProcessMagicAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			bool result;
			if (-1 == magicCode)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
				{
					result = false;
				}
				else
				{
					List<MagicActionItem> list = null;
					if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out list) || null == list)
					{
					}
					MagicActionItem magicActionItem = null;
					if (list != null && list.Count > 0)
					{
						magicActionItem = list[0];
					}
					int intValue = systemXmlItem.GetIntValue("AttackDistance", -1);
					int num = systemXmlItem.GetIntValue("MaxNum", -1);
					List<MagicActionItem> list2 = null;
					if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out list2) || null == list2)
					{
						result = false;
					}
					else
					{
						int num2 = 0;
						List<int> list3 = new List<int>();
						List<int> list4 = new List<int>();
						if (manyRangeIndex <= 0)
						{
							num2 = Global.GetNeedMagicV(attacker, magicCode, 1);
							if (num2 > 0)
							{
								int num3 = (int)attacker.MonsterInfo.VManaMax;
								int num4 = num3 * (num2 / 100);
								if (attacker.VMana - (double)num4 <= 0.0)
								{
									return false;
								}
							}
							GameManager.MonsterMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (double)num2);
							list3 = attacker.ExtensionProps.GetIDs();
							if (null != list3)
							{
								list3 = ExtensionPropsMgr.ProcessExtensionProps(list3, magicCode, 0);
							}
							list4 = attacker.ExtensionProps.GetIDs();
							if (null != list4)
							{
								list4 = ExtensionPropsMgr.ProcessExtensionProps(list4, magicCode, 1);
							}
						}
						int intValue2 = systemXmlItem.GetIntValue("TargetPlayingType", -1);
						int direction = 0;
						bool flag = false;
						if (systemXmlItem.GetIntValue("MagicType", -1) == 1 || systemXmlItem.GetIntValue("MagicType", -1) == 3)
						{
							flag = true;
							int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
							if (1 == intValue3)
							{
								if (systemXmlItem.GetIntValue("MagicType", -1) != 3)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
								}
								bool flag2 = false;
								int i = 0;
								while (i < list2.Count)
								{
									if (list2[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
									{
										goto IL_2B3;
									}
									if (Global.GetTwoPointDistance(attacker.CurrentPos, new Point((double)enemyX, (double)enemyY)) <= (double)intValue)
									{
										goto IL_2B3;
									}
									IL_2FF:
									i++;
									continue;
									IL_2B3:
									flag2 |= MagicAction.ProcessAction(attacker, attacker, list2[i].MagicActionID, list2[i].MagicActionParams, enemyX, enemyY, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									goto IL_2FF;
								}
								if (flag2)
								{
									if (systemXmlItem.GetIntValue("MagicType", -1) == 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
									}
								}
								ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, attacker);
							}
							else if (-1 == intValue3 || 2 == intValue3 || 3 == intValue3)
							{
								direction = (int)attacker.CurrentDir;
								if (-1 != enemyX && -1 != enemyY)
								{
									direction = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								if (2 == intValue3)
								{
									if (-1 == enemy)
									{
										enemy = attacker.GetObjectID();
									}
									else if (attacker.GetObjectID() != enemy)
									{
										enemy = -1;
									}
								}
								else if (-1 == intValue3 || 3 == intValue3)
								{
									if (-1 == enemy || enemy == attacker.GetObjectID())
									{
										if (-1 == enemy)
										{
											Point point = new Point((double)enemyX, (double)enemyY);
											if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
											{
												point = new Point((double)enemyX, (double)enemyY);
											}
											else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
											{
												if (-1 != enemy)
												{
													if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out point))
													{
														point = new Point((double)enemyX, (double)enemyY);
													}
												}
												direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
											}
											else
											{
												direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
											}
											List<object> list5 = new List<object>();
											GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)point.X, (int)point.Y, 50, list5);
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)point.X, (int)point.Y, 50, list5);
											if (list5.Count > 0)
											{
												int randomNumber = Global.GetRandomNumber(0, list5.Count);
												enemy = (list5[randomNumber] as IObject).GetObjectID();
											}
										}
									}
									else if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, enemy))
									{
										enemy = -1;
									}
								}
								if (intValue2 <= 0)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
								}
								if (-1 != enemy)
								{
									GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
									if (spriteType == GSpriteTypes.Monster)
									{
										Monster monster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
										if (null != monster)
										{
											if (1 == intValue2)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, monster.RoleID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < list2.Count; i++)
											{
												MagicAction.ProcessAction(attacker, monster, list2[i].MagicActionID, list2[i].MagicActionParams, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, monster);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, monster);
										}
									}
									else if (spriteType == GSpriteTypes.BiaoChe)
									{
										BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemy);
										if (null != biaoCheItem)
										{
										}
									}
									else if (spriteType == GSpriteTypes.JunQi)
									{
										JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemy);
										if (null != junQiItem)
										{
											if (1 == intValue2)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, junQiItem.GetObjectID(), enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < list2.Count; i++)
											{
												MagicAction.ProcessAction(attacker, junQiItem, list2[i].MagicActionID, list2[i].MagicActionParams, (int)junQiItem.CurrentPos.X, (int)junQiItem.CurrentPos.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, junQiItem);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, junQiItem);
										}
									}
									else if (spriteType == GSpriteTypes.FakeRole)
									{
										FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemy);
										if (null != fakeRoleItem)
										{
										}
									}
									else
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(enemy);
										if (null != gameClient)
										{
											if (1 == intValue2)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, gameClient.ClientData.RoleID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < list2.Count; i++)
											{
												MagicAction.ProcessAction(attacker, gameClient, list2[i].MagicActionID, list2[i].MagicActionParams, gameClient.ClientData.PosX, gameClient.ClientData.PosY, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, gameClient);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, gameClient);
										}
									}
								}
							}
							else
							{
								if (intValue2 <= 0)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
								}
								for (int i = 0; i < list2.Count; i++)
								{
									MagicAction.ProcessAction(attacker, null, list2[i].MagicActionID, list2[i].MagicActionParams, enemyX, enemyY, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
								}
								ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, attacker);
							}
						}
						else
						{
							direction = (int)attacker.CurrentDir;
							Point point;
							if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
							{
								point = attacker.CurrentPos;
							}
							else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
							{
								point = new Point((double)enemyX, (double)enemyY);
								if (-1 != enemy)
								{
									if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out point))
									{
										point = new Point((double)enemyX, (double)enemyY);
									}
								}
								direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
							}
							else
							{
								point = new Point((double)enemyX, (double)enemyY);
								direction = (int)Global.GetDirectionByTan((double)((int)point.X), (double)((int)point.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
							}
							List<object> list6 = new List<object>();
							if (magicActionItem != null)
							{
								if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
								{
									GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list6);
								}
								else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
								{
									GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6, magicActionItem.MagicActionParams[0], true);
								}
								else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6);
								}
							}
							else
							{
								GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list6);
							}
							int intValue3 = systemXmlItem.GetIntValue("TargetType", -1);
							if (1 != intValue3)
							{
								if (2 == intValue3)
								{
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < list2.Count; i++)
									{
										MagicAction.ProcessAction(attacker, attacker, list2[i].MagicActionID, list2[i].MagicActionParams, (int)point.X, (int)point.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									flag = true;
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, attacker);
									for (int j = 0; j < list6.Count; j++)
									{
										if ((list6[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (!Global.IsOpposition(attacker, list6[j] as GameClient))
											{
												for (int i = 0; i < list2.Count; i++)
												{
													MagicAction.ProcessAction(attacker, list6[j] as GameClient, list2[i].MagicActionID, list2[i].MagicActionParams, (int)point.X, (int)point.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list6[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, list6[j] as GameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, list6[j] as GameClient);
												if (--num <= 0)
												{
													break;
												}
											}
										}
									}
								}
								else if (-1 == intValue3 || 3 == intValue3)
								{
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									List<object> list7 = new List<object>();
									if (magicActionItem != null)
									{
										if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
										{
											GameManager.MonsterMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list7, 1);
										}
										else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
										{
											GameManager.MonsterMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, magicActionItem.MagicActionParams[0], true, 1);
										}
										else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, 1);
										}
									}
									else
									{
										GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)point.X, (int)point.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list7, 1);
									}
									List<object> list8 = new List<object>();
									foreach (object obj in list7)
									{
										if (Global.IsOpposition(attacker, obj as Monster))
										{
											list8.Add(obj);
										}
									}
									foreach (object obj in list6)
									{
										if ((obj as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (Global.IsOpposition(attacker, obj as GameClient))
											{
												list8.Add(obj);
											}
										}
									}
									double magicCodeAddPercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(attacker, list8, magicCode);
									for (int j = 0; j < list6.Count; j++)
									{
										if ((list6[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (Global.IsOpposition(attacker, list6[j] as GameClient))
											{
												for (int i = 0; i < list2.Count; i++)
												{
													MagicAction.ProcessAction(attacker, list6[j] as GameClient, list2[i].MagicActionID, list2[i].MagicActionParams, (int)point.X, (int)point.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, magicCode, magicCodeAddPercent);
												}
												flag = true;
												if (intValue2 == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list6[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, list6[j] as GameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, list6[j] as GameClient);
												if (--num <= 0)
												{
													break;
												}
											}
										}
									}
									for (int j = 0; j < list7.Count; j++)
									{
										if (Global.IsOpposition(attacker, list7[j] as Monster))
										{
											for (int i = 0; i < list2.Count; i++)
											{
												MagicAction.ProcessAction(attacker, list7[j] as Monster, list2[i].MagicActionID, list2[i].MagicActionParams, (int)point.X, (int)point.Y, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, magicCode, magicCodeAddPercent);
											}
											flag = true;
											if (intValue2 == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list7[j] as Monster).RoleID, enemyX, enemyY, magicCode);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, list7[j] as Monster);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(list4, attacker, list7[j] as Monster);
											if (--num <= 0)
											{
												break;
											}
										}
									}
									List<object> list9 = new List<object>();
									JunQiManager.LookupRangeAttackEnemies(attacker, (int)point.X, (int)point.Y, direction, systemXmlItem.GetStringValue("AttackDistance"), list9);
									for (int j = 0; j < list9.Count; j++)
									{
										for (int i = 0; i < list2.Count; i++)
										{
											MagicAction.ProcessAction(attacker, list9[j] as JunQiItem, list2[i].MagicActionID, list2[i].MagicActionParams, (int)point.X, (int)point.Y, num2, 1, -1, 0, 0, direction, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										if (intValue2 == 1)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (list9[j] as JunQiItem).JunQiID, enemyX, enemyY, magicCode);
										}
									}
								}
								else
								{
									if (intValue2 <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < list2.Count; i++)
									{
										MagicAction.ProcessAction(attacker, null, list2[i].MagicActionID, list2[i].MagicActionParams, enemyX, enemyY, num2, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(list3, attacker, attacker);
								}
							}
						}
						result = flag;
					}
				}
			}
			return result;
		}

		public static void ProcessManyAttackMagicFinish(bool bFindTarget, Monster attacker)
		{
			if (!bFindTarget || attacker.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() < 1)
			{
				attacker.MagicFinish = 1;
				attacker.CurrentMagic = -1;
				GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.RoleID, attacker.MonsterZoneNode.MapCode, -1, 116);
			}
		}
	}
}
