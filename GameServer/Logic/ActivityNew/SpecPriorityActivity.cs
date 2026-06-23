using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Logic.UserMoneyCharge;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.ActivityNew
{
	public class SpecPriorityActivity : Activity, IEventListener
	{
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int num = -1;
				int num2 = -1;
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in chargeItemBaseEventObject.Player.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB value = keyValuePair.Value;
					SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(keyValuePair.Value.TeQuanID, keyValuePair.Value.ActID);
					if (specPActivityConfig != null && specPActivityConfig.ActType == 3 && specPActivityConfig.ZhiGouID == chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID)
					{
						num = value.TeQuanID;
						num2 = value.ActID;
						break;
					}
				}
				if (num != -1 && num2 != -1)
				{
					string cmdData = this.BuildFetchSpecActAwardCmd(chargeItemBaseEventObject.Player, 0, num, num2);
					chargeItemBaseEventObject.Player.sendCmd<string>(1497, cmdData, false);
				}
				this.ConditionNumTrigger(chargeItemBaseEventObject.Player.strUserID, chargeItemBaseEventObject.Player.ClientData.RoleID, 3, chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID, null);
				this.ConditionNumTrigger(chargeItemBaseEventObject.Player.strUserID, chargeItemBaseEventObject.Player.ClientData.RoleID, 4, chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID, null);
			}
		}

		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			int param = Global.TransMoneyToYuanBao(addMoney);
			this.ConditionNumTrigger(userid, roleid, 1, param, null);
			this.ConditionNumTrigger(userid, roleid, 2, param, null);
			this.ConditionNumTrigger(userid, roleid, 7, param, null);
			this.ConditionNumTrigger(userid, roleid, 8, param, null);
		}

		public void MoneyConst(GameClient client, int YuanBaoCost)
		{
			this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, 9, YuanBaoCost, client);
			this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, 10, YuanBaoCost, client);
		}

		public bool IsMultiOpen(SpecPActivityBuffType type, out SpecPActivityConfig proto)
		{
			proto = null;
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			bool result;
			if (list.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (SpecPConditionConfig specPConditionConfig in list)
				{
					foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
						{
							List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
							foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
							{
								if (specPActivityConfig.ActType == 5)
								{
									if (specPActivityConfig.Param1 == (int)type)
									{
										proto = specPActivityConfig;
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		public double GetMult(SpecPActivityBuffType type)
		{
			SpecPActivityConfig specPActivityConfig;
			double result;
			if (!this.IsMultiOpen(type, out specPActivityConfig))
			{
				result = 0.0;
			}
			else
			{
				result = specPActivityConfig.MultiNum;
			}
			return result;
		}

		public string MackHongBaoActivityKeyStr(DateTime FromDate, DateTime ToDate)
		{
			return string.Format("SP_{0}_{1}", FromDate.ToString("yyyy-MM-dd HH$mm$ss"), ToDate.ToString("yyyy-MM-dd HH$mm$ss"));
		}

		public HongBaoListQueryData QueryHongBaoList()
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			HongBaoListQueryData result;
			if (list.Count == 0)
			{
				result = null;
			}
			else
			{
				string keyStr = this.MackHongBaoActivityKeyStr(list[0].FromDate, list[0].ToDate);
				try
				{
					HongBaoListQueryData cmd = new HongBaoListQueryData
					{
						KeyStr = keyStr
					};
					return Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1437, cmd, 0);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = null;
			}
			return result;
		}

		public List<HongBaoSendData> SendHongBaoProc(DateTime now, Dictionary<int, HongBaoSendData> dict)
		{
			List<HongBaoSendData> result;
			if (GameManager.IsKuaFuServer)
			{
				result = null;
			}
			else
			{
				List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
				if (list.Count == 0)
				{
					result = null;
				}
				else
				{
					string sender = this.MackHongBaoActivityKeyStr(list[0].FromDate, list[0].ToDate);
					List<HongBaoSendData> list2 = new List<HongBaoSendData>();
					lock (SpecPriorityActivity.Mutex)
					{
						foreach (SpecPConditionConfig specPConditionConfig in list)
						{
							foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
							{
								if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
								{
									List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
									foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
									{
										if (specPActivityConfig.ActType == 6)
										{
											if (!this.HongBaoIdSended.Contains(specPActivityConfig.ActID))
											{
												DateTime toDate = list[0].ToDate;
												if (!(now >= toDate))
												{
													foreach (HongBaoSendData hongBaoSendData in dict.Values)
													{
														if (hongBaoSendData.senderID == specPActivityConfig.ActID)
														{
															this.HongBaoIdSended.Add(specPActivityConfig.ActID);
														}
													}
													if (!this.HongBaoIdSended.Contains(specPActivityConfig.ActID))
													{
														HongBaoSendData hongBaoSendData2 = new HongBaoSendData();
														hongBaoSendData2.senderID = specPActivityConfig.ActID;
														hongBaoSendData2.sender = sender;
														hongBaoSendData2.sendTime = list[0].FromDate;
														hongBaoSendData2.type = 103;
														hongBaoSendData2.endTime = toDate;
														hongBaoSendData2.message = this.RedPacketsTeQuanMessage;
														hongBaoSendData2.sumDiamondNum = specPActivityConfig.Param1;
														hongBaoSendData2.leftZuanShi = specPActivityConfig.Param1;
														int num = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData2, GameManager.ServerId);
														if (num > 0)
														{
															hongBaoSendData2.hongBaoID = num;
															this.HongBaoIdSended.Add(specPActivityConfig.ActID);
															list2.Add(hongBaoSendData2);
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
					result = list2;
				}
			}
			return result;
		}

		public bool CanGetHongBao(GameClient client, HongBaoSendData hongBaoData)
		{
			bool result = false;
			lock (SpecPriorityActivity.Mutex)
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB value = keyValuePair.Value;
					SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(value.TeQuanID, value.ActID);
					if (specPActivityConfig != null && specPActivityConfig.ActType == 6)
					{
						if (value.ActID == hongBaoData.senderID && value.PurNum == 0)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		public bool OnRecvHongBao(GameClient client, HongBaoSendData hongBaoData)
		{
			bool result = false;
			lock (SpecPriorityActivity.Mutex)
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB value = keyValuePair.Value;
					SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(value.TeQuanID, value.ActID);
					if (specPActivityConfig != null && specPActivityConfig.ActType == 6)
					{
						if (value.ActID == hongBaoData.senderID)
						{
							result = true;
							value.PurNum = 1;
							this.UpdateClientSpecPriorityActData(client, value);
							break;
						}
					}
				}
			}
			return result;
		}

		public int OpenHongBao(int id)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			int result;
			if (list.Count == 0)
			{
				result = 0;
			}
			else
			{
				lock (SpecPriorityActivity.Mutex)
				{
					foreach (SpecPConditionConfig specPConditionConfig in list)
					{
						foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
						{
							if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
							{
								List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
								foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
								{
									if (specPActivityConfig.ActType == 6)
									{
										if (id == specPActivityConfig.ActID)
										{
											return Global.GetRandomNumber(specPActivityConfig.HongBaoRange[0], specPActivityConfig.HongBaoRange[1]);
										}
									}
								}
							}
						}
					}
				}
				result = 0;
			}
			return result;
		}

		public bool IsChouJiangOpen(SpecPActivityChouJiangType type)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			bool result;
			if (list.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (SpecPConditionConfig specPConditionConfig in list)
				{
					foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
						{
							List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
							foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
							{
								if (specPActivityConfig.ActType == 7)
								{
									if (specPActivityConfig.ChouJiangTypeSet.Contains((int)type))
									{
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		public bool CheckIconState(GameClient client)
		{
			bool flag = false;
			bool result;
			if (client.ClientData.SpecPriorityActInfoDict == null || client.ClientData.SpecPriorityActInfoDict.Count == 0)
			{
				result = flag;
			}
			else
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB value = keyValuePair.Value;
					SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(value.TeQuanID, value.ActID);
					if (specPActivityConfig != null && specPActivityConfig.ActType == 2)
					{
						int num = this.SpecActCheckCondition(client, value.TeQuanID, value.ActID, 1, false);
						if (num == 0)
						{
							flag = true;
							break;
						}
					}
				}
				result = flag;
			}
			return result;
		}

		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			this.GenerateSpecialPriorityActivity(client);
			this.NotifyActivityState(client);
			this.AutoGiveSpecialAward(client, isLogin);
		}

		public void AutoGiveSpecialAward(GameClient client, bool isLogin = false)
		{
			if (null != client.ClientData.SpecPriorityActInfoDict)
			{
				int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				lock (client.ClientData.SpecPriorityActMutex)
				{
					foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in client.ClientData.SpecPriorityActInfoDict)
					{
						SpecPriorityActInfoDB value = keyValuePair.Value;
						SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(value.TeQuanID, value.ActID);
						if (null != specPActivityConfig)
						{
							if (specPActivityConfig.ActType == 5 && specPActivityConfig.Param1 == 7)
							{
								int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
								int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "GuMuAwardDayID");
								if (value.PurNum != offsetDay && dayOfYear == roleParamsInt32FromDB)
								{
									value.PurNum = offsetDay;
									int num = (int)specPActivityConfig.MultiNum * 60;
									Global.AddGuMuMapTime(client, Global.GetAutoGiveGuMuTime(client) + num, 0);
									this.UpdateClientSpecPriorityActData(client, value);
								}
							}
						}
					}
				}
			}
		}

		public int Donate(GameClient client, int groupID, int useMoney)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			int result;
			if (list.Count == 0)
			{
				result = -12;
			}
			else
			{
				SpecPConditionConfig specPConditionConfig = list.Find((SpecPConditionConfig x) => x.GroupID == groupID);
				if (specPConditionConfig == null || (specPConditionConfig.ConditionType != 5 && specPConditionConfig.ConditionType != 6))
				{
					result = -12;
				}
				else
				{
					List<int> specPRoleInfo = this.GetSpecPRoleInfo(client);
					if (specPConditionConfig.ConditionType == 5)
					{
						if (specPConditionConfig.EveryDayFinishNum > 0 && specPRoleInfo[1] >= specPConditionConfig.EveryDayFinishNum)
						{
							return -12;
						}
					}
					else if (specPConditionConfig.EveryDayFinishNum > 0 && specPRoleInfo[2] >= specPConditionConfig.EveryDayFinishNum)
					{
						return -12;
					}
					List<GoodsData> list2 = null;
					List<FallGoodsItem> list3 = specPConditionConfig.fallGoodsItemList as List<FallGoodsItem>;
					if (null != list3)
					{
						List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(list3, 1, 1, 1.0);
						if (fallGoodsItemByPercent != null && fallGoodsItemByPercent.Count > 0)
						{
							list2 = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						}
					}
					int num;
					if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, list2, out num))
					{
						if (num == 1)
						{
							result = -101;
						}
						else
						{
							result = -100;
						}
					}
					else
					{
						if (useMoney > 0)
						{
							if (client.ClientData.UserMoney < specPConditionConfig.ConditionList[2])
							{
								return -10;
							}
							if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, specPConditionConfig.ConditionList[2], "特权活动Donate", true, true, false, DaiBiSySType.None))
							{
								return -10;
							}
						}
						else
						{
							int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, specPConditionConfig.ConditionList[0]);
							if (totalGoodsCountByID < specPConditionConfig.ConditionList[1])
							{
								return -6;
							}
							bool flag = false;
							bool flag2 = false;
							GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, specPConditionConfig.ConditionList[0], specPConditionConfig.ConditionList[1], false, out flag, out flag2, false);
						}
						if (null != list2)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list2[i].GoodsID, list2[i].GCount, list2[i].Quality, list2[i].Props, list2[i].Forge_level, list2[i].Binding, 0, "", true, 1, "特权活动捐献", list2[i].Endtime, list2[i].AddPropIndex, list2[i].BornIndex, list2[i].Lucky, list2[i].Strong, list2[i].ExcellenceInfo, list2[i].AppendPropLev, list2[i].ChangeLifeLevForEquip, null, null, 0, true);
							}
						}
						if (specPConditionConfig.ConditionType == 5)
						{
							this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, 5, -1, null);
							List<int> list4;
							(list4 = specPRoleInfo)[1] = list4[1] + 1;
						}
						else
						{
							this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, 6, -1, null);
							List<int> list4;
							(list4 = specPRoleInfo)[2] = list4[2] + 1;
						}
						this.SaveSpecPRoleInfo(client, specPRoleInfo);
						result = 0;
					}
				}
			}
			return result;
		}

		public SpecPriorityActivityData GetSpecPriorityActivityDataForClient(GameClient client)
		{
			SpecPriorityActivityData specPriorityActivityData = new SpecPriorityActivityData();
			SpecPriorityActivityData result;
			if (null == client.ClientData.SpecPriorityActInfoDict)
			{
				result = specPriorityActivityData;
			}
			else
			{
				lock (SpecPriorityActivity.Mutex)
				{
					specPriorityActivityData.ConditionDict = this.ActConditionInfoDict;
				}
				List<int> specPRoleInfo = this.GetSpecPRoleInfo(client);
				specPriorityActivityData.DonateNum = specPRoleInfo[1];
				specPriorityActivityData.DonateNumKF = specPRoleInfo[2];
				lock (client.ClientData.SpecPriorityActMutex)
				{
					foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in client.ClientData.SpecPriorityActInfoDict)
					{
						SpecPriorityActInfoDB value = keyValuePair.Value;
						SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(keyValuePair.Value.TeQuanID, keyValuePair.Value.ActID);
						if (null != specPActivityConfig)
						{
							SpecPriorityActInfo specPriorityActInfo = new SpecPriorityActInfo();
							specPriorityActInfo.TeQuanID = value.TeQuanID;
							specPriorityActInfo.ActID = value.ActID;
							if (specPActivityConfig.ActType == 2)
							{
								List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
								if (list.Count != 0)
								{
									SpecPConditionConfig specPConditionConfig = list[0];
									specPriorityActInfo.ShowNum = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(client, specPConditionConfig.FromDate, specPConditionConfig.ToDate);
								}
							}
							int num = value.PurNum;
							if (specPActivityConfig.ActType == 3)
							{
								num = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, specPActivityConfig.ZhiGouID);
							}
							if (specPActivityConfig.PurchaseNum == -1)
							{
								specPriorityActInfo.State = ((num == 1) ? 1 : 0);
							}
							else
							{
								specPriorityActInfo.LeftPurNum = specPActivityConfig.PurchaseNum - num;
								specPriorityActInfo.State = ((specPriorityActInfo.LeftPurNum <= 0) ? 1 : 0);
								if (specPriorityActInfo.LeftPurNum < 0)
								{
									specPriorityActInfo.LeftPurNum = 0;
								}
							}
							if (specPActivityConfig.ActType == 2)
							{
								if (specPriorityActInfo.ShowNum < specPActivityConfig.Param1)
								{
									specPriorityActInfo.State = -1;
								}
							}
							specPriorityActivityData.SpecActInfoList.Add(specPriorityActInfo);
						}
					}
				}
				result = specPriorityActivityData;
			}
			return result;
		}

		public bool CanActiveTeQuanID(int groupID, int tequanID)
		{
			int num = 0;
			SpecPActiveConfig specPActiveConfig = null;
			lock (SpecPriorityActivity.Mutex)
			{
				if (!this.SpecPActiveDict.TryGetValue(tequanID, out specPActiveConfig))
				{
					return false;
				}
				if (!this.ActConditionInfoDict.TryGetValue(groupID, out num))
				{
					return false;
				}
			}
			return num >= specPActiveConfig.ConditonNum;
		}

		public void GenerateSpecialPriorityActivity(GameClient client)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				DateTime now = TimeUtil.NowDateTime();
				List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
				lock (client.ClientData.SpecPriorityActMutex)
				{
					if (null == client.ClientData.SpecPriorityActInfoDict)
					{
						client.ClientData.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
					}
					Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> dictionary = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>(client.ClientData.SpecPriorityActInfoDict);
					using (Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>.Enumerator enumerator = client.ClientData.SpecPriorityActInfoDict.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp = enumerator.Current;
							bool flag2;
							if (!list.Exists(delegate(SpecPConditionConfig x)
							{
								IEnumerable<int> activeSpecPList2 = x.ActiveSpecPList;
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp2 = kvp;
								return activeSpecPList2.Contains(kvp2.Value.TeQuanID);
							}))
							{
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp3 = kvp;
								flag2 = (kvp3.Value.TeQuanID > 0);
							}
							else
							{
								flag2 = false;
							}
							if (flag2)
							{
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp3 = kvp;
								this.DeleteClientSpecPriorityActData(client, kvp3.Value.TeQuanID);
								Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> dictionary2 = dictionary;
								kvp3 = kvp;
								dictionary2.Remove(kvp3.Key);
							}
						}
					}
					foreach (SpecPConditionConfig specPConditionConfig in list)
					{
						foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
						{
							if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
							{
								List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
								foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
								{
									if (specPActivityConfig.ActType != 1)
									{
										KeyValuePair<int, int> key = new KeyValuePair<int, int>(specPActivityConfig.TeQuanID, specPActivityConfig.ActID);
										SpecPriorityActInfoDB specPriorityActInfoDB = null;
										if (!dictionary.TryGetValue(key, out specPriorityActInfoDB))
										{
											specPriorityActInfoDB = new SpecPriorityActInfoDB
											{
												TeQuanID = specPActivityConfig.TeQuanID,
												ActID = specPActivityConfig.ActID
											};
											dictionary[key] = specPriorityActInfoDB;
											this.UpdateClientSpecPriorityActData(client, specPriorityActInfoDB);
										}
									}
								}
							}
						}
					}
					for (int j = -2; j < 0; j++)
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(0, j);
						SpecPriorityActInfoDB specPriorityActInfoDB = null;
						if (!dictionary.TryGetValue(key, out specPriorityActInfoDB))
						{
							specPriorityActInfoDB = new SpecPriorityActInfoDB
							{
								TeQuanID = 0,
								ActID = j
							};
							dictionary[key] = specPriorityActInfoDB;
							this.UpdateClientSpecPriorityActData(client, specPriorityActInfoDB);
						}
					}
					client.ClientData.SpecPriorityActInfoDict = dictionary;
				}
			}
		}

		public void NotifyActivityState(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			bool flag = false;
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			if (list.Count != 0)
			{
				flag = true;
			}
			if (client.ClientSocket.IsKuaFuLogin)
			{
				flag = false;
			}
			if (flag)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					17,
					1,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					17,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public int SpecActCheckCondition(GameClient client, int TeQuanID, int ActID, int PurNum, bool CheckCost = true)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			int result;
			if (list.Count == 0)
			{
				result = -2001;
			}
			else
			{
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(TeQuanID, ActID);
				SpecPriorityActInfoDB specPriorityActInfoDB = null;
				if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
				{
					result = -2;
				}
				else
				{
					SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
					if (null == specPActivityConfig)
					{
						result = -2;
					}
					else if (specPActivityConfig.ActType == 3 || specPActivityConfig.ActType == 1)
					{
						result = -12;
					}
					else if (PurNum <= 0)
					{
						result = -12;
					}
					else
					{
						if (specPActivityConfig.PurchaseNum == -1)
						{
							if (specPriorityActInfoDB.PurNum == 1)
							{
								return -200;
							}
						}
						else if (specPActivityConfig.PurchaseNum - specPriorityActInfoDB.PurNum < PurNum)
						{
							return -200;
						}
						if (CheckCost && specPActivityConfig.ActType == 4)
						{
							if (client.ClientData.UserMoney < specPActivityConfig.Param1 * PurNum)
							{
								return -10;
							}
						}
						if (specPActivityConfig.ActType == 2)
						{
							SpecPConditionConfig specPConditionConfig = list[0];
							int num = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(client, specPConditionConfig.FromDate, specPConditionConfig.ToDate);
							if (num < specPActivityConfig.Param1)
							{
								return -10;
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public bool HasEnoughBagSpaceForAwardGoods(GameClient client, int TeQuanID, int ActID, int PurNum)
		{
			KeyValuePair<int, int> key = new KeyValuePair<int, int>(TeQuanID, ActID);
			SpecPriorityActInfoDB specPriorityActInfoDB = null;
			bool result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
			{
				result = false;
			}
			else
			{
				SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
				if (null == specPActivityConfig)
				{
					result = false;
				}
				else
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> list = new List<GoodsData>();
					foreach (GoodsData goodsData in specPActivityConfig.GoodsDataListOne)
					{
						GoodsData goodsData2 = new GoodsData(goodsData);
						goodsData2.GCount *= PurNum;
						list.Add(goodsData2);
					}
					int count = specPActivityConfig.GoodsDataListTwo.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData goodsData3 = specPActivityConfig.GoodsDataListTwo[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, goodsData3.GoodsID))
						{
							GoodsData goodsData2 = new GoodsData(goodsData3);
							goodsData2.GCount *= PurNum;
							list.Add(goodsData2);
						}
					}
					AwardItem awardItem = specPActivityConfig.GoodsDataListThr.ToAwardItem();
					foreach (GoodsData goodsData in awardItem.GoodsDataList)
					{
						GoodsData goodsData2 = new GoodsData(goodsData);
						goodsData2.Starttime = goodsData.Starttime;
						goodsData2.Endtime = goodsData.Endtime;
						goodsData2.GCount *= PurNum;
						list.Add(goodsData2);
					}
					result = Global.CanAddGoodsDataList(client, list);
				}
			}
			return result;
		}

		public int SpecActGiveAward(GameClient client, int TeQuanID, int ActID, int PurNum)
		{
			KeyValuePair<int, int> key = new KeyValuePair<int, int>(TeQuanID, ActID);
			SpecPriorityActInfoDB specPriorityActInfoDB = null;
			int result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
			{
				result = -2;
			}
			else
			{
				SpecPActivityConfig specPActivityConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
				if (null == specPActivityConfig)
				{
					result = -2;
				}
				else
				{
					string castResList = "";
					if (specPActivityConfig.ActType == 4 && specPActivityConfig.Param1 > 0)
					{
						int num = specPActivityConfig.Param1 * PurNum;
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "特权活动Mall", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
						castResList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-num,
							client.ClientData.UserMoney + num,
							client.ClientData.UserMoney
						});
					}
					AwardItem awardItem = new AwardItem();
					awardItem.GoodsDataList.Clear();
					foreach (GoodsData goodsData in specPActivityConfig.GoodsDataListOne)
					{
						GoodsData goodsData2 = new GoodsData(goodsData);
						goodsData2.GCount *= PurNum;
						awardItem.GoodsDataList.Add(goodsData2);
					}
					base.GiveAward(client, awardItem);
					awardItem.GoodsDataList.Clear();
					foreach (GoodsData goodsData in specPActivityConfig.GoodsDataListTwo)
					{
						GoodsData goodsData2 = new GoodsData(goodsData);
						goodsData2.GCount *= PurNum;
						awardItem.GoodsDataList.Add(goodsData2);
					}
					base.GiveAward(client, awardItem);
					awardItem.GoodsDataList.Clear();
					foreach (GoodsData goodsData in specPActivityConfig.GoodsDataListThr.ToAwardItem().GoodsDataList)
					{
						GoodsData goodsData2 = new GoodsData(goodsData);
						goodsData.GCount *= PurNum;
						awardItem.GoodsDataList.Add(goodsData2);
					}
					base.GiveEffectiveTimeAward(client, awardItem);
					if (specPActivityConfig.PurchaseNum == -1)
					{
						specPriorityActInfoDB.PurNum = 1;
					}
					else
					{
						specPriorityActInfoDB.PurNum += PurNum;
					}
					this.UpdateClientSpecPriorityActData(client, specPriorityActInfoDB);
					if (client._IconStateMgr.CheckSpecPriorityActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string strResList = EventLogManager.MakeGoodsDataPropString(awardItem.GoodsDataList);
					EventLogManager.AddPurchaseEvent(client, 9, specPriorityActInfoDB.ActID, castResList, strResList);
					result = 0;
				}
			}
			return result;
		}

		public string BuildFetchSpecActAwardCmd(GameClient client, int ErrCode, int tequanID, int actID)
		{
			int roleID = client.ClientData.RoleID;
			KeyValuePair<int, int> key = new KeyValuePair<int, int>(tequanID, actID);
			SpecPriorityActInfoDB specPriorityActInfoDB = null;
			string result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					-2,
					roleID,
					tequanID,
					actID,
					0,
					0
				});
			}
			else
			{
				List<SpecPActivityConfig> list = null;
				if (!this.SpecPActivityDict.TryGetValue(tequanID, out list))
				{
					result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-2,
						roleID,
						tequanID,
						actID,
						0,
						0
					});
				}
				else
				{
					SpecPActivityConfig specPActivityConfig = list.Find((SpecPActivityConfig x) => x.ActID == actID);
					if (null == specPActivityConfig)
					{
						result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							-2,
							roleID,
							tequanID,
							actID,
							0,
							0
						});
					}
					else
					{
						if (specPActivityConfig.ActType == 3)
						{
							specPriorityActInfoDB.PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, specPActivityConfig.ZhiGouID);
						}
						int num = specPActivityConfig.PurchaseNum - specPriorityActInfoDB.PurNum;
						int num2 = 0;
						result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							ErrCode,
							roleID,
							tequanID,
							actID,
							num,
							num2
						});
					}
				}
			}
			return result;
		}

		public void ModifySpecialPriorityActConitionInfo(int key, int add)
		{
			lock (SpecPriorityActivity.Mutex)
			{
				int num = 0;
				if (this.ActConditionInfoDict.TryGetValue(key, out num))
				{
					num += add;
				}
				else
				{
					num = add;
				}
				this.ActConditionInfoDict[key] = num;
				this.SaveSpecialPriorityActConitionInfo();
			}
		}

		public List<SpecPConditionConfig> CalSpecPConditionListByNow(DateTime now)
		{
			return this.SpecPConditionList.FindAll((SpecPConditionConfig x) => x.FromDate <= now && now <= x.ToDate);
		}

		public void OnConditionNumChangeBefore()
		{
			lock (SpecPriorityActivity.Mutex)
			{
				this.ActConditionInfoDictOld = new Dictionary<int, int>(this.ActConditionInfoDict);
			}
		}

		public void OnConditionNumChangeAfter()
		{
			bool flag = false;
			lock (SpecPriorityActivity.Mutex)
			{
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						int num = 0;
						Dictionary<int, int> actConditionInfoDictOld = this.ActConditionInfoDictOld;
						KeyValuePair<int, int> kvp3 = kvp;
						actConditionInfoDictOld.TryGetValue(kvp3.Key, out num);
						kvp3 = kvp;
						if (kvp3.Value != num)
						{
							SpecPConditionConfig specPConditionConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
							{
								int groupID = x.GroupID;
								KeyValuePair<int, int> kvp2 = kvp;
								return groupID == kvp2.Key;
							});
							if (null != specPConditionConfig)
							{
								foreach (int key in specPConditionConfig.ActiveSpecPList)
								{
									SpecPActiveConfig specPActiveConfig = null;
									if (this.SpecPActiveDict.TryGetValue(key, out specPActiveConfig))
									{
										bool flag3;
										if (num < specPActiveConfig.ConditonNum)
										{
											int conditonNum = specPActiveConfig.ConditonNum;
											kvp3 = kvp;
											flag3 = (conditonNum > kvp3.Value);
										}
										else
										{
											flag3 = true;
										}
										if (!flag3)
										{
											if (specPActiveConfig.ActType == 1)
											{
												this.TryRefreshBoss(specPActiveConfig);
											}
											else
											{
												flag = true;
											}
										}
									}
								}
							}
						}
					}
				}
				this.ActConditionInfoDictOld = this.ActConditionInfoDict;
			}
			if (flag)
			{
				GameManager.ClientMgr.ReGenerateSpecPriorityActGroup();
			}
		}

		public bool CheckValidChargeItem(int zhigouID)
		{
			bool result;
			if (!this.ActZhiGouIDSet.Contains(zhigouID))
			{
				result = true;
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
				foreach (SpecPConditionConfig specPConditionConfig in list)
				{
					foreach (int tequanID in specPConditionConfig.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(specPConditionConfig.GroupID, tequanID))
						{
							List<SpecPActivityConfig> specPActivityListByTequanID = this.GetSpecPActivityListByTequanID(tequanID);
							foreach (SpecPActivityConfig specPActivityConfig in specPActivityListByTequanID)
							{
								if (specPActivityConfig.ActType == 3)
								{
									if (zhigouID == specPActivityConfig.ZhiGouID)
									{
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private SpecPActivityConfig GetSpecPActivityConfig(int tequanID, int actID)
		{
			SpecPActivityConfig specPActivityConfig = null;
			List<SpecPActivityConfig> list = null;
			SpecPActivityConfig result;
			if (!this.SpecPActivityDict.TryGetValue(tequanID, out list))
			{
				result = specPActivityConfig;
			}
			else
			{
				specPActivityConfig = list.Find((SpecPActivityConfig x) => x.ActID == actID);
				result = specPActivityConfig;
			}
			return result;
		}

		private List<SpecPActivityConfig> GetSpecPActivityListByTequanID(int tequanID)
		{
			List<SpecPActivityConfig> list = null;
			lock (SpecPriorityActivity.Mutex)
			{
				this.SpecPActivityDict.TryGetValue(tequanID, out list);
			}
			if (null == list)
			{
				list = new List<SpecPActivityConfig>();
				LogManager.WriteLog(2, string.Format("特权活动找不到对应的活动数据, TeQuanID={0}", tequanID), null, true);
			}
			return list;
		}

		private void TryRefreshBoss(SpecPActiveConfig activeConfig)
		{
			List<SpecPActivityConfig> list = null;
			if (this.SpecPActivityDict.TryGetValue(activeConfig.TeQuanID, out list))
			{
				foreach (SpecPActivityConfig specPActivityConfig in list)
				{
					if (specPActivityConfig.ActType == 1)
					{
						int param = specPActivityConfig.Param1;
						int param2 = specPActivityConfig.Param2;
						List<MonsterZone> monsterZoneByMapCodeAndMonsterID = GameManager.MonsterZoneMgr.GetMonsterZoneByMapCodeAndMonsterID(param, param2);
						if (monsterZoneByMapCodeAndMonsterID != null && monsterZoneByMapCodeAndMonsterID.Count > 0)
						{
							MonsterZone monsterZone = monsterZoneByMapCodeAndMonsterID[Global.GetRandomNumber(0, monsterZoneByMapCodeAndMonsterID.Count)];
							SceneUIClasses mapSceneType = Global.GetMapSceneType(param);
							GameManager.MonsterZoneMgr.AddDynamicMonsters(param, param2, -1, 1, monsterZone.ToX, monsterZone.ToY, 0, monsterZone.PursuitRadius, mapSceneType, null, null);
						}
					}
				}
			}
		}

		private bool ConditionNumCountAlreadyUser(string userid, int roleid, SpecPConditionType type, SpecPConditionConfig condition)
		{
			string arg = condition.FromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string arg2 = condition.ToDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string text = string.Format("{0}_{1}_{2}", arg, arg2, type);
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleid,
				text,
				this.ActivityType,
				"0"
			});
			string[] array = Global.ExecuteDBCmd(10221, strcmd, 0);
			bool result;
			if (array == null || array.Length == 0)
			{
				result = true;
			}
			else
			{
				int num = Global.SafeConvertToInt32(array[3]);
				result = (num > 0);
			}
			return result;
		}

		private void CoditionNumCountUser(string userid, int roleid, SpecPConditionType type, SpecPConditionConfig condition)
		{
			string arg = condition.FromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string arg2 = condition.ToDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string text = string.Format("{0}_{1}_{2}", arg, arg2, type);
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				roleid,
				text,
				this.ActivityType,
				"1",
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH$mm$ss")
			});
			Global.ExecuteDBCmd(10222, strcmd, 0);
		}

		public List<int> GetSpecPRoleInfo(GameClient client)
		{
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "153");
			if (roleParamsIntListFromDB.Count != 3)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 3; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (roleParamsIntListFromDB[0] != offsetDay)
			{
				roleParamsIntListFromDB[0] = offsetDay;
				roleParamsIntListFromDB[1] = 0;
				roleParamsIntListFromDB[2] = 0;
			}
			return roleParamsIntListFromDB;
		}

		public void SaveSpecPRoleInfo(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "153", true);
		}

		private void ConditionNumTrigger(string userid, int roleid, SpecPConditionType type, int param, GameClient client = null)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
			SpecPConditionConfig specPConditionConfig = list.Find((SpecPConditionConfig x) => x.ConditionType == type);
			if (null != specPConditionConfig)
			{
				lock (SpecPriorityActivity.Mutex)
				{
					this.OnConditionNumChangeBefore();
					switch (type)
					{
					case 1:
						if (param >= specPConditionConfig.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, specPConditionConfig))
						{
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, specPConditionConfig);
						}
						break;
					case 2:
						if (param >= specPConditionConfig.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, specPConditionConfig))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(specPConditionConfig.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, specPConditionConfig);
						}
						break;
					case 3:
						if (specPConditionConfig.ConditionList.Contains(param))
						{
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
						}
						break;
					case 4:
						if (specPConditionConfig.ConditionList.Contains(param))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(specPConditionConfig.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
						}
						break;
					case 5:
						this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
						break;
					case 6:
						KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(specPConditionConfig.GroupID, 1);
						this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
						break;
					case 7:
					{
						int num = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(roleid, specPConditionConfig.FromDate, specPConditionConfig.ToDate);
						if (num >= specPConditionConfig.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, specPConditionConfig))
						{
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, specPConditionConfig);
						}
						break;
					}
					case 8:
					{
						int num = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(roleid, specPConditionConfig.FromDate, specPConditionConfig.ToDate);
						if (num >= specPConditionConfig.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, specPConditionConfig))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(specPConditionConfig.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, specPConditionConfig);
						}
						break;
					}
					case 9:
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(0, -2);
						SpecPriorityActInfoDB specPriorityActInfoDB = null;
						if (client != null && client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
						{
							if (specPriorityActInfoDB.PurNum != specPConditionConfig.GroupID)
							{
								specPriorityActInfoDB.PurNum = specPConditionConfig.GroupID;
								specPriorityActInfoDB.CountNum = 0;
							}
							int countNum = specPriorityActInfoDB.CountNum;
							specPriorityActInfoDB.CountNum += param;
							if (countNum < specPConditionConfig.ConditionList[0] && specPConditionConfig.ConditionList[0] <= specPriorityActInfoDB.CountNum)
							{
								this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							}
							this.UpdateClientSpecPriorityActData(client, specPriorityActInfoDB);
						}
						break;
					}
					case 10:
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(0, -1);
						SpecPriorityActInfoDB specPriorityActInfoDB = null;
						if (client != null && client.ClientData.SpecPriorityActInfoDict.TryGetValue(key, out specPriorityActInfoDB))
						{
							if (specPriorityActInfoDB.PurNum != specPConditionConfig.GroupID)
							{
								specPriorityActInfoDB.PurNum = specPConditionConfig.GroupID;
								specPriorityActInfoDB.CountNum = 0;
							}
							int countNum = specPriorityActInfoDB.CountNum;
							specPriorityActInfoDB.CountNum += param;
							if (countNum < specPConditionConfig.ConditionList[0] && specPConditionConfig.ConditionList[0] <= specPriorityActInfoDB.CountNum)
							{
								KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(specPConditionConfig.GroupID, 1);
								this.ModifySpecialPriorityActConitionInfo(specPConditionConfig.GroupID, 1);
							}
							this.UpdateClientSpecPriorityActData(client, specPriorityActInfoDB);
						}
						break;
					}
					}
					this.OnConditionNumChangeAfter();
				}
			}
		}

		private void DeleteClientSpecPriorityActData(GameClient client, int TeQuanID = 0)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, TeQuanID);
			Global.ExecuteDBCmd(13176, strcmd, client.ServerId);
		}

		private void UpdateClientSpecPriorityActData(GameClient client, SpecPriorityActInfoDB SpecPActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				SpecPActData.TeQuanID,
				SpecPActData.ActID,
				SpecPActData.PurNum,
				SpecPActData.CountNum
			});
			Global.ExecuteDBCmd(13175, strcmd, client.ServerId);
		}

		private void InitPriorityActConditionInfo(DateTime now, bool launch = false)
		{
			lock (SpecPriorityActivity.Mutex)
			{
				List<SpecPConditionConfig> list = this.CalSpecPConditionListByNow(now);
				List<int> list2 = new List<int>();
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						if (!list.Exists(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						}))
						{
							List<int> list3 = list2;
							KeyValuePair<int, int> kvp3 = kvp;
							list3.Add(kvp3.Key);
						}
					}
				}
				foreach (int key in list2)
				{
					int key;
					this.ActConditionInfoDict.Remove(key);
				}
				foreach (SpecPConditionConfig specPConditionConfig in list)
				{
					if (!this.ActConditionInfoDict.ContainsKey(specPConditionConfig.GroupID))
					{
						this.ActConditionInfoDict[specPConditionConfig.GroupID] = 0;
					}
				}
				if (launch)
				{
					string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("specpact", "");
					if (!string.IsNullOrEmpty(gameConfigItemStr))
					{
						string[] array = gameConfigItemStr.Split(new char[]
						{
							'|'
						});
						foreach (string text in array)
						{
							string[] array3 = text.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								int key = Global.SafeConvertToInt32(array3[0]);
								int value = Global.SafeConvertToInt32(array3[1]);
								if (this.ActConditionInfoDict.ContainsKey(key))
								{
									this.ActConditionInfoDict[key] = value;
								}
							}
						}
					}
				}
			}
		}

		private void SaveSpecialPriorityActConitionInfo()
		{
			lock (SpecPriorityActivity.Mutex)
			{
				string text = "";
				foreach (KeyValuePair<int, int> keyValuePair in this.ActConditionInfoDict)
				{
					text += string.Format("{0},{1}|", keyValuePair.Key, keyValuePair.Value);
				}
				if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
				{
					text = text.Substring(0, text.Length - 1);
				}
				GameManager.GameConfigMgr.SetGameConfigItem("specpact", text);
				Global.UpdateDBGameConfigg("specpact", text);
			}
		}

		public void TimerProc()
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (SpecPriorityActivity.Mutex)
				{
					SpecPrioritySyncData specPrioritySyncData = KFCopyRpcClient.getInstance().SpecPriority_GetActivityConditionInfo();
					if (null != specPrioritySyncData)
					{
						int offsetDay = TimeUtil.GetOffsetDay(dateTime);
						if (offsetDay != this.LastUpdateDayID)
						{
							this.LastUpdateDayID = offsetDay;
							this.InitPriorityActConditionInfo(dateTime, false);
						}
						this.OnConditionNumChangeBefore();
						bool flag2 = false;
						using (Dictionary<int, int>.Enumerator enumerator = specPrioritySyncData.ActConditionInfoDict.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<int, int> item = enumerator.Current;
								SpecPConditionConfig specPConditionConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
								{
									int groupID = x.GroupID;
									KeyValuePair<int, int> item2 = item;
									return groupID == item2.Key;
								});
								if (null != specPConditionConfig)
								{
									if (this.IfKFConditonType(specPConditionConfig.ConditionType))
									{
										int num = 0;
										Dictionary<int, int> actConditionInfoDict = this.ActConditionInfoDict;
										KeyValuePair<int, int> item3 = item;
										actConditionInfoDict.TryGetValue(item3.Key, out num);
										int num2 = num;
										item3 = item;
										if (num2 != item3.Value)
										{
											flag2 = true;
											Dictionary<int, int> actConditionInfoDict2 = this.ActConditionInfoDict;
											item3 = item;
											int key = item3.Key;
											item3 = item;
											actConditionInfoDict2[key] = item3.Value;
										}
									}
								}
							}
						}
						if (flag2)
						{
							this.SaveSpecialPriorityActConitionInfo();
						}
						this.OnConditionNumChangeAfter();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
		}

		public bool IfKFConditonType(SpecPConditionType ConditionType)
		{
			return 2 == ConditionType || 4 == ConditionType || 6 == ConditionType || 8 == ConditionType || 10 == ConditionType;
		}

		public bool Init()
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (!this.LoadSpecPriorityConditionData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityActiveData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityBossData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityAwardData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityZhiGouData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityMallData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityBuffData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityHongBaoData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityChouJiangData())
				{
					return false;
				}
				this.RedPacketsTeQuanMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsTeQuanMessage");
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 49;
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
				this.InitPriorityActConditionInfo(dateTime, true);
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(dateTime);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityConditionData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanTiaoJian.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanTiaoJian.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPConditionConfig specPConditionConfig = new SpecPConditionConfig();
						specPConditionConfig.GroupID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "KaiQiShiJian");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							specPConditionConfig.FromDate = DateTime.Parse(safeAttributeStr);
						}
						else
						{
							specPConditionConfig.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "JieShuShiJian");
						if (!string.IsNullOrEmpty(safeAttributeStr2))
						{
							specPConditionConfig.ToDate = DateTime.Parse(safeAttributeStr2);
						}
						else
						{
							specPConditionConfig.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						specPConditionConfig.ConditionType = (int)Global.GetSafeAttributeLong(xelement2, "TiaoJianLeiXing");
						if (specPConditionConfig.ConditionType == 5 || specPConditionConfig.ConditionType == 6)
						{
							string[] array = Global.GetSafeAttributeStr(xelement2, "GuDingLeiXing").Split(new char[]
							{
								'|'
							});
							string[] array2 = array[0].Split(new char[]
							{
								','
							});
							specPConditionConfig.ConditionList = new int[]
							{
								Global.SafeConvertToInt32(array2[0]),
								Global.SafeConvertToInt32(array2[1]),
								Global.SafeConvertToInt32(array[1])
							};
						}
						else
						{
							specPConditionConfig.ConditionList = Global.GetSafeAttributeIntArray(xelement2, "GuDingLeiXing", -1, ',');
						}
						specPConditionConfig.ActiveSpecPList = Global.GetSafeAttributeIntArray(xelement2, "JiHuoID", -1, '|');
						int num = 0;
						FallGoodsItem fallGoodsItem = null;
						string defAttributeStr = Global.GetDefAttributeStr(xelement2, "JiangLiFanKui", "");
						if (!string.IsNullOrEmpty(defAttributeStr))
						{
							List<FallGoodsItem> list = new List<FallGoodsItem>();
							string[] array3 = defAttributeStr.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array3.Length; i++)
							{
								string text = array3[i].Trim();
								if (!(text == ""))
								{
									string[] array4 = text.Split(new char[]
									{
										','
									});
									if (array4.Length == 7)
									{
										fallGoodsItem = null;
										try
										{
											fallGoodsItem = new FallGoodsItem
											{
												GoodsID = Convert.ToInt32(array4[0]),
												BasePercent = num,
												SelfPercent = (int)(Convert.ToDouble(array4[1]) * 100000.0),
												Binding = Convert.ToInt32(array4[2]),
												LuckyRate = (int)Convert.ToDouble(array4[3]),
												FallLevelID = Convert.ToInt32(array4[4]),
												ZhuiJiaID = Convert.ToInt32(array4[5]),
												ExcellencePropertyID = Convert.ToInt32(array4[6])
											};
											num += fallGoodsItem.SelfPercent;
										}
										catch (Exception)
										{
											fallGoodsItem = null;
										}
										if (null == fallGoodsItem)
										{
											LogManager.WriteLog(2, string.Format("解析特权活动JiangLiFanKui项时发生错误, ID={0}, GoodsID={1}", specPConditionConfig.GroupID, text), null, true);
										}
										else
										{
											list.Add(fallGoodsItem);
										}
									}
								}
							}
							specPConditionConfig.fallGoodsItemList = list;
						}
						specPConditionConfig.EveryDayFinishNum = (int)Global.GetSafeAttributeLong(xelement2, "MeiRiShangXian");
						this.SpecPConditionList.Add(specPConditionConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanTiaoJian.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityActiveData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanJiHuo.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanJiHuo.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActiveConfig specPActiveConfig = new SpecPActiveConfig();
						specPActiveConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActiveConfig.ConditonNum = (int)Global.GetSafeAttributeLong(xelement2, "CanShu");
						specPActiveConfig.ActType = (int)Global.GetSafeAttributeLong(xelement2, "tips");
						this.SpecPActiveDict[specPActiveConfig.TeQuanID] = specPActiveConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanJiHuo.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityBossData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanBoss.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanBoss.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.Param1 = (int)Global.GetSafeAttributeLong(xelement2, "DiTuID");
						specPActivityConfig.Param2 = (int)Global.GetSafeAttributeLong(xelement2, "GuaiWuID");
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 1;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanBoss.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityAwardData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanJiangLi.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanJiangLi.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.Param1 = (int)Global.GetSafeAttributeLong(xelement2, "LingQuTiaoJian");
						specPActivityConfig.PurchaseNum = 1;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "WuPinID");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("解析大型特权活动Award配置文件中的物品配置项失败", new object[0]), null, true);
						}
						else
						{
							specPActivityConfig.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array, "特权活动Award配置");
						}
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 2;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanJiangLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityZhiGouData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanZhiGou.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanZhiGou.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.PurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "GouMaiCiShu");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "ZhiGouJiaGe");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length == 3)
						{
							specPActivityConfig.Param1 = Global.SafeConvertToInt32(array[0]);
							specPActivityConfig.Param2 = Global.SafeConvertToInt32(array[1]);
							specPActivityConfig.ZhiGouID = Global.SafeConvertToInt32(array[2]);
						}
						this.ActZhiGouIDSet.Add(specPActivityConfig.ZhiGouID);
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 3;
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "DaoJuJiangLi");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "FenZhiYeJiangLi");
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(specPActivityConfig.ZhiGouID, specPActivityConfig.PurchaseNum, safeAttributeStr2, safeAttributeStr3, string.Format("特权活动直购 ID={0}", specPActivityConfig.ActID));
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanZhiGou.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityMallData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanShangCheng.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanShangCheng.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.Param1 = (int)Global.GetSafeAttributeLong(xelement2, "JiaGe");
						specPActivityConfig.PurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "GouMaiCiShu");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "WuPinID");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("解析大型特权活动Mall配置文件中的物品配置项失败", new object[0]), null, true);
						}
						else
						{
							specPActivityConfig.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array, "特权活动Mall配置");
						}
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 4;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityBuffData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanBuff.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanBuff.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.Param1 = (int)Global.GetSafeAttributeLong(xelement2, "HuoDongLeiXing");
						specPActivityConfig.MultiNum = Global.GetSafeAttributeDouble(xelement2, "KaiQiBeiShu");
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 5;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanBuff.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityHongBaoData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanHongBao.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanHongBao.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specPActivityConfig.Param1 = (int)Global.GetSafeAttributeLong(xelement2, "BangZuanShuLiang");
						specPActivityConfig.HongBaoRange = Global.GetSafeAttributeIntArray(xelement2, "HongBaoQuYu", -1, ',');
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 6;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecPriorityChouJiangData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanChouJiang.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanChouJiang.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecPActivityConfig specPActivityConfig = new SpecPActivityConfig();
						specPActivityConfig.TeQuanID = (int)Global.GetSafeAttributeLong(xelement2, "TeQuanID");
						specPActivityConfig.ActID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "ChouJiangLeiXing");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							foreach (string strA in array)
							{
								if (string.Compare(strA, "TeQuanQiFu", true) == 0)
								{
									specPActivityConfig.ChouJiangTypeSet.Add(1);
								}
								if (string.Compare(strA, "TeQuanShouLie", true) == 0)
								{
									specPActivityConfig.ChouJiangTypeSet.Add(2);
								}
								if (string.Compare(strA, "TeQuanBuHuo", true) == 0)
								{
									specPActivityConfig.ChouJiangTypeSet.Add(3);
								}
							}
						}
						List<SpecPActivityConfig> list = null;
						if (!this.SpecPActivityDict.TryGetValue(specPActivityConfig.TeQuanID, out list))
						{
							list = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[specPActivityConfig.TeQuanID] = list;
						}
						specPActivityConfig.ActType = 7;
						list.Add(specPActivityConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/TeQuanChouJiang.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public const string SpecPCondition_fileName = "Config/TeQuanTiaoJian.xml";

		public const string SpecPActive_fileName = "Config/TeQuanJiHuo.xml";

		public const string SpecPBoss_fileName = "Config/TeQuanBoss.xml";

		public const string SpecPAward_fileName = "Config/TeQuanJiangLi.xml";

		public const string SpecPZhiGou_fileName = "Config/TeQuanZhiGou.xml";

		public const string SpecPMall_fileName = "Config/TeQuanShangCheng.xml";

		public const string SpecPBuff_fileName = "Config/TeQuanBuff.xml";

		public const string SpecPHongBao_fileName = "Config/TeQuanHongBao.xml";

		public const string SpecPChouJiang_fileName = "Config/TeQuanChouJiang.xml";

		public static object Mutex = new object();

		public List<SpecPConditionConfig> SpecPConditionList = new List<SpecPConditionConfig>();

		public Dictionary<int, SpecPActiveConfig> SpecPActiveDict = new Dictionary<int, SpecPActiveConfig>();

		public Dictionary<int, List<SpecPActivityConfig>> SpecPActivityDict = new Dictionary<int, List<SpecPActivityConfig>>();

		public Dictionary<int, int> ActConditionInfoDict = new Dictionary<int, int>();

		public Dictionary<int, int> ActConditionInfoDictOld = new Dictionary<int, int>();

		public HashSet<int> ActZhiGouIDSet = new HashSet<int>();

		private HashSet<int> HongBaoIdSended = new HashSet<int>();

		private string RedPacketsTeQuanMessage;

		private int LastUpdateDayID;
	}
}
