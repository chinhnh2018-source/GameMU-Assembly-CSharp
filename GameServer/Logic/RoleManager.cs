using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic.Name;
using GameServer.Logic.Talent;
using GameServer.Server;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class RoleManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static RoleManager getInstance()
		{
			return RoleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.PurchaseOccupationNeedZuanShi = GameManager.systemParamsList.GetParamValueIntArrayByName("PurchaseOccupation", ',');
					this.RuntimeData.PurchaseOccupationGoods = (int)GameManager.systemParamsList.GetParamValueIntByName("PurchaseOccupationGoods", 2100);
					this.RuntimeData.CanChangeOccupationMapCodes.Clear();
					text = "Config/Settings.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "Map");
					foreach (XElement xml in xelements)
					{
						int item = (int)Global.GetSafeAttributeLong(xml, "Code");
						string defAttributeStr = Global.GetDefAttributeStr(xml, "Transfer", "0");
						if (defAttributeStr == "1")
						{
							this.RuntimeData.CanChangeOccupationMapCodes.Add(item);
						}
					}
					this.RuntimeData.DisableChangeOccupationGoodsTypes.Clear();
					for (int i = 11; i <= 21; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					for (int i = 0; i <= 6; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					for (int i = 40; i <= 45; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("OccupationUnshareCategoriy", ',');
					if (null != paramValueIntArrayByName)
					{
						foreach (int item2 in paramValueIntArrayByName)
						{
							this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(item2);
						}
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1410, 2, 2, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1411, 2, 2, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1412, 7, 7, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1410:
					result = this.PurchaseOccupation(client, nID, bytes, cmdParams);
					break;
				case 1411:
					result = this.ChangeOccupation(client, nID, bytes, cmdParams);
					break;
				case 1412:
					result = this.CreateOccupationSummoner(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(12) && GlobalNew.IsGongNengOpened(client, 86, false);
		}

		private bool PurchaseOccupation(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = 0;
			int num = Global.SafeConvertToInt32(cmdParams[1]);
			int occupation = client.ClientData.Occupation;
			bool result;
			if (GameManager.SummonerMgr.IsSummoner(num) && !GameManager.SummonerMgr.IsVersionSystemOpenOfSummoner())
			{
				result = false;
			}
			else
			{
				GameManager.ClientMgr.StopClientStoryboard(client, -1);
				lock (this.RuntimeData.Mutex)
				{
					if (!this.IsGongNengOpened(client))
					{
						cmdData = -12;
					}
					else if (!client.InSafeRegion)
					{
						cmdData = -33;
					}
					else if (occupation == num)
					{
						cmdData = -18;
					}
					else if (!this.RuntimeData.CanChangeOccupationMapCodes.Contains(client.ClientData.MapCode))
					{
						cmdData = -34;
					}
					else if (client.ClientData.OccupationList.Contains(num))
					{
						cmdData = -18;
					}
					else
					{
						int num2 = client.ClientData.OccupationList.Count - 1;
						if (num2 >= Data.RoleOccupationMaxCount)
						{
							cmdData = -36;
						}
						else if (this.RuntimeData.PurchaseOccupationNeedZuanShi == null || this.RuntimeData.PurchaseOccupationNeedZuanShi.Length <= num2)
						{
							cmdData = -36;
						}
						else if (this.RuntimeData.PurchaseOccupationNeedZuanShi[num2] < 0)
						{
							cmdData = -12;
						}
						else if (client.ClientData.UserMoney < this.RuntimeData.PurchaseOccupationNeedZuanShi[num2])
						{
							cmdData = -10;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(client, this.RuntimeData.PurchaseOccupationNeedZuanShi[num2], "购买副职业", true, true, true, true, DaiBiSySType.None))
						{
							cmdData = -10;
						}
						else
						{
							client.ClientData.OccupationList.Add(num);
							string valueString = Global.ListToString<int>(client.ClientData.OccupationList, '$');
							Global.SaveRoleParamsStringToDB(client, "20017", valueString, true);
						}
					}
				}
				client.sendCmd<int>(nID, cmdData, false);
				result = true;
			}
			return result;
		}

		private bool ChangeOccupation(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = 0;
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			int occupation = client.ClientData.Occupation;
			GameManager.ClientMgr.StopClientStoryboard(client, -1);
			if (!this.IsGongNengOpened(client))
			{
				num = -12;
			}
			else if (!client.InSafeRegion)
			{
				num = -33;
			}
			else if (occupation == num2)
			{
				num = -18;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CanChangeOccupationMapCodes.Contains(client.ClientData.MapCode))
					{
						num = -34;
						goto IL_221;
					}
					if (!client.ClientData.OccupationList.Contains(num2))
					{
						num = -20;
						goto IL_221;
					}
				}
				RoleCustomData roleCustomData = Global.sendToDB<RoleCustomData, int>(10230, client.ClientData.RoleID, client.ServerId);
				if (null == roleCustomData)
				{
					roleCustomData = new RoleCustomData
					{
						roleId = client.ClientData.RoleID
					};
				}
				this.SaveRoleCustomData(client, roleCustomData);
				if (!this.StoreRoleOccGoodsList(client))
				{
					num = -15;
				}
				if (num >= 0)
				{
					string[] array = Global.SendToDB<string>(10126, string.Format("{0}:{1}", client.ClientData.RoleID, num2), client.ServerId);
					if (array[1] != num2.ToString())
					{
						num = -15;
					}
					else
					{
						EventLogManager.AddChangeOccupationEvent(client, num2);
						client.ClientData.Occupation = num2;
						client.ClientData.IsMainOccupation = (client.ClientData.OccupationList[0] == client.ClientData.Occupation);
						this.RestoreRoleCustomData(client, roleCustomData);
						this.RestoreRoleOccGoodsList(client);
						RebornManager.getInstance().InitPlayerRebornPorperty(client);
						client.sendCmd<int>(13999, client.ClientData.RoleID, false);
					}
				}
			}
			IL_221:
			if (num < 0)
			{
				client.sendCmd<int>(nID, num, false);
			}
			return true;
		}

		private bool CreateOccupationSummoner(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int goodsID = 0;
			lock (this.RuntimeData.Mutex)
			{
				goodsID = this.RuntimeData.PurchaseOccupationGoods;
			}
			bool result;
			if (!SummonerData.CreateMapSet.Contains(client.CurrentMapCode))
			{
				string cmdData = string.Format("{0}:{1}", -21, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
				{
					"",
					"",
					"",
					"",
					"",
					""
				}));
				client.sendCmd(nID, cmdData, false);
				result = true;
			}
			else
			{
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, goodsID);
				if (totalGoodsCountByID <= 0)
				{
					string cmdData = string.Format("{0}:{1}", -6, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					client.sendCmd(nID, cmdData, false);
					result = true;
				}
				else
				{
					TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(client.strUserID);
					if (null == tmsksocket)
					{
						result = true;
					}
					else
					{
						string userID = cmdParams[0];
						string userName = cmdParams[1];
						int num = Convert.ToInt32(cmdParams[2]);
						int num2 = Convert.ToInt32(cmdParams[3]);
						string[] array = cmdParams[4].Split(new char[]
						{
							'$'
						});
						int zoneID = Convert.ToInt32(cmdParams[5]);
						string deviceID = tmsksocket.deviceID;
						if (num != 1 || num2 != 5 || !GameManager.SummonerMgr.IsVersionSystemOpenOfSummoner())
						{
							string cmdData = string.Format("{0}:{1}", -12, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
							{
								"",
								"",
								"",
								"",
								"",
								""
							}));
							client.sendCmd(nID, cmdData, false);
							result = true;
						}
						else
						{
							string text = array[0];
							int num3 = NameServerNamager.CheckInvalidCharacters(text, false);
							if (num3 <= 0)
							{
								string cmdData = string.Format("{0}:{1}", num3, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
								{
									"",
									"",
									"",
									"",
									"",
									""
								}));
								client.sendCmd(nID, cmdData, false);
								result = true;
							}
							else if (!SingletonTemplate<NameManager>.Instance().IsNameLengthOK(text))
							{
								string cmdData = string.Format("{0}:{1}", -2, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
								{
									"",
									"",
									"",
									"",
									"",
									""
								}));
								client.sendCmd(nID, cmdData, false);
								result = true;
							}
							else
							{
								num3 = NameServerNamager.RegisterNameToNameServer(zoneID, userID, array, 0, 0);
								if (num3 <= 0)
								{
									string cmdData = string.Format("{0}:{1}", num3, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
									{
										"",
										"",
										"",
										"",
										"",
										""
									}));
									client.sendCmd(nID, cmdData, false);
									result = true;
								}
								else
								{
									int num4 = 0;
									if (!SingletonTemplate<CreateRoleLimitManager>.Instance().IfCanCreateRole(userID, userName, deviceID, ((IPEndPoint)tmsksocket.RemoteEndPoint).Address.ToString(), out num4))
									{
										string cmdData = string.Format("{0}:{1}", -7, num4);
										client.sendCmd(nID, cmdData, false);
										result = true;
									}
									else
									{
										string s = string.Format("{0}:{1}", new UTF8Encoding().GetString(bytes, 0, bytes.Length), 1);
										byte[] bytes2 = new UTF8Encoding().GetBytes(s);
										TCPOutPacket tcpoutPacket = null;
										TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(TCPManager.getInstance(), tmsksocket, Global._TCPManager.tcpClientPool, TCPManager.getInstance().tcpRandKey, Global._TCPManager.TcpOutPacketPool, 102, bytes2, bytes2.Length, out tcpoutPacket, tmsksocket.ServerId);
										if (null == tcpoutPacket)
										{
											result = true;
										}
										else
										{
											tcpoutPacket.PacketCmdID = (ushort)nID;
											string text2 = null;
											tcpoutPacket.GetPacketCmdData(out text2);
											client.sendCmd(tcpoutPacket, true);
											if (null != text2)
											{
												string[] array2 = text2.Split(new char[]
												{
													':'
												});
												if (array2.Length == 2 && Global.SafeConvertToInt32(array2[0]) == 1)
												{
													bool flag2 = false;
													bool flag3 = false;
													GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, false, out flag2, out flag3, false);
													SingletonTemplate<CreateRoleLimitManager>.Instance().ModifyCreateRoleNum(userID, userName, deviceID, ((IPEndPoint)tmsksocket.RemoteEndPoint).Address.ToString());
													string[] array3 = array2[1].Split(new char[]
													{
														'$'
													});
													int cmdData2 = Global.SafeConvertToInt32(array3[0]);
													client.sendCmd<int>(13999, cmdData2, false);
												}
											}
											result = true;
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

		public bool StoreRoleOccGoodsList(GameClient client)
		{
			bool flag = true;
			List<GoodsData> usingGoodsList = Global.GetUsingGoodsList(client.ClientData);
			int site = 1000 + client.ClientData.Occupation;
			foreach (GoodsData goodsData in usingGoodsList)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				if (this.RuntimeData.DisableChangeOccupationGoodsTypes.Contains(goodsCatetoriy))
				{
					goodsData.Site = site;
					goodsData.Using = 0;
					if (!Global.UpdateGoodsSiteAndUsingToDB(client, goodsData))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				lock (client.ClientData.GoodsDataList)
				{
					foreach (GoodsData goodsData in usingGoodsList)
					{
						client.ClientData.GoodsDataList.Remove(goodsData);
					}
				}
			}
			return flag;
		}

		public void RestoreRoleOccGoodsList(GameClient client)
		{
			int num = 1000 + client.ClientData.Occupation;
			List<GoodsData> list = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, num), client.ServerId);
			if (list != null && list.Count > 0)
			{
				lock (client.ClientData.GoodsDataList)
				{
					foreach (GoodsData goodsData in list)
					{
						goodsData.Site = 0;
						goodsData.Using = 1;
						Global.UpdateGoodsSiteAndUsingToDB(client, goodsData);
						client.ClientData.GoodsDataList.Add(goodsData);
					}
				}
			}
		}

		private void RestoreRoleCustomData(GameClient client, RoleCustomData customData)
		{
			RoleCustomDataItem roleCustomDataItem = null;
			if (customData != null && customData.customDataList != null && customData.customDataList.Count > 0)
			{
				roleCustomDataItem = customData.customDataList.Find((RoleCustomDataItem x) => x.Occupation == client.ClientData.Occupation);
			}
			if (null == roleCustomDataItem)
			{
				roleCustomDataItem = new RoleCustomDataItem();
			}
			client.ClientData.MainQuickBarKeys = roleCustomDataItem.Main_quick_keys;
			GameManager.DBCmdMgr.AddDBCmd(10010, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, roleCustomDataItem.Main_quick_keys), null, client.ServerId);
			int[] array = new int[4];
			for (int i = 0; i < 4; i++)
			{
				if (roleCustomDataItem.RolePointList != null && i < roleCustomDataItem.RolePointList.Count)
				{
					array[i] = roleCustomDataItem.RolePointList[i];
				}
			}
			client.ClientData.PropStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless") + array[0];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
			client.ClientData.PropIntelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless") + array[1];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
			client.ClientData.PropDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless") + array[2];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
			client.ClientData.PropConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless") + array[3];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitution", client.ClientData.PropConstitution, true);
			TalentManager.DBTalentEffectClear(client.ClientData.RoleID, client.ClientData.ZoneID, client.ServerId);
			foreach (TalentEffectItem talentEffectItem in roleCustomDataItem.EffectList)
			{
				TalentManager.ModifyEffect(client, talentEffectItem.ID, talentEffectItem.TalentType, talentEffectItem.Level);
			}
			client.ClientData.DefaultSkillLev = roleCustomDataItem.DefaultSkillLev;
			Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillLev", client.ClientData.DefaultSkillLev, true);
			client.ClientData.DefaultSkillUseNum = roleCustomDataItem.DefaultSkillUseNum;
			Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillUseNum", client.ClientData.DefaultSkillUseNum, true);
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > 0)
			{
				for (int i = 0; i < client.ClientData.FuWenTabList.Count; i++)
				{
					int skillEquip = 0;
					List<int> shenShiActiveList = new List<int>();
					if (roleCustomDataItem.ShenShiEuipSkill != null && i < roleCustomDataItem.ShenShiEuipSkill.Count)
					{
						skillEquip = roleCustomDataItem.ShenShiEuipSkill[i].SkillEquip;
						shenShiActiveList = roleCustomDataItem.ShenShiEuipSkill[i].ShenShiActiveList;
					}
					client.ClientData.FuWenTabList[i].SkillEquip = skillEquip;
					client.ClientData.FuWenTabList[i].ShenShiActiveList = shenShiActiveList;
					Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[i], 0);
				}
			}
		}

		private void SaveRoleCustomData(GameClient client, RoleCustomData customData)
		{
			int num = -1;
			RoleCustomDataItem roleCustomDataItem = null;
			RoleCustomDataItem roleCustomDataItem2 = null;
			if (customData != null && customData.customDataList != null && customData.customDataList.Count > 0)
			{
				num = customData.customDataList.FindIndex((RoleCustomDataItem x) => x.Occupation == client.ClientData.Occupation);
				if (num >= 0)
				{
					roleCustomDataItem = customData.customDataList[num];
				}
			}
			roleCustomDataItem2 = new RoleCustomDataItem
			{
				Occupation = client.ClientData.Occupation
			};
			roleCustomDataItem2.Main_quick_keys = client.ClientData.MainQuickBarKeys;
			roleCustomDataItem2.RolePointList.Add(client.ClientData.PropStrength - Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless"));
			roleCustomDataItem2.RolePointList.Add(client.ClientData.PropIntelligence - Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless"));
			roleCustomDataItem2.RolePointList.Add(client.ClientData.PropDexterity - Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless"));
			roleCustomDataItem2.RolePointList.Add(client.ClientData.PropConstitution - Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless"));
			for (int i = 0; i < roleCustomDataItem2.RolePointList.Count; i++)
			{
				if (roleCustomDataItem2.RolePointList[i] < 0)
				{
					roleCustomDataItem2.RolePointList[i] = 0;
				}
			}
			int j = roleCustomDataItem2.RolePointList.Sum();
			while (j > client.ClientData.TotalPropPoint)
			{
				for (int i = 0; i < roleCustomDataItem2.RolePointList.Count; i++)
				{
					if (roleCustomDataItem2.RolePointList[i] > 0)
					{
						List<int> rolePointList;
						int index;
						(rolePointList = roleCustomDataItem2.RolePointList)[index = i] = rolePointList[index] - 1;
						j--;
					}
				}
			}
			TalentData myTalentData = client.ClientData.MyTalentData;
			if (myTalentData != null && null != myTalentData.EffectList)
			{
				foreach (TalentEffectItem talentEffectItem in myTalentData.EffectList)
				{
					roleCustomDataItem2.EffectList.Add(new TalentEffectItem
					{
						ID = talentEffectItem.ID,
						Level = talentEffectItem.Level,
						TalentType = talentEffectItem.TalentType
					});
				}
			}
			roleCustomDataItem2.EffectList.Sort((TalentEffectItem x, TalentEffectItem y) => x.ID - y.ID);
			roleCustomDataItem2.DefaultSkillLev = client.ClientData.DefaultSkillLev;
			roleCustomDataItem2.DefaultSkillUseNum = client.ClientData.DefaultSkillUseNum;
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > 0)
			{
				List<SkillEquipData> list = new List<SkillEquipData>();
				foreach (FuWenTabData fuWenTabData in client.ClientData.FuWenTabList)
				{
					list.Add(new SkillEquipData
					{
						SkillEquip = fuWenTabData.SkillEquip,
						ShenShiActiveList = fuWenTabData.ShenShiActiveList
					});
				}
				roleCustomDataItem2.ShenShiEuipSkill = list;
			}
			bool flag = false;
			if (null == roleCustomDataItem)
			{
				customData.customDataList.Add(roleCustomDataItem2);
			}
			else
			{
				if (null == roleCustomDataItem.RolePointList)
				{
					flag = true;
				}
				else if (roleCustomDataItem.RolePointList.Count != roleCustomDataItem2.RolePointList.Count)
				{
					flag = true;
				}
				else
				{
					int i = 0;
					while (i < roleCustomDataItem.RolePointList.Count && i < roleCustomDataItem2.RolePointList.Count)
					{
						if (roleCustomDataItem.RolePointList[i] != roleCustomDataItem2.RolePointList[i])
						{
							flag = true;
							break;
						}
						i++;
					}
				}
				if (null == roleCustomDataItem.EffectList)
				{
					flag = true;
				}
				else if (roleCustomDataItem.EffectList.Count != roleCustomDataItem2.EffectList.Count)
				{
					flag = true;
				}
				else
				{
					roleCustomDataItem.EffectList.Sort((TalentEffectItem x, TalentEffectItem y) => x.ID - y.ID);
					for (int i = 0; i < roleCustomDataItem.EffectList.Count; i++)
					{
						TalentEffectItem talentEffectItem2 = roleCustomDataItem.EffectList[i];
						TalentEffectItem talentEffectItem3 = roleCustomDataItem2.EffectList[i];
						if (talentEffectItem2.ID != talentEffectItem3.ID || talentEffectItem2.Level != talentEffectItem3.Level)
						{
							flag = true;
							break;
						}
					}
				}
				if (roleCustomDataItem.Main_quick_keys != roleCustomDataItem2.Main_quick_keys)
				{
					flag = true;
				}
				if (roleCustomDataItem2.DefaultSkillLev != roleCustomDataItem.DefaultSkillLev || roleCustomDataItem2.DefaultSkillUseNum != roleCustomDataItem.DefaultSkillUseNum)
				{
					flag = true;
				}
				if (null != roleCustomDataItem2.ShenShiEuipSkill)
				{
					flag = true;
				}
				if (flag)
				{
					customData.customDataList[num] = roleCustomDataItem2;
				}
			}
			if (client.ClientData.IsMainOccupation)
			{
				customData.roleData4Selector = Global.RoleDataEx2RoleData4Selector(client.ClientData.GetRoleData());
			}
			Global.sendToDB<int, RoleCustomData>(10233, customData, client.ServerId);
		}

		public RoleData4Selector GetMainOccupationRoleDataForSelector(int roleId, int serverId)
		{
			RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, int>(10232, roleId, serverId);
			RoleData4Selector result;
			if (roleData4Selector == null || roleData4Selector.RoleID < 0)
			{
				result = null;
			}
			else
			{
				result = roleData4Selector;
			}
			return result;
		}

		public RoleDataMini GetRoleDataMiniFromRoleData4Selector(RoleData4Selector roleData4Selector)
		{
			RoleDataMini roleDataMini = null;
			if (roleData4Selector != null && roleData4Selector.RoleID > 0)
			{
				roleDataMini = new RoleDataMini();
				roleDataMini.RoleID = roleData4Selector.RoleID;
				roleDataMini.RoleName = roleData4Selector.RoleName;
				roleDataMini.RoleSex = roleData4Selector.RoleSex;
				roleDataMini.Occupation = roleData4Selector.Occupation;
				roleDataMini.SubOccupation = roleData4Selector.SubOccupation;
				roleDataMini.OccupationList = roleData4Selector.OccupationList;
				roleDataMini.Level = roleData4Selector.Level;
				roleDataMini.Faction = roleData4Selector.Faction;
				roleDataMini.MyWingData = roleData4Selector.MyWingData;
				roleDataMini.GoodsDataList = roleData4Selector.GoodsDataList;
				roleDataMini.OtherName = roleData4Selector.OtherName;
				roleDataMini.ZoneID = roleData4Selector.ZoneId;
				roleDataMini.SettingBitFlags = roleData4Selector.SettingBitFlags;
				for (int i = roleDataMini.RoleCommonUseIntPamams.Count; i < 53; i++)
				{
					roleDataMini.RoleCommonUseIntPamams.Add(0);
				}
				roleDataMini.RoleCommonUseIntPamams[26] = roleData4Selector.FashionWingsID;
				roleDataMini.RoleCommonUseIntPamams[40] = roleData4Selector.BuffFashionID;
				roleDataMini.LifeV = (roleDataMini.MaxLifeV = 100);
				roleDataMini.MagicV = (roleDataMini.MaxMagicV = 100);
				roleDataMini.JunTuanId = roleData4Selector.JunTuanId;
				roleDataMini.JunTuanName = roleData4Selector.JunTuanName;
				roleDataMini.JunTuanZhiWu = roleData4Selector.JunTuanZhiWu;
				roleDataMini.LingDi = roleData4Selector.LingDi;
				roleDataMini.HuiJiData = roleData4Selector.HuiJiData;
				roleDataMini.CompType = roleData4Selector.CompType;
				roleDataMini.CompZhiWu = roleData4Selector.CompZhiWu;
			}
			return roleDataMini;
		}

		public void CheckSkillDataValid(GameClient client)
		{
			if (client.ClientData.SkillDataList != null)
			{
				lock (client.ClientData.SkillDataList)
				{
					List<SkillData> list = new List<SkillData>();
					foreach (SkillData skillData in client.ClientData.SkillDataList)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillData.SkillID, out systemXmlItem) && systemXmlItem.GetIntValue("ToOcuupation", -1) != client.ClientData.Occupation)
						{
							list.Add(skillData);
						}
					}
					foreach (SkillData skillData in list)
					{
						client.ClientData.SkillDataList.Remove(skillData);
					}
				}
			}
		}

		public int GetRoleIDByRoleName(string roleName, int serverID)
		{
			int num = RoleName2IDs.FindRoleIDByName(roleName, false);
			if (num <= 0)
			{
				string[] array = Global.ExecuteDBCmd(10088, roleName, serverID);
				if (array == null || array.Length != 2 || int.Parse(array[0]) < 0)
				{
					return -15;
				}
				num = int.Parse(array[0]);
			}
			return num;
		}

		public const SceneUIClasses _sceneType = 40;

		public const GameTypes _gameType = 17;

		private static RoleManager instance = new RoleManager();

		private RoleManagerData RuntimeData = new RoleManagerData();
	}
}
