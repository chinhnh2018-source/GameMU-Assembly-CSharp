using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.GoodsPack;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameEngine.Teleport;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.Tools.AStar;
using Server.Data;
using Server.Tools;
using Server.Tools.AStarEx;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

namespace HSGameEngine.GameEngine.Scene
{
	public class GScene
	{
		public GScene()
		{
			this.Scene_Loaded();
		}

		protected void Scene_Loaded()
		{
			this.clientHeartTimer = new DispatcherTimer("clientHeartTimer");
			this.clientHeartTimer.Interval = TimeSpan.FromSeconds(10.0);
			this.clientHeartTimer.Tick = new DispatcherTimerEventHandler(this.clientHeartTimer_Tick);
			this.clientHeartTimer.Start();
			this.auxiliaryTimer = new DispatcherTimer("auxiliaryTimer");
			this.auxiliaryTimer.Interval = TimeSpan.FromMilliseconds(1000.0);
			this.auxiliaryTimer.Tick = new DispatcherTimerEventHandler(this.auxiliaryTimer_Tick);
			this.auxiliaryTimer.Start();
			this.LeaderMovingTimer = new DispatcherTimer("LeaderMovingTimer");
			this.LeaderMovingTimer.Interval = TimeSpan.FromMilliseconds(1000.0);
			this.LeaderMovingTimer.Tick = new DispatcherTimerEventHandler(this.LeaderMovingTick);
			this.LeaderMovingTimer.Start();
			if (ConfigSystemParam.GetSystemParamIntByName("IsRemovePingTimer") != 1L)
			{
				bool flag = DispatcherTimerDriver.RemoveTimer("PingTimer");
				if (flag)
				{
					MUDebug.LogError<string>(new string[]
					{
						"错误：存在多份PingTimer"
					});
				}
			}
			this.PingTimer = new DispatcherTimer("PingTimer");
			this.PingTimer.Interval = TimeSpan.FromMilliseconds(2000.0);
			this.PingTimer.Tick = new DispatcherTimerEventHandler(this.PingTimeTick);
			this.PingTimer.Start();
			this.TraceEnemyTimer = new DispatcherTimer("TraceEnemyTimer");
			this.TraceEnemyTimer.Interval = TimeSpan.FromMilliseconds(500.0);
			this.TraceEnemyTimer.Tick = new DispatcherTimerEventHandler(this.TraceEnemyTimer_Tick);
			this.TraceEnemyTimer.Start();
			LGQualityManager.OnMaxVisiblePlayerChanged = new Action(this.UpdateOtherRoleVisible);
		}

		public int onRenderScene()
		{
			this.OnFrameEvents();
			if (Time.frameCount % 5 == 0)
			{
				this.AddListMonster(false);
				this.AddListRole(false);
			}
			return 0;
		}

		private void ProcessExtAction(ExtActionTypes extAction)
		{
			switch (extAction)
			{
			case ExtActionTypes.EXTACTION_NPCDLG:
			{
				int num = 2130706432 + Global.Data.TargetNpcID;
				string name = StringUtil.substitute("Role_{0}", new object[]
				{
					num
				});
				GSprite gsprite = this.FindSprite(name);
				Point point = new Point(0, 0);
				if (gsprite != null)
				{
					point = gsprite.Coordinate;
				}
				else
				{
					point = Global.GetNPCPointByID(Global.Data.roleData.MapCode, Global.Data.TargetNpcID);
				}
				if (Global.InCircle(this.Leader.Coordinate, point, 800.0) && this.LeftButtonClickOnSprite != null)
				{
					double num2 = this.CalcDirection(this.Leader, point);
					this.Leader.Action = GActions.Stand;
					if ((int)num2 != this.Leader.Direction)
					{
						this.InstantStand((int)num2);
					}
					if (gsprite != null)
					{
						gsprite.PlayNpcTalkSound();
					}
					this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
					{
						RoleID = 2130706432 + Global.Data.TargetNpcID,
						SpriteType = GSpriteTypes.NPC,
						ShowDlg = true,
						ExtensionID = Global.Data.TargetNpcID
					});
				}
				break;
			}
			case ExtActionTypes.EXTACTION_KILLMONSTER:
				if (this.Leader.LockObject == null)
				{
					GSprite gsprite2 = this.SeekMonsterToLock(false, null, Global.Data.TargetNpcID);
					if (gsprite2 != null)
					{
						this.Leader.LockObject = gsprite2.Name;
						Global.WatchSprite = gsprite2.Name;
						this.SetLockDeco(gsprite2, true);
						if (this.LeftButtonClickOnSprite != null)
						{
							this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
							{
								RoleID = gsprite2.RoleID,
								SpriteType = gsprite2.SpriteType,
								ShowDlg = false,
								ExtensionID = gsprite2.ExtensionID
							});
						}
					}
					this.StartAutoFight(Global.Data.TargetNpcID);
				}
				break;
			case ExtActionTypes.EXTACTION_GETGOODSPACK:
				GameInstance.Game.SpriteNotifyGetGoodsPack();
				break;
			case ExtActionTypes.EXTACTION_CAIJI:
			{
				GSprite gsprite3 = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
				{
					2130706432 + Global.Data.TargetNpcID
				}));
				if (gsprite3 != null)
				{
					this.SetLockDeco(gsprite3, true);
					if (this.LeftButtonClickOnSprite != null)
					{
						this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
						{
							RoleID = gsprite3.RoleID,
							SpriteType = gsprite3.SpriteType,
							ShowDlg = false,
							ExtensionID = gsprite3.ExtensionID
						});
					}
				}
				break;
			}
			}
		}

		private bool InstantAttack(double direction, int yAngle, Point to, Point targetPos, int attackNum, bool shiftPressed = false)
		{
			if (Global.IsDongJieSprite(this.Leader))
			{
				return false;
			}
			if (this.Leader.LockObject != null)
			{
				GSprite gsprite = this.FindSprite(this.Leader.LockObject);
				if (gsprite != null && !Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
				{
					return false;
				}
			}
			if (!this.IsMagicWaiting(this.Leader.Occupation))
			{
				int autoUseSkillID = this.GetAutoUseSkillID(shiftPressed);
				this.DoMagicAttack(autoUseSkillID, targetPos, this.Leader.LockObject, false, true);
			}
			if (attackNum != -1)
			{
				this.Leader.Direction = (int)direction;
				this.Leader.Action = GActions.Attack;
				this.Leader.YAngle = yAngle;
				GameInstance.Game.SpriteAction(direction, 3, to, targetPos, yAngle, new Point(0, 0));
			}
			return true;
		}

		private void InstantSite()
		{
		}

		private void InstantStand(int direction = -1)
		{
			Global.RemoveStoryboard(this.Leader.Name);
			if (direction >= 0 && direction <= 7)
			{
				this.Leader.Direction = direction;
			}
			GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
		}

		private void SpellCasting(GSprite magicer, int magicCode, bool notChangeDirection = false)
		{
			if (Global.FindMoveStroyboard(this.Leader.Name))
			{
				return;
			}
			double num = (double)magicer.Direction;
			Quaternion rotation = this.Leader.Rotation;
			if (magicer.TargetSrpite != null)
			{
				GSprite gsprite = this.FindSprite(magicer.TargetSrpite);
				if (gsprite != null)
				{
					num = this.CalcDirection(magicer, gsprite.Coordinate);
					rotation = this.CalcRotation(magicer, gsprite.Coordinate);
				}
			}
			else if (!notChangeDirection)
			{
				num = this.CalcDirection(magicer, magicer.EnemyTarget);
				rotation = this.CalcRotation(magicer, magicer.EnemyTarget);
			}
			magicer.Direction = (int)num;
			magicer.Rotation = rotation;
			if (this.FakeZhanShiChongZhuang(magicCode))
			{
				return;
			}
			magicer.Action = GActions.Magic;
			GameInstance.Game.SpriteAction(num, 6, magicer.Coordinate, magicer.EnemyTarget, magicer.YAngle, new Point(0, 0));
		}

		private void MoveToCasting(GSprite sprite, Point p, int magicRange)
		{
			if (sprite.TargetSrpite != null)
			{
				GSprite gsprite = this.FindSprite(sprite.TargetSrpite);
				if (gsprite != null)
				{
					p = gsprite.Coordinate;
				}
			}
			sprite.IsMagicMove = true;
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			Point point = new Point
			{
				X = sprite.Coordinate.X / this.CurrentMapData.GridSizeX,
				Y = sprite.Coordinate.Y / this.CurrentMapData.GridSizeY
			};
			Random random = new Random();
			Point point2 = Global.GetExtensionPoint(p, sprite.Coordinate, magicRange - random.Next(0, 101));
			if (Global.OnObstruction(point2, this.CurrentMapData))
			{
				Point gridPoint = new Point
				{
					X = point2.X / this.CurrentMapData.GridSizeX,
					Y = point2.Y / this.CurrentMapData.GridSizeY
				};
				gridPoint = Global.GetAGridPointIn4Direction(gridPoint, this.CurrentMapData.fixedObstruction, this.CurrentMapData);
				point2 = new Point(gridPoint.X * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, gridPoint.Y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
			}
			Point point3 = new Point
			{
				X = point2.X / this.CurrentMapData.GridSizeX,
				Y = point2.Y / this.CurrentMapData.GridSizeY
			};
			if (point3.X == point.X && point3.Y == point.Y)
			{
				point2 = p;
			}
			this.LinearMove(sprite, point2, 2, 0, false);
		}

		private void MagicAttack(int magicCode, int magicRange, bool notChangeDirection = false)
		{
			if (this.CanSpellCasting(magicRange) || this.SelectedSprite == null)
			{
				GameInstance.Game.SpriteMagicCode(magicCode);
				this.SpellCasting(this.Leader, magicCode, notChangeDirection);
			}
			else
			{
				this.MoveToCasting(this.Leader, this.Leader.EnemyTarget, magicRange);
				this.SkillAttackWaitingID = magicCode;
			}
			this.DoAttackOK = true;
		}

		private void ActiveTreatment(GSprite sprite)
		{
			Global.SetOpacity(sprite, this.CurrentMapData);
			if (Global.WatchSprite != null)
			{
				GSprite gsprite = this.FindSprite(Global.WatchSprite);
				if (gsprite != null)
				{
					double num = Math.Sqrt(Math.Pow((double)(gsprite.Coordinate.X - sprite.Coordinate.X), 2.0) + Math.Pow((double)(gsprite.Coordinate.Y - sprite.Coordinate.Y), 2.0));
					if (num >= (double)Global.Data.MaxUnWatchDistance)
					{
						this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
						if (sprite.LockObject == Global.WatchSprite)
						{
							sprite.LockObject = null;
						}
					}
				}
				else if (sprite.LockObject == Global.WatchSprite)
				{
					sprite.LockObject = null;
				}
				if (Global.WatchSprite == null && this.LeftButtonClickOnSprite != null)
				{
					this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
					{
						RoleID = -1,
						SpriteType = GSpriteTypes.Leader,
						ShowDlg = false,
						ExtensionID = -1
					});
				}
			}
			if (sprite.LockObject != null)
			{
				GSprite gsprite2 = this.FindSprite(sprite.LockObject);
				if (gsprite2 != null)
				{
					if (gsprite2.VLife <= 0.0)
					{
						sprite.LockObject = null;
						if (sprite.Action != GActions.Magic)
						{
							int num2 = Global.StopStoryboard(this.Leader.Name, 0);
							if (num2 >= 0)
							{
								GameInstance.Game.SpriteStopMove(num2);
							}
						}
					}
				}
				else
				{
					this.Leader.LockObject = null;
				}
			}
		}

		public int DoAttack(bool ignoreSlot = false)
		{
			bool flag = this.Leader.CanAttack();
			if ((flag && this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic && this.Leader.Action != GActions.Bow && !this.Leader.IsMagicMove) || ignoreSlot)
			{
				Point mapCursorPoint = this.GetMapCursorPoint(this.CurDestinationPos);
				this.Leader.EnemyTarget = mapCursorPoint;
				double direction = (double)this.Leader.Direction;
				int yangle = this.Leader.YAngle;
				this.InstantAttack(direction, yangle, this.Leader.Coordinate, this.Leader.EnemyTarget, 1, true);
				return 0;
			}
			return -1;
		}

		public bool CanDoMagicAttackNow()
		{
			return this.Leader != null && ((this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic && this.Leader.Action != GActions.Bow && this.Leader.Action != GActions.PreAttack) || (this.Leader.Action == GActions.PreAttack && this.Leader.CanAttack()));
		}

		public void DoEmblem()
		{
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (Global.IsEmblemInCool())
			{
				return;
			}
			if (Global.IsInHorseRequestRideCD())
			{
				return;
			}
			GameInstance.Game.SendDoEmblem();
		}

		public void DoBianShen()
		{
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			GameInstance.Game.SendDoBianShen();
		}

		public int DoMagicAttack(int magicCode, Point enemyTarget, string enemyObject = null, bool forceMagic = false, bool hint = true)
		{
			if (Global.IsDongJieSprite(this.Leader))
			{
				return 0;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return 0;
			}
			if (magicCode <= 0)
			{
				return 0;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return 0;
			}
			if (!Global.CanMapUseMagic(this.CurrentMapData, magicCode))
			{
				return 0;
			}
			if (!forceMagic)
			{
				if ((this.Leader.Action == GActions.Attack || this.Leader.Action == GActions.Magic) && this.Leader.CurrentMagic > 0)
				{
					return -1;
				}
			}
			else if (Global.IsAutoFighting())
			{
				this.ExternalQueueSkillID = magicCode;
				return 0;
			}
			if (magicCode == this.ExternalQueueSkillID)
			{
				this.ExternalQueueSkillID = -1;
			}
			SkillData skillDataByID = Global.GetSkillDataByID(magicCode);
			if (skillDataByID == null)
			{
				return -2;
			}
			this.HideMouseLeftButtonUpEffect();
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillDataByID.SkillID);
			if (maigcInfoVOByCode == null)
			{
				return -3;
			}
			int skillLevel = (skillDataByID != null) ? skillDataByID.SkillLevel : 1;
			int num = Global.GetSkillUsedMagicV(Global.Data.roleData.Occupation, skillDataByID.SkillID, skillLevel);
			num = Global.GMax(0, num);
			if (num > 0 && Global.Data.roleData.MagicV - num < 0)
			{
				if (hint)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用[{0}]技能"), new object[]
					{
						ConfigMagicInfos.GetSkillNameByID(skillDataByID.SkillID)
					}), 0, -1, -1, 0);
				}
				return -5;
			}
			int skillType = maigcInfoVOByCode.SkillType;
			int skillAttackGridNum = this.GetSkillAttackGridNum(magicCode);
			int num2 = skillAttackGridNum * this.CurrentMapData.GridSizeX;
			if (enemyTarget.X == -1 || enemyTarget.Y == -1)
			{
				this.Leader.EnemyTarget = Global.GetExtensionPoint(this.Leader.Coordinate, (double)this.Leader.Direction * 45.0, (double)(num2 / 3 * 2));
			}
			else
			{
				this.Leader.EnemyTarget = enemyTarget;
			}
			GSprite gsprite = null;
			if (maigcInfoVOByCode.TargetPos == 1)
			{
				if (skillType == 101 || skillType == 106)
				{
					if (Global.WatchSprite != null)
					{
						gsprite = this.FindSprite(Global.WatchSprite);
						if (gsprite != null && !Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							gsprite = null;
						}
					}
					if (gsprite == null && Global.ViewSprite != null)
					{
						gsprite = this.FindSprite(Global.ViewSprite);
						if (Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
							{
								this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
							}
							Global.WatchSprite = gsprite.Name;
							this.SetLockDeco(gsprite, true);
						}
					}
				}
				else if (maigcInfoVOByCode.ActionType == 0)
				{
					Point attackPointByDir = this.GetAttackPointByDir(this.Leader.Coordinate, 1, this.Leader.Direction);
					this.Leader.EnemyTarget = attackPointByDir;
				}
			}
			else if (maigcInfoVOByCode.TargetPos == -1 || maigcInfoVOByCode.TargetPos == 2)
			{
				if (enemyObject != null)
				{
					gsprite = this.FindSprite(enemyObject);
				}
				else if (Global.WatchSprite != null)
				{
					gsprite = this.FindSprite(Global.WatchSprite);
					if (gsprite != null)
					{
						if (Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							if (this.Leader.LockObject == null)
							{
								this.Leader.LockObject = gsprite.Name;
							}
						}
						else
						{
							gsprite = null;
						}
					}
				}
				if (gsprite == null && this.SelectedSprite != null)
				{
					gsprite = this.SelectedSprite;
				}
			}
			else if (maigcInfoVOByCode.TargetPos == 3)
			{
				if (enemyObject != null)
				{
					gsprite = this.FindSprite(enemyObject);
					gsprite = this.JugeSpriteInAngle(gsprite, num2);
					if (gsprite != null)
					{
						if (Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
							{
								this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
							}
							Global.WatchSprite = gsprite.Name;
							this.SetLockDeco(gsprite, true);
						}
						else
						{
							gsprite = null;
						}
					}
				}
				if (gsprite == null)
				{
					GSprite gsprite2 = this.SelectedSprite;
					if (gsprite2 == null)
					{
						gsprite2 = this.GetLastSelectedSprite();
					}
					if (gsprite2 != null)
					{
						gsprite = gsprite2;
						if (Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
							{
								this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
							}
							Global.WatchSprite = gsprite.Name;
							this.SetLockDeco(gsprite, true);
						}
					}
					if (gsprite == null && Global.WatchSprite != null)
					{
						if (maigcInfoVOByCode.MagicType == 1)
						{
							gsprite = this.FindSprite(Global.WatchSprite);
							if (maigcInfoVOByCode.TargetType == 4)
							{
								this.SelectedSprite = gsprite;
							}
						}
						if (gsprite != null && !Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							gsprite = null;
						}
					}
					if (gsprite == null && Global.ViewSprite != null && maigcInfoVOByCode.MagicType == 1)
					{
						gsprite = this.FindSprite(Global.ViewSprite);
						if (Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
						{
							if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
							{
								this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
							}
							Global.WatchSprite = gsprite.Name;
							this.SetLockDeco(gsprite, true);
						}
						else
						{
							gsprite = null;
						}
					}
				}
			}
			this.Leader.TargetSrpite = ((gsprite == null) ? null : gsprite.Name);
			this.Leader.CurrentMagic = magicCode;
			if (maigcInfoVOByCode.ActionType == 0)
			{
				if (gsprite != null)
				{
					this.Leader.EnemyTarget = gsprite.Coordinate;
				}
			}
			else if (maigcInfoVOByCode.ActionType == 1)
			{
				if (skillType == 101 || skillType == 106)
				{
					if (gsprite != null)
					{
						this.Leader.EnemyTarget = gsprite.Coordinate;
					}
				}
				else if (maigcInfoVOByCode.MagicType == 1)
				{
					if (gsprite != null)
					{
						if (string.Empty != maigcInfoVOByCode.FlyDecoration)
						{
							this.Leader.EnemyTarget = new Point(gsprite.Coordinate.X, gsprite.Coordinate.Y);
						}
						else
						{
							this.Leader.EnemyTarget = gsprite.Coordinate;
						}
					}
				}
				else if (maigcInfoVOByCode.TargetPos == 2 && gsprite != null)
				{
					this.Leader.EnemyTarget = gsprite.Coordinate;
				}
				this.Leader.MagicID = magicCode;
				this.MagicAttack(magicCode, skillAttackGridNum * this.CurrentMapData.GridSizeX, true);
			}
			if (forceMagic)
			{
			}
			this.ClearTempWaitingSkillID(magicCode);
			if (this.GetAutoFightSkillID() == magicCode && this.Leader.Occupation == 2)
			{
				this.LastCallMonsterTicks = (double)Global.GetCorrectLocalTime();
			}
			return 0;
		}

		public bool MU_Attack(double direction, int yAngle, Point to, Point targetPos, int attackNum, int skillID, bool forceAttack = false, bool hint = true)
		{
			if (Global.IsDongJieSprite(this.Leader))
			{
				return false;
			}
			Global.RemoveStoryboard(this.Leader.Name);
			int num = 1;
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
			if (skillXmlNode != null)
			{
				num = skillXmlNode.SkillType;
			}
			if (num == 1)
			{
				Point moveTo = new Point(0, 0);
				this.Leader.Direction = (int)direction;
				this.Leader.Action = GActions.Attack;
				this.Leader.YAngle = yAngle;
				int magicRange = attackNum * this.CurrentMapData.GridSizeX - this.CurrentMapData.GridSizeX / 2;
				if (this.CanSpellCasting(magicRange) || this.SelectedSprite == null)
				{
					GameInstance.Game.SpriteAction(direction, 3, to, targetPos, yAngle, moveTo);
					this.DoAttackOK = true;
				}
				else
				{
					this.MoveToCasting(this.Leader, this.Leader.EnemyTarget, magicRange);
					this.SkillAttackWaitingID = skillID;
				}
			}
			else
			{
				this.Leader.Direction = (int)direction;
				this.Leader.YAngle = yAngle;
				this.DoMagicAttack(skillID, targetPos, this.Leader.LockObject, forceAttack, hint);
			}
			return true;
		}

		private bool CanPushSprites(int toGridX, int toGridY)
		{
			MapGrid mapGrid = this.CurrentMapData._MapGrid;
			List<object> list = mapGrid.FindObjects(toGridX, toGridY);
			if (list == null || list.Count <= 0)
			{
				return true;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is GSprite)
				{
					if ((list[i] as GSprite).SpriteType == GSpriteTypes.Other)
					{
						return false;
					}
					if ((list[i] as GSprite).SpriteType == GSpriteTypes.Monster && (list[i] as GSprite).MonsterType != MonsterTypes.Noraml)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CanMoveForward(out Point p)
		{
			p = new Point(0, 0);
			if (Global.CalcOriginalOccupationID(this.Leader.Occupation) != 0)
			{
				return false;
			}
			int directGetAttackNum = this.Leader.DirectGetAttackNum;
			List<Point> gridList = ChuanQiUtils.GetGridList(this.Leader.Coordinate, this.Leader.Direction, (directGetAttackNum < 3) ? 1 : 2, false, this.Leader);
			for (int i = 0; i < gridList.Count; i++)
			{
				if (!this.CanPushSprites(gridList[i].X, gridList[i].Y))
				{
					gridList.RemoveRange(i, gridList.Count - i);
					break;
				}
			}
			if (gridList.Count <= 0)
			{
				return false;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			gridList.Insert(0, new Point(this.Leader.cx / this.CurrentMapData.GridSizeX, this.Leader.cy / this.CurrentMapData.GridSizeY));
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < gridList.Count; j++)
			{
				stringBuilder.AppendFormat("{0}_{1}", gridList[j].X, gridList[j].Y);
				if (j < gridList.Count - 1)
				{
					stringBuilder.Append("|");
				}
			}
			double simpleMoveSpeedRate = 0.75;
			if (directGetAttackNum == 2)
			{
				simpleMoveSpeedRate = 0.75;
			}
			else if (directGetAttackNum == 3)
			{
				simpleMoveSpeedRate = 0.75;
			}
			int elapsedTicks = 0;
			if (directGetAttackNum == 1)
			{
				if (this.Leader.HasWeapons())
				{
					elapsedTicks = 0;
				}
				else
				{
					elapsedTicks = -250;
				}
			}
			p = new Point(this.CurrentMapData.GridSizeX * gridList[gridList.Count - 1].X + this.CurrentMapData.GridSizeX / 2, this.CurrentMapData.GridSizeY * gridList[gridList.Count - 1].Y + this.CurrentMapData.GridSizeY / 2);
			this.StartSimpleStoryboard(this.Leader, stringBuilder.ToString(), gridList[0].X, gridList[0].Y, gridList[gridList.Count - 1].X, gridList[gridList.Count - 1].Y, p, elapsedTicks, simpleMoveSpeedRate);
			return true;
		}

		private bool DoMoveForward(GSprite sprite, Point p)
		{
			if (Global.CalcOriginalOccupationID(sprite.Occupation) != 0)
			{
				return false;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point point = new Point
			{
				X = sprite.Coordinate.X / this.CurrentMapData.GridSizeX,
				Y = sprite.Coordinate.Y / this.CurrentMapData.GridSizeY
			};
			Point point2 = new Point
			{
				X = p.X / this.CurrentMapData.GridSizeX,
				Y = p.Y / this.CurrentMapData.GridSizeY
			};
			if (point == point2)
			{
				return false;
			}
			List<ANode> list = this.FindPath(sprite, point, point2);
			if (list == null || list.Count <= 0)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.AppendFormat("{0}_{1}", list[i].x, list[i].y);
				if (i < list.Count - 1)
				{
					stringBuilder.Append("|");
				}
			}
			double simpleMoveSpeedRate = 0.25;
			if (sprite.DirectGetAttackNum == 2)
			{
				simpleMoveSpeedRate = 0.2;
			}
			else if (sprite.DirectGetAttackNum == 3)
			{
				simpleMoveSpeedRate = 0.15;
			}
			p = new Point(this.CurrentMapData.GridSizeX * list[list.Count - 1].x + this.CurrentMapData.GridSizeX / 2, this.CurrentMapData.GridSizeY * list[list.Count - 1].y + this.CurrentMapData.GridSizeY / 2);
			this.StartSimpleStoryboard(this.Leader, stringBuilder.ToString(), list[0].x, list[0].y, list[list.Count - 1].x, list[list.Count - 1].y, p, -250, simpleMoveSpeedRate);
			return true;
		}

		private bool WillAttack(GSprite attacker, GSprite defenser, int num)
		{
			if (defenser == null)
			{
				return false;
			}
			if (num < 0)
			{
				return true;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num2 = defenser.cx / this.CurrentMapData.GridSizeX;
			int num3 = defenser.cy / this.CurrentMapData.GridSizeY;
			for (int i = 0; i < 8; i++)
			{
				List<Point> gridList = ChuanQiUtils.GetGridList(attacker.Coordinate, i, num, true, null);
				for (int j = 0; j < gridList.Count; j++)
				{
					if (num2 == gridList[j].X && num3 == gridList[j].Y)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool WillAttack2(GSprite attacker, GSprite defenser, int distance)
		{
			if (defenser == null)
			{
				return false;
			}
			if (distance < 0)
			{
				return true;
			}
			int num = (int)Global.GetTwoPointDistance(attacker.Coordinate, defenser.Coordinate);
			return num <= distance;
		}

		private Point GetAnti_AttackPoint(Point p, int num)
		{
			int num2 = this.CalcDirection(p, this.Leader.Coordinate);
			num2 = (num2 + 4) % 8;
			return this.GetAttackPointByDir(p, num, num2);
		}

		private Point GetAttackPoint(Point p, int num)
		{
			int dir = this.CalcDirection(p, this.Leader.Coordinate);
			return this.GetAttackPointByDir(p, num, dir);
		}

		private Point GetAttackPointByDir(Point p, int num, int dir)
		{
			List<Point> gridList = ChuanQiUtils.GetGridList(p, dir, num, true, null);
			if (gridList.Count <= 0)
			{
				return p;
			}
			int x = gridList[gridList.Count - 1].X;
			int y = gridList[gridList.Count - 1].Y;
			return new Point(x * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
		}

		private bool CanSpellCasting(int magicRange)
		{
			Point start = this.Leader.EnemyTarget;
			if (this.Leader.TargetSrpite != null)
			{
				GSprite gsprite = this.FindSprite(this.Leader.TargetSrpite);
				if (gsprite != null)
				{
					start = gsprite.Coordinate;
				}
			}
			return magicRange <= 0 || Global.GetTwoPointDistance(start, this.Leader.Coordinate) <= (double)magicRange;
		}

		private bool FakeZhanShiChongZhuang(int magicCode)
		{
			if (magicCode == 15)
			{
				if (ConfigMagicInfos.GetSkillQueueTicks(magicCode) <= 0L)
				{
					Global.AddSkillCoolDown(magicCode, true, -1L);
				}
				GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
				Global.DoInjure(this.Leader, null, this.Leader.EnemyTarget, magicCode);
				return true;
			}
			return false;
		}

		public bool CancelLockObject(bool cancelWatch = false)
		{
			bool result = false;
			if (Global.WatchSprite != null)
			{
				GSprite gsprite = this.FindSprite(Global.WatchSprite);
				if (gsprite != null)
				{
					result = true;
					this.SetLockDeco(gsprite, false);
				}
			}
			this.Leader.LockObject = null;
			if (cancelWatch)
			{
				Global.WatchSprite = null;
			}
			if (this.LeftButtonClickOnSprite != null)
			{
				this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
				{
					RoleID = -1,
					SpriteType = GSpriteTypes.Leader,
					ShowDlg = false,
					ExtensionID = -1
				});
			}
			return result;
		}

		private bool JugeSafeRegion()
		{
			bool flag = this.InSafeRegion();
			if (flag)
			{
				if (!this.IsInSafeRegion || this.FirstJugeSafeRegion)
				{
					this.FirstJugeSafeRegion = false;
					this.IsInSafeRegion = flag;
					Global.IsInSafeRegion = flag;
					if (this.Leader != null)
					{
						this.Leader.ChangeWeaponsPosition(this.IsInSafeRegion);
					}
					if (this.SafeRegionNotfiy != null)
					{
						this.SafeRegionNotfiy(this.IsInSafeRegion);
					}
					return true;
				}
				return false;
			}
			else
			{
				if (this.IsInSafeRegion || this.FirstJugeSafeRegion)
				{
					this.FirstJugeSafeRegion = false;
					this.IsInSafeRegion = flag;
					Global.IsInSafeRegion = flag;
					if (this.Leader != null)
					{
						this.Leader.ChangeWeaponsPosition(this.IsInSafeRegion);
					}
					if (this.SafeRegionNotfiy != null)
					{
						this.SafeRegionNotfiy(this.IsInSafeRegion);
					}
					return true;
				}
				return false;
			}
		}

		private Point GetLeaderCenterPoint(Point coordinate)
		{
			if (this.LeaderCenterPoint.X == -1 || this.LeaderCenterPoint.Y == -1)
			{
				this.LeaderCenterPoint = coordinate;
				return coordinate;
			}
			double num = (double)this.LeaderCenterPoint.X;
			double num2 = (double)this.LeaderCenterPoint.Y;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			if ((double)Math.Abs(coordinate.X - this.LeaderCenterPoint.X) > num5)
			{
				if (coordinate.X - this.LeaderCenterPoint.X > 0)
				{
					num3 = (double)(coordinate.X - this.LeaderCenterPoint.X) - num5;
				}
				else
				{
					num3 = (double)(coordinate.X - this.LeaderCenterPoint.X) + num5;
				}
			}
			if ((double)Math.Abs(coordinate.Y - this.LeaderCenterPoint.Y) > num6)
			{
				if (coordinate.Y - this.LeaderCenterPoint.Y > 0)
				{
					num4 = (double)(coordinate.Y - this.LeaderCenterPoint.Y) - num6;
				}
				else
				{
					num4 = (double)(coordinate.Y - this.LeaderCenterPoint.Y) + num6;
				}
			}
			this.LeaderCenterPoint = new Point((int)(num + num3), (int)(num2 + num4));
			return this.LeaderCenterPoint;
		}

		private bool DoShiftAttack(bool forceMove = false)
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if ((double)correctLocalTime - this.LastShiftTicks >= 5.0)
			{
				this.LastShiftTicks = (double)correctLocalTime;
				if (Global.WatchSprite != null && (this.Leader.LockObject == null || Global.WatchSprite != this.Leader.LockObject || (this.Leader.LockObject != null && this.Leader.Action == GActions.Stand)))
				{
					GSprite gsprite = this.FindSprite(Global.WatchSprite);
					if (gsprite != null && gsprite.VLife > 0.0)
					{
						this.Leader.LockObject = Global.WatchSprite;
						int skillAttackGridNum = this.GetSkillAttackGridNum(-1);
						if (!this.WillAttack(this.Leader, gsprite, skillAttackGridNum))
						{
							Point attackPoint = this.GetAttackPoint(gsprite.Coordinate, skillAttackGridNum);
							if (forceMove)
							{
								if (Global.FindStoryboard(gsprite.Name) != null)
								{
									this.LinearMove(this.Leader, this.GetAnti_AttackPoint(gsprite.Coordinate, 1), 2, 0, false);
								}
								else
								{
									this.LinearMove(this.Leader, attackPoint, 2, 0, false);
								}
							}
							else if (this.Leader.Action != GActions.Run && this.Leader.Action != GActions.Walk && this.Leader.CanMove())
							{
								if (Global.FindStoryboard(gsprite.Name) != null)
								{
									this.LinearMove(this.Leader, this.GetAnti_AttackPoint(gsprite.Coordinate, 1), 2, 0, false);
								}
								else
								{
									this.LinearMove(this.Leader, attackPoint, 2, 0, false);
								}
							}
						}
						else if (this.Leader.CanAttack() && this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic && this.Leader.Action != GActions.Bow && !this.Leader.IsMagicMove)
						{
							double direction = this.CalcDirection(this.Leader, gsprite.Coordinate);
							this.Leader.EnemyTarget = gsprite.Coordinate;
							this.Leader.Rotation = this.CalcRotation(this.Leader, gsprite.Coordinate);
							this.InstantAttack(direction, this.Leader.YAngle, this.Leader.Coordinate, this.Leader.EnemyTarget, skillAttackGridNum, false);
							return true;
						}
					}
					else
					{
						this.Leader.LockObject = null;
					}
				}
			}
			return false;
		}

		private void TraceEnemy()
		{
			if (this.Leader == null)
			{
				return;
			}
			if (this.Leader.Action == GActions.Magic)
			{
				return;
			}
			if (Global.Data.roleData.LifeV <= 0 && null == PlayZone.GlobalPlayZone.m_FuhuoPanle)
			{
				if (this.Leader.Action != GActions.Death || null == PlayZone.GlobalPlayZone.m_FuhuoPanle)
				{
					if (null == PlayZone.GlobalPlayZone.m_FuhuoPanle)
					{
					}
					Global.RemoveStoryboard(this.Leader.Name);
					this.Leader.ExternalDeath();
				}
				return;
			}
			if (this.ShiftIsPressed && this.DoShiftAttack(false))
			{
				return;
			}
			if (this.Leader.LockObject != null && Global.IsAutoFighting())
			{
				GSprite gsprite = this.FindSprite(this.Leader.LockObject);
				if (gsprite != null && gsprite.VLife > 0.0)
				{
					int skillAttackGridNum = this.GetSkillAttackGridNum(-1);
					int skillAttackDistance = this.GetSkillAttackDistance(-1);
					if (!this.WillAttack2(this.Leader, gsprite, skillAttackDistance))
					{
						if (Global.Data.AutoFightData.FightRadius <= 8)
						{
							this.Leader.LockObject = null;
							return;
						}
						if (Global.FindStoryboard(this.Leader.Name) == null)
						{
							int num = (int)Global.GetDirectionByTan((double)this.Leader.Coordinate.X, (double)this.Leader.Coordinate.Y, (double)gsprite.Coordinate.X, (double)gsprite.Coordinate.Y);
							Point extensionPoint = Global.GetExtensionPoint(gsprite.Coordinate, (double)num * 45.0, (double)skillAttackDistance * 0.5);
							if (Global.OnObstruction(extensionPoint, this.CurrentMapData))
							{
								if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
								{
									return;
								}
								Point gridPoint = new Point
								{
									X = gsprite.Coordinate.X / this.CurrentMapData.GridSizeX,
									Y = gsprite.Coordinate.Y / this.CurrentMapData.GridSizeY
								};
								gridPoint = Global.GetAGridPointIn4Direction(gridPoint, this.CurrentMapData.fixedObstruction, this.CurrentMapData);
								extensionPoint = new Point(gridPoint.X * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, gridPoint.Y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
							}
							if (extensionPoint.X >= 20000 || extensionPoint.Y >= 20000)
							{
							}
							this.LinearMove(this.Leader, extensionPoint, 2, 0, false);
						}
					}
					else if ((this.Leader.Action == GActions.Attack || this.Leader.Action == GActions.Magic || this.Leader.Action == GActions.Bow) && this.Leader.CanMove())
					{
						double num2 = this.CalcDirection(this.Leader, gsprite.Coordinate);
						num2 = this.CalcDirection(this.Leader, gsprite.Coordinate);
						this.Leader.EnemyTarget = gsprite.Coordinate;
						this.ExternalSkillAttack(this.GetAutoUseSkillID(false), true);
					}
					else if (this.Leader.Action == GActions.Stand)
					{
						double num3 = this.CalcDirection(this.Leader, gsprite.Coordinate);
						this.Leader.EnemyTarget = gsprite.Coordinate;
						this.ExternalSkillAttack(this.GetAutoUseSkillID(false), true);
					}
					else if (this.Leader.Action == GActions.PreAttack)
					{
					}
				}
				else
				{
					this.Leader.LockObject = null;
				}
			}
		}

		public int GetAutoFightTargetMonsterID()
		{
			return this.AutoFightTargetMonsterID;
		}

		public void AddAutoFightDeco()
		{
			if (!Global.IsAutoFighting())
			{
				return;
			}
			if (this.AutoFightDecoNotify != null)
			{
				this.AutoFightDecoNotify(this, new StateNotifyEventArgs
				{
					state = 0
				});
			}
		}

		private void RemoveAutoFightDeco()
		{
			if (this.AutoFightDecoNotify != null)
			{
				this.AutoFightDecoNotify(this, new StateNotifyEventArgs
				{
					state = 1
				});
			}
		}

		public GSprite SeekAttackingMeMonsterToLock(int targetMonsterID = 0)
		{
			if (this.AttackingMeMonsterID <= 0)
			{
				return null;
			}
			GSprite gsprite = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
			{
				this.AttackingMeMonsterID
			}));
			if (gsprite == null)
			{
				this.AttackingMeMonsterID = -1;
				return null;
			}
			if (this.InSafeRegion(gsprite))
			{
				this.AttackingMeMonsterID = -1;
				this.AutoFightTargetMonsterID = -1;
				return null;
			}
			if (targetMonsterID != -1 && gsprite.ExtensionID != targetMonsterID)
			{
				return null;
			}
			if (gsprite.VLife <= 0.0)
			{
				return null;
			}
			if (!Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
			{
				return null;
			}
			if (!this.CanAttackTargetByOption(true, gsprite, null))
			{
				return null;
			}
			if (gsprite.SpriteType == GSpriteTypes.Monster)
			{
				MonsterData value = Global.Data.SystemMonsters.GetValue(gsprite.RoleID);
				if (value != null && value.MonsterType == 1201)
				{
					this.AttackingMeMonsterID = -1;
					return null;
				}
			}
			else if (gsprite.SpriteType == GSpriteTypes.Other && !Global.Data.AutoFightData.AutoAntiAttack)
			{
				return null;
			}
			return gsprite;
		}

		private bool CanAttackTargetByOption(bool isAutoFighting, GSprite sprite, MonsterData monsterData = null)
		{
			return (Global.IsOpposition(this.Leader, sprite, false, this.CurrentMapData.PKMode) || monsterData == null || monsterData.MonsterType == 1101) && (!isAutoFighting || !Global.Data.AutoFightData.DontAttackBigBoss || sprite.MonsterType <= MonsterTypes.Noraml || sprite.MonsterType >= MonsterTypes.DaDao);
		}

		public GSprite SeekMonsterToLock(bool isAutoFighting = true, string notFindName = null, int targetMonsterID = -1)
		{
			GSprite result = null;
			if (this.Leader.VLife <= 0.0)
			{
				return result;
			}
			int fightRadius = Global.Data.AutoFightData.FightRadius;
			int num = int.MaxValue;
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if ((gsprite.SpriteType == GSpriteTypes.Monster || gsprite.SpriteType == GSpriteTypes.JunQi) && gsprite.CurrentObjectState)
					{
						if (!this.InSafeRegion(gsprite))
						{
							if (gsprite.SpriteType == GSpriteTypes.Monster)
							{
								MonsterData value = Global.Data.SystemMonsters.GetValue(gsprite.RoleID);
								if (value == null)
								{
									goto IL_189;
								}
								if (value.MonsterType == 1201)
								{
									goto IL_189;
								}
								if (value.MasterRoleID > 0)
								{
									goto IL_189;
								}
								if (targetMonsterID != -1 && targetMonsterID != gsprite.ExtensionID)
								{
									goto IL_189;
								}
								if (!this.CanAttackTargetByOption(isAutoFighting, gsprite, value))
								{
									goto IL_189;
								}
							}
							if (notFindName == null || !(notFindName == gsprite.Name))
							{
								if (Global.InCircleByGridNum(gsprite.Coordinate, this.Leader.Coordinate, (double)fightRadius))
								{
									int num2 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, gsprite.Coordinate);
									if (num2 < num)
									{
										num = num2;
										result = gsprite;
									}
								}
							}
						}
					}
				}
				IL_189:;
			}
			return result;
		}

		public GGoodsPack SeekGoodsPackToPickUp(bool isAutoFighting = true)
		{
			GGoodsPack result = null;
			if (this.Leader.VLife <= 0.0)
			{
				return result;
			}
			int fightRadius = Global.Data.AutoFightData.FightRadius;
			int num = int.MaxValue;
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GGoodsPack)
				{
					GGoodsPack ggoodsPack = objectsList[i] as GGoodsPack;
					if (Global.CanOpenGoodsPack(ggoodsPack))
					{
						if (!isAutoFighting || Global.CanAutoGetThing(ggoodsPack))
						{
							if (Global.InCircleByGridNum(ggoodsPack.Coordinate, this.Leader.Coordinate, (double)fightRadius))
							{
								int num2 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, ggoodsPack.Coordinate);
								if (num2 < num)
								{
									num = num2;
									result = ggoodsPack;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public void StartAutoFight(int targetMonsterID = -1)
		{
			if (!SystemHelpMgr.CanAutoFight())
			{
				return;
			}
			if (this.Leader == null)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.OnLeaderPreMove())
			{
				return;
			}
			if (Global.IsDongJieSprite(this.Leader))
			{
				return;
			}
			if (this.IsSittingNow())
			{
				this.StandNow();
			}
			if (Global.IsRunning)
			{
				return;
			}
			if (this.Leader.IsInSafeRegion)
			{
				Super.HintMainText(Global.GetLang("安全区不能使用挂机功能！"), 10, 3);
				return;
			}
			this.CancelAutoFindRoad(true);
			if (!Global.IsAutoFighting())
			{
				this.HasHintMagicNotEnoughSkillID = -1;
				this.NoHintNoDrugGoBack = false;
				this.AutoFightTargetMonsterID = targetMonsterID;
				this.LastFindMonsterCmdTicks = 0L;
				this.CurrentMapMonsterNum = int.MaxValue;
				this.CurrentFintMonsterSlot = 1;
				this.AddAutoFightDeco();
				int autoGetThingsFlag = Global.GetAutoGetThingsFlag(Global.Data.AutoFightData);
				Global.Data.AutoFightData.FightStartTicks = Global.GetCorrectLocalTime();
				Global.Data.AutoFightData.FightPoint = new Point(this.Leader.Coordinate.X, this.Leader.Coordinate.Y);
				Global.Data.AutoFightData.IsGoingBackOnOutOfRange = false;
				GameInstance.Game.SpriteAutoFight(1, autoGetThingsFlag);
			}
		}

		public bool CanCancelAutoFight()
		{
			if (!Global.IsAutoFighting())
			{
				return true;
			}
			Global.Data.AutoFightData.CancelFightingNum++;
			return Global.Data.AutoFightData.CancelFightingNum >= 2;
		}

		public void CancelAutoFight(int endType = 0, bool sendStandCmd = true)
		{
			if (!Global.IsAutoFighting())
			{
				return;
			}
			Global.Data.AutoFightData.Fighting = false;
			Global.Data.AutoFightData.CancelFightingNum = 0;
			this.Leader.LockObject = null;
			this.ExternalQueueSkillID = -1;
			this.RemoveAutoFightDeco();
			GameInstance.Game.SpriteAutoFight(3, endType);
			if (Global.FindMoveStroyboard(this.Leader.Name))
			{
				int num = Global.StopStoryboard(this.Leader.Name, 0);
				if (num >= 0)
				{
					GameInstance.Game.SpriteStopMove(num);
				}
			}
		}

		public void SetNoHintNoDrugGoBack()
		{
			this.NoHintNoDrugGoBack = true;
		}

		private void AutoFightManager()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (!Global.IsAutoFighting())
			{
				return;
			}
			this.AutoFightWithMonsters();
			this.AutoFightOptions();
		}

		private void AutoFightWithMonsters()
		{
			if (Global.Data.AutoFightData.IsGoingBackOnOutOfRange)
			{
				return;
			}
			if (this.Leader.LockObject == null)
			{
				GSprite gsprite = this.SeekAttackingMeMonsterToLock(this.AutoFightTargetMonsterID);
				if (gsprite == null || !Global.InCircleByGridNum(gsprite.Coordinate, Global.Data.AutoFightData.FightPoint, (double)Global.Data.AutoFightData.FightRadius))
				{
					gsprite = this.SeekMonsterToLock(true, null, this.AutoFightTargetMonsterID);
				}
				if (gsprite != null)
				{
					if (Global.WatchSprite != null)
					{
						this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
					}
					this.Leader.LockObject = gsprite.Name;
					Global.WatchSprite = gsprite.Name;
					this.SetLockDeco(gsprite, true);
					bool flag = true;
					if (gsprite.SpriteType == GSpriteTypes.Monster && gsprite.MonsterType == MonsterTypes.CaiJi)
					{
						if (Global.InCircle(this.Leader.Coordinate, gsprite.Coordinate, 100.0))
						{
							if (this.LeftButtonClickOnSprite != null)
							{
								this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
								{
									RoleID = gsprite.RoleID,
									SpriteType = gsprite.SpriteType,
									ShowDlg = false,
									ExtensionID = gsprite.ExtensionID
								});
							}
						}
						else
						{
							Global.Data.TargetNpcID = gsprite.RoleID - 2130706432;
							this.LinearMoveByRunTo(gsprite.Coordinate, 60, 4, false, 2);
							flag = false;
						}
					}
					if (flag && this.LeftButtonClickOnSprite != null)
					{
						this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
						{
							RoleID = gsprite.RoleID,
							SpriteType = gsprite.SpriteType,
							ShowDlg = false,
							ExtensionID = gsprite.ExtensionID
						});
					}
				}
				else
				{
					long correctLocalTime = Global.GetCorrectLocalTime();
					if (correctLocalTime - this.LastFindMonsterCmdTicks >= (long)(this.CurrentFintMonsterSlot * 5 * 1000) && this.Leader.Action == GActions.Stand)
					{
						this.AutoFightTargetMonsterID = -1;
						this.LastFindMonsterCmdTicks = correctLocalTime;
						GameInstance.Game.SpriteFindMonsterPosition(Global.Data.AutoFightData.FightPoint.X, Global.Data.AutoFightData.FightPoint.Y, Global.Data.AutoFightData.FightRadius);
					}
				}
			}
			else
			{
				GSprite gsprite = Global.FindSprite(this.Leader.LockObject);
				if (gsprite != null && (this.InSafeRegion(gsprite) || !Global.InCircleByGridNum(gsprite.Coordinate, Global.Data.AutoFightData.FightPoint, (double)Global.Data.AutoFightData.FightRadius) || !this.CanAttackTargetByOption(true, gsprite, null)))
				{
					this.Leader.LockObject = null;
					return;
				}
			}
		}

		private void InitAutoFightGoodsIDs()
		{
			if (this.DoubleExpGoodsIDs == null)
			{
				this.DoubleExpGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("DoubleExpGoodsIDs", ',');
			}
			if (this.LifeReserveGoodsIDs == null)
			{
				this.LifeReserveGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("LifeReserveGoodsIDs", ',');
			}
			if (this.MagicReserverGoodsIDs == null)
			{
				this.MagicReserverGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("MagicReserveGoodsIDs", ',');
			}
		}

		private void AutoFightUseGoods(int[] goodsIDArray)
		{
			if (goodsIDArray == null)
			{
				return;
			}
			for (int i = 0; i < goodsIDArray.Length; i++)
			{
				if (goodsIDArray[i] > 0)
				{
					if (Global.GetTotalGoodsCountByID(goodsIDArray[i]) > 0)
					{
						if (!Global.GoodsCoolDown(goodsIDArray[i]))
						{
							if (!Global.IsGoodsRelateBufferExist(goodsIDArray[i]))
							{
								if (Global.UsingGoodsByID(goodsIDArray[i]))
								{
									break;
								}
							}
						}
					}
				}
			}
		}

		private void AutoFightOptions()
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - Global.Data.AutoFightData.FightStartTicks >= (long)(Global.Data.AutoFightData.MaxFightSecs * 1000))
			{
				int endType = (!Global.Data.AutoFightData.AutoGoBack) ? 0 : 1;
				if (Global.GetMapType(this.MapCode) != MapTypes.Normal)
				{
					endType = 0;
				}
				this.CancelAutoFight(endType, true);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("自动战斗时间结束！"), 0, -1, -1, 0);
				return;
			}
			this.InitAutoFightGoodsIDs();
			if (correctLocalTime - Global.Data.AutoFightData.LastCheckBufferTicks >= 10000L)
			{
				Global.Data.AutoFightData.LastCheckBufferTicks = correctLocalTime;
				if (Global.Data.AutoFightData.AutoUseExpCard)
				{
					this.AutoFightUseGoods(this.DoubleExpGoodsIDs);
				}
				if (Global.Data.AutoFightData.AutoUseLifeReserveDrugs)
				{
					this.AutoFightUseGoods(this.LifeReserveGoodsIDs);
				}
				if (Global.Data.AutoFightData.AutoUseMagicReserveDrugs)
				{
					this.AutoFightUseGoods(this.MagicReserverGoodsIDs);
				}
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			if (Math.Abs(this.Leader.Coordinate.X - Global.Data.AutoFightData.FightPoint.X) / this.CurrentMapData.GridSizeX > Global.Data.AutoFightData.FightRadius || Math.Abs(this.Leader.Coordinate.Y - Global.Data.AutoFightData.FightPoint.Y) / this.CurrentMapData.GridSizeY > Global.Data.AutoFightData.FightRadius)
			{
				if (!Global.Data.AutoFightData.IsGoingBackOnOutOfRange)
				{
					this.Leader.LockObject = null;
					this.LinearMoveByRunTo(Global.Data.AutoFightData.FightPoint, 0, 0, false, 2);
					Global.Data.AutoFightData.IsGoingBackOnOutOfRange = true;
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("超出自动战斗范围"), 0, -1, -1, 0);
				}
			}
			else
			{
				Global.Data.AutoFightData.IsGoingBackOnOutOfRange = false;
			}
		}

		public void SetDefaultSkillID(int skillID)
		{
			if (skillID == Global.Data.roleData.DefaultSkillID)
			{
				return;
			}
			Global.Data.roleData.DefaultSkillID = skillID;
			GameInstance.Game.SpriteModDefSkillID(skillID);
		}

		public int GetDefaultSkillID()
		{
			if (Global.Data.roleData.DefaultSkillID <= 0)
			{
				return Global.Data.roleData.DefaultSkillID;
			}
			return Global.Data.roleData.DefaultSkillID;
		}

		private bool SkillNeedMagicVOk(int skillID)
		{
			SkillData skillDataByID = Global.GetSkillDataByID(skillID);
			if (skillDataByID == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"服务端给的数据没有这个技能！！" + skillID
				});
				return false;
			}
			int skillLevel = (skillDataByID != null) ? skillDataByID.SkillLevel : 1;
			int num = Global.GetSkillUsedMagicV(Global.Data.roleData.Occupation, skillID, skillLevel);
			num = Global.GMax(0, num);
			return Global.Data.roleData.MagicV * 100 >= num * Global.Data.roleData.MaxMagicV;
		}

		public void ClearTempWaitingSkillID(int skillID)
		{
			if (Global.Data.TempWaitingSkillID != skillID)
			{
				return;
			}
			long skillQueueTicks = ConfigMagicInfos.GetSkillQueueTicks(skillID);
			if (skillQueueTicks <= 0L)
			{
				return;
			}
			Global.Data.TempWaitingSkillID = -1;
			Global.Data.TempWaitingSkillIDTicks = 0L;
		}

		public void SetTempWaitingSkillID(int skillID)
		{
			long skillQueueTicks = ConfigMagicInfos.GetSkillQueueTicks(skillID);
			if (skillQueueTicks <= 0L)
			{
				return;
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("召唤[{0}]成功"), new object[]
			{
				ConfigMagicInfos.GetSkillNameByID(skillID)
			}), 0, -1, -1, 0);
			Global.AddSkillCoolDown(skillID, false, -1L);
			Global.Data.TempWaitingSkillID = skillID;
			Global.Data.TempWaitingSkillIDTicks = Global.GetCorrectLocalTime();
		}

		public int GetTempWaitingSkillID()
		{
			if (Global.Data.TempWaitingSkillID <= 0)
			{
				return 0;
			}
			long skillQueueTicks = ConfigMagicInfos.GetSkillQueueTicks(Global.Data.TempWaitingSkillID);
			if (Global.GetCorrectLocalTime() < Global.Data.TempWaitingSkillIDTicks + skillQueueTicks)
			{
				return Global.Data.TempWaitingSkillID;
			}
			return 0;
		}

		private int GetAutoFightSkillID()
		{
			if (!Global.IsAutoFighting())
			{
				return -1;
			}
			int num = -1;
			if (Global.Data.AutoFightData.AutoZhaohuanShenshou)
			{
				int num2 = Global.CalcOriginalOccupationID(this.Leader.Occupation);
				if (Global.BuffMagicDict == null)
				{
					Global.BuffMagicDict = ConfigSystemParam.GetSystemParamIntDictByName("BuffMagic", '|', ',');
				}
				int[] array = null;
				if (Global.BuffMagicDict.TryGetValue(num2, ref array))
				{
					for (int i = 0; i < array.Length - 1; i++)
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(array[i + 1]);
						if (bufferDataByID == null || Global.IsBufferDataOver(bufferDataByID, 0L, false))
						{
							num = array[i];
						}
					}
				}
				if (num > 0 && Global.GetSkillDataByID(num) != null)
				{
					return num;
				}
			}
			int occu = Global.CalcOriginalOccupationID(this.Leader.Occupation);
			int num3 = Global.GetCallMagicByOccupation(occu)[0];
			if (Global.Data.IsZhaoHuanShouLiving == 0 && num3 > 0 && this.Leader.WeaponState != WeaponStates.K)
			{
				return num3;
			}
			if (Global.isOpenSkillPriority)
			{
				return this.GetAutoSkillPriorityID();
			}
			num = Global.GetDefaultSkillID(this.Leader.Occupation, -1);
			if (num > 0)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(num);
				int num4 = 0;
				if (skillXmlNode != null)
				{
					num4 = skillXmlNode.UseType;
				}
				if (this.Leader.WeaponState == WeaponStates.K && num4 == 1)
				{
					this.Leader.ReCalcWeaponState();
					if (this.Leader.WeaponState == WeaponStates.K)
					{
						string skillNameByID = ConfigMagicInfos.GetSkillNameByID(num);
						Super.HintMainText(StringUtil.substitute(Global.GetLang("未佩戴武器, 无法使用技能【{0}】"), new object[]
						{
							skillNameByID
						}), 10, 3);
						num = -1;
					}
				}
				else if (!Global.CanMapUseMagic(this.CurrentMapData, num))
				{
					string skillNameByID2 = ConfigMagicInfos.GetSkillNameByID(num);
					Super.HintMainText(StringUtil.substitute(Global.GetLang("地图限制, 禁止使用技能【{0}】"), new object[]
					{
						skillNameByID2
					}), 10, 3);
					num = -1;
				}
				else if (Global.SkillCoolDown(num))
				{
					num = -1;
				}
				else if (Global.Data.roleData.Occupation == 3 && num != -1)
				{
					num = -1;
				}
				else if (!this.SkillNeedMagicVOk(num))
				{
					string skillNameByID3 = ConfigMagicInfos.GetSkillNameByID(num);
					if (this.HasHintMagicNotEnoughSkillID != num)
					{
						this.HasHintMagicNotEnoughSkillID = num;
						Super.HintMainText(StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用技能【{0}】"), new object[]
						{
							skillNameByID3
						}), 10, 3);
					}
					num = -1;
				}
				else if (num == this.HasHintMagicNotEnoughSkillID)
				{
					this.HasHintMagicNotEnoughSkillID = -1;
				}
			}
			if (num <= 0)
			{
				int defaultSkillID = Global.GetDefaultSkillID(this.Leader.Occupation, -1);
				num = Global.GetBaseSkillID(this.Leader.Occupation);
				if (Global.Data.roleData.SkillDataList != null)
				{
					List<int> list = new List<int>();
					for (int j = 0; j < Global.Data.roleData.SkillDataList.Count; j++)
					{
						if (Global.Data.roleData.SkillDataList[j].SkillID != num)
						{
							if (Global.Data.roleData.Occupation == 3 || Global.Data.roleData.SkillDataList[j].SkillID != defaultSkillID)
							{
								if (Global.IsAutoFightSkill(Global.Data.roleData.SkillDataList[j].SkillID))
								{
									if (Global.Data.roleData.SkillDataList[j].SkillID != num3)
									{
										MagicInfoVO skillXmlNode2 = Global.GetSkillXmlNode(Global.Data.roleData.SkillDataList[j].SkillID);
										int num5 = 0;
										if (skillXmlNode2 != null)
										{
											num5 = skillXmlNode2.UseType;
										}
										if (Global.Data.roleData.Occupation == 3 && skillXmlNode2 != null)
										{
											if (Global.GetMJSType() == MJSSkillType.Strength_Sword)
											{
												int damageType = skillXmlNode2.DamageType;
												if ((1 & damageType) == 0 && damageType != -1)
												{
													goto IL_4C8;
												}
											}
											else
											{
												int damageType2 = skillXmlNode2.DamageType;
												if ((2 & damageType2) == 0 && damageType2 != -1)
												{
													goto IL_4C8;
												}
											}
										}
										if (this.Leader.WeaponState != WeaponStates.K || num5 != 1)
										{
											if (Global.CanMapUseMagic(this.CurrentMapData, Global.Data.roleData.SkillDataList[j].SkillID))
											{
												if (!Global.SkillCoolDown(Global.Data.roleData.SkillDataList[j].SkillID))
												{
													if (this.SkillNeedMagicVOk(num))
													{
														list.Add(Global.Data.roleData.SkillDataList[j].SkillID);
													}
												}
											}
										}
									}
								}
							}
						}
						IL_4C8:;
					}
					if (list.Count > 0)
					{
						num = list[Global.GetRandomNumber(0, list.Count)];
					}
				}
			}
			return num;
		}

		private int GetAutoSkillPriorityID()
		{
			int num = Global.GetBaseSkillID(this.Leader.Occupation);
			if (Global.Data.roleData.SkillDataList != null)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < Global.Data.roleData.SkillDataList.Count; i++)
				{
					if (Global.Data.roleData.SkillDataList[i].SkillID != num)
					{
						if (Global.IsAutoSkillPriority(Global.Data.roleData.SkillDataList[i].SkillID))
						{
							MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(Global.Data.roleData.SkillDataList[i].SkillID);
							int num2 = 0;
							if (skillXmlNode != null)
							{
								num2 = skillXmlNode.UseType;
							}
							if (Global.Data.roleData.Occupation == 3 && skillXmlNode != null)
							{
								if (Global.GetMJSType() == MJSSkillType.Strength_Sword)
								{
									int damageType = skillXmlNode.DamageType;
									if ((1 & damageType) == 0 && damageType != -1)
									{
										goto IL_1BD;
									}
								}
								else
								{
									int damageType2 = skillXmlNode.DamageType;
									if ((2 & damageType2) == 0 && damageType2 != -1)
									{
										goto IL_1BD;
									}
								}
							}
							if (this.Leader.WeaponState != WeaponStates.K || num2 != 1)
							{
								if (Global.CanMapUseMagic(this.CurrentMapData, Global.Data.roleData.SkillDataList[i].SkillID))
								{
									if (!Global.SkillCoolDown(Global.Data.roleData.SkillDataList[i].SkillID))
									{
										if (this.SkillNeedMagicVOk(num))
										{
											list.Add(Global.Data.roleData.SkillDataList[i].SkillID);
										}
									}
								}
							}
						}
					}
					IL_1BD:;
				}
				if (list.Count <= 0)
				{
					num = Global.GetBaseSkillID(this.Leader.Occupation);
				}
				else
				{
					num = this.GetSkillPriorityID(list);
				}
			}
			return num;
		}

		private int GetSkillPriorityID(List<int> tmpSkillList)
		{
			this.skillIDIndexs.Clear();
			List<int> list = Enumerable.ToList<int>(Global.GetSkillPriorityDict().Keys);
			List<int> list2 = Enumerable.ToList<int>(Global.GetSkillPriorityDict().Values);
			for (int i = 0; i < tmpSkillList.Count; i++)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j] == tmpSkillList[i])
					{
						this.skillIDIndexs.Add(list[j]);
					}
				}
			}
			this.skillIDIndexs.Sort();
			int result;
			if (Global.GetSkillPriorityDict().Count > 0)
			{
				result = Global.GetSkillPriorityDict()[this.skillIDIndexs[0]];
			}
			else
			{
				result = Global.GetBaseSkillID(this.Leader.Occupation);
			}
			return result;
		}

		private bool CanUseAutoFightSkillID()
		{
			return true;
		}

		public int GetAutoUseSkillID(bool shiftPressed = false)
		{
			int tempWaitingSkillID = this.GetTempWaitingSkillID();
			if (tempWaitingSkillID > 0 && Global.CanMapUseMagic(this.CurrentMapData, tempWaitingSkillID) && Global.SkillCoolDown(tempWaitingSkillID))
			{
				if (this.SkillNeedMagicVOk(tempWaitingSkillID))
				{
					return tempWaitingSkillID;
				}
				if (Global.GetSkillDataByID(tempWaitingSkillID) != null)
				{
					string skillNameByID = ConfigMagicInfos.GetSkillNameByID(tempWaitingSkillID);
					Super.HintMainText(StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用技能【{0}】"), new object[]
					{
						skillNameByID
					}), 10, 3);
				}
			}
			if (ShenHunData.IsInBianShenState() && Global.IsAutoFighting())
			{
				List<int> selfBianShenSkill = ShenHunData.GetSelfBianShenSkill();
				if (selfBianShenSkill.IndexOf(this.ExternalQueueSkillID) > -1 && !Global.SkillCoolDown(this.ExternalQueueSkillID) && this.SkillNeedMagicVOk(this.ExternalQueueSkillID))
				{
					return this.ExternalQueueSkillID;
				}
				for (int i = selfBianShenSkill.Count - 1; i > -1; i--)
				{
					int num = selfBianShenSkill[i];
					if (!Global.SkillCoolDown(num) && this.SkillNeedMagicVOk(num))
					{
						return num;
					}
				}
				return -1;
			}
			else
			{
				if (this.ExternalQueueSkillID > 0 && Global.IsAutoFighting() && Global.CanMapUseMagic(this.CurrentMapData, this.ExternalQueueSkillID) && !Global.SkillCoolDown(this.ExternalQueueSkillID))
				{
					if (this.SkillNeedMagicVOk(this.ExternalQueueSkillID))
					{
						return this.ExternalQueueSkillID;
					}
					if (Global.GetSkillDataByID(this.ExternalQueueSkillID) != null)
					{
						string skillNameByID2 = ConfigMagicInfos.GetSkillNameByID(this.ExternalQueueSkillID);
						Super.HintMainText(StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用技能【{0}】"), new object[]
						{
							skillNameByID2
						}), 10, 3);
					}
					this.ExternalQueueSkillID = -1;
				}
				if (this.CanUseAutoFightSkillID())
				{
					int autoFightSkillID = this.GetAutoFightSkillID();
					if (autoFightSkillID > 0 && autoFightSkillID != Global.Data.roleData.DefaultSkillID && Global.CanMapUseMagic(this.CurrentMapData, autoFightSkillID) && !Global.SkillCoolDown(autoFightSkillID))
					{
						if (this.SkillNeedMagicVOk(autoFightSkillID))
						{
							return autoFightSkillID;
						}
						if (Global.GetSkillDataByID(autoFightSkillID) != null)
						{
							string skillNameByID3 = ConfigMagicInfos.GetSkillNameByID(autoFightSkillID);
							Super.HintMainText(StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用技能【{0}】"), new object[]
							{
								skillNameByID3
							}), 10, 3);
						}
					}
				}
				int baseSkillID = Global.GetBaseSkillID(Global.Data.roleData.Occupation);
				if (Global.Data.roleData.Occupation > 0 && shiftPressed)
				{
					return baseSkillID;
				}
				if (Global.Data.roleData.DefaultSkillID <= 0)
				{
					return baseSkillID;
				}
				int defaultSkillID = Global.Data.roleData.DefaultSkillID;
				if (!Global.CanMapUseMagic(this.CurrentMapData, defaultSkillID))
				{
					return baseSkillID;
				}
				if (Global.SkillCoolDown(defaultSkillID))
				{
					return baseSkillID;
				}
				if (!this.SkillNeedMagicVOk(defaultSkillID))
				{
					if (Global.GetSkillDataByID(defaultSkillID) != null)
					{
						string skillNameByID4 = ConfigMagicInfos.GetSkillNameByID(defaultSkillID);
						Super.HintMainText(StringUtil.substitute(Global.GetLang("魔法值不足, 无法使用技能【{0}】"), new object[]
						{
							skillNameByID4
						}), 10, 3);
					}
					return baseSkillID;
				}
				return defaultSkillID;
			}
		}

		public int GetSkillAttackGridNum(int forceUseSkillID = -1)
		{
			int num = forceUseSkillID;
			if (num < 0)
			{
				num = this.GetAutoUseSkillID(false);
			}
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(num);
			if (skillXmlNode == null)
			{
				return 1;
			}
			int num2 = ConvertExt.SafeConvertToInt32(skillXmlNode.AttackDistance);
			if (this.CurrentMapData.GridSizeX == 0)
			{
				return 1;
			}
			return num2 / this.CurrentMapData.GridSizeX;
		}

		public int GetSkillAttackDistance(int forceUseSkillID = -1)
		{
			int num = forceUseSkillID;
			if (num < 0)
			{
				num = this.GetAutoUseSkillID(false);
			}
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(num);
			if (skillXmlNode == null)
			{
				return 100;
			}
			return ConvertExt.SafeConvertToInt32(skillXmlNode.AttackDistance);
		}

		public void AutoUseSkillManager()
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastAutoUseSkillManagerTicks < 1000L)
			{
				return;
			}
			this.LastAutoUseSkillManagerTicks = correctLocalTime;
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.AutoDrinkOptions();
			if (this.Leader.LockObject == null || !Global.IsAutoFighting() || this.Leader.Action == GActions.Magic)
			{
				return;
			}
			if (this.InSafeRegion())
			{
				return;
			}
			if (Global.IsInHorseRequestRideCD())
			{
				return;
			}
			int autoUseSkillID = this.GetAutoUseSkillID(false);
			if (Global.Data.AutoFightData.FightRadius <= 8)
			{
				this.DoMagicAttack(autoUseSkillID, new Point(-1, -1), this.Leader.LockObject, false, true);
			}
			else
			{
				this.SelectedSprite = Global.FindSprite(this.Leader.LockObject);
				this.ExternalSkillAttack(autoUseSkillID, true);
			}
		}

		private bool IsMagicWaiting(int occupation)
		{
			return this.Leader.CurrentMagic >= 0 && Global.FindMoveStroyboard(this.Leader.Name);
		}

		private void AutoUseSkillZS()
		{
			int autoFightSkillID = this.GetAutoFightSkillID();
			if (autoFightSkillID > 0 && !Global.SkillCoolDown(autoFightSkillID) && this.SkillNeedMagicVOk(autoFightSkillID))
			{
				this.SetTempWaitingSkillID(autoFightSkillID);
			}
		}

		private void AutoUseSkillFS()
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastAutoUseSkillFSTicks < 1000L)
			{
				return;
			}
			this.LastAutoUseSkillFSTicks = correctLocalTime;
			if (this.FindSprite(this.Leader.LockObject) == null)
			{
				return;
			}
			if (this.Leader.Action == GActions.Magic)
			{
				return;
			}
			if (Global.FindMoveStroyboard(this.Leader.Name) && this.Leader.IsMagicMove)
			{
				return;
			}
			if (!this.Leader.CanMagic())
			{
				return;
			}
			if (this.IsMagicWaiting(this.Leader.Occupation))
			{
				return;
			}
			int autoUseSkillID = this.GetAutoUseSkillID(false);
			this.DoMagicAttack(autoUseSkillID, new Point(-1, -1), this.Leader.LockObject, false, true);
		}

		private void AutoUseSkillDS()
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastAutoUseSkillDSTicks < 1000L)
			{
				return;
			}
			this.LastAutoUseSkillDSTicks = correctLocalTime;
			if (this.FindSprite(this.Leader.LockObject) == null)
			{
				return;
			}
			if (this.Leader.Action == GActions.Magic)
			{
				return;
			}
			if (Global.FindMoveStroyboard(this.Leader.Name) && this.Leader.IsMagicMove)
			{
				return;
			}
			if (!this.Leader.CanMagic())
			{
				return;
			}
			if (this.IsMagicWaiting(this.Leader.Occupation))
			{
				return;
			}
			int autoUseSkillID = this.GetAutoUseSkillID(false);
			this.DoMagicAttack(autoUseSkillID, new Point(-1, -1), this.Leader.LockObject, false, true);
		}

		private void AutoDrinkOptions()
		{
			bool flag = false;
			bool flag2 = false;
			if (this.AutoDrinkAddLifeVGoodsIDs == null)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AutoAddLifeVGoodsIDs", ',');
				this.AutoDrinkAddLifeVGoodsIDs = new int[systemParamIntArrayByName.Length];
				this.AutoDrinkAddLifeVGoodsLevels = new int[systemParamIntArrayByName.Length];
				for (int i = 0; i < systemParamIntArrayByName.Length; i++)
				{
					int num = 0;
					int num2 = systemParamIntArrayByName[i];
					if (num2 > 0)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num2);
						if (goodsXmlNodeByID != null)
						{
							num = Global.GetUnionLevel(goodsXmlNodeByID.ToZhuanSheng, goodsXmlNodeByID.ToLevel);
						}
						else
						{
							num2 = 0;
						}
					}
					this.AutoDrinkAddLifeVGoodsIDs[i] = num2;
					this.AutoDrinkAddLifeVGoodsLevels[i] = num;
				}
			}
			if (this.AutoDrinkAddMagicVGoodsIDs == null)
			{
				int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("AutoAddMagicVGoodsIDs", ',');
				this.AutoDrinkAddMagicVGoodsIDs = new int[systemParamIntArrayByName2.Length];
				this.AutoDrinkAddMagicVGoodsLevels = new int[systemParamIntArrayByName2.Length];
				for (int j = 0; j < systemParamIntArrayByName2.Length; j++)
				{
					int num3 = 0;
					int num4 = systemParamIntArrayByName2[j];
					if (num4 > 0)
					{
						GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(num4);
						if (goodsXmlNodeByID2 != null)
						{
							num3 = Global.GetUnionLevel(goodsXmlNodeByID2.ToZhuanSheng, goodsXmlNodeByID2.ToLevel);
						}
						else
						{
							num4 = 0;
						}
					}
					this.AutoDrinkAddMagicVGoodsIDs[j] = num4;
					this.AutoDrinkAddMagicVGoodsLevels[j] = num3;
				}
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			int num5 = 0;
			if (Global.Data.roleData.MaxLifeV > 0)
			{
				num5 = (int)((double)Global.Data.roleData.LifeV * 100.0 / (double)Global.Data.roleData.MaxLifeV);
			}
			int num6 = 0;
			if (Global.Data.roleData.MaxMagicV > 0)
			{
				num6 = (int)((double)Global.Data.roleData.MagicV * 100.0 / (double)Global.Data.roleData.MaxMagicV);
			}
			if (this.GuaJiGoAwayGoodsIDs == null)
			{
				this.GuaJiGoAwayGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("GuaJiGoAwayGoodsIDs", ',');
			}
			if (Global.Data.AutoFightData.LifeLessThanXDo && num5 < Global.Data.AutoFightData.LifeLessThanX)
			{
				int num7 = -1;
				if (this.GuaJiGoAwayGoodsIDs != null && this.GuaJiGoAwayGoodsIDs.Length >= 2)
				{
					int num8 = Math.Max(0, Global.Data.AutoFightData.LifeLessThanXAutoUse);
					num8 = Math.Min(1, Global.Data.AutoFightData.LifeLessThanXAutoUse);
					num7 = this.GuaJiGoAwayGoodsIDs[num8];
				}
				if (num7 > 0 && Global.CanMapUseGoods(this.CurrentMapData, num7) && Global.GetTotalGoodsCountByID(num7) > 0 && !Global.GoodsCoolDown(num7) && Global.CanUseGoods(num7, true, false))
				{
					this.CancelAutoFight(0, true);
					Global.UsingGoodsByID(num7);
					return;
				}
			}
			if (Global.Data.AutoFightData.MagicLessThanXDo && num6 < Global.Data.AutoFightData.MagicLessThanX)
			{
				int num9 = -1;
				if (this.GuaJiGoAwayGoodsIDs != null && this.GuaJiGoAwayGoodsIDs.Length >= 2)
				{
					int num10 = Math.Max(0, Global.Data.AutoFightData.MagicLessThanXAutoUse);
					num10 = Math.Min(1, Global.Data.AutoFightData.MagicLessThanXAutoUse);
					num9 = this.GuaJiGoAwayGoodsIDs[num10];
				}
				if (num9 > 0 && Global.CanMapUseGoods(this.CurrentMapData, num9) && Global.GetTotalGoodsCountByID(num9) > 0 && !Global.GoodsCoolDown(num9) && Global.CanUseGoods(num9, true, false))
				{
					this.CancelAutoFight(0, true);
					Global.UsingGoodsByID(num9);
					return;
				}
			}
			if (this.AutoDrinkAddLifeVGoodsIDs != null && num5 < Global.Data.roleData.AutoLifeV && correctLocalTime >= Global.Data.LastAddLifeTicks + 1000L)
			{
				bool flag3 = false;
				for (int k = 0; k < this.AutoDrinkAddLifeVGoodsIDs.Length; k++)
				{
					if (this.AutoDrinkAddLifeVGoodsIDs[k] > 0)
					{
						if (Global.CanMapUseGoods(this.CurrentMapData, this.AutoDrinkAddLifeVGoodsIDs[k]))
						{
							if (Global.GetTotalGoodsCountByID(this.AutoDrinkAddLifeVGoodsIDs[k]) > 0)
							{
								if (Global.GetUnionLevel(-1, -1) >= this.AutoDrinkAddLifeVGoodsLevels[k])
								{
									if (Global.GoodsCoolDown(this.AutoDrinkAddLifeVGoodsIDs[k]))
									{
										flag3 = true;
									}
									else if (Global.UsingGoodsByID(this.AutoDrinkAddLifeVGoodsIDs[k]))
									{
										flag3 = true;
										Global.Data.LastAddLifeTicks = correctLocalTime;
									}
								}
							}
						}
					}
				}
				flag = !flag3;
			}
			if (this.AutoDrinkAddMagicVGoodsIDs != null && num6 < Global.Data.roleData.AutoMagicV && correctLocalTime >= Global.Data.LastAddMagicTicks + 1000L)
			{
				bool flag4 = false;
				for (int l = 0; l < this.AutoDrinkAddMagicVGoodsIDs.Length; l++)
				{
					if (this.AutoDrinkAddMagicVGoodsIDs[l] > 0)
					{
						if (Global.CanMapUseGoods(this.CurrentMapData, this.AutoDrinkAddMagicVGoodsIDs[l]))
						{
							if (Global.GetTotalGoodsCountByID(this.AutoDrinkAddMagicVGoodsIDs[l]) > 0)
							{
								if (Global.GetUnionLevel(-1, -1) >= this.AutoDrinkAddMagicVGoodsLevels[l])
								{
									if (Global.GoodsCoolDown(this.AutoDrinkAddMagicVGoodsIDs[l]))
									{
										flag4 = true;
									}
									else if (Global.UsingGoodsByID(this.AutoDrinkAddMagicVGoodsIDs[l]))
									{
										flag4 = true;
										Global.Data.LastAddMagicTicks = correctLocalTime;
										break;
									}
								}
							}
						}
					}
				}
				flag2 = !flag4;
			}
			if (flag)
			{
				if (this.CanBuyAddLifeDrugs() && Global.Data.AutoFightData.AutoBuyMedicine && Global.IsAutoFighting())
				{
					if (Global.CanMapUseGoods(this.CurrentMapData, this.addLifeDrugId))
					{
						this.SendBuyAddLifeDrugsToServer();
					}
				}
				else if (!this.NoHintNoDrugGoBack && this.AutoFightingNoDrugNotfiy != null)
				{
					this.AutoFightingNoDrugNotfiy.Invoke(this, new AutoFightingNoDrugEventArgs
					{
						DrugType = 0,
						isAutoFight = Global.IsAutoFighting(),
						autoGoBack = Global.Data.AutoFightData.AutoGoBackWhenNoLifeDrugs
					});
				}
			}
			else if (flag2)
			{
				if (this.CanBuyAddMagicDrugs() && Global.Data.AutoFightData.AutoBuyMedicine && Global.IsAutoFighting())
				{
					if (Global.CanMapUseGoods(this.CurrentMapData, this.addMagicDrugId))
					{
						this.SendBuyAddMagicDrugsToServer();
					}
				}
				else if (!this.NoHintNoDrugGoBack && this.AutoFightingNoDrugNotfiy != null)
				{
					this.AutoFightingNoDrugNotfiy.Invoke(this, new AutoFightingNoDrugEventArgs
					{
						DrugType = 1,
						isAutoFight = Global.IsAutoFighting(),
						autoGoBack = Global.Data.AutoFightData.AutoGoBackWhenNoMagicDrugs
					});
				}
			}
		}

		private bool CanBuyAddLifeDrugs()
		{
			bool flag = false;
			if (Global.Data.roleData.ChangeLifeCount == 0)
			{
				flag = this.IsEnoughMoneyToBuy(1011);
				this.addLifeDrugId = ((!flag) ? 0 : 1011);
			}
			else if (Global.Data.roleData.ChangeLifeCount == 1)
			{
				flag = this.IsEnoughMoneyToBuy(1012);
				this.addLifeDrugId = ((!flag) ? 0 : 1012);
			}
			else if (Global.Data.roleData.ChangeLifeCount >= 2)
			{
				flag = this.IsEnoughMoneyToBuy(1013);
				this.addLifeDrugId = ((!flag) ? 0 : 1013);
			}
			return flag;
		}

		private bool CanBuyAddMagicDrugs()
		{
			bool flag = this.IsEnoughMoneyToBuy(1111);
			this.addMagicDrugId = ((!flag) ? 0 : 1111);
			return flag;
		}

		private bool IsEnoughMoneyToBuy(int goodId)
		{
			bool result = false;
			int priceOne = ConfigGoods.GetGoodsXmlNodeByID(goodId).PriceOne;
			int priceTwo = ConfigGoods.GetGoodsXmlNodeByID(goodId).PriceTwo;
			if (Global.Data.roleData.Money1 - priceOne * this.drugCount >= 0)
			{
				result = true;
				this.saleType = 1;
			}
			else if (Global.Data.roleData.YinLiang - priceTwo * this.drugCount >= 0)
			{
				result = true;
				this.saleType = 8;
			}
			return result;
		}

		private void SendBuyAddLifeDrugsToServer()
		{
			if (this.addLifeDrugId != 0)
			{
				GameInstance.Game.SpriteBuyGoods(this.addLifeDrugId, this.drugCount, this.saleType);
			}
		}

		private void SendBuyAddMagicDrugsToServer()
		{
			if (this.addMagicDrugId != 0)
			{
				GameInstance.Game.SpriteBuyGoods(this.addMagicDrugId, this.drugCount, this.saleType);
			}
		}

		public bool IsInStalling()
		{
			return this.Leader.StallName != string.Empty && this.Leader.StallName != null;
		}

		public bool OnLeaderPreMove()
		{
			if (Global.g_StallStateType != StallStateType.StallNull)
			{
				this.m_DisableInputEvent(this, null);
				return true;
			}
			if (Global.Data.MeditateState != 0)
			{
			}
			return false;
		}

		public void AutoFindRoad(int mapCode, Point pos, int offset, ExtActionTypes extActionType)
		{
			int num = 0;
			int num2 = 0;
			Global.GetMapMinLevelAndZhuanSheng(mapCode, out num2, out num);
			if (Global.Data.roleData.ChangeLifeCount * 400 + Global.Data.roleData.Level < num * 400 + num2)
			{
				Super.HintMainText(Global.GetLang("等级不足，无法前往目标地图！"), 10, 3);
				return;
			}
			if (this.OnLeaderPreMove())
			{
				return;
			}
			if (this.IsInStalling())
			{
				return;
			}
			if (Global.IsDongJieSprite(this.Leader))
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
				this.CancelAutoFight(0, false);
			}
			if (this.CancelAutoFindRoad(true) && (this.Leader == null || this.Leader.VLife <= 0.0))
			{
				return;
			}
			Point pos2 = pos;
			if (mapCode == this.MapCode)
			{
				if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
				{
					return;
				}
				pos2 = Global.GetAGridPointIn4Direction(new Point(pos.X / this.CurrentMapData.GridSizeX, pos.Y / this.CurrentMapData.GridSizeY), this.CurrentMapData.fixedObstruction, this.CurrentMapData);
				pos2 = new Point(pos2.X * this.CurrentMapData.GridSizeX, pos2.Y * this.CurrentMapData.GridSizeY);
			}
			Global.Data.AutoRoadItemsList = Global.FindAutoRoadItems(mapCode, pos2);
			Global.Data.AutoRoadCancelNum = 0;
			if (Global.Data.AutoRoadItemsList == null || Global.Data.AutoRoadItemsList.Count <= 0)
			{
				return;
			}
			this.Leader.LockObject = null;
			Global.Data.AutoRoadOffset = offset;
			Global.Data.AutoRoadExtActionType = extActionType;
			this.ContinueAutoFindRoadDeco();
			this.UpdateAutoFindWayOnPartMap();
		}

		private void ContinueAutoFindRoadDeco()
		{
			if (Global.Data.AutoRoadItemsList == null || Global.Data.AutoRoadItemsList.Count <= 0)
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
				return;
			}
			if (this.AutoFindRoadDecoNotify != null)
			{
				this.AutoFindRoadDecoNotify(this, new StateNotifyEventArgs
				{
					state = 0
				});
			}
		}

		private bool ProcessAutoFindRoad()
		{
			if (!this.EnableChangMap)
			{
				return false;
			}
			if (Global.Data.WaitingForMapChange)
			{
				return false;
			}
			if (Global.Data.WaitingForSystemHelp)
			{
				return false;
			}
			if (this.CurrentMapData == null)
			{
				return false;
			}
			if (Global.Data.AutoRoadItemsList == null || Global.Data.AutoRoadItemsList.Count <= 0)
			{
				return false;
			}
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				Global.Data.AutoRoadItemsList = null;
				Global.Data.AutoRoadCancelNum = 0;
				return false;
			}
			if (this.Leader.Action == GActions.Magic)
			{
				return false;
			}
			int myTimer = Global.GetMyTimer();
			this.SkillAttackWaitingID = 0;
			if (Global.Data.roleData.MapCode == Global.Data.AutoRoadItemsList[Global.Data.AutoRoadItemsList.Count - 1].MapID && Global.InCircle(this.Leader.Coordinate, Global.Data.AutoRoadItemsList[Global.Data.AutoRoadItemsList.Count - 1].ToPos, 150.0))
			{
				ExtActionTypes extActionTypes = Global.Data.AutoRoadExtActionType;
				this.CancelAutoFindRoad(true);
				if (this.Leader.ExtAction != ExtActionTypes.EXTACTION_NONE || extActionTypes != ExtActionTypes.EXTACTION_NONE)
				{
					if (this.Leader.ExtAction != ExtActionTypes.EXTACTION_NONE)
					{
						extActionTypes = this.Leader.ExtAction;
					}
					this.Leader.ExtAction = ExtActionTypes.EXTACTION_NONE;
					this.ProcessExtAction(extActionTypes);
				}
				return false;
			}
			if (Global.FindMoveStroyboard(this.Leader.Name))
			{
				return false;
			}
			MUDebug.LogTime("ProcessAutoFindRoad 1", ref myTimer, 3);
			int num = -1;
			for (int i = 0; i < Global.Data.AutoRoadItemsList.Count; i++)
			{
				if (Global.Data.AutoRoadItemsList[i].MapID == Global.Data.roleData.MapCode)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				this.CancelAutoFindRoad(true);
				return false;
			}
			MUDebug.LogTime("ProcessAutoFindRoad 2", ref myTimer, 3);
			Point toPos = Global.Data.AutoRoadItemsList[num].ToPos;
			int num2 = (Global.Data.AutoRoadItemsList[Global.Data.AutoRoadItemsList.Count - 1].MapID != Global.Data.roleData.MapCode) ? 0 : Global.Data.AutoRoadOffset;
			if (num2 <= 0 && Global.Data.AutoRoadItemsList[num].MapID == Global.Data.roleData.MapCode)
			{
				Point point = toPos;
				if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
				{
					return false;
				}
				Point agridPointIn4Direction = Global.GetAGridPointIn4Direction(new Point(point.X / this.CurrentMapData.GridSizeX, point.Y / this.CurrentMapData.GridSizeY), this.CurrentMapData.fixedObstruction, this.CurrentMapData);
				toPos = new Point(agridPointIn4Direction.X * this.CurrentMapData.GridSizeX, agridPointIn4Direction.Y * this.CurrentMapData.GridSizeY);
			}
			MUDebug.LogTime("ProcessAutoFindRoad 3", ref myTimer, 3);
			ExtActionTypes extActionTypes2 = (Global.Data.AutoRoadItemsList[num].MapID != Global.Data.roleData.MapCode) ? ExtActionTypes.EXTACTION_NONE : Global.Data.AutoRoadExtActionType;
			if (!this.LinearMoveByRunTo(toPos, num2, (int)extActionTypes2, false, 2))
			{
				this.CancelAutoFindRoad(true);
				if (extActionTypes2 != ExtActionTypes.EXTACTION_NONE)
				{
					this.Leader.ExtAction = ExtActionTypes.EXTACTION_NONE;
					this.ProcessExtAction(extActionTypes2);
				}
				return false;
			}
			MUDebug.LogTime("ProcessAutoFindRoad 4", ref myTimer, 3);
			return true;
		}

		private bool CanCancelAutoFindRoad()
		{
			if (Global.Data.AutoRoadItemsList == null)
			{
				return true;
			}
			Global.Data.AutoRoadCancelNum++;
			return Global.Data.AutoRoadCancelNum >= 2;
		}

		public bool CancelAutoFindRoad(bool sendStandCmd = true)
		{
			if (Global.Data.AutoRoadItemsList == null)
			{
				return false;
			}
			if (this.AutoFindRoadDecoNotify != null)
			{
				this.AutoFindRoadDecoNotify(this, new StateNotifyEventArgs
				{
					state = 1
				});
			}
			Global.Data.AutoRoadItemsList = null;
			Global.Data.AutoRoadOffset = 0;
			Global.Data.AutoRoadExtActionType = ExtActionTypes.EXTACTION_NONE;
			Global.Data.AutoRoadCancelNum = 0;
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameTaskBoxMini != null)
			{
				if (!Global.IsAutoFighting())
				{
					PlayZone.GlobalPlayZone.GameTaskBoxMini.ShowEffect(true);
				}
				else
				{
					PlayZone.GlobalPlayZone.GameTaskBoxMini.ShowEffect(false);
				}
			}
			if (Global.FindMoveStroyboard(this.Leader.Name))
			{
				int num = Global.StopStoryboard(this.Leader.Name, -1);
				if (num >= 0)
				{
					GameInstance.Game.SpriteStopMove(num);
				}
			}
			this.UpdateAutoFindWayOnPartMap();
			return true;
		}

		private bool IsLeaderAutoFindRoad(GSprite sprite)
		{
			bool result = false;
			if (sprite.RoleID == this.Leader.RoleID)
			{
				result = sprite.IsAutoFindRoad;
			}
			return result;
		}

		public bool IsAutoFindRoad()
		{
			return this.Leader.IsAutoFindRoad;
		}

		private void LeaderMovingTick(object sender, EventArgs e)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (this.Leader != null)
			{
				if (this.Count == 1 || this.Count == -1)
				{
					this.Count = 0;
					this.LeaderReportPos = new Point(this.Leader.Coordinate.X, this.Leader.Coordinate.Y);
					GameInstance.Game.SpritePosition(this.Leader.Coordinate, 0L);
				}
				else
				{
					this.Count++;
				}
			}
		}

		private void PingTimeTick(object sender, EventArgs e)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			int num = (this.LastProcessTicks <= 0f) ? 0 : ((int)((Time.realtimeSinceStartup - this.LastProcessTicks) * 1000f));
			int num2 = (this.LastDateTimeTicks <= 0L) ? 0 : ((int)(Time.time * 1000f - (float)this.LastDateTimeTicks));
			this.LastProcessTicks = Time.realtimeSinceStartup;
			this.LastDateTimeTicks = (long)((int)(Time.time * 1000f));
			GameInstance.Game.SpriteCheck(num, (int)((float)num * Time.timeScale));
			if (!GameInstance.Game.ActiveDisconnect)
			{
				BasePlayZone.InWaitPingCount++;
				if (BasePlayZone.InWaitPingCount * 2000 > BasePlayZone.PING_TIMEOUT)
				{
					GameInstance.Game.PingTimeOut();
					return;
				}
			}
			this.TimeSyncCounter++;
			if (this.TimeSyncCounter >= 30)
			{
				this.TimeSyncCounter = 0;
				GameInstance.Game.TimeSynchronizationByClient();
			}
		}

		protected void clientHeartTimer_Tick(object sender, EventArgs e)
		{
			this.SendClientHeart();
		}

		private void TraceEnemyTimer_Tick(object sender, EventArgs e)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			int myTimer = Global.GetMyTimer();
			this.TraceEnemy();
		}

		private void auxiliaryTimer_Tick(object sender, EventArgs e)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (Global.IsInJingLingYaoSai())
			{
				return;
			}
			MUDebug.LogStart();
			this.AutoFightManager();
			MUDebug.LogTime("auxiliaryTimer_Tick, AutoFightManager", 5);
			this.ProcessAutoFindRoad();
			MUDebug.LogTime("auxiliaryTimer_Tick, ProcessAutoFindRoad", 5);
			this.ProcessPetTrackingLeader();
			MUDebug.LogTime("auxiliaryTimer_Tick, ProcessPetTrackingLeader", 5);
			this.ProcessBiaoCheTrackingLeader();
			MUDebug.LogTime("auxiliaryTimer_Tick, ProcessBiaoCheTrackingLeader", 5);
			this.UpdateUI();
			MUDebug.LogTime("auxiliaryTimer_Tick, UpdateUI", 5);
		}

		private void UpdateGameRadarMap(GSprite sprite)
		{
			if (Global.CanGuanZhan())
			{
				return;
			}
			if (Global.Data.GameRadarMap == null || sprite.IsDefault3DRes)
			{
				return;
			}
			if (sprite.Action == GActions.Death)
			{
				return;
			}
			GRadarMapPoint gradarMapPoint = Global.Data.GameRadarMap.GetRolePoint(sprite.RoleID);
			bool flag = false;
			if (Global.Data.CurrentTeamData != null)
			{
				flag = Global.Data.CurrentTeamData.TeamRoles.Exists((TeamMemberData a) => sprite.RoleID == a.RoleID);
			}
			if (!gradarMapPoint)
			{
				string text = null;
				switch (sprite.SpriteType)
				{
				case GSpriteTypes.Other:
					if (Global.Data.CurrentTeamData != null)
					{
						text = ((!flag) ? "radarAStar" : "radarTeam");
					}
					else if (Global.IsCompMiDongMap())
					{
						text = "radarMonster";
						if (Global.Data != null && Global.Data.OtherRoles != null && Global.Data.OtherRoles.ContainsKey(sprite.RoleID) && Global.Data.OtherRoles[sprite.RoleID].CompType == Global.Data.roleData.CompType)
						{
							text = "radarNpc";
						}
					}
					else
					{
						text = "radarAStar";
					}
					break;
				case GSpriteTypes.Monster:
					text = "radarMonster";
					if (Global.Data.SystemMonsters.ContainsKey(sprite.RoleID) && Global.Data.SystemMonsters[sprite.RoleID].MasterRoleID == Global.Data.RoleID)
					{
						text = "radarNpc";
					}
					if (Global.IsInZhanMengLianSaiCompetetionMap())
					{
						if (Global.Data.SystemMonsters.ContainsKey(sprite.RoleID))
						{
							if (Global.Data.SystemMonsters[sprite.RoleID].BattleWitchSide == Global.Data.roleData.BattleWhichSide)
							{
								text = "radarNpc";
							}
							else
							{
								text = "radarMonster";
							}
						}
					}
					else if (Global.IsInKuaFuPlunderBattleMap())
					{
						if (Global.Data.SystemMonsters.ContainsKey(sprite.RoleID))
						{
							if (Global.Data.SystemMonsters[sprite.RoleID].BattleWitchSide == 1)
							{
								text = "radarNpc";
							}
							else
							{
								text = "radarMonster";
							}
							if (Global.Data.SystemMonsters[sprite.RoleID].MonsterType == 1101)
							{
								text = "radarNpc";
							}
						}
					}
					else if (Global.IsCompMiDongMap())
					{
						text = "radarNpc";
						if (Global.IsZhaoHuanShou(sprite.RoleID))
						{
							MonsterData monsterData = Global.Data.SystemMonsters[sprite.RoleID];
							if (monsterData != null)
							{
								int masterRoleID = monsterData.MasterRoleID;
								RoleData roleData = null;
								if (Global.Data.OtherRoles.TryGetValue(masterRoleID, ref roleData) && Global.Data.OtherRoles[sprite.RoleID].CompType != Global.Data.roleData.CompType)
								{
									text = "radarMonster";
								}
							}
						}
						else
						{
							MonsterData monsterData2 = Global.Data.SystemMonsters[sprite.RoleID];
							if (monsterData2 != null && monsterData2.BattleWitchSide != Global.Data.roleData.CompType)
							{
								text = "radarMonster";
							}
						}
					}
					break;
				case GSpriteTypes.NPC:
					if (!Global.IsInZhanMengLianSaiCompetetionMap())
					{
						text = "radarNpc";
					}
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					gradarMapPoint = Global.Data.GameRadarMap.AddRolePoint(sprite.RoleID);
					if (Global.IsInZhanMengLianSaiCompetetionMap() && Global.Data.SystemMonsters.ContainsKey(sprite.RoleID))
					{
						if (Global.Data.SystemMonsters[sprite.RoleID].BattleWitchSide == Global.Data.roleData.BattleWhichSide)
						{
							text = "radarNpc";
						}
						else
						{
							text = "radarMonster";
						}
					}
					if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
					{
						text = "zhongliyaosai";
						gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
					}
					else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
					{
						text = "none";
						if (Global.Data.SystemMonsters.ContainsKey(sprite.RoleID))
						{
							if (Global.Data.SystemMonsters[sprite.RoleID].BattleWitchSide == Global.Data.roleData.BattleWhichSide)
							{
								text = "mine";
							}
							else
							{
								text = "diren";
							}
						}
						gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
					}
					else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
					{
						if (ShiLiData.IsQiZuo(sprite.ExtensionID))
						{
							text = ShiLiData.GetMapMineQiZuoName(sprite.ExtensionID);
							gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
						}
						else if (sprite.SpriteType == GSpriteTypes.Monster)
						{
							text = "none";
						}
						else if (sprite.SpriteType == GSpriteTypes.Other)
						{
							if (Global.Data.OtherRoles[sprite.RoleID].CompType == Global.Data.roleData.CompType)
							{
								text = "radarNpc";
							}
							else
							{
								text = "radarMonster";
							}
							gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
						}
					}
					else if (Global.IsCompMiDongMap())
					{
						gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
						if (sprite.SpriteType == GSpriteTypes.Monster)
						{
							gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
						}
					}
					if (gradarMapPoint)
					{
						gradarMapPoint.Img = text;
					}
				}
			}
			if (!gradarMapPoint)
			{
				return;
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
			{
				if (ShiLiData.IsQiZuo(sprite.ExtensionID))
				{
					string mapMineQiZuoName = ShiLiData.GetMapMineQiZuoName(sprite.ExtensionID);
					if (gradarMapPoint.Img != mapMineQiZuoName)
					{
						gradarMapPoint.Img = mapMineQiZuoName;
						gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
					}
				}
				else if (sprite.SpriteType == GSpriteTypes.Monster)
				{
					if (gradarMapPoint.Img != "none")
					{
						gradarMapPoint.Img = "none";
					}
				}
				else if (sprite.SpriteType == GSpriteTypes.Other)
				{
					if (Global.Data.OtherRoles[sprite.RoleID].CompType == Global.Data.roleData.CompType)
					{
						gradarMapPoint.Img = "radarNpc";
					}
					else
					{
						gradarMapPoint.Img = "radarMonster";
					}
					gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
				}
			}
			else if (sprite.SpriteType == GSpriteTypes.Other)
			{
				gradarMapPoint.Img = ((!flag) ? "radarAStar" : "radarTeam");
				gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
			}
			if (Global.IsInKuaFuTeamCompete() || Global.IsInDaTaoSha())
			{
				if (Global.Data.OtherRoles.ContainsKey(sprite.RoleID))
				{
					if (Global.Data.OtherRoles[sprite.RoleID].BattleWhichSide == Global.Data.roleData.BattleWhichSide)
					{
						gradarMapPoint.Img = "radarNpc";
					}
					else
					{
						gradarMapPoint.Img = "radarMonster";
					}
				}
				gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
			}
			if (Global.IsInKuaFuPlunderBattleMap())
			{
				if (Global.Data.OtherRoles.ContainsKey(sprite.RoleID))
				{
					if (Global.Data.OtherRoles[sprite.RoleID].BattleWhichSide == 1 && Global.Data.OtherRoles[sprite.RoleID].BattleWhichSide == Global.Data.roleData.BattleWhichSide)
					{
						gradarMapPoint.Img = "radarNpc";
					}
					else if (Global.Data.OtherRoles[sprite.RoleID].BattleWhichSide == Global.Data.roleData.BattleWhichSide)
					{
						gradarMapPoint.Img = "radarNpc";
					}
					else
					{
						gradarMapPoint.Img = "radarMonster";
					}
				}
				gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 1f);
			}
			if (Global.IsCompMiDongMap())
			{
				gradarMapPoint.Img = "radarNpc";
				if (sprite.SpriteType == GSpriteTypes.Other)
				{
					if (Global.Data != null && Global.Data.OtherRoles != null && Global.Data.OtherRoles.ContainsKey(sprite.RoleID) && Global.Data.OtherRoles[sprite.RoleID].CompType != Global.Data.roleData.CompType)
					{
						gradarMapPoint.Img = "radarMonster";
					}
				}
				else if (sprite.SpriteType == GSpriteTypes.Monster)
				{
					if (Global.IsZhaoHuanShou(sprite.RoleID))
					{
						MonsterData monsterData3 = Global.Data.SystemMonsters[sprite.RoleID];
						if (monsterData3 != null)
						{
							int masterRoleID2 = monsterData3.MasterRoleID;
							RoleData roleData2 = null;
							if (Global.Data.OtherRoles.TryGetValue(masterRoleID2, ref roleData2) && Global.Data.OtherRoles[sprite.RoleID].CompType != Global.Data.roleData.CompType)
							{
								gradarMapPoint.Img = "radarMonster";
							}
						}
					}
					else
					{
						MonsterData monsterData4 = Global.Data.SystemMonsters[sprite.RoleID];
						if (monsterData4 != null && monsterData4.BattleWitchSide != Global.Data.roleData.CompType)
						{
							gradarMapPoint.Img = "radarMonster";
						}
					}
				}
			}
			Vector3 localPosition;
			localPosition..ctor((float)sprite.Coordinate.X / 100f * Global.Data.GameRadarMap.ScalingX - Global.Data.GameRadarMap.MiniMap.transform.localScale.x / 2f, (float)sprite.Coordinate.Y / 100f * Global.Data.GameRadarMap.ScalingY - Global.Data.GameRadarMap.MiniMap.transform.localScale.y / 2f, 0f);
			gradarMapPoint.transform.localPosition = localPosition;
		}

		private void SetRoleFangXiang(GSprite obj)
		{
			if (obj != null)
			{
				GameObject the3DGameObject = obj.The3DGameObject;
				Global.Data.GameRadarMap.playerArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f - the3DGameObject.transform.rotation.eulerAngles.y);
			}
		}

		private void HideWayFindingLine()
		{
			if (Global.Data.GamePartMap != null && Global.Data.GamePartMap.localMap != null)
			{
				Global.Data.GamePartMap.ClearRaderMapLinePoint();
				UserControl component = Global.Data.GamePartMap.LineContainer.GetComponent<UserControl>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}

		public void UpdateAutoFindWayOnPartMap()
		{
			if (!Global.Data.ShowingGamePartMap || Global.Data.GamePartMap == null || !Global.Data.GamePartMap.Visibility || Global.Data.AutoRoadItemsList == null || Global.Data.AutoRoadItemsList.Count <= 0)
			{
				this.HideWayFindingLine();
				return;
			}
			int num = -1;
			for (int i = 0; i < Global.Data.AutoRoadItemsList.Count; i++)
			{
				if (Global.Data.AutoRoadItemsList[i].MapID == Global.Data.roleData.MapCode)
				{
					num = i;
					break;
				}
			}
			UserControl userControl = Global.Data.GamePartMap.LineContainer.GetComponent<UserControl>();
			if (num == -1)
			{
				if (null != userControl)
				{
					Object.DestroyImmediate(userControl);
				}
				return;
			}
			StoryBoard storyBoard = StoryBoard.FindStoryBoard(this.Leader.Name);
			if (storyBoard != null)
			{
				int curPathIndex = storyBoard.GetCurPathIndex();
				List<ANode> path = storyBoard.Path;
				if (curPathIndex >= path.Count || storyBoard.IsStopped())
				{
					if (null != userControl)
					{
						Object.Destroy(userControl);
					}
					return;
				}
				if (userControl == null)
				{
					this.LineSmoothEx(ref path);
					int myTimer = Global.GetMyTimer();
					userControl = Global.Data.GamePartMap.LineContainer.AddComponent<UserControl>();
					GRadarMapPoint npcDot = null;
					for (int j = 0; j < path.Count; j++)
					{
						npcDot = Global.Data.GamePartMap.InstantiateMapLinePoint(npcDot);
						Global.Data.GamePartMap.AddMapLinePoint2(npcDot, path[j].x, path[j].y);
					}
					MUDebug.LogTime("UpdateAutoFindWayOnPartMap 2", ref myTimer, 3);
				}
			}
		}

		private void LineSmoothEx(ref List<ANode> srcLine)
		{
			int count = srcLine.Count;
			int num = 4;
			for (int i = 0; i < srcLine.Count - 1; i++)
			{
				ANode anode = srcLine[i];
				ANode anode2 = srcLine[i + 1];
				float num2 = Mathf.Sqrt((float)((anode2.x - anode.x) * (anode2.x - anode.x) + (anode2.y - anode.y) * (anode2.y - anode.y)));
				if (num2 > (float)num && num != 0)
				{
					int num3 = (int)num2 / num;
					float num4 = 1f / ((float)num3 + 1f);
					for (int j = 0; j < num3; j++)
					{
						Vector2 vector = Vector2.Lerp(new Vector2((float)anode.x, (float)anode.y), new Vector2((float)anode2.x, (float)anode2.y), num4 * (float)(j + 1));
						srcLine.Insert(i + j + 1, new ANode((int)vector.x, (int)vector.y));
					}
					i += num3;
				}
			}
		}

		private void LineSmooth(ref List<ANode> srcLine)
		{
			for (int i = 0; i < srcLine.Count - 1; i++)
			{
				if (Math.Abs(srcLine[i].x - srcLine[i + 1].x) > 0 || Math.Abs(srcLine[i].y - srcLine[i + 1].y) > 0)
				{
					int num = (srcLine[i].x - srcLine[i + 1].x < 0) ? 1 : -1;
					int num2 = (srcLine[i].y - srcLine[i + 1].y < 0) ? 1 : -1;
					int num3 = Math.Abs(srcLine[i].x - srcLine[i + 1].x);
					int num4 = Math.Abs(srcLine[i].y - srcLine[i + 1].y);
					int num5 = Math.Max(num3, num4);
					int j;
					for (j = 0; j < num5; j++)
					{
						int x = (Math.Abs(srcLine[i + j].x - srcLine[i + j + 1].x) <= 0) ? srcLine[i + j + 1].x : (srcLine[i].x + num * (j + 1));
						int y = (Math.Abs(srcLine[i + j].y - srcLine[i + j + 1].y) <= 0) ? srcLine[i + j + 1].y : (srcLine[i].y + num2 * (j + 1));
						ANode anode = new ANode(x, y);
						srcLine.Insert(i + j + 1, anode);
					}
					i += j;
				}
			}
		}

		private void UpdateUI()
		{
			if (this.Leader == null)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastUpdateUITicks < 1000L)
			{
				return;
			}
			this.CallAutoSkill();
			this.GetLastSelectedSprite();
			this.LastUpdateUITicks = correctLocalTime;
			bool flag = Global.FindMoveStroyboard(this.Leader.Name);
			int num = 3;
			if (!flag)
			{
				num = 2;
			}
			if (Global.Data.ShowingGamePartMap)
			{
				num = 1;
			}
			bool flag2 = correctLocalTime - this.LastUpdateRadarTicks >= (long)(num * 1000);
			if (flag2)
			{
				this.LastUpdateRadarTicks = correctLocalTime;
			}
			bool flag3 = correctLocalTime - this.LastMoveMiniMapTicks >= (long)(num * 1000);
			if (flag3)
			{
				this.LastMoveMiniMapTicks = correctLocalTime;
			}
			this.LastUpdateUILeaderX = (double)this.Leader.Coordinate.X;
			int num2 = 0;
			int num3 = 3;
			int num4 = 0;
			this.toDeleteObj.Clear();
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is IObject && objectsList[i] != this.LeftMouseClickDeco && !(objectsList[i] is GDecoration))
				{
					IObject @object = objectsList[i];
					if (@object is GSprite)
					{
						this.CheckNPCPosition(@object as GSprite);
					}
					bool flag4 = Math.Abs(@object.Coordinate.X - this.Leader.Coordinate.X) > 3000 || Math.Abs(@object.Coordinate.Y - this.Leader.Coordinate.Y) > 3000;
					bool flag5 = flag4;
					bool flag6 = flag4;
					if (@object is GSprite)
					{
						GSprite gsprite = @object as GSprite;
						if (gsprite.SpriteType != GSpriteTypes.NPC)
						{
							if (flag4)
							{
								flag4 = false;
								if (((gsprite.SpriteType == GSpriteTypes.Monster && gsprite.MonsterType != MonsterTypes.DSPetMonster) || (gsprite.SpriteType == GSpriteTypes.Other || gsprite.SpriteType == GSpriteTypes.FakeRole || gsprite.SpriteType == GSpriteTypes.JunQi || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet)) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe)) && (Math.Abs(@object.Coordinate.X - this.Leader.Coordinate.X) > 1000 || Math.Abs(@object.Coordinate.Y - this.Leader.Coordinate.Y) > 700))
								{
									this.toDeleteObj.Add(@object);
								}
							}
						}
						else if (@object is GGoodsPack)
						{
							flag4 = false;
							if (Math.Abs(@object.Coordinate.X - this.Leader.Coordinate.X) > 3000 || Math.Abs(@object.Coordinate.Y - this.Leader.Coordinate.Y) > 3000)
							{
								this.toDeleteObj.Add(@object);
							}
						}
						if (Global.Data.SysSetting.HideOtherRoles)
						{
							gsprite = (@object as GSprite);
							if ((gsprite.SpriteType == GSpriteTypes.Other && Global.GetMapSceneUIClass() != SceneUIClasses.JingJiChang) || (gsprite.SpriteType == GSpriteTypes.FakeRole || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet)) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe))
							{
								flag4 = true;
							}
						}
						if (Global.Data.SysSetting.HideChiBang)
						{
							gsprite = (@object as GSprite);
							if ((gsprite.SpriteType == GSpriteTypes.Other && Global.GetMapSceneUIClass() != SceneUIClasses.JingJiChang) || (gsprite.SpriteType == GSpriteTypes.FakeRole || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet)) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe))
							{
								flag5 = true;
							}
						}
						if (Global.Data.SysSetting.HideGameEffect)
						{
							gsprite = (@object as GSprite);
							if ((gsprite.SpriteType == GSpriteTypes.Other && Global.GetMapSceneUIClass() != SceneUIClasses.JingJiChang) || (gsprite.SpriteType == GSpriteTypes.FakeRole || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet)) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe))
							{
								flag6 = true;
							}
						}
					}
					if (flag4)
					{
						@object.HideObject();
					}
					else if (num4 < num3)
					{
						@object.Start();
						if (@object.ShowObject())
						{
							num4++;
						}
					}
					if (@object is GSprite)
					{
						GSprite gsprite2 = @object as GSprite;
						if (gsprite2.SpriteType == GSpriteTypes.Other)
						{
							if (flag5)
							{
								gsprite2.HideChiBang();
							}
							else
							{
								@object.Start();
								if (gsprite2.ShowChiBang())
								{
								}
							}
						}
					}
					if (@object is GSprite)
					{
						GSprite gsprite3 = @object as GSprite;
						if (gsprite3.SpriteType == GSpriteTypes.Other)
						{
							if (flag6)
							{
								gsprite3.HideOthersQiangHuaEffect();
							}
							else
							{
								@object.Start();
								if (gsprite3.ShowOthersQiangHuaEffect())
								{
								}
							}
						}
					}
				}
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite4 = objectsList[i] as GSprite;
					if (gsprite4.CurrentObjectState && flag2)
					{
						if (Global.IsZhaoHuanShou(gsprite4.RoleID))
						{
							if (Global.IsOwnZhaoHuanShou(gsprite4.RoleID))
							{
								this.UpdateGameRadarMap(gsprite4);
							}
						}
						else
						{
							this.UpdateGameRadarMap(gsprite4);
						}
						if (!flag)
						{
							if (Global.SpriteRondomTalk(gsprite4, num2 < 2))
							{
								num2++;
							}
							Global.SpriteTalk(gsprite4);
						}
					}
				}
			}
			for (int j = 0; j < this.toDeleteObj.Count; j++)
			{
				Global.RemoveObject(this.toDeleteObj[j], true);
			}
			if (flag2)
			{
				this.UpdateAutoFindWayOnPartMap();
				if (this.BackgroundHeartNotfiy != null)
				{
					this.BackgroundHeartNotfiy.Invoke(this, EventArgs.Empty);
				}
				Global.Data.GameRadarMap.RemoveDistanceRolePoint();
			}
		}

		public GSprite LoadBiaoChe(BiaoCheData biaoCheData)
		{
			return null;
		}

		private void UpdateBiaoCheEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
		}

		private void ProcessBiaoCheTrackingLeader()
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.Leader == null)
			{
				return;
			}
			if (this.Leader.BiaoChe == null)
			{
				return;
			}
		}

		private bool AddNewMapDeco(DecorationData decoData)
		{
			if (Global.Data.roleData.MapCode != decoData.MapCode)
			{
				return false;
			}
			long num = decoData.StartTicks + (long)decoData.MaxLiveTicks;
			if (decoData.MaxLiveTicks > 0 && Global.GetCorrectLocalTime() >= num)
			{
				return false;
			}
			string name = StringUtil.substitute("MapDeco{0}", new object[]
			{
				decoData.AutoID
			});
			if (this.FindName(name) is GDecoration)
			{
				return false;
			}
			Point pos = new Point(decoData.PosX, decoData.PosY);
			GDecoration decoration = Global.GetDecoration(decoData.DecoID, GDecorationTypes.Loop, pos, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = name;
				decoration.LifeTicks = ((decoData.MaxLiveTicks <= 0) ? 0L : num);
				decoration.AlphaTicks = (long)decoData.AlphaTicks;
				this.Add(decoration);
				Global.CurrentMapData._MapGrid.MoveObject(-1, -1, decoration.cx, decoration.cy, decoration);
				if (decoration.LifeTicks <= 0L)
				{
					Global.SetOpacity(decoration, this.CurrentMapData);
				}
				return true;
			}
			return false;
		}

		private bool DelMapDeco(int decoAutoID)
		{
			string name = StringUtil.substitute("MapDeco{0}", new object[]
			{
				decoAutoID
			});
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration == null)
			{
				return false;
			}
			Global.RemoveObject(gdecoration, true);
			return true;
		}

		public void AddForgeDecoration()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			GDecoration gdecoration = this.Leader.Root.FindName("Forge") as GDecoration;
			if (gdecoration != null)
			{
				return;
			}
			int num = (this.Leader.EfficaciousSection[3] - this.Leader.EfficaciousSection[2]) / 2;
			Point pos = new Point(this.Leader.CenterX, this.Leader.CenterY - num - 20);
			gdecoration = Global.GetDecoration(500, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
			if (gdecoration != null)
			{
				gdecoration.Name = "Forge";
				this.Leader.Root.Children.Add(gdecoration);
			}
		}

		public void RemoveForgeDecoration()
		{
			if (this.Leader == null)
			{
				return;
			}
			GDecoration gdecoration = this.Leader.Root.FindName("Forge") as GDecoration;
			if (gdecoration == null)
			{
				return;
			}
			Global.RemoveObject(gdecoration, true);
		}

		public void ExternalPlayDeco(int decoID, int shakeMap, int posType = 0)
		{
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (posType == 0)
			{
				GameInstance.Game.SpritePlayDeco(decoID, 2, 1, this.Leader.CenterX, this.Leader.CenterY, shakeMap, -1, -1, 0, 0);
			}
			else if (posType == 1)
			{
				GameInstance.Game.SpritePlayDeco(decoID, 2, 1, this.Leader.CenterX, this.Leader.CenterY - this.Leader.SpriteHeight, shakeMap, -1, -1, 0, 0);
			}
		}

		public void ReloadJingMaiUpDeco()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			Global.AddSpriteJingMaiUpDeco(this.Leader, Global.Data.roleData);
		}

		private void AddHuangDiDec(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
		}

		private void RemoveHuangDiDec(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
		}

		private void AddHuangHouDec(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null)
			{
				return;
			}
		}

		private void RemoveHuangHouDec(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void AddHuangChengDeco(GSprite sprite)
		{
			GDecoration gdecoration = sprite.Root.FindName("HuangChengDeco") as GDecoration;
			if (gdecoration != null)
			{
				return;
			}
			Point pos = new Point(sprite.CenterX, sprite.CenterY);
			gdecoration = Global.GetDecoration(539, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
			if (gdecoration != null)
			{
				gdecoration.Name = "HuangChengDeco";
				sprite.Root.Children.Add(gdecoration);
			}
		}

		public void RemoveHuangChengDeco(GSprite sprite)
		{
			GDecoration gdecoration = sprite.Root.FindName("HuangChengDeco") as GDecoration;
			if (gdecoration == null)
			{
				return;
			}
			Global.RemoveObject(gdecoration, true);
		}

		public void UpdateHuangChengDeco(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			if (roleData.Faction <= 0)
			{
				this.RemoveHuangChengDeco(sprite);
				return;
			}
			BangHuiLingDiItemData bhidbyLingDiID = Global.GetBHIDByLingDiID(2);
			if (bhidbyLingDiID == null || bhidbyLingDiID.BHID <= 0 || bhidbyLingDiID.BHID != roleData.Faction)
			{
				this.RemoveHuangChengDeco(sprite);
				return;
			}
			this.AddHuangChengDeco(sprite);
		}

		public void AddSpriteDeadDeco(GSprite sprite)
		{
			if (sprite.VLife > 0.0)
			{
				return;
			}
			if (sprite.SpriteType == GSpriteTypes.Monster)
			{
				if (sprite.MonsterType == MonsterTypes.CaiJi)
				{
					return;
				}
				if (Global.IsBloodCastleLingGuan(sprite.ExtensionID))
				{
					int code = 78;
					GDecoration decoration = Global.GetDecoration(code, GDecorationTypes.Loop, sprite.Coordinate, true, null, -1, -1, true, false);
					if (decoration != null)
					{
						this.Add(decoration);
					}
				}
			}
		}

		public void AddSpriteLeadDeco(GSprite sprite)
		{
			if (!(this.FindName("JDGQ_yindao") is GDecoration))
			{
				int code = 80;
				GDecoration decoration = Global.GetDecoration(code, GDecorationTypes.Loop, sprite.Coordinate, true, null, -1, -1, true, false);
				if (decoration != null)
				{
					decoration.Name = "JDGQ_yindao";
					this.Add(decoration);
					decoration.The3DGameObject.AddComponent<SpriteLeadDeco>();
				}
			}
		}

		public void AddSpriteAllJueXingTeXiao(GSprite sprite, int roleID)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
				JueXingData.ResetSelfJueXingEquips();
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			JueXingShiData jueXingData = roleData.JueXingData;
			if (jueXingData == null || null == sprite.The3DGameObject)
			{
				return;
			}
			string name = roleID + "_attackTeXiao";
			string name2 = roleID + "_defTeXiao";
			int attackEquip = jueXingData.AttackEquip;
			int defenseEquip = jueXingData.DefenseEquip;
			this.RemoveTeXiao(name);
			this.RemoveTeXiao(name2);
			if (attackEquip > 0)
			{
				MUAwakenSuitDetail awakenSuitDetailById = JueXingData.GetAwakenSuitDetailById(attackEquip);
				if (awakenSuitDetailById != null && JueXingData.GetJueXingShiEffectNum(awakenSuitDetailById, roleData) >= awakenSuitDetailById.AwakenIDs.Count)
				{
					float num = 1.6f;
					if (Global.IsBufferExist(121, roleData))
					{
						float qiang20Scale = ShenHunData.GetQiang20Scale(roleData);
						num *= qiang20Scale;
					}
					this.AddJueXingTeXiao(sprite, awakenSuitDetailById.SpecialEffect.SafeToInt32(0), name, num);
				}
			}
			if (defenseEquip > 0)
			{
				MUAwakenSuitDetail awakenSuitDetailById2 = JueXingData.GetAwakenSuitDetailById(defenseEquip);
				if (awakenSuitDetailById2 != null && JueXingData.GetJueXingShiEffectNum(awakenSuitDetailById2, roleData) >= awakenSuitDetailById2.AwakenIDs.Count)
				{
					float num2 = 1.6f;
					if (Global.IsBufferExist(121, roleData))
					{
						float qiang20Scale2 = ShenHunData.GetQiang20Scale(roleData);
						num2 *= qiang20Scale2;
					}
					this.AddJueXingTeXiao(sprite, awakenSuitDetailById2.SpecialEffect.SafeToInt32(0), name2, num2);
				}
			}
		}

		private void AddJueXingTeXiao(GSprite sprite, int decoID, string name, float size = 1f)
		{
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
				gdecoration = null;
			}
			if (gdecoration == null)
			{
				gdecoration = Global.GetDecoration(decoID, GDecorationTypes.Loop, sprite.Coordinate, true, null, -1, -1, true, false);
				if (gdecoration != null)
				{
					gdecoration.Name = name;
					this.Add(gdecoration);
					sprite.Root.Children.Add(gdecoration);
					gdecoration.The3DGameObject.transform.localPosition = new Vector3(0f, -0.15f, 0f);
					gdecoration.The3DGameObject.transform.localScale = new Vector3(size, size, size);
				}
			}
		}

		public void RemoveAllJueXingTeXiao(GSprite sprite, int roleID)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			JueXingShiData jueXingData = roleData.JueXingData;
			if (jueXingData == null || null == sprite.The3DGameObject)
			{
				return;
			}
			string name = roleID + "_attackTeXiao";
			string name2 = roleID + "_defTeXiao";
			this.RemoveTeXiao(name);
			this.RemoveTeXiao(name2);
		}

		private void RemoveTeXiao(string name)
		{
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
		}

		public void RemoveSpriteLeadDeco()
		{
			GDecoration gdecoration = this.FindName("JDGQ_yindao") as GDecoration;
			if (gdecoration == null)
			{
				return;
			}
			Global.RemoveObject(gdecoration, true);
		}

		public void AddChongShengYinJiEffect(GSprite sprite, int roleID)
		{
			if (sprite == null)
			{
				return;
			}
			this.RemoveChongShengYinJiEffect(sprite, roleID);
			if (ShenHunData.IsInBianShenState())
			{
				return;
			}
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			string rootEffectName = roleID + "_ChongShengYinJiRootEffect";
			GDecoration root = this.AddChongShengYinJiEffect(sprite, null, ChongShengYinJiData.EffectRootID, rootEffectName, 1f);
			root.DecorationLoadCompleteNotify = delegate(object s1, EventArgs e1)
			{
				Transform transform = root.The3DGameObject.transform;
				transform.name = rootEffectName;
				GameObject gameObject = s1 as GameObject;
				if (sprite.OnHorseEX)
				{
					GoodsData roleFightHorseData = Global.GetRoleFightHorseData(Global.Data.roleData.RoleID);
					if (roleFightHorseData != null)
					{
						float[] horseYinJiDecoH = Global.GetHorseYinJiDecoH(roleFightHorseData.GoodsID);
						if (horseYinJiDecoH.Length >= 3)
						{
							this.YinJiEffectRootZuoQiPosition = new Vector3(horseYinJiDecoH[0], horseYinJiDecoH[1], horseYinJiDecoH[2]);
						}
					}
					transform.localPosition = this.YinJiEffectRootZuoQiPosition;
				}
				else
				{
					transform.localPosition = new Vector3(0f, 1f, 0f);
				}
				Transform leftRoot = transform.FindChild("YinJi_effect(Clone)/001/01/left");
				Transform rightRoot = transform.FindChild("YinJi_effect(Clone)/001/01/right");
				List<int> effectIDs = ChongShengYinJiData.GetEffectIDs(roleData);
				if (effectIDs == null || effectIDs.Count <= 0)
				{
					return;
				}
				string leftEffectName = roleID + "_ChongShengYinJiLeftEffect";
				GDecoration leftDeco = this.AddChongShengYinJiEffect(sprite, leftRoot, effectIDs[0], leftEffectName, 1f);
				leftDeco.DecorationLoadCompleteNotify = delegate(object s2, EventArgs e2)
				{
					Transform transform2 = leftDeco.The3DGameObject.transform;
					transform2.name = leftEffectName;
					transform2.SetParent(leftRoot);
					transform2.localPosition = Vector3.zero;
					transform2.localRotation = Quaternion.identity;
				};
				string rightEffectName = roleID + "_ChongShengYinJiRightEffect";
				GDecoration rightDeco = this.AddChongShengYinJiEffect(sprite, rightRoot, effectIDs[1], rightEffectName, 1f);
				rightDeco.DecorationLoadCompleteNotify = delegate(object s3, EventArgs e3)
				{
					Transform transform2 = rightDeco.The3DGameObject.transform;
					transform2.name = rightEffectName;
					transform2.SetParent(rightRoot);
					transform2.localPosition = Vector3.zero;
					transform2.localRotation = Quaternion.identity;
				};
			};
		}

		private GDecoration AddChongShengYinJiEffect(GSprite sprite, Transform parent, int decoID, string name, float size = 1f)
		{
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
				gdecoration = null;
			}
			if (gdecoration == null)
			{
				gdecoration = Global.GetDecoration(decoID, GDecorationTypes.Loop, sprite.Coordinate, true, null, -1, -1, true, false);
				if (gdecoration != null)
				{
					gdecoration.Name = name;
					this.Add(gdecoration);
					if (parent == null)
					{
						sprite.Root.Children.Add(gdecoration);
						gdecoration.The3DGameObject.transform.localPosition = Vector3.zero;
						gdecoration.The3DGameObject.transform.localScale = new Vector3(size, size, size);
					}
				}
				return gdecoration;
			}
			return null;
		}

		public void ResetChongShengYinJiEffectPosition(GSprite sprite, int roleID, bool isOn)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			string name = roleID + "_ChongShengYinJiRootEffect";
			if (null == sprite.The3DGameObject)
			{
				return;
			}
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration == null)
			{
				return;
			}
			if (gdecoration.The3DGameObject != null)
			{
				Transform transform = gdecoration.The3DGameObject.transform;
				Transform transform2 = transform.FindChild("YinJi_effect(Clone)");
				if (isOn)
				{
					GoodsData roleFightHorseData = Global.GetRoleFightHorseData(Global.Data.roleData.RoleID);
					if (roleFightHorseData != null)
					{
						float[] horseYinJiDecoH = Global.GetHorseYinJiDecoH(roleFightHorseData.GoodsID);
						if (horseYinJiDecoH.Length >= 3)
						{
							this.YinJiEffectRootZuoQiPosition = new Vector3(horseYinJiDecoH[0], horseYinJiDecoH[1], horseYinJiDecoH[2]);
						}
					}
					transform2.localPosition = this.YinJiEffectRootZuoQiPosition;
				}
				else
				{
					transform2.localPosition = new Vector3(0f, 1f, 0f);
				}
			}
		}

		public void RemoveChongShengYinJiEffect(GSprite sprite, int roleID)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			string name = roleID + "_ChongShengYinJiRootEffect";
			if (null == sprite.The3DGameObject)
			{
				return;
			}
			GDecoration gdecoration = this.FindName(name) as GDecoration;
			if (gdecoration == null)
			{
				return;
			}
			string name2 = roleID + "_ChongShengYinJiLeftEffect";
			string name3 = roleID + "_ChongShengYinJiRightEffect";
			GDecoration gdecoration2 = this.FindName(name2) as GDecoration;
			if (gdecoration2 != null)
			{
				Global.RemoveObject(gdecoration2, true);
			}
			GDecoration gdecoration3 = this.FindName(name3) as GDecoration;
			if (gdecoration3 != null)
			{
				Global.RemoveObject(gdecoration3, true);
			}
			Global.RemoveObject(gdecoration, true);
		}

		private void AddFSHuDunDec(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null)
			{
				return;
			}
			if (sprite.Occupation != 1)
			{
				return;
			}
			if (roleData.FSHuDunStart <= 0L)
			{
				return;
			}
			this.AddDelayDeco(sprite, 31, roleData.FSHuDunStart, roleData.FSHuDunSeconds);
		}

		private void AddZhongDuStatus(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null)
			{
				return;
			}
			if (roleData.ZhongDuStart <= 0L)
			{
				return;
			}
			this.AddDelayColor(sprite, 43, roleData.ZhongDuStart, roleData.ZhongDuSeconds);
		}

		private void AddZhongDuStatusToMonster(GSprite sprite, MonsterData monsterData)
		{
			if (sprite == null || monsterData == null)
			{
				return;
			}
			if (monsterData.ZhongDuStart <= 0L)
			{
				return;
			}
			this.AddDelayColor(sprite, 43, monsterData.ZhongDuStart, monsterData.ZhongDuSeconds);
		}

		private void AddDSHide(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null)
			{
				return;
			}
			if (roleData.DSHideStart <= 0L)
			{
				return;
			}
			sprite.TransparentVisible = 0.2;
		}

		private void RemoveDSHide(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
			sprite.TransparentVisible = 1.0;
		}

		public void UpdateBattleSideImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			if (Global.GetLingDiIDByMapCode2(roleData.MapCode) != 2)
			{
				sprite.LingDiWordVisible = false;
				sprite.LingDiWordImageURL = null;
			}
			if (!Global.IsBattleMap())
			{
				return;
			}
			sprite.ShowName = Global.FormatShowName(roleData, 0);
			sprite.LingDiWordVisible = true;
		}

		private void HandleSheLiZhiYuanNPCDeco(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("SheLiZhiYuanNPCID");
			if (sprite.ExtensionID != num)
			{
				return;
			}
			GDecoration gdecoration = sprite.Root.FindName("SheLiZhiYuanDeco") as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			if (Global.Data.roleData.HuangDiRoleID > 0)
			{
				return;
			}
			gdecoration = Global.GetDecoration(40001, GDecorationTypes.Loop, new Point(sprite.CenterX, sprite.CenterY), false, null, -1, -1, true, false);
			if (gdecoration != null)
			{
				gdecoration.Name = "SheLiZhiYuanDeco";
				sprite.Root.Children.Add(gdecoration);
				if (!sprite.CurrentObjectState)
				{
					gdecoration.HideObject();
				}
			}
		}

		public GameObject AddFlyEffect(GSprite fromSprite, GSprite toSprite, DecorationDestroyNotifyHandler decorationDestroyNotify)
		{
			GDecoration decoration = Global.GetDecoration(81, GDecorationTypes.Loop, fromSprite.Coordinate, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = "JuQing_baozha";
				this.Add(decoration);
				decoration.DecorationDestroyNotify = decorationDestroyNotify;
				Vector3 meshSize = U3DUtils.GetMeshSize(toSprite.The3DGameObject.transform);
				Vector3 toPosition;
				toPosition..ctor(toSprite.The3DGameObject.transform.position.x, toSprite.The3DGameObject.transform.position.y + meshSize.y, toSprite.The3DGameObject.transform.position.z);
				decoration.AddFly(toPosition, 5f);
				EffectManager effectManager = decoration.The3DGameObject.AddComponent<EffectManager>();
				effectManager.TimeTotal = 20f;
				return decoration.The3DGameObject;
			}
			return null;
		}

		public GameObject AddSpriteEffect(GSprite toSprite, DecorationDestroyNotifyHandler decorationDestroyNotify)
		{
			Vector3 meshSize = U3DUtils.GetMeshSize(toSprite.The3DGameObject.transform);
			GDecoration decoration = Global.GetDecoration(82, GDecorationTypes.Loop, toSprite.Coordinate, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = "JuQing_baozha";
				this.Add(decoration);
				Vector3 vector = toSprite.The3DGameObject.transform.TransformDirection(Vector3.forward) * 0.3f;
				vector..ctor(vector.x, vector.y + meshSize.y, vector.z);
				vector += toSprite.The3DGameObject.transform.position;
				if (null != Global.MainCamera)
				{
					vector = Vector3.Lerp(vector, Global.MainCamera.transform.position, 0.1f);
				}
				decoration.The3DGameObject.transform.position = vector;
				decoration.DecorationDestroyNotify = decorationDestroyNotify;
				return decoration.The3DGameObject;
			}
			return null;
		}

		public GameObject AddDiaoQiaoEffect(DecorationDestroyNotifyHandler decorationDestroyNotify)
		{
			Point pos = new Point(77, 63);
			GDecoration decoration = Global.GetDecoration(83, GDecorationTypes.Loop, pos, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = "JuQing_DiaoQiaoFangXia";
				this.Add(decoration);
				decoration.DecorationDestroyNotify = decorationDestroyNotify;
				decoration.The3DGameObject.transform.localPosition = new Vector3(76.94f, 50.5f, 62.97f);
				return decoration.The3DGameObject;
			}
			return null;
		}

		public GameObject AddDiaoQiaoBaoZhaEffect(DecorationDestroyNotifyHandler decorationDestroyNotify)
		{
			Point pos = new Point(77, 63);
			GDecoration decoration = Global.GetDecoration(85, GDecorationTypes.Loop, pos, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = "JuQing_DiaoQiaoBaoZha";
				this.Add(decoration);
				decoration.DecorationDestroyNotify = decorationDestroyNotify;
				decoration.The3DGameObject.transform.localPosition = new Vector3(76.94f, 50.5f, 62.97f);
				return decoration.The3DGameObject;
			}
			return null;
		}

		public GameObject AddShiGuanEffect(DecorationDestroyNotifyHandler decorationDestroyNotify)
		{
			Point pos = new Point(0, 0);
			GDecoration decoration = Global.GetDecoration(84, GDecorationTypes.Loop, pos, true, null, -1, -1, true, false);
			if (decoration != null)
			{
				decoration.Name = "JuQing_DiaoQiaoBaoZha";
				this.Add(decoration);
				decoration.DecorationDestroyNotify = decorationDestroyNotify;
				decoration.The3DGameObject.transform.localPosition = new Vector3(97.61f, 50.5f, 62.86f);
				return decoration.The3DGameObject;
			}
			return null;
		}

		private void AddMingZhongDec(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null || roleData.BufferDataList == null || roleData.BufferDataList.Count == 0)
			{
				return;
			}
			if (Global.BuffMagicDict == null)
			{
				Global.BuffMagicDict = ConfigSystemParam.GetSystemParamIntDictByName("BuffMagic", '|', ',');
			}
			int num = Global.CalcOriginalOccupationID(sprite.Occupation);
			int[] array = null;
			if (Global.BuffMagicDict.TryGetValue(num, ref array))
			{
				for (int i = 0; i < roleData.BufferDataList.Count; i++)
				{
					for (int j = 0; j < array.Length - 1; j += 2)
					{
						if (array[j + 1] == roleData.BufferDataList[i].BufferID)
						{
							int magicCode = array[j];
							this.AddDelayDeco(sprite, magicCode, roleData.BufferDataList[i].StartTime, roleData.BufferDataList[i].BufferSecs);
						}
					}
				}
			}
			for (int k = 0; k < roleData.BufferDataList.Count; k++)
			{
				if (roleData.BufferDataList[k].BufferID == 101 && Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
				{
					this.AddShengbeiSlowDown(sprite, roleData.BufferDataList[k].StartTime, roleData.BufferDataList[k].BufferSecs + 10000, roleData.BufferDataList[k].BufferID);
				}
			}
			if (Global.IsInKuaFuHuoDongWangZhe())
			{
				for (int l = 0; l < roleData.BufferDataList.Count; l++)
				{
					if (roleData.BufferDataList[l].BufferID == 2080001 || roleData.BufferDataList[l].BufferID == 2080007 || roleData.BufferDataList[l].BufferID == 2080008 || roleData.BufferDataList[l].BufferID == 2080009)
					{
						int bufferDecID = Global.GetBufferDecID(roleData.BufferDataList[l].BufferID);
						this.AddBufferDeco(sprite, bufferDecID, roleData.BufferDataList[l].StartTime, roleData.BufferDataList[l].BufferSecs);
					}
				}
			}
		}

		private void AddHunMiDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null || seconds == 0)
			{
				return;
			}
			this.AddDelayDeco(sprite, 105, startTicks, seconds);
		}

		private void AddSlowDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null)
			{
				return;
			}
			this.AddDelayDeco(sprite, 106, startTicks, seconds);
		}

		private void AddDingShenDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null)
			{
				return;
			}
			this.AddDelayDeco(sprite, 2201, startTicks, seconds);
		}

		private void AddAttackDownDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null)
			{
				return;
			}
			this.AddDelayDeco(sprite, 120, startTicks, seconds);
		}

		private void AddDefenseDownDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null)
			{
				return;
			}
			this.AddDelayDeco(sprite, 305, startTicks, seconds);
		}

		private void AddHitDownDec(GSprite sprite, long startTicks, int seconds)
		{
			if (sprite == null)
			{
				return;
			}
			this.AddDelayDeco(sprite, 204, startTicks, seconds);
		}

		private void AddShengbeiSlowDown(GSprite sprite, long startTicks, int seconds, int BufferID)
		{
			if (sprite == null)
			{
				return;
			}
			int bufferDecID = Global.GetBufferDecID(BufferID);
			this.AddBufferDeco(sprite, bufferDecID, startTicks, seconds);
		}

		public GSprite LoadFakeRole(FakeRoleData fakeRoleData, bool addToCanvas)
		{
			RoleData roleData = Global.ClientDataToRoleDataMini(fakeRoleData.MyRoleDataMini);
			double num = (double)fakeRoleData.MyRoleDataMini.PosX;
			double num2 = (double)fakeRoleData.MyRoleDataMini.PosY;
			double num3 = (double)fakeRoleData.MyRoleDataMini.RoleDirection;
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				fakeRoleData.FakeRoleID
			});
			GSprite gsprite = new GSprite();
			gsprite.BattleWhichSide = roleData.BattleWhichSide;
			gsprite.FakeRoleType = (FakeRoleTypes)fakeRoleData.FakeRoleType;
			gsprite.SpriteType = GSpriteTypes.FakeRole;
			gsprite.CoordinateChanged += delegate(GSprite sender)
			{
				this.UpdateFakeRoleEvent(sender);
			};
			gsprite.SpriteDead += delegate(object s, EventArgs e)
			{
				if (this.SpriteDeadNotify != null)
				{
					this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
					{
						RoleID = (s as GSprite).RoleID,
						SpriteType = (s as GSprite).SpriteType,
						ShowDlg = false,
						ExtensionID = (s as GSprite).ExtensionID
					});
				}
				if ((s as GSprite).Name == Global.WatchSprite)
				{
					this.SetLockDeco(s as GSprite, false);
					if (this.Leader.LockObject == Global.WatchSprite)
					{
						this.Leader.LockObject = null;
					}
					Global.WatchSprite = null;
				}
				if (Global.Data.OtherRoles.ContainsKey((s as GSprite).RoleID))
				{
					RoleData roleData2 = Global.Data.OtherRoles[(s as GSprite).RoleID];
					roleData2.ZhongDuStart = 0L;
					roleData2.ZhongDuSeconds = 0;
				}
				this.AddSpriteDeadDeco(s as GSprite);
			};
			this.LoadSprite(gsprite, fakeRoleData.FakeRoleID, roleData.RoleSex, name, Global.GetRoleBHName(roleData), roleData.OtherName, roleData.RoleName, roleData.Occupation, fakeRoleData.ToExtensionID, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, Global.Data.SnameBrushColor, (double)roleData.LifeV, roleData.PKMode, roleData.PKValue, -1.0, roleData.BodyCode, roleData.WeaponCode, new Point((int)num, (int)num2), (int)num3, (double)Global.Data.LifeTotalWidth, 1.0, roleData.Faction, addToCanvas);
			gsprite.PKKingSpriteName = Global.GetPKKingSpriteName(roleData);
			gsprite.VLifeMax = (double)roleData.LifeV;
			gsprite.VArmorMax = (long)roleData.MaxArmorV;
			gsprite.VArmor = (long)roleData.CurrentArmorV;
			this.RefreshSpriteLife(gsprite);
			this.UpdateNameColor(gsprite, roleData);
			gsprite.VPK = roleData.PKValue;
			this.UpdatePKValue(roleData, gsprite);
			this.UpdateTeamFlags(gsprite, roleData.TeamID > 0, roleData.TeamLeaderRoleID == gsprite.RoleID);
			this.UpdateSkillWord(gsprite, roleData);
			this.UpdateLittleVIP(gsprite, roleData);
			this.UpdateJingMaiWord(gsprite, roleData);
			this.UpdateBattleNameImage(gsprite, roleData);
			this.UpdateHeroIndexImage(gsprite, roleData);
			this.UpdateChengJiuImage(gsprite, roleData);
			this.UpdateChuanQiJingMaiImage(gsprite, roleData);
			this.UpdateChuanWuXueImage(gsprite, roleData);
			this.UpdateHuangChengImage(gsprite, roleData);
			this.UpdateWangChengImage(gsprite, roleData);
			this.UpdateLingDiWord(gsprite, roleData);
			this.UpdateBattleSideImage(gsprite, roleData);
			this.UpdateJieriWord(gsprite, roleData);
			this.UpdateMarketName(gsprite, roleData);
			this.AddMingZhongDec(gsprite, roleData);
			if (roleData.LifeV <= 0)
			{
				gsprite.Action = GActions.Death;
			}
			if (!Global.Data.SysSetting.HideOtherRoles)
			{
				gsprite.Start();
			}
			else
			{
				gsprite.Start();
				gsprite.HideObject();
			}
			Global.AddSpriteJingMaiUpDeco(gsprite, roleData);
			this.AddHuangDiDec(gsprite);
			this.AddHuangHouDec(gsprite, roleData);
			this.AddFSHuDunDec(gsprite, roleData);
			this.AddZhongDuStatus(gsprite, roleData);
			this.AddDSHide(gsprite, roleData);
			this.AddMaBiStatus(gsprite, roleData);
			gsprite.ChangeWeaponsPosition(this.InSafeRegion(gsprite));
			return gsprite;
		}

		private void UpdateFakeRoleEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				Global.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
				bool flag = this.InSafeRegion(sprite);
				if (flag != sprite.IsInSafeRegion)
				{
					sprite.ChangeWeaponsPosition(flag);
				}
			}
		}

		private bool CanAutoFightGetThings(int goodsID)
		{
			if (!Global.IsAutoFighting())
			{
				return true;
			}
			if (!Global.Data.AutoFightData.AutoGetEquips && !Global.Data.AutoFightData.AutoGetThings)
			{
				return false;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				if (!Global.Data.AutoFightData.AutoGetEquips)
				{
					return false;
				}
			}
			else if (!Global.Data.AutoFightData.AutoGetThings)
			{
				return false;
			}
			return true;
		}

		public void ShowGoodsPackName(bool value)
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GGoodsPack)
				{
					GGoodsPack ggoodsPack = objectsList[i] as GGoodsPack;
					ggoodsPack.TextVisible = (value || Global.Data.SysSetting.ShowGoodsPackName);
				}
			}
		}

		public bool HasGoodsPack()
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GGoodsPack)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearScreenShake()
		{
			if (null == Camera.main)
			{
				return;
			}
			XftCameraShakeComp component = Camera.main.GetComponent<XftCameraShakeComp>();
			if (null == component)
			{
				return;
			}
			component.enabled = false;
		}

		private bool UpdateLeaderDirection(bool forceMove = false)
		{
			UIJoystick joystick = Global.Joystick;
			if (null == joystick || this.Leader == null)
			{
				return false;
			}
			if (Global.CanGuanZhan() && ZhanMengLianSaiGuanZhanPopupList.IsTracking)
			{
				return false;
			}
			if (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan)
			{
				return false;
			}
			if (Global.IsDongJieSprite(this.Leader))
			{
				return false;
			}
			Vector2 zero = Vector2.zero;
			int input36Direction = Global.GetInput36Direction(joystick, out zero);
			int inputDirection = Global.GetInputDirection(joystick, out zero);
			if (input36Direction >= 0)
			{
				if (this.OnLeaderPreMove())
				{
					return false;
				}
				if (this.IsInStalling())
				{
					return false;
				}
				this.CancelAutoFindRoad(true);
				if (!forceMove)
				{
					if (Global.FindStoryboard("Leader") != null)
					{
						if (input36Direction != this._LastDir)
						{
							Global.RemoveStoryboard("Leader");
							forceMove = true;
						}
					}
					else
					{
						forceMove = true;
					}
				}
				if (!Global.CanMoveByRay2(this.Leader, input36Direction))
				{
					return false;
				}
				if (forceMove)
				{
					GActions externalAction = GActions.Walk;
					this._PressJoystickCount++;
					if (this._PressJoystickCount >= 5 && !this.IsInSafeRegion)
					{
						externalAction = GActions.Run;
					}
					this.ClearScreenShake();
					this.MoveByDirectionBy36Dir(input36Direction, inputDirection, false, (int)externalAction);
				}
				this.LastJoyPosition = zero;
				this.Leader.LastJoyPosition = this.LastJoyPosition;
				this.Leader.WaitingDirection = inputDirection;
			}
			else
			{
				this._PressJoystickCount = 0;
				this.LastJoyPosition = Vector2.zero;
				this.Leader.LastJoyPosition = Vector2.zero;
			}
			this._LastDir = input36Direction;
			return true;
		}

		private void UpdateSelectedObject()
		{
			if (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan)
			{
				return;
			}
			Vector3 position = Vector3.zero;
			if (Application.isEditor || Application.platform == 2)
			{
				if (Input.GetMouseButtonDown(0))
				{
					position = Input.mousePosition;
					this.HandleAS3MouseDownEvent(position);
				}
			}
			else if (Input.touchCount > 0 && Input.GetTouch(0).phase == null)
			{
				position = Input.GetTouch(0).position;
				this.HandleAS3MouseDownEvent(position);
			}
		}

		private void HandleAS3MouseDownEvent(Vector3 position)
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (Global.IsDongJieSprite(this.Leader))
			{
				return;
			}
			if (!Global.IsMainCamera)
			{
				return;
			}
			int layer = LayerMask.NameToLayer("MUUI");
			Camera camera = UIHelper.FindCameraForLayer(layer, -1, null, null, true);
			if (camera)
			{
				Vector3 vector = camera.ScreenToWorldPoint(position);
				vector.z = -1000000f;
				Ray ray;
				ray..ctor(vector, Vector3.forward);
				RaycastHit[] array = Physics.RaycastAll(ray, float.MaxValue, LayerMask.GetMask(this.UILayers));
				if (array != null && array.Length > 0)
				{
					return;
				}
			}
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(position);
			Point point = new Point((int)(vector2.x * 100f), (int)(vector2.z * 100f));
			this.CurDestinationPos = point;
			double num = (double)Global.GetCorrectLocalTime();
			if (num - this.LeftButtonClickTicks <= 300.0)
			{
				this.MuchClickNum++;
			}
			else
			{
				this.MuchClickNum = 0;
			}
			this.LeftButtonClickTicks = num;
			this.ShiftAttackWaitingPos = new Point(-1, -1);
			this.ToOpenGoodsPack = null;
			if (this.LeftButtonClickOnSprite != null)
			{
				this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
				{
					RoleID = -1,
					SpriteType = GSpriteTypes.Leader,
					ShowDlg = false,
					ExtensionID = -1
				});
			}
			if (this.IsInStalling())
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
			}
			bool ctrlPressed = false;
			bool flag = true;
			if (this.CancelAutoFindRoad(false))
			{
				if (flag)
				{
					if (Global.WatchSprite != null)
					{
						this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
					}
					Global.WatchSprite = null;
					this.ShiftAttackWaitingPos = point;
				}
				this.Leader.LockObject = null;
				return;
			}
			this.HideMouseLeftButtonUpEffect();
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.HandleMouseDownPoint(position, flag, ctrlPressed);
		}

		public void HandleMouseDownPoint(Vector3 position, bool shiftPressed, bool ctrlPressed)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(position);
			Point point = new Point((int)(vector.x * 100f), (int)(vector.z * 100f));
			this.Leader.IsMagicMove = false;
			this.HitIObject(position);
			if (this.SelectedSprite != null)
			{
				string name = this.SelectedSprite.Name;
				bool flag = false;
				if (name != null)
				{
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (shiftPressed || gsprite.SpriteType != GSpriteTypes.Other)
						{
							if (Global.WatchSprite == null)
							{
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name != Global.WatchSprite)
							{
								if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
								{
									this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
									this.SetLockTargetDeco(this.FindSprite(Global.WatchSprite), false);
								}
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name == Global.WatchSprite)
							{
								if (this.Leader.LockObject == null)
								{
									flag = true;
								}
								else if (this.Leader.Action == GActions.Stand)
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							gsprite = this.FindSprite(Global.WatchSprite);
							if (gsprite != null && gsprite != this.Leader)
							{
								byte b = this.SetLockDeco(gsprite, true);
								this.SetLockTargetDeco(gsprite, b != 30);
							}
						}
					}
					if (gsprite != null)
					{
						bool flag2 = true;
						if (gsprite.SpriteType == GSpriteTypes.Monster && (gsprite.MonsterType == MonsterTypes.CaiJi || gsprite.MonsterType == MonsterTypes.CaiJiByTime))
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite.Coordinate, 250.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite.RoleID,
										SpriteType = gsprite.SpriteType,
										ShowDlg = false,
										ExtensionID = gsprite.ExtensionID
									});
									this.ChangeDirection(this.Leader, new Point(gsprite.Coordinate.X, gsprite.Coordinate.Y), -1);
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite.RoleID - 2130706432;
								this.LinearMoveByRunTo(gsprite.Coordinate, 60, 4, false, 2);
							}
							flag2 = false;
						}
						if (flag2 && this.LeftButtonClickOnSprite != null)
						{
							if (gsprite.SpriteType != GSpriteTypes.NPC)
							{
								this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
								{
									RoleID = gsprite.RoleID,
									SpriteType = gsprite.SpriteType,
									ShowDlg = false,
									ExtensionID = gsprite.ExtensionID
								});
							}
						}
						if (gsprite.SpriteType == GSpriteTypes.NPC)
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite.Coordinate, 300.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									if (gsprite != null)
									{
										gsprite.PlayNpcTalkSound();
									}
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite.RoleID,
										SpriteType = GSpriteTypes.NPC,
										ShowDlg = true,
										ExtensionID = gsprite.ExtensionID
									});
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite.RoleID - 2130706432;
								this.LinearMoveByRunTo(gsprite.Coordinate, 120, 1, false, 2);
							}
						}
						else if (gsprite.SpriteType == GSpriteTypes.FakeRole && (gsprite.FakeRoleType == FakeRoleTypes.DiaoXiang || gsprite.FakeRoleType == FakeRoleTypes.DiaoXiang2 || gsprite.FakeRoleType == FakeRoleTypes.DiaoXiang3 || this.SelectedSprite.FakeRoleType != FakeRoleTypes.CoupleWishMan || this.SelectedSprite.FakeRoleType != FakeRoleTypes.CoupleWishWife))
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite.Coordinate, 300.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									if (gsprite != null)
									{
										gsprite.PlayNpcTalkSound();
									}
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = 2130706432 + gsprite.ExtensionID,
										SpriteType = GSpriteTypes.NPC,
										ShowDlg = true,
										ExtensionID = gsprite.ExtensionID
									});
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite.ExtensionID;
								this.LinearMoveByRunTo(gsprite.Coordinate, 120, 1, false, 2);
							}
						}
					}
				}
			}
			else if (this.SelectedGoodsPack != null)
			{
				if (this.OnLeaderPreMove())
				{
					return;
				}
				if (this.GoodsPackNotify != null)
				{
					if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
					{
						return;
					}
					int num = this.SelectedGoodsPack.cx / this.CurrentMapData.GridSizeX;
					int num2 = this.SelectedGoodsPack.cy / this.CurrentMapData.GridSizeY;
					int num3 = this.Leader.cx / this.CurrentMapData.GridSizeX;
					int num4 = this.Leader.cy / this.CurrentMapData.GridSizeY;
					if (num != num3 || num2 != num4)
					{
						this.LinearMoveByRunTo(this.SelectedGoodsPack.Coordinate, 0, 3, false, 2);
					}
					else
					{
						GameInstance.Game.SpriteNotifyGetGoodsPack();
					}
				}
			}
			else
			{
				if (PlayZone.GlobalPlayZone != null)
				{
					PlayZone.GlobalPlayZone.HideAllFace();
				}
				this.SetLockDeco(this.LastSelectedSprite, false);
				this.SetLockTargetDeco(this.LastSelectedSprite, false);
			}
		}

		public void ExternalCallNpcDialog(int extensionID, int distance = 300)
		{
			string npcname = Global.GetNPCName(extensionID);
			GSprite gsprite = this.FindSprite(npcname);
			if (gsprite == null)
			{
				return;
			}
			if (gsprite.SpriteType != GSpriteTypes.NPC)
			{
				return;
			}
			if (gsprite != null)
			{
				string name = gsprite.Name;
				bool flag = false;
				if (name != null)
				{
					if (gsprite != null)
					{
						if (gsprite.SpriteType != GSpriteTypes.Other)
						{
							if (Global.WatchSprite == null)
							{
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name != Global.WatchSprite)
							{
								if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
								{
									this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
								}
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name == Global.WatchSprite)
							{
								if (this.Leader.LockObject == null)
								{
									flag = true;
								}
								else if (this.Leader.Action == GActions.Stand)
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							gsprite = this.FindSprite(Global.WatchSprite);
							if (gsprite != null && gsprite != this.Leader)
							{
								this.SetLockDeco(gsprite, true);
							}
						}
					}
					if (gsprite != null)
					{
						bool flag2 = true;
						if (flag2 && this.LeftButtonClickOnSprite != null)
						{
							this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
							{
								RoleID = gsprite.RoleID,
								SpriteType = gsprite.SpriteType,
								ShowDlg = false,
								ExtensionID = gsprite.ExtensionID
							});
						}
						if (gsprite.SpriteType == GSpriteTypes.NPC)
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite.Coordinate, (double)distance))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									if (gsprite != null)
									{
										gsprite.PlayNpcTalkSound();
									}
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite.RoleID,
										SpriteType = GSpriteTypes.NPC,
										ShowDlg = true,
										ExtensionID = gsprite.ExtensionID
									});
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite.RoleID - 2130706432;
								this.LinearMoveByRunTo(gsprite.Coordinate, 120, 1, false, 2);
							}
						}
					}
				}
			}
		}

		public void AutoTalk(bool isNPC, int radius, double angleLimit = 0.0)
		{
			if (Global.IsDongJieSprite(this.Leader))
			{
				return;
			}
			GSprite gsprite = null;
			if (Global.WatchSprite != null)
			{
				gsprite = this.FindSprite(Global.WatchSprite);
				gsprite = this.JugeSpriteInAngle(gsprite, radius);
				if (gsprite != null)
				{
					if (gsprite.SpriteType != GSpriteTypes.FakeRole)
					{
						if (gsprite.FakeRoleType != FakeRoleTypes.DiaoXiang && gsprite.FakeRoleType != FakeRoleTypes.DiaoXiang2 && gsprite.FakeRoleType != FakeRoleTypes.DiaoXiang3 && gsprite.FakeRoleType != FakeRoleTypes.CoupleWishMan && gsprite.FakeRoleType != FakeRoleTypes.CoupleWishWife)
						{
							gsprite = null;
						}
					}
					else if (gsprite.SpriteType != GSpriteTypes.NPC)
					{
						gsprite = null;
					}
				}
			}
			if (gsprite == null)
			{
				gsprite = this.SeekSpriteToLock(isNPC, radius);
			}
			if (gsprite == null)
			{
				return;
			}
			if (gsprite != null)
			{
				string name = gsprite.Name;
				bool flag = false;
				if (name != null)
				{
					GSprite gsprite2 = this.FindSprite(name);
					if (gsprite2 != null)
					{
						if (gsprite2.SpriteType != GSpriteTypes.Other)
						{
							if (Global.WatchSprite == null)
							{
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name != Global.WatchSprite)
							{
								if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
								{
									this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
								}
								Global.WatchSprite = name;
								flag = true;
							}
							else if (name == Global.WatchSprite)
							{
								if (this.Leader.LockObject == null)
								{
									flag = true;
								}
								else if (this.Leader.Action == GActions.Stand)
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							gsprite2 = this.FindSprite(Global.WatchSprite);
							if (gsprite2 != null && gsprite2 != this.Leader)
							{
								this.SetLockDeco(gsprite2, true);
							}
						}
					}
					if (gsprite2 != null)
					{
						bool flag2 = true;
						if (flag2 && this.LeftButtonClickOnSprite != null)
						{
							this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
							{
								RoleID = gsprite2.RoleID,
								SpriteType = gsprite2.SpriteType,
								ShowDlg = false,
								ExtensionID = gsprite2.ExtensionID
							});
						}
						if (gsprite2.SpriteType == GSpriteTypes.NPC)
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite2.Coordinate, 300.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									if (gsprite2 != null)
									{
										gsprite2.PlayNpcTalkSound();
									}
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite2.RoleID,
										SpriteType = GSpriteTypes.NPC,
										ShowDlg = true,
										ExtensionID = gsprite2.ExtensionID
									});
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite2.RoleID - 2130706432;
								this.LinearMoveByRunTo(gsprite2.Coordinate, 120, 1, false, 2);
							}
						}
						else if (gsprite2.SpriteType == GSpriteTypes.FakeRole && (gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang || gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang2 || gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang3 || gsprite.FakeRoleType != FakeRoleTypes.CoupleWishMan || gsprite.FakeRoleType != FakeRoleTypes.CoupleWishWife))
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite2.Coordinate, 300.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									if (gsprite2 != null)
									{
										gsprite2.PlayNpcTalkSound();
									}
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = 2130706432 + gsprite2.ExtensionID,
										SpriteType = GSpriteTypes.NPC,
										ShowDlg = true,
										ExtensionID = gsprite2.ExtensionID
									});
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite2.ExtensionID;
								this.LinearMoveByRunTo(gsprite2.Coordinate, 120, 1, false, 2);
							}
						}
					}
				}
			}
		}

		public void ExternalSkillAttack(int skillID, bool hint = false)
		{
			if (this.OnLeaderPreMove())
			{
				return;
			}
			if (Global.IsDongJieSprite(this.Leader))
			{
				return;
			}
			bool flag = Global.SkillCoolDown(skillID);
			if (flag)
			{
				return;
			}
			if (this.InputPublicCD(skillID))
			{
				return;
			}
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
			if (skillXmlNode != null)
			{
				int useType = skillXmlNode.UseType;
				if (useType == 1 && this.Leader.WeaponState == WeaponStates.K)
				{
					bool flag2 = this.Leader.ReCalcWeaponState();
					if (this.Leader.WeaponState == WeaponStates.K)
					{
						if (this.Leader.IsDeath)
						{
							if (hint)
							{
								Super.HintMainText(Global.GetLang("主角死亡还未复活！"), 10, 3);
							}
							return;
						}
						if (hint)
						{
							if (Global.GetMapShowEquipType(Global.Data.roleData) == 0)
							{
								Super.HintMainText(Global.GetLang("未佩戴武器无法释放该技能！"), 10, 3);
							}
							else
							{
								Super.HintMainText(Global.GetLang("未佩戴重生武器无法释放该技能！"), 10, 3);
							}
						}
						return;
					}
					else if (!flag2)
					{
						if (hint)
						{
							if (Global.GetMapShowEquipType(Global.Data.roleData) == 0)
							{
								Super.HintMainText(Global.GetLang("未佩戴武器无法释放该技能！"), 10, 3);
							}
							else
							{
								Super.HintMainText(Global.GetLang("未佩戴重生武器无法释放该技能！"), 10, 3);
							}
						}
						return;
					}
				}
				else if (useType == 1 && !this.Leader.ReCalcWeaponState())
				{
					if (hint)
					{
						if (Global.GetMapShowEquipType(Global.Data.roleData) == 0)
						{
							Super.HintMainText(Global.GetLang("未佩戴武器无法释放该技能！"), 10, 3);
						}
						else
						{
							Super.HintMainText(Global.GetLang("未佩戴重生武器无法释放该技能！"), 10, 3);
						}
					}
					return;
				}
			}
			if (skillXmlNode != null && !this.SkillNeedMagicVOk(skillID))
			{
				if (hint)
				{
					Super.HintMainText(Global.GetLang("当前的魔法值不足"), 10, 3);
				}
				return;
			}
			if (this.Leader.AnimationMgr != null && !this.Leader.AnimationMgr.IsContinuePlay(this.Leader.Action))
			{
				return;
			}
			int skillAttackGridNum = this.GetSkillAttackGridNum(skillID);
			int radius = skillAttackGridNum * this.CurrentMapData.GridSizeX - this.CurrentMapData.GridSizeX / 2;
			GSprite gsprite = null;
			if (skillXmlNode != null)
			{
				int targetType = skillXmlNode.TargetType;
				GSprite gsprite2 = this.FindSprite(Global.WatchSprite);
				if (this.SelectedSprite != null && this.SelectedSprite.The3DGameObject == null)
				{
					this.SelectedSprite = null;
				}
				switch (targetType)
				{
				case 1:
				case 2:
					gsprite = null;
					break;
				case 3:
				case 4:
					if (UICtrlBar.IsPress() && StoryBoard.ContainStoryBoard(this.Leader.Name))
					{
						return;
					}
					if (this.SelectedSprite != null)
					{
						if (Global.IsOpposition(this.Leader, this.SelectedSprite, false, this.CurrentMapData.PKMode) && this.SelectedSprite.VLife > 0.0)
						{
							gsprite = this.SelectedSprite;
						}
						else
						{
							this.SetLockDeco(this.SelectedSprite, false);
							this.SelectedSprite = null;
						}
					}
					if (this.SelectedSprite == null)
					{
						gsprite = this.SeekSpriteToLock(false, radius);
						if (gsprite != null && gsprite.VLife <= 0.0)
						{
							gsprite = null;
						}
					}
					if (gsprite == null && gsprite2 != null)
					{
						if (targetType == 4)
						{
							gsprite = gsprite2;
							if (!Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode))
							{
								gsprite = null;
								this.SetLockDeco(gsprite2, false);
								if (PlayZone.GlobalPlayZone != null)
								{
									PlayZone.GlobalPlayZone.HideAllFace();
								}
								Global.WatchSprite = null;
							}
						}
						else
						{
							this.SetLockDeco(gsprite2, false);
							if (PlayZone.GlobalPlayZone != null)
							{
								PlayZone.GlobalPlayZone.HideAllFace();
							}
							Global.WatchSprite = null;
						}
					}
					else if (gsprite != null && gsprite2 != gsprite)
					{
						this.SetLockDeco(gsprite2, false);
						if (PlayZone.GlobalPlayZone != null)
						{
							PlayZone.GlobalPlayZone.HideAllFace();
						}
						Global.WatchSprite = null;
					}
					break;
				}
			}
			this.AutoSelectSkillID = skillID;
			GSprite gsprite3 = null;
			if (gsprite != null)
			{
				string name = gsprite.Name;
				bool flag3 = false;
				if (name != null)
				{
					gsprite3 = this.FindSprite(name);
					if (gsprite3 != null)
					{
						if (Global.WatchSprite == null)
						{
							Global.WatchSprite = name;
							flag3 = true;
						}
						else if (name != Global.WatchSprite)
						{
							if (Global.WatchSprite != null && Global.WatchSprite != this.Leader.Name)
							{
								this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
							}
							Global.WatchSprite = name;
							flag3 = true;
						}
						else if (name == Global.WatchSprite)
						{
							if (this.Leader.LockObject == null)
							{
								flag3 = true;
							}
							else if (this.Leader.Action == GActions.Stand)
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							gsprite3 = this.FindSprite(Global.WatchSprite);
							if (gsprite3 != null && gsprite3 != this.Leader)
							{
								this.SetLockDeco(gsprite3, true);
							}
						}
					}
					if (this.Leader.LockObject != null)
					{
						flag3 = true;
					}
					if (gsprite3 != null)
					{
						bool flag4 = true;
						if (gsprite3.SpriteType == GSpriteTypes.Monster && (gsprite3.MonsterType == MonsterTypes.CaiJi || gsprite3.MonsterType == MonsterTypes.CaiJiByTime))
						{
							if (Global.InCircle(this.Leader.Coordinate, gsprite3.Coordinate, 100.0))
							{
								if (this.LeftButtonClickOnSprite != null)
								{
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite3.RoleID,
										SpriteType = gsprite3.SpriteType,
										ShowDlg = false,
										ExtensionID = gsprite3.ExtensionID
									});
									this.ChangeDirection(this.Leader, new Point(gsprite3.Coordinate.X, gsprite3.Coordinate.Y), -1);
								}
							}
							else
							{
								Global.Data.TargetNpcID = gsprite3.RoleID - 2130706432;
								this.LinearMoveByRunTo(gsprite3.Coordinate, 60, 4, false, 2);
							}
							flag4 = false;
						}
						if (flag4 && this.LeftButtonClickOnSprite != null)
						{
							MonsterData monsterData = null;
							if (Global.Data.SystemMonsters.TryGetValue(gsprite3.RoleID, ref monsterData))
							{
								if (Global.Data.roleData.RoleID != monsterData.MasterRoleID)
								{
									this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite3.RoleID,
										SpriteType = gsprite3.SpriteType,
										ShowDlg = false,
										ExtensionID = gsprite3.ExtensionID
									});
								}
							}
							else
							{
								this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
								{
									RoleID = gsprite3.RoleID,
									SpriteType = gsprite3.SpriteType,
									ShowDlg = false,
									ExtensionID = gsprite3.ExtensionID
								});
							}
						}
					}
				}
				if (flag3 && gsprite3 != null && gsprite3.VLife > 0.0)
				{
					bool flag5 = false;
					if (gsprite3.SpriteType == GSpriteTypes.Monster)
					{
						flag5 = Global.IsOpposition(this.Leader, gsprite3, false, this.CurrentMapData.PKMode);
						if (!flag5 && Global.IsZhaoHuanShou(gsprite3.RoleID))
						{
							this.SetLockDeco(gsprite3, false);
						}
					}
					else if (gsprite3.SpriteType == GSpriteTypes.Other)
					{
						flag5 = Global.IsOpposition(this.Leader, gsprite3, false, this.CurrentMapData.PKMode);
					}
					else if (gsprite3.SpriteType == GSpriteTypes.NPC)
					{
						flag5 = false;
					}
					else if (gsprite3.SpriteType != GSpriteTypes.Pet)
					{
						if (gsprite3.SpriteType == GSpriteTypes.BiaoChe)
						{
							flag5 = Global.IsOpposition(this.Leader, gsprite3, false, this.CurrentMapData.PKMode);
						}
						else if (gsprite3.SpriteType == GSpriteTypes.JunQi)
						{
							flag5 = Global.IsOpposition(this.Leader, gsprite3, false, this.CurrentMapData.PKMode);
						}
						else if (gsprite3.SpriteType == GSpriteTypes.FakeRole)
						{
							flag5 = Global.IsOpposition(this.Leader, gsprite3, false, this.CurrentMapData.PKMode);
						}
						else if (gsprite3.SpriteType == GSpriteTypes.Leader)
						{
						}
					}
					if (flag5)
					{
						string lockObject = this.Leader.LockObject;
						this.Leader.LockObject = Global.WatchSprite;
						if (this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic)
						{
							int num = (int)this.CalcDirection(this.Leader, gsprite3.Coordinate);
							this.Leader.Rotation = Global.GetRotationByTwoPoint(this.Leader.Coordinate, gsprite3.Coordinate);
							this.Leader.EnemyTarget = gsprite3.Coordinate;
							this.MU_Attack((double)num, this.Leader.YAngle, this.Leader.Coordinate, this.Leader.EnemyTarget, skillAttackGridNum, skillID, false, false);
						}
						else
						{
							this.ExternalWaitingSkillID = skillID;
						}
					}
				}
			}
			else
			{
				if (gsprite3 != null && gsprite3 != this.Leader)
				{
					this.SetLockDeco(gsprite3, false);
					Global.WatchSprite = null;
				}
				if (this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic)
				{
					int direction = this.Leader.Direction;
					this.Leader.EnemyTarget = this.GetAttackPointByDir(this.Leader.Coordinate, 2, direction);
					this.Leader.LockObject = null;
					this.MU_Attack((double)direction, this.Leader.YAngle, this.Leader.Coordinate, this.Leader.EnemyTarget, skillAttackGridNum, skillID, false, true);
				}
				else
				{
					this.ExternalWaitingSkillID = skillID;
				}
			}
		}

		private bool InputPublicCD(int skillID)
		{
			if (skillID == 100 || skillID == 200 || skillID == 300)
			{
				return false;
			}
			if (this.Leader != null)
			{
				if (this.BeforeSkillTime == 0L)
				{
					this.BeforeSkillTime = DateTime.Now.Ticks;
				}
				else
				{
					float skillPubCDByID = ConfigMagicInfos.GetSkillPubCDByID(skillID);
					if ((float)(DateTime.Now.Ticks - this.BeforeSkillTime) <= skillPubCDByID)
					{
						return true;
					}
					this.BeforeSkillTime = DateTime.Now.Ticks;
				}
			}
			return false;
		}

		private void OnFrameEvents()
		{
			if (Global.DisableInput)
			{
				return;
			}
			this.UpdateLeaderDirection(false);
			this.UpdateSelectedObject();
		}

		public GSprite LoadJunQi(JunQiData junQiData)
		{
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				junQiData.JunQiID
			});
			GSprite gsprite = new GSprite();
			gsprite.SpriteType = GSpriteTypes.JunQi;
			gsprite.CoordinateChanged += delegate(GSprite sender)
			{
				this.UpdateJunQiEvent(sender);
			};
			gsprite.SpriteDead += delegate(object s, EventArgs e)
			{
				if (this.SpriteDeadNotify != null)
				{
					this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
					{
						RoleID = (s as GSprite).RoleID,
						SpriteType = (s as GSprite).SpriteType,
						ShowDlg = false,
						ExtensionID = (s as GSprite).ExtensionID
					});
				}
				if ((s as GSprite).Name == Global.WatchSprite)
				{
					this.SetLockDeco(s as GSprite, false);
					if (this.Leader.LockObject == Global.WatchSprite)
					{
						this.Leader.LockObject = null;
					}
					Global.WatchSprite = null;
				}
			};
			this.LoadSprite(gsprite, junQiData.JunQiID, 0, name, string.Empty, string.Empty, Global.FormatBangHuiName(junQiData.ZoneID, junQiData.QiName), 0, junQiData.JunQiLevel, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, Global.Data.SnameBrushColor, (double)junQiData.CurrentLifeV, 0, 0, -1.0, junQiData.BodyCode, -1, new Point(junQiData.PosX, junQiData.PosY), junQiData.Direction, (double)Global.Data.LifeTotalWidth, 1.0, 0, true);
			gsprite.VLifeMax = (double)junQiData.LifeV;
			this.RefreshSpriteLife(gsprite);
			gsprite.Start();
			return gsprite;
		}

		private void UpdateJunQiEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				Global.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
		}

		private void LoadLeader(int x, int y, int direction, bool newLifeDeco = true)
		{
			GSprite pet = null;
			GSprite biaoChe = null;
			int direction2 = direction;
			GDecoration gdecoration = null;
			try
			{
				if (this.Leader != null)
				{
					pet = this.Leader.Pet;
					direction2 = this.Leader.Direction;
					biaoChe = this.Leader.BiaoChe;
					if (this.Leader.GDecorationEmblem != null)
					{
						gdecoration = this.Leader.GDecorationEmblem.Clone();
					}
					this.Leader.Destroy();
					this.Leader = null;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<string>(new string[]
				{
					"LoadLeader RemoveOldObject"
				});
				MUDebug.LogException(ex);
			}
			this.Leader = new GSprite();
			this.Leader.BattleWhichSide = Global.Data.roleData.BattleWhichSide;
			this.Leader.Pet = pet;
			this.Leader.BiaoChe = biaoChe;
			this.Leader.SpriteChangeAction += delegate(object s, SpriteChangeActionEventArgs e)
			{
				this.ProcessLeaderChangeAction(s, e);
			};
			this.Leader.SpriteDead += delegate(object s, EventArgs e)
			{
				this.ProcessLeaderDead(s, e);
			};
			this.Leader.CoordinateChanged += delegate(GSprite s)
			{
				this.UpdateLeaderEvent(s);
			};
			try
			{
				this.LoadSprite(this.Leader, Global.Data.RoleID, Global.Data.RoleSex, "Leader", Global.GetRoleBHName(Global.Data.roleData), Global.Data.roleData.OtherName, Global.Data.roleData.RoleName, Global.Data.roleData.Occupation, -1, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, Global.Data.SnameBrushColor, (double)Global.Data.roleData.LifeV, Global.Data.roleData.PKMode, Global.Data.roleData.PKValue, -1.0, Global.Data.roleData.BodyCode, Global.Data.roleData.WeaponCode, new Point(x, y), direction2, (double)Global.Data.LifeTotalWidth, 1.0, Global.Data.roleData.Faction, true);
				this.Leader.PKKingSpriteName = Global.GetPKKingSpriteName(Global.Data.roleData);
				this.Leader.VLifeMax = (double)Global.Data.roleData.MaxLifeV;
				this.Leader.VArmor = (long)Global.Data.roleData.CurrentArmorV;
				this.Leader.VArmorMax = (long)Global.Data.roleData.MaxArmorV;
				this.RefreshSpriteLife(this.Leader);
				this.RefreshRoleArmor(this.Leader);
				this.UpdateNameColor(this.Leader, Global.Data.roleData);
				this.Leader.VPK = Global.Data.roleData.PKValue;
				this.UpdatePKValue(Global.Data.roleData, this.Leader);
				this.UpdateTeamFlags(this.Leader, Global.Data.roleData.TeamID > 0, Global.Data.roleData.TeamLeaderRoleID == this.Leader.RoleID);
				this.UpdateSkillWord(this.Leader, Global.Data.roleData);
				this.UpdateLittleVIP(this.Leader, Global.Data.roleData);
				this.UpdateJingMaiWord(this.Leader, Global.Data.roleData);
				this.UpdateBattleNameImage(this.Leader, Global.Data.roleData);
				this.UpdateHeroIndexImage(this.Leader, Global.Data.roleData);
				this.UpdateChengJiuImage(this.Leader, Global.Data.roleData);
				this.UpdateChuanQiJingMaiImage(this.Leader, Global.Data.roleData);
				this.UpdateChuanWuXueImage(this.Leader, Global.Data.roleData);
				this.UpdateHuangChengImage(this.Leader, Global.Data.roleData);
				this.UpdateWangChengImage(this.Leader, Global.Data.roleData);
				this.UpdateLingDiWord(this.Leader, Global.Data.roleData);
				this.UpdateBattleSideImage(this.Leader, Global.Data.roleData);
				this.UpdateJieriWord(this.Leader, Global.Data.roleData);
				this.UpdateMarketName(this.Leader, Global.Data.roleData);
				this.AddMingZhongDec(this.Leader, Global.Data.roleData);
			}
			catch (Exception ex2)
			{
				MUDebug.LogException(ex2);
			}
			this.Leader.Start();
			this.ContinueAutoFindRoadDeco();
			this.AddAutoFightDeco();
			Global.AddSpriteJingMaiUpDeco(this.Leader, Global.Data.roleData);
			this.AddHuangDiDec(this.Leader);
			this.AddHuangHouDec(this.Leader, Global.Data.roleData);
			this.AddFSHuDunDec(this.Leader, Global.Data.roleData);
			this.AddZhongDuStatus(this.Leader, Global.Data.roleData);
			this.AddDSHide(this.Leader, Global.Data.roleData);
			this.AddMaBiStatus(this.Leader, Global.Data.roleData);
			if (gdecoration != null)
			{
				this.Leader.GDecorationEmblem = gdecoration;
			}
			else
			{
				Global.RoleEmblemClick(1);
				this.AddOrRemoveEmblemDeco(this.Leader, 0);
			}
			if (!this.JugeSafeRegion())
			{
				this.Leader.ChangeWeaponsPosition(this.IsInSafeRegion);
				this.Leader.ChangeAction(this.Leader.Action);
			}
			this.AddSpriteLeadDeco(this.Leader);
		}

		public void ResetLeaderJueXingTeXiao()
		{
			this.AddSpriteAllJueXingTeXiao(this.Leader, Global.Data.roleData.RoleID);
		}

		private void ProcessLeaderDead(object s, EventArgs e)
		{
			if (Global.WatchSprite != null)
			{
				this.SetLockDeco(this.FindSprite(Global.WatchSprite), false);
			}
			this.HideMouseLeftButtonUpEffect();
			if (this.SpriteDeadNotify != null)
			{
				this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
				{
					RoleID = this.Leader.RoleID,
					SpriteType = this.Leader.SpriteType,
					ShowDlg = false,
					ExtensionID = this.Leader.ExtensionID
				});
			}
			Global.Data.roleData.ZhongDuStart = 0L;
			Global.Data.roleData.ZhongDuSeconds = 0;
			GSprite gsprite = s as GSprite;
			if (gsprite != null && gsprite.shouHuChongController != null)
			{
				gsprite.shouHuChongController.Dispose();
				Object.Destroy(gsprite.shouHuChongController);
				gsprite.shouHuChongController = null;
			}
			this.AddSpriteDeadDeco(s as GSprite);
		}

		protected void ProcessLeaderChangeAction(object s, SpriteChangeActionEventArgs e)
		{
			if (e.Action == 0)
			{
				Point coordinate = (s as GSprite).Coordinate;
				GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, e.Action, coordinate, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
			}
			else if (e.Action == 25)
			{
				Point coordinate2 = (s as GSprite).Coordinate;
				(s as GSprite).Action = GActions.Stand;
				GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate2, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
			}
			else if (e.Action == 4)
			{
				Point coordinate3 = (s as GSprite).Coordinate;
				(s as GSprite).Action = GActions.Stand;
				GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate3, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
			}
			else if (e.Action == 3)
			{
				(s as GSprite).Action = GActions.Stand;
				Global.RemoveStoryboard((s as GSprite).Name);
				if (this.ExternalWaitingSkillID > 0)
				{
					if (this.Leader.LockObject == null)
					{
						Vector2 zero = Vector2.zero;
						int input36Direction = Global.GetInput36Direction(Global.Joystick, out zero);
						if (input36Direction >= 0 && (s as GSprite).LastJoyPosition != Vector2.zero)
						{
							(s as GSprite).Direction = (s as GSprite).WaitingDirection;
							(s as GSprite).Rotation = Quaternion.LookRotation(new Vector3((s as GSprite).LastJoyPosition.x, 0f, (s as GSprite).LastJoyPosition.y), Vector3.up);
							(s as GSprite).The3DGameObject.transform.localRotation = (s as GSprite).Rotation;
						}
					}
					this.DoAttackOK = false;
					this.ExternalSkillAttack(this.ExternalWaitingSkillID, false);
					this.ExternalWaitingSkillID = -1;
				}
				else
				{
					Point coordinate4 = (s as GSprite).Coordinate;
					GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate4, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
				}
			}
			else if (e.Action == 6)
			{
				(s as GSprite).Action = GActions.Stand;
				if (this.IsContinuationSkill(s as GSprite))
				{
					return;
				}
				if (this.ExternalWaitingSkillID > 0)
				{
					if (this.Leader.LockObject == null)
					{
						Vector2 zero2 = Vector2.zero;
						int input36Direction2 = Global.GetInput36Direction(Global.Joystick, out zero2);
						if (input36Direction2 >= 0 && (s as GSprite).LastJoyPosition != Vector2.zero)
						{
							(s as GSprite).Direction = (s as GSprite).WaitingDirection;
							(s as GSprite).Rotation = Quaternion.LookRotation(new Vector3((s as GSprite).LastJoyPosition.x, 0f, (s as GSprite).LastJoyPosition.y), Vector3.up);
							(s as GSprite).The3DGameObject.transform.localRotation = (s as GSprite).Rotation;
						}
					}
					this.DoAttackOK = false;
					this.ExternalSkillAttack(this.ExternalWaitingSkillID, false);
					this.ExternalWaitingSkillID = -1;
					if (!this.DoAttackOK)
					{
						Point coordinate5 = (s as GSprite).Coordinate;
						GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate5, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
					}
				}
				else
				{
					Point coordinate6 = (s as GSprite).Coordinate;
					GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate6, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
				}
			}
			else if (e.Action == 28 || e.Action == 29 || e.Action == 30 || e.Action == 31 || e.Action == 32 || e.Action == 33 || e.Action == 34)
			{
				Point coordinate7 = (s as GSprite).Coordinate;
				(s as GSprite).Action = GActions.Stand;
				GameInstance.Game.SpriteAction((double)(s as GSprite).Direction, 0, coordinate7, (s as GSprite).EnemyTarget, (s as GSprite).YAngle, new Point(0, 0));
			}
		}

		private bool IsContinuationSkill(GSprite gSprite)
		{
			if (gSprite.MagicID != -1)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(gSprite.MagicID);
				if (skillXmlNode != null)
				{
					int nextMagicID = skillXmlNode.NextMagicID;
					if (nextMagicID == -1)
					{
						gSprite.MagicID = -1;
						return false;
					}
					this.ExternalSkillAttack(nextMagicID, false);
					this.ExternalWaitingSkillID = -1;
					return true;
				}
			}
			gSprite.MagicID = -1;
			return false;
		}

		private void UpdateLeaderEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			int myTimer = Global.GetMyTimer();
			this.LastEventTicks = myTimer;
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = this.Leader.cx / this.CurrentMapData.GridSizeX;
			int num2 = this.Leader.cy / this.CurrentMapData.GridSizeY;
			if (num != this.Leader.OldGridX || num2 != this.Leader.OldGridY)
			{
				this.CurrentMapData._MapGrid.MoveObjectEx(this.Leader.OldGridX, this.Leader.OldGridY, num, num2, this.Leader);
				this.Leader.OldGridX = num;
				this.Leader.OldGridY = num2;
				this.JugeSafeRegion();
			}
			int teleportKey = (int)this.CurrentMapData.TerrainWithTeleports[num, num2];
			if (teleportKey >= 10 && teleportKey < 100)
			{
				if (this.LastTeleportKey != teleportKey)
				{
					this.LastTeleportKey = teleportKey;
					if (this.CanMapConversionByTeleportCode(teleportKey))
					{
						if (Global.CanGuanZhan() && ZhanMengLianSaiGuanZhanPopupList.IsTracking)
						{
							return;
						}
						if (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan)
						{
							return;
						}
						if (!Global.Data.WaitingForMapChange && Global.CanBeTransport(Global.Data.roleData.MapCode, teleportKey))
						{
							sprite.Action = GActions.Stand;
							GTeleport gteleport = this.FindName(StringUtil.substitute("Teleport{0}", new object[]
							{
								teleportKey
							})) as GTeleport;
							int to = gteleport.To;
							Super.HideNetWaiting();
							if (Global.IsPopupDownloadMapWindow(to))
							{
								string[] buttons = new string[]
								{
									Global.GetLang("立即下载"),
									Global.GetLang("稍后下载")
								};
								string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(to));
								Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
								{
									if (e1.ID == 0)
									{
										Global.RemoveStoryboard(sprite.Name);
										this.MapConversionByTeleportCode(teleportKey);
									}
								}, buttons);
							}
							else
							{
								Global.RemoveStoryboard(sprite.Name);
								this.MapConversionByTeleportCode(teleportKey);
							}
							return;
						}
					}
				}
			}
			else
			{
				this.LastTeleportKey = -1;
			}
			this.ActiveTreatment(this.Leader);
		}

		private void SendClientHeart()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this.LastClientHearTicks >= 60000L)
			{
				this.LastClientHearTicks = correctLocalTime;
				GameInstance.Game.SpriteHeart();
			}
		}

		public void RelifeOnOriginalPoint()
		{
			GSprite gsprite = this.FindSprite("Leader");
			if (gsprite == null || gsprite.VLife <= 0.0)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SpriteRealive(-1, -1, (int)this.Direction);
			}
			else
			{
				PlayZone.GlobalPlayZone.CloseFuhuo();
			}
		}

		public void RelifeOnLeaderPoint()
		{
			GSprite gsprite = this.FindSprite("Leader");
			if (gsprite == null || gsprite.VLife <= 0.0)
			{
				GameInstance.Game.SpriteRealive(this.Leader.Coordinate.X, this.Leader.Coordinate.Y, this.Leader.Direction);
			}
			else
			{
				PlayZone.GlobalPlayZone.CloseFuhuo();
			}
		}

		public GSprite GetLeader()
		{
			return this.Leader;
		}

		public void StopLeaderMoving()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.Leader.LockObject = null;
			this.CancelAutoFight(0, true);
			this.CancelAutoFindRoad(true);
			if (Global.FindMoveStroyboard(this.Leader.Name))
			{
				Global.RemoveStoryboard(this.Leader.Name);
				GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
			}
		}

		public bool CanLeaderChangeLineByAttack()
		{
			return this.Leader != null && this.Leader.GetElapsedLastAttackTicks() > 30000L;
		}

		public bool CanLeaderChangeLineByInjured()
		{
			return this.Leader != null && Global.GetCorrectLocalTime() - this.Leader.LastInjuredTicks > 30000L;
		}

		public void ClearScene()
		{
			this.EnableChangMap = false;
			if (this.Leader != null)
			{
				Global.RemoveStoryboard(this.Leader.Name);
			}
			if (this.Leader != null && this.Leader.Pet != null)
			{
				Global.RemoveStoryboard(this.Leader.Pet.Name);
				this.Leader.Pet.Action = GActions.Stand;
				this.Leader.Pet = null;
			}
			if (this.Leader != null && this.Leader.BiaoChe != null)
			{
				Global.RemoveStoryboard(this.Leader.BiaoChe.Name);
				this.Leader.BiaoChe.Action = GActions.Stand;
				this.Leader.BiaoChe = null;
			}
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				IObject @object = objectsList[i];
				if (@object != this.Leader)
				{
					Global.RemoveObject(@object, false);
				}
			}
			ObjectsManager.Clear();
			try
			{
				StoryBoard.ClearStoryBoard();
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			foreach (KeyValuePair<int, GSprite> keyValuePair in this.LoadingSpriteCacheDict)
			{
				GSprite value = keyValuePair.Value;
				Global.RemoveObject(value, false);
			}
			this.LoadingSpriteCacheDict.Clear();
			if (this.LeftButtonClickOnSprite != null)
			{
				this.LeftButtonClickOnSprite(this, new SpriteNotifyEventArgs
				{
					RoleID = -1,
					SpriteType = GSpriteTypes.Leader,
					ShowDlg = false,
					ExtensionID = -1
				});
			}
			if (this.CurrentMapData != null)
			{
				this.ClearMapData(this.CurrentMapData);
			}
			if (this.Leader != null)
			{
				this.Leader.DestroyShouHuChong();
			}
			this.Leader = null;
			this.Direction = 0.0;
			Global.Data.OtherRoles.Clear();
			Global.Data.OtherRolesByName.Clear();
			Global.Data.SystemMonsters.Clear();
			this.LeftMouseClickDeco = null;
			this.CurDestinationPos = new Point(0, 0);
			this.LeftButtonDown = false;
			this.LastUpdateUITicks = 0L;
			this.LastUpdateUILeaderX = -1.0;
			this.LastUpdateRadarTicks = 0L;
			this.LastMoveMiniMapTicks = 0L;
			this.LeaderReportPos = new Point(0, 0);
			this.LeaderBiaoCheReportPos = new Point(0, 0);
			this.MonsterItemList.Clear();
			this.RoleItemList.Clear();
			this.SelectedSprite = null;
			this.SelectedGoodsPack = null;
			this.LastShiftTicks = 0.0;
			this.MapShakingCount = 0;
			this.MapCode = -1;
			this.MapPicCode = -1;
			this.CurrentMapData = null;
			this.SectionCenter = new Point(0, 0);
			this.ToOpenGoodsPack = null;
			this.IsInSafeRegion = false;
			this.AutoSelectSkillID = -1;
			this.FirstJugeSafeRegion = true;
			this.LastTeleportKey = -1;
			this.pathFinderFast = null;
		}

		public void LoadScene(int mapCode, double leaderX, double leaderY, double direction, bool newLifeDeco)
		{
			this.MapCode = mapCode;
			this.MapPicCode = ConfigSettings.GetMapPicCodeByCode(this.MapCode);
			this.EnableChangMap = true;
			this.Direction = direction;
			this.LoadMapData();
			this.InitTerrainObject();
			this.LoadLeader((int)leaderX, (int)leaderY, (int)this.Direction, newLifeDeco);
		}

		private void InitTerrainObject()
		{
			if (null == Global.CurrentTerrainLoader)
			{
				return;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(null, "/Directional light");
			if (null != gameObject)
			{
				U3DUtils.ModifyDirectLight(gameObject, 1 << LayerMask.NameToLayer("Sprites") | 1 << LayerMask.NameToLayer("TargetCamera") | 1 << LayerMask.NameToLayer("SelfCamera"));
			}
			Global.CurrentTerrainLoader.Unload(false);
			Global.CurrentTerrainLoader = null;
			ZoneLoader.DisableSliceTerrain = (ConfigSettings.GetMapSliceTerrainByCode(this.MapPicCode) <= 0);
			if (!ZoneLoader.DisableSliceTerrain)
			{
				ZoneLoader singleton = ZoneLoader.singleton;
				singleton.m_GridSize = (float)(Global.CurrentMapData.MapWidth / 4) / 100f;
				singleton.MapName = ConfigSettings.GetMap3DResNameByCodeWithoutExt(Global.Data.roleData.MapCode);
				singleton.m_PlayerPosition = new Vector3((float)Global.Data.roleData.PosX / 100f, 50f, (float)Global.Data.roleData.PosY / 100f);
			}
		}

		private void LoadMapData()
		{
			this.CurrentMapData = new GMapData();
			Global.CurrentMapData = this.CurrentMapData;
			this.LoadObstruction();
			XElement xml = this.LoadMapXML();
			this.LoadMapSections(xml);
			this.LoadMapConfig();
			this.InitTeleports();
			this.InitGuangMus();
		}

		private void LoadMapConfig()
		{
			string text = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(this.MapCode, text);
			if (gameMapSettingsXml == null)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("加载地图配置文件失败: {0}"), text));
				return;
			}
			XElement xelement = XmlManager.GetXElement(gameMapSettingsXml, "Settings");
			if (xelement == null)
			{
				return;
			}
			this.CurrentMapData.PKMode = XmlManager.GetXElementAttributeInt(xelement, "PKMode");
			this.CurrentMapData.NotLostEquip = XmlManager.GetXElementAttributeInt(xelement, "NotLostEquip");
			this.CurrentMapData.IsolatedMap = XmlManager.GetXElementAttributeInt(xelement, "IsolatedMap");
			this.CurrentMapData.HoldRole = Global.GetXElementAttributeInt(xelement, "HoldRole");
			this.CurrentMapData.HoldMonster = Global.GetXElementAttributeInt(xelement, "HoldMonster");
			this.CurrentMapData.HoldNPC = Global.GetXElementAttributeInt(xelement, "HoldNPC");
			xelement = XmlManager.GetXElement(gameMapSettingsXml, "Limits");
			if (xelement != null)
			{
				this.CurrentMapData.LimitMagicIDs = ConvertExt.String2IntList(XmlManager.GetXElementAttributeStr(xelement, "MagicIDs"), ',');
				this.CurrentMapData.LimitGoodsIDs = ConvertExt.String2IntList(XmlManager.GetXElementAttributeStr(xelement, "GoodsIDs"), ',');
				this.CurrentMapData.MinZhuanSheng = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				this.CurrentMapData.MinLevel = Global.GetXElementAttributeInt(xelement, "MinLevel");
			}
		}

		private XElement LoadMapXML()
		{
			string xmlName = StringUtil.substitute("Mask.xml", new object[0]);
			int mapPicCodeByCode = ConfigSettings.GetMapPicCodeByCode(this.MapCode);
			XElement gameMapXml = Global.GetGameMapXml(mapPicCodeByCode, xmlName);
			int num = 0;
			string text = string.Empty;
			this.CurrentMapData.GameObjectList = new List<GMapItem>();
			XElement xelement = Global.GetXElement(gameMapXml, "GameObjects");
			if (xelement != null)
			{
				List<XElement> xelementList = Global.GetXElementList(xelement, "*");
				foreach (XElement xelement2 in xelementList)
				{
					GMapItem gmapItem = new GMapItem();
					gmapItem.ObjName = Global.GetXElementAttributeStr(xelement2, "name");
					text = Global.GetXElementAttributeStr(xelement2, "transform");
					string[] array = text.Split(new char[]
					{
						','
					});
					string[] array2 = array[0].Split(new char[]
					{
						'_'
					});
					gmapItem.Pos_x = (double)float.Parse(array2[0]);
					gmapItem.Pos_y = (double)float.Parse(array2[1]);
					gmapItem.Pos_z = (double)float.Parse(array2[2]);
					array2 = array[1].Split(new char[]
					{
						'_'
					});
					gmapItem.Rot_x = (double)float.Parse(array2[0]);
					gmapItem.Rot_y = (double)float.Parse(array2[1]);
					gmapItem.Rot_z = (double)float.Parse(array2[2]);
					array2 = array[2].Split(new char[]
					{
						'_'
					});
					gmapItem.Sca_x = (double)float.Parse(array2[0]);
					gmapItem.Sca_y = (double)float.Parse(array2[1]);
					gmapItem.Sca_z = (double)float.Parse(array2[2]);
					gmapItem.Index = num;
					this.CurrentMapData.GameObjectList.Add(gmapItem);
					num++;
				}
			}
			return gameMapXml;
		}

		private void ClearMapData(GMapData mapData)
		{
			mapData.SectionList = new List<Point2Section>();
			mapData.MapSections = new Dictionary<string, GMapSection>();
			for (int i = 0; i < 25; i++)
			{
				Point2Section point2Section = new Point2Section();
				point2Section.sectionNo = i;
				mapData.SectionList.Add(point2Section);
			}
		}

		private void LoadMapSections(XElement xml)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xml, "Width");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xml, "Height");
			if (xelementAttributeInt != this.CurrentMapData.MapWidth || xelementAttributeInt2 != this.CurrentMapData.MapHeight)
			{
				return;
			}
			this.CurrentMapData.SectionWidth = Global.GetXElementAttributeInt(xml, "SectWith");
			this.CurrentMapData.SectionHeight = Global.GetXElementAttributeInt(xml, "SectHeight");
			if (this.CurrentMapData.SectionWidth == 0 || this.CurrentMapData.SectionHeight == 0)
			{
				return;
			}
			this.CurrentMapData.SectionXNum = this.CurrentMapData.MapWidth / this.CurrentMapData.SectionWidth;
			this.CurrentMapData.SectionZNum = this.CurrentMapData.MapHeight / this.CurrentMapData.SectionHeight;
			this.ClearMapData(this.CurrentMapData);
			this.CurrentMapData.MapSections = new Dictionary<string, GMapSection>();
			string text = string.Empty;
			XElement xelement = Global.GetXElement(xml, "Sections");
			List<XElement> xelementList = Global.GetXElementList(xelement, "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement2 = xelementList[i];
				GMapSection gmapSection = new GMapSection();
				gmapSection.X = Global.GetXElementAttributeInt(xelement2, "X");
				gmapSection.Y = Global.GetXElementAttributeInt(xelement2, "Z");
				text = Global.GetXElementAttributeStr(xelement2, "GameObjects");
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					gmapSection.Indexes = new List<int>();
					for (int j = 0; j < array.Length; j++)
					{
						gmapSection.Indexes[j] = Convert.ToInt32(array[j]);
					}
				}
				string text2 = StringUtil.substitute("{0}_{1}", new object[]
				{
					gmapSection.X,
					gmapSection.Y
				});
				this.CurrentMapData.MapSections[text2] = gmapSection;
			}
		}

		private void LoadObstruction()
		{
			AssetBundle currentMapLoader = AssetBundleManager.CurrentMapLoader;
			if (null == currentMapLoader)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					new Exception(string.Format("GetGameMapXml异常, 缓存中没找到", new object[0]))
				});
				return;
			}
			TextAsset textAsset = (TextAsset)currentMapLoader.LoadAsset("obs.bytes");
			if (null == textAsset)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					new Exception(string.Format("GetGameMapXml异常, 缓存中没找到 obs.bytes", new object[0]))
				});
				return;
			}
			byte[] bytes = textAsset.bytes;
			int num = (int)bytes[0] | (int)bytes[1] << 8 | (int)bytes[2] << 16 | (int)bytes[3] << 24;
			int num2 = (int)bytes[4] | (int)bytes[5] << 8 | (int)bytes[6] << 16 | (int)bytes[7] << 24;
			this.CurrentMapData.MapWidth = num * 100;
			this.CurrentMapData.MapHeight = num2 * 100;
			this.CurrentMapData._MapGrid = new MapGrid(this.MapCode, this.CurrentMapData.MapWidth, this.CurrentMapData.MapHeight, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY, this.CurrentMapData);
			this.CurrentMapData.GridSizeX = 100;
			this.CurrentMapData.GridSizeY = 100;
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num3 = (this.CurrentMapData.MapWidth - 1) / this.CurrentMapData.GridSizeX + 1;
			int num4 = (this.CurrentMapData.MapHeight - 1) / this.CurrentMapData.GridSizeY + 1;
			num3 = (int)Math.Ceiling(Math.Log((double)num3, 2.0));
			num3 = (int)Math.Pow(2.0, (double)num3);
			num4 = (int)Math.Ceiling(Math.Log((double)num4, 2.0));
			num4 = (int)Math.Pow(2.0, (double)num4);
			this.CurrentMapData.GridSizeXNum = num3;
			this.CurrentMapData.GridSizeYNum = num4;
			this.CurrentMapData.fixedObstruction = new byte[num3, num4];
			this.CurrentMapData.TerrainWithTeleports = (this.CurrentMapData.fixedObstruction.Clone() as byte[,]);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					byte b = bytes[i * num + j + 9];
					if (b == 255)
					{
						this.CurrentMapData.fixedObstruction[i, j] = 1;
						this.CurrentMapData.TerrainWithTeleports[i, j] = b;
						if (DebugInfo.Instance.ShowAnQuanQu)
						{
							GameObject gameObject = Resources.Load("Prefabs/Test/TestCude") as GameObject;
							if (null != gameObject)
							{
								gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
								gameObject.transform.localPosition = new Vector3((float)i + 0.5f, 50.1f, (float)j + 0.5f);
							}
						}
					}
					else
					{
						this.CurrentMapData.fixedObstruction[i, j] = b;
						this.CurrentMapData.TerrainWithTeleports[i, j] = b;
						if (DebugInfo.Instance.ShowZhangAiWu && b == 0)
						{
							GameObject gameObject2 = Resources.Load("Prefabs/Test/TestCude") as GameObject;
							if (null != gameObject2)
							{
								gameObject2 = (SpawnManager.Instantiate(gameObject2) as GameObject);
								gameObject2.transform.localPosition = new Vector3((float)i + 0.5f, 50.1f, (float)j + 0.5f);
							}
						}
					}
				}
			}
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			if (mapSceneUIClass == SceneUIClasses.JingJiChang)
			{
				this.AddJingJiChangZuDang();
			}
			else if (mapSceneUIClass == SceneUIClasses.AngelTemple)
			{
				this.AddTiaShiShenDianZuDang0();
			}
			else if (mapSceneUIClass == SceneUIClasses.ShiLian)
			{
				this.AddZhuTiFuShiLianZuDang();
			}
			else if (mapSceneUIClass == SceneUIClasses.DaTaoSha)
			{
				this.AddDaTaoShaZuDang();
			}
		}

		private void LoadAnQuanQuXml()
		{
			string xmlName = string.Format("anquanqu.Xml", new object[0]);
			XElement gameMapXml = Global.GetGameMapXml(this.MapPicCode, xmlName);
			string xelementAttributeStr = Global.GetXElementAttributeStr(gameMapXml, "Value");
			if (xelementAttributeStr != string.Empty)
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					','
				});
				for (int i = 0; i < Enumerable.Count<string>(array); i++)
				{
					if (!(array[i].Trim() == string.Empty))
					{
						string[] array2 = array[i].Split(new char[]
						{
							'_'
						});
						int num = Convert.ToInt32(array2[0]) / 2;
						int num2 = Convert.ToInt32(array2[1]) / 2;
						if (num >= 0 && num < this.CurrentMapData.GridSizeXNum && num2 >= 0 && num2 < this.CurrentMapData.GridSizeYNum)
						{
							try
							{
								this.CurrentMapData.TerrainWithTeleports[num, num2] = byte.MaxValue;
							}
							catch (Exception ex)
							{
								MUDebug.LogException(ex);
							}
						}
					}
				}
			}
		}

		private void InitTeleports()
		{
			if ((this.MapCode == 6090 && !Global.ShowTeleport) || Global.GetMapSceneUIClass() == SceneUIClasses.LuolanFazhen)
			{
				return;
			}
			string text = StringUtil.substitute("teleports.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(this.MapCode, text);
			if (gameMapSettingsXml == null)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("加载地图传送点文件失败: {0}"), text));
				return;
			}
			List<XElement> xelementList = XmlManager.GetXElementList(gameMapSettingsXml, "Teleport");
			if (xelementList == null)
			{
				return;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement teleXml = xelementList[i];
				GTeleport teleport = Global.GetTeleport(teleXml, 0);
				this.Add(teleport);
				teleport.Start();
				if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
				{
					return;
				}
				for (int j = (int)(((double)teleport.Coordinate.X - teleport.Radius) / (double)this.CurrentMapData.GridSizeX) + 2; j <= (int)(((double)teleport.Coordinate.X + teleport.Radius) / (double)this.CurrentMapData.GridSizeX) - 2; j++)
				{
					for (int k = (int)(((double)teleport.Coordinate.Y - teleport.Radius) / (double)this.CurrentMapData.GridSizeY) + 2; k <= (int)(((double)teleport.Coordinate.Y + teleport.Radius) / (double)this.CurrentMapData.GridSizeY) - 2; k++)
					{
						if (j >= 0 && j < this.CurrentMapData.TerrainWithTeleports.GetUpperBound(0) && k >= 0 && j < this.CurrentMapData.TerrainWithTeleports.GetUpperBound(1))
						{
							this.CurrentMapData.TerrainWithTeleports[j, k] = teleport.Key;
						}
					}
				}
			}
		}

		public void InitGuangMus()
		{
			string xmlName = StringUtil.substitute("guangmu.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(this.MapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return;
			}
			List<XElement> xelementList = XmlManager.GetXElementList(gameMapSettingsXml, "Item");
			if (xelementList == null)
			{
				return;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement teleXml = xelementList[i];
				GGuangMuData guangMu = Global.GetGuangMu(teleXml);
				this.CurrentMapData.GuangMuDict.Add(guangMu.ID, guangMu);
				this.SetGuangMuState(guangMu, guangMu.Show);
			}
		}

		public void SetGuangMuStateAll(int show = -1)
		{
			foreach (KeyValuePair<int, GGuangMuData> keyValuePair in this.CurrentMapData.GuangMuDict)
			{
				GGuangMuData value = keyValuePair.Value;
				if (show >= 0)
				{
					this.SetGuangMuState(value, show);
				}
				else
				{
					this.SetGuangMuState(value, value.Show);
				}
			}
		}

		public void SetGuangMuState(int id, int show, bool force = false, int mapCode = -1)
		{
			if (mapCode >= 0 && mapCode != Global.Data.roleData.MapCode)
			{
				return;
			}
			GGuangMuData gguangMuData;
			if (this.CurrentMapData.GuangMuDict.TryGetValue(id, ref gguangMuData) && (force || show != gguangMuData.Show))
			{
				this.SetGuangMuState(gguangMuData, show);
			}
		}

		public void SetGuangMuState(GGuangMuData guangMuData, int show)
		{
			GameObject gameObject = null;
			string[] array = guangMuData.Path.Split(new char[]
			{
				'/'
			});
			for (int i = 0; i < array.Length; i++)
			{
				gameObject = U3DUtils.FindGameObjectByName(gameObject, array[i]);
				if (null == gameObject)
				{
					return;
				}
			}
			if (null != gameObject)
			{
				guangMuData.Show = show;
				if (show == 2)
				{
					if (!string.IsNullOrEmpty(guangMuData.Animation))
					{
						Animation component = gameObject.GetComponent<Animation>();
						if (null != component)
						{
							component.CrossFade(guangMuData.Animation);
						}
					}
					return;
				}
				gameObject.SetActive(show != 0);
				if (guangMuData.ZuDangs != null && guangMuData.ZuDangs.Length >= 2)
				{
					int num = (int)Global.GetTwoPointDistance(guangMuData.ZuDangs[0], guangMuData.ZuDangs[1]) + 1;
					for (float num2 = 0f; num2 <= (float)num; num2 += 0.5f)
					{
						int num3 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].X, (float)guangMuData.ZuDangs[1].X, num2 / (float)num);
						int num4 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].Y, (float)guangMuData.ZuDangs[1].Y, num2 / (float)num);
						if (show == 1)
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.SetValue(num3, num4, 0, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
						else
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.ResetValue(num3, num4, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
						num3 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].X, (float)guangMuData.ZuDangs[1].X, num2 / (float)num) + 1;
						num4 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].Y, (float)guangMuData.ZuDangs[1].Y, num2 / (float)num);
						if (show == 1)
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.SetValue(num3, num4, 0, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
						else
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.ResetValue(num3, num4, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
						num3 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].X, (float)guangMuData.ZuDangs[1].X, num2 / (float)num);
						num4 = (int)Mathf.Lerp((float)guangMuData.ZuDangs[0].Y, (float)guangMuData.ZuDangs[1].Y, num2 / (float)num) + 1;
						if (show == 1)
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.SetValue(num3, num4, 0, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
						else
						{
							this.CurrentMapData.fixedObstruction[num3, num4] = guangMuData.ResetValue(num3, num4, this.CurrentMapData.fixedObstruction[num3, num4]);
						}
					}
				}
			}
		}

		public void AddXueSeChengBaoZuDang0()
		{
			if (null != Global.BackgroundAudio43D && !Global.Data.SysSetting.CloseGameMusic)
			{
				Global.BackgroundAudio43D.PlayAudio("Audio/Map/xuesechengbao_2.mp3", true, false);
			}
		}

		public void AddYinDaoZhiYin(Vector2 start, Vector2 end)
		{
			this.RemoveYinDaoZhiYin();
			GameObject parent = new GameObject("YinDaoZhiYinRoot");
			Vector2 vector = end - start;
			Vector2 vector2 = vector.normalized * 6.5f;
			int num = 0;
			Vector2 vector3 = Vector2.zero;
			while (vector3.sqrMagnitude < vector.sqrMagnitude)
			{
				Vector2 vector4 = start + vector3;
				GDecoration decoration = Global.GetDecoration(127, GDecorationTypes.Loop, new Point((int)(vector4.x * 100f), (int)(vector4.y * 100f)), true, null, -1, -1, true, false);
				if (vector2.x > 0f)
				{
					decoration.The3DGameObject.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
				}
				else
				{
					decoration.The3DGameObject.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
				}
				U3DUtils.AddChild(parent, decoration.The3DGameObject, true);
				vector3 += vector2;
				num++;
			}
		}

		public void ReverseYinDaoZhiYin()
		{
			GameObject gameObject = GameObject.Find("YinDaoZhiYinRoot");
			if (gameObject != null)
			{
				Transform[] componentsInChildren = gameObject.transform.GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					if (transform != gameObject.transform)
					{
						transform.localEulerAngles = new Vector3(0f, -90f, 0f);
					}
				}
			}
		}

		public void RemoveYinDaoZhiYin()
		{
			GameObject gameObject = GameObject.Find("YinDaoZhiYinRoot");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}

		public void SetXueSeChengBaoTeleportActive(bool active)
		{
			Global.ShowTeleport = true;
			this.InitTeleports();
			Global.ShowTeleport = false;
		}

		public void AddTiaShiShenDianZuDang0()
		{
			for (int i = 44; i < 47; i++)
			{
				this.CurrentMapData.fixedObstruction[i, i + 7] = 0;
				this.CurrentMapData.fixedObstruction[i, i + 8] = 0;
			}
		}

		public void AddZhuTiFuShiLianZuDang()
		{
			for (int i = 44; i < 47; i++)
			{
				this.CurrentMapData.fixedObstruction[i, i + 7] = 0;
				this.CurrentMapData.fixedObstruction[i, i + 8] = 0;
			}
		}

		public void ClearZhuTiFuShiLianZuDang()
		{
			for (int i = 44; i < 47; i++)
			{
				this.CurrentMapData.fixedObstruction[i, i + 7] = 1;
				this.CurrentMapData.fixedObstruction[i, i + 8] = 1;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Sence/MapEffect2"), "zudang_tianshiqiulong_0");
			if (null != gameObject && gameObject.activeSelf)
			{
				gameObject.SetActive(false);
				Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(this.MapCode, false), true, false);
			}
		}

		public void ClearTianShiShenDianBaoZuDang0()
		{
			for (int i = 44; i < 47; i++)
			{
				this.CurrentMapData.fixedObstruction[i, i + 7] = 1;
				this.CurrentMapData.fixedObstruction[i, i + 8] = 1;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Sence/MapEffect2"), "zudang_tianshiqiulong_0");
			if (null != gameObject && gameObject.activeSelf)
			{
				gameObject.SetActive(false);
				Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(this.MapCode, false), true, false);
			}
		}

		public void AddJingJiChangZuDang()
		{
			int num = 38;
			for (int i = 32; i <= 38; i++)
			{
				this.CurrentMapData.fixedObstruction[i, num] = 0;
				this.CurrentMapData.fixedObstruction[i, num + 1] = 0;
				num--;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Scene/Non-Barrier"), "ZuDang_xuesechengbao");
			if (null != gameObject)
			{
				gameObject.SetActive(true);
			}
		}

		public void ClearJingJiChangZuDang()
		{
			int num = 38;
			for (int i = 32; i <= 38; i++)
			{
				this.CurrentMapData.fixedObstruction[i, num] = 1;
				this.CurrentMapData.fixedObstruction[i, num + 1] = 1;
				num--;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Scene/Non-Barrier"), "ZuDang_xuesechengbao");
			if (null != gameObject)
			{
				gameObject.SetActive(false);
			}
		}

		public void AddDaTaoShaZuDang()
		{
		}

		public void ClearDaTaoShaZuDang()
		{
		}

		public void AddDynamicBarrier(int index = -1)
		{
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Scene/Non-Barrier"), "ZuDang_xuesechengbao");
			if (null != gameObject)
			{
				gameObject.SetActive(true);
			}
		}

		public void ClearDynamicBarrier(int index = -1)
		{
			int num = 38;
			for (int i = 32; i <= 38; i++)
			{
				this.CurrentMapData.fixedObstruction[i, num] = 1;
				this.CurrentMapData.fixedObstruction[i, num + 1] = 1;
				num--;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(GameObject.Find("Scene/Non-Barrier"), "ZuDang_xuesechengbao");
			if (null != gameObject)
			{
				gameObject.SetActive(false);
			}
		}

		public void InitLuolanFazhenTeleports(FazhenMapProtoData fazhenTeleData)
		{
			int srcMapCode = fazhenTeleData.SrcMapCode;
			List<FazhenTelegateProtoData> listTelegate = fazhenTeleData.listTelegate;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, ConfigLuolanFazhen> luolanFazhenConfig = Global.GetLuolanFazhenConfig();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			for (int i = 0; i < listTelegate.Count; i++)
			{
				if (!dictionary.ContainsKey(listTelegate[i].gateId))
				{
					dictionary.Add(listTelegate[i].gateId, listTelegate[i].DestMapCode);
				}
				else
				{
					dictionary[listTelegate[i].gateId] = listTelegate[i].DestMapCode;
				}
			}
			string text = StringUtil.substitute("teleports.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(fazhenTeleData.SrcMapCode, text);
			if (gameMapSettingsXml == null)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("加载地图传送到文件失败：{0}"), text));
				return;
			}
			List<XElement> xelementList = XmlManager.GetXElementList(gameMapSettingsXml, "Teleport");
			if (xelementList == null)
			{
				return;
			}
			for (int j = 0; j < xelementList.Count; j++)
			{
				XElement xelement = xelementList[j];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Key");
				GTeleport gteleport = this.FindName(StringUtil.substitute("Teleport{0}", new object[]
				{
					xelementAttributeInt
				})) as GTeleport;
				if (dictionary.ContainsKey(xelementAttributeInt))
				{
					if (dictionary[xelementAttributeInt] == 0 && gteleport == null)
					{
						gteleport = Global.GetTeleport(xelement, 0);
						this.Add(gteleport);
					}
					else
					{
						if (gteleport != null)
						{
							GScene.Remove(gteleport);
							gteleport.Destroy();
						}
						if (luolanFazhenConfig != null && luolanFazhenConfig.ContainsKey(srcMapCode))
						{
							dictionary2 = luolanFazhenConfig[srcMapCode].DicMapToMen;
							if (dictionary2.Count > 1 && dictionary2.ContainsKey(dictionary[xelementAttributeInt].ToString()))
							{
								gteleport = Global.GetTeleport(xelement, ConvertExt.SafeConvertToInt32(dictionary2[dictionary[xelementAttributeInt].ToString()]));
							}
							else
							{
								gteleport = Global.GetTeleport(xelement, 0);
							}
						}
						else
						{
							gteleport = Global.GetTeleport(xelement, 0);
						}
						if (gteleport != null)
						{
							this.Add(gteleport);
						}
					}
				}
				if (gteleport != null)
				{
					gteleport.Start();
				}
				else
				{
					gteleport = Global.GetTeleport(xelement, 0);
					this.Add(gteleport);
					gteleport.Start();
				}
				if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
				{
					return;
				}
				for (int k = (int)(((double)gteleport.Coordinate.X - gteleport.Radius) / (double)this.CurrentMapData.GridSizeX) + 2; k <= (int)(((double)gteleport.Coordinate.X + gteleport.Radius) / (double)this.CurrentMapData.GridSizeX) - 2; k++)
				{
					for (int l = (int)(((double)gteleport.Coordinate.Y - gteleport.Radius) / (double)this.CurrentMapData.GridSizeY) + 2; l <= (int)(((double)gteleport.Coordinate.Y + gteleport.Radius) / (double)this.CurrentMapData.GridSizeY) - 2; l++)
					{
						if (k >= 0 && k < this.CurrentMapData.TerrainWithTeleports.GetUpperBound(0) && l >= 0 && k < this.CurrentMapData.TerrainWithTeleports.GetUpperBound(1))
						{
							this.CurrentMapData.TerrainWithTeleports[k, l] = gteleport.Key;
						}
					}
				}
			}
			this.CancelAutoFindRoad(true);
		}

		public void RemoveShengbeiDec(int enemy, int bufferID)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.Leader == null)
			{
				return;
			}
			GSprite gsprite;
			if (enemy != Global.Data.RoleID)
			{
				string name = StringUtil.substitute("Role_{0}", new object[]
				{
					enemy
				});
				gsprite = this.FindSprite(name);
			}
			else
			{
				gsprite = this.Leader;
			}
			RoleData roleData;
			if (Global.Data.OtherRoles.ContainsKey(gsprite.RoleID))
			{
				roleData = Global.Data.OtherRoles[gsprite.RoleID];
			}
			else
			{
				roleData = Global.Data.roleData;
			}
			if (roleData.BufferDataList != null)
			{
				for (int i = roleData.BufferDataList.Count - 1; i >= 0; i--)
				{
					if (roleData.BufferDataList[i].BufferID == bufferID)
					{
						roleData.BufferDataList.Remove(roleData.BufferDataList[i]);
					}
				}
			}
			int bufferDecID = Global.GetBufferDecID(bufferID);
			string name2 = string.Format("BufferDecoration_{0}", bufferDecID);
			GDecoration gdecoration = gsprite.Root.FindName(name2) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			string name3 = string.Format("DelayDecoration_{0}", 45);
			GDecoration gdecoration2 = gsprite.Root.FindName(name3) as GDecoration;
			if (gdecoration2 != null)
			{
				gdecoration2.The3DGameObject.SetActive(true);
			}
		}

		public void RemoveWangZheDec(int enemy, int bufferID)
		{
			this.RemoveShengbeiDec(enemy, bufferID);
		}

		public void RemoveEmblem(int RoleID, int EmblemCode, byte RefreshCD = 1, byte MsgComeFormJingJi = 0)
		{
			if (RoleID != Global.Data.RoleID)
			{
				string name = StringUtil.substitute("Role_{0}", new object[]
				{
					RoleID
				});
				GSprite gsprite = this.FindSprite(name);
				if (gsprite != null)
				{
					if (gsprite.SpriteType == GSpriteTypes.Other || gsprite.SpriteType == GSpriteTypes.FakeRole)
					{
						if (0.0 >= gsprite.VLife)
						{
							BufferData emblemBuffData = Global.GetEmblemBuffData(RoleID);
							if (emblemBuffData != null)
							{
								emblemBuffData.BufferSecs = 0;
							}
						}
						this.AddOrRemoveEmblemDeco(gsprite, 0);
					}
				}
			}
			else
			{
				GSprite gsprite = this.Leader;
				EmblemCoolDownItem emblemItem = Global.GetEmblemItem();
				if (emblemItem != null && 0.0 >= gsprite.VLife)
				{
					long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
					if (fangZhiJiaSuTime < emblemItem.StartTicks + emblemItem.ContinuedTicks)
					{
						if (MsgComeFormJingJi == 1)
						{
							byte b = 1;
							EmblemCoolDownItem emblemCoolDownItem = emblemItem;
							byte cdResetCount;
							emblemCoolDownItem.CdResetCount = (cdResetCount = emblemCoolDownItem.CdResetCount) + 1;
							if (b > cdResetCount)
							{
								Global.ChangeEmblemCoolDownData(fangZhiJiaSuTime, 0L);
							}
						}
						else
						{
							Global.ChangeEmblemCoolDownData(fangZhiJiaSuTime, 0L);
						}
					}
				}
				this.AddOrRemoveEmblemDeco(gsprite, 0);
			}
		}

		public void ToMapConversionByTeleportCode(int teleportCode)
		{
			GTeleport gteleport = this.FindName(StringUtil.substitute("Teleport{0}", new object[]
			{
				teleportCode
			})) as GTeleport;
			int to = gteleport.To;
			int toMapX = (int)gteleport.ToX;
			int toMapY = (int)gteleport.ToY;
			double toDirection = gteleport.ToDirection;
			if (Global.IsGoToKuaFuMap(to))
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, to, -1, -1, true, 0, teleportCode, false, false);
			}
			else if (Global.IsShiLiMap(to))
			{
				GameInstance.Game.EnterCompMap(to, 0, 0, teleportCode, 0);
			}
			else
			{
				GameInstance.Game.SpriteMapConversion(teleportCode, to, toMapX, toMapY, (int)toDirection);
			}
		}

		public void ToMapConversionByMapCode(int mapCode, int mapX, int mapY, int direction, int relife)
		{
			if (relife > 0)
			{
				Global.Data.roleData.LifeV = Global.Data.roleData.MaxLifeV;
				Global.Data.roleData.MagicV = Global.Data.roleData.MaxMagicV;
			}
			double num = (double)direction;
			if (Global.IsGoToKuaFuMap(mapCode))
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, mapCode, -1, -1, true, 0, 0, false, false);
			}
			else if (Global.IsShiLiMap(mapCode))
			{
				GameInstance.Game.EnterCompMap(mapCode, mapX, mapY, 0, 0);
			}
			else
			{
				GameInstance.Game.SpriteMapConversion(-1, mapCode, mapX, mapY, (int)num);
			}
		}

		private void MapConversionByTeleportCode(int teleportCode)
		{
			GTeleport teleport = this.FindName(StringUtil.substitute("Teleport{0}", new object[]
			{
				teleportCode
			})) as GTeleport;
			if (this.MapConversion != null)
			{
				this.MapConversion(this, new MapConversionByTeleportCodeEventArgs
				{
					Teleport = teleport
				});
			}
		}

		private bool CanMapConversionByTeleportCode(int teleportCode)
		{
			GTeleport gteleport = this.FindName(StringUtil.substitute("Teleport{0}", new object[]
			{
				teleportCode
			})) as GTeleport;
			if (this.MapConversion != null)
			{
				if (Global.IsInKuaFuHuoDongWangZhe())
				{
					return KuaFuWangZhePart.wangzheData.IsTeleportEnable(teleportCode);
				}
				if (gteleport.To == 8 && !this.Leader.IsFlying)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不佩戴翅膀，无法进入【天空之城】地图"), 0, -1, -1, 0);
					return false;
				}
				int num = 0;
				int num2 = 0;
				Global.GetMapMinLevelAndZhuanSheng(gteleport.To, out num, out num2);
				if (Global.Data.roleData.ChangeLifeCount * 400 + Global.Data.roleData.Level < num2 * 400 + num)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("等级未达到{0}转{1}级无法传送"), num2, num), 0, -1, -1, 0);
					return false;
				}
			}
			return true;
		}

		public double GetMapWidth()
		{
			return (double)this.CurrentMapData.MapWidth;
		}

		public double GetMapHeight()
		{
			return (double)this.CurrentMapData.MapHeight;
		}

		public Point GetMapPoint()
		{
			return new Point(0, 0);
		}

		public void RemoveWaitingLoadMonster(int monsterID)
		{
			for (int i = 0; i < this.MonsterItemList.Count; i++)
			{
				if (this.MonsterItemList[i].MonsterData.RoleID == monsterID)
				{
					GScene.CheckMonsterForZhaoHuanShou(this.MonsterItemList[i].MonsterData.RoleID, 0);
					this.MonsterItemList.RemoveAt(i);
					break;
				}
			}
		}

		public bool FindWaitingLoadMonster(int monsterID)
		{
			for (int i = 0; i < this.MonsterItemList.Count; i++)
			{
				if (this.MonsterItemList[i].MonsterData.RoleID == monsterID)
				{
					return true;
				}
			}
			return false;
		}

		public void ToLoadMonster(MonsterData monsterData, double x, double y, double direction, bool addToCanvas)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (monsterData.LifeV <= 0.0)
			{
				return;
			}
			BurstMonsterItem burstMonsterItem = new BurstMonsterItem
			{
				MonsterData = monsterData,
				X = x,
				Y = y,
				Direction = direction
			};
			if (addToCanvas)
			{
				this.AddListMonster(burstMonsterItem, addToCanvas);
				return;
			}
			this.MonsterItemList.Add(burstMonsterItem);
		}

		private void AddListMonster(bool addToCanvas)
		{
			if (this.MonsterItemList.Count <= 0)
			{
				return;
			}
			int num = 1;
			while (this.MonsterItemList.Count > 0 && num-- > 0)
			{
				BurstMonsterItem burstMonsterItem = this.MonsterItemList[0];
				this.MonsterItemList.RemoveAt(0);
				this.AddListMonster(burstMonsterItem, addToCanvas);
			}
		}

		private void AddListMonsterByID(int roleID, bool addToCanvas)
		{
			if (this.MonsterItemList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.MonsterItemList.Count; i++)
			{
				if (this.MonsterItemList[i].MonsterData.RoleID == roleID)
				{
					BurstMonsterItem burstMonsterItem = this.MonsterItemList[i];
					this.MonsterItemList.RemoveAt(i);
					this.AddListMonster(burstMonsterItem, addToCanvas);
					break;
				}
			}
		}

		public static void CheckMonsterForZhaoHuanShou(int ID, byte IsZhaoHuanShouLiving)
		{
			if (Global.IsOwnZhaoHuanShou(ID))
			{
				Global.Data.IsZhaoHuanShouLiving = IsZhaoHuanShouLiving;
			}
		}

		private void AddListMonster(BurstMonsterItem burstMonsterItem, bool addToCanvas)
		{
			string name = string.Format("Role_{0}", burstMonsterItem.MonsterData.RoleID);
			if (burstMonsterItem.MonsterData.LifeV == 0.0)
			{
				return;
			}
			GScene.CheckMonsterForZhaoHuanShou(burstMonsterItem.MonsterData.RoleID, 1);
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				if (gsprite.IsDeath && burstMonsterItem.MonsterData.LifeV > 0.0)
				{
					this.MonsterItemList.Add(burstMonsterItem);
					return;
				}
				return;
			}
			else
			{
				if (!addToCanvas && this.LoadingSpriteCacheDict.ContainsKey(burstMonsterItem.MonsterData.RoleID))
				{
					gsprite = this.LoadingSpriteCacheDict[burstMonsterItem.MonsterData.RoleID];
					if (gsprite.VLife != burstMonsterItem.MonsterData.LifeV)
					{
						gsprite.VLife = burstMonsterItem.MonsterData.LifeV;
					}
					return;
				}
				gsprite = this.LoadMonster(burstMonsterItem.MonsterData, burstMonsterItem.X, burstMonsterItem.Y, burstMonsterItem.Direction, addToCanvas);
				if (gsprite != null)
				{
					if (gsprite.VLife <= 0.0)
					{
						gsprite.VLife = burstMonsterItem.MonsterData.MaxLifeV;
						burstMonsterItem.MonsterData.LifeV = burstMonsterItem.MonsterData.MaxLifeV;
					}
					if (!addToCanvas && !this.LoadingSpriteCacheDict.ContainsKey(gsprite.RoleID))
					{
						this.LoadingSpriteCacheDict.Add(gsprite.RoleID, gsprite);
						GameInstance.Game.SpriteLoadAlready(gsprite.RoleID);
					}
				}
				return;
			}
		}

		public GSprite LoadMonster(MonsterData monsterData, double x, double y, double direction, bool addToCanvas)
		{
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				monsterData.RoleID
			});
			GSprite gsprite = new GSprite();
			gsprite.MonsterType = (MonsterTypes)monsterData.MonsterType;
			gsprite.SpriteType = GSpriteTypes.Monster;
			if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
			{
				gsprite.BattleWhichSide = monsterData.BattleWitchSide;
			}
			gsprite.CoordinateChanged += delegate(GSprite sender)
			{
				this.UpdateMonsterEvent(sender);
			};
			gsprite.SpriteChangeAction += delegate(object s, SpriteChangeActionEventArgs e)
			{
				this.ProcessMonsterChangeAction(s, e);
			};
			gsprite.SpriteDead += delegate(object s, EventArgs e)
			{
				GSprite gsprite2 = s as GSprite;
				if (this.SpriteDeadNotify != null)
				{
					this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
					{
						RoleID = (s as GSprite).RoleID,
						SpriteType = (s as GSprite).SpriteType,
						ShowDlg = false,
						ExtensionID = (s as GSprite).ExtensionID
					});
				}
				if (gsprite2.Name == Global.WatchSprite)
				{
					this.SetLockDeco(s as GSprite, false);
					if (this.Leader.LockObject == Global.WatchSprite)
					{
						this.Leader.LockObject = null;
					}
					Global.WatchSprite = null;
				}
				if (Global.Data.SystemMonsters.ContainsKey(gsprite2.RoleID))
				{
					MonsterData monsterData2 = Global.Data.SystemMonsters[gsprite2.RoleID];
					monsterData2.ZhongDuStart = 0L;
					monsterData2.ZhongDuSeconds = 0;
					this.ResetShenShouExists(gsprite2);
				}
				this.AddSpriteDeadDeco(gsprite2);
			};
			this.LoadSprite(gsprite, monsterData.RoleID, monsterData.RoleSex, name, string.Empty, string.Empty, monsterData.RoleName, 0, monsterData.ExtensionID, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, ColorSL.FromArgb(255, 255, 128, 0), monsterData.LifeV, 1, 0, -1.0, monsterData.EquipmentBody, -1, new Point((int)x, (int)y), (int)direction, (double)Global.Data.LifeTotalWidth, 1.0, 0, addToCanvas);
			gsprite.VLevel = monsterData.MonsterLevel;
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(gsprite.ExtensionID);
			gsprite.PlaySoundURL = monsterXmlNodeByID.PlaySound;
			gsprite.VLifeMax = monsterData.MaxLifeV;
			this.RefreshSpriteLife(gsprite);
			this.AddHunMiDec(gsprite, monsterData.FaintStart, monsterData.FaintSeconds);
			gsprite.Start();
			gsprite.SNameVisibile = Global.Data.SysSetting.ShowMonsterName;
			this.AddZhongDuStatusToMonster(gsprite, monsterData);
			if (this.Leader.RoleID == monsterData.MasterRoleID)
			{
				this.CallMonsterID = monsterData.RoleID;
			}
			return gsprite;
		}

		private void UpdateMonsterEvent(GSprite sprite)
		{
			if (!this.EnableChangMap || !sprite.Load3DResOK)
			{
				return;
			}
			sprite.Action = GActions.Walk;
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				Global.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
		}

		public void RemoveMonster(int monsterID)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.Leader == null)
			{
				return;
			}
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				monsterID
			});
			GSprite gsprite = Global.FindSprite(name);
			if (gsprite != null)
			{
				Global.RemoveObject(gsprite, true);
			}
			Global.Data.GameRadarMap.RemoveRolePoint(monsterID);
		}

		public void HideMonstersName()
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if (gsprite.SpriteType == GSpriteTypes.Monster)
					{
						gsprite.SNameVisibile = false;
					}
				}
			}
		}

		public void ShowMonstersName()
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if (gsprite.SpriteType == GSpriteTypes.Monster)
					{
						gsprite.SNameVisibile = true;
					}
				}
			}
		}

		public bool JugeShenShouExists()
		{
			if (this.CallMonsterID <= 0)
			{
				return false;
			}
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				this.CallMonsterID
			});
			GSprite gsprite = this.FindSprite(name);
			return gsprite != null && gsprite.VLife > 0.0;
		}

		public void ResetShenShouExists(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
			if (Global.Data.SystemMonsters.ContainsKey(sprite.RoleID))
			{
				MonsterData monsterData = Global.Data.SystemMonsters[sprite.RoleID];
				if (monsterData == null)
				{
					return;
				}
				if (this.Leader.RoleID == monsterData.MasterRoleID)
				{
					this.CallMonsterID = 0;
				}
			}
		}

		public void CallAutoSkill()
		{
			if (this.Leader == null)
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
				return;
			}
			if (!this.CanUseAutoFightSkillID())
			{
				return;
			}
			int autoFightSkillID = this.GetAutoFightSkillID();
			if (autoFightSkillID > 0 && autoFightSkillID != Global.Data.roleData.DefaultSkillID && Global.Data.roleData.Occupation > 0 && Global.CanMapUseMagic(this.CurrentMapData, autoFightSkillID) && !Global.SkillCoolDown(autoFightSkillID) && this.SkillNeedMagicVOk(autoFightSkillID) && Global.GetSkillDataByID(autoFightSkillID) != null)
			{
				this.DoMagicAttack(autoFightSkillID, new Point(-1, -1), null, false, true);
			}
		}

		protected void ProcessMonsterChangeAction(object s, SpriteChangeActionEventArgs e)
		{
			(s as GSprite).Action = GActions.Stand;
		}

		public void PlayBossAnimation(int monsterID, int toMapCode, int posX, int posY, int effectX, int effectY, long monsterTime, int checkSum)
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
			if (monsterXmlNodeByID != null && monsterXmlNodeByID.ComeAnimation > 0)
			{
				GDecoration decoration = Global.GetDecoration(monsterXmlNodeByID.ComeAnimation, GDecorationTypes.AutoRemove, new Point(effectX, effectY), false, null, -1, -1, true, false);
				if (decoration != null)
				{
					decoration.DecorationDestroyNotify = delegate(object s, EventArgs e)
					{
						PlayZone.GlobalPlayZone.ClosSkipWindow();
						GameInstance.Game.SendEndBossAnimation(monsterID, toMapCode, posX, posY, effectX, effectY, monsterTime, checkSum);
					};
					PlayZone.GlobalPlayZone.ShowSkipWindow(decoration);
				}
			}
		}

		private bool OnObstruction(GSprite sprite)
		{
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point point = new Point(sprite.Coordinate.X / this.CurrentMapData.GridSizeX, sprite.Coordinate.Y / this.CurrentMapData.GridSizeY);
			return this.CurrentMapData.fixedObstruction[point.X, point.Y] == 0;
		}

		private bool OnObstruction(Point p)
		{
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point point = new Point(p.X / this.CurrentMapData.GridSizeX, p.Y / this.CurrentMapData.GridSizeY);
			return this.CurrentMapData.fixedObstruction[point.X, point.Y] == 0;
		}

		private bool OnObstruction2(Point gridPoint)
		{
			return this.CurrentMapData.fixedObstruction[gridPoint.X, gridPoint.Y] == 0;
		}

		private bool LinearMoveByRunTo(Point p, int offset, int extAction, bool noMouseHint = false, int action = 2)
		{
			int myTimer = Global.GetMyTimer();
			this.HideMouseLeftButtonUpEffect();
			MUDebug.LogTime("LinearMoveByRunTo 1", ref myTimer, 3);
			p = this.GetAOffsetGridPoint(p, this.Leader.Coordinate, (offset > 0) ? Math.Max(offset / 50, 1) : 0);
			MUDebug.LogTime("LinearMoveByRunTo 2", ref myTimer, 3);
			Point point;
			if (Global.OnObstruction(p, this.CurrentMapData) && this.FindLinearNoObsMaxPoint(this.Leader, p, out point))
			{
				p = point;
			}
			MUDebug.LogTime("LinearMoveByRunTo 3", ref myTimer, 3);
			bool result = this.LinearMove(this.Leader, p, action, extAction, noMouseHint);
			MUDebug.LogTime("LinearMoveByRunTo 4", ref myTimer, 3);
			return result;
		}

		private double GetLinearMoveSpeed(GSprite sprite)
		{
			return 1.0;
		}

		private bool LinearMove(GSprite sprite, Point to, int action, int extAction, bool noMouseHint = false)
		{
			if (sprite.VLife <= 0.0)
			{
				return false;
			}
			if (this._LinearMove(sprite, to, action, extAction, this.GetLinearMoveSpeed(sprite), 0.0, false, string.Empty, noMouseHint, 0))
			{
				GameInstance.Game.SpriteMoveTo(sprite.Coordinate, to, action, extAction);
				return true;
			}
			return false;
		}

		private bool _LinearMove(GSprite sprite, Point p, int action, int extAction, double moveCost, double startMoveTicks, bool recalcPos, string pathString = "", bool noMouseHint = false, int currentPathIndex = 0)
		{
			int myTimer = Global.GetMyTimer();
			return this.AStarMove(sprite, p, action, extAction, moveCost, startMoveTicks, recalcPos, pathString, noMouseHint, currentPathIndex);
		}

		private double GetUnitCost(GSprite sprite, int action, double moveCost)
		{
			double num = 0.0;
			if (sprite.SpriteType != GSpriteTypes.Monster)
			{
				if (action == 1)
				{
					num = (double)Global.Data.WalkUnitCost;
				}
				else if (action == 2)
				{
					num = (double)Global.Data.RunUnitCost;
				}
				if (sprite.SpriteMoveSpeed == 0.0)
				{
					return 1.0;
				}
				num = (double)((int)(num / sprite.SpriteMoveSpeed));
				if (moveCost >= 2.0)
				{
					moveCost = 2.0;
				}
				if (moveCost <= 0.4)
				{
					moveCost = 0.4;
				}
			}
			else
			{
				num = (double)Global.Data.RunUnitCost;
			}
			return num / moveCost;
		}

		private bool FindAPointIn4Direction(GSprite sprite, Point p, double offset, out List<ANode> path)
		{
			path = null;
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point start = new Point
			{
				X = sprite.Coordinate.X / this.CurrentMapData.GridSizeX,
				Y = sprite.Coordinate.Y / this.CurrentMapData.GridSizeY
			};
			Point point = new Point
			{
				X = p.X / this.CurrentMapData.GridSizeX,
				Y = p.Y / this.CurrentMapData.GridSizeY
			};
			point = Global.GetAGridPointIn4Direction(point, this.CurrentMapData.fixedObstruction, this.CurrentMapData);
			path = this.FindPath(sprite, start, point);
			return path != null && path.Count > 0;
		}

		private bool FindLinearNoObsMaxPoint(GSprite sprite, Point p, out Point maxPoint)
		{
			maxPoint = new Point(0, 0);
			List<ANode> list = new List<ANode>();
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Global.Bresenham(list, sprite.Coordinate.X / this.CurrentMapData.GridSizeX, sprite.Coordinate.Y / this.CurrentMapData.GridSizeY, p.X, p.Y, this.CurrentMapData.fixedObstruction);
			if (list.Count > 1)
			{
				maxPoint = new Point(list[list.Count - 1].x, list[list.Count - 1].y);
				list.Clear();
				return true;
			}
			return false;
		}

		private int GetTimeEslapedGridNum(long startMoveTicks, int unitCost)
		{
			long num = Global.GetCorrectLocalTime() - startMoveTicks;
			if (num <= 0L)
			{
				return 0;
			}
			if (unitCost == 0)
			{
				return 0;
			}
			return (int)(num / (long)unitCost);
		}

		private bool JugeCanMove(int objType, int x, int y)
		{
			return this.CurrentMapData._MapGrid.CanMove(objType, x, y, 0, 0);
		}

		private bool CanWalk(GSpriteTypes objType, ANode node)
		{
			return this.CurrentMapData.fixedObstruction[node.x, node.y] != 0;
		}

		private bool CanWalk(GSpriteTypes objType, Point node)
		{
			return node.X < this.CurrentMapData.fixedObstruction.GetUpperBound(0) && node.Y < this.CurrentMapData.fixedObstruction.GetUpperBound(1) && this.CurrentMapData.fixedObstruction[node.X, node.Y] != 0;
		}

		private List<ANode> FindPath(GSprite sprite, Point start, Point end)
		{
			if (!this.CanWalk(sprite.SpriteType, start))
			{
				start = Global.GetAGridPointIn4Direction(start, this.CurrentMapData.fixedObstruction, this.CurrentMapData);
				if (!this.CanWalk(sprite.SpriteType, start))
				{
					return null;
				}
			}
			Point point;
			if (!this.CanWalk(sprite.SpriteType, end) && this.FindLinearNoObsMaxPoint(sprite, end, out point))
			{
				end = point;
			}
			if (this.pathFinderFast == null)
			{
				this.pathFinderFast = new PathFinderFast(this.CurrentMapData.fixedObstruction)
				{
					Formula = HeuristicFormula.Manhattan,
					Diagonals = true,
					HeuristicEstimate = 2,
					ReopenCloseNodes = true,
					SearchLimit = int.MaxValue,
					Punish = null,
					MaxNum = Global.GMax(this.CurrentMapData.GridSizeXNum, this.CurrentMapData.GridSizeYNum)
				};
			}
			this.pathFinderFast.EnablePunish = false;
			List<PathFinderNode> list = this.pathFinderFast.FindPath(start, end);
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			List<ANode> list2 = new List<ANode>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(new ANode(list[i].X, list[i].Y));
			}
			return list2;
		}

		private bool AStarMove(GSprite sprite, Point p, int action, int extAction, double moveCost, double startMoveTicks, bool recalcPos, string pathString, bool noMouseHint, int currentPathIndex)
		{
			List<ANode> list = null;
			MUDebug.LogStart();
			if (pathString != null && pathString.Length > 0)
			{
				list = Global.TransStringToPathArr(pathString);
				if (list.Count <= 0)
				{
					list = null;
				}
				if (list != null && list.Count > 0)
				{
					if (currentPathIndex > 0 && currentPathIndex < list.Count)
					{
						list.RemoveRange(0, currentPathIndex);
					}
					if (list.Count > 0)
					{
						if (sprite.SpriteType == GSpriteTypes.Monster)
						{
							sprite.Coordinate = new Point(list[0].x * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, list[0].y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
						}
						p = new Point(list[list.Count - 1].x * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, list[list.Count - 1].y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
					}
					else
					{
						list = null;
					}
				}
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point point = new Point
			{
				X = sprite.Coordinate.X / this.CurrentMapData.GridSizeX,
				Y = sprite.Coordinate.Y / this.CurrentMapData.GridSizeY
			};
			Point point2 = new Point
			{
				X = p.X / this.CurrentMapData.GridSizeX,
				Y = p.Y / this.CurrentMapData.GridSizeY
			};
			if (!(point != point2))
			{
				return false;
			}
			Point destination = p;
			if (list == null)
			{
				list = this.FindPath(sprite, point, point2);
			}
			if (!this.EnableChangMap)
			{
				return false;
			}
			if (list == null || list.Count <= 0)
			{
				if (this.IsLeaderAutoFindRoad(sprite))
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("无法自动寻路"), 0, -1, -1, 0);
					this.CancelAutoFindRoad(true);
				}
				else if (!this.FindAPointIn4Direction(sprite, p, (double)((int)((double)this.CurrentMapData.GridSizeX * 1.5)), out list))
				{
					Point point3;
					if (this.FindLinearNoObsMaxPoint(sprite, p, out point3))
					{
						list = null;
						point2 = new Point
						{
							X = point3.X / this.CurrentMapData.GridSizeX,
							Y = point3.Y / this.CurrentMapData.GridSizeY
						};
						destination = point3;
						list = this.FindPath(sprite, point, point2);
					}
				}
			}
			if (list == null || list.Count <= 0)
			{
				sprite.Action = GActions.Stand;
				Global.RemoveStoryboard(sprite.Name);
				if (sprite.RoleID == this.Leader.RoleID)
				{
					this.HideMouseLeftButtonUpEffect();
				}
				return false;
			}
			if (sprite.RoleID == this.Leader.RoleID && this.Leader.LockObject == null && !noMouseHint)
			{
				this.ShowMouseLeftButtonUpEffect(p);
			}
			if (pathString == null || pathString.Length <= 0)
			{
				ANode anode = new ANode(point.X, point.Y);
				list.Insert(0, anode);
			}
			if (sprite.RoleID == this.Leader.RoleID)
			{
				GameInstance.Game.CurrentSession.RolePathString = Global.TransPathToString(list);
			}
			sprite.Destination = destination;
			StoryBoard.RemoveStoryBoard(sprite.Name);
			StoryBoard storyBoard = new StoryBoard(sprite.Name);
			storyBoard.Completed += this.Move_Completed;
			sprite.ExtAction = (ExtActionTypes)extAction;
			Point point4 = new Point(list[0].x * this.CurrentMapData.GridSizeX + this.CurrentMapData.GridSizeX / 2, list[0].y * this.CurrentMapData.GridSizeY + this.CurrentMapData.GridSizeY / 2);
			if (sprite.SpriteType == GSpriteTypes.Monster && sprite.MonsterType == MonsterTypes.JUSTMOVE)
			{
				if (sprite.Coordinate != point4)
				{
					sprite.Direction = this.CalcDirection(sprite.Coordinate, point4);
				}
			}
			else
			{
				sprite.Direction = this.CalcDirection(sprite.Coordinate, point4);
			}
			sprite.Action = (GActions)action;
			storyBoard.Binding();
			int num = 0;
			if (sprite.RoleID != this.Leader.RoleID)
			{
			}
			list.RemoveAt(0);
			storyBoard.Start(sprite, list, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY, p.X, p.Y, (long)num, false);
			return true;
		}

		public void StartStoryboard(GSprite sprite, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, int direction, int action, int extAction, Point toPos)
		{
			GameInstance.Game.CurrentSession.RolePathString = pathString;
			List<ANode> list = Global.FindSubPathFromExistPath(pathString, nSrcGridX, nSrcGridY, nDestGridX, nDestGridY);
			GMapData currentMapData = this.CurrentMapData;
			Point destination = toPos;
			sprite.Destination = destination;
			StoryBoard.RemoveStoryBoard(sprite.Name);
			StoryBoard storyBoard = new StoryBoard(sprite.Name);
			storyBoard.Completed += this.Move_Completed;
			sprite.ExtAction = (ExtActionTypes)extAction;
			sprite.Direction = direction;
			sprite.Action = (GActions)action;
			storyBoard.Binding();
			list.RemoveAt(0);
			storyBoard.Start(sprite, list, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY, toPos.X, toPos.Y, 0L, false);
		}

		public void Move_Completed(object sender, EventArgs args)
		{
			try
			{
				StoryBoard storyBoard = sender as StoryBoard;
				bool flag = false;
				GSprite gsprite = this.FindSprite(storyBoard.Name);
				if (gsprite != null)
				{
					if (gsprite.RoleID == this.Leader.RoleID)
					{
						this.HideMouseLeftButtonUpEffect();
					}
					if (gsprite.RoleID != this.Leader.RoleID)
					{
						if (gsprite.SpriteType != GSpriteTypes.Monster)
						{
							gsprite.Action = GActions.Stand;
							Global.RemoveStoryboard(gsprite.Name);
						}
					}
					else
					{
						int count = storyBoard.Path.Count;
						if (count > 2)
						{
							GameInstance.Game.SpriteMoveEnd(gsprite.Coordinate, gsprite.Direction);
						}
						Point mapCursorPoint = this.GetMapCursorPoint(this.CurDestinationPos);
						if (!Global.CompareTowPointByGridXY(gsprite.Coordinate, mapCursorPoint))
						{
						}
						if ((this.MuchClick() || this.LeftButtonDown) && this.CurDestinationPos.X >= 0 && this.CurDestinationPos.Y >= 0)
						{
						}
						if (gsprite.Action != GActions.Magic)
						{
							gsprite.Action = GActions.Stand;
						}
						if (gsprite.ExtAction != ExtActionTypes.EXTACTION_NONE && Global.Data.AutoRoadExtActionType == ExtActionTypes.EXTACTION_NONE)
						{
							ExtActionTypes extAction = gsprite.ExtAction;
							gsprite.ExtAction = ExtActionTypes.EXTACTION_NONE;
							this.ProcessExtAction(extAction);
						}
					}
					storyBoard.Clear();
					if (gsprite.SpriteType == GSpriteTypes.Leader)
					{
						if (this.ShiftAttackWaitingPos.X != -1 && this.ShiftAttackWaitingPos.Y != -1)
						{
							if (this.Leader.CanAttack() && this.Leader.Action != GActions.Attack && this.Leader.Action != GActions.Magic && this.Leader.Action != GActions.Bow && !this.Leader.IsMagicMove)
							{
								double num = this.CalcDirection(this.Leader, this.ShiftAttackWaitingPos);
								this.Leader.EnemyTarget = this.ShiftAttackWaitingPos;
								this.Leader.Rotation = this.CalcRotation(this.Leader, this.ShiftAttackWaitingPos);
							}
							this.ShiftAttackWaitingPos = new Point(-1, -1);
						}
						else if (this.SkillAttackWaitingID != 0)
						{
							if (this.SkillAttackWaitingID < 0)
							{
								this.DoAttack(true);
							}
							else
							{
								MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(this.SkillAttackWaitingID);
								if (skillXmlNode != null)
								{
									int skillType = skillXmlNode.SkillType;
									if (skillType == 1)
									{
										this.ExternalSkillAttack(this.SkillAttackWaitingID, true);
									}
									else
									{
										long skillCoolDownTicks = Global.GetSkillCoolDownTicks(this.SkillAttackWaitingID);
										if (skillCoolDownTicks <= 0L)
										{
											this.DoMagicAttack(this.SkillAttackWaitingID, new Point(-1, -1), null, false, true);
										}
									}
								}
							}
							this.SkillAttackWaitingID = 0;
						}
						else if (flag)
						{
							Point mapCursorPoint2 = this.GetMapCursorPoint(this.CurDestinationPos);
							this.MoveToPoint(mapCursorPoint2, false, 0);
							this.LeftButtonDownMoveCount++;
						}
						else if (!this.UpdateLeaderDirection(true))
						{
							if (!this.ShiftIsPressed)
							{
								if ((double)Global.GetCorrectLocalTime() - this.LeftButtonClickTicks <= 100.0)
								{
									this.LeftButtonDown = false;
								}
							}
							else
							{
								this.DoShiftAttack(true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void StartSimpleStoryboard(GSprite sprite, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, Point toPos, int elapsedTicks, double simpleMoveSpeedRate)
		{
			GameInstance.Game.CurrentSession.RolePathString = pathString;
			List<ANode> list = Global.FindSubPathFromExistPath(pathString, nSrcGridX, nSrcGridY, nDestGridX, nDestGridY);
			GMapData currentMapData = this.CurrentMapData;
			Point destination = toPos;
			sprite.Destination = destination;
			StoryBoard.RemoveStoryBoard(sprite.Name);
			StoryBoard storyBoard = new StoryBoard(sprite.Name);
			storyBoard.Binding();
			list.RemoveAt(0);
			storyBoard.SimpleMoveSpeedRate = simpleMoveSpeedRate;
			storyBoard.Start(sprite, list, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY, toPos.X, toPos.Y, (long)elapsedTicks, true);
		}

		private double CalcDirection(GSprite sprite, Point p)
		{
			return Global.GetDirectionByTan((double)p.X, (double)p.Y, (double)sprite.Coordinate.X, (double)sprite.Coordinate.Y);
		}

		private Quaternion CalcRotation(GSprite sprite, Point p)
		{
			return Quaternion.LookRotation(new Vector3((float)(p.X - sprite.cx) / 100f, 0f, (float)(p.Y - sprite.cy) / 100f), Vector3.up);
		}

		private int CalcDirection(Point op, Point ep)
		{
			return (int)Global.GetDirectionByTan((double)ep.X, (double)ep.Y, (double)op.X, (double)op.Y);
		}

		private void ChangeDirection(GSprite sprite, double direction, int yAngle = -1)
		{
			if ((double)sprite.Direction != direction)
			{
				sprite.Direction = (int)direction;
				if (yAngle >= 0)
				{
					sprite.YAngle = yAngle;
				}
			}
		}

		private double ChangeDirection(GSprite sprite, Point p, int yAngle = -1)
		{
			double directionByTan = Global.GetDirectionByTan((double)p.X, (double)p.Y, (double)sprite.Coordinate.X, (double)sprite.Coordinate.Y);
			this.ChangeDirection(sprite, (double)((int)directionByTan), -1);
			return directionByTan;
		}

		private void AnimationMoveSprite(GSprite sprite, Point p, bool byAnimation, int ticks = 30)
		{
			if (Global.OnObstruction(p, this.CurrentMapData))
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = p.X / this.CurrentMapData.GridSizeX;
			int num2 = p.Y / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				this.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
			sprite.Coordinate = p;
		}

		private void AnimationMoveSprite2(GSprite sprite, Point p, bool byAnimation, int ticks = 30)
		{
			if (Global.OnObstruction(p, this.CurrentMapData))
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = p.X / this.CurrentMapData.GridSizeX;
			int num2 = p.Y / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				this.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
			sprite.Coordinate = p;
		}

		private void AnimationMoveSprite3(GSprite sprite, Point p, int ticks = 30)
		{
			if (Global.OnObstruction(p, this.CurrentMapData))
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = p.X / this.CurrentMapData.GridSizeX;
			int num2 = p.Y / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				this.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
			int direction = sprite.Direction;
			int code = 240 + direction;
			GDecoration decoration = Global.GetDecoration(code, GDecorationTypes.Loop, new Point(sprite.CenterX - sprite.ShadowOffset.X, sprite.CenterY - sprite.ShadowOffset.Y), false, null, -1, -1, true, false);
			decoration.Name = "YeManChongFeng";
			sprite.Root.Children.Add(decoration);
		}

		public void MoveToPoint(Point p, bool ignoreObs = false, int externalAction = -1)
		{
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			int num = this.CalcDirection(this.Leader.Coordinate, p);
			Quaternion rotation = this.CalcRotation(this.Leader, p);
			int num2 = Global.GetMovingAction(this.Leader.Coordinate, p);
			num2 = Global.GMax(externalAction, num2);
			if (num2 == -1)
			{
				this.Leader.Direction = num;
				this.Leader.Action = GActions.Stand;
				this.Leader.Rotation = rotation;
				GameInstance.Game.SpriteAction((double)num, 0, this.Leader.Coordinate, p, this.Leader.YAngle, new Point(0, 0));
				return;
			}
			bool flag;
			if (num2 == 1)
			{
				flag = ChuanQiUtils.WalkTo(this, this.Leader, num, 0, new Point(-1, -1));
			}
			else
			{
				flag = ChuanQiUtils.RunTo(this, this.Leader, num, 0, new Point(-1, -1), ignoreObs);
				if (!flag)
				{
					flag = ChuanQiUtils.WalkTo(this, this.Leader, num, 0, new Point(-1, -1));
				}
			}
			if (!flag)
			{
				this.TryToRun(num, false, false);
			}
		}

		public void MoveByDirection(int dir, bool ignoreObs = false, int externalAction = -1)
		{
			if (this.Leader == null || null == this.Leader.The3DGameObject)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Attack || this.Leader.Action == GActions.Magic)
			{
				return;
			}
			bool flag = false;
			if (externalAction == 1)
			{
				flag = ChuanQiUtils.WalkTo(this, this.Leader, dir, 0, new Point(-1, -1));
			}
			else if (externalAction == 2)
			{
				flag = ChuanQiUtils.RunTo(this, this.Leader, dir, 0, new Point(-1, -1), ignoreObs);
			}
			if (!flag)
			{
				if (externalAction == 1)
				{
					this.TryToWalk(dir, false, false);
				}
				else if (externalAction == 2)
				{
					this.TryToRun(dir, false, false);
				}
			}
		}

		public void TryToRun(int direction = -1, bool directTry7Dir = false, bool ignoreObs = false)
		{
			int num = (direction >= 0) ? direction : this.Leader.Direction;
			if (!directTry7Dir)
			{
				for (int i = 0; i < 3; i++)
				{
					int nDir = (num + i) % 8;
					if (Global.Data.MoveMode != 0 && ChuanQiUtils.RunTo(this, this.Leader, nDir, 0, new Point(-1, -1), ignoreObs))
					{
						return;
					}
					if (ChuanQiUtils.RunTo(this, this.Leader, nDir, 0, new Point(-1, -1), false))
					{
						return;
					}
					int num2 = num - i;
					if (num2 < 0)
					{
						num2 = 8 + num2;
					}
					if (Global.Data.MoveMode != 0 && ChuanQiUtils.RunTo(this, this.Leader, num2, 0, new Point(-1, -1), ignoreObs))
					{
						return;
					}
					if (ChuanQiUtils.RunTo(this, this.Leader, num2, 0, new Point(-1, -1), false))
					{
						return;
					}
				}
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num3 = this.Leader.cx / this.CurrentMapData.GridSizeX;
			int num4 = this.Leader.cy / this.CurrentMapData.GridSizeY;
			int num5 = num3;
			int num6 = num4;
			Random random = new Random();
			num = this.Leader.Direction;
			bool flag = false;
			for (int j = 0; j < 8; j++)
			{
				if (num3 != num5 || num4 != num6)
				{
					flag = true;
					break;
				}
				if (random.Next(0, 3) > 0)
				{
					num++;
				}
				else if (num > 0)
				{
					num--;
				}
				else
				{
					num = 7;
				}
				if (num > 7)
				{
					num = 0;
				}
				if (Global.Data.MoveMode != 0)
				{
					if (!ChuanQiUtils.RunTo(this, this.Leader, num, 0, new Point(-1, -1), ignoreObs))
					{
						ChuanQiUtils.WalkTo(this, this.Leader, num, 0, new Point(-1, -1));
					}
				}
				else
				{
					ChuanQiUtils.WalkTo(this, this.Leader, num, 0, new Point(-1, -1));
				}
				num5 = this.Leader.cx / this.CurrentMapData.GridSizeX;
				num6 = this.Leader.cy / this.CurrentMapData.GridSizeY;
			}
			if (!flag && this.Leader.Action != GActions.Stand)
			{
				this.Leader.Action = GActions.Stand;
				GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
			}
		}

		public void TryToWalk(int direction = -1, bool directTry7Dir = false, bool ignoreObs = false)
		{
			int num = (direction >= 0) ? direction : this.Leader.Direction;
			if (!directTry7Dir)
			{
				for (int i = 0; i < 3; i++)
				{
					int nDir = (num + i) % 8;
					if (Global.Data.MoveMode != 0 && ChuanQiUtils.WalkTo(this, this.Leader, nDir, 0, new Point(-1, -1)))
					{
						return;
					}
					if (ChuanQiUtils.WalkTo(this, this.Leader, nDir, 0, new Point(-1, -1)))
					{
						return;
					}
					int num2 = num - i;
					if (num2 < 0)
					{
						num2 = 8 + num2;
					}
					if (Global.Data.MoveMode != 0 && ChuanQiUtils.WalkTo(this, this.Leader, num2, 0, new Point(-1, -1)))
					{
						return;
					}
					if (ChuanQiUtils.WalkTo(this, this.Leader, num2, 0, new Point(-1, -1)))
					{
						return;
					}
				}
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num3 = this.Leader.cx / this.CurrentMapData.GridSizeX;
			int num4 = this.Leader.cy / this.CurrentMapData.GridSizeY;
			int num5 = num3;
			int num6 = num4;
			Random random = new Random();
			num = this.Leader.Direction;
			bool flag = false;
			for (int j = 0; j < 8; j++)
			{
				if (num3 != num5 || num4 != num6)
				{
					flag = true;
					break;
				}
				if (random.Next(0, 3) > 0)
				{
					num++;
				}
				else if (num > 0)
				{
					num--;
				}
				else
				{
					num = 7;
				}
				if (num > 7)
				{
					num = 0;
				}
				ChuanQiUtils.WalkTo(this, this.Leader, num, 0, new Point(-1, -1));
				num5 = this.Leader.cx / this.CurrentMapData.GridSizeX;
				num6 = this.Leader.cy / this.CurrentMapData.GridSizeY;
			}
			if (!flag && this.Leader.Action != GActions.Stand)
			{
				this.Leader.Action = GActions.Stand;
				GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
			}
		}

		public void MoveByDirectionBy36Dir(int dir36, int dir, bool ignoreObs = false, int externalAction = -1)
		{
			if (this.Leader == null || null == this.Leader.The3DGameObject)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Attack)
			{
				return;
			}
			if (this.Leader.Action == GActions.Magic && this.Leader.SkillMoveType == GSkillMoveType.None)
			{
				return;
			}
			this.CancelAutoFight(0, true);
			Transform transform = this.Leader.The3DGameObject.transform;
			Vector3 position = transform.position;
			double angle = (double)dir36 * 10.0;
			Point extensionPoint = Global.GetExtensionPoint(new Point((int)(position.x * 100f), (int)(position.z * 100f)), angle, 100.0);
			bool flag = false;
			if (this.Leader.SkillMoveType == GSkillMoveType.Rotate)
			{
				if (Time.frameCount % 2 == 0)
				{
					this.Leader.Direction = dir;
					this.Leader.Rotation = Global.GetRotationByTwoPoint(this.Leader.Coordinate, extensionPoint);
				}
				if (Time.frameCount % 5 == 0)
				{
					GameInstance.Game.SpriteChangeAngleCmd(this.Leader.Direction, this.Leader.YAngle);
				}
				return;
			}
			if (externalAction == 1)
			{
				flag = ChuanQiUtils.WalkTo(this, this.Leader, dir, 0, extensionPoint);
			}
			else if (externalAction == 2)
			{
				flag = ChuanQiUtils.RunTo(this, this.Leader, dir, 0, extensionPoint, ignoreObs);
			}
			if (!flag)
			{
				if (externalAction == 1)
				{
					this.TryToWalk(dir, false, false);
				}
				else if (externalAction == 2)
				{
					this.TryToRun(dir, false, false);
				}
			}
		}

		private List<ANode> GetHitFlyPath(Point start, Point end)
		{
			List<ANode> list = new List<ANode>();
			list.Add(new ANode(start.X, start.Y));
			list.Add(new ANode(end.X, end.Y));
			return list;
		}

		private bool HitFlySprite(GSprite sprite, Point grid)
		{
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			Point point = new Point
			{
				X = sprite.Coordinate.X / this.CurrentMapData.GridSizeX,
				Y = sprite.Coordinate.Y / this.CurrentMapData.GridSizeY
			};
			if (point == grid)
			{
				return false;
			}
			List<ANode> hitFlyPath = this.GetHitFlyPath(point, grid);
			if (hitFlyPath == null || hitFlyPath.Count <= 0)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < hitFlyPath.Count; i++)
			{
				stringBuilder.AppendFormat("{0}_{1}", hitFlyPath[i].x, hitFlyPath[i].y);
				if (i < hitFlyPath.Count - 1)
				{
					stringBuilder.Append("|");
				}
			}
			Point toPos = new Point(this.CurrentMapData.GridSizeX * hitFlyPath[hitFlyPath.Count - 1].x + this.CurrentMapData.GridSizeX / 2, this.CurrentMapData.GridSizeY * hitFlyPath[hitFlyPath.Count - 1].y + this.CurrentMapData.GridSizeY / 2);
			this.StartSimpleStoryboard(sprite, stringBuilder.ToString(), hitFlyPath[0].x, hitFlyPath[0].y, hitFlyPath[hitFlyPath.Count - 1].x, hitFlyPath[hitFlyPath.Count - 1].y, toPos, 0, (double)(hitFlyPath.Count * 2));
			return true;
		}

		private bool AddNewNPCEx(XElement npcXmlNode, double posX, double posY, int dir)
		{
			int num = 2130706432 + Global.GetXElementAttributeInt(npcXmlNode, "ID");
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				num
			});
			if (this.FindSprite(name) != null)
			{
				return true;
			}
			GSprite gsprite = new GSprite();
			gsprite.SpriteType = GSpriteTypes.NPC;
			string sname = Global.GetXElementAttributeStr(npcXmlNode, "SName");
			if (!string.IsNullOrEmpty(Global.GetXElementAttributeStr(npcXmlNode, "Function")))
			{
				sname = StringUtil.substitute("{0}•{1}", new object[]
				{
					Global.GetXElementAttributeStr(npcXmlNode, "Function"),
					Global.GetXElementAttributeStr(npcXmlNode, "SName")
				});
			}
			string text = ConfigNPCs.GetNPCSoundByID(Global.GetXElementAttributeInt(npcXmlNode, "ID"));
			if (text.Length > 0)
			{
				text = StringUtil.substitute("Audio/Npc/{0}", new object[]
				{
					text
				});
			}
			string npctalkSoundByID = ConfigNPCs.GetNPCTalkSoundByID(Global.GetXElementAttributeInt(npcXmlNode, "ID"));
			if (npctalkSoundByID.Length > 0)
			{
				string[] array = npctalkSoundByID.Split(new char[]
				{
					'|'
				});
				gsprite.PlayNpcTalkSoundURLs = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					gsprite.PlayNpcTalkSoundURLs[i] = StringUtil.substitute("Audio/Npc/{0}", new object[]
					{
						array[i]
					});
				}
			}
			gsprite.PlaySoundURL = text;
			this.LoadSprite(gsprite, num, Global.GetXElementAttributeInt(npcXmlNode, "Sex"), name, string.Empty, string.Empty, sname, 0, Global.GetXElementAttributeInt(npcXmlNode, "ID"), 65280U, 65280U, 65280U, 0.0, 0, 0, -1.0, Global.GetXElementAttributeInt(npcXmlNode, "Code"), -1, new Point((int)posX, (int)posY), dir, 0.0, 0.0, 0, true);
			gsprite.Start();
			gsprite.IsOpposition = false;
			NPCTaskState npctaskState = Global.GetNPCTaskState(Global.GetXElementAttributeInt(npcXmlNode, "ID"));
			if (npctaskState != null)
			{
				this.UpdateNPCTaskState(gsprite, (NPCTaskStates)npctaskState.TaskState);
			}
			this.UpdateNaviDeco(gsprite);
			this.HandleSheLiZhiYuanNPCDeco(gsprite);
			Global.CurrentMapData._MapGrid.MoveObject(-1, -1, gsprite.cx, gsprite.cy, gsprite);
			return true;
		}

		public void UpdateNPCTaskState(GSprite sprite, NPCTaskStates state)
		{
			if (sprite.ExtensionID == 60900)
			{
				return;
			}
			GDecoration gdecoration = sprite.Root.FindName("TaskDeco") as GDecoration;
			if (gdecoration != null)
			{
				sprite.Remove(gdecoration);
				gdecoration = null;
			}
			if (state <= NPCTaskStates.NONE)
			{
				gdecoration = null;
				sprite.NPCTaskState = NPCTaskStates.NONE;
			}
			else if (state == NPCTaskStates.NEWTASK)
			{
				gdecoration = Global.GetDecoration(68, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
				sprite.NPCTaskState = NPCTaskStates.NEWTASK;
			}
			else if (state == NPCTaskStates.DOINGTASK)
			{
				gdecoration = Global.GetDecoration(70, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
				sprite.NPCTaskState = NPCTaskStates.DOINGTASK;
			}
			else if (state >= NPCTaskStates.OKTASK)
			{
				gdecoration = Global.GetDecoration(69, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
				sprite.NPCTaskState = NPCTaskStates.OKTASK;
			}
			if (gdecoration != null)
			{
				gdecoration.Name = "TaskDeco";
				sprite.Root.Children.Add(gdecoration);
			}
		}

		private bool AddNewNPC(XElement npcXmlNode, int posX, int posY, int mapCode = -1, int dir = 0)
		{
			if (Global.Data.roleData.MapCode != mapCode)
			{
				return false;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			int x = posX / this.CurrentMapData.GridSizeX;
			int y = posY / this.CurrentMapData.GridSizeY;
			int xelementAttributeInt = Global.GetXElementAttributeInt(npcXmlNode, "ID");
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
			if (npcvobyID.IsSafe > 0)
			{
				Global.SetPartialSafeRegion(new Point(x, y), 2);
			}
			Global.SetPartialCollideRegion(new Point(x, y), npcXmlNode);
			return this.AddNewNPCEx(npcXmlNode, (double)posX, (double)posY, dir);
		}

		private bool DelNpc(int npcID, int mapCode)
		{
			if (Global.Data.roleData.IsFlashPlayer >= 1)
			{
				return false;
			}
			if (mapCode != Global.Data.roleData.MapCode)
			{
				return false;
			}
			int num = 2130706432 + npcID;
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				num
			});
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				Global.RemoveObject(gsprite, true);
			}
			return true;
		}

		private void CheckNPCPosition(GSprite sprite)
		{
			if (sprite.SpriteType != GSpriteTypes.NPC)
			{
				return;
			}
			if (sprite.Coordinate == sprite.OrigCoordinate)
			{
				return;
			}
			sprite.Action = GActions.Stand;
			sprite.Direction = 0;
			sprite.Coordinate = sprite.OrigCoordinate;
		}

		public void ServerPlayGame(int roleID)
		{
			GameInstance.Game.CurrentSession.PlayGame = true;
		}

		public static void ServerStopGame()
		{
			if (GameInstance.Game != null && GameInstance.Game.CurrentSession != null)
			{
				GameInstance.Game.CurrentSession.PlayGame = false;
			}
		}

		public void ServerLinearMove(int roleID, int mapCode, int action, int toX, int toY, double moveCost, int extAction, int fromX, int fromY, double startMoveTicks, string pathString, int currentPathIndex = 0)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.roleData.MapCode == mapCode)
					{
						if (Global.Data.RoleID == roleID)
						{
							TCPPing.RecordRecCmd(107);
							if (this.Leader != null)
							{
								this.Leader.MoveSpeed = moveCost;
							}
						}
						else
						{
							string name = StringUtil.substitute("Role_{0}", new object[]
							{
								roleID
							});
							GSprite gsprite = this.FindSprite(name);
							if (gsprite != null)
							{
								if (gsprite.VLife <= 0.0)
								{
									this.DealSpecialMonsterDead(gsprite);
								}
								else
								{
									gsprite.MoveSpeed = moveCost;
									string pathString2 = DataHelper.UnZipStringToBase64(pathString);
									this._LinearMove(gsprite, new Point(toX, toY), action, extAction, moveCost, startMoveTicks, true, pathString2, false, currentPathIndex);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerMoveEnd(int roleID, int mapCode, int action, int toX, int toY, int direction, int tryRun)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.roleData.MapCode == mapCode)
					{
						string name = "Leader";
						if (Global.Data.RoleID != roleID)
						{
							name = StringUtil.substitute("Role_{0}", new object[]
							{
								roleID
							});
						}
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && gsprite.VLife > 0.0)
						{
							Global.RemoveStoryboard(gsprite.Name);
							if (toX > 0 && toY > 0 && (toX != gsprite.Coordinate.X || toY != gsprite.Coordinate.Y))
							{
								this.AnimationMoveSprite(gsprite, new Point(toX, toY), false, 30);
							}
							this.ChangeDirection(gsprite, (double)direction, -1);
							gsprite.Action = (GActions)action;
							if (Global.Data.RoleID == roleID && tryRun == 1)
							{
								this.TryToRun(-1, true, false);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerStopMove(int roleID, int stopIndex)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string key = "Leader";
					if (Global.Data.RoleID != roleID)
					{
						key = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
					}
					Global.StopStoryboard(key, stopIndex);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private void DealSpecialMonsterDead(GSprite sprite)
		{
			if (sprite == null || sprite.VLife > 0.0)
			{
				return;
			}
			if (this.KillSpriteNotify != null)
			{
				this.KillSpriteNotify(this, new SpriteNotifyEventArgs
				{
					RoleID = sprite.RoleID,
					SpriteType = sprite.SpriteType,
					ShowDlg = false,
					ExtensionID = sprite.ExtensionID
				});
			}
			Global.RemoveObject(sprite, true);
		}

		public void ServerRunAction(int roleID, int mapCode, double direction, GActions action, int x, int y, int targetX, int targetY, int yAngle, int moveToX, int moveToY)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.roleData.MapCode == mapCode)
					{
						if (Global.Data.RoleID == roleID)
						{
							TCPPing.RecordRecCmd(114);
						}
						else
						{
							string text = string.Empty;
							text = StringUtil.substitute("Role_{0}", new object[]
							{
								roleID
							});
							Global.RemoveStoryboard(text);
							GSprite gsprite = this.FindSprite(text);
							if (gsprite == null)
							{
								MUDebug.Log<string>(new string[]
								{
									"<color=yellow> RoleRunAction  GSprite没有找到    " + text + "</color>"
								});
							}
							else if (gsprite.VLife > 0.0)
							{
								if (x > 0 && y > 0 && x != gsprite.Coordinate.X && y != gsprite.Coordinate.Y)
								{
									this.AnimationMoveSprite(gsprite, new Point(x, y), false, 30);
								}
								gsprite.EnemyTarget = new Point(targetX, targetY);
								this.ChangeDirection(gsprite, direction, yAngle);
								if (gsprite.Action == action)
								{
									gsprite.Action = GActions.Stand;
								}
								MUDebug.Log<string>(new string[]
								{
									"<color=yellow> RoleRunAction  Action  =     " + action + "</color>"
								});
								gsprite.Action = action;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeAngle(int roleID, double direction, int yAngle)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.RoleID != roleID)
					{
						string name = string.Empty;
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null)
						{
							if (gsprite.VLife > 0.0)
							{
								this.ChangeDirection(gsprite, direction, yAngle);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerAttack(int roleID, int burst, int injure, double enemyLife, long newExperience, long experience, long level, SpriteAttackResultData attackResultData = null)
		{
			try
			{
				if (this.EnableChangMap)
				{
					GSprite gsprite = ObjectsManager.FindSprite(roleID);
					if (gsprite != null)
					{
						long experience2 = Global.Data.roleData.Experience;
						Global.Data.roleData.Experience = experience;
						bool levelUp = (long)Global.Data.roleData.Level != level;
						Global.Data.roleData.Level = (int)level;
						newExperience = Math.Abs(experience - experience2);
						Global.BattleHandleEx(this.FindSprite("Leader"), gsprite, burst, (double)injure, enemyLife, (double)newExperience, levelUp, attackResultData);
						if (burst > 0)
						{
							this.StartMapShaking();
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerInjured(int attackerRoleID, int injuredRoleID, int burst, int injure, double injuredRoleLife, int attackerLevel, int toHitGridX, int toHitGridY, int subArmor, SpriteInjuredData injuredData = null)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (0 < attackerRoleID && injuredRoleID == this.Leader.RoleID)
					{
						Global.Data.nAttackRoleID = attackerRoleID;
						GSprite gsprite = Global.FindSprite(StringUtil.substitute("Role_{0}", new object[]
						{
							Global.Data.nAttackRoleID
						}));
						if (gsprite != null)
						{
							Global.Data.strAttackName = gsprite.VSName;
						}
					}
					GSprite gsprite2;
					if (injuredRoleID == this.Leader.RoleID)
					{
						gsprite2 = this.FindSprite("Leader");
					}
					else
					{
						gsprite2 = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
						{
							injuredRoleID
						}));
					}
					if (gsprite2 != null && 0 <= subArmor - 1)
					{
						gsprite2.VArmor = (long)subArmor;
					}
					if (injuredRoleID == this.Leader.RoleID)
					{
						Global.Data.roleData.LifeV = (int)injuredRoleLife;
						GSprite gsprite3 = gsprite2;
						if (gsprite3 != null)
						{
							int direction = (gsprite2 == null) ? 2 : gsprite2.Direction;
							Global.BattleHandleEx(gsprite3, burst, (double)injure, injuredRoleLife, direction);
							gsprite3.VLife = (double)((int)injuredRoleLife);
							this.RefreshSpriteLife(gsprite3);
							this.RefreshRoleArmor(gsprite3);
							if (gsprite3.Action == GActions.Stand && injure > 0)
							{
								gsprite3.Action = GActions.Injured;
							}
							if (toHitGridX > 0 && toHitGridY > 0)
							{
								this.HitFlySprite(gsprite3, new Point(toHitGridX, toHitGridY));
							}
							if (gsprite3.VLife <= 0.0 && this.KillSpriteNotify != null)
							{
								this.KillSpriteNotify(this, new SpriteNotifyEventArgs
								{
									RoleID = gsprite3.RoleID,
									SpriteType = gsprite3.SpriteType,
									ShowDlg = false,
									ExtensionID = gsprite3.ExtensionID
								});
							}
						}
						GSprite gsprite4;
						if (attackerRoleID != this.Leader.RoleID)
						{
							gsprite4 = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
							{
								attackerRoleID
							}));
							if (gsprite4 == null)
							{
								if (Global.Data.OtherRoles.ContainsKey(attackerRoleID))
								{
									if (!this.FindWaitingLoadOtherRole(attackerRoleID) && !this.LoadingSpriteCacheDict.ContainsKey(attackerRoleID))
									{
										RoleData value = Global.Data.OtherRoles.GetValue(attackerRoleID);
										if (value != null)
										{
											this.ToLoadOtherRole(value, (double)value.PosX, (double)value.PosY, (double)value.RoleDirection, false);
										}
									}
								}
								else if (Global.Data.SystemMonsters.ContainsKey(attackerRoleID))
								{
									if (!this.FindWaitingLoadMonster(attackerRoleID) && !this.LoadingSpriteCacheDict.ContainsKey(attackerRoleID))
									{
										MonsterData value2 = Global.Data.SystemMonsters.GetValue(attackerRoleID);
										if (value2 != null)
										{
											this.ToLoadMonster(value2, (double)value2.PosX, (double)value2.PosY, (double)value2.RoleDirection, false);
										}
									}
								}
							}
						}
						else
						{
							gsprite4 = this.Leader;
						}
						if (gsprite4 != null)
						{
							gsprite4.CheckAttackAction();
							this.AttackingMeMonsterID = gsprite4.RoleID;
							if ((gsprite4.SpriteType == GSpriteTypes.Other || gsprite4.SpriteType == GSpriteTypes.Leader || gsprite4.SpriteType == GSpriteTypes.FakeRole) && (gsprite4.LastAction == GActions.Attack || gsprite4.Action == GActions.Attack))
							{
								string playingMusicFile = StringUtil.substitute("Audio/Injured/injured.mp3", new object[0]);
								gsprite3.PlaySpriteSound(playingMusicFile, false);
							}
						}
						this.Leader.LastInjuredTicks = Global.GetCorrectLocalTime();
					}
					else
					{
						if (Global.Data.OtherRoles.ContainsKey(injuredRoleID))
						{
							Global.Data.OtherRoles[injuredRoleID].LifeV = (int)injuredRoleLife;
							Global.Data.OtherRoles[injuredRoleID].CurrentArmorV = subArmor;
						}
						else if (Global.Data.SystemMonsters.ContainsKey(injuredRoleID))
						{
							Global.Data.SystemMonsters[injuredRoleID].LifeV = injuredRoleLife;
							if (Global.IsOwnZhaoHuanShou(attackerRoleID))
							{
								GSprite gsprite5 = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
								{
									attackerRoleID
								}));
								if (gsprite5 != null)
								{
									if (gsprite2 != null)
									{
										Global.BattleHandleEx_(gsprite5, gsprite2, burst, (double)injure, injuredRoleLife, 0.0, false, attackerRoleID, injuredRoleID, null);
									}
								}
							}
							else if (attackerRoleID == 0)
							{
							}
						}
						else if (Global.Data.BiaoChes.ContainsKey(injuredRoleID))
						{
							Global.Data.BiaoChes[injuredRoleID].CurrentLifeV = (int)injuredRoleLife;
						}
						else if (Global.Data.JunQis.ContainsKey(injuredRoleID))
						{
							Global.Data.JunQis[injuredRoleID].CurrentLifeV = (int)injuredRoleLife;
						}
						else if (Global.Data.FakeRoles.ContainsKey(injuredRoleID))
						{
							Global.Data.FakeRoles[injuredRoleID].MyRoleDataMini.LifeV = (int)injuredRoleLife;
						}
						GSprite gsprite6 = gsprite2;
						if (gsprite6 != null)
						{
							GSprite gsprite7;
							if (attackerRoleID != this.Leader.RoleID)
							{
								gsprite7 = this.FindSprite(StringUtil.substitute("Role_{0}", new object[]
								{
									attackerRoleID
								}));
							}
							else
							{
								gsprite7 = this.Leader;
							}
							if (attackerRoleID != this.Leader.RoleID)
							{
								Global.BattleHandleEx(gsprite7, gsprite2, burst, (double)injure, injuredRoleLife, 0.0, false, null);
							}
							if (gsprite7 != null)
							{
								gsprite7.CheckAttackAction();
								if ((gsprite7.SpriteType == GSpriteTypes.Other || gsprite7.SpriteType == GSpriteTypes.Leader || gsprite7.SpriteType == GSpriteTypes.FakeRole) && (gsprite7.LastAction == GActions.Attack || gsprite7.Action == GActions.Attack))
								{
									string playingMusicFile2 = StringUtil.substitute("Audio/Injured/injured.mp3", new object[0]);
									gsprite6.PlaySpriteSound(playingMusicFile2, false);
								}
							}
							gsprite6.VLife = (double)((long)injuredRoleLife);
							this.RefreshSpriteLife(gsprite6);
							this.RefreshRoleArmor(gsprite6);
							if (injure > 0 && gsprite6.SpriteType == GSpriteTypes.Monster && gsprite6.Action != GActions.Attack)
							{
								gsprite6.Action = GActions.Injured;
							}
							if (toHitGridX > 0 && toHitGridY > 0)
							{
								this.HitFlySprite(gsprite6, new Point(toHitGridX, toHitGridY));
							}
							if (gsprite6.VLife <= 0.0)
							{
								this.SetLockTargetDeco(gsprite6, false);
								if (this.KillSpriteNotify != null)
								{
									this.KillSpriteNotify(this, new SpriteNotifyEventArgs
									{
										RoleID = gsprite6.RoleID,
										SpriteType = gsprite6.SpriteType,
										ShowDlg = false,
										ExtensionID = gsprite6.ExtensionID
									});
								}
							}
						}
					}
					RoleData roleData = null;
					if (Global.Data.OtherRoles.TryGetValue(attackerRoleID, ref roleData))
					{
						roleData.Level = attackerLevel;
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerMagicCode(int roleID, int mapCode, int magicCode)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.roleData.MapCode == mapCode)
					{
						if (roleID == Global.Data.RoleID)
						{
							Global.AddSkillCoolDown(magicCode, true, -1L);
							if (this.Leader.VLife <= 0.0)
							{
							}
						}
						else
						{
							string name = StringUtil.substitute("Role_{0}", new object[]
							{
								roleID
							});
							GSprite gsprite = this.FindSprite(name);
							if (gsprite != null)
							{
								if (gsprite.VLife > 0.0)
								{
									gsprite.CurrentMagic = magicCode;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerRealive(int roleID, int x, int y, int direction)
		{
			try
			{
				if (!this.EnableChangMap)
				{
					return;
				}
				string text = "Leader";
				if (Global.Data.RoleID != roleID)
				{
					text = StringUtil.substitute("Role_{0}", new object[]
					{
						roleID
					});
					Global.RemoveStoryboard(text);
				}
				GSprite gsprite = this.FindSprite(text);
				if (gsprite != null)
				{
					Global.RemoveObject(gsprite, true);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<string>(new string[]
				{
					"ServerRealive  RemoveOldObject"
				});
				MUDebug.LogException(ex);
			}
			try
			{
				if (roleID == Global.Data.RoleID)
				{
					this.LeaderCenterPoint = new Point(-1, -1);
					Global.Data.roleData.LifeV = Global.Data.roleData.MaxLifeV;
					Global.Data.roleData.MagicV = Global.Data.roleData.MaxMagicV;
					this.LoadLeader(x, y, direction, false);
				}
				else if (Global.Data.OtherRoles.ContainsKey(roleID))
				{
					RoleData roleData = Global.Data.OtherRoles[roleID];
					roleData.LifeV = roleData.MaxLifeV;
					roleData.MagicV = roleData.MaxMagicV;
					this.ToLoadOtherRole(roleData, (double)x, (double)y, (double)direction, false);
				}
				else if (Global.Data.SystemMonsters.ContainsKey(roleID))
				{
					MonsterData monsterData = Global.Data.SystemMonsters[roleID];
					monsterData.LifeV = monsterData.MaxLifeV;
					monsterData.MagicV = monsterData.MaxMagicV;
					this.ToLoadMonster(monsterData, (double)x, (double)y, (double)direction, false);
				}
			}
			catch (Exception ex2)
			{
				MUDebug.LogError<string>(new string[]
				{
					"ServerRealive  Other"
				});
				MUDebug.LogException(ex2);
			}
		}

		public void ServerRealife(int roleID, int x, int y, int direction, double lifeV, double magicV, int force)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.LifeV = (int)lifeV;
						Global.Data.roleData.MagicV = (int)magicV;
						if (this.Leader != null)
						{
							if (this.Leader.VLife > 0.0 || force > 0)
							{
								this.Leader.VLife = lifeV;
								this.RefreshSpriteLife(this.Leader);
								this.RefreshRoleArmor(this.Leader);
							}
						}
					}
					else
					{
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							RoleData roleData = Global.Data.OtherRoles[roleID];
							roleData.LifeV = (int)lifeV;
							roleData.MagicV = (int)magicV;
						}
						else if (Global.Data.SystemMonsters.ContainsKey(roleID))
						{
							MonsterData monsterData = Global.Data.SystemMonsters[roleID];
							monsterData.LifeV = lifeV;
							monsterData.MagicV = magicV;
						}
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null)
						{
							if (gsprite.VLife > 0.0 || force > 0)
							{
								if (gsprite.VLife != lifeV)
								{
									gsprite.VLife = lifeV;
									this.RefreshSpriteLife(gsprite);
									this.RefreshRoleArmor(gsprite);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerRoleLeave(int roleID, int spriteType)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						roleID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (this.SpriteDeadNotify != null)
						{
							this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
							{
								RoleID = gsprite.RoleID,
								SpriteType = gsprite.SpriteType,
								ShowDlg = false,
								ExtensionID = gsprite.ExtensionID
							});
						}
						if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu && gsprite.SpriteType == GSpriteTypes.Monster)
						{
							PlayZone.GlobalPlayZone.GameTaskBoxMini.SetMoYuNumber(-1);
						}
						if (gsprite.Name == Global.WatchSprite)
						{
							this.SetLockDeco(gsprite, false);
							if (this.Leader.LockObject == Global.WatchSprite)
							{
								this.Leader.LockObject = null;
							}
						}
						if (Global.IsInKuaFuTeamCompete())
						{
							Transform transform = gsprite.The3DGameObject.transform;
							if (transform != null)
							{
								int childCount = transform.childCount;
								if (childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
								{
									for (int i = 0; i < childCount; i++)
									{
										transform.GetChild(i).gameObject.SetActive(false);
									}
									SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
									if (component)
									{
										component.enabled = false;
									}
									BoxCollider component2 = transform.GetComponent<BoxCollider>();
									if (component2)
									{
										component2.enabled = false;
									}
								}
							}
						}
						Global.Data.GameRadarMap.RemoveRolePoint(roleID);
						this.AddSpriteDeadDeco(gsprite);
						Global.RemoveObject(gsprite, true);
						GScene.CheckMonsterForZhaoHuanShou(gsprite.RoleID, 0);
						Global.RemoveRoleNameColor(gsprite.RoleID);
					}
					else if (!this.RemoveCachingSprite(roleID) && !this.RemoveWaitingLoadOtherRole(roleID))
					{
						this.RemoveWaitingLoadMonster(roleID);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeRebornEquipShow(int roleId, int showEquip)
		{
			if (roleId == Global.Data.roleData.RoleID)
			{
				return;
			}
			RoleData roleData = null;
			if (!Global.Data.OtherRoles.TryGetValue(roleId, ref roleData))
			{
				return;
			}
			roleData.RebornShowEquip = showEquip;
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				roleId
			});
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				if (gsprite.VLife <= 0.0)
				{
					return;
				}
				this.ChangeBodyCode(gsprite);
			}
		}

		public void ServerChangeRebornModelShow(int roleId, int showModel)
		{
			if (roleId == Global.Data.roleData.RoleID)
			{
				return;
			}
			RoleData roleData = null;
			if (!Global.Data.OtherRoles.TryGetValue(roleId, ref roleData))
			{
				return;
			}
			roleData.RebornShowModel = showModel;
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				roleId
			});
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				if (gsprite.VLife <= 0.0)
				{
					return;
				}
				this.ChangeBodyCode(gsprite);
			}
		}

		public void ServerChangeCode(ChangeEquipData changeEquipData)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (changeEquipData == null)
					{
						if (this.Leader != null)
						{
							if (this.Leader.VLife > 0.0)
							{
								this.ChangeBodyCode(this.Leader);
							}
						}
					}
					else if (changeEquipData.RoleID != Global.Data.roleData.RoleID)
					{
						if (Global.Data.OtherRoles.ContainsKey(changeEquipData.RoleID))
						{
							GoodsData goodsData = null;
							RoleData roleData = Global.Data.OtherRoles[changeEquipData.RoleID];
							if (changeEquipData.EquipGoodsData != null)
							{
								if (Global.GetCategoriyByGoodsID(changeEquipData.EquipGoodsData.GoodsID) == 340)
								{
									goodsData = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(roleData.RoleID, changeEquipData.EquipGoodsData.Id, 1);
									if (goodsData == null)
									{
										if (roleData.MountEquipList == null)
										{
											roleData.MountEquipList = new List<GoodsData>();
										}
										roleData.MountEquipList.Add(changeEquipData.EquipGoodsData);
									}
									else
									{
										int num = roleData.MountEquipList.IndexOf(goodsData);
										roleData.MountEquipList[num] = changeEquipData.EquipGoodsData;
									}
								}
								else if (Global.IsRebornEquip(Global.GetCategoriyByGoodsID(changeEquipData.EquipGoodsData.GoodsID)))
								{
									goodsData = Global.GetGoodsDataByDbID(changeEquipData.EquipGoodsData.Id, roleData.RebornGoodsDataList);
									if (goodsData == null)
									{
										if (roleData.RebornGoodsDataList == null)
										{
											roleData.RebornGoodsDataList = new List<GoodsData>();
										}
										roleData.RebornGoodsDataList.Add(changeEquipData.EquipGoodsData);
									}
									else
									{
										int num2 = roleData.RebornGoodsDataList.IndexOf(goodsData);
										roleData.RebornGoodsDataList[num2] = changeEquipData.EquipGoodsData;
									}
								}
								else
								{
									goodsData = Global.GetGoodsDataByDbID(changeEquipData.EquipGoodsData.Id, roleData.GoodsDataList);
									if (goodsData == null)
									{
										if (roleData.GoodsDataList == null)
										{
											roleData.GoodsDataList = new List<GoodsData>();
										}
										roleData.GoodsDataList.Add(changeEquipData.EquipGoodsData);
									}
									else
									{
										int num3 = roleData.GoodsDataList.IndexOf(goodsData);
										roleData.GoodsDataList[num3] = changeEquipData.EquipGoodsData;
									}
								}
								goodsData = changeEquipData.EquipGoodsData;
							}
							if (changeEquipData.UsingWinData != null)
							{
								roleData.MyWingData = changeEquipData.UsingWinData;
							}
							string name = StringUtil.substitute("Role_{0}", new object[]
							{
								changeEquipData.RoleID
							});
							GSprite gsprite = this.FindSprite(name);
							if (gsprite != null)
							{
								if (gsprite.VLife > 0.0)
								{
									if (goodsData != null && !string.IsNullOrEmpty(Global.GetGoods3DResNameByID(goodsData.GoodsID, gsprite.Occupation)))
									{
										this.ChangeBodyCode(gsprite);
									}
									else if (changeEquipData.UsingWinData != null)
									{
										this.ChangeBodyCode(gsprite);
									}
									else if (goodsData != null && (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 38 || Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 37))
									{
										this.ChangeBodyCode(gsprite);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ReloadSprite(int roleId)
		{
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				roleId
			});
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null && gsprite.VLife > 0.0)
			{
				this.ChangeBodyCode(gsprite);
			}
		}

		public void ServerCompeteTask(int roleID, int npcID, int taskID, int cmdState)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						if (cmdState >= 0)
						{
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerExpChange(int roleID, long experience, int level, long newExperience)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						bool flag = Global.Data.roleData.Level != level;
						if (flag)
						{
							this.StartMapShaking();
						}
						Global.Data.roleData.Experience = experience;
						Global.Data.roleData.Level = level;
						if (newExperience != 0L)
						{
							Global.ShowTextForExpOrGold(ColorSL.ParseArgb(64000U), Global.GetLang("经验") + " +" + newExperience, 1000f, 0f, 40f, 280f, -150f);
						}
						Global.EarnLevel(this.Leader, flag);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerRebornExpchange(int roleID, long experience, int level, long newExperience, int reborncount)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						bool flag = Global.Data.roleData.RebornLevel != level;
						if (flag)
						{
							this.StartMapShaking();
						}
						Global.Data.roleData.RebornCount = reborncount;
						Global.Data.roleData.RebornExperience = experience;
						Global.Data.roleData.RebornLevel = level;
						if (newExperience != 0L)
						{
							Global.ShowTextForExpOrGold(ColorSL.ParseArgb(64000U), Global.GetLang("重生经验") + " +" + newExperience, 1000f, 0f, 40f, 280f, -150f);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewGoodsPack(int owerRoleID, string owerRoleName, int autoID, int goodsPackID, int toX, int toY, int goodsID, int goodsNum, long productTicks, long teamID, string teamRoleIDs, int lucky, int excellenceInfo, int appendPropLev, int forge_Level)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						if (productTicks <= 0L || Global.GetCorrectLocalTime() - productTicks < (long)(Global.Data.GoodsDestroytimeTick * 1000))
						{
							string name = StringUtil.substitute("GoodsPack{0}", new object[]
							{
								autoID
							});
							if (!(this.FindName(name) is GGoodsPack))
							{
								GGoodsPack goodsPack = Global.GetGoodsPack(autoID, goodsPackID, owerRoleID, owerRoleName, new Point(toX, toY), goodsID, goodsNum, productTicks, teamID, teamRoleIDs);
								goodsPack.Lucky = lucky;
								goodsPack.ExcellenceInfo = excellenceInfo;
								goodsPack.AppendPropLev = appendPropLev;
								goodsPack.ForgeLevel = forge_Level;
								goodsPack.EfficaciousSection = new int[4];
								goodsPack.EfficaciousSection[0] = 0;
								goodsPack.EfficaciousSection[1] = 20;
								goodsPack.EfficaciousSection[2] = 0;
								goodsPack.EfficaciousSection[3] = 20;
								this.Add(goodsPack);
								if (Global.Data.SysSetting.ShowGoodsPackName)
								{
									goodsPack.TextVisible = true;
								}
								this.CurrentMapData._MapGrid.MoveObject(-1, -1, goodsPack.cx, goodsPack.cy, goodsPack);
								Global.SetOpacity(goodsPack, this.CurrentMapData);
								goodsPack.Start();
								this.AddDecoForGoodsPack(goodsPack, autoID, owerRoleID, new Point(toX, toY), lucky, excellenceInfo, appendPropLev, forge_Level);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerDelGoodsPack(int autoID, int toRoleID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						string name = StringUtil.substitute("GoodsPack{0}", new object[]
						{
							autoID
						});
						GGoodsPack ggoodsPack = this.FindName(name) as GGoodsPack;
						if (ggoodsPack != null)
						{
							this.RemoveDecoForGoodsPack(autoID);
							if (toRoleID != this.Leader.RoleID)
							{
								Global.RemoveObject(ggoodsPack, true);
							}
							else
							{
								ggoodsPack.PickUpGoodsPackByAnimation(this.Leader);
							}
						}
						this.SelectedGoodsPack = null;
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerPickUpGoodsPack(int autoID, int toRoleID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						string name = StringUtil.substitute("GoodsPack{0}", new object[]
						{
							autoID
						});
						GGoodsPack ggoodsPack = this.FindName(name) as GGoodsPack;
						if (ggoodsPack != null)
						{
							ggoodsPack.PickUpGoodsPackByAnimation(this.Leader);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewNPC(NPCRole npc)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						string roleString = npc.RoleString;
						XElement xelement = XElement.Parse(roleString);
						if (xelement != null)
						{
							this.AddNewNPC(xelement, npc.PosX, npc.PosY, npc.MapCode, npc.Dir);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerDelNPC(int npcID, int mapCode)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						this.DelNpc(npcID, mapCode);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerUpdateNPCTaskState(int npcID, int state)
		{
			try
			{
				if (this.EnableChangMap)
				{
					NPCTaskState npctaskState = Global.GetNPCTaskState(npcID - 2130706432);
					if (npctaskState != null)
					{
						npctaskState.TaskState = state;
					}
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						npcID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (npctaskState != null)
						{
							this.UpdateNPCTaskState(gsprite, (NPCTaskStates)npctaskState.TaskState);
						}
						this.UpdateNaviDeco(gsprite);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerHited(int attacker, int enemy, int enemyX, int enemyY, int magicCode)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Quaternion rotation = Quaternion.identity;
					string name = (attacker != Global.Data.roleData.RoleID) ? StringUtil.substitute("Role_{0}", new object[]
					{
						attacker
					}) : "Leader";
					GSprite gsprite = this.FindSprite(name);
					GSprite gsprite2 = null;
					if (gsprite != null)
					{
						Point coordinate = gsprite.Coordinate;
						Point coordinate2 = new Point(enemyX, enemyY);
						if (enemy != -1)
						{
							string name2 = (enemy != Global.Data.roleData.RoleID) ? StringUtil.substitute("Role_{0}", new object[]
							{
								enemy
							}) : "Leader";
							gsprite2 = this.FindSprite(name2);
							if (gsprite2 != null)
							{
								coordinate2 = gsprite2.Coordinate;
							}
						}
						rotation = Global.GetRotationByTwoPoint(coordinate, coordinate2);
					}
					if (gsprite == null || gsprite2 == null || gsprite.SpriteType == GSpriteTypes.Leader || gsprite2.SpriteType != GSpriteTypes.Monster || magicCode == 11001)
					{
						if (magicCode < 0)
						{
							this.AddHitDecoration(enemy, magicCode, rotation, gsprite);
						}
						else
						{
							MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
							if (maigcInfoVOByCode != null)
							{
								int magicType = maigcInfoVOByCode.MagicType;
								if (magicType == 1)
								{
									if (enemy != -1)
									{
										this.AddHitDecoration(enemy, magicCode, rotation, gsprite);
									}
									else
									{
										this.AddHitDecoration(enemyX, enemyY, magicCode);
									}
								}
								else if (magicType == 2)
								{
									if (enemy != -1)
									{
										this.AddHitDecoration(enemy, magicCode, rotation, gsprite);
									}
									else
									{
										this.AddHitDecoration(enemyX, enemyY, magicCode);
									}
								}
								else if (magicType == 3)
								{
									if (this.Leader != null)
									{
										this.AddMagic(enemy, magicCode);
									}
									else
									{
										this.AddMagic(Global.Data.RoleID, magicCode);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangePos(int roleID, int x, int y, int direction, int animation)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						if (this.Leader != null)
						{
							int myTimer = Global.GetMyTimer();
							if (Global.FindMoveStroyboard(this.Leader.Name))
							{
								this.HideMouseLeftButtonUpEffect();
								Global.RemoveStoryboard(this.Leader.Name);
								this.Leader.Action = GActions.Stand;
							}
							this.LeaderCenterPoint = new Point(-1, -1);
							this.Leader.Direction = direction;
							Point to = new Point(x, y);
							if (animation == 0)
							{
								this.AnimationMoveSprite(this.Leader, new Point(x, y), true, 0);
								GameInstance.Game.SpritePosition(to, Global.GetCorrectLocalTime());
							}
							else if (animation == 1)
							{
								this.AnimationMoveSprite2(this.Leader, new Point(x, y), true, 1);
								GameInstance.Game.SpritePosition(to, Global.GetCorrectLocalTime());
							}
							else if (animation == 2)
							{
								this.LinearMoveByRunTo(new Point(x, y), 0, 0, true, 2);
							}
							else if (animation == 3)
							{
								this.AnimationMoveSprite3(this.Leader, new Point(x, y), 450);
								GameInstance.Game.SpritePosition(to, Global.GetCorrectLocalTime());
							}
							if (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan)
							{
								MUDebug.LogError<string>(new string[]
								{
									string.Concat(new object[]
									{
										"大逃杀重新锁定位置 x  ",
										x,
										"  y  ",
										y,
										"  direction  ",
										direction,
										"  roleID  ",
										roleID
									})
								});
							}
						}
						this.JugeSafeRegion();
					}
					else
					{
						string text = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(text);
						if (gsprite != null)
						{
							Global.RemoveStoryboard(gsprite.Name);
							gsprite.Action = GActions.Stand;
							gsprite.Direction = direction;
							if (animation == 0)
							{
								this.AnimationMoveSprite(gsprite, new Point(x, y), true, 0);
							}
							else if (animation == 1)
							{
								this.AnimationMoveSprite2(gsprite, new Point(x, y), true, 1);
							}
							else if (animation != 2)
							{
								if (animation == 3)
								{
									this.AnimationMoveSprite3(gsprite, new Point(x, y), 450);
								}
							}
						}
						else if (null != GameObject.Find(text))
						{
							MUDebug.Log<string>(new string[]
							{
								"<color=red>角色尝试移动 在系统中没有找到对象  但是在场景中找到了对象 对象的名字为： " + gsprite + "</color>"
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerUpdateRoleData(int roleID, int maxLifeV, int maxMagicV, int currentLifeV, int currentMagicV, long maxArmorV, long currentArmorV)
		{
			try
			{
				if (this.EnableChangMap)
				{
					GSprite gsprite;
					if (roleID == Global.Data.RoleID)
					{
						if (0 < maxMagicV)
						{
							Global.Data.roleData.MagicV = currentMagicV;
							Global.Data.roleData.MaxMagicV = maxMagicV;
						}
						if (0 < maxLifeV)
						{
							Global.Data.roleData.LifeV = currentLifeV;
							Global.Data.roleData.MaxLifeV = maxLifeV;
						}
						if (0L < maxArmorV)
						{
							Global.Data.roleData.CurrentArmorV = (int)currentArmorV;
							Global.Data.roleData.MaxArmorV = (int)maxArmorV;
						}
						if (this.Leader != null)
						{
						}
						gsprite = this.Leader;
					}
					else
					{
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							if (0 < maxMagicV)
							{
								Global.Data.OtherRoles[roleID].MaxLifeV = maxLifeV;
								Global.Data.OtherRoles[roleID].LifeV = currentLifeV;
							}
							if (0 < maxMagicV)
							{
								Global.Data.OtherRoles[roleID].MaxMagicV = maxMagicV;
								Global.Data.OtherRoles[roleID].MagicV = currentMagicV;
							}
							if (0L < maxArmorV)
							{
								Global.Data.OtherRoles[roleID].CurrentArmorV = (int)currentArmorV;
								Global.Data.OtherRoles[roleID].MaxArmorV = (int)maxArmorV;
							}
						}
						else if (Global.Data.FakeRoles.ContainsKey(roleID) && 0L < maxArmorV)
						{
							Global.Data.FakeRoles[roleID].MyRoleDataMini.CurrentArmorV = (int)currentArmorV;
							Global.Data.FakeRoles[roleID].MyRoleDataMini.MaxArmorV = (int)maxArmorV;
						}
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						gsprite = this.FindSprite(name);
					}
					if (gsprite != null)
					{
						if (0 < maxLifeV)
						{
							gsprite.VLife = (double)currentLifeV;
							gsprite.VLifeMax = (double)maxLifeV;
							this.RefreshSpriteLife(gsprite);
						}
						if (0L < maxArmorV)
						{
							gsprite.VArmor = currentArmorV;
							gsprite.VArmorMax = maxArmorV;
						}
						this.RefreshRoleArmor(gsprite);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangePKMode(int roleID, int pkMode)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.PKMode = pkMode;
						if (this.Leader != null)
						{
							this.Leader.PKMode = (GPKModes)pkMode;
						}
					}
					else
					{
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							Global.Data.OtherRoles[roleID].PKMode = pkMode;
						}
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null)
						{
							gsprite.PKMode = (GPKModes)pkMode;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangePKValue(int roleID, int pkValue, int pkPoint)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.PKValue = pkValue;
						Global.Data.roleData.PKPoint = pkPoint;
						if (this.Leader != null)
						{
							this.Leader.VPK = pkValue;
							this.UpdateNameColor(this.Leader, Global.Data.roleData);
							this.UpdatePKValue(Global.Data.roleData, this.Leader);
						}
					}
					else
					{
						RoleData roleData = null;
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
							Global.Data.OtherRoles[roleID].PKValue = pkValue;
							Global.Data.OtherRoles[roleID].PKPoint = pkPoint;
						}
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null)
						{
							gsprite.VPK = pkValue;
							if (roleData != null)
							{
								this.UpdateNameColor(gsprite, roleData);
								this.UpdatePKValue(roleData, gsprite);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeStallName(int roleID, string stallName)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.StallName = stallName;
						if (this.Leader != null)
						{
						}
					}
					else
					{
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							RoleData roleData = Global.Data.OtherRoles[roleID];
							roleData.StallName = stallName;
						}
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null)
						{
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeTeamID(int roleID, int teamID, int teamLeaderRoleID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.TeamID = teamID;
						Global.Data.roleData.TeamLeaderRoleID = teamLeaderRoleID;
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						if (roleData != null)
						{
							roleData.TeamID = teamID;
							roleData.TeamLeaderRoleID = teamLeaderRoleID;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void UpdateOthersTeamLabel()
		{
			if (this.Leader != null)
			{
				string name = string.Empty;
				foreach (int num in Global.Data.OtherRoles.Keys)
				{
					name = StringUtil.substitute("Role_{0}", new object[]
					{
						num
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						this.UpdateTeamFlags(gsprite, Global.Data.OtherRoles[num].TeamID > 0, Global.Data.OtherRoles[num].TeamLeaderRoleID == gsprite.RoleID);
					}
				}
				this.UpdateTeamFlags(this.Leader, Global.Data.roleData.TeamID > 0, Global.Data.roleData.TeamLeaderRoleID == this.Leader.RoleID);
			}
		}

		public void ServerHorse(int roleID, int horseType, int horseDbID, int horseID, int horseBodyID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.HorseDbID = horseDbID;
						if (this.Leader != null)
						{
							if (horseType == 1)
							{
								if (this.Leader.Action != GActions.Stand)
								{
									this.Leader.Action = GActions.Stand;
									GameInstance.Game.SpriteAction((double)this.Leader.Direction, 0, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
								}
								Global.Data.roleData.LastHorseID = horseDbID;
							}
							if (Global.FindMoveStroyboard(this.Leader.Name))
							{
								this.LinearMove(this.Leader, this.Leader.Destination, 2, (int)this.Leader.ExtAction, false);
							}
						}
					}
					else
					{
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && horseType == 1)
						{
							if (gsprite.Action == GActions.Run || gsprite.Action == GActions.Walk)
							{
								gsprite.Action = GActions.Stand;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerPet(int roleID, int petType, int extTag1, string extTag2)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (petType == 1)
					{
						string[] array = extTag2.Split(new char[]
						{
							'$'
						});
						if (array.Length == 7 && Global.CanShowRolePet(true))
						{
							PetRoleItem petRoleItem = new PetRoleItem
							{
								AutoRoleID = Convert.ToInt32(array[0]),
								PetName = array[1],
								Level = Convert.ToInt32(array[2]),
								BodyCode = Convert.ToInt32(array[3]),
								X = (double)Convert.ToInt32(array[4]),
								Y = (double)Convert.ToInt32(array[5]),
								Direction = (double)Convert.ToInt32(array[6])
							};
							string name = StringUtil.substitute("Role_{0}", new object[]
							{
								petRoleItem.AutoRoleID
							});
							GSprite gsprite = this.FindSprite(name);
							if (gsprite != null)
							{
								gsprite.ExternalDeath();
							}
							gsprite = this.LoadPet(petRoleItem);
							if (roleID == Global.Data.RoleID)
							{
								Global.Data.roleData.PetDbID = extTag1;
								if (this.Leader != null)
								{
									this.Leader.Pet = gsprite;
								}
							}
						}
					}
					else if (petType == 2)
					{
						string name2 = StringUtil.substitute("Role_{0}", new object[]
						{
							extTag1
						});
						GSprite gsprite2 = this.FindSprite(name2);
						if (gsprite2 != null)
						{
							gsprite2.ExternalDeath();
							if (roleID == Global.Data.RoleID)
							{
								Global.Data.roleData.PetDbID = -1;
								if (this.Leader != null)
								{
									this.Leader.Pet = null;
								}
							}
						}
					}
					else if (petType == 3)
					{
						string name3 = StringUtil.substitute("Role_{0}", new object[]
						{
							extTag1
						});
						GSprite gsprite3 = this.FindSprite(name3);
						if (gsprite3 != null)
						{
							gsprite3.VSName = extTag2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerLoadAlready(int otherRoleID, int mapCode, int currentX, int currentY, int currentDirection, int action, int toX, int toY, double moveCost, int extAction, double startMoveTicks, string pathString, int currentPathIndex)
		{
			try
			{
				if (this.EnableChangMap)
				{
					GSprite gsprite = this.AddCachingSpriteToCanvas(otherRoleID, currentX, currentY);
					if (gsprite != null)
					{
						this.AddOrRemoveEmblemDeco(gsprite, 0);
						gsprite.EnemyTarget = new Point(toX, toY);
						this.ChangeDirection(gsprite, (double)currentDirection, -1);
						if (gsprite.VLife > 0.0)
						{
							if (action == 2)
							{
								gsprite.Action = (GActions)action;
							}
							else if (action == 24 && gsprite.SpriteType == GSpriteTypes.Monster)
							{
								gsprite.Action = GActions.Attack;
							}
							else
							{
								gsprite.Action = (GActions)action;
							}
							if ((action == 1 || action == 2) && toX >= 0 && toY >= 0)
							{
								this.ServerLinearMove(otherRoleID, mapCode, action, toX, toY, moveCost, extAction, currentX, currentY, startMoveTicks, pathString, currentPathIndex);
							}
							if (gsprite.SpriteType != GSpriteTypes.Monster && Global.WatchSprite == gsprite.Name)
							{
								this.SetLockDeco(gsprite, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerPlayDeco(int roleID, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string text = StringUtil.substitute("temp_deco_{0}", new object[]
					{
						decoID
					});
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerFindMonster(int roleID, int toX, int toY, int totalMonsterNum)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID && this.Leader != null && Global.IsAutoFighting())
					{
						if (this.Leader.Coordinate.X == toX && this.Leader.Coordinate.Y == toY)
						{
							this.CurrentFintMonsterSlot++;
						}
						else
						{
							this.CurrentFintMonsterSlot = 1;
						}
						this.CurrentMapMonsterNum = totalMonsterNum;
						Point point = new Point(toX, toY);
						if (Global.OnObstruction(point, this.CurrentMapData))
						{
							Point to;
							if (this.FindLinearNoObsMaxPoint(this.Leader, point, out to))
							{
								this.LinearMove(this.Leader, to, 2, 0, false);
							}
						}
						else
						{
							this.LinearMove(this.Leader, point, 2, 0, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerPurpleNameTicks(int roleID, long startPurpleNameTicks)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.StartPurpleNameTicks = startPurpleNameTicks;
						if (this.Leader != null)
						{
							this.UpdateNameColor(this.Leader, Global.Data.roleData);
						}
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						roleData.StartPurpleNameTicks = startPurpleNameTicks;
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && roleData != null)
						{
							this.UpdateNameColor(gsprite, roleData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewBiaoChe(BiaoCheData biaoCheData)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Global.Data.BiaoChes[biaoCheData.BiaoCheID] = biaoCheData;
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						biaoCheData.BiaoCheID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite == null)
					{
						gsprite = this.LoadBiaoChe(biaoCheData);
					}
					if (biaoCheData.OwnerRoleID == Global.Data.roleData.RoleID && this.Leader != null)
					{
						this.Leader.BiaoChe = gsprite;
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerHideBiaoChe(int biaoCheID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						biaoCheID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						gsprite.ExternalDeath();
						if (Global.Data.BiaoChes.ContainsKey(biaoCheID))
						{
							BiaoCheData biaoCheData = Global.Data.BiaoChes[biaoCheID];
							if (biaoCheData.OwnerRoleID == Global.Data.RoleID && this.Leader != null)
							{
								this.Leader.BiaoChe = null;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerBiaoCheChageLife(int biaoCheID, int currentLifeV)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.BiaoChes.ContainsKey(biaoCheID))
					{
						BiaoCheData biaoCheData = Global.Data.BiaoChes[biaoCheID];
						biaoCheData.CurrentLifeV = currentLifeV;
					}
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						biaoCheID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (gsprite.VLife > 0.0)
						{
							gsprite.VLife = (double)currentLifeV;
							this.RefreshSpriteLife(gsprite);
							this.RefreshRoleArmor(gsprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeBattleName(int roleID, long startTicks, int nameIndex)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.BattleNameStart = startTicks;
						Global.Data.roleData.BattleNameIndex = nameIndex;
						if (this.Leader != null)
						{
							this.UpdateBattleNameImage(this.Leader, Global.Data.roleData);
						}
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						roleData.BattleNameStart = startTicks;
						roleData.BattleNameIndex = nameIndex;
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && roleData != null)
						{
							this.UpdateBattleNameImage(gsprite, roleData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeHeroIndex(int roleID, int heroIndex)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.HeroIndex = heroIndex;
						if (this.Leader != null)
						{
							this.UpdateHeroIndexImage(this.Leader, Global.Data.roleData);
						}
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						roleData.HeroIndex = heroIndex;
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && roleData != null)
						{
							this.UpdateHeroIndexImage(gsprite, roleData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeBangHuiInfo(int roleID, int faction, string bhName, int bhZhiWu)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						Global.Data.roleData.Faction = faction;
						Global.Data.roleData.BHName = bhName;
						Global.Data.roleData.BHZhiWu = bhZhiWu;
						if (this.Leader != null)
						{
							this.UpdateBangHuiInfo(this.Leader, Global.Data.roleData);
							this.UpdateHuangChengImage(this.Leader, Global.Data.roleData);
							this.UpdateHuangChengDeco(this.Leader, Global.Data.roleData);
							this.UpdateWangChengImage(this.Leader, Global.Data.roleData);
							this.Leader.UpdateRoleLabel(true);
						}
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						roleData.Faction = faction;
						roleData.BHName = bhName;
						roleData.BHZhiWu = bhZhiWu;
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && roleData != null)
						{
							this.UpdateBangHuiInfo(gsprite, roleData);
							this.UpdateHuangChengImage(gsprite, roleData);
							this.UpdateHuangChengDeco(gsprite, roleData);
							this.UpdateWangChengImage(gsprite, roleData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewJunQi(JunQiData junQiData)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Global.Data.JunQis[junQiData.JunQiID] = junQiData;
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						junQiData.JunQiID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						Global.RemoveObject(gsprite, true);
						gsprite = null;
					}
					if (gsprite == null)
					{
						gsprite = this.LoadJunQi(junQiData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerHideJunQi(int JunQiID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						JunQiID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						gsprite.ExternalDeath();
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerJunQiChageLife(int JunQiID, int currentLifeV)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.JunQis.ContainsKey(JunQiID))
					{
						JunQiData junQiData = Global.Data.JunQis[JunQiID];
						junQiData.CurrentLifeV = currentLifeV;
					}
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						JunQiID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (gsprite.VLife > 0.0)
						{
							gsprite.VLife = (double)currentLifeV;
							this.RefreshSpriteLife(gsprite);
							this.RefreshRoleArmor(gsprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChgHuangDiRoleID(int oldHuangDiRoleID, int huangDiRoleID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Global.Data.roleData.HuangDiRoleID = huangDiRoleID;
					int num = (int)ConfigSystemParam.GetSystemParamIntByName("SheLiZhiYuanNPCID");
					int num2 = 2130706432 + num;
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						num2
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						this.HandleSheLiZhiYuanNPCDeco(gsprite);
					}
					if (huangDiRoleID > 0)
					{
						string name2;
						if (huangDiRoleID == Global.Data.roleData.RoleID)
						{
							name2 = "Leader";
						}
						else
						{
							name2 = StringUtil.substitute("Role_{0}", new object[]
							{
								huangDiRoleID
							});
						}
						GSprite gsprite2 = this.FindSprite(name2);
						if (gsprite2 != null)
						{
							this.AddHuangDiDec(gsprite2);
						}
					}
					if (oldHuangDiRoleID > 0)
					{
						string name3;
						if (oldHuangDiRoleID == Global.Data.roleData.RoleID)
						{
							name3 = "Leader";
						}
						else
						{
							name3 = StringUtil.substitute("Role_{0}", new object[]
							{
								oldHuangDiRoleID
							});
						}
						GSprite gsprite3 = this.FindSprite(name3);
						if (gsprite3 != null)
						{
							this.RemoveHuangDiDec(gsprite3);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChgHuangHou(int roleID, int huangHou)
		{
			try
			{
				if (this.EnableChangMap)
				{
					RoleData roleData = null;
					string name;
					if (roleID == Global.Data.roleData.RoleID)
					{
						name = "Leader";
						Global.Data.roleData.HuangHou = huangHou;
						roleData = Global.Data.roleData;
					}
					else
					{
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
							roleData.HuangHou = huangHou;
						}
					}
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (huangHou == 1)
						{
							this.AddHuangHouDec(gsprite, roleData);
						}
						else
						{
							this.RemoveHuangHouDec(gsprite);
						}
						this.UpdateHuangChengImage(gsprite, roleData);
						this.UpdateWangChengImage(gsprite, roleData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeBHZhiWu(int roleID, int bhid, int bhZhiWu)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						if (Global.Data.roleData.Faction == bhid)
						{
							Global.Data.roleData.BHZhiWu = bhZhiWu;
							if (this.Leader != null)
							{
								this.UpdateBangHuiInfo(this.Leader, Global.Data.roleData);
								this.UpdateHuangChengImage(this.Leader, Global.Data.roleData);
								this.UpdateWangChengImage(this.Leader, Global.Data.roleData);
							}
						}
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						if (roleData.Faction == bhid)
						{
							roleData.BHZhiWu = bhZhiWu;
							string name = StringUtil.substitute("Role_{0}", new object[]
							{
								roleID
							});
							GSprite gsprite = this.FindSprite(name);
							if (gsprite != null && roleData != null)
							{
								this.UpdateBangHuiInfo(gsprite, roleData);
								this.UpdateHuangChengImage(gsprite, roleData);
								this.UpdateWangChengImage(gsprite, roleData);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerChangeBHLingDi(int oldBHID, int lingDiID, int bhid, int zoneID, string bhName, int tax)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if ((Global.Data.roleData.Faction == oldBHID || Global.Data.roleData.Faction == bhid) && this.Leader != null)
					{
						this.UpdateLingDiWord(this.Leader, Global.Data.roleData);
						this.UpdateHuangChengImage(this.Leader, Global.Data.roleData);
						this.UpdateHuangChengDeco(this.Leader, Global.Data.roleData);
						this.UpdateWangChengImage(this.Leader, Global.Data.roleData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerEnterAutoFightingProtect(int autoFightingProtect)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Global.Data.roleData.AutoFightingProtect = autoFightingProtect;
					if (this.Leader != null)
					{
						this.UpdateLingDiWord(this.Leader, Global.Data.roleData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ChangeBattleSide(int roleID, int sideID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (roleID == Global.Data.RoleID)
					{
						if (Global.Data.roleData.BattleWhichSide != sideID)
						{
							Global.Data.roleData.BattleWhichSide = sideID;
							this.ChangeBodyCode(this.Leader);
						}
						this.UpdateBattleSideImage(this.Leader, Global.Data.roleData);
					}
					else if (Global.Data.OtherRoles.ContainsKey(roleID))
					{
						RoleData roleData = Global.Data.OtherRoles[roleID];
						string name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						GSprite gsprite = this.FindSprite(name);
						if (gsprite != null && roleData != null)
						{
							if (roleData.BattleWhichSide != sideID)
							{
								roleData.BattleWhichSide = sideID;
								this.ChangeBodyCode(gsprite);
							}
							this.UpdateBattleSideImage(gsprite, roleData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ChangeSpeedRate(int roleID, double speedRate)
		{
			try
			{
				if (!this.EnableChangMap)
				{
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void DSHideCmd(int roleID, long hideStart)
		{
			try
			{
				if (this.EnableChangMap)
				{
					RoleData roleData = null;
					string name;
					if (roleID == Global.Data.roleData.RoleID)
					{
						name = "Leader";
						roleData = Global.Data.roleData;
						roleData.DSHideStart = hideStart;
					}
					else
					{
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
							roleData.DSHideStart = hideStart;
						}
					}
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (hideStart > 0L)
						{
							this.AddDSHide(gsprite, roleData);
						}
						else
						{
							this.RemoveDSHide(gsprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewDeco(DecorationData decoData)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						this.AddNewMapDeco(decoData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerDelDeco(int decoAutoID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (this.Leader != null)
					{
						this.DelMapDeco(decoAutoID);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerRoleBuffDataChange(int roleID, BufferData bufferData)
		{
			long startTime = bufferData.StartTime;
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime < startTime)
			{
			}
			if (!this.EnableChangMap)
			{
				return;
			}
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (Global.Data.OtherRoles.ContainsKey(roleID))
			{
				roleData = Global.Data.OtherRoles[roleID];
			}
			else if (Global.Data.SystemMonsters.ContainsKey(roleID))
			{
				MonsterData monsterData = Global.Data.SystemMonsters[roleID];
			}
			if (roleData != null)
			{
				if (roleData.BufferDataList == null)
				{
					roleData.BufferDataList = new List<BufferData>();
				}
				int num = -1;
				for (int i = 0; i < roleData.BufferDataList.Count; i++)
				{
					if (roleData.BufferDataList[i].BufferID == bufferData.BufferID)
					{
						num = i;
						break;
					}
				}
				if (num < 0)
				{
					roleData.BufferDataList.Add(bufferData);
				}
				else
				{
					roleData.BufferDataList[num] = bufferData;
				}
			}
			string name = Global.FormatRoleID(roleID);
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				BufferItemTypes bufferID = (BufferItemTypes)bufferData.BufferID;
				BufferItemTypes bufferItemTypes = bufferID;
				switch (bufferItemTypes)
				{
				case BufferItemTypes.KingOfBattleCrystal:
				case BufferItemTypes.KingOfBattleBoss_GJDZY:
				case BufferItemTypes.KingOfBattleBoss_GJDJX:
				case BufferItemTypes.KingOfBattleBoss_GJDNH:
					if (Global.IsInKuaFuHuoDongWangZhe())
					{
						if (bufferData.BufferVal == 0L)
						{
							this.RemoveWangZheDec(roleID, bufferData.BufferID);
						}
						else
						{
							this.ServerRoleStatusChanged(roleID, 10, bufferData.StartTime, bufferData.BufferSecs, (double)bufferData.BufferID);
						}
					}
					goto IL_5ED;
				case BufferItemTypes.KarenEastCrystal:
				case BufferItemTypes.CoupleArena_ZhenAi_Buff:
				case BufferItemTypes.CoupleArena_YongQi_Buff:
					break;
				default:
					switch (bufferItemTypes)
					{
					case BufferItemTypes.ZhongShenZhiShen_ChengHao:
						break;
					default:
						switch (bufferItemTypes)
						{
						case BufferItemTypes.Kuafu_Huanying:
							if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
							{
								if (bufferData.BufferVal == 0L)
								{
									this.RemoveShengbeiDec(roleID, bufferData.BufferID);
								}
								else
								{
									this.ServerRoleStatusChanged(roleID, 9, bufferData.StartTime, bufferData.BufferSecs, (double)bufferData.BufferID);
								}
							}
							goto IL_5ED;
						default:
							switch (bufferItemTypes)
							{
							case BufferItemTypes.BianShen:
								if (Global.Data.roleData.RoleID == roleID)
								{
									try
									{
										this.ServerChangeCode(null);
									}
									catch (Exception ex)
									{
										MUDebug.LogError<string>(new string[]
										{
											ex.Message
										});
									}
									try
									{
										if (UICtrlBar.singleton != null)
										{
											if (Global.IsBufferDataOver(bufferData, Global.GetCorrectLocalTime(), false))
											{
												UICtrlBar.singleton.BianShenEnd();
											}
											else
											{
												UICtrlBar.singleton.BianShen();
											}
										}
									}
									catch (Exception ex2)
									{
										MUDebug.LogError<string>(new string[]
										{
											ex2.Message
										});
									}
								}
								else if (Global.Data.OtherRoles.ContainsKey(roleID))
								{
									this.ReloadSprite(roleID);
								}
								goto IL_5ED;
							case BufferItemTypes.LianZhanBuff:
								LianShaMgr.Instance.NoticeRoleLianShaDataChange(bufferData);
								goto IL_5ED;
							case BufferItemTypes.RebornMutilExp:
								goto IL_5ED;
							default:
								if (bufferItemTypes == BufferItemTypes.GuMuTimeLimit || bufferItemTypes == BufferItemTypes.MingJieMapLimit)
								{
									goto IL_5ED;
								}
								if (bufferItemTypes == BufferItemTypes.BangHuiMatchDeHurt_Temple)
								{
									this.RefreshRoleBuffDataValue(roleID, bufferData, false);
									goto IL_5ED;
								}
								if (bufferItemTypes == BufferItemTypes.BangHuiMatchDeHurt_QiZhi)
								{
									this.RefreshRoleBuffDataValue(roleID, bufferData, false);
									goto IL_5ED;
								}
								if (bufferItemTypes == BufferItemTypes.EscapeBattleGod || bufferItemTypes == BufferItemTypes.EscapeBattleDevil)
								{
									this.RefreshRoleBuffDataValue(roleID, bufferData, false);
									goto IL_5ED;
								}
								if (bufferItemTypes != BufferItemTypes.TimeHUZHAONoShow)
								{
									goto IL_5ED;
								}
								if (bufferData.BufferSecs <= 0)
								{
									DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(11012);
									string[] array = decorationVOByCode.ResName.Split(new char[]
									{
										'.'
									});
									if (array != null && array.Length > 0)
									{
										gsprite.Remove(array[0]);
										MUDebug.Log<string>(new string[]
										{
											"清除目标身上的这个buff特效: 11012" + bufferData.BufferID
										});
									}
								}
								goto IL_5ED;
							}
							break;
						case BufferItemTypes.LangHunLingYu_ChengHao:
							break;
						}
						break;
					case BufferItemTypes.JunTuanCaiJiBuff:
						this.RefreshRoleBuffDataValue(roleID, bufferData, false);
						goto IL_5ED;
					case BufferItemTypes.HuiJiHuTi:
						if (Global.Data.roleData.RoleID == roleID)
						{
							Global.RoleEmblemClick(0);
							this.EmblemClearRoleInfluence(gsprite);
						}
						this.AddOrRemoveEmblemDeco(gsprite, 1);
						goto IL_5ED;
					}
					break;
				}
				if (roleData != null)
				{
					if (bufferData.BufferVal <= 0L)
					{
						int num2 = roleData.BufferDataList.FindIndex((BufferData e) => e.BufferID == bufferData.BufferID);
						if (0 <= num2)
						{
							roleData.BufferDataList.RemoveAt(num2);
							if (bufferData.BufferID == 2080002)
							{
								this.DestoryLingDiBuffDes(roleData.RoleID);
							}
						}
					}
					else
					{
						int num3 = roleData.BufferDataList.FindIndex((BufferData e) => e.BufferID == bufferData.BufferID);
						if (0 > num3)
						{
							roleData.BufferDataList.Add(bufferData);
							if (bufferData.BufferID == 2080002)
							{
								this.LoadLingDiBuffDes();
							}
						}
					}
				}
				if (bufferData.BufferID != 2080002)
				{
					this.UpdateTitleImage();
				}
			}
			IL_5ED:
			if (Global.SpecialTitleBuffer(bufferData.BufferID, true) && gsprite != null && gsprite.VLife > 0.0)
			{
				gsprite.UpdateRoleLabel(true);
			}
		}

		public void RefreshRoleBuffDataValue(int RoleID, BufferData bufferData, bool refreshBufferVal = false)
		{
			if (0L >= bufferData.BufferVal)
			{
				RoleData roleData;
				if (RoleID == Global.Data.roleData.RoleID)
				{
					roleData = Global.Data.roleData;
				}
				else
				{
					roleData = Global.FindRoleDataByID(RoleID);
				}
				if (roleData != null)
				{
					NGUITools.ListFastRemove<BufferData>(roleData.BufferDataList, (BufferData e) => e.BufferID == bufferData.BufferID);
				}
			}
		}

		public void ServerRoleStatusChanged(int roleID, int statusID, long startTicks, int slotSeconds, double tag)
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime < startTicks)
			{
				startTicks = correctLocalTime;
			}
			try
			{
				if (roleID == Global.Data.roleData.RoleID && statusID == 13)
				{
					Global.Data.RoleFightState = ((0L >= startTicks) ? 0 : 1);
				}
				if (this.EnableChangMap)
				{
					RoleData roleData = null;
					MonsterData monsterData = null;
					string name;
					if (roleID == Global.Data.roleData.RoleID)
					{
						name = "Leader";
						roleData = Global.Data.roleData;
					}
					else
					{
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
						}
						else if (Global.Data.SystemMonsters.ContainsKey(roleID))
						{
							monsterData = Global.Data.SystemMonsters[roleID];
						}
					}
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (statusID == 0)
						{
							roleData.FSHuDunStart = startTicks;
							roleData.FSHuDunSeconds = slotSeconds;
							this.AddFSHuDunDec(gsprite, roleData);
						}
						else if (statusID == 1)
						{
							if (roleData != null)
							{
								roleData.ZhongDuStart = startTicks;
								roleData.ZhongDuSeconds = slotSeconds;
								this.AddZhongDuStatus(gsprite, roleData);
							}
							else if (monsterData != null)
							{
								monsterData.ZhongDuStart = startTicks;
								monsterData.ZhongDuSeconds = slotSeconds;
								this.AddZhongDuStatusToMonster(gsprite, monsterData);
							}
						}
						else if (statusID == 2)
						{
							if (roleData != null && slotSeconds > 0)
							{
								roleData.DongJieStart = startTicks;
								roleData.DongJieMills = slotSeconds * 1000;
								gsprite.Action = GActions.Stand;
								this.AddMaBiStatus(gsprite, roleData);
							}
						}
						else if (statusID == 3)
						{
							if (roleData != null && slotSeconds > 0)
							{
								roleData.DongJieStart = startTicks;
								roleData.DongJieMills = slotSeconds * 1000;
								gsprite.Action = GActions.Stand;
								this.AddHunMiDec(gsprite, startTicks, slotSeconds);
							}
							else
							{
								this.AddHunMiDec(gsprite, startTicks, slotSeconds);
							}
						}
						else if (statusID == 4)
						{
							if (slotSeconds > 0)
							{
								gsprite.MovingSpeedMills = slotSeconds * 1000;
								gsprite.MoveSpeed = tag;
								this.AddSlowDec(gsprite, startTicks, slotSeconds);
							}
						}
						else if (statusID == 6)
						{
							this.AddAttackDownDec(gsprite, startTicks, slotSeconds);
						}
						else if (statusID == 7)
						{
							this.AddDefenseDownDec(gsprite, startTicks, slotSeconds);
						}
						else if (statusID == 5)
						{
							this.AddHitDownDec(gsprite, startTicks, slotSeconds);
						}
						else if (statusID == 8)
						{
							if (slotSeconds > 0)
							{
								gsprite.MovingSpeedMills = slotSeconds * 1000;
								gsprite.MoveSpeed = tag;
								this.AddDingShenDec(gsprite, startTicks, slotSeconds);
							}
						}
						else if (statusID == 9)
						{
							this.AddShengbeiSlowDown(gsprite, startTicks, slotSeconds, (int)tag);
						}
						else if (statusID == 10)
						{
							if (gsprite != null)
							{
								int bufferDecID = Global.GetBufferDecID((int)tag);
								this.AddBufferDeco(gsprite, bufferDecID, startTicks, slotSeconds);
							}
						}
						else if (statusID == 116)
						{
							if (gsprite != null)
							{
								if (Global.Data.roleData.RoleID == roleID)
								{
									Global.RoleEmblemClick(0);
									this.EmblemClearRoleInfluence(gsprite);
								}
								this.AddOrRemoveEmblemDeco(gsprite, 1);
							}
						}
						else if (statusID == 11)
						{
							if (gsprite != null && slotSeconds > 0)
							{
								gsprite.MovingSpeedMills = slotSeconds * 1000;
								gsprite.MoveSpeed = tag;
							}
						}
						else if (statusID == 12)
						{
							if (gsprite != null)
							{
								if (slotSeconds > 0)
								{
									MUDebug.Log<string>(new string[]
									{
										"<color=yellow>霸体修改玩家速度 = " + tag + "</color>"
									});
									gsprite.MovingSpeedMills = slotSeconds * 1000;
									gsprite.MoveSpeed = tag;
								}
								this.EmblemClearRoleInfluence(gsprite);
								int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EmblemEffect", ',');
								if (systemParamIntArrayByName != null)
								{
									for (int i = 0; i < systemParamIntArrayByName.Length; i++)
									{
										this.RemoveDelayDeco(gsprite, systemParamIntArrayByName[i]);
									}
								}
							}
						}
						else if (statusID == 14)
						{
							gsprite.MoveSpeed = tag;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerJieriChengHaoChanged(int roleID, int jieriChengHao)
		{
			try
			{
				if (this.EnableChangMap)
				{
					RoleData roleData = null;
					string name;
					if (roleID == Global.Data.roleData.RoleID)
					{
						name = "Leader";
						roleData = Global.Data.roleData;
					}
					else
					{
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
						}
					}
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						roleData.JieriChengHao = jieriChengHao;
						this.UpdateJieriWord(gsprite, roleData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerClientHeart(int roleID, double serverTicks, int allowTicks)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (allowTicks > 0)
					{
						double num = (double)Global.GetCorrectLocalTime() - serverTicks;
						CheatMgr.LastClientServerSubTicks += num;
						CheatMgr.LastClientServerSubNum++;
						int num2 = (int)(CheatMgr.LastClientServerSubTicks / (double)CheatMgr.LastClientServerSubNum);
						if (num2 >= allowTicks && this.NotifyYuZhouJiaSuCheat != null)
						{
							this.NotifyYuZhouJiaSuCheat(this, new NotifyYuZhouJiaSuEventArgs
							{
								CorrectLocalTime = Global.GetCorrectLocalTime(),
								ServerTicks = serverTicks,
								AvgTicks = (double)num2,
								LastClientServerSubNum = (double)CheatMgr.LastClientServerSubNum,
								AllowTicks = (double)allowTicks
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerNewFakeRole(FakeRoleData fakeRoleData)
		{
			try
			{
				if (this.EnableChangMap)
				{
					Global.Data.FakeRoles[fakeRoleData.FakeRoleID] = fakeRoleData;
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						fakeRoleData.FakeRoleID
					});
					if (this.FindSprite(name) == null)
					{
						GSprite gsprite = this.LoadFakeRole(fakeRoleData, true);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerHideFakeRole(int FakeRoleID)
		{
			try
			{
				if (this.EnableChangMap)
				{
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						FakeRoleID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						Global.RemoveObject(gsprite, true);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerFakeRoleChageLife(int FakeRoleID, int currentLifeV)
		{
			try
			{
				if (this.EnableChangMap)
				{
					if (Global.Data.FakeRoles.ContainsKey(FakeRoleID))
					{
						FakeRoleData fakeRoleData = Global.Data.FakeRoles[FakeRoleID];
						fakeRoleData.MyRoleDataMini.LifeV = currentLifeV;
					}
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						FakeRoleID
					});
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						if (gsprite.VLife > 0.0)
						{
							gsprite.VLife = (double)currentLifeV;
							this.RefreshSpriteLife(gsprite);
							this.RefreshRoleArmor(gsprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerOpenMarket(int roleID, string marketName)
		{
			try
			{
				if (this.EnableChangMap)
				{
					RoleData roleData = null;
					string name;
					if (roleID == Global.Data.roleData.RoleID)
					{
						name = "Leader";
						roleData = Global.Data.roleData;
					}
					else
					{
						name = StringUtil.substitute("Role_{0}", new object[]
						{
							roleID
						});
						if (Global.Data.OtherRoles.ContainsKey(roleID))
						{
							roleData = Global.Data.OtherRoles[roleID];
						}
					}
					GSprite gsprite = this.FindSprite(name);
					if (gsprite != null)
					{
						roleData.StallName = marketName;
						this.UpdateMarketName(gsprite, roleData);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void ServerStartMeditate(int roleID, int meditateState)
		{
			try
			{
				if (!this.EnableChangMap)
				{
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void LoadPKloversBuffer(CoupleArenaBuffHoldData Data)
		{
			try
			{
				RoleData roleData = null;
				RoleData roleData2 = null;
				string name = string.Empty;
				GSprite gsprite = null;
				GSprite gsprite2 = null;
				if (Data.YongQiHolderRname != null && Global.Data.roleData.RoleName.Equals(Data.YongQiHolderRname))
				{
					roleData2 = Global.Data.roleData;
					name = "Leader";
					gsprite2 = this.FindSprite(name);
				}
				else if (Data.YongQiHolderRname != null && Global.Data.OtherRolesByName.TryGetValue(Data.YongQiHolderRname, ref roleData2))
				{
					name = StringUtil.substitute("Role_{0}", new object[]
					{
						roleData2.RoleID
					});
					gsprite2 = this.FindSprite(name);
				}
				if (Data.ZhenAiHolderRname != null && Global.Data.OtherRolesByName.TryGetValue(Data.ZhenAiHolderRname, ref roleData))
				{
					name = StringUtil.substitute("Role_{0}", new object[]
					{
						roleData.RoleID
					});
					gsprite = this.FindSprite(name);
				}
				else if (Data.ZhenAiHolderRname != null && Global.Data.roleData.RoleName.Equals(Data.ZhenAiHolderRname))
				{
					roleData = Global.Data.roleData;
					name = "Leader";
					gsprite = this.FindSprite(name);
				}
				if (gsprite != null)
				{
					int bufferDecID = Global.GetBufferDecID(2080010);
					this.AddPKLoversBufferDeco(gsprite, bufferDecID);
				}
				if (gsprite2 != null)
				{
					int bufferDecID2 = Global.GetBufferDecID(2080011);
					this.AddPKLoversBufferDeco(gsprite2, bufferDecID2);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public void AddPKLoversBufferDeco(GSprite sprite, int BufferCode)
		{
			if (sprite == null)
			{
				return;
			}
			string name = string.Format("BufferDecoration_{0}", BufferCode);
			GDecoration gdecoration = sprite.Root.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			string name2 = string.Format("BufferDecoration_{0}", 15001);
			gdecoration = (sprite.Root.FindName(name2) as GDecoration);
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			gdecoration = (ObjectsManager.FindSprite(name) as GDecoration);
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			Point pos = new Point(sprite.CenterX, sprite.CenterY);
			gdecoration = Global.GetDecoration(BufferCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
			if (gdecoration != null)
			{
				gdecoration.Name = name;
				if (gdecoration.HangPos == 1)
				{
					sprite.Root.Children.Add(gdecoration);
				}
				else
				{
					gdecoration.cx = sprite.cx;
					gdecoration.cy = sprite.cy;
					ObjectsManager.Add(gdecoration);
				}
			}
		}

		public void LoadLingDiBuffDes()
		{
			int bufferDecID = Global.GetBufferDecID(2080002);
			Dictionary<int, RoleData>.Enumerator enumerator = Global.Data.OtherRoles.GetEnumerator();
			IL_E0:
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, RoleData> keyValuePair = enumerator.Current;
				if (keyValuePair.Value.BufferDataList != null)
				{
					int num = 0;
					for (;;)
					{
						int num2 = num;
						KeyValuePair<int, RoleData> keyValuePair2 = enumerator.Current;
						if (num2 >= keyValuePair2.Value.BufferDataList.Count)
						{
							goto IL_E0;
						}
						KeyValuePair<int, RoleData> keyValuePair3 = enumerator.Current;
						if (keyValuePair3.Value.BufferDataList[num].BufferID == 2080002)
						{
							break;
						}
						num++;
					}
					string text = "Role_{0}";
					object[] array = new object[1];
					int num3 = 0;
					KeyValuePair<int, RoleData> keyValuePair4 = enumerator.Current;
					array[num3] = keyValuePair4.Value.RoleID;
					string name = StringUtil.substitute(text, array);
					GSprite sprite = this.FindSprite(name);
					this.AddPKLoversBufferDeco(sprite, bufferDecID);
				}
			}
			for (int i = 0; i < Global.Data.roleData.BufferDataList.Count; i++)
			{
				if (Global.Data.roleData.BufferDataList[i].BufferID == 2080002)
				{
					string name = "Leader";
					GSprite sprite = this.FindSprite(name);
					this.AddPKLoversBufferDeco(sprite, bufferDecID);
					break;
				}
			}
		}

		public void DestoryLingDiBuffDes(int roleID)
		{
			string name = Global.FormatRoleID(roleID);
			GSprite gsprite = this.FindSprite(name);
			if (gsprite == null)
			{
				return;
			}
			int bufferDecID = Global.GetBufferDecID(2080002);
			string name2 = string.Format("BufferDecoration_{0}", bufferDecID);
			GDecoration gdecoration = gsprite.Root.FindName(name2) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			gdecoration = (ObjectsManager.FindSprite(name2) as GDecoration);
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
		}

		public void RoleSuboccupattonChange(int RoleID, int subOccupation)
		{
			RoleData roleData = null;
			string text = string.Empty;
			if (RoleID == Global.Data.RoleID)
			{
				text = "Leader";
				if (Global.Data.roleData.Occupation != 3 && Global.Data.roleData.Occupation != 4)
				{
					return;
				}
				roleData = Global.Data.roleData;
			}
			else
			{
				text = StringUtil.substitute("Role_{0}", new object[]
				{
					RoleID
				});
				Global.Data.OtherRoles.TryGetValue(RoleID, ref roleData);
			}
			if (roleData != null)
			{
				if (roleData.Occupation != 3 && roleData.Occupation != 4)
				{
					return;
				}
				roleData.SubOccupation = subOccupation;
			}
			GSprite gsprite = this.FindSprite(text);
			if (gsprite != null)
			{
				if (gsprite.VLife <= 0.0)
				{
					return;
				}
				this.ChangeBodyCode(gsprite);
			}
			if ("Leader".Equals(text))
			{
				if (Global.Data != null && Global.Data.GameScene != null)
				{
					Global.Data.GameScene.CancelAutoFight(0, true);
				}
				if (GameInstance.Game != null)
				{
					GameInstance.Game.SpriteModKeys(0, string.Empty);
				}
			}
		}

		public bool RemoveWaitingLoadOtherRole(int otherRoleID)
		{
			for (int i = 0; i < this.RoleItemList.Count; i++)
			{
				if (this.RoleItemList[i].RoleData.RoleID == otherRoleID)
				{
					this.RoleItemList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public bool FindWaitingLoadOtherRole(int otherRoleID)
		{
			for (int i = 0; i < this.RoleItemList.Count; i++)
			{
				if (this.RoleItemList[i].RoleData.RoleID == otherRoleID)
				{
					return true;
				}
			}
			return false;
		}

		public void ToLoadOtherRole(RoleData roleData, double x, double y, double direction, bool addToCanvas)
		{
			OtherRoleItem otherRoleItem = new OtherRoleItem
			{
				RoleData = roleData,
				X = x,
				Y = y,
				Direction = direction
			};
			if (addToCanvas)
			{
				this.AddListRole(otherRoleItem, addToCanvas);
				return;
			}
			this.RoleItemList.Add(otherRoleItem);
		}

		private void AddListRole(bool addToCanvas)
		{
			if (this.RoleItemList.Count <= 0)
			{
				return;
			}
			int num = 1;
			while (this.RoleItemList.Count > 0 && num-- > 0)
			{
				OtherRoleItem otherRoleItem = this.RoleItemList[0];
				this.RoleItemList.RemoveAt(0);
				this.AddListRole(otherRoleItem, addToCanvas);
			}
		}

		private void AddListRoleByID(int roleID, bool addToCanvas)
		{
			if (this.RoleItemList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.RoleItemList.Count; i++)
			{
				if (this.RoleItemList[i].RoleData.RoleID == roleID)
				{
					OtherRoleItem otherRoleItem = this.RoleItemList[i];
					this.RoleItemList.RemoveAt(i);
					this.AddListRole(otherRoleItem, addToCanvas);
					break;
				}
			}
		}

		private void AddListRole(OtherRoleItem otherRoleItem, bool addToCanvas)
		{
			if (otherRoleItem == null)
			{
				return;
			}
			string name = string.Format("Role_{0}", otherRoleItem.RoleData.RoleID);
			GSprite gsprite = this.FindSprite(name);
			if (0 >= otherRoleItem.RoleData.LifeV)
			{
				return;
			}
			if (gsprite != null)
			{
				if (gsprite.IsDeath && otherRoleItem.RoleData.LifeV > 0)
				{
					this.RoleItemList.Add(otherRoleItem);
					return;
				}
				if (otherRoleItem.RoleData.BodyCode != gsprite.EquipmentBody || gsprite.EquipmentWeapon != otherRoleItem.RoleData.WeaponCode)
				{
					gsprite.ExternalDeath();
					gsprite = this.LoadRole(otherRoleItem.RoleData, otherRoleItem.X, otherRoleItem.Y, otherRoleItem.Direction, addToCanvas);
					if (!addToCanvas && gsprite != null && !this.LoadingSpriteCacheDict.ContainsKey(gsprite.RoleID))
					{
						this.LoadingSpriteCacheDict[gsprite.RoleID] = gsprite;
						GameInstance.Game.SpriteLoadAlready(gsprite.RoleID);
					}
				}
				return;
			}
			else
			{
				if (!addToCanvas && this.LoadingSpriteCacheDict.ContainsKey(otherRoleItem.RoleData.RoleID))
				{
					gsprite = this.LoadingSpriteCacheDict[otherRoleItem.RoleData.RoleID];
					if (gsprite.VLife != (double)otherRoleItem.RoleData.LifeV)
					{
						gsprite.VLife = (double)otherRoleItem.RoleData.LifeV;
					}
					return;
				}
				gsprite = this.LoadRole(otherRoleItem.RoleData, otherRoleItem.X, otherRoleItem.Y, otherRoleItem.Direction, addToCanvas);
				if (gsprite != null)
				{
					if (gsprite.VLife <= 0.0)
					{
						gsprite.VLife = (double)otherRoleItem.RoleData.MaxLifeV;
						otherRoleItem.RoleData.LifeV = otherRoleItem.RoleData.MaxLifeV;
					}
					if (!addToCanvas && !this.LoadingSpriteCacheDict.ContainsKey(gsprite.RoleID))
					{
						this.LoadingSpriteCacheDict[gsprite.RoleID] = gsprite;
						GameInstance.Game.SpriteLoadAlready(gsprite.RoleID);
					}
				}
				return;
			}
		}

		public GSprite LoadRole(RoleData roleData, double x, double y, double direction, bool addToCanvas)
		{
			int myTimer = Global.GetMyTimer();
			myTimer = Global.GetMyTimer();
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				roleData.RoleID
			});
			GSprite sprite = new GSprite();
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return null;
			}
			sprite.OldGridX = roleData.PosX / this.CurrentMapData.GridSizeX;
			sprite.OldGridY = roleData.PosY / this.CurrentMapData.GridSizeX;
			sprite.BattleWhichSide = roleData.BattleWhichSide;
			sprite.SpriteType = GSpriteTypes.Other;
			sprite.CoordinateChanged += delegate(GSprite sender)
			{
				this.UpdateRoleEvent(sender);
			};
			sprite.SpriteDead += delegate(object s, EventArgs e)
			{
				if (this.SpriteDeadNotify != null)
				{
					this.SpriteDeadNotify(this, new SpriteNotifyEventArgs
					{
						RoleID = (s as GSprite).RoleID,
						SpriteType = (s as GSprite).SpriteType,
						ShowDlg = false,
						ExtensionID = (s as GSprite).ExtensionID
					});
				}
				if ((s as GSprite).Name == Global.WatchSprite)
				{
					this.SetLockDeco(s as GSprite, false);
					if (this.Leader.LockObject == Global.WatchSprite)
					{
						this.Leader.LockObject = null;
					}
					Global.WatchSprite = null;
				}
				if (Global.Data.OtherRoles.ContainsKey((s as GSprite).RoleID))
				{
					RoleData roleData2 = Global.Data.OtherRoles[(s as GSprite).RoleID];
					roleData2.ZhongDuStart = 0L;
					roleData2.ZhongDuSeconds = 0;
				}
				if (sprite.shouHuChongController != null)
				{
					sprite.shouHuChongController.Dispose();
					Object.Destroy(sprite.shouHuChongController);
					sprite.shouHuChongController = null;
				}
				this.AddSpriteDeadDeco(s as GSprite);
			};
			myTimer = Global.GetMyTimer();
			int myTimer2 = Global.GetMyTimer();
			this.LoadSprite(sprite, roleData.RoleID, roleData.RoleSex, name, Global.GetRoleBHName(roleData), roleData.OtherName, roleData.RoleName, roleData.Occupation, -1, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, Global.Data.SnameBrushColor, (double)roleData.LifeV, roleData.PKMode, roleData.PKValue, -1.0, roleData.BodyCode, roleData.WeaponCode, new Point((int)x, (int)y), (int)direction, (double)Global.Data.LifeTotalWidth, 1.0, roleData.Faction, addToCanvas);
			sprite.PKKingSpriteName = Global.GetPKKingSpriteName(roleData);
			myTimer = Global.GetMyTimer();
			sprite.VLifeMax = (double)roleData.MaxLifeV;
			sprite.VArmorMax = (long)roleData.MaxArmorV;
			sprite.VArmor = (long)roleData.CurrentArmorV;
			this.RefreshSpriteLife(sprite);
			this.RefreshRoleArmor(sprite);
			this.UpdateNameColor(sprite, roleData);
			sprite.VPK = roleData.PKValue;
			this.UpdatePKValue(roleData, sprite);
			this.UpdateTeamFlags(sprite, roleData.TeamID > 0, roleData.TeamLeaderRoleID == sprite.RoleID);
			this.UpdateSkillWord(sprite, roleData);
			this.UpdateLittleVIP(sprite, roleData);
			this.UpdateJingMaiWord(sprite, roleData);
			this.UpdateBattleNameImage(sprite, roleData);
			this.UpdateHeroIndexImage(sprite, roleData);
			this.UpdateChengJiuImage(sprite, roleData);
			this.UpdateChuanQiJingMaiImage(sprite, roleData);
			this.UpdateChuanWuXueImage(sprite, roleData);
			this.UpdateHuangChengImage(sprite, roleData);
			this.UpdateWangChengImage(sprite, roleData);
			this.UpdateLingDiWord(sprite, roleData);
			this.UpdateBattleSideImage(sprite, roleData);
			this.UpdateJieriWord(sprite, roleData);
			this.UpdateMarketName(sprite, roleData);
			this.AddMingZhongDec(sprite, roleData);
			myTimer = Global.GetMyTimer();
			sprite.IsInSafeRegion = this.InSafeRegion(sprite);
			if (roleData.LifeV <= 0)
			{
				sprite.Action = GActions.Death;
			}
			if (!Global.Data.SysSetting.HideOtherRoles)
			{
				sprite.Start();
			}
			else
			{
				sprite.Start();
				sprite.HideObject();
			}
			Global.AddSpriteJingMaiUpDeco(sprite, roleData);
			this.AddHuangDiDec(sprite);
			this.AddHuangHouDec(sprite, roleData);
			this.AddFSHuDunDec(sprite, roleData);
			this.AddZhongDuStatus(sprite, roleData);
			this.AddDSHide(sprite, roleData);
			this.AddMaBiStatus(sprite, roleData);
			sprite.ChangeWeaponsPosition(this.InSafeRegion(sprite));
			return sprite;
		}

		private void UpdateRoleEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				Global.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
				bool flag = this.InSafeRegion(sprite);
				if (flag != sprite.IsInSafeRegion)
				{
					sprite.ChangeWeaponsPosition(flag);
				}
			}
		}

		public void HideAllOtherRoles()
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if (gsprite.SpriteType == GSpriteTypes.Other || gsprite.SpriteType == GSpriteTypes.FakeRole || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe))
					{
						gsprite.HideObject();
					}
				}
			}
		}

		public void ShowAllOtherRoles()
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if (gsprite.SpriteType == GSpriteTypes.Other || (gsprite.SpriteType == GSpriteTypes.Pet && gsprite != this.Leader.Pet) || (gsprite.SpriteType == GSpriteTypes.BiaoChe && gsprite != this.Leader.BiaoChe))
					{
						gsprite.Start();
						gsprite.ShowObject();
					}
				}
			}
		}

		public void UpdateOtherRoleVisible()
		{
		}

		protected GSprite GetLastSelectedSprite()
		{
			if (this.LastSelectedSprite == null)
			{
				return null;
			}
			int timer = U3DUtils.GetTimer();
			if (timer - this.SelectedSpriteTicks >= 350)
			{
				this.LastSelectedSprite = null;
				return null;
			}
			if (Global.WatchSprite != null || Global.ViewSprite != null)
			{
				return null;
			}
			return this.LastSelectedSprite;
		}

		protected bool HitIObject(Vector3 position)
		{
			if (this.SelectedSprite != null)
			{
				this.SelectedSprite.ShadowVisible = false;
				this.LastSelectedSprite = this.SelectedSprite;
				this.SelectedSprite = null;
			}
			if (this.SelectedGoodsPack != null)
			{
				this.SelectedGoodsPack = null;
			}
			int layer = 1 << LayerMask.NameToLayer("Sprites");
			GameObject gameObject = U3DUtils.HitTest(position, layer);
			if (null != gameObject)
			{
				IObject gameObjectOwnerObject = U3DUtils.GetGameObjectOwnerObject(gameObject);
				if (gameObjectOwnerObject != null)
				{
					if (gameObjectOwnerObject is GGoodsPack)
					{
						this.SelectedGoodsPack = (gameObjectOwnerObject as GGoodsPack);
					}
					else
					{
						GSprite gsprite = gameObjectOwnerObject as GSprite;
						if (gsprite != this.Leader)
						{
							this.SelectedSprite = gsprite;
							this.SelectedSpriteTicks = U3DUtils.GetTimer();
						}
					}
				}
			}
			return this.SelectedSprite != null || null != this.SelectedGoodsPack;
		}

		private Point GetAOffsetGridPoint(Point pt1, Point pt2, int gridNum)
		{
			if (gridNum <= 0)
			{
				return pt1;
			}
			GMapData currentMapData = this.CurrentMapData;
			int num = (int)Global.GetDirectionByAspect(pt2.X, pt2.Y, pt1.X, pt1.Y);
			for (int i = num; i < 8; i++)
			{
				List<Point> gridList = ChuanQiUtils.GetGridList(pt1, i, gridNum, true, null);
				Point grid = gridList[gridList.Count - 1];
				if (!Global.OnObstructionByGrid(grid, currentMapData))
				{
					int x = grid.X;
					int y = grid.Y;
					Point result = new Point(x * currentMapData.GridSizeX + currentMapData.GridSizeX / 2, y * currentMapData.GridSizeY + currentMapData.GridSizeY / 2);
					return result;
				}
			}
			for (int j = 0; j < num; j++)
			{
				List<Point> gridList2 = ChuanQiUtils.GetGridList(pt1, j, gridNum, true, null);
				Point grid2 = gridList2[gridList2.Count - 1];
				if (!Global.OnObstructionByGrid(grid2, currentMapData))
				{
					int x2 = grid2.X;
					int y2 = grid2.Y;
					Point result2 = new Point(x2 * currentMapData.GridSizeX + currentMapData.GridSizeX / 2, y2 * currentMapData.GridSizeY + currentMapData.GridSizeY / 2);
					return result2;
				}
			}
			return pt1;
		}

		private Point GetMapCursorPoint(Point p)
		{
			return p;
		}

		private int GetMapPointValue(int value, int containerSizeValue, int mapSizeValue)
		{
			if (mapSizeValue <= containerSizeValue)
			{
				return 0;
			}
			if (value - containerSizeValue / 2 <= 0)
			{
				return 0;
			}
			if (value >= mapSizeValue - containerSizeValue / 2)
			{
				return containerSizeValue - mapSizeValue;
			}
			return containerSizeValue / 2 - value;
		}

		public int GetLeaderMapPointValueX(double scale, int containerSizeValue, int mapSizeValue)
		{
			if (scale == 0.0)
			{
				return 0;
			}
			double num = (double)this.Leader.cx / scale;
			if (num - (double)(containerSizeValue / 2) <= 0.0)
			{
				return 0;
			}
			if (num >= (double)(mapSizeValue - containerSizeValue / 2))
			{
				return containerSizeValue - mapSizeValue;
			}
			return (int)((double)(containerSizeValue / 2) - num);
		}

		public int GetLeaderMapPointValueY(double scale, int containerSizeValue, int mapSizeValue)
		{
			if (scale == 0.0)
			{
				return 0;
			}
			double num = (double)this.Leader.cy / scale;
			if (num - (double)(containerSizeValue / 2) <= 0.0)
			{
				return 0;
			}
			if (num >= (double)(mapSizeValue - containerSizeValue / 2))
			{
				return containerSizeValue - mapSizeValue;
			}
			return (int)((double)(containerSizeValue / 2) - num);
		}

		public bool MuchClick()
		{
			if (this.MuchClickNum <= 3)
			{
				return false;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			return (double)correctLocalTime - this.LeftButtonClickTicks < 350.0 && this.SelectedSprite == null && null == this.SelectedGoodsPack;
		}

		private byte SetLockDeco(GSprite sprite, bool locked)
		{
			if (locked)
			{
				if (sprite != null)
				{
					if (sprite.Action == GActions.Death)
					{
						return 0;
					}
					if (sprite.Root.FindName("lockedDeco") == null)
					{
						int num;
						if (Global.IsZhaoHuanShou(sprite.RoleID))
						{
							string name = string.Format("Role_{0}", Global.Data.SystemMonsters[sprite.RoleID].MasterRoleID);
							GSprite gsprite = this.FindName(name) as GSprite;
							if (gsprite != null)
							{
								num = ((!Global.IsOpposition(this.Leader, gsprite, false, this.CurrentMapData.PKMode)) ? 30 : 31);
							}
							else
							{
								num = ((!Global.IsOpposition(this.Leader, sprite, false, this.CurrentMapData.PKMode)) ? 30 : 31);
							}
						}
						else
						{
							num = ((!Global.IsOpposition(this.Leader, sprite, false, this.CurrentMapData.PKMode)) ? 30 : 31);
						}
						GDecoration decoration = Global.GetDecoration(num, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
						decoration.Name = "lockedDeco";
						sprite.Root.Children.Add(decoration);
						return (byte)num;
					}
				}
			}
			else if (sprite != null)
			{
				GDecoration gdecoration = sprite.Root.FindName("lockedDeco") as GDecoration;
				if (gdecoration != null)
				{
					Global.RemoveObject(gdecoration, true);
				}
			}
			return 0;
		}

		private void SetLockTargetDeco(GSprite sprite, bool locked)
		{
			if (sprite != null)
			{
				if (locked)
				{
					if (sprite.Root.FindName("lockTargetDeco") == null && !this.Leader.IsInSafeRegion && Global.IsOpposition(this.Leader, sprite, false, this.CurrentMapData.PKMode))
					{
						GDecoration decoration = Global.GetDecoration(305, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
						decoration.Name = "lockTargetDeco";
						sprite.Root.Children.Add(decoration);
					}
				}
				else
				{
					GDecoration gdecoration = sprite.Root.FindName("lockTargetDeco") as GDecoration;
					if (gdecoration != null)
					{
						Global.RemoveObject(gdecoration, true);
					}
				}
			}
		}

		private void ShowMouseLeftButtonUpEffect(Point p)
		{
			GMapData currentMapData = this.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = p.X / currentMapData.GridSizeX;
			int num2 = p.Y / currentMapData.GridSizeY;
			p = new Point(num * currentMapData.GridSizeX + currentMapData.GridSizeX / 2, num2 * currentMapData.GridSizeY + currentMapData.GridSizeY / 2);
			if (this.LeftMouseClickDeco == null)
			{
				GDecoration decoration = Global.GetDecoration(98, GDecorationTypes.Loop, p, true, null, -1, -1, true, false);
				this.Add(decoration);
				this.LeftMouseClickDeco = decoration;
			}
			else
			{
				this.LeftMouseClickDeco.ShowObject();
				this.LeftMouseClickDeco.cx = p.X;
				this.LeftMouseClickDeco.cy = p.Y;
			}
		}

		private void HideMouseLeftButtonUpEffect()
		{
			if (this.LeftMouseClickDeco != null)
			{
				this.LeftMouseClickDeco.HideObject();
			}
		}

		private void RefreshSpriteLife(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
			if (sprite.VLifeMax > 0.0)
			{
				sprite.LifeWidth = Math.Max(0.0, sprite.VLife / sprite.VLifeMax * sprite.LifeTotalWidth);
			}
			if (this.Leader != null && sprite.RoleID == this.Leader.RoleID && 0.0 <= sprite.VLife && sprite.VLife <= sprite.VLifeMax)
			{
				float num = (float)sprite.VLife / (float)sprite.VLifeMax;
				if (0.1 >= (double)num && 0f <= num)
				{
					PlayZone.GlobalPlayZone.OpenRoleLowLifeWindow();
				}
				else
				{
					PlayZone.GlobalPlayZone.CloseRoleLowLifeWindow();
				}
			}
		}

		private void RefreshRoleArmor(GSprite sprite)
		{
			if (sprite != null && (sprite.SpriteType == GSpriteTypes.Leader || sprite.SpriteType == GSpriteTypes.Other || sprite.SpriteType == GSpriteTypes.FakeRole))
			{
				if (0L < sprite.VArmorMax)
				{
					float value = Mathf.Clamp01((float)sprite.VArmor / (float)sprite.VArmorMax);
					if (null != PlayZone.GlobalPlayZone)
					{
						PlayZone.GlobalPlayZone.RefreshRoleArmor(sprite.RoleID, value);
					}
				}
				else if (null != PlayZone.GlobalPlayZone)
				{
					PlayZone.GlobalPlayZone.RefreshRoleArmor(sprite.RoleID, 0f);
				}
			}
		}

		private void ChangeBodyCode(GSprite sprite)
		{
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				Global.RemoveStoryboard(this.Leader.Name);
			}
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				int x = sprite.Coordinate.X;
				int y = sprite.Coordinate.Y;
				int direction = sprite.Direction;
				Global.RemoveObject(sprite, true);
				this.LoadLeader(x, y, direction, false);
			}
			else if (Global.Data.OtherRoles.ContainsKey(sprite.RoleID))
			{
				RoleData roleData = Global.Data.OtherRoles[sprite.RoleID];
				int x2 = sprite.Coordinate.X;
				int y2 = sprite.Coordinate.Y;
				int direction2 = sprite.Direction;
				Global.RemoveObject(sprite, true);
				this.LoadRole(roleData, (double)x2, (double)y2, (double)direction2, true);
			}
		}

		private SolidColorBrush GetSolidColorBrush(string name)
		{
			if (name == "灰色")
			{
				return new SolidColorBrush(4286611584U);
			}
			if (name == "绿色")
			{
				return new SolidColorBrush(4278222848U);
			}
			if (name == "红色")
			{
				return new SolidColorBrush(4294901760U);
			}
			if (name == "蓝色")
			{
				return new SolidColorBrush(4278190335U);
			}
			return null;
		}

		private int RemoveDelayDeco(GSprite sprite, int delayDecorationID)
		{
			int result = -1;
			if (delayDecorationID >= 0)
			{
				string name = string.Format("DelayDecoration_{0}", delayDecorationID);
				GDecoration gdecoration = sprite.Root.FindName(name) as GDecoration;
				if (gdecoration != null)
				{
					Global.RemoveObject(gdecoration, true);
				}
				gdecoration = (ObjectsManager.FindSprite(name) as GDecoration);
				if (gdecoration != null)
				{
					Global.RemoveObject(gdecoration, true);
				}
			}
			return result;
		}

		private int AddDelayDeco(GSprite sprite, int magicCode, long startTicks, int slotSeconds = 0)
		{
			int result = -1;
			if (magicCode < 0)
			{
				return result;
			}
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
			if (maigcInfoVOByCode == null)
			{
				return result;
			}
			if (maigcInfoVOByCode.DelayDecoToMap > 0)
			{
				return result;
			}
			int num = slotSeconds;
			if (num <= 0)
			{
				string magicTime = maigcInfoVOByCode.MagicTime;
				if (string.IsNullOrEmpty(magicTime))
				{
					return result;
				}
				string[] array = magicTime.Split(new char[]
				{
					','
				});
				if (array.Length <= 0)
				{
					return result;
				}
				SkillData skillDataByID = Global.GetSkillDataByID(magicCode);
				int num2 = (skillDataByID != null) ? (skillDataByID.SkillLevel - 1) : 0;
				num2 = Math.Min(num2, array.Length - 1);
				num = Global.SafeConvertToInt32(array[num2]);
				if (num <= 0)
				{
					return result;
				}
			}
			long num3 = startTicks + (long)(num * 1000);
			if (Global.GetCorrectLocalTime() >= num3)
			{
				return result;
			}
			int magicCode2 = Global.GetMagicCode(maigcInfoVOByCode, "DelayDecoration", sprite);
			bool flag = true;
			if (sprite.SpriteType == GSpriteTypes.Other)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("NoShowMagicEffectID", ',');
				if (systemParamIntArrayByName.IndexOf(magicCode2) >= 0)
				{
					flag = false;
				}
			}
			if (magicCode2 >= 0 && flag)
			{
				string name = string.Format("DelayDecoration_{0}", magicCode2);
				GDecoration gdecoration = sprite.Root.FindName(name) as GDecoration;
				if (gdecoration != null)
				{
					Global.RemoveObject(gdecoration, true);
				}
				gdecoration = (ObjectsManager.FindSprite(name) as GDecoration);
				if (gdecoration != null)
				{
					Global.RemoveObject(gdecoration, true);
				}
				Point pos = new Point(sprite.CenterX, sprite.CenterY);
				gdecoration = Global.GetDecoration(magicCode2, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
				if (gdecoration != null)
				{
					gdecoration.LifeTicks = num3;
					gdecoration.Name = name;
					if (gdecoration.HangPos == 1)
					{
						sprite.Root.Children.Add(gdecoration);
					}
					else
					{
						gdecoration.cx = sprite.cx;
						gdecoration.cy = sprite.cy;
						ObjectsManager.Add(gdecoration);
					}
				}
				result = magicCode2;
			}
			string magicColor = maigcInfoVOByCode.MagicColor;
			if (magicColor != null && string.Empty != magicColor)
			{
				SolidColorBrush solidColorBrush = this.GetSolidColorBrush(magicColor);
				sprite.MaskBrush = solidColorBrush;
			}
			return result;
		}

		private int AddDelayDeco(int enemyX, int enemyY, int magicCode)
		{
			int result = -1;
			if (Math.Abs(enemyX - this.Leader.Coordinate.X) < this.CurrentMapData.MapWidth / 2 && Math.Abs(enemyY - this.Leader.Coordinate.Y) < this.CurrentMapData.MapHeight / 2)
			{
				if (magicCode < 0)
				{
					return result;
				}
				MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
				if (maigcInfoVOByCode == null)
				{
					return result;
				}
				if (maigcInfoVOByCode.DelayDecoToMap > 0)
				{
					return result;
				}
				string magicTime = maigcInfoVOByCode.MagicTime;
				if (string.IsNullOrEmpty(magicTime))
				{
					return result;
				}
				string[] array = magicTime.Split(new char[]
				{
					','
				});
				if (array.Length <= 0)
				{
					return result;
				}
				SkillData skillDataByID = Global.GetSkillDataByID(magicCode);
				int num = (skillDataByID != null) ? (skillDataByID.SkillLevel - 1) : 0;
				num = Math.Min(num, array.Length - 1);
				int num2 = Global.SafeConvertToInt32(array[num]);
				if (num2 <= 0)
				{
					return result;
				}
				long lifeTicks = Global.GetCorrectLocalTime() + (long)(num2 * 1000);
				int magicCode2 = Global.GetMagicCode(maigcInfoVOByCode, "DelayDecoration", null);
				if (magicCode2 >= 0)
				{
					Point pos = new Point(enemyX, enemyY);
					GDecoration decoration = Global.GetDecoration(magicCode2, GDecorationTypes.Loop, pos, true, null, -1, -1, true, false);
					if (decoration != null)
					{
						decoration.LifeTicks = lifeTicks;
						this.Add(decoration);
					}
					result = magicCode2;
				}
			}
			return result;
		}

		private void AddDelayColor(GSprite sprite, int magicCode, long startTicks, int slotSeconds)
		{
			if (sprite == null)
			{
				return;
			}
			if (magicCode < 0)
			{
				return;
			}
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
			if (maigcInfoVOByCode == null)
			{
				return;
			}
			int num = slotSeconds;
			if (num <= 0)
			{
				string magicTime = maigcInfoVOByCode.MagicTime;
				if (string.IsNullOrEmpty(magicTime))
				{
					return;
				}
				string[] array = magicTime.Split(new char[]
				{
					','
				});
				if (array.Length <= 0)
				{
					return;
				}
				SkillData skillDataByID = Global.GetSkillDataByID(magicCode);
				int num2 = (skillDataByID != null) ? (skillDataByID.SkillLevel - 1) : 0;
				num2 = Math.Min(num2, array.Length - 1);
				num = Global.SafeConvertToInt32(array[num2]);
				if (num <= 0)
				{
					return;
				}
			}
			long num3 = startTicks + (long)(num * 1000);
			string magicColor = maigcInfoVOByCode.MagicColor;
			if (magicColor != null && string.Empty != magicColor)
			{
				SolidColorBrush solidColorBrush = this.GetSolidColorBrush(magicColor);
				sprite.MaskBrush = solidColorBrush;
			}
		}

		private void AddHitDecoration(int enemy, int magicCode, Quaternion rotation, GSprite attackerSprite = null)
		{
			if (enemy != -1)
			{
				GSprite gsprite;
				if (enemy != Global.Data.RoleID)
				{
					string name = StringUtil.substitute("Role_{0}", new object[]
					{
						enemy
					});
					gsprite = this.FindSprite(name);
				}
				else
				{
					gsprite = this.Leader;
				}
				if (gsprite == null)
				{
					return;
				}
				if (gsprite != null)
				{
					if (gsprite.SpriteType == GSpriteTypes.Leader && Global.Data.SysSetting.HideOtherRoles)
					{
						return;
					}
					int num = -1;
					int num2 = 3;
					int num3 = -1;
					if (magicCode > 0)
					{
						MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
						num = Global.GetMagicCode(maigcInfoVOByCode, "TargetDecoration", gsprite);
						if (magicCode == 11001)
						{
							num3 = (int)ConfigSystemParam.GetSystemParamIntByName("ZHSFlyDecoration");
						}
						num2 = maigcInfoVOByCode.ToOcuupation;
					}
					int num4 = this.AddDelayDeco(gsprite, magicCode, Global.GetCorrectLocalTime(), 0);
					string name2 = string.Format("BufferDecoration_{0}", Global.GetBufferDecID(101));
					GDecoration gdecoration = gsprite.Root.FindName(name2) as GDecoration;
					if (gdecoration != null)
					{
						string name3 = string.Format("DelayDecoration_{0}", 45);
						GDecoration gdecoration2 = gsprite.Root.FindName(name3) as GDecoration;
						if (gdecoration2 != null)
						{
							gdecoration2.The3DGameObject.SetActive(false);
						}
					}
					bool flag = true;
					if (gsprite.SpriteType == GSpriteTypes.Other)
					{
						int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("NoShowMagicEffectID", ',');
						if (systemParamIntArrayByName.IndexOf(num) >= 0)
						{
							flag = false;
						}
					}
					if (num >= 0 && num4 != num && flag)
					{
						GDecoration decoration = Global.GetDecoration(num, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, num2 <= 3);
						if (Quaternion.identity != rotation && null != gsprite.The3DGameObject)
						{
							rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y - gsprite.The3DGameObject.transform.localEulerAngles.y, rotation.eulerAngles.z);
							decoration.The3DGameObject.transform.localRotation = rotation;
						}
						gsprite.Add(decoration);
					}
					if (attackerSprite != null && num3 >= 0 && num4 != num3)
					{
						GDecoration deco2 = Global.GetDecoration(num3, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
						if (deco2 != null)
						{
							if (deco2.The3DGameObject != null && null != gsprite.The3DGameObject)
							{
								deco2.The3DGameObject.transform.localPosition = gsprite.The3DGameObject.transform.localPosition;
							}
							if (Quaternion.identity != rotation && null != gsprite.The3DGameObject && deco2.The3DGameObject != null)
							{
								rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y - gsprite.The3DGameObject.transform.localEulerAngles.y, rotation.eulerAngles.z);
								deco2.The3DGameObject.transform.localRotation = rotation;
							}
							if (attackerSprite.The3DGameObject != null)
							{
								deco2.tarTrans = attackerSprite.The3DGameObject;
							}
							deco2.DecorationLoadCompleteNotify = delegate(object s1, EventArgs e1)
							{
								if (deco2.The3DGameObject != null)
								{
									EffectMoveManager effectMoveManager = deco2.The3DGameObject.GetComponent<EffectMoveManager>();
									if (effectMoveManager == null)
									{
										effectMoveManager = deco2.The3DGameObject.AddComponent<EffectMoveManager>();
									}
									effectMoveManager.ObjMgr = deco2;
									effectMoveManager.SetTarget(deco2.tarTrans);
								}
							};
						}
					}
				}
			}
		}

		private void AddHitDecoration(int enemyX, int enemyY, int magicCode)
		{
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(magicCode);
			int magicCode2 = Global.GetMagicCode(maigcInfoVOByCode, "TargetDecoration", null);
			int num = this.AddDelayDeco(enemyX, enemyY, magicCode);
			if (magicCode2 >= 0 && num != magicCode2)
			{
				GDecorationTypes decoType = GDecorationTypes.AutoRemove;
				GDecoration decoration = Global.GetDecoration(magicCode2, decoType, new Point(enemyX, enemyY), true, null, -1, -1, true, false);
				this.Add(decoration);
			}
		}

		private void AddMagic(int roldID, int currentMagic)
		{
		}

		public void ShowRoleTalkText(int roleID, string talkText)
		{
			string name = string.Empty;
			if (roleID == Global.Data.roleData.RoleID)
			{
				name = "Leader";
			}
			else
			{
				name = StringUtil.substitute("Role_{0}", new object[]
				{
					roleID
				});
			}
			GSprite gsprite = this.FindSprite(name);
			if (gsprite != null)
			{
				Global.Data.TalkDict[gsprite.RoleID] = Global.GetCorrectLocalTime();
				gsprite.VTalk = talkText;
			}
		}

		public void ClearAllRoleTalkText()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, long> keyValuePair in Global.Data.TalkDict)
			{
				int key = keyValuePair.Key;
				list.Add(key);
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.ShowRoleTalkText(list[i], string.Empty);
			}
		}

		private void UpdateNameColor(GSprite sprite, RoleData roleData)
		{
			if (Global.GetMapSceneUIClass() == SceneUIClasses.PKKing)
			{
				if (Global.Data.roleData.RoleID != roleData.RoleID)
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
					return;
				}
			}
			else
			{
				if (Global.GetMapSceneUIClass() == SceneUIClasses.DaTaoSha)
				{
					if (roleData.BattleWhichSide == Global.Data.roleData.BattleWhichSide)
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
					}
					else
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
					}
					return;
				}
				if (Global.GetMapSceneUIClass() == SceneUIClasses.Battle)
				{
					if (roleData.BattleWhichSide == 1)
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
					}
					else
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
					}
					return;
				}
				if (Global.GetMapSceneUIClass() == SceneUIClasses.LuoLanChengZhan)
				{
					if (Global.Data.roleData.Faction != roleData.Faction)
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
						return;
					}
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu)
				{
					if (Global.CanGuanZhan())
					{
						if (Global.IsInLangHunLingYuScene())
						{
							if (roleData.BattleWhichSide == 1)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
							}
							else
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
							}
							return;
						}
					}
					else if (Global.Data.roleData.Faction != roleData.Faction)
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
						return;
					}
				}
				else
				{
					if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuWangZhe)
					{
						if (roleData.BattleWhichSide == 1)
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
						}
						else
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
						}
						return;
					}
					if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
					{
						if (roleData.BattleWhichSide != Global.Data.roleData.BattleWhichSide)
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
						}
						else
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
						}
						return;
					}
					if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) && Global.GetMapSceneUIClass() != SceneUIClasses.KuaFuBoss && Global.GetMapSceneUIClass() != SceneUIClasses.CopyWolf && Global.GetMapSceneUIClass() != SceneUIClasses.WanMoXiaGu)
					{
						if (roleData.BattleWhichSide == 1)
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 253, 1, 12));
						}
						else
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 66, 142, 176));
						}
						if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
						{
							if (roleData.BattleWhichSide == 1)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
							}
							else
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
							}
							return;
						}
						return;
					}
					else
					{
						if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
						{
							if (roleData.BattleWhichSide == 1)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 0, 0, 255));
							}
							else if (roleData.BattleWhichSide == Global.Data.roleData.BattleWhichSide)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
							}
							else
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 0, 0, 255));
							}
							return;
						}
						if (Global.IsInShiLiZhengBaBattleMap() || Global.IsInShiLiZhengBaMap() || Global.IsCompMiDongMap())
						{
							if (sprite.SpriteType == GSpriteTypes.Other || sprite.SpriteType == GSpriteTypes.Leader)
							{
								if (roleData.CompType == Global.Data.roleData.CompType)
								{
									sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
								}
								else
								{
									sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
								}
								return;
							}
						}
						else if (Global.IsInMoYuDuoBao() && (sprite.SpriteType == GSpriteTypes.Other || sprite.SpriteType == GSpriteTypes.Leader))
						{
							if (roleData.BattleWhichSide == Global.Data.roleData.BattleWhichSide)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
							}
							else
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
							}
							return;
						}
					}
				}
			}
			if (sprite != null && roleData != null)
			{
				int nameColorIndexByPKPoints = Global.GetNameColorIndexByPKPoints(roleData.PKPoint);
				if (nameColorIndexByPKPoints >= 2)
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 204, 52, 10));
				}
				else if (Global.IsPurpleName(roleData))
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 168, 168, 168));
				}
				else if (nameColorIndexByPKPoints == 1)
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 255, 0));
				}
				else
				{
					sprite.SNameBrush = new SolidColorBrush(Global.Data.SnameBrushColor);
				}
			}
			if (sprite.SpriteType == GSpriteTypes.Other || sprite.SpriteType == GSpriteTypes.FakeRole)
			{
				if (Global.IsBattleMap())
				{
					if (Global.Data.roleData.BattleWhichSide == roleData.BattleWhichSide)
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 255, 255));
					}
					else
					{
						sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 204, 52, 10));
					}
				}
				else if (Global.IsInArenaBattleMap())
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 204, 52, 10));
				}
				else if (Global.IsInWangChengOrHuangGong())
				{
					if (this.Leader.PKMode != GPKModes.Team)
					{
						if (Global.Data.roleData.Faction > 0)
						{
							if (roleData.Faction == Global.Data.roleData.Faction)
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
							}
							else
							{
								sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 249, 62, 239));
							}
						}
						else
						{
							sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 249, 62, 239));
						}
					}
				}
				else if (this.Leader.PKMode != GPKModes.Team && Global.Data.roleData.Faction > 0 && roleData.Faction == Global.Data.roleData.Faction)
				{
					sprite.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
				}
			}
			if (sprite.FakeRoleType.ToString() == "DiaoXiang3")
			{
				sprite.SNameBrush = new SolidColorBrush(Global.Data.SnameBrushColor);
			}
		}

		private void UpdatePKValue(RoleData roleData, GSprite sprite)
		{
			sprite.ShowName = Global.FormatShowName(roleData, 0);
		}

		public bool IsOnHorse()
		{
			return this.Leader == null && false;
		}

		public bool InSafeRegion(GSprite sprite)
		{
			if (sprite == null)
			{
				return false;
			}
			if (sprite.cx >= this.CurrentMapData.MapWidth || sprite.cy >= this.CurrentMapData.MapHeight)
			{
				return false;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			return byte.MaxValue == this.CurrentMapData.TerrainWithTeleports[num, num2];
		}

		public bool InSafeRegion()
		{
			return this.InSafeRegion(this.Leader);
		}

		public bool NeedUseMagic()
		{
			if (this.Leader == null)
			{
				return false;
			}
			if (this.HintNotUseAttack)
			{
				return false;
			}
			if (this.Leader.Action != GActions.Attack)
			{
				return false;
			}
			if (this.Leader.Occupation == 0)
			{
				return false;
			}
			if (Global.Data.roleData.Level >= 10)
			{
				return false;
			}
			this.HintNotUseAttack = true;
			return true;
		}

		private void UpdateNaviDeco(GSprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
		}

		private void AddDecoForGoodsPack(GGoodsPack goodsPack, int goodsPackID, int ownerRoleID, Point p, int lucky, int excellenceInfo, int appendPropLev, int forge_Level)
		{
			int code;
			switch (Global.GetZhuoyueAttributeCount(excellenceInfo))
			{
			case 1:
			case 2:
				code = 500;
				break;
			case 3:
			case 4:
				code = 501;
				break;
			case 5:
			case 6:
				code = 502;
				break;
			default:
				code = 165;
				break;
			}
			GDecoration decoration = Global.GetDecoration(code, GDecorationTypes.Loop, p, true, null, -1, -1, true, false);
			this.Add(decoration);
			goodsPack.GoodsPackDeco = decoration;
		}

		private void RemoveDecoForGoodsPack(int goodsPackID)
		{
		}

		public bool IsSittingNow()
		{
			return this.Leader == null || !this.EnableChangMap || this.Leader.Action == GActions.Sit;
		}

		public void SitNow()
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Sit)
			{
				return;
			}
			if (this.IsInStalling())
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
				if (this.EndAutoFightNotifiy != null)
				{
					this.EndAutoFightNotifiy.Invoke(this, EventArgs.Empty);
				}
				return;
			}
			this.HideMouseLeftButtonUpEffect();
			this.Leader.LockObject = null;
			this.InstantSite();
		}

		public bool IsStandLongTime(double elpasedTicks)
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return false;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return false;
			}
			if (this.Leader.Action == GActions.Stand)
			{
				long correctLocalTime = Global.GetCorrectLocalTime();
				if ((double)(correctLocalTime - this.Leader.LastActionTicks) >= elpasedTicks)
				{
					return true;
				}
			}
			return false;
		}

		public void StandNow()
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Stand)
			{
				return;
			}
			this.InstantStand(-1);
		}

		public void DoInteractionAction(string actionName)
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Stand || this.Leader.Action == GActions.IdleStand || this.Leader.Action == GActions.Wenhao || this.Leader.Action == GActions.Genwolai || this.Leader.Action == GActions.Guzhang || this.Leader.Action == GActions.Huanhu || this.Leader.Action == GActions.Jushang || this.Leader.Action == GActions.Xingli || this.Leader.Action == GActions.Chongfeng || this.Leader.Action == GActions.Mobai || this.Leader.Action == GActions.Tiaoxin || this.Leader.Action == GActions.Zuoxia || this.Leader.Action == GActions.Shuijiao)
			{
				GActions action = GActions.Wenhao;
				if (actionName != null)
				{
					if (GScene.<>f__switch$map1 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
						dictionary.Add("wenhao", 0);
						dictionary.Add("guolai", 1);
						dictionary.Add("guzhang", 2);
						dictionary.Add("huanhu", 3);
						dictionary.Add("jushang", 4);
						dictionary.Add("xingli", 5);
						dictionary.Add("chongfeng", 6);
						dictionary.Add("mobai", 7);
						dictionary.Add("tiaoxin", 8);
						dictionary.Add("zuoxia", 9);
						dictionary.Add("shuijiao", 10);
						GScene.<>f__switch$map1 = dictionary;
					}
					int num;
					if (GScene.<>f__switch$map1.TryGetValue(actionName, ref num))
					{
						switch (num)
						{
						case 0:
							action = GActions.Wenhao;
							break;
						case 1:
							action = GActions.Genwolai;
							break;
						case 2:
							action = GActions.Guzhang;
							break;
						case 3:
							action = GActions.Huanhu;
							break;
						case 4:
							action = GActions.Jushang;
							break;
						case 5:
							action = GActions.Xingli;
							break;
						case 6:
							action = GActions.Chongfeng;
							break;
						case 7:
							action = GActions.Mobai;
							break;
						case 8:
							action = GActions.Tiaoxin;
							break;
						case 9:
							action = GActions.Zuoxia;
							break;
						case 10:
							action = GActions.Shuijiao;
							break;
						}
					}
				}
				this.Leader.Action = action;
				GameInstance.Game.SpriteAction((double)this.Leader.Direction, (int)action, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
			}
		}

		public void DoAction(GActions action)
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.Leader.Action = action;
			GameInstance.Game.SpriteAction((double)this.Leader.Direction, (int)action, this.Leader.Coordinate, this.Leader.EnemyTarget, this.Leader.YAngle, new Point(0, 0));
		}

		public void AutoSitManager()
		{
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader == null || !this.EnableChangMap)
			{
				return;
			}
			if (this.Leader.VLife <= 0.0)
			{
				return;
			}
			if (this.Leader.Action == GActions.Sit)
			{
				return;
			}
			if (this.IsInStalling())
			{
				return;
			}
			if (Global.IsAutoFighting())
			{
				return;
			}
			this.HideMouseLeftButtonUpEffect();
			this.Leader.LockObject = null;
			this.InstantSite();
		}

		public bool IsFighting()
		{
			return this.Leader != null && this.Leader.VLife > 0.0 && (this.Leader.Action == GActions.Attack || this.Leader.Action == GActions.PreAttack || this.Leader.Action == GActions.Magic);
		}

		public bool IsPreAttack()
		{
			return this.Leader != null && this.Leader.VLife > 0.0 && this.Leader.Action == GActions.PreAttack;
		}

		public bool IsMoving()
		{
			return this.Leader != null && this.Leader.VLife > 0.0 && Global.FindMoveStroyboard(this.Leader.Name);
		}

		public bool IsDead()
		{
			return this.Leader == null || this.Leader.VLife <= 0.0;
		}

		public void ShowLeaderText(double offsetY, int picTextColor, string text, int numType, int numVal, int moveMode, long moveTicks, uint borderColor = 0U)
		{
			if (this.Leader == null)
			{
				return;
			}
			Global.ShowText(this.Leader, 0.0, offsetY, Colors.Uint2Color((uint)picTextColor), text, numType, (long)numVal, moveMode, (double)moveTicks, 2, HUDTextCustom.TextType.Normal);
		}

		public void ShowLeaderText(string text, long moveTicks)
		{
			if (this.Leader == null)
			{
				return;
			}
			Global.ShowText(this.Leader, text, (float)moveTicks);
		}

		public void StartMapShaking()
		{
			if (this.MapShakingCount > 0)
			{
				return;
			}
			if (Global.Data.SysSetting.AvoidScreenShaking)
			{
				return;
			}
			this.MapShakingCount = 12;
		}

		private int GetMapShakeYOffset()
		{
			if (this.MapShakingCount <= 0)
			{
				return 0;
			}
			if (Global.Data.SysSetting.AvoidScreenShaking)
			{
				return 0;
			}
			this.MapShakingCount--;
			return this.MapSakingOffsets[11 - this.MapShakingCount];
		}

		public string FormatLeaderLockObject()
		{
			if (this.Leader == null)
			{
				return string.Empty;
			}
			if (this.Leader.LockObject == null && Global.WatchSprite == null)
			{
				return string.Empty;
			}
			int num = 0;
			GSprite gsprite = null;
			if (this.Leader.LockObject != null)
			{
				gsprite = this.FindSprite(this.Leader.LockObject);
			}
			if (gsprite == null && Global.WatchSprite != null)
			{
				gsprite = this.FindSprite(Global.WatchSprite);
				if (gsprite == null)
				{
					return string.Empty;
				}
				num = 1;
			}
			if (gsprite == null)
			{
				return string.Empty;
			}
			if (gsprite.SpriteType == GSpriteTypes.NPC)
			{
				return string.Empty;
			}
			string text = (num != 0) ? Global.GetLang("锁定") : Global.GetLang("攻击");
			if (gsprite.SpriteType == GSpriteTypes.Monster && Global.Data.SystemMonsters.ContainsKey(gsprite.RoleID))
			{
				MonsterData value = Global.Data.SystemMonsters.GetValue(gsprite.RoleID);
				if (value != null && value.MonsterType == 1101)
				{
					text = Global.GetLang("采集");
				}
			}
			int num2 = (int)Global.GetDirectionByTan((double)gsprite.Coordinate.X, (double)gsprite.Coordinate.Y, (double)this.Leader.Coordinate.X, (double)this.Leader.Coordinate.Y);
			return StringUtil.substitute("<font color=\"#00ff00\">您正在{0}[</font><font color=\"#ffffff\">{1}</font><font color=\"#00ff00\">生命值{2}</font><font color=\"#ffffff\">{3}</font><font color=\"#00ff00\">]</font>", new object[]
			{
				text,
				gsprite.VSName,
				gsprite.VLife,
				GScene.DirNames[num2]
			});
		}

		private void AddMaBiStatus(GSprite sprite, RoleData roleData)
		{
			if (sprite == null || roleData == null)
			{
				return;
			}
			if (roleData.DongJieStart <= 0L || roleData.DongJieMills <= 0)
			{
				return;
			}
			SolidColorBrush solidColorBrush = this.GetSolidColorBrush("灰色");
			sprite.MaskBrush = solidColorBrush;
		}

		private GSprite JugeSpriteInAngle(GSprite sprite, int radius)
		{
			if (sprite == null)
			{
				return null;
			}
			int num = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, sprite.Coordinate);
			if (num < 200)
			{
				return sprite;
			}
			double loAngle = 0.0;
			double hiAngle = 360.0;
			Global.GetAngleRangeByDirection(this.Leader.Direction, 120.0, out loAngle, out hiAngle);
			if (Global.InCircleByAngle(sprite.Coordinate, this.Leader.Coordinate, (double)radius, loAngle, hiAngle))
			{
				return sprite;
			}
			loAngle = 0.0;
			hiAngle = 360.0;
			Global.GetAngleRangeByDirection(this.Leader.Direction, 240.0, out loAngle, out hiAngle);
			if (Global.InCircleByAngle(sprite.Coordinate, this.Leader.Coordinate, (double)radius, loAngle, hiAngle))
			{
				return sprite;
			}
			return null;
		}

		private GSprite SeekSpriteToLock(bool isNPC, int radius)
		{
			GSprite gsprite = null;
			if (Global.GetMapSceneUIClass() == SceneUIClasses.JingJiChang)
			{
				return this._SeekSpriteToLock(isNPC, 1200, 360.0);
			}
			if (gsprite == null)
			{
				gsprite = this._SeekSpriteToLock(isNPC, radius, 30.0);
				if (gsprite == null)
				{
					gsprite = this._SeekSpriteToLock(isNPC, radius, 90.0);
					if (gsprite == null)
					{
						gsprite = this._SeekSpriteToLock(isNPC, radius, 180.0);
						if (gsprite == null)
						{
							gsprite = this._SeekSpriteToLock(isNPC, radius, 360.0);
						}
					}
				}
			}
			return gsprite;
		}

		private GSprite _SeekSpriteToLock(bool isNPC, int radius, double angleLimit = 0.0)
		{
			if (this.Leader.VLife <= 0.0)
			{
				return null;
			}
			GSprite result = null;
			GSprite gsprite = null;
			GSprite result2 = null;
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MaxValue;
			double loAngle = 0.0;
			double hiAngle = 360.0;
			Global.GetAngleRangeByDirection(this.Leader.Direction, angleLimit, out loAngle, out hiAngle);
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite2 = objectsList[i] as GSprite;
					if (gsprite2.MonsterType != MonsterTypes.CaiJi)
					{
						if (gsprite2.CurrentObjectState)
						{
							if (gsprite2.SpriteType != GSpriteTypes.Leader)
							{
								if (Global.InCircleByAngle(gsprite2.Coordinate, this.Leader.Coordinate, (double)radius, loAngle, hiAngle))
								{
									if (isNPC)
									{
										if (gsprite2.SpriteType == GSpriteTypes.NPC)
										{
											int num4 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, gsprite2.Coordinate);
											if (num4 < num)
											{
												num = num4;
												result = gsprite2;
											}
										}
										else if (gsprite2.SpriteType == GSpriteTypes.FakeRole && (gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang || gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang2 || gsprite2.FakeRoleType == FakeRoleTypes.DiaoXiang3 || this.SelectedSprite.FakeRoleType != FakeRoleTypes.CoupleWishMan || this.SelectedSprite.FakeRoleType != FakeRoleTypes.CoupleWishWife))
										{
											int num5 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, gsprite2.Coordinate);
											if (num5 < num)
											{
												num = num5;
												result = gsprite2;
											}
										}
									}
									else if (Global.IsOpposition(this.Leader, gsprite2, false, this.CurrentMapData.PKMode))
									{
										GSprite gsprite3 = this.FindSprite(gsprite2.Name);
										MonsterData monsterData = null;
										if (!Global.Data.SystemMonsters.TryGetValue(gsprite3.RoleID, ref monsterData) || Global.Data.roleData.RoleID != monsterData.MasterRoleID)
										{
											if (gsprite2.SpriteType == GSpriteTypes.Monster || gsprite2.SpriteType == GSpriteTypes.JunQi)
											{
												int num6 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, gsprite2.Coordinate);
												if (num6 <= radius && num6 < num2)
												{
													num2 = num6;
													gsprite = gsprite2;
												}
											}
											else if (gsprite2.SpriteType == GSpriteTypes.Other || gsprite2.SpriteType == GSpriteTypes.FakeRole)
											{
												int num7 = (int)Global.GetTwoPointDistance(this.Leader.Coordinate, gsprite2.Coordinate);
												if (num7 <= radius && num7 < num3)
												{
													num3 = num7;
													result2 = gsprite2;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (isNPC)
			{
				return result;
			}
			if (gsprite != null)
			{
				return gsprite;
			}
			return result2;
		}

		public void ModifyMonstersSpeed(int extensionID, float speed)
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			for (int i = 0; i < objectsList.Count; i++)
			{
				if (objectsList[i] is GSprite)
				{
					GSprite gsprite = objectsList[i] as GSprite;
					if (gsprite.CurrentObjectState)
					{
						if (gsprite.SpriteType == GSpriteTypes.Monster)
						{
							if (extensionID == gsprite.ExtensionID)
							{
								U3DUtils.ModifyAnimationSpeed(gsprite.The3DGameObject, speed);
							}
						}
					}
				}
			}
		}

		private void UpdateMarketName(GSprite sprite, RoleData roleData)
		{
			sprite.VOtherName = roleData.StallName;
		}

		private void AddBufferDeco(GSprite sprite, int BufferCode, long startTicks, int slotSeconds = 0)
		{
			long num = startTicks + (long)(slotSeconds * 1000);
			if (Global.GetCorrectLocalTime() >= num)
			{
				return;
			}
			string name = string.Format("DelayDecoration_{0}", 45);
			GDecoration gdecoration = sprite.Root.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				gdecoration.The3DGameObject.SetActive(false);
			}
			string name2 = string.Format("BufferDecoration_{0}", BufferCode);
			GDecoration gdecoration2 = sprite.Root.FindName(name2) as GDecoration;
			if (gdecoration2 != null)
			{
				Global.RemoveObject(gdecoration2, true);
			}
			gdecoration2 = (ObjectsManager.FindSprite(name2) as GDecoration);
			if (gdecoration2 != null)
			{
				Global.RemoveObject(gdecoration2, true);
			}
			Point pos = new Point(sprite.CenterX, sprite.CenterY);
			gdecoration2 = Global.GetDecoration(BufferCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
			if (gdecoration2 != null)
			{
				gdecoration2.LifeTicks = num;
				gdecoration2.Name = name2;
				if (gdecoration2.HangPos == 1)
				{
					sprite.Root.Children.Add(gdecoration2);
				}
				else
				{
					gdecoration2.cx = sprite.cx;
					gdecoration2.cy = sprite.cy;
					ObjectsManager.Add(gdecoration2);
				}
			}
		}

		public void AddOrRemoveEmblemDeco(GSprite sprite, byte AddShan = 0)
		{
			if (sprite != null)
			{
				int num = (int)ConfigSystemParam.GetSystemParamIntByName("EmblemProp");
				long num2 = 0L;
				long num3 = 0L;
				if (Global.Data.roleData.RoleID == sprite.RoleID)
				{
					EmblemCoolDownItem emblemItem = Global.GetEmblemItem();
					if (emblemItem != null)
					{
						num2 = emblemItem.ContinuedTicks;
						num3 = emblemItem.StartTicks;
					}
				}
				else
				{
					BufferData emblemBuffData = Global.GetEmblemBuffData(sprite.RoleID);
					if (emblemBuffData != null)
					{
						num2 = (long)emblemBuffData.BufferSecs;
						num3 = emblemBuffData.StartTime;
					}
				}
				if (0L < num3 && 0L < num2)
				{
					BufferData emblemBuffData2 = Global.GetEmblemBuffData(sprite.RoleID);
					string name = string.Format("EmblemDecoration_H_{0}", num);
					string name2 = string.Format("EmblemDecoration_S_{0}", num);
					Dictionary<int, EmblemUp> dicUp = Global.m_DicUp;
					Dictionary<int, EmblemStarXml> dicStar = Global.m_DicStar;
					int num4 = 1;
					if (emblemBuffData2 != null && dicStar.ContainsKey((int)emblemBuffData2.BufferVal))
					{
						num4 = dicStar[(int)emblemBuffData2.BufferVal].EmblemLevel;
					}
					string resName = "TY_HJHT_01.unity3d";
					string resName2 = "TY_HJHT_02.unity3d";
					if (dicUp.ContainsKey(num4))
					{
						string[] array = dicUp[num4].Decorations.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							resName = Global.SetHuiJiTeXiao(array[0].SafeToInt32(0));
							resName2 = Global.SetHuiJiTeXiao(array[1].SafeToInt32(0));
						}
					}
					if (num2 == 0L)
					{
						GDecoration gdecorationEmblem = sprite.GDecorationEmblem;
						if (gdecorationEmblem != null)
						{
							Global.RemoveObject(gdecorationEmblem, true);
							sprite.GDecorationEmblem = null;
						}
					}
					else
					{
						int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EmblemEffect", ',');
						if (systemParamIntArrayByName != null)
						{
							for (int i = 0; i < systemParamIntArrayByName.Length; i++)
							{
								this.RemoveDelayDeco(sprite, systemParamIntArrayByName[i]);
							}
						}
						long num5 = num3 + num2;
						if (Global.GetCorrectLocalTime() >= num5)
						{
							return;
						}
						Point point = new Point(sprite.CenterX, sprite.CenterY);
						GDecoration gdecoration = sprite.GDecorationEmblem;
						if (gdecoration != null)
						{
							Global.RemoveObject(gdecoration, true);
							gdecoration = null;
						}
						if (gdecoration == null)
						{
							gdecoration = (ObjectsManager.FindSprite(name) as GDecoration);
						}
						if (gdecoration == null)
						{
							gdecoration = new GDecoration(resName2);
						}
						gdecoration.OrigCoordinate = new Point(point.X, point.Y);
						gdecoration.cx = point.X;
						gdecoration.cy = point.Y;
						gdecoration.DecorationType = GDecorationTypes.Loop;
						gdecoration.IsCache = false;
						gdecoration.OwnerName = null;
						gdecoration.TriggerType = -1;
						gdecoration.Layer = -1;
						gdecoration.HangPos = 1;
						gdecoration.SoundFileName = string.Empty;
						gdecoration.ForceSyncLoad = true;
						gdecoration.Start();
						if (gdecoration != null)
						{
							gdecoration.LifeTicks = num5;
							gdecoration.Name = name;
							if (gdecoration.HangPos == 1)
							{
								sprite.GDecorationEmblem = gdecoration;
							}
						}
						if (AddShan == 1)
						{
							GDecoration gdecoration2 = sprite.GDecorationEmblemShan;
							if (gdecoration2 != null)
							{
								Global.RemoveObject(gdecoration2, true);
								gdecoration2 = null;
							}
							if (gdecoration2 == null)
							{
								gdecoration2 = (ObjectsManager.FindSprite(name2) as GDecoration);
							}
							if (gdecoration2 == null)
							{
								gdecoration2 = new GDecoration(resName);
							}
							gdecoration2.OrigCoordinate = new Point(point.X, point.Y);
							gdecoration2.cx = point.X;
							gdecoration2.cy = point.Y;
							gdecoration2.DecorationType = GDecorationTypes.Loop;
							gdecoration2.IsCache = false;
							gdecoration2.OwnerName = null;
							gdecoration2.TriggerType = -1;
							gdecoration2.Layer = -1;
							gdecoration2.HangPos = 1;
							gdecoration2.SoundFileName = string.Empty;
							gdecoration2.ForceSyncLoad = true;
							gdecoration2.Start();
							if (gdecoration2 != null)
							{
								gdecoration2.LifeTicks = num3 + 1180L;
								gdecoration2.Name = name;
								if (gdecoration2.HangPos == 1)
								{
									sprite.GDecorationEmblemShan = gdecoration2;
								}
							}
						}
					}
				}
				else
				{
					GDecoration gdecorationEmblem2 = sprite.GDecorationEmblem;
					if (gdecorationEmblem2 != null)
					{
						Global.RemoveObject(gdecorationEmblem2, true);
					}
				}
			}
		}

		private void EmblemClearRoleInfluence(GSprite sprite)
		{
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				Global.Data.roleData.DongJieStart = 0L;
				Global.Data.roleData.DongJieSeconds = 0;
			}
			else if (Global.Data.OtherRoles.ContainsKey(sprite.RoleID))
			{
				Global.Data.OtherRoles[sprite.RoleID].DongJieStart = 0L;
				Global.Data.OtherRoles[sprite.RoleID].DongJieSeconds = 0;
			}
		}

		public void RoleHorseStateChange(byte ride, int RoleID)
		{
			GSprite gsprite;
			if (RoleID == Global.Data.RoleID)
			{
				gsprite = this.Leader;
			}
			else
			{
				string name = StringUtil.substitute("Role_{0}", new object[]
				{
					RoleID
				});
				gsprite = this.FindSprite(name);
			}
			if (gsprite != null && (gsprite.SpriteType == GSpriteTypes.Leader || gsprite.SpriteType == GSpriteTypes.Other))
			{
				if (ride == 0)
				{
					gsprite.RoleUnLoadMount(0);
				}
				else
				{
					gsprite.RoleLoadMount();
				}
			}
		}

		public GSprite LoadPet(PetRoleItem petRoleItem)
		{
			string name = StringUtil.substitute("Role_{0}", new object[]
			{
				petRoleItem.AutoRoleID
			});
			GSprite gsprite = new GSprite();
			gsprite.SpriteType = GSpriteTypes.Pet;
			gsprite.SpriteChangeAction += delegate(object s, SpriteChangeActionEventArgs e)
			{
				Point coordinate = (s as GSprite).Coordinate;
				GameInstance.Game.SpriteAction2((s as GSprite).RoleID, (double)(s as GSprite).Direction, e.Action, coordinate, (s as GSprite).EnemyTarget, 4);
			};
			gsprite.CoordinateChanged += delegate(GSprite sender)
			{
				this.UpdatePetEvent(sender);
			};
			this.LoadSprite(gsprite, petRoleItem.AutoRoleID, 0, name, string.Empty, string.Empty, petRoleItem.PetName, 0, -1, Global.Data.FactionBrushColor, Global.Data.OtherNameBrushColor, Global.Data.SnameBrushColor, 1.0, 0, 0, -1.0, petRoleItem.BodyCode, -1, new Point((int)petRoleItem.X, (int)petRoleItem.Y), 0, (double)Global.Data.LifeTotalWidth, 1.0, 0, true);
			GDecoration decoration = Global.GetDecoration(1000, GDecorationTypes.Loop, new Point(gsprite.CenterX - gsprite.ShadowOffset.X, gsprite.CenterY - gsprite.ShadowOffset.Y), false, null, -1, -1, true, false);
			Canvas.SetZIndex(decoration, -100.0);
			gsprite.Children.Add(decoration);
			if (!Global.Data.SysSetting.HideOtherRoles)
			{
				gsprite.Start();
			}
			else
			{
				gsprite.HideObject();
			}
			return gsprite;
		}

		private void UpdatePetEvent(GSprite sprite)
		{
			if (!this.EnableChangMap)
			{
				return;
			}
			if (this.CurrentMapData.GridSizeX == 0 || this.CurrentMapData.GridSizeY == 0)
			{
				return;
			}
			int num = sprite.cx / this.CurrentMapData.GridSizeX;
			int num2 = sprite.cy / this.CurrentMapData.GridSizeY;
			if (num != sprite.OldGridX || num2 != sprite.OldGridY)
			{
				Global.CurrentMapData._MapGrid.MoveObjectEx(sprite.OldGridX, sprite.OldGridY, num, num2, sprite);
				sprite.OldGridX = num;
				sprite.OldGridY = num2;
			}
		}

		private void ProcessPetTrackingLeader()
		{
			if (this.Leader == null || this.Leader.Pet == null)
			{
				return;
			}
			GSprite pet = this.Leader.Pet;
			if (Global.InCircle(this.Leader.Coordinate, pet.Coordinate, 210.0))
			{
				if (pet.Action != GActions.Stand || Global.GetCorrectLocalTime() - pet.LastActionTicks >= 5000L)
				{
				}
				return;
			}
			Point astepPoint;
			if (!Global.InCircle(this.Leader.Coordinate, pet.Coordinate, 600.0))
			{
				astepPoint = this.GetAStepPoint(this.Leader.Coordinate, pet.Coordinate, 200);
				int toDirection = (int)Global.GetDirectionByTan((double)this.Leader.Coordinate.X, (double)this.Leader.Coordinate.Y, (double)pet.Coordinate.X, (double)pet.Coordinate.Y);
				GameInstance.Game.SpriteChangePos(pet.RoleID, Global.Data.roleData.MapCode, astepPoint.X, astepPoint.Y, toDirection);
				return;
			}
			astepPoint = this.GetAStepPoint(this.Leader.Coordinate, pet.Coordinate, 200);
			if (astepPoint.X != pet.Coordinate.X || astepPoint.Y != pet.Coordinate.Y)
			{
				GameInstance.Game.SpriteMoveTo2(pet.RoleID, pet.Coordinate, astepPoint, 2, 0, 4, pet.PathString);
			}
		}

		private Point GetAStepPoint(Point pt1, Point pt2, int stepLength)
		{
			Point point;
			if (pt1 != pt2)
			{
				point = Global.GetExtensionPoint(pt1, pt2, stepLength);
				if (Global.OnObstruction(point, this.CurrentMapData))
				{
					point = Global.GetAPointIn4Direction(pt1, stepLength, this.CurrentMapData.fixedObstruction, this.CurrentMapData.MapWidth, this.CurrentMapData.MapHeight, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY);
				}
			}
			else
			{
				point = new Point(pt1.X - stepLength, pt1.Y - stepLength);
				if (Global.OnObstruction(point, this.CurrentMapData))
				{
					point = Global.GetAPointIn4Direction(pt1, stepLength, this.CurrentMapData.fixedObstruction, this.CurrentMapData.MapWidth, this.CurrentMapData.MapHeight, this.CurrentMapData.GridSizeX, this.CurrentMapData.GridSizeY);
				}
			}
			point.X = ((point.X >= 0) ? point.X : 0);
			point.X = ((point.X <= this.CurrentMapData.MapWidth) ? point.X : this.CurrentMapData.MapWidth);
			point.Y = ((point.Y >= 0) ? point.Y : 0);
			point.Y = ((point.Y <= this.CurrentMapData.MapHeight) ? point.Y : this.CurrentMapData.MapHeight);
			GMapData currentMapData = this.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return point;
			}
			int num = point.X / currentMapData.GridSizeX;
			int num2 = point.Y / currentMapData.GridSizeY;
			point = new Point(num * currentMapData.GridSizeX + currentMapData.GridSizeX / 2, num2 * currentMapData.GridSizeY + currentMapData.GridSizeY / 2);
			return point;
		}

		private void UpdateTeamFlags(GSprite sprite, bool flagsVisible, bool isTeamLeader)
		{
			if (sprite == null)
			{
				return;
			}
			sprite.FlagsVisible = flagsVisible;
			if (flagsVisible)
			{
				if (isTeamLeader)
				{
					sprite.FlagsType = 1;
				}
				else
				{
					sprite.FlagsType = 2;
				}
			}
			else
			{
				sprite.FlagsType = 0;
			}
		}

		public void UpdateSkillWord(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateLeaderSkillWord()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.UpdateSkillWord(this.Leader, Global.Data.roleData);
		}

		public void UpdateJingMaiWord(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateLeaderJingMaiWord()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.UpdateJingMaiWord(this.Leader, Global.Data.roleData);
		}

		public void UpdateBattleNameImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			if (roleData.BattleNameIndex <= 0)
			{
				sprite.BattleNameVisible = false;
				sprite.BattleNameImageURL = null;
				return;
			}
			string text = StringUtil.substitute("NetImages/GameRes/Images/BattleNames/{0}.png", new object[]
			{
				roleData.BattleNameIndex
			});
			if (text != null)
			{
				sprite.BattleNameVisible = true;
				sprite.BattleNameImageURL = text;
			}
			else
			{
				sprite.BattleNameVisible = false;
				sprite.BattleNameImageURL = null;
			}
		}

		public void UpdateHeroIndexImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateBangHuiInfo(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			sprite.VFaction = Global.GetRoleBHName(roleData);
		}

		public void UpdatePaiHangImages(GSprite sprite, RoleData roleData)
		{
			if (!Global.CanShowRoleHeadPic(true))
			{
				return;
			}
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateHuangChengImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateLingDiWord(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
		}

		public void UpdateLittleVIP(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			if (roleData.IsVIP > 0)
			{
				sprite.LittleVIPVisible = true;
			}
			else
			{
				sprite.LittleVIPVisible = false;
			}
			int num = 0;
			if (roleData.IsVIP == 3)
			{
				num = 1;
			}
			else if (roleData.IsVIP == 6)
			{
				num = 2;
			}
			string littleVIPImageURL = StringUtil.substitute("NetImages/GameRes/Images/LittleVIP/{0}.png", new object[]
			{
				num
			});
			sprite.LittleVIPImageURL = littleVIPImageURL;
		}

		public void UpdateLeaderLittleVIP()
		{
			if (this.Leader == null || this.Leader.VLife <= 0.0)
			{
				return;
			}
			this.UpdateLittleVIP(this.Leader, Global.Data.roleData);
		}

		public void UpdateJieriWord(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			sprite.JieriWordVisible = false;
			sprite.JieriWordImageURL = null;
			int jieriChengHao = roleData.JieriChengHao;
			if (jieriChengHao <= 0)
			{
				return;
			}
			string text = StringUtil.substitute("NetImages/GameRes/Images/JieRiWords/{0}.png", new object[]
			{
				0
			});
			if (text != null)
			{
				sprite.JieriWordVisible = true;
				sprite.JieriWordImageURL = text;
			}
		}

		public void UpdateLeaderChengJiuImage()
		{
			this.UpdateChengJiuImage(this.Leader, Global.Data.roleData);
		}

		public void UpdateChengJiuImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			int num;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				num = Global.GetChengJiuLevel(0);
			}
			else
			{
				num = Global.GetRoleCommonUseParamsValueForOtherRole(sprite.RoleID, RoleCommonUseIntParamsIndexs.ChengJiuLevel);
			}
			if (num <= 0)
			{
				sprite.ChengjiuFlag = string.Empty;
				return;
			}
			string text = StringUtil.substitute("chengjiu{0}", new object[]
			{
				num
			});
			if (text != null)
			{
				sprite.ChengjiuFlag = text;
			}
		}

		public void UpdateLeaderChuanQiJingMaiImage()
		{
			this.UpdateChuanQiJingMaiImage(this.Leader, Global.Data.roleData);
		}

		public void UpdateChuanQiJingMaiImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			int num;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.JingMaiLevel);
			}
			else
			{
				num = Global.GetRoleCommonUseParamsValueForOtherRole(sprite.RoleID, RoleCommonUseIntParamsIndexs.JingMaiLevel);
			}
			if (num <= 0)
			{
				sprite.ChuanQiJingMaiVisible = false;
				sprite.ChuanQiJingMaiImageURL = null;
				return;
			}
			string text = StringUtil.substitute("NetImages/GameRes/Images/JingMaiWords/{0}.png", new object[]
			{
				num
			});
			if (text != null)
			{
				sprite.ChuanQiJingMaiVisible = true;
				sprite.ChuanQiJingMaiImageURL = text;
			}
			else
			{
				sprite.ChuanQiJingMaiVisible = false;
				sprite.ChuanQiJingMaiImageURL = null;
			}
		}

		public void UpdateLeaderChuanWuXueImage()
		{
			this.UpdateChuanWuXueImage(this.Leader, Global.Data.roleData);
		}

		public void UpdateChuanWuXueImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			int num;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.WuXueLevel);
			}
			else
			{
				num = Global.GetRoleCommonUseParamsValueForOtherRole(sprite.RoleID, RoleCommonUseIntParamsIndexs.WuXueLevel);
			}
			if (num <= 0)
			{
				sprite.ChanQiWuXueVisible = false;
				sprite.ChanQiWuXueImageURL = null;
				return;
			}
			string text = StringUtil.substitute("NetImages/GameRes/Images/WuXueWords/{0}.png", new object[]
			{
				num
			});
			if (text != null)
			{
				sprite.ChanQiWuXueVisible = true;
				sprite.ChanQiWuXueImageURL = text;
			}
			else
			{
				sprite.ChanQiWuXueVisible = false;
				sprite.ChanQiWuXueImageURL = null;
			}
		}

		public void UpdateWangChengImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			if (roleData.Faction <= 0)
			{
				sprite.HuangChengNameVisible = false;
				sprite.HuangChengNameImageURL = null;
				return;
			}
			BangHuiLingDiItemData bhidbyLingDiID = Global.GetBHIDByLingDiID(6);
			if (bhidbyLingDiID == null || bhidbyLingDiID.BHID <= 0 || bhidbyLingDiID.BHID != roleData.Faction)
			{
				sprite.HuangChengNameVisible = false;
				sprite.HuangChengNameImageURL = null;
				return;
			}
			string text = StringUtil.substitute("NetImages/GameRes/Images/HuangChengNames/10.png", new object[0]);
			if (text != null)
			{
				sprite.HuangChengNameVisible = true;
				sprite.HuangChengNameImageURL = text;
			}
			else
			{
				sprite.HuangChengNameVisible = false;
				sprite.HuangChengNameImageURL = null;
			}
		}

		public void UpdateLeaderJunxianImage()
		{
			this.UpdateJunxianImage(this.Leader, Global.Data.roleData);
		}

		public void UpdateJunxianImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			int num;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
			}
			else
			{
				num = Global.GetRoleCommonUseParamsValueForOtherRole(sprite.RoleID, RoleCommonUseIntParamsIndexs.ShengWangLevel);
			}
			if (num <= 0)
			{
				sprite.JunxianFlag = string.Empty;
				return;
			}
			string text = StringUtil.substitute("junxian{0}", new object[]
			{
				num
			});
			if (text != null)
			{
				sprite.JunxianFlag = text;
			}
		}

		public void UpdateTitleImage()
		{
			this.UpdateTitleImage(this.Leader, Global.Data.roleData);
		}

		public void UpdateTitleImage(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			int num = Global.Data.roleData.RoleCommonUseIntPamams[30];
			if (num <= 0)
			{
				sprite.TitleName = null;
			}
			if (num > 0)
			{
				string text = "NetImages/GameRes/Images/ChengHao/";
				string text2 = ".png";
				int fashionGoodsID = Global.GetFashionGoodsID(num);
				string titleName = string.Concat(new object[]
				{
					text,
					"title_",
					fashionGoodsID,
					text2
				});
				sprite.TitleName = titleName;
				sprite.LuolanChengZhanFlag = "none";
				sprite.ZhongShenZhiShenFlag = "none";
			}
			string luolanChengZhanFlag = string.Empty;
			string zhongShenZhiShenFlag = string.Empty;
			RoleData roleData2;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				roleData2 = Global.Data.roleData;
			}
			else
			{
				roleData2 = Global.Data.OtherRoles.GetValue(sprite.RoleID);
			}
			if (roleData2 != null && Global.IsBufferExist(111, roleData2))
			{
				zhongShenZhiShenFlag = "zhongshenZhiShen";
				sprite.ZhongShenZhiShenFlag = zhongShenZhiShenFlag;
			}
			if (roleData2 != null && Global.IsBufferExist(103, roleData2))
			{
				if (Global.IsBangHuiLeader(roleData2, roleData2.Faction))
				{
					luolanChengZhanFlag = "langhunChengzhu";
				}
				else
				{
					luolanChengZhanFlag = "langhunGuizu";
				}
				sprite.LuolanChengZhanFlag = luolanChengZhanFlag;
			}
			else if (Global.Data.roleData != null)
			{
				if (Global.Data.roleData.BangHuiLingDiItemsDict != null && Global.Data.roleData.BangHuiLingDiItemsDict[7] != null)
				{
					BangHuiLingDiItemData bangHuiLingDiItemData = Global.Data.roleData.BangHuiLingDiItemsDict[7];
					if (roleData2 != null && bangHuiLingDiItemData.BHID == roleData2.Faction && roleData2.Faction > 0)
					{
						if (Global.IsBangHuiLeader(roleData2, roleData2.Faction))
						{
							luolanChengZhanFlag = "luolanChengzhu";
						}
						else
						{
							luolanChengZhanFlag = "luolanGuizhu";
						}
					}
					sprite.LuolanChengZhanFlag = luolanChengZhanFlag;
				}
				else
				{
					sprite.LuolanChengZhanFlag = "none";
				}
			}
		}

		public void UpdateLuolanImg(GSprite sprite, RoleData roleData)
		{
			if (sprite == null)
			{
				return;
			}
			string luolanChengZhanFlag = string.Empty;
			RoleData roleData2;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				roleData2 = Global.Data.roleData;
			}
			else
			{
				roleData2 = Global.Data.OtherRoles.GetValue(sprite.RoleID);
			}
			if (roleData2 != null && Global.IsBufferExist(103, roleData2))
			{
				if (Global.IsBangHuiLeader(roleData2, roleData2.Faction))
				{
					luolanChengZhanFlag = "langhunChengzhu";
				}
				else
				{
					luolanChengZhanFlag = "langhunGuizu";
				}
				sprite.LuolanChengZhanFlag = luolanChengZhanFlag;
			}
			else if (roleData != null && roleData.BangHuiLingDiItemsDict != null && roleData.BangHuiLingDiItemsDict[7] != null)
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = roleData.BangHuiLingDiItemsDict[7];
				if (bangHuiLingDiItemData.BHID == roleData.Faction)
				{
					if (Global.IsBangHuiLeader(roleData, roleData.Faction))
					{
						sprite.LuolanChengZhanFlag = "luolanChengzhu";
					}
					else
					{
						sprite.LuolanChengZhanFlag = "luolanGuizu";
					}
				}
			}
		}

		public IObject FindName(string name)
		{
			return ObjectsManager.FindSprite(name);
		}

		public GSprite FindSprite(string name)
		{
			return this.FindName(name) as GSprite;
		}

		private void AddSprite(GSprite sprite)
		{
			this.Add(sprite);
		}

		private void Add(IObject obj)
		{
			ObjectsManager.Add(obj);
		}

		public static void Remove(IObject obj)
		{
			ObjectsManager.Remove(obj);
		}

		private void LoadSprite(GSprite sprite, int roleID, int roleSex, string name, string faction, string otherName, string sname, int occupation, int extensionID, uint factionColor, uint otherNameColor, uint snameColor, double life, int pkMode, int pkValue, double currentMagic, int equipmentBody, int equipmentWeapon, Point coordinate, int direction, double lifeTotalWidth, double moveSpeed, int factionID, bool addToCanvas)
		{
			sprite.Name = name;
			sprite.RoleID = roleID;
			sprite.RoleSex = roleSex;
			sprite.FactionID = factionID;
			if (!string.IsNullOrEmpty(faction))
			{
				sprite.VFaction = faction;
			}
			if (!string.IsNullOrEmpty(otherName))
			{
				sprite.VOtherName = otherName;
			}
			sprite.VSName = sname;
			sprite.ShowName = Global.FormatShowName(Global.Data.roleData, 0);
			sprite.Occupation = occupation;
			sprite.SNameBrush = new SolidColorBrush(snameColor);
			sprite.VLife = life;
			sprite.PKMode = (GPKModes)pkMode;
			sprite.CurrentMagic = (int)currentMagic;
			sprite.EquipmentBody = equipmentBody;
			sprite.EquipmentWeapon = equipmentWeapon;
			sprite.cx = coordinate.X;
			sprite.cy = coordinate.Y;
			sprite.OrigCoordinate = coordinate;
			sprite.Direction = direction;
			sprite.LifeTotalWidth = lifeTotalWidth;
			sprite.SpriteMoveSpeed = moveSpeed;
			sprite.ExtensionID = extensionID;
			if (addToCanvas)
			{
				this.Add(sprite);
			}
			sprite.Action = GActions.Stand;
		}

		private GSprite AddCachingSpriteToCanvas(int roleID, int x, int y)
		{
			if (!this.LoadingSpriteCacheDict.ContainsKey(roleID))
			{
				return null;
			}
			GSprite gsprite = this.LoadingSpriteCacheDict[roleID];
			this.LoadingSpriteCacheDict.Remove(roleID);
			gsprite.Coordinate = new Point(x, y);
			this.AddSprite(gsprite);
			if (gsprite.SpriteType == GSpriteTypes.Other)
			{
				if (!Global.Data.SysSetting.HideOtherRoles)
				{
					gsprite.Start();
					gsprite.ShowObject();
				}
				else
				{
					gsprite.Start();
					gsprite.HideObject();
				}
			}
			else
			{
				gsprite.Start();
				gsprite.ShowObject();
			}
			return gsprite;
		}

		private bool RemoveCachingSprite(int roleID)
		{
			if (!this.LoadingSpriteCacheDict.ContainsKey(roleID))
			{
				return false;
			}
			GSprite obj = this.LoadingSpriteCacheDict[roleID];
			this.LoadingSpriteCacheDict.Remove(roleID);
			Global.RemoveObject(obj, false);
			return true;
		}

		private int MapCode { get; set; }

		private int MapPicCode { get; set; }

		public Point SectionCenter
		{
			get
			{
				return this._SectionCenter;
			}
			set
			{
				if (!this._SectionCenter.Equals(value))
				{
				}
			}
		}

		private int ExternalWaitingSkillID = -1;

		private bool DoAttackOK;

		private bool FirstJugeSafeRegion = true;

		private Point LeaderCenterPoint = new Point(-1, -1);

		private int AutoFightTargetMonsterID = -1;

		private int AttackingMeMonsterID = -1;

		private bool NoHintNoDrugGoBack;

		private long LastFindMonsterCmdTicks;

		private int CurrentMapMonsterNum = int.MaxValue;

		private int addLifeDrugId;

		private int addMagicDrugId;

		private int saleType;

		private int drugCount = 99;

		private int CurrentFintMonsterSlot = 1;

		private int[] DoubleExpGoodsIDs;

		private int[] LifeReserveGoodsIDs;

		private int[] MagicReserverGoodsIDs;

		private int ExternalQueueSkillID = -1;

		private int HasHintMagicNotEnoughSkillID = -1;

		private List<int> skillIDIndexs = new List<int>();

		private long LastAutoUseSkillManagerTicks = 100L;

		private long LastAutoUseSkillFSTicks;

		private long LastAutoUseSkillDSTicks;

		private int[] AutoDrinkAddLifeVGoodsIDs;

		private int[] AutoDrinkAddLifeVGoodsLevels;

		private int[] AutoDrinkAddMagicVGoodsIDs;

		private int[] AutoDrinkAddMagicVGoodsLevels;

		private int[] GuaJiGoAwayGoodsIDs;

		private Point LeaderReportPos = new Point(0, 0);

		private Point LeaderBiaoCheReportPos = new Point(0, 0);

		private int LeaderReportTicks = Global.GetMyTimer();

		private int Count = -1;

		private float LastProcessTicks;

		private long LastDateTimeTicks;

		private int TimeSyncCounter;

		private long LastUpdateUITicks;

		private double LastUpdateUILeaderX = -1.0;

		private long LastUpdateRadarTicks;

		private long LastMoveMiniMapTicks;

		private List<IObject> toDeleteObj = new List<IObject>();

		private Vector3 YinJiEffectRootZuoQiPosition = new Vector3(0.12f, 1.83f, 0.12f);

		public GGoodsPack ToOpenGoodsPack;

		private int _PressJoystickCount;

		private int _LastDir = -1;

		private Vector2 LastJoyPosition = Vector2.zero;

		private string[] UILayers = new string[]
		{
			"MUUI",
			"UI",
			"GUI",
			"DecorationUI"
		};

		private int AutoSelectSkillID = -1;

		private long BeforeSkillTime;

		protected int LastEventTicks;

		protected int LastTeleportKey = -1;

		private long LastClientHearTicks;

		private List<BurstMonsterItem> MonsterItemList = new List<BurstMonsterItem>();

		private PathFinderFast pathFinderFast;

		private List<OtherRoleItem> RoleItemList = new List<OtherRoleItem>();

		private GSprite SelectedSprite;

		private GGoodsPack SelectedGoodsPack;

		protected int SelectedSpriteTicks;

		private GSprite LastSelectedSprite;

		private int MuchClickNum;

		private GDecoration LeftMouseClickDeco;

		private bool HintNotUseAttack;

		private int MapShakingCount;

		private int[] MapSakingOffsets = new int[]
		{
			-25,
			50,
			-48,
			36,
			-27,
			18,
			-13,
			8,
			-5,
			2,
			-1,
			0
		};

		private static string[] DirNames = new string[]
		{
			Global.GetLang("正前↑"),
			Global.GetLang("右上↗"),
			Global.GetLang("正右→"),
			Global.GetLang("右下↘"),
			Global.GetLang("正后↓"),
			Global.GetLang("左下↙"),
			Global.GetLang("正左←"),
			Global.GetLang("左上↖")
		};

		public MapConversionEventHandler MapConversion;

		public SpriteNotifyEventHandler LeftButtonClickOnSprite;

		public SpriteNotifyEventHandler SpriteDeadNotify;

		public GoodsPackNotifyEventHandler GoodsPackNotify;

		public EventHandler BackgroundHeartNotfiy;

		public SpriteNotifyEventHandler SpriteStallNotify;

		public EventHandler EndAutoFightNotifiy;

		public SpriteNotifyEventHandler KillSpriteNotify;

		public EventHandler AutoFightingNoDrugNotfiy;

		public SafeRegionNotfiyEventHandler SafeRegionNotfiy;

		public StateNotifyEventHandler AutoFindRoadDecoNotify;

		public StateNotifyEventHandler AutoFightDecoNotify;

		public NotifyYuZhouJiaSuEventHandler NotifyYuZhouJiaSuCheat;

		public DisableInputEvent m_DisableInputEvent;

		private bool EnableChangMap;

		public GMapData CurrentMapData;

		private Point _SectionCenter;

		private bool IsInSafeRegion;

		private double LeftButtonClickTicks;

		private Point CurDestinationPos = new Point(0, 0);

		private bool LeftButtonDown;

		private int LeftButtonDownMoveCount;

		private double LastShiftTicks;

		private Point PreAttackWaitingMovePos = new Point(-1, -1);

		private Point ShiftAttackWaitingPos = new Point(-1, -1);

		private int SkillAttackWaitingID;

		private bool ShiftIsPressed;

		private GSprite Leader;

		private double Direction;

		private int CallMonsterID;

		private double LastCallMonsterTicks;

		protected DispatcherTimer auxiliaryTimer;

		protected DispatcherTimer LeaderMovingTimer;

		protected DispatcherTimer PingTimer;

		protected DispatcherTimer clientHeartTimer;

		protected DispatcherTimer TraceEnemyTimer;

		private Dictionary<int, GSprite> LoadingSpriteCacheDict = new Dictionary<int, GSprite>();
	}
}
