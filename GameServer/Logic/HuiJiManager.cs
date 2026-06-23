using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class HuiJiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static HuiJiManager getInstance()
		{
			return HuiJiManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.OnRoleDead(playerDeadEventObject.getPlayer());
				}
			}
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.IsGongNengOpend = false;
					this.RuntimeData.EmblemFull = GameManager.systemParamsList.GetParamValueIntArrayByName("EmblemFull", ',');
					this.RuntimeData.EmblemShengXing = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EmblemShengXing", ',');
					int platformType = GameCoreInterface.getinstance().GetPlatformType();
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("EmblemOpen", '|');
					foreach (string str in paramValueStringListByName)
					{
						List<int> list = Global.StringToIntList(str, ',');
						if (list != null && list[0] == platformType && list[1] > 0)
						{
							this.RuntimeData.IsGongNengOpend = true;
						}
					}
					this.RuntimeData.IsGongNengOpend &= !GameFuncControlManager.IsGameFuncDisabled(14);
					text = "Config/EmblemUp.xml";
					string text2 = Global.GameResPath(text);
					this.RuntimeData.EmblemUpDict.Load(text2, null);
					foreach (EmblemUpInfo emblemUpInfo in this.RuntimeData.EmblemUpDict.Value.Values)
					{
						emblemUpInfo.ExtPropTempValues[24] = emblemUpInfo.SubAttackInjurePercent;
						emblemUpInfo.ExtPropTempValues[102] = emblemUpInfo.SPAttackInjurePercent;
						emblemUpInfo.ExtPropTempValues[103] = emblemUpInfo.AttackInjurePercent;
						emblemUpInfo.ExtPropTempValues[104] = emblemUpInfo.ElementAttackInjurePercent;
					}
					text = "Config/EmblemStar.xml";
					text2 = Global.GameResPath(text);
					this.RuntimeData.EmblemStarDict.Load(text2, null);
					foreach (EmblemStarInfo emblemStarInfo in this.RuntimeData.EmblemStarDict.Value.Values)
					{
						EmblemUpInfo emblemUpInfo2;
						if (this.RuntimeData.EmblemUpDict.Value.TryGetValue(emblemStarInfo.EmblemLevel, out emblemUpInfo2))
						{
							emblemStarInfo.EmblemUpInfo = emblemUpInfo2;
							emblemUpInfo2.MaxStarLevel = Math.Max(emblemUpInfo2.MaxStarLevel, emblemStarInfo.EmblemStar);
							emblemStarInfo.LifeV += emblemUpInfo2.LifeV;
							emblemStarInfo.AddAttack += emblemUpInfo2.AddAttack;
							emblemStarInfo.AddDefense += emblemUpInfo2.AddDefense;
							emblemStarInfo.DecreaseInjureValue += emblemUpInfo2.DecreaseInjureValue;
							emblemStarInfo.ExtPropValues[13] = (double)emblemStarInfo.LifeV;
							emblemStarInfo.ExtPropValues[45] = (double)emblemStarInfo.AddAttack;
							emblemStarInfo.ExtPropValues[46] = (double)emblemStarInfo.AddDefense;
							emblemStarInfo.ExtPropValues[38] = (double)emblemStarInfo.DecreaseInjureValue;
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
			TCPCmdDispatcher.getInstance().registerProcessorEx(1445, 1, 1, HuiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1446, 1, 1, HuiJiManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(10, HuiJiManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, HuiJiManager.getInstance());
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
			switch (nID)
			{
			case 1445:
				result = this.ProcessExecuteHuiJiHuTiCmd(client, nID, bytes, cmdParams);
				break;
			case 1446:
				result = this.ProcessHuiJiStarUpCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return this.RuntimeData.IsGongNengOpend && GlobalNew.IsGongNengOpened(client, 89, false);
		}

		private bool ProcessHuiJiStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			HuiJiUpdateResultData huiJiUpdateResultData = new HuiJiUpdateResultData();
			RoleHuiJiData huiJiData = client.ClientData.HuiJiData;
			int num = 0;
			HuiJiUpdateResultData huiJiUpdateResultData2 = DataHelper.BytesToObject<HuiJiUpdateResultData>(bytes, 0, bytes.Length);
			int type = huiJiUpdateResultData2.Type;
			int zuanShi = huiJiUpdateResultData2.ZuanShi;
			int auto = huiJiUpdateResultData2.Auto;
			long num2 = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					EmblemStarInfo emblemStarInfo;
					EmblemStarInfo emblemStarInfo2;
					if (huiJiData.huiji != huiJiUpdateResultData2.HuiJi)
					{
						result = -3;
					}
					else if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji, out emblemStarInfo))
					{
						result = -3;
					}
					else if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji + 1, out emblemStarInfo2))
					{
						result = -4004;
					}
					else
					{
						bool flag2 = false;
						bool flag3 = false;
						string strCostList;
						if (type == 0)
						{
							if (emblemStarInfo.EmblemUpInfo.MaxStarLevel == emblemStarInfo.EmblemStar)
							{
								result = -4;
								goto IL_646;
							}
							if (Global.UseGoodsBindOrNot(client, emblemStarInfo.NeedGoods[0], emblemStarInfo.NeedGoods[1], true, out flag2, out flag3) < 0)
							{
								if (zuanShi <= 0 || zuanShi != emblemStarInfo.NeedDiamond)
								{
									result = -6;
									goto IL_646;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, zuanShi, "徽记升星", true, true, true, true, DaiBiSySType.HuiJiShengXing))
								{
									result = -10;
									goto IL_646;
								}
								num = emblemStarInfo.ZuanShiExp;
								strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
								{
									-zuanShi,
									client.ClientData.UserMoney + zuanShi,
									client.ClientData.UserMoney
								});
								if (Global.GetRandom() < this.RuntimeData.EmblemShengXing[1])
								{
									num = (int)((double)num * this.RuntimeData.EmblemShengXing[2]);
								}
							}
							else
							{
								num = emblemStarInfo.GoodsExp;
								strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
								{
									GoodsID = emblemStarInfo.NeedGoods[0],
									GCount = emblemStarInfo.NeedGoods[1]
								});
								if (Global.GetRandom() < this.RuntimeData.EmblemShengXing[0])
								{
									num = (int)((double)num * this.RuntimeData.EmblemShengXing[2]);
								}
							}
							huiJiData.Exp += num;
							if (huiJiData.Exp >= emblemStarInfo.StarExp)
							{
								huiJiData.huiji++;
								if (emblemStarInfo.EmblemStar < emblemStarInfo.EmblemUpInfo.MaxStarLevel - 1)
								{
									huiJiData.Exp -= emblemStarInfo.StarExp;
								}
								else
								{
									huiJiData.Exp = 0;
								}
							}
						}
						else
						{
							if (emblemStarInfo.EmblemUpInfo.MaxStarLevel != emblemStarInfo.EmblemStar)
							{
								result = -4;
								goto IL_646;
							}
							if (Global.UseGoodsBindOrNot(client, emblemStarInfo.EmblemUpInfo.NeedGoods[0], emblemStarInfo.EmblemUpInfo.NeedGoods[1], true, out flag2, out flag3) < 0)
							{
								if (zuanShi <= 0 || zuanShi != emblemStarInfo.EmblemUpInfo.NeedDiamond)
								{
									result = -6;
									goto IL_646;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, zuanShi, "徽记升阶", true, true, true, true, DaiBiSySType.HuiJiShengJie))
								{
									result = -10;
									goto IL_646;
								}
								strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
								{
									-zuanShi,
									client.ClientData.UserMoney + zuanShi,
									client.ClientData.UserMoney
								});
							}
							else
							{
								strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
								{
									GoodsID = emblemStarInfo.EmblemUpInfo.NeedGoods[0],
									GCount = emblemStarInfo.EmblemUpInfo.NeedGoods[1]
								});
								num = emblemStarInfo.GoodsExp;
							}
							huiJiData.Exp++;
							if (emblemStarInfo.EmblemUpInfo.LuckyOne + huiJiData.Exp >= 110000)
							{
								huiJiData.huiji++;
								huiJiData.Exp = 0;
							}
							else if (emblemStarInfo.EmblemUpInfo.LuckyOne + huiJiData.Exp > emblemStarInfo.EmblemUpInfo.LuckyTwo)
							{
								if (Global.GetRandom() < emblemStarInfo.EmblemUpInfo.LuckyTwoRate)
								{
									huiJiData.huiji++;
									huiJiData.Exp = 0;
								}
							}
						}
						Global.SendToDB<RoleDataCmdT<RoleHuiJiData>>(1446, new RoleDataCmdT<RoleHuiJiData>(client.ClientData.RoleID, huiJiData), client.ServerId);
						if (huiJiData.huiji > huiJiUpdateResultData2.HuiJi)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.HuiJiHuTi,
								emblemStarInfo2.ExtPropValues
							});
							client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
							{
								default(DelayExecProcIds),
								2
							});
							EventLogManager.AddHuiJiEvent(client, type, (zuanShi > 0) ? 1 : 0, num, emblemStarInfo.EmblemLevel, emblemStarInfo.EmblemStar, emblemStarInfo2.EmblemLevel, emblemStarInfo2.EmblemStar, huiJiData.Exp, strCostList);
						}
						else
						{
							EventLogManager.AddHuiJiEvent(client, type, (zuanShi > 0) ? 1 : 0, num, emblemStarInfo.EmblemLevel, emblemStarInfo.EmblemStar, emblemStarInfo.EmblemLevel, emblemStarInfo.EmblemStar, huiJiData.Exp, strCostList);
						}
					}
				}
			}
			IL_646:
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiHuiJi))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
			huiJiUpdateResultData.Result = result;
			huiJiUpdateResultData.HuiJi = huiJiData.huiji;
			huiJiUpdateResultData.Exp = huiJiData.Exp;
			huiJiUpdateResultData.Auto = auto;
			huiJiUpdateResultData.Type = type;
			client.sendCmd<HuiJiUpdateResultData>(nID, huiJiUpdateResultData, false);
			return true;
		}

		private bool ProcessExecuteHuiJiHuTiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = 0;
			long num = TimeUtil.NOW();
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			if (num < clientExtData.HuiJiCDTicks)
			{
				cmdData = -2007;
			}
			else if (!this.IsGongNengOpened(client))
			{
				cmdData = -12;
			}
			else
			{
				long num2 = 0L;
				int num3 = 0;
				double[] props = null;
				lock (this.RuntimeData.Mutex)
				{
					EmblemStarInfo emblemStarInfo;
					if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(client.ClientData.HuiJiData.huiji, out emblemStarInfo))
					{
						cmdData = -20;
						goto IL_150;
					}
					EmblemUpInfo emblemUpInfo = emblemStarInfo.EmblemUpInfo;
					if (null == emblemUpInfo)
					{
						cmdData = -20;
						goto IL_150;
					}
					num2 = (long)emblemUpInfo.CDTime;
					num3 = emblemUpInfo.DurationTime;
					props = emblemUpInfo.ExtPropTempValues;
				}
				clientExtData.HuiJiCDTicks = num + (long)num3 + num2;
				clientExtData.HuiJiCdTime = num2;
				client.buffManager.SetStatusBuff(116, num, (long)num3, 0L);
				this.OnHuiJiStateChange(client, true, client.ClientData.HuiJiData.huiji, num3, props);
				Global.RemoveBufferData(client, 119);
				ZuoQiManager.getInstance().RoleDisMount(client, true);
			}
			IL_150:
			client.sendCmd<int>(nID, cmdData, false);
			return true;
		}

		public void OnRoleDead(GameClient client)
		{
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			long num = TimeUtil.NOW() + clientExtData.HuiJiCdTime;
			if (num < clientExtData.HuiJiCDTicks)
			{
				clientExtData.HuiJiCDTicks = num;
				client.buffManager.SetStatusBuff(116, 0L, 0L, 0L);
				double moveSpeed = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveSpeed;
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, 0L, 0, moveSpeed);
			}
		}

		public void OnInitGame(GameClient client)
		{
			if (client.ClientData.HuiJiData == null)
			{
				client.ClientData.HuiJiData = new RoleHuiJiData();
			}
			bool flag = false;
			int[] emblemFull = this.RuntimeData.EmblemFull;
			if (emblemFull != null && emblemFull[0] > 0)
			{
				for (int i = 1; i < emblemFull.Length; i++)
				{
					if (emblemFull[i] == client.ClientData.MapCode)
					{
						flag = true;
						break;
					}
				}
			}
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			this.InitDataByTask(client);
			lock (this.RuntimeData.Mutex)
			{
				EmblemStarInfo emblemStarInfo;
				if (this.RuntimeData.EmblemStarDict.Value.TryGetValue(client.ClientData.HuiJiData.huiji, out emblemStarInfo) && emblemStarInfo.EmblemUpInfo != null)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.HuiJiHuTi,
						emblemStarInfo.ExtPropValues
					});
					if (flag)
					{
						clientExtData.HuiJiCDTicks = 0L;
					}
					else
					{
						clientExtData.HuiJiCDTicks = TimeUtil.NOW() + (long)emblemStarInfo.EmblemUpInfo.CDTime;
					}
					clientExtData.HuiJiCdTime = (long)emblemStarInfo.EmblemUpInfo.CDTime;
				}
			}
		}

		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.HuiJiData.huiji <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.HuiJiData.huiji = 1;
				}
			}
		}

		public void OnHuiJiStateChange(GameClient client, bool active, int level = 0, int keepTicks = 0, double[] props = null)
		{
			if (active)
			{
				if (null != props)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						31,
						props
					});
				}
				client.ClientData.DongJieStart = 0L;
				client.ClientData.DongJieSeconds = 0;
				client.RoleBuffer.SetTempExtProp(47, 0.0, 0L);
				client.RoleBuffer.SetTempExtProp(2, 0.0, 0L);
				client.RoleBuffer.SetTempExtProp(18, 0.0, 0L);
				double moveSpeed = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveSpeed;
				double[] actionParams = new double[]
				{
					(double)level,
					(double)keepTicks
				};
				Global.UpdateBufferData(client, BufferItemTypes.HuiJiHuTi, actionParams, 1, true);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, TimeUtil.NOW(), keepTicks, moveSpeed);
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					31,
					PropsCacheManager.ConstExtProps
				});
				double moveSpeed = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveSpeed;
				double[] array = new double[2];
				array[0] = (double)level;
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.HuiJiHuTi, actionParams, 1, false);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, TimeUtil.NOW(), keepTicks, moveSpeed);
			}
		}

		public EmblemStarInfo GetHuiJiStartInfo(GameClient client)
		{
			EmblemStarInfo result;
			lock (this.RuntimeData.Mutex)
			{
				RoleHuiJiData huiJiData = client.ClientData.HuiJiData;
				this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji, out result);
			}
			return result;
		}

		private static HuiJiManager instance = new HuiJiManager();

		private HuiJiManagerData RuntimeData = new HuiJiManagerData();
	}
}
