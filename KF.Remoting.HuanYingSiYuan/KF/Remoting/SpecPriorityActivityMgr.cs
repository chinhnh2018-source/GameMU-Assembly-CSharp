using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class SpecPriorityActivityMgr
	{
		public static SpecPriorityActivityMgr Instance()
		{
			return SpecPriorityActivityMgr._instance;
		}

		public void InitConfig()
		{
			try
			{
				lock (this.Mutex)
				{
					string fileName = "Config/TeQuanTiaoJian.xml";
					string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.SpecPConditionList.Clear();
					XElement xelement = ConfigHelper.Load(resourcePath);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						SpecPConditionConfig specPConditionConfig = new SpecPConditionConfig();
						specPConditionConfig.GroupID = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "KaiQiShiJian", "");
						if (!string.IsNullOrEmpty(elementAttributeValue))
						{
							specPConditionConfig.FromDate = DateTime.Parse(elementAttributeValue);
						}
						else
						{
							specPConditionConfig.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement2, "JieShuShiJian", "");
						if (!string.IsNullOrEmpty(elementAttributeValue2))
						{
							specPConditionConfig.ToDate = DateTime.Parse(elementAttributeValue2);
						}
						else
						{
							specPConditionConfig.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						specPConditionConfig.ConditionType = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "TiaoJianLeiXing", 0L);
						this.SpecPConditionList.Add(specPConditionConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void LoadDatabase(DateTime now)
		{
			try
			{
				lock (this.Mutex)
				{
					this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
					this.InitActivityConditionInfo(now, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "SpecPriorityActivityMgr.LoadDatabase failed!", ex, true);
			}
		}

		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			try
			{
				lock (this.Mutex)
				{
					int num = 0;
					if (this.ActConditionInfoDict.TryGetValue(key, out num))
					{
						this.ActConditionInfoDict[key] = num + add;
					}
					else
					{
						this.ActConditionInfoDict[key] = add;
					}
					this.SaveActivityConditionInfo();
					return 0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11000;
		}

		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			SpecPrioritySyncData result;
			lock (this.Mutex)
			{
				result = new SpecPrioritySyncData
				{
					ActConditionInfoDict = new Dictionary<int, int>(this.ActConditionInfoDict)
				};
			}
			return result;
		}

		private void SaveActivityConditionInfo()
		{
			lock (this.Mutex)
			{
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						SpecPConditionConfig specPConditionConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						});
						if (null != specPConditionConfig)
						{
							string text = string.Format("{0}_{1}_{2}", specPConditionConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss"), specPConditionConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss"), specPConditionConfig.GroupID);
							int specPriorityActivityType = this.SpecPriorityActivityType;
							string huoDongKeyStr = text;
							string userid = "0";
							KeyValuePair<int, int> kvp3 = kvp;
							this.UpdateHuodongAwardUserHist(specPriorityActivityType, huoDongKeyStr, userid, kvp3.Value);
						}
					}
				}
			}
		}

		private void InitActivityConditionInfo(DateTime now, bool launch = false)
		{
			lock (this.Mutex)
			{
				List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
				List<int> list2 = new List<int>();
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						if (!list.Exists(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						}))
						{
							List<int> list3 = list2;
							KeyValuePair<int, int> kvp3 = kvp;
							list3.Add(kvp3.Key);
						}
					}
				}
				foreach (int key in list2)
				{
					this.ActConditionInfoDict.Remove(key);
				}
				foreach (SpecPConditionConfig specPConditionConfig in list)
				{
					if (!this.ActConditionInfoDict.ContainsKey(specPConditionConfig.GroupID))
					{
						this.ActConditionInfoDict[specPConditionConfig.GroupID] = 0;
					}
				}
				if (launch)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>(this.ActConditionInfoDict);
					using (Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, int> kvp = enumerator.Current;
							SpecPConditionConfig specPConditionConfig2 = list.Find(delegate(SpecPConditionConfig x)
							{
								int groupID = x.GroupID;
								KeyValuePair<int, int> kvp4 = kvp;
								return groupID == kvp4.Key;
							});
							if (null != specPConditionConfig2)
							{
								string huoDongKeyStr = string.Format("{0}_{1}_{2}", specPConditionConfig2.FromDate.ToString("yyyy-MM-dd HH:mm:ss"), specPConditionConfig2.ToDate.ToString("yyyy-MM-dd HH:mm:ss"), specPConditionConfig2.GroupID);
								long num = this.QueryHuodongAwardUserHist(this.SpecPriorityActivityType, huoDongKeyStr, "0");
								Dictionary<int, int> actConditionInfoDict = this.ActConditionInfoDict;
								KeyValuePair<int, int> kvp3 = kvp;
								actConditionInfoDict[kvp3.Key] = (int)num;
							}
						}
					}
				}
			}
		}

		private List<SpecPConditionConfig> CalSpecPConditionListByNow(DateTime now)
		{
			List<SpecPConditionConfig> result;
			lock (this.Mutex)
			{
				result = this.SpecPConditionList.FindAll((SpecPConditionConfig x) => x.FromDate <= now && now <= x.ToDate);
			}
			return result;
		}

		public void Update(DateTime now)
		{
			try
			{
				int offsetDay = TimeUtil.GetOffsetDay(now);
				if (offsetDay != this.LastUpdateDayID)
				{
					this.InitActivityConditionInfo(now, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "SpecPriorityActivityMgr.Update failed!", ex, true);
			}
		}

		private long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long result = 0L;
			string text = "";
			lock (this.Mutex)
			{
				KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out result, out text);
			}
			return result;
		}

		private int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			long hasgettimes = 0L;
			string lastgettime = "";
			int result = 0;
			lock (this.Mutex)
			{
				int awardHistoryForUser = KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out hasgettimes, out lastgettime);
				hasgettimes = (long)extTag;
				lastgettime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (awardHistoryForUser < 0)
				{
					result = KuaFuCopyDbMgr.Instance.AddHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, hasgettimes, lastgettime);
				}
				else
				{
					result = KuaFuCopyDbMgr.Instance.UpdateHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, hasgettimes, lastgettime);
				}
			}
			return result;
		}

		private static SpecPriorityActivityMgr _instance = new SpecPriorityActivityMgr();

		public object Mutex = new object();

		public List<SpecPConditionConfig> SpecPConditionList = new List<SpecPConditionConfig>();

		public Dictionary<int, int> ActConditionInfoDict = new Dictionary<int, int>();

		public int SpecPriorityActivityType = 49;

		private int LastUpdateDayID;
	}
}
