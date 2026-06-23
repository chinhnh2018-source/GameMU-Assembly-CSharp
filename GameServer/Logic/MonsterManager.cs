using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.RefreshIconState;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class MonsterManager
	{
		public void initialize(IEnumerable<XElement> mapItems)
		{
			this.MyMonsterContainer.initialize(mapItems);
		}

		public static bool CanMonsterSeekRange(Monster monster)
		{
			return monster.MonsterType != 101 || monster.MonsterInfo.VLevel > MonsterManager.MinSeekRangeMonsterLevel;
		}

		public void AddMonster(Monster monster)
		{
			this.MyMonsterContainer.AddObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
		}

		public void RemoveMonster(Monster monster)
		{
			this.MyMonsterContainer.RemoveObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
		}

		public int GetTotalMonstersCount()
		{
			return this.MyMonsterContainer.ObjectList.Count;
		}

		public List<object> GetObjectsByMap(int mapCode)
		{
			return this.MyMonsterContainer.GetObjectsByMap(mapCode, -1);
		}

		public int GetMapMonstersCount(int mapCode)
		{
			return this.MyMonsterContainer.GetObjectsCountByMap(mapCode);
		}

		public List<object> GetCopyMapIDMonsterList(int copyMapID)
		{
			return this.MyMonsterContainer.GetObjectsByCopyMapID(copyMapID);
		}

		public int GetCopyMapIDMonstersCount(int copyMapID, int aliveType = -1)
		{
			return this.MyMonsterContainer.GetObjectsCountByCopyMapID(copyMapID, aliveType);
		}

		public bool IsAnyMonsterAliveByCopyMapID(int copyMapID)
		{
			return this.MyMonsterContainer.IsAnyMonsterAliveByCopyMapID(copyMapID);
		}

		public Monster FindMonster(int mapCode, int roleID)
		{
			object obj = this.MyMonsterContainer.FindObject(roleID, mapCode);
			return obj as Monster;
		}

		public List<Monster> FindMonsterAll(int mapCode)
		{
			List<object> list = this.MyMonsterContainer.FindObjectAll(mapCode);
			List<Monster> list2 = new List<Monster>();
			foreach (object obj in list)
			{
				if (obj is Monster)
				{
					list2.Add(obj as Monster);
				}
			}
			return list2;
		}

		public List<object> FindMonsterByExtensionID(int copyMapID, int extensionID)
		{
			return this.MyMonsterContainer.FindObjectsByExtensionID(extensionID, copyMapID);
		}

		public void LookupEnemiesInCircle(int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is Monster)
					{
						if (copyMapID == (list[i] as Monster).CopyMapID)
						{
							if (Global.InCircle((list[i] as Monster).SafeCoordinate, center, (double)radius))
							{
								enemiesList.Add(list[i]);
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> list = new List<object>();
			this.LookupEnemiesInCircleByAngle(direction, mapCode, copyMapID, toX, toY, radius, list, angle, near180);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as Monster).RoleID);
			}
		}

		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double num = 0.0;
				double num2 = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out num, out num2);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is Monster)
					{
						if (copyMapID == (list[i] as Monster).CopyMapID)
						{
							if (Global.InCircleByAngle((list[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(list[i]);
							}
							else if (Global.InCircle((list[i] as Monster).SafeCoordinate, center, 100.0))
							{
								enemiesList.Add(list[i]);
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircleByRoleAngle(int centerAngle, int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeAngle((double)centerAngle, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is Monster)
					{
						if (copyMapID == (list[i] as Monster).CopyMapID)
						{
							if (Global.InCircleByAngle((list[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(list[i]);
							}
							else if (Global.InCircle((list[i] as Monster).SafeCoordinate, center, 100.0))
							{
								enemiesList.Add(list[i]);
							}
						}
					}
				}
			}
		}

		public void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(client.ClientData.PosX, client.ClientData.PosY, radius);
			if (null != list)
			{
				Point center = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				Point apointInCircle = Global.GetAPointInCircle(center, radius, client.ClientData.RoleYAngle);
				int num = (int)apointInCircle.X;
				int num2 = (int)apointInCircle.Y;
				Point center2 = default(Point);
				center2.X = (double)((client.ClientData.PosX + num) / 2);
				center2.Y = (double)((client.ClientData.PosY + num2) / 2);
				int directionX = num - client.ClientData.PosX;
				int directionY = num2 - client.ClientData.PosY;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is Monster)
					{
						if ((list[i] as Monster).VLife > 0.0)
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as Monster).CopyMapID)
							{
								Point target = new Point((list[i] as Monster).CurrentPos.X, (list[i] as Monster).CurrentPos.Y);
								if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
								{
									rolesList.Add(list[i]);
								}
								else if (Global.InCircle(target, center, 100.0))
								{
									rolesList.Add(list[i]);
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];
			List<object> list = mapGrid.FindObjects(gridX, gridY);
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is Monster)
					{
						if (attacker.CurrentCopyMapID == (list[i] as Monster).CopyMapID)
						{
							enemiesList.Add(list[i]);
						}
					}
				}
			}
		}

		public void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];
			Point currentGrid = attacker.CurrentGrid;
			int gridX = (int)currentGrid.X;
			int gridY = (int)currentGrid.Y;
			Point gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY);
			this.LookupEnemiesAtGridXY(attacker, (int)gridPointByDirection.X, (int)gridPointByDirection.Y, enemiesList);
		}

		public void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			this.LookupAttackEnemies(attacker, direction, list);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as Monster).RoleID);
			}
		}

		public void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridPointByDirection.Count > 0)
			{
				for (int i = 0; i < gridPointByDirection.Count; i++)
				{
					this.LookupEnemiesAtGridXY(obj, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, enemiesList);
				}
			}
		}

		public void LookupRolesInSquare(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, int nWidth, List<object> rolesList, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(srcX, srcY, radius);
			if (null != list)
			{
				Point center = new Point((double)srcX, (double)srcY);
				Point center2 = default(Point);
				center2.X = (double)((srcX + toX) / 2);
				center2.Y = (double)((srcY + toY) / 2);
				int directionX = toX - srcX;
				int directionY = toY - srcY;
				for (int i = 0; i < list.Count; i++)
				{
					ObjectTypes objectType = (list[i] as IObject).ObjectType;
					if ((objectType & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (objectType == ObjectTypes.OT_MONSTER)
						{
							if ((list[i] as Monster).VLife > 0.0)
							{
								if (copyMapId == (list[i] as Monster).CopyMapID)
								{
									Point target = new Point((list[i] as Monster).CurrentPos.X, (list[i] as Monster).CurrentPos.Y);
									if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
									{
										rolesList.Add(list[i]);
									}
									else if (Global.InCircle(target, center, 100.0))
									{
										rolesList.Add(list[i]);
									}
								}
							}
						}
						else if (objectType == ObjectTypes.OT_CLIENT)
						{
							GameClient gameClient = list[i] as GameClient;
							if (gameClient != null)
							{
								if ((list[i] as GameClient).ClientData.LifeV > 0)
								{
									if (copyMapId == (list[i] as GameClient).ClientData.CopyMapID)
									{
										Point target = new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY);
										if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
										{
											rolesList.Add(gameClient);
										}
										else if (Global.InCircle(target, center, 100.0))
										{
											rolesList.Add(gameClient);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double num = 0.0;
				double num2 = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out num, out num2);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					ObjectTypes objectType = (list[i] as IObject).ObjectType;
					if ((objectType & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (objectType == ObjectTypes.OT_MONSTER)
						{
							if (copyMapId == (list[i] as Monster).CopyMapID)
							{
								if ((list[i] as Monster).VLife > 0.0)
								{
									if (Global.InCircleByAngle((list[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(list[i]);
									}
									else if (Global.InCircle((list[i] as Monster).SafeCoordinate, center, 100.0))
									{
										enemiesList.Add(list[i]);
									}
								}
							}
						}
						else if (objectType == ObjectTypes.OT_CLIENT)
						{
							GameClient gameClient = list[i] as GameClient;
							if (gameClient != null)
							{
								if (copyMapId == gameClient.ClientData.CopyMapID)
								{
									Point target = new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY);
									if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(gameClient);
									}
									else if (Global.InCircle(target, center, 160.0))
									{
										enemiesList.Add(gameClient);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircle(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, List<object> enemiesList, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					ObjectTypes objectType = (list[i] as IObject).ObjectType;
					if ((objectType & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (objectType == ObjectTypes.OT_MONSTER)
						{
							if (copyMapId == (list[i] as Monster).CopyMapID)
							{
								if ((list[i] as Monster).VLife > 0.0)
								{
									if (Global.InCircle((list[i] as Monster).SafeCoordinate, center, (double)radius))
									{
										enemiesList.Add(list[i]);
									}
								}
							}
						}
						else if (objectType == ObjectTypes.OT_CLIENT)
						{
							GameClient gameClient = list[i] as GameClient;
							if (gameClient != null)
							{
								if (copyMapId == gameClient.ClientData.CopyMapID)
								{
									Point target = new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY);
									if (Global.InCircle(target, center, (double)radius))
									{
										enemiesList.Add(gameClient);
									}
								}
							}
						}
					}
				}
			}
		}

		public void SendMonsterToClients(SocketListener sl, Monster monster, TCPOutPacketPool pool, List<object> objList, int cmd)
		{
			if (null != objList)
			{
				if (monster.VLife > 0.0 && monster.Alive)
				{
					MonsterData monsterData = monster.GetMonsterData();
					if (monsterData.LifeV <= 0.0)
					{
						Debug.WriteLine(string.Format("怪物 Role{0} 生命值为0， 不再发送", monster.RoleID));
					}
					else
					{
						for (int i = 0; i < objList.Count; i++)
						{
							if (objList[i] is GameClient)
							{
								TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(monsterData, pool, cmd);
								if (!sl.SendData((objList[i] as GameClient).ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		public int SendMonstersToClient(SocketListener sl, GameClient client, TCPOutPacketPool pool, List<object> objList, int cmd)
		{
			int result;
			if (null == objList)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				while (num2 < objList.Count && num2 < 50)
				{
					if (objList[num2] is Monster)
					{
						if (!(objList[num2] is Robot))
						{
							if ((objList[num2] as Monster).VLife > 0.0 && (objList[num2] as Monster).Alive)
							{
								MonsterData monsterData = (objList[num2] as Monster).GetMonsterData();
								if (GameManager.FlagEnableHideFlags)
								{
									if (monsterData.EquipmentBody < 0)
									{
										monsterData.EquipmentBody = 0;
									}
								}
								TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(monsterData, pool, cmd);
								if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
								{
									break;
								}
								num++;
								if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
								{
									BossLifeLog bossAttackLog = MoYuLongXue.GetBossAttackLog(client.ClientData.Faction, monsterData.RoleID);
									if (null != bossAttackLog)
									{
										client.sendCmd<BossLifeLog>(1906, bossAttackLog, false);
									}
								}
							}
						}
					}
					num2++;
				}
				result = num;
			}
			return result;
		}

		public void ProcessMonsterDead(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster, int enemyExperience, int enemyMoney, int injure)
		{
			monster.TimedActionMgr.RemoveItem(0);
			if (attacker is GameClient)
			{
				this.ProcessMonsterDeadByClient(sl, pool, attacker as GameClient, monster, enemyExperience, enemyMoney);
				GameClient gameClient = attacker as GameClient;
				gameClient.passiveSkillModule.OnKillMonster(gameClient);
			}
			else if (attacker is Monster)
			{
				this.ProcessMonsterDeadByMonster(sl, pool, attacker as Monster, monster, enemyExperience, enemyMoney);
			}
			if ((int)(Math.Pow(2.0, 31.0) - 1.0) != injure)
			{
				if (1001 == monster.MonsterType && null != monster.OwnerClient)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster.OwnerClient, StringUtil.substitute(GLang.GetLang(502, new object[0]), new object[]
					{
						monster.MonsterInfo.VSName
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 28);
				}
			}
		}

		private void ProcessMonsterDeadByClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
		{
			if (!monster.HandledDead)
			{
				monster.HandledDead = true;
				if (monster.MonsterType != 1001 || monster.OwnerClient != client)
				{
					GameManager.AngelTempleMgr.KillAngelBoss(client, monster);
					if (monster.MonsterInfo.FallBelongTo == 1)
					{
						int attackerFromList = monster.GetAttackerFromList();
						if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
							if (null != gameClient)
							{
								client = gameClient;
							}
						}
					}
					bool flag = true;
					if (client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode)
					{
						flag = false;
					}
					TeamData teamData = null;
					if (client.ClientData.TeamID > 0 && flag)
					{
						teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
					}
					if (null == teamData)
					{
						this.ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
					}
					else
					{
						int num = 0;
						List<GameClient> list = new List<GameClient>();
						lock (teamData)
						{
							for (int i = 0; i < teamData.TeamRoles.Count; i++)
							{
								if (teamData.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									num++;
									list.Add(client);
								}
								else
								{
									GameClient gameClient2 = GameManager.ClientMgr.FindClient(teamData.TeamRoles[i].RoleID);
									if (null != gameClient2)
									{
										if (gameClient2.ClientData.MapCode == client.ClientData.MapCode)
										{
											if (gameClient2.ClientData.CopyMapID == client.ClientData.CopyMapID)
											{
												if (Global.InCircle(new Point((double)gameClient2.ClientData.PosX, (double)gameClient2.ClientData.PosY), monster.SafeCoordinate, 4000.0))
												{
													num++;
													list.Add(gameClient2);
												}
											}
										}
									}
								}
							}
						}
						if (list.Count >= 1)
						{
							int num2 = (int)((double)(enemyExperience * (list.Count - 1)) * 0.1) / list.Count;
							for (int i = 0; i < list.Count; i++)
							{
								if (client == list[i])
								{
									this.ProcessSpriteKillMonster(sl, pool, list[i], monster, enemyExperience + num2, 0);
								}
								else
								{
									this.ProcessSpriteKillMonster(sl, pool, list[i], monster, num2, 0);
								}
							}
						}
						else
						{
							this.ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
						}
					}
					monster.WhoKillMeID = client.ClientData.RoleID;
					monster.WhoKillMeName = Global.FormatRoleName(client, client.ClientData.RoleName);
					GameManager.GuildCopyMapMgr.ProcessMonsterDead(client, monster);
					CopyTargetManager.ProcessMonsterDead(client, monster);
					List<int> attackerList = monster.GetAttackerList();
					for (int i = 0; i < attackerList.Count; i++)
					{
						if (client.ClientData.RoleID != attackerList[i])
						{
							GameClient gameClient2 = GameManager.ClientMgr.FindClient(attackerList[i]);
							if (null != gameClient2)
							{
								if (gameClient2.ClientData.MapCode == client.ClientData.MapCode)
								{
									if (gameClient2.ClientData.CopyMapID == client.ClientData.CopyMapID)
									{
										if (Global.InCircle(new Point((double)gameClient2.ClientData.PosX, (double)gameClient2.ClientData.PosY), monster.SafeCoordinate, 500.0))
										{
											ProcessTask.Process(sl, pool, gameClient2, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster, null, 0, -1L, null);
										}
									}
								}
							}
						}
					}
					List<GoodsPackItem> list2 = GameManager.GoodsPackMgr.ProcessMonster(sl, pool, client, monster);
					if (monster.MonsterType == 401)
					{
						string text = string.Empty;
						int i = 0;
						while (list2 != null && i < list2.Count)
						{
							text = ((i == 0) ? EventLogManager.MakeGoodsDataPropString(list2[i].GoodsDataList) : (text + EventLogManager.AddGoodsDataPropString(list2[i].GoodsDataList)));
							i++;
						}
						EventLogManager.AddBossDiedEvent(client, monster.MonsterInfo.ExtensionID, monster.CurrentMapCode, monster.GetMonsterBirthTick(), TimeUtil.NOW() * 10000L, text);
					}
					GameManager.ClientMgr.ChangeRoleLianZhan(sl, pool, client, monster, 1);
					Global.BroadcastXKilledMonster(client, monster);
					GlobalEventSource.getInstance().fireEvent(new KillMonsterEventObject(monster, client));
					GameManager.ClientMgr.UpdateKillBoss(client, 1, monster, false);
					Global.AddBattleKilledNum(client, monster, monster.MonsterInfo.BattlePersonalJiFen, monster.MonsterInfo.BattleZhenYingJiFen);
					this.ProcessOtherEventsOnMonsterDead(sl, pool, client, monster);
					this.ProcessMonsterDeadEvents(sl, pool, client, monster);
					if (401 == monster.MonsterType)
					{
						TimerBossManager.getInstance().RemoveBoss(monster.MonsterInfo.ExtensionID);
					}
					if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
					{
						ChengJiuManager.OnMonsterKilled(client, monster);
						DailyActiveManager.ProcessDailyActiveKillMonster(client, monster);
					}
					SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.KillMonsterInMap);
					sevenDayGoalEventObject.Arg1 = client.ClientData.MapCode;
					sevenDayGoalEventObject.Arg2 = monster.MonsterInfo.ExtensionID;
					GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
					DBRoleBufferManager.ProcessWaWaGiveExperience(client, monster);
					if (client.ClientData.MapCode == 6090)
					{
						FreshPlayerCopySceneManager.KillMonsterInFreshPlayerScene(client, monster);
					}
					if (Global.IsInExperienceCopyScene(client.ClientData.MapCode))
					{
						ExperienceCopySceneManager.ExperienceCopyMapKillMonster(client, monster);
					}
					if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
					{
						GameManager.BloodCastleCopySceneMgr.KillMonsterABloodCastCopyScene(client, monster);
					}
					if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
					{
						GameManager.DaimonSquareCopySceneMgr.DaimonSquareSceneKillMonster(client, monster);
					}
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(client.ClientData.FuBenID))
					{
						LuoLanFaZhenCopySceneManager.OnKillMonster(client, monster);
					}
					if (ElementWarManager.getInstance().IsElementWarCopy(client.ClientData.FuBenID))
					{
						ElementWarManager.getInstance().KillMonster(client, monster);
					}
				}
			}
		}

		private void ProcessMonsterDeadByMonster(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster monster, int enemyExperience, int enemyMoney)
		{
			if (!monster.HandledDead)
			{
				if (null != attacker.OwnerClient)
				{
					this.ProcessMonsterDeadByClient(sl, pool, attacker.OwnerClient, monster, enemyExperience, enemyMoney);
				}
				else
				{
					monster.HandledDead = true;
					GameManager.GoodsPackMgr.ProcessMonster(sl, pool, attacker, monster);
					this.ProcessMonsterDeadEvents(sl, pool, attacker, monster);
				}
			}
		}

		private void ProcessSpriteKillMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
		{
			int level = client.ClientData.Level;
			int num = 0;
			if (enemyExperience > 0)
			{
				int num2 = enemyExperience;
				double num3 = DBRoleBufferManager.ProcessDblAndThreeExperience(client);
				if (SpecailTimeManager.JugeIsDoulbeExperienceAndLingli())
				{
					num3 += 1.0;
				}
				HeFuAwardTimesActivity heFuAwardTimesActivity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				if (heFuAwardTimesActivity != null && heFuAwardTimesActivity.InActivityTime() && monster.CopyMapID <= 0)
				{
					if ((double)heFuAwardTimesActivity.activityTimes > 0.0 && SpecailTimeManager.InSpercailTime(heFuAwardTimesActivity.specialTimeID))
					{
						num3 += (double)heFuAwardTimesActivity.activityTimes - 1.0;
					}
				}
				num3 += Global.ProcessLingDiMonsterExperience(client, monster);
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 100))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 100);
					if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
					{
						num3 += (double)bufferDataByID.BufferVal / 100.0;
					}
				}
				if (DBRoleBufferManager.ProcessMonthVIP(client) > 0.0)
				{
					double vipGuMuExperienceMultiple = Global.GetVipGuMuExperienceMultiple(client);
					num3 += vipGuMuExperienceMultiple;
				}
				num3 += Global.ProcessLingDiMonsterExperience(client, monster);
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 89))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 89);
					if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
					{
						int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengJiTanBUFF", ',');
						int key = 0;
						if (paramValueIntArrayByName != null && (long)paramValueIntArrayByName.Length > bufferDataByID.BufferVal)
						{
							key = paramValueIntArrayByName[(int)(checked((IntPtr)bufferDataByID.BufferVal))];
						}
						List<MagicActionItem> list = new List<MagicActionItem>();
						if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(key, out list))
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].MagicActionID == MagicActionIDs.DB_ADD_MULTIEXP)
								{
									num3 += list[i].MagicActionParams[0];
								}
							}
						}
					}
				}
				enemyExperience = (int)((double)enemyExperience * num3);
				int num4 = 0;
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 99))
				{
					if (client.ClientData.nTempWorldLevelPer > 0.0)
					{
						num4 = (int)((double)num2 * client.ClientData.nTempWorldLevelPer);
					}
				}
				enemyExperience += num4;
				GameManager.ClientMgr.ProcessRoleExperience(client, (long)enemyExperience, true, false, true, "none");
			}
			long num5 = (long)monster.MonsterInfo.RebornExp;
			if (num5 > 0L)
			{
				double lianZhanExpRate = GameManager.ClientMgr.GetLianZhanExpRate(client);
				num5 = (long)((double)num5 * (1.0 + lianZhanExpRate));
				double num3 = DBRoleBufferManager.ProcessRebornMultiExperience(client);
				num5 = (long)((int)((double)num5 * num3));
				RebornManager.getInstance().ProcessRoleExperience(client, num5, MoneyTypes.RebornExpMonster, true, false, true, "none");
			}
			if (enemyMoney > 0)
			{
				if (num > 10)
				{
					enemyMoney = (int)((double)enemyMoney * (1.0 - (double)(num - 10) * 0.045));
					enemyMoney = Global.GMax(0, enemyMoney);
				}
				enemyMoney = Global.FilterValue(client, enemyMoney);
				if (enemyMoney > 0)
				{
					enemyMoney = (int)((double)enemyMoney * DBRoleBufferManager.ProcessDblAndThreeMoney(client));
					GameManager.ClientMgr.AddMoney1(sl, Global._TCPManager.tcpClientPool, pool, client, enemyMoney, "杀死怪物", false);
				}
			}
			ProcessTask.Process(sl, pool, client, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster, null, 0, -1L, null);
			GameManager.CopyMapMgr.ProcessKilledMonster(client, monster);
		}

		public void ProcessMonsterDeadEvents(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster)
		{
			if (901 == monster.MonsterType)
			{
				GameManager.ShengXiaoGuessMgr.OnBossKilled();
			}
		}

		public bool ChangePosition(SocketListener sl, TCPOutPacketPool pool, Monster monster, int toMapX, int toMapY, int toMapDirection, int nID, int animation = 0)
		{
			if (toMapDirection > 0)
			{
				monster.Direction = (double)toMapDirection;
			}
			monster.LastSeekEnemyTicks = TimeUtil.NOW();
			monster.VisibleItemList = null;
			this.LoseTarget(monster);
			monster.Coordinate = new Point((double)toMapX, (double)toMapY);
			List<object> all9Clients = Global.GetAll9Clients(monster);
			bool result;
			if (null == all9Clients)
			{
				result = true;
			}
			else
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					monster.RoleID,
					toMapX,
					toMapY,
					toMapDirection,
					animation
				});
				GameManager.ClientMgr.SendToClients(sl, pool, null, all9Clients, strCmd, nID);
				if (monster._Action != GActions.Stand)
				{
					List<object> objsList = all9Clients;
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, objsList);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = true;
			}
			return result;
		}

		public void ProcessOtherEventsOnMonsterDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster)
		{
		}

		public void AddSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, monster.VLife + lifeV);
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, all9Clients, 0);
				}
			}
		}

		public void SubSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					monster.VLife = Global.GMax(0.0, monster.VLife - lifeV);
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, all9Clients, 0);
				}
			}
		}

		public void AddSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
		{
			if (monster.VLife > 0.0)
			{
				monster.VMana = (double)((int)Global.GMin(monster.MonsterInfo.VManaMax, monster.VMana + magicV));
				List<object> all9Clients = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, all9Clients, 0);
			}
		}

		public void SubSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
		{
			if (monster.VLife > 0.0)
			{
				monster.VMana = (double)((int)Global.GMax(0.0, monster.VMana - magicV));
				List<object> all9Clients = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, all9Clients, 0);
			}
		}

		public int InjureToEnemy(Monster monster, int injured)
		{
			injured = DBMonsterBuffer.ProcessHuZhaoSubLifeV(monster, Math.Max(0, injured));
			injured = DBMonsterBuffer.ProcessWuDiHuZhaoNoInjured(monster, Math.Max(0, injured));
			injured = DBMonsterBuffer.ProcessMarriageFubenInjured(monster, Math.Max(0, injured));
			return Math.Max(0, injured);
		}

		public int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			int result = 0;
			if ((enemy as Monster).VLife > 0.0 && (enemy as Monster).Alive)
			{
				bool flag = false;
				bool flag2 = false;
				if (enemy.ManagerType != 0)
				{
					PreMonsterInjureEventObject preMonsterInjureEventObject = new PreMonsterInjureEventObject(client, enemy, enemy.ManagerType);
					flag2 = GlobalEventSource4Scene.getInstance().fireEvent(preMonsterInjureEventObject, enemy.ManagerType);
					if (flag2)
					{
						injure = preMonsterInjureEventObject.Injure;
					}
				}
				if (injure <= 0)
				{
					if (!flag2)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(client, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType || 2 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(client, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
				}
				MapSettingItem mapSettingInfo = Data.GetMapSettingInfo(client.ClientData.MapCode);
				EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
				int num = 0;
				int num2 = -1;
				if (!flag2)
				{
					if (injure > 0)
					{
						int num3 = (int)RoleAlgorithm.GetLifeStealV(client);
						RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "打怪击中恢复");
						if (num3 > 0 && client.ClientData.CurrentLifeV > 0 && client.ClientData.CurrentLifeV < client.ClientData.LifeV)
						{
							roleRelifeLog.hpModify = true;
							roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
							flag = true;
							client.ClientData.CurrentLifeV += num3;
						}
						if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
						{
							client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						}
						roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
						SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
						injure = this.InjureToEnemy(enemy as Monster, injure);
						injure = DBRoleBufferManager.ProcessAntiBoss(client, enemy as Monster, injure);
						if (enemy is Robot)
						{
							injure /= 2;
						}
					}
					injure = (int)((double)injure * mapSettingInfo.NormalHuntNum);
					result = injure;
					num = GameManager.MerlinInjureMgr.CalcMerlinInjure(client, enemy, injure, ref eMerlinType);
					num2 = RoleAlgorithm.CallAttackArmor(client, enemy, ref injure, ref num);
					int num4 = RebornManager.getInstance().CalcRebornInjure(client, enemy, injurePercent, baseRate, ref burst);
					injure += (int)((double)num4 * mapSettingInfo.RebornHuntNum);
				}
				if (enemy.ManagerType != 0)
				{
					AfterMonsterInjureEventObject afterMonsterInjureEventObject = new AfterMonsterInjureEventObject(client, enemy, enemy.ManagerType, injure, num);
					flag2 = GlobalEventSource4Scene.getInstance().fireEvent(afterMonsterInjureEventObject, enemy.ManagerType);
					if (flag2)
					{
						injure = afterMonsterInjureEventObject.Injure;
						num = afterMonsterInjureEventObject.MerlinInjure;
					}
				}
				double vlife = (enemy as Monster).VLife;
				(enemy as Monster).VLife -= (double)Global.GMax(0, injure + num);
				(enemy as Monster).VLife = Global.GMax((enemy as Monster).VLife, 0.0);
				double vlife2 = (enemy as Monster).VLife;
				GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(enemy as Monster, client, 0));
				Monster monster = enemy as Monster;
				int num5 = (int)(vlife - vlife2);
				if (vlife >= monster.MonsterInfo.VLifeMax || monster.Flags.InjureEvent)
				{
					GlobalEventSource.getInstance().fireEvent(new MonsterInjuredEventObject(enemy as Monster, client, injure));
				}
				if (client.ClientData.ExcellenceProp[15] > 0.0)
				{
					int randomNumber = Global.GetRandomNumber(0, 101);
					if ((double)randomNumber <= client.ClientData.ExcellenceProp[15] * 100.0)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						flag = true;
					}
				}
				if (client.ClientData.ExcellenceProp[16] > 0.0)
				{
					int randomNumber = Global.GetRandomNumber(0, 101);
					if ((double)randomNumber <= client.ClientData.ExcellenceProp[16] * 100.0)
					{
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						flag = true;
					}
				}
				bool flag3 = false;
				if ((enemy as Monster).VLife <= 0.0)
				{
					flag3 = true;
				}
				Point hitToGrid = new Point(-1.0, -1.0);
				if (flag3)
				{
					hitToGrid = ChuanQiUtils.HitFly(client, enemy as Monster, ((enemy as Monster).VLife <= 0.0) ? 2 : 1);
					if ((int)hitToGrid.X > 0 && (int)hitToGrid.Y > 0)
					{
					}
				}
				if (!flag3 && nHitFlyDistance > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int num6 = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
					if (num6 > 0)
					{
						hitToGrid = ChuanQiUtils.HitFly(client, enemy, num6);
					}
				}
				if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
				{
					GameManager.AngelTempleMgr.ProcessAttackBossInAngelTempleScene(client, enemy, num5);
				}
				Interlocked.Add(ref client.SumDamageForCopyTeam, (long)num5);
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
				{
					MoYuLongXue.ProcessAttack(client, enemy, num5);
				}
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZhuanShengShiLian.ProcessAttack(client, enemy, num5);
				}
				(enemy as Monster).AddAttacker(client, Global.GMax(0, injure + num), null);
				client.ClientData.RoleIDAttackebByMyself = (enemy as Monster).RoleID;
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(client, enemy as Monster);
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy as Monster, client);
				if (client.ClientData.DSHideStart > 0L)
				{
					Global.RemoveBufferData(client, 41);
					client.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				SceneUIClasses sceneUIClasses = mapSceneType;
				if (sceneUIClasses != 48)
				{
					switch (sceneUIClasses)
					{
					case 53:
						CompMineManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						break;
					case 54:
						RebornBoss.getInstance().OnInjureMonster(client, enemy, (long)num5);
						break;
					case 57:
						ZorkBattleManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						break;
					}
				}
				else
				{
					CompManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
				}
				if ((enemy as Monster).VLife <= 0.0)
				{
					Global.ProcessMonsterDieForRoleAttack(sl, pool, client, enemy, injure + num);
					if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
					{
						if (MoYuLongXue.ProcessMonsterDie(enemy))
						{
						}
					}
					else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
					{
						ZhuanShengShiLian.ProcessBossDie(client, enemy);
					}
				}
				else if ((enemy as Monster).LockObject < 0)
				{
					(enemy as Monster).LockObject = client.ClientData.RoleID;
					(enemy as Monster).LockFocusTime = TimeUtil.NOW();
				}
				int attackerFromList = (enemy as Monster).GetAttackerFromList();
				if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (enemy as Monster).RoleID, 0, 0, vlife2, gameClient.ClientData.Level, hitToGrid, num, eMerlinType, num2 + 1);
						ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, enemy.RoleID, 0, 0, vlife2, 0L, num, eMerlinType, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as Monster).RoleID, burst, injure, vlife2, client.ClientData.Level, hitToGrid, num, eMerlinType, num2 + 1);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.RoleID, burst, injure, vlife2, 0L, num, eMerlinType, num2 + 1);
				Global.ProcessDamageThorn(sl, pool, client, enemy, injure);
				if (flag)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(sl, pool, client, true, false, 7);
				}
				sceneUIClasses = mapSceneType;
				if (sceneUIClasses <= 39)
				{
					if (sceneUIClasses != 27)
					{
						if (sceneUIClasses == 39)
						{
							KingOfBattleManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						}
					}
					else
					{
						YongZheZhanChangManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
					}
				}
				else
				{
					switch (sceneUIClasses)
					{
					case 43:
						LingDiCaiJiManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						break;
					case 44:
						break;
					case 45:
						BangHuiMatchManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						break;
					default:
						if (sceneUIClasses != 49)
						{
							if (sceneUIClasses == 52)
							{
								CompBattleManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
							}
						}
						else
						{
							WanMoXiaGuManager.getInstance().OnInjureMonster(client, enemy, (long)num5);
						}
						break;
					}
				}
				GameManager.damageMonitor.Out(client);
			}
			return result;
		}

		public int Monster_NotifyInjured(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			int result = 0;
			if ((enemy as Monster).VLife > 0.0 && (enemy as Monster).Alive)
			{
				bool flag = false;
				if (enemy.ManagerType != 0)
				{
					PreMonsterInjureEventObject preMonsterInjureEventObject = new PreMonsterInjureEventObject(attacker, enemy, enemy.ManagerType);
					flag = GlobalEventSource4Scene.getInstance().fireEvent(preMonsterInjureEventObject, enemy.ManagerType);
					if (flag)
					{
						injure = preMonsterInjureEventObject.Injure;
					}
				}
				if (injure <= 0)
				{
					if (!flag)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(attacker, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType || 2 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(attacker, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
				}
				if (!flag)
				{
					int num = RebornManager.getInstance().CalcRebornInjure(attacker, enemy, injurePercent, baseRate, ref burst);
					MapSettingItem mapSettingInfo = Data.GetMapSettingInfo(attacker.CurrentMapCode);
					injure = (int)((double)injure * mapSettingInfo.NormalHuntNum) + (int)((double)num * mapSettingInfo.RebornHuntNum);
				}
				if (enemy.ManagerType != 0)
				{
					AfterMonsterInjureEventObject afterMonsterInjureEventObject = new AfterMonsterInjureEventObject(attacker, enemy, enemy.ManagerType, injure, 0);
					flag = GlobalEventSource4Scene.getInstance().fireEvent(afterMonsterInjureEventObject, enemy.ManagerType);
					if (flag)
					{
						injure = afterMonsterInjureEventObject.Injure;
					}
				}
				result = injure;
				double vlife = (enemy as Monster).VLife;
				(enemy as Monster).VLife -= (double)Global.GMax(0, injure);
				(enemy as Monster).VLife = Global.GMax((enemy as Monster).VLife, 0.0);
				double vlife2 = (enemy as Monster).VLife;
				if ((enemy as Monster).VLife == 0.0)
				{
					int num2 = (int)vlife;
				}
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy, attacker);
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(attacker, enemy);
				if (null != attacker.OwnerClient)
				{
					(enemy as Monster).AddAttacker(attacker.OwnerClient, Global.GMax(0, injure), attacker);
				}
				if ((enemy as Monster).VLife <= 0.0)
				{
					int vexperience = (enemy as Monster).MonsterInfo.VExperience;
					this.LoseTarget(enemy as Monster);
					if (null != attacker.OwnerClient)
					{
						Global.ProcessMonsterDieForRoleAttack(sl, pool, attacker.OwnerClient, enemy as Monster, injure);
					}
					else
					{
						Global.ProcessMonsterDieForMonsterAttack(sl, pool, attacker, enemy);
						this.ProcessMonsterDead(sl, pool, attacker, enemy as Monster, vexperience, (enemy as Monster).MonsterInfo.VMoney, injure);
						this.AddDelayDeadMonster(enemy as Monster);
					}
				}
				else if ((enemy as Monster).LockObject < 0)
				{
					(enemy as Monster).LockObject = attacker.RoleID;
					(enemy as Monster).LockFocusTime = TimeUtil.NOW();
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, attacker, attacker.MonsterZoneNode.MapCode, attacker.RoleID, (enemy as Monster).RoleID, burst, injure, vlife2, attacker.MonsterInfo.VLevel, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
			}
			return result;
		}

		public void DoMonsterHeartTimer(int mapCode = -1, int subMapCode = -1)
		{
			long num = TimeUtil.NOW();
			List<object> list = this.MyMonsterContainer._ObjectList;
			if (mapCode != -1)
			{
				list = this.MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
			}
			if (null != list)
			{
				foreach (object obj in list)
				{
					Monster monster = obj as Monster;
					if (monster.Alive)
					{
						monster.TimedActionMgr.Run(num);
						if (monster.MonsterType != 1801)
						{
							if (monster._Action != GActions.Stand)
							{
								if (num - monster.LastExecTimerTicks >= monster._HeartInterval)
								{
									monster.onSurvivalTick();
									monster.LastExecTimerTicks = num;
									monster.Timer_Tick(null, EventArgs.Empty);
								}
							}
						}
					}
				}
			}
		}

		private void MoveToRandomPoint(SocketListener sl, TCPOutPacketPool pool, Monster monster)
		{
		}

		private void CheckMonsterStandStatus(Monster monster)
		{
		}

		private bool CheckMonsterInObs(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			if (1001 == monster.MonsterType)
			{
				if (null != monster.OwnerClient)
				{
					Point currentGrid = monster.CurrentGrid;
					Point currentGrid2 = monster.OwnerClient.CurrentGrid;
					if ((int)currentGrid.X == (int)currentGrid2.X && (int)currentGrid.Y == (int)currentGrid2.Y)
					{
						return false;
					}
				}
				if (null != monster.OwnerMonster)
				{
					Point currentGrid = monster.CurrentGrid;
					Point currentGrid3 = monster.OwnerMonster.CurrentGrid;
					if ((int)currentGrid.X == (int)currentGrid3.X && (int)currentGrid.Y == (int)currentGrid3.Y)
					{
						return false;
					}
				}
			}
			int num = monster.MonsterZoneNode.MapCode / 1000;
			bool result;
			if (6 == num || 5 == num || 7 == num)
			{
				result = false;
			}
			else if (monster.IsMoving)
			{
				result = false;
			}
			else
			{
				if (ticks - monster.LastInObsJugeTicks >= 3000L)
				{
					monster.LastInObsJugeTicks = ticks;
					if (Global.InObs(ObjectTypes.OT_MONSTER, monster.MonsterZoneNode.MapCode, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, 1, 0))
					{
						Point currentGrid4 = monster.CurrentGrid;
						bool flag = true;
						if (monster.CopyMapID > 0)
						{
							if (ChuanQiUtils.CanMonsterMoveOnCopyMap(monster, (int)currentGrid4.X, (int)currentGrid4.Y))
							{
								flag = false;
							}
						}
						if (flag)
						{
							int num2 = (int)currentGrid4.X;
							int num3 = (int)currentGrid4.Y;
							ChuanQiUtils.WalkTo(monster, (Dircetions)Global.GetRandomNumber(0, 8));
							return true;
						}
						return false;
					}
				}
				result = false;
			}
			return result;
		}

		private void SearchViewRange(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int rolesNum)
		{
			if (ticks - monster.LastSeekEnemyTicks >= monster.NextSeekEnemyTicks)
			{
				monster.LastSeekEnemyTicks = ticks;
				if (rolesNum > 0 || monster.AllwaySearchEnemy)
				{
					GameManager.ClientMgr.SeekSpriteToLock(monster);
				}
			}
		}

		private void SelectTarget(Monster monster, IObject obj, long ticks)
		{
			monster.LockObject = obj.GetObjectID();
			monster.LockFocusTime = ticks;
		}

		public void LoseTarget(Monster monster)
		{
			monster.LockObject = -1;
			monster.LockFocusTime = 0L;
			monster.PetLockObjectPriority = 0;
		}

		private bool CanLock(Monster monster, IObject obj)
		{
			bool result;
			if (!Global.IsOpposition(monster, obj))
			{
				result = false;
			}
			else
			{
				GameClient gameClient = obj as GameClient;
				Monster monster2 = obj as Monster;
				if (null != monster2)
				{
					gameClient = monster2.OwnerClient;
				}
				if (monster2 == null && null != gameClient)
				{
					if (!Global.RoleIsVisible(gameClient))
					{
						return false;
					}
				}
				if (1001 != monster.MonsterType)
				{
					if (monster.DynamicMonster)
					{
						if (monster.DynamicPursuitRadius > 0)
						{
							if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
							{
								return false;
							}
						}
					}
					else if (monster.MonsterZoneNode.PursuitRadius > 0)
					{
						if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
						{
							return false;
						}
					}
				}
				int monsterType = monster.MonsterType;
				if (monsterType != 1001)
				{
					if (monsterType == 1201)
					{
						if (gameClient != null && !Global.IsRedName(gameClient) && !monster.IsAttackedBy(gameClient.ClientData.RoleID))
						{
							return false;
						}
						if (gameClient != null && obj is GameClient)
						{
						}
					}
				}
				else if (null != monster.OwnerClient)
				{
					if (2 == monster.PetAiControlType)
					{
						int num = 0;
						if (-1 != monster.LockObject)
						{
							num = monster.PetLockObjectPriority;
						}
						int num2 = this.CalculatePetAttackMasterTargetPriority(monster, obj);
						if (num2 == 0 || (monster.LockObject != obj.GetObjectID() && num2 <= num && num2 != 3))
						{
							return false;
						}
						if (monster.PetLockObjectPriority <= num2)
						{
							monster.PetLockObjectPriority = num2;
						}
					}
					else if (1 == monster.PetAiControlType)
					{
						if (obj is GameClient)
						{
							if (!Global.IsInBattle(monster.OwnerClient, obj))
							{
								return false;
							}
						}
						if (obj is Monster && (obj as Monster).MonsterType == 1201)
						{
							if (!Global.IsInBattle(monster.OwnerClient, obj))
							{
								return false;
							}
						}
					}
				}
				result = true;
			}
			return result;
		}

		public void PetAttackMasterTargetTriggerEvent(Monster monster, IObject obj)
		{
			if (1001 == monster.MonsterType && this.CanLock(monster, obj))
			{
				this.SelectTarget(monster, obj, TimeUtil.NOW());
			}
			else if (monster.CallMonster != null && this.CanLock(monster.CallMonster, obj))
			{
				this.SelectTarget(monster.CallMonster, obj, TimeUtil.NOW());
			}
		}

		public void PetAttackMasterTargetTriggerEvent(GameClient client, IObject obj)
		{
			Monster petMonsterByMonsterByType = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
			if (petMonsterByMonsterByType != null && petMonsterByMonsterByType.Alive)
			{
				this.PetAttackMasterTargetTriggerEvent(petMonsterByMonsterByType, obj);
			}
		}

		private int CalculatePetAttackMasterTargetPriority(Monster monster, IObject obj)
		{
			int result;
			if (obj == null || (monster.OwnerClient == null && null == monster.OwnerMonster))
			{
				result = 0;
			}
			else
			{
				int num = -1;
				int num2 = -1;
				GameClient gameClient = null;
				Monster monster2 = null;
				JunQiItem junQiItem = null;
				if (obj is GameClient)
				{
					gameClient = (obj as GameClient);
					num = gameClient.ClientData.RoleID;
					num2 = gameClient.ClientData.RoleIDAttackebByMyself;
				}
				else if (obj is Monster)
				{
					monster2 = (obj as Monster);
					num = monster2.RoleID;
					num2 = monster2.LockObject;
				}
				else if (obj is JunQiItem)
				{
					junQiItem = (obj as JunQiItem);
					num = junQiItem.JunQiID;
				}
				if (null != monster.OwnerClient)
				{
					if (monster.OwnerClient.InSafeRegion)
					{
						return 0;
					}
					if (monster.OwnerClient.ClientData.RoleIDAttackebByMyself == num)
					{
						return 3;
					}
					if (gameClient != null && !Global.IsOpposition(monster.OwnerClient, gameClient))
					{
						return 0;
					}
					if (monster2 != null && !Global.IsOpposition(monster.OwnerClient, monster2))
					{
						return 0;
					}
					if (junQiItem != null && !Global.IsOpposition(monster.OwnerClient, junQiItem))
					{
						return 0;
					}
					if (num2 == monster.OwnerClient.ClientData.RoleID || num == monster.OwnerClient.ClientData.RoleIDAttackMe)
					{
						return 2;
					}
					if (num2 == monster.RoleID || monster.IsAttackedBy(num))
					{
						return 1;
					}
				}
				else
				{
					if (monster.OwnerMonster.LockObject == num)
					{
						return 3;
					}
					if (num2 == monster.OwnerMonster.RoleID)
					{
						return 2;
					}
					if (num2 == monster.RoleID || monster.IsAttackedBy(num))
					{
						return 1;
					}
				}
				result = 0;
			}
			return result;
		}

		private void TryToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			bool flag = ticks - monster.LastLockEnemyTicks > 8000L || (ticks - monster.LastLockEnemyTicks > 1000L && -1 == monster.LockObject);
			if (flag && (1001 != monster.MonsterType || 2 != monster.PetAiControlType))
			{
				monster.LastLockEnemyTicks = ticks;
				if (null != monster.VisibleItemList)
				{
					int i = 0;
					while (i < monster.VisibleItemList.Count)
					{
						if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);
							if (null != gameClient)
							{
								if (!this.CanLock(monster, gameClient))
								{
									goto IL_26A;
								}
								Point currentGrid = monster.CurrentGrid;
								int num = (int)currentGrid.X;
								int num2 = (int)currentGrid.Y;
								Point currentGrid2 = gameClient.CurrentGrid;
								int num3 = (int)currentGrid2.X;
								int num4 = (int)currentGrid2.Y;
								if (Math.Abs(num - num3) + Math.Abs(num2 - num4) < 999)
								{
									this.SelectTarget(monster, gameClient, ticks);
									break;
								}
							}
							goto IL_156;
						}
						goto IL_156;
						IL_26A:
						i++;
						continue;
						IL_156:
						if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
						{
							Monster monster2 = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);
							if (this.CanLock(monster, monster2))
							{
								if (monster2 != null && monster2.CurrentCopyMapID == monster.CurrentCopyMapID)
								{
									if (monster2.OwnerClient == null || monster2.OwnerClient != monster.OwnerClient)
									{
										Point currentGrid = monster.CurrentGrid;
										int num = (int)currentGrid.X;
										int num2 = (int)currentGrid.Y;
										Point currentGrid2 = monster2.CurrentGrid;
										int num3 = (int)currentGrid2.X;
										int num4 = (int)currentGrid2.Y;
										if (Math.Abs(num - num3) + Math.Abs(num2 - num4) < 999)
										{
											this.SelectTarget(monster, monster2, ticks);
											break;
										}
									}
								}
							}
						}
						goto IL_26A;
					}
				}
			}
		}

		private bool IsLockObjectValid(Monster monster, GameClient gameClient, long ticks)
		{
			bool result;
			if (null == gameClient)
			{
				result = false;
			}
			else if (gameClient.ClientData.CurrentLifeV <= 0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point currentGrid = monster.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				Point currentGrid2 = gameClient.CurrentGrid;
				int num3 = (int)currentGrid2.X;
				int num4 = (int)currentGrid2.Y;
				result = (Math.Abs(num3 - num) <= 12 && Math.Abs(num4 - num2) <= 12);
			}
			return result;
		}

		private bool IsLockObjectValid(Monster monster, Monster targetMonster, long ticks)
		{
			bool result;
			if (null == targetMonster)
			{
				result = false;
			}
			else if (targetMonster.VLife <= 0.0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point currentGrid = monster.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				Point currentGrid2 = targetMonster.CurrentGrid;
				int num3 = (int)currentGrid2.X;
				int num4 = (int)currentGrid2.Y;
				result = (Math.Abs(num3 - num) <= 12 && Math.Abs(num4 - num2) <= 12);
			}
			return result;
		}

		private bool IsLockObjectValid(Monster monster, JunQiItem targetJunQi, long ticks)
		{
			bool result;
			if (null == targetJunQi)
			{
				result = false;
			}
			else if (targetJunQi.CurrentLifeV <= 0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point currentGrid = monster.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				Point currentGrid2 = targetJunQi.CurrentGrid;
				int num3 = (int)currentGrid2.X;
				int num4 = (int)currentGrid2.Y;
				result = (Math.Abs(num3 - num) <= 12 && Math.Abs(num4 - num2) <= 12);
			}
			return result;
		}

		private bool IsLockObjectValid(Monster monster, IObject lockObject, long ticks)
		{
			bool result;
			if (lockObject is GameClient)
			{
				result = this.IsLockObjectValid(monster, lockObject as GameClient, ticks);
			}
			else if (lockObject is Monster)
			{
				result = this.IsLockObjectValid(monster, lockObject as Monster, ticks);
			}
			else
			{
				result = (lockObject is JunQiItem && this.IsLockObjectValid(monster, lockObject as JunQiItem, ticks));
			}
			return result;
		}

		private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject lockObject, long ticks)
		{
			bool result;
			if (!this.CanLock(monster, lockObject))
			{
				this.LoseTarget(monster);
				result = false;
			}
			else if (!this.IsLockObjectValid(monster, lockObject, ticks))
			{
				this.LoseTarget(monster);
				if (monster._Action != GActions.Stand)
				{
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, all9Clients);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, Monster targetMonster, long ticks)
		{
			bool result;
			if (!this.IsLockObjectValid(monster, targetMonster, ticks))
			{
				this.LoseTarget(monster);
				if (monster._Action != GActions.Stand)
				{
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, all9Clients);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private Dircetions GetWonderingWalkDir(Monster monster)
		{
			monster.CurrentDir = (Dircetions)Global.GetRandomNumber(0, 8);
			return monster.CurrentDir;
		}

		private void Wondering(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			if (1001 == monster.MonsterType)
			{
				if (null != monster.OwnerClient)
				{
					Point currentGrid = monster.CurrentGrid;
					Point currentGrid2 = monster.OwnerClient.CurrentGrid;
					if ((int)currentGrid.X == (int)currentGrid2.X && (int)currentGrid.Y == (int)currentGrid2.Y)
					{
						return;
					}
				}
				if (null != monster.OwnerMonster)
				{
					Point currentGrid = monster.CurrentGrid;
					Point currentGrid3 = monster.OwnerMonster.CurrentGrid;
					if ((int)currentGrid.X == (int)currentGrid3.X && (int)currentGrid.Y == (int)currentGrid3.Y)
					{
						return;
					}
				}
			}
			if (monster.LockObject < 0)
			{
				if (MonsterManager.CanMonsterSeekRange(monster))
				{
					if (1001 != monster.MonsterType)
					{
						if (monster.DynamicMonster)
						{
							if (monster.DynamicPursuitRadius > 0)
							{
								if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
								{
									Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
									ChuanQiUtils.WalkTo(monster, nDir);
									return;
								}
							}
						}
						else if (monster.MonsterZoneNode.PursuitRadius > 0)
						{
							if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
							{
								Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
								ChuanQiUtils.WalkTo(monster, nDir);
								return;
							}
						}
					}
					if (Global.GetRandomNumber(0, 10) == 0)
					{
						if (monster.MoveSpeed <= 0.0)
						{
							ChuanQiUtils.TurnTo(monster, (Dircetions)Global.GetRandomNumber(0, 8));
						}
						else if (Global.GetRandomNumber(0, 4) != 0)
						{
							Dircetions nDir = this.GetWonderingWalkDir(monster);
							ChuanQiUtils.WalkTo(monster, nDir);
						}
					}
				}
			}
		}

		private void DoMonsterStandAction(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			List<object> all9Clients = Global.GetAll9Clients(monster);
			GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, all9Clients);
			monster.DestPoint = new Point(-1.0, -1.0);
			Global.RemoveStoryboard(monster.Name);
			monster.Action = GActions.Stand;
		}

		private void DoMonsterAI(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int count, int IndexOfMonsterAiAttack)
		{
			if (monster._Action == GActions.Attack)
			{
				this.DoMonsterStandAction(sl, pool, monster, ticks);
			}
			if (501 != monster.MonsterType && 601 != monster.MonsterType)
			{
				if (2000 != monster.MonsterType && 2001 != monster.MonsterType && 1601 != monster.MonsterType && 1701 != monster.MonsterType && 1501 != monster.MonsterType && 1001 != monster.MonsterType && 1502 != monster.MonsterType)
				{
					if (!monster.isReturn)
					{
						this.Wondering(sl, pool, monster, ticks);
					}
					else if (monster.CurrentGrid.X == monster.getFirstGrid().X && monster.CurrentGrid.Y == monster.getFirstGrid().Y)
					{
						monster.isReturn = false;
					}
					else
					{
						this.MonsterReturn(monster);
					}
				}
			}
		}

		private void MonsterReturn(Monster monster)
		{
			int nDir = (int)monster.CurrentDir;
			int num = (int)monster.CurrentGrid.X;
			int num2 = (int)monster.CurrentGrid.Y;
			int num3 = (int)monster.getFirstGrid().X;
			int num4 = (int)monster.getFirstGrid().Y;
			if (num != num3 || num2 != num4)
			{
				if (num3 > num)
				{
					nDir = 2;
					if (num4 > num2)
					{
						nDir = 1;
					}
					else if (num4 < num2)
					{
						nDir = 3;
					}
				}
				else if (num3 < num)
				{
					nDir = 6;
					if (num4 > num2)
					{
						nDir = 7;
					}
					else if (num4 < num2)
					{
						nDir = 5;
					}
				}
				else if (num4 > num2)
				{
					nDir = 0;
				}
				else if (num4 < num2)
				{
					nDir = 4;
				}
				ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);
			}
		}

		private void GoToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject obj, long ticks, bool justMove = false)
		{
			if (!monster.IsMoving)
			{
				if (monster.Action != GActions.Attack)
				{
					if (monster.MoveSpeed > 0.0)
					{
						if (!monster.IsMonsterDongJie())
						{
							int num = 0;
							int num2 = monster.MonsterZoneNode.MapCode / 1000;
							if (6 == num2)
							{
								num = 1000;
							}
							else if (5 == num2 || 7 == num2)
							{
								num = 100;
							}
							if (1001 == monster.MonsterType)
							{
								num = 0;
							}
							if (ticks - monster.LastActionTick >= (long)num)
							{
								if (1001 != monster.MonsterType)
								{
									if (monster.DynamicMonster)
									{
										if (monster.DynamicPursuitRadius > 0)
										{
											if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
											{
												this.LoseTarget(monster);
												return;
											}
										}
									}
									else if (monster.MonsterZoneNode.PursuitRadius > 0)
									{
										if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
										{
											this.LoseTarget(monster);
											return;
										}
									}
								}
								else
								{
									justMove = true;
								}
								Point currentGrid = monster.CurrentGrid;
								int num3 = (int)currentGrid.X;
								int num4 = (int)currentGrid.Y;
								Point currentGrid2 = obj.CurrentGrid;
								int num5 = (int)currentGrid2.X;
								int num6 = (int)currentGrid2.Y;
								int num7 = (int)monster.Direction;
								if (num3 != num5 || num4 != num6)
								{
									if (monster.VisibleItemList != null && !justMove)
									{
										int i = 0;
										while (i < monster.VisibleItemList.Count)
										{
											if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
											{
												GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);
												if (null != gameClient)
												{
													if (!Global.IsOpposition(monster, gameClient))
													{
														goto IL_3E1;
													}
													if (gameClient.ClientData.CurrentLifeV > 0)
													{
														Point currentGrid3 = gameClient.CurrentGrid;
														int num8 = (int)currentGrid3.X;
														int num9 = (int)currentGrid3.Y;
														if (Math.Abs(num3 - num8) + Math.Abs(num4 - num9) < Math.Abs(num3 - num5) + Math.Abs(num4 - num6))
														{
															this.SelectTarget(monster, gameClient, ticks);
															return;
														}
													}
												}
												goto IL_2F7;
											}
											goto IL_2F7;
											IL_3E1:
											i++;
											continue;
											IL_2F7:
											if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
											{
												Monster monster2 = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);
												if (null != monster2)
												{
													if (Global.IsOpposition(monster, monster2))
													{
														if (monster2.VLife > 0.0)
														{
															Point currentGrid3 = monster2.CurrentGrid;
															int num8 = (int)currentGrid3.X;
															int num9 = (int)currentGrid3.Y;
															if (Math.Abs(num3 - num8) + Math.Abs(num4 - num9) < Math.Abs(num3 - num5) + Math.Abs(num4 - num6))
															{
																this.SelectTarget(monster, monster2, ticks);
																return;
															}
														}
													}
												}
											}
											goto IL_3E1;
										}
									}
									int num10 = num5;
									int num11 = num6;
									if (num10 > num3)
									{
										num7 = 2;
										if (num11 > num4)
										{
											num7 = 1;
										}
										else if (num11 < num4)
										{
											num7 = 3;
										}
									}
									else if (num10 < num3)
									{
										num7 = 6;
										if (num11 > num4)
										{
											num7 = 7;
										}
										else if (num11 < num4)
										{
											num7 = 5;
										}
									}
									else if (num11 > num4)
									{
										num7 = 0;
									}
									else if (num11 < num4)
									{
										num7 = 4;
									}
									int num12 = num3;
									int num13 = num4;
									if (monster.OwnerClient != null || monster.OwnerMonster != null)
									{
										Debug.WriteLine(string.Format("ChuanQiUtils.RunTo1, dir={0}, tick={1}", num7, ticks % 10000L));
										ChuanQiUtils.RunTo1(monster, (Dircetions)num7);
									}
									else
									{
										ChuanQiUtils.WalkTo(monster, (Dircetions)num7);
									}
									currentGrid = monster.CurrentGrid;
									num3 = (int)currentGrid.X;
									num4 = (int)currentGrid.Y;
									for (int i = 0; i < 7; i++)
									{
										if (num12 != num3 || num13 != num4)
										{
											break;
										}
										if (Global.GetRandomNumber(0, 3) > 0)
										{
											num7++;
										}
										else if (num7 > 0)
										{
											num7--;
										}
										else
										{
											num7 = 7;
										}
										if (num7 > 7)
										{
											num7 = 0;
										}
										ChuanQiUtils.WalkTo(monster, (Dircetions)num7);
										currentGrid = monster.CurrentGrid;
										num3 = (int)currentGrid.X;
										num4 = (int)currentGrid.Y;
									}
								}
							}
						}
					}
				}
			}
		}

		public bool TargetInAttackRange(Monster attacker, IObject defenser, out int direction)
		{
			direction = (int)attacker.Direction;
			bool result;
			if (defenser == null)
			{
				result = false;
			}
			else if (attacker.MonsterZoneNode.MapCode != defenser.CurrentMapCode)
			{
				result = false;
			}
			else
			{
				Point currentGrid = defenser.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				Point currentGrid2 = attacker.CurrentGrid;
				int num3 = (int)currentGrid2.X;
				int num4 = (int)currentGrid2.Y;
				if (num == num3 && num2 == num4 && attacker.MoveSpeed > 0.0)
				{
					ChuanQiUtils.WalkTo(attacker, (Dircetions)Global.GetRandomNumber(0, 8));
					result = false;
				}
				else
				{
					int autoUseSkillID = attacker.GetAutoUseSkillID();
					int num5 = this.GetSkillAttackGridNum(attacker, autoUseSkillID);
					if ((double)num5 >= Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100.0)
					{
						result = true;
					}
					else
					{
						bool flag = false;
						if (num5 <= 2)
						{
							flag = true;
						}
						if (!flag && (double)num5 < Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100.0)
						{
							num5--;
							flag = true;
						}
						if (flag)
						{
							int direction2 = (int)Global.GetDirectionByAspect(num, num2, num3, num4);
							List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction2, num3, num4, num5);
							for (int i = 0; i < gridPointByDirection.Count; i++)
							{
								if (num == (int)gridPointByDirection[i].X && num2 == (int)gridPointByDirection[i].Y)
								{
									return true;
								}
							}
							result = false;
						}
						else
						{
							if (num >= num3 - num5 && num <= num3 + num5 && num2 >= num4 - num5 && num2 <= num4 + num5)
							{
								if (num < num3 && num2 == num4)
								{
									direction = 6;
									return true;
								}
								if (num > num3 && num2 == num4)
								{
									direction = 2;
									return true;
								}
								if (num == num3 && num2 < num4)
								{
									direction = 4;
									return true;
								}
								if (num == num3 && num2 > num4)
								{
									direction = 0;
									return true;
								}
								if (num < num3 && num2 < num4)
								{
									direction = 5;
									return true;
								}
								if (num > num3 && num2 < num4)
								{
									direction = 3;
									return true;
								}
								if (num < num3 && num2 > num4)
								{
									direction = 7;
									return true;
								}
								if (num > num3 && num2 > num4)
								{
									direction = 1;
									return true;
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		public void MonsterAttack(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject enemyObject, int direction, long ticks)
		{
			if (!monster.IsMoving)
			{
				if (null != enemyObject)
				{
					Point currentPos = enemyObject.CurrentPos;
					bool flag = false;
					if (monster._Action != GActions.Attack && monster.Action != GActions.PreAttack)
					{
						if (monster._ToExecSkillID > 0)
						{
							flag = true;
						}
						else if (ticks - monster.LastAttackActionTick >= monster.MaxAttackTimeSlot)
						{
							flag = true;
						}
					}
					else if (monster._Action == GActions.PreAttack || monster._Action == GActions.Stand)
					{
						double num = this.monsterMoving.CalcDirection(monster, currentPos);
						if (num != monster.SafeDirection && monster.CurrentMagic < 1)
						{
							direction = (int)num;
							flag = true;
						}
						else if (monster.EnemyTarget != currentPos && monster.CurrentMagic < 1)
						{
							flag = true;
						}
					}
					if (flag)
					{
						this.InstantAttack(monster, (double)direction, currentPos);
					}
				}
			}
		}

		private int GetMapRolesCount(Dictionary<int, int> dict, int mapCode)
		{
			int num = 0;
			int result;
			if (dict.TryGetValue(mapCode, out num))
			{
				result = num;
			}
			else
			{
				num = GameManager.ClientMgr.GetMapClientsCount(mapCode);
				dict[mapCode] = num;
				result = num;
			}
			return result;
		}

		public void DoMonsterAttack(SocketListener sl, TCPOutPacketPool pool, int IndexOfMonsterAiAttack, int mapCode = -1, int subMapCode = -1)
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			int num = 0;
			long ticks = TimeUtil.NOW();
			Monster monster = null;
			List<object> list = this.MyMonsterContainer._ObjectList;
			if (mapCode != -1)
			{
				list = this.MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
			}
			if (list != null && list.Count > 0)
			{
				int i = 0;
				while (i < list.Count)
				{
					try
					{
						monster = (list[i] as Monster);
					}
					catch (Exception)
					{
						goto IL_4EA;
					}
					goto IL_7D;
					IL_4EA:
					i++;
					continue;
					IL_7D:
					if (null == monster)
					{
						goto IL_4EA;
					}
					if (monster.MonsterType == 1801)
					{
						goto IL_4EA;
					}
					num++;
					if (monster.VLife <= 0.0 || !monster.Alive)
					{
						goto IL_4EA;
					}
					if (monster.OwnerClient != null)
					{
						if (this.DispatchMonsterOwnedByRole(monster, sl, pool, ticks))
						{
							goto IL_4EA;
						}
					}
					if (monster.OwnerMonster != null)
					{
						if (this.DispatchMonsterOwnedByMonster(monster, sl, pool, ticks))
						{
							goto IL_4EA;
						}
					}
					if (monster.ManagerType == 11)
					{
						EMoLaiXiCopySceneManager.MonsterMoveStepEMoLaiXiCopySenceCopyMap(monster);
						goto IL_4EA;
					}
					if (monster.MonsterType == 1501)
					{
						GlodCopySceneManager.MonsterMoveStepGoldCopySceneCopyMap(monster);
						goto IL_4EA;
					}
					if (monster.MonsterType == 1502)
					{
						CompMineManager.getInstance().MonsterMoveStep(monster);
						goto IL_4EA;
					}
					int mapRolesCount = this.GetMapRolesCount(dict, monster.MonsterZoneNode.MapCode);
					this.DoMonsterLifeMagicV(sl, pool, monster, ticks, mapRolesCount);
					if (mapRolesCount <= 0 && !monster.AllwaySearchEnemy)
					{
						this.LoseTarget(monster);
						monster.VisibleClientsNum = 0;
						if (monster.IsAutoSearchRoad)
						{
							this.MonsterAutoSearchRoad(monster);
							goto IL_4EA;
						}
						goto IL_4EA;
					}
					else
					{
						if (2000 == monster.MonsterType || 2001 == monster.MonsterType)
						{
							goto IL_4EA;
						}
						SpriteAttack.ExecMagicsManyTimeDmageQueueEx(monster);
						if (monster.VisibleClientsNum <= 0 && !monster.AllwaySearchEnemy)
						{
							if (monster.IsAutoSearchRoad)
							{
								this.MonsterAutoSearchRoad(monster);
								goto IL_4EA;
							}
							this.LoseTarget(monster);
							goto IL_4EA;
						}
						else
						{
							GActions action = monster.Action;
							if (GameManager.FlagDisableMovingOnAttack)
							{
								if (action == GActions.Attack || action == GActions.Magic || action == GActions.Bow)
								{
									goto IL_4EA;
								}
							}
							if (this.CheckMonsterInObs(sl, pool, monster, ticks))
							{
								goto IL_4EA;
							}
							this.SearchViewRange(sl, pool, monster, ticks, mapRolesCount);
							this.TryToLockObject(sl, pool, monster, ticks);
							if (monster.MonsterInfo.SeekRange > 0 && null == monster.VisibleItemList)
							{
								this.LoseTarget(monster);
							}
							if (-1 == monster.LockObject && monster.IsAutoSearchRoad)
							{
								this.MonsterAutoSearchRoad(monster);
								goto IL_4EA;
							}
							if (-1 == monster.LockObject || (501 == monster.MonsterType || 601 == monster.MonsterType || 701 == monster.MonsterType || 801 == monster.MonsterType || 901 == monster.MonsterType || 1601 == monster.MonsterType || 1701 == monster.MonsterType || 1501 == monster.MonsterType) || 1502 == monster.MonsterType)
							{
								this.DoMonsterAI(sl, pool, monster, ticks, num, IndexOfMonsterAiAttack);
								goto IL_4EA;
							}
							IObject @object = GameManager.ClientMgr.FindClient(monster.LockObject);
							if (null == @object)
							{
								@object = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.LockObject);
							}
							if (null == @object)
							{
								@object = JunQiManager.FindJunQiByID(monster.LockObject);
							}
							if (null == @object)
							{
								this.LoseTarget(monster);
								goto IL_4EA;
							}
							if (!this.CheckLockObject(sl, pool, monster, @object, ticks))
							{
								goto IL_4EA;
							}
							if (monster.IsMoving)
							{
								goto IL_4EA;
							}
							monster.isReturn = true;
							if (monster.MonsterType != 401)
							{
							}
							int direction = 0;
							if (!this.TargetInAttackRange(monster, @object, out direction))
							{
								if (monster.IsAutoSearchRoad)
								{
									this.MonsterAutoSearchRoad(monster);
								}
								else
								{
									this.GoToLockObject(sl, pool, monster, @object, ticks, false);
								}
							}
							else
							{
								this.MonsterAttack(sl, pool, monster, @object, direction, ticks);
							}
							goto IL_4EA;
						}
					}
				}
			}
		}

		public void MonsterAutoSearchRoad(Monster monster)
		{
			long num = TimeUtil.NOW();
			if (num - monster.MoveTime >= 500L)
			{
				int step = monster.Step;
				if (null != monster.PatrolPath)
				{
					int num2 = monster.PatrolPath.Count<int[]>() - 1;
					if (step >= num2)
					{
						monster.Step = num2 - 1;
						step = monster.Step;
					}
					int index = step + 1;
					int currentMapCode = monster.CurrentMapCode;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
					int num3 = monster.PatrolPath[index][0];
					int num4 = monster.PatrolPath[index][1];
					int num5 = num3;
					int num6 = num4;
					Point start = new Point((double)num5, (double)num6);
					Point currentGrid = monster.CurrentGrid;
					int currentX = (int)currentGrid.X;
					int currentY = (int)currentGrid.Y;
					double directionByAspect = Global.GetDirectionByAspect(num5, num6, currentX, currentY);
					if (ChuanQiUtils.WalkTo(monster, (Dircetions)directionByAspect) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 7.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 9.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 6.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 10.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 5.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 11.0) % 8.0)))
					{
						monster.MoveTime = num;
					}
					if (Global.GetTwoPointDistance(start, currentGrid) < 2.0)
					{
						monster.Step = step + 1;
					}
				}
			}
		}

		public bool DispatchMonsterOwnedByRole(Monster monster, SocketListener sl, TCPOutPacketPool pool, long ticks)
		{
			if (monster.OwnerClient != null)
			{
				if (monster.CurrentMapCode != monster.OwnerClient.CurrentMapCode || monster.CurrentCopyMapID != monster.OwnerClient.CurrentCopyMapID)
				{
					return !monster.OwnerClient.ClientData.WaitingNotifyChangeMap || true;
				}
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) >= 8.0 || Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) >= 8.0)
				{
					this.LoseTarget(monster);
					ChuanQiUtils.TransportTo(monster, (int)monster.OwnerClient.CurrentGrid.X, (int)monster.OwnerClient.CurrentGrid.Y, (Dircetions)monster.Direction, -1, "");
					return true;
				}
				if (monster.IsMoving)
				{
					return true;
				}
				if (monster.LockObject > 0)
				{
					return false;
				}
				bool flag = true;
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) <= 2.0 && Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) <= 2.0)
				{
					if (Global.GetRandomNumber(0, 10001) >= 100)
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.GoToLockObject(sl, pool, monster, monster.OwnerClient, ticks, true);
					return true;
				}
			}
			return false;
		}

		public bool DispatchMonsterOwnedByMonster(Monster monster, SocketListener sl, TCPOutPacketPool pool, long ticks)
		{
			if (monster.OwnerMonster != null)
			{
				if (monster.CurrentMapCode != monster.OwnerMonster.CurrentMapCode || monster.CurrentCopyMapID != monster.OwnerMonster.CurrentCopyMapID)
				{
					return true;
				}
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerMonster.CurrentGrid.X) >= 8.0 || Math.Abs(monster.CurrentGrid.Y - monster.OwnerMonster.CurrentGrid.Y) >= 8.0)
				{
					this.LoseTarget(monster);
					ChuanQiUtils.TransportTo(monster, (int)monster.OwnerMonster.CurrentGrid.X, (int)monster.OwnerMonster.CurrentGrid.Y, (Dircetions)monster.Direction, -1, "");
					return true;
				}
				if (monster.IsMoving)
				{
					return true;
				}
				if (monster.LockObject > 0)
				{
					return false;
				}
				bool flag = true;
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerMonster.CurrentGrid.X) <= 2.0 && Math.Abs(monster.CurrentGrid.Y - monster.OwnerMonster.CurrentGrid.Y) <= 2.0)
				{
					if (Global.GetRandomNumber(0, 10001) >= 100)
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.GoToLockObject(sl, pool, monster, monster.OwnerMonster, ticks, true);
					return true;
				}
			}
			return false;
		}

		public void DoMonsterLifeMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int mapRoleNum)
		{
			DBMonsterBuffer.ProcessDSTimeAddLifeNoShow(monster);
			DBMonsterBuffer.ProcessDSTimeSubLifeNoShow(monster);
			DBMonsterBuffer.ProcessAllTimeSubLifeNoShow(monster);
			if (ticks - monster.LastLifeMagicTick >= 10000L)
			{
				monster.LastLifeMagicTick = ticks;
				bool flag = false;
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					flag = true;
					double num = RoleAlgorithm.GetLifeRecoverValPercentV(monster) + DBMonsterBuffer.ProcessHuZhaoRecoverPercent(monster);
					double num2 = num * monster.MonsterInfo.VLifeMax;
					num2 += monster.VLife;
					monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, num2);
				}
				if (monster.VMana < monster.MonsterInfo.VManaMax)
				{
					flag = true;
					double num = RoleAlgorithm.GetMagicRecoverValPercentV(monster);
					double num3 = num * monster.MonsterInfo.VManaMax;
					num3 += monster.VMana;
					monster.VMana = Global.GMin(monster.MonsterInfo.VManaMax, num3);
				}
				if (flag)
				{
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, all9Clients, 0);
					GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(monster, null, 0));
				}
			}
		}

		public void AddDelayDeadMonster(Monster obj)
		{
			lock (this.ListDelayDeadMonster)
			{
				if (this.ListDelayDeadMonster.IndexOf(obj) < 0)
				{
					obj.AddToDeadQueueTicks = TimeUtil.NOW();
					this.ListDelayDeadMonster.Add(obj);
				}
			}
		}

		public void DeadMonsterImmediately(Monster obj)
		{
			obj.OnDead();
			this.AddDelayDeadMonster(obj);
		}

		public void DoMonsterDeadCall()
		{
			long num = TimeUtil.NOW();
			List<Monster> list = new List<Monster>();
			lock (this.ListDelayDeadMonster)
			{
				for (int i = 0; i < this.ListDelayDeadMonster.Count; i++)
				{
					if (101 != this.ListDelayDeadMonster[i].MonsterType)
					{
						if (1801 == this.ListDelayDeadMonster[i].MonsterType || 401 == this.ListDelayDeadMonster[i].MonsterType)
						{
						}
					}
					long num2;
					if (1601 == this.ListDelayDeadMonster[i].MonsterType)
					{
						num2 = 0L;
					}
					else
					{
						num2 = 1500L;
					}
					if (num - this.ListDelayDeadMonster[i].AddToDeadQueueTicks >= num2)
					{
						list.Add(this.ListDelayDeadMonster[i]);
					}
				}
			}
			foreach (Monster monster in list)
			{
				lock (this.ListDelayDeadMonster)
				{
					this.ListDelayDeadMonster.Remove(monster);
				}
				monster.OnDead();
			}
		}

		public bool AddKilledMonsterFirst(long monsterUniqueId)
		{
			return this.DeadMonsterUniqueIdDict.TryAdd(monsterUniqueId, TimeUtil.CurrentTicksInexact + 15000L);
		}

		public void DoDeadMonsterUniqueIdProc(long nowTicks)
		{
			foreach (KeyValuePair<long, long> keyValuePair in this.DeadMonsterUniqueIdDict)
			{
				if (nowTicks > keyValuePair.Value)
				{
					long num;
					this.DeadMonsterUniqueIdDict.TryRemove(keyValuePair.Key, out num);
				}
			}
		}

		protected int GetSkillAttackGridNum(Monster monster, int skillID)
		{
			int result;
			if (skillID < 0)
			{
				result = 1;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemXmlItem))
				{
					result = 1;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("SkillType", -1);
					if (1 == intValue)
					{
						result = 1;
					}
					else if (10 == intValue)
					{
						result = 2;
					}
					else
					{
						int intValue2 = systemXmlItem.GetIntValue("AttackDistance", -1);
						if (intValue2 > 0)
						{
							result = intValue2 / 100;
						}
						else
						{
							result = Global.MaxCache9XGridNum;
						}
					}
				}
			}
			return result;
		}

		protected void InstantAttack(Monster monster, double direction, Point enemyPos)
		{
			int autoUseSkillID = monster.GetAutoUseSkillID();
			int skillAttackGridNum = this.GetSkillAttackGridNum(monster, autoUseSkillID);
			if (autoUseSkillID > 0)
			{
				int num = this.DoMagicAttack(monster, autoUseSkillID, monster.LockObject, true);
			}
			if (-1 != skillAttackGridNum)
			{
				double direction2 = this.monsterMoving.CalcDirection(monster, enemyPos);
				monster.EnemyTarget = enemyPos;
				this.DoAttackAction(monster, direction2);
				if (monster.MagicFinish == -1)
				{
					List<ManyTimeDmageItem> manyTimeDmageItems = MagicsManyTimeDmageCachingMgr.GetManyTimeDmageItems(monster.CurrentMagic);
					if (manyTimeDmageItems != null && manyTimeDmageItems.Count > 0)
					{
						Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
					}
					else if (monster.CurrentMagic > 0)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(monster.CurrentMagic, out systemXmlItem))
						{
							if (systemXmlItem.GetIntValue("InjureType", -1) == 1)
							{
								Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
							}
						}
					}
				}
			}
		}

		public void DoAttackAction(Monster monster, double direction)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.IsMonsterDongJie())
				{
					monster.Action = GActions.Stand;
				}
				else
				{
					monster.Action = GActions.Attack;
				}
				Point enemyTarget = monster.EnemyTarget;
				List<object> all9Clients = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)direction, (int)monster.Action, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)enemyTarget.X, (int)enemyTarget.Y, 114, all9Clients);
				monster.DestPoint = new Point(-1.0, -1.0);
				Global.RemoveStoryboard(monster.Name);
			}
		}

		public int DoMagicAttack(Monster monster, int magicCode, int lockObject, bool doAttackAction = false)
		{
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
			{
				result = -3;
			}
			else
			{
				IObject targetObject = Global.GetTargetObject(monster.MonsterZoneNode.MapCode, lockObject);
				if (null == targetObject)
				{
					result = -1;
				}
				else
				{
					monster.EnemyTarget = new Point(-1.0, -1.0);
					IObject @object = null;
					if (1 != systemXmlItem.GetIntValue("TargetPos", -1))
					{
						if (-1 == systemXmlItem.GetIntValue("TargetPos", -1) || 2 == systemXmlItem.GetIntValue("TargetPos", -1))
						{
							int intValue = systemXmlItem.GetIntValue("AttackDistance", -1);
							if (!SpriteAttack.JugeMagicDistance(systemXmlItem, monster, lockObject, (int)targetObject.CurrentPos.X, (int)targetObject.CurrentPos.Y, magicCode, false))
							{
								return -1;
							}
							@object = targetObject;
						}
						else if (3 == systemXmlItem.GetIntValue("TargetPos", -1))
						{
						}
					}
					if (!monster.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
					{
						result = -1;
					}
					else if (monster.MagicFinish < 0)
					{
						result = -1;
					}
					else
					{
						monster.MyMagicCoolDownMgr.AddSkillCoolDown(monster, magicCode);
						monster.CurrentMagic = magicCode;
						monster.MagicFinish = -1;
						GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.RoleID, monster.MonsterZoneNode.MapCode, magicCode, 116);
						if (!doAttackAction)
						{
							if (null != @object)
							{
								monster.EnemyTarget = @object.CurrentPos;
							}
						}
						else
						{
							if (1 == systemXmlItem.GetIntValue("MagicType", -1))
							{
								if (null != @object)
								{
									monster.EnemyTarget = @object.CurrentPos;
								}
							}
							else if (2 == systemXmlItem.GetIntValue("TargetPos", -1))
							{
								if (null != @object)
								{
									monster.EnemyTarget = @object.CurrentPos;
								}
							}
							this.MagicAttack(monster, magicCode, 1024, 1 == systemXmlItem.GetIntValue("TargetPos", -1));
						}
						result = 0;
					}
				}
			}
			return result;
		}

		protected void MagicAttack(Monster monster, int magicCode, int magicRange, bool notChangeDirection = false)
		{
			this.SpellCasting(monster, magicCode, notChangeDirection);
		}

		protected double CalcDirection(Monster monster, Point p)
		{
			return Global.GetDirectionByTan(p.X, p.Y, monster.Coordinate.X, monster.Coordinate.Y);
		}

		protected void SpellCasting(Monster monster, int magicCode, bool notChangeDirection = false)
		{
			double direction = monster.Direction;
			if (!notChangeDirection)
			{
				direction = this.CalcDirection(monster, monster.EnemyTarget);
			}
			this.DoAttackAction(monster, direction);
		}

		public static int MinSeekRangeMonsterLevel = 0;

		private MonsterContainer MyMonsterContainer = new MonsterContainer();

		private MonsterMoving monsterMoving = new MonsterMoving();

		private ConcurrentDictionary<long, long> DeadMonsterUniqueIdDict = new ConcurrentDictionary<long, long>();

		private List<Monster> ListDelayDeadMonster = new List<Monster>();
	}
}
