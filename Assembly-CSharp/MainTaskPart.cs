using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MainTaskPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._TaskContentBak.URL = Global.GetGameResImageString("TaskContentBak.jpg");
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
		this._PrePage.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnPreZhangJie);
		this._NextPage.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnNextZhangJie);
		this._TaskTitle.Text = string.Empty;
		if (this._Submit != null && this._Submit.Label.text != null)
		{
			this._Submit.Label.text = Global.GetLang("立即前往");
		}
	}

	protected void OnClose(object sender, EventArgs e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, null);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnSubmit(object sender, EventArgs e)
	{
		if (this.taskData == null)
		{
			this.BodyClick();
		}
		else if (this.IsCompleted)
		{
			GameInstance.Game.SpriteCompleteTask(this.NpcID, this.TaskID, this.DbID, 0);
		}
		else
		{
			Super.PrccessAutoTaskFindRoad(this.TaskID, false, true, false, false);
		}
		this.OnClose(this, null);
	}

	public void NotifySubmitResult(int ret, int npcID, int taskID)
	{
	}

	private void ShowLastMainTask()
	{
		this.taskVO = ConfigTasks.GetTaskXmlNodeByID(Global.Data.roleData.CompletedMainTaskID);
		if (this.taskVO != null)
		{
			this.RefreshZhangJieUI(this.taskVO.TaskZhangJieID);
		}
	}

	public void RefreshTask(int newTaskID = -1)
	{
		TaskData taskData = null;
		this.TaskCompleted = false;
		this.LevelLimited = false;
		this.taskVO = null;
		if (newTaskID < 0)
		{
			if (this._ActiveTaskClass == 0)
			{
				if (Global.Data.roleData.TaskDataList == null)
				{
					this.AddWaitingTask(TaskClasses.Main);
					return;
				}
				if (!Super.FindHavingMainTask(this._ActiveTaskClass, out taskData))
				{
					this.AddWaitingTask(TaskClasses.Main);
					return;
				}
			}
			else if (this._ActiveTaskClass == 8)
			{
				string text;
				if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.RiChangRenWu, false, out text))
				{
					this.TaskInfo.Text = text;
					this.TaskID = -1;
					return;
				}
				int num;
				int num2;
				if (!Global.CanDoPaoHuanTask(this._ActiveTaskClass, out num, out num2))
				{
					this.TaskInfo.Text = Global.GetLang("您已完成了今日全部的日常任务");
					this.TaskID = -1;
					return;
				}
				if (num <= num2)
				{
					Super.FindHavingMainTask(this._ActiveTaskClass, out taskData);
					if (taskData == null)
					{
						this.AddWaitingTask(TaskClasses.DailyTask);
						return;
					}
				}
			}
		}
		else
		{
			taskData = Global.GetTaskDataByID(newTaskID);
		}
		if (taskData == null)
		{
			this.TaskInfo.Text = null;
			this.ShowLastMainTask();
			return;
		}
		this.taskVO = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
		string title = this.taskVO.Title;
		int taskClass = this.taskVO.TaskClass;
		string taskClassName = Super.GetTaskClassName(taskClass);
		bool flag = Super.JugeTaskTargetComplete(this.taskVO, 1, taskData.DoingTaskVal1);
		bool flag2 = Super.JugeTaskTargetComplete(this.taskVO, 2, taskData.DoingTaskVal2);
		if (this._ActiveTaskClass != taskClass)
		{
			return;
		}
		string text2 = string.Empty;
		int limitLevel = this.taskVO.LimitLevel;
		int limitZhuanSheng = this.taskVO.LimitZhuanSheng;
		if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
		{
			text2 = string.Format(Global.GetLang("等级达到{0}后\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng));
		}
		else
		{
			this.LevelLimited = true;
		}
		this.RefreshTaskDesc(taskData);
		this.RefreshZhangJieUI(this.taskVO.TaskZhangJieID);
		if (taskData.TaskAwards == null)
		{
			GameInstance.Game.SpriteGetTaskAwards(taskData.DoingTaskID);
		}
		else
		{
			this.RefreshTaskAwardsData(taskData.TaskAwards);
		}
		if (!flag || !flag2)
		{
			bool flag3 = false;
			int taketime = this.taskVO.Taketime;
			if (taketime > 0 && Global.GetCorrectLocalTime() - taskData.AddDateTime >= (long)(taketime * 1000))
			{
				flag3 = true;
				text2 = StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
				{
					title,
					Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						Global.GetLang("【失败】")
					})
				});
			}
			string pubStartTime = this.taskVO.PubStartTime;
			string pubEndTime = this.taskVO.PubEndTime;
			if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
			{
				double num3 = (double)Global.GetCorrectLocalTime();
				double num4 = (double)Global.SafeConvertToTicks(pubStartTime);
				double num5 = (double)Global.SafeConvertToTicks(pubEndTime);
				if (num3 < num4 || num3 > num5)
				{
					flag3 = true;
					text2 = StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
					{
						title,
						Global.GetColorStringForNGUIText(new object[]
						{
							"FF0000",
							Global.GetLang("【失败】")
						})
					});
				}
			}
			if (flag3)
			{
				this.TaskInfo.Text = text2;
				return;
			}
		}
		if (!flag)
		{
			text2 += this.GetTaskInfoPartStr(taskData, this.taskVO, 1);
		}
		else if (!flag2)
		{
			text2 += this.GetTaskInfoPartStr(taskData, this.taskVO, 2);
		}
		else
		{
			this.TaskCompleted = true;
			text2 += Super.GetTaskDestNPCDesc(this.taskVO, false);
			if (text2.Length > 0)
			{
				string taskDestNPCName = Super.GetTaskDestNPCName(this.taskVO);
				if (string.Empty != taskDestNPCName)
				{
					int mapCode = -1;
					int targetType = -1;
					int targetID = -1;
					Super.GetTaskDestNPCID(this.taskVO, ref mapCode, ref targetType, ref targetID);
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, mapCode, -1, -1);
				}
			}
		}
		this._TaskDesc.Text = text2;
	}

	private bool AddWaitingTask(TaskClasses taskClass)
	{
		int taskID = 0;
		this.taskVO = Super.FindNextTask(taskClass);
		this.TaskInfo.Text = null;
		if (this.taskVO == null)
		{
			this.ShowLastMainTask();
			return false;
		}
		string text = string.Empty;
		int limitLevel = this.taskVO.LimitLevel;
		int limitZhuanSheng = this.taskVO.LimitZhuanSheng;
		if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
		{
			text += string.Format(Global.GetLang("等级达到{0}后\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng));
		}
		else
		{
			this.LevelLimited = true;
		}
		this.TaskInfo.Text = Super.GetTaskSourceNPCDesc(this.taskVO);
		if (this.TaskInfo.Text.Length > 0)
		{
			string taskSourceNPCName = Super.GetTaskSourceNPCName(this.taskVO);
			if (string.Empty != taskSourceNPCName)
			{
				int mapCode = -1;
				int targetType = -1;
				int targetID = -1;
				Super.GetTaskSourceNPCID(this.taskVO, out mapCode, out targetType, out targetID);
				this.SetTargetPos(targetType, taskID, targetID, mapCode, -1, -1);
			}
		}
		this._TaskTitle.Text = this.taskVO.Title;
		this.RefreshZhangJieUI(this.taskVO.TaskZhangJieID);
		return true;
	}

	private string GetTaskInfoPartStr(TaskData taskData, XElement taskXmlNode, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskXmlNode, taskData.DoingTaskVal1, TargetID);
		text = Super.GetTaskTargetDesc(taskXmlNode, TargetID);
		if (text.Length > 0)
		{
			string taskTargetName = Super.GetTaskTargetName(taskXmlNode, TargetID);
			if (string.Empty != taskTargetName)
			{
				int num = -1;
				int targetType = -1;
				int targetID = -1;
				int toPosX = -1;
				int toPosY = -1;
				int num2;
				int num3;
				Super.GetTaskTargetID(taskXmlNode, TargetID, out num2, out num, out targetType, out targetID, out num3, false, out toPosX, out toPosY);
				if (num != Global.Data.roleData.MapCode && Global.GetMapType(num) != MapTypes.Normal)
				{
					Super.GetTaskDestNPCID(taskXmlNode, ref num, ref targetType, ref targetID);
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, -1, -1);
				}
				else
				{
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, toPosX, toPosY);
				}
			}
			if (!string.IsNullOrEmpty(taskTargetNum))
			{
				text = StringUtil.substitute("{0}{1}", new object[]
				{
					text,
					Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						taskTargetNum
					})
				});
			}
		}
		return text;
	}

	private string GetTaskInfoPartStr(TaskData taskData, TaskVO taskVO, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskData.DoingTaskVal1, TargetID);
		text = Super.GetTaskTargetDesc(taskVO, TargetID, false);
		if (text.Length > 0)
		{
			string taskTargetName = Super.GetTaskTargetName(taskVO, TargetID);
			if (string.Empty != taskTargetName)
			{
				int num = -1;
				int targetType = -1;
				int targetID = -1;
				int toPosX = -1;
				int toPosY = -1;
				int num2;
				int num3;
				Super.GetTaskTargetID(taskVO, TargetID, out num2, out num, out targetType, out targetID, out num3, false, out toPosX, out toPosY);
				if (num != Global.Data.roleData.MapCode && Global.GetMapType(num) != MapTypes.Normal)
				{
					Super.GetTaskDestNPCID(taskVO, ref num, ref targetType, ref targetID);
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, -1, -1);
				}
				else
				{
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, toPosX, toPosY);
				}
			}
			if (!string.IsNullOrEmpty(taskTargetNum))
			{
				text = StringUtil.substitute("{0}{1}", new object[]
				{
					text,
					Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						taskTargetNum
					})
				});
			}
		}
		return text;
	}

	private void SetTargetPos(int targetType, int taskID, int targetID, int mapCode, int toPosX = -1, int toPosY = -1)
	{
		this.MapCode = mapCode;
		this.TargetType = targetType;
		this.NpcID = targetID;
		this.TaskID = taskID;
		this.ToPosX = toPosX;
		this.ToPosY = toPosY;
		if (this.TargetType == 10001)
		{
			return;
		}
		if (this.TargetType == 10002)
		{
			return;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return;
		}
		Point point;
		if (this.TargetType == 2)
		{
			point = Global.GetMonsterPointByID(this.MapCode, this.NpcID);
		}
		else if (this.TargetType == 3)
		{
			point = Global.GetNPCPointByID(this.MapCode, this.NpcID);
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else
		{
			point = new Point(-1, -1);
		}
		if (point.X >= 0 && point.Y >= 0)
		{
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else if (toPosX >= 0 && toPosY >= 0)
		{
			this.ToPosX = toPosX;
			this.ToPosY = toPosY;
		}
	}

	private bool BodyClick()
	{
		if (!this.LevelLimited)
		{
			return false;
		}
		if (this.TaskID < 0)
		{
			return false;
		}
		if (this.TargetType == 10001)
		{
			GameInstance.Game.SpriteFindBiaoChe();
			return true;
		}
		if (this.TargetType == 10002)
		{
			return true;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return true;
		}
		int mapCode = this.MapCode;
		if (Global.IsGoToKuaFuMap(mapCode))
		{
			PlayZone.GlobalPlayZone.OpenKuafuMapView(this.TargetType, -1, this.NpcID, mapCode, this.ToPosX, this.ToPosY, false, 0, 0, false, false);
			return true;
		}
		if (Global.GetMapType(mapCode) <= MapTypes.Normal || (long)mapCode == ConfigSystemParam.GetSystemParamIntByName("SpecialCopySceneToMonsterRealive"))
		{
			if (Global.IsSpecialNormalMapCode(this.MapCode))
			{
			}
		}
		if (this.TargetType == 3)
		{
			if (Global.Data.roleData.IsFlashPlayer >= 1 && this.TaskID <= 105 && this.TaskID > 100)
			{
				if (Global.Data.GameScene != null)
				{
					Global.Data.GameScene.ExternalCallNpcDialog(this.NpcID, 20000);
				}
				return true;
			}
			if (this._ActiveTaskClass == 8 && this.TaskCompleted && this.NpcID == 119)
			{
				GameInstance.Game.SpriteClickOnNPC(this.MapCode, this.NpcID + 2130706432, this.NpcID);
				return true;
			}
		}
		Global.PlaySoundAudio("Audio/UI/TaskFindRoad", false);
		Global.Data.TargetNpcID = this.NpcID;
		Point pos;
		if (this.TargetType == 2)
		{
			pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else if (this.TargetType == 3)
		{
			pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(this.ToPosX, this.ToPosY);
		}
		if (pos.X < 0 || (pos.Y < 0 && this.ToPosX >= 0 && this.ToPosY >= 0))
		{
			pos.X = this.ToPosX;
			pos.Y = this.ToPosY;
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		Global.Data.TargetNpcID = this.NpcID;
		if (this.TargetType == 2 && Global.Data.roleData.IsFlashPlayer == 0)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		else if (this.TargetType == 3)
		{
			int num = (this.NpcID != 60900) ? 0 : 30;
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120 + num, ExtActionTypes.EXTACTION_NPCDLG);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, true))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		return true;
	}

	public void InitPartData(int npcID, int taskID, int npcExtensionID, int mapCode, bool newTask)
	{
		this.MapCode = mapCode;
		this.TaskID = taskID;
		this.NpcID = npcID;
		this.NPCExtensionID = npcExtensionID;
		this.DbID = -1;
		this.IsCompleted = false;
		this._TaskState.Text = null;
		this.taskVO = ConfigTasks.GetTaskXmlNodeByID(taskID);
		if (this.taskVO == null)
		{
			return;
		}
		int taskClass = this.taskVO.TaskClass;
		if (taskClass == this._ActiveTaskClass)
		{
			this._ActiveTaskClass = taskClass;
			this.RefreshZhangJieUI(this.taskVO.TaskZhangJieID);
			if (newTask)
			{
				this.taskData = null;
				GameInstance.Game.SpriteGetTaskAwards(taskID);
				return;
			}
			this.taskData = Global.GetTaskDataByID(taskID);
			if (this.taskData == null)
			{
				return;
			}
			this.RefreshTaskDesc(this.taskData);
			this.RefreshTaskAwardsData(this.taskData.TaskAwards);
		}
	}

	protected void SetState(int targetType)
	{
		this._Submit.Text = Global.GetLang("立即前往");
	}

	public void RefreshTaskDesc(TaskData taskData)
	{
		if (this.taskVO == null)
		{
			return;
		}
		if (taskData == null)
		{
			return;
		}
		this.DbID = taskData.DbID;
		bool flag = Super.JugeTaskTargetComplete(this.taskVO, 1, taskData.DoingTaskVal1);
		bool flag2 = Super.JugeTaskTargetComplete(this.taskVO, 2, taskData.DoingTaskVal2);
		int taskTargetType = Super.GetTaskTargetType(this.taskVO, 1);
		int taskTargetType2 = Super.GetTaskTargetType(this.taskVO, 2);
		this._TaskTitle.Text = this.taskVO.Title;
		if (flag && flag2)
		{
			if (taskTargetType2 < 0 || this.taskVO.TargetNPC2 < 0)
			{
				this._TaskDesc.text = Super.GetTaskTargetDesc(this.taskVO, 1, true);
			}
			else
			{
				this._TaskDesc.text = Super.GetTaskTargetDesc(this.taskVO, 2, true);
			}
			this.IsCompleted = true;
			this.SetState(0);
		}
		else
		{
			this.IsCompleted = false;
			List<TalkTextNode> taskTalkTextInfo = Super.GetTaskTalkTextInfo(this.TaskID, "DoingTalk");
			string text = string.Empty;
			if (!flag)
			{
				this.TargetType = taskTargetType;
				text = Super.GetTaskTargetDesc(this.taskVO, 1, true);
				text += Super.GetTaskTargetNum(this.taskVO, taskData.DoingTaskVal1, 1);
			}
			else
			{
				this.TargetType = taskTargetType2;
				text = Super.GetTaskTargetDesc(this.taskVO, 2, true);
				text += Super.GetTaskTargetNum(this.taskVO, taskData.DoingTaskVal1, 2);
			}
			this._TaskDesc.text = text;
			this.SetState(this.TargetType);
		}
	}

	public void ResreshTaskAwardsData(int starLevel)
	{
		if (this.taskData != null && this.taskData.TaskAwards != null)
		{
			TaskAwardsData taskAwardsData = new TaskAwardsData();
			this._AwardsList.ItemsSource.Clear();
			taskAwardsData.Experienceaward = this.taskData.TaskAwards.Experienceaward;
			taskAwardsData.YinLiangaward = this.taskData.TaskAwards.YinLiangaward;
			taskAwardsData.Moneyaward = this.taskData.TaskAwards.Moneyaward;
			taskAwardsData.MoJingaward = this.taskData.TaskAwards.MoJingaward;
			taskAwardsData.BindYuanBaoaward = this.taskData.TaskAwards.BindYuanBaoaward;
			UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
		}
	}

	public void RefreshTaskAwardsData(TaskAwardsData taskAwardsData)
	{
		TaskData taskDataByID = Global.GetTaskDataByID(this.TaskID);
		if (this.DbID <= 0 && taskDataByID != null)
		{
			this.DbID = taskDataByID.DbID;
			this.RefreshTaskDesc(taskDataByID);
		}
		TaskAwardsData taskAwardsData2 = new TaskAwardsData();
		this._AwardsList.ItemsSource.Clear();
		taskAwardsData2.Experienceaward = taskAwardsData.Experienceaward;
		taskAwardsData2.YinLiangaward = taskAwardsData.YinLiangaward;
		taskAwardsData2.Moneyaward = taskAwardsData.Moneyaward;
		taskAwardsData2.MoJingaward = taskAwardsData.MoJingaward;
		taskAwardsData2.BindYuanBaoaward = taskAwardsData.BindYuanBaoaward;
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData2, "CTextAwards2");
		this._AwardsGoodsList.ItemsSource.Clear();
		this.AddAwardGoods(this._AwardsGoodsList.ItemsSource, taskAwardsData);
	}

	public void RefreshZhangJieUI(int zhangJieID)
	{
		if (zhangJieID < ConfigTasks.MinZhangJieID || zhangJieID > ConfigTasks.MaxZhangJieID)
		{
			this.ZhangJieID = ConfigTasks.MaxZhangJieID;
		}
		this.ZhangJieID = zhangJieID;
		this._NextPage.gameObject.SetActive(this.ZhangJieID < this.taskVO.TaskZhangJieID);
		this._PrePage.gameObject.SetActive(this.ZhangJieID > ConfigTasks.MinZhangJieID);
		TaskZhangJieVO taskZhangJieVO = ConfigTasks.GetTaskZhangJieVO(this.ZhangJieID);
		if (taskZhangJieVO != null)
		{
			this._ZhangJieTitle.URL = Global.GetGameResTaskZhangJieTitle(taskZhangJieVO.ID);
			this._ZhangJieDesc.Text = taskZhangJieVO.ZhangJieMiaoShu;
			this._BuffGoodsList.ItemsSource.Clear();
			GoodsData gd = Global.GetDummyGoodsData(taskZhangJieVO.GlGoodsID);
			GoodVO goodsVO = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon2(this._BuffGoodsList.ItemsSource, gd, delegate(object s, MouseEvent e)
			{
				double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(gd.GoodsID);
				GTipServiceEx.ShowTip(goodsVO.Title, UIHelper.GetBaseAttributeStr(gd, goodsEquipPropsDoubleList, -1), TipTypes.NormalText, false);
			}, false, "none");
			if (this.ZhangJieID < this.taskVO.TaskZhangJieID)
			{
				this._ProgressBar.Percent = 1.0;
				this._ProgressBar.ProgessText = string.Format("{0}/{0}", taskZhangJieVO.TaskCount);
			}
			else if (this.ZhangJieID == this.taskVO.TaskZhangJieID)
			{
				this._ProgressBar.Percent = (double)this.taskVO.TaskIndexOfZhangJie / (double)taskZhangJieVO.TaskCount;
				this._ProgressBar.ProgessText = string.Format("{0}/{1}", this.taskVO.TaskIndexOfZhangJie, taskZhangJieVO.TaskCount);
				ggoodIcon.GoodImg.ToGrayBitmap = (this.taskVO.TaskIndexOfZhangJie < taskZhangJieVO.TaskCount);
			}
			else
			{
				this._ProgressBar.Percent = 0.0;
				this._ProgressBar.ProgessText = string.Format("0/{0}", taskZhangJieVO.TaskCount);
				ggoodIcon.GoodImg.ToGrayBitmap = true;
			}
		}
	}

	public void OnPreZhangJie(object sender, EventArgs e)
	{
		if (this.ZhangJieID > ConfigTasks.MinZhangJieID)
		{
			this.ZhangJieID--;
			this.RefreshZhangJieUI(this.ZhangJieID);
		}
	}

	public void OnNextZhangJie(object sender, EventArgs e)
	{
		if (this.ZhangJieID < ConfigTasks.MaxZhangJieID)
		{
			this.ZhangJieID++;
			this.RefreshZhangJieUI(this.ZhangJieID);
		}
	}

	protected void AddAwardGoods(ObservableCollection ItemCollection, TaskAwardsData taskAwards)
	{
		List<GoodsData> viewTaskInfoGoodsDataList = new List<GoodsData>();
		UIHelper.ParseAwardsItemList(taskAwards.TaskawardList, ref viewTaskInfoGoodsDataList);
		UIHelper.ParseAwardsItemList(taskAwards.OtherTaskawardList, ref viewTaskInfoGoodsDataList);
		Super.GData.ViewTaskInfoGoodsDataList = viewTaskInfoGoodsDataList;
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				UIHelper.AddGoodsIcon2(ItemCollection, Super.GData.ViewTaskInfoGoodsDataList[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	protected virtual void OnDestory()
	{
	}

	public ShowNetImage _ZhangJieTitle;

	public ShowNetImage _TaskContentBak;

	public TextBlock _TaskTitle;

	public TextBlock _AwardsTitle;

	public TextBlock _DimTitle;

	public TextBlock _TaskDesc;

	public TextBlock _TaskState;

	public TextBlock TaskInfo;

	public TextBlock _ZhangJieDesc;

	public GImgProgressBar _ProgressBar;

	public GButton _Submit;

	public GButton _PrePage;

	public GButton _NextPage;

	public ListBox _AwardsList;

	public ListBox _AwardsGoodsList;

	public ListBox _BuffGoodsList;

	private int _ActiveTaskClass;

	private bool IsCompleted;

	private bool LevelLimited;

	private bool TaskCompleted;

	private int ZhangJieID;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int MapCode = -1;

	public int ToPosX = -1;

	public int ToPosY = -1;

	private int CanTeleport = -1;

	private int NPCExtensionID = -1;

	private int TaskID = -1;

	private int NpcID = -1;

	private int DbID = -1;

	private int TargetType = -1;

	private TaskData taskData;

	private TaskTargetInfo TargetInfo = new TaskTargetInfo();

	public TaskVO taskVO;

	internal enum TaskStates
	{
		None,
		Accepted,
		Faild,
		Complete
	}
}
