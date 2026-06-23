using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class WarnManager : IManager
	{
		public static WarnManager getInstance()
		{
			return WarnManager.instance;
		}

		public bool initialize()
		{
			WarnManager.initWarnInfo();
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("WarnManager.WarnCloseClient()", new EventHandler(WarnManager.WarnCloseClient)), 5000, 5000);
			return true;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public static void initWarnInfo()
		{
			string text = Global.IsolateResPath("Config/JingGao.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					WarnManager._warnInfoList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							WarnInfo warnInfo = new WarnInfo();
							warnInfo.ID = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "ID"));
							warnInfo.Desc = Global.GetSafeAttributeStr(xelement2, "Description");
							warnInfo.TimeSec = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "Time"));
							warnInfo.Operate = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "Operate"));
							WarnManager._warnInfoList.Add(warnInfo.Operate, warnInfo);
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		private static WarnInfo GetWarnInfo(int warnType)
		{
			WarnInfo result;
			if (WarnManager._warnInfoList.ContainsKey(warnType))
			{
				result = WarnManager._warnInfoList[warnType];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static void WarnProcess(string userID, int warnType)
		{
			WarnInfo warnInfo = WarnManager.GetWarnInfo(warnType);
			if (warnInfo != null)
			{
				TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
				if (tmsksocket != null)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
					if (null != gameClient)
					{
						gameClient.sendCmd<WarnInfo>(1004, warnInfo, false);
						if (warnInfo.Operate != 1)
						{
							WarnManager.AddTaskToHashSet(gameClient, warnInfo.TimeSec);
						}
					}
				}
			}
		}

		private static void AddTaskToHashSet(GameClient client, int time)
		{
			lock (WarnManager._lock)
			{
				if (!WarnManager._clientList.ContainsKey(client))
				{
					WarnManager._clientList.Add(client, TimeUtil.NowDateTime().AddSeconds((double)time));
					if (WarnManager._clientList.Count >= 3000)
					{
						WarnManager.WarnCloseClient(null, null);
					}
				}
			}
		}

		public static void WarnCloseClient(object sender, EventArgs e)
		{
			try
			{
				Dictionary<GameClient, DateTime> dictionary = new Dictionary<GameClient, DateTime>();
				List<GameClient> list = new List<GameClient>();
				lock (WarnManager._lock)
				{
					foreach (KeyValuePair<GameClient, DateTime> keyValuePair in WarnManager._clientList)
					{
						DateTime value = keyValuePair.Value;
						if (TimeUtil.NowDateTime() >= value)
						{
							list.Add(keyValuePair.Key);
						}
						else
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					WarnManager._clientList.Clear();
					WarnManager._clientList = dictionary;
				}
				foreach (GameClient client in list)
				{
					Global.ForceCloseClient(client, "warn踢人", true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, ex.Message, null, true);
			}
		}

		private static WarnManager instance = new WarnManager();

		private static Dictionary<int, WarnInfo> _warnInfoList = new Dictionary<int, WarnInfo>();

		private static object _lock = new object();

		private static Dictionary<GameClient, DateTime> _clientList = new Dictionary<GameClient, DateTime>();
	}
}
