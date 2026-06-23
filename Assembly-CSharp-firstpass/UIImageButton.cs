using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Image Button"), ExecuteInEditMode]
public class UIImageButton : MonoBehaviour
{
	public bool isEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			return component && component.enabled;
		}
		set
		{
			Collider component = base.GetComponent<Collider>();
			if (!component)
			{
				return;
			}
			if (component.enabled != value)
			{
				component.enabled = value;
				this.UpdateImage();
			}
		}
	}

	protected virtual void Awake()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<UISprite>();
		}
	}

	private void OnEnable()
	{
		this.UpdateImage();
		if (this.mHighlighted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	private void OnDisable()
	{
		if (this.ScaleTarget != null)
		{
			foreach (Transform transform in this.ScaleTarget)
			{
				if (null != transform)
				{
					TweenScale component = transform.GetComponent<TweenScale>();
					if (component != null)
					{
						component.scale = this.ScaleStart;
						component.enabled = false;
					}
				}
			}
		}
		if (this.OffsetTarget != null)
		{
			foreach (Transform transform2 in this.OffsetTarget)
			{
				if (null != transform2)
				{
					TweenPosition component2 = transform2.GetComponent<TweenPosition>();
					if (component2 != null)
					{
						component2.position = this.OffsetStart;
						component2.enabled = false;
					}
				}
			}
		}
	}

	private void UpdateImage()
	{
		if (this.target != null)
		{
			if (this.isEnabled)
			{
				this.target.spriteName = ((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
			}
			else
			{
				this.target.spriteName = this.disabledSprite;
			}
			if (this.Width > 0f && this.Height > 0f)
			{
				this.target.transform.localScale = new Vector3(this.Width, this.Height, 1f);
			}
			else if (this.target.isActiveAndEnabled)
			{
				this.target.MakePixelPerfect();
			}
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.isEnabled && this.target != null)
		{
			this.target.spriteName = ((!isOver) ? this.normalSprite : this.hoverSprite);
			if (this.Width > 0f && this.Height > 0f)
			{
				this.target.transform.localScale = new Vector3(this.Width, this.Height, 1f);
			}
			else
			{
				this.target.MakePixelPerfect();
			}
		}
		if (this.ScaleTarget != null)
		{
			foreach (Transform transform in this.ScaleTarget)
			{
				if (null != transform)
				{
					TweenScale.Begin(transform.gameObject, this.ScaleDuration, (!isOver) ? this.ScaleStart : Vector3.Scale(this.ScaleStart, this.ScaleHover)).method = UITweener.Method.EaseInOut;
				}
			}
		}
		if (this.OffsetTarget != null)
		{
			foreach (Transform transform2 in this.OffsetTarget)
			{
				if (null != transform2)
				{
					TweenPosition.Begin(transform2.gameObject, this.OffsetDuration, (!isOver) ? this.OffsetStart : (this.OffsetStart + this.OffseteHover)).method = UITweener.Method.EaseInOut;
				}
			}
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			this.target.spriteName = this.pressedSprite;
			if (this.Width > 0f && this.Height > 0f)
			{
				this.target.transform.localScale = new Vector3(this.Width, this.Height, 1f);
			}
			else
			{
				this.target.MakePixelPerfect();
			}
		}
		else
		{
			this.UpdateImage();
		}
		if (this.ScaleTarget != null)
		{
			foreach (Transform transform in this.ScaleTarget)
			{
				if (null != transform)
				{
					TweenScale.Begin(transform.gameObject, this.ScaleDuration, (!pressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.ScaleStart : Vector3.Scale(this.ScaleStart, this.ScaleHover)) : Vector3.Scale(this.ScaleStart, this.ScalePressed)).method = UITweener.Method.EaseInOut;
				}
			}
		}
		if (this.OffsetTarget != null)
		{
			foreach (Transform transform2 in this.OffsetTarget)
			{
				if (null != transform2)
				{
					TweenPosition.Begin(transform2.gameObject, this.OffsetDuration, (!pressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.OffsetStart : (this.OffsetStart + this.OffseteHover)) : (this.OffsetStart + this.OffseteHover)).method = UITweener.Method.EaseInOut;
				}
			}
		}
	}

	public void Refresh()
	{
		this.UpdateImage();
	}

	public UISprite target;

	public string normalSprite;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	public float Width;

	public float Height;

	public Transform[] ScaleTarget;

	public Vector3 ScaleStart = Vector3.one;

	public Vector3 ScaleHover = new Vector3(1f, 1f, 1f);

	public Vector3 ScalePressed = new Vector3(1f, 1f, 1f);

	public float ScaleDuration = 0.2f;

	private bool mHighlighted;

	public Transform[] OffsetTarget;

	public Vector3 OffsetStart;

	public Vector3 OffseteHover;

	public Vector3 OffsetPressed;

	public float OffsetDuration = 0.2f;
}
