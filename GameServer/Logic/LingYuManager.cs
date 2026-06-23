using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	internal class LingYuManager
	{
		public static void LoadConfig()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuTypeFile));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuTypeFile));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuTypeFile), null, true);
			}
			else
			{
				try
				{
					LingYuManager.LingYuTypeDict.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							LingYuType lingYuType = new LingYuType();
							lingYuType.Type = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TypeID", "0"));
							lingYuType.Name = Global.GetDefAttributeStr(xelement2, "Name", "no-name");
							lingYuType.LifeScale = Global.GetSafeAttributeDouble(xelement2, "LifeScale");
							lingYuType.AttackScale = Global.GetSafeAttributeDouble(xelement2, "AttackScale");
							lingYuType.DefenseScale = Global.GetSafeAttributeDouble(xelement2, "DefenseScale");
							lingYuType.MAttackScale = Global.GetSafeAttributeDouble(xelement2, "MAttackScale");
							lingYuType.MDefenseScale = Global.GetSafeAttributeDouble(xelement2, "MDefenseScale");
							lingYuType.HitScale = Global.GetSafeAttributeDouble(xelement2, "HitScale");
							LingYuManager.LingYuTypeDict[lingYuType.Type] = lingYuType;
						}
					}
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时异常{1}", LingYuManager.LingYuTypeFile, arg), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuLevelUpFile));
			xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuLevelUpFile));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuLevelUpFile), null, true);
			}
			else
			{
				try
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TypeID", "0"));
							LingYuType lingYuType = null;
							if (!LingYuManager.LingYuTypeDict.TryGetValue(num, out lingYuType))
							{
								LogManager.WriteLog(2, string.Format("加载翎羽升级文件{0}时，未找到类型为{1}的翎羽配置", LingYuManager.LingYuLevelUpFile, num), null, true);
							}
							else
							{
								IEnumerable<XElement> enumerable2 = xelement2.Elements();
								foreach (XElement xml in enumerable2)
								{
									LingYuLevel lingYuLevel = new LingYuLevel();
									lingYuLevel.Level = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Level", "0"));
									lingYuLevel.MinAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinAttackV", "0"));
									lingYuLevel.MaxAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxAttackV", "0"));
									lingYuLevel.MinMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinMAttackV", "0"));
									lingYuLevel.MaxMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxMAttackV", "0"));
									lingYuLevel.MinDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinDefenseV", "0"));
									lingYuLevel.MaxDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxDefenseV", "0"));
									lingYuLevel.MinMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinMDefenseV", "0"));
									lingYuLevel.MaxMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxMDefenseV", "0"));
									lingYuLevel.HitV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "HitV", "0"));
									lingYuLevel.LifeV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "LifeV", "0"));
									lingYuLevel.JinBiCost = Convert.ToInt32(Global.GetDefAttributeStr(xml, "JinBiCost", "0"));
									string defAttributeStr = Global.GetDefAttributeStr(xml, "GoodsCost", "0");
									string[] array = defAttributeStr.Split(new char[]
									{
										','
									});
									if (array.Length != 2)
									{
										LogManager.WriteLog(2, string.Format("翎羽Type{0},级别{1}, 消耗物品配置错误", num, lingYuLevel.Level), null, true);
									}
									else
									{
										lingYuLevel.GoodsCost = Convert.ToInt32(array[0]);
										lingYuLevel.GoodsCostCnt = Convert.ToInt32(array[1]);
										lingYuType.LevelDict[lingYuLevel.Level] = lingYuLevel;
										LingYuManager.LingYuLevelLimit = Global.GMax(LingYuManager.LingYuLevelLimit, lingYuLevel.Level);
									}
								}
							}
						}
					}
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时异常{1}", LingYuManager.LingYuLevelUpFile, arg), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuSuitUpFile));
			xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuSuitUpFile));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuSuitUpFile), null, true);
			}
			else
			{
				try
				{
					lock (LingYuManager.LingYuTypeDict)
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (null != xelement2)
							{
								int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TypeID", "0"));
								LingYuType lingYuType = null;
								if (!LingYuManager.LingYuTypeDict.TryGetValue(num, out lingYuType))
								{
									LogManager.WriteLog(2, string.Format("加载翎羽进阶文件{0}时，未找到类型为{1}的翎羽配置", LingYuManager.LingYuSuitUpFile, num), null, true);
								}
								else
								{
									IEnumerable<XElement> enumerable2 = xelement2.Elements();
									foreach (XElement xml in enumerable2)
									{
										LingYuSuit lingYuSuit = new LingYuSuit();
										lingYuSuit.Suit = Convert.ToInt32(Global.GetDefAttributeStr(xml, "SuitID", "0"));
										lingYuSuit.MinAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinAttackV", "0"));
										lingYuSuit.MaxAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxAttackV", "0"));
										lingYuSuit.MinMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinMAttackV", "0"));
										lingYuSuit.MaxMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxMAttackV", "0"));
										lingYuSuit.MinDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinDefenseV", "0"));
										lingYuSuit.MaxDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxDefenseV", "0"));
										lingYuSuit.MinMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinMDefenseV", "0"));
										lingYuSuit.MaxMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxMDefenseV", "0"));
										lingYuSuit.HitV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "HitV", "0"));
										lingYuSuit.LifeV = Convert.ToInt32(Global.GetDefAttributeStr(xml, "LifeV", "0"));
										lingYuSuit.JinBiCost = Convert.ToInt32(Global.GetDefAttributeStr(xml, "JinBiCost", "0"));
										string defAttributeStr = Global.GetDefAttributeStr(xml, "GoodsCost", "0");
										lingYuSuit.GoodsCost = ConfigHelper.ParserIntArrayList(defAttributeStr, true, '|', ',');
										lingYuType.SuitDict[lingYuSuit.Suit] = lingYuSuit;
										LingYuManager.LingYuSuitLimit = Global.GMax(LingYuManager.LingYuSuitLimit, lingYuSuit.Suit);
									}
								}
							}
						}
						LingYuManager.LingYuSuitLimit = Global.GMin(LingYuManager.LingYuSuitLimit, (int)GameManager.systemParamsList.GetParamValueIntByName("LingYuMax", 0));
					}
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时异常{1}", LingYuManager.LingYuSuitUpFile, arg), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuCollectFile));
			xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuCollectFile));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuCollectFile), null, true);
			}
			else
			{
				try
				{
					LingYuManager.LingYuCollectList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							LingYuCollect lingYuCollect = new LingYuCollect();
							lingYuCollect.Num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0"));
							lingYuCollect.NeedSuit = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedSuit", "0"));
							lingYuCollect.Luck = Global.GetSafeAttributeDouble(xelement2, "Luck");
							lingYuCollect.DeLuck = Global.GetSafeAttributeDouble(xelement2, "DeLuck");
							LingYuManager.LingYuCollectList.Add(lingYuCollect);
						}
					}
					LingYuManager.LingYuCollectList.Sort(delegate(LingYuCollect left, LingYuCollect right)
					{
						int result;
						if (left.NeedSuit > right.NeedSuit)
						{
							result = 1;
						}
						else if (left.NeedSuit == right.NeedSuit)
						{
							if (left.Num > right.Num)
							{
								result = 1;
							}
							else if (left.Num == right.Num)
							{
								result = 0;
							}
							else
							{
								result = -1;
							}
						}
						else
						{
							result = -1;
						}
						return result;
					});
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时异常{1}", LingYuManager.LingYuCollectFile, arg), null, true);
				}
			}
		}

		public static string Error2Str(LingYuError lyError)
		{
			string result;
			if (lyError == LingYuError.Success)
			{
				result = GLang.GetLang(407, new object[0]);
			}
			else if (lyError == LingYuError.NotOpen)
			{
				result = GLang.GetLang(408, new object[0]);
			}
			else if (lyError == LingYuError.LevelFull)
			{
				result = GLang.GetLang(409, new object[0]);
			}
			else if (lyError == LingYuError.NeedLevelUp)
			{
				result = GLang.GetLang(410, new object[0]);
			}
			else if (lyError == LingYuError.NeedSuitUp)
			{
				result = GLang.GetLang(411, new object[0]);
			}
			else if (lyError == LingYuError.SuitFull)
			{
				result = GLang.GetLang(412, new object[0]);
			}
			else if (lyError == LingYuError.LevelUpMaterialNotEnough)
			{
				result = GLang.GetLang(413, new object[0]);
			}
			else if (lyError == LingYuError.LevelUpJinBiNotEnough)
			{
				result = GLang.GetLang(414, new object[0]);
			}
			else if (lyError == LingYuError.SuitUpMaterialNotEnough)
			{
				result = GLang.GetLang(415, new object[0]);
			}
			else if (lyError == LingYuError.SuitUpJinBiNotEnough)
			{
				result = GLang.GetLang(416, new object[0]);
			}
			else if (lyError == LingYuError.ErrorConfig)
			{
				result = GLang.GetLang(417, new object[0]);
			}
			else if (lyError == LingYuError.ErrorParams)
			{
				result = GLang.GetLang(418, new object[0]);
			}
			else if (lyError == LingYuError.ZuanShiNotEnough)
			{
				result = GLang.GetLang(419, new object[0]);
			}
			else if (lyError == LingYuError.DBSERVERERROR)
			{
				result = GLang.GetLang(420, new object[0]);
			}
			else
			{
				result = "unknown";
			}
			return result;
		}

		public static void UpdateLingYuProps(GameClient client)
		{
			if (null != client.ClientData.MyWingData)
			{
				if (client.ClientData.MyWingData.WingID > 0)
				{
					double num = 0.0;
					double num2 = 0.0;
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					double num6 = 0.0;
					double num7 = 0.0;
					double num8 = 0.0;
					double num9 = 0.0;
					double num10 = 0.0;
					int[] array = new int[LingYuManager.LingYuSuitLimit + 1];
					if (client.ClientData.MyWingData.Using == 1)
					{
						lock (client.ClientData.LingYuDict)
						{
							foreach (KeyValuePair<int, LingYuData> keyValuePair in client.ClientData.LingYuDict)
							{
								int type = keyValuePair.Value.Type;
								int level = keyValuePair.Value.Level;
								int suit = keyValuePair.Value.Suit;
								for (int i = 0; i <= suit; i++)
								{
									array[i]++;
								}
								LingYuType lingYuType = null;
								if (LingYuManager.LingYuTypeDict.TryGetValue(type, out lingYuType))
								{
									LingYuLevel lingYuLevel = null;
									lingYuType.LevelDict.TryGetValue(level, out lingYuLevel);
									LingYuSuit lingYuSuit = null;
									lingYuType.SuitDict.TryGetValue(suit, out lingYuSuit);
									if (lingYuLevel != null)
									{
										num += (double)lingYuLevel.MinAttackV;
										num2 += (double)lingYuLevel.MaxAttackV;
										num3 += (double)lingYuLevel.MinMAttackV;
										num4 += (double)lingYuLevel.MaxMAttackV;
										num5 += (double)lingYuLevel.MinDefenseV;
										num6 += (double)lingYuLevel.MaxDefenseV;
										num7 += (double)lingYuLevel.MinMDefenseV;
										num8 += (double)lingYuLevel.MaxMDefenseV;
										num9 += (double)lingYuLevel.HitV;
										num10 += (double)lingYuLevel.LifeV;
									}
									if (lingYuSuit != null)
									{
										num += (double)lingYuSuit.MinAttackV;
										num2 += (double)lingYuSuit.MaxAttackV;
										num3 += (double)lingYuSuit.MinMAttackV;
										num4 += (double)lingYuSuit.MaxMAttackV;
										num5 += (double)lingYuSuit.MinDefenseV;
										num6 += (double)lingYuSuit.MaxDefenseV;
										num7 += (double)lingYuSuit.MinMDefenseV;
										num8 += (double)lingYuSuit.MaxMDefenseV;
										num9 += (double)lingYuSuit.HitV;
										num10 += (double)lingYuSuit.LifeV;
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						7,
						num
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						8,
						num2
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						9,
						num3
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						10,
						num4
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						3,
						num5
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						4,
						num6
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						5,
						num7
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						6,
						num8
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						18,
						num9
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						13,
						num10
					});
					double num11 = 0.0;
					double num12 = 0.0;
					if (client.ClientData.MyWingData.Using == 1)
					{
						for (int i = LingYuManager.LingYuCollectList.Count<LingYuCollect>() - 1; i >= 0; i--)
						{
							LingYuCollect lingYuCollect = LingYuManager.LingYuCollectList[i];
							if (array[lingYuCollect.NeedSuit] >= lingYuCollect.Num)
							{
								num11 = lingYuCollect.Luck;
								num12 = lingYuCollect.DeLuck;
								break;
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						17,
						num11
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						51,
						num12
					});
				}
			}
		}

		public static List<LingYuData> GetLingYuList(GameClient client)
		{
			List<LingYuData> list = new List<LingYuData>();
			Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
			foreach (int num in keys)
			{
				LingYuData lingYuData = null;
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.TryGetValue(num, out lingYuData))
					{
						lingYuData = new LingYuData();
						lingYuData.Type = num;
						lingYuData.Level = 1;
						lingYuData.Suit = 0;
					}
				}
				list.Add(lingYuData);
			}
			return list;
		}

		public static TCPProcessCmdResults ProcessGetLingYuList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				List<LingYuData> lingYuList = LingYuManager.GetLingYuList(client);
				byte[] buffer = DataHelper.ObjectToBytes<List<LingYuData>>(lingYuList);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetLingYuList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static LingYuError AdvanceLingYuLevel(GameClient client, int roleID, int type, int useZuanshiIfNoMaterial)
		{
			LingYuError result;
			if (!GlobalNew.IsGongNengOpened(client, 50, false))
			{
				result = LingYuError.NotOpen;
			}
			else
			{
				LingYuType lingYuType = null;
				if (!LingYuManager.LingYuTypeDict.TryGetValue(type, out lingYuType))
				{
					result = LingYuError.ErrorParams;
				}
				else
				{
					LingYuData lingYuData = null;
					lock (client.ClientData.LingYuDict)
					{
						if (!client.ClientData.LingYuDict.TryGetValue(type, out lingYuData))
						{
							lingYuData = new LingYuData();
							lingYuData.Type = type;
							lingYuData.Level = 1;
							lingYuData.Suit = 0;
						}
					}
					if (lingYuData.Level == LingYuManager.LingYuLevelLimit)
					{
						result = LingYuError.LevelFull;
					}
					else if (lingYuData.Level > 0 && lingYuData.Level % 10 == 0 && lingYuData.Level / 10 != lingYuData.Suit)
					{
						result = LingYuError.NeedSuitUp;
					}
					else
					{
						LingYuLevel lingYuLevel = null;
						if (!lingYuType.LevelDict.TryGetValue(lingYuData.Level + 1, out lingYuLevel))
						{
							result = LingYuError.ErrorConfig;
						}
						else if (Global.GetTotalBindTongQianAndTongQianVal(client) < lingYuLevel.JinBiCost)
						{
							result = LingYuError.LevelUpJinBiNotEnough;
						}
						else
						{
							int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, lingYuLevel.GoodsCost);
							if (totalGoodsCountByID < lingYuLevel.GoodsCostCnt && useZuanshiIfNoMaterial == 0)
							{
								result = LingYuError.LevelUpMaterialNotEnough;
							}
							else
							{
								string text = "";
								int level = lingYuData.Level;
								int yinLiang = client.ClientData.YinLiang;
								int money = client.ClientData.Money1;
								int userMoney = client.ClientData.UserMoney;
								int gold = client.ClientData.Gold;
								int num = lingYuLevel.GoodsCostCnt;
								int num2 = 0;
								if (totalGoodsCountByID < lingYuLevel.GoodsCostCnt)
								{
									num = 0;
									int num3 = 0;
									if (!Data.LingYuMaterialZuanshiDict.TryGetValue(lingYuLevel.GoodsCost, out num3))
									{
										return LingYuError.ErrorConfig;
									}
									num2 = lingYuLevel.GoodsCostCnt * num3;
									if (client.ClientData.UserMoney < num2 && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, num2))
									{
										return LingYuError.ZuanShiNotEnough;
									}
								}
								if (!Global.SubBindTongQianAndTongQian(client, lingYuLevel.JinBiCost, "翎羽升级消耗"))
								{
									result = LingYuError.DBSERVERERROR;
								}
								else
								{
									text = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
									{
										-lingYuLevel.JinBiCost,
										yinLiang,
										client.ClientData.YinLiang,
										money,
										client.ClientData.Money1
									});
									if (num > 0)
									{
										bool flag2 = false;
										bool flag3 = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, lingYuLevel.GoodsCost, num, false, out flag2, out flag3, false))
										{
											return LingYuError.DBSERVERERROR;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = lingYuLevel.GoodsCost,
											GCount = num
										};
										text += EventLogManager.AddGoodsDataPropString(goodsData);
									}
									if (num2 > 0)
									{
										if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "翎羽升级", true, true, false, DaiBiSySType.LingYuShengXing))
										{
											return LingYuError.DBSERVERERROR;
										}
										text += EventLogManager.AddResPropString(ResLogType.FristBindZuanShi, new object[]
										{
											-num2,
											gold,
											client.ClientData.Gold,
											userMoney,
											client.ClientData.UserMoney
										});
									}
									int num4 = LingYuManager.UpdateLingYu2DB(roleID, type, lingYuData.Level + 1, lingYuData.Suit, client.ServerId);
									if (num4 < 0)
									{
										result = LingYuError.DBSERVERERROR;
									}
									else
									{
										lingYuData.Level++;
										lock (client.ClientData.LingYuDict)
										{
											client.ClientData.LingYuDict[type] = lingYuData;
										}
										LingYuManager.UpdateLingYuProps(client);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
										EventLogManager.AddLingYuLevelEvent(client, useZuanshiIfNoMaterial, level, lingYuData.Suit, lingYuData.Level, text);
										if (client._IconStateMgr.CheckReborn(client))
										{
											client._IconStateMgr.SendIconStateToClient(client);
										}
										result = LingYuError.Success;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessAdvanceLingYuLevel(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Convert.ToInt32(array[1]);
				int useZuanshiIfNoMaterial = Convert.ToInt32(array[2]);
				LingYuError lingYuError = LingYuManager.AdvanceLingYuLevel(gameClient, num, num2, useZuanshiIfNoMaterial);
				LingYuData lingYuData = null;
				lock (gameClient.ClientData.LingYuDict)
				{
					if (!gameClient.ClientData.LingYuDict.TryGetValue(num2, out lingYuData))
					{
						lingYuData = new LingYuData();
						lingYuData.Type = num2;
						lingYuData.Level = 1;
						lingYuData.Suit = 0;
					}
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					(int)lingYuError,
					lingYuData.Type,
					lingYuData.Level
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessAdvanceLingYuLevel", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static LingYuError AdvanceLingYuSuit(GameClient client, int roleID, int type, int useZuanshiIfNoMaterial)
		{
			LingYuError result;
			if (!GlobalNew.IsGongNengOpened(client, 50, false))
			{
				result = LingYuError.NotOpen;
			}
			else
			{
				LingYuType lingYuType = null;
				if (!LingYuManager.LingYuTypeDict.TryGetValue(type, out lingYuType))
				{
					result = LingYuError.ErrorParams;
				}
				else
				{
					LingYuData lingYuData = null;
					lock (client.ClientData.LingYuDict)
					{
						if (!client.ClientData.LingYuDict.TryGetValue(type, out lingYuData))
						{
							lingYuData = new LingYuData();
							lingYuData.Type = type;
							lingYuData.Level = 1;
							lingYuData.Suit = 0;
						}
					}
					if (lingYuData.Suit == LingYuManager.LingYuSuitLimit)
					{
						result = LingYuError.SuitFull;
					}
					else if (lingYuData.Level == 0 || lingYuData.Level / 10 == lingYuData.Suit)
					{
						result = LingYuError.NeedLevelUp;
					}
					else
					{
						LingYuSuit lingYuSuit = null;
						if (!lingYuType.SuitDict.TryGetValue(lingYuData.Suit + 1, out lingYuSuit))
						{
							result = LingYuError.ErrorConfig;
						}
						else if (Global.GetTotalBindTongQianAndTongQianVal(client) < lingYuSuit.JinBiCost)
						{
							result = LingYuError.SuitUpJinBiNotEnough;
						}
						else
						{
							bool flag2 = false;
							int num = 0;
							for (int i = 0; i < lingYuSuit.GoodsCost.Count; i++)
							{
								int num2 = lingYuSuit.GoodsCost[i][0];
								int num3 = lingYuSuit.GoodsCost[i][1];
								int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, num2);
								if (totalGoodsCountByID < num3)
								{
									if (useZuanshiIfNoMaterial == 0)
									{
										return LingYuError.SuitUpMaterialNotEnough;
									}
									flag2 = true;
								}
								int num4;
								if (!Data.LingYuMaterialZuanshiDict.TryGetValue(num2, out num4))
								{
									return LingYuError.ErrorConfig;
								}
								num += num3 * num4;
							}
							string text = "";
							int level = lingYuData.Level;
							int suit = lingYuData.Suit;
							int yinLiang = client.ClientData.YinLiang;
							int money = client.ClientData.Money1;
							int userMoney = client.ClientData.UserMoney;
							int gold = client.ClientData.Gold;
							if (flag2)
							{
								if (client.ClientData.UserMoney < num && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, num))
								{
									return LingYuError.ZuanShiNotEnough;
								}
							}
							if (!Global.SubBindTongQianAndTongQian(client, lingYuSuit.JinBiCost, "翎羽升阶消耗"))
							{
								result = LingYuError.DBSERVERERROR;
							}
							else
							{
								text = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
								{
									-lingYuSuit.JinBiCost,
									yinLiang,
									client.ClientData.YinLiang,
									money,
									client.ClientData.Money1
								});
								if (!flag2)
								{
									bool flag3 = false;
									bool flag4 = false;
									for (int i = 0; i < lingYuSuit.GoodsCost.Count; i++)
									{
										int num2 = lingYuSuit.GoodsCost[i][0];
										int num3 = lingYuSuit.GoodsCost[i][1];
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, num3, false, out flag3, out flag4, false))
										{
											GameManager.logDBCmdMgr.AddDBLogInfo(0, "升级失败", "翎羽升级", Global.GetMapName(client.ClientData.MapCode), "系统", "记录", 0, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
											return LingYuError.DBSERVERERROR;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = num2,
											GCount = num3
										};
										text += EventLogManager.AddGoodsDataPropString(goodsData);
									}
								}
								else
								{
									if (num <= 0)
									{
										return LingYuError.ErrorConfig;
									}
									if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "翎羽升级", true, true, false, DaiBiSySType.LingYuShengJie))
									{
										return LingYuError.DBSERVERERROR;
									}
									text += EventLogManager.AddResPropString(ResLogType.FristBindZuanShi, new object[]
									{
										-num,
										gold,
										client.ClientData.Gold,
										userMoney,
										client.ClientData.UserMoney
									});
								}
								int num5 = LingYuManager.UpdateLingYu2DB(roleID, type, lingYuData.Level, lingYuData.Suit + 1, client.ServerId);
								if (num5 < 0)
								{
									result = LingYuError.DBSERVERERROR;
								}
								else
								{
									lingYuData.Suit++;
									lock (client.ClientData.LingYuDict)
									{
										client.ClientData.LingYuDict[type] = lingYuData;
									}
									if (LingYuManager.SuitOfNotifyList.Contains(lingYuData.Suit))
									{
										string msgText = StringUtil.substitute(GLang.GetLang(421, new object[0]), new object[]
										{
											Global.FormatRoleName(client, client.ClientData.RoleName),
											lingYuType.Name,
											lingYuData.Suit
										});
										Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
									}
									LingYuManager.UpdateLingYuProps(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
									EventLogManager.AddLingYuSuitEvent(client, useZuanshiIfNoMaterial, suit, lingYuData.Suit, level, lingYuData.Level, text);
									if (client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.SendIconStateToClient(client);
									}
									result = LingYuError.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessAdvanceLingYuSuit(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Convert.ToInt32(array[1]);
				int useZuanshiIfNoMaterial = Convert.ToInt32(array[2]);
				LingYuError lingYuError = LingYuManager.AdvanceLingYuSuit(gameClient, num, num2, useZuanshiIfNoMaterial);
				LingYuData lingYuData = null;
				lock (gameClient.ClientData.LingYuDict)
				{
					if (!gameClient.ClientData.LingYuDict.TryGetValue(num2, out lingYuData))
					{
						lingYuData = new LingYuData();
						lingYuData.Type = num2;
						lingYuData.Level = 1;
						lingYuData.Suit = 0;
					}
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					(int)lingYuError,
					lingYuData.Type,
					lingYuData.Suit
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessAdvanceLingYuSuit", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static int UpdateLingYu2DB(int roleID, int type, int level, int suit, int serverId)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleID,
				type,
				level,
				suit
			});
			string[] array = Global.ExecuteDBCmd(10176, strcmd, serverId);
			int result;
			if (array == null || array.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(array[1]);
			}
			return result;
		}

		public static void InitAsOpened(GameClient client)
		{
			Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
			foreach (int num in keys)
			{
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.ContainsKey(num))
					{
						LingYuData value = new LingYuData
						{
							Type = num,
							Level = 1,
							Suit = 0
						};
						LingYuManager.UpdateLingYu2DB(client.ClientData.RoleID, num, 1, 0, client.ServerId);
						client.ClientData.LingYuDict[num] = value;
					}
				}
			}
			LingYuManager.UpdateLingYuProps(client);
		}

		public static int GetTotalLevel(GameClient client)
		{
			int num = 0;
			lock (client.ClientData.LingYuDict)
			{
				foreach (LingYuData lingYuData in client.ClientData.LingYuDict.Values)
				{
					num += lingYuData.Suit * 10 + lingYuData.Level;
				}
			}
			return num;
		}

		public static bool IfLingYuPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 50, false))
			{
				result = false;
			}
			else
			{
				Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
				if (keys.Count != client.ClientData.LingYuDict.Count)
				{
					result = false;
				}
				else
				{
					foreach (LingYuData lingYuData in client.ClientData.LingYuDict.Values)
					{
						if (lingYuData.Suit != LingYuManager.LingYuSuitLimit || lingYuData.Level != LingYuManager.LingYuLevelLimit)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static void SetLingYuMax_GM(GameClient client)
		{
			LingYuManager.InitAsOpened(client);
			lock (client.ClientData.LingYuDict)
			{
				foreach (LingYuData lingYuData in client.ClientData.LingYuDict.Values)
				{
					LingYuType lingYuType = null;
					if (LingYuManager.LingYuTypeDict.TryGetValue(lingYuData.Type, out lingYuType))
					{
						lingYuData.Suit = LingYuManager.LingYuSuitLimit;
						lingYuData.Level = LingYuManager.LingYuLevelLimit;
						LingYuManager.UpdateLingYu2DB(client.ClientData.RoleID, lingYuData.Type, lingYuData.Level, lingYuData.Suit, client.ServerId);
						string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							0,
							lingYuData.Type,
							lingYuData.Suit
						});
						client.sendCmd(802, cmdData, false);
						cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							0,
							lingYuData.Type,
							lingYuData.Level
						});
						client.sendCmd(801, cmdData, false);
					}
				}
			}
			LingYuManager.UpdateLingYuProps(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			if (client._IconStateMgr.CheckReborn(client))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		private const int DEFAULT_LINGYU_LEVEL = 1;

		private static string LingYuTypeFile = "Config/LingyuType.xml";

		private static string LingYuLevelUpFile = "Config/LingYuLevelUp.xml";

		private static string LingYuSuitUpFile = "Config/LingYuSuitUp.xml";

		private static string LingYuCollectFile = "Config/LingYucollect.xml";

		private static int LingYuLevelLimit = 0;

		private static int LingYuSuitLimit = 0;

		private static Dictionary<int, LingYuType> LingYuTypeDict = new Dictionary<int, LingYuType>();

		private static List<LingYuCollect> LingYuCollectList = new List<LingYuCollect>();

		private static int[] SuitOfNotifyList = new int[]
		{
			3,
			6,
			9
		};
	}
}
