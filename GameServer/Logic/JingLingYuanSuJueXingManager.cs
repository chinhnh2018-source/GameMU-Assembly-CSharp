using System;
using System.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class JingLingYuanSuJueXingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static JingLingYuanSuJueXingManager getInstance()
		{
			return JingLingYuanSuJueXingManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public void LoadConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string uri = "Config/JingLingYuanSu.xml";
					string text = Global.GameResPath(uri);
					this.RuntimeData.YuanSuInfoDict.Load(text, null);
					foreach (JingLingYuanSuInfo jingLingYuanSuInfo in this.RuntimeData.YuanSuInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(jingLingYuanSuInfo.ExtProps, jingLingYuanSuInfo.Attribute, "|,");
					}
					uri = "Config/JingLingYuanSuShuXing.xml";
					text = Global.GameResPath(uri);
					this.RuntimeData.ShuXingInfoDict.Load(text, null);
					foreach (JingLingYuanSuShuXingInfo jingLingYuanSuShuXingInfo in this.RuntimeData.ShuXingInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(jingLingYuanSuShuXingInfo.ExtProps, jingLingYuanSuShuXingInfo.AcetiveElement, "|,");
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1450, 2, 2, JingLingYuanSuJueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1451, 4, 4, JingLingYuanSuJueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
				case 1450:
					result = this.ProcessJingLingYuanSuJueXingActiveCmd(client, nID, bytes, cmdParams);
					break;
				case 1451:
					result = this.ProcessJingLingYuanSuJueXingUpgradeCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ProcessJingLingYuanSuJueXingActiveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[1]);
				JingLingYuanSuJueXingData jingLingYuanSuJueXingData = client.ClientData.JingLingYuanSuJueXingData;
				if (jingLingYuanSuJueXingData == null || jingLingYuanSuJueXingData.ActiveIDs == null)
				{
					jingLingYuanSuJueXingData = new JingLingYuanSuJueXingData();
					jingLingYuanSuJueXingData.ActiveIDs = new int[6];
					client.ClientData.JingLingYuanSuJueXingData = jingLingYuanSuJueXingData;
				}
				if (jingLingYuanSuJueXingData != null && null != jingLingYuanSuJueXingData.ActiveIDs)
				{
					jingLingYuanSuJueXingData.ActiveType = num2;
					num = Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jingLingYuanSuJueXingData), client.ServerId);
					JingLingYuanSuJueXingManager.getInstance().RefreshProps(client, false);
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						2
					});
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JingLingYuanSuJueXing :: 获取觉醒石数据错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public void GMSetJingLingYuanSuJueXingData(GameClient client, string[] cmdFields)
		{
			JingLingYuanSuJueXingData jingLingYuanSuJueXingData = client.ClientData.JingLingYuanSuJueXingData;
			if (jingLingYuanSuJueXingData == null || jingLingYuanSuJueXingData.ActiveIDs == null)
			{
				jingLingYuanSuJueXingData = new JingLingYuanSuJueXingData();
				jingLingYuanSuJueXingData.ActiveIDs = new int[6];
				client.ClientData.JingLingYuanSuJueXingData = jingLingYuanSuJueXingData;
			}
			int activeType = Global.SafeConvertToInt32(cmdFields[2]);
			jingLingYuanSuJueXingData.ActiveType = activeType;
			for (int i = 3; i < cmdFields.Length; i++)
			{
				jingLingYuanSuJueXingData.ActiveIDs[i - 3] = Global.SafeConvertToInt32(cmdFields[i]);
			}
			Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jingLingYuanSuJueXingData), client.ServerId);
			this.RefreshProps(client, false);
		}

		public bool ProcessJingLingYuanSuJueXingUpgradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				int num4 = Convert.ToInt32(cmdParams[3]);
				int num5 = 0;
				JingLingYuanSuJueXingData jingLingYuanSuJueXingData = client.ClientData.JingLingYuanSuJueXingData;
				if (jingLingYuanSuJueXingData == null || jingLingYuanSuJueXingData.ActiveIDs == null)
				{
					jingLingYuanSuJueXingData = new JingLingYuanSuJueXingData();
					jingLingYuanSuJueXingData.ActiveIDs = new int[6];
				}
				int num6 = (num2 - 1) * 2 + num3 - 1;
				if (num6 < 0 || num6 >= jingLingYuanSuJueXingData.ActiveIDs.Length)
				{
					num = -18;
				}
				else
				{
					int num7 = 0;
					int num8 = jingLingYuanSuJueXingData.ActiveIDs[num6];
					JingLingYuanSuInfo jingLingYuanSuInfo = null;
					JingLingYuanSuInfo jingLingYuanSuInfo2 = null;
					JingLingYuanSuInfo jingLingYuanSuInfo3 = null;
					lock (this.RuntimeData.Mutex)
					{
						if (num8 > 0)
						{
							if (this.RuntimeData.YuanSuInfoDict.Value.TryGetValue(num8, out jingLingYuanSuInfo))
							{
								if (jingLingYuanSuInfo.YuanSuType != num2 || jingLingYuanSuInfo.ShuXingType != num3)
								{
									num = -3;
									goto IL_4AB;
								}
								num7 = jingLingYuanSuInfo.QiangHuaLevel;
							}
						}
						int num9 = num7 + 1;
						int num10 = num7 - 1;
						foreach (JingLingYuanSuInfo jingLingYuanSuInfo4 in this.RuntimeData.YuanSuInfoDict.Value.Values)
						{
							if (jingLingYuanSuInfo4.YuanSuType == num2 && jingLingYuanSuInfo4.ShuXingType == num3)
							{
								if (jingLingYuanSuInfo4.QiangHuaLevel == num7)
								{
									jingLingYuanSuInfo = jingLingYuanSuInfo4;
								}
								else if (jingLingYuanSuInfo4.QiangHuaLevel == num9)
								{
									jingLingYuanSuInfo2 = jingLingYuanSuInfo4;
								}
								else if (jingLingYuanSuInfo4.QiangHuaLevel == num10)
								{
									jingLingYuanSuInfo3 = jingLingYuanSuInfo4;
								}
							}
						}
					}
					if (jingLingYuanSuInfo == null)
					{
						num = -3;
					}
					else
					{
						num5 = jingLingYuanSuInfo.ID;
						if (jingLingYuanSuInfo2 == null)
						{
							num = -23;
						}
						else if (client.ClientData.MoneyData[144] < (long)jingLingYuanSuInfo.JieXingCurrency)
						{
							num = -47;
						}
						else if (!GoodsUtil.CheckHasGoodsList(client, jingLingYuanSuInfo.NeedGoods, false))
						{
							num = -6;
						}
						else
						{
							if (num4 > 0)
							{
								if (!GoodsUtil.CheckHasGoodsList(client, jingLingYuanSuInfo.Failtofail, false))
								{
									num = -6;
									goto IL_4AB;
								}
							}
							if (!GameManager.ClientMgr.ModifyYuanSuJueXingShiValue(client, -jingLingYuanSuInfo.JieXingCurrency, "精灵元素觉醒", true, true, false))
							{
								num = -47;
							}
							else
							{
								string text = "";
								if (!GoodsUtil.CostGoodsList(client, jingLingYuanSuInfo.NeedGoods, false, ref text, "精灵元素觉醒"))
								{
								}
								if (num4 > 0)
								{
									if (!GoodsUtil.CostGoodsList(client, jingLingYuanSuInfo.Failtofail, false, ref text, "精灵元素觉醒"))
									{
									}
								}
								bool flag2 = false;
								double random = Global.GetRandom();
								if (random <= jingLingYuanSuInfo.Success)
								{
									flag2 = true;
								}
								int qiangHuaLevel;
								if (flag2)
								{
									num5 = jingLingYuanSuInfo2.ID;
									qiangHuaLevel = jingLingYuanSuInfo2.QiangHuaLevel;
									jingLingYuanSuJueXingData.ActiveIDs[num6] = num5;
								}
								else
								{
									if (num4 > 0 || null == jingLingYuanSuInfo3)
									{
										num = 6;
										goto IL_4AB;
									}
									num5 = jingLingYuanSuInfo3.ID;
									qiangHuaLevel = jingLingYuanSuInfo3.QiangHuaLevel;
									jingLingYuanSuJueXingData.ActiveIDs[num6] = num5;
								}
								if (jingLingYuanSuJueXingData.ActiveType == num2)
								{
									JingLingYuanSuJueXingManager.getInstance().RefreshProps(client, false);
									client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
									{
										2
									});
								}
								EventLogManager.AddRoleEvent(client, OpTypes.Upgrade, OpTags.Trace, LogRecordType.JingLingYuanSuJueXing, new object[]
								{
									num8,
									num7,
									num5,
									qiangHuaLevel,
									jingLingYuanSuInfo.JieXingCurrency,
									num4,
									text
								});
								num = Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jingLingYuanSuJueXingData), client.ServerId);
							}
						}
					}
				}
				IL_4AB:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num5, num4), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JingLingYuanSuJueXing :: 激活觉醒石错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public void RefreshProps(GameClient client, bool hint = true)
		{
			JingLingYuanSuJueXingData jingLingYuanSuJueXingData = client.ClientData.JingLingYuanSuJueXingData;
			if (jingLingYuanSuJueXingData != null && jingLingYuanSuJueXingData.ActiveIDs != null)
			{
				int[] array = new int[2];
				int activeType = jingLingYuanSuJueXingData.ActiveType;
				int num = (activeType - 1) * 2;
				while (num >= 0 && num < activeType * 2 && num < jingLingYuanSuJueXingData.ActiveIDs.Length)
				{
					double[] array2 = PropsCacheManager.ConstExtProps;
					if (0 <= num && num < jingLingYuanSuJueXingData.ActiveIDs.Length)
					{
						int key = jingLingYuanSuJueXingData.ActiveIDs[num];
						lock (this.RuntimeData.Mutex)
						{
							JingLingYuanSuInfo jingLingYuanSuInfo;
							if (this.RuntimeData.YuanSuInfoDict.Value.TryGetValue(key, out jingLingYuanSuInfo))
							{
								array2 = jingLingYuanSuInfo.ExtProps;
								array[num - (activeType - 1) * 2] = jingLingYuanSuInfo.QiangHuaLevel;
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JingLingYuanSuJueXing,
						num % 2,
						array2
					});
					num++;
				}
				double[] array3 = PropsCacheManager.ConstExtProps;
				int num2 = array.Min();
				if (num2 > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						int num3 = 0;
						foreach (JingLingYuanSuShuXingInfo jingLingYuanSuShuXingInfo in this.RuntimeData.ShuXingInfoDict.Value.Values)
						{
							int num4 = jingLingYuanSuShuXingInfo.Level * 4;
							if (jingLingYuanSuShuXingInfo.Tipe == activeType && num2 >= num4)
							{
								if (num3 < num4)
								{
									num3 = num4;
									array3 = jingLingYuanSuShuXingInfo.ExtProps;
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingYuanSuJueXing,
					10,
					array3
				});
				if (hint)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(16) && GlobalNew.IsGongNengOpened(client, 101, hint);
		}

		public JingLingYuanSuJueXingRunData RuntimeData = new JingLingYuanSuJueXingRunData();

		private static JingLingYuanSuJueXingManager instance = new JingLingYuanSuJueXingManager();
	}
}
