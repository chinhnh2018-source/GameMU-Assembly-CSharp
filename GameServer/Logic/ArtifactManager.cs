using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class ArtifactManager : IManager
	{
		public static ArtifactManager GetInstance()
		{
			return ArtifactManager.Instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
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

		public static void initArtifact()
		{
			ArtifactManager.LoadArtifactData();
			ArtifactManager.LoadArtifactSuitData();
		}

		public static void LoadArtifactData()
		{
			string uri = "Config/ZaiZao.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ZaiZao.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ArtifactManager._artifactList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							ArtifactData artifactData = new ArtifactData();
							artifactData.ArtifactID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							artifactData.ArtifactName = Convert.ToString(Global.GetDefAttributeStr(xelement2, "Name", ""));
							artifactData.NewEquitID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NewEquitID", "0"));
							artifactData.NeedEquitID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedEquitID", "0"));
							artifactData.NeedGoldBind = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedBandJinBi", "0"));
							artifactData.NeedZaiZao = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedZaiZao", "0"));
							artifactData.SuccessRate = (int)(Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "SuccessRate", "0")) * 100.0);
							string text = Convert.ToString(Global.GetDefAttributeStr(xelement2, "NeedGoods", ""));
							if (text.Length > 0)
							{
								artifactData.NeedMaterial = new Dictionary<int, int>();
								string[] array = text.Split(new char[]
								{
									'|'
								});
								foreach (string text2 in array)
								{
									string[] array3 = text2.Split(new char[]
									{
										','
									});
									int key = int.Parse(array3[0]);
									int value = int.Parse(array3[1]);
									artifactData.NeedMaterial.Add(key, value);
								}
							}
							string text3 = Convert.ToString(Global.GetDefAttributeStr(xelement2, "XiaoHuiGoods", ""));
							if (text3.Length > 0)
							{
								artifactData.FailMaterial = new Dictionary<int, int>();
								string[] array = text3.Split(new char[]
								{
									'|'
								});
								foreach (string text2 in array)
								{
									string[] array3 = text2.Split(new char[]
									{
										','
									});
									int key = int.Parse(array3[0]);
									int value = int.Parse(array3[1]);
									artifactData.FailMaterial.Add(key, value);
								}
							}
							ArtifactManager._artifactList.Add(artifactData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ZaiZao.xml时文件出错", ex, true);
				}
			}
		}

		public static ArtifactData GetArtifactDataByNeedId(int needID)
		{
			foreach (ArtifactData artifactData in ArtifactManager._artifactList)
			{
				if (artifactData.NeedEquitID == needID)
				{
					return artifactData;
				}
			}
			return null;
		}

		public static void LoadArtifactSuitData()
		{
			string uri = "Config/TaoZhuangProps.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/TaoZhuangProps.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ArtifactManager._artifactSuitList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							ArtifactSuitData artifactSuitData = new ArtifactSuitData();
							artifactSuitData.SuitID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							artifactSuitData.SuitName = Convert.ToString(Global.GetDefAttributeStr(xelement2, "Name", ""));
							artifactSuitData.IsMulti = (Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Multi", "0")) > 0);
							string text = Convert.ToString(Global.GetDefAttributeStr(xelement2, "GoodsID", "0"));
							if (text.Length > 0)
							{
								artifactSuitData.EquipIDList = new List<int>();
								string[] array = text.Split(new char[]
								{
									','
								});
								foreach (string s in array)
								{
									artifactSuitData.EquipIDList.Add(int.Parse(s));
								}
							}
							string text2 = Convert.ToString(Global.GetDefAttributeStr(xelement2, "TaoZhuangProps", ""));
							if (text2.Length > 0)
							{
								artifactSuitData.SuitAttr = new Dictionary<int, Dictionary<string, string>>();
								string[] array3 = text2.Split(new char[]
								{
									'|'
								});
								foreach (string text3 in array3)
								{
									string[] array4 = text3.Split(new char[]
									{
										','
									});
									int key = int.Parse(array4[0]);
									if (artifactSuitData.SuitAttr.ContainsKey(key))
									{
										artifactSuitData.SuitAttr[key].Add(array4[1], array4[2]);
									}
									else
									{
										Dictionary<string, string> dictionary = new Dictionary<string, string>();
										dictionary.Add(array4[1], array4[2]);
										artifactSuitData.SuitAttr.Add(int.Parse(array4[0]), dictionary);
									}
								}
							}
							ArtifactManager._artifactSuitList.Add(artifactSuitData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/TaoZhuangProps.xml时文件出错", ex, true);
				}
			}
		}

		public static ArtifactSuitData GetArtifactSuitDataByEquipID(int equipID)
		{
			foreach (ArtifactSuitData artifactSuitData in ArtifactManager._artifactSuitList)
			{
				foreach (int num in artifactSuitData.EquipIDList)
				{
					if (num == equipID)
					{
						return artifactSuitData;
					}
				}
			}
			return null;
		}

		public static ArtifactSuitData GetArtifactSuitDataBySuitID(int suitID)
		{
			foreach (ArtifactSuitData artifactSuitData in ArtifactManager._artifactSuitList)
			{
				if (artifactSuitData.SuitID == suitID)
				{
					return artifactSuitData;
				}
			}
			return null;
		}

		public static ArtifactResultData UpArtifact(GameClient client, int equipID, bool isUseBind)
		{
			ArtifactResultData artifactResultData = new ArtifactResultData();
			ArtifactResultData result;
			if (!GlobalNew.IsGongNengOpened(client, 54, false))
			{
				artifactResultData.State = -1;
				result = artifactResultData;
			}
			else
			{
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, equipID);
				if (goodsByDbID == null)
				{
					artifactResultData.State = -2;
					result = artifactResultData;
				}
				else
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByDbID.GoodsID);
					if (!GoodsUtil.CanUpgrade(goodsCatetoriy, 9))
					{
						if ((goodsCatetoriy < 0 || goodsCatetoriy > 6) && (goodsCatetoriy < 11 || goodsCatetoriy > 21))
						{
							artifactResultData.State = -3;
							return artifactResultData;
						}
					}
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem))
					{
						artifactResultData.State = -2;
						result = artifactResultData;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("SuitID", -1);
						ArtifactData artifactDataByNeedId = ArtifactManager.GetArtifactDataByNeedId(goodsByDbID.GoodsID);
						if (artifactDataByNeedId == null)
						{
							artifactResultData.State = -3;
							result = artifactResultData;
						}
						else if (Global.IsRoleHasEnoughMoney(client, artifactDataByNeedId.NeedZaiZao, 101) <= 0)
						{
							artifactResultData.State = -4;
							result = artifactResultData;
						}
						else
						{
							int totalBindTongQianAndTongQianVal = Global.GetTotalBindTongQianAndTongQianVal(client);
							if (artifactDataByNeedId.NeedGoldBind > totalBindTongQianAndTongQianVal)
							{
								artifactResultData.State = -5;
								result = artifactResultData;
							}
							else
							{
								foreach (KeyValuePair<int, int> keyValuePair in artifactDataByNeedId.NeedMaterial)
								{
									int key = keyValuePair.Key;
									int value = keyValuePair.Value;
									int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, key);
									if (totalGoodsCountByID < value)
									{
										artifactResultData.State = -6;
										return artifactResultData;
									}
								}
								foreach (KeyValuePair<int, int> keyValuePair in artifactDataByNeedId.FailMaterial)
								{
									int key = keyValuePair.Key;
									int value = keyValuePair.Value;
									int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, key);
									if (totalGoodsCountByID < value)
									{
										artifactResultData.State = -6;
										return artifactResultData;
									}
								}
								int idleSlotOfBagGoods = Global.GetIdleSlotOfBagGoods(client);
								if (idleSlotOfBagGoods < 0)
								{
									artifactResultData.State = -7;
									result = artifactResultData;
								}
								else if (!Global.SubBindTongQianAndTongQian(client, artifactDataByNeedId.NeedGoldBind, "神器再造"))
								{
									artifactResultData.State = -5;
									result = artifactResultData;
								}
								else
								{
									bool flag = false;
									int num = Global.GetRoleParamsInt32FromDB(client, "ArtifactFailCount");
									int num2 = (int)GameManager.systemParamsList.GetParamValueIntByName("ZaiZaoBaoDi", -1);
									if (num >= num2)
									{
										flag = true;
										num = 0;
										ArtifactManager.SetArtifactFailCount(client, num);
									}
									else
									{
										int randomNumber = Global.GetRandomNumber(0, 100);
										if (randomNumber < artifactDataByNeedId.SuccessRate)
										{
											flag = true;
											num = 0;
											ArtifactManager.SetArtifactFailCount(client, num);
										}
									}
									bool flag2 = false;
									bool flag3 = false;
									if (!flag)
									{
										foreach (KeyValuePair<int, int> keyValuePair in artifactDataByNeedId.FailMaterial)
										{
											int key = keyValuePair.Key;
											int value = keyValuePair.Value;
											if (Global.UseGoodsBindOrNot(client, key, value, isUseBind, out flag2, out flag3) < 1)
											{
												artifactResultData.State = -6;
												return artifactResultData;
											}
										}
										num++;
										Global.SaveRoleParamsInt32ValueToDB(client, "ArtifactFailCount", num, true);
										GameManager.logDBCmdMgr.AddDBLogInfo(artifactDataByNeedId.NewEquitID, artifactDataByNeedId.ArtifactName, "神器再造失败", client.ClientData.RoleName, client.ClientData.RoleName, "再造", 1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsByDbID);
										EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.ShenQiZaiZao, LogRecordType.ShenQiZaiZao, new object[]
										{
											artifactDataByNeedId.NewEquitID,
											0,
											num
										});
										artifactResultData.State = 0;
										result = artifactResultData;
									}
									else
									{
										foreach (KeyValuePair<int, int> keyValuePair in artifactDataByNeedId.NeedMaterial)
										{
											int key = keyValuePair.Key;
											int value = keyValuePair.Value;
											bool flag4 = false;
											bool flag5 = false;
											if (Global.UseGoodsBindOrNot(client, key, value, isUseBind, out flag4, out flag5) < 1)
											{
												artifactResultData.State = -6;
												return artifactResultData;
											}
											flag2 = (flag2 || flag4);
											flag3 = (flag3 || flag5);
										}
										GameManager.ClientMgr.ModifyZaiZaoValue(client, -artifactDataByNeedId.NeedZaiZao, "神器再造", true, true, false);
										EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.ShenQiZaiZao, LogRecordType.ShenQiZaiZao, new object[]
										{
											artifactDataByNeedId.NewEquitID,
											1,
											0
										});
										int forge_level = goodsByDbID.Forge_level;
										int appendPropLev = goodsByDbID.AppendPropLev;
										int lucky = goodsByDbID.Lucky;
										int excellenceInfo = goodsByDbID.ExcellenceInfo;
										List<int> washProps = goodsByDbID.WashProps;
										int juHunID = goodsByDbID.JuHunID;
										int num3 = goodsByDbID.Binding;
										List<int> elementhrtsProps = goodsByDbID.ElementhrtsProps;
										if (flag2)
										{
											num3 = 1;
										}
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, equipID, false, false))
										{
											artifactResultData.State = -8;
											result = artifactResultData;
										}
										else
										{
											int num4 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, artifactDataByNeedId.NewEquitID, 1, goodsByDbID.Quality, "", forge_level, num3, 0, goodsByDbID.Jewellist, false, 1, "神器再造", "1900-01-01 12:00:00", goodsByDbID.AddPropIndex, goodsByDbID.BornIndex, lucky, 0, excellenceInfo, appendPropLev, goodsByDbID.ChangeLifeLevForEquip, washProps, elementhrtsProps, juHunID, true);
											if (num4 < 0)
											{
												artifactResultData.State = -9;
												result = artifactResultData;
											}
											else
											{
												string msgText = StringUtil.substitute(GLang.GetLang(11, new object[0]), new object[]
												{
													Global.FormatRoleName(client, client.ClientData.RoleName),
													artifactDataByNeedId.ArtifactName,
													intValue + 1
												});
												Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
												artifactResultData.State = 1;
												artifactResultData.EquipDbID = num4;
												artifactResultData.Bind = num3;
												result = artifactResultData;
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

		public static void SetArtifactProp(GameClient client)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			if (client.ClientData.GoodsDataList != null)
			{
				lock (client.ClientData.GoodsDataList)
				{
					for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
					{
						GoodsData goodsData = client.ClientData.GoodsDataList[i];
						if (goodsData.Using > 0)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
							{
								int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
								bool flag2 = ElementhrtsManager.IsElementHrt(intValue);
								if (intValue < 49 || flag2)
								{
									ArtifactSuitData artifactSuitData = ArtifactManager.GetArtifactSuitDataByEquipID(goodsData.GoodsID);
									if (artifactSuitData != null)
									{
										if (dictionary.ContainsKey(artifactSuitData.SuitID))
										{
											bool flag3 = true;
											List<int> list = dictionary[artifactSuitData.SuitID];
											if (!artifactSuitData.IsMulti)
											{
												foreach (int num9 in list)
												{
													if (num9 == goodsData.GoodsID)
													{
														flag3 = false;
														break;
													}
												}
											}
											if (flag3)
											{
												list.Add(goodsData.GoodsID);
											}
										}
										else
										{
											List<int> list = new List<int>();
											list.Add(goodsData.GoodsID);
											dictionary.Add(artifactSuitData.SuitID, list);
										}
									}
								}
							}
						}
					}
					double[] array = new double[177];
					foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
					{
						int count = keyValuePair.Value.Count;
						if (count >= 2)
						{
							ArtifactSuitData artifactSuitData = ArtifactManager.GetArtifactSuitDataBySuitID(keyValuePair.Key);
							foreach (KeyValuePair<int, Dictionary<string, string>> keyValuePair2 in artifactSuitData.SuitAttr)
							{
								if (count >= keyValuePair2.Key)
								{
									foreach (KeyValuePair<string, string> keyValuePair3 in keyValuePair2.Value)
									{
										string[] array2 = keyValuePair3.Value.Split(new char[]
										{
											'-'
										});
										string key = keyValuePair3.Key;
										if (key == null)
										{
											goto IL_486;
										}
										if (!(key == "Attack"))
										{
											if (!(key == "Defense"))
											{
												if (!(key == "Mattack"))
												{
													if (!(key == "Mdefense"))
													{
														goto IL_486;
													}
													num7 += int.Parse(array2[0]);
													num8 += int.Parse(array2[1]);
													array[5] += (double)int.Parse(array2[0]);
													array[6] += (double)int.Parse(array2[1]);
												}
												else
												{
													num5 += int.Parse(array2[0]);
													num6 += int.Parse(array2[1]);
													array[9] += (double)int.Parse(array2[0]);
													array[10] += (double)int.Parse(array2[1]);
												}
											}
											else
											{
												num3 += int.Parse(array2[0]);
												num4 += int.Parse(array2[1]);
												array[3] += (double)int.Parse(array2[0]);
												array[4] += (double)int.Parse(array2[1]);
											}
										}
										else
										{
											num += int.Parse(array2[0]);
											num2 += int.Parse(array2[1]);
											array[7] += (double)int.Parse(array2[0]);
											array[8] += (double)int.Parse(array2[1]);
										}
										continue;
										IL_486:
										ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(keyValuePair3.Key);
										if (ExtPropIndexes.Strong <= propIndexByPropName && propIndexByPropName < ExtPropIndexes.Max)
										{
											array[(int)propIndexByPropName] += double.Parse(array2[0]);
										}
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						7,
						array
					});
				}
			}
		}

		public static void SetRebornEquipArtifactProp(GameClient client)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			if (client.ClientData.RebornGoodsDataList != null)
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						GoodsData goodsData = client.ClientData.RebornGoodsDataList[i];
						if (goodsData.Using > 0)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
							{
								if (RebornEquip.IsRebornType(goodsData.GoodsID) && RebornEquip.IsRebornEquip(goodsData.GoodsID))
								{
									ArtifactSuitData artifactSuitData = ArtifactManager.GetArtifactSuitDataByEquipID(goodsData.GoodsID);
									if (artifactSuitData != null)
									{
										if (dictionary.ContainsKey(artifactSuitData.SuitID))
										{
											bool flag2 = true;
											List<int> list = dictionary[artifactSuitData.SuitID];
											if (!artifactSuitData.IsMulti)
											{
												foreach (int num in list)
												{
													if (num == goodsData.GoodsID)
													{
														flag2 = false;
														break;
													}
												}
											}
											if (flag2)
											{
												list.Add(goodsData.GoodsID);
											}
										}
										else
										{
											List<int> list = new List<int>();
											list.Add(goodsData.GoodsID);
											dictionary.Add(artifactSuitData.SuitID, list);
										}
									}
								}
							}
						}
					}
					double[] array = new double[177];
					foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
					{
						int count = keyValuePair.Value.Count;
						if (count >= 2)
						{
							ArtifactSuitData artifactSuitData = ArtifactManager.GetArtifactSuitDataBySuitID(keyValuePair.Key);
							foreach (KeyValuePair<int, Dictionary<string, string>> keyValuePair2 in artifactSuitData.SuitAttr)
							{
								if (count >= keyValuePair2.Key)
								{
									foreach (KeyValuePair<string, string> keyValuePair3 in keyValuePair2.Value)
									{
										string key = keyValuePair3.Key;
										if (key == null)
										{
											goto IL_C25;
										}
										if (<PrivateImplementationDetails>{2EE78E64-BFB8-4874-A704-65DCF9BDB511}.$$method0x6000666-1 == null)
										{
											<PrivateImplementationDetails>{2EE78E64-BFB8-4874-A704-65DCF9BDB511}.$$method0x6000666-1 = new Dictionary<string, int>(42)
											{
												{
													"HolyAttack",
													0
												},
												{
													"HolyDefense",
													1
												},
												{
													"HolyPenetratePercent",
													2
												},
												{
													"HolyAbsorbPercent",
													3
												},
												{
													"HolyWeakPercent",
													4
												},
												{
													"HolyDoubleAttackPercent",
													5
												},
												{
													"HolyDoubleAttackInjure",
													6
												},
												{
													"ShadowAttack",
													7
												},
												{
													"ShadowDefense",
													8
												},
												{
													"ShadowPenetratePercent",
													9
												},
												{
													"ShadowAbsorbPercent",
													10
												},
												{
													"ShadowWeakPercent",
													11
												},
												{
													"ShadowDoubleAttackPercent",
													12
												},
												{
													"ShadowDoubleAttackInjure",
													13
												},
												{
													"NatureAttack",
													14
												},
												{
													"NatureDefense",
													15
												},
												{
													"NaturePenetratePercent",
													16
												},
												{
													"NatureAbsorbPercent",
													17
												},
												{
													"NatureWeakPercent",
													18
												},
												{
													"NatureDoubleAttackPercent",
													19
												},
												{
													"NatureDoubleAttackInjure",
													20
												},
												{
													"ChaosAttack",
													21
												},
												{
													"ChaosDefense",
													22
												},
												{
													"ChaosPenetratePercent",
													23
												},
												{
													"ChaosAbsorbPercent",
													24
												},
												{
													"ChaosWeakPercent",
													25
												},
												{
													"ChaosDoubleAttackPercent",
													26
												},
												{
													"ChaosDoubleAttackInjure",
													27
												},
												{
													"IncubusAttack",
													28
												},
												{
													"IncubusDefense",
													29
												},
												{
													"IncubusPenetratePercent",
													30
												},
												{
													"IncubusAbsorbPercent",
													31
												},
												{
													"IncubusWeakPercent",
													32
												},
												{
													"IncubusDoubleAttackPercent",
													33
												},
												{
													"IncubusDoubleAttackInjure",
													34
												},
												{
													"RebornAttack",
													35
												},
												{
													"RebornDefense",
													36
												},
												{
													"RebornPenetratePercent",
													37
												},
												{
													"RebornAbsorbPercent",
													38
												},
												{
													"RebornWeakPercent",
													39
												},
												{
													"RebornDoubleAttackPercent",
													40
												},
												{
													"RebornDoubleAttackInjure",
													41
												}
											};
										}
										int num2;
										if (!<PrivateImplementationDetails>{2EE78E64-BFB8-4874-A704-65DCF9BDB511}.$$method0x6000666-1.TryGetValue(key, out num2))
										{
											goto IL_C25;
										}
										switch (num2)
										{
										case 0:
											array[122] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 1:
											array[123] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 2:
											array[124] += double.Parse(keyValuePair3.Value);
											break;
										case 3:
											array[125] += double.Parse(keyValuePair3.Value);
											break;
										case 4:
											array[126] += double.Parse(keyValuePair3.Value);
											break;
										case 5:
											array[127] += double.Parse(keyValuePair3.Value);
											break;
										case 6:
											array[128] += double.Parse(keyValuePair3.Value);
											break;
										case 7:
											array[129] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 8:
											array[130] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 9:
											array[131] += double.Parse(keyValuePair3.Value);
											break;
										case 10:
											array[132] += double.Parse(keyValuePair3.Value);
											break;
										case 11:
											array[133] += double.Parse(keyValuePair3.Value);
											break;
										case 12:
											array[134] += double.Parse(keyValuePair3.Value);
											break;
										case 13:
											array[135] += double.Parse(keyValuePair3.Value);
											break;
										case 14:
											array[136] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 15:
											array[137] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 16:
											array[138] += double.Parse(keyValuePair3.Value);
											break;
										case 17:
											array[139] += double.Parse(keyValuePair3.Value);
											break;
										case 18:
											array[140] += double.Parse(keyValuePair3.Value);
											break;
										case 19:
											array[141] += double.Parse(keyValuePair3.Value);
											break;
										case 20:
											array[142] += double.Parse(keyValuePair3.Value);
											break;
										case 21:
											array[143] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 22:
											array[144] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 23:
											array[145] += double.Parse(keyValuePair3.Value);
											break;
										case 24:
											array[146] += double.Parse(keyValuePair3.Value);
											break;
										case 25:
											array[147] += double.Parse(keyValuePair3.Value);
											break;
										case 26:
											array[148] += double.Parse(keyValuePair3.Value);
											break;
										case 27:
											array[149] += double.Parse(keyValuePair3.Value);
											break;
										case 28:
											array[150] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 29:
											array[151] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 30:
											array[152] += double.Parse(keyValuePair3.Value);
											break;
										case 31:
											array[153] += double.Parse(keyValuePair3.Value);
											break;
										case 32:
											array[154] += double.Parse(keyValuePair3.Value);
											break;
										case 33:
											array[155] += double.Parse(keyValuePair3.Value);
											break;
										case 34:
											array[156] += double.Parse(keyValuePair3.Value);
											break;
										case 35:
											array[157] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 36:
											array[158] += (double)int.Parse(keyValuePair3.Value);
											break;
										case 37:
											array[159] += double.Parse(keyValuePair3.Value);
											break;
										case 38:
											array[160] += double.Parse(keyValuePair3.Value);
											break;
										case 39:
											array[161] += double.Parse(keyValuePair3.Value);
											break;
										case 40:
											array[162] += double.Parse(keyValuePair3.Value);
											break;
										case 41:
											array[163] += double.Parse(keyValuePair3.Value);
											break;
										default:
											goto IL_C25;
										}
										continue;
										IL_C25:
										ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(keyValuePair3.Key);
										if (ExtPropIndexes.Strong <= propIndexByPropName && propIndexByPropName < ExtPropIndexes.Max)
										{
											array[(int)propIndexByPropName] += double.Parse(keyValuePair3.Value);
										}
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						46,
						array
					});
				}
			}
		}

		public static void SetArtifactFailCount(GameClient client, int count)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "ArtifactFailCount", count, true);
		}

		private static ArtifactManager Instance = new ArtifactManager();

		private static List<ArtifactData> _artifactList = new List<ArtifactData>();

		private static List<ArtifactSuitData> _artifactSuitList = new List<ArtifactSuitData>();

		public enum ArtifactResultType
		{
			Success = 1,
			Fail = 0,
			EnoOpen = -1,
			EnoEquip = -2,
			EcantUp = -3,
			EnoZaiZao = -4,
			EnoGold = -5,
			EnoMaterial = -6,
			EnoBag = -7,
			EdelEquip = -8,
			EaddEquip = -9
		}
	}
}
