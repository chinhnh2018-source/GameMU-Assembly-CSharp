using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiGiveKingActivity : Activity
	{
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		public void OnGive(GameClient client, int goods, int cnt)
		{
			if (this.InActivityTime())
			{
				if (client != null)
				{
					lock (this._allMemberMutex)
					{
						bool flag2;
						JieriGiveKingItemData clientGiveKingInfo = this.GetClientGiveKingInfo(client, out flag2);
						if (clientGiveKingInfo == null)
						{
							return;
						}
						if (!flag2)
						{
							clientGiveKingInfo.TotalGive += cnt;
						}
						bool flag3 = this.orderedGiveList.Any((JieriGiveKingItemData detail1) => detail1.RoleID == client.ClientData.RoleID);
						bool flag4 = false;
						if (!flag3 && (this.orderedGiveList.Count < this.RANK_LVL_CNT || this.orderedGiveList[this.RANK_LVL_CNT - 1].TotalGive < clientGiveKingInfo.TotalGive))
						{
							this.orderedGiveList.Add(clientGiveKingInfo);
							flag4 = true;
						}
						if (flag3 || flag4)
						{
							this.buildRankingList(this.orderedGiveList);
						}
					}
					if (client._IconStateMgr.CheckJieriGiveKing(client))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		private void buildRankingList(List<JieriGiveKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriGiveKingItemData left, JieriGiveKingItemData right)
			{
				int result;
				if (left.TotalGive > right.TotalGive)
				{
					result = -1;
				}
				else if (left.TotalGive == right.TotalGive)
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
					JieriGiveKingItemData jieriGiveKingItemData = rankingList[num];
					if (jieriGiveKingItemData.TotalGive >= awardItem.MinAwardCondionValue)
					{
						jieriGiveKingItemData.Rank = i;
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

		public void LoadRankFromDB()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				string cmd = string.Format("{0}:{1}:{2}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'), this.RANK_LVL_CNT);
				List<JieriGiveKingItemData> list = Global.sendToDB<List<JieriGiveKingItemData>, string>(13203, cmd, 0);
				lock (this._allMemberMutex)
				{
					this.giveDict.Clear();
					this.orderedGiveList.Clear();
					if (list != null && list.Count != 0)
					{
						foreach (JieriGiveKingItemData jieriGiveKingItemData in list)
						{
							this.giveDict[jieriGiveKingItemData.RoleID] = jieriGiveKingItemData;
							this.orderedGiveList.Add(jieriGiveKingItemData);
						}
						this.buildRankingList(this.orderedGiveList);
					}
				}
			}
		}

		public byte[] QueryActivityInfo(GameClient client)
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this._allMemberMutex)
				{
					return DataHelper.ObjectToBytes<JieriGiveKingData>(new JieriGiveKingData
					{
						MyData = this.GetClientGiveKingInfo(client),
						RankingList = this.orderedGiveList.GetRange(0, this.RoleCountInList)
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
						JieriGiveKingItemData clientGiveKingInfo = this.GetClientGiveKingInfo(client);
						if (clientGiveKingInfo == null || clientGiveKingInfo.TotalGive < awardItem.MinAwardCondionValue || clientGiveKingInfo.GetAwardTimes > 0 || clientGiveKingInfo.Rank != awardid)
						{
							jieriGiveErrorCode = JieriGiveErrorCode.NotMeetAwardCond;
							goto IL_1CB;
						}
						string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
						string[] array = Global.ExecuteDBCmd(13205, strcmd, client.ServerId);
						if (array == null || array.Length != 1 || Convert.ToInt32(array[0]) <= 0)
						{
							jieriGiveErrorCode = JieriGiveErrorCode.DBFailed;
							goto IL_1CB;
						}
						clientGiveKingInfo.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, awardItem) || !base.GiveAward(client, myAwardItem) || !base.GiveEffectiveTimeAward(client, awardEffectTimeItem.ToAwardItem()))
					{
						LogManager.WriteLog(2, string.Format("发送节日赠送王奖励的时候，发送失败,但是设置领奖成功，roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					jieriGiveErrorCode = JieriGiveErrorCode.Success;
				}
			}
			IL_1CB:
			if (jieriGiveErrorCode == JieriGiveErrorCode.Success)
			{
				if (client._IconStateMgr.CheckJieriGiveKing(client))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
			return string.Format("{0}:{1}", (int)jieriGiveErrorCode, awardid);
		}

		private JieriGiveKingItemData GetClientGiveKingInfo(GameClient client)
		{
			bool flag;
			return this.GetClientGiveKingInfo(client, out flag);
		}

		private JieriGiveKingItemData GetClientGiveKingInfo(GameClient client, out bool bLoadFromDb)
		{
			bLoadFromDb = false;
			JieriGiveKingItemData jieriGiveKingItemData = null;
			if (!this.giveDict.TryGetValue(client.ClientData.RoleID, out jieriGiveKingItemData))
			{
				string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				jieriGiveKingItemData = Global.sendToDB<JieriGiveKingItemData, string>(13204, cmd, client.ServerId);
				if (jieriGiveKingItemData == null)
				{
					jieriGiveKingItemData = new JieriGiveKingItemData();
					jieriGiveKingItemData.RoleID = client.ClientData.RoleID;
					jieriGiveKingItemData.Rolename = client.ClientData.RoleName;
					jieriGiveKingItemData.TotalGive = 0;
					jieriGiveKingItemData.Rank = -1;
					jieriGiveKingItemData.GetAwardTimes = 0;
				}
				else
				{
					bLoadFromDb = true;
				}
				this.giveDict[client.ClientData.RoleID] = jieriGiveKingItemData;
			}
			return jieriGiveKingItemData;
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

		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.CfgFile));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.CfgFile));
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
								LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
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
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
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
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
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
					foreach (JieriGiveKingItemData jieriGiveKingItemData in this.orderedGiveList)
					{
						if (jieriGiveKingItemData.RoleID == client.ClientData.RoleID && jieriGiveKingItemData.GetAwardTimes <= 0 && jieriGiveKingItemData.Rank > 0)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this._allMemberMutex)
				{
					JieriGiveKingItemData jieriGiveKingItemData = null;
					this.giveDict.TryGetValue(roleId, out jieriGiveKingItemData);
					if (jieriGiveKingItemData != null)
					{
						jieriGiveKingItemData.Rolename = newName;
					}
				}
			}
		}

		private readonly string CfgFile = "Config/JieRiGifts/JieRiZengSongKing.xml";

		private object _allMemberMutex = new object();

		private Dictionary<int, JieriGiveKingItemData> giveDict = new Dictionary<int, JieriGiveKingItemData>();

		private List<JieriGiveKingItemData> orderedGiveList = new List<JieriGiveKingItemData>();

		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		private int RoleCountInList;
	}
}
