using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Ornament
{
	public class OrnamentManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static OrnamentManager getInstance()
		{
			return OrnamentManager.instance;
		}

		public bool initialize()
		{
			this.evHandlerDict = new Dictionary<OrnamentGoalType, Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>>();
			this.evHandlerDict[OrnamentGoalType.OGT_Talent] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_Talent);
			this.evHandlerDict[OrnamentGoalType.OGT_KingOfBattle] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_KingOfBattle);
			this.evHandlerDict[OrnamentGoalType.OGT_YongZheZhanChang] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_YongZheZhanChang);
			this.evHandlerDict[OrnamentGoalType.OGT_HuanYingSiYuan] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_HuanYingSiYuan);
			this.evHandlerDict[OrnamentGoalType.OGT_JingJiChallenge] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_JingJiChallenge);
			this.evHandlerDict[OrnamentGoalType.OGT_BHMatchGoldChampion] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_BHMatchGoldChampion);
			this.evHandlerDict[OrnamentGoalType.OGT_BHMatchJoin] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_BHMatchJoin);
			this.evHandlerDict[OrnamentGoalType.OGT_BHMatchGoldMVP] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_BHMatchGoldMVP);
			this.evHandlerDict[OrnamentGoalType.OGT_BHMatchWin] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_BHMatchWin);
			this.evHandlerDict[OrnamentGoalType.OGT_KingOfBattleMVP] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_KingOfBattleMVP);
			this.evHandlerDict[OrnamentGoalType.OGT_YongZheZhanChangMVP] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_YongZheZhanChangMVP);
			this.evHandlerDict[OrnamentGoalType.OGT_TianTiPT] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_TianTiPT);
			this.evHandlerDict[OrnamentGoalType.OGT_TianTiDiamond] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_TianTiDiamond);
			this.evHandlerDict[OrnamentGoalType.OGT_CoupleArenaDuanWei] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_CoupleArenaDuanWei);
			this.evHandlerDict[OrnamentGoalType.OGT_KuaFuLueDuo_Attacker] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_GoalAddNum);
			this.evHandlerDict[OrnamentGoalType.OGT_KuaFuLueDuo_Defender] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_GoalAddNum);
			this.evHandlerDict[OrnamentGoalType.OGT_EscapeRoleKill] = new Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>(this._Handle_EscapeKillRole);
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1615, 1, 1, OrnamentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1616, 2, 2, OrnamentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1617, 3, 3, OrnamentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1618, 1, 1, OrnamentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(37, OrnamentManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, OrnamentManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(37, OrnamentManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, OrnamentManager.getInstance());
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
			if (!GlobalNew.IsGongNengOpened(client, 83, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(512, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1615:
					result = this.ProcessOrnamentGetDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1616:
					result = this.ProcessOrnamentSlotForgeCmd(client, nID, bytes, cmdParams);
					break;
				case 1617:
					result = this.ProcessOrnamentActiveCmd(client, nID, bytes, cmdParams);
					break;
				case 1618:
					result = this.ProcessOrnamentGetGoodsListCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public void OnLogin(GameClient client)
		{
			bool flag = GameFuncControlManager.IsGameFuncDisabled(11);
			bool flag2 = GlobalNew.IsGongNengOpened(client, 83, false);
			if (flag2 && !flag)
			{
				GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 23);
				if (goodsDataByCategoriy != null && goodsDataByCategoriy.Site == 0)
				{
					if (!Global.CanAddGoods(client, goodsDataByCategoriy.GoodsID, 1, goodsDataByCategoriy.Binding, "1900-01-01 12:00:00", true, false))
					{
						if (Global.UseMailGivePlayerAward(client, goodsDataByCategoriy, GLang.GetLang(513, new object[0]), GLang.GetLang(514, new object[0]), 1.0))
						{
							Global.DestroyGoods(client, goodsDataByCategoriy);
						}
					}
					else
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							2,
							goodsDataByCategoriy.Id,
							goodsDataByCategoriy.GoodsID,
							0,
							goodsDataByCategoriy.Site,
							goodsDataByCategoriy.GCount,
							goodsDataByCategoriy.BagIndex,
							""
						});
						Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
					}
				}
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_Talent, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchJoin, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchGoldMVP, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchWin, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_EscapeRoleKill, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_TianTiPT, new int[0]));
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_TianTiDiamond, new int[0]));
				this.InitOrnamentSlotData(client);
				this.RefreshOrnamentProps(client);
				this.HandleBHMatchGoldAccident(client);
			}
		}

		private void HandleBHMatchGoldAccident(GameClient client)
		{
			Dictionary<int, OrnamentData> ornamentDataDict = client.ClientData.OrnamentDataDict;
			lock (ornamentDataDict)
			{
				List<int> list = null;
				Dictionary<int, OrnamentConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					if (!this.Func2GoalId.TryGetValue(OrnamentGoalType.OGT_BHMatchGoldChampion, out list) || list.Count <= 0)
					{
						return;
					}
					if ((dictionary = this.OrnamentConfig) == null || dictionary.Count <= 0)
					{
						return;
					}
				}
				foreach (int key in list)
				{
					OrnamentData ornamentData = null;
					if (ornamentDataDict.TryGetValue(key, out ornamentData))
					{
						OrnamentConfigData ornamentConfigData = null;
						if (dictionary.TryGetValue(key, out ornamentConfigData))
						{
							List<int> bhmatchRAnalysisExData = BangHuiMatchManager.getInstance().GetBHMatchRAnalysisExData(client);
							if (ornamentData.Param1 > bhmatchRAnalysisExData[0])
							{
								if (ornamentData.Param1 >= ornamentConfigData.GoalNum)
								{
									GoodsData ornamentGoodsDataByGoodsID = this.GetOrnamentGoodsDataByGoodsID(client, ornamentData.ID);
									if (null != ornamentGoodsDataByGoodsID)
									{
										Global.DestroyGoods(client, ornamentGoodsDataByGoodsID);
									}
								}
								ornamentData.Param1 = bhmatchRAnalysisExData[0];
								this.UpdateDb(client.ClientData.RoleID, ornamentData, client.ServerId);
							}
						}
					}
				}
			}
		}

		public int GetOrnamentCharmPoint(GoodsData goodsData)
		{
			bool flag = GameFuncControlManager.IsGameFuncDisabled(11);
			int result;
			if (flag)
			{
				result = 0;
			}
			else if (null == goodsData)
			{
				result = 0;
			}
			else if (goodsData.GCount <= 0)
			{
				result = 0;
			}
			else
			{
				Dictionary<int, OrnamentConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.OrnamentConfig;
				}
				OrnamentConfigData ornamentConfigData = null;
				if (!dictionary.TryGetValue(goodsData.GoodsID, out ornamentConfigData))
				{
					result = 0;
				}
				else
				{
					result = ornamentConfigData.RecoverPoints * goodsData.GCount;
				}
			}
			return result;
		}

		private void InitOrnamentSlotData(GameClient client)
		{
			OrnamentData ornamentData = null;
			if (!client.ClientData.OrnamentDataDict.TryGetValue(1, out ornamentData))
			{
				ornamentData = new OrnamentData();
				ornamentData.ID = 1;
				ornamentData.Param1 = 1;
				client.ClientData.OrnamentDataDict[ornamentData.ID] = ornamentData;
				this.UpdateDb(client.ClientData.RoleID, ornamentData, client.ServerId);
			}
		}

		private int CalcOrnamentSlotForgeTotalLev(GameClient client)
		{
			int num = 0;
			foreach (KeyValuePair<int, OrnamentData> keyValuePair in client.ClientData.OrnamentDataDict)
			{
				if (keyValuePair.Value.ID > 0 && keyValuePair.Value.ID < 6)
				{
					num += keyValuePair.Value.Param1;
				}
			}
			return num;
		}

		private void TryActiveOrnamentSlot(GameClient client)
		{
			int num = this.CalcOrnamentSlotForgeTotalLev(client);
			List<int> list = null;
			lock (this.ConfigMutex)
			{
				list = this.OrnamentSlotOpenConfig;
			}
			int num2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (num >= list[i])
				{
					num2 = i + 1;
				}
			}
			if (num2 > 0 && num2 < 6)
			{
				OrnamentData ornamentData = null;
				if (!client.ClientData.OrnamentDataDict.TryGetValue(num2, out ornamentData))
				{
					ornamentData = new OrnamentData();
					ornamentData.ID = num2;
					ornamentData.Param1 = 1;
					client.ClientData.OrnamentDataDict[ornamentData.ID] = ornamentData;
					this.UpdateDb(client.ClientData.RoleID, ornamentData, client.ServerId);
				}
			}
		}

		private bool UpdateDb(int roleid, OrnamentData itemData, int serverId)
		{
			bool result;
			if (!Global.sendToDB<bool, OrnamentUpdateDbData>(13223, new OrnamentUpdateDbData
			{
				RoleId = roleid,
				Data = itemData
			}, serverId))
			{
				LogManager.WriteLog(2, string.Format("饰品系统更新玩家数据失败, roleid={0}, id={1}", roleid, itemData.ID), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public int _CanUsingOrnament(GameClient client, int toBagIndex, List<GoodsData> usingList)
		{
			int num = 0;
			while (usingList != null && num < usingList.Count)
			{
				if (toBagIndex == usingList[num].BagIndex)
				{
					return -4;
				}
				num++;
			}
			if (toBagIndex <= 0 || toBagIndex >= 6)
			{
				return -55;
			}
			OrnamentData ornamentData = null;
			if (!client.ClientData.OrnamentDataDict.TryGetValue(toBagIndex, out ornamentData))
			{
				return -55;
			}
			return 0;
		}

		public GoodsData GetOrnamentGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.OrnamentGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.OrnamentGoodsDataList.Count; i++)
					{
						if (client.ClientData.OrnamentGoodsDataList[i].Id == id)
						{
							return client.ClientData.OrnamentGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public GoodsData GetOrnamentGoodsDataByGoodsID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.OrnamentGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.OrnamentGoodsDataList.Count; i++)
					{
						if (client.ClientData.OrnamentGoodsDataList[i].GoodsID == id)
						{
							return client.ClientData.OrnamentGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public GoodsData AddOrnamentGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			this.AddOrnamentGoodsData(client, goodsData);
			return goodsData;
		}

		public void AddOrnamentGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 9000)
			{
				if (null == client.ClientData.OrnamentGoodsDataList)
				{
					client.ClientData.OrnamentGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					client.ClientData.OrnamentGoodsDataList.Add(goodsData);
				}
				this.RefreshOrnamentProps(client);
			}
		}

		public bool OrnamentCanSaleBack(GameClient client, int GoodsID)
		{
			bool flag = GameFuncControlManager.IsGameFuncDisabled(11);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(GoodsID);
				if (goodsCatetoriy != 23)
				{
					result = true;
				}
				else if (client.ClientData.OrnamentGoodsDataList == null)
				{
					result = false;
				}
				else
				{
					List<GoodsData> list;
					lock (client.ClientData.OrnamentGoodsDataList)
					{
						list = new List<GoodsData>(client.ClientData.OrnamentGoodsDataList);
					}
					foreach (GoodsData goodsData in list)
					{
						if (goodsData.GoodsID == GoodsID)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public bool OrnamentCanAdd(GameClient client, int GoodsID)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(GoodsID);
			bool result;
			if (goodsCatetoriy != 23)
			{
				result = false;
			}
			else if (client.ClientData.OrnamentGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				List<GoodsData> list;
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					list = new List<GoodsData>(client.ClientData.OrnamentGoodsDataList);
				}
				foreach (GoodsData goodsData in list)
				{
					if (goodsData.GoodsID == GoodsID)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public void RemoveOrnamentGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.OrnamentGoodsDataList)
			{
				if (null != goodsData)
				{
					lock (client.ClientData.OrnamentGoodsDataList)
					{
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
						if (null != equipPropItem)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.OrnamentGoodsProps,
								goodsData.GoodsID,
								PropsCacheManager.ConstExtProps
							});
						}
						client.ClientData.OrnamentGoodsDataList.Remove(goodsData);
					}
					this.RefreshOrnamentProps(client);
				}
			}
		}

		public void RefreshOrnamentProps(GameClient client)
		{
			lock (client.ClientData.OrnamentGoodsDataList)
			{
				foreach (GoodsData goodsData in client.ClientData.OrnamentGoodsDataList)
				{
					GoodsData goodsData;
					EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
					if (null != equipPropItem)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.OrnamentGoodsProps,
							goodsData.GoodsID,
							PropsCacheManager.ConstExtProps
						});
					}
				}
				foreach (GoodsData goodsData in client.ClientData.OrnamentGoodsDataList)
				{
					GoodsData goodsData;
					EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
					if (null != equipPropItem)
					{
						double[] array = new double[177];
						if (goodsData.Using <= 0)
						{
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = equipPropItem.ExtProps[i];
							}
						}
						else
						{
							OrnamentData ornamentData = null;
							if (!client.ClientData.OrnamentDataDict.TryGetValue(goodsData.BagIndex, out ornamentData))
							{
								continue;
							}
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = ((double)ornamentData.Param1 * 0.2 + 0.8) * equipPropItem.ExtProps[i];
							}
						}
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.OrnamentGoodsProps,
							goodsData.GoodsID,
							array
						});
					}
				}
				List<OrnamentGroupConfigData> list = null;
				lock (this.ConfigMutex)
				{
					list = this.OrnamentGroupConfig;
				}
				foreach (OrnamentGroupConfigData ornamentGroupConfigData in list)
				{
					OrnamentGroupConfigData ornamentGroupConfigData2 = ornamentGroupConfigData;
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.OrnamentGroupProps,
						ornamentGroupConfigData2.ID,
						PropsCacheManager.ConstExtProps
					});
				}
				foreach (OrnamentGroupConfigData ornamentGroupConfigData in list)
				{
					bool flag3 = true;
					OrnamentGroupConfigData ornamentGroupConfigData2 = ornamentGroupConfigData;
					for (int i = 0; i < ornamentGroupConfigData2.GoodsIDList.Count; i++)
					{
						GoodsData goodsData = this.GetOrnamentGoodsDataByGoodsID(client, ornamentGroupConfigData2.GoodsIDList[i]);
						if (goodsData == null || goodsData.Using <= 0)
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.OrnamentGroupProps,
							ornamentGroupConfigData2.ID,
							ornamentGroupConfigData2.ExtProps
						});
					}
				}
			}
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
		}

		private bool CheckCanActiveChengJiuOrnament(GameClient client, OrnamentData data, OrnamentConfigData itemConfig)
		{
			bool result;
			switch (itemConfig.GoalType)
			{
			case OrnamentGoalType.OGT_Talent:
			case OrnamentGoalType.OGT_BHMatchJoin:
			case OrnamentGoalType.OGT_BHMatchGoldMVP:
			case OrnamentGoalType.OGT_BHMatchWin:
			case OrnamentGoalType.OGT_EscapeRoleKill:
				result = (data.Param1 >= itemConfig.GoalNum);
				break;
			case OrnamentGoalType.OGT_KingOfBattle:
			case OrnamentGoalType.OGT_YongZheZhanChang:
			case OrnamentGoalType.OGT_HuanYingSiYuan:
			case OrnamentGoalType.OGT_JingJiChallenge:
			case OrnamentGoalType.OGT_BHMatchGoldChampion:
			case OrnamentGoalType.OGT_KingOfBattleMVP:
			case OrnamentGoalType.OGT_YongZheZhanChangMVP:
			case OrnamentGoalType.OGT_TianTiPT:
			case OrnamentGoalType.OGT_TianTiDiamond:
			case OrnamentGoalType.OGT_CoupleArenaDuanWei:
			case OrnamentGoalType.OGT_KuaFuLueDuo_Defender:
			case OrnamentGoalType.OGT_KuaFuLueDuo_Attacker:
				result = (data.Param1 >= itemConfig.GoalNum);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 14)
			{
				GameClient player = (eventObject as PlayerInitGameEventObject).getPlayer();
				this.OnLogin(player);
			}
			else if (eventObject.getEventType() == 37)
			{
				OrnamentGoalEventObject ornamentGoalEventObject = eventObject as OrnamentGoalEventObject;
				try
				{
					bool flag = GameFuncControlManager.IsGameFuncDisabled(11);
					bool flag2 = GlobalNew.IsGongNengOpened(ornamentGoalEventObject.Client, 83, false);
					if (flag2 && !flag)
					{
						Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>> action = null;
						if (this.evHandlerDict.TryGetValue(ornamentGoalEventObject.FuncType, out action))
						{
							List<int> list = null;
							Dictionary<int, OrnamentConfigData> dictionary = null;
							lock (this.ConfigMutex)
							{
								if (!this.Func2GoalId.TryGetValue(ornamentGoalEventObject.FuncType, out list) || list.Count <= 0)
								{
									return;
								}
								if ((dictionary = this.OrnamentConfig) == null || dictionary.Count <= 0)
								{
									return;
								}
							}
							action(ornamentGoalEventObject, list, dictionary);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, "OrnamentManager.processEvent [OrnamentGoal]", ex, true);
				}
			}
		}

		private void _Handle_Talent(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						ornamentData.Param1 = evObj.Client.ClientData.MyTalentData.TotalCount;
					}
				}
			}
		}

		private void _Handle_GoalAddNum(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1 += evObj.Arg1;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_KingOfBattle(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_YongZheZhanChang(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_HuanYingSiYuan(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_JingJiChallenge(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_BHMatchGoldChampion(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_BHMatchJoin(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						List<int> bhmatchRoleAnalysisData = BangHuiMatchManager.getInstance().GetBHMatchRoleAnalysisData(evObj.Client);
						if (null != bhmatchRoleAnalysisData)
						{
							ornamentData.Param1 = bhmatchRoleAnalysisData[11];
						}
					}
				}
			}
		}

		private void _Handle_EscapeKillRole(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						List<int> escapeBattleRoleAnalysisData = EscapeBattleManager.getInstance().GetEscapeBattleRoleAnalysisData(evObj.Client);
						if (null != escapeBattleRoleAnalysisData)
						{
							ornamentData.Param1 = escapeBattleRoleAnalysisData[2];
						}
					}
				}
			}
		}

		private void _Handle_BHMatchGoldMVP(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						List<int> bhmatchRoleAnalysisData = BangHuiMatchManager.getInstance().GetBHMatchRoleAnalysisData(evObj.Client);
						if (null != bhmatchRoleAnalysisData)
						{
							ornamentData.Param1 = bhmatchRoleAnalysisData[2];
						}
					}
				}
			}
		}

		private void _Handle_BHMatchWin(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						List<int> bhmatchRoleAnalysisData = BangHuiMatchManager.getInstance().GetBHMatchRoleAnalysisData(evObj.Client);
						if (null != bhmatchRoleAnalysisData)
						{
							ornamentData.Param1 = bhmatchRoleAnalysisData[10];
						}
					}
				}
			}
		}

		private void _Handle_KingOfBattleMVP(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_YongZheZhanChangMVP(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						OrnamentConfigData ornamentConfigData = null;
						if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
						{
							if (ornamentData.Param1 < ornamentConfigData.GoalNum)
							{
								ornamentData.Param1++;
								if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
								{
									ornamentData.Param1--;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_TianTiPT(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						if (evObj.Client.ClientData.TianTiData.DuanWeiId >= 20)
						{
							OrnamentConfigData ornamentConfigData = null;
							if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
							{
								if (ornamentData.Param1 < ornamentConfigData.GoalNum)
								{
									ornamentData.Param1 = ornamentConfigData.GoalNum;
									if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
									{
										ornamentData.Param1 = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_TianTiDiamond(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						if (evObj.Client.ClientData.TianTiData.DuanWeiId >= 25)
						{
							OrnamentConfigData ornamentConfigData = null;
							if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
							{
								if (ornamentData.Param1 < ornamentConfigData.GoalNum)
								{
									ornamentData.Param1 = ornamentConfigData.GoalNum;
									if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
									{
										ornamentData.Param1 = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_CoupleArenaDuanWei(OrnamentGoalEventObject evObj, List<int> goalIdList, Dictionary<int, OrnamentConfigData> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, OrnamentData> ornamentDataDict = evObj.Client.ClientData.OrnamentDataDict;
				lock (ornamentDataDict)
				{
					foreach (int num in goalIdList)
					{
						OrnamentData ornamentData = null;
						if (!ornamentDataDict.TryGetValue(num, out ornamentData))
						{
							ornamentData = new OrnamentData();
							ornamentData.ID = num;
							ornamentDataDict[num] = ornamentData;
						}
						CoupleArenaCoupleJingJiData cachedCoupleData = SingletonTemplate<CoupleArenaManager>.Instance().GetCachedCoupleData(evObj.Client.ClientData.RoleID);
						if (cachedCoupleData != null && cachedCoupleData.DuanWeiType >= 7)
						{
							OrnamentConfigData ornamentConfigData = null;
							if (goalConfigDict.TryGetValue(num, out ornamentConfigData))
							{
								if (ornamentData.Param1 < ornamentConfigData.GoalNum)
								{
									ornamentData.Param1 = ornamentConfigData.GoalNum;
									if (!this.UpdateDb(evObj.Client.ClientData.RoleID, ornamentData, evObj.Client.ServerId))
									{
										ornamentData.Param1 = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		public bool ProcessOrnamentGetDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				byte[] buffer = DataHelper.ObjectToBytes<Dictionary<int, OrnamentData>>(client.ClientData.OrnamentDataDict);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOrnamentGetGoodsListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.OrnamentGoodsDataList);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOrnamentSlotForgeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				if (num3 <= 0 || num3 >= 6)
				{
					num = -2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				OrnamentData ornamentData = null;
				if (!client.ClientData.OrnamentDataDict.TryGetValue(num3, out ornamentData))
				{
					num = -2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				Dictionary<int, OrnamentSlotConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.OrnamentSlotLevUpConfig;
				}
				OrnamentSlotConfigData ornamentSlotConfigData = null;
				if (!dictionary.TryGetValue(ornamentData.Param1, out ornamentSlotConfigData))
				{
					num = -23;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				if (ornamentSlotConfigData.Need <= 0)
				{
					num = -23;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				if (ornamentSlotConfigData.Need > GameManager.ClientMgr.GetOrnamentCharmPointValue(client))
				{
					num = -32;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, -ornamentSlotConfigData.Need, "饰品", true, true, false);
				ornamentData.Param1++;
				this.UpdateDb(client.ClientData.RoleID, ornamentData, client.ServerId);
				this.TryActiveOrnamentSlot(client);
				this.RefreshOrnamentProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					num3,
					ornamentData.Param1
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessOrnamentActiveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				int dbID = Convert.ToInt32(cmdParams[2]);
				Dictionary<int, OrnamentConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.OrnamentConfig;
				}
				OrnamentConfigData ornamentConfigData = null;
				if (!dictionary.TryGetValue(num3, out ornamentConfigData))
				{
					num = -2;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				if (!this.OrnamentCanAdd(client, num3))
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				if (ornamentConfigData.Type == OrnamentType.OT_Active)
				{
					OrnamentData data = null;
					if (!client.ClientData.OrnamentDataDict.TryGetValue(num3, out data))
					{
						num = -2;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
					if (!this.CheckCanActiveChengJiuOrnament(client, data, ornamentConfigData))
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
					string strStartTime = "1900-01-01 12:00:00";
					string endTime = "1900-01-01 12:00:00";
					Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, num3, 1, 0, "", 0, 1, 9000, "", true, 1, "饰品激活", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
				}
				else if (ornamentConfigData.Type == OrnamentType.OT_UseGoods)
				{
					GoodsData goodsByDbID = Global.GetGoodsByDbID(client, dbID);
					if (goodsByDbID == null || goodsByDbID.Using > 0)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByDbID, 1, false, true))
					{
						num = -6;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
					string strStartTime = "1900-01-01 12:00:00";
					string endTime = "1900-01-01 12:00:00";
					Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsByDbID.GoodsID, 1, 0, "", 0, goodsByDbID.Binding, 9000, "", true, 1, "饰品激活", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
				}
				if (ornamentConfigData.GoalAward > 0)
				{
					GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, ornamentConfigData.GoalAward, "饰品激活", true, true, false);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			List<int> list = new List<int>();
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("OrnamentSiteOpen");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						list.Add(Global.SafeConvertToInt32(array2[1]));
					}
				}
			}
			lock (this.ConfigMutex)
			{
				this.OrnamentSlotOpenConfig = list;
			}
			return this.LoadOrnamentConfigFile() && this.LoadOrnamentSlotLevUpFile() && this.LoadOrnamentGroupFile();
		}

		public bool LoadOrnamentConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/Ornament.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Ornament.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<OrnamentGoalType, List<int>> dictionary = new Dictionary<OrnamentGoalType, List<int>>();
				Dictionary<int, OrnamentConfigData> dictionary2 = new Dictionary<int, OrnamentConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						OrnamentConfigData ornamentConfigData = new OrnamentConfigData();
						ornamentConfigData.GoodsID = (int)Global.GetSafeAttributeLong(xelement2, "GoodsID");
						ornamentConfigData.Type = (OrnamentType)Global.GetSafeAttributeLong(xelement2, "Type");
						ornamentConfigData.RecoverPoints = (int)Global.GetSafeAttributeLong(xelement2, "Recover");
						ornamentConfigData.GoalType = (OrnamentGoalType)Global.GetSafeAttributeLong(xelement2, "GoalType");
						ornamentConfigData.GoalNum = (int)Global.GetSafeAttributeLong(xelement2, "GoalNum");
						ornamentConfigData.GoalAward = (int)Global.GetSafeAttributeLong(xelement2, "GoalAward");
						if (!dictionary.ContainsKey(ornamentConfigData.GoalType))
						{
							dictionary[ornamentConfigData.GoalType] = new List<int>();
						}
						dictionary[ornamentConfigData.GoalType].Add(ornamentConfigData.GoodsID);
						dictionary2[ornamentConfigData.GoodsID] = ornamentConfigData;
					}
				}
				lock (this.ConfigMutex)
				{
					this.Func2GoalId = dictionary;
					this.OrnamentConfig = dictionary2;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/Ornament.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadOrnamentSlotLevUpFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/OrnamentSite.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/OrnamentSite.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, OrnamentSlotConfigData> dictionary = new Dictionary<int, OrnamentSlotConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						OrnamentSlotConfigData ornamentSlotConfigData = new OrnamentSlotConfigData();
						ornamentSlotConfigData.Level = (int)Global.GetSafeAttributeLong(xelement2, "LevelID");
						ornamentSlotConfigData.Need = (int)Global.GetSafeAttributeLong(xelement2, "Need");
						dictionary[ornamentSlotConfigData.Level] = ornamentSlotConfigData;
					}
				}
				lock (this.ConfigMutex)
				{
					this.OrnamentSlotLevUpConfig = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/OrnamentSite.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadOrnamentGroupFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/OrnamentGroup.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/OrnamentGroup.xml"));
				if (null == xelement)
				{
					return false;
				}
				List<OrnamentGroupConfigData> list = new List<OrnamentGroupConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						OrnamentGroupConfigData ornamentGroupConfigData = new OrnamentGroupConfigData();
						ornamentGroupConfigData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "OrnamentGoods");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < array.Length; i++)
						{
							ornamentGroupConfigData.GoodsIDList.Add(Global.SafeConvertToInt32(array[i]));
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GroupProperty");
						string[] array2 = safeAttributeStr2.Split(new char[]
						{
							'|'
						});
						foreach (string text in array2)
						{
							string[] array4 = text.Split(new char[]
							{
								','
							});
							if (array4.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
								if (propIndexByPropName != ExtPropIndexes.Max)
								{
									ornamentGroupConfigData.ExtProps[(int)propIndexByPropName] = Global.SafeConvertToDouble(array4[1]);
								}
							}
						}
						list.Add(ornamentGroupConfigData);
					}
				}
				lock (this.ConfigMutex)
				{
					this.OrnamentGroupConfig = list;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/OrnamentGroup.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string Ornament_OrnamentFileName = "Config/Ornament.xml";

		private const string Ornament_OrnamentSiteFileName = "Config/OrnamentSite.xml";

		private const string Ornament_OrnamentGroupFileName = "Config/OrnamentGroup.xml";

		private Dictionary<OrnamentGoalType, Action<OrnamentGoalEventObject, List<int>, Dictionary<int, OrnamentConfigData>>> evHandlerDict = null;

		private Dictionary<OrnamentGoalType, List<int>> Func2GoalId = null;

		private object ConfigMutex = new object();

		protected List<int> OrnamentSlotOpenConfig = new List<int>();

		protected Dictionary<int, OrnamentConfigData> OrnamentConfig = null;

		protected Dictionary<int, OrnamentSlotConfigData> OrnamentSlotLevUpConfig = null;

		protected List<OrnamentGroupConfigData> OrnamentGroupConfig = null;

		private static OrnamentManager instance = new OrnamentManager();
	}
}
