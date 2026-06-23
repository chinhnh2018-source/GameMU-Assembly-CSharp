using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	public class MagicCoolDownMgr
	{
		public bool SkillCoolDown(int skillID)
		{
			CoolDownItem coolDownItem = null;
			bool result;
			if (!this.SkillCoolDownDict.TryGetValue(skillID, out coolDownItem))
			{
				result = true;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num > coolDownItem.StartTicks + coolDownItem.CDTicks);
			}
			return result;
		}

		public void AddSkillCoolDown(IObject attacker, int skillID)
		{
			if (attacker is GameClient)
			{
				this.AddSkillCoolDownForClient(attacker as GameClient, skillID);
			}
			else if (attacker is Monster)
			{
				this.AddSkillCoolDownForMonster(attacker as Monster, skillID);
			}
		}

		public void AddSkillCoolDownForClient(GameClient client, int skillID)
		{
			SystemXmlItem systemXmlItem = null;
			if (GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(skillID, out systemXmlItem))
			{
				long num = TimeUtil.NOW();
				int num2 = Global.GMax(0, systemXmlItem.GetIntValue("CDTime", -1));
				int num3 = Global.GMax(0, systemXmlItem.GetIntValue("PubCDTime", -1));
				if (num2 <= 0)
				{
					int intValue = systemXmlItem.GetIntValue("ParentMagicID", -1);
					if (intValue > 0)
					{
						if (GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(intValue, out systemXmlItem))
						{
							num2 = Global.GMax(0, systemXmlItem.GetIntValue("CDTime", -1));
						}
					}
				}
				long num4 = Data.MaxServerClientTimeDiff;
				if (client.ClientData.CurrentMagicCode == skillID)
				{
					num4 = num - client.ClientData.CurrentMagicTicks;
				}
				if (num2 > 0)
				{
					num2 *= 1000;
					if (client.ClientData.CurrentMagicCode == skillID && client.ClientData.CurrentMagicCDSubPercent > 0.0)
					{
						num2 = Convert.ToInt32((double)num2 * (1.0 - client.ClientData.CurrentMagicCDSubPercent));
						num2 = (int)Math.Max((long)num2, num4);
						int intValue2 = systemXmlItem.GetIntValue("NextMagicID", -1);
						if (intValue2 <= 0)
						{
							client.ClientData.CurrentMagicCDSubPercent = 0.0;
						}
					}
					Global.AddCoolDownItem(this.SkillCoolDownDict, skillID, num, (long)num2 - num4);
					if (systemXmlItem.GetStringValue("HorseSkill") == "1")
					{
						ExtData clientExtData = ExtDataManager.GetClientExtData(client);
						clientExtData.ZuoQiSkillCDTicks = num + (long)num2 - num4;
						clientExtData.ZuoQiSkillCdTime = (long)num2 - num4;
					}
				}
				if (num3 > 0)
				{
					client.ClientData.CurrentMagicActionEndTicks = num - num4 + (long)num3;
					if (null != client.ClientData.SkillDataList)
					{
						for (int i = 0; i < client.ClientData.SkillDataList.Count; i++)
						{
							SkillData skillData = client.ClientData.SkillDataList[i];
							if (null != skillData)
							{
								Global.AddCoolDownItem(this.SkillCoolDownDict, skillData.SkillID, num, (long)num3 - num4);
							}
						}
					}
				}
			}
		}

		public void AddSkillCoolDownForMonster(Monster monster, int skillID)
		{
			SystemXmlItem systemXmlItem = null;
			if (GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(skillID, out systemXmlItem))
			{
				int intValue = systemXmlItem.GetIntValue("CDTime", -1);
				if (intValue > 0)
				{
					int intValue2 = systemXmlItem.GetIntValue("PubCDTime", -1);
					long startTicks = TimeUtil.NOW();
					Global.AddCoolDownItem(this.SkillCoolDownDict, skillID, startTicks, (long)(intValue * 1000));
					if (null != monster.MonsterInfo.SkillIDs)
					{
						for (int i = 0; i < monster.MonsterInfo.SkillIDs.Length; i++)
						{
							if (intValue2 > 0)
							{
								Global.AddCoolDownItem(this.SkillCoolDownDict, monster.MonsterInfo.SkillIDs[i], startTicks, (long)intValue2);
							}
						}
					}
				}
			}
		}

		private Dictionary<int, CoolDownItem> SkillCoolDownDict = new Dictionary<int, CoolDownItem>();
	}
}
