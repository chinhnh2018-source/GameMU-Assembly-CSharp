using System;
using UnityEngine;

public class CDNAssetBundle
{
	public CDNAssetBundle(AssetBundle bundle, WWW www)
	{
		this.mBundle = bundle;
		this.mRefCount = 1;
		this.mwww = www;
	}

	public int RefCount
	{
		get
		{
			return this.mRefCount;
		}
	}

	public WWW mwww { get; set; }

	public AssetBundle GetAssetBundle()
	{
		if (this.mBundle != null)
		{
			this.mRefCount++;
		}
		return this.mBundle;
	}

	public void Release()
	{
		if (this.mRefCount == 0)
		{
			return;
		}
		this.mRefCount--;
		if (this.mRefCount == 0 && this.mwww != null)
		{
			this.mwww.Dispose();
			this.mwww = null;
		}
		if (this.mRefCount == 0 && this.mBundle)
		{
			this.mBundle.Unload(false);
		}
	}

	private AssetBundle mBundle;

	private int mRefCount;
}
