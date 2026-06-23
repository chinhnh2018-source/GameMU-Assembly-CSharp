using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using UnityEngine;

public class PlatSDKEventListening : MonoBehaviour
{
	private void Start()
	{
		PlatSDKEventListening.SingleInstance = this;
		this.StartCpuMemInfo();
		QMQJJava.GetDeviceID();
	}

	public static void ChangeAccountButtonWaiting()
	{
		Super.ShowNetWaiting(null);
		PlatSDKEventListening.m_canchange = false;
		PlatSDKEventListening.m_startchangeacc = 1f;
	}

	private void Update()
	{
		if (!PlatSDKEventListening.m_canchange)
		{
			if (PlatSDKEventListening.m_startchangeacc > 0f)
			{
				PlatSDKEventListening.m_startchangeacc -= Time.deltaTime;
			}
			if (PlatSDKEventListening.m_startchangeacc <= 0f)
			{
				Super.HideNetWaiting();
				PlatSDKEventListening.m_canchange = true;
			}
		}
	}

	private void OnDestroy()
	{
	}

	public void StartCpuMemInfo()
	{
		QMQJJava.UploadCpuMem();
		base.CancelInvoke("Dostartcpumemloginfo");
		base.Invoke("Dostartcpumemloginfo", 10f);
	}

	private void Dostartcpumemloginfo()
	{
		QMQJJava.Cpumemcrash();
	}

	public void SendApplist()
	{
		CheckSFAndWorker.Dogpl();
	}

	public void StartLevelCDKey()
	{
		if (PlatSDKMgr._hasEntergame)
		{
			return;
		}
		if (PlatSDKMgr.PlatName != "YYB")
		{
			return;
		}
		PlatSDKMgr.LevelCDKey("login");
		base.CancelInvoke("DoLevelCDKeyOfLevel");
		base.InvokeRepeating("DoLevelCDKeyOfLevel", 300f, 300f);
	}

	private void DoLevelCDKeyOfLevel()
	{
		PlatSDKMgr.LevelCDKey("level");
	}

	private void ShowTencentRollNotice()
	{
		PlatSDKMgr.TencentNotice(0);
		base.Invoke("StopTencentRollNotice", 15f);
	}

	private void StopTencentRollNotice()
	{
		PlatSDKMgr.TencentHideNoticeRoll();
	}

	public void InitCallback()
	{
	}

	public void LoginCallback(string msg)
	{
		PlatSDKMgr.ParseLoginMsgFromSDK(msg);
		LocalPushManager.Instance.LoadConfigAndSetPush();
		CheckSFAndWorker.SingleInstance.Init();
	}

