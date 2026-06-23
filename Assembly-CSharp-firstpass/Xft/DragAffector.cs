using System;
using UnityEngine;

namespace Xft
{
	public class DragAffector : Affector
	{
		public DragAffector(Transform dragObj, bool useDir, Vector3 dir, float mag, bool useMaxDist, float maxDist, float atten, EffectNode node) : base(node, AFFECTORTYPE.DragAffector)
		{
			this.DragObj = dragObj;
			this.UseDirection = useDir;
			this.Direction = dir;
			this.Magnitude = mag;
			this.UseMaxDistance = useMaxDist;
			this.MaxDistance = maxDist;
			this.Attenuation = atten;
		}

		protected void UpdateNoAttenuationNoDir(float deltaTime)
		{
			float sqrMagnitude = (this.Node.GetOriginalPos() - this.DragObj.position).sqrMagnitude;
			float num = this.Magnitude * deltaTime;
			if (num != 0f && sqrMagnitude <= this.MaxDistance * this.MaxDistance)
			{
				if (num < 1f)
				{
					this.Node.Velocity *= 1f - num;
				}
				else
				{
					this.Node.Velocity = Vector3.zero;
				}
			}
		}

		protected void UpdateNoAttenuationNoDirNoDist(float deltaTime)
		{
			float num = this.Magnitude * deltaTime;
			if (num < 1f)
			{
				this.Node.Velocity *= 1f - num;
			}
			else
			{
				this.Node.Velocity = Vector3.zero;
			}
		}

		public override void Update(float deltaTime)
		{
			if (!this.UseDirection && this.Attenuation == 0f)
			{
				if (this.UseMaxDistance)
				{
					this.UpdateNoAttenuationNoDir(deltaTime);
				}
				else
				{
					this.UpdateNoAttenuationNoDirNoDist(deltaTime);
				}
				return;
			}
			Vector3 vector = Vector3.one;
			if (this.UseDirection && this.Direction != Vector3.zero)
			{
				vector = this.DragObj.rotation * this.Direction;
				vector.Normalize();
			}
			float magnitude = (this.Node.GetOriginalPos() - this.DragObj.position).magnitude;
			if (!this.UseMaxDistance || magnitude <= this.MaxDistance)
			{
				float num = 1f;
				if (this.UseDirection)
				{
					Vector3 velocity = this.Node.Velocity;
					velocity.Normalize();
					num = Vector3.Dot(velocity, vector);
				}
				float num2 = this.Magnitude * deltaTime * num / (1f + magnitude * this.Attenuation);
				if (num2 < 1f)
				{
					this.Node.Velocity -= num2 * this.Node.Velocity;
				}
				else
				{
					this.Node.Velocity = Vector3.zero;
				}
			}
		}

		protected Transform DragObj;

		protected bool UseDirection;

		protected Vector3 Direction;

		protected float Magnitude;

		protected bool UseMaxDistance;

		protected float MaxDistance;

		protected float Attenuation;
	}
}
