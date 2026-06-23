using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class TaskBoxItem : UserControl
{
	public TaskBoxItem(double width)
	{
		this.Width = width;
		this.Container.Width = width;
		this._TaskDesc = new GTextBlockEx(string.Empty, -1, -1, 0, -1, -1);
		this._TaskDesc.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 44, 163, 190));
		this._TaskDesc.TextWidth = width;
		this._TaskDesc.TextWrapping = TextWrapping.Wrap;
		this._TaskDesc.TextWidth = width;
		Canvas.SetLeft(this._TaskDesc, 0);
		Canvas.SetTop(this._TaskDesc, 0);
		this.Container.Children.Add(this._TaskDesc);
		this._TaskTip = new GTextBlockEx(string.Empty, -1, -1, 0, -1, -1);
		this._TaskTip.TextColor = new SolidColorBrush(uint.MaxValue);
		this._TaskTip.TextWidth = width;
		this._TaskTip.TextWrapping = TextWrapping.Wrap;
		this._TaskTip.TextWidth = width;
		Canvas.SetLeft(this._TaskTip, 0);
		Canvas.SetTop(this._TaskTip, 0);
		this.Container.Children.Add(this._TaskTip);
		this._TaskTarget1 = new GTextBlockEx(string.Empty, -1, -1, 0, -1, -1);
		this._TaskTarget1.TextColor = new SolidColorBrush(uint.MaxValue);
		this._TaskTarget1.TextWidth = width;
		this._TaskTarget1.TextWrapping = TextWrapping.Wrap;
		this._TaskTarget1.TextWidth = width;
		Canvas.SetLeft(this._TaskTarget1, 0);
		Canvas.SetTop(this._TaskTarget1, 0);
		this.Container.Children.Add(this._TaskTarget1);
		this._TaskTarget2 = new GTextBlockEx(string.Empty, -1, -1, 0, -1, -1);
		this._TaskTarget2.TextColor = new SolidColorBrush(uint.MaxValue);
		this._TaskTarget2.TextWidth = width;
		this._TaskTarget2.TextWrapping = TextWrapping.Wrap;
		this._TaskTarget2.TextWidth = width;
		Canvas.SetLeft(this._TaskTarget2, 0);
		Canvas.SetTop(this._TaskTarget2, 0);
		this.Container.Children.Add(this._TaskTarget2);
		this._TaskTargetIcon1 = U3DUtils.NEW<GIcon>();
		this._TaskTargetIcon1.Width = 20.0;
		this._TaskTargetIcon1.Height = 18.0;
		this._TaskTargetIcon1.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/taskchuansong.png"));
		this._TaskTargetIcon1.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/taskchuansong.png"));
		this._TaskTargetIcon1.TipType = 0;
		this._TaskTargetIcon1.Tip = Global.GetLang("每日免费30次\nVIP可无限免费传送");
		Canvas.SetLeft(this._TaskTargetIcon1, 0);
		Canvas.SetTop(this._TaskTargetIcon1, 0);
		this.Container.Children.Add(this._TaskTargetIcon1);
		this._TaskTargetIcon1.Visibility = false;
		this._TaskTargetIcon1.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonDown);
		this._TaskTargetIcon1.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
		this.ResetLayout();
	}

	public int TaskID
	{
		get
		{
			return this._TaskID;
		}
		set
		{
			this._TaskID = value;
		}
	}

	public int TaskLimitLevel
	{
		get
		{
			return this._TaskLimitLevel;
		}
		set
		{
			this._TaskLimitLevel = value;
		}
	}

	public GTextBlockEx TaskDesc
	{
		get
		{
			return this._TaskDesc;
		}
	}

	public GTextBlockEx TaskTip
	{
		get
		{
			return this._TaskTip;
		}
	}

	public GTextBlockEx TaskTarget1
	{
		get
		{
			return this._TaskTarget1;
		}
	}

	public GTextBlockEx TaskTarget2
	{
		get
		{
			return this._TaskTarget2;
		}
	}

	public double RealHeight
	{
		get
		{
			return this._RealHeight;
		}
	}

	public string TaskDescText
	{
		get
		{
			return this._TaskDesc.Text;
		}
		set
		{
			this._TaskDesc.Text = value;
			this.ResetLayout();
		}
	}

	public bool TaskDescIconVisible
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public string TaskTipText
	{
		get
		{
			return this._TaskTip.Text;
		}
		set
		{
			this._TaskTip.Text = value;
			this.ResetLayout();
		}
	}

	public string TaskTarget1Text
	{
		get
		{
			return this._TaskTarget1.Text;
		}
		set
		{
			this._TaskTarget1.Text = value;
			this.ResetLayout();
		}
	}

	public string TaskTarget1IconTag
	{
		get
		{
			return string.Empty;
		}
		set
		{
			this._TaskTargetIcon1.Tag = value;
			if (!string.IsNullOrEmpty(value))
			{
				this._TaskTargetIcon1.Visibility = true;
				Canvas.SetLeft(this._TaskTargetIcon1, this._TaskTarget1.RealSize.Width + 10.0);
				Canvas.SetTop(this._TaskTargetIcon1, Canvas.GetTop(this._TaskTarget1) - 5.0);
			}
			else
			{
				this._TaskTargetIcon1.Visibility = false;
			}
		}
	}

	public string TaskTarget2Text
	{
		get
		{
			return this._TaskTarget2.Text;
		}
		set
		{
			this._TaskTarget2.Text = value;
			this.ResetLayout();
		}
	}

	public string TaskTarget2IconTag
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	private void ResetLayout()
	{
		double num = 0.0;
		Canvas.SetLeft(this._TaskDesc, 0);
		Canvas.SetTop(this._TaskDesc, num);
		num += this._TaskDesc.RealSize.Height;
		Canvas.SetLeft(this._TaskTip, 10);
		Canvas.SetTop(this._TaskTip, num);
		num += this._TaskTip.RealSize.Height;
		Canvas.SetLeft(this._TaskTarget1, 10);
		Canvas.SetTop(this._TaskTarget1, num);
		num += this._TaskTarget1.RealSize.Height;
		Canvas.SetLeft(this._TaskTarget2, 10);
		Canvas.SetTop(this._TaskTarget2, num);
		num += this._TaskTarget2.RealSize.Height;
		this._RealHeight = num;
	}

	private void IconMouseLeftButtonDown(object sender, MouseEvent e)
	{
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (!((sender as GIcon).Tag is string))
		{
			return;
		}
		string[] array = ((sender as GIcon).Tag as string).Split(new char[]
		{
			','
		});
		if (array.Length == 6)
		{
			this.TansportPoint(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]), Global.SafeConvertToInt32(array[2]), Global.SafeConvertToInt32(array[3]), -1, -1, true);
		}
	}

	public bool TansportPoint(int type, int taskID, int npcID, int mapCode, int toPosX, int toPosY, bool forceTansport = false)
	{
		Global.Data.TargetNpcID = npcID;
		Point pos;
		if (type == 2)
		{
			pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else if (type == 3)
		{
			pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(toPosX, toPosY);
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		if (type == 2)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
		}
		else if (type == 3)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		if (forceTansport && Super.CanTransport(mapCode, true, false))
		{
			GameInstance.Game.SpriteTaskTransport(mapCode, pos.X, pos.Y, 0);
		}
		return true;
	}

	private void DescIconMouseLeftButtonDown(object sender, MouseEvent e)
	{
	}

	private void DescIconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		GameInstance.Game.SpriteQuickCompleteTaskByID(Global.Data.roleData.RoleID, this._TaskID);
	}

	public bool ShowHintDeco
	{
		set
		{
		}
	}

	private Loader initSwfLoader()
	{
		return new Loader();
	}

	private void onSwfLoaded(Event e)
	{
	}

	public void HideHintFrameTarget1()
	{
		if (this._HintFrameTarget1 != null)
		{
			this._HintFrameTarget1 = null;
		}
	}

	public void HideHintFrameTarget2()
	{
		if (this._HintFrameTarget2 != null)
		{
			this._HintFrameTarget2 = null;
		}
	}

	public void TaskTarget1SetSpecialText(string text, SolidColorBrush brush, bool underLine, object tag = null, bool rendText = true)
	{
	}

	public void TaskTarget2SetSpecialText(string text, SolidColorBrush brush, bool underLine, object tag = null, bool rendText = true)
	{
	}

	public void TaskDescSetSpecialText(string text, SolidColorBrush brush, bool underLine, object tag = null, bool rendText = true)
	{
		this._TaskDesc.SetSpecialText(text, brush, underLine, tag, rendText);
	}

	public void TaskTipSetSpecialText(string text, SolidColorBrush brush, bool underLine, object tag = null, bool rendText = true)
	{
		this._TaskTip.SetSpecialText(text, brush, underLine, tag, rendText);
	}

	private GTextBlockEx _TaskDesc;

	private GTextBlockEx _TaskTip;

	private GTextBlockEx _TaskTarget1;

	private GTextBlockEx _TaskTarget2;

	private GIcon _TaskTargetIcon1;

	private double _RealHeight;

	private Loader _HintFrameTarget1;

	private Loader _HintFrameTarget2;

	private int _TaskID;

	private int _TaskLimitLevel;
}
