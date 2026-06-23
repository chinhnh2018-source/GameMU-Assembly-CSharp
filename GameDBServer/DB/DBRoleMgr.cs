using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	public class DBRoleMgr
	{
		public int GetRoleInfoCount()
		{
			int count;
			lock (this.DictRoleInfos)
			{
				count = this.DictRoleInfos.Count;
			}
			return count;
		}

		public DBRoleInfo FindDBRoleInfo(ref int roleID)
		{
			if (roleID < 200000)
			{
				int tempRoleID = roleID;
				roleID = SingletonTemplate<RoleMapper>.Instance().GetLocalRoleIDByTempID(tempRoleID);
			}
			DBRoleInfo result;
			if (roleID <= 0)
			{
				result = null;
			}
			else
			{
				DBRoleInfo dbroleInfo = null;
				MyWeakReference myWeakReference = null;
				lock (this.DictRoleInfos)
				{
					if (this.DictRoleInfos.Count > 0)
					{
						if (this.DictRoleInfos.TryGetValue(roleID, out myWeakReference))
						{
							if (myWeakReference.IsAlive)
							{
								dbroleInfo = (myWeakReference.Target as DBRoleInfo);
							}
						}
					}
				}
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						dbroleInfo.LastReferenceTicks = DateTime.Now.Ticks / 10000L;
					}
				}
				result = dbroleInfo;
			}
			return result;
		}

		public int FindDBRoleID(string roleName)
		{
			int num = -1;
			int result;
			if (!this.DictRoleName2ID.TryGetValue(roleName, out num))
			{
				result = -1;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public DBRoleInfo AddDBRoleInfo(DBRoleInfo dbRoleInfo)
		{
			MyWeakReference myWeakReference = null;
			lock (this.DictRoleInfos)
			{
				if (this.DictRoleInfos.TryGetValue(dbRoleInfo.RoleID, out myWeakReference))
				{
					DBRoleInfo dbroleInfo = myWeakReference.Target as DBRoleInfo;
					if (null != dbroleInfo)
					{
						return dbroleInfo;
					}
					myWeakReference.Target = dbRoleInfo;
				}
				else
				{
					this.DictRoleInfos.Add(dbRoleInfo.RoleID, new MyWeakReference(dbRoleInfo));
				}
			}
			lock (this.DictRoleName2ID)
			{
				string key = Global.FormatRoleName(dbRoleInfo);
				this.DictRoleName2ID[key] = dbRoleInfo.RoleID;
			}
			return dbRoleInfo;
		}

		public void RemoveDBRoleInfo(int roleID)
		{
			string text = null;
			MyWeakReference myWeakReference = null;
			lock (this.DictRoleInfos)
			{
				if (this.DictRoleInfos.TryGetValue(roleID, out myWeakReference))
				{
					text = Global.FormatRoleName(myWeakReference.Target as DBRoleInfo);
					myWeakReference.Target = null;
				}
			}
			lock (this.DictRoleName2ID)
			{
				if (null != text)
				{
					this.DictRoleName2ID.Remove(text);
				}
			}
		}

		public void ClearAllDBroleInfo()
		{
			lock (this.DictRoleInfos)
			{
				this.DictRoleInfos.Clear();
			}
			lock (this.DictRoleName2ID)
			{
				this.DictRoleName2ID.Clear();
			}
		}

		public List<DBRoleInfo> GetCachingDBRoleInfoListByFaction(int faction)
		{
			List<DBRoleInfo> list = new List<DBRoleInfo>();
			lock (this.DictRoleInfos)
			{
				foreach (int key in this.DictRoleInfos.Keys)
				{
					MyWeakReference myWeakReference = this.DictRoleInfos[key];
					DBRoleInfo dbroleInfo = myWeakReference.Target as DBRoleInfo;
					if (dbroleInfo != null && dbroleInfo.Faction == faction)
					{
						list.Add(dbroleInfo);
					}
				}
			}
			return list;
		}

		public void ReleaseIdleDBRoleInfos(int ticksSlot)
		{
			long num = DateTime.Now.Ticks / 10000L;
			long num2 = TimeUtil.NOW() - (long)ticksSlot;
			Dictionary<DBRoleInfo, List<RoleParamsData>> dictionary = new Dictionary<DBRoleInfo, List<RoleParamsData>>();
			List<int> list = new List<int>();
			lock (this.DictRoleInfos)
			{
				foreach (MyWeakReference myWeakReference in this.DictRoleInfos.Values)
				{
					if (myWeakReference.IsAlive)
					{
						DBRoleInfo dbroleInfo = myWeakReference.Target as DBRoleInfo;
						if (null != dbroleInfo)
						{
							List<RoleParamsData> list2 = null;
							lock (dbroleInfo)
							{
								if (null != dbroleInfo.RoleParamsDict)
								{
									foreach (RoleParamsData roleParamsData in dbroleInfo.RoleParamsDict.Values)
									{
										if (roleParamsData.UpdateFaildTicks > 0L && num2 > roleParamsData.UpdateFaildTicks)
										{
											if (null == list2)
											{
												if (!dictionary.TryGetValue(dbroleInfo, out list2))
												{
													list2 = new List<RoleParamsData>();
													dictionary.Add(dbroleInfo, list2);
												}
											}
											list2.Add(roleParamsData);
										}
									}
								}
							}
							if (null == list2)
							{
								if (dbroleInfo.ServerLineID <= 0 && num - dbroleInfo.LastReferenceTicks >= (long)ticksSlot)
								{
									list.Add(dbroleInfo.RoleID);
								}
							}
						}
					}
				}
			}
			DBManager instance = DBManager.getInstance();
			foreach (KeyValuePair<DBRoleInfo, List<RoleParamsData>> keyValuePair in dictionary)
			{
				foreach (RoleParamsData roleParamsData2 in keyValuePair.Value)
				{
					Global.UpdateRoleParamByName(instance, keyValuePair.Key, roleParamsData2.ParamName, roleParamsData2.ParamValue, roleParamsData2.ParamType);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.RemoveDBRoleInfo(list[i]);
				LogManager.WriteLog(LogTypes.Info, string.Format("释放空闲的角色数据: {0}", list[i]), null, true);
			}
		}

		public void ReleaseDBRoleInfoByID(int roleID)
		{
			DBRoleInfo dbroleInfo = this.FindDBRoleInfo(ref roleID);
			if (null != dbroleInfo)
			{
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutEventObject(dbroleInfo));
				this.RemoveDBRoleInfo(dbroleInfo.RoleID);
				LogManager.WriteLog(LogTypes.SQL, string.Format("释放指定角色的数据: {0}", dbroleInfo.RoleID), null, true);
			}
		}

		public void LoadDBRoleInfos(DBManager dbMgr, MySQLConnection conn)
		{
		}

		public void OnChangeName(int roleId, int zoneId, string oldName, string newName)
		{
			lock (this.DictRoleName2ID)
			{
				string key = Global.FormatRoleName(zoneId, oldName);
				int value = 0;
				if (this.DictRoleName2ID.TryGetValue(key, out value))
				{
					this.DictRoleName2ID.Remove(key);
					this.DictRoleName2ID[Global.FormatRoleName(zoneId, newName)] = value;
				}
			}
		}

		private Dictionary<int, MyWeakReference> DictRoleInfos = new Dictionary<int, MyWeakReference>(10000);

		private Dictionary<string, int> DictRoleName2ID = new Dictionary<string, int>(10000);
	}
}
