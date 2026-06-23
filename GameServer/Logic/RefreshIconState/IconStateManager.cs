using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.Building;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Marriage.CoupleWish;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic.RefreshIconState
{
	public class IconStateManager
	{
		public bool AddFlushIconState(ushort nIconOrder, bool bIconState)
		{
			ushort iState = bIconState ? 1 : 0;
			return this.AddFlushIconState(nIconOrder, iState);
		}

		public bool AddFlushIconState(ushort nIconOrder, ushort iState)
		{
			ushort value = (ushort)(((int)nIconOrder << 1) + (int)iState);
			ushort num = 0;
			bool result;
			lock (this.m_StateIconsDict)
			{
				if (!this.m_StateCacheIconsDict.TryGetValue(nIconOrder, out num))
				{
					this.m_StateCacheIconsDict[nIconOrder] = value;
					this.m_StateIconsDict[nIconOrder] = value;
					result = true;
				}
				else if ((num & 1) == iState)
				{
					result = false;
				}
				else
				{
					this.m_StateCacheIconsDict[nIconOrder] = value;
					this.m_StateIconsDict[nIconOrder] = value;
					result = true;
				}
			}
			return result;
		}

		public void ResetIconStateDict(bool bIsLogin)
		{
			lock (this.m_StateIconsDict)
			{
				if (bIsLogin)
				{
					this.m_StateCacheIconsDict.Clear();
				}
				this.m_StateIconsDict.Clear();
			}
		}

		public void SendIconStateToClient(GameClient client)
		{
			ushort[] array = null;
			lock (this.m_StateIconsDict)
			{
				int num = this.m_StateIconsDict.Count<KeyValuePair<ushort, ushort>>();
				if (num > 0)
				{
					array = new ushort[num];
					num = 0;
					foreach (KeyValuePair<ushort, ushort> keyValuePair in this.m_StateIconsDict)
					{
						array[num++] = keyValuePair.Value;
					}
				}
				if (array != null && array.Length > 0)
				{
					this.m_ActivityIconStateData.arrIconState = array;
					client.sendCmd<ActivityIconStateData>(614, this.m_ActivityIconStateData, false);
					this.ResetIconStateDict(false);
				}
			}
		}

		public void LoginGameFlushIconState(GameClient client)
		{
			this.ResetIconStateDict(true);
			this.CheckHuangJinBoss(client);
			this.CheckShiJieBoss(client);
			this.CheckHuoDongState(client);
			this.CheckFuLiMeiRiHuoYue(client);
			this.CheckFuLiLianXuDengLu(client);
			this.CheckFuLiLeiJiDengLu(client);
			this.CheckFuMeiRiZaiXian(client);
			this.CheckFuUpLevelGift(client);
			this.CheckFuLiYueKaFanLi(client);
			this.CheckCombatGift(client);
			this.FlushChongZhiIconState(client);
			this.FlushUsedMoneyconState(client);
			this.CheckJingJiChangLeftTimes(client);
			this.CheckJingJiChangJiangLi(client);
			this.CheckJingJiChangJunXian(client);
			this.CheckZiYuanZhaoHui(client);
			this.CheckEmailCount(client, false);
			this.CheckFreeImpetrateState(client);
			this.CheckChengJiuUpLevelState(client);
			this.CheckPaiHangState(client);
			this.CheckVIPLevelAwardState(client);
			this.CheckThemeActivity(client);
			this.CheckReborn(client);
			this.CheckHeFuActivity(client);
			this.CheckJieRiActivity(client, true);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckSpecPriorityActivity(client);
			this.CheckGuildIcon(client, true);
			this.CheckGuildIcon(client, true);
			this.CheckPetIcon(client);
			this.CheckBuildingIcon(client, true);
			this.CheckJunTuanEraIcon(client);
			this.SendIconStateToClient(client);
			this.CheckBuChangState(client);
			this.CheckCaiJiState(client);
			GameManager.MerlinMagicBookMgr.CheckMerlinSecretAttr(client);
			this.CheckFreeZhuanPanChouState(client);
			this.CheckShenYouAwardIcon(client);
			this.CheckFuMoMailIcon(client);
		}

		public bool FlushChongZhiIconState(GameClient client)
		{
			this.CheckShouCiChongZhi(client);
			this.CheckMeiRiChongZhi(client);
			this.CheckLeiJiChongZhi(client);
			this.CheckOneDollarChongZhi(client);
			this.CheckInputFanLiNewActivity(client);
			this.CheckOneDollarBuy(client);
			this.CheckXinFuChongZhiMoney(client);
			this.CheckXinFuFreeGetMoney(client);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckSpecPriorityActivity(client);
			return false;
		}

		public bool FlushUsedMoneyconState(GameClient client)
		{
			this.CheckLeiJiXiaoFei(client);
			this.CheckXinFuUseMoney(client);
			this.CheckSpecialActivity(client);
			this.CheckEverydayActivity(client);
			this.CheckInputFanLiNewActivity(client);
			return false;
		}

		public bool CheckFuLiMeiRiHuoYue(GameClient client)
		{
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.systemDailyActiveAward.SystemXmlItemDict)
			{
				int num = Math.Max(0, keyValuePair.Value.GetIntValue("NeedhuoYue", -1));
				int intValue = keyValuePair.Value.GetIntValue("ID", -1);
				if (num <= client.ClientData.DailyActiveValues)
				{
					if (DailyActiveManager.IsDailyActiveAwardFetched(client, intValue) <= 0)
					{
						return this.AddFlushIconState(3006, true);
					}
				}
			}
			return this.AddFlushIconState(3006, false);
		}

		public bool CheckFuLiLianXuDengLuReward(GameClient client)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			bool result = true;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == dayOfYear && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep <= client.ClientData.SeriesLoginNum)
			{
				result = false;
			}
			return result;
		}

		public bool CheckFuLiLianXuDengLu(GameClient client)
		{
			bool bIconState = this.CheckFuLiLianXuDengLuReward(client);
			return this.AddFlushIconState(3007, bIconState);
		}

		public bool CheckFuLiLeiJiDengLuReward(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "TotalLoginAwardFlag");
			int chengJiuExtraDataByField = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
			int totalLoginInfoNum = Data.GetTotalLoginInfoNum();
			bool result = false;
			int num = 0;
			while (num < 7 && num < chengJiuExtraDataByField && num < totalLoginInfoNum)
			{
				if ((roleParamsInt32FromDB & 1 << num + 1) == 0)
				{
					result = true;
					break;
				}
				num++;
			}
			if (chengJiuExtraDataByField == 30)
			{
				if ((roleParamsInt32FromDB & 1024) == 0)
				{
					result = true;
				}
			}
			if (chengJiuExtraDataByField == 21)
			{
				if ((roleParamsInt32FromDB & 512) == 0)
				{
					result = true;
				}
			}
			if (chengJiuExtraDataByField == 14)
			{
				if ((roleParamsInt32FromDB & 256) == 0)
				{
					result = true;
				}
			}
			return result;
		}

		public bool CheckShenYouAwardIcon(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (GoodsUtil.GetMeditateBagGoodsCnt(client) > 0)
			{
				result = this.AddFlushIconState(3036, true);
			}
			else
			{
				result = this.AddFlushIconState(3036, false);
			}
			return result;
		}

		public bool CheckFuMoMailIcon(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				string cmd = string.Format("{0}", client.ClientData.RoleID);
				int num = Global.sendToDB<int, string>(14103, cmd, client.ServerId);
				if (num > 0)
				{
					if (this.AddFlushIconState(3037, true))
					{
						this.SendIconStateToClient(client);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = this.AddFlushIconState(3037, false);
				}
			}
			return result;
		}

		public bool CheckFuLiLeiJiDengLu(GameClient client)
		{
			bool bIconState = this.CheckFuLiLeiJiDengLuReward(client);
			return this.AddFlushIconState(3008, bIconState);
		}

		public bool CheckFuLiYueKaFanLi(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				bool flag = this.CheckFuLiYueKaFanLiAward(client);
				int num = client.ClientData.YKDetail.CurDayOfPerYueKa() - 1;
				if (client.ClientData.YKDetail.HasYueKa == 1 && num >= 0 && num < client.ClientData.YKDetail.AwardInfo.Length && client.ClientData.YKDetail.AwardInfo[num] == '1')
				{
					result = (flag | this.AddFlushIconState(3013, false));
				}
				else
				{
					result = (flag | this.AddFlushIconState(3013, true));
				}
			}
			return result;
		}

		public bool CheckFuLiYueKaFanLiAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				int num = client.ClientData.YKDetail.CurDayOfPerYueKa() - 1;
				if (client.ClientData.YKDetail.HasYueKa == 1 && num >= 0 && num < client.ClientData.YKDetail.AwardInfo.Length && client.ClientData.YKDetail.AwardInfo[num] == '0')
				{
					result = this.AddFlushIconState(3015, true);
				}
				else
				{
					result = this.AddFlushIconState(3015, false);
				}
			}
			return result;
		}

		public bool CheckFuMeiRiZaiXian(GameClient client)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != dayOfYear)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = dayOfYear;
			}
			int everyDayOnLineAwardStep = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
			int everyDayOnLineItemCount = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			bool result;
			if (everyDayOnLineItemCount == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				result = this.AddFlushIconState(3009, false);
			}
			else
			{
				int num = everyDayOnLineItemCount - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				for (int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1; i <= everyDayOnLineItemCount; i++)
				{
					EveryDayOnLineAward everyDayOnLineItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == everyDayOnLineItem)
					{
						return false;
					}
					if (client.ClientData.DayOnlineSecond >= everyDayOnLineItem.TimeSecs)
					{
						return this.AddFlushIconState(3009, true);
					}
				}
				result = this.AddFlushIconState(3009, false);
			}
			return result;
		}

		public bool CheckCombatGift(GameClient client)
		{
			long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10154");
			bool bIconState = false;
			for (int i = 0; i < HuodongCachingMgr.CombatGiftMaxVal; i++)
			{
				if (Global.GetLongSomeBit(roleParamsInt64FromDB, i * 2) == 1L && Global.GetLongSomeBit(roleParamsInt64FromDB, i * 2 + 1) == 0L)
				{
					bIconState = true;
					break;
				}
			}
			return this.AddFlushIconState(3014, bIconState);
		}

		public bool CheckFuUpLevelGift(GameClient client)
		{
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
			bool bIconState = false;
			for (int i = 0; i < roleParamsIntListFromDB.Count * 16; i++)
			{
				if (Global.GetBitValue(roleParamsIntListFromDB, i * 2) == 1 && Global.GetBitValue(roleParamsIntListFromDB, i * 2 + 1) == 0)
				{
					bIconState = true;
					break;
				}
			}
			return this.AddFlushIconState(3010, bIconState);
		}

		public bool CheckOneDollarBuy(GameClient client)
		{
			OneDollarBuyActivity oneDollarBuyActivity = HuodongCachingMgr.GetOneDollarBuyActivity();
			bool result;
			if (null == oneDollarBuyActivity)
			{
				result = this.AddFlushIconState(15052, false);
			}
			else
			{
				result = this.AddFlushIconState(15052, oneDollarBuyActivity.CheckClientCanBuy(client));
			}
			return result;
		}

		public bool CheckFuLiChongZhiHuiKui(GameClient client)
		{
			bool flag = this.CheckShouCiChongZhi(client);
			bool flag2 = this.CheckMeiRiChongZhi(client);
			bool flag3 = this.CheckLeiJiChongZhi(client);
			bool flag4 = this.CheckLeiJiXiaoFei(client);
			bool result;
			if (flag || flag2 || flag3 || flag4)
			{
				result = this.AddFlushIconState(3001, true);
			}
			else
			{
				result = this.AddFlushIconState(3001, false);
			}
			return result;
		}

		public bool CheckShouCiChongZhi(GameClient client)
		{
			int num = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			if (num > 0)
			{
				if (Global.CanGetFirstChongZhiDaLiByUserID(client))
				{
					this.AddFlushIconState(3011, 0);
					return this.AddFlushIconState(3002, 1);
				}
				this.AddFlushIconState(3011, 1);
			}
			else
			{
				this.AddFlushIconState(3011, 0);
			}
			return this.AddFlushIconState(3002, 0);
		}

		public bool CheckOneDollarChongZhi(GameClient client)
		{
			bool flag;
			bool bIconState = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.OneDollarChongZhi, out flag);
			return this.AddFlushIconState(15051, bIconState);
		}

		public bool CheckInputFanLiNewActivity(GameClient client)
		{
			bool flag;
			bool bIconState = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.InputFanLiNew, out flag);
			return this.AddFlushIconState(15054, bIconState);
		}

		public bool CheckMeiRiChongZhi(GameClient client)
		{
			bool bIconState2;
			bool bIconState = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.MeiRiChongZhiHaoLi, out bIconState2);
			this.AddFlushIconState(3012, bIconState2);
			WeedEndInputActivity weekEndInputActivity = HuodongCachingMgr.GetWeekEndInputActivity();
			if (weekEndInputActivity != null && weekEndInputActivity.InAwardTime())
			{
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (weekEndInputActivity.GetWeekEndInputOpenDay(client) != offsetDay)
				{
					bIconState = true;
				}
			}
			return this.AddFlushIconState(3003, bIconState);
		}

		public bool CheckLeiJiChongZhi(GameClient client)
		{
			bool flag;
			bool bIconState = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalCharge, out flag);
			return this.AddFlushIconState(3004, bIconState);
		}

		public bool CheckLeiJiXiaoFei(GameClient client)
		{
			bool flag;
			bool bIconState = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalConsume, out flag);
			return this.AddFlushIconState(3005, bIconState);
		}

		public bool CheckMainXinFuIcon(GameClient client)
		{
			return false;
		}

		public bool CheckXinFuKillBoss(GameClient client)
		{
			return false;
		}

		public bool CheckXinFuChongZhiMoney(GameClient client)
		{
			return false;
		}

		public bool CheckXinFuUseMoney(GameClient client)
		{
			return false;
		}

		public bool CheckXinFuFreeGetMoney(GameClient client)
		{
			return false;
		}

		public bool CheckJingJiChangLeftTimes(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().checkEnterNum(client, JingJiChangConstants.Enter_Type_Free) == ResultCode.Success)
			{
				result = this.AddFlushIconState(4003, true);
			}
			else
			{
				result = this.AddFlushIconState(4003, false);
			}
			return result;
		}

		public bool CheckJingJiChangJiangLi(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().CanGetrankingReward(client))
			{
				result = this.AddFlushIconState(4001, true);
			}
			else
			{
				result = this.AddFlushIconState(4001, false);
			}
			return result;
		}

		public bool CheckJingJiChangJunXian(GameClient client)
		{
			bool result;
			if (JingJiChangManager.getInstance().CanGradeJunXian(client))
			{
				result = this.AddFlushIconState(4002, true);
			}
			else
			{
				result = this.AddFlushIconState(4002, false);
			}
			return result;
		}

		public bool CheckShiJieBoss(GameClient client)
		{
			bool result;
			if (TimerBossManager.getInstance().HaveWorldBoss(client))
			{
				result = this.AddFlushIconState(1002, true);
			}
			else
			{
				result = this.AddFlushIconState(1002, false);
			}
			return result;
		}

		public bool CheckHuoDongState(GameClient client)
		{
			bool result;
			if (GameManager.AngelTempleMgr.CanEnterAngelTempleOnTime())
			{
				result = this.AddFlushIconState(1007, true);
			}
			else
			{
				this.AddFlushIconState(1007, false);
				result = true;
			}
			return result;
		}

		public bool CheckHuangJinBoss(GameClient client)
		{
			bool result;
			if (TimerBossManager.getInstance().HaveHuangJinBoss(client))
			{
				result = this.AddFlushIconState(1005, true);
			}
			else
			{
				result = this.AddFlushIconState(1005, false);
			}
			return result;
		}

		public bool CheckZiYuanZhaoHui(GameClient client)
		{
			bool result;
			if (CGetOldResourceManager.HasOldResource(client))
			{
				result = this.AddFlushIconState(7001, true);
			}
			else
			{
				result = this.AddFlushIconState(7001, false);
			}
			return result;
		}

		public bool CheckEmailCount(GameClient client, bool sendToClient = true)
		{
			string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 1, 1);
			int num = Global.sendToDB<int, string>(649, cmd, client.ServerId);
			bool flag;
			if (num > 0)
			{
				flag = this.AddFlushIconState(5002, true);
			}
			else
			{
				flag = this.AddFlushIconState(5002, false);
			}
			if (flag && sendToClient)
			{
				this.SendIconStateToClient(client);
			}
			return flag;
		}

		public bool CheckPaiHangState(GameClient client)
		{
			return this.AddFlushIconState(7500, Global.GetAdmireCount(client) < 10);
		}

		public bool CheckChengJiuUpLevelState(GameClient client)
		{
			bool result = this.AddFlushIconState(9000, ChengJiuManager.CanActiveNextChengHao(client));
			this.SendIconStateToClient(client);
			return result;
		}

		public bool CheckVIPLevelAwardState(GameClient client)
		{
			for (int i = 1; i <= client.ClientData.VipLevel; i++)
			{
				int num = client.ClientData.VipAwardFlag & Global.GetBitValue(i + 1);
				if (num < 1)
				{
					return this.AddFlushIconState(10001, true);
				}
			}
			return this.AddFlushIconState(10001, false);
		}

		public bool CheckFreeImpetrateState(GameClient client)
		{
			bool bIconState = false;
			DateTime d = TimeUtil.NowDateTime();
			DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "ImpetrateTime");
			double totalSeconds = (d - roleParamsDateTimeFromDB).TotalSeconds;
			double num = Global.GMax(0.0, (double)Data.FreeImpetrateIntervalTime - totalSeconds);
			if (num <= 0.0)
			{
				bIconState = true;
			}
			return this.AddFlushIconState(8000, bIconState);
		}

		public bool CheckBuChangState(GameClient client)
		{
			bool bIconState = BuChangManager.CheckGiveBuChang(client);
			return this.AddFlushIconState(11000, bIconState);
		}

		public bool CheckRebornUpgrade(GameClient client)
		{
			return this.AddFlushIconState(21001, RebornManager.getInstance().CheckRebornUpgradeIcon(client));
		}

		public bool CheckReborn(GameClient client)
		{
			this.AddFlushIconState(21001, false);
			bool flag = false;
			return this.CheckRebornUpgrade(client) || flag;
		}

		public bool CheckThemeActivity(GameClient client)
		{
			this.AddFlushIconState(11501, false);
			this.AddFlushIconState(11502, false);
			bool flag = false;
			flag = (this.CheckThemeZhiGou(client) || flag);
			return this.CheckThemeDaLiBao(client) || flag;
		}

		public bool CheckThemeZhiGou(GameClient client)
		{
			ThemeZhiGouActivity themeZhiGouActivity = HuodongCachingMgr.GetThemeZhiGouActivity();
			bool result;
			if (null == themeZhiGouActivity)
			{
				result = false;
			}
			else if (!themeZhiGouActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = themeZhiGouActivity.CheckClientCanBuy(client);
				result = this.AddFlushIconState(11501, bIconState);
			}
			return result;
		}

		public bool CheckRegressZhiGou(GameClient client)
		{
			RegressActiveDayBuy regressActiveDayBuy = HuodongCachingMgr.GetRegressActiveDayBuy();
			bool result;
			if (null == regressActiveDayBuy)
			{
				result = false;
			}
			else if (!regressActiveDayBuy.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = regressActiveDayBuy.CheckClientCanBuy(client);
				result = this.AddFlushIconState(15058, bIconState);
			}
			return result;
		}

		public bool CheckThemeDaLiBao(GameClient client)
		{
			bool bIconState = false;
			ThemeDaLiBaoActivity themeDaLiBaoActivity = HuodongCachingMgr.GetThemeDaLiBaoActivity();
			bool result;
			if (null == themeDaLiBaoActivity)
			{
				result = false;
			}
			else if (!themeDaLiBaoActivity.InActivityTime())
			{
				result = false;
			}
			else
			{
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 908, Global.GetActivityRequestCmdString(ActivityTypes.ThemeDaLiBao, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 3 == array.Length)
					{
						int num = Convert.ToInt32(array[2]);
						if (num == 0)
						{
							bIconState = true;
						}
					}
				}
				result = this.AddFlushIconState(11502, bIconState);
			}
			return result;
		}

		public bool CheckHeFuActivity(GameClient client)
		{
			this.AddFlushIconState(12001, false);
			this.AddFlushIconState(12002, false);
			this.AddFlushIconState(12003, false);
			this.AddFlushIconState(12004, false);
			this.AddFlushIconState(12005, false);
			bool flag = false;
			flag = (this.CheckHeFuLogin(client) || flag);
			flag = (this.CheckHeFuTotalLogin(client) || flag);
			flag = (this.CheckHeFuRecharge(client) || flag);
			flag = (this.CheckHeFuPKKing(client) || flag);
			flag = (this.CheckHeFuLuoLan(client) || flag);
			return this.AddFlushIconState(12000, flag);
		}

		public bool CheckHeFuLogin(GameClient client)
		{
			HeFuLoginActivity heFuLoginActivity = HuodongCachingMgr.GetHeFuLoginActivity();
			bool result;
			if (null == heFuLoginActivity)
			{
				result = false;
			}
			else if (!heFuLoginActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HeFuLoginFlag");
				int intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB, 1);
				if (intSomeBit != 0)
				{
					intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB, 2);
					if (intSomeBit == 0)
					{
						bIconState = true;
					}
					else
					{
						intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB, 3);
						if (intSomeBit == 0)
						{
							if (Global.IsVip(client))
							{
								bIconState = true;
							}
						}
					}
				}
				result = this.AddFlushIconState(12001, bIconState);
			}
			return result;
		}

		public bool CheckHeFuTotalLogin(GameClient client)
		{
			HeFuTotalLoginActivity heFuTotalLoginActivity = HuodongCachingMgr.GetHeFuTotalLoginActivity();
			bool result;
			if (null == heFuTotalLoginActivity)
			{
				result = false;
			}
			else if (!heFuTotalLoginActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginNum");
				for (int i = 1; i <= roleParamsInt32FromDB; i++)
				{
					if (heFuTotalLoginActivity.GetAward(i) != null)
					{
						int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginFlag");
						int intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB2, i);
						if (intSomeBit == 0)
						{
							bIconState = true;
							break;
						}
					}
				}
				result = this.AddFlushIconState(12002, bIconState);
			}
			return result;
		}

		public bool CheckHeFuRecharge(GameClient client)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			int offsetDay2 = Global.GetOffsetDay(Global.GetHefuStartDay());
			bool result;
			if (offsetDay == offsetDay2)
			{
				result = false;
			}
			else
			{
				HeFuRechargeActivity heFuRechargeActivity = HuodongCachingMgr.GetHeFuRechargeActivity();
				if (null == heFuRechargeActivity)
				{
					result = false;
				}
				else if (!heFuRechargeActivity.InActivityTime() && !heFuRechargeActivity.InAwardTime())
				{
					result = false;
				}
				else
				{
					bool bIconState = false;
					string[] array = null;
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10160, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						client.ClientData.RoleID,
						23,
						offsetDay2,
						Global.GetOffsetDay(DateTime.Parse(heFuRechargeActivity.ToDate)),
						heFuRechargeActivity.strcoe
					}), out array, client.ServerId);
					if (null != array)
					{
						if (array != null && 1 == array.Length)
						{
							string[] array2 = array[0].Split(new char[]
							{
								'|'
							});
							if (1 <= array.Length)
							{
								bIconState = (Convert.ToInt32(array2[0]) > 0);
							}
						}
					}
					result = this.AddFlushIconState(12003, bIconState);
				}
			}
			return result;
		}

		public bool CheckHeFuPKKing(GameClient client)
		{
			HeFuPKKingActivity heFuPKKingActivity = HuodongCachingMgr.GetHeFuPKKingActivity();
			bool result;
			if (null == heFuPKKingActivity)
			{
				result = false;
			}
			else if (!heFuPKKingActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				if (client.ClientData.RoleID == HuodongCachingMgr.GetHeFuPKKingRoleID())
				{
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HeFuPKKingFlag");
					if (roleParamsInt32FromDB == 0)
					{
						bIconState = true;
					}
				}
				result = this.AddFlushIconState(12004, bIconState);
			}
			return result;
		}

		public bool CheckHeFuLuoLan(GameClient client)
		{
			HeFuLuoLanActivity heFuLuoLanActivity = HuodongCachingMgr.GetHeFuLuoLanActivity();
			bool result;
			if (null == heFuLuoLanActivity)
			{
				result = false;
			}
			else if (!heFuLuoLanActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
				string[] array = gameConfigItemStr.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (2 == array2.Length)
					{
						if (Convert.ToInt32(array2[0]) == client.ClientData.Faction)
						{
							num++;
							if (Convert.ToInt32(array2[1]) != client.ClientData.RoleID)
							{
								num3++;
							}
						}
						if (Convert.ToInt32(array2[1]) == client.ClientData.RoleID)
						{
							num2++;
						}
					}
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HeFuLuoLanAwardFlag");
				foreach (KeyValuePair<int, HeFuLuoLanAward> keyValuePair in heFuLuoLanActivity.HeFuLuoLanAwardDict)
				{
					HeFuLuoLanAward value = keyValuePair.Value;
					if (1 == value.status)
					{
						if (num2 >= value.winNum)
						{
							int intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB, keyValuePair.Key);
							if (0 == intSomeBit)
							{
								bIconState = true;
								break;
							}
						}
					}
					else if (2 == value.status)
					{
						if (num3 >= value.winNum)
						{
							int intSomeBit = Global.GetIntSomeBit(roleParamsInt32FromDB, keyValuePair.Key);
							if (0 == intSomeBit)
							{
								bIconState = true;
								break;
							}
						}
					}
				}
				result = this.AddFlushIconState(12005, bIconState);
			}
			return result;
		}

		public bool CheckCaiJiState(GameClient client)
		{
			return this.AddFlushIconState(13000, CaiJiLogic.HasLeftnum(client));
		}

		public bool CheckSpecialActivity(GameClient client)
		{
			SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
			bool result;
			if (null == specialActivity)
			{
				result = false;
			}
			else
			{
				bool bIconState = specialActivity.CheckIconState(client);
				result = this.AddFlushIconState(14110, bIconState);
			}
			return result;
		}

		public bool CheckEverydayActivity(GameClient client)
		{
			EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
			bool result;
			if (null == everydayActivity)
			{
				result = false;
			}
			else
			{
				bool bIconState = everydayActivity.CheckIconState(client);
				result = this.AddFlushIconState(14114, bIconState);
			}
			return result;
		}

		public bool CheckSpecPriorityActivity(GameClient client)
		{
			SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
			bool result;
			if (null == specPriorityActivity)
			{
				result = false;
			}
			else
			{
				bool bIconState = specPriorityActivity.CheckIconState(client);
				result = this.AddFlushIconState(14115, bIconState);
			}
			return result;
		}

		public bool CheckJieRiActivity(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(14000, false);
				this.AddFlushIconState(14001, false);
				this.AddFlushIconState(14002, false);
				this.AddFlushIconState(14003, false);
				this.AddFlushIconState(14004, false);
				this.AddFlushIconState(14005, false);
				this.AddFlushIconState(14006, false);
				this.AddFlushIconState(14007, false);
				this.AddFlushIconState(14008, false);
				this.AddFlushIconState(14009, false);
				this.AddFlushIconState(14010, false);
				this.AddFlushIconState(14011, false);
				this.AddFlushIconState(14012, false);
				this.AddFlushIconState(14013, false);
				this.AddFlushIconState(14014, false);
				this.AddFlushIconState(14015, false);
				this.AddFlushIconState(14016, false);
				this.AddFlushIconState(14017, false);
				this.AddFlushIconState(14018, false);
				this.AddFlushIconState(14019, false);
				this.AddFlushIconState(14020, false);
				this.AddFlushIconState(14021, false);
				this.AddFlushIconState(14023, false);
				this.AddFlushIconState(14027, false);
				this.AddFlushIconState(14028, false);
				this.AddFlushIconState(14033, false);
				this.AddFlushIconState(14034, false);
				this.AddFlushIconState(14035, false);
			}
			bool flag = false;
			flag |= this.CheckJieRiLogin(client);
			flag |= this.CheckJieRiTotalLogin(client);
			flag |= this.CheckJieRiDayCZ(client);
			flag |= this.CheckJieRiLeiJiXF(client);
			flag |= this.CheckJieRiLeiJiCZ(client);
			flag |= this.CheckJieRiCZKING(client);
			flag |= this.CheckJieRiXFKING(client);
			flag |= this.CheckJieRiLeiJi(client);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriWing);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAddon);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriStrengthen);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAchievement);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriMilitaryRank);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriVIPFanli);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriAmulet);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriArchangel);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieRiHuiJi);
			flag |= this.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen);
			flag |= this.CheckJieriGive(client);
			flag |= this.CheckJieriGiveKing(client);
			flag |= this.CheckJieriRecvKing(client);
			flag |= this.CheckJieriLianXuCharge(client);
			flag |= this.CheckJieriRecv(client);
			flag |= this.CheckJieriIPointsExchg(client);
			flag |= this.CheckJieriDanBiChongZhi(client);
			flag |= this.CheckJieRiHongBaoBang(client);
			flag |= this.CheckJieRiPCKingEveryDay(client);
			bool bIconState = this.IsAnyJieRiTipActived();
			return this.AddFlushIconState(14000, bIconState) || flag;
		}

		public bool IsAnyJieRiTipActived()
		{
			return this.IsAnyTipActived(IconStateManager.m_jieRiIconList);
		}

		public bool IsAnyTipActived(List<ActivityTipTypes> iconTipList)
		{
			bool result = false;
			if (iconTipList != null)
			{
				lock (this.m_StateCacheIconsDict)
				{
					foreach (ActivityTipTypes activityTipTypes in iconTipList)
					{
						ushort num = 0;
						if (this.m_StateCacheIconsDict.TryGetValue((ushort)activityTipTypes, out num))
						{
							if ((num & 1) == 1)
							{
								result = true;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public bool CheckJieRiLogin(GameClient client)
		{
			JieriDaLiBaoActivity jieriDaLiBaoActivity = HuodongCachingMgr.GetJieriDaLiBaoActivity();
			bool result;
			if (null == jieriDaLiBaoActivity)
			{
				result = false;
			}
			else if (!jieriDaLiBaoActivity.InActivityTime() && !jieriDaLiBaoActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 461, Global.GetActivityRequestCmdString(ActivityTypes.JieriDaLiBao, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 3 == array.Length)
					{
						int num = Convert.ToInt32(array[2]);
						if (num == 0)
						{
							bIconState = true;
						}
					}
				}
				result = this.AddFlushIconState(14001, bIconState);
			}
			return result;
		}

		public bool CheckJieRiTotalLogin(GameClient client)
		{
			JieRiDengLuActivity jieRiDengLuActivity = HuodongCachingMgr.GetJieRiDengLuActivity();
			bool result;
			if (null == jieRiDengLuActivity)
			{
				result = false;
			}
			else if (!jieRiDengLuActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 462, Global.GetActivityRequestCmdString(ActivityTypes.JieriDengLuHaoLi, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 4 == array.Length)
					{
						int resource = Convert.ToInt32(array[2]);
						int num = Convert.ToInt32(array[3]);
						for (int i = 0; i < num; i++)
						{
							if (jieRiDengLuActivity.GetAward(client, i + 1) != null)
							{
								int intSomeBit = Global.GetIntSomeBit(resource, i);
								if (intSomeBit == 0)
								{
									bIconState = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14002, bIconState);
			}
			return result;
		}

		public bool CheckJieRiDayCZ(GameClient client)
		{
			JieriCZSongActivity jieriCZSongActivity = HuodongCachingMgr.GetJieriCZSongActivity();
			bool result;
			if (null == jieriCZSongActivity)
			{
				result = false;
			}
			else if (!jieriCZSongActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 464, Global.GetActivityRequestCmdString(ActivityTypes.JieriCZSong, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 5 == array.Length)
					{
						int num = Convert.ToInt32(array[3]);
						int resource = Convert.ToInt32(array[4]);
						foreach (KeyValuePair<int, AwardItem> keyValuePair in jieriCZSongActivity.AwardItemDict)
						{
							if (num >= keyValuePair.Value.MinAwardCondionValue)
							{
								int intSomeBit = Global.GetIntSomeBit(resource, keyValuePair.Key - 1);
								if (intSomeBit == 0)
								{
									bIconState = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14003, bIconState);
			}
			return result;
		}

		public bool CheckJieRiLeiJiXF(GameClient client)
		{
			JieRiTotalConsumeActivity jieRiTotalConsumeActivity = HuodongCachingMgr.GetJieRiTotalConsumeActivity();
			bool result;
			if (null == jieRiTotalConsumeActivity)
			{
				result = false;
			}
			else if (!jieRiTotalConsumeActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 683, Global.GetActivityRequestCmdString(ActivityTypes.JieriTotalConsume, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 4 == array.Length)
					{
						int num = Convert.ToInt32(array[2]);
						int resource = Convert.ToInt32(array[3]);
						foreach (KeyValuePair<int, AwardItem> keyValuePair in jieRiTotalConsumeActivity.AwardItemDict)
						{
							if (num >= keyValuePair.Value.MinAwardCondionValue)
							{
								int intSomeBit = Global.GetIntSomeBit(resource, keyValuePair.Key - 1);
								if (intSomeBit == 0)
								{
									bIconState = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14004, bIconState);
			}
			return result;
		}

		public bool CheckJieRiLeiJiCZ(GameClient client)
		{
			JieRiLeiJiCZActivity jieRiLeiJiCZActivity = HuodongCachingMgr.GetJieRiLeiJiCZActivity();
			bool result;
			if (null == jieRiLeiJiCZActivity)
			{
				result = false;
			}
			else if (!jieRiLeiJiCZActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 465, Global.GetActivityRequestCmdString(ActivityTypes.JieriLeiJiCZ, client, 0), out array, client.ServerId);
				if (null != array)
				{
					if (array != null && 4 == array.Length)
					{
						int num = Convert.ToInt32(array[2]);
						int resource = Convert.ToInt32(array[3]);
						foreach (KeyValuePair<int, AwardItem> keyValuePair in jieRiLeiJiCZActivity.AwardItemDict)
						{
							if (num >= keyValuePair.Value.MinAwardCondionValue)
							{
								int intSomeBit = Global.GetIntSomeBit(resource, keyValuePair.Key - 1);
								if (intSomeBit == 0)
								{
									bIconState = true;
									break;
								}
							}
						}
					}
				}
				result = this.AddFlushIconState(14005, bIconState);
			}
			return result;
		}

		public bool CheckJieRiLeiJi(GameClient client)
		{
			JieRiMeiRiLeiJiActivity jieriMeiRiLeiJiActivity = HuodongCachingMgr.GetJieriMeiRiLeiJiActivity();
			bool result;
			if (null == jieriMeiRiLeiJiActivity)
			{
				result = false;
			}
			else if (!jieriMeiRiLeiJiActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool flag = false;
				DateTime d = TimeUtil.NowDateTime();
				DateTime d2 = DateTime.Parse(jieriMeiRiLeiJiActivity.FromDate);
				int num = (int)(d - d2).TotalDays + 1;
				for (int i = 0; i < num; i++)
				{
					string[] array = null;
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 1806, Global.GetActivityRequestCmdString(ActivityTypes.JieRiMeiRiLeiJi, client, 1000 * (i + 1)), out array, client.ServerId);
					if (null == array)
					{
						break;
					}
					if (array == null || 4 != array.Length)
					{
						break;
					}
					int num2 = Convert.ToInt32(array[2]);
					int resource = Convert.ToInt32(array[3]);
					if (!jieriMeiRiLeiJiActivity.DayAwardItemDict.ContainsKey(i + 1))
					{
						break;
					}
					int num3 = 0;
					foreach (AwardItem awardItem in jieriMeiRiLeiJiActivity.DayAwardItemDict[i + 1])
					{
						if (num2 >= awardItem.MinAwardCondionValue)
						{
							int intSomeBit = Global.GetIntSomeBit(resource, num3);
							if (intSomeBit == 0)
							{
								flag = true;
								break;
							}
						}
						num3++;
					}
					if (flag)
					{
						break;
					}
				}
				result = this.AddFlushIconState(14028, flag);
			}
			return result;
		}

		public bool CheckJieRiHongBaoBang(GameClient client)
		{
			JieriHongBaoKingActivity instance = JieriHongBaoKingActivity.getInstance();
			bool bIconState = instance.CanGetAnyAward(client);
			return this.AddFlushIconState(14032, bIconState);
		}

		public bool CheckJieRiPCKingEveryDay(GameClient client)
		{
			bool flag = false;
			JieriPlatChargeKingEveryDay jieriPCKingEveryDayActivity = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
			if (null != jieriPCKingEveryDayActivity)
			{
				flag |= jieriPCKingEveryDayActivity.CanGetAnyAward(client);
			}
			return this.AddFlushIconState(14035, flag);
		}

		public bool CheckJieRiCZKING(GameClient client)
		{
			KingActivity jieRiCZKingActivity = HuodongCachingMgr.GetJieRiCZKingActivity();
			bool result;
			if (null == jieRiCZKingActivity)
			{
				result = false;
			}
			else if (!jieRiCZKingActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 468, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTCZKing, client, 1), out array, client.ServerId);
				if (array != null && 3 == array.Length)
				{
					int num = Convert.ToInt32(array[0]);
					int num2 = Convert.ToInt32(array[1]);
					int num3 = Convert.ToInt32(array[2]);
					if (1 == num)
					{
						if (num2 == client.ClientData.RoleID)
						{
							bIconState = (num3 == 1);
						}
					}
				}
				result = this.AddFlushIconState(14006, bIconState);
			}
			return result;
		}

		public bool CheckJieRiXFKING(GameClient client)
		{
			KingActivity jieriXiaoFeiKingActivity = HuodongCachingMgr.GetJieriXiaoFeiKingActivity();
			bool result;
			if (null == jieriXiaoFeiKingActivity)
			{
				result = false;
			}
			else if (!jieriXiaoFeiKingActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = false;
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 467, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTXiaoFeiKing, client, 1), out array, client.ServerId);
				if (array != null && 3 == array.Length)
				{
					int num = Convert.ToInt32(array[0]);
					int num2 = Convert.ToInt32(array[1]);
					int num3 = Convert.ToInt32(array[2]);
					if (1 == num)
					{
						if (num2 == client.ClientData.RoleID)
						{
							bIconState = (num3 == 1);
						}
					}
				}
				result = this.AddFlushIconState(14007, bIconState);
			}
			return result;
		}

		public bool CheckJieriGive(GameClient client)
		{
			JieriGiveActivity jieriGiveActivity = HuodongCachingMgr.GetJieriGiveActivity();
			bool result;
			if (jieriGiveActivity == null || !jieriGiveActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriGiveActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14008, bIconState);
			}
			return result;
		}

		public bool CheckJieriRecv(GameClient client)
		{
			JieriRecvActivity jieriRecvActivity = HuodongCachingMgr.GetJieriRecvActivity();
			bool result;
			if (jieriRecvActivity == null || !jieriRecvActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriRecvActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14021, bIconState);
			}
			return result;
		}

		public bool CheckJieriDanBiChongZhi(GameClient client)
		{
			DanBiChongZhiActivity danBiChongZhiActivity = HuodongCachingMgr.GetDanBiChongZhiActivity();
			bool result;
			if (danBiChongZhiActivity == null || !danBiChongZhiActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = danBiChongZhiActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14027, bIconState);
			}
			return result;
		}

		public bool CheckJieriIPointsExchg(GameClient client)
		{
			JieriIPointsExchgActivity jieriIPointsExchgActivity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			bool result;
			if (jieriIPointsExchgActivity == null || !jieriIPointsExchgActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriIPointsExchgActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14023, bIconState);
			}
			return result;
		}

		public bool CheckJieriGiveKing(GameClient client)
		{
			JieRiGiveKingActivity jieriGiveKingActivity = HuodongCachingMgr.GetJieriGiveKingActivity();
			bool result;
			if (jieriGiveKingActivity == null || !jieriGiveKingActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriGiveKingActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14009, bIconState);
			}
			return result;
		}

		public bool CheckJieriRecvKing(GameClient client)
		{
			JieRiRecvKingActivity jieriRecvKingActivity = HuodongCachingMgr.GetJieriRecvKingActivity();
			bool result;
			if (jieriRecvKingActivity == null || !jieriRecvKingActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriRecvKingActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14010, bIconState);
			}
			return result;
		}

		public bool CheckJieriLianXuCharge(GameClient client)
		{
			JieriLianXuChargeActivity jieriLianXuChargeActivity = HuodongCachingMgr.GetJieriLianXuChargeActivity();
			bool result;
			if (jieriLianXuChargeActivity == null || !jieriLianXuChargeActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				bool bIconState = jieriLianXuChargeActivity.CanGetAnyAward(client);
				result = this.AddFlushIconState(14020, bIconState);
			}
			return result;
		}

		public bool CheckGuildIcon(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(15000, false);
				this.AddFlushIconState(15001, false);
			}
			else
			{
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.InZhanMeng, -1);
			}
			bool flag = false;
			flag |= this.CheckGuildCopyMap(client);
			return this.AddFlushIconState(15000, flag);
		}

		public bool CheckGuildCopyMap(GameClient client)
		{
			bool bIconState = false;
			int num = -1;
			int num2 = -1;
			int mapcode = -1;
			GameManager.GuildCopyMapMgr.CheckCurrGuildCopyMap(client, out num, out num2, mapcode);
			bool result;
			if (num < 0)
			{
				result = false;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardFlag");
				for (int i = 0; i < GameManager.GuildCopyMapMgr.GuildCopyMapOrderList.Count; i++)
				{
					int num3 = GameManager.GuildCopyMapMgr.GuildCopyMapOrderList[i];
					if (num != 0)
					{
						bIconState = true;
						break;
					}
					if (num > 0 && num3 >= num)
					{
						break;
					}
					if (!GameManager.GuildCopyMapMgr.GetGuildCopyMapAwardDayFlag(roleParamsInt32FromDB, i, 2))
					{
						bIconState = true;
						break;
					}
				}
				result = this.AddFlushIconState(15001, bIconState);
			}
			return result;
		}

		public bool CheckPetIcon(GameClient client)
		{
			this.AddFlushIconState(16000, false);
			this.AddFlushIconState(16001, false);
			bool flag = false;
			flag |= this.CheckPetBagIcon(client);
			return flag | this.CheckCallPetIcon(client);
		}

		public bool CheckBuildingFreeQueue(GameClient client)
		{
			BuildingManager instance = BuildingManager.getInstance();
			int num = 0;
			int num2 = 0;
			instance.GetTaskNumInEachTeam(client, out num, out num2);
			return num < 4;
		}

		public bool CheckBuildingAward(GameClient client)
		{
			bool flag = false;
			BuildingManager instance = BuildingManager.getInstance();
			flag |= instance.CheckCanGetAnyAllLevelAward(client);
			return flag | instance.CheckAnyTaskFinish(client);
		}

		public bool CheckBuildingIcon(GameClient client, bool isLogin)
		{
			if (isLogin)
			{
				this.AddFlushIconState(15050, false);
			}
			bool flag = false;
			flag |= this.CheckBuildingFreeQueue(client);
			flag |= this.CheckBuildingAward(client);
			return this.AddFlushIconState(15050, flag);
		}

		public bool CheckJunTuanEraIcon(GameClient client)
		{
			return this.AddFlushIconState(15053, EraManager.getInstance().CheckJunTuanEraIcon(client));
		}

		public bool CheckKF5V5DDailyPaiHang(GameClient client)
		{
			DuanWeiRankAward duanWeiRankAward;
			bool result;
			if (TianTi5v5Manager.getInstance().CanGetMonthRankAwards(client, out duanWeiRankAward))
			{
				result = this.AddFlushIconState(15013, true);
			}
			else
			{
				result = this.AddFlushIconState(15013, false);
			}
			return result;
		}

		public bool CheckTianTiMonthPaiMingAwards(GameClient client)
		{
			ushort iState = 0;
			DuanWeiRankAward duanWeiRankAward = null;
			if (TianTiManager.getInstance().CanGetMonthRankAwards(client, out duanWeiRankAward))
			{
				iState = 1;
			}
			bool result;
			if (this.AddFlushIconState(1008, iState))
			{
				this.SendIconStateToClient(client);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool CheckPetBagIcon(GameClient client)
		{
			return false;
		}

		public bool CheckCallPetIcon(GameClient client)
		{
			bool bIconState = CallPetManager.getFreeSec(client) <= 0L;
			return this.AddFlushIconState(16001, bIconState);
		}

		public void DoSpriteIconTicks(GameClient client)
		{
			long num = TimeUtil.NOW();
			if (num >= this.m_LastTicksBuilding)
			{
				this.m_LastTicksBuilding = num + 5000L;
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			if (num >= this.m_LastTicks)
			{
				if (this.m_LastTicks == 0L)
				{
					this.m_LastTicks = num + 5000L;
				}
				else
				{
					this.m_LastTicks = num + 20000L;
					client._IconStateMgr.CheckPetIcon(client);
					client._IconStateMgr.CheckTianTiMonthPaiMingAwards(client);
					LangHunLingYuManager.getInstance().CheckTipsIconState(client);
					SingletonTemplate<ZhengBaManager>.Instance().CheckTipsIconState(client);
					SingletonTemplate<CoupleArenaManager>.Instance().CheckTipsIconState(client);
					SingletonTemplate<CoupleWishManager>.Instance().CheckTipsIconState(client);
					ZhengDuoManager.getInstance().CheckTipsIconState(client);
				}
			}
		}

		public bool CheckJieRiFanLi(GameClient client, ActivityTypes nActType)
		{
			JieriFanLiActivity jieriFanLiActivity = HuodongCachingMgr.GetJieriFanLiActivity(nActType);
			bool result;
			if (null == jieriFanLiActivity)
			{
				result = false;
			}
			else if (!jieriFanLiActivity.InAwardTime())
			{
				result = false;
			}
			else
			{
				string[] array = null;
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					jieriFanLiActivity.FromDate.Replace(':', '$'),
					jieriFanLiActivity.ToDate.Replace(':', '$'),
					(int)nActType,
					0
				});
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 927, strcmd, out array, client.ServerId);
				if (array == null || 2 != array.Length)
				{
					result = false;
				}
				else
				{
					int num = Convert.ToInt32(array[1]);
					bool flag = false;
					for (int i = 1; i <= 5; i++)
					{
						int bitValue = Global.GetBitValue(i);
						if ((num & bitValue) != bitValue)
						{
							flag = jieriFanLiActivity.CheckCondition(client, i);
							if (flag)
							{
								break;
							}
						}
					}
					ushort nIconOrder = 0;
					switch (nActType)
					{
					case ActivityTypes.JieriWing:
						nIconOrder = 14011;
						break;
					case ActivityTypes.JieriAddon:
						nIconOrder = 14012;
						break;
					case ActivityTypes.JieriStrengthen:
						nIconOrder = 14013;
						break;
					case ActivityTypes.JieriAchievement:
						nIconOrder = 14014;
						break;
					case ActivityTypes.JieriMilitaryRank:
						nIconOrder = 14015;
						break;
					case ActivityTypes.JieriVIPFanli:
						nIconOrder = 14016;
						break;
					case ActivityTypes.JieriAmulet:
						nIconOrder = 14017;
						break;
					case ActivityTypes.JieriArchangel:
						nIconOrder = 14018;
						break;
					case ActivityTypes.JieriLianXuCharge:
						break;
					case ActivityTypes.JieriMarriage:
						nIconOrder = 14019;
						break;
					default:
						switch (nActType)
						{
						case ActivityTypes.JieRiHuiJi:
							nIconOrder = 14033;
							break;
						case ActivityTypes.JieRiFuWen:
							nIconOrder = 14034;
							break;
						}
						break;
					}
					result = this.AddFlushIconState(nIconOrder, flag);
				}
			}
			return result;
		}

		public void xxxxx(GameClient client)
		{
			bool bIconState = false;
			DateTime d = TimeUtil.NowDateTime();
			DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "ImpetrateTime");
			double totalSeconds = (d - roleParamsDateTimeFromDB).TotalSeconds;
			double num = Global.GMax(0.0, (double)Data.FreeImpetrateIntervalTime - totalSeconds);
			if (num <= 0.0)
			{
				bIconState = true;
			}
			this.AddFlushIconState(8000, bIconState);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		public bool CheckFreeZhuanPanChouState(GameClient client)
		{
			bool bIconState = false;
			int num = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanFree"));
			bool result;
			if (num < 0)
			{
				bIconState = false;
				result = this.AddFlushIconState(18002, bIconState);
			}
			else
			{
				DateTime d = TimeUtil.NowDateTime();
				DateTime dateTime = Global.GetRoleParamsDateTimeFromDB(client, "10155");
				DateTime beginTime = ZhuanPanManager.getInstance().GetBeginTime();
				if (dateTime < beginTime)
				{
					dateTime = beginTime;
					Global.SaveRoleParamsDateTimeToDB(client, "10155", dateTime, true);
				}
				double totalSeconds = (d - dateTime).TotalSeconds;
				num *= 3600;
				double num2 = Global.GMax(0.0, (double)num - totalSeconds);
				if (num2 <= 0.0)
				{
					bIconState = true;
				}
				result = this.AddFlushIconState(18002, bIconState);
			}
			return result;
		}

		private Dictionary<ushort, ushort> m_StateIconsDict = new Dictionary<ushort, ushort>();

		private Dictionary<ushort, ushort> m_StateCacheIconsDict = new Dictionary<ushort, ushort>();

		private ActivityIconStateData m_ActivityIconStateData = new ActivityIconStateData();

		private long m_LastTicks = 0L;

		private long m_LastTicksBuilding = 0L;

		private static List<ActivityTipTypes> m_jieRiIconList = new List<ActivityTipTypes>
		{
			ActivityTipTypes.JieRiLogin,
			ActivityTipTypes.JieRiTotalLogin,
			ActivityTipTypes.JieRiDayCZ,
			ActivityTipTypes.JieRiLeiJiXF,
			ActivityTipTypes.JieRiLeiJiCZ,
			ActivityTipTypes.JieRiCZKING,
			ActivityTipTypes.JieRiXFKING,
			ActivityTipTypes.JieRiGive,
			ActivityTipTypes.JieRiGiveKing,
			ActivityTipTypes.JieRiRecvKing,
			ActivityTipTypes.JieRiRecv,
			ActivityTipTypes.JieriWing,
			ActivityTipTypes.JieriAddon,
			ActivityTipTypes.JieriStrengthen,
			ActivityTipTypes.JieriAchievement,
			ActivityTipTypes.JieriMilitaryRank,
			ActivityTipTypes.JieriVIPFanli,
			ActivityTipTypes.JieriAmulet,
			ActivityTipTypes.JieriArchangel,
			ActivityTipTypes.JieriMarriage,
			ActivityTipTypes.JieRiLianXuCharge,
			ActivityTipTypes.JieRiIPointsExchg,
			ActivityTipTypes.JieRiPlatChargeKing,
			ActivityTipTypes.JieRiHuiJi,
			ActivityTipTypes.JieRiFuWen,
			ActivityTipTypes.JieRiPCKingEveryDay
		};
	}
}
