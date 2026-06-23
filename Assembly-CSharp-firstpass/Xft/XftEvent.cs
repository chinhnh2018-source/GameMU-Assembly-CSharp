using System;
using UnityEngine;

namespace Xft
{
	public class XftEvent
	{
		public XftEvent(XEventType type, XftEventComponent owner)
		{
			this.m_type = type;
			this.m_owner = owner;
		}

		public Camera MyCamera
		{
			get
			{
				if (this.m_myCamera == null)
				{
					this.m_myCamera = this.FindMyCamera();
				}
				return this.m_myCamera;
			}
		}

		public void ResetMyCamera()
		{
			if (this.m_myCamera == null)
			{
				this.m_myCamera = this.FindMyCamera();
			}
		}

		protected Camera FindMyCamera()
		{
			int num = 1 << this.m_owner.gameObject.layer;
			Camera[] array = Object.FindSceneObjectsOfType(typeof(Camera)) as Camera[];
			int i = 0;
			int num2 = array.Length;
			while (i < num2)
			{
				Camera camera = array[i];
				if ((camera.cullingMask & num) != 0)
				{
					return camera;
				}
				i++;
			}
			Debug.LogError("can't find proper camera for event:" + this.m_type);
			return null;
		}

		public bool CanUpdate
		{
			get
			{
				return this.m_canUpdate;
			}
			set
			{
				this.m_canUpdate = value;
			}
		}

		public virtual void OnBegin()
		{
			this.CanUpdate = true;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Update(float deltaTime)
		{
		}

		public virtual void Reset()
		{
			this.m_elapsedTime = 0f;
			this.CanUpdate = false;
		}

		protected XEventType m_type;

		protected XftEventComponent m_owner;

		protected float m_elapsedTime;

		protected bool m_canUpdate;

		protected Camera m_myCamera;
	}
}
