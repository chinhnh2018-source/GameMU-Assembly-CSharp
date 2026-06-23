using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	internal class HolyItemManager : ICmdProcessorEx, ICmdProcessor
	{
		public static HolyItemManager getInstance()
		{
			return HolyItemManager.instance;
		}

		public void Initialize()
		{
			SystemXmlItems systemXmlItems = new SystemXmlItems();
			systemXmlItems.LoadFromXMlFile("Config/BuJian.xml", "", "ID", 0);
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in systemXmlItems.SystemXmlItemDict)
			{
				HolyPartInfo holyPartInfo = new HolyPartInfo();
				holyPartInfo.m_nCostBandJinBi = keyValuePair.Value.GetIntValue("CostBandJinBi", -1);
				holyPartInfo.m_sSuccessProbability = Convert.ToSByte(keyValuePair.Value.GetDoubleValue("SuccessProbability") * 100.0);
				if (holyPartInfo.m_sSuccessProbability < 0)
				{
					holyPartInfo.m_sSuccessProbability = -1;
				}
				string[] array = keyValuePair.Value.GetStringValue("NeedGoods").Split(new char[]
				{
					','
				});
				if (array.Length > 1)
				{
					holyPartInfo.m_nNeedGoodsCount = Global.SafeConvertToInt32(array[1]);
				}
				array = keyValuePair.Value.GetStringValue("FailCost").Split(new char[]
				{
					','
				});
				if (array.Length > 1)
				{
					holyPartInfo.m_nFailCostGoodsCount = Global.SafeConvertToInt32(array[1]);
				}
				string stringValue = keyValuePair.Value.GetStringValue("Property");
				if (stringValue != "-1")
				{
					holyPartInfo.m_PropertyList = GameManager.SystemMagicActionMgr.ParseActionsOutUse(stringValue);
				}
				holyPartInfo.m_nMaxFailCount = keyValuePair.Value.GetIntValue("FailMaxNum", -1);
				if (holyPartInfo.m_nMaxFailCount < 0)
				{
					holyPartInfo.m_nMaxFailCount = 0;
				}
				holyPartInfo.NeedGoods = ConfigParser.ParserIntArrayList(keyValuePair.Value.GetStringValue("NeedItem"), true, '|', ',');
				holyPartInfo.FaildNeedGoods = ConfigParser.ParserIntArrayList(keyValuePair.Value.GetStringValue("FailureConsumption"), true, '|', ',');
				this._partDataDic.Add(keyValuePair.Value.GetIntValue("ID", -1), holyPartInfo);
				int intValue = keyValuePair.Value.GetIntValue("SuitID", -1);
				HolyItemManager.MAX_HOLY_PART_LEVEL = Math.Max(HolyItemManager.MAX_HOLY_PART_LEVEL, Convert.ToSByte(intValue));
			}
			HolyItemManager.MAX_HOLY_PART_LEVEL = (sbyte)Global.GMin((int)HolyItemManager.MAX_HOLY_PART_LEVEL, (int)GameManager.systemParamsList.GetParamValueIntByName("ShengWuMax", 0));
			systemXmlItems = new SystemXmlItems();
			systemXmlItems.LoadFromXMlFile("Config/ShengWu.xml", "", "ID", 0);
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in systemXmlItems.SystemXmlItemDict)
			{
				HolyInfo holyInfo = new HolyInfo();
				string stringValue = keyValuePair.Value.GetStringValue("ExtraProperty");
				if (stringValue != "-1")
				{
					holyInfo.m_ExtraPropertyList = GameManager.SystemMagicActionMgr.ParseActionsOutUse(stringValue);
				}
				this._holyDataDic.Add(keyValuePair.Value.GetIntValue("ID", -1), holyInfo);
			}
			TCPCmdDispatcher.getInstance().registerProcessorEx(10206, 2, 2, HolyItemManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 10206)
			{
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					sbyte sShengWu_slot = Convert.ToSByte(cmdParams[0]);
					sbyte sBuJian_slot = Convert.ToSByte(cmdParams[1]);
					string cmdData = Convert.ToInt32(this.HolyItem_Suit_Up(client, sShengWu_slot, sBuJian_slot)).ToString();
					client.sendCmd(nID, cmdData, false);
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, "CMD_DB_UPDATE_HOLYITEM", false, false);
				}
			}
			return true;
		}

		private EHolyResult HolyItem_Suit_Up(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot)
		{
			EHolyResult result;
			if (GameFuncControlManager.IsGameFuncDisabled(6))
			{
				result = EHolyResult.NotOpen;
			}
			else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("HolyItem"))
			{
				result = EHolyResult.NotOpen;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 67, true))
			{
				result = EHolyResult.NotOpen;
			}
			else if (null == client.ClientData.MyHolyItemDataDic)
			{
				result = EHolyResult.Error;
			}
			else
			{
				Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
				HolyItemData holyItemData = null;
				HolyItemPartData holyItemPartData = null;
				HolyPartInfo holyPartInfo = null;
				if (!myHolyItemDataDic.TryGetValue(sShengWu_slot, out holyItemData))
				{
					result = EHolyResult.Error;
				}
				else if (!holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
				{
					result = EHolyResult.Error;
				}
				else if (holyItemPartData.m_sSuit >= HolyItemManager.MAX_HOLY_PART_LEVEL)
				{
					result = EHolyResult.PartSuitIsMax;
				}
				else
				{
					int bujianID = HolyPartInfo.GetBujianID(sShengWu_slot, sBuJian_slot, holyItemPartData.m_sSuit);
					if (!this._partDataDic.TryGetValue(bujianID, out holyPartInfo))
					{
						result = EHolyResult.Error;
					}
					else if (-1 != holyPartInfo.m_nCostBandJinBi && holyPartInfo.m_nCostBandJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
					{
						result = EHolyResult.NeedGold;
					}
					else if (-1 != holyPartInfo.m_nNeedGoodsCount && holyPartInfo.m_nNeedGoodsCount > holyItemPartData.m_nSlice)
					{
						result = EHolyResult.NeedHolyItemPart;
					}
					else
					{
						bool flag = false;
						int randomNumber = Global.GetRandomNumber(0, 100);
						if (-1 == holyPartInfo.m_sSuccessProbability || holyItemPartData.m_nFailCount >= holyPartInfo.m_nMaxFailCount || randomNumber < (int)holyPartInfo.m_sSuccessProbability)
						{
							flag = true;
							for (int i = 0; i < holyPartInfo.NeedGoods.Count; i++)
							{
								int num = holyPartInfo.NeedGoods[i][0];
								int num2 = holyPartInfo.NeedGoods[i][1];
								int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, num);
								if (totalGoodsCountByID < num2)
								{
									return EHolyResult.NeedGoods;
								}
							}
							if (-1 != holyPartInfo.m_nCostBandJinBi)
							{
								if (!Global.SubBindTongQianAndTongQian(client, holyPartInfo.m_nCostBandJinBi, "圣物部件升级消耗"))
								{
									return EHolyResult.Error;
								}
							}
							if (-1 != holyPartInfo.m_nNeedGoodsCount)
							{
								holyItemPartData.m_nSlice -= holyPartInfo.m_nNeedGoodsCount;
							}
							if (holyItemPartData.m_nSlice < 0)
							{
								holyItemPartData.m_nSlice = 0;
								return EHolyResult.Error;
							}
							bool flag2 = false;
							bool flag3 = false;
							for (int i = 0; i < holyPartInfo.NeedGoods.Count; i++)
							{
								int num = holyPartInfo.NeedGoods[i][0];
								int num2 = holyPartInfo.NeedGoods[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num2, false, out flag2, out flag3, false))
								{
									LogManager.WriteLog(2, string.Format("圣物部件升级时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", num, num2), null, true);
								}
								GoodsData goodsData = new GoodsData();
								goodsData.GoodsID = num;
								goodsData.GCount = num2;
							}
							HolyItemPartData holyItemPartData2 = holyItemPartData;
							holyItemPartData2.m_sSuit += 1;
							holyItemPartData.m_nFailCount = 0;
						}
						else
						{
							for (int i = 0; i < holyPartInfo.FaildNeedGoods.Count; i++)
							{
								int num = holyPartInfo.FaildNeedGoods[i][0];
								int num2 = holyPartInfo.FaildNeedGoods[i][1];
								int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, num);
								if (totalGoodsCountByID < num2)
								{
									return EHolyResult.NeedGoods;
								}
							}
							if (-1 != holyPartInfo.m_nCostBandJinBi)
							{
								if (!Global.SubBindTongQianAndTongQian(client, holyPartInfo.m_nCostBandJinBi, "圣物部件升级消耗"))
								{
									return EHolyResult.Error;
								}
							}
							bool flag2 = false;
							bool flag3 = false;
							for (int i = 0; i < holyPartInfo.FaildNeedGoods.Count; i++)
							{
								int num = holyPartInfo.FaildNeedGoods[i][0];
								int num2 = holyPartInfo.FaildNeedGoods[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num2, false, out flag2, out flag3, false))
								{
									LogManager.WriteLog(2, string.Format("圣物部件升级时，消耗{1}个GoodsID={0}的物品失败", num, num2), null, true);
								}
								GoodsData goodsData2 = new GoodsData();
								goodsData2.GoodsID = num;
								goodsData2.GCount = num2;
							}
							if (-1 != holyPartInfo.m_nFailCostGoodsCount)
							{
								holyItemPartData.m_nSlice -= holyPartInfo.m_nFailCostGoodsCount;
							}
							if (holyItemPartData.m_nSlice < 0)
							{
								holyItemPartData.m_nSlice = 0;
								return EHolyResult.Error;
							}
							holyItemPartData.m_nFailCount++;
						}
						if (flag)
						{
							this.UpdateHolyItemBuJianAttr(client, sShengWu_slot, sBuJian_slot);
							this.UpdataHolyItemExAttr(client, sShengWu_slot);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
						this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, holyItemPartData);
						this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot], "圣物进阶", "系统", client.ClientData.RoleName, flag ? "成功" : "失败", (holyPartInfo.m_nCostBandJinBi != -1) ? holyPartInfo.m_nCostBandJinBi : 0, client.ClientData.ZoneID, client.strUserID, holyItemPartData.m_nSlice, client.ServerId, null);
						if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						result = (flag ? EHolyResult.Success : EHolyResult.Fail);
					}
				}
			}
			return result;
		}

		public void GetHolyItemPart(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, int nNum)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(6))
			{
				Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
				HolyItemData holyItemData = null;
				HolyItemPartData holyItemPartData = null;
				if (myHolyItemDataDic.TryGetValue(sShengWu_slot, out holyItemData))
				{
					if (holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
					{
						holyItemPartData.m_nSlice += nNum;
						this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, holyItemPartData);
						this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
						string msgText = StringUtil.substitute(GLang.GetLang(384, new object[0]), new object[]
						{
							Global.GetLang(HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot]),
							nNum
						});
						GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.PiaoFuZi, 0);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot], "圣物碎片", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", nNum, client.ClientData.ZoneID, client.strUserID, holyItemPartData.m_nSlice, client.ServerId, null);
					}
				}
			}
		}

		public void PlayGameAfterSend(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(6))
			{
				Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
				if (myHolyItemDataDic != null)
				{
					HolyItemData holyItemData = null;
					HolyItemPartData value = null;
					for (sbyte b = 1; b <= HolyItemManager.MAX_HOLY_NUM; b += 1)
					{
						if (myHolyItemDataDic.TryGetValue(b, out holyItemData))
						{
							for (sbyte b2 = 1; b2 <= HolyItemManager.MAX_HOLY_PART_NUM; b2 += 1)
							{
								if (!holyItemData.m_PartArray.TryGetValue(b2, out value))
								{
									value = new HolyItemPartData();
									holyItemData.m_PartArray.Add(b2, value);
								}
							}
						}
						else
						{
							holyItemData = new HolyItemData();
							myHolyItemDataDic.Add(b, holyItemData);
							holyItemData.m_sType = b;
							for (sbyte b2 = 1; b2 <= HolyItemManager.MAX_HOLY_PART_NUM; b2 += 1)
							{
								value = new HolyItemPartData();
								holyItemData.m_PartArray.Add(b2, value);
							}
						}
					}
					client.sendCmd<Dictionary<sbyte, HolyItemData>>(1200, myHolyItemDataDic, false);
				}
			}
		}

		public void HolyItemSendToClient(GameClient client, sbyte sShenWu_slot, sbyte sBuJian_slot)
		{
			Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
			if (myHolyItemDataDic != null)
			{
				HolyItemData holyItemData = null;
				HolyItemPartData holyItemPartData = null;
				if (myHolyItemDataDic.TryGetValue(sShenWu_slot, out holyItemData))
				{
					if (holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							sShenWu_slot,
							sBuJian_slot,
							holyItemPartData.m_sSuit,
							holyItemPartData.m_nSlice
						});
						client.sendCmd(1201, cmdData, false);
					}
				}
			}
		}

		public void UpdateAllHolyItemAttr(GameClient client)
		{
			for (sbyte b = 1; b <= HolyItemManager.MAX_HOLY_NUM; b += 1)
			{
				for (sbyte b2 = 1; b2 <= HolyItemManager.MAX_HOLY_PART_NUM; b2 += 1)
				{
					this.UpdateHolyItemBuJianAttr(client, b, b2);
				}
				this.UpdataHolyItemExAttr(client, b);
			}
		}

		public void UpdateHolyItemBuJianAttr(GameClient client, sbyte sShenWu_slot, sbyte sBuJian_slot)
		{
			Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
			if (null != myHolyItemDataDic)
			{
				HolyItemData holyItemData = null;
				HolyItemPartData holyItemPartData = null;
				if (myHolyItemDataDic.TryGetValue(sShenWu_slot, out holyItemData))
				{
					if (holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
					{
						int bujianID = HolyPartInfo.GetBujianID(sShenWu_slot, sBuJian_slot, holyItemPartData.m_sSuit);
						HolyPartInfo holyPartInfo = null;
						if (this._partDataDic.TryGetValue(bujianID, out holyPartInfo))
						{
							for (int i = 0; i < holyPartInfo.m_PropertyList.Count; i++)
							{
								this.ProcessAction(client, holyPartInfo.m_PropertyList[i].MagicActionID, holyPartInfo.m_PropertyList[i].MagicActionParams, 16, sShenWu_slot, sBuJian_slot);
							}
						}
					}
				}
			}
		}

		public void UpdataHolyItemExAttr(GameClient client, sbyte sShenWu_slot)
		{
			Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
			if (null != myHolyItemDataDic)
			{
				HolyItemData holyItemData = null;
				HolyItemPartData holyItemPartData = null;
				int num = (int)HolyItemManager.MAX_HOLY_PART_LEVEL;
				if (myHolyItemDataDic.TryGetValue(sShenWu_slot, out holyItemData))
				{
					for (sbyte b = 1; b <= HolyItemManager.MAX_HOLY_PART_NUM; b += 1)
					{
						if (!holyItemData.m_PartArray.TryGetValue(b, out holyItemPartData))
						{
							num = 0;
							break;
						}
						if (num > (int)holyItemPartData.m_sSuit)
						{
							num = (int)holyItemPartData.m_sSuit;
						}
					}
				}
				else
				{
					num = 0;
				}
				HolyInfo holyInfo = null;
				int shengwuID = HolyInfo.GetShengwuID((sbyte)num, sShenWu_slot);
				if (this._holyDataDic.TryGetValue(shengwuID, out holyInfo))
				{
					for (int i = 0; i < holyInfo.m_ExtraPropertyList.Count; i++)
					{
						this.ProcessAction(client, holyInfo.m_ExtraPropertyList[i].MagicActionID, holyInfo.m_ExtraPropertyList[i].MagicActionParams, 16, sShenWu_slot, 100);
					}
				}
			}
		}

		private void UpdateHolyItemData2DB(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, HolyItemPartData partdata = null)
		{
			Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
			HolyItemData holyItemData = null;
			HolyItemPartData holyItemPartData = partdata;
			if (holyItemPartData == null)
			{
				if (!myHolyItemDataDic.TryGetValue(sShengWu_slot, out holyItemData))
				{
					return;
				}
				if (!holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
				{
					return;
				}
			}
			string[] array = null;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				sShengWu_slot,
				sBuJian_slot,
				holyItemPartData.m_sSuit,
				holyItemPartData.m_nSlice,
				holyItemPartData.m_nFailCount
			});
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10206, strcmd, out array, client.ServerId);
		}

		private void ProcessAction(GameClient client, MagicActionIDs id, double[] actionParams, int nPropsSystemTypes, sbyte sShengWu_slot, sbyte sBuJian_slot)
		{
			switch (id)
			{
			case MagicActionIDs.POTION:
			case MagicActionIDs.HOLYWATER:
			case MagicActionIDs.RECOVERLIFEV:
			case MagicActionIDs.LIFESTEAL:
			case MagicActionIDs.FATALHURT:
			case MagicActionIDs.ADDATTACK:
			case MagicActionIDs.ADDATTACKINJURE:
			case MagicActionIDs.HITV:
			case MagicActionIDs.ADDDEFENSE:
			case MagicActionIDs.COUNTERACTINJUREVALUE:
			case MagicActionIDs.DAMAGETHORN:
			case MagicActionIDs.DODGE:
			case MagicActionIDs.MAXLIFEPERCENT:
			case MagicActionIDs.AddAttackPercent:
			case MagicActionIDs.AddDefensePercent:
			case MagicActionIDs.HitPercent:
			{
				ExtPropIndexes extPropIndexes = ExtPropIndexes.Max;
				switch (id)
				{
				case MagicActionIDs.POTION:
					extPropIndexes = ExtPropIndexes.Potion;
					break;
				case MagicActionIDs.HOLYWATER:
					extPropIndexes = ExtPropIndexes.Holywater;
					break;
				case MagicActionIDs.RECOVERLIFEV:
					extPropIndexes = ExtPropIndexes.RecoverLifeV;
					break;
				case MagicActionIDs.LIFESTEAL:
					extPropIndexes = ExtPropIndexes.LifeSteal;
					break;
				case MagicActionIDs.FATALHURT:
					extPropIndexes = ExtPropIndexes.Fatalhurt;
					break;
				case MagicActionIDs.ADDATTACK:
					extPropIndexes = ExtPropIndexes.AddAttack;
					break;
				case MagicActionIDs.ADDATTACKINJURE:
					extPropIndexes = ExtPropIndexes.AddAttackInjure;
					break;
				case MagicActionIDs.HITV:
					extPropIndexes = ExtPropIndexes.HitV;
					break;
				case MagicActionIDs.ADDDEFENSE:
					extPropIndexes = ExtPropIndexes.AddDefense;
					break;
				case MagicActionIDs.COUNTERACTINJUREVALUE:
					extPropIndexes = ExtPropIndexes.CounteractInjureValue;
					break;
				case MagicActionIDs.DAMAGETHORN:
					extPropIndexes = ExtPropIndexes.DamageThorn;
					break;
				case MagicActionIDs.DODGE:
					extPropIndexes = ExtPropIndexes.Dodge;
					break;
				case MagicActionIDs.MAXLIFEPERCENT:
					extPropIndexes = ExtPropIndexes.MaxLifePercent;
					break;
				case MagicActionIDs.AddAttackPercent:
					extPropIndexes = ExtPropIndexes.AddAttackPercent;
					break;
				case MagicActionIDs.AddDefensePercent:
					extPropIndexes = ExtPropIndexes.AddDefensePercent;
					break;
				case MagicActionIDs.HitPercent:
					extPropIndexes = ExtPropIndexes.HitPercent;
					break;
				}
				if (extPropIndexes != ExtPropIndexes.Max)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						nPropsSystemTypes,
						(int)sShengWu_slot,
						(int)sBuJian_slot,
						1000,
						(int)extPropIndexes,
						actionParams[0]
					});
				}
				break;
			}
			case MagicActionIDs.STRENGTH:
			{
				PropsCacheManager propsCacheManager = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 0;
				object[] array2 = array;
				int num = 4;
				double[] array3 = new double[4];
				array3[0] = actionParams[0];
				array2[num] = array3;
				propsCacheManager.SetBaseProps(array);
				break;
			}
			case MagicActionIDs.CONSTITUTION:
				client.ClientData.PropsCacheManager.SetBaseProps(new object[]
				{
					nPropsSystemTypes,
					(int)sShengWu_slot,
					(int)sBuJian_slot,
					3,
					new double[]
					{
						0.0,
						0.0,
						0.0,
						actionParams[0]
					}
				});
				break;
			case MagicActionIDs.DEXTERITY:
			{
				PropsCacheManager propsCacheManager2 = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 2;
				object[] array4 = array;
				int num2 = 4;
				double[] array3 = new double[4];
				array3[2] = actionParams[0];
				array4[num2] = array3;
				propsCacheManager2.SetBaseProps(array);
				break;
			}
			case MagicActionIDs.INTELLIGENCE:
			{
				PropsCacheManager propsCacheManager3 = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 1;
				object[] array5 = array;
				int num3 = 4;
				double[] array3 = new double[4];
				array3[1] = actionParams[0];
				array5[num3] = array3;
				propsCacheManager3.SetBaseProps(array);
				break;
			}
			}
		}

		public void GMSetHolyItemLvup(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, sbyte sLv)
		{
			Dictionary<sbyte, HolyItemData> myHolyItemDataDic = client.ClientData.MyHolyItemDataDic;
			HolyItemData holyItemData = null;
			HolyItemPartData holyItemPartData = null;
			if (myHolyItemDataDic != null && myHolyItemDataDic.TryGetValue(sShengWu_slot, out holyItemData))
			{
				if (holyItemData.m_PartArray.TryGetValue(sBuJian_slot, out holyItemPartData))
				{
					holyItemPartData.m_sSuit = sLv;
					if (holyItemPartData.m_sSuit > HolyItemManager.MAX_HOLY_PART_LEVEL)
					{
						holyItemPartData.m_sSuit = HolyItemManager.MAX_HOLY_PART_LEVEL;
					}
					this.UpdateHolyItemBuJianAttr(client, sShengWu_slot, sBuJian_slot);
					this.UpdataHolyItemExAttr(client, sShengWu_slot);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, holyItemPartData);
					this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
				}
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static HolyItemManager()
		{
			string[,] array = new string[5, 7];
			array[0, 0] = "null";
			array[0, 1] = "null";
			array[0, 2] = "null";
			array[0, 3] = "null";
			array[0, 4] = "null";
			array[0, 5] = "null";
			array[0, 6] = "null";
			array[1, 0] = "null";
			array[1, 1] = "圣杯碎片1";
			array[1, 2] = "圣杯碎片2";
			array[1, 3] = "圣杯碎片3";
			array[1, 4] = "圣杯碎片4";
			array[1, 5] = "圣杯碎片5";
			array[1, 6] = "圣杯碎片6";
			array[2, 0] = "null";
			array[2, 1] = "圣冠碎片1";
			array[2, 2] = "圣冠碎片2";
			array[2, 3] = "圣冠碎片3";
			array[2, 4] = "圣冠碎片4";
			array[2, 5] = "圣冠碎片5";
			array[2, 6] = "圣冠碎片6";
			array[3, 0] = "null";
			array[3, 1] = "圣剑碎片1";
			array[3, 2] = "圣剑碎片2";
			array[3, 3] = "圣剑碎片3";
			array[3, 4] = "圣剑碎片4";
			array[3, 5] = "圣剑碎片5";
			array[3, 6] = "圣剑碎片6";
			array[4, 0] = "null";
			array[4, 1] = "圣典碎片1";
			array[4, 2] = "圣典碎片2";
			array[4, 3] = "圣典碎片3";
			array[4, 4] = "圣典碎片4";
			array[4, 5] = "圣典碎片5";
			array[4, 6] = "圣典碎片6";
			HolyItemManager.SliceNameSet = array;
			HolyItemManager.instance = new HolyItemManager();
		}

		public static sbyte MAX_HOLY_PART_LEVEL = 9;

		public static readonly sbyte MAX_HOLY_PART_NUM = 6;

		public static readonly sbyte MAX_HOLY_NUM = 4;

		private Dictionary<int, HolyPartInfo> _partDataDic = new Dictionary<int, HolyPartInfo>();

		private Dictionary<int, HolyInfo> _holyDataDic = new Dictionary<int, HolyInfo>();

		public static readonly string[,] SliceNameSet;

		private static HolyItemManager instance;
	}
}
