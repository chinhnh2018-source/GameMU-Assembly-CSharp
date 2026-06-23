using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.Data;

namespace GameServer.Logic.ActivityNew
{
	public class JieriPlatChargeKingEveryDay : Activity
	{
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
							JieriPlatChargeKingEveryDay.ChargeItem chargeItem = new JieriPlatChargeKingEveryDay.ChargeItem();
							chargeItem.Id = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							chargeItem.Rank = (int)Global.GetSafeAttributeLong(xelement3, "Ranking");
							chargeItem.Day = (int)Global.GetSafeAttributeLong(xelement3, "Day");
							chargeItem.NeedChargeYB = (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao");
							AwardItem awardItem = new AwardItem();
							AwardItem awardItem2 = new AwardItem();
							AwardEffectTimeItem awardEffectTimeItem = new AwardEffectTimeItem();
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
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日活动每日平台充值王配置");
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
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日活动每日平台充值王配置2");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "GoodsThr");
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement3, "EffectiveTime");
							awardEffectTimeItem.Init(safeAttributeStr2, safeAttributeStr3, "大型节日每日平台充值王时效性物品活动配置");
							chargeItem.allAwardGoodsOne = awardItem;
							chargeItem.occAwardGoodsTwo = awardItem2;
							chargeItem.timeAwardGoodsThr = awardEffectTimeItem;
							this.IdVsChargeItemDict[chargeItem.Id] = chargeItem;
							List<JieriPlatChargeKingEveryDay.ChargeItem> list;
							if (!this.DayVsChargeItemListDict.TryGetValue(chargeItem.Day, out list))
							{
								list = new List<JieriPlatChargeKingEveryDay.ChargeItem>();
								list.Add(chargeItem);
								this.DayVsChargeItemListDict[chargeItem.Day] = list;
							}
							else
							{
								list.Add(chargeItem);
							}
							list.Sort(delegate(JieriPlatChargeKingEveryDay.ChargeItem left, JieriPlatChargeKingEveryDay.ChargeItem right)
							{
								int result;
								if (left.Rank < right.Rank)
								{
									result = -1;
								}
								else if (left.Rank > right.Rank)
								{
									result = 1;
								}
								else
								{
									result = 0;
								}
								return result;
							});
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

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			JieriPlatChargeKingEveryDay.ChargeItem chargeItem = null;
			bool result;
			if (!this.IdVsChargeItemDict.TryGetValue(_params, out chargeItem))
			{
				result = false;
			}
			else
			{
				AwardItem allAwardGoodsOne = chargeItem.allAwardGoodsOne;
				AwardItem occAwardGoodsTwo = chargeItem.occAwardGoodsTwo;
				AwardEffectTimeItem timeAwardGoodsThr = chargeItem.timeAwardGoodsThr;
				int num = 0;
				if (allAwardGoodsOne != null && allAwardGoodsOne.GoodsDataList != null)
				{
					num += allAwardGoodsOne.GoodsDataList.Count;
				}
				if (occAwardGoodsTwo != null && occAwardGoodsTwo.GoodsDataList != null)
				{
					num += occAwardGoodsTwo.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
				}
				if (timeAwardGoodsThr != null)
				{
					num += timeAwardGoodsThr.GoodsCnt();
				}
				result = Global.CanAddGoodsNum(client, num);
			}
			return result;
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
					foreach (JieriPlatChargeKingEveryDay.ChargeItem chargeItem in this.IdVsChargeItemDict.Values)
					{
						if (this.CheckCondition(client, chargeItem.Id))
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public override bool CheckCondition(GameClient client, int extTag)
		{
			bool result;
			try
			{
				JieriPlatChargeKingEveryDay.ChargeItem chargeItem = null;
				if (!this.IdVsChargeItemDict.TryGetValue(extTag, out chargeItem))
				{
					result = false;
				}
				else
				{
					DateTime dateTime = DateTime.Parse(this.FromDate);
					DateTime dateTime2 = DateTime.Parse(this.ToDate);
					DateTime t = TimeUtil.NowDateTime();
					lock (this.Mutex)
					{
						if (t < dateTime.AddDays((double)chargeItem.Day))
						{
							return false;
						}
						InputKingPaiHangDataEx inputKingPaiHangDataEx = null;
						if (this._kfRankDict == null || !this._kfRankDict.TryGetValue(chargeItem.Day, out inputKingPaiHangDataEx))
						{
							return false;
						}
						DateTime t2;
						DateTime.TryParse(inputKingPaiHangDataEx.RankTime, out t2);
						if (t2 < dateTime.AddDays((double)chargeItem.Day))
						{
							return false;
						}
						List<InputKingPaiHangData> list = null;
						if (this._realRankDict == null || !this._realRankDict.TryGetValue(chargeItem.Day, out list))
						{
							return false;
						}
						InputKingPaiHangData inputKingPaiHangData = list.Find((InputKingPaiHangData x) => 0 == string.Compare(x.UserID, client.strUserID, true));
						if (inputKingPaiHangData == null || inputKingPaiHangData.PaiHang != chargeItem.Rank)
						{
							return false;
						}
						string huoDongKeyString = Global.GetHuoDongKeyString(this.FromDate, this.ToDate);
						long num = KFCopyRpcClient.getInstance().QueryHuodongAwardUserHist(77, huoDongKeyString, client.strUserID);
						if (num < 0L)
						{
							return false;
						}
						int bitValue = Global.GetBitValue(chargeItem.Day);
						if ((num & (long)bitValue) == (long)bitValue)
						{
							return false;
						}
					}
					result = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				result = false;
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			DateTime dateTime = DateTime.Parse(this.FromDate);
			DateTime dateTime2 = DateTime.Parse(this.ToDate);
			JieriPlatChargeKingEveryDay.ChargeItem chargeItem = null;
			bool result;
			if (!this.IdVsChargeItemDict.TryGetValue(_params, out chargeItem))
			{
				result = false;
			}
			else
			{
				string huoDongKeyString = Global.GetHuoDongKeyString(this.FromDate, this.ToDate);
				int num = KFCopyRpcClient.getInstance().UpdateHuodongAwardUserHist(77, huoDongKeyString, client.strUserID, chargeItem.Day);
				if (num < 0)
				{
					result = false;
				}
				else
				{
					AwardItem allAwardGoodsOne = chargeItem.allAwardGoodsOne;
					AwardItem occAwardGoodsTwo = chargeItem.occAwardGoodsTwo;
					AwardEffectTimeItem timeAwardGoodsThr = chargeItem.timeAwardGoodsThr;
					if (!base.GiveAward(client, allAwardGoodsOne) || !base.GiveAward(client, occAwardGoodsTwo) || !base.GiveEffectiveTimeAward(client, timeAwardGoodsThr.ToAwardItem()))
					{
						string text = string.Format("发送节日每日平台充值王奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, id={3}", client.ClientData.RoleID, client.ClientData.RoleName, _params);
						LogManager.WriteLog(2, text, null, true);
						result = false;
					}
					else
					{
						if (client._IconStateMgr.CheckJieRiPCKingEveryDay(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public void HandleCenterPaiHang(int Day, List<InputKingPaiHangData> tmpRankList)
		{
			List<JieriPlatChargeKingEveryDay.ChargeItem> list;
			if (this.DayVsChargeItemListDict.TryGetValue(Day, out list))
			{
				bool flag = false;
				int i;
				for (i = 1; i < tmpRankList.Count<InputKingPaiHangData>(); i++)
				{
					if (tmpRankList[i].PaiHangValue > tmpRankList[i - 1].PaiHangValue)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					tmpRankList.Sort((InputKingPaiHangData _left, InputKingPaiHangData _right) => _right.PaiHangValue - _left.PaiHangValue);
				}
				tmpRankList.ForEach(delegate(InputKingPaiHangData _item)
				{
					_item.PaiHangValue = Global.TransMoneyToYuanBao(_item.PaiHangValue);
				});
				list.ForEach(delegate(JieriPlatChargeKingEveryDay.ChargeItem _item)
				{
					_item.allAwardGoodsOne.MinAwardCondionValue = int.MaxValue;
				});
				int num = 0;
				i = 0;
				while (i < list.Count && num < tmpRankList.Count)
				{
					if (tmpRankList[num].PaiHangValue >= list[i].NeedChargeYB)
					{
						tmpRankList[num].PaiHang = list[i].Rank;
						num++;
					}
					i++;
				}
				if (num < tmpRankList.Count)
				{
					tmpRankList.RemoveRange(num, tmpRankList.Count - num);
				}
			}
		}

		public void Update()
		{
			if (!this.InActivityTime() && !this.InAwardTime())
			{
				this._realRankDict.Clear();
				this._kfRankDict.Clear();
			}
			else
			{
				DateTime t = TimeUtil.NowDateTime();
				if (!(t < this.lastUpdateTime.AddSeconds(15.0)))
				{
					this.lastUpdateTime = t;
					DateTime fromDate = DateTime.Parse(this.FromDate);
					DateTime toDate = DateTime.Parse(this.ToDate);
					List<InputKingPaiHangDataEx> platChargeKingEveryDay = KFCopyRpcClient.getInstance().GetPlatChargeKingEveryDay(fromDate, toDate);
					if (platChargeKingEveryDay != null)
					{
						for (int i = 0; i < platChargeKingEveryDay.Count; i++)
						{
							List<InputKingPaiHangData> list = (platChargeKingEveryDay[i] != null) ? platChargeKingEveryDay[i].ListData : null;
							if (null != list)
							{
								this.HandleCenterPaiHang(i + 1, list);
								lock (this.Mutex)
								{
									this._realRankDict[i + 1] = list;
									this._kfRankDict[i + 1] = platChargeKingEveryDay[i];
								}
							}
						}
					}
				}
			}
		}

		public JieriPlatChargeKingEverydayData BuildQueryDataForClient(GameClient client)
		{
			DateTime dateTime = DateTime.Parse(this.FromDate);
			DateTime dateTime2 = DateTime.Parse(this.ToDate);
			DateTime t = TimeUtil.NowDateTime();
			JieriPlatChargeKingEverydayData jieriPlatChargeKingEverydayData = new JieriPlatChargeKingEverydayData();
			bool flag = false;
			lock (this.Mutex)
			{
				jieriPlatChargeKingEverydayData.PaiHangDict = new Dictionary<int, List<InputKingPaiHangData>>(this._realRankDict);
				foreach (KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair in jieriPlatChargeKingEverydayData.PaiHangDict)
				{
					if (!(t < dateTime.AddDays((double)keyValuePair.Key)))
					{
						flag = keyValuePair.Value.Exists((InputKingPaiHangData x) => 0 == string.Compare(x.UserID, client.strUserID, true));
						if (flag)
						{
							break;
						}
					}
				}
			}
			if (flag)
			{
				string huoDongKeyString = Global.GetHuoDongKeyString(this.FromDate, this.ToDate);
				jieriPlatChargeKingEverydayData.hasgettimes = KFCopyRpcClient.getInstance().QueryHuodongAwardUserHist(77, huoDongKeyString, client.strUserID);
			}
			return jieriPlatChargeKingEverydayData;
		}

		private const int updateIntervalSec = 15;

		private readonly string CfgFile = "Config/JieRiGifts/JieRiMeiRiChongZhiWang.xml";

		private Dictionary<int, List<JieriPlatChargeKingEveryDay.ChargeItem>> DayVsChargeItemListDict = new Dictionary<int, List<JieriPlatChargeKingEveryDay.ChargeItem>>();

		private Dictionary<int, JieriPlatChargeKingEveryDay.ChargeItem> IdVsChargeItemDict = new Dictionary<int, JieriPlatChargeKingEveryDay.ChargeItem>();

		private object Mutex = new object();

		private Dictionary<int, List<InputKingPaiHangData>> _realRankDict = new Dictionary<int, List<InputKingPaiHangData>>();

		private Dictionary<int, InputKingPaiHangDataEx> _kfRankDict = new Dictionary<int, InputKingPaiHangDataEx>();

		private DateTime lastUpdateTime = TimeUtil.NowDateTime().AddSeconds(-30.0);

		private class ChargeItem
		{
			public int Id;

			public int Rank;

			public int Day;

			public int NeedChargeYB;

			public AwardItem allAwardGoodsOne;

			public AwardItem occAwardGoodsTwo;

			public AwardEffectTimeItem timeAwardGoodsThr;
		}
	}
}
