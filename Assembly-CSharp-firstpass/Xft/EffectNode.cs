using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xft
{
	public class EffectNode
	{
		public EffectNode(int index, Transform clienttrans, bool sync, EffectLayer owner)
		{
			this.Index = index;
			this.ClientTrans = clienttrans;
			this.SyncClient = sync;
			this.Owner = owner;
			this.LowerLeftUV = Vector2.zero;
			this.UVDimensions = Vector2.one;
			this.Scale = Vector2.one;
			this.RotateAngle = 0f;
			this.Color = Color.white;
		}

		public void SetAffectorList(List<Affector> afts)
		{
			this.AffectorList = afts;
		}

		public List<Affector> GetAffectorList()
		{
			return this.AffectorList;
		}

		public void Init(Vector3 oriDir, float speed, float life, int oriRot, float oriScaleX, float oriScaleY, Color oriColor, Vector2 oriLowerUv, Vector2 oriUVDimension)
		{
			this.OriDirection = oriDir;
			this.LifeTime = life;
			this.OriRotateAngle = oriRot;
			this.OriScaleX = oriScaleX;
			this.OriScaleY = oriScaleY;
			this.Color = oriColor;
			this.ElapsedTime = 0f;
			if (this.Owner.DirType != DIRECTION_TYPE.Sphere)
			{
				this.Velocity = this.Owner.transform.rotation * this.OriDirection * speed;
			}
			else
			{
				this.Velocity = this.OriDirection * speed;
			}
			this.LowerLeftUV = oriLowerUv;
			this.UVDimensions = oriUVDimension;
			this.IsCollisionEventSended = false;
			if (this.Type == 1)
			{
				this.Sprite.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
				this.Sprite.SetColor(oriColor);
				if (this.SimpleSprite)
				{
					this.Update(0f);
					this.Sprite.Transform();
				}
			}
			else if (this.Type == 2)
			{
				this.Ribbon.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
				this.Ribbon.SetColor(oriColor);
				this.Ribbon.SetHeadPosition(this.GetRealClientPos() + this.Position + this.OriDirection.normalized * this.Owner.TailDistance);
				this.Ribbon.ResetElementsPos();
			}
			else if (this.Type == 3)
			{
				this.Cone.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
				this.Cone.SetColor(oriColor);
				this.Cone.SetRotation((float)oriRot);
			}
			else if (this.Type == 4)
			{
				this.CusMesh.SetColor(oriColor);
				this.CusMesh.SetRotation((float)oriRot);
				this.CusMesh.SetScale(oriScaleX, oriScaleY);
				this.CusMesh.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
			}
			if (this.Type == 1)
			{
				if (this.Owner.DirType != DIRECTION_TYPE.Sphere)
				{
					this.Sprite.SetRotationTo(this.Owner.ClientTransform.rotation * this.OriDirection);
				}
				else
				{
					this.Sprite.SetRotationTo(this.OriDirection);
				}
			}
		}

		public void ResetCamera(Camera cam)
		{
			if (this.Type == 1)
			{
				this.Sprite.MainCamera = cam;
			}
			else if (this.Type == 2)
			{
				this.Ribbon.MainCamera = cam;
			}
		}

		public float GetElapsedTime()
		{
			return this.ElapsedTime;
		}

		public float GetLifeTime()
		{
			return this.LifeTime;
		}

		public void SetLocalPosition(Vector3 pos)
		{
			if (this.Type == 2)
			{
				if (!this.SyncClient)
				{
					this.Ribbon.OriHeadPos = pos;
				}
				else
				{
					this.Ribbon.OriHeadPos = this.GetRealClientPos() + pos;
				}
			}
			this.Position = pos;
		}

		public Vector3 GetLocalPosition()
		{
			return this.Position;
		}

		public Vector3 GetRealClientPos()
		{
			Vector3 vector = Vector3.one * this.Owner.Owner.Scale;
			Vector3 zero = Vector3.zero;
			zero.x = this.ClientTrans.position.x / vector.x;
			zero.y = this.ClientTrans.position.y / vector.y;
			zero.z = this.ClientTrans.position.z / vector.z;
			return zero;
		}

		public Vector3 GetOriginalPos()
		{
			Vector3 realClientPos = this.GetRealClientPos();
			Vector3 result;
			if (!this.SyncClient)
			{
				result = this.Position - realClientPos + this.ClientTrans.position;
			}
			else
			{
				result = this.Position + this.ClientTrans.position;
			}
			return result;
		}

		public Vector3 GetWorldPos()
		{
			return this.CurWorldPos;
		}

		protected bool IsSimpleSprite()
		{
			bool result = false;
			if (this.Owner.SpriteType == 2 && this.Owner.OriVelocityAxis == Vector3.zero && !this.Owner.ScaleAffectorEnable && !this.Owner.RotAffectorEnable && (double)this.Owner.OriSpeed < 0.0001 && !this.Owner.GravityAffectorEnable && !this.Owner.AirAffectorEnable && !this.Owner.TurbulenceAffectorEnable && !this.Owner.BombAffectorEnable && !this.Owner.UVRotAffectorEnable && (double)Mathf.Abs(this.Owner.OriRotationMax - this.Owner.OriRotationMin) < 0.0001 && (double)Mathf.Abs(this.Owner.OriScaleXMin - this.Owner.OriScaleXMax) < 0.0001 && (double)Mathf.Abs(this.Owner.OriScaleYMin - this.Owner.OriScaleYMax) < 0.0001)
			{
				result = true;
			}
			return result;
		}

		public void SetType(float width, float height, STYPE type, ORIPOINT orip, int uvStretch, float maxFps)
		{
			this.Type = 1;
			this.SimpleSprite = this.IsSimpleSprite();
			this.Sprite = this.Owner.GetVertexPool().AddSprite(width, height, type, orip, this.Owner.MyCamera, uvStretch, maxFps, this.SimpleSprite);
			this.Sprite.Owner = this;
		}

		public void SetType(bool useFaceObj, Transform faceobj, float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
		{
			this.Type = 2;
			this.Ribbon = this.Owner.GetVertexPool().AddRibbonTrail(this.Owner.MyCamera, useFaceObj, faceobj, width, maxelemnt, len, pos, stretchType, maxFps);
			this.Ribbon.Owner = this;
		}

		public void SetType(Vector2 size, int numSegment, float angle, Vector3 dir, int uvStretch, float maxFps, bool usedelta, AnimationCurve deltaAngle)
		{
			this.Type = 3;
			this.Cone = this.Owner.GetVertexPool().AddCone(size, numSegment, angle, dir, uvStretch, maxFps, usedelta, deltaAngle, this);
		}

		public void SetType(Mesh mesh, Vector3 dir, float maxFps)
		{
			this.Type = 4;
			this.CusMesh = this.Owner.GetVertexPool().AddCustomMesh(mesh, dir, maxFps, this);
		}

		public void Reset()
		{
			if (this.Owner.UseSubEmitters && !string.IsNullOrEmpty(this.Owner.DeathSubEmitter))
			{
				XffectComponent effect = this.Owner.SpawnCache.GetEffect(this.Owner.DeathSubEmitter);
				if (effect == null)
				{
					return;
				}
				effect.Active();
				effect.transform.position = this.CurWorldPos;
			}
			this.Position = this.Owner.EmitPoint + this.Owner.transform.position;
			this.Velocity = Vector3.zero;
			this.ElapsedTime = 0f;
			this.CurWorldPos = this.Owner.transform.position;
			this.LastWorldPos = this.CurWorldPos;
			this.IsCollisionEventSended = false;
			for (int i = 0; i < this.AffectorList.Count; i++)
			{
				Affector affector = this.AffectorList[i];
				affector.Reset();
			}
			this.Scale = Vector3.one;
			if (this.Type == 1)
			{
				this.Sprite.SetRotation((float)this.OriRotateAngle);
				this.Sprite.SetPosition(this.Position);
				this.Sprite.SetColor(Color.clear);
				this.Sprite.Update(true, 0f);
				this.Scale = Vector2.one;
			}
			else if (this.Type == 2)
			{
				Vector3 headPosition;
				if (this.Owner.AlwaysSyncRotation)
				{
					headPosition = this.ClientTrans.rotation * (this.GetRealClientPos() + this.Owner.EmitPoint);
				}
				else
				{
					headPosition = this.GetRealClientPos() + this.Owner.EmitPoint;
				}
				this.Ribbon.SetHeadPosition(headPosition);
				this.Ribbon.Reset();
				this.Ribbon.SetColor(Color.clear);
				this.Ribbon.UpdateVertices(Vector3.zero);
			}
			else if (this.Type == 3)
			{
				this.Cone.SetRotation((float)this.OriRotateAngle);
				this.Cone.SetColor(Color.clear);
				this.Cone.SetPosition(this.Position);
				this.Scale = Vector2.one;
				this.Cone.ResetAngle();
				this.Cone.Update(true, 0f);
			}
			else if (this.Type == 4)
			{
				this.CusMesh.SetColor(Color.clear);
				this.CusMesh.SetRotation((float)this.OriRotateAngle);
				this.CusMesh.Update(true, 0f);
			}
			if (this.Owner.UseSubEmitters && this.SubEmitter != null && XffectComponent.IsActive(this.SubEmitter.gameObject))
			{
				this.SubEmitter.StopEmit();
			}
		}

		public void Remove()
		{
			this.Owner.RemoveActiveNode(this);
		}

		public void Stop()
		{
			this.Reset();
			this.Remove();
		}

		public void UpdateSprite(float deltaTime)
		{
			if (this.Owner.AlwaysSyncRotation && this.Owner.DirType != DIRECTION_TYPE.Sphere)
			{
				this.Sprite.SetRotationTo(this.Owner.transform.rotation * this.OriDirection);
			}
			this.Sprite.SetScale(this.Scale.x * this.OriScaleX, this.Scale.y * this.OriScaleY);
			if (this.Owner.ColorAffectorEnable)
			{
				this.Sprite.SetColor(this.Color);
			}
			if (this.Owner.UVAffectorEnable || this.Owner.UVRotAffectorEnable)
			{
				this.Sprite.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
			}
			this.Sprite.SetRotation((float)this.OriRotateAngle + this.RotateAngle);
			this.Sprite.SetPosition(this.CurWorldPos);
			this.Sprite.Update(false, deltaTime);
		}

		public void UpdateRibbonTrail(float deltaTime)
		{
			this.Ribbon.SetHeadPosition(this.CurWorldPos);
			if (this.Owner.UVAffectorEnable || this.Owner.UVRotAffectorEnable)
			{
				this.Ribbon.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
			}
			this.Ribbon.SetColor(this.Color);
			this.Ribbon.Update(deltaTime);
		}

		public void UpdateCone(float deltaTime)
		{
			this.Cone.SetScale(this.Scale.x * this.OriScaleX, this.Scale.y * this.OriScaleY);
			if (this.Owner.ColorAffectorEnable)
			{
				this.Cone.SetColor(this.Color);
			}
			if (this.Owner.UVAffectorEnable || this.Owner.UVRotAffectorEnable)
			{
				this.Cone.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
			}
			this.Cone.SetRotation((float)this.OriRotateAngle + this.RotateAngle);
			this.Cone.SetPosition(this.CurWorldPos);
			this.Cone.Update(false, deltaTime);
		}

		public void UpdateCustomMesh(float deltaTime)
		{
			this.CusMesh.SetScale(this.Scale.x * this.OriScaleX, this.Scale.y * this.OriScaleY);
			if (this.Owner.ColorAffectorEnable)
			{
				this.CusMesh.SetColor(this.Color);
			}
			if (this.Owner.UVAffectorEnable || this.Owner.UVRotAffectorEnable)
			{
				this.CusMesh.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
			}
			this.CusMesh.SetRotation((float)this.OriRotateAngle + this.RotateAngle);
			this.CusMesh.SetPosition(this.CurWorldPos);
			this.CusMesh.Update(false, deltaTime);
		}

		public void CollisionDetection()
		{
			if (!this.Owner.UseCollisionDetection || this.IsCollisionEventSended)
			{
				return;
			}
			bool flag = false;
			GameObject obj = null;
			if (this.Owner.CollisionType == COLLITION_TYPE.Sphere && this.Owner.CollisionGoal != null)
			{
				Vector3 lastCollisionDetectDir = this.CurWorldPos + this.Velocity.normalized * this.Owner.CollisionOffset - this.Owner.CollisionGoal.position;
				float num = this.Owner.ColliisionPosRange + this.Owner.ParticleRadius;
				if (lastCollisionDetectDir.sqrMagnitude <= num * num)
				{
					flag = true;
					obj = this.Owner.CollisionGoal.gameObject;
				}
				this.LastCollisionDetectDir = lastCollisionDetectDir;
			}
			else if (this.Owner.CollisionType == COLLITION_TYPE.CollisionLayer)
			{
				int num2 = 1 << this.Owner.CollisionLayer;
				Vector3 vector = this.GetOriginalPos() + this.Velocity.normalized * this.Owner.CollisionOffset;
				RaycastHit raycastHit;
				if (Physics.SphereCast(vector, this.Owner.ParticleRadius, this.Velocity.normalized, ref raycastHit, this.Owner.ParticleRadius, num2))
				{
					flag = true;
					obj = raycastHit.collider.gameObject;
				}
			}
			else if (this.Owner.CollisionType == COLLITION_TYPE.Plane)
			{
				if (!this.Owner.CollisionPlane.GetSide(this.CurWorldPos - this.Owner.PlaneDir.normalized * this.Owner.ParticleRadius))
				{
					flag = true;
					obj = this.Owner.gameObject;
				}
			}
			else
			{
				Debug.LogError("invalid collision type!");
			}
			if (flag)
			{
				if (this.Owner.EventHandleFunctionName != string.Empty && this.Owner.EventReceiver != null)
				{
					this.Owner.EventReceiver.SendMessage(this.Owner.EventHandleFunctionName, new CollisionParam(obj, this.GetOriginalPos()));
				}
				this.IsCollisionEventSended = true;
				if (this.Owner.CollisionAutoDestroy)
				{
					this.ElapsedTime = float.PositiveInfinity;
				}
				if (this.Owner.UseSubEmitters && !string.IsNullOrEmpty(this.Owner.CollisionSubEmitter))
				{
					XffectComponent effect = this.Owner.SpawnCache.GetEffect(this.Owner.CollisionSubEmitter);
					if (effect == null)
					{
						return;
					}
					effect.Active();
					effect.transform.position = this.CurWorldPos;
				}
			}
		}

		public void Update(float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			for (int i = 0; i < this.AffectorList.Count; i++)
			{
				Affector affector = this.AffectorList[i];
				affector.Update(deltaTime);
			}
			this.Position += this.Velocity * deltaTime;
			this.CollisionDetection();
			if (this.SyncClient)
			{
				this.CurWorldPos = this.Position + this.GetRealClientPos();
			}
			else
			{
				this.CurWorldPos = this.Position;
			}
			if (this.Owner.UseSubEmitters && this.SubEmitter != null && XffectComponent.IsActive(this.SubEmitter.gameObject))
			{
				this.SubEmitter.transform.position = this.CurWorldPos;
			}
			if (this.Type == 1)
			{
				this.UpdateSprite(deltaTime);
			}
			else if (this.Type == 2)
			{
				this.UpdateRibbonTrail(deltaTime);
			}
			else if (this.Type == 3)
			{
				this.UpdateCone(deltaTime);
			}
			else if (this.Type == 4)
			{
				this.UpdateCustomMesh(deltaTime);
			}
			this.LastWorldPos = this.CurWorldPos;
			if (this.ElapsedTime > this.LifeTime && this.LifeTime > 0f)
			{
				this.Reset();
				this.Remove();
			}
		}

		protected int Type;

		public int Index;

		public Transform ClientTrans;

		public bool SyncClient;

		public EffectLayer Owner;

		protected Vector3 CurDirection;

		protected Vector3 LastWorldPos = Vector3.zero;

		protected Vector3 CurWorldPos;

		protected float ElapsedTime;

		public Sprite Sprite;

		public RibbonTrail Ribbon;

		public Cone Cone;

		public CustomMesh CusMesh;

		public Vector3 Position;

		public Vector2 LowerLeftUV;

		public Vector2 UVDimensions;

		public Vector3 Velocity;

		public Vector2 Scale;

		public float RotateAngle;

		public Color Color;

		public XffectComponent SubEmitter;

		protected List<Affector> AffectorList;

		protected Vector3 OriDirection;

		protected float LifeTime;

		protected int OriRotateAngle;

		protected float OriScaleX;

		protected float OriScaleY;

		protected bool SimpleSprite;

		protected bool IsCollisionEventSended;

		protected Vector3 LastCollisionDetectDir = Vector3.zero;
	}
}
