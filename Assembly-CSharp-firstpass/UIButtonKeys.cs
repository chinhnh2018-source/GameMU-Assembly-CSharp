using System;
using UnityEngine;

[RequireComponent(typeof(Collider)), AddComponentMenu("NGUI/Interaction/Button Keys")]
public class UIButtonKeys : MonoBehaviour
{
	private void Start()
	{
		if (this.startsSelected && (UICamera.selectedObject == null || !NGUITools.GetActive(UICamera.selectedObject)))
		{
			UICamera.selectedObject = base.gameObject;
		}
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			switch (key)
			{
			case 273:
				if (this.selectOnUp != null)
				{
					UICamera.selectedObject = this.selectOnUp.gameObject;
				}
				break;
			case 274:
				if (this.selectOnDown != null)
				{
					UICamera.selectedObject = this.selectOnDown.gameObject;
				}
				break;
			case 275:
				if (this.selectOnRight != null)
				{
					UICamera.selectedObject = this.selectOnRight.gameObject;
				}
				break;
			case 276:
				if (this.selectOnLeft != null)
				{
					UICamera.selectedObject = this.selectOnLeft.gameObject;
				}
				break;
			default:
				if (key == 9)
				{
					if (Input.GetKey(304) || Input.GetKey(303))
					{
						if (this.selectOnLeft != null)
						{
							UICamera.selectedObject = this.selectOnLeft.gameObject;
						}
						else if (this.selectOnUp != null)
						{
							UICamera.selectedObject = this.selectOnUp.gameObject;
						}
						else if (this.selectOnDown != null)
						{
							UICamera.selectedObject = this.selectOnDown.gameObject;
						}
						else if (this.selectOnRight != null)
						{
							UICamera.selectedObject = this.selectOnRight.gameObject;
						}
					}
					else if (this.selectOnRight != null)
					{
						UICamera.selectedObject = this.selectOnRight.gameObject;
					}
					else if (this.selectOnDown != null)
					{
						UICamera.selectedObject = this.selectOnDown.gameObject;
					}
					else if (this.selectOnUp != null)
					{
						UICamera.selectedObject = this.selectOnUp.gameObject;
					}
					else if (this.selectOnLeft != null)
					{
						UICamera.selectedObject = this.selectOnLeft.gameObject;
					}
				}
				break;
			}
		}
	}

	private void OnClick()
	{
		if (base.enabled && this.selectOnClick != null)
		{
			UICamera.selectedObject = this.selectOnClick.gameObject;
		}
	}

	public bool startsSelected;

	public UIButtonKeys selectOnClick;

	public UIButtonKeys selectOnUp;

	public UIButtonKeys selectOnDown;

	public UIButtonKeys selectOnLeft;

	public UIButtonKeys selectOnRight;
}
