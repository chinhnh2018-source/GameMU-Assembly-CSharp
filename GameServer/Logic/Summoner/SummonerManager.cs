using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Summoner
{
	public class SummonerManager
	{
		public void LoadSummonerData()
		{
			try
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZHSChuShi");
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(2, "召唤师静态数据有误，无法读取", null, true);
				}
				else
				{
					string[] array2 = array[4].Split(new char[]
					{
						','
					});
					if (array2.Length != 2)
					{
						LogManager.WriteLog(2, "召唤师静态数据转生与级数有误，无法读取", null, true);
					}
					else
					{
						SummonerData.InitTaskID = int.Parse(array[0]);
						SummonerData.InitTaskNpcID = int.Parse(array[1]);
						SummonerData.InitPrevTaskID = int.Parse(array[2]);
						SummonerData.InitMapID = int.Parse(array[3]);
						SummonerData.InitChangeLifeCount = int.Parse(array2[0]);
						SummonerData.InitLevel = int.Parse(array2[1]);
						if (null == SummonerData.WeaponList)
						{
							SummonerData.WeaponList = new List<int>();
						}
						SummonerData.WeaponList.Clear();
						paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZHSDaTianShi");
						array = paramValueByName.Split(new char[]
						{
							','
						});
						if (array.Length > 0)
						{
							for (int i = 0; i < array.Length; i++)
							{
								SummonerData.WeaponList.Add(int.Parse(array[i]));
							}
						}
						if (null == SummonerData.CreateMapSet)
						{
							SummonerData.CreateMapSet = new HashSet<int>();
						}
						SummonerData.CreateMapSet.Clear();
						paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZHSCreateMap");
						array = paramValueByName.Split(new char[]
						{
							','
						});
						if (array.Length > 0 && paramValueByName != "")
						{
							for (int i = 0; i < array.Length; i++)
							{
								SummonerData.CreateMapSet.Add(int.Parse(array[i]));
							}
						}
						SummonerData.AttackID = (int)GameManager.systemParamsList.GetParamValueIntByName("ZHSAttackSkill", -1);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadSummonerData", new object[0])));
			}
		}

		public bool InitSummonerInfo(GameClient client)
		{
			bool result;
			if (!this.IsVersionSystemOpenOfSummoner())
			{
				result = false;
			}
			else if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，初始化召唤师信息", new object[0]), null, true);
				result = false;
			}
			else if (Global.AutoUpChangeLifeAndLevel(client, SummonerData.InitChangeLifeCount, SummonerData.InitLevel))
			{
				this.AutoSummonerFirstAddPoint(client);
				this.AutoGiveSummonerGoods(client);
				this.AutoGiveSummonerDefaultSkillHotKey(client);
				GlobalEventSource.getInstance().fireEvent(new PlayerLevelupEventObject(client));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsSummoner(int nOccu)
		{
			return Global.CalcOriginalOccupationID(nOccu) == 5;
		}

		public bool IsFirstLoginSummoner(GameClient client, int nDestChangeLifeCount)
		{
			return this.IsVersionSystemOpenOfSummoner() && client != null && this.IsSummoner(client.ClientData.Occupation) && client.ClientData.ChangeLifeCount < nDestChangeLifeCount;
		}

		public bool IsSummonerWeapon(GameClient client, int nGoodsID)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (client.ClientData.Occupation != 5)
			{
				result = false;
			}
			else
			{
				List<int> weaponList = SummonerData.WeaponList;
				for (int i = 0; i < weaponList.Count; i++)
				{
					if (nGoodsID == weaponList[i])
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool IsVersionSystemOpenOfSummoner()
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Summoner") && !GameFuncControlManager.IsGameFuncDisabled(12);
		}

		public void AutoGiveSummonerGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法给与召唤师新手装备", new object[0]), null, true);
			}
			else if (this.IsSummoner(client.ClientData.Occupation))
			{
				int roleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> list = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("ZHSZhuangBei"), true, '|', ',');
					if (null == list)
					{
						LogManager.WriteLog(2, string.Format("召唤师初始化装备默认数据报错.RoleID{0}", roleID), null, true);
					}
					else if (list.Count <= 0)
					{
						LogManager.WriteLog(2, string.Format("召唤师初始化装备数量为空.RoleID{0}", roleID), null, true);
					}
					else
					{
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
								LogManager.WriteLog(2, string.Format("召唤师初始化装备数量ID不存在:RoleID{0},GoodsID={1}", roleID, num), null, true);
							}
							else if (!Global.IsRoleOccupationMatchGoods(client, num))
							{
								LogManager.WriteLog(2, string.Format("召唤师初始化装备与职业不符RoleID{0}, 物品id{1}.", roleID, num), null, true);
							}
							else if (1 != num2)
							{
								LogManager.WriteLog(2, string.Format("召唤师初始化装备数量必须为1件RoleID{0}, 数量{1}.", roleID, num2), null, true);
							}
							else
							{
								int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num, num2, 0, "", forgeLevel, binding, 0, "", false, 1, "自动给于召唤师装备", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceProperty, nAppendPropLev, 0, null, null, 0, true);
								if (num3 <= 0)
								{
									LogManager.WriteLog(2, string.Format("召唤师初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", roleID), null, true);
								}
								else
								{
									GoodsData goodsByDbID = Global.GetGoodsByDbID(client, num3);
									if (null == goodsByDbID)
									{
										LogManager.WriteLog(2, string.Format("召唤师初始化装备数量[GetGoodsByID]失败.RoleID{0}", roleID), null, true);
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
											LogManager.WriteLog(2, string.Format("召唤师初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", roleID), null, true);
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
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		public void AutoGiveSummonerDefaultSkillHotKey(GameClient client)
		{
			if (null != client)
			{
				string text = string.Format("0@{0}|0@{1}|0@{2}|0@{3}", new object[]
				{
					11006,
					11000,
					11004,
					11001
				});
				string cmdText = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, text);
				client.ClientData.MainQuickBarKeys = text;
				GameManager.DBCmdMgr.AddDBCmd(10010, cmdText, null, client.ServerId);
			}
		}

		public void AutoSummonerFirstAddPoint(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法根据参数表配置第一次给召唤师加点", new object[0]), null, true);
			}
			else if (this.IsSummoner(client.ClientData.Occupation))
			{
				int roleID = client.ClientData.RoleID;
				try
				{
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZHSShuXing");
					string[] array = paramValueByName.Split(new char[]
					{
						','
					});
					if (array.Length != 4)
					{
						LogManager.WriteLog(2, string.Format("召唤师读取初始加点失败，无法创建召唤师, RoleID={0}", roleID), null, true);
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
							LogManager.WriteLog(2, string.Format("召唤师初始加点不足，无法创建召唤师, RoleID={0}", roleID), null, true);
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
