using System;
using System.Text;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.UnionAlly;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Name
{
	public class NameManager : SingletonTemplate<NameManager>
	{
		private NameManager()
		{
		}

		public void LoadConfig()
		{
			try
			{
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("NameLengthRange", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length >= 2)
				{
					this.NameMinLen = paramValueIntArrayByName[0];
					this.NameMaxLen = paramValueIntArrayByName[1];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "NameManager.LoadConfig", ex, true);
				this.NameMinLen = 2;
				this.NameMaxLen = 7;
			}
		}

		public TCPProcessCmdResults ProcessChangeName(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string text2 = array[2];
				string text3 = GameManager.OnlineUserSession.FindUserID(socket);
				if (string.IsNullOrEmpty(text3))
				{
					LogManager.WriteLog(2, string.Format("角色改名时，找不到socket对应的uid，其中roleid={0}，zoneid={1}，newname={2}", num, num2, text2), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ChangeNameResult changeNameResult = new ChangeNameResult();
				if (socket.IsKuaFuLogin || GameManager.ClientMgr.FindClient(socket) != null)
				{
					changeNameResult.ErrCode = 11;
				}
				else
				{
					changeNameResult.ErrCode = (int)this.HandleChangeName(text3, num2, num, text2);
				}
				changeNameResult.ZoneId = num2;
				changeNameResult.NewName = text2;
				changeNameResult.NameInfo = this.GetChangeNameInfo(text3, num2, socket.ServerId);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ChangeNameResult>(changeNameResult, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private ChangeNameError HandleChangeName(string uid, int zoneId, int roleId, string newName)
		{
			ChangeNameError result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = ChangeNameError.ServerDenied;
			}
			else
			{
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(uid);
				if (secPwdState != null && secPwdState.NeedVerify)
				{
					result = ChangeNameError.NeedVerifySecPwd;
				}
				else if (string.IsNullOrEmpty(newName) || NameServerNamager.CheckInvalidCharacters(newName, false) <= 0)
				{
					result = ChangeNameError.InvalidName;
				}
				else if (!this.IsNameLengthOK(newName))
				{
					result = ChangeNameError.NameLengthError;
				}
				else if (NameServerNamager.RegisterNameToNameServer(zoneId, uid, new string[]
				{
					newName
				}, 0, roleId) <= 0)
				{
					result = ChangeNameError.NameAlreayUsed;
				}
				else
				{
					int num = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("FreeModName") ? 1 : 0;
					int num2 = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("ZuanShiModName") ? 1 : 0;
					string[] array = Global.ExecuteDBCmd(14001, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
					{
						uid,
						zoneId,
						roleId,
						newName,
						this.CostZuanShiBase,
						this.CostZuanShiMax,
						num,
						num2
					}), 0);
					if (array == null || array.Length != 4)
					{
						result = ChangeNameError.DBFailed;
					}
					else
					{
						int num3 = Convert.ToInt32(array[0]);
						string text = array[1];
						int num4 = Convert.ToInt32(array[2]);
						int num5 = Convert.ToInt32(array[3]);
						if (num3 == 0)
						{
							if (num4 > 0)
							{
								string reason = "改名 " + text + " -> " + newName;
								EventLogManager.AddResourceEvent(uid, zoneId, roleId, MoneyTypes.YuanBao, (long)(-(long)num4), (long)num5, reason);
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "改名", text, newName, "减少", num4, zoneId, uid, num5, 0, null);
							}
							this._OnChangeNameSuccess(roleId, text, newName);
						}
						result = (ChangeNameError)num3;
					}
				}
			}
			return result;
		}

		private void _OnChangeNameSuccess(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				RoleName2IDs.OnChangeName(roleId, oldName, newName);
				MarryLogic.OnChangeName(roleId, oldName, newName);
				GameManager.ArenaBattleMgr.OnChangeName(roleId, oldName, newName);
				if (LuoLanChengZhanManager.getInstance().GetLuoLanChengZhuRoleID() == roleId)
				{
					LuoLanChengZhanManager.getInstance().OnChangeName(roleId, oldName, newName);
				}
				GameManager.BloodCastleCopySceneMgr.OnChangeName(roleId, oldName, newName);
				GameManager.DaimonSquareCopySceneMgr.OnChangeName(roleId, oldName, newName);
				GameManager.BattleMgr.OnChangeName(roleId, oldName, newName);
				GameManager.AngelTempleMgr.OnChangeName(roleId, oldName, newName);
				MonsterBossManager.OnChangeName(roleId, oldName, newName);
				JieRiGiveKingActivity jieriGiveKingActivity = HuodongCachingMgr.GetJieriGiveKingActivity();
				if (jieriGiveKingActivity != null)
				{
					jieriGiveKingActivity.OnChangeName(roleId, oldName, newName);
				}
				JieRiRecvKingActivity jieriRecvKingActivity = HuodongCachingMgr.GetJieriRecvKingActivity();
				if (jieriRecvKingActivity != null)
				{
					jieriRecvKingActivity.OnChangeName(roleId, oldName, newName);
				}
				AllyManager.getInstance().UnionLeaderChangName(roleId, oldName, newName);
				JunTuanManager.getInstance().OnRoleChangName(roleId, oldName, newName);
				CompManager.getInstance().OnChangeName(roleId, oldName, newName);
				RebornManager.getInstance().OnChangeName(roleId, oldName, newName);
			}
		}

		public bool IsNameLengthOK(string name)
		{
			return !string.IsNullOrEmpty(name) && name.Length >= this.NameMinLen && name.Length <= this.NameMaxLen;
		}

		public ChangeNameInfo GetChangeNameInfo(string uid, int zoneId, int serverId)
		{
			return Global.sendToDB<ChangeNameInfo, string>(14002, string.Format("{0}:{1}", uid, zoneId), serverId);
		}

		public void GM_ChangeNameTest(GameClient client, string newName)
		{
		}

		public void GM_SetFreeModName(int roleid, int count)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
			if (gameClient != null)
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(gameClient, "LeftFreeChangeNameTimes");
				int nValue = count + roleParamsInt32FromDB;
				Global.SaveRoleParamsInt32ValueToDB(gameClient, "LeftFreeChangeNameTimes", nValue, true);
			}
			else
			{
				Global.UpdateRoleParamByNameOffline(roleid, "LeftFreeChangeNameTimes", count.ToString(), 0);
			}
		}

		public void GM_ChangeBangHuiName(GameClient client, string newName)
		{
			if (client != null)
			{
				this.HandleChangeBangHuiName(client, newName);
			}
		}

		public TCPProcessCmdResults ProcessChangeBangHuiName(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientSocket.IsKuaFuLogin)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (gameClient.ClientData.Faction <= 0)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EChangeGuildNameError echangeGuildNameError = this.HandleChangeBangHuiName(gameClient, text2);
				string data2 = string.Format("{0}:{1}:{2}", (int)echangeGuildNameError, gameClient.ClientData.Faction, text2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private EChangeGuildNameError HandleChangeBangHuiName(GameClient client, string newName)
		{
			EChangeGuildNameError echangeGuildNameError;
			if (string.IsNullOrEmpty(newName) || NameServerNamager.CheckInvalidCharacters(newName, false) <= 0)
			{
				echangeGuildNameError = EChangeGuildNameError.InvalidName;
			}
			else if (!this.IsNameLengthOK(newName))
			{
				echangeGuildNameError = EChangeGuildNameError.LengthError;
			}
			else
			{
				string[] array = Global.ExecuteDBCmd(14006, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, newName), client.ServerId);
				if (array == null || array.Length < 1)
				{
					echangeGuildNameError = EChangeGuildNameError.DBFailed;
				}
				else
				{
					echangeGuildNameError = (EChangeGuildNameError)Convert.ToInt32(array[0]);
				}
			}
			if (echangeGuildNameError == EChangeGuildNameError.Success)
			{
				client.ClientData.BHName = newName;
				GameManager.ClientMgr.NotifyBangHuiChangeName(client.ClientData.Faction, newName);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				Global.UpdateBangHuiMiniDataName(client.ClientData.Faction, newName);
				LuoLanChengZhanManager.getInstance().ReShowLuolanKing(0);
				if (GameManager.ArenaBattleMgr.GetPKKingRoleID() == client.ClientData.RoleID)
				{
					GameManager.ArenaBattleMgr.ReShowPKKing();
				}
				AllyManager.getInstance().UnionDataChange(client.ClientData.Faction, client.ServerId, false, 0);
				JunTuanManager.getInstance().OnBangHuiChangeName(client.ClientData.Faction, newName);
			}
			return echangeGuildNameError;
		}

		private int NameMinLen = 2;

		private int NameMaxLen = 7;

		public int CostZuanShiBase = 300;

		public int CostZuanShiMax = 1500;
	}
}
