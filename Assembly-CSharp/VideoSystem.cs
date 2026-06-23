using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class VideoSystem
{
	private VideoSystem()
	{
	}

	public static VideoSystem GetInstance()
	{
		if (VideoSystem._instance == null)
		{
			VideoSystem._instance = new VideoSystem();
		}
		return VideoSystem._instance;
	}

	public void ListenerVideoOpen(int value)
	{
		if (Global.Data != null && Global.Data.GameRadarMap != null)
		{
			if (value == 1 && this.IsViedoSystemOpen())
			{
				Global.Data.GameRadarMap.btnMomo.gameObject.SetActive(true);
			}
			else
			{
				Global.Data.GameRadarMap.btnMomo.gameObject.SetActive(false);
			}
		}
	}

	public bool IsActive()
	{
		return this.IsViedoSystemOpen() && this.ShouldShowIcon();
	}

	private bool ShouldShowIcon()
	{
		return false;
	}

	private bool IsViedoSystemOpen()
	{
		string name = string.Empty;
		switch (Global.DevicePlatform())
		{
		case AppPlatform.Default:
		case AppPlatform.Android:
			name = "AndroidViedo";
			break;
		case AppPlatform.IOS:
			name = "APPViedo";
			break;
		case AppPlatform.IOS_Jailbreak:
			name = "YueYuViedo";
			break;
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(name, '|');
		return systemParamStringArrayByName.Length != 0 && systemParamStringArrayByName[0].Equals("1");
	}

	public void CloseVideoView()
	{
	}

	public void LoginVideoSDK(string roomid, string psw, string filter)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("userId", VideoSystem.GetUserName().ToString());
		dictionary.Add("nickname", VideoSystem.GetNickName());
		dictionary.Add("filter", filter);
		dictionary.Add("channelId", VideoSystem.ChannelId().ToString());
		string text = MUJson.jsonEncode(dictionary);
		PlatSDKMgr.UnityPlayerActivity.Call("CallVideoLogin", new object[]
		{
			text,
			roomid,
			psw
		});
	}

	public void QJ_ListenerBackSpace()
	{
		PlatSDKMgr.UnityPlayerActivity.Call("MonitorBackSpace", new object[0]);
	}

	public void QJ_ListenerInput(string msg)
	{
		PlatSDKMgr.UnityPlayerActivity.Call("MonitorInputEvent", new object[]
		{
			msg
		});
	}

	public static int GetUserName()
	{
		int roleID = Global.Data.roleData.RoleID;
		return roleID ^ 1941226966;
	}

	public static string GetNickName()
	{
		return string.Format(Global.GetLang("[{0}区]{1}"), Global.Data.GameServerID, Global.Data.roleData.RoleName);
	}

	public static int ChannelId()
	{
		if (PlatSDKMgr.PlatName == "YYB")
		{
			return 4;
		}
		return 1;
	}

	private static VideoSystem _instance;
}
