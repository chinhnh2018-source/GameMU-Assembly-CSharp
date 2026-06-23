using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class SpecialEquipMgr
	{
		public static void DoEquipExtAttack(GameClient client, int categoriy, int enemy)
		{
			if (-1 != enemy)
			{
				GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
				if (null != goodsDataByCategoriy)
				{
					List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsDataByCategoriy.GoodsID);
					if (magicActionListByGoodsID != null && magicActionListByGoodsID.Count > 0)
					{
						MagicActionItem magicActionItem = magicActionListByGoodsID[0];
						if (MagicActionIDs.EXT_ATTACK_MABI == magicActionItem.MagicActionID)
						{
							double num = magicActionItem.MagicActionParams[0];
							double num2 = magicActionItem.MagicActionParams[1];
							if ((double)Global.GetRandomNumber(0, 101) <= num)
							{
								int num3 = Global.CalcOriginalOccupationID(client);
								if (0 != num3)
								{
									num *= 0.5;
								}
								if (-1 != enemy)
								{
									GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
									if (spriteType != GSpriteTypes.Monster)
									{
										if (spriteType == GSpriteTypes.Other)
										{
											GameClient gameClient = GameManager.ClientMgr.FindClient(enemy);
											if (null != gameClient)
											{
												gameClient.ClientData.DongJieStart = TimeUtil.NOW();
												gameClient.ClientData.DongJieSeconds = (int)num2;
												GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, 2, gameClient.ClientData.DongJieStart, gameClient.ClientData.DongJieSeconds, 0.0);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void DoEquipRestoreBlood(GameClient client, int categoriy)
		{
			if (client.ClientData.CurrentLifeV <= 0)
			{
				GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
				if (null != goodsDataByCategoriy)
				{
					List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsDataByCategoriy.GoodsID);
					if (magicActionListByGoodsID != null && magicActionListByGoodsID.Count > 0)
					{
						MagicActionItem magicActionItem = magicActionListByGoodsID[0];
						if (MagicActionIDs.EXT_RESTORE_BLOOD == magicActionItem.MagicActionID)
						{
							double num = magicActionItem.MagicActionParams[0];
							if (num * 1000.0 + (double)client.ClientData.SpecialEquipLastUseTicks < (double)TimeUtil.NOW())
							{
								client.ClientData.CurrentLifeV = client.ClientData.LifeV;
								client.ClientData.SpecialEquipLastUseTicks = TimeUtil.NOW();
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(531, new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
				}
			}
		}

		public static int DoSubInJure(GameClient client, int categoriy, int injure)
		{
			GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
			int result;
			if (null == goodsDataByCategoriy)
			{
				result = 0;
			}
			else
			{
				List<MagicActionItem> magicActionListByGoodsID = UsingGoods.GetMagicActionListByGoodsID(goodsDataByCategoriy.GoodsID);
				if (magicActionListByGoodsID == null || magicActionListByGoodsID.Count <= 0)
				{
					result = 0;
				}
				else
				{
					MagicActionItem magicActionItem = magicActionListByGoodsID[0];
					if (MagicActionIDs.EXT_SUB_INJURE == magicActionItem.MagicActionID)
					{
						double num = magicActionItem.MagicActionParams[0];
						double num2 = magicActionItem.MagicActionParams[1];
						if (num <= 0.0 || num2 <= 0.0)
						{
							result = 0;
						}
						else
						{
							num /= 100.0;
							int currentMagicV = client.ClientData.CurrentMagicV;
							int num3 = (int)Math.Min((double)injure * num, (double)currentMagicV / num2);
							int currentMagicV2 = client.ClientData.CurrentMagicV;
							client.ClientData.CurrentMagicV -= (int)(num2 * (double)num3);
							result = Math.Min(num3, injure);
						}
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}
	}
}
