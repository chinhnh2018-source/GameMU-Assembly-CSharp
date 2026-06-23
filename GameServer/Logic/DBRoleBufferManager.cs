using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.NewBufferExt;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class DBRoleBufferManager
	{
		public static void ProcessLifeVAndMagicVReserve(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "血瓶蓝瓶");
				bool flag = false;
				if (client.ClientData.CurrentLifeV < client.ClientData.LifeV)
				{
					if (Global.CanMapUseBuffer(client.ClientData.MapCode, 4))
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(client, 4);
						if (null != bufferDataByID)
						{
							if (bufferDataByID.BufferVal > 0L)
							{
								flag = true;
								roleRelifeLog.hpModify = true;
								roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
								int num = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
								int num2 = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("LifeRecoverNum", -1));
								num2 = (int)((double)num2 * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client)));
								num = Global.GMin(num, num2);
								num = Global.GMin(num, (int)bufferDataByID.BufferVal);
								bufferDataByID.BufferVal -= (long)Global.GMin((int)bufferDataByID.BufferVal, num2);
								num += client.ClientData.CurrentLifeV;
								client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, num);
								roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
								GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
							}
						}
					}
				}
				if (client.ClientData.CurrentMagicV < client.ClientData.MagicV)
				{
					if (Global.CanMapUseBuffer(client.ClientData.MapCode, 5))
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(client, 5);
						if (null != bufferDataByID)
						{
							if (bufferDataByID.BufferVal > 0L)
							{
								flag = true;
								roleRelifeLog.mpModify = true;
								roleRelifeLog.oldMp = client.ClientData.CurrentMagicV;
								int num3 = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
								int num4 = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MagicRecoverNum", -1));
								num4 = (int)((double)num4 * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client)));
								num3 = Global.GMin(num3, num4);
								num3 = (int)Global.GMin((long)num3, bufferDataByID.BufferVal);
								bufferDataByID.BufferVal -= (long)((int)Global.GMin(bufferDataByID.BufferVal, (long)num4));
								num3 += client.ClientData.CurrentMagicV;
								client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, num3);
								roleRelifeLog.newMp = client.ClientData.CurrentMagicV;
								GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
							}
						}
					}
				}
				SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				if (flag)
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public static int ProcessHuZhaoSubLifeV(GameClient client, int subLifeV)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 97))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 97);
					if (null != bufferDataByID)
					{
						if (bufferDataByID.BufferVal > 0L)
						{
							int l = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
							HuZhaoBufferItem huZhaoBufferItem = client.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
							if (huZhaoBufferItem != null)
							{
								l = Global.GMin(l, huZhaoBufferItem.InjuredV);
								l = Global.GMin(l, (int)bufferDataByID.BufferVal);
								bufferDataByID.BufferVal -= (long)Global.GMin((int)bufferDataByID.BufferVal, huZhaoBufferItem.InjuredV);
								subLifeV = Global.GMin(l, subLifeV);
								GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
							}
						}
						else
						{
							Global.RemoveBufferData(client, 97);
							client.MyBufferExtManager.RemoveBufferItem(97);
						}
					}
				}
			}
			return subLifeV;
		}

		public static double ProcessHuZhaoRecoverPercent(GameClient client)
		{
			double result = 0.0;
			if (client.ClientData.CurrentLifeV > 0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 97))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 97);
					if (null != bufferDataByID)
					{
						if (bufferDataByID.BufferVal > 0L)
						{
							HuZhaoBufferItem huZhaoBufferItem = client.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
							if (huZhaoBufferItem != null)
							{
								result = huZhaoBufferItem.RecoverLifePercent;
							}
						}
						else
						{
							Global.RemoveBufferData(client, 97);
							client.MyBufferExtManager.RemoveBufferItem(97);
						}
					}
				}
			}
			return result;
		}

		public static int ProcessWuDiHuZhaoNoInjured(GameClient client, int subLifeV)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 98))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 98);
					if (null != bufferDataByID)
					{
						long num = TimeUtil.NOW();
						if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							subLifeV = 0;
						}
						else
						{
							Global.RemoveBufferData(client, 98);
						}
					}
				}
			}
			return subLifeV;
		}

		public static void ProcessTimeAddLifeMagic(GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 27))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 27);
					if (null != bufferDataByID)
					{
						long num3 = TimeUtil.NOW();
						if (num3 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							int goodsID = (int)bufferDataByID.BufferVal;
							List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsID);
							if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == MagicActionIDs.DB_TIME_LIFE_MAGIC)
							{
								if (num3 - client.ClientData.StartAddLifeMagicTicks >= (long)((int)(magicActionListByGoodsID[0].MagicActionParams[3] * 1000.0)))
								{
									client.ClientData.StartAddLifeMagicTicks = num3;
									num = magicActionListByGoodsID[0].MagicActionParams[0];
									num = (double)((int)(num * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client))));
									num2 = magicActionListByGoodsID[0].MagicActionParams[1];
									num2 = (double)((int)(num2 * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client))));
								}
							}
						}
					}
				}
				RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血加蓝buff" + 27);
				if (num > 0.0)
				{
					roleRelifeLog.hpModify = true;
					roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
					int num4 = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
					num4 = Global.GMin(num4, (int)num);
					num = (double)num4;
					num4 += client.ClientData.CurrentLifeV;
					client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, num4);
					roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
				}
				if (num2 > 0.0)
				{
					roleRelifeLog.mpModify = true;
					roleRelifeLog.oldMp = client.ClientData.CurrentMagicV;
					int num5 = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
					num5 = Global.GMin(num5, (int)num2);
					num2 = (double)num5;
					num5 += client.ClientData.CurrentMagicV;
					client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, num5);
					roleRelifeLog.newMp = client.ClientData.CurrentMagicV;
				}
				SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				if (num > 0.0 || num2 > 0.0)
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public static void ProcessTimeAddLifeNoShow(GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 37))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 37);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							int goodsID = (int)bufferDataByID.BufferVal;
							List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsID);
							if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == MagicActionIDs.DB_TIME_LIFE_NOSHOW)
							{
								if (num2 - client.ClientData.StartAddLifeNoShowTicks >= (long)((int)(magicActionListByGoodsID[0].MagicActionParams[2] * 1000.0)))
								{
									double num3 = 0.0;
									client.ClientData.StartAddLifeNoShowTicks = num2;
									num = magicActionListByGoodsID[0].MagicActionParams[0];
									num3 += RoleAlgorithm.GetLifeRecoverAddPercentV(client);
									if (1000L == bufferDataByID.BufferVal || 1001L == bufferDataByID.BufferVal || 1002L == bufferDataByID.BufferVal || 1100L == bufferDataByID.BufferVal || 1101L == bufferDataByID.BufferVal || 1102L == bufferDataByID.BufferVal)
									{
										num3 += client.ClientData.PropsCacheManager.GetExtProp(87);
									}
									num = (double)((int)(num * (1.0 + num3)));
								}
							}
						}
					}
				}
				if (num > 0.0)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血buff" + 37);
					roleRelifeLog.hpModify = true;
					roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
					int num4 = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
					num4 = Global.GMin(num4, (int)num);
					num = (double)num4;
					num4 += client.ClientData.CurrentLifeV;
					client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, num4);
					roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				}
				if (num > 0.0)
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public static void ProcessTimeAddMagicNoShow(GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 38))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 38);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							int goodsID = (int)bufferDataByID.BufferVal;
							List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsID);
							if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == MagicActionIDs.DB_TIME_MAGIC_NOSHOW)
							{
								if (num2 - client.ClientData.StartAddMaigcNoShowTicks >= (long)((int)(magicActionListByGoodsID[0].MagicActionParams[2] * 1000.0)))
								{
									client.ClientData.StartAddMaigcNoShowTicks = num2;
									num = magicActionListByGoodsID[0].MagicActionParams[0];
									num = (double)((int)(num * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client))));
								}
							}
						}
					}
				}
				if (num > 0.0)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加蓝buff" + 38);
					roleRelifeLog.mpModify = true;
					roleRelifeLog.oldMp = client.ClientData.CurrentMagicV;
					int num3 = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
					num3 = Global.GMin(num3, (int)num);
					num = (double)num3;
					num3 += client.ClientData.CurrentMagicV;
					client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, num3);
					roleRelifeLog.newMp = client.ClientData.CurrentMagicV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				}
				if (num > 0.0)
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public static void ProcessDSTimeAddLifeNoShow(GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 40))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 40);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							int num3 = (int)(bufferDataByID.BufferVal >> 32 & (long)((ulong)-1));
							int num4 = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
							if (num2 - client.ClientData.DSStartDSAddLifeNoShowTicks >= (long)(num3 * 1000))
							{
								client.ClientData.DSStartDSAddLifeNoShowTicks = num2;
								num = (double)num4;
								num = (double)((int)(num * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client))));
							}
						}
					}
				}
				if (num > 0.0)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血buff" + 40);
					roleRelifeLog.hpModify = true;
					roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
					int num5 = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
					num5 = Global.GMin(num5, (int)num);
					num = (double)num5;
					num5 += client.ClientData.CurrentLifeV;
					client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, num5);
					roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				}
				if (num > 0.0)
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public static void ProcessLingLiVReserve(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 10))
			{
				if (client.ClientData.InterPower < 30000)
				{
					int num = 30000 - client.ClientData.InterPower;
					if (num > 0)
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(client, 10);
						if (null != bufferDataByID)
						{
							if (bufferDataByID.BufferVal > 0L)
							{
								int num2 = (int)Global.GMin((long)num, bufferDataByID.BufferVal);
								bufferDataByID.BufferVal -= (long)num2;
								client.ClientData.InterPower += num2;
								GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
								GameManager.ClientMgr.NotifyUpdateInterPowerCmd(sl, pool, client, 0);
							}
						}
					}
				}
			}
		}

		public static void ProcessLingLiVReserve2(SocketListener sl, TCPOutPacketPool pool, GameClient client, BufferData bufferData)
		{
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 10))
			{
				if (client.ClientData.InterPower < 30000)
				{
					int num = 30000 - client.ClientData.InterPower;
					if (num > 0)
					{
						if (null != bufferData)
						{
							if (bufferData.BufferVal > 0L)
							{
								int num2 = (int)Global.GMin((long)num, bufferData.BufferVal);
								bufferData.BufferVal -= (long)num2;
								client.ClientData.InterPower += num2;
								GameManager.ClientMgr.NotifyUpdateInterPowerCmd(sl, pool, client, 0);
							}
						}
					}
				}
			}
		}

		public static double ProcessDblLingLi(GameClient client)
		{
			double num = 1.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 3))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 3);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 2.0;
					}
					result = num;
				}
			}
			return result;
		}

		public static void RefreshTimePropBuffer(GameClient client, BufferItemTypes bufferItemType)
		{
			BufferData bufferData = Global.GetBufferDataFromDict(client, (int)bufferItemType);
			if (null == bufferData)
			{
				bufferData = Global.GetBufferDataByID(client, (int)bufferItemType);
				if (null != bufferData)
				{
					long num = TimeUtil.NOW();
					if (num - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
					{
						Global.AddBufferDataIntoDict(client, bufferData.BufferID, bufferData);
						client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
						{
							2
						});
					}
				}
			}
			else
			{
				long num = TimeUtil.NOW();
				if (num - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
				{
					Global.AddBufferDataIntoDict(client, bufferData.BufferID, null);
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						2
					});
				}
			}
		}

		public static int GetTimeAddProp(GameClient client, BufferItemTypes bufferItemType)
		{
			int num = 0;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)bufferItemType);
			int result;
			if (null == bufferDataByID)
			{
				result = num;
			}
			else
			{
				long num2 = TimeUtil.NOW();
				if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
				{
					if (bufferDataByID.BufferID == 52 || bufferDataByID.BufferID == 53 || bufferDataByID.BufferID == 54 || bufferDataByID.BufferID == 55)
					{
						int num3 = (int)bufferDataByID.BufferVal;
						if (num3 <= 0)
						{
							return num;
						}
						List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(num3);
						int num4 = (int)magicActionListByGoodsID[0].MagicActionParams[0];
						if (num4 > -1)
						{
							num = num4;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetBuffAddProp(GameClient client, BufferItemTypes bufferItemType)
		{
			int num = 0;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)bufferItemType);
			int result;
			if (null == bufferDataByID)
			{
				result = num;
			}
			else
			{
				if (bufferDataByID.BufferID == 52 || bufferDataByID.BufferID == 53 || bufferDataByID.BufferID == 54 || bufferDataByID.BufferID == 55)
				{
					int num2 = (int)bufferDataByID.BufferVal;
					if (num2 <= 0)
					{
						return num;
					}
					List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(num2);
					int num3 = (int)magicActionListByGoodsID[0].MagicActionParams[0];
					if (num3 > -1)
					{
						num = num3;
					}
				}
				result = num;
			}
			return result;
		}

		public static double ProcessTimeAddProp(GameClient client, BufferItemTypes bufferItemType)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, (int)bufferItemType))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)bufferItemType);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						int num3 = (int)bufferDataByID.BufferVal;
						List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(num3);
						num = magicActionListByGoodsID[0].MagicActionParams[0];
					}
					else if (bufferDataByID.BufferID == 52 || bufferDataByID.BufferID == 53 || bufferDataByID.BufferID == 54 || bufferDataByID.BufferID == 55)
					{
						long num4 = 0L;
						lock (bufferDataByID)
						{
							num2 = TimeUtil.NOW();
							if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
							{
								return num;
							}
							num4 = bufferDataByID.BufferVal;
							bufferDataByID.BufferVal = 0L;
							if (num4 <= 0L)
							{
								return num;
							}
							Global.RemoveBufferData(client, (int)bufferItemType);
						}
						int num3 = (int)num4;
						if (num3 <= 0)
						{
							return num;
						}
						client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
						{
							default(DelayExecProcIds),
							2
						});
					}
					result = num;
				}
			}
			return result;
		}

		public static double ProcessTempBufferProp(GameClient client, ExtPropIndexes expPropIndex)
		{
			return 0.0;
		}

		public static double ProcessAddTempAttack(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 6))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 6);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 0.1;
					}
					result = num;
				}
			}
			return result;
		}

		public static void AddAttackBuffer(GameClient client)
		{
			if (null == client.ClientData.AddTempAttackBufferData)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 6);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						client.ClientData.AddTempAttackBufferData = bufferDataByID;
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
			}
		}

		public static void RemoveAttackBuffer(GameClient client)
		{
			BufferData addTempAttackBufferData = client.ClientData.AddTempAttackBufferData;
			if (null != addTempAttackBufferData)
			{
				long num = TimeUtil.NOW();
				if (num - addTempAttackBufferData.StartTime >= (long)addTempAttackBufferData.BufferSecs * 1000L)
				{
					client.ClientData.AddTempAttackBufferData = null;
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
			}
		}

		public static double ProcessAddTempDefense(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 7))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 7);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 0.1;
					}
					result = num;
				}
			}
			return result;
		}

		public static void AddDefenseBuffer(GameClient client)
		{
			if (null == client.ClientData.AddTempDefenseBufferData)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 7);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						client.ClientData.AddTempDefenseBufferData = bufferDataByID;
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
			}
		}

		public static void RemoveDefenseBuffer(GameClient client)
		{
			BufferData addTempDefenseBufferData = client.ClientData.AddTempDefenseBufferData;
			if (null != addTempDefenseBufferData)
			{
				long num = TimeUtil.NOW();
				if (num - addTempDefenseBufferData.StartTime >= (long)addTempDefenseBufferData.BufferSecs * 1000L)
				{
					client.ClientData.AddTempDefenseBufferData = null;
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
			}
		}

		public static double ProcessUpLifeLimit(GameClient client)
		{
			double num = 0.0;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 8);
			double result;
			if (null == bufferDataByID)
			{
				result = num;
			}
			else
			{
				long num2 = TimeUtil.NOW();
				if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
				{
					num = 0.1;
				}
				result = num;
			}
			return result;
		}

		public static void AddUpLifeLimitStatus(GameClient client)
		{
			if (null == client.ClientData.UpLifeLimitBufferData)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 8);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						client.ClientData.UpLifeLimitBufferData = bufferDataByID;
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		public static void RemoveUpLifeLimitStatus(GameClient client)
		{
			BufferData upLifeLimitBufferData = client.ClientData.UpLifeLimitBufferData;
			if (null != upLifeLimitBufferData)
			{
				long num = TimeUtil.NOW();
				if (num - upLifeLimitBufferData.StartTime >= (long)upLifeLimitBufferData.BufferSecs * 1000L)
				{
					client.ClientData.UpLifeLimitBufferData = null;
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		public static double ProcessRebornMultiExperience(GameClient client)
		{
			double result = 1.0;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 123))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 123);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						result = (double)((int)(bufferDataByID.BufferVal & (long)((ulong)-1)));
					}
				}
			}
			return result;
		}

		public static double ProcessDblAndThreeExperience(GameClient client)
		{
			double num = 1.0;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 36))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 36);
				if (null != bufferDataByID)
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 5.0;
					}
				}
			}
			if (num < 5.0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 18))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 18);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							num = 3.0;
						}
					}
				}
			}
			if (num < 3.0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 1))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 1);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							num = 2.0;
						}
					}
				}
			}
			if (num <= 1.0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 46))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 46);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							num = (double)((int)(bufferDataByID.BufferVal & (long)((ulong)-1)));
						}
					}
				}
			}
			return num;
		}

		public static double ProcessDblAndThreeMoney(GameClient client)
		{
			double num = 1.0;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 19))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 19);
				if (null != bufferDataByID)
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 3.0;
					}
				}
			}
			if (num < 3.0)
			{
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 2))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 2);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							num = 2.0;
						}
					}
				}
			}
			return num;
		}

		public static double ProcessAutoGiveExperience(GameClient client)
		{
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 21))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 21);
				if (null != bufferDataByID)
				{
					long num4 = TimeUtil.NOW();
					if (num4 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num2 = (int)bufferDataByID.BufferVal;
						List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(num2);
						if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == MagicActionIDs.DB_ADD_EXP)
						{
							if (num4 - client.ClientData.StartAddExpTicks >= (long)((int)(magicActionListByGoodsID[0].MagicActionParams[2] * 1000.0)))
							{
								client.ClientData.StartAddExpTicks = num4;
								num = magicActionListByGoodsID[0].MagicActionParams[0];
							}
						}
						num3 = (int)(((long)bufferDataByID.BufferSecs * 1000L - (num4 - bufferDataByID.StartTime)) / 1000L);
					}
				}
			}
			if (num > 0.0)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, (long)((int)num), true, false, false, "none");
				if (num2 > 0)
				{
					string msgText = string.Format(GLang.GetLang(107, new object[0]), Global.GetGoodsNameByID(num2), num, num3 / 60);
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
			return num;
		}

		public static void ProcessWaWaGiveExperience(GameClient client, Monster monster)
		{
			if (monster.MonsterInfo.VLevel >= client.ClientData.Level)
			{
				double num = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 28))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 28);
					if (null != bufferDataByID)
					{
						if (bufferDataByID.BufferVal > 0L)
						{
							bufferDataByID.BufferVal -= 1L;
							int bufferSecs = bufferDataByID.BufferSecs;
							bool flag = false;
							List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(bufferSecs);
							if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == MagicActionIDs.DB_ADD_WAWA_EXP)
							{
								int num2 = (int)magicActionListByGoodsID[0].MagicActionParams[0];
								if (num2 > 0)
								{
									if (0L == bufferDataByID.BufferVal % (long)num2)
									{
										num = (double)client.ClientData.Level * (magicActionListByGoodsID[0].MagicActionParams[2] + (double)Global.GetRandomNumber(0, (int)magicActionListByGoodsID[0].MagicActionParams[3]));
										GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
										flag = true;
									}
								}
							}
							if (!flag)
							{
								if (bufferDataByID.BufferVal <= 0L)
								{
									GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
								}
							}
						}
					}
				}
				if (num > 0.0)
				{
					double num3 = 1.0;
					if (SpecailTimeManager.JugeIsDoulbeExperienceAndLingli())
					{
						num3 += 1.0;
					}
					num = (double)((int)(num * num3));
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)((int)num), true, false, false, "none");
					Global.NotifySelfWaWaExp(client, (int)num);
				}
			}
		}

		public static long ProcessZhuFuGiveExperience(GameClient client)
		{
			long result = 0L;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 29))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 29);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						result = ((long)bufferDataByID.BufferSecs * 1000L - (num - bufferDataByID.StartTime)) / 1000L;
					}
				}
			}
			return result;
		}

		public static long ProcessErGuoTouGiveExperience(GameClient client, long subTicks, out double multiExpNum)
		{
			multiExpNum = 0.0;
			long result = 0L;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 48))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 48);
				if (null != bufferDataByID)
				{
					if (bufferDataByID.BufferSecs > 0)
					{
						multiExpNum = (double)(bufferDataByID.BufferVal & (long)((ulong)-1)) - 1.0;
						bufferDataByID.BufferSecs = Math.Max(0, bufferDataByID.BufferSecs - (int)(subTicks / 1000L));
						result = (long)bufferDataByID.BufferSecs;
						GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
					}
				}
			}
			return result;
		}

		public static double ProcessDblSkillUp(GameClient client)
		{
			double num = 1.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 17))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 17);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 2.0;
					}
					result = num;
				}
			}
			return result;
		}

		public static int ProcessAntiBoss(GameClient client, Monster monster, int injuredVal)
		{
			int result;
			if (monster.MonsterType != 401 && monster.MonsterType != 301)
			{
				result = injuredVal;
			}
			else if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 11))
			{
				result = injuredVal;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 11);
				if (null == bufferDataByID)
				{
					result = injuredVal;
				}
				else
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime >= (long)bufferDataByID.BufferSecs * 1000L)
					{
						result = injuredVal;
					}
					else
					{
						result = injuredVal * (int)bufferDataByID.BufferVal;
					}
				}
			}
			return result;
		}

		public static int ProcessAntiRole(GameClient client, GameClient otherClient, int injuredVal)
		{
			int result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 12))
			{
				result = injuredVal;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 12);
				if (null == bufferDataByID)
				{
					result = injuredVal;
				}
				else
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime >= (long)bufferDataByID.BufferSecs * 1000L)
					{
						result = injuredVal;
					}
					else
					{
						result = injuredVal + (int)((double)bufferDataByID.BufferVal / 100.0 * (double)injuredVal);
					}
				}
			}
			return result;
		}

		public static double ProcessMonthVIP(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 13))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 13);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						num = 1.0;
					}
					result = num;
				}
			}
			return result;
		}

		public static bool ProcessAutoFightingProtect(GameClient client)
		{
			bool result = false;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 20))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 20);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						result = true;
					}
				}
			}
			return result;
		}

		public static bool ProcessFallTianSheng(GameClient client)
		{
			bool result = false;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, 30))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 30);
				if (null != bufferDataByID)
				{
					long num = TimeUtil.NOW();
					if (num - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						int randomNumber = Global.GetRandomNumber(0, 101);
						if ((long)randomNumber <= bufferDataByID.BufferVal)
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static void ProcessGuMu(GameClient client, long elapseTicks)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 34);
			if (bufferDataByID != null)
			{
				if (bufferDataByID.StartTime == (long)TimeUtil.NowDateTime().DayOfYear && bufferDataByID.BufferVal > 0L)
				{
					bufferDataByID.BufferVal = Math.Max(0L, bufferDataByID.BufferVal - elapseTicks / 1000L);
				}
				else if (bufferDataByID.BufferSecs > 0)
				{
					bufferDataByID.BufferSecs = (int)Math.Max(0L, (long)bufferDataByID.BufferSecs - elapseTicks / 1000L);
				}
				GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
			}
			if (bufferDataByID == null || Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				if (bufferDataByID != null)
				{
				}
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "GuMuAwardDayID");
				if (dayOfYear == roleParamsInt32FromDB)
				{
					if (client.CheckCheatData.LastNotifyLeaveGuMuTick == 0L)
					{
						client.CheckCheatData.LastNotifyLeaveGuMuTick = TimeUtil.NOW() * 10000L;
					}
					else if (TimeUtil.NOW() * 10000L - client.CheckCheatData.LastNotifyLeaveGuMuTick > 600000000L)
					{
						Global.ForceCloseClient(client, "超时未离开古墓地图", true);
					}
					GameManager.LuaMgr.GotoMap(client, GameManager.MainMapCode, -1, -1, -1);
				}
				else
				{
					if (client.CheckCheatData.LastNotifyLeaveGuMuTick > 0L)
					{
						client.CheckCheatData.LastNotifyLeaveGuMuTick = 0L;
					}
					Global.GiveGuMuTimeLimitAward(client);
				}
			}
		}

		public static void ProcessMingJieBuffer(GameClient client, long elapseTicks)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 35);
			if (bufferDataByID != null)
			{
				bufferDataByID.BufferVal -= elapseTicks / 1000L;
				GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
			}
			if (bufferDataByID == null || bufferDataByID.BufferVal <= -6L)
			{
				if (bufferDataByID != null)
				{
					Global.RemoveBufferData(client, 35);
				}
				GameManager.LuaMgr.GotoMap(client, GameManager.MainMapCode, -1, -1, -1);
			}
		}

		public static double ProcessTimeAddPkKingAttackProp(GameClient client, ExtPropIndexes attackType)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 39))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 39);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						int equipID = (int)bufferDataByID.BufferVal;
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(equipID);
						if (null != equipPropItem)
						{
							num += equipPropItem.ExtProps[(int)attackType];
						}
					}
					result = num;
				}
			}
			return result;
		}

		public static double ProcessTimeAddPkKingExpProp(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 39))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 39);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					long num2 = TimeUtil.NOW();
					if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						int goodsID = (int)bufferDataByID.BufferVal;
						List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsID);
						num = magicActionListByGoodsID[0].MagicActionParams[3];
					}
					num -= 1.0;
					num = Global.GMax(0.0, num);
					result = num;
				}
			}
			return result;
		}

		public static double ProcessTimeAddJunQiProp(GameClient client, ExtPropIndexes attackType)
		{
			return 0.0;
		}

		public static void ProcessDSTimeSubLifeNoShow(GameClient client)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, 42))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 42);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							int num3 = (int)(bufferDataByID.BufferVal >> 32 & (long)((ulong)-1));
							int num4 = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
							if (num2 - client.ClientData.DSStartDSSubLifeNoShowTicks >= (long)(num3 * 1000))
							{
								client.ClientData.DSStartDSSubLifeNoShowTicks = num2;
								num = (double)num4;
							}
						}
						else
						{
							Global.RemoveBufferData(client, 42);
						}
					}
				}
				if (num > 0.0)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.FangDuRoleID);
					if (null != gameClient)
					{
						int attackType = Global.CalcOriginalOccupationID(gameClient);
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, client, 0, (int)num, 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, 1.0, 0, 0, 0, 0.0);
						if (client.ClientData.CurrentLifeV <= 0)
						{
							Global.RemoveBufferData(client, 42);
						}
					}
				}
			}
		}

		private static void ProcessTimeSubLifeNoShow(GameClient client, int id)
		{
			if (client.ClientData.CurrentLifeV > 0)
			{
				double num = 0.0;
				DelayInjuredBufferItem delayInjuredBufferItem = null;
				if (Global.CanMapUseBuffer(client.ClientData.MapCode, id))
				{
					BufferData bufferDataByID = Global.GetBufferDataByID(client, id);
					if (null != bufferDataByID)
					{
						long num2 = TimeUtil.NOW();
						if (num2 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
						{
							delayInjuredBufferItem = (client.MyBufferExtManager.FindBufferItem(id) as DelayInjuredBufferItem);
							if (null != delayInjuredBufferItem)
							{
								if (num2 - delayInjuredBufferItem.StartSubLifeNoShowTicks >= (long)(delayInjuredBufferItem.TimeSlotSecs * 1000))
								{
									delayInjuredBufferItem.StartSubLifeNoShowTicks = num2;
									num = (double)delayInjuredBufferItem.SubLifeV;
								}
							}
						}
						else
						{
							Global.RemoveBufferData(client, id);
							client.MyBufferExtManager.RemoveBufferItem(id);
						}
					}
				}
				if (num > 0.0 && null != delayInjuredBufferItem)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(delayInjuredBufferItem.ObjectID);
					if (null != gameClient)
					{
						int attackType = Global.CalcOriginalOccupationID(gameClient);
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, client, 0, (int)num, 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, 1.0, 0, 0, 0, 0.0);
						if (client.ClientData.CurrentLifeV <= 0)
						{
							Global.RemoveBufferData(client, id);
							client.MyBufferExtManager.RemoveBufferItem(id);
						}
					}
					else
					{
						Global.RemoveBufferData(client, id);
						client.MyBufferExtManager.RemoveBufferItem(id);
					}
				}
			}
		}

		public static void ProcessAllTimeSubLifeNoShow(GameClient client)
		{
			for (int i = 93; i <= 96; i++)
			{
				DBRoleBufferManager.ProcessTimeSubLifeNoShow(client, i);
			}
		}

		public static double ProcessSpecialAttackValueBuff(GameClient client, int BufferTypes)
		{
			double num = 0.0;
			double result;
			if (!Global.CanMapUseBuffer(client.ClientData.MapCode, BufferTypes))
			{
				result = num;
			}
			else
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, BufferTypes);
				if (null == bufferDataByID)
				{
					result = num;
				}
				else
				{
					int num2;
					switch (BufferTypes)
					{
					case 64:
						num2 = 202;
						break;
					case 65:
						num2 = 203;
						break;
					case 66:
						num2 = 204;
						break;
					default:
						return num;
					}
					long num3 = TimeUtil.NOW();
					if (num3 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						int goodsID = (int)bufferDataByID.BufferVal;
						List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsID);
						if (magicActionListByGoodsID != null && magicActionListByGoodsID[0].MagicActionID == (MagicActionIDs)num2)
						{
							num = magicActionListByGoodsID[0].MagicActionParams[0];
						}
					}
					result = num;
				}
			}
			return result;
		}
	}
}
