using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class RechargeRepayActiveMgr
	{
		private static int GetBtnIndexState(int money, int minMoney, bool recode)
		{
			int result = 0;
			if (money >= minMoney && recode)
			{
				result = 2;
			}
			if (money >= minMoney && !recode)
			{
				result = 1;
			}
			return result;
		}

		private static string GetBtnIndexStateListStr(GameClient client, int money, ActivityTypes type, string[] records)
		{
			Activity activity = Global.GetActivity(type);
			string result;
			if (null == activity)
			{
				LogManager.WriteLog(2, string.Format("GetBtnIndexStateListStr Params Error: type={0}", type), null, true);
				result = "";
			}
			else
			{
				List<int> awardMinConditionlist = activity.GetAwardMinConditionlist();
				string text = "";
				for (int i = 0; i < awardMinConditionlist.Count; i++)
				{
					bool recode = false;
					if (i < records.Length)
					{
						recode = (records[i] == "2");
					}
					text += RechargeRepayActiveMgr.GetBtnIndexState(money, awardMinConditionlist[i], recode);
					if (i < awardMinConditionlist.Count - 1)
					{
						text += ",";
					}
				}
				result = text;
			}
			return result;
		}

		public static TCPProcessCmdResults QueryAllRechargeRepayActiveInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out array))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else if (array.Length != 1)
			{
				LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					int num2 = GameManager.ClientMgr.QueryTotaoChongZhiMoney(gameClient);
					num2 = Global.TransMoneyToYuanBao(num2);
					int num3 = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(gameClient);
					num3 = Global.TransMoneyToYuanBao(num3);
					string[] array2 = null;
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", num, 39), out array2, gameClient.ServerId);
					if (null == array2)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else if (array2.Length != 3)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else
					{
						int num4 = Global.SafeConvertToInt32(array2[2]);
						string data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num2,
							num3,
							num2,
							num4
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults QueryRechargeRepayActive(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out array))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else if (array.Length != 2)
			{
				LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Global.SafeConvertToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					string arg = "";
					string[] array2 = null;
					ActivityTypes activityTypes = (ActivityTypes)num2;
					if (activityTypes <= ActivityTypes.MeiRiChongZhiHaoLi)
					{
						if (activityTypes != ActivityTypes.InputFirst)
						{
							switch (activityTypes)
							{
							case ActivityTypes.HeFuLogin:
								arg = string.Format("{0}:0", Global.GetRoleParamsInt32FromDB(gameClient, "HeFuLoginFlag").ToString());
								break;
							case ActivityTypes.HeFuTotalLogin:
								arg = string.Format("{0}:{1}", Global.GetRoleParamsInt32FromDB(gameClient, "HeFuTotalLoginNum").ToString(), Global.GetRoleParamsInt32FromDB(gameClient, "HeFuTotalLoginFlag").ToString());
								break;
							case ActivityTypes.HeFuRecharge:
							{
								HeFuRechargeActivity heFuRechargeActivity = HuodongCachingMgr.GetHeFuRechargeActivity();
								if (null == heFuRechargeActivity)
								{
									arg = string.Format("{0}:{1}", "0", "0");
								}
								else if (!heFuRechargeActivity.InActivityTime() && !heFuRechargeActivity.InAwardTime())
								{
									arg = string.Format("{0}:{1}", "0", "0");
								}
								else
								{
									int offsetDay = Global.GetOffsetDay(Global.GetHefuStartDay());
									TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										array[0],
										array[1],
										offsetDay,
										Global.GetOffsetDay(DateTime.Parse(heFuRechargeActivity.ToDate)),
										heFuRechargeActivity.strcoe
									}), out array2, gameClient.ServerId);
									if (null == array2)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (array2 == null || 4 != array2.Length)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									arg = string.Format("{0}:{1}", array2[2], array2[3]);
								}
								break;
							}
							case ActivityTypes.HeFuPKKing:
								arg = string.Format("{0}:{1}", HuodongCachingMgr.GetHeFuPKKingRoleID(), Global.GetRoleParamsInt32FromDB(gameClient, "HeFuPKKingFlag").ToString());
								break;
							case ActivityTypes.MeiRiChongZhiHaoLi:
							{
								int money = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(gameClient);
								int num3 = Global.TransMoneyToYuanBao(money);
								TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", array[0], 27), out array2, gameClient.ServerId);
								if (null == array2)
								{
									return TCPProcessCmdResults.RESULT_FAILED;
								}
								if (array2.Length != 3)
								{
									return TCPProcessCmdResults.RESULT_FAILED;
								}
								string[] records = array2[1].Split(new char[]
								{
									','
								});
								arg = RechargeRepayActiveMgr.GetBtnIndexStateListStr(gameClient, num3, ActivityTypes.MeiRiChongZhiHaoLi, records);
								arg = arg + ":" + num3;
								break;
							}
							}
						}
						else
						{
							int num3 = GameManager.ClientMgr.QueryTotaoChongZhiMoney(gameClient);
							num3 = Global.TransMoneyToYuanBao(num3);
							arg = RechargeRepayActiveMgr.GetBtnIndexState(num3, 1, !Global.CanGetFirstChongZhiDaLiByUserID(gameClient)) + ":" + num3;
						}
					}
					else
					{
						switch (activityTypes)
						{
						case ActivityTypes.TotalCharge:
						{
							int num3 = GameManager.ClientMgr.QueryTotaoChongZhiMoney(gameClient);
							num3 = Global.TransMoneyToYuanBao(num3);
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", array[0], 38), out array2, gameClient.ServerId);
							if (null == array2)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (array2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] records = array2[1].Split(new char[]
							{
								','
							});
							arg = RechargeRepayActiveMgr.GetBtnIndexStateListStr(gameClient, num3, ActivityTypes.TotalCharge, records);
							arg = string.Format("{0}:{1}", arg, num3);
							break;
						}
						case ActivityTypes.TotalConsume:
						{
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", array[0], array[1]), out array2, gameClient.ServerId);
							if (null == array2)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (array2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int num4 = Global.SafeConvertToInt32(array2[2]);
							string[] records = array2[1].Split(new char[]
							{
								','
							});
							arg = RechargeRepayActiveMgr.GetBtnIndexStateListStr(gameClient, num4, ActivityTypes.TotalConsume, records);
							arg = string.Format("{0}:{1}", arg, num4);
							break;
						}
						default:
							if (activityTypes != ActivityTypes.HeFuLuoLan)
							{
								switch (activityTypes)
								{
								case ActivityTypes.OneDollarChongZhi:
								{
									OneDollarChongZhi oneDollarChongZhiActivity = HuodongCachingMgr.GetOneDollarChongZhiActivity();
									if (null == oneDollarChongZhiActivity)
									{
										arg = string.Format("{0}:{1}", "0", "0");
									}
									else if (!oneDollarChongZhiActivity.InActivityTime() && !oneDollarChongZhiActivity.InAwardTime())
									{
										arg = string.Format("{0}:{1}", "0", "0");
									}
									else
									{
										TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											array[0],
											46,
											oneDollarChongZhiActivity.FromDate.Replace(':', '$'),
											oneDollarChongZhiActivity.ToDate.Replace(':', '$')
										}), out array2, gameClient.ServerId);
										if (null == array2)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										if (array2.Length != 3)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										int num5 = Global.SafeConvertToInt32(array2[2]);
										string[] records = array2[1].Split(new char[]
										{
											','
										});
										arg = RechargeRepayActiveMgr.GetBtnIndexStateListStr(gameClient, num5, ActivityTypes.OneDollarChongZhi, records);
										arg = string.Format("{0}:{1}", arg, num5);
									}
									break;
								}
								case ActivityTypes.InputFanLiNew:
								{
									InputFanLiNew inputFanLiNewActivity = HuodongCachingMgr.GetInputFanLiNewActivity();
									if (inputFanLiNewActivity == null || !inputFanLiNewActivity.InActivityTime())
									{
										arg = string.Format("{0}:{1},{2}", "0", "0", "0");
									}
									else
									{
										TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", array[0], array[1]), out array2, gameClient.ServerId);
										if (null == array2)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										if (array2 == null || 3 != array2.Length)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										arg = string.Format("{0}:{1}", array2[1], array2[2]);
									}
									break;
								}
								}
							}
							else
							{
								string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
								arg = string.Format("{0}:{1}", gameConfigItemStr, Global.GetRoleParamsInt32FromDB(gameClient, "HeFuLuoLanAwardFlag").ToString());
							}
							break;
						}
					}
					string data2 = string.Format("{0}:{1}", arg, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}

		public static bool CheckRechargeReplay(GameClient client, ActivityTypes type, out bool hasGet)
		{
			hasGet = false;
			try
			{
				string strcmd;
				if (type == ActivityTypes.OneDollarChongZhi)
				{
					OneDollarChongZhi oneDollarChongZhiActivity = HuodongCachingMgr.GetOneDollarChongZhiActivity();
					if (oneDollarChongZhiActivity == null || !oneDollarChongZhiActivity.InActivityTime() || !oneDollarChongZhiActivity.CanGiveAward(client, 1, 0))
					{
						return false;
					}
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.RoleID,
						46,
						oneDollarChongZhiActivity.FromDate.Replace(':', '$'),
						oneDollarChongZhiActivity.ToDate.Replace(':', '$')
					});
				}
				else
				{
					strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, (int)type);
				}
				string[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10160, strcmd, out array, client.ServerId);
				if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_DATA || array == null)
				{
					return false;
				}
				if (type == ActivityTypes.InputFanLiNew)
				{
					InputFanLiNew inputFanLiNewActivity = HuodongCachingMgr.GetInputFanLiNewActivity();
					if (inputFanLiNewActivity == null || !inputFanLiNewActivity.InActivityTime())
					{
						return false;
					}
					int num = Global.SafeConvertToInt32(array[1]);
					string[] array2 = array[2].Split(new char[]
					{
						','
					});
					if (array2.Length != 2)
					{
						return false;
					}
					int chargeMoney = Global.SafeConvertToInt32(array2[0]);
					int consumeMoney = Global.SafeConvertToInt32(array2[1]);
					int awardIndex = inputFanLiNewActivity.GetAwardIndex(client, chargeMoney, consumeMoney);
					if (num > 0 || !inputFanLiNewActivity.CanGiveAward(client, awardIndex, 0))
					{
						return false;
					}
					return true;
				}
				else
				{
					int money = Global.SafeConvertToInt32(array[2]);
					string[] records = array[1].Split(new char[]
					{
						','
					});
					string btnIndexStateListStr = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, money, type, records);
					string[] array3 = btnIndexStateListStr.Split(new char[]
					{
						','
					});
					bool flag;
					if (array3.Length > 0)
					{
						flag = array3.All((string x) => x.Equals("2"));
					}
					else
					{
						flag = false;
					}
					hasGet = flag;
					foreach (string text in array3)
					{
						if (text.Equals("1"))
						{
							return true;
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return false;
		}

		private static string GetFirstChargeInfo(GameClient client)
		{
			int num = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			string arg = string.Concat(new object[]
			{
				Global.CanGetFirstChongZhiDaLiByUserID(client) ? 1 : 0,
				num,
				":",
				1
			});
			return string.Format("{0}", arg);
		}

		private static string GetDailyChargeActiveInfo(GameClient client)
		{
			string result = "";
			int num = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			return result;
		}

		private static bool GetCmdDataField(TMSKSocket socket, int nID, byte[] data, int count, out string[] fields)
		{
			string text = null;
			fields = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return false;
			}
			fields = text.Split(new char[]
			{
				':'
			});
			return true;
		}

		private static TCPProcessCmdResults GetFirstChargeAward(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out array))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				try
				{
					if (array.Length != 3)
					{
						LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					int num = Convert.ToInt32(array[0]);
					GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
					if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
					{
						LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					string data2;
					if (gameClient.ClientData.CZTaskID > 0)
					{
						data2 = string.Format("{0}:{1}:{2}:", -10, 1, 1);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					Activity activity = Global.GetActivity(ActivityTypes.InputFirst);
					if (null == activity)
					{
						data2 = string.Format("{0}:{1}:{2}:", -1, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!Global.CanGetFirstChongZhiDaLiByUserID(gameClient))
					{
						data2 = string.Format("{0}:{1}:{2}:", -10, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient))
					{
						data2 = string.Format("{0}:{1}:{2}:", -20, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num2 = GameManager.ClientMgr.QueryTotaoChongZhiMoney(gameClient);
					if (num2 <= 0)
					{
						data2 = string.Format("{0}:{1}:{2}:", -30, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num3 = Global.CalcOriginalOccupationID(gameClient);
					activity.GiveAward(gameClient);
					Global.JugeCompleteChongZhiSecondTask(gameClient, 1);
					Global.BroadcastShouChongDaLiHint(gameClient);
					gameClient._IconStateMgr.CheckShouCiChongZhi(gameClient);
					data2 = string.Format("{0}:{1}:{2}:", 0, 1, 2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
				}
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			return result;
		}

		public static string BuildWriteActiveRecordStr(string record, int nBtnIndex)
		{
			string text = "";
			string[] array = record.Split(new char[]
			{
				','
			});
			List<string> list = new List<string>();
			int num = nBtnIndex;
			if (nBtnIndex < array.Length)
			{
				num = array.Length;
			}
			for (int i = 0; i < num; i++)
			{
				if (i < array.Length)
				{
					list.Add(array[i]);
				}
				else
				{
					list.Add("1");
				}
			}
			list[nBtnIndex - 1] = "2";
			for (int i = 0; i < list.Count; i++)
			{
				text += list[i];
				if (i < list.Count - 1)
				{
					text += ",";
				}
			}
			return text;
		}

		public static TCPProcessCmdResults ProcessGetRepayAwardCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out array))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				try
				{
					int num = Convert.ToInt32(array[0]);
					int num2 = Global.SafeConvertToInt32(array[1]);
					int num3 = Convert.ToInt32(array[2]);
					GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
					if (gameClient == null || gameClient.ClientData.RoleID != num)
					{
						LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					Activity activity = Global.GetActivity((ActivityTypes)num2);
					string data2;
					if (null == activity)
					{
						data2 = string.Format("{0}:{1}::", -1, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num4 = 0;
					string arg = "";
					ActivityTypes activityTypes = (ActivityTypes)num2;
					ActivityTypes activityTypes2 = activityTypes;
					if (activityTypes2 <= ActivityTypes.MeiRiChongZhiHaoLi)
					{
						if (activityTypes2 == ActivityTypes.InputFirst)
						{
							return RechargeRepayActiveMgr.GetFirstChargeAward(socket, pool, nID, data, count, out tcpOutPacket);
						}
						switch (activityTypes2)
						{
						case ActivityTypes.HeFuLogin:
						{
							if (!activity.InAwardTime())
							{
								data2 = string.Format("{0}:{1}::", -40, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (null == activity.GetAward(num3))
							{
								data2 = string.Format("{0}:{1}::", -50, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							HeFuLoginAwardType heFuLoginAwardType = (HeFuLoginAwardType)num3;
							HeFuLoginFlagTypes mask;
							if (HeFuLoginAwardType.NormalAward == heFuLoginAwardType)
							{
								mask = HeFuLoginFlagTypes.HeFuLogin_NormalAward;
							}
							else
							{
								if (HeFuLoginAwardType.VIPAward != heFuLoginAwardType)
								{
									LogManager.WriteLog(2, string.Format("TCPProcessCmdResults ProcessGetRepayAwardCmd 领取合服登陆奖励收到无效的领取类型 CMD={0}, Client={1}, RoleID={2}, nBtnIndex={3}", new object[]
									{
										(TCPGameServerCmds)nID,
										Global.GetSocketRemoteEndPoint(socket, false),
										num,
										num3
									}), null, true);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (!Global.IsVip(gameClient))
								{
									data2 = string.Format("{0}:{1}::", -100, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								mask = HeFuLoginFlagTypes.HeFuLogin_VIPAward;
							}
							int num5 = Global.GetRoleParamsInt32FromDB(gameClient, "HeFuLoginFlag");
							int intSomeBit = Global.GetIntSomeBit(num5, 1);
							if (0 == intSomeBit)
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							intSomeBit = Global.GetIntSomeBit(num5, (int)mask);
							if (0 != intSomeBit)
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							activity.GiveAward(gameClient, num3);
							num5 = Global.SetIntSomeBit((int)mask, num5, true);
							Global.SaveRoleParamsInt32ValueToDB(gameClient, "HeFuLoginFlag", num5, true);
							arg = string.Format("{0}", num5);
							if (gameClient._IconStateMgr.CheckHeFuActivity(gameClient))
							{
								gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							}
							break;
						}
						case ActivityTypes.HeFuTotalLogin:
						{
							if (!activity.InAwardTime())
							{
								data2 = string.Format("{0}:{1}::", -40, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (null == activity.GetAward(num3))
							{
								data2 = string.Format("{0}:{1}::", -50, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(gameClient, "HeFuTotalLoginNum");
							if (roleParamsInt32FromDB < num3)
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num5 = Global.GetRoleParamsInt32FromDB(gameClient, "HeFuTotalLoginFlag");
							int intSomeBit = Global.GetIntSomeBit(num5, num3);
							if (0 != intSomeBit)
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							activity.GiveAward(gameClient, num3);
							num5 = Global.SetIntSomeBit(num3, num5, true);
							Global.SaveRoleParamsInt32ValueToDB(gameClient, "HeFuTotalLoginFlag", num5, true);
							arg = string.Format("{0}", num5);
							if (gameClient._IconStateMgr.CheckHeFuActivity(gameClient))
							{
								gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							}
							break;
						}
						case ActivityTypes.HeFuRecharge:
						{
							int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
							int offsetDay2 = Global.GetOffsetDay(Global.GetHefuStartDay());
							if (offsetDay == offsetDay2)
							{
								data2 = string.Format("{0}:{1}::", -40, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							HeFuRechargeActivity heFuRechargeActivity = HuodongCachingMgr.GetHeFuRechargeActivity();
							if (null == heFuRechargeActivity)
							{
								data2 = string.Format("{0}:{1}::", -1, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!heFuRechargeActivity.InAwardTime())
							{
								data2 = string.Format("{0}:{1}::", -40, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array2 = null;
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								num,
								(int)activityTypes,
								offsetDay2,
								Global.GetOffsetDay(DateTime.Parse(heFuRechargeActivity.ToDate)),
								heFuRechargeActivity.strcoe
							}), out array2, gameClient.ServerId);
							if (array2 == null || 3 != array2.Length)
							{
								data2 = string.Format("{0}:{1}::", -60, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (heFuRechargeActivity.ActivityType != Convert.ToInt32(array2[1]))
							{
								data2 = string.Format("{0}:{1}::", -60, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num6 = Convert.ToInt32(array2[2]);
							if (num6 <= 0)
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string param = offsetDay2 + "_" + Global.GetOffsetDay(DateTime.Parse(heFuRechargeActivity.ToDate));
							if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num6, string.Format("领取{0}活动奖励", heFuRechargeActivity.ActivityType), ActivityTypes.HeFuRecharge, param))
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
							{
								num6
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
							GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", gameClient.ClientData.RoleID, num6, string.Format("领取{0}活动奖励", heFuRechargeActivity.ActivityType)), null, gameClient.ServerId);
							if (gameClient._IconStateMgr.CheckHeFuActivity(gameClient))
							{
								gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							}
							break;
						}
						case ActivityTypes.HeFuPKKing:
						{
							if (!activity.InAwardTime())
							{
								data2 = string.Format("{0}:{1}::", -40, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (num != HuodongCachingMgr.GetHeFuPKKingRoleID())
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num5 = Global.GetRoleParamsInt32FromDB(gameClient, "HeFuPKKingFlag");
							if (num5 != 0)
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							activity.GiveAward(gameClient);
							Global.SaveRoleParamsInt32ValueToDB(gameClient, "HeFuPKKingFlag", 1, true);
							arg = string.Format("{0}", Global.GetRoleParamsInt32FromDB(gameClient, "HeFuPKKingFlag").ToString());
							if (gameClient._IconStateMgr.CheckHeFuActivity(gameClient))
							{
								gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							}
							break;
						}
						case ActivityTypes.MeiRiChongZhiHaoLi:
						{
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array2 = null;
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", num, (int)activityTypes), out array2, gameClient.ServerId);
							if (array2 == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (array2 != null && array2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int num7 = Global.SafeConvertToInt32(array2[0]);
							if (num7 != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] array3 = array2[1].Split(new char[]
							{
								','
							});
							if (num3 > 0 && num3 <= array3.Length && array3[num3 - 1] == "2")
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							AwardItem award = activity.GetAward(gameClient, num3);
							if (award == null)
							{
								data2 = string.Format("{0}:{1}::", -1, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num8 = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(gameClient);
							num8 = Global.TransMoneyToYuanBao(num8);
							if (num8 < award.MinAwardCondionValue)
							{
								data2 = string.Format("{0}:{1}::", -5, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array4 = null;
							string text = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(array2[1], num3);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", num, (int)activityTypes, text.Replace(",", "")), out array4, gameClient.ServerId);
							if (array4 == null || array4.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							activity.GiveAward(gameClient, num3);
							Global.BroadcastDayChongDaLiHint(gameClient);
							gameClient._IconStateMgr.CheckMeiRiChongZhi(gameClient);
							arg = text;
							break;
						}
						}
					}
					else
					{
						switch (activityTypes2)
						{
						case ActivityTypes.TotalCharge:
						{
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array2 = null;
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", num, (int)activityTypes), out array2, gameClient.ServerId);
							if (array2 == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (array2 != null && array2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int num7 = Global.SafeConvertToInt32(array2[0]);
							if (num7 != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] array3 = array2[1].Split(new char[]
							{
								','
							});
							if (num3 > 0 && num3 <= array3.Length && array3[num3 - 1] == "2")
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num9 = GameManager.ClientMgr.QueryTotaoChongZhiMoney(gameClient);
							num9 = Global.TransMoneyToYuanBao(num9);
							if (!activity.CanGiveAward(gameClient, num3, num9))
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array4 = null;
							string text = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(array2[1], num3);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", num, 38, text.Replace(",", "")), out array4, gameClient.ServerId);
							if (array4 == null || array4.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							activity.GiveAward(gameClient, num3);
							RechargeRepayActiveMgr.BroadcastActiveHint(gameClient, ActivityTypes.TotalCharge);
							gameClient._IconStateMgr.CheckLeiJiChongZhi(gameClient);
							arg = text;
							break;
						}
						case ActivityTypes.TotalConsume:
						{
							if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
							{
								data2 = string.Format("{0}:{1}::", -20, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array2 = null;
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", num, 39), out array2, gameClient.ServerId);
							if (array2 == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (array2 != null && array2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int num7 = Global.SafeConvertToInt32(array2[0]);
							if (num7 != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] array3 = array2[1].Split(new char[]
							{
								','
							});
							if (num3 <= array3.Length && array3[num3 - 1] == "2")
							{
								data2 = string.Format("{0}:{1}::", -10, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num9 = Global.SafeConvertToInt32(array2[2]);
							if (!activity.CanGiveAward(gameClient, num3, num9))
							{
								data2 = string.Format("{0}:{1}::", -30, num2);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] array4 = null;
							string text = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(array2[1], num3);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", num, (int)activityTypes, text.Replace(",", "")), out array4, gameClient.ServerId);
							if (array4 == null || array4.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							activity.GiveAward(gameClient, num3);
							RechargeRepayActiveMgr.BroadcastActiveHint(gameClient, ActivityTypes.TotalConsume);
							gameClient._IconStateMgr.CheckLeiJiXiaoFei(gameClient);
							arg = text;
							break;
						}
						default:
							if (activityTypes2 != ActivityTypes.HeFuLuoLan)
							{
								switch (activityTypes2)
								{
								case ActivityTypes.OneDollarChongZhi:
								{
									if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
									{
										data2 = string.Format("{0}:{1}::", -20, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] array2 = null;
									TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										num,
										(int)activityTypes,
										activity.FromDate.Replace(':', '$'),
										activity.ToDate.Replace(':', '$')
									}), out array2, gameClient.ServerId);
									if (array2 == null)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (array2 != null && array2.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int num7 = Global.SafeConvertToInt32(array2[0]);
									if (num7 != 1)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									string[] array3 = array2[1].Split(new char[]
									{
										','
									});
									if (num3 > 0 && num3 <= array3.Length && array3[num3 - 1] == "2")
									{
										data2 = string.Format("{0}:{1}::", -10, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									int num9 = Global.SafeConvertToInt32(array2[2]);
									if (!activity.CanGiveAward(gameClient, num3, num9))
									{
										data2 = string.Format("{0}:{1}::", -30, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] array4 = null;
									string text = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(array2[1], num3);
									Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										num,
										46,
										text.Replace(",", ""),
										activity.FromDate.Replace(':', '$'),
										activity.ToDate.Replace(':', '$')
									}), out array4, gameClient.ServerId);
									if (array4 == null || array4.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									activity.GiveAward(gameClient);
									gameClient._IconStateMgr.CheckOneDollarChongZhi(gameClient);
									arg = text;
									break;
								}
								case ActivityTypes.InputFanLiNew:
								{
									string[] array2 = null;
									TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", num, (int)activityTypes), out array2, gameClient.ServerId);
									if (array2 == null)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (array2 != null && array2.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int num7 = Global.SafeConvertToInt32(array2[0]);
									if (num7 != 1)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int num10 = Global.SafeConvertToInt32(array2[1]);
									if (num10 > 0)
									{
										data2 = string.Format("{0}:{1}::", -10, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] array5 = array2[2].Split(new char[]
									{
										','
									});
									if (array5.Length != 2)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int chargeMoney = Global.SafeConvertToInt32(array5[0]);
									int consumeMoney = Global.SafeConvertToInt32(array5[1]);
									InputFanLiNew inputFanLiNew = activity as InputFanLiNew;
									num3 = inputFanLiNew.GetAwardIndex(gameClient, chargeMoney, consumeMoney);
									if (!activity.CanGiveAward(gameClient, num3, 0))
									{
										data2 = string.Format("{0}:{1}::", -30, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] array4 = null;
									Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", num, (int)activityTypes, num3), out array4, gameClient.ServerId);
									if (array4 == null || array4.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									arg = string.Format("{0}", num3);
									activity.GiveAward(gameClient, num3);
									if (gameClient._IconStateMgr.CheckInputFanLiNewActivity(gameClient))
									{
										gameClient._IconStateMgr.SendIconStateToClient(gameClient);
									}
									break;
								}
								}
							}
							else
							{
								if (!activity.InAwardTime())
								{
									data2 = string.Format("{0}:{1}::", -40, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								HeFuLuoLanAward heFuLuoLanAward = (activity as HeFuLuoLanActivity).GetHeFuLuoLanAward(num3);
								if (null == heFuLuoLanAward)
								{
									data2 = string.Format("{0}:{1}::", -50, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								int num11 = 0;
								int num12 = 0;
								int num13 = 0;
								string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
								string[] array6 = gameConfigItemStr.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < array6.Length; i++)
								{
									string[] array7 = array6[i].Split(new char[]
									{
										','
									});
									if (2 == array7.Length)
									{
										if (Convert.ToInt32(array7[0]) == gameClient.ClientData.Faction)
										{
											num11++;
											if (Convert.ToInt32(array7[1]) != gameClient.ClientData.RoleID)
											{
												num13++;
											}
										}
										if (Convert.ToInt32(array7[1]) == gameClient.ClientData.RoleID)
										{
											num12++;
										}
									}
								}
								if (num11 < heFuLuoLanAward.winNum)
								{
									data2 = string.Format("{0}:{1}::", -30, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (1 == heFuLuoLanAward.status)
								{
									if (num12 < heFuLuoLanAward.winNum)
									{
										data2 = string.Format("{0}:{1}::", -30, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
								}
								else if (2 == heFuLuoLanAward.status)
								{
									if (num13 < heFuLuoLanAward.winNum)
									{
										data2 = string.Format("{0}:{1}::", -30, num2);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
								}
								int num5 = Global.GetRoleParamsInt32FromDB(gameClient, "HeFuLuoLanAwardFlag");
								int intSomeBit = Global.GetIntSomeBit(num5, num3);
								if (0 != intSomeBit)
								{
									data2 = string.Format("{0}:{1}::", -10, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (!activity.HasEnoughBagSpaceForAwardGoods(gameClient, num3))
								{
									data2 = string.Format("{0}:{1}::", -20, num2);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								activity.GiveAward(gameClient, num3);
								num5 = Global.SetIntSomeBit(num3, num5, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "HeFuLuoLanAwardFlag", num5, true);
								arg = string.Format("{0}", num3);
								if (gameClient._IconStateMgr.CheckHeFuActivity(gameClient))
								{
									gameClient._IconStateMgr.SendIconStateToClient(gameClient);
								}
							}
							break;
						}
					}
					data2 = string.Format("{0}:{1}:{2}", num4, num2, arg);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
				}
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			return result;
		}

		public static void BroadcastActiveHint(GameClient client, ActivityTypes activeType)
		{
			string text = "";
			switch (activeType)
			{
			case ActivityTypes.TotalCharge:
				text = GLang.GetLang(528, new object[0]);
				break;
			case ActivityTypes.TotalConsume:
				text = GLang.GetLang(529, new object[0]);
				break;
			}
			string msgText = StringUtil.substitute(GLang.GetLang(530, new object[0]), new object[]
			{
				Global.FormatRoleName(client, client.ClientData.RoleName),
				text
			});
			Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
		}
	}
}
