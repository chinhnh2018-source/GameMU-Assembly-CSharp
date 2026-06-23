using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Reborn;
using GameServer.Logic.UserReturn;
using GameServer.Logic.YueKa;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.UserMoneyCharge
{
	public class UserMoneyMgr
	{
		public static UserMoneyMgr getInstance()
		{
			return UserMoneyMgr.instance;
		}

		public bool InitConfig()
		{
			this.InitFirstChargeConfigData();
			this.InitItemChargeConfigData();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("JieRiChongZhiQiangGou");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						dictionary[Global.SafeConvertToInt32(array3[0])] = Global.SafeConvertToInt32(array3[1]);
					}
				}
			}
			dictionary.TryGetValue(this.GetActivityPlatformType(), out this.PlatformOpenStateVavle);
			GameManager.ClientMgr.NotifyAllActivityState(6, this.PlatformOpenStateVavle, "", "", 0);
			return true;
		}

		public void InitItemChargeConfigData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ZhiGou.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ZhiGou.xml"));
				if (xelement != null)
				{
					Dictionary<int, ChargeItemData> dictionary = new Dictionary<int, ChargeItemData>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							ChargeItemData chargeItemData = new ChargeItemData();
							chargeItemData.ChargeItemID = (int)Global.GetSafeAttributeLong(xelement2, "ZhiGouID");
							chargeItemData.ChargeDangID = (int)Global.GetSafeAttributeLong(xelement2, "ChongZhiID");
							chargeItemData.SinglePurchase = (int)Global.GetSafeAttributeLong(xelement2, "SinglePurchase");
							chargeItemData.DayPurchase = (int)Global.GetSafeAttributeLong(xelement2, "EverydayPurchase");
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "ThemePurchase", "");
							chargeItemData.ThemePurchase = Global.SafeConvertToInt32(defAttributeStr);
							chargeItemData.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
							chargeItemData.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
							if (0 == chargeItemData.FromDate.CompareTo("-1"))
							{
								chargeItemData.FromDate = "2008-08-08 08:08:08";
							}
							if (0 == chargeItemData.ToDate.CompareTo("-1"))
							{
								chargeItemData.ToDate = "2028-08-08 08:08:08";
							}
							chargeItemData.GoodsStringOne = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
							if (!string.IsNullOrEmpty(chargeItemData.GoodsStringOne))
							{
								string[] fields = chargeItemData.GoodsStringOne.Split(new char[]
								{
									'|'
								});
								chargeItemData.GoodsDataOne = HuodongCachingMgr.ParseGoodsDataList(fields, "ZhiGou.xml励配置文件1");
							}
							chargeItemData.GoodsStringTwo = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
							if (!string.IsNullOrEmpty(chargeItemData.GoodsStringTwo))
							{
								string[] fields = chargeItemData.GoodsStringTwo.Split(new char[]
								{
									'|'
								});
								chargeItemData.GoodsDataTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "ZhiGou.xml励配置文件1");
							}
							dictionary[chargeItemData.ChargeItemID] = chargeItemData;
						}
					}
					Data.ChargeItemDict = dictionary;
				}
				else
				{
					LogManager.WriteLog(1000, string.Format("丢失充值直购配置文件{0}", "Config/ZhiGou.xml"), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "读取Config/ZhiGou.xml错误:" + ex.ToString(), ex, true);
			}
		}

		public void InitFirstChargeConfigData()
		{
			try
			{
				string text = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
				text = text.ToLower();
				string sectionKey = string.Empty;
				ChargePlatformType chargePlatType;
				if (text == "app")
				{
					sectionKey = "dl_app";
					chargePlatType = ChargePlatformType.CPT_App;
				}
				else if (text == "yueyu")
				{
					sectionKey = "dl_yueyu";
					chargePlatType = ChargePlatformType.CPT_YueYu;
				}
				else if (text == "andrid" || text == "android" || text == "yyb")
				{
					sectionKey = "dl_android";
					chargePlatType = ChargePlatformType.CPT_Android;
				}
				else
				{
					sectionKey = "dl_app";
					chargePlatType = ChargePlatformType.CPT_App;
				}
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/MU_ChongZhi.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/MU_ChongZhi.xml"));
				if (xelement != null)
				{
					IEnumerable<XElement> enumerable = xelement.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey).Elements();
					SingleChargeData singleChargeData = new SingleChargeData();
					singleChargeData.YueKaBangZuan = GameManager.PlatConfigMgr.GetGameConfigItemInt("YueKaBangZuan", 0);
					singleChargeData.ChargePlatType = (int)chargePlatType;
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							int num = (int)Global.GetSafeAttributeLong(xelement2, "RMB");
							int value = (int)Global.GetSafeAttributeLong(xelement2, "FirstBindZuanShi");
							int num2 = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							if (num2 == YueKaManager.YUE_KA_MONEY_ID_IN_CHARGE_FILE)
							{
								singleChargeData.YueKaMoney = num;
							}
							singleChargeData.ChargeIDVsMoneyDict[num2] = num;
							singleChargeData.MoneyVsChargeIDDict[num] = num2;
							if (!singleChargeData.singleData.ContainsKey(num))
							{
								singleChargeData.singleData[num] = value;
							}
						}
					}
					JieriSuperInputActivity jieRiSuperInputActivity = HuodongCachingMgr.GetJieRiSuperInputActivity();
					if (null != jieRiSuperInputActivity)
					{
						jieRiSuperInputActivity.FilterSingleChargeData(singleChargeData);
					}
					if (1 == Global.sendToDB<int, byte[]>(10171, DataHelper.ObjectToBytes<SingleChargeData>(singleChargeData), 0))
					{
						Data.ChargeData = singleChargeData;
					}
				}
				else
				{
					LogManager.WriteLog(1000, string.Format("丢失平台充值配置文件{0}", "Config/MU_ChongZhi.xml"), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException("读取充值xml错误：" + ex.ToString());
			}
		}

		public void NotifyActivityState(GameClient client)
		{
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				6,
				this.PlatformOpenStateVavle,
				"",
				0,
				0
			});
			client.sendCmd(770, cmdData, false);
		}

		public int GetActivityPlatformType()
		{
			string text = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
			text = text.ToLower();
			ZhiGouActPlatformType result;
			if (text == "app")
			{
				result = ZhiGouActPlatformType.CZQG_App;
			}
			else if (text == "yueyu")
			{
				result = ZhiGouActPlatformType.CZQG_YueYu;
			}
			else if (text == "andrid" || text == "android")
			{
				result = ZhiGouActPlatformType.CZQG_Android;
			}
			else if (text == "yyb")
			{
				result = ZhiGouActPlatformType.CZQG_YYB;
			}
			else
			{
				result = ZhiGouActPlatformType.CZQG_App;
			}
			return (int)result;
		}

		private void GiveClientChargeItem(GameClient client, List<GoodsData> awardList)
		{
			int num;
			if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, awardList, out num))
			{
				Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(553, new object[0]), GLang.GetLang(554, new object[0]), 0, 0, 0);
			}
			else
			{
				for (int i = 0; i < awardList.Count; i++)
				{
					GoodsData goodsData = awardList[i];
					if (null != goodsData)
					{
						goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "充值直购", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, 0, 0, null, null, 0, true);
					}
				}
			}
		}

		public int GetChargeItemPurchaseNum(GameClient client, int ZhiGouID)
		{
			int result = 0;
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				Dictionary<int, int> chargeItemPurchaseDict2 = client.ClientData.ChargeItemPurchaseDict;
				chargeItemPurchaseDict2.TryGetValue(ZhiGouID, out result);
			}
			return result;
		}

		private void AddChargeItemPurchaseNum(GameClient client, int ZhiGouID)
		{
			int num = 0;
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				Dictionary<int, int> chargeItemPurchaseDict2 = client.ClientData.ChargeItemPurchaseDict;
				chargeItemPurchaseDict2.TryGetValue(ZhiGouID, out num);
				num = (chargeItemPurchaseDict2[ZhiGouID] = num + 1);
			}
		}

		public int GetChargeItemDayPurchaseNum(GameClient client, int ZhiGouID)
		{
			int result = 0;
			lock (client.ClientData.ChargeItemDayPurchaseDict)
			{
				int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				if (client.ClientData.ChargeItemDayPurchaseDayID != offsetDay)
				{
					client.ClientData.ChargeItemDayPurchaseDict.Clear();
					client.ClientData.ChargeItemDayPurchaseDayID = offsetDay;
				}
				Dictionary<int, int> chargeItemDayPurchaseDict2 = client.ClientData.ChargeItemDayPurchaseDict;
				chargeItemDayPurchaseDict2.TryGetValue(ZhiGouID, out result);
			}
			return result;
		}

		private void AddChargeItemDayPurchaseNum(GameClient client, int ZhiGouID, string chargeTime)
		{
			DateTime dateTime;
			DateTime.TryParse(chargeTime, out dateTime);
			int offsetDay = TimeUtil.GetOffsetDay(dateTime);
			if (client.ClientData.ChargeItemDayPurchaseDayID == offsetDay)
			{
				int num = 0;
				lock (client.ClientData.ChargeItemDayPurchaseDict)
				{
					Dictionary<int, int> chargeItemDayPurchaseDict2 = client.ClientData.ChargeItemDayPurchaseDict;
					chargeItemDayPurchaseDict2.TryGetValue(ZhiGouID, out num);
					num = (chargeItemDayPurchaseDict2[ZhiGouID] = num + 1);
				}
			}
		}

		public bool CheckChargeItemConfigLogic(int zhigouID, int PurchaseNum, string GoodsOne, string GoodsTwo, string FromSystem)
		{
			Dictionary<int, ChargeItemData> chargeItemDict = Data.ChargeItemDict;
			bool result;
			if (null == chargeItemDict)
			{
				LogManager.WriteLog(1000, string.Format("警告：充值直购文件尚未成功加载", new object[0]), null, true);
				result = false;
			}
			else
			{
				ChargeItemData chargeItemData = null;
				if (!chargeItemDict.TryGetValue(zhigouID, out chargeItemData))
				{
					LogManager.WriteLog(1000, string.Format("警告：充值直购配置错误 zhigouID={0} fromSys={1}", zhigouID, FromSystem), null, true);
					result = false;
				}
				else if (chargeItemData.SinglePurchase != PurchaseNum && chargeItemData.DayPurchase != PurchaseNum && chargeItemData.ThemePurchase != PurchaseNum)
				{
					LogManager.WriteLog(1000, string.Format("警告：充值直购配置错误 Purchase={0} fromSys={1}", PurchaseNum, FromSystem), null, true);
					result = false;
				}
				else if (chargeItemData.GoodsStringOne != GoodsOne || chargeItemData.GoodsStringTwo != GoodsTwo)
				{
					LogManager.WriteLog(1000, string.Format("警告：充值直购配置错误 GoodsOne={0} GoodsTwo={1} fromSys={2}", GoodsOne, GoodsTwo, FromSystem), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void ComputeClientChargeItem(GameClient client, List<TempItemChargeInfo> list)
		{
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				client.ClientData.ChargeItemPurchaseDict.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					TempItemChargeInfo tempItemChargeInfo = list[i];
					if (tempItemChargeInfo.isDel != 0 && tempItemChargeInfo.isDel != 2)
					{
						int num = 0;
						client.ClientData.ChargeItemPurchaseDict.TryGetValue(tempItemChargeInfo.zhigouID, out num);
						num = (client.ClientData.ChargeItemPurchaseDict[tempItemChargeInfo.zhigouID] = num + 1);
					}
				}
			}
			lock (client.ClientData.ChargeItemDayPurchaseDict)
			{
				client.ClientData.ChargeItemDayPurchaseDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				client.ClientData.ChargeItemDayPurchaseDict.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					TempItemChargeInfo tempItemChargeInfo = list[i];
					if (tempItemChargeInfo.isDel != 0 && tempItemChargeInfo.isDel != 2)
					{
						int num = 0;
						DateTime dateTime;
						DateTime.TryParse(tempItemChargeInfo.chargeTime, out dateTime);
						int offsetDay = TimeUtil.GetOffsetDay(dateTime);
						if (offsetDay == client.ClientData.ChargeItemDayPurchaseDayID)
						{
							client.ClientData.ChargeItemDayPurchaseDict.TryGetValue(tempItemChargeInfo.zhigouID, out num);
							num = (client.ClientData.ChargeItemDayPurchaseDict[tempItemChargeInfo.zhigouID] = num + 1);
						}
					}
				}
			}
		}

		public void HandleSystemChargeMoney(string userID, int addMoney)
		{
			TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
			if (null != tmsksocket)
			{
				LogManager.WriteLog(3, string.Format("通知账户ID{0}的角色更新元宝数量", userID), null, true);
				GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
				if (null != gameClient)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, 0, "", ActivityTypes.None, "");
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "GM命令强迫更新", "系统", gameClient.ClientData.RoleName, "增加", addMoney, gameClient.ClientData.ZoneID, gameClient.strUserID, gameClient.ClientData.UserMoney, gameClient.ServerId, null);
				}
			}
		}

		public void HandleClientChargeMoney(string userID, int rid, int addMoney, bool transmit, int superInputFanLi = 0)
		{
			JieriIPointsExchgActivity jieriIPointsExchgActivity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			if (null != jieriIPointsExchgActivity)
			{
				jieriIPointsExchgActivity.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
			if (null != everydayActivity)
			{
				everydayActivity.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
			if (null != everydayActivity)
			{
				specPriorityActivity.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			if (superInputFanLi > 0)
			{
				JieriSuperInputActivity jieRiSuperInputActivity = HuodongCachingMgr.GetJieRiSuperInputActivity();
				if (null != jieRiSuperInputActivity)
				{
					jieRiSuperInputActivity.OnMoneyChargeEvent(userID, rid, addMoney, superInputFanLi);
				}
			}
			TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
			if (null == tmsksocket)
			{
				SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
				if (null != specialActivity)
				{
					specialActivity.OnMoneyChargeEventOffLine(userID, rid, addMoney);
				}
			}
			else
			{
				if (transmit)
				{
					LogManager.WriteLog(3, string.Format("通知账户ID{0}的角色更新元宝数量", userID), null, true);
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
				if (null != gameClient)
				{
					SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
					if (null != specialActivity)
					{
						specialActivity.OnMoneyChargeEventOnLine(gameClient, addMoney);
					}
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, 0, "", ActivityTypes.None, "");
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "GM命令强迫更新", "系统", gameClient.ClientData.RoleName, "增加", addMoney, gameClient.ClientData.ZoneID, gameClient.strUserID, gameClient.ClientData.UserMoney, gameClient.ServerId, null);
					gameClient._IconStateMgr.FlushChongZhiIconState(gameClient);
					gameClient._IconStateMgr.CheckJieRiActivity(gameClient, false);
					gameClient._IconStateMgr.SendIconStateToClient(gameClient);
					UserReturnManager.getInstance().initUserReturnData(gameClient);
					SingletonTemplate<SevenDayActivityMgr>.Instance().OnCharge(gameClient);
					FundManager.initFundData(gameClient);
				}
				else
				{
					SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
					if (null != specialActivity)
					{
						specialActivity.OnMoneyChargeEventOffLine(userID, rid, addMoney);
					}
				}
			}
		}

		public void HandleClientChargeItem(GameClient client, byte HandleDel = 0)
		{
			if (client != null && !GameManager.IsKuaFuServer)
			{
				lock (client.ClientData.ChargeItemMutex)
				{
					List<TempItemChargeInfo> list = new List<TempItemChargeInfo>();
					string text = string.Format("{0}:{1}:{2}", client.strUserID, client.ClientData.RoleID, HandleDel);
					list = Global.sendToDB<List<TempItemChargeInfo>, string>(13320, text, client.ServerId);
					if (list != null && list.Count != 0)
					{
						if (HandleDel == 1)
						{
							this.ComputeClientChargeItem(client, list);
						}
						Dictionary<int, ChargeItemData> chargeItemDict = Data.ChargeItemDict;
						for (int i = 0; i < list.Count; i++)
						{
							TempItemChargeInfo tempItemChargeInfo = list[i];
							if (tempItemChargeInfo.isDel == 0)
							{
								int num = 0;
								int num2 = 0;
								ChargeItemData chargeItemData = null;
								if (!chargeItemDict.TryGetValue(tempItemChargeInfo.zhigouID, out chargeItemData))
								{
									LogManager.WriteLog(2, string.Format("充值直购zhigouID错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
									{
										tempItemChargeInfo.userID,
										tempItemChargeInfo.chargeRoleID,
										tempItemChargeInfo.addUserMoney,
										tempItemChargeInfo.zhigouID,
										tempItemChargeInfo.chargeTime
									}), null, true);
									num = 1;
								}
								else if (string.Compare(tempItemChargeInfo.chargeTime, chargeItemData.FromDate) < 0 || string.Compare(tempItemChargeInfo.chargeTime, chargeItemData.ToDate) > 0)
								{
									LogManager.WriteLog(2, string.Format("充值直购Time错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
									{
										tempItemChargeInfo.userID,
										tempItemChargeInfo.chargeRoleID,
										tempItemChargeInfo.addUserMoney,
										tempItemChargeInfo.zhigouID,
										tempItemChargeInfo.chargeTime
									}), null, true);
									num = 1;
								}
								else
								{
									int num3 = 0;
									if (!Data.ChargeData.ChargeIDVsMoneyDict.TryGetValue(chargeItemData.ChargeDangID, out num3))
									{
										LogManager.WriteLog(2, string.Format("充值直购Dang错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
										{
											tempItemChargeInfo.userID,
											tempItemChargeInfo.chargeRoleID,
											tempItemChargeInfo.addUserMoney,
											tempItemChargeInfo.zhigouID,
											tempItemChargeInfo.chargeTime
										}), null, true);
										num = 1;
									}
									else
									{
										int gameConfigItemInt = GameManager.PlatConfigMgr.GetGameConfigItemInt("zhigou_implicit", 0);
										if (gameConfigItemInt > 0)
										{
											if (tempItemChargeInfo.addUserMoney < num3)
											{
												LogManager.WriteLog(2, string.Format("充值直购Money错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
												{
													tempItemChargeInfo.userID,
													tempItemChargeInfo.chargeRoleID,
													tempItemChargeInfo.addUserMoney,
													tempItemChargeInfo.zhigouID,
													tempItemChargeInfo.chargeTime
												}), null, true);
												num = 1;
											}
											else
											{
												num2 = Math.Max(tempItemChargeInfo.addUserMoney - num3, 0);
											}
										}
										else if (tempItemChargeInfo.addUserMoney != num3)
										{
											LogManager.WriteLog(2, string.Format("充值直购Money错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
											{
												tempItemChargeInfo.userID,
												tempItemChargeInfo.chargeRoleID,
												tempItemChargeInfo.addUserMoney,
												tempItemChargeInfo.zhigouID,
												tempItemChargeInfo.chargeTime
											}), null, true);
											num = 1;
										}
									}
								}
								int chargeItemPurchaseNum = this.GetChargeItemPurchaseNum(client, tempItemChargeInfo.zhigouID);
								int chargeItemDayPurchaseNum = this.GetChargeItemDayPurchaseNum(client, tempItemChargeInfo.zhigouID);
								if (chargeItemData != null && chargeItemData.SinglePurchase > 0 && chargeItemPurchaseNum >= chargeItemData.SinglePurchase)
								{
									num = 1;
								}
								if (chargeItemData != null && chargeItemData.DayPurchase > 0)
								{
									EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
									if (everydayActivity == null || !everydayActivity.CheckValidChargeItem(tempItemChargeInfo.zhigouID))
									{
										num = 1;
									}
									if (chargeItemDayPurchaseNum >= chargeItemData.DayPurchase)
									{
										num = 1;
									}
								}
								if (chargeItemData != null && chargeItemData.ThemePurchase > 0)
								{
									ThemeZhiGouActivity themeZhiGouActivity = HuodongCachingMgr.GetThemeZhiGouActivity();
									if (themeZhiGouActivity == null || !themeZhiGouActivity.CheckValidChargeItem(tempItemChargeInfo.zhigouID))
									{
										num = 1;
									}
									if (chargeItemDayPurchaseNum >= chargeItemData.ThemePurchase)
									{
										num = 1;
									}
								}
								SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
								if (specPriorityActivity != null && !specPriorityActivity.CheckValidChargeItem(tempItemChargeInfo.zhigouID))
								{
									num = 1;
								}
								RegressActiveDayBuy regressActiveDayBuy = HuodongCachingMgr.GetRegressActiveDayBuy();
								if (regressActiveDayBuy != null && !regressActiveDayBuy.CheckValidChargeItem(client, tempItemChargeInfo.zhigouID))
								{
									num = 1;
								}
								text = string.Format("{0}:{1}:{2}", tempItemChargeInfo.ID, num, num2);
								string[] array = null;
								TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 13321, text, out array, client.ServerId);
								if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED && array.Length > 0)
								{
									if (num == 0)
									{
										this.AddChargeItemPurchaseNum(client, tempItemChargeInfo.zhigouID);
										this.AddChargeItemDayPurchaseNum(client, tempItemChargeInfo.zhigouID, tempItemChargeInfo.chargeTime);
									}
									if (chargeItemData != null && num == 0)
									{
										ChargeItemBaseEventObject eventObj = new ChargeItemBaseEventObject(client, chargeItemData);
										GlobalEventSource.getInstance().fireEvent(eventObj);
									}
									if (num != 1)
									{
										if (null != chargeItemData)
										{
											List<GoodsData> list2 = new List<GoodsData>();
											if (chargeItemData.GoodsDataOne != null)
											{
												list2.AddRange(chargeItemData.GoodsDataOne);
											}
											List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, chargeItemData.GoodsDataTwo);
											if (awardPro != null)
											{
												list2.AddRange(awardPro);
											}
											this.GiveClientChargeItem(client, list2);
											this.HandleClientChargeMoney(client.strUserID, client.ClientData.RoleID, tempItemChargeInfo.addUserMoney - num2, false, 0);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void ProcessSendBindGold(GameClient client, int bindMoney, string userId, int rid, string firstbindData)
		{
			if (client != null)
			{
				GameManager.ClientMgr.SendToClient(client, firstbindData, 671);
				GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindMoney, "首次充值送绑钻(在线)");
			}
			else
			{
				GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, rid, bindMoney, "首次充值送绑钻(离线)", userId);
			}
		}

		public TCPProcessCmdResults ProcessGetFirstChargeInfoCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string[] array2 = null;
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 671, gameClient.strUserID, out array2, gameClient.ServerId);
				if (null == array2)
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string text2 = array2[0];
				if (text2 == "-1")
				{
					text2 = "";
				}
				string data2 = string.Format("{0}", text2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetFirstChargeInfoCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static UserMoneyMgr instance = new UserMoneyMgr();

		public int PlatformOpenStateVavle = 0;
	}
}
