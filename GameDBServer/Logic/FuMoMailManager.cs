using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	internal class FuMoMailManager : IManager
	{
		public static FuMoMailManager getInstance()
		{
			return FuMoMailManager.instance;
		}

		public static string GetTodayGiveList(int rid, int time)
		{
			Dictionary<int, FuMoMailTemp> dictionary = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(time, out dictionary))
			{
				FuMoMailTemp fuMoMailTemp = null;
				if (dictionary.TryGetValue(rid, out fuMoMailTemp))
				{
					return fuMoMailTemp.ReceiverRID;
				}
			}
			return null;
		}

		public static void LoadCurrUserFuMoMailList()
		{
			foreach (KeyValuePair<int, FuMoMailData> keyValuePair in FuMoMailManager.FuMoMailDatas)
			{
				if (FuMoMailManager.CurrUserMailDatas.ContainsKey(keyValuePair.Value.ReceiverRID))
				{
					FuMoMailManager.CurrUserMailDatas[keyValuePair.Value.ReceiverRID].Add(keyValuePair.Value);
				}
				else
				{
					List<FuMoMailData> list = new List<FuMoMailData>();
					list.Add(keyValuePair.Value);
					FuMoMailManager.CurrUserMailDatas.Add(keyValuePair.Value.ReceiverRID, list);
				}
				FuMoMailManager.CurrUserMailDatas[keyValuePair.Value.ReceiverRID].Sort((FuMoMailData left, FuMoMailData right) => left.SendTime.CompareTo(right.SendTime));
			}
			FuMoMailManager.FuMoMailDatas.Clear();
		}

		public Dictionary<int, List<FuMoMailData>> GetFuMoMailItemDataListFromCached(int rid)
		{
			Dictionary<int, List<FuMoMailData>> dictionary = new Dictionary<int, List<FuMoMailData>>();
			List<FuMoMailData> value = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out value))
			{
				dictionary.Add(rid, value);
			}
			return dictionary;
		}

		public Dictionary<int, FuMoMailTemp> GetFuMoTempDataListFromCached(int nDate, int rid)
		{
			Dictionary<int, FuMoMailTemp> dictionary = null;
			FuMoMailTemp fuMoMailTemp = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out dictionary))
			{
				if (dictionary.TryGetValue(rid, out fuMoMailTemp))
				{
					return dictionary;
				}
			}
			return null;
		}

		public int GetFuMoTempDataAcceptFromCached(int nDate, int rid)
		{
			Dictionary<int, FuMoMailTemp> dictionary = null;
			FuMoMailTemp fuMoMailTemp = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out dictionary))
			{
				if (dictionary.TryGetValue(rid, out fuMoMailTemp))
				{
					return fuMoMailTemp.Accept;
				}
			}
			return -1;
		}

		public bool UpdataReadStateCached(int rid, int mailid, string today)
		{
			List<FuMoMailData> list = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out list))
			{
				foreach (FuMoMailData fuMoMailData in list)
				{
					if (fuMoMailData.MaillID == mailid)
					{
						fuMoMailData.IsRead = 1;
						fuMoMailData.ReadTime = today;
						return true;
					}
				}
			}
			return false;
		}

		public bool UpdataRemoveMailListCached(string[] mailidList, int rid)
		{
			List<FuMoMailData> list = null;
			bool result;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out list))
			{
				List<FuMoMailData> list2 = new List<FuMoMailData>();
				list2.AddRange(list);
				foreach (string text in mailidList)
				{
					if (!(text == ""))
					{
						foreach (FuMoMailData fuMoMailData in list)
						{
							if (Convert.ToInt32(text) == fuMoMailData.MaillID && !list2.Remove(fuMoMailData))
							{
								return false;
							}
						}
					}
				}
				FuMoMailManager.CurrUserMailDatas[rid] = list2;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool UpdataRemoveMailListCached(int mailid, int rid)
		{
			List<FuMoMailData> list = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out list))
			{
				foreach (FuMoMailData fuMoMailData in list)
				{
					if (fuMoMailData.MaillID == mailid)
					{
						if (FuMoMailManager.CurrUserMailDatas[rid].Remove(fuMoMailData))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool UpdateGiveAndListCached(int roleid, int give, int nDate, string recid_list)
		{
			Dictionary<int, FuMoMailTemp> dictionary = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out dictionary))
			{
				FuMoMailTemp fuMoMailTemp = null;
				if (dictionary.TryGetValue(roleid, out fuMoMailTemp))
				{
					FuMoMailManager.FuMoMailTemps[nDate][roleid].Give = give;
					FuMoMailManager.FuMoMailTemps[nDate][roleid].ReceiverRID = recid_list;
					return true;
				}
			}
			return false;
		}

		public bool UpdateAcceptCached(int roleid, int accept, int nDate)
		{
			Dictionary<int, FuMoMailTemp> dictionary = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out dictionary))
			{
				FuMoMailTemp fuMoMailTemp = null;
				if (dictionary.TryGetValue(roleid, out fuMoMailTemp))
				{
					FuMoMailManager.FuMoMailTemps[nDate][roleid].Accept = accept;
					return true;
				}
			}
			return false;
		}

		public bool InsertAcceptMapCached(int sendid, string recrid_list, int nDate, int accept, int give)
		{
			Dictionary<int, FuMoMailTemp> dictionary = new Dictionary<int, FuMoMailTemp>();
			Dictionary<int, FuMoMailTemp> dictionary2 = null;
			FuMoMailTemp fuMoMailTemp = new FuMoMailTemp();
			fuMoMailTemp.SenderRID = sendid;
			fuMoMailTemp.TodayID = nDate;
			fuMoMailTemp.Give = give;
			fuMoMailTemp.ReceiverRID = recrid_list;
			fuMoMailTemp.Accept = accept;
			bool result;
			if (!FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out dictionary2))
			{
				dictionary.Add(sendid, fuMoMailTemp);
				FuMoMailManager.FuMoMailTemps.Add(nDate, dictionary);
				result = true;
			}
			else if (dictionary2.ContainsKey(sendid))
			{
				result = false;
			}
			else
			{
				FuMoMailManager.FuMoMailTemps[nDate].Add(sendid, fuMoMailTemp);
				result = true;
			}
			return result;
		}

		public bool InsertFuMoMailCached(DBManager dbMgr, int sendid, string sendname, int sendjob, int recid, int num, string content, string today)
		{
			FuMoMailData fuMoMailData = new FuMoMailData
			{
				MaillID = FuMoMailManager.MaxMailID + 1,
				SenderRID = sendid,
				SenderRName = sendname,
				SenderJob = sendjob,
				SendTime = today,
				ReceiverRID = recid,
				IsRead = 0,
				ReadTime = "2000-11-11 11:11:11",
				FuMoMoney = num,
				Content = content
			};
			bool result;
			if (fuMoMailData == null)
			{
				result = false;
			}
			else
			{
				FuMoMailManager.MaxMailID = DBQuery.GetMailMaxIDFromTable(dbMgr);
				List<FuMoMailData> list = null;
				if (FuMoMailManager.CurrUserMailDatas.TryGetValue(recid, out list))
				{
					FuMoMailManager.CurrUserMailDatas[recid].Add(fuMoMailData);
					FuMoMailManager.CurrUserMailDatas[recid].Sort((FuMoMailData left, FuMoMailData right) => left.SendTime.CompareTo(right.SendTime));
					result = true;
				}
				else
				{
					list = new List<FuMoMailData>();
					list.Add(fuMoMailData);
					FuMoMailManager.CurrUserMailDatas.Add(recid, list);
					result = true;
				}
			}
			return result;
		}

		public int MaxLimitContorl(int rid)
		{
			List<FuMoMailData> list;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out list))
			{
				if (list.Count > 50)
				{
					return list.Count - 50;
				}
			}
			return 0;
		}

		public string MakeDelListSQL(string[] mailidList)
		{
			string text = null;
			int num = 0;
			foreach (string text2 in mailidList)
			{
				string text3 = string.Format(" maillid={0} ", text2);
				if (text2 == "")
				{
					break;
				}
				num++;
				if (num == 1)
				{
					text += text3;
				}
				else
				{
					text = text + " OR " + text3;
				}
			}
			return text;
		}

		public int DelFuMoMailFromLimitContorl(DBManager dbMgr, int roleid, int num)
		{
			int result;
			if (num > 0)
			{
				List<FuMoMailData> list = null;
				if (FuMoMailManager.CurrUserMailDatas.TryGetValue(roleid, out list))
				{
					int num2 = 0;
					string text = null;
					List<FuMoMailData> list2 = new List<FuMoMailData>(list);
					foreach (FuMoMailData fuMoMailData in list)
					{
						num2++;
						if (num2 > num)
						{
							break;
						}
						if (!list2.Remove(fuMoMailData))
						{
							return -1;
						}
						text = string.Format("{0}_{1}", fuMoMailData.MaillID, text);
					}
					FuMoMailManager.CurrUserMailDatas[roleid] = list2;
					if (text == null)
					{
						return 0;
					}
					string mailIDList = this.MakeDelListSQL(text.Split(new char[]
					{
						'_'
					}));
					if (DBWriter.DeleteMailFuMoByMailIDList(dbMgr, roleid, mailIDList))
					{
						return 1;
					}
				}
				result = -1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public void LoadFuMoInfoFromDB(DBManager dbMgr)
		{
			FuMoMailManager.FuMoMailDatas = DBQuery.GetFuMoMailCached(dbMgr);
			FuMoMailManager.FuMoMailTemps = DBQuery.GetFuMoMailTempCached(dbMgr);
			FuMoMailManager.MaxMailID = DBQuery.GetMailMaxIDFromTable(dbMgr);
			FuMoMailManager.LoadCurrUserFuMoMailList();
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public static int MaxMailID = 0;

		public static Dictionary<int, FuMoMailData> FuMoMailDatas = new Dictionary<int, FuMoMailData>();

		public static Dictionary<int, Dictionary<int, FuMoMailTemp>> FuMoMailTemps = new Dictionary<int, Dictionary<int, FuMoMailTemp>>();

		public static Dictionary<int, List<FuMoMailData>> CurrUserMailDatas = new Dictionary<int, List<FuMoMailData>>();

		private static FuMoMailManager instance = new FuMoMailManager();
	}
}
