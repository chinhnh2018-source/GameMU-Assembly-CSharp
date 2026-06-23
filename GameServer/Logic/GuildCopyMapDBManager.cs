using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	public class GuildCopyMapDBManager
	{
		public GuildCopyMapDB FindGuildCopyMapDB(int guildid, int serverId)
		{
			GuildCopyMapDB result;
			if (guildid <= 0)
			{
				result = null;
			}
			else
			{
				GuildCopyMapDB guildCopyMapDB = null;
				lock (this.GuildCopyMapDBDict)
				{
					if (this.GuildCopyMapDBDict.ContainsKey(guildid))
					{
						guildCopyMapDB = this.GuildCopyMapDBDict[guildid];
					}
					else
					{
						string[] array = null;
						string text = string.Format("{0}", guildid);
						TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 711, text, out array, serverId);
						if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
						{
							LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB dbRequestResult == TCPProcessCmdResults.RESULT_FAILED strDbCmd={0}", text), null, true);
							return null;
						}
						if (array.Length < 5 || Convert.ToInt32(array[0]) <= 0)
						{
							LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB 参数数量错误或失败 strDbCmd={0}, dbFields.Length={1}", text, array.Length), null, true);
							return null;
						}
						try
						{
							guildCopyMapDB = new GuildCopyMapDB
							{
								GuildID = Convert.ToInt32(array[0]),
								FuBenID = Convert.ToInt32(array[1]),
								State = Convert.ToInt32(array[2]),
								OpenDay = Convert.ToInt32(array[3]),
								Killers = array[4]
							};
							this.AddGuildCopyMapDB(guildCopyMapDB);
							if (guildid != Convert.ToInt32(array[0]))
							{
								LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB DB返回的id不符，guildid={0}, dbFields[0]={1}", guildid, Convert.ToInt32(array[0])), null, true);
								return null;
							}
						}
						catch (Exception)
						{
							LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB参数解析失败？", new object[0]), null, true);
							return null;
						}
					}
				}
				result = guildCopyMapDB;
			}
			return result;
		}

		public void AddGuildCopyMapDB(GuildCopyMapDB data)
		{
			if (!this.GuildCopyMapDBDict.ContainsKey(data.GuildID))
			{
				this.GuildCopyMapDBDict[data.GuildID] = data;
			}
		}

		public bool UpdateGuildCopyMapDB(GuildCopyMapDB data, int serverId)
		{
			GuildCopyMapDB guildCopyMapDB = this.FindGuildCopyMapDB(data.GuildID, serverId);
			bool result;
			if (null == guildCopyMapDB)
			{
				LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::UpdateGuildCopyMapDB null == oldData data.GuildID={0}", data.GuildID), null, true);
				result = false;
			}
			else
			{
				lock (this.GuildCopyMapDBDict)
				{
					string[] array = null;
					string text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						data.GuildID,
						data.FuBenID,
						data.State,
						data.OpenDay,
						data.Killers
					});
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10172, text, out array, serverId);
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
					{
						LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::ResetGuildCopyMapDB dbRequestResult == TCPProcessCmdResults.RESULT_FAILED strDbCmd={0}", text), null, true);
						return false;
					}
					if (array.Length < 1 || Convert.ToInt32(array[0]) != 1)
					{
						LogManager.WriteLog(2, string.Format("GuildCopyMapDBManager::ResetGuildCopyMapDB 参数数量错误或失败 strDbCmd={0}, dbFields.Length={1}", text, array.Length), null, true);
						return false;
					}
					this.GuildCopyMapDBDict[data.GuildID] = data;
				}
				result = true;
			}
			return result;
		}

		public void ResetGuildCopyMapDB(int guildid, int serverId)
		{
			GuildCopyMapDB data = new GuildCopyMapDB
			{
				GuildID = guildid,
				FuBenID = GameManager.GuildCopyMapMgr.FirstGuildCopyMapOrder,
				State = 0,
				OpenDay = Global.GetOffsetDay(TimeUtil.NowDateTime()),
				Killers = ""
			};
			this.UpdateGuildCopyMapDB(data, serverId);
		}

		private Dictionary<int, GuildCopyMapDB> GuildCopyMapDBDict = new Dictionary<int, GuildCopyMapDB>();
	}
}
