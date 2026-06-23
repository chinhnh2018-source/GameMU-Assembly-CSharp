using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	public class SevenDayChargeAct
	{
		public void LoadConfig()
		{
			Dictionary<int, SevenDayChargeAct._DayAward> dictionary = new Dictionary<int, SevenDayChargeAct._DayAward>();
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayChongZhi.xml")).Element("GiftList");
				foreach (XElement xml in xelement.Elements())
				{
					int key = (int)Global.GetSafeAttributeLong(xml, "ID");
					SevenDayChargeAct._DayAward dayAward = new SevenDayChargeAct._DayAward();
					dayAward.NeedCharge = (int)Global.GetSafeAttributeLong(xml, "MinZhuanShi");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsOne");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("读取{0}活动配置文件中的物品配置项失败", "Config/SevenDay/SevenDayChongZhi.xml"), null, true);
						}
						else
						{
							dayAward.AllAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Config/SevenDay/SevenDayChongZhi.xml");
						}
					}
					safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsTwo");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, "Config/SevenDay/SevenDayChongZhi.xml", null, true);
						}
						else
						{
							dayAward.OccAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Config/SevenDay/SevenDayChongZhi.xml");
						}
					}
					dictionary[key] = dayAward;
				}
				lock (this.ConfigMutex)
				{
					this.DayAwardDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayChongZhi.xml"), ex, true);
			}
		}

		public ESevenDayActErrorCode HandleGetAward(GameClient client, int day)
		{
			int num;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else if (day < 0 || day > num)
			{
				result = ESevenDayActErrorCode.VisitParamsWrong;
			}
			else
			{
				SevenDayChargeAct._DayAward dayAward = null;
				lock (this.ConfigMutex)
				{
					if (this.DayAwardDict == null || !this.DayAwardDict.TryGetValue(day, out dayAward))
					{
						return ESevenDayActErrorCode.ServerConfigError;
					}
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
				if (activityData == null)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					lock (activityData)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(day, out sevenDayItemData))
						{
							result = ESevenDayActErrorCode.ServerConfigError;
						}
						else if (sevenDayItemData.Params1 * GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10) < dayAward.NeedCharge || sevenDayItemData.AwardFlag != 0)
						{
							result = ESevenDayActErrorCode.NotReachCondition;
						}
						else
						{
							int num2 = 0;
							if (dayAward.AllAward != null && dayAward.AllAward.GoodsDataList != null)
							{
								num2 += dayAward.AllAward.GoodsDataList.Count;
							}
							if (dayAward.OccAward != null && dayAward.OccAward.GoodsDataList != null)
							{
								num2 += dayAward.OccAward.GoodsDataList.Count((GoodsData _goods) => Global.IsRoleOccupationMatchGoods(client, _goods.GoodsID));
							}
							if (num2 <= 0 || !Global.CanAddGoodsNum(client, num2))
							{
								result = ESevenDayActErrorCode.NoBagSpace;
							}
							else
							{
								sevenDayItemData.AwardFlag = 1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Charge, day, sevenDayItemData, client.ServerId))
								{
									sevenDayItemData.AwardFlag = 0;
									result = ESevenDayActErrorCode.DBFailed;
								}
								else
								{
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.AllAward, ESevenDayActType.Charge) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.OccAward, ESevenDayActType.Charge))
									{
										LogManager.WriteLog(2, string.Format("玩家领取七日充值奖励，设置领奖了但是发奖失败 roleid={0}, day={1}", client.ClientData.RoleID, day), null, true);
									}
									result = ESevenDayActErrorCode.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public void Update(GameClient client)
		{
			if (client != null)
			{
				int num;
				if (SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
				{
					DateTime d = Global.GetRegTime(client.ClientData);
					d -= d.TimeOfDay;
					DateTime dateTime = d.AddDays((double)(num - 1)).AddHours(23.0).AddMinutes(59.0).AddSeconds(59.0);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(client.ClientData.RoleID);
					stringBuilder.Append(':').Append(d.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
					stringBuilder.Append(':').Append(dateTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
					Dictionary<string, int> dictionary = Global.sendToDB<Dictionary<string, int>, string>(13222, stringBuilder.ToString(), client.ServerId);
					if (dictionary != null)
					{
						Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
						lock (activityData)
						{
							for (int i = 0; i < 7; i++)
							{
								string key = d.AddDays((double)i).ToString("yyyy-MM-dd");
								int num2;
								if (dictionary.TryGetValue(key, out num2))
								{
									SevenDayItemData sevenDayItemData;
									if (!activityData.TryGetValue(i + 1, out sevenDayItemData) || sevenDayItemData.Params1 != num2)
									{
										SevenDayItemData sevenDayItemData2 = new SevenDayItemData();
										sevenDayItemData2.AwardFlag = ((sevenDayItemData != null) ? sevenDayItemData.AwardFlag : 0);
										sevenDayItemData2.Params1 = num2;
										if (SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Charge, i + 1, sevenDayItemData2, client.ServerId))
										{
											activityData[i + 1] = sevenDayItemData2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public bool HasAnyAwardCanGet(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client))
			{
				result = false;
			}
			else
			{
				Dictionary<int, SevenDayChargeAct._DayAward> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.DayAwardDict;
				}
				if (dictionary == null)
				{
					result = false;
				}
				else
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
					if (activityData == null)
					{
						result = false;
					}
					else
					{
						lock (activityData)
						{
							foreach (KeyValuePair<int, SevenDayItemData> keyValuePair in activityData)
							{
								int key = keyValuePair.Key;
								SevenDayItemData value = keyValuePair.Value;
								SevenDayChargeAct._DayAward dayAward = null;
								if (dictionary.TryGetValue(key, out dayAward) && value.Params1 * GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10) >= dayAward.NeedCharge && value.AwardFlag != 1)
								{
									return true;
								}
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		private Dictionary<int, SevenDayChargeAct._DayAward> DayAwardDict = null;

		private object ConfigMutex = new object();

		private class _DayAward
		{
			public int NeedCharge;

			public AwardItem AllAward = new AwardItem();

			public AwardItem OccAward = new AwardItem();
		}
	}
}
