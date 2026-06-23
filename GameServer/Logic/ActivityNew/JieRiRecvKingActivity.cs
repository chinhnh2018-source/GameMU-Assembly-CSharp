using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiRecvKingActivity : Activity
	{
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		public void OnRecv(int receiver, int goods, int cnt, int serverId)
		{
			if (this.InActivityTime())
			{
				lock (this._allMemberMutex)
				{
					bool flag2;
					JieriRecvKingItemData roleRecvKingInfo = this.GetRoleRecvKingInfo(receiver, out flag2, serverId);
					if (roleRecvKingInfo == null)
					{
						return;
					}
					if (!flag2)
					{
						roleRecvKingInfo.TotalRecv += cnt;
					}
					bool flag3 = this.orderedRecvList.Any((JieriRecvKingItemData detail1) => detail1.RoleID == receiver);
					bool flag4 = false;
					if (!flag3 && (this.orderedRecvList.Count < this.RANK_LVL_CNT || this.orderedRecvList[this.RANK_LVL_CNT - 1].TotalRecv < roleRecvKingInfo.TotalRecv))
					{
						this.orderedRecvList.Add(roleRecvKingInfo);
						flag4 = true;
					}
					if (flag3 || flag4)
					{
						this.buildRankingList(this.orderedRecvList);
					}
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(receiver);
				if (gameClient != null && gameClient._IconStateMgr.CheckJieriRecvKing(gameClient))
				{
					gameClient._IconStateMgr.AddFlushIconState(14000, gameClient._IconStateMgr.IsAnyJieRiTipActived());
					gameClient._IconStateMgr.SendIconStateToClient(gameClient);
				}
			}
		}

		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiShouQuKing.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiShouQuKing.xml"));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							AwardItem awardItem = new AwardItem();
							AwardItem awardItem2 = new AwardItem();
							AwardEffectTimeItem awardEffectTimeItem = new AwardEffectTimeItem();
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日赠送王活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiShouQuKing.xml"), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日赠送王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "GoodsThr");
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement3, "EffectiveTime");
							awardEffectTimeItem.Init(safeAttributeStr2, safeAttributeStr3, "大型节日赠送王时效性物品活动配置");
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement3, "Ranking");
							string[] array2 = safeAttributeStr4.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									this.allAwardDict.Add(i, awardItem);
									this.occAwardDict.Add(i, awardItem2);
									this.timeAwardDict.Add(i, awardEffectTimeItem);
								}
							}
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiShouQuKing.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				lock (this._allMemberMutex)
				{
					foreach (JieriRecvKingItemData jieriRecvKingItemData in this.orderedRecvList)
					{
						if (jieriRecvKingItemData.RoleID == client.ClientData.RoleID && jieriRecvKingItemData.GetAwardTimes <= 0 && jieriRecvKingItemData.Rank > 0)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public void LoadRankFromDB()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				string cmd = string.Format("{0}:{1}:{2}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'), this.RANK_LVL_CNT);
				List<JieriRecvKingItemData> list = Global.sendToDB<List<JieriRecvKingItemData>, string>(13206, cmd, 0);
				lock (this._allMemberMutex)
				{
					this.recvDict.Clear();
					this.orderedRecvList.Clear();
					if (list != null && list.Count != 0)
					{
						foreach (JieriRecvKingItemData jieriRecvKingItemData in list)
						{
							this.recvDict[jieriRecvKingItemData.RoleID] = jieriRecvKingItemData;
							this.orderedRecvList.Add(jieriRecvKingItemData);
						}
						this.buildRankingList(this.orderedRecvList);
					}
				}
			}
		}

		private void buildRankingList(List<JieriRecvKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriRecvKingItemData left, JieriRecvKingItemData right)
			{
				int result;
				if (left.TotalRecv > right.TotalRecv)
				{
					result = -1;
				}
				else if (left.TotalRecv == right.TotalRecv)
				{
					result = left.RoleID - right.RoleID;
				}
				else
				{
					result = 1;
				}
				return result;
			});
			int num = 0;
			int i = 1;
			while (i <= this.RANK_LVL_CNT && num < rankingList.Count)
			{
				AwardItem awardItem = null;
				if (this.allAwardDict.TryGetValue(i, out awardItem))
				{
					JieriRecvKingItemData jieriRecvKingItemData = rankingList[num];
					if (jieriRecvKingItemData.TotalRecv >= awardItem.MinAwardCondionValue)
					{
						jieriRecvKingItemData.Rank = i;
						num++;
					}
				}
				i++;
			}
			this.RoleCountInList = num;
			for (i = rankingList.Count - 1; i >= num; i--)
			{
				rankingList[i].Rank = -1;
				if (i >= this.RANK_LVL_CNT)
				{
					rankingList.RemoveAt(i);
				}
			}
		}

		public byte[] QueryActivityInfo(GameClient client)
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this._allMemberMutex)
				{
					return DataHelper.ObjectToBytes<JieriRecvKingData>(new JieriRecvKingData
					{
						MyData = this.GetRoleRecvKingInfo(client.ClientData.RoleID, client.ServerId),
						RankingList = this.orderedRecvList.GetRange(0, this.RoleCountInList)
					});
				}
			}
			return null;
		}

		public string ProcRoleGetAward(GameClient client, int awardid)
		{
			JieriGiveErrorCode jieriGiveErrorCode = JieriGiveErrorCode.Success;
			if (!this.InAwardTime())
			{
				jieriGiveErrorCode = JieriGiveErrorCode.NotAwardTime;
			}
			else if (!this.HasEnoughBagSpaceForAwardGoods(client, awardid))
			{
				jieriGiveErrorCode = JieriGiveErrorCode.NoBagSpace;
			}
			else
			{
				AwardItem awardItem = null;
				AwardItem myAwardItem = null;
				AwardEffectTimeItem awardEffectTimeItem = null;
				if (!this.allAwardDict.TryGetValue(awardid, out awardItem) || !this.occAwardDict.TryGetValue(awardid, out myAwardItem) || !this.timeAwardDict.TryGetValue(awardid, out awardEffectTimeItem))
				{
					jieriGiveErrorCode = JieriGiveErrorCode.ConfigError;
				}
				else
				{
					lock (this._allMemberMutex)
					{
						JieriRecvKingItemData roleRecvKingInfo = this.GetRoleRecvKingInfo(client.ClientData.RoleID, client.ServerId);
						if (roleRecvKingInfo == null || roleRecvKingInfo.TotalRecv < awardItem.MinAwardCondionValue || roleRecvKingInfo.GetAwardTimes > 0 || roleRecvKingInfo.Rank != awardid)
						{
							jieriGiveErrorCode = JieriGiveErrorCode.NotMeetAwardCond;
							goto IL_1DB;
						}
						string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
						string[] array = Global.ExecuteDBCmd(13208, strcmd, client.ServerId);
						if (array == null || array.Length != 1 || Convert.ToInt32(array[0]) <= 0)
						{
							jieriGiveErrorCode = JieriGiveErrorCode.DBFailed;
							goto IL_1DB;
						}
						roleRecvKingInfo.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, awardItem) || !base.GiveAward(client, myAwardItem) || !base.GiveEffectiveTimeAward(client, awardEffectTimeItem.ToAwardItem()))
					{
						LogManager.WriteLog(2, string.Format("发送节日收取王奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					jieriGiveErrorCode = JieriGiveErrorCode.Success;
				}
			}
			IL_1DB:
			if (jieriGiveErrorCode == JieriGiveErrorCode.Success)
			{
				if (client._IconStateMgr.CheckJieriRecvKing(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			return string.Format("{0}:{1}", (int)jieriGiveErrorCode, awardid);
		}

		private JieriRecvKingItemData GetRoleRecvKingInfo(int roleid, int serverId)
		{
			bool flag;
			return this.GetRoleRecvKingInfo(roleid, out flag, serverId);
		}

		private JieriRecvKingItemData GetRoleRecvKingInfo(int roleid, out bool bLoadFromDb, int serverId)
		{
			bLoadFromDb = false;
			JieriRecvKingItemData jieriRecvKingItemData = null;
			if (!this.recvDict.TryGetValue(roleid, out jieriRecvKingItemData))
			{
				string cmd = string.Format("{0}:{1}:{2}", roleid, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				jieriRecvKingItemData = Global.sendToDB<JieriRecvKingItemData, string>(13207, cmd, serverId);
				if (jieriRecvKingItemData != null)
				{
					bLoadFromDb = true;
					this.recvDict[roleid] = jieriRecvKingItemData;
				}
			}
			return jieriRecvKingItemData;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			AwardItem awardItem = null;
			AwardItem awardItem2 = null;
			AwardEffectTimeItem awardEffectTimeItem = null;
			this.allAwardDict.TryGetValue(id, out awardItem);
			this.occAwardDict.TryGetValue(id, out awardItem);
			this.timeAwardDict.TryGetValue(id, out awardEffectTimeItem);
			int num = 0;
			if (awardItem != null && awardItem.GoodsDataList != null)
			{
				num += awardItem.GoodsDataList.Count;
			}
			if (awardItem2 != null && awardItem2.GoodsDataList != null)
			{
				num += awardItem2.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
			}
			if (awardEffectTimeItem != null)
			{
				num += awardEffectTimeItem.GoodsCnt();
			}
			return Global.CanAddGoodsNum(client, num);
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this._allMemberMutex)
				{
					JieriRecvKingItemData jieriRecvKingItemData = null;
					this.recvDict.TryGetValue(roleId, out jieriRecvKingItemData);
					if (jieriRecvKingItemData != null)
					{
						jieriRecvKingItemData.Rolename = newName;
					}
				}
			}
		}

		private const string CfgFile = "Config/JieRiGifts/JieRiShouQuKing.xml";

		private object _allMemberMutex = new object();

		private Dictionary<int, JieriRecvKingItemData> recvDict = new Dictionary<int, JieriRecvKingItemData>();

		private List<JieriRecvKingItemData> orderedRecvList = new List<JieriRecvKingItemData>();

		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		private int RoleCountInList;
	}
}
