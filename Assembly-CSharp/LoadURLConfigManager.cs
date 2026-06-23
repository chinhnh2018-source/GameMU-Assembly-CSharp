using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class LoadURLConfigManager
{
	public static LoadURLConfigManager GetInstance()
	{
		if (LoadURLConfigManager.instance == null)
		{
			LoadURLConfigManager.instance = new LoadURLConfigManager();
		}
		return LoadURLConfigManager.instance;
	}

	public void ParseXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/GroupAdd.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Add");
		if (xelementList.Count > 0)
		{
			this.ParseStdUrlList(Global.GetXElementAttributeStr(xelementList[0], "Address"));
			this.ParseStdUrlList2(Global.GetXElementAttributeStr(xelementList[1], "Address"));
		}
	}

	public string KnetAdURL
	{
		get
		{
			string text = null;
			if (Global.NetVersionXML != null)
			{
				text = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "knetAdUrl");
			}
			MUDebug.LogError<string>(new string[]
			{
				"KnetAdURL 广告地址：" + text
			});
			return text;
		}
	}

	private void ParseStdUrlList(string url)
	{
		MUDebug.Log<string>(new string[]
		{
			"StdUrlList " + url
		});
		if (string.IsNullOrEmpty(url))
		{
			this.CheckSFAndWorkerStdUrlList = null;
		}
		else
		{
			this.CheckSFAndWorkerStdUrlList = url.Split(new char[]
			{
				','
			});
		}
	}

	public string[] CheckSFAndWorkerStdUrlList
	{
		get
		{
			return this.mCheckSFAndWorkerStdUrlList;
		}
		set
		{
			this.mCheckSFAndWorkerStdUrlList = value;
		}
	}

	private void ParseStdUrlList2(string url)
	{
		MUDebug.Log<string>(new string[]
		{
			"StdUrlList2 " + url
		});
		if (string.IsNullOrEmpty(url))
		{
			this.CheckSFAndWorkerStdStdUrlList2 = null;
		}
		else
		{
			this.CheckSFAndWorkerStdStdUrlList2 = url.Split(new char[]
			{
				','
			});
		}
	}

	public string[] CheckSFAndWorkerStdStdUrlList2
	{
		get
		{
			return this.mCheckSFAndWorkerStdUrlList2;
		}
		set
		{
			this.mCheckSFAndWorkerStdUrlList2 = value;
		}
	}

	private static LoadURLConfigManager instance;

	private string xmlPath = string.Empty;

	private string[] mCheckSFAndWorkerStdUrlList;

	private string[] mCheckSFAndWorkerStdUrlList2;
}
