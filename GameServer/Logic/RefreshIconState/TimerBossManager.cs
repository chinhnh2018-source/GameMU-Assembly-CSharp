using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.RefreshIconState
{
	public class TimerBossManager
	{
		private TimerBossManager()
		{
		}

		private void LoadWorldBossInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Activity/BossInfo.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("Boss");
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							SystemXmlItem systemXmlItem = new SystemXmlItem
							{
								XMLNode = xelement
							};
							TimerBossData timerBossData = new TimerBossData();
							timerBossData.nRoleID = systemXmlItem.GetIntValue("ID", -1);
							int[] intArrayValue = systemXmlItem.GetIntArrayValue("Level", ',');
							if (intArrayValue == null || intArrayValue.Length != 2)
							{
								throw new Exception(string.Format("启动时加载xml文件: {0} 失败 Level格式错误", string.Format("Config/Activity/BossInfo.xml", new object[0])));
							}
							timerBossData.nReqLevel = intArrayValue[1];
							timerBossData.nReqChangeLiveCount = intArrayValue[0];
							this.m_WorldBossDict.Add(timerBossData.nRoleID, timerBossData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Activity/BossInfo.xml", new object[0])));
			}
		}

		private void LoadHuangJinBossInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/HuangJin.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("Boss");
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							SystemXmlItem systemXmlItem = new SystemXmlItem
							{
								XMLNode = xelement
							};
							TimerBossData timerBossData = new TimerBossData();
							timerBossData.nRoleID = systemXmlItem.GetIntValue("ID", -1);
							int[] intArrayValue = systemXmlItem.GetIntArrayValue("Level", ',');
							if (intArrayValue == null || intArrayValue.Length != 2)
							{
								throw new Exception(string.Format("启动时加载xml文件: {0} 失败 Level格式错误", string.Format("Config/HuangJin.xml", new object[0])));
							}
							timerBossData.nReqLevel = intArrayValue[1];
							timerBossData.nReqChangeLiveCount = intArrayValue[0];
							this.m_HuangJinBossDict.Add(timerBossData.nRoleID, timerBossData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/HuangJin.xml", new object[0])));
			}
		}

		public static TimerBossManager getInstance()
		{
			if (null == TimerBossManager.instance)
			{
				lock (TimerBossManager.Mutex)
				{
					if (null == TimerBossManager.instance)
					{
						TimerBossManager timerBossManager = new TimerBossManager();
						timerBossManager.LoadWorldBossInfo();
						timerBossManager.LoadHuangJinBossInfo();
						TimerBossManager.instance = timerBossManager;
					}
				}
			}
			return TimerBossManager.instance;
		}

		public void AddBoss(int nBirthType, int nRoleID)
		{
			lock (this.m_LivedInMapBoss)
			{
				this.m_LivedInMapBoss[nRoleID] = nBirthType;
			}
			if (nBirthType == 1)
			{
				this.RefreshWorldBoss();
			}
			else if (nBirthType == 7)
			{
				this.RefreshHuangJinBoss();
			}
		}

		public void RemoveBoss(int nRoleID)
		{
			int num = 0;
			lock (this.m_LivedInMapBoss)
			{
				if (!this.m_LivedInMapBoss.TryGetValue(nRoleID, out num))
				{
					return;
				}
				this.m_LivedInMapBoss.Remove(nRoleID);
			}
			if (num == 1)
			{
				this.RefreshWorldBoss();
			}
			else if (num == 7)
			{
				this.RefreshHuangJinBoss();
			}
		}

		public bool HaveWorldBoss(GameClient client)
		{
			lock (this.m_LivedInMapBoss)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.m_LivedInMapBoss)
				{
					if (keyValuePair.Value == 1)
					{
						TimerBossData timerBossData = null;
						if (this.m_WorldBossDict.TryGetValue(keyValuePair.Key, out timerBossData))
						{
							if (null != timerBossData)
							{
								if ((client.ClientData.ChangeLifeCount == timerBossData.nReqChangeLiveCount && client.ClientData.Level >= timerBossData.nReqLevel) || client.ClientData.ChangeLifeCount > timerBossData.nReqChangeLiveCount)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public bool HaveHuangJinBoss(GameClient client)
		{
			lock (this.m_LivedInMapBoss)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.m_LivedInMapBoss)
				{
					if (keyValuePair.Value == 7)
					{
						TimerBossData timerBossData = null;
						if (this.m_HuangJinBossDict.TryGetValue(keyValuePair.Key, out timerBossData))
						{
							if (null != timerBossData)
							{
								if ((client.ClientData.ChangeLifeCount == timerBossData.nReqChangeLiveCount && client.ClientData.Level >= timerBossData.nReqLevel) || client.ClientData.ChangeLifeCount > timerBossData.nReqChangeLiveCount)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public void RefreshHuangJinBoss()
		{
			int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < maxClientCount; i++)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
				if (null != gameClient)
				{
					gameClient._IconStateMgr.CheckHuangJinBoss(gameClient);
					gameClient._IconStateMgr.SendIconStateToClient(gameClient);
				}
			}
		}

		public void RefreshWorldBoss()
		{
			int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < maxClientCount; i++)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
				if (null != gameClient)
				{
					gameClient._IconStateMgr.CheckShiJieBoss(gameClient);
					gameClient._IconStateMgr.SendIconStateToClient(gameClient);
				}
			}
		}

		private Dictionary<int, TimerBossData> m_WorldBossDict = new Dictionary<int, TimerBossData>();

		private Dictionary<int, TimerBossData> m_HuangJinBossDict = new Dictionary<int, TimerBossData>();

		private Dictionary<int, int> m_LivedInMapBoss = new Dictionary<int, int>();

		private static TimerBossManager instance = null;

		private static object Mutex = new object();
	}
}
