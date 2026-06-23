using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class PassiveSkillModule
	{
		private void TryTriggerSkills(GameClient client, long nowTicks, SkillTriggerTypes type)
		{
			lock (this.mutex)
			{
				foreach (PassiveSkillData passiveSkillData in this.passiveSkillList.Values)
				{
					if (passiveSkillData.triggerType == (int)type)
					{
						long num;
						bool flag2 = this._spanTimeDict.TryGetValue(passiveSkillData.skillId, out num);
						if (!flag2 || num <= nowTicks)
						{
							int randomNumber = Global.GetRandomNumber(0, 100);
							if (randomNumber < passiveSkillData.triggerRate)
							{
								long num2;
								flag2 = this.coolDownDict.TryGetValue(passiveSkillData.skillId, out num2);
								if (!flag2 || num2 <= nowTicks)
								{
									this.coolDownDict[passiveSkillData.skillId] = nowTicks + (long)(passiveSkillData.coolDown * 1000);
									this._spanTimeDict[passiveSkillData.skillId] = nowTicks + (long)(passiveSkillData.triggerCD * 1000);
									int posX = client.ClientData.PosX;
									int posY = client.ClientData.PosY;
									SpriteAttack.AddDelayMagic(client, client.ClientData.RoleID, posX, posY, posX, posY, passiveSkillData.skillId);
									EventLogManager.AddRoleSkillEvent(client, SkillLogTypes.PassiveSkillTrigger, LogRecordType.IntValue2, new object[]
									{
										passiveSkillData.skillId,
										passiveSkillData.skillLevel,
										passiveSkillData.triggerRate,
										randomNumber,
										passiveSkillData.coolDown
									});
								}
							}
						}
					}
				}
			}
		}

		public void OnInjured(GameClient client)
		{
			long num = TimeUtil.NOW();
			if (num > this.NextTriggerSkillForInjuredTicks)
			{
				this.TryTriggerSkills(client, num, SkillTriggerTypes.Injured);
				this.NextTriggerSkillForInjuredTicks = num + 1000L;
			}
		}

		public void OnProcessMagic(GameClient client, int enemy, int enemyX, int enemyY)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.Attack);
		}

		public void OnKillMonster(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			this.TryTriggerSkills(client, nowTicks, SkillTriggerTypes.KillMonster);
		}

		public void UpdateSkillList(List<PassiveSkillData> skillList)
		{
			if (null != skillList)
			{
				lock (this.mutex)
				{
					this.passiveSkillList.Clear();
					foreach (PassiveSkillData passiveSkillData in skillList)
					{
						this.passiveSkillList[passiveSkillData.skillId] = new PassiveSkillData(passiveSkillData.skillId, passiveSkillData.skillLevel, passiveSkillData.triggerType, passiveSkillData.triggerRate, passiveSkillData.coolDown, passiveSkillData.triggerCD);
					}
					foreach (KeyValuePair<int, PassiveSkillData> keyValuePair in this.OtherPassiveSkillList)
					{
						this.passiveSkillList[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
		}

		public void UpdateOtherSkillList(List<PassiveSkillData> skillList)
		{
			if (null != skillList)
			{
				lock (this.mutex)
				{
					foreach (KeyValuePair<int, PassiveSkillData> keyValuePair in this.OtherPassiveSkillList)
					{
						this.passiveSkillList.Remove(keyValuePair.Key);
					}
					this.OtherPassiveSkillList.Clear();
					foreach (PassiveSkillData passiveSkillData in skillList)
					{
						this.OtherPassiveSkillList[passiveSkillData.skillId] = new PassiveSkillData(passiveSkillData.skillId, passiveSkillData.skillLevel, passiveSkillData.triggerType, passiveSkillData.triggerRate, passiveSkillData.coolDown, passiveSkillData.triggerCD);
						this.passiveSkillList[passiveSkillData.skillId] = new PassiveSkillData(passiveSkillData.skillId, passiveSkillData.skillLevel, passiveSkillData.triggerType, passiveSkillData.triggerRate, passiveSkillData.coolDown, passiveSkillData.triggerCD);
					}
				}
			}
		}

		public void UpdateSkillData(int magicCode, int level, int triggerType, int triggerRate, int coolDown, int spanCD)
		{
			lock (this.mutex)
			{
				PassiveSkillData passiveSkillData;
				if (this.passiveSkillList.TryGetValue(magicCode, out passiveSkillData))
				{
					passiveSkillData.skillLevel = level;
					passiveSkillData.triggerRate = triggerRate;
					passiveSkillData.coolDown = coolDown;
					passiveSkillData.triggerType = triggerType;
					passiveSkillData.triggerCD = spanCD;
				}
			}
		}

		public SkillData GetSkillData(int magicCode)
		{
			lock (this.mutex)
			{
				PassiveSkillData passiveSkillData;
				if (this.passiveSkillList.TryGetValue(magicCode, out passiveSkillData))
				{
					return passiveSkillData.skillData;
				}
			}
			return null;
		}

		private object mutex = new object();

		public Dictionary<int, PassiveSkillData> passiveSkillList = new Dictionary<int, PassiveSkillData>();

		private Dictionary<int, long> coolDownDict = new Dictionary<int, long>();

		private Dictionary<int, long> _spanTimeDict = new Dictionary<int, long>();

		public Dictionary<int, PassiveSkillData> OtherPassiveSkillList = new Dictionary<int, PassiveSkillData>();

		public long NextTriggerSkillForInjuredTicks;

		public SkillData currentSkillData = new SkillData();
	}
}
