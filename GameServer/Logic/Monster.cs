using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.BossAI;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.NewBufferExt;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class Monster : IObject
	{
		public Monster Clone()
		{
			Monster monster = new Monster();
			monster.Name = this.Name;
			monster.MonsterZoneNode = this.MonsterZoneNode;
			monster.MonsterInfo = this.MonsterInfo;
			if (null == monster.MonsterInfo)
			{
				monster.MonsterInfo = this.MonsterZoneNode.GetMonsterInfo();
			}
			if (monster.MonsterInfo != null && monster.MonsterInfo.ExtProps != null)
			{
				Array.Copy(monster.MonsterInfo.ExtProps, monster.DynamicData.ExtProps, 177);
			}
			monster.Camp = monster.MonsterInfo.Camp;
			monster.RoleID = this.RoleID;
			monster.VLife = this.VLife;
			monster.VMana = this.VMana;
			monster.AttackRange = this.AttackRange;
			monster.Direction = this.Direction;
			monster.MoveSpeed = this.MoveSpeed;
			monster.MonsterType = this.MonsterType;
			monster.NextSeekEnemyTicks = this.NextSeekEnemyTicks;
			monster.OwnerClient = this.OwnerClient;
			monster.OwnerMonster = this.OwnerMonster;
			monster.CurrentMagicLevel = this.CurrentMagicLevel;
			monster.SurvivalTime = this.SurvivalTime;
			monster.SurvivalTick = this.SurvivalTick;
			Monster.IncMonsterCount();
			return monster;
		}

		public MonsterData GetMonsterData()
		{
			MonsterData monsterData = new MonsterData();
			monsterData.RoleID = this.RoleID;
			if (null != this.OwnerClient)
			{
				monsterData.RoleName = string.Format(GLang.GetLang(674, new object[0]), Global.FormatRoleName4(this.OwnerClient), this.MonsterInfo.VSName);
			}
			else if (!string.IsNullOrEmpty(this.MonsterName))
			{
				monsterData.RoleName = this.MonsterName;
			}
			else
			{
				monsterData.RoleName = this.MonsterInfo.VSName;
			}
			monsterData.ExtensionID = this.MonsterInfo.ExtensionID;
			monsterData.Level = this.MonsterInfo.VLevel;
			monsterData.Experience = this.MonsterInfo.VExperience;
			monsterData.MaxLifeV = this.MonsterInfo.VLifeMax;
			monsterData.MaxMagicV = this.MonsterInfo.VManaMax;
			monsterData.EquipmentBody = this.MonsterInfo.EquipmentBody;
			monsterData.MonsterType = this.MonsterType;
			monsterData.BattleWitchSide = this.Camp;
			CompResourcesConfig compResourcesConfig = this.Tag as CompResourcesConfig;
			if (compResourcesConfig != null && compResourcesConfig.ResourceState == 1)
			{
				monsterData.BirthTicks = this.LastMonsterLivingTicks;
			}
			if (null != this.OwnerClient)
			{
				monsterData.MasterRoleID = this.OwnerClient.ClientData.RoleID;
			}
			monsterData.PosX = (int)this.SafeCoordinate.X;
			monsterData.PosY = (int)this.SafeCoordinate.Y;
			monsterData.RoleDirection = (int)this.SafeDirection;
			monsterData.LifeV = this.VLife;
			monsterData.MagicV = this.VMana;
			monsterData.AiControlType = (ushort)this.PetAiControlType;
			monsterData.MonsterLevel = this.MonsterInfo.VLevel;
			BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(this, 42);
			if (null != monsterBufferDataByID)
			{
				monsterData.ZhongDuStart = monsterBufferDataByID.StartTime;
				monsterData.ZhongDuSeconds = monsterBufferDataByID.BufferSecs;
			}
			else
			{
				monsterData.ZhongDuStart = 0L;
				monsterData.ZhongDuSeconds = 0;
			}
			if (this.IsMonsterDongJie())
			{
				monsterData.FaintStart = this.DongJieStart;
				monsterData.FaintSeconds = this.DongJieSeconds;
			}
			return monsterData;
		}

		public Point Realive()
		{
			this.UniqueID = Global.GetUniqueID();
			if (401 == this.MonsterType)
			{
				CompManager.getInstance().OnProcessBossRealive(this);
			}
			this.TimedActionMgr.RemoveItem(0);
			this._LastDeadTicks = 0L;
			this.HandledDead = false;
			this.VLife = this.MonsterInfo.VLifeMax;
			this.VMana = this.MonsterInfo.VManaMax;
			this.Action = GActions.Stand;
			this.DongJieStart = 0L;
			this.DongJieSeconds = 0;
			this.TempPropsBuffer.Init();
			this.WhoKillMeID = 0;
			this.WhoKillMeName = "";
			this.IsCollected = false;
			this.isDeath = false;
			this.deathDelay = 0;
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Clear();
			}
			this.Start();
			Point point;
			if (501 == this.MonsterType || 501 == this.MonsterType)
			{
				point = Global.GetRandomPoint(ObjectTypes.OT_MONSTER, this.MonsterZoneNode.MapCode);
			}
			else
			{
				point = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MonsterZoneNode.MapCode, this.MonsterZoneNode.ToX, this.MonsterZoneNode.ToY, this.MonsterZoneNode.Radius, 0, true);
			}
			this.Coordinate = point;
			this.Direction = (double)Global.GetRandomNumber(0, 8);
			GameManager.SystemServerEvents.AddEvent(string.Format("怪物复活, roleID={0}", this.RoleID), EventLevels.Debug);
			return point;
		}

		public void OnDead()
		{
			this.MyMagicsManyTimeDmageQueue.Clear();
			this._CurrentMagic = -1;
			this._MagicFinish = 0;
			this.ClearDynSkill();
			Global.RemoveMonsterBufferData(this, 42);
			this.DestPoint = new Point(-1.0, -1.0);
			Global.RemoveStoryboard(this.Name);
			GameManager.MapGridMgr.DictGrids[this.MonsterZoneNode.MapCode].RemoveObject(this);
			this.Action = GActions.Death;
			this._LastDeadTicks = TimeUtil.NOW() * 10000L;
			this.ClearBossAI();
			this.Alive = false;
			this.OnReallyDied();
		}

		public MonsterZone MonsterZoneNode { get; set; }

		public MonsterStaticInfo MonsterInfo { get; set; }

		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_MONSTER;
			}
		}

		public int GetObjectID()
		{
			return this.RoleID;
		}

		public long LastLifeMagicTick { get; set; }

		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				return new Point((double)((int)(this.Coordinate.X / (double)gameMap.MapGridWidth)), (double)((int)(this.Coordinate.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				this.Coordinate = new Point(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			}
		}

		public Point CurrentPos
		{
			get
			{
				return this.Coordinate;
			}
			set
			{
				this.Coordinate = value;
			}
		}

		public int CurrentMapCode
		{
			get
			{
				return this.MonsterZoneNode.MapCode;
			}
		}

		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
			set
			{
				this.CopyMapID = value;
			}
		}

		public Dircetions CurrentDir
		{
			get
			{
				return (Dircetions)this.Direction;
			}
			set
			{
				this.Direction = (double)value;
			}
		}

		public List<int> PassiveEffectList
		{
			get
			{
				return new List<int>();
			}
			set
			{
			}
		}

		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			T result;
			if (type != ExtComponentTypes.ManyTimeDamageQueue)
			{
				result = default(T);
			}
			else
			{
				result = (this.MyMagicsManyTimeDmageQueue as T);
			}
			return result;
		}

		public List<VisibleItem> VisibleItemList
		{
			get
			{
				List<VisibleItem> visibleItemList;
				lock (this)
				{
					visibleItemList = this._VisibleItemList;
				}
				return visibleItemList;
			}
			set
			{
				lock (this)
				{
					this._VisibleItemList = value;
				}
			}
		}

		private bool CheckAttackerListEfficiency()
		{
			bool result;
			if (TimeUtil.NOW() - this._LastLogAttackerTicks > 30000L)
			{
				lock (this._AttackerLogDict)
				{
					this._AttackerLogDict.Clear();
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void AddAttacker(GameClient client, int injured, Monster DSPet = null)
		{
			if (client != null)
			{
				int roleID = client.ClientData.RoleID;
				lock (this._AttackerLogDict)
				{
					this._LastLogAttackerTicks = TimeUtil.NOW();
					MonsterAttackerLog monsterAttackerLog = null;
					if (!this._AttackerLogDict.TryGetValue(roleID, out monsterAttackerLog))
					{
						monsterAttackerLog = new MonsterAttackerLog();
						monsterAttackerLog.RoleId = roleID;
						monsterAttackerLog.RoleName = client.ClientData.RoleName;
						monsterAttackerLog.Occupation = Global.CalcOriginalOccupationID(client);
						monsterAttackerLog.FirstAttackMs = this._LastLogAttackerTicks;
						monsterAttackerLog.FirstAttack_MaxAttckV = RoleAlgorithm.GetMaxAttackV(client);
						monsterAttackerLog.FirstAttack_MaxMAttackV = RoleAlgorithm.GetMaxMagicAttackV(client);
						monsterAttackerLog.ZhanLi = (long)client.ClientData.CombatForce;
						monsterAttackerLog.VipExp = client.ClientData.VipExp;
						this._AttackerLogDict[roleID] = monsterAttackerLog;
					}
					monsterAttackerLog.MaxAttackV = Math.Max(monsterAttackerLog.MaxAttackV, RoleAlgorithm.GetMaxAttackV(client));
					monsterAttackerLog.MaxMAttackV = Math.Max(monsterAttackerLog.MaxMAttackV, RoleAlgorithm.GetMaxMagicAttackV(client));
					monsterAttackerLog.LastAttackMs = this._LastLogAttackerTicks;
					if (null == DSPet)
					{
						monsterAttackerLog.TotalInjured += (long)injured;
						monsterAttackerLog.InjureTimes++;
					}
					else
					{
						monsterAttackerLog.TotalInjuredByPet += (long)injured;
						monsterAttackerLog.InjureTimesByPet += 1L;
					}
				}
			}
		}

		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Remove(roleID);
			}
		}

		public bool IsAttackedBy(int roleID)
		{
			this.CheckAttackerListEfficiency();
			lock (this._AttackerLogDict)
			{
				if (this._AttackerLogDict.ContainsKey(roleID))
				{
					return true;
				}
			}
			return false;
		}

		public int GetAttackerFromList()
		{
			this.CheckAttackerListEfficiency();
			int result = -1;
			long num = 0L;
			long num2 = TimeUtil.NOW();
			lock (this._AttackerLogDict)
			{
				foreach (MonsterAttackerLog monsterAttackerLog in this._AttackerLogDict.Values)
				{
					if (num2 - monsterAttackerLog.LastAttackMs < 30000L)
					{
						if (monsterAttackerLog.TotalInjured > num || monsterAttackerLog.TotalInjuredByPet > num)
						{
							num = ((monsterAttackerLog.TotalInjured > monsterAttackerLog.TotalInjuredByPet) ? monsterAttackerLog.TotalInjured : monsterAttackerLog.TotalInjuredByPet);
							result = monsterAttackerLog.RoleId;
						}
					}
				}
			}
			return result;
		}

		public List<long> GetAttackerDamageList(List<int> ridList)
		{
			List<long> list = new List<long>();
			if (null != ridList)
			{
				lock (this._AttackerLogDict)
				{
					foreach (int key in ridList)
					{
						long item = 0L;
						MonsterAttackerLog monsterAttackerLog;
						if (this._AttackerLogDict.TryGetValue(key, out monsterAttackerLog))
						{
							item = ((monsterAttackerLog.TotalInjured > monsterAttackerLog.TotalInjuredByPet) ? monsterAttackerLog.TotalInjured : monsterAttackerLog.TotalInjuredByPet);
						}
						list.Add(item);
					}
				}
			}
			return list;
		}

		public long GetAttackerDamage(int rid)
		{
			long result = 0L;
			lock (this._AttackerLogDict)
			{
				MonsterAttackerLog monsterAttackerLog;
				if (this._AttackerLogDict.TryGetValue(rid, out monsterAttackerLog))
				{
					result = ((monsterAttackerLog.TotalInjured > monsterAttackerLog.TotalInjuredByPet) ? monsterAttackerLog.TotalInjured : monsterAttackerLog.TotalInjuredByPet);
				}
			}
			return result;
		}

		public List<int> GetAttackerList()
		{
			List<int> list = new List<int>();
			long num = TimeUtil.NOW();
			lock (this._AttackerLogDict)
			{
				foreach (MonsterAttackerLog monsterAttackerLog in this._AttackerLogDict.Values)
				{
					if (num - monsterAttackerLog.LastAttackMs < 15000L)
					{
						list.Add(monsterAttackerLog.RoleId);
					}
				}
			}
			return list;
		}

		public string BuildAttackerLog()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("怪物伤害日志: MonsterId={0}, MonsterName={1},", this.MonsterInfo.ExtensionID, Global.GetMonsterNameByID(this.MonsterInfo.ExtensionID));
			lock (this._AttackerLogDict)
			{
				stringBuilder.AppendFormat("共有{0}个攻击者: ", this._AttackerLogDict.Count<KeyValuePair<int, MonsterAttackerLog>>());
				stringBuilder.AppendLine();
				foreach (MonsterAttackerLog monsterAttackerLog in this._AttackerLogDict.Values)
				{
					stringBuilder.Append("     ");
					stringBuilder.AppendFormat("roleid={0}, ", monsterAttackerLog.RoleId);
					stringBuilder.AppendFormat("rolename={0}, ", monsterAttackerLog.RoleName);
					stringBuilder.AppendFormat("职业={0}, ", Global.GetOccupationStr(monsterAttackerLog.Occupation));
					stringBuilder.AppendFormat("总计伤害={0}, ", monsterAttackerLog.TotalInjured);
					stringBuilder.AppendFormat("总计伤害Pet={0},", monsterAttackerLog.TotalInjuredByPet);
					long num = monsterAttackerLog.LastAttackMs - monsterAttackerLog.FirstAttackMs;
					stringBuilder.AppendFormat("共用时={0}ms, ", num);
					stringBuilder.AppendFormat("伤害次数={0}, ", monsterAttackerLog.InjureTimes);
					stringBuilder.AppendFormat("伤害次数Pet={0},", monsterAttackerLog.InjureTimesByPet);
					stringBuilder.AppendFormat("首次伤害物攻={0}, ", monsterAttackerLog.FirstAttack_MaxAttckV);
					stringBuilder.AppendFormat("首次伤害魔攻={0}, ", monsterAttackerLog.FirstAttack_MaxMAttackV);
					stringBuilder.AppendFormat("最大物攻={0}, ", monsterAttackerLog.MaxAttackV);
					stringBuilder.AppendFormat("最大魔攻={0}, ", monsterAttackerLog.MaxMAttackV);
					stringBuilder.AppendFormat("ZhanLi={0}, ", monsterAttackerLog.ZhanLi);
					stringBuilder.AppendFormat("VipExp={0}, ", monsterAttackerLog.VipExp);
					double num2 = Math.Max(monsterAttackerLog.MaxMAttackV, monsterAttackerLog.MaxAttackV);
					if (num2 > 0.0)
					{
						stringBuilder.AppendFormat("攻击系数={0}, ", (double)monsterAttackerLog.TotalInjured * 1.0 / (double)monsterAttackerLog.InjureTimes / num2);
					}
					else
					{
						stringBuilder.AppendFormat("攻击系数={0}[最大伤害无效]", "无效");
					}
					if (num > 0L)
					{
						stringBuilder.AppendFormat("攻速系数={0}", (double)monsterAttackerLog.InjureTimes * 1.0 / (double)num);
					}
					else
					{
						stringBuilder.AppendFormat("攻速系数={0}[总攻击时间无效]", "无效");
					}
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public long LastInObsJugeTicks { get; set; }

		public long LastSeekEnemyTicks { get; set; }

		public long NextSeekEnemyTicks { get; set; }

		public long LastLockEnemyTicks { get; set; }

		public long LockFocusTime { get; set; }

		public double MoveSpeed
		{
			get
			{
				return this._MoveSpeed;
			}
			set
			{
				this._MoveSpeed = value;
			}
		}

		public Point DestPoint
		{
			get
			{
				Point destPoint;
				lock (this)
				{
					destPoint = this._DestPoint;
				}
				return destPoint;
			}
			set
			{
				lock (this)
				{
					this._DestPoint = value;
				}
			}
		}

		public int CopyMapID
		{
			get
			{
				return this._CopyMapID;
			}
			set
			{
				this._CopyMapID = value;
			}
		}

		public bool HandledDead
		{
			get
			{
				return this._HandledDead;
			}
			set
			{
				this._HandledDead = value;
			}
		}

		public string Name { get; set; }

		public int RoleID { get; set; }

		public double VLife
		{
			get
			{
				return Thread.VolatileRead(ref this._VLife);
			}
			set
			{
				Thread.VolatileWrite(ref this._VLife, value);
			}
		}

		public double VMana
		{
			get
			{
				return Thread.VolatileRead(ref this._VMana);
			}
			set
			{
				Thread.VolatileWrite(ref this._VMana, value);
			}
		}

		public int MonsterType { get; set; }

		public int WhoKillMeID { get; set; }

		public string WhoKillMeName { get; set; }

		public void AddLife(long life)
		{
			if (this._VLife > 0.0 && this.Alive)
			{
				this._VLife = Math.Min(this.MonsterInfo.VLifeMax, this._VLife + (double)life);
			}
		}

		public int PetAiControlType
		{
			get
			{
				return this._PetAiControlType;
			}
			set
			{
				int petAiControlType = this._PetAiControlType;
				this._PetAiControlType = value;
				if (petAiControlType != this._PetAiControlType)
				{
					this._LockObject = -1;
				}
			}
		}

		public bool IsMonsterDongJie()
		{
			bool result;
			if (this.DongJieStart <= 0L)
			{
				result = false;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num < this.DongJieStart + (long)(this.DongJieSeconds * 1000));
			}
			return result;
		}

		public bool IsMonsterXuanYun()
		{
			bool result;
			if (this.XuanYunStart <= 0L)
			{
				result = false;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num < this.XuanYunStart + (long)(this.XuanYunSeconds * 1000));
			}
			return result;
		}

		public bool IsMonsterDingShen()
		{
			bool result;
			if (this.DingShenStart <= 0L)
			{
				result = false;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num < this.DingShenStart + (long)(this.DingShenSeconds * 1000));
			}
			return result;
		}

		public bool IsMonsterSpeedDown()
		{
			bool result;
			if (this.SpeedDownStart <= 0L)
			{
				result = false;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num < this.SpeedDownStart + (long)(this.SpeedDownSeconds * 1000));
			}
			return result;
		}

		public event MoveToEventHandler MoveToComplete;

		public long LastDeadTicks
		{
			get
			{
				return this._LastDeadTicks;
			}
		}

		public bool IsMoving
		{
			get
			{
				bool result;
				if (GActions.Walk != this._Action)
				{
					result = false;
				}
				else
				{
					long num = TimeUtil.NOW();
					result = (num - this._LastActionTick < (long)this.GetMovingNeedTick());
				}
				return result;
			}
		}

		public int GetMovingNeedTick()
		{
			double num = this.MoveSpeed;
			if (num < 0.05)
			{
				num = 0.05;
			}
			return this.IsMonsterSpeedDown() ? Convert.ToInt32((double)Global.MovingNeedTicksPerGrid / num) : Global.MovingNeedTicksPerGrid;
		}

		public event CoordinateEventHandler CoordinateChanged;

		public Point SafeCoordinate
		{
			get
			{
				Point safeCoordinate;
				lock (this)
				{
					safeCoordinate = this._SafeCoordinate;
				}
				return safeCoordinate;
			}
		}

		public Point SafeOldCoordinate
		{
			get
			{
				Point safeOldCoordinate;
				lock (this)
				{
					safeOldCoordinate = this._SafeOldCoordinate;
				}
				return safeOldCoordinate;
			}
		}

		public bool FirstStoryMove
		{
			get
			{
				return this._FirstStoryMove;
			}
			set
			{
				this._FirstStoryMove = value;
			}
		}

		public Point FirstCoordinate { get; set; }

		public Point getFirstGrid()
		{
			if (default(Point).Equals(this.firstGrid))
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				this.firstGrid = new Point((double)((int)(this.FirstCoordinate.X / (double)gameMap.MapGridWidth)), (double)((int)(this.FirstCoordinate.Y / (double)gameMap.MapGridHeight)));
			}
			return this.firstGrid;
		}

		public Point Coordinate
		{
			get
			{
				return this._Coordinate;
			}
			set
			{
				lock (this)
				{
					this._SafeOldCoordinate = value;
				}
				Point coordinate = this._Coordinate;
				this._Coordinate = new Point(value.X, value.Y);
				Monster.ChangeCoordinateProperty2(this, coordinate, this._Coordinate);
			}
		}

		private static void ChangeCoordinateProperty2(Monster obj, Point oldValue, Point newValue)
		{
			lock (obj)
			{
				obj._SafeOldCoordinate = oldValue;
				obj._SafeCoordinate = newValue;
			}
			if (obj.CoordinateChanged != null)
			{
				obj.CoordinateChanged(obj);
			}
		}

		public double SafeDirection
		{
			get
			{
				return Thread.VolatileRead(ref this._SafeDirection);
			}
		}

		public double Direction
		{
			get
			{
				return this._Direction;
			}
			set
			{
				lock (this)
				{
					this._SafeDirection = value;
				}
				double direction = this._Direction;
				this._Direction = value;
				if (this.Action == GActions.Attack || this.Action == GActions.Magic || this.Action == GActions.Bow)
				{
					if (this.FrameCounter < this.EndFrame)
					{
						return;
					}
				}
				Monster.ChangeDirectionProperty2(this, direction, this._Direction);
			}
		}

		private static void ChangeDirectionProperty2(Monster obj, double oldValue, double newValue)
		{
			lock (obj)
			{
				obj._SafeDirection = newValue;
			}
			obj.ChangeAction(false);
		}

		public int PetLockObjectPriority { get; set; }

		public int LockObject
		{
			get
			{
				int lockObject;
				lock (this)
				{
					lockObject = this._LockObject;
				}
				return lockObject;
			}
			set
			{
				lock (this)
				{
					this._LockObject = value;
				}
			}
		}

		public int AttackRange { get; set; }

		public long LastActionTick
		{
			get
			{
				return this._LastActionTick;
			}
		}

		public long LastAttackActionTick
		{
			get
			{
				return this._LastAttackActionTick;
			}
		}

		public GActions Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				if (this._Action != value)
				{
					if (value == GActions.Attack || value == GActions.Magic || value == GActions.Bow)
					{
						if (this._Action == GActions.PreAttack)
						{
							return;
						}
					}
					this._Action = value;
					if (this._Action == GActions.Run)
					{
						this._Action = GActions.Walk;
					}
					lock (this)
					{
						this._SafeAction = this._Action;
					}
					this._LastActionTick = TimeUtil.NOW();
					if (value == GActions.Attack || value == GActions.Magic || value == GActions.Bow)
					{
						if (this.FrameCounter < this.EndFrame)
						{
							return;
						}
					}
					if (this.CurrentMagic <= 0)
					{
						this.ChangeAction(true);
					}
				}
			}
		}

		public GActions SafeAction
		{
			get
			{
				GActions safeAction;
				lock (this)
				{
					safeAction = this._SafeAction;
				}
				return safeAction;
			}
		}

		public Point Destination { get; set; }

		public Point EnemyTarget { get; set; }

		public int FrameCounter { get; set; }

		public int StartFrame { get; set; }

		public int EndFrame { get; set; }

		public event SpriteChangeActionEventHandler SpriteChangeAction;

		private void BeginHeart()
		{
			this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
			this.LastMonsterLivingTicks = this.LastMonsterLivingSlotTicks;
			GlobalEventSource.getInstance().fireEvent(new MonsterBirthOnEventObject(this));
		}

		public void Start()
		{
			this.Alive = true;
			this.BeginHeart();
		}

		public void onSurvivalTick()
		{
			if (0 != this.SurvivalTime)
			{
				if (TimeUtil.NOW() >= this.SurvivalTick)
				{
					if (1001 != this.MonsterType)
					{
						Global.SystemKillMonster(this);
					}
					else
					{
						GameManager.MonsterMgr.DeadMonsterImmediately(this);
					}
				}
			}
		}

		public virtual void Timer_Tick(object sender, EventArgs e)
		{
			if (this.isDeath)
			{
				this.deathDelay++;
				if (this.deathDelay >= 10)
				{
				}
			}
			else
			{
				if (this.IsMonsterDongJie())
				{
					this.Action = GActions.Stand;
				}
				long num = TimeUtil.NOW();
				if (num - this.LastMonsterLivingSlotTicks >= 60000L)
				{
					this.LastMonsterLivingSlotTicks = num;
					GlobalEventSource.getInstance().fireEvent(new MonsterLivingTimeEventObject(this));
				}
				if (GActions.Walk == this.Action || GActions.Run == this.Action)
				{
					if (num - this._LastActionTick >= (long)this.GetMovingNeedTick())
					{
						if (null != this.MoveToComplete)
						{
							this.MoveToComplete(this);
						}
					}
				}
				else
				{
					double num2 = this.ChangeDirectionValue();
					int actionIndex = Global.GetActionIndex(this.Action);
					int num3 = this.MonsterInfo.EachActionFrameRange[actionIndex];
					if (GActions.PreAttack == this.Action)
					{
						actionIndex = Global.GetActionIndex(GActions.Stand);
						num3 = this.MonsterInfo.EachActionFrameRange[actionIndex];
					}
					int num4;
					if (this.Action == GActions.Death)
					{
						num4 = (int)(num2 * (double)num3) + (num3 - 1);
					}
					else
					{
						num4 = (int)(num2 * (double)num3) + this.MonsterInfo.EffectiveFrame[actionIndex];
					}
					if (this.FrameCounter == num4)
					{
						this.DoAction();
					}
					if (this.FrameCounter >= this.EndFrame && (this._Action == GActions.Attack || this._Action == GActions.Magic || this._Action == GActions.Bow))
					{
						SystemXmlItem systemXmlItem = null;
						lock (this.CurrentSkillIDIndexLock)
						{
							this._ToExecSkillID = -1;
							if (this._CurrentMagic > 0)
							{
								if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this._CurrentMagic, out systemXmlItem))
								{
									int intValue = systemXmlItem.GetIntValue("NextMagicID", -1);
									if (intValue > 0)
									{
										this._ToExecSkillID = intValue;
									}
								}
							}
							this._CurrentSkillIDIndex++;
						}
						this.OldAction = this._Action;
						this._Action = GActions.Stand;
						this._MaxAttackTimeSlot = 2000L;
						if (null == systemXmlItem)
						{
							GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this._CurrentMagic, out systemXmlItem);
						}
						if (null != systemXmlItem)
						{
							int intValue2 = systemXmlItem.GetIntValue("AttackInterval", -1);
							if (intValue2 > 0)
							{
								this._MaxAttackTimeSlot = (long)intValue2;
							}
						}
						this._LastAttackActionTick = TimeUtil.NOW();
						bool flag2 = false;
						try
						{
							Monster obj = this;
							Monitor.Enter(this, ref flag2);
							this._SafeAction = this._Action;
						}
						finally
						{
							if (flag2)
							{
								Monster obj;
								Monitor.Exit(obj);
							}
						}
						this.ChangeAction(false);
						this.FrameCounter = this.StartFrame;
					}
					else if (this.FrameCounter >= this.EndFrame && this._Action == GActions.PreAttack)
					{
						this._Action = this.OldAction;
						bool flag3 = false;
						try
						{
							Monster obj = this;
							Monitor.Enter(this, ref flag3);
							this._SafeAction = this._Action;
						}
						finally
						{
							if (flag3)
							{
								Monster obj;
								Monitor.Exit(obj);
							}
						}
						this.ChangeAction(false);
						this.FrameCounter = this.StartFrame;
					}
					else
					{
						this.FrameCounter = ((this.FrameCounter >= this.EndFrame) ? this.StartFrame : (this.FrameCounter + 1));
					}
				}
			}
		}

		private double ChangeDirectionValue()
		{
			double result;
			if (1601 == this.MonsterType)
			{
				result = 0.0;
			}
			else
			{
				result = this.Direction;
			}
			return result;
		}

		private void ChangeAction(bool resetCounter)
		{
			int actionIndex = Global.GetActionIndex(this.Action);
			int num = this.MonsterInfo.EachActionFrameRange[actionIndex];
			if (num <= 0)
			{
			}
			int num2 = (int)this.ChangeDirectionValue();
			if (this.Action == GActions.Death)
			{
			}
			if (1601 == this.MonsterType)
			{
				num2 = 0;
			}
			int num3 = Global.GetActionTick(this.Action, this.MonsterInfo.SpriteSpeedTickList);
			if (num3 <= 0)
			{
				num3 = 100;
			}
			if (GActions.Death == this.Action)
			{
				num3 *= 2;
			}
			if (GActions.PreAttack == this.Action)
			{
				int actionIndex2 = Global.GetActionIndex(GActions.Stand);
				num = this.MonsterInfo.EachActionFrameRange[actionIndex2];
				num3 = Global.GetActionTick(GActions.Stand, this.MonsterInfo.SpriteSpeedTickList);
			}
			switch (this.Action)
			{
			case GActions.Stand:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Walk:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Run:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Attack:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Injured:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Magic:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Bow:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Death:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.HorseStand:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.HorseRun:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.HorseDead:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.Sit:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			case GActions.PreAttack:
				this.RefreshThread((double)num3, num2 * num, (num2 + 1) * num - 1);
				break;
			}
			if (resetCounter)
			{
				this.FrameCounter = this.StartFrame;
			}
			else if (this.FrameCounter < this.StartFrame || this.StartFrame >= this.EndFrame)
			{
				this.FrameCounter = this.StartFrame;
			}
		}

		private void RefreshThread(double timeSpan, int startFrame, int endFrame)
		{
			this._HeartInterval = (long)timeSpan;
			this.StartFrame = startFrame;
			this.EndFrame = endFrame;
		}

		private void DoAction()
		{
			GActions action = this.Action;
			if (action != GActions.Attack)
			{
				if (action == GActions.Death)
				{
					this.isDeath = true;
				}
			}
			else if (this.LockObject == -1)
			{
				if (null != this.SpriteChangeAction)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = 0
					});
				}
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (this.CurrentMagic > 0)
				{
					if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this.CurrentMagic, out systemXmlItem))
					{
						return;
					}
					if (systemXmlItem.GetIntValue("InjureType", -1) == 1)
					{
						return;
					}
					if (this.MagicFinish == -2)
					{
						return;
					}
					if (this.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() > 0)
					{
						return;
					}
				}
				Global.DoInjure(this, this._LockObject, this.EnemyTarget);
			}
		}

		protected void OnReallyDied()
		{
			this.MonsterZoneNode.OnReallyDied(this);
		}

		public int CurrentMagic
		{
			get
			{
				return this._CurrentMagic;
			}
			set
			{
				this._CurrentMagic = value;
			}
		}

		public int CurrentMagicLevel
		{
			get
			{
				return this._CurrentMagicLevel;
			}
			set
			{
				this._CurrentMagicLevel = value;
			}
		}

		public int SurvivalTime
		{
			get
			{
				return this._SurvivalTime;
			}
			set
			{
				this._SurvivalTime = value;
			}
		}

		public long SurvivalTick
		{
			get
			{
				return this._SurvivalTick;
			}
			set
			{
				this._SurvivalTick = value;
			}
		}

		public int MagicFinish
		{
			get
			{
				return this._MagicFinish;
			}
			set
			{
				this._MagicFinish = value;
			}
		}

		public MagicCoolDownMgr MyMagicCoolDownMgr
		{
			get
			{
				return this._MagicCoolDownMgr;
			}
		}

		public long MaxAttackTimeSlot
		{
			get
			{
				return this._MaxAttackTimeSlot;
			}
			set
			{
				this._MaxAttackTimeSlot = value;
			}
		}

		public void AddDynSkillID(int skillID, int priority)
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				bool flag2 = false;
				for (int i = 0; i < this.DynSkillIDsList.Count; i++)
				{
					if (this.DynSkillIDsList[i].SkillID == skillID)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					this.DynSkillIDsList.Add(new DynSkillItem
					{
						SkillID = skillID,
						Priority = priority
					});
					this.DynSkillIDsList.Sort(delegate(DynSkillItem left, DynSkillItem right)
					{
						int result;
						if (left.Priority < right.Priority)
						{
							result = 1;
						}
						else if (left.Priority == right.Priority)
						{
							result = 0;
						}
						else
						{
							result = -1;
						}
						return result;
					});
				}
			}
		}

		public void RemoveDynSkill(int skillID)
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				for (int i = 0; i < this.DynSkillIDsList.Count; i++)
				{
					if (this.DynSkillIDsList[i].SkillID == skillID)
					{
						this.DynSkillIDsList.RemoveAt(i);
						break;
					}
				}
			}
		}

		private void ClearDynSkill()
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				this._ToExecSkillID = -1;
				this.DynSkillIDsList.Clear();
				this._CurrentSkillIDIndex = 0;
			}
		}

		public int GetAutoUseSkillID()
		{
			int num = -1;
			lock (this.CurrentSkillIDIndexLock)
			{
				if (this._ToExecSkillID > 0)
				{
					num = this._ToExecSkillID;
					return num;
				}
				if (this.DynSkillIDsList.Count > 0)
				{
					if (this._CurrentSkillIDIndex >= this.DynSkillIDsList.Count)
					{
						this._CurrentSkillIDIndex = 0;
					}
					for (int i = this._CurrentSkillIDIndex; i < this.DynSkillIDsList.Count; i++)
					{
						if (this.MyMagicCoolDownMgr.SkillCoolDown(this.DynSkillIDsList[i].SkillID))
						{
							if (this.SkillNeedMagicVOk(this, this.DynSkillIDsList[i].SkillID))
							{
								num = this.DynSkillIDsList[i].SkillID;
								break;
							}
						}
					}
				}
				if (num <= 0)
				{
					if (null != this.MonsterInfo.SkillIDs)
					{
						if (this._CurrentSkillIDIndex >= this.MonsterInfo.SkillIDs.Length)
						{
							this._CurrentSkillIDIndex = 0;
						}
						for (int i = this._CurrentSkillIDIndex; i < this.MonsterInfo.SkillIDs.Length; i++)
						{
							if (this.MyMagicCoolDownMgr.SkillCoolDown(this.MonsterInfo.SkillIDs[i]))
							{
								if (this.SkillNeedMagicVOk(this, this.MonsterInfo.SkillIDs[i]))
								{
									num = this.MonsterInfo.SkillIDs[i];
									break;
								}
							}
						}
					}
				}
			}
			return num;
		}

		protected bool SkillNeedMagicVOk(Monster monster, int skillID)
		{
			int needMagicV = Global.GetNeedMagicV(monster, skillID, 1);
			if (needMagicV > 0)
			{
				int num = (int)monster.MonsterInfo.VManaMax;
				int num2 = (int)((double)num * ((double)needMagicV / 100.0));
				num2 = Global.GMax(0, num2);
				if (monster.VMana - (double)num2 < 0.0)
				{
					return false;
				}
			}
			return true;
		}

		public long AddToDeadQueueTicks { get; set; }

		public static void IncMonsterCount()
		{
			lock (Monster.CountLock)
			{
				Monster.TotalMonsterCount++;
			}
		}

		public static void DecMonsterCount()
		{
			lock (Monster.CountLock)
			{
				Monster.TotalMonsterCount--;
			}
		}

		public static int GetMonsterCount()
		{
			int result = 0;
			lock (Monster.CountLock)
			{
				result = Monster.TotalMonsterCount;
			}
			return result;
		}

		public long GetMonsterLivingTicks()
		{
			return TimeUtil.NOW() - this.LastMonsterLivingTicks;
		}

		public long GetMonsterBirthTick()
		{
			return this.LastMonsterLivingTicks * 10000L;
		}

		public void ResetMonsterBirthTick()
		{
			this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
			this.LastMonsterLivingTicks = TimeUtil.NOW();
		}

		public bool CanExecBossAI(BossAIItem bossAIItem)
		{
			bool result;
			if (bossAIItem.TriggerNum <= 0 && bossAIItem.TriggerCD <= 0)
			{
				result = true;
			}
			else
			{
				int num = 0;
				long num2 = 0L;
				lock (this.TriggerMutex)
				{
					if (bossAIItem.TriggerNum > 0)
					{
						this.TriggerNumDict.TryGetValue(bossAIItem.ID, out num);
						if (num >= bossAIItem.TriggerNum)
						{
							return false;
						}
					}
					if (bossAIItem.TriggerCD > 0)
					{
						this.TriggerCDDict.TryGetValue(bossAIItem.ID, out num2);
						if (TimeUtil.NOW() - num2 < (long)(bossAIItem.TriggerCD * 1000))
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		public bool RecBossAI(BossAIItem bossAIItem)
		{
			int num = 0;
			lock (this.TriggerMutex)
			{
				if (bossAIItem.TriggerNum > 0)
				{
					if (!this.TriggerNumDict.TryGetValue(bossAIItem.ID, out num))
					{
						num = 0;
					}
					num++;
					this.TriggerNumDict[bossAIItem.ID] = num;
				}
				if (bossAIItem.TriggerCD > 0)
				{
					this.TriggerCDDict[bossAIItem.ID] = TimeUtil.NOW();
				}
			}
			return true;
		}

		public void ClearBossAI()
		{
			lock (this.TriggerMutex)
			{
				this.TriggerNumDict.Clear();
				this.TriggerCDDict.Clear();
			}
		}

		public bool IsCollected
		{
			get
			{
				return this._IsCollected;
			}
			set
			{
				this._IsCollected = value;
			}
		}

		public DynamicData DynamicData = new DynamicData();

		public bool Alive = false;

		public MonsterFlags Flags = new MonsterFlags();

		public bool AllwaySearchEnemy = false;

		private List<VisibleItem> _VisibleItemList = null;

		private long _LastLogAttackerTicks = 0L;

		private Dictionary<int, MonsterAttackerLog> _AttackerLogDict = new Dictionary<int, MonsterAttackerLog>();

		public long LastExecTimerTicks = 0L;

		private double _MoveSpeed = 1.0;

		private Point _DestPoint = new Point(-1.0, -1.0);

		private int _CopyMapID = -1;

		public bool _HandledDead = false;

		public int VisibleClientsNum = 0;

		public long UniqueID;

		public object Tag;

		public SceneUIClasses ManagerType = 0;

		public string MonsterName;

		public bool IsAttackRole = true;

		public bool IsAutoSearchRoad = false;

		private double _VLife = 0.0;

		private double _VMana = 0.0;

		public int Camp;

		public GameClient OwnerClient = null;

		public Monster OwnerMonster = null;

		public Monster CallMonster = null;

		public int _PetAiControlType = 2;

		public Dictionary<int, BufferData> BufferDataDict = null;

		public long DSStartDSAddLifeNoShowTicks = 0L;

		public long DSStartDSSubLifeNoShowTicks = 0L;

		public int FangDuRoleID = 0;

		public long DongJieStart = 0L;

		public int DongJieSeconds = 0;

		public long XuanYunStart = 0L;

		public int XuanYunSeconds = 0;

		public long DingShenStart = 0L;

		public int DingShenSeconds = 0;

		public long SpeedDownStart = 0L;

		public int SpeedDownSeconds = 0;

		private long _LastDeadTicks = 0L;

		private Point _SafeCoordinate;

		private Point _SafeOldCoordinate = new Point(0.0, 0.0);

		private bool _FirstStoryMove = false;

		public int SubMapCode = -1;

		private Point firstGrid = default(Point);

		public bool isReturn = false;

		private Point _Coordinate = new Point(0.0, 0.0);

		private double _SafeDirection = 0.0;

		private double _Direction = 0.0;

		private int _LockObject = -1;

		private long _LastActionTick = 0L;

		private long _LastAttackActionTick = 0L;

		private GActions OldAction = GActions.Attack;

		public GActions _Action;

		private GActions _SafeAction;

		public long _HeartInterval = 400L;

		private bool isDeath = false;

		private int deathDelay = 0;

		private int _CurrentMagic = -1;

		private int _CurrentMagicLevel = 1;

		private int _SurvivalTime = 0;

		private long _SurvivalTick = 0L;

		private int _MagicFinish = 0;

		private MagicCoolDownMgr _MagicCoolDownMgr = new MagicCoolDownMgr();

		private long _MaxAttackTimeSlot = 2000L;

		public object CurrentSkillIDIndexLock = new object();

		public int _CurrentSkillIDIndex = 0;

		public int _ToExecSkillID = -1;

		private List<DynSkillItem> DynSkillIDsList = new List<DynSkillItem>();

		public bool DynamicMonster = false;

		public int DynamicPursuitRadius = 0;

		private static object CountLock = new object();

		private static int TotalMonsterCount = 0;

		public MonsterBuffer TempPropsBuffer = new MonsterBuffer();

		public int Step;

		public long MoveTime;

		public List<int[]> PatrolPath;

		private long LastMonsterLivingSlotTicks = TimeUtil.NOW();

		private long LastMonsterLivingTicks = TimeUtil.NOW();

		public object TriggerMutex = new object();

		private Dictionary<int, int> TriggerNumDict = new Dictionary<int, int>();

		private Dictionary<int, long> TriggerCDDict = new Dictionary<int, long>();

		public SpriteExtensionProps ExtensionProps = new SpriteExtensionProps();

		public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();

		public BufferExtManager MyBufferExtManager = new BufferExtManager();

		public TimedActionManager TimedActionMgr = new TimedActionManager();

		public object CaiJiStateLock = new object();

		private bool _IsCollected = false;
	}
}
