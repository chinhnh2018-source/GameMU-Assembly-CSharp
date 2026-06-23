using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{
		private void Start()
		{
		}

		private void LateUpdate()
		{
			if (!this.target)
			{
				return;
			}
			float y = this.target.eulerAngles.y;
			float num = this.target.position.y + this.height;
			float num2 = base.transform.eulerAngles.y;
			float num3 = base.transform.position.y;
			num2 = Mathf.LerpAngle(num2, y, this.rotationDamping * Time.deltaTime);
			num3 = Mathf.Lerp(num3, num, this.heightDamping * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, num2, 0f);
			base.transform.position = this.target.position;
			base.transform.position -= quaternion * Vector3.forward * this.distance;
			base.transform.position = new Vector3(base.transform.position.x, num3, base.transform.position.z);
			base.transform.LookAt(this.target);
		}

		[SerializeField]
		private Transform target;

		[SerializeField]
		private float distance = 10f;

		[SerializeField]
		private float height = 5f;

		[SerializeField]
		private float rotationDamping;

		[SerializeField]
		private float heightDamping;
	}
}
