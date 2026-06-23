using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class ProcessHorse
	{
		public static int ProcessHorseEnchance(GameClient client, int horseDbID, int extPropIndex, bool allowAutoBuy)
		{
			HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseDataByDbID)
			{
				result = -1;
			}
			else if (extPropIndex < 0 || extPropIndex >= 10)
			{
				result = -10;
			}
			else
			{
				int num = (int)GameManager.systemParamsList.GetParamValueIntByName("HorseEnchancseGoodsID", -1);
				if (num <= 0)
				{
					result = -20;
				}
				else
				{
					int[] horsePropIntArray = Global.HorseExtStr2IntArray(horseDataByDbID.PropsNum);
					int[] horsePropIntArray2 = Global.HorseExtStr2IntArray(horseDataByDbID.PropsVal);
					int horseExtFieldIntVal = Global.GetHorseExtFieldIntVal(horsePropIntArray, (HorseExtIndexes)extPropIndex);
					int horseExtFieldIntVal2 = Global.GetHorseExtFieldIntVal(horsePropIntArray2, (HorseExtIndexes)extPropIndex);
					SystemXmlItem horseEnchanceXmlNode = Global.GetHorseEnchanceXmlNode(horseExtFieldIntVal + 1, (HorseExtIndexes)extPropIndex);
					if (null == horseEnchanceXmlNode)
					{
						result = -35;
					}
					else
					{
						int horseBasePropVal = Global.GetHorseBasePropVal(horseDataByDbID.HorseID, (HorseExtIndexes)extPropIndex, null);
						int horsePropLimitVal = Global.GetHorsePropLimitVal(horseDataByDbID.HorseID, (HorseExtIndexes)extPropIndex);
						if (horseBasePropVal + horseExtFieldIntVal2 >= horsePropLimitVal)
						{
							result = -40;
						}
						else
						{
							int num2 = Global.GMax(horseEnchanceXmlNode.GetIntValue("UseMoney", -1), 0);
							if (client.ClientData.YinLiang < num2)
							{
								result = -60;
							}
							else
							{
								int num3 = 0;
								int num4 = Global.GMax(horseEnchanceXmlNode.GetIntValue("HanTie", -1), 0);
								int num5 = num4;
								if (Global.GetTotalGoodsCountByID(client, num) < num4)
								{
									if (!allowAutoBuy)
									{
										return -70;
									}
									num5 = Global.GetTotalGoodsCountByID(client, num);
									num3 = num4 - num5;
								}
								bool flag = false;
								bool flag2 = false;
								if (num5 > 0)
								{
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num5, false, out flag, out flag2, false))
									{
										return -70;
									}
								}
								if (num3 > 0)
								{
									int num6 = Global.SubUserMoneyForGoods(client, num, num3, "坐骑强化");
									if (num6 <= 0)
									{
										return num6;
									}
								}
								if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "坐骑强化", false))
								{
									result = -60;
								}
								else
								{
									int num7 = Global.GMax(horseEnchanceXmlNode.GetIntValue("SucceedRate", -1), 0);
									int randomNumber = Global.GetRandomNumber(0, 101);
									if (client.ClientData.TempHorseEnchanceRate != 1)
									{
										num7 *= client.ClientData.TempHorseEnchanceRate;
										num7 = Global.GMin(100, num7);
									}
									if (randomNumber > num7)
									{
										result = -1000;
									}
									else
									{
										int extValue = Global.GMax(horseEnchanceXmlNode.GetIntValue("PropVal", -1), 0);
										if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
										{
											Global.UpdateHorseDataProps(client, false);
										}
										int num8 = 0;
										if (Global.UpdateHorsePropsDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, (HorseExtIndexes)extPropIndex, extValue, 1) < 0)
										{
											num8 = -2000;
										}
										if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
										{
											Global.UpdateHorseDataProps(client, true);
											if (0 == num8)
											{
												client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseDataByDbID);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
											}
										}
										if (0 == num8)
										{
											if (Global.IsHorsePropsFull(horseDataByDbID))
											{
												Global.BroadcastHorseEnchanceOk(client, horseDataByDbID.HorseID);
											}
										}
										result = num8;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static int ProcessHorseQuickAllEnchance(GameClient client, int horseDbID)
		{
			HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseDataByDbID)
			{
				result = -1;
			}
			else if (Global.IsHorsePropsFull(horseDataByDbID))
			{
				result = -10;
			}
			else
			{
				int num = (int)GameManager.systemParamsList.GetParamValueIntByName("ChaoJiLianGuGoodsID", -1);
				if (num <= 0)
				{
					result = -20;
				}
				else
				{
					int quickHorseExtPropNeedYinLiang = Global.QuickHorseExtPropNeedYinLiang;
					if (client.ClientData.YinLiang < quickHorseExtPropNeedYinLiang)
					{
						result = -30;
					}
					else if (Global.GetTotalGoodsCountByID(client, num) < 1)
					{
						result = -40;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, 1, false, out flag, out flag2, false))
						{
							result = -60;
						}
						else if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, quickHorseExtPropNeedYinLiang, "坐骑快速全部属性强化", false))
						{
							result = -70;
						}
						else
						{
							if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
							{
								Global.UpdateHorseDataProps(client, false);
							}
							for (int i = 0; i < 10; i++)
							{
								int[] horsePropIntArray = Global.HorseExtStr2IntArray(horseDataByDbID.PropsNum);
								int[] horsePropIntArray2 = Global.HorseExtStr2IntArray(horseDataByDbID.PropsVal);
								int horseExtFieldIntVal = Global.GetHorseExtFieldIntVal(horsePropIntArray, (HorseExtIndexes)i);
								int horseExtFieldIntVal2 = Global.GetHorseExtFieldIntVal(horsePropIntArray2, (HorseExtIndexes)i);
								int horseBasePropVal = Global.GetHorseBasePropVal(horseDataByDbID.HorseID, (HorseExtIndexes)i, null);
								int horsePropLimitVal = Global.GetHorsePropLimitVal(horseDataByDbID.HorseID, (HorseExtIndexes)i);
								if (horseBasePropVal + horseExtFieldIntVal2 < horsePropLimitVal)
								{
									int horseEnchanceNum = Global.GetHorseEnchanceNum(horseDataByDbID.HorseID);
									int addNum = horseEnchanceNum - horseExtFieldIntVal;
									int extValue = horsePropLimitVal - horseBasePropVal - horseExtFieldIntVal2;
									Global.UpdateHorsePropsDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, (HorseExtIndexes)i, extValue, addNum);
								}
							}
							if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
							{
								Global.UpdateHorseDataProps(client, true);
								client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseDataByDbID);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							}
							if (Global.IsHorsePropsFull(horseDataByDbID))
							{
								Global.BroadcastHorseEnchanceOk(client, horseDataByDbID.HorseID);
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}

		public static int ProcessHorseUpgrade(GameClient client, int horseDbID, bool allowAutoBuy)
		{
			HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseDataByDbID)
			{
				result = -1;
			}
			else
			{
				SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseDataByDbID.HorseID + 1);
				if (null == horseUpXmlNode)
				{
					result = -35;
				}
				else
				{
					int num = 0;
					int l = 0;
					Global.ParseHorseJinJieFu(horseDataByDbID.HorseID, out num, out l, horseUpXmlNode);
					if (num <= 0)
					{
						result = -20;
					}
					else if (horseDataByDbID.HorseID >= Global.MaxHorseID)
					{
						result = -30;
					}
					else
					{
						int num2 = Global.GMax(horseUpXmlNode.GetIntValue("UseYinLiang", -1), 0);
						num2 = Global.RecalcNeedYinLiang(num2);
						if (client.ClientData.YinLiang < num2)
						{
							result = -60;
						}
						else
						{
							int num3 = 0;
							int num4 = Global.GMax(l, 0);
							int num5 = num4;
							if (Global.GetTotalGoodsCountByID(client, num) < num4)
							{
								if (!allowAutoBuy)
								{
									return -70;
								}
								num5 = Global.GetTotalGoodsCountByID(client, num);
								num3 = num4 - num5;
							}
							bool flag = false;
							bool flag2 = false;
							if (num5 > 0)
							{
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num5, false, out flag, out flag2, false))
								{
									return -70;
								}
							}
							if (num3 > 0)
							{
								int num6 = Global.SubUserMoneyForGoods(client, num, num3, "坐骑进阶");
								if (num6 <= 0)
								{
									return num6;
								}
							}
							if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "坐骑进阶", false))
							{
								result = -60;
							}
							else
							{
								int num7 = 110000 - horseUpXmlNode.GetIntValue("HorseOne", -1);
								int num8 = 110000 - horseUpXmlNode.GetIntValue("HorseTwo", -1);
								double doubleValue = horseUpXmlNode.GetDoubleValue("HorseThree");
								int horseFailedNum = Global.GetHorseFailedNum(horseDataByDbID);
								if (horseFailedNum < num8)
								{
									Global.AddHorseFailedNum(horseDataByDbID, 1);
									Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID);
									Global.AddRoleHorseUpgradeEvent(client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID, "失败");
									result = -1000;
								}
								else
								{
									if (horseFailedNum < num7 - 1)
									{
										int num9 = (int)(doubleValue * 10000.0);
										int randomNumber = Global.GetRandomNumber(1, 10001);
										if (client.ClientData.TempHorseUpLevelRate != 1)
										{
											num9 *= client.ClientData.TempHorseUpLevelRate;
											num9 = Global.GMin(10000, num9);
										}
										if (randomNumber > num9)
										{
											Global.AddHorseFailedNum(horseDataByDbID, 1);
											Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID);
											Global.AddRoleHorseUpgradeEvent(client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID, "失败");
											return -1000;
										}
									}
									result = ProcessHorse.ProcessHorseUpgradeNow(client, horseDbID, horseDataByDbID);
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static int ProcessHorseUpgradeNow(GameClient client, int horseDbID, HorseData horseData)
		{
			if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
			{
				Global.UpdateHorseDataProps(client, false);
			}
			int horseID = horseData.HorseID;
			int num = horseData.HorseID + 1;
			Global.AddHorseTempJiFen(horseData, 0);
			horseData.JinJieFailedDayID = TimeUtil.NowDateTime().DayOfYear;
			horseData.JinJieFailedNum = 0;
			int num2 = 0;
			if (Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, num, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID) < 0)
			{
				num2 = -2000;
			}
			Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, "成功");
			if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
			{
				Global.UpdateHorseDataProps(client, true);
				if (0 == num2)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				if (0 == num2)
				{
					client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseData);
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseDbID, horseData.HorseID, horseData.BodyID, all9Clients);
					Global.BroadcastHorseUpgradeOk(client, horseID, num);
				}
			}
			return 0;
		}

		public static int GetCurrentHorseBlessPoint(GameClient client)
		{
			int horseDbID = client.ClientData.HorseDbID;
			int result;
			if (horseDbID <= 0)
			{
				result = 0;
			}
			else
			{
				HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
				if (null == horseDataByDbID)
				{
					result = 0;
				}
				else
				{
					SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseDataByDbID.HorseID + 1);
					if (null == horseUpXmlNode)
					{
						result = 0;
					}
					else
					{
						int intValue = horseUpXmlNode.GetIntValue("BlessPoint", -1);
						result = intValue;
					}
				}
			}
			return result;
		}

		public static int ProcessAddHorseAwardLucky(GameClient client, int luckyValue, bool usedTimeLimited, string getType)
		{
			int result;
			if (0 == luckyValue)
			{
				result = 0;
			}
			else
			{
				int horseDbID = client.ClientData.HorseDbID;
				if (horseDbID <= 0)
				{
					result = -300;
				}
				else
				{
					HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
					if (null == horseDataByDbID)
					{
						result = -1;
					}
					else
					{
						SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseDataByDbID.HorseID + 1);
						if (null == horseUpXmlNode)
						{
							result = -35;
						}
						else
						{
							int intValue = horseUpXmlNode.GetIntValue("BlessPoint", -1);
							int horseFailedNum = Global.GetHorseFailedNum(horseDataByDbID);
							if (horseDataByDbID.HorseID >= Global.MaxHorseID)
							{
								result = -10;
							}
							else
							{
								int num = Global.GMin(luckyValue, intValue - horseFailedNum);
								num = Global.GMax(0, num);
								if (!usedTimeLimited)
								{
									Global.AddHorseFailedNum(horseDataByDbID, num);
								}
								else
								{
									Global.AddHorseTempJiFen(horseDataByDbID, num);
								}
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID);
								Global.AddRoleHorseUpgradeEvent(client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID, getType);
								result = num;
							}
						}
					}
				}
			}
			return result;
		}

		public static int ProcessAddHorseLucky(GameClient client, int horseDbID, int luckyGoodsID)
		{
			HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseDataByDbID)
			{
				result = -1;
			}
			else
			{
				SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseDataByDbID.HorseID + 1);
				if (null == horseUpXmlNode)
				{
					result = -35;
				}
				else
				{
					int num = 110000 - horseUpXmlNode.GetIntValue("HorseOne", -1);
					int num2 = 110000 - horseUpXmlNode.GetIntValue("HorseTwo", -1);
					int horseFailedNum = Global.GetHorseFailedNum(horseDataByDbID);
					if (horseFailedNum >= num - 1)
					{
						result = -100;
					}
					else
					{
						int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("AllHorseLuckyGoodsIDs", ',');
						int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("AllHorseLuckyGoodsIDsToLucky", ',');
						if (paramValueIntArrayByName == null || paramValueIntArrayByName2 == null || paramValueIntArrayByName.Length != paramValueIntArrayByName2.Length)
						{
							result = -2;
						}
						else if (horseDataByDbID.HorseID >= Global.MaxHorseID)
						{
							result = -10;
						}
						else if (Global.GetTotalGoodsCountByID(client, luckyGoodsID) <= 0)
						{
							result = -20;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out flag, out flag2, false))
							{
								result = -30;
							}
							else
							{
								int num3 = 0;
								for (int i = 0; i < paramValueIntArrayByName.Length; i++)
								{
									if (paramValueIntArrayByName[i] == luckyGoodsID)
									{
										num3 = paramValueIntArrayByName2[i];
										break;
									}
								}
								num3 = Global.GMax(0, num3);
								Global.AddHorseFailedNum(horseDataByDbID, num3);
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID);
								Global.AddRoleHorseUpgradeEvent(client, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.JinJieFailedNum, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID, "祝福丹");
								result = num3;
							}
						}
					}
				}
			}
			return result;
		}
	}
}
