using System;
using System.Collections;
using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.SetQueue());
	}

	private IEnumerator SetQueue()
	{
		while (base.enabled)
		{
			base.GetComponent<Renderer>().material.renderQueue = 3000 - (int)Camera.main.transform.InverseTransformPoint(base.transform.position).z * 2;
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}
}
