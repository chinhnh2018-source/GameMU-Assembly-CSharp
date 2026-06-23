using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network.Protocol;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Server.Data;
using Server.Tools;

namespace HSGameEngine.GameEngine.Network
{
	public class TCPGame
	{
		static TCPGame()
		{
			TCPClient.ProcessServerCmd = new ProcessServerCmdHandler(TCPCmdHandler.ProcessServerCmd);
			if (TCPCmdHandler.ProtoUnmarshalFuncs != null)
			{
				return;
			}
			Dictionary<int, TCPCmdHandler.UnmarshalFun> dictionary = new Dictionary<int, TCPCmdHandler.UnmarshalFun>();
			dictionary.Add(107, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteNotifyOtherMoveData>));
			dictionary.Add(114, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteActionData>));
			dictionary.Add(118, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteInjuredData>));
			dictionary.Add(116, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteMagicCodeData>));
			dictionary.Add(117, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteAttackResultData>));
			dictionary.Add(155, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteHitedData>));
			dictionary.Add(120, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteRelifeData>));
			dictionary.Add(158, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SC_SprUseGoods>));
			dictionary.Add(209, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<LoadAlreadyData>));
			dictionary.Add(145, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<NewGoodsPackData>));
			dictionary.Add(131, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SCModGoods>));
			dictionary.Add(108, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SCMoveEnd>));
			dictionary.Add(614, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<ActivityIconStateData>));
			dictionary.Add(164, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SpriteLifeChangeData>));
			dictionary.Add(23, new TCPCmdHandler.UnmarshalFun(ProtoHelper.Unmarshal<SCClientHeart>));
			TCPCmdHandler.ProtoUnmarshalFuncs = dictionary;
		}

		public event SocketConnectEventHandler SocketFailed;

		public event SocketConnectEventHandler SocketSuccess;

		public event SocketConnectEventHandler SocketCommand;

		public TCPGame.GameStates GameState
		{
			get
			{
				return this._GameState;
			}
			set
			{
				this._GameState = value;
			}
		}

		public void Connect(string ip, int port, bool setEvent = true)
		{
			if (this._GameState > TCPGame.GameStates.CLIENT_READY)
			{
				throw new Exception("TCPGame已经不是出于准备状态, 无法再次连接");
			}
			this.ActiveDisconnect = false;
			if (setEvent)
			{
				this.tcpClient.SocketConnect += this.SocketConnect;
			}
			this.tcpClient.Connect(ip, port);
			this._GameState = TCPGame.GameStates.CLIENT_CONNECTING;
		}

		public bool IsActiveDisconnect()
		{
			return this.tcpClient == null || this.tcpClient.Connected;
		}

		public void Disconnect()
		{
			if (this._GameState <= TCPGame.GameStates.CLIENT_READY)
			{
				MUDebug.LogError<string>(new string[]
				{
					"TCPGame已经不是出于准备状态, 无法再次连接"
				});
				return;
			}
			this.ActiveDisconnect = true;
			this.SpriteLogOut();
			this.tcpClient.Disconnect(0);
			GScene.ServerStopGame();
			this._GameState = TCPGame.GameStates.CLIENT_READY;
		}

		public TCPClient GameClient
		{
			get
			{
				return this.tcpClient;
			}
		}

		public void ResetGameClient()
		{
			this.tcpClient = new TCPClient(1);
		}

		public bool ConnectedState
		{
			get
			{
				bool result = false;
				if (this.tcpClient != null)
				{
					result = this.tcpClient.Connected;
				}
				return result;
			}
			set
			{
				if (this.tcpClient != null)
				{
					this.tcpClient.Connected = value;
				}
			}
		}

		public void PingTimeOut()
		{
			BasePlayZone.InWaitPingCount = 0;
			MUSocketConnectEventArgs musocketConnectEventArgs = new MUSocketConnectEventArgs();
			this.ActiveDisconnect = true;
			string errorMsg = string.Format(Global.GetLang("连接游戏务器失败"), new object[0]);
			musocketConnectEventArgs.ErrorMsg = errorMsg;
			musocketConnectEventArgs.ReturnStartPage = false;
			musocketConnectEventArgs.ShowMsgBox = false;
			if (this.SocketFailed != null)
			{
				this.SocketFailed(this, musocketConnectEventArgs);
			}
		}

		public bool ActiveDisconnect
		{
			get
			{
				return this._ActiveDisconnect;
			}
			set
			{
				this._ActiveDisconnect = value;
			}
		}

		private void SocketConnect(object sender, MUSocketConnectEventArgs e)
		{
			switch (e.NetSocketType)
			{
			case 0:
				if (e.Error == "Success")
				{
					this._GameState = TCPGame.GameStates.CLIENT_CONNECTED;
					string text = string.Empty;
					text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
					{
						this.CurrentSession.UserID,
						this.CurrentSession.UserName,
						this.CurrentSession.UserToken,
						this.CurrentSession.RoleRandToken,
						20140624,
						this.CurrentSession.UserIsAdult,
						QMQJJava.GetDeviceID()
					});
					text = KuaFuLoginManager.GetKuaFuLoginString(text);
					this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 100));
				}
				else
				{
					this.ActiveDisconnect = true;
					string errorMsg = string.Format(Global.GetLang("连接游戏务器失败"), new object[0]);
					e.ErrorMsg = errorMsg;
					e.ReturnStartPage = false;
					e.ShowMsgBox = false;
					if (this.SocketFailed != null)
					{
						this.SocketFailed(this, e);
					}
				}
				break;
			case 1:
			{
				this.ActiveDisconnect = true;
				string errorMsg2 = string.Format(Global.GetLang("与游戏服务器通讯失败"), new object[0]);
				e.ErrorMsg = errorMsg2;
				e.ReturnStartPage = false;
				e.ShowMsgBox = true;
				if (this.SocketFailed != null)
				{
					this.SocketFailed(this, e);
				}
				break;
			}
			case 2:
				break;
			case 3:
				GScene.ServerStopGame();
				if (!this.ActiveDisconnect)
				{
					string errorMsg3 = string.Format(Global.GetLang("亲爱的玩家，你暂时与服务器断开了连接。请放心，我们已经保存了您的数据，请重新登陆！"), new object[0]);
					e.ErrorMsg = errorMsg3;
					e.ReturnStartPage = false;
					e.ShowMsgBox = true;
					if (this.SocketFailed != null)
					{
						this.SocketFailed(this, e);
					}
				}
				break;
			case 4:
				if (e.CmdID == 100)
				{
					int num = -1;
					if (e.fields.Length > 0)
					{
						num = Convert.ToInt32(e.fields[0]);
					}
					if (num == -1)
					{
						this.ActiveDisconnect = true;
						string errorMsg4 = string.Format(Global.GetLang("登陆游戏服务器时失败, 已经超过了口令最长有效时间, 请退出游戏重新进入..."), new object[0]);
						e.ErrorMsg = errorMsg4;
						e.ReturnStartPage = true;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
					else if (num == -2 || num == -11006)
					{
						this.ActiveDisconnect = true;
						string errorMsg5 = string.Format(Global.GetLang("登陆的用户名已经在线，请稍后重新刷新登陆"), new object[0]);
						e.ErrorMsg = errorMsg5;
						e.ReturnStartPage = false;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
					else if (num == -3)
					{
						this.ActiveDisconnect = true;
						string errorMsg6 = string.Format(Global.GetLang("登陆游戏服务器时失败, 客户端的版本太旧，请更新客户端后再重新登陆"), new object[0]);
						e.ErrorMsg = errorMsg6;
						e.ReturnStartPage = true;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
					else if (num == -10)
					{
						this.ActiveDisconnect = true;
						string errorMsg7 = string.Format(Global.GetLang("登陆游戏服务器时失败, 你已经被游戏管理员禁止登陆"), new object[0]);
						e.ErrorMsg = errorMsg7;
						e.ReturnStartPage = true;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
					else if (num == -100)
					{
						this.ActiveDisconnect = true;
						string errorMsg8 = string.Format(Global.GetLang("当前服务器在线爆满，您可登录其他服务器进行游戏！"), new object[0]);
						e.ErrorMsg = errorMsg8;
						e.ReturnStartPage = true;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
					else if (num >= 0)
					{
						this._GameState = TCPGame.GameStates.CLIENT_LOGON;
						this.CurrentSession.RoleRandToken = num;
						if (this.SocketSuccess != null)
						{
							this.SocketSuccess(this, e);
						}
						BasePlayZone.IsSpeedCheck = true;
						BasePlayZone.InWaitPingCount = 0;
					}
					else
					{
						this.ActiveDisconnect = true;
						string errorMsg9 = string.Format(Global.GetLang("登陆游戏服务器时失败, 错误码:{0}"), num);
						e.ErrorMsg = errorMsg9;
						e.ReturnStartPage = true;
						e.ShowMsgBox = true;
						if (this.SocketFailed != null)
						{
							this.SocketFailed(this, e);
						}
					}
				}
				else if (this.SocketCommand != null)
				{
					this.SocketCommand(this, e);
				}
				break;
			default:
				this.ActiveDisconnect = true;
				if (this.SocketFailed != null)
				{
					this.SocketFailed(this, e);
				}
				throw new Exception(Global.GetLang("错误的Socket操作类型"));
			}
		}

		private string StringReplaceAll(string str, string oldValue, string newValue)
		{
			return str.Replace(oldValue, newValue);
		}

		public void GetRoleList(int zoneID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				zoneID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 860));
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 101));
			this.SendVersion();
		}

		public void GetRandomPreName(int sex)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				sex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 257));
		}

		public void CreateRole(int sex, int occupation, string name, int zoneID, int LiOrZhi)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.CurrentSession.UserID,
				this.CurrentSession.UserName,
				sex,
				occupation,
				name,
				zoneID,
				LiOrZhi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 102));
		}

		public void RemoveRole(int roleid)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				roleid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 103));
		}

		public void UnRemoveRole(int roleid)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				roleid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 98));
		}

		public void CancelUnRemoveRole(int roleid)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				roleid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 99));
		}

		public void InitPlayGame()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			if (Global.Data != null && Global.Data.equipPet != null)
			{
				Global.Data.equipPet.Clear();
				Global.Data.equipPet = null;
			}
			string text = string.Empty;
			PlatSDKMgr._bReConnect = 0;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.UserID,
				this.CurrentSession.LocalRoleID,
				QMQJJava.GetSystermInfo(0)
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 104));
		}

		public void TimeSynchronization()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				MyDateTime.Now().Ticks
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 105));
			this.SendVersion();
		}

		public bool SendTimeSynchronization()
		{
			if (this.ActiveDisconnect)
			{
				return false;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				MyDateTime.Now().Ticks
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 105));
			return true;
		}

		public void TimeSynchronizationByClient()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				MyDateTime.Now().Ticks
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 831));
		}

		public void SendVersion()
		{
			string text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				Context.CodeRevision,
				Context.MainExeVer,
				Context.ResSwfVer
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 673));
		}

		public void StartPlayGame()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 106));
		}

		public void SpriteMoveTo(Point from, Point to, int action, int extAction)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = DataHelper.ZipStringToBase64(this.CurrentSession.RolePathString);
			SpriteMoveData spriteMoveData = new SpriteMoveData(this.CurrentSession.RoleID, this.CurrentSession.roleData.MapCode, action, to.X, to.Y, extAction, from.X, from.Y, TimeManager.GetCorrectLocalTime(), text);
			byte[] array = spriteMoveData.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 107));
			TCPPing.RecordSendCmd(107);
		}

		public void SpriteMoveEnd(Point to, int direction)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SCMoveEnd
			{
				RoleID = this.CurrentSession.RoleID,
				MapCode = this.CurrentSession.roleData.MapCode,
				ToMapX = to.X,
				ToMapY = to.Y,
				ToDiection = direction,
				clientTicks = TimeManager.GetCorrectLocalTime()
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 108));
		}

		public void SpriteStopMove(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 411));
		}

		public void SpriteMoveTo2(int petRoleID, Point from, Point to, int action, int extAction, int roleType, string pathString)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			string text2 = DataHelper.ZipStringToBase64(pathString);
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}", new object[]
			{
				this.CurrentSession.RoleID,
				petRoleID,
				this.CurrentSession.roleData.MapCode,
				action,
				to.X,
				to.Y,
				extAction,
				from.X,
				from.Y,
				TimeManager.GetCorrectLocalTime(),
				roleType,
				text2
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 109));
		}

		public void SpriteCheck(int processSubTicks, int dateTimeSubTicks)
		{
			if (!Global.Data.PlayGame || !BasePlayZone.IsSpeedCheck)
			{
				return;
			}
			BasePlayZone.IsSpeedCheck = false;
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				processSubTicks,
				dateTimeSubTicks
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 611));
			if (PlayZone.GlobalPlayZone.LeaderExperience != null)
			{
				PlayZone.GlobalPlayZone.LeaderExperience.SendPing();
			}
		}

		public void SpritePosition(Point to, long ticks)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			SpritePositionData spritePositionData = new SpritePositionData(this.CurrentSession.RoleID, this.CurrentSession.roleData.MapCode, to.X, to.Y, ticks);
			byte[] array = spritePositionData.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 112));
			TCPPing.RecordSendCmd(112);
		}

		public void SpritePetPosition(int toRoleID, Point to, int roleType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.CurrentSession.RoleID,
				toRoleID,
				this.CurrentSession.roleData.MapCode,
				to.X,
				to.Y,
				TimeManager.GetCorrectLocalTime(),
				roleType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 113));
		}

		public void SpriteMagicCode(int magicCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SpriteMagicCodeData
			{
				roleID = this.CurrentSession.RoleID,
				mapCode = this.CurrentSession.roleData.MapCode,
				magicCode = magicCode
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 116));
		}

		public void SpriteAction(double direction, int action, Point to, Point targetPos, int yAngle, Point moveTo)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute(" RoleID  = {0}   MapCode = :{1}  direction = :{2}  action = :{3}  to.X = :{4}   to.Y = :{5}  targetPos.X = :{6}  targetPos.Y:{7} yAngle = : {8}:  moveTo  X={9}  Y = :{10}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.MapCode,
				(int)direction,
				action,
				to.X,
				to.Y,
				targetPos.X,
				targetPos.Y,
				yAngle,
				moveTo.X,
				moveTo.Y
			});
			byte[] array = new SpriteActionData(this.CurrentSession.RoleID, this.CurrentSession.roleData.MapCode, (int)direction, action, to.X, to.Y, targetPos.X, targetPos.Y, yAngle, moveTo.X, moveTo.Y)
			{
				clientTicks = TimeManager.GetCorrectLocalTime()
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 114));
			TCPPing.RecordSendCmd(114);
		}

		public void SpriteAction2(int toRoleID, double direction, int action, Point to, Point targetPos, int roleType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				this.CurrentSession.RoleID,
				toRoleID,
				this.CurrentSession.roleData.MapCode,
				(int)direction,
				action,
				to.X,
				to.Y,
				targetPos.X,
				targetPos.Y,
				roleType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 115));
		}

		public void SpriteAttack(Point selfPos, int enemy, Point enemyPos, Point realEnemyPos, int magicCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SpriteAttackData
			{
				roleID = this.CurrentSession.RoleID,
				roleX = selfPos.X,
				roleY = selfPos.Y,
				enemy = enemy,
				enemyX = enemyPos.X,
				enemyY = enemyPos.Y,
				realEnemyX = realEnemyPos.X,
				realEnemyY = realEnemyPos.Y,
				magicCode = magicCode,
				clientTicks = TimeManager.GetCorrectLocalTime()
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 117));
		}

		public void SpriteAttack2(int injureRoleID, Point selfPos, int enemy, Point enemyPos, Point realEnemyPos, int magicCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				enemy,
				enemyPos.X,
				enemyPos.Y,
				injureRoleID,
				selfPos.X,
				selfPos.Y,
				realEnemyPos.X,
				realEnemyPos.Y,
				magicCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 523));
		}

		public void SpriteRealive(int x, int y, int direction)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				x,
				y,
				direction
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 119));
		}

		public void SpriteClickOnNPC(int mapCode, int roleID, int extensionID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new CS_ClickOn
			{
				RoleId = this.CurrentSession.RoleID,
				MapCode = mapCode,
				NpcId = roleID,
				ExtId = extensionID
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 121));
		}

		public void SpriteClickOnNPCForLuaTalk(int mapCode, int roleID, int extensionID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode,
				roleID,
				extensionID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 413));
		}

		public void SpriteExcuteNpcLuaFunction(int mapCode, int roleID, int extensionID, int tag, string luaFuncString)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode,
				roleID,
				extensionID,
				tag,
				luaFuncString
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 414));
		}

		public void SpriteNewTask(int npcID, int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 125));
		}

		public void SpriteYuanBaoCompleteTask(int npcID, int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 479));
		}

		public void SpriteMapConversion(int teleportID, int toMapCode, int toMapX, int toMapY, int toDiection)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SCMapChange
			{
				RoleID = this.CurrentSession.RoleID,
				TeleportID = teleportID,
				NewMapCode = toMapCode,
				ToNewMapX = toMapX,
				ToNewMapY = toMapY,
				ToNewDiection = toDiection
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 123));
		}

		public void SpriteGetAttrib2()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 126));
		}

		public void SpriteBuyGoods(int goodsID, int goodsNum, int saleType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsID,
				goodsNum,
				saleType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 128));
		}

		public void SpriteBuyOutGoods(int goodsDbId1, int goodsDbId2, int goodsDbId3, int goodsDbId4, int goodsDbId5)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbId1,
				goodsDbId2,
				goodsDbId3,
				goodsDbId4,
				goodsDbId5
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 129));
		}

		public void SpriteAddGoods(int goodsID, int gcount)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsID,
				gcount
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 130));
		}

		public void SpriteAddPoint(int unitPropIndexes, int point)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				unitPropIndexes,
				point
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 513));
		}

		public void SpriteClearPoint()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 515));
		}

		public void SpriteRecommendPoint(int nStrengthPoint, int nIntelligencePoint, int nDexterityPoint, int nConstitutionPoint)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new CSPropAddPoint
			{
				RoleID = this.CurrentSession.RoleID,
				Strength = nStrengthPoint,
				Intelligence = nIntelligencePoint,
				Dexterity = nDexterityPoint,
				Constitution = nConstitutionPoint
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 514));
		}

		public void SpriteModGoods(int modType, int id, int goodsID, int isusing, int site, int gcount, int bagIndex, string extraParams = "")
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			MUDebug.Log<string>(new string[]
			{
				string.Format("消息 CMD_SPR_MOD_GOODS GoodsId = {0}   Using = {1}   bagIndex = {2} modType ={3}   site = {4}", new object[]
				{
					goodsID,
					isusing,
					bagIndex,
					(ModGoodsTypes)modType,
					(SaleGoodsConsts)site
				})
			});
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				this.CurrentSession.RoleID,
				modType,
				id,
				goodsID,
				isusing,
				site,
				gcount,
				bagIndex,
				extraParams
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 131));
		}

		public void SpriteMergeGoods(int id, int site, int goodsID, int otherId, int otherGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				site,
				goodsID,
				otherId,
				otherGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 132));
		}

		public void SpriteSplitGoods(int id, int site, int goodsID, int newNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				site,
				goodsID,
				newNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 133));
		}

		public void SpriteCompleteTask(int npcID, int taskID, int dbID, int useYuanBao)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID,
				taskID,
				dbID,
				useYuanBao
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 140));
		}

		public void SpriteGetFriends()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 142));
		}

		public void SpriteAddFriend(int dbID, string otherName, int friendType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				dbID,
				this.CurrentSession.RoleID,
				otherName,
				friendType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 143));
		}

		public void SpriteRemoveFriend(int dbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				dbID,
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 144));
		}

		public void SpriteClickOnGoodsPack(int autoID, int openState)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				autoID,
				openState
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 147));
		}

		public void SpriteGetThing(int autoID, int GoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				autoID,
				GoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 148));
		}

		public void SpriteUpdatePKMode(int pkMode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				pkMode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 149));
		}

		public void SpriteGetNewTaskData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 153));
		}

		public void SpriteAbandonTask(int dbID, int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 154));
		}

		public void SpriteModTask(int dbID, int taskID, int focus)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID,
				taskID,
				focus
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 139));
		}

		public void SpriteModKeys(int type, string keys)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				keys
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 156));
		}

		public void SpriteSendChat(int index, string fromRoleName, string toRoleName, string text, ChatType chatType = ChatType.TextOrSymbol, int Occ = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			int ptid = Global.Data.roleData.PTID;
			Occ = Global.Data.roleData.Occupation;
			string text2 = string.Empty;
			string text3 = "{0}:{1}:{2}:{3}:{4}:{5}:{6}";
			object[] array = new object[7];
			array[0] = this.CurrentSession.RoleID;
			array[1] = fromRoleName;
			array[2] = 0;
			array[3] = toRoleName;
			array[4] = index;
			array[5] = text;
			int num = 6;
			object[] array2 = new object[5];
			int num2 = 0;
			int num3 = (int)chatType;
			array2[num2] = num3.ToString();
			array2[1] = "_";
			array2[2] = Occ;
			array2[3] = "_";
			array2[4] = ptid;
			array[num] = string.Concat(array2);
			text2 = StringUtil.substitute(text3, array);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 157));
		}

		public void SpriteUseGoods(int id, int goodsID, int usenum = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new CS_SprUseGoods
			{
				RoleId = this.CurrentSession.RoleID,
				DbId = id,
				GoodsId = goodsID,
				UseNum = usenum
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 158));
		}

		public void SpriteForgeGoodsNew(int id, int rockGoodsID, int lucyGoodsID = -1, int luckyNum = 1, int isFirstToUseBind = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			Global.SendEvent("100", Global.GetLang("装备强化次数"));
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				rockGoodsID,
				lucyGoodsID,
				isFirstToUseBind
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 161));
		}

		public void SpriteForgeGoods(int id, int rockGoodsID, int shenyouGoodsID, int luckyNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				rockGoodsID,
				shenyouGoodsID,
				luckyNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 161));
		}

		public void SpriteSubForgeGoods(int id, int rockGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				rockGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 362));
		}

		public void SpriteUpdateGoodsBornIndex(int goodsDbID, int rockGoodsID, int canAutoBuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID,
				rockGoodsID,
				canAutoBuy
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 370));
		}

		public void SpriteDoEquipInherit(int leftGoodsDbID, int rightGoodsDbID, int rockGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("102", Global.GetLang("强化传承次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				leftGoodsDbID,
				rightGoodsDbID,
				rockGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 371));
		}

		public void SendJuHunChuanChengToServer(int leftGoodDBID, int rightGoodDBID, int subMoneyType)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				leftGoodDBID,
				rightGoodDBID,
				subMoneyType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1801));
		}

		public void SendJuHunToServer(int goodsId, int isUseShenShi, int isBindCaiLiao)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsId,
				isUseShenShi,
				isBindCaiLiao
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1800));
		}

		public void SpriteEnchanceGoods(int id, int rockGoodsID, int luckyNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				rockGoodsID,
				luckyNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 162));
		}

		public void SpriteGetOtherAttrib(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 163));
		}

		public void SpriteMallBuy(int mallID, int goodsNum, bool autoUseGold)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				mallID,
				goodsNum,
				(!autoUseGold) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 166));
		}

		public void SpriteYinLiangBuy(int mallID, int goodsNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				mallID,
				goodsNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 167));
		}

		public void SpriteGoodsExchange(int otherRoleID, int exchangeType, int exchangeID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				exchangeType,
				exchangeID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 170));
		}

		public void SpriteGoodsInstall(int stallType, int extTag1, string extTag2)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				stallType,
				extTag1,
				extTag2
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 173));
		}

		public void SpriteTeam(int teamType, int extTag1, int extTag2)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (teamType == 1)
			{
				Global.SendEvent("1800", Global.GetLang("创建队伍次数"));
			}
			if (teamType == 3)
			{
				Global.SendEvent("1802", Global.GetLang("邀请加入队伍次数"));
			}
			if (teamType == 4)
			{
				Global.SendEvent("1801", Global.GetLang("申请加入队伍次数"));
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				teamType,
				extTag1,
				extTag2
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 176));
		}

		public void SpriteCopyTeam(TeamCmds teamCmdType, long ID, int forceRequire, int autoStart, int kickNoReady = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.CurrentSession.RoleID,
				(int)teamCmdType,
				ID,
				forceRequire,
				autoStart,
				kickNoReady
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 621));
		}

		public void SpriteRegCopyTeamEventNotify(int fubenID, int extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				fubenID,
				extTag1,
				0
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 624));
		}

		public void SpriteBattle(int battleType, int extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				battleType,
				extTag1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 179));
		}

		public void SpriteArenaBattle(int battleType, int extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				battleType,
				extTag1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 415));
		}

		public void SpriteRunNPCScript(int npcID, int scriptID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID,
				scriptID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 180));
		}

		public void SpriteDead()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (Global.Data.roleData == null)
			{
				return;
			}
			if (Global.Data.roleData.LifeV > 0)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 181));
		}

		public void SpriteAutoFight(int fightType, int extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				fightType,
				extTag1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 182));
		}

		public void SpriteHorse(int horseType, int horseDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				horseType,
				horseDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 183));
		}

		public void SpriteHorseEnchance(int horseDbID, int extIndex, int allowAutoBuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID,
				extIndex,
				allowAutoBuy
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 219));
		}

		public void SpriteHorseUpgrade(int horseDbID, int allowAutoBuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID,
				allowAutoBuy
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 220));
		}

		public void SpritePet(int petType, int extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				petType,
				extTag1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 184));
		}

		public void SpriteChangePos(int roleID, int mapCode, int toX, int toY, int toDirection)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				roleID,
				mapCode,
				toX,
				toY,
				toDirection
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 159));
		}

		public void SpriteDianjiangList(int roleID, int viewType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				viewType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 185));
		}

		public void SpriteDianJiang(int djCmdType, int extTag1, string extTag2)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				djCmdType,
				extTag1,
				extTag2
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 188));
		}

		public void SpriteGotToMap(int toMapCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (null != PlayZone.GlobalPlayZone && PlayZone.GlobalPlayZone.BaiTanState())
			{
				return;
			}
			Super.HideNetWaiting();
			Global.IsFirstPopupDownloadMapWindowInWorldMap = false;
			if (Global.IsPopupDownloadMapWindow(toMapCode))
			{
				Global.IsFirstPopupDownloadMapWindowInWorldMap = true;
				string[] buttons = new string[]
				{
					Global.GetLang("立即下载"),
					Global.GetLang("稍后下载")
				};
				string lang = Global.GetLang(string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(toMapCode)));
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					if (e1.ID == 0)
					{
						string text2 = string.Empty;
						text2 = StringUtil.substitute("{0}:{1}", new object[]
						{
							this.CurrentSession.RoleID,
							toMapCode
						});
						this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 193));
					}
					else
					{
						Global.IsFirstPopupDownloadMapWindowInWorldMap = false;
					}
				}, buttons);
			}
			else
			{
				string text = string.Empty;
				text = StringUtil.substitute("{0}:{1}", new object[]
				{
					this.CurrentSession.RoleID,
					toMapCode
				});
				this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 193));
			}
		}

		public void SpriteRunToMap(int toMapCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				toMapCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 231));
		}

		public void SpriteQueryIDByName(string otherName, int opCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherName,
				opCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 195));
		}

		public void SpriteGetHorsesList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 198));
		}

		public void SpriteGetOtherHorsesList(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 199));
		}

		public void SpriteGetPetsList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 200));
		}

		public void SpriteRemoveHorse(int horseDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 201));
		}

		public void SpriteModPet(int petDbID, int modType, string extTag1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				petDbID,
				modType,
				extTag1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 202));
		}

		public void SpriteSelectHorse(int horseDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 203));
		}

		public void SpriteGetGoodsDataListBySite(int site)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				site
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 204));
		}

		public void SpriteGetJinDanGoodsDataList(int site)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				site
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 450));
		}

		public void SpriteGetJingLingGoodsDataList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 750));
		}

		public void SpriteGetDJPoints()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 191));
		}

		public void SpriteGetLineInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				TimeManager.GetCorrectLocalTime()
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 205));
		}

		public void SpriteGoodsMergeTypes(int npcID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 134));
		}

		public void SpriteGoodsMergeItems(int mergeTypeID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				mergeTypeID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 135));
		}

		public void SpriteGoodsMergeMsg(int mergeItemID, int luckyGoodsID, int chiBangDBid, int jingShiDBid, int UseBindItemFirst)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (mergeItemID == 1)
			{
				Global.SendEvent("500", Global.GetLang("合成恶魔凭证次数"));
			}
			if (mergeItemID == 2)
			{
				Global.SendEvent("501", Global.GetLang("合成透明披风次数"));
			}
			if (mergeItemID == 50)
			{
				Global.SendEvent("502", Global.GetLang("合成果实次数"));
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.CurrentSession.RoleID,
				mergeItemID,
				luckyGoodsID,
				chiBangDBid,
				jingShiDBid,
				UseBindItemFirst
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 136));
		}

		public void SpriteGetJingMaiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 218));
		}

		public void SpriteGetJingMaiList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 206));
		}

		public void SpriteGetOtherJingMaiList(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 208));
		}

		public void SpriteUpJingMai(int jingMaiBodyLevel, int jingMaiID, int luckyPercent)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				jingMaiBodyLevel,
				jingMaiID,
				luckyPercent
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 207));
		}

		public void SpriteLoadAlready(int ohterRoleID)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ohterRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 209));
		}

		public void SpriteEquipUpgrade(int goodsDbID, int ironNum, int goldRock, int luckyNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID,
				ironNum,
				goldRock,
				luckyNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 212));
		}

		public void SpriteEnchaseJewel(int actionType, int equipGoodsDbID, int jewelGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				actionType,
				equipGoodsDbID,
				jewelGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 213));
		}

		public void SpriteShowBiGuanInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 214));
		}

		public void SpriteGetBiGuanInfo(int actionType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				actionType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 215));
		}

		public void SpriteUpSkillLevel(int skillDbID, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			if (type == 1)
			{
				Global.SendEvent("302", Global.GetLang("技能升级次数"));
			}
			else if (type == 2)
			{
				Global.SendEvent("301", Global.GetLang("技能提升熟练度次数"));
			}
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				skillDbID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 216));
		}

		public void SendFucklist(byte[] data)
		{
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, data, 0, data.Length, 699));
		}

		public void SpriteSaleGoods(int goodsDbID, int site, int saleMoney1, int saleYuanBao, int saleYinPiao, int saleGoodsCount = -1)
		{
			this.SpriteSaleGoods2(goodsDbID, site, saleMoney1, saleYuanBao, saleYinPiao, saleGoodsCount);
		}

		public void SpriteSaleGoods2(int goodsDbID, int site, int saleMoney1, int saleYuanBao, int saleYinPiao, int saleGoodsCount = -1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID,
				site,
				saleMoney1,
				saleYuanBao,
				saleYinPiao,
				saleGoodsCount
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 654));
		}

		public void SpriteSelfSaleGoodsList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 222));
		}

		public void SpriteSelfSaleGoodsList2()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 655));
		}

		public void SpriteOtherSaleGoodsList(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 223));
		}

		public void SpriteMarketRoleList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 224));
		}

		public void SpriteMarketGoodsList(int marketSearchType, string marketSearchText)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				marketSearchType,
				marketSearchText
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 225));
		}

		public void SpriteMarketGoodsList2(int marketSearchType, string marketSearchText, int startIndex = 0, int maxCount = 4)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				marketSearchType,
				startIndex,
				maxCount,
				marketSearchText
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 658));
		}

		public void SpriteMarketBuyGoods(int goodsDbID, int goodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID,
				goodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 226));
		}

		public void SpriteMarketBuyGoods2(int goodsDbID, int goodsID, int moneyType, int salePrice)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID,
				goodsID,
				moneyType,
				salePrice
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 659));
		}

		public void SpriteUpdateChengJiuLevel()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				-1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 670));
		}

		public void SpriteModDefSkillID(int skillID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				skillID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 227));
		}

		public void SpriteModAutoDrink(int autoLifeV, int autoMagicV)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				autoLifeV,
				autoMagicV
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 228));
		}

		public void SpritePlayDeco(int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1 = -1, int toY1 = -1, int moveTicks = 0, int alphaTicks = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
			{
				this.CurrentSession.RoleID,
				decoID,
				decoType,
				toBody,
				toX,
				toY,
				shakeMap,
				toX1,
				toY1,
				moveTicks,
				alphaTicks
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 229));
		}

		public void SpriteSearchRoles(string roleName, int startIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			roleName = this.StringReplaceAll(roleName, ":", string.Empty);
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				roleName,
				startIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 232));
		}

		public void SpriteSearchRolesFromDB(string roleName, int startIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			roleName = this.StringReplaceAll(roleName, ":", string.Empty);
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				roleName,
				startIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 310));
		}

		public void SpriteListMapRoles(int startIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				startIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 233));
		}

		public void SpriteListAllTeams(int startIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				startIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 234));
		}

		public void SpriteListAllCopyTeams(int startIndex, int fubenID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				startIndex,
				fubenID,
				0
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 620));
		}

		public void SpriteResetBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 235));
		}

		public void RebornShow()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2051));
		}

		public void RebornModelShow()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2059));
		}

		public void SendChongShengDaKong(int DbId, int key, bool IsBinDing, bool IsCreal)
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			if (IsBinDing)
			{
				text2 = "1";
			}
			else
			{
				text2 = "0";
			}
			if (IsCreal)
			{
				text3 = "1";
			}
			else
			{
				text3 = "0";
			}
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				DbId,
				key,
				text2,
				text3
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2050));
		}

		public void SendChongShengXiangQian(int equipDbId, int baoShiId, int key)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				equipDbId,
				baoShiId,
				key
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2054));
		}

		public void SendChongShengXieXia(int equipDbId, int key)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				equipDbId,
				key
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2055));
		}

		public void SendChongShengBuy(int goodid, int number)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				goodid,
				number
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2056));
		}

		public void SendChongShengFenJie(int goodid, int number, int binDing)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				goodid,
				number,
				binDing
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2058));
		}

		public void SendChongShengPiLiangFenJie(string ids)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				ids
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2060));
		}

		public void SendXuanCaiHeCheng(int DBID1, int num1, int DBID2, int num2, int DBID3, int num3)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				DBID1,
				num1,
				DBID2,
				num2,
				DBID3,
				num3
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2057));
		}

		public void SendGuanZhu(int holeID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				holeID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2095));
		}

		public void SendCuiLian(int holeID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				holeID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2096));
		}

		public void SendBoCaiNumberData(BoCaiTypeEnum BocaiType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(int)BocaiType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2083));
		}

		public void SendBoCaiXiaZhu(BoCaiTypeEnum BocaiType, int BuyNum, string[] numbers)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			if (numbers.Length == 5)
			{
				text = string.Format("{0},{1},{2},{3},{4}", new object[]
				{
					numbers[0],
					numbers[1],
					numbers[2],
					numbers[3],
					numbers[4]
				});
			}
			else
			{
				if (numbers.Length != 1)
				{
					return;
				}
				text = numbers[0];
			}
			Super.ShowNetWaiting(null);
			string text2 = string.Empty;
			text2 = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				(int)BocaiType,
				BuyNum,
				text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 2082));
		}

		public void SendBoCaiDaiBi(int count, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				count,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2087));
		}

		public void SendBoCaiShangChengData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2085));
		}

		public void SendBoCaiShangChengBuy(int Id, int number, string WuPinID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				Id,
				number,
				WuPinID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2086));
		}

		public void SendBoCaiCloseTuiSong(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2088));
		}

		public void SpriteResetChongShengBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2041));
		}

		public void SpriteChangeNumSkillID(int skillID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				skillID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 238));
		}

		public void SpriteGetSkillUsedNum(int skillID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				skillID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 239));
		}

		public void SpriteChangeHorseBody(int horseDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 240));
		}

		public void SpriteResetPortableBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 242));
		}

		public void SpriteResetRebornPortableBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2042));
		}

		public void SpriteResetJinDanBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 449));
		}

		public void SpriteResetJingLingBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 754));
		}

		public void SpriteExecWaBaoByYaoShi(int idXiangzi, int idYaoShi, int autoBuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				idXiangzi,
				idYaoShi,
				autoBuy
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 361));
		}

		public void SpriteExecWaBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 243));
		}

		public void SpriteGetWaBaoGoods()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 244));
		}

		public void SpriteGetHuoDongData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 245));
		}

		public void SpriteGetWLoginGift(int whichOne)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				whichOne
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 246));
		}

		public void SpriteGetLimitTimeLoginGift(int whichOne)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				whichOne
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 455));
		}

		public void SpriteGeNewStepGift(int step)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				step
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 247));
		}

		public void SpriteGetMTimeGift(int whichOne)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				whichOne
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 248));
		}

		public void SpriteGetBigGift(int whichOne)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				whichOne,
				this.CurrentSession.roleData.BigAwardID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 249));
		}

		public void SpriteGetSongLiGift(string liPinMa)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			liPinMa = this.StringReplaceAll(liPinMa, ":", string.Empty);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				liPinMa,
				this.CurrentSession.roleData.SongLiID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 250));
		}

		public void SpriteEnterFuBen(int fuBenID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.HideNetWaiting();
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					if (PlayZone.OnPreChangeMap(-1, 0))
					{
						return;
					}
					int fuBenMapCode = Global.GetFuBenMapCode(fuBenID);
					if (Global.IsPopupDownloadMapWindow(fuBenMapCode))
					{
						Global.IsFuBenFenBaoMap = true;
						string[] buttons = new string[]
						{
							Global.GetLang("立即下载"),
							Global.GetLang("稍后下载")
						};
						string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(fuBenMapCode));
						Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
						{
							if (e1.ID == 0)
							{
								string text2 = string.Empty;
								text2 = StringUtil.substitute("{0}:{1}", new object[]
								{
									this.CurrentSession.RoleID,
									fuBenID
								});
								Super.ShowNetWaiting(null);
								this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 253));
							}
							else
							{
								Global.IsFuBenFenBaoMap = false;
							}
						}, buttons);
					}
					else
					{
						string text = string.Empty;
						text = StringUtil.substitute("{0}:{1}", new object[]
						{
							this.CurrentSession.RoleID,
							fuBenID
						});
						Super.ShowNetWaiting(null);
						this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 253));
					}
				}
			}, fuBenID);
		}

		public void SpriteRiChangTaskShuaXing(int taskID, int dbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				taskID,
				dbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 526));
		}

		public void SpriteRiChangTaskYiJianWanCheng(int npcID, int npcCExtensionID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				npcID,
				npcCExtensionID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 527));
		}

		public void SpriteAdmiredOperation(int targetID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				targetID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 528));
		}

		public void SpriteBloodCastleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 529));
		}

		public void SpriteQureyBattleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 534));
		}

		public void SpriteQureyDaimonSquareInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 535));
		}

		public void SpriteSaoDangFuBen(int mapCode, int fuBenID, int huiShou)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode,
				fuBenID,
				huiShou
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 520));
		}

		public void SpriteNotifyEnterFuBen(int fuBenID, int fuBenSeqID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				fuBenID,
				fuBenSeqID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 254));
		}

		public void SpriteHeart()
		{
			byte[] array = new SCClientHeart
			{
				RoleID = this.CurrentSession.RoleID,
				RandToken = this.CurrentSession.RoleRandToken,
				ReportCliRealTick = DateTime.Now.Ticks
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 23));
			TCPPing.RecordSendCmd(23);
		}

		public void SpriteGetFuBenHist(int fuBenID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				fuBenID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 259));
		}

		public void SpriteGetFuBenBeginInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 260));
		}

		public void SpriteGetFuBenMonstersNum()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 261));
		}

		public void SpriteFindMonsterPosition(int centerX, int centerY, int radiusGridNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SCFindMonster
			{
				RoleID = this.CurrentSession.RoleID,
				X = centerX,
				Y = centerY,
				Num = radiusGridNum
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 262));
		}

		public void SpriteBatchYinPiao(int yinPiaoNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				yinPiaoNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 263));
		}

		public void SpriteForceToLaoFang()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 264));
		}

		public void SpriteGetRoleDailyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 267));
		}

		public void SpriteGetBossInfoDictData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 268));
		}

		public void SpriteGetPaiHang(int paiHangType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				paiHangType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 269));
		}

		public void SpriteStartYaBiao(int yaBiaoID, int needGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				yaBiaoID,
				needGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 271));
		}

		public void SpriteEndYaBiao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 272));
		}

		public void SpriteTakeYaBiaoGoods()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 273));
		}

		public void SpriteYaBiaoTouBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 274));
		}

		public void SpriteGetOtherAttrib2(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 275));
		}

		public void SpriteFindBiaoChe()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 278));
		}

		public void SpriteAddHorseLucky(int horseDbID, int luckyGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID,
				luckyGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 281));
		}

		public void SpriteGetChongZhiJiFen()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 287));
		}

		public void SpriteGetFuBenHistListData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 289));
		}

		public void SpriteQureyFuBenInfo(int mapCode, int fuBenID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new CS_QueryFuBen
			{
				RoleId = this.CurrentSession.RoleID,
				MapId = mapCode,
				FuBenId = fuBenID
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 522));
		}

		public void SpriteGetOtherHorsesData(int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 291));
		}

		public void SpriteGetBangHuiListData(int isVerify, int startIndex, int endIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				isVerify,
				startIndex,
				endIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 294));
		}

		public void SpriteCreateBangHui(string bangHuiName, string bhBulletin)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bangHuiName,
				bhBulletin
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 295));
		}

		public void SpriteQueryBangHuiDetail(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 297));
		}

		public void SpriteUpdateBangHuiBulletinMsg(int bhid, string bhBulletinMsg)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				bhBulletinMsg
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 298));
		}

		public void SpriteGetBangHuiMemberDataList(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 299));
		}

		public void SpriteUpdateBHVerify(int bhid, int isVerify)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				isVerify
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 300));
		}

		public void SpriteApplyToBHMember(int bhid, string bhName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				bhName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 301));
		}

		public void SpriteBeInvitedByBHVerify(int isVerify)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				isVerify
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 306));
		}

		public void SpriteUpdateBHMemberZhiWu(int bhid, int otherRoleID, int zhiWu)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("704", Global.GetLang("委任战盟官员次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				otherRoleID,
				zhiWu
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 308));
		}

		public void SpriteUpdateBHMemberChengHao(int bhid, int otherRoleID, string chengHao)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				otherRoleID,
				chengHao
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 309));
		}

		public void SpriteAddBHMember(int bhid, int otherRoleID, string otherRoleName, int toVerify)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				otherRoleID,
				otherRoleName,
				toVerify
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 302));
		}

		public void SpriteRemoveBHMember(int bhid, int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 303));
		}

		public void SpriteQuitFromBangHui(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 304));
		}

		public void SpriteDestroyBangHui(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 305));
		}

		public void SpriteChangeBangHuiName(string newname)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				newname
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 14006));
		}

		public void SpriteAgreeToBHMember(int otherRoleID, int bhid, string bhName, int agreeTo)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				bhid,
				bhName,
				agreeTo
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 311));
		}

		public void SpriteRefuseApplyToBHMember(int bhid, int otherRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				otherRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 312));
		}

		public void SpriteGetBangGongHist(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 313));
		}

		public void SpriteDonateBGMoney(int type, int count)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				type,
				count
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 314));
		}

		public void SpriteDonateBGMoney(int money1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				money1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 314));
		}

		public void SpriteDonateBGGoods(int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				goods1Num,
				goods2Num,
				goods3Num,
				goods4Num,
				goods5Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 315));
		}

		public void SpriteGetBangQiInfo(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 317));
		}

		public void SpriteRenameBangQi(string bhQiName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				bhQiName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 318));
		}

		public void SpriteUpLevelBangQi(int toLevel)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				toLevel
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 319));
		}

		public void SpriteGetBHLingDiInfoDictByBHID(int bhid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 325));
		}

		public void SpriteSetLingDiTax(int lingDiID, int newLingDiTax)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				lingDiID,
				newLingDiTax
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 326));
		}

		public void SpriteTakeTaxMoney(int lingDiID, int takeTaxMoney)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				lingDiID,
				takeTaxMoney
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 327));
		}

		public void SpriteGetHuangDiBHInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 328));
		}

		public void SpriteOpenYangGongBK(bool allowAutobuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(!allowAutobuy) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 330));
		}

		public void SpriteFreeRefreshYangGongBK()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 331));
		}

		public void SpriteClickYangGongBK(int bkIndex, bool allowAutobuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bkIndex,
				(!allowAutobuy) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 332));
		}

		public void SpriteRefreshQiZhenGe()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 333));
		}

		public void SpriteQiZhenGeBuy(int itemID, int goodsNum, bool autoUseGold)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				itemID,
				goodsNum,
				(!autoUseGold) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 334));
		}

		public void SpriteQueryQiZhenGeHist()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 335));
		}

		public void SpriteQuickJingMai(int jingMaiID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				jingMaiID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 336));
		}

		public void SpriteQuickHorseEnchance(int horseDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				horseDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 337));
		}

		public void SpriteQuickEquipEnchance(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 338));
		}

		public void SpriteQuickEquipForge(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 339));
		}

		public void SpriteGetHuangDiRoleData(int huangDiRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				huangDiRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 340));
		}

		public void SpriteAddHuangFei(int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 341));
		}

		public void SpriteRemoveHuangFei(int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 342));
		}

		public void SpriteGetHuangFeiDataList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 343));
		}

		public void SpriteSendToLaoFang(int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 344));
		}

		public void SpriteTakeOutLaoFang(int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 345));
		}

		public void SpriteBanChatToWorldMap(int otherRoleID, string otherRoleName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleID,
				otherRoleName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 346));
		}

		public void SpriteGetLingDiMapInfoData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 348));
		}

		public void SpriteGetHuangChengMapInfoData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 349));
		}

		public void SpriteAgreeAddHuangFei(int huangDiRoleID, string huangDiRoleName, int randNum, int agreeState)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				huangDiRoleID,
				huangDiRoleName,
				randNum,
				agreeState
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 352));
		}

		public void SpriteTaskTransport(int mapCode, int toPosX, int toPosY, int useChuanSongJuan = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (PlayZone.OnPreChangeMap(mapCode, 10))
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode,
				toPosX,
				toPosY,
				useChuanSongJuan
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 353));
		}

		public void SpriteTaskTransport2(int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 433));
		}

		public void SpriteLingLiGuanZhu(int doWork)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				doWork
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 354));
		}

		public void SpriteGetGoodsByDbID(int roleID, int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 355));
		}

		public void SpriteQuickCompleteTaskByID(int roleID, int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 356));
		}

		public void SpriteQueryChongZhiMoney(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 357));
		}

		public void SpriteGetFirstChongZhiDaLi(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 358));
		}

		public void SpriteGetDayChongZhiDaLi(int roleID, int activityType, int btnIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				roleID,
				activityType,
				btnIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 459));
		}

		public void SpriteGetUserMailList(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 363));
		}

		public void SpriteGetUserMailData(int roleID, int mailID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				mailID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 364));
		}

		public void SpriteFetchMailGoods(int roleID, int mailID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				mailID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 365));
		}

		public void GetAllMallGoods(int nRoleID, string strMailID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				nRoleID,
				strMailID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 672));
		}

		public void SpriteDeleteUserMail(int roleID, string mailIDs)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				roleID,
				mailIDs
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 366));
		}

		public void SpriteGetMailSendCode(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 367));
		}

		public void SpriteSendUserMail(int roleID, string strcmd)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, strcmd, 368));
		}

		public void SpriteMallZhenQiBuy(int mallID, int goodsNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				mallID,
				goodsNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 387));
		}

		public void SpriteQueryInputFanLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 372));
		}

		public void SpriteQueryInputSongLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 373));
		}

		public void SpriteQueryInputKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 374));
		}

		public void SpriteQueryLevelKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 375));
		}

		public void SpriteQueryEquipKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 376));
		}

		public void SpriteQueryHorseKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 377));
		}

		public void SpriteQueryJingMaiKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 378));
		}

		public void SpriteQueryAwardHistory(int activityType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 379));
		}

		public void SpriteFetchActivityAward(int activityType, int extTag = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType,
				extTag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 388));
		}

		public void RequestMeiRiPlatformAward(int id)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				id
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1809));
		}

		public void SpriteQueryNewZoneLevelKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 630));
		}

		public void SpriteQueryNewZoneBossKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				36
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 631));
		}

		public void SpriteQueryNewZoneChongZhiKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				34
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 631));
		}

		public void SpriteQueryNewZoneXiaoFeiKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				35
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 631));
		}

		public void SpriteQueryNewZoneFanLiKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				37
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 631));
		}

		public void SpriteQueryUpLevelGiftFlagList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 632));
		}

		public void SpriteGetUpLevelGiftFlagList(int ID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 633));
		}

		public void SpriteQueryZhanDouLiGiftFlagList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 784));
		}

		public void SpriteGetZhanDouLiGiftFlagList(int ID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 785));
		}

		public void SpriteFetchNewZoneActivityAward(int activityType, int extTag = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType,
				extTag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 629));
		}

		public void SpriteFetchVipDailyAward(int priority)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				priority
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 390));
		}

		public void SpriteQueryVipDailyDataList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 389));
		}

		public void SpriteGetVipInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 593));
		}

		public void SpriteActivityTransport(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 391));
		}

		public void SpriteQueryYangGongBKDailyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 392));
		}

		public void SpriteFetchYangGongBKJiFenAward(int awardNo)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				awardNo
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 393));
		}

		public void SpriteQueryShiLianTaAwardInfoData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 394));
		}

		public void SpriteFetchShiLianTaFuBenAward(int leave, int allowAutoBuy)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				leave,
				allowAutoBuy
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 395));
		}

		public void SpriteFetchTinyClientAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 396));
		}

		public void SpriteAddShengXiaoGuessMortgage(string mortgages, bool allowAutoBuy = false)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				mortgages,
				(!allowAutoBuy) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 400));
		}

		public void SpriteQuerySelfShengXiaoGuessMortgageList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 401));
		}

		public void SpriteQueryShengXiaoGuessHistoryList(bool allHisotry = true)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(!allHisotry) ? this.CurrentSession.RoleID : -1
			});
			int num = 402;
			if (!allHisotry)
			{
				num = 404;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, num));
		}

		public void SpriteUpdateTengXunFcmRate(double rate)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				rate
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 405));
		}

		public void SpriteExtGridByYuanBao(double addGridNum, int type, int maxUseZuanShi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				addGridNum,
				type,
				maxUseZuanShi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 408));
		}

		public void SpriteExtRebornGridByYuanBao(double addGridNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				addGridNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2048));
		}

		public void SpriteRequstSubMoney(int subType, int params1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				subType,
				params1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 409));
		}

		public void SpriteExtBagNumByYuanBao(double addGridNum, int type, int maxUseZuanShi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				addGridNum,
				type,
				maxUseZuanShi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 410));
		}

		public void SpriteExtRebornBagNumByYuanBao(double addGridNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				addGridNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2049));
		}

		public void SpriteCityWarRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 417));
		}

		public void SpriteTakeLingDiDailyAward(int lingDiID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.Faction,
				lingDiID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 418));
		}

		public void SpriteFetchChengJiuAward(int chengJiuID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				chengJiuID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 421));
		}

		public void SpriteQueryChengJiuData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 420));
		}

		public void SpriteMendEquipment(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 425));
		}

		public void SpriteEquipmentFenJie(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 428));
		}

		public void SpriteJingYuanExchange(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 429));
		}

		public void SpriteHuiZhangExchange(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 430));
		}

		public void SpriteHunqiExchange(int goodsDbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				goodsDbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 507));
		}

		public void SpriteActivateNextJingMaiLevel()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 431));
		}

		public void SpriteActivateNextWuXueLevel()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 434));
		}

		public void SpriteFetchVipOnceAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 432));
		}

		public void SpriteCaiJi(int caiJiRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				caiJiRoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 435));
		}

		public void SpriteRunTaskPlotLua(int taskPlotID, int startOrEnd)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				taskPlotID,
				startOrEnd
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 436));
		}

		public void SpriteChangePetAiType(int petMonsterRoleID, int aiType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				petMonsterRoleID,
				aiType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 439));
		}

		public void SpriteTransferSomething(int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 438));
		}

		public void SpriteFetchMallData(int dataType = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				dataType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 440));
		}

		public void SpriteFetchActivtData(int dataType = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				dataType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 478));
		}

		public void SpriteMallQiangGouBuy(int qiangGouID, int goodsNum, bool autoUseGold, int goodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				qiangGouID,
				goodsNum,
				(!autoUseGold) ? 0 : 1,
				goodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 441));
		}

		public void SpriteFetchZuanHuangWeekAward(int justcheck = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				justcheck
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 442));
		}

		public void SpriteSetSystemOpenParams(int activateIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				activateIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 443));
		}

		public void SpriteEnterTaskFuBen(int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 444));
		}

		public void SpriteGetTaskAwards(int taskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				taskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 447));
		}

		public void SpriteNotifyGetGoodsPack()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 448));
		}

		public void SpriteZaJinDan(int times)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				times
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 451));
		}

		public void SpriteQueryZaJinDanHistoryList(bool allHisotry = true)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(!allHisotry) ? this.CurrentSession.RoleID : -1
			});
			int num = 452;
			if (!allHisotry)
			{
				num = 453;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, num));
		}

		public void SpriteGetTo60Award(int awardID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				awardID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 457));
		}

		public void SpriteGetKaiFuOnline()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.roleData.ZoneID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 458));
		}

		public void SpriteGetJieriXmlData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Global.JieriXML_Version
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 460));
		}

		public void SpriteQueryJieriDaLiBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 461));
		}

		public void SpriteQueryLeijiXF()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 683));
		}

		public void SpriteQueryJieriDengLu()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 462));
		}

		public void SpriteQueryJieriVIP()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 463));
		}

		public void SpriteQueryJieriCZSong()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 464));
		}

		public void SpriteQueryLeijiCZ()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 465));
		}

		public void SpriteQueryZiKa(int activityType = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 466));
		}

		public void SpriteQueryJieriXiaoFeiKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 467));
		}

		public void SpriteQueryJieriCZKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 468));
		}

		public void GetRedemptionActivityInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1500));
		}

		public void SpriteQueryHeFuDaLiBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 480));
		}

		public void SpriteQueryHeFuVIP()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 481));
		}

		public void SpriteQueryHeFuCZSong()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 482));
		}

		public void SpriteQueryHeFuPKKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 484));
		}

		public void SpriteQueryWCKing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 485));
		}

		public void SpriteQueryHeFuFanLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 483));
		}

		public void SpriteQueryXinFanLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 486));
		}

		public void SpriteOneKeyQuickSaleOut(int oType, string goodsDbIds)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("3100", Global.GetLang("回收魔晶次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				oType,
				goodsDbIds
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 494));
		}

		public void SpriteChongShengOneKeyQuickSaleOut(string goodsDbIds)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsDbIds
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2046));
		}

		public void SpriteActiveNextLevelZhanHun()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 495));
		}

		public void SpriteActiveNextLevelRongYu()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 496));
		}

		public void SpriteActiveRongYuBuffer()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 497));
		}

		public void SpriteLianLuJingLian(int id1, int id2, int id3, int luckyGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				id1,
				id2,
				id3,
				luckyGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 498));
		}

		public void SpriteGetZaJinDanJiFen()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 499));
		}

		public void SpriteGetZaJinDanJiFenAwards(int awardsNo)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				awardsNo
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 500));
		}

		public void SpriteQueryActivityInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 501));
		}

		public void SpriteXingYunChouJiang()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 502));
		}

		public void SpriteQueryYueDuChouJiangInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 506));
		}

		public void SpriteQueryYueDuChouJiangHistory(bool allHisotry)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(!allHisotry) ? this.CurrentSession.RoleID : -1
			});
			int num = 503;
			if (!allHisotry)
			{
				num = 504;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, num));
		}

		public void SpriteExecuteYueDuChouJiang()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 505));
		}

		public void SpriteExecuteZhuanSheng()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 509));
		}

		public void SpriteExecuteZhuanZhi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 508));
		}

		public void SpriteExecuteFuBenJiangLiChouJiang(int count, params int[] indexes)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID
			});
			foreach (int num in indexes)
			{
				text = text + ":" + num;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 508));
		}

		public void SpriteExecuteFuBenJiangLiLingQu()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 508));
		}

		public void SpriteGetUsingGoodsDataList(int roleID, bool bIsSelectRole = false)
		{
			if (bIsSelectRole)
			{
				if (this.ActiveDisconnect)
				{
					return;
				}
			}
			else if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 512));
		}

		public void SpriteEquipAppendPropCmd(int id, int baohuGoodsID, int isFirstToUseBind = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			Global.SendEvent("101", Global.GetLang("装备追加次数"));
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				baohuGoodsID,
				isFirstToUseBind
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 530));
		}

		public void SpriteQureyeEverydayOnlineAwardGiftInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 539));
		}

		public void SpriteGetEveryDayOnLineAwardGiftCmd(int nTimer)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nTimer
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 540));
		}

		public void SpriteQureyeEverydaySeriesLoginInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 541));
		}

		public void SpriteMonthCardInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 850));
		}

		public void SpriteGetMonthCardAwardCard(int getDay)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				getDay
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 851));
		}

		public void SpriteRequestMarry(int otherRoleId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(string.Empty);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 890));
		}

		public void SpriteResponseMarry(int otherRoleId, int agree)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(string.Empty);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherRoleId,
				agree
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 891));
		}

		public void SpriteAutoRefuseMarry(int auto)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(string.Empty);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				auto
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 893));
		}

		public void SpriteRequestDivorce(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 892));
		}

		public void SpriteXianHua(int roseId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roseId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 871));
		}

		public void SpriteLoveFuben(int state, int dupId = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				state,
				dupId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 870));
		}

		public void SpriteChangeRing(int roseId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roseId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 872));
		}

		public void ApplyHunYan(int Type, long Time)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				Type,
				Time
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 881));
		}

		public void CancelHunYan()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 882));
		}

		public void SpriteJoinLoveHunYan(int junBanRoleId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				junBanRoleId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 883));
		}

		public void GetHunYanListInfo(bool isSelf)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				(!isSelf) ? -2 : this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 880));
		}

		public void SpriteGetSeriesLoginAwardGiftCmd(int nTimes = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nTimes
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 542));
		}

		public void SpriteUpdateGetThingFlagCmd(int flag)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				flag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 544));
		}

		public void SpriteGetExchangeMoJingAndQiFuCmd(int duhuanID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				duhuanID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 548));
		}

		public void SpriteGetMeditateExpCmd(int nStep)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nStep
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 549));
		}

		public void SpriteGetMeditateTimeInfoCmd()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 550));
		}

		public void SpriteQueryTotalLoginInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 551));
		}

		public void SpriteGetTotalLoginInfoCmd(int nIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 552));
		}

		public void SpriteQueryCleanPropAddPointInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 516));
		}

		public void SpriteQueryEquipChangeLifeCmd(int id, int rockGoodsID, int lucyGoodsID = -1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				rockGoodsID,
				lucyGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 553));
		}

		public void SpriteQueryFlakeOffEquipChangeLifeCmd(int id, int moneyType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				moneyType,
				-1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 554));
		}

		public void SpriteOneKeyFindFriendCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 555));
		}

		public void SpriteOneKeyAddFriendCmd(string rolelist)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				rolelist
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 556));
		}

		public void SpriteGetVipAwardCmd(int awardID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				awardID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 557));
		}

		public void SpriteGetDailyActiveInforCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 558));
		}

		public void SpriteGetDailyActiveAwardCmd(int awardID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				awardID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 559));
		}

		public void SpriteSetAutoAssignPropertyPointCmd(int nFlag)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nFlag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 560));
		}

		public void SpriteGetBloodcastleAwardCmd(int nValue)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nValue
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 561));
		}

		public void SpriteGetDaimonSquareAwardCmd(int nValue)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nValue
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 562));
		}

		public void SpriteGetCopyMapAwardCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 563));
		}

		public void SpriteQueryBloodCastlePlayerNumCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 545));
		}

		public void SpriteQueryDaimonSquarePlayerNumCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 546));
		}

		public void SpriteQueryExperienceCopyMapInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 565));
		}

		public void SpriteGetZhanMengShiJianDetailCmd(int idx)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				idx
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 566));
		}

		public void SpriteGetSkillInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 564));
		}

		public void SpriteGetKaiFuActivityInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 567));
		}

		public void SpriteGetTheKingOfPKActivityInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 568));
		}

		public void SpriteAngelTempleGuLiInfoCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 574));
		}

		public void SpriteGetAngelTempleInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 575));
		}

		public void SpriteGetPKKingAdrationInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 576));
		}

		public void SpritePKKingAdrationCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (nType == 1)
			{
				Global.SendEvent("1100", Global.GetLang("金币膜拜次数"));
			}
			if (nType == 2)
			{
				Global.SendEvent("1101", Global.GetLang("钻石膜拜次数"));
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 577));
		}

		public void SpriteGetShiLiAdrationInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1136));
		}

		public void SpriteShiLiAdrationCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1137));
		}

		public void SpriteJingJiDetailCmd(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 578));
		}

		public void SpriteRequestJingJiChallengeCmd(int beChallengerId, int beChallengerRanking, int enterType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (PlayZone.OnPreChangeMap(-1, 0))
			{
				return;
			}
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					string text = string.Empty;
					text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
					{
						this.CurrentSession.RoleID,
						beChallengerId,
						beChallengerRanking,
						enterType
					});
					this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 579));
				}
			}, -1);
		}

		public void SpriteGetChallengeInfoListCmd(int pageIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				pageIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 582));
		}

		public void SpriteJingJiRankingRewardCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 583));
		}

		public void SpriteJingJiRemoveChallengeCDCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("1300", Global.GetLang("竞技场挑战次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 584));
		}

		public void SpriteGetJingJiRankingBuffCmd(bool replace)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("1400", Global.GetLang("军衔护体使用次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(!replace) ? 0 : 1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 585));
		}

		public void SpriteGetJingJiJunxianLevelupCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 586));
		}

		public void SpriteGetJingJiJunxianLeaveCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 587));
		}

		public void SpriteJingjiStartFight()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 634));
		}

		public void SpriteOpenMarketCmd(string marketName, int offlineMarket = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				offlineMarket,
				marketName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 591));
		}

		public void SpriteMarketSaleMoneyCmd(int saleOutMoney, int yuanBaoPrice)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				saleOutMoney,
				yuanBaoPrice
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 592));
		}

		public void SpriteMarketSaleMoneyCmd2(int saleOutMoney, int yuanBaoPrice)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				saleOutMoney,
				yuanBaoPrice
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 653));
		}

		public void SpriteGetVIPInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 593));
		}

		public void SpriteGetVipLeveLaward(int nIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 594));
		}

		public void SpriteGetLiXianBaiTanSecsCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 596));
		}

		public void SpriteBuyLiXianBaiTanSecsCmd(int secs)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				secs
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 597));
		}

		public void SpriteQueryOpenGridTickCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 598));
		}

		public void SpriteQueryOpenPortableGridTickCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 599));
		}

		public void SpriteStartMeditateCmd(int meditateState)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				meditateState
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 600));
		}

		public void SpriteZhanMengBuildLevelUpCmd(int bhid, int buidlType, int toLevel)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				buidlType,
				toLevel
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 601));
		}

		public void SpriteZhanMengBuildGetBufferCmd(int bhid, int buildType, int toLevel)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid,
				buildType,
				toLevel
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 602));
		}

		public void SpriteGetBaiTanLogCmd(int idx)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				idx
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 603));
		}

		public void SpriteSendPushMsgIDToServerCmd(string idx)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				idx
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 604));
		}

		public void SpriteActivationPictureJudgeCmd(int idx)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				idx
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 605));
		}

		public void SpriteGetPictureJudgeInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 606));
		}

		public void SpriteReferPictureJudgeCmd(string strIDs)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("1000", Global.GetLang("一键提交图鉴次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				strIDs
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 612));
		}

		public void SpriteMuEquipUpgradeCmd(int nGoodsDBID1, int nGoodsDBID2, int nGoodsDBID3, int FuMoId = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("105", Global.GetLang("装备进阶次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				nGoodsDBID1,
				nGoodsDBID2,
				nGoodsDBID3,
				FuMoId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 607));
		}

		public void SpriteWingoffonCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 610));
		}

		public void SpriteWingUpgradeCmd(int upgradeType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				upgradeType
			});
			if (upgradeType == 0)
			{
				Global.SendEvent("402", Global.GetLang("翅膀道具进阶次数"));
			}
			else if (upgradeType == 1)
			{
				Global.SendEvent("403", Global.GetLang("翅膀自动进阶次数"));
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 609));
		}

		public void SpriteWingUpStarCmd(int upStarType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				upStarType
			});
			if (upStarType == 0)
			{
				Global.SendEvent("400", Global.GetLang("翅膀道具升星次数"));
			}
			else if (upStarType == 1)
			{
				Global.SendEvent("401", Global.GetLang("翅膀自动升星次数"));
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 608));
		}

		public void SpriteGetBindDiamondExchangeInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 613));
		}

		public void SpriteEquipAppendInheritCmd(int leftGoodsDbID, int rightGoodsDbID, int rockGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("103", Global.GetLang("追加传承次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				leftGoodsDbID,
				rightGoodsDbID,
				rockGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 615));
		}

		public void SpriteQureyWanmotaInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 618));
		}

		public void SpriteSweepWanmota(int startIndex = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				startIndex
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 616));
		}

		public void SpriteGetSweepReward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 619));
		}

		public void SpriteGetCopyTeamDamageInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 626));
		}

		public void QueryPayActiveInfo(int nRoleID, int nActiveID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				nRoleID,
				nActiveID
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 635));
		}

		public void GetChongZhiJiangLi(int nRoleID, int nActiveID, int nIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				nRoleID,
				nActiveID,
				nIndex
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 636));
		}

		public void GetAllRepayActivityInfo(int nRoleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				nRoleID
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 637));
		}

		public void DestroyGoods(int nDBID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				nDBID
			});
		}

		public void QueryActivitySomeInfo(int nTpye)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				nTpye
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 638));
		}

		public void SendEndBossAnimation(int monsterID, int toMapCode, int toX, int toY, int effectX, int effectY, long ticks, int checkCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				Global.Data.roleData.RoleID,
				monsterID,
				toMapCode,
				toX,
				toY,
				effectX,
				effectY,
				ticks,
				checkCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 640));
		}

		public void GetDailyPrivilegeInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1030));
		}

		public void SignDailyPrivilege(int signAll, int signID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				signAll,
				signID
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1031));
		}

		public void QueryTodayCandoInfo(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				type
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 641));
		}

		public void QueryGetOldResourceInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 642));
		}

		public void GetOldResource(int activeType, int GoldOrZuanShi, int getModel)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				Global.Data.roleData.RoleID,
				activeType,
				GoldOrZuanShi,
				getModel
			});
			bool flag = this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 643));
		}

		public void SpriteEquipWashPropCmd(int dbID, int washIndex, int isFirstToUseBind, int moneyType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("106", Global.GetLang("装备洗炼次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID,
				washIndex,
				isFirstToUseBind,
				moneyType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 645));
		}

		public void SpriteEquipWashInheritCmd(int leftGoodsDbID, int rightGoodsDbID, int moneyType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Global.SendEvent("104", Global.GetLang("洗炼传承次数"));
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				leftGoodsDbID,
				rightGoodsDbID,
				moneyType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 646));
		}

		public void SpriteQueryStoryCopyMapInfoCmd(int nMapID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nMapID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 648));
		}

		public void SpriteQueryImpetrateInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 650));
		}

		public void SpriteExecuteImpetrateCmd(int nParameter)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nParameter
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 651));
		}

		public void SpriteQueryJingLingZhaoHuanInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 751));
		}

		public void SpriteExecuteJingLingZhaoHuanCmd(int nParameter)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nParameter
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 752));
		}

		public void SpriteQueryStarConstelltionInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 660));
		}

		public void SpriteActivationStarConstelltionInfoCmd(int nStarSite)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nStarSite
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 661));
		}

		public void SpriteChangeAngleCmd(int dir, int yAngle)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				dir,
				yAngle
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 662));
		}

		public void GetShareStat()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 665));
		}

		public void UpdateShareStat()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 663));
		}

		public void GetShareReward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 664));
		}

		public void SpriteQueryLianzhiNumCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 669));
		}

		public void SpriteExeLianzhiCmd(int type, int count)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				count
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 668));
		}

		public void GetFirstChargeState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 671));
		}

		public void SpriteShowGonggaoPage()
		{
			if (!Global.Data.PlayGame || BasePlayZone.alreadyRequestedNotice)
			{
				return;
			}
			BasePlayZone.alreadyRequestedNotice = true;
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 677));
		}

		public void SpriteQueryWingData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 678));
		}

		public void SpriteQueryCaijiLastNum(int type = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 682));
		}

		public void SpriteStartCaiji(int monsterID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				monsterID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 680));
		}

		public void SpriteFinishCaiji(int monsterID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				monsterID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 681));
		}

		public void SendToServerDeviceType()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string androidMobileType = QMQJJava.GetAndroidMobileType();
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				androidMobileType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 686));
		}

		public void GetFamilyBossInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 711));
		}

		public void GetFamilyBossRewardCmd(int copyID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				copyID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 712));
		}

		public void SpritQueryYuansuBag(int site)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				site
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 720));
		}

		public void SpritQueryYuansuBagByUsed(int site)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				site
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 726));
		}

		public void SpriteStartTilian(int count, int isDiamond)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				count,
				isDiamond
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 723));
		}

		public void SpriteModYuansuEquip(int dbID, int state)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID,
				state
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 722));
		}

		public void SpriteStartQianghua(int dbid, int site, string dbidList)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				dbid,
				site,
				dbidList
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 724));
		}

		public void SpriteResetYuansuBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 725));
		}

		public void SpriteQueryGetYuanInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 721));
		}

		public void SpriteQueryOpenYuanSuBox(int boxsite)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				boxsite
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 727));
		}

		public void ApplyYanHui(int Type, int onlyCheck)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				Type,
				onlyCheck
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 730));
		}

		public void JoinYanHui()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 732));
		}

		public void GetYanHuiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 731));
		}

		public void HaveYanHuiNPC()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 733));
		}

		public void GetBeiZhanPetList()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				5000
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 740));
		}

		public void GetPaiZhuPetList()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				10000
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1850));
		}

		public void ChengZhanJingJia(int bidSite, int bidMoney)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				bidSite,
				bidMoney
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 700));
		}

		public void GetChengZhanDailyAwards()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 701));
		}

		public void LuoLanChengZhan(int op)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				op
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 702));
		}

		public void GetLuoLanChengZhuInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 706));
		}

		public void GetBangHuiLingDiItemData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 708));
		}

		public void GetLuolanFazhenBossCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 760));
		}

		public void GetLuolanFazhenDoorCmd(int mapCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 685));
		}

		public void GetZhanMengziJin()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 709));
		}

		public void UploadLuoLanWing(int tabId, int wingId, int toUpload)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				tabId,
				wingId,
				toUpload
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 710));
		}

		public void StoreSaveMoney(int Num)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 761));
		}

		public void StoreSaveBindMoney(int Num)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 762));
		}

		public void StartZhuLing()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 810));
		}

		public void StartZhuHun()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 811));
		}

		public void FuWenChengJiuInfo()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 780));
		}

		public void FuWenChengJiuTiSheng(int Num)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 781));
		}

		public void TianFuGetInfo()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1000));
		}

		public void OtherObjectFaceID(int id)
		{
			this.otherObjectFaceID = id;
			this.isOtherFaceId = true;
		}

		public void GetOtherTianFuInfoFromServer()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.otherObjectFaceID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 999));
		}

		public void TianFuZhuRuJingYan()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1001));
		}

		public void TianFuJiaDian(int Num, int Point)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				Num,
				Point
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1003));
		}

		public void TianFuXiDian(int Num)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1002));
		}

		public void GetLingyuList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 800));
		}

		public void GetShengjiLingyuCmd(int type, int needZuanshi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				needZuanshi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 801));
		}

		public void GetShengjieLingyuCmd(int type, int needZuanshi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				needZuanshi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 802));
		}

		public void SpriteShenqiZaizao(int dbID, int isFirstToUseBind = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID,
				isFirstToUseBind
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 791));
		}

		public void SpriteRebornJinJie(int dbID, int isFirstToUseBind = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				dbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2053));
		}

		public void GetShengYuChengZhuRoleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1160));
		}

		public void SendGetJunTuanRoleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1835));
		}

		public void SendJunTuanMoBaiInfo(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1836));
		}

		public void GetShengYuChengZhuRoleListInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1161));
		}

		public void ShengYuChengZhuAdrationCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (nType == 1)
			{
				Global.SendEvent("1100", Global.GetLang("金币膜拜次数"));
			}
			if (nType == 2)
			{
				Global.SendEvent("1101", Global.GetLang("钻石膜拜次数"));
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1162));
		}

		public void GetZhongShenChengZhuRoleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1361));
		}

		public void ZhongShenChengZhuAdrationCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (nType == 1)
			{
				Global.SendEvent("1100", Global.GetLang("金币膜拜次数"));
			}
			if (nType == 2)
			{
				Global.SendEvent("1101", Global.GetLang("钻石膜拜次数"));
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1362));
		}

		public void GetLuoLanChengZhuRoleInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 841));
		}

		public void LuoLanChengZhuAdrationCmd(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (nType == 1)
			{
				Global.SendEvent("1100", Global.GetLang("金币膜拜次数"));
			}
			if (nType == 2)
			{
				Global.SendEvent("1101", Global.GetLang("钻石膜拜次数"));
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 842));
		}

		public void SpriteLogOut()
		{
			if (this.tcpClient.Connected)
			{
				string text = string.Empty;
				text = StringUtil.substitute("{0}", new object[]
				{
					0
				});
				this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 22));
			}
		}

		public void RegionEvent(int type, int flag)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				flag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 830));
		}

		public void JoinHuanYingGroup()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 820));
		}

		public void QuitHuanYingGroup()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 821));
		}

		public void EnterHuanYing(int nType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				nType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 824));
		}

		public void GetWaitNum()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 822));
		}

		public void GetShiLiZhengBaScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 827));
		}

		public void GetHuanYingTime()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 827));
		}

		public void GetHuanYingScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 826));
		}

		public void GetAdendaInfo()
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 782));
		}

		public void UpAdenda(int num)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 783));
		}

		public void GetHuanYingWinNum()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 828));
		}

		public void SetSecondPassword(byte[] data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, data, 0, data.Length, 861));
		}

		public void VerifySecondPassword(byte[] data)
		{
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, data, 0, data.Length, 862));
		}

		public void CancelSecondPassword(byte[] data)
		{
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, data, 0, data.Length, 863));
		}

		public void GetMyReferee(string userID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void SubmitMyReferee(string userID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void GetRecallGiftSet(string userID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void PickupRecallGiftSet(string userID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void GetRecallSignInfo(string userID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void SignInRecallByID(string userID, int signID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
		}

		public void GetReturnData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 900));
		}

		public void GetReturnCheck(string returnID = "0")
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				returnID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 901));
		}

		public void GetReturnAward(int eReturnAwardType, int id, int num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				eReturnAwardType,
				id,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 902));
		}

		public void GetJieriZengsongInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 920));
		}

		public void GetJieriShouliInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 944));
		}

		public void GetJieriShouliRewardCmd(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 945));
		}

		public void GetJieriChongzhiInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 940));
		}

		public void GetJieriChongzhiRewardCmd(int awardId, int day)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				awardId,
				day
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 941));
		}

		public void GetJieriPingTaiKingCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1300));
		}

		public void SendJieriZengsongCmd(string roleName, int goodID, int goodcnt)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				roleName,
				goodID,
				goodcnt
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 921));
		}

		public void GetJieriZengsongRewardCmd(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 922));
		}

		public void GetJieriZengsongKingInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 923));
		}

		public void GetJieriShouliKingInfoCmd()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 925));
		}

		public void GetJieriZengsongKingRewardCmd(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 924));
		}

		public void GetJieriShouliKingRewardCmd(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 926));
		}

		public void GetJieriTongyongInfoCmd(int activityType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 927));
		}

		public void SetEffectHideFlagsCmd(params int[] flags)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<int[]>(flags);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 970));
		}

		public void SpriteChangeName(int roleid, int zoneid, string newname)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				roleid,
				zoneid,
				newname
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 14001));
		}

		public void GetShengWuInfo(sbyte sbyte1, sbyte m_sbyte)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				sbyte1,
				m_sbyte
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 10206));
		}

		public void GetGuardPoint()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 930));
		}

		public void RecyclingGuardPoint(string items)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				items
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 931));
		}

		public void GetGuardStatueInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 932));
		}

		public void UpLevelGuardStatue()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 933));
		}

		public void UpGradeGuardStatue()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 934));
		}

		public void EquipGuardSoul(int positionID, int soulType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				positionID,
				soulType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 935));
		}

		public void GetDressList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 946));
		}

		public void GetWeekendAwardInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1501));
		}

		public void InlayDiamond(int slotID, int shapeID, int bagIndex)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bagIndex,
				slotID,
				shapeID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 992));
		}

		public void UnloadDiamond(int slotID, int operationType, int shapeID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			if (operationType == 0)
			{
				text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
				{
					this.CurrentSession.RoleID,
					slotID,
					operationType,
					shapeID
				});
			}
			else if (operationType == 1)
			{
				text = StringUtil.substitute("{0}:{1}:{2}", new object[]
				{
					this.CurrentSession.RoleID,
					slotID,
					operationType
				});
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 993));
		}

		public void UplevelDiamond(int levelupType, int index, int shapeID, Dictionary<int, int> dic_availableDiamond)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			FluorescentGemUpTransferData fluorescentGemUpTransferData = new FluorescentGemUpTransferData(this.CurrentSession.RoleID, levelupType, (levelupType != 0) ? 0 : index, (levelupType != 1) ? 0 : index, (levelupType != 1) ? 0 : shapeID, dic_availableDiamond);
			byte[] array = DataHelper.ObjectToBytes<FluorescentGemUpTransferData>(fluorescentGemUpTransferData);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 994));
		}

		public void DigDiamond(int digType, int digTimes)
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				digType,
				digTimes
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 995));
		}

		public void PulverizeDiamond(int bagIndex, int num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bagIndex,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 996));
		}

		public void SortDiamondBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 991));
		}

		public void EnterKuaFuMap(int mapCode, int line, int bossID, int teleportID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.HideNetWaiting();
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					Global.IsFuBenFenBaoMap = false;
					if (Global.IsPopupDownloadMapWindow(mapCode))
					{
						Global.IsFuBenFenBaoMap = true;
						string[] buttons = new string[]
						{
							Global.GetLang("立即下载"),
							Global.GetLang("稍后下载")
						};
						string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(mapCode));
						Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
						{
							if (e1.ID == 0)
							{
								string text2 = string.Empty;
								text2 = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
								{
									mapCode,
									line,
									bossID,
									teleportID
								});
								MUDebug.Log<string>(new string[]
								{
									string.Concat(new object[]
									{
										"mapCode = ",
										mapCode,
										"line = ",
										line,
										"bossID = ",
										bossID
									})
								});
								this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 1141));
							}
							else
							{
								Global.IsFuBenFenBaoMap = false;
							}
						}, buttons);
					}
					else
					{
						string text = string.Empty;
						text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
						{
							mapCode,
							line,
							bossID,
							teleportID
						});
						MUDebug.Log<string>(new string[]
						{
							string.Concat(new object[]
							{
								"mapCode = ",
								mapCode,
								"line = ",
								line
							})
						});
						this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1141));
					}
				}
			}, -1);
		}

		public void GetMapInfo(int mapCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				mapCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1140));
		}

		public void SendAppNamelist(byte[] data)
		{
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, data, 0, data.Length, 29900));
		}

		public void DemandQiRiKuangHuanInfo(int actType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				actType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1310));
		}

		public void GetActivityAward(int activityType, int extTag = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				activityType,
				extTag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1311));
		}

		public void SendBuyInfo(int id, int buyCount = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				buyCount
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1312));
		}

		public void BuildGetLingDiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1550));
		}

		public void BuildSendZhiXing(int buildID, int buildTaskID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				buildID,
				buildTaskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1551));
		}

		public void SendQuickFinishTask(int buildID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				buildID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1552));
		}

		public void SendRenovateTask(int buildID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				buildID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1553));
		}

		public void SendGetLevelAward(int levelawardid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				levelawardid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1554));
		}

		public void SendGetBuildAward(int buildID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				buildID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1555));
		}

		public void SendOpenQueue()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1556));
		}

		public void SendGetTeamState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1557));
		}

		public void SendGetBuildState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1558));
		}

		public void SendGetBuildLevelAwardState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1559));
		}

		public void GetTuiGuangInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1022));
		}

		public void SendBeTuiGuangYuan()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1017));
		}

		public void SendTuiGuangGetAward(int awardType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				awardType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1018));
		}

		public void SendTuiGuangTuiJianRenID(string id)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				id
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1019));
		}

		public void SendTuiGuangGetCode(string phoneID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				phoneID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1020));
		}

		public void SendTuiGuangCheckCode(string yanzhengID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				yanzhengID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1021));
		}

		public void GetLangHunYaoSaiScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1025));
		}

		public void GetLangHunYaoSaiAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1026));
		}

		public void WolfSoulFieldJoin()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1153));
		}

		public void ApplyPlayerData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1154));
		}

		public void ApplyCityData(int cityID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				cityID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1155));
		}

		public void ApplyPlayerWorldData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1156));
		}

		public void EnterCity(int cityID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				cityID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1157));
		}

		public void GetDayAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1158));
		}

		public void GetOnePieceInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 1600));
		}

		public void SendOnePieceROLL()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 1601));
		}

		public void SendOnePieceROLL_MIRACLE(int Num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}", new object[]
			{
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1605));
		}

		public void SendOnePieceBuyDICE(DiceType type, int num = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = StringUtil.substitute("{0}:{1}", new object[]
			{
				(int)type,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1606));
		}

		public void SendOnePieceEVENT()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 1603));
		}

		public void SendOnePieceMove()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 1604));
		}

		public void SendOnePieceTiggerEvent()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string empty = string.Empty;
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, empty, 1602));
		}

		public void GetSoulCometStoneGroupInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1320));
		}

		public void GetSoulCometStone(int times, string extraFunc)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				times,
				extraFunc
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1321));
		}

		public void UplevelSoulCometStone(int dbid, int site, string swallowDBID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				dbid,
				site,
				swallowDBID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1322));
		}

		public void EquipSoulCometStone(int slotID, int dbid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				slotID,
				dbid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1323));
		}

		public void SortSoulCometStoneBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1324));
		}

		public void GetFundInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1032));
		}

		public void BuyFund(int fundType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				fundType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1033));
		}

		public void GetFundAward(int fundType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				fundType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1034));
		}

		public void HideGongNeng(int bitFlag, int open)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				bitFlag,
				open
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1330));
		}

		public void GetJingJiChangRoleLooks(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1340));
		}

		public void GetPKKingRoleLooks(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1341));
		}

		public void GetLuoLanKingRoleLooks(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1342));
		}

		public void GetMainInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1350));
		}

		public void GetAllPKLog()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1351));
		}

		public void GetAllPKState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1352));
		}

		public void GetPKState(int UnionGroup)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				UnionGroup
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1353));
		}

		public void GetZhengBaSupport(int UnionGroup, int Group, int supportType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				UnionGroup,
				Group,
				supportType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1354));
		}

		public void GetZhengBaEnter(int gameID, int enter)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				gameID,
				enter
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1357));
		}

		public void GetDaoJiShiTime()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1359));
		}

		public void GetArmyCaiJiDaoJiShiTime()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1826));
		}

		public void SendOpenArmyCaiJiWinodw()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1827));
		}

		public void SendEnterArmyCaiJiScene(int caiJitype)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				caiJitype
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1829));
		}

		public void GetDoubleCaiJiTime()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1831));
		}

		public void SendOpenDoubleCaiJiRequest()
		{
		}

		public void SendGetShouWeiRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1833));
		}

		public void SendSetShouWeiRequest(int index, int useZuanShi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				index,
				useZuanShi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1834));
		}

		public void SendFaHongBaoRequest(FaHongBaoData _data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (_data == null)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			byte[] array = DataHelper.ObjectToBytes<FaHongBaoData>(new FaHongBaoData
			{
				roleID = this.CurrentSession.RoleID,
				type = _data.type,
				count = _data.count,
				diamondNum = _data.diamondNum,
				message = _data.message
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1420));
		}

		public void SendHongBaoRankRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			HongBaoRankData hongBaoRankData = null;
			long num;
			if (Global.Data.HongBaoRankDict.TryGetValue(type, ref hongBaoRankData))
			{
				num = hongBaoRankData.flag;
			}
			else
			{
				num = 0L;
			}
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1421));
		}

		public void SendMyHongBaoRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			MyHongBaoData myHongBaoData = null;
			long num;
			if (Global.Data.HongBaoFlagDict.TryGetValue(type, ref myHongBaoData))
			{
				num = myHongBaoData.flag;
			}
			else
			{
				num = 0L;
			}
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1422));
		}

		public void SendShowHongBaoRequest(int hongBaoType, int hongBaoID, int tipFlag = 0)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				hongBaoType,
				hongBaoID,
				tipFlag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1423));
		}

		public void SendHongBaoDetailsRequest(int hongBaoType, int hongBaoID, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				hongBaoType,
				hongBaoID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1424));
		}

		public void SendHongBaoPaiHang()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.hongBaoPaiHang_Version
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1429));
		}

		public void LingQuHongBaoPaiHang(int ID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1428));
		}

		public void SendYaoSaiMainBossMainInfoRequest(int otherID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1851));
		}

		public void SendUpdateYaoSaiDiTuBossModelInfoRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1856));
		}

		public void SendZhaoHuanBossRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1852));
		}

		public void SendTaoFaBossRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1853));
		}

		public void SendFightBossInfoRequest(int otherID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1854));
		}

		public void SendFightBossResultRequest(int otherID, string jingLingList)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherID,
				jingLingList
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1855));
		}

		public void SendFightBossLogRequest(int otherID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1857));
		}

		public void SendFightBossAwardRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1858));
		}

		public void GetZhuanXiangData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Global.ZhuanxiangXML_Version
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1510));
		}

		public void GetCurrentZhuanXiang()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1511));
		}

		public void GetZhuanXiangReward(int ActID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ActID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1512));
		}

		public void GetPetSkill(int srcPetID, int tarPetID, int userMoney)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				srcPetID,
				tarPetID,
				userMoney
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1065));
		}

		public void GetEveryDayData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Global.everyDayXML_Version
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1505));
		}

		public void GetCurrentEveryDay()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1506));
		}

		public void GetEveryDayReward(int ActID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ActID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1507));
		}

		public void GetRidePetMainData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1896));
		}

		public void GetRidePetChouQu(int type, int cost)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				cost
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1897));
		}

		public void SendRoleRidePetUp(int DbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				DbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1898));
		}

		public void SendRoleRidePetUp()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1902));
		}

		public void SendRoleRideCangKuZhengLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1903));
		}

		public void SendGetRoleRideTuJian()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1900));
		}

		public void GetYiYuanGouData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1621));
		}

		public void GetXingYunYiYuanData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				46
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 635));
		}

		public void LingJiangXingYunYiYuan()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				46,
				1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 636));
		}

		public void SendGetJingLingSkillGraspNeedLingJing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1039));
		}

		public void SendJingLingSkillSlotUp(int dbId, int SkillSlotId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				dbId,
				SkillSlotId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1037));
		}

		public void SendJingLingSkillGrasp(int dbId, int SkillSlotLockId0, int SkillSlotLockId1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				dbId,
				SkillSlotLockId0,
				SkillSlotLockId1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1038));
		}

		public void SendGetIDBindingInf(string activateType, string activateInfo, string error)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.UserID,
				activateType,
				activateInfo,
				error
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1040));
		}

		public void SendGetIDBindingAward(string activateType, string activateInfo, string error)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.CurrentSession.RoleID,
				this.CurrentSession.UserID,
				activateType,
				activateInfo,
				error
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1041));
		}

		public void SendShenDianInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1035));
		}

		public void SendShenDianTiShengInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1036));
		}

		public void SendIsCanYu()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1360));
		}

		public void SendVideoOpenInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1700));
		}

		public void GetGetTaLuopaiData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1704));
		}

		public void SendTOUseTaLuopaiSuiPian(int DBID, int GoodsID, int num = 1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				DBID,
				GoodsID,
				num
			});
			MUDebug.Log<string>(new string[]
			{
				"角色使用塔罗牌道具 :: " + text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1705));
		}

		public void SendTaLuopai(int goodID, int count)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				goodID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1701));
		}

		public void SendUsingKingTeQuan()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1703));
		}

		public void SendTaLuopaiChangePOS(int GoodsID, int Pos)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				GoodsID,
				Pos
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1702));
		}

		public void SendMengHuanStoneData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2188));
		}

		public void SendMengHuanStoneChouJiangOne(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2189));
		}

		public void SendMengHuanStoneLogs()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2191));
		}

		public void SetDataVIPLiBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1515));
		}

		public void LingJiangVIPLiBao(int ID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.RoleID,
				ID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1516));
		}

		public void SetMingLing(string strcmd, int value)
		{
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, strcmd, value));
		}

		public void SetDataDanBiChongZhi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.RoleID,
				69
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 947));
		}

		public void LingJiangDanBiChongZhi(int ID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				Global.Data.RoleID,
				69,
				ID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 948));
		}

		public void GetGetTeShuChengHaoData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1840));
		}

		public void GetGetTeShuChengHaoPeiDai(int buffID, int mode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				buffID,
				mode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1841));
		}

		public void RequestHuoDongFanLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1623));
		}

		public void SendDataHuoDongFanLi()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1622));
		}

		public void SendFashionUp(int id)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				id
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1610));
		}

		public void SendActivateFashion(int DBid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				DBid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1611));
		}

		public void GetMainDataForPKLovers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1370));
		}

		public void GetStateWatcherForPKLovers(int watch)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				watch
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1380));
		}

		public void GetZhanBaoInfoForPKLovers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1371));
		}

		public void GetPaiHangInfoForPKLovers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1372));
		}

		public void SetReadyStateForPKLovers(int ready)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ready
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1373));
		}

		public void SendSingleJionInfoForPKLovers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1374));
		}

		public void SendQuitJionInfoForPKLovers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1375));
		}

		public void SendEnterGameInfoForPKLovers(int gameID, int enter)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				gameID,
				enter
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1377));
		}

		public void SendZhanMengWaiJiaoMengYouRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1046));
		}

		public void SendZhanMengWaiJiaoLogRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1047));
		}

		public void SendFaQiJieMengRequest(int zoneId, string zhanMengName)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				zoneId,
				zhanMengName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1042));
		}

		public void SendCancelJieMengRequest(int zhanMengId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				zhanMengId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1043));
		}

		public void SendJieChuJieMengRequest(int zhanMengId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				zhanMengId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1044));
		}

		public void SendAgreeOrRejectRequest(int zhanMengId, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				zhanMengId,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1045));
		}

		public void GetMainDataForCoupleWish()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1390));
		}

		public void GetWishRecordForCoupleWish()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1391));
		}

		public void WishOtherRoleForCoupleWish(CoupleWishWishReqData data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<CoupleWishWishReqData>(data);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1392));
		}

		public void GetAdmireDataForCoupleWish()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1394));
		}

		public void GetAdmireStateForCoupleWish(int id, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1395));
		}

		public void GetPartyDataForCoupleWish()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1396));
		}

		public void JoinPartyForCoupleWish(int DbCoupleId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				DbCoupleId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1397));
		}

		public void SendGetRoleAllRib()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 30100));
		}

		public void GetJieriFanbeiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:0", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 771));
			Super.ShowNetWaiting(null);
		}

		public void SendOlympicsScoreRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1050));
		}

		public void SendBeginMatchRequest(int gameType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				gameType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1059));
		}

		public void SendOlympicsMatchTimesRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1051));
		}

		public void SendOlympicsSingleMatchRequest(int gameType, int gameValue)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				gameType,
				gameValue
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1052));
		}

		public void SendOlympicsGuessAnswerRequest(string answer, int dayId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				answer,
				dayId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1054));
		}

		public void SendOlympicsGuessResultRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1055));
		}

		public void SendOlympicsRankResultRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1056));
		}

		public void SendOlympicsShopContentRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1057));
		}

		public void SendOlympicsBuyGoodsRequest(int id, int count)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				id,
				count
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1058));
		}

		public void SendOlympicsQueryAwardRequest(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1060));
		}

		public void SendOlympicsGetAwardRequest(int awardType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				awardType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1061));
		}

		public void SendGetSuperDirectBuyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1620));
		}

		public void SendCompeteCityMainActivityStateData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1070));
		}

		public void SendCompeteCityAcitvityApplyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1071));
		}

		public void SendCompeteCityEnterGameData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1073));
		}

		public void SendCompeteCityHaiXuanRankData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1074));
		}

		public void SendGetShiPinData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1615));
		}

		public void SendShiPinForge(int index)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				index
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1616));
		}

		public void SendActiveShiPin(int GoodId, int dbid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				GoodId,
				dbid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1617));
		}

		public void SendShiPinGoodsList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1618));
		}

		public void SendAoYunMainData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20200));
		}

		public void SendAoYunGetQuestionData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20201));
		}

		public void SendAoYunUseGoodsData(int goodsType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20204));
		}

		public void SendAoYunAnswerQuestionData(int answerID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				answerID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20202));
		}

		public void SendAoYunGetAwardData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20203));
		}

		public void SendAoYunBuyGoodsData(int goodsType, int Num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsType,
				Num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 20205));
		}

		public void SendGuanZhanData(int mapId, int toX, int toY)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<SpriteMoveData>(new SpriteMoveData
			{
				roleID = this.CurrentSession.RoleID,
				mapCode = mapId,
				toX = toX,
				toY = toY
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1401));
		}

		public void SendPurchaseData(int zhiye)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				zhiye
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1410));
		}

		public void SendChangeData(int zhiye)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				zhiye
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1411));
		}

		public void SendCreateRoleInGame(int sex, int occup, string nameAndPingTaiID, int zoneID, int LiOrZhi)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				this.CurrentSession.UserID,
				this.CurrentSession.UserName,
				sex,
				occup,
				nameAndPingTaiID,
				zoneID,
				LiOrZhi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1412));
		}

		public void SendGetShenQiDataRequest()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1816));
		}

		public void SendShenQiZhuRuRequest(int useDaimond)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				useDaimond
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1817));
		}

		public void UploadUserInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			UserInfoSdkData userInfoSdkData = IOSSDKPlugin.UserInfoParse();
			byte[] srcData = DataHelper.ObjectToBytes<UserInfoSdkData>(userInfoSdkData);
			byte[] array = this.AESEncrypt(srcData);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 12100));
		}

		public byte[] AESEncrypt(byte[] srcData)
		{
			return AesHelper.AesEncryptBytes(srcData, "2540BFE632846", "663AC625BC");
		}

		public byte[] RSAEncrypt(byte[] srcData)
		{
			string text = "065BE0E5D99739107B552AC2200E20B2D3FA8212D51803C774CD31E2985065CB3E8388EE608EED8D272B44A48B33E0A4EE4E4F6557C98D9E0EF111CAAABDFEF8DDC89A1872979304B93640E813704FE14DC502D78CB48CA48A73DB3EE5D3EEE3FAB4DF7C82F1BE267CE4DB4C89A034009F3FD80F2D96C0541E9F29B0F3C5437BBDD355992A6B905F71CB401493F0C9406C09D81E48C2493EE0F3C1A2CE68295C03A674FFBAFFFC0EB56A102588CA858D1DAB915CE1B7FC3D6D2CC279CD40CB31CEB66DBE29CBA1110DC1ADE847D58712DFAA9B80003E5044A4B29EA3E473EF50C5A68985E6BA9A33814A3F20CB7AB6F792B72A2C55FE2D3091C7F94C56C15373";
			string text2 = StringEncrypt.Decrypt(text, "2540BFE632846", "663AC625BC");
			RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
			rsacryptoServiceProvider.PersistKeyInCsp = false;
			rsacryptoServiceProvider.FromXmlString(text2);
			int num = 0;
			int num2 = 100;
			int num3 = srcData.Length;
			int num4 = (num3 <= num2) ? num3 : num2;
			int num5 = 1;
			List<byte> list = new List<byte>();
			do
			{
				byte[] array = new byte[num4];
				Array.Copy(srcData, num, array, 0, num4);
				list.AddRange(rsacryptoServiceProvider.Encrypt(array, false));
				num = num5 * num2;
				num4 = ((num3 - num <= num2) ? (num3 - num) : num2);
				num5++;
			}
			while (num < num3);
			return list.ToArray();
		}

		public void SendGetArmyGroupListData()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1230));
		}

		public void SendGetRoleArmyGroupData(int RoleID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1231));
		}

		public void SendGetRoleArmyGroupBHData(int ArmyGroupID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				ArmyGroupID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1232));
		}

		public void SendJionGroup(int ArmyGroupID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				ArmyGroupID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1233));
		}

		public void SendGetArmyGroupJionList()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1235));
		}

		public void SendGetArmyGroupJionEventLogList()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1236));
		}

		public void SendSetArmyGroupJionBullEtin(string Content)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				Content
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1237));
		}

		public void SendChangeArmyGroupZhiWu(int BHID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				BHID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1238));
		}

		public void SendChangeArmyGroupRoleZhiWu(List<int> data)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<List<int>>(data);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1239));
		}

		public void SendGetArmyGroupTaskList()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1221));
		}

		public void SendGetArmyGroupTaskAward(int TaskID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				TaskID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1223));
		}

		public void SendGetArmyGroupPointPaiHang()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1220));
		}

		public void SendCreatArmyGroup(string NameText)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				NameText
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1224));
		}

		public void SendArmyGroupChange()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1225));
		}

		public void SendGetArmyGroupJingYingDataLst()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1226));
		}

		public void SendQUITArmyGroup(int BHID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.UserID,
				BHID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1227));
		}

		public void SendDestoryArmyGroup()
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.UserID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1228));
		}

		public void SendArmyGroupRespondShengQing(int ret, int BHID)
		{
			if (this.ActiveDisconnect)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.UserID,
				BHID,
				ret
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1234));
		}

		public void SendSetRealTimePriority(int type, string idList)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			MUVoiceManager.GetInstance().IsGetPriorityList = false;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				idList
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1111));
		}

		public void SendGetRealTimePriority(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1112));
		}

		public void SendGetRealTimePriorityInScene()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1110));
		}

		public void EnterTerritoryFightGame(int mapCode)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				mapCode
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1210));
		}

		public void JionFightingState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1212));
		}

		public void GetXingyunChoujiangData()
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1810));
		}

		public void SendXingyunChoujiangInfo(int choujiangType)
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				choujiangType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1811));
		}

		public void GetMeiriLeichongInfo(int dayNum)
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				dayNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1806));
		}

		public void RequestMeiRiChongZhiKingInfo()
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1808));
		}

		public void GetXingyunChoujiangAward()
		{
			Super.ShowNetWaiting(null);
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1813));
		}

		public void ShenJiShengJi(int shenjiID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				shenjiID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1080));
		}

		public void ShenJiZhuRu()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1081));
		}

		public void ShenJiDianChongZhi(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1082));
		}

		public void SendGetJingLingYaiSaiData(int OtherID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				OtherID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1859));
		}

		public void SendRefreshJingLingYaoSaiTask()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1860));
		}

		public void SendJingLingYaiSaiZhiXingtask(int Sate, string Pets)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				Sate,
				Pets
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1861));
		}

		public void SendJingLingYaiSaitaskQuit(int Sate)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Sate
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1862));
		}

		public void SendJingLingYaiSaiToFight(int Sate)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Sate
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1863));
		}

		public void SendPrisonMainData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1520));
		}

		public void SendPrisonHuDongJiLuData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1521));
		}

		public void SendPrisonRevoltData(int isXiaoHaoZuanShi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				isXiaoHaoZuanShi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1522));
		}

		public void SendPrisonHuDongData(int roleID, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				roleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1523));
		}

		public void SendPrisonFreeData(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1524));
		}

		public void SendPrisonFireData(int roleId, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				roleId,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1525));
		}

		public void SendPrisonBuyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1527));
		}

		public void SendYaoSaiData(int roleID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				roleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1528));
		}

		public void SendDoEmblem()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1445));
		}

		public void SendDoBianShen()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1448));
		}

		public void SenEmblemUpLevel(HuiJiUpdateResultData data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				data
			});
			byte[] array = data.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1446));
		}

		public void SendArmorUpLevel(ArmorUpdateResultData data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"<color=yellow>  Armor = ",
					data.Armor,
					" Type = ",
					data.Type,
					"  ZuanShi  = ",
					data.ZuanShi,
					"</color>"
				})
			});
			byte[] array = data.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1447));
		}

		public void SendBianShenUpLevel(BianShenUpdateResultData data)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				data
			});
			byte[] array = DataHelper.ObjectToBytes<BianShenUpdateResultData>(data);
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1449));
		}

		public void SenYuanSuJueXingJiHuo(YuanSuJueXingType type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				(int)type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1450));
		}

		public void SenYuanSuJueXingUpLevel(YuanSuJueXingType JueXingtype, YuanSuJueXingQiangHuaType QiangHuaType, bool shenYou)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			if (shenYou)
			{
				text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
				{
					this.CurrentSession.RoleID,
					(int)JueXingtype,
					(int)QiangHuaType,
					1
				});
			}
			else
			{
				text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
				{
					this.CurrentSession.RoleID,
					(int)JueXingtype,
					(int)QiangHuaType,
					0
				});
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1451));
		}

		public void SenRetureXml()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 903));
		}

		public void SenEraData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1090));
		}

		public void SenEraJuanXian(int stage)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				stage
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1091));
		}

		public void SenEraLingQu(int awardid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				awardid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1092));
		}

		public void SenUserFuMoRoleList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2027));
		}

		public void SenUserFuMoGive(int otherRoleID, string name)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				otherRoleID,
				name
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2022));
		}

		public void SenFuMoMailList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2023));
		}

		public void SenFuMoIsRead(int mailId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				mailId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2029));
		}

		public void SenFuMoLingQu(int type, int mailID = -1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			if (type == 1)
			{
				text = StringUtil.substitute("{0}:{1}", new object[]
				{
					0,
					type
				});
			}
			else if (type == 0)
			{
				text = StringUtil.substitute("{0}:{1}", new object[]
				{
					mailID,
					type
				});
			}
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2021));
		}

		public void SenFuMoZhuangBei(int dbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				dbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2025));
		}

		public void SenFuMoZhuangBeiBaoCun(int dbID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				dbID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2026));
		}

		public void SenFuMoZhuangBeiChuanCheng(int dbIDLeft, int dbIDRight, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				Global.Data.roleData.RoleID,
				dbIDLeft,
				dbIDRight,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2024));
		}

		public void SenZhuTiFuXmlData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				Global.ZhuTiFuXML_Version
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 905));
		}

		public void SenZhuTiFuZhiGouData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 906));
		}

		public void ZhuTiFuSpriteQueryJieriDaLiBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 908));
		}

		public void SetZhuTiFudDuiHuanData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 907));
		}

		public void SetZhuTiFudRuQinData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 910));
		}

		public void SetZhuTiFuMoYuData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1906));
		}

		public void SetZhuTiFuMoYuNumber()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1907));
		}

		public void SendAlchemyData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1085));
		}

		public void SendAlchemyAddElement(int AlchemyCostType, int Num, int useDaoJu)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				AlchemyCostType,
				Num,
				useDaoJu
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1086));
		}

		public void SendAlchemyExcute(int AlchemyType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				AlchemyType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1087));
		}

		public void GetShenShiMainData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1870));
		}

		public void GetFuWenList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1871));
		}

		public void GetFuWenTabList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1872));
		}

		public void GetShenShiList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1873));
		}

		public void GetFuWenModEquip(int fuwenTabID, int WeiZhiID, int fuwenGoodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				fuwenTabID,
				WeiZhiID,
				fuwenGoodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1874));
		}

		public void GetShenShiModEquip(int TabID, int ShenShiID, int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				TabID,
				ShenShiID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1875));
		}

		public void GetSkillModEquip(int TabID, int skillID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				TabID,
				skillID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1876));
		}

		public void GetFuWenChouQu(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1877));
		}

		public void GetFuWenZhiZuo(int goodID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1878));
		}

		public void GetFuWenFenJie(string goodsID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goodsID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1879));
		}

		public void GetFuWenTabMod(int fuwenTabID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				fuwenTabID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1880));
		}

		public void GetFuWenChangeName(int FuWenTabID, string name)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				FuWenTabID,
				name
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1881));
		}

		public void GetFuWenTabBuy()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1883));
		}

		public void JueXingGetInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1886));
		}

		public void JueXingShiJiHuo(int taoZhuangId, int jueXingShiId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				taoZhuangId,
				jueXingShiId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1887));
		}

		public void JueXingTaoZhuangSelect(int type, int taoZhuangId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				type,
				taoZhuangId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1888));
		}

		public void JueXingMoHua()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1889));
		}

		public void JueXingHuiShou(string goods)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				goods
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1890));
		}

		public void ShiLiGetCompData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1125));
		}

		public void ShiLiJoinComp(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1126));
		}

		public void ShiLiGetRank(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1127));
		}

		public void ShiLiSetBulltin(string bulltin)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				bulltin
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1128));
		}

		public void ShiLiSetEnemy(int enemyCompType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				enemyCompType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1129));
		}

		public void ShiLiGetZhiWuInfo(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1130));
		}

		public void EnterCompMap(int toMapCode, int pox, int posy, int teleportId, int toBoss)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						string text2 = string.Empty;
						text2 = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							toMapCode,
							pox,
							posy,
							teleportId,
							toBoss
						});
						this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text2, 1132));
					}
				}, -1);
			}
			else
			{
				string text = string.Empty;
				text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					toMapCode,
					pox,
					posy,
					teleportId,
					toBoss
				});
				this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1132));
			}
		}

		public void ShiLiGetCompBattleData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2000));
		}

		public void ShiLiGetCompBattleCifyData(int cityId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				cityId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2001));
		}

		public void ShiLiEnterCompBattle(int cityId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				cityId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2002));
		}

		public void ShiLiGetCompBattleAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2003));
		}

		public void ShiLiGetCompBattleState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2004));
		}

		public void ShiLiGetCompBattleSideScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2005));
		}

		public void ShiLiGetCompBattleSelfScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2006));
		}

		public void ShiLiGetCompBattleAwardGet()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2009));
		}

		public void SendShiLiGetCompMiDongStates()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2013));
		}

		public void SendShiLiGetCompMiDongAwardInf()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2012));
		}

		public void SendShiLiMiDongGetBaseData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2010));
		}

		public void SendShiLiGetMiDongAwardGet()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2018));
		}

		public void SendShiLiMiDongGoToBattleGround(int mineID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				mineID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2011));
		}

		public void SendShiLiMiDongGetSelfScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2015));
		}

		public void SendShiLiMiDongGetSceneScoreInf()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2014));
		}

		public void GetZhanMengLianSaiMainInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1165));
		}

		public void GetZhanMengLianSaiRankInfo(BangHuiMatchRankType type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1166));
		}

		public void SendZhanMengLianSaiJionMes()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1170));
		}

		public void SendZhanMengLianSaiGetMilitaryMes()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1167));
		}

		public void SendZhanMengLianSaiCompetitionEnter(int zhanMengId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				zhanMengId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1171));
		}

		public void SendZhanMengLianSaiCompetitionEnterState(bool ShowNetWaiting = true)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			if (ShowNetWaiting)
			{
				Super.ShowNetWaiting(null);
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1172));
		}

		public void SendZhanMengLianSaiCompetitionScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1173));
		}

		public void SendZhanMengLianSaiCompetitionAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1174));
		}

		public void SendZhanMengLianSaiGetAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1175));
		}

		public void SendZhanMengLianSaiGetAwardRank()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1176));
		}

		public void SendZhanMengLianSaiGetRankOInfoMini()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1177));
		}

		public void SendZhanMengLianSaiGetMoBaiData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1168));
		}

		public void SendZhanMengLianSaiMoBai(int type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1169));
		}

		public void SendZhanMengLianSaiJingCaiData(int bhid1, int bhid2, int result)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				bhid1,
				bhid2,
				result
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1195));
		}

		public void SendZhanMengLianSaiJingCaiRankInfoData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1196));
		}

		public void SendZhanMengLianSaiJingCaiAwardData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1197));
		}

		public void SendGetKuFuPlubderGameStateData(long age = -1L)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<KuaFuLueDuoStateData>(new KuaFuLueDuoStateData
			{
				Age = age
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1252));
		}

		public void SendGetKuFuPlubderServerDataList(long ags, long ags1)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<KuaFuLueDuoMainInfo>(new KuaFuLueDuoMainInfo
			{
				ServerListAge = ags,
				StateListAge = ags1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1245));
		}

		public void SendGetKuFuPlubdeRankData(long age, byte type)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = DataHelper.ObjectToBytes<KuaFuLueDuoRankListCmdData>(new KuaFuLueDuoRankListCmdData
			{
				Age = age,
				RankType = (int)type
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 1246));
		}

		public void SendGetKuFuPlubdeJueXingShopData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1257));
		}

		public void SendGetKuaFuPlunderBiddingData(int ServerId, int Money)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				ServerId,
				Money
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1250));
		}

		public void SendKuaFuPlunderJUeXingShopBuy(int GoodsID, int Number)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				GoodsID,
				Number
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1258));
		}

		public void SendKuaFuPlunderJUeXingShopRefresh()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1259));
		}

		public void BuyKuaFuPlundeEnterNum(int num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1248));
		}

		public void SendGetKuaFuPlundeEnterMap(int ServerID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ServerID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1251));
		}

		public void SendGetKuaFuPlundeGetAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1255));
		}

		public void SendGetKuaFuPlundeScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1253));
		}

		public void SendCancelChangMap(int toMapCode, int mapX, int mapY, int direction, int relife)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			byte[] array = new SCMapChange
			{
				RoleID = this.CurrentSession.RoleID,
				TeleportID = 0,
				NewMapCode = toMapCode,
				ToNewMapX = mapX,
				ToNewMapY = mapY,
				ToNewDiection = direction,
				State = 1
			}.toBytes();
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, array, 0, array.Length, 123));
		}

		public void GetGuanZhanRoleMiniDatalist()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1402));
		}

		public void SendGuanZhanTrackOtherPlayer(int trackId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				trackId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1403));
		}

		public void SendZuoQiRide()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1899));
		}

		public void SendZuoQiSkill(int Skillid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				Skillid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1901));
		}

		public void CMD_SPR_GET_MEDITATE_GOODS_Request()
		{
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				1
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3002));
		}

		public void SendGameCenterMsg(int flag)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				PlatSDKMgr.PlatName,
				flag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 692));
		}

		public void SendGameCenterAwardMsg(int param = 0)
		{
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				PlatSDKMgr.PlatName,
				param
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 693));
		}

		public void SendSetYinJi(int idType1, int idType2)
		{
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				idType1,
				idType2
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2033));
		}

		public void SendYinJiLevelUp(int id, int type, int count)
		{
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				id,
				type,
				count
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2030));
		}

		public void RequestYinJiInfo()
		{
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2031));
		}

		public void SendResetYinJi()
		{
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2032));
		}

		public void SendHuiGuiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2070));
		}

		public void SendGetHuiGuiQianDaoInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2071));
		}

		public void SendHuiGuiQianDao(int huiGuiId, int dayNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				huiGuiId,
				dayNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2072));
		}

		public void SendGetHuiGuiStoreInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2073));
		}

		public void SendHuiGuiBuyItem(int id, int huiGuiID, int goodsId, int buyNum)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				id,
				huiGuiID,
				goodsId,
				buyNum
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2074));
		}

		public void SendGetHuiGuiChongZhiInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2075));
		}

		public void SendHuiGuiLingQuChongZhi(int id, int huiGuiID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				huiGuiID,
				id
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2076));
		}

		public void SendGetHuiGuiZhiGouInfo(int huiGuiId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				huiGuiId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2077));
		}

		public void SendGetJinTuanInfo(int auctionType, int m_SearchOrderType, int orderType, int startPage, int eachPageCount, string filterNames, int qualities)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
			{
				this.CurrentSession.RoleID,
				auctionType,
				m_SearchOrderType,
				orderType,
				startPage,
				eachPageCount,
				filterNames,
				qualities
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2080));
		}

		public void SendJinTuanJingJia(int auctionType, string AuctionItemKey, int Price)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				auctionType,
				AuctionItemKey,
				Price
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2081));
		}

		public void SendRoleGetRebirthAward(int RoadID, int mapID, int ExtensionID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				mapID,
				RoadID,
				ExtensionID
			});
			MUDebug.Log<string>(new string[]
			{
				text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1717));
		}

		public void SendRoleGetRebirthBossdata(int mapID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				mapID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1715));
		}

		public void SendRoleGetRebirthRankdata(int rankType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				rankType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1714));
		}

		public void SendRoleToRebirthADMoBai(int rankType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				rankType
			});
			MUDebug.Log<string>(new string[]
			{
				text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1713));
		}

		public void RequestTeamCompeteMianInfoMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3680));
		}

		public void RequestOtherTeamInfoMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3688));
		}

		public void SendCreateTeamMsg(string teamName, string xuanYan = null)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				teamName,
				xuanYan
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3685));
		}

		public void RequestSingleTeamInfoMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3695));
		}

		public void SendUpdateZhanDuiXuanYanMsg(string info)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				info
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3691));
		}

		public void SendInviteTeamMemberMsg(string otherId, string otherName = null)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				otherId,
				otherName
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3687));
		}

		public void SendAcceptInviteInfoMsg(int teamID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				teamID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3690));
		}

		public void SendAcceptRequestJoinTeamMsg(int roleId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				roleId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3721));
		}

		public void RequestDeleteTeamMemberMsg(int otherId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3686));
		}

		public void SendChangeTeamLeaderMsg(int otherId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				otherId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3698));
		}

		public void SendLeaveTeamMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3711));
		}

		public void SendDeleteTeamMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3699));
		}

		public void SendSearchEnemyMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3692));
		}

		public void SendCancelSearchEnemyMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3693));
		}

		public void RequestZhanBaoInfoMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3709));
		}

		public void RequestDuanWeiRankInfoMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3704));
		}

		public void RequestEnterTeamCompeteScene(int gameID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				gameID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3697));
		}

		public void RequestConfirmBattleTeamInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3712));
		}

		public void SendTeamLeaderConfirmBattleMsg()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3713));
		}

		public void SendIsAcceptBattleMsg(int ret)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ret
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3714));
		}

		public void RequestZhanDuiAcceptAward()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3679));
		}

		public void RequestJoinTeamCompete(int ZhanDuiID)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				ZhanDuiID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 3719));
		}

		public void SendGetRoleTeQuanActivityXMLData(int clientVersion)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				clientVersion
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1495));
		}

		public void SendGetRoleTeQuanBuyOrGetAward(int tequanID, int actID, int num)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}:{3}", new object[]
			{
				this.CurrentSession.RoleID,
				tequanID,
				actID,
				num
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1497));
		}

		public void SendTeQuanActiviteRoleJuanZeng(int tequanID, int useZuanshi)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				this.CurrentSession.RoleID,
				tequanID,
				useZuanshi
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1498));
		}

		public void SendRidePetShengYinLoad(int DBid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				DBid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2091));
		}

		public void SendRidePetShengYinShengJi(int DBid, string Goods)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				DBid,
				Goods
			});
			MUDebug.Log<string>(new string[]
			{
				text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2090));
		}

		public void SendRidePetReset(int DBid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				DBid
			});
			MUDebug.Log<string>(new string[]
			{
				text
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2094));
		}

		public void SendRidePetShengYinUnLoad(int DBid)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				DBid
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2092));
		}

		public void SendRidePetShengYinTrimBag()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2093));
		}

		public void SendDuoBaoGetBaseInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2100));
		}

		public void SendDuoBaoEnter()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2101));
		}

		public void SendDuoBaoGetAwardImfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2102));
		}

		public void SendDuoBaoGetActivityState()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2103));
		}

		public void SendDuoBaoSideScore()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2104));
		}

		public void SendDuoRankInfo()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2105));
		}

		public void SendDuoGetAward(int id)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				id
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2108));
		}

		public void SendBaoMing()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2109));
		}

		public void SendMoShenShengJie(int type, int xingType)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				type,
				xingType
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2097));
		}

		public void SendTeamZhengBaEnter()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1277));
		}

		public void RequestTeamZhengBaMainInfo(long flag)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				flag
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1272));
		}

		public void RequestTeamMatchList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1273));
		}

		public void SendTeamMatchYaZhu(int teamId)
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}:{1}", new object[]
			{
				this.CurrentSession.RoleID,
				teamId
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1275));
		}

		public void RequestTeamMatchYaZhuList()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1280));
		}

		public void RequestTeamZhengBaZhanBao()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 1276));
		}

		public void RequestDaTaoShaMainData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2112));
		}

		public void RequestRegisterDaTaoSha()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2116));
		}

		public void RequestEnterDaTaoShaScene()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2110));
		}

		public void RequestDaTaoShaRankData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2114));
		}

		public void SendScceptDaTaoShaAwardData()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2115));
		}

		public void RequestBuyMoShenBuff()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2119));
		}

		public void SendInviteDaTaoShaTeamMembers()
		{
			if (!Global.Data.PlayGame)
			{
				return;
			}
			string text = string.Empty;
			text = StringUtil.substitute("{0}", new object[]
			{
				this.CurrentSession.RoleID
			});
			this.tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(this.tcpClient.OutPacketPool, text, 2117));
		}

		private TCPGame.GameStates _GameState;

		public Session CurrentSession = new Session();

		private TCPClient tcpClient = new TCPClient(1);

		public bool _ActiveDisconnect;

		private int otherObjectFaceID;

		public bool isOtherFaceId;

		public enum GameStates
		{
			CLIENT_READY,
			CLIENT_CONNECTING,
			CLIENT_CONNECTED,
			CLIENT_LOGON
		}
	}
}
