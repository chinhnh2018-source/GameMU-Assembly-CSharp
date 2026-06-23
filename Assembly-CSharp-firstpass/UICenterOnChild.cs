using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center On Child")]
public class UICenterOnChild : MonoBehaviour
{
	public GameObject centeredObject
	{
		get
		{
			return this.mCenteredObject;
		}
	}

	private void OnEnable()
	{
		this.Recenter();
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			this.Recenter();
		}
	}

	public void Recenter()
	{
		if (this.mDrag == null)
		{
			this.mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (this.mDrag == null)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					base.GetType(),
					" requires ",
					typeof(UIDraggablePanel),
					" on a parent object in order to work"
				}), this);
				base.enabled = false;
				return;
			}
			this.mDrag.onDragFinished = new UIDraggablePanel.OnDragFinished(this.OnDragFinished);
			if (this.mDrag.horizontalScrollBar != null)
			{
				this.mDrag.horizontalScrollBar.onDragFinished = new UIScrollBar.OnDragFinished(this.OnDragFinished);
			}
			if (this.mDrag.verticalScrollBar != null)
			{
				this.mDrag.verticalScrollBar.onDragFinished = new UIScrollBar.OnDragFinished(this.OnDragFinished);
			}
		}
		if (this.mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = this.mDrag.panel.clipRange;
		Transform cachedTransform = this.mDrag.panel.cachedTransform;
		Vector3 vector = cachedTransform.localPosition;
		vector.x += clipRange.x;
		vector.y += clipRange.y;
		vector = cachedTransform.parent.TransformPoint(vector);
		Vector3 vector2 = vector - this.mDrag.currentMomentum * (this.mDrag.momentumAmount * 0.1f);
		this.mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int i = 0;
		int childCount = transform2.childCount;
		while (i < childCount)
		{
			Transform child = transform2.GetChild(i);
			if (child.gameObject.activeInHierarchy)
			{
				float num2 = Vector3.SqrMagnitude(child.position - vector2);
				if (num2 < num)
				{
					num = num2;
					transform = child;
				}
			}
			i++;
		}
		if (transform != null)
		{
			this.mCenteredObject = transform.gameObject;
			Vector3 vector3 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector4 = cachedTransform.InverseTransformPoint(vector);
			Vector3 vector5 = vector3 - vector4;
			if (this.mDrag.scale.x == 0f)
			{
				vector5.x = 0f;
			}
			if (this.mDrag.scale.y == 0f)
			{
				vector5.y = 0f;
			}
			if (this.mDrag.scale.z == 0f)
			{
				vector5.z = 0f;
			}
			SpringPanel.Begin(this.mDrag.gameObject, cachedTransform.localPosition - vector5, this.springStrength).onFinished = this.onFinished;
		}
		else
		{
			this.mCenteredObject = null;
		}
	}

	public float springStrength = 8f;

	public SpringPanel.OnFinished onFinished;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;
}
