using System;
using Server.Data;
using UnityEngine;

public class ZhiXianTaskItem : UserControl
{
	public void InitData(string content, TaskData taskData)
	{
		this.mContent.Text = content;
		this.mTaskData = taskData;
		this.taskVo = ConfigTasks.GetTaskXmlNodeByID(this.mTaskData.DoingTaskID);
	}

	public bool Selected
	{
		set
		{
			if (value)
			{
				this.mSelected.SetActive(true);
			}
			else
			{
				this.mSelected.SetActive(false);
			}
		}
	}

	public TextBlock mContent;

	public TaskData mTaskData;

	public TaskVO taskVo;

	public GameObject mSelected;
}
