using System;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ArmorManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static ArmorManager getInstance()
		{
			return ArmorManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num != 10)
			{
				if (num == 57)
				{
					TimerEventObject timerEventObject = eventObject as TimerEventObject;
					if (null != timerEventObject)
					{
						this.OnTimer(timerEventObject);
					}
				}
			}
			else
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
					this.RuntimeData.HudunBaoji = GameManager.systemParamsList.GetParamValueDoubleArrayByName("HudunBaoji", ',');
					text = "Config/ShenshenghudunJie.xml";
					string text2 = Global.GameResPath(text);
					this.RuntimeData.ArmorUpDict.Load(text2, null);
					text = "Config/ShenshenghudunXing.xml";
					text2 = Global.GameResPath(text);
					this.RuntimeData.ArmorStarDict.Load(text2, null);
					foreach (ArmorStarInfo armorStarInfo in this.RuntimeData.ArmorStarDict.Value.Values)
					{
						ArmorUpInfo armorUpInfo;
						if (this.RuntimeData.ArmorUpDict.Value.TryGetValue(armorStarInfo.ArmorupStage, out armorUpInfo))
						{
							armorStarInfo.ArmorUpInfo = armorUpInfo;
							armorUpInfo.MaxStarLevel = Math.Max(armorUpInfo.MaxStarLevel, armorStarInfo.StarLevel);
							armorStarInfo.ArmorUp += armorUpInfo.ArmorUp;
							armorStarInfo.AddAttack += armorUpInfo.AddAttack;
							armorStarInfo.AddDefense += armorUpInfo.AddDefense;
							armorStarInfo.ShenmingUP += armorUpInfo.ShenmingUP;
							armorStarInfo.ExtPropValues[119] = (double)armorStarInfo.ArmorUp;
							armorStarInfo.ExtPropValues[45] = (double)armorStarInfo.AddAttack;
							armorStarInfo.ExtPropValues[46] = (double)armorStarInfo.AddDefense;
							armorStarInfo.ExtPropValues[13] = (double)armorStarInfo.ShenmingUP;
							armorStarInfo.ExtPropValues[120] = armorUpInfo.Damageabsorption;
							armorStarInfo.ExtPropValues[121] = armorUpInfo.Armorrecovery;
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
			TCPCmdDispatcher.getInstance().registerProcessorEx(1447, 1, 1, ArmorManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(10, ArmorManager.getInstance());
			GlobalEventSource.getInstance().registerListener(57, ArmorManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ArmorManager.getInstance());
			GlobalEventSource.getInstance().removeListener(57, ArmorManager.getInstance());
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
			return nID != 1447 || this.ProcessArmorStarUpCmd(client, nID, bytes, cmdParams);
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return GlobalNew.IsGongNengOpened(client, 100, false);
		}

		private bool ProcessArmorStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			ArmorUpdateResultData armorUpdateResultData = new ArmorUpdateResultData();
			RoleArmorData armorData = client.ClientData.ArmorData;
			int num = 0;
			ArmorUpdateResultData armorUpdateResultData2 = DataHelper.BytesToObject<ArmorUpdateResultData>(bytes, 0, bytes.Length);
			int type = armorUpdateResultData2.Type;
			int zuanShi = armorUpdateResultData2.ZuanShi;
			int auto = armorUpdateResultData2.Auto;
			long num2 = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else
			{
				bool flag = false;
				lock (this.RuntimeData.Mutex)
				{
					if (armorData.Armor != armorUpdateResultData2.Armor)
					{
						result = -3;
						goto IL_611;
					}
					ArmorStarInfo armorStarInfo;
					if (!this.RuntimeData.ArmorStarDict.Value.TryGetValue(armorData.Armor, out armorStarInfo))
					{
						result = -3;
						goto IL_611;
					}
					ArmorStarInfo armorStarInfo2;
					if (!this.RuntimeData.ArmorStarDict.Value.TryGetValue(armorData.Armor + 1, out armorStarInfo2))
					{
						result = -4004;
						goto IL_611;
					}
					bool flag3 = false;
					bool flag4 = false;
					string strCostList;
					if (type == 0)
					{
						if (armorStarInfo.ArmorUpInfo.MaxStarLevel == armorStarInfo.StarLevel)
						{
							result = -4;
							goto IL_611;
						}
						if (Global.UseGoodsBindOrNot(client, armorStarInfo.NeedGoods[0], armorStarInfo.NeedGoods[1], true, out flag3, out flag4) < 0)
						{
							if (zuanShi <= 0 || zuanShi != armorStarInfo.NeedDiamond)
							{
								result = -6;
								goto IL_611;
							}
							if (!GameManager.ClientMgr.SubUserMoney(client, zuanShi, "神圣护盾升星", true, true, true, true, DaiBiSySType.None))
							{
								result = -10;
								goto IL_611;
							}
							num = armorStarInfo.ZuanShiExp;
							strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
							{
								-zuanShi,
								client.ClientData.UserMoney + zuanShi,
								client.ClientData.UserMoney
							});
							if (Global.GetRandom() < this.RuntimeData.HudunBaoji[1])
							{
								num = (int)((double)num * this.RuntimeData.HudunBaoji[2]);
							}
						}
						else
						{
							num = armorStarInfo.GoodsExp;
							strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
							{
								GoodsID = armorStarInfo.NeedGoods[0],
								GCount = armorStarInfo.NeedGoods[1]
							});
							if (Global.GetRandom() < this.RuntimeData.HudunBaoji[0])
							{
								num = (int)((double)num * this.RuntimeData.HudunBaoji[2]);
							}
						}
						armorData.Exp += num;
						if (armorData.Exp >= armorStarInfo.StarExp)
						{
							armorData.Armor++;
							if (armorStarInfo.StarLevel < armorStarInfo.ArmorUpInfo.MaxStarLevel - 1)
							{
								armorData.Exp -= armorStarInfo.StarExp;
							}
							else
							{
								armorData.Exp = 0;
							}
						}
					}
					else
					{
						if (armorStarInfo.ArmorUpInfo.MaxStarLevel != armorStarInfo.StarLevel)
						{
							result = -4;
							goto IL_611;
						}
						if (Global.UseGoodsBindOrNot(client, armorStarInfo.ArmorUpInfo.NeedGoods[0], armorStarInfo.ArmorUpInfo.NeedGoods[1], true, out flag3, out flag4) < 0)
						{
							if (zuanShi <= 0 || zuanShi != armorStarInfo.ArmorUpInfo.NeedDiamond)
							{
								result = -6;
								goto IL_611;
							}
							if (!GameManager.ClientMgr.SubUserMoney(client, zuanShi, "神圣护盾升阶", true, true, true, true, DaiBiSySType.None))
							{
								result = -10;
								goto IL_611;
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
								GoodsID = armorStarInfo.ArmorUpInfo.NeedGoods[0],
								GCount = armorStarInfo.ArmorUpInfo.NeedGoods[1]
							});
							num = armorStarInfo.GoodsExp;
						}
						armorData.Exp++;
						if (armorStarInfo.ArmorUpInfo.LuckyOne + armorData.Exp >= 110000)
						{
							armorData.Armor++;
							armorData.Exp = 0;
						}
						else if (armorStarInfo.ArmorUpInfo.LuckyOne + armorData.Exp > armorStarInfo.ArmorUpInfo.LuckyTwo)
						{
							if (Global.GetRandom() < armorStarInfo.ArmorUpInfo.LuckyTwoRate)
							{
								armorData.Armor++;
								armorData.Exp = 0;
							}
						}
					}
					Global.SendToDB<RoleDataCmdT<RoleArmorData>>(1447, new RoleDataCmdT<RoleArmorData>(client.ClientData.RoleID, armorData), client.ServerId);
					if (armorData.Armor > armorUpdateResultData2.Armor)
					{
						flag = true;
						EventLogManager.AddArmorEvent(client, type, (zuanShi > 0) ? 1 : 0, num, armorStarInfo.ArmorupStage, armorStarInfo.StarLevel, armorStarInfo2.ArmorupStage, armorStarInfo2.StarLevel, armorData.Exp, strCostList);
					}
					else
					{
						EventLogManager.AddArmorEvent(client, type, (zuanShi > 0) ? 1 : 0, num, armorStarInfo.ArmorupStage, armorStarInfo.StarLevel, armorStarInfo.ArmorupStage, armorStarInfo.StarLevel, armorData.Exp, strCostList);
					}
				}
				if (flag)
				{
					this.ResetArmor(client, true);
				}
			}
			IL_611:
			armorUpdateResultData.Result = result;
			armorUpdateResultData.Armor = armorData.Armor;
			armorUpdateResultData.Exp = armorData.Exp;
			armorUpdateResultData.Auto = auto;
			armorUpdateResultData.Type = type;
			client.sendCmd<ArmorUpdateResultData>(nID, armorUpdateResultData, false);
			return true;
		}

		public void ResetArmor(GameClient client, bool reset = true)
		{
			if (client.ClientData.ArmorData.Armor > 0)
			{
				ExtData clientExtData = ExtDataManager.GetClientExtData(client);
				lock (this.RuntimeData.Mutex)
				{
					ArmorStarInfo armorStarInfo;
					if (this.RuntimeData.ArmorStarDict.Value.TryGetValue(client.ClientData.ArmorData.Armor, out armorStarInfo) && armorStarInfo.ArmorUpInfo != null)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.Armor,
							armorStarInfo.ExtPropValues
						});
					}
				}
				client.ClientData.ArmorPercent = RoleAlgorithm.GetExtProp(client, 120);
				int num = (int)RoleAlgorithm.GetExtProp(client, 119);
				if (reset)
				{
					clientExtData.ArmorCurrentV = clientExtData.ArmorMaxV;
				}
				else if (num > clientExtData.ArmorMaxV)
				{
					clientExtData.ArmorCurrentV += num - clientExtData.ArmorMaxV;
				}
				clientExtData.ArmorMaxV = num;
				if (clientExtData.ArmorMaxV != client.ClientData.ArmorV || clientExtData.ArmorCurrentV != client.ClientData.CurrentArmorV)
				{
					client.ClientData.ArmorV = clientExtData.ArmorMaxV;
					client.ClientData.CurrentArmorV = clientExtData.ArmorCurrentV;
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						2
					});
				}
			}
		}

		public void OnRoleDead(GameClient client)
		{
			this.ResetArmor(client, true);
		}

		public void OnInitGame(GameClient client)
		{
			if (client.ClientData.ArmorData == null)
			{
				client.ClientData.ArmorData = new RoleArmorData();
			}
			this.InitDataByTask(client);
			this.ResetArmor(client, true);
		}

		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.ArmorData.Armor <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.ArmorData.Armor = 1;
					this.ResetArmor(client, true);
				}
			}
		}

		public void OnTimer(TimerEventObject eventObj)
		{
			GameClient client = eventObj.Client;
			if (client.ClientData.CurrentArmorV < client.ClientData.ArmorV)
			{
				if (!client.buffManager.IsBuffEnabled(114))
				{
					double num = Global.Clamp((double)eventObj.DeltaTicks / 1000.0, 0.0, 5.0);
					int num2 = (int)RoleAlgorithm.GetExtProp(client, 119);
					int num3 = (int)((double)num2 * RoleAlgorithm.GetExtProp(client, 121) * num);
					client.ClientData.CurrentArmorV += num3;
					if (client.ClientData.CurrentArmorV > num2)
					{
						client.ClientData.CurrentArmorV = num2;
					}
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 4);
				}
			}
		}

		private static ArmorManager instance = new ArmorManager();

		private ArmorManagerData RuntimeData = new ArmorManagerData();
	}
}
