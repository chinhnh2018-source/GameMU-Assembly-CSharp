using System;
using UnityEngine;

public class AdjustRenderQueue : MonoBehaviour
{
	private void Start()
	{
		if (this.renderQueueOffsets != null)
		{
			int num = 0;
			while (num < this.renderQueueOffsets.Length && num < base.GetComponent<Renderer>().materials.Length)
			{
				base.GetComponent<Renderer>().materials[num].renderQueue += this.renderQueueOffsets[num];
				num++;
			}
		}
	}

	public int[] renderQueueOffsets;
}
