using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.BocaiSys
{
	public class BoCaiBuy2DBList
	{
		private BoCaiBuy2DBList()
		{
		}

		public static BoCaiBuy2DBList getInstance()
		{
			return BoCaiBuy2DBList.instance;
		}

		public void AddData(BuyBoCai2SDB DbData, int num, bool isAdd = true)
		{
			lock (this.mutex)
			{
				this.dataList.RemoveAll((BoCaiBuy2DBData x) => x.data.m_RoleID == DbData.m_RoleID && x.data.strBuyValue.Equals(DbData.strBuyValue) && x.data.DataPeriods == DbData.DataPeriods);
				this.FailDataList.RemoveAll((BoCaiBuy2DBData x) => x.data.m_RoleID == DbData.m_RoleID && x.data.strBuyValue.Equals(DbData.strBuyValue) && x.data.DataPeriods == DbData.DataPeriods);
				if (isAdd)
				{
					BoCaiBuy2DBData item = default(BoCaiBuy2DBData);
					item.data = DbData;
					item.itemNum = num;
					this.dataList.Add(item);
				}
			}
		}

		public void SoptServer()
		{
			try
			{
				lock (this.mutex)
				{
					this.dataList.AddRange(this.FailDataList);
					foreach (BoCaiBuy2DBData boCaiBuy2DBData in this.dataList)
					{
						BuyBoCai2SDB data = boCaiBuy2DBData.data;
						if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, data, 0)))
						{
							LogManager.WriteLog(2, string.Format("[ljl_博彩购买队列]  服务器关闭... 发送失败 {0}，{1},BocaiType={2},DataPeriods={3},BuyNum={4},BuyValue={5}，“<0是发送状态”={6},win={7},send={8}", new object[]
							{
								data.m_RoleName,
								data.m_RoleID,
								data.BocaiType,
								data.DataPeriods,
								data.BuyNum,
								data.strBuyValue,
								boCaiBuy2DBData.itemNum,
								data.IsWin,
								data.IsSend
							}), null, true);
							GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买博彩DB通信失败扣物品成功", data.m_RoleName, data.m_RoleName, "减少", boCaiBuy2DBData.itemNum, data.ZoneID, data.strUserID, -1, data.ServerId, "");
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩购买失败队列]{0}", ex.ToString()), null, true);
			}
		}

		public void BigTimeUpData()
		{
			try
			{
				List<BoCaiBuy2DBData> list = new List<BoCaiBuy2DBData>();
				lock (this.mutex)
				{
					if ((TimeUtil.NowDateTime() - this.upFailDataTime).TotalMinutes > 1.0)
					{
						this.upFailDataTime = TimeUtil.NowDateTime();
						list.AddRange(this.FailDataList);
						this.FailDataList.Clear();
					}
					list.AddRange(this.dataList);
					this.dataList.Clear();
				}
				foreach (BoCaiBuy2DBData item in list)
				{
					BuyBoCai2SDB data = item.data;
					if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, data, 0)))
					{
						LogManager.WriteLog(1, string.Format("[ljl_博彩购买队列] 发送失败 稍后会继续执行 {0}，{1},BocaiType={2},DataPeriods={3},BuyNum={4},BuyValue={5}，“<0是发送状态”={6},win={7},send={8}", new object[]
						{
							data.m_RoleName,
							data.m_RoleID,
							data.BocaiType,
							data.DataPeriods,
							data.BuyNum,
							data.strBuyValue,
							item.itemNum,
							data.IsWin,
							data.IsSend
						}), null, true);
						lock (this.mutex)
						{
							this.FailDataList.Add(item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩购买失败队列]{0}", ex.ToString()), null, true);
			}
		}

		private static BoCaiBuy2DBList instance = new BoCaiBuy2DBList();

		private DateTime upFailDataTime;

		private object mutex = new object();

		private List<BoCaiBuy2DBData> dataList = new List<BoCaiBuy2DBData>();

		private List<BoCaiBuy2DBData> FailDataList = new List<BoCaiBuy2DBData>();
	}
}
