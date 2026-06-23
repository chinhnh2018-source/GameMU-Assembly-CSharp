using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class WindowManage
{
	public static void AddWindows(MonoBehaviour Win, bool isModel = false, Transform Parent = null)
	{
		float num = -100f;
		WindowManage.SetLast();
		if (WindowManage.WindowList.Count != 0)
		{
			num = WindowManage.GetZ(WindowManage.WindowList[WindowManage.WindowList.Count - 1].WindowObj.transform);
		}
		if (num < -10000f)
		{
			WindowManage.Sort();
			num = WindowManage.GetZ(WindowManage.WindowList[WindowManage.WindowList.Count - 1].WindowObj.transform);
			if (num < -10000f)
			{
				Debug.Log(Global.GetLang("窗口缓冲区深度不足"));
			}
		}
		if (null != Parent)
		{
			U3DUtils.AddChild(Parent.gameObject, Win.gameObject, true);
		}
		Windows_st windows_st;
		windows_st.WindowObj = Win;
		windows_st.isModel = isModel;
		if (WindowManage.WindowList.Count != 0 && WindowManage.WindowList[WindowManage.WindowList.Count - 1].isModel)
		{
			Win.transform.localPosition = new Vector3(Win.transform.localPosition.x, Win.transform.localPosition.y, num - 1200f);
		}
		else
		{
			Win.transform.localPosition = new Vector3(Win.transform.localPosition.x, Win.transform.localPosition.y, num - 4f);
		}
		WindowManage.WindowList.Add(windows_st);
	}

	public static void RemoveWindows(MonoBehaviour Win)
	{
		for (int i = 0; i < WindowManage.WindowList.Count; i++)
		{
			if (null == WindowManage.WindowList[i].WindowObj)
			{
				WindowManage.WindowList.Remove(WindowManage.WindowList[i]);
			}
			if (WindowManage.WindowList[i].WindowObj == Win)
			{
				WindowManage.WindowList.Remove(WindowManage.WindowList[i]);
			}
		}
	}

	public static float GetZ(Transform Win)
	{
		float num = Win.localPosition.z;
		Transform parent = Win.parent;
		while (null != parent)
		{
			num += parent.transform.localPosition.z;
			parent = parent.transform.parent;
		}
		return num;
	}

	public static void Sort()
	{
		float num = 0f;
		float num2 = -104f;
		for (int i = 0; i < WindowManage.WindowList.Count; i++)
		{
			if (null == WindowManage.WindowList[i].WindowObj)
			{
				WindowManage.WindowList.Remove(WindowManage.WindowList[i]);
			}
			else
			{
				WindowManage.WindowList[i].WindowObj.transform.localPosition = new Vector3(WindowManage.WindowList[i].WindowObj.transform.localPosition.x, WindowManage.WindowList[i].WindowObj.transform.localPosition.y, num + num2);
				num = WindowManage.GetZ(WindowManage.WindowList[i].WindowObj.transform);
				if (WindowManage.WindowList[i].isModel)
				{
					num2 -= 1000f;
				}
				else
				{
					num2 -= 4f;
				}
			}
		}
	}

	public static void SetLast()
	{
		for (int i = WindowManage.WindowList.Count - 1; i >= 0; i--)
		{
			if (null != WindowManage.WindowList[i].WindowObj)
			{
				return;
			}
			WindowManage.WindowList.Remove(WindowManage.WindowList[i]);
		}
	}

	public static void ClearAll()
	{
		for (int i = 0; i < WindowManage.WindowList.Count; i++)
		{
			if (null != WindowManage.WindowList[i].WindowObj)
			{
				Object.Destroy(WindowManage.WindowList[i].WindowObj);
			}
		}
		WindowManage.WindowList.Clear();
	}

	public static bool IsHaveWindow()
	{
		return WindowManage.WindowList.Count > 0;
	}

	public static void CloseMostTopChildWindow()
	{
		MUDebug.Log<string>(new string[]
		{
			" 开始CloseMostTopChildWindow()"
		});
		if (PlayZone.GlobalPlayZone.GameFubenBoxMini != null && PlayZone.GlobalPlayZone.GameFubenBoxMini.gameObject.activeInHierarchy)
		{
			MUDebug.Log<string>(new string[]
			{
				"YN Debug:CloseMostTopChildWindow()_enter if()"
			});
			if (PlayZone.GlobalPlayZone.AutoFightingNoDrugDialogBoxWindow != null)
			{
				return;
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.JingJiChang)
			{
				MUDebug.Log<string>(new string[]
				{
					"YN Debug:CloseMostTopChildWindow()_enter if()_else if(JingJiChang)"
				});
				return;
			}
			MUDebug.Log<string>(new string[]
			{
				"YN Debug:CloseMostTopChildWindow()_enter if()_else if(JingJiChang)_else"
			});
			PlayZone.GlobalPlayZone.GameFubenBoxMini.OnLeaveFuBenIconClick(null, null);
			WindowManage.WindowList.Clear();
			return;
		}
		else
		{
			if (!WindowManage.IsHaveWindow())
			{
				MUDebug.Log<string>(new string[]
				{
					"YN Debug:CloseMostTopChildWindow()_enter if(!IsHaveWindow())"
				});
				PlayZone.GlobalPlayZone.ShowSystemSettingWindow(0);
				return;
			}
			MUDebug.Log<string>(new string[]
			{
				"YN Debug:CloseMostTopChildWindow()_enter if(!IsHaveWindow())_else"
			});
			MonoBehaviour windowObj = WindowManage.WindowList[WindowManage.WindowList.Count - 1].WindowObj;
			if (windowObj is GChildWindow)
			{
				(windowObj as GChildWindow).NotifyClose(0);
			}
			return;
		}
	}

	public static List<Windows_st> WindowList = new List<Windows_st>();
}
