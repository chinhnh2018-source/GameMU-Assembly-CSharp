using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LocalPushManager : MonoBehaviour
{
	public static LocalPushManager Instance
	{
		get
		{
			if (LocalPushManager._inst == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.SetActive(true);
				LocalPushManager._inst = gameObject.AddComponent<LocalPushManager>();
				Object.DontDestroyOnLoad(gameObject);
				return LocalPushManager._inst;
			}
			return LocalPushManager._inst;
		}
	}

	private static void Init()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(LocalPushManager.PUSH_JAR_PATH);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("Init", new object[]
			{
				PlatSDKMgr.UnityPlayerActivity
			});
		}
	}

	public void LoadConfigAndSetPush()
	{
		base.StartCoroutine(this.OnLoadConfigAndSetPush());
	}

	private IEnumerator OnLoadConfigAndSetPush()
	{
		LocalPushManager.Init();
		LocalPushManager._configurl = Global.WebPath(StringUtil.substitute("TuiSong.xml", new object[0]));
		WWW www = new WWW(LocalPushManager._configurl);
		yield return www;
		XElement xml = null;
		if (!string.IsNullOrEmpty(www.error))
		{
			yield break;
		}
		string con = Global.GetUTF8StringFromBytes(www.bytes);
		xml = XElement.Parse(con);
		www.Dispose();
		www = null;
		if (xml != null)
		{
			LocalPushManager._pushItemlist = new List<PushItem>();
			List<XElement> pushlist = Global.GetXElementList(xml, "TuiSong");
			foreach (XElement item in pushlist)
			{
				PushItem notion = new PushItem();
				int id = Global.GetXElementAttributeInt(item, "ID");
				notion.id = id;
				notion.title = Global.GetXElementAttributeStr(item, "Title");
				notion.type = Global.GetXElementAttributeInt(item, "Type");
				notion.content = Global.GetXElementAttributeStr(item, "Description");
				notion.overCount = Global.GetXElementAttributeInt(item, "Num");
				string time = Global.GetXElementAttributeStr(item, "Time");
				if (notion.type == 1)
				{
					string[] strtime = time.Split(new char[]
					{
						':'
					});
					notion.time.Add(int.Parse(strtime[0]));
					notion.time.Add(int.Parse(strtime[1]));
				}
				else if (notion.type == 3)
				{
					string[] strtime2 = time.Split(new char[]
					{
						','
					});
					notion.weekday = int.Parse(strtime2[0]);
					string[] temp = strtime2[1].Split(new char[]
					{
						':'
					});
					notion.time.Add(int.Parse(temp[0]));
					notion.time.Add(int.Parse(temp[1]));
				}
				else if (notion.type == 4)
				{
					string[] strtime3 = time.Split(new char[]
					{
						','
					});
					notion.callbackDay = int.Parse(strtime3[0]);
					string[] temp2 = strtime3[1].Split(new char[]
					{
						':'
					});
					notion.time.Add(int.Parse(temp2[0]));
					notion.time.Add(int.Parse(temp2[1]));
				}
				LocalPushManager._pushItemlist.Add(notion);
			}
			LocalPushManager.SetPush();
		}
		yield break;
	}

	private static void SetPush()
	{
		if (LocalPushManager._pushItemlist != null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(LocalPushManager.PUSH_JAR_PATH);
			if (androidJavaClass != null)
			{
				for (int i = 0; i < LocalPushManager._pushItemlist.Count; i++)
				{
					PushItem pushItem = LocalPushManager._pushItemlist[i];
					if (pushItem.type == 1)
					{
						string text = "2050-01-01 00:00:00";
						DateTime now = DateTime.Now;
						if (pushItem.overCount > 0)
						{
							text = DateTime.Parse(now.AddDays((double)pushItem.overCount).ToString("yyyy-MM-dd") + " 23:59:59").ToString("yyyy-MM-dd HH:mm:ss");
						}
						androidJavaClass.CallStatic("SetDailyPush", new object[]
						{
							pushItem.id.ToString(),
							pushItem.title,
							pushItem.content,
							pushItem.time[0],
							pushItem.time[1],
							0,
							text
						});
					}
					else if (pushItem.type == 3)
					{
						string text2 = "2050-01-01 00:00:00";
						DateTime now2 = DateTime.Now;
						if (pushItem.overCount > 0)
						{
							text2 = DateTime.Parse(now2.AddDays((double)(pushItem.overCount * 7)).ToString("yyyy-MM-dd") + " 23:59:59").ToString("yyyy-MM-dd HH:mm:ss");
						}
						androidJavaClass.CallStatic("SetWeakPush", new object[]
						{
							pushItem.id.ToString(),
							pushItem.title,
							pushItem.content,
							pushItem.weekday,
							pushItem.time[0],
							pushItem.time[1],
							0,
							text2
						});
					}
					else if (pushItem.type == 4)
					{
						DateTime dateTime = default(DateTime);
						dateTime = DateTime.Now.AddDays((double)pushItem.callbackDay);
						androidJavaClass.CallStatic("SetCallUserBackPush", new object[]
						{
							pushItem.id.ToString(),
							pushItem.title,
							pushItem.content,
							dateTime.Year,
							dateTime.Month,
							dateTime.Day,
							pushItem.time[0],
							pushItem.time[1],
							0
						});
					}
				}
			}
		}
	}

	public void LoadLocalPushConfigForIOS()
	{
		base.StartCoroutine(this.LoadConfigForIOS());
	}

	private IEnumerator LoadConfigForIOS()
	{
		LocalPushManager._configurl = Global.WebPath(StringUtil.substitute("TuiSong.xml", new object[0]));
		WWW www = new WWW(LocalPushManager._configurl);
		yield return www;
		XElement xml = null;
		if (!string.IsNullOrEmpty(www.error))
		{
			yield break;
		}
		string con = Global.GetUTF8StringFromBytes(www.bytes);
		xml = XElement.Parse(con);
		www.Dispose();
		www = null;
		LocalPushManager.configXml = xml;
		yield break;
	}

	public void RegistLocalNotification()
	{
		this.LoadConfigAndRegist();
	}

	private void LoadConfigAndRegist()
	{
	}

	public void CleanNotification()
	{
	}

	private static string PUSH_JAR_PATH = "com.tianmashikong.notification.LocalPushPlugin";

	private static List<PushItem> _pushItemlist = null;

	private static string _configurl = Global.WebPath(StringUtil.substitute("TuiSong.xml", new object[0]));

	private static string _localconfigurl = Application.streamingAssetsPath + "/TuiSong.xml";

	private static XElement configXml = null;

	private static LocalPushManager _inst = null;
}
