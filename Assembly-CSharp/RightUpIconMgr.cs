using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class RightUpIconMgr
{
	public bool show { get; set; }

	public bool IsShowAll
	{
		get
		{
			return this._IsShowAll;
		}
		set
		{
			if (!this._IsShowAll && this.show)
			{
				this.ShowAll();
			}
			else if (this._IsShowAll && !this.show)
			{
				this.HideAll();
			}
			this._IsShowAll = this.show;
		}
	}

	public static RightUpIconMgr Instance()
	{
		return RightUpIconMgr.g_RightUpIconMgr;
	}

	public void AddAtPosition(SpriteSL smallBtn, int index)
	{
		if (index >= 0 && null != smallBtn && this.WndArr.IndexOf(smallBtn) < 0)
		{
			if (index >= this.WndArr.Count)
			{
				index = this.WndArr.Count;
			}
			this.RepositionAll();
		}
	}

	public bool ContainWnd(SpriteSL smallBtn)
	{
		return this.WndArr.IndexOf(smallBtn) >= 0;
	}

	public void Add(SpriteSL smallBtn)
	{
		if (null != smallBtn && this.WndArr.IndexOf(smallBtn) < 0)
		{
			this.WndArr.Add(smallBtn);
			this.RepositionAll();
		}
	}

	public void Remove(SpriteSL smallBtn)
	{
		if (null != smallBtn)
		{
			int num = this.WndArr.IndexOf(smallBtn);
			if (num >= 0)
			{
				this.WndArr.RemoveRange(num, 1);
				this.RepositionAll();
			}
		}
	}

	public void RepositionAll()
	{
		int num = (int)Global.GlobalMainWindow.ActualWidth - 304;
		int num2 = 10;
		int num3 = 55;
		int num4 = 59;
		int num5 = 5;
		int num6 = 0;
		for (int i = 0; i < this.WndArr.Count; i++)
		{
			SpriteSL spriteSL = this.WndArr[i];
			if (null != spriteSL && spriteSL.Visibility)
			{
				Canvas.SetLeft(spriteSL, num - num6 % num5 * num3);
				Canvas.SetTop(spriteSL, (double)num2 + Math.Floor((double)num6 / (double)num5) * (double)num4);
				num6++;
			}
		}
	}

	public void ShowAll()
	{
		for (int i = 0; i < this.WndArr.Count; i++)
		{
			SpriteSL spriteSL = this.WndArr[i];
			if (null != spriteSL)
			{
				spriteSL.Visibility = true;
			}
		}
	}

	public void HideAll()
	{
		for (int i = 0; i < this.WndArr.Count; i++)
		{
			SpriteSL spriteSL = this.WndArr[i];
			if (null != spriteSL)
			{
				spriteSL.Visibility = false;
			}
		}
	}

	private List<SpriteSL> WndArr = new List<SpriteSL>();

	private static RightUpIconMgr g_RightUpIconMgr = U3DUtils.NEW<RightUpIconMgr>();

	private bool _IsShowAll = true;
}
