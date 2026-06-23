using System;
using UnityEngine;

public class LingyuAttributeMap : MonoBehaviour
{
	private void Start()
	{
		if (this._mf == null)
		{
			this._mf = (MeshFilter)base.gameObject.AddComponent(typeof(MeshFilter));
		}
		if (this._mr == null)
		{
			this._mr = (MeshRenderer)base.gameObject.AddComponent(typeof(MeshRenderer));
			this._mr.material = this._mt;
		}
	}

	private void Update()
	{
		if (this.isActivity)
		{
			this._point1 = new Vector3(-3f * (1f - this.WufangValue), 4.5f * (1f - this.WufangValue), 0f);
			this._point2 = new Vector3(3f * (1f - this.WugongValue), 4.5f * (1f - this.WugongValue), 0f);
			this._point3 = new Vector3(6f * (1f - this.LifeValue), 0f * (1f - this.LifeValue), 0f);
			this._point4 = new Vector3(3f * (1f - this.MogongValue), -4.5f * (1f - this.MogongValue), 0f);
			this._point5 = new Vector3(-3f * (1f - this.MofangValue), -4.5f * (1f - this.MofangValue), 0f);
			this._point6 = new Vector3(-6f * (1f - this.MingzhongValue), 0f * (1f - this.MingzhongValue), 0f);
			this.newVertices = new Vector3[]
			{
				this._point1,
				this._point2,
				this._point3,
				this._point4,
				this._point5,
				this._point6,
				this.origin
			};
			this.newTriangles = new int[]
			{
				6,
				0,
				1,
				6,
				1,
				2,
				6,
				2,
				3,
				6,
				3,
				4,
				6,
				4,
				5,
				6,
				5,
				0
			};
			this.newUV = new Vector2[]
			{
				Vector2.Lerp(this.point1, this._origin, this.WufangValue),
				Vector2.Lerp(this.point2, this._origin, this.WugongValue),
				Vector2.Lerp(this.point3, this._origin, this.LifeValue),
				Vector2.Lerp(this.point4, this._origin, this.MogongValue),
				Vector2.Lerp(this.point5, this._origin, this.MofangValue),
				Vector2.Lerp(this.point6, this._origin, this.MingzhongValue),
				new Vector2(0.5f, 0.5f)
			};
			this._mf.mesh.vertices = this.newVertices;
			this._mf.mesh.uv = this.newUV;
			this._mf.mesh.triangles = this.newTriangles;
			this.InitMapView();
		}
	}

	private void InitMapView()
	{
		this.UsedTime += Time.deltaTime;
		if (this.TotalTime != 0f)
		{
			this.LifeValue = Mathf.Lerp(this.LifeValue, 1f - this.item.attribute[0], this.UsedTime / this.TotalTime);
			this.WugongValue = Mathf.Lerp(this.WugongValue, 1f - this.item.attribute[1], this.UsedTime / this.TotalTime);
			this.WufangValue = Mathf.Lerp(this.WufangValue, 1f - this.item.attribute[2], this.UsedTime / this.TotalTime);
			this.MogongValue = Mathf.Lerp(this.MogongValue, 1f - this.item.attribute[3], this.UsedTime / this.TotalTime);
			this.MofangValue = Mathf.Lerp(this.MofangValue, 1f - this.item.attribute[4], this.UsedTime / this.TotalTime);
			this.MingzhongValue = Mathf.Lerp(this.MingzhongValue, 1f - this.item.attribute[5], this.UsedTime / this.TotalTime);
		}
		if (this.UsedTime >= this.TotalTime)
		{
			this.isActivity = false;
		}
	}

	public void BeginInitMapView()
	{
		if (this.item != null)
		{
			this.isActivity = true;
			this.UsedTime = 0f;
		}
	}

	protected Vector3 _point1;

	protected Vector3 _point2;

	protected Vector3 _point3;

	protected Vector3 _point4;

	protected Vector3 _point5;

	protected Vector3 _point6;

	protected Vector3 origin = Vector3.zero;

	public float WufangValue;

	public float WugongValue;

	public float LifeValue;

	public float MogongValue;

	public float MofangValue;

	public float MingzhongValue;

	public MeshFilter _mf;

	public Material _mt;

	public MeshRenderer _mr;

	private Vector3[] newVertices;

	private Vector2[] newUV;

	private int[] newTriangles;

	public LingYuItem item;

	private Vector2 point1 = new Vector2(0.25f, 1f);

	private Vector2 point2 = new Vector2(0.75f, 1f);

	private Vector2 point3 = new Vector2(1f, 0.5f);

	private Vector2 point4 = new Vector2(0.75f, 0f);

	private Vector2 point5 = new Vector2(0.25f, 0f);

	private Vector2 point6 = new Vector2(0f, 0.5f);

	private Vector2 _origin = new Vector2(0.5f, 0.5f);

	private float UsedTime;

	private float TotalTime = 3f;

	private bool isActivity;
}
