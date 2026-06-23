using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameDBServer.Core.GameEvent;
using GameDBServer.Logic;
using GameDBServer.Server;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Core.AssemblyPatch
{
	public class AssemblyPatchManager : IManager, IEventListener
	{
		public static AssemblyPatchManager getInstance()
		{
			return AssemblyPatchManager.instance;
		}

		public bool initialize()
		{
			bool result;
			if (!this.InitConfig())
			{
				result = true;
			}
			else
			{
				GlobalEventSource.getInstance().removeListener(0, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(1, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(2, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(3, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(4, AssemblyPatchManager.getInstance());
				List<EventTypes> list = this.patchCfgDict.Keys.ToList<EventTypes>();
				foreach (EventTypes eventType in list)
				{
					GlobalEventSource.getInstance().registerListener((int)eventType, AssemblyPatchManager.getInstance());
				}
				result = true;
			}
			return result;
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

		public bool InitConfig()
		{
			string fileName = "AssemblyPatch.xml";
			XElement xelement = ConfigHelper.Load(fileName);
			bool result;
			if (null == xelement)
			{
				result = false;
			}
			else
			{
				try
				{
					Dictionary<EventTypes, List<MethodConfig>> dictionary = new Dictionary<EventTypes, List<MethodConfig>>();
					bool[] array = new bool[32768];
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						MethodConfig methodConfig = new MethodConfig();
						Enum.TryParse<EventTypes>(ConfigHelper.GetElementAttributeValue(xml, "Type", ""), true, out methodConfig.eventType);
						methodConfig.assemblyName = ConfigHelper.GetElementAttributeValue(xml, "AssemblyName", "");
						methodConfig.fullClassName = ConfigHelper.GetElementAttributeValue(xml, "fullClassName", "");
						methodConfig.methodName = ConfigHelper.GetElementAttributeValue(xml, "methodName", "");
						methodConfig.methodParams = ConfigHelper.GetElementAttributeValue(xml, "methodParams", "").Split(new char[]
						{
							','
						});
						methodConfig.cmdID = (int)ConfigHelper.GetElementAttributeValueLong(xml, "CmdID", 0L);
						if (this.CheckMethod(methodConfig))
						{
							array[methodConfig.cmdID] = true;
							if (dictionary.ContainsKey(methodConfig.eventType))
							{
								dictionary[methodConfig.eventType].Add(methodConfig);
							}
							else
							{
								List<MethodConfig> list = new List<MethodConfig>();
								list.Add(methodConfig);
								dictionary.Add(methodConfig.eventType, list);
							}
						}
					}
					this.patchCfgDict = dictionary;
					this.CmdRegisteredFlags = array;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Exception, "加载AssemblyPatch时文件出错\r\n" + ex.ToString(), null, true);
				}
				result = true;
			}
			return result;
		}

		private bool CheckMethod(MethodConfig cfg)
		{
			bool result;
			try
			{
				AssemblyLoader assemblyLoader = this.GetAssemblyLoader(cfg.assemblyName);
				if (null == assemblyLoader)
				{
					assemblyLoader = new AssemblyLoader();
					if (!assemblyLoader.LoadAssembly(cfg.assemblyName))
					{
						return false;
					}
					this.AddAssemblyLoader(cfg.assemblyName, assemblyLoader);
				}
				result = assemblyLoader.LoadMethod(cfg.fullClassName, cfg.methodName);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("PatchMgr::CheckMethod Error, AssemblyName={0}, fullClassName={1}, methodName={2}\r\n", cfg.assemblyName, cfg.fullClassName, cfg.methodName) + ex.ToString(), null, true);
				result = false;
			}
			return result;
		}

		private AssemblyLoader GetAssemblyLoader(string AssemblyName)
		{
			AssemblyLoader result;
			if (!this.patchDict.ContainsKey(AssemblyName))
			{
				result = null;
			}
			else
			{
				result = this.patchDict[AssemblyName];
			}
			return result;
		}

		private void AddAssemblyLoader(string AssemblyName, AssemblyLoader loader)
		{
			try
			{
				this.patchDict.Add(AssemblyName, loader);
			}
			catch
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("PatchMgr::AddLoader Error, AssemblyName={0}", AssemblyName), null, true);
			}
		}

		public void processEvent(EventObject eventObject)
		{
			List<MethodConfig> list = null;
			if (this.patchCfgDict.TryGetValue((EventTypes)eventObject.getEventType(), out list))
			{
				if (list != null && list.Count > 0)
				{
					foreach (MethodConfig methodConfig in list)
					{
						AssemblyLoader assemblyLoader = this.GetAssemblyLoader(methodConfig.assemblyName);
						if (null != assemblyLoader)
						{
							GameServerClient gameServerClient = null;
							object[] array;
							if (null != gameServerClient)
							{
								int num = 1;
								array = new object[methodConfig.methodParams.Length + num];
								array[0] = gameServerClient;
								for (int i = 0; i < methodConfig.methodParams.Length; i++)
								{
									array[i + num] = methodConfig.methodParams[i];
								}
							}
							else
							{
								array = new object[methodConfig.methodParams.Length];
								for (int i = 0; i < methodConfig.methodParams.Length; i++)
								{
									array[i] = methodConfig.methodParams[i];
								}
							}
							assemblyLoader.Invoke(methodConfig.fullClassName, methodConfig.methodName, array);
						}
					}
				}
			}
		}

		public bool IfNeedMonMsg()
		{
			return this.patchCfgDict.ContainsKey(EventTypes.BeforeProcessMsg);
		}

		public TCPProcessCmdResults ProcessMsg(GameServerClient client, int nID, byte[] data, int count)
		{
			try
			{
				if (!this.CmdRegisteredFlags[nID])
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				List<MethodConfig> list = null;
				if (!this.patchCfgDict.TryGetValue(EventTypes.BeforeProcessMsg, out list))
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				if (list == null || list.Count <= 0)
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				foreach (MethodConfig methodConfig in list)
				{
					if (methodConfig.cmdID == nID)
					{
						AssemblyLoader assemblyLoader = this.GetAssemblyLoader(methodConfig.assemblyName);
						if (null != assemblyLoader)
						{
							int num = 8;
							object[] array = new object[methodConfig.methodParams.Length + num];
							array[0] = client;
							array[1] = nID;
							array[2] = data;
							array[3] = count;
							for (int i = 0; i < methodConfig.methodParams.Length; i++)
							{
								array[i + num] = methodConfig.methodParams[i];
							}
							TcpResult tcpResult = (TcpResult)assemblyLoader.Invoke(methodConfig.fullClassName, methodConfig.methodName, array);
							return tcpResult.cmdResult;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("AssemblyPatchMgr::ProcessMsg Error nID={0}", nID), null, true);
			}
			return TCPProcessCmdResults.RESUTL_CONTINUE;
		}

		private static AssemblyPatchManager instance = new AssemblyPatchManager();

		private Dictionary<EventTypes, List<MethodConfig>> patchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();

		private bool[] CmdRegisteredFlags = new bool[32768];

		private Dictionary<string, AssemblyLoader> patchDict = new Dictionary<string, AssemblyLoader>();
	}
}
