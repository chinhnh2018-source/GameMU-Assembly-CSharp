using System;
using UnityEngine;

public class CharacterRenderManager : ManualUpdateBehaviour
{
	private void Awake()
	{
		this.mTransform = base.transform;
		this.mEnvRotMatID = Shader.PropertyToID("_RotateMatrix");
		if (Camera.main && !Camera.main.GetComponent<SetCameraShaderVariants>())
		{
			Camera.main.gameObject.AddComponent<SetCameraShaderVariants>();
		}
	}

	public override void ManualUpdate()
	{
		Matrix4x4 matrix4x = default(Matrix4x4);
		float num = Mathf.Sin(Time.time);
		float num2 = Mathf.Cos(Time.time);
		matrix4x[0, 0] = num2;
		matrix4x[0, 2] = -num;
		matrix4x[1, 1] = 1f;
		matrix4x[2, 0] = num;
		matrix4x[2, 2] = num2;
		matrix4x[3, 3] = 1f;
		Shader.SetGlobalMatrix(this.mEnvRotMatID, matrix4x);
	}

	private Transform mTransform;

	private int mEnvRotMatID;
}
