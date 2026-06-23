using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class TiltInput : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.mapping.type == TiltInput.AxisMapping.MappingType.NamedAxis)
			{
				this.m_SteerAxis = new CrossPlatformInputManager.VirtualAxis(this.mapping.axisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_SteerAxis);
			}
		}

		private void Update()
		{
			float num = 0f;
			if (Input.acceleration != Vector3.zero)
			{
				TiltInput.AxisOptions axisOptions = this.tiltAroundAxis;
				if (axisOptions != TiltInput.AxisOptions.ForwardAxis)
				{
					if (axisOptions == TiltInput.AxisOptions.SidewaysAxis)
					{
						num = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y) * 57.29578f + this.centreAngleOffset;
					}
				}
				else
				{
					num = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y) * 57.29578f + this.centreAngleOffset;
				}
			}
			float num2 = Mathf.InverseLerp(-this.fullTiltAngle, this.fullTiltAngle, num) * 2f - 1f;
			switch (this.mapping.type)
			{
			case TiltInput.AxisMapping.MappingType.NamedAxis:
				this.m_SteerAxis.Update(num2);
				break;
			case TiltInput.AxisMapping.MappingType.MousePositionX:
				CrossPlatformInputManager.SetVirtualMousePositionX(num2 * (float)Screen.width);
				break;
			case TiltInput.AxisMapping.MappingType.MousePositionY:
				CrossPlatformInputManager.SetVirtualMousePositionY(num2 * (float)Screen.width);
				break;
			case TiltInput.AxisMapping.MappingType.MousePositionZ:
				CrossPlatformInputManager.SetVirtualMousePositionZ(num2 * (float)Screen.width);
				break;
			}
		}

		private void OnDisable()
		{
			this.m_SteerAxis.Remove();
		}

		public TiltInput.AxisMapping mapping;

		public TiltInput.AxisOptions tiltAroundAxis;

		public float fullTiltAngle = 25f;

		public float centreAngleOffset;

		private CrossPlatformInputManager.VirtualAxis m_SteerAxis;

		public enum AxisOptions
		{
			ForwardAxis,
			SidewaysAxis
		}

		[Serializable]
		public class AxisMapping
		{
			public TiltInput.AxisMapping.MappingType type;

			public string axisName;

			public enum MappingType
			{
				NamedAxis,
				MousePositionX,
				MousePositionY,
				MousePositionZ
			}
		}
	}
}
