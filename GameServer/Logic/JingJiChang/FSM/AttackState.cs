using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang.FSM
{
	internal class AttackState : IFSMState
	{
		public AttackState(GameClient player, Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
			this.target = player;
			this.owner.LockObject = player.GetObjectID();
			EMagicSwordTowardType magicSwordTypeByWeapon = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(owner.getRoleDataMini().Occupation, owner.getRoleDataMini().GoodsDataList, null);
			this.fiveComboSkillList = JingJiChangConstants.getJingJiChangeFiveCombatSkillList(owner.getRoleDataMini().Occupation, magicSwordTypeByWeapon);
		}

		public void onBegin()
		{
			this.changeAction(GActions.Stand);
			this.benginCombatTime = TimeUtil.NOW() + 2000L;
		}

		public void onEnd()
		{
			this.simulateEndTime = 0L;
			this.castSimulateEndTime = 0L;
			this.skillSpellCDTime = 0L;
			this.benginCombatTime = 0L;
		}

		public void onUpdate(long ticks)
		{
			if (ticks >= this.benginCombatTime)
			{
				if (this.owner.VLife <= 0.0)
				{
					this.owner.MyMagicsManyTimeDmageQueue.Clear();
					this.FSM.switchState(AIState.DEAD);
				}
				else if (!this.owner.IsMonsterDongJie())
				{
					SpriteAttack.ExecMagicsManyTimeDmageQueueEx(this.owner);
					if (null == this.target)
					{
						this.FSM.switchState(AIState.RETURN);
					}
					else if (this.target.ClientData.CurrentLifeV <= 0)
					{
						this.FSM.switchState(AIState.RETURN);
					}
					else
					{
						if (this.castSimulateEndTime > 0L)
						{
							if (ticks > this.castSimulateEndTime)
							{
								this.castSimulateEndTime = 0L;
								int num = 0;
								if (this.testAttackDistance(out num))
								{
									this.owner.Direction = (double)num;
									SpriteAttack.ProcessAttackByJingJiRobot(this.owner, this.target, this.skillId, -1, 1.0);
								}
							}
						}
						if (this.simulateEndTime > 0L)
						{
							if (ticks >= this.simulateEndTime)
							{
								this.simulateEndTime = 0L;
								int nextSkillID = Global.GetNextSkillID(this.skillId);
								if (nextSkillID <= 0)
								{
									if (this.isCombatCD)
									{
										this.changeAction(GActions.Stand);
										this.skillSpellCDTime = ticks + 500L;
									}
								}
							}
						}
						else if (this.skillSpellCDTime <= 0L || ticks >= this.skillSpellCDTime)
						{
							if (this.skillSpellCDTime > 0L && ticks >= this.skillSpellCDTime)
							{
								this.skillSpellCDTime = 0L;
							}
							else
							{
								if (this.isUseFiveComboSkill)
								{
									this.selectFiveComboSkill();
								}
								else
								{
									bool flag;
									this.selectSkill(out flag);
									if (this.skillId == -1)
									{
										return;
									}
									if (flag)
									{
										this.isCombatCD = false;
										this.isUseFiveComboSkill = true;
										return;
									}
									this.isCombatCD = true;
								}
								int direction = 0;
								if (!this.testAttackDistance(out direction))
								{
									this.moveTo(ticks);
								}
								else
								{
									if (this.owner.Action == GActions.Run)
									{
										this.owner.Direction = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
										this.changeAction(GActions.Stand);
									}
									this.attack(direction);
								}
							}
						}
					}
				}
			}
		}

		private bool testAttackDistance(out int direction)
		{
			direction = 0;
			SystemXmlItem systemXmlItem = null;
			bool result;
			if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this.skillId, out systemXmlItem))
			{
				result = false;
			}
			else
			{
				List<MagicActionItem> list = null;
				if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(this.skillId, out list) || null == list)
				{
				}
				List<MagicActionItem> list2 = null;
				if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(this.skillId, out list2) || null == list2)
				{
					result = false;
				}
				else
				{
					MagicActionItem magicActionItem = null;
					if (list != null && list.Count > 0)
					{
						magicActionItem = list[0];
					}
					int intValue = systemXmlItem.GetIntValue("AttackDistance", -1);
					if (systemXmlItem.GetIntValue("MagicType", -1) == 1 || systemXmlItem.GetIntValue("MagicType", -1) == 3)
					{
						int intValue2 = systemXmlItem.GetIntValue("TargetType", -1);
						if (1 == intValue2)
						{
							return true;
						}
						if (this.skillId == 11004 && 4 == intValue2)
						{
							return true;
						}
						if (2 == intValue2)
						{
							return false;
						}
					}
					else
					{
						int num = 0;
						Point currentPos;
						if (1 == systemXmlItem.GetIntValue("TargetPos", -1))
						{
							currentPos = this.owner.CurrentPos;
						}
						else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
						{
							currentPos = this.target.CurrentPos;
							num = (int)Global.GetDirectionByTan((double)((int)currentPos.X), (double)((int)currentPos.Y), this.owner.CurrentPos.X, this.owner.CurrentPos.Y);
						}
						else
						{
							currentPos = this.target.CurrentPos;
							num = (int)Global.GetDirectionByTan((double)((int)currentPos.X), (double)((int)currentPos.Y), this.owner.CurrentPos.X, this.owner.CurrentPos.Y);
						}
						List<object> list3 = new List<object>();
						direction = num;
						if (magicActionItem != null)
						{
							if (magicActionItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
							{
								GameManager.ClientMgr.LookupRolesInSquare(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y, (int)currentPos.X, (int)currentPos.Y, (int)magicActionItem.MagicActionParams[0], (int)magicActionItem.MagicActionParams[1], list3);
							}
							else if (magicActionItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
							{
								GameManager.ClientMgr.LookupEnemiesInCircleByAngle(num, this.owner.CurrentMapCode, this.owner.CopyMapID, (int)currentPos.X, (int)currentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list3, magicActionItem.MagicActionParams[0], true);
							}
							else if (magicActionItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
							{
								GameManager.ClientMgr.LookupEnemiesInCircle(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list3);
							}
						}
						else
						{
							GameManager.ClientMgr.LookupEnemiesInCircle(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)currentPos.X, (int)currentPos.Y, Global.SafeConvertToInt32(systemXmlItem.GetStringValue("AttackDistance")), list3);
						}
						if (list3.Count <= 0)
						{
							return false;
						}
						for (int i = 0; i < list3.Count; i++)
						{
							if ((list3[i] as GameClient).ClientData.RoleID == this.target.GetObjectID())
							{
								return true;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		private void selectFiveComboSkill()
		{
			if (!this.isSelectFiveComboSkill)
			{
				this.skillId = this.fiveComboSkillList[this.fiveComboSkillIndex];
				if (this.fiveComboSkillIndex >= this.fiveComboSkillList.Length - 1)
				{
					this.fiveComboSkillIndex = 0;
					this.isUseFiveComboSkill = false;
				}
				else
				{
					this.fiveComboSkillIndex++;
					this.isSelectFiveComboSkill = true;
				}
			}
		}

		private void selectSkill(out bool isFiveCombo)
		{
			this.skillId = -1;
			isFiveCombo = false;
			if (null != this.owner.MonsterInfo.SkillIDs)
			{
				int nextSkillID = Global.GetNextSkillID(this.prevSkillID);
				if (nextSkillID > 0)
				{
					this.skillId = nextSkillID;
					this.prevSkillID = nextSkillID;
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(0, this.owner.MonsterInfo.SkillIDs.Length);
					for (int i = randomNumber; i < this.owner.MonsterInfo.SkillIDs.Length; i++)
					{
						if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(this.owner.MonsterInfo.SkillIDs[i]))
						{
							if (this.SkillNeedMagicVOk(this.owner.MonsterInfo.SkillIDs[i]))
							{
								this.skillId = this.owner.MonsterInfo.SkillIDs[i];
								break;
							}
						}
					}
					if (this.skillId == -1)
					{
						for (int i = randomNumber - 1; i >= 0; i--)
						{
							if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(this.owner.MonsterInfo.SkillIDs[i]))
							{
								if (this.SkillNeedMagicVOk(this.owner.MonsterInfo.SkillIDs[i]))
								{
									this.skillId = this.owner.MonsterInfo.SkillIDs[i];
								}
							}
						}
					}
					if (!this.isTryHighPrioritySkill)
					{
						EMagicSwordTowardType magicSwordTypeByWeapon = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(this.owner.getRoleDataMini().Occupation, this.owner.getRoleDataMini().GoodsDataList, null);
						int jingJiChangeHighPrioritySkill = JingJiChangConstants.GetJingJiChangeHighPrioritySkill(this.owner.getRoleDataMini().Occupation, magicSwordTypeByWeapon);
						if (jingJiChangeHighPrioritySkill != -1)
						{
							if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(jingJiChangeHighPrioritySkill) && this.SkillNeedMagicVOk(jingJiChangeHighPrioritySkill))
							{
								this.skillId = jingJiChangeHighPrioritySkill;
							}
						}
						this.isTryHighPrioritySkill = true;
					}
					this.prevSkillID = this.skillId;
					for (int i = 0; i < this.fiveComboSkillList.Length; i++)
					{
						if (this.fiveComboSkillList[i] == this.skillId)
						{
							isFiveCombo = true;
							break;
						}
					}
				}
			}
		}

		private bool SkillNeedMagicVOk(int skillID)
		{
			int needMagicV = Global.GetNeedMagicV(this.owner, skillID, 1);
			if (needMagicV > 0)
			{
				int num = (int)this.owner.MonsterInfo.VManaMax;
				int num2 = num * (needMagicV / 100);
				num2 = Global.GMax(0, num2);
				if (this.owner.VMana - (double)num2 < 0.0)
				{
					return false;
				}
			}
			return true;
		}

		private void moveTo(long ticks)
		{
			if (ticks >= this.moveEndTime)
			{
				Point currentGrid = this.owner.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				Point currentGrid2 = this.target.CurrentGrid;
				int num3 = (int)currentGrid2.X;
				int num4 = (int)currentGrid2.Y;
				int num5 = (int)this.owner.Direction;
				if (num != num3 || num2 != num4)
				{
					int num6 = num3;
					int num7 = num4;
					if (num6 > num)
					{
						num5 = 2;
						if (num7 > num2)
						{
							num5 = 1;
						}
						else if (num7 < num2)
						{
							num5 = 3;
						}
					}
					else if (num6 < num)
					{
						num5 = 6;
						if (num7 > num2)
						{
							num5 = 7;
						}
						else if (num7 < num2)
						{
							num5 = 5;
						}
					}
					else if (num7 > num2)
					{
						num5 = 0;
					}
					else if (num7 < num2)
					{
						num5 = 4;
					}
					this.owner.Direction = (double)num5;
					int num8 = num;
					int num9 = num2;
					ChuanQiUtils.RunTo1(this.owner, (Dircetions)num5);
					currentGrid = this.owner.CurrentGrid;
					num = (int)currentGrid.X;
					num2 = (int)currentGrid.Y;
					for (int i = 0; i < 7; i++)
					{
						if (num8 != num || num9 != num2)
						{
							break;
						}
						if (Global.GetRandomNumber(0, 3) > 0)
						{
							num5++;
						}
						else if (num5 > 0)
						{
							num5--;
						}
						else
						{
							num5 = 7;
						}
						if (num5 > 7)
						{
							num5 = 0;
						}
						ChuanQiUtils.RunTo1(this.owner, (Dircetions)num5);
						currentGrid = this.owner.CurrentGrid;
						num = (int)currentGrid.X;
						num2 = (int)currentGrid.Y;
					}
				}
				this.moveEndTime = ticks + 600L;
			}
		}

		private void attack(int direction)
		{
			if (!this.owner.IsMoving)
			{
				if (null != this.target)
				{
					double num = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
					if (num != this.owner.SafeDirection)
					{
						this.owner.Direction = (double)((int)num);
					}
					if (this.owner.EnemyTarget != this.target.CurrentPos)
					{
						this.owner.EnemyTarget = this.target.CurrentPos;
					}
					this.owner.CurrentMagic = this.skillId;
					if (this.skillId > 0)
					{
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict[this.skillId].GetStringValue("SkillAction") == "" || GameManager.SystemMagicsMgr.SystemXmlItemDict[this.skillId].GetStringValue("SkillAction").Equals(""))
						{
							this.changeAction(GActions.Attack);
						}
						else
						{
							GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.owner, this.owner.RoleID, this.owner.MonsterZoneNode.MapCode, this.skillId, 116);
							this.changeAction(GActions.Magic);
						}
					}
					else
					{
						this.changeAction(GActions.Attack);
					}
					this.isSelectFiveComboSkill = false;
					this.simulate();
				}
			}
		}

		private void changeAction(GActions action)
		{
			if (this.owner.VLife > 0.0)
			{
				double num = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
				List<object> all9Clients = Global.GetAll9Clients(this.owner);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.owner, this.owner.MonsterZoneNode.MapCode, this.owner.CopyMapID, this.owner.RoleID, (int)num, (int)action, (int)this.owner.SafeCoordinate.X, (int)this.owner.SafeCoordinate.Y, (int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, 114, all9Clients);
				Global.RemoveStoryboard(this.owner.Name);
				this.monsterMoving.ChangeDirection(this.owner, num);
				this.owner.Action = action;
			}
		}

		private void simulate()
		{
			int num;
			int num2;
			if (this.skillId == -1)
			{
				num = 3;
				num2 = 3;
			}
			else
			{
				num = 5;
				num2 = 5;
				for (int i = 0; i < JingJiChangConstants.SkillFrameCounts.Length; i++)
				{
					if (this.skillId == JingJiChangConstants.SkillFrameCounts[i][0])
					{
						num = JingJiChangConstants.SkillFrameCounts[i][1];
						num2 = JingJiChangConstants.SkillFrameCounts[i][2];
						break;
					}
				}
			}
			this.simulateEndTime = TimeUtil.NOW() + (long)(num * 100);
			this.castSimulateEndTime = TimeUtil.NOW() + (long)(num2 * 100);
		}

		public static readonly AIState state = AIState.ATTACK;

		private Robot owner = null;

		private FinishStateMachine FSM = null;

		private long moveEndTime = 0L;

		private MonsterMoving monsterMoving = new MonsterMoving();

		private GameClient target = null;

		private int skillId = -1;

		private int prevSkillID = -1;

		private long simulateEndTime = 0L;

		private long castSimulateEndTime = 0L;

		private long skillSpellCDTime = 0L;

		private long benginCombatTime = 0L;

		private int[] fiveComboSkillList;

		private bool isTryHighPrioritySkill = false;

		private bool isCombatCD = true;

		private bool isUseFiveComboSkill = false;

		private int fiveComboSkillIndex = 0;

		private bool isSelectFiveComboSkill = false;
	}
}
