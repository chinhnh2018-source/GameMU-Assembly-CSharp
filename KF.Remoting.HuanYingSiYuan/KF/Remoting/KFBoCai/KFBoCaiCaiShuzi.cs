using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	public class KFBoCaiCaiShuzi : BocaiBase
	{
		public static KFBoCaiCaiShuzi GetInstance()
		{
			return KFBoCaiCaiShuzi.instance;
		}

		private KFBoCaiCaiShuzi()
		{
			this.StopBuyTime = 1800;
			this.BoCaiType = 2;
		}

		private void InitConfig()
		{
			try
			{
				CaiShuZiConfig caiShuZiConfig = KFBoCaiConfigManager.GetCaiShuZiConfig();
				if (null != caiShuZiConfig)
				{
					this.Config = new CaiShuZiConfig();
					this.Config.ID = caiShuZiConfig.ID;
					this.Config.XiTongChouCheng = caiShuZiConfig.XiTongChouCheng;
					this.Config.BuChongTiaoJian = caiShuZiConfig.BuChongTiaoJian;
					this.Config.KaiQiShiJian = caiShuZiConfig.KaiQiShiJian;
					this.Config.JieShuShiJian = caiShuZiConfig.JieShuShiJian;
					this.Config.XiaoHaoDaiBi = caiShuZiConfig.XiaoHaoDaiBi;
					this.Config.KaiJiangShiJian = caiShuZiConfig.KaiJiangShiJian;
					this.Config.ChuFaBiZhong = caiShuZiConfig.ChuFaBiZhong;
					this.Config.AnNiuList = new List<CaiShuZiAnNiu>();
					foreach (CaiShuZiAnNiu caiShuZiAnNiu in caiShuZiConfig.AnNiuList)
					{
						CaiShuZiAnNiu caiShuZiAnNiu2 = new CaiShuZiAnNiu();
						caiShuZiAnNiu2.NO = caiShuZiAnNiu.NO;
						caiShuZiAnNiu2.Percent = caiShuZiAnNiu.Percent;
						this.Config.AnNiuList.Add(caiShuZiAnNiu2);
					}
					this.OpenData.AllBalance = Math.Max(this.OpenData.AllBalance, (long)this.Config.BuChongTiaoJian);
					this.OpenData.XiaoHaoDaiBi = this.Config.XiaoHaoDaiBi;
				}
				else
				{
					this.Config = null;
					LogManager.WriteLog(2, "[ljl_CaiShuZi_猜数字] KFBoCaiConfigManager.GetCaiShuZiConfig() == null", null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		private void StartServerSamePeriods(DateTime time)
		{
			try
			{
				OpenLottery openLottery;
				KFBoCaiDbManager.SelectOpenLottery(this.MaxPeriods, this.BoCaiType, out openLottery);
				List<KFBuyBocaiData> list;
				if (null == openLottery)
				{
					KFBoCaiDbManager.StopServer(string.Format("[ljl_CaiShuZi_猜数字] 开奖记录读取失败 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType));
				}
				else if (!string.IsNullOrEmpty(openLottery.strWinNum))
				{
					if (openLottery.XiaoHaoDaiBi < 1)
					{
						openLottery.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					}
					this.OpenData = openLottery;
					this.SetUpToDBOpenData();
					this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
					this.Stage = 5;
					base.KFSendStageData();
					base.KFSendPeriodsData();
					LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 并且已经开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
				}
				else if (!KFBoCaiDbManager.LoadBuyHistory(this.BoCaiType, this.MaxPeriods, out list))
				{
					KFBoCaiDbManager.StopServer(string.Format("[ljl_CaiShuZi_猜数字]读取购买记录失败 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType));
				}
				else
				{
					this.RoleBuyDict = new Dictionary<string, List<KFBuyBocaiData>>();
					using (List<KFBuyBocaiData>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KFBuyBocaiData item = enumerator.Current;
							List<KFBuyBocaiData> list2;
							if (this.RoleBuyDict.TryGetValue(item.GetKey(), out list2))
							{
								KFBuyBocaiData kfbuyBocaiData = list2.Find((KFBuyBocaiData x) => x.BuyValue.Equals(item.BuyValue));
								if (kfbuyBocaiData == null)
								{
									list2.Add(item);
								}
								else
								{
									kfbuyBocaiData.BuyNum += item.BuyNum;
								}
							}
							else
							{
								list2 = new List<KFBuyBocaiData>();
								list2.Add(item);
								this.RoleBuyDict.Add(item.GetKey(), list2);
							}
						}
					}
					if (openLottery.XiaoHaoDaiBi < 1)
					{
						openLottery.XiaoHaoDaiBi = this.OpenData.XiaoHaoDaiBi;
					}
					this.OpenData = openLottery;
					this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
					this.SetUpToDBOpenData();
					if (DateTime.Parse(this.Config.KaiJiangShiJian) >= time)
					{
						this.Stage = 2;
						LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 并且没开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
					}
					else if ((DateTime.Parse("23:59:59") - time).TotalMinutes < 2.0)
					{
						this.Stage = 4;
						this.SetUpToDBOpenData();
						LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 状态设置开奖 &&强制开奖 不足2分钟 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
						this.Thread();
					}
					else
					{
						this.Stage = 4;
						LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字] 和上期是一期 状态设置开奖 BoCaiType={1},DataPeriods={0}", this.MaxPeriods, this.BoCaiType), null, true);
					}
					base.KFSendStageData();
					base.KFSendPeriodsData();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		private void GetOldBalance()
		{
			List<OpenLottery> list;
			KFBoCaiDbManager.SelectOpenLottery(this.BoCaiType, string.Format(" AND `strWinNum`!='{0}' ORDER BY `DataPeriods` DESC LIMIT 1;", ""), out list);
			if (null == list)
			{
				KFBoCaiDbManager.StopServer("找上期余额失败");
			}
			if (list.Count > 0)
			{
				this.OpenData.AllBalance = Math.Max(this.OpenData.AllBalance, list[0].SurplusBalance);
			}
		}

		protected override void Init()
		{
			try
			{
				lock (this.mutex)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					long nowPeriods = this.GetNowPeriods(dateTime.AddYears(-1));
					KFBoCaiDbManager.DelTableData("t_bocai_open_lottery", string.Format("BocaiType={1} AND DataPeriods < {0}", nowPeriods, this.BoCaiType));
					KFBoCaiDbManager.DelTableData("t_bocai_buy_history", string.Format("BocaiType={1} AND DataPeriods < {0}", nowPeriods, this.BoCaiType));
					this.InitConfig();
					KFBoCaiDbManager.LoadLotteryHistory(this.BoCaiType, out this.BoCaiWinHistoryList, "");
					KFBoCaiDbManager.SelectOpenLottery(this.BoCaiType, this.SelectOpenHisttory10, out this.OpenHistory);
					this.MaxPeriods = KFBoCaiDbManager.GetMaxPeriods(this.BoCaiType);
					if (this.MaxPeriods < 0L)
					{
						KFBoCaiDbManager.StopServer("[ljl_caidaxiao_猜数字] 猜数字 maxPeriods == -1");
					}
					else
					{
						if (null == this.Config)
						{
							LogManager.WriteLog(2, "[ljl_CaiShuZi_猜数字]猜数字配置文件错误", null, true);
						}
						else if (DateTime.Parse(this.Config.KaiQiShiJian) < dateTime)
						{
							long nowPeriods2 = this.GetNowPeriods(dateTime);
							if (this.MaxPeriods == nowPeriods2)
							{
								this.StartServerSamePeriods(dateTime);
								return;
							}
						}
						else
						{
							LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字] 未开启 开启时间 {0}", this.Config.KaiQiShiJian), null, true);
						}
						this.GetOldBalance();
						this.Stage = 1;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
				KFBoCaiDbManager.StopServer("初始化 Exception");
			}
		}

		private bool StartBuy(DateTime time, long Periods, long SurplusBalance = 0L)
		{
			this.PeriodsStartTime = DateTime.Parse(TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss"));
			this.OpenData.DataPeriods = Periods;
			this.OpenData.strWinNum = "";
			this.OpenData.WinInfo = "";
			this.OpenData.BocaiType = this.BoCaiType;
			this.OpenData.SurplusBalance = SurplusBalance;
			this.SetUpToDBOpenData();
			bool result;
			if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
			{
				LogManager.WriteLog(2, "[ljl_CaiShuZi_猜数字]KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
							if (DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)) <= dateTime)
							{
								return;
							}
							if (DateTime.Parse(this.Config.KaiQiShiJian) > dateTime)
							{
								if (reload)
								{
									this.InitConfig();
								}
								return;
							}
							if (DateTime.Parse(this.Config.JieShuShiJian) <= dateTime)
							{
								this.InitConfig();
								base.KFSendStageData();
								return;
							}
						}
						if (1 == this.Stage)
						{
							if (this.StartBuy(dateTime, this.GetNowPeriods(dateTime), 0L))
							{
								this.Stage = 2;
								base.KFSendPeriodsData();
								base.KFSendStageData();
								this.UpBalanceTime = dateTime;
							}
						}
						else if (2 == this.Stage && (DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)) <= dateTime || this.PeriodsStartTime.Day != dateTime.Day))
						{
							this.Stage = 3;
							base.KFSendPeriodsData();
							base.KFSendStageData();
						}
						else if (2 == this.Stage && (dateTime - this.UpBalanceTime).TotalSeconds > 600.0)
						{
							this.UpBalanceTime = dateTime;
							base.KFSendPeriodsData();
						}
						else if (3 == this.Stage && (DateTime.Parse(this.Config.KaiJiangShiJian) <= dateTime || this.PeriodsStartTime.Day != dateTime.Day))
						{
							this.Stage = 4;
							base.KFSendStageData();
						}
						else if (5 == this.Stage && this.PeriodsStartTime.Day != dateTime.Day)
						{
							this.RoleBuyDict.Clear();
							this.OpenData.AllBalance = this.OpenData.SurplusBalance;
							this.InitConfig();
							this.Stage = 1;
							base.KFSendStageData();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		private void GetWinRoleNum(List<int> value, out int no1Num, out int no2Num, out int no3Num, out List<KFBoCaoHistoryData> Hsitory)
		{
			no1Num = 0;
			no2Num = 0;
			no3Num = 0;
			Hsitory = new List<KFBoCaoHistoryData>();
			lock (this.mutex)
			{
				foreach (List<KFBuyBocaiData> list in this.RoleBuyDict.Values)
				{
					foreach (KFBuyBocaiData kfbuyBocaiData in list)
					{
						int num = 0;
						List<int> list2;
						KFBoCaiDbManager.String2ListInt(kfbuyBocaiData.BuyValue, out list2);
						if (list2.Count == value.Count)
						{
							for (int i = 0; i < value.Count; i++)
							{
								if (value[i] == list2[i])
								{
									num++;
								}
							}
							KFBoCaoHistoryData kfboCaoHistoryData = new KFBoCaoHistoryData();
							kfboCaoHistoryData.DataPeriods = this.OpenData.DataPeriods;
							kfboCaoHistoryData.RoleID = kfbuyBocaiData.RoleID;
							kfboCaoHistoryData.ZoneID = kfbuyBocaiData.ZoneID;
							kfboCaoHistoryData.ServerID = kfbuyBocaiData.ServerID;
							kfboCaoHistoryData.RoleName = kfbuyBocaiData.RoleName;
							kfboCaoHistoryData.BuyNum = kfbuyBocaiData.BuyNum;
							if (5 == num)
							{
								no1Num += kfbuyBocaiData.BuyNum;
								kfboCaoHistoryData.WinNo = 1;
							}
							else if (4 == num)
							{
								no2Num += kfbuyBocaiData.BuyNum;
								kfboCaoHistoryData.WinNo = 2;
							}
							else
							{
								if (3 != num)
								{
									continue;
								}
								no3Num += kfbuyBocaiData.BuyNum;
								kfboCaoHistoryData.WinNo = 3;
							}
							Hsitory.Add(kfboCaoHistoryData);
						}
					}
				}
			}
		}

		public override void Thread()
		{
			try
			{
				lock (this.mutex)
				{
					if (1 < this.Stage && this.UpToDBOpenData.AllBalance != this.OpenData.AllBalance && this.OpenData.DataPeriods > 1L)
					{
						this.SetUpToDBOpenData();
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							this.UpToDBOpenData.AllBalance = 0L;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			if (4 == this.Stage)
			{
				try
				{
					List<int> list = new List<int>();
					int num = 0;
					if ((long)this.Config.ChuFaBiZhong <= this.OpenData.AllBalance && this.RoleBuyDict.Count > 0)
					{
						int randomNumber = Global.GetRandomNumber(0, this.RoleBuyDict.Count);
						List<string> list2 = this.RoleBuyDict.Keys.ToList<string>();
						List<KFBuyBocaiData> list3 = this.RoleBuyDict[list2[randomNumber]];
						randomNumber = Global.GetRandomNumber(0, list3.Count);
						KFBoCaiDbManager.String2ListInt(list3[randomNumber].BuyValue, out list);
					}
					else
					{
						while (list.Count < 5)
						{
							list.Add(Global.GetRandomNumber(0, 10));
						}
					}
					int num2;
					int num3;
					List<KFBoCaoHistoryData> list4;
					this.GetWinRoleNum(list, out num2, out num3, out num, out list4);
					LogManager.WriteLog(0, string.Format("[ljl_CaiShuZi_猜数字]猜数1等奖人数={0}，二等奖={1}，3等奖={2}", num2, num3, num), null, true);
					long num4 = 0L;
					long num5 = 0L;
					long num6 = 0L;
					lock (this.mutex)
					{
						long num7 = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[0].Percent);
						long num8 = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[1].Percent);
						long num9 = (long)((double)this.OpenData.AllBalance * this.Config.AnNiuList[2].Percent);
						this.OpenData.SurplusBalance = this.OpenData.AllBalance;
						if (num2 > 0)
						{
							this.OpenData.SurplusBalance -= num7;
							num4 = num7 / (long)num2;
						}
						if (num3 > 0)
						{
							this.OpenData.SurplusBalance -= num8;
							num5 = num8 / (long)num3;
						}
						if (num > 0)
						{
							this.OpenData.SurplusBalance -= num9;
							num6 = num9 / (long)num;
						}
						this.OpenData.WinInfo = string.Format("{0},{1},{2}", num4, num5, num6);
						this.OpenData.strWinNum = KFBoCaiDbManager.ListInt2String(list);
						if (!KFBoCaiDbManager.InserOpenLottery(this.OpenData))
						{
							LogManager.WriteLog(2, "[ljl_CaiShuZi_猜数字]开始计算中奖了 KFBoCaiDbManager.InserOpenLottery(data) false", null, true);
							this.OpenData.SurplusBalance = 0L;
							this.OpenData.WinInfo = "";
							this.OpenData.strWinNum = "";
							return;
						}
						this.BoCaiWinHistoryList.Clear();
						this.BoCaiWinHistoryList.AddRange(list4);
						base.SetOpenHistory(this.GetOpenLottery());
						this.Stage = 5;
					}
					foreach (KFBoCaoHistoryData kfboCaoHistoryData in list4)
					{
						if (1 == kfboCaoHistoryData.WinNo)
						{
							kfboCaoHistoryData.WinMoney = (long)kfboCaoHistoryData.BuyNum * num4;
						}
						else if (2 == kfboCaoHistoryData.WinNo)
						{
							kfboCaoHistoryData.WinMoney = (long)kfboCaoHistoryData.BuyNum * num5;
						}
						else if (3 == kfboCaoHistoryData.WinNo)
						{
							kfboCaoHistoryData.WinMoney = (long)kfboCaoHistoryData.BuyNum * num6;
						}
						if (!KFBoCaiDbManager.InsertLotteryHistory(this.BoCaiType, kfboCaoHistoryData))
						{
							LogManager.WriteLog(2, string.Format("[ljl_CaiShuZi_猜数字]插入中奖历史 false DataPeriods ={0}, name={1},id={2},WinNo={3},WinMoney={4}", new object[]
							{
								kfboCaoHistoryData.DataPeriods,
								kfboCaoHistoryData.RoleName,
								kfboCaoHistoryData.RoleID,
								kfboCaoHistoryData.WinNo,
								kfboCaoHistoryData.WinMoney
							}), null, true);
						}
					}
					if (!KFBoCaiDbManager.DelTableData("t_bocai_lottery_history", string.Format("DataPeriods < {0}", this.OpenData.DataPeriods)))
					{
						LogManager.WriteLog(2, string.Format("[ljl_CaiShuZi_猜数字] DelTableData  t_bocai_lottery_history false DataPeriods ={0}", this.OpenData.DataPeriods), null, true);
					}
					base.KFSendPeriodsData();
					base.KFSendStageData();
				}
				catch (Exception ex)
				{
					this.Stage = 5;
					LogManager.WriteLog(9, string.Format("[ljl_CaiShuZi_猜数字]{0}", ex.ToString()), null, true);
				}
			}
		}

		private long GetNowPeriods(DateTime _time)
		{
			return Convert.ToInt64(string.Format("{0}1", TimeUtil.DataTimeToString(_time, "yyMMdd")));
		}

		public bool IsCanBuy(string buyValue, int buyNum, long DataPeriods)
		{
			bool result;
			if (this.Stage != 2 || DataPeriods != this.OpenData.DataPeriods)
			{
				result = false;
			}
			else
			{
				List<int> list = new List<int>();
				KFBoCaiDbManager.String2ListInt(buyValue, out list);
				result = (list.Count == 5);
			}
			return result;
		}

		public bool BuyBoCai(KFBuyBocaiData data)
		{
			bool result;
			lock (this.mutex)
			{
				bool flag2 = false;
				List<KFBuyBocaiData> list;
				if (this.RoleBuyDict.TryGetValue(data.GetKey(), out list))
				{
					KFBuyBocaiData kfbuyBocaiData = list.Find((KFBuyBocaiData x) => x.BuyValue.Equals(data.BuyValue));
					if (kfbuyBocaiData == null)
					{
						if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
						{
							list.Add(data);
							flag2 = true;
						}
					}
					else
					{
						data.BuyNum += kfbuyBocaiData.BuyNum;
						if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
						{
							kfbuyBocaiData.BuyNum = data.BuyNum;
							flag2 = true;
						}
					}
				}
				else if (KFBoCaiDbManager.InserBuyBocai(this.OpenData.DataPeriods, data))
				{
					list = new List<KFBuyBocaiData>();
					list.Add(data);
					this.RoleBuyDict.Add(data.GetKey(), list);
					flag2 = true;
				}
				if (flag2)
				{
					this.OpenData.AllBalance += (long)((double)(data.BuyNum * this.OpenData.XiaoHaoDaiBi) * (1.0 - this.Config.XiTongChouCheng));
				}
				result = flag2;
			}
			return result;
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
					kfstageData.isOpen = (DateTime.Parse(this.Config.KaiQiShiJian) <= TimeUtil.NowDateTime() && DateTime.Parse(this.Config.JieShuShiJian) >= TimeUtil.NowDateTime());
					kfstageData.isOpenDay = kfstageData.isOpen;
					if (kfstageData.isOpen)
					{
						kfstageData.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian), TimeUtil.NowDateTime(), true);
						kfstageData.LastOpenTime = this.GetLastTime(kfstageData.Stage);
					}
					else
					{
						kfstageData.OpenTime = base.GetDiffTime(DateTime.Parse(this.Config.KaiQiShiJian), TimeUtil.NowDateTime(), false);
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
					return base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian).AddSeconds((double)(-(double)this.StopBuyTime)), d, true);
				}
				if (3 == stage)
				{
					return base.GetDiffTime(DateTime.Parse(this.Config.KaiJiangShiJian), d, true);
				}
				if (4 <= stage)
				{
					if (this.PeriodsStartTime.Day == d.Day)
					{
						return base.GetDiffTime(DateTime.Parse("23:59:59"), d, true);
					}
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

		private const int UpAllBalanceTime = 600;

		private static KFBoCaiCaiShuzi instance = new KFBoCaiCaiShuzi();

		private CaiShuZiConfig Config = null;

		private DateTime UpBalanceTime;
	}
}
