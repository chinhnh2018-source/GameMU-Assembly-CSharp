using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	public class PreDeleteRoleMgr
	{
		public void LoadPreDeleteRoleFromDB(DBManager dbMgr)
		{
			DBQuery.QueryPreDeleteRoleDict(dbMgr, this._PreDeleteRoleDict);
		}

		public void AddPreDeleteRole(int rid, DateTime tm)
		{
			lock (this._PreDeleteRoleDict)
			{
				this._PreDeleteRoleDict[rid] = tm;
			}
		}

		public bool RemovePreDeleteRole(DBUserInfo dbUserInfo, DBRoleInfo dbRoleInfo)
		{
			bool result;
			lock (this._PreDeleteRoleDict)
			{
				string userID = dbUserInfo.UserID;
				int roleID = dbRoleInfo.RoleID;
				DBManager instance = DBManager.getInstance();
				bool flag2 = false;
				bool userRole = DBQuery.GetUserRole(instance, userID, roleID);
				if (userRole)
				{
					flag2 = DBWriter.UnPreRemoveRole(instance, roleID);
				}
				if (!flag2)
				{
					result = false;
				}
				else
				{
					this._PreDeleteRoleDict.Remove(roleID);
					lock (dbUserInfo)
					{
						int num = dbUserInfo.ListRoleIDs.IndexOf(roleID);
						if (num >= 0 && num < dbUserInfo.ListRoleIDs.Count)
						{
							dbUserInfo.ListRolePreRemoveTime[num] = "";
						}
					}
					result = true;
				}
			}
			return result;
		}

		public bool IfInPreDeleteState(int rid)
		{
			bool result;
			lock (this._PreDeleteRoleDict)
			{
				result = this._PreDeleteRoleDict.ContainsKey(rid);
			}
			return result;
		}

		public int CalcPreDeleteRoleLeftSeconds(string PreRemoveTime)
		{
			int num = -1;
			int result;
			if (string.IsNullOrEmpty(PreRemoveTime))
			{
				result = num;
			}
			else
			{
				DateTime d = DateTime.Parse(PreRemoveTime);
				num = GameDBManager.PreDeleteRoleDelaySeconds - (int)(TimeUtil.NowDateTime() - d).TotalSeconds;
				if (num < 0)
				{
					num = 0;
				}
				result = num;
			}
			return result;
		}

		public void UpdatePreDeleteRole()
		{
			lock (this._PreDeleteRoleDict)
			{
				DBManager instance = DBManager.getInstance();
				List<PreDeleteRoleInfo> list = new List<PreDeleteRoleInfo>();
				foreach (KeyValuePair<int, DateTime> keyValuePair in this._PreDeleteRoleDict)
				{
					if ((TimeUtil.NowDateTime() - keyValuePair.Value).TotalSeconds >= (double)GameDBManager.PreDeleteRoleDelaySeconds)
					{
						int key = keyValuePair.Key;
						PreDeleteRoleInfo preDeleteRoleInfo = new PreDeleteRoleInfo();
						preDeleteRoleInfo.RoleID = key;
						DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref key);
						if (null != dbroleInfo)
						{
							preDeleteRoleInfo.UserID = dbroleInfo.UserID;
							preDeleteRoleInfo.ZoneID = dbroleInfo.ZoneID;
							bool flag2 = false;
							bool userRole = DBQuery.GetUserRole(instance, dbroleInfo.UserID, key);
							if (userRole)
							{
								flag2 = DBWriter.RemoveRole(instance, key);
							}
							DBUserInfo dbuserInfo = instance.GetDBUserInfo(dbroleInfo.UserID);
							if (flag2 && null != dbuserInfo)
							{
								list.Add(preDeleteRoleInfo);
								this.HandleDeleteRole(dbuserInfo, dbroleInfo);
							}
						}
					}
				}
				foreach (PreDeleteRoleInfo preDeleteRoleInfo2 in list)
				{
					string gmCmd = string.Format("-deleterole {0} {1} {2}", preDeleteRoleInfo2.UserID, preDeleteRoleInfo2.RoleID, preDeleteRoleInfo2.ZoneID);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
					this._PreDeleteRoleDict.Remove(preDeleteRoleInfo2.RoleID);
				}
			}
		}

		public void HandleDeleteRole(DBUserInfo dbUserInfo, DBRoleInfo dbRoleInfo)
		{
			DBManager instance = DBManager.getInstance();
			string userID = dbUserInfo.UserID;
			int roleID = dbRoleInfo.RoleID;
			lock (dbUserInfo)
			{
				if (dbRoleInfo.Faction > 0 && dbRoleInfo.BHZhiWu == 1)
				{
					int num = -1;
					bool flag2 = false;
					List<BangHuiMemberData> bangHuiMemberDataList = DBQuery.GetBangHuiMemberDataList(instance, dbRoleInfo.Faction);
					List<BangHuiMgrItemData> bangHuiMgrItemItemDataList = DBQuery.GetBangHuiMgrItemItemDataList(instance, dbRoleInfo.Faction);
					if (bangHuiMemberDataList != null && bangHuiMemberDataList.Count > 0)
					{
						if (bangHuiMgrItemItemDataList != null)
						{
							num = Global.GetDBRoleInfoByZhiWu(bangHuiMgrItemItemDataList, 2);
							if (num <= 0)
							{
								num = Global.GetDBRoleInfoByZhiWu(bangHuiMgrItemItemDataList, 3);
								if (num <= 0)
								{
									num = Global.GetDBRoleInfoByZhiWu(bangHuiMgrItemItemDataList, 4);
									if (num <= 0)
									{
										flag2 = true;
									}
								}
							}
						}
						if (flag2)
						{
							for (int i = 0; i < bangHuiMemberDataList.Count; i++)
							{
								if (bangHuiMemberDataList[i].RoleID != roleID)
								{
									num = bangHuiMemberDataList[i].RoleID;
									break;
								}
							}
						}
						if (num > 0)
						{
							lock (Global.BangHuiMutex)
							{
								DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref num);
								if (dbroleInfo != null)
								{
									if (dbroleInfo.Faction == dbRoleInfo.Faction)
									{
										dbroleInfo.BHZhiWu = 1;
										DBWriter.UpdateBangHuiMemberZhiWu(instance, dbroleInfo.Faction, num, 1);
										DBWriter.UpdateBangHuiRoleID(instance, num, dbroleInfo.Faction);
										int serverLineID = dbRoleInfo.ServerLineID;
										string chatMsg = string.Format("0::0::0:-chbhzhiwu {0} {1} {2} {3}:0:0:-1", new object[]
										{
											dbRoleInfo.Faction,
											num,
											1,
											dbRoleInfo.RoleID
										});
										List<LineItem> lineItemList = LineManager.GetLineItemList();
										if (null != lineItemList)
										{
											for (int i = 0; i < lineItemList.Count; i++)
											{
												if (lineItemList[i].LineID != serverLineID)
												{
													ChatMsgManager.AddChatMsg(lineItemList[i].LineID, chatMsg);
												}
											}
										}
									}
								}
							}
						}
						BangHuiDestroyMgr.ClearBangHuiLingDi(instance, dbRoleInfo.Faction);
					}
					else
					{
						BangHuiDestroyMgr.DoDestroyBangHui(instance, dbRoleInfo.Faction);
					}
				}
				dbRoleInfo.Faction = 0;
				dbRoleInfo.BHName = "";
				dbRoleInfo.BHZhiWu = 0;
				dbRoleInfo.BangGong = 0;
				DBWriter.UpdateRoleBangHuiInfo(instance, dbRoleInfo.RoleID, dbRoleInfo.Faction, dbRoleInfo.BHName, 0);
				int num2 = dbUserInfo.ListRoleIDs.IndexOf(roleID);
				if (num2 >= 0 && num2 < dbUserInfo.ListRoleIDs.Count)
				{
					dbUserInfo.ListRoleIDs.RemoveAt(num2);
					dbUserInfo.ListRoleSexes.RemoveAt(num2);
					dbUserInfo.ListRoleOccups.RemoveAt(num2);
					dbUserInfo.ListRoleNames.RemoveAt(num2);
					dbUserInfo.ListRoleLevels.RemoveAt(num2);
					dbUserInfo.ListRoleZoneIDs.RemoveAt(num2);
					dbUserInfo.ListRoleChangeLifeCount.RemoveAt(num2);
					dbUserInfo.ListRolePreRemoveTime.RemoveAt(num2);
				}
			}
		}

		private Dictionary<int, DateTime> _PreDeleteRoleDict = new Dictionary<int, DateTime>();
	}
}
