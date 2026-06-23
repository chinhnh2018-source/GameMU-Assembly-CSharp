using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.CheatGuard
{
	internal class SpeedUpTickCheck : SingletonTemplate<SpeedUpTickCheck>
	{
		private SpeedUpTickCheck()
		{
		}

		private void ForceRemove(int roleId)
		{
			lock (this.Mutex)
			{
				this.checkRoleDict.Remove(roleId);
				this.roleLastLog1Ticks.Remove(roleId);
				this.roleLastLog2Ticks.Remove(roleId);
			}
		}

		public void LoadConfig()
		{
			try
			{
				string[] array = GameManager.systemParamsList.GetParamValueByName("SpeedUpTickCheck").Split(new char[]
				{
					','
				});
				this.TotalElapsedTimes = Convert.ToInt32(array[0]);
				this.TotalElapsedDiffRate = Convert.ToDouble(array[1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, ex.Message.ToString(), null, true);
				this.TotalElapsedTimes = 10;
				this.TotalElapsedDiffRate = 0.2;
			}
		}

		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		public void OnLogout(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		public void OnClientHeart(GameClient client, long reportRealClientTick)
		{
			if (client != null)
			{
				if (!client.ClientSocket.session.IsGM)
				{
					lock (this.Mutex)
					{
						SpeedUpTickCheck.CheckRoleItem checkRoleItem = null;
						if (!this.checkRoleDict.TryGetValue(client.ClientData.RoleID, out checkRoleItem))
						{
							checkRoleItem = new SpeedUpTickCheck.CheckRoleItem();
							checkRoleItem.RoleId = client.ClientData.RoleID;
							checkRoleItem.RoleName = client.ClientData.RoleName;
							checkRoleItem.UserId = client.strUserID;
							checkRoleItem.IpAndPort = RobotTaskValidator.getInstance().GetIp(client);
							checkRoleItem.LastReportClientTick = reportRealClientTick;
							checkRoleItem.LastReceiveServerMs = TimeUtil.timeGetTime();
							checkRoleItem.MaybeTroubleTimes = 0;
							checkRoleItem.MaybeTroubleDiffRates.Clear();
							checkRoleItem.CliTotalElapsedTicks = 0L;
							checkRoleItem.SrvTotalElapsedTicks = 0L;
							checkRoleItem.TotalElapsedTimes = 0;
							this.checkRoleDict[client.ClientData.RoleID] = checkRoleItem;
						}
						else
						{
							long lastReportClientTick = checkRoleItem.LastReportClientTick;
							uint lastReceiveServerMs = checkRoleItem.LastReceiveServerMs;
							checkRoleItem.LastReportClientTick = reportRealClientTick;
							checkRoleItem.LastReceiveServerMs = TimeUtil.timeGetTime();
							uint num = checkRoleItem.LastReceiveServerMs - lastReceiveServerMs;
							long num2 = (long)((ulong)num * 10000UL);
							long num3 = checkRoleItem.LastReportClientTick - lastReportClientTick;
							if (num2 > 0L)
							{
								double num4 = (double)Math.Abs(num2 - num3) * 1.0 / (double)num2;
								if (num4 > this.currUseDiffRate)
								{
									checkRoleItem.MaybeTroubleTimes++;
									checkRoleItem.MaybeTroubleDiffRates.Add(num4);
									if (checkRoleItem.MaybeTroubleTimes >= 5)
									{
										long num5 = 0L;
										if (!this.roleLastLog1Ticks.TryGetValue(checkRoleItem.RoleId, out num5) || TimeUtil.NowDateTime().Ticks - num5 >= 7200000000L)
										{
											this.roleLastLog1Ticks[checkRoleItem.RoleId] = TimeUtil.NowDateTime().Ticks;
											LogManager.WriteLog(1000, string.Format("Check1 uid={0},rid={1},rname={2},ip={3} 疑似使用加速, 心跳时间差比例={4}", new object[]
											{
												checkRoleItem.UserId,
												checkRoleItem.RoleId,
												checkRoleItem.RoleName,
												checkRoleItem.IpAndPort,
												string.Join<double>(",", checkRoleItem.MaybeTroubleDiffRates)
											}), null, false);
										}
										checkRoleItem.MaybeTroubleTimes = 0;
										checkRoleItem.MaybeTroubleDiffRates.Clear();
									}
								}
								else if (Global.GetRandom() > 0.6)
								{
									this.totalDiffRate += num4;
									this.totalDiffCnt += 1.0;
									if (this.totalDiffCnt >= 100.0)
									{
										double num6 = this.currUseDiffRate;
										this.currUseDiffRate = this.totalDiffRate / this.totalDiffCnt;
										this.totalDiffCnt = 0.0;
										LogManager.WriteLog(2, string.Format("加速时间允许时间差范围变更 {0} ---> {1}", num6, this.currUseDiffRate), null, true);
									}
								}
								checkRoleItem.CliTotalElapsedTicks += num3;
								checkRoleItem.SrvTotalElapsedTicks += num2;
								checkRoleItem.TotalElapsedTimes++;
								if (checkRoleItem.TotalElapsedTimes >= this.TotalElapsedTimes)
								{
									double num7 = (double)Math.Abs(checkRoleItem.SrvTotalElapsedTicks - checkRoleItem.CliTotalElapsedTicks) * 1.0 / (double)checkRoleItem.SrvTotalElapsedTicks;
									if (num7 > this.TotalElapsedDiffRate)
									{
										long num8 = 0L;
										if (!this.roleLastLog2Ticks.TryGetValue(checkRoleItem.RoleId, out num8) || TimeUtil.NowDateTime().Ticks - num8 >= 7200000000L)
										{
											this.roleLastLog2Ticks[checkRoleItem.RoleId] = TimeUtil.NowDateTime().Ticks;
											LogManager.WriteLog(1000, string.Format("Check2 uid={0},rid={1},rname={2},ip={3} 疑似使用加速, CliTotalElapsedTicks={4}, SrvTotalElapsedTicks={5}, diffRate={6}", new object[]
											{
												checkRoleItem.UserId,
												checkRoleItem.RoleId,
												checkRoleItem.RoleName,
												checkRoleItem.IpAndPort,
												checkRoleItem.CliTotalElapsedTicks,
												checkRoleItem.SrvTotalElapsedTicks,
												num7
											}), null, false);
										}
									}
									checkRoleItem.CliTotalElapsedTicks = 0L;
									checkRoleItem.SrvTotalElapsedTicks = 0L;
									checkRoleItem.TotalElapsedTimes = 0;
								}
							}
						}
					}
				}
			}
		}

		private object Mutex = new object();

		private Dictionary<int, SpeedUpTickCheck.CheckRoleItem> checkRoleDict = new Dictionary<int, SpeedUpTickCheck.CheckRoleItem>();

		private Dictionary<int, long> roleLastLog1Ticks = new Dictionary<int, long>();

		private Dictionary<int, long> roleLastLog2Ticks = new Dictionary<int, long>();

		private double totalDiffRate = 0.0;

		private double totalDiffCnt = 0.0;

		private double currUseDiffRate = 1.0;

		private int TotalElapsedTimes = 10;

		private double TotalElapsedDiffRate = 0.2;

		private class CheckRoleItem
		{
			public string UserId;

			public int RoleId;

			public string RoleName;

			public string IpAndPort;

			public long LastReportClientTick;

			public uint LastReceiveServerMs;

			public int MaybeTroubleTimes;

			public List<double> MaybeTroubleDiffRates = new List<double>();

			public long CliTotalElapsedTicks;

			public long SrvTotalElapsedTicks;

			public int TotalElapsedTimes;
		}
	}
}
