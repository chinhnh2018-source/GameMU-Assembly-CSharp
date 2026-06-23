using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class DaimonSquareCopySceneManager
	{
		public void InitDaimonSquareCopyScene()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.DemoSque);
		}

		public void SetDaimonSquareCopySceneTotalPoint(string sName, int nPoint)
		{
			this.m_nDaimonSquareMaxName = sName;
			this.m_nDaimonSquareMaxPoint = nPoint;
		}

		public bool CanEnterExistCopyScene(GameClient client)
		{
			CopyMap copyMap = null;
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			lock (this.m_DaimonSquareCopyScenesList)
			{
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(fuBenSeqID, out copyMap))
				{
					return false;
				}
			}
			bool result;
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				DaimonSquareScene daimonSquareScene = null;
				int fubenMapID = copyMap.FubenMapID;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(fubenMapID, out dictionary))
				{
					result = false;
				}
				else if (!dictionary.TryGetValue(fuBenSeqID, out daimonSquareScene))
				{
					result = false;
				}
				else if (daimonSquareScene.m_eStatus >= DaimonSquareStatus.FIGHT_STATUS_END)
				{
					result = false;
				}
				else
				{
					result = daimonSquareScene.CantiansRole(client);
				}
			}
			return result;
		}

		public void AddDaimonSquareCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (this.m_DaimonSquareCopyScenesList)
			{
				CopyMap copyMap = null;
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSequenceID, out copyMap) || copyMap == null)
				{
					this.m_DaimonSquareCopyScenesList.Add(nSequenceID, mapInfo);
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				DaimonSquareScene daimonSquareScene = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dictionary))
				{
					dictionary = new Dictionary<int, DaimonSquareScene>();
					this.m_DaimonSquareCopyScenesInfo.Add(nFubenID, dictionary);
				}
				if (!dictionary.TryGetValue(nSequenceID, out daimonSquareScene))
				{
					daimonSquareScene = new DaimonSquareScene();
					daimonSquareScene.CleanAllInfo();
					dictionary[nSequenceID] = daimonSquareScene;
				}
				daimonSquareScene.m_nMapCode = nMapCodeID;
				daimonSquareScene.m_CopyMap = mapInfo;
			}
		}

		public void RemoveDaimonSquareListCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (this.m_DaimonSquareCopyScenesList)
			{
				CopyMap copyMap = null;
				if (this.m_DaimonSquareCopyScenesList.TryGetValue(nSqeID, out copyMap) && copyMap != null)
				{
					this.m_DaimonSquareCopyScenesList.Remove(nSqeID);
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nCopyID, out dictionary) && dictionary != null)
				{
					DaimonSquareScene daimonSquareScene = null;
					if (dictionary.TryGetValue(nSqeID, out daimonSquareScene) && daimonSquareScene != null)
					{
						dictionary.Remove(nSqeID);
					}
					if (dictionary.Count <= 0)
					{
						this.m_DaimonSquareCopyScenesInfo.Remove(nCopyID);
					}
				}
			}
		}

		public int CheckDaimonSquareListScenes(int nFuBenMapID)
		{
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFuBenMapID, out dictionary))
				{
					return -1;
				}
				if (dictionary == null)
				{
					return -1;
				}
				DaimonSquareDataInfo daimonSquareDataInfo = null;
				if (!Data.DaimonSquareDataInfoList.TryGetValue(nFuBenMapID, out daimonSquareDataInfo))
				{
					return -1;
				}
				if (daimonSquareDataInfo == null)
				{
					return -1;
				}
				foreach (KeyValuePair<int, DaimonSquareScene> keyValuePair in dictionary)
				{
					int key = keyValuePair.Key;
					DaimonSquareScene value = keyValuePair.Value;
					if (key >= 0 && value != null)
					{
						if (key == nFuBenMapID && value.m_nPlarerCount < daimonSquareDataInfo.MaxEnterNum && value.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN)
						{
							return key;
						}
					}
				}
			}
			return -1;
		}

		public bool IsDaimonSquareCopyScene(int nFuBenMapID)
		{
			return Data.DaimonSquareDataInfoList.ContainsKey(nFuBenMapID);
		}

		public bool IsDaimonSquareCopyScene2(int nMpaCodeID)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(nMpaCodeID);
			return mapSceneType == 6;
		}

		public CopyMap GetDaimonSquareCopySceneInfo(int nSequenceID)
		{
			CopyMap result;
			if (nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				CopyMap copyMap = null;
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSequenceID, out copyMap))
				{
					result = null;
				}
				else
				{
					result = copyMap;
				}
			}
			return result;
		}

		public DaimonSquareScene GetDaimonSquareCopySceneDataInfo(CopyMap cmInfo, int nSequenceID, int nFuBenID)
		{
			DaimonSquareScene result;
			if (cmInfo == null || nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFuBenID, out dictionary) || dictionary == null)
				{
					result = null;
				}
				else
				{
					DaimonSquareScene daimonSquareScene = null;
					if (!dictionary.TryGetValue(nSequenceID, out daimonSquareScene) || daimonSquareScene == null)
					{
						result = null;
					}
					else
					{
						result = daimonSquareScene;
					}
				}
			}
			return result;
		}

		public int EnterDaimonSquareSceneCopySceneCount(GameClient client, int nFubenID, out int nDemonNum)
		{
			nDemonNum = -1;
			DaimonSquareDataInfo daimonSquareDataInfo = null;
			int result;
			if (!Data.DaimonSquareDataInfoList.TryGetValue(nFubenID, out daimonSquareDataInfo))
			{
				result = -1;
			}
			else
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				int nType = 2;
				int num = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, nType);
				nDemonNum = num;
				if (num >= daimonSquareDataInfo.MaxEnterNum)
				{
					bool flag = true;
					int dayOfYear2 = TimeUtil.NowDateTime().DayOfYear;
					int vipLevel = client.ClientData.VipLevel;
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterDaimonSquareCountAddValue", ',');
					if (vipLevel > 0 && paramValueIntArrayByName != null && paramValueIntArrayByName[vipLevel] > 0)
					{
						int num2 = paramValueIntArrayByName[vipLevel];
						if (num < daimonSquareDataInfo.MaxEnterNum + num2)
						{
							Global.UpdateVipDailyData(client, dayOfYear2, 1000002);
							flag = false;
						}
					}
					if (flag)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(101, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						return -1;
					}
				}
				result = 1;
			}
			return result;
		}

		public void SendMegToClient(GameClient client, int nFubenID, int nSquID, int nCmdID)
		{
			CopyMap copyMap = null;
			lock (this.m_DaimonSquareCopyScenesList)
			{
				if (!this.m_DaimonSquareCopyScenesList.TryGetValue(nSquID, out copyMap) || copyMap == null)
				{
					return;
				}
			}
			lock (this.m_DaimonSquareCopyScenesInfo)
			{
				Dictionary<int, DaimonSquareScene> dictionary = null;
				if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dictionary) && dictionary != null)
				{
					DaimonSquareScene daimonSquareScene = null;
					if (dictionary.TryGetValue(nSquID, out daimonSquareScene) && daimonSquareScene != null)
					{
						DaimonSquareDataInfo daimonSquareDataInfo = null;
						if (Data.DaimonSquareDataInfoList.TryGetValue(nFubenID, out daimonSquareDataInfo) && daimonSquareDataInfo != null)
						{
							if (nCmdID == 536)
							{
								long num = TimeUtil.NOW();
								if (daimonSquareScene.m_lPrepareTime > 0L)
								{
									int num2 = 15;
									if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_PREPARE)
									{
										num2 = (int)(((long)(daimonSquareDataInfo.PrepareTime * 1000) - (num - daimonSquareScene.m_lPrepareTime)) / 1000L);
									}
									else if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
									{
										num2 = (int)(((long)(daimonSquareDataInfo.DurationTime * 1000) - (num - daimonSquareScene.m_lBeginTime)) / 1000L);
									}
									string strCmd = string.Format("{0}:{1}", (int)daimonSquareScene.m_eStatus, num2);
									GameManager.ClientMgr.SendToClient(client, strCmd, 536);
								}
							}
							else if (nCmdID == 546)
							{
								if (daimonSquareScene.m_eStatus <= DaimonSquareStatus.FIGHT_STATUS_PREPARE)
								{
									string strCmd = string.Format("{0}", daimonSquareScene.m_nPlarerCount);
									GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 546, 0, 0, 0, daimonSquareScene.m_nPlarerCount);
								}
							}
						}
					}
				}
			}
		}

		public int EnterDaimonSquareSceneCopyScene(GameClient client, int nFubenID, int nDemonNum, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			if (client.ClientData.DaimonSquarePoint > 0)
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareFuBenSeqID");
				int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
				int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
				int num = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2);
				if (num == 0)
				{
					DaimonSquareDataInfo daimonSquareDataInfo = null;
					if (Data.DaimonSquareDataInfoList.TryGetValue(roleParamsInt32FromDB2, out daimonSquareDataInfo))
					{
						if (daimonSquareDataInfo == null)
						{
							client.ClientData.DaimonSquarePoint = 0;
							return 1;
						}
						string text = null;
						for (int i = 0; i < daimonSquareDataInfo.AwardItem.Length; i++)
						{
							text += daimonSquareDataInfo.AwardItem[i];
							if (i != daimonSquareDataInfo.AwardItem.Length - 1)
							{
								text += "|";
							}
						}
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							roleParamsInt32FromDB3,
							client.ClientData.DaimonSquarePoint,
							Global.CalcExpForRoleScore(client.ClientData.DaimonSquarePoint, daimonSquareDataInfo.ExpModulus),
							client.ClientData.DaimonSquarePoint * daimonSquareDataInfo.MoneyModulus,
							text
						});
						GameManager.ClientMgr.SendToClient(client, strCmd, 538);
						return -1;
					}
				}
				else
				{
					client.ClientData.DaimonSquarePoint = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
				}
			}
			int daimonSquareCopySceneIDForRole = Global.GetDaimonSquareCopySceneIDForRole(client);
			int result;
			if (daimonSquareCopySceneIDForRole <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(102, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
				LogManager.WriteLog(2, string.Format("enter bloodcastle scene fail!! get scene info fail!!!!", new object[0]), null, true);
				result = -1;
			}
			else
			{
				DaimonSquareDataInfo daimonSquareDataInfo2 = null;
				if (!Data.DaimonSquareDataInfoList.TryGetValue(daimonSquareCopySceneIDForRole, out daimonSquareDataInfo2) || daimonSquareDataInfo2 == null)
				{
					result = -1;
				}
				else if (!Global.CanEnterDaimonSquareOnTime(daimonSquareDataInfo2.BeginTime, daimonSquareDataInfo2.PrepareTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(103, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -1;
				}
				else
				{
					GoodsData goodsByID = Global.GetGoodsByID(client, daimonSquareDataInfo2.NeedGoodsID);
					if (goodsByID == null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(104, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else if (goodsByID.GCount < daimonSquareDataInfo2.NeedGoodsNum)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(105, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, daimonSquareDataInfo2.NeedGoodsID, 1, false, out flag, out flag2, false))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(106, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = -1;
						}
						else
						{
							Dictionary<int, DaimonSquareScene> dictionary = null;
							lock (this.m_DaimonSquareCopyScenesInfo)
							{
								if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(daimonSquareCopySceneIDForRole, out dictionary) && dictionary != null)
								{
									foreach (KeyValuePair<int, DaimonSquareScene> keyValuePair in dictionary)
									{
										if (keyValuePair.Value.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN)
										{
											if (keyValuePair.Value.m_nPlarerCount < daimonSquareDataInfo2.MaxPlayerNum)
											{
												keyValuePair.Value.m_nPlarerCount++;
												nSeqID = keyValuePair.Key;
											}
										}
									}
								}
								if (nSeqID < 0)
								{
									string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
									if (array != null && array.Length >= 2)
									{
										nSeqID = Global.SafeConvertToInt32(array[1]);
										if (nSeqID > 0)
										{
											DaimonSquareScene daimonSquareScene = null;
											if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(nFubenID, out dictionary) || dictionary == null)
											{
												dictionary = new Dictionary<int, DaimonSquareScene>();
												this.m_DaimonSquareCopyScenesInfo.Add(nFubenID, dictionary);
											}
											if (!dictionary.TryGetValue(nSeqID, out daimonSquareScene) || daimonSquareScene == null)
											{
												daimonSquareScene = new DaimonSquareScene();
												daimonSquareScene.CleanAllInfo();
												daimonSquareScene.m_nMapCode = mapCode;
												daimonSquareScene.m_nPlarerCount = 1;
												dictionary[nSeqID] = daimonSquareScene;
											}
										}
									}
								}
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareFuBenSeqID", nSeqID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneid", daimonSquareCopySceneIDForRole, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneFinishFlag", 0, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneTimer", 0, true);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		public void HeartBeatDaimonSquareScene()
		{
			long num = TimeUtil.NOW();
			if (Math.Abs(num - DaimonSquareCopySceneManager.LastHeartBeatTicks) >= 1000L)
			{
				DaimonSquareCopySceneManager.LastHeartBeatTicks = num;
				lock (this.m_DaimonSquareCopyScenesList)
				{
					foreach (KeyValuePair<int, CopyMap> keyValuePair in this.m_DaimonSquareCopyScenesList)
					{
						int fuBenSeqID = keyValuePair.Value.FuBenSeqID;
						int fubenMapID = keyValuePair.Value.FubenMapID;
						int mapCode = keyValuePair.Value.MapCode;
						if (fuBenSeqID >= 0 && fubenMapID >= 0 && mapCode >= 0)
						{
							lock (this.m_DaimonSquareCopyScenesInfo)
							{
								DaimonSquareDataInfo daimonSquareDataInfo = null;
								if (Data.DaimonSquareDataInfoList.TryGetValue(fubenMapID, out daimonSquareDataInfo) && daimonSquareDataInfo != null)
								{
									Dictionary<int, DaimonSquareScene> dictionary = null;
									if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(fubenMapID, out dictionary) && dictionary != null)
									{
										DaimonSquareScene daimonSquareScene = null;
										if (dictionary.TryGetValue(fuBenSeqID, out daimonSquareScene) && daimonSquareScene != null)
										{
											long num2 = TimeUtil.NOW();
											if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_NULL)
											{
												int num3 = 0;
												string s = null;
												if (Global.CanEnterDaimonSquareCopySceneOnTime(daimonSquareDataInfo.BeginTime, daimonSquareDataInfo.PrepareTime + daimonSquareDataInfo.DurationTime, out num3, out s))
												{
													daimonSquareScene.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_PREPARE;
													DateTime dateTime = DateTime.Parse(s);
													daimonSquareScene.m_lPrepareTime = dateTime.Ticks / 10000L;
													daimonSquareScene.m_nMonsterTotalWave = daimonSquareDataInfo.MonsterID.Length;
													List<GameClient> clientsList = keyValuePair.Value.GetClientsList();
													if (null == clientsList)
													{
														break;
													}
													for (int i = 0; i < clientsList.Count; i++)
													{
														this.SendMegToClient(clientsList[i], fubenMapID, fuBenSeqID, 536);
														this.SendMegToClient(clientsList[i], fubenMapID, fuBenSeqID, 546);
													}
												}
											}
											else if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_PREPARE)
											{
												if (num2 >= daimonSquareScene.m_lPrepareTime + (long)(daimonSquareDataInfo.PrepareTime * 1000))
												{
													daimonSquareScene.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_BEGIN;
													daimonSquareScene.m_lBeginTime = TimeUtil.NOW();
													int nTimer = (int)(((long)(daimonSquareDataInfo.DurationTime * 1000) - (num2 - daimonSquareScene.m_lBeginTime)) / 1000L);
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, 536, 3, nTimer, 0, 0, 0);
													List<GameClient> clientsList2 = daimonSquareScene.m_CopyMap.GetClientsList();
													if (null != clientsList2)
													{
														foreach (GameClient gameClient in clientsList2)
														{
															if (daimonSquareScene.AddRole(gameClient))
															{
																Global.UpdateRoleEnterActivityCount(gameClient, SpecialActivityTypes.DemoSque);
															}
														}
													}
												}
											}
											else if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
											{
												if (num2 < daimonSquareScene.m_lBeginTime + 20000L)
												{
													List<GameClient> clientsList2 = daimonSquareScene.m_CopyMap.GetClientsList();
													if (null != clientsList2)
													{
														foreach (GameClient gameClient in clientsList2)
														{
															if (daimonSquareScene.AddRole(gameClient))
															{
																LogManager.WriteLog(2, string.Format("EnterCount#DemoSque#rid={0}", gameClient.ClientData.RoleID), null, true);
																Global.UpdateRoleEnterActivityCount(gameClient, SpecialActivityTypes.DemoSque);
															}
														}
													}
												}
												bool flag3 = false;
												lock (daimonSquareScene.m_CreateMonsterMutex)
												{
													if (daimonSquareScene.m_nCreateMonsterFlag == 0 && daimonSquareScene.m_nMonsterWave < daimonSquareScene.m_nMonsterTotalWave)
													{
														flag3 = true;
													}
													if (num2 >= daimonSquareScene.m_lBeginTime + (long)(daimonSquareDataInfo.DurationTime * 1000) || daimonSquareScene.m_nKillMonsterTotalNum == daimonSquareDataInfo.MonsterSum)
													{
														daimonSquareScene.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
														daimonSquareScene.m_lEndTime = TimeUtil.NOW();
														try
														{
															string text = string.Format("恶魔广场已结束,是否完成{0},结束时间{1},m_nCreateMonsterFlag:{2},m_nMonsterWave:{3},m_nMonsterTotalWave:{4},m_nKillMonsterNum:{5},m_nNeedKillMonsterNum:{6}", new object[]
															{
																daimonSquareScene.m_bIsFinishTask,
																new DateTime(daimonSquareScene.m_lEndTime * 10000L),
																daimonSquareScene.m_nCreateMonsterFlag,
																daimonSquareScene.m_nMonsterWave,
																daimonSquareScene.m_nMonsterTotalWave,
																daimonSquareScene.m_nKillMonsterNum,
																daimonSquareScene.m_nNeedKillMonsterNum
															});
															LogManager.WriteLog(2, text, null, true);
														}
														catch
														{
														}
													}
												}
												if (flag3)
												{
													this.DaimonSquareSceneCreateMonster(daimonSquareScene, daimonSquareDataInfo, keyValuePair.Value.CopyMapID, keyValuePair.Value);
												}
											}
											else if (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_END)
											{
												int nTimer = (int)(((long)(daimonSquareDataInfo.LeaveTime * 1000) - (num2 - daimonSquareScene.m_lEndTime)) / 1000L);
												if (!daimonSquareScene.m_bEndFlag)
												{
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, 536, 4, nTimer, 0, 0, 0);
													long num4 = daimonSquareScene.m_lEndTime - daimonSquareScene.m_lBeginTime;
													long num5 = ((long)(daimonSquareDataInfo.DurationTime * 1000) - num4) / 1000L;
													if (num5 >= (long)daimonSquareDataInfo.DurationTime)
													{
														num5 = (long)(daimonSquareDataInfo.DurationTime / 2);
													}
													int num6 = (int)((long)daimonSquareDataInfo.TimeModulus * num5);
													if (num6 < 0)
													{
														num6 = 0;
													}
													GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, daimonSquareScene, 538, num6);
													daimonSquareScene.m_bEndFlag = true;
												}
												if (num2 >= daimonSquareScene.m_lEndTime + (long)(daimonSquareDataInfo.LeaveTime * 1000) + 3000L)
												{
													try
													{
														if (keyValuePair.Value == GameManager.CopyMapMgr.FindCopyMap(keyValuePair.Value.MapCode, keyValuePair.Value.FuBenSeqID))
														{
															if (!daimonSquareScene.ClearRole)
															{
																daimonSquareScene.ClearRole = true;
																List<GameClient> clientsList = keyValuePair.Value.GetClientsList();
																if (clientsList != null && clientsList.Count > 0)
																{
																	string text = string.Format("恶魔广场已结束,是否完成{0},结束时间{1},m_nCreateMonsterFlag:{2},m_nMonsterWave:{3},m_nMonsterTotalWave:{4},m_nKillMonsterNum:{5},m_nNeedKillMonsterNum:{6},踢出玩家列表:", new object[]
																	{
																		daimonSquareScene.m_bIsFinishTask,
																		new DateTime(daimonSquareScene.m_lEndTime * 10000L),
																		daimonSquareScene.m_nCreateMonsterFlag,
																		daimonSquareScene.m_nMonsterWave,
																		daimonSquareScene.m_nMonsterTotalWave,
																		daimonSquareScene.m_nKillMonsterNum,
																		daimonSquareScene.m_nNeedKillMonsterNum
																	});
																	LogManager.WriteLog(2, text, null, true);
																}
															}
														}
													}
													catch (Exception ex)
													{
														DataHelper.WriteExceptionLogEx(ex, "恶魔广场调度异常");
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void DaimonSquareSceneCreateMonster(DaimonSquareScene bcTmp, DaimonSquareDataInfo bcDataTmp, int nCopyMapID, CopyMap cmInfo)
		{
			string[] array = null;
			string[] array2 = null;
			lock (bcTmp.m_CreateMonsterMutex)
			{
				if (bcTmp.m_nMonsterWave >= bcTmp.m_nMonsterTotalWave)
				{
					return;
				}
				bcTmp.m_nCreateMonsterFlag = 1;
				string text = bcDataTmp.MonsterNum[bcTmp.m_nMonsterWave];
				string text2 = bcDataTmp.MonsterID[bcTmp.m_nMonsterWave];
				string text3 = bcDataTmp.CreateNextWaveMonsterCondition[bcTmp.m_nMonsterWave];
				if (text2 == null || text == null || text3 == null)
				{
					return;
				}
				array = text.Split(new char[]
				{
					','
				});
				array2 = text2.Split(new char[]
				{
					','
				});
				string[] array3 = text3.Split(new char[]
				{
					','
				});
				if (array.Length != array2.Length)
				{
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					int num = Global.SafeConvertToInt32(array[i]);
					int monsterID = Global.SafeConvertToInt32(array2[i]);
					for (int j = 0; j < num; j++)
					{
						bcTmp.m_nCreateMonsterCount++;
					}
				}
				bcTmp.m_nNeedKillMonsterNum = (int)Math.Ceiling((double)bcTmp.m_nCreateMonsterCount * Global.SafeConvertToDouble(array3[0]) / 100.0);
				bcTmp.m_nMonsterWave++;
			}
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("恶魔广场报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.posX + Global.GetRandomNumber(-bcDataTmp.Radius, bcDataTmp.Radius)) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(bcDataTmp.posZ + Global.GetRandomNumber(-bcDataTmp.Radius, bcDataTmp.Radius)) / gameMap.MapGridHeight;
				int radius = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.Radius);
				for (int i = 0; i < array.Length; i++)
				{
					int num = Global.SafeConvertToInt32(array[i]);
					int monsterID = Global.SafeConvertToInt32(array2[i]);
					for (int j = 0; j < num; j++)
					{
						GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, monsterID, nCopyMapID, 1, gridX, gridY, radius, 0, 0, null, null);
					}
				}
				GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmInfo, 537, 0, 0, bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, -100, 0);
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
			{
				DaimonSquareDataInfo daimonSquareDataInfo = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(client.ClientData.FuBenID, out daimonSquareDataInfo) && daimonSquareDataInfo != null)
				{
					Dictionary<int, DaimonSquareScene> dictionary = null;
					if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) && dictionary != null)
					{
						DaimonSquareScene daimonSquareScene = null;
						if (dictionary.TryGetValue(client.ClientData.FuBenSeqID, out daimonSquareScene) && daimonSquareScene != null)
						{
							string strCmd = string.Format("{0}:{1}", daimonSquareDataInfo.MonsterID.Length - daimonSquareScene.m_nMonsterWave, client.ClientData.DaimonSquarePoint);
							GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strCmd, 537);
						}
					}
				}
			}
		}

		public void DaimonSquareSceneKillMonster(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
			{
				DaimonSquareDataInfo daimonSquareDataInfo = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(client.ClientData.FuBenID, out daimonSquareDataInfo) && daimonSquareDataInfo != null)
				{
					Dictionary<int, DaimonSquareScene> dictionary = null;
					if (this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) && dictionary != null)
					{
						DaimonSquareScene daimonSquareScene = null;
						if (dictionary.TryGetValue(client.ClientData.FuBenSeqID, out daimonSquareScene) && daimonSquareScene != null)
						{
							if (daimonSquareScene.AddKilledMonster(monster))
							{
								if (!daimonSquareScene.m_bEndFlag)
								{
									bool flag = false;
									lock (daimonSquareScene.m_CreateMonsterMutex)
									{
										daimonSquareScene.m_nKillMonsterNum++;
										client.ClientData.DaimonSquarePoint += monster.MonsterInfo.DaimonSquareJiFen;
										if (daimonSquareScene.m_nCreateMonsterFlag == 1 && daimonSquareScene.m_nKillMonsterNum == daimonSquareScene.m_nNeedKillMonsterNum)
										{
											daimonSquareScene.m_nCreateMonsterFlag = 0;
											daimonSquareScene.m_nNeedKillMonsterNum = 0;
											daimonSquareScene.m_nKillMonsterNum = 0;
											daimonSquareScene.m_nCreateMonsterCount = 0;
										}
										if (daimonSquareScene.m_nKillMonsterTotalNum == daimonSquareDataInfo.MonsterSum)
										{
											daimonSquareScene.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
											daimonSquareScene.m_lEndTime = TimeUtil.NOW();
											daimonSquareScene.m_bIsFinishTask = true;
										}
									}
									if (flag)
									{
										DaimonSquareCopySceneManager.CompleteDaimonSquareScene(client, daimonSquareScene, daimonSquareDataInfo);
									}
									string strCmd = string.Format("{0}:{1}", daimonSquareDataInfo.MonsterID.Length - daimonSquareScene.m_nMonsterWave, client.ClientData.DaimonSquarePoint);
									GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strCmd, 537);
								}
							}
						}
					}
				}
			}
		}

		public static void CompleteDaimonSquareScene(GameClient client, DaimonSquareScene bsInfo, DaimonSquareDataInfo dsData)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
			if (roleParamsInt32FromDB != 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneFinishFlag", 1, true);
				long num = bsInfo.m_lEndTime - bsInfo.m_lBeginTime;
				long num2 = ((long)(dsData.DurationTime * 1000) - num) / 1000L;
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquareSceneTimer", (int)num2, true);
			}
		}

		public void GiveAwardDaimonSquareCopyScene(GameClient client, int nMultiple)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareFuBenSeqID");
			int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
			int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneFinishFlag");
			int roleParamsInt32FromDB4 = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneTimer");
			int num = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2);
			if (num > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				DaimonSquareDataInfo daimonSquareDataInfo = null;
				if (Data.DaimonSquareDataInfoList.TryGetValue(roleParamsInt32FromDB2, out daimonSquareDataInfo))
				{
					if (daimonSquareDataInfo != null)
					{
						if (roleParamsInt32FromDB3 == 1)
						{
							string[] awardItem = daimonSquareDataInfo.AwardItem;
							if (awardItem != null && awardItem.Length > 0)
							{
								for (int i = 0; i < awardItem.Length; i++)
								{
									if (!string.IsNullOrEmpty(awardItem[i].Trim()))
									{
										string[] array = awardItem[i].Split(new char[]
										{
											','
										});
										if (!string.IsNullOrEmpty(array[i].Trim()))
										{
											int goodsID = Convert.ToInt32(array[0].Trim());
											int num2 = Convert.ToInt32(array[1].Trim());
											int binding = 1;
											GoodsData goodsData = new GoodsData
											{
												Id = -1,
												GoodsID = goodsID,
												Using = 0,
												Forge_level = 0,
												Starttime = "1900-01-01 12:00:00",
												Endtime = "1900-01-01 12:00:00",
												Site = 0,
												Quality = 0,
												Props = "",
												GCount = num2,
												Binding = binding,
												Jewellist = "",
												BagIndex = 0,
												AddPropIndex = 0,
												BornIndex = 0,
												Lucky = 0,
												Strong = 0,
												ExcellenceInfo = 0,
												AppendPropLev = 0,
												ChangeLifeLevForEquip = 0
											};
											string goodsFromWhere = "恶魔广场--统一奖励";
											if (!Global.CanAddGoodsNum(client, num2))
											{
												Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(2, new object[0]), GLang.GetLang(2603, new object[0]), 1.0);
											}
											else
											{
												Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, num2, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, goodsFromWhere, goodsData.Endtime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
											}
										}
									}
								}
							}
						}
						int num3 = daimonSquareDataInfo.TimeModulus * roleParamsInt32FromDB4;
						if (client.ClientData.DaimonSquarePoint > 0)
						{
							long num4 = (long)nMultiple * Global.CalcExpForRoleScore(client.ClientData.DaimonSquarePoint, daimonSquareDataInfo.ExpModulus);
							int num5 = client.ClientData.DaimonSquarePoint * daimonSquareDataInfo.MoneyModulus;
							if (num4 > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, num4, false, true, false, "none");
								GameManager.ClientMgr.NotifyAddExpMsg(client, num4);
							}
							if (num5 > 0)
							{
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num5, "恶魔广场副本", false);
								GameManager.ClientMgr.NotifyAddJinBiMsg(client, num5);
							}
							if (client.ClientData.DaimonSquarePoint > client.ClientData.DaimonSquarePointTotalPoint)
							{
								client.ClientData.DaimonSquarePointTotalPoint = client.ClientData.DaimonSquarePoint;
							}
							if (client.ClientData.DaimonSquarePoint > this.m_nDaimonSquareMaxPoint)
							{
								this.SetDaimonSquareCopySceneTotalPoint(client.ClientData.RoleName, client.ClientData.DaimonSquarePoint);
							}
							client.ClientData.DaimonSquarePoint = 0;
							Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
						}
						GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2, 1);
						ProcessTask.ProcessAddTaskVal(client, TaskTypes.Daimon, -1, 1, new object[0]);
					}
				}
			}
		}

		public void LeaveDaimonSquareCopyScene(GameClient client, bool clearScore = false)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DaimonSquareSceneid");
			if (client.ClientData.CopyMapID >= 0 && client.ClientData.FuBenSeqID >= 0 && this.IsDaimonSquareCopyScene(roleParamsInt32FromDB))
			{
				CopyMap copyMap = null;
				lock (this.m_DaimonSquareCopyScenesList)
				{
					if (!this.m_DaimonSquareCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
					{
						return;
					}
				}
				Dictionary<int, DaimonSquareScene> dictionary = null;
				lock (this.m_DaimonSquareCopyScenesInfo)
				{
					if (!this.m_DaimonSquareCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) || dictionary == null)
					{
						return;
					}
					DaimonSquareScene daimonSquareScene = null;
					if (!dictionary.TryGetValue(client.ClientData.FuBenSeqID, out daimonSquareScene) || daimonSquareScene == null)
					{
						return;
					}
					Interlocked.Decrement(ref daimonSquareScene.m_nPlarerCount);
					if (daimonSquareScene.m_eStatus < DaimonSquareStatus.FIGHT_STATUS_BEGIN || (daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN && TimeUtil.NOW() < daimonSquareScene.m_lBeginTime + 30000L))
					{
						GameManager.ClientMgr.NotifyDaimonSquareCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 546, 0, 0, 0, daimonSquareScene.m_nPlarerCount);
					}
					if (clearScore && daimonSquareScene.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
					{
						client.ClientData.DaimonSquarePoint = 0;
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
			}
		}

		public void LogOutWhenInDaimonSquareCopyMap(GameClient client)
		{
			this.LeaveDaimonSquareCopyScene(client, false);
		}

		public static void CleanDaimonSquareCopyScene(int mapid)
		{
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_nDaimonSquareMaxName) && this.m_nDaimonSquareMaxName == oldName)
				{
					this.m_nDaimonSquareMaxName = newName;
				}
			}
		}

		public Dictionary<int, CopyMap> m_DaimonSquareCopyScenesList = new Dictionary<int, CopyMap>();

		public Dictionary<int, Dictionary<int, DaimonSquareScene>> m_DaimonSquareCopyScenesInfo = new Dictionary<int, Dictionary<int, DaimonSquareScene>>();

		public static object m_Mutex = new object();

		public int m_nDaimonSquareMaxPoint = -1;

		public string m_nDaimonSquareMaxName = "";

		private static long LastHeartBeatTicks = 0L;
	}
}
