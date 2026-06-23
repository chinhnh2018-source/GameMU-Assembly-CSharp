using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	public class LingDiCaiJiService
	{
		public static LingDiCaiJiService Instance()
		{
			return LingDiCaiJiService._instance;
		}

		public int KfServerId
		{
			get
			{
				return this._KfServerId;
			}
			set
			{
				this._KfServerId = value;
			}
		}

		public void InitConfig()
		{
			try
			{
				this.RoleNumMax = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("ManorMaxPlayer", -1);
				LingDiData lingDiData = new LingDiData
				{
					RoleId = 0,
					LingDiType = 0,
					JunTuanId = -1,
					JunTuanName = "",
					BeginTime = DateTime.MaxValue,
					EndTime = DateTime.MinValue,
					OpenCount = 0,
					ShouWeiList = new List<LingDiShouWei>(),
					RoleData = null
				};
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(string.Format("select * from t_lingditequan where lingzhu=1 and lingditype=0", new object[0]), false);
				if (mySqlDataReader.Read())
				{
					lingDiData.RoleId = Convert.ToInt32(mySqlDataReader["rid"].ToString());
					lingDiData.LingDiType = 0;
					lingDiData.JunTuanId = Convert.ToInt32(mySqlDataReader["juntuanid"].ToString());
					lingDiData.JunTuanName = mySqlDataReader["juntuanname"].ToString();
					lingDiData.BeginTime = DateTime.Parse(mySqlDataReader["begintime"].ToString());
					lingDiData.EndTime = DateTime.Parse(mySqlDataReader["endtime"].ToString());
					lingDiData.OpenCount = Convert.ToInt32(mySqlDataReader["opencount"].ToString());
					string[] array = mySqlDataReader["shouwei"].ToString().Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						if (array3.Length >= 2)
						{
							lingDiData.ShouWeiList.Add(new LingDiShouWei
							{
								State = Convert.ToInt32(array3[0]),
								FreeBuShuTime = DateTime.Parse(array3[1])
							});
						}
					}
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("roledata")))
					{
						lingDiData.RoleData = (byte[])mySqlDataReader["roledata"];
					}
				}
				else
				{
					lingDiData.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
				}
				LingDiData lingDiData2 = new LingDiData
				{
					RoleId = 0,
					LingDiType = 1,
					JunTuanId = -1,
					JunTuanName = "",
					BeginTime = DateTime.MaxValue,
					EndTime = DateTime.MinValue,
					OpenCount = 0,
					ShouWeiList = new List<LingDiShouWei>(),
					RoleData = null
				};
				MySqlDataReader mySqlDataReader2 = DbHelperMySQL.ExecuteReader(string.Format("select * from t_lingditequan where lingzhu=1 and lingditype=1", new object[0]), false);
				if (mySqlDataReader2.Read())
				{
					lingDiData2.RoleId = Convert.ToInt32(mySqlDataReader2["rid"].ToString());
					lingDiData2.LingDiType = 1;
					lingDiData2.JunTuanId = Convert.ToInt32(mySqlDataReader2["juntuanid"].ToString());
					lingDiData2.JunTuanName = mySqlDataReader2["juntuanname"].ToString();
					lingDiData2.BeginTime = DateTime.Parse(mySqlDataReader2["begintime"].ToString());
					lingDiData2.EndTime = DateTime.Parse(mySqlDataReader2["endtime"].ToString());
					lingDiData2.OpenCount = Convert.ToInt32(mySqlDataReader2["opencount"].ToString());
					string[] array = mySqlDataReader2["shouwei"].ToString().Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						if (array3.Length >= 2)
						{
							lingDiData2.ShouWeiList.Add(new LingDiShouWei
							{
								State = Convert.ToInt32(array3[0]),
								FreeBuShuTime = DateTime.Parse(array3[1])
							});
						}
					}
					if (!mySqlDataReader2.IsDBNull(mySqlDataReader2.GetOrdinal("roledata")))
					{
						lingDiData2.RoleData = (byte[])mySqlDataReader2["roledata"];
					}
				}
				else
				{
					lingDiData2.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData2.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData2.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					lingDiData2.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
				}
				lock (this.Mutex)
				{
					this.LingDiDataList = new List<LingDiData>();
					this.LingDiDataList.Add(lingDiData);
					this.LingDiDataList.Add(lingDiData2);
					this.Initialized = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJiServer 初始化地图信息出错 ex:{0}", ex.Message.ToString()), null, true);
			}
		}

		public bool isLingZhu(int junTuanId)
		{
			bool result;
			lock (this.Mutex)
			{
				if (this.LingDiDataList == null)
				{
					result = false;
				}
				else
				{
					foreach (LingDiData lingDiData in this.LingDiDataList)
					{
						if (lingDiData.JunTuanId == junTuanId && junTuanId > 0)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public int SetDoubleOpenTime(int roleId, int lingDiType, DateTime openTime, int openSeconds)
		{
			LingDiData lingDiData = null;
			lock (this.Mutex)
			{
				if (this.LingDiDataList[lingDiType] == null)
				{
					return -8;
				}
				this.LingDiDataList[lingDiType].BeginTime = openTime;
				this.LingDiDataList[lingDiType].EndTime = openTime.AddSeconds((double)openSeconds);
				this.LingDiDataList[lingDiType].OpenCount++;
				lingDiData = this.LingDiDataList[lingDiType];
			}
			int num = 0;
			try
			{
				string sqlstring = string.Format("update t_lingditequan set begintime='{0}',endtime='{1}',opencount={2},opttime='{5}' where rid={3} and lingditype={4}", new object[]
				{
					lingDiData.BeginTime,
					lingDiData.EndTime,
					lingDiData.OpenCount,
					roleId,
					lingDiType,
					TimeUtil.NowDateTime().ToString()
				});
				num = DbHelperMySQL.ExecuteSql(sqlstring);
				if (num > 0)
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(21, new AsyncDataItem(29, new object[]
					{
						lingDiData
					}), 0);
					return 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("初始化跨服地图{0}时出错!!!", "领地采集"), null, true);
				return -7;
			}
			return num;
		}

		public int SetShouWeiTime(int roleId, int lingDiType, DateTime openTime, int index)
		{
			int num = 0;
			try
			{
				LingDiData lingDiData = new LingDiData();
				string text = "";
				lock (this.Mutex)
				{
					if (this.LingDiDataList[lingDiType] == null)
					{
						return -8;
					}
					lingDiData = this.LingDiDataList[lingDiType];
					if (this.LingDiDataList[lingDiType].ShouWeiList.Count < index + 1)
					{
						return -8;
					}
					lingDiData.ShouWeiList[index].State = 2;
					lingDiData.ShouWeiList[index].FreeBuShuTime = DateTime.MaxValue;
					text = lingDiData.ShouWeiList[0].State + "," + lingDiData.ShouWeiList[0].FreeBuShuTime.ToString();
					for (int i = 1; i < lingDiData.ShouWeiList.Count; i++)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"|",
							lingDiData.ShouWeiList[i].State,
							",",
							lingDiData.ShouWeiList[i].FreeBuShuTime.ToString()
						});
					}
				}
				string sqlstring = string.Format("update t_lingditequan set shouwei='{0}' where rid={1} and lingditype={2}", text, roleId, lingDiType);
				num = DbHelperMySQL.ExecuteSql(sqlstring);
				if (num > 0)
				{
					return 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("初始化跨服地图{0}时出错!!!", "领地采集"), null, true);
				return -7;
			}
			return num;
		}

		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			lock (this.Mutex)
			{
				if (lingDiType == 0 && this.RoleNumDiGong >= this.RoleNumMax)
				{
					return -10;
				}
				if (lingDiType == 1 && this.RoleNumHuangMo >= this.RoleNumMax)
				{
					return -10;
				}
				if (this.KfServerId <= 0)
				{
					int num = 0;
					if (!ClientAgentManager.Instance().SpecialKfFuben(22, 0L, this.RoleNumMax, out num))
					{
						LogManager.WriteLog(2, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", num), null, true);
						return -1;
					}
					this.KfServerId = num;
				}
				if (this.KfServerId != 0)
				{
					if (lingDiType == 0)
					{
						this.RoleNumDiGong++;
					}
					else
					{
						this.RoleNumHuangMo++;
					}
				}
			}
			return this.KfServerId;
		}

		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			if (this.KfServerId <= 0)
			{
				int num = 0;
				if (!ClientAgentManager.Instance().SpecialKfFuben(22, 0L, this.RoleNumMax, out num))
				{
					LogManager.WriteLog(2, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", num), null, true);
					return -1;
				}
				this.KfServerId = num;
			}
			int result;
			if (this.KfServerId == 0)
			{
				result = -1;
			}
			else if (serverId != this.KfServerId)
			{
				result = this.KfServerId;
			}
			else
			{
				lock (this.Mutex)
				{
					if (lingDiType == 0)
					{
						this.RoleNumDiGong = roleNum;
					}
					else if (lingDiType == 1)
					{
						this.RoleNumHuangMo = roleNum;
					}
					result = this.KfServerId;
				}
			}
			return result;
		}

		public int GetLingDiRoleNum(int lingDiType)
		{
			if (this.KfServerId <= 0)
			{
				int num = 0;
				if (!ClientAgentManager.Instance().SpecialKfFuben(22, 0L, this.RoleNumMax, out num))
				{
					LogManager.WriteLog(2, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", num), null, true);
					return -1;
				}
				this.KfServerId = num;
			}
			int result;
			if (this.KfServerId == 0)
			{
				result = -1;
			}
			else
			{
				lock (this.Mutex)
				{
					if (lingDiType == 0)
					{
						result = this.RoleNumDiGong;
					}
					else if (lingDiType == 1)
					{
						result = this.RoleNumHuangMo;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}

		public List<LingDiData> GetLingDiData()
		{
			List<LingDiData> result = new List<LingDiData>();
			lock (this.Mutex)
			{
				if (this.LingDiDataList.Count < 2 || this.LingDiDataList[0] == null || null == this.LingDiDataList[1])
				{
					return result;
				}
				result = this.LingDiDataList;
			}
			return result;
		}

		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			try
			{
				int num = 0;
				LingDiData lingDiData = new LingDiData();
				lock (this.Mutex)
				{
					if (this.LingDiDataList.Count < 2 || this.LingDiDataList[0] == null || null == this.LingDiDataList[1])
					{
						return 0;
					}
					num = this.LingDiDataList[lingDiType].RoleId;
					this.LingDiDataList[lingDiType].RoleId = roleId;
					this.LingDiDataList[lingDiType].OpenCount = 0;
					this.LingDiDataList[lingDiType].BeginTime = DateTime.MaxValue;
					this.LingDiDataList[lingDiType].EndTime = DateTime.MinValue;
					foreach (LingDiShouWei lingDiShouWei in this.LingDiDataList[lingDiType].ShouWeiList)
					{
						lingDiShouWei.State = 0;
						lingDiShouWei.FreeBuShuTime = DateTime.MinValue;
					}
					this.LingDiDataList[lingDiType].JunTuanId = junTuanId;
					this.LingDiDataList[lingDiType].RoleData = roledata;
					this.LingDiDataList[lingDiType].JunTuanName = junTuanName;
					lingDiData = this.LingDiDataList[lingDiType];
				}
				string text = TimeUtil.NowDateTime().ToString();
				string text2 = string.Format("update t_lingditequan set lingzhu=0,opttime='{2}' where rid={0} and lingditype={1};", num, lingDiType, text);
				text2 += string.Format("replace into t_lingditequan(rid,lingditype,juntuanid,juntuanname,lingzhu,roledata,opttime) values ({0},{1},{2},'{3}',{4},@roledata,'{5}');", new object[]
				{
					roleId,
					lingDiType,
					junTuanId,
					junTuanName,
					zhiWu,
					text
				});
				int num2 = DbHelperMySQL.ExecuteSqlInsertImg(text2, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("roledata", roledata)
				});
				if (num2 > 0)
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(21, new AsyncDataItem(28, new object[]
					{
						lingDiData
					}), 0);
					return 1;
				}
				return num2;
			}
			catch
			{
			}
			return 0;
		}

		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			try
			{
				this.LingDiDataList[lingDiType].ShouWeiList = shouWeiList;
				string text = shouWeiList[0].State + "," + shouWeiList[0].FreeBuShuTime.ToString();
				for (int i = 1; i < shouWeiList.Count; i++)
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"|",
						shouWeiList[i].State,
						",",
						shouWeiList[i].FreeBuShuTime.ToString()
					});
				}
				string sqlstring = string.Format("update t_lingditequan set shouwei='{0}',opttime='{2}' where lingzhu=1 and lingditype={1};", text, lingDiType, TimeUtil.NowDateTime().ToString());
				return DbHelperMySQL.ExecuteSql(sqlstring);
			}
			catch
			{
			}
			return 0;
		}

		public bool GetClientCacheItems(int serverId)
		{
			lock (this.Mutex)
			{
				if (this.Initialized)
				{
					ClientAgent currentClientAgent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (currentClientAgent != null && currentClientAgent.ClientInfo.ClientId > 0)
					{
						int num;
						if (!this.BroadcastServerIdHashSet.TryGetValue(serverId, out num) || num != currentClientAgent.ClientInfo.ClientId)
						{
							this.BroadcastServerIdHashSet[serverId] = currentClientAgent.ClientInfo.ClientId;
							return true;
						}
					}
				}
			}
			return false;
		}

		private static LingDiCaiJiService _instance = new LingDiCaiJiService();

		private int _KfServerId = 0;

		private object Mutex = new object();

		private int RoleNumDiGong = 0;

		private int RoleNumHuangMo = 0;

		private int RoleNumMax = 0;

		public List<LingDiData> LingDiDataList = new List<LingDiData>();

		public Dictionary<int, int> BroadcastServerIdHashSet = new Dictionary<int, int>();

		public bool Initialized = false;
	}
}
