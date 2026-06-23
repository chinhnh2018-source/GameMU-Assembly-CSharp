using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class SaleRoleManager
	{
		public static void AddSaleRoleItem(GameClient client)
		{
			lock (SaleRoleManager._SaleRoleDict)
			{
				SaleRoleManager._SaleRoleDict[client.ClientData.RoleID] = client;
				SaleRoleManager._SaleRoleDataList = null;
			}
		}

		public static GameClient RemoveSaleRoleItem(int roleID)
		{
			GameClient result;
			lock (SaleRoleManager._SaleRoleDict)
			{
				GameClient gameClient = null;
				if (SaleRoleManager._SaleRoleDict.TryGetValue(roleID, out gameClient))
				{
					SaleRoleManager._SaleRoleDict.Remove(roleID);
				}
				SaleRoleManager._SaleRoleDataList = null;
				result = gameClient;
			}
			return result;
		}

		public static List<SaleRoleData> GetSaleRoleDataList()
		{
			long saleRoleDataListTicks = TimeUtil.NOW();
			List<SaleRoleData> result;
			lock (SaleRoleManager._SaleRoleDict)
			{
				List<SaleRoleData> list = new List<SaleRoleData>();
				foreach (GameClient gameClient in SaleRoleManager._SaleRoleDict.Values)
				{
					int num = (gameClient.ClientData.SaleGoodsDataList == null) ? 0 : gameClient.ClientData.SaleGoodsDataList.Count;
					if (num > 0)
					{
						list.Add(new SaleRoleData
						{
							RoleID = gameClient.ClientData.RoleID,
							RoleName = Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName),
							RoleLevel = gameClient.ClientData.Level,
							SaleGoodsNum = num
						});
						if (list.Count >= 250)
						{
							break;
						}
					}
				}
				SaleRoleManager._SaleRoleDataList = list;
				SaleRoleManager._SaleRoleDataListTicks = saleRoleDataListTicks;
				result = list;
			}
			return result;
		}

		private static List<SaleRoleData> _SaleRoleDataList = null;

		private static long _SaleRoleDataListTicks = 0L;

		private static Dictionary<int, GameClient> _SaleRoleDict = new Dictionary<int, GameClient>();
	}
}
