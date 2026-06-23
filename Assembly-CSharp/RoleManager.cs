using System;
using System.Collections;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RoleManager : UserControl
{
	public bool DirectLogin { get; set; }

	public bool ConnectFailed { get; set; }

	private void TcpReLoadProcInit()
	{
		string xapParamByName = Super.GetXapParamByName("serverip", string.Empty);
		this.m_tcpClient.SocketConnect += this.SocketConnect;
		this.m_tcpClient.Connect(xapParamByName, Global.GetUserLoginPort());
		Super.ShowNetWaiting(Global.GetLang("正在连接用户服务器..."));
	}

	private void TcpReLoadProcInit(string ip, int port)
	{
		this.m_tcpClient.SocketConnect += this.SocketConnect;
		this.m_tcpClient.Connect(ip, port);
		Super.ShowNetWaiting(Global.GetLang("正在连接用户服务器..."));
	}

	private void SocketConnect(object sender, MUSocketConnectEventArgs e)
	{
		MainGame.QueueOnMainThread(delegate()
		{
			switch (e.NetSocketType)
			{
			case 0:
				if (e.Error == "Success")
				{
					string text = string.Empty;
					int num;
					byte[] bytes;
					if ("0" == Global.GetLoginMode())
					{
						string empty = string.Empty;
						string empty2 = string.Empty;
						string empty3 = string.Empty;
						string empty4 = string.Empty;
						string empty5 = string.Empty;
						if (KuaFuLoginManager.IsKuaFuLoginMode1(ref empty, ref empty2, ref empty3, ref empty4, ref empty5))
						{
							num = 20;
							text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								20140624,
								empty,
								empty2,
								empty3,
								empty4,
								empty5
							});
							bytes = new UTF8Encoding().GetBytes(text);
						}
						else
						{
							num = 1;
							text = StringUtil.substitute("{0}:{1}:{2}", new object[]
							{
								20140624,
								Global.g_strUserName,
								Global.g_strPassWord
							});
							bytes = new UTF8Encoding().GetBytes(text);
						}
					}
					else
					{
						string text2 = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
						string text3 = Global.StringReplaceAll(Super.GetXapParamByName("n", string.Empty), ":", string.Empty);
						string text4 = Global.StringReplaceAll(Super.GetXapParamByName("t", string.Empty), ":", string.Empty);
						string text5 = Global.StringReplaceAll(Super.GetXapParamByName("cm", string.Empty), ":", string.Empty);
						string text6 = Global.StringReplaceAll(Super.GetXapParamByName((!Global.GetNowServerIsZhuTiFu(Global.Data.ServerData.LastServer)) ? "token" : "Zttoken", string.Empty), ":", string.Empty);
						MUDebug.Log<string>(new string[]
						{
							string.Concat(new object[]
							{
								"token:",
								text6,
								"   ServerID : ",
								Global.Data.ServerData.LastServer.nZoneID
							})
						});
						num = 20;
						text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							20140624,
							text2,
							text3,
							text4,
							text5,
							text6
						});
						bytes = new UTF8Encoding().GetBytes(text);
					}
					TCPOutPacket tcpoutPacket = new TCPOutPacket();
					tcpoutPacket.PacketCmdID = (short)num;
					tcpoutPacket.FinalWriteData(bytes, 0, bytes.Length);
					this.m_tcpClient.SendData(tcpoutPacket);
					Super.HideNetWaiting();
				}
				else
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("连接游戏用户服务器失败"), new object[0]), -1, -1, -1, -1, false);
				}
				break;
			case 1:
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("向游戏用户服务器发送信息失败"), new object[0]), -1, -1, -1, -1, false);
				break;
			case 2:
				break;
			case 3:
				GScene.ServerStopGame();
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("与游戏用户服务器的连接被断开"), new object[0]), -1, -1, -1, -1, false);
				break;
			case 4:
				this.m_tcpClient.Disconnect(2);
				if ("-1" == e.fields[0])
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("您输入的用户名或密码错误, 请重新输入后再试..."), -1, -1, -1, -1, false);
				}
				else if ("-2" == e.fields[0])
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("登陆用户服务器时失败, 客户端的版本太旧，请更新客户端后再重新登陆"), -1, -1, -1, -1, false);
				}
				else if ("-100" == e.fields[0])
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("登录口令过期,请退出游戏重新登录"), -1, -1, -1, -1, false);
				}
				else
				{
					GameInstance.Game.CurrentSession.UserID = e.fields[0];
					GameInstance.Game.CurrentSession.UserName = e.fields[1];
					GameInstance.Game.CurrentSession.UserToken = e.fields[2];
					GameInstance.Game.CurrentSession.UserIsAdult = Convert.ToInt32(e.fields[3]);
					Super.HideNetWaiting();
					this.m_tcpClient.SocketConnect -= this.SocketConnect;
					this.m_tcpClient.Destroy();
					this.m_tcpClient = null;
					this.UserControl_Loaded(this);
				}
				break;
			default:
				throw new Exception(Global.GetLang("错误的Socket操作类型"));
			}
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (Global.g_bReconnRoleManager)
		{
			string ip;
			int port;
			if (KuaFuLoginManager.LoginKuaFuServer(out ip, out port))
			{
				this.TcpReLoadProcInit(ip, port);
			}
			else
			{
				this.TcpReLoadProcInit();
			}
		}
		if (!Global.g_bReconnRoleManager)
		{
			this.UserControl_Loaded(this);
		}
		this.clientHeartTimerForRole = new DispatcherTimer("clientHeartTimerForRole");
		this.clientHeartTimerForRole.Interval = TimeSpan.FromSeconds(10.0);
		this.clientHeartTimerForRole.Tick = new DispatcherTimerEventHandler(this.clientHeartTimer_Tick);
		this.clientHeartTimerForRole.Start();
		this.currentPingTicks = TimeManager.GetCorrectLocalTime();
	}

	protected void clientHeartTimer_Tick(object sender, EventArgs e)
	{
		if ((TimeManager.GetCorrectLocalTime() - this.currentPingTicks) / 1000L > 60L)
		{
			this.ClosedBySDK();
			return;
		}
		GameInstance.Game.SpriteHeart();
	}

	public override void Destroy()
	{
		base.Destroy();
		GameObject gameObject = GameObject.FindWithTag("MainCamera");
		if (null != gameObject)
		{
			Global.MainCamera.transform.localPosition = Vector3.zero;
			Global.MainCamera.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
			Global.MainCamera.far = 35f;
			Global.MainCamera.fieldOfView = 30f;
			RenderSettings.ambientLight = new Color(1f, 1f, 1f);
		}
		Super.HideNetWaiting();
		DispatcherTimerDriver.RemoveTimer("clientHeartTimerForRole");
		this.CloseWaitingQueue();
		this.CloseServerBusyWindow();
	}

	private IEnumerator Reconnect(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameInstance.CreateNewTCPGame();
		Super.ShowRoleManager(Super.MainWindowRoot);
		Object.Destroy(base.gameObject);
		yield break;
	}

	private void GameSocketFailed(object sender, MUSocketConnectEventArgs e)
	{
		int errorCode = 0;
		if (e != null && e.fields != null && e.fields.Length > 0)
		{
			errorCode = Convert.ToInt32(e.fields[0]);
		}
		MainGame.QueueOnMainThread(delegate()
		{
			if (KuaFuLoginManager.DirectLogin() && e.CmdID == 100)
			{
				if (errorCode == -11007)
				{
					e.ShowMsgBox = false;
					KuaFuLoginManager.ChangeToOriginalServer();
					GameInstance.Game.SocketFailed -= this.GameSocketFailed;
					GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
					GameInstance.Game.SocketCommand -= this.GameSocketCommand;
					GameInstance.Game.Disconnect();
					this.StartCoroutine(this.Reconnect(2.5f));
					return;
				}
				if (errorCode == -2)
				{
					e.ShowMsgBox = false;
					GameInstance.Game.SocketFailed -= this.GameSocketFailed;
					GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
					GameInstance.Game.SocketCommand -= this.GameSocketCommand;
					GameInstance.Game.Disconnect();
					this.StartCoroutine(this.Reconnect(2.5f));
					return;
				}
			}
			this.ConnectFailed = true;
			Super.HideNetWaiting();
			GameInstance.Game.SocketFailed -= this.GameSocketFailed;
			GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
			GameInstance.Game.SocketCommand -= this.GameSocketCommand;
			GameInstance.Game.Disconnect();
			if (e.ShowMsgBox)
			{
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("提示"), e.ErrorMsg, -1, -1, -1, -1, e.ReturnStartPage);
			}
			GameInstance.CreateNewTCPGame();
			if (this.StartGameByRole != null)
			{
				this.StartGameByRole.Invoke(this, EventArgs.Empty);
			}
			if (!string.IsNullOrEmpty(e.ErrorMsg))
			{
				string text = "err=" + e.ErrorMsg;
				if (e.fields != null)
				{
					for (int i = 0; i < e.fields.Length; i++)
					{
						text = text + ",p=" + e.fields[i];
					}
				}
				MUDebug.LogError<string>(new string[]
				{
					text
				});
			}
		});
	}

	private void GameSocketSuccess(object sender, object e)
	{
		MainGame.QueueOnMainThread(delegate()
		{
			if (!this.DirectLogin)
			{
				GameInstance.Game.GetRoleList(Global.Data.GameServerID);
			}
			Super.HideNetWaiting();
			if (this.DirectLogin)
			{
				Super.ShowNetWaiting(Global.GetLang("正在进入游戏..."));
				GameInstance.Game.InitPlayGame();
			}
		});
	}

	private void GameSocketCommand(object sender, MUSocketConnectEventArgs e)
	{
		MainGame.QueueOnMainThread(delegate()
		{
			if (e.CmdID == 101 && e.fields.Length > 1 && Convert.ToInt32(e.fields[0]) >= 0)
			{
				if (null != this.waitingQueue)
				{
					this.CloseWaitingQueue();
				}
			}
			else if (e.CmdID == 1110)
			{
				GVoiceSceneData gvoiceSceneData = DataHelper.BytesToObject<GVoiceSceneData>(e.bytesData, 0, e.bytesData.Length);
				if (gvoiceSceneData == null)
				{
					return;
				}
				Global.VoiceAPP_ID = gvoiceSceneData.SDKGameID;
				Global.VoiceAPP_Key = gvoiceSceneData.SDKKey;
			}
			if (e.CmdID >= 101 && e.CmdID <= 102)
			{
				Super.HideNetWaiting();
				if (e.fields.Length > 1 && Convert.ToInt32(e.fields[0]) > 0)
				{
					this.roleSelector.AddRoleList(e.fields[1], e.CmdID != 102);
					if (e.CmdID == 102)
					{
						this.roleSelector.DirectEnterGame(false);
						PlatSDKMgr.OnCreateRole(PlatSDKMgr._lastCreateRole, Global.GetTimeStamp().ToString());
						PlatSDKMgr.CreateRoleReport();
					}
				}
				else if (e.fields.Length > 1 && Convert.ToInt32(e.fields[0]) < 0)
				{
					int num = Convert.ToInt32(e.fields[0]);
					string message = string.Empty;
					if (e.CmdID == 101)
					{
						message = Global.GetLang("获取用户的角色列表失败");
						if (num == -1)
						{
							this.ShowWaitingQueue();
						}
						else if (num == -2)
						{
							this.ShowServerBusyWindow();
						}
						return;
					}
					if (num == -2)
					{
						message = Global.GetLang("服务器角色已满，无法创建新角色，请更换服务器。");
					}
					else if (num == -3)
					{
						message = Global.GetLang("创建角色失败, 名字包含特殊字符, 请换个名称后创建");
					}
					else if (num == -4)
					{
						message = Global.GetLang("创建角色失败, 角色名称已经存在, 请换个名称后创建");
					}
					else if (num == -5)
					{
						message = Global.GetLang("魔剑士创建需要至少有1个3转以上其他职业角色");
					}
					else if (num == -7)
					{
						string text = e.fields[1];
						int num2 = int.Parse(text);
						if (num2 > 0)
						{
							string timeStrBySecEx = Global.GetTimeStrBySecEx((double)num2, true, -1);
							message = string.Format(Global.GetLang("创建角色过于频繁,请等待{0}后再试!"), timeStrBySecEx);
						}
						else
						{
							message = Global.GetLang("创建角色过于频繁,请等待后再试!");
						}
					}
					else
					{
						message = Global.GetLang("创建角色失败, 角色名称已经存在, 请换个名称后创建");
					}
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("提示"), message, -1, -1, -1, -1, false);
				}
				else if (e.CmdID == 101 && this.roleSelector.GetRolesCount() <= 0)
				{
					this.roleSelector.ShowRoleListBox(-1);
					this.ShowCreateRolePanel(1);
				}
			}
			else if (e.CmdID == 103)
			{
				Super.HideNetWaiting();
				this.roleSelector.RemoveRole(e.fields[0]);
			}
			else if (e.CmdID == 98)
			{
				Super.HideNetWaiting();
				if (2 <= e.fields.Length)
				{
					this.roleSelector.UnRemoveRole(e.fields[0], e.fields[1]);
				}
				else
				{
					this.roleSelector.UnRemoveRole(e.fields[0], string.Empty);
				}
			}
			else if (e.CmdID == 99)
			{
				Super.HideNetWaiting();
				if (1 <= e.fields.Length)
				{
					this.roleSelector.CancelUnRemoveRole(e.fields[0]);
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"服务器发来的数据有误!(取消予删除)"
					});
				}
			}
			else if (e.CmdID == 512)
			{
				Super.HideNetWaiting();
				RoleData4Selector roleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(e.bytesData, 0, e.bytesData.Length);
				this.roleSelector.GetRoleUsingGoodsDataList(roleData4Selector);
				if (roleData4Selector.Occupation == 3 || roleData4Selector.Occupation == 4)
				{
					Global.currentMJSType = Global.GetMJSType(roleData4Selector.GoodsDataList, Global.CheckRoleOcc(roleData4Selector.Occupation, roleData4Selector.SubOccupation), 0);
				}
			}
			else if (e.CmdID == 104)
			{
				Global.Data.WaitingForMapChange = false;
				Global.g_nLoginTime = Global.GetCorrectLocalTime();
				RoleData roleData = DataHelper.BytesToObject<RoleData>(e.bytesData, 0, e.bytesData.Length);
				if (roleData == null)
				{
					string lang = Global.GetLang("初始化游戏登陆数据时失败");
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), lang, -1, -1, -1, -1, false);
					Super.HideNetWaiting();
				}
				if (roleData.RoleID < 0)
				{
					string message2 = string.Empty;
					if (roleData.RoleID == -10)
					{
						message2 = Global.GetLang("你已经被游戏管理员禁止登陆");
					}
					else if (roleData.RoleID == -20)
					{
						message2 = string.Format(Global.GetLang("系统检测您的角色使用加速,已被系统封号, 离解封还剩余: {0} 分钟"), (int)((double)roleData.BodyCode / 60.0));
					}
					else if (roleData.RoleID == -12)
					{
						KuaFuLoginManager.ClearLoginInfo();
						if (string.IsNullOrEmpty(roleData.RoleName))
						{
							message2 = string.Format(Global.GetLang("活动时间已结束,返回原服"), (int)((double)roleData.BodyCode / 60.0));
						}
						else
						{
							message2 = roleData.RoleName;
						}
					}
					else if (roleData.RoleID == -30)
					{
						if (Application.loadedLevelName == "empty")
						{
							KuaFuLoginManager.ClearLoginInfo();
							this.ClosedBySDK();
							return;
						}
						message2 = Global.GetLang("二级密码尚未验证");
					}
					else if (roleData.RoleID == -50)
					{
						message2 = string.Format(Global.GetLang("系统检测您的角色使用外挂,已被系统封号, 离解封还剩余: {0} 分钟"), (int)((double)roleData.BodyCode / 60.0));
					}
					else
					{
						if (roleData.RoleID == -60)
						{
							Super.HideNetWaiting();
							this.ClosedBySDK();
							return;
						}
						if (roleData.RoleID == -40)
						{
							float realtimeSinceStartup = Time.realtimeSinceStartup;
							if (null != MainGame._current)
							{
								MainGame._current.ShowStepServerTimeWaiting(roleData.BodyCode);
							}
							return;
						}
						if (roleData.RoleID == -80)
						{
							message2 = string.Format(Global.GetLang("系统检测您的角色交易异常,已被系统封号, 离解封还剩余: {0} 分钟"), (int)((double)roleData.BodyCode / 60.0));
						}
						else
						{
							message2 = Global.GetLang("初始化游戏登陆数据时失败");
						}
					}
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), message2, -1, -1, -1, -1, false);
					Super.HideNetWaiting();
					return;
				}
				GameInstance.Game.CurrentSession.RoleID = roleData.RoleID;
				KuaFuLoginManager.OnChangeServerComplete();
				GameInstance.Game.CurrentSession.roleData = roleData;
				GameInstance.Game.TimeSynchronization();
				Global.ClearFashionAndTitleData();
				HuoDongCommonFlag.ClearStaticData();
				Global.GetUsingGoodsDataList();
				Global.ChangeEmblemCoolDownData(Global.GetCorrectLocalTime(), 0L);
				JueXingNetManager.UpdateJueXingSuiPian(null);
				ConfigSystemOpen.CatulateSystemOpenByRoleData();
				Global.LoadSystemSettings();
				Global.LoadAutoFightSettings();
				Global.LoadZhuanShengPromptingSettings();
			}
			else if (e.CmdID == 105)
			{
				long num3 = Convert.ToInt64(e.fields[1]);
				long ticks = MyDateTime.Now().Ticks;
				long num4 = Convert.ToInt64(e.fields[2]);
				num4 += (ticks - num3) / 2L;
				if (!Global.SyscTimeCheck(ticks, num3))
				{
					return;
				}
				long localTimeSubServerTime = ticks - num4;
				Global.SetLocalTimeSubServerTime(localTimeSubServerTime);
				if (!this.DirectLogin)
				{
					this.roleSelector.StartGame();
				}
				else
				{
					GameInstance.Game.SocketFailed -= this.GameSocketFailed;
					GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
					GameInstance.Game.SocketCommand -= this.GameSocketCommand;
					if (this.StartGameByRole != null)
					{
						this.StartGameByRole.Invoke(this, EventArgs.Empty);
					}
				}
				Super.HideNetWaiting();
			}
			else if (e.CmdID == 257)
			{
				Super.HideNetWaiting();
				this.roleCreator.SetRandomPreName(e.fields[1]);
			}
			else if (e.CmdID == 860)
			{
				Global.HasSecondPassword = Convert.ToInt32(e.fields[0]);
				Global.NeedVerifySecondPassword = Convert.ToInt32(e.fields[1]);
			}
			else if (e.CmdID == 862)
			{
				if (!Global.AssertException(e.fields.Length == 3, StringUtil.substitute(Global.GetLang("命令参数个数错误: {0}, 命令: {1}"), new object[]
				{
					e.fields.Length,
					(TCPGameServerCmds)e.CmdID
				})))
				{
					return;
				}
				int errorCode = Convert.ToInt32(e.fields[0]);
				Global.HasSecondPassword = Convert.ToInt32(e.fields[1]);
				Global.NeedVerifySecondPassword = Convert.ToInt32(e.fields[2]);
				Global.HandleSecondPasswordErrorCode(errorCode);
			}
			else if (e.CmdID == 673)
			{
				MUDebug.LogError<string>(new string[]
				{
					"验证版本号"
				});
				if (!Global.AssertException(e.fields.Length == 3, StringUtil.substitute(Global.GetLang("命令参数个数错误: {0}, 命令: {1}"), new object[]
				{
					e.fields.Length,
					(TCPGameServerCmds)e.CmdID
				})))
				{
					return;
				}
				int num5 = Convert.ToInt32(e.fields[0]);
				if ((!string.IsNullOrEmpty(e.fields[1]) && string.Compare(Context.MainExeVer, e.fields[1], true) != 0) || (!string.IsNullOrEmpty(e.fields[2]) && string.Compare(Context.ResSwfVer, e.fields[2], true) != 0))
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("需要升级到最新版本才能继续游戏，请出游戏重新进入！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Application.Quit();
						}
						else if (messageBoxReturn == 1)
						{
							Application.Quit();
						}
						return true;
					};
				}
			}
			else if (e.CmdID == 14002)
			{
				ChangeNameInfo changeNameInfo = DataHelper.BytesToObject<ChangeNameInfo>(e.bytesData, 0, e.bytesData.Length);
				GaiMingPart._ChangeNameInfo = changeNameInfo;
			}
			else if (e.CmdID == 14001)
			{
				ChangeNameResult data = DataHelper.BytesToObject<ChangeNameResult>(e.bytesData, 0, e.bytesData.Length);
				GaiMingPart.Instance.GaiMingHuiDiao(data);
			}
			else if (e.CmdID == 971)
			{
				if (!Global.AssertException(e.fields.Length == 2, StringUtil.substitute(Global.GetLang("命令参数个数错误: {0}, 命令: {1}"), new object[]
				{
					e.fields.Length,
					(TCPGameServerCmds)e.CmdID
				})))
				{
					return;
				}
				int num6 = Convert.ToInt32(e.fields[0]);
				int leftSeconds = Convert.ToInt32(e.fields[1]);
				if (null != this.waitingQueue)
				{
					if (num6 <= 0)
					{
						this.CloseWaitingQueue();
						return;
					}
					this.waitingQueue.RefreshWaitingQueue(num6, leftSeconds);
				}
			}
			else if (e.CmdID == 23)
			{
				this.currentPingTicks = TimeManager.GetCorrectLocalTime();
			}
		});
	}

	private DateTime SafeConvertDateTime(string str)
	{
		int[] array = new int[6];
		string[] array2 = str.Split(new char[]
		{
			' '
		});
		if (array2.Length == 2)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				if (i == 0)
				{
					string[] array3 = array2[i].Split(new char[]
					{
						'-'
					});
					if (array3.Length == 3)
					{
						for (int j = 0; j < array3.Length; j++)
						{
							array[j] = int.Parse(array3[j]);
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"服务器传的时间格式有误"
						});
					}
				}
				else if (i == 1)
				{
					string[] array4 = array2[i].Split(new char[]
					{
						'#'
					});
					if (array4.Length == 3)
					{
						for (int k = 0; k < array4.Length; k++)
						{
							array[k + 3] = int.Parse(array4[k]);
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"服务器传的时间格式有误"
						});
					}
				}
			}
		}
		string text = string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]
		{
			array[0],
			array[1],
			array[2],
			array[3],
			array[4],
			array[5]
		});
		DateTime result;
		if (DateTime.TryParse(text, ref result))
		{
			return result;
		}
		return default(DateTime);
	}

	public IEnumerator ShowMovieAndCreatePanel()
	{
		if (this.roleSelector != null)
		{
			this.roleSelector.Visibility = false;
		}
		Handheld.PlayFullScreenMovie("finalsmall_nologo_2.mp4", Color.black, 3, 0);
		yield return new WaitForSeconds(0.2f);
		this.ShowCreateRolePanel(1);
		yield return null;
		yield break;
	}

	private void ShowCreateRolePanel(int nType = 0)
	{
		this.roleSelector.Visibility = false;
		if (null == this.roleCreator)
		{
			this.roleCreator = U3DUtils.NEW<RoleCreator>();
			NGUITools.AddChild2(base.MyGameObject, this.roleCreator);
			this.roleCreator.SetWindowType(nType);
			RoleCreator roleCreator = this.roleCreator;
			roleCreator.RolePanelChanged = (EventHandler)Delegate.Combine(roleCreator.RolePanelChanged, delegate(object s2, EventArgs e2)
			{
				string loginMode = Global.GetLoginMode();
				if ("0" == loginMode)
				{
					this.roleCreator.Visibility = false;
					this.roleSelector.Visibility = true;
					this.roleSelector.Show3DObjects();
				}
				else if (s2 is RoleCreator)
				{
					RoleCreator roleCreator2 = (RoleCreator)s2;
					if (!roleCreator2.HasRoleInfo)
					{
						GameInstance.Game.SocketFailed -= this.GameSocketFailed;
						GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
						GameInstance.Game.SocketCommand -= this.GameSocketCommand;
						GameInstance.Game.Disconnect();
						GameInstance.CreateNewTCPGame();
						Object.Destroy(base.gameObject);
						PlatformUserLogin platformUserLogin = Super.ShowPlatformUserLogin(Super.MainWindowRoot, true);
						platformUserLogin.StartCoroutine<bool>(platformUserLogin.ShowXuanFuFirstPart(Super.MainWindowRoot));
						return;
					}
					this.roleCreator.Visibility = false;
					this.roleSelector.Visibility = true;
					this.roleSelector.Show3DObjects();
				}
			});
		}
		else
		{
			this.roleCreator.Show3DObjects();
		}
		if (null != this.roleSelector)
		{
			this.roleCreator.ZHSCanCreate = this.roleSelector.ZHSCreatorCondition();
		}
		this.roleCreator.Visibility = true;
	}

	protected void ShowDeleteRoleWindow()
	{
		this.m_DeleteRoleWindow = U3DUtils.NEW<GChildWindow>();
		this.m_DeleteRoleWindow.ModalType = ChildWindowModalType.Translucent;
		NGUITools.AddChild2(base.MyGameObject, this.m_DeleteRoleWindow);
		Super.InitChildWindow(this.m_DeleteRoleWindow, "删除角色");
		if (null == this.m_DeleteRolePanle)
		{
			this.m_DeleteRolePanle = U3DUtils.NEW<DeleteRolePart>();
			this.m_DeleteRolePanle.m_RoleInfo = this.roleSelector.m_DeleteRoleInfo;
			this.m_DeleteRolePanle.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.m_DeleteRoleWindow.SetContent(this.m_DeleteRoleWindow.BodyPresenter, this.m_DeleteRolePanle, 0.0, 0.0, true);
			string occupationStr = Global.GetOccupationStr(this.roleSelector.m_DeleteRoleInfo.OccupType);
			this.m_DeleteRolePanle.m_RoleName.text = this.roleSelector.m_DeleteRoleInfo.strName;
			string text = string.Format(Global.GetLang("{0}转"), this.roleSelector.m_DeleteRoleInfo.nChangeLifeCount);
			this.m_DeleteRolePanle.m_RolLevel.text = text;
			this.m_DeleteRolePanle.m_RoleOccups.text = occupationStr;
			this.m_DeleteRolePanle.ItemEventHandler = delegate(object sender, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					string text2 = this.m_DeleteRolePanle.m_InPut.text;
					string text3 = this.m_DeleteRolePanle.m_LblYanZhengMa.text;
					if (string.Compare(text2, text3, 5) == 0)
					{
						this.CloseDeleteRoleWindow();
						GameInstance.Game.UnRemoveRole(this.roleSelector.m_DeleteRoleInfo.nID);
					}
					else
					{
						string lang = Global.GetLang("验证码错误！！！");
						Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), lang, -1, -1, -1, -1, false);
					}
				}
				if (e.ID == 1)
				{
					this.CloseDeleteRoleWindow();
				}
			};
		}
	}

	protected void CloseDeleteRoleWindow()
	{
		if (null != this.m_DeleteRoleWindow)
		{
			Object.Destroy(this.m_DeleteRoleWindow.gameObject);
			this.m_DeleteRoleWindow = null;
			this.m_DeleteRolePanle = null;
		}
	}

	private void UserControl_Loaded(MonoBehaviour sender)
	{
		if (this.Is3DBackground)
		{
		}
		if (!this.DirectLogin)
		{
			this.roleSelector = U3DUtils.NEW<RoleSelector>();
			NGUITools.AddChild2(base.MyGameObject, this.roleSelector);
			RoleSelector roleSelector = this.roleSelector;
			roleSelector.RolePanelChanged = (EventHandler)Delegate.Combine(roleSelector.RolePanelChanged, delegate(object s1, EventArgs e1)
			{
				int nType = 0;
				if (this.roleSelector.GetRolesCount() <= 0)
				{
					nType = 1;
				}
				this.ShowCreateRolePanel(nType);
			});
			RoleSelector roleSelector2 = this.roleSelector;
			roleSelector2.DeleteRoleBtnUp = (EventHandler)Delegate.Combine(roleSelector2.DeleteRoleBtnUp, delegate(object s1, EventArgs e)
			{
				this.ShowDeleteRoleWindow();
			});
			RoleSelector roleSelector3 = this.roleSelector;
			roleSelector3.StartGameByRole = (EventHandler)Delegate.Combine(roleSelector3.StartGameByRole, delegate(object s1, EventArgs e1)
			{
				this.roleCreator = null;
				this.roleSelector = null;
				GameInstance.Game.SocketFailed -= this.GameSocketFailed;
				GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
				GameInstance.Game.SocketCommand -= this.GameSocketCommand;
				if (this.StartGameByRole != null)
				{
					this.StartGameByRole.Invoke(sender, EventArgs.Empty);
				}
			});
			RoleSelector roleSelector4 = this.roleSelector;
			roleSelector4.GoBackEvent = (EventHandler)Delegate.Combine(roleSelector4.GoBackEvent, delegate(object s1, EventArgs e1)
			{
				this.roleCreator = null;
				this.roleSelector = null;
				GameInstance.Game.SocketFailed -= this.GameSocketFailed;
				GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
				GameInstance.Game.SocketCommand -= this.GameSocketCommand;
				if (this.StartGameByRole != null)
				{
					this.GoBackEvent.Invoke(sender, EventArgs.Empty);
				}
			});
		}
		if (GameInstance.Game.ConnectedState)
		{
			GameInstance.CreateNewTCPGame();
		}
		GameInstance.Game.SocketFailed += this.GameSocketFailed;
		GameInstance.Game.SocketSuccess += new SocketConnectEventHandler(this.GameSocketSuccess);
		GameInstance.Game.SocketCommand += this.GameSocketCommand;
		if (Global.CurrentListData != null)
		{
			Super.ShowNetWaiting(Global.GetLang("正在连接游戏服务器..."));
			Super.GData.FirstEnterGameServer = false;
			string ip;
			int port;
			if (KuaFuLoginManager.LoginKuaFuServer(out ip, out port))
			{
				GameInstance.Game.Connect(ip, port, true);
			}
			else
			{
				GameInstance.Game.Connect(Global.CurrentListData.GameServerIP, Global.CurrentListData.GameServerPort, true);
			}
		}
	}

	private IEnumerator Init3DMap()
	{
		yield break;
	}

	public void ClosedBySDK()
	{
		this.roleCreator = null;
		this.roleSelector = null;
		GameInstance.Game.SocketFailed -= this.GameSocketFailed;
		GameInstance.Game.SocketSuccess -= new SocketConnectEventHandler(this.GameSocketSuccess);
		GameInstance.Game.SocketCommand -= this.GameSocketCommand;
		if (this.StartGameByRole != null)
		{
			this.GoBackEvent.Invoke(this, EventArgs.Empty);
		}
	}

	protected void ShowWaitingQueue()
	{
		if (null == this.waitingWindow)
		{
			this.waitingWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(this.waitingWindow, "_waitingQueueWindow");
			this.Container.Children.Add(this.waitingWindow);
			this.waitingWindow.ModalType = ChildWindowModalType.TransBak;
			if (null == this.waitingQueue)
			{
				this.waitingQueue = U3DUtils.NEW<WaitingQueue>();
				this.waitingQueue.buttonClick = delegate(object s, EventArgs e)
				{
					this.CloseWaitingQueue();
					this.ClosedBySDK();
				};
			}
			this.waitingWindow.SetContent(this.waitingWindow.BodyPresenter, this.waitingQueue, 0.0, 0.0, true);
		}
	}

	public void CloseWaitingQueue()
	{
		if (null != this.waitingQueue)
		{
			Object.Destroy(this.waitingQueue.gameObject);
			this.waitingQueue = null;
		}
		if (null != this.waitingWindow)
		{
			Super.CloseChildWindow(this, this.waitingWindow);
			Object.Destroy(this.waitingWindow.gameObject);
			this.waitingWindow = null;
		}
	}

	protected void ShowServerBusyWindow()
	{
		if (null == this.serverBusyWindow)
		{
			this.serverBusyWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(this.serverBusyWindow, "_serverBusyWindow");
			this.Container.Children.Add(this.serverBusyWindow);
			this.serverBusyWindow.ModalType = ChildWindowModalType.TransBak;
			if (null == this.serverBusy)
			{
				this.serverBusy = U3DUtils.NEW<ServerBusy>();
				this.serverBusy.buttonClick = delegate(object s, EventArgs e)
				{
					this.CloseServerBusyWindow();
					this.ClosedBySDK();
				};
			}
			this.serverBusyWindow.SetContent(this.serverBusyWindow.BodyPresenter, this.serverBusy, 0.0, 0.0, true);
		}
	}

	public void CloseServerBusyWindow()
	{
		if (null != this.serverBusy)
		{
			Object.Destroy(this.serverBusy.gameObject);
			this.serverBusy = null;
		}
		if (null != this.serverBusyWindow)
		{
			Super.CloseChildWindow(this, this.serverBusyWindow);
			Object.Destroy(this.serverBusyWindow.gameObject);
			this.serverBusyWindow = null;
		}
	}

	private const long timeOut = 60L;

	public static GameObject DecoBackground;

	protected GChildWindow m_DeleteRoleWindow;

	protected DeleteRolePart m_DeleteRolePanle;

	public GChildWindow waitingWindow;

	public WaitingQueue waitingQueue;

	public GChildWindow serverBusyWindow;

	public ServerBusy serverBusy;

	private RoleSelector roleSelector;

	private RoleCreator roleCreator;

	public EventHandler StartGameByRole;

	public EventHandler GoBackEvent;

	private bool Is3DBackground = true;

	private TCPClient m_tcpClient = new TCPClient(2);

	private long currentPingTicks;

	protected DispatcherTimer clientHeartTimerForRole;
}
