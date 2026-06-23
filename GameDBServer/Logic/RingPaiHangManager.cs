using System;
using System.Collections.Generic;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class RingPaiHangManager : IManager
	{
		public static RingPaiHangManager getInstance()
		{
			return RingPaiHangManager.instance;
		}

		public bool initialize()
		{
			this.initData();
			return true;
		}

		private void initData()
		{
			List<RingRankingInfo> playerRingDataList = RingPaiHangDBController.getInstance().getPlayerRingDataList();
			if (null != playerRingDataList)
			{
				foreach (RingRankingInfo ringRankingInfo in playerRingDataList)
				{
					this.playerRingDatas.Add(ringRankingInfo.nRoleID, ringRankingInfo);
					this.rankingDatas.Add(ringRankingInfo.getPlayerRingRankingData());
				}
			}
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public List<PaiHangItemData> getRankingList(int pageIndex, int pageShowNum = -1)
		{
			int num = Math.Max(pageShowNum, RingPaiHangManager.RankingList_PageShowNum);
			if (num > this.rankingDatas.Count)
			{
				num = this.rankingDatas.Count;
			}
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			for (int i = 0; i < num; i++)
			{
				list.Add(this.rankingDatas[i].getPaiHangItemData());
			}
			return list;
		}

		public void ModifyRingPaihangData(RingRankingInfo data)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerRingRankingData playerRingRankingData = this.rankingDatas.Find((PlayerRingRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == playerRingRankingData)
					{
						if (this.rankingDatas.Count < RingPaiHangManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerRingRankingData());
						}
						else if (this.CompareTo(data.getPlayerRingRankingData(), this.rankingDatas[this.rankingDatas.Count - 1]) < 1)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerRingRankingData());
						}
					}
					else
					{
						try
						{
							playerRingRankingData.UpdateData(data);
							this.rankingDatas.Sort();
						}
						catch (Exception e)
						{
							DataHelper.WriteFormatExceptionLog(e, "", false, false);
						}
					}
				}
			}
		}

		public int createRingData(int nRoleID, RingRankingInfo data = null)
		{
			lock (this.playerRingDatas)
			{
				if (data == null)
				{
					data = this.getRingData(nRoleID);
				}
				if (data != null && !this.playerRingDatas.ContainsKey(nRoleID))
				{
					this.playerRingDatas.Add(data.nRoleID, data);
				}
			}
			if (null != data)
			{
				this.ModifyRingPaihangData(data);
			}
			return 1;
		}

		public RingRankingInfo getRingData(int nRoleID)
		{
			RingRankingInfo result = null;
			lock (this.playerRingDatas)
			{
				if (this.playerRingDatas.TryGetValue(nRoleID, out result))
				{
					return result;
				}
			}
			return RingPaiHangDBController.getInstance().getRingDataById(nRoleID);
		}

		private int CompareTo(PlayerRingRankingData A, PlayerRingRankingData B)
		{
			int result;
			if (A.GoodWillLevel == B.GoodWillLevel)
			{
				if (A.GoodWillStar == B.GoodWillStar)
				{
					int num = string.Compare(A.RingAddTime, B.RingAddTime);
					result = ((num < 0) ? -1 : ((num == 0) ? 0 : 1));
				}
				else
				{
					result = ((A.GoodWillStar < B.GoodWillStar) ? 1 : -1);
				}
			}
			else
			{
				result = ((A.GoodWillLevel < B.GoodWillLevel) ? 1 : -1);
			}
			return result;
		}

		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			RingRankingInfo ringRankingInfo = null;
			lock (this.playerRingDatas)
			{
				this.playerRingDatas.TryGetValue(roleid, out ringRankingInfo);
			}
			if (ringRankingInfo != null)
			{
				ringRankingInfo.strRoleName = newName;
			}
		}

		private static RingPaiHangManager instance = new RingPaiHangManager();

		public static readonly int RankingList_Max_Num = 100;

		public static readonly int RankingList_PageShowNum = 30;

		private List<PlayerRingRankingData> rankingDatas = new List<PlayerRingRankingData>();

		private Dictionary<int, RingRankingInfo> playerRingDatas = new Dictionary<int, RingRankingInfo>();
	}
}
