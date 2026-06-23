using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToHideRoof : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		Transform parent = col.transform.parent;
		if (null == parent)
		{
			return;
		}
		int childCount = parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (child.gameObject.name.EndsWith("_hide"))
			{
				child.gameObject.SetActive(false);
				this.hideGameObjects.Add(child.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (this.hideGameObjects.Count > 0)
		{
			for (int i = 0; i < this.hideGameObjects.Count; i++)
			{
				this.hideGameObjects[i].SetActive(true);
			}
			this.hideGameObjects.Clear();
		}
	}

	private List<GameObject> hideGameObjects = new List<GameObject>();
}
