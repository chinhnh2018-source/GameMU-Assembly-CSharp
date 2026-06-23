using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic.Copy;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	internal class LuoLanFaZhenCopySceneManager
	{
		public static int getAwardRate(int FuBenID, int FuBenSeqID)
		{
			int result;
			if (FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
			{
				result = 1;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(FuBenSeqID);
				if (null == fubenData)
				{
					result = 1;
				}
				else if (fubenData.SpecailBossLeftNum == 0)
				{
					result = LuoLanFaZhenCopySceneManager.SpecialAwardRate;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}

		public static void initialize()
		{
			try
			{
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LuoLanFaZhen", ',');
				if (paramValueIntArrayByName.Length != 5)
				{
					throw new Exception("systemParamsList.LuoLanFaZhen参数数量应该是5");
				}
				LuoLanFaZhenCopySceneManager.SpecialBossID = paramValueIntArrayByName[0];
				LuoLanFaZhenCopySceneManager.SpecialMapCode = paramValueIntArrayByName[1];
				LuoLanFaZhenCopySceneManager.SpecialAwardRate = paramValueIntArrayByName[2];
				LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID = paramValueIntArrayByName[3];
				LuoLanFaZhenCopySceneManager.SpecialTeleRate = paramValueIntArrayByName[4];
				SystemXmlItem systemXmlItem = null;
				if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID, out systemXmlItem) && systemXmlItem != null)
				{
					LuoLanFaZhenCopySceneManager.FinalBossID = systemXmlItem.GetIntValue("BossID", -1);
				}
				LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.LoadFromXMlFile("Config/LuoLanFaZhen.xml", "", "MapID", 0);
				List<int> list = LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.SystemXmlItemDict.Keys.ToList<int>();
				foreach (int num in list)
				{
					SystemXmlItem systemXmlItem2 = null;
					if (LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.SystemXmlItemDict.TryGetValue(num, out systemXmlItem2) && systemXmlItem2 != null)
					{
						SystemFazhenMapData systemFazhenMapData = new SystemFazhenMapData();
						systemFazhenMapData.MapCode = num;
						int[] intArrayValue = systemXmlItem2.GetIntArrayValue("TeShuMapID", '|');
						if (intArrayValue != null && intArrayValue.Length >= 3)
						{
							systemFazhenMapData.SpecialDestMapCode = intArrayValue[0];
							systemFazhenMapData.SpecialDestX = intArrayValue[1];
							systemFazhenMapData.SpecialDestY = intArrayValue[2];
						}
						else
						{
							systemFazhenMapData.SpecialDestMapCode = -1;
							systemFazhenMapData.SpecialDestX = -1;
							systemFazhenMapData.SpecialDestY = -1;
						}
						int[] intArrayValue2 = systemXmlItem2.GetIntArrayValue("ChuanSongMenID", '|');
						string stringValue = systemXmlItem2.GetStringValue("MuDidiID");
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (intArrayValue2.Length != array.Length)
						{
							throw new Exception("LuoLanFaZhen.xml传送门数量和目标地图数量不一致");
						}
						for (int i = 0; i < intArrayValue2.Length; i++)
						{
							systemFazhenMapData.listGateID.Add(intArrayValue2[i]);
						}
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							systemFazhenMapData.listDestMapCode.Add(Convert.ToInt32(array2[0]));
						}
						LuoLanFaZhenCopySceneManager.m_AllMapGatesStaticData[num] = systemFazhenMapData;
					}
				}
			}
			catch (Exception)
			{
				throw new Exception("罗兰法阵配置项出错");
			}
		}

		public static bool OnFubenOver(int FubenSeqID)
		{
			lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
			{
				LuoLanFaZhenCopySceneManager.AllFazhenFubenData.Remove(FubenSeqID);
			}
			return true;
		}

		public static SingleLuoLanFaZhenFubenData GetFubenData(int FubenSeqID)
		{
			SingleLuoLanFaZhenFubenData result;
			if (FubenSeqID <= 0)
			{
				result = null;
			}
			else
			{
				SingleLuoLanFaZhenFubenData singleLuoLanFaZhenFubenData = null;
				lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
				{
					if (!LuoLanFaZhenCopySceneManager.AllFazhenFubenData.TryGetValue(FubenSeqID, out singleLuoLanFaZhenFubenData))
					{
						return null;
					}
				}
				result = singleLuoLanFaZhenFubenData;
			}
			return result;
		}

		public static bool IsLuoLanFaZhen(int FubenID)
		{
			return FubenID == LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID;
		}

		public static bool IsLuoLanFaZhenMap(int mapcode)
		{
			return null != FuBenManager.FindMapCodeByFuBenID(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID, mapcode);
		}

		public static bool EnterFubenMapWhenLogin(GameClient client)
		{
			return null != LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
		}

		public static bool OnEnterFubenMap(GameClient client, int oldmapcode, bool isLogin)
		{
			return LuoLanFaZhenCopySceneManager.TryAddFubenData(client.ClientData.FuBenSeqID, client.ClientData.FuBenID, client.ClientData.CopyMapID, client.ClientData.MapCode);
		}

		public static void OnLeaveFubenMap(GameClient client, int toMapCode)
		{
		}

		public static void CreateRandomGates(int MapCode, FazhenMapData MapData)
		{
			SystemFazhenMapData systemFazhenMapData = null;
			if (LuoLanFaZhenCopySceneManager.m_AllMapGatesStaticData.TryGetValue(MapCode, out systemFazhenMapData))
			{
				if (null != systemFazhenMapData)
				{
					List<int> list = new List<int>();
					foreach (int item in systemFazhenMapData.listGateID)
					{
						int randomNumber = Global.GetRandomNumber(0, list.Count + 1);
						list.Insert(randomNumber, item);
					}
					if (list.Count == systemFazhenMapData.listDestMapCode.Count)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(MapData.MapCode, out gameMap) && null != gameMap)
						{
							lock (MapData.Telegates)
							{
								for (int i = 0; i < list.Count; i++)
								{
									MapTeleport mapTeleport = null;
									if (gameMap.MapTeleportDict.TryGetValue(systemFazhenMapData.listGateID[i], out mapTeleport) && null != mapTeleport)
									{
										SingleFazhenTelegateData singleFazhenTelegateData = new SingleFazhenTelegateData();
										singleFazhenTelegateData.usedAlready = false;
										singleFazhenTelegateData.gateId = list[i];
										singleFazhenTelegateData.destMapCode = systemFazhenMapData.listDestMapCode[i];
										singleFazhenTelegateData.SpecialDestMapCode = systemFazhenMapData.SpecialDestMapCode;
										singleFazhenTelegateData.SpecialDestX = systemFazhenMapData.SpecialDestX;
										singleFazhenTelegateData.SpecialDestY = systemFazhenMapData.SpecialDestY;
										singleFazhenTelegateData.destX = mapTeleport.ToX;
										singleFazhenTelegateData.destY = mapTeleport.ToY;
										MapData.Telegates[singleFazhenTelegateData.gateId] = singleFazhenTelegateData;
									}
								}
							}
						}
					}
				}
			}
		}

		protected static bool TryAddFubenData(int _FubenSeqID, int _FubenID, int _MapID, int _MapCode)
		{
			bool result;
			if (_FubenSeqID <= 0 || _FubenID <= 0 || _MapID <= 0 || _MapCode <= 0)
			{
				result = false;
			}
			else
			{
				FazhenMapData fazhenMapData = null;
				SingleLuoLanFaZhenFubenData singleLuoLanFaZhenFubenData = null;
				lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
				{
					if (!LuoLanFaZhenCopySceneManager.AllFazhenFubenData.TryGetValue(_FubenSeqID, out singleLuoLanFaZhenFubenData) || null == singleLuoLanFaZhenFubenData)
					{
						singleLuoLanFaZhenFubenData = new SingleLuoLanFaZhenFubenData
						{
							FubenID = _FubenID,
							FubenSeqID = _FubenSeqID
						};
						fazhenMapData = new FazhenMapData
						{
							CopyMapID = _MapID,
							MapCode = _MapCode
						};
						LuoLanFaZhenCopySceneManager.CreateRandomGates(_MapCode, fazhenMapData);
						singleLuoLanFaZhenFubenData.MapDatas[_MapID] = fazhenMapData;
						LuoLanFaZhenCopySceneManager.AllFazhenFubenData[_FubenSeqID] = singleLuoLanFaZhenFubenData;
					}
					else
					{
						lock (singleLuoLanFaZhenFubenData.MapDatas)
						{
							if (!singleLuoLanFaZhenFubenData.MapDatas.TryGetValue(_MapID, out fazhenMapData) || null == fazhenMapData)
							{
								fazhenMapData = new FazhenMapData
								{
									CopyMapID = _MapID,
									MapCode = _MapCode
								};
								LuoLanFaZhenCopySceneManager.CreateRandomGates(_MapCode, fazhenMapData);
								singleLuoLanFaZhenFubenData.MapDatas[_MapID] = fazhenMapData;
							}
						}
					}
				}
				result = true;
			}
			return result;
		}

		public static TCPProcessCmdResults OnTeleport(GameClient client, int teleportID, TCPOutPacketPool pool, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			TCPProcessCmdResults result;
			if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID || client.ClientData.FuBenSeqID <= 0)
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					FazhenMapData fazhenMapData = null;
					SingleFazhenTelegateData singleFazhenTelegateData = null;
					lock (fubenData.MapDatas)
					{
						if (!fubenData.MapDatas.TryGetValue(client.ClientData.CopyMapID, out fazhenMapData) || null == fazhenMapData)
						{
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					if (fazhenMapData.MapCode != client.ClientData.MapCode || fazhenMapData.CopyMapID != client.ClientData.CopyMapID)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else
					{
						lock (fazhenMapData.Telegates)
						{
							if (!fazhenMapData.Telegates.TryGetValue(teleportID, out singleFazhenTelegateData) || null == singleFazhenTelegateData)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
						}
						if (singleFazhenTelegateData.destMapCode <= 0)
						{
							result = TCPProcessCmdResults.RESULT_FAILED;
						}
						else
						{
							bool flag3 = false;
							if (singleFazhenTelegateData.SpecialDestMapCode > 0)
							{
								if (0 != fubenData.SpecailBossLeftNum)
								{
									int randomNumber = Global.GetRandomNumber(0, 100);
									if (randomNumber < LuoLanFaZhenCopySceneManager.SpecialTeleRate)
									{
										flag3 = true;
									}
								}
							}
							if (flag3)
							{
								GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, teleportID, singleFazhenTelegateData.SpecialDestMapCode, singleFazhenTelegateData.SpecialDestX, singleFazhenTelegateData.SpecialDestY, client.ClientData.RoleDirection, 123);
							}
							else
							{
								bool flag4 = false;
								lock (singleFazhenTelegateData)
								{
									if (!singleFazhenTelegateData.usedAlready)
									{
										singleFazhenTelegateData.usedAlready = true;
										flag4 = true;
									}
								}
								if (flag4)
								{
									FazhenMapProtoData fazhenMapProtoData = new FazhenMapProtoData();
									fazhenMapProtoData.listTelegate = new List<FazhenTelegateProtoData>();
									fazhenMapProtoData.SrcMapCode = fazhenMapData.MapCode;
									FazhenTelegateProtoData fazhenTelegateProtoData = new FazhenTelegateProtoData();
									fazhenTelegateProtoData.gateId = singleFazhenTelegateData.gateId;
									fazhenTelegateProtoData.DestMapCode = singleFazhenTelegateData.destMapCode;
									fazhenMapProtoData.listTelegate.Add(fazhenTelegateProtoData);
									LuoLanFaZhenCopySceneManager.BroadMapData<FazhenMapProtoData>(685, fazhenMapProtoData, fazhenMapData.MapCode, client.ClientData.FuBenSeqID);
								}
								GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, teleportID, singleFazhenTelegateData.destMapCode, singleFazhenTelegateData.destX, singleFazhenTelegateData.destY, client.ClientData.RoleDirection, 123);
							}
							result = TCPProcessCmdResults.RESULT_OK;
						}
					}
				}
			}
			return result;
		}

		public static bool OnKillMonster(GameClient client, Monster monster)
		{
			bool result;
			if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
			{
				result = false;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					result = false;
				}
				else
				{
					List<int> list = null;
					bool flag = false;
					if (monster.MonsterInfo.ExtensionID == LuoLanFaZhenCopySceneManager.FinalBossID)
					{
						fubenData.FinalBossLeftNum = 0;
						flag = true;
					}
					else if (monster.MonsterInfo.ExtensionID == LuoLanFaZhenCopySceneManager.SpecialBossID)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
						if (null == fuBenInfoItem)
						{
							return false;
						}
						fubenData.SpecailBossLeftNum = 0;
						string msg = StringUtil.substitute(GLang.GetLang(98, new object[0]), new object[]
						{
							client.ClientData.RoleName
						});
						list = SingletonTemplate<CopyTeamManager>.Instance().GetTeamCopyMapCodes(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID);
						if (null == list)
						{
							return false;
						}
						foreach (int mapCode in list)
						{
							LuoLanFaZhenCopySceneManager.BroadMapMessage(msg, mapCode, client.ClientData.FuBenSeqID);
						}
						flag = true;
					}
					if (flag)
					{
						if (null == list)
						{
							list = SingletonTemplate<CopyTeamManager>.Instance().GetTeamCopyMapCodes(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID);
						}
						if (null == list)
						{
							return false;
						}
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID,
							LuoLanFaZhenCopySceneManager.FinalBossID,
							fubenData.FinalBossLeftNum,
							LuoLanFaZhenCopySceneManager.SpecialBossID,
							fubenData.SpecailBossLeftNum
						});
						foreach (int mapCode in list)
						{
							LuoLanFaZhenCopySceneManager.BroadMapData(760, cmdData, mapCode, client.ClientData.FuBenSeqID);
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessFazhenTeleportCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = 0;
				if (!int.TryParse(array[1], out num2))
				{
					LogManager.WriteLog(2, string.Format("ProcessFazhenTeleportCMD, roleID={0}, MapCode={1}", num, array[1]), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientData.MapCode != num2)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (gameClient.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(gameClient.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				FazhenMapData fazhenMapData = null;
				lock (fubenData.MapDatas)
				{
					fubenData.MapDatas.TryGetValue(gameClient.ClientData.CopyMapID, out fazhenMapData);
					if (null == fazhenMapData)
					{
						return TCPProcessCmdResults.RESULT_OK;
					}
				}
				FazhenMapProtoData fazhenMapProtoData = new FazhenMapProtoData();
				fazhenMapProtoData.listTelegate = new List<FazhenTelegateProtoData>();
				fazhenMapProtoData.SrcMapCode = fazhenMapData.MapCode;
				lock (fazhenMapData.Telegates)
				{
					List<int> list = fazhenMapData.Telegates.Keys.ToList<int>();
					if (null != list)
					{
						foreach (int num3 in list)
						{
							SingleFazhenTelegateData singleFazhenTelegateData = fazhenMapData.Telegates[num3];
							if (null != singleFazhenTelegateData)
							{
								FazhenTelegateProtoData fazhenTelegateProtoData = new FazhenTelegateProtoData();
								fazhenTelegateProtoData.gateId = num3;
								if (singleFazhenTelegateData.usedAlready || LuoLanFaZhenCopySceneManager.GM_OpenState == 1)
								{
									fazhenTelegateProtoData.DestMapCode = singleFazhenTelegateData.destMapCode;
								}
								else
								{
									fazhenTelegateProtoData.DestMapCode = 0;
								}
								fazhenMapProtoData.listTelegate.Add(fazhenTelegateProtoData);
							}
						}
					}
				}
				byte[] array2 = DataHelper.ObjectToBytes<FazhenMapProtoData>(fazhenMapProtoData);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array2, 0, array2.Length, 685);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessFazhenTeleportCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessFazhenBossCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(gameClient.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID,
					LuoLanFaZhenCopySceneManager.FinalBossID,
					fubenData.FinalBossLeftNum,
					LuoLanFaZhenCopySceneManager.SpecialBossID,
					fubenData.SpecailBossLeftNum
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 760);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessFazhenBossCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static void BroadMapData<T>(int cmdID, T cmdData, int mapCode, int FuBenSeqID)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.FuBenSeqID == FuBenSeqID)
						{
							gameClient.sendCmd<T>(cmdID, cmdData, false);
						}
					}
				}
			}
		}

		public static void BroadMapData(int cmdID, string cmdData, int mapCode, int FuBenSeqID)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.FuBenSeqID == FuBenSeqID)
						{
							gameClient.sendCmd(cmdID, cmdData, false);
						}
					}
				}
			}
		}

		public static void BroadMapMessage(string msg, int mapCode, int FuBenSeqID)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.FuBenSeqID == FuBenSeqID)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, msg, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
					}
				}
			}
		}

		public static void GM_SetOpenState(int openstate)
		{
			LuoLanFaZhenCopySceneManager.GM_OpenState = openstate;
		}

		public static int GM_OpenState = 0;

		public static SystemXmlItems systemLuoLanFaZhen = new SystemXmlItems();

		private static Dictionary<int, SingleLuoLanFaZhenFubenData> AllFazhenFubenData = new Dictionary<int, SingleLuoLanFaZhenFubenData>();

		private static Dictionary<int, SystemFazhenMapData> m_AllMapGatesStaticData = new Dictionary<int, SystemFazhenMapData>();

		public static int LuoLanFaZhenFubenID = 4201;

		public static int SpecialTeleRate = 10;

		protected static int FinalBossID = 0;

		protected static int SpecialBossID = 0;

		protected static int SpecialMapCode = 0;

		protected static int SpecialAwardRate = 0;
	}
}
