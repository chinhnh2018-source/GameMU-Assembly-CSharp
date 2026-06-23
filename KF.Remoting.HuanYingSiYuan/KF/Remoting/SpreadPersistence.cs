using System;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class SpreadPersistence
	{
		private SpreadPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				XElement xelement = ConfigHelper.Load("config.xml");
				Consts.TelMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadTelMaxCount", "value", 5L);
				Consts.TelTimeLimit = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadTelTimeLimit", "value", 20L) * 60;
				Consts.TelTimeStop = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadTelTimeStop", "value", 120L) * 60;
				Consts.VerifyRoleMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadVerifyRoleMaxCount", "value", 5L);
				Consts.VerifyRoleTimeLimit = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadVerifyRoleTimeLimit", "value", 5L) * 60;
				Consts.VerifyRoleTimeStop = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SpreadVerifyRoleTimeStop", "value", 30L) * 60;
				Consts.IsTest = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "IsTest", "value", 0L);
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		private int ExecuteSqlNoQuery2(string sqlCmd)
		{
			int result = 0;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public bool DBSpreadSign(int pzoneID, int proleID)
		{
			string sqlCmd = string.Format("REPLACE INTO t_spread(logTime, zoneID, roleID) VALUES('{0}','{1}','{2}')", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), pzoneID, proleID);
			int num = this.ExecuteSqlNoQuery2(sqlCmd);
			return num > 0;
		}

		public bool DBSpreadSignCheck(int pzoneID, int proleID)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread where zoneID = {0} and roleID={1}", pzoneID, proleID));
				if (null != single)
				{
					int num = Convert.ToInt32(single);
					if (num > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public bool DBSpreadVeruftCheck(int czoneID, int croleID, string cuserID)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread_role where (czoneID = {0} and croleID={1}) or (cuserID='{2}') limit 1", czoneID, croleID, cuserID));
				if (null != single)
				{
					int num = Convert.ToInt32(single);
					if (num > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public int DBSpreadCountAll(int pzoneID, int proleID)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != single)
				{
					return Convert.ToInt32(single);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		public int DBSpreadCountVip(int pzoneID, int proleID)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where isVip>0 and pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != single)
				{
					return Convert.ToInt32(single);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		public int DBSpreadCountLevel(int pzoneID, int proleID)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select count(*) c from t_spread_role where isLevel>0 and pzoneID={0} and proleID={1}", pzoneID, proleID));
				if (null != single)
				{
					return Convert.ToInt32(single);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		public bool DBSpreadIsVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			string sqlCmd = string.Format("UPDATE t_spread_role set isVip=1 where pzoneID={0} and proleID={1} and czoneID={2} and croleID={3};", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID
			});
			int num = this.ExecuteSqlNoQuery2(sqlCmd);
			return num > 0;
		}

		public bool DBSpreadIsLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			string sqlCmd = string.Format("UPDATE t_spread_role set isLevel=1 where  pzoneID={0} and proleID={1} and czoneID={2} and croleID={3};", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID
			});
			int num = this.ExecuteSqlNoQuery2(sqlCmd);
			return num > 0;
		}

		public bool DBSpreadRoleAdd(int pzoneID, int proleID, string cuserID, int czoneID, int croleID, string tel, int isVip, int isLevel)
		{
			string sqlCmd = string.Format("INSERT INTO t_spread_role(pzoneID,proleID,cuserID,czoneID,croleID,tel,isVip,isLevel,logTime) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", new object[]
			{
				pzoneID,
				proleID,
				cuserID,
				czoneID,
				croleID,
				tel,
				isVip,
				isLevel,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
			});
			int num = this.ExecuteSqlNoQuery2(sqlCmd);
			return num > 0;
		}

		public bool DBSpreadTelCodeAdd(int pzoneID, int proleID, int czoneID, int croleID, string tel, int telCode)
		{
			string sqlCmd = string.Format("INSERT INTO t_spread_tel(pzoneID, proleID, czoneID,croleID,tel,telCode,logTime) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", new object[]
			{
				pzoneID,
				proleID,
				czoneID,
				croleID,
				tel,
				telCode,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
			});
			int num = this.ExecuteSqlNoQuery2(sqlCmd);
			return num > 0;
		}

		public bool DBSpreadTelBind(string tel)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("select IFNULL(id,0) d from t_spread_role where tel='{0}'", tel));
				if (null != single)
				{
					int num = Convert.ToInt32(single);
					if (num > 0)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public static readonly SpreadPersistence Instance = new SpreadPersistence();

		public object Mutex = new object();

		public bool Initialized = false;
	}
}
