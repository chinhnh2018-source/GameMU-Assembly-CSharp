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

namespace GameServer.Logic.YueKa
{
	public class YueKaManager
	{
		public static void LoadConfig()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(YueKaManager.YUE_KA_GOODS_FILE));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(YueKaManager.YUE_KA_GOODS_FILE));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", YueKaManager.YUE_KA_GOODS_FILE), null, true);
			}
			else
			{
				try
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							YueKaAward yueKaAward = new YueKaAward();
							yueKaAward.Init(xelement2);
							YueKaManager.AllGoodsDict[yueKaAward.Day] = yueKaAward;
						}
					}
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时异常{1}", YueKaManager.YUE_KA_GOODS_FILE, arg), null, true);
				}
			}
		}

		public static void HandleUserBuyYueKa(string userID, int roleID)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				LogManager.WriteLog(2, string.Format("HandleUserBuyYueKa, userid={0}, roleid={1}", userID, roleID), null, true);
				if (null != gameClient)
				{
					LogManager.WriteLog(2, string.Format("HandleUserBuyYueKa, 玩家在线, 在线的userid={0},  roleid={1}", userID, gameClient.ClientData.RoleID), null, true);
					Global.ProcessVipLevelUp(gameClient);
					lock (gameClient.ClientData.YKDetail)
					{
						if (gameClient.ClientData.YKDetail.HasYueKa == 0)
						{
							DateTime now = TimeUtil.NowDateTime();
							gameClient.ClientData.YKDetail.HasYueKa = 1;
							gameClient.ClientData.YKDetail.BegOffsetDay = Global.GetOffsetDay(now);
							gameClient.ClientData.YKDetail.EndOffsetDay = gameClient.ClientData.YKDetail.BegOffsetDay + YueKaManager.DAYS_PER_YUE_KA;
							gameClient.ClientData.YKDetail.CurOffsetDay = Global.GetOffsetDay(now);
							gameClient.ClientData.YKDetail.AwardInfo = "0";
						}
						else
						{
							gameClient.ClientData.YKDetail.EndOffsetDay += YueKaManager.DAYS_PER_YUE_KA;
						}
						GameManager.ClientMgr.NotifySelfParamsValueChange(gameClient, RoleCommonUseIntParamsIndexs.YueKaRemainDay, gameClient.ClientData.YKDetail.RemainDayOfYueKa());
						YueKaManager._UpdateYKDetail2DB(gameClient, gameClient.ClientData.YKDetail);
						if (gameClient._IconStateMgr.CheckFuLiYueKaFanLi(gameClient))
						{
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
						}
					}
				}
				else
				{
					LogManager.WriteLog(2, string.Format("玩家购买了月卡，但是处理的时候找不到在线角色, UserID={0}, last roldid={1}, 转交给db处理", userID, roleID), null, true);
					int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					string strcmd = string.Format("{0}:{1}:{2}", roleID, offsetDay, "YueKaInfo");
					string[] array = Global.ExecuteDBCmd(10181, strcmd, 0);
					if (array == null || array.Length != 1 || array[0] != "0")
					{
						LogManager.WriteLog(2, string.Format("玩家购买了月卡，但是处理的时候找不到在线角色, UserID={0}, last roldid={1}, 转交给db处理时失败了", userID, roleID), null, true);
					}
				}
			}
		}

		public static TCPProcessCmdResults ProcessGetYueKaData(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (1 != array.Length)
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
				YueKaData instance = null;
				lock (gameClient.ClientData.YKDetail)
				{
					instance = gameClient.ClientData.YKDetail.ToYueKaData();
				}
				GameManager.ClientMgr.SendToClient(gameClient, DataHelper.ObjectToBytes<YueKaData>(instance), nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetYueKaData", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessGetYueKaAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				YueKaError yueKaError = YueKaManager._GetYueKaAward(client, num2);
				string strCmd = string.Format("{0}:{1}:{2}", num, (int)yueKaError, num2);
				GameManager.ClientMgr.SendToClient(client, strCmd, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetYueKaData", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static YueKaError _GetYueKaAward(GameClient client, int day)
		{
			YueKaError result;
			if (GameFuncControlManager.IsGameFuncDisabled(3))
			{
				result = YueKaError.YK_CannotAward_HasNotYueKa;
			}
			else if (day <= 0 || day > YueKaManager.DAYS_PER_YUE_KA)
			{
				result = YueKaError.YK_CannotAward_ParamInvalid;
			}
			else
			{
				lock (client.ClientData.YKDetail)
				{
					if (client.ClientData.YKDetail.HasYueKa == 0)
					{
						return YueKaError.YK_CannotAward_HasNotYueKa;
					}
					if (day < client.ClientData.YKDetail.CurDayOfPerYueKa())
					{
						return YueKaError.YK_CannotAward_DayHasPassed;
					}
					if (day > client.ClientData.YKDetail.CurDayOfPerYueKa())
					{
						return YueKaError.YK_CannotAward_TimeNotReach;
					}
					string awardInfo = client.ClientData.YKDetail.AwardInfo;
					if (awardInfo.Length < day || awardInfo[day - 1] == '1')
					{
						return YueKaError.YK_CannotAward_AlreadyAward;
					}
					YueKaAward yueKaAward = null;
					YueKaManager.AllGoodsDict.TryGetValue(day, out yueKaAward);
					if (yueKaAward == null)
					{
						return YueKaError.YK_CannotAward_ConfigError;
					}
					List<GoodsData> goodsByOcc = yueKaAward.GetGoodsByOcc(Global.CalcOriginalOccupationID(client));
					if (goodsByOcc != null && goodsByOcc.Count > 0)
					{
						if (!Global.CanAddGoodsNum(client, goodsByOcc.Count))
						{
							return YueKaError.YK_CannotAward_BagNotEnough;
						}
						foreach (GoodsData goodsData in goodsByOcc)
						{
							goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("第{0}天月卡返利", yueKaAward.Day), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
						}
						client.ClientData.AddAwardRecord(RoleAwardMsg.YueKaoAward, goodsByOcc, false);
					}
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yueKaAward.BindZuanShi, string.Format("第{0}天月卡返利", yueKaAward.Day));
					client.ClientData.AddAwardRecord(RoleAwardMsg.YueKaoAward, MoneyTypes.BindYuanBao, yueKaAward.BindZuanShi);
					GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.YueKaoAward, "");
					client.ClientData.YKDetail.AwardInfo = awardInfo.Substring(0, day - 1) + "1";
					YueKaManager._UpdateYKDetail2DB(client, client.ClientData.YKDetail);
					if (client._IconStateMgr.CheckFuLiYueKaFanLi(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				result = YueKaError.YK_Success;
			}
			return result;
		}

		private static void _SendAward2Player(GameClient client, YueKaAward award)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				List<GoodsData> goodsByOcc = award.GetGoodsByOcc(Global.CalcOriginalOccupationID(client));
				if (!Global.CanAddGoodsNum(client, goodsByOcc.Count))
				{
					foreach (GoodsData goodsData in goodsByOcc)
					{
						Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(576, new object[0]), string.Format(GLang.GetLang(577, new object[0]), award.Day), 1.0);
					}
				}
				else
				{
					foreach (GoodsData goodsData2 in goodsByOcc)
					{
						goodsData2.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, string.Format("第{0}天月卡返利", award.Day), false, goodsData2.Endtime, goodsData2.AddPropIndex, goodsData2.BornIndex, goodsData2.Lucky, goodsData2.Strong, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, goodsData2.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, award.BindZuanShi, string.Format("第{0}天月卡返利", award.Day));
			}
		}

		public static void CheckValid(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				if (client != null)
				{
					lock (client.ClientData.YKDetail)
					{
						if (client.ClientData.YKDetail.HasYueKa != 0)
						{
							int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
							if (offsetDay >= client.ClientData.YKDetail.EndOffsetDay)
							{
								client.ClientData.YKDetail.HasYueKa = 0;
							}
							else
							{
								int num = client.ClientData.YKDetail.CurOffsetDay - client.ClientData.YKDetail.AwardInfo.Length + 1;
								if (offsetDay >= num + YueKaManager.DAYS_PER_YUE_KA)
								{
									client.ClientData.YKDetail.CurOffsetDay = offsetDay;
									client.ClientData.YKDetail.AwardInfo = "";
									for (int i = num + YueKaManager.DAYS_PER_YUE_KA; i <= offsetDay; i++)
									{
										YueKaDetail ykdetail2 = client.ClientData.YKDetail;
										ykdetail2.AwardInfo += "0";
									}
								}
								else
								{
									for (int i = client.ClientData.YKDetail.CurOffsetDay + 1; i <= offsetDay; i++)
									{
										YueKaDetail ykdetail3 = client.ClientData.YKDetail;
										ykdetail3.AwardInfo += "0";
									}
									client.ClientData.YKDetail.CurOffsetDay = offsetDay;
								}
							}
							YueKaManager._UpdateYKDetail2DB(client, client.ClientData.YKDetail);
						}
					}
				}
			}
		}

		private static void _UpdateYKDetail2DB(GameClient client, YueKaDetail YKDetail)
		{
			string valueString = client.ClientData.YKDetail.SerializeToString();
			Global.SaveRoleParamsStringToDB(client, "YueKaInfo", valueString, true);
		}

		public static void UpdateNewDay(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				if (client != null)
				{
					YueKaManager.CheckValid(client);
					lock (client.ClientData.YKDetail)
					{
						if (client._IconStateMgr.CheckFuLiYueKaFanLi(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.YueKaRemainDay, client.ClientData.YKDetail.RemainDayOfYueKa());
					}
				}
			}
		}

		public static int DAYS_PER_YUE_KA = 30;

		public static readonly int YUE_KA_MONEY_ID_IN_CHARGE_FILE = 10000;

		private static readonly string YUE_KA_GOODS_FILE = "Config/Activity/Card.xml";

		private static Dictionary<int, YueKaAward> AllGoodsDict = new Dictionary<int, YueKaAward>();
	}
}
