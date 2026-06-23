using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class EverydayActivity : Activity, IEventListener
	{
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int num = -1;
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<int, EverydayActInfoDB> keyValuePair in chargeItemBaseEventObject.Player.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB value = keyValuePair.Value;
					EverydayActivityConfig everydayActivityConfig = null;
					if (this.ActivityConfigDict.TryGetValue(value.ActID, out everydayActivityConfig))
					{
						if (everydayActivityConfig.Type == 14 && everydayActivityConfig.Price.ZhiGouID == chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID)
						{
							num = value.ActID;
							break;
						}
					}
				}
				if (num != -1)
				{
					string cmdData = this.BuildFetchEverydayActAwardCmd(chargeItemBaseEventObject.Player, 0, num);
					chargeItemBaseEventObject.Player.sendCmd<string>(1507, cmdData, false);
				}
			}
		}

		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("EveryDayChongZhiDuiHuan");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					':'
				});
				if (array.Length == 2)
				{
					int num = Convert.ToInt32(array[0]);
					if (num != 0)
					{
						double num2 = Convert.ToDouble(array[1]) / (double)num;
						int num3 = (int)(num2 * (double)Global.TransMoneyToYuanBao(addMoney));
						DateTime dateTime = TimeUtil.NowDateTime();
						DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
						DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							roleid,
							num3,
							dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'),
							dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$')
						});
						Global.ExecuteDBCmd(13173, strcmd, 0);
					}
				}
			}
		}

		public bool CheckValidChargeItem(int zhigouID)
		{
			List<EverydayActGroupInfoDB> list = null;
			lock (this.Mutex)
			{
				list = this.GetCurrentActGroupInfo();
			}
			this.CleanUpActGroupInfo(list, 1);
			foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in list)
			{
				EverydayActivityGroupConfig everydayActivityGroupConfig = null;
				if (this.ActivityGroupConfigDict.TryGetValue(everydayActGroupInfoDB.GroupID, out everydayActivityGroupConfig))
				{
					foreach (int key in everydayActivityGroupConfig.ActivityIDList)
					{
						EverydayActivityConfig everydayActivityConfig = null;
						if (this.ActivityConfigDict.TryGetValue(key, out everydayActivityConfig))
						{
							if (everydayActivityConfig.Price.ZhiGouID == zhigouID)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public void MoneyConst(GameClient client, int moneyCost)
		{
			if (client.ClientData.EverydayActInfoDict != null && client.ClientData.EverydayActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> keyValuePair in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB value = keyValuePair.Value;
					EverydayActivityConfig everydayActivityConfig = null;
					if (this.ActivityConfigDict.TryGetValue(value.ActID, out everydayActivityConfig))
					{
						if (everydayActivityConfig.Type == 3)
						{
							value.CountNum += moneyCost;
							this.UpdateClientEverydayActData(client, value);
						}
					}
				}
				if (client._IconStateMgr.CheckEverydayActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public bool CheckIconState(GameClient client)
		{
			bool flag = false;
			bool result;
			if (client.ClientData.EverydayActInfoDict == null || client.ClientData.EverydayActInfoDict.Count == 0)
			{
				result = flag;
			}
			else
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> keyValuePair in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB value = keyValuePair.Value;
					int num = this.EverydayActCheckCondition(client, keyValuePair.Key, false);
					if (num == 0)
					{
						flag = true;
						break;
					}
				}
				result = flag;
			}
			return result;
		}

		public void OnRoleLogin(GameClient client)
		{
			this.GenerateEverydayActivity(client);
			this.NotifyActivityState(client);
		}

		public EverydayActivityData GetEverydayActivityDataForClient(GameClient client)
		{
			EverydayActivityData everydayActivityData = new EverydayActivityData();
			everydayActivityData.EveryActInfoList = new List<EverydayActInfo>();
			EverydayActivityData result;
			if (null == client.ClientData.EverydayActInfoDict)
			{
				result = everydayActivityData;
			}
			else
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> keyValuePair in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB value = keyValuePair.Value;
					EverydayActivityConfig everydayActivityConfig = null;
					if (this.ActivityConfigDict.TryGetValue(value.ActID, out everydayActivityConfig))
					{
						EverydayActInfo everydayActInfo = new EverydayActInfo();
						everydayActInfo.ActID = value.ActID;
						EveryActGoalData currentGoalNum = this.GetCurrentGoalNum(client, value, everydayActivityConfig);
						everydayActInfo.ShowNum = currentGoalNum.NumOne;
						everydayActInfo.ShowNum2 = currentGoalNum.NumTwo;
						int num = value.PurNum;
						if (everydayActivityConfig.Type == 14)
						{
							num = UserMoneyMgr.getInstance().GetChargeItemDayPurchaseNum(client, everydayActivityConfig.Price.ZhiGouID);
						}
						if (everydayActivityConfig.PurchaseNum == -1)
						{
							everydayActInfo.State = ((num == 1) ? 1 : 0);
						}
						else
						{
							everydayActInfo.LeftPurNum = everydayActivityConfig.PurchaseNum - num;
							everydayActInfo.State = ((everydayActInfo.LeftPurNum <= 0) ? 1 : 0);
							if (everydayActInfo.LeftPurNum < 0)
							{
								everydayActInfo.LeftPurNum = 0;
							}
						}
						if (everydayActivityConfig.GoalData.IsValid())
						{
							if (currentGoalNum.NumOne < everydayActivityConfig.GoalData.NumOne || (currentGoalNum.NumOne == everydayActivityConfig.GoalData.NumOne && currentGoalNum.NumTwo < everydayActivityConfig.GoalData.NumTwo))
							{
								everydayActInfo.State = -1;
							}
						}
						everydayActivityData.EveryActInfoList.Add(everydayActInfo);
					}
				}
				result = everydayActivityData;
			}
			return result;
		}

		public int EverydayActCheckCondition(GameClient client, int ActID, bool CheckCost = true)
		{
			EverydayActInfoDB everydayActInfoDB = null;
			int result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out everydayActInfoDB))
			{
				result = -2;
			}
			else
			{
				EverydayActivityConfig everydayActivityConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(everydayActInfoDB.ActID, out everydayActivityConfig))
				{
					result = -2;
				}
				else if (everydayActivityConfig.Type == 14)
				{
					result = -12;
				}
				else
				{
					EveryActGoalData currentGoalNum = this.GetCurrentGoalNum(client, everydayActInfoDB, everydayActivityConfig);
					if (everydayActivityConfig.GoalData.IsValid())
					{
						if (currentGoalNum.NumOne < everydayActivityConfig.GoalData.NumOne || (currentGoalNum.NumOne == everydayActivityConfig.GoalData.NumOne && currentGoalNum.NumTwo < everydayActivityConfig.GoalData.NumTwo))
						{
							return -12;
						}
					}
					if (everydayActivityConfig.PurchaseNum == -1)
					{
						if (everydayActInfoDB.PurNum == 1)
						{
							return -200;
						}
					}
					else if (everydayActivityConfig.PurchaseNum - everydayActInfoDB.PurNum <= 0)
					{
						return -200;
					}
					if (CheckCost && everydayActivityConfig.Type == 2)
					{
						if (this.GetCurrentEverydayActJiFen(client, everydayActivityConfig) < everydayActivityConfig.Price.NumOne)
						{
							return -39;
						}
					}
					if (CheckCost && everydayActivityConfig.Type == 1)
					{
						if (client.ClientData.UserMoney < everydayActivityConfig.Price.NumOne)
						{
							return -10;
						}
					}
					result = 0;
				}
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int ActID)
		{
			EverydayActInfoDB everydayActInfoDB = null;
			bool result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out everydayActInfoDB))
			{
				result = false;
			}
			else
			{
				EverydayActivityConfig everydayActivityConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(everydayActInfoDB.ActID, out everydayActivityConfig))
				{
					result = false;
				}
				else
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> list = new List<GoodsData>();
					foreach (GoodsData item in everydayActivityConfig.GoodsDataListOne)
					{
						list.Add(item);
					}
					int count = everydayActivityConfig.GoodsDataListTwo.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData goodsData = everydayActivityConfig.GoodsDataListTwo[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, goodsData.GoodsID))
						{
							list.Add(goodsData);
						}
					}
					AwardItem awardItem = everydayActivityConfig.GoodsDataListThr.ToAwardItem();
					foreach (GoodsData item in awardItem.GoodsDataList)
					{
						list.Add(item);
					}
					result = Global.CanAddGoodsDataList(client, list);
				}
			}
			return result;
		}

		public int EverydayActGiveAward(GameClient client, int ActID)
		{
			EverydayActInfoDB everydayActInfoDB = null;
			int result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out everydayActInfoDB))
			{
				result = -2;
			}
			else
			{
				EverydayActivityConfig everydayActivityConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(everydayActInfoDB.ActID, out everydayActivityConfig))
				{
					result = -2;
				}
				else
				{
					if (everydayActivityConfig.Type == 2)
					{
						if (!this.SubEverydayActJiFen(client, everydayActivityConfig))
						{
							return -39;
						}
					}
					if (everydayActivityConfig.Type == 1 && everydayActivityConfig.Price.NumOne > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, everydayActivityConfig.Price.NumOne, "每日活动抢购", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
					}
					AwardItem awardItem = new AwardItem();
					awardItem.GoodsDataList = everydayActivityConfig.GoodsDataListOne;
					base.GiveAward(client, awardItem);
					awardItem.GoodsDataList = everydayActivityConfig.GoodsDataListTwo;
					base.GiveAward(client, awardItem);
					awardItem = everydayActivityConfig.GoodsDataListThr.ToAwardItem();
					base.GiveEffectiveTimeAward(client, awardItem);
					if (everydayActivityConfig.PurchaseNum == -1)
					{
						everydayActInfoDB.PurNum = 1;
					}
					else
					{
						everydayActInfoDB.PurNum++;
					}
					this.UpdateClientEverydayActData(client, everydayActInfoDB);
					if (client._IconStateMgr.CheckEverydayActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					result = 0;
				}
			}
			return result;
		}

		public string BuildFetchEverydayActAwardCmd(GameClient client, int ErrCode, int actID)
		{
			int roleID = client.ClientData.RoleID;
			EverydayActInfoDB everydayActInfoDB = null;
			string result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(actID, out everydayActInfoDB))
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-2,
					roleID,
					actID,
					0,
					0
				});
			}
			else
			{
				EverydayActivityConfig everydayActivityConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(everydayActInfoDB.ActID, out everydayActivityConfig))
				{
					result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-2,
						roleID,
						actID,
						0,
						0
					});
				}
				else
				{
					if (everydayActivityConfig.Type == 14)
					{
						everydayActInfoDB.PurNum = UserMoneyMgr.getInstance().GetChargeItemDayPurchaseNum(client, everydayActivityConfig.Price.ZhiGouID);
					}
					int num = everydayActivityConfig.PurchaseNum - everydayActInfoDB.PurNum;
					EveryActGoalData currentGoalNum = this.GetCurrentGoalNum(client, everydayActInfoDB, everydayActivityConfig);
					int numOne = currentGoalNum.NumOne;
					result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						ErrCode,
						roleID,
						actID,
						num,
						numOne
					});
				}
			}
			return result;
		}

		public void NotifyActivityState(GameClient client)
		{
			bool flag = false;
			if (client.ClientData.EverydayActInfoDict != null && client.ClientData.EverydayActInfoDict.Count != 0 && this.PlatformOpenStateVavle == 1 && !client.ClientSocket.IsKuaFuLogin)
			{
				flag = true;
			}
			if (flag)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					12,
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
					12,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public void ShowActiveConditionInfoGM(GameClient client)
		{
			for (int i = 0; i < 23; i++)
			{
				EverydayActNeedType everydayActNeedType = this.ConvertPaiHangTypesToActNeedType((PaiHangTypes)i);
				if (EverydayActNeedType.EANT_Null != everydayActNeedType)
				{
					string cmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
					{
						0,
						i,
						100
					});
					PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, cmd, 0);
					if (null == paiHangData)
					{
						this.ActiveConditionDict.Clear();
						return;
					}
					if (null != paiHangData.PaiHangList)
					{
						this.CacheNeedCondition(everydayActNeedType, paiHangData);
					}
				}
			}
			foreach (KeyValuePair<int, EveryActActiveData> keyValuePair in this.ActiveConditionDict)
			{
				string textMsg = string.Format("NeedType={0} NumOne={1} NumTwo={2}", keyValuePair.Key, keyValuePair.Value.NumOne, keyValuePair.Value.NumTwo);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
			}
			if (null != client.ClientData.EverydayActInfoDict)
			{
				foreach (EverydayActInfoDB everydayActInfoDB in client.ClientData.EverydayActInfoDict.Values)
				{
					EverydayActivityGroupConfig everydayActivityGroupConfig = null;
					if (this.ActivityGroupConfigDict.TryGetValue(everydayActInfoDB.GroupID, out everydayActivityGroupConfig))
					{
						string textMsg = string.Format("GroupID={0} ActID={1} PurNum={2} CountNum={3} TypeID={4}", new object[]
						{
							everydayActInfoDB.GroupID,
							everydayActInfoDB.ActID,
							everydayActInfoDB.PurNum,
							everydayActInfoDB.CountNum,
							everydayActivityGroupConfig.TypeID
						});
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					}
				}
			}
			List<EverydayActGroupInfoDB> currentActGroupInfo = this.GetCurrentActGroupInfo();
			foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in currentActGroupInfo)
			{
				EverydayActivityGroupConfig everydayActivityGroupConfig = null;
				if (this.ActivityGroupConfigDict.TryGetValue(everydayActGroupInfoDB.GroupID, out everydayActivityGroupConfig))
				{
					string textMsg = string.Format("ActiveDay={0} GroupID={1} TypeID={2}", everydayActGroupInfoDB.ActiveDay, everydayActGroupInfoDB.GroupID, everydayActivityGroupConfig.TypeID);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
				}
			}
		}

		public void CacheNeedCondition(EverydayActNeedType type, PaiHangData paiHangData)
		{
			EveryActActiveData everyActActiveData = new EveryActActiveData();
			List<PaiHangItemData> paiHangList = paiHangData.PaiHangList;
			int num = Math.Min(paiHangList.Count, 100);
			for (int i = 0; i < num; i++)
			{
				PaiHangItemData paiHangItemData = paiHangList[i];
				if (EverydayActNeedType.EANT_CombatForce == type || EverydayActNeedType.EANT_UserMoney == type || EverydayActNeedType.EANT_ChengJiu == type || EverydayActNeedType.EANT_ShengWang == type || EverydayActNeedType.EANT_HolyItem == type)
				{
					everyActActiveData.NumOne += (long)paiHangItemData.Val1;
				}
				else if (EverydayActNeedType.EANT_Level == type || EverydayActNeedType.EANT_Wing == type || EverydayActNeedType.EANT_Ring == type || EverydayActNeedType.EANT_Merlin == type || EverydayActNeedType.EANT_GuardStatue == type)
				{
					everyActActiveData.NumOne += (long)paiHangItemData.Val1;
					everyActActiveData.NumTwo += (long)paiHangItemData.Val2;
				}
			}
			if (num > 0)
			{
				if (EverydayActNeedType.EANT_CombatForce == type || EverydayActNeedType.EANT_UserMoney == type || EverydayActNeedType.EANT_ChengJiu == type || EverydayActNeedType.EANT_ShengWang == type || EverydayActNeedType.EANT_HolyItem == type)
				{
					everyActActiveData.NumOne /= (long)num;
				}
				else if (EverydayActNeedType.EANT_Level == type)
				{
					int num2 = Global.GetUnionLevel2((int)everyActActiveData.NumTwo, (int)everyActActiveData.NumOne) / num;
					everyActActiveData.NumOne = (long)((num2 - 1) / 100);
					everyActActiveData.NumTwo = (long)((num2 - 1) % 100 + 1);
				}
				else if (EverydayActNeedType.EANT_Wing == type)
				{
					int num2 = (int)everyActActiveData.NumOne * 10 + (int)everyActActiveData.NumTwo;
					num2 /= num;
					everyActActiveData.NumOne = (long)(num2 / 10);
					everyActActiveData.NumTwo = (long)(num2 % 10);
				}
				else if (EverydayActNeedType.EANT_Ring == type)
				{
					int num2 = (int)everyActActiveData.NumOne * MarriageOtherLogic.getInstance().GetMaxGoodwillStar() + (int)everyActActiveData.NumTwo;
					num2 /= num;
					everyActActiveData.NumOne = (long)(num2 / MarriageOtherLogic.getInstance().GetMaxGoodwillStar());
					everyActActiveData.NumTwo = (long)(num2 % MarriageOtherLogic.getInstance().GetMaxGoodwillStar());
				}
				else if (EverydayActNeedType.EANT_Merlin == type)
				{
					int num2 = (int)everyActActiveData.NumOne * MerlinSystemParamsConfigData._MaxStarNum + (int)everyActActiveData.NumTwo;
					num2 /= num;
					everyActActiveData.NumOne = (long)(num2 / MerlinSystemParamsConfigData._MaxStarNum);
					everyActActiveData.NumTwo = (long)(num2 % MerlinSystemParamsConfigData._MaxStarNum);
				}
				else if (EverydayActNeedType.EANT_GuardStatue == type)
				{
					int num2 = (int)everyActActiveData.NumTwo;
					num2 /= num;
					everyActActiveData.NumOne = (long)(num2 / 10 + 1);
					everyActActiveData.NumTwo = (long)(num2 % 10);
				}
			}
			this.ActiveConditionDict[(int)type] = everyActActiveData;
		}

		private bool CheckNeedCondition(EverydayActivityGroupConfig myActGroupConfig)
		{
			EveryActActiveData everyActActiveData = null;
			if (!this.ActiveConditionDict.TryGetValue(myActGroupConfig.NeedType, out everyActActiveData))
			{
				everyActActiveData = new EveryActActiveData();
			}
			return this.CheckFirstSecondCondition((int)everyActActiveData.NumOne, (int)everyActActiveData.NumTwo, myActGroupConfig.NeedNum);
		}

		private void CleanUpActGroupInfo(List<EverydayActGroupInfoDB> ActGroupInfoList, int CleanDayNum)
		{
			int NowDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			ActGroupInfoList.RemoveAll((EverydayActGroupInfoDB x) => NowDay - x.ActiveDay > CleanDayNum);
		}

		private List<EverydayActGroupInfoDB> GetCurrentActGroupInfo()
		{
			List<EverydayActGroupInfoDB> list = new List<EverydayActGroupInfoDB>();
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("everydayact", "");
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
						list.Add(new EverydayActGroupInfoDB
						{
							ActiveDay = Global.SafeConvertToInt32(array3[0]),
							GroupID = Global.SafeConvertToInt32(array3[1])
						});
					}
				}
			}
			this.CleanUpActGroupInfo(list, 2);
			return list;
		}

		private void SaveCurrentActGroupInfo(List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			string text = "";
			foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in ActGroupInfoList)
			{
				text += string.Format("{0},{1}|", everydayActGroupInfoDB.ActiveDay, everydayActGroupInfoDB.GroupID);
			}
			if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
			{
				text = text.Substring(0, text.Length - 1);
			}
			GameManager.GameConfigMgr.SetGameConfigItem("everydayact", text);
			Global.UpdateDBGameConfigg("everydayact", text);
		}

		private List<int> FilterValidGroupIDList(List<int> GroupIDList, int TypeID)
		{
			List<int> list = new List<int>(GroupIDList);
			list.RemoveAll(delegate(int item)
			{
				EverydayActivityGroupConfig everydayActivityGroupConfig = null;
				return !this.ActivityGroupConfigDict.TryGetValue(item, out everydayActivityGroupConfig) || everydayActivityGroupConfig.TypeID != TypeID;
			});
			return list;
		}

		private HashSet<int> FilterValidTypeIDSet(HashSet<int> TypeIDSet, List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			HashSet<int> hashSet = new HashSet<int>(TypeIDSet);
			hashSet.RemoveWhere(delegate(int item)
			{
				bool flag = true;
				foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in ActGroupInfoList)
				{
					EverydayActivityGroupConfig everydayActivityGroupConfig = null;
					if (this.ActivityGroupConfigDict.TryGetValue(everydayActGroupInfoDB.GroupID, out everydayActivityGroupConfig))
					{
						if (item == everydayActivityGroupConfig.TypeID)
						{
							flag = false;
							break;
						}
					}
				}
				return !flag;
			});
			return hashSet;
		}

		private void GenerateEverydayActGroupID(List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in ActGroupInfoList)
			{
				if (offsetDay == everydayActGroupInfoDB.ActiveDay)
				{
					return;
				}
			}
			for (int i = 0; i < 23; i++)
			{
				EverydayActNeedType everydayActNeedType = this.ConvertPaiHangTypesToActNeedType((PaiHangTypes)i);
				if (EverydayActNeedType.EANT_Null != everydayActNeedType)
				{
					string cmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
					{
						0,
						i,
						100
					});
					PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, cmd, 0);
					if (null == paiHangData)
					{
						this.ActiveConditionDict.Clear();
						return;
					}
					if (null != paiHangData.PaiHangList)
					{
						this.CacheNeedCondition(everydayActNeedType, paiHangData);
					}
				}
			}
			EveryActActiveData everyActActiveData = null;
			if (this.ActiveConditionDict.TryGetValue(2, out everyActActiveData))
			{
				HashSet<int> hashSet = new HashSet<int>();
				List<int> list = new List<int>();
				foreach (EverydayActivityGroupConfig everydayActivityGroupConfig in this.ActivityGroupConfigDict.Values)
				{
					EverydayActivityGroupConfig everydayActivityGroupConfig2 = everydayActivityGroupConfig;
					EverydayActivityTypeConfig everydayActivityTypeConfig = null;
					if (this.ActivityTypeConfigDict.TryGetValue(everydayActivityGroupConfig2.TypeID, out everydayActivityTypeConfig))
					{
						if (this.CheckFirstSecondCondition((int)everyActActiveData.NumOne, (int)everyActActiveData.NumTwo, everydayActivityTypeConfig.LevLimit))
						{
							if (this.CheckNeedCondition(everydayActivityGroupConfig2))
							{
								hashSet.Add(everydayActivityGroupConfig2.TypeID);
								list.Add(everydayActivityGroupConfig2.GroupID);
							}
						}
					}
				}
				int num = 0;
				List<EverydayActGroupInfoDB> actGroupInfoList = new List<EverydayActGroupInfoDB>(ActGroupInfoList);
				for (int j = 2; j >= 0; j--)
				{
					this.CleanUpActGroupInfo(actGroupInfoList, j);
					HashSet<int> hashSet2 = this.FilterValidTypeIDSet(hashSet, actGroupInfoList);
					if (hashSet2.Count != 0)
					{
						int[] array = hashSet2.ToArray<int>();
						int randomNumber = Global.GetRandomNumber(0, array.Length);
						int typeID = array[randomNumber];
						List<int> list2 = this.FilterValidGroupIDList(list, typeID);
						if (list2.Count != 0)
						{
							randomNumber = Global.GetRandomNumber(0, list2.Count);
							num = list2[randomNumber];
							break;
						}
					}
				}
				EverydayActivityGroupConfig everydayActivityGroupConfig3 = null;
				if (this.ActivityGroupConfigDict.TryGetValue(num, out everydayActivityGroupConfig3))
				{
					ActGroupInfoList.Add(new EverydayActGroupInfoDB
					{
						GroupID = num,
						ActiveDay = offsetDay
					});
				}
				this.SaveCurrentActGroupInfo(ActGroupInfoList);
			}
		}

		private void GenerateEverydayActivity(GameClient client)
		{
			if (this.PlatformOpenStateVavle == 1 && !client.ClientSocket.IsKuaFuLogin)
			{
				List<EverydayActGroupInfoDB> list = null;
				lock (this.Mutex)
				{
					List<EverydayActGroupInfoDB> currentActGroupInfo = this.GetCurrentActGroupInfo();
					this.GenerateEverydayActGroupID(currentActGroupInfo);
					list = new List<EverydayActGroupInfoDB>(currentActGroupInfo);
					this.CleanUpActGroupInfo(list, 0);
				}
				lock (client.ClientData)
				{
					if (null == client.ClientData.EverydayActInfoDict)
					{
						client.ClientData.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
					}
					Dictionary<int, EverydayActInfoDB> dictionary = new Dictionary<int, EverydayActInfoDB>(client.ClientData.EverydayActInfoDict);
					using (Dictionary<int, EverydayActInfoDB>.ValueCollection.Enumerator enumerator = client.ClientData.EverydayActInfoDict.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EverydayActInfoDB info = enumerator.Current;
							if (!list.Exists((EverydayActGroupInfoDB x) => x.GroupID == info.GroupID && x.ActiveDay == info.ActiveDay))
							{
								dictionary.Remove(info.ActID);
								this.DeleteClientEverydayActData(client, info.GroupID, 0);
							}
						}
					}
					foreach (EverydayActGroupInfoDB everydayActGroupInfoDB in list)
					{
						EverydayActivityGroupConfig everydayActivityGroupConfig = null;
						if (this.ActivityGroupConfigDict.TryGetValue(everydayActGroupInfoDB.GroupID, out everydayActivityGroupConfig))
						{
							using (Dictionary<int, EverydayActInfoDB>.ValueCollection.Enumerator enumerator = client.ClientData.EverydayActInfoDict.Values.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									EverydayActInfoDB info = enumerator.Current;
									if (everydayActGroupInfoDB.GroupID == info.GroupID)
									{
										if (!everydayActivityGroupConfig.ActivityIDList.Exists((int x) => x == info.ActID))
										{
											dictionary.Remove(info.ActID);
											this.DeleteClientEverydayActData(client, info.GroupID, info.ActID);
										}
									}
								}
							}
							foreach (int num in everydayActivityGroupConfig.ActivityIDList)
							{
								EverydayActivityConfig everydayActivityConfig = null;
								if (this.ActivityConfigDict.TryGetValue(num, out everydayActivityConfig))
								{
									if (everydayActivityConfig.Type != 14 || UserMoneyMgr.getInstance().PlatformOpenStateVavle != 0)
									{
										EverydayActInfoDB everydayActInfoDB = null;
										if (!dictionary.TryGetValue(num, out everydayActInfoDB))
										{
											everydayActInfoDB = new EverydayActInfoDB
											{
												GroupID = everydayActGroupInfoDB.GroupID,
												ActID = num,
												ActiveDay = everydayActGroupInfoDB.ActiveDay
											};
											dictionary[everydayActInfoDB.ActID] = everydayActInfoDB;
											this.UpdateClientEverydayActData(client, everydayActInfoDB);
										}
									}
								}
							}
						}
					}
					client.ClientData.EverydayActInfoDict = dictionary;
				}
			}
		}

		private EverydayActNeedType ConvertPaiHangTypesToActNeedType(PaiHangTypes type)
		{
			EverydayActNeedType result = EverydayActNeedType.EANT_Null;
			switch (type)
			{
			case PaiHangTypes.RoleLevel:
				result = EverydayActNeedType.EANT_Level;
				break;
			case PaiHangTypes.CombatForceList:
				result = EverydayActNeedType.EANT_CombatForce;
				break;
			case PaiHangTypes.Wing:
				result = EverydayActNeedType.EANT_Wing;
				break;
			case PaiHangTypes.Ring:
				result = EverydayActNeedType.EANT_Ring;
				break;
			case PaiHangTypes.Merlin:
				result = EverydayActNeedType.EANT_Merlin;
				break;
			case PaiHangTypes.UserMoney:
				result = EverydayActNeedType.EANT_UserMoney;
				break;
			case PaiHangTypes.ChengJiu:
				result = EverydayActNeedType.EANT_ChengJiu;
				break;
			case PaiHangTypes.ShengWang:
				result = EverydayActNeedType.EANT_ShengWang;
				break;
			case PaiHangTypes.GuardStatue:
				result = EverydayActNeedType.EANT_GuardStatue;
				break;
			case PaiHangTypes.HolyItem:
				result = EverydayActNeedType.EANT_HolyItem;
				break;
			}
			return result;
		}

		private void DeleteClientEverydayActData(GameClient client, int GroupID, int ActID = 0)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, GroupID, ActID);
			Global.ExecuteDBCmd(13171, strcmd, client.ServerId);
		}

		private void UpdateClientEverydayActData(GameClient client, EverydayActInfoDB EverydayActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				EverydayActData.GroupID,
				EverydayActData.ActID,
				EverydayActData.PurNum,
				EverydayActData.CountNum,
				EverydayActData.ActiveDay
			});
			Global.ExecuteDBCmd(13170, strcmd, client.ServerId);
		}

		private bool CheckFirstSecondCondition(int FirstValue, int SecondValue, EveryActLimitData Limit)
		{
			if (Limit.MinFirst != -1)
			{
				if (FirstValue < Limit.MinFirst || (Limit.MinSecond != -1 && FirstValue == Limit.MinFirst && SecondValue < Limit.MinSecond))
				{
					return false;
				}
			}
			if (Limit.MaxFirst != -1)
			{
				if (FirstValue > Limit.MaxFirst || (Limit.MaxSecond != -1 && FirstValue == Limit.MaxFirst && SecondValue > Limit.MaxSecond))
				{
					return false;
				}
			}
			return true;
		}

		private int CalMyGuardStatueLevel(GameClient client)
		{
			GuardStatueData guardStatue = client.ClientData.MyGuardStatueDetail.GuardStatue;
			int result;
			if (guardStatue.Level > 0 && guardStatue.Level % 10 == 0 && (guardStatue.Level + 10) / 10 != guardStatue.Suit)
			{
				result = 10;
			}
			else
			{
				result = guardStatue.Level % 10;
			}
			return result;
		}

		private int GetCurrentEverydayActJiFen(GameClient client, EverydayActivityConfig myActConfig)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] array = Global.ExecuteDBCmd(13172, strcmd, client.ServerId);
			int result;
			if (array == null || array.Length < 2)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(array[1]);
			}
			return result;
		}

		private bool SubEverydayActJiFen(GameClient client, EverydayActivityConfig myActConfig)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				-myActConfig.Price.NumOne,
				dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'),
				dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$')
			});
			string[] array = Global.ExecuteDBCmd(13173, strcmd, client.ServerId);
			bool result;
			if (array == null || array.Length < 2)
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(array[1]);
				result = (num >= 0);
			}
			return result;
		}

		private EveryActGoalData GetCurrentGoalNum(GameClient client, EverydayActInfoDB mySaveData, EverydayActivityConfig myActConfig)
		{
			EveryActGoalData everyActGoalData = new EveryActGoalData();
			switch (myActConfig.Type)
			{
			case 1:
				everyActGoalData.NumOne = client.ClientData.UserMoney;
				break;
			case 2:
				everyActGoalData.NumOne = this.GetCurrentEverydayActJiFen(client, myActConfig);
				break;
			case 3:
				everyActGoalData.NumOne = mySaveData.CountNum;
				break;
			case 5:
				everyActGoalData.NumOne = client.ClientData.ChangeLifeCount;
				everyActGoalData.NumTwo = client.ClientData.Level;
				break;
			case 6:
				if (client.ClientData.MyWingData != null)
				{
					everyActGoalData.NumOne = client.ClientData.MyWingData.WingID;
					everyActGoalData.NumTwo = client.ClientData.MyWingData.ForgeLevel;
				}
				break;
			case 7:
				everyActGoalData.NumOne = client.ClientData.VipLevel;
				break;
			case 8:
				everyActGoalData.NumOne = ChengJiuManager.GetChengJiuLevel(client);
				break;
			case 9:
				everyActGoalData.NumOne = GameManager.ClientMgr.GetShengWangLevelValue(client);
				break;
			case 10:
				if (client.ClientData.MerlinData != null)
				{
					everyActGoalData.NumOne = client.ClientData.MerlinData._Level;
					everyActGoalData.NumTwo = client.ClientData.MerlinData._StarNum;
				}
				break;
			case 11:
			{
				int num = 0;
				foreach (KeyValuePair<sbyte, HolyItemData> keyValuePair in client.ClientData.MyHolyItemDataDic)
				{
					HolyItemData value = keyValuePair.Value;
					foreach (KeyValuePair<sbyte, HolyItemPartData> keyValuePair2 in value.m_PartArray)
					{
						HolyItemPartData value2 = keyValuePair2.Value;
						num += (int)value2.m_sSuit;
					}
				}
				everyActGoalData.NumOne = num;
				break;
			}
			case 12:
				if (client.ClientData.MyMarriageData != null)
				{
					everyActGoalData.NumOne = (int)client.ClientData.MyMarriageData.byGoodwilllevel;
					everyActGoalData.NumTwo = (int)client.ClientData.MyMarriageData.byGoodwillstar;
				}
				break;
			case 13:
				if (client.ClientData.MyGuardStatueDetail != null)
				{
					everyActGoalData.NumOne = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
					everyActGoalData.NumTwo = this.CalMyGuardStatueLevel(client);
				}
				break;
			}
			return everyActGoalData;
		}

		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			try
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("EveryDayActivityOpen");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
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
							dictionary[Global.SafeConvertToInt32(array3[0])] = Global.SafeConvertToInt32(array3[1]);
						}
					}
				}
				dictionary.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				if (!this.LoadEverydayActivityTypeData())
				{
					return false;
				}
				if (!this.LoadEverydayActivityGroupData())
				{
					return false;
				}
				if (!this.LoadEverydayActivityData())
				{
					return false;
				}
				this.CheckEverydayConfigFileLogic();
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 47;
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private void CheckEverydayConfigFileLogic()
		{
			foreach (EverydayActivityGroupConfig everydayActivityGroupConfig in this.ActivityGroupConfigDict.Values)
			{
				EverydayActivityTypeConfig everydayActivityTypeConfig = null;
				if (!this.ActivityTypeConfigDict.TryGetValue(everydayActivityGroupConfig.TypeID, out everydayActivityTypeConfig))
				{
					LogManager.WriteLog(1000, string.Format("警告：每日活动找不到对应TypeID GroupID={0} TypeID={1}", everydayActivityGroupConfig.GroupID, everydayActivityGroupConfig.TypeID), null, true);
				}
				foreach (int num in everydayActivityGroupConfig.ActivityIDList)
				{
					EverydayActivityConfig everydayActivityConfig = null;
					if (!this.ActivityConfigDict.TryGetValue(num, out everydayActivityConfig))
					{
						LogManager.WriteLog(1000, string.Format("警告：每日活动找不到对应ActivityID GroupID={0} ActivityID={1}", everydayActivityGroupConfig.GroupID, num), null, true);
					}
				}
			}
		}

		private bool ParseEverydayActLimitData(EveryActLimitData LevLimit, string Value)
		{
			bool result;
			if (string.Compare(Value, "-1") == 0 || string.IsNullOrEmpty(Value))
			{
				result = true;
			}
			else
			{
				string[] array = Value.Split(new char[]
				{
					'|'
				});
				if (array.Length != 2)
				{
					result = false;
				}
				else
				{
					string[] array2 = array[0].Split(new char[]
					{
						','
					});
					string[] array3 = array[1].Split(new char[]
					{
						','
					});
					if (array2.Length == 2 && array3.Length == 2)
					{
						LevLimit.MinFirst = Global.SafeConvertToInt32(array2[0]);
						LevLimit.MinSecond = Global.SafeConvertToInt32(array2[1]);
						LevLimit.MaxFirst = Global.SafeConvertToInt32(array3[0]);
						LevLimit.MaxSecond = Global.SafeConvertToInt32(array3[1]);
					}
					else
					{
						if (array2.Length != 1 || array3.Length != 1)
						{
							return false;
						}
						LevLimit.MinFirst = Global.SafeConvertToInt32(array2[0]);
						LevLimit.MaxFirst = Global.SafeConvertToInt32(array3[0]);
					}
					result = true;
				}
			}
			return result;
		}

		public bool LoadEverydayActivityTypeData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityType.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityType.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						EverydayActivityTypeConfig everydayActivityTypeConfig = new EverydayActivityTypeConfig();
						everydayActivityTypeConfig.TypeID = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "OpenLevel");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							everydayActivityTypeConfig.LevLimit.MinFirst = Global.SafeConvertToInt32(array[0]);
							everydayActivityTypeConfig.LevLimit.MinSecond = Global.SafeConvertToInt32(array[1]);
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "CloseLevel");
						array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							everydayActivityTypeConfig.LevLimit.MaxFirst = Global.SafeConvertToInt32(array[0]);
							everydayActivityTypeConfig.LevLimit.MaxSecond = Global.SafeConvertToInt32(array[1]);
						}
						this.ActivityTypeConfigDict[everydayActivityTypeConfig.TypeID] = everydayActivityTypeConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivityType.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEverydayActivityGroupData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityGroup.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityGroup.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						EverydayActivityGroupConfig everydayActivityGroupConfig = new EverydayActivityGroupConfig();
						everydayActivityGroupConfig.GroupID = (int)Global.GetSafeAttributeLong(xelement2, "GroupID");
						everydayActivityGroupConfig.TypeID = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
						everydayActivityGroupConfig.NeedType = (int)Global.GetSafeAttributeLong(xelement2, "NeedType");
						if (!this.ParseEverydayActLimitData(everydayActivityGroupConfig.NeedNum, Global.GetSafeAttributeStr(xelement2, "NeedNum")))
						{
							LogManager.WriteLog(2, string.Format("解析每日活动组文件NeedNum失败 GroupID:{0}", everydayActivityGroupConfig.GroupID), null, true);
						}
						else
						{
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "ActivityID");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							foreach (string str in array)
							{
								everydayActivityGroupConfig.ActivityIDList.Add(Global.SafeConvertToInt32(str));
							}
							this.ActivityGroupConfigDict[everydayActivityGroupConfig.GroupID] = everydayActivityGroupConfig;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivityGroup.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEverydayActivityData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivity.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivity.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						EverydayActivityConfig everydayActivityConfig = new EverydayActivityConfig();
						everydayActivityConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ActivityID");
						everydayActivityConfig.Type = (int)Global.GetSafeAttributeLong(xelement2, "GoalType");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoalNum");
						string[] array = safeAttributeStr.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							everydayActivityConfig.GoalData.NumOne = Global.SafeConvertToInt32(array[0]);
							everydayActivityConfig.GoalData.NumTwo = Global.SafeConvertToInt32(array[1]);
						}
						else
						{
							everydayActivityConfig.GoalData.NumOne = Global.SafeConvertToInt32(array[0]);
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
						string[] array2 = safeAttributeStr2.Split(new char[]
						{
							'|'
						});
						if (array2.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("解析大型每日活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							everydayActivityConfig.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array2, "每日活动配置1");
						}
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
						if (!string.IsNullOrEmpty(safeAttributeStr3))
						{
							array2 = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							everydayActivityConfig.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(array2, "每日活动配置2");
						}
						string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement2, "GoodsThr");
						everydayActivityConfig.GoodsDataListThr.Init(safeAttributeStr4, Global.GetSafeAttributeStr(xelement2, "EffectiveTime"), "每日活动配置3");
						string safeAttributeStr5 = Global.GetSafeAttributeStr(xelement2, "Price");
						string[] array3 = safeAttributeStr5.Split(new char[]
						{
							'|'
						});
						if (array3.Length == 2)
						{
							everydayActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
							everydayActivityConfig.Price.NumTwo = Global.SafeConvertToInt32(array3[1]);
						}
						else if (array3.Length == 3)
						{
							everydayActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
							everydayActivityConfig.Price.NumTwo = Global.SafeConvertToInt32(array3[1]);
							everydayActivityConfig.Price.ZhiGouID = Global.SafeConvertToInt32(array3[2]);
						}
						else
						{
							everydayActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
						}
						everydayActivityConfig.PurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "PurchaseNum");
						if (everydayActivityConfig.Type == 14)
						{
							UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(everydayActivityConfig.Price.ZhiGouID, everydayActivityConfig.PurchaseNum, safeAttributeStr2, safeAttributeStr3, string.Format("每日活动 ID={0}", everydayActivityConfig.ID));
						}
						this.ActivityConfigDict[everydayActivityConfig.ID] = everydayActivityConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivity.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		protected const string EveryDayChongZhiDuiHuan = "EveryDayChongZhiDuiHuan";

		protected const string EveryDayActivityOpen = "EveryDayActivityOpen";

		public const string EverydayActivityTypeData_fileName = "Config/EveryDayActivity/EveryDayActivityType.xml";

		public const string EverydayActivityGroupData_fileName = "Config/EveryDayActivity/EveryDayActivityGroup.xml";

		public const string EverydayActivityData_fileName = "Config/EveryDayActivity/EveryDayActivity.xml";

		public const int MaxActiveConditionDataNum = 100;

		public object Mutex = new object();

		protected int PlatformOpenStateVavle = 0;

		protected Dictionary<int, EverydayActivityTypeConfig> ActivityTypeConfigDict = new Dictionary<int, EverydayActivityTypeConfig>();

		protected Dictionary<int, EverydayActivityGroupConfig> ActivityGroupConfigDict = new Dictionary<int, EverydayActivityGroupConfig>();

		protected Dictionary<int, EverydayActivityConfig> ActivityConfigDict = new Dictionary<int, EverydayActivityConfig>();

		protected Dictionary<int, EveryActActiveData> ActiveConditionDict = new Dictionary<int, EveryActActiveData>();
	}
}
