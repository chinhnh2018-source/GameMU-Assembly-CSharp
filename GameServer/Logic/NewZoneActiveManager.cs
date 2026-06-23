using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class NewZoneActiveManager
	{
		public static TCPProcessCmdResults ProcessQueryLevelUpMadmanCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (NewZoneActiveManager.QueryLevelUpMadman(client, pool, nID, out tcpOutPacket))
				{
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessQueryLevelUpMadmanCmd", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static bool QueryLevelUpMadman(GameClient client, TCPOutPacketPool pool, int nID, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				KingActivity kingActivity = (KingActivity)Global.GetActivity(ActivityTypes.NewZoneUpLevelMadman);
				NewZoneUpLevelData newZoneUpLevelData = new NewZoneUpLevelData();
				int count = kingActivity.RoleLimit.Count;
				newZoneUpLevelData.Items = new List<NewZoneUpLevelItemData>();
				for (int i = 1; i < count + 1; i++)
				{
					NewZoneUpLevelItemData newZoneUpLevelItemData = new NewZoneUpLevelItemData();
					AwardItem award = kingActivity.GetAward(client, i);
					newZoneUpLevelItemData.LeftNum = award.MinAwardCondionValue2 - Global.GetChongJiLingQuShenZhuangQuota(client, i);
					newZoneUpLevelItemData.GetAward = !Global.CanGetChongJiLingQuShenZhuang(client, i);
					newZoneUpLevelData.Items.Add(newZoneUpLevelItemData);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<NewZoneUpLevelData>(newZoneUpLevelData, pool, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "LevelUpMadman", false, false);
			}
			return false;
		}

		public static TCPProcessCmdResults ProcessGetActiveInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int type = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				return Global.RequestToDBServer2(tcpClientPool, pool, nID, Global.GetActivityRequestCmdString((ActivityTypes)type, gameClient, 0), out tcpOutPacket, gameClient.ServerId);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static TCPProcessCmdResults GetNewLevelUpMadmanAward(GameClient client, TCPOutPacketPool pool, int nID, int nRoleID, int nActivityType, int nBtnIndex, out TCPOutPacket tcpOutPacket)
		{
			Activity activity = Global.GetActivity((ActivityTypes)nActivityType);
			TCPProcessCmdResults result;
			if (!activity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
			{
				string data = string.Format("{0}:{1}:0", -20, nActivityType);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				int num = Global.CalcOriginalOccupationID(client);
				int changeLifeCount = client.ClientData.ChangeLifeCount;
				AwardItem award = activity.GetAward(client, num, 1);
				if (award == null)
				{
					string data = string.Format("{0}:{1}:0", -1, nActivityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else if (!activity.CanGiveAward())
				{
					string data = string.Format("{0}:{1}:0", -10, nActivityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					award = activity.GetAward(client, nBtnIndex);
					if (changeLifeCount < award.MinAwardCondionValue)
					{
						string data = string.Format("{0}:{1}:0", -100, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else if (changeLifeCount == award.MinAwardCondionValue && client.ClientData.Level < award.MinAwardCondionValue3)
					{
						string data = string.Format("{0}:{1}:0", -101, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else if (!Global.CanGetChongJiLingQuShenZhuang(client, nBtnIndex))
					{
						string data = string.Format("{0}:{1}:0", -103, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else
					{
						int chongJiLingQuShenZhuangQuota = Global.GetChongJiLingQuShenZhuangQuota(client, nBtnIndex);
						if (chongJiLingQuShenZhuangQuota >= award.MinAwardCondionValue2)
						{
							string data = string.Format("{0}:{1}:0", -102, nActivityType);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
						else
						{
							activity.GiveAward(client, nBtnIndex, num);
							Global.CompleteChongJiLingQuShenZhuang(client, nBtnIndex, chongJiLingQuShenZhuangQuota + 1);
							AwardItem award2 = activity.GetAward(client, nBtnIndex, 2);
							if (award2 != null && award2.GoodsDataList.Count > 0)
							{
								Global.BroadcastChongJiLingQuShengZhuangHint(client, nBtnIndex, award2.GoodsDataList[num].GoodsID);
							}
							string data = string.Format("{0}:{1}:{2}", 1, nActivityType, nBtnIndex);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessGetActiveAwards(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num2 = Global.SafeConvertToInt32(array[1]);
				int nBtnIndex = Global.SafeConvertToInt32(array[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				switch (num2)
				{
				case 33:
					return NewZoneActiveManager.GetNewLevelUpMadmanAward(client, pool, nID, num, num2, nBtnIndex, out tcpOutPacket);
				case 34:
				case 35:
				case 36:
				case 37:
					return NewZoneActiveManager.GetActiveAwards(client, tcpClientPool, pool, nID, num, num2, out tcpOutPacket);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults GetActiveAwards(GameClient client, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, int roleID, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				Activity activity = Global.GetActivity((ActivityTypes)activityType);
				string data;
				if (null == activity)
				{
					data = string.Format("{0}:{1}:0", -1, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!activity.CanGiveAward())
				{
					data = string.Format("{0}:{1}:0", -10, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!activity.HasEnoughBagSpaceForAwardGoods(client))
				{
					data = string.Format("{0}:{1}:0", -20, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] array = null;
				int num = 629;
				string activityRequestCmdString = Global.GetActivityRequestCmdString((ActivityTypes)activityType, client, activityType);
				if (num <= 0 || string.IsNullOrEmpty(activityRequestCmdString))
				{
					data = string.Format("{0}:{1}:0", -4, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Global.RequestToDBServer(tcpClientPool, pool, num, activityRequestCmdString, out array, client.ServerId);
				if (array == null || array.Length != 3)
				{
					data = string.Format("{0}:{1}:0", -5, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = Global.SafeConvertToInt32(array[0]);
				if (num2 <= 0)
				{
					data = string.Format("{0}:{1}:0", num2, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!activity.GiveAward(client, Global.SafeConvertToInt32(array[1])))
				{
					data = string.Format("{0}:{1}:0", -7, activityType);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data = string.Format("{0}:{1}:{2}", 1, activityType, array[1]);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "GetActiveAwards", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
