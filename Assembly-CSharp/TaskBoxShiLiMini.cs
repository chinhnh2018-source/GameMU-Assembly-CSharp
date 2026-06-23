using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TaskBoxShiLiMini : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnRenWu.Text = Global.GetLang("势力任务");
		this.btnBoss.Text = Global.GetLang("BOSS");
		this.lblWordDemage.text = Global.GetLang("我的伤害:");
		this.lblTongMengName.text = Global.GetLang("同盟");
		this.lblJiaoTuanName.text = Global.GetLang("教团");
		this.lblXieHuiName.text = Global.GetLang("协会");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnRenWu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(TaskBoxShiLiMini.TaskShiLiSelectType.Renwu);
		};
		this.btnBoss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(TaskBoxShiLiMini.TaskShiLiSelectType.Boss);
		};
		this.AddEventLinster();
		this.Init();
	}

	public void RefreshData()
	{
		ShiLiData.RefreshReWuWindow();
		if (this.m_lstTasks.Count > 0)
		{
			for (int i = 0; i < this.m_lstTasks.Count; i++)
			{
				Object.Destroy(this.m_lstTasks[i].gameObject);
			}
		}
		this.m_lstTasks.Clear();
		List<TaskData> taskDataList = GameInstance.Game.CurrentSession.roleData.TaskDataList;
		List<TaskData> list = new List<TaskData>();
		if (taskDataList == null)
		{
			return;
		}
		for (int j = 0; j < taskDataList.Count; j++)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskDataList[j].DoingTaskID);
			if (taskXmlNodeByID == null)
			{
				if (taskDataList[j].DoingTaskID != -1)
				{
					MUDebug.LogError<string>(new string[]
					{
						"未找到任务配置:id" + taskDataList[j].DoingTaskID
					});
				}
			}
			else if (ShiLiData.BeShiLiTask(taskXmlNodeByID.TaskClass))
			{
				list.Add(taskDataList[j]);
			}
		}
		this.ReSortTasks(list);
		for (int k = 0; k < list.Count; k++)
		{
			TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(list[k].DoingTaskID);
			ShiLiTaskInfo shiLiTaskInfo = U3DUtils.NEW<ShiLiTaskInfo>();
			shiLiTaskInfo.gameObject.name = "task" + k;
			shiLiTaskInfo.transform.SetParent(this.taskGrid.transform);
			shiLiTaskInfo.transform.localScale = Vector3.one;
			shiLiTaskInfo.transform.localPosition = new Vector3(0f, (float)(0 - 58 * k), 0f);
			UIEventListener.Get(shiLiTaskInfo.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTask);
			this.m_lstTasks.Add(shiLiTaskInfo);
			shiLiTaskInfo.InitTask(list[k], taskXmlNodeByID2);
		}
		SpringPanel.Begin(this.panelDrag.gameObject, Vector3.zero, 10f);
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

	public void RefreshTask(TaskData taskData)
	{
		ShiLiTaskInfo taskInfo = this.GetTaskInfo(taskData);
		if (taskInfo != null)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			taskInfo.InitTask(taskData, taskXmlNodeByID);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"找不到任务"
			});
		}
	}

	public void AddNewTask(TaskData taskData)
	{
		ShiLiTaskInfo shiLiTaskInfo = this.GetTaskInfo(taskData);
		if (shiLiTaskInfo != null)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			shiLiTaskInfo.InitTask(taskData, taskXmlNodeByID);
		}
		else
		{
			TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			shiLiTaskInfo = U3DUtils.NEW<ShiLiTaskInfo>();
			int count = this.m_lstTasks.Count;
			shiLiTaskInfo.gameObject.name = "task" + count;
			shiLiTaskInfo.transform.SetParent(this.taskGrid.transform);
			shiLiTaskInfo.transform.localScale = Vector3.one;
			shiLiTaskInfo.transform.localPosition = new Vector3(0f, (float)(0 - 58 * count), 0f);
			UIEventListener.Get(shiLiTaskInfo.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTask);
			this.m_lstTasks.Add(shiLiTaskInfo);
			shiLiTaskInfo.InitTask(taskData, taskXmlNodeByID2);
		}
	}

	private ShiLiTaskInfo GetTaskInfo(TaskData taskData)
	{
		return this.m_lstTasks.Find((ShiLiTaskInfo info) => info.TaskData.DoingTaskID == taskData.DoingTaskID);
	}

	private void OnClickTask(GameObject taskGameObject)
	{
		if (this.OnSelectTask != null)
		{
			ShiLiTaskInfo component = taskGameObject.GetComponent<ShiLiTaskInfo>();
			if (component != null)
			{
				this.OnSelectTask.Invoke(component.TaskData);
			}
		}
	}

	public void Init()
	{
		this.m_lstSldiers.Clear();
		this.m_lstPosition.Clear();
		this.m_lstSldiers.Add(this.sliderTongMeng);
		this.m_lstPosition.Add(this.sliderTongMeng.transform.parent.transform.localPosition);
		this.m_lstSldiers.Add(this.sliderJiaoTuan);
		this.m_lstPosition.Add(this.sliderJiaoTuan.transform.parent.transform.localPosition);
		this.m_lstSldiers.Add(this.sliderXieHui);
		this.m_lstPosition.Add(this.sliderXieHui.transform.parent.transform.localPosition);
		this.initData();
		this.OnSelectType(TaskBoxShiLiMini.TaskShiLiSelectType.Renwu);
	}

	private void initData()
	{
		this.SetBossTotalHP(1000L);
		this.SetBossDemage(ShiLiType.ShenShengJiaoTuan, 0L);
		this.SetBossDemage(ShiLiType.ZiYouTongMeng, 0L);
		this.SetBossDemage(ShiLiType.ZhiMengXieHui, 0L);
		this.RefeshAllSliders();
		this.lblDemageValue.text = "0";
	}

	private void OnSelectType(TaskBoxShiLiMini.TaskShiLiSelectType type)
	{
		if (this.m_selectType == type)
		{
			return;
		}
		this.m_selectType = type;
		if (this.m_selectType == TaskBoxShiLiMini.TaskShiLiSelectType.Renwu)
		{
			this.SetButtonSprite(this.btnRenWu, "mainTaskTab");
			this.SetButtonSprite(this.btnBoss, "mainTaskBak_normal");
			this.objRenWu.SetActive(true);
			this.objBoss.SetActive(false);
		}
		else if (this.m_selectType == TaskBoxShiLiMini.TaskShiLiSelectType.Boss)
		{
			this.SetButtonSprite(this.btnBoss, "mainTaskTab");
			this.SetButtonSprite(this.btnRenWu, "mainTaskBak_normal");
			this.objRenWu.SetActive(false);
			this.objBoss.SetActive(true);
		}
	}

	public void SetBossDemage(ShiLiType type, long demage)
	{
		if (demage >= this.m_bossTotalHP / 2L)
		{
			this.m_maxValue = demage;
		}
		else
		{
			this.m_maxValue = this.m_bossTotalHP / 2L;
		}
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			this.m_jiaoTuanDemage = demage;
			break;
		case ShiLiType.ZiYouTongMeng:
			this.m_tongMengDemage = demage;
			break;
		case ShiLiType.ZhiMengXieHui:
			this.m_XieHuiDemage = demage;
			break;
		}
	}

	private void SetBossTotalHP(long bossHP)
	{
		this.m_bossTotalHP = bossHP;
	}

	private void RefeshAllSliders()
	{
		this.sliderTongMeng.sliderValue = (float)this.m_tongMengDemage * 1f / (float)this.m_maxValue;
		this.sliderJiaoTuan.sliderValue = (float)this.m_jiaoTuanDemage * 1f / (float)this.m_maxValue;
		this.sliderXieHui.sliderValue = (float)this.m_XieHuiDemage * 1f / (float)this.m_maxValue;
		this.ReSortSlider();
		for (int i = 0; i < this.m_lstSldiers.Count; i++)
		{
			this.m_lstSldiers[i].transform.parent.localPosition = this.m_lstPosition[i];
		}
	}

	private void ReSortSlider()
	{
		this.m_lstSldiers.Sort((UISlider s1, UISlider s2) => s2.sliderValue.CompareTo(s1.sliderValue));
	}

	private void SetButtonSprite(GButton btn, string spriteName)
	{
		btn.normalSprite = spriteName;
		btn.hoverSprite = spriteName;
		btn.pressedSprite = spriteName;
		btn.target.spriteName = spriteName;
	}

	private new void OnDestroy()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<CompBattleScoreData>("CMD_SPR_COMP_SIDE_SCORE", new Action<CompBattleScoreData>(this.ServerSidSocre));
		MUEventManager.AddEventListener<long>("CMD_SPR_COMP_SELF_SCORE", new Action<long>(this.ServerSelfSocre));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompBattleScoreData>("CMD_SPR_COMP_SIDE_SCORE", new Action<CompBattleScoreData>(this.ServerSidSocre));
		MUEventManager.AddEventListener<long>("CMD_SPR_COMP_SELF_SCORE", new Action<long>(this.ServerSelfSocre));
	}

	private void ServerSidSocre(CompBattleScoreData data)
	{
		if (data == null)
		{
			return;
		}
		this.SetBossTotalHP(data.BossHP);
		this.SetBossDemage(ShiLiType.ShenShengJiaoTuan, data.Score1);
		this.SetBossDemage(ShiLiType.ZiYouTongMeng, data.Score2);
		this.SetBossDemage(ShiLiType.ZhiMengXieHui, data.Score3);
		this.RefeshAllSliders();
	}

	private void ServerSelfSocre(long score)
	{
		this.lblDemageValue.text = score.ToString();
	}

	private new const int height = 58;

	private const string SelectStr = "mainTaskTab";

	private const string NormalStr = "mainTaskBak_normal";

	public Action<TaskData> OnSelectTask;

	public GButton btnRenWu;

	public GButton btnBoss;

	public GameObject objRenWu;

	public GameObject objBoss;

	public UILabel lblTongMengName;

	public UILabel lblJiaoTuanName;

	public UILabel lblXieHuiName;

	public UISlider sliderTongMeng;

	public UISlider sliderJiaoTuan;

	public UISlider sliderXieHui;

	public UILabel lblWordDemage;

	public UILabel lblDemageValue;

	public GameObject taskGrid;

	public UIDraggablePanel panelDrag;

	private List<ShiLiTaskInfo> m_lstTasks = new List<ShiLiTaskInfo>();

	private TaskBoxShiLiMini.TaskShiLiSelectType m_selectType;

	private long m_tongMengDemage;

	private long m_jiaoTuanDemage;

	private long m_XieHuiDemage;

	private long m_maxValue = 1000L;

	private long m_bossTotalHP = 1000L;

	private List<UISlider> m_lstSldiers = new List<UISlider>();

	private List<Vector3> m_lstPosition = new List<Vector3>();

	public enum TaskShiLiSelectType
	{
		None,
		Renwu,
		Boss
	}
}
