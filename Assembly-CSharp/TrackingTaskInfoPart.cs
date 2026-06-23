using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;

public class TrackingTaskInfoPart : UserControl
{
	public TrackingTaskInfoPart()
	{
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	private void InitControls()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 13.0;
		gicon.Height = 13.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/x1.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/x0.png"));
		gicon.Tip = Global.GetLang("关闭");
		gicon.MouseLeftButtonDown = delegate(object sender, MouseEvent e)
		{
		};
		gicon.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.ToClose != null)
			{
				this.StopHeart();
				this.ToClose.Invoke(this.thisCtrl, EventArgs.Empty);
			}
		};
		Canvas.SetLeft(gicon, this.Width - 13.0 - 10.0);
		Canvas.SetTop(gicon, 4);
		this.Container.Children.Add(gicon);
		this.textBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		this.textBlockEx.TextColor = new SolidColorBrush(uint.MaxValue);
		this.textBlockEx.TextWidth = this.Container.Width - 8.0;
		this.textBlockEx.TextHeight = this.Container.Height - 8.0;
		this.textBlockEx.FontSize = 12;
		Canvas.SetLeft(this.textBlockEx, 0);
		Canvas.SetTop(this.textBlockEx, 12);
		this.Container.Children.Add(this.textBlockEx);
		this.textBlockEx.TextClick = new UIEventEventHandler(this.TextClick);
		this.textBlockEx.TextMouseEnter = new UIEventEventHandler(this.TextMouseEnter);
		this.textBlockEx.TextMouseLeave = new UIEventEventHandler(this.TextMouseLeave);
	}

	public bool TextMouseEnter(object sender, BaseEventArgs _e)
	{
		if (!(_e.Tag is SpecialTextItem))
		{
			return false;
		}
		(sender as GTextBlockExItem).Link(new SolidColorBrush(4289014314U));
		return false;
	}

	public bool TextMouseLeave(object sender, BaseEventArgs _e)
	{
		if (!(_e.Tag is SpecialTextItem))
		{
			return false;
		}
		(sender as GTextBlockExItem).Unlink();
		return false;
	}

	public bool TextClick(object sender, BaseEventArgs _e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(_e.Tag is SpecialTextItem))
		{
			return false;
		}
		string text = (_e.Tag as SpecialTextItem).Text;
		string text2 = (_e.Tag as SpecialTextItem).Tag as string;
		if (text2 == null)
		{
			return false;
		}
		string[] array = text2.Split(new char[]
		{
			','
		});
		if (array.Length != 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息错误, 无法寻路: {0}"), new object[]
			{
				text
			}), 0, -1, -1, 0);
			return false;
		}
		int num = Convert.ToInt32(array[0]);
		int mapCode = Convert.ToInt32(array[2]);
		int x = Convert.ToInt32(array[3]);
		int y = Convert.ToInt32(array[4]);
		Global.Data.TargetNpcID = Convert.ToInt32(array[1]);
		Point pos;
		if (num == 1)
		{
			pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else if (num == 0)
		{
			pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(x, y);
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误, 无法自动寻路: {0}"), new object[]
			{
				text
			}), 0, -1, -1, 0);
			return false;
		}
		if (num == 1)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
		}
		else if (num == 0)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		return false;
	}

	public void InitPartSize(int width, string itemTag)
	{
		this.Width = (double)width;
		this.Container.Width = this.Width - 20.0;
		this.InitControls();
		this.InitPartData(itemTag);
		this.Height = this.textBlockEx.RealSize.Height + 42.0 - 20.0;
		this.Container.Height = this.Height;
		Canvas.SetLeft(this.Container, 10);
		Canvas.SetTop(this.Container, 10);
	}

	private void InitPartData(string itemTag)
	{
		if (itemTag == null)
		{
			return;
		}
		string[] array = itemTag.Split(new char[]
		{
			','
		});
		if (array.Length != 4)
		{
			return;
		}
		int num = Convert.ToInt32(array[0]);
		int id = Convert.ToInt32(array[1]);
		int num2 = Convert.ToInt32(array[2]);
		int num3 = Convert.ToInt32(array[3]);
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(id);
		if (taskXmlNodeByID == null)
		{
			return;
		}
		string description = taskXmlNodeByID.Description2;
		this.textBlockEx.Text = description + "\n";
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		MonsterVO monsterVO = null;
		NPCInfoVO npcinfoVO = null;
		if (taskXmlNodeByID.SourceNPC >= 0)
		{
			npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.SourceNPC);
			if (npcinfoVO != null)
			{
				text = npcinfoVO.SName;
				if (string.Empty != text)
				{
					string tag = StringUtil.substitute("0,{0},{1},-1,-1", new object[]
					{
						npcinfoVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text, new SolidColorBrush(4294967040U), true, tag, true);
				}
			}
		}
		if (taskXmlNodeByID.DestNPC >= 0)
		{
			npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.DestNPC);
			if (npcinfoVO != null)
			{
				text2 = npcinfoVO.SName;
				if (string.Empty != text2)
				{
					string tag2 = StringUtil.substitute("0,{0},{1},-1,-1", new object[]
					{
						npcinfoVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text2, new SolidColorBrush(4294967040U), true, tag2, true);
				}
			}
		}
		bool flag = true;
		if (taskXmlNodeByID.TargetNPC1 >= 0)
		{
			int targetType = taskXmlNodeByID.TargetType1;
			if (targetType == 1 || targetType == 2 || targetType == 8)
			{
				flag = false;
				monsterVO = ConfigMonsters.GetMonsterXmlNodeByID(taskXmlNodeByID.TargetNPC1);
			}
			else if (targetType != 4)
			{
				npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.TargetNPC1);
			}
			if (monsterVO != null)
			{
				text3 = monsterVO.SName;
				if (string.Empty != text3)
				{
					string tag3 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
					{
						(!flag) ? 1 : 0,
						monsterVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(monsterVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text3, new SolidColorBrush(4294967040U), true, tag3, true);
				}
			}
			if (npcinfoVO != null)
			{
				text3 = npcinfoVO.SName;
				if (string.Empty != text3)
				{
					string tag4 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
					{
						(!flag) ? 1 : 0,
						npcinfoVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text3, new SolidColorBrush(4294967040U), true, tag4, true);
				}
			}
		}
		flag = true;
		if (taskXmlNodeByID.TargetNPC2 >= 0)
		{
			int targetType2 = taskXmlNodeByID.TargetType2;
			if (targetType2 == 1 || targetType2 == 2 || targetType2 == 8)
			{
				flag = false;
				monsterVO = ConfigMonsters.GetMonsterXmlNodeByID(taskXmlNodeByID.TargetNPC2);
			}
			else if (targetType2 != 4)
			{
				npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.TargetNPC2);
			}
			if (monsterVO != null)
			{
				text4 = monsterVO.SName;
				if (string.Empty != text4)
				{
					string tag5 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
					{
						(!flag) ? 1 : 0,
						monsterVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(monsterVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text4, new SolidColorBrush(4294967040U), true, tag5, true);
				}
			}
			if (npcinfoVO != null)
			{
				text4 = npcinfoVO.SName;
				if (string.Empty != text4)
				{
					string tag6 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
					{
						(!flag) ? 1 : 0,
						npcinfoVO.ID,
						Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
					});
					this.textBlockEx.SetSpecialText(text4, new SolidColorBrush(4294967040U), true, tag6, true);
				}
			}
		}
		if (taskXmlNodeByID.TargetMapCode1 >= 0)
		{
			int targetType3 = taskXmlNodeByID.TargetType1;
			string text5 = taskXmlNodeByID.TargetMapCode1.ToString();
			string targetPos = taskXmlNodeByID.TargetPos1;
			if (string.Empty != targetPos)
			{
				string tag7 = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					2,
					-1,
					text5,
					targetPos
				});
				this.textBlockEx.SetSpecialText(targetPos, new SolidColorBrush(4294967040U), true, tag7, true);
			}
		}
		if (taskXmlNodeByID.TargetMapCode2 >= 0)
		{
			int targetType4 = taskXmlNodeByID.TargetType2;
			string text6 = taskXmlNodeByID.TargetMapCode2.ToString();
			string text7 = taskXmlNodeByID.TargetPos2.ToString();
			if (string.Empty != text7)
			{
				string tag8 = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					2,
					-1,
					text6,
					text7
				});
				this.textBlockEx.SetSpecialText(text7, new SolidColorBrush(4294967040U), true, tag8, true);
			}
		}
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("TrackingTaskInfoPart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(100.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ToCloseTimer_Tick);
		this._TimerCount = 0;
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ToCloseTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		if (this._TimerCount >= 10 && this.ToClose != null)
		{
			this.StopHeart();
			this.ToClose.Invoke(this, EventArgs.Empty);
		}
	}

	private SpriteSL thisCtrl;

	public GTextBlockEx textBlockEx;

	private int _TimerCount;

	private DispatcherTimer _Timer;

	public EventHandler ToClose;
}
