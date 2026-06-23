using System;
using UnityEngine;

namespace Xft
{
	public class Emitter
	{
		public Emitter(EffectLayer owner)
		{
			this.Layer = owner;
			this.EmitLoop = (float)this.Layer.EmitLoop;
			this.EmitDist = this.Layer.DiffDistance;
			this.LastClientPos = this.Layer.ClientTransform.position;
		}

		public void Reset()
		{
			this.EmitterElapsedTime = 0f;
			this.EmitDelayTime = 0f;
			this.IsFirstEmit = true;
			this.EmitLoop = (float)this.Layer.EmitLoop;
			this.EmitDist = this.Layer.DiffDistance;
			this.m_emitCount = 0;
			this.CurveEmitDone = false;
			this.m_curveCountTime = 0f;
		}

		public void StopEmit()
		{
			this.EmitLoop = 0f;
			this.EmitterElapsedTime = 999999f;
			this.EmitDist = 9999999f;
		}

		protected int EmitByCurve(float deltaTime)
		{
			AnimationCurve emitterCurve = this.Layer.EmitterCurve;
			if (emitterCurve == null)
			{
				Debug.LogWarning("emitter hasn't set a curve yet!");
				return 0;
			}
			this.EmitterElapsedTime += deltaTime;
			int num = (int)emitterCurve.Evaluate(this.EmitterElapsedTime) - this.m_emitCount;
			int num2;
			if (num > this.Layer.AvailableNodeCount)
			{
				num2 = this.Layer.AvailableNodeCount;
			}
			else
			{
				num2 = num;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			this.m_emitCount += num2;
			if (num2 == 0)
			{
				this.m_curveCountTime += deltaTime;
				if (this.m_curveCountTime > 1f)
				{
					this.CurveEmitDone = true;
				}
			}
			else
			{
				this.m_curveCountTime = 0f;
			}
			if (this.CurveEmitDone)
			{
				return 0;
			}
			return num2;
		}

		protected int EmitByDistance()
		{
			if (this.Layer.mStopped)
			{
				return 0;
			}
			if ((this.Layer.ClientTransform.position - this.LastClientPos).magnitude >= this.EmitDist)
			{
				this.LastClientPos = this.Layer.ClientTransform.position;
				return 1;
			}
			return 0;
		}

		protected int EmitByBurst(float deltaTime)
		{
			if (!this.IsFirstEmit)
			{
				this.EmitLoop = 0f;
				return 0;
			}
			this.IsFirstEmit = false;
			int num = (int)this.Layer.EmitRate;
			int num2;
			if (num > this.Layer.AvailableNodeCount)
			{
				num2 = this.Layer.AvailableNodeCount;
			}
			else
			{
				num2 = num;
			}
			if (num2 <= 0)
			{
				return 0;
			}
			return num2;
		}

		protected int EmitByRate(float deltaTime)
		{
			if (this.Layer.IsBurstEmit)
			{
				return this.EmitByBurst(deltaTime);
			}
			this.EmitDelayTime += deltaTime;
			if (this.EmitDelayTime < this.Layer.EmitDelay && !this.IsFirstEmit)
			{
				return 0;
			}
			this.EmitterElapsedTime += deltaTime;
			if (this.EmitterElapsedTime >= this.Layer.EmitDuration)
			{
				if (this.EmitLoop > 0f)
				{
					this.EmitLoop -= 1f;
				}
				this.m_emitCount = 0;
				this.EmitterElapsedTime = 0f;
				this.EmitDelayTime = 0f;
				this.IsFirstEmit = false;
			}
			if (this.EmitLoop == 0f)
			{
				return 0;
			}
			if (this.Layer.AvailableNodeCount == 0)
			{
				return 0;
			}
			int num = (int)(this.EmitterElapsedTime * this.Layer.EmitRate) - this.m_emitCount;
			int num2;
			if (num > this.Layer.AvailableNodeCount)
			{
				num2 = this.Layer.AvailableNodeCount;
			}
			else
			{
				num2 = num;
			}
			if (num2 <= 0)
			{
				return 0;
			}
			this.m_emitCount += num2;
			return num2;
		}

		public Vector3 GetEmitRotation(EffectNode node)
		{
			Vector3 vector = Vector3.zero;
			if (this.Layer.DirType == DIRECTION_TYPE.Sphere)
			{
				vector = node.GetOriginalPos() - this.Layer.DirCenter.position;
				if (vector == Vector3.zero)
				{
					Vector3 up = Vector3.up;
					Quaternion quaternion = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
					vector = quaternion * up;
				}
			}
			else if (this.Layer.DirType == DIRECTION_TYPE.Planar)
			{
				vector = this.Layer.OriVelocityAxis;
			}
			else if (this.Layer.DirType == DIRECTION_TYPE.Cone)
			{
				if (this.Layer.EmitType == 3 && this.Layer.EmitUniform)
				{
					Vector3 vector2;
					if (!this.Layer.SyncClient)
					{
						vector2 = node.Position - (node.GetRealClientPos() + this.Layer.EmitPoint);
					}
					else
					{
						vector2 = node.Position - this.Layer.EmitPoint;
					}
					int num = this.Layer.AngleAroundAxis;
					if (this.Layer.UseRandomDirAngle)
					{
						num = Random.Range(this.Layer.AngleAroundAxis, this.Layer.AngleAroundAxisMax);
					}
					Vector3 vector3 = Vector3.RotateTowards(vector2, this.Layer.CircleDir, (float)(90 - num) * 0.0174532924f, 1f);
					Quaternion quaternion2 = Quaternion.FromToRotation(vector2, vector3);
					vector = quaternion2 * vector2;
				}
				else
				{
					Quaternion quaternion3 = Quaternion.Euler(0f, 0f, (float)this.Layer.AngleAroundAxis);
					Quaternion quaternion4 = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
					Quaternion quaternion5 = Quaternion.FromToRotation(Vector3.up, this.Layer.OriVelocityAxis);
					vector = quaternion5 * quaternion4 * quaternion3 * Vector3.up;
				}
			}
			else if (this.Layer.DirType == DIRECTION_TYPE.Cylindrical)
			{
				Vector3 vector4 = node.GetOriginalPos() - this.Layer.DirCenter.position;
				if (vector4 == Vector3.zero)
				{
					Vector3 up2 = Vector3.up;
					Quaternion quaternion6 = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
					vector4 = quaternion6 * up2;
				}
				float num2 = Vector3.Dot(this.Layer.OriVelocityAxis, vector4);
				vector = vector4 - num2 * this.Layer.OriVelocityAxis.normalized;
			}
			return vector;
		}

		public void SetEmitPosition(EffectNode node)
		{
			Vector3 vector = Vector3.zero;
			Vector3 realClientPos = node.GetRealClientPos();
			if (this.Layer.EmitType == 1)
			{
				Vector3 emitPoint = this.Layer.EmitPoint;
				float x = Random.Range(emitPoint.x - this.Layer.BoxSize.x / 2f, emitPoint.x + this.Layer.BoxSize.x / 2f);
				float y = Random.Range(emitPoint.y - this.Layer.BoxSize.y / 2f, emitPoint.y + this.Layer.BoxSize.y / 2f);
				float z = Random.Range(emitPoint.z - this.Layer.BoxSize.z / 2f, emitPoint.z + this.Layer.BoxSize.z / 2f);
				vector.x = x;
				vector.y = y;
				vector.z = z;
				if (!this.Layer.SyncClient)
				{
					vector = this.Layer.ClientTransform.rotation * vector + realClientPos;
				}
				else
				{
					vector = this.Layer.ClientTransform.rotation * vector;
				}
			}
			else if (this.Layer.EmitType == 0)
			{
				vector = this.Layer.EmitPoint;
				if (!this.Layer.SyncClient)
				{
					vector = realClientPos + this.Layer.EmitPoint;
				}
			}
			else if (this.Layer.EmitType == 2)
			{
				vector = this.Layer.EmitPoint;
				if (!this.Layer.SyncClient)
				{
					vector = realClientPos + this.Layer.EmitPoint;
				}
				Vector3 vector2 = Vector3.up * this.Layer.Radius;
				Quaternion quaternion = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
				vector = quaternion * vector2 + vector;
			}
			else if (this.Layer.EmitType == 4)
			{
				Vector3 vector3 = this.Layer.EmitPoint + this.Layer.ClientTransform.forward * this.Layer.LineLengthLeft;
				Vector3 vector4 = this.Layer.EmitPoint + this.Layer.ClientTransform.forward * this.Layer.LineLengthRight;
				Vector3 vector5 = vector4 - vector3;
				float num2;
				if (this.Layer.EmitUniform)
				{
					float num = (float)(node.Index + 1) / (float)this.Layer.MaxENodes;
					num2 = vector5.magnitude * num;
				}
				else
				{
					num2 = Random.Range(0f, vector5.magnitude);
				}
				vector = vector3 + vector5.normalized * num2;
				if (!this.Layer.SyncClient)
				{
					vector = realClientPos + vector;
				}
			}
			else if (this.Layer.EmitType == 3)
			{
				float num4;
				if (this.Layer.EmitUniform)
				{
					float num3 = (float)(node.Index + 1) / (float)this.Layer.MaxENodes;
					num4 = 360f * num3;
				}
				else
				{
					num4 = (float)Random.Range(0, 360);
				}
				float num5 = this.Layer.Radius;
				if (this.Layer.UseRandomCircle)
				{
					num5 = Random.Range(this.Layer.CircleRadiusMin, this.Layer.CircleRadiusMax);
				}
				Quaternion quaternion2 = Quaternion.Euler(0f, num4, 0f);
				Vector3 vector6 = quaternion2 * (Vector3.right * num5);
				Quaternion quaternion3 = Quaternion.FromToRotation(Vector3.up, this.Layer.ClientTransform.rotation * this.Layer.CircleDir);
				vector = quaternion3 * vector6;
				if (!this.Layer.SyncClient)
				{
					vector = realClientPos + vector + this.Layer.EmitPoint;
				}
				else
				{
					vector += this.Layer.EmitPoint;
				}
			}
			else if (this.Layer.EmitType == 5)
			{
				if (this.Layer.EmitMesh == null)
				{
					Debug.LogWarning("please set a mesh to the emitter.");
					return;
				}
				if (this.Layer.EmitMeshType == 0)
				{
					int vertexCount = this.Layer.EmitMesh.vertexCount;
					int num6;
					if (this.Layer.EmitUniform)
					{
						num6 = node.Index % (vertexCount - 1);
					}
					else
					{
						num6 = Random.Range(0, vertexCount - 1);
					}
					vector = this.Layer.EmitMesh.vertices[num6];
					if (!this.Layer.SyncClient)
					{
						vector = realClientPos + vector + this.Layer.EmitPoint;
					}
					else
					{
						vector += this.Layer.EmitPoint;
					}
				}
				else if (this.Layer.EmitMeshType == 1)
				{
					Vector3[] vertices = this.Layer.EmitMesh.vertices;
					int num7 = this.Layer.EmitMesh.triangles.Length / 3;
					int num6;
					if (this.Layer.EmitUniform)
					{
						num6 = node.Index % (num7 - 1);
					}
					else
					{
						num6 = Random.Range(0, num7 - 1);
					}
					int num8 = this.Layer.EmitMesh.triangles[num6 * 3];
					int num9 = this.Layer.EmitMesh.triangles[num6 * 3 + 1];
					int num10 = this.Layer.EmitMesh.triangles[num6 * 3 + 2];
					vector = (vertices[num8] + vertices[num9] + vertices[num10]) / 3f;
					if (!this.Layer.SyncClient)
					{
						vector = realClientPos + vector + this.Layer.EmitPoint;
					}
					else
					{
						vector += this.Layer.EmitPoint;
					}
				}
			}
			node.SetLocalPosition(vector);
		}

		public int GetNodes(float deltaTime)
		{
			if (this.Layer.EmitWay == EEmitWay.ByRate)
			{
				return this.EmitByRate(deltaTime);
			}
			if (this.Layer.EmitWay == EEmitWay.ByCurve)
			{
				return this.EmitByCurve(deltaTime);
			}
			return this.EmitByDistance();
		}

		public EffectLayer Layer;

		public float EmitterElapsedTime;

		private float EmitDelayTime;

		private bool IsFirstEmit = true;

		public float EmitLoop;

		public float EmitDist = 1f;

		public bool CurveEmitDone;

		private Vector3 LastClientPos = Vector3.zero;

		protected int m_emitCount;

		protected float m_curveCountTime;
	}
}
