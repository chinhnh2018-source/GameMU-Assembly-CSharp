using System;
using UnityEngine;

namespace Xft
{
	public class Sprite
	{
		public Sprite(VertexPool.VertexSegment segment, float width, float height, STYPE type, ORIPOINT oripoint, Camera cam, int uvStretch, float maxFps, bool simple)
		{
			this.UVChanged = (this.ColorChanged = false);
			this.MyTransform.position = Vector3.zero;
			this.MyTransform.rotation = Quaternion.identity;
			this.LocalMat = (this.WorldMat = Matrix4x4.identity);
			this.Vertexsegment = segment;
			this.UVStretch = uvStretch;
			this.LastMat = Matrix4x4.identity;
			this.ElapsedTime = 0f;
			this.Fps = 1f / maxFps;
			this.Simple = simple;
			this.OriPoint = oripoint;
			this.RotateAxis = Vector3.zero;
			this.SetSizeXZ(width, height);
			this.RotateAxis.y = 1f;
			this.Type = type;
			this.MainCamera = cam;
			this.ResetSegment();
		}

		public void ResetSegment()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int indexStart = this.Vertexsegment.IndexStart;
			int vertStart = this.Vertexsegment.VertStart;
			pool.Indices[indexStart] = vertStart;
			pool.Indices[indexStart + 1] = vertStart + 3;
			pool.Indices[indexStart + 2] = vertStart + 1;
			pool.Indices[indexStart + 3] = vertStart + 3;
			pool.Indices[indexStart + 4] = vertStart + 2;
			pool.Indices[indexStart + 5] = vertStart + 1;
			pool.Vertices[vertStart] = Vector3.zero;
			pool.Vertices[vertStart + 1] = Vector3.zero;
			pool.Vertices[vertStart + 2] = Vector3.zero;
			pool.Vertices[vertStart + 3] = Vector3.zero;
			pool.Colors[vertStart] = Color.white;
			pool.Colors[vertStart + 1] = Color.white;
			pool.Colors[vertStart + 2] = Color.white;
			pool.Colors[vertStart + 3] = Color.white;
			pool.UVs[vertStart] = Vector2.zero;
			pool.UVs[vertStart + 1] = Vector2.zero;
			pool.UVs[vertStart + 2] = Vector2.zero;
			pool.UVs[vertStart + 3] = Vector2.zero;
			pool.UVChanged = (pool.IndiceChanged = (pool.ColorChanged = (pool.VertChanged = true)));
		}

		public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
		{
			this.LowerLeftUV = lowerleft;
			this.LowerLeftUV.y = 1f - this.LowerLeftUV.y;
			this.UVDimensions = dimensions;
			this.UVDimensions.y = -this.UVDimensions.y;
			this.UVChanged = true;
		}

		public void SetPosition(Vector3 pos)
		{
			this.MyTransform.position = pos;
		}

		public void SetRotation(Quaternion q)
		{
			this.MyTransform.rotation = q;
		}

		public void SetRotationFaceTo(Vector3 dir)
		{
			this.MyTransform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
		}

