using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class HttpCache
{
	public static void Save()
	{
		try
		{
			if (HttpCache.HttpCacheDict != null)
			{
				if (HttpCache.HttpCacheDict.Count > 0)
				{
					using (StringWriter stringWriter = new StringWriter())
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(DictionarySerializable<string, HttpCache.HttpCacheInfo>));
						xmlSerializer.Serialize(stringWriter, HttpCache.HttpCacheDict);
						PlayerPrefs.SetString("Http_Cache", stringWriter.ToString());
						PlayerPrefs.Save();
					}
				}
				else if (PlayerPrefs.HasKey("Http_Cache"))
				{
					PlayerPrefs.DeleteKey("Http_Cache");
					PlayerPrefs.Save();
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void Load()
	{
		if (PlayerPrefs.HasKey("Http_Cache"))
		{
			string @string = PlayerPrefs.GetString("Http_Cache");
			if (@string != null && @string.Length > 0)
			{
				using (StringReader stringReader = new StringReader(@string))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DictionarySerializable<string, HttpCache.HttpCacheInfo>));
					HttpCache.HttpCacheDict = (DictionarySerializable<string, HttpCache.HttpCacheInfo>)xmlSerializer.Deserialize(stringReader);
				}
			}
		}
		else
		{
			HttpCache.HttpCacheDict = new DictionarySerializable<string, HttpCache.HttpCacheInfo>();
		}
	}

	public static void Add(string theKey, HttpCache.HttpCacheInfo theData)
	{
		if (HttpCache.HttpCacheDict == null)
		{
			HttpCache.Load();
		}
		if (HttpCache.HttpCacheDict.ContainsKey(theKey))
		{
			HttpCache.HttpCacheDict.Remove(theKey);
		}
		HttpCache.HttpCacheDict.Add(theKey, theData);
		HttpCache.Save();
	}

	public static void Del(string theKey)
	{
		if (HttpCache.HttpCacheDict == null)
		{
			HttpCache.Load();
		}
		if (HttpCache.HttpCacheDict.ContainsKey(theKey))
		{
			HttpCache.HttpCacheDict.Remove(theKey);
		}
		HttpCache.Save();
	}

	public static Dictionary<string, HttpCache.HttpCacheInfo> Get()
	{
		if (HttpCache.HttpCacheDict == null)
		{
			HttpCache.Load();
		}
		return HttpCache.HttpCacheDict;
	}

	private const string KEY_HTTP_CACHE = "Http_Cache";

	private static DictionarySerializable<string, HttpCache.HttpCacheInfo> HttpCacheDict;

	public class HttpCacheInfo
	{
		public byte[] postdata;

		public string url;
	}
}
