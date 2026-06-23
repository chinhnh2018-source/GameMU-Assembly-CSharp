using System;
using UnityEngine;

public class ManagedSkeleton : MonoBehaviour
{
	public void ManualRelease()
	{
		base.transform.parent = null;
		RoleSkeletonManager.ManualReleaseSkeleton(this.ID, base.gameObject);
		if (this.onDestroyed != null)
		{
			this.onDestroyed.Invoke(this.BundleID);
		}
		this.onDestroyed = null;
	}

	private void OnDestroy()
	{
		RoleSkeletonManager.ManualDelete(this.ID, base.gameObject);
		if (this.onDestroyed != null)
		{
			this.onDestroyed.Invoke(this.BundleID);
		}
		this.onDestroyed = null;
	}

	public string ID;

	public string BundleID;

	public Action<string> onDestroyed;
}
