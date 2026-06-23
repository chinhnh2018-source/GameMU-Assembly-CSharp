using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class GameConfigManager
{
	public static IEnumerator InitVersionConfig()
	{
		if (Global.NetVersionXML == null)
		{
			XElement xml = null;
			WWW www = new WWW(PathUtils.GetWWWPath(PathUtils.SteamingAssetsPath_DontUseThis("version.xml")));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				yield break;
			}
			string content = Global.GetUTF8StringFromBytes(Program.DecryptSceneData(www.bytes));
			content = GameConfigManager.ParseProgramData(content);
			xml = XElement.Parse(content);
			if (xml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"没有找到version.xml"
				});
				yield break;
			}
			Global.NetVersionXML = xml;
			www.Dispose();
			www = null;
		}
		if (GameConfigManager.NextStep != null)
		{
			GameConfigManager.NextStep(null, new NextStepEventArgs
			{
				StepType = 0
			});
		}
		yield break;
	}

	public static string ParseProgramData(string content)
	{
		string text = "</Config>";
		int num = content.LastIndexOf(text);
		if (num > 0 && content.Length > num + text.Length)
		{
			content = content.Substring(0, num + text.Length);
		}
		return content;
	}

	public static NextStepEventHandler NextStep;
}
