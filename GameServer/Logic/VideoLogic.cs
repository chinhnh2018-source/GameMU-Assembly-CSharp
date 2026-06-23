using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Video;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class VideoLogic : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static VideoLogic getInstance()
		{
			return VideoLogic.instance;
		}

		public bool initialize()
		{
			this.LoadVideoXml();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1402, 1, 1, VideoLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1403, 2, 2, VideoLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(28, VideoLogic.getInstance());
			GlobalEventSource.getInstance().registerListener(12, VideoLogic.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, VideoLogic.getInstance());
			GlobalEventSource.getInstance().removeListener(12, VideoLogic.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 12)
			{
				PlayerLogoutEventObject playerLogoutEventObject = eventObject as PlayerLogoutEventObject;
				if (null != playerLogoutEventObject)
				{
					GameClient client = playerLogoutEventObject.getPlayer();
					this.HandlePlayerLogout(client);
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				if (null != onStartPlayGameEventObject)
				{
					GameClient client = onStartPlayGameEventObject.Client;
					this.HandleStartPlayGame(client);
				}
			}
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1402:
				result = this.ProcessGuanZhanRoleMiniDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1403:
				result = this.ProcessGuanZhanTrackingCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public void LoadVideoXml()
		{
			lock (this.Mutex)
			{
				this.VideoList.Clear();
				string text = Global.GameResPath("Config/Viedo.xml");
				XElement xelement = ConfigHelper.Load(text);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, "未找到配置文件:" + text, null, true);
				}
				else
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						VideoData videoData = new VideoData();
						videoData.TalkID = Convert.ToInt32(Global.GetSafeAttributeStr(xelement2, "TalkID"));
						videoData.MinZhuanSheng = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MinZhuanSheng"));
						videoData.MinLevel = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MinLevel"));
						videoData.MaxZhuanSheng = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MaxZhuanSheng"));
						videoData.MaxLevel = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MaxLevel"));
						videoData.MinVip = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MinVip"));
						videoData.MaxVip = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "MaxVip"));
						videoData.PassWord = Global.GetSafeAttributeStr(xelement2, "PassWord");
						videoData.ZhuanshengSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "ZhuanshengSift"));
						videoData.LevelSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "LevelSift"));
						videoData.VIPSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xelement2, "VIPSift"));
						this.VideoList.Add(videoData);
					}
					this.VideoList = (from x in this.VideoList
					orderby x.MinVip descending
					select x).ToList<VideoData>();
				}
				this.VideoGMDict.Clear();
				text = Global.IsolateResPath("Config/GuanZhanList.xml");
				xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					foreach (XElement xelement2 in xelement.Elements())
					{
						this.VideoGMDict[Global.GetSafeAttributeStr(xelement2, "UID")] = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ZoneID", 0L);
					}
				}
				else
				{
					LogManager.WriteLog(1000, "未找到配置文件:" + text, null, true);
				}
				this.ChuanSongDict.Clear();
				text = Global.IsolateResPath("Config/GuanZhanTransfer.xml");
				xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					foreach (XElement xelement2 in xelement.Elements())
					{
						ChuanSongItem chuanSongItem = new ChuanSongItem();
						chuanSongItem.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						chuanSongItem.MapCode = (int)Global.GetSafeAttributeLong(xelement2, "MapCode");
						List<int> list = Global.StringToIntList(Global.GetSafeAttributeStr(xelement2, "Site"), '|');
						chuanSongItem.PosX = list[0];
						chuanSongItem.PosY = list[1];
						this.ChuanSongDict.Add(chuanSongItem.ID, chuanSongItem);
					}
				}
				else
				{
					LogManager.WriteLog(1000, "未找到配置文件:" + text, null, true);
				}
			}
		}

		public TCPProcessCmdResults ProcessOpenVideoCmd(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 1700);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 1700);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				VideoData videoRoomData = this.GetVideoRoomData(client);
				if (videoRoomData == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int playerFilterStatus = this.GetPlayerFilterStatus(client, videoRoomData);
				string data2 = string.Format("{0}:{1}:{2}", videoRoomData.TalkID, videoRoomData.PassWord, playerFilterStatus);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private VideoData GetVideoRoomData(GameClient client)
		{
			foreach (VideoData videoData in this.VideoList)
			{
				if (client.ClientData.VipLevel >= videoData.MinVip && client.ClientData.VipLevel <= videoData.MaxVip && client.ClientData.Level + client.ClientData.ChangeLifeCount * 100 <= videoData.MaxLevel + videoData.MaxZhuanSheng * 100 && client.ClientData.Level + client.ClientData.ChangeLifeCount * 100 >= videoData.MinLevel + videoData.MinZhuanSheng * 100)
				{
					return videoData;
				}
			}
			return null;
		}

		private int GetPlayerFilterStatus(GameClient client, VideoData data)
		{
			return (client.ClientData.Level >= data.LevelSift || client.ClientData.VipLevel >= data.VIPSift || client.ClientData.ChangeLifeCount >= data.ZhuanshengSift) ? 1 : 0;
		}

		public int GetOrSendPlayerVideoStatus(GameClient client, List<int> RoleCommonUseIntPamams = null)
		{
			int num = (this.GetVideoRoomData(client) == null) ? 0 : 1;
			if (RoleCommonUseIntPamams != null && RoleCommonUseIntPamams.Count >= 36 && RoleCommonUseIntPamams[36] == 0 && num == 1)
			{
				client.ClientData.RoleCommonUseIntPamams[36] = 1;
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.VideoButton, num);
			}
			return num;
		}

		public bool IsGuanZhanGM(GameClient client)
		{
			lock (this.Mutex)
			{
				int num;
				if (this.VideoGMDict.TryGetValue(client.strUserID, out num) && (num <= 0 || num == client.ClientData.ZoneID))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetGuanZhanPos(int mapCode, ref int posX, ref int posY)
		{
			ChuanSongItem chuanSongItem = null;
			lock (this.Mutex)
			{
				foreach (ChuanSongItem chuanSongItem2 in this.ChuanSongDict.Values)
				{
					if (chuanSongItem2.MapCode == mapCode)
					{
						chuanSongItem = chuanSongItem2;
						if (chuanSongItem2.PosX == posX && chuanSongItem2.PosY == posY)
						{
							return true;
						}
					}
				}
				if (null != chuanSongItem)
				{
					posX = chuanSongItem.PosX;
					posY = chuanSongItem.PosY;
				}
			}
			return chuanSongItem != null;
		}

		public TCPProcessCmdResults ProcessGuanZhanMoveToCmd(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				SpriteMoveData spriteMoveData = DataHelper.BytesToObject<SpriteMoveData>(data, 0, count);
				if (null == spriteMoveData)
				{
					LogManager.WriteLog(2, string.Format("ProcessOpenVideoCmd解析客户端数据失败!", new object[0]), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = spriteMoveData.roleID;
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref roleID))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientData.HideGM == 0)
				{
					LogManager.WriteLog(2, string.Format("帐号{0}无观战权限,禁止传送!", gameClient.strUserID), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int mapCode = spriteMoveData.mapCode;
				int toX = spriteMoveData.toX;
				int toY = spriteMoveData.toY;
				if (Global.GetMapSceneType(mapCode) != Global.GetMapSceneType(gameClient.ClientData.MapCode))
				{
					LogManager.WriteLog(2, string.Format("必须进入观战地图,才可传送在本活动关联地图范围内传送!", gameClient.strUserID), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameMap gameMap;
				if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
				{
					if (mapCode != gameClient.ClientData.MapCode)
					{
						gameClient.ClientData.WaitingChangeMapToMapCode = mapCode;
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, mapCode, toX, toY, -1, 0);
					}
					else
					{
						GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, toX, toY, -1);
					}
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public bool ProcessGuanZhanRoleMiniDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					return true;
				}
				if (client.ClientData.HideGM == 0)
				{
					return true;
				}
				int num2 = 0;
				int num3 = 0;
				if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref num2, ref num3))
				{
					return true;
				}
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				GuanZhanData guanZhanData = new GuanZhanData();
				if (45 == mapSceneType)
				{
					BangHuiMatchManager.getInstance().FillGuanZhanData(client, guanZhanData);
				}
				if (35 == mapSceneType)
				{
					LangHunLingYuManager.getInstance().FillGuanZhanData(client, guanZhanData);
				}
				if (59 == mapSceneType)
				{
					EscapeBattleManager.getInstance().FillGuanZhanData(client, guanZhanData);
				}
				client.sendCmd<GuanZhanData>(nID, guanZhanData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGuanZhanTrackingCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					return true;
				}
				lock (this.Mutex)
				{
					if (client.ClientData.HideGM == 0 || num == num2)
					{
						return true;
					}
					int num3 = 0;
					int num4 = 0;
					if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref num3, ref num4))
					{
						int num5 = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", num5, -1), false);
						return true;
					}
					if (-1 == num2)
					{
						this.CancleTracking(client, true);
						return true;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(num2);
					if (gameClient == null || gameClient.ClientData.HideGM > 0 || gameClient.ClientData.WaitingNotifyChangeMap || gameClient.ClientData.WaitingForChangeMap)
					{
						int num5 = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", num5, -1), false);
						return true;
					}
					SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					SceneUIClasses mapSceneType2 = Global.GetMapSceneType(gameClient.ClientData.MapCode);
					if (mapSceneType != mapSceneType2)
					{
						int num5 = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", num5, -1), false);
						return true;
					}
					this.Tracking(client, gameClient);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private void Tracking(GameClient tClient, GameClient beTClient)
		{
			try
			{
				lock (this.Mutex)
				{
					this.CancleTracking(tClient, true);
					if (tClient.ClientData.MapCode != beTClient.ClientData.MapCode)
					{
						GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), tClient, -1, beTClient.ClientData.MapCode, beTClient.ClientData.PosX, beTClient.ClientData.PosY, beTClient.ClientData.RoleDirection, 123);
					}
					else
					{
						GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, tClient, beTClient.ClientData.PosX, beTClient.ClientData.PosY, -1);
					}
					tClient.ClientData.BeTrackingRoleID = beTClient.ClientData.RoleID;
					if (!beTClient.ClientData.TrackingRoleIDList.Exists((int x) => x == tClient.ClientData.RoleID))
					{
						beTClient.ClientData.TrackingRoleIDList.Add(tClient.ClientData.RoleID);
					}
					int num = 0;
					tClient.sendCmd(1403, string.Format("{0}:{1}", num, beTClient.ClientData.RoleID), false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public void CancleTracking(GameClient client, bool notify = true)
		{
			try
			{
				if (null != client)
				{
					lock (this.Mutex)
					{
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.BeTrackingRoleID);
							if (null != gameClient)
							{
								gameClient.ClientData.TrackingRoleIDList.RemoveAll((int x) => x == client.ClientData.RoleID);
							}
							client.ClientData.BeTrackingRoleID = 0;
							if (notify)
							{
								int num = 0;
								client.sendCmd(1403, string.Format("{0}:{1}", num, -1), false);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public void TryTrackingOther(GameClient tClient, GameClient lostClient)
		{
			try
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(lostClient.ClientData.MapCode);
				GuanZhanData guanZhanData = new GuanZhanData();
				if (45 == mapSceneType)
				{
					BangHuiMatchManager.getInstance().FillGuanZhanData(lostClient, guanZhanData);
				}
				if (59 == mapSceneType)
				{
					EscapeBattleManager.getInstance().FillGuanZhanData(lostClient, guanZhanData);
				}
				List<GuanZhanRoleMiniData> list = null;
				if (guanZhanData.RoleMiniDataDict.TryGetValue(lostClient.ClientData.BattleWhichSide, out list))
				{
					foreach (GuanZhanRoleMiniData guanZhanRoleMiniData in list)
					{
						if (guanZhanRoleMiniData.RoleID != lostClient.ClientData.RoleID)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(guanZhanRoleMiniData.RoleID);
							if (null != gameClient)
							{
								this.Tracking(tClient, gameClient);
								return;
							}
						}
					}
				}
				guanZhanData.RoleMiniDataDict.Remove(lostClient.ClientData.BattleWhichSide);
				foreach (List<GuanZhanRoleMiniData> list2 in guanZhanData.RoleMiniDataDict.Values)
				{
					foreach (GuanZhanRoleMiniData guanZhanRoleMiniData in list)
					{
						if (guanZhanRoleMiniData.RoleID != lostClient.ClientData.RoleID)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(guanZhanRoleMiniData.RoleID);
							if (null != gameClient)
							{
								this.Tracking(tClient, gameClient);
								return;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public void ClientRelive(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.TrackingRoleIDList.Count != 0)
					{
						bool flag2 = true;
						int num = 0;
						int num2 = 0;
						if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref num, ref num2))
						{
							flag2 = false;
						}
						List<int> list = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int roleID in list)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != gameClient)
							{
								if (!flag2)
								{
									this.CancleTracking(client, true);
								}
								else
								{
									this.Tracking(gameClient, client);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		private void HandleStartPlayGame(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.BeTrackingRoleID != 0 || client.ClientData.TrackingRoleIDList.Count != 0)
					{
						bool flag2 = true;
						int num = 0;
						int num2 = 0;
						if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref num, ref num2))
						{
							flag2 = false;
						}
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							if (!flag2)
							{
								this.CancleTracking(client, true);
							}
							else
							{
								ClientManager.DoSpriteMapGridMove(client, 0);
								GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.BeTrackingRoleID);
								if (null != gameClient)
								{
									int num3 = 0;
									client.sendCmd(1403, string.Format("{0}:{1}", num3, gameClient.ClientData.RoleID), false);
								}
							}
						}
						List<int> list = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int roleID in list)
						{
							GameClient gameClient2 = GameManager.ClientMgr.FindClient(roleID);
							if (null != gameClient2)
							{
								if (!flag2)
								{
									this.CancleTracking(gameClient2, true);
								}
								else if (gameClient2.ClientData.MapCode != client.ClientData.MapCode)
								{
									GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), gameClient2, -1, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, 123);
								}
								else
								{
									GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, client.ClientData.PosX, client.ClientData.PosY, -1);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		private void HandlePlayerLogout(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.BeTrackingRoleID != 0 || client.ClientData.TrackingRoleIDList.Count != 0)
					{
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							this.CancleTracking(client, true);
						}
						List<int> list = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int roleID in list)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != gameClient)
							{
								this.CancleTracking(gameClient, true);
								this.TryTrackingOther(gameClient, client);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		private static VideoLogic instance = new VideoLogic();

		public object Mutex = new object();

		private List<VideoData> VideoList = new List<VideoData>();

		private Dictionary<string, int> VideoGMDict = new Dictionary<string, int>();

		private Dictionary<int, ChuanSongItem> ChuanSongDict = new Dictionary<int, ChuanSongItem>();
	}
}
