using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.FuMo
{
	internal class FuMoManager : ICmdProcessorEx, ICmdProcessor
	{
		public static FuMoManager getInstance()
		{
			return FuMoManager.instance;
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

		public void Initialize()
		{
			string text = Global.GameResPath("Config/EquipEnchantmentExtra.xml");
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			try
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					FuMoExtraTemplate fuMoExtraTemplate = new FuMoExtraTemplate();
					fuMoExtraTemplate.Condition = new List<int>();
					fuMoExtraTemplate.Parameter = new Dictionary<double, double>();
					fuMoExtraTemplate.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					if (FuMoManager.MaxExtrasID < fuMoExtraTemplate.ID)
					{
						FuMoManager.MaxExtrasID = fuMoExtraTemplate.ID;
					}
					fuMoExtraTemplate.Name = Global.GetSafeAttributeStr(xml, "Name");
					string[] array = Global.GetSafeAttributeStr(xml, "Condition").Split(new char[]
					{
						','
					});
					foreach (string text2 in array)
					{
						fuMoExtraTemplate.Condition.Add(Convert.ToInt32(text2));
					}
					fuMoExtraTemplate.Type = Global.GetSafeAttributeStr(xml, "Type");
					string[] array3 = Global.GetSafeAttributeStr(xml, "Parameter").Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array3)
					{
						string[] array4 = text2.Split(new char[]
						{
							','
						});
						fuMoExtraTemplate.Parameter.Add(Convert.ToDouble(array4[0]), Convert.ToDouble(array4[1]));
					}
					if (!FuMoManager.IDExtrasTypeMap.ContainsKey(fuMoExtraTemplate.ID))
					{
						FuMoManager.IDExtrasTypeMap.Add(fuMoExtraTemplate.ID, (int)ConfigParser.GetPropIndexByPropName(fuMoExtraTemplate.Type));
					}
					FuMoManager.FuMoExtras.Add(fuMoExtraTemplate.ID, fuMoExtraTemplate);
					FuMoManager.FuMoExtras[fuMoExtraTemplate.ID].Parameter = (from p in FuMoManager.FuMoExtras[fuMoExtraTemplate.ID].Parameter
					orderby p.Value
					select p).ToDictionary((KeyValuePair<double, double> p) => p.Key, (KeyValuePair<double, double> p) => p.Value);
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。", text));
			}
			try
			{
				text = Global.GameResPath("Config/EquipEnchantmentRandom.xml");
				XElement xelement2 = XElement.Load(text);
				if (null == xelement2)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable2 = xelement2.Elements();
				foreach (XElement xml2 in enumerable2)
				{
					FuMoRandomTemplate fuMoRandomTemplate = new FuMoRandomTemplate();
					List<int> list = new List<int>();
					fuMoRandomTemplate.Parameter = new Dictionary<double, double>();
					fuMoRandomTemplate.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml2, "ID"));
					if (FuMoManager.MaxRandomID < fuMoRandomTemplate.ID)
					{
						FuMoManager.MaxRandomID = fuMoRandomTemplate.ID;
					}
					fuMoRandomTemplate.Name = Global.GetSafeAttributeStr(xml2, "Name");
					fuMoRandomTemplate.Type = Global.GetSafeAttributeStr(xml2, "Type");
					string[] array5 = Global.GetSafeAttributeStr(xml2, "Parameter").Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array5)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						fuMoRandomTemplate.Parameter.Add(Convert.ToDouble(array3[0]), Convert.ToDouble(array3[1]));
					}
					fuMoRandomTemplate.BeginNum = Convert.ToInt32(Global.GetSafeAttributeStr(xml2, "BeginNum"));
					fuMoRandomTemplate.EndNum = Convert.ToInt32(Global.GetSafeAttributeStr(xml2, "EndNum"));
					FuMoManager.FuMoRandoms.Add(fuMoRandomTemplate.ID, fuMoRandomTemplate);
					FuMoManager.FuMoRandoms[fuMoRandomTemplate.ID].Parameter = (from p in FuMoManager.FuMoRandoms[fuMoRandomTemplate.ID].Parameter
					orderby p.Value
					select p).ToDictionary((KeyValuePair<double, double> p) => p.Key, (KeyValuePair<double, double> p) => p.Value);
					list.Add(fuMoRandomTemplate.BeginNum);
					list.Add(fuMoRandomTemplate.EndNum);
					if (!FuMoManager.IDMap.ContainsKey(fuMoRandomTemplate.ID))
					{
						FuMoManager.IDMap.Add(fuMoRandomTemplate.ID, list);
					}
					if (!FuMoManager.IDRandomsTypeMap.ContainsKey(fuMoRandomTemplate.ID))
					{
						FuMoManager.IDRandomsTypeMap.Add(fuMoRandomTemplate.ID, (int)ConfigParser.GetPropIndexByPropName(fuMoRandomTemplate.Type));
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
			}
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("EnchantmentGiveLimit", ',');
			FuMoManager.FuMoParems.GiveNum = paramValueIntArrayByName[0];
			FuMoManager.FuMoParems.AcceptNum = paramValueIntArrayByName[1];
			FuMoManager.FuMoParems.FuMoMoneyAddNum = (int)GameManager.systemParamsList.GetParamValueIntByName("EnchantmentGiveNum", -1);
			FuMoManager.FuMoParems.FuMoMoneySubNum = -(int)GameManager.systemParamsList.GetParamValueIntByName("EnchantmentCost", -1);
			FuMoManager.FuMoParems.DailyActiveCond = (int)GameManager.systemParamsList.GetParamValueIntByName("FuMoHuoyue", -1);
			int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("EnchantmentInheritCost", ',');
			FuMoManager.FuMoParems.FuMoJinBi = paramValueIntArrayByName2[0];
			FuMoManager.FuMoParems.FuMoZuanShi = paramValueIntArrayByName2[1];
			double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EnchantmentRandomNum", ',');
			FuMoManager.FuMoParems.FuMoProb1 = paramValueDoubleArrayByName[0];
			FuMoManager.FuMoParems.FuMoProb2 = paramValueDoubleArrayByName[1];
			TCPCmdDispatcher.getInstance().registerProcessorEx(2021, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2022, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2023, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2024, 4, 4, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2025, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2026, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2027, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2028, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2029, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		public static bool TryFuMoTrueAttrValue(List<int> inAttr, out List<int> outAttr)
		{
			bool flag = true;
			outAttr = null;
			if (inAttr.Count > 6)
			{
				flag = false;
			}
			for (int i = 0; i < inAttr.Count - 1; i += 2)
			{
				if (inAttr[i] == 0 || inAttr[i + 1] == 0)
				{
					flag = false;
				}
				if (i < 4 && !FuMoManager.IDRandomsTypeMap.ContainsValue(inAttr[i]))
				{
					flag = false;
				}
				if (i == 4 && !FuMoManager.IDExtrasTypeMap.ContainsValue(inAttr[i]))
				{
					flag = false;
				}
				if (!flag)
				{
					outAttr = new List<int>();
					outAttr.Add(14);
					outAttr.Add(0);
					outAttr.Add(94);
					outAttr.Add(0);
					return false;
				}
			}
			return true;
		}

		public static bool IsSameIDFromRandom(int rand1, int rand2, out int resid)
		{
			int num = -1;
			int num2 = -2;
			resid = -1;
			foreach (KeyValuePair<int, FuMoRandomTemplate> keyValuePair in FuMoManager.FuMoRandoms)
			{
				if (rand1 < keyValuePair.Value.EndNum && rand1 > keyValuePair.Value.BeginNum)
				{
					num = keyValuePair.Key;
				}
				if (rand2 < keyValuePair.Value.EndNum && rand2 > keyValuePair.Value.BeginNum)
				{
					num2 = keyValuePair.Key;
				}
			}
			bool result;
			if (num == num2)
			{
				resid = num;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool FilterAttrWeight(int rAttr1, int rAttr2, out List<int> list)
		{
			list = new List<int>();
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<int, List<int>> keyValuePair in FuMoManager.IDMap)
			{
				if (rAttr1 >= keyValuePair.Value[0] && rAttr1 <= keyValuePair.Value[1])
				{
					num = keyValuePair.Key;
				}
				if (rAttr2 >= keyValuePair.Value[0] && rAttr2 <= keyValuePair.Value[1])
				{
					num2 = keyValuePair.Key;
				}
				if (num == num2 && num != 1 && num != 10)
				{
					int randomNumber = Global.GetRandomNumber(0, 2);
					if (randomNumber == 0)
					{
						num2 = Global.GetRandomNumber(1, num);
					}
					else
					{
						num2 = Global.GetRandomNumber(num, FuMoManager.MaxRandomID);
					}
				}
				else if (num == num2 && num == 1)
				{
					num2 = Global.GetRandomNumber(num + 1, FuMoManager.MaxRandomID);
				}
				else if (num == num2 && num == FuMoManager.MaxRandomID)
				{
					num2 = Global.GetRandomNumber(1, FuMoManager.MaxRandomID);
				}
				if (num != 0 && num2 != 0)
				{
					list.Add(num);
					list.Add(num2);
					list.Sort();
					return true;
				}
			}
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2021:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int mailid = Convert.ToInt32(cmdParams[0]);
					int getType = Convert.ToInt32(cmdParams[1]);
					int num = 0;
					int num2 = Convert.ToInt32(FuMoManager.ProcessGetFuMoMoneyCmd(client, mailid, getType, out num));
					client.sendCmd(nID, string.Format("{0}:{1}", num2, num), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_UPDATA_FUMOMONEY_ACCEPTNUM", false, false);
				}
				break;
			case 2022:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int recrid = Convert.ToInt32(cmdParams[0]);
					string recname = cmdParams[1];
					int num2 = Convert.ToInt32(FuMoManager.ProcessFoMoMoneyGiveCmd(client, recrid, recname));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_GIVE_FUMOMONEY", false, false);
				}
				break;
			case 2023:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num3 = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, List<FuMoMailData>> cmdData;
					int num4 = Convert.ToInt32(FuMoManager.ProcessGetFoMoMoneyMailListCmd(client, out cmdData));
					client.sendCmd<Dictionary<int, List<FuMoMailData>>>(nID, cmdData, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_GET_FUMOMAIL_LIST", false, false);
				}
				break;
			case 2024:
				if (cmdParams == null || cmdParams.Length != 4)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int leftGoodsDbID = Convert.ToInt32(cmdParams[1]);
					int rightGoodsDbID = Convert.ToInt32(cmdParams[2]);
					int nSubMoneyType = Convert.ToInt32(cmdParams[3]);
					int num2 = Convert.ToInt32(FuMoManager.ProcessSpriteEquipAppendFuMoAttrInhertCmd(client, roleID, leftGoodsDbID, rightGoodsDbID, nSubMoneyType));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_FUMOATTR_APPEND", false, false);
				}
				break;
			case 2025:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int dbID = Convert.ToInt32(cmdParams[1]);
					FuMoCachedTemplate cmdData2 = null;
					int num2 = Convert.ToInt32(FuMoManager.ProcessEquipFuMoAttrAppendCmd(client, roleID, dbID, out cmdData2));
					client.sendCmd<FuMoCachedTemplate>(nID, cmdData2, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_FUMOATTR", false, false);
				}
				break;
			case 2026:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int dbID2 = Convert.ToInt32(cmdParams[1]);
					int num2 = Convert.ToInt32(FuMoManager.ProcessSaveFuMoAttrCmd(client, roleID, dbID2));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_SAVE_FUMOATTR", false, false);
				}
				break;
			case 2027:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					string arg = null;
					int num2 = Convert.ToInt32(FuMoManager.ProcessGiveTodayUserListCmd(client, roleID, out arg));
					client.sendCmd(nID, string.Format("{0}:{1}", num2, arg), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_TODAY_USER_GIVE_LIST", false, false);
				}
				break;
			case 2028:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					FuMoCachedTemplate cmdData3 = null;
					int num2 = Convert.ToInt32(FuMoManager.ProcessNotSaveFuMoAttrCmd(client, roleID, out cmdData3));
					client.sendCmd<FuMoCachedTemplate>(nID, cmdData3, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_NOT_SAVE_FUMOATTR", false, false);
				}
				break;
			case 2029:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int mailID = Convert.ToInt32(cmdParams[1]);
					int num2 = Convert.ToInt32(FuMoManager.UpdataIsReadCmd(client, roleID, mailID));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_UPDATA_FUMOMAIL_ISREAD", false, false);
				}
				break;
			}
			return true;
		}

		public static bool GetRandomValue(List<int> inList, out List<int> outList)
		{
			int num = 0;
			outList = new List<int>();
			lock (outList)
			{
				foreach (int key in inList)
				{
					num++;
					if (num == 3)
					{
						FuMoExtraTemplate fuMoExtraTemplate = null;
						if (!FuMoManager.FuMoExtras.TryGetValue(key, out fuMoExtraTemplate))
						{
							return false;
						}
						double random = Global.GetRandom();
						double num2 = 0.0;
						int item = 0;
						foreach (KeyValuePair<double, double> keyValuePair in fuMoExtraTemplate.Parameter)
						{
							if (random < keyValuePair.Value + num2)
							{
								item = (int)(keyValuePair.Key * 1000.0);
								break;
							}
							num2 += keyValuePair.Value;
						}
						outList.Add((int)ConfigParser.GetPropIndexByPropName(fuMoExtraTemplate.Type));
						outList.Add(item);
						break;
					}
					else
					{
						FuMoRandomTemplate fuMoRandomTemplate = null;
						if (!FuMoManager.FuMoRandoms.TryGetValue(key, out fuMoRandomTemplate))
						{
							return false;
						}
						double random = Global.GetRandom();
						double num3 = 0.0;
						int item2 = 0;
						foreach (KeyValuePair<double, double> keyValuePair in fuMoRandomTemplate.Parameter)
						{
							if (random < keyValuePair.Value + num3)
							{
								item2 = (int)(keyValuePair.Key * 1000.0);
								break;
							}
							num3 += keyValuePair.Value;
						}
						outList.Add((int)ConfigParser.GetPropIndexByPropName(fuMoRandomTemplate.Type));
						outList.Add(item2);
					}
				}
			}
			return true;
		}

		public static bool GetRandomAttrArray(out List<int> list)
		{
			list = null;
			List<int> list2 = null;
			lock (FuMoManager.Mutex)
			{
				if (FuMoManager.FilterAttrWeight(Global.GetRandomNumber(0, 100000), Global.GetRandomNumber(0, 100000), out list2))
				{
					foreach (KeyValuePair<int, FuMoExtraTemplate> keyValuePair in FuMoManager.FuMoExtras)
					{
						if (keyValuePair.Value.Condition[0] == list2[0] && keyValuePair.Value.Condition[1] == list2[1])
						{
							list2.Add(keyValuePair.Key);
							break;
						}
					}
				}
			}
			return FuMoManager.GetRandomValue(list2, out list);
		}

		private static TCPProcessCmdResults UpdateMailFuMoData2DB(GameClient client, int recrid, int num, string content, string recname)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				recrid,
				num,
				content,
				client.ClientData.Occupation
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14102, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults UpdateMailFuMoGiveNumData2DB(GameClient client, string recrid_list, int nDate, int accept, int give)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				recrid_list,
				nDate,
				accept,
				give
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14104, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults UpdateFuMoMailMap2DB(GameClient client, int rid, int give, int nDate, string recid_list)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				rid,
				nDate,
				give,
				recid_list
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14108, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults UpdateFuMoAcceptNum(GameClient client, int nDate, int num)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, nDate, num);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14101, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults DeleteFuMoMail(GameClient client, int mailid)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14109, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults DeleteFuMoMailList(GameClient client, string mailid_list)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid_list);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14110, strcmd, out array, client.ServerId);
		}

		private static TCPProcessCmdResults UpdateMailState(GameClient client, int mailid)
		{
			string[] array = null;
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14111, strcmd, out array, client.ServerId);
		}

		public static FuMoMailOptEnum GiveFuMoMoneyToDB(GameClient client, int recrid, string recname)
		{
			FuMoMailOptEnum result;
			if (Global.FindFriendData(client, recrid) == null)
			{
				result = FuMoMailOptEnum.FuMo_NotFriend;
			}
			else
			{
				bool flag = false;
				GameClient gameClient = GameManager.ClientMgr.FindClient(recrid);
				if (gameClient == null)
				{
					gameClient = new GameClient();
					gameClient.ClientData = Global.GetSafeClientDataFromLocalOrDB(recrid);
				}
				else
				{
					flag = true;
				}
				if (!GlobalNew.IsGongNengOpened(client, 104, false))
				{
					result = FuMoMailOptEnum.FuMo_GongNengWeiKaiQi;
				}
				else if (!GlobalNew.IsGongNengOpened(gameClient, 104, false))
				{
					result = FuMoMailOptEnum.FuMo_OtherGongNengWeiKaiQi;
				}
				else
				{
					int dailyActiveDataByField = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
					if (dailyActiveDataByField < FuMoManager.FuMoParems.DailyActiveCond)
					{
						result = FuMoMailOptEnum.FuMo_GiveFuMoMoneyActive;
					}
					else
					{
						string cmd = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
						Dictionary<int, FuMoMailTemp> dictionary = Global.sendToDB<Dictionary<int, FuMoMailTemp>, string>(14107, cmd, 0);
						FuMoMailTemp fuMoMailTemp = null;
						lock (dictionary)
						{
							if (dictionary.TryGetValue(client.ClientData.RoleID, out fuMoMailTemp))
							{
								if (fuMoMailTemp.Give >= FuMoManager.FuMoParems.GiveNum)
								{
									return FuMoMailOptEnum.FuMo_GiveFuMoMoneyMax;
								}
								string[] array = fuMoMailTemp.ReceiverRID.Split(new char[]
								{
									'_'
								});
								foreach (string text in array)
								{
									if (text != "" && Convert.ToInt32(text) == recrid)
									{
										LogManager.WriteLog(2, string.Format("附魔币赠送当日已经总送过了, RoleID={0}", client.ClientData.RoleID), null, true);
										return FuMoMailOptEnum.FuMo_GiveFuMoMoneyRepeat;
									}
								}
								string recid_list = string.Format("{0}_{1}", recrid, fuMoMailTemp.ReceiverRID);
								int offsetDayNow = TimeUtil.GetOffsetDayNow();
								fuMoMailTemp.Give++;
								if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateFuMoMailMap2DB(client, fuMoMailTemp.SenderRID, fuMoMailTemp.Give, offsetDayNow, recid_list))
								{
									LogManager.WriteLog(2, string.Format("更新映射表出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_RunFuMoDBError;
								}
							}
							else if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateMailFuMoGiveNumData2DB(client, recrid.ToString(), TimeUtil.GetOffsetDayNow(), 0, 1))
							{
								LogManager.WriteLog(2, string.Format("修改赠送次数出错, RoleID={0}", client.ClientData.RoleID), null, true);
								return FuMoMailOptEnum.FuMo_RunFuMoDBError;
							}
						}
						if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateMailFuMoData2DB(client, recrid, FuMoManager.FuMoParems.FuMoMoneyAddNum, GLang.GetLang(6003, new object[0]), recname))
						{
							LogManager.WriteLog(2, string.Format("插入新的赠送附魔灵石邮件出错, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_RunFuMoDBError;
						}
						else
						{
							if (flag)
							{
								gameClient._IconStateMgr.CheckFuMoMailIcon(gameClient);
							}
							EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
							{
								client.ClientData.RoleID,
								recrid,
								FuMoManager.FuMoParems.FuMoMoneyAddNum,
								"赠送给对方附魔灵石"
							});
							result = FuMoMailOptEnum.FuMo_AcceptSucc;
						}
					}
				}
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessFoMoMoneyGiveCmd(GameClient client, int recrid, string recname)
		{
			return FuMoManager.GiveFuMoMoneyToDB(client, recrid, recname);
		}

		private static FuMoMailOptEnum ProcessGetFoMoMoneyMailListCmd(GameClient client, out Dictionary<int, List<FuMoMailData>> maildata)
		{
			maildata = null;
			maildata = Global.sendToDB<Dictionary<int, List<FuMoMailData>>, string>(14106, client.ClientData.RoleID.ToString(), 0);
			client._IconStateMgr.CheckFuMoMailIcon(client);
			return FuMoMailOptEnum.FuMo_AcceptSucc;
		}

		private static FuMoMailOptEnum ProcessGetFuMoMoneyCmd(GameClient client, int mailid, int GetType, out int ResGetNum)
		{
			ResGetNum = 0;
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
			string[] array = null;
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14105, strcmd, out array, client.ServerId);
			FuMoMailOptEnum result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				LogManager.WriteLog(2, string.Format("获取接受次数出错, RoleID={0}", client.ClientData.RoleID), null, true);
				result = FuMoMailOptEnum.FuMo_GetAcceptError;
			}
			else
			{
				int num = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[0]);
				if (num2 != client.ClientData.RoleID)
				{
					result = FuMoMailOptEnum.FuMo_Fail;
				}
				else if (num >= FuMoManager.FuMoParems.AcceptNum)
				{
					result = FuMoMailOptEnum.FuMo_AcceptMaxTime;
				}
				else
				{
					Dictionary<int, List<FuMoMailData>> dictionary = Global.sendToDB<Dictionary<int, List<FuMoMailData>>, string>(14106, client.ClientData.RoleID.ToString(), 0);
					if (dictionary == null)
					{
						LogManager.WriteLog(2, string.Format("查找玩家的附魔灵石邮件列表为空, RoleID={0}", client.ClientData.RoleID), null, true);
						result = FuMoMailOptEnum.FuMo_GetMailListError;
					}
					else
					{
						lock (dictionary)
						{
							List<FuMoMailData> list = new List<FuMoMailData>();
							if (!dictionary.TryGetValue(client.ClientData.RoleID, out list))
							{
								LogManager.WriteLog(2, string.Format("查找玩家的附魔灵石邮件列表为空, RoleID={0}", client.ClientData.RoleID), null, true);
								return FuMoMailOptEnum.FuMo_GetMailListError;
							}
							if (GetType == 0)
							{
								if (list == null)
								{
									return FuMoMailOptEnum.FuMo_Fail;
								}
								bool flag2 = true;
								foreach (FuMoMailData fuMoMailData in list)
								{
									if (fuMoMailData.MaillID == mailid)
									{
										int offsetDayNow = TimeUtil.GetOffsetDayNow();
										if (num == -1)
										{
											if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateMailFuMoGiveNumData2DB(client, "", offsetDayNow, 1, 0))
											{
												LogManager.WriteLog(2, string.Format("领取插入映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
												return FuMoMailOptEnum.FuMo_DBError;
											}
										}
										else if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateFuMoAcceptNum(client, offsetDayNow, num + 1))
										{
											LogManager.WriteLog(2, string.Format("领取更新映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
											return FuMoMailOptEnum.FuMo_DBError;
										}
										if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.DeleteFuMoMail(client, fuMoMailData.MaillID))
										{
											LogManager.WriteLog(2, string.Format("领取删除邮件数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
											return FuMoMailOptEnum.FuMo_DBError;
										}
										if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, FuMoManager.FuMoParems.FuMoMoneyAddNum, "附魔邮件领取", true, true, false))
										{
											return FuMoMailOptEnum.FuMo_MoneyError;
										}
										flag2 = false;
										ResGetNum = 1;
										if (null != client)
										{
											client._IconStateMgr.CheckFuMoMailIcon(client);
										}
										EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.RoleID,
											FuMoManager.FuMoParems.FuMoMoneySubNum,
											"手动领取附魔灵石邮件"
										});
										return FuMoMailOptEnum.FuMo_AcceptSucc;
									}
								}
								if (flag2)
								{
									LogManager.WriteLog(0, string.Format("没有找到当前玩家的附魔灵石邮件id, RoleID={0} MailID={1}", client.ClientData.RoleID, mailid), null, true);
									return FuMoMailOptEnum.FuMo_NotFriend;
								}
							}
							else
							{
								if (GetType != 1)
								{
									LogManager.WriteLog(2, string.Format("领取附魔灵石操作类型出错, RoleID={0} Type={1}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_NotFriend;
								}
								if (list == null)
								{
									return FuMoMailOptEnum.FuMo_Fail;
								}
								bool flag2 = false;
								int num3;
								if (num == -1)
								{
									flag2 = true;
									num3 = FuMoManager.FuMoParems.AcceptNum;
								}
								else
								{
									num3 = FuMoManager.FuMoParems.AcceptNum - num;
								}
								int num4 = 0;
								int num5 = 0;
								string text = null;
								foreach (FuMoMailData fuMoMailData in list)
								{
									if (num4 >= num3)
									{
										break;
									}
									num5 += FuMoManager.FuMoParems.FuMoMoneyAddNum;
									text = string.Format("{0}_{1}", fuMoMailData.MaillID, text);
									num4++;
								}
								int offsetDayNow = TimeUtil.GetOffsetDayNow();
								if (flag2)
								{
									if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateMailFuMoGiveNumData2DB(client, "", offsetDayNow, num4, 0))
									{
										LogManager.WriteLog(2, string.Format("批量领取插入映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
										return FuMoMailOptEnum.FuMo_DBError;
									}
								}
								else if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateFuMoAcceptNum(client, offsetDayNow, num + num4))
								{
									LogManager.WriteLog(2, string.Format("批量领取更新映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_DBError;
								}
								ResGetNum = num4;
								if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.DeleteFuMoMailList(client, text))
								{
									LogManager.WriteLog(2, string.Format("批量领取删除邮件数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_DBError;
								}
								if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, num5, "附魔邮件领取", true, true, false))
								{
									return FuMoMailOptEnum.FuMo_MoneyError;
								}
								if (null != client)
								{
									client._IconStateMgr.CheckFuMoMailIcon(client);
								}
								EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleID,
									num5,
									"一键领取附魔灵石邮件"
								});
								return FuMoMailOptEnum.FuMo_AcceptSucc;
							}
						}
						result = FuMoMailOptEnum.FuMo_Fail;
					}
				}
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessEquipFuMoAttrAppendCmd(GameClient client, int roleID, int DbID, out FuMoCachedTemplate FMCached)
		{
			FMCached = new FuMoCachedTemplate
			{
				Result = -1,
				RoleID = roleID,
				DbID = DbID,
				AttrTypeValue = null
			};
			FuMoMailOptEnum result;
			if (!GlobalNew.IsGongNengOpened(client, 104, false))
			{
				LogManager.WriteLog(2, string.Format("附魔玩家功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				FMCached.Result = 19;
				result = FuMoMailOptEnum.FuMo_GongNengWeiKaiQi;
			}
			else if (Global.GetRoleParamsInt32FromDB(client, "10217") < Math.Abs(FuMoManager.FuMoParems.FuMoMoneySubNum))
			{
				FMCached.Result = 15;
				result = FuMoMailOptEnum.FuMo_MoneyError;
			}
			else
			{
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, DbID);
				if (goodsByDbID == null)
				{
					LogManager.WriteLog(2, string.Format("没有找到附魔物品,RoleID={0} ", client.ClientData.RoleID), null, true);
					FMCached.Result = 11;
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else if (!GoodsUtil.IsEquip(goodsByDbID.GoodsID))
				{
					LogManager.WriteLog(2, string.Format("不属于可附魔类型装备,RoleID={0}, Goodid = {1}", client.ClientData.RoleID, goodsByDbID.GoodsID), null, true);
					FMCached.Result = 17;
					result = FuMoMailOptEnum.FuMo_NotFuMoType;
				}
				else
				{
					List<int> list = null;
					if (!FuMoManager.GetRandomAttrArray(out list))
					{
						LogManager.WriteLog(2, string.Format("获取随机属性出错,RoleID={0}, Goodid = {1}", client.ClientData.RoleID, goodsByDbID.GoodsID), null, true);
						FMCached.Result = 20;
						result = FuMoMailOptEnum.FuMo_GetRandomAttrError;
					}
					else
					{
						List<int> list2 = null;
						if (!FuMoManager.TryFuMoTrueAttrValue(list, out list2))
						{
							list = list2;
						}
						lock (FuMoManager.FuMoCached)
						{
							if (FuMoManager.FuMoCached.ContainsKey(client.ClientData.RoleID))
							{
								if (!FuMoManager.FuMoCached.Remove(client.ClientData.RoleID))
								{
									FMCached.Result = -1;
									return FuMoMailOptEnum.FuMo_Fail;
								}
							}
						}
						lock (list)
						{
							if (null == goodsByDbID.ElementhrtsProps)
							{
								UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
								{
									RoleID = client.ClientData.RoleID,
									DbID = DbID
								};
								updateGoodsArgs.ElementhrtsProps = list;
								Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs, true);
								if (goodsByDbID.Using > 0)
								{
									Global.RefreshEquipPropAndNotify(client);
								}
							}
							FMCached.Result = 1;
							FMCached.RoleID = client.ClientData.RoleID;
							FMCached.DbID = DbID;
							FMCached.AttrTypeValue = list;
							FuMoManager.FuMoCached.Add(client.ClientData.RoleID, FMCached);
						}
						if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, FuMoManager.FuMoParems.FuMoMoneySubNum, "附魔消耗", true, true, false))
						{
							result = FuMoMailOptEnum.FuMo_MoneyError;
						}
						else
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(DbID, "装备附魔", "装备附魔操作", client.ClientData.RoleName, "系统", "添加附魔属性", 0, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, goodsByDbID);
							result = FuMoMailOptEnum.FuMo_AcceptSucc;
						}
					}
				}
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessSaveFuMoAttrCmd(GameClient client, int roleID, int DbID)
		{
			GoodsData goodsByDbID = Global.GetGoodsByDbID(client, DbID);
			FuMoMailOptEnum result;
			if (goodsByDbID == null)
			{
				LogManager.WriteLog(2, string.Format("没有找到附魔物品,RoleID={0} DbID={1} ", client.ClientData.RoleID, DbID), null, true);
				result = FuMoMailOptEnum.FuMo_GetGoodInfo;
			}
			else
			{
				FuMoCachedTemplate fuMoCachedTemplate = null;
				if (!FuMoManager.FuMoCached.TryGetValue(roleID, out fuMoCachedTemplate))
				{
					result = FuMoMailOptEnum.FuMo_Fail;
				}
				else
				{
					lock (fuMoCachedTemplate)
					{
						if (fuMoCachedTemplate.RoleID != roleID && fuMoCachedTemplate.DbID != DbID)
						{
							LogManager.WriteLog(2, string.Format("玩家对应装备附魔保存出错 id RoleID={0} 缓存DbID {1} 参数DbID{2}", client.ClientData.RoleID, fuMoCachedTemplate.DbID, DbID), null, true);
							return FuMoMailOptEnum.FuMo_SaveError;
						}
						if (fuMoCachedTemplate.Result != 1)
						{
							LogManager.WriteLog(2, string.Format("缓存的属性是错误的 id RoleID={0} 缓存DbID {1} 参数DbID{2}", client.ClientData.RoleID, fuMoCachedTemplate.DbID, DbID), null, true);
							return FuMoMailOptEnum.FuMo_SaveError;
						}
						UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
						{
							RoleID = client.ClientData.RoleID,
							DbID = DbID
						};
						updateGoodsArgs.ElementhrtsProps = fuMoCachedTemplate.AttrTypeValue;
						Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs, true);
						if (!FuMoManager.FuMoCached.Remove(client.ClientData.RoleID))
						{
							return FuMoMailOptEnum.FuMo_Fail;
						}
						if (goodsByDbID.Using > 0)
						{
							Global.RefreshEquipPropAndNotify(client);
						}
					}
					result = FuMoMailOptEnum.FuMo_AcceptSucc;
				}
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessNotSaveFuMoAttrCmd(GameClient client, int roleID, out FuMoCachedTemplate temp)
		{
			temp = null;
			FuMoCachedTemplate fuMoCachedTemplate = null;
			FuMoMailOptEnum result;
			if (FuMoManager.FuMoCached.TryGetValue(roleID, out fuMoCachedTemplate))
			{
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, fuMoCachedTemplate.DbID);
				if (goodsByDbID == null)
				{
					LogManager.WriteLog(2, string.Format("没有找到附魔物品,RoleID={0} DbID={1} ", client.ClientData.RoleID, fuMoCachedTemplate.DbID), null, true);
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else
				{
					temp = fuMoCachedTemplate;
					result = FuMoMailOptEnum.FuMo_AcceptSucc;
				}
			}
			else
			{
				result = FuMoMailOptEnum.FuMo_RoleInfoError;
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessSpriteEquipAppendFuMoAttrInhertCmd(GameClient client, int roleID, int leftGoodsDbID, int rightGoodsDbID, int nSubMoneyType)
		{
			GoodsData goodsByDbID = Global.GetGoodsByDbID(client, leftGoodsDbID);
			FuMoMailOptEnum result;
			if (null == goodsByDbID)
			{
				LogManager.WriteLog(0, string.Format("获取物品信息出错, RoleID={0}", client.ClientData.RoleID), null, true);
				result = FuMoMailOptEnum.FuMo_GetGoodInfo;
			}
			else if (!GoodsUtil.IsEquip(goodsByDbID.GoodsID))
			{
				result = FuMoMailOptEnum.FuMo_NotFuMoType;
			}
			else
			{
				GoodsData goodsByDbID2 = Global.GetGoodsByDbID(client, rightGoodsDbID);
				if (null == goodsByDbID2)
				{
					LogManager.WriteLog(0, string.Format("获取物品信息出错, RoleID={0}", client.ClientData.RoleID), null, true);
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else if (!GoodsUtil.IsEquip(goodsByDbID2.GoodsID))
				{
					result = FuMoMailOptEnum.FuMo_NotFuMoType;
				}
				else
				{
					int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsByDbID.GoodsID);
					int mainOccupationByGoodsID2 = Global.GetMainOccupationByGoodsID(goodsByDbID2.GoodsID);
					if (mainOccupationByGoodsID != mainOccupationByGoodsID2)
					{
						LogManager.WriteLog(2, string.Format("装备职业不同, RoleID={0}", client.ClientData.RoleID), null, true);
						result = FuMoMailOptEnum.FuMo_EquipJobDiff;
					}
					else
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByDbID.GoodsID);
						int goodsCatetoriy2 = Global.GetGoodsCatetoriy(goodsByDbID2.GoodsID);
						int num = GoodsUtil.CanUpgradeInhert(goodsCatetoriy, goodsCatetoriy2, 14);
						if (num < 0)
						{
							LogManager.WriteLog(2, string.Format("装备左右类型不同, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_LiftRightEquipDiff;
						}
						else if (goodsByDbID.Site != 0 || goodsByDbID2.Site != 0)
						{
							LogManager.WriteLog(2, string.Format("装备不在背包拒绝操作, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_EquipNotInGoods;
						}
						else if (nSubMoneyType < 1 || nSubMoneyType > 2)
						{
							LogManager.WriteLog(0, string.Format("参数使用货币类型出错, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_Fail;
						}
						else
						{
							if (nSubMoneyType == 1)
							{
								if (!Global.SubBindTongQianAndTongQian(client, FuMoManager.FuMoParems.FuMoJinBi, "装备附魔传承"))
								{
									return FuMoMailOptEnum.FuMo_JinBiLacking;
								}
							}
							else if (nSubMoneyType == 2)
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, FuMoManager.FuMoParems.FuMoZuanShi, "装备附魔传承", true, true, true, true, DaiBiSySType.None))
								{
									return FuMoMailOptEnum.FuMo_ZuanShiLacking;
								}
							}
							if (goodsByDbID.ElementhrtsProps.Count <= 0)
							{
								LogManager.WriteLog(0, string.Format("左边没有附魔属性, RoleID={0}", client.ClientData.RoleID), null, true);
								result = FuMoMailOptEnum.FuMo_FuMoAttrError;
							}
							else
							{
								int binding = 0;
								if (goodsByDbID2.Binding == 1 || goodsByDbID.Binding == 1)
								{
									binding = 1;
								}
								List<int> list = new List<int>();
								lock (list)
								{
									list.AddRange(goodsByDbID.ElementhrtsProps);
									if (list.Count < 4)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = leftGoodsDbID
									};
									updateGoodsArgs.ElementhrtsProps = null;
									if (Global.UpdateGoodsProp(client, goodsByDbID, updateGoodsArgs, true) < 0)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
									UpdateGoodsArgs updateGoodsArgs2 = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = rightGoodsDbID
									};
									updateGoodsArgs2.Binding = binding;
									updateGoodsArgs2.ElementhrtsProps = new List<int>(list);
									if (Global.UpdateGoodsProp(client, goodsByDbID2, updateGoodsArgs2, true) < 0)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
								}
								Global.ModRoleGoodsEvent(client, goodsByDbID, 0, "装备附魔传承_提供方", false);
								Global.ModRoleGoodsEvent(client, goodsByDbID2, 0, "装备附魔传承_接受方", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID.GoodsID, (long)goodsByDbID.Id, 0, goodsByDbID.GCount, "装备附魔传承_提供方");
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsByDbID2.GoodsID, (long)goodsByDbID2.Id, 0, goodsByDbID2.GCount, "装备附魔传承_接受方");
								ProcessTask.ProcessAddTaskVal(client, TaskTypes.EquipChuanCheng, -1, 1, new object[0]);
								Global.BroadcastAppendChuanChengOk(client, goodsByDbID, goodsByDbID2);
								if (goodsByDbID.Using > 0 || goodsByDbID2.Using > 0)
								{
									Global.RefreshEquipPropAndNotify(client);
								}
								result = FuMoMailOptEnum.FuMo_AcceptSucc;
							}
						}
					}
				}
			}
			return result;
		}

		private static FuMoMailOptEnum ProcessGiveTodayUserListCmd(GameClient client, int roleID, out string UserList)
		{
			UserList = null;
			string cmd = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
			Dictionary<int, FuMoMailTemp> dictionary = Global.sendToDB<Dictionary<int, FuMoMailTemp>, string>(14107, cmd, 0);
			FuMoMailTemp fuMoMailTemp = null;
			lock (dictionary)
			{
				if (dictionary.TryGetValue(client.ClientData.RoleID, out fuMoMailTemp))
				{
					UserList = fuMoMailTemp.ReceiverRID;
				}
			}
			return FuMoMailOptEnum.FuMo_AcceptSucc;
		}

		private static FuMoMailOptEnum UpdataIsReadCmd(GameClient client, int roleID, int mailID)
		{
			string strcmd = string.Format("{0}:{1}", roleID, mailID);
			string[] array = null;
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14111, strcmd, out array, client.ServerId);
			FuMoMailOptEnum result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = FuMoMailOptEnum.FuMo_Fail;
			}
			else
			{
				result = FuMoMailOptEnum.FuMo_AcceptSucc;
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public static Dictionary<int, FuMoExtraTemplate> FuMoExtras = new Dictionary<int, FuMoExtraTemplate>();

		public static Dictionary<int, FuMoRandomTemplate> FuMoRandoms = new Dictionary<int, FuMoRandomTemplate>();

		public static FuMoParemLimit FuMoParems = new FuMoParemLimit();

		public static Dictionary<int, List<int>> IDMap = new Dictionary<int, List<int>>();

		public static Dictionary<int, FuMoCachedTemplate> FuMoCached = new Dictionary<int, FuMoCachedTemplate>();

		public static Dictionary<int, int> IDRandomsTypeMap = new Dictionary<int, int>();

		public static Dictionary<int, int> IDExtrasTypeMap = new Dictionary<int, int>();

		public static int MaxRandomID = 0;

		public static int MaxExtrasID = 0;

		public static object Mutex = new object();

		private static FuMoManager instance = new FuMoManager();
	}
}
