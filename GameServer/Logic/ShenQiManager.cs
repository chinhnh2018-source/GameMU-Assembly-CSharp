using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class ShenQiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static ShenQiManager getInstance()
		{
			return ShenQiManager.instance;
		}

		public bool initialize()
		{
			this.LoadArtifactXml();
			this.LoadToughnessXml();
			this.LoadGodXml();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1816, 1, 1, ShenQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1817, 2, 2, ShenQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (GameFuncControlManager.IsGameFuncDisabled(13))
			{
				result = false;
			}
			else
			{
				switch (nID)
				{
				case 1816:
					result = this.ProcessShenQiInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1817:
					result = this.ProcessShenQiUpCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ProcessShenQiInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				ShenQiData shenQiData = ShenQiManager.GetShenQiData(client);
				client.sendCmd<ShenQiData>(nID, shenQiData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessShenQiUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int useBaoJi = Convert.ToInt32(cmdParams[1]);
				ShenQiData cmdData = this.LevelUpShenQiData(client, useBaoJi);
				client.sendCmd<ShenQiData>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void LoadArtifactXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenQiConsts.Artifact);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, ArtifactItem> dictionary = new Dictionary<int, ArtifactItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "Name", "");
							int[] propArray = new int[]
							{
								Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LifeV", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddAttack", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddDefense", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Toughness", "0"))
							};
							string[] array = Global.GetDefAttributeStr(xelement2, "QiangHua", "").Split(new char[]
							{
								'|'
							});
							int[] array2 = new int[3];
							int[][] array3 = new int[3][];
							for (int i = 0; i < array.Length; i++)
							{
								string[] array4 = array[i].Split(new char[]
								{
									','
								});
								array2[i] = (int)(Convert.ToDouble(array4[0]) * 100.0);
								array3[i] = new int[4];
								array3[i][0] = Convert.ToInt32(array4[1]);
								array3[i][1] = Convert.ToInt32(array4[2]);
								array3[i][2] = Convert.ToInt32(array4[3]);
								array3[i][3] = Convert.ToInt32(array4[4]);
							}
							int costShenLiJingHua = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "CostShenLiJingHua", "0"));
							int costGoldCoin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "CostGoldCoin", "0"));
							int costDiamond = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "CostDiamond", "0"));
							int costGoldGoodsID = 0;
							int costGoldGoodsNum = 0;
							string[] array5 = Global.GetDefAttributeStr(xelement2, "CostGoldGoods", "").Split(new char[]
							{
								','
							});
							if (array5.Length == 2)
							{
								costGoldGoodsID = Convert.ToInt32(array5[0]);
								costGoldGoodsNum = Convert.ToInt32(array5[1]);
							}
							dictionary[num] = new ArtifactItem
							{
								ID = num,
								Name = defAttributeStr,
								PropArray = propArray,
								QiangHuaRate = array2,
								QiangHuaArray = array3,
								CostShenLiJingHua = costShenLiJingHua,
								CostGoldCoin = costGoldCoin,
								CostDiamond = costDiamond,
								CostGoldGoodsID = costGoldGoodsID,
								CostGoldGoodsNum = costGoldGoodsNum
							};
						}
					}
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadToughnessXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenQiConsts.Toughness);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					List<ToughnessItem> list = new List<ToughnessItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int toughness = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Toughness", "0"));
							double deLucky = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeLucky", "0"));
							double deFatalAttack = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeFatalAttack", "0"));
							double deDoubleAttack = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeDoubleAttack", "0"));
							double deSavagePercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeSavagePercent", "0"));
							double deColdPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeColdPercent", "0"));
							double deRuthlessPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeRuthlessPercent", "0"));
							double deFrozenPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeFrozenPercent", "0"));
							double dePalsyPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DePalsyPercent", "0"));
							double deSpeedDownPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeSpeedDownPercent", "0"));
							double deBlowPercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DeBlowPercent", "0"));
							list.Add(new ToughnessItem
							{
								Toughness = toughness,
								DeLucky = deLucky,
								DeFatalAttack = deFatalAttack,
								DeDoubleAttack = deDoubleAttack,
								DeSavagePercent = deSavagePercent,
								DeColdPercent = deColdPercent,
								DeRuthlessPercent = deRuthlessPercent,
								DeFrozenPercent = deFrozenPercent,
								DePalsyPercent = dePalsyPercent,
								DeSpeedDownPercent = deSpeedDownPercent,
								DeBlowPercent = deBlowPercent
							});
						}
					}
					if (list.Count < 1)
					{
						LogManager.WriteLog(1000, string.Format("ShenQi :: 韧性表不存在数据。", new object[0]), null, true);
					}
					else
					{
						list.Sort((ToughnessItem x, ToughnessItem y) => x.Toughness - y.Toughness);
						lock (this.ShenQiRunTimeData.Mutex)
						{
							this.ShenQiRunTimeData.ToughnessXmlList = list;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadGodXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenQiConsts.God);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					List<GodItem> list = new List<GodItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							string[] array = Global.GetDefAttributeStr(xelement2, "OpenCondition", "").Split(new char[]
							{
								'|'
							});
							List<int> list2 = new List<int>();
							foreach (string text2 in array)
							{
								list2.Add(Convert.ToInt32(text2));
							}
							string[] array3 = Global.GetDefAttributeStr(xelement2, "ActivationProperty", "").Split(new char[]
							{
								'|'
							});
							List<Dictionary<int, double>> list3 = new List<Dictionary<int, double>>();
							foreach (string text2 in array3)
							{
								Dictionary<int, double> dictionary = new Dictionary<int, double>();
								string[] array4 = text2.Split(new char[]
								{
									','
								});
								if (array4.Length == 2)
								{
									dictionary[(int)ConfigParser.GetPropIndexByPropName(array4[0])] = Global.SafeConvertToDouble(array4[1]);
								}
								list3.Add(dictionary);
							}
							list.Add(new GodItem
							{
								ID = id,
								OpenCondition = list2,
								ActivationProperty = list3
							});
						}
					}
					if (list.Count < 1)
					{
						LogManager.WriteLog(1000, string.Format("ShenQi :: 神像表不存在数据。", new object[0]), null, true);
					}
					else
					{
						lock (this.ShenQiRunTimeData.Mutex)
						{
							this.ShenQiRunTimeData.GodXmlList = list;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public static ShenQiData GetShenQiData(GameClient client)
		{
			ShenQiData result;
			if (!GlobalNew.IsGongNengOpened(client, 87, false))
			{
				result = new ShenQiData
				{
					UpResultType = -1
				};
			}
			else
			{
				ShenQiData shenQiData = client.ClientData.shenQiData;
				try
				{
					if (null == shenQiData)
					{
						shenQiData = new ShenQiData();
						List<int> list = Global.GetRoleParamsIntListFromDB(client, "36");
						if (list == null || list.Count < 5)
						{
							list = new List<int>();
							for (int i = 0; i < 5; i++)
							{
								list.Add(0);
							}
							list[0] = 1;
							Global.SaveRoleParamsIntListToDB(client, list, "36", true);
						}
						shenQiData.ShenQiID = list[0];
						shenQiData.LifeAdd = list[1];
						shenQiData.AttackAdd = list[2];
						shenQiData.DefenseAdd = list[3];
						shenQiData.ToughnessAdd = list[4];
						client.ClientData.shenQiData = shenQiData;
					}
					shenQiData.ShenLiJingHuaLeft = client.ClientData.ShenLiJingHuaPoints;
					shenQiData.UpResultType = -100;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("ShenQi :: 获取角色神器数据错误 ex:{0}", ex.Message), null, true);
				}
				result = shenQiData;
			}
			return result;
		}

		public ShenQiData LevelUpShenQiData(GameClient client, int useBaoJi)
		{
			ShenQiData result;
			if (!GlobalNew.IsGongNengOpened(client, 87, false))
			{
				result = new ShenQiData
				{
					UpResultType = -1
				};
			}
			else
			{
				ShenQiData shenQiData = client.ClientData.shenQiData;
				try
				{
					if (null == shenQiData)
					{
						LogManager.WriteLog(2, string.Format("ShenQi :: 注入失败，获取角色神器数据错误，角色id：{0}", client.ClientData.RoleID), null, true);
						return new ShenQiData
						{
							UpResultType = 0
						};
					}
					ArtifactItem artifactItem = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict.TryGetValue(shenQiData.ShenQiID, out artifactItem);
					}
					if (null == artifactItem)
					{
						LogManager.WriteLog(2, string.Format("ShenQi :: 注入失败，获取角色神器数据配置项错误，角色id：{0}, ShenQiID：{1}", client.ClientData.RoleID, shenQiData.ShenQiID), null, true);
						return new ShenQiData
						{
							UpResultType = 0
						};
					}
					if (client.ClientData.ShenLiJingHuaPoints < artifactItem.CostShenLiJingHua)
					{
						return new ShenQiData
						{
							UpResultType = -2
						};
					}
					int num = 0;
					if (artifactItem.CostGoldGoodsID > 0)
					{
						num = Global.GetTotalGoodsCountByID(client, artifactItem.CostGoldGoodsID);
					}
					if (client.ClientData.YinLiang < artifactItem.CostGoldCoin && (artifactItem.CostGoldGoodsNum <= 0 || num < artifactItem.CostGoldGoodsNum))
					{
						return new ShenQiData
						{
							UpResultType = -4
						};
					}
					if (useBaoJi > 0 && client.ClientData.UserMoney < artifactItem.CostDiamond && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, artifactItem.CostDiamond))
					{
						return new ShenQiData
						{
							UpResultType = -3
						};
					}
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, -artifactItem.CostShenLiJingHua, "神器注入_精华", true, true);
					if (artifactItem.CostGoldGoodsNum > 0 && num >= artifactItem.CostGoldGoodsNum)
					{
						bool flag2 = false;
						bool flag3 = false;
						if (Global.UseGoodsBindOrNot(client, artifactItem.CostGoldGoodsID, artifactItem.CostGoldGoodsNum, true, out flag2, out flag3) < 1)
						{
							return new ShenQiData
							{
								UpResultType = -4
							};
						}
					}
					else if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, artifactItem.CostGoldCoin, "神器注入_金币", false))
					{
						return new ShenQiData
						{
							UpResultType = -4
						};
					}
					if (useBaoJi > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, artifactItem.CostDiamond, "神器注入_钻石", true, true, false, DaiBiSySType.ShenQiXiTong))
						{
							return new ShenQiData
							{
								UpResultType = -4
							};
						}
					}
					int num2 = 0;
					int randomNumber = Global.GetRandomNumber(0, 101);
					int[] array = null;
					for (int i = 0; i < artifactItem.QiangHuaRate.Length; i++)
					{
						num2 += artifactItem.QiangHuaRate[i];
						if (randomNumber <= num2)
						{
							array = artifactItem.QiangHuaArray[i];
							shenQiData.BurstType = i;
							if (useBaoJi > 0 && 0 == i)
							{
								array = artifactItem.QiangHuaArray[1];
								shenQiData.BurstType = 1;
							}
							break;
						}
					}
					shenQiData.LifeAdd += array[0];
					shenQiData.LifeAdd = ((shenQiData.LifeAdd > artifactItem.PropArray[0]) ? artifactItem.PropArray[0] : shenQiData.LifeAdd);
					shenQiData.AttackAdd += array[1];
					shenQiData.AttackAdd = ((shenQiData.AttackAdd > artifactItem.PropArray[1]) ? artifactItem.PropArray[1] : shenQiData.AttackAdd);
					shenQiData.DefenseAdd += array[2];
					shenQiData.DefenseAdd = ((shenQiData.DefenseAdd > artifactItem.PropArray[2]) ? artifactItem.PropArray[2] : shenQiData.DefenseAdd);
					shenQiData.ToughnessAdd += array[3];
					shenQiData.ToughnessAdd = ((shenQiData.ToughnessAdd > artifactItem.PropArray[3]) ? artifactItem.PropArray[3] : shenQiData.ToughnessAdd);
					if (shenQiData.LifeAdd < artifactItem.PropArray[0] || shenQiData.DefenseAdd < artifactItem.PropArray[2] || shenQiData.AttackAdd < artifactItem.PropArray[1] || shenQiData.ToughnessAdd < artifactItem.PropArray[3])
					{
						shenQiData.UpResultType = 1;
					}
					else if (this.ShenQiRunTimeData.ArtifactXmlDict.ContainsKey(shenQiData.ShenQiID + 1))
					{
						shenQiData.UpResultType = 2;
						shenQiData.ShenQiID++;
						shenQiData.LifeAdd = 0;
						shenQiData.AttackAdd = 0;
						shenQiData.DefenseAdd = 0;
						shenQiData.ToughnessAdd = 0;
					}
					else
					{
						shenQiData.UpResultType = 3;
					}
					shenQiData.ShenLiJingHuaLeft = client.ClientData.ShenLiJingHuaPoints;
					List<int> list = new List<int>();
					list.AddRange(new int[]
					{
						shenQiData.ShenQiID,
						shenQiData.LifeAdd,
						shenQiData.AttackAdd,
						shenQiData.DefenseAdd,
						shenQiData.ToughnessAdd
					});
					Global.SaveRoleParamsIntListToDB(client, list, "36", true);
					client.ClientData.shenQiData = shenQiData;
					this.UpdateRoleShenQiProps(client);
					this.UpdateRoleTouhgnessProps(client);
					if (shenQiData.UpResultType == 2 || shenQiData.UpResultType == 3)
					{
						this.UpdateRoleGodProps(client);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神像", "神器升阶", client.ClientData.RoleName, "系统", "增加", 1, client.ClientData.ZoneID, client.strUserID, shenQiData.ShenQiID, client.ServerId, null);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "韧性", "神器注入_韧性", client.ClientData.RoleName, "系统", "增加", array[3], client.ClientData.ZoneID, client.strUserID, Convert.ToInt32(RoleAlgorithm.GetExtProp(client, 101)), client.ServerId, null);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("ShenQi :: 升级角色神器数据错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
					return shenQiData;
				}
				result = shenQiData;
			}
			return result;
		}

		public void UpdateRoleShenQiProps(GameClient client)
		{
			try
			{
				ShenQiData shenQiData = client.ClientData.shenQiData;
				if (null != shenQiData)
				{
					int num = shenQiData.LifeAdd;
					int num2 = shenQiData.AttackAdd;
					int num3 = shenQiData.DefenseAdd;
					int num4 = shenQiData.ToughnessAdd;
					Dictionary<int, ArtifactItem> dictionary = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						dictionary = this.ShenQiRunTimeData.ArtifactXmlDict;
					}
					if (null != dictionary)
					{
						foreach (ArtifactItem artifactItem in dictionary.Values)
						{
							if (artifactItem.ID < shenQiData.ShenQiID)
							{
								num += artifactItem.PropArray[0];
								num2 += artifactItem.PropArray[1];
								num3 += artifactItem.PropArray[2];
								num4 += artifactItem.PropArray[3];
							}
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							13,
							num
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							45,
							num2
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							46,
							num3
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							101,
							num4
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenQi :: 更新角色神器加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		public void UpdateRoleTouhgnessProps(GameClient client)
		{
			try
			{
				double extProp = RoleAlgorithm.GetExtProp(client, 101);
				if (extProp >= 1.0)
				{
					List<ToughnessItem> list = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						list = this.ShenQiRunTimeData.ToughnessXmlList;
					}
					if (list != null && list.Count >= 1)
					{
						int i;
						for (i = list.Count - 1; i >= 0; i--)
						{
							if (extProp >= (double)list[i].Toughness)
							{
								break;
							}
						}
						ToughnessItem toughnessItem = (i >= 0) ? list[i] : null;
						if (null != toughnessItem)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								51,
								toughnessItem.DeLucky
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								52,
								toughnessItem.DeFatalAttack
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								53,
								toughnessItem.DeDoubleAttack
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								64,
								toughnessItem.DeSavagePercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								65,
								toughnessItem.DeColdPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								66,
								toughnessItem.DeRuthlessPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								97,
								toughnessItem.DeFrozenPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								98,
								toughnessItem.DePalsyPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								99,
								toughnessItem.DeSpeedDownPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								100,
								toughnessItem.DeBlowPercent
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenQi :: 更新角色韧性加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		public void UpdateRoleGodProps(GameClient client)
		{
			try
			{
				ShenQiData shenQiData = client.ClientData.shenQiData;
				int num = shenQiData.ShenQiID;
				if (num >= 1)
				{
					ArtifactItem artifactItem = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict.TryGetValue(num, out artifactItem);
					}
					if (shenQiData.LifeAdd < artifactItem.PropArray[0] || shenQiData.DefenseAdd < artifactItem.PropArray[2] || shenQiData.AttackAdd < artifactItem.PropArray[1] || shenQiData.ToughnessAdd < artifactItem.PropArray[3])
					{
						num--;
					}
					List<GodItem> list = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						list = this.ShenQiRunTimeData.GodXmlList;
					}
					if (list != null && list.Count >= 1)
					{
						Dictionary<int, double> dictionary = new Dictionary<int, double>();
						foreach (GodItem godItem in list)
						{
							bool flag3 = true;
							foreach (int num2 in godItem.OpenCondition)
							{
								if (num2 > num)
								{
									flag3 = false;
									break;
								}
							}
							if (flag3)
							{
								foreach (Dictionary<int, double> dictionary2 in godItem.ActivationProperty)
								{
									foreach (KeyValuePair<int, double> keyValuePair in dictionary2)
									{
										dictionary[keyValuePair.Key] = (dictionary.ContainsKey(keyValuePair.Key) ? (dictionary[keyValuePair.Key] + keyValuePair.Value) : keyValuePair.Value);
									}
								}
							}
						}
						foreach (KeyValuePair<int, double> keyValuePair2 in dictionary)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								28,
								keyValuePair2.Key,
								keyValuePair2.Value
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenQi :: 更新角色神像加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		public static void InitRoleShenQiData(GameClient client)
		{
			client.ClientData.ShenLiJingHuaPoints = Global.GetRoleParamsInt32FromDB(client, "10157");
			if (GlobalNew.IsGongNengOpened(client, 87, false))
			{
				client.ClientData.shenQiData = ShenQiManager.GetShenQiData(client);
				ShenQiManager.getInstance().UpdateRoleShenQiProps(client);
				ShenQiManager.getInstance().UpdateRoleTouhgnessProps(client);
				ShenQiManager.getInstance().UpdateRoleGodProps(client);
			}
		}

		public ShenQiRunData ShenQiRunTimeData = new ShenQiRunData();

		private static ShenQiManager instance = new ShenQiManager();
	}
}
