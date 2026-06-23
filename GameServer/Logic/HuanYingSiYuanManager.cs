using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class HuanYingSiYuanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static HuanYingSiYuanManager getInstance()
		{
			return HuanYingSiYuanManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(820, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(821, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(824, 2, 2, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(822, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(828, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(826, 1, 1, HuanYingSiYuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10000, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().registerListener(31, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, HuanYingSiYuanManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10000, 25, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().removeListener(31, HuanYingSiYuanManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, HuanYingSiYuanManager.getInstance());
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
			case 820:
				return this.ProcessHuanYingSiYuanEnqueueCmd(client, nID, bytes, cmdParams);
			case 821:
				return this.ProcessHuanYingSiYuanDequeueCmd(client, nID, bytes, cmdParams);
			case 822:
				return this.ProcessHuanYingSiYuanQueueRoleCountCmd(client, nID, bytes, cmdParams);
			case 824:
				return this.ProcessHuanYingSiYuanEnterRespondCmd(client, nID, bytes, cmdParams);
			case 826:
				return this.ProcessHuanYingSiYuanScoreInfoCmd(client, nID, bytes, cmdParams);
			case 828:
				return this.ProcessHuanYingSiYuanSuccessCountCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject clientRegionEventObject = eventObject as ClientRegionEventObject;
				if (null != clientRegionEventObject)
				{
					if (clientRegionEventObject.EventType == 1 && clientRegionEventObject.Flag == 1)
					{
						this.SubmitShengBei(clientRegionEventObject.Client);
					}
				}
			}
			else if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					if (playerDeadEventObject.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEventObject.getAttackerRole(), playerDeadEventObject.getPlayer());
					}
					else
					{
						this.TryLostShengBei(playerDeadEventObject.getPlayer());
					}
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10000:
			{
				KuaFuFuBenRoleCountEvent kuaFuFuBenRoleCountEvent = eventObject as KuaFuFuBenRoleCountEvent;
				if (null != kuaFuFuBenRoleCountEvent)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuFuBenRoleCountEvent.RoleId);
					if (null != gameClient)
					{
						gameClient.sendCmd<int>(822, kuaFuFuBenRoleCountEvent.RoleCount, false);
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10001:
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
								gameClient.sendCmd<KuaFuServerLoginData>(823, clientKuaFuServerLoginData, false);
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			}
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.ShengBeiDataDict.Clear();
					text = "Config/HolyGrail.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ShengBeiData shengBeiData = new ShengBeiData();
						XElement xml;
						shengBeiData.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						shengBeiData.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						shengBeiData.Time = (int)Global.GetSafeAttributeLong(xml, "Time");
						shengBeiData.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
						shengBeiData.Score = (int)Global.GetSafeAttributeLong(xml, "Score");
						shengBeiData.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						shengBeiData.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(shengBeiData.GoodsID);
						if (null != equipPropItem)
						{
							shengBeiData.BufferProps = equipPropItem.ExtProps;
						}
						else
						{
							result = false;
							LogManager.WriteLog(1000, "幻影寺院的圣杯Buffer的GoodsID在物品表中找不到", null, true);
						}
						this.RuntimeData.ShengBeiDataDict[shengBeiData.ID] = shengBeiData;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/TempleMirageRebirth.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						HuanYingSiYuanBirthPoint huanYingSiYuanBirthPoint = new HuanYingSiYuanBirthPoint();
						XElement xml;
						huanYingSiYuanBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						huanYingSiYuanBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						huanYingSiYuanBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						huanYingSiYuanBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[huanYingSiYuanBirthPoint.ID] = huanYingSiYuanBirthPoint;
					}
					this.RuntimeData.ContinuityKillAwardDict.Clear();
					text = "Config/ContinuityKillAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ContinuityKillAward continuityKillAward = new ContinuityKillAward();
						XElement xml;
						continuityKillAward.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						continuityKillAward.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
						continuityKillAward.Score = (int)Global.GetSafeAttributeLong(xml, "Score");
						this.RuntimeData.ContinuityKillAwardDict[continuityKillAward.Num] = continuityKillAward;
					}
					this.RuntimeData.MapCode = 0;
					text = "Config/TempleMirage.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					using (IEnumerator<XElement> enumerator = enumerable.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							XElement xml = enumerator.Current;
							this.RuntimeData.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
							this.RuntimeData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
							this.RuntimeData.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
							this.RuntimeData.MinRequestNum = (int)Global.GetSafeAttributeLong(xml, "MinRequestNum");
							this.RuntimeData.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
							this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
							this.RuntimeData.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
							this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
							this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
							if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-'))
							{
								result = false;
								LogManager.WriteLog(1000, "读取幻影寺院时间配置(TimePoints)出错", null, true);
							}
							GameMap gameMap = null;
							if (!GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapCode, out gameMap))
							{
								LogManager.WriteLog(1000, string.Format("缺少幻影寺院地图 {0}", this.RuntimeData.MapCode), null, true);
							}
							this.RuntimeData.MapGridWidth = gameMap.MapGridWidth;
							this.RuntimeData.MapGridHeight = gameMap.MapGridHeight;
						}
					}
					this.RuntimeData.TempleMirageEXPAward = GameManager.systemParamsList.GetParamValueIntByName("TempleMirageEXPAward", -1);
					this.RuntimeData.TempleMirageWin = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMirageWin", -1);
					this.RuntimeData.TempleMiragePK = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMiragePK", -1);
					this.RuntimeData.TempleMirageMinJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("TempleMirageMinJiFen", -1);
					this.RuntimeData.AwardGoods = GameManager.systemParamsList.GetParamValueByName("TempleMirageGoodsAward");
					if (!ConfigParser.ParseStrInt2(GameManager.systemParamsList.GetParamValueByName("TempleMirageWinNum"), ref this.RuntimeData.TempleMirageWinExtraNum, ref this.RuntimeData.TempleMirageWinExtraRate, ','))
					{
						result = false;
						LogManager.WriteLog(1000, "读取幻影寺院多倍奖励配置(TempleMirageWin)出错", null, true);
					}
					if (!ConfigParser.ParseStrInt2(GameManager.systemParamsList.GetParamValueByName("TempleMirageAward"), ref this.RuntimeData.TempleMirageAwardChengJiu, ref this.RuntimeData.TempleMirageAwardShengWang, ','))
					{
						result = false;
						LogManager.WriteLog(1000, "读取幻影寺院多倍奖励配置(TempleMirageWin)出错", null, true);
					}
					List<List<int>> list = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("TempleMirageLevel"), true, '|', ',');
					if (list.Count == 0)
					{
						result = false;
						LogManager.WriteLog(1000, "读取幻影寺院等级分组配置(TempleMirageLevel)出错", null, true);
					}
					else
					{
						for (int i = 0; i < list.Count; i++)
						{
							List<int> list2 = list[i];
							this.RuntimeData.Range2GroupIndexDict.Add(new RangeKey(Global.GetUnionLevel(list2[0], list2[1], false), Global.GetUnionLevel(list2[2], list2[3], false), null), i + 1);
						}
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

		public void GMStartHuoDongNow()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, "00:00-23:59:59", true, '|', '-');
				}
			}
			catch (Exception ex)
			{
			}
		}

		public bool ProcessHuanYingSiYuanEnqueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
				int num = -2001;
				int groupIndex = 1;
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
					if (num >= 1)
					{
						if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out groupIndex))
						{
							num = -12;
						}
					}
				}
				if (num >= 0)
				{
					num = HuanYingSiYuanClient.getInstance().HuanYingSiYuanSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 1, groupIndex, client.ClientData.CombatForce);
					if (num == 1)
					{
						client.ClientData.SignUpGameType = 1;
						GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 1);
					}
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

		public bool ProcessHuanYingSiYuanQueueRoleCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int num = -2001;
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
				if (num >= 0)
				{
					num = HuanYingSiYuanClient.getInstance().GetRoleKuaFuFuBenRoleCount(client.ClientData.RoleID);
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

		public bool ProcessHuanYingSiYuanSuccessCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				int offsetDayNow = Global.GetOffsetDayNow();
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
				if (roleParamsInt32FromDB == offsetDayNow)
				{
					cmdData = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
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

		public bool ProcessHuanYingSiYuanScoreInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessHuanYingSiYuanEnterRespondCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int num = 1;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				lock (this.RuntimeData.Mutex)
				{
					int num3;
					if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out num3))
					{
						num = -12;
					}
				}
				client.ClientData.SignUpGameType = 0;
				if (num >= 0)
				{
					if (num2 > 0)
					{
						num = HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 4, false);
						if (num >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							client.sendCmd<int>(nID, num, false);
							client.sendCmd<int>(821, 0, false);
						}
					}
					else
					{
						HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 0, false);
						Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						client.sendCmd<int>(nID, 0, false);
						client.sendCmd<int>(821, 0, false);
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

		public bool ProcessHuanYingSiYuanDequeueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int num = 1;
				lock (this.RuntimeData.Mutex)
				{
					int num2;
					if (!this.RuntimeData.Range2GroupIndexDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out num2))
					{
						num = -12;
					}
				}
				client.ClientData.SignUpGameType = 0;
				if (num >= 0)
				{
					num = HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 0, false);
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

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int num = client.ClientData.BattleWhichSide;
			if (num <= 0)
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				num = HuanYingSiYuanClient.getInstance().GetRoleBattleWhichSide((int)clientKuaFuServerLoginData.GameId, clientKuaFuServerLoginData.RoleId);
				if (num > 0)
				{
					client.ClientData.BattleWhichSide = num;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				HuanYingSiYuanBirthPoint huanYingSiYuanBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(num, out huanYingSiYuanBirthPoint))
				{
					posX = huanYingSiYuanBirthPoint.PosX;
					posY = huanYingSiYuanBirthPoint.PosY;
					return num;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public void InitRoleDailyHYSYData(GameClient client)
		{
			if (this.IsGongNengOpened(client, false))
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
				int num = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessDayId");
				int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
				int offsetDayNow = Global.GetOffsetDayNow();
				if (num + 1 != offsetDayNow)
				{
					if (roleParamsInt32FromDB + 1 == offsetDayNow)
					{
						num = roleParamsInt32FromDB;
						int nValue = roleParamsInt32FromDB2;
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessDayId", num, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessCount", nValue, true);
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessDayId", offsetDayNow - 1, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "HysyYTDSuccessCount", 0, true);
					}
				}
			}
		}

		public int GetLeftCount(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "HysySuccessDayId");
			int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "HysySuccessCount");
			int offsetDayNow = Global.GetOffsetDayNow();
			int num = 3;
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("TempleMirageWinNum", ',');
			if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length == 2)
			{
				num = paramValueIntArrayByName[0];
			}
			int result;
			if (roleParamsInt32FromDB == offsetDayNow)
			{
				result = Global.GMax(0, num - roleParamsInt32FromDB2);
			}
			else
			{
				result = num;
			}
			return result;
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
				client.ClientData.MapCode = this.RuntimeData.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.BattleWhichSide = birthPoint;
				int num = 0;
				lock (HuanYingSiYuanManager.Mutex)
				{
					if (!this.GameId2FuBenSeq.TryGetValue((int)Global.GetClientKuaFuServerLoginData(client).GameId, out num))
					{
						num = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.GameId2FuBenSeq[(int)Global.GetClientKuaFuServerLoginData(client).GameId] = num;
					}
				}
				Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = num;
				client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
				result = true;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.RuntimeData.MapCode)
			{
				int posX;
				int posY;
				int birthPoint = this.GetBirthPoint(client, out posX, out posY);
				if (birthPoint <= 0)
				{
					result = false;
				}
				else
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, -1);
					Global.ClientRealive(client, posX, posY, -1);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("HuanYingSiYuan") && !GameFuncControlManager.IsGameFuncDisabled(3) && GlobalNew.IsGongNengOpened(client, 57, hint);
		}

		public bool AddHuanYingSiYuanCopyScenes(GameClient client, CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapCode)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene = null;
					if (!this.HuanYingSiYuanSceneDict.TryGetValue(fuBenSeqID, out huanYingSiYuanScene))
					{
						huanYingSiYuanScene = new HuanYingSiYuanScene();
						huanYingSiYuanScene.CopyMap = copyMap;
						huanYingSiYuanScene.CleanAllInfo();
						huanYingSiYuanScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						huanYingSiYuanScene.m_nMapCode = mapCode;
						huanYingSiYuanScene.CopyMapId = copyMap.CopyMapID;
						huanYingSiYuanScene.FuBenSeqId = fuBenSeqID;
						huanYingSiYuanScene.m_nPlarerCount = 1;
						this.HuanYingSiYuanSceneDict[fuBenSeqID] = huanYingSiYuanScene;
					}
					else
					{
						huanYingSiYuanScene.m_nPlarerCount++;
					}
					if (client.ClientData.BattleWhichSide == 1)
					{
						huanYingSiYuanScene.ScoreInfoData.Count1 += 1L;
					}
					else
					{
						huanYingSiYuanScene.ScoreInfoData.Count2++;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
				}
				client.SceneContextData2 = new HuanYingSiYuanLianShaContextData();
				HuanYingSiYuanClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 5, 0, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 1);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveHuanYingSiYuanListCopyScenes(CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapCode)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene;
					if (this.HuanYingSiYuanSceneDict.TryRemove(copyMap.FuBenSeqID, out huanYingSiYuanScene))
					{
						this.GameId2FuBenSeq.Remove(huanYingSiYuanScene.GameId);
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

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= HuanYingSiYuanManager.NextHeartBeatTicks)
			{
				HuanYingSiYuanManager.NextHeartBeatTicks = num + 1020L;
				foreach (HuanYingSiYuanScene huanYingSiYuanScene in this.HuanYingSiYuanSceneDict.Values)
				{
					lock (HuanYingSiYuanManager.Mutex)
					{
						int fuBenSeqId = huanYingSiYuanScene.FuBenSeqId;
						int copyMapId = huanYingSiYuanScene.CopyMapId;
						int nMapCode = huanYingSiYuanScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = huanYingSiYuanScene.CopyMap;
							DateTime time = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (huanYingSiYuanScene.m_eStatus == 0)
							{
								huanYingSiYuanScene.m_lPrepareTime = num2;
								huanYingSiYuanScene.m_lBeginTime = num2 + (long)(this.RuntimeData.PrepareSecs * 1000);
								huanYingSiYuanScene.m_eStatus = 1;
								huanYingSiYuanScene.StateTimeData.GameType = 1;
								huanYingSiYuanScene.StateTimeData.State = huanYingSiYuanScene.m_eStatus;
								huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
							}
							else if (huanYingSiYuanScene.m_eStatus == 1)
							{
								if (num2 >= huanYingSiYuanScene.m_lBeginTime)
								{
									huanYingSiYuanScene.m_eStatus = 2;
									huanYingSiYuanScene.m_lEndTime = num2 + (long)(this.RuntimeData.FightingSecs * 1000);
									huanYingSiYuanScene.StateTimeData.GameType = 1;
									huanYingSiYuanScene.StateTimeData.State = huanYingSiYuanScene.m_eStatus;
									huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
									foreach (ShengBeiData shengBeiData in this.RuntimeData.ShengBeiDataDict.Values)
									{
										HuanYingSiYuanShengBeiContextData contextData = new HuanYingSiYuanShengBeiContextData
										{
											UniqueId = this.GetInternalId(),
											FuBenSeqId = huanYingSiYuanScene.FuBenSeqId,
											ShengBeiId = shengBeiData.ID,
											BufferGoodsId = shengBeiData.GoodsID,
											MonsterId = shengBeiData.MonsterID,
											PosX = shengBeiData.PosX,
											PosY = shengBeiData.PosY,
											CopyMapID = huanYingSiYuanScene.CopyMapId,
											Score = shengBeiData.Score,
											Time = shengBeiData.Time,
											BufferProps = shengBeiData.BufferProps
										};
										this.CreateMonster(huanYingSiYuanScene, contextData);
									}
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
								}
							}
							else if (huanYingSiYuanScene.m_eStatus == 2)
							{
								if (num2 >= huanYingSiYuanScene.m_lEndTime)
								{
									int successSide = 0;
									if (huanYingSiYuanScene.ScoreInfoData.Score1 > huanYingSiYuanScene.ScoreInfoData.Score2)
									{
										successSide = 1;
									}
									else if (huanYingSiYuanScene.ScoreInfoData.Score2 > huanYingSiYuanScene.ScoreInfoData.Score1)
									{
										successSide = 2;
									}
									this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, successSide);
								}
								else
								{
									this.CheckShengBeiBufferTime(huanYingSiYuanScene, num);
								}
							}
							else if (huanYingSiYuanScene.m_eStatus == 3)
							{
								huanYingSiYuanScene.m_eStatus = 4;
								huanYingSiYuanScene.m_lEndTime = num;
								huanYingSiYuanScene.m_lLeaveTime = huanYingSiYuanScene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
								HuanYingSiYuanClient.getInstance().GameFuBenChangeState(huanYingSiYuanScene.GameId, 3, time);
								huanYingSiYuanScene.StateTimeData.GameType = 1;
								huanYingSiYuanScene.StateTimeData.State = 3;
								huanYingSiYuanScene.StateTimeData.EndTicks = huanYingSiYuanScene.m_lLeaveTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, huanYingSiYuanScene.CopyMap);
								this.GiveAwards(huanYingSiYuanScene);
							}
							else if (huanYingSiYuanScene.m_eStatus == 4)
							{
								if (num2 >= huanYingSiYuanScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(huanYingSiYuanScene.m_lLeaveTime);
									huanYingSiYuanScene.m_eStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "幻影寺院清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public int GetInternalId()
		{
			int num = Interlocked.Increment(ref this.InternalShengBeiId);
			if (num < 0)
			{
				num = (this.InternalShengBeiId = 1);
			}
			return num;
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, huanYingSiYuanScene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, false);
					}
				}
			}
		}

		public void CreateMonster(HuanYingSiYuanScene scene, HuanYingSiYuanShengBeiContextData contextData = null)
		{
			int gridX = contextData.PosX / this.RuntimeData.MapGridWidth;
			int gridY = contextData.PosY / this.RuntimeData.MapGridHeight;
			GameManager.MonsterZoneMgr.AddDynamicMonsters(this.RuntimeData.MapCode, contextData.MonsterId, contextData.CopyMapID, 1, gridX, gridY, 0, 0, 25, contextData, null);
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData = client.SceneContextData as HuanYingSiYuanShengBeiContextData;
			if (null != huanYingSiYuanShengBeiContextData)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					HuanYingSiYuanScene huanYingSiYuanScene;
					if (this.HuanYingSiYuanSceneDict.TryGetValue(huanYingSiYuanShengBeiContextData.FuBenSeqId, out huanYingSiYuanScene))
					{
						if (huanYingSiYuanScene.ShengBeiContextDict.ContainsKey(huanYingSiYuanShengBeiContextData.UniqueId))
						{
							return -300;
						}
					}
				}
			}
			huanYingSiYuanShengBeiContextData = (monster.Tag as HuanYingSiYuanShengBeiContextData);
			int result;
			if (huanYingSiYuanShengBeiContextData != null)
			{
				result = huanYingSiYuanShengBeiContextData.Time;
			}
			else
			{
				result = -302;
			}
			return result;
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData = monster.Tag as HuanYingSiYuanShengBeiContextData;
			HuanYingSiYuanScene huanYingSiYuanScene = null;
			if (null != huanYingSiYuanShengBeiContextData)
			{
				long endTicks = TimeUtil.NOW() + (long)(this.RuntimeData.HoldShengBeiSecs * 1000);
				lock (HuanYingSiYuanManager.Mutex)
				{
					huanYingSiYuanShengBeiContextData.OwnerRoleId = client.ClientData.RoleID;
					huanYingSiYuanShengBeiContextData.EndTicks = endTicks;
					if (this.HuanYingSiYuanSceneDict.TryGetValue(huanYingSiYuanShengBeiContextData.FuBenSeqId, out huanYingSiYuanScene))
					{
						if (huanYingSiYuanScene.m_eStatus == 2)
						{
							this.GetShengBei(huanYingSiYuanScene, client, huanYingSiYuanShengBeiContextData);
						}
					}
				}
			}
		}

		public void CompleteHuanYingSiYuanScene(HuanYingSiYuanScene huanYingSiYuanScene, int successSide)
		{
			huanYingSiYuanScene.m_eStatus = 3;
			huanYingSiYuanScene.SuccessSide = successSide;
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			this.TryLostShengBei(other);
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					if (huanYingSiYuanScene.m_eStatus == 2)
					{
						int num = 0;
						HuanYingSiYuanLianShaContextData huanYingSiYuanLianShaContextData = client.SceneContextData2 as HuanYingSiYuanLianShaContextData;
						HuanYingSiYuanLianShaContextData huanYingSiYuanLianShaContextData2 = other.SceneContextData2 as HuanYingSiYuanLianShaContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						if (null != huanYingSiYuanLianShaContextData)
						{
							huanYingSiYuanLianShaContextData.KillNum++;
							ContinuityKillAward continuityKillAward;
							if (this.RuntimeData.ContinuityKillAwardDict.TryGetValue(huanYingSiYuanLianShaContextData.KillNum, out continuityKillAward))
							{
								huanYingSiYuanAddScore.ByLianShaNum = 1;
								huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
								huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
								huanYingSiYuanLianSha.LianShaType = continuityKillAward.ID;
								huanYingSiYuanLianSha.ExtScore = continuityKillAward.Score;
								huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
								num += huanYingSiYuanLianSha.ExtScore;
							}
						}
						if (null != huanYingSiYuanLianShaContextData2)
						{
							if (huanYingSiYuanLianShaContextData2.KillNum >= 2)
							{
								huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
								huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
								huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
								huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
								huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
								huanYingSiYuanLianshaOver.ExtScore = huanYingSiYuanLianShaContextData2.KillNum * 5;
								num += huanYingSiYuanLianshaOver.ExtScore;
							}
							huanYingSiYuanLianShaContextData2.KillNum = 0;
						}
						num += this.RuntimeData.TempleMiragePK;
						huanYingSiYuanAddScore.Score = num;
						if (client.ClientData.BattleWhichSide == 1)
						{
							huanYingSiYuanScene.ScoreInfoData.Score1 += num;
							if (huanYingSiYuanScene.ScoreInfoData.Score1 >= this.RuntimeData.TempleMirageWin)
							{
								this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 1);
							}
						}
						else
						{
							huanYingSiYuanScene.ScoreInfoData.Score2 += num;
							if (huanYingSiYuanScene.ScoreInfoData.Score2 >= this.RuntimeData.TempleMirageWin)
							{
								this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 2);
							}
						}
						if (null != huanYingSiYuanLianShaContextData)
						{
							huanYingSiYuanLianShaContextData.TotalScore += num;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(818, huanYingSiYuanLianSha, huanYingSiYuanScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(819, huanYingSiYuanLianshaOver, huanYingSiYuanScene.CopyMap);
						}
					}
				}
			}
		}

		private void GetShengBei(HuanYingSiYuanScene huanYingSiYuanScene, GameClient client, HuanYingSiYuanShengBeiContextData contextData)
		{
			if (null != contextData)
			{
				lock (HuanYingSiYuanManager.Mutex)
				{
					client.SceneContextData = contextData;
					huanYingSiYuanScene.ShengBeiContextDict[contextData.UniqueId] = contextData;
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.HysyShengBei,
						contextData.BufferProps
					});
					double[] actionParams = new double[]
					{
						(double)contextData.BufferGoodsId,
						(double)this.RuntimeData.HoldShengBeiSecs
					};
					Global.UpdateBufferData(client, BufferItemTypes.HysyShengBei, actionParams, 0, true);
				}
			}
		}

		private HuanYingSiYuanShengBeiContextData LostShengBei(GameClient client)
		{
			HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData = null;
			if (null != client.SceneContextData)
			{
				huanYingSiYuanShengBeiContextData = (client.SceneContextData as HuanYingSiYuanShengBeiContextData);
				if (null != huanYingSiYuanShengBeiContextData)
				{
					lock (HuanYingSiYuanManager.Mutex)
					{
						PropsCacheManager propsCacheManager = client.ClientData.PropsCacheManager;
						object[] array = new object[2];
						array[0] = PropsSystemTypes.HysyShengBei;
						propsCacheManager.SetExtProps(array);
						double[] array2 = new double[2];
						double[] actionParams = array2;
						Global.UpdateBufferData(client, BufferItemTypes.HysyShengBei, actionParams, 0, true);
						client.SceneContextData = null;
					}
				}
			}
			return huanYingSiYuanShengBeiContextData;
		}

		private void SubmitShengBei(GameClient client)
		{
			if (null != client.SceneContextData)
			{
				HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData = client.SceneContextData as HuanYingSiYuanShengBeiContextData;
				if (null != huanYingSiYuanShengBeiContextData)
				{
					long num = TimeUtil.NOW();
					if (huanYingSiYuanShengBeiContextData.EndTicks - num <= (long)((this.RuntimeData.HoldShengBeiSecs - this.RuntimeData.MinSubmitShengBeiSecs) * 1000))
					{
						Point start = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
						lock (this.RuntimeData.Mutex)
						{
							HuanYingSiYuanBirthPoint huanYingSiYuanBirthPoint;
							if (!this.RuntimeData.MapBirthPointDict.TryGetValue(client.ClientData.BattleWhichSide, out huanYingSiYuanBirthPoint))
							{
								return;
							}
							Point end = new Point((double)huanYingSiYuanBirthPoint.PosX, (double)huanYingSiYuanBirthPoint.PosY);
							if (Global.GetTwoPointDistance(start, end) > 1000.0)
							{
								return;
							}
						}
						lock (HuanYingSiYuanManager.Mutex)
						{
							HuanYingSiYuanScene huanYingSiYuanScene;
							if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene) && huanYingSiYuanScene.m_eStatus == 2)
							{
								if (huanYingSiYuanScene.ShengBeiContextDict.Remove(huanYingSiYuanShengBeiContextData.UniqueId))
								{
									this.LostShengBei(client);
									this.CreateMonster(huanYingSiYuanScene, huanYingSiYuanShengBeiContextData);
									huanYingSiYuanShengBeiContextData.OwnerRoleId = 0;
									if (client.ClientData.BattleWhichSide == 1)
									{
										huanYingSiYuanScene.ScoreInfoData.Score1 += huanYingSiYuanShengBeiContextData.Score;
										if (huanYingSiYuanScene.ScoreInfoData.Score1 >= this.RuntimeData.TempleMirageWin)
										{
											this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 1);
										}
									}
									else
									{
										huanYingSiYuanScene.ScoreInfoData.Score2 += huanYingSiYuanShengBeiContextData.Score;
										if (huanYingSiYuanScene.ScoreInfoData.Score2 >= this.RuntimeData.TempleMirageWin)
										{
											this.CompleteHuanYingSiYuanScene(huanYingSiYuanScene, 2);
										}
									}
									HuanYingSiYuanLianShaContextData huanYingSiYuanLianShaContextData = client.SceneContextData2 as HuanYingSiYuanLianShaContextData;
									if (null != huanYingSiYuanLianShaContextData)
									{
										huanYingSiYuanLianShaContextData.TotalScore += huanYingSiYuanShengBeiContextData.Score;
									}
									HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
									huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
									huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
									huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
									huanYingSiYuanAddScore.Score = huanYingSiYuanShengBeiContextData.Score;
									huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
									huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
									CopyMap copyMap = huanYingSiYuanScene.CopyMap;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanAddScore>(829, huanYingSiYuanAddScore, copyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, copyMap);
								}
							}
						}
					}
				}
			}
		}

		private void CheckShengBeiBufferTime(HuanYingSiYuanScene huanYingSiYuanScene, long nowTicks)
		{
			List<HuanYingSiYuanShengBeiContextData> list = new List<HuanYingSiYuanShengBeiContextData>();
			lock (HuanYingSiYuanManager.Mutex)
			{
				if (huanYingSiYuanScene.m_eStatus == 2)
				{
					if (huanYingSiYuanScene.ShengBeiContextDict.Count > 0)
					{
						foreach (HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData in huanYingSiYuanScene.ShengBeiContextDict.Values)
						{
							if (huanYingSiYuanShengBeiContextData.EndTicks < nowTicks)
							{
								list.Add(huanYingSiYuanShengBeiContextData);
								if (huanYingSiYuanShengBeiContextData.OwnerRoleId != 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(huanYingSiYuanShengBeiContextData.OwnerRoleId);
									if (null != gameClient)
									{
										this.LostShengBei(gameClient);
									}
								}
								huanYingSiYuanShengBeiContextData.OwnerRoleId = 0;
								this.CreateMonster(huanYingSiYuanScene, huanYingSiYuanShengBeiContextData);
							}
						}
					}
					if (list.Count > 0)
					{
						foreach (HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData in list)
						{
							huanYingSiYuanScene.ShengBeiContextDict.Remove(huanYingSiYuanShengBeiContextData.UniqueId);
						}
					}
				}
			}
		}

		private void TryLostShengBei(GameClient client)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				HuanYingSiYuanScene huanYingSiYuanScene = null;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					HuanYingSiYuanShengBeiContextData huanYingSiYuanShengBeiContextData = this.LostShengBei(client);
					if (null != huanYingSiYuanShengBeiContextData)
					{
						huanYingSiYuanShengBeiContextData.OwnerRoleId = 0;
						huanYingSiYuanScene.ShengBeiContextDict.Remove(huanYingSiYuanShengBeiContextData.UniqueId);
						this.CreateMonster(huanYingSiYuanScene, huanYingSiYuanShengBeiContextData);
					}
				}
			}
		}

		public void GiveAwards(HuanYingSiYuanScene huanYingSiYuanScene)
		{
			try
			{
				List<GameClient> clientsList = huanYingSiYuanScene.CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					int offsetDayNow = Global.GetOffsetDayNow();
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
						{
							bool flag = false;
							double num = 0.5;
							int num2 = 1;
							int num3 = 0;
							string text = null;
							HuanYingSiYuanLianShaContextData huanYingSiYuanLianShaContextData = gameClient.SceneContextData2 as HuanYingSiYuanLianShaContextData;
							if (huanYingSiYuanLianShaContextData != null && huanYingSiYuanLianShaContextData.TotalScore >= this.RuntimeData.TempleMirageMinJiFen)
							{
								if (gameClient.ClientData.BattleWhichSide == huanYingSiYuanScene.SuccessSide)
								{
									flag = true;
									num = 1.0;
									int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(gameClient, "HysySuccessDayId");
									if (roleParamsInt32FromDB == offsetDayNow)
									{
										num3 = Global.GetRoleParamsInt32FromDB(gameClient, "HysySuccessCount");
										if (num3 < this.RuntimeData.TempleMirageWinExtraNum)
										{
											num2 = this.RuntimeData.TempleMirageWinExtraRate;
											text = this.RuntimeData.AwardGoods;
										}
									}
									else
									{
										num2 = this.RuntimeData.TempleMirageWinExtraRate;
										text = this.RuntimeData.AwardGoods;
									}
								}
							}
							else
							{
								num = 0.0;
								num2 = 0;
							}
							long num4 = (long)((double)this.RuntimeData.TempleMirageEXPAward * num * (double)gameClient.ClientData.ChangeLifeCount);
							int num5 = (int)((double)this.RuntimeData.TempleMirageAwardChengJiu * num);
							int num6 = (int)((double)this.RuntimeData.TempleMirageAwardShengWang * num);
							if (num4 > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(gameClient, num4 * (long)num2, false, true, false, "none");
							}
							if (num5 > 0)
							{
								ChengJiuManager.AddChengJiuPoints(gameClient, "幻影寺院获得成就", num5 * num2, true, true);
							}
							if (num6 > 0)
							{
								GameManager.ClientMgr.ModifyShengWangValue(gameClient, num6 * num2, "幻影寺院奖励", false, true);
							}
							if (!string.IsNullOrEmpty(text))
							{
								AwardsItemList awardsItemList = new AwardsItemList();
								awardsItemList.Add(text);
								List<GoodsData> list = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
								if (!Global.CanAddGoodsDataList(gameClient, list))
								{
									GameManager.ClientMgr.SendMailWhenPacketFull(gameClient, list, GLang.GetLang(385, new object[0]), GLang.GetLang(385, new object[0]));
								}
								else
								{
									for (int j = 0; j < list.Count; j++)
									{
										GoodsData goodsData = list[j];
										if (null != goodsData)
										{
											goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "幻影寺院奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
										}
									}
								}
							}
							HuanYingSiYuanAwardsData cmdData = new HuanYingSiYuanAwardsData
							{
								SuccessSide = huanYingSiYuanScene.SuccessSide,
								Exp = num4,
								ShengWang = num6,
								ChengJiuAward = num5,
								AwardsRate = num2,
								AwardGoods = text
							};
							if (flag)
							{
								if (num > 0.0)
								{
									Global.SaveRoleParamsInt32ValueToDB(gameClient, "HysySuccessDayId", offsetDayNow, true);
									Global.SaveRoleParamsInt32ValueToDB(gameClient, "HysySuccessCount", num3 + 1, true);
									GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 1, 0, 1);
									if (huanYingSiYuanScene.ScoreInfoData.Score1 >= 1000 || huanYingSiYuanScene.ScoreInfoData.Score2 >= 1000)
									{
										GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 0, 1, 1);
									}
									GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_HuanYingSiYuan, new int[0]));
								}
							}
							gameClient.sendCmd<HuanYingSiYuanAwardsData>(825, cmdData, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "幻影寺院清场调度异常");
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (HuanYingSiYuanManager.Mutex)
			{
				this.TryLostShengBei(client);
				HuanYingSiYuanScene huanYingSiYuanScene = null;
				if (this.HuanYingSiYuanSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out huanYingSiYuanScene))
				{
					huanYingSiYuanScene.m_nPlarerCount--;
					if (client.ClientData.BattleWhichSide == 1)
					{
						huanYingSiYuanScene.ScoreInfoData.Count1 -= 1L;
					}
					else
					{
						huanYingSiYuanScene.ScoreInfoData.Count2--;
					}
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanScoreInfoData>(826, huanYingSiYuanScene.ScoreInfoData, huanYingSiYuanScene.CopyMap);
				}
			}
			HuanYingSiYuanClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 0, 0, 0);
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses ManagerType = 25;

		private static HuanYingSiYuanManager instance = new HuanYingSiYuanManager();

		public HuanYingSiYuanData RuntimeData = new HuanYingSiYuanData();

		public static object Mutex = new object();

		private int InternalShengBeiId = 0;

		public ConcurrentDictionary<int, HuanYingSiYuanScene> HuanYingSiYuanSceneDict = new ConcurrentDictionary<int, HuanYingSiYuanScene>();

		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();

		private static long NextHeartBeatTicks = 0L;
	}
}
