using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DragonTail : MonoBehaviour
{
	private void OnDisable()
	{
		this.sections.Clear();
		base.GetComponent<MeshFilter>().mesh.Clear();
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		float num = Time.time;
		while (this.sections.Count > 0 && num > this.sections[this.sections.Count - 1].time + this.time)
		{
			this.sections.RemoveAt(this.sections.Count - 1);
		}
		if (this.sections.Count == 0 || (this.sections[0].point - position).sqrMagnitude > this.minDistance * this.minDistance)
		{
			DragonTail.TrailSection trailSection = new DragonTail.TrailSection();
			trailSection.point = position;
			if (this.alwaysUp)
			{
				trailSection.upDir = Vector3.up;
			}
			else
			{
				trailSection.upDir = base.transform.TransformDirection(Vector3.up);
			}
			trailSection.time = num;
			this.sections.Insert(0, trailSection);
		}
		this.GenerateMesh();
	}

	private void GenerateMesh()
	{
		Mesh mesh = base.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		if (this.sections.Count < 2)
		{
			return;
		}
		Vector3[] array = new Vector3[this.sections.Count * 2];
		Color[] array2 = new Color[this.sections.Count * 2];
		Vector2[] array3 = new Vector2[this.sections.Count * 2];
		DragonTail.TrailSection trailSection = this.sections[0];
		DragonTail.TrailSection trailSection2 = this.sections[0];
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		for (int i = 0; i < this.sections.Count; i++)
		{
			trailSection2 = this.sections[i];
			float num = 0f;
			if (i != 0 && this.time != 0f)
			{
				num = Mathf.Clamp01((Time.time - trailSection2.time) / this.time);
			}
			Vector3 upDir = trailSection2.upDir;
			array[i * 2] = worldToLocalMatrix.MultiplyPoint(trailSection2.point);
			array[i * 2 + 1] = worldToLocalMatrix.MultiplyPoint(trailSection2.point + upDir * this.height);
			array3[i * 2] = new Vector2(num, 0f);
			array3[i * 2 + 1] = new Vector2(num, 1f);
			Color color = Color.Lerp(this.startColor, this.endColor, num);
			array2[i * 2] = color;
			array2[i * 2 + 1] = color;
		}
		int[] array4 = new int[(this.sections.Count - 1) * 2 * 3];
		for (int j = 0; j < array4.Length / 6; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 2;
			array4[j * 6 + 4] = j * 2 + 1;
			array4[j * 6 + 5] = j * 2 + 3;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
	}

	public float height = 2f;

	public float time = 2f;

	public bool alwaysUp;

	public float minDistance = 0.1f;

	public Color startColor = Color.white;

	public Color endColor = new Color(1f, 1f, 1f, 0f);

	private List<DragonTail.TrailSection> sections = new List<DragonTail.TrailSection>();

	private class TrailSection
	{
		public Vector3 point;

		public Vector3 upDir;

		public float time;
	}
}
