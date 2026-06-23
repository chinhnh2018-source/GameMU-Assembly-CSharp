using System;
using UnityEngine;

public class SetCameraShaderVariants : MonoBehaviour
{
	private static Texture2D RoughnessLUT
	{
		get
		{
			if (!SetCameraShaderVariants.roughnessLUT)
			{
				SetCameraShaderVariants.roughnessLUT = Resources.Load<Texture2D>("MUTexture/roughnessLUT");
			}
			return SetCameraShaderVariants.roughnessLUT;
		}
	}

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
		Shader.SetGlobalTexture("_NHxRoughness", SetCameraShaderVariants.RoughnessLUT);
	}

	private Transform mTransform;

	private int mWorld2HVMatID;

	private static Texture2D roughnessLUT;
}
