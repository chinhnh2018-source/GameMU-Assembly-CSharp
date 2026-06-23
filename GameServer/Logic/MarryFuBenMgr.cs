using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	internal class MarryFuBenMgr : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static MarryFuBenMgr getInstance()
		{
			return MarryFuBenMgr.instance;
		}

		public bool initialize()
		{
			MarriageOtherLogic.getInstance().init();
			this.ManAndWifeBossXmlItems.LoadFromXMlFile("Config/ManAndWifeBoss.xml", "", "MonsterID", 0);
			TCPCmdDispatcher.getInstance().registerProcessorEx(870, 1, 2, MarryFuBenMgr.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(18, MarryFuBenMgr.getInstance());
			GlobalEventSource.getInstance().registerListener(12, MarryFuBenMgr.getInstance());
			return true;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			GlobalEventSource.getInstance().removeListener(18, MarryFuBenMgr.getInstance());
			GlobalEventSource.getInstance().removeListener(12, MarryFuBenMgr.getInstance());
			if (null != this.MarriageInstanceDic)
			{
				lock (this.MarriageInstanceDic)
				{
					this.MarriageInstanceDic.Clear();
				}
			}
			MarriageOtherLogic.getInstance().destroy();
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 18)
			{
				Monster monster = (eventObject as MonsterBlooadChangedEventObject).getMonster();
				GameClient gameClient = (eventObject as MonsterBlooadChangedEventObject).getGameClient();
				if (monster != null && null != gameClient)
				{
					if (gameClient.ClientData.CopyMapID > 0 && gameClient.ClientData.FuBenSeqID > 0 && MapTypes.MarriageCopy == Global.GetMapType(gameClient.ClientData.MapCode) && MapTypes.MarriageCopy == Global.GetMapType(monster.CurrentMapCode))
					{
						SystemXmlItem systemXmlItem = null;
						if (this.ManAndWifeBossXmlItems.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out systemXmlItem) && null != systemXmlItem)
						{
							if (systemXmlItem.GetIntValue("Need", -1) != (int)gameClient.ClientData.MyMarriageData.byMarrytype)
							{
								BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, systemXmlItem.GetIntValue("GoodsID", -1));
								if (monsterBufferDataByID == null || Global.IsBufferDataOver(monsterBufferDataByID, 0L))
								{
									double[] array = new double[]
									{
										15.0,
										1.0
									};
									EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(2000808);
									if (null != equipPropItem)
									{
										array[1] = equipPropItem.ExtProps[24];
									}
									Global.UpdateMonsterBufferData(monster, BufferItemTypes.MU_MARRIAGE_SUBDAMAGEPERCENTTIMER, array);
									string text = string.Format(GLang.GetLang(484, new object[0]), gameClient.ClientData.RoleName, monster.MonsterInfo.VSName);
									GameManager.ClientMgr.BroadSpecialHintText(monster.CurrentMapCode, monster.CurrentCopyMapID, text);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 12)
			{
				GameClient gameClient = (eventObject as PlayerLogoutEventObject).getPlayer();
				this.ClientExitRoom(gameClient);
			}
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 870)
			{
				int num = 0;
				try
				{
					num = Global.SafeConvertToInt32(cmdParams[0]);
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, "ProcessMarryFuben", false, false);
				}
				int[] cmdData = null;
				if (!MarryLogic.IsVersionSystemOpenOfMarriage())
				{
					cmdData = new int[]
					{
						6
					};
					client.sendCmd<int[]>(nID, cmdData, false);
				}
				else
				{
					if (1 == num)
					{
						cmdData = new int[]
						{
							(int)this.GetMarriageInstanceState(client, null)
						};
					}
					else if (2 == num)
					{
						cmdData = new int[]
						{
							(int)this.ClientEnterRoom(client)
						};
					}
					else if (3 == num)
					{
						cmdData = new int[]
						{
							(int)this.ClientExitRoom(client)
						};
					}
					else if (4 == num)
					{
						cmdData = new int[]
						{
							(int)this.ClientReady(client, Global.SafeConvertToInt32(cmdParams[1]))
						};
					}
					else if (5 == num)
					{
						cmdData = new int[]
						{
							(int)this.ClientExitReady(client)
						};
					}
					client.sendCmd<int[]>(nID, cmdData, false);
				}
			}
			return true;
		}

		private MarriageInstance GetMarriageInstance(GameClient client)
		{
			MarriageInstance result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				lock (this.MarriageInstanceDic)
				{
					MarriageInstance marriageInstance = null;
					this.MarriageInstanceDic.TryGetValue(client.ClientData.RoleID, out marriageInstance);
					if (null == marriageInstance)
					{
						this.MarriageInstanceDic.TryGetValue(client.ClientData.MyMarriageData.nSpouseID, out marriageInstance);
					}
					result = marriageInstance;
				}
			}
			return result;
		}

		private MarryFubenResult GetMarriageInstanceState(GameClient client, MarriageInstance FubenInstance = null)
		{
			MarryFubenResult result;
			if (null == client)
			{
				result = MarryFubenResult.Error;
			}
			else
			{
				int[] cmdData = new int[6];
				if (null == FubenInstance)
				{
					FubenInstance = this.GetMarriageInstance(client);
				}
				if (null != FubenInstance)
				{
					cmdData = new int[]
					{
						FubenInstance.nHusband_ID,
						FubenInstance.nHusband_state,
						FubenInstance.nWife_ID,
						FubenInstance.nWife_state,
						FubenInstance.nHusband_FuBenID,
						FubenInstance.nWife_FuBenID
					};
					client.sendCmd<int[]>(870, cmdData, false);
					result = MarryFubenResult.Success;
				}
				else
				{
					int[] array = new int[6];
					array[0] = -1;
					array[2] = -1;
					cmdData = array;
					client.sendCmd<int[]>(870, cmdData, false);
					result = MarryFubenResult.Success;
				}
			}
			return result;
		}

		public MarryFubenResult ClientEnterRoom(GameClient client)
		{
			MarryFubenResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryFubenResult.Error_Denied_For_Minor_Occupation;
			}
			else if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				result = MarryFubenResult.InFuben;
			}
			else
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (mapSceneType == 21 || mapSceneType == 20)
				{
					result = MarryFubenResult.InFuben;
				}
				else
				{
					GameClient client2 = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
					MarriageInstance marriageInstance = this.GetMarriageInstance(client);
					lock (this.MarriageInstanceDic)
					{
						if (null == marriageInstance)
						{
							marriageInstance = new MarriageInstance();
							marriageInstance.nCreateRole_ID = client.ClientData.RoleID;
							if (1 == client.ClientData.MyMarriageData.byMarrytype)
							{
								marriageInstance.nHusband_ID = client.ClientData.RoleID;
								marriageInstance.nHusband_state = 0;
							}
							else if (2 == client.ClientData.MyMarriageData.byMarrytype)
							{
								marriageInstance.nWife_ID = client.ClientData.RoleID;
								marriageInstance.nWife_state = 0;
							}
							this.MarriageInstanceDic.Add(marriageInstance.nCreateRole_ID, marriageInstance);
							this.GetMarriageInstanceState(client, marriageInstance);
							this.GetMarriageInstanceState(client2, marriageInstance);
							result = MarryFubenResult.Success;
						}
						else
						{
							if (1 == client.ClientData.MyMarriageData.byMarrytype)
							{
								marriageInstance.nHusband_ID = client.ClientData.RoleID;
								marriageInstance.nHusband_FuBenID = 0;
								marriageInstance.nHusband_state = 0;
							}
							else if (2 == client.ClientData.MyMarriageData.byMarrytype)
							{
								marriageInstance.nWife_ID = client.ClientData.RoleID;
								marriageInstance.nWife_FuBenID = 0;
								marriageInstance.nWife_state = 0;
							}
							this.GetMarriageInstanceState(client, marriageInstance);
							this.GetMarriageInstanceState(client2, marriageInstance);
							result = MarryFubenResult.Success;
						}
					}
				}
			}
			return result;
		}

		public MarryFubenResult ClientExitRoom(GameClient client)
		{
			MarryFubenResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else
			{
				GameClient client2 = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				MarriageInstance marriageInstance = this.GetMarriageInstance(client);
				if (null != marriageInstance)
				{
					if (1 == client.ClientData.MyMarriageData.byMarrytype)
					{
						marriageInstance.nHusband_ID = -1;
						marriageInstance.nHusband_state = -1;
						marriageInstance.nHusband_FuBenID = 0;
					}
					else
					{
						marriageInstance.nWife_ID = -1;
						marriageInstance.nWife_state = -1;
						marriageInstance.nWife_FuBenID = 0;
					}
					if (-1 == marriageInstance.nHusband_ID && -1 == marriageInstance.nWife_ID)
					{
						this.RemoveMarriageInstance(marriageInstance, false);
						marriageInstance = null;
					}
					this.GetMarriageInstanceState(client, marriageInstance);
					this.GetMarriageInstanceState(client2, marriageInstance);
					result = MarryFubenResult.Success;
				}
				else
				{
					result = MarryFubenResult.Error;
				}
			}
			return result;
		}

		public MarryFubenResult ClientReady(GameClient client, int FuBenID)
		{
			MarryFubenResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryFubenResult.Error_Denied_For_Minor_Occupation;
			}
			else if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				result = MarryFubenResult.InFuben;
			}
			else
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (mapSceneType == 21 || mapSceneType == 20)
				{
					result = MarryFubenResult.InFuben;
				}
				else
				{
					GameClient client2 = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
					MarriageInstance marriageInstance = this.GetMarriageInstance(client);
					if (null == marriageInstance)
					{
						result = MarryFubenResult.Error;
					}
					else if (1 == marriageInstance.nHusband_state && 1 == marriageInstance.nWife_state && marriageInstance.nHusband_FuBenID == marriageInstance.nWife_FuBenID)
					{
						result = MarryFubenResult.IsReaday;
					}
					else
					{
						if (1 == client.ClientData.MyMarriageData.byMarrytype)
						{
							marriageInstance.nHusband_state = 1;
							marriageInstance.nHusband_FuBenID = FuBenID;
						}
						else if (2 == client.ClientData.MyMarriageData.byMarrytype)
						{
							marriageInstance.nWife_state = 1;
							marriageInstance.nWife_FuBenID = FuBenID;
						}
						this.GetMarriageInstanceState(client, marriageInstance);
						this.GetMarriageInstanceState(client2, marriageInstance);
						result = MarryFubenResult.Success;
					}
				}
			}
			return result;
		}

		public MarryFubenResult ClientExitReady(GameClient client)
		{
			MarryFubenResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else
			{
				GameClient client2 = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				MarriageInstance marriageInstance = this.GetMarriageInstance(client);
				if (null != marriageInstance)
				{
					if (1 == marriageInstance.nHusband_state && 1 == marriageInstance.nWife_state && marriageInstance.nHusband_FuBenID == marriageInstance.nWife_FuBenID)
					{
						result = MarryFubenResult.IsReaday;
					}
					else
					{
						if (1 == client.ClientData.MyMarriageData.byMarrytype)
						{
							marriageInstance.nHusband_state = 0;
							marriageInstance.nHusband_FuBenID = 0;
						}
						else
						{
							marriageInstance.nWife_state = 0;
							marriageInstance.nWife_FuBenID = 0;
						}
						this.GetMarriageInstanceState(client, marriageInstance);
						this.GetMarriageInstanceState(client2, marriageInstance);
						result = MarryFubenResult.Success;
					}
				}
				else
				{
					result = MarryFubenResult.Error;
				}
			}
			return result;
		}

		public void StartInstance(GameClient client)
		{
			this.ClientExitRoom(client);
		}

		private void RemoveMarriageInstance(MarriageInstance FubenInstance, bool bNeedsendtoclient = false)
		{
			lock (this.MarriageInstanceDic)
			{
				if (bNeedsendtoclient)
				{
					GameClient client = GameManager.ClientMgr.FindClient(FubenInstance.nHusband_ID);
					this.GetMarriageInstanceState(client, FubenInstance);
					GameClient client2 = GameManager.ClientMgr.FindClient(FubenInstance.nWife_ID);
					this.GetMarriageInstanceState(client2, FubenInstance);
				}
				this.MarriageInstanceDic.Remove(FubenInstance.nCreateRole_ID);
			}
		}

		public static bool UpdateMarriageData2DB(GameClient client)
		{
			return MarryFuBenMgr.UpdateMarriageData2DB(client.ClientData.RoleID, client.ClientData.MyMarriageData, client);
		}

		public static bool UpdateMarriageData2DB(int nRoleID, MarriageData updateMarriageData, GameClient self)
		{
			byte[] array = DataHelper.ObjectToBytes<MarriageData>(updateMarriageData);
			byte[] bytes = BitConverter.GetBytes(nRoleID);
			byte[] array2 = new byte[array.Length + 4];
			Array.Copy(bytes, array2, 4);
			Array.Copy(array, 0, array2, 4, array.Length);
			return Global.sendToDB<bool, byte[]>(10185, array2, self.ServerId);
		}

		public bool CanEnterSceneEX(GameClient client)
		{
			MarriageInstance marriageInstance = this.GetMarriageInstance(client);
			GameClient gameClient = GameManager.ClientMgr.FindClient(marriageInstance.nHusband_ID);
			bool result;
			if (gameClient == null)
			{
				result = false;
			}
			else
			{
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(marriageInstance.nWife_ID);
				if (gameClient2 == null)
				{
					result = false;
				}
				else if (1 != marriageInstance.nHusband_state || 1 != marriageInstance.nWife_state)
				{
					this.RemoveMarriageInstance(marriageInstance, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public MarriageInstance GetMarriageInstanceEX(GameClient client)
		{
			return this.GetMarriageInstance(client);
		}

		private static MarryFuBenMgr instance = new MarryFuBenMgr();

		private Dictionary<int, MarriageInstance> MarriageInstanceDic = new Dictionary<int, MarriageInstance>();

		private SystemXmlItem MarriageFubenXmlItem = null;

		private SystemXmlItems ManAndWifeBossXmlItems = new SystemXmlItems();
	}
}
