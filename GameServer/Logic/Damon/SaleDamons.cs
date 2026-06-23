using System;
using GameServer.Logic.Goods;
using GameServer.Server;
using Server.Data;

namespace GameServer.Logic.Damon
{
	public class SaleDamons
	{
		public static TCPProcessCmdResults SaleDamonsProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int num = 0;
			string[] array = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < array.Length)
			{
				int dbID = Global.SafeConvertToInt32(array[i]);
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, dbID);
				if (goodsByDbID != null && goodsByDbID.Site == 0 && goodsByDbID.Using <= 0)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByDbID.GoodsID);
					if (goodsCatetoriy >= 9 && goodsCatetoriy <= 10)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem) && null != systemXmlItem)
						{
							string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								4,
								goodsByDbID.Id,
								goodsByDbID.GoodsID,
								0,
								goodsByDbID.Site,
								goodsByDbID.GCount,
								goodsByDbID.BagIndex,
								""
							});
							if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null))
							{
								int intValue = systemXmlItem.GetIntValue("ZhanHunPrice", -1);
								if (intValue > 0)
								{
									num += intValue;
								}
								for (int j = 0; j < goodsByDbID.Forge_level; j++)
								{
									SystemXmlItem systemXmlItem2 = null;
									GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(j + 2, out systemXmlItem2);
									if (null != systemXmlItem2)
									{
										int intValue2 = systemXmlItem2.GetIntValue("NeedEXP", -1);
										if (intValue2 > 0)
										{
											num += intValue2;
										}
									}
								}
								num += (int)PetSkillManager.DelGoodsReturnLingJing(goodsByDbID);
							}
						}
					}
				}
				IL_20D:
				i++;
				continue;
				goto IL_20D;
			}
			if (num > 0)
			{
				GameManager.ClientMgr.ModifyMUMoHeValue(client, num, "一键出售或者回收", true, true, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		public static TCPProcessCmdResults SaleStoreDamonsProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int num = 0;
			string[] array = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < array.Length)
			{
				int id = Global.SafeConvertToInt32(array[i]);
				GoodsData petByDbID = CallPetManager.GetPetByDbID(client, id);
				if (petByDbID != null && petByDbID.Site == 4000 && petByDbID.Using <= 0)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(petByDbID.GoodsID);
					if (goodsCatetoriy >= 9 && goodsCatetoriy <= 10)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(petByDbID.GoodsID, out systemXmlItem) && null != systemXmlItem)
						{
							string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								4,
								petByDbID.Id,
								petByDbID.GoodsID,
								0,
								petByDbID.Site,
								petByDbID.GCount,
								petByDbID.BagIndex,
								""
							});
							if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null))
							{
								int intValue = systemXmlItem.GetIntValue("ZhanHunPrice", -1);
								if (intValue > 0)
								{
									num += intValue;
								}
								for (int j = 0; j < petByDbID.Forge_level; j++)
								{
									SystemXmlItem systemXmlItem2 = null;
									GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(j + 2, out systemXmlItem2);
									if (null != systemXmlItem2)
									{
										int intValue2 = systemXmlItem2.GetIntValue("NeedEXP", -1);
										if (intValue2 > 0)
										{
											num += intValue2;
										}
									}
								}
							}
						}
					}
				}
				IL_207:
				i++;
				continue;
				goto IL_207;
			}
			if (num > 0)
			{
				GameManager.ClientMgr.ModifyMUMoHeValue(client, num, "一键出售或者回收", true, true, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}
	}
}
