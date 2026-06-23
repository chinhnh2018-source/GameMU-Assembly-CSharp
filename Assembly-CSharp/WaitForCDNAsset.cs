using System;
using System.Collections;
using UnityEngine;

public class WaitForCDNAsset : IEnumerator
{
	public WaitForCDNAsset(string bundleID)
	{
		this.bundleId = bundleID;
		this.mTimeOut = 10f;
		this.mStartTime = Time.unscaledTime;
		this.mIsDone = false;
		this.mWWW = BackStageDownloadManager.instance.RequestAsset(bundleID, new Action<string, CDNAssetBundle>(this.OnRequestComplete), new Action<bool>(this.OnImgCacheCallBack));
	}

	public object Current
	{
		get
		{
			return null;
		}
	}

	public bool MoveNext()
	{
		if (this.TimeOut)
		{
			this.error = "TIMEOUT";
			MUDebug.LogError<string>(new string[]
			{
				"MoveNext() TIMEOUT  " + this.bundleId
			});
		}
		return !this.mIsDone && !this.TimeOut;
	}

	public void Reset()
	{
	}

	public WWW www
	{
		get
		{
			return this.mWWW;
		}
	}

	public bool TimeOut
	{
		get
		{
			if (this.www != null && this.www.progress != this.mProgress)
			{
				this.mProgress = this.www.progress;
				this.mStartTime = Time.unscaledTime;
				return false;
			}
			return Time.unscaledTime - this.mStartTime > this.mTimeOut;
		}
	}

	private void OnRequestComplete(string e, CDNAssetBundle bundle)
	{
		this.mWWW = bundle.mwww;
		this.mIsDone = true;
		this.cdnAssetBundle = bundle;
		this.assetBundle = this.cdnAssetBundle.GetAssetBundle();
		this.error = e;
	}

	private void OnImgCacheCallBack(bool isImg)
	{
		this.isImgCallBack = isImg;
		if (this.isImgCallBack)
		{
			BackStageDownloadManager.instance.AddImgsWaitForCDNAsset(this.bundleId, this);
		}
	}

	public string error;

	public float mProgress;

	public CDNAssetBundle cdnAssetBundle;

	public AssetBundle assetBundle;

	public bool mIsDone;

	private float mStartTime;

	private float mTimeOut;

	private WWW mWWW;

	private string bundleId = string.Empty;

	private bool isImgCallBack;
}
