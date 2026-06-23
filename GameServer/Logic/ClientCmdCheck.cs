using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public static class ClientCmdCheck
	{
		public static long GetClientTicks(GameClient client, long ccTicks = 0L)
		{
			long num = (long)client.ClientSocket.ClientCmdSecs * 1000L + TimeUtil.Before1970Ticks;
			long result;
			if (Math.Abs(ccTicks - num) >= 1000L)
			{
				result = TimeUtil.NowTickCount64() - client.ClientData.ClientExtData.ServerClientTimeDiffTicks;
			}
			else
			{
				result = ccTicks;
			}
			return result;
		}

		public static bool ClientCheck(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			if (Data.CheckTimeBoost && clientExtData.KeepAlive)
			{
				long num = TimeUtil.NowTickCount64();
				long num2 = (long)client.ClientSocket.ClientCmdSecs * 1000L + TimeUtil.Before1970Ticks;
				long num3 = num - num2;
				lock (clientExtData)
				{
					if (num3 < clientExtData.MinTimeDiff)
					{
						if (clientExtData.ClientLoginClientTicks == 0L)
						{
							clientExtData.ServerLoginTickCount = num;
							clientExtData.ClientLoginClientTicks = num2;
							clientExtData.MinTimeDiff = num3 - 18000L;
						}
						else if (num < clientExtData.ServerLoginTickCount + 20000L)
						{
							clientExtData.MinTimeDiff = num3 - 18000L;
						}
						else if (num3 < clientExtData.MinTimeDiff)
						{
							clientExtData.KeepAlive = false;
							LogManager.WriteLog(1001, string.Format("客户端时钟偏差过大,可能是加速或调时钟#rid={0},login={1},MinTimeDiff={2},diff={3}", new object[]
							{
								client.ClientData.RoleID,
								clientExtData.ServerLoginTickCount,
								clientExtData.MinTimeDiff,
								num3
							}), null, true);
						}
					}
					else
					{
						lock (clientExtData)
						{
							clientExtData.ServerClientTimeDiffTicks += (num3 - clientExtData.ServerClientTimeDiffTicks) / (clientExtData.CalcNum += 1L);
						}
					}
				}
			}
			return clientExtData.KeepAlive;
		}

		public static void RecordClientPosition(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				clientExtData.EndMoveTicks = 0L;
				clientExtData.RunStoryboard = false;
				clientExtData.FromX = client.ClientData.PosX;
				clientExtData.FromY = client.ClientData.PosY;
				clientExtData.ToX = client.ClientData.PosX;
				clientExtData.ToY = client.ClientData.PosY;
				clientExtData.MapCode = client.ClientData.MapCode;
				clientExtData.ReservedTicks = 0L;
			}
		}

		public static void StopClientStoryboard(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				clientExtData.RunStoryboard = false;
			}
		}

		public static void ResetClientPosition(GameClient client, int posX, int posY)
		{
			GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, posX, posY, client.ClientData.RoleDirection, 159, 0);
		}

		public static void ClientAction(GameClient client, long nowTicks, long reserveTicks)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				if (client.ClientData.MapCode == clientExtData.MapCode)
				{
					clientExtData.CanMoveTicks = ClientCmdCheck.GetClientTicks(client, 0L) + reserveTicks;
				}
			}
		}

		public static void ClientStopMove(GameClient client, int x, int y, long startMoveTicks = 0L)
		{
			if (Data.CheckPositionCheat)
			{
				startMoveTicks = ClientCmdCheck.GetClientTicks(client, startMoveTicks);
				bool flag = ClientCmdCheck.MoveTo(client, x, y, startMoveTicks, true);
				if (flag)
				{
					ClientCmdCheck.ResetClientPosition(client, client.ClientData.PosX, client.ClientData.PosY);
				}
			}
		}

		public static void MoveSpeedChange(GameClient client, double newMoveSpeed)
		{
			if (Data.CheckPositionCheatSpeed)
			{
				long clientTicks = ClientCmdCheck.GetClientTicks(client, 0L);
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode == clientExtData.MapCode)
					{
						if (!clientExtData.RunStoryboard)
						{
							if (clientExtData.EndMoveTicks > clientExtData.StartMoveTicks)
							{
								long num = clientTicks - clientExtData.StartMoveTicks;
								double num2 = (double)num / (double)(clientExtData.EndMoveTicks - clientExtData.StartMoveTicks);
								if (num2 < ClientCmdCheck.MinDistanceFactor)
								{
									int num3 = (int)((double)(clientExtData.ToX - clientExtData.FromX) * (1.0 - num2));
									int num4 = (int)((double)(clientExtData.ToY - clientExtData.FromY) * (1.0 - num2));
									if (Math.Abs(num3) + Math.Abs(num4) > 50)
									{
										clientExtData.FromX = (client.ClientData.PosX = clientExtData.ToX - num3);
										clientExtData.FromY = (client.ClientData.PosY = clientExtData.ToY - num4);
										num3 = clientExtData.ToX - clientExtData.FromX;
										num4 = clientExtData.ToY - clientExtData.FromY;
										clientExtData.MaxDistance2 = num3 * num3 + num4 * num4;
										clientExtData.StartMoveTicks = clientTicks;
										clientExtData.ReservedTicks = 0L;
										if (newMoveSpeed >= 0.05)
										{
											clientExtData.MoveSpeed = newMoveSpeed;
											clientExtData.EndMoveTicks = clientTicks + (long)(Math.Pow((double)clientExtData.MaxDistance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed));
										}
										else
										{
											clientExtData.StartMoveTicks = clientTicks;
											clientExtData.FromX = client.ClientData.PosX;
											clientExtData.FromY = client.ClientData.PosY;
										}
									}
									else
									{
										clientExtData.StartMoveTicks = clientTicks;
										clientExtData.ToX = client.ClientData.PosX;
										clientExtData.ToY = client.ClientData.PosY;
									}
								}
								else
								{
									clientExtData.StartMoveTicks = clientTicks;
									clientExtData.ToX = client.ClientData.PosX;
									clientExtData.ToY = client.ClientData.PosY;
								}
							}
						}
					}
				}
			}
		}

		public static bool ClientPosition(GameClient client, int x, int y, long startMoveTicks = 0L)
		{
			bool result;
			if (!Data.CheckPositionCheat)
			{
				result = true;
			}
			else
			{
				bool flag = false;
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode != clientExtData.MapCode)
					{
						return false;
					}
					int num = x - clientExtData.ToX;
					int num2 = y - clientExtData.ToY;
					if (num != 0 || num2 != 0)
					{
						if (!clientExtData.RunStoryboard)
						{
							if (clientExtData.StartMoveTicks < clientExtData.EndMoveTicks)
							{
								if (startMoveTicks >= clientExtData.EndMoveTicks)
								{
									clientExtData.StartMoveTicks = startMoveTicks;
									client.ClientData.PosX = clientExtData.ToX;
									client.ClientData.PosY = clientExtData.ToY;
								}
							}
							else if (Math.Abs(num) + Math.Abs(num2) >= 500)
							{
								LogManager.WriteLog(1001, string.Format("ClientPosition位置不匹配#rid={0}", client.ClientData.RoleID), null, true);
								client.ClientData.PosX = clientExtData.ToX;
								client.ClientData.PosY = clientExtData.ToY;
								flag = true;
								clientExtData.StartMoveTicks = startMoveTicks;
							}
						}
					}
				}
				if (flag)
				{
					ClientCmdCheck.ResetClientPosition(client, client.ClientData.PosX, client.ClientData.PosY);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private static bool MoveTo(GameClient client, int x, int y, long startMoveTicks, bool stop)
		{
			bool flag = false;
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				if (client.ClientData.MapCode != clientExtData.MapCode)
				{
					LogManager.WriteLog(1001, string.Format("MoveTo地图不匹配#rid={2},current={0},last={1}", client.ClientData.MapCode, clientExtData.MapCode, client.ClientData.RoleID), null, true);
					return false;
				}
				if (startMoveTicks < clientExtData.CanMoveTicks)
				{
					LogManager.WriteLog(1001, string.Format("MoveTo未到可移动时间rid={1},#time={0}", clientExtData.CanMoveTicks - startMoveTicks, client.ClientData.RoleID), null, true);
					return false;
				}
				int num = clientExtData.ToX - clientExtData.FromX;
				int num2 = clientExtData.ToY - clientExtData.FromY;
				if (num != 0 || num2 != 0)
				{
					int num3 = x - clientExtData.FromX;
					int num4 = y - clientExtData.FromY;
					long num5 = (long)(num3 * num3 + num4 * num4);
					if (num5 > 0L)
					{
						long num6;
						if (startMoveTicks < clientExtData.StopMoveTicks)
						{
							num6 = startMoveTicks - clientExtData.StartMoveTicks - clientExtData.ReservedTicks;
						}
						else
						{
							num6 = clientExtData.StopMoveTicks - clientExtData.StartMoveTicks - clientExtData.ReservedTicks;
						}
						if (!clientExtData.RunStoryboard && client.InSafeRegion)
						{
							clientExtData.ReservedTicks += (long)(Math.Pow((double)num5, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed * 0.8) - (double)num6);
						}
						else
						{
							clientExtData.ReservedTicks += (long)(Math.Pow((double)num5, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed) - (double)num6);
						}
						if (clientExtData.ReservedTicks > ClientCmdCheck.MaxReserveMs)
						{
							flag = true;
							LogManager.WriteLog(1001, string.Format("MoveTo时间校验超限#rid={1},ticks={0}", clientExtData.ReservedTicks, client.ClientData.RoleID), null, true);
						}
						else if (clientExtData.ReservedTicks < -100L)
						{
							clientExtData.ReservedTicks = -100L;
						}
					}
				}
				else
				{
					int num3 = x - clientExtData.FromX;
					int num4 = y - clientExtData.FromY;
					long num5 = (long)(num3 * num3 + num4 * num4);
					if (num5 > 5000L)
					{
						x = clientExtData.FromX;
						y = clientExtData.FromY;
						flag = true;
					}
				}
				if (flag)
				{
					clientExtData.ToX = x;
					clientExtData.ToY = y;
					clientExtData.MaxDistance2 = (x - clientExtData.FromX) * (x - clientExtData.FromX) + (y - clientExtData.FromY) * (y - clientExtData.FromY);
				}
				else if (!clientExtData.RunStoryboard)
				{
					client.ClientData.PosX = x;
					client.ClientData.PosY = y;
				}
				if (stop)
				{
					clientExtData.StopMoveTicks = startMoveTicks;
				}
				else
				{
					clientExtData.StopMoveTicks = long.MaxValue;
				}
			}
			return flag;
		}

		public static void SpritePreMove(GameClient client, int fromX, int fromY, int toX, int toY, long startMoveTicks)
		{
			if (!Data.IgnoreClientPos)
			{
				client.ClientData.PosX = fromX;
				client.ClientData.PosY = fromY;
				client.ClientData.ReportPosTicks = startMoveTicks;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
				mapGrid.MoveObject(-1, -1, fromX, fromY, client);
			}
			client.ClientData.DestPoint = new Point((double)toX, (double)toY);
		}

		public static bool ValidateClientMoveStartTicks(GameClient client, long startMoveTicks = 0L)
		{
			double paramValueDoubleByName = GameManager.systemParamsList.GetParamValueDoubleByName("CHEAT_STARTMOVE", 0.0);
			bool result;
			if (paramValueDoubleByName <= 0.0)
			{
				result = true;
			}
			else
			{
				int num = (int)GameManager.systemParamsList.GetParamValueIntByName("CHECK_STARTMOVE_COUNT", 0);
				if (num <= 0)
				{
					result = true;
				}
				else
				{
					long num2 = (long)((ulong)TimeUtil.timeGetTime());
					if (num2 - client.CheckCheatData.LastStartMoveServerTicks < 1000L)
					{
						result = true;
					}
					else if (client.CheckCheatData.LastStartMoveTicks <= 0L || client.CheckCheatData.LastStartMoveServerTicks <= 0L)
					{
						client.CheckCheatData.LastStartMoveTicks = startMoveTicks;
						client.CheckCheatData.LastStartMoveServerTicks = num2;
						result = true;
					}
					else
					{
						long num3 = startMoveTicks - client.CheckCheatData.LastStartMoveTicks;
						long num4 = num2 - client.CheckCheatData.LastStartMoveServerTicks;
						if (num3 > 0L && num4 > 0L)
						{
							if ((double)num3 > (double)num4 + Math.Abs((double)num4 * paramValueDoubleByName))
							{
								client.CheckCheatData.LastMoveStartMoveTicksCheatNum += 1L;
							}
							else
							{
								client.CheckCheatData.LastMoveStartMoveTicksCheatNum = 0L;
							}
						}
						if (client.CheckCheatData.LastMoveStartMoveTicksCheatNum >= (long)num)
						{
							int posX = client.ClientData.PosX;
							int posY = client.ClientData.PosY;
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, posX, posY, client.ClientData.RoleDirection, 159, 0);
							client.ClientData.InstantMoveTick = TimeUtil.NOW() + 1000L;
							LogManager.WriteLog(2, string.Format("通过STARTMOVE指令判断客户端启用的本地进程加速: {0}, {1}, {2} {3}, 断开连接", new object[]
							{
								Global.GetSocketRemoteEndPoint(client.ClientSocket, false),
								client.ClientData.RoleID,
								num3,
								num4
							}), null, true);
							result = false;
						}
						else
						{
							client.CheckCheatData.LastStartMoveTicks = startMoveTicks;
							client.CheckCheatData.LastStartMoveServerTicks = num2;
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static bool SpriteMoveCmd(GameClient client, int fromX, int fromY, int toX, int toY, long startMoveTicks, double moveSpeed, List<Point> path, out bool stepMove)
		{
			stepMove = false;
			bool result;
			if (!Data.CheckPositionCheat)
			{
				ClientCmdCheck.SpritePreMove(client, fromX, fromY, toX, toY, startMoveTicks);
				result = true;
			}
			else
			{
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				bool flag2;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode != clientExtData.MapCode)
					{
						return false;
					}
					flag2 = ClientCmdCheck.MoveTo(client, fromX, fromY, startMoveTicks, false);
					if (!flag2)
					{
						ClientCmdCheck.SpritePreMove(client, fromX, fromY, toX, toY, startMoveTicks);
						clientExtData.RunStoryboard = false;
						if (moveSpeed < 0.05)
						{
							return false;
						}
						if (path.Count < 2)
						{
							LogManager.WriteLog(1001, string.Format("SpriteMoveCmd路径点不足两个#rid={0}", client.ClientData.RoleID), null, true);
							return false;
						}
						if (path.Count == 2)
						{
							if (Math.Abs(path[0].X - path[1].X) > 1.0 || Math.Abs(path[0].Y - path[1].Y) > 1.0)
							{
								LogManager.WriteLog(1001, string.Format("SpriteMoveCmd,2点路径非法#rid={0}", client.ClientData.RoleID), null, true);
								return false;
							}
							toX = (int)path[1].X * 100 + 50;
							toY = (int)path[1].Y * 100 + 50;
						}
						else if (path.Count == 3)
						{
							if (path[0].X == path[1].X && path[0].Y == path[1].Y)
							{
								if (Math.Abs(path[0].X - path[2].X) + Math.Abs(path[0].Y - path[2].Y) > 2.0)
								{
									clientExtData.RunStoryboard = true;
								}
							}
							else
							{
								if (path[0].X + path[2].X != path[1].X + path[1].X || path[0].Y + path[2].Y != path[1].Y + path[1].Y)
								{
									LogManager.WriteLog(1001, string.Format("SpriteMoveCmd,3点路径非法#rid={0}", client.ClientData.RoleID), null, true);
									return false;
								}
								if (Math.Abs(path[0].X - path[2].X) > 2.0 || Math.Abs(path[0].Y - path[2].Y) > 2.0)
								{
									LogManager.WriteLog(1001, string.Format("SpriteMoveCmd,3点距离非法#rid={0}", client.ClientData.RoleID), null, true);
									return false;
								}
							}
							toX = (int)path[2].X * 100 + 50;
							toY = (int)path[2].Y * 100 + 50;
						}
						else
						{
							clientExtData.RunStoryboard = true;
						}
						clientExtData.FromX = fromX;
						clientExtData.FromY = fromY;
						clientExtData.ToX = toX;
						clientExtData.ToY = toY;
						clientExtData.StartMoveTicks = startMoveTicks;
						clientExtData.MaxDistance2 = (toX - fromX) * (toX - fromX) + (toY - fromY) * (toY - fromY);
						clientExtData.MoveSpeed = moveSpeed;
						clientExtData.EndMoveTicks = startMoveTicks + (long)(Math.Pow((double)clientExtData.MaxDistance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed));
						stepMove = !clientExtData.RunStoryboard;
					}
				}
				if (flag2)
				{
					ClientCmdCheck.ResetClientPosition(client, clientExtData.FromX, clientExtData.FromY);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public static string GetLifeLogString(int mapCode, int current, int max, int add)
		{
			string result;
			try
			{
				if (current >= max)
				{
					result = null;
				}
				else
				{
					lock (ClientCmdCheck.MapCodes)
					{
						if ((long)(add * 100) < (long)max * ClientCmdCheck.MinLogAddLifePercent)
						{
							return null;
						}
						if (!ClientCmdCheck.MapCodes.Contains(mapCode))
						{
							return null;
						}
					}
					StackTrace stackTrace = new StackTrace(1, true);
					result = string.Format("mapCode={0},life={1},max={2},add={3}\r\n{4}", new object[]
					{
						mapCode,
						current,
						max,
						add,
						stackTrace.ToString()
					});
				}
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}
			return result;
		}

		public static void WriteZhanLiLogs(GameClient client)
		{
			try
			{
				if (client.ClientData.ChangeLifeCount >= 4)
				{
					bool flag = false;
					long num = TimeUtil.NOW();
					long num2 = (long)client.ClientData.CombatForce;
					int offsetDayNow = TimeUtil.GetOffsetDayNow();
					ExtData clientExtData = ExtDataManager.GetClientExtData(client);
					lock (clientExtData.ZhanLiLogged)
					{
						if (clientExtData.OffsetDay != offsetDayNow)
						{
							clientExtData.LastZhanLi = 0L;
							clientExtData.ZhanLiWriteten = 0L;
							clientExtData.OffsetDay = offsetDayNow;
							clientExtData.ZhanLiLogNextWriteTicks = num + (long)Global.GetRandomNumber(20000, 180000);
							clientExtData.ZhanLiLogged.Clear();
						}
						else if (clientExtData.ZhanLiWriteten < num2 && clientExtData.ZhanLiLogNextWriteTicks < num)
						{
							clientExtData.ZhanLiLogNextWriteTicks = num + 5000L;
							if (clientExtData.LastZhanLi != num2)
							{
								clientExtData.LastZhanLi = num2;
							}
							else if (!clientExtData.ZhanLiLogged.Contains((long)client.ClientData.CombatForce))
							{
								flag = true;
								clientExtData.ZhanLiWriteten = num2;
								clientExtData.ZhanLiLogged.Add((long)client.ClientData.CombatForce);
							}
						}
					}
					if (flag)
					{
						StringBuilder stringBuilder = new StringBuilder();
						Global.PrintSomeProps(client, ref stringBuilder);
						LogManager.WriteLog(10, stringBuilder.ToString(), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static void WriteLifeLogs(GameClient client)
		{
			try
			{
				ClientCmdCheck.WriteZhanLiLogs(client);
				List<string> list = null;
				lock (client.ClientData)
				{
					if (client.ClientData.AddLifeAlertList.Count == 0)
					{
						return;
					}
					list = new List<string>();
					while (client.ClientData.AddLifeAlertList.Count > 0)
					{
						list.Add(client.ClientData.AddLifeAlertList.Dequeue());
					}
				}
				foreach (string text in list)
				{
					LogManager.WriteLog(1002, string.Format("#AlertLog#AddLifeAlert#rid={0},rname={1},userid={2},{3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.strUserID,
						text
					}), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static long MaxCheckTicks = 3000L;

		public static double MoveSpeedPerMS = 0.5;

		public static long MaxReserveMs = 200L;

		public static double MaxDistanceFactor = 1.05;

		public static double MinDistanceFactor = 0.95;

		public static long MinLogAddLifeV = 1000L;

		public static long MinLogAddLifePercent = 15L;

		public static HashSet<int> MapCodes = new HashSet<int>();
	}
}
