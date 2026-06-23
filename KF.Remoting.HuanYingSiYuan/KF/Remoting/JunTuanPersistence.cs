using System;
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
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class JunTuanPersistence
	{
		private JunTuanPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				XElement xelement = ConfigHelper.Load("config.xml");
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(juntuanid),0) FROM t_juntuan;"));
				}
				string text = "";
				lock (this.RuntimeData.Mutex)
				{
					try
					{
						this.RuntimeData.LegionsNeed = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsNeed", -1);
						this.RuntimeData.LegionsCreateCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsCreateCD", -1);
						this.RuntimeData.LegionsCastZuanShi = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsCastZuanShi", -1);
						this.RuntimeData.LegionsJionCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsJionCD", -1);
						this.RuntimeData.LegionsEliteNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsEliteNum", -1);
						this.RuntimeData.LegionProsperityCost = KuaFuServerManager.systemParamsList.GetParamValueIntArrayByName("LegionProsperityCost");
						string paramValueByName = KuaFuServerManager.systemParamsList.GetParamValueByName("LegionTasksTime");
						if (!ConfigHelper.ParserTimeRangeListWithDay2(this.RuntimeData.TaskStartEndTimeList, paramValueByName, true, '|', '-', ',') || this.RuntimeData.TaskStartEndTimeList.Count != 2)
						{
							LogManager.WriteLog(1000, string.Format("解析systemparams.xml的LegionTasksTime出错", text), null, true);
						}
						text = "Config/LegionsManager.xml";
						string resourcePath = KuaFuServerManager.GetResourcePath(text, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.RolePermissionDict.Load(resourcePath, null);
						text = "Config/LegionTasks.xml";
						resourcePath = KuaFuServerManager.GetResourcePath(text, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.TaskList.Load(resourcePath, null);
						text = "Config/LegionsWar.xml";
						resourcePath = KuaFuServerManager.GetResourcePath(text, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.KarenBattleMapList.Load(resourcePath, null);
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
					}
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(1, "JunTuan 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		public bool LoadDatabase()
		{
			YaoSaiService.Instance().LoadYaoSaiData();
			JunTuanEraService.Instance().LoadJunTuanEraData();
			int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
			List<JunTuanData> list = new List<JunTuanData>();
			if (!this.LoadJunTuanDataList(list))
			{
				LogManager.WriteLog(1000, "加载军团数据失败", null, true);
			}
			foreach (JunTuanData junTuanData in list)
			{
				List<int> list2 = new List<int>();
				List<JunTuanBangHuiData> list3 = new List<JunTuanBangHuiData>();
				this.LoadJunTuanBangHuiList(list3, list2, junTuanData.JunTuanId);
				if (list2.Count == 0)
				{
					this.DeleteJunTuan(junTuanData.JunTuanId);
				}
				else
				{
					List<JunTuanTaskData> list4 = new List<JunTuanTaskData>();
					this.LoadJunTuanTaskList(list4, weekStartDayIdNow, junTuanData.JunTuanId);
					List<JunTuanEventLog> list5 = new List<JunTuanEventLog>();
					this.LoadJunTuanLogList(list5, junTuanData.JunTuanId);
					List<JunTuanRequestData> list6 = new List<JunTuanRequestData>();
					this.LoadJunTuanRequestList(list6, junTuanData.JunTuanId);
					int junTuanId = junTuanData.JunTuanId;
					JunTuanDetailData junTuanDetailData;
					if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
					{
						junTuanDetailData = new JunTuanDetailData
						{
							JunTuanId = junTuanId
						};
						this.JunTuanAllDataDict[junTuanId] = junTuanDetailData;
					}
					junTuanDetailData.JunTuanData.V = junTuanData;
					junTuanDetailData.JunTuanTaskAllData.V.JunTuanId = junTuanId;
					junTuanDetailData.JunTuanTaskAllData.V.TaskList = list4;
					junTuanDetailData.JunTuanBangHuiList.V = list3;
					junTuanDetailData.JunTuanBaseData.V.BhList = list2;
					junTuanDetailData.JoinDataList.V = list6;
					junTuanDetailData.EventLogList.V = list5;
					junTuanDetailData.JunTuanData.Age = 1L;
					junTuanDetailData.JunTuanRoleDataList.Age = 1L;
					junTuanDetailData.JunTuanTaskAllData.Age = 1L;
					junTuanDetailData.JunTuanBangHuiList.Age = 1L;
					junTuanDetailData.JoinDataList.Age = 1L;
					junTuanDetailData.EventLogList.Age = 1L;
					foreach (JunTuanRequestData junTuanRequestData in list6)
					{
						JunTuanBangHuiData junTuanBangHuiData;
						if (!this.JunTuanBangHuiDataDict.TryGetValue(junTuanRequestData.BhId, out junTuanBangHuiData))
						{
							junTuanBangHuiData = new JunTuanBangHuiData();
							junTuanBangHuiData.BhId = junTuanRequestData.BhId;
							this.JunTuanBangHuiDataDict[junTuanBangHuiData.BhId] = junTuanBangHuiData;
							junTuanBangHuiData.BhName = junTuanRequestData.BhName;
							junTuanBangHuiData.BhZoneId = junTuanRequestData.BhZoneId;
							junTuanBangHuiData.LeaderName = junTuanRequestData.LeaderName;
							junTuanBangHuiData.LeaderZoneId = junTuanRequestData.LeaderZoneId;
							junTuanBangHuiData.RoleNum = junTuanRequestData.RoleNum;
							junTuanBangHuiData.ZhanLi = junTuanRequestData.ZhanLi;
							junTuanBangHuiData.LeaderOccupation = junTuanRequestData.Occupation;
							junTuanBangHuiData.LeaderRoleId = junTuanRequestData.LeaderRoleId;
						}
					}
					foreach (JunTuanBangHuiData junTuanBangHuiData in list3)
					{
						JunTuanBangHuiData junTuanBangHuiData;
						this.JunTuanBangHuiDataDict[junTuanBangHuiData.BhId] = junTuanBangHuiData;
					}
					this.ReloadJunTuanRoleDataList(junTuanDetailData);
					this.UpdateRequestDataListCmdData(junTuanDetailData);
					this.UpdateLogDataListCmdData(junTuanDetailData);
				}
			}
			this.UpdateJunTuanRankDataList();
			this.UpdateCmdData();
			return true;
		}

		public void ReloadJunTuanRoleDataList(JunTuanDetailData detailData)
		{
			List<JunTuanRoleData> list = new List<JunTuanRoleData>();
			this.LoadJunTuanRoleList(list, detailData.JunTuanBaseData.V.BhList, detailData.JunTuanId);
			detailData.JunTuanRoleDataList.V = list;
			this.JunTuanUpdateBhList(detailData, false, true);
			this.UpdateRoleDataListCmdData(detailData);
		}

		public void UpdateCmdData()
		{
			this.UpdateJunTuanMiniDataList();
			this.UpdateJunTuanBaseDataList();
		}

		public void UpdateRequestDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
			detailData.JunTuanData.V.RequestCount = detailData.JoinDataList.V.Count;
			TimeUtil.AgeByNow(ref detailData.RequestDataListCmdData.Age);
			detailData.RequestDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRequestData>>(detailData.JoinDataList.V);
		}

		public void UpdateRoleDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.JunTuanRoleDataListCmdData.Age);
			detailData.JunTuanRoleDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRoleData>>(detailData.JunTuanRoleDataList.V);
		}

		public void UpdateLogDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.EventLogListCmdData.Age);
			detailData.EventLogListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanEventLog>>(detailData.EventLogList.V);
		}

		public int JunTuanRankDataComparison(JunTuanRankData x, JunTuanRankData y)
		{
			int num = y.Point - x.Point;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				result = DateTime.Compare(y.LastTime, x.LastTime);
			}
			return result;
		}

		public void UpdateJunTuanRankDataList()
		{
			int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
			List<JunTuanRankData> list = new List<JunTuanRankData>();
			if (!this.LoadJunTuanRankList(list, weekStartDayIdNow))
			{
				LogManager.WriteLog(1000, "加载军团贡献排行数据失败", null, true);
			}
			else
			{
				lock (this.Mutex)
				{
					int count = this.JunTuanRankDataList.V.Count;
					this.JunTuanRankDataList.V.Clear();
					for (int i = 0; i < list.Count; i++)
					{
						JunTuanRankData junTuanRankData = list[i];
						if (junTuanRankData.Point > 0)
						{
							junTuanRankData.Rank = i + 1;
							if (i < this.MaxPaiMingRank)
							{
								this.JunTuanRankDataList.V.Add(junTuanRankData);
							}
						}
						int junTuanId = junTuanRankData.JunTuanId;
						JunTuanDetailData junTuanDetailData;
						if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out junTuanDetailData))
						{
							if (junTuanDetailData.JunTuanTaskAllData.V.TaskPoint != junTuanRankData.Point || junTuanDetailData.JunTuanTaskAllData.V.TaskLastTime != junTuanRankData.LastTime)
							{
								junTuanDetailData.JunTuanTaskAllData.V.TaskPoint = junTuanRankData.Point;
								junTuanDetailData.JunTuanTaskAllData.V.TaskLastTime = junTuanRankData.LastTime;
								TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanTaskAllData.Age);
							}
							if (junTuanDetailData.JunTuanData.V.WeekRank != junTuanRankData.Rank || junTuanDetailData.JunTuanData.V.WeekPoint != junTuanRankData.Point)
							{
								junTuanDetailData.JunTuanData.V.WeekRank = junTuanRankData.Rank;
								junTuanDetailData.JunTuanData.V.WeekPoint = junTuanRankData.Point;
								TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanData.Age);
							}
						}
					}
					if (count != 0 || this.JunTuanRankDataList.V.Count != 0)
					{
						TimeUtil.AgeByNow(ref this.JunTuanRankDataList.Age);
						this.JunTuanRankDataListCmdData.Age = this.JunTuanRankDataList.Age;
						this.JunTuanRankDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRankData>>(this.JunTuanRankDataList.V);
					}
				}
			}
		}

		public int AddJunTuanPoint(JunTuanDetailData detailData, int point, bool taskPoint = false)
		{
			int point2;
			lock (this.Mutex)
			{
				detailData.JunTuanData.V.Point += point;
				if (taskPoint)
				{
					detailData.JunTuanData.V.WeekPoint += point;
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				this.UpdateJunTuanPointData(detailData.JunTuanData.V.JunTuanId, detailData.JunTuanData.V.Point);
				point2 = detailData.JunTuanData.V.Point;
			}
			return point2;
		}

		public void JunTuanUpdateBhList(JunTuanDetailData detailData, bool updateAll = true, bool updateLeaderInfo = false)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < detailData.JunTuanBangHuiList.V.Count; i++)
				{
					JunTuanBangHuiData junTuanBangHuiData = detailData.JunTuanBangHuiList.V[i];
					list.Add(junTuanBangHuiData.BhId);
					if (i == 0)
					{
						junTuanBangHuiData.JuTuanZhiWu = 1;
						detailData.LeaderBhId = junTuanBangHuiData.BhId;
					}
					else
					{
						junTuanBangHuiData.JuTuanZhiWu = 0;
					}
				}
				if (updateLeaderInfo)
				{
					using (List<JunTuanRoleData>.Enumerator enumerator = detailData.JunTuanRoleDataList.V.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JunTuanRoleData roleData = enumerator.Current;
							if (roleData.JuTuanZhiWu == 1 || roleData.JuTuanZhiWu == 2)
							{
								JunTuanBangHuiData junTuanBangHuiData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == roleData.BhId);
								if (null != junTuanBangHuiData)
								{
									junTuanBangHuiData.LeaderOccupation = roleData.Occu;
									junTuanBangHuiData.LeaderRoleId = roleData.RoleId;
									junTuanBangHuiData.LeaderName = roleData.RoleName;
									junTuanBangHuiData.LeaderZoneId = roleData.ZoneId;
								}
								if (roleData.BhId == detailData.LeaderBhId)
								{
									detailData.JunTuanData.V.LeaderZoneId = roleData.ZoneId;
									detailData.JunTuanData.V.LeaderName = roleData.RoleName;
									detailData.JunTuanData.V.LeaderRoleId = roleData.RoleId;
								}
							}
						}
					}
				}
				detailData.JunTuanBaseData.V.BhList = list;
				TimeUtil.AgeByNow(ref detailData.JunTuanBaseData.Age);
				detailData.JunTuanData.V.BangHuiNum = detailData.JunTuanBangHuiList.V.Count;
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiListCmdData.Age);
				detailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(detailData.JunTuanBangHuiList.V);
				if (updateAll)
				{
					this.UpdateJunTuanMiniDataList();
					this.UpdateJunTuanBaseDataList();
				}
			}
		}

		public void UpdateJunTuanMiniDataList()
		{
			lock (this.Mutex)
			{
				List<JunTuanMiniData> list = new List<JunTuanMiniData>();
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					list.Add(new JunTuanMiniData
					{
						JunTuanId = junTuanDetailData.JunTuanData.V.JunTuanId,
						JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
						LeaderZoneId = junTuanDetailData.JunTuanData.V.LeaderZoneId,
						LeaderName = junTuanDetailData.JunTuanData.V.LeaderName,
						BangHuiNum = junTuanDetailData.JunTuanData.V.BangHuiNum,
						LingDi = junTuanDetailData.JunTuanData.V.LingDi
					});
				}
				TimeUtil.AgeByNow(ref this.JunTuanMiniDataListCmdData.Age);
				this.JunTuanMiniDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanMiniData>>(list);
			}
		}

		public void UpdateJunTuanTaskList()
		{
			bool flag = false;
			bool flag2 = true;
			DateTime time = TimeUtil.NowDateTime();
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow();
			if (timeOfWeekNow < this.RuntimeData.TaskStartEndTimeList[0])
			{
				flag2 = false;
			}
			else if (timeOfWeekNow > this.RuntimeData.TaskStartEndTimeList[1])
			{
				flag = true;
			}
			int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
			lock (this.Mutex)
			{
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					bool flag4 = false;
					if (junTuanDetailData.JunTuanTaskAllData.V.TaskList == null || junTuanDetailData.JunTuanTaskAllData.V.TaskList.Count == 0)
					{
						List<JunTuanTaskData> list2 = new List<JunTuanTaskData>();
						foreach (JunTuanTaskInfo junTuanTaskInfo in this.RuntimeData.TaskList.Value.Values)
						{
							JunTuanTaskData junTuanTaskData = new JunTuanTaskData
							{
								TaskId = junTuanTaskInfo.ID,
								WeekDay = weekStartDayIdNow
							};
							list2.Add(junTuanTaskData);
							this.UpdateJunTuanTaskData(junTuanDetailData.JunTuanId, junTuanTaskData, time, 0);
						}
						junTuanDetailData.JunTuanTaskAllData.V.TaskList = list2;
						flag4 = true;
					}
					List<JunTuanTaskData> list3 = null;
					foreach (JunTuanTaskData junTuanTaskData in junTuanDetailData.JunTuanTaskAllData.V.TaskList)
					{
						JunTuanTaskData junTuanTaskData;
						if (junTuanTaskData.WeekDay != weekStartDayIdNow && flag2)
						{
							if (null == list3)
							{
								list3 = new List<JunTuanTaskData>();
							}
							list3.Add(junTuanTaskData);
						}
						else if (junTuanTaskData.TaskState == 0L && flag)
						{
							junTuanTaskData.TaskState = 2L;
							this.UpdateJunTuanTaskData(junTuanDetailData.JunTuanId, junTuanTaskData);
							flag4 = true;
						}
					}
					if (null != list3)
					{
						flag4 = true;
						foreach (JunTuanTaskData item in list3)
						{
							junTuanDetailData.JunTuanTaskAllData.V.TaskList.Remove(item);
						}
					}
					if (flag4)
					{
						TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanTaskAllData.Age);
					}
				}
			}
		}

		public void CheckJunTuanBangHuiList()
		{
			long num = TimeUtil.NOW();
			lock (this.Mutex)
			{
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					bool flag2 = false;
					if (junTuanDetailData.JunTuanBangHuiList.V != null)
					{
						foreach (JunTuanBangHuiData junTuanBangHuiData in junTuanDetailData.JunTuanBangHuiList.V)
						{
							if (junTuanBangHuiData.NextUpdateTicks > 0L && junTuanBangHuiData.NextUpdateTicks < num)
							{
								flag2 = true;
								junTuanBangHuiData.NextUpdateTicks = 0L;
							}
						}
					}
					if (flag2)
					{
						TimeUtil.AgeByNow(ref junTuanDetailData.JunTuanBangHuiListCmdData.Age);
						junTuanDetailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(junTuanDetailData.JunTuanBangHuiList.V);
					}
				}
			}
		}

		public void UpdateJunTuanBaseDataList()
		{
			lock (this.Mutex)
			{
				this.BangHuiJunTuanIdDict.Clear();
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					JunTuanBaseData junTuanBaseData = new JunTuanBaseData
					{
						JunTuanId = junTuanDetailData.JunTuanData.V.JunTuanId,
						JunTuanName = junTuanDetailData.JunTuanData.V.JunTuanName,
						LingDi = junTuanDetailData.JunTuanData.V.LingDi
					};
					junTuanBaseData.BhList = new List<int>();
					foreach (JunTuanBangHuiData junTuanBangHuiData in junTuanDetailData.JunTuanBangHuiList.V)
					{
						this.BangHuiJunTuanIdDict[junTuanBangHuiData.BhId] = junTuanBaseData.JunTuanId;
						junTuanBaseData.BhList.Add(junTuanBangHuiData.BhId);
					}
					list.Add(junTuanBaseData);
				}
				TimeUtil.AgeByNow(ref this.JunTuanBaseDataListCmdData.Age);
				this.JunTuanBaseDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBaseData>>(list);
			}
		}

		public void AddJuntuanEventLog(JunTuanDetailData detailData, JunTuanEventLog logData)
		{
			try
			{
				lock (this.Mutex)
				{
					TimeUtil.AgeByNow(ref detailData.EventLogList.Age);
					detailData.EventLogList.V.Add(logData);
					if (detailData.EventLogList.V.Count > this.MaxLogCount)
					{
						detailData.EventLogList.V.RemoveRange(this.MaxLogCount, detailData.EventLogList.V.Count - this.MaxLogCount);
					}
					this.UpdateLogDataListCmdData(detailData);
				}
				this.AddJunTuanEventLog(detailData.JunTuanId, logData);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void CheckJunTuanPoint()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int offsetDay = TimeUtil.GetOffsetDay2(dateTime);
			lock (this.Mutex)
			{
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				List<JunTuanDetailData> list2 = new List<JunTuanDetailData>();
				foreach (JunTuanDetailData junTuanDetailData in this.JunTuanAllDataDict.Values)
				{
					if (junTuanDetailData.JunTuanData.V.PointCostDay < offsetDay)
					{
						junTuanDetailData.JunTuanData.V.PointCostDay = offsetDay;
						this.AddJunTuanPoint(junTuanDetailData, -this.RuntimeData.LegionProsperityCost[1], false);
						this.AddDelayWriteSql(string.Format("update t_juntuan set pointcostday={1} where juntuanid={0}", junTuanDetailData.JunTuanId, offsetDay));
						if (junTuanDetailData.JunTuanData.V.Point < this.RuntimeData.LegionProsperityCost[3])
						{
							if (!LingDiCaiJiService.Instance().isLingZhu(junTuanDetailData.JunTuanData.V.JunTuanId))
							{
								list2.Add(junTuanDetailData);
							}
						}
					}
				}
				if (list2.Count > 0)
				{
					foreach (JunTuanDetailData junTuanDetailData in list2)
					{
						this.DestroyJunTuan(junTuanDetailData);
						this.AddJunTuanEventLog(junTuanDetailData.JunTuanId, new JunTuanEventLog
						{
							EventType = 7,
							Message = "AutoDestroy",
							Time = dateTime
						});
					}
				}
			}
		}

		private bool LoadJunTuanRankList(List<JunTuanRankData> list, int weekDay)
		{
			MySqlDataReader mySqlDataReader = null;
			bool result;
			try
			{
				string strSQL = string.Format("SELECT j.juntuanid,juntuanname,SUM(IF(t.`taskstate`=1,t.`taskpoint`,0)) AS `point` ,MAX(lasttime) AS lasttime FROM t_juntuan j LEFT JOIN t_juntuan_task t ON j.juntuanid=t.juntuanid WHERE `weekday`={0} GROUP BY juntuanid ORDER BY SUM(t.`taskpoint`) DESC,MAX(lasttime) ASC,juntuanid ASC;", weekDay);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanRankData
					{
						JunTuanId = Convert.ToInt32(mySqlDataReader[0].ToString()),
						JunTuanName = mySqlDataReader[1].ToString(),
						Point = (int)Convert.ToInt64(mySqlDataReader[2].ToString()),
						LastTime = Convert.ToDateTime(mySqlDataReader[3].ToString())
					});
					num++;
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		public bool LoadJunTuanDataList(List<JunTuanData> list)
		{
			MySqlDataReader mySqlDataReader = null;
			bool result;
			try
			{
				string strSQL = string.Format("SELECT juntuanid,juntuanname,bulletin,zoneid,rname,`point`,pointcostday,lingdi,voice FROM t_juntuan where isdel=0", new object[0]);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanData
					{
						JunTuanId = Convert.ToInt32(mySqlDataReader[0].ToString()),
						JunTuanName = mySqlDataReader[1].ToString(),
						Bulletin = mySqlDataReader[2].ToString(),
						LeaderZoneId = Convert.ToInt32(mySqlDataReader[3].ToString()),
						LeaderName = mySqlDataReader[4].ToString(),
						Point = (int)Convert.ToInt64(mySqlDataReader[5].ToString()),
						PointCostDay = Convert.ToInt32(mySqlDataReader[6].ToString()),
						LingDi = Convert.ToInt32(mySqlDataReader[7].ToString()),
						GVoicePrioritys = mySqlDataReader[7].ToString()
					});
					num++;
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		private bool LoadJunTuanTaskList(List<JunTuanTaskData> list, int weekDay, int junTuanId)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT taskid,taskv,taskstate,lasttime FROM t_juntuan_task WHERE `weekday`={0} AND juntuanid={1};", weekDay, junTuanId);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanTaskData
					{
						TaskId = Convert.ToInt32(mySqlDataReader[0]),
						TaskValue = Convert.ToInt32(mySqlDataReader[1]),
						TaskState = (long)Convert.ToInt32(mySqlDataReader[2]),
						WeekDay = weekDay
					});
					result = true;
					num++;
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
			return result;
		}

		private bool LoadJunTuanBangHuiList(List<JunTuanBangHuiData> list, List<int> bhIdList, int junTuanId)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT bhid,bhzoneid,bhname,rolenum,zhanli,zhiwu FROM t_juntuan_banghui WHERE juntuanid={0} order by zhiwu desc;", junTuanId);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					JunTuanBangHuiData junTuanBangHuiData = new JunTuanBangHuiData();
					junTuanBangHuiData.BhId = Convert.ToInt32(mySqlDataReader[0]);
					junTuanBangHuiData.BhZoneId = Convert.ToInt32(mySqlDataReader[1]);
					junTuanBangHuiData.BhName = mySqlDataReader[2].ToString();
					junTuanBangHuiData.RoleNum = Convert.ToInt32(mySqlDataReader[3]);
					junTuanBangHuiData.ZhanLi = Global.SafeConvertToInt64(mySqlDataReader[4].ToString());
					junTuanBangHuiData.JuTuanZhiWu = Convert.ToInt32(mySqlDataReader[5]);
					if (junTuanBangHuiData.JuTuanZhiWu > 0)
					{
						bhIdList.Insert(0, junTuanBangHuiData.BhId);
						list.Insert(0, junTuanBangHuiData);
					}
					else
					{
						junTuanBangHuiData.JuTuanZhiWu = 0;
						list.Add(junTuanBangHuiData);
						bhIdList.Add(junTuanBangHuiData.BhId);
					}
					result = true;
					num++;
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
			return result;
		}

		public bool LoadJunTuanRoleList(List<JunTuanRoleData> list, List<int> bhidList, int junTuanId)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT rid,rname,zoneid,b.`bhname`,b.`bhzoneid`,r.zhanli,r.zhiwu,zhuansheng,`level`,r.bhid,r.occu FROM t_juntuan_roles r LEFT JOIN t_juntuan_banghui b ON r.bhid=b.bhid WHERE r.bhid in ({0});", string.Join<int>(",", bhidList));
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanRoleData
					{
						RoleId = Convert.ToInt32(mySqlDataReader[0]),
						RoleName = mySqlDataReader[1].ToString(),
						ZoneId = Convert.ToInt32(mySqlDataReader[2]),
						BhName = mySqlDataReader[3].ToString(),
						BhZoneId = Convert.ToInt32(mySqlDataReader[4]),
						ZhanLi = (int)Math.Min(Convert.ToInt64(mySqlDataReader[5]), 2147483647L),
						JuTuanZhiWu = Convert.ToInt32(mySqlDataReader[6]),
						ChangeLifeCount = Convert.ToInt32(mySqlDataReader[7]),
						Level = Convert.ToInt32(mySqlDataReader[8]),
						BhId = Convert.ToInt32(mySqlDataReader[9]),
						Occu = Convert.ToInt32(mySqlDataReader[10])
					});
					result = true;
					num++;
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
			return result;
		}

		private bool LoadJunTuanRequestList(List<JunTuanRequestData> list, int junTuanId)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `bhid`,`juntuanid`,`zoneid`,`bhname`,`zhanli`,`rolenum`,`leadezoneid`,`leadername` FROM `t_juntuan_request` WHERE juntuanid={0} AND `time`>='{1}' ORDER BY `time` DESC;", junTuanId, TimeUtil.NowDateTime().AddDays(-1.0).ToString("yyyy-MM-dd HH:mm:ss"));
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanRequestData
					{
						BhId = Convert.ToInt32(mySqlDataReader[0]),
						JunTuanId = Convert.ToInt32(mySqlDataReader[1]),
						BhZoneId = Convert.ToInt32(mySqlDataReader[2]),
						BhName = mySqlDataReader[3].ToString(),
						ZhanLi = (long)((int)Convert.ToInt64(mySqlDataReader[4])),
						RoleNum = Convert.ToInt32(mySqlDataReader[5]),
						LeaderZoneId = Convert.ToInt32(mySqlDataReader[6]),
						LeaderName = mySqlDataReader[7].ToString()
					});
					result = true;
					num++;
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
			return result;
		}

		private bool LoadJunTuanLogList(List<JunTuanEventLog> list, int junTuanId)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `eventtype`,`time`,`message` FROM t_juntuan_log WHERE juntuanid={0} limit {1};", junTuanId, this.MaxLogCount);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					list.Add(new JunTuanEventLog
					{
						EventType = Convert.ToInt32(mySqlDataReader[0]),
						Time = Convert.ToDateTime(mySqlDataReader[1].ToString()),
						Message = mySqlDataReader[2].ToString()
					});
					result = true;
					num++;
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
			return result;
		}

		public void AddDelayWriteSql(string sql)
		{
			lock (this.Mutex)
			{
				this.DelayWriteSqlQueue.Enqueue(sql);
			}
		}

		private void WriteDataToDb(string sql)
		{
			try
			{
				LogManager.WriteLog(3, sql, null, true);
				DbHelperMySQL.ExecuteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(string.Format("sql: {0}\r\n{1}", sql, ex.ToString()));
			}
		}

		public void DelayWriteDataProc()
		{
			List<string> list = null;
			lock (this.Mutex)
			{
				if (this.DelayWriteSqlQueue.Count == 0)
				{
					return;
				}
				list = this.DelayWriteSqlQueue.ToList<string>();
				this.DelayWriteSqlQueue.Clear();
			}
			foreach (string sql in list)
			{
				this.WriteDataToDb(sql);
			}
		}

		public long ExecuteSqlGetIncrement(string sqlCmd)
		{
			long result;
			try
			{
				LogManager.WriteLog(3, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSqlGetIncrement(sqlCmd, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1L;
			}
			return result;
		}

		public int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(3, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		public long CreateJunTuan(string junTuanName, string bulletin, int zoneid, string rname, int initPoint, int pointcostday)
		{
			string sqlCmd = string.Format("insert  into `t_juntuan`(`juntuanname`,`bulletin`,`zoneid`,`rname`,`point`,`pointcostday`,`lingdi`) values ('{1}','{2}',{3},'{4}',{5},{6},{7});", new object[]
			{
				0,
				junTuanName,
				bulletin,
				zoneid,
				rname,
				initPoint,
				pointcostday,
				0
			});
			return this.ExecuteSqlGetIncrement(sqlCmd);
		}

		public long AddJunTuanJoinData(JunTuanRequestData data)
		{
			try
			{
				string sqlCmd = string.Format("REPLACE INTO `t_juntuan_request` (`bhid`, `juntuanid`, `time`, `state`, `zoneid`, `bhname`, `zhanli`, `rolenum`, `leadezoneid`, `leadername`) VALUES ({0},{1},now(),{2},{3},'{4}',{5},{6},{7},'{8}');", new object[]
				{
					data.BhId,
					data.JunTuanId,
					0,
					data.BhZoneId,
					data.BhName,
					data.ZhanLi,
					data.RoleNum,
					data.LeaderZoneId,
					data.LeaderName
				});
				return (long)this.ExecuteSqlNoQuery(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -1L;
		}

		public int UpdateJunTuanBangHuiData(JunTuanBangHuiData data, int junTuanId)
		{
			string sqlCmd = string.Format("REPLACE INTO `t_juntuan_banghui` (`bhid`, `juntuanid`, `bhzoneid`, `bhname`, `rolenum`, `zhanli`, `zhiwu`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", new object[]
			{
				data.BhId,
				junTuanId,
				data.BhZoneId,
				data.BhName,
				data.RoleNum,
				data.ZhanLi,
				data.JuTuanZhiWu
			});
			return this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void UpdateJunTuanPointData(int junTuanId, int point)
		{
			string sqlCmd = string.Format("update `t_juntuan` set `point`={0} where juntuanid={1};", point, junTuanId);
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void AddJunTuanEventLog(int junTuanId, JunTuanEventLog data)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_log` (`juntuanid`,`eventtype`, `time`, `message`) VALUES ({0},{1},'{2}','{3}');", new object[]
			{
				junTuanId,
				data.EventType,
				TimeUtil.DataTimeToString(data.Time, "yyyy-MM-dd HH:mm:ss"),
				data.Message
			});
			this.AddDelayWriteSql(sql);
		}

		public void UpdateJunTuanTaskData(int junTuanId, JunTuanTaskData data, DateTime time, int taskPoint)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_task` (`weekday`,`juntuanid`,`taskid`, `taskv`, `taskpoint`, `taskstate`, `lasttime`) VALUES ({0},{1},{2},{3},{4},{5},'{6}') on duplicate key update `taskv`={3}, `taskpoint`={4}, `taskstate`={5}, `lasttime`='{6}';", new object[]
			{
				data.WeekDay,
				junTuanId,
				data.TaskId,
				data.TaskValue,
				taskPoint,
				data.TaskState,
				TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss")
			});
			this.AddDelayWriteSql(sql);
		}

		public void UpdateJunTuanTaskData(int junTuanId, JunTuanTaskData data)
		{
			string sql = string.Format("update `t_juntuan_task` set taskstate={3},taskv={4} where weekday={0} and juntuanid={1} and taskid={2};", new object[]
			{
				data.WeekDay,
				junTuanId,
				data.TaskId,
				data.TaskState,
				data.TaskValue
			});
			this.AddDelayWriteSql(sql);
		}

		public void DeleteJunTuanRequestData(JunTuanDetailData detailData, JunTuanRequestData data)
		{
			lock (this.Mutex)
			{
				detailData.JoinDataList.V.Remove(data);
				TimeUtil.AgeByNow(ref detailData.RequestDataListCmdData.Age);
				detailData.RequestDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRequestData>>(detailData.JoinDataList.V);
			}
			string sql = string.Format("DELETE FROM t_juntuan_request WHERE bhid={0} AND juntuanid={1};", data.BhId, data.JunTuanId);
			this.AddDelayWriteSql(sql);
		}

		public void UpdateJunTuanLingDi(int junTuanId, int lingDi)
		{
			int num = 3 ^ lingDi;
			string sql = string.Format("UPDATE t_juntuaN SET lingdi=lingdi|{1} WHERE juntuanid={0};", junTuanId, lingDi, num);
			this.AddDelayWriteSql(sql);
			sql = string.Format("UPDATE t_juntuaN SET lingdi=lingdi&{2} WHERE juntuanid<>{0};", junTuanId, lingDi, num);
			this.AddDelayWriteSql(sql);
		}

		public void DestroyJunTuan(JunTuanDetailData detailData)
		{
			foreach (JunTuanBangHuiData junTuanBangHuiData in detailData.JunTuanBangHuiList.V)
			{
				junTuanBangHuiData.JuTuanZhiWu = 0;
				this.UpdateJunTuanBangHuiData(junTuanBangHuiData, 0);
				this.BangHuiJunTuanIdDict[junTuanBangHuiData.BhId] = 0;
			}
			this.JunTuanAllDataDict.Remove(detailData.JunTuanId);
			this.DeleteJunTuan(detailData.JunTuanId);
			this.UpdateJunTuanMiniDataList();
			this.UpdateJunTuanBaseDataList();
		}

		public void DeleteJunTuan(int junTuanId)
		{
			string sql = string.Format("update t_juntuan set isdel=1 WHERE juntuanid={0};", junTuanId);
			this.AddDelayWriteSql(sql);
		}

		public void UpdateJuntuanRoleDataList(int bhid, Dictionary<int, JunTuanRoleData> dict)
		{
			try
			{
				string sql = string.Format("DELETE FROM t_juntuan_roles WHERE bhid={1} and rid NOT IN ({0});", string.Join<int>(",", dict.Keys), bhid);
				this.AddDelayWriteSql(sql);
				foreach (JunTuanRoleData junTuanRoleData in dict.Values)
				{
					sql = string.Format("REPLACE INTO t_juntuan_roles(bhid,rid,rname,zoneid,zhanli,zhiwu,zhuansheng,`level`,occu) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8});", new object[]
					{
						junTuanRoleData.BhId,
						junTuanRoleData.RoleId,
						junTuanRoleData.RoleName,
						junTuanRoleData.ZoneId,
						junTuanRoleData.ZhanLi,
						junTuanRoleData.JuTuanZhiWu,
						junTuanRoleData.ChangeLifeCount,
						junTuanRoleData.Level,
						junTuanRoleData.Occu
					});
					this.AddDelayWriteSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void UpdateJuntuanRoleData(JunTuanRoleData data)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_juntuan_roles(bhid,rid,rname,zoneid,zhanli,zhiwu,zhuansheng,`level`,occu) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8});", new object[]
				{
					data.BhId,
					data.RoleId,
					data.RoleName,
					data.ZoneId,
					data.ZhanLi,
					data.JuTuanZhiWu,
					data.ChangeLifeCount,
					data.Level,
					data.Occu
				});
				this.AddDelayWriteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public bool GetHongBaoHuoDongData(ref long huoDongStartTicks, ref int nextSendId, ref long leftCharge, ref long totalCharge)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 40);
				int num;
				if (single == null || !int.TryParse(single.ToString(), out num))
				{
					huoDongStartTicks = DateTime.MinValue.Ticks;
				}
				else
				{
					huoDongStartTicks = TimeUtil.UnixSecondsToTicks(num);
				}
				single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 43);
				if (single == null || !int.TryParse(single.ToString(), out nextSendId))
				{
					huoDongStartTicks = 0L;
				}
				single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 41);
				if (single == null || !long.TryParse(single.ToString(), out leftCharge))
				{
					huoDongStartTicks = 0L;
				}
				single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 42);
				if (single == null || !long.TryParse(single.ToString(), out totalCharge))
				{
					huoDongStartTicks = 0L;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		public void UpdateHongBaoHuoDongData(long huoDongStartTicks, int nextSendId, long leftCharge, long totalCharge)
		{
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 40, TimeUtil.SysTicksToUnixSeconds(huoDongStartTicks)));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 43, nextSendId));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 41, leftCharge));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 42, totalCharge));
		}

		public long CreateHongBao(string keystr, int senderid, DateTime startTime, DateTime endTime, int zuanshi, int state)
		{
			string sqlCmd = string.Format("INSERT INTO `t_hongbao_chongzhi_send` (`keystr`,`senderid`,`sendtime`,`endtime`,`zuanshi`,`leftzuanshi`,`state`) VALUES ('{0}','{1}','{2}','{3}','{4}','{4}','{5}');", new object[]
			{
				keystr,
				senderid,
				startTime.ToString("yyyy-MM-dd HH:mm:ss"),
				endTime.ToString("yyyy-MM-dd HH:mm:ss"),
				zuanshi,
				state
			});
			return this.ExecuteSqlGetIncrement(sqlCmd);
		}

		public void UpdateHongBao(int hongbaoid, int leftzuanshi, int state)
		{
			string sql = string.Format("UPDATE `t_hongbao_chongzhi_send` SET leftzuanshi={1},state={2} WHERE id={0};", hongbaoid, leftzuanshi, state);
			this.AddDelayWriteSql(sql);
		}

		public bool LoadHongBaoDataList(string keyStr, Dictionary<int, SystemHongBaoData> hongBaoDict, Dictionary<long, int> recvDict)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT id,senderid,sendtime,endtime,leftzuanshi,state FROM `t_hongbao_chongzhi_send` where keystr='{0}';", keyStr);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					SystemHongBaoData systemHongBaoData = new SystemHongBaoData();
					systemHongBaoData.HongBaoId = Convert.ToInt32(mySqlDataReader[0].ToString());
					systemHongBaoData.ID = (int)Convert.ToInt64(mySqlDataReader[1].ToString());
					systemHongBaoData.StartTime = Convert.ToDateTime(mySqlDataReader[2].ToString()).Ticks / 10000L;
					systemHongBaoData.DurationTime = (int)(Convert.ToDateTime(mySqlDataReader[3].ToString()).Ticks / 10000L - systemHongBaoData.StartTime);
					systemHongBaoData.LeftZuanShi = Convert.ToInt32(mySqlDataReader[4].ToString());
					systemHongBaoData.State = Convert.ToInt32(mySqlDataReader[5].ToString());
					hongBaoDict[systemHongBaoData.HongBaoId] = systemHongBaoData;
					num++;
				}
				mySqlDataReader.Close();
				for (int i = 0; i < 100000000; i += 10000)
				{
					bool flag = false;
					strSQL = string.Format("SELECT hongbaoid,rid FROM `t_hongbao_chongzhi_recv` where keystr='{0}' limit {1},10000;", keyStr, i);
					mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					num = 1;
					while (mySqlDataReader.Read())
					{
						flag = true;
						int num2 = Convert.ToInt32(mySqlDataReader[0].ToString());
						int num3 = Convert.ToInt32(mySqlDataReader[1].ToString());
						long key = ((long)num2 << 36) + (long)num3;
						recvDict[key] = 1;
						num++;
					}
					mySqlDataReader.Close();
					if (!flag)
					{
						break;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return true;
		}

		public void WriteHongBaoRecv(string keystr, int hongBaoId, int rid, int zoneid, string userid, string rname, int zuanshi)
		{
			string sql = string.Format("INSERT INTO `t_hongbao_chongzhi_recv` (`keystr`, `hongbaoid`, `rid`, `lasttime`, `zoneid`, `userid`, `rname`, `zuanshi`) VALUES ('{0}', '{1}', '{2}', now(), '{3}', '{4}', '{5}', '{6}');", new object[]
			{
				keystr,
				hongBaoId,
				rid,
				zoneid,
				userid,
				rname,
				zuanshi
			});
			this.AddDelayWriteSql(sql);
		}

		public static readonly JunTuanPersistence Instance = new JunTuanPersistence();

		public object Mutex = new object();

		public bool Initialized = false;

		public JunTuanRuntimeData RuntimeData = new JunTuanRuntimeData();

		public KuaFuCmdData JunTuanBaseDataListCmdData = new KuaFuCmdData();

		public KuaFuCmdData JunTuanMiniDataListCmdData = new KuaFuCmdData();

		public KuaFuCmdData JunTuanRankDataListCmdData = new KuaFuCmdData();

		public Dictionary<int, JunTuanDetailData> JunTuanAllDataDict = new Dictionary<int, JunTuanDetailData>();

		public Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		public KuaFuData<List<JunTuanRankData>> JunTuanRankDataList = new KuaFuData<List<JunTuanRankData>>();

		public Dictionary<int, JunTuanBangHuiData> JunTuanBangHuiDataDict = new Dictionary<int, JunTuanBangHuiData>();

		private int MaxPaiMingRank = 100;

		private int MaxLogCount = 100;

		private int CurrGameId = Global.UninitGameId;

		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
