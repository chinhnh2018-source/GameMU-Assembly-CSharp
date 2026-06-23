using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class BulletinMsgManager
	{
		public void LoadBulletinMsgFromDBServer()
		{
			this._BulletinMsgDict = Global.LoadDBBulletinMsgDict();
			if (null == this._BulletinMsgDict)
			{
				this._BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
			}
		}

		public BulletinMsgData AddBulletinMsg(string msgID, int playMinutes, int playNum, string bulletinText, int msgType = 0)
		{
			BulletinMsgData bulletinMsgData = new BulletinMsgData
			{
				MsgID = msgID,
				PlayMinutes = playMinutes,
				ToPlayNum = playNum,
				BulletinText = bulletinText,
				BulletinTicks = TimeUtil.NOW(),
				MsgType = msgType
			};
			if (playMinutes != 0)
			{
				lock (this._BulletinMsgDict)
				{
					this._BulletinMsgDict[msgID] = bulletinMsgData;
				}
				if (playMinutes < 0)
				{
					string fromDate = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
					string toDate = TimeUtil.NowDateTime().AddMinutes((double)playMinutes).ToString("yyyy-MM-dd HH:mm:ss");
					Global.AddDBBulletinMsg(msgID, fromDate, toDate, 0, bulletinText);
				}
			}
			return bulletinMsgData;
		}

		public BulletinMsgData AddBulletinMsgBackground(string msgID, string fromDate, string toDate, int interval, string bulletinText)
		{
			BulletinMsgData bulletinMsgData = new BulletinMsgData
			{
				MsgID = msgID,
				Interval = interval,
				BulletinText = bulletinText,
				BulletinTicks = DataHelper.ConvertToTicks(fromDate)
			};
			long num = DataHelper.ConvertToTicks(toDate) - bulletinMsgData.BulletinTicks;
			bulletinMsgData.PlayMinutes = (int)(num / 60000L);
			BulletinMsgData result;
			if (string.IsNullOrEmpty(msgID) || num < 0L || interval <= 0)
			{
				LogManager.WriteLog(2, string.Format("后台公告数据错误:{0} {1} {2} {3} {4}", new object[]
				{
					msgID,
					fromDate,
					toDate,
					interval,
					bulletinText
				}), null, true);
				result = null;
			}
			else
			{
				lock (this._BulletinMsgDict)
				{
					this._BulletinMsgDict[msgID] = bulletinMsgData;
					Global.AddDBBulletinMsg(msgID, fromDate, toDate, interval, bulletinText);
				}
				result = bulletinMsgData;
			}
			return result;
		}

		public BulletinMsgData RemoveBulletinMsg(string msgID)
		{
			BulletinMsgData bulletinMsgData = null;
			lock (this._BulletinMsgDict)
			{
				if (this._BulletinMsgDict.TryGetValue(msgID, out bulletinMsgData))
				{
					this._BulletinMsgDict.Remove(msgID);
					if (bulletinMsgData.PlayMinutes < 0 || bulletinMsgData.Interval > 0)
					{
						Global.RemoveDBBulletinMsg(msgID);
					}
				}
			}
			return bulletinMsgData;
		}

		public void SendAllBulletinMsg(GameClient client)
		{
			long num = TimeUtil.NOW();
			List<BulletinMsgData> list = new List<BulletinMsgData>();
			lock (this._BulletinMsgDict)
			{
				foreach (BulletinMsgData bulletinMsgData in this._BulletinMsgDict.Values)
				{
					if (num >= bulletinMsgData.BulletinTicks)
					{
						list.Add(bulletinMsgData);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, list[i]);
			}
		}

		public void SendAllBulletinMsgToGM(GameClient client)
		{
			List<string> list = new List<string>();
			lock (this._BulletinMsgDict)
			{
				foreach (string key in this._BulletinMsgDict.Keys)
				{
					BulletinMsgData bulletinMsgData = this._BulletinMsgDict[key];
					string text = new DateTime(bulletinMsgData.BulletinTicks * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string item = string.Format("{0} {1} {2} {3} {4} {5}", new object[]
					{
						bulletinMsgData.MsgID,
						bulletinMsgData.PlayMinutes,
						bulletinMsgData.ToPlayNum,
						bulletinMsgData.Interval,
						text,
						bulletinMsgData.BulletinText
					});
					list.Add(item);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, list[i]);
			}
		}

		public void ProcessBulletinMsg()
		{
			long num = TimeUtil.NOW();
			List<string> list = new List<string>();
			lock (this._BulletinMsgDict)
			{
				foreach (string text in this._BulletinMsgDict.Keys)
				{
					BulletinMsgData bulletinMsgData = this._BulletinMsgDict[text];
					if (bulletinMsgData.PlayMinutes >= 0)
					{
						if (bulletinMsgData.Interval > 0 && num >= bulletinMsgData.BulletinTicks && num - bulletinMsgData.LastBulletinTicks >= (long)(bulletinMsgData.Interval * 1000))
						{
							bulletinMsgData.LastBulletinTicks = num;
							BulletinMsgData bulletinMsgData2 = new BulletinMsgData
							{
								MsgID = bulletinMsgData.MsgID,
								PlayMinutes = 0,
								ToPlayNum = 1,
								BulletinText = bulletinMsgData.BulletinText,
								BulletinTicks = bulletinMsgData.BulletinTicks,
								MsgType = 0
							};
							GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData2, 0, 0);
						}
						if (num - bulletinMsgData.BulletinTicks >= (long)bulletinMsgData.PlayMinutes * 60L * 1000L)
						{
							Global.RemoveDBBulletinMsg(text);
							list.Add(text);
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this._BulletinMsgDict.Remove(list[i]);
				}
				list.Clear();
				list = null;
			}
		}

		private Dictionary<string, BulletinMsgData> _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
	}
}
