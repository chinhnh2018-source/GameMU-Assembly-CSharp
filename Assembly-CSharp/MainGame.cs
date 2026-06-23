using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.Render;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Umeng;
using UnityEngine;

public class MainGame : TTMonoBehaviour
{
	public static bool IsUpdateEnabled { get; private set; }

	private void Awake()
	{
		MainGame.IsUpdateEnabled = true;
		MUDebug.IsOpenDebug = false;
		DebugTextLog.DebugLog = UnityDebugTextLog.DebugLog;
		MUDebug.Log<string>(new string[]
		{
			"MainGame:Awake, Begin"
		});
		MainGame.ticks = Global.GetMyTimer();
		MainGame._current = this;
		Application.runInBackground = true;
		Application.targetFrameRate = 40;
		this.rotMatrixID = Shader.PropertyToID("_RotateMatrix");
		this.roughnessLUTID = Shader.PropertyToID("_NHxRoughness");
		this.roughnessLUT = Resources.Load<Texture2D>("MUTexture/roughnessLUT");
		MUDebug.Log<string>(new string[]
		{
			"MainGame:Awake, End"
		});
	}

	private void Start()
	{
		MUDebug.InitFlag();
		bool strictChk = CheckSFAndWorker.checkNet();
		TmskTime.Init(strictChk);
		Global.setIOSNoBackup();
		MUDebug.Log<string>(new string[]
		{
			"MainGame:Start, Begin"
		});
		this.InitStep1();
		this.InitStep2();
		QMQJJava.Cpumemcrash();
		this.InitUmeng();
		MUDebug.Log<string>(new string[]
		{
			"MainGame:Start, End"
		});
		Global.LoadLangDict(true);
		if (Application.internetReachability == 1)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("您现在是通过非WiFi环境上网，产生的流量可能被运营商收取费用！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StartGame();
				}
				return true;
			};
		}
		else
		{
			this.StartGame();
		}
	}

	public IEnumerator LoadDownloadInfo()
	{
		string url = PathUtils.SteamingAssetsPath_DontUseThis("DownloadInfo.txt");
		if (string.IsNullOrEmpty(url))
		{
			yield break;
		}
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			string sinfo = www.text;
			if (sinfo != null)
			{
				string[] infos = sinfo.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < infos.Length; i++)
				{
					if (!string.IsNullOrEmpty(infos[i]) && !Global.DownLoadInfos.Contains(infos[i]))
					{
						Global.SaveDownloadInfo(infos[i]);
					}
				}
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				www.error
			});
		}
		www = null;
		yield break;
	}

	private void StartGame()
	{
		if (PlatSDKMgr.PlatName == "TM")
		{
			this.InitOK = true;
			Super.ShowLoadingGame(this.Stage);
			return;
		}
		base.StartCoroutine<bool>(UpdateGame.InitConfigVersion());
		UpdateGame.UpdateNextStep = delegate(object s, NextStepEventArgs e)
		{
			UpdateGame.UpdateNextStep = null;
			this.InitOK = true;
		};
	}

	private void InitStep1()
	{
		Global.MainStage = this.Stage;
		Global.GlobalMainWindow = this.Stage;
		Global.GlobalMainWindow.Height = 540.0;
		Global.GlobalMainWindow.Width = 960.0;
		Global.Joystick = this.Joystick;
		Global.BackgroundAudio4UI = this.BackgroundAudio4UI;
		Global.AudioListener4UI = this.AudioListener4UI;
		Global.BackgroundAudio43D = this.BackgroundAudio43D;
		Global.AudioListener43D = this.AudioListener43D;
		Global.DirectLight = this.DirectLight;
		Screen.sleepTimeout = -1;
		this.InitData();
		MyDateTime.Init();
		Global.MainCamera = this.MainCamera;
		Global.UICamera = this.UICamera;
		Global.DecorationUICamera = this.DecorationUICamera;
	}

	private void InitStep2()
	{
		Super.ModalLayer = this.ModalLayer;
		Super.DialogLayer = this.DialogLayer;
		Super.NetWaiting = this.NetWaiting;
		Super.GData.GlobalUIAudioSource = this.GlobalUIAudioSource;
		Super.MainWindowRoot = this.Stage;
	}

	private void InitUmeng()
	{
		string appKey = "55e407f4e0f55aaf4e002835";
		PlatSDKMgr.SDKInit();
		Analytics.StartWithAppKeyAndChannelId(appKey, PlatSDKMgr.PlatName);
		Analytics.SetLogEnabled(false);
	}

	private void InitData()
	{
		Global.Data = new GData();
		Super.GData = new SuperData();
	}

	private void Update()
	{
		TmskTime.Update(Time.deltaTime);
		if (Input.GetKeyDown(27) || Input.GetKeyDown(278))
		{
			if (null != PlayZone.GlobalPlayZone)
			{
				if (SystemHelpPart.IsMaskShowing())
				{
					this.exitCount++;
					if (this.exitCount > 2)
					{
						this.exitCount = 0;
						SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
					}
				}
				else
				{
					this.exitCount = 0;
					if (Global.HasReturnPingTaiName() && Global.HasReturn())
					{
						WindowManage.CloseMostTopChildWindow();
					}
					else
					{
						PlayZone.GlobalPlayZone.ShowSystemSettingWindow(0);
					}
				}
			}
			else
			{
				this.exitCount = 0;
				PlatSDKMgr.OnAppQuit();
				if (this.messageBoxWindow != null)
				{
					return;
				}
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("确定要退出游戏吗？"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					if (messageBoxReturn == 0)
					{
						Application.Quit();
					}
					else if (messageBoxReturn == 1)
					{
					}
					return true;
				};
			}
			return;
		}
		if (!this.InitOK)
		{
			return;
		}
		try
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			float num = Mathf.Sin(Time.time);
			float num2 = Mathf.Cos(Time.time);
			matrix4x[0, 0] = num2;
			matrix4x[0, 2] = -num;
			matrix4x[1, 1] = 1f;
			matrix4x[2, 0] = num;
			matrix4x[2, 2] = num2;
			matrix4x[3, 3] = 1f;
			Shader.SetGlobalMatrix(this.rotMatrixID, matrix4x);
			Shader.SetGlobalTexture(this.roughnessLUTID, this.roughnessLUT);
			Global.UpdateFrameRate();
			this.RenderGame();
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnApplicationQuit()
	{
		if (null != PlayZone.GlobalPlayZone)
		{
			PlayZone.GlobalPlayZone.CloseSocket();
		}
		PlatSDKMgr.OnAppQuit();
		MUDebug.Dispose();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			MUVoiceManager.GetInstance().Pause();
			BackStageDownloadManager.instance.SaveFiles();
		}
		else
		{
			MUVoiceManager.GetInstance().Resume();
		}
	}

	private void TouchJianCe()
	{
		if (MUBindingManager.Instance.beOpenMockMouse)
		{
			return;
		}
		if (Input.touchCount > 0)
		{
			this.frontTicks = DateTime.Now.Ticks;
		}
		if (DateTime.Now.Ticks - this.frontTicks >= (long)((ulong)-1894967296) && !this.isLowPower && null != PlayZone.GlobalPlayZone)
		{
			Application.targetFrameRate = 15;
			if (this.originHeight == 0 || this.originWidth == 0)
			{
				this.originHeight = Screen.height;
				this.originWidth = Screen.width;
			}
			Screen.SetResolution(this.originWidth >> 1, this.originHeight >> 1, true);
			if (Global.Data != null && Global.Data.SysSetting != null)
			{
				this.preHideOtherRoles = Global.Data.SysSetting.HideOtherRoles;
				if (!this.preHideOtherRoles)
				{
					Global.Data.SysSetting.HideOtherRoles = true;
				}
				this.preCloseMusic = Global.Data.SysSetting.CloseGameMusic;
				if (!this.preCloseMusic)
				{
					Global.Data.SysSetting.CloseGameMusic = true;
					if (null != Global.BackgroundAudio43D)
					{
						Global.BackgroundAudio43D.StopPlay();
					}
				}
			}
			PlayZone.GlobalPlayZone.OpenLowPowerPart(new DPSelectedItemEventHandler(this.OnLowPowerCloseHandler));
			this.isLowPower = true;
		}
	}

	private void OnLowPowerCloseHandler(object sender, DPSelectedItemEventArgs args)
	{
		this.frontTicks = DateTime.Now.Ticks;
		if (Application.targetFrameRate == 15)
		{
			Application.targetFrameRate = 40;
			if (this.originHeight != 0 && this.originWidth != 0 && Screen.height != this.originHeight)
			{
				Screen.SetResolution(this.originWidth, this.originHeight, true);
			}
		}
		if (this.isLowPower)
		{
			if (Global.Data != null && Global.Data.SysSetting != null)
			{
				Global.Data.SysSetting.HideOtherRoles = this.preHideOtherRoles;
				Global.Data.SysSetting.CloseGameMusic = this.preCloseMusic;
				if (!this.preCloseMusic && null != Global.BackgroundAudio43D && Global.Data.roleData != null)
				{
					Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, false), true, false);
				}
			}
			if (null != PlayZone.GlobalPlayZone)
			{
				PlayZone.GlobalPlayZone.CloseLowPower();
			}
		}
		this.isLowPower = false;
	}

	private void RenderGame()
	{
		this.TouchJianCe();
		this.UpdateRenderQuality();
		this.DoQueueMainActions();
		MonsterCachingManager.ClearCachingDeadItems();
		Global.ExecutionMemoryProtection();
		DispatcherTimerDriver.ExecuteTimers();
		if (Global.Data == null)
		{
			return;
		}
		if (Global.Data.WaitingForMapChange)
		{
			return;
		}
		StoryBoard.runStoryBoards(false);
		bool isLeaderMoving = false;
		if (null != Super.MainGameMgr)
		{
			Super.MainGameMgr.onRenderScene();
			Super.MainGameMgr.onUIRenderFrame();
		}
		RenderManager.ProcessRenderObject(isLeaderMoving, false);
		MUVoiceManager.GetInstance().Update();
		LianShaMgr.Instance.OnRenderGame();
	}

	private void AutoRunTasks()
	{
	}

	public static void QueueOnMainThread(Action action)
	{
		MainGame.QueueOnMainThread(action, 0f);
	}

	public static void QueueOnMainThread(Action action, float time)
	{
		if (time != 0f)
		{
			List<MainGame.DelayedQueueItem> delayed = MainGame._current._delayed;
			lock (delayed)
			{
				MainGame._current._delayed.Add(new MainGame.DelayedQueueItem
				{
					time = Time.time + time,
					action = action
				});
			}
		}
		else
		{
			List<Action> actions = MainGame._current._actions;
			lock (actions)
			{
				MainGame._current._actions.Add(action);
			}
		}
	}

	private void DoQueueMainActions()
	{
		List<Action> actions = this._actions;
		lock (actions)
		{
			if (this._actions.Count > 0)
			{
				this._currentActions.AddRange(this._actions);
				this._actions.Clear();
			}
		}
		if (this._currentActions.Count > 0)
		{
			for (int i = 0; i < this._currentActions.Count; i++)
			{
				try
				{
					this._currentActions[i].Invoke();
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
			}
			this._currentActions.Clear();
		}
		List<MainGame.DelayedQueueItem> delayed = this._delayed;
		lock (delayed)
		{
			if (this._delayed.Count > 0)
			{
				this._currentDelayed.AddRange(Enumerable.Where<MainGame.DelayedQueueItem>(this._delayed, (MainGame.DelayedQueueItem d) => d.time <= Time.time));
				for (int j = 0; j < this._currentDelayed.Count; j++)
				{
					this._delayed.Remove(this._currentDelayed[j]);
				}
			}
		}
		if (this._currentDelayed.Count > 0)
		{
			for (int k = 0; k < this._currentDelayed.Count; k++)
			{
				try
				{
					this._currentDelayed[k].action.Invoke();
				}
				catch (Exception ex2)
				{
					MUDebug.LogException(ex2);
				}
			}
			this._currentDelayed.Clear();
		}
	}

	public void StartCheckHasNetwork()
	{
		this.CheckValue = 0f;
		if (MainGame._current != null)
		{
			MainGame._current.InvokeRepeating("CheckHasNetwork", this.CheckNetworkRate, this.CheckNetworkRate);
		}
	}

	public void CancelCheckHasNetwork()
	{
		this.CheckValue = 0f;
		if (MainGame._current != null)
		{
			MainGame._current.CancelInvoke("CheckHasNetwork");
		}
	}

	public void CheckHasNetwork()
	{
		if (Application.internetReachability == null)
		{
			this.CheckValue += 1f;
		}
		else
		{
			this.CheckValue = 0f;
		}
		if (this.CheckValue > 5f)
		{
			this.CancelCheckHasNetwork();
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接,请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
		}
	}

	public void QuickClient(int second)
	{
		if (second <= 0)
		{
			Application.Quit();
			return;
		}
		base.Invoke("Quick_Client", (float)second);
	}

	public void Quick_Client()
	{
		Application.Quit();
	}

	private void UpdateRenderQuality()
	{
		this.mAvgFrameRate = this.mAvgFrameRate * this.mFrameWeightParam + this.mFrameWeight / Time.deltaTime;
		if (this.mAvgFrameRate < 20f && this.mRenderQuality == MainGame.RenderQuality.High)
		{
			Shader.globalMaximumLOD = 1000;
			this.mRenderQuality = MainGame.RenderQuality.Low;
		}
		else if (this.mAvgFrameRate > 25f && this.mRenderQuality == MainGame.RenderQuality.Low)
		{
			Shader.globalMaximumLOD = 2000;
			this.mRenderQuality = MainGame.RenderQuality.High;
		}
	}

	public void ShowStepServerTimeWaiting(int sceond)
	{
		this.m_StepServerTimesceond = sceond;
		if (base.IsInvoking("ShowMessgae_StepServer"))
		{
			base.CancelInvoke("ShowMessgae_StepServer");
		}
		Super.ShowNetWaiting(null);
		this.ShowMessgae_StepServer();
		base.InvokeRepeating("ShowMessgae_StepServer", 1f, 1f);
	}

	private void ShowMessgae_StepServer()
	{
		string text = string.Format(Global.GetLang("跨服时间同步中: {0} 秒"), this.m_StepServerTimesceond--);
		if (null == this.m_GChildWindow_KuaFu)
		{
			this.m_GChildWindow_KuaFu = Super.ShowMessageBox(Global.MainStage, 0, Global.GetLang("提示"), text, -1, -1, -1, -1, 0.7, Vector3.zero, delegate(object s0, MouseEvent e0)
			{
				GameInstance.Game.InitPlayGame();
			}, Global.GetLang("确定"));
			MyMessageBoxPart component = this.m_GChildWindow_KuaFu.Body.getChildAt(0).GetComponent<MyMessageBoxPart>();
			if (null != component)
			{
				NGUITools.SetActive(component.OkBtn, false);
			}
		}
		else
		{
			MyMessageBoxPart component2 = this.m_GChildWindow_KuaFu.Body.getChildAt(0).GetComponent<MyMessageBoxPart>();
			if (null != component2)
			{
				component2.HintText = text;
			}
		}
		if (0 > this.m_StepServerTimesceond)
		{
			Super.HideNetWaiting();
			MyMessageBoxPart component3 = this.m_GChildWindow_KuaFu.Body.getChildAt(0).GetComponent<MyMessageBoxPart>();
			if (null != component3)
			{
				component3.ButtonClick.Invoke(component3, EventArgs.Empty);
				GameInstance.Game.InitPlayGame();
			}
			base.CancelInvoke("ShowMessgae_StepServer");
		}
	}

	public IEnumerator LoadShareBundle()
	{
		string url = PathUtils.WebPath("shareshader.unity3d");
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				www.error
			});
			yield break;
		}
		this.shareBundle = www.assetBundle;
		this.sharedShaders = this.shareBundle.LoadAllAssets<Shader>();
		yield break;
	}

	public IEnumerator LoadEffectShareBundle()
	{
		string url = PathUtils.WebPath("effectshare.unity3d");
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			this.shareEffectBundle = www.assetBundle;
		}
		if (this.shareEffectBundle != null)
		{
			Object[] assets = this.shareEffectBundle.LoadAllAssets();
			MuAssetManager.Instance.AddPermenentSharedAssets(assets);
		}
		yield break;
	}

	public StageSL Stage;

	public GameObject ModalLayer;

	public GameObject DialogLayer;

	public GameObject NetWaiting;

	public UIJoystick Joystick;

	public NetAudioSource BackgroundAudio4UI;

	public AudioListener AudioListener4UI;

	public NetAudioSource BackgroundAudio43D;

	public AudioListener AudioListener43D;

	public Camera MainCamera;

	public Camera UICamera;

	public Camera DecorationUICamera;

	public Light DirectLight;

	public NetAudioSource GlobalUIAudioSource;

	public GameObject UMeng;

	private bool InitOK;

	public static int ticks;

	private int rotMatrixID;

	private int roughnessLUTID;

	private Texture2D roughnessLUT;

	private AssetBundle shareBundle;

	private AssetBundle shareEffectBundle;

	private int exitCount;

	private GChildWindow messageBoxWindow;

	private long frontTicks = DateTime.Now.Ticks;

	private bool isLowPower;

	private bool preHideOtherRoles;

	private bool preCloseMusic;

	private int originHeight;

	private int originWidth;

	private float times;

	public static MainGame _current;

	private List<Action> _actions = new List<Action>();

	private List<MainGame.DelayedQueueItem> _delayed = new List<MainGame.DelayedQueueItem>();

	private List<MainGame.DelayedQueueItem> _currentDelayed = new List<MainGame.DelayedQueueItem>();

	private List<Action> _currentActions = new List<Action>();

	private float CheckNetworkRate = 0.2f;

	private float CheckValue;

	private MainGame.RenderQuality mRenderQuality;

	private float mAvgFrameRate = 45f;

	private float mFrameWeight = 0.0166666675f;

	private float mFrameWeightParam = 0.983333349f;

	private int m_StepServerTimesceond = 20;

	private GChildWindow m_GChildWindow_KuaFu;

	private Shader[] sharedShaders;

	public struct DelayedQueueItem
	{
		public float time;

		public Action action;
	}

	private enum RenderQuality
	{
		High,
		Low
	}
}
