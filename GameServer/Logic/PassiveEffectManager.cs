using System;
using System.Collections.Generic;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using Server.Tools;

namespace GameServer.Logic
{
	public class PassiveEffectManager
	{
		public static double GetPassiveEffectAddPercent(IObject attacker, int triggerType, int effectType)
		{
			double num = 0.0;
			try
			{
				List<int> passiveEffectList;
				if (attacker is GameClient)
				{
					passiveEffectList = attacker.PassiveEffectList;
				}
				else
				{
					if (!(attacker is Robot))
					{
						return 0.0;
					}
					passiveEffectList = (attacker as Robot).PassiveEffectList;
				}
				if (null == passiveEffectList)
				{
					return num;
				}
				foreach (int num2 in passiveEffectList)
				{
					PassiveEffectData passiveEffectData;
					if (!PassiveEffectManager.passiveExtDict.TryGetValue(num2, out passiveEffectData))
					{
						SystemXmlItem systemXmlItem;
						if (!GameManager.SystemPassiveMgr.SystemXmlItemDict.TryGetValue(num2, out systemXmlItem))
						{
							continue;
						}
						passiveEffectData = new PassiveEffectData
						{
							skillId = num2,
							triggerRate = systemXmlItem.GetIntValue("Rate", -1),
							triggerType = systemXmlItem.GetIntValue("Type", -1)
						};
						PassiveEffectManager.passiveExtDict[num2] = passiveEffectData;
					}
					if (triggerType == passiveEffectData.triggerType)
					{
						int randomNumber = Global.GetRandomNumber(0, 100);
						if (randomNumber < passiveEffectData.triggerRate)
						{
							List<MagicActionItem> list = null;
							if (!GameManager.SystemPassiveEffectMgr.MagicActionsDict.TryGetValue(num2, out list) || null == list)
							{
								return num;
							}
							foreach (MagicActionItem magicActionItem in list)
							{
								double num3 = 0.0;
								double num4 = 0.0;
								if (magicActionItem.MagicActionParams.Length > 0)
								{
									num3 = magicActionItem.MagicActionParams[0];
								}
								if (magicActionItem.MagicActionParams.Length > 1)
								{
									num4 = magicActionItem.MagicActionParams[1];
								}
								switch (magicActionItem.MagicActionID)
								{
								case MagicActionIDs.MU_ADD_OWN_ELEMENT_DAMAGE:
									if (effectType == 1)
									{
										num += num3;
									}
									break;
								case MagicActionIDs.MU_ADD_OWN_ELEMENT_REDUCTION:
									if (effectType == 2)
									{
										if (attacker is GameClient)
										{
											GameClient gameClient = attacker as GameClient;
											if (Convert.ToInt32((double)gameClient.ClientData.LifeV * num3) >= gameClient.ClientData.CurrentLifeV)
											{
												num += num4;
											}
										}
										else if (attacker is Robot)
										{
											Robot robot = attacker as Robot;
											if (robot.MonsterInfo.VLifeMax * num3 >= robot.VLife)
											{
												num += num4;
											}
										}
									}
									break;
								case MagicActionIDs.MU_REDUCE_TARGET_ELEMENT_REDUCTION:
									if (effectType == 3)
									{
										num += num3;
									}
									break;
								case MagicActionIDs.MU_ADD_OWN_DAMAGE_REBOUND:
									if (effectType == 4)
									{
										num += num3;
									}
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("被动效果获取异常：rid={0},triggerType={1},effectType={2}", 0, triggerType, effectType), null, true);
			}
			return num;
		}

		private object mutex = new object();

		private Dictionary<int, List<MagicActionItem>> _MagicActionsDict = null;

		public static Dictionary<int, PassiveEffectData> passiveExtDict = new Dictionary<int, PassiveEffectData>();
	}
}
