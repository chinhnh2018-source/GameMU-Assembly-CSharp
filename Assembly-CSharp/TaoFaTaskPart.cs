using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TaoFaTaskPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._ExAwardsList.X = -70.0;
		this.staticTxt.text = Global.GetLang("全部完成额外奖励");
		this._Submit.Text = Global.GetLang("立即前往");
		this.Finish_Count.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		PlayZone.GlobalPlayZone._TaoFaTaskPart = this;
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnButton_Click);
	}

	private void OnButton_Click(object sender, EventArgs e)
	{
		if (!this.isComplete)
		{
			Super.PrccessAutoTaskFindRoad(this.currentTaskID, false, true, false, false);
			this.OnClose(this, null);
		}
		else
		{
			Global.SendEvent("1901", Global.GetLang("完成日常任务次数"));
			if (this.taskData != null)
			{
				GameInstance.Game.SpriteCompleteTask(this.NpcID, this.currentTaskID, this.taskData.DbID, 0);
			}
		}
	}

	public void ShowTaskPart(int npcID, int taskID, int npcExtensionID, int mapCode, bool newTask)
	{
		this.NpcID = npcID;
		this.InitPartData(taskID, newTask);
	}

	public void InitPartData(int taskID = -1, bool newTask = false)
	{
		this.Back.GetComponent<ShowNetImage>().URL = "NetImages/GameRes/Images/Plate/taofaback.png";
		this._Submit.isEnabled = true;
		string empty = string.Empty;
		if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.TaoFaRenWu, false, out empty))
		{
			this._TaskDesc.text = empty;
			return;
		}
		int num;
		int num2;
		if (Global.CanDoPaoHuanTask(9, out num, out num2))
		{
			this.RefreshTask(taskID, newTask);
			return;
		}
		this.AllFinish();
	}

	public void RefreshTask(int taskID = -1, bool newTask = false)
	{
		if (taskID < 0)
		{
			Super.FindHavingMainTask(9, out this.taskData);
			if (this.taskData != null)
			{
				taskID = this.taskData.DoingTaskID;
			}
		}
		else
		{
			this.taskData = Global.GetTaskDataByID(taskID);
		}
		this.currentTaskID = taskID;
		if (newTask)
		{
			this.taskData = null;
			GameInstance.Game.SpriteGetTaskAwards(taskID);
			return;
		}
		if (this.taskData == null)
		{
			return;
		}
		this.RefreshAll(this.taskData.TaskAwards);
	}

	public void RefreshAll(TaskAwardsData TaskAwards)
	{
		this.taskData = Global.GetTaskDataByID(this.currentTaskID);
		if (this.taskData != null)
		{
			this.taskInfo = ConfigTasks.GetTaskXmlNodeByID(this.taskData.DoingTaskID);
			if (this.taskInfo != null)
			{
				this.RefreshTaskDesc(this.taskData, this.taskInfo);
				if (this.Modal3D.transform.childCount == 0)
				{
					MonsterVO monsterVO;
					ConfigMonsters.MonsterXmlNode.TryGetValue(this.taskInfo.TargetNPC1, ref monsterVO);
					this.resLoader = UIHelper.LoadMonsterRes(this.Modal3D, this.taskInfo.TargetNPC1, monsterVO.Scale);
				}
			}
		}
		this.RefreshTaskAwardsData(TaskAwards);
	}

	public void RefreshTaskAwardsData(TaskAwardsData awardsData)
	{
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		TaskAwardsData taskAwardsData2 = new TaskAwardsData();
		this._AwardsList.ItemsSource.Clear();
		this._ExAwardsList.ItemsSource.Clear();
		this._AwardsGoodsList.ItemsSource.Clear();
		taskAwardsData.Experienceaward = awardsData.Experienceaward;
		taskAwardsData.YinLiangaward = awardsData.YinLiangaward;
		taskAwardsData.Moneyaward = awardsData.Moneyaward;
		taskAwardsData.XingHunaward = awardsData.XingHunaward;
		taskAwardsData.BindYuanBaoaward = awardsData.BindYuanBaoaward;
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
		taskAwardsData2.BindYuanBaoaward = (int)ConfigSystemParam.GetSystemParamIntByName("PriceTaskAward");
		UIHelper.AddAwardData(this._ExAwardsList.ItemsSource, taskAwardsData2, "CTextAwards2");
		this.AddAwardGoods(this._AwardsGoodsList.ItemsSource, awardsData);
	}

	public void RefreshTaskDesc(TaskData taskData, TaskVO taskInfo)
	{
		bool flag = Super.JugeTaskTargetComplete(taskInfo, 1, taskData.DoingTaskVal1);
		bool flag2 = Super.JugeTaskTargetComplete(taskInfo, 2, taskData.DoingTaskVal2);
		if (flag && flag2)
		{
			this.isComplete = true;
		}
		else
		{
			this.isComplete = false;
		}
		if (taskData == null)
		{
			return;
		}
		string text = string.Empty;
		if (taskInfo != null)
		{
			text = Super.GetTaskTargetDesc(taskInfo, 1, false);
			text += Super.GetTaskTargetNum(taskInfo, taskData.DoingTaskVal1, 1);
			if (this.isComplete)
			{
				text += Global.GetLang("已完成");
			}
		}
		this._TaskDesc.text = text;
		if (!this.isComplete)
		{
			this._Submit.Text = Global.GetLang("立即前往");
		}
		else
		{
			this._Submit.Text = Global.GetLang("立即完成");
		}
		this.UpdateTaskCountInfo();
	}

	public int UpdateTaskCountInfo()
	{
		int num = 1;
		int num2 = 5;
		DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(9);
		if (dailyTaskData != null)
		{
			int num3 = dailyTaskData.RecNum + 1;
			num2 = Global.GetMaxDailyTaskNum(9, dailyTaskData);
			num = Math.Min(num3, num2);
			this._TaskTitle.Text = string.Format(Global.GetLang("第{0}环"), num);
			if (dailyTaskData.RecTime != Global.GetCorrectDateTime().ToString("yyyy-MM-dd"))
			{
				num = 0;
			}
			this.Finish_Count.Text = Global.GetLang("已完成环数:") + ColorCode.EncodingText(string.Format("{0}/{1}", num, num2), "fffffe");
		}
		else
		{
			this.Finish_Count.Text = Global.GetLang("已完成环数:") + ColorCode.EncodingText(string.Format("{0}/{1}", num, num2), "fffffe");
			this._TaskTitle.Text = string.Format(Global.GetLang("第{0}环"), num);
		}
		return num;
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

	public void NotifySubmitResult(int ret, int npcID, int taskID)
	{
		this.Modal3D.Clear();
		if (Global.CanDoPaoHuanTask(9))
		{
			PlayZone.GlobalPlayZone.ClickNPC(120, 1);
		}
		else
		{
			this.AllFinish();
		}
	}

	public void AllFinish()
	{
		this._AwardsList.ItemsSource.Clear();
		this._TaskDesc.text = Global.GetLang("您已完成了今日全部的讨伐任务");
		this.UpdateTaskCountInfo();
		this._Submit.isEnabled = false;
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

	protected override void OnDestroy()
	{
		if (this.resLoader != null)
		{
			this.resLoader.Stop();
		}
		PlayZone.GlobalPlayZone._TaoFaTaskPart = null;
	}

	public TextBlock _TaskTitle;

	public TextBlock _TaskDesc;

	public GButton _Submit;

	public ListBox _AwardsList;

	public ListBox _ExAwardsList;

	public ListBox _AwardsGoodsList;

	public ListBox _FinishAwardsList;

	public TextBlock Finish_Count;

	public Modal3DShow Modal3D;

	public GameObject Back;

	public TaskData taskData;

	public TaskVO taskInfo;

	public bool isComplete;

	public int currentTaskID = -1;

	public int NpcID = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock staticTxt;

	private MonsterNPCResLoader resLoader;
}
