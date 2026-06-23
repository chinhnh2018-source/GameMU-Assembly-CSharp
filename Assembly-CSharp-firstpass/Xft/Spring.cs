using System;
using UnityEngine;

namespace Xft
{
	public class Spring
	{
		public Spring(Transform transform, Spring.TransformType modifier)
		{
			this.m_Transform = transform;
			this.Modifier = modifier;
			this.RefreshTransformType();
		}

		public Transform Transform
		{
			set
			{
				this.m_Transform = value;
				this.RefreshTransformType();
			}
		}

		public bool Done
		{
			get
			{
				return this.m_done;
			}
			set
			{
				this.m_done = value;
			}
		}

		public void FixedUpdate()
		{
			if (this.m_VelocityFadeInEndTime > Time.time)
			{
				this.m_VelocityFadeInCap = Mathf.Clamp01(1f - (this.m_VelocityFadeInEndTime - Time.time) / this.m_VelocityFadeInLength);
			}
			else
			{
				this.m_VelocityFadeInCap = 1f;
			}
			if (this.Modifier != this.m_CurrentTransformType)
			{
				this.RefreshTransformType();
			}
			this.m_TransformFunc();
		}

		private void Position()
		{
			this.Calculate();
			this.m_Transform.localPosition = this.State;
		}

		private void PositionAdditive()
		{
			this.Calculate();
			this.m_Transform.localPosition += this.State;
		}

		private void Rotation()
		{
			this.Calculate();
			this.m_Transform.localEulerAngles = this.State;
		}

		private void RotationAdditive()
		{
			this.Calculate();
			this.m_Transform.localEulerAngles += this.State;
		}

		private void Scale()
		{
			this.Calculate();
			this.m_Transform.localScale = this.State;
		}

		private void ScaleAdditive()
		{
			this.Calculate();
			this.m_Transform.localScale += this.State;
		}

		public void RefreshTransformType()
		{
			switch (this.Modifier)
			{
			case Spring.TransformType.Position:
				this.State = this.m_Transform.localPosition;
				this.m_TransformFunc = new Spring.TransformDelegate(this.Position);
				break;
			case Spring.TransformType.PositionAdditive:
				this.State = this.m_Transform.localPosition;
				this.m_TransformFunc = new Spring.TransformDelegate(this.PositionAdditive);
				break;
			case Spring.TransformType.Rotation:
				this.State = this.m_Transform.localEulerAngles;
				this.m_TransformFunc = new Spring.TransformDelegate(this.Rotation);
				break;
			case Spring.TransformType.RotationAdditive:
				this.State = this.m_Transform.localEulerAngles;
				this.m_TransformFunc = new Spring.TransformDelegate(this.RotationAdditive);
				break;
			case Spring.TransformType.Scale:
				this.State = this.m_Transform.localScale;
				this.m_TransformFunc = new Spring.TransformDelegate(this.Scale);
				break;
			case Spring.TransformType.ScaleAdditive:
				this.State = this.m_Transform.localScale;
				this.m_TransformFunc = new Spring.TransformDelegate(this.ScaleAdditive);
				break;
			}
			this.m_CurrentTransformType = this.Modifier;
			this.RestState = this.State;
		}

		protected void Calculate()
		{
			if (this.State == this.RestState)
			{
				this.m_done = true;
				return;
			}
			Vector3 vector = this.RestState - this.State;
			this.m_Velocity += Vector3.Scale(vector, this.Stiffness);
			this.m_Velocity = Vector3.Scale(this.m_Velocity, this.Damping);
			this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
			if (Mathf.Abs(this.m_Velocity.sqrMagnitude) > this.MinVelocity * this.MinVelocity)
			{
				this.Move();
			}
			else
			{
				this.Reset();
			}
		}

		public void AddForce(Vector3 force)
		{
			force *= this.m_VelocityFadeInCap;
			this.m_Velocity += force;
			this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
			this.Move();
			this.m_done = false;
		}

		public void AddForce(float x, float y, float z)
		{
			this.AddForce(new Vector3(x, y, z));
		}

		protected void Move()
		{
			this.State += this.m_Velocity;
			this.State = new Vector3(Mathf.Clamp(this.State.x, this.MinState.x, this.MaxState.x), Mathf.Clamp(this.State.y, this.MinState.y, this.MaxState.y), Mathf.Clamp(this.State.z, this.MinState.z, this.MaxState.z));
		}

		public void Reset()
		{
			this.m_Velocity = Vector3.zero;
			this.State = this.RestState;
			this.m_done = true;
		}

		public void Stop()
		{
			this.m_Velocity = Vector3.zero;
		}

		public void ForceVelocityFadeIn(float seconds)
		{
			this.m_VelocityFadeInLength = seconds;
			this.m_VelocityFadeInEndTime = Time.time + seconds;
			this.m_VelocityFadeInCap = 0f;
		}

		public Spring.TransformType Modifier;

		protected Spring.TransformDelegate m_TransformFunc;

		public Vector3 State = Vector3.zero;

		protected Spring.TransformType m_CurrentTransformType;

		protected Vector3 m_Velocity = Vector3.zero;

		public Vector3 RestState = Vector3.zero;

		public Vector3 Stiffness = new Vector3(0.5f, 0.5f, 0.5f);

		public Vector3 Damping = new Vector3(0.75f, 0.75f, 0.75f);

		protected float m_VelocityFadeInCap = 1f;

		protected float m_VelocityFadeInEndTime;

		protected float m_VelocityFadeInLength;

		public float MaxVelocity = 10000f;

		public float MinVelocity = 1E-07f;

		public Vector3 MaxState = new Vector3(10000f, 10000f, 10000f);

		public Vector3 MinState = new Vector3(-10000f, -10000f, -10000f);

		protected Transform m_Transform;

		protected bool m_done;

		public enum TransformType
		{
			Position,
			PositionAdditive,
			Rotation,
			RotationAdditive,
			Scale,
			ScaleAdditive
		}

		protected delegate void TransformDelegate();
	}
}
