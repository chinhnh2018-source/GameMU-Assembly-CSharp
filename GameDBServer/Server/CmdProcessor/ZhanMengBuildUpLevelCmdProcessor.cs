using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
	{
		private ZhanMengBuildUpLevelCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(601, this);
		}

		public static ZhanMengBuildUpLevelCmdProcessor getInstance()
		{
			return ZhanMengBuildUpLevelCmdProcessor.instance;
		}

		private bool CheckHaveUpGradeItem(string strReqItem, DBManager dbMgr, int nBangHuiID, int nRoleID, int nToLevel)
		{
			BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, nBangHuiID);
			string[] array = strReqItem.Split(new char[]
			{
				'|'
			});
			int[] array2 = new int[5];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = 0;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					','
				});
				if (2 == array3.Length)
				{
					array2[i] = int.Parse(array3[1]);
				}
			}
			bool result;
			if (bangHuiBagData.Goods1Num < array2[0])
			{
				result = false;
			}
			else if (bangHuiBagData.Goods2Num < array2[1])
			{
				result = false;
			}
			else if (bangHuiBagData.Goods3Num < array2[2])
			{
				result = false;
			}
			else if (bangHuiBagData.Goods4Num < array2[3])
			{
				result = false;
			}
			else if (bangHuiBagData.Goods5Num < array2[4])
			{
				result = false;
			}
			else
			{
				DBWriter.UpdateBangHuiQiLevel(dbMgr, nBangHuiID, nToLevel, array2[0], array2[1], array2[2], array2[3], array2[4], 0);
				result = true;
			}
			return result;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] array = text.Split(new char[]
			{
				':'
			});
			if (array.Length != 7)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				int num5 = Convert.ToInt32(array[4]);
				int num6 = Convert.ToInt32(array[5]);
				string strReqItem = array[6];
				DBManager dbmanager = DBManager.getInstance();
				DBRoleInfo dbroleInfo = dbmanager.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbmanager, num2);
					if (null == bangHuiDetailData)
					{
						string cmdData = string.Format("{0}", -1000);
						client.sendCmd(nID, cmdData);
					}
					else if (num != bangHuiDetailData.BZRoleID)
					{
						string cmdData = string.Format("{0}", -9368);
						client.sendCmd(nID, cmdData);
					}
					else
					{
						int num7 = bangHuiDetailData.QiLevel;
						if (num3 == 1)
						{
							if (bangHuiDetailData.JiTan < bangHuiDetailData.QiLevel || bangHuiDetailData.JunXie < bangHuiDetailData.QiLevel || bangHuiDetailData.GuangHuan < bangHuiDetailData.QiLevel)
							{
								string cmdData = string.Format("{0}", -1005);
								client.sendCmd(nID, cmdData);
								return;
							}
							num7 = bangHuiDetailData.QiLevel;
						}
						else if (num3 == 2)
						{
							if (bangHuiDetailData.JiTan >= bangHuiDetailData.QiLevel)
							{
								string cmdData = string.Format("{0}", -1005);
								client.sendCmd(nID, cmdData);
								return;
							}
							num7 = bangHuiDetailData.JiTan;
						}
						else if (num3 == 3)
						{
							if (bangHuiDetailData.JunXie >= bangHuiDetailData.QiLevel)
							{
								string cmdData = string.Format("{0}", -1005);
								client.sendCmd(nID, cmdData);
								return;
							}
							num7 = bangHuiDetailData.JunXie;
						}
						else if (num3 == 4)
						{
							if (bangHuiDetailData.GuangHuan >= bangHuiDetailData.QiLevel)
							{
								string cmdData = string.Format("{0}", -1005);
								client.sendCmd(nID, cmdData);
								return;
							}
							num7 = bangHuiDetailData.GuangHuan;
						}
						if (num7 + 1 != num5)
						{
							string cmdData = string.Format("{0}", -1005);
							client.sendCmd(nID, cmdData);
						}
						else if (bangHuiDetailData.TotalMoney < num4 + num6)
						{
							string cmdData = string.Format("{0}", -1120);
							client.sendCmd(nID, cmdData);
						}
						else if (bangHuiDetailData.TotalMoney < num4)
						{
							string cmdData = string.Format("{0}", -1110);
							client.sendCmd(nID, cmdData);
						}
						else if (!this.CheckHaveUpGradeItem(strReqItem, dbmanager, num2, num, bangHuiDetailData.QiLevel))
						{
							string cmdData = string.Format("{0}", -1210);
							client.sendCmd(nID, cmdData);
						}
						else
						{
							string cmdData;
							string fieldName;
							if (num3 == 1)
							{
								fieldName = "qilevel";
							}
							else if (num3 == 2)
							{
								fieldName = "jitan";
							}
							else if (num3 == 3)
							{
								fieldName = "junxie";
							}
							else
							{
								if (num3 != 4)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("ZhanMengBuildUpLevelCmdProcessor::processCmd Param Error: buildType={0}", num3), null, true);
									cmdData = string.Format("{0}", -1310);
									client.sendCmd(nID, cmdData);
									return;
								}
								fieldName = "guanghuan";
							}
							DBWriter.UpdateZhanMengBuildLevel(dbmanager, num2, num5, num4, fieldName);
							if (num3 == 1)
							{
								GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(num2, num5);
							}
							cmdData = string.Format("{0}", 0);
							client.sendCmd(nID, cmdData);
						}
					}
				}
			}
		}

		private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();
	}
}
