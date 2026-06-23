using System;
using UnityEngine;

public class FxUISoftClip : MonoBehaviour
{
	private void Awake()
	{
		this.mPanel = base.GetComponentInParent<UIPanel>();
		if (!this.mPanel)
		{
			base.enabled = false;
			return;
		}
		Renderer[] components = base.GetComponents<Renderer>();
		Vector4 clipRange = this.mPanel.clipRange;
		Vector2 vector;
		vector..ctor(1000f, 1000f);
		if (this.mPanel.clipSoftness.x > 0f)
		{
			vector.x = clipRange.z / this.mPanel.clipSoftness.x;
		}
		if (this.mPanel.clipSoftness.y > 0f)
		{
			vector.y = clipRange.w / this.mPanel.clipSoftness.y;
		}
		Vector4 vector2 = Vector4.one;
		if (clipRange.z != 0f && clipRange.w != 0f)
		{
			vector2 = new Vector4(1f / clipRange.z, 1f / clipRange.w, -clipRange.x / clipRange.z, -clipRange.y / clipRange.w) * 2f;
		}
		for (int i = 0; i < components.Length; i++)
		{
			Material[] sharedMaterials = components[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				sharedMaterials[j].SetVector("_ClipParams", vector2);
				sharedMaterials[j].SetVector("_ClipSharpness", vector);
				sharedMaterials[j].SetMatrix("_World2Panel", this.mPanel.worldToLocal);
			}
		}
	}

	private UIPanel mPanel;
}
