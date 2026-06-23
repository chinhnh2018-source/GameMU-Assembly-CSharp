using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
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
				GlobalEventSource.getInstance().removeListener(14, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(41, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(12, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(38, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(39, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(40, AssemblyPatchManager.getInstance());
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

		private bool CheckVersion(string cur, string max)
		{
			bool result;
			if (string.IsNullOrEmpty(cur))
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(max))
			{
				result = true;
			}
			else
			{
				string[] array = cur.Split(new char[]
				{
					'.'
				});
				string[] array2 = max.Split(new char[]
				{
					'.'
				});
				result = (array.Length == array2.Length && array.Length == 4 && (!(array[0] != array2[0]) && !(array[1] != array2[1])) && !(array[2] != array2[2]) && array[3].CompareTo(array2[3]) <= 0);
			}
			return result;
		}

		public bool InitConfig()
		{
			string text = "AssemblyPatch.xml";
			XElement xelement = ConfigHelper.Load(text);
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
						string defAttributeStr = Global.GetDefAttributeStr(xml, "VersionID", "99.9.9.999999");
						string versionStr = Program.GetVersionStr();
						if (!this.CheckVersion(versionStr, defAttributeStr))
						{
							Program.DeleteFile("AssemblyPatch.xml");
							Program.DeleteFile("AssemblyPatch.dll");
							Program.DeleteFile("DotNetDetour.dll");
							Program.DeleteFile("dlls\\AssemblyPatch.dll");
							Program.DeleteFile("dlls\\DotNetDetour.dll");
							return true;
						}
						MethodConfig methodConfig = new MethodConfig();
						Enum.TryParse<EventTypes>(Global.GetDefAttributeStr(xml, "Type", ""), true, out methodConfig.eventType);
						methodConfig.assemblyName = Global.GetDefAttributeStr(xml, "AssemblyName", "");
						methodConfig.fullClassName = Global.GetDefAttributeStr(xml, "fullClassName", "");
						methodConfig.methodName = Global.GetDefAttributeStr(xml, "methodName", "");
						methodConfig.methodParams = Global.GetDefAttributeStr(xml, "methodParams", "").Split(new char[]
						{
							','
						});
						methodConfig.cmdID = (int)Global.GetSafeAttributeLong(xml, "CmdID");
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
					LogManager.WriteLog(1000, "加载AssemblyPatch时文件出错", ex, true);
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
				LogManager.WriteLog(2, string.Format("PatchMgr::CheckMethod Error, AssemblyName={0}, fullClassName={1}, methodName={2}", cfg.assemblyName, cfg.fullClassName, cfg.methodName), ex, false);
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
				LogManager.WriteLog(2, string.Format("PatchMgr::AddLoader Error, AssemblyName={0}", AssemblyName), null, true);
			}
		}

		public void processEvent(EventObject eventObject)
		{
			List<MethodConfig> list = null;
			if (this.patchCfgDict.TryGetValue((EventTypes)eventObject.getEventType(), out list))
			{
				if (list != null && list.Count > 0)
				{
					GameClient gameClient = null;
					if (eventObject.getEventType() == 14)
					{
						PlayerInitGameEventObject playerInitGameEventObject = (PlayerInitGameEventObject)eventObject;
						gameClient = playerInitGameEventObject.getPlayer();
					}
					else if (eventObject.getEventType() == 41)
					{
						PlayerLoginGameEventObject playerLoginGameEventObject = (PlayerLoginGameEventObject)eventObject;
						gameClient = playerLoginGameEventObject.getPlayer();
					}
					else if (eventObject.getEventType() == 12)
					{
						PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
						gameClient = playerLogoutEventObject.getPlayer();
					}
					else if (eventObject.getEventType() == 38)
					{
						PlayerOnlineEventObject playerOnlineEventObject = (PlayerOnlineEventObject)eventObject;
						gameClient = playerOnlineEventObject.getPlayer();
					}
					foreach (MethodConfig methodConfig in list)
					{
						AssemblyLoader assemblyLoader = this.GetAssemblyLoader(methodConfig.assemblyName);
						if (null != assemblyLoader)
						{
							object[] array;
							if (null != gameClient)
							{
								int num = 1;
								array = new object[methodConfig.methodParams.Length + num];
								array[0] = gameClient;
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

		public TCPProcessCmdResults ProcessMsg(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
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
							array[0] = tcpMgr;
							array[1] = socket;
							array[2] = tcpClientPool;
							array[3] = tcpRandKey;
							array[4] = pool;
							array[5] = nID;
							array[6] = data;
							array[7] = count;
							for (int i = 0; i < methodConfig.methodParams.Length; i++)
							{
								array[i + num] = methodConfig.methodParams[i];
							}
							TcpResult tcpResult = (TcpResult)assemblyLoader.Invoke(methodConfig.fullClassName, methodConfig.methodName, array);
							tcpOutPacket = tcpResult.outPacket;
							return tcpResult.cmdResult;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("AssemblyPatchMgr::ProcessMsg Error nID={0}", nID), null, true);
			}
			return TCPProcessCmdResults.RESUTL_CONTINUE;
		}

		private static AssemblyPatchManager instance = new AssemblyPatchManager();

		private Dictionary<EventTypes, List<MethodConfig>> patchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();

		private bool[] CmdRegisteredFlags = new bool[32768];

		private Dictionary<string, AssemblyLoader> patchDict = new Dictionary<string, AssemblyLoader>();
	}
}
