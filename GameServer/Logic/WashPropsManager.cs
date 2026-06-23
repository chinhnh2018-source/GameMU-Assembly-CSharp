using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Goods;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	public static class WashPropsManager
	{
		public static bool initialize()
		{
			WashPropsManager.InitConfig();
			TCPCmdDispatcher.getInstance().registerProcessor(645, 5, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS));
			TCPCmdDispatcher.getInstance().registerProcessor(646, 4, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT));
			return true;
		}

		public static bool startup()
		{
			return true;
		}

		public static bool showdown()
		{
			return true;
		}

		public static bool destroy()
		{
			return true;
		}

		public static void InitConfig()
		{
			WashPropsManager.XiLianTypeDict.Clear();
			WashPropsManager.XiLianShuXingDict.Clear();
			WashPropsManager.PropsIds.Clear();
			WashPropsManager.PropsIds.Add(13);
			WashPropsManager.PropsIds.Add(27);
			WashPropsManager.PropsIds.Add(38);
			WashPropsManager.PropsIds.Add(45);
			WashPropsManager.PropsIds.Add(46);
			WashPropsManager.PropsIds.Add(18);
			WashPropsManager.PropsIds.Add(19);
			WashPropsManager.PropsIds.Add(44);
			string text = "Config/XiLianType.xml";
			try
			{
				string text2 = Global.GameResPath("Config/XiLianType.xml");
				XElement xelement = XElement.Load(text2);
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text2));
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					XiLianType xiLianType = new XiLianType();
					xiLianType.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					xiLianType.Color = Global.GetSafeAttributeStr(xelement2, "Color");
					xiLianType.ShuXingNum = (int)Global.GetSafeAttributeLong(xelement2, "ShuXingNum");
					xiLianType.Text = Global.GetSafeAttributeStr(xelement2, "Text");
					xiLianType.FirstShuXing = 0.0;
					xiLianType.ShuXingLimitMultiplying = Global.GetSafeAttributeDouble(xelement2, "Multiplying");
					WashPropsManager.XiLianTypeDict.Add(xiLianType.ID, xiLianType);
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。", text));
			}
			text = "Config/XiLianShuXing.xml";
			try
			{
				string text2 = Global.GameResPath(text);
				XElement xelement = XElement.Load(text2);
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text2));
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					WashPropsManager.ParseXiLianShuXing(xelement2);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
			}
			try
			{
				WashPropsManager.XiLianChuanChengGoodsRates = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengGoodsRates", ',');
				WashPropsManager.XiLianChuanChengXiaoHaoJinBi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoJinBi", ',');
				WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoZhuanShi", ',');
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
			}
		}

		public static void ParseXiLianShuXing(XElement node)
		{
			XiLianShuXing xiLianShuXing = new XiLianShuXing();
			xiLianShuXing.ID = (int)Global.GetSafeAttributeLong(node, "ID");
			xiLianShuXing.Name = Global.GetSafeAttributeStr(node, "Name");
			xiLianShuXing.NeedJinBi = (int)Global.GetSafeAttributeLong(node, "NeedJinBi");
			xiLianShuXing.NeedZuanShi = (int)Global.GetSafeAttributeLong(node, "NeedZuanShi");
			long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(node, "NeedGoods", 2);
			if (null != safeAttributeLongArray)
			{
				xiLianShuXing.NeedGoodsIDs.Add((int)safeAttributeLongArray[0]);
				xiLianShuXing.NeedGoodsCounts.Add((int)safeAttributeLongArray[1]);
			}
			foreach (int num in WashPropsManager.PropsIds)
			{
				ExtPropIndexes extPropIndexes = (ExtPropIndexes)num;
				string attribute = string.Format("JinBi{0}", extPropIndexes.ToString());
				safeAttributeLongArray = Global.GetSafeAttributeLongArray(node, attribute, -1);
				if (safeAttributeLongArray != null && safeAttributeLongArray.Length > 0)
				{
					xiLianShuXing.PromoteJinBiRange.Add(num, new List<long>(safeAttributeLongArray));
				}
				attribute = string.Format("ZuanShi{0}", extPropIndexes.ToString());
				safeAttributeLongArray = Global.GetSafeAttributeLongArray(node, attribute, -1);
				if (safeAttributeLongArray != null && safeAttributeLongArray.Length > 0)
				{
					xiLianShuXing.PromoteZuanShiRange.Add(num, new List<long>(safeAttributeLongArray));
				}
				xiLianShuXing.PromotePropLimit.Add((int)extPropIndexes, (int)Global.GetSafeAttributeLong(node, extPropIndexes.ToString()));
			}
			WashPropsManager.XiLianShuXingDict.Add(xiLianShuXing.ID, xiLianShuXing);
		}

		public static bool WashProps(GameClient client, int dbid, int washIndex, bool firstUseBinding, int moneyType)
		{
			int cmdId = 645;
			List<int> list = new List<int>();
			list.Add(washIndex);
			list.Add(1);
			list.Add(dbid);
			list.Add(0);
			int num = 0;
			double num2 = 1.0;
			object obj = null;
			if (HuodongCachingMgr.GetJieriFuLiActivity().IsOpened(EJieRiFuLiType.OneDiscountDiamond, out obj))
			{
				List<double> list2 = (List<double>)obj;
				if (list2.Count != 2)
				{
					list[1] = -3;
					client.sendCmd<List<int>>(cmdId, list, false);
					return true;
				}
				if (moneyType == 0)
				{
					num2 = list2[0];
				}
				else if (moneyType == 1)
				{
					num2 = list2[1];
				}
			}
			bool result;
			if (washIndex > -2 || washIndex < -5)
			{
				list[1] = -5;
				client.sendCmd<List<int>>(cmdId, list, false);
				result = true;
			}
			else if (moneyType < 0 || moneyType > 1)
			{
				list[1] = -14;
				client.sendCmd<List<int>>(cmdId, list, false);
				result = true;
			}
			else
			{
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, dbid);
				SystemXmlItem systemXmlItem;
				if (null == goodsByDbID)
				{
					list[1] = -1;
					client.sendCmd<List<int>>(cmdId, list, false);
					result = true;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem))
				{
					list[1] = -3;
					client.sendCmd<List<int>>(cmdId, list, false);
					result = true;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("XiLian", -1);
					XiLianShuXing xiLianShuXing;
					if (!WashPropsManager.XiLianShuXingDict.TryGetValue(intValue, out xiLianShuXing))
					{
						list[1] = -3;
						client.sendCmd<List<int>>(cmdId, list, false);
						result = true;
					}
					else
					{
						if (washIndex == -2 || washIndex == -1)
						{
							if (moneyType == 0)
							{
								if (xiLianShuXing.NeedJinBi < 1)
								{
									list[1] = -3;
									client.sendCmd<List<int>>(cmdId, list, false);
									return true;
								}
								num = Math.Max((int)((double)xiLianShuXing.NeedJinBi * num2), 1);
								if (client.ClientData.Money1 + client.ClientData.YinLiang < num)
								{
									list[1] = -9;
									client.sendCmd<List<int>>(cmdId, list, false);
									return true;
								}
							}
							else if (moneyType == 1)
							{
								if (xiLianShuXing.NeedZuanShi < 1)
								{
									list[1] = -3;
									client.sendCmd<List<int>>(cmdId, list, false);
									return true;
								}
								num = Math.Max((int)((double)xiLianShuXing.NeedZuanShi * num2), 1);
								if (client.ClientData.UserMoney < num && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, num))
								{
									list[1] = -10;
									client.sendCmd<List<int>>(cmdId, list, false);
									return true;
								}
							}
						}
						if (washIndex == -1)
						{
							if (goodsByDbID.WashProps != null && goodsByDbID.WashProps.Count > 0)
							{
								list[1] = -5;
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								int equipColor = Global.GetEquipColor(goodsByDbID);
								XiLianType xiLianType;
								if (equipColor <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(equipColor, out xiLianType) || xiLianType.ShuXingNum <= 0)
								{
									list[1] = -5;
									client.sendCmd<List<int>>(cmdId, list, false);
									result = true;
								}
								else
								{
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = dbid
									};
									updateGoodsArgs.WashProps = new List<int>();
									if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
									{
										client.ClientData._ReplaceExtArg.Reset();
										if (SingletonTemplate<GoodsReplaceManager>.Instance().NeedCheckSuit(Global.GetGoodsCatetoriy(goodsByDbID.GoodsID)))
										{
											client.ClientData._ReplaceExtArg.CurrEquipSuit = Global.GetEquipGoodsSuitID(goodsByDbID.GoodsID);
										}
										GoodsReplaceResult replaceResult = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, xiLianShuXing.NeedGoodsIDs[0]);
										if (replaceResult == null || replaceResult.TotalGoodsCnt() < xiLianShuXing.NeedGoodsCounts[0])
										{
											list[1] = -6;
											client.sendCmd<List<int>>(cmdId, list, false);
											return true;
										}
										List<GoodsReplaceResult.ReplaceItem> list3 = new List<GoodsReplaceResult.ReplaceItem>();
										if (firstUseBinding)
										{
											list3.AddRange(replaceResult.BindList);
											list3.Add(replaceResult.OriginBindGoods);
											list3.AddRange(replaceResult.UnBindList);
											list3.Add(replaceResult.OriginUnBindGoods);
										}
										else
										{
											list3.AddRange(replaceResult.UnBindList);
											list3.Add(replaceResult.OriginUnBindGoods);
											list3.AddRange(replaceResult.BindList);
											list3.Add(replaceResult.OriginBindGoods);
										}
										int num3 = xiLianShuXing.NeedGoodsCounts[0];
										foreach (GoodsReplaceResult.ReplaceItem replaceItem in list3)
										{
											if (replaceItem.GoodsCnt > 0)
											{
												int num4 = Math.Min(num3, replaceItem.GoodsCnt);
												if (num4 <= 0)
												{
													break;
												}
												bool flag = false;
												bool flag2 = false;
												bool flag3 = false;
												if (replaceItem.IsBind)
												{
													if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num4, false, out flag, out flag2, false))
													{
														flag3 = true;
													}
													updateGoodsArgs.Binding = 1;
												}
												else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num4, false, out flag, out flag2, false))
												{
													flag3 = true;
												}
												if (flag3)
												{
													list[1] = -6;
													client.sendCmd<List<int>>(cmdId, list, false);
													return true;
												}
												num3 -= num4;
											}
										}
									}
									for (int i = 0; i < xiLianType.ShuXingNum; i++)
									{
										int randomNumber = Global.GetRandomNumber(0, WashPropsManager.PropsIds.Count);
										int num5 = WashPropsManager.PropsIds[randomNumber];
										int num6 = xiLianShuXing.PromotePropLimit[num5];
										int num7 = (int)Math.Ceiling((double)num6 * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);
										updateGoodsArgs.WashProps.Add(num5);
										updateGoodsArgs.WashProps.Add(num7);
									}
									Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs, true);
									Global.ModRoleGoodsEvent(client, goodsByDbID, 0, "装备洗炼激活", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID.GoodsID, (long)goodsByDbID.Id, 0, goodsByDbID.GCount, "装备洗炼激活");
									list[3] = ((goodsByDbID.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
									list.AddRange(goodsByDbID.WashProps);
									client.sendCmd<List<int>>(cmdId, list, false);
									result = true;
								}
							}
						}
						else if (washIndex == -2)
						{
							UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
							{
								RoleID = client.ClientData.RoleID,
								DbID = dbid
							};
							if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
							{
								client.ClientData._ReplaceExtArg.Reset();
								if (SingletonTemplate<GoodsReplaceManager>.Instance().NeedCheckSuit(Global.GetGoodsCatetoriy(goodsByDbID.GoodsID)))
								{
									client.ClientData._ReplaceExtArg.CurrEquipSuit = Global.GetEquipGoodsSuitID(goodsByDbID.GoodsID);
								}
								GoodsReplaceResult replaceResult = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, xiLianShuXing.NeedGoodsIDs[0]);
								if (replaceResult == null || replaceResult.TotalGoodsCnt() < xiLianShuXing.NeedGoodsCounts[0])
								{
									list[1] = -6;
									client.sendCmd<List<int>>(cmdId, list, false);
									return true;
								}
								List<GoodsReplaceResult.ReplaceItem> list3 = new List<GoodsReplaceResult.ReplaceItem>();
								if (firstUseBinding)
								{
									list3.AddRange(replaceResult.BindList);
									list3.Add(replaceResult.OriginBindGoods);
									list3.AddRange(replaceResult.UnBindList);
									list3.Add(replaceResult.OriginUnBindGoods);
								}
								else
								{
									list3.AddRange(replaceResult.UnBindList);
									list3.Add(replaceResult.OriginUnBindGoods);
									list3.AddRange(replaceResult.BindList);
									list3.Add(replaceResult.OriginBindGoods);
								}
								int num3 = xiLianShuXing.NeedGoodsCounts[0];
								foreach (GoodsReplaceResult.ReplaceItem replaceItem in list3)
								{
									if (replaceItem.GoodsCnt > 0)
									{
										int num4 = Math.Min(num3, replaceItem.GoodsCnt);
										if (num4 <= 0)
										{
											break;
										}
										bool flag = false;
										bool flag2 = false;
										bool flag3 = false;
										if (replaceItem.IsBind)
										{
											if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num4, false, out flag, out flag2, false))
											{
												flag3 = true;
											}
											updateGoodsArgs.Binding = 1;
										}
										else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num4, false, out flag, out flag2, false))
										{
											flag3 = true;
										}
										if (flag3)
										{
											list[1] = -6;
											client.sendCmd<List<int>>(cmdId, list, false);
											return true;
										}
										num3 -= num4;
									}
								}
							}
							if (moneyType == 0)
							{
								Global.SubBindTongQianAndTongQian(client, num, "洗炼");
							}
							else if (moneyType == 1)
							{
								GameManager.ClientMgr.SubUserMoney(client, num, "洗炼", true, true, true, true, DaiBiSySType.ZhuangBeiPeiYang);
							}
							int equipColor = Global.GetEquipColor(goodsByDbID);
							XiLianType xiLianType;
							if (equipColor <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(equipColor, out xiLianType) || xiLianType.ShuXingNum <= 0)
							{
								list[1] = -5;
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								if (goodsByDbID.WashProps == null || goodsByDbID.WashProps.Count == 0)
								{
									List<int> list4 = new List<int>(xiLianType.ShuXingNum * 2);
									int num8 = xiLianType.ShuXingNum;
									foreach (KeyValuePair<int, int> keyValuePair in xiLianShuXing.PromotePropLimit)
									{
										if (keyValuePair.Value > 0)
										{
											list4.Add(keyValuePair.Key);
											list4.Add(0);
											if (--num8 <= 0)
											{
												break;
											}
										}
									}
									updateGoodsArgs.WashProps = list4;
								}
								else
								{
									updateGoodsArgs.WashProps = new List<int>(goodsByDbID.WashProps);
								}
								for (int i = 0; i < updateGoodsArgs.WashProps.Count; i += 2)
								{
									int num5 = updateGoodsArgs.WashProps[i];
									if (!xiLianShuXing.PromotePropLimit.ContainsKey(num5))
									{
										list[1] = -3;
										client.sendCmd<List<int>>(cmdId, list, false);
										return true;
									}
									int num7 = updateGoodsArgs.WashProps[i + 1];
									int num6 = (int)((double)xiLianShuXing.PromotePropLimit[num5] * xiLianType.ShuXingLimitMultiplying);
									if (moneyType == 0)
									{
										int randomNumber2 = Global.GetRandomNumber(0, xiLianShuXing.PromoteJinBiRange[num5].Count);
										num7 += (int)xiLianShuXing.PromoteJinBiRange[num5][randomNumber2];
									}
									else if (moneyType == 1)
									{
										int randomNumber2 = Global.GetRandomNumber(0, xiLianShuXing.PromoteZuanShiRange[num5].Count);
										num7 += (int)xiLianShuXing.PromoteZuanShiRange[num5][randomNumber2];
									}
									num7 = Global.Clamp(num7, 0, num6);
									updateGoodsArgs.WashProps[i + 1] = num7;
								}
								client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
								client.ClientData.TempWashPropOperationIndex = washIndex;
								list[3] = ((goodsByDbID.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
								list.AddRange(updateGoodsArgs.WashProps);
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
						}
						else if (washIndex == -3)
						{
							UpdateGoodsArgs updateGoodsArgs2;
							if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsByDbID.Id, out updateGoodsArgs2))
							{
								list[1] = -2;
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs2, true);
								Global.ModRoleGoodsEvent(client, goodsByDbID, 0, "装备洗炼", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID.GoodsID, (long)goodsByDbID.Id, 0, goodsByDbID.GCount, "装备洗炼");
								client.ClientData.TempWashPropsDict.Remove(goodsByDbID.Id);
								list[3] = ((goodsByDbID.Binding > 0) ? 1 : 0);
								list.AddRange(goodsByDbID.WashProps);
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
						}
						else if (washIndex == -4)
						{
							client.ClientData.TempWashPropsDict.Remove(dbid);
							client.sendCmd<List<int>>(cmdId, list, false);
							result = true;
						}
						else if (washIndex == -5)
						{
							UpdateGoodsArgs updateGoodsArgs2;
							if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsByDbID.Id, out updateGoodsArgs2))
							{
								list[1] = -4;
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								list[0] = client.ClientData.TempWashPropOperationIndex;
								list[2] = updateGoodsArgs2.DbID;
								list[3] = (updateGoodsArgs2.Binding | goodsByDbID.Binding);
								list.AddRange(updateGoodsArgs2.WashProps);
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
						}
						else if (washIndex >= 0)
						{
							if (washIndex < 0 || goodsByDbID.WashProps == null || goodsByDbID.WashProps.Count / 2 <= washIndex)
							{
								list[1] = -2;
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								int equipColor = Global.GetEquipColor(goodsByDbID);
								XiLianType xiLianType;
								if (equipColor <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(equipColor, out xiLianType) || xiLianType.ShuXingNum <= washIndex)
								{
									list[1] = -5;
									client.sendCmd<List<int>>(cmdId, list, false);
									result = true;
								}
								else
								{
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = dbid
									};
									updateGoodsArgs.WashProps = new List<int>(goodsByDbID.WashProps);
									if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
									{
										bool flag4 = firstUseBinding;
										bool flag5 = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out flag4, out flag5, false))
										{
											list[1] = -6;
											client.sendCmd<List<int>>(cmdId, list, false);
											return true;
										}
										if (goodsByDbID.Binding == 0 && flag4)
										{
											updateGoodsArgs.Binding = 1;
										}
									}
									int randomNumber = Global.GetRandomNumber(0, WashPropsManager.PropsIds.Count);
									int num5 = WashPropsManager.PropsIds[randomNumber];
									int num6 = xiLianShuXing.PromotePropLimit[num5];
									int num7 = (int)Math.Ceiling((double)num6 * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);
									updateGoodsArgs.WashProps[washIndex * 2] = num5;
									updateGoodsArgs.WashProps[washIndex * 2 + 1] = num7;
									client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
									client.ClientData.TempWashPropOperationIndex = washIndex;
									list[3] = ((goodsByDbID.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
									list.Add(num5);
									list.Add(num7);
									client.sendCmd<List<int>>(cmdId, list, false);
									result = true;
								}
							}
						}
						else
						{
							list[1] = -2;
							client.sendCmd<List<int>>(cmdId, list, false);
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static bool WashPropsInherit(GameClient client, int leftGoodsDbID, int rightGoodsDbID, int nSubMoneyType)
		{
			int roleID = client.ClientData.RoleID;
			int cmdId = 646;
			List<int> list = new List<int>();
			list.Add(1);
			list.Add(leftGoodsDbID);
			list.Add(rightGoodsDbID);
			list.Add(0);
			GoodsData goodsByDbID = Global.GetGoodsByDbID(client, leftGoodsDbID);
			bool result;
			if (null == goodsByDbID)
			{
				list[0] = -1;
				client.sendCmd<List<int>>(cmdId, list, false);
				result = true;
			}
			else
			{
				GoodsData goodsByDbID2 = Global.GetGoodsByDbID(client, rightGoodsDbID);
				SystemXmlItem systemXmlItem;
				if (null == goodsByDbID2)
				{
					list[0] = -1;
					client.sendCmd<List<int>>(cmdId, list, false);
					result = true;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID2.GoodsID, out systemXmlItem))
				{
					list.Add(-3);
					client.sendCmd<List<int>>(cmdId, list, false);
					result = true;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("XiLian", -1);
					XiLianShuXing xiLianShuXing;
					if (!WashPropsManager.XiLianShuXingDict.TryGetValue(intValue, out xiLianShuXing))
					{
						list.Add(-3);
						client.sendCmd<List<int>>(cmdId, list, false);
						result = true;
					}
					else
					{
						int equipColor = Global.GetEquipColor(goodsByDbID);
						int equipColor2 = Global.GetEquipColor(goodsByDbID2);
						if (equipColor < 2 || equipColor2 < 2 || null == goodsByDbID.WashProps)
						{
							list[0] = -12;
							client.sendCmd<List<int>>(cmdId, list, false);
							result = true;
						}
						else
						{
							XiLianType xiLianType = null;
							if (!WashPropsManager.XiLianTypeDict.TryGetValue(equipColor2, out xiLianType))
							{
								list.Add(-3);
								client.sendCmd<List<int>>(cmdId, list, false);
								result = true;
							}
							else
							{
								int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsByDbID.GoodsID);
								int mainOccupationByGoodsID2 = Global.GetMainOccupationByGoodsID(goodsByDbID2.GoodsID);
								if (mainOccupationByGoodsID != mainOccupationByGoodsID2)
								{
									list[0] = -12;
									client.sendCmd<List<int>>(cmdId, list, false);
									result = true;
								}
								else
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByDbID.GoodsID);
									int goodsCatetoriy2 = Global.GetGoodsCatetoriy(goodsByDbID2.GoodsID);
									int num = GoodsUtil.CanUpgradeInhert(goodsCatetoriy, goodsCatetoriy2, 8);
									if (num < 0)
									{
										list[0] = -13;
										client.sendCmd<List<int>>(cmdId, list, false);
										result = true;
									}
									else if (goodsByDbID.Site != 0 || goodsByDbID2.Site != 0)
									{
										list[0] = -8;
										client.sendCmd<List<int>>(cmdId, list, false);
										result = true;
									}
									else if (nSubMoneyType < 1 || nSubMoneyType > 2)
									{
										list[0] = -14;
										client.sendCmd<List<int>>(cmdId, list, false);
										result = true;
									}
									else
									{
										if (nSubMoneyType == 1)
										{
											if (WashPropsManager.XiLianChuanChengXiaoHaoJinBi[0] > 0 && !Global.SubBindTongQianAndTongQian(client, WashPropsManager.XiLianChuanChengXiaoHaoJinBi[0], "洗练属性传承"))
											{
												list[0] = -9;
												client.sendCmd<List<int>>(cmdId, list, false);
												return true;
											}
										}
										else if (nSubMoneyType == 2)
										{
											if (WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi[0] > 0 && !GameManager.ClientMgr.SubUserMoney(client, WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi[0], "洗练属性传承", true, true, true, true, DaiBiSySType.ZhuangBeiChuanCheng))
											{
												list[0] = -10;
												client.sendCmd<List<int>>(cmdId, list, false);
												return true;
											}
										}
										int num2 = 0;
										if (goodsByDbID2.Binding == 1 || goodsByDbID.Binding == 1)
										{
											num2 = 1;
										}
										int randomNumber = Global.GetRandomNumber(0, 101);
										if (WashPropsManager.XiLianChuanChengGoodsRates != null && randomNumber > WashPropsManager.XiLianChuanChengGoodsRates[equipColor])
										{
											list[0] = -11;
											client.sendCmd<List<int>>(cmdId, list, false);
											result = true;
										}
										else
										{
											UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
											{
												RoleID = roleID,
												DbID = leftGoodsDbID
											};
											updateGoodsArgs.WashProps = new List<int>(goodsByDbID.WashProps);
											UpdateGoodsArgs updateGoodsArgs2 = new UpdateGoodsArgs
											{
												RoleID = roleID,
												DbID = rightGoodsDbID
											};
											if (goodsByDbID2.WashProps == null || goodsByDbID2.WashProps.Count == 0)
											{
												updateGoodsArgs2.WashProps = new List<int>(xiLianType.ShuXingNum * 2);
												int num3 = 0;
												foreach (KeyValuePair<int, int> keyValuePair in xiLianShuXing.PromotePropLimit)
												{
													if (keyValuePair.Value > 0)
													{
														updateGoodsArgs2.WashProps.Add(keyValuePair.Key);
														updateGoodsArgs2.WashProps.Add(0);
														if (++num3 >= xiLianType.ShuXingNum)
														{
															break;
														}
													}
												}
											}
											else
											{
												updateGoodsArgs2.WashProps = new List<int>(goodsByDbID2.WashProps);
											}
											List<int> list2 = new List<int>();
											for (int i = 0; i < updateGoodsArgs2.WashProps.Count - 1; i += 2)
											{
												int num4 = updateGoodsArgs2.WashProps[i];
												int num5 = 0;
												if (list2.Contains(num4) || !xiLianShuXing.PromotePropLimit.TryGetValue(num4, out num5) || num5 <= 0)
												{
													foreach (KeyValuePair<int, int> keyValuePair in xiLianShuXing.PromotePropLimit)
													{
														if (keyValuePair.Value > 0)
														{
															updateGoodsArgs2.WashProps[i] = keyValuePair.Key;
															updateGoodsArgs2.WashProps[i + 1] = 0;
															list2.Add(keyValuePair.Key);
														}
													}
												}
												else
												{
													list2.Add(num4);
												}
											}
											List<int> list3 = new List<int>();
											for (int i = 0; i < updateGoodsArgs.WashProps.Count - 1; i += 2)
											{
												for (int j = 0; j < updateGoodsArgs2.WashProps.Count - 1; j += 2)
												{
													if (updateGoodsArgs.WashProps[i] == updateGoodsArgs2.WashProps[j])
													{
														int num4 = updateGoodsArgs.WashProps[i];
														int num5 = 0;
														list3.Add(num4);
														updateGoodsArgs2.WashProps[j] = num4;
														if (xiLianShuXing.PromotePropLimit.TryGetValue(num4, out num5))
														{
															updateGoodsArgs2.WashProps[j + 1] = (int)Math.Round(Global.Clamp((double)updateGoodsArgs.WashProps[i + 1], 0.0, (double)num5 * xiLianType.ShuXingLimitMultiplying));
														}
														else
														{
															updateGoodsArgs2.WashProps[j + 1] = 0;
														}
													}
												}
											}
											for (int i = 0; i < updateGoodsArgs.WashProps.Count - 1; i += 2)
											{
												if (!list3.Contains(updateGoodsArgs.WashProps[i]))
												{
													list3.Add(updateGoodsArgs.WashProps[i]);
													for (int j = 0; j < updateGoodsArgs2.WashProps.Count - 1; j += 2)
													{
														if (!list3.Contains(updateGoodsArgs2.WashProps[j]))
														{
															list3.Add(updateGoodsArgs2.WashProps[j]);
															int num4 = updateGoodsArgs2.WashProps[j];
															int num5 = 0;
															updateGoodsArgs2.WashProps[i] = num4;
															if (xiLianShuXing.PromotePropLimit.TryGetValue(num4, out num5))
															{
																if (updateGoodsArgs.WashProps[i] == 44 && updateGoodsArgs2.WashProps[j] == 45)
																{
																	updateGoodsArgs2.WashProps[j + 1] = (int)Math.Floor(Global.Clamp((double)(updateGoodsArgs.WashProps[i + 1] * 10), 0.0, (double)num5 * xiLianType.ShuXingLimitMultiplying));
																}
																else if (updateGoodsArgs.WashProps[i] == 45 && updateGoodsArgs2.WashProps[j] == 44)
																{
																	updateGoodsArgs2.WashProps[j + 1] = (int)Math.Floor(Global.Clamp((double)(updateGoodsArgs.WashProps[i + 1] / 10), 0.0, (double)num5 * xiLianType.ShuXingLimitMultiplying));
																}
																else
																{
																	updateGoodsArgs2.WashProps[j + 1] = 0;
																}
															}
															else
															{
																updateGoodsArgs2.WashProps[j + 1] = 0;
															}
														}
													}
												}
											}
											updateGoodsArgs.WashProps = null;
											updateGoodsArgs2.Binding = num2;
											client.ClientData.TempWashPropsDict.Remove(updateGoodsArgs.DbID);
											client.ClientData.TempWashPropsDict.Remove(updateGoodsArgs2.DbID);
											if (Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs, true) < 0)
											{
												list[0] = -15;
												client.sendCmd<List<int>>(cmdId, list, false);
												result = true;
											}
											else if (Global.UpdateGoodsProp(client, goodsByDbID2, updateGoodsArgs2, true) < 0)
											{
												list[0] = -15;
												client.sendCmd<List<int>>(cmdId, list, false);
												result = true;
											}
											else
											{
												Global.ModRoleGoodsEvent(client, goodsByDbID, 0, "装备洗炼传承_提供方", false);
												Global.ModRoleGoodsEvent(client, goodsByDbID2, 0, "装备洗炼传承_接受方", false);
												EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID.GoodsID, (long)goodsByDbID.Id, 0, goodsByDbID.GCount, "装备洗炼传承_提供方");
												EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID2.GoodsID, (long)goodsByDbID2.Id, 0, goodsByDbID2.GCount, "装备洗炼传承_接受方");
												if (goodsByDbID.Using > 0 || goodsByDbID2.Using > 0)
												{
													Global.RefreshEquipPropAndNotify(client);
												}
												GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.EquipChuanChengTimes));
												ProcessTask.ProcessAddTaskVal(client, TaskTypes.EquipChuanCheng, -1, 1, new object[0]);
												list[3] = num2;
												list.AddRange(goodsByDbID2.WashProps);
												client.sendCmd<List<int>>(cmdId, list, false);
												result = true;
											}
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

		public static List<int> WashPropsMax(params List<int>[] args)
		{
			List<int> list = null;
			if (null != args)
			{
				foreach (List<int> list2 in args)
				{
					if (null != list2)
					{
						if (null == list)
						{
							list = list2.GetRange(0, list2.Count);
						}
						else
						{
							for (int j = 0; j < list.Count - 1; j += 2)
							{
								for (int k = 0; k < list2.Count - 1; k += 2)
								{
									if (list[j] == list2[k] && list2[k + 1] > list[j + 1])
									{
										list[j + 1] = list2[k + 1];
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		private static int[] XiLianChuanChengGoodsRates = new int[5];

		private static int[] XiLianChuanChengXiaoHaoJinBi = new int[16];

		private static int[] XiLianChuanChengXiaoHaoZhuanShi = new int[16];

		private static List<int> PropsIds = new List<int>();

		private static Dictionary<int, XiLianType> XiLianTypeDict = new Dictionary<int, XiLianType>();

		private static Dictionary<int, XiLianShuXing> XiLianShuXingDict = new Dictionary<int, XiLianShuXing>();

		public static class WashOperations
		{
			public const int WashProps = 0;

			public const int WashPropsActive = -1;

			public const int WashPropsQuantity = -2;

			public const int WashPropsCommit = -3;

			public const int WashPropsCancle = -4;

			public const int WashPropsQuery = -5;
		}
	}
}