	public void LoginOutCallback()
	{
		Global.RootParams["uid"] = "-1";
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.CloseFirstWorldPart();
		}
		if (PlayZone.GlobalPlayZone == null)
		{
			Super.DestroyRoleManagerAndBack();
		}
		else if (null != Super.CurrentLoadingMap)
		{
			Super.DestroyLoadingMap();
			Super.BackToLogin();
		}
		PlayZone.GlobalPlayZone.ReLogin();
		if (PlatSDKMgr.PlatName == "YK")
		{
			PlatSDKMgr.Login(null, string.Empty);
		}
	}

	public void PayCallback(string msg)
	{
		string platName = PlatSDKMgr.PlatName;
		int num;
		if (platName != null)
		{
			if (PlatSDKEventListening.<>f__switch$mapF == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("QQ", 0);
				dictionary.Add("YYB", 0);
				PlatSDKEventListening.<>f__switch$mapF = dictionary;
			}
			if (PlatSDKEventListening.<>f__switch$mapF.TryGetValue(platName, ref num))
			{
				if (num == 0)
				{
					PlatSDKMgr.OnTencentPayCallback(msg, false, false);
				}
			}
		}
		platName = PlatSDKMgr.PlatName;
		if (platName != null)
		{
			if (PlatSDKEventListening.<>f__switch$map10 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("GATGoogle", 0);
				dictionary.Add("GATAn", 0);
				dictionary.Add("Google", 1);
				dictionary.Add("Tstore", 1);
				PlatSDKEventListening.<>f__switch$map10 = dictionary;
			}
			if (PlatSDKEventListening.<>f__switch$map10.TryGetValue(platName, ref num))
			{
				if (num != 0)
				{
					if (num != 1)
					{
					}
				}
			}
		}
	}

	public void CommonCallback(string msg)
	{
		if (PlatSDKMgr.PlatName == "QH360")
		{
			int num = Global.SafeConvertToInt32(msg);
			GameInstance.Game.CurrentSession.UserIsAdult = num;
			GameInstance.Game.SpriteUpdateTengXunFcmRate((double)num);
		}
	}

	public void VideoIconCallback(string msg)
	{
		if (msg.StartsWith("0"))
		{
			if (!Global.IsWatchingVedio)
			{
				Global.IsWatchingVedio = true;
				this.CloseMusicAndAudio();
				MUDebug.LogError<string>(new string[]
				{
					"关闭音乐，亲加视频==="
				});
			}
		}
		else
		{
			Global.IsWatchingVedio = false;
			this.OpenMusicAndAudio();
			MUDebug.LogError<string>(new string[]
			{
				"开启音乐，亲加视频==="
			});
		}
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.ChangeWatchVedioIcon(Global.IsWatchingVedio);
		}
	}

	public void ScreenMaskCallback(string msg)
	{
		MUDebug.LogError<string>(new string[]
		{
			"ScreenMaskCallback msg=" + msg
		});
		if (msg.StartsWith("0"))
		{
			QJVideoMaskPart.ShowQJVedioMask();
			QJVideoMaskPart.mCurQJVideoWindowState = QJVideoMaskPart.QJVideoWindowState.Close;
		}
		else if (msg.StartsWith("1"))
		{
			QJVideoMaskPart.ShowQJVedioMask();
			QJVideoMaskPart.mCurQJVideoWindowState = QJVideoMaskPart.QJVideoWindowState.Half;
		}
		else if (msg.StartsWith("2"))
		{
			QJVideoMaskPart.ShowQJVedioMask();
			QJVideoMaskPart.mCurQJVideoWindowState = QJVideoMaskPart.QJVideoWindowState.Full;
		}
	}

	private void CloseMusicAndAudio()
	{
		this.CloseGameMusic = Global.Data.SysSetting.CloseGameMusic;
		this.CloseGameAudio = Global.Data.SysSetting.CloseGameAudio;
		MUDebug.LogError<string>(new string[]
		{
			"CloseGameMusic " + this.CloseGameMusic
		});
		MUDebug.LogError<string>(new string[]
		{
			"CloseGameMusic " + this.CloseGameAudio
		});
		if (!this.CloseGameMusic)
		{
			Global.Data.SysSetting.CloseGameMusic = true;
			Global.BackgroundAudio43D.StopPlay();
		}
		if (!this.CloseGameAudio)
		{
			Global.Data.SysSetting.CloseGameAudio = true;
		}
	}

	private void OpenMusicAndAudio()
	{
		MUDebug.LogError<string>(new string[]
		{
			" Open CloseGameMusic " + this.CloseGameMusic
		});
		MUDebug.LogError<string>(new string[]
		{
			" Open CloseGameMusic " + this.CloseGameAudio
		});
		if (!this.CloseGameMusic)
		{
			Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, false), true, false);
			Global.Data.SysSetting.CloseGameMusic = false;
		}
		if (!this.CloseGameAudio)
		{
			Global.Data.SysSetting.CloseGameAudio = false;
		}
	}

	public void InitFinish(string msg)
	{
	}

	public void OnWXShareCallback(string msg)
	{
		GameInstance.Game.UpdateShareStat();
	}

	public void OnCheckTTSCallback(string result)
	{
		bool isTtsEnabled = result == "true";
		Global.Data.IsTtsEnabled = isTtsEnabled;
	}

	public void OnStartTTSCallback(string resultStr)
	{
		QMQJJava.StartCheckTTSResult(resultStr);
	}

	public void LogCallback(string msg)
	{
	}

	public void MemoryWarning()
	{
		MuAssetManager.Instance.UnLoadAllUnuseResource();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void ProductInfoCallback(string productInfo)
	{
		AppsIAPPlugin.RequestOrderInfo(productInfo);
	}

	public void LoginfoCallback(string msg)
	{
		AppsIAPPlugin.PostLogInfo(msg);
	}

	public void PayResult(string resultCode)
	{
		AppsIAPPlugin.PostPayResultInfo(resultCode);
	}

	public void ThisIsBackWindow(string msg)
	{
	}

	public void YouAreAsshole(string msg)
	{
	}

	public void FuckCallback(string msg)
	{
		CheckSFAndWorker.GetApplicationList(msg);
	}

	public void RadioCallback(string msg)
	{
		if (PlayZone.GlobalPlayZone.GameChatBox != null)
		{
			PlayZone.GlobalPlayZone.GameChatBox.VioceToWord(msg);
		}
	}

	public void RPStartRecording(string isRecording)
	{
		bool isRecording2 = isRecording == "1";
		PlayZone.GlobalPlayZone.ResetGRadarMapRecordBtn(isRecording2);
	}

	public void SetLiveBtnOn(string isLiveOn)
	{
		bool liveOn = isLiveOn == "1";
		PlayZone.GlobalPlayZone.ResetLiveBtn(liveOn);
	}

	public void RegistReportCallback(string uid)
	{
		PlatSDKMgr.RegistReport(uid, false);
	}

	public void VisitorRegistReportCallback(string uid)
	{
		PlatSDKMgr.RegistReport(uid, true);
	}

	public void LiveOn()
	{
	}

	public void GameCenterCallback(string msg)
	{
		MUDebug.Log<string>(new string[]
		{
			"111111GameCenterCallback msg " + msg
		});
		if (msg.Contains("1"))
		{
			Global.IsOpenGameFromGameCenter = true;
		}
		else if (msg.Contains("0"))
		{
			Global.IsOpenGameFromGameCenter = false;
		}
	}

	public int m_platform;

	private int m_payTicktime = 1200;

	private float m_tempTime;

	private bool m_hasInvokeNotice;

	public static bool m_canchange = true;

	private static float m_startchangeacc;

	private static float m_lastDodododoTime;

	private static float m_unsafestart = -1f;

	public static PlatSDKEventListening SingleInstance;

	public bool CloseGameMusic;

	public bool CloseGameAudio;
}
