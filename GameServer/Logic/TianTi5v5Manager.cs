using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using ProtoBuf;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class TianTi5v5Manager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2, ICopySceneManager
	{
		public static TianTi5v5Manager getInstance()
		{
			return TianTi5v5Manager.instance;
		}

		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTi5v5Manager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
			return this.InitConfig(false);
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(3693, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3697, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3680, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3687, 2, 3, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3690, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3685, 2, 3, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3686, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3688, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3691, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3695, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3694, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3698, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3700, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3699, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3704, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3709, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3711, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3713, 1, 1, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3714, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3712, 0, 5, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3679, 0, 5, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3719, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(3721, 2, 2, TianTi5v5Manager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(60, 55, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(13, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(59, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, TianTi5v5Manager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(60, 55, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().removeListener(13, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(59, TianTi5v5Manager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, TianTi5v5Manager.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 3679:
				return this.ProcessTianTi5v5GetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
			case 3680:
				return this.ProcessGetTianTi5v5DataAndDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 3685:
				return this.ProcessCreateZhanDui(client, nID, bytes, cmdParams);
			case 3686:
				return this.ProcessDeleteZhanDuiMember(client, nID, bytes, cmdParams);
			case 3687:
				return this.ProcessInviteOther2MyZhanDui(client, nID, bytes, cmdParams);
			case 3688:
				return this.ProcessGetZhanDuiList(client, nID, bytes, cmdParams);
			case 3690:
				return this.ProcessAgreeZhanDuiInvite(client, nID, bytes, cmdParams);
			case 3691:
				return this.ProcessUpdateZhanDuiXuanYan(client, nID, bytes, cmdParams);
			case 3693:
				return this.ProcessTianTi5v5QuitCmd(client, nID, bytes, cmdParams);
			case 3695:
				return this.ProcessGetMyZhanDuiInfo(client, nID, bytes, cmdParams);
			case 3697:
				return this.ProcessTianTi5v5EnterCmd(client, nID, bytes, cmdParams);
			case 3698:
				return this.ProcessChangeZhanDuiLeader(client, nID, bytes, cmdParams);
			case 3699:
				return this.ProcessDeleteZhanDui(client, nID, bytes, cmdParams);
			case 3700:
				return this.ProcessZhanDuiKF5V5JingJiData(client, nID, bytes, cmdParams);
			case 3704:
				return this.ProcessGetDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 3709:
				return this.ProcessTianTi5v5GeLogCmd(client, nID, bytes, cmdParams);
			case 3711:
				return this.ProcessQuitFromZhanDui(client, nID, bytes, cmdParams);
			case 3713:
				return this.ProcessConfirmBattleCmd(client, nID, bytes, cmdParams);
			case 3714:
				return this.ProcessAcceptConfirmBattleCmd(client, nID, bytes, cmdParams);
			case 3719:
				return this.ProcessRequestToZhanDui(client, nID, bytes, cmdParams);
			case 3721:
				return this.ProcessAgreeZhanDuiRequest(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					if (playerDeadEventObject.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEventObject.getAttackerRole(), playerDeadEventObject.getPlayer());
					}
				}
			}
			else if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.InitRoleTianTi5v5Data(playerInitGameEventObject.getPlayer());
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				if (null != onStartPlayGameEventObject)
				{
					this.SendZhanDuiInviteData(onStartPlayGameEventObject.Client);
					this.SendZhanDuiRequestData(onStartPlayGameEventObject.Client);
				}
			}
			else if (eventType == 59)
			{
				OnClientMapChangedEventObject onClientMapChangedEventObject = eventObject as OnClientMapChangedEventObject;
				if (null != onClientMapChangedEventObject)
				{
					this.SendZhanDuiInviteData(onClientMapChangedEventObject.Client);
					this.SendZhanDuiRequestData(onClientMapChangedEventObject.Client);
				}
			}
			else if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject playerLeaveFuBenEventObject = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(playerLeaveFuBenEventObject.getPlayer());
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 60)
			{
				this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient);
			}
		}

		public bool InitConfig(bool reload = false)
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.TianTi5v5CD = (int)GameManager.systemParamsList.GetParamValueIntByName("TianTi5v5CD", -1);
					Dictionary<int, TianTiDuanWei> dictionary = new Dictionary<int, TianTiDuanWei>();
					Dictionary<RangeKey, int> dictionary2 = new Dictionary<RangeKey, int>(RangeKey.Comparer);
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					text = "Config/TeamDuanWei.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						TianTiDuanWei tianTiDuanWei = new TianTiDuanWei();
						tianTiDuanWei.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						tianTiDuanWei.NeedDuanWeiJiFen = (int)Global.GetSafeAttributeLong(xml, "NeedDuanWeiJiFen");
						tianTiDuanWei.WinJiFen = (int)Global.GetSafeAttributeLong(xml, "WinJiFen");
						tianTiDuanWei.LoseJiFen = (int)Global.GetSafeAttributeLong(xml, "LoseJiFen");
						tianTiDuanWei.RongYaoNum = (int)Global.GetSafeAttributeLong(xml, "RongYaoNum");
						tianTiDuanWei.WinRongYu = (int)Global.GetSafeAttributeLong(xml, "WinRongYu");
						tianTiDuanWei.LoseRongYu = (int)Global.GetSafeAttributeLong(xml, "LoseRongYu");
						if (num2 > 0)
						{
							dictionary2[new RangeKey(num, tianTiDuanWei.NeedDuanWeiJiFen - 1, null)] = num2;
						}
						num = tianTiDuanWei.NeedDuanWeiJiFen;
						num2 = tianTiDuanWei.ID;
						num3 = tianTiDuanWei.ID;
						dictionary[tianTiDuanWei.ID] = tianTiDuanWei;
					}
					if (num3 > 0 && num > 0)
					{
						dictionary2[new RangeKey(num, int.MaxValue, null)] = num3;
					}
					this.RuntimeData.TianTi5v5DuanWeiDict = dictionary;
					this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict = dictionary2;
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/TeamBattleBirthPoint.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						TianTiBirthPoint tianTiBirthPoint = new TianTiBirthPoint();
						tianTiBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						tianTiBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						tianTiBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						tianTiBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[tianTiBirthPoint.ID] = tianTiBirthPoint;
					}
					this.RuntimeData.DuanWeiRankAwardDict.Clear();
					text = "Config/TeamDuanWeiAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						DuanWeiRankAward duanWeiRankAward = new DuanWeiRankAward();
						duanWeiRankAward.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						duanWeiRankAward.StarRank = (int)Global.GetSafeAttributeLong(xml, "StarRank");
						duanWeiRankAward.EndRank = (int)Global.GetSafeAttributeLong(xml, "EndRank");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "Award"), ref duanWeiRankAward.Award, '|', ',');
						if (duanWeiRankAward.EndRank < 0)
						{
							duanWeiRankAward.EndRank = int.MaxValue;
						}
						this.RuntimeData.DuanWeiRankAwardDict[new RangeKey(duanWeiRankAward.StarRank, duanWeiRankAward.EndRank, null)] = duanWeiRankAward;
					}
					this.RuntimeData.MapCodeDict.Clear();
					text = "Config/TeamBattle.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						int num4 = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						if (!this.RuntimeData.MapCodeDict.ContainsKey(num4))
						{
							this.RuntimeData.MapCodeDict[num4] = 1;
							this.RuntimeData.MapCodeList.Add(num4);
						}
						this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-'))
						{
							result = false;
							LogManager.WriteLog(1000, "读取跨服组队竞技时间配置(TimePoints)出错", null, true);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("缺少跨服组队竞技地图 {0}", num4), null, true);
						}
					}
					this.RuntimeData.TeamBattleMap = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleMap", ',');
					this.RuntimeData.ZhanDuiJinZuan = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamNeedZuan", 50);
					this.RuntimeData.TeamConfirmTime = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamConfirmTime", -1);
					this.RuntimeData.TeamAwardLimit = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamAwardLimit", -1);
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamLevelLimit", ',');
					if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length >= 2)
					{
						this.RuntimeData.ZhanDuiDengJiTp = new Tuple<int, int>(paramValueIntArrayByName[0], paramValueIntArrayByName[1]);
					}
					this.RuntimeData.MaxZhanDuiNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxZhanDuiNum", 1000);
					this.RuntimeData.MaxTianTi5v5JiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTianTi5v5JiFen", -1);
					this.RuntimeData.TeamBattleWatch = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleWatch", ',');
					this.RuntimeData.MinConfirmFightTeamCnt = (int)GameManager.systemParamsList.GetParamValueIntByName("MinConfirmFightTeamCnt", 1);
					this.RuntimeData.TeamBattleNameRange = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleNameRange", ',');
					if (this.RuntimeData.TeamBattleNameRange == null)
					{
						this.RuntimeData.TeamBattleNameRange = new int[]
						{
							2,
							7
						};
					}
					this.RuntimeData.TeamApply = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamApply", ',');
					if (this.RuntimeData.TeamApply == null)
					{
						this.RuntimeData.TeamApply = new int[]
						{
							1,
							2
						};
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public void GMStartHuoDongNow(int v)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					if (v == 0)
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, this.RuntimeData.TimePointsStr, true, '|', '-');
					}
					else
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, "00:00-23:59:59", true, '|', '-');
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private TianTi5v5ZhanDuiData NewZhanDuiData(GameClient client)
		{
			int duanWeiId = this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.Values.Min();
			TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = new TianTi5v5ZhanDuiData
			{
				ZhanDuiID = 0,
				DuanWeiId = duanWeiId
			};
			TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = new TianTi5v5ZhanDuiRoleData();
			tianTi5v5ZhanDuiRoleData.RoleID = client.ClientData.LocalRoleID;
			tianTi5v5ZhanDuiRoleData.RoleName = client.ClientData.RoleName;
			tianTi5v5ZhanDuiRoleData.RoleOcc = client.ClientData.Occupation;
			tianTi5v5ZhanDuiRoleData.ZhanLi = (long)client.ClientData.CombatForce;
			tianTi5v5ZhanDuiRoleData.Level = client.ClientData.Level;
			tianTi5v5ZhanDuiRoleData.ZhuanSheng = client.ClientData.ChangeLifeCount;
			tianTi5v5ZhanDuiRoleData.ZoneID = client.ClientData.ZoneID;
			tianTi5v5ZhanDuiRoleData.RebornLevel = client.ClientData.RebornLevel;
			tianTi5v5ZhanDuiData.teamerList.Add(tianTi5v5ZhanDuiRoleData);
			tianTi5v5ZhanDuiData.ZhanDouLi = tianTi5v5ZhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
			RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleData4Selector != null || roleData4Selector.RoleID < 0)
			{
				tianTi5v5ZhanDuiData.teamerList[0].ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
			}
			return tianTi5v5ZhanDuiData;
		}

		private TianTi5v5ZhanDuiRoleData NewRoleData(GameClient client)
		{
			TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = new TianTi5v5ZhanDuiRoleData();
			tianTi5v5ZhanDuiRoleData.RoleID = client.ClientData.LocalRoleID;
			tianTi5v5ZhanDuiRoleData.RoleName = client.ClientData.RoleName;
			tianTi5v5ZhanDuiRoleData.RoleOcc = client.ClientData.Occupation;
			tianTi5v5ZhanDuiRoleData.ZhanLi = (long)client.ClientData.CombatForce;
			tianTi5v5ZhanDuiRoleData.Level = client.ClientData.Level;
			tianTi5v5ZhanDuiRoleData.ZhuanSheng = client.ClientData.ChangeLifeCount;
			tianTi5v5ZhanDuiRoleData.RebornLevel = client.ClientData.RebornLevel;
			RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleData4Selector != null || roleData4Selector.RoleID < 0)
			{
				tianTi5v5ZhanDuiRoleData.ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
			}
			return tianTi5v5ZhanDuiRoleData;
		}

		public void GMSetRoleData(GameClient client, int duanWeiId, int duanWeiJiFen, int rongYao, int monthDuanWeiRank, int lianSheng, int successCount, int fightCount)
		{
			TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId) ?? this.NewZhanDuiData(client);
			if (null != tianTi5v5ZhanDuiData)
			{
				tianTi5v5ZhanDuiData.DuanWeiJiFen = duanWeiJiFen;
				tianTi5v5ZhanDuiData.MonthDuanWeiRank = monthDuanWeiRank;
				tianTi5v5ZhanDuiData.LianSheng = lianSheng;
				tianTi5v5ZhanDuiData.SuccessCount = successCount;
				tianTi5v5ZhanDuiData.FightCount = fightCount;
				int duanWeiId2;
				if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(tianTi5v5ZhanDuiData.ZhanDuiID, out duanWeiId2))
				{
					tianTi5v5ZhanDuiData.DuanWeiId = duanWeiId2;
				}
				RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (roleData4Selector != null || roleData4Selector.RoleID < 0)
				{
					tianTi5v5ZhanDuiData.teamerList[0].ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
				}
				GameManager.ClientMgr.ModifyTeamRongYaoValue(client, rongYao - client.ClientData.TeamRongYao, "GM添加", false);
				TianTiClient.getInstance().UpdateZhanDuiData(tianTi5v5ZhanDuiData, 1);
			}
		}

		public void TimerProc(object sender, EventArgs e)
		{
			bool flag = false;
			TianTi5v5RankData tianTi5v5RankData = TianTiClient.getInstance().ZhanDuiGetRankingData(this.RuntimeData.RankData.ModifyTime);
			lock (this.RuntimeData.Mutex)
			{
				if (tianTi5v5RankData != null && tianTi5v5RankData.ModifyTime > this.RuntimeData.ModifyTime)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Dictionary<int, TianTi5v5ZhanDuiData> dictionary = new Dictionary<int, TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> list = new List<TianTi5v5ZhanDuiData>();
				Dictionary<int, TianTi5v5ZhanDuiData> dictionary2 = new Dictionary<int, TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> list2 = new List<TianTi5v5ZhanDuiData>();
				if (null != tianTi5v5RankData.DayPaiHangList)
				{
					foreach (TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData in tianTi5v5RankData.DayPaiHangList)
					{
						dictionary[tianTi5v5ZhanDuiData.ZhanDuiID] = tianTi5v5ZhanDuiData;
						if (list.Count < this.RuntimeData.MaxDayPaiMingListCount)
						{
							list.Add(tianTi5v5ZhanDuiData);
						}
					}
				}
				if (null != tianTi5v5RankData.MonthPaiHangList)
				{
					foreach (TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData in tianTi5v5RankData.MonthPaiHangList)
					{
						dictionary2[tianTi5v5ZhanDuiData.ZhanDuiID] = tianTi5v5ZhanDuiData;
						if (list2.Count < this.RuntimeData.MaxMonthPaiMingListCount)
						{
							list2.Add(tianTi5v5ZhanDuiData);
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.ModifyTime = tianTi5v5RankData.ModifyTime;
					this.RuntimeData.MaxPaiMingRank = tianTi5v5RankData.MaxPaiMingRank;
					this.RuntimeData.ZhanDuiDataPaiHangDict = dictionary;
					this.RuntimeData.ZhanDuiDataPaiHangList = list;
					this.RuntimeData.ZhanDuiDataMonthPaiHangDict = dictionary2;
					this.RuntimeData.ZhanDuiDataMonthPaiHangList = list2;
				}
			}
		}

		public bool ProcessTianTi5v5JoinCmd(int zhanDuiID, TianTi5v5PiPeiState piPeiState)
		{
			if (piPeiState.State == 0)
			{
				piPeiState.EndTicks += (long)this.RuntimeData.MaxSignUp;
				piPeiState.State = 1;
				int num = TianTiClient.getInstance().ZhanDuiRoleSignUp(GameManager.ServerId, 34, zhanDuiID, piPeiState.ZhanLi, piPeiState.GroupIndex);
				if (num > 0)
				{
					GlobalNew.UpdateKuaFuRoleDayLogData(GameManager.ServerId, zhanDuiID, TimeUtil.NowDateTime(), GameManager.ServerId, 1, 0, 0, 0, 34);
					return true;
				}
			}
			return false;
		}

		public bool ProcessGetTianTi5v5DataAndDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					this.GetMainInfo(client);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					TianTi5v5Manager.TianTi5v5ZhanDuiDataList cmdData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (!ListExt.IsNullOrEmpty<TianTi5v5ZhanDuiData>(this.RuntimeData.ZhanDuiDataPaiHangList))
						{
							int count = Math.Min(this.RuntimeData.MaxDayPaiMingListCount, this.RuntimeData.ZhanDuiDataPaiHangList.Count);
							cmdData = new TianTi5v5Manager.TianTi5v5ZhanDuiDataList(this.RuntimeData.ZhanDuiDataPaiHangList.GetRange(0, count));
						}
					}
					client.sendCmd<TianTi5v5Manager.TianTi5v5ZhanDuiDataList>(nID, cmdData, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTi5v5GetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = -20;
				DuanWeiRankAward duanWeiRankAward = null;
				if (this.CanGetMonthRankAwards(client, out duanWeiRankAward))
				{
					List<GoodsData> list = Global.ConvertToGoodsDataList(duanWeiRankAward.Award.Items, -1);
					if (!Global.CanAddGoodsDataList(client, list))
					{
						cmdData = -100;
					}
					else
					{
						cmdData = 0;
						Global.SaveRoleParamsDateTimeToDB(client, "10220", TimeUtil.NowDateTime(), true);
						for (int i = 0; i < list.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
						}
					}
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTi5v5GeLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<TianTi5v5LogItemData> cmdData = new List<TianTi5v5LogItemData>();
				cmdData = Global.sendToDB<List<TianTi5v5LogItemData>, int>(3709, client.ClientData.ZhanDuiID, client.ServerId);
				client.sendCmd<List<TianTi5v5LogItemData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessConfirmBattleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						num = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						num = -4016;
					}
					else
					{
						bool flag = false;
						TianTi5v5PiPeiState tianTi5v5PiPeiState = new TianTi5v5PiPeiState();
						tianTi5v5PiPeiState.RoleList = new List<TianTi5v5PiPeiRoleState>();
						lock (this.RuntimeData.Mutex)
						{
							num = -2001;
							TimeSpan timeOfDay = TimeUtil.NowDateTime().TimeOfDay;
							lock (this.RuntimeData.Mutex)
							{
								for (int i = 0; i < this.RuntimeData.TimePoints.Count - 1; i += 2)
								{
									if (timeOfDay >= this.RuntimeData.TimePoints[i] && timeOfDay < this.RuntimeData.TimePoints[i + 1])
									{
										num = 1;
										break;
									}
								}
							}
							if (num < 0)
							{
								goto IL_437;
							}
							if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
							{
								num = -21;
								goto IL_437;
							}
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							if (zhanDuiData.teamerList.Count < this.RuntimeData.MinConfirmFightTeamCnt)
							{
								num = -4026;
								goto IL_437;
							}
							foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
							{
								TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState = new TianTi5v5PiPeiRoleState
								{
									RoleID = tianTi5v5ZhanDuiRoleData.RoleID,
									RoleName = tianTi5v5ZhanDuiRoleData.RoleName,
									Occupation = tianTi5v5ZhanDuiRoleData.RoleOcc
								};
								GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5ZhanDuiRoleData.RoleID);
								if (null == gameClient)
								{
									tianTi5v5PiPeiRoleState.State = 4;
								}
								else if (!this.RuntimeData.TeamBattleMap.Contains(gameClient.ClientData.MapCode))
								{
									tianTi5v5PiPeiRoleState.State = 3;
								}
								else if (tianTi5v5ZhanDuiRoleData.RoleID == client.ClientData.RoleID)
								{
									tianTi5v5PiPeiRoleState.State = 1;
								}
								else
								{
									tianTi5v5PiPeiRoleState.State = 0;
								}
								tianTi5v5PiPeiState.RoleList.Add(tianTi5v5PiPeiRoleState);
							}
							tianTi5v5PiPeiState.ZhanLi = zhanDuiData.ZhanDouLi;
							tianTi5v5PiPeiState.GroupIndex = zhanDuiData.DuanWeiId;
							tianTi5v5PiPeiState.EndTicks = TimeUtil.NOW() + (long)(this.RuntimeData.TeamConfirmTime * 1000);
							this.RuntimeData.ConfirmBattleDict[zhanDuiID] = tianTi5v5PiPeiState;
							flag = tianTi5v5PiPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1);
							bool flag4 = tianTi5v5PiPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1 || x.State == 0);
						}
						foreach (TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState in tianTi5v5PiPeiState.RoleList)
						{
							TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState;
							GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5PiPeiRoleState.RoleID);
							if ((gameClient != null && tianTi5v5PiPeiRoleState.State == 1) || tianTi5v5PiPeiRoleState.State == 0)
							{
								gameClient.sendCmd<TianTi5v5PiPeiState>(3712, tianTi5v5PiPeiState, false);
							}
						}
						if (flag)
						{
							this.ProcessTianTi5v5JoinCmd(zhanDuiID, tianTi5v5PiPeiState);
						}
					}
				}
				IL_437:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAcceptConfirmBattleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 1;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (zhanDuiID <= 0)
				{
					cmdData = -4013;
				}
				else
				{
					bool flag = false;
					TianTi5v5PiPeiState tianTi5v5PiPeiState;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out tianTi5v5PiPeiState) || tianTi5v5PiPeiState.EndTicks < TimeUtil.NOW())
						{
							cmdData = -2003;
							goto IL_25E;
						}
						foreach (TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState in tianTi5v5PiPeiState.RoleList)
						{
							if (tianTi5v5PiPeiRoleState.RoleID == client.ClientData.RoleID)
							{
								if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
								{
									tianTi5v5PiPeiRoleState.State = 3;
								}
								else if (num == 1)
								{
									tianTi5v5PiPeiRoleState.State = 1;
								}
								else
								{
									tianTi5v5PiPeiRoleState.State = 2;
								}
							}
						}
						flag = tianTi5v5PiPeiState.RoleList.All((TianTi5v5PiPeiRoleState x) => x.State == 1);
						bool flag3 = tianTi5v5PiPeiState.RoleList.Any((TianTi5v5PiPeiRoleState x) => x.State == 2);
						if (flag3)
						{
							tianTi5v5PiPeiState.State = 2;
						}
					}
					foreach (TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState in tianTi5v5PiPeiState.RoleList)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5PiPeiRoleState.RoleID);
						if ((gameClient != null && tianTi5v5PiPeiRoleState.State == 1) || tianTi5v5PiPeiRoleState.State == 0)
						{
							gameClient.sendCmd<TianTi5v5PiPeiState>(3712, tianTi5v5PiPeiState, false);
						}
					}
					if (flag)
					{
						this.ProcessTianTi5v5JoinCmd(zhanDuiID, tianTi5v5PiPeiState);
					}
				}
				IL_25E:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTi5v5EnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTi5v5QuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (zhanDuiID <= 0)
				{
					num = -4013;
				}
				else if (client.ClientData.ZhanDuiZhiWu != 1)
				{
					num = -4016;
				}
				else
				{
					TianTi5v5PiPeiState tianTi5v5PiPeiState;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out tianTi5v5PiPeiState) || tianTi5v5PiPeiState.EndTicks < TimeUtil.NOW())
						{
							num = -2003;
							goto IL_15D;
						}
					}
					num = TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, zhanDuiID, client.ClientData.RoleID, 0, 0);
					if (num != 3)
					{
						tianTi5v5PiPeiState.State = 2;
						foreach (TianTi5v5PiPeiRoleState tianTi5v5PiPeiRoleState in tianTi5v5PiPeiState.RoleList)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5PiPeiRoleState.RoleID);
							if (null != gameClient)
							{
								gameClient.sendCmd<int>(nID, 0, false);
							}
						}
						client.ClientData.SignUpGameType = 0;
						return true;
					}
				}
				IL_15D:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessQuitFromZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int cmdData = this.QuitFromZhanDui(client);
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessDeleteZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int cmdData = this.DeleteZhanDui(client);
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessDeleteZhanDuiMember(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int cmdData = this.DeleteZhanDuiMember(client, Convert.ToInt32(cmdParams[1]));
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCreateZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					string xuanyan = (cmdParams.Length >= 3) ? cmdParams[2] : null;
					int cmdData = this.CreateZhanDui(client, cmdParams[1], xuanyan);
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public int CreateZhanDui(GameClient client, string teamName, string xuanyan)
		{
			int result;
			if (!this.IsGongNengOpened(client, false))
			{
				result = -400;
			}
			else
			{
				teamName = teamName.Trim();
				if (NameServerNamager.CheckInvalidCharacters(teamName, false) <= 0)
				{
					result = -4027;
				}
				else if (teamName.Length < this.RuntimeData.TeamBattleNameRange[0] || teamName.Length > this.RuntimeData.TeamBattleNameRange[1])
				{
					result = -4027;
				}
				else if (!string.IsNullOrEmpty(xuanyan) && (NameServerNamager.CheckInvalidCharacters(xuanyan, false) <= 0 || xuanyan.Length > 255))
				{
					result = -40;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (0 != Global.AvalidLevel(client, this.RuntimeData.ZhanDuiDengJiTp.Item1, this.RuntimeData.ZhanDuiDengJiTp.Item2, -1, -1))
						{
							return -19;
						}
						if (Global.IsRoleHasEnoughMoney(client, this.RuntimeData.ZhanDuiJinZuan, 40) <= 0)
						{
							return -10;
						}
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						if (tianTi5v5ZhanDuiData != null)
						{
							return -4014;
						}
						tianTi5v5ZhanDuiData = this.NewZhanDuiData(client);
						tianTi5v5ZhanDuiData.ZhanDuiName = teamName;
						tianTi5v5ZhanDuiData.XuanYan = xuanyan;
						tianTi5v5ZhanDuiData.LeaderRoleName = Global.FormatRoleName4(client);
						tianTi5v5ZhanDuiData.LeaderRoleID = client.ClientData.LocalRoleID;
						tianTi5v5ZhanDuiData.ZoneID = client.ClientData.ZoneID;
						int num = TianTiClient.getInstance().CreateZhanDui(tianTi5v5ZhanDuiData);
						if (num <= 0)
						{
							LogManager.WriteLog(2, string.Format("CreateZhanDui  ErrCode= {0} ,rleid= {1}", num, client.ClientData.RoleID), null, true);
							return num;
						}
						int num2 = num;
						string costStr = "";
						if (Global.SubRoleMoneyForGoods(client, this.RuntimeData.ZhanDuiJinZuan, 40, "创建战队") <= 0)
						{
							return -10;
						}
						tianTi5v5ZhanDuiData.ZhanDuiID = num2;
						this.UpdateZhanDuiData2DB(tianTi5v5ZhanDuiData, client.ServerId, 1);
						client.ClientData.ZhanDuiID = num2;
						client.ClientData.ZhanDuiZhiWu = 1;
						this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, num2, client.ClientData.ZhanDuiZhiWu, client.ServerId);
						EventLogManager.AddCreateZhanDuiEvent(client, (long)num2, teamName, costStr);
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					result = 0;
				}
			}
			return result;
		}

		public bool ProcessAgreeZhanDuiInvite(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int nTeamID = Convert.ToInt32(cmdParams[1]);
					int cmdData = this.AddMe2ZhanDui(client, nTeamID);
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessAgreeZhanDuiRequest(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int otherRoleID = Convert.ToInt32(cmdParams[1]);
				int cmdData = this.ZhanDuiAddMember(client, otherRoleID);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public List<TianTi5v5ZhanDuiMiniData> GetZhanDuiMiniDataList(int maxCount, int serverID)
		{
			AgeDataT<int> ageDataT = new AgeDataT<int>(this.RuntimeData.ZhanDuiSimpleList.Age, maxCount);
			lock (this.RuntimeData.Mutex)
			{
				ageDataT.Age = this.RuntimeData.ZhanDuiSimpleList.Age;
			}
			AgeDataT<List<TianTi5v5ZhanDuiMiniData>> ageDataT2 = Global.sendToDB<AgeDataT<List<TianTi5v5ZhanDuiMiniData>>, AgeDataT<int>>(3688, ageDataT, serverID);
			List<TianTi5v5ZhanDuiMiniData> result;
			if (ageDataT2 == null || ageDataT2.V == null)
			{
				result = null;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.ZhanDuiSimpleList.Age < ageDataT2.Age)
					{
						this.RuntimeData.ZhanDuiSimpleList.V = ageDataT2.V;
					}
					result = this.RuntimeData.ZhanDuiSimpleList.V;
				}
			}
			return result;
		}

		public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
		{
			TianTi5v5ZhanDuiData result;
			if (zhanDuiID <= 0)
			{
				result = null;
			}
			else
			{
				AgeDataT<int> ageDataT = new AgeDataT<int>(0L, zhanDuiID);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2))
					{
						ageDataT.Age = ageDataT2.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT3 = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<int>>(3715, ageDataT, serverID);
				if (ageDataT3 == null)
				{
					result = null;
				}
				else
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2) || ageDataT2.Age < ageDataT3.Age)
						{
							ageDataT2 = ageDataT3;
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = ageDataT3;
						}
					}
					result = ageDataT2.V;
				}
			}
			return result;
		}

		public TianTi5v5ZhanDuiData UpdateZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID, ZhanDuiDataModeTypes ZhanDuiDataModeType)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result;
			if (zhanDuiID <= 0)
			{
				result = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2))
					{
						ageDataT.Age = ageDataT2.Age;
					}
				}
				zhanDuiData.ZhanDuiDataModeType = ZhanDuiDataModeType;
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT3 = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3716, ageDataT, serverID);
				if (ageDataT3 == null)
				{
					result = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2) || ageDataT2.Age < ageDataT3.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = ageDataT3;
						}
						result = ageDataT3.V;
					}
				}
			}
			return result;
		}

		public TianTi5v5ZhanDuiData UpdateZorkZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result;
			if (zhanDuiID <= 0)
			{
				result = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2))
					{
						ageDataT.Age = ageDataT2.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT3 = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3722, ageDataT, serverID);
				if (ageDataT3 == null)
				{
					result = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2) || ageDataT2.Age < ageDataT3.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = ageDataT3;
						}
						result = ageDataT3.V;
					}
				}
			}
			return result;
		}

		public TianTi5v5ZhanDuiData UpdateEscapeZhanDuiData2DB(TianTi5v5ZhanDuiData zhanDuiData, int serverID)
		{
			int zhanDuiID = zhanDuiData.ZhanDuiID;
			TianTi5v5ZhanDuiData result;
			if (zhanDuiID <= 0)
			{
				result = null;
			}
			else
			{
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT = new AgeDataT<TianTi5v5ZhanDuiData>(0L, zhanDuiData);
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2))
					{
						ageDataT.Age = ageDataT2.Age;
					}
				}
				AgeDataT<TianTi5v5ZhanDuiData> ageDataT3 = Global.sendToDB<AgeDataT<TianTi5v5ZhanDuiData>, AgeDataT<TianTi5v5ZhanDuiData>>(3723, ageDataT, serverID);
				if (ageDataT3 == null)
				{
					result = null;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageDataT2;
						if (!this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT2) || ageDataT2.Age < ageDataT3.Age)
						{
							this.RuntimeData.ZhanDuiDataAgeDict[zhanDuiID] = ageDataT3;
						}
						result = ageDataT3.V;
					}
				}
			}
			return result;
		}

		public bool DeleteZhanDuiData2DB(int zhanDuiID, int serverID)
		{
			int num = Global.sendToDB<int, int>(3699, zhanDuiID, serverID);
			if (num >= 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT;
					if (this.RuntimeData.ZhanDuiDataAgeDict.TryGetValue(zhanDuiID, out ageDataT))
					{
						ageDataT.V = null;
					}
					return true;
				}
			}
			return false;
		}

		public void ChangeRoleZhanDuiID2DB(int roleID, int zhanDuiID, int zhiWu, int serverID)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
			if (gameClient != null)
			{
				gameClient.ClientData.ZhanDuiID = zhanDuiID;
				gameClient.ClientData.ZhanDuiZhiWu = zhiWu;
				gameClient.sendCmd<int[]>(3717, new int[]
				{
					roleID,
					zhanDuiID,
					zhiWu
				}, false);
				GlobalEventSource.getInstance().fireEvent(new EventObject(64, new object[]
				{
					gameClient
				}));
			}
			int num = Global.sendToDB<int, int[]>(3717, new int[]
			{
				roleID,
				zhanDuiID,
				zhiWu
			}, serverID);
		}

		public void BroadcastZhanDuiDataChanged(int zhanDuiID, int serverID)
		{
			List<int> list = new List<int>();
			TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, serverID);
			if (zhanDuiData != null && null != zhanDuiData.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5ZhanDuiRoleData.RoleID);
					if (null != gameClient)
					{
						gameClient.sendCmd<int>(3718, zhanDuiID, false);
					}
				}
			}
		}

		private void GetZhanDuiRoleState(TianTi5v5ZhanDuiData data)
		{
			if (data != null && null != data.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in data.teamerList)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5ZhanDuiRoleData.RoleID);
					if (null != gameClient)
					{
						tianTi5v5ZhanDuiRoleData.OnlineState = 1;
					}
					else
					{
						tianTi5v5ZhanDuiRoleData.OnlineState = 0;
					}
				}
			}
		}

		public bool GetMainInfo(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			this.InitRoleTianTi5v5Data(client);
			TianTi5v5DataAndDayPaiHang tianTi5v5DataAndDayPaiHang = new TianTi5v5DataAndDayPaiHang();
			lock (this.RuntimeData.Mutex)
			{
				tianTi5v5DataAndDayPaiHang.TianTi5v5Data = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (!ListExt.IsNullOrEmpty<TianTi5v5ZhanDuiData>(this.RuntimeData.ZhanDuiDataPaiHangList))
				{
					int count = Math.Min(this.RuntimeData.MinDayPaiMingListCount, this.RuntimeData.ZhanDuiDataPaiHangList.Count);
					tianTi5v5DataAndDayPaiHang.PaiHangRoleDataList = this.RuntimeData.ZhanDuiDataPaiHangList.GetRange(0, count);
				}
			}
			if (null != tianTi5v5DataAndDayPaiHang.TianTi5v5Data)
			{
				DuanWeiRankAward duanWeiRankAward = null;
				if (this.CanGetMonthRankAwards(client, out duanWeiRankAward))
				{
					tianTi5v5DataAndDayPaiHang.HaveMonthPaiHangAwards = 1;
				}
				long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10219");
				int num = (int)(roleParamsInt64FromDB / 100000L);
				int num2 = (int)(roleParamsInt64FromDB % 100000L);
				int offsetDay = Global.GetOffsetDay(now);
				if (offsetDay != num)
				{
					tianTi5v5DataAndDayPaiHang.TodayFightCount = 0;
				}
				else
				{
					tianTi5v5DataAndDayPaiHang.TodayFightCount = num2;
					TianTiDuanWei tianTiDuanWei;
					if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(tianTi5v5DataAndDayPaiHang.TianTi5v5Data.DuanWeiId, out tianTiDuanWei))
					{
						tianTi5v5DataAndDayPaiHang.TodayFightCount = Math.Min(tianTiDuanWei.RongYaoNum, num2);
					}
				}
			}
			client.sendCmd<TianTi5v5DataAndDayPaiHang>(3680, tianTi5v5DataAndDayPaiHang, false);
			return true;
		}

		public bool ProcessInviteOther2MyZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					List<int> list = new List<int>();
					if (!this.KuaFuServerOK())
					{
						cmdData = -11000;
					}
					else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
					{
						cmdData = -12;
					}
					else if ((string.IsNullOrEmpty(cmdParams[1]) || cmdParams[1] == "0") && cmdParams.Length >= 3)
					{
						int num = RoleManager.getInstance().GetRoleIDByRoleName(cmdParams[2], client.ServerId);
						if (num <= 0)
						{
							cmdData = -4030;
						}
						else
						{
							list.Add(num);
						}
					}
					else
					{
						foreach (string value in cmdParams[1].Split(new char[]
						{
							'|'
						}))
						{
							int num = Convert.ToInt32(value);
							if (num > 0)
							{
								list.Add(num);
							}
						}
					}
					using (List<int>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int roleid = enumerator.Current;
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData == null)
							{
								cmdData = -4013;
								break;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
							lock (this.RuntimeData.Mutex)
							{
								if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
								{
									cmdData = -4016;
									break;
								}
								if (null != gameClient)
								{
									if (gameClient.ServerId != client.ServerId)
									{
										cmdData = -4025;
										continue;
									}
									if (0 != Global.AvalidLevel(gameClient, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
									{
										cmdData = -19;
										continue;
									}
									if (null != this.GetZhanDuiData(gameClient.ClientData.ZhanDuiID, gameClient.ServerId))
									{
										cmdData = -4024;
										continue;
									}
								}
								if (null == zhanDuiData.teamerList)
								{
									cmdData = -3;
									break;
								}
								if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
								{
									cmdData = -4028;
									break;
								}
								if (zhanDuiData.teamerList.Any((TianTi5v5ZhanDuiRoleData x) => x.RoleID == roleid))
								{
									cmdData = -4014;
									continue;
								}
								List<int> list2;
								if (!this.RuntimeData.ZhanDuiInviteListDict.TryGetValue(roleid, out list2))
								{
									list2 = new List<int>();
									this.RuntimeData.ZhanDuiInviteListDict[roleid] = list2;
								}
								if (list2.Contains(zhanDuiData.ZhanDuiID))
								{
									continue;
								}
								list2.Add(zhanDuiData.ZhanDuiID);
							}
							if (null != gameClient)
							{
								this.SendZhanDuiInviteData(gameClient);
							}
						}
					}
				}
				client.sendCmd<int>(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessRequestToZhanDui(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int roleID = client.ClientData.RoleID;
					int num = Convert.ToInt32(cmdParams[1]);
					if (!this.KuaFuServerOK())
					{
						cmdData = -11000;
					}
					else
					{
						long num2 = TimeUtil.NOW();
						long key = ((long)client.ClientData.RoleID << 32) + (long)num;
						lock (this.RuntimeData.Mutex)
						{
							long num3;
							if (this.RuntimeData.RoleRequestZhanDuiTicksDict.TryGetValue(key, out num3) && num3 + 86400000L > num2)
							{
								cmdData = -2007;
								goto IL_324;
							}
						}
						if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
						{
							cmdData = -12;
						}
						else
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData != null)
							{
								cmdData = -4012;
							}
							else
							{
								zhanDuiData = this.GetZhanDuiData(num, client.ServerId);
								if (zhanDuiData == null)
								{
									cmdData = -4025;
								}
								else if (0 != Global.AvalidLevel(client, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
								{
									cmdData = -19;
								}
								else if (null == zhanDuiData.teamerList)
								{
									cmdData = -3;
								}
								else if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
								{
									cmdData = -4028;
								}
								else
								{
									TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = this.NewRoleData(client);
									if (null == tianTi5v5ZhanDuiRoleData)
									{
										cmdData = -15;
									}
									else
									{
										lock (this.RuntimeData.Mutex)
										{
											List<TianTi5v5ZhanDuiRoleData> list;
											if (!this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(num, out list))
											{
												list = new List<TianTi5v5ZhanDuiRoleData>();
												this.RuntimeData.ZhanDuiRequestListDict[num] = list;
											}
											if (list.Any((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID))
											{
												goto IL_324;
											}
											list.Add(tianTi5v5ZhanDuiRoleData);
											this.RuntimeData.RoleRequestZhanDuiTicksDict[key] = num2;
										}
										GameClient gameClient = GameManager.ClientMgr.FindClient(zhanDuiData.LeaderRoleID);
										if (null != gameClient)
										{
											cmdData = this.SendZhanDuiRequestData(gameClient);
										}
									}
								}
							}
						}
					}
					IL_324:;
				}
				client.sendCmd<int>(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public int SendZhanDuiInviteData(GameClient client)
		{
			int result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = 0;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.TeamApply.Contains(client.ClientData.MapCode))
					{
						return -4019;
					}
					List<int> list;
					if (!this.RuntimeData.ZhanDuiInviteListDict.TryGetValue(client.ClientData.RoleID, out list))
					{
						return 0;
					}
					try
					{
						if (0 != Global.AvalidLevel(client, this.RuntimeData.LvLimit.Item1, this.RuntimeData.LvLimit.Item2, -1, -1))
						{
							this.RuntimeData.ZhanDuiInviteListDict.Remove(client.ClientData.RoleID);
							return -19;
						}
						if (null != this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId))
						{
							return -4024;
						}
						foreach (int zhanDuiID in list)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							client.sendCmd(3689, string.Format("{0}:{1}:{2}", zhanDuiData.ZhanDuiID, zhanDuiData.LeaderRoleName, zhanDuiData.ZhanDuiName), false);
						}
					}
					finally
					{
						list.Clear();
					}
				}
				result = 0;
			}
			return result;
		}

		public int SendZhanDuiRequestData(GameClient client)
		{
			int result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = 0;
			}
			else if (client.ClientData.ZhanDuiID <= 0 || client.ClientData.ZhanDuiZhiWu == 0)
			{
				result = 0;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.TeamApply.Contains(client.ClientData.MapCode))
					{
						return -4019;
					}
					List<TianTi5v5ZhanDuiRoleData> list;
					if (!this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
					{
						return 0;
					}
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (zhanDuiData == null || zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4013;
					}
					if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
					{
						list.Clear();
						return -4028;
					}
					foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in list)
					{
						if (tianTi5v5ZhanDuiRoleData.OnlineState == 0)
						{
							tianTi5v5ZhanDuiRoleData.OnlineState = 1;
							client.sendCmd(3720, string.Format("{0}:{1}:{2}", tianTi5v5ZhanDuiRoleData.RoleID, tianTi5v5ZhanDuiRoleData.RoleName, tianTi5v5ZhanDuiRoleData.RoleOcc), false);
						}
					}
				}
				result = 0;
			}
			return result;
		}

		public bool ProcessGetZhanDuiList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					List<TianTi5v5ZhanDuiMiniData> zhanDuiMiniDataList = this.GetZhanDuiMiniDataList(this.RuntimeData.MaxZhanDuiNum, client.ServerId);
					client.sendCmd<List<TianTi5v5ZhanDuiMiniData>>(nID, zhanDuiMiniDataList, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public int AddMe2ZhanDui(GameClient client, int nTeamID)
		{
			int result;
			if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, nTeamID, 4), 10000))
			{
				result = -12;
			}
			else if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (0 != Global.AvalidLevel(client, this.RuntimeData.ZhanDuiDengJiTp.Item1, this.RuntimeData.ZhanDuiDengJiTp.Item2, -1, -1))
			{
				result = -19;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (zhanDuiData != null)
				{
					if (zhanDuiData.ZhanDuiID == nTeamID)
					{
						result = -4014;
					}
					else
					{
						result = -4024;
					}
				}
				else
				{
					zhanDuiData = this.GetZhanDuiData(nTeamID, client.ServerId);
					if (null == zhanDuiData)
					{
						result = -4013;
					}
					else
					{
						TianTi5v5ZhanDuiRoleData item = this.NewRoleData(client);
						lock (this.RuntimeData.Mutex)
						{
							zhanDuiData = this.GetZhanDuiData(nTeamID, client.ServerId);
							if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
							{
								return -4028;
							}
							zhanDuiData.teamerList.Add(item);
							zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
							int num = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, 1);
							if (num < 0)
							{
								zhanDuiData.teamerList.Remove(item);
								LogManager.WriteLog(2, string.Format("AddMe2ZhanDui ErrCode={0} ,roleid ={1} teamID={2}", num, client.ClientData.RoleID, nTeamID), null, true);
								return -11000;
							}
							client.ClientData.ZhanDuiID = zhanDuiData.ZhanDuiID;
							client.ClientData.ZhanDuiZhiWu = 0;
							this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, nTeamID, 0, client.ServerId);
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, 1);
						}
						if (null != zhanDuiData)
						{
							EventLogManager.AddAttendZhanDuiEvent(client, (long)nTeamID, zhanDuiData.ZhanDuiName, zhanDuiData.LeaderRoleID, string.Join("|", zhanDuiData.teamerList.ConvertAll<string>((TianTi5v5ZhanDuiRoleData x) => x.RoleName)));
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
						result = 0;
					}
				}
			}
			return result;
		}

		public int ZhanDuiAddMember(GameClient client, int otherRoleID)
		{
			int result;
			if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (!this.KuaFuServerOK())
				{
					result = -11000;
				}
				else
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (null == zhanDuiData)
					{
						result = -4013;
					}
					else
					{
						TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = null;
						GameClient gameClient = GameManager.ClientMgr.FindClient(otherRoleID);
						if (null != gameClient)
						{
							if (gameClient.ClientData.ZhanDuiID > 0)
							{
								return -4014;
							}
							tianTi5v5ZhanDuiRoleData = this.NewRoleData(gameClient);
						}
						else
						{
							RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, otherRoleID), 0);
							if (null == roleDataEx)
							{
								return -15;
							}
							if (roleDataEx.ZhanDuiID > 0)
							{
								return -4014;
							}
						}
						lock (this.RuntimeData.Mutex)
						{
							if (client.ClientData.RoleID != zhanDuiData.LeaderRoleID)
							{
								return -4016;
							}
							List<TianTi5v5ZhanDuiRoleData> list;
							if (this.RuntimeData.ZhanDuiRequestListDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
							{
								if (null == tianTi5v5ZhanDuiRoleData)
								{
									tianTi5v5ZhanDuiRoleData = list.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == otherRoleID);
								}
								if (null != tianTi5v5ZhanDuiRoleData)
								{
									list.RemoveAll((TianTi5v5ZhanDuiRoleData x) => x.RoleID == otherRoleID);
								}
							}
							if (null == tianTi5v5ZhanDuiRoleData)
							{
								return -11003;
							}
							zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							if (zhanDuiData.teamerList.Count >= this.RuntimeData.MaxTeamCnt)
							{
								return -4028;
							}
							zhanDuiData.teamerList.Add(tianTi5v5ZhanDuiRoleData);
							zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
							int num = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, 1);
							if (num < 0)
							{
								zhanDuiData.teamerList.Remove(tianTi5v5ZhanDuiRoleData);
								LogManager.WriteLog(2, string.Format("ZhanDuiAddMember ErrCode={0} ,roleid ={1} teamID={2}", num, client.ClientData.RoleID, zhanDuiID), null, true);
								return -11000;
							}
							this.ChangeRoleZhanDuiID2DB(otherRoleID, zhanDuiID, 0, client.ServerId);
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, 1);
						}
						if (null != zhanDuiData)
						{
							EventLogManager.AddAttendZhanDuiEvent(client, (long)zhanDuiID, zhanDuiData.ZhanDuiName, zhanDuiData.LeaderRoleID, string.Join("|", zhanDuiData.teamerList.ConvertAll<string>((TianTi5v5ZhanDuiRoleData x) => x.RoleName)));
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
						result = 0;
					}
				}
			}
			return result;
		}

		public int QuitFromZhanDui(GameClient client)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = null;
				lock (this.RuntimeData.Mutex)
				{
					tianTi5v5ZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (tianTi5v5ZhanDuiData == null)
					{
						return -4013;
					}
					if (tianTi5v5ZhanDuiData.LeaderRoleID == client.ClientData.RoleID)
					{
						return -4017;
					}
					TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = tianTi5v5ZhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
					if (null == tianTi5v5ZhanDuiRoleData)
					{
						return -4017;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					tianTi5v5ZhanDuiData.teamerList.Remove(tianTi5v5ZhanDuiRoleData);
					tianTi5v5ZhanDuiData.ZhanDouLi = tianTi5v5ZhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					int num = TianTiClient.getInstance().UpdateZhanDuiData(tianTi5v5ZhanDuiData, 1);
					if (num < 0)
					{
						tianTi5v5ZhanDuiData.teamerList.Add(tianTi5v5ZhanDuiRoleData);
						LogManager.WriteLog(2, string.Format("UpdateZhanDuiData ErrCode={0} ,roleid ={1} teamID={2}", num, client.ClientData.RoleID, tianTi5v5ZhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					client.ClientData.ZhanDuiID = 0;
					client.ClientData.ZhanDuiZhiWu = 0;
					this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, 0, 0, client.ServerId);
					this.UpdateZhanDuiData2DB(tianTi5v5ZhanDuiData, client.ServerId, 1);
				}
				EventLogManager.QuitZhanDuiEvent(client, (long)tianTi5v5ZhanDuiData.ZhanDuiID, tianTi5v5ZhanDuiData.ZhanDuiName, tianTi5v5ZhanDuiData.LeaderRoleID, "");
				result = 0;
			}
			return result;
		}

		public int DeleteZhanDuiMember(GameClient client, int targetRoleID)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
			{
				result = -12;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (zhanDuiData == null)
					{
						return -4013;
					}
					if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4016;
					}
					TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == targetRoleID);
					if (null == tianTi5v5ZhanDuiRoleData)
					{
						return -4017;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					zhanDuiData.teamerList.Remove(tianTi5v5ZhanDuiRoleData);
					zhanDuiData.ZhanDouLi = zhanDuiData.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					int num = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, 1);
					if (num < 0)
					{
						zhanDuiData.teamerList.Add(tianTi5v5ZhanDuiRoleData);
						LogManager.WriteLog(2, string.Format("UpdateZhanDuiData ErrCode={0} ,roleid ={1} teamID={2}", num, client.ClientData.RoleID, zhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(targetRoleID);
					if (null != gameClient)
					{
						gameClient.ClientData.ZhanDuiID = 0;
						gameClient.ClientData.ZhanDuiZhiWu = 0;
					}
					this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, 1);
					this.ChangeRoleZhanDuiID2DB(targetRoleID, 0, 0, client.ServerId);
				}
				result = 0;
			}
			return result;
		}

		public int DeleteZhanDui(GameClient client)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 0), 10000))
			{
				result = -12;
			}
			else
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = null;
				lock (this.RuntimeData.Mutex)
				{
					tianTi5v5ZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					if (tianTi5v5ZhanDuiData == null)
					{
						return -4013;
					}
					if (tianTi5v5ZhanDuiData.LeaderRoleID != client.ClientData.RoleID)
					{
						return -4016;
					}
					int num = TianTiClient.getInstance().DeleteZhanDui(GameManager.ServerId, client.ClientData.RoleID, tianTi5v5ZhanDuiData.ZhanDuiID);
					if (num < 0)
					{
						LogManager.WriteLog(2, string.Format("DeleteZhanDui ErrCode={0} ,roleid ={1} ZhanDuiID={2}", num, client.ClientData.RoleID, tianTi5v5ZhanDuiData.ZhanDuiID), null, true);
						return -11000;
					}
					this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					if (!this.DeleteZhanDuiData2DB(tianTi5v5ZhanDuiData.ZhanDuiID, client.ServerId))
					{
						LogManager.WriteLog(2, "解散战队,本地数据库删除战队数据时失败,zhanduiid=" + tianTi5v5ZhanDuiData.ZhanDuiID, null, true);
					}
					foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in tianTi5v5ZhanDuiData.teamerList)
					{
						this.ChangeRoleZhanDuiID2DB(tianTi5v5ZhanDuiRoleData.RoleID, 0, 0, client.ServerId);
						GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5ZhanDuiRoleData.RoleID);
						if (null != gameClient)
						{
							gameClient.ClientData.ZhanDuiID = 0;
							gameClient.ClientData.ZhanDuiZhiWu = 0;
						}
					}
				}
				EventLogManager.DeleteZhanDuiEvent(client, (long)tianTi5v5ZhanDuiData.ZhanDuiID, tianTi5v5ZhanDuiData.ZhanDuiName, client.ClientData.RoleID, "");
				result = 0;
			}
			return result;
		}

		public bool ProcessUpdateZhanDuiXuanYan(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int cmdData = this.UpdateZhanDuiXuanYan(client, cmdParams[1]);
					client.sendCmd<int>(nID, cmdData, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetMyZhanDuiInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					lock (this.RuntimeData.Mutex)
					{
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						if (null != tianTi5v5ZhanDuiData)
						{
							tianTi5v5ZhanDuiData = tianTi5v5ZhanDuiData.Clone();
							this.GetZhanDuiRoleState(tianTi5v5ZhanDuiData);
						}
						client.sendCmd<TianTi5v5ZhanDuiData>(nID, tianTi5v5ZhanDuiData, false);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessChangeZhanDuiLeader(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int newLeaderId = Convert.ToInt32(cmdParams[1]);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					int num = 0;
					if (!GlobalEventSource4Scene.getInstance().fireEvent(new PreZhanDuiChangeMemberEventObject(client, client.ClientData.ZhanDuiID, 4), 10000))
					{
						num = -12;
					}
					else
					{
						lock (this.RuntimeData.Mutex)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (zhanDuiData == null)
							{
								num = -4013;
								goto IL_1EF;
							}
							if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
							{
								num = -4016;
								goto IL_1EF;
							}
							TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == newLeaderId);
							if (null == tianTi5v5ZhanDuiRoleData)
							{
								num = -4017;
								goto IL_1EF;
							}
							num = TianTiClient.getInstance().UpdateZhanDuiData(zhanDuiData, 1);
							if (num < 0)
							{
								goto IL_1EF;
							}
							zhanDuiData.LeaderRoleID = newLeaderId;
							zhanDuiData.LeaderRoleName = Global.FormatRoleName4(client);
							EventLogManager.ChangeZhanDuiLeaderEvent(client, (long)zhanDuiData.ZhanDuiID, zhanDuiData.ZhanDuiName, client.ClientData.RoleID, newLeaderId, "");
							this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, 1);
							this.ChangeRoleZhanDuiID2DB(newLeaderId, zhanDuiData.ZhanDuiID, 1, client.ServerId);
							this.ChangeRoleZhanDuiID2DB(client.ClientData.RoleID, zhanDuiData.ZhanDuiID, 0, client.ServerId);
						}
						this.BroadcastZhanDuiDataChanged(client.ClientData.ZhanDuiID, client.ServerId);
					}
					IL_1EF:
					client.sendCmd<int>(nID, num, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessZhanDuiKF5V5JingJiData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool InitRoleTianTi5v5Data(GameClient client)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = true;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (null == zhanDuiData)
				{
					result = true;
				}
				else
				{
					bool flag = false;
					DateTime t = TimeUtil.NowDateTime().AddMonths(-1);
					t = new DateTime(t.Year, t.Month, 1);
					lock (this.RuntimeData.Mutex)
					{
						this.UpdateByMonth(zhanDuiData);
						if (this.RuntimeData.ModifyTime > t)
						{
							int num = this.RuntimeData.MaxPaiMingRank + 1;
							TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
							if (this.RuntimeData.ZhanDuiDataPaiHangDict.TryGetValue(zhanDuiData.ZhanDuiID, out tianTi5v5ZhanDuiData))
							{
								num = tianTi5v5ZhanDuiData.DuanWeiRank;
							}
							if (zhanDuiData.DuanWeiRank != num)
							{
								flag = true;
								zhanDuiData.DuanWeiRank = num;
							}
							num = this.RuntimeData.MaxPaiMingRank + 1;
							if (this.RuntimeData.ZhanDuiDataMonthPaiHangDict.TryGetValue(zhanDuiData.ZhanDuiID, out tianTi5v5ZhanDuiData))
							{
								num = tianTi5v5ZhanDuiData.DuanWeiRank;
							}
							if (zhanDuiData.MonthDuanWeiRank != num)
							{
								flag = true;
								zhanDuiData.MonthDuanWeiRank = num;
							}
						}
						int duanWeiId;
						if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(zhanDuiData.DuanWeiJiFen, out duanWeiId))
						{
							zhanDuiData.DuanWeiId = duanWeiId;
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public bool UpdateByMonth(TianTi5v5ZhanDuiData data)
		{
			DateTime now = DateTime.Now;
			DateTime dateTime = new DateTime(now.Year, now.Month, 1);
			DateTime t = dateTime.AddMonths(-1);
			bool result;
			if (data.LastFightTime < dateTime)
			{
				data.DuanWeiJiFen = 0;
				data.DuanWeiId = 1;
				data.DuanWeiRank = this.RuntimeData.RankData.MaxPaiMingRank;
				data.LianSheng = 0;
				data.FightCount = 0;
				data.SuccessCount = 0;
				if (data.LastFightTime < t)
				{
					data.MonthDuanWeiRank = 0;
				}
				data.LastFightTime = dateTime;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				TianTiBirthPoint tianTiBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out tianTiBirthPoint))
				{
					posX = tianTiBirthPoint.PosX;
					posY = tianTiBirthPoint.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			int num = (int)kuaFuServerLoginData.GameId;
			KuaFu5v5FuBenData kuaFu5v5FuBenData;
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5FuBenItem tianTi5v5FuBenItem;
				if (!this.RuntimeData.TianTi5v5FuBenItemDict.TryGetValue(num, out tianTi5v5FuBenItem))
				{
					tianTi5v5FuBenItem = new TianTi5v5FuBenItem
					{
						GameId = num
					};
					tianTi5v5FuBenItem.FuBenSeqId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					this.RuntimeData.TianTi5v5FuBenItemDict[tianTi5v5FuBenItem.GameId] = tianTi5v5FuBenItem;
				}
				kuaFuServerLoginData.FuBenSeqId = tianTi5v5FuBenItem.FuBenSeqId;
				this.RuntimeData.FuBenDataDict.TryGetValue(num, out kuaFu5v5FuBenData);
			}
			if (null == kuaFu5v5FuBenData)
			{
				kuaFu5v5FuBenData = TianTiClient.getInstance().ZhanDuiGetFuBenData(num);
				if (null != kuaFu5v5FuBenData)
				{
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.FuBenDataDict[num] = kuaFu5v5FuBenData;
					}
				}
			}
			return kuaFu5v5FuBenData != null && kuaFu5v5FuBenData.LoginInfo.KuaFuServerId == GameManager.ServerId && kuaFu5v5FuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId);
		}

		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			int num = (int)clientKuaFuServerLoginData.GameId;
			KuaFu5v5FuBenData kuaFu5v5FuBenData;
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5FuBenItem tianTi5v5FuBenItem;
				if (!this.RuntimeData.TianTi5v5FuBenItemDict.TryGetValue(num, out tianTi5v5FuBenItem))
				{
					tianTi5v5FuBenItem = new TianTi5v5FuBenItem
					{
						GameId = num
					};
					tianTi5v5FuBenItem.FuBenSeqId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					this.RuntimeData.TianTi5v5FuBenItemDict[tianTi5v5FuBenItem.GameId] = tianTi5v5FuBenItem;
				}
				clientKuaFuServerLoginData.FuBenSeqId = tianTi5v5FuBenItem.FuBenSeqId;
				this.RuntimeData.FuBenDataDict.TryGetValue(num, out kuaFu5v5FuBenData);
			}
			if (null == kuaFu5v5FuBenData)
			{
				kuaFu5v5FuBenData = TianTiClient.getInstance().ZhanDuiGetFuBenData(num);
				if (null != kuaFu5v5FuBenData)
				{
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.FuBenDataDict[num] = kuaFu5v5FuBenData;
					}
				}
			}
			bool result;
			if (kuaFu5v5FuBenData == null || kuaFu5v5FuBenData.State >= 3)
			{
				result = false;
			}
			else
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData;
				if (kuaFu5v5FuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
				{
					client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				}
				int posX;
				int posY;
				int birthPoint = this.GetBirthPoint(client, out posX, out posY);
				if (birthPoint <= 0)
				{
					result = false;
				}
				else
				{
					int index = (int)clientKuaFuServerLoginData.GameId % this.RuntimeData.MapCodeList.Count;
					client.ClientData.MapCode = this.RuntimeData.MapCodeList[index];
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					int num2 = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.GameId2FuBenSeq.TryGetValue((int)clientKuaFuServerLoginData.GameId, out num2))
						{
							num2 = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.RuntimeData.GameId2FuBenSeq[(int)clientKuaFuServerLoginData.GameId] = num2;
						}
					}
					clientKuaFuServerLoginData.FuBenSeqId = num2;
					client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
					client.SceneType = 55;
					result = true;
				}
			}
			return result;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 108, hint);
		}

		public bool CanGetMonthRankAwards(GameClient client, out DuanWeiRankAward duanWeiRankAward)
		{
			duanWeiRankAward = null;
			bool result;
			if (TimeUtil.GetOffsetMonth(this.RuntimeData.RankData.ModifyTime) != TimeUtil.GetOffsetMonth(TimeUtil.NowDateTime()))
			{
				result = false;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				lock (this.RuntimeData.Mutex)
				{
					if (zhanDuiData != null && this.RuntimeData.ZhanDuiDataMonthPaiHangDict != null)
					{
						int key = this.RuntimeData.RankData.MaxPaiMingRank;
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
						if (this.RuntimeData.ZhanDuiDataMonthPaiHangDict.TryGetValue(zhanDuiData.ZhanDuiID, out tianTi5v5ZhanDuiData))
						{
							key = tianTi5v5ZhanDuiData.MonthDuanWeiRank;
						}
						if (this.RuntimeData.DuanWeiRankAwardDict.TryGetValue(key, out duanWeiRankAward))
						{
							DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "10220");
							DateTime dateTime = TimeUtil.NowDateTime();
							if (roleParamsDateTimeFromDB.Month == dateTime.Month && roleParamsDateTimeFromDB.Year == dateTime.Year)
							{
								return false;
							}
							if (new DateTime(roleParamsDateTimeFromDB.Year, roleParamsDateTimeFromDB.Month, 1) >= new DateTime(this.RuntimeData.ModifyTime.Year, this.RuntimeData.ModifyTime.Month, 1))
							{
								return false;
							}
							TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = zhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
							if (tianTi5v5ZhanDuiRoleData == null)
							{
								return false;
							}
							int offsetMonthDayID = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime().AddMonths(-1));
							int[] monthFightCounts = tianTi5v5ZhanDuiRoleData.MonthFightCounts;
							if (monthFightCounts == null || monthFightCounts.Length != 4)
							{
								return false;
							}
							int num = 0;
							if (monthFightCounts[0] == offsetMonthDayID)
							{
								num = monthFightCounts[1];
							}
							else if (monthFightCounts[2] == offsetMonthDayID)
							{
								num = monthFightCounts[3];
							}
							if (num < this.RuntimeData.TeamAwardLimit)
							{
								return false;
							}
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public bool KuaFuServerOK()
		{
			return TianTiClient.getInstance().IsKuaFuServerOK();
		}

		public int UpdateZhanDuiXuanYan(GameClient client, string xuanYan)
		{
			int result;
			if (!this.KuaFuServerOK())
			{
				result = -11000;
			}
			else
			{
				xuanYan = xuanYan.Trim();
				if (NameServerNamager.CheckInvalidCharacters(xuanYan, false) <= 0)
				{
					result = -40;
				}
				else
				{
					TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
					lock (this.RuntimeData.Mutex)
					{
						if (zhanDuiData == null)
						{
							return -4013;
						}
						if (zhanDuiData.LeaderRoleID != client.ClientData.RoleID)
						{
							return -4016;
						}
						int num = TianTiClient.getInstance().UpdateZhanDuiXuanYan((long)zhanDuiData.ZhanDuiID, xuanYan);
						if (num < 0)
						{
							return num;
						}
						zhanDuiData.XuanYan = xuanYan;
						this.UpdateZhanDuiData2DB(zhanDuiData, client.ServerId, 1);
					}
					result = 0;
				}
			}
			return result;
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 55)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5Scene tianTi5v5Scene = null;
					if (!this.TianTi5v5SceneDict.TryGetValue(fuBenSeqID, out tianTi5v5Scene))
					{
						tianTi5v5Scene = new TianTi5v5Scene();
						tianTi5v5Scene.CopyMap = copyMap;
						tianTi5v5Scene.CleanAllInfo();
						tianTi5v5Scene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						tianTi5v5Scene.m_nMapCode = mapCode;
						tianTi5v5Scene.CopyMapId = copyMap.CopyMapID;
						tianTi5v5Scene.FuBenSeqId = fuBenSeqID;
						tianTi5v5Scene.m_nPlarerCount = 1;
						KuaFu5v5FuBenData fuBenData;
						if (this.RuntimeData.FuBenDataDict.TryGetValue(tianTi5v5Scene.GameId, out fuBenData))
						{
							tianTi5v5Scene.FuBenData = fuBenData;
						}
						this.TianTi5v5SceneDict[fuBenSeqID] = tianTi5v5Scene;
					}
					else
					{
						tianTi5v5Scene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					this.SaveClientBattleSide(tianTi5v5Scene, client);
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					tianTi5v5Scene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.BattleWhichSide, true);
					tianTi5v5Scene.ClientDict[client.ClientData.RoleID] = client;
					if (!tianTi5v5Scene.ZhanDuiDataDict.Any((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID))
					{
						TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
						this.UpdateByMonth(zhanDuiData);
						tianTi5v5Scene.ZhanDuiDataDict.Add(new Tuple<TianTi5v5ZhanDuiData, int>(zhanDuiData, client.ServerId));
					}
				}
				TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, client.ClientData.ZhanDuiID, client.ClientData.RoleID, 5, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 34);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 55)
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTi5v5Scene tianTi5v5Scene;
					if (this.TianTi5v5SceneDict.TryRemove(copyMap.FuBenSeqID, out tianTi5v5Scene))
					{
						this.RuntimeData.GameId2FuBenSeq.Remove(tianTi5v5Scene.GameId);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void SaveClientBattleSide(TianTi5v5Scene scene, GameClient client)
		{
			TianTi5v5RoleMiniData tianTi5v5RoleMiniData;
			if (!scene.RoleIdDuanWeiIdDict.TryGetValue(client.ClientData.ZhanDuiID, out tianTi5v5RoleMiniData))
			{
				tianTi5v5RoleMiniData = new TianTi5v5RoleMiniData();
				scene.RoleIdDuanWeiIdDict[client.ClientData.ZhanDuiID] = tianTi5v5RoleMiniData;
				tianTi5v5RoleMiniData.RoleId = client.ClientData.ZhanDuiID;
			}
			tianTi5v5RoleMiniData.BattleWitchSide = client.ClientData.BattleWhichSide;
			TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
			if (null != zhanDuiData)
			{
				tianTi5v5RoleMiniData.DuanWeiId = zhanDuiData.DuanWeiId;
				tianTi5v5RoleMiniData.RoleName = zhanDuiData.ZhanDuiName;
				if (client.ClientData.RoleID == zhanDuiData.LeaderRoleID)
				{
					tianTi5v5RoleMiniData.ZoneId = client.ClientData.ZoneID;
				}
			}
		}

		private TianTi5v5RoleMiniData GetEnemyBattleSide(TianTi5v5Scene scene, GameClient client)
		{
			foreach (KeyValuePair<int, TianTi5v5RoleMiniData> keyValuePair in scene.RoleIdDuanWeiIdDict)
			{
				if (client.ClientData.ZhanDuiID != keyValuePair.Key)
				{
					return keyValuePair.Value;
				}
			}
			return scene.RoleIdDuanWeiIdDict.Values.FirstOrDefault<TianTi5v5RoleMiniData>();
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= TianTi5v5Manager.NextHeartBeatTicks)
			{
				TianTi5v5Manager.NextHeartBeatTicks = num + 1020L;
				foreach (TianTi5v5Scene tianTi5v5Scene in this.TianTi5v5SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = tianTi5v5Scene.FuBenSeqId;
						int copyMapId = tianTi5v5Scene.CopyMapId;
						int nMapCode = tianTi5v5Scene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = tianTi5v5Scene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							tianTi5v5Scene.ScoreInfoData.Count1 = 0L;
							tianTi5v5Scene.ScoreInfoData.Count2 = 0;
							List<KeyValuePair<int, Tuple<int, bool>>> list = new List<KeyValuePair<int, Tuple<int, bool>>>();
							foreach (int num3 in tianTi5v5Scene.RoleSideStateDict.Keys)
							{
								Tuple<int, bool> tuple = tianTi5v5Scene.RoleSideStateDict[num3];
								GameClient gameClient = GameManager.ClientMgr.FindClient(num3);
								if (null == gameClient)
								{
									if (tuple.Item2)
									{
										list.Add(new KeyValuePair<int, Tuple<int, bool>>(num3, new Tuple<int, bool>(tuple.Item1, false)));
									}
								}
								else if (!gameClient.ClientData.FirstPlayStart)
								{
									if (tuple.Item1 == 1)
									{
										tianTi5v5Scene.ScoreInfoData.Count1 += 1L;
									}
									else if (tuple.Item1 == 2)
									{
										tianTi5v5Scene.ScoreInfoData.Count2++;
									}
								}
							}
							foreach (KeyValuePair<int, Tuple<int, bool>> keyValuePair in list)
							{
								tianTi5v5Scene.RoleSideStateDict[keyValuePair.Key] = keyValuePair.Value;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<KuaFu5v5ScoreInfoData>(3708, tianTi5v5Scene.ScoreInfoData, copyMap);
							if (tianTi5v5Scene.m_eStatus == 0)
							{
								tianTi5v5Scene.m_lPrepareTime = num2;
								tianTi5v5Scene.m_lBeginTime = num2 + (long)(this.RuntimeData.WaitingEnterSecs * 1000);
								tianTi5v5Scene.m_eStatus = 1;
								tianTi5v5Scene.StateTimeData.GameType = 34;
								tianTi5v5Scene.StateTimeData.State = tianTi5v5Scene.m_eStatus;
								tianTi5v5Scene.StateTimeData.EndTicks = tianTi5v5Scene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTi5v5Scene.StateTimeData, tianTi5v5Scene.CopyMap);
							}
							else if (tianTi5v5Scene.m_eStatus == 1)
							{
								bool flag2 = false;
								if (num2 >= tianTi5v5Scene.m_lBeginTime)
								{
									flag2 = true;
								}
								else
								{
									bool flag3;
									if (tianTi5v5Scene.RoleSideStateDict.Count >= tianTi5v5Scene.FuBenData.RoleDict.Count)
									{
										flag3 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
									}
									else
									{
										flag3 = true;
									}
									if (!flag3)
									{
										flag2 = true;
									}
								}
								if (flag2)
								{
									tianTi5v5Scene.m_lBeginTime = num2;
									tianTi5v5Scene.m_eStatus = 2;
									tianTi5v5Scene.m_lEndTime = num2 + (long)(this.RuntimeData.FightingSecs * 1000);
									tianTi5v5Scene.StateTimeData.GameType = 34;
									tianTi5v5Scene.StateTimeData.State = tianTi5v5Scene.m_eStatus;
									tianTi5v5Scene.StateTimeData.EndTicks = tianTi5v5Scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTi5v5Scene.StateTimeData, tianTi5v5Scene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
							}
							else if (tianTi5v5Scene.m_eStatus == 2)
							{
								if (num2 >= tianTi5v5Scene.m_lEndTime)
								{
									this.CompleteTianTi5v5Scene(tianTi5v5Scene, 0);
								}
								else if (num2 - tianTi5v5Scene.m_lBeginTime > 1000L)
								{
									this.SceneCheckComplete(tianTi5v5Scene, true);
								}
							}
							else if (tianTi5v5Scene.m_eStatus == 3)
							{
								this.ProcessEnd(tianTi5v5Scene, now, num);
							}
							else if (tianTi5v5Scene.m_eStatus == 4)
							{
								if (num2 >= tianTi5v5Scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(tianTi5v5Scene.m_lLeaveTime);
									tianTi5v5Scene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int i = 0; i < clientsList.Count; i++)
											{
												GameClient gameClient2 = clientsList[i];
												if (gameClient2 != null)
												{
													KuaFuManager.getInstance().GotoLastMap(gameClient2);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "跨服组队竞技清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5Scene tianTi5v5Scene;
				if (this.TianTi5v5SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTi5v5Scene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, tianTi5v5Scene.StateTimeData, false);
				}
			}
		}

		public void CompleteTianTi5v5Scene(TianTi5v5Scene scene, int successSide)
		{
			scene.m_eStatus = 3;
			scene.SuccessSide = successSide;
		}

		public void CancleTianTi5v5Scene(int gameId)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.CancledGameIdDict.Add(gameId);
			}
		}

		private int SceneCheckComplete(TianTi5v5Scene scene, bool complete = true)
		{
			int num = 0;
			if (scene.RoleSideStateDict.Count > 0)
			{
				int existsSide = scene.RoleSideStateDict.First<KeyValuePair<int, Tuple<int, bool>>>().Value.Item1;
				if (scene.RoleSideStateDict.All((KeyValuePair<int, Tuple<int, bool>> x) => x.Value.Item1 == existsSide))
				{
					num = existsSide;
				}
			}
			else
			{
				num = scene.LastLeaveZhanDuiID;
			}
			if (num != 0 && complete)
			{
				this.CompleteTianTi5v5Scene(scene, num);
			}
			return num;
		}

		private void SceneRemoveRole(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTi5v5Scene tianTi5v5Scene;
				if (this.TianTi5v5SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTi5v5Scene))
				{
					if (tianTi5v5Scene.m_eStatus < 3)
					{
						tianTi5v5Scene.RoleSideStateDict.Remove(client.ClientData.RoleID);
						if (tianTi5v5Scene.RoleSideStateDict.Count == 0)
						{
							tianTi5v5Scene.LastLeaveZhanDuiID = client.ClientData.BattleWhichSide;
						}
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			if (client.SceneType == 55)
			{
				this.SceneRemoveRole(other);
				GameManager.ClientMgr.ChangePosition(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, other, this.RuntimeData.TeamBattleWatch[0], this.RuntimeData.TeamBattleWatch[1], 4, 159, 0);
			}
		}

		public void RoleLeaveFuBen(GameClient client)
		{
			if (client.SceneType == 55)
			{
				this.SceneRemoveRole(client);
			}
		}

		private void ProcessEnd(TianTi5v5Scene scene, DateTime now, long nowTicks)
		{
			scene.m_eStatus = 4;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
			TianTiClient.getInstance().ZhanDuiRoleChangeState(GameManager.ServerId, 0, 0, 3, scene.GameId);
			scene.StateTimeData.GameType = 34;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			if (scene.SuccessSide == -1)
			{
				this.GameCanceled(scene);
			}
			else
			{
				this.GiveAwards(scene);
			}
		}

		public void GiveAwards(TianTi5v5Scene scene)
		{
			try
			{
				DateTime endTime = TimeUtil.NowDateTime();
				DateTime now = endTime.Subtract(this.RuntimeData.RefreshTime);
				List<GameClient> list = scene.ClientDict.Values.ToList<GameClient>();
				HashSet<int> hashSet = new HashSet<int>();
				if (list != null && list.Count > 0)
				{
					int offsetDayNow = Global.GetOffsetDayNow();
					int i = 0;
					while (i < list.Count)
					{
						GameClient client = list[i];
						if (client != null)
						{
							bool flag = false;
							GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.RoleID);
							if (gameClient != null && gameClient.SceneType == 55)
							{
								flag = true;
							}
							Tuple<TianTi5v5ZhanDuiData, int> tuple = scene.ZhanDuiDataDict.Find((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID);
							if (tuple != null)
							{
								TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = tuple.Item1;
								tianTi5v5ZhanDuiData = this.GetZhanDuiData(tianTi5v5ZhanDuiData.ZhanDuiID, tuple.Item2);
								if (null != tianTi5v5ZhanDuiData)
								{
									int index = scene.ZhanDuiDataDict.FindIndex((Tuple<TianTi5v5ZhanDuiData, int> x) => x.Item1.ZhanDuiID == client.ClientData.ZhanDuiID);
									scene.ZhanDuiDataDict[index] = new Tuple<TianTi5v5ZhanDuiData, int>(tianTi5v5ZhanDuiData, tuple.Item2);
									bool flag2 = client.ClientData.BattleWhichSide == scene.SuccessSide;
									if (flag2)
									{
										int zhanDuiID = client.ClientData.ZhanDuiID;
									}
									int duanWeiId = tianTi5v5ZhanDuiData.DuanWeiId;
									TianTi5v5RoleMiniData enemyBattleSide = this.GetEnemyBattleSide(scene, client);
									int num = 0;
									int num2 = 0;
									int num3 = 0;
									bool flag3 = hashSet.Add(client.ClientData.ZhanDuiID);
									long num4 = Global.GetRoleParamsInt64FromDB(client, "10219");
									int num5 = (int)(num4 / 100000L);
									int num6 = (int)(num4 % 100000L);
									int offsetDay = Global.GetOffsetDay(now);
									if (offsetDay != num5)
									{
										num5 = offsetDay;
										num6 = 1;
									}
									else
									{
										num6++;
									}
									num4 = (long)num6 + 100000L * (long)num5;
									Global.SaveRoleParamsInt64ValueToDB(client, "10219", num4, true);
									if (flag2)
									{
										if (flag3)
										{
											tianTi5v5ZhanDuiData.LianSheng++;
											tianTi5v5ZhanDuiData.SuccessCount++;
										}
										TianTiDuanWei tianTiDuanWei;
										if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(enemyBattleSide.DuanWeiId, out tianTiDuanWei))
										{
											num = tianTiDuanWei.WinJiFen;
											num2 = (int)((double)tianTiDuanWei.WinJiFen * Math.Min(2.0, (double)(tianTi5v5ZhanDuiData.LianSheng - 1) * 0.2));
											if (num6 <= tianTiDuanWei.RongYaoNum)
											{
												num3 = tianTiDuanWei.WinRongYu;
											}
										}
									}
									else
									{
										if (flag3)
										{
											tianTi5v5ZhanDuiData.LianSheng = 0;
										}
										TianTiDuanWei tianTiDuanWei;
										if (this.RuntimeData.TianTi5v5DuanWeiDict.TryGetValue(tianTi5v5ZhanDuiData.DuanWeiId, out tianTiDuanWei))
										{
											num = tianTiDuanWei.LoseJiFen;
											if (num6 <= tianTiDuanWei.RongYaoNum)
											{
												num3 = tianTiDuanWei.LoseRongYu;
											}
										}
									}
									if (num != 0 && flag3)
									{
										tianTi5v5ZhanDuiData.DuanWeiJiFen += num + num2;
										tianTi5v5ZhanDuiData.DuanWeiJiFen = Math.Max(0, tianTi5v5ZhanDuiData.DuanWeiJiFen);
									}
									if (flag3)
									{
										tianTi5v5ZhanDuiData.FightCount++;
										if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(tianTi5v5ZhanDuiData.DuanWeiJiFen, out duanWeiId))
										{
											tianTi5v5ZhanDuiData.DuanWeiId = duanWeiId;
										}
										TianTi5v5LogItemData cmd = new TianTi5v5LogItemData
										{
											Success = (flag2 ? 1 : 0),
											ZoneId1 = tianTi5v5ZhanDuiData.ZoneID,
											RoleName1 = tianTi5v5ZhanDuiData.ZhanDuiName,
											ZoneId2 = enemyBattleSide.ZoneId,
											RoleName2 = enemyBattleSide.RoleName,
											DuanWeiJiFenAward = num + num2,
											RongYaoAward = num3,
											RoleId = tianTi5v5ZhanDuiData.ZhanDuiID,
											EndTime = endTime
										};
										Global.sendToDB<int, TianTi5v5LogItemData>(3670, cmd, client.ServerId);
									}
									TianTi5v5AwardsData tianTi5v5AwardsData = new TianTi5v5AwardsData();
									tianTi5v5AwardsData.DuanWeiJiFen = num;
									tianTi5v5AwardsData.LianShengJiFen = num2;
									tianTi5v5AwardsData.RongYao = num3;
									tianTi5v5AwardsData.DuanWeiId = tianTi5v5ZhanDuiData.DuanWeiId;
									if (flag2)
									{
										tianTi5v5AwardsData.Success = 1;
										GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 34);
									}
									else
									{
										GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 0, 1, 34);
									}
									if (flag)
									{
										if (num3 != 0)
										{
											GameManager.ClientMgr.ModifyTeamRongYaoValue(gameClient, num3, "组队竞技获得荣耀", true);
										}
										gameClient.sendCmd<TianTi5v5AwardsData>(3710, tianTi5v5AwardsData, false);
									}
									TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = tianTi5v5ZhanDuiData.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == client.ClientData.RoleID);
									if (null != tianTi5v5ZhanDuiRoleData)
									{
										int offsetMonthDayID = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime());
										int offsetMonthDayID2 = TimeUtil.GetOffsetMonthDayID(TimeUtil.NowDateTime().AddMonths(-1));
										int[] array = tianTi5v5ZhanDuiRoleData.MonthFightCounts;
										if (array == null || array.Length != 4)
										{
											array = new int[4];
											tianTi5v5ZhanDuiRoleData.MonthFightCounts = array;
										}
										if (array[0] == offsetMonthDayID2)
										{
											array[2] = array[0];
											array[3] = array[1];
											array[0] = offsetMonthDayID;
											array[1] = 1;
										}
										else
										{
											array[0] = offsetMonthDayID;
											array[1]++;
										}
										tianTi5v5ZhanDuiRoleData.MonthFigntCount++;
										tianTi5v5ZhanDuiRoleData.ZhanLi = (long)client.ClientData.CombatForce;
										tianTi5v5ZhanDuiRoleData.RoleName = client.ClientData.RoleName;
										tianTi5v5ZhanDuiRoleData.RoleOcc = client.ClientData.Occupation;
										tianTi5v5ZhanDuiRoleData.ZhuanSheng = client.ClientData.ChangeLifeCount;
										tianTi5v5ZhanDuiRoleData.Level = client.ClientData.Level;
										tianTi5v5ZhanDuiRoleData.RebornLevel = client.ClientData.RebornLevel;
										RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
										if (roleData4Selector != null || roleData4Selector.RoleID < 0)
										{
											tianTi5v5ZhanDuiRoleData.ModelData = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
										}
									}
								}
							}
						}
						IL_788:
						i++;
						continue;
						goto IL_788;
					}
				}
				foreach (Tuple<TianTi5v5ZhanDuiData, int> tuple in scene.ZhanDuiDataDict)
				{
					Tuple<TianTi5v5ZhanDuiData, int> tuple;
					TianTi5v5ZhanDuiData item = tuple.Item1;
					item.ZhanDouLi = item.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
					item.LastFightTime = TimeUtil.NowDateTime();
					this.UpdateZhanDuiData2DB(item, tuple.Item2, 0);
					TianTiClient.getInstance().UpdateZhanDuiData(item, 0);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		public void GameCanceled(TianTi5v5Scene scene)
		{
			try
			{
				List<GameClient> clientsList = scene.CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
						{
							gameClient.sendCmd<TianTi5v5AwardsData>(3710, new TianTi5v5AwardsData
							{
								Success = -1
							}, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		public const SceneUIClasses ManagerType = 55;

		private static TianTi5v5Manager instance = new TianTi5v5Manager();

		public TianTi5v5Data RuntimeData = new TianTi5v5Data();

		public ConcurrentDictionary<int, TianTi5v5Scene> TianTi5v5SceneDict = new ConcurrentDictionary<int, TianTi5v5Scene>();

		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		private static long NextHeartBeatTicks = 0L;

		[ProtoContract]
		private class TianTi5v5ZhanDuiDataList : List<TianTi5v5ZhanDuiData>, ICompressed
		{
			public TianTi5v5ZhanDuiDataList()
			{
			}

			public TianTi5v5ZhanDuiDataList(List<TianTi5v5ZhanDuiData> list) : base(list)
			{
			}
		}
	}
}
