using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class KuaFuMapManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		public static KuaFuMapManager getInstance()
		{
			return KuaFuMapManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuBossManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1141, 2, 4, KuaFuMapManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1140, 1, 1, KuaFuMapManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1140:
				result = this.ProcessGetKuaFuLineDataListCmd(client, nID, bytes, cmdParams);
				break;
			case 1141:
				result = this.ProcessKuaFuMapEnterCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.LineMap2KuaFuLineDataDict.Clear();
					this.RuntimeData.ServerMap2KuaFuLineDataDict.Clear();
					this.RuntimeData.KuaFuMapServerIdDict.Clear();
					this.RuntimeData.MapCode2KuaFuLineDataDict.Clear();
					text = "Config/MapLine.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						int maxOnlineCount = (int)Global.GetSafeAttributeLong(xelement2, "MaxNum");
						int mapType = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Type", 0L);
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Line");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array)
							{
								KuaFuLineData kuaFuLineData = new KuaFuLineData();
								string[] array3 = text2.Split(new char[]
								{
									','
								});
								kuaFuLineData.Line = int.Parse(array3[0]);
								kuaFuLineData.MapCode = int.Parse(array3[1]);
								if (array3.Length >= 3)
								{
									kuaFuLineData.ServerId = int.Parse(array3[2]);
								}
								kuaFuLineData.MapType = mapType;
								kuaFuLineData.MaxOnlineCount = maxOnlineCount;
								this.RuntimeData.LineMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.Line, kuaFuLineData.MapCode), kuaFuLineData);
								List<KuaFuLineData> list = null;
								if (kuaFuLineData.ServerId > 0)
								{
									if (this.RuntimeData.ServerMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.ServerId, kuaFuLineData.MapCode), kuaFuLineData))
									{
										if (!this.RuntimeData.KuaFuMapServerIdDict.TryGetValue(kuaFuLineData.ServerId, out list))
										{
											list = new List<KuaFuLineData>();
											this.RuntimeData.KuaFuMapServerIdDict.TryAdd(kuaFuLineData.ServerId, list);
										}
										list.Add(kuaFuLineData);
									}
								}
								if (!this.RuntimeData.MapCode2KuaFuLineDataDict.TryGetValue(kuaFuLineData.MapCode, out list))
								{
									list = new List<KuaFuLineData>();
									this.RuntimeData.MapCode2KuaFuLineDataDict.TryAdd(kuaFuLineData.MapCode, list);
								}
								list.Add(kuaFuLineData);
							}
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

		public bool IsKuaFuMap(int mapCode)
		{
			return this.RuntimeData.MapCode2KuaFuLineDataDict.ContainsKey(mapCode);
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == null || 48 == mapSceneType || 54 == mapSceneType;
		}

		public bool ProcessGetKuaFuLineDataListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int mapCode = Global.SafeConvertToInt32(cmdParams[0]);
				if (Global.GetMapSceneType(mapCode) == 54)
				{
					List<KuaFuLineData> cmdData = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
					client.sendCmd<List<KuaFuLineData>>(nID, cmdData, false);
				}
				else
				{
					List<KuaFuLineData> cmdData = YongZheZhanChangClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
					client.sendCmd<List<KuaFuLineData>>(nID, cmdData, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKuaFuMapEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int num4 = 0;
				int num5 = 0;
				if (cmdParams.Length >= 3)
				{
					num4 = Global.SafeConvertToInt32(cmdParams[2]);
				}
				if (cmdParams.Length >= 4)
				{
					num5 = Global.SafeConvertToInt32(cmdParams[3]);
				}
				KuaFuLineData kuaFuLineData;
				if (!KuaFuMapManager.getInstance().IsKuaFuMap(num2))
				{
					num = -12;
				}
				else if (!this.RuntimeData.LineMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(num3, num2), out kuaFuLineData))
				{
					num = -12;
				}
				else if (!Global.CanEnterMap(client, num2) || (num2 == client.ClientData.MapCode && kuaFuLineData.MapType != 1))
				{
					num = -12;
				}
				else
				{
					if (num2 == client.ClientData.MapCode && kuaFuLineData.MapType == 1)
					{
						List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(num2) as List<KuaFuLineData>;
						if (null == list)
						{
							num = -12;
							goto IL_67F;
						}
						KuaFuLineData kuaFuLineData2 = list.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
						if (kuaFuLineData2 != null && kuaFuLineData2.Line == kuaFuLineData.Line)
						{
							num = -4011;
							goto IL_67F;
						}
					}
					if (!KuaFuMapManager.getInstance().IsKuaFuMap(client.ClientData.MapCode) && !this.CheckMap(client))
					{
						num = -21;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						num = -12;
					}
					else if (kuaFuLineData.OnlineCount >= kuaFuLineData.MaxOnlineCount)
					{
						num = -100;
					}
					else
					{
						int mapCode = client.ClientData.MapCode;
						if (num5 > 0)
						{
							GameMap gameMap = null;
							if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
							{
								num = -3;
								goto IL_67F;
							}
							MapTeleport mapTeleport = null;
							if (!gameMap.MapTeleportDict.TryGetValue(num5, out mapTeleport) || mapTeleport.ToMapID != num2)
							{
								num = -12;
								goto IL_67F;
							}
							if (Global.GetTwoPointDistance(client.CurrentPos, new Point((double)mapTeleport.X, (double)mapTeleport.Y)) > 800.0)
							{
								num = -301;
								goto IL_67F;
							}
						}
						KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						int num7;
						if (kuaFuLineData.MapType == 1)
						{
							if (!GlobalNew.IsGongNengOpened(client, 105, true))
							{
								num = -400;
								goto IL_67F;
							}
							string signToken;
							string str;
							int num6 = KuaFuWorldClient.getInstance().EnterPTKuaFuMap(client.ServerId, client.ClientData.LocalRoleID, client.ClientData.ServerPTID, kuaFuLineData.MapCode, kuaFuLineData.Line, clientKuaFuServerLoginData, out signToken, out str);
							if (num6 == -4010)
							{
								KuaFuWorldRoleData kuaFuWorldRoleData = new KuaFuWorldRoleData
								{
									LocalRoleID = client.ClientData.LocalRoleID,
									UserID = client.strUserID,
									WorldRoleID = client.ClientData.WorldRoleID,
									Channel = client.ClientData.Channel,
									PTID = client.ClientData.ServerPTID,
									ServerID = client.ServerId,
									ZoneID = client.ClientData.ZoneID
								};
								num6 = KuaFuWorldClient.getInstance().RegPTKuaFuRoleData(ref kuaFuWorldRoleData);
								num6 = KuaFuWorldClient.getInstance().EnterPTKuaFuMap(client.ServerId, client.ClientData.LocalRoleID, client.ClientData.ServerPTID, kuaFuLineData.MapCode, kuaFuLineData.Line, clientKuaFuServerLoginData, out signToken, out str);
							}
							if (num6 < 0)
							{
								num = num6;
								goto IL_67F;
							}
							KFRebornRoleData kfrebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
							if (null == kfrebornRoleData)
							{
								num = KuaFuWorldClient.getInstance().Reborn_RoleReborn(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, client.ClientData.RoleName, client.ClientData.RebornLevel);
								if (num < 0)
								{
									goto IL_67F;
								}
								LogManager.WriteLog(5, string.Format("Reborn_RoleReborn ptId={0} roleId={1} roleName={2} rebornLevel={3}", new object[]
								{
									client.ClientData.ServerPTID,
									client.ClientData.LocalRoleID,
									client.ClientData.RoleName,
									client.ClientData.RebornLevel
								}), null, true);
							}
							clientKuaFuServerLoginData.PTID = client.ClientData.ServerPTID;
							clientKuaFuServerLoginData.RoleId = client.ClientData.LocalRoleID;
							clientKuaFuServerLoginData.SignToken = signToken;
							clientKuaFuServerLoginData.TempRoleID = num6;
							clientKuaFuServerLoginData.SignCode = MD5Helper.get_md5_string(clientKuaFuServerLoginData.SignDataString() + str).ToLower();
							num7 = clientKuaFuServerLoginData.TargetServerID;
						}
						else
						{
							clientKuaFuServerLoginData.SignCode = null;
							num7 = YongZheZhanChangClient.getInstance().EnterKuaFuMap(client.ClientData.LocalRoleID, kuaFuLineData.MapCode, kuaFuLineData.Line, client.ServerId, Global.GetClientKuaFuServerLoginData(client));
						}
						clientKuaFuServerLoginData.Line = num3;
						if (num7 > 0)
						{
							bool flag = 0 == 0;
							int num8 = (num5 > 0) ? 0 : Global.GetMapTransNeedMoney(num2);
							if (Global.GetTotalBindTongQianAndTongQianVal(client) < num8)
							{
								GameManager.ClientMgr.NotifyImportantMsg(client, StringUtil.substitute(GLang.GetLang(171, new object[0]), new object[]
								{
									num8,
									Global.GetMapName(num2)
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
								num = -9;
								Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							}
							else
							{
								int[] array = new int[5];
								array[0] = mapCode;
								array[1] = num5;
								array[2] = num4;
								Global.SaveRoleParamsIntListToDB(client, new List<int>(array), "EnterKuaFuMapFlag", true);
								GlobalNew.RecordSwitchKuaFuServerLog(client);
								client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
							}
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							num = num7;
						}
					}
				}
				IL_67F:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			client.ClientData.MapCode = (int)clientKuaFuServerLoginData.GameId;
			client.ClientData.PosX = 0;
			client.ClientData.PosY = 0;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count >= 5)
			{
				int num = roleParamsIntListFromDB[0];
				int num2 = roleParamsIntListFromDB[1];
				int num3 = roleParamsIntListFromDB[2];
				if (num > 0 && num2 > 0)
				{
					GameMap gameMap = null;
					MapTeleport mapTeleport = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(num, out gameMap) && gameMap.MapTeleportDict.TryGetValue(num2, out mapTeleport))
					{
						GameMap gameMap2 = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(mapTeleport.ToMapID, out gameMap2) && gameMap2.CanMove(mapTeleport.ToX / gameMap2.MapGridWidth, mapTeleport.ToY / gameMap2.MapGridHeight))
						{
							client.ClientData.MapCode = mapTeleport.ToMapID;
							client.ClientData.PosX = mapTeleport.ToX;
							client.ClientData.PosY = mapTeleport.ToY;
						}
					}
				}
				if (num3 > 0)
				{
					Global.ProcessVipLevelUp(client);
					if (Global.IsVip(client) && (long)client.ClientData.VipLevel >= GameManager.systemParamsList.GetParamValueIntByName("VIPBossChuanSong", 4))
					{
						int toX;
						int toY;
						int radius;
						if (GameManager.MonsterZoneMgr.GetMonsterBirthPoint(client.ClientData.MapCode, num3, out toX, out toY, out radius))
						{
							radius = 1;
							Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, toX, toY, radius);
							client.ClientData.PosX = (int)mapPoint.X;
							client.ClientData.PosY = (int)mapPoint.Y;
						}
					}
				}
			}
			return true;
		}

		public void OnStartPlayGame(GameClient client)
		{
			bool flag = false;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count >= 5 && roleParamsIntListFromDB[1] > 0)
			{
				flag = true;
			}
			if (!flag)
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				int mapCode = (int)clientKuaFuServerLoginData.GameId;
				int mapTransNeedMoney = Global.GetMapTransNeedMoney(mapCode);
				if (mapTransNeedMoney > 0 && Global.SubBindTongQianAndTongQian(client, mapTransNeedMoney, "地图传送"))
				{
					GameManager.ClientMgr.NotifyImportantMsg(client, StringUtil.substitute(GLang.GetLang(172, new object[0]), new object[]
					{
						mapTransNeedMoney,
						Global.GetMapName(mapCode)
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
			Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
			{
				roleParamsIntListFromDB[0],
				0,
				0,
				0,
				0
			}), "EnterKuaFuMapFlag", true);
		}

		public void TimerProc(object sender, EventArgs e)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (KuaFuManager.getInstance().CanKuaFuLogin())
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (int num in this.RuntimeData.MapCode2KuaFuLineDataDict.Keys)
					{
						dictionary[num] = 0;
					}
				}
			}
			List<int> list = dictionary.Keys.ToList<int>();
			foreach (int num in list)
			{
				dictionary[num] = GameManager.ClientMgr.GetMapClientsCount(num);
			}
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				KuaFuWorldClient.getInstance().UpdateKuaFuMapClientCount(dictionary);
			}
			else
			{
				YongZheZhanChangClient.getInstance().UpdateKuaFuMapClientCount(dictionary);
			}
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(7) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KuaFuMap");
		}

		public const SceneUIClasses ManagerType = 10003;

		private static KuaFuMapManager instance = new KuaFuMapManager();

		public KuaFuMapData RuntimeData = new KuaFuMapData();
	}
}
