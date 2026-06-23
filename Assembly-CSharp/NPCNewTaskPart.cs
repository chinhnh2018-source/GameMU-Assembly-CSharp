using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class NPCNewTaskPart : UserControl
{
	protected override void InitializeComponent()
	{
		Canvas.SetLeft(this.TaskTitle, 65);
		Canvas.SetTop(this.TaskTitle, 16);
		this.Container.Children.Add(this.TaskTitle);
		this.TaskTitle.Text = string.Empty;
		this.TaskTitle.TextSize = (double)FontSizeMgr.NPCNameFontSize;
		this.TaskTitle.TextColor = new SolidColorBrush(16766720U);
		Canvas.SetLeft(this.TalkText, 13);
		Canvas.SetTop(this.TalkText, 38);
		this.Container.Children.Add(this.TalkText);
		this.TalkText.Text = string.Empty;
		this.TalkText.TextSize = (double)FontSizeMgr.NPCTalkFontSize;
		this.TalkText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.AcceptTaskBtn = U3DUtils.NEW<GIcon>();
		this.AcceptTaskBtn.Width = 104.0;
		this.AcceptTaskBtn.Height = 34.0;
		this.AcceptTaskBtn.BodySource = new ImageBrush(Global.GetLoginResImage("Images/Plate/btntask_normal.png"));
		this.AcceptTaskBtn.NewSource = new ImageBrush(Global.GetLoginResImage("Images/Plate/btntask_hover.png"));
		this.AcceptTaskBtn.Text = Global.GetLang("接受任务");
		this.AcceptTaskBtn.TextColor = new SolidColorBrush(16762880U);
		this.AcceptTaskBtn.HorizontalAlignment = global::Layout.Right;
		this.AcceptTaskBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = 2,
					ID = this.TaskID
				});
			}
		};
		Canvas.SetLeft(this.AcceptTaskBtn, 175);
		Canvas.SetTop(this.AcceptTaskBtn, 171);
		this.Container.Children.Add(this.AcceptTaskBtn);
		this.Container.Children.Add(this.YuanBaoCompleteTaskText);
		this.YuanBaoCompleteTaskText.Text = StringUtil.substitute(Global.GetLang("{0}钻石立即完成领取3倍经验奖励"), new object[]
		{
			ConfigSystemParam.GetSystemParamIntByName("YuanBaoCompleteTask")
		});
		this.YuanBaoCompleteTaskText.TextSize = (double)FontSizeMgr.NPCTalkFontSize;
		this.YuanBaoCompleteTaskText.TextColor = new SolidColorBrush(65280U);
		this.YuanBaoCompleteTaskText.TextUnderLine = true;
		this.YuanBaoCompleteTaskText.mouseEnabled = true;
		this.YuanBaoCompleteTaskText.addEventListener("mouseDown", new MouseEventHandler(this.tb_OnMouseDown2));
		this.YuanBaoCompleteTaskText.addEventListener("mouseOver", new MouseEventHandler(this.tb_OnMouseEnter));
		this.YuanBaoCompleteTaskText.addEventListener("mouseOut", new MouseEventHandler(this.tb_OnMouseLeave));
		Canvas.SetLeft(this.YuanBaoCompleteTaskText, 270.0 - this.YuanBaoCompleteTaskText.RealSize.Width);
		Canvas.SetTop(this.YuanBaoCompleteTaskText, 155);
		this.YuanBaoCompleteTaskText.Visibility = false;
		Canvas.SetLeft(this.AcceptTaskText, 175);
		Canvas.SetTop(this.AcceptTaskText, 171);
		this.Container.Children.Add(this.AcceptTaskText);
		this.AcceptTaskText.Text = Global.GetLang("接受任务");
		this.AcceptTaskText.TextSize = (double)FontSizeMgr.NPCTalkFontSize;
		this.AcceptTaskText.TextColor = new SolidColorBrush(65280U);
		this.AcceptTaskText.TextUnderLine = true;
		this.AcceptTaskText.mouseEnabled = true;
		this.AcceptTaskText.addEventListener("mouseDown", new MouseEventHandler(this.tb_OnMouseDown));
		this.AcceptTaskText.addEventListener("mouseOver", new MouseEventHandler(this.tb_OnMouseEnter));
		this.AcceptTaskText.addEventListener("mouseOut", new MouseEventHandler(this.tb_OnMouseLeave));
		Canvas.SetLeft(this.AcceptTaskText, 270.0 - this.AcceptTaskText.RealSize.Width);
		Canvas.SetTop(this.AcceptTaskText, 175);
		Canvas.SetLeft(this.TaskGuid, 13);
		Canvas.SetTop(this.TaskGuid, 208);
		this.Container.Children.Add(this.TaskGuid);
		this.TaskGuid.Text = " ";
		this.TaskGuid.TextSize = (double)FontSizeMgr.NPCTalkFontSize;
		this.TaskGuid.TextColor = new SolidColorBrush(16766720U);
		this.taskJiangLi = U3DUtils.NEW<TaskJiangLi>();
		this.Container.Children.Add(this.taskJiangLi);
		Canvas.SetLeft(this.taskJiangLi, 13);
		Canvas.SetTop(this.taskJiangLi, 238);
		this.thisCtrl = this;
	}

	public int NpcExtensionID
	{
		get
		{
			return this._NpcExtensionID;
		}
		set
		{
			this._NpcExtensionID = value;
		}
	}

	public int NpcID
	{
		get
		{
			return this._NpcID;
		}
		set
		{
			this._NpcID = value;
		}
	}

	private void InitControls()
	{
		this.TalkText.Width = this.Container.Width - 20.0;
		this.TalkText.BodyWidth = this.Container.Width - 20.0;
		this.TalkText.TextFontWrapping = TextWrapping.Wrap;
		this.TaskTitle.Width = this.Container.Width / 2.0;
		this.NetImage2 = U3DUtils.NEW<ShowNetImage>();
		this.NetImage2.BodyWidth = 64.0;
		this.NetImage2.BodyHeight = 64.0;
		Canvas.SetLeft(this.NetImage2, -16);
		Canvas.SetTop(this.NetImage2, -55);
		this.Container.Children.Add(this.NetImage2);
		Sprite sprite = new Sprite();
		this.Container.Children.Add(sprite);
		sprite.graphics.beginFill(0, 1);
		sprite.graphics.drawCircle(16, -23, 32);
		sprite.graphics.endFill();
		this.NetImage2.mask = sprite;
		this.NetImage1 = U3DUtils.NEW<ShowNetImage>();
		this.NetImage1.BodyWidth = 90.0;
		this.NetImage1.BodyHeight = 73.0;
		Canvas.SetLeft(this.NetImage1, -28);
		Canvas.SetTop(this.NetImage1, -60);
		this.Container.Children.Add(this.NetImage1);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.BackgroundAlpha = 0.8;
		this.Container.BackgroundColor = 1384487U;
		this.InitControls();
	}

	public void InitPartData()
	{
	}

	public void GetNewData(int taskID)
	{
		this.TaskTitle.Text = string.Empty;
		this.TalkText.Text = string.Empty;
		this.TaskGuid.Text = string.Empty;
		this.AcceptTaskBtn.Text = "接受任务";
		this.AcceptTaskText.Text = Global.GetLang("接受任务");
		Canvas.SetLeft(this.AcceptTaskText, 270.0 - this.AcceptTaskText.RealSize.Width);
		this.TaskID = taskID;
		if (this.TaskID != -1)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (taskXmlNodeByID != null)
			{
				GameInstance.Game.SpriteGetTaskAwards(this.TaskID);
				this.TaskTitle.Text = taskXmlNodeByID.Title;
				List<TalkTextNode> taskTalkTextInfo = Super.GetTaskTalkTextInfo(this.TaskID, "AcceptTalk");
				this.TalkText.htmlText = Super.FormatTaskTalkText(taskTalkTextInfo);
				Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务接取UI"), taskID, -1, 1);
				this.TaskGuid.htmlText = taskXmlNodeByID.Description;
				Canvas.SetLeft(this.TaskGuid, 11);
			}
		}
		int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(this.NpcExtensionID);
		this.NetImage1.ShowImage(StringUtil.substitute("NetImages/GameRes/Images/Plate/npcTouxiang_bak.png", new object[0]));
		this.NetImage2.ShowImage(StringUtil.substitute("NetImages/NPCs/{0}.png", new object[]
		{
			Global.FormatStr("000", npcpicCodeByID)
		}));
		if (Global.GetTaskTeleportsByID(this.TaskID) > 0)
		{
			this.AcceptTaskBtn.Text = Global.GetLang("接受任务并传送");
			this.AcceptTaskText.Text = Global.GetLang("接受任务并传送");
			Canvas.SetLeft(this.AcceptTaskText, 270.0 - this.AcceptTaskText.RealSize.Width);
		}
		int taskClassByID = Global.GetTaskClassByID(this.TaskID);
		if ((taskClassByID >= 3 && taskClassByID <= 5) || taskClassByID == 7)
		{
			this.AcceptTaskBtn.Visibility = false;
			this.AcceptTaskText.Visibility = true;
			this.YuanBaoCompleteTaskText.Visibility = true;
		}
		else
		{
			this.AcceptTaskBtn.Visibility = true;
			this.AcceptTaskText.Visibility = false;
			this.YuanBaoCompleteTaskText.Visibility = false;
		}
	}

	public void CleanUpChildWindows()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("任务接取UI"), null);
		this.NetImage1.CleanUpChildWindows();
		this.NetImage1.Destroy();
		this.NetImage2.CleanUpChildWindows();
		this.NetImage2.Destroy();
	}

	public void RefreshTaskAwardsData(TaskAwardsData taskAwardsData)
	{
		this.taskJiangLi.TaskAwardsData = taskAwardsData;
	}

	private void tb_OnMouseDown(MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
			{
				IDType = 2,
				ID = this.TaskID
			});
		}
	}

	private void tb_OnMouseDown2(MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
			{
				IDType = 3,
				ID = this.TaskID
			});
		}
	}

	private void tb_OnMouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
		}
	}

	private void tb_OnMouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
		}
	}

	private ShowNetImage NetImage1;

	private ShowNetImage NetImage2;

	private int TaskID = -1;

	private GTextBlockOutLine TaskTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TalkText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private SpriteSL thisCtrl = new SpriteSL();

	private GTextBlockOutLine TaskGuid = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GIcon AcceptTaskBtn;

	private TaskJiangLi taskJiangLi;

	private GTextBlockOutLine YuanBaoCompleteTaskText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine AcceptTaskText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _NpcExtensionID;

	private int _NpcID;

	public DPSelectedItemEventHandler DPSelectedItem;
}
