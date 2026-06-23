using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
	[ExecuteInEditMode, RequireComponent(typeof(WaterBase))]
	public class PlanarReflection : MonoBehaviour
	{
		public void Start()
		{
			this.m_SharedMaterial = ((WaterBase)base.gameObject.GetComponent(typeof(WaterBase))).sharedMaterial;
		}

		private Camera CreateReflectionCameraFor(Camera cam)
		{
			string text = base.gameObject.name + "Reflection" + cam.name;
			GameObject gameObject = GameObject.Find(text);
			if (!gameObject)
			{
				gameObject = new GameObject(text, new Type[]
				{
					typeof(Camera)
				});
			}
			if (!gameObject.GetComponent(typeof(Camera)))
			{
				gameObject.AddComponent(typeof(Camera));
			}
			Camera component = gameObject.GetComponent<Camera>();
			component.backgroundColor = this.clearColor;
			component.clearFlags = ((!this.reflectSkybox) ? 2 : 1);
			this.SetStandardCameraParameter(component, this.reflectionMask);
			if (!component.targetTexture)
			{
				component.targetTexture = this.CreateTextureFor(cam);
			}
			return component;
		}

		private void SetStandardCameraParameter(Camera cam, LayerMask mask)
		{
			cam.cullingMask = (mask & ~(1 << LayerMask.NameToLayer("Water")));
			cam.backgroundColor = Color.black;
			cam.enabled = false;
		}

		private RenderTexture CreateTextureFor(Camera cam)
		{
			return new RenderTexture(Mathf.FloorToInt((float)cam.pixelWidth * 0.5f), Mathf.FloorToInt((float)cam.pixelHeight * 0.5f), 24)
			{
				hideFlags = 52
			};
		}

		public void RenderHelpCameras(Camera currentCam)
		{
			if (this.m_HelperCameras == null)
			{
				this.m_HelperCameras = new Dictionary<Camera, bool>();
			}
			if (!this.m_HelperCameras.ContainsKey(currentCam))
			{
				this.m_HelperCameras.Add(currentCam, false);
			}
			if (this.m_HelperCameras[currentCam])
			{
				return;
			}
			if (!this.m_ReflectionCamera)
			{
				this.m_ReflectionCamera = this.CreateReflectionCameraFor(currentCam);
			}
			this.RenderReflectionFor(currentCam, this.m_ReflectionCamera);
			this.m_HelperCameras[currentCam] = true;
		}

		public void LateUpdate()
		{
			if (this.m_HelperCameras != null)
			{
				this.m_HelperCameras.Clear();
			}
		}

		public void WaterTileBeingRendered(Transform tr, Camera currentCam)
		{
			this.RenderHelpCameras(currentCam);
			if (this.m_ReflectionCamera && this.m_SharedMaterial)
			{
				this.m_SharedMaterial.SetTexture(this.reflectionSampler, this.m_ReflectionCamera.targetTexture);
			}
		}

		public void OnEnable()
		{
			Shader.EnableKeyword("WATER_REFLECTIVE");
			Shader.DisableKeyword("WATER_SIMPLE");
		}

		public void OnDisable()
		{
			Shader.EnableKeyword("WATER_SIMPLE");
			Shader.DisableKeyword("WATER_REFLECTIVE");
		}

		private void RenderReflectionFor(Camera cam, Camera reflectCamera)
		{
			if (!reflectCamera)
			{
				return;
			}
			if (this.m_SharedMaterial && !this.m_SharedMaterial.HasProperty(this.reflectionSampler))
			{
				return;
			}
			reflectCamera.cullingMask = (this.reflectionMask & ~(1 << LayerMask.NameToLayer("Water")));
			this.SaneCameraSettings(reflectCamera);
			reflectCamera.backgroundColor = this.clearColor;
			reflectCamera.clearFlags = ((!this.reflectSkybox) ? 2 : 1);
			if (this.reflectSkybox && cam.gameObject.GetComponent(typeof(Skybox)))
			{
				Skybox skybox = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
				if (!skybox)
				{
					skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
				}
				skybox.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
			}
			GL.invertCulling = true;
			Transform transform = base.transform;
			Vector3 eulerAngles = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
			reflectCamera.transform.position = cam.transform.position;
			Vector3 position = transform.transform.position;
			position.y = transform.position.y;
			Vector3 up = transform.transform.up;
			float num = -Vector3.Dot(up, position) - this.clipPlaneOffset;
			Vector4 plane;
			plane..ctor(up.x, up.y, up.z, num);
			Matrix4x4 matrix4x = Matrix4x4.zero;
			matrix4x = PlanarReflection.CalculateReflectionMatrix(matrix4x, plane);
			this.m_Oldpos = cam.transform.position;
			Vector3 position2 = matrix4x.MultiplyPoint(this.m_Oldpos);
			reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
			Vector4 clipPlane = this.CameraSpacePlane(reflectCamera, position, up, 1f);
			Matrix4x4 matrix4x2 = cam.projectionMatrix;
			matrix4x2 = PlanarReflection.CalculateObliqueMatrix(matrix4x2, clipPlane);
			reflectCamera.projectionMatrix = matrix4x2;
			reflectCamera.transform.position = position2;
			Vector3 eulerAngles2 = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
			reflectCamera.Render();
			GL.invertCulling = false;
		}

		private void SaneCameraSettings(Camera helperCam)
		{
			helperCam.depthTextureMode = 0;
			helperCam.backgroundColor = Color.black;
			helperCam.clearFlags = 2;
			helperCam.renderingPath = 1;
		}

		private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
		{
			Vector4 vector = projection.inverse * new Vector4(PlanarReflection.Sgn(clipPlane.x), PlanarReflection.Sgn(clipPlane.y), 1f, 1f);
			Vector4 vector2 = clipPlane * (2f / Vector4.Dot(clipPlane, vector));
			projection[2] = vector2.x - projection[3];
			projection[6] = vector2.y - projection[7];
			projection[10] = vector2.z - projection[11];
			projection[14] = vector2.w - projection[15];
			return projection;
		}

		private static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
			return reflectionMat;
		}

		private static float Sgn(float a)
		{
			if (a > 0f)
			{
				return 1f;
			}
			if (a < 0f)
			{
				return -1f;
			}
			return 0f;
		}

		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 vector = pos + normal * this.clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Vector3 vector2 = worldToCameraMatrix.MultiplyPoint(vector);
			Vector3 vector3 = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(vector3.x, vector3.y, vector3.z, -Vector3.Dot(vector2, vector3));
		}

		public LayerMask reflectionMask;

		public bool reflectSkybox;

		public Color clearColor = Color.grey;

		public string reflectionSampler = "_ReflectionTex";

		public float clipPlaneOffset = 0.07f;

		private Vector3 m_Oldpos;

		private Camera m_ReflectionCamera;

		private Material m_SharedMaterial;

		private Dictionary<Camera, bool> m_HelperCameras;
	}
}
