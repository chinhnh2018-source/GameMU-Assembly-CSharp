using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.Talent
{
	public class TalentManager : ICmdProcessorEx, ICmdProcessor
	{
		public static TalentManager getInstance()
		{
			return TalentManager.instance;
		}

		public bool initialize()
		{
			TalentManager.LoadTalentExpInfo();
			TalentManager.LoadTalentSpecialData();
			TalentManager.LoadTalentInfoData();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(999, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1000, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1001, 1, 1, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1002, 2, 2, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1003, 3, 3, TalentManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			case 999:
				result = this.ProcessCmdTalentOther(client, nID, bytes, cmdParams);
				break;
			case 1000:
				result = this.ProcessCmdTalentGetData(client, nID, bytes, cmdParams);
				break;
			case 1001:
				result = this.ProcessCmdTalentAddExp(client, nID, bytes, cmdParams);
				break;
			case 1002:
				result = this.ProcessCmdTalentWash(client, nID, bytes, cmdParams);
				break;
			case 1003:
				result = this.ProcessCmdTalentAddEffect(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool ProcessCmdTalentOther(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				TalentData cmdData = null;
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				if (gameClient != null)
				{
					cmdData = TalentManager.GetTalentData(gameClient);
				}
				client.sendCmd<TalentData>(999, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCmdTalentGetData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				TalentData talentData = TalentManager.GetTalentData(client);
				client.sendCmd<TalentData>(1000, talentData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCmdTalentAddExp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int state = TalentManager.TalentAddExp(client);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1001, talentData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCmdTalentWash(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int washType = int.Parse(cmdParams[1]);
				int state = TalentManager.TalentWash(client, washType);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1002, talentData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCmdTalentAddEffect(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int effectID = int.Parse(cmdParams[1]);
				int addCount = int.Parse(cmdParams[2]);
				int state = TalentManager.TalentAddEffect(client, effectID, addCount);
				TalentData talentData = TalentManager.GetTalentData(client);
				talentData.State = state;
				client.sendCmd<TalentData>(1003, talentData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private static TalentData GetTalentData(GameClient client)
		{
			TalentData result;
			if (!GlobalNew.IsGongNengOpened(client, 59, false))
			{
				result = null;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = null;
			}
			else
			{
				client.ClientData.MyTalentData.IsOpen = true;
				client.ClientData.MyTalentData.SkillOneValue = client.ClientData.MyTalentPropData.SkillOneValue;
				client.ClientData.MyTalentData.SkillAllValue = client.ClientData.MyTalentPropData.SkillAllValue;
				client.ClientData.MyTalentData.Occupation = client.ClientData.Occupation;
				result = client.ClientData.MyTalentData;
			}
			return result;
		}

		private static int TalentAddExp(GameClient client)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, 59, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (client.ClientData.Experience <= 0L)
			{
				result = TalentResultType.EnoExp;
			}
			else
			{
				TalentData myTalentData = client.ClientData.MyTalentData;
				int num = (myTalentData.TotalCount <= 0) ? 1 : (myTalentData.TotalCount + 1);
				if (!TalentManager._TalentExpList.ContainsKey(num))
				{
					result = TalentResultType.EnoOpenPoint;
				}
				else
				{
					TalentExpInfo talentExpInfo = TalentManager._TalentExpList[num];
					int num2 = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
					if (num2 < talentExpInfo.RoleLevel)
					{
						result = TalentResultType.EnoOpenPoint;
					}
					else
					{
						long num3 = talentExpInfo.Exp - myTalentData.Exp;
						long num4 = 0L;
						long experience = client.ClientData.Experience;
						bool flag = false;
						long expAdd;
						if (num3 <= experience)
						{
							flag = true;
							expAdd = num3;
						}
						else
						{
							num4 = myTalentData.Exp + experience;
							num--;
							expAdd = experience;
						}
						if (!TalentManager.DBTalentModify(client.ClientData.RoleID, num, num4, expAdd, flag, client.ClientData.ZoneID, client.ServerId))
						{
							result = TalentResultType.EFail;
						}
						else
						{
							if (flag)
							{
								myTalentData.Exp = num4;
								myTalentData.TotalCount++;
								client.ClientData.Experience -= num3;
							}
							else
							{
								myTalentData.Exp = num4;
								client.ClientData.Experience -= experience;
							}
							GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -num4);
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_Talent, new int[0]));
							if (flag)
							{
								result = TalentResultType.Success;
							}
							else
							{
								result = TalentResultType.SuccessHalf;
							}
						}
					}
				}
			}
			return result;
		}

		private static int TalentWash(GameClient client, int washType)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, 59, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = TalentResultType.EnoOpen;
			}
			else
			{
				TalentData myTalentData = client.ClientData.MyTalentData;
				int talentUseCount = TalentManager.GetTalentUseCount(myTalentData);
				if (talentUseCount <= 0)
				{
					result = TalentResultType.EnoTalentCount;
				}
				else
				{
					if (washType == 0)
					{
						int washDiamond = TalentManager.GetWashDiamond(talentUseCount);
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, washDiamond, "天赋洗点", true, true, false, DaiBiSySType.TianFuXiDian))
						{
							return TalentResultType.EnoDiamond;
						}
					}
					else
					{
						int goodsID = 0;
						int subNum = 0;
						TalentManager.GetWashGoods(out goodsID, out subNum);
						GoodsData goodsByID = Global.GetGoodsByID(client, goodsID);
						if (goodsByID == null)
						{
							return TalentResultType.EnoWash;
						}
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByID, subNum, false, false))
						{
							return TalentResultType.EnoWash;
						}
					}
					if (!TalentManager.DBTalentEffectClear(client.ClientData.RoleID, client.ClientData.ZoneID, client.ServerId))
					{
						result = TalentResultType.EFail;
					}
					else
					{
						myTalentData.CountList[1] = 0;
						myTalentData.CountList[2] = 0;
						myTalentData.CountList[3] = 0;
						myTalentData.EffectList = new List<TalentEffectItem>();
						TalentPropData myTalentPropData = client.ClientData.MyTalentPropData;
						myTalentPropData.ResetProps();
						TalentManager.SetTalentProp(client, TalentEffectType.PropBasic, myTalentPropData.PropItem);
						TalentManager.SetTalentProp(client, TalentEffectType.PropExt, myTalentPropData.PropItem);
						TalentManager.RefreshProp(client);
						result = TalentResultType.Success;
					}
				}
			}
			return result;
		}

		private static int TalentAddEffect(GameClient client, int effectID, int addCount)
		{
			int result;
			if (!GlobalNew.IsGongNengOpened(client, 59, false))
			{
				result = TalentResultType.EnoOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = TalentResultType.EnoOpen;
			}
			else
			{
				TalentInfo talentInfoByID = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, effectID);
				if (talentInfoByID == null)
				{
					result = TalentResultType.EnoEffect;
				}
				else
				{
					TalentData myTalentData = client.ClientData.MyTalentData;
					int num = myTalentData.TotalCount - TalentManager.GetTalentUseCount(myTalentData);
					if (num < addCount)
					{
						result = TalentResultType.EnoTalentCount;
					}
					else if (!TalentManager.IsEffectOpen(myTalentData, talentInfoByID.NeedTalentID, talentInfoByID.NeedTalentLevel))
					{
						result = TalentResultType.EnoOpenPreEffect;
					}
					else if (talentInfoByID.NeedTalentCount > 0 && talentInfoByID.NeedTalentCount > myTalentData.CountList[talentInfoByID.Type])
					{
						result = TalentResultType.EnoOpenPreCount;
					}
					else
					{
						int num2 = 0;
						TalentEffectItem talentEffectItem = TalentManager.GetOpenEffectItem(myTalentData, effectID);
						if (talentEffectItem != null)
						{
							if (talentEffectItem.Level >= talentInfoByID.LevelMax)
							{
								return TalentResultType.EisMaxLevel;
							}
							num2 = talentEffectItem.Level;
						}
						num2 += addCount;
						List<TalentEffectInfo> itemEffectList = talentInfoByID.EffectList[num2];
						if (num2 > talentInfoByID.LevelMax)
						{
							result = TalentResultType.EisMaxLevel;
						}
						else if (!TalentManager.DBTalentEffectModify(client.ClientData.RoleID, talentInfoByID.Type, effectID, num2, client.ClientData.ZoneID, client.ServerId))
						{
							result = TalentResultType.EFail;
						}
						else
						{
							Dictionary<int, int> countList;
							int type;
							(countList = myTalentData.CountList)[type = talentInfoByID.Type] = countList[type] + addCount;
							if (talentEffectItem == null)
							{
								talentEffectItem = new TalentEffectItem();
								talentEffectItem.ID = effectID;
								talentEffectItem.TalentType = talentInfoByID.Type;
								myTalentData.EffectList.Add(talentEffectItem);
							}
							talentEffectItem.Level = num2;
							talentEffectItem.ItemEffectList = itemEffectList;
							TalentManager.initTalentEffectProp(client);
							TalentManager.RefreshProp(client);
							result = TalentResultType.Success;
						}
					}
				}
			}
			return result;
		}

		public static void ModifyEffect(GameClient client, int effectID, int talentType, int newLevel)
		{
			try
			{
				TalentData myTalentData = client.ClientData.MyTalentData;
				TalentInfo talentInfoByID = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, effectID);
				if (talentInfoByID != null)
				{
					List<TalentEffectInfo> itemEffectList = talentInfoByID.EffectList[newLevel];
					bool flag = TalentManager.DBTalentEffectModify(client.ClientData.RoleID, talentType, effectID, newLevel, client.ClientData.ZoneID, client.ServerId);
					Dictionary<int, int> countList;
					(countList = myTalentData.CountList)[talentType] = countList[talentType] + newLevel;
					TalentEffectItem talentEffectItem = TalentManager.GetOpenEffectItem(myTalentData, effectID);
					if (talentEffectItem == null)
					{
						talentEffectItem = new TalentEffectItem();
						talentEffectItem.ID = effectID;
						talentEffectItem.TalentType = talentType;
						myTalentData.EffectList.Add(talentEffectItem);
					}
					talentEffectItem.Level = newLevel;
					talentEffectItem.ItemEffectList = itemEffectList;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		private static bool IsEffectOpen(TalentData talentData, int effectID, int level)
		{
			bool result;
			if (effectID <= 0)
			{
				result = true;
			}
			else
			{
				TalentEffectItem openEffectItem = TalentManager.GetOpenEffectItem(talentData, effectID);
				if (openEffectItem != null)
				{
					if (openEffectItem.Level >= level)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private static TalentEffectItem GetOpenEffectItem(TalentData talentData, int effectID)
		{
			TalentEffectItem result;
			if (effectID <= 0)
			{
				result = null;
			}
			else
			{
				foreach (TalentEffectItem talentEffectItem in talentData.EffectList)
				{
					if (talentEffectItem.ID == effectID)
					{
						return talentEffectItem;
					}
				}
				result = null;
			}
			return result;
		}

		private static int GetTalentUseCount(TalentData talentData)
		{
			int result;
			if (talentData.CountList == null || talentData.CountList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				result = talentData.CountList[2] + talentData.CountList[1] + talentData.CountList[3];
			}
			return result;
		}

		public static void initTalentEffectProp(GameClient client)
		{
			TalentData talentData = TalentManager.GetTalentData(client);
			if (talentData != null && talentData.IsOpen)
			{
				TalentPropData myTalentPropData = client.ClientData.MyTalentPropData;
				myTalentPropData.ResetProps();
				foreach (TalentEffectItem talentEffectItem in talentData.EffectList)
				{
					TalentInfo talentInfoByID = TalentManager.GetTalentInfoByID(client.ClientData.Occupation, talentEffectItem.ID);
					if (talentInfoByID.LevelMax >= talentEffectItem.Level)
					{
						talentEffectItem.ItemEffectList = talentInfoByID.EffectList[talentEffectItem.Level];
						foreach (TalentEffectInfo talentEffectInfo in talentEffectItem.ItemEffectList)
						{
							switch (talentEffectInfo.EffectType)
							{
							case 1:
								myTalentPropData.PropItem.BaseProps[talentEffectInfo.EffectID] += (double)((int)talentEffectInfo.EffectValue);
								break;
							case 2:
								myTalentPropData.PropItem.ExtProps[talentEffectInfo.EffectID] += talentEffectInfo.EffectValue;
								break;
							case 3:
								if (myTalentPropData.SkillOneValue.ContainsKey(talentEffectInfo.EffectID))
								{
									Dictionary<int, int> skillOneValue;
									int effectID;
									(skillOneValue = myTalentPropData.SkillOneValue)[effectID = talentEffectInfo.EffectID] = skillOneValue[effectID] + (int)talentEffectInfo.EffectValue;
								}
								else
								{
									myTalentPropData.SkillOneValue.Add(talentEffectInfo.EffectID, (int)talentEffectInfo.EffectValue);
								}
								break;
							case 4:
								myTalentPropData.SkillAllValue += (int)talentEffectInfo.EffectValue;
								break;
							}
						}
					}
				}
				TalentManager.InitSpecialProp(client);
				client.ClientData.MyTalentData.SkillOneValue = client.ClientData.MyTalentPropData.SkillOneValue;
				client.ClientData.MyTalentData.SkillAllValue = client.ClientData.MyTalentPropData.SkillAllValue;
				TalentManager.SetTalentProp(client, TalentEffectType.PropBasic, myTalentPropData.PropItem);
				TalentManager.SetTalentProp(client, TalentEffectType.PropExt, myTalentPropData.PropItem);
			}
		}

		private static void InitSpecialProp(GameClient client)
		{
			TalentData myTalentData = client.ClientData.MyTalentData;
			if (myTalentData.CountList != null && myTalentData.CountList.Count > 0)
			{
				foreach (KeyValuePair<int, int> keyValuePair in myTalentData.CountList)
				{
					int key = keyValuePair.Key;
					int value = keyValuePair.Value;
					TalentSpecialInfo talentSpecialInfo = TalentManager._TalentSpecialList[key];
					int num = value / talentSpecialInfo.SingleCount;
					foreach (KeyValuePair<int, double> keyValuePair2 in talentSpecialInfo.EffectList)
					{
						int key2 = keyValuePair2.Key;
						double num2 = keyValuePair2.Value * (double)num;
						client.ClientData.MyTalentPropData.PropItem.ExtProps[key2] += num2;
					}
				}
			}
		}

		private static void SetTalentProp(GameClient client, TalentEffectType type, EquipPropItem item)
		{
			switch (type)
			{
			case TalentEffectType.PropBasic:
				client.ClientData.PropsCacheManager.SetBaseProps(new object[]
				{
					14,
					(int)type,
					item.BaseProps
				});
				break;
			case TalentEffectType.PropExt:
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					14,
					(int)type,
					item.ExtProps
				});
				break;
			}
		}

		public static void RefreshProp(GameClient client)
		{
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				default(DelayExecProcIds),
				2
			});
		}

		public static int GetSkillLevel(GameClient client, int skillID)
		{
			int num = 0;
			if (client.ClientData.MyTalentData.IsOpen)
			{
				TalentPropData myTalentPropData = client.ClientData.MyTalentPropData;
				if (myTalentPropData.SkillOneValue.ContainsKey(skillID))
				{
					num += myTalentPropData.SkillOneValue[skillID];
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemXmlItem))
					{
						return num;
					}
					int intValue = systemXmlItem.GetIntValue("ParentMagicID", -1);
					if (intValue > 0)
					{
						SkillData skillDataByID = Global.GetSkillDataByID(client, intValue);
						if (null != skillDataByID)
						{
							if (myTalentPropData.SkillOneValue.ContainsKey(skillDataByID.SkillID))
							{
								num += myTalentPropData.SkillOneValue[skillDataByID.SkillID];
							}
						}
					}
				}
				num += myTalentPropData.SkillAllValue;
			}
			return num;
		}

		private static void LoadTalentExpInfo()
		{
			string text = Global.GameResPath("Config/TianFuDian.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					TalentManager._TalentExpList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							TalentExpInfo talentExpInfo = new TalentExpInfo();
							talentExpInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TianFuDian", "0"));
							talentExpInfo.Exp = Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "NeedExp", "0"));
							string[] array = Global.GetDefAttributeStr(xelement2, "NeedLevel", "0,0").Split(new char[]
							{
								','
							});
							talentExpInfo.RoleLevel = int.Parse(array[0]) * 100 + int.Parse(array[1]);
							TalentManager._TalentExpList.Add(talentExpInfo.ID, talentExpInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		private static int GetWashDiamond(int count)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ResettingTianFuCostZuanShi", ',');
			return Math.Min(count * paramValueIntArrayByName[0], paramValueIntArrayByName[1]);
		}

		private static void GetWashGoods(out int goodsID, out int goodsCount)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ResettingTianFuCostGoods", ',');
			goodsID = paramValueIntArrayByName[0];
			goodsCount = paramValueIntArrayByName[1];
		}

		public static void LoadTalentSpecialData()
		{
			string text = Global.GameResPath("Config/TianFuGroupProperty.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					Dictionary<int, TalentSpecialInfo> dictionary = new Dictionary<int, TalentSpecialInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							TalentSpecialInfo talentSpecialInfo = new TalentSpecialInfo();
							talentSpecialInfo.SpecialType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TianFuType", "0"));
							talentSpecialInfo.SingleCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedExp", "0"));
							talentSpecialInfo.EffectList = new Dictionary<int, double>();
							double value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "TripleAttack", "0"));
							talentSpecialInfo.EffectList.Add(61, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "SlowAttack", "0"));
							talentSpecialInfo.EffectList.Add(62, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "VampiricAttack", "0"));
							talentSpecialInfo.EffectList.Add(63, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "TripleDefense", "0"));
							talentSpecialInfo.EffectList.Add(64, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "SlowDefensee", "0"));
							talentSpecialInfo.EffectList.Add(65, value);
							value = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "VampiricDefense", "0"));
							talentSpecialInfo.EffectList.Add(66, value);
							dictionary.Add(talentSpecialInfo.SpecialType, talentSpecialInfo);
						}
					}
					TalentManager._TalentSpecialList = dictionary;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		private static void LoadTalentInfoData()
		{
			TalentManager._TalentInfoList.Clear();
			for (int i = 0; i < 6; i++)
			{
				Dictionary<int, TalentInfo> dictionary = new Dictionary<int, TalentInfo>();
				string text = Global.GameResPath(string.Format("Config/TianFuProperty_{0}.xml", i));
				XElement xelement = CheckHelper.LoadXml(text, false);
				if (null == xelement)
				{
					TalentManager._TalentInfoList.Add(i, dictionary);
				}
				else
				{
					try
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								TalentInfo talentInfo = new TalentInfo();
								talentInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								talentInfo.Type = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TianFuType", "0"));
								talentInfo.Name = Global.GetDefAttributeStr(xelement2, "Name", "");
								talentInfo.NeedTalentCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedInputPoint", "0"));
								talentInfo.NeedTalentID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedTianFu", "0"));
								talentInfo.NeedTalentLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedTianFuLevel", "0"));
								talentInfo.LevelMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LevelMax", "0"));
								talentInfo.EffectType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EffectType", "0"));
								talentInfo.EffectList = new Dictionary<int, List<TalentEffectInfo>>();
								string defAttributeStr = Global.GetDefAttributeStr(xelement2, "Effect1", "");
								TalentManager.XmlGetTalentEffect(talentInfo, 1, defAttributeStr);
								defAttributeStr = Global.GetDefAttributeStr(xelement2, "Effect2", "");
								TalentManager.XmlGetTalentEffect(talentInfo, 2, defAttributeStr);
								defAttributeStr = Global.GetDefAttributeStr(xelement2, "Effect3", "");
								TalentManager.XmlGetTalentEffect(talentInfo, 3, defAttributeStr);
								defAttributeStr = Global.GetDefAttributeStr(xelement2, "Effect4", "");
								TalentManager.XmlGetTalentEffect(talentInfo, 4, defAttributeStr);
								defAttributeStr = Global.GetDefAttributeStr(xelement2, "Effect5", "");
								TalentManager.XmlGetTalentEffect(talentInfo, 5, defAttributeStr);
								dictionary.Add(talentInfo.ID, talentInfo);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!{1}", text, ex.Message), null, true);
					}
					TalentManager._TalentInfoList.Add(i, dictionary);
				}
			}
		}

		private static void XmlGetTalentEffect(TalentInfo talentInfo, int level, string strEffect)
		{
			if (!string.IsNullOrEmpty(strEffect))
			{
				string[] array = strEffect.Split(new char[]
				{
					'|'
				});
				List<TalentEffectInfo> list = new List<TalentEffectInfo>();
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					TalentEffectInfo talentEffectInfo = new TalentEffectInfo();
					talentEffectInfo.EffectType = talentInfo.EffectType;
					switch (talentEffectInfo.EffectType)
					{
					case 1:
						talentEffectInfo.EffectID = (int)Enum.Parse(typeof(UnitPropIndexes), array3[0]);
						talentEffectInfo.EffectValue = double.Parse(array3[1]);
						break;
					case 2:
						talentEffectInfo.EffectID = (int)Enum.Parse(typeof(ExtPropIndexes), array3[0]);
						talentEffectInfo.EffectValue = double.Parse(array3[1]);
						break;
					case 3:
					case 4:
						talentEffectInfo.EffectID = int.Parse(array3[1]);
						talentEffectInfo.EffectValue = double.Parse(array3[2]);
						break;
					}
					list.Add(talentEffectInfo);
				}
				talentInfo.EffectList.Add(level, list);
			}
		}

		private static TalentInfo GetTalentInfoByID(int type, int id)
		{
			TalentInfo result;
			if (type >= 6 || type < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, TalentInfo> dictionary = TalentManager._TalentInfoList[type];
				if (dictionary.ContainsKey(id))
				{
					result = dictionary[id];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static bool DBTalentModify(int roleID, int totalCount, long exp, long expAdd, bool isUp, int zoneID, int serverID)
		{
			bool result = false;
			int num = isUp ? 1 : 0;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				roleID,
				totalCount,
				exp,
				expAdd,
				num,
				zoneID
			});
			string[] array = Global.ExecuteDBCmd(13108, strcmd, serverID);
			if (array != null && array.Length == 1)
			{
				result = (array[0] == 0.ToString());
			}
			return result;
		}

		public static bool DBTalentEffectModify(int roleID, int talentType, int effectID, int effectLevel, int zoneID, int serverID)
		{
			bool result = false;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				roleID,
				talentType,
				effectID,
				effectLevel,
				zoneID
			});
			string[] array = Global.ExecuteDBCmd(13109, strcmd, serverID);
			if (array != null && array.Length == 1)
			{
				result = (array[0] == 0.ToString());
			}
			return result;
		}

		public static bool DBTalentEffectClear(int roleID, int zoneID, int serverID)
		{
			bool result = false;
			string strcmd = string.Format("{0}:{1}", roleID, zoneID);
			string[] array = Global.ExecuteDBCmd(13110, strcmd, serverID);
			if (array != null && array.Length == 1)
			{
				result = (array[0] == 0.ToString());
			}
			return result;
		}

		public static bool TalentAddCount(GameClient client, int count)
		{
			TalentData myTalentData = client.ClientData.MyTalentData;
			bool result;
			if (!myTalentData.IsOpen)
			{
				result = false;
			}
			else if (!TalentManager.DBTalentModify(client.ClientData.RoleID, count, 0L, 0L, false, client.ClientData.ZoneID, client.ServerId))
			{
				result = false;
			}
			else
			{
				myTalentData.TotalCount = count;
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_Talent, new int[0]));
				result = true;
			}
			return result;
		}

		private static TalentManager instance = new TalentManager();

		private static Dictionary<int, TalentExpInfo> _TalentExpList = new Dictionary<int, TalentExpInfo>();

		private static Dictionary<int, TalentSpecialInfo> _TalentSpecialList = new Dictionary<int, TalentSpecialInfo>();

		private static Dictionary<int, Dictionary<int, TalentInfo>> _TalentInfoList = new Dictionary<int, Dictionary<int, TalentInfo>>();
	}
}
