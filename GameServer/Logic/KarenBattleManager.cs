using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KarenBattleManager : IManager, IManager2, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		public static KarenBattleManager getInstance()
		{
			return KarenBattleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KarenBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1210, 2, 2, KarenBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1212, 1, 1, KarenBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, KarenBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, KarenBattleManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, KarenBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, KarenBattleManager.getInstance());
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
			case 1210:
				return this.ProcessKarenBattleEnterCmd(client, nID, bytes, cmdParams);
			case 1212:
				return this.ProcessGetKarenBattleStateCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 24:
			{
				PreBangHuiRemoveMemberEventObject preBangHuiRemoveMemberEventObject = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != preBangHuiRemoveMemberEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(preBangHuiRemoveMemberEventObject);
				}
				break;
			}
			case 25:
			{
				PreBangHuiChangeZhiWuEventObject preBangHuiChangeZhiWuEventObject = eventObject as PreBangHuiChangeZhiWuEventObject;
				if (null != preBangHuiChangeZhiWuEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiChangeZhiWu(preBangHuiChangeZhiWuEventObject);
				}
				break;
			}
			}
		}

		public bool LoadKarenPublicConfig()
		{
			bool result = true;
			string text = "";
			lock (this.Mutex)
			{
				try
				{
					this.SceneDataDict.Clear();
					text = "Config/LegionsWar.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KarenBattleSceneInfo karenBattleSceneInfo = new KarenBattleSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(xml, "ID");
						int num = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						karenBattleSceneInfo.Id = id;
						karenBattleSceneInfo.MapCode = num;
						karenBattleSceneInfo.MaxLegions = (int)Global.GetSafeAttributeLong(xml, "LegionsMax");
						karenBattleSceneInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
						karenBattleSceneInfo.EnterCD = (int)Global.GetSafeAttributeLong(xml, "EnterCD");
						karenBattleSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						karenBattleSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						karenBattleSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						karenBattleSceneInfo.Exp = Global.GetSafeAttributeLong(xml, "Exp");
						karenBattleSceneInfo.BandJinBi = (int)Global.GetSafeAttributeLong(xml, "BandJinBi");
						if (!ConfigParser.ParserTimeRangeListWithDay(karenBattleSceneInfo.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < karenBattleSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(karenBattleSceneInfo.TimePoints[i].Hours, karenBattleSceneInfo.TimePoints[i].Minutes, karenBattleSceneInfo.TimePoints[i].Seconds);
							karenBattleSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "WinGoods"), ref karenBattleSceneInfo.WinAwardsItemList, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "LoseGoods"), ref karenBattleSceneInfo.LoseAwardsItemList, '|', ',');
						this.SceneDataDict[num] = karenBattleSceneInfo;
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

		public bool InitConfig()
		{
			lock (this.Mutex)
			{
				this.KarenBattleEnterMap.Clear();
				List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("LegionsWarEnterMap", ',');
				if (null != paramValueStringListByName)
				{
					foreach (string str in paramValueStringListByName)
					{
						this.KarenBattleEnterMap.Add(Global.SafeConvertToInt32(str));
					}
				}
			}
			return this.LoadKarenPublicConfig();
		}

		public bool ProcessKarenBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				KarenBattleSceneInfo karenBattleSceneInfo = null;
				KarenGameStates karenGameStates = KarenGameStates.None;
				int num4 = 0;
				int num5 = 0;
				JunTuanRankData junTuanRankDataByClient = this.GetJunTuanRankDataByClient(client);
				if (junTuanRankDataByClient == null || !this.CheckCanEnterKarenBattle(client))
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						0,
						num5,
						num4
					}), false);
					return true;
				}
				if (!this.CheckMap(client))
				{
					num = -21;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						0,
						num5,
						num4
					}), false);
					return true;
				}
				num = this.CheckTimeCondition(ref karenGameStates);
				if (karenGameStates != KarenGameStates.Start)
				{
					num = -2001;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						0,
						num5,
						num4
					}), false);
					return true;
				}
				lock (this.Mutex)
				{
					if (!this.SceneDataDict.TryGetValue(num3, out karenBattleSceneInfo))
					{
						num = -5;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							0,
							num5,
							num4
						}), false);
						return true;
					}
					foreach (KeyValuePair<int, KarenBattleSceneInfo> keyValuePair in this.SceneDataDict)
					{
						KarenFuBenData karenKuaFuFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(keyValuePair.Key);
						if (null != karenKuaFuFuBenData)
						{
							SceneUIClasses mapSceneType = Global.GetMapSceneType(keyValuePair.Value.MapCode);
							if (mapSceneType == 41)
							{
								num5 = karenKuaFuFuBenData.GetRoleCountWithEnter(junTuanRankDataByClient.Rank);
							}
							else
							{
								num4 = karenKuaFuFuBenData.GetRoleCountWithEnter(junTuanRankDataByClient.Rank);
							}
						}
					}
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20019");
					if (!this.GMTest && TimeUtil.NowDateTime().Ticks - roleParamsDateTimeFromDB.Ticks < 10000000L * (long)karenBattleSceneInfo.EnterCD)
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, string.Format(GLang.GetLang(2615, new object[0]), karenBattleSceneInfo.EnterCD), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						num = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							0,
							num5,
							num4
						}), false);
						return true;
					}
					KuaFuServerInfo kuaFuServerInfo = null;
					KarenFuBenData karenKuaFuFuBenData2 = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(num3);
					if (karenKuaFuFuBenData2 == null || !KuaFuManager.getInstance().TryGetValue(karenKuaFuFuBenData2.ServerId, out kuaFuServerInfo))
					{
						num = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							0,
							num5,
							num4
						}), false);
						return true;
					}
					if (karenKuaFuFuBenData2.GetRoleCountWithEnter(junTuanRankDataByClient.Rank) >= karenBattleSceneInfo.MaxEnterNum)
					{
						num = -22;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							0,
							num5,
							num4
						}), false);
						return true;
					}
					SceneUIClasses mapSceneType2 = Global.GetMapSceneType(karenBattleSceneInfo.MapCode);
					KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
					if (null != clientKuaFuServerLoginData)
					{
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.GameId = (long)karenKuaFuFuBenData2.GameId;
						clientKuaFuServerLoginData.GameType = karenKuaFuFuBenData2.GameType;
						clientKuaFuServerLoginData.EndTicks = karenKuaFuFuBenData2.EndTime.Ticks;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
						clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
						clientKuaFuServerLoginData.FuBenSeqId = 0;
					}
					if (num >= 0)
					{
						num = JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, (int)clientKuaFuServerLoginData.GameId, junTuanRankDataByClient.Rank, 4);
						if (num >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					0,
					num5,
					num4
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetKarenBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						"",
						""
					}), false);
					return true;
				}
				KarenGameStates karenGameStates = KarenGameStates.None;
				this.CheckTimeCondition(ref karenGameStates);
				if (this.CheckCanEnterKarenBattle(client))
				{
					if (karenGameStates == KarenGameStates.Wait)
					{
						num = 1;
					}
					else if (karenGameStates == KarenGameStates.Start)
					{
						lock (this.Mutex)
						{
							foreach (KeyValuePair<int, KarenBattleSceneInfo> keyValuePair in this.SceneDataDict)
							{
								KarenFuBenData karenKuaFuFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(keyValuePair.Key);
								if (null != karenKuaFuFuBenData)
								{
									JunTuanRankData junTuanRankDataByClient = this.GetJunTuanRankDataByClient(client);
									if (null != junTuanRankDataByClient)
									{
										SceneUIClasses mapSceneType = Global.GetMapSceneType(keyValuePair.Value.MapCode);
										if (mapSceneType == 41)
										{
											num2 = karenKuaFuFuBenData.GetRoleCountWithEnter(junTuanRankDataByClient.Rank);
										}
										else
										{
											num3 = karenKuaFuFuBenData.GetRoleCountWithEnter(junTuanRankDataByClient.Rank);
										}
									}
								}
							}
						}
						num = 2;
					}
				}
				else if (karenGameStates == KarenGameStates.Wait || karenGameStates == KarenGameStates.Start)
				{
					JunTuanRankData junTuanRankDataByClient2 = this.GetJunTuanRankDataByClient(client);
					if (null != junTuanRankDataByClient2)
					{
						num = 3;
					}
					else
					{
						num = 4;
					}
				}
				string text = "";
				string text2 = "";
				List<LingDiData> lingDiData = JunTuanClient.getInstance().GetLingDiData();
				if (null != lingDiData)
				{
					foreach (LingDiData lingDiData2 in lingDiData)
					{
						SceneUIClasses sceneUIClasses = this.ConvertCaiJiLingDiTypeToMapSceneType(lingDiData2.LingDiType);
						if (sceneUIClasses == 41)
						{
							text2 = lingDiData2.JunTuanName;
						}
						else if (sceneUIClasses == 42)
						{
							text = lingDiData2.JunTuanName;
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					num3,
					text2,
					text
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private SceneUIClasses ConvertCaiJiLingDiTypeToMapSceneType(int lingdiType)
		{
			SceneUIClasses result;
			if (lingdiType == 0)
			{
				result = 41;
			}
			else if (lingdiType == 1)
			{
				result = 42;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private int ConvertMapSceneTypeToCaiJiLingDiType(SceneUIClasses mapsceneType)
		{
			int result;
			if (mapsceneType == 41)
			{
				result = 0;
			}
			else if (mapsceneType == 42)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public JunTuanRankData GetJunTuanRankDataBySide(int Side)
		{
			List<JunTuanRankData> junTuanRankingData = JunTuanClient.getInstance().GetJunTuanRankingData();
			JunTuanRankData result;
			if (null == junTuanRankingData)
			{
				result = null;
			}
			else
			{
				KarenBattleSceneInfo karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (Side <= 0 || Side > junTuanRankingData.Count || Side > karenBattleSceneInfo.MaxLegions)
				{
					result = null;
				}
				else
				{
					result = junTuanRankingData[Side - 1];
				}
			}
			return result;
		}

		public JunTuanRankData GetJunTuanRankDataByClient(GameClient client)
		{
			List<JunTuanRankData> list = JunTuanClient.getInstance().GetJunTuanRankingData();
			JunTuanRankData result;
			if (null == list)
			{
				result = null;
			}
			else
			{
				KarenBattleSceneInfo karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (list.Count > karenBattleSceneInfo.MaxLegions)
				{
					list = new List<JunTuanRankData>(list.GetRange(0, karenBattleSceneInfo.MaxLegions));
				}
				result = list.Find((JunTuanRankData x) => x.JunTuanId == client.ClientData.JunTuanId);
			}
			return result;
		}

		private bool CheckCanEnterKarenBattle(GameClient client)
		{
			bool result;
			if (client == null || client.ClientData.Faction == 0 || 0 == client.ClientData.JunTuanId)
			{
				result = false;
			}
			else if (client.ClientData.JunTuanZhiWu == 0 || 4 == client.ClientData.JunTuanZhiWu)
			{
				result = false;
			}
			else
			{
				JunTuanRankData junTuanRankDataByClient = this.GetJunTuanRankDataByClient(client);
				result = (null != junTuanRankDataByClient);
			}
			return result;
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KarenFuBenRoleData karenFuBenRoleData = JunTuanClient.getInstance().GetKarenFuBenRoleData((int)kuaFuServerLoginData.GameId, kuaFuServerLoginData.RoleId);
			bool result;
			if (karenFuBenRoleData == null || (long)karenFuBenRoleData.KuaFuMapCode != kuaFuServerLoginData.GameId || karenFuBenRoleData.KuaFuServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void OnInitGame(SceneUIClasses ManagerType, GameClient client)
		{
			lock (this.Mutex)
			{
				this.FactionIDVsServerIDDict[client.ClientData.Faction] = client.ServerId;
			}
			Global.SaveRoleParamsDateTimeToDB(client, "20019", TimeUtil.NowDateTime(), true);
			EventLogManager.AddKarenBattleEnterEvent(this.ConvertMapSceneTypeToCaiJiLingDiType(ManagerType), client);
		}

		private void TimerProc(object sender, EventArgs e)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				KarenBattleSceneInfo karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				for (int i = 0; i < karenBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)karenBattleSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= karenBattleSceneInfo.SecondsOfDay[i] - 120.0 && dateTime.TimeOfDay.TotalSeconds <= karenBattleSceneInfo.SecondsOfDay[i + 1] + 120.0)
					{
						double num = karenBattleSceneInfo.SecondsOfDay[i] - dateTime.TimeOfDay.TotalSeconds;
						double num2 = karenBattleSceneInfo.SecondsOfDay[i + 1] + 120.0 - dateTime.TimeOfDay.TotalSeconds;
						if (!this.PrepareGame)
						{
							if (num > 0.0 && num < 120.0)
							{
								this.PrepareGame = true;
								flag = true;
								break;
							}
						}
						else if (num < 0.0)
						{
							flag2 = true;
							this.PrepareGame = false;
							break;
						}
						if (!this.EndGame)
						{
							if (num2 > 0.0 && num2 < 120.0)
							{
								this.EndGame = true;
								flag3 = true;
								break;
							}
						}
						else if (num2 < 0.0)
						{
							this.EndGame = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				LogManager.WriteLog(2, "阿卡伦战场活动即将开始,准备通知军团角色进入!", null, true);
				lock (this.Mutex)
				{
					this.FactionIDVsServerIDDict.Clear();
				}
			}
			if (flag2)
			{
				int num3 = 0;
				int num4 = 0;
				lock (this.Mutex)
				{
					LogManager.WriteLog(2, "阿卡伦战场开启,可以通知已分配到场次的玩家进入游戏了", null, true);
					KarenBattleSceneInfo karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
					foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
					{
						if (this.CheckCanEnterKarenBattle(gameClient) && this.CheckMap(gameClient) && this.IsGongNengOpened(gameClient, false))
						{
							if (null != gameClient)
							{
								gameClient.sendCmd(1210, string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									1,
									1,
									num3,
									num4
								}), false);
							}
						}
					}
				}
			}
			if (flag3)
			{
				lock (this.Mutex)
				{
					foreach (KarenBattleSceneInfo karenBattleSceneInfo2 in this.SceneDataDict.Values)
					{
						KarenFuBenData karenKuaFuFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(karenBattleSceneInfo2.MapCode);
						if (karenKuaFuFuBenData != null && karenKuaFuFuBenData.ServerId == GameManager.ServerId)
						{
							SceneUIClasses mapSceneType = Global.GetMapSceneType(karenBattleSceneInfo2.MapCode);
							if (mapSceneType != 42 || KarenBattleManager_MapEast.getInstance().SceneDict.Count == 0)
							{
								if (mapSceneType != 41 || KarenBattleManager_MapWest.getInstance().SceneDict.Count == 0)
								{
									int lingDiType = this.ConvertMapSceneTypeToCaiJiLingDiType(mapSceneType);
									LingDiData lingDiData = null;
									List<LingDiData> lingDiData2 = JunTuanClient.getInstance().GetLingDiData();
									if (null != lingDiData2)
									{
										lingDiData = lingDiData2.Find((LingDiData x) => x.LingDiType == lingDiType);
									}
									RoleData4Selector oldLeader = (lingDiData != null && lingDiData.RoleData != null) ? DataHelper.BytesToObject<RoleData4Selector>(lingDiData.RoleData, 0, lingDiData.RoleData.Length) : null;
									LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, 0, 0, "", null);
									EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, 0, 0, 0);
								}
							}
						}
					}
				}
			}
			this.UpdateKuaFuMapClientCount(KarenBattleManager_MapWest.getInstance().SceneDict.Values.FirstOrDefault<KarenBattleScene>());
			this.UpdateKuaFuMapClientCount(KarenBattleManager_MapEast.getInstance().SceneDict.Values.FirstOrDefault<KarenBattleScene>());
		}

		private void UpdateKuaFuMapClientCount(KarenBattleScene scene)
		{
			if (null != scene)
			{
				CopyMap copyMap = scene.CopyMap;
				if (null != copyMap)
				{
					KarenBattleSceneInfo karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
					List<int> list = new List<int>(new int[karenBattleSceneInfo.MaxLegions]);
					List<GameClient> clientsList = copyMap.GetClientsList();
					if (clientsList != null && clientsList.Count > 0)
					{
						for (int i = 0; i < clientsList.Count; i++)
						{
							GameClient gameClient = clientsList[i];
							if (gameClient != null)
							{
								int battleWhichSide = gameClient.ClientData.BattleWhichSide;
								if (battleWhichSide > 0 && battleWhichSide < list.Count)
								{
									List<int> list2;
									int index;
									(list2 = list)[index = battleWhichSide - 1] = list2[index] + 1;
								}
							}
						}
					}
					JunTuanClient.getInstance().UpdateKuaFuMapClientCount(scene.GameId, list);
				}
			}
		}

		public bool InActivityTime()
		{
			KarenGameStates karenGameStates = KarenGameStates.None;
			this.CheckTimeCondition(ref karenGameStates);
			return karenGameStates == KarenGameStates.Start;
		}

		private int CheckTimeCondition(ref KarenGameStates state)
		{
			int result = 0;
			KarenBattleSceneInfo karenBattleSceneInfo = null;
			lock (this.Mutex)
			{
				karenBattleSceneInfo = this.SceneDataDict.Values.FirstOrDefault<KarenBattleSceneInfo>();
				if (null == karenBattleSceneInfo)
				{
					return -12;
				}
			}
			result = -2001;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				for (int i = 0; i < karenBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)karenBattleSceneInfo.TimePoints[i].Days)
					{
						if (dateTime.TimeOfDay.TotalSeconds >= karenBattleSceneInfo.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= karenBattleSceneInfo.SecondsOfDay[i + 1])
						{
							state = KarenGameStates.Start;
							result = 1;
						}
						else if (dateTime.TimeOfDay.TotalSeconds < karenBattleSceneInfo.SecondsOfDay[i])
						{
							state = KarenGameStates.Wait;
							result = 1;
						}
						else
						{
							state = KarenGameStates.None;
							result = -2001;
						}
						break;
					}
				}
			}
			return result;
		}

		public KarenBattleSceneInfo TryGetKarenBattleSceneInfoBySceneType(SceneUIClasses SceneType)
		{
			foreach (KeyValuePair<int, KarenBattleSceneInfo> keyValuePair in this.SceneDataDict)
			{
				if (Global.GetMapSceneType(keyValuePair.Value.MapCode) == SceneType)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public KarenBattleSceneInfo TryGetKarenBattleSceneInfo(int MapCode)
		{
			KarenBattleSceneInfo result = null;
			this.SceneDataDict.TryGetValue(MapCode, out result);
			return result;
		}

		public KarenBattleSceneInfo TryGetKarenBattleSceneInfoByBattleID(int BattleID)
		{
			foreach (KeyValuePair<int, KarenBattleSceneInfo> keyValuePair in this.SceneDataDict)
			{
				if (keyValuePair.Value.Id == BattleID)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public TimeSpan GetStartTime(int sceneId)
		{
			KarenBattleSceneInfo karenBattleSceneInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				if (!this.SceneDataDict.TryGetValue(sceneId, out karenBattleSceneInfo))
				{
					goto IL_13C;
				}
			}
			lock (this.Mutex)
			{
				for (int i = 0; i < karenBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)karenBattleSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= karenBattleSceneInfo.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= karenBattleSceneInfo.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(karenBattleSceneInfo.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_13C:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		public void GiveAwards(KarenBattleScene scene)
		{
			try
			{
				foreach (KarenBattleClientContextData karenBattleClientContextData in scene.ClientContextDataDict.Values)
				{
					int num;
					if (karenBattleClientContextData.BattleWhichSide == scene.SuccessSide)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(karenBattleClientContextData.RoleId);
					string text = string.Format("{0},{1}", scene.SceneInfo.Id, num);
					if (gameClient != null && gameClient.ClientData.MapCode == scene.m_nMapCode)
					{
						this.NtfCanGetAward(gameClient, num, scene.SceneInfo);
						this.GiveRoleAwards(gameClient, num, scene.SceneInfo);
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "阿卡伦系统清场调度异常");
			}
		}

		public void PushGameResultData(KarenBattleScene scene)
		{
			JunTuanRankData junTuanRankDataBySide = this.GetJunTuanRankDataBySide(scene.SuccessSide);
			if (null != junTuanRankDataBySide)
			{
				JunTuanBaseData junTuanBaseDataByJunTuanID = JunTuanManager.getInstance().GetJunTuanBaseDataByJunTuanID(junTuanRankDataBySide.JunTuanId);
				if (null == junTuanBaseDataByJunTuanID)
				{
					LogManager.WriteLog(1000, string.Format("无法获取军团基本信息 JunTuanId={0}", junTuanRankDataBySide.JunTuanId), null, true);
				}
				else if (junTuanBaseDataByJunTuanID.BhList == null || junTuanBaseDataByJunTuanID.BhList.Count == 0)
				{
					LogManager.WriteLog(1000, string.Format("军团基本信息BhList为空 JunTuanId={0}", junTuanRankDataBySide.JunTuanId), null, true);
				}
				else
				{
					int num = junTuanBaseDataByJunTuanID.BhList[0];
					int num2 = 0;
					SceneUIClasses mapSceneType = Global.GetMapSceneType(scene.m_nMapCode);
					int lingDiType = this.ConvertMapSceneTypeToCaiJiLingDiType(mapSceneType);
					LingDiData lingDiData = null;
					List<LingDiData> lingDiData2 = JunTuanClient.getInstance().GetLingDiData();
					if (null != lingDiData2)
					{
						lingDiData = lingDiData2.Find((LingDiData x) => x.LingDiType == lingDiType);
					}
					RoleData4Selector oldLeader = (lingDiData != null && lingDiData.RoleData != null) ? DataHelper.BytesToObject<RoleData4Selector>(lingDiData.RoleData, 0, lingDiData.RoleData.Length) : null;
					lock (this.Mutex)
					{
						if (!this.FactionIDVsServerIDDict.TryGetValue(num, out num2))
						{
							JunTuanData junTuanData = JunTuanClient.getInstance().GetJunTuanData(num, junTuanRankDataBySide.JunTuanId, true);
							if (null == junTuanData)
							{
								LogManager.WriteLog(1000, string.Format("无法获取JunTuanData BangHuiID={0} JunTuanId={1}", num, junTuanRankDataBySide.JunTuanId), null, true);
								return;
							}
							LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, junTuanData.LeaderRoleId, junTuanRankDataBySide.JunTuanId, junTuanRankDataBySide.JunTuanName, null);
							EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, junTuanData.LeaderZoneId, junTuanRankDataBySide.JunTuanId, junTuanData.LeaderRoleId);
							return;
						}
					}
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, num, num2);
					if (null == bangHuiDetailData)
					{
						LogManager.WriteLog(1000, string.Format("无法获取帮会详细信息 BangHuiID={0} ServerID={1}", num, num2), null, true);
					}
					else
					{
						RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bangHuiDetailData.BZRoleID), num2);
						if (roleDataEx == null || roleDataEx.RoleID <= 0)
						{
							LogManager.WriteLog(1000, string.Format("无法获取帮主详细信息 BangHuiID={0} BZRoleID={1} ServerID={2}", num, bangHuiDetailData.BZRoleID, num2), null, true);
						}
						else
						{
							JunTuanManager.getInstance().OnInitGame(roleDataEx);
							RoleData4Selector client = Global.RoleDataEx2RoleData4Selector(roleDataEx);
							LingDiCaiJiManager.getInstance().SetLingZhu(lingDiType, roleDataEx.RoleID, junTuanRankDataBySide.JunTuanId, junTuanRankDataBySide.JunTuanName, client);
							EventLogManager.AddKarenBattleEvent(lingDiType, oldLeader, roleDataEx.ZoneID, junTuanRankDataBySide.JunTuanId, roleDataEx.RoleID);
						}
					}
				}
			}
		}

		public void NtfKarenNotifyMsg(KarenBattleScene scene, KarenNotifyMsgType index, int LegionID, string param1, string param2)
		{
			KarenNotifyMsg karenNotifyMsg = new KarenNotifyMsg();
			karenNotifyMsg.index = (int)index;
			karenNotifyMsg.LegionID = LegionID;
			karenNotifyMsg.param1 = param1;
			karenNotifyMsg.param2 = param2;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<KarenNotifyMsg>(1214, karenNotifyMsg, scene.CopyMap);
		}

		private void NtfCanGetAward(GameClient client, int success, KarenBattleSceneInfo sceneInfo)
		{
			long num = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			int num2 = sceneInfo.BandJinBi;
			List<AwardsItemData> items;
			if (success > 0)
			{
				items = sceneInfo.WinAwardsItemList.Items;
			}
			else
			{
				num = (long)((double)num * 0.8);
				num2 = (int)((double)num2 * 0.8);
				items = sceneInfo.LoseAwardsItemList.Items;
			}
			num -= num % 10000L;
			num2 -= num2 % 10000;
			client.sendCmd<KarenBattleAwardsData>(1211, new KarenBattleAwardsData
			{
				Exp = num,
				BindJinBi = num2,
				Success = success,
				AwardGoodsDataList = Global.ConvertToGoodsDataList(items, -1)
			}, false);
		}

		private int GiveRoleAwards(GameClient client, int success, KarenBattleSceneInfo sceneInfo)
		{
			long num = 0L;
			int num2 = 0;
			num = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			num2 = sceneInfo.BandJinBi;
			List<AwardsItemData> items;
			if (success > 0)
			{
				items = sceneInfo.WinAwardsItemList.Items;
			}
			else
			{
				num = (long)((double)num * 0.8);
				num2 = (int)((double)num2 * 0.8);
				items = sceneInfo.LoseAwardsItemList.Items;
			}
			num -= num % 10000L;
			num2 -= num2 % 10000;
			string text = "阿卡伦战场奖励";
			SceneUIClasses mapSceneType = Global.GetMapSceneType(sceneInfo.MapCode);
			string lang;
			if (41 == mapSceneType)
			{
				lang = GLang.GetLang(2617, new object[0]);
			}
			else
			{
				lang = GLang.GetLang(2618, new object[0]);
			}
			if (items != null && !Global.CanAddGoodsNum(client, items.Count))
			{
				Global.UseMailGivePlayerAward2(client, items, GLang.GetLang(2616, new object[0]), lang, 0, 0, 0);
			}
			else if (items != null)
			{
				foreach (AwardsItemData awardsItemData in items)
				{
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, text, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
				}
			}
			if (num > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, num, true, true, false, "none");
			}
			if (num2 > 0)
			{
				GameManager.ClientMgr.AddMoney1(client, num2, text, true);
			}
			return 1;
		}

		private bool CheckMap(GameClient client)
		{
			lock (this.Mutex)
			{
				if (!this.KarenBattleEnterMap.Contains(client.ClientData.MapCode))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(6) && !GameFuncControlManager.IsGameFuncDisabled(7) && JunTuanManager.getInstance().IsGongNengOpened(client, hint);
		}

		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (e.Player.ClientData.JunTuanId > 0 && this.InActivityTime())
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2619, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool OnPreBangHuiChangeZhiWu(PreBangHuiChangeZhiWuEventObject e)
		{
			bool result;
			if (e.Player.ClientData.JunTuanId > 0 && this.InActivityTime() && e.TargetZhiWu == 1)
			{
				e.ErrorCode = -201;
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2620, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public DateTime GetLastStartTime(int sceneId)
		{
			DateTime result = DateTime.MaxValue;
			KarenBattleSceneInfo karenBattleSceneInfo = null;
			TimeSpan minValue = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = 0;
			lock (this.Mutex)
			{
				if (!this.SceneDataDict.TryGetValue(sceneId, out karenBattleSceneInfo))
				{
					return DateTime.MaxValue;
				}
			}
			lock (this.Mutex)
			{
				int num2 = 0;
				if (num2 < karenBattleSceneInfo.TimePoints.Count - 1)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)karenBattleSceneInfo.TimePoints[num2].Days)
					{
						if (dateTime.TimeOfDay.TotalSeconds <= karenBattleSceneInfo.SecondsOfDay[num2 + 1])
						{
							num = 7;
						}
					}
					else
					{
						num = dateTime.DayOfWeek - (DayOfWeek)karenBattleSceneInfo.TimePoints[num2].Days;
						if (num < 0)
						{
							num += 7;
						}
					}
					result = dateTime.AddDays((double)(-(double)num)).Date.Add(TimeSpan.FromSeconds(karenBattleSceneInfo.SecondsOfDay[num2 + 1]));
				}
			}
			return result;
		}

		public const SceneUIClasses ManagerType = 0;

		public bool GMTest = false;

		public object Mutex = new object();

		private bool PrepareGame = false;

		private bool EndGame = false;

		private static KarenBattleManager instance = new KarenBattleManager();

		public Dictionary<int, KarenBattleSceneInfo> SceneDataDict = new Dictionary<int, KarenBattleSceneInfo>();

		public HashSet<int> KarenBattleEnterMap = new HashSet<int>();

		public Dictionary<int, int> FactionIDVsServerIDDict = new Dictionary<int, int>();
	}
}
