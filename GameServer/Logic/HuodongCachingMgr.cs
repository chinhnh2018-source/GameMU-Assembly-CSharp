using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class HuodongCachingMgr
	{
		public static List<GoodsData> ParseGoodsDataList(string[] fields, string fileName)
		{
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] array = fields[i].Split(new char[]
				{
					','
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] array2 = Global.StringArray2IntArray(array);
					GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
					list.Add(newGoodsData);
				}
			}
			return list;
		}

		public static List<AwardEffectTimeItem.TimeDetail> ParseGoodsTimeList(string[] fields, string fileName)
		{
			List<AwardEffectTimeItem.TimeDetail> result;
			if (fields == null)
			{
				result = null;
			}
			else
			{
				List<AwardEffectTimeItem.TimeDetail> list = new List<AwardEffectTimeItem.TimeDetail>();
				foreach (string text in fields)
				{
					AwardEffectTimeItem.TimeDetail timeDetail = new AwardEffectTimeItem.TimeDetail();
					string[] array = text.Split(new char[]
					{
						','
					});
					int num = Convert.ToInt32(array[0]);
					bool flag = false;
					if (num == 1)
					{
						if (array.Length == 2)
						{
							timeDetail.Type = AwardEffectTimeItem.EffectTimeType.ETT_LastMinutesFromNow;
							timeDetail.LastMinutes = Convert.ToInt32(array[1]);
							flag = true;
						}
					}
					else if (num == 2)
					{
						if (array.Length == 3)
						{
							timeDetail.Type = AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime;
							timeDetail.AbsoluteStartTime = array[1];
							timeDetail.AbsoluteEndTime = array[2];
							flag = true;
						}
					}
					if (!flag)
					{
						timeDetail.Type = AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime;
						timeDetail.AbsoluteStartTime = "1900-01-01 12:00:00";
						timeDetail.AbsoluteEndTime = "1900-01-01 12:00:00";
					}
					list.Add(timeDetail);
				}
				result = list;
			}
			return result;
		}

		public static List<GoodsData> ParseGoodsDataList2(string[] fields, string fileName)
		{
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] array = fields[i].Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] array2 = Global.StringArray2IntArray(array);
					GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], 0, 0, 0, 0, 0, 0, 0, 0, 0);
					list.Add(newGoodsData);
				}
			}
			return list;
		}

		private static string ParseDateTime(string str)
		{
			int num = str.IndexOf('月');
			string result;
			if (-1 == num)
			{
				result = "";
			}
			else
			{
				int num2 = str.IndexOf('日');
				if (-1 == num2)
				{
					result = "";
				}
				else
				{
					int num3 = str.IndexOf('时');
					if (-1 == num3)
					{
						result = "";
					}
					else
					{
						int num4 = str.IndexOf('分');
						if (-1 == num4)
						{
							result = "";
						}
						else
						{
							string text = str.Substring(0, num);
							if (string.IsNullOrEmpty(text))
							{
								result = "";
							}
							else
							{
								string text2 = str.Substring(num + 1, num2 - num - 1);
								if (string.IsNullOrEmpty(text2))
								{
									result = "";
								}
								else
								{
									string text3 = str.Substring(num2 + 1, num3 - num2 - 1);
									if (string.IsNullOrEmpty(text3))
									{
										result = "";
									}
									else
									{
										string text4 = str.Substring(num3 + 1, num4 - num3 - 1);
										if (string.IsNullOrEmpty(text4))
										{
											result = "";
										}
										else
										{
											int year = TimeUtil.NowDateTime().Year;
											result = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", new object[]
											{
												year,
												text,
												text2,
												text3,
												text4,
												0
											});
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static long GetHuoDongDateTime(string str)
		{
			string str2 = HuodongCachingMgr.ParseDateTime(str);
			return Global.SafeConvertToTicks(str2);
		}

		public static long GetHuoDongDateTimeForCommonTimeString(string str)
		{
			return Global.SafeConvertToTicks(str);
		}

		private static int GetBitValue(int whichOne)
		{
			int result = 0;
			if (1 == whichOne)
			{
				result = 1;
			}
			else if (2 == whichOne)
			{
				result = 2;
			}
			else if (3 == whichOne)
			{
				result = 4;
			}
			else if (4 == whichOne)
			{
				result = 8;
			}
			else if (5 == whichOne)
			{
				result = 16;
			}
			else if (6 == whichOne)
			{
				result = 32;
			}
			else if (7 == whichOne)
			{
				result = 64;
			}
			return result;
		}

		public static bool GiveAward(GameClient client, AwardItem myAwardItem, string goodsFromWere)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int goodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, goodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", true, 1, goodsFromWere, "1900-01-01 12:00:00", myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
					client.ClientData.AddAwardRecord(RoleAwardMsg.CombatGift, myAwardItem.GoodsDataList, false);
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, goodsFromWere, ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, goodsFromWere), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		protected static bool GiveEffectiveTimeAward(GameClient client, AwardItem myAwardItem, string goodsFromWhere)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int goodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, goodsID))
						{
							Global.AddEffectiveTimeGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", false, 1, goodsFromWhere, myAwardItem.GoodsDataList[i].Starttime, myAwardItem.GoodsDataList[i].Endtime, myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, goodsFromWhere, ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, goodsFromWhere), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		private static WLoginItem GetWLoginItem(int whichOne)
		{
			WLoginItem wloginItem = null;
			lock (HuodongCachingMgr._WLoginDict)
			{
				if (HuodongCachingMgr._WLoginDict.TryGetValue(whichOne, out wloginItem))
				{
					return wloginItem;
				}
			}
			SystemXmlItem systemXmlItem = null;
			WLoginItem result;
			if (!GameManager.systemWeekLoginGiftMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位周连续登录配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("TimeOl", -1);
				wloginItem = new WLoginItem
				{
					TimeOl = intValue,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._WLoginDict)
				{
					HuodongCachingMgr._WLoginDict[whichOne] = wloginItem;
				}
				string stringValue = systemXmlItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(stringValue))
				{
					LogManager.WriteLog(1, string.Format("根据奖励类型定位周连续登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
					result = wloginItem;
				}
				else
				{
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("根据奖励类型定位周连续登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
						result = wloginItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "周连续登录配置");
						wloginItem.GoodsDataList = goodsDataList;
						result = wloginItem;
					}
				}
			}
			return result;
		}

		public static int ResetWLoginItem()
		{
			int result = GameManager.systemWeekLoginGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._WLoginDict)
			{
				HuodongCachingMgr._WLoginDict.Clear();
			}
			return result;
		}

		public static int ProcessGetWLoginGift(GameClient client, int whichOne)
		{
			WLoginItem wloginItem = HuodongCachingMgr.GetWLoginItem(whichOne);
			int result;
			if (null == wloginItem)
			{
				result = -1;
			}
			else if (wloginItem.GoodsDataList == null || wloginItem.GoodsDataList.Count <= 0)
			{
				result = -5;
			}
			else if (client.ClientData.MyHuodongData.LoginNum < wloginItem.TimeOl)
			{
				result = -10;
			}
			else
			{
				int bitValue = HuodongCachingMgr.GetBitValue(whichOne);
				if ((client.ClientData.MyHuodongData.LoginGiftState & bitValue) == bitValue)
				{
					result = -100;
				}
				else if (!Global.CanAddGoodsDataList(client, wloginItem.GoodsDataList))
				{
					result = -200;
				}
				else
				{
					for (int i = 0; i < wloginItem.GoodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, wloginItem.GoodsDataList[i].GoodsID, wloginItem.GoodsDataList[i].GCount, wloginItem.GoodsDataList[i].Quality, "", wloginItem.GoodsDataList[i].Forge_level, wloginItem.GoodsDataList[i].Binding, 0, "", true, 1, "周连续登录奖励", "1900-01-01 12:00:00", wloginItem.GoodsDataList[i].AddPropIndex, wloginItem.GoodsDataList[i].BornIndex, wloginItem.GoodsDataList[i].Lucky, wloginItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
					}
					client.ClientData.MyHuodongData.LoginGiftState = (client.ClientData.MyHuodongData.LoginGiftState | bitValue);
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		private static MOnlineTimeItem GetMOnlineTimeItem(int whichOne)
		{
			MOnlineTimeItem monlineTimeItem = null;
			lock (HuodongCachingMgr._MonthTimeDict)
			{
				if (HuodongCachingMgr._MonthTimeDict.TryGetValue(whichOne, out monlineTimeItem))
				{
					return monlineTimeItem;
				}
			}
			SystemXmlItem systemXmlItem = null;
			MOnlineTimeItem result;
			if (!GameManager.systemMOnlineTimeGiftMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位月在线时长配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int timeOl = Global.GMax(systemXmlItem.GetIntValue("TimeOl", -1), 0) * 3600;
				int bindYuanBao = Global.GMax(systemXmlItem.GetIntValue("BindYuanBao", -1), 0);
				monlineTimeItem = new MOnlineTimeItem
				{
					TimeOl = timeOl,
					BindYuanBao = bindYuanBao
				};
				lock (HuodongCachingMgr._MonthTimeDict)
				{
					HuodongCachingMgr._MonthTimeDict[whichOne] = monlineTimeItem;
				}
				result = monlineTimeItem;
			}
			return result;
		}

		public static int ResetMOnlineTimeItem()
		{
			int result = GameManager.systemMOnlineTimeGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._MonthTimeDict)
			{
				HuodongCachingMgr._MonthTimeDict.Clear();
			}
			return result;
		}

		public static int ProcessGetMOnlineTimeGift(GameClient client, int whichOne)
		{
			MOnlineTimeItem monlineTimeItem = HuodongCachingMgr.GetMOnlineTimeItem(whichOne);
			int result;
			if (null == monlineTimeItem)
			{
				result = -1;
			}
			else if (client.ClientData.MyHuodongData.CurMTime < monlineTimeItem.TimeOl)
			{
				result = -10;
			}
			else
			{
				int bitValue = HuodongCachingMgr.GetBitValue(whichOne);
				if ((client.ClientData.MyHuodongData.OnlineGiftState & bitValue) == bitValue)
				{
					result = -100;
				}
				else
				{
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, monlineTimeItem.BindYuanBao, "月在线时长礼物");
					client.ClientData.MyHuodongData.OnlineGiftState = (client.ClientData.MyHuodongData.OnlineGiftState | bitValue);
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		private static NewStepItem GetNewStepItem(int step)
		{
			NewStepItem newStepItem = null;
			lock (HuodongCachingMgr._NewStepDict)
			{
				if (HuodongCachingMgr._NewStepDict.TryGetValue(step, out newStepItem))
				{
					return newStepItem;
				}
			}
			SystemXmlItem systemXmlItem = null;
			NewStepItem result;
			if (!GameManager.systemNewRoleGiftMgr.SystemXmlItemDict.TryGetValue(step, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
				result = null;
			}
			else
			{
				int timeSecs = Global.GMax(systemXmlItem.GetIntValue("TimeSecs", -1), 0) * 60;
				newStepItem = new NewStepItem
				{
					TimeSecs = timeSecs,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._NewStepDict)
				{
					HuodongCachingMgr._NewStepDict[step] = newStepItem;
				}
				string stringValue = systemXmlItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(stringValue))
				{
					LogManager.WriteLog(1, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
					result = newStepItem;
				}
				else
				{
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
						result = newStepItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "见面有礼配置项");
						newStepItem.GoodsDataList = goodsDataList;
						newStepItem.BindMoney = systemXmlItem.GetIntValue("BindMoney", -1);
						newStepItem.BindYuanBao = systemXmlItem.GetIntValue("BindYuanBao", -1);
						result = newStepItem;
					}
				}
			}
			return result;
		}

		public static int ResetNewStepItem()
		{
			int result = GameManager.systemNewRoleGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._NewStepDict)
			{
				HuodongCachingMgr._NewStepDict.Clear();
			}
			return result;
		}

		public static int ProcessGetNewStepGift(GameClient client, int step)
		{
			NewStepItem newStepItem = HuodongCachingMgr.GetNewStepItem(step + 1);
			int result;
			if (null == newStepItem)
			{
				result = -1;
			}
			else if (newStepItem.GoodsDataList == null || newStepItem.GoodsDataList.Count <= 0)
			{
				result = -5;
			}
			else if (client.ClientData.MyHuodongData.NewStep != step)
			{
				result = -10;
			}
			else
			{
				long num = TimeUtil.NOW();
				if (num - client.ClientData.MyHuodongData.StepTime < (long)(newStepItem.TimeSecs * 1000))
				{
					int num2 = newStepItem.TimeSecs - (int)((num - client.ClientData.MyHuodongData.StepTime) / 1000L);
					result = -(10000 + num2);
				}
				else if (!Global.CanAddGoodsDataList(client, newStepItem.GoodsDataList))
				{
					result = -200;
				}
				else
				{
					for (int i = 0; i < newStepItem.GoodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, newStepItem.GoodsDataList[i].GoodsID, newStepItem.GoodsDataList[i].GCount, newStepItem.GoodsDataList[i].Quality, "", newStepItem.GoodsDataList[i].Forge_level, newStepItem.GoodsDataList[i].Binding, 0, "", true, 1, "新手见面奖品", "1900-01-01 12:00:00", newStepItem.GoodsDataList[i].AddPropIndex, newStepItem.GoodsDataList[i].BornIndex, newStepItem.GoodsDataList[i].Lucky, newStepItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
					}
					int bindMoney = newStepItem.BindMoney;
					if (bindMoney > 0)
					{
						GameManager.ClientMgr.NotifyAddJinBiMsg(client, bindMoney);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindMoney, "新手见面礼物", false);
						GameManager.SystemServerEvents.AddEvent(string.Format("从新手见面奖品领取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.Money1,
							bindMoney
						}), EventLevels.Record);
					}
					int bindYuanBao = newStepItem.BindYuanBao;
					if (bindYuanBao > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindYuanBao, "新手见面礼物");
						GameManager.SystemServerEvents.AddEvent(string.Format("从新手见面奖品领取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.UserMoney,
							bindYuanBao
						}), EventLevels.Record);
					}
					client.ClientData.MyHuodongData.NewStep++;
					client.ClientData.MyHuodongData.StepTime = num;
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		public static int CombatGiftMaxVal
		{
			get
			{
				return HuodongCachingMgr._CombatAwardlDict.Count;
			}
		}

		private static void InitCombatAwardDict()
		{
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				if (HuodongCachingMgr._CombatAwardlDict.Count == 0)
				{
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.systemCombatAwardMgr.SystemXmlItemDict)
					{
						SystemXmlItem value = keyValuePair.Value;
						CombatAwardItem combatAwardItem = new CombatAwardItem
						{
							ID = value.GetIntValue("ID", -1),
							ComBatValue = value.GetIntValue("ComatEffectiveness", -1)
						};
						string stringValue = value.GetStringValue("GoodsOne");
						if (!string.IsNullOrEmpty(stringValue))
						{
							string[] array = stringValue.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								combatAwardItem.GeneralAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, string.Format("战斗力奖励配置项 GoodsOne", new object[0]));
							}
						}
						string stringValue2 = value.GetStringValue("GoodsTwo");
						if (!string.IsNullOrEmpty(stringValue2))
						{
							string[] array = stringValue2.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								combatAwardItem.OccAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, string.Format("战斗力奖励配置项 GoodsTwo", new object[0]));
							}
						}
						string stringValue3 = value.GetStringValue("GoodsThr");
						string stringValue4 = value.GetStringValue("EffectiveTime");
						combatAwardItem.EffectTimeAwardItem.Init(stringValue3, stringValue4, string.Format("战斗力奖励配置项 GoodsThr 和 EffectiveTime", new object[0]));
						HuodongCachingMgr._CombatAwardlDict[combatAwardItem.ID] = combatAwardItem;
					}
				}
			}
		}

		public static int GetNextCombatGiftNeedVal(GameClient client)
		{
			HuodongCachingMgr.InitCombatAwardDict();
			long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10154");
			for (int i = 0; i < HuodongCachingMgr.CombatGiftMaxVal; i++)
			{
				if (Global.GetLongSomeBit(roleParamsInt64FromDB, i * 2) == 0L)
				{
					CombatAwardItem combatAwardItem = HuodongCachingMgr.GetCombatAwardItem(client, i + 1);
					if (combatAwardItem != null)
					{
						return combatAwardItem.ComBatValue;
					}
				}
			}
			return -1;
		}

		public static int GiveCombatGift(GameClient client, CombatAwardItem combatAwardItem)
		{
			int result;
			if (combatAwardItem == null)
			{
				result = -101;
			}
			else
			{
				long num = Global.GetRoleParamsInt64FromDB(client, "10154");
				if (Global.GetLongSomeBit(num, (combatAwardItem.ID - 1) * 2) == 0L)
				{
					result = -101;
				}
				else if (1L == Global.GetLongSomeBit(num, (combatAwardItem.ID - 1) * 2 + 1))
				{
					result = -103;
				}
				else
				{
					int num2 = combatAwardItem.TotalAwardCnt(client);
					if (num2 > 0 && Global.CanAddGoodsNum(client, num2))
					{
						if (!HuodongCachingMgr.GiveAward(client, combatAwardItem.GeneralAwardItem, "战力礼包") || !HuodongCachingMgr.GiveAward(client, combatAwardItem.OccAwardItem, "战力礼包") || !HuodongCachingMgr.GiveEffectiveTimeAward(client, combatAwardItem.EffectTimeAwardItem.ToAwardItem(), "战力礼包"))
						{
							LogManager.WriteLog(2, string.Format("发送战力礼包奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, combatAwardItem.ID), null, true);
						}
						GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.CombatGift, "");
						num = Global.SetLongSomeBit((combatAwardItem.ID - 1) * 2 + 1, num, true);
						Global.SaveRoleParamsInt64ValueToDB(client, "10154", num, true);
						result = 1;
					}
					else
					{
						result = -20;
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessQueryCombatGiftFlagList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10154");
				int combatGiftMaxVal = HuodongCachingMgr.CombatGiftMaxVal;
				string text2 = null;
				for (int i = 0; i < combatGiftMaxVal; i++)
				{
					int num2;
					if (Global.GetLongSomeBit(roleParamsInt64FromDB, i * 2) == 0L)
					{
						num2 = 0;
					}
					else if (Global.GetLongSomeBit(roleParamsInt64FromDB, i * 2 + 1) == 0L)
					{
						num2 = 1;
					}
					else
					{
						num2 = 2;
					}
					text2 = text2 + num2 + "_";
				}
				if (text2 != null)
				{
					text2 = text2.Substring(0, text2.Length - 1);
				}
				else
				{
					text2 = "";
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessGetCombatGiftAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num3 = -101;
				CombatAwardItem combatAwardItem = HuodongCachingMgr.GetCombatAwardItem(gameClient, num2);
				if (null != combatAwardItem)
				{
					num3 = HuodongCachingMgr.GiveCombatGift(gameClient, combatAwardItem);
					gameClient._IconStateMgr.CheckCombatGift(gameClient);
				}
				text = string.Format("{0}:{1}", num3, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static CombatAwardItem GetCombatAwardItem(GameClient client, int awardIndex)
		{
			HuodongCachingMgr.InitCombatAwardDict();
			CombatAwardItem result = null;
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				HuodongCachingMgr._CombatAwardlDict.TryGetValue(awardIndex, out result);
			}
			return result;
		}

		public static int ResetCombatAwardItem()
		{
			int result = GameManager.systemCombatAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				HuodongCachingMgr._CombatAwardlDict.Clear();
			}
			HuodongCachingMgr.InitCombatAwardDict();
			return result;
		}

		public static int ProcessCombatGift(GameClient client, bool give = false)
		{
			int result;
			if (client.ClientData.NextCombatForceGiftVal <= 0)
			{
				result = -1;
			}
			else if (client.ClientData.CombatForce < client.ClientData.NextCombatForceGiftVal)
			{
				result = -1;
			}
			else
			{
				int num = 0;
				lock (HuodongCachingMgr._CombatAwardlDict)
				{
					IEnumerable<KeyValuePair<int, CombatAwardItem>> source = from items in HuodongCachingMgr._CombatAwardlDict
					where items.Value.ComBatValue <= client.ClientData.CombatForce
					select items;
					if (source.Any<KeyValuePair<int, CombatAwardItem>>())
					{
						num = source.Max((KeyValuePair<int, CombatAwardItem> _b) => _b.Value.ID);
					}
				}
				long num2 = Global.GetRoleParamsInt64FromDB(client, "10154");
				for (int i = 0; i < num; i++)
				{
					if (Global.GetLongSomeBit(num2, i * 2) != 1L)
					{
						num2 = Global.SetLongSomeBit(i * 2, num2, true);
					}
				}
				Global.SaveRoleParamsInt64ValueToDB(client, "10154", num2, true);
				client._IconStateMgr.CheckCombatGift(client);
				client.ClientData.NextCombatForceGiftVal = HuodongCachingMgr.GetNextCombatGiftNeedVal(client);
				result = 0;
			}
			return result;
		}

		private static void InitUpLevelDict()
		{
			lock (HuodongCachingMgr._UpLevelDict)
			{
				if (HuodongCachingMgr._UpLevelDict.Count == 0)
				{
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.systemUpLevelGiftMgr.SystemXmlItemDict)
					{
						SystemXmlItem value = keyValuePair.Value;
						UpLevelItem upLevelItem = new UpLevelItem
						{
							ID = value.GetIntValue("ID", -1),
							ToLevel = Global.GetUnionLevel(value.GetIntValue("ToZhuanSheng", -1), value.GetIntValue("ToLevel", -1), false),
							GoodsDataList = null,
							BindMoney = value.GetIntValue("BindMoney", -1),
							MoJing = value.GetIntValue("MoJing", -1),
							Occupation = value.GetIntValue("Occupation", -1)
						};
						Dictionary<int, UpLevelItem> dictionary;
						if (!HuodongCachingMgr._UpLevelDict.TryGetValue(upLevelItem.Occupation, out dictionary))
						{
							dictionary = new Dictionary<int, UpLevelItem>();
							HuodongCachingMgr._UpLevelDict.Add(upLevelItem.Occupation, dictionary);
						}
						dictionary.Add(upLevelItem.ToLevel, upLevelItem);
						string stringValue = value.GetStringValue("GoodsIDs");
						if (!string.IsNullOrEmpty(stringValue))
						{
							string[] array = stringValue.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "升级有礼配置项");
								upLevelItem.GoodsDataList = goodsDataList;
							}
						}
					}
				}
			}
		}

		private static UpLevelItem GetUpLevelItem(int occu, int unionlevel)
		{
			HuodongCachingMgr.InitUpLevelDict();
			UpLevelItem result2;
			lock (HuodongCachingMgr._UpLevelDict)
			{
				UpLevelItem result = null;
				Dictionary<int, UpLevelItem> dictionary;
				if (HuodongCachingMgr._UpLevelDict.TryGetValue(occu, out dictionary))
				{
					if (dictionary.TryGetValue(unionlevel, out result))
					{
						return result;
					}
				}
				result2 = null;
			}
			return result2;
		}

		private static UpLevelItem GetUpLevelItemByID(int occu, int id)
		{
			HuodongCachingMgr.InitUpLevelDict();
			lock (HuodongCachingMgr._UpLevelDict)
			{
				Dictionary<int, UpLevelItem> dictionary;
				if (HuodongCachingMgr._UpLevelDict.TryGetValue(occu, out dictionary))
				{
					foreach (KeyValuePair<int, UpLevelItem> keyValuePair in dictionary)
					{
						if (keyValuePair.Value.ID == id)
						{
							return keyValuePair.Value;
						}
					}
				}
			}
			return null;
		}

		public static int ResetUpLevelItem()
		{
			int result = GameManager.systemUpLevelGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._UpLevelDict)
			{
				HuodongCachingMgr._UpLevelDict.Clear();
			}
			return result;
		}

		public static int GiveUpLevelGift(GameClient client, UpLevelItem newStepItem)
		{
			int unionLevel = Global.GetUnionLevel(client, false);
			int result;
			if (newStepItem.ToLevel > unionLevel)
			{
				result = -101;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
				for (int i = 0; i < 6; i++)
				{
					UpLevelItem upLevelItem = HuodongCachingMgr.GetUpLevelItem(i, newStepItem.ToLevel);
					if (upLevelItem != null && 1 == Global.GetBitValue(roleParamsIntListFromDB, upLevelItem.ID * 2 + 1))
					{
						return -103;
					}
				}
				if (newStepItem.GoodsDataList != null && newStepItem.GoodsDataList.Count > 0)
				{
					if (!Global.CanAddGoodsDataList(client, newStepItem.GoodsDataList))
					{
						return -20;
					}
					for (int j = 0; j < newStepItem.GoodsDataList.Count; j++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, newStepItem.GoodsDataList[j].GoodsID, newStepItem.GoodsDataList[j].GCount, newStepItem.GoodsDataList[j].Quality, "", newStepItem.GoodsDataList[j].Forge_level, newStepItem.GoodsDataList[j].Binding, 0, "", true, 1, "升级有礼奖品", "1900-01-01 12:00:00", newStepItem.GoodsDataList[j].AddPropIndex, newStepItem.GoodsDataList[j].BornIndex, newStepItem.GoodsDataList[j].Lucky, newStepItem.GoodsDataList[j].Strong, newStepItem.GoodsDataList[j].ExcellenceInfo, newStepItem.GoodsDataList[j].AppendPropLev, 0, null, null, 0, true);
					}
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, newStepItem.GoodsDataList, false);
				}
				for (int i = 0; i < 6; i++)
				{
					UpLevelItem upLevelItem = HuodongCachingMgr.GetUpLevelItem(i, newStepItem.ToLevel);
					if (null != upLevelItem)
					{
						Global.SetBitValue(ref roleParamsIntListFromDB, upLevelItem.ID * 2 + 1, 1);
					}
				}
				Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "UpLevelGiftFlags", true);
				int bindMoney = newStepItem.BindMoney;
				if (bindMoney > 0)
				{
					GameManager.ClientMgr.NotifyAddJinBiMsg(client, bindMoney);
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindMoney, "升级有礼", false);
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.Money1,
						bindMoney
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.TongQian, bindMoney);
				}
				int bindYuanBao = newStepItem.BindYuanBao;
				if (bindYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindYuanBao, "升级有礼");
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.UserMoney,
						bindYuanBao
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.BindYuanBao, bindYuanBao);
				}
				int moJing = newStepItem.MoJing;
				if (moJing > 0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, moJing, "升级有礼", false, true, false);
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取魔晶, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						GameManager.ClientMgr.GetTianDiJingYuanValue(client),
						moJing
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.JingYuanZhi, moJing);
				}
				GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.DengJiLiBao, "");
				GameManager.ClientMgr.NotifyGetLevelUpGiftData(client, unionLevel);
				result = 1;
			}
			return result;
		}

		public static int ProcessGetUpLevelGift(GameClient client, bool give = false)
		{
			int result;
			if (client.ClientData.MapCode == 6090)
			{
				result = -1;
			}
			else
			{
				int unionLevel = Global.GetUnionLevel(client, false);
				UpLevelItem upLevelItem = HuodongCachingMgr.GetUpLevelItem(Global.CalcOriginalOccupationID(client), unionLevel);
				if (null == upLevelItem)
				{
					result = -1;
				}
				else if (upLevelItem.Occupation != Global.CalcOriginalOccupationID(client))
				{
					result = -1;
				}
				else
				{
					List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
					if (Global.GetBitValue(roleParamsIntListFromDB, upLevelItem.ID * 2) == 0)
					{
						Global.SetBitValue(ref roleParamsIntListFromDB, upLevelItem.ID * 2, 1);
						Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "UpLevelGiftFlags", true);
					}
					if (give && 0 == Global.GetBitValue(roleParamsIntListFromDB, upLevelItem.ID * 2 + 1))
					{
						result = HuodongCachingMgr.GiveUpLevelGift(client, upLevelItem);
					}
					else
					{
						client._IconStateMgr.CheckFuUpLevelGift(client);
						client._IconStateMgr.CheckSpecialActivity(client);
						client._IconStateMgr.CheckEverydayActivity(client);
						result = 0;
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessQueryUpLevelGiftFlagList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<int>>(roleParamsIntListFromDB, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessGetUpLevelGiftAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num3 = -101;
				UpLevelItem upLevelItemByID = HuodongCachingMgr.GetUpLevelItemByID(Global.CalcOriginalOccupationID(gameClient), num2);
				if (null != upLevelItemByID)
				{
					num3 = HuodongCachingMgr.GiveUpLevelGift(gameClient, upLevelItemByID);
					gameClient._IconStateMgr.CheckFuUpLevelGift(gameClient);
				}
				text = string.Format("{0}:{1}", num3, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static BigAwardItem GetBigAwardItem()
		{
			lock (HuodongCachingMgr._BigAwardItemMutex)
			{
				if (HuodongCachingMgr._BigAwardItem != null)
				{
					return HuodongCachingMgr._BigAwardItem;
				}
			}
			try
			{
				string uri = "Config/Gifts/BigGift.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				BigAwardItem bigAwardItem = new BigAwardItem();
				XElement xelement2 = xelement.Element("GiftTime");
				if (null != xelement2)
				{
					string text = Global.GetSafeAttributeStr(xelement2, "FromDate");
					string text2 = Global.GetSafeAttributeStr(xelement2, "ToDate");
					if (text.Trim().CompareTo("-1") == 0 && 0 == text2.Trim().CompareTo("-1"))
					{
						text = "2012-06-06 16:16:16";
						text2 = "2032-06-06 16:16:16";
					}
					bigAwardItem.StartTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(text);
					bigAwardItem.EndTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(text2);
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							int key = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							bigAwardItem.NeedJiFenDict[key] = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "NeedJiFen"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大奖活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取大奖活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									bigAwardItem.GoodsDataListDict[key] = HuodongCachingMgr.ParseGoodsDataList(array, "大奖活动配置");
								}
							}
						}
					}
				}
				lock (HuodongCachingMgr._BigAwardItemMutex)
				{
					HuodongCachingMgr._BigAwardItem = bigAwardItem;
				}
				return bigAwardItem;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/BigGift.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetBigAwardItem()
		{
			string uri = "Config/Gifts/BigGift.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._BigAwardItemMutex)
			{
				HuodongCachingMgr._BigAwardItem = null;
			}
			return 0;
		}

		public static int ProcessGetBigAwardGift(GameClient client, int bigAwardID, int whichOne)
		{
			BigAwardItem bigAwardItem = HuodongCachingMgr.GetBigAwardItem();
			int result;
			if (null == bigAwardItem)
			{
				result = -1;
			}
			else if (bigAwardID != GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0) || GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0) <= 0)
			{
				result = -5;
			}
			else
			{
				long num = TimeUtil.NOW();
				if (num < bigAwardItem.StartTicks || num >= bigAwardItem.EndTicks)
				{
					result = -10;
				}
				else
				{
					int num2 = 0;
					if (!bigAwardItem.NeedJiFenDict.TryGetValue(whichOne, out num2))
					{
						result = -30;
					}
					else
					{
						List<GoodsData> list = null;
						if (!bigAwardItem.GoodsDataListDict.TryGetValue(whichOne, out list))
						{
							result = -50;
						}
						else if (!Global.CanAddGoodsDataList(client, list))
						{
							result = -300;
						}
						else
						{
							int num3 = 0;
							if (num2 > 0)
							{
								string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, num2);
								string[] array = Global.ExecuteDBCmd(10046, strcmd, client.ServerId);
								if (array == null || array.Length < 2)
								{
									return -200;
								}
								num3 = Convert.ToInt32(array[1]);
								if (num3 < 0)
								{
									return num3 * 1000;
								}
							}
							for (int i = 0; i < list.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "充值有礼", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, 0, 0, 0, null, null, 0, true);
							}
							Global.BroadcastJiFenDaLiHint(client);
							result = num3;
						}
					}
				}
			}
			return result;
		}

		private static SongLiItem GetSongLiItem()
		{
			lock (HuodongCachingMgr._SongLiItemMutex)
			{
				if (HuodongCachingMgr._SongLiItem != null)
				{
					return HuodongCachingMgr._SongLiItem;
				}
			}
			try
			{
				string sectionKey = string.Empty;
				string giftExchangeFileName = Global.GetGiftExchangeFileName(out sectionKey);
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(giftExchangeFileName));
				if (null == xelement)
				{
					return null;
				}
				SongLiItem songLiItem = new SongLiItem();
				xelement = xelement.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey);
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					string text = Global.GetSafeAttributeStr(xelement2, "FromDate");
					string text2 = Global.GetSafeAttributeStr(xelement2, "ToDate");
					if (text.Trim().CompareTo("-1") == 0 && 0 == text2.Trim().CompareTo("-1"))
					{
						text = "2012-06-06 16:16:16";
						text2 = "2032-06-06 16:16:16";
					}
					songLiItem.StartTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(text);
					songLiItem.EndTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(text2);
					songLiItem.IsNeedCode = (int)Global.GetSafeAttributeLong(xelement2, "IsNeedCode");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					if (null != enumerable)
					{
						for (int i = 0; i < enumerable.Count<XElement>(); i++)
						{
							XElement xelement3 = enumerable.ElementAt(i);
							if (null != xelement3)
							{
								int key = (int)Global.GetSafeAttributeLong(xelement3, "ID");
								string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
								if (string.IsNullOrEmpty(safeAttributeStr))
								{
									LogManager.WriteLog(1, string.Format("读取送礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									string[] array = safeAttributeStr.Split(new char[]
									{
										'|'
									});
									if (array.Length <= 0)
									{
										LogManager.WriteLog(1, string.Format("读取送礼活动配置文件中的物品配置项失败", new object[0]), null, true);
									}
									else
									{
										List<GoodsData> value = HuodongCachingMgr.ParseGoodsDataList(array, "送礼活动配置");
										songLiItem.SongGoodsDataDict[key] = value;
									}
								}
							}
						}
					}
				}
				lock (HuodongCachingMgr._SongLiItemMutex)
				{
					HuodongCachingMgr._SongLiItem = songLiItem;
				}
				return songLiItem;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("处理送礼活动配置时发生异常" + ex.ToString());
			}
			return null;
		}

		public static int ResetSongLiItem()
		{
			string giftExchangeFileName = Global.GetGiftExchangeFileName();
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(giftExchangeFileName));
			lock (HuodongCachingMgr._SongLiItemMutex)
			{
				HuodongCachingMgr._SongLiItem = null;
			}
			return 0;
		}

		public static int ProcessGetSongLiGift(GameClient client, int songLiID, string liPinMa)
		{
			string[] array = liPinMa.Split(new char[]
			{
				'$'
			});
			string ptid = "1";
			string channel = "APPS";
			string appid = "1";
			string text = array[0];
			if (array.Length >= 4)
			{
				text = array[0];
				ptid = array[1];
				channel = array[2];
				appid = array[3];
			}
			int zoneID = client.ClientData.ZoneID;
			string strUserID = client.strUserID;
			int roleID = client.ClientData.RoleID;
			string giftId = "";
			int result;
			if (text.Length < 20)
			{
				if (GameManager.PlatConfigMgr.GetGameConfigItemStr("lipinma_v2", "0") == "1")
				{
					if (TimeUtil.NOW() * 10000L - client.ClientData.GetLiPinMaTicks < 30000000L)
					{
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.UseGiftCodeMsg(-11000));
						result = 0;
					}
					else
					{
						client.ClientData.GetLiPinMaTicks = TimeUtil.NOW() * 10000L;
						int num = HuanYingSiYuanClient.getInstance().UseGiftCode(ptid, strUserID, roleID.ToString(), channel, text, appid, zoneID, ref giftId);
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.UseGiftCodeMsg(num));
						if (num < 0)
						{
							result = 0;
						}
						else
						{
							GiftCodeNewManager.getInstance().ProcessGiftCodeCmd(client, strUserID, roleID, giftId, text);
							result = 1;
						}
					}
				}
				else
				{
					result = -1020;
				}
			}
			else if (GameManager.PlatConfigMgr.GetGameConfigItemStr("lipinma_v1", "0").StartsWith("-"))
			{
				result = -1020;
			}
			else
			{
				SongLiItem songLiItem = HuodongCachingMgr.GetSongLiItem();
				if (null == songLiItem)
				{
					result = -1;
				}
				else if (songLiID != GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0) || GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0) <= 0)
				{
					result = -5;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 < songLiItem.StartTicks || num2 >= songLiItem.EndTicks)
					{
						result = -10;
					}
					else if (null == songLiItem.SongGoodsDataDict)
					{
						result = -50;
					}
					else
					{
						string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, songLiID, liPinMa);
						string[] array2 = Global.ExecuteDBCmd(10061, strcmd, client.ServerId);
						if (array2 == null || array2.Length < 2)
						{
							result = -200;
						}
						else
						{
							int num3 = Convert.ToInt32(array2[1]);
							if (num3 < 0)
							{
								result = num3;
							}
							else
							{
								int key = num3;
								List<GoodsData> list = null;
								if (!songLiItem.SongGoodsDataDict.TryGetValue(key, out list) || null == list)
								{
									result = -50;
								}
								else if (!Global.CanAddGoodsDataList(client, list))
								{
									result = -400;
								}
								else if (string.IsNullOrEmpty(liPinMa))
								{
									result = -100;
								}
								else
								{
									strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, songLiID, liPinMa);
									array2 = Global.ExecuteDBCmd(10047, strcmd, client.ServerId);
									if (array2 == null || array2.Length < 2)
									{
										result = -200;
									}
									else
									{
										num3 = Convert.ToInt32(array2[1]);
										if (num3 < 0)
										{
											result = num3;
										}
										else
										{
											client.ClientData.MyHuodongData.SongLiID = songLiID;
											for (int i = 0; i < list.Count; i++)
											{
												Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "系统送礼", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, 0, 0, 0, null, null, 0, true);
											}
											result = 0;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static string praseKalendsGiftCode(string liPinMa, int used = 0)
		{
			string result;
			try
			{
				string text = GameManager.GameConfigMgr.GetGameConfigItemStr("kl_giftcode_u_r_l", "");
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					text = "http://" + text;
					string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("kl_giftcode_md5key", "tmsk_mu_06");
					if (string.IsNullOrEmpty(gameConfigItemStr))
					{
						result = null;
					}
					else
					{
						int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("kl_giftcode_timeout", 200);
						long num = (long)DataHelper.UnixSecondsNow();
						string value = MD5Helper.get_md5_string(string.Concat(new object[]
						{
							liPinMa,
							num,
							used,
							gameConfigItemStr
						}));
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary["giftid"] = liPinMa;
						dictionary["time"] = num.ToString();
						dictionary["used"] = used.ToString();
						dictionary["sign"] = value;
						string json = Global.GetJson(dictionary);
						string text2 = Global.doPost(text, json, gameConfigItemInt);
						if (string.IsNullOrEmpty(text2))
						{
							LogManager.WriteLog(2, string.Format("kl_giftcode text null ", new object[0]), null, true);
							result = null;
						}
						else
						{
							int num2 = 0;
							if (int.TryParse(text2, out num2))
							{
								LogManager.WriteLog(2, string.Format("kl_giftcode return error : {0}", text2), null, true);
								result = null;
							}
							else
							{
								Hashtable hashtable = (Hashtable)MUJson.jsonDecode(text2);
								if (null == hashtable)
								{
									LogManager.WriteLog(2, string.Format("kl_giftcode rspTable null : {0}", text2), null, true);
									result = null;
								}
								else
								{
									string text3 = hashtable["giftid"].ToString();
									if (string.IsNullOrEmpty(text3))
									{
										result = null;
									}
									else
									{
										string text4 = hashtable["time"].ToString();
										if (string.IsNullOrEmpty(text4))
										{
											LogManager.WriteLog(2, string.Format("kl_giftcode time null : {0}", text2), null, true);
											result = null;
										}
										else
										{
											long.TryParse(text4, out num);
											string text5 = hashtable["sign"].ToString();
											if (string.IsNullOrEmpty(text5))
											{
												LogManager.WriteLog(2, string.Format("kl_giftcode sign null : {0}", text2), null, true);
												result = null;
											}
											else
											{
												text5 = text5.ToLower();
												string text6 = text3 + num + gameConfigItemStr;
												string text7 = MD5Helper.get_md5_string(text6);
												text7 = text7.ToLower();
												if (text7 != text5)
												{
													LogManager.WriteLog(2, string.Format("kl_giftcode MD5 error : {0}", text2), null, true);
													result = null;
												}
												else
												{
													if ("-1" != text3)
													{
														if (text3.Length < 5)
														{
															LogManager.WriteLog(2, string.Format("kl_giftcode GiftCode Length error : {0}", text2), null, true);
															return null;
														}
													}
													result = text3;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "praseKalendsGiftCode", false, false);
				result = null;
			}
			return result;
		}

		private static void InitLimitTimeLoginTimes()
		{
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginStartTime.Year != 1971 || HuodongCachingMgr._LimitTimeLoginEndTime.Year != 1971)
				{
					return;
				}
			}
			try
			{
				string uri = "Config/Gifts/HuoDongLoginNumGift.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null != xelement)
				{
					XElement xelement2 = xelement.Element("Activities");
					if (null != xelement2)
					{
						string huoDongTimeByKaiFu = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
						string huoDongTimeByKaiFu2 = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
						lock (HuodongCachingMgr._LimitTimeLoginDict)
						{
							try
							{
								HuodongCachingMgr._LimitTimeLoginStartTime = DateTime.Parse(huoDongTimeByKaiFu);
							}
							catch (Exception)
							{
								LogManager.WriteLog(1, string.Format("根据奖励类型定位限时累计登录配置的开始时间错误, fromDate={0}", huoDongTimeByKaiFu), null, true);
							}
							try
							{
								HuodongCachingMgr._LimitTimeLoginEndTime = DateTime.Parse(huoDongTimeByKaiFu2);
							}
							catch (Exception)
							{
								LogManager.WriteLog(1, string.Format("根据奖励类型定位限时累计登录配置的结束时间错误, toDate={0}", huoDongTimeByKaiFu2), null, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/HuoDongLoginNumGift.xml解析出现异常", ex, true);
			}
		}

		public static bool JugeInLimitTimeLoginPeriod()
		{
			return HuodongCachingMgr.GetLimitTimeLoginHuoDongID() > 0;
		}

		public static int GetLimitTimeLoginHuoDongID()
		{
			HuodongCachingMgr.InitLimitTimeLoginTimes();
			DateTime dateTime = TimeUtil.NowDateTime();
			int result;
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginStartTime.Year == 1971 || HuodongCachingMgr._LimitTimeLoginEndTime.Year == 1971)
				{
					result = -1;
				}
				else if (HuodongCachingMgr._LimitTimeLoginStartTime.Ticks >= HuodongCachingMgr._LimitTimeLoginEndTime.Ticks)
				{
					result = -1;
				}
				else if (dateTime.Ticks >= HuodongCachingMgr._LimitTimeLoginStartTime.Ticks && dateTime.Ticks < HuodongCachingMgr._LimitTimeLoginEndTime.Ticks)
				{
					result = HuodongCachingMgr._LimitTimeLoginStartTime.DayOfYear;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		private static LimitTimeLoginItem GetLimitTimeLoginItem(int whichOne)
		{
			LimitTimeLoginItem limitTimeLoginItem = null;
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginDict.TryGetValue(whichOne, out limitTimeLoginItem))
				{
					return limitTimeLoginItem;
				}
			}
			SystemXmlItem systemXmlItem = null;
			LimitTimeLoginItem result;
			if (!GameManager.SystemDengLuDali.SystemXmlItemDict.TryGetValue(whichOne, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位限时累计登录配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("TimeOl", -1);
				limitTimeLoginItem = new LimitTimeLoginItem
				{
					TimeOl = intValue,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._LimitTimeLoginDict)
				{
					HuodongCachingMgr._LimitTimeLoginDict[whichOne] = limitTimeLoginItem;
				}
				string stringValue = systemXmlItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(stringValue))
				{
					LogManager.WriteLog(1, string.Format("根据奖励类型定位限时累计登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
					result = limitTimeLoginItem;
				}
				else
				{
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("根据奖励类型定位限时累计登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
						result = limitTimeLoginItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "限时累计登录配置");
						limitTimeLoginItem.GoodsDataList = goodsDataList;
						result = limitTimeLoginItem;
					}
				}
			}
			return result;
		}

		public static int ResetLimitTimeLoginItem()
		{
			int result = GameManager.SystemDengLuDali.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				HuodongCachingMgr._LimitTimeLoginStartTime = new DateTime(1971, 1, 1);
				HuodongCachingMgr._LimitTimeLoginEndTime = new DateTime(1971, 1, 1);
				HuodongCachingMgr._LimitTimeLoginDict.Clear();
			}
			return result;
		}

		public static int ProcessGetLimitTimeLoginGift(GameClient client, int whichOne)
		{
			int result;
			if (!HuodongCachingMgr.JugeInLimitTimeLoginPeriod())
			{
				result = -10000;
			}
			else
			{
				LimitTimeLoginItem limitTimeLoginItem = HuodongCachingMgr.GetLimitTimeLoginItem(whichOne);
				if (null == limitTimeLoginItem)
				{
					result = -1;
				}
				else if (limitTimeLoginItem.GoodsDataList == null || limitTimeLoginItem.GoodsDataList.Count <= 0)
				{
					result = -5;
				}
				else if (client.ClientData.MyHuodongData.LimitTimeLoginNum < limitTimeLoginItem.TimeOl)
				{
					result = -10;
				}
				else
				{
					int bitValue = HuodongCachingMgr.GetBitValue(whichOne);
					if ((client.ClientData.MyHuodongData.LimitTimeGiftState & bitValue) == bitValue)
					{
						result = -100;
					}
					else if (!Global.CanAddGoodsDataList(client, limitTimeLoginItem.GoodsDataList))
					{
						result = -200;
					}
					else
					{
						for (int i = 0; i < limitTimeLoginItem.GoodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, limitTimeLoginItem.GoodsDataList[i].GoodsID, limitTimeLoginItem.GoodsDataList[i].GCount, limitTimeLoginItem.GoodsDataList[i].Quality, "", limitTimeLoginItem.GoodsDataList[i].Forge_level, limitTimeLoginItem.GoodsDataList[i].Binding, 0, "", true, 1, "限时累计登录奖励", "1900-01-01 12:00:00", limitTimeLoginItem.GoodsDataList[i].AddPropIndex, limitTimeLoginItem.GoodsDataList[i].BornIndex, limitTimeLoginItem.GoodsDataList[i].Lucky, limitTimeLoginItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
						}
						client.ClientData.MyHuodongData.LimitTimeGiftState = (client.ClientData.MyHuodongData.LimitTimeGiftState | bitValue);
						Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyHuodongData(client);
						result = 0;
					}
				}
			}
			return result;
		}

		public static int GetEveryDayOnLineItemCount()
		{
			return GameManager.systemEveryDayOnLineAwardMgr.SystemXmlItemDict.Count;
		}

		public static EveryDayOnLineAward GetEveryDayOnLineItem(int step)
		{
			EveryDayOnLineAward everyDayOnLineAward = null;
			lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
			{
				if (HuodongCachingMgr._EveryDayOnLineAwardDict.TryGetValue(step, out everyDayOnLineAward))
				{
					return everyDayOnLineAward;
				}
			}
			SystemXmlItem systemXmlItem = null;
			EveryDayOnLineAward result;
			if (!GameManager.systemEveryDayOnLineAwardMgr.SystemXmlItemDict.TryGetValue(step, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位每日在线奖励配置项失败, Step={0}", step), null, true);
				result = null;
			}
			else
			{
				int timeSecs = Global.GMax(systemXmlItem.GetIntValue("TimeSecs", -1), 0) * 60;
				everyDayOnLineAward = new EveryDayOnLineAward
				{
					TimeSecs = timeSecs,
					FallPacketID = -1
				};
				lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
				{
					HuodongCachingMgr._EveryDayOnLineAwardDict[step] = everyDayOnLineAward;
				}
				int intValue = systemXmlItem.GetIntValue("FallID", -1);
				if (intValue == -1)
				{
					LogManager.WriteLog(1, string.Format("根据奖励类型定位每日在线奖励配置项失败, Step={0}", step), null, true);
					result = everyDayOnLineAward;
				}
				else
				{
					everyDayOnLineAward.FallPacketID = intValue;
					result = everyDayOnLineAward;
				}
			}
			return result;
		}

		public static int ResetEveryDayOnLineAwardItem()
		{
			int result = GameManager.systemEveryDayOnLineAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
			{
				HuodongCachingMgr._EveryDayOnLineAwardDict.Clear();
			}
			return result;
		}

		public static int ProcessGetEveryDayOnLineAwardGift(GameClient client, List<GoodsData> goodsDataList, int nType = 0)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != dayOfYear)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = dayOfYear;
			}
			int everyDayOnLineItemCount = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			int result;
			if (everyDayOnLineItemCount == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				result = -1;
			}
			else
			{
				bool flag = false;
				int num = 1;
				int num2 = everyDayOnLineItemCount - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1;
				while (i <= everyDayOnLineItemCount)
				{
					EveryDayOnLineAward everyDayOnLineItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == everyDayOnLineItem)
					{
						return -2;
					}
					if (client.ClientData.DayOnlineSecond < everyDayOnLineItem.TimeSecs)
					{
						if (!flag)
						{
							return -3;
						}
						return 1;
					}
					else
					{
						num = GoodsBaoXiang.ProcessActivityAward(client, everyDayOnLineItem.FallPacketID, 1, 1, "每日在线奖励物品", goodsDataList);
						if (num != 1)
						{
							return num;
						}
						flag = true;
						client.ClientData.MyHuodongData.EveryDayOnLineAwardStep++;
						i++;
					}
				}
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = dayOfYear;
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyHuodongData(client);
				result = num;
			}
			return result;
		}

		public static int ProcessGetEveryDayOnLineAwardGift2(GameClient client, List<GoodsData> goodsDataList, out int nRet)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != dayOfYear)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = dayOfYear;
			}
			int num = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
			int everyDayOnLineItemCount = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			int result;
			if (everyDayOnLineItemCount == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				nRet = -1;
				result = num;
			}
			else
			{
				int num2 = everyDayOnLineItemCount - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				for (int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1; i <= everyDayOnLineItemCount; i++)
				{
					EveryDayOnLineAward everyDayOnLineItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == everyDayOnLineItem)
					{
						nRet = -2;
						return num;
					}
					if (client.ClientData.DayOnlineSecond < everyDayOnLineItem.TimeSecs)
					{
						if (num == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
						{
							nRet = -3;
						}
						else
						{
							nRet = 1;
						}
						return num;
					}
					nRet = GoodsBaoXiang.ProcessActivityAward(client, everyDayOnLineItem.FallPacketID, 1, 1, "每日在线奖励物品", goodsDataList);
					if (nRet != 1)
					{
						return num;
					}
					num++;
				}
				nRet = 1;
				result = num;
			}
			return result;
		}

		public static int GetSeriesLoginCount()
		{
			return GameManager.systemSeriesLoginAwardMgr.SystemXmlItemDict.Count;
		}

		private static SeriesLoginAward GetSeriesLoginAward(int whichOne)
		{
			SeriesLoginAward seriesLoginAward = null;
			lock (HuodongCachingMgr._SeriesLoginAward)
			{
				if (HuodongCachingMgr._SeriesLoginAward.TryGetValue(whichOne, out seriesLoginAward))
				{
					return seriesLoginAward;
				}
			}
			SystemXmlItem systemXmlItem = null;
			SeriesLoginAward result;
			if (!GameManager.systemSeriesLoginAwardMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemXmlItem))
			{
				LogManager.WriteLog(1, string.Format("根据奖励类型定位连续登录奖励配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("LoginTime", -1);
				seriesLoginAward = new SeriesLoginAward
				{
					NeedSeriesLoginNum = intValue,
					FallPacketID = -1
				};
				lock (HuodongCachingMgr._SeriesLoginAward)
				{
					HuodongCachingMgr._SeriesLoginAward[whichOne] = seriesLoginAward;
				}
				int intValue2 = systemXmlItem.GetIntValue("FallID", -1);
				if (intValue2 == -1)
				{
					LogManager.WriteLog(1, string.Format("根据奖励类型定位连续登陆奖励配置项失败, Step={0}", whichOne), null, true);
					result = seriesLoginAward;
				}
				else
				{
					seriesLoginAward.FallPacketID = intValue2;
					result = seriesLoginAward;
				}
			}
			return result;
		}

		public static int ResetSeriesLoginItem()
		{
			int result = GameManager.systemSeriesLoginAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._SeriesLoginAward)
			{
				HuodongCachingMgr._SeriesLoginAward.Clear();
			}
			return result;
		}

		public static int ProcessGetSeriesLoginGift(GameClient client, List<GoodsData> goodsDataList, int nIndex = 0)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == dayOfYear && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep == client.ClientData.SeriesLoginNum)
			{
				result = -2;
			}
			else
			{
				int num = -1;
				int seriesLoginCount = HuodongCachingMgr.GetSeriesLoginCount();
				for (int i = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep + 1; i <= seriesLoginCount; i++)
				{
					SeriesLoginAward seriesLoginAward = HuodongCachingMgr.GetSeriesLoginAward(i);
					if (null == seriesLoginAward)
					{
						return -1;
					}
					if (seriesLoginAward.FallPacketID == -1)
					{
						return -1;
					}
					if (client.ClientData.SeriesLoginNum < seriesLoginAward.NeedSeriesLoginNum)
					{
						break;
					}
					num = GoodsBaoXiang.ProcessActivityAward(client, seriesLoginAward.FallPacketID, 1, 1, "连续登陆奖励物品", goodsDataList);
					if (num != 1)
					{
						break;
					}
					client.ClientData.MyHuodongData.SeriesLoginGetAwardStep++;
				}
				client.ClientData.MyHuodongData.SeriesLoginAwardDayID = dayOfYear;
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyHuodongData(client);
				result = num;
			}
			return result;
		}

		public static int ProcessGetSeriesLoginGift2(GameClient client, List<GoodsData> goodsDataList)
		{
			int num = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep;
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == dayOfYear && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep == client.ClientData.SeriesLoginNum)
			{
				result = num;
			}
			else
			{
				int seriesLoginCount = HuodongCachingMgr.GetSeriesLoginCount();
				for (int i = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep + 1; i <= seriesLoginCount; i++)
				{
					SeriesLoginAward seriesLoginAward = HuodongCachingMgr.GetSeriesLoginAward(i);
					if (null == seriesLoginAward)
					{
						return num;
					}
					if (seriesLoginAward.FallPacketID == -1)
					{
						return num;
					}
					if (client.ClientData.SeriesLoginNum < seriesLoginAward.NeedSeriesLoginNum)
					{
						return num;
					}
					int num2 = GoodsBaoXiang.ProcessActivityAward(client, seriesLoginAward.FallPacketID, 1, 1, "连续登陆奖励物品", goodsDataList);
					if (num2 != 1)
					{
						return num;
					}
					num++;
				}
				result = num;
			}
			return result;
		}

		public static bool LoadActivitiesConfig()
		{
			string text = "";
			Activity activity = HuodongCachingMgr.GetFirstChongZhiActivity();
			if (activity == null || activity.GetParamsValidateCode() < 0)
			{
				text = "HuodongCachingMgr.GetFirstChongZhiActivity()配置项出错";
			}
			else
			{
				activity = HuodongCachingMgr.GetInputFanLiActivity();
				if (activity == null || activity.GetParamsValidateCode() < 0)
				{
					text = "充值返利活动配置项出错";
				}
				else
				{
					activity = HuodongCachingMgr.GetWeekEndInputActivity();
					if (activity == null || activity.GetParamsValidateCode() < 0)
					{
						text = "周末充值活动配置项出错";
					}
					else
					{
						activity = HuodongCachingMgr.GetInputSongActivity();
						if (activity == null || activity.GetParamsValidateCode() < 0)
						{
							text = "充值送礼活动配置项出错";
						}
						else
						{
							activity = HuodongCachingMgr.GetInputKingActivity();
							if (activity == null || activity.GetParamsValidateCode() < 0)
							{
								text = "充值王活动配置项出错";
							}
							else
							{
								activity = HuodongCachingMgr.GetLevelKingActivity();
								if (activity == null || activity.GetParamsValidateCode() < 0)
								{
									text = "冲级王活动配置项出错";
								}
								else
								{
									activity = HuodongCachingMgr.GetEquipKingActivity();
									if (activity == null || activity.GetParamsValidateCode() < 0)
									{
										text = "装备王活动配置项出错";
									}
									else
									{
										activity = HuodongCachingMgr.GetHorseKingActivity();
										if (activity == null || activity.GetParamsValidateCode() < 0)
										{
											text = "坐骑王活动配置项出错";
										}
										else
										{
											activity = HuodongCachingMgr.GetJingMaiKingActivity();
											if (activity == null || activity.GetParamsValidateCode() < 0)
											{
												text = "经脉王活动配置项出错";
											}
											else
											{
												activity = HuodongCachingMgr.GetSpecialActivity();
												if (activity == null || activity.GetParamsValidateCode() < 0)
												{
													text = "专享活动配置项出错";
												}
												else
												{
													activity = HuodongCachingMgr.GetEverydayActivity();
													if (activity == null || activity.GetParamsValidateCode() < 0)
													{
														text = "每日活动配置项出错";
													}
													else
													{
														activity = HuodongCachingMgr.GetSpecPriorityActivity();
														if (activity == null || activity.GetParamsValidateCode() < 0)
														{
															text = "特权活动配置项出错";
														}
														else
														{
															activity = HuodongCachingMgr.GetOneDollarBuyActivity();
															if (activity == null || activity.GetParamsValidateCode() < 0)
															{
																text = "1元直购活动配置项出错";
															}
															else
															{
																activity = HuodongCachingMgr.GetJieRiSuperInputActivity();
																if (activity == null || activity.GetParamsValidateCode() < 0)
																{
																	text = "节日超级充值返利活动配置项出错";
																}
																else
																{
																	activity = HuodongCachingMgr.GetOneDollarChongZhiActivity();
																	if (activity == null || activity.GetParamsValidateCode() < 0)
																	{
																		text = "1元充值活动配置项出错";
																	}
																	else
																	{
																		activity = HuodongCachingMgr.GetInputFanLiNewActivity();
																		if (activity == null || activity.GetParamsValidateCode() < 0)
																		{
																			text = "3周年充值返利活动配置项出错";
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool result;
			if (!string.IsNullOrEmpty(text))
			{
				LogManager.WriteLog(1000, text, null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static DanBiChongZhiActivity GetDanBiChongZhiActivity()
		{
			lock (HuodongCachingMgr._DanBiChongZhiMutex)
			{
				if (HuodongCachingMgr._DanBiChongZhiAct != null)
				{
					return HuodongCachingMgr._DanBiChongZhiAct;
				}
			}
			DanBiChongZhiActivity danBiChongZhiActivity = new DanBiChongZhiActivity();
			if (danBiChongZhiActivity.init())
			{
				lock (HuodongCachingMgr._DanBiChongZhiMutex)
				{
					HuodongCachingMgr._DanBiChongZhiAct = danBiChongZhiActivity;
					return HuodongCachingMgr._DanBiChongZhiAct;
				}
			}
			return null;
		}

		public static int ResetDanBiChongZhiActivity()
		{
			string uri = "Config/JieRiGifts/JieRiDanBiChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._DanBiChongZhiMutex)
			{
				HuodongCachingMgr._DanBiChongZhiAct = null;
			}
			return 0;
		}

		public static FirstChongZhiGift GetFirstChongZhiActivity()
		{
			lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
			{
				if (HuodongCachingMgr._FirstChongZhiActivity != null)
				{
					return HuodongCachingMgr._FirstChongZhiActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/FirstCharge.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				FirstChongZhiGift firstChongZhiGift = new FirstChongZhiGift();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					firstChongZhiGift.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					firstChongZhiGift.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					firstChongZhiGift.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					firstChongZhiGift.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
					firstChongZhiGift.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
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
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取首充活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取首充活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "首充活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取首充活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取首充活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "首充活动配置");
								}
							}
							firstChongZhiGift.AwardDict = awardItem;
							firstChongZhiGift.AwardDict2 = awardItem2;
						}
					}
				}
				firstChongZhiGift.PredealDateTime();
				lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
				{
					HuodongCachingMgr._FirstChongZhiActivity = firstChongZhiGift;
				}
				return firstChongZhiGift;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/FirstCharge.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetFirstChongZhiGift()
		{
			string uri = "Config/Gifts/FirstCharge.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
			{
				HuodongCachingMgr._FirstChongZhiActivity = null;
			}
			return 0;
		}

		public static InputFanLiActivity GetInputFanLiActivity()
		{
			lock (HuodongCachingMgr._InputFanLiActivityMutex)
			{
				if (HuodongCachingMgr._InputFanLiActivity != null)
				{
					return HuodongCachingMgr._InputFanLiActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/FanLi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				InputFanLiActivity inputFanLiActivity = new InputFanLiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					inputFanLiActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					inputFanLiActivity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 23, 59, 59);
					inputFanLiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					inputFanLiActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(8, 0, 0, 0);
					inputFanLiActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						inputFanLiActivity.FanLiPersent = Global.GetSafeAttributeDouble(xelement3, "FanLi");
						if (inputFanLiActivity.FanLiPersent < 0.0)
						{
							inputFanLiActivity.FanLiPersent = 0.0;
						}
					}
				}
				lock (HuodongCachingMgr._InputFanLiActivityMutex)
				{
					HuodongCachingMgr._InputFanLiActivity = inputFanLiActivity;
				}
				return inputFanLiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/FanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetInputFanLiActivity()
		{
			string uri = "Config/Gifts/FanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._InputFanLiActivityMutex)
			{
				HuodongCachingMgr._InputFanLiActivity = null;
			}
			return 0;
		}

		public static InputSongActivity GetInputSongActivity()
		{
			lock (HuodongCachingMgr._InputSongActivityMutex)
			{
				if (HuodongCachingMgr._InputSongActivity != null)
				{
					return HuodongCachingMgr._InputSongActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/ChongZhiSong.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				InputSongActivity inputSongActivity = new InputSongActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					inputSongActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					inputSongActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					inputSongActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					inputSongActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					inputSongActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
				}
				inputSongActivity.MyAwardItem = new AwardItem();
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						inputSongActivity.MyAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
						inputSongActivity.MyAwardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "YuanBao"));
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取充值加送活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("读取充值加送活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								inputSongActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "充值加送活动配置");
							}
						}
					}
				}
				inputSongActivity.PredealDateTime();
				lock (HuodongCachingMgr._InputSongActivityMutex)
				{
					HuodongCachingMgr._InputSongActivity = inputSongActivity;
				}
				return inputSongActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/ChongZhiSong.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetInputSongActivity()
		{
			string uri = "Config/Gifts/ChongZhiSong.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._InputSongActivityMutex)
			{
				HuodongCachingMgr._InputSongActivity = null;
			}
			return 0;
		}

		public static KingActivity GetInputKingActivity()
		{
			lock (HuodongCachingMgr._InputKingActivityMutex)
			{
				if (HuodongCachingMgr._InputKingActivity != null)
				{
					return HuodongCachingMgr._InputKingActivity;
				}
			}
			try
			{
				string uri = "Config/XinFuGifts/MuChongZhi.xml";
				if (Global.isDoubleXinFu(34))
				{
					uri = "Config/XinFuGifts/MuDoubleChongZhi.xml";
				}
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取充值王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "充值王活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取充值王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "充值王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "ID");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
									kingActivity.AwardDict2.Add(i, awardItem2);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._InputKingActivityMutex)
				{
					HuodongCachingMgr._InputKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/XinFuGifts/MuChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetInputKingActivity()
		{
			string uri = "Config/Gifts/MuChongZhi.xml";
			if (Global.isDoubleXinFu(34))
			{
				uri = "Config/XinFuGifts/MuDoubleChongZhi.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._InputKingActivityMutex)
			{
				HuodongCachingMgr._InputKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetLevelKingActivity()
		{
			lock (HuodongCachingMgr._LevelKingActivityMutex)
			{
				if (HuodongCachingMgr._LevelKingActivity != null)
				{
					return HuodongCachingMgr._LevelKingActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/LevelKing.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinLevel"));
							awardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "YuanBao"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取冲级王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取冲级王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "冲级王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "Ranking");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._LevelKingActivityMutex)
				{
					HuodongCachingMgr._LevelKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/LevelKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetLevelKingActivity()
		{
			string uri = "Config/Gifts/LevelKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._LevelKingActivityMutex)
			{
				HuodongCachingMgr._LevelKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetEquipKingActivity()
		{
			lock (HuodongCachingMgr._EquipKingActivityMutex)
			{
				if (HuodongCachingMgr._EquipKingActivity != null)
				{
					return HuodongCachingMgr._EquipKingActivity;
				}
			}
			try
			{
				string uri = "Config/XinFuGifts/MuBoss.xml";
				if (Global.isDoubleXinFu(36))
				{
					uri = "Config/XinFuGifts/MuDoubleBoss.xml";
				}
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinBoss"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取Boss王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取Boss王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Boss王活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取Boss王活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取Boss王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "Boss王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "ID");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
									kingActivity.AwardDict2.Add(i, awardItem2);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._EquipKingActivityMutex)
				{
					HuodongCachingMgr._EquipKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/XinFuGifts/MuBoss.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetEquipKingActivity()
		{
			string uri = "Config/Gifts/MuBoss.xml";
			if (Global.isDoubleXinFu(36))
			{
				uri = "Config/XinFuGifts/MuDoubleBoss.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._EquipKingActivityMutex)
			{
				HuodongCachingMgr._EquipKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetHorseKingActivity()
		{
			lock (HuodongCachingMgr._HorseKingActivityMutex)
			{
				if (HuodongCachingMgr._HorseKingActivity != null)
				{
					return HuodongCachingMgr._HorseKingActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/WuXueKing.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinWuXue"));
							awardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "YuanBao"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取武学王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取武学王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "武学王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "Ranking");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._HorseKingActivityMutex)
				{
					HuodongCachingMgr._HorseKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/WuXueKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHorseKingActivity()
		{
			string uri = "Config/Gifts/WuXueKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._HorseKingActivityMutex)
			{
				HuodongCachingMgr._HorseKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetJingMaiKingActivity()
		{
			lock (HuodongCachingMgr._JingMaiKingActivityMutex)
			{
				if (HuodongCachingMgr._JingMaiKingActivity != null)
				{
					return HuodongCachingMgr._JingMaiKingActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/JingMaiKing.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinJingMai"));
							awardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "YuanBao"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取经脉王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取经脉王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "经脉王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "Ranking");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._JingMaiKingActivityMutex)
				{
					HuodongCachingMgr._JingMaiKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/JingMaiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJingMaiKingActivity()
		{
			string uri = "Config/Gifts/JingMaiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._JingMaiKingActivityMutex)
			{
				HuodongCachingMgr._JingMaiKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetXinXiaoFeiKingActivity()
		{
			lock (HuodongCachingMgr._XinXiaofeiKingMutex)
			{
				if (HuodongCachingMgr._XinXiaofeiKingActivity != null)
				{
					return HuodongCachingMgr._XinXiaofeiKingActivity;
				}
			}
			try
			{
				string uri = "Config/XinFuGifts/MuXiaoFei.xml";
				if (Global.isDoubleXinFu(35))
				{
					uri = "Config/XinFuGifts/MuDoubleXiaoFei.xml";
				}
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					kingActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					kingActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "新服消费达人活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "新服消费达人活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "ID");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
									kingActivity.AwardDict2.Add(i, awardItem2);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._XinXiaofeiKingMutex)
				{
					HuodongCachingMgr._XinXiaofeiKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/XinFuGifts/MuXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetXinXiaoFeiKingActivity()
		{
			string uri = "Config/JieRiGifts/MuXiaoFei.xml";
			if (Global.isDoubleXinFu(35))
			{
				uri = "Config/XinFuGifts/MuDoubleXiaoFei.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._XinXiaofeiKingMutex)
			{
				HuodongCachingMgr._XinXiaofeiKingActivity = null;
			}
			return 0;
		}

		public static void ReadAwardConfig(XElement args, out Dictionary<int, AwardItem> AwardDict, out Dictionary<int, AwardItem> AwardDict2)
		{
			AwardDict = new Dictionary<int, AwardItem>();
			AwardDict2 = new Dictionary<int, AwardItem>();
			if (null != args)
			{
				IEnumerable<XElement> enumerable = args.Elements();
				foreach (XElement xelement in enumerable)
				{
					if (null != xelement)
					{
						AwardItem awardItem = new AwardItem();
						AwardItem awardItem2 = new AwardItem();
						XAttribute xattribute = xelement.Attribute("MinYuanBao");
						if (xattribute != null)
						{
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement, "MinYuanBao"));
						}
						awardItem.AwardYuanBao = 0;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsOne");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("读取新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "新服消费达人活动配置");
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsTwo");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "新服消费达人活动配置");
							}
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "ID");
						int key = Global.SafeConvertToInt32(safeAttributeStr2);
						AwardDict.Add(key, awardItem);
						AwardDict2.Add(key, awardItem2);
					}
				}
			}
		}

		public static void ReadAwardConfig(XElement args, out Dictionary<int, AwardItem> AwardDict, out Dictionary<int, AwardItem> AwardDict2, out Dictionary<int, AwardEffectTimeItem> AwardDict3)
		{
			AwardDict = new Dictionary<int, AwardItem>();
			AwardDict2 = new Dictionary<int, AwardItem>();
			AwardDict3 = new Dictionary<int, AwardEffectTimeItem>();
			if (null != args)
			{
				IEnumerable<XElement> enumerable = args.Elements();
				foreach (XElement xelement in enumerable)
				{
					if (null != xelement)
					{
						AwardItem awardItem = new AwardItem();
						AwardItem awardItem2 = new AwardItem();
						AwardEffectTimeItem awardEffectTimeItem = new AwardEffectTimeItem();
						XAttribute xattribute = xelement.Attribute("MinYuanBao");
						if (xattribute != null)
						{
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement, "MinYuanBao"));
						}
						awardItem.AwardYuanBao = 0;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsOne");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("节日活动返利配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("节日活动返利配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "节日活动返利配置");
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsTwo");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("节日活动返利配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("节日活动返利配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "节日活动返利配置");
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsThr");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								awardEffectTimeItem.Init(safeAttributeStr, Global.GetSafeAttributeStr(xelement, "EffectiveTime"), "节日返利");
							}
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "ID");
						int key = Global.SafeConvertToInt32(safeAttributeStr2);
						AwardDict.Add(key, awardItem);
						AwardDict2.Add(key, awardItem2);
						AwardDict3.Add(key, awardEffectTimeItem);
					}
				}
			}
		}

		public static HuodongCachingMgr.TotalChargeActivity GetTotalChargeActivity()
		{
			lock (HuodongCachingMgr._TotalChargeActivityMutex)
			{
				if (HuodongCachingMgr._TotalChargeActivity != null)
				{
					return HuodongCachingMgr._TotalChargeActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/LeiJiChongZhi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HuodongCachingMgr.TotalChargeActivity totalChargeActivity = new HuodongCachingMgr.TotalChargeActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					totalChargeActivity.ActivityType = 38;
				}
				xelement2 = xelement.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(xelement2, out totalChargeActivity.AwardDict, out totalChargeActivity.AwardDict2);
				totalChargeActivity.PredealDateTime();
				lock (HuodongCachingMgr._TotalChargeActivityMutex)
				{
					HuodongCachingMgr._TotalChargeActivity = totalChargeActivity;
				}
				return totalChargeActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/LeiJiChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetTotalChargeActivity()
		{
			string uri = "Config/Gifts/LeiJiChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._TotalChargeActivityMutex)
			{
				HuodongCachingMgr._TotalChargeActivity = null;
			}
			return 0;
		}

		public static HuodongCachingMgr.TotalConsumeActivity GetTotalConsumeActivity()
		{
			lock (HuodongCachingMgr._TotalConsumeActivityMutex)
			{
				if (HuodongCachingMgr._TotalConsumeActivity != null)
				{
					return HuodongCachingMgr._TotalConsumeActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/LeiJiXiaoFei.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HuodongCachingMgr.TotalConsumeActivity totalConsumeActivity = new HuodongCachingMgr.TotalConsumeActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					totalConsumeActivity.ActivityType = 39;
				}
				xelement2 = xelement.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(xelement2, out totalConsumeActivity.AwardDict, out totalConsumeActivity.AwardDict2);
				totalConsumeActivity.PredealDateTime();
				lock (HuodongCachingMgr._TotalConsumeActivityMutex)
				{
					HuodongCachingMgr._TotalConsumeActivity = totalConsumeActivity;
				}
				return totalConsumeActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/LeiJiXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetTotalConsumeActivity()
		{
			string uri = "Config/Gifts/LeiJiXiaoFei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._TotalConsumeActivityMutex)
			{
				HuodongCachingMgr._TotalConsumeActivity = null;
			}
			return 0;
		}

		public static JieriFanLiActivity GetJieriFanLiActivity(ActivityTypes nActType)
		{
			int num = 0;
			string attribute = "";
			switch (nActType)
			{
			case ActivityTypes.JieriWing:
				num = 0;
				attribute = "WingLevel";
				break;
			case ActivityTypes.JieriAddon:
				num = 1;
				attribute = "ZhuiJiaLevel";
				break;
			case ActivityTypes.JieriStrengthen:
				num = 2;
				attribute = "QiangHuaLevel";
				break;
			case ActivityTypes.JieriAchievement:
				num = 3;
				attribute = "ChengJiuLevel";
				break;
			case ActivityTypes.JieriMilitaryRank:
				num = 4;
				attribute = "JunXianLevel";
				break;
			case ActivityTypes.JieriVIPFanli:
				num = 5;
				attribute = "VIPLevel";
				break;
			case ActivityTypes.JieriAmulet:
				num = 6;
				attribute = "HuShenFuLevel";
				break;
			case ActivityTypes.JieriArchangel:
				num = 7;
				attribute = "DaTianShiLevel";
				break;
			case ActivityTypes.JieriLianXuCharge:
				break;
			case ActivityTypes.JieriMarriage:
				num = 8;
				attribute = "GoodWillSuit";
				break;
			default:
				switch (nActType)
				{
				case ActivityTypes.JieRiHuiJi:
					num = 9;
					attribute = "EmblemLevel";
					break;
				case ActivityTypes.JieRiFuWen:
					num = 10;
					attribute = "FuWenLevel";
					break;
				}
				break;
			}
			lock (HuodongCachingMgr._JieriWingFanliActMutex)
			{
				if (null != HuodongCachingMgr._JieriWingFanliAct[num])
				{
					return HuodongCachingMgr._JieriWingFanliAct[num];
				}
			}
			string text = "";
			try
			{
				text = "Config/JieRiGifts/";
				switch (nActType)
				{
				case ActivityTypes.JieriWing:
					text += "WingFanLi.xml";
					break;
				case ActivityTypes.JieriAddon:
					text += "ZhuiJiaFanLi.xml";
					break;
				case ActivityTypes.JieriStrengthen:
					text += "QiangHuaFanLi.xml";
					break;
				case ActivityTypes.JieriAchievement:
					text += "ChengJiuFanLi.xml";
					break;
				case ActivityTypes.JieriMilitaryRank:
					text += "JunXianFanLi.xml";
					break;
				case ActivityTypes.JieriVIPFanli:
					text += "VIPFanLi.xml";
					break;
				case ActivityTypes.JieriAmulet:
					text += "HuShenFuFanLi.xml";
					break;
				case ActivityTypes.JieriArchangel:
					text += "DaTianShiFanLi.xml";
					break;
				case ActivityTypes.JieriLianXuCharge:
					break;
				case ActivityTypes.JieriMarriage:
					text += "HunYinFanLi.xml";
					break;
				default:
					switch (nActType)
					{
					case ActivityTypes.JieRiHuiJi:
						text += "JieRiHuiJiFanLi.xml";
						break;
					case ActivityTypes.JieRiFuWen:
						text += "JieRiFuWenFanLi.xml";
						break;
					}
					break;
				}
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
				if (null == xelement)
				{
					return null;
				}
				JieriFanLiActivity jieriFanLiActivity = new JieriFanLiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieriFanLiActivity.ActivityType = (int)nActType;
					jieriFanLiActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieriFanLiActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieriFanLiActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieriFanLiActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(xelement2, out jieriFanLiActivity.AwardDict, out jieriFanLiActivity.AwardDict2, out jieriFanLiActivity.AwardDict3);
				jieriFanLiActivity.PredealDateTime();
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "ID");
							int key = Global.SafeConvertToInt32(safeAttributeStr);
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, attribute);
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							jieriFanLiActivity.AwardDict[key].MinAwardCondionValue = Convert.ToInt32(array[0]);
							if (array.Length > 1)
							{
								jieriFanLiActivity.AwardDict[key].MinAwardCondionValue2 = Convert.ToInt32(array[1]);
							}
						}
					}
				}
				lock (HuodongCachingMgr._JieriWingFanliActMutex)
				{
					HuodongCachingMgr._JieriWingFanliAct[num] = jieriFanLiActivity;
				}
				return jieriFanLiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常", text), ex, true);
			}
			return null;
		}

		public static JieriLianXuChargeActivity GetJieriLianXuChargeActivity()
		{
			lock (HuodongCachingMgr._JieriLianXuChargeMutex)
			{
				if (HuodongCachingMgr._JieriLianXuChargeAct != null)
				{
					return HuodongCachingMgr._JieriLianXuChargeAct;
				}
			}
			JieriLianXuChargeActivity jieriLianXuChargeActivity = new JieriLianXuChargeActivity();
			if (jieriLianXuChargeActivity.Init())
			{
				lock (HuodongCachingMgr._JieriLianXuChargeMutex)
				{
					HuodongCachingMgr._JieriLianXuChargeAct = jieriLianXuChargeActivity;
					return HuodongCachingMgr._JieriLianXuChargeAct;
				}
			}
			return null;
		}

		public static int ResetJieriLianXuChargeActivity()
		{
			lock (HuodongCachingMgr._JieriLianXuChargeMutex)
			{
				HuodongCachingMgr._JieriLianXuChargeAct = null;
			}
			return 0;
		}

		public static JieriPlatChargeKingEveryDay GetJieriPCKingEveryDayActivity()
		{
			lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
			{
				if (HuodongCachingMgr._JieriPCKingEveryDayAct != null)
				{
					return HuodongCachingMgr._JieriPCKingEveryDayAct;
				}
			}
			JieriPlatChargeKingEveryDay jieriPlatChargeKingEveryDay = new JieriPlatChargeKingEveryDay();
			if (jieriPlatChargeKingEveryDay.Init())
			{
				lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
				{
					HuodongCachingMgr._JieriPCKingEveryDayAct = jieriPlatChargeKingEveryDay;
					return HuodongCachingMgr._JieriPCKingEveryDayAct;
				}
			}
			return null;
		}

		public static int ResetJieriPCKingActivityEveryDay()
		{
			lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
			{
				HuodongCachingMgr._JieriPCKingEveryDayAct = null;
			}
			return 0;
		}

		public static JieriPlatChargeKing GetJieriPlatChargeKingActivity()
		{
			lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
			{
				if (HuodongCachingMgr._JieriPlatChargeKingAct != null)
				{
					return HuodongCachingMgr._JieriPlatChargeKingAct;
				}
			}
			JieriPlatChargeKing jieriPlatChargeKing = new JieriPlatChargeKing();
			if (jieriPlatChargeKing.Init())
			{
				lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
				{
					HuodongCachingMgr._JieriPlatChargeKingAct = jieriPlatChargeKing;
					return HuodongCachingMgr._JieriPlatChargeKingAct;
				}
			}
			return null;
		}

		public static int ResetJieriPlatChargeKingActivity()
		{
			lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
			{
				HuodongCachingMgr._JieriPlatChargeKingAct = null;
			}
			return 0;
		}

		private static void InitUpLevelAwardItemDict()
		{
			if (null == HuodongCachingMgr.UpLevelAwardItemDict)
			{
				try
				{
					string uri = "Config/Gifts/UpLevelAward.xml";
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
					if (null != xelement)
					{
						Dictionary<int, UpLevelAwardItem> dictionary = new Dictionary<int, UpLevelAwardItem>();
						IEnumerable<XElement> enumerable = xelement.Elements("Level");
						if (null != enumerable)
						{
							foreach (XElement xml in enumerable)
							{
								UpLevelAwardItem upLevelAwardItem = new UpLevelAwardItem
								{
									ID = (int)Global.GetSafeAttributeLong(xml, "ID"),
									MinDay = (int)Global.GetSafeAttributeLong(xml, "MinDay"),
									MaxDay = (int)Global.GetSafeAttributeLong(xml, "MaxDay"),
									AwardYuanBao = (int)Global.GetSafeAttributeLong(xml, "AwardYuanBao")
								};
								dictionary[upLevelAwardItem.ID] = upLevelAwardItem;
							}
						}
						HuodongCachingMgr.UpLevelAwardItemDict = dictionary;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "Config/Gifts/UpLevelAward.xml解析出现异常", ex, true);
				}
			}
		}

		public static void ProcessUpLevelAward4_60Level_100Level(GameClient client, int oldLevel, int newLevel)
		{
			HuodongCachingMgr.InitUpLevelAwardItemDict();
			Dictionary<int, UpLevelAwardItem> upLevelAwardItemDict = HuodongCachingMgr.UpLevelAwardItemDict;
			if (null != upLevelAwardItemDict)
			{
				int num = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetRegTime(client.ClientData), false);
				num++;
				int num2 = 0;
				if (oldLevel < 60 && newLevel >= 60)
				{
					for (int i = 0; i < upLevelAwardItemDict.Values.Count; i++)
					{
						UpLevelAwardItem upLevelAwardItem = upLevelAwardItemDict.Values.ElementAt(i);
						if (num >= upLevelAwardItem.MinDay && num <= upLevelAwardItem.MaxDay)
						{
							num2 = (int)Math.Pow(2.0, (double)i);
						}
					}
				}
				else if (oldLevel < 100 && newLevel >= 100)
				{
					if (num >= 1 && num <= 100)
					{
						num2 = 16;
					}
				}
				if (num2 > 0)
				{
					int num3 = GameManager.ClientMgr.GetTo60or100ID(client);
					if ((num3 & num2) != num2)
					{
						num3 |= num2;
						GameManager.ClientMgr.ModifyTo60or100ID(client, num3, true, true);
					}
				}
			}
		}

		public static void ProcessGetUpLevelAward4_60Level_100Level(GameClient client, int awardID)
		{
			HuodongCachingMgr.InitUpLevelAwardItemDict();
			Dictionary<int, UpLevelAwardItem> upLevelAwardItemDict = HuodongCachingMgr.UpLevelAwardItemDict;
			if (null != upLevelAwardItemDict)
			{
				UpLevelAwardItem upLevelAwardItem = null;
				if (upLevelAwardItemDict.TryGetValue(awardID, out upLevelAwardItem))
				{
					if (null != upLevelAwardItem)
					{
						int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-to60level", 0);
						if (gameConfigItemInt <= 0)
						{
							int num = 32;
							int num2 = GameManager.ClientMgr.GetTo60or100ID(client);
							if ((num2 & num) == num)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(388, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							else
							{
								num2 |= num;
								GameManager.ClientMgr.ModifyTo60or100ID(client, num2, true, true);
								bool flag = false;
								if (null != client)
								{
									flag = GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, upLevelAwardItem.AwardYuanBao, "达到60级绑定元宝奖励");
								}
								if (!flag)
								{
									LogManager.WriteLog(2, string.Format("处理达到60级绑定元宝奖励时，为角色名称={0}, 添加绑定元宝{1} 失败", client.ClientData.RoleName, upLevelAwardItem.AwardYuanBao), null, true);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(389, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format(GLang.GetLang(391, new object[0]), upLevelAwardItem.AwardYuanBao), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
									Global.BroadcastClientTo60(client, upLevelAwardItem.MinDay, upLevelAwardItem.MaxDay, upLevelAwardItem.AwardYuanBao);
								}
							}
						}
					}
				}
			}
		}

		private static void InitKaiFuGiftItemDict()
		{
			if (null == HuodongCachingMgr.KaiFuGiftItemDict)
			{
				try
				{
					string uri = "Config/Gifts/KaiFuGift.xml";
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
					if (null != xelement)
					{
						Dictionary<int, KaiFuGiftItem> dictionary = new Dictionary<int, KaiFuGiftItem>();
						IEnumerable<XElement> enumerable = xelement.Elements("KaiFu");
						if (null != enumerable)
						{
							foreach (XElement xml in enumerable)
							{
								KaiFuGiftItem kaiFuGiftItem = new KaiFuGiftItem
								{
									Day = (int)Global.GetSafeAttributeLong(xml, "Day"),
									MinTime = (int)Global.GetSafeAttributeLong(xml, "MinTime"),
									MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel"),
									AwardYuanBao = (int)Global.GetSafeAttributeLong(xml, "AwardYuanBao")
								};
								dictionary[kaiFuGiftItem.Day] = kaiFuGiftItem;
							}
						}
						HuodongCachingMgr.KaiFuGiftItemDict = dictionary;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "Config/Gifts/KaiFuGift.xml解析出现异常", ex, true);
				}
			}
		}

		public static void ProcessKaiFuGiftAward(GameClient client)
		{
			int level = client.ClientData.Level;
			if (level >= 40)
			{
				int num = client.ClientData.TotalOnlineSecs / 3600;
				if (num >= 2)
				{
					int num2 = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
					num2++;
					Dictionary<int, KaiFuGiftItem> kaiFuGiftItemDict = HuodongCachingMgr.KaiFuGiftItemDict;
					if (null != kaiFuGiftItemDict)
					{
						KaiFuGiftItem kaiFuGiftItem = null;
						if (kaiFuGiftItemDict.TryGetValue(num2, out kaiFuGiftItem))
						{
							if (level >= kaiFuGiftItem.MinLevel)
							{
								if (num >= kaiFuGiftItem.MinTime)
								{
									int kaiFuOnlineDayID = GameManager.ClientMgr.GetKaiFuOnlineDayID(client);
									if (num2 != kaiFuOnlineDayID)
									{
										GameManager.ClientMgr.ModifyKaiFuOnlineDayID(client, num2, true, true);
										int num3 = Global.GetRoleParamsInt32FromDB(client, "KaiFuOnlineDayBit");
										num3 |= (int)Math.Pow(2.0, (double)(num2 - 1));
										Global.SaveRoleParamsInt32ValueToDB(client, "KaiFuOnlineDayBit", num3, true);
									}
								}
							}
						}
					}
				}
			}
		}

		public static void ProcessDayOnlineSecs(GameClient client, int preLoginDay)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear != preLoginDay)
			{
				int num = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
				num++;
				if (num > 1)
				{
					if (num < 9)
					{
						int totalOnlineSecs = client.ClientData.TotalOnlineSecs;
						Global.SaveRoleParamsInt32ValueToDB(client, string.Format("{0}{1}", "KaiFuOnlineDayTimes_", num - 1), totalOnlineSecs, true);
					}
				}
			}
		}

		public static bool GetCurrentDayKaiFuOnlineSecs(GameClient client, out int totalOnlineSecs, out int dayID)
		{
			totalOnlineSecs = 0;
			dayID = 0;
			int num = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
			num++;
			bool result;
			if (num >= 8)
			{
				result = false;
			}
			else
			{
				totalOnlineSecs = client.ClientData.TotalOnlineSecs;
				dayID = num;
				result = true;
			}
			return result;
		}

		public static void ProcessKaiFuGiftAwardActions()
		{
			HuodongCachingMgr.ProcessGetKaiFuGiftAward();
			HuodongCachingMgr.ProcessAutoAddKaiFuGiftRoleNum();
		}

		public static void ProcessGetKaiFuGiftAward()
		{
			int num = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
			num++;
			if (num > 1)
			{
				if (num < 9)
				{
					int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
					if (dayOfYear != HuodongCachingMgr.LastProcessGetKaiFuGiftAwardDayID)
					{
						if (HuodongCachingMgr.ProcessKaiFuGiftAwardHour == TimeUtil.NowDateTime().Hour)
						{
							HuodongCachingMgr.LastProcessGetKaiFuGiftAwardDayID = dayOfYear;
							int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-kaifuaward", 0);
							if (gameConfigItemInt <= 0)
							{
								string[] array = Global.ExecuteDBCmd(10111, string.Format("{0}", num - 1), 0);
								if (array != null && array.Length >= 4)
								{
									int num2 = Global.SafeConvertToInt32(array[0]);
									if (num2 > 0)
									{
										int num3 = Global.SafeConvertToInt32(array[1]);
										string text = array[2];
										int num4 = Global.SafeConvertToInt32(array[3]);
										Dictionary<int, KaiFuGiftItem> kaiFuGiftItemDict = HuodongCachingMgr.KaiFuGiftItemDict;
										if (null != kaiFuGiftItemDict)
										{
											KaiFuGiftItem kaiFuGiftItem = null;
											if (kaiFuGiftItemDict.TryGetValue(num - 1, out kaiFuGiftItem))
											{
												GameClient gameClient = GameManager.ClientMgr.FindClient(num2);
												bool flag;
												if (null != gameClient)
												{
													flag = GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, kaiFuGiftItem.AwardYuanBao, "开服在线奖励", ActivityTypes.None, "");
												}
												else
												{
													flag = GameManager.ClientMgr.AddUserMoneyOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, num2, kaiFuGiftItem.AwardYuanBao, "开服在线奖励", num3, Global.QueryUserMoneyFromDB(num2, text, 0));
												}
												if (!flag)
												{
													LogManager.WriteLog(2, string.Format("处理开服在线奖励活动时，为角色名称={0}, 添加元宝{1} 失败", text, kaiFuGiftItem.AwardYuanBao), null, true);
												}
												else
												{
													GameManager.DBCmdMgr.AddDBCmd(10112, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
													{
														num2,
														Math.Max(1, num - 1),
														kaiFuGiftItem.AwardYuanBao,
														num4,
														num3
													}), null, 0);
													GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", num2, kaiFuGiftItem.AwardYuanBao, GLang.GetLang(392, new object[0])), null, 0);
													Global.BroadcastClientKaiFuOnlineRandomAward(num3, text, kaiFuGiftItem.AwardYuanBao);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void ProcessAutoAddKaiFuGiftRoleNum()
		{
		}

		public static void FixKaiFuOnlineAwardDataList(List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList, int dayID, int serverId)
		{
			if (null != kaiFuOnlineAwardDataList)
			{
				bool flag = false;
				for (int i = 0; i < kaiFuOnlineAwardDataList.Count; i++)
				{
					if (kaiFuOnlineAwardDataList[i].DayID == dayID)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					string[] array = Global.ExecuteDBCmd(10111, string.Format("{0}", dayID), serverId);
					if (array != null && array.Length >= 4)
					{
						int totalRoleNum = Global.SafeConvertToInt32(array[3]);
						kaiFuOnlineAwardDataList.Add(new KaiFuOnlineAwardData
						{
							DayID = dayID,
							TotalRoleNum = totalRoleNum
						});
					}
				}
			}
		}

		public static void OnJieriRoleLogin(GameClient client, int preLoginDay, bool isLogin = false)
		{
			int offsetDayNow = Global.GetOffsetDayNow();
			int num = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "10152"));
			if (num > offsetDayNow)
			{
				LogManager.WriteLog(2, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10152", offsetDayNow, true);
				OneDollarBuyActivity oneDollarBuyActivity = HuodongCachingMgr.GetOneDollarBuyActivity();
				if (null != oneDollarBuyActivity)
				{
					oneDollarBuyActivity.OnRoleLogin(client);
				}
				OneDollarChongZhi oneDollarChongZhiActivity = HuodongCachingMgr.GetOneDollarChongZhiActivity();
				if (null != oneDollarChongZhiActivity)
				{
					oneDollarChongZhiActivity.OnRoleLogin(client);
				}
				InputFanLiNew inputFanLiNewActivity = HuodongCachingMgr.GetInputFanLiNewActivity();
				if (null != inputFanLiNewActivity)
				{
					inputFanLiNewActivity.OnRoleLogin(client);
				}
				RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
				if (null != regressActiveOpen)
				{
					regressActiveOpen.OnRoleLogin(client);
				}
				SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
				if (null != specialActivity)
				{
					specialActivity.OnRoleLogin(client, isLogin);
				}
				EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
				if (null != everydayActivity)
				{
					everydayActivity.OnRoleLogin(client);
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specialActivity)
				{
					specPriorityActivity.OnRoleLogin(client, isLogin);
				}
				WeedEndInputActivity weekEndInputActivity = HuodongCachingMgr.GetWeekEndInputActivity();
				if (null != weekEndInputActivity)
				{
					weekEndInputActivity.OnRoleLogin(client, isLogin);
				}
				JieriSuperInputActivity jieRiSuperInputActivity = HuodongCachingMgr.GetJieRiSuperInputActivity();
				if (null != jieRiSuperInputActivity)
				{
					jieRiSuperInputActivity.OnRoleLogin(client);
				}
				int num2 = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "JieriLoginDayID"));
				JieRiDengLuActivity jieRiDengLuActivity = HuodongCachingMgr.GetJieRiDengLuActivity();
				if (jieRiDengLuActivity != null && jieRiDengLuActivity.InActivityTime() && num2 < offsetDayNow)
				{
					DateTime now = DateTime.Parse(jieRiDengLuActivity.FromDate);
					DateTime now2 = DateTime.Parse(jieRiDengLuActivity.ToDate);
					int offsetDay = Global.GetOffsetDay(now);
					int offsetDay2 = Global.GetOffsetDay(now2);
					int num3 = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "JieriLoginNum"));
					if (num2 >= offsetDay && num2 <= offsetDay2)
					{
						num3++;
						if (num3 > offsetDayNow - offsetDay + 1)
						{
							num3 = offsetDayNow - offsetDay + 1;
						}
					}
					else
					{
						num3 = 1;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "JieriLoginNum", num3, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "JieriLoginDayID", offsetDayNow, true);
				}
			}
		}

		public static int GetZiKaTodayLeftMergeNum(GameClient client, int index)
		{
			JieRiZiKaLiaBaoActivity jieRiZiKaLiaBaoActivity = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
			int result;
			if (null == jieRiZiKaLiaBaoActivity)
			{
				result = 0;
			}
			else
			{
				JieRiZiKa award = jieRiZiKaLiaBaoActivity.GetAward(index);
				if (null == award)
				{
					result = 0;
				}
				else
				{
					int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					int num = 0;
					int num2 = 0;
					string name = "JRExcharge" + index;
					string roleParamByName = Global.GetRoleParamByName(client, name);
					if (null != roleParamByName)
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
						result = award.DayMaxTimes - num2;
					}
					else
					{
						result = award.DayMaxTimes;
					}
				}
			}
			return result;
		}

		public static int ModifyZiKaTodayLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			string text = "JRExcharge" + index;
			string roleParamByName = Global.GetRoleParamByName(client, text);
			int num = 0;
			int num2 = 0;
			if (null != roleParamByName)
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (2 != array.Length)
				{
					return 0;
				}
				num = Convert.ToInt32(array[0]);
				num2 = Convert.ToInt32(array[1]);
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

		public static string MergeZiKa(GameClient client, int index)
		{
			string text = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, 14);
			string result;
			if (HuodongCachingMgr.GetZiKaTodayLeftMergeNum(client, index) <= 0)
			{
				text = string.Format("{0}:{1}:{2}", -20000, client.ClientData.RoleID, 14);
				result = text;
			}
			else
			{
				JieRiZiKaLiaBaoActivity jieRiZiKaLiaBaoActivity = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
				if (null == jieRiZiKaLiaBaoActivity)
				{
					text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
					result = text;
				}
				else
				{
					JieRiZiKa award = jieRiZiKaLiaBaoActivity.GetAward(index);
					if (null == award)
					{
						text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = text;
					}
					else if (null == award.MyAwardItem)
					{
						text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = text;
					}
					else if (null == award.MyAwardItem.GoodsDataList)
					{
						text = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = text;
					}
					else
					{
						if (null != award.NeedGoodsList)
						{
							for (int i = 0; i < award.NeedGoodsList.Count; i++)
							{
								if (Global.GetTotalGoodsNotUsingCountByID(client, award.NeedGoodsList[i].GoodsID) < award.NeedGoodsList[i].GCount)
								{
									return string.Format("{0}:{1}:{2}", -20003, client.ClientData.RoleID, 14);
								}
							}
						}
						if (award.NeedMoJing > 0)
						{
							if (GameManager.ClientMgr.GetTianDiJingYuanValue(client) < award.NeedMoJing)
							{
								return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 14);
							}
						}
						if (award.NeedQiFuJiFen > 0)
						{
							if (Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen") < award.NeedQiFuJiFen)
							{
								return string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 14);
							}
						}
						if (award.NeedPetJiFen > 0)
						{
						}
						string text2 = "";
						if (award.NeedMoJing > 0)
						{
							int tianDiJingYuanValue = GameManager.ClientMgr.GetTianDiJingYuanValue(client);
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -award.NeedMoJing, "字卡系统兑换物品", false, true, false);
							text2 += EventLogManager.AddResPropString(ResLogType.RongLianZhi, new object[]
							{
								-award.NeedMoJing,
								tianDiJingYuanValue,
								GameManager.ClientMgr.GetTianDiJingYuanValue(client)
							});
						}
						if (award.NeedQiFuJiFen > 0)
						{
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen");
							Global.AddZaJinDanJiFen(client, -award.NeedQiFuJiFen, "字卡系统兑换物品", false);
							text2 += EventLogManager.AddResPropString(ResLogType.QiFuJiFen, new object[]
							{
								-award.NeedQiFuJiFen,
								roleParamsInt32FromDB,
								Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen")
							});
						}
						if (null != award.NeedGoodsList)
						{
							for (int i = 0; i < award.NeedGoodsList.Count; i++)
							{
								bool flag = false;
								bool flag2 = false;
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, award.NeedGoodsList[i].GoodsID, award.NeedGoodsList[i].GCount, false, out flag, out flag2, true))
								{
									return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 14);
								}
								text2 += EventLogManager.AddGoodsDataPropString(award.NeedGoodsList[i]);
							}
						}
						if (!jieRiZiKaLiaBaoActivity.GiveAward(client, index))
						{
							text = string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 14);
							result = text;
						}
						else
						{
							if (text2.Length > 0)
							{
								text2 = text2.Remove(0, 1);
							}
							string strResList = EventLogManager.MakeGoodsDataPropString(award.MyAwardItem.GoodsDataList);
							EventLogManager.AddPurchaseEvent(client, 1, index, text2, strResList);
							int num = Math.Max(0, award.DayMaxTimes - HuodongCachingMgr.ModifyZiKaTodayLeftMergeNum(client, index, 1));
							text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								1,
								client.ClientData.RoleID,
								14,
								num,
								index
							});
							result = text;
						}
					}
				}
			}
			return result;
		}

		public static bool LoadJieriActivitiesConfig()
		{
			JieriActivityConfig jieriActivityConfig = HuodongCachingMgr.GetJieriActivityConfig();
			bool result;
			if (null == jieriActivityConfig)
			{
				result = true;
			}
			else
			{
				string text = "";
				Activity activity = HuodongCachingMgr.GetJieriDaLiBaoActivity();
				if (activity == null || activity.GetParamsValidateCode() < 0)
				{
					text = "节日大礼包活动配置项出错";
				}
				else
				{
					activity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
					if (activity == null || activity.GetParamsValidateCode() < 0)
					{
						text = "节日充值点兑换活动配置项出错";
					}
					else
					{
						activity = HuodongCachingMgr.GetJieRiDengLuActivity();
						if (activity == null || activity.GetParamsValidateCode() < 0)
						{
							text = "节日登录豪礼活动配置项出错";
						}
						else
						{
							activity = HuodongCachingMgr.GetJieriCZSongActivity();
							if (activity == null || activity.GetParamsValidateCode() < 0)
							{
								text = "节日累计充值活动配置项出错";
							}
							else
							{
								activity = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
								if (activity == null || activity.GetParamsValidateCode() < 0)
								{
									text = "节日字卡换礼盒活动配置项出错";
								}
								else
								{
									activity = HuodongCachingMgr.GetJieriXiaoFeiKingActivity();
									if (activity == null || activity.GetParamsValidateCode() < 0)
									{
										text = "节日消费王活动配置项出错";
									}
									else
									{
										activity = HuodongCachingMgr.GetJieRiLeiJiCZActivity();
										if (null == activity)
										{
											text = "节日累计充值活动配置项出错";
										}
										else
										{
											activity = HuodongCachingMgr.GetJieRiTotalConsumeActivity();
											if (null == activity)
											{
												text = "节日累计消费活动配置项出错";
											}
											else
											{
												activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
												if (null == activity)
												{
													text = "节日累计消费活动配置项出错";
												}
												else
												{
													activity = HuodongCachingMgr.GetJieRiCZKingActivity();
													if (null == activity)
													{
														text = "节日累计消费活动配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriGiveActivity() == null)
													{
														text = "节日赠送活动配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriGiveKingActivity() == null)
													{
														text = "节日赠送王配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriRecvKingActivity() == null)
													{
														text = "节日收取王配置项出错";
													}
													else
													{
														activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriWing);
														if (null == activity)
														{
															text = "节日翅膀返利活动配置项出错";
														}
														else
														{
															activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAddon);
															if (null == activity)
															{
																text = "节日节日追加返利活动配置项出错";
															}
															else
															{
																activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriStrengthen);
																if (null == activity)
																{
																	text = "节日强化返利活动配置项出错";
																}
																else
																{
																	activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAchievement);
																	if (null == activity)
																	{
																		text = "节日成就返利活动配置项出错";
																	}
																	else
																	{
																		activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriMilitaryRank);
																		if (null == activity)
																		{
																			text = "节日军衔返利活动配置项出错";
																		}
																		else
																		{
																			activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriVIPFanli);
																			if (null == activity)
																			{
																				text = "节日VIP返利活动配置项出错";
																			}
																			else
																			{
																				activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAmulet);
																				if (null == activity)
																				{
																					text = "节日护身符返利活动配置项出错";
																				}
																				else
																				{
																					activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriArchangel);
																					if (null == activity)
																					{
																						text = "节日大天使返利活动配置项出错";
																					}
																					else
																					{
																						activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriMarriage);
																						if (null == activity)
																						{
																							text = "节日婚姻返利活动配置项出错";
																						}
																						else
																						{
																							activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieRiHuiJi);
																							if (null == activity)
																							{
																								text = "节日徽记返利活动配置项出错";
																							}
																							else
																							{
																								activity = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieRiFuWen);
																								if (null == activity)
																								{
																									text = "节日符文返利活动配置项出错";
																								}
																								else
																								{
																									activity = HuodongCachingMgr.GetJieriLianXuChargeActivity();
																									if (null == activity)
																									{
																										text = "节日连续充值活动配置项出错";
																									}
																									else
																									{
																										activity = HuodongCachingMgr.GetJieriRecvActivity();
																										if (null == activity)
																										{
																											text = "节日收礼活动配置项出错";
																										}
																										else
																										{
																											activity = HuodongCachingMgr.GetJieriPlatChargeKingActivity();
																											if (null == activity)
																											{
																												text = "节日平台充值王活动配置出错";
																											}
																											else
																											{
																												activity = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
																												if (null == activity)
																												{
																													text = "节日每日平台充值王活动配置出错";
																												}
																												else
																												{
																													activity = HuodongCachingMgr.GetJieriFuLiActivity();
																													if (null == activity)
																													{
																														text = "节日福利活动配置出错";
																													}
																													else
																													{
																														activity = HuodongCachingMgr.GetJieRiCZQGActivity();
																														if (null == activity)
																														{
																															text = "节日充值抢购配置出错";
																														}
																														else
																														{
																															activity = HuodongCachingMgr.GetOneDollarBuyActivity();
																															if (null == activity)
																															{
																																text = "1元直购配置出错";
																															}
																															else
																															{
																																activity = HuodongCachingMgr.GetJieriVIPYouHuiAct();
																																if (null == activity)
																																{
																																	text = "节日VIP优惠配置出错";
																																}
																																else
																																{
																																	activity = HuodongCachingMgr.GetDanBiChongZhiActivity();
																																	if (activity == null || activity.GetParamsValidateCode() < 0)
																																	{
																																		text = "单笔充值活动配置项出错";
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					LogManager.WriteLog(1000, text, null, true);
					result = true;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public static int GetThemeActivityState()
		{
			ThemeActivityConfig themeActivityConfig = HuodongCachingMgr.GetThemeActivityConfig();
			int result;
			if (null != themeActivityConfig)
			{
				result = themeActivityConfig.ActivityOpenVavle;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static ThemeActivityConfig GetThemeActivityConfig()
		{
			lock (HuodongCachingMgr._ThemeActivityConfigMutex)
			{
				if (HuodongCachingMgr._ThemeActivityConfig != null)
				{
					return HuodongCachingMgr._ThemeActivityConfig;
				}
			}
			try
			{
				string uri = "Config/ThemeActivityType.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				ThemeActivityConfig themeActivityConfig = new ThemeActivityConfig();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					XElement xelement2;
					int num = Convert.ToInt32(Global.GetSafeAttributeStr(xelement2, "Type"));
					int value = Convert.ToInt32(Global.GetSafeAttributeStr(xelement2, "EndData"));
					string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "PeiZhi");
					themeActivityConfig.ConfigDict[num] = safeAttributeStr;
					themeActivityConfig.EndDataDict[num] = value;
					themeActivityConfig.openList.Add(num);
					safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Name");
					themeActivityConfig.ActivityNameDict[num] = safeAttributeStr;
				}
				uri = "Config/ThemeActivityOpen.xml";
				xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null != xelement)
				{
					XElement xelement2 = xelement.Element("ThemeActivityOpen");
					if (null != xelement2)
					{
						themeActivityConfig.ActivityOpenVavle = Convert.ToInt32(Global.GetSafeAttributeStr(xelement2, "Open"));
					}
				}
				lock (HuodongCachingMgr._ThemeActivityConfigMutex)
				{
					HuodongCachingMgr._ThemeActivityConfig = themeActivityConfig;
				}
				return themeActivityConfig;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/ThemeActivityType.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetThemeActivityConfig()
		{
			string uri = "Config/ThemeActivityType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/ThemeActivityOpen.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._ThemeActivityConfigMutex)
			{
				HuodongCachingMgr._ThemeActivityConfig = null;
			}
			return 0;
		}

		public static JieriActivityConfig GetJieriActivityConfig()
		{
			lock (HuodongCachingMgr._JieriActivityConfigMutex)
			{
				if (HuodongCachingMgr._JieriActivityConfig != null)
				{
					return HuodongCachingMgr._JieriActivityConfig;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/MuJieRiType.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieriActivityConfig jieriActivityConfig = new JieriActivityConfig();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Type"));
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "PeiZhi");
					jieriActivityConfig.ConfigDict[num] = safeAttributeStr;
					jieriActivityConfig.openList.Add(num);
					safeAttributeStr = Global.GetSafeAttributeStr(xml, "Name");
					jieriActivityConfig.ActivityNameDict[num] = safeAttributeStr;
				}
				lock (HuodongCachingMgr._JieriActivityConfigMutex)
				{
					HuodongCachingMgr._JieriActivityConfig = jieriActivityConfig;
				}
				return jieriActivityConfig;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/MuJieRiType.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieriActivityConfig()
		{
			string uri = "Config/JieRiGifts/MuJieRiType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriActivityConfigMutex)
			{
				HuodongCachingMgr._JieriActivityConfig = null;
			}
			return 0;
		}

		public static JieriDaLiBaoActivity GetJieriDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
			{
				if (HuodongCachingMgr._JieriDaLiBaoActivity != null)
				{
					return HuodongCachingMgr._JieriDaLiBaoActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiLiBao.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieriDaLiBaoActivity jieriDaLiBaoActivity = new JieriDaLiBaoActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieriDaLiBaoActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieriDaLiBaoActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieriDaLiBaoActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieriDaLiBaoActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieriDaLiBaoActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				jieriDaLiBaoActivity.MyAwardItem = new AwardItem();
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						jieriDaLiBaoActivity.MyAwardItem.MinAwardCondionValue = 0;
						jieriDaLiBaoActivity.MyAwardItem.AwardYuanBao = 0;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取大型节日礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析大型节日礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								jieriDaLiBaoActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日礼包配置1");
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取大型节日礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析大型节日礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日礼包配置2");
								foreach (GoodsData goodsData in list)
								{
									SystemXmlItem systemXmlItem = null;
									if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
									{
										int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsData.GoodsID);
										AwardItem awardItem = jieriDaLiBaoActivity.GetOccAward(mainOccupationByGoodsID);
										if (null == awardItem)
										{
											awardItem = new AwardItem();
											awardItem.GoodsDataList.Add(goodsData);
											jieriDaLiBaoActivity.OccAwardItemDict[mainOccupationByGoodsID] = awardItem;
										}
										else
										{
											awardItem.GoodsDataList.Add(goodsData);
										}
									}
								}
							}
						}
					}
				}
				jieriDaLiBaoActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
				{
					HuodongCachingMgr._JieriDaLiBaoActivity = jieriDaLiBaoActivity;
				}
				return jieriDaLiBaoActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiLiBao.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieriDaLiBaoActivity()
		{
			string uri = "Config/JieRiGifts/JieRiLiBao.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
			{
				HuodongCachingMgr._JieriDaLiBaoActivity = null;
			}
			return 0;
		}

		public static JieRiDengLuActivity GetJieRiDengLuActivity()
		{
			lock (HuodongCachingMgr._JieriDengLuActivityMutex)
			{
				if (HuodongCachingMgr._JieRiDengLuActivity != null)
				{
					return HuodongCachingMgr._JieRiDengLuActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiDengLu.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiDengLuActivity jieRiDengLuActivity = new JieRiDengLuActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiDengLuActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiDengLuActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiDengLuActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiDengLuActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiDengLuActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							awardItem.MinAwardCondionValue = 0;
							awardItem.AwardYuanBao = 0;
							int num = Convert.ToInt32(Global.GetSafeAttributeStr(xelement3, "TimeOl"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取节日登录有礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日登录有礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "节日登录有礼配置");
								}
							}
							jieRiDengLuActivity.AwardItemDict[num] = awardItem;
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取节日登录有礼活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日登录有礼活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "节日登录有礼配置2");
									foreach (GoodsData goodsData in list)
									{
										SystemXmlItem systemXmlItem = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
										{
											int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsData.GoodsID);
											int key = num * 100 + mainOccupationByGoodsID;
											AwardItem awardItem2 = jieRiDengLuActivity.GetOccAward(key);
											if (null == awardItem2)
											{
												awardItem2 = new AwardItem();
												awardItem2.GoodsDataList.Add(goodsData);
												jieRiDengLuActivity.OccAwardItemDict[key] = awardItem2;
											}
											else
											{
												awardItem2.GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
					}
				}
				jieRiDengLuActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieriDengLuActivityMutex)
				{
					HuodongCachingMgr._JieRiDengLuActivity = jieRiDengLuActivity;
				}
				return jieRiDengLuActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiDengLu.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiDengLuActivity()
		{
			string uri = "Config/JieRiGifts/JieRiDengLu.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriDengLuActivityMutex)
			{
				HuodongCachingMgr._JieRiDengLuActivity = null;
			}
			return 0;
		}

		public static JieriVIPActivity GetJieriVIPActivity()
		{
			lock (HuodongCachingMgr._JieriVIPActivityMutex)
			{
				if (HuodongCachingMgr._JieriVIPActivity != null)
				{
					return HuodongCachingMgr._JieriVIPActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiVip.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieriVIPActivity jieriVIPActivity = new JieriVIPActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieriVIPActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieriVIPActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieriVIPActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieriVIPActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieriVIPActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				jieriVIPActivity.MyAwardItem = new AwardItem();
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						jieriVIPActivity.MyAwardItem.MinAwardCondionValue = 0;
						jieriVIPActivity.MyAwardItem.AwardYuanBao = 0;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取大型节日VIP活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("读取大型节日VIP活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								jieriVIPActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日VIP配置");
							}
						}
					}
				}
				jieriVIPActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieriVIPActivityMutex)
				{
					HuodongCachingMgr._JieriVIPActivity = jieriVIPActivity;
				}
				return jieriVIPActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiVip.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieriVIPActivity()
		{
			string uri = "Config/JieRiGifts/JieRiVip.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriVIPActivityMutex)
			{
				HuodongCachingMgr._JieriVIPActivity = null;
			}
			return 0;
		}

		public static JieriGiveActivity GetJieriGiveActivity()
		{
			lock (HuodongCachingMgr._JieriGiveMutex)
			{
				if (HuodongCachingMgr._JieriGiveActivity != null)
				{
					return HuodongCachingMgr._JieriGiveActivity;
				}
			}
			JieriGiveActivity jieriGiveActivity = new JieriGiveActivity();
			if (jieriGiveActivity.Init())
			{
				lock (HuodongCachingMgr._JieriGiveMutex)
				{
					HuodongCachingMgr._JieriGiveActivity = jieriGiveActivity;
					return jieriGiveActivity;
				}
			}
			return null;
		}

		public static int ResetJieriGiveActivity()
		{
			lock (HuodongCachingMgr._JieriGiveMutex)
			{
				HuodongCachingMgr._JieriGiveActivity = null;
			}
			return 0;
		}

		public static JieriRecvActivity GetJieriRecvActivity()
		{
			lock (HuodongCachingMgr._JieriRecvMutex)
			{
				if (HuodongCachingMgr._JieriRecvActivity != null)
				{
					return HuodongCachingMgr._JieriRecvActivity;
				}
			}
			JieriRecvActivity jieriRecvActivity = new JieriRecvActivity();
			if (jieriRecvActivity.Init())
			{
				lock (HuodongCachingMgr._JieriRecvMutex)
				{
					HuodongCachingMgr._JieriRecvActivity = jieriRecvActivity;
					return jieriRecvActivity;
				}
			}
			return null;
		}

		public static int ResetJieriRecvActivity()
		{
			lock (HuodongCachingMgr._JieriRecvMutex)
			{
				HuodongCachingMgr._JieriRecvActivity = null;
			}
			return 0;
		}

		public static JieRiGiveKingActivity GetJieriGiveKingActivity()
		{
			lock (HuodongCachingMgr._JieriGiveKingMutex)
			{
				if (HuodongCachingMgr._JieriGiveKingActivity != null)
				{
					return HuodongCachingMgr._JieriGiveKingActivity;
				}
			}
			JieRiGiveKingActivity jieRiGiveKingActivity = new JieRiGiveKingActivity();
			if (jieRiGiveKingActivity.Init())
			{
				jieRiGiveKingActivity.LoadRankFromDB();
				lock (HuodongCachingMgr._JieriGiveKingMutex)
				{
					HuodongCachingMgr._JieriGiveKingActivity = jieRiGiveKingActivity;
					return jieRiGiveKingActivity;
				}
			}
			return null;
		}

		public static int ResetJieRiGiveKingActivity()
		{
			lock (HuodongCachingMgr._JieriGiveKingMutex)
			{
				HuodongCachingMgr._JieriGiveKingActivity = null;
			}
			return 0;
		}

		public static JieRiRecvKingActivity GetJieriRecvKingActivity()
		{
			lock (HuodongCachingMgr._JieriRecvKingMutex)
			{
				if (HuodongCachingMgr._JieriRecvKingActivity != null)
				{
					return HuodongCachingMgr._JieriRecvKingActivity;
				}
			}
			JieRiRecvKingActivity jieRiRecvKingActivity = new JieRiRecvKingActivity();
			if (jieRiRecvKingActivity.Init())
			{
				jieRiRecvKingActivity.LoadRankFromDB();
				lock (HuodongCachingMgr._JieriRecvKingMutex)
				{
					HuodongCachingMgr._JieriRecvKingActivity = jieRiRecvKingActivity;
					return jieRiRecvKingActivity;
				}
			}
			return null;
		}

		public static int ResetJieriRecvKingActivity()
		{
			lock (HuodongCachingMgr._JieriRecvKingMutex)
			{
				HuodongCachingMgr._JieriRecvKingActivity = null;
			}
			return 0;
		}

		public static JieRiFuLiActivity GetJieriFuLiActivity()
		{
			lock (HuodongCachingMgr._JieriFuLiMutex)
			{
				if (HuodongCachingMgr._JieriFuLiActivity != null)
				{
					return HuodongCachingMgr._JieriFuLiActivity;
				}
			}
			JieRiFuLiActivity jieRiFuLiActivity = new JieRiFuLiActivity();
			if (jieRiFuLiActivity.Init())
			{
				lock (HuodongCachingMgr._JieriFuLiMutex)
				{
					HuodongCachingMgr._JieriFuLiActivity = jieRiFuLiActivity;
					return jieRiFuLiActivity;
				}
			}
			return null;
		}

		public static int ResetJieriFuLiActivity()
		{
			lock (HuodongCachingMgr._JieriFuLiMutex)
			{
				HuodongCachingMgr._JieriFuLiActivity = null;
			}
			return 0;
		}

		public static int ResetOneDollarChongZhiActivity()
		{
			lock (HuodongCachingMgr._OneDollarChongZhiMutex)
			{
				HuodongCachingMgr._OneDollarChongZhi = null;
			}
			GameManager.ClientMgr.NotifyAllOneDollarChongZhiState();
			return 0;
		}

		public static OneDollarChongZhi GetOneDollarChongZhiActivity()
		{
			lock (HuodongCachingMgr._OneDollarChongZhiMutex)
			{
				if (HuodongCachingMgr._OneDollarChongZhi != null)
				{
					return HuodongCachingMgr._OneDollarChongZhi;
				}
				OneDollarChongZhi oneDollarChongZhi = new OneDollarChongZhi();
				if (oneDollarChongZhi.Init())
				{
					HuodongCachingMgr._OneDollarChongZhi = oneDollarChongZhi;
					return HuodongCachingMgr._OneDollarChongZhi;
				}
			}
			return null;
		}

		public static int ResetInputFanLiNewActivity()
		{
			lock (HuodongCachingMgr._InputFanLiNewMutex)
			{
				HuodongCachingMgr._InputFanLiNew = null;
			}
			GameManager.ClientMgr.NotifyAllInputFanLiNewState();
			return 0;
		}

		public static InputFanLiNew GetInputFanLiNewActivity()
		{
			lock (HuodongCachingMgr._InputFanLiNewMutex)
			{
				if (HuodongCachingMgr._InputFanLiNew != null)
				{
					return HuodongCachingMgr._InputFanLiNew;
				}
				InputFanLiNew inputFanLiNew = new InputFanLiNew();
				if (inputFanLiNew.Init())
				{
					HuodongCachingMgr._InputFanLiNew = inputFanLiNew;
					return HuodongCachingMgr._InputFanLiNew;
				}
			}
			return null;
		}

		public static int ResetRegressActiveOpen()
		{
			lock (HuodongCachingMgr._RegressActiveOpenMutex)
			{
				HuodongCachingMgr._RegressActiveOpen = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveOpenState();
			return 0;
		}

		public static RegressActiveOpen GetRegressActiveOpen()
		{
			lock (HuodongCachingMgr._RegressActiveOpenMutex)
			{
				if (HuodongCachingMgr._RegressActiveOpen != null)
				{
					return HuodongCachingMgr._RegressActiveOpen;
				}
				RegressActiveOpen regressActiveOpen = new RegressActiveOpen();
				if (regressActiveOpen.Init())
				{
					HuodongCachingMgr._RegressActiveOpen = regressActiveOpen;
					return HuodongCachingMgr._RegressActiveOpen;
				}
			}
			return null;
		}

		public static int ResetRegressActiveSignGift()
		{
			lock (HuodongCachingMgr._RegressActiveSignGiftMutex)
			{
				HuodongCachingMgr._RegressActiveSignGift = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveSignGiftState();
			return 0;
		}

		public static RegressActiveSignGift GetRegressActiveSignGift()
		{
			lock (HuodongCachingMgr._RegressActiveSignGiftMutex)
			{
				if (HuodongCachingMgr._RegressActiveSignGift != null)
				{
					return HuodongCachingMgr._RegressActiveSignGift;
				}
				RegressActiveSignGift regressActiveSignGift = new RegressActiveSignGift();
				if (regressActiveSignGift.Init())
				{
					HuodongCachingMgr._RegressActiveSignGift = regressActiveSignGift;
					return HuodongCachingMgr._RegressActiveSignGift;
				}
			}
			return null;
		}

		public static int ResetRegressActiveTotalRecharge()
		{
			lock (HuodongCachingMgr._RegressActiveTotalRechargeMutex)
			{
				HuodongCachingMgr._RegressActiveTotalRecharge = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveTotalRechargeState();
			return 0;
		}

		public static RegressActiveTotalRecharge GetRegressActiveTotalRecharge()
		{
			lock (HuodongCachingMgr._RegressActiveTotalRechargeMutex)
			{
				if (HuodongCachingMgr._RegressActiveTotalRecharge != null)
				{
					return HuodongCachingMgr._RegressActiveTotalRecharge;
				}
				RegressActiveTotalRecharge regressActiveTotalRecharge = new RegressActiveTotalRecharge();
				if (regressActiveTotalRecharge.Init())
				{
					HuodongCachingMgr._RegressActiveTotalRecharge = regressActiveTotalRecharge;
					return HuodongCachingMgr._RegressActiveTotalRecharge;
				}
			}
			return null;
		}

		public static int ResetRegressActiveDayBuy()
		{
			lock (HuodongCachingMgr._RegressActiveDayBuyMutex)
			{
				HuodongCachingMgr._RegressActiveDayBuy = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		public static RegressActiveDayBuy GetRegressActiveDayBuy()
		{
			lock (HuodongCachingMgr._RegressActiveDayBuyMutex)
			{
				if (HuodongCachingMgr._RegressActiveDayBuy != null)
				{
					return HuodongCachingMgr._RegressActiveDayBuy;
				}
				RegressActiveDayBuy regressActiveDayBuy = new RegressActiveDayBuy();
				if (regressActiveDayBuy.Init())
				{
					HuodongCachingMgr._RegressActiveDayBuy = regressActiveDayBuy;
					return HuodongCachingMgr._RegressActiveDayBuy;
				}
			}
			return null;
		}

		public static int ResetRegressActiveStore()
		{
			lock (HuodongCachingMgr._RegressActiveStoreMutex)
			{
				HuodongCachingMgr._RegressActiveStore = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		public static RegressActiveStore GetRegressActiveStore()
		{
			lock (HuodongCachingMgr._RegressActiveStoreMutex)
			{
				if (HuodongCachingMgr._RegressActiveStore != null)
				{
					return HuodongCachingMgr._RegressActiveStore;
				}
				RegressActiveStore regressActiveStore = new RegressActiveStore();
				if (regressActiveStore.Init())
				{
					HuodongCachingMgr._RegressActiveStore = regressActiveStore;
					return HuodongCachingMgr._RegressActiveStore;
				}
			}
			return null;
		}

		public static int ResetJieRiSuperInputFanLiActivity()
		{
			lock (HuodongCachingMgr._JieriSuperInputMutex)
			{
				HuodongCachingMgr._JieriSuperInput = null;
			}
			HuodongCachingMgr.GetJieRiSuperInputActivity();
			return 0;
		}

		public static JieriSuperInputActivity GetJieRiSuperInputActivity()
		{
			lock (HuodongCachingMgr._JieriSuperInputMutex)
			{
				if (HuodongCachingMgr._JieriSuperInput != null)
				{
					return HuodongCachingMgr._JieriSuperInput;
				}
				JieriSuperInputActivity jieriSuperInputActivity = new JieriSuperInputActivity();
				if (jieriSuperInputActivity.Init())
				{
					HuodongCachingMgr._JieriSuperInput = jieriSuperInputActivity;
					return HuodongCachingMgr._JieriSuperInput;
				}
			}
			return null;
		}

		public static int ResetOneDollarBuyActivity()
		{
			lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
			{
				if (null != HuodongCachingMgr._OneDollarBuyActivity)
				{
					HuodongCachingMgr._OneDollarBuyActivity.Dispose();
				}
				HuodongCachingMgr._OneDollarBuyActivity = null;
			}
			HuodongCachingMgr.GetOneDollarBuyActivity();
			return 0;
		}

		public static OneDollarBuyActivity GetOneDollarBuyActivity()
		{
			lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
			{
				if (HuodongCachingMgr._OneDollarBuyActivity != null)
				{
					return HuodongCachingMgr._OneDollarBuyActivity;
				}
			}
			OneDollarBuyActivity oneDollarBuyActivity = new OneDollarBuyActivity();
			if (oneDollarBuyActivity.Init())
			{
				lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
				{
					HuodongCachingMgr._OneDollarBuyActivity = oneDollarBuyActivity;
					return HuodongCachingMgr._OneDollarBuyActivity;
				}
			}
			return null;
		}

		public static int ResetJieRiCZQGActivity()
		{
			lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
			{
				if (null != HuodongCachingMgr._JieRiCZQGActivity)
				{
					HuodongCachingMgr._JieRiCZQGActivity.Dispose();
				}
				HuodongCachingMgr._JieRiCZQGActivity = null;
			}
			return 0;
		}

		public static JieRiCZQGActivity GetJieRiCZQGActivity()
		{
			lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
			{
				if (HuodongCachingMgr._JieRiCZQGActivity != null)
				{
					return HuodongCachingMgr._JieRiCZQGActivity;
				}
			}
			JieRiCZQGActivity jieRiCZQGActivity = new JieRiCZQGActivity();
			if (jieRiCZQGActivity.Init())
			{
				lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
				{
					HuodongCachingMgr._JieRiCZQGActivity = jieRiCZQGActivity;
					return HuodongCachingMgr._JieRiCZQGActivity;
				}
			}
			return null;
		}

		public static int ResetJieriVIPYouHuiAct()
		{
			lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
			{
				HuodongCachingMgr._JieriVIPYouHuiActivity = null;
			}
			return 0;
		}

		public static JieriVIPYouHuiActivity GetJieriVIPYouHuiAct()
		{
			lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
			{
				if (HuodongCachingMgr._JieriVIPYouHuiActivity != null)
				{
					return HuodongCachingMgr._JieriVIPYouHuiActivity;
				}
			}
			JieriVIPYouHuiActivity jieriVIPYouHuiActivity = new JieriVIPYouHuiActivity();
			if (jieriVIPYouHuiActivity.Init())
			{
				lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
				{
					HuodongCachingMgr._JieriVIPYouHuiActivity = jieriVIPYouHuiActivity;
					return HuodongCachingMgr._JieriVIPYouHuiActivity;
				}
			}
			return null;
		}

		public static int ResetSpecialActivity()
		{
			lock (HuodongCachingMgr._SpecialActivityMutex)
			{
				if (null != HuodongCachingMgr._SpecialActivity)
				{
					HuodongCachingMgr._SpecialActivity.Dispose();
				}
				HuodongCachingMgr._SpecialActivity = null;
			}
			GameManager.ClientMgr.ReGenerateSpecActGroup();
			return 0;
		}

		public static SpecialActivity GetSpecialActivity()
		{
			lock (HuodongCachingMgr._SpecialActivityMutex)
			{
				if (HuodongCachingMgr._SpecialActivity != null)
				{
					return HuodongCachingMgr._SpecialActivity;
				}
			}
			SpecialActivity specialActivity = new SpecialActivity();
			if (specialActivity.Init())
			{
				lock (HuodongCachingMgr._SpecialActivityMutex)
				{
					HuodongCachingMgr._SpecialActivity = specialActivity;
					return HuodongCachingMgr._SpecialActivity;
				}
			}
			return null;
		}

		public static int ResetSpecPriorityActivity()
		{
			lock (HuodongCachingMgr._SpecPriorityActivityMutex)
			{
				if (null != HuodongCachingMgr._SpecPriorityActivity)
				{
					HuodongCachingMgr._SpecPriorityActivity.Dispose();
				}
				HuodongCachingMgr._SpecPriorityActivity = null;
			}
			GameManager.ClientMgr.ReGenerateSpecPriorityActGroup();
			return 0;
		}

		public static SpecPriorityActivity GetSpecPriorityActivity()
		{
			lock (HuodongCachingMgr._SpecPriorityActivityMutex)
			{
				if (HuodongCachingMgr._SpecPriorityActivity != null)
				{
					return HuodongCachingMgr._SpecPriorityActivity;
				}
				SpecPriorityActivity specPriorityActivity = new SpecPriorityActivity();
				if (specPriorityActivity.Init())
				{
					HuodongCachingMgr._SpecPriorityActivity = specPriorityActivity;
					return HuodongCachingMgr._SpecPriorityActivity;
				}
			}
			return null;
		}

		public static int ResetThemeDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._ThemeDaLiBaoActivityMutex)
			{
				HuodongCachingMgr._ThemeDaLiBaoActivity = null;
			}
			return 0;
		}

		public static ThemeDaLiBaoActivity GetThemeDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._ThemeDaLiBaoActivityMutex)
			{
				if (HuodongCachingMgr._ThemeDaLiBaoActivity != null)
				{
					return HuodongCachingMgr._ThemeDaLiBaoActivity;
				}
				ThemeDaLiBaoActivity themeDaLiBaoActivity = new ThemeDaLiBaoActivity();
				if (themeDaLiBaoActivity.Init())
				{
					HuodongCachingMgr._ThemeDaLiBaoActivity = themeDaLiBaoActivity;
					return HuodongCachingMgr._ThemeDaLiBaoActivity;
				}
			}
			return null;
		}

		public static int ResetThemeDuiHuanActivity()
		{
			lock (HuodongCachingMgr._ThemeDuiHuanActivityMutex)
			{
				HuodongCachingMgr._ThemeDuiHuanActivity = null;
			}
			return 0;
		}

		public static ThemeDuiHuanActivity GetThemeDuiHuanActivity()
		{
			lock (HuodongCachingMgr._ThemeDuiHuanActivityMutex)
			{
				if (HuodongCachingMgr._ThemeDuiHuanActivity != null)
				{
					return HuodongCachingMgr._ThemeDuiHuanActivity;
				}
				ThemeDuiHuanActivity themeDuiHuanActivity = new ThemeDuiHuanActivity();
				if (themeDuiHuanActivity.Init())
				{
					HuodongCachingMgr._ThemeDuiHuanActivity = themeDuiHuanActivity;
					return HuodongCachingMgr._ThemeDuiHuanActivity;
				}
			}
			return null;
		}

		public static int ResetThemeZhiGouActivity()
		{
			lock (HuodongCachingMgr._ThemeZhiGouActivityMutex)
			{
				if (null != HuodongCachingMgr._ThemeZhiGouActivity)
				{
					HuodongCachingMgr._ThemeZhiGouActivity.Dispose();
				}
				HuodongCachingMgr._ThemeZhiGouActivity = null;
			}
			return 0;
		}

		public static ThemeZhiGouActivity GetThemeZhiGouActivity()
		{
			lock (HuodongCachingMgr._ThemeZhiGouActivityMutex)
			{
				if (HuodongCachingMgr._ThemeZhiGouActivity != null)
				{
					return HuodongCachingMgr._ThemeZhiGouActivity;
				}
				ThemeZhiGouActivity themeZhiGouActivity = new ThemeZhiGouActivity();
				if (themeZhiGouActivity.Init())
				{
					HuodongCachingMgr._ThemeZhiGouActivity = themeZhiGouActivity;
					return HuodongCachingMgr._ThemeZhiGouActivity;
				}
			}
			return null;
		}

		public static int ResetEverydayActivity()
		{
			lock (HuodongCachingMgr._EverydayActivityMutex)
			{
				if (null != HuodongCachingMgr._EverydayActivity)
				{
					HuodongCachingMgr._EverydayActivity.Dispose();
				}
				HuodongCachingMgr._EverydayActivity = null;
			}
			GameManager.ClientMgr.ReGenerateEverydayActGroup();
			return 0;
		}

		public static EverydayActivity GetEverydayActivity()
		{
			lock (HuodongCachingMgr._EverydayActivityMutex)
			{
				if (HuodongCachingMgr._EverydayActivity != null)
				{
					return HuodongCachingMgr._EverydayActivity;
				}
				EverydayActivity everydayActivity = new EverydayActivity();
				if (everydayActivity.Init())
				{
					HuodongCachingMgr._EverydayActivity = everydayActivity;
					return HuodongCachingMgr._EverydayActivity;
				}
			}
			return null;
		}

		public static int ResetWeedEndInputActivity()
		{
			string uri = "Config/Gifts/ZhouMoChongZhiType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			uri = "Config/Gifts/ZhouMoChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._WeedEndInputActivityMutex)
			{
				HuodongCachingMgr._WeedEndInputActivity = null;
			}
			return 0;
		}

		public static WeedEndInputActivity GetWeekEndInputActivity()
		{
			lock (HuodongCachingMgr._WeedEndInputActivityMutex)
			{
				if (HuodongCachingMgr._WeedEndInputActivity != null)
				{
					return HuodongCachingMgr._WeedEndInputActivity;
				}
			}
			WeedEndInputActivity weedEndInputActivity = new WeedEndInputActivity();
			if (weedEndInputActivity.Init())
			{
				lock (HuodongCachingMgr._WeedEndInputActivityMutex)
				{
					HuodongCachingMgr._WeedEndInputActivity = weedEndInputActivity;
					return HuodongCachingMgr._WeedEndInputActivity;
				}
			}
			return null;
		}

		public static int ResetJieriIPointsExchangeActivity()
		{
			string uri = "Config/JieRiGifts/ChongZhiDuiHuan.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
			{
				HuodongCachingMgr._JieriIPointsExchgActivity = null;
			}
			return 0;
		}

		public static JieriIPointsExchgActivity GetJieriIPointsExchgActivity()
		{
			lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
			{
				if (HuodongCachingMgr._JieriIPointsExchgActivity != null)
				{
					return HuodongCachingMgr._JieriIPointsExchgActivity;
				}
			}
			JieriIPointsExchgActivity jieriIPointsExchgActivity = new JieriIPointsExchgActivity();
			if (jieriIPointsExchgActivity.Init())
			{
				lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
				{
					HuodongCachingMgr._JieriIPointsExchgActivity = jieriIPointsExchgActivity;
					return HuodongCachingMgr._JieriIPointsExchgActivity;
				}
			}
			return null;
		}

		public static JieriCZSongActivity GetJieriCZSongActivity()
		{
			lock (HuodongCachingMgr._JieriCZSongActivityMutex)
			{
				if (HuodongCachingMgr._JieriCZSongActivity != null)
				{
					return HuodongCachingMgr._JieriCZSongActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiDayChongZhi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieriCZSongActivity jieriCZSongActivity = new JieriCZSongActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieriCZSongActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieriCZSongActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieriCZSongActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieriCZSongActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieriCZSongActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							int num = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值送配置1");
								}
							}
							jieriCZSongActivity.AwardItemDict[num] = awardItem;
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值送配置2");
									foreach (GoodsData goodsData in list)
									{
										SystemXmlItem systemXmlItem = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
										{
											int num2 = num;
											AwardItem awardItem2 = jieriCZSongActivity.GetOccAward(num2);
											if (null == awardItem2)
											{
												awardItem2 = new AwardItem();
												awardItem2.GoodsDataList.Add(goodsData);
												awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
												jieriCZSongActivity.OccAwardItemDict[num2] = awardItem2;
											}
											else
											{
												awardItem2.GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
					}
				}
				jieriCZSongActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieriCZSongActivityMutex)
				{
					HuodongCachingMgr._JieriCZSongActivity = jieriCZSongActivity;
				}
				return jieriCZSongActivity;
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static int ResetJieriCZSongActivity()
		{
			string uri = "Config/JieRiGifts/JieRiDayChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriCZSongActivityMutex)
			{
				HuodongCachingMgr._JieriCZSongActivity = null;
			}
			return 0;
		}

		public static JieRiLeiJiCZActivity GetJieRiLeiJiCZActivity()
		{
			lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
			{
				if (HuodongCachingMgr._JieRiLeiJiCZActivity != null)
				{
					return HuodongCachingMgr._JieRiLeiJiCZActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiLeiJi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiLeiJiCZActivity jieRiLeiJiCZActivity = new JieRiLeiJiCZActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiLeiJiCZActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiLeiJiCZActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiLeiJiCZActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiLeiJiCZActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiLeiJiCZActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							int num = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取节日累计充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日累计充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "节日累计充值配置1");
								}
							}
							jieRiLeiJiCZActivity.AwardItemDict[num] = awardItem;
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日累计充值活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "节日累计充值配置2");
									foreach (GoodsData goodsData in list)
									{
										SystemXmlItem systemXmlItem = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
										{
											int num2 = num;
											AwardItem awardItem2 = jieRiLeiJiCZActivity.GetOccAward(num2);
											if (null == awardItem2)
											{
												awardItem2 = new AwardItem();
												awardItem2.GoodsDataList.Add(goodsData);
												awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
												jieRiLeiJiCZActivity.OccAwardItemDict[num2] = awardItem2;
											}
											else
											{
												awardItem2.GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
					}
				}
				jieRiLeiJiCZActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
				{
					HuodongCachingMgr._JieRiLeiJiCZActivity = jieRiLeiJiCZActivity;
				}
				return jieRiLeiJiCZActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiLeiJi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiLeiJiCZActivity()
		{
			string uri = "Config/JieRiGifts/JieRiLeiJi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
			{
				HuodongCachingMgr._JieRiLeiJiCZActivity = null;
			}
			return 0;
		}

		public static JieRiMeiRiLeiJiActivity GetJieriMeiRiLeiJiActivity()
		{
			lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
			{
				if (HuodongCachingMgr._JieRiMeiRiLeiJiActivity != null)
				{
					return HuodongCachingMgr._JieRiMeiRiLeiJiActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiMeiRiLeiJi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiMeiRiLeiJiActivity jieRiMeiRiLeiJiActivity = new JieRiMeiRiLeiJiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiMeiRiLeiJiActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiMeiRiLeiJiActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiMeiRiLeiJiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiMeiRiLeiJiActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiMeiRiLeiJiActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							int key = (int)Global.GetSafeAttributeLong(xelement3, "Day");
							int num = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值送配置1");
								}
							}
							if (!jieRiMeiRiLeiJiActivity.DayAwardItemDict.ContainsKey(key))
							{
								jieRiMeiRiLeiJiActivity.DayAwardItemDict[key] = new List<AwardItem>();
							}
							jieRiMeiRiLeiJiActivity.DayAwardItemDict[key].Add(awardItem);
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值送配置2");
									foreach (GoodsData goodsData in list)
									{
										SystemXmlItem systemXmlItem = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
										{
											int num2 = num;
											AwardItem awardItem2 = jieRiMeiRiLeiJiActivity.GetOccAward(num2);
											if (null == awardItem2)
											{
												awardItem2 = new AwardItem();
												awardItem2.GoodsDataList.Add(goodsData);
												awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
												if (!jieRiMeiRiLeiJiActivity.DayOccAwardItemDict.ContainsKey(key))
												{
													jieRiMeiRiLeiJiActivity.DayOccAwardItemDict[key] = new Dictionary<int, AwardItem>();
												}
												jieRiMeiRiLeiJiActivity.DayOccAwardItemDict[key][num2] = awardItem2;
											}
											else
											{
												awardItem2.GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
					}
				}
				jieRiMeiRiLeiJiActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
				{
					HuodongCachingMgr._JieRiMeiRiLeiJiActivity = jieRiMeiRiLeiJiActivity;
				}
				return jieRiMeiRiLeiJiActivity;
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static int ResetJieRiMeiRiLeiJiActivity()
		{
			string uri = "Config/JieRiGifts/JieRiMeiRiLeiJi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
			{
				HuodongCachingMgr._JieRiMeiRiLeiJiActivity = null;
			}
			return 0;
		}

		public static JieRiTotalConsumeActivity GetJieRiTotalConsumeActivity()
		{
			lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
			{
				if (HuodongCachingMgr._JieRiTotalConsumeActivity != null)
				{
					return HuodongCachingMgr._JieRiTotalConsumeActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiTotalConsumeActivity jieRiTotalConsumeActivity = new JieRiTotalConsumeActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiTotalConsumeActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiTotalConsumeActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiTotalConsumeActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiTotalConsumeActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiTotalConsumeActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							int num = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取节日累计消费活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日累计消费活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "节日累计消费配置1");
								}
							}
							jieRiTotalConsumeActivity.AwardItemDict[num] = awardItem;
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析节日累计消费活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "节日累计消费配置2");
									foreach (GoodsData goodsData in list)
									{
										SystemXmlItem systemXmlItem = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
										{
											int num2 = num;
											AwardItem awardItem2 = jieRiTotalConsumeActivity.GetOccAward(num2);
											if (null == awardItem2)
											{
												awardItem2 = new AwardItem();
												awardItem2.GoodsDataList.Add(goodsData);
												awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
												jieRiTotalConsumeActivity.OccAwardItemDict[num2] = awardItem2;
											}
											else
											{
												awardItem2.GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
					}
				}
				jieRiTotalConsumeActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
				{
					HuodongCachingMgr._JieRiTotalConsumeActivity = jieRiTotalConsumeActivity;
				}
				return jieRiTotalConsumeActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiTotalConsumeActivity()
		{
			string uri = "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
			{
				HuodongCachingMgr._JieRiTotalConsumeActivity = null;
			}
			return 0;
		}

		public static JieRiMultAwardActivity GetJieRiMultAwardActivity()
		{
			lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
			{
				if (HuodongCachingMgr._JieRiMultAwardActivity != null)
				{
					return HuodongCachingMgr._JieRiMultAwardActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiDuoBei.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiMultAwardActivity jieRiMultAwardActivity = new JieRiMultAwardActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiMultAwardActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiMultAwardActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiMultAwardActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiMultAwardActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiMultAwardActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							JieRiMultConfig jieRiMultConfig = new JieRiMultConfig();
							jieRiMultConfig.index = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							jieRiMultConfig.type = (int)Global.GetSafeAttributeLong(xelement3, "TypeID");
							jieRiMultConfig.Multiplying = Global.GetSafeAttributeDouble(xelement3, "Multiplying");
							jieRiMultConfig.Effective = (int)Global.GetSafeAttributeLong(xelement3, "Effective");
							jieRiMultConfig.StartDate = Global.GetSafeAttributeStr(xelement3, "AwardStartDate");
							jieRiMultConfig.EndDate = Global.GetSafeAttributeStr(xelement3, "AwardEndDate");
							jieRiMultAwardActivity.activityDict[jieRiMultConfig.type] = jieRiMultConfig;
						}
					}
				}
				lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
				{
					HuodongCachingMgr._JieRiMultAwardActivity = jieRiMultAwardActivity;
				}
				return jieRiMultAwardActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiDuoBei.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiMultAwardActivity()
		{
			string uri = "Config/JieRiGifts/JieRiDuoBei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
			{
				HuodongCachingMgr._JieRiMultAwardActivity = null;
			}
			return 0;
		}

		public static int ResetJieRiFanLiAwardActivity()
		{
			string uri = "Config/JieRiGifts/WingFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/ZhuiJiaFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/QiangHuaFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/ChengJiuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/JunXianFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/VIPFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/HuShenFuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/DaTianShiFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/HunYinFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/JieRiHuiJiFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			uri = "Config/JieRiGifts/JieRiFuWenFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieriWingFanliActMutex)
			{
				for (int i = 0; i < HuodongCachingMgr._JieriWingFanliAct.Length; i++)
				{
					HuodongCachingMgr._JieriWingFanliAct[i] = null;
				}
			}
			return 0;
		}

		public static JieRiZiKaLiaBaoActivity GetJieRiZiKaLiaBaoActivity()
		{
			lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
			{
				if (HuodongCachingMgr._JieRiZiKaLiaBaoActivity != null)
				{
					return HuodongCachingMgr._JieRiZiKaLiaBaoActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiBaoXiang.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				JieRiZiKaLiaBaoActivity jieRiZiKaLiaBaoActivity = new JieRiZiKaLiaBaoActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					jieRiZiKaLiaBaoActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					jieRiZiKaLiaBaoActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					jieRiZiKaLiaBaoActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					jieRiZiKaLiaBaoActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					jieRiZiKaLiaBaoActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							JieRiZiKa jieRiZiKa = new JieRiZiKa();
							jieRiZiKa.id = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							jieRiZiKa.type = (int)Global.GetSafeAttributeLong(xelement3, "Type");
							jieRiZiKa.NeedMoJing = (int)Global.GetSafeAttributeLong(xelement3, "MoJing");
							jieRiZiKa.NeedQiFuJiFen = (int)Global.GetSafeAttributeLong(xelement3, "JiFen");
							jieRiZiKa.DayMaxTimes = (int)Global.GetSafeAttributeLong(xelement3, "DayMaxTimes");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "DuiHuanGoodsIDs");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日字卡换礼盒活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									jieRiZiKa.NeedGoodsList = HuodongCachingMgr.ParseGoodsDataList2(array, "大型节日字卡换礼盒配置1");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "NewGoodsID");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日字卡换礼盒活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取大型节日字卡换礼盒活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									jieRiZiKa.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日字卡换礼盒合成配置2");
								}
							}
							jieRiZiKaLiaBaoActivity.JieRiZiKaDict[jieRiZiKa.id] = jieRiZiKa;
						}
					}
				}
				jieRiZiKaLiaBaoActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
				{
					HuodongCachingMgr._JieRiZiKaLiaBaoActivity = jieRiZiKaLiaBaoActivity;
				}
				return jieRiZiKaLiaBaoActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiBaoXiang.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiZiKaLiaBaoActivity()
		{
			string uri = "Config/JieRiGifts/JieRiBaoXiang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
			{
				HuodongCachingMgr._JieRiZiKaLiaBaoActivity = null;
			}
			return 0;
		}

		public static KingActivity GetJieriXiaoFeiKingActivity()
		{
			lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
			{
				if (HuodongCachingMgr._JieRiXiaoFeiKingActivity != null)
				{
					return HuodongCachingMgr._JieRiXiaoFeiKingActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiXiaoFeiKing.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					kingActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					kingActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日消费王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取大型节日消费王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日消费王活动配置");
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
									LogManager.WriteLog(1, string.Format("读取大型节日消费王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日消费王活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "ID");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'-'
							});
							if (array2.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array2[0]);
								int num2 = Global.SafeConvertToInt32(array2[array2.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									kingActivity.AwardDict.Add(i, awardItem);
									kingActivity.AwardDict2.Add(i, awardItem2);
								}
							}
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
				{
					HuodongCachingMgr._JieRiXiaoFeiKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiXiaoFeiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiXiaoFeiKingActivity()
		{
			string uri = "Config/JieRiGifts/JieRiXiaoFeiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
			{
				HuodongCachingMgr._JieRiXiaoFeiKingActivity = null;
			}
			return 0;
		}

		public static KingActivity GetJieRiCZKingActivity()
		{
			lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
			{
				if (HuodongCachingMgr._JieRiCZKingActivity != null)
				{
					return HuodongCachingMgr._JieRiCZKingActivity;
				}
			}
			try
			{
				string uri = "Config/JieRiGifts/JieRiChongZhiKing.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				KingActivity kingActivity = new KingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					kingActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					kingActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					kingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					kingActivity.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					kingActivity.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
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
							int key = Convert.ToInt32(Global.GetSafeAttributeStr(xelement3, "ID"));
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值王活动配置1");
								}
							}
							kingActivity.AwardDict.Add(key, awardItem);
							AwardItem awardItem2 = new AwardItem();
							awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem2.AwardYuanBao = 0;
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值王活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值王活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日充值王活动配置2");
								}
							}
							kingActivity.AwardDict2.Add(key, awardItem2);
						}
					}
				}
				kingActivity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
				{
					HuodongCachingMgr._JieRiCZKingActivity = kingActivity;
				}
				return kingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiChongZhiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetJieRiCZKingActivity()
		{
			string uri = "Config/JieRiGifts/JieRiChongZhiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
			{
				HuodongCachingMgr._JieRiCZKingActivity = null;
			}
			return 0;
		}

		public static bool LoadHeFuActivitiesConfig()
		{
			string text = "";
			Activity activity = HuodongCachingMgr.GetHeFuLoginActivity();
			if (activity == null || activity.GetParamsValidateCode() < 0)
			{
				text = "合服大礼包活动配置项出错";
			}
			else
			{
				activity = HuodongCachingMgr.GetHeFuTotalLoginActivity();
				if (activity == null || activity.GetParamsValidateCode() < 0)
				{
					text = "合服累计登陆活动配置项出错";
				}
				else
				{
					activity = HuodongCachingMgr.GetHeFuPKKingActivity();
					if (activity == null || activity.GetParamsValidateCode() < 0)
					{
						text = "合服PK王活动配置项出错";
					}
					else
					{
						activity = HuodongCachingMgr.GetHeFuWCKingActivity();
						if (activity == null || activity.GetParamsValidateCode() < 0)
						{
							text = "合服王城霸主活动配置项出错";
						}
						else
						{
							activity = HuodongCachingMgr.GetHeFuRechargeActivity();
							if (activity == null || activity.GetParamsValidateCode() < 0)
							{
								text = "合服充值返利活动配置项出错";
							}
							else
							{
								activity = HuodongCachingMgr.GetXinFanLiActivity();
								if (activity == null || activity.GetParamsValidateCode() < 0)
								{
									text = "新的新区返利活动配置项出错";
								}
								else
								{
									activity = HuodongCachingMgr.GetHeFuLuoLanActivity();
									if (null == activity)
									{
										text = "合服罗兰城主活动配置项出错";
									}
								}
							}
						}
					}
				}
			}
			bool result;
			if (!string.IsNullOrEmpty(text))
			{
				LogManager.WriteLog(1000, text, null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static HeFuActivityConfig GetHeFuActivityConfing()
		{
			lock (HuodongCachingMgr._HeFuActivityConfigMutex)
			{
				if (HuodongCachingMgr._HeFuActivityConfig != null)
				{
					return HuodongCachingMgr._HeFuActivityConfig;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuType.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuActivityConfig heFuActivityConfig = new HeFuActivityConfig();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int item = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					heFuActivityConfig.openList.Add(item);
				}
				lock (HuodongCachingMgr._HeFuActivityConfigMutex)
				{
					HuodongCachingMgr._HeFuActivityConfig = heFuActivityConfig;
				}
				return heFuActivityConfig;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuType.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuActivityConfig()
		{
			string uri = "Config/HeFuGifts/HeFuType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuActivityConfigMutex)
			{
				HuodongCachingMgr._HeFuActivityConfig = null;
			}
			return 0;
		}

		public static HeFuLoginActivity GetHeFuLoginActivity()
		{
			lock (HuodongCachingMgr._HeFuLoginActivityMutex)
			{
				if (HuodongCachingMgr._HeFuLoginActivity != null)
				{
					return HuodongCachingMgr._HeFuLoginActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuLiBao.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuLoginActivity heFuLoginActivity = new HeFuLoginActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuLoginActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("Time");
				int num = 7;
				int num2 = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", num2.ToString()));
				}
				heFuLoginActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuLoginActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuLoginActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuLoginActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(num2 - 1, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						AwardItem awardItem = new AwardItem();
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取Config/HeFuGifts/HeFuLiBao.xml的普通奖励失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析Config/HeFuGifts/HeFuLiBao.xml的普通奖励失败", new object[0]), null, true);
							}
							else
							{
								awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型合服礼包配置");
							}
						}
						heFuLoginActivity.AwardDict[1] = awardItem;
						AwardItem awardItem2 = new AwardItem();
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "VIPGoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr2))
						{
							LogManager.WriteLog(1, string.Format("读取Config/HeFuGifts/HeFuLiBao.xml的VIP奖励失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析Config/HeFuGifts/HeFuLiBao.xml的VIP奖励失败", new object[0]), null, true);
							}
							else
							{
								awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型合服礼包配置");
							}
						}
						heFuLoginActivity.AwardDict[2] = awardItem2;
					}
				}
				lock (HuodongCachingMgr._HeFuLoginActivityMutex)
				{
					HuodongCachingMgr._HeFuLoginActivity = heFuLoginActivity;
				}
				return heFuLoginActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuLiBao.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuLoginActivity()
		{
			string uri = "Config/HeFuGifts/HeFuLiBao.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuLoginActivityMutex)
			{
				HuodongCachingMgr._HeFuLoginActivity = null;
			}
			return 0;
		}

		public static HeFuTotalLoginActivity GetHeFuTotalLoginActivity()
		{
			lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
			{
				if (HuodongCachingMgr._HeFuTotalLoginActivity != null)
				{
					return HuodongCachingMgr._HeFuTotalLoginActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuDengLu.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuTotalLoginActivity heFuTotalLoginActivity = new HeFuTotalLoginActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuTotalLoginActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("Time");
				int num = 7;
				int num2 = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", num2.ToString()));
				}
				heFuTotalLoginActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuTotalLoginActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuTotalLoginActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuTotalLoginActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(num2 - 1, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xml in enumerable)
					{
						int key = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "TimeOl"));
						AwardItem awardItem = new AwardItem();
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取合服累计登陆配置文件中的GoodsIDs失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析合服累计登陆配置文件中的GoodsIDs失败", new object[0]), null, true);
							}
							else
							{
								awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "合服累计登陆配置");
							}
						}
						heFuTotalLoginActivity.AwardDict[key] = awardItem;
					}
				}
				lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
				{
					HuodongCachingMgr._HeFuTotalLoginActivity = heFuTotalLoginActivity;
				}
				return heFuTotalLoginActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuDengLu.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuTotalLoginActivity()
		{
			string uri = "Config/HeFuGifts/HeFuDengLu.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
			{
				HuodongCachingMgr._HeFuTotalLoginActivity = null;
			}
			return 0;
		}

		public static int GetHeFuPKKingRoleID()
		{
			HeFuPKKingActivity heFuPKKingActivity = HuodongCachingMgr.GetHeFuPKKingActivity();
			int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkking", 0);
			int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingnum", 0);
			int result;
			if (heFuPKKingActivity != null && !heFuPKKingActivity.InActivityTime() && !heFuPKKingActivity.InAwardTime())
			{
				result = 0;
			}
			else if (gameConfigItemInt > 0 && gameConfigItemInt2 >= heFuPKKingActivity.winerCount)
			{
				result = gameConfigItemInt;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static void UpdateHeFuPKKingRoleID(int roleID)
		{
			HeFuPKKingActivity heFuPKKingActivity = HuodongCachingMgr.GetHeFuPKKingActivity();
			if (heFuPKKingActivity == null || heFuPKKingActivity.InActivityTime())
			{
				int num = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkking", 0);
				int num2 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingdayid", 0);
				int num3 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingnum", 0);
				if (0 >= HuodongCachingMgr.GetHeFuPKKingRoleID())
				{
					int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (roleID != num || offsetDay != num2 + 1)
					{
						num = roleID;
						num2 = offsetDay;
						num3 = 1;
					}
					else
					{
						num = roleID;
						num2 = offsetDay;
						num3++;
					}
					Global.UpdateDBGameConfigg("hefupkking", num.ToString());
					Global.UpdateDBGameConfigg("hefupkkingdayid", num2.ToString());
					Global.UpdateDBGameConfigg("hefupkkingnum", num3.ToString());
				}
			}
		}

		public static HeFuPKKingActivity GetHeFuPKKingActivity()
		{
			lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
			{
				if (HuodongCachingMgr._HeFuPKKingActivity != null)
				{
					return HuodongCachingMgr._HeFuPKKingActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/PKJiangLi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuPKKingActivity heFuPKKingActivity = new HeFuPKKingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuPKKingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					heFuPKKingActivity.winerCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "WinerCount", "3"));
				}
				xelement2 = xelement.Element("Time");
				int num = 5;
				int num2 = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", num2.ToString()));
				}
				heFuPKKingActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuPKKingActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuPKKingActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(heFuPKKingActivity.winerCount, 0, 0, 0);
				heFuPKKingActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(num2 - 1, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDOne");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
							}
							else
							{
								heFuPKKingActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "合服战场之神配置");
							}
						}
					}
				}
				lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
				{
					HuodongCachingMgr._HeFuPKKingActivity = heFuPKKingActivity;
				}
				return heFuPKKingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/PKJiangLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuPKKingActivity()
		{
			string uri = "Config/HeFuGifts/PKJiangLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
			{
				HuodongCachingMgr._HeFuPKKingActivity = null;
			}
			return 0;
		}

		public static HeFuLuoLanActivity GetHeFuLuoLanActivity()
		{
			lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
			{
				if (HuodongCachingMgr._HeFuLuoLanActivity != null)
				{
					return HuodongCachingMgr._HeFuLuoLanActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuLuoLan.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuLuoLanActivity heFuLuoLanActivity = new HeFuLuoLanActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuLuoLanActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("Time");
				int num = 7;
				int num2 = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", num2.ToString()));
				}
				heFuLuoLanActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuLuoLanActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuLuoLanActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuLuoLanActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(num2 - 1, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							HeFuLuoLanAward heFuLuoLanAward = new HeFuLuoLanAward();
							int key = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "ID"));
							heFuLuoLanAward.winNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "WinNum"));
							heFuLuoLanAward.status = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Status"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取合服罗兰城主配置GoodsOne失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析合服罗兰城主配置GoodsOne失败", new object[0]), null, true);
								}
								else
								{
									heFuLuoLanAward.awardData.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "合服罗兰城主配置");
								}
							}
							heFuLuoLanActivity.HeFuLuoLanAwardDict[key] = heFuLuoLanAward;
						}
					}
				}
				lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
				{
					HuodongCachingMgr._HeFuLuoLanActivity = heFuLuoLanActivity;
				}
				return heFuLuoLanActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuLuoLan.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuLuoLanActivity()
		{
			string uri = "Config/HeFuGifts/HeFuLuoLan.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
			{
				HuodongCachingMgr._HeFuLuoLanActivity = null;
			}
			return 0;
		}

		public static HeFuAwardTimesActivity GetHeFuAwardTimesActivity()
		{
			lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
			{
				if (HuodongCachingMgr._HeFuAwardTimeActivity != null)
				{
					return HuodongCachingMgr._HeFuAwardTimeActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuZhangChang.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuAwardTimesActivity heFuAwardTimesActivity = new HeFuAwardTimesActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuAwardTimesActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("Time");
				int num = 7;
				int num2 = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", num2.ToString()));
				}
				heFuAwardTimesActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuAwardTimesActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuAwardTimesActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuAwardTimesActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(num2 - 1, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "ActivitiesIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取合服为战而生配置ActivitiesIDs失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < array.Length; i++)
								{
									heFuAwardTimesActivity.activityList.Add(Convert.ToInt32(array[i]));
								}
							}
						}
						heFuAwardTimesActivity.activityTimes = (float)Convert.ToDouble(Global.GetDefAttributeStr(xelement3, "Override", "2"));
						heFuAwardTimesActivity.specialTimeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement3, "SpecialTimeID", "0"));
					}
				}
				lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
				{
					HuodongCachingMgr._HeFuAwardTimeActivity = heFuAwardTimesActivity;
				}
				return heFuAwardTimesActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuZhangChang.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuAwardTimeActivity()
		{
			string uri = "Config/HeFuGifts/HeFuZhangChang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
			{
				HuodongCachingMgr._HeFuAwardTimeActivity = null;
			}
			return 0;
		}

		public static int GetHeFuWCKingBHID()
		{
			HeFuWCKingActivity heFuWCKingActivity = HuodongCachingMgr.GetHeFuWCKingActivity();
			DateTime t = DateTime.Parse(heFuWCKingActivity.AwardStartDate);
			DateTime t2 = DateTime.Parse(heFuWCKingActivity.AwardEndDate);
			int result;
			if (TimeUtil.NowDateTime() >= t && TimeUtil.NowDateTime() <= t2)
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwcking", 0);
				int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingnum", 0);
				if (gameConfigItemInt2 >= 3)
				{
					result = gameConfigItemInt;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static void UpdateHeFuWCKingBHID(int bhid)
		{
			HeFuWCKingActivity heFuWCKingActivity = HuodongCachingMgr.GetHeFuWCKingActivity();
			DateTime t = DateTime.Parse(heFuWCKingActivity.FromDate);
			DateTime t2 = DateTime.Parse(heFuWCKingActivity.ToDate);
			if (TimeUtil.NowDateTime() >= t && TimeUtil.NowDateTime() <= t2)
			{
				int num = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwcking", 0);
				int num2 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingdayid", 0);
				int num3 = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingnum", 0);
				if (num3 < 3)
				{
					int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
					if (dayOfYear != num2)
					{
						if (num != bhid)
						{
							num = bhid;
							num2 = dayOfYear;
							num3 = 1;
						}
						else
						{
							if (num2 == dayOfYear - 1 || (dayOfYear == 1 && num2 >= 365))
							{
								num3++;
							}
							else
							{
								num3 = 1;
							}
							num = bhid;
							num2 = dayOfYear;
						}
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwcking", num.ToString(), false);
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingdayid", num2.ToString(), false);
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingnum", num3.ToString(), false);
					}
				}
			}
			else
			{
				GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingnum", "0", false);
			}
		}

		public static HeFuWCKingActivity GetHeFuWCKingActivity()
		{
			lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
			{
				if (HuodongCachingMgr._HeFuWCKingActivity != null)
				{
					return HuodongCachingMgr._HeFuWCKingActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/WangChengJiangLi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuWCKingActivity heFuWCKingActivity = new HeFuWCKingActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuWCKingActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
					heFuWCKingActivity.ToDate = Global.GetHuoDongTimeByHeFu(4, 23, 59, 59);
					heFuWCKingActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					heFuWCKingActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
					heFuWCKingActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(5, 23, 59, 59);
				}
				heFuWCKingActivity.MyAwardItem = new AwardItem();
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					XElement xelement3 = xelement2.Element("Award");
					if (null != xelement3)
					{
						heFuWCKingActivity.MyAwardItem.MinAwardCondionValue = 0;
						heFuWCKingActivity.MyAwardItem.AwardYuanBao = 0;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取大型合服王城争霸活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("读取大型合服王城争霸活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								heFuWCKingActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型合服王城争霸配置");
							}
						}
					}
				}
				heFuWCKingActivity.PredealDateTime();
				lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
				{
					HuodongCachingMgr._HeFuWCKingActivity = heFuWCKingActivity;
				}
				return heFuWCKingActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/WangChengJiangLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuWCKingActivity()
		{
			string uri = "Config/HeFuGifts/WangChengJiangLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
			{
				HuodongCachingMgr._HeFuWCKingActivity = null;
			}
			return 0;
		}

		public static HeFuRechargeActivity GetHeFuRechargeActivity()
		{
			lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
			{
				if (HuodongCachingMgr._HeFuRechargeActivity != null)
				{
					return HuodongCachingMgr._HeFuRechargeActivity;
				}
			}
			try
			{
				string uri = "Config/HeFuGifts/HeFuFanLi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				HeFuRechargeActivity heFuRechargeActivity = new HeFuRechargeActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					heFuRechargeActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("Time");
				int num = 7;
				int addDays = 7;
				if (null != xelement2)
				{
					num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Activity", num.ToString()));
					addDays = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Award", addDays.ToString()));
				}
				heFuRechargeActivity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				heFuRechargeActivity.ToDate = Global.GetHuoDongTimeByHeFu(num - 1, 23, 59, 59);
				heFuRechargeActivity.AwardStartDate = Global.GetHuoDongTimeByHeFu(1, 0, 0, 0);
				heFuRechargeActivity.AwardEndDate = Global.GetHuoDongTimeByHeFu(addDays, 23, 59, 59);
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							int num2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Level"));
							HeFuRechargeData heFuRechargeData = new HeFuRechargeData();
							string defAttributeStr = Global.GetDefAttributeStr(xelement3, "FanLi", "0.0");
							heFuRechargeData.Coe = (float)Convert.ToDouble(defAttributeStr);
							heFuRechargeData.LowLimit = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							heFuRechargeActivity.ConfigDict[num2] = heFuRechargeData;
							HeFuRechargeActivity heFuRechargeActivity2 = heFuRechargeActivity;
							heFuRechargeActivity2.strcoe += num2;
							HeFuRechargeActivity heFuRechargeActivity3 = heFuRechargeActivity;
							heFuRechargeActivity3.strcoe += ",";
							HeFuRechargeActivity heFuRechargeActivity4 = heFuRechargeActivity;
							heFuRechargeActivity4.strcoe += heFuRechargeData.Coe;
							HeFuRechargeActivity heFuRechargeActivity5 = heFuRechargeActivity;
							heFuRechargeActivity5.strcoe += "|";
						}
					}
				}
				lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
				{
					HuodongCachingMgr._HeFuRechargeActivity = heFuRechargeActivity;
				}
				return heFuRechargeActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/HeFuGifts/HeFuFanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetHeFuRechargeActivity()
		{
			string uri = "Config/HeFuGifts/HeFuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
			{
				HuodongCachingMgr._HeFuRechargeActivity = null;
			}
			return 0;
		}

		public static XinFanLiActivity GetXinFanLiActivity()
		{
			lock (HuodongCachingMgr._XinFanLiActivityMutex)
			{
				if (HuodongCachingMgr._XinFanLiActivity != null)
				{
					return HuodongCachingMgr._XinFanLiActivity;
				}
			}
			try
			{
				string uri = "Config/XinFuGifts/MuFanLi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				XinFanLiActivity xinFanLiActivity = new XinFanLiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					xinFanLiActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					xinFanLiActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					xinFanLiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					xinFanLiActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(1, 0, 0, 0);
					xinFanLiActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(7, 23, 59, 59);
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)(Global.GetSafeAttributeDouble(xelement3, "FanLi") * 100.0));
							awardItem.AwardYuanBao = 0;
							awardItem.GoodsDataList = new List<GoodsData>();
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "ID");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'-'
							});
							if (array.Length > 0)
							{
								int num = Global.SafeConvertToInt32(array[0]);
								int num2 = Global.SafeConvertToInt32(array[array.Length - 1]);
								for (int i = num; i <= num2; i++)
								{
									xinFanLiActivity.AwardDict.Add(i, awardItem);
								}
							}
						}
					}
				}
				xinFanLiActivity.PredealDateTime();
				lock (HuodongCachingMgr._XinFanLiActivityMutex)
				{
					HuodongCachingMgr._XinFanLiActivity = xinFanLiActivity;
				}
				return xinFanLiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/XinFuGifts/MuFanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetXinFanLiActivity()
		{
			string uri = "Config/Gifts/XinFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._XinFanLiActivityMutex)
			{
				HuodongCachingMgr._XinFanLiActivity = null;
			}
			return 0;
		}

		public static MeiRiChongZhiActivity GetMeiRiChongZhiActivity()
		{
			lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._MeiRiChongZhiHaoLiActivity != null)
				{
					return HuodongCachingMgr._MeiRiChongZhiHaoLiActivity;
				}
			}
			try
			{
				string uri = "Config/Gifts/DayChongZhi.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				MeiRiChongZhiActivity meiRiChongZhiActivity = new MeiRiChongZhiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					meiRiChongZhiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
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
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao"));
							awardItem.GoodsDataList = new List<GoodsData>();
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取每日充值豪礼活动配置文件中的物品配置1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取每日充值豪礼活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "每日充值豪礼活动");
								}
							}
							int key = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							meiRiChongZhiActivity.AwardDict.Add(key, awardItem);
						}
					}
				}
				meiRiChongZhiActivity.PredealDateTime();
				lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
				{
					HuodongCachingMgr._MeiRiChongZhiHaoLiActivity = meiRiChongZhiActivity;
				}
				return meiRiChongZhiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/Gifts/DayChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetMeiRiChongZhiActivity()
		{
			string uri = "Config/Gifts/DayChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
			{
				HuodongCachingMgr._MeiRiChongZhiHaoLiActivity = null;
			}
			return 0;
		}

		public static KingActivity GetChongJiHaoLiActivity()
		{
			lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._ChongJiHaoLiActivity != null)
				{
					return HuodongCachingMgr._ChongJiHaoLiActivity;
				}
			}
			try
			{
				string uri = "Config/XinFuGifts/MuLevel.xml";
				if (Global.isDoubleXinFu(33))
				{
					uri = "Config/XinFuGifts/MuDoubleLevel.xml";
				}
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				ChongJiHaoLiActivity chongJiHaoLiActivity = new ChongJiHaoLiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					chongJiHaoLiActivity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					chongJiHaoLiActivity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					chongJiHaoLiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					chongJiHaoLiActivity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					chongJiHaoLiActivity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							int key = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							AwardItem awardItem = new AwardItem();
							AwardItem awardItem2 = new AwardItem();
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinZhuanSheng"));
							awardItem.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Roles"));
							awardItem.MinAwardCondionValue3 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinLevel"));
							awardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinZhuanSheng"));
							awardItem2.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Roles"));
							awardItem2.MinAwardCondionValue3 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "MinLevel"));
							awardItem.GoodsDataList = new List<GoodsData>();
							awardItem2.GoodsDataList = new List<GoodsData>();
							int num = (int)Global.GetSafeAttributeLong(xelement3, "Roles");
							if (num == -1)
							{
								LogManager.WriteLog(1, string.Format("读取MuLevel.xml失败 字段：Roles", new object[0]), null, true);
							}
							else
							{
								chongJiHaoLiActivity.RoleLimit.Add(key, num);
							}
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取MuLevel.xml失败 GoodsOne", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取MuLevel.xml 失败 奖励列表1配置错误", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "读取MuLevel.xml 奖励列表1");
								}
							}
							chongJiHaoLiActivity.AwardDict.Add(key, awardItem);
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取MuLevel.xml失败 GoodsTwo", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取MuLevel.xml 失败 奖励列表2 配置错误", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "读取MuLevel.xml 奖励列表2");
								}
							}
							chongJiHaoLiActivity.AwardDict2.Add(key, awardItem2);
						}
					}
				}
				chongJiHaoLiActivity.PredealDateTime();
				lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
				{
					HuodongCachingMgr._ChongJiHaoLiActivity = chongJiHaoLiActivity;
				}
				return chongJiHaoLiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public static int ResetChongJiHaoLiActivity()
		{
			string uri = "Config/XinFuGifts/MuLevel.xml";
			if (Global.isDoubleXinFu(33))
			{
				uri = "Config/XinFuGifts/MuDoubleLevel.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
			lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
			{
				HuodongCachingMgr._ChongJiHaoLiActivity = null;
			}
			return 0;
		}

		public static ShenZhuangHuiKuiHaoLiActivity GetShenZhuangJiQiHuiKuiHaoLiActivity()
		{
			lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity != null)
				{
					return HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity;
				}
			}
			try
			{
				string uri = "Config/RiChangGifts/ShenZhuangAward.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				ShenZhuangHuiKuiHaoLiActivity shenZhuangHuiKuiHaoLiActivity = new ShenZhuangHuiKuiHaoLiActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					shenZhuangHuiKuiHaoLiActivity.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							shenZhuangHuiKuiHaoLiActivity.MyAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Roles"));
							shenZhuangHuiKuiHaoLiActivity.MyAwardItem.GoodsDataList = new List<GoodsData>();
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsIDs");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取神装激情回馈豪礼配置文件1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取神装激情回馈配置文件失败", new object[0]), null, true);
								}
								else
								{
									shenZhuangHuiKuiHaoLiActivity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "神装激情回馈");
								}
							}
						}
					}
				}
				shenZhuangHuiKuiHaoLiActivity.PredealDateTime();
				lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
				{
					HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity = shenZhuangHuiKuiHaoLiActivity;
				}
				return shenZhuangHuiKuiHaoLiActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/RiChangGifts/ShenZhuangAward.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetShenZhuangJiQiHuiKuiHaoLiActivity()
		{
			string uri = "Config/RiChangGifts/ShenZhuangAward.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
			{
				HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity = null;
			}
			return 0;
		}

		public static YueDuZhuanPanActivity GetYueDuZhuanPanActivity()
		{
			lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
			{
				if (HuodongCachingMgr._YueDuZhuanPanActivity != null)
				{
					return HuodongCachingMgr._YueDuZhuanPanActivity;
				}
			}
			try
			{
				string uri = "Config/RiChangGifts/NewDig2.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return null;
				}
				YueDuZhuanPanActivity yueDuZhuanPanActivity = new YueDuZhuanPanActivity();
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					yueDuZhuanPanActivity.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					yueDuZhuanPanActivity.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
				}
				lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
				{
					HuodongCachingMgr._YueDuZhuanPanActivity = yueDuZhuanPanActivity;
				}
				return yueDuZhuanPanActivity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/RiChangGifts/NewDig2.xml解析出现异常", ex, true);
			}
			return null;
		}

		public static int ResetYueDuZhuanPanActivity()
		{
			string uri = "Config/RiChangGifts/NewDig2.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
			{
				HuodongCachingMgr._YueDuZhuanPanActivity = null;
			}
			return 0;
		}

		public static void CheckJieRiActivityState(long ticks)
		{
			if (ticks - HuodongCachingMgr.lastJieRiProcessTicks >= 10000L)
			{
				HuodongCachingMgr.lastJieRiProcessTicks = ticks;
				DateTime jieriStartDay = Global.GetJieriStartDay();
				DateTime t = Global.GetJieriStartDay().AddDays((double)Global.GetJieriDaysNum());
				if (TimeUtil.NowDateTime() >= jieriStartDay && TimeUtil.NowDateTime() < t)
				{
					if (HuodongCachingMgr.JieRiState == 0)
					{
						HuodongCachingMgr.JieRiState = 1;
						GameManager.ClientMgr.NotifyAllActivityState(1, HuodongCachingMgr.JieRiState, "", "", 0);
					}
				}
				if (TimeUtil.NowDateTime() >= t)
				{
					if (HuodongCachingMgr.JieRiState == 1)
					{
						HuodongCachingMgr.JieRiState = 0;
						GameManager.ClientMgr.NotifyAllActivityState(1, HuodongCachingMgr.JieRiState, "", "", 0);
					}
				}
				t = Global.GetHefuStartDay().AddDays(8.0);
				if (TimeUtil.NowDateTime() >= t)
				{
					if (HuodongCachingMgr.HefuState == 1)
					{
						HuodongCachingMgr.HefuState = 0;
						GameManager.ClientMgr.NotifyAllActivityState(2, HuodongCachingMgr.HefuState, "", "", 0);
					}
				}
				ThemeActivityConfig themeActivityConfig = HuodongCachingMgr.GetThemeActivityConfig();
				if (null != themeActivityConfig)
				{
					if (HuodongCachingMgr.ThemeState != themeActivityConfig.ActivityOpenVavle)
					{
						HuodongCachingMgr.ThemeState = themeActivityConfig.ActivityOpenVavle;
						GameManager.ClientMgr.NotifyAllActivityState(14, themeActivityConfig.ActivityOpenVavle, "", "", 0);
					}
				}
				else if (HuodongCachingMgr.ThemeState == 1)
				{
					HuodongCachingMgr.ThemeState = 0;
					GameManager.ClientMgr.NotifyAllActivityState(14, 0, "", "", 0);
				}
			}
		}

		private static long lastJieRiProcessTicks = 0L;

		private static int JieRiState = 0;

		private static int HefuState = 1;

		private static int ThemeState = 0;

		private static Dictionary<int, WLoginItem> _WLoginDict = new Dictionary<int, WLoginItem>();

		private static Dictionary<int, MOnlineTimeItem> _MonthTimeDict = new Dictionary<int, MOnlineTimeItem>();

		private static Dictionary<int, NewStepItem> _NewStepDict = new Dictionary<int, NewStepItem>();

		private static Dictionary<int, CombatAwardItem> _CombatAwardlDict = new Dictionary<int, CombatAwardItem>();

		private static Dictionary<int, Dictionary<int, UpLevelItem>> _UpLevelDict = new Dictionary<int, Dictionary<int, UpLevelItem>>();

		private static object _BigAwardItemMutex = new object();

		private static BigAwardItem _BigAwardItem = null;

		private static object _SongLiItemMutex = new object();

		private static SongLiItem _SongLiItem = null;

		private static DateTime _LimitTimeLoginStartTime = new DateTime(1971, 1, 1);

		private static DateTime _LimitTimeLoginEndTime = new DateTime(1971, 1, 1);

		private static Dictionary<int, LimitTimeLoginItem> _LimitTimeLoginDict = new Dictionary<int, LimitTimeLoginItem>();

		private static Dictionary<int, EveryDayOnLineAward> _EveryDayOnLineAwardDict = new Dictionary<int, EveryDayOnLineAward>();

		private static Dictionary<int, SeriesLoginAward> _SeriesLoginAward = new Dictionary<int, SeriesLoginAward>();

		private static FirstChongZhiGift _FirstChongZhiActivity = null;

		private static object _FirstChongZhiActivityMutex = new object();

		private static InputFanLiActivity _InputFanLiActivity = null;

		private static object _InputFanLiActivityMutex = new object();

		private static InputSongActivity _InputSongActivity = null;

		private static object _InputSongActivityMutex = new object();

		private static KingActivity _InputKingActivity = null;

		private static object _InputKingActivityMutex = new object();

		private static KingActivity _LevelKingActivity = null;

		private static object _LevelKingActivityMutex = new object();

		private static KingActivity _EquipKingActivity = null;

		private static object _EquipKingActivityMutex = new object();

		private static KingActivity _HorseKingActivity = null;

		private static object _HorseKingActivityMutex = new object();

		private static KingActivity _JingMaiKingActivity = null;

		private static object _JingMaiKingActivityMutex = new object();

		private static KingActivity _XinXiaofeiKingActivity = null;

		private static object _XinXiaofeiKingMutex = new object();

		public static Dictionary<int, UpLevelAwardItem> UpLevelAwardItemDict = null;

		public static Dictionary<int, KaiFuGiftItem> KaiFuGiftItemDict = null;

		private static int LastProcessGetKaiFuGiftAwardDayID = 0;

		public static int ProcessKaiFuGiftAwardHour = 12;

		private static int LastAutoAddKaiFuGiftRoleNumDayID = TimeUtil.NowDateTime().DayOfYear;

		private static JieriActivityConfig _JieriActivityConfig = null;

		private static object _JieriActivityConfigMutex = new object();

		private static ThemeActivityConfig _ThemeActivityConfig = null;

		private static object _ThemeActivityConfigMutex = new object();

		private static ThemeDaLiBaoActivity _ThemeDaLiBaoActivity = null;

		private static object _ThemeDaLiBaoActivityMutex = new object();

		private static ThemeDuiHuanActivity _ThemeDuiHuanActivity = null;

		private static object _ThemeDuiHuanActivityMutex = new object();

		private static ThemeZhiGouActivity _ThemeZhiGouActivity = null;

		private static object _ThemeZhiGouActivityMutex = new object();

		private static JieriDaLiBaoActivity _JieriDaLiBaoActivity = null;

		private static object _JieriDaLiBaoActivityMutex = new object();

		private static JieRiDengLuActivity _JieRiDengLuActivity = null;

		private static object _JieriDengLuActivityMutex = new object();

		private static JieriVIPActivity _JieriVIPActivity = null;

		private static object _JieriVIPActivityMutex = new object();

		private static JieriGiveActivity _JieriGiveActivity = null;

		private static object _JieriGiveMutex = new object();

		private static JieriRecvActivity _JieriRecvActivity = null;

		private static object _JieriRecvMutex = new object();

		private static JieRiGiveKingActivity _JieriGiveKingActivity = null;

		private static object _JieriGiveKingMutex = new object();

		private static JieRiRecvKingActivity _JieriRecvKingActivity = null;

		private static object _JieriRecvKingMutex = new object();

		private static JieRiFuLiActivity _JieriFuLiActivity = null;

		private static object _JieriFuLiMutex = new object();

		private static JieriCZSongActivity _JieriCZSongActivity = null;

		private static object _JieriCZSongActivityMutex = new object();

		private static JieRiCZQGActivity _JieRiCZQGActivity = null;

		private static object _JieRiCZQGActivityMutex = new object();

		private static OneDollarBuyActivity _OneDollarBuyActivity = null;

		private static object _OneDollarBuyActivityMutex = new object();

		private static OneDollarChongZhi _OneDollarChongZhi = null;

		private static object _OneDollarChongZhiMutex = new object();

		private static InputFanLiNew _InputFanLiNew = null;

		private static object _InputFanLiNewMutex = new object();

		private static RegressActiveOpen _RegressActiveOpen = null;

		private static object _RegressActiveOpenMutex = new object();

		private static RegressActiveSignGift _RegressActiveSignGift = null;

		private static object _RegressActiveSignGiftMutex = new object();

		private static RegressActiveTotalRecharge _RegressActiveTotalRecharge = null;

		private static object _RegressActiveTotalRechargeMutex = new object();

		private static RegressActiveDayBuy _RegressActiveDayBuy = null;

		private static object _RegressActiveDayBuyMutex = new object();

		private static RegressActiveStore _RegressActiveStore = null;

		private static object _RegressActiveStoreMutex = new object();

		private static JieriSuperInputActivity _JieriSuperInput = null;

		private static object _JieriSuperInputMutex = new object();

		private static JieriVIPYouHuiActivity _JieriVIPYouHuiActivity = null;

		private static object _JieriVIPYouHuiActMutex = new object();

		private static SpecialActivity _SpecialActivity = null;

		private static object _SpecialActivityMutex = new object();

		private static SpecPriorityActivity _SpecPriorityActivity = null;

		private static object _SpecPriorityActivityMutex = new object();

		private static EverydayActivity _EverydayActivity = null;

		private static object _EverydayActivityMutex = new object();

		private static JieriIPointsExchgActivity _JieriIPointsExchgActivity = null;

		private static object _JieriIPointsExchgActivityMutex = new object();

		private static WeedEndInputActivity _WeedEndInputActivity = null;

		private static object _WeedEndInputActivityMutex = new object();

		private static JieRiLeiJiCZActivity _JieRiLeiJiCZActivity = null;

		private static object _JieRiLeiJiCZActivityMutex = new object();

		private static JieRiTotalConsumeActivity _JieRiTotalConsumeActivity = null;

		private static object _JieRiTotalConsumeActivityMutex = new object();

		private static JieRiMeiRiLeiJiActivity _JieRiMeiRiLeiJiActivity = null;

		private static object _JieRiMeiRiLeiJiActivityMutex = new object();

		private static JieRiMultAwardActivity _JieRiMultAwardActivity = null;

		private static object _JieRiMultAwardActivityMutex = new object();

		private static JieRiZiKaLiaBaoActivity _JieRiZiKaLiaBaoActivity = null;

		private static object _JieRiZiKaLiaBaoActivityMutex = new object();

		private static KingActivity _JieRiXiaoFeiKingActivity = null;

		private static object _JieRiXiaoFeiKingActivityMutex = new object();

		private static KingActivity _JieRiCZKingActivity = null;

		private static object _JieRiCZKingActivityMutex = new object();

		private static HuodongCachingMgr.TotalChargeActivity _TotalChargeActivity = null;

		private static object _TotalChargeActivityMutex = new object();

		private static HuodongCachingMgr.TotalConsumeActivity _TotalConsumeActivity = null;

		private static object _TotalConsumeActivityMutex = new object();

		private static JieriFanLiActivity[] _JieriWingFanliAct = new JieriFanLiActivity[11];

		private static object _JieriWingFanliActMutex = new object();

		private static object _JieriLianXuChargeMutex = new object();

		private static JieriLianXuChargeActivity _JieriLianXuChargeAct = null;

		private static object _JieriPlatChargeKingMutex = new object();

		private static JieriPlatChargeKing _JieriPlatChargeKingAct = null;

		private static object _JieriPCKingEveryDayMutex = new object();

		private static JieriPlatChargeKingEveryDay _JieriPCKingEveryDayAct = null;

		private static object _DanBiChongZhiMutex = new object();

		private static DanBiChongZhiActivity _DanBiChongZhiAct = null;

		private static HeFuActivityConfig _HeFuActivityConfig = null;

		private static object _HeFuActivityConfigMutex = new object();

		private static HeFuLoginActivity _HeFuLoginActivity = null;

		private static object _HeFuLoginActivityMutex = new object();

		private static HeFuRechargeActivity _HeFuRechargeActivity = null;

		private static object _HeFuRechargeActivityMutex = new object();

		private static HeFuTotalLoginActivity _HeFuTotalLoginActivity = null;

		private static object _HeFuTotalLoginActivityMutex = new object();

		private static HeFuPKKingActivity _HeFuPKKingActivity = null;

		private static object _HeFuAwardTimeActivityMutex = new object();

		private static HeFuAwardTimesActivity _HeFuAwardTimeActivity = null;

		private static HeFuLuoLanActivity _HeFuLuoLanActivity = null;

		private static object _HeFuLuoLanActivityMutex = new object();

		private static object _HeFuPKKingActivityMutex = new object();

		private static HeFuWCKingActivity _HeFuWCKingActivity = null;

		private static object _HeFuWCKingActivityMutex = new object();

		private static XinFanLiActivity _XinFanLiActivity = null;

		private static object _XinFanLiActivityMutex = new object();

		private static object _MeiRiChongZhiHaoLiActivityMutex = new object();

		private static MeiRiChongZhiActivity _MeiRiChongZhiHaoLiActivity = null;

		private static object _ChongJiHaoLiActivityMutex = new object();

		private static ChongJiHaoLiActivity _ChongJiHaoLiActivity = null;

		private static object _ShenZhuangJiQingHuiKuiHaoLiActivityMutex = new object();

		private static ShenZhuangHuiKuiHaoLiActivity _ShenZhuangJiQingHuiKuiHaoLiActivity = null;

		private static object _YueDuZhuanPanActivityMutex = new object();

		private static YueDuZhuanPanActivity _YueDuZhuanPanActivity = null;

		private static class GiftCodeFlags
		{
			public const int Local = 1;

			public const int Center = 2;
		}

		public class TotalConsumeActivity : KingActivity
		{
			public override bool CanGiveAward(GameClient client, int index, int totalMoney)
			{
				bool flag = false;
				try
				{
					if (this.AwardDict != null && this.AwardDict.ContainsKey(index))
					{
						flag = (this.AwardDict[index].MinAwardCondionValue <= totalMoney);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				return flag;
			}
		}

		public class TotalChargeActivity : KingActivity
		{
			public override bool CanGiveAward(GameClient client, int index, int totalMoney)
			{
				bool flag = false;
				try
				{
					if (this.AwardDict != null && this.AwardDict.ContainsKey(index))
					{
						flag = (this.AwardDict[index].MinAwardCondionValue <= totalMoney);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				return flag;
			}
		}
	}
}
