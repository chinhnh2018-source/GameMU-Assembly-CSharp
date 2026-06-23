using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	public class DBWriter
	{
		public static string CurrentGoodsBakTableName
		{
			get
			{
				return DBWriter.GoodsBakTableNames[DBWriter.GoodsBakTableIndex % DBWriter.GoodsBakTableNames.Length];
			}
		}

		public static int ExecuteSQLNoQuery(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
		{
			int result = 0;
			bool flag = true;
			try
			{
				if (conn == null)
				{
					flag = false;
					conn = dbMgr.DBConns.PopDBConnection();
				}
				MySQLCommand mySQLCommand2;
				MySQLCommand mySQLCommand = mySQLCommand2 = new MySQLCommand(sqlText, conn);
				try
				{
					mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return -1;
				}
				finally
				{
					if (mySQLCommand2 != null)
					{
						mySQLCommand2.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -2;
			}
			finally
			{
				if (!flag && null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		public static bool CheckRoleCountFull(DBManager dbMgr)
		{
			bool result = true;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand("SELECT max(rid) AS LastID from t_roles", mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read())
				{
					num = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
				}
				if (null != mySQLCommand)
				{
					mySQLCommand.Dispose();
				}
				if (num % GameDBManager.DBAutoIncreaseStepValue >= GameDBManager.DBAutoIncreaseStepValue - 500)
				{
					int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("role_ext_auto_increment", 0);
					result = (gameConfigItemInt < 1500000000 || num >= gameConfigItemInt || 0 != DBWriter.ChangeTablesAutoIncrementValue(dbMgr, "t_roles", gameConfigItemInt));
				}
				else
				{
					result = false;
				}
			}
			catch (MySQLException)
			{
				result = true;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int CreateRole(DBManager dbMgr, string userID, string userName, int sex, int occup, string roleName, int zoneID, int bagnum, int isflashplayer, int nMagicSwordParam, int rebornbagnum)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string text2 = "-1:0:-1:-1";
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_roles (userid, rname, sex, occupation, position, regtime, lasttime, biguantime, zoneid, bhname, chenghao, username, bagnum, isflashplayer, magic_sword_param, reborn_bagnum) VALUES('{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', '{7}', {8}, '', '', '{9}', {10}, {11}, {12}, {13})", new object[]
				{
					userID,
					roleName,
					sex,
					occup,
					text2,
					text,
					text,
					text,
					zoneID,
					userName,
					bagnum,
					isflashplayer,
					nMagicSwordParam,
					rebornbagnum
				});
				if (!myDbConnection.ExecuteNonQueryBool(sql, 0))
				{
					return result;
				}
				try
				{
					result = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					result = -2;
				}
			}
			return result;
		}

		public static bool UnPreRemoveRole(DBManager dbMgr, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET predeltime=NULL WHERE rid={0} and predeltime IS NOT NULL", roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool PreRemoveRole(DBManager dbMgr, int roleID, DateTime Now)
		{
			bool result = false;
			string arg = Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET predeltime='{0}' WHERE rid={1} and predeltime IS NULL", arg, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool RemoveRole(DBManager dbMgr, int roleID)
		{
			bool result = false;
			string arg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET isdel=1, deltime='{0}', predeltime=NULL WHERE rid={1}", arg, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool RemoveRoleByName(DBManager dbMgr, string roleName)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET isdel=1 WHERE rname='{0}'", roleName);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UnRemoveRole(DBManager dbMgr, string roleName)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET isdel=0 WHERE rname='{0}'", roleName);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UnRemoveRoleByID(DBManager dbMgr, int rid)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET isdel=0 WHERE rid='{0}'", rid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleLoginInfo(DBManager dbMgr, int roleID, int loginNum, int loginDayID, int loginDayNum, string userid, int zoneid, string ip)
		{
			bool result = false;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET lasttime='{0}', loginnum={1}, logindayid={2}, logindaynum={3} WHERE rid={4}", new object[]
				{
					text,
					loginNum,
					loginDayID,
					loginDayNum,
					roleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
				DateTime now = DateTime.Now;
				sql = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},0,1) ON DUPLICATE KEY UPDATE loginnum=loginnum+1,rid={2}", new object[]
				{
					userid,
					Global.GetOffsetDay(now),
					roleID,
					text,
					Global.GetDayEndTime(now),
					ip,
					null,
					zoneid
				});
				myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleLogOff(DBManager dbMgr, int roleID, string userid, int zoneid, string ip, int onlineSecs)
		{
			bool result = false;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET logofftime='{0}' WHERE rid={1}", text, roleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
				DateTime now = DateTime.Now;
				sql = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},{8},1) ON DUPLICATE KEY UPDATE logouttime='{4}',onlinesecs=LEAST(onlineSecs+{8},86400);", new object[]
				{
					userid,
					Global.GetOffsetDay(now),
					roleID,
					Global.GetDayStartTime(now),
					text,
					ip,
					null,
					zoneid,
					onlineSecs
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
				result = true;
			}
			return result;
		}

		public static bool UpdateRoleOnlineSecs(DBManager dbMgr, int roleID, int totalOnlineSecs, int antiAddictionSecs)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET totalonlinesecs={0}, antiaddictionsecs={1} WHERE rid={2}", totalOnlineSecs, antiAddictionSecs, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBiGuanTime(DBManager dbMgr, int roleID, long biguanTime)
		{
			bool result = false;
			string arg = new DateTime(biguanTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET biguantime='{0}' WHERE rid={1}", arg, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBattleNameInfo(DBManager dbMgr, int roleID, long startTime, int nameIndex)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET battlenamestart={0}, battlenameindex={1} WHERE rid={2}", startTime, nameIndex, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleCZTaskID(DBManager dbMgr, int roleID, int czTaskID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET cztaskid={0} WHERE rid={1}", czTaskID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleOnceAwardFlag(DBManager dbMgr, int roleID, long onceawardflag)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET onceawardflag={0} WHERE rid={1}", onceawardflag, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBanProps(DBManager dbMgr, int roleID, string colName, long value)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET {0}={1} WHERE rid={2}", colName, value, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBangHuiInfo(DBManager dbMgr, int roleID, int faction, string bhName, int bhZhiWu)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET faction={0}, bhname='{1}', bhzhiwu={2} WHERE rid={3}", new object[]
				{
					faction,
					bhName,
					bhZhiWu,
					roleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleJunTuanInfo(DBManager dbMgr, int roleID, int junTuanZhiWu)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET juntuanzhiwu={1} WHERE rid={0}", roleID, junTuanZhiWu);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateAllRoleBangHuiName(DBManager dbMgr, int bhid, string newName)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET bhname='{0}' WHERE faction={1}", newName, bhid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool ClearAllRoleBangHuiInfo(DBManager dbMgr, int bhid)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET faction=0, bhname='', bhzhiwu=0, chenghao='' WHERE faction={0}", bhid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool ClearLastBangHuiInfoByRoleID(DBManager dbMgr, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET chenghao='' WHERE rid={0}", roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBangHuiVerify(DBManager dbMgr, int roleID, int toVerify)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET bhverify={0} WHERE rid={1}", toVerify, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateBHMatchSupportFlagData(DBManager dbMgr, BHMatchSupportData cmdData)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_banghui_match_support_flag (season, round, rid, bhid1, bhid2, guess, is_award, time)\r\n                                            VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, NOW()) ON DUPLICATE KEY UPDATE is_award={6}, time=NOW();", new object[]
				{
					cmdData.season,
					cmdData.round,
					cmdData.rid,
					cmdData.bhid1,
					cmdData.bhid2,
					cmdData.guess,
					cmdData.isaward
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int NewTask(DBManager dbMgr, int roleID, int npcID, int taskID, string addtime, int focus, int nStarLevel)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_tasks (taskid, rid, value1, value2, focus, isdel, addtime, starlevel) VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", new object[]
				{
					taskID,
					roleID,
					0,
					0,
					focus,
					0,
					addtime,
					nStarLevel
				});
				num = myDbConnection.ExecuteNonQuery(sql, 0);
				if (num < 0)
				{
					return num;
				}
				num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
			}
			return num;
		}

		public static bool UpdateRolePosition(DBManager dbMgr, int roleID, string position)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET position='{0}' WHERE rid={1}", position, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleExpLevel(DBManager dbMgr, int roleID, int level, long experience)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET level={0}, experience={1} WHERE rid={2}", level, experience, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleInterPower(DBManager dbMgr, int roleID, int interPower)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET interpower={0} WHERE rid={1}", interPower, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleMoney1(DBManager dbMgr, int roleID, int money)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET money1={0} WHERE rid={1}", money, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleYinLiang(DBManager dbMgr, int roleID, int yinLiang)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET yinliang={0} WHERE rid={1}", yinLiang, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleGold(DBManager dbMgr, int roleID, int gold)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET money2={0} WHERE rid={1}", gold, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleStoreYinLiang(DBManager dbMgr, int roleID, long yinLiang)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET store_yinliang={0} WHERE rid={1}", yinLiang, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleStoreMoney(DBManager dbMgr, int roleID, long money)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET store_money={0} WHERE rid={1}", money, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleOccupationBranch(DBManager dbMgr, int nRoleID, int nMagicSwordParam)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET magic_sword_param={0} WHERE rid={1}", nMagicSwordParam, nRoleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleBangGong(DBManager dbMgr, int roleID, int bgDayID1, int bgMoney, int bgDayID2, int bgGoods, int bangGong)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET bgdayid1={0}, bgmoney={1}, bgdayid2={2}, bggoods={3}, banggong={4} WHERE rid={5}", new object[]
				{
					bgDayID1,
					bgMoney,
					bgDayID2,
					bgGoods,
					bangGong,
					roleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateUserInfo(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			string userID = dbUserInfo.UserID;
			long num = (long)dbUserInfo.Money;
			long num2 = (long)dbUserInfo.RealMoney;
			long num3 = (long)dbUserInfo.GiftID;
			long num4 = (long)dbUserInfo.GiftJiFen;
			string text = Global.GCC(3, new object[]
			{
				userID,
				num,
				num2
			});
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_money (userid,money,realmoney,giftid,giftjifen,cc) VALUES('{0}',{1},{2},{3},{4},'{5}') ON DUPLICATE KEY UPDATE money={1},realmoney={2},giftid={3},giftjifen={4},cc='{5}'", new object[]
				{
					userID,
					num,
					num2,
					num3,
					num4,
					text
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateUserSpecJiFen(DBManager dbMgr, string userID, int specjifen)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_money set specjifen={1} where userid='{0}'", userID, specjifen);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateUserEveryJiFen(DBManager dbMgr, string userID, int specjifen)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_money set everyjifen={1} where userid='{0}'", userID, specjifen);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateUserInputPoints(DBManager dbMgr, string userID, int ipoints)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_money set points={1} where userid='{0}'", userID, ipoints);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateUserGiftJiFen(DBManager dbMgr, string userID, int giftJiFen)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_money set giftjifen={1} where userid='{0}'", userID, giftJiFen);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleDefSkillID(DBManager dbMgr, int roleID, int defSkillID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET skillid={0} WHERE rid={1}", defSkillID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleJieBiaoInfo(DBManager dbMgr, int roleID, int jieBiaoDayID, int jieBiaoDayNum)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET jiebiaodayid={0}, jiebiaonum={1} WHERE rid={2}", jieBiaoDayID, jieBiaoDayNum, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleAutoDrink(DBManager dbMgr, int roleID, int autoLifeV, int autoMagicV)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET autolife={0}, automagic={1} WHERE rid={2}", autoLifeV, autoMagicV, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int MoveGoods(DBManager dbMgr, int roleID, int goodsDbID, int oldRid, int site = 0)
		{
			int result = -10;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_goods SET rid={0}, site={3} WHERE Id={1} and rid={2}", new object[]
				{
					roleID,
					goodsDbID,
					oldRid,
					site
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int NewGoods(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int quality, string props, int forgeLevel, int binding, int site, string jewelList, int bagindex, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_goods (rid, goodsid, quality, Props, gcount, forge_level, binding, site, jewellist, bagindex, starttime, endtime, addpropindex, bornindex, lucky, strong, excellenceinfo, appendproplev, equipchangelife, ehinfo) VALUES({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, '{8}', {9}, '{10}', '{11}', {12}, {13}, {14}, {15}, {16}, {17}, {18}, '')", new object[]
					{
						roleID,
						goodsID,
						quality,
						props,
						goodsNum,
						forgeLevel,
						binding,
						site,
						jewelList,
						bagindex,
						startTime,
						endTime,
						addPropIndex,
						bornIndex,
						lucky,
						strong,
						ExcellenceProperty,
						nAppendPropLev,
						nEquipChangeLife
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num < 0)
					{
						return num;
					}
					num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static string FormatUpdateSQL(int id, string[] fields, int startIndex, string[] fieldNames, string tableName, byte[] fieldTypes, string idName = "Id")
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append("UPDATE ").Append(tableName).Append(" SET ");
			for (int i = 0; i < fieldNames.Count<string>(); i++)
			{
				if (!(fields[startIndex + i] == "*"))
				{
					if (fieldTypes[i] == 0)
					{
						stringBuilder.AppendFormat("{0}={1}", fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 1)
					{
						stringBuilder.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 2)
					{
						stringBuilder.AppendFormat("{0}={1}+{2}", fieldNames[i], fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 3)
					{
						stringBuilder.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i].Replace('$', ':')).Append(',');
					}
				}
			}
			stringBuilder[stringBuilder.Length - 1] = ' ';
			stringBuilder.AppendFormat(" WHERE {0}={1}", idName, id);
			if (stringBuilder.Length > 100)
			{
			}
			return stringBuilder.ToString();
		}

		public static int UpdateGoods(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateGoods_fieldNames, "t_goods", DBWriter._UpdateGoods_fieldTypes, "Id");
				text += string.Format(" and rid={0}", fields[0]);
				result = myDbConnection.ExecuteNonQuery(text, 0);
			}
			return result;
		}

		public static int RemoveGoods(DBManager dbMgr, int id)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_goods SET gcount=0 WHERE Id={0}", id);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int MoveGoodsDataToBackupTable(DBManager dbMgr, int id)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO {1} SELECT *,0,NOW(),0 FROM t_goods WHERE Id={0}", id, DBWriter.CurrentGoodsBakTableName);
				myDbConnection.ExecuteNonQuery(sql, 0);
				sql = string.Format("DELETE FROM t_goods WHERE Id={0}", id);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int SwitchGoodsBackupTable(DBManager dbMgr)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				lock (DBWriter.GoodsBakTableMutex)
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					int num = TimeUtil.NowDateTime().Month % DBWriter.GoodsBakTableNames.Length;
					if (DBWriter.GoodsBakTableIndex < 0)
					{
						DBWriter.GoodsBakTableIndex = num;
					}
					int num2 = DBWriter.GoodsBakTableIndex % DBWriter.GoodsBakTableNames.Length;
					if (num != num2)
					{
						string arg = DBWriter.GoodsBakTableNames[num];
						string sqlText = string.Format("SELECT id FROM {0} limit 1;", arg);
						int num3 = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, mySQLConnection);
						if (num3 > 0)
						{
							sqlText = string.Format("TRUNCATE TABLE {0};", arg);
							num3 = DBWriter.ExecuteSQLNoQuery(dbMgr, sqlText, mySQLConnection);
							if (num3 < 0)
							{
								LogManager.WriteLog(LogTypes.Error, "阶段物品备份表失败，不切换数据库", null, true);
							}
						}
						else
						{
							DBWriter.GoodsBakTableIndex = num;
						}
					}
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return 0;
		}

		public static bool WirterAutoCompletionTaskByTaskID(DBManager dbMgr, int roleID, List<int> taskIDList)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				for (int i = 1; i < taskIDList.Count; i++)
				{
					string sql = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskIDList[i], roleID);
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
				result = true;
			}
			return result;
		}

		public static int UpdateTask(DBManager dbMgr, int dbID, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(dbID, fields, startIndex, DBWriter._UpdateTask_fieldNames, "t_tasks", DBWriter._UpdateTask_fieldTypes, "Id");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool CompleteTask(DBManager dbMgr, int roleID, int npcID, int taskID, int dbID, int isMainTask)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskID, roleID);
				if (isMainTask == 0)
				{
					myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
				sql = string.Format("delete from t_tasks WHERE Id={0}", dbID);
				myDbConnection.ExecuteNonQueryBool(sql, 0);
				result = true;
			}
			return result;
		}

		public static bool DeleteTask(DBManager dbMgr, int roleID, int taskID, int dbID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool GMSetTask(DBManager dbMgr, int roleID, int taskID, List<int> taskIDList)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				for (int i = 1; i < taskIDList.Count - 1; i++)
				{
					string sql = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskIDList[i], roleID);
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
				result = true;
			}
			return result;
		}

		public static int AddFriend(DBManager dbMgr, int dbID, int roleID, int otherID, int friendType)
		{
			int result = dbID;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_friends (myid, otherid, friendType) VALUES({0}, {1}, {2}) ON DUPLICATE KEY UPDATE friendType={3}", new object[]
					{
						roleID,
						otherID,
						friendType,
						friendType
					});
					if (myDbConnection.ExecuteNonQueryBool(sql, 0) && dbID < 0)
					{
						result = myDbConnection.GetSingleInt(string.Format("SELECT Id FROM t_friends where myid={0} and otherid={1}", roleID, otherID), 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					result = -2;
				}
			}
			return result;
		}

		public static bool RemoveFriend(DBManager dbMgr, int dbID, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_friends WHERE Id={0}", dbID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdatePKMode(DBManager dbMgr, int roleID, int pkMode)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET pkmode={0} WHERE rid={1}", pkMode, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdatePKValues(DBManager dbMgr, int roleID, int pkValue, int pkPoint)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET pkvalue={0}, pkpoint={1} WHERE rid={2}", pkValue, pkPoint, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateLianZhan(DBManager dbMgr, int roleID, int lianzhan)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET lianzhan={0} WHERE rid={1}", lianzhan, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateKillBoss(DBManager dbMgr, int roleID, int killBoss)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET killboss={0} WHERE rid={1}", killBoss, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateBattleNum(DBManager dbMgr, int roleID, int battleNum)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET battlenum={0} WHERE rid={1}", battleNum, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateHeroIndex(DBManager dbMgr, int roleID, int heroIndex)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET heroindex={0} WHERE rid={1}", heroIndex, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleStat(DBManager dbMgr, int roleID, int equipJiFen, int xueWeiNum, int skillLearnedNum, int horseJiFen)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET equipjifen={0}, xueweinum={1}, skilllearnednum={2}, horsejifen={3} WHERE rid={4}", new object[]
				{
					equipJiFen,
					xueWeiNum,
					skillLearnedNum,
					horseJiFen,
					roleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleKeys(DBManager dbMgr, int roleID, int type, string keys)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (0 == type)
				{
					sql = string.Format("UPDATE t_roles SET main_quick_keys='{0}' WHERE rid={1}", keys, roleID);
				}
				else
				{
					sql = string.Format("UPDATE t_roles SET other_quick_keys='{0}' WHERE rid={1}", keys, roleID);
				}
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleLeftFightSecs(DBManager dbMgr, int roleID, int leftFightSecs)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET leftfightsecs={0} WHERE rid={1}", leftFightSecs, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int NewHorse(DBManager dbMgr, int roleID, int horseID, int bodyID, string addtime)
		{
			int num = -1;
			string text = "0,0,0,0,0,0,0,0,0";
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_horses (rid, horseid, bodyid, propsNum, PropsVal, addtime, isdel, failednum) VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, {7})", new object[]
					{
						roleID,
						horseID,
						bodyID,
						text,
						text,
						addtime,
						0,
						0
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num < 0)
					{
						return num;
					}
					num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static int NewPet(DBManager dbMgr, int roleID, int petID, string petName, int petType, string props, string addtime)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_pets (rid, petid, petname, pettype, feednum, realivenum, addtime, props, isdel) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, '{6}', '{7}', {8})", new object[]
					{
						roleID,
						petID,
						petName,
						petType,
						0,
						0,
						addtime,
						props,
						0
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num < 0)
					{
						return num;
					}
					num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static int UpdateHorse(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int result = -1;
			string[] fieldNames = new string[]
			{
				"isdel",
				"horseid",
				"bodyid",
				"propsNum",
				"PropsVal",
				"failednum",
				"temptime",
				"tempnum",
				"faileddayid"
			};
			byte[] fieldTypes = new byte[]
			{
				0,
				0,
				0,
				1,
				1,
				0,
				3,
				0,
				0
			};
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_horses", fieldTypes, "Id");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleHorse(DBManager dbMgr, int roleID, int horseDbID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (horseDbID > 0)
				{
					sql = string.Format("UPDATE t_roles SET horseid={0}, lasthorseid={1} WHERE rid={2}", horseDbID, horseDbID, roleID);
				}
				else
				{
					sql = string.Format("UPDATE t_roles SET horseid={0} WHERE rid={1}", horseDbID, roleID);
				}
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int UpdatePet(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdatePet_fieldNames, "t_pets", DBWriter._UpdatePet_fieldTypes, "Id");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateRolePet(DBManager dbMgr, int roleID, int petDbID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET petid={0} WHERE rid={1}", petDbID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int AddRoleDJPoint(DBManager dbMgr, int dbID, int roleID, int djPoint)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					bool flag = false;
					string sql;
					if (dbID <= 0)
					{
						flag = true;
						sql = string.Format("INSERT INTO t_djpoints (rid, djpoint, total, wincnt) VALUES({0}, {1}, {2}, {3})", new object[]
						{
							roleID,
							djPoint,
							1,
							(djPoint > 0) ? 1 : 0
						});
					}
					else
					{
						sql = string.Format("UPDATE t_djpoints SET djpoint=djpoint+{0}, total=total+1, wincnt=wincnt+{1} WHERE Id={2}", djPoint, (djPoint > 0) ? 1 : 0, dbID);
					}
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num >= 0)
					{
						if (flag)
						{
							num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
						}
						else
						{
							num = dbID;
						}
					}
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static int UpRoleJingMai(DBManager dbMgr, int roleID, int dbID, int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					bool flag = false;
					string sql;
					if (dbID <= 0)
					{
						flag = true;
						sql = string.Format("INSERT INTO t_jingmai (rid, jmid, jmlevel, bodylevel) VALUES({0}, {1}, {2}, {3})", new object[]
						{
							roleID,
							jingMaiID,
							jingMaiLevel,
							jingMaiBodyLevel
						});
					}
					else
					{
						sql = string.Format("UPDATE t_jingmai SET jmlevel={0} WHERE Id={1}", jingMaiLevel, dbID);
					}
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num >= 0)
					{
						if (flag)
						{
							num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
						}
						else
						{
							num = dbID;
						}
					}
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static int NewBulletinText(DBManager dbMgr, string msgID, string fromDate, string toDate, int interval, string bulletinText)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_bulletin (msgid, intervals, bulletintext, fromdate, todate, opttime) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}')\r\n                                                ON DUPLICATE KEY UPDATE intervals={1}, bulletintext='{2}', fromdate='{3}', todate='{4}', opttime='{5}'", new object[]
				{
					msgID,
					interval,
					bulletinText,
					fromDate,
					toDate,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int RemoveBulletinText(DBManager dbMgr, string msgID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_bulletin WHERE msgid='{0}'", msgID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateGameConfig(DBManager dbMgr, string paramName, string paramValue)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_config (paramname, paramvalue) VALUES('{0}', '{1}') ON DUPLICATE KEY UPDATE paramvalue='{2}'", paramName, paramValue, paramValue);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddSkill(DBManager dbMgr, int roleID, int skillID, int skillLevel)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_skills (rid, skillid, skilllevel, usednum) VALUES({0}, {1}, {2}, 0)", roleID, skillID, skillLevel);
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num >= 0)
					{
						num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static bool UpdateSkillInfo(DBManager dbMgr, int skillDbID, int skillLevel, int usedNum)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_skills SET skilllevel={0}, usednum={1} WHERE Id={2}", skillLevel, usedNum, skillDbID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateJingMaiExp(DBManager dbMgr, int roleID, int jingMaiExpNum, int totalJingMaiExp)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET jingmai_exp_num={0}, total_jingmai_exp={1} WHERE rid={2}", jingMaiExpNum, totalJingMaiExp, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleBufferItem(DBManager dbMgr, int roleID, int bufferID, long startTime, int bufferSecs, long bufferVal)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_buffer (rid, bufferid, starttime, buffersecs, bufferval) VALUES({0}, {1}, {2}, {3}, {4}) ON DUPLICATE KEY UPDATE starttime={5}, buffersecs={6}, bufferval={7}", new object[]
				{
					roleID,
					bufferID,
					startTime,
					bufferSecs,
					bufferVal,
					startTime,
					bufferSecs,
					bufferVal,
					roleID,
					bufferID
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleDailyTaskData(DBManager dbMgr, int roleID, int huanID, string rectime, int recnum, int taskClass, int extDayID, int extNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_dailytasks (rid, huanid, rectime, recnum, taskClass, extdayid, extnum) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE huanid={1}, rectime='{2}', recnum={3}, taskClass={4}, extdayid={5}, extnum={6}", new object[]
				{
					roleID,
					huanID,
					rectime,
					recnum,
					taskClass,
					extDayID,
					extNum
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleDailyJingMaiData(DBManager dbMgr, int roleID, string jmTime, int jmNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_dailyjingmai (rid, jmtime, jmnum) VALUES({0}, '{1}', {2}) ON DUPLICATE KEY UPDATE jmtime='{3}', jmnum={4}", new object[]
				{
					roleID,
					jmTime,
					jmNum,
					jmTime,
					jmNum
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleNumSkillID(DBManager dbMgr, int roleID, int numSkillID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET numskillid={0} WHERE rid={1}", numSkillID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleMainTaskID(DBManager dbMgr, int roleID, int mainTaskID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET maintaskid={0} WHERE rid={1}", mainTaskID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int UpdateRolePBInfo(DBManager dbMgr, int roleID, int extGridNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_ptbag (rid, extgridnum) VALUES({0}, {1}) ON DUPLICATE KEY UPDATE extgridnum={2}", roleID, extGridNum, extGridNum);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleRebornStorageInfo(DBManager dbMgr, int roleID, int extGridNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.InsertRebornGridNum, roleID, extGridNum, extGridNum);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleBagNum(DBManager dbMgr, int roleID, int bagNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles set bagnum={0} where rid={1}", bagNum, roleID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleRebornBagNum(DBManager dbMgr, int roleID, int bagNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateRebornBagNum, bagNum, roleID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleRebornShowEquip(DBManager dbMgr, int roleID, int bagNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateRebornShow, bagNum, roleID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleRebornShowModel(DBManager dbMgr, int roleID, int bagNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateRebornShowModel, bagNum, roleID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static void CreateHuoDong(DBManager dbMgr, int roleID)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_huodong (rid, loginweekid, logindayid, loginnum, newstep, steptime, lastmtime, curmid, curmtime, songliid, logingiftstate, onlinegiftstate) VALUES({0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, {10}, {11})", new object[]
				{
					roleID,
					"",
					"",
					0,
					0,
					text,
					0,
					"",
					0,
					0,
					0,
					0
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static int UpdateHuoDong(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateActivity_fieldNames, "t_huodong", DBWriter._UpdateActivity_fieldTypes, "rid");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int ClearAllLiPinMa(DBManager dbMgr)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_linpinma", new object[0]);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int InsertNewLiPinMa(DBManager dbMgr, string liPinMa, string songLiID, string maxNum, string ptid, string ptRepeat, string usedNum = "0")
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_linpinma (lipinma, huodongid, maxnum, usednum, ptid, ptrepeat) VALUES('{0}', {1}, {2}, {3}, {4}, {5})", new object[]
				{
					liPinMa,
					songLiID,
					maxNum,
					usedNum,
					ptid,
					ptRepeat
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateLiPinMaUsedNum(DBManager dbMgr, string liPinMa, int usedNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_linpinma SET usednum={0} WHERE lipinma='{1}'", usedNum, liPinMa);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int DeleteSpecialActivityData(DBManager dbMgr, int roleID, int groupID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (groupID == 0)
				{
					sql = string.Format("DELETE FROM t_special_activity WHERE rid={0}", roleID);
				}
				else
				{
					sql = string.Format("DELETE FROM t_special_activity WHERE rid={0} AND groupid={1}", roleID, groupID);
				}
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int DeleteSpecialPriorityActivityData(DBManager dbMgr, int roleID, int tequanID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (tequanID == 0)
				{
					sql = string.Format("DELETE FROM t_special_priority_activity WHERE rid={0}", roleID);
				}
				else
				{
					sql = string.Format("DELETE FROM t_special_priority_activity WHERE rid={0} AND tequanid={1}", roleID, tequanID);
				}
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int DeleteEverydayActivityData(DBManager dbMgr, int roleID, int groupID, int actID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = "";
				if (groupID == 0)
				{
					sql = string.Format("DELETE FROM t_everyday_activity WHERE rid={0}", roleID);
				}
				else if (groupID != 0 && actID == 0)
				{
					sql = string.Format("DELETE FROM t_everyday_activity WHERE rid={0} AND groupid={1}", roleID, groupID);
				}
				else if (groupID != 0 && actID != 0)
				{
					sql = string.Format("DELETE FROM t_everyday_activity WHERE rid={0} AND groupid={1} AND actid={2}", roleID, groupID, actID);
				}
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateSpecialActivityData(DBManager dbMgr, int roleID, SpecActInfoDB SpecAct)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_special_activity (rid, groupid, actid, purchaseNum, countNum, active) VALUES({0}, {1}, {2}, {3}, {4}, {5})\r\n                        ON DUPLICATE KEY UPDATE groupid={1}, actid={2}, purchaseNum={3}, countNum={4}, active={5}", new object[]
				{
					roleID,
					SpecAct.GroupID,
					SpecAct.ActID,
					SpecAct.PurNum,
					SpecAct.CountNum,
					SpecAct.Active
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateSpecialPriorityActivityData(DBManager dbMgr, int roleID, SpecPriorityActInfoDB SpecAct)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_special_priority_activity (rid, tequanid, actid, purchaseNum, countNum) VALUES({0}, {1}, {2}, {3}, {4})\r\n                        ON DUPLICATE KEY UPDATE tequanid={1}, actid={2}, purchaseNum={3}, countNum={4}", new object[]
				{
					roleID,
					SpecAct.TeQuanID,
					SpecAct.ActID,
					SpecAct.PurNum,
					SpecAct.CountNum
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateEverydayActivityData(DBManager dbMgr, int roleID, EverydayActInfoDB EveryAct)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_everyday_activity (rid, groupid, actid, purchaseNum, countNum, activeDay) VALUES({0}, {1}, {2}, {3}, {4}, {5})\r\n                        ON DUPLICATE KEY UPDATE groupid={1}, actid={2}, purchaseNum={3}, countNum={4}, activeDay={5}", new object[]
				{
					roleID,
					EveryAct.GroupID,
					EveryAct.ActID,
					EveryAct.PurNum,
					EveryAct.CountNum,
					EveryAct.ActiveDay
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateFuBenData(DBManager dbMgr, int roleID, int fuBenID, int dayID, int enterNum, int nQuickPassTimeSec, int nFinishNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_fuben (rid, fubenid, dayid, enternum, quickpasstimer, finishnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}) ON DUPLICATE KEY UPDATE fubenid={1}, dayid={2}, enternum={3}, quickpasstimer={4}, finishnum={5}", new object[]
				{
					roleID,
					fuBenID,
					dayID,
					enterNum,
					nQuickPassTimeSec,
					nFinishNum
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int InsertNewPreName(DBManager dbMgr, string preName, int sex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_prenames (name, sex, used) VALUES('{0}', {1}, {2})", preName, sex, 0);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdatePreNameUsedState(DBManager dbMgr, string preName, int used)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_prenames SET used={0} WHERE name='{1}'", used, preName);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int InsertNewFuBenHist(DBManager dbMgr, int fuBenID, int roleID, string roleName, int usedSecs)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_fubenhist (fubenid, rid, rname, usedsecs) VALUES({0}, {1}, '{2}', {3}) ON DUPLICATE KEY UPDATE rid={4}, rname='{5}', usedsecs={6}", new object[]
				{
					fuBenID,
					roleID,
					roleName,
					usedSecs,
					roleID,
					roleName,
					usedSecs
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleDailyData(DBManager dbMgr, int roleID, int expDayID, int todayExp, int lingLiDayID, int todayLingLi, int killBossDayID, int todayKillBoss, int fuBenDayID, int todayFuBenNum, int wuXingDayID, int wuXingNum, int rebornExpDayID, int rebornExpMonster, int rebornExpSale)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_dailydata (rid, expdayid, todayexp, linglidayid, todaylingli, killbossdayid, todaykillboss, fubendayid, todayfubennum, wuxingdayid, wuxingnum, reborndayid, rebornexpmonster, rebornexpsale) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}) ON DUPLICATE KEY UPDATE expdayid={1}, todayexp={2}, linglidayid={3}, todaylingli={4}, killbossdayid={5}, todaykillboss={6}, fubendayid={7}, todayfubennum={8}, wuxingdayid={9}, wuxingnum={10}, reborndayid={11}, rebornexpmonster={12}, rebornexpsale={13}", new object[]
				{
					roleID,
					expDayID,
					todayExp,
					lingLiDayID,
					todayLingLi,
					killBossDayID,
					todayKillBoss,
					fuBenDayID,
					todayFuBenNum,
					wuXingDayID,
					wuXingNum,
					rebornExpDayID,
					rebornExpMonster,
					rebornExpSale
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateYaBiaoData(DBManager dbMgr, int roleID, int yaBiaoID, long startTime, int state, int lineID, int touBao, int yaBiaoDayID, int yaBiaoNum, int takeGoods)
		{
			int result = -1;
			string text = "1900-01-01 12:00:00";
			if (startTime > 0L)
			{
				text = new DateTime(startTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
			}
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_yabiao (rid, yabiaoid, starttime, state, lineid, toubao, yabiaodayid, yabiaonum, takegoods) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}) ON DUPLICATE KEY UPDATE yabiaoid={1}, starttime='{2}', state={3}, lineid={4}, toubao={5}, yabiaodayid={6}, yabiaonum={7}, takegoods={8}", new object[]
				{
					roleID,
					yaBiaoID,
					text,
					state,
					lineID,
					touBao,
					yaBiaoDayID,
					yaBiaoNum,
					takeGoods
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateYaBiaoDataState(DBManager dbMgr, int roleID, int state)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_yabiao SET state={0} WHERE rid={1}", state, roleID);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewMallBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_mallbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewQiZhenGeBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_qizhengebuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewShengXiaoGuessHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int guessKey, int mortgage, int resultKey, int gainNum, int leftMortgage)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_shengxiaoguesshist (rid, rname, guesskey, mortgage, resultkey, gainnum, leftmortgage, zoneid, guesstime) VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, '{8}')", new object[]
				{
					roleID,
					roleName,
					guessKey,
					mortgage,
					resultKey,
					gainNum,
					leftMortgage,
					zoneID,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewYinPiaoBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinPiaoNum)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_yinpiaobuy (rid, goodsid, goodsnum, totalprice, leftyinpiao, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftYinPiaoNum,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewOnlineNumItem(DBManager dbMgr, int totalNum, DateTime dateTime, string strMapOnlineInfo)
		{
			int result = -1;
			string arg = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_onlinenum (num, rectime, mapnum) VALUES({0}, '{1}', '{2}')", totalNum, arg, strMapOnlineInfo);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int CreateBangHui(DBManager dbMgr, int roleID, int zoneID, int totalLevel, string bhName, string bhBulletin, int nMoney = 0)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				MySQLConnection mySQLConnection2 = mySQLConnection;
				string[] array = new string[]
				{
					"zoneid"
				};
				string[] array2 = new string[]
				{
					"t_banghui"
				};
				object[,] array3 = new object[1, 3];
				array3[0, 0] = "bhname";
				array3[0, 1] = "=";
				array3[0, 2] = bhName;
				MySQLSelectCommand mySQLSelectCommand = new MySQLSelectCommand(mySQLConnection2, array, array2, array3, null, null);
				if (mySQLSelectCommand.Table.Rows.Count > 0)
				{
					result = -1;
					LogManager.WriteLog(LogTypes.Error, string.Format("帮会已存在: {0}", bhName), null, true);
					return result;
				}
				bool flag = false;
				string text2 = string.Format("INSERT INTO t_banghui (bhname, zoneid, rid, totalnum, totallevel, bhbulletin, buildtime, qiname, tongqian) VALUES('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8})", new object[]
				{
					bhName,
					zoneID,
					roleID,
					1,
					totalLevel,
					bhBulletin,
					text,
					bhName,
					nMoney
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
				}
				catch (MySQLException)
				{
					flag = true;
					result = -1;
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text2), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
				try
				{
					if (!flag)
					{
						mySQLCommand = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", mySQLConnection);
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						if (mySQLDataReader.Read())
						{
							result = Convert.ToInt32(mySQLDataReader[0].ToString());
						}
						mySQLCommand.Dispose();
						mySQLCommand = null;
					}
				}
				catch (MySQLException)
				{
					result = -2;
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static void DeleteBangHui(DBManager dbMgr, int bhid)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET isdel=1 WHERE bhid={0}", bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateZhengDuoUsedTime(DBManager dbMgr, int bhid, int weekDay, int usedTime)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET zhengduousedtime={1},zhengduoweek={2} WHERE bhid={0}", bhid, usedTime, weekDay);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiBulletin(DBManager dbMgr, int bhid, string bhBulletinMsg)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET bhbulletin='{0}' WHERE bhid={1} AND isdel=0", bhBulletinMsg, bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiVerify(DBManager dbMgr, int roleID, int bhid, int isVerify)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET isverfiy={0} WHERE bhid={1} AND rid={2}", isVerify, bhid, roleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiRoleID(DBManager dbMgr, int roleID, int bhid)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET rid={0} WHERE bhid={1}", roleID, bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void ClearBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int zhiWu)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET bhzhiwu=0 WHERE faction={0} AND bhzhiwu={1}", bhid, zhiWu);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void ChangeJunTuanZhiWuList(DBManager dbMgr, int bhid, int zhiwu, List<int> list)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = string.Format("UPDATE t_roles SET juntuanzhiwu={1} WHERE faction={0}", bhid, zhiwu);
				if (list.Count > 0)
				{
					string arg = string.Join<int>(",", list);
					text += string.Format(" AND rid in({0});", arg);
					myDbConnection.ExecuteNonQuery(text, 0);
				}
			}
		}

		public static void ClearBangHuiZhiWuNotInList(DBManager dbMgr, int bhid, List<int> list)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = string.Format("UPDATE t_roles SET juntuanzhiwu=0 WHERE faction={0}", bhid);
				if (list.Count > 0)
				{
					string arg = string.Join<int>(",", list);
					text += string.Format(" AND rid not in({0});", arg);
				}
				myDbConnection.ExecuteNonQuery(text, 0);
			}
		}

		public static void UpdateBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int otherRoleID, int zhiWu)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET bhzhiwu={0} WHERE faction={1} AND rid={2}", zhiWu, bhid, otherRoleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiMemberChengHao(DBManager dbMgr, int bhid, int otherRoleID, string chengHao)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET chenghao='{0}' WHERE faction={1} AND rid={2}", chengHao, bhid, otherRoleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiBangGong(DBManager dbMgr, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET goods1num=goods1num+{0}, goods2num=goods2num+{1}, goods3num=goods3num+{2}, goods4num=goods4num+{3}, goods5num=goods5num+{4}, tongqian=tongqian+{5} WHERE bhid={6}", new object[]
				{
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					tongQian,
					bhid
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void AddBangGongHistItem(DBManager dbMgr, int roleID, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian, int banggong)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_banggonghist (bhid, rid, goods1num, goods2num, goods3num, goods4num, goods5num, tongqian, banggong, addtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}') ON DUPLICATE KEY UPDATE goods1num=goods1num+{2}, goods2num=goods2num+{3}, goods3num=goods3num+{4}, goods4num=goods4num+{5}, goods5num=goods5num+{6}, tongqian=tongqian+{7}, banggong=banggong+{8}, addtime='{9}'", new object[]
				{
					roleID,
					bhid,
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					tongQian,
					banggong,
					text
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiQiName(DBManager dbMgr, int bhid, string qiName, int needMoney)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET qiname='{0}', tongqian=tongqian-{1} WHERE bhid={2}", qiName, needMoney, bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiQiLevel(DBManager dbMgr, int bhid, int toLevel, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int needMoney)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET qilevel={0}, goods1num=goods1num-{1}, goods2num=goods2num-{2}, goods3num=goods3num-{3}, goods4num=goods4num-{4}, goods5num=goods5num-{5}, tongqian=tongqian-{6} WHERE bhid={7}", new object[]
				{
					toLevel,
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					needMoney,
					bhid
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateOnePieceTreasureLog(DBManager dbMgr, string KeyTime, int LogType, int addValue)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				switch (LogType)
				{
				case 0:
					sql = string.Format("INSERT INTO t_treasure_log (time, role) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE role=role+{1}", KeyTime, addValue);
					break;
				case 1:
					sql = string.Format("INSERT INTO t_treasure_log (time, dice) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE dice=dice+{1}", KeyTime, addValue);
					break;
				case 2:
					sql = string.Format("INSERT INTO t_treasure_log (time, superdice) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE superdice=superdice+{1}", KeyTime, addValue);
					break;
				case 3:
					sql = string.Format("INSERT INTO t_treasure_log (time, movenum) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE movenum=movenum+{1}", KeyTime, addValue);
					break;
				default:
					throw new ArgumentException("The UpdateOnePieceTreasureLog RPC OnePieceTreasureLogType Wrong");
				}
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBuildingLog(DBManager dbMgr, string KeyTime, BuildingLogType LogType)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				switch (LogType)
				{
				case BuildingLogType.BuildLog_TaskRole:
					sql = string.Format("INSERT INTO t_building_log (time, task_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE task_role=task_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Task:
					sql = string.Format("INSERT INTO t_building_log (time, task) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE task=task+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_RefreshRole:
					sql = string.Format("INSERT INTO t_building_log (time, refresh_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE refresh_role=refresh_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Refresh:
					sql = string.Format("INSERT INTO t_building_log (time, refresh) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE refresh=refresh+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_OpenRole:
					sql = string.Format("INSERT INTO t_building_log (time, open_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE open_role=open_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Open:
					sql = string.Format("INSERT INTO t_building_log (time, open) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE open=open+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Push:
					sql = string.Format("INSERT INTO t_building_log (time, push) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE push=push+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_PushUse:
					sql = string.Format("INSERT INTO t_building_log (time, pushuse) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE pushuse=pushuse+1", KeyTime);
					break;
				default:
					throw new ArgumentException("The UpdateBuildingLog RPC BuildingLogType Wrong");
				}
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBuildingData(DBManager dbMgr, int rid, BuildingData myBuildData)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_building (rid, buildid, taskid_1, taskid_2, taskid_3, taskid_4, level, exp, developtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}') ON DUPLICATE KEY UPDATE taskid_1 = {2}, taskid_2 = {3}, taskid_3 = {4}, taskid_4 = {5}, level = {6}, exp = {7}, developtime = '{8}'", new object[]
				{
					rid,
					myBuildData.BuildId,
					myBuildData.TaskID_1,
					myBuildData.TaskID_2,
					myBuildData.TaskID_3,
					myBuildData.TaskID_4,
					myBuildData.BuildLev,
					myBuildData.BuildExp,
					myBuildData.BuildTime
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateOramentData(DBManager dbMgr, OrnamentUpdateDbData myOrnamentData)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_ornament (roleid, goodsid, param1, param2) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE param1 = {2}, param2 = {3}", new object[]
				{
					myOrnamentData.RoleId,
					myOrnamentData.Data.ID,
					myOrnamentData.Data.Param1,
					myOrnamentData.Data.Param2
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateZhanMengBuildLevel(DBManager dbMgr, int bhid, int toLevel, int needMoney, string fieldName)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET {0}={1}, tongqian=tongqian-{2} WHERE bhid={3}", new object[]
				{
					fieldName,
					toLevel,
					needMoney,
					bhid
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void AddBangHuiTongQian(DBManager dbMgr, int bhid, int addMoney)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET tongqian=tongqian+{0} WHERE bhid={1}", addMoney, bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void SubBangHuiTongQian(DBManager dbMgr, int bhid, int subMoney)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0} WHERE bhid={1}", subMoney, bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiSumData(DBManager dbMgr)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = "UPDATE t_banghui b,(SELECT SUM(combatforce) AS totalcombatforce,SUM(LEVEL) AS totallevel,COUNT(rid) AS totalnum,faction AS bhid FROM t_roles WHERE isdel=0 and faction>0 GROUP BY faction) r SET b.totalcombatforce=r.totalcombatforce,b.totallevel=r.totallevel,b.totalnum=r.totalnum WHERE b.`bhid`=r.bhid and b.isdel=0";
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiNumLevel(DBManager dbMgr, int bhid, int totalNum, int totalLevel, int TotalCombatForce)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET totalnum={0}, totallevel={1}, totalcombatforce={3} WHERE bhid={2}", new object[]
				{
					totalNum,
					totalLevel,
					bhid,
					TotalCombatForce
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBHLingDi(DBManager dbMgr, BangHuiLingDiInfoData bangHuiLingDiInfoData)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_lingdi (lingdi, bhid, tax, takedayid, takedaynum, yestodaytax, taxdayid, todaytax, totaltax, warrequest, awardfetchday) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}) ON DUPLICATE KEY UPDATE bhid={1}, tax={2}, takedayid={3}, takedaynum={4}, yestodaytax={5}, taxdayid={6}, todaytax={7}, totaltax={8}, warrequest='{9}', awardfetchday={10}", new object[]
				{
					bangHuiLingDiInfoData.LingDiID,
					bangHuiLingDiInfoData.BHID,
					bangHuiLingDiInfoData.LingDiTax,
					bangHuiLingDiInfoData.TakeDayID,
					bangHuiLingDiInfoData.TakeDayNum,
					bangHuiLingDiInfoData.YestodayTax,
					bangHuiLingDiInfoData.TaxDayID,
					bangHuiLingDiInfoData.TodayTax,
					bangHuiLingDiInfoData.TotalTax,
					bangHuiLingDiInfoData.WarRequest,
					bangHuiLingDiInfoData.AwardFetchDay
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void ClearBHLingDiByID(DBManager dbMgr, int bhid)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_lingdi SET bhid=0, tax=0, takedayid=0, takedaynum=0, yestodaytax=0, taxdayid=0, todaytax=0, totaltax=0 WHERE bhid={0}", bhid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void ClearBHLingDiTotalTaxByID(DBManager dbMgr, int lingDiID)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_lingdi SET totaltax=0 WHERE lingdi={0}", lingDiID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void SubBangHuiTongQianByQiLevel(DBManager dbMgr, int moneyPerLevel)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0} where isdel=0", moneyPerLevel);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateBangHuiFuBen(DBManager dbMgr, int bhid, int fubenid, int state, int openday, string killers)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_banghui SET fubenid={0}, fubenstate={1}, openday={2}, killers='{3}' WHERE bhid={4} AND isdel=0", new object[]
				{
					fubenid,
					state,
					openday,
					killers,
					bhid
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateRoleToHuangFei(DBManager dbMgr, int roleID, int huangHou)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET huanghou={0} WHERE rid={1}", huangHou, roleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateHuangDiTeQuan(DBManager dbMgr, HuangDiTeQuanItem hangDiTeQuanItem)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_hdtequan (Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE Id={0}, tolaofangdayid={1}, tolaofangnum={2}, offlaofangdayid={3}, offlaofangnum={4}, bancatdayid={5}, bancatnum={6}", new object[]
				{
					hangDiTeQuanItem.ID,
					hangDiTeQuanItem.ToLaoFangDayID,
					hangDiTeQuanItem.ToLaoFangNum,
					hangDiTeQuanItem.OffLaoFangDayID,
					hangDiTeQuanItem.OffLaoFangNum,
					hangDiTeQuanItem.BanCatDayID,
					hangDiTeQuanItem.BanCatNum
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void AddRefreshQiZhenGeRec(DBManager dbMgr, int roleID, int oldUserMoney, int leftUserMoney)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_refreshqizhen (rid, oldusermoney, leftusermoney, refreshtime) VALUES({0}, {1}, {2}, '{3}')", new object[]
				{
					roleID,
					oldUserMoney,
					leftUserMoney,
					text
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void AddUsedLiPinMa(DBManager dbMgr, int huodongID, string lipinMa, int pingTaiID, int roleID)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_usedlipinma (lipinma, huodongid, ptid, rid) VALUES('{0}', {1}, {2}, {3})", new object[]
				{
					lipinMa,
					huodongID,
					pingTaiID,
					roleID
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void AddMoneyWarning(DBManager dbMgr, int roleID, int usedMoney, int goodsMoney)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_warning (rid, usedmoney, goodsmoney, warningtime) VALUES({0}, {1}, {2}, '{3}')", new object[]
				{
					roleID,
					usedMoney,
					goodsMoney,
					text
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static int AddNewBuyItemFromNpc(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int moneyType)
		{
			int result;
			if (GameDBManager.DisableSomeLog)
			{
				result = 1;
			}
			else
			{
				int num = -1;
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("INSERT INTO t_npcbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, moneytype) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6})", new object[]
					{
						roleID,
						goodsID,
						goodsNum,
						totalPrice,
						leftMoney,
						text,
						moneyType
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
				}
				result = num;
			}
			return result;
		}

		public static int AddNewYinLiangBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinLiang)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_yinliangbuy (rid, goodsid, goodsnum, totalprice, leftyinliang, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftYinLiang,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewGoldBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftGold)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_goldbuy (rid, goodsid, goodsnum, totalprice, leftgold, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftGold,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewBangGongBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftBangGong)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_banggongbuy (rid, goodsid, goodsnum, totalprice, leftbanggong, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftBangGong,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static void UpdateLastScanInputLogID(DBManager dbMgr, int lastid)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_inputhist (Id, lastid) VALUES(1, {0}) ON DUPLICATE KEY UPDATE lastid={0}", lastid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void UpdateMailHasReadFlag(DBManager dbMgr, int mailID, int rid)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_mail SET isread=1, readtime=now() where mailid={0} and receiverrid={1}", mailID, rid);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static bool UpdateMailHasFetchGoodsFlag(DBManager dbMgr, int mailID, int rid)
		{
			bool result = true;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_mail SET hasfetchattachment=1 where mailid={0} and receiverrid={1}", mailID, rid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool DeleteMailDataItemExcludeGoodsList(DBManager dbMgr, int mailID, int rid)
		{
			bool result = true;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE from t_mail where mailid={0} and receiverrid={1}", mailID, rid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static void DeleteMailGoodsList(DBManager dbMgr, int mailID)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE from t_mailgoods where mailid={0}", mailID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static int AddMailBody(DBManager dbMgr, int senderrid, string senderrname, int receiverrid, string reveiverrname, string subject, string content, int yinliang, int tongqian, int yuanbao)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_mail (senderrid, senderrname, sendtime, receiverrid, reveiverrname, readtime, isread, mailtype, hasfetchattachment, subject,content, yinliang, tongqian, yuanbao) VALUES ({0},'{1}','{2}', {3}, '{4}','{5}',{6},{7},{8},'{9}','{10}',{11},{12}, {13})", new object[]
				{
					senderrid,
					senderrname,
					text,
					receiverrid,
					reveiverrname,
					"2000-11-11 11:11:11",
					0,
					1,
					0,
					subject,
					content,
					yinliang,
					tongqian,
					yuanbao
				});
				int num = myDbConnection.ExecuteNonQuery(sql, 0);
				if (num < 0)
				{
					result = -2;
					return result;
				}
				try
				{
					result = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					result = -3;
				}
			}
			return result;
		}

		public static bool AddMailGoodsDataItem(DBManager dbMgr, int mailID, int goodsid, int forge_level, int quality, string Props, int gcount, int origholenum, int rmbholenum, string jewellist, int addpropindex, int binding, int bornindex, int lucky, int strong, int ExcellenceInfo, int nAppendPropLev, int nEquipChangeLife)
		{
			bool result = true;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_mailgoods (mailid,goodsid,forge_level, quality,Props,gcount,origholenum,rmbholenum,jewellist,addpropindex,binding,bornindex,lucky,strong, excellenceinfo, appendproplev, equipchangelife) VALUES ({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9},{10},{11},{12},{13},{14},{15},{16})", new object[]
				{
					mailID,
					goodsid,
					forge_level,
					quality,
					Props,
					gcount,
					origholenum,
					rmbholenum,
					jewellist,
					addpropindex,
					binding,
					bornindex,
					lucky,
					strong,
					ExcellenceInfo,
					nAppendPropLev,
					nEquipChangeLife
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static void UpdateLastScanMailID(DBManager dbMgr, int roleID, int mailID)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_mailtemp (receiverrid, mailid) VALUES ({0}, {1})", roleID, mailID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void ClearOverdueMails(DBManager dbMgr, DateTime overdueTime)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_mailgoods WHERE mailid IN (SELECT mailid FROM t_mail WHERE sendtime < '{0}');", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
				if (myDbConnection.ExecuteNonQuery(sql, 0) >= 0)
				{
					sql = string.Format("DELETE from t_mail where sendtime < '{0}'", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
		}

		public static void DeleteLastScanMailIDs(DBManager dbMgr, Dictionary<int, int> lastMailDict)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "";
				foreach (KeyValuePair<int, int> keyValuePair in lastMailDict)
				{
					if (text.Length > 0)
					{
						text += " or ";
					}
					else
					{
						text = " where ";
					}
					text += string.Format(" (mailid<={0} and receiverrid={1}) ", keyValuePair.Value, keyValuePair.Key);
				}
				string sql = string.Format("DELETE from t_mailtemp {0}", text);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void DeleteMailIDInMailTemp(DBManager dbMgr, int mailID)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE from t_mailtemp where mailid={0}", mailID);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static bool UpdateRoleLastMail(DBManager dbMgr, int roleID, int mailID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET lastmailid={0} WHERE rid={1}", mailID, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int AddHongDongPaiHangRecord(DBManager dbMgr, int rid, string rname, int zoneid, int huoDongType, int paihang, string paihangtime, int phvalue)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_huodongpaihang (rid, rname, zoneid, type, paihang, paihangtime, phvalue) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})", new object[]
				{
					rid,
					rname,
					zoneid,
					huoDongType,
					paihang,
					paihangtime,
					phvalue
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_huodongawardrolehist (rid, zoneid, activitytype, keystr, hasgettimes,lastgettime) VALUES({0}, {1}, {2}, '{3}', {4}, '{5}')", new object[]
				{
					rid,
					zoneid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_huodongawarduserhist (userid, activitytype, keystr, hasgettimes,lastgettime) VALUES('{0}', {1}, '{2}', {3}, '{4}')", new object[]
				{
					userid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddRegressHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime, string activedata, string stage)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RegressInsertDaySignData, new object[]
				{
					userid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime,
					activedata,
					stage
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRegressHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime, string activedata, string stage)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RegressUpdateDaySignData, new object[]
				{
					activedata,
					lastgettime,
					userid,
					activitytype,
					keystr,
					stage
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_huodongawardrolehist SET hasgettimes={0}, lastgettime='{1}' WHERE rid={2} AND zoneid={3} AND activitytype={4} AND keystr='{5}' AND hasgettimes!={0}", new object[]
				{
					hasgettimes,
					lastgettime,
					rid,
					zoneid,
					activitytype,
					keystr,
					hasgettimes
				});
				result = ((myDbConnection.ExecuteNonQuery(sql, 0) > 0) ? 0 : -1);
			}
			return result;
		}

		public static int UpdateHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("update t_huodongawarduserhist set hasgettimes={0}, lastgettime='{1}' where userid='{2}' and activitytype={3} and keystr='{4}' and hasgettimes!={5}", new object[]
				{
					hasgettimes,
					lastgettime,
					userid,
					activitytype,
					keystr,
					hasgettimes
				});
				result = ((myDbConnection.ExecuteNonQuery(sql, 0) > 0) ? 0 : -1);
			}
			return result;
		}

		public static int AddLimitGoodsBuyItem(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_limitgoodsbuy (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayID={2}, usedNum={3}", new object[]
				{
					roleID,
					goodsID,
					dayID,
					usedNum
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddVipDailyData(DBManager dbMgr, int roleID, int priorityType, int dayID, int usedTimes)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_vipdailydata (rid, prioritytype, dayid, usedtimes) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usedtimes={3}", new object[]
				{
					roleID,
					priorityType,
					dayID,
					usedTimes
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddYangGongBKDailyJiFenData(DBManager dbMgr, int roleID, int jifen, int dayID, long awardhistory)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_yangguangbkdailydata (rid, jifen, dayid, awardhistory) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE jifen={1}, dayid={2}, awardhistory={3}", new object[]
				{
					roleID,
					jifen,
					dayID,
					awardhistory
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int ChangeTablesAutoIncrementValue(DBManager dbMgr, string sTableName, int nAutoIncrementValue)
		{
			int result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("alter table {0} auto_increment={1}", sTableName, nAutoIncrementValue);
				myDbConnection.ExecuteNonQuery(sql, 0);
				result = 0;
			}
			return result;
		}

		public static bool UpdateGoodsLimit(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_goodslimit (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usednum={3}", new object[]
				{
					roleID,
					goodsID,
					dayID,
					usedNum
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleParams(DBManager dbMgr, int roleID, string name, string value, RoleParamType roleParamType = null)
		{
			if (roleParamType == null)
			{
				roleParamType = RoleParamNameInfo.GetRoleParamType(name, value);
			}
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO `{3}` (`rid`, `{4}`, `{5}`) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE `{5}`='{2}'", new object[]
				{
					roleID,
					roleParamType.KeyString,
					value,
					roleParamType.TableName,
					roleParamType.IdxName,
					roleParamType.ColumnName
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static string GetRoleParams(int roleID, string name)
		{
			RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(name, null);
			string result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT {2} FROM `{3}` WHERE rid={0} AND idx={1}", new object[]
				{
					roleID,
					roleParamType.IdxKey,
					roleParamType.ColumnName,
					roleParamType.TableName
				});
				object single = myDbConnection.GetSingle(sql, 0, new MySQLParameter[0]);
				result = ((single == null) ? "" : single.ToString());
			}
			return result;
		}

		public static bool UpdateWebOldPlayer(string roleID, string chouJiangType, string addDay)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO `t_weboldplayer` SET rid={0}, {1}=1, addday='{2}' ON DUPLICATE KEY UPDATE {1}=1", roleID, chouJiangType, addDay);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId, int actStartDay)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_qianggoubuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, qianggouid, actstartday) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7})", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					text,
					qiangGouId,
					actStartDay
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewQiangGouItem(DBManager dbMgr, int group, int random, int itemid, int goodsid, int origprice, int price, int singlepurchase, int fullpurchase, int daystime)
		{
			int num = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_qianggouitem (itemgroup, random, itemid, goodsid, origprice, price, singlepurchase, fullpurchase, daystime, starttime, endtime, istimeover) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}','{10}', {11})", new object[]
				{
					group,
					random,
					itemid,
					goodsid,
					origprice,
					price,
					singlepurchase,
					fullpurchase,
					daystime,
					text,
					text,
					0
				});
				num = myDbConnection.ExecuteNonQuery(sql, 0);
				try
				{
					if (num >= 0)
					{
						num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		public static bool UpdateQiangGouItemTimeOverFlag(DBManager dbMgr, int qiangGouId)
		{
			bool result = false;
			string arg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_qianggouitem SET istimeover=1, endtime='{0}' WHERE Id={1}", arg, qiangGouId);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int AddNewZaJinDanHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int timesselected, int usedyuanbao, int usedjindan, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp, string srtProp)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_zajindanhist (rid, rname, zoneid, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, strprop, operationtime) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},'{11}','{12}')", new object[]
				{
					roleID,
					roleName,
					zoneID,
					timesselected,
					usedyuanbao,
					usedjindan,
					gaingoodsid,
					gaingoodsnum,
					gaingold,
					gainyinliang,
					gainexp,
					srtProp,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddKaiFuOnlineAward(DBManager dbMgr, int rid, int dayID, int yuanBao, int totalRoleNum, int zoneID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_kfonlineawards (rid, dayid, yuanbao, totalrolenum, zoneid) VALUES ({0},{1},{2},{3},{4}) ON DUPLICATE KEY UPDATE rid={0}, yuanbao={2}, totalrolenum={3}", new object[]
				{
					rid,
					dayID,
					yuanBao,
					totalRoleNum,
					zoneID
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddSystemGiveUserMoney(DBManager dbMgr, int rid, int yuanBao, string giveType)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_givemoney (rid, yuanbao, rectime, givetype) VALUES ({0},{1},'{2}','{3}')", new object[]
				{
					rid,
					yuanBao,
					text,
					giveType
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddExchange1Item(DBManager dbMgr, int rid, int goodsid, int goodsnum, int leftgoodsnum, int otherroleid, string result)
		{
			int result2 = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_exchange1 (rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, rectime) VALUES ({0},{1},{2},{3},{4},'{5}','{6}')", new object[]
				{
					rid,
					goodsid,
					goodsnum,
					leftgoodsnum,
					otherroleid,
					result,
					text
				});
				result2 = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result2;
		}

		public static int AddExchange2Item(DBManager dbMgr, int rid, int yinliang, int leftyinliang, int otherroleid)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_exchange2 (rid, yinliang, leftyinliang, otherroleid, rectime) VALUES ({0},{1},{2},{3},'{4}')", new object[]
				{
					rid,
					yinliang,
					leftyinliang,
					otherroleid,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddExchange3Item(DBManager dbMgr, int rid, int yuanbao, int leftyuanbao, int otherroleid)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_exchange3 (rid, yuanbao, leftyuanbao, otherroleid, rectime) VALUES ({0},{1},{2},{3},'{4}')", new object[]
				{
					rid,
					yuanbao,
					leftyuanbao,
					otherroleid,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddFallGoodsItem(DBManager dbMgr, int rid, int autoid, int goodsdbid, int goodsid, int goodsnum, int binding, int quality, int forgelevel, string jewellist, string mapname, string goodsgrid, string fromname)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_fallgoods (rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname, rectime) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}','{10}','{11}','{12}')", new object[]
				{
					rid,
					autoid,
					goodsdbid,
					goodsid,
					goodsnum,
					binding,
					quality,
					forgelevel,
					jewellist,
					mapname,
					goodsgrid,
					fromname,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int AddNewYueDuChouJiangHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp)
		{
			int result = -1;
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_yueduchoujianghist (rid, rname, zoneid, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, operationtime) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},'{8}')", new object[]
				{
					roleID,
					roleName,
					zoneID,
					gaingoodsid,
					gaingoodsnum,
					gaingold,
					gainyinliang,
					gainexp,
					text
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleOccupation(DBManager dbMgr, int roleID, int nOccu)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET occupation={0} WHERE rid={1}", nOccu, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateBloodCastleEnterCount(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, string lastgettime)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, lastgettime) VALUES({0}, {1}, {2}, {3}, '{4}')", new object[]
				{
					nRoleID,
					nType,
					nDate,
					nCount,
					lastgettime
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleInfoForFlashPlayerFlag(DBManager dbMgr, int nRoleID, int isflashplayer)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET isflashplayer={1} WHERE rid={0}", nRoleID, isflashplayer);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleExpForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string arg = "";
				string sql = string.Format("UPDATE t_roles SET experience={1}, maintaskid={1}, main_quick_keys='{2}' WHERE rid={0}", nRoleID, 0, arg);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleLevForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, 1);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleGoodsForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_goods WHERE rid = {0}", nRoleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleTasksForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_tasks WHERE rid = {0}", nRoleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
				sql = string.Format("DELETE FROM t_taskslog WHERE rid = {0}", nRoleID);
				myDbConnection.ExecuteNonQuery(sql, 0);
				result = true;
			}
			return result;
		}

		public static bool UpdateRoleTasksStarLevel(DBManager dbMgr, int nRoleID, int taskid, int StarLevel)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_tasks SET starlevel={2} WHERE rid = {0} AND Id = {1}", nRoleID, taskid, StarLevel);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleChangeLifeInfo(DBManager dbMgr, int nRoleID, int ChangeCount)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET changelifecount={1} WHERE rid={0}", nRoleID, ChangeCount);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleAdmiredInfo1(DBManager dbMgr, int nRoleID, int nCount)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET admiredcount={1} WHERE rid={0}", nRoleID, nCount);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleAdmiredInfo2(DBManager dbMgr, int nRoleID1, int nRoleID2, int nDate)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_adorationinfo(roleid, adorationroleid, dayid) VALUES({0}, {1}, {2})", nRoleID1, nRoleID2, nDate);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleCombatForce(DBManager dbMgr, int roleID, int CombatForce)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET combatforce={0} WHERE rid={1}", CombatForce, roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleLevel(DBManager dbMgr, int nRoleID, int lev)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, lev);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleDayActivityPoint(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, long nValue)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, totalpoint, lastgettime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					nRoleID,
					nType,
					nDate,
					nCount,
					nValue,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool DeleteRoleDayActivityInfo(DBManager dbMgr, int roleID, int activityid)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", roleID, activityid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int SetRoleAutoAssignPropertyPoint(DBManager dbMgr, int roleID, int nFlag)
		{
			int result = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET autoassignpropertypoint = {0} WHERE rid = {1}", nFlag, roleID);
				result = (myDbConnection.ExecuteNonQueryBool(sql, 0) ? 1 : 0);
			}
			return result;
		}

		public static int SetUserPushMessageID(DBManager dbMgr, string strUser, string strPushMegID)
		{
			int result = 0;
			string arg = DateTime.Now.ToString("yyyy-MM-dd");
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_pushmessageinfo(userid, pushid, lastlogintime) VALUE('{0}', '{1}', '{2}')", strUser, strPushMegID, arg);
				result = (myDbConnection.ExecuteNonQueryBool(sql, 0) ? 1 : 0);
			}
			return result;
		}

		public static int NewWing(DBManager dbMgr, int roleID, int wingID, int forgeLevel, string addtime, string strRoleName, int nOccupation)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_wings (rid, rname, occupation, wingid, forgeLevel, addtime, isdel, failednum, equiped, zhulingnum, zhuhunnum) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10})", new object[]
				{
					roleID,
					strRoleName,
					nOccupation,
					wingID,
					forgeLevel,
					addtime,
					0,
					0,
					0,
					0,
					0
				});
				bool flag = !myDbConnection.ExecuteNonQueryBool(sql, 0);
				try
				{
					if (!flag)
					{
						result = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					result = -2;
				}
			}
			return result;
		}

		public static int UpdateWing(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateWing_fieldNames, "t_wings", DBWriter._UpdateWing_fieldTypes, "Id");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateLingYu(DBManager dbMgr, int roleID, int type, int level, int suit)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_lingyu(roleid, type, level, suit) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					roleID,
					type,
					level,
					suit
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleReferPictureJudgeInfo(DBManager dbMgr, int roleid, int nPictureJudgeID, int nNum)
		{
			int result = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_picturejudgeinfo(roleid, picturejudgeid, refercount) VALUES({0}, {1}, {2})", roleid, nPictureJudgeID, nNum);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nExchangeid, int nDayID, int nNum)
		{
			int result = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_mojingexchangeinfo(roleid, exchangeid, exchangenum, dayid) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					nRoleid,
					nExchangeid,
					nNum,
					nDayID
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateResourceGetInfo(DBManager dbMgr, int nRoleid, int type, OldResourceInfo info)
		{
			int result = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (info == null)
				{
					sql = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget,yuansufenmo) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})", new object[]
					{
						nRoleid,
						type,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0
					});
				}
				else
				{
					sql = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget,yuansufenmo) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})", new object[]
					{
						nRoleid,
						info.type,
						info.exp,
						info.leftCount,
						info.mojing,
						info.bandmoney,
						info.zhangong,
						info.chengjiu,
						info.shengwang,
						info.bandDiamond,
						info.xinghun,
						0,
						info.yuanSuFenMo
					});
				}
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateGoods2(DBManager dbMgr, int roleID, GoodsData gd, UpdateGoodsArgs args)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder("UPDATE t_goods SET ");
				foreach (UpdatePropIndexes updatePropIndexes in args.ChangedIndexes)
				{
					if (updatePropIndexes == UpdatePropIndexes.WashProps)
					{
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						string arg = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(args.WashProps));
						stringBuilder.AppendFormat("{0}='{1}'", "washprops", arg);
						flag = true;
					}
					else if (updatePropIndexes == UpdatePropIndexes.ElementhrtsProps)
					{
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						string arg2 = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(args.ElementhrtsProps));
						stringBuilder.AppendFormat("{0}='{1}'", "ehinfo", arg2);
						flag = true;
					}
					else if (updatePropIndexes == UpdatePropIndexes.JuHun)
					{
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.AppendFormat("{0}='{1}'", "juhun", args.JuHunProps);
						flag = true;
					}
					else if (updatePropIndexes < UpdatePropIndexes.MaxBaseIndex)
					{
						if (updatePropIndexes == UpdatePropIndexes.binding)
						{
							DBWriter.AppendSQLForGoodsProps(stringBuilder, (int)updatePropIndexes, args.Binding, DBWriter._UpdateGoods2_fieldNames, DBWriter._UpdateGoods2_fieldTypes, flag);
							flag = true;
						}
					}
				}
				if (flag)
				{
					stringBuilder.AppendFormat(" WHERE {0}={1} and rid={2}", "Id", args.DbID, args.RoleID);
					string sql = stringBuilder.ToString();
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num >= 0)
					{
						args.CopyPropsTo(gd);
					}
				}
			}
			return num;
		}

		public static void AppendSQLForGoodsProps(StringBuilder sb, int index, object value, string[] fieldNames, byte[] fieldTypes, bool hasAppend)
		{
			if (hasAppend)
			{
				sb.Append(", ");
			}
			if (0 == fieldTypes[index])
			{
				sb.AppendFormat("{0}={1}", fieldNames[index], value);
			}
			else if (1 == fieldTypes[index])
			{
				sb.AppendFormat("{0}='{1}'", fieldNames[index], value);
			}
			else if (2 == fieldTypes[index])
			{
				sb.AppendFormat("{0}={0}+{1}", fieldNames[index], value);
			}
			else if (3 == fieldTypes[index])
			{
				sb.AppendFormat("{0}='{1}'", fieldNames[index], value.ToString().Replace('$', ':'));
			}
		}

		public static int UpdateRoleStarConstellationInfo(DBManager dbMgr, int roleid, int nStarSite, int nStarSlot)
		{
			int result = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_starconstellationinfo(roleid, starsiteid, starslotid) VALUES({0}, {1}, {2})", roleid, nStarSite, nStarSlot);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int SaveConsumeLog(DBManager dbMgr, int roleid, string cdate, int amount)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_consumelog (rid, amount, cdate) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE amount=amount+{1};", roleid, amount, cdate);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateVipLevelAwardFlagInfo(DBManager dbMgr, string strUserid, int nFlag, int nZoneID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE userid='{1}' and zoneid = {2}", nFlag, strUserid, nZoneID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateVipLevelAwardFlagInfoByRoleID(DBManager dbMgr, int nRoleid, int nFlag, int nZoneID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE rid={1} and zoneid = {2}", nFlag, nRoleid, nZoneID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int ExecuteSQLReadInt(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
		{
			int result = 0;
			bool flag = true;
			try
			{
				if (conn == null)
				{
					flag = false;
					conn = dbMgr.DBConns.PopDBConnection();
				}
				MySQLCommand mySQLCommand2;
				MySQLCommand mySQLCommand = mySQLCommand2 = new MySQLCommand(sqlText, conn);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						result = Convert.ToInt32(mySQLDataReader[0].ToString());
					}
					mySQLDataReader.Close();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return -1;
				}
				finally
				{
					if (mySQLCommand2 != null)
					{
						mySQLCommand2.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -2;
			}
			finally
			{
				if (!flag && null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		public static int ValidateDatabase(DBManager dbMgr, string dbName)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_roles_auto_increment", 0);
				if (gameConfigItemInt < 200000 || gameConfigItemInt % 100000 != 0)
				{
					Global.LogAndExitProcess("flag_t_roles_auto_increment 未设置");
				}
				int num = 0;
				string[] array = Global.GetLocalAddressIPs().Split(new char[]
				{
					'_'
				});
				foreach (string text in array)
				{
					int num2 = text.IndexOf('.');
					if (num2 > 0)
					{
						num2 = text.IndexOf('.', num2 + 1);
						if (num2 > 0)
						{
							string key = text.Substring(0, num2);
							if (GameDBManager.IPRange2AutoIncreaseStepDict.TryGetValue(key, out num))
							{
								break;
							}
						}
					}
				}
				if (num > 0)
				{
					if (num != gameConfigItemInt)
					{
						Global.LogAndExitProcess("flag_t_roles_auto_increment 不正确");
					}
				}
				else if (num != 0 && 200000 != gameConfigItemInt)
				{
					Global.LogAndExitProcess("flag_t_roles_auto_increment 不正确");
				}
				GameDBManager.DBAutoIncreaseStepValue = gameConfigItemInt;
				foreach (string[] array4 in DBWriter.ValidateDatabaseTables)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = '{1}' AND column_name='{2}' limit 1;", dbName, array4[0], array4[1]);
					int num3 = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, mySQLConnection);
					if (num3 <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少列：'{1}'", array4[0], array4[1]));
					}
				}
				foreach (string[] array4 in DBWriter.ValidateDatabaseTableRows)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM {0} {1} limit 1;", array4[0], array4[1]);
					int num3 = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, mySQLConnection);
					if (num3 <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 缺少：{1}", array4[0], array4[2]));
					}
				}
				foreach (string[] array4 in DBWriter.ValidateDatabaseTableIndexes)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.statistics WHERE table_schema='{0}' AND TABLE_NAME='{1}' AND INDEX_NAME='{2}' limit 1;", dbName, array4[0], array4[1]);
					int num3 = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, mySQLConnection);
					if (num3 <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", array4[0], array4[1]));
					}
				}
				DBWriter.SwitchGoodsBackupTable(dbMgr);
				DBWriter.ClearOverdueTianTiItemLog(dbMgr, TimeUtil.NowDateTime().AddMonths(-3));
				DBWriter.ClearBigTable_NameCheck(dbMgr);
				if (!DBWriter.CheckMoneyCC(dbMgr, dbName, mySQLConnection))
				{
					return 0;
				}
			}
			catch (MySQLException ex)
			{
				LogManager.WriteException(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
				throw new Exception(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return 1;
		}

		private static bool CheckTableColumn(DBManager dbMgr, string dbName, string tableName, string columnName, MySQLConnection conn = null)
		{
			string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = '{1}' AND column_name='{2}' limit 1;", dbName, tableName, columnName);
			int num = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
			return num > 0;
		}

		public static bool CheckMoneyCC(DBManager dbMgr, string dbName, MySQLConnection conn = null)
		{
			if (!DBWriter.CheckTableColumn(dbMgr, dbName, "t_money", "cc", conn))
			{
				if (0 > DBWriter.ExecuteSQLNoQuery(dbMgr, "ALTER TABLE `t_money` ADD COLUMN `cc` CHAR(64) CHARSET utf8 COLLATE utf8_general_ci NULL AFTER `realmoney`;", conn))
				{
					Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", "t_money", "cc"));
					return false;
				}
				string text = string.Format("SELECT userid, money, realmoney,cc FROM t_money", new object[0]);
				using (MySQLCommand mySQLCommand = new MySQLCommand(text, conn))
				{
					using (MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx())
					{
						while (mySQLDataReader.Read())
						{
							string text2 = mySQLDataReader["userid"].ToString();
							int num = Convert.ToInt32(mySQLDataReader["money"].ToString());
							int num2 = Convert.ToInt32(mySQLDataReader["realmoney"].ToString());
							string text3 = mySQLDataReader["cc"].ToString();
							if (string.IsNullOrEmpty(text3))
							{
								string arg = Global.GCC(3, new object[]
								{
									text2,
									num,
									num2
								});
								text = string.Format("update t_money set cc='{0}' where userid='{1}';", arg, text2);
								if (DBWriter.ExecuteSQLNoQuery(dbMgr, text, conn) >= 0)
								{
									LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCorrect#t_money#userid={0},money={1},cc={2}", text2, num, text3), null, true);
									GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
								}
							}
						}
					}
				}
			}
			if (!DBWriter.CheckTableColumn(dbMgr, dbName, "t_tempmoney", "cc", conn))
			{
				if (0 > DBWriter.ExecuteSQLNoQuery(dbMgr, "ALTER TABLE `t_tempmoney` ADD COLUMN `cc` CHAR(64) CHARSET utf8 COLLATE utf8_general_ci NULL AFTER `id`;", conn))
				{
					Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", "t_tempmoney", "cc"));
					return false;
				}
			}
			return true;
		}

		public static int ClearUnusedGoodsData(DBManager dbMgr, bool clearAll = false)
		{
			int num = -1;
			int num2 = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				int num3 = 0;
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				try
				{
					MySQLCommand mySQLCommand2;
					MySQLCommand mySQLCommand = mySQLCommand2 = new MySQLCommand("SELECT MAX(id) FROM t_goods", mySQLConnection);
					try
					{
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						if (mySQLDataReader.Read())
						{
							num = Convert.ToInt32(mySQLDataReader[0].ToString());
						}
						mySQLDataReader.Close();
					}
					catch
					{
						return -1;
					}
					finally
					{
						if (mySQLCommand2 != null)
						{
							mySQLCommand2.Dispose();
						}
					}
					mySQLCommand = (mySQLCommand2 = new MySQLCommand("SELECT MIN(id) FROM t_goods", mySQLConnection));
					try
					{
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						if (mySQLDataReader.Read())
						{
							num2 = Convert.ToInt32(mySQLDataReader[0].ToString());
						}
						mySQLDataReader.Close();
					}
					catch
					{
						return -1;
					}
					finally
					{
						if (mySQLCommand2 != null)
						{
							mySQLCommand2.Dispose();
						}
					}
				}
				catch (MySQLException ex)
				{
					LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
					return -1;
				}
				try
				{
					int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("last_clear_goods_dbid", 0);
					if (gameConfigItemInt > num2)
					{
						num2 = gameConfigItemInt;
					}
					int gameConfigItemInt2 = GameDBManager.GameConfigMgr.GetGameConfigItemInt("max_clear_goods_count", 1);
					num3 = num2 + gameConfigItemInt2;
					if (num < num3)
					{
						return 0;
					}
					if (clearAll)
					{
						num3 = num;
					}
					GameDBManager.GameConfigMgr.UpdateGameConfigItem("last_goods_dbid", num3.ToString());
					DBWriter.UpdateGameConfig(dbMgr, "last_goods_dbid", num3.ToString());
					string text = string.Format("INSERT INTO t_goods_bak SELECT *,0,NOW(),0 FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", num2, num3);
					MySQLCommand mySQLCommand2;
					MySQLCommand mySQLCommand = mySQLCommand2 = new MySQLCommand(text, mySQLConnection);
					try
					{
						mySQLCommand.ExecuteNonQuery();
					}
					finally
					{
						if (mySQLCommand2 != null)
						{
							mySQLCommand2.Dispose();
						}
					}
					text = string.Format("DELETE FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", num2, num3);
					mySQLCommand = (mySQLCommand2 = new MySQLCommand(text, mySQLConnection));
					try
					{
						mySQLCommand.ExecuteNonQuery();
					}
					finally
					{
						if (mySQLCommand2 != null)
						{
							mySQLCommand2.Dispose();
						}
					}
				}
				catch (MySQLException ex)
				{
					LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
					return -1;
				}
			}
			catch (Exception ex2)
			{
				LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex2.ToString()));
				return -1;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return 1;
		}

		public static int ClearBigTable_NameCheck(DBManager dbMgr)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_name_check;", new object[0]);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int insertItemLog(DBManager dbMgr, string[] fields)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "t_usemoney_log";
				string sql = string.Format("INSERT INTO {0} (DBId, userid, ObjName, optFrom, currEnvName, tarEnvName, optType, optTime, optAmount, zoneID, optSurplus) VALUES({1}, '{9}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, {8}, {10})", new object[]
				{
					text,
					fields[0],
					fields[1],
					fields[2],
					fields[3],
					fields[4],
					fields[5],
					fields[6],
					fields[7],
					fields[8],
					fields[9]
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateRoleKuaFuDayLog(DBManager dbMgr, RoleKuaFuDayLogData data)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_kf_day_role_log (gametype,day,rid,zoneid,signup_count, start_game_count, success_count, faild_count) VALUES({0},'{1}',{2},{3},{4},{5},{6},{7}) on duplicate key update zoneid={3},signup_count=signup_count+{4},start_game_count=start_game_count+{5},success_count=success_count+{6},faild_count=faild_count+{7}", new object[]
				{
					data.GameType,
					data.Day,
					data.RoleID,
					data.ZoneId,
					data.SignupCount,
					data.StartGameCount,
					data.SuccessCount,
					data.FaildCount
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int InsertTianTiItemLog(DBManager dbMgr, TianTiLogItemData data)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "t_kf_tianti_game_log";
				string sql = string.Format("INSERT INTO {0} (rid,zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime) VALUES({1},{2},'{3}',{4},'{5}',{6},{7},{8},'{9}')", new object[]
				{
					text,
					data.RoleId,
					data.ZoneId1,
					data.RoleName1,
					data.ZoneId2,
					data.RoleName2,
					data.Success,
					data.DuanWeiJiFenAward,
					data.RongYaoAward,
					data.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int InsertKF5v5ItemLog(DBManager dbMgr, TianTiLogItemData data)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "t_5v5_game_log";
				string sql = string.Format("INSERT INTO {0} (rid,zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime) VALUES({1},{2},'{3}',{4},'{5}',{6},{7},{8},'{9}')", new object[]
				{
					text,
					data.RoleId,
					data.ZoneId1,
					data.RoleName1,
					data.ZoneId2,
					data.RoleName2,
					data.Success,
					data.DuanWeiJiFenAward,
					data.RongYaoAward,
					data.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int ClearOverdueTianTiItemLog(DBManager dbMgr, DateTime minTime)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_kf_tianti_game_log WHERE endtime < '{0}'", minTime.ToString("yyyy-MM-dd"));
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateTianTiRoleData(DBManager dbMgr, RoleTianTiData data)
		{
			int result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_kf_tianti_role (rid,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,todayfightcount,lastfightdayid,monthduanweirank,fetchmonthawarddate,rongyao) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},'{10}',{11}) ", new object[]
				{
					data.RoleId,
					data.DuanWeiId,
					data.DuanWeiJiFen,
					data.DuanWeiRank,
					data.LianSheng,
					data.FightCount,
					data.SuccessCount,
					data.TodayFightCount,
					data.LastFightDayId,
					data.MonthDuanWeiRank,
					data.FetchMonthDuanWeiRankAwardsTime.ToString("yyyy-MM-dd"),
					data.RongYao
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int UpdateTianTiRoleRongYao(DBManager dbMgr, int rid, int rongYao)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_kf_tianti_role SET rongyao={1} WHERE rid={0};", rid, rongYao);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static void InsertCityInfo(DBManager dbMgr, string ip, string userid)
		{
			IPInfo ipinfo = CityNameManager.ParseIP(ip);
			if (null != ipinfo)
			{
				string regionName = ipinfo.RegionName;
				string cityName = ipinfo.CityName;
				int offsetDay = Global.GetOffsetDay(DateTime.Now);
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}') ON DUPLICATE KEY UPDATE lastip='{4}'", new object[]
					{
						userid,
						offsetDay,
						regionName,
						cityName,
						ip,
						text,
						Global.GetDayEndTime(DateTime.Now)
					});
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
		}

		public static void UpdateCityInfoItem(DBManager dbMgr, string ip, string userid, string fieldName, int addVal)
		{
			IPInfo ipinfo = CityNameManager.ParseIP(ip);
			if (null != ipinfo)
			{
				string regionName = ipinfo.RegionName;
				string cityName = ipinfo.CityName;
				int offsetDay = Global.GetOffsetDay(DateTime.Now);
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime, {7}) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', {8}) ON DUPLICATE KEY UPDATE lastip='{4}', {7}={7}+{8}", new object[]
					{
						userid,
						offsetDay,
						regionName,
						cityName,
						ip,
						Global.GetDayStartTime(DateTime.Now),
						Global.GetDayEndTime(DateTime.Now),
						fieldName,
						addVal
					});
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
		}

		public static void UpdateCityInfoLogoutTime(DBManager dbMgr, string ip, string userid, int onlineSecs, int activeVal)
		{
			IPInfo ipinfo = CityNameManager.ParseIP(ip);
			if (null != ipinfo)
			{
				string regionName = ipinfo.RegionName;
				string cityName = ipinfo.CityName;
				int offsetDay = Global.GetOffsetDay(DateTime.Now);
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime, onlinesecs, activeval) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}) ON DUPLICATE KEY UPDATE logouttime='{6}', onlinesecs=onlinesecs+{7}, activeval=activeval+{8}", new object[]
					{
						userid,
						offsetDay,
						regionName,
						cityName,
						ip,
						Global.GetDayStartTime(DateTime.Now),
						text,
						onlineSecs,
						activeVal
					});
					myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
		}

		public static bool UpdateUsrSecondPassword(DBManager dbMgr, string usrid, string secPwd)
		{
			bool result;
			if (string.IsNullOrEmpty(usrid))
			{
				result = false;
			}
			else
			{
				bool flag = false;
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql;
					if (!string.IsNullOrEmpty(secPwd))
					{
						sql = string.Format("REPLACE INTO t_secondpassword(userid, secpwd) VALUES('{0}','{1}')", usrid, secPwd);
					}
					else
					{
						sql = string.Format("DELETE FROM t_secondpassword WHERE userid='{0}'", usrid);
					}
					flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
				result = flag;
			}
			return result;
		}

		public static bool UpdateMarriageData(DBManager dbMgr, int nRoleID, int nSpouseID, sbyte byMarrytype, int nRingID, int nGoodwillexp, sbyte byGoodwillStar, sbyte byGoodwilllevel, int nGivenrose, string strLovemessage, sbyte byAutoReject, string changtime)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_marry(roleid, spouseid, marrytype, ringid, goodwillexp, goodwillstar, goodwilllevel, givenrose, lovemessage, autoreject, changtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, '{10}')", new object[]
				{
					nRoleID,
					nSpouseID,
					byMarrytype,
					nRingID,
					nGoodwillexp,
					byGoodwillStar,
					byGoodwilllevel,
					nGivenrose,
					strLovemessage,
					byAutoReject,
					changtime
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool AddMarryParty(DBManager dbMgr, int roleID, int partyType, string startTime, int husbandRoleID, int wifeRoleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_marryparty (roleid, partytype, joinCount, startTime, husbandid, wifeid) VALUES({0}, {1}, {2}, '{3}', {4}, {5})", new object[]
				{
					roleID,
					partyType,
					0,
					startTime,
					husbandRoleID,
					wifeRoleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool RemoveMarryParty(DBManager dbMgr, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_marryparty WHERE roleid={0}", roleID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool IncMarryPartyJoin(DBManager dbMgr, int roleID, int joinerID, int joinCount)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_marryparty SET joincount=joincount+1 WHERE roleid={0}", roleID);
				if (myDbConnection.ExecuteNonQueryBool(sql, 0))
				{
					result = true;
				}
				sql = string.Format("REPLACE INTO t_marryparty_join(roleid, partyroleid, joincount) VALUES({0}, {1}, {2})", joinerID, roleID, joinCount);
				if (myDbConnection.ExecuteNonQueryBool(sql, 0))
				{
					result = true;
				}
			}
			return result;
		}

		public static bool ClearMarryPartyJoin(DBManager dbMgr, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql;
				if (roleID <= 0)
				{
					sql = string.Format("delete from t_marryparty_join", new object[0]);
				}
				else
				{
					sql = string.Format("delete from t_marryparty_join WHERE roleid = {0}", roleID);
				}
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool DeleteChargeItemInfo(DBManager dbMgr, int id, byte isDel)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_tempitem SET isdel = {0} WHERE id = {1}", isDel, id);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int ModifyGMailRecord(DBManager dbMgr, int roleID, int gmailID, int mailID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "t_rolegmail_record";
				string sql = string.Format("REPLACE INTO {0} (roleid, gmailid, mailid) VALUES({1}, {2}, {3})", new object[]
				{
					text,
					roleID,
					gmailID,
					mailID
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool UpdateHolyItemData(DBManager dbMgr, int nRoleID, sbyte sShengwu_type, sbyte sPart_slot, sbyte sPart_suit, int nPart_slice, int nFail_count)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_holyitem(roleid, shengwu_type, part_slot, part_suit, part_slice, fail_count) VALUES({0}, {1}, {2}, {3}, {4}, {5})", new object[]
				{
					nRoleID,
					sShengwu_type,
					sPart_slot,
					sPart_suit,
					nPart_slice,
					nFail_count
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateTarotData(DBManager dbMgr, int nRoleID, string tarotInfo, string kingInfo)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_tarot(roleid, tarotinfo, kingbuff) VALUES({0}, '{1}','{2}')", nRoleID, tarotInfo, kingInfo);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateTenState(DBManager dbMgr, int id, int state)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_ten SET state={0} WHERE id={1}", state, id);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool ActivateStateSet(DBManager dbMgr, int zoneID, string userID, int roleID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_activate (userID, zoneID, roleID, logTime) VALUES('{0}', {1}, {2}, '{3}')", new object[]
				{
					userID,
					zoneID,
					roleID,
					DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateGiftCodeState(DBManager dbMgr, int id, int mailid, string time)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_giftcode SET mailid={0},usetime='{1}' WHERE id={2}", mailid, time, id);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int InsertChargeTempItem(DBManager dbMgr, string UserID, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_tempitem (uid, rid, addmoney, itemid, chargetime) VALUES('{0}', {1}, {2}, {3}, '{4}')", new object[]
				{
					UserID,
					chargeRoleID,
					addUserMoney,
					zhigouID,
					chargeTm
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static bool Insert2OrderNo(DBManager dbMgr, string order_no)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("INSERT INTO `t_order` (`order_no`) VALUES ('{0}');", order_no);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
					result = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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
			return result;
		}

		public static bool Insert2InputLog(DBManager dbMgr, RechargeData rechargeData)
		{
			bool result;
			if (rechargeData.BudanFlag == 2)
			{
				rechargeData.RoleID = -1;
				result = true;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				bool flag = false;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("INSERT INTO `{12}` (`amount`, `u`, `rid`, `order_no`, `cporder_no`, \r\n                    `time`, `sign`, `inputtime`, `result`, `zoneid`, `itemid`, `chargetime`) \r\n                    VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', \r\n                    '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');", new object[]
					{
						rechargeData.Amount,
						rechargeData.UserID,
						rechargeData.RoleID,
						rechargeData.order_no,
						rechargeData.cporder_no,
						rechargeData.Time,
						rechargeData.Sign,
						rechargeData.ChargeTime,
						"success",
						rechargeData.ZoneID,
						rechargeData.ItemId,
						rechargeData.ChargeTime,
						(rechargeData.BudanFlag != 1) ? "t_inputlog" : "t_inputlog2"
					});
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					try
					{
						mySQLCommand.ExecuteNonQuery();
						flag = true;
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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

		public static bool Insert2TempMoney(DBManager dbMgr, TempMoneyInfo info)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("INSERT INTO `t_tempmoney` (`cc`, `uid`, `rid`, `addmoney`, `itemid`, `budanflag`, `chargetime`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", new object[]
				{
					info.cc,
					info.userID,
					info.chargeRoleID,
					info.addUserMoney,
					info.addUserItem,
					info.budanflag,
					info.chargeTm
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
					result = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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
			return result;
		}

		public static bool UpdateFacebookState(DBManager dbMgr, int id, int state)
		{
			bool result = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("UPDATE t_facebook SET state={0} WHERE id={1}", state, id);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
				result = true;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool InsertFoMoMailData(DBManager dbMgr, int sendid, string sendername, int sendjob, int recrid, int num, string content, string today)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.InsertOneMail, new object[]
				{
					sendid,
					sendername,
					sendjob,
					today,
					recrid,
					0,
					"2000-11-11 11:11:11",
					num,
					content
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
					result = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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
			return result;
		}

		public static bool UpdateIsReadFoMoMailData(DBManager dbMgr, int mailid, string today)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.UpdateReadState, 1, today, mailid);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
					result = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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
			return result;
		}

		public static bool InsertFoMoMailDataTemp(DBManager dbMgr, int sendid, string recrid_list, int nDate, int accept, int give)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.InsertMapList, new object[]
				{
					nDate,
					sendid,
					recrid_list,
					accept,
					give
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
					result = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", text), null, true);
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
			return result;
		}

		public static bool UpdateRoleStoreFuMoMoneyAcceptNum(DBManager dbMgr, int roleID, int nDate, int acceptnum)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateMapAccept, acceptnum, roleID, nDate);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRoleStoreFuMoMoneyGiveNum(DBManager dbMgr, int roleID, int givenum, int nDate, string recid_list)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateMapGive, new object[]
				{
					givenum,
					recid_list,
					roleID,
					nDate
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool DeleteMailFuMoByMailID(DBManager dbMgr, int mailID)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.DeleteMailByID, mailID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool DeleteMailFuMoByMailIDList(DBManager dbMgr, int rid, string mailIDList)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.DeleteMailByIDList, mailIDList, rid);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRebornYinJiInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.UpdateRebornYinJi, new object[]
				{
					RoleID,
					StampInfo,
					ResetNum,
					UsePoint
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool InsertRebornYinJiInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.InsertRebornYinJi, new object[]
				{
					RoleID,
					StampInfo,
					ResetNum,
					UsePoint
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static int AddUserLimitGoodsBuyItem(DBManager dbMgr, string UserID, int goodsID, int dayID, int usedNum, string stage)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RegressInsertStore, new object[]
				{
					UserID,
					goodsID,
					dayID,
					usedNum,
					stage
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static List<string> GetUserZoneID(DBManager dbMgr, int roleID)
		{
			MySQLConnection mySQLConnection = null;
			string item = "";
			int num = -1;
			List<string> list = new List<string>();
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT userid,zoneid FROM t_roles WHERE rid = {0}", roleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						item = mySQLDataReader[0].ToString();
						num = Convert.ToInt32(mySQLDataReader[1].ToString());
					}
					mySQLCommand.Dispose();
					mySQLCommand = null;
				}
				catch (MySQLException)
				{
					if (null != mySQLCommand)
					{
						mySQLCommand.Dispose();
						mySQLCommand = null;
					}
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
				list.Add(item);
				list.Add(num.ToString());
			}
			return list;
		}

		public static bool InsertRebornEquipHoleInfo(int RoleID, int HoleID, int Level, int Able)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RebornEquipHoleInsertInfo, new object[]
				{
					RoleID,
					HoleID,
					Level,
					Able
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateRebornEquipHoleInfo(int RoleID, int HoleID, int Level, int Able)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RebornEquipHoleUpdateInfo, new object[]
				{
					Level,
					Able,
					RoleID,
					HoleID
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool InsertMazingerStoreInfo(int RoleID, int TypeID, int Stage, int Level, int Exp)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.MazingerStoreInsertInfo, new object[]
				{
					RoleID,
					TypeID,
					Stage,
					Level,
					Exp
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static bool UpdateMazingerStoreInfo(int RoleID, int TypeID, int Stage, int Level, int Exp)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.MazingerStoreUpdateInfo, new object[]
				{
					RoleID,
					TypeID,
					Stage,
					Level,
					Exp
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DBWriter()
		{
			byte[] array = new byte[3];
			DBWriter._UpdateTask_fieldTypes = array;
			DBWriter._UpdatePet_fieldNames = new string[]
			{
				"petname",
				"pettype",
				"feednum",
				"realivenum",
				"props",
				"isdel",
				"addtime",
				"level"
			};
			DBWriter._UpdatePet_fieldTypes = new byte[]
			{
				1,
				0,
				0,
				0,
				1,
				0,
				3,
				0
			};
			DBWriter._UpdateActivity_fieldNames = new string[]
			{
				"loginweekid",
				"logindayid",
				"loginnum",
				"newstep",
				"steptime",
				"lastmtime",
				"curmid",
				"curmtime",
				"songliid",
				"logingiftstate",
				"onlinegiftstate",
				"lastlimittimehuodongid",
				"lastlimittimedayid",
				"limittimeloginnum",
				"limittimegiftstate",
				"everydayonlineawardstep",
				"geteverydayonlineawarddayid",
				"serieslogingetawardstep",
				"seriesloginawarddayid",
				"seriesloginawardgoodsid",
				"everydayonlineawardgoodsid"
			};
			DBWriter._UpdateActivity_fieldTypes = new byte[]
			{
				1,
				1,
				0,
				0,
				3,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				1
			};
			DBWriter._UpdateWing_fieldNames = new string[]
			{
				"equiped",
				"wingid",
				"forgeLevel",
				"failednum",
				"starexp",
				"zhulingnum",
				"zhuhunnum"
			};
			array = new byte[7];
			DBWriter._UpdateWing_fieldTypes = array;
			DBWriter._UpdateGoods2_fieldNames = new string[]
			{
				"isusing",
				"forge_level",
				"starttime",
				"endtime",
				"site",
				"quality",
				"Props",
				"gcount",
				"jewellist",
				"bagindex",
				"salemoney1",
				"saleyuanbao",
				"saleyinpiao",
				"binding",
				"addpropindex",
				"bornindex",
				"lucky",
				"strong",
				"excellenceinfo",
				"appendproplev",
				"equipchangelife"
			};
			DBWriter._UpdateGoods2_fieldTypes = new byte[]
			{
				0,
				0,
				1,
				1,
				0,
				0,
				1,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			DBWriter.GoodsBakTableMutex = new object();
			DBWriter.GoodsBakTableIndex = -1;
			DBWriter.GoodsBakTableNames = new string[]
			{
				"t_goods_bak",
				"t_goods_bak_1"
			};
		}

		private const string RoleExtIdKey = "role_ext_auto_increment";

		private const int RoleExtIdValidStart = 1500000000;

		private static readonly string[][] ValidateDatabaseTables = new string[][]
		{
			new string[]
			{
				"t_login",
				"userid"
			},
			new string[]
			{
				"t_usemoney_log",
				"Id"
			},
			new string[]
			{
				"t_goods_bak",
				"id"
			},
			new string[]
			{
				"t_goods_bak_1",
				"id"
			},
			new string[]
			{
				"t_goods",
				"washprops"
			},
			new string[]
			{
				"t_goods",
				"ehinfo"
			},
			new string[]
			{
				"t_goods_bak",
				"washprops"
			},
			new string[]
			{
				"t_goods_bak",
				"ehinfo"
			}
		};

		private static readonly string[][] ValidateDatabaseTableRows = new string[][]
		{
			new string[]
			{
				"t_lingdi",
				"where lingdi=7",
				"罗兰城战配置信息(lingdi=7)"
			}
		};

		private static readonly string[][] ValidateDatabaseTableIndexes = new string[][]
		{
			new string[]
			{
				"t_lingyu",
				"PRIMARY"
			}
		};

		private static readonly string[] _UpdateGoods_fieldNames = new string[]
		{
			"isusing",
			"forge_level",
			"starttime",
			"endtime",
			"site",
			"quality",
			"Props",
			"gcount",
			"jewellist",
			"bagindex",
			"salemoney1",
			"saleyuanbao",
			"saleyinpiao",
			"binding",
			"addpropindex",
			"bornindex",
			"lucky",
			"strong",
			"excellenceinfo",
			"appendproplev",
			"equipchangelife"
		};

		private static readonly byte[] _UpdateGoods_fieldTypes = new byte[]
		{
			0,
			0,
			1,
			1,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		private static readonly string[] _UpdateTask_fieldNames = new string[]
		{
			"focus",
			"value1",
			"value2"
		};

		private static readonly byte[] _UpdateTask_fieldTypes;

		private static readonly string[] _UpdatePet_fieldNames;

		private static readonly byte[] _UpdatePet_fieldTypes;

		private static readonly string[] _UpdateActivity_fieldNames;

		private static readonly byte[] _UpdateActivity_fieldTypes;

		private static readonly string[] _UpdateWing_fieldNames;

		private static readonly byte[] _UpdateWing_fieldTypes;

		private static readonly string[] _UpdateGoods2_fieldNames;

		private static readonly byte[] _UpdateGoods2_fieldTypes;

		private static object GoodsBakTableMutex;

		private static int GoodsBakTableIndex;

		private static readonly string[] GoodsBakTableNames;
	}
}
