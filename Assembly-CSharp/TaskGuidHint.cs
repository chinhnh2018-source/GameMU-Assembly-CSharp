using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class TaskGuidHint : SpriteSL
{
	public TaskGuidHint(Sprite container, Stage stg, string content, EventHandler dfun = null, object config = null, int delayingTicks = 3960)
	{
		base.mouseEnabled = false;
		base.mouseChildren = false;
		TaskGuidHint._Stage = stg;
		this._Container = container;
		this.delete_fun = dfun;
		this._stayTime = delayingTicks;
		this.lastTimer = U3DUtils.GetTimer();
		this.buildBuffer(content, config);
		if (TaskGuidHint.taskGuidHintMap[0] != null)
		{
			TaskGuidHint.taskGuidHintMap[0].deleteText();
		}
		TaskGuidHint.taskGuidHintMap[0] = this;
		this.X = ((double)stg.stageWidth - base.width) * 0.5;
		this.Y = (double)(stg.stageHeight - 160);
		Canvas.SetZIndex(this, 6.0);
	}

	protected void buildBuffer(string content, object config)
	{
	}

	public void deleteText()
	{
	}

	protected void run(Event e)
	{
	}

	public static void resetPosition()
	{
	}

	public static uint STARTY = 100U;

	private static List<TaskGuidHint> taskGuidHintMap = new List<TaskGuidHint>();

	private static Stage _Stage = null;

	private Sprite _Container;

	protected EventHandler delete_fun;

	internal int _stayTime;

	internal int lastTimer;

	public static string DefaultFontName = "SimSun";

	public static string KaiTi = "KaiTi_GB2312";
}
