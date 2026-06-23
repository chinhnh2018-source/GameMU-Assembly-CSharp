using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using Server.Tools;

namespace GameServer.Logic
{
	public class GuildCopyMapManager
	{
		public int FirstGuildCopyMapOrder
		{
			get
			{
				return 40000;
			}
		}

		public List<int> GuildCopyMapOrderList
		{
			get
			{
				return this.guildCopyMapOrderList;
			}
		}

		public void LoadGuildCopyMapOrder()
		{
			this.GuildCopyMapOrderList.Clear();
			this.GuildCopyMapOrderList.Add(this.FirstGuildCopyMapOrder);
			int key = this.FirstGuildCopyMapOrder;
			for (;;)
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(key, out systemXmlItem))
				{
					break;
				}
				if (null == systemXmlItem)
				{
					break;
				}
				int intValue = systemXmlItem.GetIntValue("DownCopyID", -1);
				if (intValue <= 0)
				{
					goto Block_3;
				}
				key = intValue;
				this.GuildCopyMapOrderList.Add(intValue);
			}
			return;
			Block_3:
			this.LastGuildCopyMapOrder = key;
		}

		public int LastGuildCopyMapOrder
		{
			get
			{
				return this.lastGuildCopyMapOrder;
			}
			set
			{
				this.lastGuildCopyMapOrder = value;
			}
		}

		public int MaxDamageSendCount
		{
			get
			{
				return this.maxDamageSendCount;
			}
		}

		public bool IsPrepareResetTime()
		{
			DateTime t = TimeUtil.NowDateTime();
			DayOfWeek dayOfWeek = t.DayOfWeek;
			bool result;
			if (dayOfWeek != DayOfWeek.Sunday)
			{
				result = false;
			}
			else
			{
				DateTime t2 = new DateTime(t.Year, t.Month, t.Day, 23, 55, 0);
				DateTime t3 = new DateTime(t.Year, t.Month, t.Day, 23, 56, 0);
				result = (t >= t2 && t <= t3);
			}
			return result;
		}

		public bool IsRefuseTime()
		{
			DateTime t = TimeUtil.NowDateTime();
			DayOfWeek dayOfWeek = t.DayOfWeek;
			bool result;
			if (dayOfWeek != DayOfWeek.Sunday)
			{
				result = false;
			}
			else
			{
				DateTime t2 = new DateTime(t.Year, t.Month, t.Day, 23, 55, 0);
				DateTime t3 = new DateTime(t.Year, t.Month, t.Day, 23, 59, 59);
				result = (t >= t2 && t <= t3);
			}
			return result;
		}

		public bool IsGuildCopyMap(int fubenID)
		{
			return this.GuildCopyMapOrderList.IndexOf(fubenID) >= 0;
		}

		public int GetGuildCopyMapIndex(int fubenID)
		{
			return this.GuildCopyMapOrderList.IndexOf(fubenID);
		}

		public int GetNextGuildCopyMapIndex(int fubenID)
		{
			int result;
			if (fubenID == this.LastGuildCopyMapOrder)
			{
				result = -1;
			}
			else
			{
				int guildCopyMapIndex = this.GetGuildCopyMapIndex(fubenID);
				if (guildCopyMapIndex < 0)
				{
					result = -1;
				}
				else
				{
					result = this.GetGuildCopyMapOrderByIndex(guildCopyMapIndex + 1);
				}
			}
			return result;
		}

		public int GetGuildCopyMapOrderByIndex(int index)
		{
			int result;
			if (index < 0 || index >= this.GuildCopyMapOrderList.Count)
			{
				result = -1;
			}
			else
			{
				result = this.GuildCopyMapOrderList[index];
			}
			return result;
		}

		public bool GetGuildCopyMapAwardDayFlag(int Flag, int day, int index)
		{
			return (Flag >> day * 2 & index) == index;
		}

		public int SetGuildCopyMapAwardDayFlag(int Flag, int day, int index)
		{
			return Flag | index << day * 2;
		}

		public void UpdateGuildCopyMap(int guildid, int fubenid, int seqid, int mapcode)
		{
			GuildCopyMap copyMap = new GuildCopyMap
			{
				GuildID = guildid,
				FuBenID = fubenid,
				SeqID = seqid,
				MapCode = mapcode
			};
			this.UpdateGuildCopyMap(guildid, copyMap);
		}

		public void UpdateGuildCopyMap(int guildid, GuildCopyMap CopyMap)
		{
			lock (this.GuildCopyMapDict)
			{
				this.GuildCopyMapDict[guildid] = CopyMap;
			}
		}

		public GuildCopyMap FindGuildCopyMap(int guildid)
		{
			GuildCopyMap result = null;
			lock (this.GuildCopyMapDict)
			{
				if (this.GuildCopyMapDict.ContainsKey(guildid))
				{
					result = this.GuildCopyMapDict[guildid];
				}
			}
			return result;
		}

		public GuildCopyMap FindActiveGuildCopyMap()
		{
			GuildCopyMap result = null;
			lock (this.GuildCopyMapDict)
			{
				using (Dictionary<int, GuildCopyMap>.Enumerator enumerator = this.GuildCopyMapDict.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, GuildCopyMap> keyValuePair = enumerator.Current;
						return keyValuePair.Value;
					}
				}
			}
			return result;
		}

		public GuildCopyMap FindGuildCopyMapBySeqID(int seqid)
		{
			lock (this.GuildCopyMapDict)
			{
				foreach (KeyValuePair<int, GuildCopyMap> keyValuePair in this.GuildCopyMapDict)
				{
					if (seqid == keyValuePair.Value.SeqID)
					{
						return keyValuePair.Value;
					}
				}
			}
			return null;
		}

		public void RemoveGuildCopyMap(int guildid)
		{
			this.GuildCopyMapDict.Remove(guildid);
		}

		public void CheckCurrGuildCopyMap(GameClient client, out int fubenid, out int seqid, int mapcode)
		{
			fubenid = -1;
			seqid = -1;
			int faction = client.ClientData.Faction;
			GuildCopyMapDB guildCopyMapDB = GameManager.GuildCopyMapDBMgr.FindGuildCopyMapDB(faction, client.ServerId);
			if (null != guildCopyMapDB)
			{
				DateTime realDate = Global.GetRealDate(guildCopyMapDB.OpenDay);
				if (Global.BeginOfWeek(realDate) != Global.BeginOfWeek(TimeUtil.NowDateTime()))
				{
					GameManager.GuildCopyMapDBMgr.ResetGuildCopyMapDB(faction, client.ServerId);
					fubenid = this.FirstGuildCopyMapOrder;
				}
				else if (guildCopyMapDB.FuBenID >= this.LastGuildCopyMapOrder && guildCopyMapDB.State == 2)
				{
					fubenid = 0;
				}
				else if (guildCopyMapDB.State == 2)
				{
					guildCopyMapDB.FuBenID = this.GetNextGuildCopyMapIndex(guildCopyMapDB.FuBenID);
					guildCopyMapDB.State = 0;
					guildCopyMapDB.OpenDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (GameManager.GuildCopyMapDBMgr.UpdateGuildCopyMapDB(guildCopyMapDB, client.ServerId))
					{
						fubenid = guildCopyMapDB.FuBenID;
					}
					this.UpdateGuildCopyMap(faction, fubenid, -1, -1);
				}
				else
				{
					fubenid = guildCopyMapDB.FuBenID;
					GuildCopyMap guildCopyMap = this.FindGuildCopyMap(faction);
					if (null != guildCopyMap)
					{
						seqid = guildCopyMap.SeqID;
					}
				}
			}
		}

		public void EnterGuildCopyMap(GameClient client, out int fubenid, out int seqid, int mapcode)
		{
			fubenid = -1;
			seqid = -1;
			int faction = client.ClientData.Faction;
			lock (this.GuildCopyMapDict)
			{
				this.CheckCurrGuildCopyMap(client, out fubenid, out seqid, mapcode);
				if (seqid < 0)
				{
					string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (array != null && array.Length >= 2)
					{
						seqid = Global.SafeConvertToInt32(array[1]);
						if (seqid > 0)
						{
							this.UpdateGuildCopyMap(faction, fubenid, seqid, mapcode);
						}
					}
				}
			}
		}

		public void ProcessMonsterDead(GameClient client, Monster monster)
		{
			if (this.IsGuildCopyMap(monster.CurrentMapCode))
			{
				SystemXmlItem systemXmlItem = null;
				if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(monster.CurrentMapCode, out systemXmlItem))
				{
					if (null != systemXmlItem)
					{
						int intValue = systemXmlItem.GetIntValue("BossID", -1);
						if (intValue == monster.MonsterInfo.ExtensionID)
						{
							CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(monster.CurrentCopyMapID);
							if (null == copyMap)
							{
								LogManager.WriteLog(2, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == copyMap), CurrentCopyMapID={0}", monster.CurrentCopyMapID), null, true);
							}
							else
							{
								GuildCopyMap guildCopyMap = GameManager.GuildCopyMapMgr.FindGuildCopyMapBySeqID(copyMap.FuBenSeqID);
								if (null == guildCopyMap)
								{
									LogManager.WriteLog(2, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == mapData), copyMap.FuBenSeqID={0}", copyMap.FuBenSeqID), null, true);
								}
								else
								{
									int guildID = guildCopyMap.GuildID;
									GuildCopyMapDB guildCopyMapDB = GameManager.GuildCopyMapDBMgr.FindGuildCopyMapDB(guildID, client.ServerId);
									if (null == guildCopyMapDB)
									{
										LogManager.WriteLog(2, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == data), guildid={0}", client.ClientData.Faction), null, true);
									}
									else
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList == null || clientsList.Count <= 0)
										{
											LogManager.WriteLog(2, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == objsList || objsList.Count <= 0), CurrentCopyMapID={0}", monster.CurrentCopyMapID), null, true);
										}
										else
										{
											if (copyMap.FubenMapID >= guildCopyMapDB.FuBenID)
											{
												guildCopyMapDB.FuBenID = copyMap.FubenMapID;
												guildCopyMapDB.State = 2;
												if (copyMap.FubenMapID == GameManager.GuildCopyMapMgr.FirstGuildCopyMapOrder)
												{
													guildCopyMapDB.Killers = monster.WhoKillMeName;
												}
												else
												{
													GuildCopyMapDB guildCopyMapDB2 = guildCopyMapDB;
													guildCopyMapDB2.Killers += ",";
													GuildCopyMapDB guildCopyMapDB3 = guildCopyMapDB;
													guildCopyMapDB3.Killers += monster.WhoKillMeName;
												}
											}
											GlobalEventSource.getInstance().fireEvent(ZhanMengShijianEvent.createKillBossEvent(Global.FormatRoleName4(client), client.ClientData.Faction, monster.MonsterInfo.ExtensionID, client.ServerId));
											if (!GameManager.GuildCopyMapDBMgr.UpdateGuildCopyMapDB(guildCopyMapDB, client.ServerId))
											{
												string format = "GuildCopyMapManager::ProcessMonsterDead (false == result), \r\n                        data.GuildID={0}, data.FuBenID={1}, data.State={2}, data.OpenDay={3}, data.Killers={4}";
												LogManager.WriteLog(2, string.Format(format, new object[]
												{
													guildCopyMapDB.GuildID,
													guildCopyMapDB.FuBenID,
													guildCopyMapDB.State,
													guildCopyMapDB.OpenDay,
													guildCopyMapDB.Killers
												}), null, true);
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

		public int GetZhanGongAward(GameClient client, int fubenid, int awardZhanGong)
		{
			int result;
			if (!this.IsGuildCopyMap(fubenid))
			{
				result = 0;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardDay");
				DateTime realDate = Global.GetRealDate(roleParamsInt32FromDB);
				if (Global.BeginOfWeek(realDate) != Global.BeginOfWeek(TimeUtil.NowDateTime()))
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", 0, true);
				}
				int num = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardFlag");
				bool guildCopyMapAwardDayFlag = this.GetGuildCopyMapAwardDayFlag(num, this.GetGuildCopyMapIndex(fubenid), 1);
				if (guildCopyMapAwardDayFlag)
				{
					result = -1;
				}
				else
				{
					num = this.SetGuildCopyMapAwardDayFlag(num, this.GetGuildCopyMapIndex(fubenid), 1);
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", num, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardDay", Global.GetOffsetDay(TimeUtil.NowDateTime()), true);
					result = awardZhanGong;
				}
			}
			return result;
		}

		private const int firstGuildCopyMapOrder = 40000;

		private Dictionary<int, GuildCopyMap> GuildCopyMapDict = new Dictionary<int, GuildCopyMap>();

		private List<int> guildCopyMapOrderList = new List<int>();

		private int lastGuildCopyMapOrder = 40006;

		private int maxDamageSendCount = 5;

		public long lastProcessEndTicks = 0L;

		public bool ProcessEndFlag = false;
	}
}
