using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Marriage.CoupleWish;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	internal class MarryLogic
	{
		public static void LoadMarryBaseConfig()
		{
			MarryLogic.MarryCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("JieHunCost"));
			MarryLogic.MarryCD = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("QiuHuiCD"));
			MarryLogic.MarryReplyTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MarriageTipsTime"));
			MarryLogic.DivorceCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("DivorceJinBiCost"));
			MarryLogic.DivorceForceCost = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("DivorceZuanShiCost"));
		}

		public static bool IsVersionSystemOpenOfMarriage()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(4) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Marriage");
		}

		public static MarryApplyData AddMarryApply(int roleID, MarryApplyType type, int spouseID)
		{
			MarryApplyData marryApplyData = null;
			lock (MarryLogic.MarryApplyList)
			{
				if (!MarryLogic.MarryApplyList.ContainsKey(roleID))
				{
					marryApplyData = new MarryApplyData
					{
						ApplyExpireTime = TimeUtil.NOW() + (long)(MarryLogic.MarryReplyTime * 1000),
						ApplyCDEndTime = 0L,
						ApplySpouseRoleID = spouseID,
						ApplyType = type
					};
					if (type == MarryApplyType.ApplyInit)
					{
						marryApplyData.ApplyCDEndTime = TimeUtil.NOW() + (long)(MarryLogic.MarryCD * 1000);
					}
					else
					{
						marryApplyData.ApplyCDEndTime = marryApplyData.ApplyExpireTime;
					}
					MarryLogic.MarryApplyList.Add(roleID, marryApplyData);
				}
			}
			return marryApplyData;
		}

		public static bool RemoveMarryApply(int roleID, MarryApplyType type = MarryApplyType.ApplyNull)
		{
			bool result;
			lock (MarryLogic.MarryApplyList)
			{
				if (type == MarryApplyType.ApplyNull)
				{
					result = MarryLogic.MarryApplyList.Remove(roleID);
				}
				else
				{
					MarryApplyData marryApplyData;
					bool flag2 = MarryLogic.MarryApplyList.TryGetValue(roleID, out marryApplyData);
					if (flag2)
					{
						if (marryApplyData.ApplyType != type)
						{
							flag2 = false;
						}
						else if (marryApplyData.ApplyExpireTime == 0L)
						{
							flag2 = false;
						}
						else if (marryApplyData.ApplyExpireTime <= TimeUtil.NOW())
						{
							flag2 = false;
						}
						else
						{
							marryApplyData.ApplyExpireTime = 0L;
						}
					}
					result = flag2;
				}
			}
			return result;
		}

		public static void ApplyPeriodicClear(long ticks)
		{
			if (ticks >= MarryLogic.NextPeriodicCheckTime)
			{
				MarryLogic.NextPeriodicCheckTime = ticks + 10000L;
				lock (MarryLogic.MarryApplyList)
				{
					foreach (KeyValuePair<int, MarryApplyData> keyValuePair in MarryLogic.MarryApplyList.ToList<KeyValuePair<int, MarryApplyData>>())
					{
						MarryApplyData value = keyValuePair.Value;
						if (value.ApplyExpireTime > 0L && value.ApplyExpireTime <= ticks)
						{
							MarryLogic.ApplyReturnMoney(keyValuePair.Key, value, null);
							value.ApplyExpireTime = 0L;
						}
						if (value.ApplyCDEndTime <= ticks)
						{
							MarryLogic.MarryApplyList.Remove(keyValuePair.Key);
						}
					}
				}
			}
		}

		public static void ApplyShutdownClear()
		{
			lock (MarryLogic.MarryApplyList)
			{
				foreach (KeyValuePair<int, MarryApplyData> keyValuePair in MarryLogic.MarryApplyList)
				{
					MarryApplyData value = keyValuePair.Value;
					if (value.ApplyExpireTime > 0L)
					{
						MarryLogic.ApplyReturnMoney(keyValuePair.Key, value, null);
					}
				}
				MarryLogic.MarryApplyList.Clear();
			}
		}

		public static void ApplyLogoutClear(GameClient client)
		{
			lock (MarryLogic.MarryApplyList)
			{
				MarryApplyData marryApplyData;
				if (MarryLogic.MarryApplyList.TryGetValue(client.ClientData.RoleID, out marryApplyData))
				{
					if (marryApplyData.ApplyExpireTime > 0L)
					{
						MarryLogic.ApplyReturnMoney(0, marryApplyData, client);
						marryApplyData.ApplyExpireTime = 0L;
					}
				}
			}
		}

		public static void ApplyReturnMoney(int roleID, MarryApplyData applyData, GameClient client = null)
		{
			if (client == null)
			{
				client = GameManager.ClientMgr.FindClient(roleID);
			}
			if (client != null)
			{
				if (applyData.ApplyType == MarryApplyType.ApplyInit)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.MarryCost, "求婚超时返回钻石", ActivityTypes.None, "");
				}
				else if (applyData.ApplyType == MarryApplyType.ApplyDivorce)
				{
					GameManager.ClientMgr.AddMoney1(client, MarryLogic.DivorceCost, "离婚超时返还绑金", true);
				}
			}
		}

		public static bool ApplyExist(int roleID)
		{
			lock (MarryLogic.MarryApplyList)
			{
				foreach (KeyValuePair<int, MarryApplyData> keyValuePair in MarryLogic.MarryApplyList)
				{
					if (roleID == keyValuePair.Value.ApplySpouseRoleID || roleID == keyValuePair.Key)
					{
						if (keyValuePair.Value.ApplyExpireTime > 0L)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static TCPProcessCmdResults ProcessMarryInit(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
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
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientSocket.IsKuaFuLogin)
				{
					gameClient.sendCmd(nID, string.Format("{0}:{1}:{2}", -12, num, num2), false);
					tcpOutPacket = null;
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryResult marryResult = MarryLogic.MarryInit(gameClient, num2);
				string data2 = string.Format("{0}:{1}:{2}", (int)marryResult, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessMarryReply(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int accept = Convert.ToInt32(array[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult marryResult = MarryLogic.MarryReply(client, num2, accept);
				string data2 = string.Format("{0}:{1}:{2}", (int)marryResult, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessMarryDivorce(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int divorceType = Convert.ToInt32(array[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult marryResult = MarryLogic.MarryDivorce(client, (MarryDivorceType)divorceType);
				string data2 = string.Format("{0}", (int)marryResult);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessMarryAutoReject(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int autoReject = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryResult marryResult = MarryLogic.MarryAutoReject(gameClient, autoReject);
				string data2 = string.Format("{0}:{1}", (int)marryResult, gameClient.ClientData.MyMarriageData.byAutoReject);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static bool SameSexMarry(bool diff = false)
		{
			return diff || GameManager.PlatConfigMgr.GetGameConfigItemStr("SameSexMarry", "0") == "1";
		}

		public static MarryResult MarryInit(GameClient client, int spouseID)
		{
			MarryResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryResult.Error_Denied_For_Minor_Occupation;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 58, true) || !MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (client.ClientData.MyMarriageData.byMarrytype > 0)
			{
				result = MarryResult.SelfMarried;
			}
			else if (client.ClientData.ChangeLifeCount < 3)
			{
				result = MarryResult.SelfLevelNotEnough;
			}
			else if (client.ClientData.ExchangeID > 0 || client.ClientSocket.IsKuaFuLogin || client.ClientData.CopyMapID > 0)
			{
				result = MarryResult.SelfBusy;
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(spouseID);
				if (gameClient == null)
				{
					result = MarryResult.TargetOffline;
				}
				else if (!gameClient.ClientData.IsMainOccupation)
				{
					result = MarryResult.Error_Denied_For_Minor_Occupation;
				}
				else if (!GlobalNew.IsGongNengOpened(gameClient, 58, false))
				{
					result = MarryResult.TargetNotOpen;
				}
				else
				{
					if (!MarryLogic.SameSexMarry(false))
					{
						if (client.ClientData.RoleSex == gameClient.ClientData.RoleSex)
						{
							return MarryResult.InvalidSex;
						}
					}
					if (gameClient.ClientData.MyMarriageData.byMarrytype > 0)
					{
						result = MarryResult.TargetMarried;
					}
					else if (gameClient.ClientData.ChangeLifeCount < 3)
					{
						result = MarryResult.TargetLevelNotEnough;
					}
					else if (gameClient.ClientData.ExchangeID > 0 || gameClient.ClientSocket.IsKuaFuLogin || gameClient.ClientData.CopyMapID > 0)
					{
						result = MarryResult.TargetBusy;
					}
					else if (MarryLogic.ApplyExist(spouseID))
					{
						result = MarryResult.TargetBusy;
					}
					else if (gameClient.ClientData.MyMarriageData.byAutoReject == 1)
					{
						result = MarryResult.AutoReject;
					}
					else if (MarryLogic.AddMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyInit, spouseID) == null)
					{
						result = MarryResult.ApplyCD;
					}
					else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.MarryCost, "结婚", false, true, false, DaiBiSySType.None))
					{
						MarryLogic.RemoveMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyNull);
						result = MarryResult.MoneyNotEnough;
					}
					else
					{
						string cmdData = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, client.ClientData.RoleName);
						gameClient.sendCmd(894, cmdData, false);
						result = MarryResult.Success;
					}
				}
			}
			return result;
		}

		public static MarryResult MarryReply(GameClient client, int sourceID, int accept)
		{
			MarryResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (client.ClientData.MyMarriageData.byMarrytype > 0)
			{
				result = MarryResult.SelfMarried;
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(sourceID);
				if (gameClient == null)
				{
					result = MarryResult.ApplyTimeout;
				}
				else if (gameClient.ClientData.MyMarriageData.byMarrytype > 0)
				{
					result = MarryResult.TargetMarried;
				}
				else if (!MarryLogic.RemoveMarryApply(sourceID, MarryApplyType.ApplyInit))
				{
					result = MarryResult.ApplyTimeout;
				}
				else
				{
					if (!client.ClientData.IsMainOccupation || !gameClient.ClientData.IsMainOccupation)
					{
						accept = 0;
					}
					if (accept == 0 || client.ClientData.MyMarriageData.byAutoReject == 1)
					{
						string cmdData = string.Format("{0}:{1}:{2}", 1, client.ClientData.RoleID, client.ClientData.RoleName);
						gameClient.sendCmd(894, cmdData, false);
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, MarryLogic.MarryCost, "求婚被拒绝返还钻石", ActivityTypes.None, "");
					}
					else
					{
						MarryLogic.RemoveMarryApply(sourceID, MarryApplyType.ApplyNull);
						MarryLogic.ApplyLogoutClear(client);
						MarryLogic.RemoveMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyNull);
						int num = 0;
						if (null != MarriageOtherLogic.getInstance().WeddingRingDic.SystemXmlItemDict)
						{
							num = MarriageOtherLogic.getInstance().WeddingRingDic.SystemXmlItemDict.Keys.First<int>();
						}
						if (gameClient.ClientData.MyMarriageData.nRingID <= 0)
						{
							gameClient.ClientData.MyMarriageData.nRingID = num;
						}
						if (client.ClientData.MyMarriageData.nRingID <= 0)
						{
							client.ClientData.MyMarriageData.nRingID = num;
						}
						sbyte b = (gameClient.ClientData.RoleSex != 1 || client.ClientData.RoleSex == gameClient.ClientData.RoleSex) ? 1 : 2;
						gameClient.ClientData.MyMarriageData.byMarrytype = b;
						client.ClientData.MyMarriageData.byMarrytype = ((b == 1) ? 2 : 1);
						gameClient.ClientData.MyMarriageData.nSpouseID = client.ClientData.RoleID;
						client.ClientData.MyMarriageData.nSpouseID = sourceID;
						if (gameClient.ClientData.MyMarriageData.byGoodwilllevel == 0)
						{
							gameClient.ClientData.MyMarriageData.ChangTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
							gameClient.ClientData.MyMarriageData.byGoodwilllevel = 1;
						}
						if (client.ClientData.MyMarriageData.byGoodwilllevel == 0)
						{
							client.ClientData.MyMarriageData.ChangTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
							client.ClientData.MyMarriageData.byGoodwilllevel = 1;
						}
						EventLogManager.AddRingBuyEvent(gameClient, 0, num, 0, 0, 0, 1, "");
						EventLogManager.AddRingBuyEvent(client, 0, num, 0, 0, 0, 1, "");
						MarryFuBenMgr.UpdateMarriageData2DB(gameClient);
						MarryFuBenMgr.UpdateMarriageData2DB(client);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(gameClient, true);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
						MarriageOtherLogic.getInstance().UpdateRingAttr(gameClient, true, false);
						if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
						{
							client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
							client._IconStateMgr.SendIconStateToClient(client);
						}
						if (gameClient._IconStateMgr.CheckJieRiFanLi(gameClient, ActivityTypes.JieriMarriage))
						{
							gameClient._IconStateMgr.AddFlushIconState(14000, gameClient._IconStateMgr.IsAnyJieRiTipActived());
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
						}
						FriendData friendData = Global.FindFriendData(client, sourceID);
						if (friendData != null && friendData.FriendType != 0)
						{
							GameManager.ClientMgr.RemoveFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, friendData.DbID);
							friendData = null;
						}
						if (friendData == null)
						{
							GameManager.ClientMgr.AddFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, -1, sourceID, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), 0);
						}
						friendData = Global.FindFriendData(gameClient, client.ClientData.RoleID);
						if (friendData != null && friendData.FriendType != 0)
						{
							GameManager.ClientMgr.RemoveFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, friendData.DbID);
							friendData = null;
						}
						if (friendData == null)
						{
							GameManager.ClientMgr.AddFriend(Global._TCPManager, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, -1, client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), 0);
						}
						string msgText = string.Format(GLang.GetLang(485, new object[0]), gameClient.ClientData.RoleName, client.ClientData.RoleName);
						Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
						SingletonTemplate<CoupleArenaManager>.Instance().OnMarry(gameClient, client);
					}
					result = MarryResult.Success;
				}
			}
			return result;
		}

		public static MarryResult MarryDivorce(GameClient client, MarryDivorceType divorceType)
		{
			MarryResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryResult.NotOpen;
			}
			else if (0 >= client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryResult.NotMarried;
			}
			else if (!SingletonTemplate<CoupleArenaManager>.Instance().IsNowCanDivorce(TimeUtil.NowDateTime()))
			{
				result = MarryResult.DeniedByCoupleAreanTime;
			}
			else
			{
				int nSpouseID = client.ClientData.MyMarriageData.nSpouseID;
				GameClient gameClient = GameManager.ClientMgr.FindClient(nSpouseID);
				if (divorceType == MarryDivorceType.DivorceForce || divorceType == MarryDivorceType.DivorceFree || divorceType == MarryDivorceType.DivorceFreeAccept)
				{
					if (client.ClientData.ExchangeID > 0 || client.ClientSocket.IsKuaFuLogin || client.ClientData.CopyMapID > 0)
					{
						return MarryResult.SelfBusy;
					}
					if (-1 != client.ClientData.FuBenID && MapTypes.MarriageCopy == Global.GetMapType(client.ClientData.MapCode))
					{
						return MarryResult.SelfBusy;
					}
					if (null != gameClient)
					{
						if (-1 != gameClient.ClientData.FuBenID && MapTypes.MarriageCopy == Global.GetMapType(gameClient.ClientData.MapCode))
						{
							return MarryResult.TargetBusy;
						}
					}
					if (divorceType == MarryDivorceType.DivorceForce || divorceType == MarryDivorceType.DivorceFree)
					{
						if (MarryLogic.ApplyExist(client.ClientData.RoleID))
						{
							return MarryResult.SelfBusy;
						}
					}
				}
				int roleID = client.ClientData.RoleID;
				int num = nSpouseID;
				if (client.ClientData.MyMarriageData.byMarrytype == 2)
				{
					DataHelper2.Swap<int>(ref roleID, ref num);
				}
				if (divorceType == MarryDivorceType.DivorceForce)
				{
					if (client.ClientData.UserMoney < MarryLogic.DivorceForceCost)
					{
						return MarryResult.MoneyNotEnough;
					}
					if (!SingletonTemplate<CoupleWishManager>.Instance().PreClearDivorceData(roleID, num))
					{
						return MarryResult.NotOpen;
					}
					if (!SingletonTemplate<CoupleArenaManager>.Instance().PreClearDivorceData(roleID, num))
					{
						return MarryResult.NotOpen;
					}
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MarryLogic.DivorceForceCost, "强制离婚", false, true, false, DaiBiSySType.None))
					{
					}
					client.ClientData.MyMarriageData.byMarrytype = -1;
					client.ClientData.MyMarriageData.nSpouseID = -1;
					MarryFuBenMgr.UpdateMarriageData2DB(client);
					MarriageOtherLogic.getInstance().ResetRingAttr(client);
					if (null != gameClient)
					{
						gameClient.ClientData.MyMarriageData.nSpouseID = -1;
						gameClient.ClientData.MyMarriageData.byMarrytype = -1;
						MarryFuBenMgr.UpdateMarriageData2DB(gameClient);
						MarriageOtherLogic.getInstance().ResetRingAttr(gameClient);
						MarriageOtherLogic.getInstance().SendMarriageDataToClient(gameClient, true);
						if (gameClient._IconStateMgr.CheckJieRiFanLi(gameClient, ActivityTypes.JieriMarriage))
						{
							gameClient._IconStateMgr.AddFlushIconState(14000, gameClient._IconStateMgr.IsAnyJieRiTipActived());
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
						}
					}
					else
					{
						string cmd = string.Format("{0}", nSpouseID);
						MarriageData marriageData = Global.sendToDB<MarriageData, string>(10186, cmd, client.ServerId);
						if (marriageData != null && 0 < marriageData.byMarrytype)
						{
							marriageData.byMarrytype = -1;
							marriageData.nSpouseID = -1;
							MarryFuBenMgr.UpdateMarriageData2DB(nSpouseID, marriageData, client);
						}
					}
					MarryPartyLogic.getInstance().MarryPartyRemove(client.ClientData.RoleID, true, client);
					MarryPartyLogic.getInstance().MarryPartyRemove(nSpouseID, true, client);
					MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string content = string.Format(GLang.GetLang(486, new object[0]), client.ClientData.RoleName);
					MarryLogic.SendDivorceMail(nSpouseID, GLang.GetLang(487, new object[0]), content, gameClient, client.ServerId);
					SingletonTemplate<CoupleArenaManager>.Instance().OnDivorce(client.ClientData.RoleID, nSpouseID);
				}
				else if (divorceType == MarryDivorceType.DivorceFree)
				{
					if (null == gameClient)
					{
						return MarryResult.TargetOffline;
					}
					if (gameClient.ClientData.ExchangeID > 0 || gameClient.ClientSocket.IsKuaFuLogin || gameClient.ClientData.CopyMapID > 0)
					{
						return MarryResult.TargetBusy;
					}
					if (Global.GetTotalBindTongQianAndTongQianVal(client) < MarryLogic.DivorceCost)
					{
						return MarryResult.MoneyNotEnough;
					}
					if (!Global.SubBindTongQianAndTongQian(client, MarryLogic.DivorceCost, "申请离婚"))
					{
						return MarryResult.MoneyNotEnough;
					}
					MarryLogic.AddMarryApply(client.ClientData.RoleID, MarryApplyType.ApplyDivorce, nSpouseID);
					string cmdData = string.Format("{0}:{1}", client.ClientData.RoleID, 1);
					gameClient.sendCmd(892, cmdData, false);
					SingletonTemplate<CoupleArenaManager>.Instance().OnSpouseRequestDivorce(gameClient, client);
				}
				else
				{
					if (null == gameClient)
					{
						return MarryResult.TargetOffline;
					}
					if (!MarryLogic.RemoveMarryApply(nSpouseID, MarryApplyType.ApplyDivorce))
					{
						return MarryResult.ApplyTimeout;
					}
					MarryLogic.RemoveMarryApply(nSpouseID, MarryApplyType.ApplyNull);
					if (divorceType == MarryDivorceType.DivorceFreeAccept)
					{
						if (SingletonTemplate<CoupleWishManager>.Instance().PreClearDivorceData(roleID, num) && SingletonTemplate<CoupleArenaManager>.Instance().PreClearDivorceData(roleID, num))
						{
							client.ClientData.MyMarriageData.byMarrytype = -1;
							client.ClientData.MyMarriageData.nSpouseID = -1;
							gameClient.ClientData.MyMarriageData.byMarrytype = -1;
							gameClient.ClientData.MyMarriageData.nSpouseID = -1;
							MarryFuBenMgr.UpdateMarriageData2DB(client);
							MarryFuBenMgr.UpdateMarriageData2DB(gameClient);
							MarriageOtherLogic.getInstance().SendMarriageDataToClient(client, true);
							MarriageOtherLogic.getInstance().SendMarriageDataToClient(gameClient, true);
							MarriageOtherLogic.getInstance().ResetRingAttr(client);
							MarriageOtherLogic.getInstance().ResetRingAttr(gameClient);
							MarryPartyLogic.getInstance().MarryPartyRemove(client.ClientData.RoleID, true, client);
							MarryPartyLogic.getInstance().MarryPartyRemove(nSpouseID, true, client);
							if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage))
							{
								client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
								client._IconStateMgr.SendIconStateToClient(client);
							}
							if (gameClient._IconStateMgr.CheckJieRiFanLi(gameClient, ActivityTypes.JieriMarriage))
							{
								gameClient._IconStateMgr.AddFlushIconState(14000, gameClient._IconStateMgr.IsAnyJieRiTipActived());
								gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							}
							SingletonTemplate<CoupleArenaManager>.Instance().OnDivorce(client.ClientData.RoleID, nSpouseID);
						}
						else
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, MarryLogic.DivorceCost, "自由离婚拒绝返还绑金", false);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(488, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(488, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					else if (divorceType == MarryDivorceType.DivorceFreeReject)
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, MarryLogic.DivorceCost, "自由离婚拒绝返还绑金", false);
						string cmdData = string.Format("{0}:{1}", client.ClientData.RoleID, 3);
						gameClient.sendCmd(892, cmdData, false);
					}
				}
				result = MarryResult.Success;
			}
			return result;
		}

		public static bool SendDivorceMail(int roleID, string subject, string content, GameClient client, int serverId)
		{
			string text = "";
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				-1,
				GLang.GetLang(112, new object[0]),
				roleID,
				"",
				subject,
				content,
				0,
				0,
				0,
				text
			});
			string[] array = Global.ExecuteDBCmd(10086, strcmd, serverId);
			if (client != null)
			{
				client._IconStateMgr.CheckEmailCount(client, true);
			}
			return array == null;
		}

		public static MarryResult MarryAutoReject(GameClient client, int autoReject)
		{
			if ((int)client.ClientData.MyMarriageData.byAutoReject != autoReject)
			{
				client.ClientData.MyMarriageData.byAutoReject = (sbyte)autoReject;
			}
			MarryFuBenMgr.UpdateMarriageData2DB(client);
			return MarryResult.Success;
		}

		public static bool IsMarried(int roleID)
		{
			RoleDataEx offlineRoleData = MarryLogic.GetOfflineRoleData(roleID);
			if (offlineRoleData != null && offlineRoleData.MyMarriageData != null)
			{
				if (offlineRoleData.MyMarriageData.byMarrytype != -1)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetSpouseID(int roleID)
		{
			RoleDataEx offlineRoleData = MarryLogic.GetOfflineRoleData(roleID);
			return (offlineRoleData != null && offlineRoleData.MyMarriageData != null) ? offlineRoleData.MyMarriageData.nSpouseID : -1;
		}

		public static string GetRoleName(int roleID)
		{
			RoleDataEx offlineRoleData = MarryLogic.GetOfflineRoleData(roleID);
			return (offlineRoleData != null) ? offlineRoleData.RoleName : "";
		}

		public static RoleDataEx GetOfflineRoleData(int roleID)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
			RoleDataEx result;
			if (null != gameClient)
			{
				result = gameClient.ClientData.GetRoleData();
			}
			else
			{
				SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleID);
				result = ((safeClientDataFromLocalOrDB != null) ? safeClientDataFromLocalOrDB.GetRoleData() : null);
			}
			return result;
		}

		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (safeClientDataFromLocalOrDB != null && safeClientDataFromLocalOrDB.MyMarriageData != null && safeClientDataFromLocalOrDB.MyMarriageData.nSpouseID != -1)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(safeClientDataFromLocalOrDB.MyMarriageData.nSpouseID);
					if (gameClient != null)
					{
						MarriageOtherLogic.getInstance().SendSpouseDataToClient(gameClient);
					}
					MarryPartyLogic.getInstance().OnChangeName(roleId, oldName, newName);
				}
			}
		}

		public static Dictionary<int, MarryApplyData> MarryApplyList = new Dictionary<int, MarryApplyData>();

		public static long NextPeriodicCheckTime = 0L;

		private static int MarryCost;

		private static int MarryCD;

		private static int MarryReplyTime;

		private static int DivorceCost;

		private static int DivorceForceCost;
	}
}
