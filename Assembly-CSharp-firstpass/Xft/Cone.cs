using System;
using UnityEngine;

namespace Xft
{
	public class Cone
	{
		public Cone(VertexPool.VertexSegment segment, Vector2 size, int numseg, float angle, Vector3 dir, int uvStretch, float maxFps)
		{
			this.Vertexsegment = segment;
			this.Size = size;
			this.Direction = dir;
			this.UVStretch = uvStretch;
			this.Fps = 1f / maxFps;
			this.SetColor(Color.white);
			this.NumSegment = numseg;
			this.SpreadAngle = angle;
			this.OriSpreadAngle = angle;
			this.InitVerts();
		}

		public void SetUVCoord(Vector2 topleft, Vector2 dimensions)
		{
			this.LowerLeftUV = topleft;
			this.UVDimensions = dimensions;
			this.UVChanged = true;
		}

		public void SetColor(Color c)
		{
			this.Color = c;
			this.ColorChanged = true;
		}

		public void SetPosition(Vector3 pos)
		{
			this.Position = pos;
		}

		public void SetScale(float width, float height)
		{
			this.Scale.x = width;
			this.Scale.y = height;
		}

		public void SetRotation(float angle)
		{
			this.OriRotAngle = angle;
		}

		public void ResetAngle()
		{
			this.SpreadAngle = this.OriSpreadAngle;
		}

		protected void UpdateRotAngle(float deltaTime)
		{
			if (!this.UseDeltaAngle)
			{
				return;
			}
			this.SpreadAngle = this.CurveAngle.Evaluate(this.Owner.GetElapsedTime());
			for (int i = this.NumVerts / 2; i < this.NumVerts; i++)
			{
				this.Verts[i] = this.Verts[i - this.NumVerts / 2] + Vector3.up * this.Size.y;
				this.Verts[i] = Vector3.RotateTowards(this.Verts[i], this.Verts[i - this.NumVerts / 2], this.SpreadAngle * 0.0174532924f, 0f);
			}
		}

		public void UpdateVerts()
		{
			Vector3 vector = Vector3.forward * this.Size.x;
			for (int i = 0; i < this.NumVerts / 2; i++)
			{
				this.Verts[i] = Quaternion.Euler(0f, this.OriRotAngle + this.SegmentAngle * (float)i, 0f) * vector;
			}
			for (int j = this.NumVerts / 2; j < this.NumVerts; j++)
			{
				this.Verts[j] = this.Verts[j - this.NumVerts / 2] + Vector3.up * this.Size.y;
				this.Verts[j] = Vector3.RotateTowards(this.Verts[j], this.Verts[j - this.NumVerts / 2], this.SpreadAngle * 0.0174532924f, 0f);
			}
		}

		public void InitVerts()
		{
			this.NumVerts = (this.NumSegment + 1) * 2;
			this.SegmentAngle = 360f / (float)this.NumSegment;
			this.Verts = new Vector3[this.NumVerts];
			this.VertsTemp = new Vector3[this.NumVerts];
			this.UpdateVerts();
			VertexPool pool = this.Vertexsegment.Pool;
			int indexStart = this.Vertexsegment.IndexStart;
			int vertStart = this.Vertexsegment.VertStart;
			for (int i = 0; i < this.NumSegment; i++)
			{
				int num = indexStart + i * 6;
				int num2 = vertStart + i;
				pool.Indices[num] = num2 + this.NumSegment + 1;
				pool.Indices[num + 1] = num2 + this.NumSegment + 2;
				pool.Indices[num + 2] = num2;
				pool.Indices[num + 3] = num2 + this.NumSegment + 2;
				pool.Indices[num + 4] = num2 + 1;
				pool.Indices[num + 5] = num2;
				pool.Vertices[num2 + this.NumSegment + 1] = Vector3.zero;
				pool.Vertices[num2 + this.NumSegment + 2] = Vector3.zero;
				pool.Vertices[num2] = Vector3.zero;
				pool.Vertices[num2 + 1] = Vector3.zero;
			}
		}

		public void UpdateUV()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			float num = this.UVDimensions.x / (float)this.NumSegment;
			for (int i = 0; i < this.NumSegment + 1; i++)
			{
				pool.UVs[vertStart + i] = this.LowerLeftUV;
				Vector2[] uvs = pool.UVs;
				int num2 = vertStart + i;
				uvs[num2].x = uvs[num2].x + (float)i * num;
			}
			for (int j = this.NumSegment + 1; j < this.NumVerts; j++)
			{
				pool.UVs[vertStart + j] = this.LowerLeftUV + Vector2.up * this.UVDimensions.y;
				Vector2[] uvs2 = pool.UVs;
				int num3 = vertStart + j;
				uvs2[num3].x = uvs2[num3].x + (float)(j - this.NumSegment - 1) * num;
			}
			this.Vertexsegment.Pool.UVChanged = true;
		}

		public void UpdateColor()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			for (int i = 0; i < this.NumVerts; i++)
			{
				pool.Colors[vertStart + i] = this.Color;
			}
			this.Vertexsegment.Pool.ColorChanged = true;
		}

		public void Transform()
		{
			if (this.Owner.Owner.RotAffectorEnable || this.OriRotAngle != 0f)
			{
				this.UpdateVerts();
			}
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.Direction);
			for (int i = 0; i < this.NumSegment + 1; i++)
			{
				this.VertsTemp[i] = this.Verts[i] * this.Scale.x;
				this.VertsTemp[i] = quaternion * this.VertsTemp[i];
				this.VertsTemp[i] = this.VertsTemp[i] + this.Position;
			}
			for (int j = this.NumSegment + 1; j < this.NumVerts; j++)
			{
				this.VertsTemp[j] = this.Verts[j] * this.Scale.x;
				this.VertsTemp[j].y = this.Verts[j].y * this.Scale.y;
				this.VertsTemp[j] = quaternion * this.VertsTemp[j];
				this.VertsTemp[j] = this.VertsTemp[j] + this.Position;
			}
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			for (int k = 0; k < this.NumVerts; k++)
			{
				pool.Vertices[vertStart + k] = this.VertsTemp[k];
			}
		}

		public void Update(bool force, float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			if (this.ElapsedTime > this.Fps || force)
			{
				this.UpdateRotAngle(deltaTime);
				this.Transform();
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

		public Vector2 Size;

		public Vector3 Direction;

		public int UVStretch;

		public int NumSegment = 4;

		public float SpreadAngle;

		public float OriSpreadAngle;

		public float OriRotAngle = 45f;

		public bool UseDeltaAngle;

		public AnimationCurve CurveAngle;

		public EffectNode Owner;

		protected int NumVerts;

		protected float SegmentAngle;

		protected VertexPool.VertexSegment Vertexsegment;

		protected float Fps;

		protected Vector3[] Verts;

		protected Vector3[] VertsTemp;

		protected float ElapsedTime;

		protected bool UVChanged = true;

		protected bool ColorChanged = true;

		protected float OriUVX;

		public Vector3 Position = Vector3.zero;

		public Color Color;

		public Vector2 Scale;

		protected Vector2 LowerLeftUV = Vector2.zero;

		protected Vector2 UVDimensions = Vector2.one;
	}
}
