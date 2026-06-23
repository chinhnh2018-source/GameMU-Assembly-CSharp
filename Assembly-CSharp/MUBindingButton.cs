using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MUBindingButton : MonoBehaviour
{
	private void Awake()
	{
		this.m_orgScale = base.transform.localScale;
	}

	public void OnButtonSelect()
	{
		base.transform.localScale = new Vector3(this.m_orgScale.x * 1.2f, this.m_orgScale.y * 1.2f, this.m_orgScale.z);
	}

	public void OnButtonUnSelect()
	{
		base.transform.localScale = this.m_orgScale;
	}

	public void ExecuteClick()
	{
		if (base.gameObject.activeInHierarchy)
		{
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			if (component != null && component.enabled)
			{
				UICamera.hoveredObject = base.gameObject;
				UICamera.selectedObject = base.gameObject;
				UICamera.Notify(base.gameObject, "OnPress", true);
				UICamera.Notify(base.gameObject, "OnClick", null);
				UICamera.Notify(base.gameObject, "OnPress", false);
				UICamera.hoveredObject = null;
				UICamera.selectedObject = null;
			}
		}
	}

	public int tabIndex;

	private Vector3 m_orgScale;
}
