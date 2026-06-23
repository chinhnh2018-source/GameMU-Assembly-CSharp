using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class YongZheZhanChangPersistence
	{
		private YongZheZhanChangPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				XElement xelement = ConfigHelper.Load("config.xml");
				this.YongZheZhanChangRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "MinEnterCount", "value", 100L);
				this.KuaFuBossRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "KuaFuBossRoleCount", "value", 50L);
				this.KingOfBattleRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "KingOfBattleRoleCount", "value", 40L);
				string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement, "add", "key", "LangHunLingYuResetCityTime", "value", "");
				if (string.IsNullOrEmpty(elementAttributeValue) || !TimeSpan.TryParse(elementAttributeValue, out this.LangHunLingYuResetCityTime))
				{
					this.LangHunLingYuResetCityTime = TimeSpan.MaxValue;
				}
				int num = Math.Max(this.YongZheZhanChangRoleCount, this.KuaFuBossRoleCount);
				num = Math.Max(num, this.KingOfBattleRoleCount);
				this.MaxServerCapcity = Math.Max(3000 / num, 30);
				this.InitLangHunLingYuConfig();
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(id),0) FROM t_yongzhezhanchang_game_fuben;"));
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		private void InitLangHunLingYuConfig()
		{
			string text = "";
			lock (this.Mutex)
			{
				try
				{
					Dictionary<int, CityLevelInfo> dictionary = new Dictionary<int, CityLevelInfo>();
					text = "Config/MU_City.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(text, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "TypeID", "");
						if (string.Compare(elementAttributeValue, KuaFuServerManager.platformType.ToString(), true) == 0)
						{
							foreach (XElement xelement3 in xelement2.Elements())
							{
								CityLevelInfo cityLevelInfo = new CityLevelInfo();
								cityLevelInfo.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "ID", 0L);
								cityLevelInfo.CityLevel = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "CityLevel", 0L);
								cityLevelInfo.CityNum = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "CityNum", 0L);
								cityLevelInfo.MaxNum = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "MaxNum", 0L);
								string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement3, "AttackWeekDay", "");
								cityLevelInfo.AttackWeekDay = ConfigHelper.String2IntArray(elementAttributeValue2, ',');
								if (!ConfigHelper.ParserTimeRangeListWithDay(cityLevelInfo.BaoMingTime, ConfigHelper.GetElementAttributeValue(xelement3, "BaoMingTime", "").Replace(';', '|'), true, '|', '-', ','))
								{
									LogManager.WriteLog(1000, string.Format("解析文件{0}的BaoMingTime出错", text), null, true);
									KuaFuServerManager.LoadConfigSuccess = false;
								}
								if (!ConfigHelper.ParserTimeRangeList(cityLevelInfo.AttackTime, ConfigHelper.GetElementAttributeValue(xelement3, "AttackTime", ""), true, '|', '-'))
								{
									LogManager.WriteLog(1000, string.Format("解析文件{0}的BaoMingTime出错", text), null, true);
									KuaFuServerManager.LoadConfigSuccess = false;
								}
								dictionary[cityLevelInfo.CityLevel] = cityLevelInfo;
							}
							break;
						}
					}
					this.CityLevelInfoDict = dictionary;
					if (this.CityLevelInfoDict.Count == 0)
					{
						LogManager.WriteLog(1000, string.Format("读取配置{0}失败,读取到的城池配置数为0", new object[0]), null, true);
						KuaFuServerManager.LoadConfigSuccess = false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()), null, true);
					KuaFuServerManager.LoadConfigSuccess = false;
				}
			}
		}

		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(1, "YongZheZhanChang 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		public void UpdateRoleInfoData(object data)
		{
			if (this.YongZheZhanChangRoleInfoDataQueue.Count > 100000)
			{
				object obj;
				this.YongZheZhanChangRoleInfoDataQueue.TryDequeue(out obj);
			}
			this.YongZheZhanChangRoleInfoDataQueue.Enqueue(data);
		}

		public void WriteRoleInfoDataToDb(object obj)
		{
			try
			{
				string text = "";
				if (obj is YongZheZhanChangStatisticalData)
				{
					YongZheZhanChangStatisticalData yongZheZhanChangStatisticalData = obj as YongZheZhanChangStatisticalData;
					text = string.Format("INSERT INTO t_yongzhezhanchang_fuben_log(gameid,allrolecount,winrolecount,loserolecount,lianshascore,zhongjiescore,caijiscore,bossscore,killscore,gametime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},now())", new object[]
					{
						yongZheZhanChangStatisticalData.GameId,
						yongZheZhanChangStatisticalData.AllRoleCount,
						yongZheZhanChangStatisticalData.WinRoleCount,
						yongZheZhanChangStatisticalData.LoseRoleCount,
						yongZheZhanChangStatisticalData.LianShaScore,
						yongZheZhanChangStatisticalData.ZhongJieScore,
						yongZheZhanChangStatisticalData.CaiJiScore,
						yongZheZhanChangStatisticalData.BossScore,
						yongZheZhanChangStatisticalData.KillScore
					});
				}
				else if (obj is KingOfBattleStatisticalData)
				{
					KingOfBattleStatisticalData kingOfBattleStatisticalData = obj as KingOfBattleStatisticalData;
					text = string.Format("INSERT INTO t_kingofbattle_fuben_log(gameid,allrolecount,winrolecount,loserolecount,lianshascore,zhongjiescore,caijiscore,bossscore,killscore,gametime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},now())", new object[]
					{
						kingOfBattleStatisticalData.GameId,
						kingOfBattleStatisticalData.AllRoleCount,
						kingOfBattleStatisticalData.WinRoleCount,
						kingOfBattleStatisticalData.LoseRoleCount,
						kingOfBattleStatisticalData.LianShaScore,
						kingOfBattleStatisticalData.ZhongJieScore,
						kingOfBattleStatisticalData.CaiJiScore,
						kingOfBattleStatisticalData.BossScore,
						kingOfBattleStatisticalData.KillScore
					});
				}
				else if (obj is KuaFuBossStatisticalData)
				{
					KuaFuBossStatisticalData kuaFuBossStatisticalData = obj as KuaFuBossStatisticalData;
					for (int i = 0; i < kuaFuBossStatisticalData.MonsterDieTimeList.Count - 1; i += 2)
					{
						text += string.Format("INSERT INTO t_kuafu_boss_log VALUES({0},{1},{2});", kuaFuBossStatisticalData.GameId, kuaFuBossStatisticalData.MonsterDieTimeList[i], kuaFuBossStatisticalData.MonsterDieTimeList[i + 1]);
					}
				}
				else if (obj is GameLogItem)
				{
					GameLogItem gameLogItem = obj as GameLogItem;
					text = string.Format("INSERT INTO t_yongzhezhanchang_role_statistics_log(servercount,fubencount,signupcount,entercount,gametime) VALUES({0},{1},{2},{3},now());", new object[]
					{
						gameLogItem.ServerCount,
						gameLogItem.FubenCount,
						gameLogItem.SignUpCount,
						gameLogItem.EnterCount
					});
				}
				if (!string.IsNullOrEmpty(text))
				{
					DbHelperMySQL.ExecuteSql(text);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void WriteRoleInfoDataProc()
		{
			for (int i = 0; i < 1000; i++)
			{
				object obj;
				if (!this.YongZheZhanChangRoleInfoDataQueue.TryDequeue(out obj))
				{
					break;
				}
				this.WriteRoleInfoDataToDb(obj);
			}
		}

		private bool ExecuteSqlNoQuery(string sqlCmd)
		{
			bool result;
			try
			{
				DbHelperMySQL.ExecuteSql(sqlCmd);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			return result;
		}

		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		public void LogCreateYongZheFuBen(int kfSrvId, int gameId, int fubenSeq, int roleNum)
		{
			string sqlCmd = string.Format("INSERT INTO t_yongzhezhanchang_game_fuben(`id`,`serverid`,`fubensid`,`createtime`,`rolenum`) VALUES({0},{1},{2},'{3}',{4});", new object[]
			{
				gameId,
				kfSrvId,
				fubenSeq,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
				roleNum
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void SaveCityData(LangHunLingYuCityDataEx cityDataEx)
		{
			this.ExecuteSqlNoQuery(string.Format("INSERT INTO t_lhly_city(cityid,citylevel,owner,attacker1,attacker2,attacker3) VALUES({0},{1},{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE citylevel={1},owner={2},attacker1={3},attacker2={4},attacker3={5};", new object[]
			{
				cityDataEx.CityId,
				cityDataEx.CityLevel,
				cityDataEx.Site[0],
				cityDataEx.Site[1],
				cityDataEx.Site[2],
				cityDataEx.Site[3]
			}));
		}

		public void SaveBangHuiData(LangHunLingYuBangHuiDataEx bangHuiDataEx)
		{
			this.ExecuteSqlNoQuery(string.Format("INSERT INTO t_lhly_banghui(bhid,bhname,zoneid,level) VALUES({0},'{1}',{2},{3}) ON DUPLICATE KEY UPDATE bhname='{1}',zoneid={2},level={3};", new object[]
			{
				bangHuiDataEx.Bhid,
				bangHuiDataEx.BhName,
				bangHuiDataEx.ZoneId,
				bangHuiDataEx.Level
			}));
		}

		public bool LoadCityOwnerHistory(List<LangHunLingYuKingHist> LHLYCityOwnerList)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT * FROM `t_lhly_hist`", new object[0]);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					LangHunLingYuKingHist langHunLingYuKingHist = new LangHunLingYuKingHist();
					langHunLingYuKingHist.rid = Convert.ToInt32(mySqlDataReader["role_id"]);
					langHunLingYuKingHist.AdmireCount = Convert.ToInt32(mySqlDataReader["admire_count"]);
					langHunLingYuKingHist.CompleteTime = DateTime.Parse(mySqlDataReader["time"].ToString());
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data")))
					{
						langHunLingYuKingHist.CityOwnerRoleData = (byte[])mySqlDataReader["data"];
					}
					LHLYCityOwnerList.Add(langHunLingYuKingHist);
					num++;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return false;
		}

		public void InsertCityOwnerHistory(LangHunLingYuKingHist CityOwnerData)
		{
			try
			{
				string strSQL = string.Format("INSERT INTO t_lhly_hist(role_id, admire_count, time, data) VALUES({0}, {1}, '{2}', @content)", CityOwnerData.rid, CityOwnerData.AdmireCount, CityOwnerData.CompleteTime.ToString());
				DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", CityOwnerData.CityOwnerRoleData)
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void AdmireCityOwner(int rid)
		{
			try
			{
				string sqlstring = string.Format("UPDATE t_lhly_hist SET admire_count=admire_count+1 WHERE role_id={0}", rid);
				DbHelperMySQL.ExecuteSql(sqlstring);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public bool LoadBangHuiDataExList(List<LangHunLingYuBangHuiDataEx> list)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT * FROM `t_lhly_banghui`;", new object[0]);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new LangHunLingYuBangHuiDataEx
					{
						Bhid = Convert.ToInt32(mySqlDataReader["bhid"]),
						ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]),
						BhName = mySqlDataReader["bhname"].ToString()
					});
					num++;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return false;
		}

		public bool LoadCityDataExList(List<LangHunLingYuCityDataEx> list)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT * FROM `t_lhly_city`;", new object[0]);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					LangHunLingYuCityDataEx langHunLingYuCityDataEx = new LangHunLingYuCityDataEx();
					langHunLingYuCityDataEx.CityId = Convert.ToInt32(mySqlDataReader["cityid"]);
					langHunLingYuCityDataEx.CityLevel = Convert.ToInt32(mySqlDataReader["citylevel"]);
					langHunLingYuCityDataEx.Site[0] = (long)Convert.ToInt32(mySqlDataReader["owner"].ToString());
					langHunLingYuCityDataEx.Site[1] = (long)Convert.ToInt32(mySqlDataReader["attacker1"].ToString());
					langHunLingYuCityDataEx.Site[2] = (long)Convert.ToInt32(mySqlDataReader["attacker2"].ToString());
					langHunLingYuCityDataEx.Site[3] = (long)Convert.ToInt32(mySqlDataReader["attacker3"].ToString());
					list.Add(langHunLingYuCityDataEx);
					num++;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return false;
		}

		public int GetKuaFoWorldMaxTempRoleID(out int limit)
		{
			int num = 50;
			int num2 = (int)DbHelperMySQL.GetSingleLong("select value from t_async where id = " + num);
			if (num2 < 0)
			{
				num2 = 0;
				this.ExecuteSqlNoQuery(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", num, num2));
			}
			limit = num2 + 199980;
			string sql = string.Format("select max(temprid) from `t_pt_roles` where temprid>={0} and temprid<={1}", num2, num2 + 199999);
			int num3 = (int)DbHelperMySQL.GetSingleLong(sql);
			if (num3 <= 0)
			{
				num3 = 0;
			}
			return num3;
		}

		public KuaFuWorldRoleData QueryKuaFuWorldRoleData(int roleID, int ptid)
		{
			KuaFuWorldRoleData kuaFuWorldRoleData = null;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT ptid,rid,temprid,userid,zoneid,channel,roledata FROM `t_pt_roles` where ptid={0} and rid={1}", ptid, roleID);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				if (mySqlDataReader.Read())
				{
					kuaFuWorldRoleData = new KuaFuWorldRoleData();
					kuaFuWorldRoleData.PTID = Convert.ToInt32(mySqlDataReader["ptid"].ToString());
					kuaFuWorldRoleData.LocalRoleID = Convert.ToInt32(mySqlDataReader["rid"].ToString());
					kuaFuWorldRoleData.TempRoleID = Convert.ToInt32(mySqlDataReader["temprid"].ToString());
					kuaFuWorldRoleData.UserID = mySqlDataReader["userid"].ToString();
					kuaFuWorldRoleData.ZoneID = Convert.ToInt32(mySqlDataReader["zoneid"].ToString());
					kuaFuWorldRoleData.Channel = mySqlDataReader["channel"].ToString();
					kuaFuWorldRoleData.RoleData = (mySqlDataReader["roledata"] as byte[]);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return kuaFuWorldRoleData;
		}

		public KuaFuWorldRoleData InsertKuaFuWorldRoleData(KuaFuWorldRoleData data, int tempRoleID)
		{
			try
			{
				object[] array;
				int num = DbHelperMySQL.GetSingleValues(string.Format("SELECT ptid,rid FROM `t_pt_roles` where temprid={0}", data.LocalRoleID), out array);
				bool flag;
				if (num >= 0)
				{
					flag = !array.All((object x) => x != null);
				}
				else
				{
					flag = true;
				}
				if (!flag)
				{
					if (!(data.PTID.ToString() != array[0].ToString()))
					{
						return this.QueryKuaFuWorldRoleData(data.LocalRoleID, data.PTID);
					}
					data.TempRoleID = tempRoleID;
				}
				else
				{
					data.TempRoleID = data.LocalRoleID;
				}
				List<Tuple<string, byte[]>> list = new List<Tuple<string, byte[]>>();
				list.Add(new Tuple<string, byte[]>("content", data.RoleData));
				string strSQL = string.Format("insert into `t_pt_roles`(ptid,rid,temprid,userid,zoneid,channel,roledata) values('{0}','{1}','{2}','{3}','{4}','{5}',@content)", new object[]
				{
					data.PTID,
					data.LocalRoleID,
					data.TempRoleID,
					data.UserID,
					data.ZoneID,
					data.Channel
				});
				num = DbHelperMySQL.ExecuteSqlInsertImg(strSQL, list);
				if (num >= 0)
				{
					return data;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public int WriteKuaFuWorldRoleData(KuaFuWorldRoleData data)
		{
			int result = -15;
			try
			{
				string strSQL = string.Format("update `t_pt_roles` set roledata=@content where ptid={0} and rid={1}", data.PTID, data.PTID);
				result = DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", data.RoleData)
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static readonly YongZheZhanChangPersistence Instance = new YongZheZhanChangPersistence();

		public object Mutex = new object();

		private int CurrGameId = Global.UninitGameId;

		public bool Initialized = false;

		private int MaxPaiMingRank = 100;

		public int KuaFuBossRoleCount = 50;

		public int YongZheZhanChangRoleCount = 100;

		public int KingOfBattleRoleCount = 40;

		public int MinEnterCount = 100;

		public int MaxServerCapcity = 30;

		public int ServerCapacityRate = 1;

		public int LangHunLingYuServerCapacityRate = 5;

		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();

		public ConcurrentQueue<object> YongZheZhanChangRoleInfoDataQueue = new ConcurrentQueue<object>();

		public bool LangHunLingYuInitialized = false;

		public TimeSpan LangHunLingYuResetCityTime;

		public DateTime LastLangHunLingYuResetCityTime;

		public int OtherListUpdateOffsetDay = 0;

		public Dictionary<int, CityLevelInfo> CityLevelInfoDict = new Dictionary<int, CityLevelInfo>();

		public DateTime LastLangHunLingYuBroadcastTime;

		public Dictionary<int, int> LangHunLingYuBroadcastServerIdHashSet = new Dictionary<int, int>();
	}
}
