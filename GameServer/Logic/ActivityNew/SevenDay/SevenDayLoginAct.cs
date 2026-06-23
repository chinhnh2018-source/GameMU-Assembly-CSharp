using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	public class SevenDayLoginAct
	{
		public void LoadConfig()
		{
			Dictionary<int, SevenDayLoginAct._DayAward> dictionary = new Dictionary<int, SevenDayLoginAct._DayAward>();
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayLogin.xml")).Element("GiftList");
				foreach (XElement xml in xelement.Elements())
				{
					int key = (int)Global.GetSafeAttributeLong(xml, "ID");
					SevenDayLoginAct._DayAward dayAward = new SevenDayLoginAct._DayAward();
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsOne");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("读取{0}活动配置文件中的物品配置项失败", "Config/SevenDay/SevenDayLogin.xml"), null, true);
						}
						else
						{
							dayAward.AllAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Config/SevenDay/SevenDayLogin.xml");
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
							LogManager.WriteLog(1, "Config/SevenDay/SevenDayLogin.xml", null, true);
						}
						else
						{
							dayAward.OccAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Config/SevenDay/SevenDayLogin.xml");
						}
					}
					string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "GoodsThr");
					string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "EffectiveTime");
					dayAward.TimeAward.Init(safeAttributeStr2, safeAttributeStr3, "Config/SevenDay/SevenDayLogin.xml 时效性物品");
					dictionary[key] = dayAward;
				}
				lock (this.ConfigMutex)
				{
					this.DayAwardDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayLogin.xml"), ex, true);
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
			else
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
				if (activityData == null)
				{
					result = ESevenDayActErrorCode.NotInActivityTime;
				}
				else if (day <= 0 || day > num)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					Dictionary<int, SevenDayLoginAct._DayAward> dictionary = null;
					lock (this.ConfigMutex)
					{
						dictionary = this.DayAwardDict;
					}
					SevenDayLoginAct._DayAward dayAward = null;
					if (dictionary == null || !dictionary.TryGetValue(day, out dayAward))
					{
						result = ESevenDayActErrorCode.ServerConfigError;
					}
					else
					{
						lock (activityData)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(day, out sevenDayItemData))
							{
								result = ESevenDayActErrorCode.NotReachCondition;
							}
							else if (sevenDayItemData.Params1 != 1 || sevenDayItemData.AwardFlag == 1)
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
								if (dayAward.TimeAward != null)
								{
									num2 += dayAward.TimeAward.GoodsCnt();
								}
								if (num2 <= 0 || !Global.CanAddGoodsNum(client, num2))
								{
									result = ESevenDayActErrorCode.NoBagSpace;
								}
								else
								{
									sevenDayItemData.AwardFlag = 1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Login, day, sevenDayItemData, client.ServerId))
									{
										sevenDayItemData.AwardFlag = 0;
										result = ESevenDayActErrorCode.DBFailed;
									}
									else
									{
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.AllAward, ESevenDayActType.Login) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.OccAward, ESevenDayActType.Login) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveEffectiveTimeAward(client, dayAward.TimeAward.ToAwardItem(), ESevenDayActType.Login))
										{
											LogManager.WriteLog(2, string.Format("玩家领取七日活动奖励，设置领奖了但是发奖失败 roleid={0}, day={1}", client.ClientData.RoleID, day), null, true);
										}
										result = ESevenDayActErrorCode.Success;
									}
								}
							}
						}
					}
				}
			}
			return result;
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
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
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
							SevenDayItemData value = keyValuePair.Value;
							if (value.Params1 == 1 && value.AwardFlag != 1)
							{
								return true;
							}
						}
					}
					result = false;
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
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
					lock (activityData)
					{
						if (!activityData.ContainsKey(num))
						{
							SevenDayItemData sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.AwardFlag = 0;
							sevenDayItemData.Params1 = 1;
							if (SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Login, num, sevenDayItemData, client.ServerId))
							{
								activityData[num] = sevenDayItemData;
							}
						}
					}
				}
			}
		}

		private Dictionary<int, SevenDayLoginAct._DayAward> DayAwardDict = null;

		private object ConfigMutex = new object();

		private class _DayAward
		{
			public AwardItem AllAward = new AwardItem();

			public AwardItem OccAward = new AwardItem();

			public AwardEffectTimeItem TimeAward = new AwardEffectTimeItem();
		}
	}
}
