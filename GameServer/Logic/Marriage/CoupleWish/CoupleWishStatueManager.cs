using System;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Marriage.CoupleWish
{
	internal class CoupleWishStatueManager
	{
		public void SetWishConfig(CoupleWishConfig config)
		{
			this._Config = config;
		}

		public bool LoadConfig()
		{
			bool result;
			try
			{
				string[] array = GameManager.systemParamsList.GetParamValueByName("WishHunYanNPC").Split(new char[]
				{
					','
				});
				this.YanHuiMapCode = Convert.ToInt32(array[0]);
				this.YanHuiNpcId = Convert.ToInt32(array[1]);
				this.YanHuiNpcX = Convert.ToInt32(array[2]);
				this.YanHuiNpcY = Convert.ToInt32(array[3]);
				this.YanHuiNpcDir = Convert.ToInt32(array[4]);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = false;
			}
			return result;
		}

		public void SetDiaoXiang(CoupleWishSyncStatueData newStatue)
		{
			if (newStatue.DbCoupleId > 0 && (newStatue.ManRoleDataEx == null || newStatue.WifeRoleDataEx == null))
			{
				RoleData4Selector mainOccupationRoleDataForSelector = RoleManager.getInstance().GetMainOccupationRoleDataForSelector(newStatue.Man.RoleId, GameManager.ServerId);
				RoleData4Selector mainOccupationRoleDataForSelector2 = RoleManager.getInstance().GetMainOccupationRoleDataForSelector(newStatue.Wife.RoleId, GameManager.ServerId);
				if (mainOccupationRoleDataForSelector != null && mainOccupationRoleDataForSelector2 != null)
				{
					CoupleWishReportStatueData coupleWishReportStatueData = new CoupleWishReportStatueData();
					coupleWishReportStatueData.DbCoupleId = newStatue.DbCoupleId;
					coupleWishReportStatueData.ManStatue = DataHelper.ObjectToBytes<RoleData4Selector>(mainOccupationRoleDataForSelector);
					coupleWishReportStatueData.WifeStatue = DataHelper.ObjectToBytes<RoleData4Selector>(mainOccupationRoleDataForSelector2);
					TianTiClient.getInstance().CoupleWishReportCoupleStatue(coupleWishReportStatueData);
				}
			}
			if (newStatue.DbCoupleId > 0 && newStatue.ManRoleDataEx != null && newStatue.WifeRoleDataEx != null)
			{
				if (newStatue.IsDivorced == 1)
				{
					this.ReshowCoupleStatue(null, null);
				}
				else if (this._Statue == null || this._Statue.ManRoleDataEx == null || this._Statue.WifeRoleDataEx == null || this._Statue.DbCoupleId != newStatue.DbCoupleId)
				{
					this.ReshowCoupleStatue(DataHelper.BytesToObject<RoleData4Selector>(newStatue.ManRoleDataEx, 0, newStatue.ManRoleDataEx.Length), DataHelper.BytesToObject<RoleData4Selector>(newStatue.WifeRoleDataEx, 0, newStatue.WifeRoleDataEx.Length));
				}
			}
			else
			{
				this.ReshowCoupleStatue(null, null);
			}
			NPC npcfromConfig = NPCGeneralManager.GetNPCFromConfig(this.YanHuiMapCode, this.YanHuiNpcId, this.YanHuiNpcX, this.YanHuiNpcY, this.YanHuiNpcDir);
			if (newStatue.DbCoupleId > 0 && npcfromConfig != null && (this._Statue == null || this._Statue.DbCoupleId != newStatue.DbCoupleId) && newStatue.YanHuiJoinNum < this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				NPCGeneralManager.AddNpcToMap(npcfromConfig);
			}
			if (newStatue.DbCoupleId <= 0 || newStatue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				NPCGeneralManager.RemoveMapNpc(this.YanHuiMapCode, this.YanHuiNpcId);
			}
			this._Statue = newStatue;
		}

		public CoupleWishYanHuiData HandleQueryParty(GameClient client)
		{
			CoupleWishYanHuiData coupleWishYanHuiData = new CoupleWishYanHuiData();
			if (this._Statue != null && this._Statue.Man != null && this._Statue.Wife != null)
			{
				coupleWishYanHuiData.Man = this._Statue.Man;
				coupleWishYanHuiData.Wife = this._Statue.Wife;
				coupleWishYanHuiData.TotalJoinNum = this._Statue.YanHuiJoinNum;
				coupleWishYanHuiData.DbCoupleId = this._Statue.DbCoupleId;
				coupleWishYanHuiData.MyJoinNum = this.GetJoinPartyNum(client, this._Statue.DbCoupleId);
			}
			return coupleWishYanHuiData;
		}

		public int HandleJoinParty(GameClient client, int toCouleId)
		{
			int result;
			if (this._Statue == null || this._Statue.DbCoupleId <= 0 || this._Statue.DbCoupleId != toCouleId)
			{
				result = -12;
			}
			else if (this.GetJoinPartyNum(client, toCouleId) >= this._Config.YanHuiCfg.EachRoleMaxJoinNum || this._Statue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
			{
				result = -16;
			}
			else if (Global.GetTotalBindTongQianAndTongQianVal(client) < this._Config.YanHuiCfg.CostBindJinBi)
			{
				result = -9;
			}
			else
			{
				int num = TianTiClient.getInstance().CoupleWishJoinParty(client.ClientData.RoleID, client.ClientData.ZoneID, toCouleId);
				if (num < 0)
				{
					result = num;
				}
				else
				{
					Global.SubBindTongQianAndTongQian(client, this._Config.YanHuiCfg.CostBindJinBi, "情侣祝福宴会");
					this.AddJoinPartyNum(client, toCouleId, 1);
					this._Statue.YanHuiJoinNum++;
					if (this._Config.YanHuiCfg.GetExp > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)this._Config.YanHuiCfg.GetExp, false, true, false, "none");
						GameManager.ClientMgr.NotifyAddExpMsg(client, (long)this._Config.YanHuiCfg.GetExp);
					}
					if (this._Config.YanHuiCfg.GetXingHun > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(client, this._Config.YanHuiCfg.GetXingHun, "情侣祝福榜宴会", true, true);
					}
					if (this._Config.YanHuiCfg.GetShengWang > 0)
					{
						GameManager.ClientMgr.ModifyShengWangValue(client, this._Config.YanHuiCfg.GetShengWang, "情侣祝福榜宴会", true, true);
					}
					result = 1;
				}
			}
			return result;
		}

		private int GetJoinPartyNum(GameClient client, int toCoupleId)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else
			{
				string roleParamByName = Global.GetRoleParamByName(client, "31");
				string[] array = (!string.IsNullOrEmpty(roleParamByName)) ? roleParamByName.Split(new char[]
				{
					','
				}) : null;
				if (array != null && array.Length == 2 && Convert.ToInt32(array[0]) == toCoupleId)
				{
					result = Convert.ToInt32(array[1]);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private void AddJoinPartyNum(GameClient client, int toCoupleId, int addNum = 1)
		{
			int num = addNum + this.GetJoinPartyNum(client, toCoupleId);
			Global.SaveRoleParamsStringToDB(client, "31", string.Format("{0},{1}", toCoupleId, num), true);
		}

		public CoupleWishTop1AdmireData HandleQueryAdmireData(GameClient client)
		{
			CoupleWishTop1AdmireData coupleWishTop1AdmireData = new CoupleWishTop1AdmireData();
			if (this._Statue != null && this._Statue.IsDivorced != 1 && this._Statue.DbCoupleId > 0 && this._Statue.ManRoleDataEx != null && this._Statue.WifeRoleDataEx != null)
			{
				coupleWishTop1AdmireData.DbCoupleId = this._Statue.DbCoupleId;
				coupleWishTop1AdmireData.ManSelector = DataHelper.BytesToObject<RoleData4Selector>(this._Statue.ManRoleDataEx, 0, this._Statue.ManRoleDataEx.Length);
				coupleWishTop1AdmireData.WifeSelector = DataHelper.BytesToObject<RoleData4Selector>(this._Statue.WifeRoleDataEx, 0, this._Statue.WifeRoleDataEx.Length);
				coupleWishTop1AdmireData.BeAdmireCount = this._Statue.BeAdmireCount;
			}
			coupleWishTop1AdmireData.MyAdmireCount = this.GetAdmireCount(client, TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime()));
			return coupleWishTop1AdmireData;
		}

		public int HandleAdmireStatue(GameClient client, int toCoupleId, int admireType)
		{
			int toDay = TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime());
			MoBaiData moBaiData = null;
			int result;
			if (!Data.MoBaiDataInfoList.TryGetValue(2, out moBaiData))
			{
				result = -3;
			}
			else if (client.ClientData.ChangeLifeCount < moBaiData.MinZhuanSheng || (client.ClientData.ChangeLifeCount == moBaiData.MinZhuanSheng && client.ClientData.Level < moBaiData.MinLevel))
			{
				result = -19;
			}
			else
			{
				int num = moBaiData.AdrationMaxLimit;
				int admireCount = this.GetAdmireCount(client, toDay);
				if (this._Statue != null && this._Statue.IsDivorced != 1 && this._Statue.DbCoupleId > 0 && (client.ClientData.RoleID == this._Statue.Man.RoleId || client.ClientData.RoleID == this._Statue.Wife.RoleId))
				{
					num += moBaiData.ExtraNumber;
				}
				int vipLevel = client.ClientData.VipLevel;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (vipLevel > VIPEumValue.VIPENUMVALUE_MAXLEVEL || paramValueIntArrayByName.Length < 1)
				{
					result = -3;
				}
				else
				{
					num += paramValueIntArrayByName[vipLevel];
					double num2 = 0.0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(12);
						if (null != config)
						{
							num2 += config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num2 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_Admire);
					}
					num2 = Math.Max(1.0, num2);
					num = (int)((double)num * num2);
					if (admireCount >= num)
					{
						result = -16;
					}
					else if (admireType == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < moBaiData.NeedJinBi)
					{
						result = -9;
					}
					else if (admireType == 2 && client.ClientData.UserMoney < moBaiData.NeedZuanShi)
					{
						result = -10;
					}
					else
					{
						int num3 = TianTiClient.getInstance().CoupleWishAdmire(client.ClientData.RoleID, client.ClientData.ZoneID, admireType, toCoupleId);
						double num4 = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
						if (admireType == 1)
						{
							Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜情侣祝福");
							long num5 = (long)(num4 * (double)moBaiData.JinBiExpAward);
							if (num5 > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, num5, true, true, false, "none");
							}
							if (moBaiData.JinBiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (moBaiData.LingJingAwardByJinBi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜情侣祝福", true, true, false);
							}
						}
						if (admireType == 2)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜情侣祝福", true, true, false, DaiBiSySType.None);
							int num6 = (int)(num4 * (double)moBaiData.ZuanShiExpAward);
							if (num6 > 0)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, (long)num6, true, true, false, "none");
							}
							if (moBaiData.ZuanShiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (moBaiData.LingJingAwardByZuanShi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜情侣祝福", true, true, false);
							}
						}
						this.AddAdmireCount(client, toDay, toCoupleId, 1);
						if (this._Statue != null && this._Statue.DbCoupleId > 0 && this._Statue.DbCoupleId == toCoupleId)
						{
							this._Statue.BeAdmireCount++;
						}
						result = 1;
					}
				}
			}
			return result;
		}

		private int GetAdmireCount(GameClient client, int toDay)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else
			{
				string roleParamByName = Global.GetRoleParamByName(client, "30");
				string[] array = (!string.IsNullOrEmpty(roleParamByName)) ? roleParamByName.Split(new char[]
				{
					','
				}) : null;
				if (array != null && array.Length == 3 && Convert.ToInt32(array[0]) == toDay)
				{
					result = Convert.ToInt32(array[1]);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private void AddAdmireCount(GameClient client, int toDay, int toCoupleId, int addCount = 1)
		{
			if (client != null)
			{
				int num = addCount + this.GetAdmireCount(client, toDay);
				Global.SaveRoleParamsStringToDB(client, "30", string.Format("{0},{1},{2}", toDay, num, toCoupleId), true);
			}
		}

		private void ReshowCoupleStatue(RoleData4Selector manStatue, RoleData4Selector wifeStatue)
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.CoupleWishMan);
			if (null != npc)
			{
				if (manStatue == null)
				{
					npc.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishMan, true);
				}
				else
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishMan, true);
					FakeRoleManager.ProcessNewFakeRole(manStatue, npc.MapCode, FakeRoleTypes.CoupleWishMan, (int)npc.CurrentDir, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.CoupleWishMan);
				}
			}
			NPC npc2 = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.CoupleWishWife);
			if (null != npc2)
			{
				if (wifeStatue == null)
				{
					npc2.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc2);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishWife, true);
				}
				else
				{
					npc2.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc2);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CoupleWishWife, true);
					FakeRoleManager.ProcessNewFakeRole(wifeStatue, npc2.MapCode, FakeRoleTypes.CoupleWishWife, (int)npc2.CurrentDir, (int)npc2.CurrentPos.X, (int)npc2.CurrentPos.Y, FakeRoleNpcId.CoupleWishWife);
				}
			}
		}

		private int YanHuiMapCode;

		private int YanHuiNpcId;

		private int YanHuiNpcX;

		private int YanHuiNpcY;

		private int YanHuiNpcDir;

		private CoupleWishSyncStatueData _Statue = null;

		private CoupleWishConfig _Config = null;
	}
}
