using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class BuChangManager
	{
		public static void ResetBuChangItemDict()
		{
			GameManager.SystemBuChang.ReloadLoadFromXMlFile();
			BuChangManager.InitBuChangDict();
		}

		private static void InitBuChangDict()
		{
			lock (BuChangManager._BuChangItemDict)
			{
				BuChangManager._BuChangItemDict.Clear();
			}
			foreach (SystemXmlItem systemXmlItem in GameManager.SystemBuChang.SystemXmlItemDict.Values)
			{
				BuChangItem buChangItem = new BuChangItem
				{
					MinLevel = systemXmlItem.GetIntValue("MinLevel", -1),
					MinZhuanSheng = systemXmlItem.GetIntValue("MinZhuanSheng", -1),
					MaxLevel = systemXmlItem.GetIntValue("MaxLevel", -1),
					MaxZhuanSheng = systemXmlItem.GetIntValue("MaxZhuanSheng", -1),
					Experience = Math.Max(0L, (long)systemXmlItem.GetDoubleValue("AwardExp")),
					MoJing = Math.Max(0, systemXmlItem.GetIntValue("MoJing", -1)),
					GoodsDataList = BuChangManager.ParseGoodsDataList(systemXmlItem.GetStringValue("Goods"))
				};
				int unionLevel = Global.GetUnionLevel(buChangItem.MinZhuanSheng, buChangItem.MinLevel, false);
				int unionLevel2 = Global.GetUnionLevel(buChangItem.MaxZhuanSheng, buChangItem.MaxLevel, false);
				lock (BuChangManager._BuChangItemDict)
				{
					BuChangManager._BuChangItemDict[new RangeKey(unionLevel, unionLevel2, null)] = buChangItem;
				}
			}
		}

		public static BuChangItem GetBuChangItem(int unionLevel)
		{
			BuChangItem result = null;
			lock (BuChangManager._BuChangItemDict)
			{
				if (BuChangManager._BuChangItemDict.TryGetValue(unionLevel, out result))
				{
					return result;
				}
			}
			BuChangManager.InitBuChangDict();
			lock (BuChangManager._BuChangItemDict)
			{
				if (BuChangManager._BuChangItemDict.TryGetValue(unionLevel, out result))
				{
					return result;
				}
			}
			return result;
		}

		public static long GetBuChangExp(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			long result;
			if (null == buChangItem)
			{
				result = 0L;
			}
			else
			{
				result = buChangItem.Experience;
			}
			return result;
		}

		public static int GetBuChangBindYuanBao(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			int result;
			if (null == buChangItem)
			{
				result = 0;
			}
			else
			{
				result = buChangItem.MoJing;
			}
			return result;
		}

		public static List<GoodsData> GetBuChangGoodsDataList(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			List<GoodsData> result;
			if (null == buChangItem)
			{
				result = null;
			}
			else
			{
				result = buChangItem.GoodsDataList;
			}
			return result;
		}

		private static List<GoodsData> ParseGoodsDataList(string goodsIDs)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (string.IsNullOrEmpty(goodsIDs))
			{
				result = list;
			}
			else
			{
				string[] array = goodsIDs.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						int[] array3 = Global.StringArray2IntArray(array2);
						GoodsData newGoodsData = Global.GetNewGoodsData(array3[0], array3[1], 0, array3[3], array3[2], 0, array3[5], 0, array3[6], array3[4], 0);
						list.Add(newGoodsData);
					}
				}
				result = list;
			}
			return result;
		}

		public static bool CanGiveBuChang()
		{
			try
			{
				string timeByBuChang = Global.GetTimeByBuChang(0, 0, 0, 0);
				string timeByBuChang2 = Global.GetTimeByBuChang(1, 23, 59, 59);
				DateTime t = DateTime.Parse(timeByBuChang);
				DateTime t2 = DateTime.Parse(timeByBuChang2);
				if (TimeUtil.NowDateTime() >= t && TimeUtil.NowDateTime() <= t2)
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		public static bool HasEnoughBagSpaceForAwardGoods(GameClient client, BuChangItem buChangItem)
		{
			int count = buChangItem.GoodsDataList.Count;
			return count <= 0 || Global.CanAddGoodsDataList(client, buChangItem.GoodsDataList);
		}

		public static bool CheckGiveBuChang(GameClient client)
		{
			bool result;
			if (!BuChangManager.CanGiveBuChang())
			{
				result = false;
			}
			else
			{
				BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
				if (null == buChangItem)
				{
					result = false;
				}
				else
				{
					DateTime buChangStartDay = Global.GetBuChangStartDay();
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BuChangFlag");
					result = (buChangStartDay.DayOfYear != roleParamsInt32FromDB);
				}
			}
			return result;
		}

		public static void GiveBuChang(GameClient client)
		{
			if (!BuChangManager.CanGiveBuChang())
			{
				GameManager.LuaMgr.Error(client, GLang.GetLang(24, new object[0]), 0);
			}
			else
			{
				BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
				if (null == buChangItem)
				{
					GameManager.LuaMgr.Error(client, GLang.GetLang(25, new object[0]), 0);
				}
				else if (!BuChangManager.HasEnoughBagSpaceForAwardGoods(client, buChangItem))
				{
					GameManager.LuaMgr.Error(client, GLang.GetLang(26, new object[0]), 0);
				}
				else
				{
					DateTime buChangStartDay = Global.GetBuChangStartDay();
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BuChangFlag");
					if (buChangStartDay.DayOfYear == roleParamsInt32FromDB)
					{
						GameManager.LuaMgr.Error(client, GLang.GetLang(27, new object[0]), 0);
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "BuChangFlag", buChangStartDay.DayOfYear, true);
						for (int i = 0; i < buChangItem.GoodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, buChangItem.GoodsDataList[i].GoodsID, buChangItem.GoodsDataList[i].GCount, buChangItem.GoodsDataList[i].Quality, "", buChangItem.GoodsDataList[i].Forge_level, buChangItem.GoodsDataList[i].Binding, 0, "", true, 1, "系统补偿物品", "1900-01-01 12:00:00", buChangItem.GoodsDataList[i].AddPropIndex, buChangItem.GoodsDataList[i].BornIndex, buChangItem.GoodsDataList[i].Lucky, buChangItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
						}
						if (buChangItem.MoJing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, buChangItem.MoJing, "系统补偿", false, true, false);
						}
						if (buChangItem.Experience > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, buChangItem.Experience, false, true, false, "none");
						}
						client._IconStateMgr.CheckBuChangState(client);
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		private static Dictionary<RangeKey, BuChangItem> _BuChangItemDict = new Dictionary<RangeKey, BuChangItem>(new RangeKey(0, 0, null));
	}
}
