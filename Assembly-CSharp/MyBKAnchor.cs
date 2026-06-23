using System;
using UnityEngine;

[ExecuteInEditMode]
public class MyBKAnchor : MonoBehaviour
{
	private void Start()
	{
		if (this.mycamera == null)
		{
			this.mycamera = MyAnchorCameraTool.FindCameraForLayer(base.gameObject.layer);
			this.MyAnchorCamera = this.mycamera.GetComponent<MyAnchorCamera>();
			this.BeforeScale = base.transform.localScale;
		}
	}

	private void Update()
	{
		this.BKAdapt();
	}

	private void BKAdapt()
	{
		this.anchorScale = this.MyAnchorCamera.resolutionScale;
		this.relativeSize = new Vector2(this.MyAnchorCamera.suitableUI_width, this.MyAnchorCamera.suitableUI_height);
		this.mRect = this.mycamera.pixelRect;
		float width = this.mRect.width;
		float height = this.mRect.height;
		switch (this.anchor)
		{
		case MyBKAnchor.BKAnchor.Anchor_x:
			if (this.relativeSize.x != 0f && this.anchorScale != 0f)
			{
				base.transform.localScale = new Vector3(this.BeforeScale.x / this.relativeSize.x * width / this.anchorScale, this.BeforeScale.y, this.BeforeScale.z);
			}
			break;
		case MyBKAnchor.BKAnchor.Anchor_y:
			if (this.relativeSize.y != 0f && this.anchorScale != 0f)
			{
				base.transform.localScale = new Vector3(this.BeforeScale.x, this.BeforeScale.y / this.relativeSize.y * height / this.anchorScale, this.BeforeScale.z);
			}
			break;
		case MyBKAnchor.BKAnchor.Anchor_x_y:
			if (this.relativeSize.x != 0f && this.anchorScale != 0f && this.relativeSize.y != 0f && this.anchorScale != 0f)
			{
				base.transform.localScale = new Vector3(this.BeforeScale.x / this.relativeSize.x * width / this.anchorScale, this.BeforeScale.y / this.relativeSize.y * height / this.anchorScale, this.BeforeScale.z);
			}
			break;
		case MyBKAnchor.BKAnchor.BasedOnY:
			if (this.relativeSize.x != 0f && this.anchorScale != 0f && this.relativeSize.y != 0f && this.anchorScale != 0f)
			{
				base.transform.localScale = new Vector3(this.BeforeScale.x / this.relativeSize.x * height / this.anchorScale, this.BeforeScale.y / this.relativeSize.y * height / this.anchorScale, this.BeforeScale.z);
			}
			break;
		}
	}

	private Vector2 relativeSize = Vector2.zero;

	private Camera mycamera;

	private MyAnchorCamera MyAnchorCamera;

	private Rect mRect;

	public MyBKAnchor.BKAnchor anchor;

	private float anchorScale = 1f;

	private Vector3 BeforeScale = Vector3.zero;

	public enum BKAnchor
	{
		Anchor_x,
		Anchor_y,
		Anchor_x_y,
		BasedOnY
	}
}
