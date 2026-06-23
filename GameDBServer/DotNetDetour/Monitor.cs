using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetDetour
{
	public class Monitor
	{
		public static void InstallEx(params Assembly[] assemblies)
		{
			lock (Monitor.Mutex)
			{
				List<DestAndOri> list = new List<DestAndOri>();
				List<IMethodMonitor> list2 = new List<IMethodMonitor>();
				foreach (Assembly assembly in assemblies)
				{
					list2.AddRange(assembly.GetImplementedObjectsByInterface<IMethodMonitor>());
				}
				foreach (IMethodMonitor methodMonitor in list2)
				{
					DestAndOri destAndOri = new DestAndOri();
					destAndOri.Dest = methodMonitor.GetType().GetMethods().FirstOrDefault((MethodInfo t) => t.GetCustomAttributes(typeof(MonitorAttribute), false).Length > 0);
					destAndOri.Ori = methodMonitor.GetType().GetMethods().FirstOrDefault((MethodInfo t) => t.GetCustomAttributes(typeof(OriginalAttribute), false).Length > 0);
					if (!(destAndOri.Dest == null) && !(destAndOri.Ori == null))
					{
						MethodInfo methodInfo = null;
						MethodInfo dest = destAndOri.Dest;
						MonitorAttribute monitorAttribute = dest.GetCustomAttributes(typeof(MonitorAttribute), false).FirstOrDefault<object>() as MonitorAttribute;
						string name = dest.Name;
						Type[] types = (from t in dest.GetParameters()
						select t.ParameterType).ToArray<Type>();
						if (monitorAttribute.Type != null)
						{
							methodInfo = monitorAttribute.Type.GetMethod(name, types);
						}
						else
						{
							string srcNamespaceAndClass = monitorAttribute.NamespaceName + "." + monitorAttribute.ClassName;
							foreach (Assembly assembly2 in assemblies)
							{
								Type type = assembly2.GetType(srcNamespaceAndClass);
								if (type == null)
								{
									type = assembly2.GetExportedTypes().FirstOrDefault((Type t) => t.FullName == srcNamespaceAndClass);
								}
								if (type != null)
								{
									methodInfo = type.GetMethod(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
									break;
								}
							}
						}
						if (!(methodInfo == null))
						{
							destAndOri.Src = methodInfo;
							if (destAndOri.Dest != null)
							{
								List<DestAndOri> list3;
								if (!Monitor.destAndOrisDict.TryGetValue(destAndOri.Src, out list3))
								{
									list3 = new List<DestAndOri>();
									Monitor.destAndOrisDict[destAndOri.Src] = list3;
								}
								if (!list3.Contains(destAndOri))
								{
									if (list3.Count > 0)
									{
										destAndOri.Src = list3.LastOrDefault<DestAndOri>().Dest;
									}
									list3.Add(destAndOri);
									list.Add(destAndOri);
								}
							}
						}
					}
				}
				Monitor.InstallInternalEx(list);
			}
		}

		private static void InstallInternalEx(List<DestAndOri> destAndOris)
		{
			foreach (DestAndOri destAndOri in destAndOris)
			{
				MethodInfo src = destAndOri.Src;
				MethodInfo dest = destAndOri.Dest;
				MethodInfo ori = destAndOri.Ori;
				IDetour detour = DetourFactory.CreateDetourEngine();
				detour.Patch(src, dest, ori);
			}
		}

		private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
		}

		private static object Mutex = new object();

		private static Dictionary<MethodInfo, List<DestAndOri>> destAndOrisDict = new Dictionary<MethodInfo, List<DestAndOri>>();
	}
}
