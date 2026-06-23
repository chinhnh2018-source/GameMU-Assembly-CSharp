using System;

public class ErrorAssetData
{
	public ErrorAssetData(string path, string error, int count)
	{
		this.assetBundleID = path;
		this.error = error;
		this.count = count;
	}

	public string assetBundleID = string.Empty;

	public string error = string.Empty;

	public int count;

	public int MaxCount = 5;
}
