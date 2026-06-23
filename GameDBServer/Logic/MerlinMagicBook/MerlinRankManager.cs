using System;
using System.Collections.Generic;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	public class MerlinRankManager : IManager
	{
		public static MerlinRankManager getInstance()
		{
			return MerlinRankManager.instance;
		}

		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		private void initCmdProcessor()
		{
		}

		private void initData()
		{
			List<MerlinRankingInfo> playerMerlinDataList = MerlinRankDBController.getInstance().getPlayerMerlinDataList();
			if (null != playerMerlinDataList)
			{
				foreach (MerlinRankingInfo merlinRankingInfo in playerMerlinDataList)
				{
					this.playerMerlinDatas.Add(merlinRankingInfo.nRoleID, merlinRankingInfo);
					this.rankingDatas.Add(merlinRankingInfo.getPlayerMerlinRankingData());
				}
			}
		}

		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		private void removeData()
		{
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
			this.removeListener();
			this.removeData();
			return true;
		}

		public List<PaiHangItemData> getRankingList(int pageIndex, int pageShowNum = -1)
		{
			int num = Math.Max(pageShowNum, MerlinRankManager.RankingList_PageShowNum);
			if (num > this.rankingDatas.Count)
			{
				num = this.rankingDatas.Count;
			}
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			lock (this.rankingDatas)
			{
				for (int i = 0; i < num; i++)
				{
					list.Add(this.rankingDatas[i].getPaiHangItemData());
				}
			}
			return list;
		}

		public void ModifyMerlinRankData(MerlinRankingInfo data, bool bIsLogin)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerMerlinRankingData playerMerlinRankingData = this.rankingDatas.Find((PlayerMerlinRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == playerMerlinRankingData)
					{
						if (this.rankingDatas.Count < MerlinRankManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerMerlinRankingData());
						}
						else if (data.nLevel * 20 + data.nStarNum > this.rankingDatas[this.rankingDatas.Count - 1].Level * 20 + this.rankingDatas[this.rankingDatas.Count - 1].StarNum)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerMerlinRankingData());
						}
						try
						{
							this.rankingDatas.Sort();
						}
						catch (Exception e)
						{
							DataHelper.WriteFormatExceptionLog(e, "", false, false);
						}
					}
					else if (!bIsLogin)
					{
						try
						{
							playerMerlinRankingData.UpdateData(data);
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

		public int createMerlinData(int nRoleID)
		{
			MerlinRankingInfo merlinRankingInfo = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.ContainsKey(nRoleID))
				{
					return 0;
				}
				merlinRankingInfo = this.getMerlinData(nRoleID);
				if (null != merlinRankingInfo)
				{
					this.playerMerlinDatas.Add(merlinRankingInfo.nRoleID, merlinRankingInfo);
				}
			}
			if (null != merlinRankingInfo)
			{
				this.ModifyMerlinRankData(merlinRankingInfo, false);
			}
			return 1;
		}

		public MerlinRankingInfo getMerlinData(int nRoleID)
		{
			MerlinRankingInfo result = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.TryGetValue(nRoleID, out result))
				{
					return result;
				}
			}
			return MerlinRankDBController.getInstance().getMerlinDataByRoleID(nRoleID);
		}

		public void onPlayerLogin(int roleId, string strRoleName)
		{
			MerlinRankingInfo merlinRankingInfo = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.TryGetValue(roleId, out merlinRankingInfo))
				{
					return;
				}
			}
			if (null == merlinRankingInfo)
			{
				merlinRankingInfo = MerlinRankDBController.getInstance().getMerlinDataByRoleID(roleId);
				if (null != merlinRankingInfo)
				{
					this.ModifyMerlinRankData(merlinRankingInfo, true);
					lock (this.playerMerlinDatas)
					{
						this.playerMerlinDatas.Add(merlinRankingInfo.nRoleID, merlinRankingInfo);
					}
				}
			}
		}

		public void onPlayerLogout(int roleId)
		{
			MerlinRankingInfo merlinRankingInfo = null;
			lock (this.playerMerlinDatas)
			{
				this.playerMerlinDatas.TryGetValue(roleId, out merlinRankingInfo);
				if (null != merlinRankingInfo)
				{
					this.playerMerlinDatas.Remove(roleId);
				}
			}
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this.playerMerlinDatas)
				{
					MerlinRankingInfo merlinRankingInfo = null;
					if (this.playerMerlinDatas.TryGetValue(roleId, out merlinRankingInfo))
					{
						merlinRankingInfo.strRoleName = newName;
					}
				}
			}
		}

		private static MerlinRankManager instance = new MerlinRankManager();

		public static readonly int RankingList_Max_Num = 100;

		public static readonly int RankingList_PageShowNum = 30;

		private List<PlayerMerlinRankingData> rankingDatas = new List<PlayerMerlinRankingData>();

		private Dictionary<int, MerlinRankingInfo> playerMerlinDatas = new Dictionary<int, MerlinRankingInfo>();
	}
}
