using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class ServerBufferZhaoHui
{
	public static ServerBufferZhaoHui Instance
	{
		get
		{
			if (ServerBufferZhaoHui._Instance == null)
			{
				ServerBufferZhaoHui._Instance = new ServerBufferZhaoHui();
			}
			return ServerBufferZhaoHui._Instance;
		}
	}

	public static bool IsValid()
	{
		return ServerBufferZhaoHui.Instance.userData != null;
	}

	public EReturnAwardState eReturnAwardState
	{
		get
		{
			return this._eReturnAwardState;
		}
		set
		{
			this._eReturnAwardState = value;
		}
	}

	public int TodayReturnDayCount
	{
		get
		{
			if (this.userData == null)
			{
				return 0;
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime timeReturn = this.userData.TimeReturn;
			int num;
			if (correctDateTime.Year == timeReturn.Year && correctDateTime.Month == timeReturn.Month && correctDateTime.Day == timeReturn.Day)
			{
				num = 1;
			}
			else
			{
				DateTime dateTime;
				dateTime..ctor(timeReturn.Year, timeReturn.Month, timeReturn.Day, 23, 59, 59);
				TimeSpan timeSpan = correctDateTime - dateTime;
				num = 1 + timeSpan.Days;
				if (timeSpan.Hours > 0 || timeSpan.Minutes > 0 || timeSpan.Seconds > 0)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int UnionLevel
	{
		get
		{
			if (this.userData == null)
			{
				return 1;
			}
			return Global.GetUnionLevel(this.userData.ZhuanSheng, this.userData.DengJi);
		}
	}

	public EReturnState ServerReturnState
	{
		get
		{
			if (this.userData == null)
			{
				return 0;
			}
			return this.userData.ReturnState;
		}
	}

	public EReturnState ClientReturnState
	{
		get
		{
			return this._ClientReturnState;
		}
		set
		{
			this._ClientReturnState = value;
		}
	}

	public void SendGetAwardList()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
	}

	public void SendOpenAward(int awardType, int awardID, int num)
	{
		Super.ShowNetWaiting(null);
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.GetReturnAward(awardType, awardID, num);
	}

	public void SendCheckTuiJianRen(string returnID = "0")
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetReturnCheck(returnID);
	}

	public static void ClearStaticData()
	{
		ServerBufferZhaoHui._Instance = null;
	}

	private static ServerBufferZhaoHui _Instance;

	public VerifyPlayerRecallData verifyData;

	public UserReturnData userData;

	public EReturnState eReturnState;

	public EReturnAwardState _eReturnAwardState;

	public EReturnAwardType eReturnAwardType = 1;

	public string eReturnAwardData = string.Empty;

	private EReturnState _ClientReturnState;

	public static string startTime = string.Empty;

	public static string endTime = string.Empty;

	public static int huodongID;
}
