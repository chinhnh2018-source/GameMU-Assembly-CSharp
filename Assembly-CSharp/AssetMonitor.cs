using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class AssetMonitor
{
	public void LogAssetState(string url, AssetAction action, string assetName = null, Type assetType = null)
	{
		this.mAssetStates.Add(new AssetState
		{
			url = url,
			action = action,
			assetName = assetName,
			timeStamp = Time.time
		});
	}

	public void LogFile()
	{
		using (FileStream fileStream = new FileStream(Application.dataPath + "/../AssetLog.xml", 2))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<AssetState>));
			xmlSerializer.Serialize(fileStream, this.mAssetStates);
		}
	}

	private List<AssetState> mAssetStates = new List<AssetState>(2048);
}
