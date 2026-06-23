using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.TuJian;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class AlchemyManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static AlchemyManager getInstance()
		{
			return AlchemyManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1085, 1, 1, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1086, 3, 4, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1087, 2, 2, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!GlobalNew.IsGongNengOpened(client, 90, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1085:
					result = this.ProcessAlchemyDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1086:
					result = this.ProcessAlchemyAddElementCmd(client, nID, bytes, cmdParams);
					break;
				case 1087:
					result = this.ProcessAlchemyExcuteCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public void OnLogin(GameClient client)
		{
			this.AlchemyRollBack(client, client.ClientData.AlchemyInfo.rollbackType);
			this.RefreshAlchemyProps(client);
		}

		public bool AlchemyRollBackOffline(int rid, string rollbackType)
		{
			return Global.sendToDB<bool, string>(13098, string.Format("{0}:{1}", rid, rollbackType), 0);
		}

		public bool AlchemyRollBackCheck(int costType, int useNum)
		{
			bool result;
			if (useNum <= 0 || costType < 1 || costType >= 15)
			{
				result = false;
			}
			else
			{
				Dictionary<int, AlchemyConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.AlchemyConfig;
				}
				AlchemyConfigData alchemyConfigData = null;
				result = (dictionary.TryGetValue(costType, out alchemyConfigData) && useNum >= alchemyConfigData.Unit);
			}
			return result;
		}

		public void AlchemyRollBack(GameClient client, string rollbackType)
		{
			if (GlobalNew.IsGongNengOpened(client, 90, false))
			{
				if (!string.IsNullOrEmpty(rollbackType))
				{
					string[] array = rollbackType.Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						int num = Global.SafeConvertToInt32(array[0]);
						int num2 = Global.SafeConvertToInt32(array[1]);
						Dictionary<int, AlchemyConfigData> dictionary = null;
						lock (this.ConfigMutex)
						{
							dictionary = this.AlchemyConfig;
						}
						AlchemyConfigData alchemyConfigData = null;
						if (!dictionary.TryGetValue(num, out alchemyConfigData) || num2 < alchemyConfigData.Unit)
						{
							LogManager.WriteLog(3, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", client.ClientData.RoleID, rollbackType), null, true);
							client.ClientData.AlchemyInfo.rollbackType = "";
							this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
						}
						else
						{
							num2 -= num2 % alchemyConfigData.Unit;
							int num3 = num2 / alchemyConfigData.Unit * alchemyConfigData.Element;
							int num4 = 0;
							client.ClientData.AlchemyInfo.HistCost.TryGetValue(num, out num4);
							if (num4 < num2 || client.ClientData.AlchemyInfo.BaseData.Element < num3)
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", client.ClientData.RoleID, rollbackType), null, true);
								client.ClientData.AlchemyInfo.rollbackType = "";
								this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
							}
							else
							{
								GameManager.ClientMgr.ModifyAlchemyElementValue(client, -num3, "GM命令-alchemy", false, false);
								this.ModifyAddElementCost(client, num, num2, true);
								client.ClientData.AlchemyInfo.HistCost[num] = num4 - num2;
								this.UpdateAlchemyDataDB(client);
								client.ClientData.AlchemyInfo.rollbackType = "";
								this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
								LogManager.WriteLog(3, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 成功！", client.ClientData.RoleID, rollbackType), null, true);
							}
						}
					}
				}
			}
		}

		private bool CheckCostEnough(GameClient client, int costType, int useNum, bool bindOnly)
		{
			bool result;
			switch (costType)
			{
			case 1:
				result = (GameManager.ClientMgr.GetTianDiJingYuanValue(client) >= useNum);
				break;
			case 2:
				result = (GameManager.ClientMgr.GetChengJiuPointsValue(client) >= useNum);
				break;
			case 3:
				result = (GameManager.ClientMgr.GetShengWangValue(client) >= useNum);
				break;
			case 4:
				result = (client.ClientData.StarSoul >= useNum);
				break;
			case 5:
				result = (GameManager.ClientMgr.GetMUMoHeValue(client) >= useNum);
				break;
			case 6:
				result = (Global.GetRoleParamsInt32FromDB(client, "ElementPowder") >= useNum);
				break;
			case 7:
				result = (GameManager.ClientMgr.GetZaiZaoValue(client) >= useNum);
				break;
			case 8:
				result = (client != null && client.ClientData.MyGuardStatueDetail.IsActived && client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint >= useNum);
				break;
			case 9:
				result = (client.ClientData.TianTiData.RongYao >= useNum);
				break;
			case 10:
				result = (client.ClientData.FluorescentPoint >= useNum);
				break;
			case 11:
				result = (GameManager.ClientMgr.GetLangHunFenMoValue(client) >= useNum);
				break;
			case 12:
				result = (client.ClientData.ShenLiJingHuaPoints >= useNum);
				break;
			case 13:
				result = (client.ClientData.BangGong >= useNum);
				break;
			case 14:
				result = (GameManager.ClientMgr.GetOrnamentCharmPointValue(client) >= useNum);
				break;
			default:
				if (costType >= this.MinGoodsID && useNum > 0)
				{
					if (bindOnly)
					{
						result = (Global.GetTotalBindGoodsCountByID(client, costType) >= useNum);
					}
					else
					{
						result = (Global.GetTotalGoodsCountByID(client, costType) >= useNum);
					}
				}
				else
				{
					result = false;
				}
				break;
			}
			return result;
		}

		private bool ModifyAddElementCost(GameClient client, int costType, int useNum, bool bindOnly)
		{
			bool result;
			if (0 == useNum)
			{
				result = false;
			}
			else
			{
				string text = "炼金-灌注";
				if (useNum > 0)
				{
					text = "炼金-灌注GM回滚";
				}
				bool flag;
				switch (costType)
				{
				case 1:
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, useNum, text, true, true, false);
					flag = true;
					break;
				case 2:
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, useNum, text, true, true);
					flag = true;
					break;
				case 3:
					GameManager.ClientMgr.ModifyShengWangValue(client, useNum, text, true, true);
					flag = true;
					break;
				case 4:
					GameManager.ClientMgr.ModifyStarSoulValue(client, useNum, text, true, true);
					flag = true;
					break;
				case 5:
					GameManager.ClientMgr.ModifyMUMoHeValue(client, useNum, text, true, true, false);
					flag = true;
					break;
				case 6:
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, useNum, text, true, false);
					flag = true;
					break;
				case 7:
					GameManager.ClientMgr.ModifyZaiZaoValue(client, useNum, text, true, true, false);
					flag = true;
					break;
				case 8:
					SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, useNum, text);
					flag = true;
					break;
				case 9:
					flag = GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, useNum, text, true);
					break;
				case 10:
					if (useNum > 0)
					{
						flag = GameManager.FluorescentGemMgr.AddFluorescentPoint(client, useNum, text, true);
					}
					else
					{
						flag = GameManager.FluorescentGemMgr.DecFluorescentPoint(client, -useNum, text, false);
					}
					break;
				case 11:
					GameManager.ClientMgr.ModifyLangHunFenMoValue(client, useNum, text, true, true);
					flag = true;
					break;
				case 12:
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, useNum, text, true, true);
					flag = true;
					break;
				case 13:
					if (useNum > 0)
					{
						flag = GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref useNum, AddBangGongTypes.Alchemy, 0);
						if (flag)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", text, "系统", client.ClientData.RoleName, "增加", useNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
						}
					}
					else
					{
						flag = GameManager.ClientMgr.SubUserBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, -useNum);
						if (flag)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", text, "系统", client.ClientData.RoleName, "减少", -useNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
						}
					}
					break;
				case 14:
					GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, useNum, text, true, true, false);
					flag = true;
					break;
				default:
					if (costType >= this.MinGoodsID)
					{
						if (useNum > 0)
						{
							GoodsData goodsData = new GoodsData
							{
								GoodsID = costType,
								GCount = useNum,
								Binding = 1
							};
							if (!Global.CanAddGoodsNum(client, useNum))
							{
								Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(5000, new object[0]), GLang.GetLang(5000, new object[0]), 1.0);
								GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(5001, new object[0]));
							}
							else
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, text, goodsData.Endtime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
							}
						}
						else
						{
							useNum = -useNum;
							if (Global.GetTotalGoodsCountByID(client, costType) < useNum)
							{
								return false;
							}
							int totalBindGoodsCountByID = Global.GetTotalBindGoodsCountByID(client, costType);
							if (bindOnly && totalBindGoodsCountByID < useNum)
							{
								return false;
							}
							int num = Math.Min(totalBindGoodsCountByID, useNum);
							if (num > 0)
							{
								bool flag2;
								bool flag3;
								if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costType, num, false, out flag2, out flag3, false))
								{
									LogManager.WriteLog(2, "扣除物品时发现道具不足", null, true);
								}
							}
							int num2 = useNum - num;
							if (num2 <= 0)
							{
								return true;
							}
							bool flag4;
							bool flag5;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costType, num2, false, out flag4, out flag5, false))
							{
								LogManager.WriteLog(2, "扣除物品时发现道具不足", null, true);
							}
							return true;
						}
					}
					return false;
				}
				result = flag;
			}
			return result;
		}

		private int GetTodayAddElementCost(GameClient client, int costType)
		{
			int result = 0;
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			if (offsetDay == client.ClientData.AlchemyInfo.ElementDayID)
			{
				client.ClientData.AlchemyInfo.BaseData.ToDayCost.TryGetValue(costType, out result);
			}
			return result;
		}

		private void UpdateTodayAddElementCost(GameClient client, int costType, int useNum)
		{
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			int todayAddElementCost = this.GetTodayAddElementCost(client, costType);
			if (offsetDay > client.ClientData.AlchemyInfo.ElementDayID)
			{
				client.ClientData.AlchemyInfo.ElementDayID = offsetDay;
				client.ClientData.AlchemyInfo.BaseData.ToDayCost.Clear();
			}
			client.ClientData.AlchemyInfo.BaseData.ToDayCost[costType] = todayAddElementCost + useNum;
		}

		private void UpdateHistAddElementCost(GameClient client, int costType, int useNum)
		{
			int num = 0;
			client.ClientData.AlchemyInfo.HistCost.TryGetValue(costType, out num);
			client.ClientData.AlchemyInfo.HistCost[costType] = (int)Math.Min((long)num + (long)useNum, 2147483647L);
		}

		public bool UpdateAlchemyDataDB(GameClient client)
		{
			return Global.sendToDB<bool, AlchemyDataDB>(13097, client.ClientData.AlchemyInfo, client.ServerId);
		}

		private int RandomAlchemyProp(GameClient client)
		{
			int result = 0;
			int num = 0;
			Dictionary<int, int> alchemyValue = client.ClientData.AlchemyInfo.BaseData.AlchemyValue;
			foreach (KeyValuePair<int, int> keyValuePair in alchemyValue)
			{
				if (num == 0 || keyValuePair.Value < num)
				{
					num = keyValuePair.Value;
				}
			}
			List<int> list = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				int num2 = 0;
				alchemyValue.TryGetValue(i, out num2);
				if (num2 - num < this.RandomLimit)
				{
					list.Add(i);
				}
			}
			if (list.Count != 0)
			{
				result = list[Global.GetRandomNumber(0, list.Count)];
			}
			return result;
		}

		private void RefreshAlchemyProps(GameClient client)
		{
			List<double> list = null;
			lock (this.ConfigMutex)
			{
				list = this.AlchemyPropList;
			}
			double[] array = new double[177];
			foreach (KeyValuePair<int, int> keyValuePair in client.ClientData.AlchemyInfo.BaseData.AlchemyValue)
			{
				switch (keyValuePair.Key)
				{
				case 0:
					array[13] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 1:
					array[7] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[8] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[9] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[10] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 2:
					array[3] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[4] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[5] += (double)keyValuePair.Value * list[keyValuePair.Key];
					array[6] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 3:
					array[18] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 4:
					array[19] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 5:
					array[27] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 6:
					array[38] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 7:
					array[44] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				case 8:
					array[30] += (double)keyValuePair.Value * list[keyValuePair.Key];
					break;
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.Alchemy,
				array
			});
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				default(DelayExecProcIds),
				2
			});
		}

		public bool ProcessAlchemyDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (offsetDay > client.ClientData.AlchemyInfo.ElementDayID)
				{
					client.ClientData.AlchemyInfo.BaseData.ToDayCost.Clear();
					client.ClientData.AlchemyInfo.ElementDayID = offsetDay;
				}
				byte[] buffer = DataHelper.ObjectToBytes<AlchemyData>(client.ClientData.AlchemyInfo.BaseData);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAlchemyAddElementCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				int num4 = Convert.ToInt32(cmdParams[2]);
				bool bindOnly = true;
				if (cmdParams.Length >= 4)
				{
					bindOnly = (Convert.ToInt32(cmdParams[3]) > 0);
				}
				Dictionary<int, AlchemyConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.AlchemyConfig;
				}
				AlchemyConfigData alchemyConfigData = null;
				if (!dictionary.TryGetValue(num3, out alchemyConfigData))
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						0,
						num3,
						0
					}), false);
					return true;
				}
				if (!this.CheckCostEnough(client, num3, num4, bindOnly) || num4 < alchemyConfigData.Unit)
				{
					if (num3 < this.MinGoodsID)
					{
						num = -12;
					}
					else
					{
						num = -6;
					}
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						0,
						num3,
						0
					}), false);
					return true;
				}
				int todayAddElementCost = this.GetTodayAddElementCost(client, num3);
				if (alchemyConfigData.Limit != -1 && todayAddElementCost + num4 > alchemyConfigData.Limit)
				{
					num = -36;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						0,
						num3,
						todayAddElementCost
					}), false);
					return true;
				}
				num4 -= num4 % alchemyConfigData.Unit;
				if (!this.ModifyAddElementCost(client, num3, -num4, bindOnly))
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						0,
						num3,
						todayAddElementCost
					}), false);
					return true;
				}
				GameManager.ClientMgr.ModifyAlchemyElementValue(client, num4 / alchemyConfigData.Unit * alchemyConfigData.Element, "灌注", false, false);
				this.UpdateTodayAddElementCost(client, num3, num4);
				this.UpdateHistAddElementCost(client, num3, num4);
				this.UpdateAlchemyDataDB(client);
				todayAddElementCost = this.GetTodayAddElementCost(client, num3);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					client.ClientData.AlchemyInfo.BaseData.Element,
					num3,
					todayAddElementCost
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAlchemyExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				if (num3 != 0 && num3 != 1)
				{
					return true;
				}
				string text = "";
				if (num3 == 0)
				{
					if (client.ClientData.AlchemyInfo.BaseData.Element < this.LevelUpElement)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							client.ClientData.AlchemyInfo.BaseData.Element,
							text
						}), false);
						return true;
					}
					int num4 = this.RandomAlchemyProp(client);
					text = string.Format("{0}", num4);
					int num5 = 0;
					client.ClientData.AlchemyInfo.BaseData.AlchemyValue.TryGetValue(num4, out num5);
					num5 = (client.ClientData.AlchemyInfo.BaseData.AlchemyValue[num4] = num5 + 1);
					GameManager.ClientMgr.ModifyAlchemyElementValue(client, -this.LevelUpElement, "炼金", false, false);
				}
				else
				{
					if (client.ClientData.AlchemyInfo.BaseData.Element < this.LevelUpElement * 10)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							client.ClientData.AlchemyInfo.BaseData.Element,
							text
						}), false);
						return true;
					}
					for (int i = 0; i < 10; i++)
					{
						int num4 = this.RandomAlchemyProp(client);
						text += string.Format("{0}|", num4);
						int num5 = 0;
						client.ClientData.AlchemyInfo.BaseData.AlchemyValue.TryGetValue(num4, out num5);
						num5 = (client.ClientData.AlchemyInfo.BaseData.AlchemyValue[num4] = num5 + 1);
					}
					GameManager.ClientMgr.ModifyAlchemyElementValue(client, -this.LevelUpElement * 10, "炼金", false, false);
				}
				this.RefreshAlchemyProps(client);
				this.UpdateAlchemyDataDB(client);
				if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
				{
					text = text.Substring(0, text.Length - 1);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					client.ClientData.AlchemyInfo.BaseData.Element,
					text
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("AlchemyLevelUp");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				this.LevelUpElement = Global.SafeConvertToInt32(paramValueByName);
			}
			List<double> list = new List<double>();
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("AlchemyProperty");
			if (!string.IsNullOrEmpty(paramValueByName2))
			{
				string[] array = paramValueByName2.Split(new char[]
				{
					','
				});
				foreach (string str in array)
				{
					list.Add(Global.SafeConvertToDouble(str));
				}
			}
			lock (this.ConfigMutex)
			{
				this.AlchemyPropList = list;
			}
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("AlchemyRandomLimit");
			if (!string.IsNullOrEmpty(paramValueByName3))
			{
				this.RandomLimit = Global.SafeConvertToInt32(paramValueByName3);
			}
			return this.LoadAlchemyConfigFile();
		}

		public bool LoadAlchemyConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/CurrencyConversion.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/CurrencyConversion.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, AlchemyConfigData> dictionary = new Dictionary<int, AlchemyConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					AlchemyConfigData alchemyConfigData = new AlchemyConfigData();
					alchemyConfigData.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					alchemyConfigData.TypeID = (int)Global.GetSafeAttributeLong(xml, "Type");
					alchemyConfigData.Unit = (int)Global.GetSafeAttributeLong(xml, "Unit");
					alchemyConfigData.Element = (int)Global.GetSafeAttributeLong(xml, "Element");
					alchemyConfigData.Limit = (int)Global.GetSafeAttributeLong(xml, "Limit");
					dictionary[alchemyConfigData.TypeID] = alchemyConfigData;
				}
				lock (this.ConfigMutex)
				{
					this.AlchemyConfig = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/CurrencyConversion.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string Alchemy_FileName = "Config/CurrencyConversion.xml";

		private const string Alchemy_SystemParamKey_Level = "AlchemyLevelUp";

		private const string Alchemy_SystemParamKey_Prop = "AlchemyProperty";

		private const string Alchemy_SystemParamKey_Limit = "AlchemyRandomLimit";

		private object ConfigMutex = new object();

		private int LevelUpElement = 0;

		private List<double> AlchemyPropList = null;

		private int RandomLimit = 0;

		private int MinGoodsID = 100;

		private Dictionary<int, AlchemyConfigData> AlchemyConfig = new Dictionary<int, AlchemyConfigData>();

		private static AlchemyManager instance = new AlchemyManager();
	}
}
