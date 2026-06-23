using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		private void OnEnable()
		{
			this.CheckEnableControlRig();
		}

		private void CheckEnableControlRig()
		{
			this.EnableControlRig(false);
		}

		private void EnableControlRig(bool enabled)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.SetActive(enabled);
			}
		}
	}
}
