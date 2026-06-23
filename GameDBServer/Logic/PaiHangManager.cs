using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Logic.MerlinMagicBook;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Logic.Wing;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class PaiHangManager
	{
		private static void LoadPaiHangLists(DBManager dbMgr)
		{
			PaiHangManager.RoleEquipPaiHangList = DBQuery.GetRoleEquipPaiHang(dbMgr);
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("paihang-jingmailevel", 0) > 0)
			{
				PaiHangManager.RoleXueWeiNumPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "JingMaiLevel");
			}
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("paihang-wuxuelevel", 0) > 0)
			{
				PaiHangManager.RoleSkillLevelPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "WuXueLevel");
			}
			PaiHangManager.RoleHorseJiFenPaiHangList = DBQuery.GetRoleHorseJiFenPaiHang(dbMgr);
			PaiHangManager.RoleLevelPaiHangList = DBQuery.GetRoleLevelPaiHang(dbMgr);
			PaiHangManager.RoleYinLiangPaiHangList = DBQuery.GetRoleYinLiangPaiHang(dbMgr);
			PaiHangManager.RoleLianZhanPaiHangList = DBQuery.GetRoleLianZhanPaiHang(dbMgr);
			PaiHangManager.RoleKillBossPaiHangList = DBQuery.GetRoleKillBossPaiHang(dbMgr);
			PaiHangManager.RoleBattleNumPaiHangList = DBQuery.GetRoleBattleNumPaiHang(dbMgr);
			PaiHangManager.RoleHeroIndexPaiHangList = DBQuery.GetRoleHeroIndexPaiHang(dbMgr);
			PaiHangManager.RoleGoldPaiHangList = DBQuery.GetRoleGoldPaiHang(dbMgr);
			PaiHangManager.CombatForcePaiHangList = DBQuery.GetRoleCombatForcePaiHang(dbMgr);
			PaiHangManager.UserMoneyPaiHangList = DBQuery.GetUserMoneyPaiHang(dbMgr);
			PaiHangManager.RoleChengJiuPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "ChengJiuLevel");
			PaiHangManager.RoleShengWangPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, "ShengWangLevel");
			PaiHangManager.RoleGuardStatuePaiHangList = DBQuery.GetRoleGuardStatuePaiHang(dbMgr);
			PaiHangManager.RoleHolyItemPaiHangList = DBQuery.GetRoleHolyItemPaiHang(dbMgr);
			PaiHangManager.StorePaiHangForHuoDong(dbMgr);
		}

		public static void ProcessPaiHang(DBManager dbMgr, bool force = false)
		{
			if (force)
			{
				PaiHangManager.LoadPaiHangLists(dbMgr);
			}
			else
			{
				DateTime now = DateTime.Now;
				long num = now.Ticks / 10000L;
				if (num - PaiHangManager.LastUpdatePaiHangTickTimer >= 1800000L)
				{
					PaiHangManager.LastUpdatePaiHangTickTimer = num;
					PaiHangManager.LoadPaiHangLists(dbMgr);
				}
				else
				{
					int dayOfYear = now.DayOfYear;
					if (dayOfYear != PaiHangManager.LastCheckPaiHangDayID)
					{
						PaiHangManager.LastCheckPaiHangDayID = dayOfYear;
						string gmCmd = string.Format("-updatepaihangbang", new object[0]);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
					}
				}
			}
		}

		public static PaiHangData GetPaiHangData(int paiHangType, int pageShowNum = -1)
		{
			List<PaiHangItemData> paiHangList = null;
			switch (paiHangType)
			{
			case 1:
				paiHangList = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				paiHangList = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				paiHangList = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				paiHangList = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				paiHangList = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				paiHangList = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				paiHangList = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				paiHangList = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				paiHangList = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				paiHangList = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				paiHangList = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				paiHangList = PaiHangManager.CombatForcePaiHangList;
				break;
			case 13:
				paiHangList = JingJiChangManager.getInstance().getRankingList(0);
				break;
			case 14:
				paiHangList = WanMoTaManager.getInstance().getRankingList(0);
				break;
			case 15:
				paiHangList = WingPaiHangManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 16:
				paiHangList = RingPaiHangManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 17:
				paiHangList = MerlinRankManager.getInstance().getRankingList(0, pageShowNum);
				break;
			case 18:
				paiHangList = PaiHangManager.UserMoneyPaiHangList;
				break;
			case 19:
				paiHangList = PaiHangManager.RoleChengJiuPaiHangList;
				break;
			case 20:
				paiHangList = PaiHangManager.RoleShengWangPaiHangList;
				break;
			case 21:
				paiHangList = PaiHangManager.RoleGuardStatuePaiHangList;
				break;
			case 22:
				paiHangList = PaiHangManager.RoleHolyItemPaiHangList;
				break;
			}
			return new PaiHangData
			{
				PaiHangType = paiHangType,
				PaiHangList = paiHangList
			};
		}

		protected static void StorePaiHangForHuoDong(DBManager dbMgr)
		{
			PaiHangManager.StorePaiHangPos(dbMgr, 5, 5, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 8, 6, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 3, 7, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 2, 8, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 8, 36, 10);
			PaiHangManager.StorePaiHangPos(dbMgr, 5, 33, 10);
		}

		protected static void StorePaiHangPos(DBManager dbMgr, int paiHangType, int huoDongType, int maxPaiHang = 10)
		{
			List<PaiHangItemData> list = null;
			switch (paiHangType)
			{
			case 1:
				list = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				list = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				list = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				list = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				list = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				list = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				list = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				list = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				list = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				list = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				list = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				list = PaiHangManager.CombatForcePaiHangList;
				break;
			}
			if (null != list)
			{
				string paihangtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				int num = 0;
				while (num < list.Count && num < maxPaiHang)
				{
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref list[num].RoleID);
					if (null != dbroleInfo)
					{
						int val = list[num].Val1;
						DBWriter.AddHongDongPaiHangRecord(dbMgr, list[num].RoleID, dbroleInfo.RoleName, dbroleInfo.ZoneID, huoDongType, num + 1, paihangtime, val);
					}
					num++;
				}
			}
		}

		public static int GetPaiHangPosByRoleID(int paiHangType, int roleID)
		{
			List<PaiHangItemData> list = null;
			switch (paiHangType)
			{
			case 1:
				list = PaiHangManager.RoleEquipPaiHangList;
				break;
			case 2:
				list = PaiHangManager.RoleXueWeiNumPaiHangList;
				break;
			case 3:
				list = PaiHangManager.RoleSkillLevelPaiHangList;
				break;
			case 4:
				list = PaiHangManager.RoleHorseJiFenPaiHangList;
				break;
			case 5:
				list = PaiHangManager.RoleLevelPaiHangList;
				break;
			case 6:
				list = PaiHangManager.RoleYinLiangPaiHangList;
				break;
			case 7:
				list = PaiHangManager.RoleLianZhanPaiHangList;
				break;
			case 8:
				list = PaiHangManager.RoleKillBossPaiHangList;
				break;
			case 9:
				list = PaiHangManager.RoleBattleNumPaiHangList;
				break;
			case 10:
				list = PaiHangManager.RoleHeroIndexPaiHangList;
				break;
			case 11:
				list = PaiHangManager.RoleGoldPaiHangList;
				break;
			case 12:
				list = PaiHangManager.CombatForcePaiHangList;
				break;
			case 19:
				list = PaiHangManager.RoleChengJiuPaiHangList;
				break;
			case 20:
				list = PaiHangManager.RoleShengWangPaiHangList;
				break;
			case 21:
				list = PaiHangManager.RoleGuardStatuePaiHangList;
				break;
			case 22:
				list = PaiHangManager.RoleHolyItemPaiHangList;
				break;
			}
			int result;
			if (null == list)
			{
				result = -1;
			}
			else
			{
				int num = 0;
				while (num < list.Count && num < 10)
				{
					if (list[num].RoleID == roleID)
					{
						return num;
					}
					num++;
				}
				result = -1;
			}
			return result;
		}

		public static Dictionary<int, int> CalcPaiHangPosDictRoleID(int roleID)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 1; i < 23; i++)
			{
				dictionary[i] = PaiHangManager.GetPaiHangPosByRoleID(i, roleID);
			}
			return dictionary;
		}

		public static void OnChangeName(int roleid, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				Action<List<PaiHangItemData>> action = delegate(List<PaiHangItemData> _itemList)
				{
					if (_itemList != null)
					{
						try
						{
							List<PaiHangItemData> list = _itemList.FindAll((PaiHangItemData _item) => _item.RoleID == roleid);
							if (list != null)
							{
								foreach (PaiHangItemData paiHangItemData in list)
								{
									paiHangItemData.RoleName = newName;
								}
							}
						}
						catch (Exception)
						{
						}
					}
				};
				action(PaiHangManager.RoleEquipPaiHangList);
				action(PaiHangManager.RoleXueWeiNumPaiHangList);
				action(PaiHangManager.RoleSkillLevelPaiHangList);
				action(PaiHangManager.RoleHorseJiFenPaiHangList);
				action(PaiHangManager.RoleLevelPaiHangList);
				action(PaiHangManager.RoleYinLiangPaiHangList);
				action(PaiHangManager.RoleLianZhanPaiHangList);
				action(PaiHangManager.RoleKillBossPaiHangList);
				action(PaiHangManager.RoleBattleNumPaiHangList);
				action(PaiHangManager.RoleHeroIndexPaiHangList);
				action(PaiHangManager.RoleGoldPaiHangList);
				action(PaiHangManager.CombatForcePaiHangList);
				action(PaiHangManager.RoleChengJiuPaiHangList);
				action(PaiHangManager.RoleShengWangPaiHangList);
				PaiHangManager._UpdateName_t_huodongpaihang(roleid, oldName, newName);
				JingJiChangManager.getInstance().OnChangeName(roleid, oldName, newName);
				WanMoTaManager.getInstance().OnChangeName(roleid, oldName, newName);
				WingPaiHangManager.getInstance().OnChangeName(roleid, oldName, newName);
				RingPaiHangManager.getInstance().OnChangeName(roleid, oldName, newName);
				MerlinRankManager.getInstance().OnChangeName(roleid, oldName, newName);
			}
		}

		private static void _UpdateName_t_huodongpaihang(int roleid, string oldName, string newName)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_huodongpaihang SET rname='{0}' WHERE rid={1}", newName, roleid);
				if (!myDbConnection.ExecuteNonQueryBool(sql, 0))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名，更新t_huodongpaihang失败, roleId={0}, oldName={1}, newName={2}", roleid, oldName, newName), null, true);
				}
			}
		}

		private const long UpdatePaiHangIntervalTimer = 1800000L;

		private static int LastCheckPaiHangDayID = DateTime.Now.DayOfYear;

		private static int LastCheckPaiHangTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		private static int PaiHangTimer = 420;

		private static long LastUpdatePaiHangTickTimer = 0L;

		private static List<PaiHangItemData> RoleEquipPaiHangList = null;

		private static List<PaiHangItemData> RoleXueWeiNumPaiHangList = null;

		private static List<PaiHangItemData> RoleSkillLevelPaiHangList = null;

		private static List<PaiHangItemData> RoleHorseJiFenPaiHangList = null;

		private static List<PaiHangItemData> RoleLevelPaiHangList = null;

		private static List<PaiHangItemData> RoleYinLiangPaiHangList = null;

		private static List<PaiHangItemData> RoleLianZhanPaiHangList = null;

		private static List<PaiHangItemData> RoleKillBossPaiHangList = null;

		private static List<PaiHangItemData> RoleBattleNumPaiHangList = null;

		private static List<PaiHangItemData> RoleHeroIndexPaiHangList = null;

		private static List<PaiHangItemData> RoleGoldPaiHangList = null;

		private static List<PaiHangItemData> UserMoneyPaiHangList = null;

		private static List<PaiHangItemData> RoleChengJiuPaiHangList = null;

		private static List<PaiHangItemData> RoleShengWangPaiHangList = null;

		private static List<PaiHangItemData> RoleGuardStatuePaiHangList = null;

		private static List<PaiHangItemData> RoleHolyItemPaiHangList = null;

		private static List<PaiHangItemData> CombatForcePaiHangList = null;
	}
}
