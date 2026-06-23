using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

[ExecuteInEditMode]
public class MyUIAnchor : MonoBehaviour
{
	private void Awake()
	{
		this._transform = base.transform;
	}

	private void Start()
	{
		this.UpdateTransform();
		if (null != Global.UICamera)
		{
			this.MyAnchorCamera = Global.UICamera.GetComponent<MyAnchorCamera>();
		}
	}

	private void UpdateTransform()
	{
		if (this.MyAnchorCamera != null && this.MyAnchorCamera.scale != 0f)
		{
			this.anchorScale = this.MyAnchorCamera.resolutionScale;
			Vector3 localPosition = this._transform.localPosition;
			Vector3 zero = Vector3.zero;
			if (this.anchorScale != 0f)
			{
				switch (this.anchor)
				{
				case MyUIAnchor.Anchor.Upper:
					zero..ctor(0f, (float)Screen.height / 2f / this.anchorScale, localPosition.z);
					break;
				case MyUIAnchor.Anchor.Lower:
					zero..ctor(0f, (float)(-(float)Screen.height) / 2f / this.anchorScale, localPosition.z);
					break;
				case MyUIAnchor.Anchor.MiddleLeft:
					zero..ctor((float)(-(float)Screen.width) / 2f / this.anchorScale, 0f, localPosition.z);
					break;
				case MyUIAnchor.Anchor.MiddleCenter:
					zero..ctor(0f, 0f, localPosition.z);
					break;
				case MyUIAnchor.Anchor.MiddleRight:
					zero..ctor((float)Screen.width / 2f / this.anchorScale, 0f, localPosition.z);
					break;
				case MyUIAnchor.Anchor.TopLeftCorner:
					zero..ctor((float)(-(float)Screen.width) / 2f / this.anchorScale, (float)Screen.height / 2f / this.anchorScale, localPosition.z);
					break;
				case MyUIAnchor.Anchor.TopRightCorner:
					zero..ctor((float)Screen.width / 2f / this.anchorScale, (float)Screen.height / 2f / this.anchorScale, localPosition.z);
					break;
				case MyUIAnchor.Anchor.LowerLeftCorner:
					zero..ctor((float)(-(float)Screen.width) / 2f / this.anchorScale, (float)(-(float)Screen.height) / 2f / this.anchorScale, localPosition.z);
					break;
				case MyUIAnchor.Anchor.LowerRightCorner:
					zero..ctor((float)Screen.width / 2f / this.anchorScale, (float)(-(float)Screen.height) / 2f / this.anchorScale, localPosition.z);
					break;
				}
			}
			this._transform.localPosition = zero + new Vector3(this.offset.x, this.offset.y, 0f);
		}
	}

	private void Update()
	{
		this.UpdateTransform();
	}

	public MyUIAnchor.Anchor anchor;

	public Vector2 offset = Vector2.zero;

	public MyAnchorCamera MyAnchorCamera;

	public bool IsSettedOffsetForIphoneX;

	private Transform _transform;

	private float anchorScale = 1f;

	public enum Anchor
	{
		Upper,
		Lower,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		TopLeftCorner,
		TopRightCorner,
		LowerLeftCorner,
		LowerRightCorner
	}
}
