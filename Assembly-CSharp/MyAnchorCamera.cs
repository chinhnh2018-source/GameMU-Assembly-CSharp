using System;
using UnityEngine;

[ExecuteInEditMode]
public class MyAnchorCamera : MonoBehaviour
{
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		if (this.cam != null)
		{
			this.UpdateCameraMatrix();
		}
	}

	private void Update()
	{
		if (this.mLastHeight != Screen.height || this.mLastWidth != Screen.width)
		{
			this.mLastHeight = Screen.height;
			this.mLastWidth = Screen.width;
			if (this.cam != null)
			{
				this.mCamWidth = this.cam.pixelWidth;
				this.mCamHeight = this.cam.pixelHeight;
				this.mCamFar = this.cam.farClipPlane;
				this.mCamNear = this.cam.nearClipPlane;
			}
			this.UpdateCameraMatrix();
		}
		else if (this.cam && (this.mCamHeight != this.cam.pixelHeight || this.mCamWidth != this.cam.pixelWidth || this.mCamFar != this.cam.farClipPlane || this.mCamNear != this.cam.nearClipPlane))
		{
			this.mLastHeight = Screen.height;
			this.mLastWidth = Screen.width;
			this.mCamWidth = this.cam.pixelWidth;
			this.mCamHeight = this.cam.pixelHeight;
			this.mCamFar = this.cam.farClipPlane;
			this.mCamNear = this.cam.nearClipPlane;
			this.UpdateCameraMatrix();
		}
	}

	private void SelectMode()
	{
		switch (this.Model)
		{
		case MyAnchorCamera.AnchorModel.Auto:
			if (this.suitableUI_height != 0f && this.suitableUI_width != 0f)
			{
				if (Mathf.Abs((float)Screen.width - this.suitableUI_width) > Mathf.Abs((float)Screen.height - this.suitableUI_height))
				{
					this.scale = (float)Screen.width / this.suitableUI_width;
				}
				else
				{
					this.scale = (float)Screen.height / this.suitableUI_height;
				}
			}
			else
			{
				this.scale = 1f;
			}
			break;
		case MyAnchorCamera.AnchorModel.Tall:
			if (this.suitableUI_height != 0f)
			{
				this.scale = (float)Screen.height / this.suitableUI_height;
			}
			else
			{
				this.scale = 1f;
			}
			break;
		case MyAnchorCamera.AnchorModel.Width:
			if (this.suitableUI_width != 0f)
			{
				this.scale = (float)Screen.width / this.suitableUI_width;
				if (this.suitableUI_height != 0f)
				{
					float num = (float)Screen.height / this.suitableUI_height;
					if (num > 1f && this.scale > num)
					{
						this.scale = num;
					}
				}
			}
			else
			{
				this.scale = 1f;
			}
			break;
		}
		this.resolutionScale = this.scale;
		if (this.isNGUIHierarchy)
		{
			if (this.UIRootTransform == null)
			{
				this.UIRootTransform = MyAnchorCameraTool.FindInParents<UIRoot>(base.gameObject).transform;
			}
			float x = this.UIRootTransform.localScale.x;
			if (x != 0f)
			{
				this.scale *= 1f / x;
			}
		}
	}

	public void UpdateCameraMatrix()
	{
		this.SelectMode();
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)this.cam.pixelWidth;
		float num4 = (float)this.cam.pixelHeight;
		float farClipPlane = this.cam.farClipPlane;
		float near = this.cam.near;
		float num5 = 2f / (num3 - num) * this.scale;
		float num6 = 2f / (num4 - num2) * this.scale;
		float num7 = -2f / (farClipPlane - near);
		float num8 = 0f;
		float num9 = 0f;
		float num10;
		if (this.isNGUIHierarchy)
		{
			num10 = 0f;
		}
		else
		{
			num10 = -1f;
		}
		Matrix4x4 projectionMatrix = default(Matrix4x4);
		projectionMatrix[0, 0] = num5;
		projectionMatrix[0, 1] = 0f;
		projectionMatrix[0, 2] = 0f;
		projectionMatrix[0, 3] = num8;
		projectionMatrix[1, 0] = 0f;
		projectionMatrix[1, 1] = num6;
		projectionMatrix[1, 2] = 0f;
		projectionMatrix[1, 3] = num9;
		projectionMatrix[2, 0] = 0f;
		projectionMatrix[2, 1] = 0f;
		projectionMatrix[2, 2] = num7;
		projectionMatrix[2, 3] = num10;
		projectionMatrix[3, 0] = 0f;
		projectionMatrix[3, 1] = 0f;
		projectionMatrix[3, 2] = 0f;
		projectionMatrix[3, 3] = 1f;
		this.cam.projectionMatrix = projectionMatrix;
	}

	public MyAnchorCamera.AnchorModel Model;

	public float resolutionScale = 1f;

	public float scale = 1f;

	public Camera cam;

	public float suitableUI_width;

	public float suitableUI_height;

	public bool ShowResolution;

	public bool isNGUIHierarchy;

	private int mLastWidth;

	private int mLastHeight;

	private int mCamWidth;

	private int mCamHeight;

	private float mCamFar;

	private float mCamNear;

	private Transform UIRootTransform;

	public enum AnchorModel
	{
		Auto,
		Tall,
		Width
	}
}
