using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffectManager : MonoBehaviour
{
	private void Start()
	{
		Transform transform = base.transform;
		this.RootPos = transform.localPosition;
		this.WalkAllChildrens(transform, this.RootPos);
		base.StartCoroutine(this.HandleMapEffect());
	}

	private void WalkAllChildrens(Transform parent, Vector3 parentPos)
	{
		int childCount = parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (Vector3.zero == child.localPosition)
			{
				this.WalkAllChildrens(child, parentPos + child.localPosition);
			}
			else
			{
				Vector3 vector = parentPos + child.localPosition;
				this.MyChildrens.Add(child);
				this.MyChildrensPos.Add(vector);
			}
		}
	}

	private IEnumerator HandleMapEffect()
	{
		while (base.enabled)
		{
			for (int i = 0; i < this.MyChildrens.Count; i++)
			{
				Vector3 pos = this.MyChildrensPos[i];
				if (this.MyChildrens[i] != null)
				{
					if (Vector3.Distance(pos, LeaderInfo.LeaderPos) >= 25f)
					{
						this.MyChildrens[i].gameObject.SetActive(false);
					}
					else
					{
						this.MyChildrens[i].gameObject.SetActive(true);
					}
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void ClearMapEffect()
	{
		this.MyChildrensPos.Clear();
		for (int i = 0; i < this.MyChildrens.Count; i++)
		{
			this.MyChildrens[i].gameObject.SetActive(true);
			Object.Destroy(this.MyChildrens[i].gameObject);
		}
		this.MyChildrens.Clear();
	}

	private List<Transform> MyChildrens = new List<Transform>();

	private List<Vector3> MyChildrensPos = new List<Vector3>();

	private Vector3 RootPos = Vector3.zero;
}
