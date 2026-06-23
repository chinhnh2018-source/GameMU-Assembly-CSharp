using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.Building;
using GameServer.Logic.Damon;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class YaoSaiMissionManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static YaoSaiMissionManager getInstance()
		{
			return YaoSaiMissionManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1859, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1860, 1, 1, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1861, 3, 3, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1862, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1863, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1859:
					result = this.ProcessGetMissionInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1860:
					result = this.ProcessRefreshMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1861:
					result = this.ProcessExcuteMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1862:
					result = this.ProcessQuitMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1863:
					result = this.ProcessGetMissionAwardCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ProcessGetMissionInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int rid = Global.SafeConvertToInt32(cmdParams[1]);
				List<YaoSaiMissionData> roleMissionDataList = this.GetRoleMissionDataList(rid);
				YaoSaiMissionMainData cmdData = new YaoSaiMissionMainData
				{
					MissionDataList = roleMissionDataList,
					ExcuteMissionCount = Global.GetRoleParamsInt32FromDB(client, "10180"),
					FreeRefreshTime = Global.GetRoleParamsDateTimeFromDB(client, "10181").AddSeconds((double)YaoSaiMissionManager.MissionRefreshSeconds)
				};
				client.sendCmd<YaoSaiMissionMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 获取主页面任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessRefreshMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				DateTime dateTime = TimeUtil.NowDateTime();
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10180");
				int cmdData;
				if (roleParamsInt32FromDB >= YaoSaiMissionManager.MissionCountLimit)
				{
					cmdData = 2;
				}
				else
				{
					List<YaoSaiMissionData> roleMissionWaitList = this.GetRoleMissionWaitList(rid);
					if (roleMissionWaitList == null || 0 == roleMissionWaitList.Count)
					{
						cmdData = 3;
					}
					else
					{
						bool flag = Global.GetRoleParamsDateTimeFromDB(client, "10181").AddSeconds((double)YaoSaiMissionManager.MissionRefreshSeconds) >= dateTime;
						if (flag && client.ClientData.UserMoney < YaoSaiMissionManager.RefreshMissionCost)
						{
							cmdData = 4;
						}
						else
						{
							for (int i = 0; i < roleMissionWaitList.Count; i++)
							{
								if (!this.RandomYaoSaiMission(client, roleMissionWaitList[i]))
								{
									break;
								}
							}
							if (flag && YaoSaiMissionManager.RefreshMissionCost > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiMissionManager.RefreshMissionCost, "要塞刷新任务", true, true, false, DaiBiSySType.JingLingYaoSaiShuaXin))
								{
									cmdData = 4;
									goto IL_18A;
								}
							}
							if (!flag)
							{
								Global.SaveRoleParamsDateTimeToDB(client, "10181", dateTime, true);
							}
							cmdData = this.UpdateYaoSaiMissionDataDB(client, roleMissionWaitList);
						}
					}
				}
				IL_18A:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 处理刷新任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessExcuteMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				int num = Global.GetRoleParamsInt32FromDB(client, "10180");
				int cmdData;
				if (num >= YaoSaiMissionManager.MissionCountLimit)
				{
					cmdData = 2;
				}
				else
				{
					List<YaoSaiMissionData> roleMissionDataList = this.GetRoleMissionDataList(rid);
					if (roleMissionDataList == null || 0 == roleMissionDataList.Count)
					{
						cmdData = 3;
					}
					else
					{
						YaoSaiMissionData yaoSaiMissionData = roleMissionDataList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
						if (yaoSaiMissionData == null || yaoSaiMissionData.State != 0)
						{
							cmdData = 6;
						}
						else if (!this.CanZhiPai(client, cmdParams[2]))
						{
							cmdData = 7;
						}
						else
						{
							List<YaoSaiMissionData> list = new List<YaoSaiMissionData>();
							yaoSaiMissionData.StartTime = TimeUtil.NowDateTime();
							yaoSaiMissionData.ZhiPaiJingLing = cmdParams[2];
							yaoSaiMissionData.State = 3;
							list.Add(yaoSaiMissionData);
							string[] array = cmdParams[2].Split(new char[]
							{
								'|'
							});
							foreach (string str in array)
							{
								GoodsData paiZhuDamonGoodsDataByDbID = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(client, Global.SafeConvertToInt32(str));
								string cmdData2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									3,
									paiZhuDamonGoodsDataByDbID.Id,
									paiZhuDamonGoodsDataByDbID.GoodsID,
									paiZhuDamonGoodsDataByDbID.Using,
									10001,
									paiZhuDamonGoodsDataByDbID.GCount,
									paiZhuDamonGoodsDataByDbID.BagIndex,
									""
								});
								Global.ModifyGoodsByCmdParams(client, cmdData2, "客户端修改", null);
							}
							num++;
							Global.SaveRoleParamsInt32ValueToDB(client, "10180", num, true);
							this.UpdateYaoSaiMissionSortList(client.ClientData.RoleID, yaoSaiMissionData);
							cmdData = this.UpdateYaoSaiMissionDataDB(client, list);
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "missionid=" + yaoSaiMissionData.MissionID, "要塞任务_Site=" + yaoSaiMissionData.SiteID, client.ClientData.RoleName, "系统", "精灵", array.Length, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
						}
					}
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 执行执行任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessQuitMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				List<YaoSaiMissionData> roleMissionDataList = this.GetRoleMissionDataList(rid);
				int cmdData;
				if (roleMissionDataList == null || 0 == roleMissionDataList.Count)
				{
					cmdData = 6;
				}
				else
				{
					YaoSaiMissionData yaoSaiMissionData = roleMissionDataList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
					if (yaoSaiMissionData == null || yaoSaiMissionData.State != 3)
					{
						cmdData = 9;
					}
					else
					{
						string[] array = yaoSaiMissionData.ZhiPaiJingLing.Split(new char[]
						{
							'|'
						});
						foreach (string str in array)
						{
							GoodsData paiZhuDamonGoodsDataByDbID = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(client, Global.SafeConvertToInt32(str));
							if (null != paiZhuDamonGoodsDataByDbID)
							{
								string cmdData2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									3,
									paiZhuDamonGoodsDataByDbID.Id,
									paiZhuDamonGoodsDataByDbID.GoodsID,
									paiZhuDamonGoodsDataByDbID.Using,
									10000,
									paiZhuDamonGoodsDataByDbID.GCount,
									paiZhuDamonGoodsDataByDbID.BagIndex,
									""
								});
								Global.ModifyGoodsByCmdParams(client, cmdData2, "客户端修改", null);
							}
						}
						List<YaoSaiMissionData> list = new List<YaoSaiMissionData>();
						yaoSaiMissionData.StartTime = DateTime.MinValue;
						yaoSaiMissionData.ZhiPaiJingLing = "";
						yaoSaiMissionData.State = 0;
						list.Add(yaoSaiMissionData);
						this.UpdateYaoSaiMissionSortList(client.ClientData.RoleID, yaoSaiMissionData);
						cmdData = this.UpdateYaoSaiMissionDataDB(client, list);
					}
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 执行放弃任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetMissionAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int rid = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				List<YaoSaiMissionData> roleMissionDataList = this.GetRoleMissionDataList(rid);
				int num;
				if (roleMissionDataList == null || 0 == roleMissionDataList.Count)
				{
					num = 6;
				}
				else
				{
					YaoSaiMissionData yaoSaiMissionData = roleMissionDataList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
					if (yaoSaiMissionData == null || (yaoSaiMissionData.State != 1 && yaoSaiMissionData.State != 2))
					{
						num = 8;
					}
					else
					{
						PetMissionItem petMissionItem = null;
						if (!this.PetMissionIDXmlDIct.TryGetValue(yaoSaiMissionData.MissionID, out petMissionItem))
						{
							num = 5;
						}
						else
						{
							int num2 = (yaoSaiMissionData.State == 1) ? 100 : YaoSaiMissionManager.FailAwardRate;
							List<YaoSaiMissionData> list = new List<YaoSaiMissionData>();
							yaoSaiMissionData.StartTime = DateTime.MinValue;
							yaoSaiMissionData.ZhiPaiJingLing = "";
							yaoSaiMissionData.State = 0;
							list.Add(yaoSaiMissionData);
							num = this.DeleteYaoSaiMissionDataDB(client, list);
							if (num == 0)
							{
								int num3 = petMissionItem.CrystalNum * num2 / 100;
								if (num3 > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, num3, "领取要塞任务奖励", true, true, false);
								}
								int num4 = petMissionItem.SignNum * num2 / 100;
								if (num4 > 0)
								{
									GameManager.ClientMgr.ModifyShenJiJiFenValue(client, num4, "领取要塞任务奖励", true, true);
								}
								string[] array = petMissionItem.Activator.Split(new char[]
								{
									'|'
								});
								foreach (string text in array)
								{
									string[] array3 = text.Split(new char[]
									{
										','
									});
									if (array3.Length >= 2)
									{
										int addType = Global.SafeConvertToInt32(array3[0]);
										int num5 = Global.SafeConvertToInt32(array3[1]) * num2 / 100;
										if (num5 > 0)
										{
											BuildingManager.getInstance().ModifyNengLiangPointsValue(client, addType, num5, "领取要塞任务奖励", true, true);
										}
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 执行放弃任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public List<YaoSaiMissionData> GetRoleMissionDataList(int rid)
		{
			List<YaoSaiMissionData> list = null;
			List<YaoSaiMissionData> result;
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(rid, out list))
					{
						list = Global.sendToDB<List<YaoSaiMissionData>, string>(20311, rid.ToString(), GameCoreInterface.getinstance().GetLocalServerId());
						if (null != list)
						{
							this.RunTimeData.RoleMissionCacheDict[rid] = list;
							foreach (YaoSaiMissionData yaoSaiMissionData in list)
							{
								if (yaoSaiMissionData.State == 3)
								{
									PetMissionItem petMissionItem = null;
									if (this.PetMissionIDXmlDIct.TryGetValue(yaoSaiMissionData.MissionID, out petMissionItem))
									{
										long num = yaoSaiMissionData.StartTime.Ticks / 10000L + (long)(petMissionItem.Time * 1000);
										while (this.RunTimeData.MissionSortList.ContainsKey(num))
										{
											num += 1L;
										}
										this.RunTimeData.MissionSortList.Add(num, new RoleMissionData
										{
											RoleID = rid,
											MissionData = yaoSaiMissionData
										});
									}
								}
							}
						}
					}
				}
				result = list;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 获取角色要塞任务列表信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
				result = null;
			}
			return result;
		}

		public List<YaoSaiMissionData> GetRoleMissionWaitList(int rid)
		{
			List<YaoSaiMissionData> list = new List<YaoSaiMissionData>();
			List<YaoSaiMissionData> list2 = this.GetRoleMissionDataList(rid);
			List<YaoSaiMissionData> result;
			try
			{
				if (null == list2)
				{
					list2 = new List<YaoSaiMissionData>();
				}
				int i;
				for (i = 0; i < 5; i++)
				{
					YaoSaiMissionData yaoSaiMissionData = list2.Find((YaoSaiMissionData x) => x.SiteID == i + 1);
					if (null == yaoSaiMissionData)
					{
						yaoSaiMissionData = new YaoSaiMissionData
						{
							SiteID = i + 1,
							ZhiPaiJingLing = ""
						};
					}
					if (yaoSaiMissionData.State == 0)
					{
						list.Add(yaoSaiMissionData);
					}
				}
				result = list;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 获取角色要塞未开始任务列表信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
				result = null;
			}
			return result;
		}

		public void OnLogin(GameClient client, bool isNewDay)
		{
			if (isNewDay)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10180", 0, true);
			}
		}

		public bool RandomYaoSaiMission(GameClient client, YaoSaiMissionData missionData)
		{
			bool result;
			try
			{
				int allShenJiPointNum = ShenJiFuWenManager.getInstance().GetAllShenJiPointNum(client);
				int key = 0;
				foreach (KeyValuePair<int, ShenJiLevelItem> keyValuePair in this.ShenJiLevelDict)
				{
					if (allShenJiPointNum >= keyValuePair.Value.StartValue && (allShenJiPointNum <= keyValuePair.Value.EndValue || keyValuePair.Value.EndValue < 0))
					{
						key = keyValuePair.Key;
						break;
					}
				}
				int randomNumber = Global.GetRandomNumber(1, 100001);
				List<PetMissionItem> list = null;
				if (!this.PetMissionXmlDict.TryGetValue(key, out list))
				{
					result = false;
				}
				else
				{
					foreach (PetMissionItem petMissionItem in list)
					{
						if (petMissionItem.Type == missionData.SiteID && randomNumber >= petMissionItem.RateStartVal && randomNumber <= petMissionItem.RateEndVal)
						{
							missionData.MissionID = petMissionItem.ID;
							return true;
						}
					}
					result = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 刷新角色要塞任务列表信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
				result = false;
			}
			return result;
		}

		public int UpdateYaoSaiMissionDataDB(GameClient client, List<YaoSaiMissionData> missionList)
		{
			int result;
			if (null == missionList)
			{
				result = 5;
			}
			else
			{
				int roleID = client.ClientData.RoleID;
				lock (this.RunTimeData.Mutex)
				{
					List<YaoSaiMissionData> list = null;
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(roleID, out list))
					{
						this.RunTimeData.RoleMissionCacheDict[roleID] = missionList;
					}
					else
					{
						using (List<YaoSaiMissionData>.Enumerator enumerator = missionList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								YaoSaiMissionData item = enumerator.Current;
								YaoSaiMissionData yaoSaiMissionData = list.Find((YaoSaiMissionData x) => x.SiteID == item.SiteID);
								if (null == yaoSaiMissionData)
								{
									list.Add(item);
								}
								else
								{
									yaoSaiMissionData = item;
								}
							}
						}
					}
				}
				Dictionary<int, List<YaoSaiMissionData>> dictionary = new Dictionary<int, List<YaoSaiMissionData>>();
				dictionary[roleID] = missionList;
				Global.sendToDB<int, Dictionary<int, List<YaoSaiMissionData>>>(20312, dictionary, 0);
				result = 0;
			}
			return result;
		}

		public int DeleteYaoSaiMissionDataDB(GameClient client, List<YaoSaiMissionData> missionList)
		{
			int result;
			if (null == missionList)
			{
				result = 5;
			}
			else
			{
				int roleID = client.ClientData.RoleID;
				lock (this.RunTimeData.Mutex)
				{
					List<YaoSaiMissionData> list = null;
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(roleID, out list))
					{
						return 6;
					}
					foreach (YaoSaiMissionData yaoSaiMissionData in missionList)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].SiteID == yaoSaiMissionData.SiteID)
							{
								list.RemoveAt(i);
								break;
							}
						}
					}
				}
				Dictionary<int, List<YaoSaiMissionData>> dictionary = new Dictionary<int, List<YaoSaiMissionData>>();
				dictionary[roleID] = missionList;
				Global.sendToDB<int, Dictionary<int, List<YaoSaiMissionData>>>(20313, dictionary, 0);
				result = 0;
			}
			return result;
		}

		public int UpdateYaoSaiMissionSortList(int rid, YaoSaiMissionData mission)
		{
			int result;
			if (null == mission)
			{
				result = 5;
			}
			else
			{
				PetMissionItem petMissionItem = null;
				if (!this.PetMissionIDXmlDIct.TryGetValue(mission.MissionID, out petMissionItem))
				{
					result = 5;
				}
				else
				{
					lock (this.RunTimeData.Mutex)
					{
						if (mission.State != 3)
						{
							int num = 0;
							foreach (KeyValuePair<long, RoleMissionData> keyValuePair in this.RunTimeData.MissionSortList)
							{
								if (keyValuePair.Value.RoleID == rid && keyValuePair.Value.MissionData.SiteID == mission.SiteID)
								{
									break;
								}
								num++;
							}
							if (num < this.RunTimeData.MissionSortList.Count)
							{
								this.RunTimeData.MissionSortList.RemoveAt(num);
							}
						}
						else
						{
							long num2 = mission.StartTime.Ticks / 10000L + (long)(petMissionItem.Time * 1000);
							while (this.RunTimeData.MissionSortList.ContainsKey(num2))
							{
								num2 += 1L;
							}
							this.RunTimeData.MissionSortList.Add(num2, new RoleMissionData
							{
								RoleID = rid,
								MissionData = mission
							});
						}
					}
					result = 0;
				}
			}
			return result;
		}

		public bool CanZhiPai(GameClient client, string jingLing)
		{
			bool result;
			if (string.IsNullOrEmpty(jingLing))
			{
				result = false;
			}
			else
			{
				string[] array = jingLing.Split(new char[]
				{
					'|'
				});
				List<string> list = new List<string>();
				if (array.Length < 1 || array.Length > 3)
				{
					result = false;
				}
				else
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string item = array2[i];
						if (list.Contains(item))
						{
							return false;
						}
						list.Add(item);
						if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(item)))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public int GetMissionRate(GameClient client, YaoSaiMissionData missionData)
		{
			int num = 0;
			int result;
			if (null == missionData)
			{
				result = 0;
			}
			else
			{
				try
				{
					int missionID = missionData.MissionID;
					PetMissionItem petMissionItem = null;
					if (!this.PetMissionIDXmlDIct.TryGetValue(missionID, out petMissionItem))
					{
						result = 0;
					}
					else
					{
						num += petMissionItem.SuccessRate;
						bool flag = false;
						string[] array = missionData.ZhiPaiJingLing.Split(new char[]
						{
							'|'
						});
						if (array.Length < 1 || array.Length > 3)
						{
							LogManager.WriteLog(2, string.Format("YaoSaiMission :: 计算任务成功率错误 找不到角色精灵，rid={0},siteid={1}", client.ClientData.RoleID, missionData.SiteID), null, true);
							result = 0;
						}
						else
						{
							string[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								string jingling = array2[i];
								GoodsData goodsData = client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Id == Convert.ToInt32(jingling));
								if (null == goodsData)
								{
									LogManager.WriteLog(2, string.Format("YaoSaiMission :: 计算任务成功率错误 找不到任务数据，rid={0},siteid={1}", client.ClientData.RoleID, missionData.SiteID), null, true);
									return 0;
								}
								int num2 = 1 + (1 + goodsData.Forge_level) / petMissionItem.PetLevelStep;
								int num3 = num2 * petMissionItem.PetLevelStepRate;
								num += num3;
								int num4 = 1 + Global.GetEquipExcellencePropNum(goodsData) / petMissionItem.ExcellentStep;
								int num5 = num4 * petMissionItem.ExcellentStepRate;
								num += num5;
								if (goodsData.GoodsID == petMissionItem.SpecialPet)
								{
									flag = true;
								}
							}
							if (flag)
							{
								num += petMissionItem.SpecialPetRate;
							}
							result = num + 10;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("YaoSaiMission :: 计算任务成功率错误 ex:{0}", ex.Message), null, true);
					result = 0;
				}
			}
			return result;
		}

		public void RefReshMission(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			if (Global.GetRoleParamsInt64FromDB(client, "10184") > 0L)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 已经有角色数据了还瞎刷 rid={0}", roleID), null, true);
			}
			else
			{
				List<YaoSaiMissionData> roleMissionWaitList = this.GetRoleMissionWaitList(roleID);
				int num;
				if (roleMissionWaitList == null || 0 == roleMissionWaitList.Count)
				{
					num = 3;
				}
				else
				{
					for (int i = 0; i < roleMissionWaitList.Count; i++)
					{
						if (!this.RandomYaoSaiMission(client, roleMissionWaitList[i]))
						{
							break;
						}
					}
					num = this.UpdateYaoSaiMissionDataDB(client, roleMissionWaitList);
				}
				if (num != 0)
				{
					LogManager.WriteLog(2, string.Format("YaoSaiMission :: 创建要塞时刷任务失败，rid={0}，errCode={1}", roleID, num), null, true);
				}
				else
				{
					Global.SaveRoleParamsDateTimeToDB(client, "10184", TimeUtil.NowDateTime(), true);
					Global.SaveRoleParamsDateTimeToDB(client, "10181", TimeUtil.NowDateTime(), true);
				}
			}
		}

		public void YaoSaiMissionTimer_Work()
		{
			long num = TimeUtil.NOW();
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					IList<long> keys = this.RunTimeData.MissionSortList.Keys;
					int i = 0;
					while (i < keys.Count)
					{
						Dictionary<int, YaoSaiMissionData> dictionary = new Dictionary<int, YaoSaiMissionData>();
						if (num < keys[i])
						{
							break;
						}
						try
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(this.RunTimeData.MissionSortList[keys[i]].RoleID);
							if (null != gameClient)
							{
								int randomNumber = Global.GetRandomNumber(0, 101);
								int missionRate = this.GetMissionRate(gameClient, this.RunTimeData.MissionSortList[keys[i]].MissionData);
								if (missionRate >= randomNumber)
								{
									this.RunTimeData.MissionSortList[keys[i]].MissionData.State = 1;
								}
								else
								{
									this.RunTimeData.MissionSortList[keys[i]].MissionData.State = 2;
								}
								YaoSaiMissionData missionData = this.RunTimeData.MissionSortList[keys[i]].MissionData;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "missionid=" + missionData.MissionID, "要塞任务_Site=" + missionData.SiteID, gameClient.ClientData.RoleName, "系统", "完成", missionData.State, gameClient.ClientData.ZoneID, gameClient.strUserID, -1, gameClient.ServerId, null);
								string[] array = this.RunTimeData.MissionSortList[keys[i]].MissionData.ZhiPaiJingLing.Split(new char[]
								{
									'|'
								});
								foreach (string text in array)
								{
									if (!string.IsNullOrEmpty(text))
									{
										GoodsData paiZhuDamonGoodsDataByDbID = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(gameClient, Global.SafeConvertToInt32(text));
										if (null != paiZhuDamonGoodsDataByDbID)
										{
											string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
											{
												gameClient.ClientData.RoleID,
												3,
												paiZhuDamonGoodsDataByDbID.Id,
												paiZhuDamonGoodsDataByDbID.GoodsID,
												paiZhuDamonGoodsDataByDbID.Using,
												10000,
												paiZhuDamonGoodsDataByDbID.GCount,
												paiZhuDamonGoodsDataByDbID.BagIndex,
												""
											});
											Global.ModifyGoodsByCmdParams(gameClient, cmdData, "客户端修改", null);
										}
									}
								}
								this.UpdateYaoSaiMissionDataDB(gameClient, new List<YaoSaiMissionData>
								{
									this.RunTimeData.MissionSortList[keys[i]].MissionData
								});
								this.RunTimeData.MissionSortList.RemoveAt(i);
								i--;
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(2, string.Format("YaoSaiMission :: 定时器错误 ex:{0}", ex.Message), null, true);
						}
						IL_361:
						i++;
						continue;
						goto IL_361;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("YaoSaiMission :: 定时器刷新要塞任务信息错误 ex:{}", ex.Message), null, true);
			}
		}

		public void LoadConfig()
		{
			this.LoadPetMissionXml();
			this.LoadSystemParams();
		}

		public void LoadSystemParams()
		{
			try
			{
				string[] array = GameManager.systemParamsList.GetParamValueByName("PetMissionMax").Split(new char[]
				{
					','
				});
				if (array.Length < 2)
				{
					LogManager.WriteLog(2, string.Format("YaoSaiMission :: 配置表配置出错 PetMissionMax", new object[0]), null, true);
				}
				else
				{
					YaoSaiMissionManager.MissionCountLimit = Global.SafeConvertToInt32(array[0]);
					YaoSaiMissionManager.MissionRefreshSeconds = Global.SafeConvertToInt32(array[1]);
					YaoSaiMissionManager.RefreshMissionCost = Global.SafeConvertToInt32(GameManager.systemParamsList.GetParamValueByName("RefreshMissionCost"));
					YaoSaiMissionManager.FailAwardRate = Global.SafeConvertToInt32(GameManager.systemParamsList.GetParamValueByName("FailAwardRate"));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", "SystemParms.xml", ex.Message), ex, true);
			}
		}

		public void LoadRunTimeDataFromDB()
		{
			int num = 0;
			for (;;)
			{
				try
				{
					Dictionary<int, List<YaoSaiMissionData>> dictionary = Global.sendToDB<Dictionary<int, List<YaoSaiMissionData>>, int>(20311, 0, GameCoreInterface.getinstance().GetLocalServerId());
					lock (this.RunTimeData.Mutex)
					{
						if (null != dictionary)
						{
							this.RunTimeData.RoleMissionCacheDict = dictionary;
							foreach (KeyValuePair<int, List<YaoSaiMissionData>> keyValuePair in dictionary)
							{
								if (null != keyValuePair.Value)
								{
									foreach (YaoSaiMissionData yaoSaiMissionData in keyValuePair.Value)
									{
										if (yaoSaiMissionData.State == 3)
										{
											PetMissionItem petMissionItem = null;
											if (this.PetMissionIDXmlDIct.TryGetValue(yaoSaiMissionData.MissionID, out petMissionItem))
											{
												long num2 = yaoSaiMissionData.StartTime.Ticks / 10000L + (long)(petMissionItem.Time * 1000);
												while (this.RunTimeData.MissionSortList.ContainsKey(num2))
												{
													num2 += 1L;
												}
												this.RunTimeData.MissionSortList.Add(num2, new RoleMissionData
												{
													RoleID = keyValuePair.Key,
													MissionData = yaoSaiMissionData
												});
											}
										}
									}
								}
							}
							break;
						}
						if (++num < 10)
						{
							Thread.Sleep(1000);
						}
						else
						{
							LogManager.WriteLog(1000, string.Format("YaoSaiMission :: 初始化数据库数据失败。", new object[0]), null, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("YaoSaiMission :: 获取数据库数据失败。ex:{0}", ex.Message), null, true);
				}
			}
		}

		public void LoadPetMissionXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath("Config\\PetMission.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements("SignLevel");
					if (null != enumerable)
					{
						this.ShenJiLevelDict.Clear();
						this.PetMissionXmlDict.Clear();
						foreach (XElement xelement2 in enumerable)
						{
							int key = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "LevelID", "0"));
							this.ShenJiLevelDict[key] = new ShenJiLevelItem
							{
								StartValue = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "SignLevelStart", "0")),
								EndValue = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "SignLevelEnd", "0"))
							};
							IEnumerable<XElement> enumerable2 = xelement2.Elements("PetMission");
							if (null == enumerable2)
							{
								break;
							}
							foreach (XElement xml in enumerable2)
							{
								int num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
								string[] array = Global.GetDefAttributeStr(xml, "RateInterval", "").Split(new char[]
								{
									','
								});
								if (array.Length < 2)
								{
									array = new string[]
									{
										"0",
										"0"
									};
								}
								if (!this.PetMissionXmlDict.ContainsKey(key))
								{
									this.PetMissionXmlDict[key] = new List<PetMissionItem>();
								}
								PetMissionItem petMissionItem = new PetMissionItem
								{
									ID = num,
									Type = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "Type", "0")),
									RateStartVal = Global.SafeConvertToInt32(array[0]),
									RateEndVal = Global.SafeConvertToInt32(array[1]),
									SuccessRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "SuccessRate", "0")),
									PetLevelStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "PetLevelStep", "0")),
									PetLevelStepRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "PetLevelStepRate", "0")),
									ExcellentStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "ExcellentStep", "0")),
									ExcellentStepRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "ExcellentStepRate", "0")),
									SpecialPet = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "SpecialPet", "0")),
									SpecialPetRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "SpecialPetRate", "0")),
									Time = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "Time", "0")),
									CrystalNum = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "CrystalNum", "0")),
									SignNum = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xml, "SignNum", "0")),
									Activator = Global.GetDefAttributeStr(xml, "Activator", "")
								};
								this.PetMissionXmlDict[key].Add(petMissionItem);
								this.PetMissionIDXmlDIct[num] = petMissionItem;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", text, ex.Message), ex, true);
			}
		}

		public YaoSaiMissionRunTimeData RunTimeData = new YaoSaiMissionRunTimeData();

		public Dictionary<int, ShenJiLevelItem> ShenJiLevelDict = new Dictionary<int, ShenJiLevelItem>();

		public Dictionary<int, List<PetMissionItem>> PetMissionXmlDict = new Dictionary<int, List<PetMissionItem>>();

		public Dictionary<int, PetMissionItem> PetMissionIDXmlDIct = new Dictionary<int, PetMissionItem>();

		public static int MissionCountLimit = 0;

		public static int MissionRefreshSeconds = 0;

		public static int RefreshMissionCost = 0;

		public static int FailAwardRate = 0;

		private static YaoSaiMissionManager instance = new YaoSaiMissionManager();
	}
}
