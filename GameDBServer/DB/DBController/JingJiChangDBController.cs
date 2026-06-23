using System;
using System.Collections.Generic;
using System.Data;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	public class JingJiChangDBController : DBController<PlayerJingJiData>
	{
		private JingJiChangDBController()
		{
		}

		public static JingJiChangDBController getInstance()
		{
			return JingJiChangDBController.instance;
		}

		public PlayerJingJiData getPlayerJingJiDataById(int Id)
		{
			string sql = string.Format("select * from t_jingjichang where roleId = {0};", Id);
			return base.queryForObject(sql);
		}

		public List<PlayerJingJiData> getPlayerJingJiDataList()
		{
			string sql = string.Format("select * from t_jingjichang where ranking != -1 order by ranking limit {0};", JingJiChangConstants.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		public bool updateNextRewardTime(int roleId, long nextRewardTime)
		{
			string sql = string.Format("update t_jingjichang set nextRewardTime={0} where roleId={1};", nextRewardTime, roleId);
			return base.update(sql) > 0;
		}

		public bool updateJingJiDataForFailed(int roleId, long nextChallengeTime)
		{
			string sql = string.Format("update t_jingjichang set winCount=0,nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);
			return base.update(sql) > 0;
		}

		public bool updateJingJiWinCount(int roleId, int winCount)
		{
			string sql = string.Format("update t_jingjichang set winCount={0} where roleId={1};", winCount, roleId);
			return base.update(sql) > 0;
		}

		public bool updateJingJiMaxWinCount(int roleId, int maxWinCount)
		{
			string sql = string.Format("update t_jingjichang set maxwincnt={0} where roleId={1};", maxWinCount, roleId);
			return base.update(sql) > 0;
		}

		public bool updateJingJiRanking(int roleId, int ranking)
		{
			string sql = string.Format("update t_jingjichang set ranking={0} where roleId={1};", ranking, roleId);
			return base.update(sql) > 0;
		}

		public bool updateNextChallengeTime(int roleId, long nextChallengeTime)
		{
			string sql = string.Format("update t_jingjichang set nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);
			return base.update(sql) > 0;
		}

		public bool updateJingJiDataForWin(PlayerJingJiData data)
		{
			data.convertString();
			string text = "update t_jingjichang set level=@level,occupationid=@occupationid,changeLiveCount=@changeLiveCount,winCount=@winCount,nextChallengeTime=@nextChallengeTime,baseProps=@baseProps,extProps=@extProps,equipDatas=@equipDatas,skillDatas=@skillDatas,CombatForce=@CombatForce,wingData=@wingData,settingFlags=@settingFlags,shenshiequip=@shenShiEquipData,passiveEffect=@passiveEffectData,suboccupation=@suboccupation where roleId=@roleId;";
			MySQLConnection mySQLConnection = null;
			int num = -1;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand();
				mySQLCommand.Connection = mySQLConnection;
				mySQLCommand.CommandType = CommandType.Text;
				mySQLCommand.CommandText = text;
				mySQLCommand.Parameters.Add("@roleId", DbType.Int32);
				mySQLCommand.Parameters.Add("@level", DbType.Int32);
				mySQLCommand.Parameters.Add("@changeLiveCount", DbType.Int32);
				mySQLCommand.Parameters.Add("@winCount", DbType.Int32);
				mySQLCommand.Parameters.Add("@nextChallengeTime", DbType.Int64);
				mySQLCommand.Parameters.Add("@baseProps", DbType.String);
				mySQLCommand.Parameters.Add("@extProps", DbType.String);
				mySQLCommand.Parameters.Add("@equipDatas", DbType.String);
				mySQLCommand.Parameters.Add("@skillDatas", DbType.String);
				mySQLCommand.Parameters.Add("@CombatForce", DbType.Int32);
				mySQLCommand.Parameters.Add("@wingData", DbType.String);
				mySQLCommand.Parameters.Add("@settingFlags", DbType.Int64);
				mySQLCommand.Parameters.Add("@occupationid", DbType.Int32);
				mySQLCommand.Parameters.Add("@shenShiEquipData", DbType.String);
				mySQLCommand.Parameters.Add("@passiveEffectData", DbType.String);
				mySQLCommand.Parameters.Add("@suboccupation", DbType.Int32);
				mySQLCommand.Parameters["@roleId"].Value = data.roleId;
				mySQLCommand.Parameters["@level"].Value = data.level;
				mySQLCommand.Parameters["@changeLiveCount"].Value = data.changeLiveCount;
				mySQLCommand.Parameters["@winCount"].Value = data.winCount;
				mySQLCommand.Parameters["@nextChallengeTime"].Value = data.nextChallengeTime;
				mySQLCommand.Parameters["@baseProps"].Value = data.stringBaseProps;
				mySQLCommand.Parameters["@extProps"].Value = data.stringExtProps;
				mySQLCommand.Parameters["@equipDatas"].Value = data.stringEquipDatas;
				mySQLCommand.Parameters["@skillDatas"].Value = data.stringSkillDatas;
				mySQLCommand.Parameters["@CombatForce"].Value = data.combatForce;
				mySQLCommand.Parameters["@wingData"].Value = data.stringWingData;
				mySQLCommand.Parameters["@settingFlags"].Value = data.settingFlags;
				mySQLCommand.Parameters["@occupationid"].Value = data.occupationId;
				mySQLCommand.Parameters["@shenShiEquipData"].Value = data.stringShenShiEuipSkill;
				mySQLCommand.Parameters["@passiveEffectData"].Value = data.stringPassiveEffect;
				mySQLCommand.Parameters["@suboccupation"].Value = data.SubOccupation;
				try
				{
					num = mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库更新竞技场数据失败: {0},{1}", text, arg), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return num > 0;
		}

		public bool insertJingJiData(PlayerJingJiData data)
		{
			data.convertString();
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("insert into t_jingjichang \r\n                    (roleId,roleName,name,zoneId,level,changeLiveCount,occupationId,winCount,ranking,nextRewardTime,nextChallengeTime,\r\n                    baseProps,extProps,equipDatas,skillDatas,CombatForce,sex,wingData,settingFlags, shenshiequip, passiveEffect,suboccupation) \r\n                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', {18}, '{19}', '{20}','{21}')", new object[]
				{
					data.roleId,
					data.roleName,
					data.name,
					data.zoneId,
					data.level,
					data.changeLiveCount,
					data.occupationId,
					data.winCount,
					data.ranking,
					data.nextRewardTime,
					data.nextChallengeTime,
					data.stringBaseProps,
					data.stringExtProps,
					data.stringEquipDatas,
					data.stringSkillDatas,
					data.combatForce,
					data.sex,
					data.stringWingData,
					data.settingFlags,
					data.stringShenShiEuipSkill,
					data.stringPassiveEffect,
					data.SubOccupation
				});
				num = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return num >= 0;
		}

		internal void OnChangeName(int roleId, string oldName, string newName)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_jingjichang SET roleName='{0}',name='{1}' WHERE roleId={2}", newName, newName, roleId);
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		private static JingJiChangDBController instance = new JingJiChangDBController();
	}
}
