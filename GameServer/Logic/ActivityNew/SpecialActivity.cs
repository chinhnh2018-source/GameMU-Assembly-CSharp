using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class SpecialActivity : Activity, IEventListener
	{
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int num = -1;
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in chargeItemBaseEventObject.Player.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB value = keyValuePair.Value;
					if (value.Active != 0)
					{
						SpecialActivityConfig specialActivityConfig = null;
						if (this.SpecialActDict.TryGetValue(value.ActID, out specialActivityConfig))
						{
							if (specialActivityConfig.Type == 14 && specialActivityConfig.Price.ZhiGouID == chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID)
							{
								num = value.ActID;
								break;
							}
						}
					}
				}
				if (num != -1)
				{
					string cmdData = this.BuildFetchSpecActAwardCmd(chargeItemBaseEventObject.Player, 0, num);
					chargeItemBaseEventObject.Player.sendCmd<string>(1512, cmdData, false);
				}
			}
		}

		public void OnMoneyChargeEventOnLine(GameClient client, int addMoney)
		{
			int num = this.GenerateSpecActGroupID();
			if (-1 != num)
			{
				SpecialActivityTimeConfig specialActivityTimeConfig = null;
				if (this.SpecialActTimeDict.TryGetValue(num, out specialActivityTimeConfig))
				{
					string text = specialActivityTimeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
					string text2 = specialActivityTimeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						this.OnMoneyChargeEvent(client.strUserID, client.ClientData.RoleID, addMoney, text, text2);
					}
				}
			}
		}

		public void OnMoneyChargeEventOffLine(string userid, int roleid, int addMoney)
		{
			int key = this.GenerateSpecActGroupID();
			SpecialActivityTimeConfig specialActivityTimeConfig = null;
			if (this.SpecialActTimeDict.TryGetValue(key, out specialActivityTimeConfig))
			{
				string fromActDate = specialActivityTimeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string toActDate = specialActivityTimeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				this.OnMoneyChargeEvent(userid, roleid, addMoney, fromActDate, toActDate);
			}
		}

		protected void OnMoneyChargeEvent(string userid, int roleid, int addMoney, string FromActDate, string ToActDate)
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("SpecialChongZhiDuiHuan");
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
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							roleid,
							num3,
							FromActDate.Replace(':', '$'),
							ToActDate.Replace(':', '$')
						});
						Global.ExecuteDBCmd(13163, strcmd, 0);
					}
				}
			}
		}

		public void MoneyConst(GameClient client, int moneyCost)
		{
			if (client.ClientData.SpecActInfoDict != null && client.ClientData.SpecActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB value = keyValuePair.Value;
					if (value.Active != 0)
					{
						SpecialActivityConfig specialActivityConfig = null;
						if (this.SpecialActDict.TryGetValue(value.ActID, out specialActivityConfig))
						{
							if (specialActivityConfig.Type == 3)
							{
								value.CountNum += moneyCost;
								this.UpdateClientSpecActData(client, value);
							}
						}
					}
				}
				if (client._IconStateMgr.CheckSpecialActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public bool CheckIconState(GameClient client)
		{
			bool flag = false;
			bool result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = flag;
			}
			else
			{
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB value = keyValuePair.Value;
					int num = this.SpecActCheckCondition(client, keyValuePair.Key, false);
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

		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			this.GenerateSpecActGroup(client);
			this.NotifyActivityState(client);
		}

		public SpecialActivityData GetSpecialActivityDataForClient(GameClient client)
		{
			SpecialActivityData specialActivityData = new SpecialActivityData();
			specialActivityData.GroupID = this.GetClientSpecActGroupID(client);
			specialActivityData.SpecActInfoList = new List<SpecActInfo>();
			foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
			{
				SpecActInfoDB value = keyValuePair.Value;
				if (value.Active != 0)
				{
					SpecialActivityConfig specialActivityConfig = null;
					if (this.SpecialActDict.TryGetValue(value.ActID, out specialActivityConfig))
					{
						SpecActInfo specActInfo = new SpecActInfo();
						specActInfo.ActID = value.ActID;
						SpecActGoalData currentGoalNum = this.GetCurrentGoalNum(client, value, specialActivityConfig);
						specActInfo.ShowNum = currentGoalNum.NumOne;
						specActInfo.ShowNum2 = currentGoalNum.NumTwo;
						int num = value.PurNum;
						if (specialActivityConfig.Type == 14)
						{
							num = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, specialActivityConfig.Price.ZhiGouID);
						}
						if (specialActivityConfig.PurchaseNum == -1)
						{
							specActInfo.State = ((num == 1) ? 1 : 0);
						}
						else
						{
							specActInfo.LeftPurNum = specialActivityConfig.PurchaseNum - num;
							specActInfo.State = ((specActInfo.LeftPurNum <= 0) ? 1 : 0);
							if (specActInfo.LeftPurNum < 0)
							{
								specActInfo.LeftPurNum = 0;
							}
						}
						if (specialActivityConfig.GoalData.IsValid())
						{
							if (currentGoalNum.NumOne < specialActivityConfig.GoalData.NumOne || (currentGoalNum.NumOne == specialActivityConfig.GoalData.NumOne && currentGoalNum.NumTwo < specialActivityConfig.GoalData.NumTwo))
							{
								specActInfo.State = -1;
							}
						}
						specialActivityData.SpecActInfoList.Add(specActInfo);
					}
				}
			}
			return specialActivityData;
		}

		public int SpecActCheckCondition(GameClient client, int ActID, bool CheckCost = true)
		{
			SpecActInfoDB specActInfoDB = null;
			int result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out specActInfoDB))
			{
				result = -2;
			}
			else if (specActInfoDB.Active == 0)
			{
				result = -2;
			}
			else
			{
				SpecialActivityConfig specialActivityConfig = null;
				if (!this.SpecialActDict.TryGetValue(specActInfoDB.ActID, out specialActivityConfig))
				{
					result = -2;
				}
				else if (specialActivityConfig.Type == 14)
				{
					result = -12;
				}
				else
				{
					DateTime t = TimeUtil.NowDateTime();
					if (t < specialActivityConfig.FromDay || t > specialActivityConfig.ToDay)
					{
						result = -2001;
					}
					else
					{
						SpecActGoalData currentGoalNum = this.GetCurrentGoalNum(client, specActInfoDB, specialActivityConfig);
						if (specialActivityConfig.GoalData.IsValid())
						{
							if (currentGoalNum.NumOne < specialActivityConfig.GoalData.NumOne || (currentGoalNum.NumOne == specialActivityConfig.GoalData.NumOne && currentGoalNum.NumTwo < specialActivityConfig.GoalData.NumTwo))
							{
								return -12;
							}
						}
						if (specialActivityConfig.PurchaseNum == -1)
						{
							if (specActInfoDB.PurNum == 1)
							{
								return -200;
							}
						}
						else if (specialActivityConfig.PurchaseNum - specActInfoDB.PurNum <= 0)
						{
							return -200;
						}
						if (specialActivityConfig.Type == 2)
						{
							if (this.GetCurrentSpecActJiFen(client, specialActivityConfig) < specialActivityConfig.Price.NumOne)
							{
								return -24;
							}
						}
						if (CheckCost && specialActivityConfig.Type == 1)
						{
							if (client.ClientData.UserMoney < specialActivityConfig.Price.NumOne)
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

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int ActID)
		{
			SpecActInfoDB specActInfoDB = null;
			bool result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out specActInfoDB))
			{
				result = false;
			}
			else if (specActInfoDB.Active == 0)
			{
				result = false;
			}
			else
			{
				SpecialActivityConfig specialActivityConfig = null;
				if (!this.SpecialActDict.TryGetValue(specActInfoDB.ActID, out specialActivityConfig))
				{
					result = false;
				}
				else
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> list = new List<GoodsData>();
					foreach (GoodsData item in specialActivityConfig.GoodsDataListOne)
					{
						list.Add(item);
					}
					int count = specialActivityConfig.GoodsDataListTwo.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData goodsData = specialActivityConfig.GoodsDataListTwo[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, goodsData.GoodsID))
						{
							list.Add(goodsData);
						}
					}
					AwardItem awardItem = specialActivityConfig.GoodsDataListThr.ToAwardItem();
					foreach (GoodsData item in awardItem.GoodsDataList)
					{
						list.Add(item);
					}
					result = Global.CanAddGoodsDataList(client, list);
				}
			}
			return result;
		}

		public int SpecActGiveAward(GameClient client, int ActID)
		{
			SpecActInfoDB specActInfoDB = null;
			int result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out specActInfoDB))
			{
				result = -2;
			}
			else if (specActInfoDB.Active == 0)
			{
				result = -2;
			}
			else
			{
				SpecialActivityConfig specialActivityConfig = null;
				if (!this.SpecialActDict.TryGetValue(specActInfoDB.ActID, out specialActivityConfig))
				{
					result = -2;
				}
				else
				{
					string castResList = "";
					if (specialActivityConfig.Type == 2)
					{
						if (!this.SubSpecActJiFen(client, specialActivityConfig))
						{
							return -24;
						}
						int currentSpecActJiFen = this.GetCurrentSpecActJiFen(client, specialActivityConfig);
						castResList = EventLogManager.NewResPropString(ResLogType.SpecJiFen, new object[]
						{
							-specialActivityConfig.Price.NumOne,
							currentSpecActJiFen + specialActivityConfig.Price.NumOne,
							currentSpecActJiFen
						});
					}
					if (specialActivityConfig.Type == 1 && specialActivityConfig.Price.NumOne > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, specialActivityConfig.Price.NumOne, "专属活动抢购", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
						castResList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-specialActivityConfig.Price.NumOne,
							client.ClientData.UserMoney + specialActivityConfig.Price.NumOne,
							client.ClientData.UserMoney
						});
					}
					AwardItem awardItem = new AwardItem();
					awardItem.GoodsDataList = specialActivityConfig.GoodsDataListOne;
					base.GiveAward(client, awardItem);
					awardItem.GoodsDataList = specialActivityConfig.GoodsDataListTwo;
					base.GiveAward(client, awardItem);
					awardItem = specialActivityConfig.GoodsDataListThr.ToAwardItem();
					base.GiveEffectiveTimeAward(client, awardItem);
					if (specialActivityConfig.PurchaseNum == -1)
					{
						specActInfoDB.PurNum = 1;
					}
					else
					{
						specActInfoDB.PurNum++;
					}
					this.UpdateClientSpecActData(client, specActInfoDB);
					if (client._IconStateMgr.CheckSpecialActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string strResList = EventLogManager.MakeGoodsDataPropString(awardItem.GoodsDataList);
					EventLogManager.AddPurchaseEvent(client, 7, specActInfoDB.ActID, castResList, strResList);
					result = 0;
				}
			}
			return result;
		}

		public string BuildFetchSpecActAwardCmd(GameClient client, int ErrCode, int actID)
		{
			int roleID = client.ClientData.RoleID;
			SpecActInfoDB specActInfoDB = null;
			string result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(actID, out specActInfoDB))
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
				SpecialActivityConfig specialActivityConfig = null;
				if (!this.SpecialActDict.TryGetValue(specActInfoDB.ActID, out specialActivityConfig))
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
					if (specialActivityConfig.Type == 14)
					{
						specActInfoDB.PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, specialActivityConfig.Price.ZhiGouID);
					}
					int num = specialActivityConfig.PurchaseNum - specActInfoDB.PurNum;
					SpecActGoalData currentGoalNum = this.GetCurrentGoalNum(client, specActInfoDB, specialActivityConfig);
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

		private int GenerateSpecActGroupID()
		{
			DateTime kaiFuTime = Global.GetKaiFuTime();
			DateTime t = TimeUtil.NowDateTime();
			foreach (KeyValuePair<int, SpecialActivityTimeConfig> keyValuePair in this.SpecialActTimeDict)
			{
				SpecialActivityTimeConfig value = keyValuePair.Value;
				if (!(kaiFuTime < value.ServerOpenFromDate) && !(kaiFuTime > value.ServerOpenToDate))
				{
					if (!(t < value.FromDate) && !(t > value.ToDate))
					{
						return value.GroupID;
					}
				}
			}
			return -1;
		}

		private int GetCurrentSpecActJiFen(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecialActivityTimeConfig specialActivityTimeConfig = null;
			int result;
			if (!this.SpecialActTimeDict.TryGetValue(myActConfig.GroupID, out specialActivityTimeConfig))
			{
				result = 0;
			}
			else
			{
				string text = specialActivityTimeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string text2 = specialActivityTimeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, text.Replace(':', '$'), text2.Replace(':', '$'));
				string[] array = Global.ExecuteDBCmd(13162, strcmd, client.ServerId);
				if (array == null || array.Length < 2)
				{
					result = 0;
				}
				else
				{
					result = Global.SafeConvertToInt32(array[1]);
				}
			}
			return result;
		}

		private bool SubSpecActJiFen(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecialActivityTimeConfig specialActivityTimeConfig = null;
			bool result;
			if (!this.SpecialActTimeDict.TryGetValue(myActConfig.GroupID, out specialActivityTimeConfig))
			{
				result = false;
			}
			else
			{
				string text = specialActivityTimeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string text2 = specialActivityTimeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					-myActConfig.Price.NumOne,
					text.Replace(':', '$'),
					text2.Replace(':', '$')
				});
				string[] array = Global.ExecuteDBCmd(13163, strcmd, client.ServerId);
				if (array == null || array.Length < 2)
				{
					result = false;
				}
				else
				{
					int num = Convert.ToInt32(array[1]);
					result = (num >= 0);
				}
			}
			return result;
		}

		private HashSet<int> GetClientSpecActGroupIDSet(GameClient client)
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = hashSet;
			}
			else
			{
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
				{
					hashSet.Add(keyValuePair.Value.GroupID);
				}
				result = hashSet;
			}
			return result;
		}

		private int GetClientSpecActGroupID(GameClient client)
		{
			int num = -1;
			int result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = num;
			}
			else
			{
				using (Dictionary<int, SpecActInfoDB>.Enumerator enumerator = client.ClientData.SpecActInfoDict.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, SpecActInfoDB> keyValuePair = enumerator.Current;
						num = keyValuePair.Value.GroupID;
					}
				}
				result = num;
			}
			return result;
		}

		private SpecActInfoDB CreatNewSpecAct(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecActInfoDB specActInfoDB = new SpecActInfoDB
			{
				GroupID = myActConfig.GroupID,
				ActID = myActConfig.ID
			};
			if (this.CheckNeedCondition(client, myActConfig))
			{
				specActInfoDB.Active = 1;
			}
			else
			{
				specActInfoDB.Active = 0;
			}
			return specActInfoDB;
		}

		private void GenerateSpecActGroup(GameClient client)
		{
			int num = this.GenerateSpecActGroupID();
			DateTime t = TimeUtil.NowDateTime();
			lock (client.ClientData)
			{
				if (null == client.ClientData.SpecActInfoDict)
				{
					client.ClientData.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
				}
				HashSet<int> clientSpecActGroupIDSet = this.GetClientSpecActGroupIDSet(client);
				foreach (int num2 in clientSpecActGroupIDSet)
				{
					if (num2 != num)
					{
						this.DeleteClientSpecActData(client, num2);
					}
				}
				Dictionary<int, SpecActInfoDB> dictionary = new Dictionary<int, SpecActInfoDB>(client.ClientData.SpecActInfoDict);
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
				{
					if (keyValuePair.Value.GroupID != num)
					{
						dictionary.Remove(keyValuePair.Key);
					}
				}
				foreach (KeyValuePair<int, SpecialActivityConfig> keyValuePair2 in this.SpecialActDict)
				{
					SpecialActivityConfig value = keyValuePair2.Value;
					if (value.GroupID == num)
					{
						if (value.Type != 14 || UserMoneyMgr.getInstance().PlatformOpenStateVavle != 0)
						{
							SpecActInfoDB specActInfoDB = null;
							if (dictionary.TryGetValue(value.ID, out specActInfoDB))
							{
								if (t < value.FromDay || t > value.ToDay)
								{
									specActInfoDB.Active = 0;
									this.UpdateClientSpecActData(client, specActInfoDB);
								}
							}
							else if (!(t < value.FromDay) && !(t > value.ToDay))
							{
								specActInfoDB = this.CreatNewSpecAct(client, value);
								dictionary[specActInfoDB.ActID] = specActInfoDB;
								this.UpdateClientSpecActData(client, specActInfoDB);
							}
						}
					}
				}
				client.ClientData.SpecActInfoDict = dictionary;
			}
		}

		private bool CheckFirstSecondCondition(int FirstValue, int SecondValue, SpecActLimitData Limit)
		{
			return FirstValue >= Limit.MinFirst && (FirstValue != Limit.MinFirst || SecondValue >= Limit.MinSecond) && FirstValue <= Limit.MaxFirst && (FirstValue != Limit.MaxFirst || SecondValue <= Limit.MaxSecond);
		}

		private bool CheckNeedCondition(GameClient client, SpecialActivityConfig myActConfig)
		{
			if (myActConfig.LevLimit.IfValid())
			{
				if (!this.CheckFirstSecondCondition(client.ClientData.ChangeLifeCount, client.ClientData.Level, myActConfig.LevLimit))
				{
					return false;
				}
			}
			if (myActConfig.VipLimit.IfValid())
			{
				if (client.ClientData.VipLevel < myActConfig.VipLimit.MinFirst || client.ClientData.VipLevel > myActConfig.VipLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.ChongZhiLimit.IfValid())
			{
				int money = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
				int num = Global.TransMoneyToYuanBao(money);
				if (num < myActConfig.ChongZhiLimit.MinFirst || num > myActConfig.ChongZhiLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.WingLimit.IfValid())
			{
				if (client.ClientData.MyWingData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, myActConfig.WingLimit))
				{
					return false;
				}
			}
			if (myActConfig.ChengJiuLimit.IfValid())
			{
				int chengJiuLevel = ChengJiuManager.GetChengJiuLevel(client);
				if (chengJiuLevel < myActConfig.ChengJiuLimit.MinFirst || chengJiuLevel > myActConfig.ChengJiuLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.JunXianLimit.IfValid())
			{
				int shengWangLevelValue = GameManager.ClientMgr.GetShengWangLevelValue(client);
				if (shengWangLevelValue < myActConfig.JunXianLimit.MinFirst || shengWangLevelValue > myActConfig.JunXianLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.MerlinLimit.IfValid())
			{
				if (client.ClientData.MerlinData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, myActConfig.MerlinLimit))
				{
					return false;
				}
			}
			if (myActConfig.ShengWuLimit.IfValid())
			{
				int num2 = 0;
				foreach (KeyValuePair<sbyte, HolyItemData> keyValuePair in client.ClientData.MyHolyItemDataDic)
				{
					HolyItemData value = keyValuePair.Value;
					foreach (KeyValuePair<sbyte, HolyItemPartData> keyValuePair2 in value.m_PartArray)
					{
						HolyItemPartData value2 = keyValuePair2.Value;
						num2 += (int)value2.m_sSuit;
					}
				}
				if (num2 < myActConfig.ShengWuLimit.MinFirst || num2 > myActConfig.ShengWuLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.RingLimit.IfValid())
			{
				if (client.ClientData.MyMarriageData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition((int)client.ClientData.MyMarriageData.byGoodwilllevel, (int)client.ClientData.MyMarriageData.byGoodwillstar, myActConfig.RingLimit))
				{
					return false;
				}
			}
			if (myActConfig.ShouHuShenLimit.IfValid())
			{
				if (client.ClientData.MyGuardStatueDetail == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MyGuardStatueDetail.GuardStatue.Suit, this.CalMyGuardStatueLevel(client), myActConfig.ShouHuShenLimit))
				{
					return false;
				}
			}
			return true;
		}

		private SpecActGoalData GetCurrentGoalNum(GameClient client, SpecActInfoDB mySaveData, SpecialActivityConfig myActConfig)
		{
			SpecActGoalData specActGoalData = new SpecActGoalData();
			switch (myActConfig.Type)
			{
			case 1:
				specActGoalData.NumOne = client.ClientData.UserMoney;
				break;
			case 2:
				specActGoalData.NumOne = this.GetCurrentSpecActJiFen(client, myActConfig);
				break;
			case 3:
				specActGoalData.NumOne = mySaveData.CountNum;
				break;
			case 5:
				specActGoalData.NumOne = client.ClientData.ChangeLifeCount;
				specActGoalData.NumTwo = client.ClientData.Level;
				break;
			case 6:
				if (client.ClientData.MyWingData != null)
				{
					specActGoalData.NumOne = client.ClientData.MyWingData.WingID;
					specActGoalData.NumTwo = client.ClientData.MyWingData.ForgeLevel;
				}
				break;
			case 7:
				specActGoalData.NumOne = client.ClientData.VipLevel;
				break;
			case 8:
				specActGoalData.NumOne = ChengJiuManager.GetChengJiuLevel(client);
				break;
			case 9:
				specActGoalData.NumOne = GameManager.ClientMgr.GetShengWangLevelValue(client);
				break;
			case 10:
				if (client.ClientData.MerlinData != null)
				{
					specActGoalData.NumOne = client.ClientData.MerlinData._Level;
					specActGoalData.NumTwo = client.ClientData.MerlinData._StarNum;
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
				specActGoalData.NumOne = num;
				break;
			}
			case 12:
				if (client.ClientData.MyMarriageData != null)
				{
					specActGoalData.NumOne = (int)client.ClientData.MyMarriageData.byGoodwilllevel;
					specActGoalData.NumTwo = (int)client.ClientData.MyMarriageData.byGoodwillstar;
				}
				break;
			case 13:
				if (client.ClientData.MyGuardStatueDetail != null)
				{
					specActGoalData.NumOne = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
					specActGoalData.NumTwo = this.CalMyGuardStatueLevel(client);
				}
				break;
			}
			return specActGoalData;
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

		private void DeleteClientSpecActData(GameClient client, int GroupID = 0)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, GroupID);
			Global.ExecuteDBCmd(13161, strcmd, client.ServerId);
		}

		private void UpdateClientSpecActData(GameClient client, SpecActInfoDB SpecActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				SpecActData.GroupID,
				SpecActData.ActID,
				SpecActData.PurNum,
				SpecActData.CountNum,
				SpecActData.Active
			});
			Global.ExecuteDBCmd(13160, strcmd, client.ServerId);
		}

		public void NotifyActivityState(GameClient client)
		{
			bool flag = false;
			if (client.ClientData.SpecActInfoDict != null && client.ClientData.SpecActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in client.ClientData.SpecActInfoDict)
				{
					if (keyValuePair.Value.Active == 1)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					4,
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
					4,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			try
			{
				if (!this.LoadSpecialActivityTimeData())
				{
					return false;
				}
				if (!this.LoadSpecialActivityData())
				{
					return false;
				}
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 44;
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

		private bool ParseSpecActLimitData(SpecActLimitData LevLimit, string Value)
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

		private bool ParseSpecActDay(int groupID, string Day, SpecialActivityConfig myData)
		{
			SpecialActivityTimeConfig specialActivityTimeConfig = null;
			bool result;
			if (!this.SpecialActTimeDict.TryGetValue(groupID, out specialActivityTimeConfig))
			{
				result = false;
			}
			else if (string.Compare(Day, "-1") == 0 || string.IsNullOrEmpty(Day))
			{
				myData.FromDay = specialActivityTimeConfig.FromDate;
				myData.ToDay = specialActivityTimeConfig.ToDate;
				result = true;
			}
			else
			{
				string[] array = Day.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					int addDays = Global.SafeConvertToInt32(array[0]) - 1;
					int addDays2 = Global.SafeConvertToInt32(array[1]);
					myData.FromDay = Global.GetAddDaysDataTime(specialActivityTimeConfig.FromDate, addDays, true);
					myData.ToDay = Global.GetAddDaysDataTime(specialActivityTimeConfig.FromDate, addDays2, true);
				}
				else
				{
					int addDays = Global.SafeConvertToInt32(array[0]) - 1;
					myData.FromDay = Global.GetAddDaysDataTime(specialActivityTimeConfig.FromDate, addDays, true);
					myData.ToDay = new DateTime(myData.FromDay.Year, myData.FromDay.Month, myData.FromDay.Day, 23, 59, 59);
				}
				result = true;
			}
			return result;
		}

		public bool LoadSpecialActivityData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SpecialActivity/SpecialActivity.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SpecialActivity/SpecialActivity.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecialActivityConfig specialActivityConfig = new SpecialActivityConfig();
						specialActivityConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						specialActivityConfig.GroupID = (int)Global.GetSafeAttributeLong(xelement2, "GroupID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Day");
						if (!this.ParseSpecActDay(specialActivityConfig.GroupID, safeAttributeStr, specialActivityConfig))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件Day失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.LevLimit, Global.GetSafeAttributeStr(xelement2, "NeedLevel")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedLevel失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.VipLimit, Global.GetSafeAttributeStr(xelement2, "NeedVIP")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedVIP失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.ChongZhiLimit, Global.GetSafeAttributeStr(xelement2, "NeedChongZhi")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedChongZhi失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.WingLimit, Global.GetSafeAttributeStr(xelement2, "NeedWing")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedWing失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.ChengJiuLimit, Global.GetSafeAttributeStr(xelement2, "NeedChengJiu")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedChengJiu失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.JunXianLimit, Global.GetSafeAttributeStr(xelement2, "NeedJunXian")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedJunXian失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.MerlinLimit, Global.GetSafeAttributeStr(xelement2, "NeedMerlin")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedMerlin失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.ShengWuLimit, Global.GetSafeAttributeStr(xelement2, "NeedShengWu")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedShengWu失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.RingLimit, Global.GetSafeAttributeStr(xelement2, "NeedRing")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedRing失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(specialActivityConfig.ShouHuShenLimit, Global.GetSafeAttributeStr(xelement2, "NeedShouHuShen")))
						{
							LogManager.WriteLog(2, string.Format("解析专享活动文件NeedShouHuShen失败 ID:{0},GroupID:{1}", specialActivityConfig.ID, specialActivityConfig.GroupID), null, true);
						}
						else
						{
							specialActivityConfig.Type = (int)Global.GetSafeAttributeLong(xelement2, "Type");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "Goal");
							string[] array = safeAttributeStr2.Split(new char[]
							{
								','
							});
							if (array.Length == 2)
							{
								specialActivityConfig.GoalData.NumOne = Global.SafeConvertToInt32(array[0]);
								specialActivityConfig.GoalData.NumTwo = Global.SafeConvertToInt32(array[1]);
							}
							else
							{
								specialActivityConfig.GoalData.NumOne = Global.SafeConvertToInt32(array[0]);
							}
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
							string[] array2 = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							if (array2.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析大型专享活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								specialActivityConfig.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array2, "专享活动配置1");
							}
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr4))
							{
								array2 = safeAttributeStr4.Split(new char[]
								{
									'|'
								});
								specialActivityConfig.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(array2, "专享活动配置2");
							}
							string safeAttributeStr5 = Global.GetSafeAttributeStr(xelement2, "GoodsThr");
							specialActivityConfig.GoodsDataListThr.Init(safeAttributeStr5, Global.GetSafeAttributeStr(xelement2, "EffectiveTime"), "专享活动配置3");
							string safeAttributeStr6 = Global.GetSafeAttributeStr(xelement2, "Price");
							string[] array3 = safeAttributeStr6.Split(new char[]
							{
								'|'
							});
							if (array3.Length == 2)
							{
								specialActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
								specialActivityConfig.Price.NumTwo = Global.SafeConvertToInt32(array3[1]);
							}
							else if (array3.Length == 3)
							{
								specialActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
								specialActivityConfig.Price.NumTwo = Global.SafeConvertToInt32(array3[1]);
								specialActivityConfig.Price.ZhiGouID = Global.SafeConvertToInt32(array3[2]);
							}
							else
							{
								specialActivityConfig.Price.NumOne = Global.SafeConvertToInt32(array3[0]);
							}
							specialActivityConfig.PurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "PurchaseNum");
							if (specialActivityConfig.Type == 14)
							{
								UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(specialActivityConfig.Price.ZhiGouID, specialActivityConfig.PurchaseNum, safeAttributeStr3, safeAttributeStr4, string.Format("专享活动 ID={0}", specialActivityConfig.ID));
							}
							this.SpecialActDict[specialActivityConfig.ID] = specialActivityConfig;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/SpecialActivity/SpecialActivity.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadSpecialActivityTimeData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SpecialActivity/SpecialActivityTime.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SpecialActivity/SpecialActivityTime.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						SpecialActivityTimeConfig specialActivityTimeConfig = new SpecialActivityTimeConfig();
						specialActivityTimeConfig.GroupID = (int)Global.GetSafeAttributeLong(xelement2, "GroupID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "ServerOpenFromDate");
						if (string.Compare(safeAttributeStr, "-1") != 0)
						{
							specialActivityTimeConfig.ServerOpenFromDate = DateTime.Parse(safeAttributeStr);
						}
						else
						{
							specialActivityTimeConfig.ServerOpenFromDate = Global.GetKaiFuTime();
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "ServerOpenToDate");
						if (string.Compare(safeAttributeStr2, "-1") != 0)
						{
							specialActivityTimeConfig.ServerOpenToDate = DateTime.Parse(safeAttributeStr2);
						}
						else
						{
							specialActivityTimeConfig.ServerOpenToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "FromDate");
						if (!string.IsNullOrEmpty(safeAttributeStr3))
						{
							specialActivityTimeConfig.FromDate = DateTime.Parse(safeAttributeStr3);
						}
						else
						{
							specialActivityTimeConfig.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement2, "ToDate");
						if (!string.IsNullOrEmpty(safeAttributeStr4))
						{
							specialActivityTimeConfig.ToDate = DateTime.Parse(safeAttributeStr4);
						}
						else
						{
							specialActivityTimeConfig.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						this.SpecialActTimeDict[specialActivityTimeConfig.GroupID] = specialActivityTimeConfig;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/SpecialActivity/SpecialActivityTime.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		protected const string SpecialChongZhiDuiHuan = "SpecialChongZhiDuiHuan";

		public const string SpecialActivityData_fileName = "Config/SpecialActivity/SpecialActivity.xml";

		public const string SpecialActivityTimeData_fileName = "Config/SpecialActivity/SpecialActivityTime.xml";

		protected Dictionary<int, SpecialActivityTimeConfig> SpecialActTimeDict = new Dictionary<int, SpecialActivityTimeConfig>();

		protected Dictionary<int, SpecialActivityConfig> SpecialActDict = new Dictionary<int, SpecialActivityConfig>();
	}
}
