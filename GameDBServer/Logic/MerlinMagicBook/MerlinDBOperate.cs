using System;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	public class MerlinDBOperate
	{
		public static bool InsertMerlinData(DBManager dbMgr, MerlinGrowthSaveDBData merlinData, string addTime)
		{
			bool result;
			if (null == merlinData)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string text = new DateTime(merlinData._ToTicks * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string sql = string.Format("INSERT INTO t_merlin_magic_book(roleID, occupation, level, level_up_fail_num, starNum, starExp, luckyPoint, toTicks, addTime, activeFrozen, activePalsy, activeSpeedDown, activeBlow, unActiveFrozen, unActivePalsy, unActiveSpeedDown, unActiveBlow) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}', '{8}', {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16})", new object[]
					{
						merlinData._RoleID,
						merlinData._Occupation,
						merlinData._Level,
						merlinData._LevelUpFailNum,
						merlinData._StarNum,
						merlinData._StarExp,
						merlinData._LuckyPoint,
						text,
						addTime,
						merlinData._ActiveAttr[0] * 100.0,
						merlinData._ActiveAttr[1] * 100.0,
						merlinData._ActiveAttr[2] * 100.0,
						merlinData._ActiveAttr[3] * 100.0,
						merlinData._UnActiveAttr[0] * 100.0,
						merlinData._UnActiveAttr[1] * 100.0,
						merlinData._UnActiveAttr[2] * 100.0,
						merlinData._UnActiveAttr[3] * 100.0
					});
					flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
				result = flag;
			}
			return result;
		}

		public static bool UpdateMerlinData(DBManager dbMgr, int nRoleID, string[] fields, int nStartIndex)
		{
			bool result;
			if (fields == null || fields.Length != 15 || nStartIndex >= fields.Length)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					if (fields[6] != "*")
					{
						string text = new DateTime(Convert.ToInt64(fields[6]) * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
						fields[6] = text;
					}
					for (int i = 7; i <= 14; i++)
					{
						if (fields[i] != "*")
						{
							fields[i] = (Convert.ToDouble(fields[i]) * 100.0).ToString();
						}
					}
					string text2 = DBWriter.FormatUpdateSQL(nRoleID, fields, nStartIndex, MerlinDBOperate.t_fieldNames, "t_merlin_magic_book", MerlinDBOperate.t_fieldTypes, "roleID");
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
					MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
					try
					{
						mySQLCommand.ExecuteNonQuery();
						flag = true;
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text2), null, true);
					}
					mySQLCommand.Dispose();
					mySQLCommand = null;
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = flag;
			}
			return result;
		}

		public static MerlinGrowthSaveDBData QueryMerlinData(DBManager dbMgr, int nRoleID)
		{
			MySQLConnection mySQLConnection = null;
			MerlinGrowthSaveDBData merlinGrowthSaveDBData = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT roleID, (SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName, occupation, level, level_up_fail_num, starNum, starExp, luckyPoint, toTicks, addTime, activeFrozen, activePalsy, activeSpeedDown, activeBlow, unActiveFrozen, unActivePalsy, unActiveSpeedDown, unActiveBlow FROM t_merlin_magic_book WHERE roleID = {0}", nRoleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						merlinGrowthSaveDBData = new MerlinGrowthSaveDBData();
						merlinGrowthSaveDBData._RoleID = Global.SafeConvertToInt32(mySQLDataReader["roleID"].ToString(), 10);
						merlinGrowthSaveDBData._Occupation = Global.SafeConvertToInt32(mySQLDataReader["occupation"].ToString(), 10);
						merlinGrowthSaveDBData._Level = Global.SafeConvertToInt32(mySQLDataReader["level"].ToString(), 10);
						merlinGrowthSaveDBData._LevelUpFailNum = Global.SafeConvertToInt32(mySQLDataReader["level_up_fail_num"].ToString(), 10);
						merlinGrowthSaveDBData._StarNum = Global.SafeConvertToInt32(mySQLDataReader["starNum"].ToString(), 10);
						merlinGrowthSaveDBData._StarExp = Global.SafeConvertToInt32(mySQLDataReader["starExp"].ToString(), 10);
						merlinGrowthSaveDBData._LuckyPoint = Global.SafeConvertToInt32(mySQLDataReader["luckyPoint"].ToString(), 10);
						merlinGrowthSaveDBData._ToTicks = DataHelper.ConvertToTicks(mySQLDataReader["toTicks"].ToString());
						merlinGrowthSaveDBData._AddTime = DataHelper.ConvertToTicks(mySQLDataReader["addTime"].ToString());
						merlinGrowthSaveDBData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(mySQLDataReader["activeFrozen"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(mySQLDataReader["activePalsy"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(mySQLDataReader["activeSpeedDown"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(mySQLDataReader["activeBlow"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(mySQLDataReader["unActiveFrozen"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(mySQLDataReader["unActivePalsy"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(mySQLDataReader["unActiveSpeedDown"].ToString(), 10) / 100);
						merlinGrowthSaveDBData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(mySQLDataReader["unActiveBlow"].ToString(), 10) / 100);
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", text), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return merlinGrowthSaveDBData;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MerlinDBOperate()
		{
			byte[] array = new byte[14];
			array[5] = 1;
			MerlinDBOperate.t_fieldTypes = array;
		}

		private static readonly string[] t_fieldNames = new string[]
		{
			"level",
			"level_up_fail_num",
			"starNum",
			"starExp",
			"luckyPoint",
			"toTicks",
			"activeFrozen",
			"activePalsy",
			"activeSpeedDown",
			"activeBlow",
			"unActiveFrozen",
			"unActivePalsy",
			"unActiveSpeedDown",
			"unActiveBlow"
		};

		private static readonly byte[] t_fieldTypes;
	}
}
