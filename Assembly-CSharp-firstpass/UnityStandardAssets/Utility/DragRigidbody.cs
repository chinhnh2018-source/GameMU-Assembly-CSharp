using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class DragRigidbody : MonoBehaviour
	{
		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Camera camera = this.FindCamera();
			RaycastHit raycastHit = default(RaycastHit);
			if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition).origin, camera.ScreenPointToRay(Input.mousePosition).direction, ref raycastHit, 100f, -5))
			{
				return;
			}
			if (!raycastHit.rigidbody || raycastHit.rigidbody.isKinematic)
			{
				return;
			}
			if (!this.m_SpringJoint)
			{
				GameObject gameObject = new GameObject("Rigidbody dragger");
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				this.m_SpringJoint = gameObject.AddComponent<SpringJoint>();
				rigidbody.isKinematic = true;
			}
			this.m_SpringJoint.transform.position = raycastHit.point;
			this.m_SpringJoint.anchor = Vector3.zero;
			this.m_SpringJoint.spring = 50f;
			this.m_SpringJoint.damper = 5f;
			this.m_SpringJoint.maxDistance = 0.2f;
			this.m_SpringJoint.connectedBody = raycastHit.rigidbody;
			base.StartCoroutine("DragObject", raycastHit.distance);
		}

		private IEnumerator DragObject(float distance)
		{
			float oldDrag = this.m_SpringJoint.connectedBody.drag;
			float oldAngularDrag = this.m_SpringJoint.connectedBody.angularDrag;
			this.m_SpringJoint.connectedBody.drag = 10f;
			this.m_SpringJoint.connectedBody.angularDrag = 5f;
			Camera mainCamera = this.FindCamera();
			while (Input.GetMouseButton(0))
			{
				Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				this.m_SpringJoint.transform.position = ray.GetPoint(distance);
				yield return null;
			}
			if (this.m_SpringJoint.connectedBody)
			{
				this.m_SpringJoint.connectedBody.drag = oldDrag;
				this.m_SpringJoint.connectedBody.angularDrag = oldAngularDrag;
				this.m_SpringJoint.connectedBody = null;
			}
			yield break;
		}

		private Camera FindCamera()
		{
			if (base.GetComponent<Camera>())
			{
				return base.GetComponent<Camera>();
			}
			return Camera.main;
		}

		private const float k_Spring = 50f;

		private const float k_Damper = 5f;

		private const float k_Drag = 10f;

		private const float k_AngularDrag = 5f;

		private const float k_Distance = 0.2f;

		private const bool k_AttachToCenterOfMass = false;

		private SpringJoint m_SpringJoint;
	}
}
