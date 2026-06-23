using System;
using System.Collections.Generic;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	public class BoCaiCaiDaXiao
	{
		private BoCaiCaiDaXiao()
		{
		}

		public static BoCaiCaiDaXiao GetInstance()
		{
			return BoCaiCaiDaXiao.instance;
		}

		private void GetRank()
		{
			try
			{
				List<OpenLottery> newOpenLottery = BoCaiManager.getInstance().GetNewOpenLottery10(this.BoCaiType);
				if (null != newOpenLottery)
				{
					ReturnValue<List<KFBoCaoHistoryData>> winHistory = TcpCall.KFBoCaiManager.GetWinHistory(this.BoCaiType);
					if (!winHistory.IsReturn)
					{
						LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小]猜大小获取排行 失败", null, true);
					}
					else
					{
						List<KFBoCaoHistoryData> value = winHistory.Value;
						lock (this.mutex)
						{
							this.OpenHistory = newOpenLottery;
							this.WinHistory.Clear();
							if (null != value)
							{
								using (List<KFBoCaoHistoryData>.Enumerator enumerator = value.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KFBoCaoHistoryData item = enumerator.Current;
										if (item.RoleID >= 0)
										{
											OpenLottery openLottery = this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == item.DataPeriods);
											if (openLottery != null && !string.IsNullOrEmpty(openLottery.strWinNum))
											{
												item.OpenData = openLottery.strWinNum;
												this.WinHistory.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private bool GetStageData()
		{
			try
			{
				ReturnValue<KFStageData> kfstageData = TcpCall.KFBoCaiManager.GetKFStageData(this.BoCaiType);
				if (!kfstageData.IsReturn)
				{
					return false;
				}
				KFStageData value = kfstageData.Value;
				return this.SetStageData(value, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private bool GetOpenLotteryData(bool init = false)
		{
			try
			{
				ReturnValue<OpenLottery> openLottery = TcpCall.KFBoCaiManager.GetOpenLottery(this.BoCaiType);
				if (!openLottery.IsReturn)
				{
					return false;
				}
				OpenLottery value = openLottery.Value;
				return this.SetOpenLotteryData(value, init, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private void startNewGame(OpenLottery OpenData = null)
		{
			try
			{
				lock (this.mutex)
				{
					this.BoCaiBaseList.Clear();
					this.ServerOpenData.XiaoHaoDaiBi = -1;
					this.ServerData = new CenterServerCaiDaXiao();
					this.GetRank();
					if (null != OpenData)
					{
						this.SetOpenLotteryData(OpenData, true, false);
						this.GetStageData();
					}
					else
					{
						this.GetStageData();
						if (this.StageData.isOpenDay && this.StageData.Stage >= 2)
						{
							if (!this.GetOpenLotteryData(true))
							{
								LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小]本期开奖数据 失败", null, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public void Init()
		{
			try
			{
				lock (this.mutex)
				{
					this.StageData.isOpen = false;
					this.StageData.isOpenDay = false;
					this.startNewGame(null);
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
					if (!this.StageData.isOpen || !this.StageData.isOpenDay)
					{
						LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜大小] 初始化猜大小 博彩猜大小 暂未开启活动 isOpen={0}, OpenTime={1}", this.StageData.isOpen, this.StageData.OpenTime), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private void CheckStageData(DateTime _time, bool isReload)
		{
			try
			{
				lock (this.mutex)
				{
					bool flag2 = false;
					double totalSeconds = (_time - this.ServerData.GetStageDataTime).TotalSeconds;
					if (isReload)
					{
						flag2 = true;
					}
					else if (this.StageData.isOpenDay)
					{
						if (totalSeconds > 60.0)
						{
							flag2 = true;
						}
						else if (this.StageData.Stage == 1 && totalSeconds > 5.0)
						{
							flag2 = true;
						}
						else if (this.StageData.Stage == 4 && totalSeconds > 5.0)
						{
							flag2 = true;
						}
						else if (this.StageData.Stage >= 1 && this.StageData.LastOpenTime >= 0L && (totalSeconds + 1.0) * 1000.0 > (double)this.StageData.LastOpenTime)
						{
							flag2 = true;
						}
						else if (this.StageData.LastOpenTime < 0L && totalSeconds > 3.0)
						{
							flag2 = true;
						}
					}
					else if (this.StageData.OpenTime >= 0L && (this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime) - _time).TotalSeconds < 1.0)
					{
						flag2 = true;
					}
					else if (this.StageData.OpenTime < 0L && totalSeconds > 30.0)
					{
						flag2 = true;
					}
					if (flag2)
					{
						this.GetStageData();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public void BigTimeUpData(bool reload = false)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				this.CheckStageData(dateTime, reload);
				if (this.StageData.isOpenDay)
				{
					if (this.StageData.Stage < 5 && this.StageData.Stage >= 2 && this.ServerOpenData.DataPeriods < 1L)
					{
						this.GetOpenLotteryData(false);
					}
					else if (this.StageData.Stage >= 2 && this.StageData.Stage < 4 && (dateTime - this.ServerData.UpRateTime).TotalSeconds > 15.0)
					{
						this.GetOpenLotteryData(false);
					}
					else if (this.StageData.LastOpenTime < 0L && (dateTime - this.ServerData.UpRateTime).TotalSeconds > 3.0)
					{
						this.GetOpenLotteryData(false);
					}
				}
				else if (this.StageData.OpenTime > 60L && (long)(dateTime - this.ServerData.UpOldOpenTime).TotalMinutes > 30L)
				{
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					if (string.IsNullOrEmpty(this.ServerOpenData.strWinNum))
					{
						BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
					}
					else
					{
						BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods + 1L);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public double CompensateRate(string winNum, string info, out string winType)
		{
			winType = "";
			try
			{
				lock (this.mutex)
				{
					int num = BoCaiHelper.String2Int(winNum);
					if (num < 0)
					{
						return 0.0;
					}
					DiceValueEnum diceValueEnum;
					if (num > 3 && num < 11)
					{
						diceValueEnum = 1;
					}
					else if (num >= 11 && num < 18)
					{
						diceValueEnum = 3;
					}
					else
					{
						diceValueEnum = 2;
					}
					int num2 = diceValueEnum;
					winType = num2.ToString();
					string[] array = info.Split(new char[]
					{
						','
					});
					long num3 = Convert.ToInt64(array[diceValueEnum - 1]);
					long num4 = Convert.ToInt64(array[0]) + Convert.ToInt64(array[1]) + Convert.ToInt64(array[2]);
					if (num4 > 0L && num3 > 0L)
					{
						return Math.Truncate(100.0 * (double)num4 / (double)num3) / 100.0;
					}
					return 1.0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return 0.0;
		}

		private void OpenLotterySetWin()
		{
			try
			{
				if (this.StageData.Stage == 5 && !this.ServerData.IsAward)
				{
					if (string.IsNullOrEmpty(this.ServerOpenData.strWinNum) || string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
					{
						this.GetOpenLotteryData(false);
					}
					if (this.ServerOpenData.DataPeriods >= 1L && this.ServerOpenData.XiaoHaoDaiBi >= 1 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum) && !string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
					{
						LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜大小]猜大小 开奖 GetOpenLottery su DataPeriods={0}", this.ServerOpenData.DataPeriods), null, true);
						string winType;
						double num = this.CompensateRate(this.ServerOpenData.strWinNum, this.ServerOpenData.WinInfo, out winType);
						if (num < 1.0)
						{
							LogManager.WriteLog(0, "[ljl_caidaxiao_猜大小]猜大小 开奖 赔率 < 1 ", null, true);
						}
						else
						{
							List<BuyBoCai2SDB> list = new List<BuyBoCai2SDB>();
							lock (this.mutex)
							{
								foreach (PlayerBuyBoCaiData playerBuyBoCaiData in this.BoCaiBaseList)
								{
									BuyBoCai2SDB buyBoCai2SDB = new BuyBoCai2SDB
									{
										ZoneID = playerBuyBoCaiData.ZoneID,
										m_RoleID = playerBuyBoCaiData.RoleID,
										ServerId = playerBuyBoCaiData.ServerId,
										strUserID = playerBuyBoCaiData.strUserID,
										m_RoleName = playerBuyBoCaiData.RoleName,
										DataPeriods = this.ServerOpenData.DataPeriods,
										BocaiType = this.BoCaiType,
										IsSend = false,
										IsWin = false
									};
									foreach (BoCaiBuyItem boCaiBuyItem in playerBuyBoCaiData.BuyItemList)
									{
										BuyBoCai2SDB buyBoCai2SDB2 = new BuyBoCai2SDB();
										GlobalNew.Copy<BuyBoCai2SDB>(buyBoCai2SDB, ref buyBoCai2SDB2);
										buyBoCai2SDB2.BuyNum = boCaiBuyItem.BuyNum;
										buyBoCai2SDB2.strBuyValue = boCaiBuyItem.strBuyValue;
										list.Add(buyBoCai2SDB2);
									}
								}
							}
							this.ServerOpenData.IsAward = true;
							this.ServerData.IsAward = true;
							foreach (BuyBoCai2SDB buyBoCai2SDB in list)
							{
								BuyBoCai2SDB buyBoCai2SDB;
								if (!BoCaiManager.getInstance().SendWinItem(this.ServerOpenData, buyBoCai2SDB, num, false, winType))
								{
									this.ServerOpenData.IsAward = false;
								}
							}
							if (this.ServerOpenData.IsAward)
							{
								Global.Send2DB<OpenLottery>(2084, this.ServerOpenData, 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public bool SetStageData(KFStageData data, bool isKF = true)
		{
			try
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				lock (this.mutex)
				{
					if (null == data)
					{
						return false;
					}
					flag2 = (this.StageData.isOpen != data.isOpen);
					if (data.Stage != this.StageData.Stage)
					{
						flag = (this.StageData.isOpenDay && data.Stage == 5);
						flag3 = true;
					}
					this.StageData = data;
					this.ServerData.GetStageDataTime = TimeUtil.NowDateTime();
				}
				if (flag)
				{
					this.OpenLotterySetWin();
					this.GetRank();
				}
				if (flag3 && (this.StageData.Stage == 5 || this.StageData.Stage == 3 || this.StageData.Stage == 2))
				{
					if (this.ServerOpenData.DataPeriods < 0L)
					{
						this.GetOpenLotteryData(false);
					}
					this.UpdateBoCai();
				}
				if (flag2)
				{
					this.PriorityActivity(null);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool SetOpenLotteryData(OpenLottery OpenData, bool init = false, bool isKF = false)
		{
			try
			{
				lock (this.mutex)
				{
					if (null != OpenData)
					{
						if (OpenData.DataPeriods < 1L || OpenData.XiaoHaoDaiBi < 1)
						{
							if (this.StageData.Stage > 1)
							{
								LogManager.WriteLog(2, string.Format("[ljl_caidaxiao_猜大小] DataPeriods = {0},XiaoHaoDaiBi={1} ", OpenData.DataPeriods, OpenData.XiaoHaoDaiBi), null, true);
							}
							return false;
						}
						if (this.ServerOpenData.DataPeriods != OpenData.DataPeriods && this.ServerOpenData.DataPeriods > 0L && !init)
						{
							this.startNewGame(OpenData);
							return true;
						}
						if (this.ServerOpenData.DataPeriods != OpenData.DataPeriods)
						{
						}
						this.ServerOpenData = OpenData;
						this.ServerData.UpRateTime = TimeUtil.NowDateTime();
						Global.Send2DB<OpenLottery>(2084, OpenData, 0);
						if (isKF && this.StageData.Stage == 2)
						{
							this.UpdateBoCai();
						}
						return true;
					}
					else
					{
						LogManager.WriteLog(2, "[ljl_caidaxiao_猜大小] 猜大小 TcpCall.KFBoCaiManager.GetOpenLottery = null", null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool IsCanBuy()
		{
			bool result;
			lock (this.mutex)
			{
				result = (this.StageData.isOpenDay && this.StageData.Stage == 2 && this.ServerOpenData.DataPeriods > 1L);
			}
			return result;
		}

		public long GetDataPeriods()
		{
			long dataPeriods;
			lock (this.mutex)
			{
				dataPeriods = this.ServerOpenData.DataPeriods;
			}
			return dataPeriods;
		}

		public int GetXiaoHaoDaiBi()
		{
			int xiaoHaoDaiBi;
			lock (this.mutex)
			{
				xiaoHaoDaiBi = this.ServerOpenData.XiaoHaoDaiBi;
			}
			return xiaoHaoDaiBi;
		}

		public int GetBuyNum(int roleID)
		{
			int result;
			lock (this.mutex)
			{
				int num = 0;
				PlayerBuyBoCaiData playerBuyBoCaiData = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == roleID);
				if (null != playerBuyBoCaiData)
				{
					foreach (BoCaiBuyItem boCaiBuyItem in playerBuyBoCaiData.BuyItemList)
					{
						num += boCaiBuyItem.BuyNum;
					}
				}
				result = num;
			}
			return result;
		}

		public BuyBoCai2SDB BuyBocai(GameClient client, int buyNum, string BuyVal, ref int allNum)
		{
			BuyBoCai2SDB buyBoCai2SDB = null;
			try
			{
				lock (this.mutex)
				{
					PlayerBuyBoCaiData playerBuyBoCaiData = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == client.ClientData.RoleID);
					if (null == playerBuyBoCaiData)
					{
						playerBuyBoCaiData = new PlayerBuyBoCaiData();
						playerBuyBoCaiData.RoleID = client.ClientData.RoleID;
						playerBuyBoCaiData.RoleName = client.ClientData.RoleName;
						playerBuyBoCaiData.ZoneID = client.ClientData.ZoneID;
						playerBuyBoCaiData.strUserID = client.strUserID;
						playerBuyBoCaiData.ServerId = client.ServerId;
						playerBuyBoCaiData.BuyItemList = new List<BoCaiBuyItem>();
						BoCaiBuyItem boCaiBuyItem = new BoCaiBuyItem
						{
							BuyNum = buyNum,
							strBuyValue = BuyVal
						};
						playerBuyBoCaiData.BuyItemList.Add(boCaiBuyItem);
						this.BoCaiBaseList.Add(playerBuyBoCaiData);
					}
					else
					{
						BoCaiBuyItem boCaiBuyItem = playerBuyBoCaiData.BuyItemList.Find((BoCaiBuyItem x) => x.strBuyValue.Equals(BuyVal));
						if (null == boCaiBuyItem)
						{
							boCaiBuyItem = new BoCaiBuyItem
							{
								BuyNum = buyNum,
								strBuyValue = BuyVal,
								DataPeriods = this.ServerOpenData.DataPeriods
							};
							playerBuyBoCaiData.BuyItemList.Add(boCaiBuyItem);
						}
						else
						{
							boCaiBuyItem.BuyNum += buyNum;
							allNum = boCaiBuyItem.BuyNum;
						}
					}
					buyBoCai2SDB = new BuyBoCai2SDB();
					buyBoCai2SDB.m_RoleID = playerBuyBoCaiData.RoleID;
					buyBoCai2SDB.m_RoleName = playerBuyBoCaiData.RoleName;
					buyBoCai2SDB.ZoneID = playerBuyBoCaiData.ZoneID;
					buyBoCai2SDB.strUserID = playerBuyBoCaiData.strUserID;
					buyBoCai2SDB.ServerId = playerBuyBoCaiData.ServerId;
					buyBoCai2SDB.BuyNum = buyNum;
					buyBoCai2SDB.strBuyValue = BuyVal;
					buyBoCai2SDB.BocaiType = this.BoCaiType;
					buyBoCai2SDB.DataPeriods = this.ServerOpenData.DataPeriods;
					buyBoCai2SDB.IsSend = false;
					buyBoCai2SDB.IsWin = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
			return buyBoCai2SDB;
		}

		public void CopyBuyList(out List<BoCaiBuyItem> itemList, int roleID)
		{
			itemList = new List<BoCaiBuyItem>();
			try
			{
				lock (this.mutex)
				{
					PlayerBuyBoCaiData playerBuyBoCaiData = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == roleID);
					if (null != playerBuyBoCaiData)
					{
						foreach (BoCaiBuyItem boCaiBuyItem in playerBuyBoCaiData.BuyItemList)
						{
							BoCaiBuyItem item = new BoCaiBuyItem
							{
								BuyNum = boCaiBuyItem.BuyNum,
								strBuyValue = boCaiBuyItem.strBuyValue,
								DataPeriods = boCaiBuyItem.DataPeriods
							};
							itemList.Add(item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public void OpenGetBoCai(int roleid, ref GetBoCaiResult mgsData)
		{
			try
			{
				lock (this.mutex)
				{
					this.CopyBuyList(out mgsData.ItemList, roleid);
					mgsData.NowPeriods = this.ServerOpenData.DataPeriods;
					mgsData.IsOpen = (this.StageData.Stage > 1);
					mgsData.Value1 = this.ServerOpenData.WinInfo;
					mgsData.Stage = this.StageData.Stage;
					mgsData.OpenHistory = new List<BoCaiOpenHistory>();
					if (this.StageData.isOpenDay)
					{
						mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					else
					{
						mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					if (mgsData.Stage == 5)
					{
						mgsData.OpenTime = this.StageData.LastOpenTime;
					}
					BoCaiHelper.CopyHistoryData(this.WinHistory, out mgsData.WinLotteryRoleList);
					if (null != this.OpenHistory)
					{
						foreach (OpenLottery openLottery in this.OpenHistory)
						{
							BoCaiOpenHistory boCaiOpenHistory = new BoCaiOpenHistory();
							boCaiOpenHistory.DataPeriods = openLottery.DataPeriods;
							boCaiOpenHistory.OpenValue = openLottery.strWinNum;
							mgsData.OpenHistory.Add(boCaiOpenHistory);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public void UpdateBoCai()
		{
			try
			{
				lock (this.mutex)
				{
					BoCaiUpdate boCaiUpdate = new BoCaiUpdate();
					boCaiUpdate.BocaiType = this.BoCaiType;
					boCaiUpdate.Value1 = this.ServerOpenData.WinInfo;
					boCaiUpdate.DataPeriods = this.ServerOpenData.DataPeriods;
					boCaiUpdate.Stage = this.StageData.Stage;
					if (this.StageData.isOpenDay)
					{
						boCaiUpdate.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					else
					{
						boCaiUpdate.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
					}
					if (boCaiUpdate.Stage == 5)
					{
						boCaiUpdate.OpenTime = this.StageData.LastOpenTime;
					}
					FunctionSendManager.GetInstance().SendMsg<BoCaiUpdate>(FunctionType.CaiDaXiao, 2084, boCaiUpdate);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		public void PriorityActivity(GameClient client = null)
		{
			try
			{
				int num = 0;
				lock (this.mutex)
				{
					num = Convert.ToInt32(this.StageData.isOpen);
				}
				if (null == client)
				{
					GameManager.ClientMgr.NotifyAllActivityState(19, num, "", "", 0);
				}
				else
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						19,
						num,
						"",
						0,
						0
					});
					client.sendCmd(770, cmdData, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜大小]{0}", ex.ToString()), null, true);
			}
		}

		private static BoCaiCaiDaXiao instance = new BoCaiCaiDaXiao();

		private object mutex = new object();

		private int BoCaiType = 1;

		public KFStageData StageData = new KFStageData();

		private OpenLottery ServerOpenData = new OpenLottery();

		private CenterServerCaiDaXiao ServerData = new CenterServerCaiDaXiao();

		private List<OpenLottery> OpenHistory = new List<OpenLottery>();

		private List<KFBoCaoHistoryData> WinHistory = new List<KFBoCaoHistoryData>();

		private List<PlayerBuyBoCaiData> BoCaiBaseList = new List<PlayerBuyBoCaiData>();
	}
}
