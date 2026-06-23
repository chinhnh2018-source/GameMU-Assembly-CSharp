using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Tmsk.Tools.Tools;

namespace GameDBServer.Logic
{
	public class BangHuiDestroyMgr
	{
		public static void ProcessDestroyBangHui(DBManager dbMgr)
		{
			DateTime now = DateTime.Now;
			int dayOfYear = now.DayOfYear;
			DayOfWeek dayOfWeek = now.DayOfWeek;
			int num = now.Hour * 60 + now.Minute;
			if (dayOfYear != BangHuiDestroyMgr.LastCheckDestroyDayID)
			{
				BangHuiDestroyMgr.LastCheckDestroyDayID = dayOfYear;
				BangHuiDestroyMgr.LastCheckDestroyTimer = num;
			}
			else if (num >= BangHuiDestroyMgr.DestroyTimer && BangHuiDestroyMgr.LastCheckDestroyTimer < BangHuiDestroyMgr.DestroyTimer && dayOfWeek == DayOfWeek.Sunday)
			{
				BangHuiDestroyMgr.LastCheckDestroyDayID = dayOfYear;
				BangHuiDestroyMgr.LastCheckDestroyTimer = num;
				BangHuiDestroyMgr.HandleDestroyBangHuis(dbMgr);
			}
		}

		private static void HandleDestroyBangHuis(DBManager dbMgr)
		{
			int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-per-qilevel", 10000);
			if (gameConfigItemInt > 0)
			{
				DBWriter.SubBangHuiTongQianByQiLevel(dbMgr, gameConfigItemInt);
				int gameConfigItemInt2 = GameDBManager.GameConfigMgr.GetGameConfigItemInt("juntuanbanghuimax", 8);
				string gameConfigItemStr = GameDBManager.GameConfigMgr.GetGameConfigItemStr("bhmatch_goldjoin", "");
				List<int> list = ConfigHelper.String2IntList(gameConfigItemStr, '|');
				List<int> noMoneyBangHuiList = DBQuery.GetNoMoneyBangHuiList(dbMgr, gameConfigItemInt2);
				for (int i = 0; i < noMoneyBangHuiList.Count; i++)
				{
					int bhid = noMoneyBangHuiList[i];
					if (!list.Exists((int x) => x == bhid))
					{
						BangHuiDestroyMgr.DoDestroyBangHui(dbMgr, bhid);
						string gmCmd = string.Format("-autodestroybh {0}", bhid);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
					}
				}
			}
		}

		public static void DoDestroyBangHui(DBManager dbMgr, int bhid)
		{
			lock (Global.BangHuiMutex)
			{
				DBWriter.DeleteBangHui(dbMgr, bhid);
				GameDBManager.BangHuiJunQiMgr.RemoveBangHuiJunQi(bhid);
				DBWriter.ClearAllRoleBangHuiInfo(dbMgr, bhid);
				List<DBRoleInfo> cachingDBRoleInfoListByFaction = dbMgr.DBRoleMgr.GetCachingDBRoleInfoListByFaction(bhid);
				if (null != cachingDBRoleInfoListByFaction)
				{
					for (int i = 0; i < cachingDBRoleInfoListByFaction.Count; i++)
					{
						cachingDBRoleInfoListByFaction[i].Faction = 0;
						cachingDBRoleInfoListByFaction[i].BHName = "";
						cachingDBRoleInfoListByFaction[i].BHZhiWu = 0;
					}
				}
			}
			DBWriter.ClearBHLingDiByID(dbMgr, bhid);
			GameDBManager.BangHuiLingDiMgr.ClearBangHuiLingDi(bhid);
			ZhanMengShiJianManager.getInstance().onZhanMengJieSan(bhid);
			string gmCmd = string.Format("-synclingdi", new object[0]);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
		}

		public static void ClearBangHuiLingDi(DBManager dbMgr, int bhid)
		{
			lock (Global.BangHuiMutex)
			{
				GameDBManager.BangHuiJunQiMgr.RemoveBangHuiJunQi(bhid);
			}
			DBWriter.ClearBHLingDiByID(dbMgr, bhid);
			GameDBManager.BangHuiLingDiMgr.ClearBangHuiLingDi(bhid);
			string gmCmd = string.Format("-synclingdi", new object[0]);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
		}

		private static int LastCheckDestroyDayID = DateTime.Now.DayOfYear;

		private static int LastCheckDestroyTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		private static int DestroyTimer = 21;
	}
}
