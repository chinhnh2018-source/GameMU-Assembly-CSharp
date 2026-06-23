using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.AoYunDaTi
{
	public class AoYunDaTiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static AoYunDaTiManager getInstance()
		{
			return AoYunDaTiManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(20200, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20201, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20202, 2, 2, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20203, 1, 1, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20204, 2, 2, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(20205, 3, 3, AoYunDaTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool LoadConfig()
		{
			this.Destory_Work();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName(AoyunDaTiConsts.GoodsParams);
				if (paramValueByName == "" || paramValueByName == null)
				{
					return true;
				}
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				string[] array2 = array[0].Split(new char[]
				{
					','
				});
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice = new int[2];
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit = new int[2];
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[0] = Convert.ToInt32(array2[0]);
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0] = Convert.ToInt32(array2[1]);
				string[] array3 = array[1].Split(new char[]
				{
					','
				});
				AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[1] = Convert.ToInt32(array3[0]);
				AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1] = Convert.ToInt32(array3[1]);
				AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuanShengExpXiShu", ',');
			}
			AoYunDaTiManager.LoadQuestionTime();
			AoYunDaTiManager.LoadQuestionBank();
			AoYunDaTiManager.LoadPaiHangAwad();
			AoYunDaTiManager.LoadAoYunDaTi();
			DateTime dateTime = TimeUtil.NowDateTime();
			this.SetCurrentQuestionTimeItem(dateTime);
			AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = (currentQuestionTimeItem != null && dateTime > currentQuestionTimeItem.BeginTime);
			}
			AoYunDaTiManager.InitAoyunRank();
			AoYunDaTiManager.InitLastAoyunRank();
			AoYunDaTiManager.CheckActivityIcon(AoYunDaTiManager.AoyunRunTimeData.DaTiOpen);
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 20200:
					result = this.ProcessAoYunInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 20201:
					result = this.ProcessGetQuestionCmd(client, nID, bytes, cmdParams);
					break;
				case 20202:
					result = this.ProcessAnswerQuestionCmd(client, nID, bytes, cmdParams);
					break;
				case 20203:
					result = this.ProcessRecPaiHangAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 20204:
					result = this.ProcessUseGoodsCmd(client, nID, bytes, cmdParams);
					break;
				case 20205:
					result = this.ProcessBuyGoodsCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ProcessAoYunInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				bool flag = false;
				int num = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey - 1;
				if (num >= 0 && num < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					AoyunQuestionTimeItem aoyunQuestionTimeItem = AoYunDaTiManager.XmlQuestionTimeList[num];
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20006");
					DateTime beginTime = aoyunQuestionTimeItem.BeginTime;
					flag = (roleParamsDateTimeFromDB > beginTime);
					DateTime roleParamsDateTimeFromDB2 = Global.GetRoleParamsDateTimeFromDB(client, "20005");
					DateTime endTime = aoyunQuestionTimeItem.EndTime;
					flag = (flag && endTime > roleParamsDateTimeFromDB2);
				}
				AoyunQuestionTimeItem aoyunQuestionTimeItem2 = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (aoyunQuestionTimeItem2 == null)
				{
					aoyunQuestionTimeItem2 = new AoyunQuestionTimeItem
					{
						BeginTime = DateTime.MaxValue,
						EndTime = DateTime.MinValue
					};
				}
				List<AoyunPaiHangRoleData> list = new List<AoyunPaiHangRoleData>();
				foreach (AoyunPaiHangRoleData aoyunPaiHangRoleData in AoYunDaTiManager.AoyunRunTimeData.AoyunRankList)
				{
					if (aoyunPaiHangRoleData.RoleRank < 0 || aoyunPaiHangRoleData.RoleRank > 20)
					{
						break;
					}
					list.Add(aoyunPaiHangRoleData);
				}
				AoyunDatiMainData aoyunDatiMainData = new AoyunDatiMainData
				{
					AoyunPaiHangRoleDataArray = list,
					StartTime = aoyunQuestionTimeItem2.BeginTime,
					EndTime = aoyunQuestionTimeItem2.EndTime,
					SelfRank = -1,
					TianShiNum = Global.GetRoleParamsInt32FromDB(client, "20002"),
					EMoNum = Global.GetRoleParamsInt32FromDB(client, "20004"),
					NextStartTime = DateTime.MaxValue
				};
				int num2 = 0;
				if (AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
				{
					AoyunPaiHangRoleData aoyunPaiHangRoleData2 = AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Find((AoyunPaiHangRoleData _x) => _x.RoleId == roleID);
					if (aoyunPaiHangRoleData2 != null)
					{
						aoyunDatiMainData.SelfRank = aoyunPaiHangRoleData2.RoleRank;
					}
				}
				if (AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(roleID, out num2) && !AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
				{
					aoyunDatiMainData.SelfRank = num2;
				}
				aoyunDatiMainData.IsHaveAward = (flag && num2 > 0);
				num = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1;
				if (num > 0 && num < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					AoyunQuestionTimeItem aoyunQuestionTimeItem3 = AoYunDaTiManager.XmlQuestionTimeList[num];
					aoyunDatiMainData.NextStartTime = aoyunQuestionTimeItem3.BeginTime;
				}
				else
				{
					aoyunDatiMainData.NextStartTime = DateTime.MaxValue;
				}
				client.sendCmd<AoyunDatiMainData>(nID, aoyunDatiMainData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetQuestionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				DateTime now = TimeUtil.NowDateTime();
				int num = Convert.ToInt32(cmdParams[0]);
				AoyunQuestionItemData roleQuestionFromDB = AoYunDaTiManager.GetRoleQuestionFromDB(client, now);
				client.sendCmd<AoyunQuestionItemData>(nID, roleQuestionFromDB, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAnswerQuestionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				DateTime dateTime = TimeUtil.NowDateTime();
				int key = Convert.ToInt32(cmdParams[0]);
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = 1;
				DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20006");
				AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
				if (currentQuestionBank == null)
				{
					num2 = -1007;
				}
				else if (num < 0 || num > 3)
				{
					num2 = -1006;
				}
				else if (roleParamsDateTimeFromDB > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
				{
					num2 = -1005;
				}
				else if (dateTime > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestionBank.QuestionAwardXml.ExamTime))
				{
					num2 = -1;
				}
				else
				{
					if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime < Global.GetRoleParamsDateTimeFromDB(client, "20001"))
					{
						int[] answerIndex = currentQuestionBank.QuestionAwardXml.AnswerIndex;
						if (num == answerIndex[1] || num == answerIndex[2])
						{
							num2 = -1007;
							client.sendCmd(nID, num2.ToString(), false);
							return true;
						}
					}
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic[key] = num;
					}
					Global.SaveRoleParamsDateTimeToDB(client, "20006", dateTime, true);
				}
				client.sendCmd(nID, num2.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetPaiHangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string filePath = Global.GameResPath(AoyunDaTiConsts.QuestionAward);
				XElement xelement = CheckHelper.LoadXml(filePath, true);
				if (null == xelement)
				{
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessRecPaiHangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				DateTime dateTime = TimeUtil.NowDateTime();
				int key = Convert.ToInt32(cmdParams[0]);
				string text = "";
				int num = -1;
				if (!AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(key, out num))
				{
					num = -1;
				}
				string cmdData;
				if (num < 0)
				{
					cmdData = "-1:0:";
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int num2 = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey - 1;
				if (num2 < 0)
				{
					cmdData = "-1:0:";
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				AoyunQuestionTimeItem aoyunQuestionTimeItem = null;
				if (num2 < AoYunDaTiManager.XmlQuestionTimeList.Count)
				{
					aoyunQuestionTimeItem = AoYunDaTiManager.XmlQuestionTimeList[num2];
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20005");
					DateTime endTime = aoyunQuestionTimeItem.EndTime;
					if (endTime <= roleParamsDateTimeFromDB)
					{
						cmdData = "-1002:" + num + ":";
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					DateTime roleParamsDateTimeFromDB2 = Global.GetRoleParamsDateTimeFromDB(client, "20006");
					DateTime beginTime = aoyunQuestionTimeItem.BeginTime;
					if (roleParamsDateTimeFromDB2 < beginTime)
					{
						cmdData = "-1:0:";
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				int i;
				for (i = 0; i < AoYunDaTiManager.XmlPaiHangAward.Count - 1; i++)
				{
					if (num >= AoYunDaTiManager.XmlPaiHangAward[i].BeginNum && num <= AoYunDaTiManager.XmlPaiHangAward[i].EndNum)
					{
						text = AoYunDaTiManager.XmlPaiHangAward[i].GoodsOne;
						AoYunDaTiManager.GiveGoodsAward(client, text);
						Global.SaveRoleParamsDateTimeToDB(client, "20005", aoyunQuestionTimeItem.EndTime, true);
						break;
					}
				}
				if (i == AoYunDaTiManager.XmlPaiHangAward.Count - 1)
				{
					text = AoYunDaTiManager.XmlPaiHangAward[i].GoodsOne;
					AoYunDaTiManager.GiveGoodsAward(client, text);
					Global.SaveRoleParamsDateTimeToDB(client, "20005", aoyunQuestionTimeItem.EndTime, true);
				}
				cmdData = string.Concat(new object[]
				{
					"1:",
					num,
					":",
					text
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessUseGoodsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				DateTime dateTime = TimeUtil.NowDateTime();
				int key = Convert.ToInt32(cmdParams[0]);
				int num = Convert.ToInt32(cmdParams[1]);
				string roleParamsKey = (num == 0) ? "20001" : "20003";
				DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, roleParamsKey);
				string text = (num == 0) ? "20002" : "20004";
				int num2 = Global.GetRoleParamsInt32FromDB(client, text);
				AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
				string text2;
				if (dateTime > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestionBank.QuestionAwardXml.ExamTime))
				{
					text2 = string.Concat(new object[]
					{
						"-1005:-1:-1:",
						num,
						":",
						num2
					});
					client.sendCmd(nID, text2, false);
					return true;
				}
				if (AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.ContainsKey(key))
				{
					text2 = string.Concat(new object[]
					{
						"-1006:-1:-1:",
						num,
						":",
						num2
					});
					client.sendCmd(nID, text2, false);
					return true;
				}
				if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime < roleParamsDateTimeFromDB)
				{
					text2 = "-1001:";
					if (num == 0)
					{
						int[] answerIndex = currentQuestionBank.QuestionAwardXml.AnswerIndex;
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							answerIndex[1],
							":",
							answerIndex[2],
							":0:",
							num2
						});
					}
					else
					{
						text2 = text2 + "-1:-1:1:" + num2;
					}
					client.sendCmd(nID, text2, false);
					return true;
				}
				if (num2 <= 0)
				{
					text2 = string.Concat(new object[]
					{
						"-1:-1:-1:",
						num,
						":",
						num2
					});
					client.sendCmd(nID, text2, false);
					return true;
				}
				num2--;
				if (num == 0)
				{
					int[] answerIndex = currentQuestionBank.QuestionAwardXml.AnswerIndex;
					text2 = string.Concat(new object[]
					{
						"1:",
						answerIndex[1],
						":",
						answerIndex[2],
						":0:",
						num2
					});
				}
				else
				{
					text2 = "1:-1:-1:1:" + num2;
				}
				Global.SaveRoleParamsInt32ValueToDB(client, text, num2, true);
				Global.SaveRoleParamsDateTimeToDB(client, roleParamsKey, dateTime, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具使用", text, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
				client.sendCmd(nID, text2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuyGoodsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				DateTime dateTime = TimeUtil.NowDateTime();
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				string cmdData;
				if (num2 < 0 || num2 > 1 || num3 < 1 || num3 > AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[num2])
				{
					cmdData = "-1004::";
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				string text = (num2 == 0) ? "20002" : "20004";
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, text);
				if (num3 + roleParamsInt32FromDB > AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[num2])
				{
					cmdData = string.Concat(new object[]
					{
						"-1:",
						num2,
						":",
						roleParamsInt32FromDB
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int subMoney = num3 * AoYunDaTiManager.AoyunRunTimeData.GoodsPrice[num2];
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, "奥运答题道具购买", true, true, false, DaiBiSySType.None))
				{
					cmdData = string.Concat(new object[]
					{
						"-1003:",
						num2,
						":",
						roleParamsInt32FromDB
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				num3 += roleParamsInt32FromDB;
				Global.SaveRoleParamsInt32ValueToDB(client, text, num3, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具购买", text, client.ClientData.RoleName, "系统", "修改", 1, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
				cmdData = string.Concat(new object[]
				{
					"1:",
					num2,
					":",
					num3
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private void SetCurrentQuestionTimeItem(DateTime time)
		{
			for (int i = 0; i < AoYunDaTiManager.XmlQuestionTimeList.Count; i++)
			{
				DateTime endTime = AoYunDaTiManager.XmlQuestionTimeList[i].EndTime;
				if (!(time >= endTime))
				{
					lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
					{
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = i;
					}
					this.SetCurrentQuestionList(time);
					this.SetCurrentQuestionNum(time);
					return;
				}
			}
			lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
			{
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = AoYunDaTiManager.XmlQuestionTimeList.Count;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
			}
			LogManager.WriteLog(2, string.Format("获取奥运答题时间配置出错，没有存在比当前时间还晚的活动", new object[0]), null, true);
		}

		private void SetCurrentQuestionList(DateTime time)
		{
			int num = 0;
			int num2 = 0;
			try
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				}
				AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTimeItem != null)
				{
					int questionNum = currentQuestionTimeItem.QuestionNum;
					num = currentQuestionTimeItem.QuestionBegin;
					num2 = currentQuestionTimeItem.QuestionEnd;
					if (AoYunDaTiManager.XmlQuestionBankDic.Count == 0)
					{
						LogManager.WriteLog(2, string.Format("获取奥运答题题目列表出错，问题词典不存在", new object[0]), null, true);
					}
					else
					{
						List<int> list = new List<int>();
						if (num2 - num + 1 < questionNum || AoYunDaTiManager.XmlQuestionBankDic.Count < questionNum)
						{
							for (int i = 0; i < questionNum; i++)
							{
								int randomNumber = Global.GetRandomNumber(num, num2);
								AoyunQuestionBankItem aoyunQuestionBankItem;
								if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(randomNumber, out aoyunQuestionBankItem))
								{
									list.Add(randomNumber);
								}
								else
								{
									i--;
								}
							}
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList = list;
							}
						}
						else
						{
							int num3 = num;
							if (currentQuestionTimeItem.RandomType == 1)
							{
								for (int i = 0; i < questionNum; i++)
								{
									if (AoYunDaTiManager.XmlQuestionBankDic.ContainsKey(num3))
									{
										list.Add(num3);
									}
									else
									{
										i--;
									}
									num3++;
									if (num3 > num + questionNum * 5)
									{
										LogManager.WriteLog(2, string.Format("获取奥运答题题目列表出错", new object[0]), null, true);
										return;
									}
								}
							}
							else
							{
								List<int> list2 = new List<int>();
								for (int i = num3; i <= num2; i++)
								{
									if (AoYunDaTiManager.XmlQuestionBankDic.ContainsKey(i))
									{
										list2.Add(i);
									}
								}
								int count = list2.Count;
								for (int i = 0; i < questionNum; i++)
								{
									int randomNumber2 = Global.GetRandomNumber(i, count);
									int value = list2[i];
									list2[i] = list2[randomNumber2];
									list2[randomNumber2] = value;
									list.Add(list2[i]);
								}
							}
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList = list;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("出题出错，开始题：{0} 结束题：{1}", num, num2), null, true);
			}
		}

		private void SetCurrentQuestionNum(DateTime now)
		{
			AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
			if (currentQuestionTimeItem == null)
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
				}
			}
			else
			{
				if (now > currentQuestionTimeItem.EndTime)
				{
					LogManager.WriteLog(2, string.Format("获取奥运答题题目信息出错，结束时间：{0} 当前时间：{1}", currentQuestionTimeItem.EndTime, now), null, true);
				}
				if (now < currentQuestionTimeItem.BeginTime)
				{
					lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
					{
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
						AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
					}
				}
				else
				{
					double num = (now - currentQuestionTimeItem.BeginTime).TotalSeconds;
					for (int i = 0; i < AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count; i++)
					{
						int key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList[i];
						AoyunQuestionBankItem aoyunQuestionBankItem;
						if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(key, out aoyunQuestionBankItem))
						{
							if (num <= (double)(aoyunQuestionBankItem.QuestionAwardXml.ExamTime + aoyunQuestionBankItem.QuestionAwardXml.WaitTime))
							{
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = i;
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = now;
								}
								return;
							}
							num -= (double)(aoyunQuestionBankItem.QuestionAwardXml.ExamTime + aoyunQuestionBankItem.QuestionAwardXml.WaitTime);
						}
					}
					LogManager.WriteLog(2, string.Format("获取奥运答题题目信息出错，问题答完了，开始时间：{0} 当前时间：{1}", currentQuestionTimeItem.BeginTime, now), null, true);
				}
			}
		}

		private static void LoadQuestionTime()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(AoyunDaTiConsts.QuestionTime);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					AoYunDaTiManager.XmlQuestionTimeList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							AoyunQuestionTimeItem aoyunQuestionTimeItem = new AoyunQuestionTimeItem();
							string s = Global.GetDefAttributeStr(xelement2, "Date", "") + " " + Global.GetDefAttributeStr(xelement2, "BeginTime", "");
							aoyunQuestionTimeItem.BeginTime = DateTime.Parse(s);
							string s2 = Global.GetDefAttributeStr(xelement2, "Date", "") + " " + Global.GetDefAttributeStr(xelement2, "EndTime", "");
							aoyunQuestionTimeItem.EndTime = DateTime.Parse(s2);
							aoyunQuestionTimeItem.RandomType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RandomType", "0"));
							aoyunQuestionTimeItem.QuestionNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0"));
							aoyunQuestionTimeItem.QuestionBegin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "QuestionBegin", "0"));
							aoyunQuestionTimeItem.QuestionEnd = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "QuestionEnd", "0"));
							AoYunDaTiManager.XmlQuestionTimeList.Add(aoyunQuestionTimeItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		private static void LoadQuestionBank()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(AoyunDaTiConsts.QuestionBank);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					AoYunDaTiManager.XmlQuestionBankDic.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							AoyunQuestionAwardXml aoyunQuestionAwardXml = new AoyunQuestionAwardXml();
							aoyunQuestionAwardXml.TrueAnswer = 0;
							aoyunQuestionAwardXml.JinBi = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "JinBi", "0"));
							aoyunQuestionAwardXml.Exp = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Exp", "0"));
							aoyunQuestionAwardXml.TianShiItem = Array.ConvertAll<string, double>(Global.GetDefAttributeStr(xelement2, "TianShiItem", "").Split(new char[]
							{
								'|'
							}), (string s) => double.Parse(s));
							aoyunQuestionAwardXml.EMoItem = Array.ConvertAll<string, double>(Global.GetDefAttributeStr(xelement2, "TianShiItem", "").Split(new char[]
							{
								'|'
							}), (string s) => double.Parse(s));
							aoyunQuestionAwardXml.ExamTime = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ExamTime", "0"));
							aoyunQuestionAwardXml.WaitTime = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "WaitTime", "0"));
							aoyunQuestionAwardXml.WinScore = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "WinScore", "0"));
							aoyunQuestionAwardXml.LoseScore = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LoseScore", "0"));
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							AoyunQuestionItemData aoyunQuestionItemData = new AoyunQuestionItemData();
							aoyunQuestionItemData.Question = Global.GetDefAttributeStr(xelement2, "Questiong", "");
							string[] array = new string[]
							{
								Global.GetDefAttributeStr(xelement2, "Ture", ""),
								Global.GetDefAttributeStr(xelement2, "False1", ""),
								Global.GetDefAttributeStr(xelement2, "False2", ""),
								Global.GetDefAttributeStr(xelement2, "False3", "")
							};
							int[] array2 = new int[]
							{
								0,
								1,
								2,
								3
							};
							aoyunQuestionItemData.AnswerContentArray = new string[4];
							for (int i = 0; i < 4; i++)
							{
								int randomNumber = Global.GetRandomNumber(i, 4);
								int num = array2[i];
								array2[i] = array2[randomNumber];
								array2[randomNumber] = num;
							}
							aoyunQuestionAwardXml.AnswerIndex = array2;
							aoyunQuestionAwardXml.TrueAnswer = aoyunQuestionAwardXml.AnswerIndex[0];
							aoyunQuestionItemData.AnswerContentArray[aoyunQuestionAwardXml.AnswerIndex[0]] = array[0];
							aoyunQuestionItemData.AnswerContentArray[aoyunQuestionAwardXml.AnswerIndex[1]] = array[1];
							aoyunQuestionItemData.AnswerContentArray[aoyunQuestionAwardXml.AnswerIndex[2]] = array[2];
							aoyunQuestionItemData.AnswerContentArray[aoyunQuestionAwardXml.AnswerIndex[3]] = array[3];
							aoyunQuestionItemData.UseTianShi = false;
							aoyunQuestionItemData.UseEMo = false;
							aoyunQuestionItemData.EndTime = DateTime.MinValue;
							AoyunQuestionBankItem value = new AoyunQuestionBankItem
							{
								QuestionItem = aoyunQuestionItemData,
								QuestionAwardXml = aoyunQuestionAwardXml
							};
							AoYunDaTiManager.XmlQuestionBankDic.Add(key, value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		private static void LoadPaiHangAwad()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(AoyunDaTiConsts.QuestionAward);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					AoYunDaTiManager.XmlPaiHangAward.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							AoyunDaTiPaiHangAwardXml item = new AoyunDaTiPaiHangAwardXml
							{
								Name = Global.GetDefAttributeStr(xelement2, "Name", ""),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EngNum", "0")),
								GoodsOne = Global.GetDefAttributeStr(xelement2, "GoodsOne", ""),
								MinScore = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinScore", "0"))
							};
							AoYunDaTiManager.XmlPaiHangAward.Add(item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		private static void LoadAoYunDaTi()
		{
			try
			{
				if (AoYunDaTiManager.XmlQuestionTimeList == null || AoYunDaTiManager.XmlQuestionTimeList.Count <= 0)
				{
					AoYunDaTiManager.XmlAoyunDaTiOpen = new AoyunDaTiOpenXml
					{
						HuoDongID = 1,
						BeginTime = DateTime.MaxValue,
						EndTime = DateTime.MinValue
					};
				}
				else
				{
					AoYunDaTiManager.XmlAoyunDaTiOpen = new AoyunDaTiOpenXml
					{
						HuoDongID = 1,
						BeginTime = AoYunDaTiManager.XmlQuestionTimeList[0].BeginTime.Date,
						EndTime = AoYunDaTiManager.XmlQuestionTimeList[AoYunDaTiManager.XmlQuestionTimeList.Count - 1].EndTime.Date.AddDays(1.0)
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("活动开启配置信息, 失败。", new object[0]), ex, true);
			}
		}

		private static void UpdateQuestion(DateTime now)
		{
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
			}
			int num = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum;
			num++;
			if (num <= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count)
			{
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = num;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = now;
				}
			}
		}

		private static void SendQuestionToAll(DateTime now)
		{
			AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
			if (currentQuestionBank != null)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
				{
					AoyunQuestionItemData roleQuestionFromDB = AoYunDaTiManager.GetRoleQuestionFromDB(nextClient, now);
					if (roleQuestionFromDB != null)
					{
						nextClient.sendCmd<AoyunQuestionItemData>(20201, roleQuestionFromDB, false);
					}
				}
			}
		}

		private static void SendAnswerOverToAll()
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
			{
				string cmdData = "0";
				int num2 = 0;
				if (AoYunDaTiManager.AoyunRunTimeData.LastRankDic.TryGetValue(nextClient.ClientData.RoleID, out num2))
				{
					if (num2 > 0)
					{
						cmdData = "1";
					}
				}
				nextClient.sendCmd(20207, cmdData, false);
			}
		}

		private static void SendAnswerToAll(DateTime now)
		{
			try
			{
				AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
				if (currentQuestionBank != null)
				{
					AoyunQuestionAwardXml questionAwardXml = currentQuestionBank.QuestionAwardXml;
					AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
					if (currentQuestionTimeItem != null)
					{
						int num = 0;
						GameClient nextClient;
						while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
						{
							int roleID = nextClient.ClientData.RoleID;
							int num2 = -1;
							int num3 = Global.GetRoleParamsInt32FromDB(nextClient, "20002");
							int num4 = Global.GetRoleParamsInt32FromDB(nextClient, "20004");
							int num5 = 0;
							DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(nextClient, "20006");
							if (roleParamsDateTimeFromDB > currentQuestionTimeItem.BeginTime)
							{
								num5 = Global.GetRoleParamsInt32FromDB(nextClient, "20008");
							}
							if (AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.TryGetValue(roleID, out num2))
							{
								AoyunQuestionAward aoyunQuestionAward = new AoyunQuestionAward();
								int num6 = questionAwardXml.JinBi;
								int num7 = questionAwardXml.Exp;
								aoyunQuestionAward.RightAnswer = questionAwardXml.TrueAnswer;
								int changeLifeCount = nextClient.ClientData.ChangeLifeCount;
								if (AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef.Length > changeLifeCount)
								{
									num7 *= AoYunDaTiManager.AoyunRunTimeData.ZhuanShengExpCoef[changeLifeCount];
								}
								int num8;
								if (questionAwardXml.TrueAnswer != num2)
								{
									aoyunQuestionAward.Result = -1;
									num8 = questionAwardXml.LoseScore;
									num6 /= 2;
									num7 /= 2;
									aoyunQuestionAward.RightAnswer = questionAwardXml.TrueAnswer;
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.TianShiItem[1])
									{
										if (num3 + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0])
										{
											num3++;
											Global.SaveRoleParamsInt32ValueToDB(nextClient, "20002", num3, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, StringUtil.substitute(GLang.GetLang(7, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20002", nextClient.ClientData.RoleName, "系统", "修改", 1, nextClient.ClientData.ZoneID, nextClient.strUserID, num3, nextClient.ServerId, null);
										}
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.EMoItem[1])
									{
										if (num4 + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1])
										{
											num4++;
											Global.SaveRoleParamsInt32ValueToDB(nextClient, "20004", num4, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, StringUtil.substitute(GLang.GetLang(8, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20004", nextClient.ClientData.RoleName, "系统", "修改", 1, nextClient.ClientData.ZoneID, nextClient.strUserID, num4, nextClient.ServerId, null);
										}
									}
								}
								else
								{
									aoyunQuestionAward.Result = 1;
									num8 = questionAwardXml.WinScore;
									if (roleParamsDateTimeFromDB > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
									{
										num8 -= (roleParamsDateTimeFromDB - AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime).Seconds;
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.TianShiItem[0])
									{
										if (num3 + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[0])
										{
											num3++;
											Global.SaveRoleParamsInt32ValueToDB(nextClient, "20002", num3, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, StringUtil.substitute(GLang.GetLang(7, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20002", nextClient.ClientData.RoleName, "系统", "修改", 1, nextClient.ClientData.ZoneID, nextClient.strUserID, num3, nextClient.ServerId, null);
										}
									}
									if ((double)Global.GetRandomNumber(1, 100) / 100.0 < questionAwardXml.EMoItem[0])
									{
										if (num4 + 1 <= AoYunDaTiManager.AoyunRunTimeData.GoodsLimit[1])
										{
											num4++;
											Global.SaveRoleParamsInt32ValueToDB(nextClient, "20004", num4, true);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, StringUtil.substitute(GLang.GetLang(8, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "答题道具奖励", "20004", nextClient.ClientData.RoleName, "系统", "修改", 1, nextClient.ClientData.ZoneID, nextClient.strUserID, num4, nextClient.ServerId, null);
										}
									}
									List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(nextClient, "20007");
									roleParamsIntListFromDB[AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum] = 1;
									Global.SaveRoleParamsIntListToDB(nextClient, roleParamsIntListFromDB, "20007", true);
								}
								DateTime roleParamsDateTimeFromDB2 = Global.GetRoleParamsDateTimeFromDB(nextClient, "20003");
								if (roleParamsDateTimeFromDB2 > AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime)
								{
									num8 *= 2;
								}
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, nextClient, num6, "奥运答题添加", false);
								GameManager.ClientMgr.ProcessRoleExperience(nextClient, (long)num7, false, true, false, "none");
								num5 += num8;
								AoyunPaiHangRoleData aoyunPaiHangRoleData = null;
								if (!AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.TryGetValue(roleID, out aoyunPaiHangRoleData))
								{
									aoyunPaiHangRoleData = new AoyunPaiHangRoleData
									{
										ZoneId = nextClient.ClientData.ZoneID,
										RoleId = roleID,
										RoleName = nextClient.ClientData.RoleName,
										RoleCurrentPoint = 0,
										RoleRank = 0
									};
								}
								aoyunPaiHangRoleData.RolePoint = num5;
								Global.SaveRoleParamsInt32ValueToDB(nextClient, "20008", num5, true);
								if (num8 > 0)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, string.Format(GLang.GetLang(666, new object[0]), num8), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								aoyunQuestionAward.TianShiCount = num3;
								aoyunQuestionAward.EMoCount = num4;
								aoyunQuestionAward.RolePoint = num5;
								nextClient.sendCmd<AoyunQuestionAward>(20206, aoyunQuestionAward, false);
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic[roleID] = aoyunPaiHangRoleData;
								}
							}
							else
							{
								AoyunQuestionAward aoyunQuestionAward = new AoyunQuestionAward
								{
									Result = -1,
									RightAnswer = questionAwardXml.TrueAnswer,
									TianShiCount = num3,
									EMoCount = num4,
									RolePoint = num5
								};
								Global.SaveRoleParamsInt32ValueToDB(nextClient, "20008", num5, true);
								nextClient.sendCmd<AoyunQuestionAward>(20206, aoyunQuestionAward, false);
							}
						}
						lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
						{
							AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Values.ToList<AoyunPaiHangRoleData>();
						}
						AoYunDaTiManager.UpdateRankList();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载发送问题答案给所有在线用户 失败:", new object[0]), ex, true);
			}
		}

		private static void InitAoyunRank()
		{
			List<AoyunPaiHangRoleData> aoyunRoleListFromDB = AoYunDaTiManager.GetAoyunRoleListFromDB();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = aoyunRoleListFromDB;
			}
			AoYunDaTiManager.UpdateRankList();
		}

		private static void InitLastAoyunRank()
		{
			Dictionary<int, int> lastAoyunRoleDicFromDB = AoYunDaTiManager.GetLastAoyunRoleDicFromDB();
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.LastRankDic = lastAoyunRoleDicFromDB;
			}
		}

		private void UpdateCurrentQuestionTimeItem(DateTime time)
		{
			try
			{
				for (int i = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1; i < AoYunDaTiManager.XmlQuestionTimeList.Count; i++)
				{
					DateTime endTime = AoYunDaTiManager.XmlQuestionTimeList[i].EndTime;
					if (!(time >= endTime))
					{
						lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
						{
							AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = i;
						}
						this.SetCurrentQuestionList(time);
						this.SetCurrentQuestionNum(time);
						return;
					}
				}
				LogManager.WriteLog(0, string.Format("活动结束了", new object[0]), null, true);
				lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
				{
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = AoYunDaTiManager.XmlQuestionTimeList.Count;
					AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("奥运答题更新当前答题时间设置失败：", new object[0]), ex, true);
			}
		}

		private static AoyunQuestionTimeItem GetCurrentQuestionTimeItem()
		{
			int currentQuestionTimeKey = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey;
			AoyunQuestionTimeItem result;
			if (currentQuestionTimeKey < 0 || currentQuestionTimeKey >= AoYunDaTiManager.XmlQuestionTimeList.Count)
			{
				result = null;
			}
			else
			{
				result = AoYunDaTiManager.XmlQuestionTimeList[currentQuestionTimeKey];
			}
			return result;
		}

		private static AoyunQuestionBankItem GetCurrentQuestionBank()
		{
			int currentQuestionNum = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum;
			AoyunQuestionBankItem result;
			if (currentQuestionNum < 0 || currentQuestionNum >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count)
			{
				result = null;
			}
			else
			{
				int key = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList[currentQuestionNum];
				AoyunQuestionBankItem aoyunQuestionBankItem;
				if (AoYunDaTiManager.XmlQuestionBankDic.TryGetValue(key, out aoyunQuestionBankItem))
				{
					result = aoyunQuestionBankItem;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static AoyunQuestionItemData GetRoleQuestionFromDB(GameClient client, DateTime now)
		{
			AoyunQuestionItemData result;
			try
			{
				AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTimeItem == null)
				{
					result = null;
				}
				else
				{
					AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
					if (currentQuestionBank == null)
					{
						result = null;
					}
					else
					{
						AoyunQuestionItemData questionItem = currentQuestionBank.QuestionItem;
						DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20001");
						questionItem.UseTianShi = (roleParamsDateTimeFromDB >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime);
						DateTime roleParamsDateTimeFromDB2 = Global.GetRoleParamsDateTimeFromDB(client, "20003");
						questionItem.UseEMo = (roleParamsDateTimeFromDB2 >= AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime);
						questionItem.QuestionId = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum + 1;
						questionItem.EndTime = AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime.AddSeconds((double)currentQuestionBank.QuestionAwardXml.ExamTime);
						DateTime roleParamsDateTimeFromDB3 = Global.GetRoleParamsDateTimeFromDB(client, "20006");
						if (roleParamsDateTimeFromDB3 < currentQuestionTimeItem.BeginTime)
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "20008", 0, true);
						}
						questionItem.RolePoint = Global.GetRoleParamsInt32FromDB(client, "20008");
						if (!AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.TryGetValue(client.ClientData.RoleID, out questionItem.RoleAnswer))
						{
							questionItem.RoleAnswer = -1;
						}
						List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "20007");
						if (roleParamsIntListFromDB.Count != currentQuestionTimeItem.QuestionNum)
						{
							roleParamsIntListFromDB.Clear();
							for (int i = 0; i < currentQuestionTimeItem.QuestionNum; i++)
							{
								roleParamsIntListFromDB.Add(0);
							}
							Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "20007", true);
						}
						else if (currentQuestionTimeItem.BeginTime > Global.GetRoleParamsDateTimeFromDB(client, "20006"))
						{
							roleParamsIntListFromDB.Clear();
							for (int i = 0; i < currentQuestionTimeItem.QuestionNum; i++)
							{
								roleParamsIntListFromDB.Add(0);
							}
							Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "20007", true);
						}
						questionItem.QuestionState = new List<bool>();
						foreach (int i in roleParamsIntListFromDB)
						{
							int i;
							questionItem.QuestionState.Add(i == 1);
						}
						result = questionItem;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("奥运答题获取角色答题信息失败：", new object[0]), ex, true);
				result = null;
			}
			return result;
		}

		private static void UpdateRankList()
		{
			try
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
				}
				List<AoyunPaiHangRoleData> aoyunRankList = AoYunDaTiManager.AoyunRunTimeData.AoyunRankList;
				if (aoyunRankList != null && aoyunRankList.Count >= 1)
				{
					aoyunRankList.Sort((AoyunPaiHangRoleData x, AoyunPaiHangRoleData y) => y.RolePoint - x.RolePoint);
					Dictionary<int, AoyunPaiHangRoleData> dictionary = new Dictionary<int, AoyunPaiHangRoleData>();
					int num = 1;
					int num2 = 0;
					int num3 = aoyunRankList.Count<AoyunPaiHangRoleData>();
					for (int i = 0; i < num3; i++)
					{
						AoyunPaiHangRoleData aoyunPaiHangRoleData = aoyunRankList[i];
						if (aoyunPaiHangRoleData.RolePoint > 0)
						{
							AoyunDaTiPaiHangAwardXml aoyunDaTiPaiHangAwardXml = AoYunDaTiManager.XmlPaiHangAward[num2];
							while (aoyunDaTiPaiHangAwardXml.MinScore > aoyunPaiHangRoleData.RolePoint && num2 < AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								aoyunDaTiPaiHangAwardXml = AoYunDaTiManager.XmlPaiHangAward[++num2];
								num = aoyunDaTiPaiHangAwardXml.BeginNum;
							}
							if (aoyunDaTiPaiHangAwardXml.MinScore > aoyunPaiHangRoleData.RolePoint && num2 == AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								aoyunRankList[i].RoleRank = -1;
							}
							else
							{
								aoyunRankList[i].RoleRank = num++;
							}
							dictionary.Add(aoyunRankList[i].RoleId, aoyunRankList[i]);
							if (num == aoyunDaTiPaiHangAwardXml.EndNum + 1 && num2 < AoYunDaTiManager.XmlPaiHangAward.Count - 1)
							{
								aoyunDaTiPaiHangAwardXml = AoYunDaTiManager.XmlPaiHangAward[++num2];
							}
						}
					}
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic = dictionary;
						AoYunDaTiManager.AoyunRunTimeData.AoyunRankList = aoyunRankList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("奥运答题获取排行榜失败：", new object[0]), ex, true);
			}
		}

		private static List<AoyunPaiHangRoleData> GetAoyunRoleListFromDB()
		{
			List<AoyunPaiHangRoleData> list = Global.sendToDB<List<AoyunPaiHangRoleData>, int>(20300, 0, 0);
			if (null == list)
			{
				list = new List<AoyunPaiHangRoleData>();
			}
			return list;
		}

		private static Dictionary<int, int> GetLastAoyunRoleDicFromDB()
		{
			Dictionary<int, int> dictionary = Global.sendToDB<Dictionary<int, int>, int>(20301, 0, 0);
			if (null == dictionary)
			{
				dictionary = new Dictionary<int, int>();
			}
			return dictionary;
		}

		private static void GiveGoodsAward(GameClient client, string goods)
		{
			string[] array = goods.Split(new char[]
			{
				'|'
			});
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] == ""))
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						GoodsData goodsData = new GoodsData
						{
							Id = -1,
							GoodsID = Convert.ToInt32(array2[0]),
							Using = 0,
							Forge_level = Convert.ToInt32(array2[3]),
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							GCount = Convert.ToInt32(array2[1]),
							Binding = Convert.ToInt32(array2[2]),
							BagIndex = 0,
							Lucky = Convert.ToInt32(array2[5]),
							ExcellenceInfo = Convert.ToInt32(array2[6]),
							AppendPropLev = Convert.ToInt32(array2[4])
						};
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
						{
							string textMsg = string.Format("系统中不存在{0}", goodsData.GoodsID);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
						}
						else
						{
							list.Add(goodsData);
						}
					}
				}
			}
			if (!Global.CanAddGoodsNum(client, array.Length))
			{
				Global.UseMailGivePlayerAward2(client, list, GLang.GetLang(10, new object[0]), GLang.GetLang(10, new object[0]), 0, 0, 0);
			}
			else
			{
				foreach (GoodsData goodsData2 in list)
				{
					SystemXmlItem systemXmlItem2 = null;
					GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData2.GoodsID, out systemXmlItem2);
					string stringValue = systemXmlItem2.GetStringValue("Title");
					LogManager.WriteLog(3, string.Format("奥运答题奖励{0} {1}", client.ClientData.RoleID, stringValue), null, true);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", true, 1, "奥运答题奖励", "1900-01-01 12:00:00", 0, 0, goodsData2.Lucky, 0, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, 0, null, null, 0, true);
				}
			}
		}

		private static bool CheckAoYunDaTiOpen(DateTime now)
		{
			bool result;
			if (AoYunDaTiManager.XmlAoyunDaTiOpen == null)
			{
				result = false;
			}
			else if (!AoYunDaTiManager.AoyunRunTimeData.GongNengOpen && now >= AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime && now <= AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime)
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.GongNengOpen = true;
				}
				GameManager.ClientMgr.NotifyAllActivityState(7, 1, AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.HuoDongID);
				result = true;
			}
			else if (AoYunDaTiManager.AoyunRunTimeData.GongNengOpen && now >= AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime)
			{
				lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
				{
					AoYunDaTiManager.AoyunRunTimeData.GongNengOpen = false;
				}
				GameManager.ClientMgr.NotifyAllActivityState(7, 0, AoYunDaTiManager.XmlAoyunDaTiOpen.BeginTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.EndTime.ToString("yyyyMMddHHmmss"), AoYunDaTiManager.XmlAoyunDaTiOpen.HuoDongID);
				result = false;
			}
			else
			{
				result = AoYunDaTiManager.AoyunRunTimeData.GongNengOpen;
			}
			return result;
		}

		public void NotifyActivityState(GameClient client)
		{
			if (AoYunDaTiManager.AoyunRunTimeData.GongNengOpen)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					7,
					1,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					7,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			client._IconStateMgr.AddFlushIconState(18000, AoYunDaTiManager.AoyunRunTimeData.DaTiOpen);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		private static void CheckActivityIcon(bool show)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
			{
				nextClient._IconStateMgr.AddFlushIconState(18000, show);
				nextClient._IconStateMgr.SendIconStateToClient(nextClient);
			}
		}

		public void AddGrade(GameClient client, int grade)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "20008", grade, true);
		}

		public void AoyunDaTiTimer_Work()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			if (AoYunDaTiManager.CheckAoYunDaTiOpen(dateTime))
			{
				AoyunQuestionTimeItem currentQuestionTimeItem = AoYunDaTiManager.GetCurrentQuestionTimeItem();
				if (currentQuestionTimeItem != null)
				{
					lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
					{
						if (!AoYunDaTiManager.AoyunRunTimeData.DaTiOpen)
						{
							if (dateTime > currentQuestionTimeItem.BeginTime)
							{
								AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Clear();
								AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
								AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
								Global.sendToDB<int, int>(20303, 0, 0);
								AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = true;
								AoYunDaTiManager.CheckActivityIcon(true);
								AoYunDaTiManager.UpdateQuestion(dateTime);
								AoYunDaTiManager.SendQuestionToAll(dateTime);
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.SendAnswer = true;
								}
							}
							return;
						}
					}
					AoyunQuestionBankItem currentQuestionBank = AoYunDaTiManager.GetCurrentQuestionBank();
					if (currentQuestionBank != null)
					{
						double totalSeconds = (dateTime - AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime).TotalSeconds;
						if (totalSeconds > (double)(currentQuestionBank.QuestionAwardXml.ExamTime + currentQuestionBank.QuestionAwardXml.WaitTime))
						{
							if (AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum == AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Count - 1)
							{
								EventLogManager.AddGameEvent(LogRecordType.AoYunDaTi, new object[]
								{
									AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey + 1,
									currentQuestionTimeItem.BeginTime,
									currentQuestionTimeItem.EndTime,
									AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Count
								});
								Dictionary<int, int> dictionary = new Dictionary<int, int>();
								foreach (AoyunPaiHangRoleData aoyunPaiHangRoleData in AoYunDaTiManager.AoyunRunTimeData.AoyunRankList)
								{
									dictionary[aoyunPaiHangRoleData.RoleId] = aoyunPaiHangRoleData.RoleRank;
								}
								Global.sendToDB<int, Dictionary<int, int>>(20302, dictionary, 0);
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.LastRankDic = dictionary;
								}
								AoYunDaTiManager.SendAnswerOverToAll();
								lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
								{
									AoYunDaTiManager.AoyunRunTimeData.DaTiOpen = false;
								}
								AoYunDaTiManager.CheckActivityIcon(false);
								this.UpdateCurrentQuestionTimeItem(dateTime);
							}
							else
							{
								AoYunDaTiManager.UpdateQuestion(dateTime);
								AoYunDaTiManager.SendQuestionToAll(dateTime);
								lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
								{
									AoYunDaTiManager.QuestionDataTimer.SendAnswer = true;
								}
							}
						}
						else if (AoYunDaTiManager.QuestionDataTimer.SendAnswer && totalSeconds > (double)currentQuestionBank.QuestionAwardXml.ExamTime)
						{
							lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
							{
								AoYunDaTiManager.QuestionDataTimer.SendAnswer = false;
							}
							AoYunDaTiManager.SendAnswerToAll(dateTime);
						}
					}
				}
			}
		}

		private void Destory_Work()
		{
			lock (AoYunDaTiManager.QuestionDataTimer.Mutex)
			{
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBankKeyList.Clear();
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionNum = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionTimeKey = -1;
				AoYunDaTiManager.QuestionDataTimer.CurrentQuestionBeginTime = DateTime.MaxValue;
			}
			lock (AoYunDaTiManager.AoyunRunTimeData.Mutex)
			{
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankList.Clear();
				AoYunDaTiManager.AoyunRunTimeData.AoyunRankRoleDataDic.Clear();
				AoYunDaTiManager.AoyunRunTimeData.AoyunRoleAnswerDic.Clear();
			}
		}

		public static AoyunData AoyunRunTimeData = new AoyunData();

		private static QuestionData QuestionDataTimer = new QuestionData();

		private static AoYunDaTiManager instance = new AoYunDaTiManager();

		private static List<AoyunQuestionTimeItem> XmlQuestionTimeList = new List<AoyunQuestionTimeItem>();

		private static Dictionary<int, AoyunQuestionBankItem> XmlQuestionBankDic = new Dictionary<int, AoyunQuestionBankItem>();

		private static List<AoyunDaTiPaiHangAwardXml> XmlPaiHangAward = new List<AoyunDaTiPaiHangAwardXml>();

		private static AoyunDaTiOpenXml XmlAoyunDaTiOpen = null;
	}
}
