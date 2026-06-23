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
	public class BianShenManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static BianShenManager getInstance()
		{
			return BianShenManager.instance;
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
				if (num != 28)
				{
					if (num == 58)
					{
						MainTaskProgressEvent mainTaskProgressEvent = eventObject as MainTaskProgressEvent;
						if (null != mainTaskProgressEvent)
						{
							this.InitDataByTask(mainTaskProgressEvent.Client);
						}
					}
				}
				else
				{
					OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
					if (null != onStartPlayGameEventObject)
					{
						if (!this.CanBianShenByMap(onStartPlayGameEventObject.Client))
						{
							this.ClearBianShen(onStartPlayGameEventObject.Client);
						}
					}
				}
			}
			else
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.ClearBianShen(playerDeadEventObject.getPlayer());
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
					this.RuntimeData.IsGongNengOpend = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(103);
					this.RuntimeData.BianShenFull = GameManager.systemParamsList.GetParamValueIntArrayByName("BianShenFull", ',');
					this.RuntimeData.BianShenCDSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationCD", -1);
					this.RuntimeData.TransfigurationBuff = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationBuff", -1);
					this.RuntimeData.NeedGoods = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("TransfigurationGoods"), true, '|', ',');
					this.RuntimeData.FreeNum = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationFree", -1);
					text = "Config/TransfigurationFashionEffect.xml";
					string text2 = Global.GameResPath(text);
					this.RuntimeData.FashionEffectInfoDict.Load(text2, null);
					foreach (FashionEffectInfo fashionEffectInfo in this.RuntimeData.FashionEffectInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(fashionEffectInfo.ExtPropValues, fashionEffectInfo.ProPerty, "|,");
					}
					text = "Config/TransfigurationLevel.xml";
					text2 = Global.GameResPath(text);
					this.RuntimeData.BianShenStarDict.Load(text2, null);
					foreach (BianShenStarInfo bianShenStarInfo in this.RuntimeData.BianShenStarDict.Value.Values)
					{
						foreach (int key in bianShenStarInfo.OccupationID)
						{
							List<BianShenStarInfo> list;
							if (!this.RuntimeData.BianShenUpDict.TryGetValue(key, out list))
							{
								list = new List<BianShenStarInfo>();
								this.RuntimeData.BianShenUpDict[key] = list;
							}
							int level = bianShenStarInfo.Level;
							while (list.Count < level + 1)
							{
								list.Add(null);
							}
							list[level] = bianShenStarInfo;
							ConfigParser.ParseExtprops(bianShenStarInfo.ExtPropValues, bianShenStarInfo.ProPerty, "|,");
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
			TCPCmdDispatcher.getInstance().registerProcessorEx(1448, 1, 1, BianShenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1449, 1, 1, BianShenManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(28, BianShenManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, BianShenManager.getInstance());
			GlobalEventSource.getInstance().registerListener(58, BianShenManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, BianShenManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, BianShenManager.getInstance());
			GlobalEventSource.getInstance().removeListener(58, BianShenManager.getInstance());
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
			case 1448:
				result = this.ProcessExecuteBianShenCmd(client, nID, bytes, cmdParams);
				break;
			case 1449:
				result = this.ProcessBianShenStarUpCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return this.RuntimeData.IsGongNengOpend && GlobalNew.IsGongNengOpened(client, 103, false);
		}

		private bool ProcessBianShenStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			BianShenUpdateResultData bianShenUpdateResultData = new BianShenUpdateResultData();
			RoleBianShenData bianShenData = client.ClientData.BianShenData;
			BianShenUpdateResultData bianShenUpdateResultData2 = DataHelper.BytesToObject<BianShenUpdateResultData>(bytes, 0, bytes.Length);
			int type = bianShenUpdateResultData2.Type;
			int zuanShi = bianShenUpdateResultData2.ZuanShi;
			int auto = bianShenUpdateResultData2.Auto;
			long num = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -400;
			}
			else
			{
				string strCostList = "";
				lock (this.RuntimeData.Mutex)
				{
					List<BianShenStarInfo> list;
					if (bianShenData.BianShen != bianShenUpdateResultData2.BianShen)
					{
						result = -18;
					}
					else if (!this.RuntimeData.BianShenUpDict.TryGetValue(client.ClientData.Occupation, out list))
					{
						result = -400;
					}
					else if (bianShenData.BianShen >= list.Count - 1)
					{
						result = -23;
					}
					else
					{
						BianShenStarInfo bianShenStarInfo = list[bianShenData.BianShen];
						BianShenStarInfo bianShenStarInfo2 = list[bianShenData.BianShen + 1];
						if (bianShenStarInfo == null || bianShenStarInfo2 == null)
						{
							result = -3;
						}
						else if (!GoodsUtil.CostGoodsList(client, bianShenStarInfo.NeedGoods, false, ref strCostList, "变身升级"))
						{
							result = -6;
						}
						else
						{
							int num2 = bianShenStarInfo.GoodsExp;
							if (Global.GetRandom() < bianShenStarInfo.ExpCritRate)
							{
								num2 = (int)((double)num2 * bianShenStarInfo.ExpCritTimes);
							}
							bianShenData.Exp += num2;
							if (bianShenData.Exp >= bianShenStarInfo.UpExp)
							{
								bianShenData.BianShen++;
								bianShenData.Exp -= bianShenStarInfo.UpExp;
							}
							Global.SendToDB<RoleDataCmdT<RoleBianShenData>>(1449, new RoleDataCmdT<RoleBianShenData>(client.ClientData.RoleID, bianShenData), client.ServerId);
							if (bianShenData.BianShen > bianShenUpdateResultData2.BianShen)
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BianShen,
									bianShenStarInfo2.ExtPropValues
								});
								client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
								{
									default(DelayExecProcIds),
									2
								});
								EventLogManager.AddBianShenEvent(client, type, (zuanShi > 0) ? 1 : 0, num2, bianShenStarInfo.Level, bianShenStarInfo2.Level, bianShenData.Exp, strCostList);
							}
							else
							{
								EventLogManager.AddBianShenEvent(client, type, (zuanShi > 0) ? 1 : 0, num2, bianShenStarInfo.Level, bianShenStarInfo.Level, bianShenData.Exp, strCostList);
							}
						}
					}
				}
			}
			bianShenUpdateResultData.Result = result;
			bianShenUpdateResultData.BianShen = bianShenData.BianShen;
			bianShenUpdateResultData.Exp = bianShenData.Exp;
			bianShenUpdateResultData.Auto = auto;
			bianShenUpdateResultData.Type = type;
			client.sendCmd<BianShenUpdateResultData>(nID, bianShenUpdateResultData, false);
			return true;
		}

		private bool ProcessExecuteBianShenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = 0;
			long num = TimeUtil.NOW();
			if (!this.CanBianShenByMap(client))
			{
				cmdData = -21;
			}
			else if (client.ClientData.IsDongJie())
			{
				cmdData = -500;
			}
			else
			{
				ExtData clientExtData = ExtDataManager.GetClientExtData(client);
				if (num < clientExtData.BianShenCDTicks)
				{
					cmdData = -2007;
				}
				else if (!this.IsGongNengOpened(client) || client.ClientData.HideGM > 0)
				{
					cmdData = -12;
				}
				else
				{
					ZuoQiManager.getInstance().RoleDisMount(client, true);
					long num2 = (long)(this.RuntimeData.BianShenCDSecs * 1000);
					int num3 = 0;
					int num4 = 1;
					double[] props = null;
					List<int> list = null;
					BianShenStarInfo bianShenStarInfo;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.BianShenStarDict.Value.TryGetValue(client.ClientData.BianShenData.BianShen, out bianShenStarInfo))
						{
							cmdData = -20;
							goto IL_4E7;
						}
						num4 = bianShenStarInfo.Level;
					}
					string text = "";
					long num5 = client.ClientData.MoneyData[147];
					int num6 = (int)(num5 / 10000L);
					int num7 = (int)(num5 % 10000L);
					if (num6 != TimeUtil.GetOffsetDayNow())
					{
						num7 = 0;
						num6 = TimeUtil.GetOffsetDayNow();
					}
					if (num7 < this.RuntimeData.FreeNum)
					{
						num7++;
						num5 = (long)(num6 * 10000 + num7);
						client.ClientData.MoneyData[147] = num5;
						Global.SaveRoleParamsInt64ValueToDB(client, "10216", num5, true);
						GameManager.ClientMgr.NotifySelfPropertyValue(client, 147, num5);
					}
					else if (!GoodsUtil.CostGoodsList(client, this.RuntimeData.NeedGoods, false, ref text, "变身"))
					{
						cmdData = -6;
						goto IL_4E7;
					}
					int occuDamageType = OccupationUtil.GetOccuDamageType(client.ClientData.OccupationIndex);
					GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 28);
					if (null != goodsDataByCategoriy)
					{
						FashionBagData fashionBagData = FashionManager.getInstance().GetFashionBagData(client, goodsDataByCategoriy);
						if (fashionBagData != null && fashionBagData.FasionTab == 7)
						{
							if (occuDamageType == 1)
							{
								list = fashionBagData.MagicSkill;
							}
							else
							{
								list = fashionBagData.AttackSkill;
							}
							num3 = fashionBagData.BianShenDuration;
							if (fashionBagData.BianShenEffect > 0)
							{
								lock (this.RuntimeData.Mutex)
								{
									FashionEffectInfo fashionEffectInfo;
									if (this.RuntimeData.FashionEffectInfoDict.Value.TryGetValue(fashionBagData.BianShenEffect, out fashionEffectInfo))
									{
										props = fashionEffectInfo.ExtPropValues;
									}
								}
							}
						}
					}
					else
					{
						num3 = bianShenStarInfo.Duration;
						if (occuDamageType == 1)
						{
							list = bianShenStarInfo.MagicSkill;
						}
						else
						{
							list = bianShenStarInfo.AttackSkill;
						}
					}
					if (null != list)
					{
						lock (client.ClientData.SkillDataList)
						{
							using (List<int>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									int skillID = enumerator.Current;
									SkillData skillData = client.ClientData.SkillDataList.Find((SkillData x) => x.SkillID == skillID);
									if (null == skillData)
									{
										SkillData skillData2 = new SkillData();
										skillData2.SkillID = skillID;
										skillData2.SkillLevel = num4;
										client.ClientData.SkillDataList.Add(skillData2);
									}
									else if (skillData.SkillLevel != num4)
									{
										skillData.SkillLevel = num4;
									}
								}
							}
						}
					}
					clientExtData.skillIDList = list;
					clientExtData.BianShenCDTicks = num + num2 + (long)(num3 * 1000);
					clientExtData.BianShenCdTime = num2;
					clientExtData.BianShenToTicks = num + (long)(num3 * 1000);
					client.buffManager.SetStatusBuff(121, num, (long)(num3 * 1000), 0L);
					this.OnBianShenStateChange(client, true, client.ClientData.BianShenData.BianShen, num3, props);
					GameManager.ClientMgr.NotifySkillCDTime(client, -1, clientExtData.BianShenCDTicks, false);
				}
			}
			IL_4E7:
			client.sendCmd<int>(nID, cmdData, false);
			return true;
		}

		public bool CanBianShenByMap(GameClient client)
		{
			MapSettingItem mapSettingItem;
			return Data.SettingsDict.Value.TryGetValue(client.ClientData.MapCode, out mapSettingItem) && mapSettingItem.Transfiguration > 0;
		}

		private void OnStartPlayGame(GameClient client)
		{
			if (!this.CanBianShenByMap(client))
			{
				if (client.buffManager.IsBuffEnabled(121))
				{
					this.ClearBianShen(client);
				}
			}
		}

		public void ClearBianShen(GameClient client)
		{
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			long num = TimeUtil.NOW();
			if (num < clientExtData.BianShenToTicks)
			{
				clientExtData.BianShenToTicks = 0L;
				clientExtData.BianShenCDTicks = num + clientExtData.BianShenCdTime;
				clientExtData.skillIDList = null;
				client.buffManager.SetStatusBuff(121, 0L, 0L, 0L);
			}
		}

		public bool CanUseMagic(GameClient client, int skillID)
		{
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			List<int> skillIDList = clientExtData.skillIDList;
			if (client.buffManager.IsBuffEnabled(121))
			{
				if (skillIDList != null && skillIDList.Contains(skillID))
				{
					return true;
				}
			}
			else if (skillIDList == null || !skillIDList.Contains(skillID))
			{
				return true;
			}
			return false;
		}

		public void OnInitGame(GameClient client)
		{
			RoleBianShenData roleBianShenData = client.ClientData.BianShenData;
			if (roleBianShenData == null)
			{
				roleBianShenData = (client.ClientData.BianShenData = new RoleBianShenData());
			}
			bool flag = false;
			int[] bianShenFull = this.RuntimeData.BianShenFull;
			if (bianShenFull != null && bianShenFull[0] > 0)
			{
				for (int i = 1; i < bianShenFull.Length; i++)
				{
					if (bianShenFull[i] == client.ClientData.MapCode)
					{
						flag = true;
						break;
					}
				}
			}
			ExtData clientExtData = ExtDataManager.GetClientExtData(client);
			this.InitDataByTask(client);
			if (roleBianShenData.BianShen > 0)
			{
				clientExtData.BianShenToTicks = 0L;
				clientExtData.skillIDList = null;
				lock (this.RuntimeData.Mutex)
				{
					List<BianShenStarInfo> list;
					if (this.RuntimeData.BianShenUpDict.TryGetValue(client.ClientData.Occupation, out list) && roleBianShenData.BianShen < list.Count)
					{
						BianShenStarInfo bianShenStarInfo = list[roleBianShenData.BianShen];
						if (null != bianShenStarInfo)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.BianShen,
								bianShenStarInfo.ExtPropValues
							});
							if (flag)
							{
								clientExtData.BianShenCDTicks = 0L;
							}
							else
							{
								long num = TimeUtil.NOW();
								long num2 = num + clientExtData.BianShenCdTime;
								if (num2 < clientExtData.BianShenCDTicks)
								{
									clientExtData.BianShenCDTicks = num2;
								}
								if (num < clientExtData.BianShenCDTicks)
								{
									GameManager.ClientMgr.NotifySkillCDTime(client, -1, clientExtData.BianShenCDTicks, true);
								}
							}
						}
					}
				}
			}
		}

		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.BianShenData.BianShen <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.BianShenData.BianShen = 1;
				}
			}
		}

		public void OnBianShenStateChange(GameClient client, bool active, int level = 0, int keepSecs = 0, double[] props = null)
		{
			if (active)
			{
				if (null != props)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						41,
						props
					});
				}
				double[] actionParams = new double[]
				{
					(double)this.RuntimeData.TransfigurationBuff,
					(double)keepSecs
				};
				Global.UpdateBufferData(client, BufferItemTypes.BianShen, actionParams, 1, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					41,
					PropsCacheManager.ConstExtProps
				});
				double[] array = new double[2];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.BianShen, actionParams, 1, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		public int GetBianShenLevel(GameClient client)
		{
			int result;
			if (null != client.ClientData.BianShenData)
			{
				result = client.ClientData.BianShenData.BianShen;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static BianShenManager instance = new BianShenManager();

		private BianShenManagerData RuntimeData = new BianShenManagerData();
	}
}
