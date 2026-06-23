using System;
using ClientTools.Log;

public static class DebugTextLog
{
	public static void LogException(Exception exception)
	{
		if (DebugTextLog.DebugLog != null)
		{
			DebugTextLog.DebugLog.LogException(exception);
		}
	}

	public static void LogError(params object[] messages)
	{
		if (DebugTextLog.DebugLog != null)
		{
			DebugTextLog.DebugLog.LogError(messages);
		}
	}

	public static void Log(params object[] messages)
	{
		if (DebugTextLog.DebugLog != null)
		{
			DebugTextLog.DebugLog.Log(messages);
		}
	}

	public static void LogStackMsg(string message)
	{
		if (DebugTextLog.DebugLog != null)
		{
			DebugTextLog.DebugLog.LogStackMsg(message);
		}
	}

	public static IDebugTextLog DebugLog;
}
