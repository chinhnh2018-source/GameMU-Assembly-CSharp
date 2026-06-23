using System;
using UnityEngine;

public class CDNDownLoadData
{
	public CDNDownLoadData(string assetbundleid, Action<string, CDNAssetBundle> onHandle = null)
	{
		this.assetbundleid = assetbundleid;
		this.onHandle = onHandle;
	}

	public string assetbundleid = string.Empty;

	public Action<string, CDNAssetBundle> onHandle;

	public WWW downWWW;
}
