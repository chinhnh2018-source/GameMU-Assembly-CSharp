using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class TianTiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static TianTiManager getInstance()
		{
			return TianTiManager.instance;
		}

		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTiManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
			return this.InitConfig(false);
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(950, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(951, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(952, 2, 2, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(954, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(955, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(956, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(969, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 26, TianTiManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, TianTiManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 26, TianTiManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, TianTiManager.getInstance());
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
			case 950:
				return this.ProcessTianTiJoinCmd(client, nID, bytes, cmdParams);
			case 951:
				return this.ProcessTianTiQuitCmd(client, nID, bytes, cmdParams);
			case 952:
				return this.ProcessTianTiEnterCmd(client, nID, bytes, cmdParams);
			case 953:
				break;
			case 954:
				return this.ProcessGetTianTiDataAndDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 955:
				return this.ProcessGetTianTiMonthPaiHangDataCmd(client, nID, bytes, cmdParams);
			case 956:
				return this.ProcessTianTiGetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
			default:
				if (nID == 969)
				{
					return this.ProcessTianTiGeLogCmd(client, nID, bytes, cmdParams);
				}
				break;
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
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != kuaFuNotifyEnterGameEvent)
				{
					KuaFuServerLoginData kuaFuServerLoginData = kuaFuNotifyEnterGameEvent.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
						if (null != gameClient)
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(gameClient);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
								gameClient.sendCmd<long>(952, kuaFuServerLoginData.GameId, false);
							}
						}
					}
					eventObject.Handled = true;
				}
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
					if (reload)
					{
						this.RuntimeData.TianTiCD = (int)GameManager.systemParamsList.GetParamValueIntByName("TianTiCD", -1);
						return result;
					}
					this.RuntimeData.TianTiDuanWeiDict.Clear();
					this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.Clear();
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					text = "Config/DuanWei.xml";
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
							this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict[new RangeKey(num, tianTiDuanWei.NeedDuanWeiJiFen - 1, null)] = num2;
						}
						num = tianTiDuanWei.NeedDuanWeiJiFen;
						num2 = tianTiDuanWei.ID;
						num3 = tianTiDuanWei.ID;
						this.RuntimeData.TianTiDuanWeiDict[tianTiDuanWei.ID] = tianTiDuanWei;
					}
					if (num3 > 0 && num > 0)
					{
						this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict[new RangeKey(num, int.MaxValue, null)] = num3;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/TianTiBirthPoint.xml";
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
					text = "Config/DuanWeiRankAward.xml";
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
					text = "Config/TianTi.xml";
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
						this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-'))
						{
							result = false;
							LogManager.WriteLog(1000, "读取跨服天梯系统时间配置(TimePoints)出错", null, true);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("缺少跨服天梯系统地图 {0}", num4), null, true);
						}
					}
					this.RuntimeData.DuanWeiJiFenNum = (int)GameManager.systemParamsList.GetParamValueIntByName("DuanWeiJiFenNum", -1);
					this.RuntimeData.WinDuanWeiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("WinDuanWeiJiFen", -1);
					this.RuntimeData.LoseDuanWeiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("LoseDuanWeiJiFen", -1);
					this.RuntimeData.MaxTianTiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTianTiJiFen", -1);
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

		public void GMSetRoleData(GameClient client, int duanWeiId, int duanWeiJiFen, int rongYao, int monthDuanWeiRank, int lianSheng, int successCount, int fightCount)
		{
			RoleTianTiData tianTiData = client.ClientData.TianTiData;
			tianTiData.DuanWeiId = duanWeiId;
			tianTiData.DuanWeiJiFen = duanWeiJiFen;
			tianTiData.RongYao = rongYao;
			tianTiData.MonthDuanWeiRank = monthDuanWeiRank;
			tianTiData.LianSheng = lianSheng;
			tianTiData.SuccessCount = successCount;
			tianTiData.FightCount = fightCount;
			int duanWeiId2;
			if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(tianTiData.DuanWeiJiFen, out duanWeiId2))
			{
				tianTiData.DuanWeiId = duanWeiId2;
			}
			Global.sendToDB<int, RoleTianTiData>(10201, tianTiData, client.ServerId);
			TianTiPaiHangRoleData tianTiPaiHangRoleData = new TianTiPaiHangRoleData();
			tianTiPaiHangRoleData.DuanWeiId = tianTiData.DuanWeiId;
			tianTiPaiHangRoleData.RoleId = tianTiData.RoleId;
			tianTiPaiHangRoleData.RoleName = client.ClientData.RoleName;
			tianTiPaiHangRoleData.Occupation = client.ClientData.Occupation;
			tianTiPaiHangRoleData.ZhanLi = client.ClientData.CombatForce;
			tianTiPaiHangRoleData.ZoneId = client.ClientData.ZoneID;
			tianTiPaiHangRoleData.DuanWeiJiFen = tianTiData.DuanWeiJiFen;
			RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleData4Selector != null || roleData4Selector.RoleID < 0)
			{
				tianTiPaiHangRoleData.RoleData4Selector = roleData4Selector;
			}
			PlayerJingJiData playerJingJiData = JingJiChangManager.getInstance().createJingJiData(client);
			TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
			tianTiRoleInfoData.RoleId = tianTiPaiHangRoleData.RoleId;
			tianTiRoleInfoData.ZoneId = tianTiPaiHangRoleData.ZoneId;
			tianTiRoleInfoData.ZhanLi = tianTiPaiHangRoleData.ZhanLi;
			tianTiRoleInfoData.RoleName = tianTiPaiHangRoleData.RoleName;
			tianTiRoleInfoData.DuanWeiId = tianTiPaiHangRoleData.DuanWeiId;
			tianTiRoleInfoData.DuanWeiJiFen = tianTiPaiHangRoleData.DuanWeiJiFen;
			tianTiRoleInfoData.DuanWeiRank = tianTiPaiHangRoleData.DuanWeiRank;
			tianTiRoleInfoData.TianTiPaiHangRoleData = DataHelper.ObjectToBytes<TianTiPaiHangRoleData>(tianTiPaiHangRoleData);
			tianTiRoleInfoData.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(playerJingJiData);
			TianTiClient.getInstance().UpdateRoleInfoData(tianTiRoleInfoData);
			GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, rongYao, "GM添加", true);
		}

		public void TimerProc(object sender, EventArgs e)
		{
			bool flag = false;
			TianTiRankData rankingData = TianTiClient.getInstance().GetRankingData();
			lock (this.RuntimeData.Mutex)
			{
				if (rankingData != null && rankingData.ModifyTime > this.RuntimeData.ModifyTime)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Dictionary<int, TianTiPaiHangRoleData> dictionary = new Dictionary<int, TianTiPaiHangRoleData>();
				List<TianTiPaiHangRoleData> list = new List<TianTiPaiHangRoleData>();
				Dictionary<int, TianTiPaiHangRoleData> dictionary2 = new Dictionary<int, TianTiPaiHangRoleData>();
				List<TianTiPaiHangRoleData> list2 = new List<TianTiPaiHangRoleData>();
				if (null != rankingData.TianTiRoleInfoDataList)
				{
					foreach (TianTiRoleInfoData tianTiRoleInfoData in rankingData.TianTiRoleInfoDataList)
					{
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (null != tianTiRoleInfoData.TianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData = DataHelper.BytesToObject<TianTiPaiHangRoleData>(tianTiRoleInfoData.TianTiPaiHangRoleData, 0, tianTiRoleInfoData.TianTiPaiHangRoleData.Length);
						}
						else
						{
							tianTiPaiHangRoleData = new TianTiPaiHangRoleData
							{
								RoleId = tianTiRoleInfoData.RoleId
							};
						}
						if (null != tianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData.RoleId = tianTiRoleInfoData.RoleId;
							tianTiPaiHangRoleData.DuanWeiRank = tianTiRoleInfoData.DuanWeiRank;
							dictionary[tianTiPaiHangRoleData.RoleId] = tianTiPaiHangRoleData;
							if (list.Count < this.RuntimeData.MaxDayPaiMingListCount)
							{
								list.Add(tianTiPaiHangRoleData);
							}
						}
					}
				}
				if (null != rankingData.TianTiMonthRoleInfoDataList)
				{
					foreach (TianTiRoleInfoData tianTiRoleInfoData in rankingData.TianTiMonthRoleInfoDataList)
					{
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (null != tianTiRoleInfoData.TianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData = DataHelper.BytesToObject<TianTiPaiHangRoleData>(tianTiRoleInfoData.TianTiPaiHangRoleData, 0, tianTiRoleInfoData.TianTiPaiHangRoleData.Length);
						}
						else
						{
							tianTiPaiHangRoleData = new TianTiPaiHangRoleData
							{
								RoleId = tianTiRoleInfoData.RoleId
							};
						}
						if (null != tianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData.RoleId = tianTiRoleInfoData.RoleId;
							tianTiPaiHangRoleData.DuanWeiRank = tianTiRoleInfoData.DuanWeiRank;
							dictionary2[tianTiPaiHangRoleData.RoleId] = tianTiPaiHangRoleData;
							if (list2.Count < this.RuntimeData.MaxMonthPaiMingListCount)
							{
								list2.Add(tianTiPaiHangRoleData);
							}
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.ModifyTime = rankingData.ModifyTime;
					this.RuntimeData.MaxPaiMingRank = rankingData.MaxPaiMingRank;
					this.RuntimeData.TianTiPaiHangRoleDataDict = dictionary;
					this.RuntimeData.TianTiPaiHangRoleDataList = list;
					this.RuntimeData.TianTiMonthPaiHangRoleDataDict = dictionary2;
					this.RuntimeData.TianTiMonthPaiHangRoleDataList = list2;
				}
			}
		}

		public bool ProcessTianTiJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (mapSceneType != 0)
				{
					client.sendCmd<int>(nID, -21, false);
					return true;
				}
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd<int>(nID, -2001, false);
					return true;
				}
				long num;
				if (this.RuntimeData.TianTiCDDict.TryGetValue(client.ClientData.RoleID, out num) && num > 0L)
				{
					if (num + (long)(this.RuntimeData.TianTiCD * 1000) > TimeUtil.NOW())
					{
						GameManager.ClientMgr.NotifySkillCDTime(client, -2, num + (long)(this.RuntimeData.TianTiCD * 1000), false);
						client.sendCmd<int>(nID, -2004, false);
						return true;
					}
					this.RuntimeData.TianTiCDDict[client.ClientData.RoleID] = 0L;
				}
				int num2 = -2001;
				int duanWeiId = client.ClientData.TianTiData.DuanWeiId;
				TimeSpan timeOfDay = TimeUtil.NowDateTime().TimeOfDay;
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < this.RuntimeData.TimePoints.Count - 1; i += 2)
					{
						if (timeOfDay >= this.RuntimeData.TimePoints[i] && timeOfDay < this.RuntimeData.TimePoints[i + 1])
						{
							num2 = 1;
							break;
						}
					}
				}
				if (num2 >= 0)
				{
					num2 = TianTiClient.getInstance().TianTiSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 2, duanWeiId, client.ClientData.CombatForce);
					if (num2 > 0)
					{
						client.ClientData.SignUpGameType = 2;
						GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 2);
					}
				}
				client.sendCmd<int>(nID, num2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetTianTiDataAndDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				TianTiDataAndDayPaiHang tianTiDataAndDayPaiHang = new TianTiDataAndDayPaiHang();
				if (this.IsGongNengOpened(client, false))
				{
					this.InitRoleTianTiData(client);
					tianTiDataAndDayPaiHang.TianTiData = client.ClientData.TianTiData;
					lock (this.RuntimeData.Mutex)
					{
						int count = this.RuntimeData.TianTiPaiHangRoleDataList.Count;
						if (count > 0)
						{
							tianTiDataAndDayPaiHang.PaiHangRoleDataList = this.RuntimeData.TianTiPaiHangRoleDataList.GetRange(0, count);
						}
					}
				}
				client.sendCmd<TianTiDataAndDayPaiHang>(nID, tianTiDataAndDayPaiHang, false);
				long num;
				if (this.RuntimeData.TianTiCDDict.TryGetValue(client.ClientData.RoleID, out num))
				{
					if (num + (long)(this.RuntimeData.TianTiCD * 1000) > TimeUtil.NOW())
					{
						GameManager.ClientMgr.NotifySkillCDTime(client, -2, num + (long)(this.RuntimeData.TianTiCD * 1000), false);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetTianTiMonthPaiHangDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				TianTiMonthPaiHangData tianTiMonthPaiHangData = new TianTiMonthPaiHangData();
				if (this.IsGongNengOpened(client, false))
				{
					tianTiMonthPaiHangData.SelfPaiHangRoleData = new TianTiPaiHangRoleData
					{
						RoleId = client.ClientData.RoleID,
						RoleName = client.ClientData.RoleName,
						DuanWeiId = client.ClientData.TianTiData.DuanWeiId,
						DuanWeiJiFen = client.ClientData.TianTiData.DuanWeiJiFen,
						DuanWeiRank = client.ClientData.TianTiData.DuanWeiRank
					};
					lock (this.RuntimeData.Mutex)
					{
						if (null != this.RuntimeData.TianTiMonthPaiHangRoleDataList)
						{
							tianTiMonthPaiHangData.PaiHangRoleDataList = new List<TianTiPaiHangRoleData>(this.RuntimeData.TianTiMonthPaiHangRoleDataList);
						}
					}
				}
				client.sendCmd<TianTiMonthPaiHangData>(nID, tianTiMonthPaiHangData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTiGetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
						client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime = TimeUtil.NowDateTime();
						Global.sendToDB<int, RoleTianTiData>(10201, client.ClientData.TianTiData, client.ServerId);
						for (int i = 0; i < list.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
						}
					}
				}
				else if (duanWeiRankAward != null)
				{
					if (client.CodeRevision <= 2)
					{
						cmdData = 1;
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(537, new object[0]));
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

		public bool ProcessTianTiGeLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<TianTiLogItemData> cmdData = new List<TianTiLogItemData>();
				cmdData = Global.sendToDB<List<TianTiLogItemData>, int>(969, client.ClientData.RoleID, client.ServerId);
				client.sendCmd<List<TianTiLogItemData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTiEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				if (num > 0)
				{
					int num2 = TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 4, false);
					if (num2 >= 0)
					{
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
					else
					{
						num = 0;
					}
				}
				else
				{
					TianTiClient.getInstance().RoleChangeState(GameManager.ServerId, client.ClientData.RoleID, 0);
					long num3 = TimeUtil.NOW();
					this.RuntimeData.TianTiCDDict[client.ClientData.RoleID] = num3;
					GameManager.ClientMgr.NotifySkillCDTime(client, -2, num3 + (long)(this.RuntimeData.TianTiCD * 1000), false);
				}
				client.ClientData.SignUpGameType = 0;
				if (num <= 0)
				{
					Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					client.sendCmd<int>(951, 0, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessTianTiQuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int num = 1;
				if (num >= 0)
				{
					num = TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 0, false);
					client.ClientData.SignUpGameType = 0;
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitRoleTianTiData(GameClient client)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				DateTime dateTime = TimeUtil.NowDateTime();
				DateTime t = dateTime.AddMonths(-1);
				t = new DateTime(t.Year, t.Month, 1);
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.ModifyTime > t)
					{
						int num = this.RuntimeData.MaxPaiMingRank + 1;
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (this.RuntimeData.TianTiPaiHangRoleDataDict.TryGetValue(client.ClientData.RoleID, out tianTiPaiHangRoleData))
						{
							num = tianTiPaiHangRoleData.DuanWeiRank;
						}
						if (client.ClientData.TianTiData.DuanWeiRank != num)
						{
							flag = true;
							client.ClientData.TianTiData.DuanWeiRank = num;
						}
						num = this.RuntimeData.MaxPaiMingRank + 1;
						if (this.RuntimeData.TianTiMonthPaiHangRoleDataDict.TryGetValue(client.ClientData.RoleID, out tianTiPaiHangRoleData))
						{
							num = tianTiPaiHangRoleData.DuanWeiRank;
						}
						if (client.ClientData.TianTiData.MonthDuanWeiRank != num)
						{
							flag = true;
							client.ClientData.TianTiData.MonthDuanWeiRank = num;
						}
					}
					DateTime realDate = Global.GetRealDate(client.ClientData.TianTiData.LastFightDayId);
					if (this.RuntimeData.ModifyTime > realDate && realDate.Month != this.RuntimeData.ModifyTime.Month)
					{
						client.ClientData.TianTiData.LianSheng = 0;
						client.ClientData.TianTiData.SuccessCount = 0;
						client.ClientData.TianTiData.FightCount = 0;
						client.ClientData.TianTiData.DuanWeiJiFen = 0;
					}
					int duanWeiId;
					if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(client.ClientData.TianTiData.DuanWeiJiFen, out duanWeiId))
					{
						client.ClientData.TianTiData.DuanWeiId = duanWeiId;
					}
					if (!client.ClientSocket.IsKuaFuLogin && realDate.Date != dateTime.Subtract(this.RuntimeData.RefreshTime).Date)
					{
						client.ClientData.TianTiData.TodayFightCount = 0;
					}
					if (realDate < t)
					{
						client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime = t.AddMonths(1);
					}
					client.ClientData.TianTiData.RankUpdateTime = this.RuntimeData.ModifyTime;
				}
				if (client.ClientData.TianTiData.TodayFightCount == 0)
				{
					client.ClientData.TianTiData.DayDuanWeiJiFen = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "TianTiDayScore", 0, true);
				}
				else
				{
					client.ClientData.TianTiData.DayDuanWeiJiFen = Global.GetRoleParamsInt32FromDB(client, "TianTiDayScore");
				}
				result = flag;
			}
			return result;
		}

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int num = client.ClientData.BattleWhichSide;
			if (num <= 0)
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				num = TianTiClient.getInstance().GetRoleBattleWhichSide((int)clientKuaFuServerLoginData.GameId, clientKuaFuServerLoginData.RoleId);
				if (num > 0)
				{
					client.ClientData.BattleWhichSide = num;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				TianTiBirthPoint tianTiBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(num, out tianTiBirthPoint))
				{
					posX = tianTiBirthPoint.PosX;
					posY = tianTiBirthPoint.PosY;
					return num;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public bool OnInitGame(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client, out posX, out posY);
			bool result;
			if (birthPoint <= 0)
			{
				result = false;
			}
			else
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				int index = (int)clientKuaFuServerLoginData.GameId % this.RuntimeData.MapCodeList.Count;
				client.ClientData.MapCode = this.RuntimeData.MapCodeList[index];
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.BattleWhichSide = birthPoint;
				int num = 0;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.GameId2FuBenSeq.TryGetValue((int)clientKuaFuServerLoginData.GameId, out num))
					{
						num = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.GameId2FuBenSeq[(int)clientKuaFuServerLoginData.GameId] = num;
					}
				}
				clientKuaFuServerLoginData.FuBenSeqId = num;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("TianTi") && !GameFuncControlManager.IsGameFuncDisabled(5) && GlobalNew.IsGongNengOpened(client, 62, hint);
		}

		public bool CanGetMonthRankAwards(GameClient client, out DuanWeiRankAward duanWeiRankAward)
		{
			duanWeiRankAward = null;
			lock (this.RuntimeData.Mutex)
			{
				if (client.ClientData.TianTiData.MonthDuanWeiRank > 0)
				{
					if (this.RuntimeData.DuanWeiRankAwardDict.TryGetValue(client.ClientData.TianTiData.MonthDuanWeiRank, out duanWeiRankAward))
					{
						DateTime fetchMonthDuanWeiRankAwardsTime = client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime;
						DateTime dateTime = TimeUtil.NowDateTime();
						if (fetchMonthDuanWeiRankAwardsTime.Month != dateTime.Month || fetchMonthDuanWeiRankAwardsTime.Year != dateTime.Year)
						{
							if (new DateTime(fetchMonthDuanWeiRankAwardsTime.Year, fetchMonthDuanWeiRankAwardsTime.Month, 1) < new DateTime(this.RuntimeData.ModifyTime.Year, this.RuntimeData.ModifyTime.Month, 1))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public bool AddTianTiCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 26)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					TianTiScene tianTiScene = null;
					if (!this.TianTiSceneDict.TryGetValue(fuBenSeqID, out tianTiScene))
					{
						tianTiScene = new TianTiScene();
						tianTiScene.CopyMap = copyMap;
						tianTiScene.CleanAllInfo();
						tianTiScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						tianTiScene.m_nMapCode = mapCode;
						tianTiScene.CopyMapId = copyMap.CopyMapID;
						tianTiScene.FuBenSeqId = fuBenSeqID;
						tianTiScene.m_nPlarerCount = 1;
						this.TianTiSceneDict[fuBenSeqID] = tianTiScene;
					}
					else
					{
						tianTiScene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					this.SaveClientBattleSide(tianTiScene, client);
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					if (tianTiScene.SuccessSide == -1)
					{
						client.sendCmd<TianTiAwardsData>(953, new TianTiAwardsData
						{
							Success = -1
						}, false);
					}
				}
				TianTiClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 5, 0, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 2);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveTianTiCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 26)
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTiScene tianTiScene;
					if (this.TianTiSceneDict.TryRemove(copyMap.FuBenSeqID, out tianTiScene))
					{
						this.RuntimeData.GameId2FuBenSeq.Remove(tianTiScene.GameId);
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

		private void SaveClientBattleSide(TianTiScene tianTiScene, GameClient client)
		{
			TianTiRoleMiniData tianTiRoleMiniData;
			if (!tianTiScene.RoleIdDuanWeiIdDict.TryGetValue(client.ClientData.RoleID, out tianTiRoleMiniData))
			{
				tianTiRoleMiniData = new TianTiRoleMiniData();
				tianTiScene.RoleIdDuanWeiIdDict[client.ClientData.RoleID] = tianTiRoleMiniData;
			}
			tianTiRoleMiniData.RoleId = client.ClientData.RoleID;
			tianTiRoleMiniData.RoleName = client.ClientData.RoleName;
			tianTiRoleMiniData.BattleWitchSide = client.ClientData.BattleWhichSide;
			tianTiRoleMiniData.ZoneId = client.ClientData.ZoneID;
			tianTiRoleMiniData.DuanWeiId = client.ClientData.TianTiData.DuanWeiId;
		}

		private TianTiRoleMiniData GetEnemyBattleSide(TianTiScene tianTiScene, GameClient client)
		{
			foreach (KeyValuePair<int, TianTiRoleMiniData> keyValuePair in tianTiScene.RoleIdDuanWeiIdDict)
			{
				if (client.ClientData.RoleID != keyValuePair.Key)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= TianTiManager.NextHeartBeatTicks)
			{
				TianTiManager.NextHeartBeatTicks = num + 1020L;
				foreach (TianTiScene tianTiScene in this.TianTiSceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = tianTiScene.FuBenSeqId;
						int copyMapId = tianTiScene.CopyMapId;
						int nMapCode = tianTiScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = tianTiScene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (tianTiScene.m_eStatus == 0)
							{
								tianTiScene.m_lPrepareTime = num2;
								tianTiScene.m_lBeginTime = num2 + (long)(this.RuntimeData.WaitingEnterSecs * 1000);
								tianTiScene.m_eStatus = 1;
								tianTiScene.StateTimeData.GameType = 2;
								tianTiScene.StateTimeData.State = tianTiScene.m_eStatus;
								tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
							}
							else if (tianTiScene.m_eStatus == 1)
							{
								bool flag2;
								if (copyMap.GetGameClientCount() >= 2)
								{
									flag2 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
								}
								else
								{
									flag2 = true;
								}
								if (!flag2)
								{
									tianTiScene.m_eStatus = 2;
									tianTiScene.m_lEndTime = num2 + (long)(this.RuntimeData.FightingSecs * 1000);
									tianTiScene.StateTimeData.GameType = 2;
									tianTiScene.StateTimeData.State = tianTiScene.m_eStatus;
									tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
								else if (num2 >= tianTiScene.m_lBeginTime || this.CancledGameIdDict.Contains(tianTiScene.GameId))
								{
									this.CompleteTianTiScene(tianTiScene, -1);
								}
							}
							else if (tianTiScene.m_eStatus == 2)
							{
								if (num2 >= tianTiScene.m_lEndTime)
								{
									this.CompleteTianTiScene(tianTiScene, 0);
								}
							}
							else if (tianTiScene.m_eStatus == 3)
							{
								this.ProcessEnd(tianTiScene, now, num);
							}
							else if (tianTiScene.m_eStatus == 4)
							{
								if (num2 >= tianTiScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(tianTiScene.m_lLeaveTime);
									tianTiScene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int i = 0; i < clientsList.Count; i++)
											{
												GameClient gameClient = clientsList[i];
												if (gameClient != null)
												{
													KuaFuManager.getInstance().GotoLastMap(gameClient);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "跨服天梯系统清场调度异常");
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
				TianTiScene tianTiScene;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, false);
				}
			}
		}

		public void CompleteTianTiScene(TianTiScene tianTiScene, int successSide)
		{
			tianTiScene.m_eStatus = 3;
			tianTiScene.SuccessSide = successSide;
		}

		public void CancleTianTiScene(int gameId)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.CancledGameIdDict.Add(gameId);
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTiScene tianTiScene;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					if (tianTiScene.m_eStatus < 3)
					{
						this.CompleteTianTiScene(tianTiScene, client.ClientData.BattleWhichSide);
					}
					int posX;
					int posY;
					int birthPoint = this.GetBirthPoint(other, out posX, out posY);
					if (birthPoint > 0)
					{
						other.ClientData.PosX = posX;
						other.ClientData.PosY = posY;
						GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, other.ClientData.RoleID, posX, posY, -1);
						Global.ClientRealive(other, posX, posY, -1);
					}
				}
			}
		}

		private void ProcessEnd(TianTiScene tianTiScene, DateTime now, long nowTicks)
		{
			tianTiScene.m_eStatus = 4;
			tianTiScene.m_lEndTime = nowTicks;
			tianTiScene.m_lLeaveTime = tianTiScene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
			TianTiClient.getInstance().GameFuBenChangeState(tianTiScene.GameId, 3, now);
			tianTiScene.StateTimeData.GameType = 2;
			tianTiScene.StateTimeData.State = 3;
			tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
			if (tianTiScene.SuccessSide == -1)
			{
				this.GameCanceled(tianTiScene);
			}
			else
			{
				this.GiveAwards(tianTiScene);
			}
		}

		public void GiveAwards(TianTiScene tianTiScene)
		{
			try
			{
				DateTime endTime = TimeUtil.NowDateTime();
				DateTime now = endTime.Subtract(this.RuntimeData.RefreshTime);
				List<GameClient> clientsList = tianTiScene.CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					int offsetDayNow = Global.GetOffsetDayNow();
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
						{
							RoleTianTiData tianTiData = gameClient.ClientData.TianTiData;
							bool flag = gameClient.ClientData.BattleWhichSide == tianTiScene.SuccessSide;
							int duanWeiId = tianTiData.DuanWeiId;
							TianTiRoleMiniData enemyBattleSide = this.GetEnemyBattleSide(tianTiScene, gameClient);
							int num = 0;
							int num2 = 0;
							int num3 = 0;
							int offsetDay = Global.GetOffsetDay(now);
							if (offsetDay != tianTiData.LastFightDayId)
							{
								tianTiData.LastFightDayId = offsetDay;
								tianTiData.TodayFightCount = 1;
							}
							else
							{
								tianTiData.TodayFightCount++;
							}
							if (tianTiData.DayDuanWeiJiFen < this.RuntimeData.MaxTianTiJiFen)
							{
								if (flag)
								{
									tianTiData.LianSheng++;
									tianTiData.SuccessCount++;
									TianTiDuanWei tianTiDuanWei;
									if (this.RuntimeData.TianTiDuanWeiDict.TryGetValue(enemyBattleSide.DuanWeiId, out tianTiDuanWei))
									{
										num = tianTiDuanWei.WinJiFen;
										num2 = (int)((double)tianTiDuanWei.WinJiFen * Math.Min(2.0, (double)(tianTiData.LianSheng - 1) * 0.2));
										if (tianTiData.TodayFightCount <= tianTiDuanWei.RongYaoNum)
										{
											num3 = tianTiDuanWei.WinRongYu;
										}
									}
								}
								else
								{
									tianTiData.LianSheng = 0;
									TianTiDuanWei tianTiDuanWei;
									if (this.RuntimeData.TianTiDuanWeiDict.TryGetValue(tianTiData.DuanWeiId, out tianTiDuanWei))
									{
										num = tianTiDuanWei.LoseJiFen;
										if (tianTiData.TodayFightCount <= tianTiDuanWei.RongYaoNum)
										{
											num3 = tianTiDuanWei.LoseRongYu;
										}
									}
								}
								if (num != 0)
								{
									tianTiData.DuanWeiJiFen += num + num2;
									tianTiData.DuanWeiJiFen = Math.Max(0, tianTiData.DuanWeiJiFen);
									tianTiData.DayDuanWeiJiFen += num + num2;
									tianTiData.DayDuanWeiJiFen = Math.Max(0, tianTiData.DayDuanWeiJiFen);
									Global.SaveRoleParamsInt32ValueToDB(gameClient, "TianTiDayScore", tianTiData.DayDuanWeiJiFen, true);
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyHintMsg(gameClient, GLang.GetLang(538, new object[0]));
							}
							if (num3 != 0)
							{
								GameManager.ClientMgr.ModifyTianTiRongYaoValue(gameClient, num3, "天梯系统获得荣耀", true);
							}
							tianTiData.FightCount++;
							if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(tianTiData.DuanWeiJiFen, out duanWeiId))
							{
								tianTiData.DuanWeiId = duanWeiId;
							}
							TianTiAwardsData tianTiAwardsData = new TianTiAwardsData();
							tianTiAwardsData.DuanWeiJiFen = num;
							tianTiAwardsData.LianShengJiFen = num2;
							tianTiAwardsData.RongYao = num3;
							tianTiAwardsData.DuanWeiId = tianTiData.DuanWeiId;
							if (flag)
							{
								JunTuanManager.getInstance().AddJunTuanTaskValue(gameClient, 3, 1);
								tianTiAwardsData.Success = 1;
								GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 1, 0, 2);
							}
							else
							{
								GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 0, 1, 2);
							}
							gameClient.sendCmd<TianTiAwardsData>(953, tianTiAwardsData, false);
							Global.sendToDB<int, RoleTianTiData>(10201, tianTiData, gameClient.ServerId);
							TianTiLogItemData cmd = new TianTiLogItemData
							{
								Success = tianTiAwardsData.Success,
								ZoneId1 = gameClient.ClientData.ZoneID,
								RoleName1 = gameClient.ClientData.RoleName,
								ZoneId2 = enemyBattleSide.ZoneId,
								RoleName2 = enemyBattleSide.RoleName,
								DuanWeiJiFenAward = num + num2,
								RongYaoAward = num3,
								RoleId = gameClient.ClientData.RoleID,
								EndTime = endTime
							};
							Global.sendToDB<int, TianTiLogItemData>(10200, cmd, gameClient.ServerId);
							TianTiPaiHangRoleData tianTiPaiHangRoleData = new TianTiPaiHangRoleData();
							tianTiPaiHangRoleData.DuanWeiId = tianTiData.DuanWeiId;
							tianTiPaiHangRoleData.RoleId = tianTiData.RoleId;
							tianTiPaiHangRoleData.RoleName = gameClient.ClientData.RoleName;
							tianTiPaiHangRoleData.Occupation = gameClient.ClientData.Occupation;
							tianTiPaiHangRoleData.ZhanLi = gameClient.ClientData.CombatForce;
							tianTiPaiHangRoleData.ZoneId = gameClient.ClientData.ZoneID;
							tianTiPaiHangRoleData.DuanWeiJiFen = tianTiData.DuanWeiJiFen;
							RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", gameClient.ClientData.RoleID), gameClient.ServerId);
							if (roleData4Selector != null || roleData4Selector.RoleID < 0)
							{
								tianTiPaiHangRoleData.RoleData4Selector = roleData4Selector;
							}
							PlayerJingJiData playerJingJiData = JingJiChangManager.getInstance().createJingJiData(gameClient);
							TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
							tianTiRoleInfoData.RoleId = tianTiPaiHangRoleData.RoleId;
							tianTiRoleInfoData.ZoneId = tianTiPaiHangRoleData.ZoneId;
							tianTiRoleInfoData.ZhanLi = tianTiPaiHangRoleData.ZhanLi;
							tianTiRoleInfoData.RoleName = tianTiPaiHangRoleData.RoleName;
							tianTiRoleInfoData.DuanWeiId = tianTiPaiHangRoleData.DuanWeiId;
							tianTiRoleInfoData.DuanWeiJiFen = tianTiPaiHangRoleData.DuanWeiJiFen;
							tianTiRoleInfoData.DuanWeiRank = tianTiPaiHangRoleData.DuanWeiRank;
							tianTiRoleInfoData.TianTiPaiHangRoleData = DataHelper.ObjectToBytes<TianTiPaiHangRoleData>(tianTiPaiHangRoleData);
							tianTiRoleInfoData.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(playerJingJiData);
							TianTiClient.getInstance().UpdateRoleInfoData(tianTiRoleInfoData);
						}
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_TianTiPT, new int[0]));
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_TianTiDiamond, new int[0]));
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		public void GameCanceled(TianTiScene tianTiScene)
		{
			try
			{
				List<GameClient> clientsList = tianTiScene.CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
						{
							gameClient.sendCmd<TianTiAwardsData>(953, new TianTiAwardsData
							{
								Success = -1
							}, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTiScene tianTiScene = null;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					if (tianTiScene.m_eStatus < 3)
					{
						if (tianTiScene.CopyMap.GetGameClientCount() >= 2)
						{
							if (client.ClientData.BattleWhichSide == 1)
							{
								this.CompleteTianTiScene(tianTiScene, 2);
							}
							else
							{
								this.CompleteTianTiScene(tianTiScene, 1);
							}
						}
						else
						{
							this.CompleteTianTiScene(tianTiScene, -1);
						}
						this.ProcessEnd(tianTiScene, TimeUtil.NowDateTime(), TimeUtil.NOW());
					}
				}
			}
			TianTiClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 0, 0, 0);
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses ManagerType = 26;

		private static TianTiManager instance = new TianTiManager();

		public TianTiData RuntimeData = new TianTiData();

		public ConcurrentDictionary<int, TianTiScene> TianTiSceneDict = new ConcurrentDictionary<int, TianTiScene>();

		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		private static long NextHeartBeatTicks = 0L;
	}
}
