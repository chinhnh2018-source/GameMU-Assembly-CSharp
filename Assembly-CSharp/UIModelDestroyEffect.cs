using System;
using UnityEngine;

public class UIModelDestroyEffect : MonoBehaviour
{
	public void DestroyEffect()
	{
		Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (this.effectObj != null && this.effectObj.Length > 0)
		{
			for (int i = 0; i < this.effectObj.Length; i++)
			{
				GameObject gameObject = this.effectObj[i];
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
			this.effectObj = null;
		}
	}

	public GameObject[] effectObj;
}
