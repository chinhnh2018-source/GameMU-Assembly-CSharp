using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class NPCDoingTasksPart : UserControl
{
	public NPCDoingTasksPart()
	{
		this.TaskDescText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		this.TaskDescText.Width = 380.0;
		this.TaskDescText.TextWidth = 380.0;
		this.TaskDescText.HorizontalAlignment = global::Layout.Left;
		this.TaskDescText.VerticalAlignment = global::Layout.Top;
		this.TaskDescText.FontSize = 12;
		this.TaskDescText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.TaskDescText.TextFontWrapping = TextWrapping.Wrap;
		this.TaskDescText.TextClick = new UIEventEventHandler(this.TextClick);
		this.TaskDescText.TextMouseEnter = new UIEventEventHandler(this.TextMouseEnter);
		this.TaskDescText.TextMouseLeave = new UIEventEventHandler(this.TextMouseLeave);
		this.TaskDesc.Children.Add(this.TaskDescText);
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.ContainPanel);
		this.ContainPanel.HorizontalAlignment = global::Layout.Left;
		this.ContainPanel.VerticalAlignment = global::Layout.Top;
		this.ContainPanel.Orientation = global::Layout.Vertical;
		this.ContainPanel.Children.Add(this.checkPanel);
		this.checkPanel.HorizontalAlignment = global::Layout.Left;
		this.checkPanel.VerticalAlignment = global::Layout.Top;
		this.checkPanel.Orientation = global::Layout.Horizontal;
		this.checkPanel.Children.Add(this.GTextBlockOutLine1);
		this.GTextBlockOutLine1.Text = Global.GetLang("任务介绍:");
		this.GTextBlockOutLine1.FontSize = HSTextField.defaultFontSize;
		this.GTextBlockOutLine1.VerticalAlignment = global::Layout.Top;
		this.GTextBlockOutLine1.TextColor = new SolidColorBrush(2202065U);
		this.GTextBlockOutLine1.Width = 150.0;
		this.ContainPanel.Children.Add(this.Rectangle2);
		this.Rectangle2.Height = 8.0;
		this.Rectangle2.Width = 290.0;
		this.Rectangle2.HorizontalAlignment = global::Layout.Left;
		this.Rectangle2.VerticalAlignment = global::Layout.Top;
		this.Rectangle2.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.TaskDesc);
		this.TaskDesc.HorizontalAlignment = global::Layout.Left;
		this.TaskDesc.VerticalAlignment = global::Layout.Top;
		this.TaskDesc.Orientation = global::Layout.Vertical;
		this.ContainPanel.Children.Add(this.Rectangle3);
		this.Rectangle3.Height = 8.0;
		this.Rectangle3.Width = 290.0;
		this.Rectangle3.HorizontalAlignment = global::Layout.Left;
		this.Rectangle3.VerticalAlignment = global::Layout.Top;
		this.Rectangle3.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.StackPanel4);
		this.StackPanel4.HorizontalAlignment = global::Layout.Left;
		this.StackPanel4.VerticalAlignment = global::Layout.Top;
		this.StackPanel4.Orientation = global::Layout.Horizontal;
		this.StackPanel4.Children.Add(this.GTextBlockOutLine5);
		this.GTextBlockOutLine5.Text = Global.GetLang("任务目标：");
		this.GTextBlockOutLine5.FontSize = HSTextField.defaultFontSize;
		this.GTextBlockOutLine5.VerticalAlignment = global::Layout.Top;
		this.GTextBlockOutLine5.TextColor = new SolidColorBrush(2202065U);
		this.GTextBlockOutLine5.Width = 180.0;
		this.ContainPanel.Children.Add(this.Rectangle6);
		this.Rectangle6.Height = 5.0;
		this.Rectangle6.Width = 290.0;
		this.Rectangle6.HorizontalAlignment = global::Layout.Left;
		this.Rectangle6.VerticalAlignment = global::Layout.Top;
		this.Rectangle6.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.TaskTarget);
		this.TaskTarget.HorizontalAlignment = global::Layout.Left;
		this.TaskTarget.VerticalAlignment = global::Layout.Top;
		this.TaskTarget.Children.Add(this.TaskTargetText);
		this.TaskTargetText.Text = Global.GetLang("任务目标的内容");
		this.TaskTargetText.VerticalAlignment = global::Layout.Top;
		this.TaskTargetText.FontSize = HSTextField.defaultFontSize;
		this.TaskTargetText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.TaskTargetText.Width = 360.0;
		this.ContainPanel.Children.Add(this.Rectangle7);
		this.Rectangle7.Height = 5.0;
		this.Rectangle7.Width = 290.0;
		this.Rectangle7.HorizontalAlignment = global::Layout.Left;
		this.Rectangle7.VerticalAlignment = global::Layout.Top;
		this.Rectangle7.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.StackPanel8);
		this.StackPanel8.HorizontalAlignment = global::Layout.Left;
		this.StackPanel8.VerticalAlignment = global::Layout.Top;
		this.StackPanel8.Orientation = global::Layout.Horizontal;
		this.StackPanel8.Children.Add(this.GTextBlockOutLine9);
		this.GTextBlockOutLine9.Text = Global.GetLang("任务奖励：");
		this.GTextBlockOutLine9.FontSize = HSTextField.defaultFontSize;
		this.GTextBlockOutLine9.VerticalAlignment = global::Layout.Top;
		this.GTextBlockOutLine9.TextColor = new SolidColorBrush(2202065U);
		this.GTextBlockOutLine9.Width = 180.0;
		this.ContainPanel.Children.Add(this.Rectangle10);
		this.Rectangle10.Height = 5.0;
		this.Rectangle10.Width = 290.0;
		this.Rectangle10.HorizontalAlignment = global::Layout.Left;
		this.Rectangle10.VerticalAlignment = global::Layout.Top;
		this.Rectangle10.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.TaskAward);
		this.TaskAward.HorizontalAlignment = global::Layout.Left;
		this.TaskAward.VerticalAlignment = global::Layout.Top;
		this.TaskAward.Children.Add(this.TaskAwardText);
		this.TaskAwardText.Text = string.Empty;
		this.TaskAwardText.VerticalAlignment = global::Layout.Top;
		this.TaskAwardText.FontSize = HSTextField.defaultFontSize;
		this.TaskAwardText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.TaskAwardText.Width = 360.0;
		this.ContainPanel.Children.Add(this.Rectangle11);
		this.Rectangle11.Height = 5.0;
		this.Rectangle11.Width = 290.0;
		this.Rectangle11.HorizontalAlignment = global::Layout.Left;
		this.Rectangle11.VerticalAlignment = global::Layout.Top;
		this.Rectangle11.IsHitTestVisible = false;
		this.ContainPanel.Children.Add(this.ListPanel);
		this.ListPanel.HorizontalAlignment = global::Layout.Left;
		this.ListPanel.VerticalAlignment = global::Layout.Top;
		this.ListPanel.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 15);
		this.listBox.Width = 400.0;
		this.listBox.Height = 60.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.thisCtrl = this;
		this.taskJiangLi = U3DUtils.NEW<TaskJiangLi>();
		this.ContainPanel.Children.Add(this.taskJiangLi);
		Canvas.SetLeft(this.taskJiangLi, 13);
		Canvas.SetTop(this.taskJiangLi, 238);
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
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
		this.treeView1.InitControls2(138, 310);
		this.treeView1.IdentWidth = 5.0;
		this.treeView1.IdentWidth2 = 15.0;
		Canvas.SetLeft(this.treeView1, 10);
		Canvas.SetTop(this.treeView1, 45);
		this.Container.Children.Add(this.treeView1);
		this.treeView1.ItemClick = new EventHandler(this.treeView1_SelectedItemChanged);
		this.scrollViewer1 = new GScrollView(430, 280, 0);
		Canvas.SetLeft(this.scrollViewer1, 172);
		Canvas.SetTop(this.scrollViewer1, 50);
		this.Container.Children.Remove(this.ContainPanel, true);
		this.ContainPanel.Width = this.scrollViewer1.Width;
		this.scrollViewer1.Viewer = this.ContainPanel;
		this.Container.Children.Add(this.scrollViewer1);
		this.scrollViewer1.ResetScrollView();
		this.checkBoxTask = new GCheckBox();
		this.checkBoxTask.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		this.checkBoxTask.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		this.checkBoxTask.Text = Global.GetLang("隐藏游戏界面上的任务引导");
		this.checkBoxTask.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 33, 153, 209));
		this.checkBoxTask.CheckChanged = new BaseEventHandler2(this.checkBoxTask_Click);
		Canvas.SetLeft(this.checkBoxTask, 590);
		this.checkPanel.Children.Add(this.checkBoxTask);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("放弃任务");
		Canvas.SetLeft(gicon, 499);
		Canvas.SetTop(gicon, 278);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			SpriteSL selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
			if (null != selectedSubLevelItem)
			{
				GTextBlockOutLine gtextBlockOutLine = selectedSubLevelItem.getChildAt(selectedSubLevelItem.numChildren - 1).SafeGetComponent<GTextBlockOutLine>();
				if (null == gtextBlockOutLine)
				{
					return;
				}
				int num = (int)gtextBlockOutLine.Tag;
				TaskData taskData = Super.GetTaskDataByTaskID(num);
				if (taskData != null)
				{
					string text = string.Empty;
					TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(num);
					if (taskXmlNodeByID != null)
					{
						text = taskXmlNodeByID.Title;
					}
					GChildWindow messageBoxWindow = Super.ShowMessageBox(this.tc.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("确认要放弃任务:【{0}】？"), new object[]
					{
						text
					}), ((int)this.tc.TabWidth - 253) / 2, ((int)this.tc.TabHeight - 171) / 2, (int)this.tc.TabWidth, (int)this.tc.TabHeight, 0.01, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.tc.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SpriteAbandonTask(taskData.DbID, taskData.DoingTaskID);
							this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
							this.Container.Children.Add(this.LoadingWin);
						}
						return true;
					};
				}
			}
		};
	}

	public void RefreshTaskDataFocus(int taskID)
	{
		SpriteSL selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
		if (null != selectedSubLevelItem)
		{
			GTextBlockOutLine gtextBlockOutLine = selectedSubLevelItem.getChildAt(selectedSubLevelItem.numChildren - 1).SafeGetComponent<GTextBlockOutLine>();
			if (null == gtextBlockOutLine)
			{
				return;
			}
			if (taskID == (int)gtextBlockOutLine.Tag)
			{
				TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(taskID);
				if (taskDataByTaskID != null)
				{
					this.checkBoxTask.Check = (taskDataByTaskID.DoingTaskFocus <= 0);
				}
			}
		}
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			this.treeView1_SelectedItemChanged(null, null);
			return;
		}
		this.FirstGetNewData = false;
		this.RefreshTaskData();
	}

	public void RefreshTaskData()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.TaskDescText.Text = string.Empty;
		this.TaskTargetText.Text = string.Empty;
		this.TaskAwardText.Text = string.Empty;
		this.treeView1.ClearAll();
		this.treeView1.AddOneLevelItem(Global.GetLang("主线任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("支线任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("循环任务"));
		this.treeView1.AddOneLevelItem(Global.GetLang("猎杀日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("武学日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("军功日常"));
		this.treeView1.AddOneLevelItem(Global.GetLang("魔族势力"));
		if (Global.Data.roleData.TaskDataList == null)
		{
			this.treeView1.RefreshListBox();
			this.treeView1_SelectedItemChanged(null, null);
			return;
		}
		for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(Global.Data.roleData.TaskDataList[i].DoingTaskID);
			if (taskXmlNodeByID != null)
			{
				int taskClass = taskXmlNodeByID.TaskClass;
				if (taskClass >= 0 && taskClass <= 7)
				{
					uint color = uint.MaxValue;
					string text = taskXmlNodeByID.Title;
					if (taskClass == 3 || taskClass == 4 || taskClass == 5 || taskClass == 6 || taskClass == 7)
					{
						text = Global.GetPaoHuanTaskTitle(taskXmlNodeByID, text);
					}
					if (Super.JugeTaskComplete(taskXmlNodeByID, Global.Data.roleData.TaskDataList[i].DoingTaskVal1, Global.Data.roleData.TaskDataList[i].DoingTaskVal2))
					{
						color = ColorSL.FromArgb(255, 30, 144, 255);
						text = StringUtil.substitute("{0}({1})", new object[]
						{
							text,
							Global.GetLang("完成")
						});
					}
					else
					{
						int taketime = taskXmlNodeByID.Taketime;
						if (taketime > 0 && Global.GetCorrectLocalTime() - Global.Data.roleData.TaskDataList[i].AddDateTime >= (long)(taketime * 1000))
						{
							color = ColorSL.FromArgb(255, 224, 211, 48);
							text = StringUtil.substitute("{0}({1})", new object[]
							{
								text,
								Global.GetLang("失败")
							});
						}
						string pubStartTime = taskXmlNodeByID.PubStartTime;
						string pubEndTime = taskXmlNodeByID.PubEndTime;
						if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
						{
							double num = (double)Global.GetCorrectLocalTime();
							double num2 = (double)Global.SafeConvertToTicks(pubStartTime);
							double num3 = (double)Global.SafeConvertToTicks(pubEndTime);
							if (num < num2 || num > num3)
							{
								color = ColorSL.FromArgb(255, 224, 211, 48);
								text = StringUtil.substitute("{0}({1})", new object[]
								{
									text,
									Global.GetLang("失败")
								});
							}
						}
						int limitLevel = taskXmlNodeByID.LimitLevel;
						if (limitLevel > 0 && Global.Data.roleData.Level < limitLevel)
						{
							color = ColorSL.FromArgb(255, 224, 211, 48);
							text = StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
							{
								text,
								Global.Data.roleData.Level,
								limitLevel
							});
						}
					}
					GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
					gtextBlockOutLine.TextSize = (double)HSTextField.defaultFontSize;
					gtextBlockOutLine.TextColor = new SolidColorBrush(color);
					gtextBlockOutLine.Text = text;
					gtextBlockOutLine.Tag = Global.Data.roleData.TaskDataList[i].DoingTaskID;
					gtextBlockOutLine.Width = 152.0;
					gtextBlockOutLine.TextFontWrapping = TextWrapping.Wrap;
					Canvas canvas = new Canvas();
					canvas.Width = 200.0;
					canvas.Height = 20.0;
					canvas.Background = new SolidColorBrush(4278190080U);
					canvas.BackgroundAlpha = 0.001;
					canvas.Children.Add(gtextBlockOutLine);
					this.treeView1.AddSubLevelItem(taskClass, canvas);
				}
			}
		}
		this.treeView1.SelectedOneLevelItem = 0;
		if (this.treeView1.GetSubLevelItemCount(0) > 0)
		{
			this.treeView1.SelectedSubLevelItem = 0;
		}
		this.treeView1.RefreshListBox();
		this.treeView1_SelectedItemChanged(null, null);
	}

	public void InitPartData()
	{
	}

	public void CleanUpChildWindows()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("任务信息UI"), null);
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void treeView1_SelectedItemChanged(object sender, object e)
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("任务信息UI"), null);
		Super.GData.ViewTaskInfoGoodsDataList = null;
		this.goodsName.RemoveRange(0, this.goodsName.Count);
		this.ItemCollection.Clear();
		SpriteSL selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
		if (null != selectedSubLevelItem)
		{
			GTextBlockOutLine gtextBlockOutLine = selectedSubLevelItem.getChildAt(selectedSubLevelItem.numChildren - 1).SafeGetComponent<GTextBlockOutLine>();
			if (null == gtextBlockOutLine)
			{
				return;
			}
			string text = gtextBlockOutLine.Text;
			int num = (int)gtextBlockOutLine.Tag;
			this.taskJiangLi.TaskID = num;
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(num);
			if (taskXmlNodeByID != null)
			{
				if (text.IndexOf(Global.GetLang("(完成")) == -1)
				{
					Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务信息UI"), num, 0, 1);
				}
				else
				{
					Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务信息UI"), num, 1, 1);
				}
				this.TaskTargetText.Text = StringUtil.substitute("  {0}{1}", new object[]
				{
					Super.GetTaskTargetDesc(taskXmlNodeByID, 1, true),
					Super.GetTaskTargetNum(taskXmlNodeByID, Super.GetTaskDataByTaskID(num).DoingTaskVal1, 1)
				});
				if (taskXmlNodeByID.TargetNPC2 >= 0)
				{
					if (this.TaskTargetText.Text.Length > 0)
					{
						GTextBlockOutLine taskTargetText = this.TaskTargetText;
						taskTargetText.Text += "\n  ";
					}
					GTextBlockOutLine taskTargetText2 = this.TaskTargetText;
					taskTargetText2.Text += StringUtil.substitute("{0}{1}", new object[]
					{
						Super.GetTaskTargetDesc(taskXmlNodeByID, 2, true),
						Super.GetTaskTargetNum(taskXmlNodeByID, Super.GetTaskDataByTaskID(num).DoingTaskVal2, 2)
					});
				}
				string description = taskXmlNodeByID.Description2;
				this.TaskDescText.NoRenderText = string.Empty + description + "\n";
				string text2 = string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				string text5 = string.Empty;
				MonsterVO monsterVO = null;
				NPCInfoVO npcinfoVO = null;
				if (taskXmlNodeByID.SourceNPC >= 0)
				{
					npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.SourceNPC);
					if (npcinfoVO != null)
					{
						text2 = npcinfoVO.SName;
						if (string.Empty != text2)
						{
							string tag = StringUtil.substitute("0,{0},{1},-1,-1", new object[]
							{
								npcinfoVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text2, new SolidColorBrush(4294967040U), true, tag, false);
						}
					}
				}
				if (taskXmlNodeByID.DestNPC >= 0)
				{
					npcinfoVO = ConfigNPCs.GetNPCVOByID(taskXmlNodeByID.DestNPC);
					if (npcinfoVO != null)
					{
						text3 = npcinfoVO.SName;
						if (string.Empty != text3)
						{
							string tag2 = StringUtil.substitute("0,{0},{1},-1,-1", new object[]
							{
								npcinfoVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text3, new SolidColorBrush(4294967040U), true, tag2, false);
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
						text4 = monsterVO.SName;
						if (string.Empty != text4)
						{
							string tag3 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
							{
								(!flag) ? 1 : 0,
								monsterVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(monsterVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text4, new SolidColorBrush(4294967040U), true, tag3, false);
						}
					}
					if (npcinfoVO != null)
					{
						text4 = npcinfoVO.SName;
						if (string.Empty != text4)
						{
							string tag4 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
							{
								(!flag) ? 1 : 0,
								npcinfoVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text4, new SolidColorBrush(4294967040U), true, tag4, false);
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
						text5 = monsterVO.SName;
						if (string.Empty != text5)
						{
							string tag5 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
							{
								(!flag) ? 1 : 0,
								monsterVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(monsterVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text5, new SolidColorBrush(4294967040U), true, tag5, false);
						}
					}
					if (npcinfoVO != null)
					{
						text5 = npcinfoVO.SName;
						if (string.Empty != text5)
						{
							string tag6 = StringUtil.substitute("{0},{1},{2},-1,-1", new object[]
							{
								(!flag) ? 1 : 0,
								npcinfoVO.ID,
								Global.GetNPCOrMonsterMapCodeByID(npcinfoVO.MapCode)
							});
							this.TaskDescText.SetSpecialText(text5, new SolidColorBrush(4294967040U), true, tag6, false);
						}
					}
				}
				if (taskXmlNodeByID.TargetMapCode1 >= 0)
				{
					int targetType3 = taskXmlNodeByID.TargetType1;
					string text6 = taskXmlNodeByID.TargetMapCode1.ToString();
					string text7 = taskXmlNodeByID.TargetPos1.ToString();
					if (string.Empty != text7)
					{
						string tag7 = StringUtil.substitute("{0},{1},{2},{3}", new object[]
						{
							2,
							-1,
							text6,
							text7
						});
						this.TaskDescText.SetSpecialText(text7, new SolidColorBrush(4294967040U), true, tag7, false);
					}
				}
				if (taskXmlNodeByID.TargetMapCode2 >= 0)
				{
					int targetType4 = taskXmlNodeByID.TargetType2;
					string text8 = taskXmlNodeByID.TargetMapCode2.ToString();
					string text9 = taskXmlNodeByID.TargetPos2.ToString();
					if (string.Empty != text9)
					{
						string tag8 = StringUtil.substitute("{0},{1},{2},{3}", new object[]
						{
							2,
							-1,
							text8,
							text9
						});
						this.TaskDescText.SetSpecialText(text9, new SolidColorBrush(4294967040U), true, tag8, false);
					}
				}
				this.TaskDescText.RenderText();
				TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(num);
				if (taskDataByTaskID != null)
				{
					this.checkBoxTask.Check = (taskDataByTaskID.DoingTaskFocus <= 0);
				}
			}
		}
		else
		{
			this.TaskDescText.Text = string.Empty;
			this.TaskTargetText.Text = string.Empty;
			this.TaskAwardText.Text = string.Empty;
			this.checkBoxTask.Check = false;
			this.taskJiangLi.TaskID = 0;
		}
		this.scrollViewer1.ResetScrollView();
	}

	private GGoodIcon GetIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty);
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			15
		});
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	public static string GetTaskAwards(TaskData taskData)
	{
		string text = string.Empty;
		if (taskData == null)
		{
			return text;
		}
		if (taskData.TaskAwards.Experienceaward > 0L)
		{
			if (text.Length > 0)
			{
				text += "\n  ";
			}
			text += StringUtil.substitute(Global.GetLang("经验 x {0}"), new object[]
			{
				taskData.TaskAwards.Experienceaward
			});
		}
		if (taskData.TaskAwards.Moneyaward > 0)
		{
			if (text.Length > 0)
			{
				text += "   ";
			}
			text += StringUtil.substitute(Global.GetLang("  绑定金币 x {0}"), new object[]
			{
				taskData.TaskAwards.Moneyaward
			});
		}
		if (taskData.TaskAwards.YinLiangaward > 0)
		{
			if (text.Length > 0)
			{
				text += "  ";
			}
			text += StringUtil.substitute(Global.GetLang("  金币 x {0}"), new object[]
			{
				taskData.TaskAwards.YinLiangaward
			});
		}
		if (taskData.TaskAwards.LingLiaward > 0)
		{
			if (text.Length > 0)
			{
				text += "  ";
			}
			text += StringUtil.substitute(Global.GetLang("  灵力 x {0}"), new object[]
			{
				taskData.TaskAwards.LingLiaward
			});
		}
		return text;
	}

	private List<GoodsData> ParseAwards(TaskData taskData)
	{
		if (taskData == null)
		{
			return null;
		}
		if (taskData.TaskAwards == null)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		if (taskData.TaskAwards.TaskawardList != null)
		{
			for (int i = 0; i < taskData.TaskAwards.TaskawardList.Count; i++)
			{
				this.ParseAwardsItem(taskData.TaskAwards.TaskawardList[i], list, true);
			}
		}
		if (taskData.TaskAwards.OtherTaskawardList != null)
		{
			for (int j = 0; j < taskData.TaskAwards.OtherTaskawardList.Count; j++)
			{
				this.ParseAwardsItem(taskData.TaskAwards.OtherTaskawardList[j], list, false);
			}
		}
		return list;
	}

	private void ParseAwardsItem(AwardsItemData awardsItemData, List<GoodsData> goodsDataList, bool occupation)
	{
		if (awardsItemData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(awardsItemData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			if (occupation)
			{
				if (Global.ValidOccupation(goodsXmlNodeByID.ToOccupation, -1))
				{
					goodsDataList.Add(new GoodsData
					{
						Id = goodsDataList.Count + 1,
						GoodsID = awardsItemData.GoodsID,
						Using = 0,
						Forge_level = awardsItemData.Level,
						Starttime = "1900-01-01 12:00:00",
						Endtime = awardsItemData.EndTime,
						Site = 0,
						Quality = awardsItemData.Quality,
						Props = string.Empty,
						GCount = awardsItemData.GoodsNum,
						Binding = awardsItemData.Binding,
						Jewellist = string.Empty,
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = 0,
						Lucky = 0,
						Strong = 0,
						ExcellenceInfo = awardsItemData.ExcellencePorpValue,
						AppendPropLev = awardsItemData.IsHaveLuckyProp
					});
					this.goodsName.Add(goodsXmlNodeByID.Title);
				}
			}
			else
			{
				goodsDataList.Add(new GoodsData
				{
					Id = goodsDataList.Count + 1,
					GoodsID = awardsItemData.GoodsID,
					Using = 0,
					Forge_level = awardsItemData.Level,
					Starttime = "1900-01-01 12:00:00",
					Endtime = awardsItemData.EndTime,
					Site = 0,
					Quality = awardsItemData.Quality,
					Props = string.Empty,
					GCount = awardsItemData.GoodsNum,
					Binding = awardsItemData.Binding,
					Jewellist = string.Empty,
					BagIndex = 0,
					AddPropIndex = 0,
					BornIndex = 0,
					Lucky = 0
				});
				this.goodsName.Add(goodsXmlNodeByID.Title);
			}
		}
	}

	public bool TextMouseEnter(object sender, BaseEventArgs e)
	{
		if (!(e.Tag is SpecialTextItem))
		{
			return false;
		}
		(sender as GTextBlockExItem).Link(new SolidColorBrush(4289014314U));
		return false;
	}

	public bool TextMouseLeave(object sender, BaseEventArgs e)
	{
		if (!(e.Tag is SpecialTextItem))
		{
			return false;
		}
		(sender as GTextBlockExItem).Unlink();
		return false;
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			return false;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		string text2 = (e.Tag as SpecialTextItem).Tag as string;
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("扩展路径信息, 无法寻路: {0}"), new object[]
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路定位: {0}"), new object[]
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
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		}
		return false;
	}

	private void checkBoxTask_Checked(object sender, BaseEventArgs e)
	{
		SpriteSL selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
		if (null != selectedSubLevelItem)
		{
			GTextBlockOutLine gtextBlockOutLine = selectedSubLevelItem.getChildAt(selectedSubLevelItem.numChildren - 1).SafeGetComponent<GTextBlockOutLine>();
			if (null == gtextBlockOutLine)
			{
				return;
			}
			int taskID = (int)gtextBlockOutLine.Tag;
			TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(taskID);
			if (taskDataByTaskID != null)
			{
				GameInstance.Game.SpriteModTask(taskDataByTaskID.DbID, taskDataByTaskID.DoingTaskID, 0);
			}
		}
	}

	private void checkBoxTask_Unchecked(object sender, BaseEventArgs e)
	{
		if (Super.GetFocusTaskCount() < Global.Data.TaskMaxFocusCount)
		{
			SpriteSL selectedSubLevelItem = this.treeView1.GetSelectedSubLevelItem();
			if (null != selectedSubLevelItem)
			{
				GTextBlockOutLine gtextBlockOutLine = selectedSubLevelItem.getChildAt(selectedSubLevelItem.numChildren - 1).SafeGetComponent<GTextBlockOutLine>();
				if (null == gtextBlockOutLine)
				{
					return;
				}
				int taskID = (int)gtextBlockOutLine.Tag;
				TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(taskID);
				if (taskDataByTaskID != null)
				{
					GameInstance.Game.SpriteModTask(taskDataByTaskID.DbID, taskDataByTaskID.DoingTaskID, 1);
				}
			}
		}
		else
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.tc.Container, 0, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("追踪的任务个数不能超过:{0}个!"), new object[]
			{
				Global.Data.TaskMaxFocusCount
			}), ((int)this.tc.TabWidth - 253) / 2, ((int)this.tc.TabHeight - 171) / 2, (int)this.tc.TabWidth, (int)this.tc.TabHeight, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				this.checkBoxTask.Check = true;
				Super.CloseMessageBox(this.tc.Container, messageBoxWindow);
				return true;
			};
		}
	}

	private void checkBoxTask_Click(object sender, BaseEventArgs e)
	{
		if (this.checkBoxTask.Check)
		{
			this.checkBoxTask_Checked(sender, e);
		}
		else
		{
			this.checkBoxTask_Unchecked(sender, e);
		}
	}

	private GTextBlockEx TaskDescText;

	private GScrollView scrollViewer1;

	private GCheckBox checkBoxTask;

	private LoadingWindow LoadingWin;

	private GTreeListBox treeView1;

	private bool FirstGetNewData = true;

	private BitmapData backImg = Global.GetGameResImage("Images/Plate/xtsl_rec1.png");

	private List<string> goodsName = new List<string>();

	private Canvas Root;

	private StackPanel ContainPanel = new StackPanel();

	private StackPanel checkPanel = new StackPanel();

	private GTextBlockOutLine GTextBlockOutLine1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL Rectangle2 = new RectangleSL();

	private StackPanel TaskDesc = new StackPanel();

	private RectangleSL Rectangle3 = new RectangleSL();

	private StackPanel StackPanel4 = new StackPanel();

	private GTextBlockOutLine GTextBlockOutLine5 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL Rectangle6 = new RectangleSL();

	private StackPanel TaskTarget = new StackPanel();

	private GTextBlockOutLine TaskTargetText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL Rectangle7 = new RectangleSL();

	private StackPanel StackPanel8 = new StackPanel();

	private GTextBlockOutLine GTextBlockOutLine9 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL Rectangle10 = new RectangleSL();

	private StackPanel TaskAward = new StackPanel();

	private GTextBlockOutLine TaskAwardText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL Rectangle11 = new RectangleSL();

	private StackPanel ListPanel = new StackPanel();

	private ListBox listBox = new ListBox();

	private SpriteSL thisCtrl = new SpriteSL();

	private TaskJiangLi taskJiangLi;

	private ObservableCollection _ItemCollection;

	private GTabControl _tc;

	public DPSelectedItemEventHandler DPSelectedItem;
}
