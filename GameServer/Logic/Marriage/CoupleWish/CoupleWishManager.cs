using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Marriage.CoupleWish
{
	public class CoupleWishManager : SingletonTemplate<CoupleWishManager>, IManager, ICmdProcessorEx, ICmdProcessor
	{
		public bool initialize()
		{
			bool result;
			if (!this._Config.Load(Global.GameResPath(CoupleWishConsts.RankAwardCfgFile), Global.GameResPath(CoupleWishConsts.WishTypeCfgFile), Global.GameResPath(CoupleWishConsts.YanHuiCfgFile)))
			{
				result = false;
			}
			else
			{
				this.StatueMgr.SetWishConfig(this._Config);
				if (!this.StatueMgr.LoadConfig())
				{
					result = false;
				}
				else
				{
					foreach (CoupleWishRankAwardConfig coupleWishRankAwardConfig in this._Config.RankAwardCfgList)
					{
						List<GoodsData> goodsOneTag = GoodsHelper.ParseGoodsDataList(((string)coupleWishRankAwardConfig.GoodsOneTag).Split(new char[]
						{
							'|'
						}), CoupleWishConsts.RankAwardCfgFile);
						List<GoodsData> goodsTwoTag = GoodsHelper.ParseGoodsDataList(((string)coupleWishRankAwardConfig.GoodsTwoTag).Split(new char[]
						{
							'|'
						}), CoupleWishConsts.RankAwardCfgFile);
						coupleWishRankAwardConfig.GoodsOneTag = goodsOneTag;
						coupleWishRankAwardConfig.GoodsTwoTag = goodsTwoTag;
					}
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("WishEffectAwardMax", ',');
					this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.BangJin] = paramValueIntArrayByName[0];
					this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.BangZuan] = paramValueIntArrayByName[1];
					this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.Exp] = paramValueIntArrayByName[2];
					int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("WishEffectAwardMap", ',');
					if (paramValueIntArrayByName2 != null)
					{
						foreach (int item in paramValueIntArrayByName2)
						{
							if (!this.CanEffectAwardMap.Contains(item))
							{
								this.CanEffectAwardMap.Add(item);
							}
						}
					}
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CoupleWishManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
					result = true;
				}
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1390, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1391, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerStreamProcessorEx(1392, SingletonTemplate<CoupleWishManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessorEx(1394, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1395, 3, 3, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1396, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1397, 2, 2, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 1390)
			{
				this.HandleGetMainDataCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1391)
			{
				this.HandleGetWishRecordCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1392)
			{
				this.HandleWishOtherRoleCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1395)
			{
				this.HandleAdmireStatueCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1394)
			{
				this.HandleGetAdmireDataCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1396)
			{
				this.HandleGetPartyDataCommand(client, nID, bytes, cmdParams);
			}
			else if (nID == 1397)
			{
				this.HandleJoinPartyCommand(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		private void HandleJoinPartyCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int toCouleId = Convert.ToInt32(cmdParams[1]);
			lock (this.Mutex)
			{
				client.sendCmd(nID, this.StatueMgr.HandleJoinParty(client, toCouleId).ToString(), false);
			}
		}

		private void HandleGetPartyDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			lock (this.Mutex)
			{
				CoupleWishYanHuiData cmdData = this.StatueMgr.HandleQueryParty(client);
				client.sendCmd<CoupleWishYanHuiData>(nID, cmdData, false);
			}
		}

		private void HandleGetAdmireDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			lock (this.Mutex)
			{
				CoupleWishTop1AdmireData cmdData = this.StatueMgr.HandleQueryAdmireData(client);
				client.sendCmd<CoupleWishTop1AdmireData>(nID, cmdData, false);
			}
		}

		private void HandleAdmireStatueCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int toCoupleId = Convert.ToInt32(cmdParams[1]);
			int num = Convert.ToInt32(cmdParams[2]);
			if (num == 1 || num == 2)
			{
				lock (this.Mutex)
				{
					client.sendCmd(nID, this.StatueMgr.HandleAdmireStatue(client, toCoupleId, num).ToString(), false);
				}
			}
		}

		private void HandleWishOtherRoleCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			CoupleWishWishReqData cliReq = DataHelper.BytesToObject<CoupleWishWishReqData>(bytes, 0, bytes.Length);
			DateTime dateTime = TimeUtil.NowDateTime();
			int num;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd(nID, -12.ToString(), false);
			}
			else if (cliReq.CostType != 1 && cliReq.CostType != 2)
			{
				client.sendCmd(nID, -18.ToString(), false);
			}
			else if (!this._Config.IsInWishTime(dateTime, ref num))
			{
				client.sendCmd(nID, -31.ToString(), false);
			}
			else
			{
				CoupleWishTypeConfig coupleWishTypeConfig = this._Config.WishTypeCfgList.Find((CoupleWishTypeConfig _w) => _w.WishType == cliReq.WishType);
				if (coupleWishTypeConfig == null)
				{
					client.sendCmd(nID, -3.ToString(), false);
				}
				else if (cliReq.CostType == 1 && coupleWishTypeConfig.CostGoodsId > 0 && coupleWishTypeConfig.CostGoodsNum > 0 && Global.GetTotalGoodsCountByID(client, coupleWishTypeConfig.CostGoodsId) < coupleWishTypeConfig.CostGoodsNum)
				{
					client.sendCmd(nID, -6.ToString(), false);
				}
				else if (cliReq.CostType == 2 && coupleWishTypeConfig.CostZuanShi > 0 && client.ClientData.UserMoney < coupleWishTypeConfig.CostZuanShi)
				{
					client.sendCmd(nID, -10.ToString(), false);
				}
				else
				{
					if (!string.IsNullOrEmpty(cliReq.WishTxt))
					{
						if (coupleWishTypeConfig.CanHaveWishTxt != 1)
						{
							client.sendCmd(nID, -25.ToString(), false);
							return;
						}
						if (cliReq.WishTxt.Length > CoupleWishConsts.MaxWishTxtLen)
						{
							client.sendCmd(nID, -26.ToString(), false);
							return;
						}
					}
					CoupleWishWishRoleReq coupleWishWishRoleReq = new CoupleWishWishRoleReq();
					coupleWishWishRoleReq.From.RoleId = client.ClientData.RoleID;
					coupleWishWishRoleReq.From.ZoneId = client.ClientData.ZoneID;
					coupleWishWishRoleReq.From.RoleName = client.ClientData.RoleName;
					coupleWishWishRoleReq.WishType = cliReq.WishType;
					coupleWishWishRoleReq.WishTxt = cliReq.WishTxt;
					RoleData4Selector roleData4Selector = null;
					RoleData4Selector roleData4Selector2 = null;
					CoupleWishCoupleDataK coupleWishCoupleDataK = null;
					if (cliReq.IsWishRankRole)
					{
						coupleWishWishRoleReq.IsWishRank = true;
						lock (this.Mutex)
						{
							int index;
							if (!this.SyncData.ThisWeek.CoupleIdex.TryGetValue(cliReq.ToRankCoupleId, out index))
							{
								client.sendCmd(nID, -12.ToString(), false);
								return;
							}
							coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[index];
							if (coupleWishCoupleDataK == null || coupleWishCoupleDataK.DbCoupleId != cliReq.ToRankCoupleId || coupleWishCoupleDataK.Rank > CoupleWishConsts.MaxRankNum * 2)
							{
								client.sendCmd(nID, -12.ToString(), false);
								return;
							}
							coupleWishWishRoleReq.ToCoupleId = cliReq.ToRankCoupleId;
							roleData4Selector = Global.sendToDB<RoleData4Selector, int>(10232, coupleWishCoupleDataK.Man.RoleId, client.ServerId);
							roleData4Selector2 = Global.sendToDB<RoleData4Selector, int>(10232, coupleWishCoupleDataK.Wife.RoleId, client.ServerId);
							if (roleData4Selector == null || roleData4Selector2 == null || roleData4Selector.RoleID <= 0 || roleData4Selector2.RoleID <= 0)
							{
								roleData4Selector2 = (roleData4Selector = null);
							}
						}
					}
					else
					{
						int num2 = -1;
						if (!string.IsNullOrEmpty(cliReq.ToLocalRoleName))
						{
							num2 = RoleName2IDs.FindRoleIDByName(cliReq.ToLocalRoleName, true);
						}
						if (num2 <= 0)
						{
							client.sendCmd(nID, -28.ToString(), false);
							return;
						}
						if (num2 == client.ClientData.RoleID)
						{
							client.sendCmd(nID, -27.ToString(), false);
							return;
						}
						int spouseID = MarryLogic.GetSpouseID(num2);
						if (spouseID <= 0)
						{
							client.sendCmd(nID, -29.ToString(), false);
							return;
						}
						roleData4Selector = Global.sendToDB<RoleData4Selector, int>(10232, num2, client.ServerId);
						roleData4Selector2 = Global.sendToDB<RoleData4Selector, int>(10232, spouseID, client.ServerId);
						if (roleData4Selector == null || roleData4Selector2 == null)
						{
							client.sendCmd(nID, -15.ToString(), false);
							return;
						}
						if (!MarryLogic.SameSexMarry(false))
						{
							if (roleData4Selector.RoleSex == roleData4Selector2.RoleSex)
							{
								client.sendCmd(nID, -15.ToString(), false);
								return;
							}
							if (roleData4Selector.RoleSex == 1)
							{
								DataHelper2.Swap<RoleData4Selector>(ref roleData4Selector, ref roleData4Selector2);
							}
						}
					}
					if (roleData4Selector != null && roleData4Selector2 != null)
					{
						coupleWishWishRoleReq.ToMan.RoleId = roleData4Selector.RoleID;
						coupleWishWishRoleReq.ToMan.ZoneId = roleData4Selector.ZoneId;
						coupleWishWishRoleReq.ToMan.RoleName = roleData4Selector.RoleName;
						coupleWishWishRoleReq.ToManSelector = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
						coupleWishWishRoleReq.ToWife.RoleId = roleData4Selector2.RoleID;
						coupleWishWishRoleReq.ToWife.ZoneId = roleData4Selector2.ZoneId;
						coupleWishWishRoleReq.ToWife.RoleName = roleData4Selector2.RoleName;
						coupleWishWishRoleReq.ToWifeSelector = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector2);
					}
					int num3 = TianTiClient.getInstance().CoupleWishWishRole(coupleWishWishRoleReq);
					if (num3 < 0)
					{
						client.sendCmd(nID, num3.ToString(), false);
					}
					else
					{
						if (cliReq.CostType == 1 && coupleWishTypeConfig.CostGoodsId > 0 && coupleWishTypeConfig.CostGoodsNum > 0)
						{
							bool flag2 = false;
							bool flag3 = false;
							Global.UseGoodsBindOrNot(client, coupleWishTypeConfig.CostGoodsId, coupleWishTypeConfig.CostGoodsNum, true, out flag2, out flag3);
						}
						if (cliReq.CostType == 2 && coupleWishTypeConfig.CostZuanShi > 0)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, coupleWishTypeConfig.CostZuanShi, "情侣祝福", true, true, false, DaiBiSySType.None);
						}
						if (coupleWishTypeConfig.IsHaveEffect == 1)
						{
							CoupleWishNtfWishEffectData coupleWishNtfWishEffectData = new CoupleWishNtfWishEffectData();
							coupleWishNtfWishEffectData.From = coupleWishWishRoleReq.From;
							coupleWishNtfWishEffectData.WishType = cliReq.WishType;
							coupleWishNtfWishEffectData.WishTxt = cliReq.WishTxt;
							coupleWishNtfWishEffectData.To = new List<KuaFuRoleMiniData>();
							if (cliReq.IsWishRankRole)
							{
								coupleWishNtfWishEffectData.To.Add(coupleWishCoupleDataK.Man);
								coupleWishNtfWishEffectData.To.Add(coupleWishCoupleDataK.Wife);
							}
							else if (coupleWishWishRoleReq.ToMan.RoleName == cliReq.ToLocalRoleName)
							{
								coupleWishNtfWishEffectData.To.Add(coupleWishWishRoleReq.ToMan);
							}
							else
							{
								coupleWishNtfWishEffectData.To.Add(coupleWishWishRoleReq.ToWife);
							}
							lock (this.Mutex)
							{
								this.HandleWishEffect(coupleWishNtfWishEffectData);
							}
						}
						client.sendCmd(nID, 1.ToString(), false);
					}
				}
			}
		}

		private void HandleGetWishRecordCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			List<CoupleWishWishRecordData> list = TianTiClient.getInstance().CoupleWishGetWishRecord(client.ClientData.RoleID);
			if (list != null)
			{
				list.Reverse();
			}
			client.sendCmd<List<CoupleWishWishRecordData>>(nID, list, false);
		}

		private void HandleGetMainDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (this.IsGongNengOpened(client))
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				CoupleWishMainData coupleWishMainData = new CoupleWishMainData();
				lock (this.Mutex)
				{
					coupleWishMainData.RankList = new List<CoupleWishCoupleData>(this.ThisWeekTopNList);
					coupleWishMainData.CanGetAwardId = this.CheckGiveAward(client);
					int index;
					if (this.SyncData.ThisWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out index))
					{
						CoupleWishCoupleDataK coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[index];
						if (coupleWishCoupleDataK.Man.RoleId == client.ClientData.RoleID || coupleWishCoupleDataK.Wife.RoleId == client.ClientData.RoleID)
						{
							coupleWishMainData.MyCoupleRank = coupleWishCoupleDataK.Rank;
							coupleWishMainData.MyCoupleBeWishNum = coupleWishCoupleDataK.BeWishedNum;
						}
					}
				}
				coupleWishMainData.MyCoupleManSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.RoleID, client.ServerId);
				if (MarryLogic.IsMarried(client.ClientData.RoleID))
				{
					coupleWishMainData.MyCoupleWifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.MyMarriageData.nSpouseID, client.ServerId);
				}
				if (client.ClientData.RoleSex == 1)
				{
					DataHelper2.Swap<RoleData4Selector>(ref coupleWishMainData.MyCoupleManSelector, ref coupleWishMainData.MyCoupleWifeSelector);
				}
				client.sendCmd<CoupleWishMainData>(nID, coupleWishMainData, false);
			}
		}

		private int CheckGiveAward(GameClient client)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				int num;
				if (!this._Config.IsInAwardTime(dateTime, ref num))
				{
					result = 0;
				}
				else
				{
					bool flag = false;
					try
					{
						object mutex;
						Monitor.Enter(mutex = this.Mutex, ref flag);
						string roleParamByName = Global.GetRoleParamByName(client, "29");
						string[] array = string.IsNullOrEmpty(roleParamByName) ? null : roleParamByName.Split(new char[]
						{
							','
						});
						int index;
						if (array != null && array.Length == 2 && Convert.ToInt32(array[0]) == num)
						{
							result = 0;
						}
						else if (num != this.SyncData.LastWeek.Week)
						{
							result = 0;
						}
						else if (!this.SyncData.LastWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out index))
						{
							result = 0;
						}
						else
						{
							CoupleWishCoupleDataK coupleData = this.SyncData.LastWeek.RankList[index];
							if (coupleData == null)
							{
								result = 0;
							}
							else if (coupleData.Man.RoleId != client.ClientData.RoleID && coupleData.Wife.RoleId != client.ClientData.RoleID)
							{
								result = 0;
							}
							else
							{
								CoupleWishRankAwardConfig coupleWishRankAwardConfig = this._Config.RankAwardCfgList.Find((CoupleWishRankAwardConfig _r) => coupleData.Rank >= _r.StartRank && (_r.EndRank <= 0 || coupleData.Rank <= _r.EndRank));
								if (coupleWishRankAwardConfig == null)
								{
									result = 0;
								}
								else
								{
									List<GoodsData> list = new List<GoodsData>();
									list.AddRange(coupleWishRankAwardConfig.GoodsOneTag as List<GoodsData>);
									list.AddRange((coupleWishRankAwardConfig.GoodsTwoTag as List<GoodsData>).FindAll((GoodsData _g) => Global.IsCanGiveRewardByOccupation(client, _g.GoodsID)));
									if (Global.CanAddGoodsDataList(client, list))
									{
										foreach (GoodsData goodsData in list)
										{
											Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "情侣排行榜", false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
										}
									}
									else
									{
										Global.UseMailGivePlayerAward3(client.ClientData.RoleID, list, GLang.GetLang(479, new object[0]), string.Format(GLang.GetLang(480, new object[0]), coupleData.Rank), 0, 0, 0);
									}
									Global.SaveRoleParamsStringToDB(client, "29", string.Format("{0},{1}", num, coupleWishRankAwardConfig.Id), true);
									this.CheckTipsIconState(client);
									result = coupleWishRankAwardConfig.Id;
								}
							}
						}
					}
					finally
					{
						if (flag)
						{
							object mutex;
							Monitor.Exit(mutex);
						}
					}
				}
			}
			return result;
		}

		public void CheckTipsIconState(GameClient client)
		{
			if (client != null && this.IsGongNengOpened(client))
			{
				bool bIconState = false;
				lock (this.Mutex)
				{
					int num = 0;
					if (this._Config.IsInAwardTime(TimeUtil.NowDateTime(), ref num))
					{
						string roleParamByName = Global.GetRoleParamByName(client, "29");
						string[] array = string.IsNullOrEmpty(roleParamByName) ? null : roleParamByName.Split(new char[]
						{
							','
						});
						if (array == null || array.Length != 2 || (Convert.ToInt32(array[0]) != num && this.SyncData.LastWeek.Week == num))
						{
							int index;
							if (this.SyncData.LastWeek.Week == num && this.SyncData.LastWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out index))
							{
								int rank = this.SyncData.LastWeek.RankList[index].Rank;
								bIconState = this._Config.RankAwardCfgList.Exists((CoupleWishRankAwardConfig _a) => rank >= _a.StartRank && (_a.EndRank <= 0 || rank <= _a.EndRank));
							}
						}
					}
				}
				if (client._IconStateMgr.AddFlushIconState(15012, bIconState))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public bool IsGongNengOpened(GameClient client)
		{
			return client != null;
		}

		private void SaveGetNextEffectAward(GameClient client, int day, CoupleWishManager.EWishEffectAwardType awardType, int get)
		{
			string roleParamByName = Global.GetRoleParamByName(client, "34");
			string[] array = (!string.IsNullOrEmpty(roleParamByName)) ? roleParamByName.Split(new char[]
			{
				','
			}) : null;
			int[] array2 = new int[5];
			array2[0] = (int)awardType;
			array2[1] = day;
			if (array != null && array.Length == 5 && Convert.ToInt32(array[1]) == day)
			{
				for (int i = 0; i < 3; i++)
				{
					array2[2 + i] = Convert.ToInt32(array[2 + i]);
				}
			}
			array2[(int)(2 + awardType)] = (int)Math.Min(2147483647L, (long)(array2[(int)(2 + awardType)] + get));
			Global.SaveRoleParamsStringToDB(client, "34", string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				array2[0],
				array2[1],
				array2[2],
				array2[3],
				array2[4]
			}), true);
		}

		private bool GetNextCanEffectAward(GameClient client, int day, out CoupleWishManager.EWishEffectAwardType awardType, out int canGetMax)
		{
			awardType = CoupleWishManager.EWishEffectAwardType.None;
			canGetMax = 0;
			string roleParamByName = Global.GetRoleParamByName(client, "34");
			string[] array = (!string.IsNullOrEmpty(roleParamByName)) ? roleParamByName.Split(new char[]
			{
				','
			}) : null;
			bool result;
			if (array == null || array.Length != 5)
			{
				awardType = CoupleWishManager.EWishEffectAwardType.BangJin;
				canGetMax = this.WishEffectDayMaxAward[awardType];
				result = true;
			}
			else if (Convert.ToInt32(array[1]) != day)
			{
				awardType = (Convert.ToInt32(array[0]) + CoupleWishManager.EWishEffectAwardType.BangZuan) % CoupleWishManager.EWishEffectAwardType.Max;
				canGetMax = this.WishEffectDayMaxAward[awardType];
				result = true;
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					int num = (Convert.ToInt32(array[0]) + i + 1) % 3;
					int num2 = Convert.ToInt32(array[2 + num]);
					if (this.WishEffectDayMaxAward[(CoupleWishManager.EWishEffectAwardType)num] > num2)
					{
						awardType = (CoupleWishManager.EWishEffectAwardType)num;
						canGetMax = this.WishEffectDayMaxAward[awardType] - num2;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private void HandleWishEffect(CoupleWishNtfWishEffectData effectData)
		{
			if (effectData != null)
			{
				int day = TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime());
				int num = 0;
				GameClient nextClient;
				while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
				{
					if (!nextClient.ClientData.FirstPlayStart)
					{
						if (this.CanEffectAwardMap.Contains(nextClient.ClientData.MapCode))
						{
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(nextClient.ClientData.MapCode, out gameMap))
							{
								if (gameMap.InSafeRegionList(nextClient.CurrentGrid))
								{
									effectData.GetBinJinBi = 0;
									effectData.GetBindZuanShi = 0;
									effectData.GetExp = 0;
									CoupleWishManager.EWishEffectAwardType ewishEffectAwardType;
									int val;
									if (!this.GetNextCanEffectAward(nextClient, day, out ewishEffectAwardType, out val))
									{
										nextClient.sendCmd<CoupleWishNtfWishEffectData>(1393, effectData, false);
									}
									else
									{
										int num2 = nextClient.ClientData.ChangeLifeCount * 100 + nextClient.ClientData.Level;
										int num3;
										if (ewishEffectAwardType == CoupleWishManager.EWishEffectAwardType.BangJin)
										{
											effectData.GetBinJinBi = Math.Max(0, Math.Min(400 * num2, val));
											num3 = effectData.GetBinJinBi;
										}
										else if (ewishEffectAwardType == CoupleWishManager.EWishEffectAwardType.BangZuan)
										{
											effectData.GetBindZuanShi = Math.Max(0, Math.Min((int)(0.08 * (double)num2), val));
											num3 = effectData.GetBindZuanShi;
										}
										else
										{
											if (ewishEffectAwardType != CoupleWishManager.EWishEffectAwardType.Exp)
											{
												continue;
											}
											effectData.GetExp = Math.Max(0, Math.Min(4000 * num2, val));
											num3 = effectData.GetExp;
										}
										if (effectData.GetBinJinBi > 0)
										{
											GameManager.ClientMgr.AddMoney1(nextClient, effectData.GetBinJinBi, "情侣祝福特效", true);
											string textMsg = string.Format(GLang.GetLang(481, new object[0]), effectData.From.RoleName, num3);
											GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, textMsg);
										}
										if (effectData.GetBindZuanShi > 0)
										{
											GameManager.ClientMgr.AddUserGold(nextClient, effectData.GetBindZuanShi, "情侣祝福特效");
											string textMsg = string.Format(GLang.GetLang(482, new object[0]), effectData.From.RoleName, num3);
											GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, textMsg);
										}
										if (effectData.GetExp > 0)
										{
											GameManager.ClientMgr.ProcessRoleExperience(nextClient, (long)effectData.GetExp, false, true, false, "none");
											GameManager.ClientMgr.NotifyAddExpMsg(nextClient, (long)effectData.GetExp);
											string textMsg = string.Format(GLang.GetLang(483, new object[0]), effectData.From.RoleName, num3);
											GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, textMsg);
										}
										nextClient.sendCmd<CoupleWishNtfWishEffectData>(1393, effectData, false);
										this.SaveGetNextEffectAward(nextClient, day, ewishEffectAwardType, num3);
									}
								}
							}
						}
					}
				}
			}
		}

		public bool PreClearDivorceData(int man, int wife)
		{
			return TianTiClient.getInstance().CoupleWishPreDivorce(man, wife) >= 0;
		}

		private void TimerProc(object sender, EventArgs e)
		{
			CoupleWishSyncData coupleWishSyncData = TianTiClient.getInstance().CoupleWishSyncCenterData(this.SyncData.ThisWeek.ModifyTime, this.SyncData.LastWeek.ModifyTime, this.SyncData.Statue.ModifyTime);
			if (coupleWishSyncData != null)
			{
				lock (this.Mutex)
				{
					if (coupleWishSyncData.ThisWeek.ModifyTime != this.SyncData.ThisWeek.ModifyTime)
					{
						this.SyncData.ThisWeek = coupleWishSyncData.ThisWeek;
						this.ThisWeekTopNList.Clear();
						foreach (CoupleWishCoupleDataK coupleWishCoupleDataK in this.SyncData.ThisWeek.RankList)
						{
							if (coupleWishCoupleDataK.Rank > CoupleWishConsts.MaxRankNum)
							{
								break;
							}
							CoupleWishCoupleData coupleWishCoupleData = new CoupleWishCoupleData();
							coupleWishCoupleData.DbCoupleId = coupleWishCoupleDataK.DbCoupleId;
							coupleWishCoupleData.Man = coupleWishCoupleDataK.Man;
							if (coupleWishCoupleDataK.ManSelector != null)
							{
								coupleWishCoupleData.ManSelector = DataHelper.BytesToObject<RoleData4Selector>(coupleWishCoupleDataK.ManSelector, 0, coupleWishCoupleDataK.ManSelector.Length);
							}
							coupleWishCoupleData.Wife = coupleWishCoupleDataK.Wife;
							if (coupleWishCoupleDataK.WifeSelector != null)
							{
								coupleWishCoupleData.WifeSelector = DataHelper.BytesToObject<RoleData4Selector>(coupleWishCoupleDataK.WifeSelector, 0, coupleWishCoupleDataK.WifeSelector.Length);
							}
							coupleWishCoupleData.BeWishedNum = coupleWishCoupleDataK.BeWishedNum;
							coupleWishCoupleData.Rank = coupleWishCoupleDataK.Rank;
							this.ThisWeekTopNList.Add(coupleWishCoupleData);
						}
					}
					if (coupleWishSyncData.LastWeek.ModifyTime != this.SyncData.LastWeek.ModifyTime)
					{
						this.SyncData.LastWeek = coupleWishSyncData.LastWeek;
					}
					if (coupleWishSyncData.Statue.ModifyTime != this.SyncData.Statue.ModifyTime)
					{
						this.SyncData.Statue = coupleWishSyncData.Statue;
						this.StatueMgr.SetDiaoXiang(this.SyncData.Statue);
					}
				}
			}
		}

		private object Mutex = new object();

		private CoupleWishSyncData SyncData = new CoupleWishSyncData();

		private List<CoupleWishCoupleData> ThisWeekTopNList = new List<CoupleWishCoupleData>();

		public readonly CoupleWishConfig _Config = new CoupleWishConfig();

		private CoupleWishStatueManager StatueMgr = new CoupleWishStatueManager();

		private Dictionary<CoupleWishManager.EWishEffectAwardType, int> WishEffectDayMaxAward = new Dictionary<CoupleWishManager.EWishEffectAwardType, int>
		{
			{
				CoupleWishManager.EWishEffectAwardType.BangJin,
				60000
			},
			{
				CoupleWishManager.EWishEffectAwardType.BangZuan,
				10000
			},
			{
				CoupleWishManager.EWishEffectAwardType.Exp,
				1000000
			}
		};

		private HashSet<int> CanEffectAwardMap = new HashSet<int>();

		private enum EWishEffectAwardType
		{
			BangJin,
			BangZuan,
			Exp,
			Max,
			None = 99
		}
	}
}
