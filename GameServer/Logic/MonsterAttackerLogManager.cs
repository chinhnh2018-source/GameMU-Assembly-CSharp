using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	public class MonsterAttackerLogManager : SingletonTemplate<MonsterAttackerLogManager>, IEventListener
	{
		private MonsterAttackerLogManager()
		{
			GlobalEventSource.getInstance().registerListener(11, this);
		}

		public void LoadRecordMonsters()
		{
			lock (this.Mutex)
			{
				this.NeedRecordLogMonsters.Clear();
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LogAttackBoss", ',');
				int num = 0;
				while (paramValueIntArrayByName != null && num < paramValueIntArrayByName.Length)
				{
					if (!this.NeedRecordLogMonsters.Contains(paramValueIntArrayByName[num]))
					{
						this.NeedRecordLogMonsters.Add(paramValueIntArrayByName[num]);
					}
					num++;
				}
			}
		}

		public bool IsNeedRecordAttackLog(int monsterId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.NeedRecordLogMonsters.Contains(monsterId);
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				if (null != monsterDeadEventObject)
				{
					Monster monster = monsterDeadEventObject.getMonster();
					if (monster != null)
					{
						if (this.IsNeedRecordAttackLog(monster.MonsterInfo.ExtensionID))
						{
							string text = monster.BuildAttackerLog();
							LogManager.WriteLog(6, text, null, true);
						}
					}
				}
			}
		}

		public void AddRoleRelifeLog(RoleRelifeLog log)
		{
			if (log != null)
			{
				if (log.hpModify || log.mpModify)
				{
					bool flag = false;
					lock (this.Mutex)
					{
						flag = this.NeedRecordRelifeRoles.Contains(log.RoleId);
					}
					if (flag)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendFormat("回血日志， roleid={0}, rolename={1}, mapcode={2}, reason={3}", new object[]
						{
							log.RoleId,
							log.Rolename,
							log.MapCode,
							log.Reason
						});
						if (log.hpModify)
						{
							stringBuilder.AppendFormat(" ,oldHp={0}, newHp={1}, addHp={2}", log.oldHp, log.newHp, log.newHp - log.oldHp);
						}
						if (log.mpModify)
						{
							stringBuilder.AppendFormat(" ,oldMp={0}, newMp={1}, addMp={2}", log.oldMp, log.newMp, log.newMp - log.oldMp);
						}
						LogManager.WriteLog(6, stringBuilder.ToString(), null, true);
					}
				}
			}
		}

		public void SetLogRoleRelife(int roleId, bool bLog = true)
		{
			lock (this.Mutex)
			{
				if (bLog && !this.NeedRecordRelifeRoles.Contains(roleId))
				{
					this.NeedRecordRelifeRoles.Add(roleId);
				}
				if (!bLog)
				{
					this.NeedRecordRelifeRoles.Remove(roleId);
				}
			}
		}

		private object Mutex = new object();

		private HashSet<int> NeedRecordLogMonsters = new HashSet<int>();

		private HashSet<int> NeedRecordRelifeRoles = new HashSet<int>();
	}
}
