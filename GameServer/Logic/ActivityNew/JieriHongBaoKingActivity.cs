using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriHongBaoKingActivity : JieRiActivity
	{
		private int RANK_LVL_CNT
		{
			get
			{
				return this.allAwardDict.Count;
			}
		}

		public static JieriHongBaoKingActivity getInstance()
		{
			return JieriHongBaoKingActivity.instance;
		}

		public bool OnRecv(GameClient client, int zuanshi, string strFrom)
		{
			bool result;
			if (!this.InActivityTime())
			{
				LogManager.WriteLog(15, string.Format("领取红包失败#已不在活动时间内#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanshi), null, true);
				result = false;
			}
			else
			{
				int rid = client.ClientData.RoleID;
				string roleName = client.ClientData.RoleName;
				List<string> cmd = new List<string>
				{
					this.ActivityKeyStr,
					rid.ToString(),
					"1",
					TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss"),
					roleName,
					zuanshi.ToString()
				};
				JieriHongBaoKingItemData jieriHongBaoKingItemData = Global.sendToDB<JieriHongBaoKingItemData, List<string>>(1436, cmd, client.ServerId);
				if (jieriHongBaoKingItemData == null)
				{
					LogManager.WriteLog(15, string.Format("领取红包失败#红包钻石已扣减但无法记录领取者#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanshi), null, true);
				}
				lock (this.Mutex)
				{
					if (jieriHongBaoKingItemData == null)
					{
						return true;
					}
					JieriHongBaoKingItemData jieriHongBaoKingItemData2;
					if (!this.recvDict.TryGetValue(rid, out jieriHongBaoKingItemData2))
					{
						jieriHongBaoKingItemData2 = jieriHongBaoKingItemData;
						this.recvDict[rid] = jieriHongBaoKingItemData2;
					}
					else
					{
						jieriHongBaoKingItemData2.TotalRecv = jieriHongBaoKingItemData.TotalRecv;
						jieriHongBaoKingItemData2.GetAwardTimes = jieriHongBaoKingItemData.GetAwardTimes;
					}
					bool flag2 = this.orderedRecvList.Any((JieriHongBaoKingItemData x) => x.RoleID == rid);
					bool flag3 = false;
					if (!flag2 && (this.orderedRecvList.Count < this.RANK_LVL_CNT || this.orderedRecvList[this.RANK_LVL_CNT - 1].TotalRecv < jieriHongBaoKingItemData.TotalRecv))
					{
						this.orderedRecvList.Add(jieriHongBaoKingItemData2);
						flag3 = true;
					}
					if (flag2 || flag3)
					{
						this.buildRankingList(this.orderedRecvList);
					}
				}
				result = true;
			}
			return result;
		}

		public bool Init()
		{
			try
			{
				this.allAwardDict.Clear();
				this.occAwardDict.Clear();
				this.timeAwardDict.Clear();
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiHongBaoBang.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiHongBaoBang.xml"));
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Threshold"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
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
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", "Config/JieRiGifts/JieRiHongBaoBang.xml"), null, true);
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
				this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiHongBaoBang.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (GameManager.IsKuaFuServer)
			{
				result = false;
			}
			else if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					foreach (JieriHongBaoKingItemData jieriHongBaoKingItemData in this.orderedRecvList)
					{
						if (jieriHongBaoKingItemData.RoleID == client.ClientData.RoleID && jieriHongBaoKingItemData.GetAwardTimes <= 0)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public bool LoadRankFromDB()
		{
			bool result;
			if (GameManager.IsKuaFuServer)
			{
				result = false;
			}
			else
			{
				if (this.InActivityTime() || this.InAwardTime())
				{
					List<string> cmd = new List<string>
					{
						this.ActivityKeyStr,
						"20"
					};
					List<JieriHongBaoKingItemData> list = Global.sendToDB<List<JieriHongBaoKingItemData>, List<string>>(1431, cmd, 0);
					lock (this.Mutex)
					{
						this.recvDict.Clear();
						this.orderedRecvList.Clear();
						if (list == null || list.Count == 0)
						{
							return true;
						}
						foreach (JieriHongBaoKingItemData jieriHongBaoKingItemData in list)
						{
							this.recvDict[jieriHongBaoKingItemData.RoleID] = jieriHongBaoKingItemData;
							this.orderedRecvList.Add(jieriHongBaoKingItemData);
						}
						this.buildRankingList(this.orderedRecvList);
					}
				}
				result = true;
			}
			return result;
		}

		private void buildRankingList(List<JieriHongBaoKingItemData> rankingList)
		{
			rankingList.Sort(delegate(JieriHongBaoKingItemData left, JieriHongBaoKingItemData right)
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
					JieriHongBaoKingItemData jieriHongBaoKingItemData = rankingList[num];
					if (jieriHongBaoKingItemData.TotalRecv >= awardItem.MinAwardCondionValue)
					{
						jieriHongBaoKingItemData.Rank = i;
						num++;
					}
				}
				i++;
			}
			for (i = rankingList.Count - 1; i >= num; i--)
			{
				rankingList[i].Rank = -1;
				rankingList.RemoveAt(i);
			}
		}

		public void QueryActivityInfo(GameClient client)
		{
			JieriHongBaoKingData jieriHongBaoKingData = new JieriHongBaoKingData();
			jieriHongBaoKingData.DataAge = TimeUtil.NOW();
			if (this.InActivityTime() || this.InAwardTime())
			{
				lock (this.Mutex)
				{
					jieriHongBaoKingData.RankList = this.orderedRecvList;
				}
				int rid = client.ClientData.RoleID;
				string roleName = client.ClientData.RoleName;
				JieriHongBaoKingItemData jieriHongBaoKingItemData = jieriHongBaoKingData.RankList.Find((JieriHongBaoKingItemData x) => x.RoleID == rid);
				if (null != jieriHongBaoKingItemData)
				{
					jieriHongBaoKingData.SelfCount = jieriHongBaoKingItemData.TotalRecv;
				}
				else
				{
					List<string> cmd = new List<string>
					{
						this.ActivityKeyStr,
						rid.ToString()
					};
					jieriHongBaoKingData.SelfCount = Global.sendToDB<int, List<string>>(1440, cmd, client.ServerId);
				}
			}
			client.sendCmd<JieriHongBaoKingData>(1429, jieriHongBaoKingData, false);
		}

		public new void GetAward(GameClient client, int awardid)
		{
			int cmdData = 0;
			if (!this.InAwardTime())
			{
				cmdData = -2001;
			}
			else if (!this.HasEnoughBagSpaceForAwardGoods(client, awardid))
			{
				cmdData = -100;
			}
			else
			{
				AwardItem awardItem = null;
				AwardItem myAwardItem = null;
				AwardEffectTimeItem awardEffectTimeItem = null;
				if (!this.allAwardDict.TryGetValue(awardid, out awardItem) || !this.occAwardDict.TryGetValue(awardid, out myAwardItem) || !this.timeAwardDict.TryGetValue(awardid, out awardEffectTimeItem))
				{
					cmdData = -3;
				}
				else
				{
					lock (this.Mutex)
					{
						JieriHongBaoKingItemData jieriHongBaoKingItemData;
						if (!this.recvDict.TryGetValue(client.ClientData.RoleID, out jieriHongBaoKingItemData))
						{
							cmdData = -20;
							goto IL_21F;
						}
						if (jieriHongBaoKingItemData.GetAwardTimes > 0)
						{
							cmdData = -200;
							goto IL_21F;
						}
						JieriHongBaoKingItemData roleRecvKingInfo = this.GetRoleRecvKingInfo(client, 0, 0, client.ServerId);
						if (roleRecvKingInfo == null || roleRecvKingInfo.TotalRecv < awardItem.MinAwardCondionValue || roleRecvKingInfo.GetAwardTimes > 0 || roleRecvKingInfo.Rank != awardid)
						{
							cmdData = -20;
							goto IL_21F;
						}
						List<string> cmd = new List<string>
						{
							this.ActivityKeyStr,
							client.ClientData.RoleID.ToString(),
							"1"
						};
						int num = Global.sendToDB<int, List<string>>(1428, cmd, client.ServerId);
						if (num < 0)
						{
							cmdData = -15;
							goto IL_21F;
						}
						roleRecvKingInfo.GetAwardTimes = 1;
					}
					if (!base.GiveAward(client, awardItem) || !base.GiveAward(client, myAwardItem) || !base.GiveEffectiveTimeAward(client, awardEffectTimeItem.ToAwardItem()))
					{
						LogManager.WriteLog(2, string.Format("发送节日红包榜奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
					}
					client._IconStateMgr.CheckJieRiHongBaoBang(client);
				}
			}
			IL_21F:
			client.sendCmd<int>(1428, cmdData, false);
		}

		private JieriHongBaoKingItemData GetRoleRecvKingInfo(GameClient client, int count, int flags, int serverId)
		{
			int roleID = client.ClientData.RoleID;
			JieriHongBaoKingItemData result = null;
			lock (this.Mutex)
			{
				if (this.recvDict.TryGetValue(roleID, out result))
				{
					return result;
				}
			}
			return null;
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
				lock (this.Mutex)
				{
					JieriHongBaoKingItemData jieriHongBaoKingItemData = null;
					this.recvDict.TryGetValue(roleId, out jieriHongBaoKingItemData);
					if (jieriHongBaoKingItemData != null)
					{
						jieriHongBaoKingItemData.Rolename = newName;
					}
				}
			}
		}

		private const string CfgFile = "Config/JieRiGifts/JieRiHongBaoBang.xml";

		private object Mutex = new object();

		private Dictionary<int, JieriHongBaoKingItemData> recvDict = new Dictionary<int, JieriHongBaoKingItemData>();

		private List<JieriHongBaoKingItemData> orderedRecvList = new List<JieriHongBaoKingItemData>();

		public Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		private static JieriHongBaoKingActivity instance = new JieriHongBaoKingActivity();
	}
}
