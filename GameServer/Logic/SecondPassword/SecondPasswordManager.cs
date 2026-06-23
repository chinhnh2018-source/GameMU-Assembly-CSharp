using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.SecondPassword
{
	public class SecondPasswordManager
	{
		public static long ValidSecWhenLogout
		{
			get
			{
				return SecondPasswordManager._ValidSecWhenLogout;
			}
			set
			{
				SecondPasswordManager._ValidSecWhenLogout = value;
				if (SecondPasswordManager._ValidSecWhenLogout < 0L)
				{
					SecondPasswordManager._ValidSecWhenLogout = 300L;
				}
			}
		}

		public static SecPwdState GetSecPwdState(string userid)
		{
			SecPwdState result;
			if (string.IsNullOrEmpty(userid))
			{
				result = null;
			}
			else
			{
				SecPwdState secPwdState = null;
				lock (SecondPasswordManager._UsrSecPwdDict)
				{
					SecondPasswordManager._UsrSecPwdDict.TryGetValue(userid, out secPwdState);
				}
				result = secPwdState;
			}
			return result;
		}

		public static void SetSecPwdState(string usrid, SecPwdState state)
		{
			if (!string.IsNullOrEmpty(usrid))
			{
				lock (SecondPasswordManager._UsrSecPwdDict)
				{
					if (state != null)
					{
						SecondPasswordManager._UsrSecPwdDict[usrid] = state;
					}
					else
					{
						SecondPasswordManager._UsrSecPwdDict.Remove(usrid);
					}
				}
			}
		}

		public static TCPProcessCmdResults ProcessSetSecPwd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				SetSecondPassword setSecondPassword = DataHelper.BytesToObject<SetSecondPassword>(data, 0, count);
				if (setSecondPassword == null)
				{
					LogManager.WriteLog(2, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != setSecondPassword.RoleID)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), setSecondPassword.RoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(3))
				{
					LogManager.WriteLog(2, string.Format("ProcessSetSecPwd功能尚未开放, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), setSecondPassword.RoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(gameClient.strUserID);
				string text = (secPwdState != null) ? secPwdState.SecPwd : null;
				SecondPasswordError secondPasswordError;
				if (!string.IsNullOrEmpty(text))
				{
					if (setSecondPassword.OldSecPwd == null || text != setSecondPassword.OldSecPwd)
					{
						secondPasswordError = SecondPasswordError.SecPwdVerifyFailed;
						goto IL_1F8;
					}
				}
				string text2 = SecondPasswordRC4.Decrypt(setSecondPassword.NewSecPwd);
				if (string.IsNullOrEmpty(text2))
				{
					secondPasswordError = SecondPasswordError.SecPwdIsNull;
				}
				else if (!Regex.IsMatch(text2, "^[a-zA-Z0-9_]+$"))
				{
					secondPasswordError = SecondPasswordError.SecPwdCharInvalid;
				}
				else if (text2.Length < 6)
				{
					secondPasswordError = SecondPasswordError.SecPwdIsTooShort;
				}
				else if (text2.Length > 8)
				{
					secondPasswordError = SecondPasswordError.SecPwdIsTooLong;
				}
				else
				{
					string strcmd = string.Format("{0}:{1}", gameClient.strUserID, setSecondPassword.NewSecPwd);
					string[] array = Global.ExecuteDBCmd(10183, strcmd, gameClient.ServerId);
					if (array == null || array.Length != 2)
					{
						secondPasswordError = SecondPasswordError.SecPwdDBFailed;
					}
					else
					{
						secondPasswordError = SecondPasswordError.SecPwdSetSuccess;
					}
				}
				IL_1F8:
				if (secondPasswordError == SecondPasswordError.SecPwdSetSuccess)
				{
					if (secPwdState == null)
					{
						secPwdState = new SecPwdState();
					}
					secPwdState.SecPwd = setSecondPassword.NewSecPwd;
					secPwdState.NeedVerify = false;
					SecondPasswordManager.SetSecPwdState(gameClient.strUserID, secPwdState);
				}
				int num = 0;
				int num2 = 0;
				if (secPwdState != null)
				{
					num = 1;
					num2 = (secPwdState.NeedVerify ? 1 : 0);
				}
				string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					setSecondPassword.RoleID,
					(int)secondPasswordError,
					num,
					num2
				});
				GameManager.ClientMgr.SendToClient(gameClient, strCmd, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcClrSecPwd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				VerifySecondPassword verifySecondPassword = DataHelper.BytesToObject<VerifySecondPassword>(data, 0, count);
				if (verifySecondPassword == null)
				{
					LogManager.WriteLog(2, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string text = GameManager.OnlineUserSession.FindUserID(socket);
				if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(verifySecondPassword.UserID) || text != verifySecondPassword.UserID)
				{
					LogManager.WriteLog(2, string.Format("玩家请求清除二级密码，但是玩家发送的uid错误, {0}", Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(verifySecondPassword.UserID);
				int num;
				if (secPwdState == null)
				{
					num = 2;
				}
				else if (string.IsNullOrEmpty(verifySecondPassword.SecPwd) || verifySecondPassword.SecPwd != secPwdState.SecPwd)
				{
					num = 1;
				}
				else if (!SecondPasswordManager.ClearUserSecPwd(verifySecondPassword.UserID))
				{
					num = 8;
				}
				else
				{
					num = 9;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, num.ToString(), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static void OnUsrLogout(string userID)
		{
			SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(userID);
			if (secPwdState != null)
			{
				secPwdState.AuthDeadTime = TimeUtil.NowDateTime().AddSeconds((double)((int)SecondPasswordManager.ValidSecWhenLogout));
				SecondPasswordManager.SetSecPwdState(userID, secPwdState);
			}
		}

		public static SecPwdState InitUserState(string userID, bool alreadyOnline)
		{
			SecPwdState result;
			if (GameFuncControlManager.IsGameFuncDisabled(3))
			{
				result = null;
			}
			else if (string.IsNullOrEmpty(userID))
			{
				result = null;
			}
			else
			{
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(userID);
				if (secPwdState == null)
				{
					string[] array = Global.ExecuteDBCmd(10184, userID, 0);
					if (array != null && array.Length == 2 && !string.IsNullOrEmpty(array[1]))
					{
						secPwdState = new SecPwdState();
						secPwdState.SecPwd = array[1];
						secPwdState.NeedVerify = true;
					}
				}
				else
				{
					if (alreadyOnline)
					{
						secPwdState.NeedVerify = true;
					}
					if (!secPwdState.NeedVerify)
					{
						if (TimeUtil.NowDateTime() > secPwdState.AuthDeadTime)
						{
							secPwdState.NeedVerify = true;
						}
					}
				}
				if (secPwdState != null)
				{
					SecondPasswordManager.SetSecPwdState(userID, secPwdState);
				}
				result = secPwdState;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessUsrCheckState(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (2 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string userid = array[0];
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(userid);
				string data2;
				if (secPwdState != null)
				{
					data2 = string.Format("{0}:{1}", 1, secPwdState.NeedVerify ? 1 : 0);
				}
				else
				{
					data2 = string.Format("{0}:{1}", 0, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessUsrVerify(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				VerifySecondPassword verifySecondPassword = DataHelper.BytesToObject<VerifySecondPassword>(data, 0, count);
				if (verifySecondPassword == null)
				{
					LogManager.WriteLog(2, string.Format("解析指令错误, cmd={0}", nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(verifySecondPassword.UserID);
				int num;
				int num2;
				int num3;
				if (secPwdState == null)
				{
					num = 0;
					num2 = 0;
					num3 = 0;
				}
				else if (string.IsNullOrEmpty(verifySecondPassword.SecPwd) || verifySecondPassword.SecPwd != secPwdState.SecPwd)
				{
					num = 1;
					num2 = 1;
					num3 = 1;
				}
				else
				{
					num = 0;
					num2 = 1;
					num3 = 0;
					secPwdState.NeedVerify = false;
				}
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static bool Update2DB(string useid, string secpwd)
		{
			string strcmd = string.Format("{0}:{1}", useid, secpwd);
			string[] array = Global.ExecuteDBCmd(10183, strcmd, 0);
			return array != null && array.Length == 2;
		}

		private static bool Clear2DB(string userid)
		{
			return SecondPasswordManager.Update2DB(userid, "");
		}

		public static bool ClearUserSecPwd(string usrid)
		{
			bool result;
			if (string.IsNullOrEmpty(usrid))
			{
				result = false;
			}
			else
			{
				TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(usrid);
				GameClient gameClient = null;
				if (null != tmsksocket)
				{
					gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
				}
				if (gameClient != null)
				{
					SecPwdState secPwdState = SecondPasswordManager.GetSecPwdState(usrid);
					if (secPwdState == null)
					{
						result = true;
					}
					else if (SecondPasswordManager.Clear2DB(usrid))
					{
						SecondPasswordManager.SetSecPwdState(usrid, null);
						int num = 0;
						int num2 = 0;
						string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							gameClient.ClientData.RoleID,
							7,
							num,
							num2
						});
						GameManager.ClientMgr.SendToClient(gameClient, strCmd, 861);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else if (SecondPasswordManager.Clear2DB(usrid))
				{
					SecondPasswordManager.SetSecPwdState(usrid, null);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private const int _PwdMinLen = 6;

		private const int _PwdMaxLen = 8;

		private static long _ValidSecWhenLogout = 300L;

		private static Dictionary<string, SecPwdState> _UsrSecPwdDict = new Dictionary<string, SecPwdState>();
	}
}
