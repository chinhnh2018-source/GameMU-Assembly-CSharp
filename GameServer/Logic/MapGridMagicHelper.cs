using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;

namespace GameServer.Logic
{
	public class MapGridMagicHelper
	{
		public void AddMagicHelper(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, Point centerGridXY, int gridWidthNum, int gridHeightNum, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			List<Point> list = new List<Point>();
			list.Add(centerGridXY);
			int i = (int)centerGridXY.X - gridWidthNum;
			while ((double)i <= centerGridXY.X + (double)gridWidthNum)
			{
				int num = (int)centerGridXY.Y - gridHeightNum;
				while ((double)num <= centerGridXY.Y + (double)gridHeightNum)
				{
					list.Add(new Point((double)i, (double)num));
					num++;
				}
				i++;
			}
			for (i = 0; i < list.Count; i++)
			{
				if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)list[i].X, (int)list[i].Y))
				{
					Dictionary<MagicActionIDs, GridMagicHelperItem> dictionary = null;
					string key = string.Format("{0}_{1}_{2}", list[i].X, list[i].Y, copyMapID);
					lock (this._GridMagicHelperDict)
					{
						if (!this._GridMagicHelperDict.TryGetValue(key, out dictionary))
						{
							dictionary = new Dictionary<MagicActionIDs, GridMagicHelperItem>();
							this._GridMagicHelperDict[key] = dictionary;
						}
					}
					lock (dictionary)
					{
						if (dictionary.ContainsKey(magicActionID))
						{
							goto IL_23B;
						}
					}
					GridMagicHelperItem gridMagicHelperItem = new GridMagicHelperItem
					{
						MagicActionID = magicActionID,
						MagicActionParams = magicActionParams,
						StartedTicks = TimeUtil.NOW(),
						LastTicks = TimeUtil.NOW(),
						ExecutedNum = 0,
						MapCode = mapCode
					};
					lock (dictionary)
					{
						dictionary[gridMagicHelperItem.MagicActionID] = gridMagicHelperItem;
					}
				}
				IL_23B:;
			}
		}

		public void AddMagicHelperEx(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, int posX, int posY, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY))
			{
				GridMagicHelperItemKey key = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = posX,
					PosY = posY,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				GridMagicHelperItemEx value = new GridMagicHelperItemEx
				{
					MagicActionID = magicActionID,
					MagicActionParams = magicActionParams,
					StartedTicks = TimeUtil.NOW(),
					LastTicks = TimeUtil.NOW(),
					ExecutedNum = 0,
					MapCode = mapCode
				};
				lock (this._GridMagicHelperDictEx)
				{
					if (!this._GridMagicHelperDictEx.ContainsKey(key))
					{
						this._GridMagicHelperDictEx.Add(key, value);
					}
				}
			}
		}

		public void AddMagicHelperExAction(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, Point centerGridXY, int gridWidthNum, int gridHeightNum, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			int num = (int)centerGridXY.X;
			int num2 = (int)centerGridXY.Y;
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, num, num2))
			{
				GridMagicHelperItemKey key = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = num,
					PosY = num2,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				GridMagicHelperItemEx gridMagicHelperItemEx = new GridMagicHelperItemEx
				{
					MagicActionID = magicActionID,
					MagicActionParams = magicActionParams,
					StartedTicks = TimeUtil.NOW(),
					LastTicks = TimeUtil.NOW(),
					ExecutedNum = 0,
					MapCode = mapCode,
					AttackerRoleId = (int)magicActionParams[2]
				};
				int num3 = 3;
				int num4 = magicActionParams.Length - num3 - 1;
				key.MagicActionID2 = (MagicActionIDs)magicActionParams[num3];
				gridMagicHelperItemEx.MagicActionParams2 = new double[num4];
				Array.Copy(magicActionParams, num3 + 1, gridMagicHelperItemEx.MagicActionParams2, 0, num4);
				HashSet<Point> hashSet = new HashSet<Point>();
				hashSet.Add(centerGridXY);
				int num5 = (int)centerGridXY.X - gridWidthNum;
				while ((double)num5 <= centerGridXY.X + (double)gridWidthNum)
				{
					int num6 = (int)centerGridXY.Y - gridHeightNum;
					while ((double)num6 <= centerGridXY.Y + (double)gridHeightNum)
					{
						hashSet.Add(new Point((double)num5, (double)num6));
						num6++;
					}
					num5++;
				}
				foreach (Point item in hashSet)
				{
					if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)item.X, (int)item.Y))
					{
						gridMagicHelperItemEx.PointList.Add(item);
					}
				}
				lock (this._GridMagicHelperDictEx)
				{
					if (!this._GridMagicHelperDictEx.ContainsKey(key))
					{
						this._GridMagicHelperDictEx.Add(key, gridMagicHelperItemEx);
					}
				}
			}
		}

		private bool CanExecuteItem(Dictionary<MagicActionIDs, GridMagicHelperItem> dict, GridMagicHelperItem magicHelperItem, double effectSecs, int maxNum)
		{
			long num = TimeUtil.NOW();
			long num2 = magicHelperItem.StartedTicks + (long)(effectSecs * 1000.0);
			bool result;
			if (maxNum <= 0)
			{
				if (num >= num2)
				{
					bool flag = false;
					try
					{
						Dictionary<MagicActionIDs, GridMagicHelperItem> obj = dict;
						Monitor.Enter(dict, ref flag);
						dict.Remove(magicHelperItem.MagicActionID);
					}
					finally
					{
						if (flag)
						{
							Dictionary<MagicActionIDs, GridMagicHelperItem> obj;
							Monitor.Exit(obj);
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (magicHelperItem.ExecutedNum >= maxNum)
			{
				bool flag2 = false;
				try
				{
					Dictionary<MagicActionIDs, GridMagicHelperItem> obj = dict;
					Monitor.Enter(dict, ref flag2);
					dict.Remove(magicHelperItem.MagicActionID);
				}
				finally
				{
					if (flag2)
					{
						Dictionary<MagicActionIDs, GridMagicHelperItem> obj;
						Monitor.Exit(obj);
					}
				}
				result = false;
			}
			else
			{
				long num3 = (long)(effectSecs / (double)maxNum * 1000.0);
				result = (num - magicHelperItem.LastTicks >= num3);
			}
			return result;
		}

		private bool CanExecuteItemEx(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, double effectSecs, int maxNum, long nowTicks)
		{
			long num = magicHelperItem.StartedTicks + (long)(effectSecs * 1000.0);
			bool result;
			if (maxNum <= 0)
			{
				if (nowTicks >= num)
				{
					lock (this._GridMagicHelperDictEx)
					{
						this._GridMagicHelperDictEx.Remove(key);
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (magicHelperItem.ExecutedNum >= maxNum)
			{
				lock (this._GridMagicHelperDictEx)
				{
					this._GridMagicHelperDictEx.Remove(key);
				}
				result = false;
			}
			else
			{
				long num2 = (long)(effectSecs / (double)maxNum * 1000.0);
				result = (nowTicks - magicHelperItem.LastTicks >= num2);
			}
			return result;
		}

		public void ExecuteMAttack(string gridXY, Dictionary<MagicActionIDs, GridMagicHelperItem> dict)
		{
			string[] array = gridXY.Split(new char[]
			{
				'_'
			});
			int gridX = Global.SafeConvertToInt32(array[0]);
			int gridY = Global.SafeConvertToInt32(array[1]);
			GridMagicHelperItem gridMagicHelperItem = null;
			lock (dict)
			{
				dict.TryGetValue(MagicActionIDs.FIRE_WALL, out gridMagicHelperItem);
			}
			if (null != gridMagicHelperItem)
			{
				if (this.CanExecuteItem(dict, gridMagicHelperItem, (double)((int)gridMagicHelperItem.MagicActionParams[0]), (int)gridMagicHelperItem.MagicActionParams[1]))
				{
					gridMagicHelperItem.ExecutedNum++;
					gridMagicHelperItem.LastTicks = TimeUtil.NOW();
					double attackPercent = gridMagicHelperItem.MagicActionParams[2];
					int num = (int)gridMagicHelperItem.MagicActionParams[3];
					if (-1 != num)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(num);
						if (null != gameClient)
						{
							List<object> list = new List<object>();
							GameManager.ClientMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							BiaoCheManager.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							JunQiManager.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							for (int i = 0; i < list.Count; i++)
							{
								IObject @object = list[i] as IObject;
								if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
								{
									if (@object is GameClient)
									{
										if ((@object as GameClient).ClientData.RoleID != num && Global.IsOpposition(gameClient, @object as GameClient))
										{
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as GameClient, 0, 0, 1.0, 1, false, 0, attackPercent, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
										}
									}
									else if (@object is Monster)
									{
										if (Global.IsOpposition(gameClient, @object as Monster))
										{
											GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as Monster, 0, 0, 1.0, 1, false, 0, attackPercent, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
										}
									}
									else if (@object is BiaoCheItem)
									{
										if (Global.IsOpposition(gameClient, @object as BiaoCheItem))
										{
											BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
									else if (@object is JunQiItem)
									{
										if (Global.IsOpposition(gameClient, @object as JunQiItem))
										{
											JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as JunQiItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
									else if (@object is FakeRoleItem)
									{
										if (Global.IsOpposition(gameClient, @object as FakeRoleItem))
										{
											FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as FakeRoleItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void ExecuteMUFireWall(int id, string gridXY, Dictionary<MagicActionIDs, GridMagicHelperItem> dict)
		{
			string[] array = gridXY.Split(new char[]
			{
				'_'
			});
			int gridX = Global.SafeConvertToInt32(array[0]);
			int gridY = Global.SafeConvertToInt32(array[1]);
			GridMagicHelperItem gridMagicHelperItem = null;
			lock (dict)
			{
				dict.TryGetValue((MagicActionIDs)id, out gridMagicHelperItem);
			}
			if (null != gridMagicHelperItem)
			{
				if (this.CanExecuteItem(dict, gridMagicHelperItem, (double)((int)gridMagicHelperItem.MagicActionParams[0]), (int)gridMagicHelperItem.MagicActionParams[1]))
				{
					gridMagicHelperItem.ExecutedNum++;
					gridMagicHelperItem.LastTicks = TimeUtil.NOW();
					int num = (int)gridMagicHelperItem.MagicActionParams[2];
					int num2 = (int)gridMagicHelperItem.MagicActionParams[3];
					double num3 = gridMagicHelperItem.MagicActionParams[4];
					if (-1 != num2)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(num2);
						if (null != gameClient)
						{
							List<object> list = new List<object>();
							GameManager.ClientMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							BiaoCheManager.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							JunQiManager.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
							for (int i = 0; i < list.Count; i++)
							{
								IObject @object = list[i] as IObject;
								if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
								{
									if (@object is GameClient)
									{
										if ((@object as GameClient).ClientData.RoleID != num2 && Global.IsOpposition(gameClient, @object as GameClient))
										{
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, num3, num, 0, 0, 0.0);
										}
									}
									else if (@object is Monster)
									{
										if (Global.IsOpposition(gameClient, @object as Monster))
										{
											GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num3, num, 0, 0, 0.0);
										}
									}
									else if (@object is BiaoCheItem)
									{
										if (Global.IsOpposition(gameClient, @object as BiaoCheItem))
										{
											BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, num3, num, 0);
										}
									}
									else if (@object is JunQiItem)
									{
										if (Global.IsOpposition(gameClient, @object as JunQiItem))
										{
											JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, num3, num, 0);
										}
									}
									else if (@object is FakeRoleItem)
									{
										if (Global.IsOpposition(gameClient, @object as FakeRoleItem))
										{
											FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as FakeRoleItem, 0, 0, 1.0, 1, false, 0, num3, num, 0, 1.0, 0, 0);
										}
									}
								}
							}
						}
						else
						{
							Monster monster = GameManager.MonsterMgr.FindMonster(gridMagicHelperItem.MapCode, num2);
							if (null != monster)
							{
								List<object> list = new List<object>();
								GameManager.ClientMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
								GameManager.MonsterMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
								BiaoCheManager.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
								JunQiManager.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
								for (int i = 0; i < list.Count; i++)
								{
									IObject @object = list[i] as IObject;
									if (@object.CurrentCopyMapID == monster.CurrentCopyMapID)
									{
										if (@object is GameClient)
										{
											if ((@object as GameClient).ClientData.RoleID != num2 && Global.IsOpposition(monster, @object as GameClient))
											{
												GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num3, num, 0, 0, 0.0);
											}
										}
										else if (@object is Monster)
										{
											if (Global.IsOpposition(monster, @object as Monster))
											{
												GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num3, num, 0, 0, 0.0);
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

		public void ExecuteAllItems()
		{
			List<string> list = new List<string>();
			lock (this._GridMagicHelperDict)
			{
				list = this._GridMagicHelperDict.Keys.ToList<string>();
			}
			Dictionary<MagicActionIDs, GridMagicHelperItem> dictionary = null;
			for (int i = 0; i < list.Count; i++)
			{
				dictionary = null;
				lock (this._GridMagicHelperDict)
				{
					this._GridMagicHelperDict.TryGetValue(list[i], out dictionary);
				}
				if (null != dictionary)
				{
					this.ExecuteMAttack(list[i], dictionary);
					this.ExecuteMUFireWall(268, list[i], dictionary);
					this.ExecuteMUFireWall(269, list[i], dictionary);
					this.ExecuteMUFireWall(270, list[i], dictionary);
				}
			}
		}

		public int GetObjectAddMapBuffer(int nAttackID)
		{
			int num = 0;
			lock (this._GridMagicHelperDictEx)
			{
				foreach (KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx> keyValuePair in this._GridMagicHelperDictEx)
				{
					if ((int)keyValuePair.Value.MagicActionParams[3] == nAttackID)
					{
						if (keyValuePair.Value.ExecutedNum < 1)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		public void ExecuteAllItemsEx()
		{
			long nowTicks = TimeUtil.NOW();
			this.Mgr.Run(nowTicks);
			List<KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx>> list = null;
			lock (this._GridMagicHelperDictEx)
			{
				foreach (KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx> item in this._GridMagicHelperDictEx)
				{
					if (this.CanExecuteItemEx(item.Key, item.Value, (double)((int)item.Value.MagicActionParams[0]), (int)item.Value.MagicActionParams[1], nowTicks))
					{
						if (null == list)
						{
							list = new List<KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx>>();
						}
						list.Add(item);
					}
				}
			}
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					switch (list[i].Key.MagicActionID)
					{
					case MagicActionIDs.MU_FIRE_WALL_X:
						this.ExecuteMUFireWall_X(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_SECTOR:
						this.ExecuteMUFireSector(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_STRAIGHT:
						this.ExecuteMUFireStraight(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_WALL_ACTION:
						this.ExecuteMUFireWallAction(list[i].Key, list[i].Value, nowTicks);
						break;
					}
				}
			}
		}

		public void ExecuteMUFireWall_X(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int magicActionID = (int)key.MagicActionID;
			int num = key.PosX;
			int num2 = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int num3 = (int)magicHelperItem.MagicActionParams[2];
			int num4 = (int)magicHelperItem.MagicActionParams[3];
			double num5 = magicHelperItem.MagicActionParams[4];
			int radius = (int)magicHelperItem.MagicActionParams[5];
			double num6 = magicHelperItem.MagicActionParams[6];
			double num7 = magicHelperItem.MagicActionParams[7];
			double num8 = magicHelperItem.MagicActionParams[8];
			int num9 = (int)magicHelperItem.MagicActionParams[15];
			int magicCode = (int)magicHelperItem.MagicActionParams[16];
			num *= num9;
			num2 *= num9;
			if (-1 != num4)
			{
				int num10 = magicHelperItem.MaxNum;
				GameClient gameClient = GameManager.ClientMgr.FindClient(num4);
				if (null != gameClient)
				{
					List<object> list = new List<object>();
					GameManager.ClientMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, radius, list);
					GameManager.MonsterMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, radius, list);
					JunQiManager.LookupEnemiesInCircle(gameClient, magicHelperItem.MapCode, num, num2, radius, list);
					List<object> list2 = new List<object>();
					foreach (object obj in list)
					{
						if (obj is Monster)
						{
							if (Global.IsOpposition(gameClient, obj as Monster))
							{
								list2.Add(obj);
							}
						}
						else if (obj is GameClient)
						{
							if ((obj as GameClient).ClientData.RoleID != gameClient.ClientData.RoleID)
							{
								if (Global.IsOpposition(gameClient, obj as GameClient))
								{
									list2.Add(obj);
								}
							}
						}
					}
					double magicCodeAddPercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(gameClient, list2, magicCode);
					for (int i = 0; i < list.Count; i++)
					{
						IObject @object = list[i] as IObject;
						if (gameClient.CurrentMapCode == @object.CurrentMapCode)
						{
							if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
							{
								bool flag = false;
								if (@object is GameClient)
								{
									if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(gameClient, @object as GameClient))
									{
										num10--;
										flag = true;
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, num5, num3, 0, magicCode, magicCodeAddPercent);
									}
								}
								else if (@object is Monster)
								{
									if (Global.IsOpposition(gameClient, @object as Monster))
									{
										num10--;
										flag = true;
										GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, magicCode, magicCodeAddPercent);
									}
								}
								else if (@object is BiaoCheItem)
								{
									if (Global.IsOpposition(gameClient, @object as BiaoCheItem))
									{
										num10--;
										flag = true;
										BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, num5, num3, 0);
									}
								}
								else if (@object is JunQiItem)
								{
									if (Global.IsOpposition(gameClient, @object as JunQiItem))
									{
										num10--;
										flag = true;
										JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, num5, num3, 0);
									}
								}
								else if (@object is FakeRoleItem)
								{
									if (Global.IsOpposition(gameClient, @object as FakeRoleItem))
									{
										num10--;
										flag = true;
										FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as FakeRoleItem, 0, 0, 1.0, 1, false, 0, num5, num3, 0, 1.0, 0, 0);
									}
								}
								if (flag)
								{
									if (num6 > 0.9999 || (num6 > 0.0 && Global.GetRandom() < num6))
									{
										double[] actionParams = new double[]
										{
											num7,
											num8
										};
										MagicAction.ProcessAction(gameClient, @object, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
									}
								}
								if (num10 == 0)
								{
									break;
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, num4);
					if (null != monster)
					{
						List<object> list = new List<object>();
						GameManager.ClientMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, monster.CopyMapID, num, num2, radius, list);
						GameManager.MonsterMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, monster.CopyMapID, num, num2, radius, list);
						List<object> list2 = new List<object>();
						foreach (object obj in list)
						{
							if (obj is Monster)
							{
								if (Global.IsOpposition(monster, obj as Monster))
								{
									list2.Add(obj);
								}
							}
							else if (obj is GameClient)
							{
								if (Global.IsOpposition(monster, obj as GameClient))
								{
									list2.Add(obj);
								}
							}
						}
						double magicCodeAddPercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(monster, list2, magicCode);
						for (int i = 0; i < list.Count; i++)
						{
							IObject @object = list[i] as IObject;
							if (monster.CurrentMapCode == @object.CurrentMapCode)
							{
								if (@object.CurrentCopyMapID == monster.CurrentCopyMapID)
								{
									bool flag = false;
									if (@object is GameClient)
									{
										if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(monster, @object as GameClient))
										{
											num10--;
											flag = true;
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, magicCode, magicCodeAddPercent);
										}
									}
									else if (@object is Monster)
									{
										if (Global.IsOpposition(monster, @object as Monster))
										{
											num10--;
											flag = true;
											GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, magicCode, magicCodeAddPercent);
										}
									}
									if (flag)
									{
										if (num6 > 0.9999 || (num6 > 0.0 && Global.GetRandom() < num6))
										{
											double[] actionParams = new double[]
											{
												num7,
												num8
											};
											MagicAction.ProcessAction(monster, @object, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
										}
									}
									if (num10 == 0)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void ExecuteMUFireWallAction(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int attackerRoleId = magicHelperItem.AttackerRoleId;
			if (-1 != attackerRoleId)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(attackerRoleId);
				if (null != gameClient)
				{
					List<object> list = new List<object>();
					foreach (Point point in magicHelperItem.PointList)
					{
						int gridX = (int)point.X;
						int gridY = (int)point.Y;
						GameManager.ClientMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
						GameManager.MonsterMgr.LookupEnemiesAtGridXY(gameClient, gridX, gridY, list);
					}
					for (int i = 0; i < list.Count; i++)
					{
						IObject @object = list[i] as IObject;
						if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
						{
							MagicAction.ProcessAction(gameClient, @object, key.MagicActionID2, magicHelperItem.MagicActionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attackerRoleId);
					if (null != monster)
					{
						List<object> list = new List<object>();
						foreach (Point point in magicHelperItem.PointList)
						{
							int gridX = (int)point.X;
							int gridY = (int)point.Y;
							GameManager.ClientMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, list);
						}
						for (int i = 0; i < list.Count; i++)
						{
							IObject @object = list[i] as IObject;
							if (@object.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								MagicAction.ProcessAction(monster, @object, key.MagicActionID2, magicHelperItem.MagicActionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							}
						}
					}
				}
			}
		}

		public void ExecuteMUFireSector(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int magicActionID = (int)key.MagicActionID;
			int num = key.PosX;
			int num2 = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int num3 = (int)magicHelperItem.MagicActionParams[2];
			int num4 = (int)magicHelperItem.MagicActionParams[3];
			double num5 = magicHelperItem.MagicActionParams[4];
			int radius = (int)magicHelperItem.MagicActionParams[5];
			int num6 = (int)magicHelperItem.MagicActionParams[6];
			int direction = (int)magicHelperItem.MagicActionParams[7];
			int num7 = (int)magicHelperItem.MagicActionParams[9];
			num *= num7;
			num2 *= num7;
			if (-1 != num4)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(num4);
				if (null != gameClient)
				{
					List<object> list = new List<object>();
					GameManager.ClientMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, radius, list, (double)num6, false);
					GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, radius, list, (double)num6, false);
					for (int i = 0; i < list.Count; i++)
					{
						IObject @object = list[i] as IObject;
						if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
						{
							if (@object is GameClient)
							{
								if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(gameClient, @object as GameClient))
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, num5, num3, 0, 0, 0.0);
								}
							}
							else if (@object is Monster)
							{
								if (Global.IsOpposition(gameClient, @object as Monster))
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
								}
							}
							else if (@object is BiaoCheItem)
							{
								if (Global.IsOpposition(gameClient, @object as BiaoCheItem))
								{
									BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, num5, num3, 0);
								}
							}
							else if (@object is JunQiItem)
							{
								if (Global.IsOpposition(gameClient, @object as JunQiItem))
								{
									JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, num5, num3, 0);
								}
							}
							else if (@object is FakeRoleItem)
							{
								if (Global.IsOpposition(gameClient, @object as FakeRoleItem))
								{
									FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as FakeRoleItem, 0, 0, 1.0, 1, false, 0, num5, num3, 0, 1.0, 0, 0);
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, num4);
					if (null != monster)
					{
						List<object> list = new List<object>();
						GameManager.ClientMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, monster.CopyMapID, num, num2, radius, list, (double)num6, false);
						GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, monster.CopyMapID, num, num2, radius, list, (double)num6, false);
						for (int i = 0; i < list.Count; i++)
						{
							IObject @object = list[i] as IObject;
							if (@object.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								if (@object is GameClient)
								{
									if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(monster, @object as GameClient))
									{
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
									}
								}
								else if (@object is Monster)
								{
									if (Global.IsOpposition(monster, @object as Monster))
									{
										GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
									}
								}
							}
						}
					}
				}
			}
		}

		public void ExecuteMUFireStraight(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int magicActionID = (int)key.MagicActionID;
			int num = key.PosX;
			int num2 = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int num3 = (int)magicHelperItem.MagicActionParams[2];
			int num4 = (int)magicHelperItem.MagicActionParams[3];
			double num5 = magicHelperItem.MagicActionParams[4];
			int radius = (int)magicHelperItem.MagicActionParams[5];
			int nWidth = (int)magicHelperItem.MagicActionParams[6];
			int num6 = (int)magicHelperItem.MagicActionParams[7];
			int num7 = (int)magicHelperItem.MagicActionParams[8];
			int num8 = (int)magicHelperItem.MagicActionParams[9];
			num *= num8;
			num2 *= num8;
			if (-1 != num4)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(num4);
				if (null != gameClient)
				{
					List<object> list = new List<object>();
					GameManager.ClientMgr.LookupRolesInSquare(magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, 0, 0, radius, nWidth, list);
					GameManager.MonsterMgr.LookupRolesInSquare(magicHelperItem.MapCode, gameClient.ClientData.CopyMapID, num, num2, 0, 0, radius, nWidth, list, 1);
					for (int i = 0; i < list.Count; i++)
					{
						IObject @object = list[i] as IObject;
						if (@object.CurrentCopyMapID == gameClient.CurrentCopyMapID)
						{
							if (@object is GameClient)
							{
								if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(gameClient, @object as GameClient))
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, num5, num3, 0, 0, 0.0);
								}
							}
							else if (@object is Monster)
							{
								if (Global.IsOpposition(gameClient, @object as Monster))
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
								}
							}
							else if (@object is BiaoCheItem)
							{
								if (Global.IsOpposition(gameClient, @object as BiaoCheItem))
								{
									BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, num5, num3, 0);
								}
							}
							else if (@object is JunQiItem)
							{
								if (Global.IsOpposition(gameClient, @object as JunQiItem))
								{
									JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, num5, num3, 0);
								}
							}
							else if (@object is FakeRoleItem)
							{
								if (Global.IsOpposition(gameClient, @object as FakeRoleItem))
								{
									FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, @object as FakeRoleItem, 0, 0, 1.0, 1, false, 0, num5, num3, 0, 1.0, 0, 0);
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, num4);
					if (null != monster)
					{
						List<object> list = new List<object>();
						GameManager.ClientMgr.LookupRolesInSquare(magicHelperItem.MapCode, monster.CopyMapID, num, num2, 0, 0, radius, nWidth, list);
						GameManager.MonsterMgr.LookupRolesInSquare(magicHelperItem.MapCode, monster.CopyMapID, num, num2, 0, 0, radius, nWidth, list, 1);
						for (int i = 0; i < list.Count; i++)
						{
							IObject @object = list[i] as IObject;
							if (@object.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								if (@object is GameClient)
								{
									if ((@object as GameClient).ClientData.RoleID != num4 && Global.IsOpposition(monster, @object as GameClient))
									{
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
									}
								}
								else if (@object is Monster)
								{
									if (Global.IsOpposition(monster, @object as Monster))
									{
										GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, @object as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num5, num3, 0, 0, 0.0);
									}
								}
							}
						}
					}
				}
			}
		}

		public void AddGridMagic(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, int posX, int posY, int DelayDecoration, int DecorationTime, int copyMapID, int maxHitCount = 8)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY))
			{
				GridMagicHelperItemKey itemKey = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = posX,
					PosY = posY,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				if (DelayDecoration > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
					Point pos = new Point((double)(posX * mapGrid.MapGridWidth + mapGrid.MapGridWidth / 2), (double)(posY * mapGrid.MapGridHeight + mapGrid.MapGridHeight / 2));
					DecorationManager.AddDecoToMap(mapCode, copyMapID, pos, DelayDecoration, DecorationTime * 1000, 2000, true);
				}
				long num = (long)magicActionParams[0] * 1000L;
				int execCount = (int)magicActionParams[1];
				GridMagicHelperItemEx gridMagicHelperItemEx = new GridMagicHelperItemEx();
				gridMagicHelperItemEx.MagicActionID = magicActionID;
				gridMagicHelperItemEx.MagicActionParams = magicActionParams;
				gridMagicHelperItemEx.LastTicks = TimeUtil.NOW();
				gridMagicHelperItemEx.ExecutedNum = 0;
				gridMagicHelperItemEx.MapCode = mapCode;
				gridMagicHelperItemEx.MaxNum = maxHitCount;
				if (MagicActionIDs.MU_FIRE_WALL_Y == magicActionID)
				{
					gridMagicHelperItemEx.StartedTicks = TimeUtil.NOW();
				}
				else
				{
					gridMagicHelperItemEx.StartedTicks = TimeUtil.NOW() + num;
				}
				MapGridMagicHelper.GridMagicItem context = new MapGridMagicHelper.GridMagicItem
				{
					ItemKey = itemKey,
					MagicHelperItem = gridMagicHelperItemEx
				};
				this.Mgr.AddItem(gridMagicHelperItemEx.StartedTicks, num, execCount, 0, new Action<long, object>(this.FireWallActionProc), context);
			}
		}

		public void FireWallActionProc(long execTicks, object state)
		{
			MapGridMagicHelper.GridMagicItem gridMagicItem = state as MapGridMagicHelper.GridMagicItem;
			if (null != gridMagicItem)
			{
				this.ExecuteMUFireWall_X(gridMagicItem.ItemKey, gridMagicItem.MagicHelperItem, execTicks);
			}
		}

		private Dictionary<string, Dictionary<MagicActionIDs, GridMagicHelperItem>> _GridMagicHelperDict = new Dictionary<string, Dictionary<MagicActionIDs, GridMagicHelperItem>>();

		private SortedDictionary<GridMagicHelperItemKey, GridMagicHelperItemEx> _GridMagicHelperDictEx = new SortedDictionary<GridMagicHelperItemKey, GridMagicHelperItemEx>(GridMagicHelperItemKey.Comparer);

		private TimedActionManager Mgr = new TimedActionManager();

		private class GridMagicItem
		{
			public GridMagicHelperItemKey ItemKey;

			public GridMagicHelperItemEx MagicHelperItem;
		}
	}
}
