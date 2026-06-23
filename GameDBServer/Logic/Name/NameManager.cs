using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Name
{
	public class NameManager : SingletonTemplate<NameManager>
	{
		private NameManager()
		{
		}

		public TCPProcessCmdResults ProcChangeName(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				int num = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				string text3 = array[3];
				int num3 = Convert.ToInt32(array[4]);
				int val = Convert.ToInt32(array[5]);
				int num4 = Convert.ToInt32(array[6]);
				int num5 = Convert.ToInt32(array[7]);
				string text4 = "";
				string arg = "";
				int num6 = 0;
				DBUserInfo dbuserInfo = null;
				DBRoleInfo dbroleInfo = null;
				ChangeNameType changeNameType = ChangeNameType.CNT_Unknown;
				ChangeNameError changeNameError = ChangeNameError.Success;
				bool flag = false;
				dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (dbuserInfo == null)
				{
					arg = "账号找不到 : " + text2;
					changeNameError = ChangeNameError.DBFailed;
				}
				else
				{
					lock (dbuserInfo)
					{
						int i;
						for (i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
						{
							if (dbuserInfo.ListRoleZoneIDs[i] == num && dbuserInfo.ListRoleIDs[i] == num2)
							{
								break;
							}
						}
						if (i == dbuserInfo.ListRoleIDs.Count)
						{
							arg = string.Concat(new object[]
							{
								"账号: ",
								text2,
								" 在 ",
								num.ToString(),
								" 区，不包含角色 ",
								num2
							});
							changeNameError = ChangeNameError.NotContainRole;
							goto IL_641;
						}
					}
					dbroleInfo = dbMgr.GetDBRoleInfo(ref num2);
					if (null == dbroleInfo)
					{
						arg = "查找不到dbroleinfo,roleid=" + num2.ToString();
						changeNameError = ChangeNameError.DBFailed;
					}
					else if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(num2))
					{
						arg = "处在预删除状态dbroleinfo,roleid=" + num2.ToString();
						changeNameError = ChangeNameError.DBFailed;
					}
					else
					{
						text4 = dbroleInfo.RoleName;
						int num7 = Global.GetRoleParamsInt32(dbroleInfo, "LeftFreeChangeNameTimes");
						if (num7 > 0 && 1 == num4)
						{
							changeNameType = ChangeNameType.CNT_Free;
						}
						else
						{
							changeNameType = ChangeNameType.CNT_ZuanShi;
						}
						if ((changeNameType == ChangeNameType.CNT_Free && 1 != num4) || (changeNameType == ChangeNameType.CNT_ZuanShi && 1 != num5))
						{
							arg = "服务器没有开放改名功能, " + changeNameType.ToString();
							changeNameError = ChangeNameError.ServerDenied;
						}
						else
						{
							SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(text4);
							if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(text3) || dbMgr.IsRolenameExist(text3) || !SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, text3))
							{
								arg = "新名字： " + text3 + " 已被占用";
								changeNameError = ChangeNameError.NameAlreayUsed;
							}
							else
							{
								flag = true;
								if (PreNamesManager.SetUsedPreName(text3))
								{
									DBWriter.UpdatePreNameUsedState(dbMgr, text3, 1);
								}
								string name = "";
								string value = "";
								if (changeNameType == ChangeNameType.CNT_Free)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("角色请求免费改名，roleid={0}, old name={1}, new name={2}", num2, text4, text3), null, true);
									num7 = Math.Max(0, num7 - 1);
									name = "LeftFreeChangeNameTimes";
									value = num7.ToString();
								}
								else if (changeNameType == ChangeNameType.CNT_ZuanShi)
								{
									int num8 = Global.GetRoleParamsInt32(dbroleInfo, "AlreadyZuanShiChangeNameTimes");
									num6 = Math.Min(val, num3 * (num8 + 1));
									LogManager.WriteLog(LogTypes.Error, string.Format("角色请求钻石改名，roleid={0}, old name={1}, new name={2}, costzuanshi={3}", new object[]
									{
										num2,
										text4,
										text3,
										num6
									}), null, true);
									lock (dbuserInfo)
									{
										if (dbuserInfo.Money < num6)
										{
											arg = "钻石不足";
											changeNameError = ChangeNameError.ZuanShiNotEnough;
											goto IL_641;
										}
										int money = dbuserInfo.Money;
										dbuserInfo.Money -= num6;
										if (!DBWriter.UpdateUserInfo(dbMgr, dbuserInfo))
										{
											dbuserInfo.Money = money;
											arg = string.Format("改名时更新用户的元宝失败，UserID={0}", dbuserInfo.UserID);
											changeNameError = ChangeNameError.DBFailed;
											goto IL_641;
										}
										num8++;
										name = "AlreadyZuanShiChangeNameTimes";
										value = num8.ToString();
									}
								}
								Global.UpdateRoleParamByName(dbMgr, dbroleInfo, name, value, null);
								string sql = string.Format("UPDATE t_roles SET rname='{0}' WHERE rid={1} AND userid='{2}' AND zoneid={3}", new object[]
								{
									text3,
									num2,
									text2,
									num
								});
								if (!this._Util_ExecNonQuery(dbMgr, sql))
								{
									arg = "更新t_roles的名字失败";
									changeNameError = ChangeNameError.DBFailed;
								}
								else
								{
									lock (dbuserInfo)
									{
										for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
										{
											if (dbuserInfo.ListRoleZoneIDs[i] == num && dbuserInfo.ListRoleIDs[i] == num2)
											{
												dbuserInfo.ListRoleNames[i] = text3;
												break;
											}
										}
									}
									lock (dbroleInfo)
									{
										text4 = dbroleInfo.RoleName;
										dbroleInfo.RoleName = text3;
									}
									changeNameError = ChangeNameError.Success;
								}
							}
						}
					}
				}
				IL_641:
				if (changeNameError == ChangeNameError.Success)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名成功，roleid={0}, old name={1}，new name={2}", num2, text4, text3), null, true);
					this.AddChangeNameDBRecord(dbMgr, num2, text4, text3, changeNameType, num6);
					this._OnChangeNameSuccess(dbMgr, num2, num, text4, text3);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名失败，roleid={0}, name={1}, reason={2}", num2, text4, arg), null, true);
					if (flag)
					{
						SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(text3);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)changeNameError,
					text4,
					num6,
					dbuserInfo.Money
				}), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private void _OnChangeNameSuccess(DBManager dbMgr, int roleId, int zoneId, string oldName, string newName)
		{
			DBManager.getInstance().DBRoleMgr.OnChangeName(roleId, zoneId, oldName, newName);
			FuBenHistManager.OnChangeName(roleId, oldName, newName);
			PaiHangManager.OnChangeName(roleId, oldName, newName);
			string sql = string.Format("UPDATE t_mail SET senderrname='{0}' WHERE senderrid={1}", newName, roleId);
			sql = string.Format("UPDATE t_mail SET reveiverrname='{0}' WHERE receiverrid={1}", newName, roleId);
			if (!this._Util_ExecNonQuery(dbMgr, sql))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("更新t_mail [reveiverrname] 失败, roleId={0}, oldName={1}, newName={2}", roleId, oldName, newName), null, true);
			}
		}

		private bool _Util_ExecNonQuery(DBManager dbMgr, string sql)
		{
			bool result = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				try
				{
					mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception)
				{
					result = false;
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
				result = true;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcQueryEachRoleInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				ChangeNameInfo changeNameInfo = new ChangeNameInfo();
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (dbuserInfo != null)
				{
					lock (dbuserInfo)
					{
						changeNameInfo.ZuanShi = dbuserInfo.Money;
						for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
						{
							if (dbuserInfo.ListRoleZoneIDs[i] == num)
							{
								int num2 = dbuserInfo.ListRoleIDs[i];
								DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num2);
								if (dbroleInfo != null)
								{
									int roleId = dbuserInfo.ListRoleIDs[i];
									int roleParamsInt = Global.GetRoleParamsInt32(dbroleInfo, "LeftFreeChangeNameTimes");
									int roleParamsInt2 = Global.GetRoleParamsInt32(dbroleInfo, "AlreadyZuanShiChangeNameTimes");
									changeNameInfo.RoleList.Add(new EachRoleChangeName
									{
										RoleId = roleId,
										LeftFreeTimes = roleParamsInt,
										AlreadyZuanShiTimes = roleParamsInt2
									});
								}
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ChangeNameInfo>(changeNameInfo, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private bool AddChangeNameDBRecord(DBManager dbMgr, int roleid, string oldName, string newName, ChangeNameType cnt, int costDiamond)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_change_name(roleid,oldname,newname,type,cost_diamond,time) VALUES({0},'{1}','{2}',{3},{4},'{5}')", new object[]
				{
					roleid,
					oldName,
					newName,
					(int)cnt,
					costDiamond,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public bool IsNameCanUseInDb(DBManager dbMgr, string name)
		{
			bool result;
			if (dbMgr == null || string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				string text = name + "99999999";
				try
				{
					int managedThreadId = Thread.CurrentThread.ManagedThreadId;
					string text2 = string.Format("REPLACE INTO t_name_check(`id`,`name`) VALUES({0},'{1}');", managedThreadId, text);
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
					MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand = new MySQLCommand(string.Format("SELECT name FROM t_name_check WHERE Id = {0};", managedThreadId), mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						string text3 = mySQLDataReader["name"].ToString();
						if (!string.IsNullOrEmpty(text3) && text3 == text)
						{
							return true;
						}
					}
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = false;
			}
			return result;
		}

		public TCPProcessCmdResults ProcChangeBangHuiName(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			string text2 = "";
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string text3 = array[2];
				EChangeGuildNameError echangeGuildNameError = EChangeGuildNameError.DBFailed;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					echangeGuildNameError = EChangeGuildNameError.DBFailed;
				}
				else
				{
					lock (dbroleInfo)
					{
						if (dbroleInfo.Faction != num2 || dbroleInfo.BHZhiWu != 1)
						{
							echangeGuildNameError = EChangeGuildNameError.OperatorDenied;
							goto IL_381;
						}
					}
					if (!this.IsNameCanUseInDb(dbMgr, text3))
					{
						echangeGuildNameError = EChangeGuildNameError.InvalidName;
					}
					else
					{
						BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
						if (bangHuiDetailData == null || bangHuiDetailData.CanModNameTimes <= 0 || bangHuiDetailData.BZRoleID != num)
						{
							echangeGuildNameError = EChangeGuildNameError.OperatorDenied;
						}
						else
						{
							text2 = bangHuiDetailData.BHName;
							SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(bangHuiDetailData.BHName);
							if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(text3) || dbMgr.IsBangHuiNameExist(text3))
							{
								echangeGuildNameError = EChangeGuildNameError.NameAlreadyUsed;
							}
							else
							{
								string sql = string.Format("UPDATE t_banghui SET bhname='{0}', can_mod_name_times={1} WHERE bhid={2}", text3, bangHuiDetailData.CanModNameTimes - 1, num2);
								if (!this._Util_ExecNonQuery(dbMgr, sql))
								{
									echangeGuildNameError = EChangeGuildNameError.DBFailed;
								}
								else
								{
									lock (dbroleInfo)
									{
										dbroleInfo.BHName = text3;
									}
									if (!DBWriter.UpdateAllRoleBangHuiName(dbMgr, num2, text3))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("更新帮会id={0}的名字 {1} => {2}，更新t_roles未(全部)成功", num2, bangHuiDetailData.BHName, text3), null, true);
									}
									List<DBRoleInfo> cachingDBRoleInfoListByFaction = dbMgr.DBRoleMgr.GetCachingDBRoleInfoListByFaction(num2);
									if (null != cachingDBRoleInfoListByFaction)
									{
										for (int i = 0; i < cachingDBRoleInfoListByFaction.Count; i++)
										{
											cachingDBRoleInfoListByFaction[i].BHName = text3;
										}
									}
									ZhanMengShiJianData zhanMengShiJianData = new ZhanMengShiJianData();
									zhanMengShiJianData.BHID = num2;
									zhanMengShiJianData.ShiJianType = ZhanMengShiJianConstants.ChangeName;
									zhanMengShiJianData.RoleName = dbroleInfo.RoleName;
									zhanMengShiJianData.SubSzValue1 = text3;
									zhanMengShiJianData.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
									ZhanMengShiJianManager.getInstance().onAddZhanMengShiJian(zhanMengShiJianData);
									string sql2 = string.Format("INSERT INTO t_change_name_banghui(bhid,by_role,old_name,new_name,time) VALUES({0},{1},'{2}','{3}','{4}')", new object[]
									{
										num2,
										num,
										text2,
										text3,
										DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
									});
									this._Util_ExecNonQuery(dbMgr, sql2);
									echangeGuildNameError = EChangeGuildNameError.Success;
								}
							}
						}
					}
				}
				IL_381:
				if (echangeGuildNameError == EChangeGuildNameError.Success)
				{
					GameDBManager.BangHuiLingDiMgr.OnChangeBangHuiName(num2, text2, text3);
					string gmCmd = string.Format("-synclingdi", new object[0]);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", (int)echangeGuildNameError), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcAddBangHuiChangeNameTimes(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string @string = new UTF8Encoding().GetString(data, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				int instance;
				if (null == dbroleInfo)
				{
					instance = -1;
				}
				else
				{
					int faction = dbroleInfo.Faction;
					string sql = string.Format("UPDATE t_banghui SET can_mod_name_times=can_mod_name_times+{0} WHERE bhid={1}", num2, faction);
					if (!this._Util_ExecNonQuery(dbMgr, sql))
					{
						instance = -3;
					}
					else
					{
						BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, faction);
						if (bangHuiDetailData == null)
						{
							instance = -2;
						}
						else
						{
							instance = bangHuiDetailData.CanModNameTimes;
						}
					}
				}
				byte[] array2 = DataHelper.ObjectToBytes<int>(instance);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array2, 0, array2.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
