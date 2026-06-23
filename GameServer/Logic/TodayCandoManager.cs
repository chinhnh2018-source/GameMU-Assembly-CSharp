using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.WanMota;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class TodayCandoManager
	{
		public static XElement xmlData
		{
			get
			{
				lock (TodayCandoManager._xmlDataMutex)
				{
					if (TodayCandoManager._xmlData != null)
					{
						return TodayCandoManager._xmlData;
					}
				}
				XElement xmlData = null;
				try
				{
					string uri = "Config/JinRiKeZuo.xml";
					xmlData = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(uri));
				}
				catch (Exception ex)
				{
					xmlData = null;
					LogManager.WriteException(ex.ToString());
				}
				lock (TodayCandoManager._xmlDataMutex)
				{
					TodayCandoManager._xmlData = xmlData;
				}
				return TodayCandoManager._xmlData;
			}
		}

		private static int GetLeftCountByType(GameClient client, int type, int copyId)
		{
			int num = 0;
			switch (type)
			{
			case 1:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 8);
				if (null == dailyTaskData)
				{
					return 10;
				}
				int maxDailyTaskNum = Global.GetMaxDailyTaskNum(client, 8, dailyTaskData);
				num = maxDailyTaskNum - dailyTaskData.RecNum;
				goto IL_4AC;
			}
			case 5:
			{
				int key = Global.GetDaimonSquareCopySceneIDForRole(client);
				DaimonSquareDataInfo daimonSquareDataInfo = null;
				Data.DaimonSquareDataInfoList.TryGetValue(key, out daimonSquareDataInfo);
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, 2);
				if (num2 < 0)
				{
					num2 = 0;
				}
				int vipLevel = client.ClientData.VipLevel;
				int num3 = 0;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterDaimonSquareCountAddValue", ',');
				if (vipLevel > 0 && paramValueIntArrayByName != null && paramValueIntArrayByName[vipLevel] > 0)
				{
					num3 = paramValueIntArrayByName[vipLevel];
				}
				num = daimonSquareDataInfo.MaxEnterNum + num3 - num2;
				goto IL_4AC;
			}
			case 6:
			{
				int key = Global.GetBloodCastleCopySceneIDForRole(client);
				BloodCastleDataInfo bloodCastleDataInfo = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(key, out bloodCastleDataInfo))
				{
					goto IL_4AC;
				}
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				int nType = 1;
				int num2 = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, nType);
				if (num2 < 0)
				{
					num2 = 0;
				}
				int vipLevel = client.ClientData.VipLevel;
				int num3 = 0;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterBloodCastleCountAddValue", ',');
				if (vipLevel > 0 && paramValueIntArrayByName != null && paramValueIntArrayByName[vipLevel] > 0)
				{
					num3 = paramValueIntArrayByName[vipLevel];
				}
				num = bloodCastleDataInfo.MaxEnterNum + num3 - num2;
				goto IL_4AC;
			}
			case 7:
			{
				DateTime t = TimeUtil.NowDateTime();
				string text = TimeUtil.NowDateTime().ToString("HH:mm");
				List<string> beginTime = GameManager.AngelTempleMgr.m_AngelTempleData.BeginTime;
				num = 0;
				for (int i = 0; i < beginTime.Count; i++)
				{
					DateTime t2 = DateTime.Parse(beginTime[i]).AddMinutes((double)(GameManager.AngelTempleMgr.m_AngelTempleData.PrepareTime / 60));
					if (t <= t2)
					{
						num++;
					}
				}
				goto IL_4AC;
			}
			case 8:
				num = 1;
				if (SweepWanMotaManager.GetSweepCount(client) >= SweepWanMotaManager.nWanMoTaMaxSweepNum)
				{
					num = 0;
				}
				goto IL_4AC;
			case 9:
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 34);
				num = (int)(bufferDataByID.BufferVal - (long)bufferDataByID.BufferSecs);
				goto IL_4AC;
			}
			case 10:
				num = GameManager.BattleMgr.LeftEnterCount();
				goto IL_4AC;
			case 11:
				num = GameManager.ArenaBattleMgr.LeftEnterCount();
				goto IL_4AC;
			case 13:
				num = JingJiChangManager.getInstance().GetLeftEnterCount(client);
				goto IL_4AC;
			case 15:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				if (null == dailyTaskData)
				{
					return Global.MaxTaofaTaskNumForMU;
				}
				int maxDailyTaskNum = Global.GetMaxDailyTaskNum(client, 9, dailyTaskData);
				num = maxDailyTaskNum - dailyTaskData.RecNum;
				goto IL_4AC;
			}
			case 16:
			{
				int num4 = 0;
				CaiJiLogic.ReqCaiJiLastNum(client, 0, out num4);
				num = num4;
				goto IL_4AC;
			}
			case 19:
				num = HuanYingSiYuanManager.getInstance().GetLeftCount(client);
				goto IL_4AC;
			}
			if (copyId > 0)
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out systemXmlItem))
				{
					return -1;
				}
				int intValue = systemXmlItem.GetIntValue("EnterNumber", -1);
				int intValue2 = systemXmlItem.GetIntValue("FinishNumber", -1);
				int num5 = (intValue < intValue2) ? intValue2 : intValue;
				if (type == 4 || type == 3)
				{
					int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiFuBenNum", ',');
					if (type == 3)
					{
						paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinYanFuBenNum", ',');
					}
					if (client.ClientData.VipLevel > 0 && client.ClientData.VipLevel <= VIPEumValue.VIPENUMVALUE_MAXLEVEL && paramValueIntArrayByName2 != null && paramValueIntArrayByName2.Length > VIPEumValue.VIPENUMVALUE_MAXLEVEL)
					{
						num5 += paramValueIntArrayByName2[client.ClientData.VipLevel];
					}
				}
				FuBenData fuBenData = Global.GetFuBenData(client, copyId);
				if (null == fuBenData)
				{
					return num5;
				}
				num = num5 - fuBenData.EnterNum;
			}
			IL_4AC:
			return num;
		}

		private static bool TaskHasDone(GameClient client, int taskID)
		{
			return client.ClientData.MainTaskID >= taskID;
		}

		private static List<TodayCandoData> GetRoleCandoData(int typeId, GameClient client)
		{
			List<TodayCandoData> list = new List<TodayCandoData>();
			List<TodayCandoData> result;
			if (TodayCandoManager.xmlData == null)
			{
				result = null;
			}
			else
			{
				IEnumerable<XElement> enumerable = TodayCandoManager.xmlData.Elements();
				int num = -1;
				int num2 = -1;
				int num3 = 0;
				int num4 = -1;
				Dictionary<int, List<TodayCandoData>> dictionary = new Dictionary<int, List<TodayCandoData>>();
				foreach (XElement xelement in enumerable)
				{
					if (null != xelement)
					{
						int num5 = (int)Global.GetSafeAttributeLong(xelement, "Type");
						string[] array = Global.GetSafeAttributeStr(xelement, "MinLevel").Split(new char[]
						{
							','
						});
						string[] array2 = Global.GetSafeAttributeStr(xelement, "MaxLevel").Split(new char[]
						{
							','
						});
						int num6 = Global.SafeConvertToInt32(array[0]);
						int num7 = Global.SafeConvertToInt32(array[1]);
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "NeedRenWu");
						bool flag = false;
						if (Global.SafeConvertToInt32(array[0]) < client.ClientData.ChangeLifeCount && client.ClientData.ChangeLifeCount < Global.SafeConvertToInt32(array2[0]))
						{
							flag = true;
						}
						if (Global.SafeConvertToInt32(array[0]) == client.ClientData.ChangeLifeCount)
						{
							if (Global.SafeConvertToInt32(array[1]) <= client.ClientData.Level)
							{
								flag = true;
							}
						}
						if (Global.SafeConvertToInt32(array2[0]) == client.ClientData.ChangeLifeCount)
						{
							if (Global.SafeConvertToInt32(array2[1]) >= client.ClientData.Level)
							{
								flag = true;
							}
						}
						bool flag2;
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							flag2 = true;
						}
						else
						{
							int taskID = Global.SafeConvertToInt32(safeAttributeStr);
							flag2 = TodayCandoManager.TaskHasDone(client, taskID);
						}
						if (num5 == typeId && flag && flag2)
						{
							TodayCandoData todayCandoData = new TodayCandoData();
							todayCandoData.ID = (int)Global.GetSafeAttributeLong(xelement, "ID");
							int num8 = (int)Global.GetSafeAttributeLong(xelement, "SecondType");
							if (num3 == 0)
							{
								num4 = num8;
							}
							else if (num3 != 0 && num4 != num8)
							{
								num2 = num6;
								num = num7;
							}
							num3++;
							if (num2 < num6 && num4 == num8)
							{
								if (dictionary.ContainsKey(num8))
								{
									foreach (TodayCandoData item in dictionary[num8])
									{
										list.Remove(item);
									}
									dictionary[num8] = new List<TodayCandoData>();
								}
								num2 = num6;
								num = num7;
							}
							if (num2 == num6 && num < num7 && num4 == num8)
							{
								if (dictionary.ContainsKey(num8))
								{
									foreach (TodayCandoData item in dictionary[num8])
									{
										list.Remove(item);
									}
									dictionary[num8] = new List<TodayCandoData>();
								}
								num2 = num6;
								num = num7;
							}
							int copyId = (int)Global.GetSafeAttributeLong(xelement, "CodeID");
							int leftCountByType = TodayCandoManager.GetLeftCountByType(client, num8, copyId);
							if (leftCountByType > 0)
							{
								todayCandoData.LeftCount = leftCountByType;
								list.Add(todayCandoData);
								if (dictionary.ContainsKey(num8))
								{
									dictionary[num8].Add(todayCandoData);
								}
								else
								{
									dictionary[num8] = new List<TodayCandoData>();
									dictionary[num8].Add(todayCandoData);
								}
								num4 = num8;
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessQueryTodayCandoInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int typeId = Global.SafeConvertToInt32(array[1]);
				List<TodayCandoData> roleCandoData = TodayCandoManager.GetRoleCandoData(typeId, client);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<TodayCandoData>>(roleCandoData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "QueryTodayCandoInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static object _xmlDataMutex = new object();

		private static XElement _xmlData = null;
	}
}
