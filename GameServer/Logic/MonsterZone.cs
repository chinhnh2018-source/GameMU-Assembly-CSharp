using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.MoRi;
using GameServer.Logic.RefreshIconState;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class MonsterZone
	{
		public int MapCode { get; set; }

		public int ID { get; set; }

		public int Code { get; set; }

		public int ToX { get; set; }

		public int ToY { get; set; }

		public int Radius { get; set; }

		public int TotalNum { get; set; }

		public int Timeslot { get; set; }

		public int PursuitRadius { get; set; }

		public int BirthType { get; set; }

		public int ConfigBirthType { get; set; }

		public int SpawnMonstersAfterKaiFuDays { get; set; }

		public int SpawnMonstersDays { get; set; }

		public List<BirthTimeForDayOfWeek> SpawnMonstersDayOfWeek { get; set; }

		public List<BirthTimePoint> BirthTimePointList { get; set; }

		public int BirthRate { get; set; }

		public MonsterStaticInfo GetMonsterInfo()
		{
			return this.MonsterInfo;
		}

		private void LoadMonster(Monster monster, MonsterZone monsterZone, MonsterStaticInfo monsterInfo, int monsterType, int roleID, string name, double life, double mana, Point coordinate, double direction, double moveSpeed, int attackRange)
		{
			monster.Name = name;
			monster.MonsterZoneNode = monsterZone;
			monster.MonsterInfo = monsterInfo;
			monster.RoleID = roleID;
			monster.VLife = life;
			monster.VMana = mana;
			monster.AttackRange = attackRange;
			monster.FirstCoordinate = coordinate;
			monster.Coordinate = coordinate;
			monster.Direction = direction;
			monster.MoveSpeed = moveSpeed;
			monster.MonsterType = monsterType;
			if (monster.MonsterInfo.ExtProps != null)
			{
				Array.Copy(monster.MonsterInfo.ExtProps, monster.DynamicData.ExtProps, 177);
			}
			monster.CoordinateChanged += this.UpdateMonsterEvent;
			monster.NextSeekEnemyTicks = (long)(3000 + Global.GetRandomNumber(0, 2000));
		}

		private Monster CopyMonster(Monster oldMonster)
		{
			Monster monster = oldMonster.Clone();
			monster.CoordinateChanged += this.UpdateMonsterEvent;
			monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
			return monster;
		}

		private void DestroyMonster(Monster monster)
		{
			if (monster.OwnerClient != null)
			{
				monster.OwnerClient.ClientData.SummonMonstersList.Remove(monster);
				monster.OwnerClient = null;
			}
			if (monster.OwnerMonster != null)
			{
				monster.OwnerMonster.CallMonster = null;
				monster.OwnerMonster = null;
			}
			monster.CoordinateChanged -= this.UpdateMonsterEvent;
			monster.MoveToComplete -= new MoveToEventHandler(this.MoveToComplete);
			GameManager.MapGridMgr.DictGrids[this.MapCode].RemoveObject(monster);
			bool flag = this.MonsterList.Remove(monster);
			GameManager.MonsterMgr.RemoveMonster(monster);
			GameManager.MonsterIDMgr.PushBack((long)monster.RoleID);
			if (flag)
			{
				Monster.DecMonsterCount();
			}
		}

		private Monster InitMonster(XElement monsterXml, double maxLifeV, double maxMagicV, XElement xmlFrameConfig, double moveSpeed, bool attachEvent = true)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
			Monster monster = new Monster();
			int num = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
			monster.UniqueID = Global.GetUniqueID();
			this.LoadMonster(monster, this, this.MonsterInfo, (int)Global.GetSafeAttributeLong(monsterXml, "MonsterType"), num, string.Format("Role_{0}", num), maxLifeV, maxMagicV, Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, this.ToX, this.ToY, this.Radius, 0, true), (double)Global.GetRandomNumber(0, 8), moveSpeed, (int)Global.GetSafeAttributeLong(monsterXml, "AttackRange"));
			if (attachEvent)
			{
				monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
			}
			return monster;
		}

		private bool CanRealiveByRate()
		{
			bool result;
			if (this.BirthRate >= 10000)
			{
				result = true;
			}
			else
			{
				int randomNumber = Global.GetRandomNumber(1, 10001);
				result = (randomNumber <= this.BirthRate);
			}
			return result;
		}

		public void LoadStaticMonsterInfo_2()
		{
			MonsterStaticInfo info = MonsterStaticInfoMgr.GetInfo(this.Code);
			if (null == info)
			{
				this.LoadStaticMonsterInfo();
			}
			else
			{
				this.MonsterInfo = info;
			}
		}

		public void LoadStaticMonsterInfo()
		{
			string text = string.Format("Config/Monsters.xml", new object[0]);
			XElement xelement = GameManager.MonsterZoneMgr.AllMonstersXml;
			if (xelement == null)
			{
				throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			XElement safeXElement = Global.GetSafeXElement(xelement, "Monster", "ID", this.Code.ToString());
			text = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(safeXElement, "ResName"));
			string text2 = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
			try
			{
				xelement = null;
				string text3 = Global.ResPath(text);
				if (File.Exists(text3))
				{
					xelement = XElement.Load(text3);
				}
			}
			catch (Exception)
			{
				xelement = null;
			}
			if (null == xelement)
			{
				text = text2;
				xelement = null;
				string text3 = Global.ResPath(text);
				if (File.Exists(text3))
				{
					xelement = XElement.Load(text3);
				}
				if (null == xelement)
				{
					throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			XElement safeXElement2 = Global.GetSafeXElement(xelement, "FrameConfig");
			XElement safeXElement3 = Global.GetSafeXElement(xelement, "SpeedConfig");
			int[] speedTickList = Global.StringArray2IntArray(Global.GetSafeAttributeStr(safeXElement3, "Tick").Split(new char[]
			{
				','
			}));
			double num = Global.GetSafeAttributeDouble(safeXElement3, "UnitSpeed") / 100.0;
			double safeAttributeDouble = Global.GetSafeAttributeDouble(safeXElement, "MonsterSpeed");
			num *= safeAttributeDouble;
			double num2 = (double)Global.GetSafeAttributeLong(safeXElement, "MaxLife");
			double maxMagicV = (double)Global.GetSafeAttributeLong(safeXElement, "MaxMagic");
			if (num2 <= 0.0)
			{
				LogManager.WriteLog(2, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(safeXElement, "ID"), Global.GetSafeAttributeStr(safeXElement, "SName")), null, true);
			}
			else
			{
				this.InitMonsterStaticInfo(safeXElement, num2, maxMagicV, safeXElement2, num, speedTickList);
			}
		}

		public void LoadMonsters()
		{
			string text = string.Format("Config/Monsters.xml", new object[0]);
			XElement xelement = GameManager.MonsterZoneMgr.AllMonstersXml;
			if (xelement == null)
			{
				throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			XElement safeXElement = Global.GetSafeXElement(xelement, "Monster", "ID", this.Code.ToString());
			MonsterNameManager.AddMonsterName(this.Code, Global.GetSafeAttributeStr(safeXElement, "SName"));
			text = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(safeXElement, "ResName"));
			string text2 = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
			try
			{
				xelement = XElement.Load(Global.ResPath(text));
			}
			catch (Exception)
			{
				xelement = null;
			}
			if (null == xelement)
			{
				text = text2;
				xelement = XElement.Load(Global.ResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			XElement safeXElement2 = Global.GetSafeXElement(xelement, "FrameConfig");
			XElement safeXElement3 = Global.GetSafeXElement(xelement, "SpeedConfig");
			double num = Global.GetSafeAttributeDouble(safeXElement3, "UnitSpeed") / 100.0;
			double safeAttributeDouble = Global.GetSafeAttributeDouble(safeXElement, "MonsterSpeed");
			num *= safeAttributeDouble;
			double num2 = (double)Global.GetSafeAttributeLong(safeXElement, "MaxLife");
			double maxMagicV = (double)Global.GetSafeAttributeLong(safeXElement, "MaxMagic");
			if (num2 <= 0.0)
			{
				LogManager.WriteLog(2, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(safeXElement, "ID"), Global.GetSafeAttributeStr(safeXElement, "SName")), null, true);
			}
			else if (!this.IsFuBenMap)
			{
				for (int i = 0; i < this.TotalNum; i++)
				{
					Monster monster = this.InitMonster(safeXElement, num2, maxMagicV, safeXElement2, num, true);
					if (MonsterTypes.None == this.MonsterType)
					{
						this.MonsterType = (MonsterTypes)monster.MonsterType;
					}
					this.MonsterList.Add(monster);
					GameManager.MonsterMgr.AddMonster(monster);
					if (401 == monster.MonsterType)
					{
						MonsterBossManager.AddBoss(monster);
					}
				}
			}
			else
			{
				Monster monster = this.InitMonster(safeXElement, num2, maxMagicV, safeXElement2, num, true);
				if (MonsterTypes.None == this.MonsterType)
				{
					this.MonsterType = (MonsterTypes)monster.MonsterType;
				}
				this.SeedMonster = monster;
			}
		}

		public Monster LoadDynamicMonsterSeed()
		{
			string text = string.Format("Config/Monsters.xml", new object[0]);
			XElement xelement = GameManager.MonsterZoneMgr.AllMonstersXml;
			if (xelement == null)
			{
				throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			XElement safeXElement = Global.GetSafeXElement(xelement, "Monster", "ID", this.Code.ToString());
			MonsterNameManager.AddMonsterName(this.Code, Global.GetSafeAttributeStr(safeXElement, "SName"));
			text = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(safeXElement, "ResName"));
			string text2 = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
			try
			{
				xelement = null;
				string text3 = Global.ResPath(text);
				if (File.Exists(text3))
				{
					xelement = XElement.Load(text3);
				}
			}
			catch (Exception)
			{
				xelement = null;
			}
			if (null == xelement)
			{
				text = text2;
				xelement = null;
				string text3 = Global.ResPath(text);
				if (File.Exists(text3))
				{
					xelement = XElement.Load(text3);
				}
				if (null == xelement)
				{
					throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			XElement safeXElement2 = Global.GetSafeXElement(xelement, "FrameConfig");
			XElement safeXElement3 = Global.GetSafeXElement(xelement, "SpeedConfig");
			double num = Global.GetSafeAttributeDouble(safeXElement3, "UnitSpeed") / 100.0;
			double safeAttributeDouble = Global.GetSafeAttributeDouble(safeXElement, "MonsterSpeed");
			num *= safeAttributeDouble;
			double num2 = (double)Global.GetSafeAttributeLong(safeXElement, "MaxLife");
			double maxMagicV = (double)Global.GetSafeAttributeLong(safeXElement, "MaxMagic");
			Monster result;
			if (num2 <= 0.0)
			{
				LogManager.WriteLog(2, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(safeXElement, "ID"), Global.GetSafeAttributeStr(safeXElement, "SName")), null, true);
				result = null;
			}
			else
			{
				result = this.InitMonster(safeXElement, num2, maxMagicV, safeXElement2, num, false);
			}
			return result;
		}

		private void MoveToComplete(object sender)
		{
			(sender as Monster).DestPoint = new Point(-1.0, -1.0);
			(sender as Monster).Action = GActions.Stand;
			Global.RemoveStoryboard((sender as Monster).Name);
		}

		private void UpdateMonsterEvent(Monster monster)
		{
			if (!monster.FirstStoryMove)
			{
				GameManager.MapGridMgr.DictGrids[this.MapCode].MoveObject((int)monster.SafeOldCoordinate.X, (int)monster.SafeOldCoordinate.Y, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, monster);
			}
			else
			{
				monster.FirstStoryMove = false;
			}
		}

		public string GetNextBirthTimePoint()
		{
			string result;
			if (this.BirthType == 7 && this.SpawnMonstersDayOfWeek != null)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				int num = -1;
				if (this.LastBirthTimePointIndex >= 0)
				{
					num = (this.LastBirthTimePointIndex + 1) % this.SpawnMonstersDayOfWeek.Count;
				}
				else
				{
					DateTime dateTime2 = TimeUtil.NowDateTime();
					int num2 = (int)(dateTime2.DayOfWeek * (DayOfWeek)1440 + dateTime2.Hour * 60 + dateTime2.Minute);
					for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
					{
						int num3 = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek * 1440 + this.SpawnMonstersDayOfWeek[i].BirthTime.BirthHour * 60 + this.SpawnMonstersDayOfWeek[i].BirthTime.BirthMinute;
						if (num2 <= num3)
						{
							num = i;
							break;
						}
					}
					if (num < 0)
					{
						num = 0;
					}
				}
				int birthDayOfWeek = this.SpawnMonstersDayOfWeek[num].BirthDayOfWeek;
				int birthHour = this.SpawnMonstersDayOfWeek[num].BirthTime.BirthHour;
				int birthMinute = this.SpawnMonstersDayOfWeek[num].BirthTime.BirthMinute;
				result = string.Format("{0}${1}${2}", birthHour, birthMinute, birthDayOfWeek);
			}
			else if (null == this.BirthTimePointList)
			{
				result = "";
			}
			else
			{
				int lastBirthTimePointIndex = this.LastBirthTimePointIndex;
				int num = 0;
				if (lastBirthTimePointIndex >= 0)
				{
					num = (lastBirthTimePointIndex + 1) % this.BirthTimePointList.Count;
				}
				else
				{
					DateTime dateTime2 = TimeUtil.NowDateTime();
					int num3 = dateTime2.Hour * 60 + dateTime2.Minute;
					for (int i = 0; i < this.BirthTimePointList.Count; i++)
					{
						int num2 = this.BirthTimePointList[i].BirthHour * 60 + this.BirthTimePointList[i].BirthMinute;
						if (num3 <= num2)
						{
							num = i;
							break;
						}
					}
				}
				result = string.Format("{0}${1}", this.BirthTimePointList[num].BirthHour, this.BirthTimePointList[num].BirthMinute);
			}
			return result;
		}

		private bool CanBirthOnTimePoint(DateTime now, BirthTimePoint birthTimePoint)
		{
			if (now.DayOfYear == this.LastBirthDayID)
			{
				if (null != this.LastBirthTimePoint)
				{
					if (this.LastBirthTimePoint.BirthHour == birthTimePoint.BirthHour && this.LastBirthTimePoint.BirthMinute == birthTimePoint.BirthMinute)
					{
						return false;
					}
				}
			}
			bool result;
			if (now.Hour != birthTimePoint.BirthHour)
			{
				result = false;
			}
			else
			{
				while (this.LastReloadMonstersDateTime < now)
				{
					if (this.LastReloadMonstersDateTime.Minute == birthTimePoint.BirthMinute)
					{
						return true;
					}
					this.LastReloadMonstersDateTime = this.LastReloadMonstersDateTime.AddMinutes(1.0);
				}
				result = false;
			}
			return result;
		}

		private bool CanBirthOnTimePointForWeekOfDay(DateTime now, BirthTimePoint birthTimePoint)
		{
			if (now.DayOfYear == this.LastBirthDayID)
			{
				if (null != this.LastBirthTimePoint)
				{
					if (this.LastBirthTimePoint.BirthHour == birthTimePoint.BirthHour && this.LastBirthTimePoint.BirthMinute == birthTimePoint.BirthMinute)
					{
						return false;
					}
				}
			}
			bool result;
			if (now.Hour != birthTimePoint.BirthHour)
			{
				result = false;
			}
			else
			{
				int birthMinute = birthTimePoint.BirthMinute;
				int num = birthTimePoint.BirthMinute + 1;
				result = (now.Minute >= birthMinute && now.Minute <= num);
			}
			return result;
		}

		public void ReloadMonsters(SocketListener sl, TCPOutPacketPool pool)
		{
			if (!this.IsFuBenMap)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (Global.CanMapInLimitTimes(this.MapCode, dateTime))
				{
					if (!this.CanTodayReloadMonsters() || !this.CanTodayReloadMonstersForDayOfWeek())
					{
						if (!this.HasSystemKilledAllOfThisZone)
						{
							this.SystemKillAllMonstersOfThisZone();
							this.HasSystemKilledAllOfThisZone = true;
						}
					}
					else
					{
						this.HasSystemKilledAllOfThisZone = false;
						if (this.Code > 0 && this.ConfigBirthType == 6)
						{
							try
							{
								this.LoadStaticMonsterInfo();
							}
							catch (Exception ex)
							{
								LogManager.WriteLog(2, "reload jieri boss monster failed", ex, true);
							}
						}
						if (0 == this.BirthType)
						{
							long ticks = dateTime.Ticks;
							if (this.LastReloadTicks <= 0L || ticks - this.LastReloadTicks >= 10000000L)
							{
								this.LastReloadTicks = ticks;
								this.MonsterRealive(sl, pool, -1, 65535);
							}
						}
						else if (1 == this.BirthType)
						{
							if (null != this.BirthTimePointList)
							{
								int num = 0;
								if (this.LastBirthTimePointIndex >= 0)
								{
									num = (this.LastBirthTimePointIndex + 1) % this.BirthTimePointList.Count;
								}
								else
								{
									for (int i = 0; i < this.BirthTimePointList.Count; i++)
									{
										if (this.CanBirthOnTimePoint(dateTime, this.BirthTimePointList[i]))
										{
											num = i;
											break;
										}
									}
								}
								BirthTimePoint birthTimePoint = this.BirthTimePointList[num];
								if (this.CanBirthOnTimePoint(dateTime, birthTimePoint))
								{
									this.LastBirthTimePointIndex = num;
									this.LastBirthTimePoint = birthTimePoint;
									this.LastBirthDayID = TimeUtil.NowDateTime().DayOfYear;
									this.MonsterRealive(sl, pool, -1, 65535);
								}
							}
						}
						else if (7 == this.BirthType)
						{
							if (this.SpawnMonstersDayOfWeek == null)
							{
								return;
							}
							DayOfWeek dayOfWeek = TimeUtil.NowDateTime().DayOfWeek;
							for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
							{
								int birthDayOfWeek = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek;
								if (birthDayOfWeek == (int)dayOfWeek)
								{
									BirthTimePoint birthTime = this.SpawnMonstersDayOfWeek[i].BirthTime;
									if (this.CanBirthOnTimePoint(dateTime, birthTime))
									{
										this.LastBirthTimePointIndex = i;
										this.LastBirthTimePoint = birthTime;
										this.LastBirthDayID = TimeUtil.NowDateTime().DayOfYear;
										this.MonsterRealive(sl, pool, -1, 65535);
									}
								}
							}
						}
						this.LastReloadMonstersDateTime = dateTime;
					}
				}
			}
		}

		public bool CanTodayReloadMonsters()
		{
			bool result;
			if (this.SpawnMonstersAfterKaiFuDays <= 0 && this.SpawnMonstersDays <= 0)
			{
				result = true;
			}
			else
			{
				DateTime right = Global.GetKaiFuTime();
				if (this.ConfigBirthType == 5)
				{
					HeFuActivityConfig heFuActivityConfing = HuodongCachingMgr.GetHeFuActivityConfing();
					if (null == heFuActivityConfing)
					{
						return false;
					}
					if (!heFuActivityConfing.InList(26))
					{
						return false;
					}
					right = Global.GetHefuStartDay();
				}
				else if (this.ConfigBirthType == 6)
				{
					JieriActivityConfig jieriActivityConfig = HuodongCachingMgr.GetJieriActivityConfig();
					if (null == jieriActivityConfig)
					{
						return false;
					}
					if (!jieriActivityConfig.InList(17))
					{
						return false;
					}
					right = Global.GetJieriStartDay();
				}
				DateTime left = TimeUtil.NowDateTime();
				int num = Global.GetDaysSpanNum(left, right, true) + 1;
				if (this.SpawnMonstersAfterKaiFuDays <= 0 || num >= this.SpawnMonstersAfterKaiFuDays)
				{
					if (this.SpawnMonstersDays <= 0 || num < this.SpawnMonstersDays + this.SpawnMonstersAfterKaiFuDays)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool CanTodayReloadMonstersForDayOfWeek()
		{
			bool result;
			if (this.SpawnMonstersDayOfWeek == null)
			{
				result = true;
			}
			else if (this.ConfigBirthType != 7)
			{
				result = true;
			}
			else
			{
				DayOfWeek dayOfWeek = TimeUtil.NowDateTime().DayOfWeek;
				for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
				{
					int birthDayOfWeek = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek;
					if (birthDayOfWeek == (int)dayOfWeek)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public void SystemKillAllMonstersOfThisZone()
		{
			for (int i = 0; i < this.MonsterList.Count; i++)
			{
				if (null != this.MonsterList[i])
				{
					if (this.MonsterList[i].Alive)
					{
						Global.SystemKillMonster(this.MonsterList[i]);
					}
				}
			}
		}

		private void RepositionMonster(Monster monster, int toX, int toY)
		{
			GameManager.MapGridMgr.DictGrids[this.MapCode].MoveObject(-1, -1, toX, toY, monster);
		}

		private void MonsterRealive(SocketListener sl, TCPOutPacketPool pool, int copyMapID = -1, int birthCount = 65535)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(this.MapCode);
			if (50 != mapSceneType || MoYuLongXue.InActivityTime())
			{
				int num = 0;
				for (int i = 0; i < this.MonsterList.Count; i++)
				{
					if (null != this.MonsterList[i])
					{
						if (num >= birthCount)
						{
							break;
						}
						if (-1 != copyMapID)
						{
							if (this.MonsterList[i].CopyMapID != copyMapID)
							{
								goto IL_48F;
							}
						}
						if (!this.MonsterList[i].Alive)
						{
							if ((this.BirthType == 0 || 2 == this.BirthType) && this.Timeslot > 0)
							{
								long num2 = (long)this.Timeslot * 1000L * 10000L;
								if (TimeUtil.NOW() * 10000L - this.MonsterList[i].LastDeadTicks < num2)
								{
									goto IL_48F;
								}
								if (this.BirthType == 0 && this.BirthTimePointList != null && this.BirthTimePointList.Count == 2)
								{
									DateTime dateTime = TimeUtil.NowDateTime();
									int num3 = dateTime.Hour * 60 + dateTime.Minute;
									int num4 = this.BirthTimePointList[0].BirthHour * 60 + this.BirthTimePointList[0].BirthMinute;
									int num5 = this.BirthTimePointList[1].BirthHour * 60 + this.BirthTimePointList[1].BirthMinute;
									if (num3 < num4 || num5 < num3)
									{
										goto IL_48F;
									}
								}
							}
							if (this.CanRealiveByRate())
							{
								Point point = this.MonsterList[i].Realive();
								this.RepositionMonster(this.MonsterList[i], (int)point.X, (int)point.Y);
								List<object> all9Clients = Global.GetAll9Clients(this.MonsterList[i]);
								GameManager.ClientMgr.NotifyMonsterRealive(sl, pool, this.MonsterList[i], this.MapCode, this.MonsterList[i].CopyMapID, this.MonsterList[i].RoleID, (int)this.MonsterList[i].Coordinate.X, (int)this.MonsterList[i].Coordinate.Y, (int)this.MonsterList[i].Direction, all9Clients);
								num++;
								if (401 == this.MonsterList[i].MonsterType)
								{
									if (this.BirthType == 0 && this.Timeslot >= 1800)
									{
										GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, StringUtil.substitute(GLang.GetLang(503, new object[0]), new object[]
										{
											this.MonsterList[i].MonsterInfo.VSName,
											Global.GetMapName(this.MonsterList[i].CurrentMapCode)
										}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0, 0, 0, 100, 100);
									}
									if (50 == mapSceneType)
									{
										MoYuLongXue.ProcessAddMonster(this.MonsterList[i]);
									}
								}
								if (Global.IsGongGaoReliveMonster(this.MonsterList[i].MonsterInfo.ExtensionID))
								{
									GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, StringUtil.substitute(GLang.GetLang(504, new object[0]), new object[]
									{
										Global.GetMapName(this.MonsterList[i].CurrentMapCode),
										this.MonsterList[i].MonsterInfo.VSName
									}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0, 0, 0, 100, 100);
								}
								if (this.BirthType == 1 || this.BirthType == 7)
								{
									TimerBossManager.getInstance().AddBoss(this.BirthType, this.MonsterList[i].MonsterInfo.ExtensionID);
								}
							}
						}
					}
					IL_48F:;
				}
			}
		}

		public void LoadCopyMapMonsters(int copyMapID)
		{
			if (this.IsFuBenMap)
			{
				if (null != this.SeedMonster)
				{
					for (int i = 0; i < this.TotalNum; i++)
					{
						Monster monster = this.CopyMonster(this.SeedMonster);
						int num = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
						monster.RoleID = num;
						monster.UniqueID = Global.GetUniqueID();
						monster.Name = string.Format("Role_{0}", num);
						monster.CopyMapID = copyMapID;
						monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, this.ToX, this.ToY, this.Radius, 0, true);
						if (Global.InOnlyObsByXY(ObjectTypes.OT_MONSTER, monster.CurrentMapCode, (int)monster.FirstCoordinate.X, (int)monster.FirstCoordinate.Y))
						{
							Debug.WriteLine("abc");
						}
						monster.Direction = (double)Global.GetRandomNumber(0, 8);
						this.MonsterList.Add(monster);
						GameManager.MonsterMgr.AddMonster(monster);
						monster.Start();
						monster.Coordinate = monster.FirstCoordinate;
					}
				}
			}
		}

		public void ReloadCopyMapMonsters(SocketListener sl, TCPOutPacketPool pool, int copyMapID)
		{
			if (this.IsFuBenMap)
			{
				if (2 == this.BirthType)
				{
					this.MonsterRealive(sl, pool, copyMapID, 65535);
				}
			}
		}

		public void ClearCopyMapMonsters(int copyMapID)
		{
			if (this.IsFuBenMap)
			{
				List<Monster> list = new List<Monster>();
				bool flag = false;
				for (int i = 0; i < this.MonsterList.Count; i++)
				{
					if (null == this.MonsterList[i])
					{
						flag = true;
					}
					else if (this.MonsterList[i].CopyMapID == copyMapID)
					{
						list.Add(this.MonsterList[i]);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this.DestroyMonster(list[i]);
				}
				if (flag)
				{
					this.MonsterList.RemoveAll((Monster x) => null == x);
				}
			}
		}

		public void DestroyDeadMonsters(bool onlyFuBen = true)
		{
			if (this.IsFuBenMap || !onlyFuBen)
			{
				if (this.BirthType != 2)
				{
					long num = TimeUtil.NOW() * 10000L;
					long num2 = 300000000L;
					if (num - this.LastDestroyTicks >= num2)
					{
						this.LastDestroyTicks = num;
						List<Monster> list = new List<Monster>();
						bool flag = false;
						for (int i = 0; i < this.MonsterList.Count; i++)
						{
							if (null == this.MonsterList[i])
							{
								flag = true;
							}
							else if (!this.MonsterList[i].Alive)
							{
								list.Add(this.MonsterList[i]);
							}
						}
						for (int i = 0; i < list.Count; i++)
						{
							this.DestroyMonster(list[i]);
						}
						if (flag)
						{
							this.MonsterList.RemoveAll((Monster x) => null == x);
							LogManager.WriteLog(2, string.Format("DestroyDeadMonsters MonsterList Exist Null!!!", new object[0]), null, true);
						}
					}
				}
			}
		}

		public void DestroyDeadDynamicMonsters()
		{
			if (this.IsDynamicZone())
			{
				this.DestroyDeadMonsters(false);
			}
		}

		public bool IsDynamicZone()
		{
			return 3 == this.BirthType;
		}

		public Monster LoadDynamicRobot(MonsterZoneQueueItem monsterZoneQueueItem)
		{
			Monster seedMonster = monsterZoneQueueItem.seedMonster;
			seedMonster.Tag = monsterZoneQueueItem.Tag;
			seedMonster.ManagerType = monsterZoneQueueItem.ManagerType;
			seedMonster.CopyMapID = monsterZoneQueueItem.CopyMapID;
			seedMonster.MonsterZoneNode = this;
			seedMonster.CoordinateChanged += this.UpdateMonsterEvent;
			seedMonster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
			seedMonster.DynamicMonster = true;
			seedMonster.DynamicPursuitRadius = monsterZoneQueueItem.PursuitRadius;
			seedMonster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
			seedMonster.Direction = (double)Global.GetRandomNumber(0, 8);
			this.MonsterList.Add(seedMonster);
			GameManager.MonsterMgr.AddMonster(seedMonster);
			seedMonster.Start();
			seedMonster.Coordinate = seedMonster.FirstCoordinate;
			JingJiChangManager.getInstance().onRobotBron(seedMonster as Robot);
			return seedMonster;
		}

		public void LoadDynamicMonsters(MonsterZoneQueueItem monsterZoneQueueItem)
		{
			if (this.IsDynamicZone())
			{
				if (monsterZoneQueueItem != null && null != monsterZoneQueueItem.seedMonster)
				{
					for (int i = 0; i < monsterZoneQueueItem.BirthCount; i++)
					{
						Monster monster = this.CopyMonster(monsterZoneQueueItem.seedMonster);
						if (monster.OwnerClient != null)
						{
							monster.OwnerClient.ClientData.SummonMonstersList.Add(monster);
						}
						if (monster.OwnerMonster != null)
						{
							monster.OwnerMonster.CallMonster = monster;
						}
						int num = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
						monster.RoleID = num;
						monster.UniqueID = Global.GetUniqueID();
						monster.Tag = monsterZoneQueueItem.Tag;
						monster.ManagerType = monsterZoneQueueItem.ManagerType;
						monster.Name = string.Format("Role_{0}", num);
						monster.CopyMapID = monsterZoneQueueItem.CopyMapID;
						monster.MonsterZoneNode = this;
						monster.CoordinateChanged += this.UpdateMonsterEvent;
						monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
						monster.DynamicMonster = true;
						monster.DynamicPursuitRadius = monsterZoneQueueItem.PursuitRadius;
						monster.Flags.Copy(monsterZoneQueueItem.Flags);
						if (monster.OwnerClient != null)
						{
							monster.FirstCoordinate = new Point((double)monster.OwnerClient.ClientData.PosX, (double)monster.OwnerClient.ClientData.PosY);
						}
						if (monster.OwnerMonster != null)
						{
							monster.FirstCoordinate = new Point(monster.OwnerMonster.CurrentPos.X, monster.OwnerMonster.CurrentPos.Y);
						}
						else if (monster.ManagerType == 11)
						{
							monster.FirstCoordinate = new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY);
							monster.Step = 0;
							monster.PatrolPath = (monster.Tag as List<int[]>);
						}
						else if (monster.MonsterType == 1501)
						{
							Point firstCoordinate = new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY);
							monster.FirstCoordinate = firstCoordinate;
							monster.Step = 0;
							monster.PatrolPath = Data.Goldcopyscenedata.m_MonsterPatorlPathList;
						}
						else if (monster.MonsterType == 1502)
						{
							monster.FirstCoordinate = Global.GridToPixel(this.MapCode, new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY));
							monster.Step = 0;
						}
						else if (monster.MonsterType == 2001)
						{
							if (Global.InOnlyObs(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY))
							{
								monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
							}
							else
							{
								monster.FirstCoordinate = Global.GridToPixel(this.MapCode, new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY));
							}
						}
						else
						{
							monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
						}
						monster.Direction = (double)Global.GetRandomNumber(0, 8);
						if (monsterZoneQueueItem.MyMonsterZone.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
						{
							GameManager.AngelTempleMgr.OnLoadDynamicMonsters(monster);
						}
						else if (monsterZoneQueueItem.MyMonsterZone.MapCode == SingletonTemplate<MoRiJudgeManager>.Instance().MapCode)
						{
							SingletonTemplate<MoRiJudgeManager>.Instance().OnLoadDynamicMonsters(monster);
						}
						else if (LingDiCaiJiManager.getInstance().GetLingDiType(monsterZoneQueueItem.MyMonsterZone.MapCode) != 2)
						{
							LingDiCaiJiManager.getInstance().OnLoadDynamicMonsters(monsterZoneQueueItem.MyMonsterZone.MapCode, monster);
						}
						if (Global.IsStoryCopyMapScene(monsterZoneQueueItem.MyMonsterZone.MapCode))
						{
							CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(monster.CopyMapID);
							if (copyMap == null)
							{
								break;
							}
							SystemXmlItem systemXmlItem = null;
							if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemXmlItem) && systemXmlItem != null)
							{
								int intValue = systemXmlItem.GetIntValue("BossID", -1);
								if (intValue == monster.MonsterInfo.ExtensionID)
								{
									Global.NotifyClientStoryCopyMapInfo(monster.CopyMapID, 2);
								}
							}
						}
						this.MonsterList.Add(monster);
						GameManager.MonsterMgr.AddMonster(monster);
						GlobalEventSource4Scene.getInstance().fireEvent(new OnCreateMonsterEventObject(monster), monster.ManagerType);
						monster.Start();
						monster.Coordinate = monster.FirstCoordinate;
						if (1001 == monster.MonsterType)
						{
							GameManager.ClientMgr.NotifyOthersMyDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, 160, 2, -1, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, 0, -1, -1, 0, 0, null);
						}
					}
				}
			}
		}

		public string GetMonstersInfoString()
		{
			int num = 0;
			int num2 = 0;
			foreach (Monster monster in this.MonsterList)
			{
				if (null != monster)
				{
					if (monster.Alive)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
			}
			string result;
			if (num + num2 > 0)
			{
				result = string.Format("地图{0}, {1}副本, aliveCount={2}, deadCount={3}, total={4}", new object[]
				{
					this.MapCode,
					this.IsFuBenMap ? "是" : "不是",
					num,
					num2,
					num + num2
				});
			}
			else
			{
				result = "";
			}
			return result;
		}

		public void ReloadNormalMapMonsters(SocketListener sl, TCPOutPacketPool pool, int birthCount)
		{
			if (!this.IsFuBenMap)
			{
				if (2 == this.BirthType)
				{
					this.MonsterRealive(sl, pool, -1, birthCount);
				}
			}
		}

		public void OnReallyDied(Monster deadMonster)
		{
		}

		private void InitMonsterStaticInfo(XElement monsterXml, double maxLifeV, double maxMagicV, XElement xmlFrameConfig, double moveSpeed, int[] speedTickList)
		{
			double[] extProps = ConfigParser.ParserExtPropsFromAttrubite(monsterXml, MonsterZone.MonsterExtPropsConfigList);
			this.SetStaticInfo4Monster(Global.GetSafeAttributeStr(monsterXml, "SName"), (int)Global.GetSafeAttributeLong(monsterXml, "ID"), maxLifeV, maxMagicV, (int)Global.GetSafeAttributeLong(monsterXml, "Level"), (int)Global.GetSafeAttributeLong(monsterXml, "Experience"), 0, Global.StringArray2IntArray(Global.GetSafeAttributeStr(xmlFrameConfig, "EachActionFrameRange").Split(new char[]
			{
				','
			})), Global.StringArray2IntArray(Global.GetSafeAttributeStr(xmlFrameConfig, "EachActionEffectiveFrame").Split(new char[]
			{
				','
			})), (int)Global.GetSafeAttributeLong(monsterXml, "AttackRange"), (int)Global.GetSafeAttributeLong(monsterXml, "SeedRange"), (int)Global.GetSafeAttributeLong(monsterXml, "Code"), -1, speedTickList, 0, 0, (int)Global.GetSafeAttributeLong(monsterXml, "MinAttackPercent"), (int)Global.GetSafeAttributeLong(monsterXml, "MaxAttackPercent"), (int)Global.GetSafeAttributeLong(monsterXml, "DefensePercent"), (int)Global.GetSafeAttributeLong(monsterXml, "MDefensePercent"), Global.GetSafeAttributeDouble(monsterXml, "HitV"), Global.GetSafeAttributeDouble(monsterXml, "Dodge"), Global.GetSafeAttributeDouble(monsterXml, "RecoverLifeV"), Global.GetSafeAttributeDouble(monsterXml, "RecoverMagicV"), Global.GetSafeAttributeDouble(monsterXml, "DamageThornPercent"), Global.GetSafeAttributeDouble(monsterXml, "DamageThorn"), Global.GetSafeAttributeDouble(monsterXml, "SubAttackInjurePercent"), Global.GetSafeAttributeDouble(monsterXml, "SubAttackInjure"), Global.GetSafeAttributeDouble(monsterXml, "IgnoreDefensePercent"), Global.GetSafeAttributeDouble(monsterXml, "IgnoreDefenseRate"), Global.GetSafeAttributeDouble(monsterXml, "Lucky"), Global.GetSafeAttributeDouble(monsterXml, "FatalAttack"), Global.GetSafeAttributeDouble(monsterXml, "DoubleAttack"), (int)Global.GetSafeAttributeLong(monsterXml, "FallID"), (int)Global.GetSafeAttributeLong(monsterXml, "MonsterType"), (int)Global.GetSafeAttributeLong(monsterXml, "PersonalJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "CampJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "EMoJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "XueSeJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "Belong"), Global.String2IntArray(Global.GetSafeAttributeStr(monsterXml, "SkillIDs"), ','), (int)Global.GetSafeAttributeLong(monsterXml, "AttackType"), (int)Global.GetSafeAttributeLong(monsterXml, "Camp"), (int)Global.GetSafeAttributeLong(monsterXml, "AIID"), (int)Global.GetSafeAttributeLong(monsterXml, "ZhuanSheng"), (int)Global.GetSafeAttributeLong(monsterXml, "LangHunJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "RebornExp"), extProps);
		}

		private void SetStaticInfo4Monster(string sname, int extensionID, double life, double mana, int level, int experience, int money, int[] frameRange, int[] effectiveFrame, int attackRange, int seekRange, int equipmentBody, int equipmentWeapon, int[] speedTickList, int toOccupation, int toRoleLevel, int minAttack, int maxAttack, int defense, int magicDefense, double hitV, double dodge, double recoverLifeV, double recoverMagicV, double DamageThornPercent, double DamageThorn, double SubAttackInjurePercent, double SubAttackInjure, double IgnoreDefensePercent, double IgnoreDefenseRate, double Lucky, double FatalAttack, double DoubleAttack, int fallGoodsPackID, int monsterType, int battlePersonalJiFen, int battleZhenYingJiFen, int nDaimonSquareJiFen, int nBloodCastJiFen, int fallBelongTo, int[] skillIDs, int attackType, int camp, int AIID, int nChangeLifeCount, int nWolfScore, int rebornExp, double[] extProps)
		{
			this.MonsterInfo.SpriteSpeedTickList = speedTickList;
			this.MonsterInfo.VSName = sname;
			this.MonsterInfo.ExtensionID = extensionID;
			this.MonsterInfo.VLifeMax = life;
			this.MonsterInfo.VManaMax = mana;
			this.MonsterInfo.VLevel = level;
			this.MonsterInfo.VExperience = experience;
			this.MonsterInfo.VMoney = money;
			this.MonsterInfo.EachActionFrameRange = frameRange;
			this.MonsterInfo.EffectiveFrame = effectiveFrame;
			this.MonsterInfo.SeekRange = seekRange;
			this.MonsterInfo.EquipmentBody = equipmentBody;
			this.MonsterInfo.EquipmentWeapon = equipmentWeapon;
			this.MonsterInfo.ToOccupation = toOccupation;
			this.MonsterInfo.MinAttack = minAttack;
			this.MonsterInfo.MaxAttack = maxAttack;
			this.MonsterInfo.Defense = defense;
			this.MonsterInfo.MDefense = magicDefense;
			this.MonsterInfo.HitV = hitV;
			this.MonsterInfo.Dodge = dodge;
			this.MonsterInfo.RecoverLifeV = recoverLifeV;
			this.MonsterInfo.RecoverMagicV = recoverMagicV;
			this.MonsterInfo.MonsterDamageThornPercent = DamageThornPercent;
			this.MonsterInfo.MonsterDamageThorn = DamageThorn;
			this.MonsterInfo.MonsterSubAttackInjurePercent = SubAttackInjurePercent;
			this.MonsterInfo.MonsterSubAttackInjure = SubAttackInjure;
			this.MonsterInfo.MonsterIgnoreDefensePercent = IgnoreDefensePercent;
			this.MonsterInfo.MonsterIgnoreDefenseRate = IgnoreDefenseRate;
			this.MonsterInfo.MonsterLucky = Lucky;
			this.MonsterInfo.MonsterFatalAttack = FatalAttack;
			this.MonsterInfo.MonsterDoubleAttack = DoubleAttack;
			this.MonsterInfo.FallGoodsPackID = fallGoodsPackID;
			this.MonsterInfo.BattlePersonalJiFen = Global.GMax(0, battlePersonalJiFen);
			this.MonsterInfo.BattleZhenYingJiFen = Global.GMax(0, battleZhenYingJiFen);
			this.MonsterInfo.DaimonSquareJiFen = Global.GMax(0, nDaimonSquareJiFen);
			this.MonsterInfo.BloodCastJiFen = Global.GMax(0, nBloodCastJiFen);
			this.MonsterInfo.WolfScore = Global.GMax(0, nWolfScore);
			this.MonsterInfo.FallBelongTo = Global.GMax(0, fallBelongTo);
			this.MonsterInfo.SkillIDs = skillIDs;
			this.MonsterInfo.AttackType = attackType;
			this.MonsterInfo.Camp = camp;
			this.MonsterInfo.AIID = AIID;
			this.MonsterInfo.ChangeLifeCount = ((nChangeLifeCount < 0) ? 0 : nChangeLifeCount);
			this.MonsterInfo.RebornExp = rebornExp;
			this.MonsterInfo.ExtProps = extProps;
			MonsterStaticInfoMgr.SetInfo(extensionID, this.MonsterInfo);
		}

		private bool HasSystemKilledAllOfThisZone = false;

		public bool IsFuBenMap = false;

		public MonsterTypes MonsterType = MonsterTypes.None;

		private long LastReloadTicks = 0L;

		private long LastDestroyTicks = 0L;

		private int LastBirthDayID = -1;

		private BirthTimePoint LastBirthTimePoint = null;

		private int LastBirthTimePointIndex = -1;

		private DateTime LastReloadMonstersDateTime = DateTime.MaxValue;

		private MonsterStaticInfo MonsterInfo = new MonsterStaticInfo();

		private List<Monster> MonsterList = new List<Monster>(100);

		private Monster SeedMonster = null;

		private static List<KeyValuePair<int, string>> MonsterExtPropsConfigList = new List<KeyValuePair<int, string>>
		{
			new KeyValuePair<int, string>(24, "SubAttackInjurePercent"),
			new KeyValuePair<int, string>(25, "SubAttackInjure"),
			new KeyValuePair<int, string>(122, "HolyAttack"),
			new KeyValuePair<int, string>(123, "HolyDefense"),
			new KeyValuePair<int, string>(126, "HolyWeakPercent"),
			new KeyValuePair<int, string>(129, "ShadowAttack"),
			new KeyValuePair<int, string>(130, "ShadowDefense"),
			new KeyValuePair<int, string>(133, "ShadowWeakPercent"),
			new KeyValuePair<int, string>(136, "NatureAttack"),
			new KeyValuePair<int, string>(137, "NatureDefense"),
			new KeyValuePair<int, string>(140, "NatureWeakPercent"),
			new KeyValuePair<int, string>(143, "ChaosAttack"),
			new KeyValuePair<int, string>(144, "ChaosDefense"),
			new KeyValuePair<int, string>(147, "ChaosWeakPercent"),
			new KeyValuePair<int, string>(150, "IncubusAttack"),
			new KeyValuePair<int, string>(151, "IncubusDefense"),
			new KeyValuePair<int, string>(154, "IncubusWeakPercent")
		};
	}
}
