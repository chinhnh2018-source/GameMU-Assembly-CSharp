using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.MagicSword
{
	public class MagicSwordManager
	{
		public void LoadMagicSwordData()
		{
			try
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("MJSChuShi");
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(2, "魔剑士静态数据有误，无法读取", null, true);
				}
				else
				{
					string[] array2 = array[4].Split(new char[]
					{
						','
					});
					if (array2.Length != 2)
					{
						LogManager.WriteLog(2, "魔剑士静态数据转生与级数有误，无法读取", null, true);
					}
					else
					{
						MagicSwordData.InitTaskID = int.Parse(array[0]);
						MagicSwordData.InitTaskNpcID = int.Parse(array[1]);
						MagicSwordData.InitPrevTaskID = int.Parse(array[2]);
						MagicSwordData.InitMapID = int.Parse(array[3]);
						MagicSwordData.InitChangeLifeCount = int.Parse(array2[0]);
						MagicSwordData.InitLevel = int.Parse(array2[1]);
						if (null == MagicSwordData.StrengthWeaponList)
						{
							MagicSwordData.StrengthWeaponList = new List<int>();
						}
						MagicSwordData.StrengthWeaponList.Clear();
						paramValueByName = GameManager.systemParamsList.GetParamValueByName("LiMJSDaTianShi");
						array = paramValueByName.Split(new char[]
						{
							','
						});
						if (array.Length > 0)
						{
							for (int i = 0; i < array.Length; i++)
							{
								MagicSwordData.StrengthWeaponList.Add(int.Parse(array[i]));
							}
						}
						if (null == MagicSwordData.IntelligenceWeaponList)
						{
							MagicSwordData.IntelligenceWeaponList = new List<int>();
						}
						MagicSwordData.IntelligenceWeaponList.Clear();
						paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhiMJSDaTianShi");
						array = paramValueByName.Split(new char[]
						{
							','
						});
						if (array.Length > 0)
						{
							for (int i = 0; i < array.Length; i++)
							{
								MagicSwordData.IntelligenceWeaponList.Add(int.Parse(array[i]));
							}
						}
						MagicSwordData.StrAttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("LiMJSAttackSkill", -1);
						MagicSwordData.IntAttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("ZhiMJSAttackSkill", -1);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMagicSwordData", new object[0])));
			}
		}

		public bool InitMagicSwordInfo(GameClient client, EMagicSwordTowardType eType)
		{
			bool result;
			if (!this.IsVersionSystemOpenOfMagicSword())
			{
				result = false;
			}
			else if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，初始化魔剑士信息", new object[0]), null, true);
				result = false;
			}
			else if (Global.AutoUpChangeLifeAndLevel(client, MagicSwordData.InitChangeLifeCount, MagicSwordData.InitLevel))
			{
				this.AutoMaigcSwordFirstAddPoint(client, eType);
				this.AutoGiveMagicSwordGoods(client);
				this.AutoGiveMagicSwordDefaultSkillHotKey(client, eType);
				GlobalEventSource.getInstance().fireEvent(new PlayerLevelupEventObject(client));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsMagicSword(GameClient client)
		{
			return client != null && this.IsMagicSword(client.ClientData.Occupation);
		}

		public bool IsMagicSword(int nOccu)
		{
			return Global.CalcOriginalOccupationID(nOccu) == 3;
		}

		public bool IsFirstLoginMagicSword(GameClient client, int nDestChangeLifeCount)
		{
			return this.IsVersionSystemOpenOfMagicSword() && client != null && this.IsMagicSword(client) && client.ClientData.ChangeLifeCount < nDestChangeLifeCount;
		}

		public bool IsMagicSwordAngelWeapon(GameClient client, int nGoodsID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.ClientData.Occupation != 3)
			{
				result = false;
			}
			else
			{
				List<int> list;
				switch (this.GetMagicSwordTowardType(client))
				{
				case EMagicSwordTowardType.EMST_Strength:
					list = MagicSwordData.StrengthWeaponList;
					break;
				case EMagicSwordTowardType.EMST_Intelligence:
					list = MagicSwordData.IntelligenceWeaponList;
					break;
				default:
					return false;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (nGoodsID == list[i])
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool IsMagicSwordWeapon(int nGoodsID)
		{
			SystemXmlItem systemXmlItem = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemXmlItem))
			{
				result = false;
			}
			else
			{
				ItemCategories intValue = (ItemCategories)systemXmlItem.GetIntValue("Categoriy", -1);
				bool flag = false;
				switch (intValue)
				{
				case ItemCategories.WuQi_Jian:
				case ItemCategories.WuQi_Fu:
				case ItemCategories.WuQi_Chui:
				case ItemCategories.WuQi_Mao:
				case ItemCategories.WuQi_Zhang:
				case ItemCategories.WuQi_Dun:
				case ItemCategories.WuQi_Dao:
					flag = true;
					break;
				}
				result = flag;
			}
			return result;
		}

		public bool IsVersionSystemOpenOfMagicSword()
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MagicSword") && !GameFuncControlManager.IsGameFuncDisabled(4);
		}

		public bool CanUseMagicOfMagicSword(GameClient client, int nMagicID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.buffManager.IsBuffEnabled(121))
			{
				result = true;
			}
			else if (!this.IsMagicSword(client))
			{
				result = true;
			}
			else if (!this.IsVersionSystemOpenOfMagicSword())
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(nMagicID, out systemXmlItem))
				{
					result = false;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("DamageType", -1);
					if (intValue < 0)
					{
						result = true;
					}
					else
					{
						switch (intValue)
						{
						case 1:
							if (nMagicID != 10000)
							{
								switch (nMagicID)
								{
								case 10088:
								case 10089:
								case 10090:
								case 10091:
									break;
								default:
									goto IL_122;
								}
							}
							return true;
						case 2:
							if (nMagicID != 10100)
							{
								switch (nMagicID)
								{
								case 10188:
								case 10189:
								case 10190:
								case 10191:
									break;
								default:
									goto IL_122;
								}
							}
							return true;
						}
						IL_122:
						List<GoodsData> weaponEquipList = client.UsingEquipMgr.GetWeaponEquipList();
						lock (weaponEquipList)
						{
							if (weaponEquipList == null || weaponEquipList.Count <= 0)
							{
								result = false;
							}
							else
							{
								List<GoodsData> list = new List<GoodsData>();
								List<GoodsData> list2 = new List<GoodsData>();
								foreach (GoodsData goodsData in weaponEquipList)
								{
									if (RebornEquip.IsRebornEquipShengQi(goodsData.GoodsID) || RebornEquip.IsRebornEquipShengWu(goodsData.GoodsID))
									{
										list2.Add(goodsData);
									}
									else
									{
										list.Add(goodsData);
									}
								}
								SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
								GoodsData magicWeaponGoods;
								if (mapSceneType != 54)
								{
									if (client.ClientData.RebornShowEquip == 0)
									{
										magicWeaponGoods = RebornEquip.GetMagicWeaponGoods(list, true);
									}
									else
									{
										magicWeaponGoods = RebornEquip.GetMagicWeaponGoods(list2, false);
									}
								}
								else
								{
									magicWeaponGoods = RebornEquip.GetMagicWeaponGoods(list2, false);
								}
								if (magicWeaponGoods == null)
								{
									result = false;
								}
								else
								{
									SystemXmlItem systemXmlItem2 = null;
									if (RebornEquip.IsRebornEquipShengQi(magicWeaponGoods.GoodsID) || RebornEquip.IsRebornEquipShengWu(magicWeaponGoods.GoodsID))
									{
										if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
										{
											RebornEquipXmlStruct rebornEquipXmlStruct;
											if (!RebornEquip.EquipSQSW.TryGetValue(magicWeaponGoods.GoodsID, out rebornEquipXmlStruct))
											{
												return false;
											}
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornEquipXmlStruct.LMJSModGoodID, out systemXmlItem2))
											{
												return false;
											}
										}
										else
										{
											RebornEquipXmlStruct rebornEquipXmlStruct;
											if (!RebornEquip.EquipSQSW.TryGetValue(magicWeaponGoods.GoodsID, out rebornEquipXmlStruct))
											{
												return false;
											}
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornEquipXmlStruct.FMJSModGoodID, out systemXmlItem2))
											{
												return false;
											}
										}
									}
									else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(magicWeaponGoods.GoodsID, out systemXmlItem2))
									{
										return false;
									}
									int intValue2 = systemXmlItem2.GetIntValue("Strength", -1);
									int intValue3 = systemXmlItem2.GetIntValue("Intelligence", -1);
									int num = (intValue2 >= intValue3) ? 1 : 2;
									if (num == intValue)
									{
										result = true;
									}
									else
									{
										LogManager.WriteLog(1, string.Format("武器与技能类型不符，无法释放技能: RoleID={0}, 武器id{1}, 武器类型{2}, 技能id{3}, 技能类型{4}", new object[]
										{
											client.ClientData.RoleID,
											magicWeaponGoods.GoodsID,
											num,
											nMagicID,
											intValue
										}), null, true);
										result = false;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public bool IsCanAward2MagicSword(GameClient client, int nGoodsID)
		{
			int nOccu = Global.CalcOriginalOccupationID(client);
			bool result;
			if (!this.IsMagicSword(nOccu))
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemXmlItem))
				{
					result = false;
				}
				else if (Global.GetMainOccupationByGoodsID(nGoodsID) == -1)
				{
					result = true;
				}
				else
				{
					EMagicSwordTowardType magicSwordTowardType = this.GetMagicSwordTowardType(client);
					int intValue = systemXmlItem.GetIntValue("Strength", -1);
					int intValue2 = systemXmlItem.GetIntValue("Intelligence", -1);
					EMagicSwordTowardType emagicSwordTowardType = EMagicSwordTowardType.EMST_Intelligence;
					if (intValue >= intValue2)
					{
						emagicSwordTowardType = EMagicSwordTowardType.EMST_Strength;
					}
					result = (magicSwordTowardType == emagicSwordTowardType);
				}
			}
			return result;
		}

		public EMagicSwordTowardType GetMagicSwordTowardType(GameClient client)
		{
			double strength = RoleAlgorithm.GetStrength(client, true);
			double intelligence = RoleAlgorithm.GetIntelligence(client, true);
			EMagicSwordTowardType result;
			if (strength >= intelligence)
			{
				result = EMagicSwordTowardType.EMST_Strength;
			}
			else
			{
				result = EMagicSwordTowardType.EMST_Intelligence;
			}
			return result;
		}

		public void AutoGiveMagicSwordGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法给与魔剑士新手装备", new object[0]), null, true);
			}
			else if (this.IsMagicSword(client))
			{
				int roleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> list;
					if (EMagicSwordTowardType.EMST_Strength == this.GetMagicSwordTowardType(client))
					{
						list = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("LiMJSZhuangBei"), true, '|', ',');
						if (null == list)
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备默认数据报错.RoleID{0}", roleID), null, true);
							return;
						}
						if (list.Count <= 0)
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量为空.RoleID{0}", roleID), null, true);
							return;
						}
					}
					else
					{
						list = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("ZhiMJSZhuangBei"), true, '|', ',');
						if (null == list)
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备默认数据报错.RoleID{0}", roleID), null, true);
							return;
						}
						if (list.Count <= 0)
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量为空.RoleID{0}", roleID), null, true);
							return;
						}
					}
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i][0];
						int num2 = list[i][1];
						int binding = list[i][2];
						int forgeLevel = list[i][3];
						int nAppendPropLev = list[i][4];
						int lucky = list[i][5];
						int excellenceProperty = list[i][6];
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量ID不存在:RoleID{0},GoodsID={1}", roleID, num), null, true);
						}
						else if (!Global.IsRoleOccupationMatchGoods(client, num))
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备与职业不符RoleID{0}, 物品id{1}.", roleID, num), null, true);
						}
						else if (1 != num2)
						{
							LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量必须为1件RoleID{0}, 数量{1}.", roleID, num2), null, true);
						}
						else
						{
							int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num, num2, 0, "", forgeLevel, binding, 0, "", false, 1, "自动给于魔剑士装备", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceProperty, nAppendPropLev, 0, null, null, 0, true);
							if (num3 <= 0)
							{
								LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", roleID), null, true);
							}
							else
							{
								GoodsData goodsByDbID = Global.GetGoodsByDbID(client, num3);
								if (null == goodsByDbID)
								{
									LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量[GetGoodsByID]失败.RoleID{0}", roleID), null, true);
								}
								else
								{
									int num4 = 0;
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByDbID.GoodsID);
									if (goodsCatetoriy == 6 && flag)
									{
										num4++;
									}
									string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										1,
										goodsByDbID.Id,
										goodsByDbID.GoodsID,
										1,
										goodsByDbID.Site,
										goodsByDbID.GCount,
										num4,
										""
									});
									TCPProcessCmdResults tcpprocessCmdResults = Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
									if (TCPProcessCmdResults.RESULT_FAILED == tcpprocessCmdResults)
									{
										LogManager.WriteLog(2, string.Format("魔剑士初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", roleID), null, true);
									}
									else
									{
										Global.RefreshEquipProp(client, goodsByDbID);
										if (goodsCatetoriy == 6)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		public void AutoGiveMagicSwordDefaultSkillHotKey(GameClient client, EMagicSwordTowardType eType)
		{
			if (null != client)
			{
				string text;
				switch (eType)
				{
				case EMagicSwordTowardType.EMST_Strength:
					text = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
					{
						10004,
						10000,
						10006,
						10001
					});
					break;
				case EMagicSwordTowardType.EMST_Intelligence:
					text = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
					{
						10104,
						10100,
						10106,
						10101
					});
					break;
				default:
					return;
				}
				string cmdText = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, text);
				client.ClientData.MainQuickBarKeys = text;
				GameManager.DBCmdMgr.AddDBCmd(10010, cmdText, null, client.ServerId);
			}
		}

		public EMagicSwordTowardType GetMagicSwordTypeByWeapon(int nOccu, List<GoodsData> list, GameClient client = null)
		{
			EMagicSwordTowardType result;
			lock (list)
			{
				EMagicSwordTowardType emagicSwordTowardType = EMagicSwordTowardType.EMST_Strength;
				if (!this.IsMagicSword(nOccu))
				{
					result = EMagicSwordTowardType.EMST_Not;
				}
				else if (list == null || list.Count <= 0)
				{
					result = emagicSwordTowardType;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					List<GoodsData> list2 = new List<GoodsData>();
					List<GoodsData> list3 = new List<GoodsData>();
					List<GoodsData> list4 = new List<GoodsData>();
					GoodsData goodsData;
					for (int i = 0; i < list.Count; i++)
					{
						goodsData = list[i];
						if (null != goodsData)
						{
							int goodsID = goodsData.GoodsID;
							if (RebornEquip.IsRebornEquipShengQi(goodsID) || RebornEquip.IsRebornEquipShengWu(goodsID))
							{
								list4.Add(goodsData);
							}
							else
							{
								list3.Add(goodsData);
							}
						}
					}
					if (client == null)
					{
						if (list3 == null || list3.Count <= 0)
						{
							return emagicSwordTowardType;
						}
						goodsData = RebornEquip.GetMagicWeaponGoods(list3, true);
					}
					else
					{
						SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
						if (mapSceneType != 54)
						{
							if (client.ClientData.RebornShowEquip == 0)
							{
								goodsData = RebornEquip.GetMagicWeaponGoods(list3, true);
							}
							else
							{
								if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
								{
									return EMagicSwordTowardType.EMST_Strength;
								}
								return EMagicSwordTowardType.EMST_Intelligence;
							}
						}
						else
						{
							if (client.ClientData.PropStrength >= client.ClientData.PropIntelligence)
							{
								return EMagicSwordTowardType.EMST_Strength;
							}
							return EMagicSwordTowardType.EMST_Intelligence;
						}
					}
					if (null == goodsData)
					{
						result = emagicSwordTowardType;
					}
					else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
					{
						result = emagicSwordTowardType;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("Strength", -1);
						int intValue2 = systemXmlItem.GetIntValue("Intelligence", -1);
						emagicSwordTowardType = ((intValue >= intValue2) ? EMagicSwordTowardType.EMST_Strength : EMagicSwordTowardType.EMST_Intelligence);
						result = emagicSwordTowardType;
					}
				}
			}
			return result;
		}

		public void AutoMaigcSwordFirstAddPoint(GameClient client, EMagicSwordTowardType eType)
		{
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法根据参数表配置第一次给魔剑士加点", new object[0]), null, true);
			}
			else if (this.IsMagicSword(client))
			{
				if (eType == EMagicSwordTowardType.EMST_Strength || eType == EMagicSwordTowardType.EMST_Intelligence)
				{
					int roleID = client.ClientData.RoleID;
					try
					{
						string paramValueByName;
						if (eType == EMagicSwordTowardType.EMST_Strength)
						{
							paramValueByName = GameManager.systemParamsList.GetParamValueByName("LiMJS");
						}
						else
						{
							paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhiMJS");
						}
						string[] array = paramValueByName.Split(new char[]
						{
							','
						});
						if (array.Length != 4)
						{
							LogManager.WriteLog(2, string.Format("魔剑士读取初始加点失败，无法创建魔剑士, RoleID={0}", roleID), null, true);
						}
						else
						{
							int num = 0;
							for (int i = 0; i < array.Length; i++)
							{
								num += int.Parse(array[i]);
							}
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
							int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "PropStrength");
							int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "PropIntelligence");
							int roleParamsInt32FromDB4 = Global.GetRoleParamsInt32FromDB(client, "PropDexterity");
							int roleParamsInt32FromDB5 = Global.GetRoleParamsInt32FromDB(client, "PropConstitution");
							int num2 = roleParamsInt32FromDB - roleParamsInt32FromDB2 - roleParamsInt32FromDB3 - roleParamsInt32FromDB4 - roleParamsInt32FromDB5;
							if (num2 < num)
							{
								LogManager.WriteLog(2, string.Format("魔剑士初始加点不足，无法创建魔剑士, RoleID={0}", roleID), null, true);
							}
							else
							{
								client.ClientData.PropStrength += int.Parse(array[0]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
								client.ClientData.PropIntelligence += int.Parse(array[1]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
								client.ClientData.PropDexterity += int.Parse(array[2]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
								client.ClientData.PropConstitution += int.Parse(array[3]);
								Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitution", client.ClientData.PropConstitution, true);
								client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
								client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
								if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
								{
									client.ClientData.CurrentLifeV = client.ClientData.LifeV;
								}
								if (client.ClientData.CurrentMagicV > client.ClientData.MagicV)
								{
									client.ClientData.CurrentMagicV = client.ClientData.MagicV;
								}
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							}
						}
					}
					catch (Exception e)
					{
						DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
					}
				}
			}
		}
	}
}
