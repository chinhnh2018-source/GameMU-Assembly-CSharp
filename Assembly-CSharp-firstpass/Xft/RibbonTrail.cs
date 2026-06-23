using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xft
{
	public class RibbonTrail
	{
		public RibbonTrail(VertexPool.VertexSegment segment, Camera mainCam, bool useFaceObj, Transform faceobj, float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
		{
			if (maxelemnt <= 2)
			{
				Debug.LogError("ribbon trail's maxelement should > 2!");
			}
			this.MaxElements = maxelemnt;
			this.Vertexsegment = segment;
			this.ElementArray = new RibbonTrail.Element[this.MaxElements];
			this.Head = (this.Tail = 99999);
			this.OriHeadPos = pos;
			this.SetTrailLen(len);
			this.UnitWidth = width;
			this.HeadPosition = pos;
			this.StretchType = stretchType;
			Vector3 vector;
			if (this.UseFaceObject)
			{
				vector = faceobj.position - this.HeadPosition;
			}
			else
			{
				vector = Vector3.zero;
			}
			RibbonTrail.Element element = new RibbonTrail.Element(this.HeadPosition, this.UnitWidth);
			element.Normal = vector.normalized;
			this.IndexDirty = false;
			this.Fps = 1f / maxFps;
			this.AddElememt(element);
			this.AddElememt(new RibbonTrail.Element(this.HeadPosition, this.UnitWidth)
			{
				Normal = vector.normalized
			});
			for (int i = 0; i < this.MaxElements; i++)
			{
				this.ElementPool.Add(new RibbonTrail.Element(this.HeadPosition, this.UnitWidth));
			}
			this.MainCam = mainCam;
			this.UseFaceObject = useFaceObj;
			this.FaceObject = faceobj;
		}

		public Camera MainCamera
		{
			get
			{
				return this.MainCam;
			}
			set
			{
				this.MainCam = value;
			}
		}

		public void ResetElementsPos()
		{
			if (this.Head != 99999 && this.Head != this.Tail)
			{
				int num = this.Head;
				for (;;)
				{
					int num2 = num;
					if (num2 == this.MaxElements)
					{
						num2 = 0;
					}
					this.ElementArray[num2].Position = this.OriHeadPos;
					if (num2 == this.Tail)
					{
						break;
					}
					num = num2 + 1;
				}
			}
		}

		public void Reset()
		{
			this.ResetElementsPos();
			this.StretchCount = 0;
		}

		public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
		{
			this.LowerLeftUV = lowerleft;
			this.UVDimensions = dimensions;
			this.UVDimensions.y = -this.UVDimensions.y;
			this.LowerLeftUV.y = 1f - this.LowerLeftUV.y;
		}

		public void SetColor(Color color)
		{
			this.Color = color;
		}

		public void SetTrailLen(float len)
		{
			this.TrailLength = len;
			this.ElemLength = this.TrailLength / (float)(this.MaxElements - 1);
			this.SquaredElemLength = this.ElemLength * this.ElemLength;
		}

		public void SetHeadPosition(Vector3 pos)
		{
			this.HeadPosition = pos;
		}

		public int GetStretchCount()
		{
			return this.StretchCount;
		}

		public void Smooth()
		{
			if (this.ElemCount <= 3)
			{
				return;
			}
			RibbonTrail.Element element = this.ElementArray[this.Head];
			int num = this.Head + 1;
			if (num == this.MaxElements)
			{
				num = 0;
			}
			int num2 = num + 1;
			if (num2 == this.MaxElements)
			{
				num2 = 0;
			}
			RibbonTrail.Element element2 = this.ElementArray[num];
			RibbonTrail.Element element3 = this.ElementArray[num2];
			Vector3 vector = element.Position - element2.Position;
			Vector3 vector2 = element2.Position - element3.Position;
			float num3 = Vector3.Angle(vector, vector2);
			if (num3 > 60f)
			{
				Vector3 vector3 = (element.Position + element3.Position) / 2f;
				Vector3 vector4 = vector3 - element2.Position;
				Vector3 zero = Vector3.zero;
				float num4 = 0.1f / (num3 / 60f);
				element2.Position = Vector3.SmoothDamp(element2.Position, element2.Position + vector4.normalized * element2.Width, ref zero, num4);
			}
		}

		public void Update(float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			if (this.ElapsedTime < this.Fps)
			{
				return;
			}
			this.ElapsedTime -= this.Fps;
			bool flag = false;
			Vector3 vector = Vector3.one * this.Owner.Owner.Owner.Scale;
			while (!flag)
			{
				RibbonTrail.Element element = this.ElementArray[this.Head];
				int num = this.Head + 1;
				if (num == this.MaxElements)
				{
					num = 0;
				}
				RibbonTrail.Element element2 = this.ElementArray[num];
				Vector3 headPosition = this.HeadPosition;
				Vector3 vector2 = headPosition - element2.Position;
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude >= this.SquaredElemLength)
				{
					this.StretchCount++;
					Vector3 vector3 = vector2 * (this.ElemLength / vector2.magnitude);
					element.Position = element2.Position + vector3;
					RibbonTrail.Element element3 = this.ElementPool[0];
					this.ElementPool.RemoveAt(0);
					element3.Position = headPosition;
					element3.Width = this.UnitWidth;
					if (this.UseFaceObject)
					{
						element3.Normal = this.FaceObject.position - Vector3.Scale(headPosition, vector);
					}
					else
					{
						element3.Normal = Vector3.zero;
					}
					this.AddElememt(element3);
					vector2 = headPosition - element.Position;
					if (vector2.sqrMagnitude <= this.SquaredElemLength)
					{
						flag = true;
					}
				}
				else
				{
					element.Position = headPosition;
					flag = true;
				}
				if ((this.Tail + 1) % this.MaxElements == this.Head)
				{
					RibbonTrail.Element element4 = this.ElementArray[this.Tail];
					int num2;
					if (this.Tail == 0)
					{
						num2 = this.MaxElements - 1;
					}
					else
					{
						num2 = this.Tail - 1;
					}
					RibbonTrail.Element element5 = this.ElementArray[num2];
					Vector3 vector4 = element4.Position - element5.Position;
					float magnitude = vector4.magnitude;
					if ((double)magnitude > 1E-06)
					{
						float num3 = this.ElemLength - vector2.magnitude;
						vector4 *= num3 / magnitude;
						element4.Position = element5.Position + vector4;
					}
				}
			}
			Vector3 position = this.MainCam.transform.position;
			this.UpdateVertices(position);
			this.UpdateIndices();
		}

		public void UpdateIndices()
		{
			if (!this.IndexDirty)
			{
				return;
			}
			VertexPool pool = this.Vertexsegment.Pool;
			if (this.Head != 99999 && this.Head != this.Tail)
			{
				int num = this.Head;
				int num2 = 0;
				for (;;)
				{
					int num3 = num + 1;
					if (num3 == this.MaxElements)
					{
						num3 = 0;
					}
					if (num3 * 2 >= 65536)
					{
						Debug.LogError("Too many elements!");
					}
					int num4 = this.Vertexsegment.VertStart + num3 * 2;
					int num5 = this.Vertexsegment.VertStart + num * 2;
					int num6 = this.Vertexsegment.IndexStart + num2 * 6;
					pool.Indices[num6] = num5;
					pool.Indices[num6 + 1] = num5 + 1;
					pool.Indices[num6 + 2] = num4;
					pool.Indices[num6 + 3] = num5 + 1;
					pool.Indices[num6 + 4] = num4 + 1;
					pool.Indices[num6 + 5] = num4;
					if (num3 == this.Tail)
					{
						break;
					}
					num = num3;
					num2++;
				}
				pool.IndiceChanged = true;
			}
			this.IndexDirty = false;
		}

		public void UpdateVertices(Vector3 eyePos)
		{
			float num = 0f;
			float num2 = this.ElemLength * (float)(this.MaxElements - 2);
			Vector3 vector = Vector3.one * this.Owner.Owner.Owner.Scale;
			if (this.Head != 99999 && this.Head != this.Tail)
			{
				int num3 = this.Head;
				int num4 = this.Head;
				for (;;)
				{
					if (num4 == this.MaxElements)
					{
						num4 = 0;
					}
					RibbonTrail.Element element = this.ElementArray[num4];
					if (num4 * 2 >= 65536)
					{
						Debug.LogError("Too many elements!");
					}
					int num5 = this.Vertexsegment.VertStart + num4 * 2;
					int num6 = num4 + 1;
					if (num6 == this.MaxElements)
					{
						num6 = 0;
					}
					Vector3 vector2;
					if (num4 == this.Head)
					{
						vector2 = Vector3.Scale(this.ElementArray[num6].Position, vector) - Vector3.Scale(element.Position, vector);
					}
					else if (num4 == this.Tail)
					{
						vector2 = Vector3.Scale(element.Position, vector) - Vector3.Scale(this.ElementArray[num3].Position, vector);
					}
					else
					{
						vector2 = Vector3.Scale(this.ElementArray[num6].Position, vector) - Vector3.Scale(this.ElementArray[num3].Position, vector);
					}
					Vector3 vector3;
					if (!this.UseFaceObject)
					{
						vector3 = eyePos - Vector3.Scale(element.Position, vector);
					}
					else
					{
						vector3 = element.Normal;
					}
					Vector3 vector4 = Vector3.Cross(vector2, vector3);
					vector4.Normalize();
					vector4 *= element.Width * 0.5f;
					Vector3 vector5 = element.Position - vector4;
					Vector3 vector6 = element.Position + vector4;
					VertexPool pool = this.Vertexsegment.Pool;
					float num7;
					if (this.StretchType == 0)
					{
						num7 = num / num2 * Mathf.Abs(this.UVDimensions.y);
					}
					else
					{
						num7 = num / num2 * Mathf.Abs(this.UVDimensions.x);
					}
					Vector2 zero = Vector2.zero;
					pool.Vertices[num5] = vector5;
					pool.Colors[num5] = this.Color;
					if (this.StretchType == 0)
					{
						zero.x = this.LowerLeftUV.x + this.UVDimensions.x;
						zero.y = this.LowerLeftUV.y - num7;
					}
					else
					{
						zero.x = this.LowerLeftUV.x + num7;
						zero.y = this.LowerLeftUV.y;
					}
					pool.UVs[num5] = zero;
					pool.Vertices[num5 + 1] = vector6;
					pool.Colors[num5 + 1] = this.Color;
					if (this.StretchType == 0)
					{
						zero.x = this.LowerLeftUV.x;
						zero.y = this.LowerLeftUV.y - num7;
					}
					else
					{
						zero.x = this.LowerLeftUV.x + num7;
						zero.y = this.LowerLeftUV.y - Mathf.Abs(this.UVDimensions.y);
					}
					pool.UVs[num5 + 1] = zero;
					if (num4 == this.Tail)
					{
						break;
					}
					num3 = num4;
					num += (this.ElementArray[num6].Position - element.Position).magnitude;
					num4++;
				}
				this.Vertexsegment.Pool.UVChanged = true;
				this.Vertexsegment.Pool.VertChanged = true;
				this.Vertexsegment.Pool.ColorChanged = true;
			}
		}

		public void AddElememt(RibbonTrail.Element dtls)
		{
			if (this.Head == 99999)
			{
				this.Tail = this.MaxElements - 1;
				this.Head = this.Tail;
				this.IndexDirty = true;
				this.ElemCount++;
			}
			else
			{
				if (this.Head == 0)
				{
					this.Head = this.MaxElements - 1;
				}
				else
				{
					this.Head--;
				}
				if (this.Head == this.Tail)
				{
					if (this.Tail == 0)
					{
						this.Tail = this.MaxElements - 1;
					}
					else
					{
						this.Tail--;
					}
				}
				else
				{
					this.ElemCount++;
				}
			}
			if (this.ElementArray[this.Head] != null)
			{
				this.ElementPool.Add(this.ElementArray[this.Head]);
			}
			this.ElementArray[this.Head] = dtls;
			this.IndexDirty = true;
		}

		public const int CHAIN_EMPTY = 99999;

		protected List<RibbonTrail.Element> ElementPool = new List<RibbonTrail.Element>();

		protected VertexPool.VertexSegment Vertexsegment;

		public int MaxElements;

		public RibbonTrail.Element[] ElementArray;

		public int Head;

		public int Tail;

		protected Vector3 HeadPosition;

		protected float TrailLength;

		protected float ElemLength;

		public float SquaredElemLength;

		protected float UnitWidth;

		protected bool IndexDirty;

		protected Vector2 LowerLeftUV;

		protected Vector2 UVDimensions;

		protected Color Color = Color.white;

		public int ElemCount;

		protected int StretchType;

		protected float ElapsedTime;

		protected float Fps;

		protected int StretchCount;

		public Vector3 OriHeadPos;

		private Camera MainCam;

		private bool UseFaceObject;

		private Transform FaceObject;

		public EffectNode Owner;

		public class Element
		{
			public Element(Vector3 position, float width)
			{
				this.Position = position;
				this.Width = width;
			}

			public Vector3 Position;

			public Vector3 Normal;

			public float Width;
		}
	}
}
