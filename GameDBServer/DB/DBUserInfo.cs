using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	public class DBUserInfo
	{
		public string UserID { get; set; }

		public List<int> ListRoleIDs
		{
			get
			{
				return this._ListRoleIDs;
			}
		}

		public List<int> ListRoleSexes
		{
			get
			{
				return this._ListRoleSexes;
			}
		}

		public List<int> ListRoleOccups
		{
			get
			{
				return this._ListRoleOccups;
			}
		}

		public List<string> ListRoleNames
		{
			get
			{
				return this._ListRoleNames;
			}
		}

		public List<int> ListRoleLevels
		{
			get
			{
				return this._ListRoleLevels;
			}
		}

		public List<int> ListRoleZoneIDs
		{
			get
			{
				return this._ListRoleZoneIDs;
			}
		}

		public List<int> ListRoleChangeLifeCount
		{
			get
			{
				return this._ListRoleChangeLifeCount;
			}
		}

		public List<string> ListRolePreRemoveTime
		{
			get
			{
				return this._ListRolePreRemoveTime;
			}
		}

		public string SecPwd { get; set; }

		public int Money { get; set; }

		public int RealMoney { get; set; }

		public int GiftID { get; set; }

		public int GiftJiFen { get; set; }

		public int InputPoints { get; set; }

		public int SpecJiFen { get; set; }

		public int EveryJiFen { get; set; }

		public string PushMessageID { get; set; }

		public long LastReferenceTicks
		{
			get
			{
				return this._LastReferenceTicks;
			}
			set
			{
				this._LastReferenceTicks = value;
			}
		}

		public bool Query(MySQLConnection conn, string userID)
		{
			LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载用户数据: {0}", userID), null, true);
			this.UserID = userID;
			string[] array = new string[]
			{
				"rid",
				"userid",
				"rname",
				"sex",
				"occupation",
				"level",
				"zoneid",
				"changelifecount",
				"predeltime"
			};
			string[] array2 = new string[]
			{
				"t_roles"
			};
			object[,] array3 = new object[2, 3];
			array3[0, 0] = "userid";
			array3[0, 1] = "=";
			array3[0, 2] = userID;
			array3[1, 0] = "isdel";
			array3[1, 1] = "=";
			array3[1, 2] = 0;
			MySQLSelectCommand mySQLSelectCommand = new MySQLSelectCommand(conn, array, array2, array3, null, null);
			if (mySQLSelectCommand.Table.Rows.Count > 0)
			{
				for (int i = 0; i < mySQLSelectCommand.Table.Rows.Count; i++)
				{
					this.ListRoleIDs.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["rid"].ToString()));
					this.ListRoleNames.Add(mySQLSelectCommand.Table.Rows[i]["rname"].ToString());
					this.ListRoleSexes.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["sex"].ToString()));
					this.ListRoleOccups.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["occupation"].ToString()));
					this.ListRoleLevels.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["level"].ToString()));
					this.ListRoleZoneIDs.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["zoneid"].ToString()));
					this.ListRoleChangeLifeCount.Add(Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["changelifecount"].ToString()));
					this.ListRolePreRemoveTime.Add(mySQLSelectCommand.Table.Rows[i]["predeltime"].ToString());
				}
			}
			this.Money = 0;
			string[] array4 = new string[]
			{
				"money",
				"realmoney",
				"giftid",
				"giftjifen",
				"points",
				"specjifen",
				"everyjifen",
				"cc"
			};
			string[] array5 = new string[]
			{
				"t_money"
			};
			array3 = new object[1, 3];
			array3[0, 0] = "userid";
			array3[0, 1] = "=";
			array3[0, 2] = userID;
			mySQLSelectCommand = new MySQLSelectCommand(conn, array4, array5, array3, null, null);
			if (mySQLSelectCommand.Table.Rows.Count > 0)
			{
				this.Money = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["money"].ToString());
				this.RealMoney = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["realmoney"].ToString());
				this.GiftID = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["giftid"].ToString());
				this.GiftJiFen = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["giftjifen"].ToString());
				this.InputPoints = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["points"].ToString());
				this.SpecJiFen = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["specjifen"].ToString());
				this.EveryJiFen = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["everyjifen"].ToString());
				string text = mySQLSelectCommand.Table.Rows[0]["cc"].ToString();
				if (!Global.CCC(text, 3, new object[]
				{
					userID,
					this.Money,
					this.RealMoney
				}))
				{
					LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_money#userid={0},money={1},cc={2}", userID, this.Money, text), null, true);
					return false;
				}
			}
			string[] array6 = new string[]
			{
				"userid",
				"pushid",
				"lastlogintime"
			};
			string[] array7 = new string[]
			{
				"t_pushmessageinfo"
			};
			array3 = new object[1, 3];
			array3[0, 0] = "userid";
			array3[0, 1] = "=";
			array3[0, 2] = userID;
			mySQLSelectCommand = new MySQLSelectCommand(conn, array6, array7, array3, null, null);
			if (mySQLSelectCommand.Table.Rows.Count > 0)
			{
				this.PushMessageID = mySQLSelectCommand.Table.Rows[0]["pushid"].ToString();
			}
			string[] array8 = new string[]
			{
				"userid",
				"secpwd"
			};
			string[] array9 = new string[]
			{
				"t_secondpassword"
			};
			array3 = new object[1, 3];
			array3[0, 0] = "userid";
			array3[0, 1] = "=";
			array3[0, 2] = userID;
			mySQLSelectCommand = new MySQLSelectCommand(conn, array8, array9, array3, null, null);
			if (mySQLSelectCommand.Table.Rows.Count > 0)
			{
				this.SecPwd = mySQLSelectCommand.Table.Rows[0]["secpwd"].ToString();
			}
			else
			{
				this.SecPwd = "";
			}
			return true;
		}

		public UserMiniData GetUserMiniData(string userId, int roleId, int OnlyZoneId)
		{
			UserMiniData userMiniData = new UserMiniData();
			userMiniData.UserId = this.UserID;
			userMiniData.RealMoney = this.RealMoney;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				MySQLConnection dbConn = myDbConnection.DbConn;
				string[] array = new string[]
				{
					"rid",
					"rname",
					"sex",
					"occupation",
					"level",
					"zoneid",
					"changelifecount",
					"regtime",
					"lasttime",
					"logofftime"
				};
				string[] array2 = new string[]
				{
					"t_roles"
				};
				object[,] array3 = new object[3, 3];
				array3[0, 0] = "userid";
				array3[0, 1] = "=";
				array3[0, 2] = userId;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				array3[2, 0] = "zoneid";
				array3[2, 1] = "=";
				array3[2, 2] = OnlyZoneId;
				MySQLSelectCommand mySQLSelectCommand = new MySQLSelectCommand(dbConn, array, array2, array3, null, null);
				if (mySQLSelectCommand.Table.Rows.Count > 0)
				{
					for (int i = 0; i < mySQLSelectCommand.Table.Rows.Count; i++)
					{
						int num = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["rid"].ToString());
						string text = mySQLSelectCommand.Table.Rows[i]["rname"].ToString();
						int num2 = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["sex"].ToString());
						int num3 = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["occupation"].ToString());
						int num4 = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["level"].ToString());
						int num5 = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["zoneid"].ToString());
						int num6 = Convert.ToInt32(mySQLSelectCommand.Table.Rows[i]["changelifecount"].ToString());
						DateTime dateTime;
						DateTime.TryParse(mySQLSelectCommand.Table.Rows[i]["regtime"].ToString(), out dateTime);
						DateTime dateTime2;
						DateTime.TryParse(mySQLSelectCommand.Table.Rows[i]["lasttime"].ToString(), out dateTime2);
						DateTime dateTime3;
						DateTime.TryParse(mySQLSelectCommand.Table.Rows[i]["logofftime"].ToString(), out dateTime3);
						if (num == roleId)
						{
							userMiniData.RoleCreateTime = dateTime;
							userMiniData.RoleLastLoginTime = dateTime2;
							userMiniData.RoleLastLogoutTime = dateTime3;
						}
						if (userMiniData.MinCreateRoleTime > dateTime)
						{
							userMiniData.MinCreateRoleTime = dateTime;
						}
						if (userMiniData.LastLoginTime < dateTime2)
						{
							userMiniData.LastLoginTime = dateTime2;
							userMiniData.LastRoleId = num;
						}
						if (userMiniData.LastLogoutTime < dateTime3)
						{
							userMiniData.LastLogoutTime = dateTime3;
						}
						if ((userMiniData.MaxChangeLifeCount << 16) + userMiniData.MaxLevel < (num6 << 16) + num4)
						{
							userMiniData.MaxChangeLifeCount = num6;
							userMiniData.MaxLevel = num4;
						}
					}
				}
			}
			return userMiniData;
		}

		private List<int> _ListRoleIDs = new List<int>();

		private List<int> _ListRoleSexes = new List<int>();

		private List<int> _ListRoleOccups = new List<int>();

		private List<string> _ListRoleNames = new List<string>();

		private List<int> _ListRoleLevels = new List<int>();

		private List<int> _ListRoleZoneIDs = new List<int>();

		private List<int> _ListRoleChangeLifeCount = new List<int>();

		private List<string> _ListRolePreRemoveTime = new List<string>();

		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		public long LogoutServerTicks = 0L;
	}
}
