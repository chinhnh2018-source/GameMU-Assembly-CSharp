using System;
using System.Collections.Generic;
using UnityEngine;

public struct UniWebViewMessage
{
	public UniWebViewMessage(string rawMessage)
	{
		this.rawMessage = rawMessage;
		string[] array = rawMessage.Split(new string[]
		{
			"://"
		}, 0);
		if (array.Length >= 2)
		{
			this.scheme = array[0];
			string text = string.Empty;
			for (int i = 1; i < array.Length; i++)
			{
				text += array[i];
			}
			string[] array2 = text.Split(new char[]
			{
				"?".get_Chars(0)
			});
			this.path = array2[0].TrimEnd(new char[]
			{
				'/'
			});
			this.args = new Dictionary<string, string>();
			if (array2.Length > 1)
			{
				foreach (string text2 in array2[1].Split(new char[]
				{
					"&".get_Chars(0)
				}))
				{
					string[] array4 = text2.Split(new char[]
					{
						"=".get_Chars(0)
					});
					if (array4.Length > 1)
					{
						this.args[array4[0]] = WWW.UnEscapeURL(array4[1]);
					}
				}
			}
		}
		else
		{
			Debug.LogError("Bad url scheme. Can not be parsed to UniWebViewMessage: " + rawMessage);
		}
	}

	public string rawMessage { get; private set; }

	public string scheme { get; private set; }

	public string path { get; private set; }

	public Dictionary<string, string> args { get; private set; }
}
