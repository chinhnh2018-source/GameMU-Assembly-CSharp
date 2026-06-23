using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class UserRegressActiveManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		public static UserRegressActiveManager getInstance()
		{
			return UserRegressActiveManager.instance;
		}

		public bool InitConfig()
		{
			return true;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2070, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2071, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2072, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2073, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2074, 4, 4, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2075, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2076, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2077, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2070:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					string text;
					int num3;
					int num4;
					int num5;
					int num6;
					int num2 = Convert.ToInt32(this.ProcessGetRegressAcitveFile(client, out text, out num3, out num4, out num5, out num6));
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						num2,
						text,
						num3,
						num4,
						num5,
						num6
					}), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_GETFILE", false, false);
				}
				break;
			case 2071:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, int> cmdData;
					int num2 = Convert.ToInt32(this.ProcessRegressSignInfo(client, out cmdData));
					client.sendCmd<Dictionary<int, int>>(nID, cmdData, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_GETSIGNINFO", false, false);
				}
				break;
			case 2072:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int level = Convert.ToInt32(cmdParams[0]);
					int day = Convert.ToInt32(cmdParams[1]);
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveDaySignAward(client, level, day));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_SING", false, false);
				}
				break;
			case 2073:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, int> cmdData2;
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveGetStoreInfo(client, out cmdData2));
					client.sendCmd<Dictionary<int, int>>(nID, cmdData2, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_GETSTOREINFO", false, false);
				}
				break;
			case 2074:
				if (cmdParams == null || cmdParams.Length != 4)
				{
					return false;
				}
				try
				{
					int storeConfID = Convert.ToInt32(cmdParams[0]);
					int level = Convert.ToInt32(cmdParams[1]);
					int goodsID = Convert.ToInt32(cmdParams[2]);
					int count = Convert.ToInt32(cmdParams[3]);
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveStore(client, storeConfID, level, goodsID, count));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_STOREBUY", false, false);
				}
				break;
			case 2075:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					int num7;
					string arg;
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveRechargeInfo(client, num, out num7, out arg));
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, num7, arg), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_INPUTINFO", false, false);
				}
				break;
			case 2076:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int level = Convert.ToInt32(cmdParams[0]);
					int rechargeConfID = Convert.ToInt32(cmdParams[1]);
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveRecharge(client, level, rechargeConfID));
					client.sendCmd(nID, string.Format("{0}", num2), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_INPUT", false, false);
				}
				break;
			case 2077:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					int level = Convert.ToInt32(cmdParams[1]);
					Dictionary<int, int> cmdData3;
					int num2 = Convert.ToInt32(this.ProcessRegressAcitveDayBuy(client, num, level, out cmdData3));
					client.sendCmd<Dictionary<int, int>>(nID, cmdData3, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_REGRESSACTIVE_ZHIGOU_QUERY", false, false);
				}
				break;
			}
			return true;
		}

		public static bool GetRegressMinRegtime(GameClient client, out string Regtime)
		{
			string[] array = null;
			Regtime = "";
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.strUserID);
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14130, strcmd, out array, client.ServerId);
			bool result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (array == null || array.Length != 2)
			{
				result = false;
			}
			else if (!array[0].Equals(client.strUserID))
			{
				result = false;
			}
			else
			{
				Regtime = array[1].Replace("$", ":");
				result = true;
			}
			return result;
		}

		public RegressActiveOpcode ProcessGetRegressAcitveFile(GameClient client, out string ToClientRegtime, out int ToClientID, out int ToClientLevel, out int CurrDay, out int ActiveMoney)
		{
			ToClientRegtime = "";
			ToClientID = 0;
			ToClientLevel = 0;
			CurrDay = 0;
			ActiveMoney = 0;
			string strUserID = client.strUserID;
			RegressActiveOpcode result;
			if (strUserID == null || strUserID.Equals("") || !strUserID.Equals(client.strUserID))
			{
				result = RegressActiveOpcode.RegressActiveUserInfoErr;
			}
			else
			{
				RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
				string text;
				if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
				{
					result = RegressActiveOpcode.RegressActiveOpenErr;
				}
				else if (!regressActiveOpen.CanGiveAward())
				{
					result = RegressActiveOpcode.RegressActiveNotIn;
				}
				else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
				{
					result = RegressActiveOpcode.RegressActiveGetRegTime;
				}
				else
				{
					int num;
					int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num);
					if (0 == userActiveFile)
					{
						result = RegressActiveOpcode.RegressActiveGetFile;
					}
					else
					{
						ToClientRegtime = text.Replace(":", "$");
						ToClientID = num;
						ToClientLevel = userActiveFile;
						DateTime now = TimeUtil.NowDateTime();
						int num2 = Global.GetOffsetDay(now) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
						CurrDay = num2 + 1;
						string arg = "2011-11-11 00$00$00";
						string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, arg, regressActiveOpen.FromDate.Replace(':', '$'));
						string[] array;
						if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
						{
							result = RegressActiveOpcode.RegressActiveGetFile;
						}
						else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
						{
							result = RegressActiveOpcode.RegressActiveGetFile;
						}
						else
						{
							int num3 = Convert.ToInt32(array[1]);
							if (num3 < 0)
							{
								num3 = 0;
							}
							ActiveMoney = num3;
							result = RegressActiveOpcode.RegressActiveSucc;
						}
					}
				}
			}
			return result;
		}

		public Dictionary<int, int> ProcessSignInfo(Dictionary<string, string> SignInfoStr, int ThisDay, out string SignDay)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			SignDay = "";
			foreach (KeyValuePair<string, string> keyValuePair in SignInfoStr)
			{
				if (ThisDay - Global.GetOffsetDay(DateTime.Parse(keyValuePair.Key)) == 0)
				{
					SignDay = keyValuePair.Value;
				}
				if (!keyValuePair.Value.Equals(""))
				{
					string[] array = keyValuePair.Value.Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						if (!text.Equals(""))
						{
							int key = Convert.ToInt32(text);
							if (dictionary.ContainsKey(key))
							{
								return null;
							}
							dictionary.Add(key, 1);
						}
					}
				}
			}
			return dictionary;
		}

		public Dictionary<int, int> ChangeData(Dictionary<string, string> SignInfoStr, Dictionary<int, int> Info)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> result;
			if (SignInfoStr.Count == 0 && Info.Count == 0)
			{
				dictionary.Add(1, 0);
				result = dictionary;
			}
			else
			{
				for (int i = 1; i <= SignInfoStr.Count; i++)
				{
					if (!Info.ContainsKey(i))
					{
						dictionary.Add(i, 0);
					}
					else
					{
						dictionary.Add(i, Info[i]);
					}
				}
				result = dictionary;
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressSignInfo(GameClient client, out Dictionary<int, int> SignInfo)
		{
			SignInfo = new Dictionary<int, int>();
			RegressActiveOpcode result;
			if (!client.strUserID.Equals(client.strUserID))
			{
				result = RegressActiveOpcode.RegressActiveSignGetInfoFail;
			}
			else
			{
				RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
				if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
				{
					result = RegressActiveOpcode.RegressActiveOpenErr;
				}
				else
				{
					string arg = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
					string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, arg);
					Dictionary<string, string> dictionary = Global.sendToDB<Dictionary<string, string>, string>(14132, cmd, 0);
					if (dictionary == null)
					{
						result = RegressActiveOpcode.RegressActiveSignSelectFail;
					}
					else
					{
						DateTime now = TimeUtil.NowDateTime();
						int offsetDay = Global.GetOffsetDay(now);
						string text;
						Dictionary<int, int> dictionary2 = this.ProcessSignInfo(dictionary, offsetDay, out text);
						if (dictionary2 == null)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else if (dictionary2.Count > dictionary.Count)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else
						{
							Dictionary<int, int> dictionary3 = this.ChangeData(dictionary, dictionary2);
							SignInfo = dictionary3;
							result = RegressActiveOpcode.RegressActiveSucc;
						}
					}
				}
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveDaySignAward(GameClient client, int Level, int Day)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string text;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int num;
				int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num);
				if (0 == userActiveFile)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (userActiveFile != Level)
				{
					result = RegressActiveOpcode.RegressActiveSignCheckFail;
				}
				else
				{
					string text2 = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
					string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, text2);
					Dictionary<string, string> dictionary = Global.sendToDB<Dictionary<string, string>, string>(14132, cmd, 0);
					if (dictionary == null)
					{
						result = RegressActiveOpcode.RegressActiveSignSelectFail;
					}
					else
					{
						DateTime now = TimeUtil.NowDateTime();
						DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
						DateTime dateTime2 = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
						string text3;
						Dictionary<int, int> dictionary2 = this.ProcessSignInfo(dictionary, Global.GetOffsetDay(now), out text3);
						if (dictionary2 == null)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else
						{
							Dictionary<int, int> dictionary3 = this.ChangeData(dictionary, dictionary2);
							int num2;
							if (!dictionary3.TryGetValue(Day, out num2))
							{
								result = RegressActiveOpcode.RegressActiveSignNotDay;
							}
							else if (num2 == 1)
							{
								result = RegressActiveOpcode.RegressActiveSignHas;
							}
							else
							{
								RegressActiveSignGift regressActiveSignGift = HuodongCachingMgr.GetRegressActiveSignGift();
								List<GoodsData> goodsData;
								int num3;
								if (regressActiveSignGift == null || !regressActiveSignGift.InActivityTime())
								{
									result = RegressActiveOpcode.RegressActiveSignConfErr;
								}
								else if (!regressActiveSignGift.GetAwardGoodsList(client, Level, Day, out goodsData, out num3))
								{
									result = RegressActiveOpcode.RegressActiveSignGetAwardFail;
								}
								else
								{
									int num4;
									if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, goodsData, out num4))
									{
										if (num4 == 1)
										{
											return RegressActiveOpcode.RegressActiveSignRebornBagFail;
										}
										if (num4 == 2)
										{
											return RegressActiveOpcode.RegressActiveSignBaseBagFail;
										}
									}
									string arg = dateTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
									string arg2 = dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
									string text4 = string.Format("{0}_{1}", arg, arg2);
									text3 = text3 + "|" + num3.ToString();
									string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										client.ClientData.RoleID,
										111,
										text3,
										text2
									});
									string[] array = Global.ExecuteDBCmd(14138, strcmd, 0);
									if (array == null || array.Length != 4)
									{
										result = RegressActiveOpcode.RegressActiveSignRecordFail;
									}
									else if (!regressActiveSignGift.GiveAward(client, goodsData))
									{
										result = RegressActiveOpcode.RegressActiveSignGiveAwardFail;
									}
									else
									{
										result = RegressActiveOpcode.RegressActiveSucc;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveRechargeInfo(GameClient client, int RoleID, out int Money, out string ConfIDList)
		{
			Money = 0;
			ConfIDList = "";
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				string text = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, regressActiveOpen.FromDate.Replace(':', '$'), text.Replace(':', '$'));
				string[] array;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
				{
					result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
				}
				else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
				{
					result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
				}
				else
				{
					Money = Convert.ToInt32(array[1]);
					if (Money < 0)
					{
						Money = 0;
					}
					string arg = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
					string strcmd2 = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 112, arg);
					string[] array2;
					Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, strcmd2, out array2, 0);
					if (array2 == null || array2.Length != 4 || Convert.ToInt32(array2[0]) != client.ClientData.RoleID)
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else
					{
						if (!array2[3].Equals(""))
						{
							string[] array3 = array2[3].Split(new char[]
							{
								'|'
							});
							int num = 0;
							foreach (string str in array3)
							{
								num++;
								ConfIDList += str;
								if (num == array3.Length)
								{
									break;
								}
								ConfIDList += "_";
							}
						}
						result = RegressActiveOpcode.RegressActiveSucc;
					}
				}
			}
			return result;
		}

		public void RoleOnlineHandler(GameClient client)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			if (null != regressActiveOpen)
			{
				regressActiveOpen.OnRoleLogin(client);
			}
			else
			{
				regressActiveOpen.Init();
			}
			if (RegressActiveOpen.OpenStateVavle == 1)
			{
				string arg = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, arg);
				string[] array;
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, strcmd, out array, 0);
			}
		}

		public string RepairRegressStoreData(string[] idList)
		{
			string text = "";
			string result;
			if (idList.Length == 0)
			{
				result = text;
			}
			else
			{
				List<string> list = new List<string>();
				foreach (string text2 in idList)
				{
					if (!text2.Equals(""))
					{
						if (-1 == list.IndexOf(text2))
						{
							list.Add(text2);
						}
					}
				}
				foreach (string text2 in list)
				{
					text += "_";
					text += text2;
				}
				result = text;
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveRecharge(GameClient client, int Level, int RechargeConfID)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string text;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int num;
				int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num);
				if (0 == userActiveFile)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (userActiveFile != Level)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					string text2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, regressActiveOpen.FromDate.Replace(':', '$'), text2.Replace(':', '$'));
					string[] array;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else
					{
						int num2 = Convert.ToInt32(array[1]);
						if (num2 < 0)
						{
							num2 = 0;
						}
						string text3 = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
						string strcmd2 = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 112, text3);
						string text4 = "";
						string[] array2;
						Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, strcmd2, out array2, 0);
						if (array2 == null || array2.Length != 4 || Convert.ToInt32(array2[0]) != client.ClientData.RoleID)
						{
							result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
						}
						else
						{
							if (!array2[3].Equals(""))
							{
								string[] array3 = array2[3].Split(new char[]
								{
									'_'
								});
								foreach (string text5 in array3)
								{
									if (!text5.Equals("") && Convert.ToInt32(text5) == RechargeConfID)
									{
										return RegressActiveOpcode.RegressActiveInputHas;
									}
								}
								text4 = this.RepairRegressStoreData(array3);
								if (!string.IsNullOrEmpty(text4) && !text4.Equals(array2[3]))
								{
									string strcmd3 = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										client.ClientData.RoleID,
										112,
										text4,
										text3
									});
									string[] array5 = Global.ExecuteDBCmd(14138, strcmd3, 0);
									if (array5 == null || array5.Length != 4)
									{
										return RegressActiveOpcode.RegressActiveUpdateInputInfoErr;
									}
								}
							}
							RegressActiveTotalRecharge regressActiveTotalRecharge = HuodongCachingMgr.GetRegressActiveTotalRecharge();
							List<GoodsData> list;
							if (regressActiveTotalRecharge == null || !regressActiveTotalRecharge.InActivityTime())
							{
								result = RegressActiveOpcode.RegressActiveInputConfErr;
							}
							else if (regressActiveTotalRecharge.GiveAwardCheck(client, Level, num2, RechargeConfID, out list))
							{
								result = RegressActiveOpcode.RegressActiveInputCheckAwardErr;
							}
							else if (list == null)
							{
								result = RegressActiveOpcode.RegressActiveInputCheckAwardErr;
							}
							else
							{
								int num3;
								if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, list, out num3))
								{
									if (num3 == 1)
									{
										return RegressActiveOpcode.RegressActiveSignRebornBagFail;
									}
									if (num3 == 2)
									{
										return RegressActiveOpcode.RegressActiveSignBaseBagFail;
									}
								}
								DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
								DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
								string arg = dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
								string arg2 = dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
								string text6 = string.Format("{0}_{1}", arg, arg2);
								string text7 = text4 + "_" + RechargeConfID.ToString();
								string strcmd4 = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									client.ClientData.RoleID,
									112,
									text7,
									text3
								});
								string[] array6 = Global.ExecuteDBCmd(14138, strcmd4, 0);
								if (array6 == null || array6.Length != 4)
								{
									result = RegressActiveOpcode.RegressActiveUpdateInputInfoErr;
								}
								else if (!regressActiveTotalRecharge.GiveAward(client, list))
								{
									result = RegressActiveOpcode.RegressActiveInputGiveAwardErr;
								}
								else
								{
									result = RegressActiveOpcode.RegressActiveSucc;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveDayBuy(GameClient client, int Role, int Level, out Dictionary<int, int> ZhiGouDict)
		{
			ZhiGouDict = null;
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string text;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int num;
				int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num);
				if (0 == userActiveFile)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (userActiveFile != Level)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					RegressActiveDayBuy regressActiveDayBuy = HuodongCachingMgr.GetRegressActiveDayBuy();
					if (null == regressActiveDayBuy)
					{
						result = RegressActiveOpcode.RegressActiveBuyGetInfoErr;
					}
					else
					{
						dictionary = regressActiveDayBuy.BuildRegressZhiGouInfoForClient(client);
						ZhiGouDict = dictionary;
						result = RegressActiveOpcode.RegressActiveSucc;
					}
				}
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveGetStoreInfo(GameClient client, out Dictionary<int, int> GoodInfo)
		{
			GoodInfo = null;
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				string s = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int num = Global.GetOffsetDay(DateTime.Parse(s)) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
				if (num < 0)
				{
					result = RegressActiveOpcode.RegressActiveStoreCheckDayFail;
				}
				else
				{
					string arg = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
					string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, num, arg);
					GoodInfo = Global.sendToDB<Dictionary<int, int>, string>(14133, cmd, 0);
					result = RegressActiveOpcode.RegressActiveSucc;
				}
			}
			return result;
		}

		public RegressActiveOpcode ProcessRegressAcitveStore(GameClient client, int StoreConfID, int Level, int GoodsID, int Count)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string text;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int num;
				int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num);
				if (0 == userActiveFile)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (userActiveFile != Level)
				{
					result = RegressActiveOpcode.RegressActiveStoreCheckFail;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					string s = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
					int num2 = Global.GetOffsetDay(DateTime.Parse(s)) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
					if (num2 < 0)
					{
						result = RegressActiveOpcode.RegressActiveStoreCheckDayFail;
					}
					else
					{
						RegressActiveStore regressActiveStore = HuodongCachingMgr.GetRegressActiveStore();
						if (null == regressActiveStore)
						{
							result = RegressActiveOpcode.RegressActiveStoreConfErr;
						}
						else
						{
							string text2 = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate)), Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.ToDate)));
							int num3;
							int num4;
							GoodsData goodsData;
							if (!regressActiveStore.RegressStoreGoodsBuyCheck(client, StoreConfID, Level, num2 + 1, GoodsID, Count, text2, out num3, out num4, out goodsData))
							{
								result = RegressActiveOpcode.RegressActiveStoreBuyFail;
							}
							else if (goodsData == null)
							{
								result = RegressActiveOpcode.RegressActiveStoreCheckGoodFail;
							}
							else if (num4 <= 0 || num3 <= 0)
							{
								result = RegressActiveOpcode.RegressActiveStoreCheckParmErr;
							}
							else
							{
								int num5;
								if (!RebornEquip.OneIsCanIntoRebornOrBaseBag(client, goodsData, out num5))
								{
									if (num5 == 1)
									{
										return RegressActiveOpcode.RegressActiveSignRebornBagFail;
									}
									if (num5 == 2)
									{
										return RegressActiveOpcode.RegressActiveSignBaseBagFail;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, "三周年商城购买物品", true, true, false, DaiBiSySType.None))
								{
									result = RegressActiveOpcode.RegressActiveStoreUserYuanBaoFail;
								}
								else
								{
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										client.ClientData.RoleID,
										StoreConfID,
										num2,
										num4,
										text2
									});
									string[] array;
									if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14135, strcmd, out array, 0))
									{
										result = RegressActiveOpcode.RegressActiveStoreInsertInfoErr;
									}
									else if (array == null || array.Length != 2 || Convert.ToInt32(array[1]) != 0)
									{
										result = RegressActiveOpcode.RegressActiveStoreInsertInfoErr;
									}
									else
									{
										if (Global.GetGoodsRebornEquip(goodsData.GoodsID) == 1)
										{
											Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, Count, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 15000, goodsData.Jewellist, true, 1, "三周年商城购买", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
										}
										else
										{
											Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, Count, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "三周年商城购买", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
										}
										result = RegressActiveOpcode.RegressActiveSucc;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static UserRegressActiveManager instance = new UserRegressActiveManager();
	}
}
