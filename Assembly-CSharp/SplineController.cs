using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineInterpolator)), AddComponentMenu("Splines/Spline Controller")]
public class SplineController : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Transform[] transforms = this.GetTransforms();
		if (transforms == null || transforms.Length < 2)
		{
			return;
		}
		SplineInterpolator splineInterpolator = base.GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
		this.SetupSplineInterpolator(splineInterpolator, transforms);
		splineInterpolator.StartInterpolation(null, false, this.WrapMode);
		Vector3 vector = transforms[0].position;
		for (int i = 1; i <= 100; i++)
		{
			float timeParam = (float)i * this.Duration / 100f;
			Vector3 hermiteAtTime = splineInterpolator.GetHermiteAtTime(timeParam);
			float num = (hermiteAtTime - vector).magnitude * 2f;
			Gizmos.color = new Color(num, 0f, 0f, 1f);
			Gizmos.DrawLine(vector, hermiteAtTime);
			vector = hermiteAtTime;
		}
	}

	private void Start()
	{
		this.mSplineInterp = (base.GetComponent(typeof(SplineInterpolator)) as SplineInterpolator);
		this.mTransforms = this.GetTransforms();
		if (this.HideOnExecute)
		{
			this.DisableTransforms();
		}
		if (this.AutoStart)
		{
			this.FollowSpline();
		}
	}

	private void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		interp.Reset();
		if (this.AutoClose)
		{
			if (trans.Length == 0)
			{
				return;
			}
		}
		else if (trans.Length - 1 == 0)
		{
			return;
		}
		float num = (!this.AutoClose) ? (this.Duration / (float)(trans.Length - 1)) : (this.Duration / (float)trans.Length);
		int i;
		for (i = 0; i < trans.Length; i++)
		{
			if (this.OrientationMode == eOrientationMode.NODE)
			{
				interp.AddPoint(trans[i].position, trans[i].rotation, num * (float)i, new Vector2(0f, 1f));
			}
			else if (this.OrientationMode == eOrientationMode.TANGENT)
			{
				Quaternion quat;
				if (i != trans.Length - 1)
				{
					quat = Quaternion.LookRotation(trans[i + 1].position - trans[i].position, trans[i].up);
				}
				else if (this.AutoClose)
				{
					quat = Quaternion.LookRotation(trans[0].position - trans[i].position, trans[i].up);
				}
				else
				{
					quat = trans[i].rotation;
				}
				interp.AddPoint(trans[i].position, quat, num * (float)i, new Vector2(0f, 1f));
			}
		}
		if (this.AutoClose)
		{
			interp.SetAutoCloseMode(num * (float)i);
		}
	}

	private Transform[] GetTransforms()
	{
		if (this.SplineRoot != null)
		{
			List<Component> list = new List<Component>(this.SplineRoot.GetComponentsInChildren(typeof(Transform)));
			List<Transform> list2 = list.ConvertAll<Transform>((Component c) => (Transform)c);
			list2.Remove(this.SplineRoot.transform);
			list2.Sort((Transform a, Transform b) => a.name.CompareTo(b.name));
			return list2.ToArray();
		}
		return null;
	}

	private void DisableTransforms()
	{
		if (this.SplineRoot != null)
		{
			this.SplineRoot.SetActive(false);
		}
	}

	private void FollowSpline()
	{
		if (this.mTransforms.Length > 0)
		{
			this.SetupSplineInterpolator(this.mSplineInterp, this.mTransforms);
			this.mSplineInterp.StartInterpolation(null, true, this.WrapMode);
		}
	}

	public GameObject SplineRoot;

	public float Duration = 10f;

	public eOrientationMode OrientationMode;

	public eWrapMode WrapMode;

	public bool AutoStart = true;

	public bool AutoClose = true;

	public bool HideOnExecute = true;

	private SplineInterpolator mSplineInterp;

	private Transform[] mTransforms;
}
