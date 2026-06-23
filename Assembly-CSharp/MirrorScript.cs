using System;
using UnityEngine;

public class MirrorScript : MonoBehaviour
{
	private void Awake()
	{
		this.mCachedTrans = base.transform;
		this.mainCamera = Camera.main;
		if (!this.mainCamera)
		{
			this.mainCamera = MainGame._current.MainCamera;
		}
		if (!this.mainCamera)
		{
			base.enabled = false;
			return;
		}
		GameObject gameObject = new GameObject();
		this.mMirrorCamera = gameObject.AddComponent<Camera>();
		this.mMirrorCamera.CopyFrom(this.mainCamera);
		gameObject.name = "MirrorCamera";
		this.rt = new RenderTexture(Screen.width / 2, Screen.height / 2, 16);
		this.mMirrorCamera.targetTexture = this.rt;
		this.mMirrorCamera.orthographic = false;
		this.mMirrorCamera.clearFlags = 2;
		this.mMirrorCamera.backgroundColor = Color.black;
		this.mMirrorCamera.cullingMask = this.MirrorLayer;
		this.render = base.GetComponent<Renderer>();
	}

	private void OnBecameVisible()
	{
		if (this.mMirrorCamera)
		{
			this.mMirrorCamera.enabled = true;
		}
	}

	private void OnBecameInvisible()
	{
		if (this.mMirrorCamera)
		{
			this.mMirrorCamera.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (null != this.mMirrorCamera && null != this.mMirrorCamera.gameObject)
		{
			Object.Destroy(this.mMirrorCamera.gameObject);
		}
	}

	private void Update()
	{
		if (!this.mMirrorCamera.enabled)
		{
			return;
		}
		if (!this.rt || !this.rt.IsCreated() || !this.mMirrorCamera.targetTexture)
		{
			if (this.rt)
			{
				this.rt.Release();
			}
			this.rt = new RenderTexture(Screen.width / 2, Screen.height / 2, 16);
			this.mMirrorCamera.targetTexture = this.rt;
			if (null != this.render)
			{
				for (int i = 0; i < this.render.materials.Length; i++)
				{
					if (this.render.materials[i].shader.name.Contains("MobileMirrorStandard"))
					{
						this.render.materials[i].SetTexture("_ReflectTex", this.rt);
					}
				}
			}
		}
		if (this.prePos != this.mainCamera.transform.position || this.preRotation != this.mainCamera.transform.rotation || this.mLastFOV != this.mainCamera.fieldOfView)
		{
			this.prePos = this.mainCamera.transform.position;
			this.preRotation = this.mainCamera.transform.rotation;
			if (this.mLastFOV != this.mainCamera.fieldOfView)
			{
				this.mMirrorCamera.fieldOfView = this.mainCamera.fieldOfView;
				this.mMirrorCamera.ResetProjectionMatrix();
				this.mLastFOV = this.mainCamera.fieldOfView;
			}
			this.mCamToMirror = this.mCachedTrans.position - this.mainCamera.transform.position;
			this.mMirrorNormal = this.mCachedTrans.up;
			this.mReflCamToMirror = this.mCamToMirror - 2f * Vector3.Dot(this.mCamToMirror, this.mMirrorNormal) * this.mMirrorNormal;
			this.mMirrorCamera.transform.position = this.mCachedTrans.position - this.mReflCamToMirror;
			Vector3 forward = this.mainCamera.transform.forward;
			Vector3 forward2 = forward - 2f * Vector3.Dot(forward, this.mMirrorNormal) * this.mMirrorNormal;
			this.mMirrorCamera.transform.forward = forward2;
			Matrix4x4 worldToCameraMatrix = this.mMirrorCamera.worldToCameraMatrix;
			Vector3 vector = worldToCameraMatrix.MultiplyPoint(base.transform.position);
			Vector3 normalized = worldToCameraMatrix.MultiplyVector(this.mMirrorNormal).normalized;
			if (vector == Vector3.zero)
			{
				return;
			}
			this.mMirrorCamera.projectionMatrix = this.mMirrorCamera.CalculateObliqueMatrix(new Vector4(normalized.x, normalized.y, normalized.z, -Vector3.Dot(vector, normalized)));
			if (null != this.render)
			{
				for (int j = 0; j < this.render.materials.Length; j++)
				{
					if (this.render.materials[j].shader.name.Contains("MobileMirrorStandard"))
					{
						this.render.materials[j].SetTexture("_ReflectTex", this.rt);
						this.render.materials[j].SetMatrix("_ProjMat", this.mMirrorCamera.projectionMatrix);
						this.render.materials[j].SetMatrix("_ProjMat1", this.mMirrorCamera.worldToCameraMatrix);
					}
				}
			}
		}
	}

	public LayerMask MirrorLayer;

	private Camera mainCamera;

	private Camera mMirrorCamera;

	private Vector3 mMirrorNormal;

	private Vector3 mCamToMirror;

	private Vector3 mReflCamToMirror;

	private Transform mCachedTrans;

	private RenderTexture rt;

	private Renderer render;

	private Vector3 prePos = Vector3.zero;

	private float mLastFOV;

	private Quaternion preRotation = default(Quaternion);
}
