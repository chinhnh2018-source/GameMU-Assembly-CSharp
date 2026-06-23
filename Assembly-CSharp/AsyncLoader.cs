using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class AsyncLoader : TTMonoBehaviour
{
	private void Start()
	{
		if (AsyncLoader.Downloader_Root == null)
		{
			AsyncLoader.Downloader_Root = new GameObject("__Downloader_Parent");
			AsyncLoader.Downloader_Root.layer = 31;
		}
		base.transform.parent = AsyncLoader.Downloader_Root.transform;
		base.gameObject.layer = AsyncLoader.Downloader_Root.layer;
	}

	public void GetResourceByVer(Downloader _downloader)
	{
		this.downloader = _downloader;
		base.StartCoroutine<bool>(this.DownloadRoutine(_downloader), new TTMonoBehaviour.CoroutineExceptionHandler(this.CoroutineException));
	}

	private void CoroutineException()
	{
		DownloadEventArgs eventArgs = new DownloadEventArgs();
		if (string.IsNullOrEmpty(this.Url))
		{
			this.CompleteRequest(eventArgs);
		}
	}

	public void CompleteRequest(DownloadEventArgs eventArgs)
	{
		if (this.downloader.Completed != null)
		{
			if (eventArgs == null)
			{
				eventArgs = new DownloadEventArgs
				{
					type = 0
				};
			}
			this.downloader.Completed(this.downloader, eventArgs);
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DownloadRoutine(Downloader _downloader)
	{
		DownloadEventArgs eventArgs = new DownloadEventArgs();
		if (string.IsNullOrEmpty(this.Url))
		{
			this.CompleteRequest(eventArgs);
			yield break;
		}
		WWW www = new WWW(this.Url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			this.CompleteRequest(eventArgs);
			yield break;
		}
		if (this.AssetType == typeof(Texture))
		{
			eventArgs.target = www.textureNonReadable;
		}
		else if (!string.IsNullOrEmpty(this.AssetName))
		{
			AssetBundleRequest abr = www.assetBundle.LoadAssetAsync(this.AssetName, typeof(Object));
			yield return abr;
			eventArgs.target = Object.Instantiate(abr.asset);
			www.assetBundle.Unload(false);
		}
		else
		{
			string name = PathUtils.GetAssetName(this.Url.Substring(this.Url.LastIndexOf('/') + 1));
			AssetBundleRequest abr2 = www.assetBundle.LoadAssetAsync(name, typeof(Object));
			yield return abr2;
			eventArgs.target = Object.Instantiate(abr2.asset);
			www.assetBundle.Unload(false);
		}
		www.Dispose();
		www = null;
		if (_downloader.target != null)
		{
			_downloader.target.target = (eventArgs.target as Object);
		}
		eventArgs.type = 1;
		this.CompleteRequest(eventArgs);
		yield break;
	}

	private static GameObject Downloader_Root;

	private Downloader downloader;

	public string Url;

	public string AssetName;

	public Type AssetType;
}
