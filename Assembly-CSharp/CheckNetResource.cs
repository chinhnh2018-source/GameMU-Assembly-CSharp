using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class CheckNetResource : TTMonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button(Global.GetLang("开始检测"), new GUILayoutOption[]
		{
			GUILayout.Height(60f)
		}))
		{
			base.StartCoroutine<bool>(this.CheckResource());
		}
		if (GUILayout.Button(Global.GetLang("清理"), new GUILayoutOption[]
		{
			GUILayout.Height(60f)
		}))
		{
			this.FailedFiles = string.Empty;
		}
		GUILayout.TextField(string.Format("{0}/{1},failed count={2}", this.CurrLoadedCount, this.TotalCount, this.FailCount), new GUILayoutOption[0]);
		GUILayout.TextArea(this.FailedFiles, new GUILayoutOption[0]);
	}

	public IEnumerator CheckResource()
	{
		WWW www = new WWW(PathUtils.SteamingAssetsPath_DontUseThis("version.xml"));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				www.error
			});
			MUDebug.LogError<string>(new string[]
			{
				"获取安装包中version.xml错误"
			});
			yield break;
		}
		string content = Global.GetUTF8StringFromBytes(www.bytes);
		XElement xml = XElement.Parse(content);
		if (xml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"安装包中Version.xml错误"
			});
			yield break;
		}
		CheckNetResource.VersionXml = xml;
		string url = null;
		url = Global.ReadXmlConfigStr(CheckNetResource.VersionXml, string.Empty, "URL");
		url = PathUtils.GetWWWPath(url + "version.xml") + "?v=" + Global.GetTimeStamp();
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			this.FailedFiles = this.FailedFiles + Global.GetLang("Critical Error：") + www.url + "\n";
			yield break;
		}
		byte[] remoteVersionBytes = www.bytes;
		content = Global.GetUTF8StringFromBytes(www.bytes);
		xml = XElement.Parse(content);
		if (xml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"没有找到远程的version.xml"
			});
			yield break;
		}
		CheckNetResource.NetVersionXML = xml;
		url = Global.ReadXmlConfigStr(CheckNetResource.NetVersionXML, string.Empty, "URL");
		CheckNetResource.URL = url;
		url = PathUtils.GetWWWPath(url + "index.unity3d") + "?v=" + Global.GetTimeStamp();
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				www.error
			});
			this.FailedFiles = this.FailedFiles + Global.GetLang("Critical Error：") + www.url + "\n";
			yield break;
		}
		AssetBundleManager.AddAssetBundle("index.unity3d", www.assetBundle);
		CheckNetResource.NetIndexXML = XmlManager.GetResXml("index.unity3d", "index");
		List<XElement> netIndexFileList = Global.GetXElementList(CheckNetResource.NetIndexXML, "File");
		url = CheckNetResource.URL;
		this.CurrLoadedCount = 0;
		this.FailCount = 0;
		this.TotalCount = netIndexFileList.Count;
		foreach (XElement fileXmlItem in netIndexFileList)
		{
			string fileName = Global.GetXElementAttributeStr(fileXmlItem, "Name");
			string crc32 = Global.GetXElementAttributeStr(fileXmlItem, "CRC32");
			if (crc32 == null || crc32 == string.Empty)
			{
				crc32 = Global.GetTimeStamp().ToString();
			}
			fileName = fileName.Replace("\\", "/");
			using (www = new WWW(PathUtils.GetWWWPath(url + fileName) + "?v=" + crc32))
			{
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					this.FailedFiles = this.FailedFiles + www.url + "\n";
					this.FailCount++;
				}
				else
				{
					this.CurrLoadedCount++;
				}
			}
		}
		this.FailedFiles += "\n Over";
		yield break;
	}

	public static XElement VersionXml;

	public static XElement NetVersionXML;

	public static XElement NetIndexXML;

	public static string URL = string.Empty;

	public string FailedFiles = string.Empty;

	public int CurrLoadedCount;

	public int FailCount;

	public int TotalCount;
}
