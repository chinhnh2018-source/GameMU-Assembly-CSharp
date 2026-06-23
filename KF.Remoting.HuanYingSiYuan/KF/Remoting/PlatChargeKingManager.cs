using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Data;
using Tmsk.Tools;

namespace KF.Remoting
{
	internal class PlatChargeKingManager
	{
		public void Update()
		{
			try
			{
				lock (this.Mutex)
				{
					if (this.IsNeedDownload())
					{
						bool flag2 = false;
						List<InputKingPaiHangDataEx> list = new List<InputKingPaiHangDataEx>();
						if (KuaFuServerManager.GetPlatChargeKingUrl != null)
						{
							for (int i = 0; i < KuaFuServerManager.GetPlatChargeKingUrl.Length; i++)
							{
								ClientServerListData clientServerListData = new ClientServerListData();
								clientServerListData.lTime = TimeUtil.NOW();
								clientServerListData.strMD5 = MD5Helper.get_md5_string(ConstData.HTTP_MD5_KEY + clientServerListData.lTime.ToString());
								byte[] array = DataHelper2.ObjectToBytes<ClientServerListData>(clientServerListData);
								byte[] array2 = WebHelper.RequestByPost(KuaFuServerManager.GetPlatChargeKingUrl[i], array, 2000, 30000);
								if (array2 == null)
								{
									flag2 = true;
									break;
								}
								InputKingPaiHangDataEx inputKingPaiHangDataEx = DataHelper2.BytesToObject<InputKingPaiHangDataEx>(array2, 0, array2.Length);
								if (inputKingPaiHangDataEx == null)
								{
									flag2 = true;
									break;
								}
								list.Add(inputKingPaiHangDataEx);
							}
							if (flag2)
							{
								this.rankEx = new InputKingPaiHangDataEx();
							}
							else
							{
								this.rankEx = this.MergePlatfromInputKingList(list);
							}
						}
					}
					if (this.IsNeedDownloadEveryDay())
					{
						if (KuaFuServerManager.GetPlatChargeKingUrl_EveryDay != null)
						{
							bool flag2 = false;
							Dictionary<int, List<InputKingPaiHangDataEx>> dictionary = new Dictionary<int, List<InputKingPaiHangDataEx>>();
							for (int i = 0; i < KuaFuServerManager.GetPlatChargeKingUrl_EveryDay.Length; i++)
							{
								List<InputKingPaiHangDataEx> list2 = new List<InputKingPaiHangDataEx>();
								if (this.MeiRiPCKingFromDate < this.MeiRiPCKingToDate)
								{
									DateTime t = this.MeiRiPCKingFromDate;
									while (t < this.MeiRiPCKingToDate && t < TimeUtil.NowDateTime())
									{
										InputKingPaiHangDataEx inputKingPaiHangDataEx = null;
										byte[] array = DataHelper2.ObjectToBytes<InputKingPaiHangDataSearch>(new InputKingPaiHangDataSearch
										{
											startDate = t.ToString("yyyy-MM-dd HH:mm:ss"),
											endDate = t.AddDays(1.0).AddSeconds(-1.0).ToString("yyyy-MM-dd HH:mm:ss")
										});
										byte[] array2 = WebHelper.RequestByPost(KuaFuServerManager.GetPlatChargeKingUrl_EveryDay[i], array, 2000, 30000);
										if (array2 != null)
										{
											inputKingPaiHangDataEx = DataHelper2.BytesToObject<InputKingPaiHangDataEx>(array2, 0, array2.Length);
										}
										if (null != inputKingPaiHangDataEx)
										{
											list2.Add(inputKingPaiHangDataEx);
										}
										else
										{
											list2.Add(new InputKingPaiHangDataEx());
											flag2 = true;
										}
										if (flag2)
										{
											break;
										}
										t = t.AddDays(1.0);
									}
									if (flag2)
									{
										break;
									}
									if (!dictionary.ContainsKey(i))
									{
										dictionary.Add(i, list2);
									}
								}
							}
							if (flag2)
							{
								this.rankExList = new List<InputKingPaiHangDataEx>();
							}
							else
							{
								this.rankExList = this.MergePlatfromInputKingListEveryDay(dictionary);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "PlatChargeKingManager.Update exception", ex, true);
			}
		}

		public InputKingPaiHangDataEx GetRankEx()
		{
			InputKingPaiHangDataEx result = null;
			lock (this.Mutex)
			{
				this.bHasVisitor = true;
				result = this.rankEx;
			}
			return result;
		}

		public List<InputKingPaiHangDataEx> GetRankExList(DateTime fromDate, DateTime toDate)
		{
			List<InputKingPaiHangDataEx> list = null;
			List<InputKingPaiHangDataEx> result;
			if (fromDate >= toDate || fromDate < this.MeiRiPCKingFromDate || toDate < this.MeiRiPCKingToDate)
			{
				result = list;
			}
			else
			{
				lock (this.Mutex)
				{
					this.MeiRiPCKingFromDate = fromDate;
					this.MeiRiPCKingToDate = toDate;
					this.bHasVisitorEvery = true;
					list = this.rankExList;
				}
				result = list;
			}
			return result;
		}

		public InputKingPaiHangDataEx MergePlatfromInputKingList(List<InputKingPaiHangDataEx> EveryPlatfrom)
		{
			InputKingPaiHangDataEx inputKingPaiHangDataEx = new InputKingPaiHangDataEx();
			InputKingPaiHangDataEx result;
			if (EveryPlatfrom == null)
			{
				result = inputKingPaiHangDataEx;
			}
			else
			{
				bool flag = false;
				string rankTime = "";
				string startTime = "";
				string endTime = "";
				List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
				foreach (InputKingPaiHangDataEx inputKingPaiHangDataEx2 in EveryPlatfrom)
				{
					if (inputKingPaiHangDataEx2 != null)
					{
						if (inputKingPaiHangDataEx2.ListData != null)
						{
							if (!flag && !string.IsNullOrEmpty(inputKingPaiHangDataEx2.RankTime) && !string.IsNullOrEmpty(inputKingPaiHangDataEx2.StartTime) && !string.IsNullOrEmpty(inputKingPaiHangDataEx2.EndTime))
							{
								rankTime = inputKingPaiHangDataEx2.RankTime;
								startTime = inputKingPaiHangDataEx2.StartTime;
								endTime = inputKingPaiHangDataEx2.EndTime;
								flag = true;
							}
							foreach (InputKingPaiHangData inputKingPaiHangData in inputKingPaiHangDataEx2.ListData)
							{
								if (inputKingPaiHangData != null)
								{
									list.Add(inputKingPaiHangData);
								}
							}
						}
					}
				}
				list.Sort(delegate(InputKingPaiHangData _left, InputKingPaiHangData _right)
				{
					int result2;
					if (_left.PaiHangValue > _right.PaiHangValue)
					{
						result2 = -1;
					}
					else if (_left.PaiHangValue < _right.PaiHangValue)
					{
						result2 = 1;
					}
					else if (!string.IsNullOrEmpty(_left.InputTime))
					{
						int num = _left.InputTime.CompareTo(_right.InputTime);
						if (0 == num)
						{
							if (!string.IsNullOrEmpty(_left.UserID))
							{
								result2 = _left.UserID.CompareTo(_right.UserID);
							}
							else
							{
								result2 = 1;
							}
						}
						else
						{
							result2 = num;
						}
					}
					else if (!string.IsNullOrEmpty(_left.UserID))
					{
						result2 = _left.UserID.CompareTo(_right.UserID);
					}
					else
					{
						result2 = 1;
					}
					return result2;
				});
				inputKingPaiHangDataEx.ListData = list;
				inputKingPaiHangDataEx.RankTime = rankTime;
				inputKingPaiHangDataEx.StartTime = startTime;
				inputKingPaiHangDataEx.EndTime = endTime;
				result = inputKingPaiHangDataEx;
			}
			return result;
		}

		public List<InputKingPaiHangDataEx> MergePlatfromInputKingListEveryDay(Dictionary<int, List<InputKingPaiHangDataEx>> EveryPlatfrom)
		{
			List<InputKingPaiHangDataEx> list = new List<InputKingPaiHangDataEx>();
			List<InputKingPaiHangDataEx> result;
			if (EveryPlatfrom == null)
			{
				result = list;
			}
			else
			{
				Dictionary<int, List<InputKingPaiHangDataEx>> dictionary = new Dictionary<int, List<InputKingPaiHangDataEx>>();
				int num = 0;
				foreach (List<InputKingPaiHangDataEx> list2 in EveryPlatfrom.Values)
				{
					foreach (InputKingPaiHangDataEx item in list2)
					{
						num++;
						if (dictionary.ContainsKey(num))
						{
							dictionary[num].Add(item);
						}
						else
						{
							dictionary.Add(num, new List<InputKingPaiHangDataEx>
							{
								item
							});
						}
					}
					num = 0;
				}
				foreach (KeyValuePair<int, List<InputKingPaiHangDataEx>> keyValuePair in dictionary)
				{
					InputKingPaiHangDataEx inputKingPaiHangDataEx = this.MergePlatfromInputKingList(keyValuePair.Value);
					if (inputKingPaiHangDataEx != null)
					{
						list.Add(inputKingPaiHangDataEx);
					}
					else
					{
						list.Add(new InputKingPaiHangDataEx());
					}
				}
				result = list;
			}
			return result;
		}

		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long result = 0L;
			string text = "";
			lock (this.Mutex)
			{
				KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out result, out text);
			}
			return result;
		}

		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			long num = 0L;
			string lastgettime = "";
			int result = 0;
			lock (this.Mutex)
			{
				int awardHistoryForUser = KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out num, out lastgettime);
				num |= 1L << extTag - 1;
				lastgettime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (awardHistoryForUser < 0)
				{
					result = KuaFuCopyDbMgr.Instance.AddHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, num, lastgettime);
				}
				else
				{
					result = KuaFuCopyDbMgr.Instance.UpdateHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, num, lastgettime);
				}
			}
			return result;
		}

		private bool IsNeedDownload()
		{
			return this.bHasVisitor;
		}

		private bool IsNeedDownloadEveryDay()
		{
			return this.bHasVisitorEvery;
		}

		private object Mutex = new object();

		private InputKingPaiHangDataEx rankEx = null;

		private bool bHasVisitor = false;

		private DateTime MeiRiPCKingFromDate = DateTime.MinValue;

		private DateTime MeiRiPCKingToDate = DateTime.MinValue;

		private List<InputKingPaiHangDataEx> rankExList = new List<InputKingPaiHangDataEx>();

		private bool bHasVisitorEvery = false;
	}
}
