using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.AoYunDaTi;
using GameServer.Logic.BocaiSys;
using GameServer.Logic.BossAI;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.Damon;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.GoldAuction;
using GameServer.Logic.Goods;
using GameServer.Logic.KuaFuIPStatistics;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.Olympics;
using GameServer.Logic.RefreshIconState;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.TuJian;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Logic.UserReturn;
using GameServer.Logic.YueKa;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using GameServer.Tools;
using GameServer.Tools.CheckSysValue;
using KF.Contract.Data;
using KF.TcpCall;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer
{
	public class Program : IConnectInfoContainer
	{
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(Program.ControlCtrlDelegate HandlerRoutine, bool Add);

		public static bool HandlerRoutine(int CtrlType)
		{
			switch (CtrlType)
			{
			}
			return true;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

		[DllImport("user32.dll")]
		private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		private static void HideCloseBtn()
		{
			Console.Title = "Server_" + Global.GetRandomNumber(0, 100000);
			IntPtr hWnd = Program.FindWindow(null, Console.Title);
			IntPtr systemMenu = Program.GetSystemMenu(hWnd, IntPtr.Zero);
			uint uPosition = 61536U;
			Program.RemoveMenu(systemMenu, uPosition, 0U);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Exception e2 = e.ExceptionObject as Exception;
				DataHelper.WriteFormatExceptionLog(e2, "CurrentDomain_UnhandledException", UnhandedException.ShowErrMsgBox, true);
				if (Program.bDumpAndExit_ServerRunOk)
				{
					if (!Directory.Exists(Program.DumpBaseDir))
					{
						Directory.CreateDirectory(Program.DumpBaseDir);
					}
					SysConOut.WriteLine("");
					SysConOut.WriteLine("I had a problem, and i'm writting `dump` now, please wait for a moment...");
					Process process = Process.Start("C:\\Program Files\\Debugging Tools for Windows (x64)\\adplus.exe", "-hang -o " + Program.DumpBaseDir + " -p " + Process.GetCurrentProcess().Id.ToString());
					process.WaitForExit();
					Thread.Sleep(5000);
				}
			}
			catch
			{
			}
			finally
			{
				if (Program.bDumpAndExit_ServerRunOk)
				{
					Process.GetCurrentProcess().Kill();
					Process.GetCurrentProcess().WaitForExit();
				}
			}
		}

		private static void ExceptionHook()
		{
			AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
		}

		public static void DeleteFile(string strFileName)
		{
			string text = Directory.GetCurrentDirectory() + "\\" + strFileName;
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
				{
					fileInfo.Attributes = FileAttributes.Normal;
				}
				File.Delete(text);
			}
		}

		public static void WritePIDToFile(string strFile)
		{
			string path = Directory.GetCurrentDirectory() + "\\" + strFile;
			Process currentProcess = Process.GetCurrentProcess();
			int id = currentProcess.Id;
			File.WriteAllText(path, string.Concat(id));
		}

		public static int GetServerPIDFromFile()
		{
			string path = Directory.GetCurrentDirectory() + "\\GameServerStop.txt";
			int result;
			if (File.Exists(path))
			{
				string s = File.ReadAllText(path);
				result = int.Parse(s);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static void Main(string[] args)
		{
			Program.DeleteFile("Start.txt");
			Program.DeleteFile("Stop.txt");
			Program.DeleteFile("GameServerStop.txt");
			args = Environment.GetCommandLineArgs();
			if (args.Contains("-gmsettime"))
			{
				GMCommands.EnableGMSetAllServerTime = true;
			}
			if (args.Contains("-testmode"))
			{
				Consts.TestMode = true;
			}
			if (!GCSettings.IsServerGC && Environment.ProcessorCount > 2)
			{
				SysConOut.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
				Console.WriteLine("GC模式不正确,禁止启动,尝试自动设置为正确模式");
				string text = Process.GetCurrentProcess().MainModule.FileName + ".config";
				XElement xelement = XElement.Load(text);
				XElement xelement2 = xelement.Element("runtime");
				if (null == xelement2)
				{
					xelement.SetElementValue("runtime", "");
					xelement2 = xelement.Element("runtime");
				}
				xelement2.SetElementValue("gcServer", "");
				xelement2.Element("gcServer").SetAttributeValue("enabled", "true");
				xelement.Save(text);
				Console.WriteLine("自动设置为服务器模式,重新启动即可");
				Console.Read();
			}
			else
			{
				Program.HideCloseBtn();
				Program.SetConsoleCtrlHandler(Program.newDelegate, true);
				if (Console.WindowWidth < 88)
				{
					Console.BufferWidth = 88;
					Console.WindowWidth = 88;
				}
				Program.ExceptionHook();
				TimeUtil.Init();
				Program.InitCommonCmd();
				Global.CheckCodes();
				Program.OnStartServer();
				Program.ShowCmdHelpInfo(null);
				Program.WritePIDToFile("Start.txt");
				Program.bDumpAndExit_ServerRunOk = false;
				Thread thread = new Thread(new ParameterizedThreadStart(Program.ConsoleInputThread));
				thread.IsBackground = true;
				thread.Start();
				while (!Program.NeedExitServer || !Program.ServerConsole.MustCloseNow || Program.ServerConsole.MainDispatcherWorker.IsBusy)
				{
					Thread.Sleep(1000);
				}
				thread.Abort();
				Process.GetCurrentProcess().Kill();
			}
		}

		public static void ConsoleInputThread(object obj)
		{
			while (!Program.NeedExitServer)
			{
				string text = Console.ReadLine();
				if (!string.IsNullOrEmpty(text))
				{
					if (text != null && 0 == text.CompareTo("exit"))
					{
						SysConOut.WriteLine("确认退出吗(输入 y 将立即退出)？");
						text = Console.ReadLine();
						if (0 == text.CompareTo("y"))
						{
							break;
						}
					}
					Program.ParseInputCmd(text);
				}
			}
			Program.OnExitServer();
		}

		private static void ParseInputCmd(string cmd)
		{
			Program.CmdCallback cmdCallback = null;
			int num = cmd.IndexOf('/');
			string key = cmd;
			if (num > 0)
			{
				key = cmd.Substring(0, num - 1).TrimEnd(new char[0]);
			}
			if (Program.CmdDict.TryGetValue(key, out cmdCallback) && null != cmdCallback)
			{
				cmdCallback(cmd);
			}
			else
			{
				SysConOut.WriteLine("未知命令,输入 help 查看具体命令信息");
			}
		}

		private static void OnStartServer()
		{
			Program.ServerConsole.InitServer();
			Console.Title = string.Format("游戏服务器{0}线@{1}@{2}", GameManager.ServerLineID, Program.GetVersionDateTime(), Program.ProgramExtName);
		}

		private static void OnExitServer()
		{
			Program.ServerConsole.ExitServer();
		}

		public static void Exit()
		{
			Program.NeedExitServer = true;
		}

		private static void InitCommonCmd()
		{
			Program.CmdDict.Add("help", new Program.CmdCallback(Program.ShowCmdHelpInfo));
			Program.CmdDict.Add("gc", new Program.CmdCallback(Program.GarbageCollect));
			Program.CmdDict.Add("show dbconnect", new Program.CmdCallback(Program.ShowDBConnectInfo));
			Program.CmdDict.Add("show baseinfo", new Program.CmdCallback(Program.ShowServerBaseInfo));
			Program.CmdDict.Add("show tcpinfo", new Program.CmdCallback(Program.ShowServerTCPInfo));
			Program.CmdDict.Add("show copymapinfo", new Program.CmdCallback(Program.ShowCopyMapInfo));
			Program.CmdDict.Add("show gcinfo", new Program.CmdCallback(Program.ShowGCInfo));
			Program.CmdDict.Add("show roleinfo", new Program.CmdCallback(Program.ShowRoleInfo));
			Program.CmdDict.Add("list copymap", new Program.CmdCallback(Program.ListCopyMap));
			Program.CmdDict.Add("write map", new Program.CmdCallback(CheckSysValueHelper.WriteMap));
			Program.CmdDict.Add("check val", new Program.CmdCallback(CheckSysValueHelper.GetValue));
			Program.CmdDict.Add("testmode 5", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testmode 1", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testmode 0", new Program.CmdCallback(Program.SetTestMode));
			Program.CmdDict.Add("testkf 0", new Program.CmdCallback(S2KFCommunication.stop));
			Program.CmdDict.Add("testkf 1", new Program.CmdCallback(S2KFCommunication.start));
			Program.CmdDict.Add("patch", new Program.CmdCallback(Program.RunPatchFromConsole));
			Program.CmdDict.Add("show objinfo", new Program.CmdCallback(Program.ShowObjectInfo));
			Program.CmdDict.Add("clear", delegate(string x)
			{
				Console.Clear();
			});
			Program.CmdDict.Add("show magicactions", delegate(string x)
			{
				SystemMagicAction.PrintMaigcActionUsage();
			});
			Program.CmdDict.Add("report", delegate(string x)
			{
				GameManager.ServerMonitor.CheckReport();
			});
		}

		public static void LoadIPList(string strCmd)
		{
			try
			{
				if (string.IsNullOrEmpty(strCmd))
				{
					strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("whiteiplist", "");
				}
				LogManager.WriteLog(2, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", strCmd), null, true);
				bool enabeld = true;
				string[] ipList = strCmd.Split(new char[]
				{
					','
				});
				List<string> list = Global._TCPManager.MySocketListener.InitIPWhiteList(ipList, enabeld);
				if (list.Count > 0)
				{
					Console.WriteLine("IP白名单列表内容如下:");
					foreach (string value in list)
					{
						Console.WriteLine(value);
					}
				}
				else
				{
					Console.WriteLine("IP白名单为空,不限制IP登录");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("读取IP白名单异常,所以不限制IP登录.异常信息:\n" + ex.ToString());
			}
		}

		public static void CalcGCInfo()
		{
			long num = TimeUtil.NOW();
			for (int i = 0; i < 3; i++)
			{
				Program.GCCollectionCounts1[i] = GC.CollectionCount(i);
				if (Program.GCCollectionCounts[i] != 0)
				{
					int num2 = Program.GCCollectionCounts1[i] - Program.GCCollectionCounts[i];
					if (num >= Program.MaxGCCollectionCounts1sTicks[i] + 1000L)
					{
						if (num2 > Program.MaxGCCollectionCounts1s[i])
						{
							Program.MaxGCCollectionCounts1s[i] = num2;
						}
						Program.MaxGCCollectionCounts1sTicks[i] = num;
					}
					if (num >= Program.MaxGCCollectionCounts5sTicks[i] + 5000L)
					{
						if (Program.GCCollectionCounts5[i] != 0)
						{
							int num3 = Program.GCCollectionCounts1[i] - Program.GCCollectionCounts5[i];
							if (num3 > Program.MaxGCCollectionCounts5s[i])
							{
								Program.MaxGCCollectionCounts5s[i] = num3;
							}
						}
						Program.MaxGCCollectionCounts5sTicks[i] = num;
						Program.GCCollectionCounts5[i] = Program.GCCollectionCounts1[i];
					}
					Program.GCCollectionCountsNow[i] = num2;
				}
				Program.GCCollectionCounts[i] = Program.GCCollectionCounts1[i];
			}
		}

		private static void ShowGCInfo(string cmd = null)
		{
			try
			{
				Console.WriteLine(string.Format("GC计数类别    {0,-10} {1,-10} {2,-10}", "0 gen", "1 gen", "2 gen"));
				Console.WriteLine(string.Format("总计GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCounts[0], Program.GCCollectionCounts[1], Program.GCCollectionCounts[2]));
				Console.WriteLine(string.Format("每秒GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCountsNow[0], Program.GCCollectionCountsNow[1], Program.GCCollectionCountsNow[2]));
				Console.WriteLine(string.Format("1秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts1s[0], Program.MaxGCCollectionCounts1s[1], Program.MaxGCCollectionCounts1s[2]));
				Console.WriteLine(string.Format("5秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts5s[0], Program.MaxGCCollectionCounts5s[1], Program.MaxGCCollectionCounts5s[2]));
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ShowGCInfo()", false, false);
			}
		}

		private static void ShowCmdHelpInfo(string cmd = null)
		{
			SysConOut.WriteLine(string.Format("游戏服务器{0}:", GameManager.ServerLineID));
			SysConOut.WriteLine("输入 help， 显示帮助信息");
			SysConOut.WriteLine("输入 exit， 然后输入y退出？");
			SysConOut.WriteLine("输入 gc， 执行垃圾回收");
			SysConOut.WriteLine("输入 show dbconnect， 查看数据库链接信息");
			SysConOut.WriteLine("输入 show baseinfo， 查看基础运行信息");
			SysConOut.WriteLine("输入 show tcpinfo， 查看通讯相关信息");
			SysConOut.WriteLine("输入 show copymapinfo， 查看副本相关信息");
			SysConOut.WriteLine("输入 show gcinfo， 查看GC相关信息");
			SysConOut.WriteLine("输入 write map 绘画静态成员关系图,如果本地无缓存或者调试新加的点击，不然无法模糊查询");
			SysConOut.WriteLine("输入 check val 查询系统变量值");
			SysConOut.WriteLine("输入 testkf 1 开启中心压力测试");
			SysConOut.WriteLine("输入 testkf 0 关闭中心压力测试");
		}

		private static void GarbageCollect(string cmd = null)
		{
			try
			{
				GC.Collect();
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "GarbageCollect()", false, false);
			}
		}

		private static string ReadPasswd()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
				if (consoleKeyInfo.Key == ConsoleKey.Enter)
				{
					break;
				}
				if (Console.CursorLeft > 0)
				{
					Console.CursorLeft--;
					Console.Write("*");
					stringBuilder.Append(consoleKeyInfo.KeyChar);
				}
			}
			return stringBuilder.ToString();
		}

		private static void SetTestMode(string cmd = null)
		{
			if (!string.IsNullOrEmpty(cmd))
			{
				if ("tmsk201405" == Program.ReadPasswd())
				{
					if (cmd.IndexOf("testmode 5") == 0)
					{
						GameManager.TestGamePerformanceMode = true;
						GameManager.TestGamePerformanceAllPK = true;
						Console.WriteLine("开启压测模式,全体PK");
					}
					else if (cmd.IndexOf("testmode 1") == 0)
					{
						GameManager.TestGamePerformanceMode = true;
						GameManager.TestGamePerformanceAllPK = false;
						Console.WriteLine("开启压测模式,和平模式");
					}
					else
					{
						GameManager.TestGamePerformanceMode = false;
						GameManager.TestGamePerformanceAllPK = false;
						Console.WriteLine("关闭压测模式");
					}
				}
			}
		}

		public static void RunPatchFromConsole(string cmd)
		{
			try
			{
				if (!string.IsNullOrEmpty(cmd))
				{
					if (!("tmsk201405" != Program.ReadPasswd()))
					{
						Console.WriteLine("输入补丁信息:");
						string arg = Console.ReadLine();
						Program.RunPatch(arg, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
			}
		}

		public static void RunPatch(string arg, bool console = true)
		{
			try
			{
				if (!string.IsNullOrEmpty(arg))
				{
					if (!string.IsNullOrEmpty(arg))
					{
						char[] separator = new char[]
						{
							' '
						};
						string[] array = arg.Split(separator, StringSplitOptions.RemoveEmptyEntries);
						if (array != null && array.Length >= 3 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]) && !string.IsNullOrEmpty(array[2]))
						{
							string text = DataHelper.CurrentDirectory + array[0];
							if (File.Exists(text))
							{
								Assembly assembly = Assembly.LoadFrom(text);
								if (null != assembly)
								{
									Type type = assembly.GetType(array[1]);
									if (null != type)
									{
										MethodInfo method = type.GetMethod(array[2], BindingFlags.Static | BindingFlags.NonPublic);
										if (null != method)
										{
											object[] parameters = new object[]
											{
												array
											};
											string text2 = (string)method.Invoke(null, parameters);
											LogManager.WriteLog(3, "执行修补程序" + arg + ",结果:" + text2, null, true);
											if (console && text2 != null && text2.Length < 4096)
											{
												Console.WriteLine(text2);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
			}
		}

		public static void ShowObjectInfo(string cmd)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("在线玩家数{0}\n", GameManager.ClientMgr.GetClientCountFromDict());
				stringBuilder.AppendFormat("各地图人数\n{0}", GameManager.ClientMgr.GetAllMapRoleNumStr());
				stringBuilder.AppendFormat("地图对象引用的角色对象数\n{0}", GameManager.MapGridMgr.GetAllMapClientCountForConsole());
				stringBuilder.AppendLine("命令执行结束\n");
				Console.WriteLine(stringBuilder.ToString());
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "执行ShowGameClientInfo异常");
			}
		}

		private static void ShowDBConnectInfo(string cmd = null)
		{
			try
			{
				foreach (KeyValuePair<int, string> keyValuePair in Program.ServerConsole.DBServerConnectDict)
				{
					SysConOut.WriteLine(keyValuePair.Value);
				}
				foreach (KeyValuePair<int, string> keyValuePair in Program.ServerConsole.LogDBServerConnectDict)
				{
					SysConOut.WriteLine(keyValuePair.Value);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ShowDBConnectInfo()", false, false);
			}
		}

		private static void ShowServerBaseInfo(string cmd = null)
		{
			SysConOut.WriteLine(string.Format("在线数量 {0}/{1}", GameManager.ClientMgr.GetClientCount(), Global._TCPManager.MySocketListener.ConnectedSocketsCount));
			int num = 0;
			int num2 = 0;
			ThreadPool.GetMaxThreads(out num, out num2);
			SysConOut.WriteLine(string.Format("线程池信息 workerThreads={0}, completionPortThreads={1}", num, num2));
			SysConOut.WriteLine(string.Format("TCP事件读写缓存数量 readPool={0}/{2}, writePool={1}/{2}", Global._TCPManager.MySocketListener.ReadPoolCount, Global._TCPManager.MySocketListener.WritePoolCount, Global._TCPManager.MySocketListener.numConnections * 3));
			SysConOut.WriteLine(string.Format("数据库指令数量 {0}", GameManager.DBCmdMgr.GetDBCmdCount()));
			SysConOut.WriteLine(string.Format("与DbServer的连接数量{0}/{1}", Global._TCPManager.tcpClientPool.GetPoolCount(), Global._TCPManager.tcpClientPool.InitCount));
			SysConOut.WriteLine(string.Format("TcpOutPacketPool个数:{0}, 实例: {1}, TcpInPacketPool个数:{2}, 实例: {3}, TCPCmdWrapper个数: {4}, SendCmdWrapper: {5}", new object[]
			{
				Global._TCPManager.TcpOutPacketPool.Count,
				TCPOutPacket.GetInstanceCount(),
				Global._TCPManager.TcpInPacketPool.Count,
				TCPInPacket.GetInstanceCount(),
				TCPCmdWrapper.GetTotalCount(),
				SendCmdWrapper.GetInstanceCount()
			}));
			string value = Global._MemoryManager.GetCacheInfoStr();
			SysConOut.WriteLine(value);
			value = Global._FullBufferManager.GetFullBufferInfoStr();
			SysConOut.WriteLine(value);
			value = Global._TCPManager.GetAllCacheCmdPacketInfo();
			SysConOut.WriteLine(value);
		}

		private static void ShowServerTCPInfo(string cmd = null)
		{
			bool flag = cmd.Contains("/c");
			bool flag2 = cmd.Contains("/d");
			DateTime dateTime = TimeUtil.NowDateTime();
			SysConOut.WriteLine(string.Format("当前时间:{0},统计时长:{1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), (dateTime - ProcessSessionTask.StartTime).ToString()));
			if (flag)
			{
				flag2 = true;
				ProcessSessionTask.StartTime = dateTime;
			}
			SysConOut.WriteLine(string.Format("总接收字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesReadSize / 1048576.0));
			SysConOut.WriteLine(string.Format("总发送字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesWriteSize / 1048576.0));
			SysConOut.WriteLine(string.Format("总处理指令个数 {0}", TCPCmdHandler.TotalHandledCmdsNum));
			SysConOut.WriteLine(string.Format("当前正在处理指令的线程数 {0}", TCPCmdHandler.GetHandlingCmdCount()));
			SysConOut.WriteLine(string.Format("单个指令消耗的最大时间 {0}", TCPCmdHandler.MaxUsedTicksByCmdID));
			SysConOut.WriteLine(string.Format("消耗的最大时间指令ID {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID));
			SysConOut.WriteLine(string.Format("发送调用总次数 {0}", Global._TCPManager.MySocketListener.GTotalSendCount));
			SysConOut.WriteLine(string.Format("发送的最大包的大小 {0}", Global._SendBufferManager.MaxOutPacketSize));
			SysConOut.WriteLine(string.Format("发送的最大包的指令ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID));
			SysConOut.WriteLine(string.Format("指令处理平均耗时（毫秒）{0}", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0));
			SysConOut.WriteLine(string.Format("指令处理耗时详情", new object[0]));
			try
			{
				if (flag2)
				{
					if (Console.WindowWidth < 160)
					{
						Console.WindowWidth = 160;
					}
				}
				else if (Console.WindowWidth >= 88)
				{
					Console.WindowWidth = 88;
				}
			}
			catch
			{
			}
			int num = 0;
			lock (ProcessSessionTask.cmdMoniter)
			{
				List<Tuple<string, string>> list = new List<Tuple<string, string>>();
				foreach (PorcessCmdMoniter porcessCmdMoniter in ProcessSessionTask.cmdMoniter.Values)
				{
					if (porcessCmdMoniter.processNum > 0 || porcessCmdMoniter.SendNum > 0L)
					{
						if (flag2)
						{
							string item;
							if (num++ == 0)
							{
								item = string.Format("{0, -48}{1, 6}{2, 7}{3, 7} {4, 7} {5, 4} {6, 4} {7, 5}", new object[]
								{
									"消息",
									"已处理次数",
									"平均处理时长",
									"总计消耗时长",
									"总计字节数",
									"发送次数",
									"发送字节数",
									"失败/成功/数据"
								});
								list.Add(new Tuple<string, string>("", item));
							}
							item = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##} {4, 13:0.##} {5, 8} {6, 12} {7, 4}/{8}/{9}", new object[]
							{
								(TCPGameServerCmds)porcessCmdMoniter.cmd,
								porcessCmdMoniter.processNum,
								TimeUtil.TimeMS(porcessCmdMoniter.avgProcessTime(), 2),
								TimeUtil.TimeMS(porcessCmdMoniter.processTotalTime, 2),
								porcessCmdMoniter.GetTotalBytes(),
								porcessCmdMoniter.SendNum,
								porcessCmdMoniter.OutPutBytes,
								porcessCmdMoniter.Num_Faild,
								porcessCmdMoniter.Num_OK,
								porcessCmdMoniter.Num_WithData
							});
							list.Add(new Tuple<string, string>(((TCPGameServerCmds)porcessCmdMoniter.cmd).ToString(), item));
						}
						else
						{
							string item;
							if (num++ == 0)
							{
								item = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", new object[]
								{
									"消息",
									"已处理次数",
									"平均处理时长",
									"总计消耗时长"
								});
								list.Add(new Tuple<string, string>("", item));
							}
							item = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", new object[]
							{
								(TCPGameServerCmds)porcessCmdMoniter.cmd,
								porcessCmdMoniter.processNum,
								TimeUtil.TimeMS(porcessCmdMoniter.avgProcessTime(), 2),
								TimeUtil.TimeMS(porcessCmdMoniter.processTotalTime, 2)
							});
							list.Add(new Tuple<string, string>(((TCPGameServerCmds)porcessCmdMoniter.cmd).ToString(), item));
						}
					}
					if (flag)
					{
						porcessCmdMoniter.Reset();
					}
				}
				list.Sort((Tuple<string, string> x, Tuple<string, string> y) => string.Compare(x.Item1, y.Item1));
				foreach (Tuple<string, string> tuple in list)
				{
					Console.ForegroundColor = num++ / 2 % 2 + ConsoleColor.Yellow;
					SysConOut.WriteLine(tuple.Item2);
				}
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		private static void ShowCopyMapInfo(string cmd = null)
		{
			string copyMapStrInfo = GameManager.CopyMapMgr.GetCopyMapStrInfo();
			SysConOut.WriteLine(copyMapStrInfo);
		}

		private static void ListCopyMap(string cmd = null)
		{
			string value = GameManager.CopyMapMgr.ListCopyMapStrInfo();
			SysConOut.WriteLine(value);
		}

		private static void ShowRoleInfo(string cmd = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < maxClientCount; i++)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
				if (null != gameClient)
				{
					stringBuilder.AppendFormat("{0, -12} : {4, -3} : {5, -8} : {6, -8} : {7, -8} : {1}({2},{3})\n", new object[]
					{
						gameClient.ClientData.RoleName,
						gameClient.ClientData.MapCode,
						gameClient.ClientData.PosX,
						gameClient.ClientData.PosY,
						gameClient.CodeRevision,
						gameClient.MainExeVer,
						gameClient.ResVer,
						gameClient.ClientSocket.ClientCmdSecs
					});
				}
			}
			if (stringBuilder.Length == 0)
			{
				SysConOut.WriteLine("没有玩家在线");
			}
			else
			{
				SysConOut.WriteLine(stringBuilder.ToString());
			}
		}

		public void AddDBConnectInfo(int index, string info)
		{
			lock (this.DBServerConnectDict)
			{
				if (this.DBServerConnectDict.ContainsKey(index))
				{
					this.DBServerConnectDict[index] = info;
				}
				else
				{
					this.DBServerConnectDict.Add(index, info);
				}
			}
		}

		public void AddLogDBConnectInfo(int index, string info)
		{
			lock (this.LogDBServerConnectDict)
			{
				if (this.LogDBServerConnectDict.ContainsKey(index))
				{
					this.LogDBServerConnectDict[index] = info;
				}
				else
				{
					this.LogDBServerConnectDict.Add(index, info);
				}
			}
		}

		private static void InitProgramExtName()
		{
			Program.ProgramExtName = DataHelper.CurrentDirectory;
		}

		public void InitServer()
		{
			Program.InitProgramExtName();
			int val = 0;
			int val2 = 0;
			ThreadPool.GetMinThreads(out val, out val2);
			ThreadPool.SetMinThreads(Math.Max(val, 1), Math.Max(val2, 64));
			ThreadPool.GetMaxThreads(out val, out val2);
			ThreadPool.SetMaxThreads(Math.Min(val, 1), Math.Min(val2, 360));
			if (!File.Exists("Policy.xml"))
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Policy.xml"));
			}
			TCPPolicy.LoadPolicyServerFile("Policy.xml");
			Global.LoadLangDict();
			SearchTable.Init(14);
			SysConOut.WriteLine("正在初始化游戏资源目录");
			XElement xml = this.InitGameResPath();
			try
			{
				SysConOut.WriteLine("正在初始化数据库连接");
				this.InitTCPManager(xml, true);
				SysConOut.WriteLine("从数据库中获取配置参数");
				GameManager.GameConfigMgr.LoadGameConfigFromDBServer();
				this.InitGameConfigWithDB();
				KFCallManager.Start();
				SysConOut.WriteLine("正在初始化GameRes压缩资源");
				this.InitGameRes();
				SysConOut.WriteLine("载入世界等级");
				WorldLevelManager.getInstance().InitConfig();
				WorldLevelManager.getInstance().ResetWorldLevel();
				SysConOut.WriteLine("正在初始化游戏管理对象");
				this.InitGameManager(xml);
				LuaExeManager.getInstance().InitLuaEnv();
				SysConOut.WriteLine("正在初始化游戏的所有地图和地图中的怪物");
				this.InitGameMapsAndMonsters();
				Data.LoadConfig();
				SingletonTemplate<CreateRoleLimitManager>.Instance().LoadConfig();
				SysConOut.WriteLine("正在初始化活动管理器");
				GlobalServiceManager.initialize();
				GlobalServiceManager.startup();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Process.GetCurrentProcess().Kill();
			}
			SysConOut.WriteLine("正在设置后台工作者线程");
			this.eventWorker = new BackgroundWorker();
			this.eventWorker.DoWork += this.eventWorker_DoWork;
			this.dbCommandWorker = new BackgroundWorker();
			this.dbCommandWorker.DoWork += this.dbCommandWorker_DoWork;
			this.logDBCommandWorker = new BackgroundWorker();
			this.logDBCommandWorker.DoWork += this.logDBCommandWorker_DoWork;
			this.clientsWorker = new BackgroundWorker();
			this.clientsWorker.DoWork += new DoWorkEventHandler(this.clientsWorker_DoWork);
			this.buffersWorker = new BackgroundWorker();
			this.buffersWorker.DoWork += new DoWorkEventHandler(this.buffersWorker_DoWork);
			this.spriteDBWorker = new BackgroundWorker();
			this.spriteDBWorker.DoWork += new DoWorkEventHandler(this.spriteDBWorker_DoWork);
			this.othersWorker = new BackgroundWorker();
			this.othersWorker.DoWork += new DoWorkEventHandler(this.othersWorker_DoWork);
			this.FightingWorker = new BackgroundWorker();
			this.FightingWorker.DoWork += new DoWorkEventHandler(this.FightingWorker_DoWork);
			this.chatMsgWorker = new BackgroundWorker();
			this.chatMsgWorker.DoWork += new DoWorkEventHandler(this.chatMsgWorker_DoWork);
			this.fuBenWorker = new BackgroundWorker();
			this.fuBenWorker.DoWork += new DoWorkEventHandler(this.fuBenWorker_DoWork);
			this.dbWriterWorker = new BackgroundWorker();
			this.dbWriterWorker.DoWork += new DoWorkEventHandler(this.dbWriterWorker_DoWork);
			this.SocketSendCacheDataWorker = new BackgroundWorker();
			this.SocketSendCacheDataWorker.DoWork += new DoWorkEventHandler(this.SocketSendCacheDataWorker_DoWork);
			this.ShengXiaoGuessWorker = new BackgroundWorker();
			this.ShengXiaoGuessWorker.DoWork += new DoWorkEventHandler(this.ShengXiaoGuessWorker_DoWork);
			this.MainDispatcherWorker = new BackgroundWorker();
			this.MainDispatcherWorker.DoWork += new DoWorkEventHandler(this.MainDispatcherWorker_DoWork);
			this.socketCheckWorker = new BackgroundWorker();
			this.socketCheckWorker.DoWork += new DoWorkEventHandler(this.SocketCheckWorker_DoWork);
			this.dynamicMonstersWorker = new BackgroundWorker();
			this.dynamicMonstersWorker.DoWork += new DoWorkEventHandler(this.DynamicMonstersWorker_DoWork);
			this.BanWorker = new BackgroundWorker();
			this.BanWorker.DoWork += new DoWorkEventHandler(this.LoadBanWorker_DoWork);
			this.TwLogWorker = new BackgroundWorker();
			this.TwLogWorker.DoWork += this.TwLogWorker_DoWork;
			this.IPStatisticsWorker = new BackgroundWorker();
			this.IPStatisticsWorker.DoWork += this.IPStatisticsWorker_DoWork;
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("MapGridMagicHelper.ExecuteAllItemsEx()", delegate(object s, EventArgs e)
			{
				GameManager.GridMagicHelperMgrEx.ExecuteAllItemsEx();
			}), 1000, 200);
			for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
			{
				int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;
				if (mapCode == 6090)
				{
					for (int j = 0; j < 25; j++)
					{
					}
				}
				else
				{
					ScheduleExecutor2.Instance.scheduleExecute(new MonsterTask(mapCode, -1), 0, 80);
				}
			}
			this.Gird9UpdateWorkers = new BackgroundWorker[Program.MaxGird9UpdateWorkersNum];
			for (int k = 0; k < Program.MaxGird9UpdateWorkersNum; k++)
			{
				this.Gird9UpdateWorkers[k] = new BackgroundWorker();
				this.Gird9UpdateWorkers[k].DoWork += new DoWorkEventHandler(this.Gird9UpdateWorker_DoWork);
			}
			if (GameManager.IsKuaFuServer)
			{
				for (int i = 0; i < 5; i++)
				{
					int index = i;
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("DoSpriteExtensionWork", delegate(object s, EventArgs e)
					{
						GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, index, 5);
					}), 30000 + i, 100);
				}
			}
			this.RoleStroyboardDispatcherWorker = new BackgroundWorker();
			this.RoleStroyboardDispatcherWorker.DoWork += this.RoleStroyboardDispatcherWorker_DoWork;
			UnhandedException.ShowErrMsgBox = false;
			Global._TCPManager.MySocketListener.DontAccept = false;
			if (!this.MainDispatcherWorker.IsBusy)
			{
				this.MainDispatcherWorker.RunWorkerAsync();
			}
			if (!this.dynamicMonstersWorker.IsBusy)
			{
				this.dynamicMonstersWorker.RunWorkerAsync();
			}
			for (int k = 0; k < Program.MaxGird9UpdateWorkersNum; k++)
			{
				if (!this.Gird9UpdateWorkers[k].IsBusy)
				{
					this.Gird9UpdateWorkers[k].RunWorkerAsync(k);
				}
			}
			if (!this.RoleStroyboardDispatcherWorker.IsBusy)
			{
				this.RoleStroyboardDispatcherWorker.RunWorkerAsync();
			}
			Program.StartThreadPoolDriverTimer();
			GameManager.GameConfigMgr.SetGameConfigItem("gameserver_version", Program.GetVersionDateTime());
			Global.UpdateDBGameConfigg("gameserver_version", Program.GetVersionDateTime());
			GameManager.ArenaBattleMgr.ReShowPKKing();
			LuoLanChengZhanManager.getInstance().ReShowLuolanKing(0);
			SysConOut.WriteLine("正在初始化通信监听");
			Thread.Sleep(3000);
			this.InitTCPManager(xml, false);
			GroupMailManager.RequestNewGroupMailList();
			SysConOut.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
			SysConOut.WriteLine("服务器已经正常启动");
			GameManager.ServerStarting = false;
		}

		public void ExitServer()
		{
			if (!Program.NeedExitServer)
			{
				GlobalServiceManager.showdown();
				GlobalServiceManager.destroy();
				Global._TCPManager.Stop();
				this.Window_Closing();
				BoCaiBuy2DBList.getInstance().SoptServer();
				SysConOut.WriteLine("正在尝试关闭服务器,看到服务器关闭完毕提示后回车退出系统");
				if (0 == Program.GetServerPIDFromFile())
				{
					string text = Console.ReadLine();
					while (this.MainDispatcherWorker.IsBusy)
					{
						SysConOut.WriteLine("正在尝试关闭服务器");
						text = Console.ReadLine();
					}
					Program.StopThreadPoolDriverTimer();
				}
			}
		}

		private XElement InitGameResPath()
		{
			XElement xelement = null;
			try
			{
				xelement = XElement.Load("AppConfig.xml");
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "AppConfig.xml"));
			}
			Global.AbsoluteGameResPath = Global.GetSafeAttributeStr(xelement, "Resource", "Path");
			string currentDirectory = DataHelper.CurrentDirectory;
			if (Global.AbsoluteGameResPath.IndexOf("$SERVER$") >= 0)
			{
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("$SERVER$", currentDirectory);
			}
			if (!string.IsNullOrEmpty(Global.AbsoluteGameResPath))
			{
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("\\", "/");
				Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.TrimEnd(new char[]
				{
					'/'
				});
			}
			Global.CheckConfigPathType();
			return xelement;
		}

		private void InitGameRes()
		{
			try
			{
				Global.AddXElement("ConfigSettings", Global.GetGameResXml("Config/Settings.xml"));
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败 错误信息:{1}", "Config/Settings.xml", ex.Message));
			}
			try
			{
				Global.AddXElement("ConfigLevelUp", Global.GetGameResXml("Config/LevelUp.xml"));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Config/LevelUp.xml"));
			}
			try
			{
				Global.AddXElement("Configgoods", Global.GetGameResXml("Config/Goods.xml"));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", "Config/Goods.xml"));
			}
			Data.WalkUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "WalkUnitCost");
			Data.RunUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "RunUnitCost");
			string[] array = Global.GetSafeAttribute(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "Tick").Value.Split(new char[]
			{
				','
			});
			Data.SpeedTickList = new int[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				Data.SpeedTickList[j] = Convert.ToInt32(array[j]);
			}
			XElement safeXElement = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "DistanceConfig");
			Data.WalkStepWidth = (int)Global.GetSafeAttributeLong(safeXElement, "WalkStepWidth");
			Data.RunStepWidth = (int)Global.GetSafeAttributeLong(safeXElement, "RunStepWidth");
			Data.MaxAttackDistance = (int)Global.GetSafeAttributeLong(safeXElement, "MaxAttackDistance");
			Data.MinAttackDistance = (int)Global.GetSafeAttributeLong(safeXElement, "MinAttackDistance");
			Data.MaxMagicDistance = (int)Global.GetSafeAttributeLong(safeXElement, "MaxMagicDistance");
			Data.MaxAttackSlotTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpeedConfig"), "MaxAttackSlotTick");
			XElement safeXElement2 = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "SpriteConfig");
			Data.LifeTotalWidth = (int)Global.GetSafeAttributeLong(safeXElement2, "LifeTotalWidth");
			Data.HoldWidth = (int)Global.GetSafeAttributeLong(safeXElement2, "HoldWidth");
			Data.HoldHeight = (int)Global.GetSafeAttributeLong(safeXElement2, "HoldHeight");
			Data.GoodsPackOvertimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "GoodsPack"), "MaxOvertimeTick");
			Data.PackDestroyTimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "GoodsPack"), "PackDestroyTimeTick");
			Data.TaskMaxFocusCount = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Task"), "MaxFocusNum");
			Data.AliveGoodsID = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Alive"), "GoodsID");
			Data.AliveMaxLevel = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Alive"), "MaxLevel");
			Data.AutoGetThing = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Bag"), "AutoGetThing");
			int maxLevel = 0;
			IEnumerable<XElement> enumerable = Global.XmlInfo["ConfigLevelUp"].Elements("Experience");
			if (null != enumerable)
			{
				int num = enumerable.Count<XElement>();
				maxLevel = num;
				Data.LevelUpExperienceList = new long[num];
				int i;
				for (i = 0; i < num; i++)
				{
					Data.LevelUpExperienceList[i] = Convert.ToInt64(enumerable.Single((XElement X) => X.Attribute("Level").Value == i.ToString()).Attribute("Value").Value);
				}
			}
			this.LoadRoleSitExpList(maxLevel);
			this.LoadRoleBasePropItems(maxLevel);
			this.LoadRoleZhuanZhiInfo();
			GameManager.ChangeLifeMgr.LoadRoleZhuanShengInfo();
			this.LoadRoleOccupationAddPointInfo();
			this.LoadRoleChangeLifeAddPointInfo();
			WeaponAdornManager.LoadWeaponAdornInfo();
			this.LoadBloodCastleDataInfo();
			this.LoadMoBaiDataInfo();
			this.InitMapStallPosList();
			this.InitMapNameDictionary();
		}

		private void LoadRoleSitExpList(int maxLevel)
		{
			Data.RoleSitExpList = new RoleSitExpItem[maxLevel];
			XElement xelement = null;
			try
			{
				xelement = Global.GetGameResXml(string.Format("Config/RoleSiteExp.xml", new object[0]));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/RoleSiteExp.xml", new object[0])));
			}
			IEnumerable<XElement> enumerable = xelement.Elements();
			foreach (XElement xml in enumerable)
			{
				int num = (int)Global.GetSafeAttributeLong(xml, "ID");
				if (num >= Data.RoleSitExpList.Length)
				{
					break;
				}
				Data.RoleSitExpList[num] = new RoleSitExpItem
				{
					Level = num,
					Experience = (int)Global.GetSafeAttributeLong(xml, "Experience"),
					InterPower = (int)Global.GetSafeAttributeLong(xml, "InterPower"),
					SkilledDegrees = (int)Global.GetSafeAttributeLong(xml, "SkilledDegrees"),
					PKPoint = (int)Global.GetSafeAttributeLong(xml, "PkPoints")
				};
			}
		}

		private void LoadRoleBasePropItems(int maxLevel)
		{
			int i = 0;
			while (i < 6)
			{
				RoleBasePropItem[] array = new RoleBasePropItem[maxLevel];
				XElement xelement = null;
				try
				{
					xelement = Global.GetGameResXml(string.Format("Config/Roles/{0}.xml", i));
				}
				catch (Exception ex)
				{
					if (i != 4)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/{0}.xml", i)));
					}
					Data.RoleBasePropList.Add(array);
					goto IL_38B;
				}
				goto IL_69;
				IL_38B:
				i++;
				continue;
				IL_69:
				int num = 0;
				IEnumerable<XElement> enumerable = xelement.Elements("Levels").Elements<XElement>();
				foreach (XElement xml in enumerable)
				{
					double[] array2 = new double[177];
					for (int j = 0; j < 177; j++)
					{
						array2[j] = Convert.ToDouble(Global.GetDefAttributeStr(xml, ((ExtPropIndexes)j).ToString(), "0.0"));
					}
					array2[13] = Global.GetSafeAttributeDouble(xml, "LifeV");
					array2[15] = Global.GetSafeAttributeDouble(xml, "MagicV");
					array2[3] = Global.GetSafeAttributeDouble(xml, "MinDefenseV");
					array2[4] = Global.GetSafeAttributeDouble(xml, "MaxDefenseV");
					array2[5] = Global.GetSafeAttributeDouble(xml, "MinMDefenseV");
					array2[6] = Global.GetSafeAttributeDouble(xml, "MaxMDefenseV");
					array2[7] = Global.GetSafeAttributeDouble(xml, "MinAttackV");
					array2[8] = Global.GetSafeAttributeDouble(xml, "MaxAttackV");
					array2[9] = Global.GetSafeAttributeDouble(xml, "MinMAttackV");
					array2[10] = Global.GetSafeAttributeDouble(xml, "MaxMAttackV");
					array2[22] = Global.GetSafeAttributeDouble(xml, "RecoverLifeV");
					array2[23] = Global.GetSafeAttributeDouble(xml, "RecoverMagicV");
					array2[88] = 0.0;
					array2[89] = 0.0;
					array2[2] = 1.0;
					array[num] = new RoleBasePropItem
					{
						arrRoleExtProp = array2,
						LifeV = Global.GetSafeAttributeDouble(xml, "LifeV"),
						MagicV = Global.GetSafeAttributeDouble(xml, "MagicV"),
						MinDefenseV = Global.GetSafeAttributeDouble(xml, "MinDefenseV"),
						MaxDefenseV = Global.GetSafeAttributeDouble(xml, "MaxDefenseV"),
						MinMDefenseV = Global.GetSafeAttributeDouble(xml, "MinMDefenseV"),
						MaxMDefenseV = Global.GetSafeAttributeDouble(xml, "MaxMDefenseV"),
						MinAttackV = Global.GetSafeAttributeDouble(xml, "MinAttackV"),
						MaxAttackV = Global.GetSafeAttributeDouble(xml, "MaxAttackV"),
						MinMAttackV = Global.GetSafeAttributeDouble(xml, "MinMAttackV"),
						MaxMAttackV = Global.GetSafeAttributeDouble(xml, "MaxMAttackV"),
						RecoverLifeV = Global.GetSafeAttributeDouble(xml, "RecoverLifeV"),
						RecoverMagicV = Global.GetSafeAttributeDouble(xml, "RecoverMagicV"),
						Dodge = Global.GetSafeAttributeDouble(xml, "Dodge"),
						HitV = Global.GetSafeAttributeDouble(xml, "HitV"),
						PhySkillIncreasePercent = Global.GetSafeAttributeDouble(xml, "PhySkillIncreasePercent"),
						MagicSkillIncreasePercent = Global.GetSafeAttributeDouble(xml, "MagicSkillIncreasePercent"),
						AttackSpeed = Global.GetSafeAttributeDouble(xml, "AttackSpeed")
					};
					num++;
					if (num >= array.Length)
					{
						break;
					}
				}
				Data.RoleBasePropList.Add(array);
				goto IL_38B;
			}
		}

		private void LoadRoleZhuanZhiInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Roles/ZhuanZhi.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("ZhuanZhis").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							ChangeOccupInfo changeOccupInfo = new ChangeOccupInfo();
							int key = (int)Global.GetSafeAttributeLong(xelement, "OccupationID");
							changeOccupInfo.OccupationID = (int)Global.GetSafeAttributeLong(xelement, "OccupationID");
							changeOccupInfo.NeedLevel = (int)Global.GetSafeAttributeLong(xelement, "Level");
							changeOccupInfo.NeedMoney = (int)Global.GetSafeAttributeLong(xelement, "NeedJinBi");
							changeOccupInfo.AwardPropPoint = (int)Global.GetSafeAttributeLong(xelement, "AwardShuXing");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "NeedGoods");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									changeOccupInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(safeAttributeStr, "转职文件");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "AwardGoods");
							if (string.IsNullOrEmpty(safeAttributeStr2))
							{
								LogManager.WriteLog(1, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] array2 = safeAttributeStr2.Split(new char[]
								{
									'|'
								});
								if (array2.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("转职文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									changeOccupInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(safeAttributeStr2, "转职文件");
								}
							}
							Data.ChangeOccupInfoList.Add(key, changeOccupInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml", new object[0])));
			}
		}

		private void LoadRoleOccupationAddPointInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Roles/OccupationAddPoint.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("ShuXings").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							OccupationAddPointInfo occupationAddPointInfo = new OccupationAddPointInfo();
							int key = (int)Global.GetSafeAttributeLong(xelement, "OccupationID");
							occupationAddPointInfo.OccupationID = (int)Global.GetSafeAttributeLong(xelement, "OccupationID");
							occupationAddPointInfo.AddPoint = (int)Global.GetSafeAttributeLong(xelement, "JiaDian");
							Data.OccupationAddPointInfoList.Add(key, occupationAddPointInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml", new object[0])));
			}
		}

		private void LoadRoleChangeLifeAddPointInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Roles/ZhuanShengAddPoint.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("ShuXings").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							ChangeLifeAddPointInfo changeLifeAddPointInfo = new ChangeLifeAddPointInfo();
							int key = (int)Global.GetSafeAttributeLong(xelement, "ZhuanShengLevel");
							changeLifeAddPointInfo.ChangeLevel = (int)Global.GetSafeAttributeLong(xelement, "ZhuanShengLevel");
							changeLifeAddPointInfo.AddPoint = (int)Global.GetSafeAttributeLong(xelement, "JiaDian");
							changeLifeAddPointInfo.nStrLimit = (int)Global.GetSafeAttributeLong(xelement, "Strength");
							changeLifeAddPointInfo.nDexLimit = (int)Global.GetSafeAttributeLong(xelement, "Dexterity");
							changeLifeAddPointInfo.nIntLimit = (int)Global.GetSafeAttributeLong(xelement, "Intelligence");
							changeLifeAddPointInfo.nConLimit = (int)Global.GetSafeAttributeLong(xelement, "Constitution");
							Data.ChangeLifeAddPointInfoList.Add(key, changeLifeAddPointInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanShengAddPoint.xml", new object[0])));
			}
		}

		private void LoadMoBaiDataInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/MoBai.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							MoBaiData moBaiData = new MoBaiData();
							moBaiData.ID = (int)Global.GetSafeAttributeLong(xelement, "ID");
							moBaiData.AdrationMaxLimit = (int)Global.GetSafeAttributeLong(xelement, "Number");
							moBaiData.NeedJinBi = (int)Global.GetSafeAttributeLong(xelement, "NeedJinBi");
							moBaiData.JinBiExpAward = (int)Global.GetSafeAttributeLong(xelement, "JinBiExpAward");
							moBaiData.JinBiZhanGongAward = (int)Global.GetSafeAttributeLong(xelement, "JinBiZhanGongAward");
							moBaiData.NeedZuanShi = (int)Global.GetSafeAttributeLong(xelement, "NeedZuanShi");
							moBaiData.ZuanShiExpAward = (int)Global.GetSafeAttributeLong(xelement, "ZuanShiExpAward");
							moBaiData.ZuanShiZhanGongAward = (int)Global.GetSafeAttributeLong(xelement, "ZuanShiZhanGongAward");
							moBaiData.ExtraNumber = (int)Global.GetSafeAttributeLong(xelement, "ExtraNumber");
							moBaiData.LingJingAwardByJinBi = (int)Global.GetSafeAttributeLong(xelement, "JinBiLingJing");
							moBaiData.LingJingAwardByZuanShi = (int)Global.GetSafeAttributeLong(xelement, "ZuanShiLingJing");
							moBaiData.ShenLiJingHuaByJinBi = (int)Global.GetSafeAttributeLong(xelement, "JinBishenlijinghua");
							moBaiData.ShenLiJingHuaByZuanShi = (int)Global.GetSafeAttributeLong(xelement, "ZuanShishenlijinghua");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "MinLevel");
							string[] sa = safeAttributeStr.Split(new char[]
							{
								','
							});
							int[] array = Global.StringArray2IntArray(sa);
							moBaiData.MinZhuanSheng = array[0];
							moBaiData.MinLevel = array[1];
							Data.MoBaiDataInfoList.Add(moBaiData.ID, moBaiData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/MoBai.xml", new object[0])));
			}
		}

		private void LoadBloodCastleDataInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/BloodCastleInfo.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("BloodCastleInfos").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							BloodCastleDataInfo bloodCastleDataInfo = new BloodCastleDataInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement, "MapCode");
							bloodCastleDataInfo.MapCode = num;
							bloodCastleDataInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xelement, "MinChangeLife");
							bloodCastleDataInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xelement, "MaxChangeLife");
							bloodCastleDataInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xelement, "MaxEnter");
							bloodCastleDataInfo.MinLevel = (int)Global.GetSafeAttributeLong(xelement, "MinLevel");
							bloodCastleDataInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xelement, "MaxLevel");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsIDs");
							string[] sa = safeAttributeStr.Split(new char[]
							{
								','
							});
							int[] array = Global.StringArray2IntArray(sa);
							bloodCastleDataInfo.NeedGoodsID = array[0];
							bloodCastleDataInfo.NeedGoodsNum = array[1];
							bloodCastleDataInfo.MaxPlayerNum = (int)Global.GetSafeAttributeLong(xelement, "MaxPlayer");
							bloodCastleDataInfo.NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster1Level");
							bloodCastleDataInfo.NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster1Num");
							bloodCastleDataInfo.NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster2ID");
							bloodCastleDataInfo.NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster2Num");
							bloodCastleDataInfo.NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xelement, "NeedCreateMonster2Num");
							bloodCastleDataInfo.NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xelement, "NeedCreateMonster2Pos");
							bloodCastleDataInfo.NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xelement, "NeedCreateMonster2Radius");
							bloodCastleDataInfo.NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xelement, "PursuitRadius");
							bloodCastleDataInfo.GateID = (int)Global.GetSafeAttributeLong(xelement, "GateID");
							bloodCastleDataInfo.GatePos = Global.GetSafeAttributeStr(xelement, "GatePos");
							bloodCastleDataInfo.CrystalID = (int)Global.GetSafeAttributeLong(xelement, "CrystalID");
							bloodCastleDataInfo.CrystalPos = Global.GetSafeAttributeStr(xelement, "CrystalPos");
							bloodCastleDataInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xelement, "TimeModulus");
							bloodCastleDataInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xelement, "ExpModulus");
							bloodCastleDataInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xelement, "MoneyModulus");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "AwardItem1");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							bloodCastleDataInfo.AwardItem1 = array2;
							safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "AwardItem2");
							array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							bloodCastleDataInfo.AwardItem2 = array2;
							List<string> list = new List<string>();
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement, "BeginTime");
							if (safeAttributeStr3 != null && safeAttributeStr3 != "")
							{
								string[] array3 = safeAttributeStr3.Split(new char[]
								{
									','
								});
								for (int i = 0; i < array3.Length; i++)
								{
									list.Add(array3[i].Trim());
								}
							}
							bloodCastleDataInfo.BeginTime = list;
							bloodCastleDataInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xelement, "PrepareTime");
							bloodCastleDataInfo.DurationTime = (int)Global.GetSafeAttributeLong(xelement, "DurationTime");
							bloodCastleDataInfo.LeaveTime = (int)Global.GetSafeAttributeLong(xelement, "LeaveTime");
							bloodCastleDataInfo.DiaoXiangID = (int)Global.GetSafeAttributeLong(xelement, "DiaoXiangID");
							bloodCastleDataInfo.DiaoXiangPos = Global.GetSafeAttributeStr(xelement, "DiaoXiangPos");
							Data.BloodCastleDataInfoList.Add(num, bloodCastleDataInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BloodCastleInfo.xml", new object[0])));
			}
		}

		private void LoadCopyScoreDataInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/FuBenPingFen.xml", new object[0]));
				if (null != gameResXml)
				{
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("CopyScoreDataMapInfo", ',');
					List<CopyScoreDataInfo> list = new List<CopyScoreDataInfo>();
					IEnumerable<XElement> enumerable = gameResXml.Elements("CopyScoreInfos").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							CopyScoreDataInfo copyScoreDataInfo = new CopyScoreDataInfo();
							int copyMapID = (int)Global.GetSafeAttributeLong(xelement, "ID");
							copyScoreDataInfo.CopyMapID = copyMapID;
							copyScoreDataInfo.ScoreName = Global.GetSafeAttributeStr(xelement, "PingFenName");
							copyScoreDataInfo.MinScore = (int)Global.GetSafeAttributeLong(xelement, "MinFen");
							copyScoreDataInfo.MaxScore = (int)Global.GetSafeAttributeLong(xelement, "MaxFen");
							copyScoreDataInfo.ExpModulus = Global.GetSafeAttributeDouble(xelement, "ExpXiShu");
							copyScoreDataInfo.MoneyModulus = Global.GetSafeAttributeDouble(xelement, "JinBiXiShu");
							copyScoreDataInfo.FallPacketID = (int)Global.GetSafeAttributeLong(xelement, "GoodsList");
							copyScoreDataInfo.AwardType = (int)Global.GetSafeAttributeLong(xelement, "AwardType");
							copyScoreDataInfo.MinMoJing = (int)Global.GetSafeAttributeLong(xelement, "MinMoJing");
							copyScoreDataInfo.MaxMoJing = (int)Global.GetSafeAttributeLong(xelement, "MaxMoJing");
							list.Add(copyScoreDataInfo);
						}
					}
					foreach (int num in paramValueIntArrayByName)
					{
						List<CopyScoreDataInfo> list2 = new List<CopyScoreDataInfo>();
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j].CopyMapID == num)
							{
								list2.Add(list[j]);
							}
						}
						Data.CopyScoreDataInfoList.Add(num, list2);
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FuBenPingFen.xml", new object[0])));
			}
		}

		private void LoadFreshPlayerCopySceneInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/FreshPlayerCopySceneInfo.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("FreshPlayerCopySceneInfos").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							Data.FreshPlayerSceneInfo = new FreshPlayerCopySceneInfo
							{
								MapCode = (int)Global.GetSafeAttributeLong(xelement, "MapCode"),
								NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster1Level"),
								NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xelement, "NeedKillMonster1Num"),
								NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xelement, "WuShiID"),
								NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xelement, "KillWuShiNum"),
								NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xelement, "WuShiNum"),
								NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xelement, "WuShiPos"),
								NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xelement, "WuShiRadius"),
								NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xelement, "PursuitRadius"),
								GateID = (int)Global.GetSafeAttributeLong(xelement, "GateID"),
								GatePos = Global.GetSafeAttributeStr(xelement, "GatePos"),
								CrystalID = (int)Global.GetSafeAttributeLong(xelement, "CrystalID"),
								CrystalPos = Global.GetSafeAttributeStr(xelement, "CrystalPos"),
								DiaoXiangID = (int)Global.GetSafeAttributeLong(xelement, "DiaoXiangID"),
								DiaoXiangPos = Global.GetSafeAttributeStr(xelement, "DiaoXiangPos")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FreshPlayerCopySceneInfo.xml", new object[0])));
			}
		}

		private void LoadTaskStarDataInfo()
		{
			try
			{
				string uri = "Config/TaskStarInfos.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements("TaskStarInfos").Elements<XElement>();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							TaskStarDataInfo taskStarDataInfo = new TaskStarDataInfo();
							taskStarDataInfo.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							taskStarDataInfo.ExpModulus = Global.GetSafeAttributeDouble(xelement2, "EXPXiShu");
							taskStarDataInfo.BindYuanBaoModulus = Global.GetSafeAttributeDouble(xelement2, "BindZhuanShiXiShu");
							taskStarDataInfo.StarSoulModulus = Global.GetSafeAttributeDouble(xelement2, "XingHunXiShu");
							taskStarDataInfo.Probability = (int)(Global.GetSafeAttributeDouble(xelement2, "GaiLv") * 10000.0);
							Data.TaskStarInfo.Add(taskStarDataInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/TaskStarInfos.xml", new object[0])));
			}
		}

		private void LoadDailyCircleTaskAwardInfo()
		{
			try
			{
				string uri = "Config/DailyCircleTaskAward.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							DailyCircleTaskAwardInfo dailyCircleTaskAwardInfo = new DailyCircleTaskAwardInfo();
							dailyCircleTaskAwardInfo.ID = (int)Global.GetSafeAttributeLong(xelement2, "Id");
							dailyCircleTaskAwardInfo.MinChangeLifeLev = (int)Global.GetSafeAttributeLong(xelement2, "MinzhuanshengLevel");
							dailyCircleTaskAwardInfo.MaxChangeLifeLev = (int)Global.GetSafeAttributeLong(xelement2, "MaxzhuanshengLevel");
							dailyCircleTaskAwardInfo.MinLev = (int)Global.GetSafeAttributeLong(xelement2, "MinLevel");
							dailyCircleTaskAwardInfo.MaxLev = (int)Global.GetSafeAttributeLong(xelement2, "MaxLevel");
							dailyCircleTaskAwardInfo.Experience = (int)Global.GetSafeAttributeLong(xelement2, "EXP");
							dailyCircleTaskAwardInfo.XingHun = (int)Global.GetSafeAttributeLong(xelement2, "XingHun");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsIDs");
							string[] sa = safeAttributeStr.Split(new char[]
							{
								','
							});
							int[] array = Global.StringArray2IntArray(sa);
							dailyCircleTaskAwardInfo.GoodsID = array[0];
							dailyCircleTaskAwardInfo.GoodsNum = array[1];
							dailyCircleTaskAwardInfo.Binding = ((array.Length >= 3) ? array[2] : 1);
							Data.DailyCircleTaskAward.Add(dailyCircleTaskAwardInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/DailyCircleTaskAward.xml", new object[0])));
			}
		}

		private void LoadTaofaTaskAwardInfo()
		{
			try
			{
				int bangZuan = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskAward", -1);
				Data.TaofaTaskExAward.BangZuan = bangZuan;
				Global.MaxTaofaTaskNumForMU = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskNum", -1);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load PriceTaskAward : {0} fail", string.Format("systemParamsList.PriceTaskAward", new object[0])));
			}
		}

		private void LoadCombatForceInfoInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Roles/CombatForceInfo.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (xelement != null)
						{
							CombatForceInfo combatForceInfo = new CombatForceInfo();
							int key = (int)Global.GetSafeAttributeLong(xelement, "ID");
							combatForceInfo.ID = (int)Global.GetSafeAttributeDouble(xelement, "ID");
							combatForceInfo.MaxHPModulus = Global.GetSafeAttributeDouble(xelement, "LifeV");
							combatForceInfo.MaxMPModulus = Global.GetSafeAttributeDouble(xelement, "MagicV");
							combatForceInfo.MinPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xelement, "MinDefenseV");
							combatForceInfo.MaxPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xelement, "MaxDefenseV");
							combatForceInfo.MinMagicDefenseModulus = Global.GetSafeAttributeDouble(xelement, "MinMDefenseV");
							combatForceInfo.MaxMagicDefenseModulus = Global.GetSafeAttributeDouble(xelement, "MaxMDefenseV");
							combatForceInfo.MinPhysicsAttackModulus = Global.GetSafeAttributeDouble(xelement, "MinAttackV");
							combatForceInfo.MaxPhysicsAttackModulus = Global.GetSafeAttributeDouble(xelement, "MaxAttackV");
							combatForceInfo.MinMagicAttackModulus = Global.GetSafeAttributeDouble(xelement, "MinMAttackV");
							combatForceInfo.MaxMagicAttackModulus = Global.GetSafeAttributeDouble(xelement, "MaxMAttackV");
							combatForceInfo.HitValueModulus = Global.GetSafeAttributeDouble(xelement, "HitV");
							combatForceInfo.DodgeModulus = Global.GetSafeAttributeDouble(xelement, "Dodge");
							combatForceInfo.AddAttackInjureModulus = Global.GetSafeAttributeDouble(xelement, "AddAttackInjure");
							combatForceInfo.DecreaseInjureModulus = Global.GetSafeAttributeDouble(xelement, "DecreaseInjureValue");
							combatForceInfo.LifeStealModulus = Global.GetSafeAttributeDouble(xelement, "LifeSteal");
							combatForceInfo.AddAttackModulus = Global.GetSafeAttributeDouble(xelement, "AddAttack");
							combatForceInfo.AddDefenseModulus = Global.GetSafeAttributeDouble(xelement, "AddDefense");
							combatForceInfo.FireAttack = Global.GetSafeAttributeDouble(xelement, "FireAttack");
							combatForceInfo.WaterAttack = Global.GetSafeAttributeDouble(xelement, "WaterAttack");
							combatForceInfo.LightningAttack = Global.GetSafeAttributeDouble(xelement, "LightningAttack");
							combatForceInfo.SoilAttack = Global.GetSafeAttributeDouble(xelement, "SoilAttack");
							combatForceInfo.IceAttack = Global.GetSafeAttributeDouble(xelement, "IceAttack");
							combatForceInfo.WindAttack = Global.GetSafeAttributeDouble(xelement, "WindAttack");
							combatForceInfo.ArmorMax = ConfigHelper.GetElementAttributeValueDouble(xelement, "ArmorMax", 1.0);
							combatForceInfo.HolyAttack = Global.GetSafeAttributeDouble(xelement, "HolyAttack");
							combatForceInfo.HolyDefense = Global.GetSafeAttributeDouble(xelement, "HolyDefense");
							combatForceInfo.ShadowAttack = Global.GetSafeAttributeDouble(xelement, "ShadowAttack");
							combatForceInfo.ShadowDefense = Global.GetSafeAttributeDouble(xelement, "ShadowDefense");
							combatForceInfo.NatureAttack = Global.GetSafeAttributeDouble(xelement, "NatureAttack");
							combatForceInfo.NatureDefense = Global.GetSafeAttributeDouble(xelement, "NatureDefense");
							combatForceInfo.ChaosAttack = Global.GetSafeAttributeDouble(xelement, "ChaosAttack");
							combatForceInfo.ChaosDefense = Global.GetSafeAttributeDouble(xelement, "ChaosDefense");
							combatForceInfo.IncubusAttack = Global.GetSafeAttributeDouble(xelement, "IncubusAttack");
							combatForceInfo.IncubusDefense = Global.GetSafeAttributeDouble(xelement, "IncubusDefense");
							Data.CombatForceDataInfo.Add(key, combatForceInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/CombatForceInfo.xml", new object[0])));
			}
		}

		private void LoadDaimonSquareDataInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/Demon.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("DaimonSquareInfos").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							DaimonSquareDataInfo daimonSquareDataInfo = new DaimonSquareDataInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement, "MapCode");
							daimonSquareDataInfo.MapCode = num;
							daimonSquareDataInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xelement, "MinChangeLife");
							daimonSquareDataInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xelement, "MaxChangeLife");
							daimonSquareDataInfo.MinLevel = (int)Global.GetSafeAttributeLong(xelement, "MinLevel");
							daimonSquareDataInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xelement, "MaxLevel");
							daimonSquareDataInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xelement, "MaxEnter");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "GoodsIDs");
							string[] sa = safeAttributeStr.Split(new char[]
							{
								','
							});
							int[] array = Global.StringArray2IntArray(sa);
							daimonSquareDataInfo.NeedGoodsID = array[0];
							daimonSquareDataInfo.NeedGoodsNum = array[1];
							daimonSquareDataInfo.MaxPlayerNum = (int)Global.GetSafeAttributeLong(xelement, "MaxPlayer");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "MonsterID");
							daimonSquareDataInfo.MonsterID = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							int num2 = daimonSquareDataInfo.MonsterID.Length;
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement, "MonsterNumber");
							daimonSquareDataInfo.MonsterNum = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							int num3 = daimonSquareDataInfo.MonsterNum.Length;
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement, "MonsterPos");
							string[] array2 = safeAttributeStr4.Split(new char[]
							{
								','
							});
							daimonSquareDataInfo.posX = Global.SafeConvertToInt32(array2[0]);
							daimonSquareDataInfo.posZ = Global.SafeConvertToInt32(array2[1]);
							daimonSquareDataInfo.Radius = Global.SafeConvertToInt32(array2[2]);
							daimonSquareDataInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xelement, "MonsterSum");
							string safeAttributeStr5 = Global.GetSafeAttributeStr(xelement, "SuccessConditions");
							daimonSquareDataInfo.CreateNextWaveMonsterCondition = safeAttributeStr5.Split(new char[]
							{
								'|'
							});
							int num4 = daimonSquareDataInfo.CreateNextWaveMonsterCondition.Length;
							if (num2 != num3 || num2 != num4)
							{
							}
							daimonSquareDataInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xelement, "TimeModulus");
							daimonSquareDataInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xelement, "ExpModulus");
							daimonSquareDataInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xelement, "MoneyModulus");
							string safeAttributeStr6 = Global.GetSafeAttributeStr(xelement, "AwardItem1");
							string[] awardItem = safeAttributeStr6.Split(new char[]
							{
								'|'
							});
							daimonSquareDataInfo.AwardItem = awardItem;
							List<string> list = new List<string>();
							string safeAttributeStr7 = Global.GetSafeAttributeStr(xelement, "BeginTime");
							if (safeAttributeStr7 != null && safeAttributeStr7 != "")
							{
								string[] array3 = safeAttributeStr7.Split(new char[]
								{
									','
								});
								for (int i = 0; i < array3.Length; i++)
								{
									list.Add(array3[i].Trim());
								}
							}
							daimonSquareDataInfo.BeginTime = list;
							daimonSquareDataInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xelement, "PrepareTime");
							daimonSquareDataInfo.DurationTime = (int)Global.GetSafeAttributeLong(xelement, "DurationTime");
							daimonSquareDataInfo.LeaveTime = (int)Global.GetSafeAttributeLong(xelement, "LeaveTime");
							Data.DaimonSquareDataInfoList.Add(num, daimonSquareDataInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Demon.xml", new object[0])));
			}
		}

		private void LoadSystemParamsDataForCache()
		{
			try
			{
				double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiJiaCheng", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.WingForgeLevelAddShangHaiJiaCheng = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.WingForgeLevelAddDefenseRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingZhuiJiaLevelAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.WingZhuiJiaLevelAddDefenseRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiXiShou", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.WingForgeLevelAddShangHaiXiShou = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddAttackRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ForgeLevelAddAttackRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddAttackRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ZhuiJiaLevelAddAttackRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ForgeLevelAddDefenseRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ZhuiJiaLevelAddDefenseRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddMaxLifeVRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ForgeLevelAddMaxLifeVRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddAttackRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ZhuoYueAddAttackRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.ZhuoYueAddDefenseRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("RebornZhuoYueAddRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.RebornZhuoYueAddRates = paramValueDoubleArrayByName;
				}
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ShiJieChuanSong");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						int key = Global.SafeConvertToInt32(array2[0]);
						int value = Global.SafeConvertToInt32(array2[1]);
						Data.MapTransNeedMoneyDict.Add(key, value);
					}
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddAttackRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.EquipChangeLifeAddAttackRates = paramValueDoubleArrayByName;
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddDefenseRates", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length > 0)
				{
					Data.EquipChangeLifeAddDefenseRates = paramValueDoubleArrayByName;
				}
				int num = (int)GameManager.systemParamsList.GetParamValueDoubleByName("ZuanshiVIPExp", 0.0);
				if (num != 0)
				{
					Data.DiamondToVipExpValue = num;
				}
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("BossStaticDataIDForChengJiu", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
				{
					Data.KillBossCountForChengJiu = paramValueIntArrayByName;
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ForgeProtectStoneGoodsIDS");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					Data.ForgeProtectStoneGoodsID = new int[array.Length];
					Data.ForgeProtectStoneGoodsNum = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						int num2 = Global.SafeConvertToInt32(array2[0]);
						int num3 = Global.SafeConvertToInt32(array2[1]);
						Data.ForgeProtectStoneGoodsID[i] = num2;
						Data.ForgeProtectStoneGoodsNum[i] = num3;
					}
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("WinCaiLiaoZuanShi");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array3 = paramValueByName.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array3.Length; i++)
					{
						string[] array4 = array3[i].Split(new char[]
						{
							','
						});
						int key2 = Global.SafeConvertToInt32(array4[0]);
						int value2 = Global.SafeConvertToInt32(array4[1]);
						Data.LingYuMaterialZuanshiDict[key2] = value2;
					}
				}
				SecondPasswordManager.ValidSecWhenLogout = GameManager.systemParamsList.GetParamValueIntByName("SecondPasswordTime", -1);
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("MoBaiNumber");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					Data.PKkingadrationData.AdrationMaxLimit = Global.SafeConvertToInt32(paramValueByName);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("JiBiMoBai");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array5 = paramValueByName.Split(new char[]
					{
						','
					});
					Data.PKkingadrationData.GoldAdrationSpend = Global.SafeConvertToInt32(array5[0]);
					Data.PKkingadrationData.GoldAdrationExpModulus = Global.SafeConvertToInt32(array5[1]);
					Data.PKkingadrationData.GoldAdrationShengWangModulus = Global.SafeConvertToInt32(array5[2]);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZuanShiMoBai");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array5 = paramValueByName.Split(new char[]
					{
						','
					});
					Data.PKkingadrationData.DiamondAdrationSpend = Global.SafeConvertToInt32(array5[0]);
					Data.PKkingadrationData.DiamondAdrationExpModulus = Global.SafeConvertToInt32(array5[1]);
					Data.PKkingadrationData.DiamondAdrationShengWangModulus = Global.SafeConvertToInt32(array5[2]);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("LuoLanMoBaiNumber");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					Data.LLCZadrationData.AdrationMaxLimit = Global.SafeConvertToInt32(paramValueByName);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("LuoLanJiBiMoBai");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array5 = paramValueByName.Split(new char[]
					{
						','
					});
					Data.LLCZadrationData.GoldAdrationSpend = Global.SafeConvertToInt32(array5[0]);
					Data.LLCZadrationData.GoldAdrationExpModulus = Global.SafeConvertToInt32(array5[1]);
					Data.LLCZadrationData.GoldAdrationShengWangModulus = Global.SafeConvertToInt32(array5[2]);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("LuoLanZuanShiMoBai");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array5 = paramValueByName.Split(new char[]
					{
						','
					});
					Data.LLCZadrationData.DiamondAdrationSpend = Global.SafeConvertToInt32(array5[0]);
					Data.LLCZadrationData.DiamondAdrationExpModulus = Global.SafeConvertToInt32(array5[1]);
					Data.LLCZadrationData.DiamondAdrationShengWangModulus = Global.SafeConvertToInt32(array5[2]);
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("CangKuAward");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array5 = paramValueByName.Split(new char[]
					{
						'|'
					});
					Data.InsertAwardtPortableBagTaskID = Global.SafeConvertToInt32(array5[0]);
					Data.InsertAwardtPortableBagGoodsInfo = array5[1];
				}
				paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("HongMingDebuff", ',');
				if (paramValueDoubleArrayByName != null)
				{
					Data.RedNameDebuffInfo = paramValueDoubleArrayByName;
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsIDs");
				if (paramValueByName != null && paramValueByName.Length > 0)
				{
					Data.ForgeNeedGoodsID = Global.String2StringArray(paramValueByName, '|');
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsNum");
				if (paramValueByName != null && paramValueByName.Length > 0)
				{
					Data.ForgeNeedGoodsNum = Global.String2StringArray(paramValueByName, '|');
				}
				if (Data.ForgeNeedGoodsID.Length != Data.ForgeNeedGoodsNum.Length)
				{
					throw new Exception(string.Format("load file : {0} error", string.Format("LoadSystemParamsDataForCache", new object[0])));
				}
				num = (int)GameManager.systemParamsList.GetParamValueDoubleByName("PaiHangChongBai", 0.0);
				if (num != 0)
				{
					Data.PaihangbangAdration = num;
				}
				paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("storycopymapid", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
				{
					Data.StoryCopyMapID = paramValueIntArrayByName;
				}
				num = (int)GameManager.systemParamsList.GetParamValueDoubleByName("QiFuTime", 0.0);
				if (num != 0)
				{
					Data.FreeImpetrateIntervalTime = num * 60;
				}
				SingletonTemplate<GuardStatueManager>.Instance().SuitFactor = GameManager.systemParamsList.GetParamValueDoubleByName("ShouHuSuit", 0.0);
				SingletonTemplate<GuardStatueManager>.Instance().LevelFactor = GameManager.systemParamsList.GetParamValueDoubleByName("ShouHuLevel", 0.0);
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ShouHuMax");
				SingletonTemplate<GuardStatueManager>.Instance().InitRecoverPoint_BySysParam(paramValueByName);
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("ShouHuDiaoXiang");
				SingletonTemplate<GuardStatueManager>.Instance().InitSoulSlot_BySysParam(paramValueByName);
				paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ModName", ',');
				SingletonTemplate<NameManager>.Instance().CostZuanShiBase = paramValueIntArrayByName[0];
				SingletonTemplate<NameManager>.Instance().CostZuanShiMax = paramValueIntArrayByName[1];
				SingletonTemplate<MoRiJudgeManager>.Instance().AwardFactor = GameManager.systemParamsList.GetParamValueDoubleArrayByName("MoRiShenPanAward", ',');
				KuaFuManager.getInstance().InitCopyTime();
				SingletonTemplate<SoulStoneManager>.Instance().LoadJingHuaExpConfig();
				SingletonTemplate<MonsterAttackerLogManager>.Instance().LoadRecordMonsters();
				SingletonTemplate<SpeedUpTickCheck>.Instance().LoadConfig();
				SingletonTemplate<NameManager>.Instance().LoadConfig();
				SingletonTemplate<CoupleArenaManager>.Instance().InitSystenParams();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load file : {0} fail, {1}", string.Format("LoadSystemParamsDataForCache", new object[0]), ex.ToString()));
			}
		}

		public static void LoadTotalLoginDataInfo()
		{
			try
			{
				string uri = "Config/Gifts/NewHuoDongLoginNumGift.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null != xelement)
				{
					Dictionary<int, TotalLoginDataInfo> dictionary = new Dictionary<int, TotalLoginDataInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements("HuoDongLoginNumGift").Elements<XElement>();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							TotalLoginDataInfo totalLoginDataInfo = new TotalLoginDataInfo();
							int key = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							totalLoginDataInfo.TotalLoginDays = (int)Global.GetSafeAttributeLong(xelement2, "TimeOl");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID1");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length != 0)
							{
								totalLoginDataInfo.NormalAward = new List<GoodsData>();
								foreach (string text in array)
								{
									string[] array2 = text.Split(new char[]
									{
										','
									});
									if (array2.Length == 7)
									{
										GoodsData goodsData = new GoodsData();
										goodsData.GoodsID = Global.SafeConvertToInt32(array2[0]);
										goodsData.GCount = Global.SafeConvertToInt32(array2[1]);
										goodsData.Binding = Global.SafeConvertToInt32(array2[2]);
										goodsData.Forge_level = Global.SafeConvertToInt32(array2[3]);
										goodsData.AppendPropLev = Global.SafeConvertToInt32(array2[4]);
										goodsData.Lucky = Global.SafeConvertToInt32(array2[5]);
										goodsData.ExcellenceInfo = Global.SafeConvertToInt32(array2[6]);
										totalLoginDataInfo.NormalAward.Add(goodsData);
									}
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID2");
							array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length != 0)
							{
								totalLoginDataInfo.Award0 = new List<GoodsData>();
								totalLoginDataInfo.Award1 = new List<GoodsData>();
								totalLoginDataInfo.Award2 = new List<GoodsData>();
								totalLoginDataInfo.Award3 = new List<GoodsData>();
								totalLoginDataInfo.Award5 = new List<GoodsData>();
								foreach (string text in array)
								{
									string[] array2 = text.Split(new char[]
									{
										','
									});
									if (array2.Length == 7)
									{
										GoodsData goodsData = new GoodsData();
										goodsData.GoodsID = Global.SafeConvertToInt32(array2[0]);
										goodsData.GCount = Global.SafeConvertToInt32(array2[1]);
										goodsData.Binding = Global.SafeConvertToInt32(array2[2]);
										goodsData.Forge_level = Global.SafeConvertToInt32(array2[3]);
										goodsData.AppendPropLev = Global.SafeConvertToInt32(array2[4]);
										goodsData.Lucky = Global.SafeConvertToInt32(array2[5]);
										goodsData.ExcellenceInfo = Global.SafeConvertToInt32(array2[6]);
										int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsData.GoodsID);
										if (mainOccupationByGoodsID == 0)
										{
											totalLoginDataInfo.Award0.Add(goodsData);
										}
										else if (mainOccupationByGoodsID == 1)
										{
											totalLoginDataInfo.Award1.Add(goodsData);
										}
										else if (mainOccupationByGoodsID == 2)
										{
											totalLoginDataInfo.Award2.Add(goodsData);
										}
										else if (mainOccupationByGoodsID == 3)
										{
											totalLoginDataInfo.Award3.Add(goodsData);
										}
										else if (mainOccupationByGoodsID == 5)
										{
											totalLoginDataInfo.Award5.Add(goodsData);
										}
									}
								}
							}
							dictionary.Add(key, totalLoginDataInfo);
						}
					}
					lock (Data.TotalLoginDataInfoListLock)
					{
						Data.TotalLoginDataInfoList = dictionary;
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/NewHuoDongLoginNumGift.xml", new object[0])));
			}
		}

		private void LoadVIPDataInfo()
		{
			try
			{
				string uri = "Config/Gifts/VipDailyAwards.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							VIPDataInfo vipdataInfo = new VIPDataInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement2, "AwardID");
							vipdataInfo.AwardID = num;
							vipdataInfo.ZuanShi = (int)Global.GetSafeAttributeDouble(xelement2, "ZuanShi");
							vipdataInfo.BindZuanShi = (int)Global.GetSafeAttributeDouble(xelement2, "BindZuanShi");
							vipdataInfo.JinBi = (int)Global.GetSafeAttributeDouble(xelement2, "JinBi");
							vipdataInfo.BindJinBi = (int)Global.GetSafeAttributeDouble(xelement2, "BindJinBi");
							vipdataInfo.VIPlev = (int)Global.GetSafeAttributeDouble(xelement2, "VIPlev");
							vipdataInfo.XiHongMing = (int)Global.GetSafeAttributeDouble(xelement2, "XiHongMing");
							vipdataInfo.XiuLi = (int)Global.GetSafeAttributeDouble(xelement2, "XiuLi");
							vipdataInfo.DailyMaxUseTimes = (int)Global.GetSafeAttributeDouble(xelement2, "DailyMaxUseTimes");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "BufferGoods");
							if (safeAttributeStr != null)
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									','
								});
								if (array.Count<string>() > 0)
								{
									vipdataInfo.BufferGoods = new int[array.Count<string>()];
									for (int i = 0; i < array.Count<string>(); i++)
									{
										int num2 = Global.SafeConvertToInt32(array[i]);
										vipdataInfo.BufferGoods[i] = num2;
									}
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GoodsIDs");
							if (!string.IsNullOrEmpty(safeAttributeStr2))
							{
								string[] array2 = safeAttributeStr2.Split(new char[]
								{
									'|'
								});
								if (array2.Length != 0)
								{
									vipdataInfo.AwardGoods = new List<GoodsData>();
									foreach (string text in array2)
									{
										string[] array3 = text.Split(new char[]
										{
											','
										});
										GoodsData goodsData = new GoodsData();
										if (array3.Length == 7)
										{
											goodsData.GoodsID = Global.SafeConvertToInt32(array3[0]);
											goodsData.GCount = Global.SafeConvertToInt32(array3[1]);
											goodsData.Binding = Global.SafeConvertToInt32(array3[2]);
											goodsData.Forge_level = Global.SafeConvertToInt32(array3[3]);
											goodsData.AppendPropLev = Global.SafeConvertToInt32(array3[4]);
											goodsData.Lucky = Global.SafeConvertToInt32(array3[5]);
											goodsData.ExcellenceInfo = Global.SafeConvertToInt32(array3[6]);
										}
										else
										{
											goodsData.GoodsID = Global.SafeConvertToInt32(array3[0]);
										}
										vipdataInfo.AwardGoods.Add(goodsData);
									}
								}
							}
							Data.VIPDataInfoList.Add(num, vipdataInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/VipDailyAwards.xml", new object[0])));
			}
		}

		private void LoadVIPLevAwardAndExpInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/MuVip.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							VIPLevAwardAndExpInfo viplevAwardAndExpInfo = new VIPLevAwardAndExpInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement, "VIPLevel");
							viplevAwardAndExpInfo.VipLev = num;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "LiBaoAward");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length != 0)
							{
								viplevAwardAndExpInfo.AwardList = new List<GoodsData>();
								foreach (string text in array)
								{
									string[] array2 = text.Split(new char[]
									{
										','
									});
									GoodsData goodsData = new GoodsData();
									if (array2.Length == 7)
									{
										goodsData.GoodsID = Global.SafeConvertToInt32(array2[0]);
										goodsData.GCount = Global.SafeConvertToInt32(array2[1]);
										goodsData.Binding = Global.SafeConvertToInt32(array2[2]);
										goodsData.Forge_level = Global.SafeConvertToInt32(array2[3]);
										goodsData.AppendPropLev = Global.SafeConvertToInt32(array2[4]);
										goodsData.Lucky = Global.SafeConvertToInt32(array2[5]);
										goodsData.ExcellenceInfo = Global.SafeConvertToInt32(array2[6]);
									}
									else
									{
										goodsData.GoodsID = Global.SafeConvertToInt32(array2[0]);
									}
									viplevAwardAndExpInfo.AwardList.Add(goodsData);
								}
							}
							viplevAwardAndExpInfo.NeedExp = (int)Global.GetSafeAttributeLong(xelement, "NeedExp");
							Data.VIPLevAwardAndExpInfoList.Add(num, viplevAwardAndExpInfo);
							VIPEumValue.VIPENUMVALUE_MAXLEVEL = Math.Max(num, VIPEumValue.VIPENUMVALUE_MAXLEVEL);
							if (VIPEumValue.VIP_MIN_NEED_EXP <= 0)
							{
								VIPEumValue.VIP_MIN_NEED_EXP = viplevAwardAndExpInfo.NeedExp;
							}
							VIPEumValue.VIP_MIN_NEED_EXP = Math.Min(viplevAwardAndExpInfo.NeedExp, VIPEumValue.VIP_MIN_NEED_EXP);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/MuVip.xml", new object[0])));
			}
		}

		private void LoadMeditateInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/MingXiang.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							MeditateData meditateData = new MeditateData();
							int num = (int)Global.GetSafeAttributeDouble(xelement, "ID");
							meditateData.MeditateID = num;
							meditateData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xelement, "MinZhuanSheng");
							meditateData.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xelement, "MaxZhuanSheng");
							meditateData.MinLevel = (int)Global.GetSafeAttributeLong(xelement, "MinLevel");
							meditateData.MaxLevel = (int)Global.GetSafeAttributeLong(xelement, "MaxLevel");
							meditateData.Experience = (int)Global.GetSafeAttributeLong(xelement, "Experience");
							meditateData.StarSoul = (int)Global.GetSafeAttributeLong(xelement, "Xinghun");
							meditateData.MediateRewardTuple = ConfigHelper.GetElementAttributeValueIntArray(xelement, "RewardID", null);
							meditateData.GetRewardTime = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "GetRewardTime", 600L) * 1000;
							Data.MeditateInfoList.Add(num, meditateData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("load xml file : {0}", string.Format("Config/VIPExp.xml", new object[0])), ex, true);
			}
		}

		private void LoadExperienceCopyMapDataInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/JinYanFuBen.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("JinYanFuBen").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							ExperienceCopyMapDataInfo experienceCopyMapDataInfo = new ExperienceCopyMapDataInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement, "MapCode");
							experienceCopyMapDataInfo.CopyMapID = (int)Global.GetSafeAttributeLong(xelement, "ID");
							experienceCopyMapDataInfo.MapCodeID = num;
							experienceCopyMapDataInfo.MonsterIDList = new Dictionary<int, List<int>>();
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "MonsterID");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split(new char[]
								{
									','
								});
								List<int> list = new List<int>();
								for (int j = 0; j < array2.Length; j++)
								{
									int item = Global.SafeConvertToInt32(array2[j]);
									list.Add(item);
								}
								experienceCopyMapDataInfo.MonsterIDList.Add(i, list);
							}
							experienceCopyMapDataInfo.MonsterNumList = new Dictionary<int, List<int>>();
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "MonsterNumber");
							string[] array3 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array3.Length; i++)
							{
								string[] array4 = array3[i].Split(new char[]
								{
									','
								});
								List<int> list2 = new List<int>();
								for (int j = 0; j < array4.Length; j++)
								{
									int item2 = Global.SafeConvertToInt32(array4[j]);
									list2.Add(item2);
								}
								experienceCopyMapDataInfo.MonsterNumList.Add(i, list2);
							}
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement, "MonsterPos");
							string[] array5 = safeAttributeStr3.Split(new char[]
							{
								','
							});
							experienceCopyMapDataInfo.posX = Global.SafeConvertToInt32(array5[0]);
							experienceCopyMapDataInfo.posZ = Global.SafeConvertToInt32(array5[1]);
							experienceCopyMapDataInfo.Radius = Global.SafeConvertToInt32(array5[2]);
							experienceCopyMapDataInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xelement, "MonsterSum");
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement, "SuccessConditions");
							string[] array6 = safeAttributeStr4.Split(new char[]
							{
								'|'
							});
							experienceCopyMapDataInfo.CreateNextWaveMonsterCondition = new int[array6.Length];
							for (int i = 0; i < array6.Length; i++)
							{
								experienceCopyMapDataInfo.CreateNextWaveMonsterCondition[i] = Global.SafeConvertToInt32(array6[i]);
							}
							int num2 = experienceCopyMapDataInfo.CreateNextWaveMonsterCondition.Length;
							Data.ExperienceCopyMapDataInfoList.Add(num, experienceCopyMapDataInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/JinYanFuBen.xml", new object[0])));
			}
		}

		private void LoadBossHomeInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/BossZhiJia.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("BossZhiJias").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							Data.BosshomeData = new BossHomeData
							{
								MapID = (int)Global.GetSafeAttributeDouble(xelement, "MapCode"),
								VIPLevLimit = (int)Global.GetSafeAttributeLong(xelement, "KaiQiVipLevel"),
								MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xelement, "MinChangeLife"),
								MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xelement, "MaxChangeLife"),
								MinLevel = (int)Global.GetSafeAttributeLong(xelement, "MinLevel"),
								MaxLevel = (int)Global.GetSafeAttributeLong(xelement, "MaxLevel"),
								EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xelement, "EnterNeedZuanShi"),
								OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xelement, "MapTimeNeedZuanShi")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BossZhiJia.xml", new object[0])));
			}
		}

		private void LoadGoldTempleInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/HuangJinShengDian.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("HuangJinShengDians").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							Data.GoldtempleData = new GoldTempleData
							{
								MapID = (int)Global.GetSafeAttributeDouble(xelement, "MapCode"),
								VIPLevLimit = (int)Global.GetSafeAttributeLong(xelement, "KaiQiVipLevel"),
								MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xelement, "MinChangeLife"),
								MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xelement, "MaxChangeLife"),
								MinLevel = (int)Global.GetSafeAttributeLong(xelement, "MinLevel"),
								MaxLevel = (int)Global.GetSafeAttributeLong(xelement, "MaxLevel"),
								EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xelement, "EnterNeedZuanShi"),
								OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xelement, "MapTimeNeedZuanShi")
							};
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/HuangJinShengDian.xml", new object[0])));
			}
		}

		private void LoadEquipUpgradeInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/MuEquipUp.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("Equip");
					foreach (XElement xelement in enumerable)
					{
						IEnumerable<XElement> enumerable2 = xelement.Elements("Item");
						Dictionary<int, MuEquipUpgradeData> dictionary = new Dictionary<int, MuEquipUpgradeData>();
						int num = (int)Global.GetSafeAttributeLong(xelement, "Categoriy");
						foreach (XElement xelement2 in enumerable2)
						{
							if (null != xelement2)
							{
								MuEquipUpgradeData muEquipUpgradeData = new MuEquipUpgradeData();
								int num2 = (int)Global.GetSafeAttributeLong(xelement2, "SuitID");
								muEquipUpgradeData.CategoriyID = num;
								muEquipUpgradeData.SuitID = num2;
								muEquipUpgradeData.NeedMoJing = (int)Global.GetSafeAttributeLong(xelement2, "NeedMoJing");
								dictionary[num2] = muEquipUpgradeData;
							}
						}
						Data.EquipUpgradeData.Add(num, dictionary);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/MuEquipUp.xml", new object[0])));
			}
		}

		private void LoadFuBenNeedInfo()
		{
			try
			{
				List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("FuBenNeed", '|');
				if (paramValueStringListByName != null && paramValueStringListByName.Count > 0)
				{
					foreach (string str in paramValueStringListByName)
					{
						int[] array = Global.String2IntArray(str, ',');
						if (array != null && array.Length == 2)
						{
							int key = array[0];
							int value = array[1];
							Data.FuBenNeedDict[key] = value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/MuEquipUp.xml", new object[0])));
			}
		}

		private void LoadGoldCopySceneInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/JinBiFuBen.xml", new object[0]));
				if (null != gameResXml)
				{
					GoldCopySceneData goldCopySceneData = new GoldCopySceneData();
					XElement xelement = gameResXml.Element("PatrolPath");
					if (null != xelement)
					{
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "Path");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("金币副本怪路径为空", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("金币副本怪路径为空", new object[0]), null, true);
							}
							else
							{
								goldCopySceneData.m_MonsterPatorlPathList = new List<int[]>();
								for (int i = 0; i < array.Length; i++)
								{
									string[] array2 = array[i].Split(new char[]
									{
										','
									});
									if (array2.Length != 2)
									{
										LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败,坐标有误", "Config/JinBiFuBen.xml"), null, true);
									}
									else
									{
										int[] item = Global.StringArray2IntArray(array2);
										goldCopySceneData.m_MonsterPatorlPathList.Add(item);
									}
								}
								Data.Goldcopyscenedata.m_MonsterPatorlPathList = goldCopySceneData.m_MonsterPatorlPathList;
							}
						}
					}
					IEnumerable<XElement> enumerable = gameResXml.Elements("Monsters").Elements<XElement>();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							GoldCopySceneMonster goldCopySceneMonster = new GoldCopySceneMonster();
							goldCopySceneMonster.m_MonsterID = new List<int>();
							int num = (int)Global.GetSafeAttributeLong(xelement2, "WaveID");
							goldCopySceneMonster.m_Wave = num;
							goldCopySceneMonster.m_Num = (int)Global.GetSafeAttributeLong(xelement2, "Num");
							goldCopySceneMonster.m_Delay1 = (int)Global.GetSafeAttributeLong(xelement2, "Delay1");
							goldCopySceneMonster.m_Delay2 = (int)Global.GetSafeAttributeLong(xelement2, "Delay2");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "MonsterList");
							if (string.IsNullOrEmpty(safeAttributeStr2))
							{
								LogManager.WriteLog(1, string.Format("金币副本怪ID为空", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr2.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("金币副本怪ID为空", new object[0]), null, true);
								}
								else
								{
									for (int i = 0; i < array.Length; i++)
									{
										int item2 = Global.SafeConvertToInt32(array[i]);
										goldCopySceneMonster.m_MonsterID.Add(item2);
									}
								}
							}
							goldCopySceneData.GoldCopySceneMonsterData.Add(num, goldCopySceneMonster);
						}
					}
					Data.Goldcopyscenedata.GoldCopySceneMonsterData = goldCopySceneData.GoldCopySceneMonsterData;
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/JinBiFuBen.xml", new object[0])));
			}
		}

		private void LoadEquipJuHunInfo()
		{
			Dictionary<int, EquipJuHunXmlData> dictionary = new Dictionary<int, EquipJuHunXmlData>();
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/JuHun.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (xelement != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "ID", "0"));
							int type = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "Type", "0"));
							int level = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "Level", "0"));
							double growProportion = Convert.ToDouble(Global.GetDefAttributeStr(xelement, "GrowProportion", "0"));
							Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
							string[] array = Global.GetDefAttributeStr(xelement, "CostGoods", "").Split(new char[]
							{
								'|'
							});
							foreach (string text in array)
							{
								string[] array3 = text.Split(new char[]
								{
									','
								});
								if (array3.Length == 2)
								{
									dictionary2.Add(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[1]));
								}
							}
							Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
							string[] array4 = Global.GetDefAttributeStr(xelement, "ProtectGoods", "").Split(new char[]
							{
								'|'
							});
							foreach (string text in array4)
							{
								string[] array3 = text.Split(new char[]
								{
									','
								});
								if (array3.Length == 2)
								{
									dictionary3.Add(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[1]));
								}
							}
							int costBandJinBi = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "CostBandJinBi", "0"));
							double successProportion = Convert.ToDouble(Global.GetDefAttributeStr(xelement, "SuccessProportion", "0"));
							EquipJuHunXmlData value = new EquipJuHunXmlData
							{
								ID = num,
								Type = type,
								Level = level,
								GrowProportion = growProportion,
								CostGoods = dictionary2,
								ProtectGoods = dictionary3,
								CostBandJinBi = costBandJinBi,
								SuccessProportion = successProportion
							};
							dictionary.Add(num, value);
						}
					}
					Data.EquipJuHunDataDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("启动时加载xml文件: {0} 失败\r\n{1}", string.Format("Config/JuHun.xml", new object[0]), ex.ToString()), null, true);
			}
		}

		private void LoadBagType()
		{
			Dictionary<int, BagTypeXmlData> dictionary = new Dictionary<int, BagTypeXmlData>();
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/BagType.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements();
					foreach (XElement xelement in enumerable)
					{
						if (xelement != null)
						{
							string defAttributeStr = Global.GetDefAttributeStr(xelement, "GoodsType", "");
							if (!string.IsNullOrEmpty(defAttributeStr))
							{
								int bagType = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "BagType", "0"));
								int bindingType = Convert.ToInt32(Global.GetDefAttributeStr(xelement, "BangdingType", "0"));
								foreach (string value in defAttributeStr.Split(new char[]
								{
									','
								}))
								{
									dictionary[Convert.ToInt32(value)] = new BagTypeXmlData
									{
										BagType = bagType,
										BindingType = bindingType
									};
								}
							}
						}
					}
					Data.BagTypeDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("启动时加载xml文件: {0} 失败\r\n{1}", string.Format("Config/BagType.xml", new object[0]), ex.ToString()), null, true);
			}
		}

		private void InitMapStallPosList()
		{
			Data.MapStallList.Clear();
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				IEnumerable<XElement> enumerable = null;
				try
				{
					XElement safeXElement = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "MapStalls");
					if (null != safeXElement)
					{
						enumerable = safeXElement.Elements("Stall");
					}
				}
				catch (Exception)
				{
				}
				if (null != enumerable)
				{
					foreach (XElement xml in enumerable)
					{
						int mapID = (int)Global.GetSafeAttributeLong(xml, "Code");
						Point toPos = new Point((double)((int)Global.GetSafeAttributeLong(xml, "X")), (double)((int)Global.GetSafeAttributeLong(xml, "Y")));
						int radius = (int)Global.GetSafeAttributeLong(xml, "Radius");
						MapStallItem mapStallItem = new MapStallItem
						{
							MapID = mapID,
							ToPos = toPos,
							Radius = radius
						};
						if (null != mapStallItem)
						{
							Data.MapStallList.Add(mapStallItem);
						}
					}
				}
			}
		}

		private void InitMapNameDictionary()
		{
			Data.MapNamesDict.Clear();
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				IEnumerable<XElement> enumerable = null;
				try
				{
					XElement safeXElement = Global.GetSafeXElement(Global.XmlInfo["ConfigSettings"], "Maps");
					if (null != safeXElement)
					{
						enumerable = safeXElement.Elements("Map");
					}
				}
				catch (Exception)
				{
				}
				if (null != enumerable)
				{
					foreach (XElement xml in enumerable)
					{
						int key = (int)Global.GetSafeAttributeLong(xml, "Code");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Name");
						Data.MapNamesDict[key] = safeAttributeStr;
					}
				}
			}
		}

		private void ExitOnError(string msg, Exception ex)
		{
			LogManager.WriteLog(1000, msg + ex.ToString(), null, true);
			Console.ReadLine();
			Process.GetCurrentProcess().Kill();
		}

		private void InitGameMapsAndMonsters()
		{
			XElement xelement = null;
			try
			{
				xelement = Global.GetGameResXml("Config/MapConfig.xml");
			}
			catch (Exception ex)
			{
				this.ExitOnError(string.Format("启动时加载xml文件: {0} 失败", "MapConfig.xml"), ex);
			}
			IEnumerable<XElement> enumerable = xelement.Elements();
			GameManager.ClientMgr.initialize(enumerable);
			GameManager.MonsterMgr.initialize(enumerable);
			MonsterStaticInfoMgr.Initialize();
			GameManager.MonsterZoneMgr.LoadAllMonsterXml();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (XElement xelement2 in enumerable)
			{
				int mapPicCodeByCode = Global.GetMapPicCodeByCode((int)Global.GetSafeAttributeLong(xelement2, "Code"));
				try
				{
					GameMap gameMap = GameManager.MapMgr.InitAddMap((int)Global.GetSafeAttributeLong(xelement2, "Code"), mapPicCodeByCode, 0, 0, (int)Global.GetSafeAttributeLong(xelement2, "BirthPosX"), (int)Global.GetSafeAttributeLong(xelement2, "BirthPosY"), (int)Global.GetSafeAttributeLong(xelement2, "BirthRadius"));
					GameManager.MapGridMgr.InitAddMapGrid((int)Global.GetSafeAttributeLong(xelement2, "Code"), gameMap.MapWidth, gameMap.MapHeight, GameManager.MapGridWidth, GameManager.MapGridHeight, gameMap);
					GameManager.MonsterZoneMgr.AddMapMonsters((int)Global.GetSafeAttributeLong(xelement2, "Code"), gameMap);
					NPCGeneralManager.LoadMapNPCRoles((int)Global.GetSafeAttributeLong(xelement2, "Code"), gameMap);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载地图配置时出错,xml={0}", xelement2.ToString()), ex, true);
				}
			}
			stopwatch.Stop();
			JunQiManager.InitLingDiJunQi();
			SysConOut.WriteLine(StringUtil.substitute("所有地图的怪物总的个数为:{0}, 耗时:{1}ms", new object[]
			{
				GameManager.MonsterMgr.GetTotalMonstersCount(),
				stopwatch.ElapsedMilliseconds
			}));
		}

		private void InitCache(XElement xml)
		{
			Global._FullBufferManager = new FullBufferManager();
			Global._SendBufferManager = new SendBufferManager();
			SendBuffer.SendDataIntervalTicks = (long)Global.GMax(20, Global.GMin(500, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataIntervalTicks")));
			SendBuffer.MaxSingleSocketSendBufferSize = Global.GMax(18000, Global.GMin(256000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "MaxSingleSocketSendBufferSize")));
			SendBuffer.SendDataTimeOutTicks = (long)Global.GMax(3000, Global.GMin(20000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataTimeOutTicks")));
			SendBuffer.MaxBufferSizeForLargePackge = SendBuffer.MaxSingleSocketSendBufferSize * 2 / 3;
			Global._MemoryManager = new MemoryManager();
			string safeAttributeStr = Global.GetSafeAttributeStr(xml, "CacheMemoryParam", "CacheMemoryBlocks");
			if (string.IsNullOrWhiteSpace(safeAttributeStr))
			{
				Global._MemoryManager.AddBatchBlock(100, 1500);
				Global._MemoryManager.AddBatchBlock(600, 400);
				Global._MemoryManager.AddBatchBlock(600, 50);
				Global._MemoryManager.AddBatchBlock(600, 100);
			}
			else
			{
				string[] array = safeAttributeStr.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					int num = int.Parse(array3[0]);
					int num2 = int.Parse(array3[1]);
					num2 = Global.GMax(num2, 80);
					if (num > 0 && num2 > 0)
					{
						Global._MemoryManager.AddBatchBlock(num2, num);
						GameManager.MemoryPoolConfigDict[num] = num2;
					}
				}
			}
		}

		private void InitTCPManager(XElement xml, bool bConnectDB)
		{
			if (bConnectDB)
			{
				GameManager.DefaultMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "Code");
				GameManager.MainMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "MainCode");
				GameManager.ServerLineID = (int)Global.GetSafeAttributeLong(xml, "Server", "LineID");
				GameManager.IsKuaFuServer = (GameManager.ServerLineID > 1);
				GameManager.AutoGiveGoodsIDList = null;
				LogManager.LogTypeToWrite = (int)Global.GetSafeAttributeLong(xml, "Server", "LogType");
				GameManager.SystemServerEvents.EventLevel = (EventLevels)Global.GetSafeAttributeLong(xml, "Server", "EventLevel");
				GameManager.SystemRoleTaskEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithTongQianEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYinLiangEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYinPiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleSaleEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents1.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents2.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleExchangeEvents3.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleGoodsEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleHorseEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBangGongEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleJingMaiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleRefreshQiZhenGeEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleWaBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleMapEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFuBenAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleWuXingAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRolePaoHuanOkEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleYaBiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleLianZhanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleHuoDongMonsterEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleDigTreasureWithYaoShiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleAutoSubEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithTianDiJingYuanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFetchVipAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleFetchMailMoneyEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				GameManager.SystemRoleBuyWithGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
				int num = Global.GMax(0, (int)Global.GetSafeAttributeLong(xml, "DBLog", "DBLogEnable"));
				this.InitCache(xml);
				try
				{
					Global.Flag_NameServer = true;
					NameServerNamager.Init(xml);
				}
				catch (Exception ex)
				{
					Global.Flag_NameServer = false;
					Console.WriteLine(ex.ToString());
				}
				int capacity = (int)Global.GetSafeAttributeLong(xml, "Socket", "capacity") * 3;
				TCPManager.getInstance().initialize(capacity);
				Global._TCPManager = TCPManager.getInstance();
				Global._TCPManager.tcpClientPool.RootWindow = this;
				Global._TCPManager.tcpClientPool.Init((int)Global.GetSafeAttributeLong(xml, "DBServer", "pool"), Global.GetSafeAttributeStr(xml, "DBServer", "ip"), (int)Global.GetSafeAttributeLong(xml, "DBServer", "port"), "DBServer");
				Global._TCPManager.tcpLogClientPool.RootWindow = this;
				Global._TCPManager.tcpLogClientPool.Init((int)Global.GetSafeAttributeLong(xml, "LogDBServer", "pool"), Global.GetSafeAttributeStr(xml, "LogDBServer", "ip"), (int)Global.GetSafeAttributeLong(xml, "LogDBServer", "port"), "LogDBServer");
				GameManager.systemGMCommands.InitGMCommands(null);
			}
			else
			{
				TCPCmdHandler.KeySHA1 = Global.GetSafeAttributeStr(xml, "Token", "sha1");
				TCPCmdHandler.KeyData = Global.GetSafeAttributeStr(xml, "Token", "data");
				TCPCmdHandler.WebKey = Global.GetSafeAttributeStr(xml, "Token", "webkey");
				TCPCmdHandler.WebKeyLocal = TCPCmdHandler.WebKey;
				string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("loginwebkey", TCPCmdHandler.WebKey);
				if (!string.IsNullOrEmpty(gameConfigItemStr) && gameConfigItemStr.Length >= 5)
				{
					TCPCmdHandler.WebKey = gameConfigItemStr;
				}
				Global._TCPManager.tcpRandKey.Init((int)Global.GetSafeAttributeLong(xml, "Token", "count"), (int)Global.GetSafeAttributeLong(xml, "Token", "randseed"));
				Global._TCPManager.RootWindow = this;
				Global._TCPManager.Start(Global.GetSafeAttributeStr(xml, "Socket", "ip"), (int)Global.GetSafeAttributeLong(xml, "Socket", "port"));
			}
		}

		public static void DyLoadConfig()
		{
		}

		private void InitGameManager(XElement xml)
		{
			GameManager.AppMainWnd = this;
			GameManager.SystemTasksMgr.LoadFromXMlFile("Config/SystemTasks.xml", "Tasks", "ID", 1);
			ProcessTask.InitBranchTasks(GameManager.SystemTasksMgr.SystemXmlItemDict);
			GameManager.NPCTasksMgr.LoadNPCTasks(GameManager.SystemTasksMgr);
			GameManager.SystemNPCsMgr.LoadFromXMlFile("Config/npcs.xml", "NPCs", "ID", 0);
			GameManager.SystemOperasMgr.LoadFromXMlFile("Config/SystemOperations.xml", "Operations", "ID", 0);
			GameManager.SystemGoods.LoadFromXMlFile("Config/Goods.xml", "Goods", "ID", 0);
			SingletonTemplate<GoodsCanUseManager>.Instance().Init();
			SingletonTemplate<GoodsReplaceManager>.Instance().Init();
			GameManager.NPCSaleListMgr.LoadSaleList();
			GameManager.SystemGoodsNamgMgr.LoadGoodsItemsDict(GameManager.SystemGoods);
			GameManager.SystemMagicActionMgr.ParseGoodsActions(GameManager.SystemGoods);
			GameManager.SystemMagicsMgr.LoadFromXMlFile("Config/Magics.xml", "Magics", "ID", 0);
			GameManager.SystemMagicQuickMgr.LoadMagicItemsDict(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr.ParseMagicActions(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr.ParseMagicActionRelations(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicActionMgr2.ParseMagicActions2(GameManager.SystemMagicsMgr);
			GameManager.SystemMagicScanTypeMgr.ParseScanTypeActions2(GameManager.SystemMagicsMgr);
			MagicsManyTimeDmageCachingMgr.ParseManyTimeDmageItems(GameManager.SystemMagicsMgr);
			GameManager.SystemPassiveMgr.LoadFromXMlFile("Config/PassiveEffect.xml", "", "ID", 0);
			GameManager.SystemPassiveEffectMgr.ParseMagicActions(GameManager.SystemPassiveMgr);
			GameManager.SystemMonsterGoodsList.LoadFromXMlFile("Config/MonsterGoodsList.xml", "", "ID", 1);
			GameManager.SystemLimitTimeMonsterGoodsList.LoadFromXMlFile("Config/HuoDongMonsterGoodsList.xml", "", "ID", 1);
			GameManager.SystemGoodsQuality.LoadFromXMlFile("Config/GoodsQuality.xml", "", "ID", 1);
			GameManager.SystemGoodsLevel.LoadFromXMlFile("Config/GoodsLevel.xml", "", "ID", 1);
			GameManager.SystemGoodsBornIndex.LoadFromXMlFile("Config/GoodsBorn.xml", "", "ID", 1);
			GameManager.SystemGoodsZhuiJia.LoadFromXMlFile("Config/GoodsZhuiJia.xml", "", "ID", 1);
			GameManager.SystemGoodsExcellenceProperty.LoadFromXMlFile("Config/ExcellencePropertyRandom.xml", "ExcellenceProperty", "ID", 1);
			GameManager.SystemBattle.LoadFromXMlFile("Config/Battle.xml", "", "ID", 0);
			GameManager.SystemBattlePaiMingAwards.LoadFromXMlFile("Config/BattlePaiMingAward.xml", "", "ID", 0);
			GameManager.SystemArenaBattle.LoadFromXMlFile("Config/ArenaBattle.xml", "", "ID", 0);
			GameManager.systemNPCScripts.LoadFromXMlFile("Config/NPCScripts.xml", "Scripts", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseNPCScriptActions(GameManager.systemNPCScripts);
			GameManager.systemPets.LoadFromXMlFile("Config/Pet.xml", "Pets", "ID", 0);
			HorseCachingManager.LoadHorseEnchanceItems();
			GameManager.systemGoodsMergeTypes.LoadFromXMlFile("Config/GoodsMergeType.xml", "Types", "ID", 1);
			GameManager.systemGoodsMergeItems.LoadFromXMlFile("Config/GoodsMergeItems.xml", "Items", "ID", 1);
			GameManager.systemParamsList.LoadParamsList();
			BuffManager.InitConfig();
			GameManager.systemMallMgr.LoadFromXMlFile("Config/Mall.xml", "Mall", "ID", 1);
			GongGaoDataManager.LoadGongGaoData();
			JingMaiCacheManager.LoadJingMaiItems();
			MagicsCacheManager.LoadMagicItems();
			TimerBossManager.getInstance();
			GameManager.systemJingMaiExpMgr.LoadFromXMlFile("Config/JingMaiExp.xml", "", "ID", 0);
			GameManager.systemGoodsBaoGuoMgr.LoadFromXMlFile("Config/GoodsPack.xml", "", "ID", 0);
			GameManager.systemWaBaoMgr.LoadFromXMlFile("Config/Dig.xml", "", "ID", 0);
			GameManager.systemWeekLoginGiftMgr.LoadFromXMlFile("Config/Gifts/LoginNumGift.xml", "", "ID", 1);
			GameManager.systemMOnlineTimeGiftMgr.LoadFromXMlFile("Config/Gifts/OnlieTimeGift.xml", "", "ID", 1);
			GameManager.systemNewRoleGiftMgr.LoadFromXMlFile("Config/Gifts/NewRoleGift.xml", "", "ID", 1);
			GameManager.systemCombatAwardMgr.LoadFromXMlFile("Config/Gifts/ComatEffectivenessGift.xml", "", "ID", 1);
			GameManager.systemUpLevelGiftMgr.LoadFromXMlFile("Config/Gifts/UpLevelGift.xml", "", "ID", 1);
			GameManager.systemFuBenMgr.LoadFromXMlFile("Config/FuBen.xml", "", "ID", 0);
			GameManager.systemYaBiaoMgr.LoadFromXMlFile("Config/Yabiao.xml", "", "ID", 0);
			GameManager.systemSpecialTimeMgr.LoadFromXMlFile("Config/SpecialTimes.xml", "", "ID", 1);
			GameManager.systemHeroConfigMgr.LoadFromXMlFile("Config/Hero.xml", "", "ID", 0);
			GameManager.systemBangHuiFlagUpLevelMgr.LoadFromXMlFile("Config/FlagUpLevel.xml", "Flag", "ID", 0);
			GameManager.systemJunQiMgr.LoadFromXMlFile("Config/JunQi.xml", "", "ID", 0);
			GameManager.systemQiZuoMgr.LoadFromXMlFile("Config/QiZuo.xml", "", "ID", 0);
			GameManager.systemLingQiMapQiZhiMgr.LoadFromXMlFile("Config/LingDiQiZhi.xml", "", "ID", 0);
			GameManager.systemQiZhenGeGoodsMgr.LoadFromXMlFile("Config/QiZhenGeGoods.xml", "Mall", "ID", 0);
			GameManager.systemHuangChengFuHuoMgr.LoadFromXMlFile("Config/HuangCheng.xml", "", "ID", 0);
			GameManager.systemBattleExpMgr.LoadFromXMlFile("Config/BattleExp.xml", "", "ID", 0);
			GameManager.systemBangZhanAwardsMgr.LoadFromXMlFile("Config/BangZhanAward.xml", "", "ID", 0);
			GameManager.systemBattleRebirthMgr.LoadFromXMlFile("Config/Rebirth.xml", "", "ID", 0);
			GameManager.systemBattleAwardMgr.LoadFromXMlFile("Config/BattleAward.xml", "", "ID", 0);
			GameManager.systemEquipBornMgr.LoadFromXMlFile("Config/EquipBorn.xml", "", "ID", 0);
			GameManager.systemBornNameMgr.LoadFromXMlFile("Config/BornName.xml", "", "ID", 0);
			GameManager.systemVipDailyAwardsMgr.LoadFromXMlFile("Config/Gifts/VipDailyAwards.xml", "", "AwardID", 1);
			GameManager.systemActivityTipMgr.LoadFromXMlFile("Config/Activity/ActivityTip.xml", "", "ID", 0);
			GameManager.systemLuckyAwardMgr.LoadFromXMlFile("Config/LuckyAward.xml", "", "ID", 0);
			GameManager.systemLuckyAward2Mgr.LoadFromXMlFile("Config/LuckyAward2.xml", "", "ID", 0);
			GameManager.systemLuckyMgr.LoadFromXMlFile("Config/Lucky.xml", "", "Number", 0);
			GameManager.systemChengJiu.LoadFromXMlFile("Config/ChengJiu.xml", "ChengJiu", "ChengJiuID", 0);
			ChengJiuManager.InitChengJiuConfig();
			GameManager.systemChengJiuBuffer.LoadFromXMlFile("Config/ChengJiuBuff.xml", "", "ID", 0);
			GameManager.systemWeaponTongLing.LoadFromXMlFile("Config/TongLing.xml", "", "ID", 0);
			QianKunManager.LoadImpetrateItemsInfo();
			QianKunManager.LoadImpetrateItemsInfoFree();
			QianKunManager.LoadImpetrateItemsInfoHuodong();
			QianKunManager.LoadImpetrateItemsInfoFreeHuoDong();
			QianKunManager.LoadImpetrateItemsInfoTeQuan();
			QianKunManager.LoadImpetrateItemsInfoFreeTeQuan();
			GameManager.systemImpetrateByLevelMgr.LoadFromXMlFile("Config/DigType.xml", "", "ID", 0);
			GameManager.systemXingYunChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig1.xml", "", "ID", 0);
			GameManager.systemYueDuZhuanPanChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig2.xml", "GiftList", "ID", 0);
			GameManager.systemEveryDayOnLineAwardMgr.LoadFromXMlFile("Config/Gifts/MUNewRoleGift.xml", "", "ID", 1);
			GameManager.systemSeriesLoginAwardMgr.LoadFromXMlFile("Config/Gifts/MULoginNumGift.xml", "", "ID", 1);
			GameManager.systemMonsterMgr.LoadFromXMlFile("Config/Monsters.xml", "Monsters", "ID", 0);
			GameManager.SystemJingMaiLevel.LoadFromXMlFile("Config/JingMai.xml", "", "ID", 0);
			GameManager.SystemWuXueLevel.LoadFromXMlFile("Config/WuXue.xml", "", "ID", 0);
			GameManager.SystemTaskPlots.LoadFromXMlFile("Config/TaskPlot.xml", "", "ID", 1);
			GameManager.SystemQiangGou.LoadFromXMlFile("Config/QiangGou.xml", "", "ID", 1);
			GameManager.SystemHeFuQiangGou.LoadFromXMlFile("Config/HeFuGifts/HeFuQiangGou.xml", "", "ID", 0);
			GameManager.SystemJieRiQiangGou.LoadFromXMlFile("Config/JieRiGifts/JieRiQiangGou.xml", "", "ID", 0);
			GameManager.SystemZuanHuangLevel.LoadFromXMlFile("Config/ZuanHuang.xml", "", "ID", 0);
			GameManager.SystemSystemOpen.LoadFromXMlFile("Config/SystemOpen.xml", "", "ID", 0);
			GameManager.SystemDropMoney.LoadFromXMlFile("Config/DropMoney.xml", "", "ID", 0);
			GameManager.SystemDengLuDali.LoadFromXMlFile("Config/Gifts/HuoDongLoginNumGift.xml", "GoodsList", "ID", 1);
			GameManager.SystemBuChang.LoadFromXMlFile("Config/BuChang.xml", "", "ID", 0);
			GameManager.SystemZhanHunLevel.LoadFromXMlFile("Config/ZhanHun.xml", "", "ID", 0);
			GameManager.SystemRongYuLevel.LoadFromXMlFile("Config/RongYu.xml", "", "ID", 0);
			GameManager.SystemExchangeMoJingAndQiFu.LoadFromXMlFile("Config/DuiHuanItems.xml", "Items", "ID", 1);
			GameManager.SystemExchangeType.LoadFromXMlFile("Config/DuiHuanType.xml", "DuiHuan", "ID", 1);
			GameManager.systemCaiJiMonsterMgr.LoadFromXMlFile("Config/CrystalMonster.xml", "", "MonsterID", 0);
			GameManager.SystemDamonUpgrade.LoadFromXMlFile("Config/PetLevelUp.xml", "", "ID", 0);
			GameManager.QingGongYanMgr.LoadQingGongYanConfig();
			CopyTargetManager.LoadConfig();
			CallPetManager.LoadCallPetType();
			CallPetManager.LoadCallPetConfig();
			CallPetManager.LoadCallPetSystem();
			WingStarCacheManager.LoadWingStarItems();
			Global.LoadVipLevelAwardList();
			ChengJiuManager.InitFlagIndex();
			ChengJiuManager.initAchievementRune();
			EquipUpgradeCacheMgr.LoadEquipUpgradeItems();
			FuBenManager.LoadFuBenMap();
			GoodsBaoGuoCachingMgr.LoadGoodsBaoGuoDict();
			WuXingMapMgr.LoadXuXingConfig();
			WuXingMapMgr.LoadWuXingAward();
			BroadcastInfoMgr.LoadBroadcastInfoItemList();
			PopupWinMgr.LoadPopupWinItemList();
			MallGoodsMgr.InitMallGoodsPriceDict();
			ChuanQiQianHua.LoadEquipQianHuaProps();
			this.LoadCopyScoreDataInfo();
			this.LoadFreshPlayerCopySceneInfo();
			this.LoadTaskStarDataInfo();
			this.LoadDailyCircleTaskAwardInfo();
			this.LoadTaofaTaskAwardInfo();
			this.LoadCombatForceInfoInfo();
			this.LoadDaimonSquareDataInfo();
			this.LoadSystemParamsDataForCache();
			Program.LoadTotalLoginDataInfo();
			this.LoadVIPDataInfo();
			this.LoadVIPLevAwardAndExpInfo();
			this.LoadMeditateInfo();
			GameManager.systemDailyActiveInfo.LoadFromXMlFile("Config/DailyActiveInfor.xml", "DailyActive", "DailyActiveID", 0);
			GameManager.systemDailyActiveAward.LoadFromXMlFile("Config/DailyActiveAward.xml", "DailyActiveAward", "ID", 0);
			DailyActiveManager.InitDailyActiveFlagIndex();
			this.LoadExperienceCopyMapDataInfo();
			GameManager.systemAngelTempleData.LoadFromXMlFile("Config/AngelTemple.xml", "", "ID", 0);
			GameManager.AngelTempleAward.LoadFromXMlFile("Config/AngelTempleAward.xml", "", "ID", 0);
			GameManager.AngelTempleLuckyAward.LoadFromXMlFile("Config/AngelTempleLuckyAward.xml", "", "ID", 0);
			GameManager.TaskZhangJie.LoadFromXMlFile("Config/TaskZhangJie.xml", "", "ID", 1);
			ReloadXmlManager.InitTaskZhangJieInfo();
			GameManager.JiaoYiTab.LoadFromXMlFile("Config/JiaoYiTab.xml", "", "TabID", 0);
			GameManager.JiaoYiType.LoadFromXMlFile("Config/JiaoYiType.xml", "", "ID", 0);
			GameManager.SystemZhanMengBuild.LoadFromXMlFile("Config/ZhanMengBuild.xml", "", "ID", 0);
			GameManager.SystemWingsUp.LoadFromXMlFile("Config/Wing/WingUp.xml", "", "Level", 0);
			GameManager.SystemBossAI.LoadFromXMlFile("Config/AI.xml", "", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseBossAIActions(GameManager.SystemBossAI);
			BossAICachingMgr.LoadBossAICachingItems(GameManager.SystemBossAI);
			GameManager.SystemExtensionProps.LoadFromXMlFile("Config/TuoZhan.xml", "", "ID", 0);
			GameManager.SystemMagicActionMgr.ParseExtensionPropsActions(GameManager.SystemExtensionProps);
			ExtensionPropsMgr.LoadCachingItems(GameManager.SystemExtensionProps);
			this.LoadBossHomeInfo();
			this.LoadGoldTempleInfo();
			this.LoadFuBenNeedInfo();
			this.LoadEquipUpgradeInfo();
			this.LoadGoldCopySceneInfo();
			GameManager.MagicSwordMgr.LoadMagicSwordData();
			GameManager.SummonerMgr.LoadSummonerData();
			GameManager.MerlinMagicBookMgr.LoadMerlinSystemParamsConfigData();
			GameManager.MerlinMagicBookMgr.LoadMerlinConfigData();
			GameManager.FluorescentGemMgr.LoadFluorescentGemConfigData();
			SingletonTemplate<GetInterestingDataMgr>.Instance().LoadConfig();
			Global.LoadSpecialMachineConfig();
			ElementhrtsManager.LoadRefineType();
			ElementhrtsManager.LoadElementHrtsBase();
			ElementhrtsManager.LoadElementHrtsLevelInfo();
			ElementhrtsManager.LoadSpecialElementHrtsExp();
			this.LoadEquipJuHunInfo();
			this.LoadBagType();
			WeaponMaster.LoadWeaponMaster();
			MoYuLongXue.LoadMoYuXml();
			ZhuanShengShiLian.LoadZhuanShengShiLianXml();
			GameManager.PlatConfigMgr.LoadPlatConfig();
			UserMoneyMgr.getInstance().InitConfig();
			Program.LoadIPList("");
			GameManager.BattleMgr.Init();
			GameManager.ArenaBattleMgr.Init();
			GameManager.ShengXiaoGuessMgr.Init();
			GameManager.BulletinMsgMgr.LoadBulletinMsgFromDBServer();
			JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();
			JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
			JunQiManager.ParseWeekDaysTimes();
			Global.InitBagParams();
			Global.InitGuMuMapCodes();
			Global.InitVipGumuExpMultiple();
			Global.InitMingJieMapCodeList();
			Global.InitDecreaseInjureInfo();
			Global.InitAllForgeLevelInfo();
			Global.LoadItemLogMark();
			Global.LoadLogTradeGoods();
			Global.LoadForgeSystemParams();
			Global.LoadReliveMonsterGongGaoMark();
			ArtifactManager.initArtifact();
			if (!HuodongCachingMgr.LoadActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			if (!HuodongCachingMgr.LoadHeFuActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			if (!HuodongCachingMgr.LoadJieriActivitiesConfig())
			{
				Process.GetCurrentProcess().Kill();
			}
			Global.InitMapSceneTypeDict();
			GameManager.AngelTempleMgr.InitAngelTemple();
			GameManager.BloodCastleCopySceneMgr.InitBloodCastleCopyScene();
			GameManager.DaimonSquareCopySceneMgr.InitDaimonSquareCopyScene();
			GameManager.StarConstellationMgr.LoadStarConstellationTypeInfo();
			GameManager.StarConstellationMgr.LoadStarConstellationDetailInfo();
			CaiJiLogic.LoadConfig();
			GameManager.GuildCopyMapMgr.LoadGuildCopyMapOrder();
			LingYuManager.LoadConfig();
			ZhuLingZhuHunManager.LoadConfig();
			YueKaManager.LoadConfig();
			UpgradeDamon.LoadUpgradeAttr();
			GameManager.VersionSystemOpenMgr.LoadVersionSystemOpenData();
			SingletonTemplate<TuJianManager>.Instance().LoadConfig();
			SingletonTemplate<GuardStatueManager>.Instance().LoadConfig();
			GameManager.loginWaitLogic.LoadConfig();
			GameFuncControlManager.LoadConfig(Global.GameResPath("Config/GameFuncControl.xml"));
			Data.ThemeActivityState = HuodongCachingMgr.GetThemeActivityState();
			if (Data.ThemeActivityState == 0 && Data.ZhuTiID > 0)
			{
				LogManager.WriteLog(1000, "主题服开关配置ThemeActivityState和ZhuTiID配置不一致", null, true);
			}
			LogFilterConfig.InitConfig();
			GoldAuctionConfigModel.LoadConfig();
			BoCaiConfigMgr.LoadConfig(false);
		}

		private void InitGameConfigWithDB()
		{
			GameManager.ServerId = Global.sendToDB<int, string>(11002, "", 0);
			GameManager.PTID = Global.sendToDB<int, string>(11005, "", 0);
			GameManager.KuaFuServerId = ConstData.ConvertToKuaFuServerID(GameManager.ServerId, GameManager.PTID);
			GameManager.Flag_OptimizationBagReset = (GameManager.GameConfigMgr.GetGameConfigItemInt("optimization_bag_reset", 1) > 0);
			GameManager.SetLogFlags((long)GameManager.GameConfigMgr.GetGameConfigItemInt("logflags", int.MaxValue));
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "");
			for (PlatformTypes platformTypes = 0; platformTypes < 8; platformTypes++)
			{
				if (0 == string.Compare(gameConfigItemStr, platformTypes.ToString(), true))
				{
					GameManager.PlatformType = platformTypes;
					break;
				}
			}
			if (gameConfigItemStr == "andrid")
			{
				GameManager.PlatformType = 3;
			}
			if (0 == GameManager.PlatformType)
			{
				throw new Exception(string.Format("t_config platformtype wrong!!!", new object[0]));
			}
			if (GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", -1) <= 0)
			{
				throw new Exception(string.Format("t_config money-to-yuanbao wrong!!!", new object[0]));
			}
			GameManager.LoadGameConfigFlags();
		}

		private void InitMonsterManager()
		{
		}

		protected static void StartThreadPoolDriverTimer()
		{
			Program.ThreadPoolDriverTimer = new Timer(new TimerCallback(Program.ThreadPoolDriverTimer_Tick), null, 1000, 1000);
			Program.LogThreadPoolDriverTimer = new Timer(new TimerCallback(Program.LogThreadPoolDriverTimer_Tick), null, 500, 500);
		}

		protected static void StopThreadPoolDriverTimer()
		{
			Program.ThreadPoolDriverTimer.Change(-1, -1);
			Program.LogThreadPoolDriverTimer.Change(-1, -1);
		}

		protected static void ThreadPoolDriverTimer_Tick(object sender)
		{
			try
			{
				Program.ServerConsole.ExecuteBackgroundWorkers(null, EventArgs.Empty);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ThreadPoolDriverTimer_Tick", false, false);
			}
		}

		public static void LogThreadPoolDriverTimer_Tick(object sender)
		{
			try
			{
				Program.ServerConsole.ExecuteBackgroundLogWorkers(null, EventArgs.Empty);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "LogThreadPoolDriverTimer_Tick", false, false);
			}
		}

		private void ExecuteBackgroundLogWorkers(object sender, EventArgs e)
		{
			try
			{
				if (!this.logDBCommandWorker.IsBusy)
				{
					this.logDBCommandWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "logDBCommandWorker", false, false);
			}
		}

		private void ExecuteBackgroundWorkers(object sender, EventArgs e)
		{
			try
			{
				if (!this.eventWorker.IsBusy)
				{
					this.eventWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "eventWorker", false, false);
			}
			try
			{
				if (!this.dbCommandWorker.IsBusy)
				{
					this.dbCommandWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "dbCommandWorker", false, false);
			}
			try
			{
				if (!this.clientsWorker.IsBusy)
				{
					this.clientsWorker.RunWorkerAsync(0);
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "clientsWorker", false, false);
			}
			try
			{
				if (!this.buffersWorker.IsBusy)
				{
					this.buffersWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "buffersWorker", false, false);
			}
			try
			{
				if (!this.spriteDBWorker.IsBusy)
				{
					this.spriteDBWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "spriteDBWorker", false, false);
			}
			try
			{
				if (!this.othersWorker.IsBusy)
				{
					this.othersWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "othersWorker", false, false);
			}
			try
			{
				if (!this.FightingWorker.IsBusy)
				{
					this.FightingWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "FightingWorker", false, false);
			}
			try
			{
				if (!this.chatMsgWorker.IsBusy)
				{
					this.chatMsgWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "chatMsgWorker", false, false);
			}
			try
			{
				if (!this.fuBenWorker.IsBusy)
				{
					this.fuBenWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "fuBenWorker", false, false);
			}
			try
			{
				if (!this.dbWriterWorker.IsBusy)
				{
					this.dbWriterWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "dbWriterWorker", false, false);
			}
			try
			{
				if (!this.SocketSendCacheDataWorker.IsBusy)
				{
					this.SocketSendCacheDataWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "SocketSendCacheDataWorker", false, false);
			}
			try
			{
				if (!this.ShengXiaoGuessWorker.IsBusy)
				{
					this.ShengXiaoGuessWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "ShengXiaoGuessWorker", false, false);
			}
			try
			{
				if (!this.socketCheckWorker.IsBusy)
				{
					this.socketCheckWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "socketCheckWorker", false, false);
			}
			try
			{
				if (!this.BanWorker.IsBusy)
				{
					this.BanWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "BanWorker", false, false);
			}
			try
			{
				if (!this.IPStatisticsWorker.IsBusy)
				{
					this.IPStatisticsWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "BanWorker", false, false);
			}
			try
			{
				if (!this.TwLogWorker.IsBusy)
				{
					this.TwLogWorker.RunWorkerAsync();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "TwLogWorker", false, false);
			}
			Program.CalcGCInfo();
		}

		private void closingTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				string title = "";
				GameClient randomClient = GameManager.ClientMgr.GetRandomClient();
				if (null != randomClient)
				{
					title = string.Format("游戏服务器{0}, 关闭中, 剩余{1}个角色", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());
					Global.ForceCloseClient(randomClient, "游戏服务器关闭", true);
				}
				else
				{
					this.ClosingCounter -= 200;
					if (this.ClosingCounter <= 0)
					{
						Global._SendBufferManager.Exit = true;
						this.MustCloseNow = true;
					}
					else
					{
						int num = GameManager.DBCmdMgr.GetDBCmdCount() + this.ClosingCounter / 200;
						title = string.Format("游戏服务器{0}, 关闭中, 倒计时:{1}", GameManager.ServerLineID, num);
					}
				}
				Console.Title = title;
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "closingTimer_Tick", false, false);
			}
		}

		public void auxiliaryTimer_Tick(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			try
			{
				long num2 = TimeUtil.NOW();
				if (num2 - this.LastAuxiliaryTicks > 1000L)
				{
					this.DoLog(string.Format("\r\nauxiliaryTimer_Tick开始执行经过时间:{0}毫秒", num2 - this.LastAuxiliaryTicks));
				}
				this.LastAuxiliaryTicks = num2;
				num2 = TimeUtil.NOW();
				num2 = TimeUtil.NOW();
				long num3 = TimeUtil.NOW();
				num2 = TimeUtil.NOW();
				Global._TCPManager.tcpClientPool.Supply();
				Global._TCPManager.tcpLogClientPool.Supply();
				num3 = TimeUtil.NOW();
				if (num3 > num2 + 1000L)
				{
					this.DoLog(string.Format("tcpClientPool.Supply 消耗:{0}毫秒", num3 - num2));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "auxiliaryTimer_Tick", false, false);
			}
			long num4 = TimeUtil.NOW();
			if (num4 > num + 1000L)
			{
				this.DoLog(string.Format("auxiliaryTimer_Tick 消耗:{0}毫秒", num4 - num));
			}
		}

		public void dynamicMonsterTimer_Tick(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			try
			{
				long num2 = TimeUtil.NOW();
				if (num2 - this.LastDynamicMonsterTicks > 1000L)
				{
					this.DoLog(string.Format("\r\ndynamicMonsterTimer_Tick开始执行经过时间:{0}毫秒", num2 - this.LastDynamicMonsterTicks));
				}
				this.LastDynamicMonsterTicks = num2;
				num2 = TimeUtil.NOW();
				GameManager.MonsterZoneMgr.RunMapMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long num3 = TimeUtil.NOW();
				if (num3 > num2 + 1000L)
				{
					this.DoLog(string.Format("RunMapMonsters 消耗:{0}毫秒", num3 - num2));
				}
				num2 = TimeUtil.NOW();
				GameManager.MonsterZoneMgr.RunMapDynamicMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				num3 = TimeUtil.NOW();
				if (num3 > num2 + 1000L)
				{
					this.DoLog(string.Format("RunMapDynamicMonsters 消耗:{0}毫秒", num3 - num2));
				}
				num2 = TimeUtil.NOW();
				GameManager.MonsterMgr.DoMonsterDeadCall();
				num3 = TimeUtil.NOW();
				if (num3 > num2 + 1000L)
				{
					this.DoLog(string.Format("DoMonsterDeadCall 消耗:{0}毫秒", num3 - num2));
				}
				if (num3 > this.LastMonsterUniqueIdProcTicks)
				{
					num2 = num3;
					this.LastMonsterUniqueIdProcTicks = num3 + 60000L;
					GameManager.MonsterMgr.DoDeadMonsterUniqueIdProc(num2);
					num3 = TimeUtil.NOW();
					if (num3 > num2 + 1000L)
					{
						this.DoLog(string.Format("DoDeadMonsterUniqueIdProc 消耗:{0}毫秒", num3 - num2));
					}
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "dynamicMonsterTimer_Tick", false, false);
			}
			long num4 = TimeUtil.NOW();
			if (num4 > num + 1000L)
			{
				this.DoLog(string.Format("dynamicMonsterTimer_Tick 消耗:{0}毫秒", num4 - num));
			}
		}

		private void DoLog(string warning)
		{
			LogManager.WriteLog(2, warning, null, true);
		}

		private void MainDispatcherWorker_DoWork(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			long num3 = TimeUtil.NOW();
			int num4 = 100;
			int num5 = 0;
			for (;;)
			{
				try
				{
					num2 = TimeUtil.NOW();
					if (num2 - num >= 500L)
					{
						GameManager.GM_NoCheckTokenTimeRemainMS -= num2 - num;
						num = num2;
						this.auxiliaryTimer_Tick(null, null);
						GlobalEventSource.getInstance().fireEvent(new GameRunningEventObject());
					}
					if (Program.NeedExitServer)
					{
						this.closingTimer_Tick(null, null);
						if (this.MustCloseNow)
						{
							break;
						}
					}
					num3 = TimeUtil.NOW();
					int num6 = (int)Math.Max(5L, (long)num4 - (num3 - num2));
					long num7 = TimeUtil.NOW();
					long num8 = DateTime.Now.Ticks / 10000L;
					Thread.Sleep(num6);
					long num9 = TimeUtil.NOW();
					long num10 = DateTime.Now.Ticks / 10000L;
					if (num9 - num7 > (long)(num6 + 1000))
					{
						LogManager.WriteLog(2, string.Format("MainDispatcherWorker_DoWork sleepMs={0} endTicks={1} dataTimeTicks={2}", num6, num9 - num7, num10 - num8), null, true);
					}
					num5++;
					if (num5 >= 100000)
					{
						num5 = 0;
					}
					if (0 != Program.GetServerPIDFromFile())
					{
						Program.OnExitServer();
					}
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "MainDispatcherWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("主循环线程退出，回车退出系统");
			if (0 != Program.GetServerPIDFromFile())
			{
				Program.WritePIDToFile("Stop.txt");
				Program.StopThreadPoolDriverTimer();
			}
		}

		private void TwLogWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				TwLogManager.ScanLog();
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "TwLogWorker_DoWork", false, false);
			}
		}

		private void IPStatisticsWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				GlobalEventSource.getInstance().DispatchEventAsync();
				IPStatisticsManager.getInstance().TimerProcForIP();
				IPStatisticsManager.getInstance().TimerProcForUserID();
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "IPStatisticsWorker_DoWork", false, false);
			}
		}

		private void LoadBanWorker_DoWork(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			long num3 = TimeUtil.NOW();
			int num4 = 20;
			for (;;)
			{
				try
				{
					num2 = TimeUtil.NOW();
					if (num2 - num >= 20L)
					{
						num = num2;
						FileBanLogic.Tick();
						BanManager.CheckBanMemory();
					}
					num3 = TimeUtil.NOW();
					int millisecondsTimeout = (int)Math.Max(5L, (long)num4 - (num3 - num2));
					Thread.Sleep(millisecondsTimeout);
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "LoadBanWorker_DoWork", false, false);
				}
			}
		}

		private void DynamicMonstersWorker_DoWork(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			long num3 = TimeUtil.NOW();
			int num4 = 100;
			int num5 = 0;
			for (;;)
			{
				try
				{
					num2 = TimeUtil.NOW();
					if (num2 - num >= 100L)
					{
						num = num2;
						this.dynamicMonsterTimer_Tick(null, null);
					}
					if (Program.NeedExitServer)
					{
						if (num5 % 2 == 0)
						{
							if (this.MustCloseNow)
							{
								break;
							}
						}
					}
					num3 = TimeUtil.NOW();
					int num6 = (int)Math.Max(5L, (long)num4 - (num3 - num2));
					if (num6 > num4)
					{
						num6 = num4;
						LogManager.WriteLog(10, string.Format("TimeMismatch#DynamicMonstersWorker_DoWork,startTicks={0},endTicks={1}", num2, num3), null, true);
					}
					GameManager.LastFlushMonsterMs = num;
					Thread.Sleep(num6);
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "DynamicMonstersWorker_DoWork", false, false);
				}
			}
		}

		private void RoleStroyboardDispatcherWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			int num3 = 100;
			int num4 = 0;
			for (;;)
			{
				try
				{
					num = TimeUtil.NOW();
					long num5 = num;
					StoryBoard4Client.runStoryBoards();
					long num6 = TimeUtil.NOW();
					if (num6 > num5 + 1000L)
					{
						this.DoLog(string.Format("StoryBoard4Client.runStoryBoards 消耗:{0}毫秒", num6 - num5));
					}
					if (Program.NeedExitServer)
					{
						if (num4 % 2 == 0)
						{
							this.closingTimer_Tick(null, null);
							if (this.MustCloseNow)
							{
								break;
							}
						}
					}
					num2 = TimeUtil.NOW();
					int millisecondsTimeout = (int)Math.Max(5L, (long)num3 - (num2 - num));
					Thread.Sleep(millisecondsTimeout);
					num4++;
					if (num4 >= 100000)
					{
						num4 = 0;
					}
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "RoleStroyboardDispatcherWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("角色故事版驱动线程退出，回车退出系统");
		}

		private void eventWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				while (GameManager.SystemServerEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleTaskEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithTongQianEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYinLiangEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJunGongEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYinPiaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleQiZhenGeBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleQiangGouBuyWithYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleSaleEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents1.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents2.WriteEvent())
				{
				}
				while (GameManager.SystemRoleExchangeEvents3.WriteEvent())
				{
				}
				while (GameManager.SystemRoleGoodsEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleHorseEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBangGongEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleJingMaiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleRefreshQiZhenGeEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleWaBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleMapEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFuBenAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleWuXingAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRolePaoHuanOkEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleYaBiaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleLianZhanEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleHuoDongMonsterEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubYuanBaoEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubGoldEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleAutoSubEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleDigTreasureWithYaoShiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithTianDiJingYuanEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFetchMailMoneyEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleFetchVipAwardEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithGoldEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJingYuanZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithLieShaZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithZhuangBeiJiFenEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithJunGongZhiEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleBuyWithZhanHunEvents.WriteEvent())
				{
				}
				while (GameManager.SystemGlobalGameEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleGameEvents.WriteEvent())
				{
				}
				while (GameManager.SystemClientLogsEvents.WriteEvent())
				{
				}
				while (GameManager.SystemRoleConsumeEvents.WriteEvent())
				{
				}
				EventLogManager.WriteAllEvents();
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "eventWorker_DoWork", false, false);
			}
		}

		private void dbCommandWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				DelayForceClosingMgr.ProcessDelaySockets();
				GameManager.DBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool);
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "dbCommandWorker_DoWork", false, false);
			}
		}

		private void logDBCommandWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				GameManager.logDBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpLogClientPool, Global._TCPManager.TcpOutPacketPool);
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "logDBCommandWorker_DoWork", false, false);
			}
		}

		private void clientsWorker_DoWork(object sender, EventArgs e)
		{
			DoWorkEventArgs doWorkEventArgs = e as DoWorkEventArgs;
			try
			{
				long num = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteBackgourndWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("clientsWorker_DoWork{0} 消耗:{1}毫秒", (int)doWorkEventArgs.Argument, num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, string.Format("clientsWorker_DoWork{0}", (int)doWorkEventArgs.Argument), false, false);
			}
		}

		private void buffersWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteBuffersWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("buffersWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, string.Format("buffersWorker_DoWork", new object[0]), false, false);
			}
		}

		private void spriteDBWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.ClientMgr.DoSpriteDBWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("spriteDBWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, string.Format("spriteDBWorker_DoWork", new object[0]), false, false);
			}
		}

		private void othersWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.GridMagicHelperMgr.ExecuteAllItems();
				GameManager.BulletinMsgMgr.ProcessBulletinMsg();
				DecorationManager.ProcessAllDecos(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				GameManager.GoodsPackMgr.ProcessAllGoodsPackItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				BiaoCheManager.ProcessAllBiaoCheItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				FakeRoleManager.ProcessAllFakeRoleItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				SpecailTimeManager.ProcessDoulbeExperience();
				HuodongCachingMgr.ProcessKaiFuGiftAwardActions();
				DelayActionManager.HeartBeatDelayAction();
				GameManager.QingGongYanMgr.CheckQingGongYan(num);
				HuodongCachingMgr.CheckJieRiActivityState(num);
				UserReturnManager.getInstance().CheckUserReturnOpenState(num);
				OlympicsManager.getInstance().CheckOlympicsOpenState(num, false);
				WorldLevelManager.getInstance().ResetWorldLevel();
				MarryPartyLogic.getInstance().MarryPartyPeriodicUpdate(num);
				MarryLogic.ApplyPeriodicClear(num);
				GameManager.loginWaitLogic.Tick();
				TimeUtil.RecordTimeAnchor();
				JieriPlatChargeKing jieriPlatChargeKingActivity = HuodongCachingMgr.GetJieriPlatChargeKingActivity();
				if (jieriPlatChargeKingActivity != null)
				{
					jieriPlatChargeKingActivity.Update();
				}
				JieriPlatChargeKingEveryDay jieriPCKingEveryDayActivity = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
				if (jieriPCKingEveryDayActivity != null)
				{
					jieriPCKingEveryDayActivity.Update();
				}
				GameManager.ServerMonitor.CheckReport();
				SingletonTemplate<TradeBlackManager>.Instance().Update();
				AoYunDaTiManager.getInstance().AoyunDaTiTimer_Work();
				ZhuanPanManager.getInstance().ZhuanPanTimer_Work();
				YaoSaiMissionManager.getInstance().YaoSaiMissionTimer_Work();
				EraManager.getInstance().EraTimer_Work();
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					specPriorityActivity.TimerProc();
				}
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("othersWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "othersWorker_DoWork", false, false);
			}
		}

		private void FightingWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.ArenaBattleMgr.Process();
				GameManager.BattleMgr.Process();
				GameManager.DJRoomMgr.ProcessFighting();
				JunQiManager.ProcessAllJunQiItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);
				JunQiManager.ProcessLingDiZhanResult();
				LuoLanChengZhanManager.getInstance().ProcessWangChengZhanResult();
				KarenBattleManager_MapWest.getInstance().TimerProc();
				KarenBattleManager_MapEast.getInstance().TimerProc();
				GameManager.AngelTempleMgr.HeartBeatAngelTempleScene();
				GameManager.BosshomeMgr.HeartBeatBossHomeScene();
				GameManager.GoldTempleMgr.HeartBeatGoldtempleScene();
				ZhuanShengShiLian.TimerProc();
				ThemeBoss.getInstance().TimerProc();
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("FightingWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "FightingWorker_DoWork", false, false);
			}
		}

		private void ShengXiaoGuessWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.ShengXiaoGuessMgr.Process();
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("ShengXiaoGuessWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "ShengXiaoGuessWorker_DoWork", false, false);
			}
		}

		private void chatMsgWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				BanChatManager.GetBanChatDictFromDBServer();
				GameManager.ClientMgr.HandleTransferChatMsg();
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("chatMsgWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "chatMsgWorker_DoWork", false, false);
			}
		}

		private void fuBenWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				GameManager.CopyMapMgr.CheckCopyTeamDamage(num, false);
				GameManager.CopyMapMgr.ProcessEndCopyMap();
				GameManager.CopyMapMgr.ProcessEndGuildCopyMapFlag();
				GameManager.CopyMapMgr.ProcessEndGuildCopyMap(num);
				FreshPlayerCopySceneManager.HeartBeatFreshPlayerCopyMap();
				ExperienceCopySceneManager.HeartBeatExperienceCopyMap();
				GlodCopySceneManager.HeartBeatGlodCopyScene();
				EMoLaiXiCopySceneManager.HeartBeatEMoLaiXiCopyScene();
				GameManager.BloodCastleCopySceneMgr.HeartBeatBloodCastScene();
				HuanYingSiYuanManager.getInstance().TimerProc();
				TianTiManager.getInstance().TimerProc();
				GlobalServiceManager.TimerProc();
				KingOfBattleManager.getInstance().TimerProc();
				YongZheZhanChangManager.getInstance().TimerProc();
				KuaFuBossManager.getInstance().TimerProc();
				SingletonTemplate<MoRiJudgeManager>.Instance().TimerProc();
				BangHuiMatchManager.getInstance().TimerProc();
				KuaFuLueDuoManager.getInstance().TimerProc();
				CompManager.getInstance().TimerProc_fuBenWorker();
				CompBattleManager.getInstance().TimerProc();
				CompMineManager.getInstance().TimerProc();
				ZorkBattleManager.getInstance().TimerProc();
				ElementWarManager.getInstance().TimerProc();
				CopyWolfManager.getInstance().TimerProc();
				SingletonTemplate<CoupleArenaManager>.Instance().UpdateCopyScene();
				GameManager.DaimonSquareCopySceneMgr.HeartBeatDaimonSquareScene();
				BroadcastInfoMgr.ProcessBroadcastInfos();
				PopupWinMgr.ProcessPopupWins();
				LingDiCaiJiManager.getInstance().TimerProc();
				RebornBoss.getInstance().TimerProc_fuBenWorker();
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("fuBenWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "fuBenWorker_DoWork", false, false);
			}
		}

		private void dbWriterWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				if (num - this.LastWriteDBLogTicks >= 30000L)
				{
					this.LastWriteDBLogTicks = num;
					Global._TCPManager.MySocketListener.ClearTimeoutSocket();
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "dbWriterWorker_DoWork", false, false);
			}
		}

		private void SocketSendCacheDataWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				Global._SendBufferManager.TrySendAll();
				long num2 = TimeUtil.NOW();
				if (num2 > num + 1000L)
				{
					this.DoLog(string.Format("SocketSendCacheDataWorker_DoWork 消耗:{0}毫秒", num2 - num));
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "SocketFlushBuffer_DoWork", false, false);
			}
		}

		private void CmdPacketProcessWorker_DoWork(object sender, EventArgs e)
		{
			Queue<CmdPacket> ls = new Queue<CmdPacket>();
			while (!Program.NeedExitServer)
			{
				try
				{
					Global._TCPManager.ProcessCmdPackets(ls);
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "CmdPacketProcessWorker_DoWork", false, false);
				}
				Thread.Sleep(5);
			}
		}

		private void Gird9UpdateWorker_DoWork(object sender, EventArgs e)
		{
			DoWorkEventArgs doWorkEventArgs = e as DoWorkEventArgs;
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			int num3 = 300;
			int num4 = 0;
			while (!Program.NeedExitServer)
			{
				try
				{
					num = TimeUtil.NOW();
					long num5 = num;
					GameManager.ClientMgr.DoSpritesMapGridMove((int)doWorkEventArgs.Argument);
					long num6 = TimeUtil.NOW();
					if (num6 > num5 + 1000L)
					{
						this.DoLog(string.Format("DoSpritesMapGridMove, 序号:{0} 消耗:{1}毫秒", (int)doWorkEventArgs.Argument, num6 - num5));
					}
					num2 = TimeUtil.NOW();
					int num7 = (int)Math.Max(5L, (long)num3 - (num2 - num));
					if (num7 > num3)
					{
						num7 = num3;
						LogManager.WriteLog(10, string.Format("TimeMismatch#Gird9UpdateWorker_DoWork,startTicks={0},endTicks={1}", num, num2), null, true);
					}
					Thread.Sleep(num7);
					num4++;
					if (num4 >= 100000)
					{
						num4 = 0;
					}
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "Gird9UpdateWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine(string.Format("9宫格更新驱动线程{0}退出...", (int)doWorkEventArgs.Argument));
		}

		private void RoleExtensionWorker_DoWork(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			long num2 = TimeUtil.NOW();
			int num3 = 100;
			int num4 = 0;
			while (!Program.NeedExitServer)
			{
				try
				{
					num = TimeUtil.NOW();
					long num5 = num;
					GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 0, 0);
					long num6 = TimeUtil.NOW();
					if (num6 > num5 + 1000L)
					{
						this.DoLog(string.Format("RoleExtensionWorker_DoWork, 消耗:{0}毫秒", num6 - num5));
					}
					num2 = TimeUtil.NOW();
					int millisecondsTimeout = (int)Math.Max(5L, (long)num3 - (num2 - num));
					Thread.Sleep(millisecondsTimeout);
					num4++;
					if (num4 >= 100000)
					{
						num4 = 0;
					}
				}
				catch (Exception e2)
				{
					DataHelper.WriteFormatExceptionLog(e2, "RoleExtensionWorker_DoWork", false, false);
				}
			}
			SysConOut.WriteLine("角色拓展线程退出");
		}

		private void SocketCheckWorker_DoWork(object sender, EventArgs e)
		{
			try
			{
				long num = TimeUtil.NOW();
				if (num - this.LastSocketCheckTicks >= 300000L)
				{
					this.LastSocketCheckTicks = num;
					int num2 = 900000;
					List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
					foreach (TMSKSocket tmsksocket in socketList)
					{
						long num3 = TimeUtil.NOW();
						long num4 = num3 - tmsksocket.session.SocketTime[0];
						if (tmsksocket.session.SocketState < 4 && num4 > (long)num2)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
							if (null == gameClient)
							{
								Global.ForceCloseSocket(tmsksocket, "被GM踢了, 但是这个socket上没有对应的client", true);
							}
						}
					}
				}
			}
			catch (Exception e2)
			{
				DataHelper.WriteFormatExceptionLog(e2, "SocketCheckWorker_DoWork", false, false);
			}
		}

		private void Window_Closing()
		{
			if (!this.MustCloseNow)
			{
				if (!this.EnterClosingMode)
				{
					this.EnterClosingMode = true;
					Global._TCPManager.MySocketListener.DontAccept = true;
					this.LastWriteDBLogTicks = 0L;
					Program.NeedExitServer = true;
				}
			}
		}

		public static string GetVersionDateTime()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			int revision = executingAssembly.GetName().Version.Revision;
			int build = executingAssembly.GetName().Version.Build;
			DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
			TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
			TimeSpan timeSpan2 = new TimeSpan(timeSpan.Days + build, 0, 0, revision * 2);
			DateTime dateTime2 = new DateTime(timeSpan2.Ticks);
			string version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(executingAssembly, typeof(AssemblyFileVersionAttribute))).Version;
			return dateTime2.ToString("yyyy-MM-dd_HH") + string.Format("_{0}", version);
		}

		public static string GetVersionStr()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			return ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(executingAssembly, typeof(AssemblyFileVersionAttribute))).Version;
		}

		public static FileVersionInfo AssemblyFileVersion;

		private static Program.ControlCtrlDelegate newDelegate = new Program.ControlCtrlDelegate(Program.HandlerRoutine);

		public static Program ServerConsole = new Program();

		private static Dictionary<string, Program.CmdCallback> CmdDict = new Dictionary<string, Program.CmdCallback>();

		public static bool NeedExitServer = false;

		private static string DumpBaseDir = "d:\\dumps\\";

		private static bool bDumpAndExit_ServerRunOk = false;

		public static int[] GCCollectionCounts = new int[3];

		public static int[] GCCollectionCounts1 = new int[3];

		public static int[] GCCollectionCounts5 = new int[3];

		public static int[] GCCollectionCountsNow = new int[3];

		public static int[] MaxGCCollectionCounts1s = new int[3];

		public static int[] MaxGCCollectionCounts5s = new int[3];

		public static long[] MaxGCCollectionCounts1sTicks = new long[3];

		public static long[] MaxGCCollectionCounts5sTicks = new long[3];

		public Dictionary<int, string> DBServerConnectDict = new Dictionary<int, string>();

		public Dictionary<int, string> LogDBServerConnectDict = new Dictionary<int, string>();

		private static string ProgramExtName = "";

		private static Timer ThreadPoolDriverTimer = null;

		private static Timer LogThreadPoolDriverTimer = null;

		private BackgroundWorker eventWorker;

		private BackgroundWorker dbCommandWorker;

		private BackgroundWorker logDBCommandWorker;

		private BackgroundWorker clientsWorker;

		private BackgroundWorker buffersWorker;

		private BackgroundWorker spriteDBWorker;

		private BackgroundWorker othersWorker;

		private BackgroundWorker FightingWorker;

		private BackgroundWorker chatMsgWorker;

		private BackgroundWorker fuBenWorker;

		private BackgroundWorker dbWriterWorker;

		private BackgroundWorker SocketSendCacheDataWorker;

		private BackgroundWorker ShengXiaoGuessWorker;

		private BackgroundWorker MainDispatcherWorker;

		private BackgroundWorker socketCheckWorker;

		private BackgroundWorker dynamicMonstersWorker;

		private BackgroundWorker TwLogWorker;

		private BackgroundWorker BanWorker;

		private BackgroundWorker IPStatisticsWorker;

		private ScheduleExecutor monsterExecutor = null;

		private int MaxMonsterProcessWorkersNum = 5;

		private BackgroundWorker[] Gird9UpdateWorkers;

		public static int MaxGird9UpdateWorkersNum = 5;

		private BackgroundWorker RoleStroyboardDispatcherWorker;

		private bool MustCloseNow = false;

		private bool EnterClosingMode = false;

		private int ClosingCounter = 6000;

		private long LastWriteDBLogTicks = TimeUtil.NOW();

		private long LastAuxiliaryTicks = TimeUtil.NOW();

		private long LastDynamicMonsterTicks = TimeUtil.NOW();

		private long LastMonsterUniqueIdProcTicks = TimeUtil.NOW();

		private long LastSocketCheckTicks = TimeUtil.NOW();

		public delegate bool ControlCtrlDelegate(int CtrlType);

		public delegate void CmdCallback(string cmd);

		private delegate string PatchDelegate(string[] args);
	}
}
