using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiHongBaoActivity : JieRiActivity
	{
		public static JieRiHongBaoActivity getInstance()
		{
			return JieRiHongBaoActivity.instance;
		}

		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiQuanMinHongBao.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiQuanMinHongBao.xml"));
				if (null == xelement)
				{
					return false;
				}
				lock (this.Mutex)
				{
					this.RedPacketsQuanMinMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsQuanMinMessage");
					this.HongBaoDict.Clear();
					XElement xelement2 = xelement.Element("Activities");
					if (null != xelement2)
					{
						this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
						this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
						this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
						this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
						this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
					}
					base.PredealDateTime();
					xelement2 = xelement.Element("GiftList");
					if (null != xelement2)
					{
						IEnumerable<XElement> enumerable = xelement2.Elements();
						foreach (XElement xelement3 in enumerable)
						{
							if (null != xelement3)
							{
								RedPacketPeopleItem redPacketPeopleItem = new RedPacketPeopleItem();
								redPacketPeopleItem.ID = (int)Global.GetSafeAttributeLong(xelement3, "ID");
								redPacketPeopleItem.Day = (int)Global.GetSafeAttributeLong(xelement3, "Day");
								redPacketPeopleItem.Time = TimeSpan.Parse(Global.GetSafeAttributeStr(xelement3, "Time"));
								redPacketPeopleItem.RedPacketSize = (int)Global.GetSafeAttributeLong(xelement3, "RedPacketSize");
								redPacketPeopleItem.Interval = Global.GetSafeAttributeIntArray(xelement3, "Interval", -1, ',');
								redPacketPeopleItem.DurationTime = (int)Global.GetSafeAttributeLong(xelement3, "DurationTime");
								redPacketPeopleItem.SendTime = this.StartTime.AddDays((double)(redPacketPeopleItem.Day - 1)).Add(redPacketPeopleItem.Time);
								this.HongBaoDict.Add(redPacketPeopleItem.ID, redPacketPeopleItem);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiQuanMinHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public HongBaoListQueryData QueryHongBaoList()
		{
			try
			{
				HongBaoListQueryData cmd = new HongBaoListQueryData
				{
					KeyStr = this.ActivityKeyStr
				};
				return Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1437, cmd, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public List<HongBaoSendData> SendHongBaoProc(DateTime now, Dictionary<int, HongBaoSendData> dict)
		{
			List<HongBaoSendData> result;
			if (GameManager.IsKuaFuServer)
			{
				result = null;
			}
			else
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				lock (this.Mutex)
				{
					foreach (RedPacketPeopleItem redPacketPeopleItem in this.HongBaoDict.Values)
					{
						if (now > redPacketPeopleItem.SendTime && !this.HongBaoIdSended.Contains(redPacketPeopleItem.ID))
						{
							DateTime t = redPacketPeopleItem.SendTime.AddSeconds((double)redPacketPeopleItem.DurationTime);
							if (!(now >= t))
							{
								foreach (HongBaoSendData hongBaoSendData in dict.Values)
								{
									if (hongBaoSendData.senderID == redPacketPeopleItem.ID)
									{
										this.HongBaoIdSended.Add(redPacketPeopleItem.ID);
									}
								}
								if (!this.HongBaoIdSended.Contains(redPacketPeopleItem.ID))
								{
									HongBaoSendData hongBaoSendData2 = new HongBaoSendData();
									hongBaoSendData2.senderID = redPacketPeopleItem.ID;
									hongBaoSendData2.sender = this.ActivityKeyStr;
									hongBaoSendData2.sendTime = redPacketPeopleItem.SendTime;
									hongBaoSendData2.type = 102;
									hongBaoSendData2.endTime = redPacketPeopleItem.SendTime.AddSeconds((double)redPacketPeopleItem.DurationTime);
									hongBaoSendData2.message = this.RedPacketsQuanMinMessage;
									hongBaoSendData2.sumDiamondNum = redPacketPeopleItem.RedPacketSize;
									hongBaoSendData2.leftZuanShi = redPacketPeopleItem.RedPacketSize;
									int num = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData2, GameManager.ServerId);
									if (num > 0)
									{
										hongBaoSendData2.hongBaoID = num;
										this.HongBaoIdSended.Add(redPacketPeopleItem.ID);
										list.Add(hongBaoSendData2);
									}
								}
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public int OpenHongBao(int id)
		{
			lock (this.Mutex)
			{
				RedPacketPeopleItem redPacketPeopleItem;
				if (this.HongBaoDict.TryGetValue(id, out redPacketPeopleItem))
				{
					if (redPacketPeopleItem.Interval.Length == 2)
					{
						return Global.GetRandomNumber(redPacketPeopleItem.Interval[0], redPacketPeopleItem.Interval[1]);
					}
				}
			}
			return 0;
		}

		private const string CfgFile = "Config/JieRiGifts/JieRiQuanMinHongBao.xml";

		private object Mutex = new object();

		private string RedPacketsQuanMinMessage;

		private SortedDictionary<int, RedPacketPeopleItem> HongBaoDict = new SortedDictionary<int, RedPacketPeopleItem>();

		private HashSet<int> HongBaoIdSended = new HashSet<int>();

		private static JieRiHongBaoActivity instance = new JieRiHongBaoActivity();
	}
}
