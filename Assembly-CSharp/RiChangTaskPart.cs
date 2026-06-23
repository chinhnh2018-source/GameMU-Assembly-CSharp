using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RiChangTaskPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("立即前往");
		this._ShuaXing.Text = Global.GetLang("一键满星");
		this.Ex_YiJianWanCheng.Text = Global.GetLang("一键全部完成");
		if (this.ConstHint != null && this.ConstHint.Length == 5)
		{
			this.ConstHint[0].Text = Global.GetLang("将10环任务提升至5星\n并全部2倍立即完成");
			this.ConstHint[1].Text = Global.GetLang("消耗");
			this.ConstHint[2].Text = Global.GetLang("奖励星级");
			this.ConstHint[3].Text = Global.GetLang("刷星费用");
			this.ConstHint[4].Text = Global.GetLang("倍经验");
			this.ConstHint[4].transform.localPosition = new Vector3(20f, 0f, -1f);
		}
		this._Count.text = string.Format(Global.GetLang("第{0}环"), 1);
		this._Submit2.Label.transform.localPosition = new Vector3(60f, 9f, -1f);
		this.ConstHint[2].Pivot = 5;
		this.ConstHint[3].Pivot = 5;
		this.ConstHint[2].X = 145.0;
		this.ConstHint[3].X = 145.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Bak.URL = Global.GetGameResImageString("taskRichang_bak.jpg");
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClose);
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnWanCheng_Click);
		this._Submit2.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnWanCheng2_Click);
		this._ShuaXing.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnShuaXing_Click);
		this.Ex_YiJianWanCheng.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnYiJianWanCheng_Click);
		this.Ex_YiJianWanCheng.isEnabled = false;
		this.TaskStarInfosNeedJinBi = (int)ConfigSystemParam.GetSystemParamIntByName("TaskStarInfosNeedJinBi");
		this.CompleteTaskNeedYuanBao = (int)ConfigSystemParam.GetSystemParamIntByName("CompleteTaskNeedYuanBao");
		this.CompleteEveryTaskNeedYuanBao = (int)ConfigSystemParam.GetSystemParamIntByName("DoubleExp");
		this.TaskStarInfosNeedJinBi = Math.Max(0, this.TaskStarInfosNeedJinBi);
		this.CompleteTaskNeedYuanBao = Math.Max(0, this.CompleteTaskNeedYuanBao);
		this._NeedMoney.text = string.Format(Global.GetLang("{0}金币"), this.TaskStarInfosNeedJinBi);
		this._EveryTaskNeedZuanShi.text = this.CompleteEveryTaskNeedYuanBao.ToString();
		PlayZone.GlobalPlayZone._RiChangTaskPart = this;
		this._SubmitPos = this._Submit.transform.localPosition;
		this._Submit2Pos = this._Submit2.transform.localPosition;
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

	private void OnWanCheng_Click(object sender, EventArgs e)
	{
		if (this.taskData == null)
		{
			Super.PrccessAutoTaskFindRoad(this.TaskID, false, true, false, false);
			this.OnClose(this, null);
		}
		else if (this.IsCompleted)
		{
			Global.SendEvent("1901", Global.GetLang("完成日常任务次数"));
			GameInstance.Game.SpriteCompleteTask(this.NpcID, this.TaskID, this.DbID, 0);
		}
		else
		{
			Super.PrccessAutoTaskFindRoad(this.TaskID, false, false, false, true);
			this.OnClose(this, null);
		}
	}

	private void OnWanCheng2_Click(object sender, EventArgs e)
	{
		if (this.IsCompleted)
		{
			Global.SendEvent("1901", Global.GetLang("完成日常任务次数"));
			GameInstance.Game.SpriteCompleteTask(this.NpcID, this.TaskID, this.DbID, 2);
		}
	}

	public void NotifySubmitResult(int ret, int npcID, int taskID)
	{
		int num = this.UpdateTaskCountInfo();
		if (this.dailyTaskData != null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("日常任务({0}/{1})完成"), this.dailyTaskData.RecNum, this.maxCount), 0, -1, -1, 0);
		}
		if (Global.CanDoPaoHuanTask(8))
		{
			GameInstance.Game.SpriteClickOnNPC(this.MapCode, npcID, this.NPCExtensionID);
			this._Submit.isEnabled = true;
			this._ShuaXing.isEnabled = true;
		}
		else
		{
			this._Submit.isEnabled = false;
			this._ShuaXing.isEnabled = false;
		}
		this.IsCompleted = false;
	}

	private void OnShuaXing_Click(object sender, EventArgs e)
	{
		if (this._StarLevel.Percent != 1.0)
		{
			Global.SendEvent("1900", Global.GetLang("日常任务刷星次数"));
			GameInstance.Game.SpriteRiChangTaskShuaXing(this.TaskID, this.DbID);
		}
		else
		{
			Super.HintMainText(Global.GetLang("当前星级已达到最大"), 10, 3);
		}
	}

	private void OnYiJianWanCheng_Click(object sender, EventArgs e)
	{
		if ((long)Global.GetVIPLeve() < ConfigSystemParam.GetSystemParamIntByName("VIPRiChangYiJianWanCheng"))
		{
			Super.HintMainText(string.Format(Global.GetLang("成为VIP{0}级用户才可使用此功能！"), ConfigSystemParam.GetSystemParamIntByName("VIPRiChangYiJianWanCheng")), 10, 3);
			return;
		}
		if (this.NeedYuanBao > 0)
		{
			string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "YiJianWanChengRiChang", this.NeedYuanBao);
			string message = string.Format(Global.GetLang("确定花费{0}{1}将剩余日常任务全部提升至5星并完成?"), this.NeedYuanBao, text);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteRiChangTaskYiJianWanCheng(this.NpcID, this.NPCExtensionID);
				}
			}, new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			});
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"Error:不该执行到此处"
			});
		}
	}

	public void RefreshTask(int newTaskID = -1)
	{
		this._Submit.isEnabled = true;
		this._ShuaXing.isEnabled = true;
		this.UpdateTaskCountInfo();
		if (newTaskID < 0)
		{
			if (this._ActiveTaskClass == 8)
			{
				string text;
				if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.RiChangRenWu, false, out text))
				{
					this._Desc.Text = text;
					this.TaskID = -1;
					return;
				}
				int num;
				int num2;
				if (!Global.CanDoPaoHuanTask(this._ActiveTaskClass, out num, out num2))
				{
					this._Desc.Text = Global.GetLang("您已完成了今日全部的日常任务");
					this._Submit.isEnabled = false;
					this._ShuaXing.isEnabled = false;
					this.AnimHand.SetActive(false);
					this.TaskID = -1;
					return;
				}
				if (num <= num2)
				{
					Super.FindHavingMainTask(this._ActiveTaskClass, out this.taskData);
					if (this.taskData == null)
					{
						this.AddWaitingTask(TaskClasses.DailyTask);
						return;
					}
				}
			}
		}
		else
		{
			this.taskData = Global.GetTaskDataByID(newTaskID);
		}
		if (this.taskData == null)
		{
			return;
		}
	}

	private bool AddWaitingTask(TaskClasses taskClass)
	{
		int taskID = 0;
		this.taskVO = Super.FindNextTask(taskClass);
		this._Desc.Text = null;
		if (this.taskVO == null)
		{
			return false;
		}
		string text = string.Empty;
		int limitLevel = this.taskVO.LimitLevel;
		int limitZhuanSheng = this.taskVO.LimitZhuanSheng;
		if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
		{
			text += string.Format(Global.GetLang("等级达到{0}后\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng));
		}
		this._Desc.Text = Super.GetTaskSourceNPCDesc(this.taskVO);
		if (this._Desc.Text.Length > 0)
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
		return true;
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
		if (this.TaskID < 0)
		{
			return false;
		}
		if (this.TargetType == 10001)
		{
			GameInstance.Game.SpriteFindBiaoChe();
			return true;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return true;
		}
		int mapCode = this.MapCode;
		if (this.TargetType == 3 && this._ActiveTaskClass == 8 && this.TaskCompleted && this.NpcID == 119)
		{
			GameInstance.Game.SpriteClickOnNPC(this.MapCode, this.NpcID + 2130706432, this.NpcID);
			return true;
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

	public void RefreshTask(int taskID, bool newTask)
	{
		this.InitPartData(2130706551, taskID, 119, 1, newTask);
	}

	public void InitPartData(int npcID, int taskID, int npcExtensionID, int mapCode, bool newTask)
	{
		this.MapCode = mapCode;
		this.TaskID = taskID;
		this.NpcID = npcID;
		this.NPCExtensionID = npcExtensionID;
		this.DbID = -1;
		this.IsCompleted = false;
		this._Submit.isEnabled = true;
		this._ShuaXing.isEnabled = true;
		this.taskVO = ConfigTasks.GetTaskXmlNodeByID(taskID);
		if (this.taskVO == null)
		{
			return;
		}
		int taskClass = this.taskVO.TaskClass;
		if (taskClass == 8)
		{
			this.TaskClass = taskClass;
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
		if (this.IsCompleted)
		{
			this._Submit.Text = Global.GetLang("普通经验");
			this._Submit2.gameObject.SetActive(true);
		}
		else
		{
			this._Submit.Text = Global.GetLang("立即前往");
			this._Submit2.gameObject.SetActive(false);
		}
		if (this.IsCompleted)
		{
			this._Submit.transform.localPosition = this._Submit2Pos;
			this._Submit2.transform.localPosition = this._SubmitPos;
		}
		else
		{
			this._Submit.transform.localPosition = this._SubmitPos;
			this._Submit2.transform.localPosition = this._Submit2Pos;
		}
	}

	public int UpdateTaskCountInfo()
	{
		int num = 1;
		this.dailyTaskData = Global.FindDailyTaskDataByTaskClass(this.TaskClass);
		if (this.dailyTaskData != null)
		{
			int num2 = this.dailyTaskData.RecNum + 1;
			this.maxCount = Global.GetMaxDailyTaskNum(this.TaskClass, this.dailyTaskData);
			num = Math.Min(num2, this.maxCount);
			this._Count.text = string.Format(Global.GetLang("第{0}环"), num);
			if (this.dailyTaskData.RecTime != Global.GetCorrectDateTime().ToString("yyyy-MM-dd"))
			{
				num = 0;
			}
		}
		this.Ex_Count.text = Global.GetLang("已完成环数:") + ColorCode.EncodingText(string.Format("{0}/{1}", num, this.maxCount), "fffffe");
		if (this.dailyTaskData == null || this.dailyTaskData.RecTime != Global.GetCorrectDateTime().ToString("yyyy-MM-dd"))
		{
			this.NeedYuanBao = (this.CompleteTaskNeedYuanBao + this.CompleteEveryTaskNeedYuanBao) * this.maxCount;
		}
		else
		{
			this.NeedYuanBao = (this.CompleteTaskNeedYuanBao + this.CompleteEveryTaskNeedYuanBao) * (this.maxCount - this.dailyTaskData.RecNum);
		}
		this.NeedYuanBao = Math.Max(0, this.NeedYuanBao);
		this.Ex_NeedZuanShi.text = string.Format("{0}", this.NeedYuanBao);
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "YiJianWanChengRiChang", this.NeedYuanBao, string.Empty);
		this.Ex_YiJianWanCheng.isEnabled = (this.NeedYuanBao > 0);
		return num;
	}

	public void RefreshTaskDesc(TaskData taskData)
	{
		if (taskData == null)
		{
			return;
		}
		this.DbID = taskData.DbID;
		this._StarLevel.Percent = (double)((float)taskData.StarLevel / 5f);
		this._ShuaXing.isEnabled = (taskData.StarLevel < 5);
		this.AnimHand.SetActive(taskData.StarLevel < 5 && Global.Data.roleData.ChangeLifeCount < 2);
		bool flag = Super.JugeTaskTargetComplete(this.taskVO, 1, taskData.DoingTaskVal1);
		bool flag2 = Super.JugeTaskTargetComplete(this.taskVO, 2, taskData.DoingTaskVal2);
		int taskTargetType = Super.GetTaskTargetType(this.taskVO, 1);
		int taskTargetType2 = Super.GetTaskTargetType(this.taskVO, 2);
		if (flag && flag2)
		{
			if (taskTargetType2 < 0 || this.taskVO.TargetNPC2 < 0)
			{
				this._Desc.text = Super.GetTaskTargetDesc(this.taskVO, 1, false) + Global.GetLang("(已完成)");
			}
			else
			{
				this._Desc.text = Super.GetTaskTargetDesc(this.taskVO, 2, false) + Global.GetLang("(已完成)");
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
				text = Super.GetTaskTargetDesc(this.taskVO, 1, false);
				text += Super.GetTaskTargetNum(this.taskVO, taskData.DoingTaskVal1, 1);
			}
			else
			{
				this.TargetType = taskTargetType2;
				text = Super.GetTaskTargetDesc(this.taskVO, 2, false);
				text += Super.GetTaskTargetNum(this.taskVO, taskData.DoingTaskVal1, 2);
			}
			this._Desc.text = text;
			this.SetState(this.TargetType);
		}
		this.UpdateTaskCountInfo();
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
			taskAwardsData.XingHunaward = this.taskData.TaskAwards.XingHunaward;
			taskAwardsData.BindYuanBaoaward = this.taskData.TaskAwards.BindYuanBaoaward;
			if (this.taskData != null)
			{
				TaskStarInfo taskStarInfo = Global.GetTaskStarInfo(this.taskData.StarLevel);
				if (taskStarInfo != null)
				{
					taskAwardsData.Experienceaward = (long)((float)taskAwardsData.Experienceaward * taskStarInfo.ExpModule);
					taskAwardsData.MoJingaward = (int)((float)this.taskData.TaskAwards.MoJingaward * taskStarInfo.BindZuanModule);
					taskAwardsData.XingHunaward = (int)((float)taskAwardsData.XingHunaward * taskStarInfo.XingHunXiShu);
				}
			}
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
		taskAwardsData2.XingHunaward = taskAwardsData.XingHunaward;
		taskAwardsData2.BindYuanBaoaward = taskAwardsData.BindYuanBaoaward;
		taskAwardsData2.XingHunaward = taskAwardsData.XingHunaward;
		if (taskDataByID != null)
		{
			TaskStarInfo taskStarInfo = Global.GetTaskStarInfo(taskDataByID.StarLevel);
			if (taskStarInfo != null)
			{
				taskAwardsData2.Experienceaward = (long)((float)taskAwardsData2.Experienceaward * taskStarInfo.ExpModule);
				taskAwardsData2.MoJingaward = (int)((float)taskAwardsData2.MoJingaward * taskStarInfo.BindZuanModule);
				taskAwardsData2.XingHunaward = (int)((float)taskAwardsData2.XingHunaward * taskStarInfo.XingHunXiShu);
			}
		}
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData2, "CTextAwards2");
		this._AwardsGoodsList.ItemsSource.Clear();
		this.AddAwardGoods(this._AwardsGoodsList.ItemsSource, taskAwardsData);
		taskAwardsData2 = new TaskAwardsData();
		taskAwardsData2.Experienceaward = (long)taskAwardsData.AddExperienceForDailyCircleTask;
		taskAwardsData2.XingHunaward = taskAwardsData.AddMoJingForDailyCircleTask;
		this.Ex_AwardsList.ItemsSource.Clear();
		UIHelper.AddAwardData(this.Ex_AwardsList.ItemsSource, taskAwardsData2, "CTextAwards2");
		this.Ex_AwardsGoodsList.ItemsSource.Clear();
		if (!string.IsNullOrEmpty(taskAwardsData.AddGoodsForDailyCircleTask))
		{
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(taskAwardsData.AddGoodsForDailyCircleTask, 0, int.MaxValue);
			UIHelper.AddAwardGoods2(this.Ex_AwardsGoodsList.ItemsSource, goodsList, null, false, "bagGrid4_bak");
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

	public void NotifyYiJianWanChengResult(int ret)
	{
		if (ret != -22 && ret != -21)
		{
			if (ret != -3)
			{
				if (ret != 1)
				{
					Super.HintMainText(string.Format(Global.GetLang("一键完成10环日常任务出错,未知错误({0})"), ret), 10, 3);
				}
				else
				{
					this.OnClose(null, null);
					Super.HintMainText(string.Format(Global.GetLang("成功完成10环日常任务"), new object[0]), 10, 3);
				}
			}
			else
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
		}
		else
		{
			long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("VIPRiChangYiJianWanCheng");
			Super.HintMainText(string.Format(Global.GetLang("VIP{0}及以上玩家才可使用一键完成功能!"), systemParamIntByName), 10, 3);
		}
	}

	public void RefreshStarAnim(int starLevel)
	{
		for (int i = 0; i < 5; i++)
		{
			if (i < starLevel)
			{
				this.FiveStarAnims[i].gameObject.SetActive(true);
				this.FiveStarAnims[i].Play();
			}
			else
			{
				this.FiveStarAnims[i].gameObject.SetActive(false);
			}
		}
		if (starLevel == 5)
		{
			this.CircleAnim.gameObject.SetActive(true);
			this.CircleAnim.Play();
		}
		else
		{
			this.CircleAnim.gameObject.SetActive(false);
		}
	}

	public void NotifyShuaXingResult(int ret, int taskID, int starLevel)
	{
		if (ret == 1)
		{
			this.taskData = Global.GetTaskDataByID(taskID);
			if (this.taskData != null)
			{
				this.taskData.StarLevel = starLevel;
				this.AnimHand.SetActive(this.taskData.StarLevel < 5 && Global.Data.roleData.ChangeLifeCount < 2);
				this._ShuaXing.isEnabled = (this.taskData.StarLevel < 5);
			}
			this.RefreshStarAnim(starLevel);
			this._StarLevel.Percent = (double)((float)starLevel / 5f);
			this.ResreshTaskAwardsData(starLevel);
			Super.HintMainText(string.Format(Global.GetLang("刷新任务星级为{0}星"), starLevel), 10, 3);
		}
		else if (ret == -4)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
		}
		else
		{
			Super.HintMainText(string.Format(Global.GetLang("刷星失败: {0}"), ret), 10, 3);
		}
	}

	protected override void OnDestroy()
	{
		PlayZone.GlobalPlayZone._RiChangTaskPart = null;
	}

	private const int ConstHideAnimOnStarLevel = 5;

	public ShowNetImage _Bak;

	public GButton _Close;

	public TextBlock _Title;

	public GButton _Submit;

	public GButton _Submit2;

	public TextBlock _Count;

	public GButton _ShuaXing;

	public ListBox _AwardsList;

	public ListBox _AwardsGoodsList;

	public ListBox Ex_AwardsList;

	public ListBox Ex_AwardsGoodsList;

	public TextBlock _Desc;

	public GImgProgressBar _StarLevel;

	public TextBlock _NeedMoney;

	public TextBlock _EveryTaskNeedZuanShi;

	public TextBlock Ex_NeedZuanShi;

	public GButton Ex_YiJianWanCheng;

	public TextBlock Ex_Count;

	public List<UISprite> listDaiBi = new List<UISprite>();

	public GameObject AnimHand;

	public TextBlock[] ConstHint;

	private int TaskClass = 8;

	private bool IsCompleted;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int MapCode = -1;

	public int ToPosX = -1;

	public int ToPosY = -1;

	private int CanTeleport = -1;

	private int NPCExtensionID = -1;

	private int TaskID = -1;

	private int NpcID = -1;

	private int DbID = -1;

	private int NeedYuanBao;

	private int TargetType = -1;

	private int CompleteTaskNeedYuanBao;

	private int TaskStarInfosNeedJinBi;

	private int CompleteEveryTaskNeedYuanBao;

	private Vector3 _SubmitPos = Vector3.zero;

	private Vector3 _Submit2Pos = Vector3.zero;

	public Animation CircleAnim;

	public Animation[] FiveStarAnims;

	private int maxCount = 10;

	private DailyTaskData dailyTaskData;

	private TaskData taskData;

	private TaskTargetInfo TargetInfo = new TaskTargetInfo();

	public TaskVO taskVO;

	private int _ActiveTaskClass = 8;

	private bool TaskCompleted;

	internal enum TaskStates
	{
		None,
		Accepted,
		Faild,
		Complete
	}
}
