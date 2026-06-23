using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.Spread
{
	public class SpreadManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		public static SpreadManager getInstance()
		{
			return SpreadManager.instance;
		}

		public bool initialize()
		{
			SpreadManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1017, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1018, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1019, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1020, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1021, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1022, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10014, 10002, SpreadManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10014, 10002, SpreadManager.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1017:
				result = this.ProcessSpreadSignCmd(client, nID, bytes, cmdParams);
				break;
			case 1018:
				result = this.ProcessSpreadAwardCmd(client, nID, bytes, cmdParams);
				break;
			case 1019:
				result = this.ProcessSpreadVerifyCodeCmd(client, nID, bytes, cmdParams);
				break;
			case 1020:
				result = this.ProcessSpreadTelCodeGetCmd(client, nID, bytes, cmdParams);
				break;
			case 1021:
				result = this.ProcessSpreadTelCodeVerifyCmd(client, nID, bytes, cmdParams);
				break;
			case 1022:
				result = this.ProcessSpreadInfoCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool ProcessSpreadInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				SpreadData spreadInfo = this.GetSpreadInfo(client);
				client.sendCmd<SpreadData>(nID, spreadInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSpreadSignCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				string cmdData = this.SpreadSign(client);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSpreadAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int awardType = Convert.ToInt32(cmdParams[0]);
				SpreadData cmdData = this.SpreadAward(client, awardType);
				client.sendCmd<SpreadData>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSpreadVerifyCodeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string verifyCode = cmdParams[0];
				client.sendCmd(nID, this.SpreadVerifyCode(client, verifyCode).ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSpreadTelCodeGetCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string tel = cmdParams[0];
				client.sendCmd(nID, this.SpreadTelCodeGet(client, tel).ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSpreadTelCodeVerifyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string telCode = cmdParams[0];
				client.sendCmd(nID, this.SpreadTelCodeVerify(client, telCode).ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10014)
			{
				KFNotifySpreadCountGameEvent kfnotifySpreadCountGameEvent = eventObject as KFNotifySpreadCountGameEvent;
				if (null != kfnotifySpreadCountGameEvent)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kfnotifySpreadCountGameEvent.PRoleID);
					if (null != gameClient)
					{
						SpreadManager.initRoleSpreadData(gameClient);
					}
					eventObject.Handled = true;
				}
			}
		}

		private SpreadData GetSpreadInfo(GameClient client)
		{
			SpreadData mySpreadData2;
			lock (client.ClientData.LockSpread)
			{
				SpreadData mySpreadData = client.ClientData.MySpreadData;
				if (mySpreadData != null)
				{
					mySpreadData.State = 0;
				}
				if (SpreadManager.IsSpreadOpen() != mySpreadData.IsOpen)
				{
					SpreadManager.initRoleSpreadData(client);
				}
				mySpreadData2 = client.ClientData.MySpreadData;
			}
			return mySpreadData2;
		}

		private string SpreadSign(GameClient client)
		{
			string result;
			lock (client.ClientData.LockSpread)
			{
				string format = "{0}:{1}";
				SpreadData spreadInfo = this.GetSpreadInfo(client);
				if (!spreadInfo.IsOpen)
				{
					result = string.Format(format, -2, "");
				}
				else if (!string.IsNullOrEmpty(spreadInfo.SpreadCode))
				{
					result = string.Format(format, -21, "");
				}
				else
				{
					string spreadCode = this.GetSpreadCode(client);
					if (!SpreadManager.HSpreadSign(client))
					{
						result = string.Format(format, -21, "");
					}
					else
					{
						spreadInfo.SpreadCode = spreadCode;
						Global.UpdateRoleParamByName(client, "SpreadCode", spreadCode, true);
						result = string.Format(format, 1, spreadCode);
					}
				}
			}
			return result;
		}

		private SpreadData SpreadAward(GameClient client, int awardType)
		{
			SpreadData result;
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadInfo = this.GetSpreadInfo(client);
				if (!spreadInfo.IsOpen)
				{
					spreadInfo.State = -2;
					result = spreadInfo;
				}
				else
				{
					ESpreadState espreadState = 0;
					switch (awardType)
					{
					case 1:
						espreadState = this.AwardOne(client, spreadInfo, awardType, SpreadManager._awardVipInfo, spreadInfo.CountVip);
						break;
					case 2:
						espreadState = this.AwardOne(client, spreadInfo, awardType, SpreadManager._awardLevelInfo, spreadInfo.CountLevel);
						break;
					case 3:
						espreadState = this.AwardCount(client, spreadInfo);
						break;
					case 4:
						espreadState = this.AwardOne(client, spreadInfo, awardType, SpreadManager._awardVerifyInfo, 1);
						break;
					}
					if (espreadState != 1)
					{
						spreadInfo.State = espreadState;
					}
					result = client.ClientData.MySpreadData;
				}
			}
			return result;
		}

		private ESpreadState SpreadVerifyCode(GameClient client, string verifyCode)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				if (string.IsNullOrEmpty(verifyCode))
				{
					result = -11;
				}
				else
				{
					SpreadData spreadInfo = this.GetSpreadInfo(client);
					if (!spreadInfo.IsOpen)
					{
						result = -2;
					}
					else if (!string.IsNullOrEmpty(spreadInfo.VerifyCode))
					{
						result = -12;
					}
					else
					{
						DateTime regTime = Global.GetRegTime(client.ClientData);
						if (regTime < SpreadManager._createDate)
						{
							result = -13;
						}
						else
						{
							string[] array = verifyCode.Split(new char[]
							{
								'#'
							});
							if (array.Length < 2)
							{
								result = -14;
							}
							else
							{
								int num = StringUtil.SpreadCodeToID(array[0]);
								int num2 = StringUtil.SpreadCodeToID(array[1]);
								if (num == client.ClientData.ZoneID && num2 == client.ClientData.RoleID)
								{
									result = -15;
								}
								else
								{
									ESpreadState espreadState = this.HCheckVerifyCode(client, num, num2);
									if (espreadState == -12)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(532, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										result = -12;
									}
									else if (espreadState != 1)
									{
										result = espreadState;
									}
									else
									{
										SpreadVerifyData spreadVerifyData = new SpreadVerifyData();
										spreadVerifyData.VerifyCode = verifyCode;
										client.ClientData.MySpreadVerifyData = spreadVerifyData;
										result = 1;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private ESpreadState SpreadTelCodeGet(GameClient client, string tel)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadInfo = this.GetSpreadInfo(client);
				if (!spreadInfo.IsOpen)
				{
					result = -2;
				}
				else
				{
					SpreadVerifyData mySpreadVerifyData = client.ClientData.MySpreadVerifyData;
					if (mySpreadVerifyData == null || string.IsNullOrEmpty(mySpreadVerifyData.VerifyCode))
					{
						result = -11;
					}
					else if (string.IsNullOrEmpty(tel))
					{
						result = -30;
					}
					else if (!this.IsTel(tel))
					{
						result = -31;
					}
					else if (!string.IsNullOrEmpty(mySpreadVerifyData.Tel) && TimeUtil.NowDateTime() < mySpreadVerifyData.TelTime.AddSeconds((double)SpreadManager.TEL_CODE_VERIFY_SECOND))
					{
						result = 1;
					}
					else
					{
						ESpreadState espreadState = this.HTelCodeGet(client, tel);
						if (espreadState != 1)
						{
							result = espreadState;
						}
						else if (string.IsNullOrEmpty(tel))
						{
							result = -33;
						}
						else
						{
							mySpreadVerifyData.Tel = tel;
							mySpreadVerifyData.TelTime = TimeUtil.NowDateTime();
							result = 1;
						}
					}
				}
			}
			return result;
		}

		private ESpreadState SpreadTelCodeVerify(GameClient client, string telCode)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadInfo = this.GetSpreadInfo(client);
				if (!spreadInfo.IsOpen)
				{
					result = -2;
				}
				else
				{
					SpreadVerifyData mySpreadVerifyData = client.ClientData.MySpreadVerifyData;
					if (mySpreadVerifyData == null || string.IsNullOrEmpty(mySpreadVerifyData.VerifyCode))
					{
						result = -11;
					}
					else if (string.IsNullOrEmpty(mySpreadVerifyData.Tel))
					{
						result = -30;
					}
					else if (TimeUtil.NowDateTime() > mySpreadVerifyData.TelTime.AddSeconds((double)SpreadManager.TEL_CODE_VERIFY_SECOND))
					{
						result = -35;
					}
					else if (!this.IsTelCode(telCode))
					{
						result = -34;
					}
					else
					{
						int telCode2 = int.Parse(telCode);
						ESpreadState espreadState = this.HTelCodeVerify(client, telCode2);
						if (espreadState != 1)
						{
							result = espreadState;
						}
						else
						{
							int num = (client.ClientData.VipLevel >= SpreadManager._vipLimit) ? 1 : 0;
							int num2 = (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level >= SpreadManager._levelLimit) ? 1 : 0;
							if (num > 0)
							{
								Global.UpdateRoleParamByName(client, "SpreadIsVip", "1", true);
							}
							if (num2 > 0)
							{
								Global.UpdateRoleParamByName(client, "SpreadIsLevel", "1", true);
							}
							spreadInfo.VerifyCode = mySpreadVerifyData.VerifyCode;
							client.ClientData.MySpreadVerifyData = null;
							Global.UpdateRoleParamByName(client, "VerifyCode", mySpreadVerifyData.VerifyCode, true);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		private ESpreadState AwardOne(GameClient client, SpreadData data, int awardType, SpreadAwardInfo awardInfo, int countSum)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				bool flag2 = false;
				ESpreadState espreadState = -1;
				switch (awardType)
				{
				case 1:
				case 2:
					if (string.IsNullOrEmpty(data.SpreadCode))
					{
						return -20;
					}
					break;
				case 4:
					if (string.IsNullOrEmpty(data.VerifyCode))
					{
						return -10;
					}
					break;
				}
				string text = "";
				int num = 0;
				data.AwardDic.TryGetValue(awardType, out text);
				if (!string.IsNullOrEmpty(text))
				{
					num = Math.Max(int.Parse(text), 0);
				}
				int num2 = countSum - num;
				if (num2 <= 0)
				{
					result = -4;
				}
				else
				{
					int num3 = (num2 + 9) / 10;
					for (int i = 0; i < num3; i++)
					{
						int val = num2 - i * 10;
						int num4 = Math.Min(val, 10);
						List<GoodsData> list = new List<GoodsData>();
						if (awardInfo != null && awardInfo.DefaultGoodsList != null)
						{
							list.AddRange(awardInfo.DefaultGoodsList);
						}
						List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
						if (awardPro != null)
						{
							list.AddRange(awardPro);
						}
						if (!Global.CanAddGoodsDataList(client, list))
						{
							espreadState = -3;
							break;
						}
						num += num4;
						bool flag3 = SpreadManager.DBAwardUpdate(client.ClientData.ZoneID, client.ClientData.RoleID, awardType, num.ToString(), client.ServerId);
						if (!flag3)
						{
							espreadState = -1;
							break;
						}
						if (data.AwardDic.ContainsKey(awardType))
						{
							data.AwardDic[awardType] = num.ToString();
						}
						else
						{
							data.AwardDic.Add(awardType, num.ToString());
						}
						for (int j = 0; j < list.Count; j++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[j].GoodsID, list[j].GCount * num4, list[j].Quality, "", list[j].Forge_level, list[j].Binding, 0, "", true, 1, "推广", "1900-01-01 12:00:00", list[j].AddPropIndex, list[j].BornIndex, list[j].Lucky, list[j].Strong, list[j].ExcellenceInfo, list[j].AppendPropLev, 0, null, null, 0, true);
						}
						flag2 = true;
					}
					if (flag2)
					{
						SpreadManager.CheckActivityTip(client);
						result = 1;
					}
					else
					{
						result = espreadState;
					}
				}
			}
			return result;
		}

		private ESpreadState AwardCount(GameClient client, SpreadData data)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				bool flag2 = false;
				ESpreadState espreadState = -1;
				int key = 3;
				if (string.IsNullOrEmpty(data.SpreadCode))
				{
					result = -20;
				}
				else
				{
					IEnumerable<KeyValuePair<int, int>> enumerable = from dic in data.AwardCountDic
					where dic.Value == 0 && dic.Key <= data.CountRole
					select dic;
					if (!enumerable.Any<KeyValuePair<int, int>>())
					{
						result = -4;
					}
					else
					{
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						foreach (KeyValuePair<int, int> keyValuePair in enumerable)
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
						foreach (KeyValuePair<int, int> keyValuePair in dictionary)
						{
							if (!SpreadManager._awardCountDic.ContainsKey(keyValuePair.Key))
							{
								espreadState = -4;
								break;
							}
							SpreadCountAwardInfo spreadCountAwardInfo = SpreadManager._awardCountDic[keyValuePair.Key];
							if (spreadCountAwardInfo == null)
							{
								espreadState = -4;
								break;
							}
							List<GoodsData> list = new List<GoodsData>();
							if (spreadCountAwardInfo != null && spreadCountAwardInfo.DefaultGoodsList != null)
							{
								list.AddRange(spreadCountAwardInfo.DefaultGoodsList);
							}
							List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, spreadCountAwardInfo.ProGoodsList);
							if (awardPro != null)
							{
								list.AddRange(awardPro);
							}
							if (!Global.CanAddGoodsDataList(client, list))
							{
								espreadState = -3;
								break;
							}
							data.AwardCountDic[keyValuePair.Key] = 1;
							string text = SpreadManager.AwardCountToStr(data.AwardCountDic);
							bool flag3 = SpreadManager.DBAwardUpdate(client.ClientData.ZoneID, client.ClientData.RoleID, 3, text, client.ServerId);
							if (!flag3)
							{
								espreadState = -1;
								data.AwardCountDic[keyValuePair.Key] = 0;
								break;
							}
							if (data.AwardDic.ContainsKey(key))
							{
								data.AwardDic[key] = text;
							}
							else
							{
								data.AwardDic.Add(key, text);
							}
							for (int i = 0; i < list.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "推广", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
							}
							flag2 = true;
						}
						if (flag2)
						{
							SpreadManager.CheckActivityTip(client);
							result = 1;
						}
						else
						{
							result = espreadState;
						}
					}
				}
			}
			return result;
		}

		public static void initRoleSpreadData(GameClient client)
		{
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadData = new SpreadData();
				spreadData.IsOpen = SpreadManager.IsSpreadOpen();
				if (!spreadData.IsOpen)
				{
					client.ClientData.MySpreadData = spreadData;
				}
				else
				{
					spreadData.SpreadCode = Global.GetRoleParamByName(client, "SpreadCode");
					spreadData.VerifyCode = Global.GetRoleParamByName(client, "VerifyCode");
					if (string.IsNullOrEmpty(spreadData.SpreadCode) && string.IsNullOrEmpty(spreadData.VerifyCode))
					{
						client.ClientData.MySpreadData = spreadData;
					}
					else
					{
						if (!string.IsNullOrEmpty(spreadData.SpreadCode))
						{
							int[] array = SpreadManager.HSpreadCount(client);
							spreadData.CountRole = array[0];
							spreadData.CountVip = Math.Min(array[1], SpreadManager._vipCountMax);
							spreadData.CountLevel = Math.Min(array[2], SpreadManager._levelCountMax);
						}
						spreadData.AwardDic = SpreadManager.DBAwardGet(client.ClientData.ZoneID, client.ClientData.RoleID, client.ServerId);
						string awardStr = "";
						spreadData.AwardDic.TryGetValue(3, out awardStr);
						spreadData.AwardCountDic = SpreadManager.initAwardCountDic(awardStr);
						client.ClientData.MySpreadData = spreadData;
						SpreadManager.CheckActivityTip(client);
					}
				}
			}
		}

		private static Dictionary<int, int> initAwardCountDic(string awardStr)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (string.IsNullOrEmpty(awardStr))
			{
				foreach (int key in SpreadManager._awardCountDic.Keys)
				{
					dictionary.Add(key, 0);
				}
			}
			else
			{
				string[] array = awardStr.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					dictionary.Add(int.Parse(array[i]), int.Parse(array[++i]));
				}
			}
			return dictionary;
		}

		private static string AwardCountToStr(Dictionary<int, int> dic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, int> keyValuePair in dic)
			{
				stringBuilder.Append(string.Format("{0},{1},", keyValuePair.Key, keyValuePair.Value));
			}
			string text = stringBuilder.ToString();
			return text.Substring(0, text.Length - 1);
		}

		private static void CheckActivityTip(GameClient client)
		{
			SpreadData data = client.ClientData.MySpreadData;
			bool flag = false;
			if (!string.IsNullOrEmpty(data.SpreadCode))
			{
				if (data.CountLevel > 0)
				{
					if (!data.AwardDic.ContainsKey(2))
					{
						flag = true;
					}
					else
					{
						int num = int.Parse(data.AwardDic[2]);
						if (data.CountLevel - num > 0)
						{
							flag = true;
						}
					}
				}
				if (data.CountVip > 0)
				{
					if (!data.AwardDic.ContainsKey(1))
					{
						flag = true;
					}
					else
					{
						IEnumerable<KeyValuePair<int, string>> source = from info in data.AwardDic
						where info.Key == 1 && data.CountVip - int.Parse(info.Value) > 0
						select info;
						if (source.Any<KeyValuePair<int, string>>())
						{
							flag = true;
						}
					}
				}
				if (data.CountRole > 0)
				{
					IEnumerable<KeyValuePair<int, int>> source2 = from info in data.AwardCountDic
					where info.Key <= data.CountRole && info.Value == 0
					select info;
					if (source2.Any<KeyValuePair<int, int>>())
					{
						flag = true;
					}
				}
			}
			if (!string.IsNullOrEmpty(data.VerifyCode))
			{
				IEnumerable<KeyValuePair<int, string>> source = from info in data.AwardDic
				where info.Key == 4 && int.Parse(info.Value) <= 0
				select info;
				if (source.Any<KeyValuePair<int, string>>())
				{
					flag = true;
				}
			}
			if (flag)
			{
				client._IconStateMgr.AddFlushIconState(14105, true);
			}
			else
			{
				client._IconStateMgr.AddFlushIconState(14105, false);
			}
			client._IconStateMgr.SendIconStateToClient(client);
		}

		private string GetSpreadCode(GameClient client)
		{
			int zoneID = client.ClientData.ZoneID;
			int roleID = client.ClientData.RoleID;
			return string.Format("{0}#{1}", StringUtil.SpreadIDToCode(zoneID), StringUtil.SpreadIDToCode(roleID));
		}

		public static bool IsSpreadOpen()
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(7))
			{
				result = false;
			}
			else
			{
				int num = 0;
				switch (GameCoreInterface.getinstance().GetPlatformType())
				{
				case 1:
					num = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_APP", -1);
					break;
				case 2:
					num = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_YueYu", -1);
					break;
				case 3:
					num = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_Android", -1);
					break;
				}
				result = (num > 0);
			}
			return result;
		}

		public void SpreadIsLevel(GameClient client)
		{
			SpreadData spreadInfo = this.GetSpreadInfo(client);
			if (spreadInfo != null && spreadInfo.IsOpen && !string.IsNullOrEmpty(spreadInfo.VerifyCode))
			{
				int num = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				bool flag = Global.GetRoleParamsInt32FromDB(client, "SpreadIsLevel") > 0;
				if (num >= SpreadManager._levelLimit && !flag)
				{
					string[] array = spreadInfo.VerifyCode.Split(new char[]
					{
						'#'
					});
					if (array.Length >= 2)
					{
						int pzoneID = StringUtil.SpreadCodeToID(array[0]);
						int proleID = StringUtil.SpreadCodeToID(array[1]);
						bool flag2 = SpreadClient.getInstance().SpreadLevel(pzoneID, proleID, client.ClientData.ZoneID, client.ClientData.RoleID);
						if (flag2)
						{
							Global.UpdateRoleParamByName(client, "SpreadIsLevel", "1", true);
						}
					}
				}
			}
		}

		public void SpreadIsVip(GameClient client)
		{
			SpreadData spreadInfo = this.GetSpreadInfo(client);
			if (spreadInfo != null && spreadInfo.IsOpen && !string.IsNullOrEmpty(spreadInfo.VerifyCode))
			{
				int vipLevel = client.ClientData.VipLevel;
				bool flag = Global.GetRoleParamsInt32FromDB(client, "SpreadIsVip") > 0;
				if (vipLevel >= SpreadManager._vipLimit && !flag)
				{
					string[] array = spreadInfo.VerifyCode.Split(new char[]
					{
						'#'
					});
					if (array.Length >= 2)
					{
						int pzoneID = StringUtil.SpreadCodeToID(array[0]);
						int proleID = StringUtil.SpreadCodeToID(array[1]);
						bool flag2 = SpreadClient.getInstance().SpreadVip(pzoneID, proleID, client.ClientData.ZoneID, client.ClientData.RoleID);
						if (flag2)
						{
							Global.UpdateRoleParamByName(client, "SpreadIsVip", "1", true);
						}
					}
				}
			}
		}

		public static Dictionary<int, string> DBAwardGet(int zoneID, int roleID, int serverID)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			string text = "";
			string strcmd = string.Format("{0}:{1}", zoneID, roleID);
			string[] array = Global.ExecuteDBCmd(13114, strcmd, serverID);
			if (array != null && array.Length == 1)
			{
				text = array[0];
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array2 = text.Split(new char[]
				{
					'$'
				});
				foreach (string text2 in array2)
				{
					string[] array4 = text2.Split(new char[]
					{
						'#'
					});
					dictionary.Add(int.Parse(array4[0]), array4[1]);
				}
			}
			return dictionary;
		}

		public static bool DBAwardUpdate(int zoneID, int roleID, int awardType, string award, int serverID)
		{
			bool result = false;
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				zoneID,
				roleID,
				awardType,
				award
			});
			string[] array = Global.ExecuteDBCmd(13115, strcmd, serverID);
			if (array != null && array.Length == 1)
			{
				result = (array[0] == "1");
			}
			return result;
		}

		public static bool HSpreadSign(GameClient client)
		{
			int num = SpreadClient.getInstance().SpreadSign(client.ClientData.ZoneID, client.ClientData.RoleID);
			return num > 0;
		}

		public static int[] HSpreadCount(GameClient client)
		{
			return SpreadClient.getInstance().SpreadCount(client.ClientData.ZoneID, client.ClientData.RoleID);
		}

		private ESpreadState HCheckVerifyCode(GameClient client, int pzoneID, int proleID)
		{
			int isVip = (client.ClientData.VipLevel >= SpreadManager._vipLimit) ? 1 : 0;
			int isLevel = (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level >= SpreadManager._levelLimit) ? 1 : 0;
			return SpreadClient.getInstance().CheckVerifyCode(client.strUserID, client.ClientData.ZoneID, client.ClientData.RoleID, pzoneID, proleID, isVip, isLevel);
		}

		private ESpreadState HTelCodeGet(GameClient client, string tel)
		{
			return SpreadClient.getInstance().TelCodeGet(client.ClientData.ZoneID, client.ClientData.RoleID, tel);
		}

		private ESpreadState HTelCodeVerify(GameClient client, int telCode)
		{
			return SpreadClient.getInstance().TelCodeVerify(client.ClientData.ZoneID, client.ClientData.RoleID, telCode);
		}

		private bool IsTel(string tel)
		{
			return Regex.IsMatch(tel.ToString(), "^(0|86|17951)?(1[0-9])[0-9]{9}$");
		}

		private bool IsTelCode(string tel)
		{
			return Regex.IsMatch(tel.ToString(), "^\\d{6}$");
		}

		private static bool InitConfig()
		{
			string text = "";
			try
			{
				SpreadManager._awardCountDic.Clear();
				text = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanLeiJi.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("GiftList");
				if (null == xelement2)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement2.Elements();
				string safeAttributeStr;
				foreach (XElement xelement3 in enumerable)
				{
					if (xelement3 != null)
					{
						SpreadCountAwardInfo spreadCountAwardInfo = new SpreadCountAwardInfo();
						spreadCountAwardInfo.Count = Convert.ToInt32(Global.GetDefAttributeStr(xelement3, "MinNum", "0"));
						safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								spreadCountAwardInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								spreadCountAwardInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
							}
						}
						SpreadManager._awardCountDic.Add(spreadCountAwardInfo.Count, spreadCountAwardInfo);
					}
				}
				SpreadManager._levelLimit = 0;
				SpreadManager._awardLevelInfo = new SpreadAwardInfo();
				text = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanLevel.xml");
				xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				xelement2 = xelement.Element("TuiGuangYuanLevel");
				if (null == xelement2)
				{
					return false;
				}
				int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinZhuanSheng", "0"));
				int num2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinLevel", "0"));
				SpreadManager._levelLimit = num * 100 + num2;
				xelement2 = xelement.Element("GiftList").Element("Award");
				if (null == xelement2)
				{
					return false;
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardLevelInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardLevelInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				SpreadManager._vipLimit = 0;
				SpreadManager._awardVipInfo = new SpreadAwardInfo();
				text = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanVip.xml");
				xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				xelement2 = xelement.Element("TuiGuangYuanVip");
				if (null == xelement2)
				{
					return false;
				}
				SpreadManager._vipLimit = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "VipLevel", "0"));
				xelement2 = xelement.Element("GiftList").Element("Award");
				if (null == xelement2)
				{
					return false;
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardVipInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardVipInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				SpreadManager._awardVerifyInfo = new SpreadAwardInfo();
				text = Global.IsolateResPath("Config/TuiGuang/TuiGuangXinYongHu.xml");
				xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				xelement2 = xelement.Element("GiftList").Element("Award");
				if (null == xelement2)
				{
					return false;
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardVerifyInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
				if (!string.IsNullOrEmpty(safeAttributeStr))
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						SpreadManager._awardVerifyInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
					}
				}
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("TuiGuangCreatData");
				SpreadManager._createDate = DateTime.Parse(paramValueByName);
				SpreadManager._vipCountMax = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuangVIPRewardNum", SpreadManager.VIP_LEVEL_COUNT_MAX_DEFAULT);
				SpreadManager._levelCountMax = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuangLevelRewardNum", SpreadManager.VIP_LEVEL_COUNT_MAX_DEFAULT);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				return false;
			}
			return true;
		}

		public void SpreadSetCount(GameClient client, int[] counts)
		{
			SpreadData spreadInfo = this.GetSpreadInfo(client);
			spreadInfo.CountRole = counts[0];
			spreadInfo.CountVip = Math.Min(counts[1], SpreadManager._vipCountMax);
			spreadInfo.CountLevel = Math.Min(counts[2], SpreadManager._levelCountMax);
			client.ClientData.MySpreadData = spreadInfo;
		}

		public const int _sceneType = 10002;

		private static int TEL_CODE_VERIFY_SECOND = 70;

		private static int AWARD_COUNT_MAX = 10;

		private static SpreadManager instance = new SpreadManager();

		private static Dictionary<int, SpreadCountAwardInfo> _awardCountDic = new Dictionary<int, SpreadCountAwardInfo>();

		private static int _levelLimit = 0;

		private static SpreadAwardInfo _awardLevelInfo = new SpreadAwardInfo();

		private static int _vipLimit = 0;

		private static SpreadAwardInfo _awardVipInfo = new SpreadAwardInfo();

		private static SpreadAwardInfo _awardVerifyInfo = new SpreadAwardInfo();

		private static DateTime _createDate = DateTime.MinValue;

		private static int _vipCountMax = 0;

		private static int _levelCountMax = 0;

		private static int VIP_LEVEL_COUNT_MAX_DEFAULT = 50;
	}
}
