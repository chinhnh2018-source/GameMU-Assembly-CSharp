using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic
{
	public class FundManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static FundManager getInstance()
		{
			return FundManager.instance;
		}

		public bool initialize()
		{
			FundManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1032, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1033, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1034, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1032:
				result = this.ProcessFundInfoCmd(client, nID, bytes, cmdParams);
				break;
			case 1033:
				result = this.ProcessFundBuyCmd(client, nID, bytes, cmdParams);
				break;
			case 1034:
				result = this.ProcessFundAwardCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool ProcessFundInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				FundData cmdData = FundManager.FundGetData(client);
				client.sendCmd<FundData>(1032, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessFundBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int fundType = int.Parse(cmdParams[0]);
				FundData cmdData = FundManager.FundBuy(client, fundType);
				client.sendCmd<FundData>(1033, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessFundAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int fundType = int.Parse(cmdParams[0]);
				FundData cmdData = FundManager.FundAward(client, fundType);
				client.sendCmd<FundData>(1034, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private static FundData FundGetData(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			bool flag = FundManager.IsGongNengOpened(client, false);
			if (fundData.IsOpen != flag)
			{
				FundManager.initFundData(client);
			}
			return FundManager.GetFundData(client);
		}

		private static FundData FundBuy(GameClient client, int fundType)
		{
			FundData fundData = FundManager.GetFundData(client);
			FundData result;
			if (!fundData.IsOpen)
			{
				result = fundData;
			}
			else
			{
				fundData.FundType = fundType;
				if (GameFuncControlManager.IsGameFuncDisabled(8))
				{
					fundData.State = -2;
					result = fundData;
				}
				else if (!fundData.FundDic.ContainsKey(fundType))
				{
					fundData.State = -1;
					result = fundData;
				}
				else
				{
					FundItem fundItem = fundData.FundDic[fundType];
					if (fundItem.BuyType == 1)
					{
						fundData.State = -4;
						result = fundData;
					}
					else if (fundItem.BuyType == 3)
					{
						fundData.State = -5;
						result = fundData;
					}
					else
					{
						FundInfo fundInfo = FundManager._fundDic[fundItem.FundID];
						if (fundInfo.Price > client.ClientData.UserMoney)
						{
							fundData.State = -3;
							result = fundData;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, fundInfo.Price, "基金购买", true, false, false, DaiBiSySType.None))
						{
							fundData.State = -1;
							result = fundData;
						}
						else
						{
							DateTime buyTime = TimeUtil.NowDateTime();
							if (!FundManager.DBFundBuy(client, new FundDBItem
							{
								zoneID = client.ClientData.ZoneID,
								UserID = client.strUserID,
								RoleID = client.ClientData.RoleID,
								FundType = fundData.FundType,
								FundID = fundItem.FundID,
								BuyTime = buyTime,
								State = 1
							}))
							{
								fundData.State = -1;
								result = fundData;
							}
							else
							{
								fundItem.BuyType = 1;
								fundItem.BuyTime = buyTime;
								if (fundItem.FundType == 2)
								{
									fundItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(fundItem.BuyTime) + 1;
								}
								FundAwardInfo fundAwardInfo = FundManager._fundAwardDic[fundItem.AwardID];
								if (fundItem.Value1 >= fundAwardInfo.Value1 && fundItem.Value2 >= fundAwardInfo.Value2)
								{
									fundItem.AwardType = 2;
								}
								else
								{
									fundItem.AwardType = 3;
								}
								fundData.State = 1;
								fundData.FundType = fundType;
								FundManager.CheckActivityTip(client);
								result = fundData;
							}
						}
					}
				}
			}
			return result;
		}

		private static FundData FundAward(GameClient client, int fundType)
		{
			FundData fundData = FundManager.GetFundData(client);
			FundData result;
			if (!fundData.IsOpen)
			{
				result = fundData;
			}
			else
			{
				fundData.FundType = fundType;
				if (!fundData.FundDic.ContainsKey(fundType))
				{
					fundData.State = -1;
					result = fundData;
				}
				else
				{
					FundItem myItem = fundData.FundDic[fundType];
					if (myItem.BuyType != 1)
					{
						fundData.State = -8;
						result = fundData;
					}
					else if (myItem.AwardType == 3)
					{
						fundData.State = -6;
						result = fundData;
					}
					else if (myItem.AwardType == 1)
					{
						fundData.State = -7;
						result = fundData;
					}
					else
					{
						DateTime buyTime = TimeUtil.NowDateTime();
						FundDBItem fundDBItem = new FundDBItem();
						fundDBItem.zoneID = client.ClientData.ZoneID;
						fundDBItem.UserID = client.strUserID;
						fundDBItem.RoleID = client.ClientData.RoleID;
						fundDBItem.FundType = fundData.FundType;
						fundDBItem.FundID = myItem.FundID;
						fundDBItem.BuyTime = buyTime;
						fundDBItem.AwardID = myItem.AwardID;
						int state = 1;
						bool flag = (from info in FundManager._fundAwardDic.Values
						where info.FundType == myItem.FundType && info.FundID == myItem.FundID && info.AwardID > myItem.AwardID
						select info).Any<FundAwardInfo>();
						bool flag2 = (from info in FundManager._fundDic.Values
						where info.FundType == myItem.FundType && info.FundID > myItem.FundID
						select info).Any<FundInfo>();
						if (!flag && !flag2)
						{
							state = 2;
						}
						fundDBItem.State = state;
						if (!FundManager.DBFundAward(client, fundDBItem))
						{
							fundData.State = -1;
							result = fundData;
						}
						else
						{
							FundAwardInfo fundAwardInfo = FundManager._fundAwardDic[myItem.AwardID];
							if (!FundManager.AddDiamone(client, fundAwardInfo.AwardIsBind, fundAwardInfo.AwardCount))
							{
								fundData.State = -1;
								result = fundData;
							}
							else
							{
								fundData.State = 1;
								if (fundDBItem.State == 2)
								{
									myItem.AwardType = 1;
									FundManager.CheckActivityTip(client);
									result = fundData;
								}
								else
								{
									FundManager.initFundAwardNext(client, myItem);
									FundManager.CheckActivityTip(client);
									fundData.FundType = fundType;
									result = fundData;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static bool AddDiamone(GameClient client, bool isBind, int diamond)
		{
			bool result;
			if (isBind)
			{
				result = GameManager.ClientMgr.AddUserGold(client, diamond, "基金绑钻");
			}
			else
			{
				result = GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, diamond, "基金钻石", ActivityTypes.None, "");
			}
			return result;
		}

		public static FundData GetFundData(GameClient client)
		{
			FundData myFundData;
			lock (client.ClientData.LockFund)
			{
				myFundData = client.ClientData.MyFundData;
			}
			return myFundData;
		}

		public static void initFundData(GameClient client)
		{
			lock (client.ClientData.LockFund)
			{
				FundData fundData = new FundData();
				if (!FundManager.IsGongNengOpened(client, false))
				{
					client.ClientData.MyFundData = fundData;
				}
				else
				{
					fundData.IsOpen = true;
					fundData.FundDic.Add(1, FundManager.initFundItem(client, EFund.ChangeLife));
					fundData.FundDic.Add(2, FundManager.initFundItem(client, EFund.Login));
					fundData.FundDic.Add(3, FundManager.initFundItem(client, EFund.Money));
					List<FundDBItem> list = FundManager.DBFundInfo(client);
					if (list == null)
					{
						client.ClientData.MyFundData = fundData;
					}
					else
					{
						foreach (FundDBItem fundDBItem in list)
						{
							if (fundData.FundDic.ContainsKey(fundDBItem.FundType) && fundDBItem.State > 0)
							{
								FundItem fundItem = fundData.FundDic[fundDBItem.FundType];
								fundItem.BuyType = 1;
								fundItem.BuyTime = fundDBItem.BuyTime;
								fundItem.FundID = fundDBItem.FundID;
								fundItem.AwardID = fundDBItem.AwardID;
								fundItem.AwardType = 1;
								if (fundItem.FundType == 3)
								{
									fundItem.Value1 = fundDBItem.Value1;
									fundItem.Value2 = fundDBItem.Value2;
								}
								if (fundItem.FundType == 2 && fundItem.BuyTime > DateTime.MinValue)
								{
									fundItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(fundItem.BuyTime) + 1;
								}
								if (fundDBItem.State == 1)
								{
									FundManager.initFundAwardNext(client, fundItem);
								}
							}
						}
						client.ClientData.MyFundData = fundData;
						FundManager.CheckActivityTip(client);
					}
				}
			}
		}

		private static FundItem initFundItem(GameClient client, EFund fundType)
		{
			bool flag = false;
			FundItem result;
			try
			{
				object lockFund;
				Monitor.Enter(lockFund = client.ClientData.LockFund, ref flag);
				FundInfo fundInfo = (from info in FundManager._fundDic.Values
				where info.FundType == (int)fundType
				orderby info.FundID
				select info).First<FundInfo>();
				FundItem fundItem = new FundItem();
				fundItem.FundID = fundInfo.FundID;
				fundItem.FundType = (int)fundType;
				fundItem.BuyType = 3;
				if (client.ClientData.VipLevel >= fundInfo.MinVip)
				{
					fundItem.BuyType = 2;
				}
				FundAwardInfo fundAwardInfo = (from info in FundManager._fundAwardDic.Values
				where info.FundType == (int)fundType && info.FundID == fundInfo.FundID
				orderby info.AwardID
				select info).First<FundAwardInfo>();
				fundItem.AwardID = fundAwardInfo.AwardID;
				fundItem.AwardType = 3;
				FundManager.checkFundItemValue(client, fundItem);
				result = fundItem;
			}
			finally
			{
				if (flag)
				{
					object lockFund;
					Monitor.Exit(lockFund);
				}
			}
			return result;
		}

		private static void checkFundItemValue(GameClient client, FundItem fundItem)
		{
			lock (client.ClientData.LockFund)
			{
				switch (fundItem.FundType)
				{
				case 1:
					fundItem.Value1 = client.ClientData.ChangeLifeCount;
					fundItem.Value2 = client.ClientData.Level;
					break;
				case 2:
					if (fundItem.BuyTime > DateTime.MinValue)
					{
						fundItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(fundItem.BuyTime) + 1;
					}
					break;
				}
			}
		}

		private static void initFundAwardNext(GameClient client, FundItem fundItem)
		{
			lock (client.ClientData.LockFund)
			{
				IOrderedEnumerable<FundAwardInfo> source = from info in FundManager._fundAwardDic.Values
				where info.FundType == fundItem.FundType && info.FundID == fundItem.FundID && info.AwardID > fundItem.AwardID
				orderby info.AwardID
				select info;
				if (source.Any<FundAwardInfo>())
				{
					FundAwardInfo fundAwardInfo = source.First<FundAwardInfo>();
					fundItem.AwardID = fundAwardInfo.AwardID;
					if (fundItem.Value1 >= fundAwardInfo.Value1 && fundItem.Value2 >= fundAwardInfo.Value2)
					{
						fundItem.AwardType = 2;
					}
					else
					{
						fundItem.AwardType = 3;
					}
					return;
				}
			}
			IOrderedEnumerable<FundInfo> source2 = from info in FundManager._fundDic.Values
			where info.FundType == fundItem.FundType && info.FundID > fundItem.FundID
			orderby info.FundID
			select info;
			if (source2.Any<FundInfo>())
			{
				FundInfo fundInfo = source2.First<FundInfo>();
				fundItem.FundID = fundInfo.FundID;
				fundItem.BuyTime = DateTime.MinValue;
				fundItem.BuyType = 3;
				if (client.ClientData.VipLevel >= fundInfo.MinVip)
				{
					fundItem.BuyType = 2;
				}
				FundAwardInfo fundAwardInfo = (from award in FundManager._fundAwardDic.Values
				where award.FundType == fundItem.FundType && award.FundID == fundItem.FundID && award.AwardID > fundItem.AwardID
				orderby award.AwardID
				select award).First<FundAwardInfo>();
				fundItem.AwardID = fundAwardInfo.AwardID;
				fundItem.AwardType = 3;
				fundItem.Value1 = 0;
				fundItem.Value2 = 0;
			}
		}

		private static List<FundDBItem> DBFundInfo(GameClient client)
		{
			List<FundDBItem> list = new List<FundDBItem>();
			return Global.sendToDB<List<FundDBItem>, int>(13116, client.ClientData.RoleID, client.ServerId);
		}

		private static bool DBFundBuy(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13117, item, client.ServerId);
		}

		private static bool DBFundAward(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13118, item, client.ServerId);
		}

		private static bool DBFundMoney(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13119, item, client.ServerId);
		}

		public static bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Fund") && GlobalNew.IsGongNengOpened(client, 75, hint);
		}

		public static void FundChangeLife(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				if (fundData.FundDic.ContainsKey(1))
				{
					FundItem fundItem = fundData.FundDic[1];
					fundItem.Value1 = client.ClientData.ChangeLifeCount;
					fundItem.Value2 = client.ClientData.Level;
					if (fundItem.BuyType == 1 && fundItem.AwardType == 3)
					{
						FundAwardInfo fundAwardInfo = FundManager._fundAwardDic[fundItem.AwardID];
						if (fundItem.Value1 >= fundAwardInfo.Value1 && fundItem.Value2 >= fundAwardInfo.Value2)
						{
							fundItem.AwardType = 2;
							FundManager.CheckActivityTip(client);
						}
					}
				}
			}
		}

		public static void FundVip(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				bool flag = false;
				foreach (FundItem fundItem in fundData.FundDic.Values)
				{
					if (fundItem.BuyType == 3)
					{
						FundInfo fundInfo = FundManager._fundDic[fundItem.FundID];
						if (client.ClientData.VipLevel >= fundInfo.MinVip)
						{
							fundItem.BuyType = 2;
							flag = true;
						}
					}
				}
				if (flag)
				{
					FundManager.CheckActivityTip(client);
				}
			}
		}

		public static void FundMoneyCost(GameClient client, int moneyCost)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				if (fundData.FundDic.ContainsKey(3))
				{
					FundItem fundItem = fundData.FundDic[3];
					if (fundItem.BuyType == 1)
					{
						if (FundManager.DBFundMoney(client, new FundDBItem
						{
							UserID = client.strUserID,
							RoleID = client.ClientData.RoleID,
							Value1 = 0,
							Value2 = moneyCost
						}))
						{
							fundItem.Value2 += moneyCost;
							FundAwardInfo fundAwardInfo = FundManager._fundAwardDic[fundItem.AwardID];
							if (fundItem.AwardType == 3 && fundItem.Value1 >= fundAwardInfo.Value1 && fundItem.Value2 >= fundAwardInfo.Value2)
							{
								fundItem.AwardType = 2;
								FundManager.CheckActivityTip(client);
							}
						}
					}
				}
			}
		}

		private static bool InitConfig()
		{
			string text = "";
			try
			{
				FundManager._fundDic.Clear();
				text = Global.GameResPath("Config/Fund/FundSet.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						FundInfo fundInfo = new FundInfo();
						fundInfo.FundID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MainID", "0"));
						fundInfo.FundType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "PageID", "0"));
						fundInfo.MinVip = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinVip", "0"));
						fundInfo.NextID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NextID", "0"));
						fundInfo.Price = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Price", "0"));
						FundManager._fundDic.Add(fundInfo.FundID, fundInfo);
					}
				}
				FundManager._fundAwardDic.Clear();
				text = Global.GameResPath("Config/Fund/Fund.xml");
				xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						FundAwardInfo fundAwardInfo = new FundAwardInfo();
						fundAwardInfo.AwardID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
						fundAwardInfo.FundID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MainID", "0"));
						fundAwardInfo.FundType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoalType", "0"));
						fundAwardInfo.AwardIsBind = (Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RewardType", "0")) > 0);
						fundAwardInfo.AwardCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RewardCount", "0"));
						string[] array = Global.GetDefAttributeStr(xelement2, "GoalNum", "0,0").Split(new char[]
						{
							','
						});
						switch (fundAwardInfo.FundType)
						{
						case 1:
							fundAwardInfo.Value1 = int.Parse(array[0]);
							fundAwardInfo.Value2 = int.Parse(array[1]);
							break;
						case 2:
							fundAwardInfo.Value1 = int.Parse(array[0]);
							break;
						case 3:
							fundAwardInfo.Value1 = int.Parse(array[0]);
							fundAwardInfo.Value2 = int.Parse(array[1]);
							break;
						}
						FundManager._fundAwardDic.Add(fundAwardInfo.AwardID, fundAwardInfo);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				return false;
			}
			return true;
		}

		private static void CheckActivityTip(GameClient client)
		{
			lock (client.ClientData.LockFund)
			{
				FundData fundData = FundManager.GetFundData(client);
				if (fundData.IsOpen)
				{
					bool flag2 = false;
					bool flag3 = false;
					List<int> list = new List<int>();
					foreach (FundItem fundItem in fundData.FundDic.Values)
					{
						bool flag4 = fundItem.BuyType == 2 || fundItem.AwardType == 2;
						flag3 = (flag3 || flag4);
						switch (fundItem.FundType)
						{
						case 1:
							flag2 |= client._IconStateMgr.AddFlushIconState(14106, flag4);
							break;
						case 2:
							flag2 |= client._IconStateMgr.AddFlushIconState(14107, flag4);
							break;
						case 3:
							flag2 |= client._IconStateMgr.AddFlushIconState(14108, flag4);
							break;
						}
					}
					flag2 |= client._IconStateMgr.AddFlushIconState(14109, flag3);
					if (flag2)
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		private static FundManager instance = new FundManager();

		private static Dictionary<int, FundInfo> _fundDic = new Dictionary<int, FundInfo>();

		private static Dictionary<int, FundAwardInfo> _fundAwardDic = new Dictionary<int, FundAwardInfo>();
	}
}
