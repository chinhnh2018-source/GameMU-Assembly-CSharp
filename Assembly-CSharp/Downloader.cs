using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class Downloader
{
	public Downloader(IAsyncLoad _target = null)
	{
		this.target = _target;
	}

	public void CancelRequest()
	{
		this.asyncLoader.CompleteRequest(null);
	}

	public void GetResourceByVer(string path, string ver, bool force)
	{
		if (this.go != null)
		{
			this.asyncLoader = this.go.AddComponent<AsyncLoader>();
			if (this.asyncLoader != null)
			{
				this.go.GetComponent<AsyncLoader>().CompleteRequest(null);
			}
		}
		else
		{
			this.go = new GameObject(string.Empty);
		}
		if (null == this.asyncLoader)
		{
			this.asyncLoader = this.go.AddComponent<AsyncLoader>();
		}
		this.asyncLoader.Url = path;
		this.asyncLoader.AssetType = typeof(Texture);
		this.asyncLoader.GetResourceByVer(this);
	}

	private GameObject go;

	private AsyncLoader asyncLoader;

	public string Args;

	public DownloaderEventHander Completed;

	public IAsyncLoad target;
}
