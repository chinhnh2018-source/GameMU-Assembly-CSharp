using System;
using System.Collections.Generic;
using UnityEngine;

public class MuAsset : MonoBehaviour
{
	public void ReleaseResource(string assetID)
	{
		if (string.IsNullOrEmpty(assetID))
		{
			return;
		}
		for (int i = 0; i < this.assetIDs.Count; i++)
		{
			if (this.assetIDs[i].Equals(assetID))
			{
				this.assetIDs.Remove(this.assetIDs[i]);
				if (this.onDeleteOne != null)
				{
					this.onDeleteOne.Invoke(assetID);
				}
				return;
			}
		}
	}

	private void OnDestroy()
	{
		if (this.onDestroy != null)
		{
			this.onDestroy.Invoke(this.assetIDs);
		}
	}

	public List<string> assetIDs = new List<string>();

	public Action<List<string>> onDestroy;

	public Action<string> onDeleteOne;
}
