using System;
using UnityEngine;

public class GetRange : MonoBehaviour
{
	private void Start()
	{
	}

	public void BoxChip(GameObject obj)
	{
		UIPanel component = base.transform.GetComponent<UIPanel>();
		float num = component.clipRange.z / 2f - component.clipRange.x;
		float num2 = component.clipRange.w / 2f - component.clipRange.y;
		float num3 = component.clipRange.z / 2f + component.clipRange.x;
		float num4 = component.clipRange.w / 2f + component.clipRange.y;
		Vector3 vector = this.cam.WorldToViewportPoint(base.transform.TransformPoint(-num, -num2, 0f));
		Vector3 vector2 = this.cam.WorldToViewportPoint(base.transform.TransformPoint(num3, num4, 0f));
		Vector4 vector3;
		vector3..ctor(vector.x, vector.y, vector2.x, vector2.y);
		obj.GetComponent<Renderer>().material.SetVector("_Panel", vector3);
	}

	public Camera cam;
}
