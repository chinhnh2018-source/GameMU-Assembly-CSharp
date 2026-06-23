using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;

public class CfgSingleWndMgr
{
	public static CfgSingleWndMgr Instance()
	{
		return CfgSingleWndMgr.g_SingleWndMgr;
	}

	public bool Add(string key, SpriteSL obj)
	{
		if (key == null || StringUtil.isWhitespace(key) || null == obj)
		{
			return false;
		}
		SpriteSL value = this.WindowDict.GetValue(key);
		if (null != value)
		{
			return false;
		}
		this.WindowDict[key] = obj;
		return true;
	}

	public SpriteSL Get(string key)
	{
		if (!this.WindowDict.ContainsKey(key))
		{
			return null;
		}
		return this.WindowDict.GetValue(key);
	}

	public void Remove(string key)
	{
		if (this.WindowDict.ContainsKey(key))
		{
			this.WindowDict.Remove(key);
		}
	}

	public bool ShowWindow(string key, bool tryCreate = false, UserControl parentWnd = null)
	{
		SpriteSL spriteSL = this.Get(key);
		if (null == spriteSL && tryCreate)
		{
			spriteSL = CfgWndName.CreateWnd(key, parentWnd, -1);
		}
		if (null != spriteSL)
		{
			spriteSL.Visibility = true;
			if (RightUpIconMgr.Instance().ContainWnd(spriteSL))
			{
				RightUpIconMgr.Instance().RepositionAll();
			}
			return true;
		}
		return false;
	}

	public bool HideWindow(string key)
	{
		SpriteSL spriteSL = this.Get(key);
		if (null != spriteSL)
		{
			CfgWnd cfgWnd = U3DUtils.NEW<CfgWnd>();
			if (null != cfgWnd)
			{
				if (RightUpIconMgr.Instance().ContainWnd(spriteSL))
				{
					RightUpIconMgr.Instance().Remove(spriteSL);
					RightUpIconMgr.Instance().RepositionAll();
				}
				cfgWnd.Close();
			}
			else
			{
				spriteSL.Visibility = false;
			}
			return true;
		}
		return false;
	}

	public void TryRepositionInterface()
	{
		foreach (KeyValuePair<string, SpriteSL> keyValuePair in this.WindowDict)
		{
			SpriteSL value = keyValuePair.Value;
			CfgBitmapWindow cfgBitmapWindow = U3DUtils.NEW<CfgBitmapWindow>();
			if (null != cfgBitmapWindow)
			{
				cfgBitmapWindow.TryAdjustPosition();
			}
		}
	}

	private static CfgSingleWndMgr g_SingleWndMgr = U3DUtils.NEW<CfgSingleWndMgr>();

	private Dictionary<string, SpriteSL> WindowDict = new Dictionary<string, SpriteSL>();
}
