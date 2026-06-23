using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Logic.JingJiChang;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class MonsterZoneManager
	{
		public MonsterZoneManager()
		{
			for (int i = 0; i < this.WaitingAddDynamicMonsterQueue.Length; i++)
			{
				if (null == this.WaitingAddDynamicMonsterQueue[i])
				{
					this.WaitingAddDynamicMonsterQueue[i] = new Queue<MonsterZoneQueueItem>();
				}
			}
		}

		public Dictionary<int, Monster> DictDynamicMonsterSeed { get; set; }

		public XElement AllMonstersXml
		{
			get
			{
				XElement allMonstersXml;
				lock (this._allMonsterXmlMutex)
				{
					allMonstersXml = this._allMonstersXml;
				}
				return allMonstersXml;
			}
			set
			{
				lock (this._allMonsterXmlMutex)
				{
					this._allMonstersXml = value;
				}
			}
		}

		public void LoadAllMonsterXml()
		{
			XElement xelement = null;
			try
			{
				xelement = XElement.Load(Global.GameResPath("Config/Monsters.xml"));
			}
			catch (Exception ex)
			{
			}
			if (xelement != null)
			{
				this.AllMonstersXml = xelement;
			}
		}

		private void AddMap2MonsterZoneDict(MonsterZone monsterZone)
		{
			List<MonsterZone> list = null;
			if (this.Map2MonsterZoneDict.TryGetValue(monsterZone.MapCode, out list))
			{
				list.Add(monsterZone);
			}
			else
			{
				list = new List<MonsterZone>();
				this.Map2MonsterZoneDict[monsterZone.MapCode] = list;
				list.Add(monsterZone);
			}
		}

		private List<BirthTimePoint> ParseBirthTimePoints(string s)
		{
			List<BirthTimePoint> result;
			if (string.IsNullOrEmpty(s))
			{
				result = null;
			}
			else
			{
				string[] array = s.Split(new char[]
				{
					'|'
				});
				if (array.Length <= 0)
				{
					result = null;
				}
				else
				{
					List<BirthTimePoint> list = new List<BirthTimePoint>();
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							string[] array2 = array[i].Split(new char[]
							{
								':'
							});
							if (array2.Length == 2)
							{
								string str = array2[0].TrimStart(new char[]
								{
									'0'
								});
								string str2 = array2[1].TrimStart(new char[]
								{
									'0'
								});
								BirthTimePoint item = new BirthTimePoint
								{
									BirthHour = Global.SafeConvertToInt32(str),
									BirthMinute = Global.SafeConvertToInt32(str2)
								};
								list.Add(item);
							}
						}
					}
					result = ((list.Count > 0) ? list : null);
				}
			}
			return result;
		}

		public void AddMapMonsters(int mapCode, GameMap gameMap)
		{
			this.AddDynamicMonsterZone(mapCode);
			string text = string.Format("Map/{0}/Monsters.xml", mapCode);
			XElement xelement = null;
			try
			{
				xelement = XElement.Load(Global.ResPath(text));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载地图怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements("Monsters").Elements<XElement>();
			if (null != enumerable)
			{
				bool flag = FuBenManager.IsFuBenMap(mapCode);
				foreach (XElement xelement2 in enumerable)
				{
					string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "TimePoints");
					int num = (int)Global.GetSafeAttributeLong(xelement2, "BirthType");
					int num2 = num;
					string s = safeAttributeStr;
					int spawnMonstersAfterKaiFuDays = 0;
					int spawnMonstersDays = 0;
					List<BirthTimeForDayOfWeek> list = new List<BirthTimeForDayOfWeek>();
					List<BirthTimePoint> birthTimePointList = null;
					if (4 == num || 5 == num || 6 == num)
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							';'
						});
						if (4 != array.Length)
						{
							throw new Exception(string.Format("地图{0}的类型4的刷怪配置参数个数不对!!!!", mapCode));
						}
						spawnMonstersAfterKaiFuDays = int.Parse(array[0]);
						spawnMonstersDays = int.Parse(array[1]);
						num2 = int.Parse(array[2]);
						s = array[3];
						if (1 != num2 && 0 != num2)
						{
							throw new Exception(string.Format("地图{0}的类型4的刷怪配置子类型不对!!!!", mapCode));
						}
					}
					if (7 == num)
					{
						string[] array2 = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array2.Length > 0)
						{
							int i = 0;
							while (i < array2.Length)
							{
								string text2 = array2[i];
								if (text2 != null)
								{
									string[] array3 = text2.Split(new char[]
									{
										','
									});
									if (array3 != null && array3.Length == 2)
									{
										int num3 = int.Parse(array3[0]);
										string text3 = array3[1];
										if (num3 != -1 && !string.IsNullOrEmpty(text3))
										{
											string[] array4 = text3.Split(new char[]
											{
												':'
											});
											if (array4.Length == 2)
											{
												string str = array4[0].TrimStart(new char[]
												{
													'0'
												});
												string str2 = array4[1].TrimStart(new char[]
												{
													'0'
												});
												BirthTimePoint birthTime = new BirthTimePoint
												{
													BirthHour = Global.SafeConvertToInt32(str),
													BirthMinute = Global.SafeConvertToInt32(str2)
												};
												list.Add(new BirthTimeForDayOfWeek
												{
													BirthDayOfWeek = num3,
													BirthTime = birthTime
												});
											}
										}
									}
								}
								IL_2E5:
								i++;
								continue;
								goto IL_2E5;
							}
						}
					}
					else
					{
						birthTimePointList = this.ParseBirthTimePoints(s);
					}
					MonsterZone monsterZone = new MonsterZone
					{
						MapCode = mapCode,
						ID = (int)Global.GetSafeAttributeLong(xelement2, "ID"),
						Code = (int)Global.GetSafeAttributeLong(xelement2, "Code"),
						ToX = (int)Global.GetSafeAttributeLong(xelement2, "X") / gameMap.MapGridWidth,
						ToY = (int)Global.GetSafeAttributeLong(xelement2, "Y") / gameMap.MapGridHeight,
						Radius = (int)Global.GetSafeAttributeLong(xelement2, "Radius") / gameMap.MapGridWidth,
						TotalNum = (int)Global.GetSafeAttributeLong(xelement2, "Num"),
						Timeslot = (int)Global.GetSafeAttributeLong(xelement2, "Timeslot"),
						IsFuBenMap = flag,
						BirthType = num2,
						ConfigBirthType = num,
						SpawnMonstersAfterKaiFuDays = spawnMonstersAfterKaiFuDays,
						SpawnMonstersDays = spawnMonstersDays,
						SpawnMonstersDayOfWeek = list,
						BirthTimePointList = birthTimePointList,
						BirthRate = (int)(Global.GetSafeAttributeDouble(xelement2, "BirthRate") * 10000.0)
					};
					XAttribute xattribute = xelement2.Attribute("PursuitRadius");
					if (null != xattribute)
					{
						monsterZone.PursuitRadius = (int)Global.GetSafeAttributeLong(xelement2, "PursuitRadius");
					}
					else
					{
						monsterZone.PursuitRadius = (int)Global.GetSafeAttributeLong(xelement2, "Radius");
					}
					lock (this.InitMonsterZoneMutex)
					{
						this.MonsterZoneList.Add(monsterZone);
						if (flag)
						{
							this.FuBenMonsterZoneList.Add(monsterZone);
						}
						this.AddMap2MonsterZoneDict(monsterZone);
					}
					monsterZone.LoadStaticMonsterInfo_2();
					monsterZone.LoadMonsters();
				}
			}
		}

		public void RunMapMonsters(SocketListener sl, TCPOutPacketPool pool)
		{
			for (int i = 0; i < this.MonsterZoneList.Count; i++)
			{
				this.MonsterZoneList[i].ReloadMonsters(sl, pool);
			}
			for (int i = 0; i < this.FuBenMonsterZoneList.Count; i++)
			{
				this.FuBenMonsterZoneList[i].DestroyDeadMonsters(true);
			}
			List<MonsterZone> list = this.MonsterDynamicZoneDict.Values.ToList<MonsterZone>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].DestroyDeadDynamicMonsters();
			}
		}

		public void RunMapDynamicMonsters(SocketListener sl, TCPOutPacketPool pool)
		{
			for (int i = 0; i < MonsterZoneManager.MaxRunQueueNum; i++)
			{
				if (!this.RunAddCopyMapMonsters())
				{
					break;
				}
			}
			for (int i = 0; i < MonsterZoneManager.MaxRunQueueNum; i++)
			{
				if (!this.RunDestroyCopyMapMonsters())
				{
					break;
				}
			}
			for (int i = 0; i < MonsterZoneManager.MaxRunQueueNum; i++)
			{
				if (!this.RunReloadCopyMapMonsters())
				{
					break;
				}
			}
			for (int i = 0; i < MonsterZoneManager.MaxRunQueueNum; i++)
			{
				if (!this.RunReloadNormalMapMonsters())
				{
					break;
				}
			}
			for (int i = 0; i < MonsterZoneManager.MaxRunAddDynamicMonstersQueueNum; i++)
			{
				if (!this.RunAddRobots())
				{
					break;
				}
			}
			int j = 0;
			int num = 0;
			while (j < MonsterZoneManager.MaxRunAddDynamicMonstersQueueNum)
			{
				for (int i = 0; i < 10; i++)
				{
					num++;
					if (this.RunAddDynamicMonsters(i))
					{
						j++;
					}
				}
				if (num >= MonsterZoneManager.MaxRunQueueNum)
				{
					break;
				}
			}
		}

		public int WaitingAddFuBenMonsterQueueCount()
		{
			int count;
			lock (this.WaitingAddFuBenMonsterQueue)
			{
				count = this.WaitingAddFuBenMonsterQueue.Count;
			}
			return count;
		}

		private bool RunAddCopyMapMonsters()
		{
			MonsterZoneQueueItem monsterZoneQueueItem = null;
			lock (this.WaitingAddFuBenMonsterQueue)
			{
				if (this.WaitingAddFuBenMonsterQueue.Count > 0)
				{
					monsterZoneQueueItem = this.WaitingAddFuBenMonsterQueue.Dequeue();
				}
			}
			bool result;
			if (null != monsterZoneQueueItem)
			{
				monsterZoneQueueItem.MyMonsterZone.LoadCopyMapMonsters(monsterZoneQueueItem.CopyMapID);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public int WaitingDestroyFuBenMonsterQueueCount()
		{
			int count;
			lock (this.WaitingDestroyFuBenMonsterQueue)
			{
				count = this.WaitingDestroyFuBenMonsterQueue.Count;
			}
			return count;
		}

		private bool RunDestroyCopyMapMonsters()
		{
			MonsterZoneQueueItem monsterZoneQueueItem = null;
			lock (this.WaitingDestroyFuBenMonsterQueue)
			{
				if (this.WaitingDestroyFuBenMonsterQueue.Count > 0)
				{
					monsterZoneQueueItem = this.WaitingDestroyFuBenMonsterQueue.Dequeue();
				}
			}
			bool result;
			if (null != monsterZoneQueueItem)
			{
				monsterZoneQueueItem.MyMonsterZone.ClearCopyMapMonsters(monsterZoneQueueItem.CopyMapID);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public int WaitingReloadFuBenMonsterQueueCount()
		{
			int count;
			lock (this.WaitingReloadFuBenMonsterQueue)
			{
				count = this.WaitingReloadFuBenMonsterQueue.Count;
			}
			return count;
		}

		private bool RunReloadCopyMapMonsters()
		{
			MonsterZoneQueueItem monsterZoneQueueItem = null;
			lock (this.WaitingReloadFuBenMonsterQueue)
			{
				if (this.WaitingReloadFuBenMonsterQueue.Count > 0)
				{
					monsterZoneQueueItem = this.WaitingReloadFuBenMonsterQueue.Dequeue();
				}
			}
			bool result;
			if (null != monsterZoneQueueItem)
			{
				monsterZoneQueueItem.MyMonsterZone.ReloadCopyMapMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monsterZoneQueueItem.CopyMapID);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void AddCopyMapMonsters(int mapCode, int copyMapID)
		{
			List<MonsterZone> list = null;
			if (this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					lock (this.WaitingAddFuBenMonsterQueue)
					{
						this.WaitingAddFuBenMonsterQueue.Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = 0,
							MyMonsterZone = list[i]
						});
					}
				}
			}
		}

		public void DestroyCopyMapMonsters(int mapCode, int copyMapID)
		{
			List<MonsterZone> list = null;
			if (this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					lock (this.WaitingDestroyFuBenMonsterQueue)
					{
						this.WaitingDestroyFuBenMonsterQueue.Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = 0,
							MyMonsterZone = list[i]
						});
					}
				}
			}
		}

		public void ReloadCopyMapMonsters(int mapCode, int copyMapID)
		{
			List<MonsterZone> list = null;
			if (this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					lock (this.WaitingReloadFuBenMonsterQueue)
					{
						this.WaitingReloadFuBenMonsterQueue.Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = 0,
							MyMonsterZone = list[i]
						});
					}
				}
			}
		}

		public int GetMapTotalMonsterNum(int mapCode, MonsterTypes monsterType, bool excludePets = true)
		{
			List<MonsterZone> list = null;
			int result;
			if (!this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				result = 0;
			}
			else
			{
				int num = 0;
				int i = 0;
				while (i < list.Count)
				{
					if (MonsterTypes.None == monsterType)
					{
						goto IL_47;
					}
					if (list[i].MonsterType == monsterType)
					{
						goto IL_47;
					}
					IL_76:
					i++;
					continue;
					IL_47:
					if (excludePets && list[i].IsDynamicZone())
					{
						goto IL_76;
					}
					num += list[i].TotalNum;
					goto IL_76;
				}
				result = num;
			}
			return result;
		}

		public int GetMapMonsterNum(int mapCode, int nMonsterID)
		{
			List<MonsterZone> list = null;
			int result;
			if (!this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					MonsterStaticInfo monsterInfo = list[i].GetMonsterInfo();
					if (monsterInfo != null)
					{
						if (monsterInfo.ExtensionID == nMonsterID)
						{
							num += list[i].TotalNum;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public bool GetMonsterBirthPoint(int mapCode, int nMonsterID, out int posX, out int posY, out int radis)
		{
			posX = 0;
			posY = 0;
			radis = 0;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				List<MonsterZone> list = null;
				if (!this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						MonsterZone monsterZone = list[i];
						if (monsterZone.Code == nMonsterID)
						{
							Point point = Global.GridToPixel(mapCode, (double)monsterZone.ToX, (double)monsterZone.ToY);
							posX = (int)point.X;
							posY = (int)point.Y;
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		private bool RunReloadNormalMapMonsters()
		{
			MonsterZoneQueueItem monsterZoneQueueItem = null;
			lock (this.WaitingReloadNormalMapMonsterQueue)
			{
				if (this.WaitingReloadNormalMapMonsterQueue.Count > 0)
				{
					monsterZoneQueueItem = this.WaitingReloadNormalMapMonsterQueue.Dequeue();
				}
			}
			bool result;
			if (null != monsterZoneQueueItem)
			{
				monsterZoneQueueItem.MyMonsterZone.ReloadNormalMapMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monsterZoneQueueItem.BirthCount);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void ReloadNormalMapMonsters(int mapCode, int birthCount)
		{
			List<MonsterZone> list = null;
			if (this.Map2MonsterZoneDict.TryGetValue(mapCode, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					lock (this.WaitingReloadNormalMapMonsterQueue)
					{
						this.WaitingReloadNormalMapMonsterQueue.Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = -1,
							BirthCount = birthCount,
							MyMonsterZone = list[i]
						});
					}
				}
			}
		}

		public MonsterZone GetDynamicMonsterZone(int mapCode)
		{
			MonsterZone monsterZone = null;
			MonsterZone result;
			if (this.MonsterDynamicZoneDict.TryGetValue(mapCode, out monsterZone))
			{
				result = monsterZone;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void AddDynamicMonsterZone(int mapCode)
		{
			bool flag = FuBenManager.IsFuBenMap(mapCode);
			MonsterZone monsterZone = new MonsterZone
			{
				MapCode = mapCode,
				ID = this.MonsterDynamicZoneDict.Count + 10000,
				Code = -1,
				ToX = -1,
				ToY = -1,
				Radius = 300,
				TotalNum = 0,
				Timeslot = 1,
				IsFuBenMap = flag,
				BirthType = 3,
				ConfigBirthType = -1,
				BirthTimePointList = null,
				BirthRate = 10000
			};
			monsterZone.PursuitRadius = 0;
			lock (this.InitMonsterZoneMutex)
			{
				this.MonsterDynamicZoneDict.Add(mapCode, monsterZone);
				this.MonsterZoneList.Add(monsterZone);
				if (flag)
				{
					this.FuBenMonsterZoneList.Add(monsterZone);
				}
				this.AddMap2MonsterZoneDict(monsterZone);
			}
		}

		private void InitDynamicMonsterSeedByMonserID(int monsterID)
		{
			MonsterZone monsterZone = new MonsterZone();
			Monster monster = null;
			if (!this._DictDynamicMonsterSeed.TryGetValue(monsterID, out monster) || null == monster)
			{
				int id = 1;
				lock (this._DictDynamicMonsterSeed)
				{
					id = this._DictDynamicMonsterSeed.Count + 1;
				}
				monsterZone.MapCode = 1;
				monsterZone.ID = id;
				monsterZone.Code = monsterID;
				monsterZone.LoadStaticMonsterInfo();
				monster = monsterZone.LoadDynamicMonsterSeed();
				lock (this._DictDynamicMonsterSeed)
				{
					if (!this._DictDynamicMonsterSeed.ContainsKey(monsterID))
					{
						this._DictDynamicMonsterSeed.Add(monsterID, monster);
					}
				}
			}
		}

		public Monster GetDynamicMonsterSeed(int monsterID)
		{
			Monster result = null;
			lock (this._DictDynamicMonsterSeed)
			{
				if (this._DictDynamicMonsterSeed.TryGetValue(monsterID, out result))
				{
					return result;
				}
			}
			try
			{
				this.InitDynamicMonsterSeedByMonserID(monsterID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "InitDynamicMonsterSeed()", false, false);
			}
			lock (this._DictDynamicMonsterSeed)
			{
				this._DictDynamicMonsterSeed.TryGetValue(monsterID, out result);
			}
			return result;
		}

		public Monster GetMonsterByMonsterID(int monsterID)
		{
			return this.GetDynamicMonsterSeed(monsterID);
		}

		public void AddDynamicRobot(int mapCode, Robot robot, int copyMapID = -1, int addNum = 1, int gridX = 0, int gridY = 0, int radius = 3, int pursuitRadius = 0, SceneUIClasses managerType = 0, object tag = null)
		{
			this.TraceAllDynamicMonsters();
			MonsterZone monsterZone = null;
			if (this.MonsterDynamicZoneDict.TryGetValue(mapCode, out monsterZone))
			{
				robot.MonsterZoneNode = monsterZone;
				lock (this.WaitingReloadRobotQueue)
				{
					this.WaitingReloadRobotQueue.Enqueue(new MonsterZoneQueueItem
					{
						CopyMapID = copyMapID,
						BirthCount = addNum,
						MyMonsterZone = monsterZone,
						seedMonster = robot,
						ToX = gridX,
						ToY = gridY,
						Radius = radius,
						PursuitRadius = pursuitRadius,
						Tag = tag,
						ManagerType = managerType
					});
				}
			}
		}

		public Monster AddDynamicMonsters(int mapCode, int monsterID, int copyMapID = -1, int addNum = 1, int gridX = 0, int gridY = 0, int radius = 3, int pursuitRadius = 0, SceneUIClasses managerType = 0, object tag = null, MonsterFlags flags = null)
		{
			this.TraceAllDynamicMonsters();
			MonsterZone myMonsterZone = null;
			Monster result;
			if (!this.MonsterDynamicZoneDict.TryGetValue(mapCode, out myMonsterZone))
			{
				result = null;
			}
			else
			{
				Monster dynamicMonsterSeed = this.GetDynamicMonsterSeed(monsterID);
				if (null == dynamicMonsterSeed)
				{
					result = null;
				}
				else
				{
					int num;
					if (copyMapID >= 0)
					{
						num = Global.Clamp(copyMapID % 10, 0, 9);
					}
					else
					{
						num = Global.Clamp(mapCode % 10, 0, 9);
					}
					lock (this.WaitingAddDynamicMonsterQueue)
					{
						this.WaitingAddDynamicMonsterQueue[num].Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = addNum,
							MyMonsterZone = myMonsterZone,
							seedMonster = dynamicMonsterSeed,
							ToX = gridX,
							ToY = gridY,
							Radius = radius,
							PursuitRadius = pursuitRadius,
							Tag = tag,
							ManagerType = managerType,
							Flags = flags
						});
					}
					result = dynamicMonsterSeed;
				}
			}
			return result;
		}

		public bool CallDynamicMonstersOwnedByRole(GameClient client, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1, int pursuitRadius = 0)
		{
			this.TraceAllDynamicMonsters();
			int mapCode = client.ClientData.MapCode;
			int copyMapID = client.ClientData.CopyMapID;
			Point currentGrid = client.CurrentGrid;
			int toX = (int)currentGrid.X;
			int toY = (int)currentGrid.Y;
			int radius = 3;
			MonsterZone myMonsterZone = null;
			bool result;
			if (!this.MonsterDynamicZoneDict.TryGetValue(mapCode, out myMonsterZone))
			{
				result = false;
			}
			else
			{
				Monster dynamicMonsterSeed = this.GetDynamicMonsterSeed(monsterID);
				if (null == dynamicMonsterSeed)
				{
					result = false;
				}
				else
				{
					Monster monster = dynamicMonsterSeed.Clone();
					monster.MonsterInfo = monster.MonsterInfo.Clone();
					monster.MonsterType = callAsType;
					monster.OwnerClient = client;
					if (callAsType == 1001)
					{
						Global.RecalcDSMonsterProps(client, monster, magicLevel, SurvivalTime);
					}
					int num;
					if (client.ClientData.CopyMapID >= 0)
					{
						num = Global.Clamp(client.ClientData.CopyMapID % 10, 0, 9);
					}
					else
					{
						num = Global.Clamp(client.ClientData.MapCode % 10, 0, 9);
					}
					lock (this.WaitingAddDynamicMonsterQueue)
					{
						this.WaitingAddDynamicMonsterQueue[num].Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = callNum,
							MyMonsterZone = myMonsterZone,
							seedMonster = monster,
							ToX = toX,
							ToY = toY,
							Radius = radius,
							PursuitRadius = pursuitRadius
						});
					}
					result = true;
				}
			}
			return result;
		}

		public bool CallDynamicMonstersOwnedByMonster(Monster owner, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1, int pursuitRadius = 0)
		{
			this.TraceAllDynamicMonsters();
			int currentMapCode = owner.CurrentMapCode;
			int copyMapID = owner.CopyMapID;
			Point currentGrid = owner.CurrentGrid;
			int toX = (int)currentGrid.X;
			int toY = (int)currentGrid.Y;
			int radius = 3;
			MonsterZone dynamicMonsterZone = this.GetDynamicMonsterZone(currentMapCode);
			bool result;
			if (null == dynamicMonsterZone)
			{
				result = false;
			}
			else
			{
				Monster dynamicMonsterSeed = this.GetDynamicMonsterSeed(monsterID);
				if (null == dynamicMonsterSeed)
				{
					result = false;
				}
				else
				{
					Monster monster = dynamicMonsterSeed.Clone();
					monster.MonsterInfo = monster.MonsterInfo.Clone();
					monster.MonsterType = callAsType;
					monster.OwnerMonster = owner;
					if (callAsType == 1001)
					{
						Global.RecalcDSMonsterProps(owner, monster, magicLevel, SurvivalTime);
					}
					int num;
					if (owner.CurrentCopyMapID >= 0)
					{
						num = Global.Clamp(owner.CurrentCopyMapID % 10, 0, 9);
					}
					else
					{
						num = Global.Clamp(owner.CurrentCopyMapID % 10, 0, 9);
					}
					lock (this.WaitingAddDynamicMonsterQueue)
					{
						this.WaitingAddDynamicMonsterQueue[num].Enqueue(new MonsterZoneQueueItem
						{
							CopyMapID = copyMapID,
							BirthCount = callNum,
							MyMonsterZone = dynamicMonsterZone,
							seedMonster = monster,
							ToX = toX,
							ToY = toY,
							Radius = radius,
							PursuitRadius = pursuitRadius
						});
					}
					result = true;
				}
			}
			return result;
		}

		private bool RunAddDynamicMonsters(int index)
		{
			bool result;
			if (index < 0 || index >= 10)
			{
				result = false;
			}
			else
			{
				MonsterZoneQueueItem monsterZoneQueueItem = null;
				lock (this.WaitingAddDynamicMonsterQueue)
				{
					if (this.WaitingAddDynamicMonsterQueue[index].Count > 0)
					{
						monsterZoneQueueItem = this.WaitingAddDynamicMonsterQueue[index].Dequeue();
					}
				}
				if (null != monsterZoneQueueItem)
				{
					monsterZoneQueueItem.MyMonsterZone.LoadDynamicMonsters(monsterZoneQueueItem);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool RunAddRobots()
		{
			MonsterZoneQueueItem monsterZoneQueueItem = null;
			lock (this.WaitingReloadRobotQueue)
			{
				if (this.WaitingReloadRobotQueue.Count > 0)
				{
					monsterZoneQueueItem = this.WaitingReloadRobotQueue.Dequeue();
				}
			}
			bool result;
			if (null != monsterZoneQueueItem)
			{
				monsterZoneQueueItem.MyMonsterZone.LoadDynamicRobot(monsterZoneQueueItem);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected void TraceAllDynamicMonsters()
		{
		}

		public List<MonsterZone> GetMonsterZoneListByMapCode(int mapCode)
		{
			List<MonsterZone> result = null;
			this.Map2MonsterZoneDict.TryGetValue(mapCode, out result);
			return result;
		}

		public List<MonsterZone> GetMonsterZoneByMapCodeAndMonsterID(int mapCode, int monsterID)
		{
			List<MonsterZone> list = new List<MonsterZone>();
			List<MonsterZone> monsterZoneListByMapCode = this.GetMonsterZoneListByMapCode(mapCode);
			List<MonsterZone> result;
			if (null == monsterZoneListByMapCode)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < monsterZoneListByMapCode.Count; i++)
				{
					if (monsterID == monsterZoneListByMapCode[i].Code)
					{
						list.Add(monsterZoneListByMapCode[i]);
					}
				}
				result = list;
			}
			return result;
		}

		public Point GetMonsterPointByMapCodeAndMonsterID(int mapCode, int monsterID)
		{
			Point point = new Point(-1.0, -1.0);
			List<MonsterZone> monsterZoneByMapCodeAndMonsterID = this.GetMonsterZoneByMapCodeAndMonsterID(mapCode, monsterID);
			Point result;
			if (monsterZoneByMapCodeAndMonsterID == null || monsterZoneByMapCodeAndMonsterID.Count <= 0)
			{
				result = point;
			}
			else
			{
				List<Point> list = new List<Point>();
				for (int i = 0; i < monsterZoneByMapCodeAndMonsterID.Count; i++)
				{
					Point mapPointByGridXY = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, monsterZoneByMapCodeAndMonsterID[i].MapCode, monsterZoneByMapCodeAndMonsterID[i].ToX, monsterZoneByMapCodeAndMonsterID[i].ToY, monsterZoneByMapCodeAndMonsterID[i].Radius, 0, false);
					list.Add(mapPointByGridXY);
				}
				if (list.Count <= 0)
				{
					result = new Point(-1.0, -1.0);
				}
				else
				{
					result = list[Global.GetRandomNumber(0, list.Count)];
				}
			}
			return result;
		}

		private const int Max_WaitingAddDynamicMonsterQueneCount = 10;

		public static int MaxRunQueueNum = 100;

		public static int MaxWaitingRunQueueNum = 200;

		public static int MaxRunAddDynamicMonstersQueueNum = 30;

		private Dictionary<int, MonsterZone> MonsterDynamicZoneDict = new Dictionary<int, MonsterZone>(100);

		private List<MonsterZone> MonsterZoneList = new List<MonsterZone>(100);

		private List<MonsterZone> FuBenMonsterZoneList = new List<MonsterZone>(100);

		private Dictionary<int, List<MonsterZone>> Map2MonsterZoneDict = new Dictionary<int, List<MonsterZone>>(100);

		private Queue<MonsterZoneQueueItem> WaitingAddFuBenMonsterQueue = new Queue<MonsterZoneQueueItem>();

		private Queue<MonsterZoneQueueItem> WaitingDestroyFuBenMonsterQueue = new Queue<MonsterZoneQueueItem>();

		private Queue<MonsterZoneQueueItem> WaitingReloadFuBenMonsterQueue = new Queue<MonsterZoneQueueItem>();

		private Queue<MonsterZoneQueueItem> WaitingReloadNormalMapMonsterQueue = new Queue<MonsterZoneQueueItem>();

		private Queue<MonsterZoneQueueItem>[] WaitingAddDynamicMonsterQueue = new Queue<MonsterZoneQueueItem>[10];

		private Queue<MonsterZoneQueueItem> WaitingReloadRobotQueue = new Queue<MonsterZoneQueueItem>();

		private Dictionary<int, Monster> _DictDynamicMonsterSeed = new Dictionary<int, Monster>();

		private XElement _allMonstersXml = null;

		private object _allMonsterXmlMutex = new object();

		private object InitMonsterZoneMutex = new object();
	}
}
