using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpService : TTMonoBehaviour
{
	private void Awake()
	{
		HttpService.sInstance = this;
	}

	public static HttpService Instance
	{
		get
		{
			if (HttpService.sInstance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "HttpServiceObject";
				gameObject.SetActive(true);
				HttpService.sInstance = gameObject.AddComponent<HttpService>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return HttpService.sInstance;
		}
	}

	private void Start()
	{
	}

	public void Load(string url, Action<WWW, object> callback, object param, int timeout = 0)
	{
		if (string.IsNullOrEmpty(url))
		{
			MUDebug.LogError<string>(new string[]
			{
				"[Http Service] URL is NULL"
			});
			return;
		}
		if (callback == null)
		{
			callback = delegate(WWW www, object arg)
			{
			};
		}
		if (timeout == 0)
		{
			timeout = 30;
		}
		base.StartCoroutine<bool>(this.loadFromRemote(url, callback, delegate(WWW arg)
		{
		}, param, null, null, timeout = 0));
	}

	public void Load(string url, Action<WWW, object> callback, byte[] postData, object param, int timeout = 0)
	{
		if (string.IsNullOrEmpty(url))
		{
			MUDebug.LogError<string>(new string[]
			{
				"[Http Service] URL is NULL"
			});
			return;
		}
		if (callback == null)
		{
			callback = delegate(WWW www, object arg)
			{
			};
		}
		if (timeout == 0)
		{
			timeout = 30;
		}
		base.StartCoroutine<bool>(this.loadFromRemote(url, callback, delegate(WWW arg)
		{
		}, param, null, postData, timeout = 0));
	}

	public void Load(string url, Action<WWW, object> callback, int timeout)
	{
		if (string.IsNullOrEmpty(url))
		{
			MUDebug.LogError<string>(new string[]
			{
				"[Http Service] URL is NULL"
			});
			return;
		}
		if (callback == null)
		{
			callback = delegate(WWW www, object arg)
			{
			};
		}
		if (timeout == 0)
		{
			timeout = 30;
		}
		base.StartCoroutine<bool>(this.loadFromRemote(url, callback, delegate(WWW arg)
		{
		}, null, null, null, timeout));
	}

	public void Load(string url, Action<WWW, object> callback, WWWForm form, object param, int timeout)
	{
		if (string.IsNullOrEmpty(url))
		{
			MUDebug.LogError<string>(new string[]
			{
				"[Http Service] URL is NULL"
			});
			return;
		}
		if (callback == null)
		{
			callback = delegate(WWW www, object arg)
			{
			};
		}
		if (timeout == 0)
		{
			timeout = 30;
		}
		base.StartCoroutine<bool>(this.loadFromRemote(url, callback, delegate(WWW arg)
		{
		}, param, form, null, timeout));
	}

	public void Load(string url, Action<WWW, object> callback, Action<WWW> progressCallback, object param, int timeout = 0)
	{
		if (string.IsNullOrEmpty(url))
		{
			MUDebug.LogError<string>(new string[]
			{
				"[Http Service] URL is NULL"
			});
			return;
		}
		if (callback == null)
		{
			callback = delegate(WWW www, object arg)
			{
			};
		}
		if (progressCallback == null)
		{
			progressCallback = delegate(WWW arg)
			{
			};
		}
		if (timeout == 0)
		{
			timeout = 30;
		}
		base.StartCoroutine<bool>(this.loadFromRemote(url, callback, progressCallback, param, null, null, timeout));
	}

	private IEnumerator loadFromRemote(string url, Action<WWW, object> callback, Action<WWW> progressCallback, object param = null, WWWForm form = null, byte[] postData = null, int timeout = 0)
	{
		yield return 1;
		if (this.DownloadList.ContainsKey(url))
		{
			HttpService.URLRequest urlrequest = this.DownloadList[url];
			urlrequest.callbacks = (Action<WWW, object>)Delegate.Combine(urlrequest.callbacks, callback);
			HttpService.URLRequest urlrequest2 = this.DownloadList[url];
			urlrequest2.progressCallbacks = (Action<WWW>)Delegate.Combine(urlrequest2.progressCallbacks, progressCallback);
			this.DownloadList[url].callbackParmas.Add(param);
		}
		else
		{
			this.DownloadList.Add(url, new HttpService.URLRequest(url, callback, progressCallback, param, form, postData, timeout));
		}
		yield break;
	}

	private void Update()
	{
		List<string> list = null;
		foreach (KeyValuePair<string, HttpService.URLRequest> keyValuePair in this.DownloadList)
		{
			HttpService.URLRequest value = keyValuePair.Value;
			if (value.wwwObject.isDone)
			{
				Action<WWW, object> action = value.callbacks;
				while (action != null && action.GetInvocationList().GetLength(0) > 0)
				{
					Action<WWW, object> action2 = (Action<WWW, object>)action.GetInvocationList()[0];
					if (!string.IsNullOrEmpty(value.wwwObject.error))
					{
						MUDebug.LogWarning<string>(new string[]
						{
							string.Concat(new string[]
							{
								base.name,
								" : ",
								value.wwwObject.error,
								" , ",
								value.Url
							})
						});
					}
					try
					{
						action = (Action<WWW, object>)Delegate.Remove(action, action2);
						HttpService.URLRequest urlrequest = value;
						urlrequest.progressCallbacks = (Action<WWW>)Delegate.Remove(urlrequest.progressCallbacks, (Action<WWW>)value.progressCallbacks.GetInvocationList()[0]);
						action2.Invoke(value.wwwObject, value.callbackParmas[0]);
					}
					catch (Exception ex)
					{
						MUDebug.LogWarning<Exception>(new Exception[]
						{
							ex
						});
					}
					if (value.callbackParmas.Count > 0)
					{
						value.callbackParmas.RemoveAt(0);
					}
				}
				if (list == null)
				{
					list = new List<string>();
				}
				List<string> list2 = list;
				Dictionary<string, HttpService.URLRequest>.Enumerator enumerator;
				KeyValuePair<string, HttpService.URLRequest> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Key);
			}
			else if (value.progressCallbacks != null)
			{
				try
				{
					if (Time.realtimeSinceStartup - value.startTime > (float)value.Timeout)
					{
						MUDebug.LogWarning<string>(new string[]
						{
							"[Http Service]" + value.Url + ", TIMEOUT"
						});
						Action<WWW, object> action3 = value.callbacks;
						while (action3 != null && action3.GetInvocationList().GetLength(0) > 0)
						{
							Action<WWW, object> action4 = (Action<WWW, object>)action3.GetInvocationList()[0];
							if (!string.IsNullOrEmpty(value.wwwObject.error))
							{
								MUDebug.LogWarning<string>(new string[]
								{
									value.wwwObject.error
								});
							}
							try
							{
								action3 = (Action<WWW, object>)Delegate.Remove(action3, action4);
								HttpService.URLRequest urlrequest2 = value;
								urlrequest2.progressCallbacks = (Action<WWW>)Delegate.Remove(urlrequest2.progressCallbacks, (Action<WWW>)value.progressCallbacks.GetInvocationList()[0]);
								action4.Invoke(null, value.Url);
							}
							catch (Exception ex2)
							{
								MUDebug.LogException(ex2);
							}
							if (value.callbackParmas.Count > 0)
							{
								value.callbackParmas.RemoveAt(0);
							}
						}
						if (list == null)
						{
							list = new List<string>();
						}
						List<string> list3 = list;
						Dictionary<string, HttpService.URLRequest>.Enumerator enumerator;
						KeyValuePair<string, HttpService.URLRequest> keyValuePair3 = enumerator.Current;
						list3.Add(keyValuePair3.Key);
					}
					else
					{
						((Action<WWW>)value.progressCallbacks.GetInvocationList()[0]).Invoke(value.wwwObject);
					}
				}
				catch (Exception ex3)
				{
					MUDebug.LogException(ex3);
				}
			}
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				this.DownloadList.Remove(list[i]);
			}
			list.Clear();
			list = null;
		}
	}

	public const int DEFAULT_TIMEOUT = 30;

	private static HttpService sInstance;

	public Action<WWW, object> LoadFinishDelegate;

	public Action<WWW> LoadProgressDelegate;

	private Dictionary<string, HttpService.URLRequest> DownloadList = new Dictionary<string, HttpService.URLRequest>();

	public class URLRequest
	{
		public URLRequest(string url, Action<WWW, object> callback, Action<WWW> progressCallback, object param = null, WWWForm form = null, byte[] postData = null, int timeout = 0)
		{
			this.Url = url;
			this.callbacks = callback;
			this.progressCallbacks = progressCallback;
			this.startTime = Time.realtimeSinceStartup;
			if (timeout <= 0)
			{
				timeout = 30;
			}
			this.Timeout = timeout;
			if (postData != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/octet-stream");
				this.wwwObject = new WWW(url, postData, dictionary);
			}
			else if (form != null)
			{
				this.wwwObject = new WWW(url, form);
			}
			else
			{
				this.wwwObject = new WWW(url);
			}
			this.callbackParmas.Add(param);
		}

		public const int DEFAULT_TIMEOUT = 30;

		public string Url;

		public Action<WWW, object> callbacks;

		public Action<WWW> progressCallbacks;

		public WWW wwwObject;

		public WWWForm form;

		public List<object> callbackParmas = new List<object>();

		public float startTime;

		public int Timeout;
	}
}
