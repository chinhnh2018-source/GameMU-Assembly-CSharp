using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class ThemeDuiHuanActivity : Activity
	{
		public List<int> GetIndexAll()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, ThemeDuiHuan> keyValuePair in this.ThemeDuiHuanDict)
			{
				list.Add(keyValuePair.Key);
			}
			return list;
		}

		public ThemeDuiHuan GetAwardConfig(int id)
		{
			ThemeDuiHuan result = null;
			if (this.ThemeDuiHuanDict.ContainsKey(id))
			{
				result = this.ThemeDuiHuanDict[id];
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			ThemeDuiHuan awardConfig = this.GetAwardConfig(_params);
			return null != awardConfig && base.GiveAward(client, awardConfig.MyAwardItem);
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			ThemeDuiHuan awardConfig = this.GetAwardConfig(_params);
			return null != awardConfig && null != awardConfig.MyAwardItem && (awardConfig.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, awardConfig.MyAwardItem.GoodsDataList));
		}

		public int GetThemeDHTodayLeftMergeNum(GameClient client, int index)
		{
			ThemeDuiHuan awardConfig = this.GetAwardConfig(index);
			int result;
			if (null == awardConfig)
			{
				result = 0;
			}
			else
			{
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				int num = 0;
				int num2 = 0;
				string name = (Global.SafeConvertToInt32("50") + index - 1).ToString();
				string roleParamByName = Global.GetRoleParamByName(client, name);
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					string[] array = roleParamByName.Split(new char[]
					{
						','
					});
					if (2 == array.Length)
					{
						num = Convert.ToInt32(array[0]);
						num2 = Convert.ToInt32(array[1]);
					}
				}
				if (offsetDay == num)
				{
					result = awardConfig.DayMaxTimes - num2;
				}
				else
				{
					result = awardConfig.DayMaxTimes;
				}
			}
			return result;
		}

		public int ModifyThemeTodayLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			string text = (Global.SafeConvertToInt32("50") + index - 1).ToString();
			string roleParamByName = Global.GetRoleParamByName(client, text);
			int num = 0;
			int num2 = 0;
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (2 == array.Length)
				{
					num = Convert.ToInt32(array[0]);
					num2 = Convert.ToInt32(array[1]);
				}
			}
			if (offsetDay == num)
			{
				num2 += addNum;
			}
			else
			{
				num = offsetDay;
				num2 = addNum;
			}
			string valueString = string.Format("{0},{1}", num, num2);
			Global.SaveRoleParamsStringToDB(client, text, valueString, true);
			return num2;
		}

		public string MergeGoods(GameClient client, int index)
		{
			string text = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, 154);
			string result;
			if (this.GetThemeDHTodayLeftMergeNum(client, index) <= 0)
			{
				text = string.Format("{0}:{1}:{2}", -20000, client.ClientData.RoleID, 154);
				result = text;
			}
			else
			{
				ThemeDuiHuan awardConfig = this.GetAwardConfig(index);
				if (null == awardConfig)
				{
					text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = text;
				}
				else if (null == awardConfig.MyAwardItem)
				{
					text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = text;
				}
				else if (null == awardConfig.MyAwardItem.GoodsDataList)
				{
					text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = text;
				}
				else
				{
					if (null != awardConfig.NeedGoodsList)
					{
						for (int i = 0; i < awardConfig.NeedGoodsList.Count; i++)
						{
							if (Global.GetTotalGoodsNotUsingCountByID(client, awardConfig.NeedGoodsList[i].GoodsID) < awardConfig.NeedGoodsList[i].GCount)
							{
								return string.Format("{0}:{1}:{2}", -20003, client.ClientData.RoleID, 154);
							}
						}
					}
					string text2 = "";
					if (null != awardConfig.NeedGoodsList)
					{
						for (int i = 0; i < awardConfig.NeedGoodsList.Count; i++)
						{
							bool flag = false;
							bool flag2 = false;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, awardConfig.NeedGoodsList[i].GoodsID, awardConfig.NeedGoodsList[i].GCount, false, out flag, out flag2, true))
							{
								return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 154);
							}
							text2 += EventLogManager.AddGoodsDataPropString(awardConfig.NeedGoodsList[i]);
						}
					}
					if (!this.GiveAward(client, index))
					{
						text = string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 154);
						result = text;
					}
					else
					{
						if (text2.Length > 0)
						{
							text2 = text2.Remove(0, 1);
						}
						string strResList = EventLogManager.MakeGoodsDataPropString(awardConfig.MyAwardItem.GoodsDataList);
						EventLogManager.AddPurchaseEvent(client, 8, index, text2, strResList);
						int num = Math.Max(0, awardConfig.DayMaxTimes - this.ModifyThemeTodayLeftMergeNum(client, index, 1));
						text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							client.ClientData.RoleID,
							154,
							num,
							index
						});
						result = text;
					}
				}
			}
			return result;
		}

		public bool Init()
		{
			try
			{
				string uri = "Config/ThemeActivityDuiHuan.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return false;
				}
				this.ActivityType = 154;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						ThemeDuiHuan themeDuiHuan = new ThemeDuiHuan();
						themeDuiHuan.id = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						themeDuiHuan.DayMaxTimes = (int)Global.GetSafeAttributeLong(xelement2, "DayMaxTimes");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "DuiHuanGoodsIDs");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析大型主题服兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								themeDuiHuan.NeedGoodsList = HuodongCachingMgr.ParseGoodsDataList2(array, "大型主题服兑换配置1");
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "NewGoodsID");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取大型主题服兑换活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("读取大型主题服兑换活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								themeDuiHuan.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型主题服兑换合成配置2");
							}
						}
						this.ThemeDuiHuanDict[themeDuiHuan.id] = themeDuiHuan;
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/ThemeActivityDuiHuan.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		public Dictionary<int, ThemeDuiHuan> ThemeDuiHuanDict = new Dictionary<int, ThemeDuiHuan>();
	}
}
