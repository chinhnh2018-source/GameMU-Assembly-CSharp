using System;
using UnityEngine;

namespace Xft
{
	public class CameraShakeEvent : XftEvent
	{
		public CameraShakeEvent(XftEventComponent owner) : base(XEventType.CameraShake, owner)
		{
		}

		public override void Initialize()
		{
			this.ToggleCameraShakeComponent(false);
		}

		public override void Reset()
		{
			base.Reset();
		}

		public override void OnBegin()
		{
			base.OnBegin();
			if (this.m_cameraShake.enabled && this.m_cameraShake.Client != null)
			{
				Debug.Log("Camera Shake Event is in use! please wait a moment");
				return;
			}
			this.m_cameraShake.Reset(this.m_owner);
			this.m_cameraShake.PositionSpring.AddForce(this.m_owner.PositionForce);
			this.m_cameraShake.RotationSpring.AddForce(this.m_owner.RotationForce);
			this.m_cameraShake.enabled = true;
			this.m_cameraShake.EarthQuakeToggled = true;
		}

		protected void ToggleCameraShakeComponent(bool flag)
		{
			this.m_cameraShake = base.MyCamera.gameObject.GetComponent<XftCameraShakeComp>();
			if (this.m_cameraShake == null)
			{
				this.m_cameraShake = base.MyCamera.gameObject.AddComponent<XftCameraShakeComp>();
			}
			if (this.m_cameraShake.PositionSpring == null || this.m_cameraShake.RotationSpring == null)
			{
				this.m_cameraShake.Init();
			}
			this.m_cameraShake.enabled = flag;
		}

		protected XftCameraShakeComp m_cameraShake;
	}
}
