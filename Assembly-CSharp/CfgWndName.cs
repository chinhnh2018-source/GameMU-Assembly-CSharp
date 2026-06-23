using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class CfgWndName
{
	public static CfgWnd CreateWnd(string wndName, UserControl parentWnd = null, int left = -1)
	{
		int num = Math.Max((int)((Global.GlobalMainWindow.ActualWidth - 668.0) / 2.0), 0);
		int num2 = Math.Max((int)(Global.GlobalMainWindow.ActualWidth - 300.0), 0);
		if (CfgWndName.IsIntroduceWindow(wndName))
		{
		}
		if (left >= 0)
		{
		}
		CfgWnd cfgWnd = CfgSingleWndMgr.Instance().Get(wndName) as CfgWnd;
		if (null == cfgWnd)
		{
			if (null == parentWnd)
			{
				UserControl globalPlayZone = Super.GData.GlobalPlayZone;
			}
			cfgWnd = U3DUtils.NEW<CfgWnd>();
			CfgSingleWndMgr.Instance().Add(wndName, cfgWnd);
		}
		return cfgWnd;
	}

	public static int GetCfgWndIDByName(string cfgWndName)
	{
		string[] array = cfgWndName.Split(new char[]
		{
			'_'
		});
		if (array.Length == 2)
		{
			return Global.SafeConvertToInt32(array[1]);
		}
		return -1;
	}

	public static string GetCfgWndNameByID(int id)
	{
		return StringUtil.substitute("CfgWnd_{0}", new object[]
		{
			id
		});
	}

	public static bool IsIntroduceWindow(string wndName)
	{
		string[] array = wndName.Split(new char[]
		{
			'_'
		});
		return array.Length == 2 && array[1].SafeToInt32(0) % 2 == 0;
	}
}
