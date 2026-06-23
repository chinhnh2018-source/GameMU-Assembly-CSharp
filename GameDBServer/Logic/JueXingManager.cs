using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class JueXingManager : SingletonTemplate<JueXingManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20317, SingletonTemplate<JueXingManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20318, SingletonTemplate<JueXingManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 20317)
			{
				this.GetRoleJueXingData(client, nID, cmdParams, count);
			}
			else if (nID == 20318)
			{
				this.UpdateRoleJueXingData(client, nID, cmdParams, count);
			}
		}

		private void GetRoleJueXingData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				int num = Convert.ToInt32(@string);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					client.sendCmd<List<TaoZhuangData>>(nID, dbroleInfo.JueXingTaoZhuangList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		private void UpdateRoleJueXingData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					client.sendCmd<int>(nID, -4);
				}
				int num = Convert.ToInt32(array[0]);
				int suitid = Convert.ToInt32(array[1]);
				string text = array[2];
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("REPLACE INTO t_juexing(rid, suitid, activite) VALUES('{0}', '{1}', '{2}')", num, suitid, text);
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					TaoZhuangData taoZhuangData = dbroleInfo.JueXingTaoZhuangList.Find((TaoZhuangData _g) => _g.ID == suitid);
					if (null == taoZhuangData)
					{
						taoZhuangData = new TaoZhuangData
						{
							ID = suitid,
							ActiviteList = new List<int>()
						};
						dbroleInfo.JueXingTaoZhuangList.Add(taoZhuangData);
					}
					taoZhuangData.ActiviteList = Array.ConvertAll<string, int>(text.Split(new char[]
					{
						','
					}), (string x) => Convert.ToInt32(x)).ToList<int>();
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}
	}
}
