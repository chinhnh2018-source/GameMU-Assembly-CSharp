using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	internal class JingJiGetRoleLooksCmdProcessor : ICmdProcessor
	{
		private JingJiGetRoleLooksCmdProcessor()
		{
		}

		public static JingJiGetRoleLooksCmdProcessor getInstance()
		{
			return JingJiGetRoleLooksCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[1]);
			PlayerJingJiData playerJingJiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(num), client.ServerId);
			if (playerJingJiData != null)
			{
				client.sendCmd<RoleData4Selector>(1340, new RoleData4Selector
				{
					RoleID = playerJingJiData.roleId,
					RoleName = playerJingJiData.roleName,
					RoleSex = playerJingJiData.sex,
					Occupation = playerJingJiData.occupationId,
					SubOccupation = playerJingJiData.SubOccupation,
					OccupationList = playerJingJiData.OccupationList,
					Level = playerJingJiData.level,
					MyWingData = playerJingJiData.wingData,
					GoodsDataList = JingJiChangManager.GetUsingGoodsList(playerJingJiData.equipDatas),
					CombatForce = playerJingJiData.combatForce,
					AdmiredCount = playerJingJiData.AdmiredCount,
					SettingBitFlags = playerJingJiData.settingFlags,
					JunTuanId = playerJingJiData.JunTuanId,
					JunTuanName = playerJingJiData.JunTuanName,
					JunTuanZhiWu = playerJingJiData.JunTuanZhiWu,
					LingDi = playerJingJiData.LingDi,
					CompType = playerJingJiData.CompType,
					CompZhiWu = playerJingJiData.CompZhiWu
				}, false);
			}
			return true;
		}

		private static JingJiGetRoleLooksCmdProcessor instance = new JingJiGetRoleLooksCmdProcessor();
	}
}
