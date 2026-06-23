using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ShiLiTaskInfo : UserControl
{
	public TaskData TaskData
	{
		get
		{
			return this.m_taskData;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void InitTask(TaskData task, TaskVO taskVO)
	{
		this.m_taskData = task;
		this.m_taskData.RoadOtherTaskId = task.DoingTaskID;
		this.m_taskData.TaskClass = taskVO.TaskClass;
		try
		{
			string text = Super.GetTaskTargetDesc(taskVO, 1, true);
			string taskTargetNum = Super.GetTaskTargetNum(taskVO, task.DoingTaskVal1, 1);
			text = StringUtil.substitute("{0}{1}", new object[]
			{
				text,
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					taskTargetNum
				})
			});
			this.lblTaskInfo.text = text;
		}
		catch
		{
			this.lblTaskInfo.text = string.Empty;
		}
	}

	public UILabel lblTaskInfo;

	private TaskData m_taskData;
}
