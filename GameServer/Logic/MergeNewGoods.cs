using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Data;

namespace GameServer.Logic
{
	public class MergeNewGoods
	{
		private static CacheMergeItem GetCacheMergeItem(int mergeItemID)
		{
			CacheMergeItem cacheMergeItem = null;
			lock (MergeNewGoods.MergeItemsDict)
			{
				if (MergeNewGoods.MergeItemsDict.TryGetValue(mergeItemID, out cacheMergeItem))
				{
					return cacheMergeItem;
				}
			}
			SystemXmlItem systemXmlItem = null;
			CacheMergeItem result;
			if (!GameManager.systemGoodsMergeItems.SystemXmlItemDict.TryGetValue(mergeItemID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				string text = systemXmlItem.GetStringValue("OrigGoodsIDs").Trim();
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						'|'
					});
					if (null != array)
					{
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Trim().Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								list.Add(Convert.ToInt32(array2[0]));
								list2.Add(Convert.ToInt32(array2[1]));
							}
						}
					}
				}
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				string text2 = systemXmlItem.GetStringValue("destroy").Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[]
					{
						'|'
					});
					if (null != array)
					{
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Trim().Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								dictionary[array2[0]] = Convert.ToInt32(array2[1]);
							}
						}
					}
				}
				List<int> list3 = new List<int>();
				string stringValue = systemXmlItem.GetStringValue("NewGoodsID");
				if (!string.IsNullOrEmpty(stringValue))
				{
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					if (null != array)
					{
						for (int i = 0; i < array.Length; i++)
						{
							int num = Convert.ToInt32(array[i]);
							if (num > 0)
							{
								list3.Add(num);
							}
						}
					}
				}
				cacheMergeItem = new CacheMergeItem
				{
					MergeType = systemXmlItem.GetIntValue("MergeType", -1),
					NewGoodsID = list3,
					OrigGoodsIDList = list,
					OrigGoodsNumList = list2,
					DianJuan = systemXmlItem.GetIntValue("DianJuan", -1),
					Money = systemXmlItem.GetIntValue("Money", -1),
					JingYuan = systemXmlItem.GetIntValue("JingYuan", -1),
					SuccessRate = Global.GMin(systemXmlItem.GetIntValue("SuccessRate", -1), 100),
					DestroyGoodsIDs = dictionary,
					PubStartTime = systemXmlItem.GetStringValue("PubStartTime"),
					PubEndTime = systemXmlItem.GetStringValue("PubEndTime")
				};
				lock (MergeNewGoods.MergeItemsDict)
				{
					MergeNewGoods.MergeItemsDict[mergeItemID] = cacheMergeItem;
				}
				result = cacheMergeItem;
			}
			return result;
		}

		public static int ReloadCacheMergeItems()
		{
			int result = GameManager.systemGoodsMergeItems.ReloadLoadFromXMlFile();
			lock (MergeNewGoods.MergeItemsDict)
			{
				MergeNewGoods.MergeItemsDict.Clear();
			}
			return result;
		}

		private static int CanMergeNewGoods(GameClient client, CacheMergeItem cacheMergeItem, int nMergeTargetItemID, bool bLeftGrid = false)
		{
			if (!string.IsNullOrEmpty(cacheMergeItem.PubStartTime) && !string.IsNullOrEmpty(cacheMergeItem.PubEndTime))
			{
				long num = Global.SafeConvertToTicks(cacheMergeItem.PubStartTime);
				long num2 = Global.SafeConvertToTicks(cacheMergeItem.PubEndTime);
				long num3 = TimeUtil.NOW();
				if (num3 < num || num3 > num2)
				{
					return -50;
				}
			}
			int result;
			if (!Global.CanAddGoods(client, nMergeTargetItemID, 1, 0, "1900-01-01 12:00:00", true, bLeftGrid))
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					int totalBindGoodsCountByID = Global.GetTotalBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					int totalNotBindGoodsCountByID = Global.GetTotalNotBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (totalBindGoodsCountByID + totalNotBindGoodsCountByID < cacheMergeItem.OrigGoodsNumList[i])
					{
						return -2;
					}
				}
				if (cacheMergeItem.DianJuan > 0)
				{
					if (client.ClientData.UserMoney < cacheMergeItem.DianJuan)
					{
						return -3;
					}
				}
				if (cacheMergeItem.Money > 0)
				{
					if (Global.GetTotalBindTongQianAndTongQianVal(client) < cacheMergeItem.Money)
					{
						return -4;
					}
				}
				if (cacheMergeItem.JingYuan > 0)
				{
					if (GameManager.ClientMgr.GetTianDiJingYuanValue(client) < cacheMergeItem.JingYuan)
					{
						return -7;
					}
				}
				result = 0;
			}
			return result;
		}

		private static bool JugeSucess(int mergeItemID, CacheMergeItem cacheMergeItem, int addSuccessPercent)
		{
			int randomNumber = Global.GetRandomNumber(0, 101);
			double num = 0.0;
			if (50 == mergeItemID)
			{
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(13);
					if (null != config)
					{
						num += config.GetMult();
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					num += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_MergeFruit);
				}
			}
			num = Math.Max(1.0, num);
			int num2 = (int)((double)cacheMergeItem.SuccessRate * num);
			return randomNumber <= num2 + addSuccessPercent;
		}

		private static int GetUsingGoodsNum(bool sucesss, CacheMergeItem cacheMergeItem, int goodsID, int goodsNum)
		{
			int result;
			if (sucesss)
			{
				result = goodsNum;
			}
			else if (!cacheMergeItem.DestroyGoodsIDs.ContainsKey(goodsID.ToString()))
			{
				result = goodsNum;
			}
			else
			{
				cacheMergeItem.DestroyGoodsIDs.TryGetValue(goodsID.ToString(), out goodsNum);
				result = goodsNum;
			}
			return result;
		}

		private static int ProcessMergeNewGoods(GameClient client, int mergeItemID, CacheMergeItem cacheMergeItem, int luckyGoodsID, int nUseBindItemFirst)
		{
			int num = 0;
			int addSuccessPercent = 0;
			bool bLeftGrid = false;
			int num2 = cacheMergeItem.NewGoodsID[0];
			if (cacheMergeItem.NewGoodsID.Count > 1)
			{
				if (!Global.CanAddGoodsNum(client, 1))
				{
					return -1;
				}
				num2 = cacheMergeItem.NewGoodsID[Global.GetRandomNumber(0, cacheMergeItem.NewGoodsID.Count)];
				bLeftGrid = true;
			}
			int num3 = MergeNewGoods.CanMergeNewGoods(client, cacheMergeItem, num2, bLeftGrid);
			int result;
			if (num3 < 0)
			{
				result = num3;
			}
			else
			{
				if (luckyGoodsID > 0)
				{
					int luckyValue = Global.GetLuckyValue(luckyGoodsID);
					if (luckyValue > 0)
					{
						bool flag = false;
						bool flag2 = false;
						if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out flag, out flag2, false))
						{
							if (num <= 0)
							{
								num = (flag ? 1 : 0);
							}
							addSuccessPercent = luckyValue;
						}
					}
				}
				bool flag3 = MergeNewGoods.JugeSucess(mergeItemID, cacheMergeItem, addSuccessPercent);
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					int usingGoodsNum = MergeNewGoods.GetUsingGoodsNum(flag3, cacheMergeItem, cacheMergeItem.OrigGoodsIDList[i], cacheMergeItem.OrigGoodsNumList[i]);
					int totalBindGoodsCountByID = Global.GetTotalBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					int totalNotBindGoodsCountByID = Global.GetTotalNotBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (usingGoodsNum > totalBindGoodsCountByID + totalNotBindGoodsCountByID)
					{
						return -10;
					}
					bool flag = false;
					bool flag2 = false;
					if (nUseBindItemFirst > 0 && totalBindGoodsCountByID > 0)
					{
						int num4;
						int num5;
						if (usingGoodsNum > totalBindGoodsCountByID)
						{
							num4 = totalBindGoodsCountByID;
							num5 = usingGoodsNum - totalBindGoodsCountByID;
						}
						else
						{
							num4 = usingGoodsNum;
							num5 = 0;
						}
						if (num4 > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], num4, false, out flag, out flag2, true))
							{
								return -10;
							}
						}
						if (num5 > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], num5, false, out flag, out flag2, true))
							{
								return -10;
							}
						}
						num = 1;
					}
					else
					{
						int num4;
						int num5;
						if (usingGoodsNum > totalNotBindGoodsCountByID)
						{
							num4 = totalNotBindGoodsCountByID;
							num5 = usingGoodsNum - totalNotBindGoodsCountByID;
						}
						else
						{
							num4 = usingGoodsNum;
							num5 = 0;
						}
						if (num4 > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], num4, false, out flag, out flag2, true))
							{
								return -10;
							}
						}
						if (num5 > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], num5, false, out flag, out flag2, true))
							{
								return -10;
							}
							num = 1;
						}
					}
				}
				if (cacheMergeItem.DianJuan > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.DianJuan, "合成新物品", true, true, false, DaiBiSySType.None))
					{
						return -11;
					}
				}
				if (cacheMergeItem.Money > 0)
				{
					if (!Global.SubBindTongQianAndTongQian(client, cacheMergeItem.Money, "材料合成"))
					{
						return -12;
					}
				}
				if (cacheMergeItem.JingYuan > 0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -cacheMergeItem.JingYuan, "材料合成", true, true, false);
				}
				if (!flag3)
				{
					result = -1000;
				}
				else
				{
					int num6 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num2, 1, 0, "", 0, num, 0, "", true, 1, "材料合成新物品", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					if (num6 < 0)
					{
						result = -20;
					}
					else
					{
						if (90 == Global.GetGoodsCatetoriy(num2))
						{
							if (Global.GetJewelLevel(num2) >= 6)
							{
								Global.BroadcastMergeJewelOk(client, num2);
							}
						}
						if (120 == Global.GetGoodsCatetoriy(num2))
						{
						}
						ChengJiuManager.OnFirstHeCheng(client);
						ChengJiuManager.OnRoleGoodsHeCheng(client, num2);
						SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.HeChengTimes);
						sevenDayGoalEventObject.Arg1 = num2;
						GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
						ProcessTask.ProcessAddTaskVal(client, TaskTypes.Merge_GuoShi, cacheMergeItem.MergeType, 1, new object[0]);
						result = 0;
					}
				}
			}
			return result;
		}

		public static int Process(GameClient client, int mergeItemID, int luckyGoodsID, int WingDBID, int CrystalDBID, int nUseBindItemFirst)
		{
			CacheMergeItem cacheMergeItem = MergeNewGoods.GetCacheMergeItem(mergeItemID);
			int result;
			if (null == cacheMergeItem)
			{
				result = -1000;
			}
			else
			{
				if (mergeItemID >= 4 && mergeItemID <= 6)
				{
					int num = MergeNewGoods.CanMergeNewGoods(client, cacheMergeItem, cacheMergeItem.NewGoodsID[0], false);
					if (num < 0)
					{
						return num;
					}
					num = MergeNewGoods.ProcessWingMerge(client, mergeItemID, luckyGoodsID, WingDBID, CrystalDBID, cacheMergeItem);
					if (num < 0)
					{
						return num;
					}
					ChengJiuManager.OnFirstHeCheng(client);
					ChengJiuManager.OnRoleGoodsHeCheng(client, cacheMergeItem.NewGoodsID[0]);
				}
				else
				{
					int num = MergeNewGoods.ProcessMergeNewGoods(client, mergeItemID, cacheMergeItem, luckyGoodsID, nUseBindItemFirst);
					if (num < 0)
					{
						return num;
					}
				}
				result = 0;
			}
			return result;
		}

		public static int ProcessWingMerge(GameClient client, int mergeItemID, int luckyGoodsID, int WingDBID, int CrystalDBID, CacheMergeItem cacheMergeItem)
		{
			GoodsData goodsData = null;
			if (mergeItemID == 5 || mergeItemID == 6)
			{
				if (WingDBID < 0)
				{
					return -304;
				}
				goodsData = Global.GetGoodsByDbID(client, WingDBID);
				if (goodsData == null)
				{
					return -305;
				}
			}
			bool flag = false;
			bool flag2 = false;
			if (cacheMergeItem.OrigGoodsIDList != null)
			{
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					GoodsData goodsByID = Global.GetGoodsByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (null == goodsByID)
					{
						return -301;
					}
					if (goodsByID.GCount < cacheMergeItem.OrigGoodsNumList[i])
					{
						return -301;
					}
				}
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], cacheMergeItem.OrigGoodsNumList[i], false, out flag, out flag2, true))
					{
						return -301;
					}
				}
			}
			int num = 0;
			List<int> crystalIDForWingMerge = MergeNewGoods.GetCrystalIDForWingMerge(client, mergeItemID);
			int result;
			if (crystalIDForWingMerge == null)
			{
				result = -302;
			}
			else
			{
				int num2 = -1;
				bool flag3 = false;
				bool flag4 = false;
				if (CrystalDBID > 0)
				{
					GoodsData goodsByDbID = Global.GetGoodsByDbID(client, CrystalDBID);
					if (goodsByDbID != null)
					{
						num2 = goodsByDbID.GoodsID;
						if (crystalIDForWingMerge.Count > 0 && !crystalIDForWingMerge.Contains(num2))
						{
							return -302;
						}
						if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByDbID.GoodsID, 1, false, out flag3, out flag4, false))
						{
							if (num <= 0)
							{
								num = (flag3 ? 1 : 0);
							}
						}
					}
				}
				if (!MergeNewGoods.RollWingMergeSuccess(client, cacheMergeItem, luckyGoodsID))
				{
					result = -300;
				}
				else
				{
					int fianlWingGoodsID = MergeNewGoods.GetFianlWingGoodsID(client, mergeItemID, num2, crystalIDForWingMerge);
					int excellenceProperty = MergeNewGoods.RollWingGoodsExcellenceProperty(mergeItemID);
					int forgeLevel = 0;
					if (goodsData != null)
					{
						forgeLevel = goodsData.Forge_level;
					}
					int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, fianlWingGoodsID, 1, 0, "", forgeLevel, num, 0, "", true, 1, "材料合成新物品--翅膀合成", "1900-01-01 12:00:00", 0, 0, 0, 0, excellenceProperty, 0, 0, null, null, 0, true);
					if (num3 < 0)
					{
						result = -20;
					}
					else
					{
						if (goodsData != null)
						{
							bool flag5 = false;
							bool flag6 = false;
							GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, 1, false, out flag5, out flag6, false);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public static List<int> GetWingIDForWingMerge(GameClient client, int mergeItemID)
		{
			List<int> list = null;
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("HeChengChiBang");
			string[] array = paramValueByName.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (array.Length < 0 || array.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> list2 = new List<List<int>>();
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length != 3)
					{
						return null;
					}
					List<int> list3 = new List<int>();
					int item = -1;
					if (!int.TryParse(array2[0], out item))
					{
						return null;
					}
					list3.Add(item);
					int item2 = -1;
					if (!int.TryParse(array2[1], out item2))
					{
						return null;
					}
					list3.Add(item2);
					int item3 = -1;
					if (!int.TryParse(array2[2], out item3))
					{
						return null;
					}
					list3.Add(item3);
					list2.Add(list3);
				}
				if (mergeItemID == 5)
				{
					result = list2[0];
				}
				else if (mergeItemID == 6)
				{
					result = list2[1];
				}
				else
				{
					result = list;
				}
			}
			return result;
		}

		public static int RollWingGoodsExcellenceProperty(int MergeID)
		{
			int num = 0;
			double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingMergeExcellencePropertyRandomID", ',');
			int result;
			if (paramValueDoubleArrayByName == null || paramValueDoubleArrayByName.Length != 3)
			{
				result = num;
			}
			else
			{
				int num2 = -1;
				if (MergeID == 4)
				{
					num2 = 0;
				}
				else if (MergeID == 5)
				{
					num2 = 1;
				}
				else if (MergeID == 6)
				{
					num2 = 2;
				}
				if (num2 == -1)
				{
					result = num;
				}
				else
				{
					int num3 = (int)paramValueDoubleArrayByName[num2];
					if (num3 == -1)
					{
						result = num;
					}
					else
					{
						ExcellencePropertyGroupItem excellencePropertyGroupItem = GameManager.GoodsPackMgr.GetExcellencePropertyGroupItem(num3);
						if (excellencePropertyGroupItem == null || excellencePropertyGroupItem.ExcellencePropertyItems == null || excellencePropertyGroupItem.Max == null || excellencePropertyGroupItem.Max.Length <= 0)
						{
							result = num;
						}
						else
						{
							int num4 = 0;
							int randomNumber = Global.GetRandomNumber(1, 100001);
							int i;
							for (i = 0; i < excellencePropertyGroupItem.ExcellencePropertyItems.Length; i++)
							{
								if (randomNumber > excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent && randomNumber <= excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent + excellencePropertyGroupItem.ExcellencePropertyItems[i].SelfPercent)
								{
									num4 = excellencePropertyGroupItem.ExcellencePropertyItems[i].Num;
									break;
								}
							}
							List<int> list = new List<int>();
							if (num4 > 0 && num4 <= excellencePropertyGroupItem.Max.Length)
							{
								int num5 = 0;
								do
								{
									int randomNumber2 = Global.GetRandomNumber(0, excellencePropertyGroupItem.Max.Length);
									if (list.IndexOf(randomNumber2) < 0)
									{
										list.Add(randomNumber2);
										num5++;
									}
								}
								while (num5 != num4);
							}
							i = 0;
							while (i < list.Count && i < excellencePropertyGroupItem.Max.Length)
							{
								num |= Global.GetBitValue(excellencePropertyGroupItem.Max[list[i]]);
								i++;
							}
							result = num;
						}
					}
				}
			}
			return result;
		}

		public static bool RollWingMergeSuccess(GameClient client, CacheMergeItem cacheMergeItem, int luckyGoodsID)
		{
			int num = 0;
			int num2 = 0;
			if (luckyGoodsID > 0)
			{
				int luckyValue = Global.GetLuckyValue(luckyGoodsID);
				if (luckyValue > 0)
				{
					bool flag = false;
					bool flag2 = false;
					if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out flag, out flag2, false))
					{
						if (num <= 0)
						{
							int num3 = flag ? 1 : 0;
						}
						num2 = luckyValue;
					}
				}
			}
			int randomNumber = Global.GetRandomNumber(0, 101);
			return randomNumber <= cacheMergeItem.SuccessRate + num2;
		}

		public static List<int> GetCrystalIDForWingMerge(GameClient client, int mergeItemID)
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhiYeHeChengJingShi");
			string[] array = paramValueByName.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (array.Length < 0 || array.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> list = new List<List<int>>();
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length != 3)
					{
						return null;
					}
					List<int> list2 = new List<int>();
					int item = -1;
					if (!int.TryParse(array2[0], out item))
					{
						return null;
					}
					list2.Add(item);
					int item2 = -1;
					if (!int.TryParse(array2[1], out item2))
					{
						return null;
					}
					list2.Add(item2);
					int item3 = -1;
					if (!int.TryParse(array2[2], out item3))
					{
						return null;
					}
					list2.Add(item3);
					list.Add(list2);
				}
				if (mergeItemID == 4)
				{
					result = list[0];
				}
				else if (mergeItemID == 5)
				{
					result = list[1];
				}
				else if (mergeItemID == 6)
				{
					result = list[2];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static List<int> GetWingMergeCreateGoodsID(GameClient client, int mergeItemID)
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("WingMergeCreatedID");
			string[] array = paramValueByName.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (array.Length < 0 || array.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> list = new List<List<int>>();
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length != 3)
					{
						return null;
					}
					List<int> list2 = new List<int>();
					int item = -1;
					if (!int.TryParse(array2[0], out item))
					{
						return null;
					}
					list2.Add(item);
					int item2 = -1;
					if (!int.TryParse(array2[1], out item2))
					{
						return null;
					}
					list2.Add(item2);
					int item3 = -1;
					if (!int.TryParse(array2[2], out item3))
					{
						return null;
					}
					list2.Add(item3);
					list.Add(list2);
				}
				if (mergeItemID == 4)
				{
					result = list[0];
				}
				else if (mergeItemID == 5)
				{
					result = list[1];
				}
				else if (mergeItemID == 6)
				{
					result = list[2];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static int GetFianlWingGoodsID(GameClient client, int mergeItemID, int nGoodsID, List<int> nNeedCrystalID)
		{
			List<int> wingMergeCreateGoodsID = MergeNewGoods.GetWingMergeCreateGoodsID(client, mergeItemID);
			int result;
			if (wingMergeCreateGoodsID == null)
			{
				result = -303;
			}
			else
			{
				int num = -1;
				if (nGoodsID != -1)
				{
					for (int i = 0; i < nNeedCrystalID.Count; i++)
					{
						if (nNeedCrystalID[i] == nGoodsID)
						{
							num = i;
							break;
						}
					}
					if (num == -1)
					{
						return -303;
					}
				}
				else
				{
					num = Global.GetRandomNumber(0, 3);
				}
				if (num < 0 || num > 3)
				{
					num = 0;
				}
				int num2 = wingMergeCreateGoodsID[num];
				result = num2;
			}
			return result;
		}

		private static Dictionary<int, CacheMergeItem> MergeItemsDict = new Dictionary<int, CacheMergeItem>();
	}
}
