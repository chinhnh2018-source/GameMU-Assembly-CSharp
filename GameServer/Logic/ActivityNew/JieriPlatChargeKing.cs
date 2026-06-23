using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Client;
using Server.Tools;
using Tmsk.Contract.Data;

namespace GameServer.Logic.ActivityNew
{
	public class JieriPlatChargeKing : Activity
	{
		public List<InputKingPaiHangData> RealRankList
		{
			get
			{
				List<InputKingPaiHangData> realRankList;
				lock (this.Mutex)
				{
					realRankList = this._realRankList;
				}
				return realRankList;
			}
			private set
			{
				lock (this.Mutex)
				{
					this._realRankList = value;
				}
			}
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
							JieriPlatChargeKing.ChargeItem chargeItem = new JieriPlatChargeKing.ChargeItem();
							chargeItem.Rank = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							chargeItem.NeedChargeYB = (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao");
							this.chargeItemList.Add(chargeItem);
						}
					}
					this.chargeItemList.Sort(delegate(JieriPlatChargeKing.ChargeItem left, JieriPlatChargeKing.ChargeItem right)
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
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void Update()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				DateTime t = TimeUtil.NowDateTime();
				if (!(t < this.lastUpdateTime.AddSeconds(15.0)))
				{
					this.lastUpdateTime = t;
					InputKingPaiHangDataEx platChargeKing = KFCopyRpcClient.getInstance().GetPlatChargeKing();
					if (platChargeKing != null)
					{
						List<InputKingPaiHangData> listData = platChargeKing.ListData;
						if (platChargeKing.StartTime != this.FromDate || platChargeKing.EndTime != this.ToDate)
						{
						}
						if (listData != null)
						{
							bool flag = false;
							int i;
							for (i = 1; i < listData.Count<InputKingPaiHangData>(); i++)
							{
								if (listData[i].PaiHangValue > listData[i - 1].PaiHangValue)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								listData.Sort((InputKingPaiHangData _left, InputKingPaiHangData _right) => _right.PaiHangValue - _left.PaiHangValue);
							}
							listData.ForEach(delegate(InputKingPaiHangData _item)
							{
								_item.PaiHangValue = Global.TransMoneyToYuanBao(_item.PaiHangValue);
							});
							int num = 0;
							i = 0;
							while (i < this.chargeItemList.Count && num < listData.Count)
							{
								if (listData[num].PaiHangValue >= this.chargeItemList[i].NeedChargeYB)
								{
									listData[num].PaiHang = this.chargeItemList[i].Rank;
									num++;
								}
								i++;
							}
							if (num < listData.Count)
							{
								listData.RemoveRange(num, listData.Count - num);
							}
						}
						this.RealRankList = listData;
					}
				}
			}
			else
			{
				this.RealRankList = null;
			}
		}

		private const int updateIntervalSec = 15;

		private readonly string CfgFile = "Config/JieRiGifts/PingTaiChongZhiKing.xml";

		private List<JieriPlatChargeKing.ChargeItem> chargeItemList = new List<JieriPlatChargeKing.ChargeItem>();

		private object Mutex = new object();

		private List<InputKingPaiHangData> _realRankList = null;

		private DateTime lastUpdateTime = TimeUtil.NowDateTime().AddSeconds(-30.0);

		private class ChargeItem
		{
			public int Rank;

			public int NeedChargeYB;
		}
	}
}
