using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	internal class KFBoCaiCaiDaXiao : BocaiBase
	{
		public static KFBoCaiCaiDaXiao GetInstance()
		{
			return KFBoCaiCaiDaXiao.instance;
		}

		private KFBoCaiCaiDaXiao()
		{
			this.StopBuyTime = 240;
			this.BoCaiType = 1;
			this.SelectOpenHisttory10 = " ORDER BY `DataPeriods` DESC LIMIT 10";
		}

		private void InitConfig()
		{
			try
			{
				CaiDaXiaoConfig caiDaXiaoConfig = KFBoCaiConfigManager.GetCaiDaXiaoConfig();
				if (null != caiDaXiaoConfig)
				{
					this.Config = new CaiDaXiaoConfig();
					this.Config.ID = caiDaXiaoConfig.ID;
					this.Config.HuoDongKaiQi = caiDaXiaoConfig.HuoDongKaiQi;
					this.Config.HuoDongJieSu = caiDaXiaoConfig.HuoDongJieSu;
					this.Config.MeiRiKaiQi = caiDaXiaoConfig.MeiRiKaiQi;
					this.Config.MeiRiJieSu = caiDaXiaoConfig.MeiRiJieSu;
					this.Config.ZhuShuShangXian = caiDaXiaoConfig.ZhuShuShangXian;
					this.OpenData.XiaoHaoDaiBi = caiDaXiaoConfig.XiaoHaoDaiBi;
				}
				else
				{
					this.Config = null;
					LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小] KFBoCaiConfigManager.GetCaiShuZiConfig() == null", null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private long GetNowPeriods(DateTime _time)
		{
			return Convert.ToInt64(string.Format("{0}001", TimeUtil.DataTimeToString(_time, "yyMMdd")));
		}

		private bool StartBuy(long Periods, DateTime time)
		{
			this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
			this.OpenData.DataPeriods = Periods;
			this.OpenData.strWinNum = "";
			this.OpenData.WinInfo = "0,0,0";
			this.OpenData.BocaiType = this.BoCaiType;
			this.OpenData.SurplusBalance = 0L;
			this.OpenData.AllBalance = 0L;
			this.SetUpToDBOpenData();
			bool result;
			if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
			{
				LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小] 猜大小开始购买 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
				result = false;
			}
			else
			{
				this.InsertHistoryData();
				LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜大小] 猜大小开启新的一轮 Periods={0}", Periods), null, true);
				result = true;
			}
			return result;
		}

		private string SetWinInfo(string value, int buyNum)
		{
			try
			{
				lock (this.mutex)
				{
					int num = Convert.ToInt32(value);
					string[] array = this.OpenData.WinInfo.Split(new char[]
					{
						','
					});
					int num2 = Convert.ToInt32(array[0]);
					int num3 = Convert.ToInt32(array[1]);
					int num4 = Convert.ToInt32(array[2]);
					if (1 == num)
					{
						return string.Format("{0},{1},{2}", num2 + buyNum, num3, num4);
					}
					if (2 == num)
					{
						return string.Format("{0},{1},{2}", num2, num3 + buyNum, num4);
					}
					if (3 == num)
					{
						return string.Format("{0},{1},{2}", num2, num3, num4 + buyNum);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return "";
		}

		protected override void Init()
		{
			try
			{
				lock (this.mutex)
				{
					long nowPeriods = this.GetNowPeriods(TimeUtil.NowDateTime().AddMonths(-6));
					KFBoCaiDbManager.DelTableData("t_bocai_open_lottery", string.Format("BocaiType={1} AND DataPeriods < {0}", nowPeriods, this.BoCaiType));
					KFBoCaiDbManager.DelTableData("t_bocai_buy_history", string.Format("BocaiType={1} AND DataPeriods < {0}", nowPeriods, this.BoCaiType));
					this.InitConfig();
					KFBoCaiDbManager.SelectOpenLottery(this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
					List<KFBoCaoHistoryData> history = new List<KFBoCaoHistoryData>();
					KFBoCaiDbManager.LoadLotteryHistory(this.BoCaiType, out history, "LIMIT 50");
					this.addHistory(history);
					this.MaxPeriods = KFBoCaiDbManager.GetMaxPeriods(this.BoCaiType);
					if (this.MaxPeriods < 0L)
					{
						KFBoCaiDbManager.StopServer("[ljl_caidaxiao_猜大小] 猜大小 maxPeriods == -1");
					}
					else
					{
						if (null == this.Config)
						{
							LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小]猜大小配置文件错误", null, true);
						}
						this.Stage = 1;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
				KFBoCaiDbManager.StopServer("初始化 Exception");
			}
		}

		public override void UpData(bool reload = false)
		{
			try
			{
				lock (this.mutex)
				{
					if (this.Config == null || 0 == this.Stage)
					{
						if (reload)
						{
							this.InitConfig();
						}
					}
					else
					{
						DateTime dateTime = TimeUtil.NowDateTime();
						if (1 == this.Stage)
						{
							if (DateTime.Parse(this.Config.HuoDongJieSu) <= dateTime)
							{
								this.InitConfig();
								base.KFSendStageData();
								return;
							}
							if (DateTime.Parse(this.Config.HuoDongKaiQi) > dateTime)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.MeiRiKaiQi) > dateTime)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.MeiRiJieSu) <= dateTime.AddSeconds(5.0))
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
						}
						if (1 == this.Stage)
						{
							long num = this.GetNowPeriods(dateTime);
							if (this.MaxPeriods >= num)
							{
								num = this.MaxPeriods + 1L;
							}
							if (this.StartBuy(num, dateTime))
							{
								this.MaxPeriods = num;
								this.Stage = 2;
								this.CompensateRateTime = dateTime;
								base.KFSendPeriodsData();
								base.KFSendStageData();
							}
						}
						else if (2 == this.Stage && this.PeriodsStartTime.AddSeconds((double)this.StopBuyTime) <= dateTime)
						{
							this.Stage = 3;
							base.KFSendPeriodsData();
							base.KFSendStageData();
						}
						else if (2 == this.Stage && (dateTime - this.CompensateRateTime).TotalSeconds > 10.0)
						{
							this.CompensateRateTime = dateTime;
							base.KFSendPeriodsData();
						}
						else if (3 == this.Stage && this.PeriodsStartTime.AddSeconds(270.0) <= dateTime)
						{
							this.Stage = 4;
							base.KFSendStageData();
						}
						else if (5 == this.Stage && this.PeriodsStartTime.AddMilliseconds(299960.0) <= dateTime)
						{
							this.RoleBuyDict.Clear();
							this.InitConfig();
							this.Stage = 1;
							base.KFSendStageData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private static int SortHistory(KFBoCaoHistoryData d1, KFBoCaoHistoryData d2)
		{
			int result;
			if (d1.DataPeriods > d2.DataPeriods)
			{
				result = -1;
			}
			else if (d1.DataPeriods < d2.DataPeriods)
			{
				result = -1;
			}
			else if (d1.WinMoney > d2.WinMoney)
			{
				result = -1;
			}
			else if (d1.WinMoney < d2.WinMoney)
			{
				result = -1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private void addHistory(List<KFBoCaoHistoryData> History)
		{
			try
			{
				long longData = 0L;
				lock (this.mutex)
				{
					this.BoCaiWinHistoryList.AddRange(History);
					List<long> list = new List<long>();
					using (List<KFBoCaoHistoryData>.Enumerator enumerator = this.BoCaiWinHistoryList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KFBoCaoHistoryData item = enumerator.Current;
							if (list.Find((long x) => x == item.DataPeriods) <= 0L)
							{
								list.Add(item.DataPeriods);
							}
						}
					}
					if (list.Count > 10)
					{
						list.Sort();
						list.Reverse();
						longData = list[9];
						this.BoCaiWinHistoryList = this.BoCaiWinHistoryList.FindAll((KFBoCaoHistoryData x) => x.DataPeriods >= longData);
					}
					this.BoCaiWinHistoryList.Sort(new Comparison<KFBoCaoHistoryData>(KFBoCaiCaiDaXiao.SortHistory));
				}
				if (longData >= 1L)
				{
					if (!KFBoCaiDbManager.DelTableData("t_bocai_lottery_history", string.Format("DataPeriods < {0} AND `BocaiType`={1}", longData, this.BoCaiType)))
					{
						LogManager.WriteLog(2, string.Format("[ljl_caidaxiao_猜大小] DelTableData  t_bocai_lottery_history false DataPeriods {0}", longData), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private KFBoCaoHistoryData InsertHistoryData()
		{
			KFBoCaoHistoryData kfboCaoHistoryData = new KFBoCaoHistoryData();
			kfboCaoHistoryData.DataPeriods = this.OpenData.DataPeriods;
			kfboCaoHistoryData.RoleID = -1;
			kfboCaoHistoryData.ZoneID = -1;
			kfboCaoHistoryData.ServerID = -1;
			kfboCaoHistoryData.RoleName = "占位";
			kfboCaoHistoryData.BuyNum = -1;
			kfboCaoHistoryData.WinMoney = -1L;
			KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, kfboCaoHistoryData);
			return kfboCaoHistoryData;
		}

		private void SetUpToDBOpenData()
		{
			try
			{
				lock (this.mutex)
				{
					this.UpToDBOpenData.AllBalance = this.OpenData.AllBalance;
					this.UpToDBOpenData.DataPeriods = this.OpenData.DataPeriods;
					this.UpToDBOpenData.strWinNum = this.OpenData.strWinNum;
					this.UpToDBOpenData.BocaiType = this.OpenData.BocaiType;
					this.UpToDBOpenData.SurplusBalance = this.OpenData.SurplusBalance;
					this.UpToDBOpenData.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					this.UpToDBOpenData.WinInfo = this.OpenData.WinInfo;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public override void Thread()
		{
			try
			{
				lock (this.mutex)
				{
					if (this.Stage != null && !this.UpToDBOpenData.WinInfo.Equals(this.OpenData.WinInfo) && this.OpenData.DataPeriods > 1L)
					{
						this.SetUpToDBOpenData();
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							this.UpToDBOpenData.WinInfo = "";
						}
						else
						{
							this.InsertHistoryData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			if (4 == this.Stage)
			{
				try
				{
					List<KFBoCaoHistoryData> list = new List<KFBoCaoHistoryData>();
					lock (this.mutex)
					{
						int num = 0;
						List<int> list2 = new List<int>();
						for (int i = 0; i < 3; i++)
						{
							int randomNumber = Global.GetRandomNumber(1, 7);
							list2.Add(randomNumber);
							num += randomNumber;
						}
						this.OpenData.strWinNum = KFBoCaiDbManager.ListInt2String(list2);
						LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜大小]猜大小 {0},winNum={1}", this.OpenData.DataPeriods, this.OpenData.strWinNum), null, true);
						if (num > 3 && num < 11)
						{
							num = 1;
						}
						else if (num >= 11 && num < 18)
						{
							num = 3;
						}
						else
						{
							num = 2;
						}
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小] 猜大小 开始计算中奖了 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
							return;
						}
						double num2 = this.CompensateRate(num);
						foreach (List<KFBuyBocaiData> list3 in this.RoleBuyDict.Values)
						{
							foreach (KFBuyBocaiData kfbuyBocaiData in list3)
							{
								if (num == Convert.ToInt32(kfbuyBocaiData.BuyValue))
								{
									list.Add(new KFBoCaoHistoryData
									{
										DataPeriods = this.OpenData.DataPeriods,
										RoleID = kfbuyBocaiData.RoleID,
										ZoneID = kfbuyBocaiData.ZoneID,
										ServerID = kfbuyBocaiData.ServerID,
										RoleName = kfbuyBocaiData.RoleName,
										BuyNum = kfbuyBocaiData.BuyNum,
										WinMoney = (long)((int)(num2 * (double)kfbuyBocaiData.BuyNum))
									});
								}
							}
						}
					}
					if (list.Count > 5)
					{
						list.Sort(new Comparison<KFBoCaoHistoryData>(KFBoCaiCaiDaXiao.SortHistory));
					}
					list = list.GetRange(0, Math.Min(5, list.Count));
					foreach (KFBoCaoHistoryData kfboCaoHistoryData in list)
					{
						if (!KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, kfboCaoHistoryData))
						{
							LogManager.WriteLog(2, string.Format("[ljl_caidaxiao_猜大小]猜大小插入中奖历史 false DataPeriods ={0}", kfboCaoHistoryData.DataPeriods), null, true);
						}
					}
					if (list.Count < 1)
					{
						list.Add(this.InsertHistoryData());
					}
					this.addHistory(list);
					base.SetOpenHistory(this.GetOpenLottery());
					this.Stage = 5;
					base.KFSendPeriodsData();
					base.KFSendStageData();
				}
				catch (Exception ex)
				{
					this.Stage = 5;
					LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
				}
			}
		}

		public double CompensateRate(DiceValueEnum val)
		{
			try
			{
				lock (this.mutex)
				{
					long num = Convert.ToInt64(this.OpenData.WinInfo.Split(new char[]
					{
						','
					})[val - 1]);
					if (this.OpenData.AllBalance > 0L || num > 0L)
					{
						return Math.Truncate(100.0 * (double)this.OpenData.AllBalance / (double)num) / 100.0;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return 1.0;
		}

		public override OpenLottery GetOpenLottery()
		{
			OpenLottery result;
			lock (this.mutex)
			{
				result = new OpenLottery
				{
					DataPeriods = this.OpenData.DataPeriods,
					strWinNum = this.OpenData.strWinNum,
					BocaiType = this.OpenData.BocaiType,
					SurplusBalance = this.OpenData.SurplusBalance,
					AllBalance = this.OpenData.AllBalance,
					XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi,
					WinInfo = this.OpenData.WinInfo,
					IsAward = false
				};
			}
			return result;
		}

		public override KFStageData GetKFStageData()
		{
			KFStageData result;
			lock (this.mutex)
			{
				KFStageData kfstageData = new KFStageData();
				kfstageData.Stage = this.Stage;
				kfstageData.isOpen = false;
				kfstageData.OpenTime = -1L;
				kfstageData.isOpenDay = false;
				kfstageData.LastOpenTime = -1L;
				kfstageData.BoCaiType = this.BoCaiType;
				if (this.Config == null)
				{
					result = kfstageData;
				}
				else
				{
					if (kfstageData.Stage > 1)
					{
						kfstageData.isOpen = true;
						kfstageData.isOpenDay = true;
					}
					else
					{
						kfstageData.isOpen = (DateTime.Parse(this.Config.HuoDongKaiQi) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.HuoDongJieSu) >= TimeUtil.NowDateTime());
						kfstageData.isOpenDay = (DateTime.Parse(this.Config.MeiRiKaiQi) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.MeiRiJieSu) >= TimeUtil.NowDateTime() && kfstageData.isOpen);
					}
					if (!kfstageData.isOpen)
					{
						kfstageData.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.HuoDongKaiQi), TimeUtil.NowDateTime(), false);
					}
					else if (kfstageData.isOpenDay)
					{
						kfstageData.OpenTime = base.GetDiffTime(this.PeriodsStartTime.AddSeconds(270.0), TimeUtil.NowDateTime(), true);
						kfstageData.LastOpenTime = this.GetLastTime(kfstageData.Stage);
					}
					else if (DateTime.Parse(this.Config.MeiRiKaiQi) >= TimeUtil.NowDateTime())
					{
						kfstageData.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.MeiRiKaiQi), TimeUtil.NowDateTime(), false);
					}
					else if (DateTime.Parse(this.Config.MeiRiJieSu) < TimeUtil.NowDateTime())
					{
						kfstageData.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.MeiRiKaiQi).AddDays(1.0), TimeUtil.NowDateTime(), false);
					}
					result = kfstageData;
				}
			}
			return result;
		}

		private long GetLastTime(BoCaiStageEnum stage)
		{
			try
			{
				if (null == this.Config)
				{
					return -1L;
				}
				DateTime d = TimeUtil.NowDateTime();
				if (2 == stage)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds((double)this.StopBuyTime), d, true);
				}
				if (3 == stage)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds(270.0), d, true);
				}
				if (stage >= 4)
				{
					return base.GetDiffTime(this.PeriodsStartTime.AddSeconds(300.0), d, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
			return 0L;
		}

		public List<KFBoCaoHistoryData> GetWinHistory()
		{
			List<KFBoCaoHistoryData> result;
			lock (this.mutex)
			{
				List<KFBoCaoHistoryData> list = new List<KFBoCaoHistoryData>();
				foreach (KFBoCaoHistoryData kfboCaoHistoryData in this.BoCaiWinHistoryList)
				{
					KFBoCaoHistoryData kfboCaoHistoryData2 = new KFBoCaoHistoryData();
					kfboCaoHistoryData2.RoleID = kfboCaoHistoryData.RoleID;
					kfboCaoHistoryData2.ZoneID = kfboCaoHistoryData.ZoneID;
					kfboCaoHistoryData2.ServerID = kfboCaoHistoryData.ServerID;
					kfboCaoHistoryData2.RoleName = kfboCaoHistoryData.RoleName;
					kfboCaoHistoryData2.BuyNum = kfboCaoHistoryData.BuyNum;
					kfboCaoHistoryData2.WinNo = kfboCaoHistoryData.WinNo;
					kfboCaoHistoryData2.WinMoney = kfboCaoHistoryData.WinMoney;
					kfboCaoHistoryData2.DataPeriods = kfboCaoHistoryData.DataPeriods;
					list.Add(kfboCaoHistoryData);
				}
				result = list;
			}
			return result;
		}

		public bool IsCanBuy(string buyValue, int buyNum, long DataPeriods)
		{
			bool result;
			if (this.Stage != 2 || DataPeriods != this.OpenData.DataPeriods)
			{
				result = false;
			}
			else if (buyNum > this.Config.ZhuShuShangXian)
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(buyValue);
				result = (1 <= num && num <= 3);
			}
			return result;
		}

		public bool BuyBoCai(KFBuyBocaiData data)
		{
			bool result;
			lock (this.mutex)
			{
				string text = this.SetWinInfo(data.BuyValue, data.BuyNum);
				if (string.IsNullOrEmpty(text))
				{
					result = false;
				}
				else
				{
					bool flag2 = true;
					List<KFBuyBocaiData> list;
					if (this.RoleBuyDict.TryGetValue(data.GetKey(), out list))
					{
						KFBuyBocaiData kfbuyBocaiData = list.Find((KFBuyBocaiData x) => x.BuyValue.Equals(data.BuyValue));
						if (kfbuyBocaiData == null)
						{
							if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
							{
								this.OpenData.WinInfo = text;
								list.Add(data);
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							data.BuyNum += kfbuyBocaiData.BuyNum;
							if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
							{
								this.OpenData.WinInfo = text;
								kfbuyBocaiData.BuyNum = data.BuyNum;
							}
							else
							{
								flag2 = false;
							}
						}
					}
					else if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
					{
						list = new List<KFBuyBocaiData>();
						list.Add(data);
						this.OpenData.WinInfo = text;
						this.RoleBuyDict.Add(data.GetKey(), list);
					}
					else
					{
						flag2 = false;
					}
					result = flag2;
				}
			}
			return result;
		}

		private const int RankNum = 5;

		private const int LastTime = 300;

		private const int OpenTime = 270;

		private const int UpCompensateRateTime = 10;

		private DateTime CompensateRateTime;

		private CaiDaXiaoConfig Config = null;

		private static KFBoCaiCaiDaXiao instance = new KFBoCaiCaiDaXiao();
	}
}
