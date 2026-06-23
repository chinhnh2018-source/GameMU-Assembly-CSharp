using System;
using HSGameEngine.GameEngine.Logic;

public class RecordVideoSystem
{
	private RecordVideoSystem()
	{
	}

	public static RecordVideoSystem GetInstance()
	{
		if (RecordVideoSystem._instance == null)
		{
			RecordVideoSystem._instance = new RecordVideoSystem();
		}
		return RecordVideoSystem._instance;
	}

	public void RecordVideoOpen(int value)
	{
		if (Global.Data != null && Global.Data.GameRadarMap != null)
		{
			if (value == 1 && this.IsViedoSystemOpen())
			{
				Global.Data.GameRadarMap.btnRecordVedio.gameObject.SetActive(true);
			}
			else
			{
				Global.Data.GameRadarMap.btnRecordVedio.gameObject.SetActive(false);
			}
		}
	}

	public bool IsActive()
	{
		return this.IsViedoSystemOpen();
	}

	public static bool IsSupportReplay()
	{
		return false;
	}

	public static bool StartRecording()
	{
		return false;
	}

	public static bool StopRecording()
	{
		return false;
	}

	private bool IsViedoSystemOpen()
	{
		if (Context.IsHaiwai)
		{
			return false;
		}
		string name = "IsRecordViedoSystemOpen";
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(name, '|');
		return systemParamStringArrayByName.Length > 0 && systemParamStringArrayByName[0].Equals("1");
	}

	public void CloseVideoView()
	{
		PlatSDKMgr.UnityPlayerActivity.Call("CloseVideoView", new object[0]);
	}

	public static DateTime LastClickDateTime;

	private static RecordVideoSystem _instance;
}
