using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.NewBufferExt;
using Server.Data;

namespace GameServer.Logic
{
	public class DBMonsterBuffer
	{
		public static void ProcessDSTimeAddLifeNoShow(Monster monster)
		{
			if (monster.VLife > 0.0)
			{
				double num = 0.0;
				BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 40);
				if (null != monsterBufferDataByID)
				{
					long num2 = TimeUtil.NOW();
					if (num2 - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
					{
						int num3 = (int)(monsterBufferDataByID.BufferVal >> 32 & (long)((ulong)-1));
						int num4 = (int)(monsterBufferDataByID.BufferVal & (long)((ulong)-1));
						if (num2 - monster.DSStartDSAddLifeNoShowTicks >= (long)(num3 * 1000))
						{
							monster.DSStartDSAddLifeNoShowTicks = num2;
							num = (double)num4;
						}
					}
					else
					{
						Global.RemoveMonsterBufferData(monster, 40);
					}
				}
				if (monster.VLife < monster.MonsterInfo.VLifeMax && num > 0.0)
				{
					num += monster.VLife;
					monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, num);
					List<object> all9Clients = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, all9Clients, 0);
				}
			}
		}

		public static int ProcessHuZhaoSubLifeV(Monster monster, int subLifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
				{
					BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 97);
					if (null != monsterBufferDataByID)
					{
						if (monsterBufferDataByID.BufferVal > 0L)
						{
							long l = (long)(monster.MonsterInfo.VLifeMax - monster.VLife);
							HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
							if (huZhaoBufferItem != null)
							{
								l = Global.GMin(l, (long)huZhaoBufferItem.InjuredV);
								l = (long)((int)Global.GMin(l, (long)((int)monsterBufferDataByID.BufferVal)));
								monsterBufferDataByID.BufferVal -= (long)Global.GMin((int)monsterBufferDataByID.BufferVal, huZhaoBufferItem.InjuredV);
								subLifeV = (int)Global.GMin(l, (long)subLifeV);
							}
						}
						else
						{
							Global.RemoveMonsterBufferData(monster, 97);
							monster.MyBufferExtManager.RemoveBufferItem(97);
							monsterBufferDataByID.BufferSecs = 0;
							monsterBufferDataByID.StartTime = 0L;
							GameManager.ClientMgr.NotifyOtherBufferData(monster, monsterBufferDataByID);
						}
					}
				}
			}
			return subLifeV;
		}

		public static double ProcessHuZhaoRecoverPercent(Monster monster)
		{
			double result = 0.0;
			if (monster.VLife > 0.0)
			{
				if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
				{
					BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 97);
					if (null != monsterBufferDataByID)
					{
						if (monsterBufferDataByID.BufferVal > 0L)
						{
							HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
							if (huZhaoBufferItem != null)
							{
								result = huZhaoBufferItem.RecoverLifePercent;
							}
						}
						else
						{
							Global.RemoveMonsterBufferData(monster, 97);
							monster.MyBufferExtManager.RemoveBufferItem(97);
						}
					}
				}
			}
			return result;
		}

		public static int ProcessWuDiHuZhaoNoInjured(Monster monster, int subLifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (Global.CanMapUseBuffer(monster.CurrentMapCode, 98))
				{
					BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 98);
					if (null != monsterBufferDataByID)
					{
						long num = TimeUtil.NOW();
						if (num - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
						{
							subLifeV = 0;
						}
						else
						{
							Global.RemoveMonsterBufferData(monster, 98);
						}
					}
				}
			}
			return subLifeV;
		}

		public static int ProcessMarriageFubenInjured(Monster monster, int subLifeV)
		{
			if (monster.VLife > 0.0 && subLifeV > 0)
			{
				if (Global.CanMapUseBuffer(monster.CurrentMapCode, 2000808))
				{
					BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 2000808);
					if (null != monsterBufferDataByID)
					{
						subLifeV = (int)((double)subLifeV * ((double)monsterBufferDataByID.BufferVal / 100.0));
					}
				}
			}
			return subLifeV;
		}

		public static void ProcessDSTimeSubLifeNoShow(Monster monster)
		{
			if (monster.VLife > 0.0)
			{
				double num = 0.0;
				BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 42);
				if (null != monsterBufferDataByID)
				{
					long num2 = TimeUtil.NOW();
					if (num2 - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
					{
						int num3 = (int)(monsterBufferDataByID.BufferVal >> 32 & (long)((ulong)-1));
						int num4 = (int)(monsterBufferDataByID.BufferVal & (long)((ulong)-1));
						if (num2 - monster.DSStartDSSubLifeNoShowTicks >= (long)(num3 * 1000))
						{
							monster.DSStartDSSubLifeNoShowTicks = num2;
							num = (double)num4;
						}
					}
					else
					{
						Global.RemoveMonsterBufferData(monster, 42);
					}
				}
				if (num > 0.0)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(monster.FangDuRoleID);
					if (null != gameClient)
					{
						int attackType = Global.CalcOriginalOccupationID(gameClient);
						GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, monster, 0, (int)num, 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						if (monster.VLife <= 0.0)
						{
							Global.RemoveMonsterBufferData(monster, 42);
						}
					}
				}
			}
		}

		private static void ProcessTimeSubLifeNoShow(Monster monster, int id)
		{
			if (monster.VLife > 0.0)
			{
				double num = 0.0;
				DelayInjuredBufferItem delayInjuredBufferItem = null;
				BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, id);
				if (null != monsterBufferDataByID)
				{
					long num2 = TimeUtil.NOW();
					if (num2 - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
					{
						delayInjuredBufferItem = (monster.MyBufferExtManager.FindBufferItem(id) as DelayInjuredBufferItem);
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
						Global.RemoveMonsterBufferData(monster, id);
						monster.MyBufferExtManager.RemoveBufferItem(id);
					}
				}
				if (num > 0.0 && null != delayInjuredBufferItem)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(delayInjuredBufferItem.ObjectID);
					if (null != gameClient)
					{
						int attackType = Global.CalcOriginalOccupationID(gameClient);
						GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, monster, 0, (int)num, 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						if (monster.VLife <= 0.0)
						{
							Global.RemoveMonsterBufferData(monster, id);
							monster.MyBufferExtManager.RemoveBufferItem(id);
						}
					}
					else
					{
						Global.RemoveMonsterBufferData(monster, id);
						monster.MyBufferExtManager.RemoveBufferItem(id);
					}
				}
			}
		}

		public static void ProcessAllTimeSubLifeNoShow(Monster monster)
		{
			for (int i = 93; i <= 96; i++)
			{
				DBMonsterBuffer.ProcessTimeSubLifeNoShow(monster, i);
			}
		}
	}
}
