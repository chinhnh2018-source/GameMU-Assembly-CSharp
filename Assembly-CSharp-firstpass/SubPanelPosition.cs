using System;
using UnityEngine;

public class SubPanelPosition : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("SetPanel", 0.5f);
	}

	private void SetPanel()
	{
		this.parent = base.transform.parent;
		this.child = base.transform.GetChild(0);
		this.PanelScript = base.transform.GetComponent<UIPanel>();
		base.transform.parent = null;
		this.child.parent = null;
		if (this.screenDirection == SubPanelPosition.ScreenDirection.vertical)
		{
			this.rateX = (float)Screen.width / this.size;
			this.rateY = 1f;
			this.ScaleSize = base.transform.localScale.y;
		}
		else if (this.screenDirection == SubPanelPosition.ScreenDirection.horizontal)
		{
			this.rateX = 1f;
			this.rateY = (float)Screen.height / this.size;
			this.ScaleSize = base.transform.localScale.x;
		}
		base.transform.localScale = new Vector4(this.ScaleSize, this.ScaleSize, this.ScaleSize, this.ScaleSize);
		base.transform.parent = this.parent;
		this.child.parent = base.transform;
		this.PanelScript.clipRange = new Vector4(this.PanelScript.clipRange.x, this.PanelScript.clipRange.y, this.PanelScript.clipRange.z * this.rateX, this.PanelScript.clipRange.w * this.rateY);
	}

	public SubPanelPosition.ScreenDirection screenDirection;

	public float size;

	private Transform parent;

	private Transform child;

	private float ScaleSize;

	private float rateX;

	private float rateY;

	private UIPanel PanelScript;

	public enum ScreenDirection
	{
		horizontal,
		vertical
	}
}
