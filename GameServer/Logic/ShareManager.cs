using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	internal class ShareManager
	{
		public static List<GoodsData> ShareGoodslist
		{
			get
			{
				List<GoodsData> shareGoodslist;
				if (ShareManager._ShareGoodslist != null && ShareManager._ShareGoodslist.Count > 0)
				{
					shareGoodslist = ShareManager._ShareGoodslist;
				}
				else
				{
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ShareAward");
					lock (ShareManager._ShareGoodsMutex)
					{
						ShareManager._ShareGoodslist = ShareManager.ParseGoodsDataList(paramValueByName.Split(new char[]
						{
							'|'
						}));
					}
					shareGoodslist = ShareManager._ShareGoodslist;
				}
				return shareGoodslist;
			}
		}

		private static List<GoodsData> ParseGoodsDataList(string[] fields)
		{
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] array = fields[i].Split(new char[]
				{
					','
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(1, string.Format("解析分享奖励道具失败, 物品配置项个数错误", new object[0]), null, true);
				}
				else
				{
					int[] array2 = Global.StringArray2IntArray(array);
					GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
					list.Add(newGoodsData);
				}
			}
			return list;
		}

		public static bool CanGetShareAward(GameClient client)
		{
			string roleParamByName = Global.GetRoleParamByName(client, "DailyShare");
			bool result;
			if (roleParamByName == null)
			{
				result = false;
			}
			else
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				string a = array[0];
				result = (a == TimeUtil.NowDateTime().DayOfYear.ToString() && array[1] == "0");
			}
			return result;
		}

		public static bool HasDoneShare(GameClient client)
		{
			string roleParamByName = Global.GetRoleParamByName(client, "DailyShare");
			bool result;
			if (roleParamByName == null)
			{
				result = false;
			}
			else
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				string a = array[0];
				result = (a == TimeUtil.NowDateTime().DayOfYear.ToString());
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessShareCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = 0;
				int num3 = 0;
				switch (nID)
				{
				case 663:
					ShareManager.UpdateRoleShareState(client);
					if (ShareManager.HasDoneShare(client))
					{
						if (ShareManager.CanGetShareAward(client))
						{
							num3 = 1;
						}
						else
						{
							num3 = 2;
						}
					}
					else
					{
						num3 = 0;
					}
					break;
				case 664:
					num2 = ShareManager.GiveRoleShareAward(client);
					if (num2 == 0 || num2 == -2)
					{
						num3 = 2;
					}
					else if (num2 == -1)
					{
						num3 = 0;
					}
					else
					{
						num3 = 1;
					}
					break;
				case 665:
					if (ShareManager.HasDoneShare(client))
					{
						if (ShareManager.CanGetShareAward(client))
						{
							num3 = 1;
						}
						else
						{
							num3 = 2;
						}
					}
					else
					{
						num3 = 0;
					}
					break;
				}
				string data2 = string.Format("{0}:{1}", num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessShareCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static int UpdateRoleShareState(GameClient client)
		{
			int num = 0;
			string text = Global.GetRoleParamByName(client, "DailyShare");
			if (text == null)
			{
				text = "-1,0";
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			int result;
			if (array[0] == TimeUtil.NowDateTime().DayOfYear.ToString())
			{
				result = 1;
			}
			else
			{
				Global.SaveRoleParamsStringToDB(client, "DailyShare", string.Format("{0},{1}", TimeUtil.NowDateTime().DayOfYear, 0), true);
				result = num;
			}
			return result;
		}

		public static int GiveRoleShareAward(GameClient client)
		{
			int result = 0;
			if (ShareManager.CanGetShareAward(client))
			{
				if (Global.CanAddGoodsDataList(client, ShareManager.ShareGoodslist))
				{
					List<GoodsData> shareGoodslist = ShareManager.ShareGoodslist;
					foreach (GoodsData goodsData in shareGoodslist)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, "分享", "1900-01-01 12:00:00", goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, goodsData.WashProps, null, 0, true);
					}
					Global.SaveRoleParamsStringToDB(client, "DailyShare", string.Format("{0},{1}", TimeUtil.NowDateTime().DayOfYear, 1), true);
					client.ClientData.AddAwardRecord(RoleAwardMsg.FenXiang, shareGoodslist, false);
					GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.FenXiang, "");
				}
				else
				{
					result = -3;
				}
			}
			else if (!ShareManager.HasDoneShare(client))
			{
				result = -1;
			}
			else
			{
				result = -2;
			}
			return result;
		}

		private static List<GoodsData> _ShareGoodslist = null;

		private static object _ShareGoodsMutex = new object();
	}
}
