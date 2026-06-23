using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class SystemHintWindow
{
	public static void InitNoticeContainer()
	{
		if (null != SystemHintWindow.noticeContainer)
		{
			return;
		}
	}

	public static void AddHintText(uint color, string text, double fontSize, int delayingTicks, string fontName = "SimSun")
	{
		SystemErrorText.ShowErrorText(null, 0, 0, color, text, ShowGameInfoTypes.OnlySysHint);
	}

	public static void AddHintText(GameInfoTextItem gameInfoTextItem, double fontSize, int delayingTicks)
	{
		SystemHintWindow.AddHintText(SystemHintWindow.GetColor(gameInfoTextItem), gameInfoTextItem.TextMsg, fontSize, delayingTicks, "SimSun");
	}

	public static void AddHintText(uint color, string text, double fontSize)
	{
	}

	private static uint GetColor(GameInfoTextItem gameInfoTextItem)
	{
		uint result = uint.MaxValue;
		if (gameInfoTextItem.GameInfoTypeIndex != GameInfoTypeIndexes.Normal)
		{
			if (gameInfoTextItem.GameInfoTypeIndex == GameInfoTypeIndexes.Error)
			{
				result = ColorSL.FromArgb(255, 225, 0, 0);
			}
			else if (gameInfoTextItem.GameInfoTypeIndex == GameInfoTypeIndexes.Hot)
			{
				result = 4294967040U;
			}
		}
		return result;
	}

	public static void AddTaskGuidHintText(int taskID)
	{
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
		if (taskXmlNodeByID == null)
		{
			return;
		}
		string guideText = taskXmlNodeByID.GuideText;
	}

	public static void InitMapNoticeContainer()
	{
		if (null != SystemHintWindow.MapNoticeContainer)
		{
			return;
		}
	}

	public static void AddMapHintText(uint color, string text, double fontSize)
	{
		SystemHintWindow.InitMapNoticeContainer();
	}

	public static void InitTaskNoticeContainer()
	{
		if (null != SystemHintWindow.TaskNoticeContainer)
		{
			return;
		}
	}

	public static void AddTaskTargetHintText(string text)
	{
		SystemHintWindow.InitTaskNoticeContainer();
	}

	public static void InitPropsNoticeContainer()
	{
		if (null != SystemHintWindow.PropsNoticeContainer)
		{
			return;
		}
	}

	public static void AddPropsHintText(uint color, string text)
	{
		SystemHintWindow.InitPropsNoticeContainer();
	}

	public static void InitErrorsNoticeContainer()
	{
		if (null != SystemHintWindow.ErrorsNoticeContainer)
		{
			return;
		}
	}

	public static void AddErrorHintText(uint color, string text)
	{
	}

	public static void InitRoleInfoNoticeContainer()
	{
		if (null != SystemHintWindow.RoleInfoNoticeContainer)
		{
			return;
		}
	}

	public static void AddRoleInfoHintText(uint color, string text)
	{
		SystemHintWindow.InitRoleInfoNoticeContainer();
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyBox, text, 0, -1, -1, 0);
	}

	public static void ResetPosition()
	{
		if (null != SystemHintWindow.noticeContainer)
		{
			SystemHintWindow.noticeContainer.resetPosition(0.0);
		}
		if (null != SystemHintWindow.MapNoticeContainer)
		{
			SystemHintWindow.MapNoticeContainer.resetPosition(0.0);
		}
		if (null != SystemHintWindow.TaskNoticeContainer)
		{
			SystemHintWindow.TaskNoticeContainer.resetPosition(0.0);
		}
		if (null != SystemHintWindow.PropsNoticeContainer)
		{
			SystemHintWindow.PropsNoticeContainer.resetPosition(0.0);
		}
		if (null != SystemHintWindow.ErrorsNoticeContainer)
		{
			SystemHintWindow.ErrorsNoticeContainer.resetPosition(Global.GlobalMainWindow.ActualHeight - 230.0);
		}
		if (null != SystemHintWindow.RoleInfoNoticeContainer)
		{
			SystemHintWindow.RoleInfoNoticeContainer._LeftPos = Global.GlobalMainWindow.ActualWidth - 150.0;
			SystemHintWindow.RoleInfoNoticeContainer.resetPosition(Global.GlobalMainWindow.ActualHeight - 150.0);
		}
		TaskGuidHint.resetPosition();
	}

	public static Canvas Root;

	private static GNoticeContainer noticeContainer;

	private static GNoticeContainer MapNoticeContainer;

	private static GNoticeContainer TaskNoticeContainer;

	private static GNoticeContainer PropsNoticeContainer;

	private static GNoticeContainer ErrorsNoticeContainer;

	private static GNoticeContainer RoleInfoNoticeContainer;
}
