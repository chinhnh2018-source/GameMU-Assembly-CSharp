using System;
using System.Collections.Generic;
using UnityEngine;

public class SplineInterpolator : MonoBehaviour
{
	private void Awake()
	{
		this.Reset();
	}

	public void StartInterpolation(OnEndCallback endCallback, bool bRotations, eWrapMode mode)
	{
		if (this.mState != "Reset")
		{
			throw new Exception("First reset, add points and then call here");
		}
		this.mState = ((mode != eWrapMode.ONCE) ? "Loop" : "Once");
		this.mRotations = bRotations;
		this.mOnEndCallback = endCallback;
		this.SetInput();
	}

	public void Reset()
	{
		this.mNodes.Clear();
		this.mState = "Reset";
		this.mCurrentIdx = 1;
		this.mCurrentTime = 0f;
		this.mRotations = false;
		this.mEndPointsMode = eEndPointsMode.AUTO;
	}

	public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds, Vector2 easeInOut)
	{
		if (this.mState != "Reset")
		{
			throw new Exception("Cannot add points after start");
		}
		this.mNodes.Add(new SplineInterpolator.SplineNode(pos, quat, timeInSeconds, easeInOut));
	}

	private void SetInput()
	{
		if (this.mNodes.Count < 2)
		{
			throw new Exception("Invalid number of points");
		}
		if (this.mRotations)
		{
			for (int i = 1; i < this.mNodes.Count; i++)
			{
				SplineInterpolator.SplineNode splineNode = this.mNodes[i];
				SplineInterpolator.SplineNode splineNode2 = this.mNodes[i - 1];
				if (Quaternion.Dot(splineNode.Rot, splineNode2.Rot) < 0f)
				{
					splineNode.Rot.x = -splineNode.Rot.x;
					splineNode.Rot.y = -splineNode.Rot.y;
					splineNode.Rot.z = -splineNode.Rot.z;
					splineNode.Rot.w = -splineNode.Rot.w;
				}
			}
		}
		if (this.mEndPointsMode == eEndPointsMode.AUTO)
		{
			this.mNodes.Insert(0, this.mNodes[0]);
			this.mNodes.Add(this.mNodes[this.mNodes.Count - 1]);
		}
		else if (this.mEndPointsMode == eEndPointsMode.EXPLICIT && this.mNodes.Count < 4)
		{
			throw new Exception("Invalid number of points");
		}
	}

	private void SetExplicitMode()
	{
		if (this.mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		this.mEndPointsMode = eEndPointsMode.EXPLICIT;
	}

	public void SetAutoCloseMode(float joiningPointTime)
	{
		if (this.mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		this.mEndPointsMode = eEndPointsMode.AUTOCLOSED;
		this.mNodes.Add(new SplineInterpolator.SplineNode(this.mNodes[0]));
		this.mNodes[this.mNodes.Count - 1].Time = joiningPointTime;
		Vector3 normalized = (this.mNodes[1].Point - this.mNodes[0].Point).normalized;
		Vector3 normalized2 = (this.mNodes[this.mNodes.Count - 2].Point - this.mNodes[this.mNodes.Count - 1].Point).normalized;
		float magnitude = (this.mNodes[1].Point - this.mNodes[0].Point).magnitude;
		float magnitude2 = (this.mNodes[this.mNodes.Count - 2].Point - this.mNodes[this.mNodes.Count - 1].Point).magnitude;
		SplineInterpolator.SplineNode splineNode = new SplineInterpolator.SplineNode(this.mNodes[0]);
		splineNode.Point = this.mNodes[0].Point + normalized2 * magnitude;
		SplineInterpolator.SplineNode splineNode2 = new SplineInterpolator.SplineNode(this.mNodes[this.mNodes.Count - 1]);
		splineNode2.Point = this.mNodes[0].Point + normalized * magnitude2;
		this.mNodes.Insert(0, splineNode);
		this.mNodes.Add(splineNode2);
	}

	private void Update()
	{
		if (this.mState == "Reset" || this.mState == "Stopped" || this.mNodes.Count < 4)
		{
			return;
		}
		this.mCurrentTime += Time.deltaTime;
		if (this.mCurrentTime >= this.mNodes[this.mCurrentIdx + 1].Time)
		{
			if (this.mCurrentIdx < this.mNodes.Count - 3)
			{
				this.mCurrentIdx++;
			}
			else if (this.mState != "Loop")
			{
				this.mState = "Stopped";
				base.transform.position = this.mNodes[this.mNodes.Count - 2].Point;
				if (this.mRotations)
				{
					base.transform.rotation = this.mNodes[this.mNodes.Count - 2].Rot;
				}
				if (this.mOnEndCallback != null)
				{
					this.mOnEndCallback();
				}
			}
			else
			{
				this.mCurrentIdx = 1;
				this.mCurrentTime = 0f;
			}
		}
		if (this.mState != "Stopped")
		{
			float t = (this.mCurrentTime - this.mNodes[this.mCurrentIdx].Time) / (this.mNodes[this.mCurrentIdx + 1].Time - this.mNodes[this.mCurrentIdx].Time);
			t = MathUtils.Ease(t, this.mNodes[this.mCurrentIdx].EaseIO.x, this.mNodes[this.mCurrentIdx].EaseIO.y);
			base.transform.position = this.GetHermiteInternal(this.mCurrentIdx, t);
			if (this.mRotations)
			{
				base.transform.rotation = this.GetSquad(this.mCurrentIdx, t);
			}
		}
	}

	private Quaternion GetSquad(int idxFirstPoint, float t)
	{
		Quaternion rot = this.mNodes[idxFirstPoint - 1].Rot;
		Quaternion rot2 = this.mNodes[idxFirstPoint].Rot;
		Quaternion rot3 = this.mNodes[idxFirstPoint + 1].Rot;
		Quaternion rot4 = this.mNodes[idxFirstPoint + 2].Rot;
		Quaternion squadIntermediate = MathUtils.GetSquadIntermediate(rot, rot2, rot3);
		Quaternion squadIntermediate2 = MathUtils.GetSquadIntermediate(rot2, rot3, rot4);
		return MathUtils.GetQuatSquad(t, rot2, rot3, squadIntermediate, squadIntermediate2);
	}

	public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		float num = t * t;
		float num2 = num * t;
		Vector3 point = this.mNodes[idxFirstPoint - 1].Point;
		Vector3 point2 = this.mNodes[idxFirstPoint].Point;
		Vector3 point3 = this.mNodes[idxFirstPoint + 1].Point;
		Vector3 point4 = this.mNodes[idxFirstPoint + 2].Point;
		float num3 = 0.5f;
		Vector3 vector = num3 * (point3 - point);
		Vector3 vector2 = num3 * (point4 - point2);
		float num4 = 2f * num2 - 3f * num + 1f;
		float num5 = -2f * num2 + 3f * num;
		float num6 = num2 - 2f * num + t;
		float num7 = num2 - num;
		return num4 * point2 + num5 * point3 + num6 * vector + num7 * vector2;
	}

	public Vector3 GetHermiteAtTime(float timeParam)
	{
		if (timeParam >= this.mNodes[this.mNodes.Count - 2].Time)
		{
			return this.mNodes[this.mNodes.Count - 2].Point;
		}
		int i;
		for (i = 1; i < this.mNodes.Count - 2; i++)
		{
			if (this.mNodes[i].Time > timeParam)
			{
				break;
			}
		}
		int num = i - 1;
		float t = (timeParam - this.mNodes[num].Time) / (this.mNodes[num + 1].Time - this.mNodes[num].Time);
		t = MathUtils.Ease(t, this.mNodes[num].EaseIO.x, this.mNodes[num].EaseIO.y);
		return this.GetHermiteInternal(num, t);
	}

	private eEndPointsMode mEndPointsMode;

	private List<SplineInterpolator.SplineNode> mNodes = new List<SplineInterpolator.SplineNode>();

	private string mState = string.Empty;

	private bool mRotations;

	private OnEndCallback mOnEndCallback;

	private float mCurrentTime;

	private int mCurrentIdx = 1;

	internal class SplineNode
	{
		internal SplineNode(Vector3 p, Quaternion q, float t, Vector2 io)
		{
			this.Point = p;
			this.Rot = q;
			this.Time = t;
			this.EaseIO = io;
		}

		internal SplineNode(SplineInterpolator.SplineNode o)
		{
			this.Point = o.Point;
			this.Rot = o.Rot;
			this.Time = o.Time;
			this.EaseIO = o.EaseIO;
		}

		internal Vector3 Point;

		internal Quaternion Rot;

		internal float Time;

		internal Vector2 EaseIO;
	}
}
