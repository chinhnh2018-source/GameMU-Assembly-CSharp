using System;
using System.Collections;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class UserLogin : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		string loginMode = Global.GetLoginMode();
		if ("0" == loginMode)
		{
			this.LoginBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.SubmitBtn_Click);
			this.RegisterBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.RegisterBtn_Click);
			this.UserName.text = PlayerPrefs.GetString("userName", string.Empty);
			this.UserPassword.text = PlayerPrefs.GetString("password", string.Empty);
		}
		else
		{
			this.LoginBtn.gameObject.SetActive(false);
			this.RegisterBtn.gameObject.SetActive(false);
			this.Background.gameObject.SetActive(false);
		}
		this.UserControl_Loaded();
		if (this.Is3DBackground)
		{
			base.StartCoroutine<bool>(this.Init3DMap());
		}
	}

	private IEnumerator Init3DMap()
	{
		bool initScene = false;
		if (null == Global.Login3DBakMapLoader)
		{
			WWW www = null;
			if (Global.IsTuiGuangFenBao)
			{
				www = new WWW(PathUtils.WebPath("Map/denglu_TG.unity3d"));
			}
			else
			{
				www = new WWW(PathUtils.WebPath("Map/Denglu.unity3d"));
			}
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
			Global.Login3DBakMapLoader = www.assetBundle;
			if (null != Global.Login3DBakMapLoader)
			{
				initScene = true;
			}
			www.Dispose();
			www = null;
		}
		if (initScene)
		{
			Global.AudioListener43D.enabled = false;
			CameraShake.Instance.enabled = false;
			CameraShake.Instance.OriginPosition = new Vector3(8f, 2f, 98f);
			Global.MainCamera.transform.localPosition = new Vector3(8f, 2f, 98f);
			Global.MainCamera.transform.localRotation = Quaternion.Euler(0.597f, 181.89f, -0.6f);
			Global.MainCamera.far = 2000f;
			if (Global.IsTuiGuangFenBao)
			{
				Global.MainCamera.fieldOfView = 21f;
			}
			else
			{
				Global.MainCamera.fieldOfView = 35f;
			}
			LoadingMap.ClearSpeicalMapEffect();
			PerformanceCtrl.PerformanceType = PerformanceTypes.HiUsage;
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 1000f);
			string levelName = null;
			if (Global.IsTuiGuangFenBao)
			{
				levelName = "denglu_TG";
			}
			else
			{
				levelName = "denglu";
			}
			AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
			Global.DirectLight.enabled = false;
			Global.IsInGameScene = false;
			yield return asyncOperation;
			PerformanceCtrl.CopyCameraParmasForDengLu();
		}
		yield break;
	}

	public void DestroyLogin3Map()
	{
		if (null != Global.Login3DBakMapLoader)
		{
			Global.Login3DBakMapLoader.Unload(true);
			Global.Login3DBakMapLoader = null;
			GameObject gameObject = GameObject.FindWithTag("MainCamera");
			if (null != gameObject)
			{
				Global.DirectLight.enabled = true;
				Global.MainCamera.transform.localPosition = Vector3.zero;
				Global.MainCamera.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
				Global.MainCamera.far = 35f;
				Global.MainCamera.fieldOfView = 30f;
				RenderSettings.ambientLight = new Color(1f, 1f, 1f);
				Global.AudioListener43D.enabled = false;
				Global.AudioListener4UI.enabled = true;
				Global.BackgroundAudio4UI.PlayAudio("Audio/Map/RoleManager.mp3", true, false);
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void SubmitBtn_Click(object sender, MouseEvent e)
	{
		if (UserLogin.RECORD_LOGIN_FIRST_ID != PlatformUserLogin.GamePingTaiID)
		{
			UserLogin.RECORD_LOGIN_FIRST_ID = PlatformUserLogin.GamePingTaiID;
			LoadConfig.ClearConfigData();
			Super.InitDonwloadConfig();
		}
		Global.ClearJieRiHuoDongConfig();
		if (ConfigTasks.TaskXmlNodeDict.Count <= 0 || ConfigGoods.GoodsXmlNodeDict.Count <= 0 || ConfigNPCs.NPCVODict.Count <= 0 || ConfigMonsters.MonsterXmlNode.Count <= 0)
		{
			if (this.UITimer == null)
			{
				this.UITimer = new DispatcherTimer("UserLogin_wait_for_data_UITimer")
				{
					Interval = TimeSpan.FromMilliseconds(500.0)
				};
				this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
				this.UITimer.Start();
				Super.ShowNetWaiting(null);
			}
			return;
		}
		this.SubmitBtn_ClickReal(sender, e);
	}

	private void StopUITimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Stop();
			this.UITimer.Tick = null;
			this.UITimer = null;
		}
	}

	private void UITimer_Tick(object sender, object e)
	{
		if (ConfigTasks.TaskXmlNodeDict.Count > 0 && ConfigGoods.GoodsXmlNodeDict.Count > 0 && ConfigNPCs.NPCVODict.Count > 0 && ConfigMonsters.MonsterXmlNode.Count > 0)
		{
			this.StopUITimer();
			Super.HideNetWaiting();
			this.SubmitBtn_ClickReal(null, null);
		}
	}

	private void SubmitBtn_ClickReal(object sender, MouseEvent e)
	{
		if (string.IsNullOrEmpty(this.UserName.text) || string.IsNullOrEmpty(this.UserPassword.text))
		{
			Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("请输入正确的用户名和密码"), new object[0]), -1, -1, -1, -1, false);
			return;
		}
		this.ConnectToLoginServer();
	}

	private void RegisterBtn_Click(object sender, MouseEvent e)
	{
		Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("提示"), Global.GetLang("暂时未开放用户注册，请联系我们获取测试账号!"), -1, -1, -1, -1, false);
	}

	public void ConnectToLoginServer()
	{
		string xapParamByName = Super.GetXapParamByName("serverip", "192.168.0.206");
		this.MyLoginIP = xapParamByName;
		Super.ShowNetWaiting(Global.GetLang("正在连接用户服务器..."));
		if (this.ActiveDisconnect)
		{
			this.ActiveDisconnect = false;
			this.ResetTCPClient();
			this.tcpClient.SocketConnect += this.SocketConnect;
		}
		this.tcpClient.Connect(xapParamByName, Global.GetUserLoginPort());
	}

	public void ResetTCPClient()
	{
		if (this.tcpClient != null)
		{
			this.tcpClient.Disconnect(2);
			this.tcpClient.Destroy();
			this.tcpClient = new TCPClient(2);
		}
	}

	private void UserControl_Loaded()
	{
		this.tcpClient.SocketConnect += this.SocketConnect;
		string loginMode = Global.GetLoginMode();
		if (!("0" == loginMode))
		{
			this.ConnectToLoginServer();
		}
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
						this.UserName.text = Global.StringReplaceAll(this.UserName.text, ":", string.Empty);
						this.UserPassword.text = Global.StringReplaceAll(this.UserPassword.text, ":", string.Empty);
						num = 1;
						text = StringUtil.substitute("{0}:{1}:{2}", new object[]
						{
							20140624,
							this.UserName.text,
							this.UserPassword.text
						});
						bytes = new UTF8Encoding().GetBytes(text);
						PlayerPrefs.SetString("userName", this.UserName.text);
						PlayerPrefs.SetString("password", this.UserPassword.text);
					}
					else
					{
						string text2 = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
						string text3 = Global.StringReplaceAll(Super.GetXapParamByName("n", string.Empty), ":", string.Empty);
						string text4 = Global.StringReplaceAll(Super.GetXapParamByName("t", string.Empty), ":", string.Empty);
						string text5 = Global.StringReplaceAll(Super.GetXapParamByName("cm", string.Empty), ":", string.Empty);
						string text6 = Global.StringReplaceAll(Super.GetXapParamByName((!Global.GetNowServerIsZhuTiFu(Global.Data.ServerData.LastServer)) ? "token" : "Zttoken", string.Empty), ":", string.Empty);
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
						MUDebug.Log<string>(new string[]
						{
							string.Concat(new object[]
							{
								"SeverId : ",
								Global.Data.ServerData.LastServer.nZoneID,
								"  token : ",
								text6
							})
						});
					}
					TCPOutPacket tcpoutPacket = new TCPOutPacket();
					tcpoutPacket.PacketCmdID = (short)num;
					tcpoutPacket.FinalWriteData(bytes, 0, bytes.Length);
					this.tcpClient.SendData(tcpoutPacket);
				}
				else
				{
					this.ActiveDisconnect = true;
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("连接服务器失败，请稍后再试"), new object[0]), -1, -1, -1, -1, false);
				}
				break;
			case 1:
				this.ActiveDisconnect = true;
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("向游戏用户服务器发送信息失败"), new object[0]), -1, -1, -1, -1, false);
				break;
			case 2:
				break;
			case 3:
				GScene.ServerStopGame();
				if (!this.ActiveDisconnect)
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("与游戏用户服务器的连接被断开"), new object[0]), -1, -1, -1, -1, false);
				}
				break;
			case 4:
				this.ActiveDisconnect = true;
				this.tcpClient.Disconnect(2);
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
				else
				{
					GameInstance.Game.CurrentSession.UserID = e.fields[0];
					GameInstance.Game.CurrentSession.UserName = e.fields[1];
					GameInstance.Game.CurrentSession.UserToken = e.fields[2];
					GameInstance.Game.CurrentSession.UserIsAdult = Convert.ToInt32(e.fields[3]);
					Super.HideNetWaiting();
					this.tcpClient.SocketConnect -= this.SocketConnect;
					this.tcpClient.Destroy();
					this.tcpClient = null;
					if (this.LoginGameToLineServer != null)
					{
						Global.g_strUserName = this.UserName.text;
						Global.g_strPassWord = this.UserPassword.text;
						this.LoginGameToLineServer.Invoke(this, EventArgs.Empty);
					}
				}
				break;
			default:
				throw new Exception(Global.GetLang("错误的Socket操作类型"));
			}
		});
	}

	public UILabel UserName;

	public UILabel UserPassword;

	public GButton LoginBtn;

	public GButton RegisterBtn;

	public new ShowNetImage Background;

	private bool Is3DBackground = true;

	protected DispatcherTimer UITimer;

	private static string RECORD_LOGIN_FIRST_ID = string.Empty;

	public EventHandler LoginGameToLineServer;

	private TCPClient tcpClient = new TCPClient(2);

	private bool ActiveDisconnect;

	private string MyLoginIP = string.Empty;
}
