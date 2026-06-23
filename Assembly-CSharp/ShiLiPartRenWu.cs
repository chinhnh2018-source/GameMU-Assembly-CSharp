using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiPartRenWu : UserControl
{
	private void InitTextInPrefabs()
	{
		Global.SetSprite(base.gameObject);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			ShiLiData.OpenHelpWindow(ShiLiHelpType.HelpCompIntroTask);
		};
		this.InitTasks();
	}

	private void InitTasks()
	{
		List<TaskData> taskDataList = GameInstance.Game.CurrentSession.roleData.TaskDataList;
		List<DailyTaskData> myDailyTaskDataList = GameInstance.Game.CurrentSession.roleData.MyDailyTaskDataList;
		List<TaskData> list = new List<TaskData>();
		List<int> list2 = new List<int>();
		if (taskDataList == null)
		{
			return;
		}
		for (int i = 0; i < taskDataList.Count; i++)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskDataList[i].DoingTaskID);
			if (taskXmlNodeByID == null)
			{
				if (taskDataList[i].DoingTaskID != -1)
				{
					MUDebug.LogError<string>(new string[]
					{
						"未找到任务配置:id" + taskDataList[i].DoingTaskID
					});
				}
			}
			else if (ShiLiData.BeShiLiTask(taskXmlNodeByID.TaskClass))
			{
				list.Add(taskDataList[i]);
			}
		}
		for (int j = 0; j < myDailyTaskDataList.Count; j++)
		{
			if (ShiLiData.BeShiLiTask(myDailyTaskDataList[j].TaskClass))
			{
				int taskNum = ShiLiData.GetTaskNum(myDailyTaskDataList[j].TaskClass);
				if (taskNum > 0)
				{
					if (myDailyTaskDataList[j].RecNum >= taskNum)
					{
						int selfCompType = ShiLiData.GetSelfCompType();
						int taskIdByNum = ShiLiData.GetTaskIdByNum((ShiLiType)selfCompType, myDailyTaskDataList[j].TaskClass, taskNum - 1);
						if (ConfigTasks.GetTaskXmlNodeByID(taskIdByNum) == null)
						{
							MUDebug.LogError<string>(new string[]
							{
								"未找到任务配置:id" + taskIdByNum
							});
						}
						else
						{
							list2.Add(taskIdByNum);
						}
					}
				}
			}
		}
		this.ReSortTasks(list);
		for (int k = 0; k < list.Count; k++)
		{
			TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(list[k].DoingTaskID);
			ShiLiPartRenWuTaskItem shiLiPartRenWuTaskItem = U3DUtils.NEW<ShiLiPartRenWuTaskItem>();
			shiLiPartRenWuTaskItem.gameObject.name = "task" + (this.m_lstTasks.Count + 1);
			shiLiPartRenWuTaskItem.transform.SetParent(this.taskGrid.transform);
			shiLiPartRenWuTaskItem.transform.localScale = Vector3.one;
			shiLiPartRenWuTaskItem.transform.localPosition = new Vector3((float)(this.m_lstTasks.Count * 310), 0f, 0f);
			this.m_lstTasks.Add(shiLiPartRenWuTaskItem);
			shiLiPartRenWuTaskItem.OnLingQuJingLi = new Action<TaskData>(this.OnClickLingQu);
			shiLiPartRenWuTaskItem.InitTask(list[k], taskXmlNodeByID2);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			TaskVO taskXmlNodeByID3 = ConfigTasks.GetTaskXmlNodeByID(list2[l]);
			ShiLiPartRenWuTaskItem shiLiPartRenWuTaskItem2 = U3DUtils.NEW<ShiLiPartRenWuTaskItem>();
			shiLiPartRenWuTaskItem2.gameObject.name = "task" + (this.m_lstTasks.Count + 1);
			shiLiPartRenWuTaskItem2.transform.SetParent(this.taskGrid.transform);
			shiLiPartRenWuTaskItem2.transform.localScale = Vector3.one;
			shiLiPartRenWuTaskItem2.transform.localPosition = new Vector3((float)(this.m_lstTasks.Count * 310), 0f, 0f);
			this.m_lstTasks.Add(shiLiPartRenWuTaskItem2);
			shiLiPartRenWuTaskItem2.OnLingQuJingLi = new Action<TaskData>(this.OnClickLingQu);
			shiLiPartRenWuTaskItem2.InitCompleteTask(taskXmlNodeByID3);
		}
	}

	public void JiangLiGet(int taskId)
	{
		for (int i = 0; i < this.m_lstTasks.Count; i++)
		{
			ShiLiPartRenWuTaskItem shiLiPartRenWuTaskItem = this.m_lstTasks[i];
			if (shiLiPartRenWuTaskItem.TaskData != null)
			{
				if (shiLiPartRenWuTaskItem.TaskData.DoingTaskID == taskId)
				{
					shiLiPartRenWuTaskItem.SetGotJingLi();
					break;
				}
			}
		}
	}

	private void OnClickLingQu(TaskData taskData)
	{
		int num = 0;
		int num2 = 0;
		int npcID = 0;
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
		Super.GetTaskDestNPCID(taskXmlNodeByID, ref num, ref num2, ref npcID);
		GameInstance.Game.SpriteCompleteTask(npcID, taskData.DoingTaskID, taskData.DbID, 0);
	}

	private void ReSortTasks(List<TaskData> shiLiTasks)
	{
		shiLiTasks.Sort(delegate(TaskData task1, TaskData task2)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(task1.DoingTaskID);
			TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(task2.DoingTaskID);
			bool flag = Super.JugeTaskComplete(taskXmlNodeByID, task1.DoingTaskVal1, task1.DoingTaskVal2);
			bool flag2 = Super.JugeTaskComplete(taskXmlNodeByID2, task2.DoingTaskVal1, task2.DoingTaskVal2);
			if (flag && !flag2)
			{
				return -1;
			}
			if (!flag && flag2)
			{
				return 1;
			}
			return task1.DoingTaskID - task2.DoingTaskID;
		});
	}

	public void Refresh()
	{
		if (this.m_lstTasks.Count > 0)
		{
			for (int i = 0; i < this.m_lstTasks.Count; i++)
			{
				Object.Destroy(this.m_lstTasks[i].gameObject);
			}
		}
		this.m_lstTasks.Clear();
		this.InitTasks();
		SpringPanel.Begin(this.panelDrag.gameObject, Vector3.zero, 0f);
	}

	public void TestInit(int num)
	{
		for (int i = 0; i < num; i++)
		{
			ShiLiPartRenWuTaskItem shiLiPartRenWuTaskItem = U3DUtils.NEW<ShiLiPartRenWuTaskItem>();
			shiLiPartRenWuTaskItem.gameObject.name = "task" + i;
			shiLiPartRenWuTaskItem.transform.SetParent(this.taskGrid.transform);
			shiLiPartRenWuTaskItem.transform.localScale = Vector3.one;
			shiLiPartRenWuTaskItem.transform.localPosition = Vector3.zero;
			this.m_lstTasks.Add(shiLiPartRenWuTaskItem);
		}
		this.taskGrid.Reposition();
	}

	private new void Update()
	{
		if (this.panleContent.clipRange.x > this.taskGrid.cellWidth / 2f)
		{
			if (!this.imgZuoJianTou.gameObject.activeSelf)
			{
				this.imgZuoJianTou.gameObject.SetActive(true);
			}
		}
		else if (this.imgZuoJianTou.gameObject.activeSelf)
		{
			this.imgZuoJianTou.gameObject.SetActive(false);
		}
		if (this.panleContent.clipRange.x > this.taskGrid.cellWidth * ((float)this.m_lstTasks.Count - 3.5f))
		{
			if (this.imgYouJianTou.gameObject.activeSelf)
			{
				this.imgYouJianTou.gameObject.SetActive(false);
			}
		}
		else if (!this.imgYouJianTou.gameObject.activeSelf)
		{
			this.imgYouJianTou.gameObject.SetActive(true);
		}
	}

	private const int CellWidth = 310;

	public DPSelectedItemEventHandler CloseHandler;

	public GButton btnClose;

	public GButton btnHelp;

	public UIPanel panleContent;

	public UIGrid taskGrid;

	public UISprite imgZuoJianTou;

	public UISprite imgYouJianTou;

	public UIDraggablePanel panelDrag;

	private List<ShiLiPartRenWuTaskItem> m_lstTasks = new List<ShiLiPartRenWuTaskItem>();
}
