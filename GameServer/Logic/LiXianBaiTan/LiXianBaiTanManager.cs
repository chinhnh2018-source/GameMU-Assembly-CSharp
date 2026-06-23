using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.LiXianBaiTan
{
	public class LiXianBaiTanManager : ScheduleTask, IManager
	{
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		public static LiXianBaiTanManager getInstance()
		{
			return LiXianBaiTanManager._Instance;
		}

		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(this, 0, 30000);
			return true;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			ScheduleExecutor2.Instance.scheduleCancle(this);
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void run()
		{
			this.ProcessQueue();
		}

		public void ProcessQueue()
		{
			long num = TimeUtil.NOW();
			List<LiXianSaleRoleItem> list;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				list = LiXianBaiTanManager._LiXianRoleInfoDict.Values.ToList<LiXianSaleRoleItem>();
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (num - list[i].StartTicks >= (long)list[i].LiXianBaiTanMaxTicks)
				{
					LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(list[i].RoleID);
					if (list[i].LiXianBaiTanMaxTicks >= 43200000)
					{
					}
				}
			}
		}

		public static void AddLiXianSaleGoodsItem(LiXianSaleGoodsItem liXianSaleGoodsItem)
		{
			SaleManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				LiXianBaiTanManager._LiXianSaleGoodsDict[liXianSaleGoodsItem.GoodsDbID] = liXianSaleGoodsItem;
			}
		}

		public static void AddLiXianSaleGoodsItems(GameClient client, int fakeRoleID)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			List<GoodsData> saleGoodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != saleGoodsDataList)
			{
				lock (saleGoodsDataList)
				{
					for (int i = 0; i < saleGoodsDataList.Count; i++)
					{
						LiXianSaleGoodsItem liXianSaleGoodsItem = new LiXianSaleGoodsItem
						{
							GoodsDbID = saleGoodsDataList[i].Id,
							SalingGoodsData = saleGoodsDataList[i],
							ZoneID = client.ClientData.ZoneID,
							UserID = userID,
							RoleID = client.ClientData.RoleID,
							RoleName = client.ClientData.RoleName,
							RoleLevel = client.ClientData.Level
						};
						LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
					}
				}
			}
			int num = (int)SaleManager.MaxSaleGoodsTime;
			num = Math.Min(43200000, num);
			GameManager.ClientMgr.ModifyLiXianBaiTanTicksValue(client, -num, true);
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				LiXianBaiTanManager._LiXianRoleInfoDict[client.ClientData.RoleID] = new LiXianSaleRoleItem
				{
					ZoneID = client.ClientData.ZoneID,
					UserID = userID,
					RoleID = client.ClientData.RoleID,
					RoleName = client.ClientData.RoleName,
					RoleLevel = client.ClientData.Level,
					CurrentGrid = client.CurrentGrid,
					LiXianBaiTanMaxTicks = num,
					StartTicks = TimeUtil.NOW(),
					FakeRoleID = fakeRoleID
				};
			}
		}

		public static LiXianSaleGoodsItem RemoveLiXianSaleGoodsItem(int goodsDbID)
		{
			SaleManager.RemoveSaleGoodsItem(goodsDbID);
			LiXianSaleGoodsItem result;
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				LiXianSaleGoodsItem liXianSaleGoodsItem = null;
				if (LiXianBaiTanManager._LiXianSaleGoodsDict.TryGetValue(goodsDbID, out liXianSaleGoodsItem))
				{
					LiXianBaiTanManager._LiXianSaleGoodsDict.Remove(goodsDbID);
				}
				result = liXianSaleGoodsItem;
			}
			return result;
		}

		public static void RemoveLiXianSaleGoodsItems(GameClient client)
		{
			LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(client.ClientData.RoleID);
		}

		public static void RemoveLiXianSaleGoodsItems(int roleID)
		{
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				List<LiXianSaleGoodsItem> list = new List<LiXianSaleGoodsItem>();
				foreach (LiXianSaleGoodsItem liXianSaleGoodsItem in LiXianBaiTanManager._LiXianSaleGoodsDict.Values)
				{
					if (liXianSaleGoodsItem.RoleID == roleID)
					{
						list.Add(liXianSaleGoodsItem);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					LiXianBaiTanManager._LiXianSaleGoodsDict.Remove(list[i].GoodsDbID);
					SaleManager.RemoveSaleGoodsItem(list[i].GoodsDbID);
				}
			}
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				LiXianBaiTanManager._LiXianRoleInfoDict.Remove(roleID);
			}
		}

		public static void GetBackLiXianSaleLeftTicks(GameClient client)
		{
			LiXianSaleRoleItem liXianSaleRoleItem = null;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				if (LiXianBaiTanManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianSaleRoleItem))
				{
					long num = TimeUtil.NOW();
					long num2 = num - liXianSaleRoleItem.StartTicks;
					if (num2 < (long)liXianSaleRoleItem.LiXianBaiTanMaxTicks)
					{
						num2 = Math.Max(0L, (long)liXianSaleRoleItem.LiXianBaiTanMaxTicks - num2);
						GameManager.ClientMgr.ModifyLiXianBaiTanTicksValue(client, (int)num2, true);
					}
				}
			}
		}

		public static List<GoodsData> GetLiXianSaleGoodsList(int roleID)
		{
			List<GoodsData> list = new List<GoodsData>();
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				List<LiXianSaleGoodsItem> list2 = new List<LiXianSaleGoodsItem>();
				foreach (LiXianSaleGoodsItem liXianSaleGoodsItem in LiXianBaiTanManager._LiXianSaleGoodsDict.Values)
				{
					if (liXianSaleGoodsItem.RoleID == roleID)
					{
						list.Add(liXianSaleGoodsItem.SalingGoodsData);
					}
				}
			}
			return list;
		}

		public static void DelFakeRoleByClient(GameClient client)
		{
			int num = -1;
			LiXianSaleRoleItem liXianSaleRoleItem = null;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				if (!LiXianBaiTanManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianSaleRoleItem))
				{
					return;
				}
				num = liXianSaleRoleItem.FakeRoleID;
			}
			if (num > 0)
			{
				FakeRoleManager.ProcessDelFakeRole(num, false);
			}
		}

		public static int GetLiXianRoleCountByPoint(Point grid)
		{
			int num = 0;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				foreach (LiXianSaleRoleItem liXianSaleRoleItem in LiXianBaiTanManager._LiXianRoleInfoDict.Values)
				{
					if (liXianSaleRoleItem.CurrentGrid.X == grid.X && liXianSaleRoleItem.CurrentGrid.Y == grid.Y)
					{
						num++;
					}
				}
			}
			return num;
		}

		private TaskInternalLock _InternalLock = new TaskInternalLock();

		private static LiXianBaiTanManager _Instance = new LiXianBaiTanManager();

		private static Dictionary<int, LiXianSaleGoodsItem> _LiXianSaleGoodsDict = new Dictionary<int, LiXianSaleGoodsItem>();

		private static Dictionary<int, LiXianSaleRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianSaleRoleItem>();
	}
}
