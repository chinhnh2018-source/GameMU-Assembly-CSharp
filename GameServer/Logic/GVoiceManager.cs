using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class GVoiceManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static GVoiceManager getInstance()
		{
			return GVoiceManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("GVoiceManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1110, 1, 1, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1112, 2, 2, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1111, 3, 3, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(28, GVoiceManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, GVoiceManager.getInstance());
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
			bool result;
			switch (nID)
			{
			case 1110:
				result = this.ProcessGetGVoiceSceneDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1111:
				result = this.ProcessGVoiceSetRoleListCmd(client, nID, bytes, cmdParams);
				break;
			case 1112:
				result = this.ProcessGVoiceGetRoleListCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				if (null != onStartPlayGameEventObject)
				{
					this.OnStartPlayGame(onStartPlayGameEventObject.Client);
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
		}

		public bool InitConfig()
		{
			bool result = true;
			string arg = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					int num = 0;
					this.RuntimeData.SDKGameID = StringEncrypt.Decrypt(GameManager.PlatConfigMgr.GetGameConfigItemStr("gvoice_app_id", ""), "eabcix675u49,/", "3&3i4x4^+-0");
					this.RuntimeData.SDKKey = StringEncrypt.Decrypt(GameManager.PlatConfigMgr.GetGameConfigItemStr("gvoice_app_key", ""), "eabcix675u49,/", "3&3i4x4^+-0");
					this.RuntimeData.VoiceMessage = GameManager.systemParamsList.GetParamValueIntArrayByName("VoiceMessage", ',');
					this.RuntimeData.VoicePowerNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VoicePowerNum", ',');
					this.RuntimeData.MapCode2GVoiceTypeDict.Clear();
					this.RuntimeData.MapCode2GVoiceGroupDict.Clear();
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhanMengVoice");
					if (!string.IsNullOrEmpty(paramValueByName))
					{
						List<List<int>> list = ConfigHelper.ParserIntArrayList(paramValueByName, false, '|', ',');
						foreach (List<int> list2 in list)
						{
							num++;
							foreach (int key in list2)
							{
								this.RuntimeData.MapCode2GVoiceTypeDict[key] = 1;
								this.RuntimeData.MapCode2GVoiceGroupDict[key] = num;
							}
						}
					}
					paramValueByName = GameManager.systemParamsList.GetParamValueByName("JunTuanVoice");
					if (!string.IsNullOrEmpty(paramValueByName))
					{
						List<List<int>> list = ConfigHelper.ParserIntArrayList(paramValueByName, false, '|', ',');
						foreach (List<int> list2 in list)
						{
							num++;
							foreach (int key in list2)
							{
								this.RuntimeData.MapCode2GVoiceTypeDict[key] = 2;
								this.RuntimeData.MapCode2GVoiceGroupDict[key] = num;
							}
						}
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", arg), ex, true);
				}
			}
			return result;
		}

		public void SendGVoiceInitData(GameClient client)
		{
			client.sendCmd<GVoiceSceneData>(1110, new GVoiceSceneData
			{
				SDKGameID = this.RuntimeData.SDKGameID,
				SDKKey = this.RuntimeData.SDKKey
			}, false);
		}

		public bool ProcessGetGVoiceSceneDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GVoiceSceneData gvoiceSceneData = new GVoiceSceneData();
				GVoicePriorityData gvoicePriorityData = null;
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int mapCode = client.ClientData.MapCode;
				MapTypes mapType = Global.GetMapType(mapCode);
				lock (this.RuntimeData.Mutex)
				{
					int num;
					if (this.RuntimeData.MapCode2GVoiceTypeDict.TryGetValue(mapCode, out num))
					{
						string text = null;
						if (num == 1)
						{
							if (!this.RuntimeData.ZhanMengGVoiceDict.TryGetValue(faction, out text))
							{
								string[] array = Global.SendToDB<int>(1112, faction, client.ServerId);
								if (array != null && array.Length >= 1)
								{
									text = array[0];
									this.RuntimeData.ZhanMengGVoiceDict[faction] = text;
								}
							}
							client.ClientData.GVoicePrioritys = text;
							gvoicePriorityData = new GVoicePriorityData
							{
								ID = faction,
								Type = num,
								RoleIdList = text
							};
						}
						else if (num == 2)
						{
							if (!this.RuntimeData.JunTuanGVoiceDict.TryGetValue(junTuanId, out text))
							{
								text = JunTuanClient.getInstance().GetJunTuanGVoicePrioritys(faction);
								this.RuntimeData.JunTuanGVoiceDict[junTuanId] = text;
							}
							client.ClientData.GVoicePrioritys = text;
							gvoicePriorityData = new GVoicePriorityData
							{
								ID = junTuanId,
								Type = num,
								RoleIdList = text
							};
						}
						int num2;
						this.RuntimeData.MapCode2GVoiceGroupDict.TryGetValue(mapCode, out num2);
						string key;
						if (mapType == MapTypes.Normal)
						{
							key = string.Format("{0}_{1}_{2}", GameManager.ServerId, num2, gvoicePriorityData.ID);
						}
						else
						{
							key = string.Format("{0}_{1}_{2}_{3}", new object[]
							{
								GameManager.ServerId,
								num2,
								client.ClientData.FuBenSeqID,
								gvoicePriorityData.ID
							});
						}
						GVoiceSceneData gvoiceSceneData2;
						if (!this.RuntimeData.FuBenSeqID2RoomName.TryGetValue(key, out gvoiceSceneData2))
						{
							gvoiceSceneData2 = new GVoiceSceneData();
							gvoiceSceneData2.RoomName = Guid.NewGuid().ToString("N");
							this.RuntimeData.FuBenSeqID2RoomName[key] = gvoiceSceneData2;
						}
						gvoiceSceneData.RoomName = gvoiceSceneData2.RoomName;
						gvoiceSceneData.SDKGameID = this.RuntimeData.SDKGameID;
						gvoiceSceneData.SDKKey = this.RuntimeData.SDKKey;
					}
				}
				if (gvoicePriorityData != null)
				{
					client.sendCmd<GVoicePriorityData>(1112, gvoicePriorityData, false);
				}
				client.sendCmd<GVoiceSceneData>(nID, gvoiceSceneData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGVoiceSetRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				string text = cmdParams[2];
				int faction = client.ClientData.Faction;
				int roleID = client.ClientData.RoleID;
				string b = roleID.ToString();
				HashSet<string> hashSet = new HashSet<string>();
				if (!string.IsNullOrEmpty(text))
				{
					foreach (string text2 in text.Split(new char[]
					{
						','
					}))
					{
						if (text2 != b)
						{
							hashSet.Add(text2);
						}
					}
				}
				text = string.Join(",", hashSet);
				if (num == 1)
				{
					if (faction <= 0 || client.ClientData.BHZhiWu != 1)
					{
						cmdData = -1002;
					}
					else if (hashSet.Count >= this.RuntimeData.VoicePowerNum[0])
					{
						cmdData = -1035;
					}
					else
					{
						Global.sendToDB<string, string>(1111, string.Format("{0}:{1}:{2}", roleID, faction, text), client.ServerId);
						GMCmdData gmcmdData = new GMCmdData
						{
							Fields = new string[]
							{
								"-gvoicepriority",
								1.ToString(),
								faction.ToString(),
								text
							}
						};
						HuanYingSiYuanClient.getInstance().BroadcastGMCmdData(gmcmdData, 1);
						this.UpdateGVoicePriority(gmcmdData, true);
					}
				}
				else if (num == 2)
				{
					if (client.ClientData.BHZhiWu != 1 || (client.ClientData.JunTuanId <= 0 && client.ClientData.JunTuanZhiWu == 1))
					{
						cmdData = -1024;
					}
					else if (hashSet.Count >= this.RuntimeData.VoicePowerNum[1])
					{
						cmdData = -1035;
					}
					else
					{
						cmdData = JunTuanClient.getInstance().ChangeJunTuanGVoicePrioritys(faction, text);
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

		public bool ProcessGVoiceGetRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int roleID = client.ClientData.RoleID;
				GVoicePriorityData gvoicePriorityData = new GVoicePriorityData();
				gvoicePriorityData.Type = num;
				string text = "";
				lock (this.RuntimeData.Mutex)
				{
					if (num == 1)
					{
						gvoicePriorityData.ID = faction;
						if (faction > 0)
						{
							if (!this.RuntimeData.ZhanMengGVoiceDict.TryGetValue(faction, out text))
							{
								text = Global.sendToDB<string, int>(1112, faction, client.ServerId);
								this.RuntimeData.ZhanMengGVoiceDict[faction] = text;
							}
							client.ClientData.GVoicePrioritys = text;
						}
					}
					else if (num == 2)
					{
						gvoicePriorityData.ID = client.ClientData.JunTuanId;
						if (client.ClientData.JunTuanId > 0)
						{
							if (!this.RuntimeData.JunTuanGVoiceDict.TryGetValue(junTuanId, out text))
							{
								text = JunTuanClient.getInstance().GetJunTuanGVoicePrioritys(faction);
								this.RuntimeData.JunTuanGVoiceDict[junTuanId] = text;
							}
							client.ClientData.GVoicePrioritys = text;
						}
					}
				}
				gvoicePriorityData.RoleIdList = text;
				client.sendCmd<GVoicePriorityData>(nID, gvoicePriorityData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameFuncControlManager.IsGameFuncDisabled(15) || Global.GetUnionLevel(client, false) >= Global.GetUnionLevel(this.RuntimeData.VoiceMessage[0], this.RuntimeData.VoiceMessage[1], false) || client.ClientData.VipLevel >= this.RuntimeData.VoiceMessage[2];
		}

		public void OnStartPlayGame(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			lock (this.RuntimeData.Mutex)
			{
				int gvoiceType;
				if (!this.RuntimeData.MapCode2GVoiceTypeDict.TryGetValue(mapCode, out gvoiceType))
				{
					client.ClientData.GVoiceType = 0;
				}
				else
				{
					client.ClientData.GVoiceType = gvoiceType;
				}
			}
		}

		public void UpdateGVoicePriority(GMCmdData cmdData, bool force = true)
		{
			if (cmdData.Fields.Length >= 4)
			{
				if (!(cmdData.Fields[0] != "-gvoicepriority"))
				{
					int num = Global.SafeConvertToInt32(cmdData.Fields[1]);
					int num2 = Global.SafeConvertToInt32(cmdData.Fields[2]);
					string text = cmdData.Fields[3];
					lock (this.RuntimeData.Mutex)
					{
						if (num == 1)
						{
							this.RuntimeData.ZhanMengGVoiceDict[num2] = text;
						}
						else if (num == 2)
						{
							this.RuntimeData.JunTuanGVoiceDict[num2] = text;
						}
					}
					GVoicePriorityData cmdData2 = new GVoicePriorityData
					{
						ID = num2,
						Type = num,
						RoleIdList = text
					};
					if (num == 1)
					{
						foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
						{
							if (force || gameClient.ClientData.GVoiceType == 1)
							{
								if (gameClient.ClientData.Faction == num2 && gameClient.ClientData.GVoicePrioritys != text)
								{
									gameClient.ClientData.GVoicePrioritys = text;
									gameClient.sendCmd<GVoicePriorityData>(1112, cmdData2, false);
								}
							}
						}
					}
					else if (num == 2)
					{
						foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
						{
							if (force || gameClient.ClientData.GVoiceType == 2)
							{
								if (gameClient.ClientData.JunTuanId == num2 && gameClient.ClientData.GVoicePrioritys != text)
								{
									gameClient.ClientData.GVoicePrioritys = text;
									gameClient.sendCmd<GVoicePriorityData>(1112, cmdData2, false);
								}
							}
						}
					}
				}
			}
		}

		private void TimerProc(object sender, EventArgs e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long num = TimeUtil.NOW();
			if (this.RuntimeData.NextTicks1 < num)
			{
				this.RuntimeData.NextTicks1 = num + 3000L;
			}
		}

		private static GVoiceManager instance = new GVoiceManager();

		public GVoiceRuntimeData RuntimeData = new GVoiceRuntimeData();
	}
}
