using System;
using UnityEngine;

namespace Xft
{
	public class CustomMesh
	{
		public CustomMesh(VertexPool.VertexSegment segment, Mesh mesh, Vector3 dir, float maxFps, EffectNode owner)
		{
			this.MyMesh = mesh;
			this.m_owner = owner;
			this.MeshVerts = new Vector3[mesh.vertices.Length];
			mesh.vertices.CopyTo(this.MeshVerts, 0);
			this.Vertexsegment = segment;
			this.MyDirection = dir;
			this.Fps = 1f / maxFps;
			this.SetPosition(Vector3.zero);
			this.InitVerts();
		}

		public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
		{
			this.LowerLeftUV = lowerleft;
			this.UVDimensions = dimensions;
			this.UVChanged = true;
		}

		public void SetColor(Color c)
		{
			this.MyColor = c;
			this.ColorChanged = true;
		}

		public void SetPosition(Vector3 pos)
		{
			this.MyPosition = pos;
		}

		public void SetScale(float width, float height)
		{
			this.MyScale.x = width;
			this.MyScale.y = height;
		}

		public void SetRotation(float angle)
		{
			this.MyRotation = Quaternion.AngleAxis(angle, Vector3.up);
		}

		public void InitVerts()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int indexStart = this.Vertexsegment.IndexStart;
			int vertStart = this.Vertexsegment.VertStart;
			for (int i = 0; i < this.MeshVerts.Length; i++)
			{
				pool.Vertices[vertStart + i] = this.MeshVerts[i];
			}
			int[] triangles = this.MyMesh.triangles;
			for (int j = 0; j < this.Vertexsegment.IndexCount; j++)
			{
				pool.Indices[j + indexStart] = triangles[j] + vertStart;
			}
			this.m_oriUvs = this.MyMesh.uv;
			for (int k = 0; k < this.m_oriUvs.Length; k++)
			{
				pool.UVs[k + vertStart] = this.m_oriUvs[k];
			}
			Color[] colors = this.MyMesh.colors;
			for (int l = 0; l < colors.Length; l++)
			{
				pool.Colors[l + vertStart] = Color.clear;
			}
		}

		public void UpdateUV()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			for (int i = 0; i < this.m_oriUvs.Length; i++)
			{
				pool.UVs[i + vertStart] = this.LowerLeftUV + Vector2.Scale(this.m_oriUvs[i], this.UVDimensions);
			}
			this.Vertexsegment.Pool.UVChanged = true;
		}

		public void UpdateColor()
		{
			VertexPool pool = this.Vertexsegment.Pool;
			int vertStart = this.Vertexsegment.VertStart;
			for (int i = 0; i < this.Vertexsegment.VertCount; i++)
			{
				pool.Colors[vertStart + i] = this.MyColor;
			}
			this.Vertexsegment.Pool.ColorChanged = true;
		}

		public void Transform()
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.MyDirection);
			Vector3 one = Vector3.one;
			one.x = (one.z = this.MyScale.x);
			one.y = this.MyScale.y;
			this.LocalMat.SetTRS(Vector3.zero, quaternion * this.MyRotation, one);
			this.WorldMat.SetTRS(this.MyPosition, Quaternion.identity, Vector3.one);
			Matrix4x4 matrix4x = this.WorldMat * this.LocalMat;
			VertexPool pool = this.Vertexsegment.Pool;
			for (int i = this.Vertexsegment.VertStart; i < this.Vertexsegment.VertStart + this.Vertexsegment.VertCount; i++)
			{
				pool.Vertices[i] = matrix4x.MultiplyPoint3x4(this.MeshVerts[i - this.Vertexsegment.VertStart]);
			}
		}

		public void Update(bool force, float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			if (this.ElapsedTime > this.Fps || force)
			{
				this.Transform();
				if (this.ColorChanged)
				{
					this.UpdateColor();
				}
				if (this.UVChanged)
				{
					this.UpdateUV();
				}
				this.ColorChanged = (this.UVChanged = false);
				if (!force)
				{
					this.ElapsedTime -= this.Fps;
				}
			}
		}

		protected VertexPool.VertexSegment Vertexsegment;

		public Mesh MyMesh;

		public Vector3[] MeshVerts;

		public Color MyColor;

		public Vector3 MyPosition = Vector3.zero;

		public Vector2 MyScale = Vector2.one;

		public Quaternion MyRotation = Quaternion.identity;

		public Vector3 MyDirection;

		private Matrix4x4 LocalMat;

		private Matrix4x4 WorldMat;

		private float Fps = 0.016f;

		private float ElapsedTime;

		public bool ColorChanged;

		public bool UVChanged;

		protected Vector2 LowerLeftUV;

		protected Vector2 UVDimensions;

		protected Vector2[] m_oriUvs;

		protected EffectNode m_owner;
	}
}
