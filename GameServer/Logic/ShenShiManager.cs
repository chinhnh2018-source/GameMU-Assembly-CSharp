using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ShenShiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static ShenShiManager getInstance()
		{
			return ShenShiManager.instance;
		}

		public bool initialize()
		{
			this.LoadFuWenHoleXml();
			this.LoadFuWenXml();
			this.LoadFuWenGodXml();
			this.LoadFuWenRandomXml();
			this.LoadHuoDongFuWenRandomXml();
			this.LoadDefaultXml();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1870, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1871, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1872, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1873, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1874, 4, 4, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1875, 4, 4, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1876, 3, 3, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1877, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1878, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1879, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1880, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1881, 3, 3, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1883, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1870:
					return this.ProcessShenShiMainInfoCmd(client, nID, bytes, cmdParams);
				case 1871:
					return this.ProcessGetFuWenListCmd(client, nID, bytes, cmdParams);
				case 1872:
					return this.ProcessGetFuWenTabListCmd(client, nID, bytes, cmdParams);
				case 1873:
					return this.ProcessGetShenShiListCmd(client, nID, bytes, cmdParams);
				case 1874:
					return this.ProcessModFuWenCmd(client, nID, bytes, cmdParams);
				case 1875:
					return this.ProcessModShenShiCmd(client, nID, bytes, cmdParams);
				case 1876:
					return this.ProcessModSkillCmd(client, nID, bytes, cmdParams);
				case 1877:
					return this.ProcessFuWenChouQuCmd(client, nID, bytes, cmdParams);
				case 1878:
					return this.ProcessFuWenZhiZuoCmd(client, nID, bytes, cmdParams);
				case 1879:
					return this.ProcessFuWenFenJieCmd(client, nID, bytes, cmdParams);
				case 1880:
					return this.ProcessModFuWenTabCmd(client, nID, bytes, cmdParams);
				case 1881:
					return this.ProcessModFuWenTabNameCmd(client, nID, bytes, cmdParams);
				case 1883:
					return this.ProcessFuWenTabBuyCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public bool ProcessShenShiMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				ShenShiMainData cmdData = new ShenShiMainData
				{
					FuWenTabId = Global.GetRoleParamsInt32FromDB(client, "10185"),
					NextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10186")
				};
				client.sendCmd<ShenShiMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取主页面信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetFuWenTabListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<List<FuWenTabData>>(nID, client.ClientData.FuWenTabList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取符文页错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetFuWenListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.FuWenGoodsDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取符文背包信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessGetShenShiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "38");
				client.sendCmd<List<int>>(nID, roleParamsIntListFromDB, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取神识列表错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessModFuWenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				int num4 = Convert.ToInt32(cmdParams[3]);
				int num5 = 0;
				string text = "";
				if (client.ClientData.FuWenTabList == null || num2 >= client.ClientData.FuWenTabList.Count || num2 < 0 || num3 < 0 || num3 >= 24)
				{
					num5 = -1;
				}
				else
				{
					FuWenHoleItem fuWenHole = this.GetFuWenHole(num3 + 1);
					if (null == fuWenHole)
					{
						num5 = -5;
					}
					else if (fuWenHole.OpenLevel > Global.GetUnionLevel(client, false))
					{
						num5 = -4;
					}
					else
					{
						if (num4 > 0)
						{
							GoodsData fuWenGoodsDataByGoodsID = ShenShiManager.GetFuWenGoodsDataByGoodsID(client, num4);
							if (fuWenGoodsDataByGoodsID == null)
							{
								num5 = -3;
								goto IL_274;
							}
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(fuWenGoodsDataByGoodsID.GoodsID, out systemXmlItem))
							{
								num5 = -5;
								goto IL_274;
							}
							FuWenItem fuWenItem = null;
							if (fuWenGoodsDataByGoodsID == null || !this.ShenShiRunTimeData.FuWenDict.TryGetValue(fuWenGoodsDataByGoodsID.GoodsID, out fuWenItem))
							{
								num5 = -5;
								goto IL_274;
							}
							if (!this.CheckIsFuWenByGoodsID(fuWenGoodsDataByGoodsID.GoodsID))
							{
								num5 = -7;
								goto IL_274;
							}
							if (fuWenItem.Type != this.GetFuWenHole(num3 + 1).Type)
							{
								num5 = -6;
								goto IL_274;
							}
							int tabEquipFuWenNum = this.GetTabEquipFuWenNum(client, num2, fuWenGoodsDataByGoodsID.GoodsID);
							if (tabEquipFuWenNum >= ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, fuWenGoodsDataByGoodsID.GoodsID))
							{
								num5 = -8;
								goto IL_274;
							}
						}
						else
						{
							num4 = 0;
						}
						client.ClientData.FuWenTabList[num2].FuWenEquipList[num3] = num4;
						this.CheckShenShiProps(client, num2, false);
						if (Global.GetRoleParamsInt32FromDB(client, "10185") == num2)
						{
							text = this.CheckShenShiList(client, num2);
							this.UpdateFuWenProps(client);
						}
						if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[num2], client.ServerId) < 0)
						{
							num5 = -9;
						}
					}
				}
				IL_274:
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					num5,
					cmdParams[1],
					cmdParams[2],
					cmdParams[3],
					text,
					string.Join<int>(",", client.ClientData.FuWenTabList[num2].ShenShiActiveList)
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 替换符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessModShenShiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				int num4 = Convert.ToInt32(cmdParams[3]);
				int num5 = 0;
				if (client.ClientData.FuWenTabList == null || num2 >= client.ClientData.FuWenTabList.Count || num2 < 0)
				{
					num5 = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					num5 = -19;
				}
				else
				{
					if (num4 > 0)
					{
						List<int> list = this.SelectCanActiveList(this.GetActiveShenShiList(client, client.ClientData.FuWenTabList[num2].FuWenEquipList));
						if (!list.Contains(num3))
						{
							num5 = -10;
							goto IL_2C5;
						}
						FuWenGodItem fuWenGodItem = null;
						if (!this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(num3, out fuWenGodItem))
						{
							num5 = -5;
							goto IL_2C5;
						}
						foreach (int key in client.ClientData.FuWenTabList[num2].ShenShiActiveList)
						{
							FuWenGodItem fuWenGodItem2 = null;
							if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem2))
							{
								if (fuWenGodItem2.Type == fuWenGodItem.Type)
								{
									num5 = -11;
									break;
								}
							}
						}
						if (num5 != 0)
						{
							goto IL_2C5;
						}
						if (client.ClientData.FuWenTabList[num2].ShenShiActiveList.Count >= 3)
						{
							num5 = -12;
							goto IL_2C5;
						}
						client.ClientData.FuWenTabList[num2].ShenShiActiveList.Add(num3);
					}
					else
					{
						if (!client.ClientData.FuWenTabList[num2].ShenShiActiveList.Contains(num3))
						{
							num5 = -13;
							goto IL_2C5;
						}
						client.ClientData.FuWenTabList[num2].ShenShiActiveList.Remove(num3);
					}
					if (Global.GetRoleParamsInt32FromDB(client, "10185") == num2)
					{
						FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[num2];
						client.ClientData.ShenShiEquipData.ShenShiActiveList = fuWenTabData.ShenShiActiveList;
					}
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[num2], client.ServerId) < 0)
					{
						num5 = -9;
					}
				}
				IL_2C5:
				string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num5,
					cmdParams[1],
					cmdParams[2],
					cmdParams[3]
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 更改神识错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessModSkillCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				int num4 = 0;
				if (0 == num3)
				{
					num4 = -22;
				}
				else if (client.ClientData.FuWenTabList == null || num2 >= client.ClientData.FuWenTabList.Count || num2 < 0)
				{
					num4 = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					num4 = -19;
				}
				else
				{
					if (num3 > 0)
					{
						if (!SpriteAttack.CanUseMaigc(client, num3))
						{
							num4 = -14;
							goto IL_1CA;
						}
						if (!this.FuWenMagicList.Contains(num3))
						{
							num4 = -14;
							goto IL_1CA;
						}
						client.ClientData.FuWenTabList[num2].SkillEquip = num3;
					}
					else
					{
						if (client.ClientData.FuWenTabList[num2].SkillEquip <= 0)
						{
							num4 = -15;
							goto IL_1CA;
						}
						client.ClientData.FuWenTabList[num2].SkillEquip = 0;
					}
					if (Global.GetRoleParamsInt32FromDB(client, "10185") == num2)
					{
						FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[num2];
						client.ClientData.ShenShiEquipData.SkillEquip = fuWenTabData.SkillEquip;
					}
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[num2], client.ServerId) < 0)
					{
						num4 = -9;
					}
				}
				IL_1CA:
				string cmdData = string.Format("{0}:{1}:{2}", num4, cmdParams[1], cmdParams[2]);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 替换技能错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessFuWenChouQuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				DateTime t = TimeUtil.NowDateTime();
				FuWenChouQuResult fuWenChouQuResult = new FuWenChouQuResult
				{
					Result = 0,
					ChouQuCount = num2
				};
				int num3 = (num2 == 1) ? 1 : 10;
				if (!ShenShiManager.CanAddGoodsNum(client, num3))
				{
					fuWenChouQuResult.Result = -16;
				}
				else
				{
					List<int> list = new List<int>();
					if (num2 == 1)
					{
						int num4 = 0;
						int randomNumber = Global.GetRandomNumber(1, 100001);
						lock (this.ShenShiRunTimeData.Mutex)
						{
							foreach (FuWenRandomItem fuWenRandomItem in this.GetRandomList(0))
							{
								if (randomNumber >= fuWenRandomItem.BeginNum && randomNumber <= fuWenRandomItem.EndNum)
								{
									if (this.CheckIsFuWenByGoodsID(fuWenRandomItem.GoodsID))
									{
										num4 = fuWenRandomItem.GoodsID;
										break;
									}
								}
							}
						}
						if (num4 == 0)
						{
							fuWenChouQuResult.Result = -5;
							goto IL_553;
						}
						DateTime dateTime = Global.GetRoleParamsDateTimeFromDB(client, "10186");
						if (dateTime < t)
						{
							dateTime = t.AddSeconds((double)this.FuWenFreeTime);
							Global.SaveRoleParamsDateTimeToDB(client, "10186", dateTime, true);
						}
						else if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.FuWenChouQuCost, "神识符文抽取_钻石(改幸运之星)", false, DaiBiSySType.FuWenChouQu))
						{
							fuWenChouQuResult.Result = -17;
							goto IL_553;
						}
						list.Add(num4);
						fuWenChouQuResult.GoodsList = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
						{
							num4,
							1,
							0,
							0,
							0,
							0,
							0
						});
						fuWenChouQuResult.FreeTime = dateTime;
					}
					else
					{
						int num4 = 0;
						for (int i = 0; i < num3 - 1; i++)
						{
							int randomNumber = Global.GetRandomNumber(1, 100001);
							lock (this.ShenShiRunTimeData.Mutex)
							{
								foreach (FuWenRandomItem fuWenRandomItem in this.GetRandomList(0))
								{
									if (randomNumber >= fuWenRandomItem.BeginNum && randomNumber <= fuWenRandomItem.EndNum)
									{
										if (this.CheckIsFuWenByGoodsID(fuWenRandomItem.GoodsID))
										{
											list.Add(fuWenRandomItem.GoodsID);
											break;
										}
									}
								}
							}
						}
						int randomNumber2 = Global.GetRandomNumber(1, 100001);
						lock (this.ShenShiRunTimeData.Mutex)
						{
							foreach (FuWenRandomItem fuWenRandomItem in this.GetRandomList(1))
							{
								if (randomNumber2 >= fuWenRandomItem.BeginNum && randomNumber2 <= fuWenRandomItem.EndNum)
								{
									if (this.CheckIsFuWenByGoodsID(fuWenRandomItem.GoodsID))
									{
										list.Add(fuWenRandomItem.GoodsID);
										break;
									}
								}
							}
						}
						if (list.Count < num3)
						{
							fuWenChouQuResult.Result = -5;
							goto IL_553;
						}
						int value = list[num3 - 1];
						randomNumber2 = Global.GetRandomNumber(0, num3);
						list[num3 - 1] = list[randomNumber2];
						list[randomNumber2] = value;
						if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.FuWenChouQuCost_10, "神识符文抽取10_钻石(改幸运之星)", false, DaiBiSySType.FuWenChouQu))
						{
							fuWenChouQuResult.Result = -17;
							goto IL_553;
						}
						fuWenChouQuResult.GoodsList = string.Join("|", list.ConvertAll<string>((int _gd) => string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
						{
							_gd,
							1,
							0,
							0,
							0,
							0,
							0
						})));
						fuWenChouQuResult.FreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10186");
					}
					foreach (int goodsID in list)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, 0, "", 0, 1, 11000, "", true, 1, "神识符文抽取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
				}
				IL_553:
				client.sendCmd<FuWenChouQuResult>(nID, fuWenChouQuResult, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 抽取符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessFuWenZhiZuoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = 0;
				if (!this.CheckIsFuWenByGoodsID(num2))
				{
					num3 = -7;
				}
				else if (!ShenShiManager.CanAddGoodsNum(client, 1))
				{
					num3 = -16;
				}
				else if (ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, num2) >= 8)
				{
					num3 = -8;
				}
				else
				{
					FuWenItem fuWenItem = null;
					if (!this.ShenShiRunTimeData.FuWenDict.TryGetValue(num2, out fuWenItem))
					{
						num3 = -7;
					}
					else if (fuWenItem.Level >= 7 && GameManager.systemParamsList.GetParamValueIntByName("FuWenSevenOpen", -1) != 1L)
					{
						num3 = -5;
					}
					else if (fuWenItem.PayNum > client.ClientData.FuWenZhiChen)
					{
						num3 = -18;
					}
					else
					{
						GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, -fuWenItem.PayNum, "制作神识符文消耗", true, true, false);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num2, 1, 0, "", 0, 1, 11000, "", true, 1, "神识符文制作", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
				}
				string cmdData = string.Format("{0}:{1}", num3, string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					num2,
					1,
					0,
					0,
					0,
					0,
					0
				}));
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 制作符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessFuWenFenJieCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				List<int> list = Array.ConvertAll<string, int>(cmdParams[1].Split(new char[]
				{
					','
				}), (string x) => Convert.ToInt32(x)).ToList<int>();
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < list.Count; i++)
				{
					int num4 = list[i++];
					if (list.Count == i)
					{
						num2 = -3;
						break;
					}
					int num5 = list[i];
					if (num5 > 0)
					{
						List<GoodsData> fuWenGoodsDataListByGoodsID = this.GetFuWenGoodsDataListByGoodsID(client, num4);
						if (fuWenGoodsDataListByGoodsID == null || fuWenGoodsDataListByGoodsID.Count < 1)
						{
							num2 = -3;
							break;
						}
						if (!this.CheckIsFuWenByGoodsID(num4))
						{
							num2 = -7;
							break;
						}
						FuWenItem fuWenItem = null;
						if (!this.ShenShiRunTimeData.FuWenDict.TryGetValue(num4, out fuWenItem))
						{
							num2 = -7;
							break;
						}
						for (int j = 0; j < fuWenGoodsDataListByGoodsID.Count; j++)
						{
							GoodsData goodsData = fuWenGoodsDataListByGoodsID[j];
							int num6 = (num5 > goodsData.GCount) ? goodsData.GCount : num5;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, num6, false, false))
							{
								num2 = -9;
								break;
							}
							num5 -= num6;
							if (num5 <= 0 || num2 != 0)
							{
								break;
							}
						}
						if (num5 > 0)
						{
							num2 = -3;
							break;
						}
						if (num2 == 0)
						{
							num3 += fuWenItem.SendNum * list[i];
						}
					}
				}
				GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, num3, "分解符文获得", true, true, false);
				for (int k = 0; k < client.ClientData.FuWenTabList.Count; k++)
				{
					this.UpdateFuWenTabList(client, k);
				}
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				client.sendCmd(nID, string.Format("{0}", num2), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 分解符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessModFuWenTabCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
				int num3 = 0;
				string text = "";
				if (client.ClientData.FuWenTabList == null || num2 >= client.ClientData.FuWenTabList.Count || num2 < 0)
				{
					num3 = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					num3 = -19;
				}
				else if (roleParamsInt32FromDB != num2)
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "10185", num2, true);
					text = this.CheckShenShiList(client, num2);
					this.CheckShenShiProps(client, num2, false);
					this.UpdateFuWenProps(client);
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[num2], client.ServerId) < 0)
					{
						num3 = -9;
					}
				}
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num3,
					cmdParams[1],
					text,
					string.Join<int>(",", client.ClientData.FuWenTabList[num2].ShenShiActiveList)
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 更改启用符文页错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessModFuWenTabNameCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				string text = cmdParams[2];
				int num3 = 0;
				if (client.ClientData.FuWenTabList == null || num2 >= client.ClientData.FuWenTabList.Count || num2 < 0)
				{
					num3 = -1;
				}
				else if (string.IsNullOrEmpty(text) || NameServerNamager.CheckInvalidCharacters(text, false) <= 0 || text.Length < 1 || text.Length > 5)
				{
					num3 = -20;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					num3 = -19;
				}
				else
				{
					client.ClientData.FuWenTabList[num2].Name = text;
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[num2], client.ServerId) < 0)
					{
						num3 = -9;
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num3, cmdParams[1], cmdParams[2]), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 更改符文页名称错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessFuWenTabBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int tabID = 0;
				FuWenTabData fuWenTabData = null;
				int count = client.ClientData.FuWenTabList.Count;
				if (count >= this.InitFuWenTabNum + this.FuWenTabBuyCost.Count)
				{
					tabID = -21;
				}
				else
				{
					int num2 = this.FuWenTabBuyCost[count - this.InitFuWenTabNum];
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "神识符文页购买_钻石", true, true, false, DaiBiSySType.None))
					{
						tabID = -17;
					}
					else
					{
						fuWenTabData = ShenShiManager.AddRoleFuWenTab(client.ClientData.RoleID, count, client.ServerId);
						if (fuWenTabData != null)
						{
							fuWenTabData.ShenShiActiveList = new List<int>();
							client.ClientData.FuWenTabList.Add(fuWenTabData);
						}
						else
						{
							tabID = -9;
							GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "神识符文页购买回滚_钻石", ActivityTypes.None, "");
						}
					}
				}
				if (null == fuWenTabData)
				{
					fuWenTabData = new FuWenTabData
					{
						TabID = tabID
					};
				}
				client.sendCmd<FuWenTabData>(nID, fuWenTabData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 处理购买符文页命令。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public void LoadFuWenHoleXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenShiConsts.FuWenHole);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, FuWenHoleItem> dictionary = new Dictionary<int, FuWenHoleItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "HoleID", "0"));
							int[] array = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xelement2, "OpenLevel", "").Split(new char[]
							{
								'|'
							}), (string x) => Convert.ToInt32(x));
							if (array.Length < 2)
							{
								LogManager.WriteLog(2, string.Format("加载xml配置文件:{0}, 错误。", text), null, true);
							}
							else
							{
								dictionary[num] = new FuWenHoleItem
								{
									HoleID = num,
									Type = this.ToType(Global.GetDefAttributeStr(xelement2, "Type", "")),
									OpenLevel = Global.GetUnionLevel(array[0], array[1], false)
								};
							}
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenHoleDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadFuWenXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenShiConsts.FuWen);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, FuWenItem> dictionary = new Dictionary<int, FuWenItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0"));
							dictionary[num] = new FuWenItem
							{
								GoodsId = num,
								Type = this.ToType(Global.GetDefAttributeStr(xelement2, "Type", "0")),
								Level = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Level", "0")),
								Blue = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Blue", "0")),
								Red = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Red", "0")),
								Green = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Green", "0")),
								PayNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "PayNum", "0")),
								SendNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SendNum", "0"))
							};
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadFuWenGodXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenShiConsts.FuWenGod);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, FuWenGodItem> dictionary = new Dictionary<int, FuWenGodItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Type", "0"));
							int num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Level", "0"));
							int num3 = 100 * num + num2;
							dictionary[num3] = new FuWenGodItem
							{
								GodId = num3,
								Type = num,
								Level = num2,
								NeedBlue = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedBlue", "0")),
								NeedRed = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedRed", "0")),
								NeedGreen = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedGreen", "0")),
								MagicItemList = GameManager.SystemMagicActionMgr.ParseActionsInterface(Global.GetDefAttributeStr(xelement2, "ShenShiScript", ""))
							};
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenGodDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadFuWenRandomXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenShiConsts.FuWenRandom);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					List<FuWenRandomItem> list = new List<FuWenRandomItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							list.Add(new FuWenRandomItem
							{
								ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
								GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
							});
						}
					}
					text = Global.GameResPath(ShenShiConsts.FuWenPayRandom);
					xelement = CheckHelper.LoadXml(text, true);
					if (null != xelement)
					{
						List<FuWenRandomItem> list2 = new List<FuWenRandomItem>();
						enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								list2.Add(new FuWenRandomItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
									GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
									BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
									EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
								});
							}
						}
						lock (this.ShenShiRunTimeData.Mutex)
						{
							this.ShenShiRunTimeData.FuWenRandomList = list;
							this.ShenShiRunTimeData.FuWenPayRandomList = list2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void ReloadConfig()
		{
			this.LoadHuoDongFuWenRandomXml();
		}

		public void LoadHuoDongFuWenRandomXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ShenShiConsts.HuoDongFuWenRandom);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					List<FuWenRandomItem> list = new List<FuWenRandomItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							list.Add(new FuWenRandomItem
							{
								ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
								GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
							});
						}
					}
					text = Global.GameResPath(ShenShiConsts.HuoDongFuWenPayRandom);
					xelement = CheckHelper.LoadXml(text, true);
					if (null != xelement)
					{
						List<FuWenRandomItem> list2 = new List<FuWenRandomItem>();
						enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								list2.Add(new FuWenRandomItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
									GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
									BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
									EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
								});
							}
						}
						lock (this.ShenShiRunTimeData.Mutex)
						{
							this.ShenShiRunTimeData.HuoDongFuWenRandomList = list;
							this.ShenShiRunTimeData.HuoDongFuWenPayRandomList = list2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadDefaultXml()
		{
			try
			{
				this.FuWenFreeTime = (int)GameManager.systemParamsList.GetParamValueIntByName("FuWenFreeRandom", -1);
				string[] array = GameManager.systemParamsList.GetParamValueByName("FuWenPay").Split(new char[]
				{
					','
				});
				this.FuWenChouQuCost = Convert.ToInt32(array[0]);
				this.FuWenChouQuCost_10 = Convert.ToInt32(array[1]);
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				foreach (string value in GameManager.systemParamsList.GetParamValueByName("FuWenMagic").Split(new char[]
				{
					','
				}))
				{
					int num = Convert.ToInt32(value);
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(num, out systemXmlItem))
					{
						int intValue = systemXmlItem.GetIntValue("NextMagicID", -1);
						if (intValue > 0)
						{
							dictionary.Add(intValue, num);
						}
						this.FuWenMagicList.Add(num);
					}
				}
				lock (this.ShenShiRunTimeData.Mutex)
				{
					this.ShenShiRunTimeData.ParentMagicCode = dictionary;
				}
				string[] array3 = GameManager.systemParamsList.GetParamValueByName("FuWenList").Split(new char[]
				{
					','
				});
				if (array3.Length < 1)
				{
					LogManager.WriteLog(2, string.Format("没配置符文页数量", new object[0]), null, true);
				}
				else
				{
					this.InitFuWenTabNum = Convert.ToInt32(array3[0]);
					for (int j = 1; j < array3.Length; j++)
					{
						this.FuWenTabBuyCost.Add(Convert.ToInt32(array3[j]));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		public List<FuWenRandomItem> GetRandomList(int type)
		{
			List<FuWenRandomItem> result = this.ShenShiRunTimeData.FuWenRandomList;
			if (HuodongCachingMgr.GetJieriFuLiActivity().IsOpened(EJieRiFuLiType.FuWenKuangHuan))
			{
				if (type == 1)
				{
					result = this.ShenShiRunTimeData.HuoDongFuWenPayRandomList;
				}
				else
				{
					result = this.ShenShiRunTimeData.HuoDongFuWenRandomList;
				}
			}
			else if (type == 1)
			{
				result = this.ShenShiRunTimeData.FuWenPayRandomList;
			}
			else
			{
				result = this.ShenShiRunTimeData.FuWenRandomList;
			}
			return result;
		}

		public static FuWenTabData AddRoleFuWenTab(int rid, int tabID, int serverID)
		{
			FuWenTabData fuWenTabData = new FuWenTabData
			{
				TabID = tabID,
				Name = string.Format(GLang.GetLang(2621, new object[0]), tabID + 1),
				FuWenEquipList = new List<int>(new int[24]),
				SkillEquip = 0,
				OwnerID = rid
			};
			Global.sendToDB<int, FuWenTabData>(20315, fuWenTabData, serverID);
			return fuWenTabData;
		}

		public FuWenHoleItem GetFuWenHole(int fuWenId)
		{
			FuWenHoleItem result = null;
			try
			{
				lock (this.ShenShiRunTimeData.Mutex)
				{
					if (null == this.ShenShiRunTimeData.FuWenHoleDict)
					{
						return null;
					}
					this.ShenShiRunTimeData.FuWenHoleDict.TryGetValue(fuWenId, out result);
					return result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShiManager :: 获取符文页数据, 失败。FuWenID:{0}, ex:{1}", fuWenId, ex.Message), null, true);
			}
			return null;
		}

		public void InitRoleShenShiData(GameClient client)
		{
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 11000), client.ServerId);
			}
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.FuWenTabList)
				{
					client.ClientData.FuWenTabList = new List<FuWenTabData>();
				}
				for (int i = 0; i < client.ClientData.FuWenTabList.Count; i++)
				{
					if (client.ClientData.FuWenTabList[i].ShenShiActiveList == null)
					{
						client.ClientData.FuWenTabList[i].ShenShiActiveList = new List<int>();
					}
				}
				for (int i = client.ClientData.FuWenTabList.Count; i < this.InitFuWenTabNum; i++)
				{
					FuWenTabData fuWenTabData = ShenShiManager.AddRoleFuWenTab(client.ClientData.RoleID, i, client.ServerId);
					if (fuWenTabData == null)
					{
						LogManager.WriteLog(2, string.Format("初始化角色符文数据, 失败。rid:{0} tabID:{1}", client.ClientData.RoleID, i), null, true);
						break;
					}
					fuWenTabData.ShenShiActiveList = new List<int>();
					client.ClientData.FuWenTabList.Add(fuWenTabData);
				}
				if (null == client.ClientData.ShenShiEquipData)
				{
					client.ClientData.ShenShiEquipData = new SkillEquipData
					{
						ShenShiActiveList = new List<int>()
					};
				}
				this.UpdateFuWenTabList(client, Global.GetRoleParamsInt32FromDB(client, "10185"));
				client.ClientData.FuWenZhiChen = Global.GetRoleParamsInt32FromDB(client, "10187");
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
				client.sendCmd<List<GoodsData>>(1871, client.ClientData.FuWenGoodsDataList, false);
				client.sendCmd<List<FuWenTabData>>(1872, client.ClientData.FuWenTabList, false);
			}
		}

		public int ToType(string type)
		{
			if (type != null)
			{
				if (type == "Blue")
				{
					return 1;
				}
				if (type == "Red")
				{
					return 2;
				}
				if (type == "Green")
				{
					return 3;
				}
			}
			LogManager.WriteLog(2, string.Format("ShenShi :: 类型配置错误。type:{0}", type), null, true);
			return 0;
		}

		public static GoodsData AddFuWenGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = 1,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList.Add(goodsData);
			}
			return goodsData;
		}

		public static int GetIdleSlotOfFuWenBagGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.FuWenGoodsDataList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.FuWenGoodsDataList[i].BagIndex);
					}
				}
				for (int j = 0; j < ShenShiManager.GetMaxFuWenCount(); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0;
		}

		public static int GetMaxFuWenCount()
		{
			return int.MaxValue;
		}

		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(15) && GlobalNew.IsGongNengOpened(client, 92, hint);
		}

		public static GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string startTime, string endTime, ref int startIndex)
		{
			GoodsData result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (client.ClientData.FuWenGoodsDataList)
				{
					if (startIndex >= client.ClientData.FuWenGoodsDataList.Count)
					{
						return null;
					}
					for (int i = startIndex; i < client.ClientData.FuWenGoodsDataList.Count; i++)
					{
						GoodsData goodsData = client.ClientData.FuWenGoodsDataList[i];
						if (goodsData.GoodsID == goodsID && goodsData.Binding == bingding && Global.DateTimeEqual(goodsData.Endtime, endTime) && Global.DateTimeEqual(goodsData.Starttime, startTime))
						{
							startIndex = i + 1;
							return goodsData;
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static void UpdateFuWenGoodsData(GameClient client, GoodsData goodsData)
		{
			if (client.ClientData.FuWenGoodsDataList != null && null != goodsData)
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					if (goodsData.GCount == 0)
					{
						client.ClientData.FuWenGoodsDataList.Remove(goodsData);
					}
					else
					{
						for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
						{
							if (client.ClientData.FuWenGoodsDataList[i].GoodsID == goodsData.GoodsID)
							{
								client.ClientData.FuWenGoodsDataList[i] = goodsData;
							}
						}
					}
				}
			}
		}

		public static GoodsData GetFuWenGoodsDataByGoodsID(GameClient client, int goodsID)
		{
			GoodsData result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					result = client.ClientData.FuWenGoodsDataList.Find((GoodsData _g) => _g.GoodsID == goodsID);
				}
			}
			return result;
		}

		public List<GoodsData> GetFuWenGoodsDataListByGoodsID(GameClient client, int goodsID)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = list;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					foreach (GoodsData goodsData in client.ClientData.FuWenGoodsDataList)
					{
						if (goodsData.GoodsID == goodsID)
						{
							list.Add(goodsData);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetFuWenGoodsDataCountByGoodsID(GameClient client, int goodsID)
		{
			int result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = 0;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					int num = 0;
					foreach (GoodsData goodsData in client.ClientData.FuWenGoodsDataList)
					{
						if (goodsData.GoodsID == goodsID)
						{
							num += goodsData.GCount;
						}
					}
					result = num;
				}
			}
			return result;
		}

		public static GoodsData GetFuWenGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				result = null;
			}
			else if (id <= 0)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
					{
						if (client.ClientData.FuWenGoodsDataList[i].Id == id)
						{
							return client.ClientData.FuWenGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public string CheckShenShiList(GameClient client, int tabId)
		{
			string result = "";
			try
			{
				if (tabId != Global.GetRoleParamsInt32FromDB(client, "10185"))
				{
					return ":";
				}
				FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[tabId];
				List<int> list = Global.GetRoleParamsIntListFromDB(client, "38");
				List<int> activeShenShiList = this.GetActiveShenShiList(client, fuWenTabData.FuWenEquipList);
				activeShenShiList.AddRange(list);
				List<int> list2 = this.SelectCanActiveList(activeShenShiList);
				list = list2.Except(list).ToList<int>();
				if (list.Count > 0)
				{
					int num = list2.Count - 24;
					if (num > 0)
					{
						list2.RemoveRange(list2.Count - 1, num);
					}
					Global.SaveRoleParamsIntListToDB(client, list2, "38", true);
					result = string.Join<int>(",", list);
				}
				return result;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 检查是否有可激活神识错误。rid:{0} tabId:{1} ex:{2}", client.ClientData.RoleID, tabId, ex.Message), null, true);
			}
			return result;
		}

		public List<int> GetActiveShenShiList(GameClient client, List<int> fuWenList)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (fuWenList == null || fuWenList.Count < 0)
			{
				result = list;
			}
			else
			{
				try
				{
					int blue = 0;
					int red = 0;
					int green = 0;
					for (int i = 0; i < fuWenList.Count; i++)
					{
						if (fuWenList[i] > 0)
						{
							FuWenItem fuWenItem = null;
							if (this.ShenShiRunTimeData.FuWenDict.TryGetValue(fuWenList[i], out fuWenItem))
							{
								if (fuWenItem.Type == this.GetFuWenHole(i + 1).Type)
								{
									blue += fuWenItem.Blue;
									red += fuWenItem.Red;
									green += fuWenItem.Green;
								}
							}
						}
					}
					list = this.ShenShiRunTimeData.FuWenGodDict.Values.ToList<FuWenGodItem>().FindAll((FuWenGodItem _g) => _g.NeedBlue <= blue && _g.NeedRed <= red && _g.NeedGreen <= green).ConvertAll<int>((FuWenGodItem _g) => _g.GodId);
					return list;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("ShenShi :: 获取可激活神识列错误。 ex:{0}", ex.Message), null, true);
				}
				result = list;
			}
			return result;
		}

		public List<int> SelectCanActiveList(List<int> shenShiList)
		{
			List<int> list = new List<int>();
			Dictionary<int, FuWenGodItem> dictionary = new Dictionary<int, FuWenGodItem>();
			foreach (int key in shenShiList)
			{
				FuWenGodItem fuWenGodItem = null;
				if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem))
				{
					FuWenGodItem fuWenGodItem2 = null;
					if (!dictionary.TryGetValue(fuWenGodItem.Type, out fuWenGodItem2))
					{
						dictionary[fuWenGodItem.Type] = fuWenGodItem;
					}
					else if (fuWenGodItem2.Level < fuWenGodItem.Level)
					{
						dictionary[fuWenGodItem.Type] = fuWenGodItem;
					}
				}
			}
			return dictionary.Values.ToList<FuWenGodItem>().ConvertAll<int>((FuWenGodItem _g) => _g.GodId);
		}

		public int GetTabEquipFuWenNum(GameClient client, int tabID, int goodsID)
		{
			int result;
			if (client.ClientData.FuWenTabList == null || tabID >= client.ClientData.FuWenTabList.Count)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.FuWenTabList[tabID].FuWenEquipList.FindAll((int _g) => _g == goodsID).Count;
			}
			return result;
		}

		public bool CheckIsFuWenByGoodsID(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem) && systemXmlItem.GetIntValue("Categoriy", -1) == 940;
		}

		public void UpdateFuWenTabList(GameClient client, int tabId)
		{
			lock (client.ClientData.FuWenTabList)
			{
				if (tabId < client.ClientData.FuWenTabList.Count)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
					bool flag2 = false;
					for (int i = 0; i < client.ClientData.FuWenTabList[tabId].FuWenEquipList.Count; i++)
					{
						int num = client.ClientData.FuWenTabList[tabId].FuWenEquipList[i];
						if (0 != num)
						{
							int num2 = 0;
							int num3;
							if (dictionary2.ContainsKey(num))
							{
								Dictionary<int, int> dictionary3;
								int key;
								num3 = ((dictionary3 = dictionary2)[key = num] = dictionary3[key] + 1);
							}
							else
							{
								num3 = (dictionary2[num] = 1);
							}
							if (!dictionary.TryGetValue(num, out num2))
							{
								num2 = (dictionary[num] = ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, num));
							}
							if (num2 < num3)
							{
								client.ClientData.FuWenTabList[tabId].FuWenEquipList[i] = 0;
								flag2 = true;
							}
						}
					}
					this.CheckShenShiProps(client, tabId, true);
					if (tabId == Global.GetRoleParamsInt32FromDB(client, "10185"))
					{
						this.UpdateFuWenProps(client);
					}
					if (flag2)
					{
						Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId);
					}
				}
			}
		}

		public int GetCurrentTabTotalLevel(GameClient client)
		{
			int num = 0;
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
			int result;
			if (client.ClientData.FuWenTabList == null || client.ClientData.FuWenTabList.Count <= roleParamsInt32FromDB)
			{
				result = num;
			}
			else
			{
				FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[roleParamsInt32FromDB];
				foreach (int key in fuWenTabData.FuWenEquipList)
				{
					FuWenItem fuWenItem = null;
					if (this.ShenShiRunTimeData.FuWenDict.TryGetValue(key, out fuWenItem))
					{
						num += fuWenItem.Level;
					}
				}
				result = num;
			}
			return result;
		}

		public void UpdateFuWenProps(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > roleParamsInt32FromDB)
			{
				FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[roleParamsInt32FromDB];
				double[] array = new double[177];
				foreach (int num in fuWenTabData.FuWenEquipList)
				{
					if (num > 0)
					{
						GoodsData fuWenGoodsDataByGoodsID = ShenShiManager.GetFuWenGoodsDataByGoodsID(client, num);
						if (null != fuWenGoodsDataByGoodsID)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemGoods))
							{
								EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(num);
								if (null != equipPropItem)
								{
									for (int i = 0; i < 177; i++)
									{
										array[i] += Global.GetEquipExtPropsItemVal(client, fuWenGoodsDataByGoodsID, equipPropItem, i, systemGoods);
									}
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ShenShiFuWen,
					array
				});
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		public void CheckShenShiProps(GameClient client, int tabId, bool writeDB = false)
		{
			try
			{
				if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > tabId)
				{
					FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
					List<int> list = this.SelectCanActiveList(this.GetActiveShenShiList(client, tabData.FuWenEquipList));
					int i;
					for (i = 0; i < tabData.ShenShiActiveList.Count; i++)
					{
						int num = list.Find((int x) => x / 100 == tabData.ShenShiActiveList[i] / 100);
						if (num > 0)
						{
							tabData.ShenShiActiveList[i] = num;
						}
						else
						{
							tabData.ShenShiActiveList.RemoveAt(i--);
						}
					}
					if (tabId == Global.GetRoleParamsInt32FromDB(client, "10185"))
					{
						client.ClientData.ShenShiEquipData.SkillEquip = tabData.SkillEquip;
						client.ClientData.ShenShiEquipData.ShenShiActiveList = tabData.ShenShiActiveList;
					}
					if (writeDB)
					{
						Global.sendToDB<int, FuWenTabData>(20316, tabData, client.ServerId);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 检查神识有效性。rid:{0} tabId:{1} ex:{2}", client.ClientData.RoleID, tabId, ex.Message), null, true);
			}
		}

		public FuWenTabData GetRoleFuWenTabData(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
			FuWenTabData result;
			if (client.ClientData.FuWenTabList == null || client.ClientData.FuWenTabList.Count <= roleParamsInt32FromDB)
			{
				result = null;
			}
			else
			{
				result = client.ClientData.FuWenTabList[roleParamsInt32FromDB];
			}
			return result;
		}

		public List<int> GetAttackerShenShiActiveList(object attacker, ref int rid, ref int magicCode)
		{
			if (this.ShenShiRunTimeData.ParentMagicCode.ContainsKey(magicCode))
			{
				magicCode = this.ShenShiRunTimeData.ParentMagicCode[magicCode];
			}
			List<int> list = new List<int>();
			SkillEquipData shenShiEquipData;
			if (attacker is GameClient)
			{
				shenShiEquipData = (attacker as GameClient).ClientData.ShenShiEquipData;
				rid = (attacker as GameClient).ClientData.RoleID;
			}
			else
			{
				if (!(attacker is Robot))
				{
					return null;
				}
				shenShiEquipData = (attacker as Robot).getRoleDataMini().ShenShiEquipData;
				rid = (attacker as Robot).RoleID;
			}
			List<int> result;
			if (shenShiEquipData == null || shenShiEquipData.SkillEquip != magicCode || shenShiEquipData.ShenShiActiveList == null || shenShiEquipData.ShenShiActiveList.Count < 1)
			{
				result = null;
			}
			else
			{
				result = shenShiEquipData.ShenShiActiveList;
			}
			return result;
		}

		public double GetMagicCodeAddPercent(object attacker, object enemy, int magicCode)
		{
			double num = 0.0;
			int num2 = 0;
			try
			{
				GameClient gameClient = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> attackerShenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref num2, ref magicCode);
				if (attackerShenShiActiveList == null || attackerShenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int key in attackerShenShiActiveList)
				{
					FuWenGodItem fuWenGodItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem))
					{
						foreach (MagicActionItem magicActionItem in fuWenGodItem.MagicItemList)
						{
							double num3 = 0.0;
							double num4 = 0.0;
							if (magicActionItem.MagicActionParams.Length > 0)
							{
								num3 = magicActionItem.MagicActionParams[0];
							}
							if (magicActionItem.MagicActionParams.Length > 1)
							{
								num4 = magicActionItem.MagicActionParams[1];
							}
							switch (magicActionItem.MagicActionID)
							{
							case MagicActionIDs.MU_XINGHONG:
								if (null != gameClient)
								{
									if (Convert.ToInt32((double)gameClient.ClientData.LifeV * num3) >= gameClient.ClientData.CurrentLifeV)
									{
										num += num4;
									}
								}
								else if (null != robot)
								{
									if (robot.MonsterInfo.VLifeMax * num3 >= robot.VLife)
									{
										num += num4;
									}
								}
								break;
							case MagicActionIDs.MU_JUXIONG:
								if (gameClient != null)
								{
									if (Convert.ToInt32((double)gameClient.ClientData.LifeV * num3) <= gameClient.ClientData.CurrentLifeV)
									{
										num += num4;
									}
								}
								else if (robot != null)
								{
									if (robot.MonsterInfo.VLifeMax * num3 <= robot.VLife)
									{
										num += num4;
									}
								}
								break;
							case MagicActionIDs.MU_SUOMING:
								if (enemy is GameClient)
								{
									if (Convert.ToInt32((double)(enemy as GameClient).ClientData.LifeV * num3) >= (enemy as GameClient).ClientData.CurrentLifeV)
									{
										num += num4;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).MonsterInfo.VLifeMax * num3 >= (enemy as Monster).VLife)
									{
										num += num4;
									}
								}
								break;
							case MagicActionIDs.MU_ZHANZHENG:
								if (enemy is GameClient)
								{
									if (Convert.ToInt32((double)(enemy as GameClient).ClientData.LifeV * num3) <= (enemy as GameClient).ClientData.CurrentLifeV)
									{
										num += num4;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).MonsterInfo.VLifeMax * num3 <= (enemy as Monster).VLife)
									{
										num += num4;
									}
								}
								break;
							case MagicActionIDs.MU_SUILU:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).buffManager.IsBuffEnabled(117) || (enemy as GameClient).RoleBuffer.GetExtProp(50) > 0.1)
									{
										num += num3;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterXuanYun())
									{
										num += num3;
									}
								}
								break;
							case MagicActionIDs.MU_JIELV:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).RoleBuffer.GetExtProp(47) > 0.1)
									{
										num += num3;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterDingShen())
									{
										num += num3;
									}
								}
								break;
							case MagicActionIDs.MU_XUELANG:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).buffManager.IsBuffEnabled(118))
									{
										num += num3;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterSpeedDown())
									{
										num += num3;
									}
								}
								break;
							case MagicActionIDs.MU_XIAOYONG:
								if (null != gameClient)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(gameClient.CurrentPos, (enemy as GameClient).CurrentPos) <= num3 * 100.0)
										{
											num += num4;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(gameClient.CurrentPos, (enemy as Monster).CurrentPos) <= num3 * 100.0)
										{
											num += num4;
										}
									}
								}
								else if (null != robot)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as GameClient).CurrentPos) <= num3 * 100.0)
										{
											num += num4;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as Monster).CurrentPos) <= num3 * 100.0)
										{
											num += num4;
										}
									}
								}
								break;
							case MagicActionIDs.MU_MAOYOU:
								if (null != gameClient)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(gameClient.CurrentPos, (enemy as GameClient).CurrentPos) >= num3 * 100.0)
										{
											num += num4;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(gameClient.CurrentPos, (enemy as Monster).CurrentPos) >= num3 * 100.0)
										{
											num += num4;
										}
									}
								}
								else if (null != robot)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as GameClient).CurrentPos) >= num3 * 100.0)
										{
											num += num4;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as Monster).CurrentPos) >= num3 * 100.0)
										{
											num += num4;
										}
									}
								}
								break;
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					num /= 5.0;
				}
				return num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取神识伤害百分比加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", num2, magicCode, ex.Message), null, true);
			}
			return num;
		}

		public double GetMagicCodeAddInjure(object attacker, object enemy, int magicCode)
		{
			double num = 0.0;
			int num2 = 0;
			try
			{
				GameClient gameClient = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> attackerShenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref num2, ref magicCode);
				if (attackerShenShiActiveList == null || attackerShenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int key in attackerShenShiActiveList)
				{
					FuWenGodItem fuWenGodItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem))
					{
						foreach (MagicActionItem magicActionItem in fuWenGodItem.MagicItemList)
						{
							double num3 = 0.0;
							if (magicActionItem.MagicActionParams.Length > 0)
							{
								num3 = magicActionItem.MagicActionParams[0];
							}
							MagicActionIDs magicActionID = magicActionItem.MagicActionID;
							if (magicActionID == MagicActionIDs.MU_LUHUO)
							{
								if (null != gameClient)
								{
									double num4 = (RoleAlgorithm.GetExtProp(gameClient, 4) + RoleAlgorithm.GetExtProp(gameClient, 6)) * 0.5;
									num += num4 * num3;
								}
								else if (null != robot)
								{
									double num4 = (double)(robot.MonsterInfo.Defense + robot.MonsterInfo.MDefense) * 0.5;
									num += num4 * num3;
								}
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					num /= 5.0;
				}
				return num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取神识伤害加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", num2, magicCode, ex.Message), null, true);
			}
			return num;
		}

		public double GetMagicCodeSkillCDSubPercent(GameClient client, int magicCode)
		{
			double num = 0.0;
			try
			{
				if (client.ClientData.CurrentMagicCDSubPercent > 0.0 || this.ShenShiRunTimeData.ParentMagicCode.ContainsKey(magicCode))
				{
					return client.ClientData.CurrentMagicCDSubPercent;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10185");
				if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > roleParamsInt32FromDB)
				{
					FuWenTabData fuWenTabData = client.ClientData.FuWenTabList[roleParamsInt32FromDB];
					if (fuWenTabData.SkillEquip == magicCode && fuWenTabData.ShenShiActiveList != null && fuWenTabData.ShenShiActiveList.Count >= 0)
					{
						foreach (int key in fuWenTabData.ShenShiActiveList)
						{
							FuWenGodItem fuWenGodItem = null;
							if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem))
							{
								foreach (MagicActionItem magicActionItem in fuWenGodItem.MagicItemList)
								{
									double num2 = 0.0;
									double num3 = 0.0;
									if (magicActionItem.MagicActionParams.Length > 0)
									{
										num2 = magicActionItem.MagicActionParams[0];
									}
									if (magicActionItem.MagicActionParams.Length > 1)
									{
										num3 = magicActionItem.MagicActionParams[1];
									}
									MagicActionIDs magicActionID = magicActionItem.MagicActionID;
									if (magicActionID == MagicActionIDs.MU_MENGJING)
									{
										if (Global.GetRandom() <= num2)
										{
											num += num3;
										}
									}
								}
							}
						}
					}
				}
				return (num > 1.0) ? 1.0 : num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取神识CD缩减加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", client.ClientData.RoleID, magicCode, ex.Message), null, true);
			}
			return num;
		}

		public double GetMagicCodeAddPercent2(object attacker, List<object> enemyList, int magicCode)
		{
			double num = 0.0;
			int num2 = 0;
			try
			{
				GameClient gameClient = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> attackerShenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref num2, ref magicCode);
				if (attackerShenShiActiveList == null || attackerShenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int key in attackerShenShiActiveList)
				{
					FuWenGodItem fuWenGodItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(key, out fuWenGodItem))
					{
						foreach (MagicActionItem magicActionItem in fuWenGodItem.MagicItemList)
						{
							double num3 = 0.0;
							double num4 = 0.0;
							if (magicActionItem.MagicActionParams.Length > 0)
							{
								num3 = magicActionItem.MagicActionParams[0];
							}
							if (magicActionItem.MagicActionParams.Length > 1)
							{
								num4 = magicActionItem.MagicActionParams[1];
							}
							MagicActionIDs magicActionID = magicActionItem.MagicActionID;
							if (magicActionID == MagicActionIDs.MU_BAIZHAN)
							{
								if ((double)enemyList.Count >= num3)
								{
									num += num4;
								}
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					num /= 5.0;
				}
				return num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ShenShi :: 获取神识技能伤害百分比加成。rid:{0}, maggicCode:{1}, ex:{2}", num2, magicCode, ex.Message), null, true);
			}
			return num;
		}

		public ShenShiRunData ShenShiRunTimeData = new ShenShiRunData();

		public int FuWenFreeTime = 0;

		public int FuWenChouQuCost = 0;

		public int FuWenChouQuCost_10 = 0;

		public List<int> FuWenMagicList = new List<int>();

		public int InitFuWenTabNum = 0;

		public List<int> FuWenTabBuyCost = new List<int>();

		private static ShenShiManager instance = new ShenShiManager();
	}
}
