using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class JunTuanEraService
	{
		public static JunTuanEraService Instance()
		{
			return JunTuanEraService._instance;
		}

		public void InitConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string fileName = "Config/EraUI.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraUIConfigDict.Load(resourcePath, null);
					fileName = "Config/EraTask.xml";
					resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraTaskConfigDict.Clear();
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EraTaskConfig eraTaskConfig = new EraTaskConfig();
						eraTaskConfig.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						eraTaskConfig.EraID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EraID", 0L);
						eraTaskConfig.EraStage = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EraStage", 0L);
						eraTaskConfig.Reward = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Reward", 0L);
						string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "CompletionCondition", "");
						string[] array = elementAttributeValue.Split(new char[]
						{
							'|'
						});
						foreach (string text in array)
						{
							string[] array3 = text.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								int key = Convert.ToInt32(array3[0]);
								int value = Convert.ToInt32(array3[1]);
								eraTaskConfig.CompletionCondition.Add(new KeyValuePair<int, int>(key, value));
							}
						}
						this.RuntimeData.EraTaskConfigDict[eraTaskConfig.ID] = eraTaskConfig;
					}
					fileName = "Config/EraReward.xml";
					resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RuntimeData.EraAwardConfigDict.Clear();
					xelement = ConfigHelper.Load(resourcePath);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EraAwardConfigBase eraAwardConfigBase = new EraAwardConfigBase();
						eraAwardConfigBase.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						eraAwardConfigBase.EraID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EraID", 0L);
						eraAwardConfigBase.AwardType = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Type", 0L);
						string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement2, "StartTime", "");
						if (!string.IsNullOrEmpty(elementAttributeValue2))
						{
							DateTime.TryParse(elementAttributeValue2, out eraAwardConfigBase.StartTime);
						}
						string elementAttributeValue3 = ConfigHelper.GetElementAttributeValue(xelement2, "EndTime", "");
						if (!string.IsNullOrEmpty(elementAttributeValue3))
						{
							DateTime.TryParse(elementAttributeValue3, out eraAwardConfigBase.EndTime);
						}
						this.RuntimeData.EraAwardConfigDict[eraAwardConfigBase.ID] = eraAwardConfigBase;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public KuaFuCmdData GetEraRankData(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (dataAge != this.EraRankList.Age)
				{
					result = new KuaFuCmdData
					{
						Age = this.EraRankList.Age,
						Bytes0 = DataHelper2.ObjectToBytes<List<KFEraRankData>>(this.EraRankList.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.EraRankList.Age
					};
				}
			}
			return result;
		}

		public KuaFuCmdData GetEraData(int juntuanid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (0 == this.RuntimeData.CurrentEraID)
				{
					result = null;
				}
				else
				{
					KuaFuData<KFEraData> kuaFuData = null;
					if (juntuanid == 0 && 0L == dataAge)
					{
						kuaFuData = new KuaFuData<KFEraData>();
						kuaFuData.V.EraID = this.RuntimeData.CurrentEraID;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
					}
					else
					{
						if (!this.EraDataDict.TryGetValue(juntuanid, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KFEraData>();
							kuaFuData.V.EraID = this.RuntimeData.CurrentEraID;
							kuaFuData.V.JunTuanID = juntuanid;
							kuaFuData.V.EraStage = 1;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.EraDataDict[juntuanid] = kuaFuData;
						}
						if (kuaFuData.V.FastEraStage != this.RuntimeData.CurFastEraStage || kuaFuData.V.FastEraStateProcess != this.RuntimeData.CurFastEraStateProcess)
						{
							TimeUtil.AgeByNow(ref kuaFuData.Age);
						}
					}
					kuaFuData.V.FastEraStage = this.RuntimeData.CurFastEraStage;
					kuaFuData.V.FastEraStateProcess = this.RuntimeData.CurFastEraStateProcess;
					if (dataAge != kuaFuData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFEraData>(kuaFuData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = kuaFuData.Age
						};
					}
				}
			}
			return result;
		}

		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			bool result;
			lock (this.Mutex)
			{
				if (0 == this.RuntimeData.CurrentEraID)
				{
					result = false;
				}
				else
				{
					KuaFuData<KFEraData> kuaFuData = null;
					if (!this.EraDataDict.TryGetValue(juntuanid, out kuaFuData))
					{
						result = false;
					}
					else
					{
						EraTaskConfig eraTaskConfig = null;
						lock (this.RuntimeData.Mutex)
						{
							if (!this.RuntimeData.EraTaskConfigDict.TryGetValue(taskid, out eraTaskConfig))
							{
								return false;
							}
						}
						if (eraTaskConfig.EraID != this.RuntimeData.CurrentEraID)
						{
							result = false;
						}
						else
						{
							EraTaskData eraTaskData = kuaFuData.V.EraTaskList.Find((EraTaskData x) => x.TaskID == taskid);
							if (null == eraTaskData)
							{
								eraTaskData = new EraTaskData();
								eraTaskData.TaskID = taskid;
								kuaFuData.V.EraTaskList.Add(eraTaskData);
							}
							if (this.CheckTaskComplete(eraTaskData, eraTaskConfig))
							{
								result = true;
							}
							else
							{
								for (int i = 0; i < eraTaskConfig.CompletionCondition.Count; i++)
								{
									int value = eraTaskConfig.CompletionCondition[i].Value;
									switch (i)
									{
									case 0:
										eraTaskData.TaskVal1 = Math.Min(eraTaskData.TaskVal1 + var1, value);
										break;
									case 1:
										eraTaskData.TaskVal2 = Math.Min(eraTaskData.TaskVal2 + var2, value);
										break;
									case 2:
										eraTaskData.TaskVal3 = Math.Min(eraTaskData.TaskVal3 + var3, value);
										break;
									}
								}
								if (this.CheckTaskComplete(eraTaskData, eraTaskConfig))
								{
									if (this.HandleAddEraProcess(kuaFuData, eraTaskConfig))
									{
										this.SaveEraData(kuaFuData.V, true);
									}
								}
								else
								{
									this.SaveEraData(kuaFuData.V, false);
								}
								this.SaveEraTaskData(juntuanid, eraTaskData);
								TimeUtil.AgeByNow(ref kuaFuData.Age);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private void HandleFastEraStage(KFEraData data)
		{
			if (data.EraStage > this.RuntimeData.CurFastEraStage)
			{
				this.RuntimeData.CurFastEraStage = data.EraStage;
				this.RuntimeData.CurFastEraStateProcess = data.EraStageProcess;
			}
			else if (data.EraStage == this.RuntimeData.CurFastEraStage && data.EraStageProcess > this.RuntimeData.CurFastEraStateProcess)
			{
				this.RuntimeData.CurFastEraStateProcess = data.EraStageProcess;
			}
		}

		private bool HandleAddEraProcess(KuaFuData<KFEraData> data, EraTaskConfig taskConfig)
		{
			bool result;
			if (taskConfig.EraStage != (int)data.V.EraStage)
			{
				result = false;
			}
			else
			{
				int eraStageProcess = data.V.EraStageProcess;
				int eraStage = (int)data.V.EraStage;
				data.V.EraStageProcess = Math.Min(data.V.EraStageProcess + taskConfig.Reward, 100);
				if (data.V.EraStageProcess == 100 && data.V.EraStage < 4)
				{
					data.V.EraStage = (byte)Math.Min((int)(data.V.EraStage + 1), 4);
					data.V.EraStageProcess = 0;
					AsyncDataItem evItem = new AsyncDataItem(32, new object[]
					{
						data.V.JunTuanID
					});
					ClientAgentManager.Instance().BroadCastAsyncEvent(21, evItem, 0);
					data.V.EraTimePointList.Add(TimeUtil.NowDateTime());
				}
				if (data.V.EraStageProcess == eraStageProcess && (int)data.V.EraStage == eraStage)
				{
					result = false;
				}
				else
				{
					if (data.V.EraStageProcess == 100 && data.V.EraStage == 4)
					{
						data.V.EraTimePointList.Add(TimeUtil.NowDateTime());
					}
					this.HandleFastEraStage(data.V);
					bool flag = false;
					KFEraRankData kferaRankData = this.EraRankList.V.Find((KFEraRankData x) => x.JunTuanID == data.V.JunTuanID);
					if (null != kferaRankData)
					{
						flag = true;
						kferaRankData.JunTuanID = data.V.JunTuanID;
						kferaRankData.EraStage = data.V.EraStage;
						kferaRankData.EraStageProcess = data.V.EraStageProcess;
						kferaRankData.RankTime = TimeUtil.NowDateTime();
					}
					else if (this.EraRankList.V.Count < 5)
					{
						flag = true;
						kferaRankData = new KFEraRankData();
						kferaRankData.JunTuanID = data.V.JunTuanID;
						kferaRankData.EraStage = data.V.EraStage;
						kferaRankData.EraStageProcess = data.V.EraStageProcess;
						kferaRankData.RankTime = TimeUtil.NowDateTime();
						this.EraRankList.V.Add(kferaRankData);
					}
					else
					{
						KFEraRankData kferaRankData2 = this.EraRankList.V[this.EraRankList.V.Count - 1];
						if (data.V.EraStage > kferaRankData2.EraStage || (data.V.EraStage == kferaRankData2.EraStage && data.V.EraStageProcess > kferaRankData2.EraStageProcess))
						{
							flag = true;
							kferaRankData2.JunTuanID = data.V.JunTuanID;
							kferaRankData2.EraStage = data.V.EraStage;
							kferaRankData2.EraStageProcess = data.V.EraStageProcess;
							kferaRankData2.RankTime = TimeUtil.NowDateTime();
						}
					}
					if (flag)
					{
						this.EraRankList.V.Sort(delegate(KFEraRankData left, KFEraRankData right)
						{
							int result2;
							if (left.EraStage > right.EraStage)
							{
								result2 = -1;
							}
							else if (left.EraStage < right.EraStage)
							{
								result2 = 1;
							}
							else if (left.EraStageProcess > right.EraStageProcess)
							{
								result2 = -1;
							}
							else if (left.EraStageProcess < right.EraStageProcess)
							{
								result2 = 1;
							}
							else if (left.RankTime < right.RankTime)
							{
								result2 = -1;
							}
							else if (left.RankTime > right.RankTime)
							{
								result2 = 1;
							}
							else
							{
								result2 = 0;
							}
							return result2;
						});
						for (int i = 0; i < this.EraRankList.V.Count; i++)
						{
							this.EraRankList.V[i].RankValue = i + 1;
						}
						TimeUtil.AgeByNow(ref this.EraRankList.Age);
					}
					result = true;
				}
			}
			return result;
		}

		private bool CheckTaskComplete(EraTaskData taskData, EraTaskConfig taskConfig)
		{
			bool flag = true;
			for (int i = 0; i < taskConfig.CompletionCondition.Count; i++)
			{
				int value = taskConfig.CompletionCondition[i].Value;
				switch (i)
				{
				case 0:
					flag &= (taskData.TaskVal1 >= value);
					break;
				case 1:
					flag &= (taskData.TaskVal2 >= value);
					break;
				case 2:
					flag &= (taskData.TaskVal3 >= value);
					break;
				}
			}
			return flag;
		}

		private int CalCurrentEraID(DateTime now)
		{
			lock (this.RuntimeData.Mutex)
			{
				Dictionary<int, EraUIConfig> value = this.RuntimeData.EraUIConfigDict.Value;
				foreach (EraUIConfig eraUIConfig in value.Values)
				{
					if (now >= eraUIConfig.StartTime && now <= eraUIConfig.EndTime)
					{
						return eraUIConfig.ID;
					}
				}
			}
			return 0;
		}

		public void HandleChangeEraID(DateTime now, bool broadCast = false)
		{
			lock (this.Mutex)
			{
				int offsetDay = TimeUtil.GetOffsetDay(now);
				if (offsetDay != this.RuntimeData.EraUpdateDayID)
				{
					this.RuntimeData.EraUpdateDayID = offsetDay;
					int num = this.CalCurrentEraID(now);
					if (num != this.RuntimeData.CurrentEraID)
					{
						this.EraDataDict.Clear();
						this.EraRankList.V.Clear();
						TimeUtil.AgeByNow(ref this.EraRankList.Age);
						this.RuntimeData.CurrentEraID = num;
						this.RuntimeData.CurFastEraStage = 1;
						this.RuntimeData.CurFastEraStateProcess = 0;
						if (broadCast)
						{
							AsyncDataItem evItem = new AsyncDataItem(33, new object[]
							{
								num
							});
							ClientAgentManager.Instance().BroadCastAsyncEvent(21, evItem, 0);
						}
					}
				}
			}
		}

		private bool InRankAwardTime()
		{
			int currentEraID = this.RuntimeData.CurrentEraID;
			bool result;
			if (currentEraID <= 0)
			{
				result = false;
			}
			else
			{
				Dictionary<int, EraAwardConfigBase> dictionary = null;
				lock (this.RuntimeData.Mutex)
				{
					dictionary = this.RuntimeData.EraAwardConfigDict;
				}
				foreach (EraAwardConfigBase eraAwardConfigBase in dictionary.Values)
				{
					if (eraAwardConfigBase.EraID == currentEraID && eraAwardConfigBase.AwardType == 2)
					{
						DateTime t = TimeUtil.NowDateTime();
						if (t >= eraAwardConfigBase.StartTime && t <= eraAwardConfigBase.EndTime)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public void OnJunTuanDestroy(int juntuanId)
		{
			if (!this.InRankAwardTime())
			{
				lock (this.Mutex)
				{
					KuaFuData<KFEraData> kuaFuData = null;
					if (this.EraDataDict.TryGetValue(juntuanId, out kuaFuData))
					{
						bool flag2 = false;
						foreach (KFEraRankData kferaRankData in this.EraRankList.V)
						{
							if (kferaRankData.JunTuanID == juntuanId)
							{
								flag2 = true;
							}
						}
						this.ClearEraData(this.RuntimeData.CurrentEraID, juntuanId);
						this.EraDataDict.Remove(juntuanId);
						if (flag2)
						{
							this.LoadEraRankData();
						}
					}
				}
			}
		}

		public void LoadJunTuanEraData()
		{
			this.HandleChangeEraID(TimeUtil.NowDateTime(), false);
			this.LoadEraData();
			this.HandleEraTaskAccident();
			this.LoadEraRankData();
		}

		private void HandleEraTaskAccident()
		{
			if (this.EraDataDict.Count > 0)
			{
				this.RuntimeData.CurFastEraStage = 1;
				this.RuntimeData.CurFastEraStateProcess = 0;
				foreach (KuaFuData<KFEraData> kuaFuData in this.EraDataDict.Values)
				{
					int num = 0;
					foreach (EraTaskData eraTaskData in kuaFuData.V.EraTaskList)
					{
						EraTaskConfig eraTaskConfig = null;
						lock (this.RuntimeData.Mutex)
						{
							if (!this.RuntimeData.EraTaskConfigDict.TryGetValue(eraTaskData.TaskID, out eraTaskConfig))
							{
								continue;
							}
						}
						if (eraTaskConfig.EraStage == (int)kuaFuData.V.EraStage)
						{
							if (this.CheckTaskComplete(eraTaskData, eraTaskConfig))
							{
								num += eraTaskConfig.Reward;
							}
						}
					}
					if (num != kuaFuData.V.EraStageProcess || kuaFuData.V.EraStageProcess > 100)
					{
						LogManager.WriteLog(5, string.Format("HandleEraTaskAccident JunTuanId:{0} Stage:{1} BeforeProcess:{2} AfterProcess:{3}", new object[]
						{
							kuaFuData.V.JunTuanID,
							kuaFuData.V.EraStage,
							kuaFuData.V.EraStageProcess,
							num
						}), null, true);
						kuaFuData.V.EraStageProcess = Math.Min(num, 100);
						this.SaveEraData(kuaFuData.V, false);
					}
					this.HandleFastEraStage(kuaFuData.V);
				}
			}
		}

		private void LoadEraRankData()
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				this.EraRankList.V.Clear();
				string strSQL = string.Format("SELECT `juntuanid`, `stage`, `process`, `ranktm` FROM t_juntuan_era WHERE `eraid`={0} AND (`stage`>1 OR (`stage`=1 AND `process`>0))\r\n                                ORDER BY `stage` DESC, `process` DESC, `ranktm` ASC LIMIT {1};", this.RuntimeData.CurrentEraID, 5);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFEraRankData kferaRankData = new KFEraRankData();
					kferaRankData.RankValue = num;
					kferaRankData.JunTuanID = Convert.ToInt32(mySqlDataReader["juntuanid"].ToString());
					kferaRankData.EraStage = Convert.ToByte(mySqlDataReader["stage"].ToString());
					kferaRankData.EraStageProcess = Convert.ToInt32(mySqlDataReader["process"].ToString());
					string text = mySqlDataReader["ranktm"].ToString();
					if (!string.IsNullOrEmpty(text))
					{
						DateTime.TryParse(text, out kferaRankData.RankTime);
					}
					this.EraRankList.V.Add(kferaRankData);
					num++;
				}
				TimeUtil.AgeByNow(ref this.EraRankList.Age);
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
		}

		private void LoadEraData()
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT * FROM t_juntuan_era WHERE `eraid`={0};", this.RuntimeData.CurrentEraID);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				this.EraDataDict.Clear();
				int num = 1;
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KuaFuData<KFEraData> kuaFuData = new KuaFuData<KFEraData>();
					kuaFuData.V.EraID = Convert.ToInt32(mySqlDataReader["eraid"].ToString());
					kuaFuData.V.JunTuanID = Convert.ToInt32(mySqlDataReader["juntuanid"].ToString());
					kuaFuData.V.EraStage = Convert.ToByte(mySqlDataReader["stage"].ToString());
					kuaFuData.V.EraStageProcess = Convert.ToInt32(mySqlDataReader["process"].ToString());
					kuaFuData.V.EraTaskList = this.LoadEraTaskList(kuaFuData.V.JunTuanID);
					kuaFuData.V.ParseEraTimePointsData(mySqlDataReader["tmpoints"].ToString());
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					this.EraDataDict[kuaFuData.V.JunTuanID] = kuaFuData;
					this.HandleFastEraStage(kuaFuData.V);
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
		}

		private List<EraTaskData> LoadEraTaskList(int junTuanId)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				List<EraTaskData> list = new List<EraTaskData>();
				string strSQL = string.Format("SELECT * FROM t_juntuan_era_task WHERE `eraid`={0} AND `juntuanid`={1};", this.RuntimeData.CurrentEraID, junTuanId);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					list.Add(new EraTaskData
					{
						TaskID = Convert.ToInt32(mySqlDataReader["taskid"].ToString()),
						TaskVal1 = Convert.ToInt32(mySqlDataReader["taskv1"].ToString()),
						TaskVal2 = Convert.ToInt32(mySqlDataReader["taskv2"].ToString()),
						TaskVal3 = Convert.ToInt32(mySqlDataReader["taskv3"].ToString())
					});
				}
				return list;
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
			return null;
		}

		private void SaveEraTaskData(int junTuanId, EraTaskData data)
		{
			string sqlCmd = string.Format("INSERT INTO `t_juntuan_era_task` (`eraid`,`juntuanid`,`taskid`,`taskv1`,`taskv2`,`taskv3`) VALUES ({0},{1},{2},{3},{4},{5})\r\n                                ON DUPLICATE KEY UPDATE `taskv1`={3}, `taskv2`={4}, `taskv3`={5};", new object[]
			{
				this.RuntimeData.CurrentEraID,
				junTuanId,
				data.TaskID,
				data.TaskVal1,
				data.TaskVal2,
				data.TaskVal3
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		private void SaveEraData(KFEraData data, bool chgProcess = false)
		{
			string sqlCmd = string.Format("INSERT INTO `t_juntuan_era` (`eraid`,`juntuanid`,`stage`,`process`,`tmpoints`,`ranktm`) VALUES ({0},{1},{2},{3},'{4}',NOW())\r\n                                ON DUPLICATE KEY UPDATE `stage`={2}, `process`={3}, `tmpoints`='{4}';", new object[]
			{
				data.EraID,
				data.JunTuanID,
				data.EraStage,
				data.EraStageProcess,
				data.getStringValue(data.EraTimePointList)
			});
			this.ExecuteSqlNoQuery(sqlCmd);
			if (chgProcess)
			{
				sqlCmd = string.Format("UPDATE t_juntuan_era SET ranktm=NOW() WHERE `eraid`={0} AND `juntuanid`={1};", data.EraID, data.JunTuanID);
				this.ExecuteSqlNoQuery(sqlCmd);
			}
		}

		private void ClearEraData(int eraID, int juntuanId)
		{
			string sqlCmd = string.Format("DELETE FROM `t_juntuan_era` WHERE `eraid`={0} AND `juntuanid`={1}", eraID, juntuanId);
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		private int ExecuteSqlNoQuery(string sqlCmd)
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

		private static JunTuanEraService _instance = new JunTuanEraService();

		private object Mutex = new object();

		private JunTuanEraRuntimeData RuntimeData = new JunTuanEraRuntimeData();

		public KuaFuData<List<KFEraRankData>> EraRankList = new KuaFuData<List<KFEraRankData>>();

		public Dictionary<int, KuaFuData<KFEraData>> EraDataDict = new Dictionary<int, KuaFuData<KFEraData>>();
	}
}
