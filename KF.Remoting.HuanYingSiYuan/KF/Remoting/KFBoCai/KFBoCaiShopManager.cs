using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	public class KFBoCaiShopManager
	{
		public static KFBoCaiShopManager GetInstance()
		{
			return KFBoCaiShopManager.instance;
		}

		private KFBoCaiShopManager()
		{
		}

		public void InitData()
		{
			try
			{
				lock (this.mutex)
				{
					this.StartTime = TimeUtil.NowDateTime();
					string text = TimeUtil.DataTimeToString(this.StartTime, "yyMMdd");
					KFBoCaiDbManager.DelTableData("t_bocai_shop", string.Format("Periods!='{0}'", text));
					KFBoCaiDbManager.SelectBoCaiShop(text, out this.cacheList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		public void Update()
		{
			try
			{
				if (this.StartTime.Day != TimeUtil.NowDateTime().Day)
				{
					this.StartTime = TimeUtil.NowDateTime();
					lock (this.mutex)
					{
						this.cacheList.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		private bool AddItem(KFBoCaiShopDB Item)
		{
			try
			{
				lock (this.mutex)
				{
					KFBoCaiShopDB kfboCaiShopDB = this.cacheList.Find((KFBoCaiShopDB x) => x.ID == Item.ID && Item.WuPinID == x.WuPinID);
					if (null == kfboCaiShopDB)
					{
						kfboCaiShopDB = Item;
						this.cacheList.Add(Item);
					}
					else
					{
						kfboCaiShopDB.BuyNum += Item.BuyNum;
					}
					KFBoCaiDbManager.ReplaceBoCaiShop(kfboCaiShopDB);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool BuyItem(KFBoCaiShopDB Item, int maxNum)
		{
			try
			{
				lock (this.mutex)
				{
					KFBoCaiShopDB kfboCaiShopDB = this.cacheList.Find((KFBoCaiShopDB x) => x.ID == Item.ID && Item.WuPinID == x.WuPinID);
					if (null == kfboCaiShopDB)
					{
						return this.AddItem(Item);
					}
					if (kfboCaiShopDB.BuyNum + Item.BuyNum <= maxNum)
					{
						return this.AddItem(Item);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private static KFBoCaiShopManager instance = new KFBoCaiShopManager();

		private List<KFBoCaiShopDB> cacheList = new List<KFBoCaiShopDB>();

		private DateTime StartTime;

		public object mutex = new object();
	}
}