		public void SetRotationTo(Vector3 dir)
		{
			if (dir == Vector3.zero)
			{
				return;
			}
			Quaternion rotation = Quaternion.identity;
			Vector3 vector = dir;
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				vector = Vector3.up;
			}
			if (this.OriPoint == ORIPOINT.CENTER)
			{
				Quaternion quaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
				Quaternion quaternion2 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion2 * quaternion;
			}
			else if (this.OriPoint == ORIPOINT.LEFT_UP)
			{
				Quaternion quaternion3 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v3), vector);
				Quaternion quaternion4 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion4 * quaternion3;
			}
			else if (this.OriPoint == ORIPOINT.LEFT_BOTTOM)
			{
				Quaternion quaternion5 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v4), vector);
				Quaternion quaternion6 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion6 * quaternion5;
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_BOTTOM)
			{
				Quaternion quaternion7 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v1), vector);
				Quaternion quaternion8 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion8 * quaternion7;
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_UP)
			{
				Quaternion quaternion9 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v2), vector);
				Quaternion quaternion10 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion10 * quaternion9;
			}
			else if (this.OriPoint == ORIPOINT.BOTTOM_CENTER)
			{
				Quaternion quaternion11 = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
				Quaternion quaternion12 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion12 * quaternion11;
			}
			else if (this.OriPoint == ORIPOINT.TOP_CENTER)
			{
				Quaternion quaternion13 = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), vector);
				Quaternion quaternion14 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion14 * quaternion13;
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_CENTER)
			{
				Quaternion quaternion15 = Quaternion.FromToRotation(new Vector3(-1f, 0f, 0f), vector);
				Quaternion quaternion16 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion16 * quaternion15;
			}
			else if (this.OriPoint == ORIPOINT.LEFT_CENTER)
			{
				Quaternion quaternion17 = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), vector);
				Quaternion quaternion18 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion18 * quaternion17;
			}
			this.MyTransform.rotation = rotation;
		}

		public void SetSizeXZ(float width, float height)
		{
			this.v1 = new Vector3(-width / 2f, 0f, height / 2f);
			this.v2 = new Vector3(-width / 2f, 0f, -height / 2f);
			this.v3 = new Vector3(width / 2f, 0f, -height / 2f);
			this.v4 = new Vector3(width / 2f, 0f, height / 2f);
			Vector3 zero = Vector3.zero;
			if (this.OriPoint == ORIPOINT.LEFT_UP)
			{
				zero = this.v3;
			}
			else if (this.OriPoint == ORIPOINT.LEFT_BOTTOM)
			{
				zero = this.v4;
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_BOTTOM)
			{
				zero = this.v1;
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_UP)
			{
				zero = this.v2;
			}
			else if (this.OriPoint == ORIPOINT.BOTTOM_CENTER)
			{
				zero..ctor(0f, 0f, height / 2f);
			}
			else if (this.OriPoint == ORIPOINT.TOP_CENTER)
			{
				zero..ctor(0f, 0f, -height / 2f);
			}
			else if (this.OriPoint == ORIPOINT.LEFT_CENTER)
			{
				zero..ctor(width / 2f, 0f, 0f);
			}
			else if (this.OriPoint == ORIPOINT.RIGHT_CENTER)
			{
				zero..ctor(-width / 2f, 0f, 0f);
			}
			this.v1 += zero;
			this.v2 += zero;
			this.v3 += zero;
			this.v4 += zero;
		}

		public void UpdateUV()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			if (this.UVDimensions.y > 0f)
			{
				pool.UVs[vertStart] = this.LowerLeftUV + Vector2.up * this.UVDimensions.y;
				pool.UVs[vertStart + 1] = this.LowerLeftUV;
				pool.UVs[vertStart + 2] = this.LowerLeftUV + Vector2.right * this.UVDimensions.x;
				pool.UVs[vertStart + 3] = this.LowerLeftUV + this.UVDimensions;
			}
			else
			{
				pool.UVs[vertStart] = this.LowerLeftUV;
				pool.UVs[vertStart + 1] = this.LowerLeftUV + Vector2.up * this.UVDimensions.y;
				pool.UVs[vertStart + 2] = this.LowerLeftUV + this.UVDimensions;
				pool.UVs[vertStart + 3] = this.LowerLeftUV + Vector2.right * this.UVDimensions.x;
			}
			this.Vertexsegment.Pool.UVChanged = true;
		}

		public void UpdateColor()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			pool.Colors[vertStart] = this.Color;
			pool.Colors[vertStart + 1] = this.Color;
			pool.Colors[vertStart + 2] = this.Color;
			pool.Colors[vertStart + 3] = this.Color;
			this.Vertexsegment.Pool.ColorChanged = true;
		}

		public void SetCustomHeight(float[] h)
		{
			this.UseCustomHeight = true;
			this.h1 = h[0];
			this.h2 = h[1];
			this.h3 = h[2];
			this.h4 = h[3];
			this.Transform();
		}

		public void Transform()
		{
			this.LocalMat.SetTRS(Vector3.zero, this.Rotation, this.ScaleVector);
			Transform transform = this.MainCamera.transform;
			if (this.Type == STYPE.BILLBOARD)
			{
				this.MyTransform.LookAt(transform.rotation * Vector3.up, transform.rotation * Vector3.back);
			}
			else if (this.Type == STYPE.BILLBOARD_Y)
			{
				Vector3 up = transform.position - this.MyTransform.position;
				up.y = 0f;
				this.MyTransform.LookAt(Vector3.up, up);
			}
			this.WorldMat.SetTRS(this.MyTransform.position, this.MyTransform.rotation, Vector3.one);
			Matrix4x4 matrix4x = this.WorldMat * this.LocalMat;
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			Vector3 vector = matrix4x.MultiplyPoint3x4(this.v1);
			Vector3 vector2 = matrix4x.MultiplyPoint3x4(this.v2);
			Vector3 vector3 = matrix4x.MultiplyPoint3x4(this.v3);
			Vector3 vector4 = matrix4x.MultiplyPoint3x4(this.v4);
			if (this.Type == STYPE.BILLBOARD_SELF)
			{
				Vector3 vector5 = Vector3.zero;
				Vector3 vector6 = Vector3.zero;
				Vector3 vector7 = Vector3.one * this.Owner.Owner.Owner.Scale;
				float magnitude;
				if (this.UVStretch == 0)
				{
					vector5 = (vector + vector4) / 2f;
					vector6 = (vector2 + vector3) / 2f;
					magnitude = (vector4 - vector).magnitude;
				}
				else
				{
					vector5 = (vector + vector2) / 2f;
					vector6 = (vector4 + vector3) / 2f;
					magnitude = (vector2 - vector).magnitude;
				}
				Vector3 vector8 = vector5 - vector6;
				Vector3 vector9 = this.MainCamera.transform.position - Vector3.Scale(vector5, vector7);
				Vector3 vector10 = Vector3.Cross(vector8, vector9);
				vector10.Normalize();
				vector10 *= magnitude * 0.5f;
				Vector3 vector11 = this.MainCamera.transform.position - Vector3.Scale(vector6, vector7);
				Vector3 vector12 = Vector3.Cross(vector8, vector11);
				vector12.Normalize();
				vector12 *= magnitude * 0.5f;
				if (this.UVStretch == 0)
				{
					vector = vector5 - vector10;
					vector4 = vector5 + vector10;
					vector2 = vector6 - vector12;
					vector3 = vector6 + vector12;
				}
				else
				{
					vector = vector5 - vector10;
					vector2 = vector5 + vector10;
					vector4 = vector6 - vector12;
					vector3 = vector6 + vector12;
				}
			}
			pool.Vertices[vertStart] = vector;
			pool.Vertices[vertStart + 1] = vector2;
			pool.Vertices[vertStart + 2] = vector3;
			pool.Vertices[vertStart + 3] = vector4;
			if (this.UseCustomHeight)
			{
				pool.Vertices[vertStart].y = this.h1;
				pool.Vertices[vertStart + 1].y = this.h2;
				pool.Vertices[vertStart + 2].y = this.h3;
				pool.Vertices[vertStart + 3].y = this.h4;
			}
		}

		public void SetRotation(float angle)
		{
			this.Rotation = Quaternion.AngleAxis(angle, this.RotateAxis);
		}

		public void SetScale(float width, float height)
		{
			this.ScaleVector.x = width;
			this.ScaleVector.z = height;
		}

		public void Update(bool force, float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			if (this.ElapsedTime > this.Fps || force)
			{
				if (!this.Simple || force)
				{
					this.Transform();
				}
				if (this.UVChanged)
				{
					this.UpdateUV();
				}
				if (this.ColorChanged)
				{
					this.UpdateColor();
				}
				this.UVChanged = (this.ColorChanged = false);
				if (!force)
				{
					this.ElapsedTime -= this.Fps;
				}
			}
		}

		public void SetColor(Color c)
		{
			this.Color = c;
			this.ColorChanged = true;
		}

		protected Vector2 LowerLeftUV;

		protected Vector2 UVDimensions;

		public STransform MyTransform;

		public Vector3 v1 = Vector3.zero;

		public Vector3 v2 = Vector3.zero;

		public Vector3 v3 = Vector3.zero;

		public Vector3 v4 = Vector3.zero;

		protected VertexPool.VertexSegment Vertexsegment;

		public EffectNode Owner;

		public Color Color;

		private Vector3 ScaleVector;

		private Quaternion Rotation;

		private Matrix4x4 LocalMat;

		private Matrix4x4 WorldMat;

		protected float ElapsedTime;

		protected float Fps;

		public Camera MainCamera;

		protected bool UVChanged;

		protected bool ColorChanged;

		protected Matrix4x4 LastMat;

		protected Vector3 RotateAxis;

		private STYPE Type;

		private ORIPOINT OriPoint;

		private int UVStretch;

		private bool Simple;

		public bool UseCustomHeight;

		public float h1;

		public float h2;

		public float h3;

		public float h4;
	}
}
