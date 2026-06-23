using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
	private void FindPanel()
	{
		this.mPanel = ((!(this.target != null)) ? null : UIPanel.Find(this.target.transform, false));
		if (this.mPanel == null)
		{
			this.restrictWithinPanel = false;
		}
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
		{
			this.mPressed = pressed;
			if (pressed)
			{
				if (this.restrictWithinPanel && this.mPanel == null)
				{
					this.FindPanel();
				}
				if (this.restrictWithinPanel)
				{
					this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
				}
				this.mMomentum = Vector3.zero;
				this.mScroll = 0f;
				SpringPosition component = this.target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
				this.mLastPos = UICamera.lastHit.point;
				Transform transform = UICamera.currentCamera.transform;
				this.mPlane = new Plane(((!(this.mPanel != null)) ? transform.rotation : this.mPanel.cachedTransform.rotation) * Vector3.back, this.mLastPos);
			}
			else if (this.restrictWithinPanel && this.mPanel.clipping != null && this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
			{
				this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, false);
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
		{
			UICamera.currentTouch.clickNotification = 2;
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float num = 0f;
			if (this.mPlane.Raycast(ray, ref num))
			{
				Vector3 point = ray.GetPoint(num);
				Vector3 vector = point - this.mLastPos;
				this.mLastPos = point;
				if (vector.x != 0f || vector.y != 0f)
				{
					vector = this.target.InverseTransformDirection(vector);
					vector.Scale(this.scale);
					vector = this.target.TransformDirection(vector);
				}
				if (this.dragEffect != UIDragObject.DragEffect.None)
				{
					this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
				}
				if (this.restrictWithinPanel)
				{
					Vector3 localPosition = this.target.localPosition;
					this.target.position += vector;
					this.mBounds.center = this.mBounds.center + (this.target.localPosition - localPosition);
					if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.mPanel.clipping != null && this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, true))
					{
						this.mMomentum = Vector3.zero;
						this.mScroll = 0f;
					}
				}
				else
				{
					this.target.position += vector;
				}
			}
		}
	}

	private void LateUpdate()
	{
		float num = base.UpdateRealTimeDelta();
		if (this.target == null)
		{
			return;
		}
		if (this.mPressed)
		{
			SpringPosition component = this.target.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			this.mScroll = 0f;
		}
		else
		{
			this.mMomentum += this.scale * (-this.mScroll * 0.05f);
			this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, num);
			if (this.mMomentum.magnitude > 0.0001f)
			{
				if (this.mPanel == null)
				{
					this.FindPanel();
				}
				if (this.mPanel != null)
				{
					this.target.position += NGUIMath.SpringDampen(ref this.mMomentum, 9f, num);
					if (this.restrictWithinPanel && this.mPanel.clipping != null)
					{
						this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
						if (!this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, this.dragEffect == UIDragObject.DragEffect.None))
						{
							SpringPosition component2 = this.target.GetComponent<SpringPosition>();
							if (component2 != null)
							{
								component2.enabled = false;
							}
						}
						ListBox component3 = this.target.GetComponent<ListBox>();
						if (null != component3)
						{
							component3.SpringPositionProc();
						}
					}
					return;
				}
			}
			else
			{
				this.mScroll = 0f;
			}
		}
		NGUIMath.SpringDampen(ref this.mMomentum, 9f, num);
	}

	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
			{
				this.mScroll = 0f;
			}
			this.mScroll += delta * this.scrollWheelFactor;
		}
	}

	public Transform target;

	public Vector3 scale = Vector3.one;

	public float scrollWheelFactor;

	public bool restrictWithinPanel;

	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	public float momentumAmount = 35f;

	private Plane mPlane;

	private Vector3 mLastPos;

	private UIPanel mPanel;

	private bool mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private float mScroll;

	private Bounds mBounds;

	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}
}
