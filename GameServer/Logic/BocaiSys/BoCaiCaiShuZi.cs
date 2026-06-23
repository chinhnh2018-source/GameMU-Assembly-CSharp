using System;
using System.Collections.Generic;
using System.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	public class BoCaiCaiShuZi
	{
		private BoCaiCaiShuZi()
		{
		}

		public static BoCaiCaiShuZi GetInstance()
		{
			return BoCaiCaiShuZi.instance;
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			return false;
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
						LogManager.WriteLog(2, "[ljl_caidaxiao_猜数字]猜数字获取排行 失败", null, true);
					}
					else
					{
						List<KFBoCaoHistoryData> value = winHistory.Value;
						lock (this.mutex)
						{
							this.RankResult = true;
							this.OpenHistory = newOpenLottery;
							this.WinHistory.Clear();
							if (null != value)
							{
								using (List<KFBoCaoHistoryData>.Enumerator enumerator = value.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KFBoCaoHistoryData item = enumerator.Current;
										OpenLottery openLottery = this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == item.DataPeriods);
										if (openLottery != null && !string.IsNullOrEmpty(openLottery.strWinNum))
										{
											item.OpenData = openLottery.strWinNum;
											this.WinHistory.Add(item);
										}
									}
								}
								List<long> list = new List<long>();
								List<long> list2 = new List<long>();
								foreach (OpenLottery openLottery2 in this.OpenHistory)
								{
									list.Add(openLottery2.DataPeriods);
								}
								list2 = this.BuyItemHistoryDict.Keys.ToList<long>();
								using (List<long>.Enumerator enumerator3 = list2.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										long Periods = enumerator3.Current;
										if (list.Find((long x) => x == Periods) < 1L)
										{
											this.BuyItemHistoryDict.Remove(Periods);
										}
									}
								}
								foreach (long num in list)
								{
									if (!this.BuyItemHistoryDict.ContainsKey(num))
									{
										List<BuyBoCai2SDB> list3;
										if (BoCaiManager.getInstance().GetBuyList2DB(this.BoCaiType, num, out list3, 1))
										{
											List<RoleBuyHistory> list4 = new List<RoleBuyHistory>();
											using (List<BuyBoCai2SDB>.Enumerator enumerator4 = list3.GetEnumerator())
											{
												while (enumerator4.MoveNext())
												{
													BuyBoCai2SDB dbdata = enumerator4.Current;
													RoleBuyHistory roleBuyHistory = list4.Find((RoleBuyHistory x) => x.RoleID == dbdata.m_RoleID);
													if (null == roleBuyHistory)
													{
														roleBuyHistory = new RoleBuyHistory();
														roleBuyHistory.RoleID = dbdata.m_RoleID;
														roleBuyHistory.BuyItemList = new List<BoCaiBuyItem>();
														list4.Add(roleBuyHistory);
													}
													roleBuyHistory.BuyItemList.Add(new BoCaiBuyItem
													{
														BuyNum = dbdata.BuyNum,
														strBuyValue = dbdata.strBuyValue,
														DataPeriods = num
													});
												}
											}
											this.BuyItemHistoryDict.Add(num, list4);
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		public bool LoadBuyList(long DataPeriods)
		{
			try
			{
				if (!this.IsStart)
				{
					return true;
				}
				List<BuyBoCai2SDB> list = new List<BuyBoCai2SDB>();
				if (DataPeriods > 0L && !BoCaiManager.getInstance().GetBuyList2DB(this.BoCaiType, DataPeriods, out list, 1))
				{
					LogManager.WriteLog(2, string.Format("[ljl_caidaxiao_猜数字]获取购买记录失败 BoCaiType={0},DataPeriods={1}", this.BoCaiType, DataPeriods), null, true);
					return false;
				}
				lock (this.mutex)
				{
					using (List<BuyBoCai2SDB>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BuyBoCai2SDB item = enumerator.Current;
							PlayerBuyBoCaiData playerBuyBoCaiData = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == item.m_RoleID);
							if (null == playerBuyBoCaiData)
							{
								playerBuyBoCaiData = new PlayerBuyBoCaiData();
								playerBuyBoCaiData.BuyItemList = new List<BoCaiBuyItem>();
								playerBuyBoCaiData.ZoneID = item.ZoneID;
								playerBuyBoCaiData.RoleID = item.m_RoleID;
								playerBuyBoCaiData.ServerId = item.ServerId;
								playerBuyBoCaiData.RoleName = item.m_RoleName;
								playerBuyBoCaiData.strUserID = item.strUserID;
								this.BoCaiBaseList.Add(playerBuyBoCaiData);
							}
							playerBuyBoCaiData.BuyItemList.Add(new BoCaiBuyItem
							{
								BuyNum = item.BuyNum,
								strBuyValue = item.strBuyValue,
								DataPeriods = item.DataPeriods
							});
						}
					}
					LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜数字]加载购买数据true ,DataPeriods = {0}", DataPeriods), null, true);
					this.IsStart = false;
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			LogManager.WriteLog(2, "[ljl_caidaxiao_猜数字]猜数字获取排行 失败", null, true);
			return false;
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private void startNewGame(OpenLottery OpenData = null, bool init = false)
		{
			try
			{
				lock (this.mutex)
				{
					this.BoCaiBaseList.Clear();
					this.ServerOpenData.XiaoHaoDaiBi = -1;
					this.ServerData = new CenterServerCaiShuZi();
					this.GetRank();
					if (null != OpenData)
					{
						this.SetOpenLotteryData(OpenData, true, false);
						this.GetStageData();
					}
					else
					{
						if (this.GetStageData())
						{
							this.StartServerStage = this.StageData.Stage;
						}
						else
						{
							this.StartServerStage = 6;
						}
						if (this.StageData.isOpenDay && this.StageData.Stage >= 2)
						{
							if (!this.GetOpenLotteryData(true))
							{
								LogManager.WriteLog(2, "[ljl_caidaxiao_猜数字]本期开奖数据 失败", null, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		public void Init()
		{
			try
			{
				lock (this.mutex)
				{
					this.IsStart = true;
					this.StageData.isOpen = false;
					this.StageData.isOpenDay = false;
					GetOpenList getOpenList;
					if (BoCaiManager.getInstance().GetOpenList2DB(this.BoCaiType, out getOpenList) && getOpenList != null && null != getOpenList.ItemList)
					{
						this.StartServerOpenData = new OpenLottery();
						this.StartServerOpenData.DataPeriods = 0L;
						if (getOpenList.ItemList.Count < 1)
						{
							this.StartServerOpenData.IsAward = true;
							this.StartServerOpenData.DataPeriods = getOpenList.MaxDataPeriods;
						}
						foreach (OpenLottery openLottery in getOpenList.ItemList)
						{
							if (openLottery.DataPeriods > this.StartServerOpenData.DataPeriods)
							{
								this.StartServerOpenData = openLottery;
							}
						}
					}
					this.startNewGame(null, true);
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
					if (!this.StageData.isOpen)
					{
						LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜数字]博彩猜数字 暂未开启活动 OpenTime={0}", this.StageData.OpenTime), null, true);
					}
				}
				this.OpenLotterySetWin();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
						if (totalSeconds > (double)this.UpStageTime)
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
						else if (this.StageData.LastOpenTime < 0L && totalSeconds > 5.0)
						{
							flag2 = true;
						}
						else if (_time.Day != this.ServerData.GetStageDataTime.Day && _time.AddSeconds(-5.0).Day != this.ServerData.GetStageDataTime.Day)
						{
							flag2 = true;
						}
					}
					else if (this.ServerData.GetStageDataTime.Day != _time.Day)
					{
						flag2 = true;
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
					else if (this.StageData.Stage >= 2 && this.StageData.Stage < 4 && (dateTime - this.ServerData.UpBalanceTime).TotalSeconds > (double)this.UpAllBalanceTime)
					{
						this.GetOpenLotteryData(false);
					}
					else if (dateTime.Day != this.ServerData.UpBalanceTime.Day && dateTime.AddSeconds(-5.0).Day != this.ServerData.UpBalanceTime.Day)
					{
						this.GetOpenLotteryData(false);
					}
					this.OpenLotterySetWin();
				}
				if (!this.RankResult && dateTime.Second % 5 == 0)
				{
					this.GetRank();
				}
				if (this.StageData.Stage <= 2 && (dateTime - this.ServerData.UpOldOpenTime).Hours > 1)
				{
					this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
					BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
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
					if (this.FirstDataPeriods == this.ServerOpenData.DataPeriods && this.FirstDataPeriods > 0L)
					{
						if (null != this.StartServerOpenData)
						{
							if (this.StartServerOpenData.DataPeriods == this.FirstDataPeriods && this.StartServerOpenData.IsAward)
							{
								return;
							}
						}
						else if (this.StartServerStage > 4)
						{
							return;
						}
					}
					if (this.ServerOpenData.DataPeriods >= 1L && this.ServerOpenData.XiaoHaoDaiBi >= 1 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum) && !string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
					{
						LogManager.WriteLog(0, string.Format("[ljl_caidaxiao_猜数字]猜数字 开奖 GetOpenLottery su DataPeriods={0}", this.ServerOpenData.DataPeriods), null, true);
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
									if (boCaiBuyItem.DataPeriods == this.ServerOpenData.DataPeriods)
									{
										BuyBoCai2SDB buyBoCai2SDB2 = new BuyBoCai2SDB();
										GlobalNew.Copy<BuyBoCai2SDB>(buyBoCai2SDB, ref buyBoCai2SDB2);
										buyBoCai2SDB2.BuyNum = boCaiBuyItem.BuyNum;
										buyBoCai2SDB2.strBuyValue = boCaiBuyItem.strBuyValue;
										list.Add(buyBoCai2SDB2);
									}
								}
							}
						}
						this.ServerOpenData.IsAward = true;
						this.ServerData.IsAward = true;
						foreach (BuyBoCai2SDB buyBoCai2SDB in list)
						{
							BuyBoCai2SDB buyBoCai2SDB;
							if (!BoCaiManager.getInstance().SendWinItem(this.ServerOpenData, buyBoCai2SDB))
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
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
					if (data.Stage != this.StageData.Stage)
					{
						flag = (this.StageData.isOpenDay && data.Stage == 5);
						flag3 = true;
					}
					flag2 = (this.StageData.isOpen != data.isOpen);
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
					if (this.ServerOpenData.DataPeriods < 0L || (this.StageData.Stage == 2 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum)))
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
						if (this.FirstDataPeriods < 1L)
						{
							this.FirstDataPeriods = OpenData.DataPeriods;
						}
						if (OpenData.DataPeriods < 1L || OpenData.XiaoHaoDaiBi < 1)
						{
							if (this.StageData.Stage > 1)
							{
								LogManager.WriteLog(2, string.Format("[ljl_caidaxiao_猜数字] DataPeriods = {0},XiaoHaoDaiBi={1} ", OpenData.DataPeriods, OpenData.XiaoHaoDaiBi), null, true);
							}
							return false;
						}
						if (this.ServerOpenData.DataPeriods < 1L)
						{
							if (!this.LoadBuyList(OpenData.DataPeriods))
							{
								return false;
							}
						}
						if (this.ServerOpenData.DataPeriods < OpenData.DataPeriods && this.ServerOpenData.DataPeriods > 1L && !init)
						{
							this.startNewGame(OpenData, false);
							return true;
						}
						if (this.ServerOpenData.DataPeriods < OpenData.DataPeriods)
						{
						}
						this.ServerOpenData = OpenData;
						this.ServerData.UpBalanceTime = TimeUtil.NowDateTime();
						if (this.FirstDataPeriods == OpenData.DataPeriods && this.StartServerStage > 4)
						{
							if (null != this.StartServerOpenData)
							{
								if (this.StartServerOpenData.DataPeriods != this.FirstDataPeriods || !this.StartServerOpenData.IsAward)
								{
									Global.Send2DB<OpenLottery>(2084, OpenData, 0);
								}
							}
						}
						else
						{
							Global.Send2DB<OpenLottery>(2084, OpenData, 0);
						}
						if (isKF && this.StageData.Stage == 2)
						{
							this.UpdateBoCai();
						}
						return true;
					}
					else
					{
						LogManager.WriteLog(2, "[ljl_caidaxiao_猜数字] 猜数字 TcpCall.KFBoCaiManager.GetOpenLottery = null", null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
							strBuyValue = BuyVal,
							DataPeriods = this.ServerOpenData.DataPeriods
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
			return buyBoCai2SDB;
		}

		public void OpenGetBoCai(int roleid, ref GetBoCaiResult mgsData)
		{
			try
			{
				lock (this.mutex)
				{
					if (null == this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == this.ServerOpenData.DataPeriods))
					{
						this.CopyBuyList(out mgsData.ItemList, roleid);
					}
					this.CopyBuyHistoryList(ref mgsData.ItemList, roleid);
					mgsData.NowPeriods = this.ServerOpenData.DataPeriods;
					mgsData.IsOpen = (this.StageData.Stage > 1);
					mgsData.Value1 = this.ServerOpenData.AllBalance.ToString();
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
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
					boCaiUpdate.Value1 = this.ServerOpenData.AllBalance.ToString();
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
					FunctionSendManager.GetInstance().SendMsg<BoCaiUpdate>(FunctionType.CaiShuZi, 2084, boCaiUpdate);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		public void CopyBuyHistoryList(ref List<BoCaiBuyItem> itemList, int roleID)
		{
			try
			{
				lock (this.mutex)
				{
					foreach (List<RoleBuyHistory> list in this.BuyItemHistoryDict.Values)
					{
						RoleBuyHistory roleBuyHistory = list.Find((RoleBuyHistory x) => x.RoleID == roleID);
						if (null != roleBuyHistory)
						{
							foreach (BoCaiBuyItem boCaiBuyItem in roleBuyHistory.BuyItemList)
							{
								itemList.Add(new BoCaiBuyItem
								{
									BuyNum = boCaiBuyItem.BuyNum,
									strBuyValue = boCaiBuyItem.strBuyValue,
									DataPeriods = boCaiBuyItem.DataPeriods
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
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

		public long GetDataPeriods()
		{
			long dataPeriods;
			lock (this.mutex)
			{
				dataPeriods = this.ServerOpenData.DataPeriods;
			}
			return dataPeriods;
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
					GameManager.ClientMgr.NotifyAllActivityState(18, num, "", "", 0);
				}
				else
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						18,
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
				LogManager.WriteLog(9, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
			}
		}

		private static BoCaiCaiShuZi instance = new BoCaiCaiShuZi();

		private bool IsStart;

		private bool RankResult = false;

		private int UpStageTime = 1800;

		private int UpAllBalanceTime = 660;

		private object mutex = new object();

		private int BoCaiType = 2;

		private KFStageData StageData = new KFStageData();

		private OpenLottery ServerOpenData = new OpenLottery();

		private long FirstDataPeriods = 0L;

		private OpenLottery StartServerOpenData = null;

		private BoCaiStageEnum StartServerStage = 6;

		private CenterServerCaiShuZi ServerData = new CenterServerCaiShuZi();

		private List<OpenLottery> OpenHistory = new List<OpenLottery>();

		private List<KFBoCaoHistoryData> WinHistory = new List<KFBoCaoHistoryData>();

		private List<PlayerBuyBoCaiData> BoCaiBaseList = new List<PlayerBuyBoCaiData>();

		private Dictionary<long, List<RoleBuyHistory>> BuyItemHistoryDict = new Dictionary<long, List<RoleBuyHistory>>();
	}
}
