using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;
using Tmsk.Tools;

namespace GameServer.Logic
{
	public class ServerMonitorManager
	{
		public ServerMonitorManager()
		{
			this._PrevCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
			this._LastCalcCpuMs = TimeUtil.NOW();
			this._LastReportMs = TimeUtil.NOW();
		}

		private bool GetCpuAndMem(out double cpuLoad, out double memMb)
		{
			cpuLoad = 0.0;
			memMb = 0.0;
			try
			{
				long num = TimeUtil.NOW();
				double num2 = (double)(num - this._LastCalcCpuMs);
				TimeSpan totalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
				if (num2 > 0.0)
				{
					cpuLoad = (totalProcessorTime - this._PrevCpuTime).TotalMilliseconds * 1.0 / num2 / (double)Environment.ProcessorCount;
					cpuLoad = Math.Min(cpuLoad, 1.0);
				}
				memMb = (double)Process.GetCurrentProcess().WorkingSet64 / 1048576.0;
				this._LastCalcCpuMs = num;
				this._PrevCpuTime = totalProcessorTime;
			}
			catch (Exception)
			{
				cpuLoad = 0.0;
				memMb = 0.0;
				return false;
			}
			return true;
		}

		private void RefreshReportConfig()
		{
			if (this._BNeedReLoad)
			{
				this._BNeedReLoad = false;
				string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("server_monitor_report", "");
				string[] array = gameConfigItemStr.Split(new char[]
				{
					','
				});
				string reportToUrl = string.Empty;
				int num = 3;
				if (array.Length >= 1)
				{
					reportToUrl = array[0];
				}
				if (array.Length >= 2)
				{
					if (!int.TryParse(array[1], out num))
					{
						num = 5;
					}
				}
				num = Math.Max(3, num);
				this._ReportToUrl = reportToUrl;
				this._ReportIntervalSec = num;
			}
		}

		public void SetNeedReload()
		{
			this._BNeedReLoad = true;
		}

		public void CheckReport()
		{
			try
			{
				this.RefreshReportConfig();
				if (!string.IsNullOrEmpty(this._ReportToUrl))
				{
					long num = TimeUtil.NOW();
					if (num - this._LastReportMs >= (long)(this._ReportIntervalSec * 1000))
					{
						if (!this._BIsReporting)
						{
							this._BIsReporting = true;
							this._LastReportMs = num;
							StringBuilder sb = new StringBuilder();
							sb.Append(this._ReportToUrl).Append("?");
							sb.AppendFormat("serverid={0}&", GameCoreInterface.getinstance().GetLocalServerId());
							sb.AppendFormat("platform={0}&", GameCoreInterface.getinstance().GetPlatformType().ToString());
							double num2;
							double num3;
							this.GetCpuAndMem(out num2, out num3);
							sb.AppendFormat("cpu={0}&", num2);
							sb.AppendFormat("mem={0}&", num3);
							sb.AppendFormat("roleCount={0}&", GameManager.ClientMgr.GetClientCount());
							sb.AppendFormat("procCmdCount={0}&", TCPCmdHandler.TotalHandledCmdsNum);
							sb.AppendFormat("cmdAvgProcMs={0}&", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0);
							sb.AppendFormat("cmdMaxProcMs={0}&", TCPCmdHandler.MaxUsedTicksByCmdID);
							sb.AppendFormat("dbConnCount={0}&", Global._TCPManager.tcpClientPool.GetPoolCount());
							sb.AppendFormat("lastFlushMonsterToNow={0}", GameManager.LastFlushMonsterMs * 10000L);
							new Task(delegate()
							{
								WebHelper.RequestByGet(sb.ToString(), 2000, 30000);
								this._BIsReporting = false;
							}).Start();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "ServerAnalysisManager.CheckReport() failed!", ex, true);
			}
		}

		public const string ReportUrlCfgKey = "server_monitor_report";

		private TimeSpan _PrevCpuTime = TimeSpan.Zero;

		private long _LastCalcCpuMs;

		private long _LastReportMs;

		private bool _BIsReporting = false;

		private bool _BNeedReLoad = true;

		private string _ReportToUrl = string.Empty;

		private int _ReportIntervalSec = 5;
	}
}
