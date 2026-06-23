using System;
using System.Text;
using System.Threading;
using GameDBServer.Data.Tarot;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Tarot
{
	public class TarotManager
	{
		public static TCPProcessCmdResults ProcessUpdateTarotDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 20100);
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
				int nRoleID = Convert.ToInt32(array[0]);
				string data2 = array[1];
				string text2 = array[2];
				string text3 = string.Empty;
				string empty = string.Empty;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbroleInfo)
				{
					bool flag = false;
					try
					{
						DBRoleInfo obj;
						Monitor.Enter(obj = dbroleInfo, ref flag);
						TarotCardData tarotData = new TarotCardData(data2);
						if (tarotData.GoodId > 0)
						{
							TarotCardData tarotCardData = dbroleInfo.TarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == tarotData.GoodId);
							if (tarotCardData == null)
							{
								dbroleInfo.TarotData.TarotCardDatas.Add(tarotData);
							}
							else
							{
								TarotCardData tarotCardData2 = dbroleInfo.TarotData.TarotCardDatas.Find((TarotCardData x) => x.Postion == tarotData.Postion);
								if (tarotCardData2 != null)
								{
									tarotCardData2.Postion = 0;
								}
								tarotCardData.Level = tarotData.Level;
								tarotCardData.Postion = tarotData.Postion;
								tarotCardData.TarotMoney = tarotData.TarotMoney;
							}
						}
						foreach (TarotCardData tarotCardData3 in dbroleInfo.TarotData.TarotCardDatas)
						{
							text3 += tarotCardData3.GetDataStrInfo();
						}
						if (text2 != "-1")
						{
							TarotKingData kingData = new TarotKingData(text2);
							dbroleInfo.TarotData.KingData = kingData;
						}
					}
					finally
					{
						if (flag)
						{
							DBRoleInfo obj;
							Monitor.Exit(obj);
						}
					}
				}
				string data3 = DBWriter.UpdateTarotData(dbMgr, nRoleID, text3, dbroleInfo.TarotData.KingData.GetDataStrInfo()) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data3, nID);
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
