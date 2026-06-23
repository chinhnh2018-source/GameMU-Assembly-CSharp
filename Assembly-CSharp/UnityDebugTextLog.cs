using System;
using ClientTools.Log;

public class UnityDebugTextLog : IDebugTextLog
{
	public void Log(params object[] messages)
	{
		MUDebug.Log<object>("   ", messages);
	}

	public void LogWarning(params object[] messages)
	{
		MUDebug.Log<object>("   ", messages);
	}

	public void LogError(params object[] messages)
	{
		MUDebug.LogError<object>("   ", messages);
	}

	public void LogException(Exception exception)
	{
		MUDebug.LogException(exception);
	}

	public void LogStackMsg(string message)
	{
		MUDebug.LogStackMsg(message);
	}

	public void LogError<T>(string separator, T[] messages)
	{
		MUDebug.LogError<T>(separator, messages);
	}

	public static IDebugTextLog DebugLog = new UnityDebugTextLog();
}
