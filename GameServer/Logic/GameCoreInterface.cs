using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class GameCoreInterface : ICoreInterface
	{
		public static GameCoreInterface getinstance()
		{
			return GameCoreInterface.CoreInterface;
		}

		public int GetNewFuBenSeqId()
		{
			int result = -1;
			try
			{
				string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", 0), 0);
				if (array != null && array.Length >= 2)
				{
					result = Global.SafeConvertToInt32(array[1]);
				}
			}
			catch (Exception ex)
			{
				result = -1;
			}
			return result;
		}

		public int GetLocalServerId()
		{
			return GameManager.ServerId;
		}

		public ISceneEventSource GetEventSourceInterface()
		{
			return GlobalEventSource4Scene.getInstance();
		}

		public string GetGameConfigStr(string name, string defVal)
		{
			return GameManager.GameConfigMgr.GetGameConfigItemStr(name, defVal);
		}

		public PlatformTypes GetPlatformType()
		{
			return GameManager.PlatformType;
		}

		public void SetRuntimeVariable(string name, string val)
		{
			if (null != name)
			{
				lock (this.RuntimeVariableDict)
				{
					this.RuntimeVariableDict[name] = val;
				}
			}
		}

		public string GetRuntimeVariable(string name, string defVal)
		{
			string result;
			if (null == name)
			{
				result = defVal;
			}
			else
			{
				lock (this.RuntimeVariableDict)
				{
					string result2;
					if (this.RuntimeVariableDict.TryGetValue(name, out result2))
					{
						return result2;
					}
				}
				result = defVal;
			}
			return result;
		}

		public int GetRuntimeVariable(string name, int defVal)
		{
			int result;
			if (null == name)
			{
				result = defVal;
			}
			else
			{
				lock (this.RuntimeVariableDict)
				{
					string s;
					if (this.RuntimeVariableDict.TryGetValue(name, out s))
					{
						int result2;
						if (int.TryParse(s, out result2))
						{
							return result2;
						}
					}
				}
				result = defVal;
			}
			return result;
		}

		public string GetLocalAddressIPs()
		{
			return Global.GetLocalAddressIPs();
		}

		public int GetMapClientCount(int mapCode)
		{
			return GameManager.ClientMgr.GetMapClientsCount(mapCode);
		}

		private static GameCoreInterface CoreInterface = new GameCoreInterface();

		private Dictionary<string, string> RuntimeVariableDict = new Dictionary<string, string>();
	}
}
