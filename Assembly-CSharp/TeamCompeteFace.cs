using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TeamCompeteFace
{
	public static TeamCompeteFace GetInstance()
	{
		if (TeamCompeteFace.instance == null)
		{
			TeamCompeteFace.instance = new TeamCompeteFace();
		}
		return TeamCompeteFace.instance;
	}

	public T GetUIRef<T>() where T : UserControl
	{
		string text = typeof(T).ToString();
		UserControl userControl = null;
		if (this.DictPartUI.TryGetValue(text, ref userControl) && this.DictWindowUI.ContainsKey(text) && userControl != null)
		{
			return userControl as T;
		}
		MUDebug.LogError<string>(new string[]
		{
			"未打开该界面 " + text
		});
		return (T)((object)null);
	}

	public T OpenUI<T>(bool isShowModal = false) where T : UserControl
	{
		string text = typeof(T).ToString();
		GChildWindow gchildWindow = null;
		UserControl userControl = null;
		if (this.DictWindowUI.TryGetValue(text, ref gchildWindow) && this.DictPartUI.TryGetValue(text, ref userControl))
		{
			if (gchildWindow != null && userControl != null)
			{
				return userControl as T;
			}
			gchildWindow = null;
			userControl = null;
		}
		if (gchildWindow == null)
		{
			gchildWindow = U3DUtils.NEW<GChildWindow>();
			gchildWindow.Modal = true;
			gchildWindow.IsShowModal = isShowModal;
			Super.InitChildWindow(gchildWindow, text);
			Super.GData.GlobalPlayZone.Children.Add(gchildWindow);
		}
		if (userControl == null)
		{
			userControl = U3DUtils.NEW<T>();
			gchildWindow.Body.Add(userControl);
		}
		this.DictWindowUI[text] = gchildWindow;
		this.DictPartUI[text] = userControl;
		return userControl as T;
	}

	public void CloseUI(Type type)
	{
		string text = type.ToString();
		GChildWindow gchildWindow = null;
		UserControl userControl = null;
		if (this.DictWindowUI.TryGetValue(text, ref gchildWindow))
		{
			Super.CloseChildWindow(gchildWindow.Children, gchildWindow);
			Super.GData.GlobalPlayZone.Children.Remove(gchildWindow, true);
			this.DictWindowUI.Remove(text);
		}
		if (this.DictPartUI.TryGetValue(text, ref userControl))
		{
			userControl.transform.parent = null;
			Object.Destroy(userControl.gameObject);
			this.DictPartUI.Remove(text);
		}
	}

	public void Clear()
	{
		foreach (KeyValuePair<string, GChildWindow> keyValuePair in this.DictWindowUI)
		{
			GChildWindow value = keyValuePair.Value;
			Dictionary<string, GChildWindow>.Enumerator enumerator;
			KeyValuePair<string, GChildWindow> keyValuePair2 = enumerator.Current;
			string key = keyValuePair2.Key;
			Super.CloseChildWindow(value.Children, value);
			Super.GData.GlobalPlayZone.Children.Remove(value, true);
			this.DictWindowUI.Remove(key);
		}
		foreach (KeyValuePair<string, UserControl> keyValuePair3 in this.DictPartUI)
		{
			UserControl value2 = keyValuePair3.Value;
			Dictionary<string, UserControl>.Enumerator enumerator2;
			KeyValuePair<string, UserControl> keyValuePair4 = enumerator2.Current;
			string key2 = keyValuePair4.Key;
			value2.transform.parent = null;
			Object.Destroy(value2.gameObject);
			this.DictPartUI.Remove(key2);
		}
		this.DictPartUI.Clear();
		this.DictWindowUI.Clear();
	}

	private static TeamCompeteFace instance;

	private Dictionary<string, UserControl> DictPartUI = new Dictionary<string, UserControl>();

	private Dictionary<string, GChildWindow> DictWindowUI = new Dictionary<string, GChildWindow>();
}
