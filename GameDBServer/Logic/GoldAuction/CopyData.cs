using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	public class CopyData
	{
		private static void Copy<T>(T sData, ref T rData)
		{
			try
			{
				foreach (FieldInfo fieldInfo in rData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					fieldInfo.SetValue(rData, fieldInfo.GetValue(sData));
				}
			}
			catch (Exception ex)
			{
				rData = default(T);
				LjlLog.WriteLog(LogTypes.Error, ex.ToString(), "");
			}
		}

		public static bool CopyAuctionDBItem(List<GoldAuctionDBItem> sData, ref List<GoldAuctionDBItem> rData)
		{
			try
			{
				foreach (GoldAuctionDBItem goldAuctionDBItem in sData)
				{
					GoldAuctionDBItem goldAuctionDBItem2 = new GoldAuctionDBItem();
					CopyData.Copy<GoldAuctionDBItem>(goldAuctionDBItem, ref goldAuctionDBItem2);
					goldAuctionDBItem2.BuyerData = new AuctionRoleData();
					CopyData.Copy<AuctionRoleData>(goldAuctionDBItem.BuyerData, ref goldAuctionDBItem2.BuyerData);
					if (goldAuctionDBItem2 != null)
					{
						goldAuctionDBItem2.RoleList = new List<AuctionRoleData>();
						foreach (AuctionRoleData sData2 in goldAuctionDBItem.RoleList)
						{
							AuctionRoleData item = new AuctionRoleData();
							CopyData.Copy<AuctionRoleData>(sData2, ref item);
							goldAuctionDBItem2.RoleList.Add(item);
						}
						goldAuctionDBItem2.OldAuctionType = goldAuctionDBItem2.AuctionType;
						rData.Add(goldAuctionDBItem2);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Error, ex.ToString(), "");
			}
			return false;
		}
	}
}
