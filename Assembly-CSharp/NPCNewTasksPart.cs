using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class NPCNewTasksPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		this.treeView1 = U3DUtils.NEW<GTreeListBox>();
		this.treeView1.InitControls2(606, 278);
		this.treeView1.IdentWidth = 25.0;
		this.treeView1.IdentWidth2 = 35.0;
		Canvas.SetLeft(this.treeView1, 4);
		Canvas.SetTop(this.treeView1, 25);
		this.treeView1.ItemClick = new EventHandler(this.TreeView_MouseLeftButtonDown1);
		this.treeView1.AddOneLevelItem(Global.GetLang("主线任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("支线任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("循环任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("猎杀日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("武学日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("军功日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("魔族势力"));
		this.treeView1.RefreshListBox();
		this.Container.Children.Add(this.ScrollViewer1);
		this.ScrollViewer1.Width = 606.0;
		this.ScrollViewer1.Height = 278.0;
		Canvas.SetLeft(this.ScrollViewer1, 4);
		Canvas.SetTop(this.ScrollViewer1, 45);
		this.ScrollViewer1.HorizontalScrollBarVisibility = global::ScrollBarVisibility.Disabled;
		this.ScrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer1.Viewer = this.treeView1;
	}

	public void RefreshTaskData(List<int> newTaskDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.NewTaskDataList = newTaskDataList;
		this.treeView1.ClearSubLevelItem(0);
		this.treeView1.ClearSubLevelItem(1);
		this.treeView1.ClearSubLevelItem(2);
		this.treeView1.ClearSubLevelItem(3);
		this.treeView1.ClearSubLevelItem(4);
		this.treeView1.ClearSubLevelItem(5);
		this.treeView1.ClearSubLevelItem(6);
		if (this.NewTaskDataList == null)
		{
			return;
		}
		for (int i = 0; i < this.NewTaskDataList.Count; i++)
		{
			int id = this.NewTaskDataList[i];
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(id);
			if (taskXmlNodeByID != null)
			{
				int maxLevel = taskXmlNodeByID.MaxLevel;
				if (Global.Data.roleData.Level <= maxLevel)
				{
					int taskClass = taskXmlNodeByID.TaskClass;
					if (taskClass >= 0 && taskClass <= 7)
					{
						Canvas canvas = new Canvas();
						canvas.Width = 560.0;
						canvas.Height = 20.0;
						canvas.Background = new SolidColorBrush(4278190080U);
						canvas.BackgroundAlpha = 0.001;
						string text = taskXmlNodeByID.Title;
						if (taskClass == 3 || taskClass == 4 || taskClass == 5 || taskClass == 6 || taskClass == 7)
						{
							text = Global.GetPaoHuanTaskTitle(taskXmlNodeByID, text);
						}
						GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
						gtextBlockOutLine.Visibility = true;
						gtextBlockOutLine.FontSize = 12;
						gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
						gtextBlockOutLine.Text = text;
						gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 30, 144, 255));
						canvas.Children.Add(gtextBlockOutLine);
						Canvas.SetLeft(gtextBlockOutLine, 0);
						Canvas.SetTop(gtextBlockOutLine, 0);
						gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
						gtextBlockOutLine.Visibility = true;
						gtextBlockOutLine.FontSize = 12;
						gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
						gtextBlockOutLine.Text = taskXmlNodeByID.MinLevel.ToString();
						gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
						canvas.Children.Add(gtextBlockOutLine);
						Canvas.SetLeft(gtextBlockOutLine, 221);
						Canvas.SetTop(gtextBlockOutLine, 0);
						string tag = null;
						string text2 = string.Empty;
						if (taskXmlNodeByID.SourceNPC > 0)
						{
							NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.SourceNPC);
							if (npcvobyID != null)
							{
								text2 = npcvobyID.SName;
								text2 = "[" + ConfigSettings.GetMapNameByCode(Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode), false) + "] " + text2;
								tag = StringUtil.substitute("{0},{1}", new object[]
								{
									npcvobyID.ID,
									Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode)
								});
							}
						}
						gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
						gtextBlockOutLine.Visibility = true;
						gtextBlockOutLine.FontSize = 12;
						gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
						gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 255, 0));
						gtextBlockOutLine.Text = text2;
						canvas.Children.Add(gtextBlockOutLine);
						Canvas.SetLeft(gtextBlockOutLine, 392);
						Canvas.SetTop(gtextBlockOutLine, 0);
						canvas.Tag = tag;
						this.treeView1.AddSubLevelItem(taskClass, canvas);
					}
				}
			}
		}
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		this.treeView1.ResetListBox();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteGetNewTaskData();
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void TreeView_MouseLeftButtonDown1(object sender, object e)
	{
		object selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
		if (selectedSubLevelItem == null || !(selectedSubLevelItem is Canvas))
		{
			return;
		}
		Canvas canvas = selectedSubLevelItem as Canvas;
		if (null == canvas)
		{
			return;
		}
		double num = (double)Global.GetCorrectLocalTime();
		if (num - this.LastClickTicks <= 300.0)
		{
			string text = canvas.Tag as string;
			if (text == null)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有找到路径信息, 无法寻路"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				int mapCode = Convert.ToInt32(array[1]);
				Global.Data.TargetNpcID = Convert.ToInt32(array[0]);
				Point npcpointByID = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
				if (npcpointByID.X == -1 || npcpointByID.Y == -1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("NPC在场景中不存在 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
				}
				else
				{
					Global.Data.GameScene.AutoFindRoad(mapCode, npcpointByID, 120, ExtActionTypes.EXTACTION_NPCDLG);
				}
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				}
			}
		}
		else
		{
			this.LastClickTicks = num;
		}
	}

	private LoadingWindow LoadingWin;

	private GTreeListBox treeView1;

	private List<int> NewTaskDataList;

	private double LastClickTicks;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private Canvas Root;

	private GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);

	public DPSelectedItemEventHandler DPSelectedItem;
}
