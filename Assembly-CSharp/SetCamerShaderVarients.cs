using System;
using UnityEngine;

public class SetCamerShaderVarients : MonoBehaviour
{
	private void Awake()
	{
		this.mTransform = base.transform;
		this.mWorld2HVMatID = Shader.PropertyToID("_Wolrd2HV");
	}

	private void OnPreRender()
	{
		Vector4 vector = Camera.main.transform.right;
		Vector4 vector2 = new Vector3(0f, 1f, 0f);
		Vector4 vector3;
		vector3..ctor(vector.z, 0f, -vector.x);
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x.SetRow(0, vector);
		matrix4x.SetRow(1, vector2);
		matrix4x.SetRow(2, vector3);
		Shader.SetGlobalMatrix(this.mWorld2HVMatID, matrix4x);
	}

	private Transform mTransform;

	private int mWorld2HVMatID;
}
