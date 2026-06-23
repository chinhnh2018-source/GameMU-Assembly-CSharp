using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class LingDiCaiJiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		public static LingDiCaiJiManager getInstance()
		{
			return LingDiCaiJiManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool LoadConfig()
		{
			this.LoadCollectXml();
			this.InitDefaultXml();
			this.InitMap();
			this.InitShouWei();
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.DoubleOpenState.Clear();
				this.CaiJiRunTimeData.DoubleOpenState.Add(false);
				this.CaiJiRunTimeData.DoubleOpenState.Add(false);
			}
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1826, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1827, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1829, 2, 2, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1831, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1832, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1833, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1834, 3, 3, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1835, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1836, 2, 2, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!this.IsGongNengOpened())
			{
				result = false;
			}
			else
			{
				switch (nID)
				{
				case 1826:
					return this.ProcesDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1827:
					return this.ProcessMainDataCmd(client, nID, bytes, cmdParams);
				case 1829:
					return this.ProcessLingDiEnterCmd(client, nID, bytes, cmdParams);
				case 1831:
					return this.ProcessLingZhuGetDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1832:
					return this.ProcessLingZhuSetDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1833:
					return this.ProcessLingDiGetShouWeiCmd(client, nID, bytes, cmdParams);
				case 1834:
					return this.ProcessLingDiSetShouWeiCmd(client, nID, bytes, cmdParams);
				case 1835:
					return this.ProcessLingDiGetAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1836:
					return this.ProcessLingDiAdmireCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
		}

		public void processEvent(EventObjectEx eventObject)
		{
		}

		public void NotifyJunTuanRequest(LingDiData lingDi, int eventType)
		{
			switch (eventType)
			{
			case 28:
				if (!this.SyncLingDi(lingDi))
				{
					lock (this.CaiJiRunTimeData.Mutex)
					{
						this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType] = lingDi;
					}
				}
				break;
			case 29:
				this.UpdateDoubleOpenTime(lingDi);
				break;
			}
		}

		public bool ProcesDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				DateTime cmdData = DateTime.MinValue;
				cmdData = this.GetDoubleOpenTime();
				client.sendCmd<DateTime>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessMainDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				LingDiCaiJiMainData lingDiCaiJiMainData = new LingDiCaiJiMainData();
				List<LingDiCaiJiData> list = new List<LingDiCaiJiData>();
				List<LingDiData> list2 = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						list = null;
					}
					else
					{
						list2 = this.CaiJiRunTimeData.LingDiDataList;
						foreach (LingDiData lingDiData in list2)
						{
							list.Add(new LingDiCaiJiData
							{
								LingDiType = lingDiData.LingDiType,
								BeginTime = lingDiData.BeginTime,
								EndTime = lingDiData.EndTime,
								HaveJunTuan = (lingDiData.RoleId > 0),
								ZhanLingName = lingDiData.JunTuanName
							});
						}
					}
					lingDiCaiJiMainData.LingDiCaiJiDataList = list;
					int num2 = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
					if (num2 < 0)
					{
						num2 = 0;
					}
					lingDiCaiJiMainData.LingDiCaiJiLeftCount = num2;
				}
				client.sendCmd<LingDiCaiJiMainData>(nID, lingDiCaiJiMainData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingDiEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleId = Convert.ToInt32(cmdParams[0]);
				int num = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.ChangeLifeCount < this.ChangeLifeLimit || (client.ClientData.ChangeLifeCount == this.ChangeLifeLimit && client.ClientData.Level < this.LevelLimit))
				{
					client.sendCmd<int>(nID, -19, false);
					return true;
				}
				long num2 = TimeUtil.NOW();
				int num3 = -10;
				lock (this.DataMutex)
				{
					if (this.NextCheckNumTicks > num2)
					{
						client.sendCmd<int>(nID, num3, false);
						return true;
					}
				}
				num3 = JunTuanClient.getInstance().CanEnterKuaFuMap(roleId, num);
				client.sendCmd<int>(nID, (num3 > 0) ? 1 : num3, false);
				if (num3 > 0)
				{
					string text;
					int num4;
					string text2;
					int num5;
					string serverIp;
					int serverPort;
					if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(num3, out text, out num4, out text2, out num5, out serverIp, out serverPort))
					{
						LogManager.WriteLog(2, string.Format("领地采集被分配到服务器ServerId={0}, 但是找不到该跨服活动服务器", num3), null, true);
						return true;
					}
					client.ClientSocket.ClientKuaFuServerLoginData.RoleId = roleId;
					client.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)(num + 1);
					client.ClientSocket.ClientKuaFuServerLoginData.GameType = 22;
					client.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
					client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
					client.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
					client.ClientSocket.ClientKuaFuServerLoginData.ServerIp = serverIp;
					client.ClientSocket.ClientKuaFuServerLoginData.ServerPort = serverPort;
					GlobalNew.RecordSwitchKuaFuServerLog(client);
					client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
				}
				else
				{
					lock (this.DataMutex)
					{
						this.NextCheckNumTicks = num2 + 5020L;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingZhuGetDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				LingDiData lingDiData = new LingDiData();
				LingZhuOpenData cmdData = new LingZhuOpenData
				{
					BeginTime = LingDiCaiJiManager.OpenTime,
					EndTime = LingDiCaiJiManager.CloseTime,
					OpenType = this.CanOpenDouble(client, out lingDiData),
					DoubleOpenEndTime = lingDiData.EndTime,
					LeftCount = LingDiCaiJiManager.OpenCountWeekly - lingDiData.OpenCount
				};
				client.sendCmd<LingZhuOpenData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessLingZhuSetDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 设置双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingDiGetShouWeiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				LingZhuShouWeiData shouWeiData = this.GetShouWeiData(client);
				client.sendCmd<LingZhuShouWeiData>(nID, shouWeiData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingDiSetShouWeiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
				int num4 = -1;
				DateTime dateTime = TimeUtil.NowDateTime();
				LingDiData lingDiData = new LingDiData();
				num4 = this.CanSetShouWei(client, num2, dateTime, num3);
				if (num4 != 1)
				{
					client.sendCmd(nID, num4 + ":" + num2, false);
					return true;
				}
				int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
				int junTuanPointCost = (num3 > 0) ? 0 : this.FanRongCost;
				num4 = JunTuanClient.getInstance().SetShouWeiTime(client.ClientData.RoleID, client.ClientData.Faction, lingDiType, dateTime, num2, junTuanPointCost);
				if (num4 == 1)
				{
					int subMoney = 0;
					if (num3 > 0)
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							int num5 = (int)(this.CaiJiRunTimeData.LingDiDataList[this.GetLingDiType(client.ClientData.MapCode)].ShouWeiList[num2].FreeBuShuTime - dateTime).TotalSeconds;
							if (num5 > this.FuHuoSeconds)
							{
								num5 = this.FuHuoSeconds;
							}
							subMoney = this.ZuanShiCost * num5 / this.FuHuoSeconds + 1;
						}
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, "复活守卫_钻石", true, true, false, DaiBiSySType.None))
						{
							num4 = -13;
							client.sendCmd(nID, num4 + ":" + num2, false);
							return true;
						}
					}
					LingDiShouWeiMonsterItem lingDiShouWeiMonsterItem = new LingDiShouWeiMonsterItem();
					lock (this.CaiJiRunTimeData.Mutex)
					{
						this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[num2].State = 2;
						this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[num2].FreeBuShuTime = DateTime.MaxValue;
						lingDiShouWeiMonsterItem = this.CaiJiRunTimeData.ShouWeiQueue[LingDiCaiJiManager.MapCode[lingDiType]][num2];
					}
					GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[lingDiType], lingDiShouWeiMonsterItem.Code, -1, 1, lingDiShouWeiMonsterItem.PosX / 100, lingDiShouWeiMonsterItem.PosY / 100, 0, 0, 43, lingDiShouWeiMonsterItem, null);
				}
				else
				{
					if (num4 == -1030)
					{
						num4 = -14;
					}
					num4 = -2;
				}
				client.sendCmd(nID, num4 + ":" + num2, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingDiGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
					LingDiLingZhuShowData lingDiLingZhuShowData = new LingDiLingZhuShowData();
					if (lingDiType == 0)
					{
						lingDiLingZhuShowData.AdmireCount = Global.GetRoleParamsInt32FromDB(client, "10161");
					}
					else
					{
						lingDiLingZhuShowData.AdmireCount = Global.GetRoleParamsInt32FromDB(client, "10164");
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						lingDiLingZhuShowData.RoleData4Selector = (this.CaiJiRunTimeData.KuaFuSyncNeed ? null : this.CaiJiRunTimeData.LingZhuRoleDataList[lingDiType]);
					}
					client.sendCmd<LingDiLingZhuShowData>(nID, lingDiLingZhuShowData, false);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public bool ProcessLingDiAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
				int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
				if (lingDiType == 2)
				{
					num3 = -3;
				}
				else
				{
					int key = (lingDiType == 0) ? 4 : 5;
					int offsetDayNow = TimeUtil.GetOffsetDayNow();
					MoBaiData moBaiData = null;
					if (!Data.MoBaiDataInfoList.TryGetValue(key, out moBaiData))
					{
						num3 = -3;
					}
					else if (client.ClientData.ChangeLifeCount < moBaiData.MinZhuanSheng || (client.ClientData.ChangeLifeCount == moBaiData.MinZhuanSheng && client.ClientData.Level < moBaiData.MinLevel))
					{
						num3 = -19;
					}
					else
					{
						int num4 = moBaiData.AdrationMaxLimit;
						lock (this.CaiJiRunTimeData.Mutex)
						{
							if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId == num)
							{
								num4 += moBaiData.ExtraNumber;
							}
						}
						string roleParamsKey;
						if (lingDiType == 0)
						{
							roleParamsKey = "10161";
						}
						else
						{
							roleParamsKey = "10164";
						}
						int num5 = Global.GetRoleParamsInt32FromDB(client, roleParamsKey);
						if (num5 >= num4)
						{
							num3 = -16;
						}
						else if (num2 == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < moBaiData.NeedJinBi)
						{
							num3 = -9;
						}
						else if (num2 == 2 && client.ClientData.UserMoney < moBaiData.NeedZuanShi)
						{
							num3 = -10;
						}
						else
						{
							double num6 = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
							if (num2 == 1)
							{
								if (!Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜军团领主" + lingDiType))
								{
									num3 = -9;
									goto IL_4B8;
								}
								long num7 = (long)(num6 * (double)moBaiData.JinBiExpAward);
								if (num7 > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, num7, true, true, false, "none");
								}
								if (moBaiData.JinBiZhanGongAward > 0)
								{
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.JunTuanLingZhuMoBai, 0);
								}
								if (moBaiData.LingJingAwardByJinBi > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜军团领主" + lingDiType, true, true, false);
								}
								if (moBaiData.ShenLiJingHuaByJinBi > 0)
								{
									GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByJinBi, "膜拜军团领主" + lingDiType, true, true);
								}
							}
							if (num2 == 2)
							{
								GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜军团领主" + lingDiType, true, true, false, DaiBiSySType.None);
								int num8 = (int)(num6 * (double)moBaiData.ZuanShiExpAward);
								if (num8 > 0)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, (long)num8, true, true, false, "none");
								}
								if (moBaiData.ZuanShiZhanGongAward > 0)
								{
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.JunTuanLingZhuMoBai, 0);
								}
								if (moBaiData.LingJingAwardByZuanShi > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜军团领主" + lingDiType, true, true, false);
								}
								if (moBaiData.ShenLiJingHuaByZuanShi > 0)
								{
									GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByZuanShi, "膜拜军团领主" + lingDiType, true, true);
								}
							}
							num5++;
							Global.SaveRoleParamsInt64ValueToDB(client, roleParamsKey, (long)num5, true);
						}
					}
				}
				IL_4B8:
				client.sendCmd(nID, num3.ToString(), false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 雕像膜拜信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		public void LoadCollectXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(LingDiCaiJiConsts.CollectMonster);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					this.CollectMonsterXml.Clear();
					foreach (XElement xml in enumerable)
					{
						int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MonsterID", "0"));
						this.CollectMonsterXml[num] = new ManorCollectMonster
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0")),
							MonsterID = num,
							Type = (CryStealType)Convert.ToInt32(Global.GetDefAttributeStr(xml, "Type", "0")),
							Name = Global.GetDefAttributeStr(xml, "Name", ""),
							GatherTime = Convert.ToInt32(Global.GetDefAttributeStr(xml, "GatherTime", "0")),
							FuHuoTime = Convert.ToInt32(Global.GetDefAttributeStr(xml, "FuHuoTime", "0")),
							ShenLiJingHua = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ShenLiJingHua", "0")),
							YuanSuFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xml, "YuanSuFenMo", "0")),
							YingShiFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xml, "YingShiFenMo", "0")),
							LangHunFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xml, "LangHunFenMo", "0"))
						};
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", text, ex.Message), ex, true);
			}
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			LingDiCrystalMonsterItem lingDiCrystalMonsterItem = (monster != null) ? (monster.Tag as LingDiCrystalMonsterItem) : null;
			int result;
			if (lingDiCrystalMonsterItem == null)
			{
				result = -200;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10158");
				if (roleParamsInt32FromDB >= LingDiCaiJiManager.WeeklyNum)
				{
					result = -5;
				}
				else
				{
					int num = this.CollectMonsterXml[lingDiCrystalMonsterItem.Code].GatherTime;
					BufferData bufferDataByID = Global.GetBufferDataByID(client.ClientData, 115);
					if (null != bufferDataByID)
					{
						num *= 100 - (int)bufferDataByID.BufferVal;
						num /= 100;
					}
					result = ((num > 1) ? num : 1);
				}
			}
			return result;
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			LingDiCrystalMonsterItem lingDiCrystalMonsterItem = (monster != null) ? (monster.Tag as LingDiCrystalMonsterItem) : null;
			if (null != lingDiCrystalMonsterItem)
			{
				try
				{
					ManorCollectMonster manorCollectMonster = this.CollectMonsterXml[lingDiCrystalMonsterItem.Code];
					int num = 1;
					DateTime t = TimeUtil.NowDateTime();
					int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
					if (lingDiType != 2)
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							if (t > this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime && t < this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime)
							{
								num = LingDiCaiJiManager.BeiLv;
							}
							if (client.ClientData.JunTuanId != 0 && client.ClientData.JunTuanId == this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId)
							{
								DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "10182");
								DateTime lastStartTime = KarenBattleManager.getInstance().GetLastStartTime(this.ConvertCaiJiLingDiTypeToMapCode(lingDiType));
								if (roleParamsDateTimeFromDB < lastStartTime)
								{
									num += LingDiCaiJiManager.ZhanLingBeiLv;
								}
							}
						}
						int num2 = manorCollectMonster.ShenLiJingHua * num;
						int num3 = manorCollectMonster.YuanSuFenMo * num;
						int num4 = manorCollectMonster.YingShiFenMo * num;
						int num5 = manorCollectMonster.LangHunFenMo * num;
						if (num2 > 0)
						{
							GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, num2, "领地采集_神力精华", true, true);
						}
						if (num3 > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, num3, "领地采集获得元素粉末", true, false);
						}
						if (num4 > 0)
						{
							GameManager.FluorescentGemMgr.AddFluorescentPoint(client, num4, "领地采集获得荧光粉末", true);
						}
						if (num5 > 0)
						{
							GameManager.ClientMgr.ModifyLangHunFenMoValue(client, num5, "领地采集活动狼魂粉末", true, true);
						}
						client.ClientData.LingDiCaiJiNum++;
						Global.SaveRoleParamsInt32ValueToDB(client, "10158", client.ClientData.LingDiCaiJiNum, true);
						int num6 = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
						if (num6 < 0)
						{
							num6 = 0;
						}
						client.sendCmd(1828, num6.ToString(), false);
						int mapCode = client.ClientData.MapCode;
						long key = TimeUtil.NOW() + (long)(manorCollectMonster.FuHuoTime * 1000);
						lock (this.CaiJiRunTimeData.Mutex)
						{
							SortedList<long, List<object>> sortedList = null;
							List<object> list = null;
							if (manorCollectMonster.Type == CryStealType.Chao)
							{
								if (!this.CaiJiRunTimeData.ChaoShuiJingQueue.TryGetValue(mapCode, out sortedList) || null == sortedList)
								{
									sortedList = new SortedList<long, List<object>>();
									this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode] = sortedList;
								}
							}
							else if (!this.CaiJiRunTimeData.NormalShuiJingQueue.TryGetValue(mapCode, out sortedList) || null == sortedList)
							{
								sortedList = new SortedList<long, List<object>>();
								this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode] = sortedList;
							}
							if (!sortedList.TryGetValue(key, out list) || null == list)
							{
								list = new List<object>();
								sortedList.Add(key, list);
							}
							list.Add(lingDiCrystalMonsterItem);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 发放采集奖励信息错误 ex:{0}", ex.Message), null, true);
				}
			}
		}

		public bool IsOpposition(GameClient me, int monsterType)
		{
			bool result;
			if (me.ClientData.HideGM > 0)
			{
				result = false;
			}
			else
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						result = false;
					}
					else
					{
						int lingDiType = this.GetLingDiType(me.ClientData.MapCode);
						result = (me.ClientData.JunTuanId != this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId);
					}
				}
			}
			return result;
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 2101 || monster.MonsterType == 2102)
			{
				LingDiShouWeiMonsterItem lingDiShouWeiMonsterItem = monster.Tag as LingDiShouWeiMonsterItem;
				if (null != lingDiShouWeiMonsterItem)
				{
					if (monster.HandledDead)
					{
						lingDiShouWeiMonsterItem.ShouWeiData.State = 1;
						lingDiShouWeiMonsterItem.ShouWeiData.FreeBuShuTime = TimeUtil.NowDateTime().AddSeconds((double)this.FuHuoSeconds);
						int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
						lock (this.CaiJiRunTimeData.Mutex)
						{
							this.CaiJiRunTimeData.ShouWeiQueue[client.ClientData.MapCode][lingDiShouWeiMonsterItem.ID - 1] = lingDiShouWeiMonsterItem;
							this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[lingDiShouWeiMonsterItem.ID - 1] = lingDiShouWeiMonsterItem.ShouWeiData;
							JunTuanClient.getInstance().SetShouWei(lingDiType, this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList);
						}
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "领地守卫", "守卫被击杀", client.ClientData.MapCode + ":" + lingDiShouWeiMonsterItem.ID, client.ClientData.RoleName, "被击杀", 1, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
					}
				}
			}
		}

		public DateTime GetDoubleOpenTime()
		{
			DateTime result = DateTime.MinValue;
			DateTime t = TimeUtil.NowDateTime();
			List<LingDiData> list = new List<LingDiData>();
			lock (this.CaiJiRunTimeData.Mutex)
			{
				list = this.CaiJiRunTimeData.LingDiDataList;
				if (list == null || list.Count < 2)
				{
					result = DateTime.MinValue;
				}
				else if (list[0].EndTime.DayOfYear != DateTime.Today.DayOfYear)
				{
					result = ((list[1].EndTime.DayOfYear == DateTime.Today.DayOfYear && t > list[1].BeginTime) ? list[1].EndTime : DateTime.MinValue);
				}
				else if (list[1].EndTime.DayOfYear != DateTime.Today.DayOfYear)
				{
					result = ((list[0].EndTime.DayOfYear == DateTime.Today.DayOfYear && t > list[0].BeginTime) ? list[0].EndTime : DateTime.MinValue);
				}
				else if (t < list[0].BeginTime)
				{
					result = ((t > list[1].BeginTime) ? list[1].EndTime : DateTime.MinValue);
				}
				else if (t < list[1].BeginTime)
				{
					result = ((t > list[0].BeginTime) ? list[0].EndTime : DateTime.MinValue);
				}
				else
				{
					result = ((list[0].EndTime < list[1].EndTime) ? list[1].EndTime : list[0].EndTime);
				}
			}
			return result;
		}

		public LingZhuShouWeiData GetShouWeiData(GameClient client)
		{
			LingZhuShouWeiData lingZhuShouWeiData = new LingZhuShouWeiData
			{
				Result = -8,
				ShouWeiList = new List<LingDiShouWeiData>()
			};
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			LingZhuShouWeiData result;
			if (lingDiType == 2)
			{
				result = new LingZhuShouWeiData
				{
					Result = -9
				};
			}
			else
			{
				List<LingDiData> list = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return new LingZhuShouWeiData
						{
							Result = -1
						};
					}
					list = this.CaiJiRunTimeData.LingDiDataList;
					if (list.Count < 2 || null == list[lingDiType])
					{
						return new LingZhuShouWeiData
						{
							Result = -8
						};
					}
					if (client.ClientData.RoleID != list[lingDiType].RoleId)
					{
						return new LingZhuShouWeiData
						{
							Result = -2
						};
					}
					List<LingDiShouWei> shouWeiList = list[lingDiType].ShouWeiList;
					lingZhuShouWeiData.Result = 1;
					DateTime dateTime = TimeUtil.NowDateTime();
					foreach (LingDiShouWei lingDiShouWei in shouWeiList)
					{
						int zuanShiCost = 0;
						if (lingDiShouWei.FreeBuShuTime > dateTime)
						{
							int num = (int)(lingDiShouWei.FreeBuShuTime - dateTime).TotalSeconds;
							if (num > this.FuHuoSeconds)
							{
								num = this.FuHuoSeconds;
							}
							zuanShiCost = this.ZuanShiCost * num / this.FuHuoSeconds + 1;
						}
						LingDiShouWeiData item = new LingDiShouWeiData
						{
							State = lingDiShouWei.State,
							FreeBuShuTime = lingDiShouWei.FreeBuShuTime,
							ZuanShiCost = zuanShiCost
						};
						lingZhuShouWeiData.ShouWeiList.Add(item);
					}
				}
				result = lingZhuShouWeiData;
			}
			return result;
		}

		public void UpdateDoubleOpenTime(LingDiData openItem)
		{
			string text = "";
			try
			{
				DateTime t = TimeUtil.NowDateTime();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.LingDiDataList[openItem.LingDiType].EndTime = openItem.EndTime;
					this.CaiJiRunTimeData.LingDiDataList[openItem.LingDiType].BeginTime = openItem.BeginTime;
					if (t < openItem.BeginTime || t > openItem.EndTime)
					{
						return;
					}
					if (openItem.LingDiType == 0)
					{
						text = GLang.GetLang(2612, new object[0]);
					}
					else if (openItem.LingDiType == 1)
					{
						text = GLang.GetLang(2613, new object[0]);
					}
				}
				if (text != "")
				{
					int num = 0;
					GameClient nextClient;
					while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, text, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0);
						if (this.GetLingDiType(nextClient.ClientData.MapCode) == openItem.LingDiType)
						{
							nextClient.sendCmd<DateTime>(1830, openItem.EndTime, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		public void OnLoadDynamicMonsters(int mapCode, Monster monster)
		{
			LingDiShouWeiMonsterItem lingDiShouWeiMonsterItem = null;
			if (monster != null && (lingDiShouWeiMonsterItem = (monster.Tag as LingDiShouWeiMonsterItem)) != null)
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.ShouWeiMonster[mapCode][lingDiShouWeiMonsterItem.ID] = monster;
				}
			}
		}

		public int SetLingZhu(int lingDiType, int rid, int junTuanId, string junTuanName, RoleData4Selector client)
		{
			int result;
			try
			{
				int zhiWu = 0;
				if (rid != 0 && junTuanId != 0)
				{
					zhiWu = 1;
				}
				byte[] roledata = DataHelper.ObjectToBytes<RoleData4Selector>(client);
				int num = JunTuanClient.getInstance().SetLingZhu(rid, lingDiType, junTuanId, junTuanName, zhiWu, roledata);
				if (num <= 0)
				{
					result = num;
				}
				else
				{
					num = JunTuanClient.getInstance().UpdateJunTuanLingDi(junTuanId, lingDiType + 1);
					if (num <= 0)
					{
						result = num;
					}
					else
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							this.CaiJiRunTimeData.KuaFuSyncNeed = true;
						}
						result = num;
					}
				}
			}
			catch
			{
				result = 0;
			}
			return result;
		}

		public void InitRoleLingDiCaiJiData(GameClient client, bool isNewDay)
		{
			client.ClientData.LingDiCaiJiNum = Global.GetRoleParamsInt32FromDB(client, "10158");
			DateTime t = TimeUtil.NowDateTime();
			DateTime doubleOpenTime = this.GetDoubleOpenTime();
			if (t < doubleOpenTime)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					5,
					2,
					GLang.GetLang(2614, new object[0]),
					0
				});
				client.SendCmdAfterStartPlayGame(194, cmdData);
			}
			if (isNewDay)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10161", 0, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10164", 0, true);
				string roleParamByName = Global.GetRoleParamByName(client, "RoleLoginRecorde");
				string[] array = (roleParamByName == null) ? null : roleParamByName.Split(new char[]
				{
					','
				});
				int num;
				if (array != null && array.Length == 2)
				{
					num = Convert.ToInt32(array[0]);
				}
				else
				{
					num = 0;
				}
				int offsetDayNow = Global.GetOffsetDayNow();
				int num2 = (int)((TimeUtil.NowDateTime().DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)7) : TimeUtil.NowDateTime().DayOfWeek);
				if (num2 - offsetDayNow + num < 1)
				{
					if (client.ClientData.LingDiCaiJiNum > 0)
					{
						client.ClientData.LingDiCaiJiNum = 0;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "10158", client.ClientData.LingDiCaiJiNum, true);
					SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					if (mapSceneType == 43)
					{
						client.sendCmd(1828, (LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum).ToString(), false);
					}
				}
			}
		}

		public void CleanKuaFuData()
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.LingDiDataList.Clear();
				this.CaiJiRunTimeData.KuaFuSyncNeed = true;
			}
		}

		public bool SyncKuaFuData()
		{
			try
			{
				List<LingDiData> lingDiData = JunTuanClient.getInstance().GetLingDiData();
				if (lingDiData == null || lingDiData.Count < 2)
				{
					return true;
				}
				for (int i = 0; i < lingDiData.Count; i++)
				{
					LingDiData lingDiData2 = lingDiData[i];
					if (this.SyncLingDi(lingDiData2))
					{
						return true;
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						if (this.CaiJiRunTimeData.LingDiDataList == null || this.CaiJiRunTimeData.LingDiDataList.Count < 2)
						{
							this.CaiJiRunTimeData.LingDiDataList = lingDiData;
						}
						this.CaiJiRunTimeData.LingDiDataList[lingDiData2.LingDiType] = lingDiData2;
					}
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.KuaFuSyncNeed = false;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 同步中心信息错误 ex:{0}", ex.Message), null, true);
			}
			return true;
		}

		public bool SyncLingDi(LingDiData lingDi)
		{
			try
			{
				if (lingDi == null)
				{
					return true;
				}
				RoleData4Selector roleData4Selector = (lingDi.RoleData == null) ? null : DataHelper.BytesToObject<RoleData4Selector>(lingDi.RoleData, 0, lingDi.RoleData.Length);
				try
				{
					NPC npc = NPCGeneralManager.FindNPC(LingDiCaiJiManager.MapCode[lingDi.LingDiType], FakeRoleNpcId.JunTuanLingZhu + lingDi.LingDiType);
					if (null != npc)
					{
						npc.ShowNpc = true;
						GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
						FakeRoleTypes fakeRoleType = (lingDi.LingDiType == 0) ? FakeRoleTypes.LingDiDiGongLingZhu : FakeRoleTypes.LingDiHuangMoLingZhu;
						FakeRoleManager.ProcessDelFakeRoleByType(fakeRoleType, true);
						if (roleData4Selector != null)
						{
							roleData4Selector.BuffFashionID = ((lingDi.LingDiType == 0) ? 10010 : 10008);
							npc.ShowNpc = false;
							FakeRoleManager.ProcessNewFakeRole(roleData4Selector, LingDiCaiJiManager.MapCode[lingDi.LingDiType], fakeRoleType, (int)npc.CurrentDir, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.JunTuanLingZhu + lingDi.LingDiType);
						}
						else
						{
							if (lingDi.JunTuanId > 0)
							{
								RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, lingDi.RoleId), 0);
								if (roleDataEx != null && roleDataEx.RoleID > 0)
								{
									RoleData4Selector client = Global.RoleDataEx2RoleData4Selector(roleDataEx);
									LingDiCaiJiManager.getInstance().SetLingZhu(lingDi.LingDiType, roleDataEx.RoleID, lingDi.JunTuanId, lingDi.JunTuanName, client);
								}
								return true;
							}
							GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
						}
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						if (this.CaiJiRunTimeData.LingZhuRoleDataList.Count < 2)
						{
							this.CaiJiRunTimeData.LingZhuRoleDataList = new List<RoleData4Selector>();
							this.CaiJiRunTimeData.LingZhuRoleDataList.Add(null);
							this.CaiJiRunTimeData.LingZhuRoleDataList.Add(null);
						}
						this.CaiJiRunTimeData.LingZhuRoleDataList[lingDi.LingDiType] = roleData4Selector;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 设置领主雕像信息错误 ex:{0}", ex.Message), null, true);
					return true;
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					List<LingDiShouWeiMonsterItem> list = new List<LingDiShouWeiMonsterItem>();
					if (!this.CaiJiRunTimeData.ShouWeiQueue.TryGetValue(LingDiCaiJiManager.MapCode[lingDi.LingDiType], out list))
					{
						LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 守卫配置错误 lingDiType:{0}, mapcode:{1}", lingDi.LingDiType, LingDiCaiJiManager.MapCode[lingDi.LingDiType]), null, true);
					}
					for (int i = 0; i < lingDi.ShouWeiList.Count; i++)
					{
						list[i].ShouWeiData = lingDi.ShouWeiList[i];
						Dictionary<int, Monster> dictionary = this.CaiJiRunTimeData.ShouWeiMonster[LingDiCaiJiManager.MapCode[lingDi.LingDiType]];
						Monster obj = null;
						if (dictionary.TryGetValue(list[i].ID, out obj))
						{
							this.CaiJiRunTimeData.ShouWeiMonster[LingDiCaiJiManager.MapCode[lingDi.LingDiType]].Remove(list[i].ID);
							GameManager.MonsterMgr.DeadMonsterImmediately(obj);
						}
						if (lingDi.ShouWeiList[i].State == 2 && lingDi.RoleId > 0)
						{
							Monster monster = GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[lingDi.LingDiType], list[i].Code, -1, 1, list[i].PosX / 100, list[i].PosY / 100, 0, 0, 43, list[i], null);
						}
					}
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (lingDi.RoleId <= 0)
					{
						DateTime t = TimeUtil.NowDateTime();
						foreach (DoubleOpenItem doubleOpenItem in this.DoubleOpenTimeDefaultList)
						{
							if (t.DayOfWeek == (DayOfWeek)doubleOpenItem.WeekDay)
							{
								if (t > DateTime.Today.AddTicks(doubleOpenItem.DayStartTicks) && t < DateTime.Today.AddTicks(doubleOpenItem.DayEndTicks))
								{
									lingDi.BeginTime = DateTime.Today.AddTicks(doubleOpenItem.DayStartTicks);
									lingDi.EndTime = DateTime.Today.AddTicks(doubleOpenItem.DayEndTicks);
								}
								break;
							}
						}
					}
				}
				int num = 0;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count > lingDi.LingDiType)
					{
						if (this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType] != null)
						{
							num = this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType].RoleId;
						}
					}
				}
				if (num == lingDi.RoleId)
				{
					return false;
				}
				int num2 = 0;
				BufferItemTypes bufferItemTypes = BufferItemTypes.DiGongLingZhu;
				BufferItemTypes bufferItemTypes2 = BufferItemTypes.DiGongChenMin;
				if (lingDi.LingDiType == 1)
				{
					bufferItemTypes = BufferItemTypes.HuangMoLingZhu;
					bufferItemTypes2 = BufferItemTypes.HuangMoChenMin;
				}
				GameClient nextClient;
				while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num2, false)) != null)
				{
					if (this.GetLingDiType(nextClient.ClientData.MapCode) == lingDi.LingDiType && nextClient.ClientData.JunTuanId == lingDi.JunTuanId && lingDi.JunTuanId != 0)
					{
						Global.UpdateBufferData(nextClient, BufferItemTypes.JunTuanCaiJiBuff, this.BuffParam, 1, false);
						if (num == lingDi.RoleId)
						{
							continue;
						}
						if (nextClient.ClientData.RoleID == lingDi.RoleId)
						{
							nextClient.sendCmd(1837, "1", false);
						}
					}
					else
					{
						if (this.GetLingDiType(nextClient.ClientData.MapCode) == lingDi.LingDiType && nextClient.ClientData.RoleID == num)
						{
							nextClient.sendCmd(1837, "0", false);
						}
						Global.RemoveBufferData(nextClient, 115);
					}
					if (nextClient.ClientData.JunTuanId == lingDi.JunTuanId && lingDi.JunTuanId != 0)
					{
						if (nextClient.ClientData.RoleID == lingDi.RoleId)
						{
							Global.UpdateBufferData(nextClient, bufferItemTypes, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(nextClient, bufferItemTypes2, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else if (nextClient.ClientData.RoleID == num)
					{
						GameClient client2 = nextClient;
						BufferItemTypes bufferItemType = bufferItemTypes;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client2, bufferItemType, actionParams, 1, false);
					}
					else
					{
						GameClient client3 = nextClient;
						BufferItemTypes bufferItemType2 = bufferItemTypes2;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client3, bufferItemType2, actionParams, 1, false);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 同步中心信息错误 ex:{0}", ex.Message), null, true);
			}
			return true;
		}

		private bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(13);
		}

		public void TimerProc()
		{
			if (this.IsGongNengOpened())
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				long num = TimeUtil.NOW();
				if (this.NextSyncTicks < num)
				{
					if (JunTuanClient.getInstance().GetClientCacheItems(GameCoreInterface.getinstance().GetLocalServerId()))
					{
						this.SetSync();
					}
				}
				bool flag = true;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					flag = this.CaiJiRunTimeData.KuaFuSyncNeed;
				}
				lock (this.DataMutex)
				{
					if (flag && this.NextSyncTicks < num)
					{
						this.NextSyncTicks = num + 5020L;
						if (this.SyncKuaFuData())
						{
							return;
						}
					}
				}
				try
				{
					lock (this.DataMutex)
					{
						if (this.NextHeartBeatTicks > num)
						{
							return;
						}
						this.NextHeartBeatTicks = num + 1020L;
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							int num2 = LingDiCaiJiManager.MapCode[i];
							while (this.CaiJiRunTimeData.NormalShuiJingQueue[num2].Count > 0)
							{
								KeyValuePair<long, List<object>> keyValuePair = this.CaiJiRunTimeData.NormalShuiJingQueue[num2].First<KeyValuePair<long, List<object>>>();
								if (num < keyValuePair.Key)
								{
									break;
								}
								try
								{
									foreach (object obj2 in keyValuePair.Value)
									{
										if (obj2 is LingDiCrystalMonsterItem)
										{
											LingDiCrystalMonsterItem lingDiCrystalMonsterItem = obj2 as LingDiCrystalMonsterItem;
											if (this.CollectMonsterXml.ContainsKey(lingDiCrystalMonsterItem.Code))
											{
												GameManager.MonsterZoneMgr.AddDynamicMonsters(num2, lingDiCrystalMonsterItem.Code, -1, 1, lingDiCrystalMonsterItem.PosX / 100, lingDiCrystalMonsterItem.PosY / 100, 0, 0, 43, lingDiCrystalMonsterItem, null);
											}
										}
									}
								}
								finally
								{
									this.CaiJiRunTimeData.NormalShuiJingQueue[num2].RemoveAt(0);
								}
							}
							if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count != 0)
							{
								int lingDiType = this.GetLingDiType(num2);
								if (!(dateTime < this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime) && !(dateTime > this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime))
								{
									while (this.CaiJiRunTimeData.ChaoShuiJingQueue[num2].Count > 0)
									{
										KeyValuePair<long, List<object>> keyValuePair = this.CaiJiRunTimeData.ChaoShuiJingQueue[num2].First<KeyValuePair<long, List<object>>>();
										if (num < keyValuePair.Key)
										{
											break;
										}
										try
										{
											foreach (object obj2 in keyValuePair.Value)
											{
												if (obj2 is LingDiCrystalMonsterItem)
												{
													LingDiCrystalMonsterItem lingDiCrystalMonsterItem = obj2 as LingDiCrystalMonsterItem;
													if (this.CollectMonsterXml.ContainsKey(lingDiCrystalMonsterItem.Code))
													{
														GameManager.MonsterZoneMgr.AddDynamicMonsters(num2, lingDiCrystalMonsterItem.Code, -1, 1, lingDiCrystalMonsterItem.PosX / 100, lingDiCrystalMonsterItem.PosY / 100, 0, 0, 43, lingDiCrystalMonsterItem, null);
													}
												}
											}
										}
										finally
										{
											this.CaiJiRunTimeData.ChaoShuiJingQueue[num2].RemoveAt(0);
										}
									}
								}
							}
						}
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count != 0)
							{
								int lingDiType = this.GetLingDiType(LingDiCaiJiManager.MapCode[i]);
								foreach (DoubleOpenItem doubleOpenItem in this.DoubleOpenTimeDefaultList)
								{
									if (dateTime.DayOfWeek == (DayOfWeek)doubleOpenItem.WeekDay)
									{
										if (!this.CaiJiRunTimeData.DoubleOpenState[lingDiType])
										{
											if (dateTime > DateTime.Today.AddTicks(doubleOpenItem.DayStartTicks) && dateTime < DateTime.Today.AddTicks(doubleOpenItem.DayEndTicks))
											{
												this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime = DateTime.Today.AddTicks(doubleOpenItem.DayStartTicks);
												this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime = DateTime.Today.AddTicks(doubleOpenItem.DayEndTicks);
												this.UpdateDoubleOpenTime(this.CaiJiRunTimeData.LingDiDataList[lingDiType]);
												this.CaiJiRunTimeData.DoubleOpenState[lingDiType] = true;
											}
										}
										else if (dateTime > DateTime.Today.AddTicks(doubleOpenItem.DayEndTicks))
										{
											this.CaiJiRunTimeData.DoubleOpenState[lingDiType] = false;
										}
										break;
									}
								}
								if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId > 0)
								{
									bool flag6 = false;
									List<LingDiShouWeiMonsterItem> list = new List<LingDiShouWeiMonsterItem>();
									if (!this.CaiJiRunTimeData.ShouWeiQueue.TryGetValue(LingDiCaiJiManager.MapCode[i], out list))
									{
										return;
									}
									for (int j = 0; j < list.Count; j++)
									{
										if (list[j].ShouWeiData.State == 1 && list[j].ShouWeiData.FreeBuShuTime < dateTime)
										{
											flag6 = true;
											list[j].ShouWeiData.State = 2;
											this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[j] = list[j].ShouWeiData;
											Monster monster = GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[i], list[j].Code, -1, 1, list[j].PosX / 100, list[j].PosY / 100, 0, 0, 43, list[j], null);
										}
									}
									if (flag6)
									{
										JunTuanClient.getInstance().SetShouWei(lingDiType, this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList);
									}
								}
							}
						}
						lock (this.DataMutex)
						{
							if (this.NextSyncRoleNumTicks > num)
							{
								return;
							}
							this.NextSyncRoleNumTicks = num + 5020L;
						}
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							int mapClientsCount = GameManager.ClientMgr.GetMapClientsCount(LingDiCaiJiManager.MapCode[i]);
							int num3 = JunTuanClient.getInstance().UpdateMapRoleNum(this.GetLingDiType(LingDiCaiJiManager.MapCode[i]), mapClientsCount, GameCoreInterface.getinstance().GetLocalServerId());
							if (num3 > 0 && num3 != GameCoreInterface.getinstance().GetLocalServerId())
							{
								this.NextSyncRoleNumTicks = num + 180000L;
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("LingDiCaiJi :: 刷怪错误ex:{0}", ex.Message), null, true);
				}
			}
		}

		public int GetLingDiType(int mapCode)
		{
			for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
			{
				if (mapCode == LingDiCaiJiManager.MapCode[i])
				{
					return i;
				}
			}
			return 2;
		}

		public int CanOpenDouble(GameClient client, out LingDiData lingDi)
		{
			lingDi = new LingDiData
			{
				EndTime = DateTime.MinValue,
				OpenCount = int.MaxValue
			};
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			int result;
			if (lingDiType == 2)
			{
				result = -9;
			}
			else
			{
				List<LingDiData> list = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						result = -1;
					}
					else
					{
						list = this.CaiJiRunTimeData.LingDiDataList;
						if (list.Count < 2 || null == list[lingDiType])
						{
							result = -8;
						}
						else
						{
							lingDi = list[lingDiType];
							if (lingDi.RoleId != client.ClientData.RoleID || lingDi.JunTuanId == 0)
							{
								result = -2;
							}
							else
							{
								DateTime t = TimeUtil.NowDateTime();
								if (t > lingDi.BeginTime && t < lingDi.EndTime)
								{
									result = -4;
								}
								else if (LingDiCaiJiManager.OpenCountWeekly - lingDi.OpenCount < 1)
								{
									result = -5;
								}
								else
								{
									result = 1;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public int CanSetShouWei(GameClient client, int index, DateTime now, int useZhuanShi)
		{
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			int result;
			if (lingDiType == 2)
			{
				result = -9;
			}
			else
			{
				List<LingDiData> list = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return -1;
					}
					list = this.CaiJiRunTimeData.LingDiDataList;
					if (list.Count < 2)
					{
						return -1;
					}
					LingDiData lingDiData = list[lingDiType];
					if (lingDiData.RoleId != client.ClientData.RoleID)
					{
						return -2;
					}
					if (lingDiData.ShouWeiList.Count < index + 1)
					{
						return -11;
					}
					if (lingDiData.ShouWeiList[index].State == 2)
					{
						return -15;
					}
					if (lingDiData.ShouWeiList[index].FreeBuShuTime > now)
					{
						int num = (int)(lingDiData.ShouWeiList[index].FreeBuShuTime - now).TotalSeconds;
						if (num > this.FuHuoSeconds)
						{
							num = this.FuHuoSeconds;
						}
						int num2 = this.ZuanShiCost * num / this.FuHuoSeconds + 1;
						if (useZhuanShi <= 0)
						{
							return -12;
						}
						if (client.ClientData.UserMoney < num2)
						{
							return -13;
						}
					}
					if (useZhuanShi > 0)
					{
						if (lingDiData.ShouWeiList[index].State == 0)
						{
							return -16;
						}
						return 1;
					}
					else
					{
						if (this.FanRongCost <= 0)
						{
							return 1;
						}
						int junTuanPoint = JunTuanClient.getInstance().GetJunTuanPoint(client.ClientData.Faction, client.ClientData.JunTuanId);
						if (junTuanPoint < 0)
						{
							if (junTuanPoint == -11000)
							{
								return -1;
							}
							return -2;
						}
						else if (junTuanPoint < this.FanRongCost)
						{
							return -14;
						}
					}
				}
				result = 1;
			}
			return result;
		}

		public bool isLingZhu(int rid)
		{
			bool result;
			lock (this.CaiJiRunTimeData.Mutex)
			{
				if (this.CaiJiRunTimeData.LingDiDataList == null)
				{
					result = false;
				}
				else
				{
					foreach (LingDiData lingDiData in this.CaiJiRunTimeData.LingDiDataList)
					{
						if (lingDiData.RoleId == rid)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public void OnLogin(GameClient client)
		{
			this.UpdateChengHaoBuff(client);
		}

		public void UpdateChengHaoBuff(GameClient client)
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				if (!this.CaiJiRunTimeData.KuaFuSyncNeed)
				{
					int junTuanId = client.ClientData.JunTuanId;
					if (junTuanId == this.CaiJiRunTimeData.LingDiDataList[0].JunTuanId && junTuanId > 0)
					{
						if (client.ClientData.RoleID == this.CaiJiRunTimeData.LingDiDataList[0].RoleId)
						{
							Global.UpdateBufferData(client, BufferItemTypes.DiGongLingZhu, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(client, BufferItemTypes.DiGongChenMin, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else
					{
						BufferItemTypes bufferItemType = BufferItemTypes.DiGongLingZhu;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType, actionParams, 1, false);
						BufferItemTypes bufferItemType2 = BufferItemTypes.DiGongChenMin;
						actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType2, actionParams, 1, false);
					}
					if (junTuanId == this.CaiJiRunTimeData.LingDiDataList[1].JunTuanId && junTuanId > 0)
					{
						if (client.ClientData.RoleID == this.CaiJiRunTimeData.LingDiDataList[1].RoleId)
						{
							Global.UpdateBufferData(client, BufferItemTypes.HuangMoLingZhu, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(client, BufferItemTypes.HuangMoChenMin, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else
					{
						BufferItemTypes bufferItemType3 = BufferItemTypes.HuangMoLingZhu;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType3, actionParams, 1, false);
						BufferItemTypes bufferItemType4 = BufferItemTypes.HuangMoChenMin;
						actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType4, actionParams, 1, false);
					}
				}
			}
		}

		private int ConvertCaiJiLingDiTypeToMapCode(int lingDiType)
		{
			int result;
			if (lingDiType == 0)
			{
				result = 83003;
			}
			else if (lingDiType == 1)
			{
				result = 83002;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int GetLingDiRoleNum(int lingDiType)
		{
			return JunTuanClient.getInstance().GetLingDiRoleNum(lingDiType);
		}

		public void SetSync()
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.KuaFuSyncNeed = true;
			}
		}

		public bool KuaFuInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			client.ClientData.MapCode = LingDiCaiJiManager.MapCode[Convert.ToInt32(clientKuaFuServerLoginData.GameId) - 1];
			int posX = 0;
			int posY = 0;
			bool result;
			if (!this.GetBirthPoint(client.ClientData.MapCode, out posX, out posY))
			{
				LogManager.WriteLog(2, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
				result = false;
			}
			else
			{
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				result = true;
			}
			return result;
		}

		public void InitDefaultXml()
		{
			LingDiCaiJiManager.MapCode = new int[2];
			LingDiCaiJiManager.MapCode[0] = 83000;
			LingDiCaiJiManager.MapCode[1] = 83001;
			try
			{
				LingDiCaiJiManager.WeeklyNum = (int)GameManager.systemParamsList.GetParamValueIntByName("ManorCollectNum", -1);
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ManorCollectDoubleAward");
				string[] array = paramValueByName.Split(new char[]
				{
					','
				});
				if (array.Length < 4)
				{
					LogManager.WriteLog(2, string.Format("LingDICaiJiManager 获取跨服领地特权配置出错:: ManorCollectDoubleAward", new object[0]), null, true);
				}
				else
				{
					LingDiCaiJiManager.OpenTime = DateTime.Parse(array[0]).TimeOfDay;
					LingDiCaiJiManager.CloseTime = DateTime.Parse(array[1]).TimeOfDay;
					LingDiCaiJiManager.OpenSeconds = Convert.ToInt32(array[2]);
					LingDiCaiJiManager.BeiLv = Convert.ToInt32(array[3]);
					LingDiCaiJiManager.OpenCountWeekly = Convert.ToInt32(array[4]);
					string[] array2 = GameManager.systemParamsList.GetParamValueByName("GuardCost").Split(new char[]
					{
						','
					});
					this.FanRongCost = Convert.ToInt32(array2[0]);
					this.ZuanShiCost = Convert.ToInt32(array2[1]);
					this.FuHuoSeconds = Convert.ToInt32(array2[2]);
					string[] array3 = GameManager.systemParamsList.GetParamValueByName("ManorMinLevel").Split(new char[]
					{
						','
					});
					this.ChangeLifeLimit = Convert.ToInt32(array3[0]);
					this.LevelLimit = Convert.ToInt32(array3[1]);
					string[] array4 = GameManager.systemParamsList.GetParamValueByName("ManorBuffID").Split(new char[]
					{
						','
					});
					this.BuffParam[0] = Convert.ToDouble(array4[0]);
					this.BuffParam[1] = Convert.ToDouble(array4[1]);
					string[] array5 = GameManager.systemParamsList.GetParamValueByName("ManorCollectDoubleAwardDefault").Split(new char[]
					{
						',',
						'|'
					});
					this.DoubleOpenTimeDefaultList.Clear();
					this.DoubleOpenTimeDefaultList.Add(new DoubleOpenItem
					{
						WeekDay = Convert.ToInt32(array5[2]),
						DayStartTicks = DateTime.Parse(array5[0]).TimeOfDay.Ticks,
						DayEndTicks = DateTime.Parse(array5[1]).TimeOfDay.Ticks
					});
					this.DoubleOpenTimeDefaultList.Add(new DoubleOpenItem
					{
						WeekDay = Convert.ToInt32(array5[5]),
						DayStartTicks = DateTime.Parse(array5[3]).TimeOfDay.Ticks,
						DayEndTicks = DateTime.Parse(array5[4]).TimeOfDay.Ticks
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDICaiJiClient 获取领地特权配置出错:: ex:{0}", ex.Message), null, true);
			}
		}

		public void InitMap()
		{
			try
			{
				foreach (int num in LingDiCaiJiManager.MapCode)
				{
					string uri = string.Format("Config/ManorCrystal/{0}/CrystalMonster_{0}.xml", num);
					XElement xelement = CheckHelper.LoadXml(Global.GameResPath(uri), true);
					if (null == xelement)
					{
						break;
					}
					IEnumerable<XElement> enumerable = Global.GetSafeXElement(xelement, "Monsters").Elements();
					List<object> list = new List<object>();
					List<object> list2 = new List<object>();
					foreach (XElement xml in enumerable)
					{
						LingDiCrystalMonsterItem lingDiCrystalMonsterItem = new LingDiCrystalMonsterItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0")),
							Code = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Code", "0")),
							PosX = Convert.ToInt32(Global.GetDefAttributeStr(xml, "X", "0")),
							PosY = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Y", "0"))
						};
						if (this.CollectMonsterXml.ContainsKey(lingDiCrystalMonsterItem.Code))
						{
							if (this.CollectMonsterXml[lingDiCrystalMonsterItem.Code].Type == CryStealType.Chao)
							{
								list2.Add(lingDiCrystalMonsterItem);
							}
							else
							{
								GameManager.MonsterZoneMgr.AddDynamicMonsters(num, lingDiCrystalMonsterItem.Code, -1, 1, lingDiCrystalMonsterItem.PosX / 100, lingDiCrystalMonsterItem.PosY / 100, 0, 0, 43, lingDiCrystalMonsterItem, null);
							}
						}
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						SortedList<long, List<object>> sortedList = null;
						if (!this.CaiJiRunTimeData.ChaoShuiJingQueue.TryGetValue(num, out sortedList) || null == sortedList)
						{
							sortedList = new SortedList<long, List<object>>();
							this.CaiJiRunTimeData.ChaoShuiJingQueue[num] = sortedList;
						}
						this.CaiJiRunTimeData.ChaoShuiJingQueue[num].Add(TimeUtil.NOW(), list2);
						SortedList<long, List<object>> sortedList2 = null;
						if (!this.CaiJiRunTimeData.NormalShuiJingQueue.TryGetValue(num, out sortedList2) || null == sortedList2)
						{
							sortedList2 = new SortedList<long, List<object>>();
							this.CaiJiRunTimeData.NormalShuiJingQueue[num] = sortedList2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJiManager_Copy :: 初始化刷怪异常：{0}", ex.Message.ToString()), null, true);
			}
		}

		public void InitShouWei()
		{
			try
			{
				Dictionary<int, List<LingDiShouWeiMonsterItem>> dictionary = new Dictionary<int, List<LingDiShouWeiMonsterItem>>();
				Dictionary<int, Dictionary<int, Monster>> dictionary2 = new Dictionary<int, Dictionary<int, Monster>>();
				foreach (int num in LingDiCaiJiManager.MapCode)
				{
					string uri = string.Format("Config/ManorCrystal/{0}/ShouWeiMonster.xml", num);
					XElement xelement = CheckHelper.LoadXml(Global.GameResPath(uri), true);
					if (null == xelement)
					{
						return;
					}
					IEnumerable<XElement> enumerable = Global.GetSafeXElement(xelement, "Monsters").Elements();
					List<LingDiShouWeiMonsterItem> list = new List<LingDiShouWeiMonsterItem>();
					foreach (XElement xml in enumerable)
					{
						LingDiShouWeiMonsterItem item = new LingDiShouWeiMonsterItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0")),
							Code = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Code", "0")),
							PosX = Convert.ToInt32(Global.GetDefAttributeStr(xml, "X", "0")),
							PosY = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Y", "0")),
							ShouWeiData = new LingDiShouWei
							{
								State = 0,
								FreeBuShuTime = DateTime.MinValue
							}
						};
						list.Add(item);
					}
					dictionary[num] = list;
					Dictionary<int, Monster> value = new Dictionary<int, Monster>();
					dictionary2[num] = value;
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.ShouWeiQueue = dictionary;
					this.CaiJiRunTimeData.ShouWeiMonster = dictionary2;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("LingDiCaiJiManager_Copy :: 初始化刷怪异常：{0}", ex.Message.ToString()), null, true);
			}
		}

		private bool GetBirthPoint(int mapCode, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("LingDICaiJiManager 获取跨服地图配置出错:: GetBirthPoint", new object[0]), null, true);
				result = false;
			}
			else
			{
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, gameMap.DefaultBirthPosX, gameMap.DefaultBirthPosY, gameMap.BirthRadius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		public void NotifyPlayGame(GameClient client)
		{
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			if (lingDiType != 2)
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return;
					}
					if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId == client.ClientData.JunTuanId && client.ClientData.JunTuanId != 0)
					{
						Global.UpdateBufferData(client, BufferItemTypes.JunTuanCaiJiBuff, this.BuffParam, 1, false);
					}
					string cmdData = (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId == client.ClientData.RoleID) ? "1" : "0";
					client.sendCmd(1837, cmdData, false);
				}
				int num = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
				if (num < 0)
				{
					num = 0;
				}
				client.sendCmd(1828, num.ToString(), false);
				DateTime cmdData2 = DateTime.MinValue;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					cmdData2 = this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime;
				}
				client.sendCmd<DateTime>(1830, cmdData2, false);
			}
		}

		public void OnLeaveFuBen(GameClient client)
		{
			Global.RemoveBufferData(client, 115);
		}

		public LingDiCaiJiRunData CaiJiRunTimeData = new LingDiCaiJiRunData();

		public Dictionary<int, ManorCollectMonster> CollectMonsterXml = new Dictionary<int, ManorCollectMonster>();

		private static LingDiCaiJiManager instance = new LingDiCaiJiManager();

		private object DataMutex = new object();

		private long NextHeartBeatTicks = 0L;

		private long NextSyncTicks = 0L;

		private long NextCheckNumTicks = 0L;

		private long NextSyncRoleNumTicks = 0L;

		public static int[] MapCode;

		public static int WeeklyNum = 0;

		public static int OpenSeconds = 0;

		public static int BeiLv = 1;

		public static int ZhanLingBeiLv = 1;

		public static int OpenCountWeekly = 0;

		public static TimeSpan OpenTime = DateTime.MaxValue.TimeOfDay;

		public static TimeSpan CloseTime = DateTime.MinValue.TimeOfDay;

		public List<DoubleOpenItem> DoubleOpenTimeDefaultList = new List<DoubleOpenItem>();

		public int FanRongCost = int.MaxValue;

		public int ZuanShiCost = int.MaxValue;

		public int FuHuoSeconds = int.MaxValue;

		public int ChangeLifeLimit = int.MaxValue;

		public int LevelLimit = int.MaxValue;

		public double[] BuffParam = new double[2];
	}
}
