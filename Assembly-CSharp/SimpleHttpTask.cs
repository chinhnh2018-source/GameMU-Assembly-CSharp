using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SimpleHttpTask : TTMonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (this._currentHttpInfo != null && this._currentWWWTask != null && (int)this._currentTime > -1)
		{
			this._currentTime += Time.deltaTime;
			if (this._currentTime >= this._currentHttpInfo._timeOut)
			{
				if (this._currentHttpInfo.callbackDel != null)
				{
					this._currentHttpInfo.callbackDel(null);
				}
				this._currentTime = -1f;
				this._currentWWWTask.Dispose();
				this._currentWWWTask = null;
				this._currentHttpInfo = null;
				NGUITools.DestroyImmediate(base.gameObject);
			}
		}
	}

	public static SimpleHttpTask newInstance
	{
		get
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "HttpTask" + DateTime.Now.ToString();
			gameObject.SetActive(true);
			SimpleHttpTask result = gameObject.AddComponent<SimpleHttpTask>();
			Object.DontDestroyOnLoad(gameObject);
			return result;
		}
	}

	public static void HttpGet(string url, Dictionary<string, string> data, byte[] byteData, SimpleHttpTask.HttpCallback callback, float timeOut = 10f)
	{
		HttpInfo httpInfo = new HttpInfo();
		httpInfo.callbackDel = callback;
		httpInfo._url = url;
		httpInfo._formData = data;
		httpInfo._byteData = byteData;
		httpInfo._type = "GET";
		httpInfo._timeOut = timeOut;
		SimpleHttpTask.newInstance.startHttp(httpInfo);
	}

	public static void HttpPost(string url, Dictionary<string, string> data, byte[] byteData, SimpleHttpTask.HttpCallback callback, float timeOut = 10f)
	{
		HttpInfo httpInfo = new HttpInfo();
		httpInfo.callbackDel = callback;
		httpInfo._url = url;
		httpInfo._formData = data;
		httpInfo._byteData = byteData;
		httpInfo._type = "POST";
		httpInfo._timeOut = timeOut;
		SimpleHttpTask.newInstance.startHttp(httpInfo);
	}

	public void startHttp(HttpInfo info)
	{
		if (info != null)
		{
			if (info._type == "GET")
			{
				base.StartCoroutine<bool>(this.DoHttpGet(info));
			}
			if (info._type == "POST")
			{
				base.StartCoroutine<bool>(this.DoHttpPost(info));
			}
		}
	}

	private static string UrlEncode(string str)
	{
		byte[] bytes = Encoding.Default.GetBytes(str);
		string text = string.Empty;
		for (int i = 0; i < bytes.Length; i++)
		{
			text += "%";
			text += bytes[i].ToString("X2");
		}
		return text;
	}

	private IEnumerator DoHttpGet(HttpInfo info)
	{
		string dataurl = info._url;
		if (info._formData != null)
		{
			dataurl += "?";
			int i = 0;
			foreach (KeyValuePair<string, string> item in info._formData)
			{
				dataurl = dataurl + item.Key + "=" + item.Value;
				i++;
				if (i < info._formData.Count)
				{
					dataurl += "&";
				}
			}
		}
		WWW getData = new WWW(dataurl);
		this._currentWWWTask = getData;
		this._currentTime = 0f;
		this._currentHttpInfo = info;
		yield return getData;
		if (info.callbackDel != null)
		{
			info.callbackDel(getData);
		}
		getData.Dispose();
		getData = null;
		this._currentHttpInfo = null;
		this._currentTime = -1f;
		this._currentWWWTask = null;
		NGUITools.DestroyImmediate(base.gameObject);
		yield break;
	}

	private IEnumerator DoHttpPost(HttpInfo info)
	{
		WWWForm form = null;
		WWW getData = null;
		if (info != null)
		{
			if (info._formData != null)
			{
				form = new WWWForm();
				foreach (KeyValuePair<string, string> item in info._formData)
				{
					form.AddField(item.Key, item.Value);
				}
				getData = new WWW(info._url, form);
			}
			if (info._byteData != null)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				dict.Add("Content-Type", "application/octet-stream");
				getData = new WWW(info._url, info._byteData, dict);
			}
		}
		this._currentWWWTask = getData;
		this._currentTime = 0f;
		this._currentHttpInfo = info;
		yield return getData;
		if (info != null && info.callbackDel != null)
		{
			info.callbackDel(getData);
		}
		getData.Dispose();
		getData = null;
		this._currentHttpInfo = null;
		this._currentTime = -1f;
		this._currentWWWTask = null;
		NGUITools.DestroyImmediate(base.gameObject);
		yield break;
	}

	public LinkedList<HttpInfo> _httpInfoList = new LinkedList<HttpInfo>();

	public SimpleHttpTask sInstance;

	public WWW _currentWWWTask;

	public HttpInfo _currentHttpInfo;

	public float _currentTime = -1f;

	public delegate void HttpCallback(WWW wwwData);
}
