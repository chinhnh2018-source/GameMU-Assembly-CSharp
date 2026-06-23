using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class QingGongYanManager
	{
		public void LoadQingGongYanConfig()
		{
			lock (this._QingGongYanMutex)
			{
				this.QingGongYanDict.Clear();
				string uri = "Config/GleeFeastAward.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						QingGongYanInfo qingGongYanInfo = new QingGongYanInfo();
						qingGongYanInfo.Index = (int)Global.GetSafeAttributeLong(xml, "ID");
						qingGongYanInfo.NpcID = (int)Global.GetSafeAttributeLong(xml, "NPCID");
						qingGongYanInfo.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						qingGongYanInfo.X = (int)Global.GetSafeAttributeLong(xml, "X");
						qingGongYanInfo.Y = (int)Global.GetSafeAttributeLong(xml, "Y");
						qingGongYanInfo.Direction = (int)Global.GetSafeAttributeLong(xml, "Direction");
						string[] array = Global.GetSafeAttributeStr(xml, "ProhibitedTime").Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < array.Length; i++)
						{
							qingGongYanInfo.ProhibitedTimeList.Add(array[i]);
						}
						qingGongYanInfo.BeginTime = Global.GetSafeAttributeStr(xml, "BeginTime");
						qingGongYanInfo.OverTime = Global.GetSafeAttributeStr(xml, "OverTime");
						qingGongYanInfo.FunctionID = (int)Global.GetSafeAttributeLong(xml, "FunctionID");
						qingGongYanInfo.HoldBindJinBi = (int)Global.GetSafeAttributeLong(xml, "ConductBindJinBi");
						qingGongYanInfo.TotalNum = (int)Global.GetSafeAttributeLong(xml, "SumNum");
						qingGongYanInfo.SingleNum = (int)Global.GetSafeAttributeLong(xml, "UseNum");
						qingGongYanInfo.JoinBindJinBi = (int)Global.GetSafeAttributeLong(xml, "BindJinBi");
						qingGongYanInfo.ExpAward = (int)Global.GetSafeAttributeLong(xml, "EXPAward");
						qingGongYanInfo.XingHunAward = (int)Global.GetSafeAttributeLong(xml, "XingHunAward");
						qingGongYanInfo.ZhanGongAward = (int)Global.GetSafeAttributeLong(xml, "ZhanGongAward");
						qingGongYanInfo.ZuanShiCoe = (int)Global.GetSafeAttributeLong(xml, "ZuanShiRatio");
						this.QingGongYanDict[qingGongYanInfo.Index] = qingGongYanInfo;
					}
				}
			}
		}

		private QingGongYanInfo GetQingGongYanConfig(int index)
		{
			QingGongYanInfo result = null;
			lock (this._QingGongYanMutex)
			{
				if (this.QingGongYanDict.ContainsKey(index))
				{
					result = this.QingGongYanDict[index];
				}
			}
			return result;
		}

		public QingGongYanResult HoldQingGongYan(GameClient client, int index, int onlyCheck = 0)
		{
			QingGongYanResult result;
			if (!Global.IsKingCityLeader(client))
			{
				result = QingGongYanResult.NotKing;
			}
			else
			{
				QingGongYanInfo qingGongYanConfig = this.GetQingGongYanConfig(index);
				if (null == qingGongYanConfig)
				{
					result = QingGongYanResult.ErrorParam;
				}
				else if (qingGongYanConfig.IfBanTime(TimeUtil.NowDateTime()))
				{
					result = QingGongYanResult.OutTime;
				}
				else
				{
					int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
					int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (gameConfigItemInt == offsetDay && TimeUtil.NowDateTime() <= DateTime.Parse(qingGongYanConfig.OverTime))
					{
						result = QingGongYanResult.RepeatHold;
					}
					else
					{
						int num;
						if (TimeUtil.NowDateTime() < DateTime.Parse(qingGongYanConfig.BeginTime))
						{
							num = offsetDay;
						}
						else
						{
							num = offsetDay + 1;
						}
						if (num == gameConfigItemInt)
						{
							result = QingGongYanResult.RepeatHold;
						}
						else
						{
							if (qingGongYanConfig.HoldBindJinBi > 0)
							{
								if (qingGongYanConfig.HoldBindJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
								{
									return QingGongYanResult.MoneyNotEnough;
								}
							}
							if (onlyCheck > 0)
							{
								result = QingGongYanResult.CheckSuccess;
							}
							else
							{
								if (qingGongYanConfig.HoldBindJinBi > 0)
								{
									if (!Global.SubBindTongQianAndTongQian(client, qingGongYanConfig.HoldBindJinBi, "举办庆功宴"))
									{
										return QingGongYanResult.MoneyNotEnough;
									}
								}
								Global.UpdateDBGameConfigg("qinggongyan_roleid", client.ClientData.RoleID.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", client.ClientData.RoleID.ToString());
								BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
								if (null != bangHuiMiniData)
								{
									Global.UpdateDBGameConfigg("qinggongyan_guildname", bangHuiMiniData.BHName);
									GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", bangHuiMiniData.BHName);
								}
								else
								{
									Global.UpdateDBGameConfigg("qinggongyan_guildname", "");
									GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", "");
								}
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", client.ClientData.RoleName);
								Global.UpdateDBGameConfigg("qinggongyan_startday", num.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", num.ToString());
								Global.UpdateDBGameConfigg("qinggongyan_grade", index.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", index.ToString());
								Global.UpdateDBGameConfigg("qinggongyan_joincount", "0");
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "0");
								Global.UpdateDBGameConfigg("qinggongyan_joinmoney", "0");
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "0");
								Global.UpdateDBGameConfigg("qinggongyan_jubanmoney", qingGongYanConfig.HoldBindJinBi.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_jubanmoney", qingGongYanConfig.HoldBindJinBi.ToString());
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "举办庆功宴", num.ToString(), "", client.ClientData.RoleName, "", index, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								EventLogManager.AddRoleEvent(client, OpTypes.Hold, OpTags.QingGongYan, LogRecordType.OffsetDayId, new object[]
								{
									num
								});
								result = QingGongYanResult.Success;
							}
						}
					}
				}
			}
			return result;
		}

		private bool IfNeedOpenQingGongYan()
		{
			QingGongYanInfo infoData = this.GetInfoData();
			bool result;
			if (null == infoData)
			{
				result = false;
			}
			else
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				result = (gameConfigItemInt == offsetDay && !(TimeUtil.NowDateTime() < DateTime.Parse(infoData.BeginTime)) && !(TimeUtil.NowDateTime() > DateTime.Parse(infoData.OverTime)) && !this.QingGongYanOpenFlag);
			}
			return result;
		}

		private bool IfNeedCloseQingGongYan()
		{
			QingGongYanInfo infoData = this.GetInfoData();
			bool result;
			if (null == infoData)
			{
				result = false;
			}
			else
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				result = (gameConfigItemInt == offsetDay && !(TimeUtil.NowDateTime() <= DateTime.Parse(infoData.OverTime)) && this.QingGongYanOpenFlag);
			}
			return result;
		}

		public void CheckQingGongYan(long ticks)
		{
			if (ticks - this.lastProcessTicks >= 10000L)
			{
				this.lastProcessTicks = ticks;
				if (this.IfNeedOpenQingGongYan())
				{
					this.OpenQingGongYan();
				}
				if (this.IfNeedCloseQingGongYan())
				{
					this.CloseQingGongYan();
				}
			}
		}

		private void OpenQingGongYan()
		{
			this.QingGongYanOpenFlag = true;
			QingGongYanInfo infoData = this.GetInfoData();
			if (null != infoData)
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[infoData.MapCode];
				NPC npcfromConfig = NPCGeneralManager.GetNPCFromConfig(infoData.MapCode, infoData.NpcID, infoData.X, infoData.Y, infoData.Direction);
				if (null != npcfromConfig)
				{
					if (NPCGeneralManager.AddNpcToMap(npcfromConfig))
					{
						GameManager.ClientMgr.BroadcastServerCmd(733, "1", false);
						this.QingGongYanNpc = npcfromConfig;
						string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_guildname", "");
						string msgText = StringUtil.substitute(GLang.GetLang(524, new object[0]), new object[]
						{
							gameConfigItemStr
						});
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox, 0, 0, 100, 100);
					}
					else
					{
						LogManager.WriteLog(2, string.Format("OpenQingGongYan, AddNpcToMap Faild !InfoData.MapCode={0}, InfoData.NpcID={1}", infoData.MapCode, infoData.NpcID), null, true);
					}
				}
			}
		}

		private QingGongYanInfo GetInfoData()
		{
			int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_grade", 0);
			QingGongYanInfo result;
			if (gameConfigItemInt <= 0)
			{
				result = null;
			}
			else
			{
				result = this.GetQingGongYanConfig(gameConfigItemInt);
			}
			return result;
		}

		private void CloseQingGongYan()
		{
			if (null != this.QingGongYanNpc)
			{
				NPCGeneralManager.RemoveMapNpc(this.QingGongYanNpc.MapCode, this.QingGongYanNpc.NpcID);
				this.QingGongYanNpc = null;
				GameManager.ClientMgr.BroadcastServerCmd(733, "0", false);
			}
			this.QingGongYanOpenFlag = false;
			QingGongYanInfo infoData = this.GetInfoData();
			if (null != infoData)
			{
				if (infoData.ZuanShiCoe > 0)
				{
					int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joinmoney", 0);
					int num = gameConfigItemInt / infoData.ZuanShiCoe;
					int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_roleid", 0);
					if (gameConfigItemInt2 > 0)
					{
						string sContent = string.Format(GLang.GetLang(525, new object[0]), new object[]
						{
							TimeUtil.NowDateTime().Year,
							TimeUtil.NowDateTime().Month,
							TimeUtil.NowDateTime().Day,
							DateTime.Parse(infoData.BeginTime).Hour,
							DateTime.Parse(infoData.BeginTime).Minute,
							num
						});
						Global.UseMailGivePlayerAward3(gameConfigItemInt2, null, GLang.GetLang(526, new object[0]), sContent, num, 0, 0);
						Global.UpdateDBGameConfigg("qinggongyan_roleid", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", "");
						Global.UpdateDBGameConfigg("qinggongyan_guildname", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", "");
						Global.UpdateDBGameConfigg("qinggongyan_startday", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", "");
						Global.UpdateDBGameConfigg("qinggongyan_grade", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", "");
						Global.UpdateDBGameConfigg("qinggongyan_joincount", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "");
						Global.UpdateDBGameConfigg("qinggongyan_joinmoney", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "");
						Global.UpdateDBGameConfigg("qinggongyan_jubanmoney", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_jubanmoney", "");
						string msgText = StringUtil.substitute(GLang.GetLang(527, new object[0]), new object[0]);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox, 0, 0, 100, 100);
					}
				}
			}
		}

		public QingGongYanResult JoinQingGongYan(GameClient client)
		{
			QingGongYanResult result;
			if (null == this.QingGongYanNpc)
			{
				result = QingGongYanResult.OutTime;
			}
			else
			{
				QingGongYanInfo infoData = this.GetInfoData();
				if (null == infoData)
				{
					result = QingGongYanResult.OutTime;
				}
				else
				{
					int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joincount", 0);
					if (gameConfigItemInt > 0)
					{
						if (gameConfigItemInt >= infoData.TotalNum)
						{
							return QingGongYanResult.TotalNotEnough;
						}
					}
					if (infoData.JoinBindJinBi > 0)
					{
						if (infoData.JoinBindJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
						{
							return QingGongYanResult.MoneyNotEnough;
						}
					}
					string roleParamByName = Global.GetRoleParamByName(client, "QingGongYanJoinFlag");
					int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					int num = 0;
					int num2 = 0;
					if (null != roleParamByName)
					{
						string[] array = roleParamByName.Split(new char[]
						{
							','
						});
						if (2 == array.Length)
						{
							num = Convert.ToInt32(array[0]);
							num2 = Convert.ToInt32(array[1]);
						}
					}
					if (offsetDay != num)
					{
						num2 = 0;
					}
					if (infoData.SingleNum > 0)
					{
						if (num2 >= infoData.SingleNum)
						{
							return QingGongYanResult.CountNotEnough;
						}
					}
					if (infoData.JoinBindJinBi > 0)
					{
						if (!Global.SubBindTongQianAndTongQian(client, infoData.JoinBindJinBi, "参加庆功宴"))
						{
							return QingGongYanResult.MoneyNotEnough;
						}
					}
					string value = offsetDay.ToString() + "," + (num2 + 1).ToString();
					Global.UpdateRoleParamByName(client, "QingGongYanJoinFlag", value, true);
					Global.UpdateDBGameConfigg("qinggongyan_joincount", (gameConfigItemInt + 1).ToString());
					GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", (gameConfigItemInt + 1).ToString());
					int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joinmoney", 0);
					Global.UpdateDBGameConfigg("qinggongyan_joinmoney", (gameConfigItemInt2 + infoData.JoinBindJinBi).ToString());
					GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", (gameConfigItemInt2 + infoData.JoinBindJinBi).ToString());
					if (infoData.ExpAward > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)infoData.ExpAward, true, true, false, "none");
					}
					if (infoData.XingHunAward > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(client, infoData.XingHunAward, "庆功宴", true, true);
					}
					if (infoData.ZhanGongAward > 0)
					{
						int zhanGongAward = infoData.ZhanGongAward;
						if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref zhanGongAward, AddBangGongTypes.BG_QGY, 0))
						{
							if (0 != zhanGongAward)
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "罗兰宴会领取", "系统", client.ClientData.RoleName, "增加", zhanGongAward, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
							}
						}
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取帮贡, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.BangGong,
							zhanGongAward
						}), EventLevels.Record);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "参加庆功宴", "", "", client.ClientData.RoleName, "", 1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
					EventLogManager.AddRoleEvent(client, OpTypes.Join, OpTags.QingGongYan, LogRecordType.OffsetDayId, new object[]
					{
						offsetDay
					});
					result = QingGongYanResult.Success;
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcessHoldQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int index = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				QingGongYanResult qingGongYanResult = this.HoldQingGongYan(client, index, num2);
				string data2;
				if (qingGongYanResult != QingGongYanResult.Success)
				{
					data2 = string.Format("{0}:{1}:{2}", (int)qingGongYanResult, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}", 0, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessHoldQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessQueryQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
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
				int num2 = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_grade", "0"));
				int num3 = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joincount", "0"));
				int num4 = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joinmoney", "0"));
				string roleParamByName = Global.GetRoleParamByName(client, "QingGongYanJoinFlag");
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				int num5 = 0;
				int num6 = 0;
				if (null != roleParamByName)
				{
					string[] array2 = roleParamByName.Split(new char[]
					{
						','
					});
					if (2 == array2.Length)
					{
						num5 = Convert.ToInt32(array2[0]);
						num6 = Convert.ToInt32(array2[1]);
					}
				}
				if (offsetDay != num5)
				{
					num6 = 0;
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					num6,
					num3,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessHoldQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessJoinQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
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
				QingGongYanResult qingGongYanResult = this.JoinQingGongYan(client);
				string data2;
				if (qingGongYanResult != QingGongYanResult.Success)
				{
					data2 = string.Format("{0}:{1}", (int)qingGongYanResult, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", 0, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessCMD_SPR_QueryQingGongYanOpenCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string @string = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				int num = (this.QingGongYanNpc == null) ? 0 : 1;
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private object _QingGongYanMutex = new object();

		private Dictionary<int, QingGongYanInfo> QingGongYanDict = new Dictionary<int, QingGongYanInfo>();

		private bool QingGongYanOpenFlag = false;

		private NPC QingGongYanNpc = null;

		private long lastProcessTicks = 0L;
	}
}
