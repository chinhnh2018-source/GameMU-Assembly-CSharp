using System;
using System.Collections.Generic;
using UnityEngine;

public class CavasAnimation : UserControl
{
	private static int Comparison(CavasAnimation.Entry a, CavasAnimation.Entry b)
	{
		if (a.movementStart < b.movementStart)
		{
			return -1;
		}
		if (a.movementStart > b.movementStart)
		{
			return 1;
		}
		return 0;
	}

	public bool isVisible
	{
		get
		{
			return this.mList.Count != 0;
		}
	}

	private CavasAnimation.Entry Create()
	{
		if (this.mUnused.Count > 0)
		{
			CavasAnimation.Entry entry = this.mUnused[this.mUnused.Count - 1];
			this.mUnused.RemoveAt(this.mUnused.Count - 1);
			entry.time = Time.realtimeSinceStartup;
			NGUITools.SetActive(entry.label.gameObject, true);
			entry.offset = 0f;
			this.mList.Add(entry);
			return entry;
		}
		CavasAnimation.Entry entry2 = new CavasAnimation.Entry();
		entry2.time = Time.realtimeSinceStartup;
		entry2.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		this.mList.Add(entry2);
		this.counter++;
		return entry2;
	}

	private void Delete(CavasAnimation.Entry ent)
	{
		this.mList.Remove(ent);
		this.mUnused.Add(ent);
		NGUITools.SetActive(ent.label.gameObject, false);
	}

	public void Add(object obj, Color c, float stayDuration)
	{
		if (!base.enabled)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float val = 0f;
		bool flag = false;
		if (obj is UIPanel)
		{
			CavasAnimation.Entry entry = this.Create();
			entry.stay = stayDuration;
			entry.val = val;
			entry.label = (obj as UIPanel);
			flag = true;
		}
		if (flag)
		{
			this.mList.Sort(new Comparison<CavasAnimation.Entry>(CavasAnimation.Comparison));
		}
	}

	private void OnDisable()
	{
		int i = this.mList.Count;
		while (i > 0)
		{
			CavasAnimation.Entry entry = this.mList[--i];
			if (entry.label != null)
			{
				entry.label.enabled = false;
			}
			else
			{
				this.mList.RemoveAt(i);
			}
		}
	}

	private new void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Keyframe[] keys = this.offsetCurve.keys;
		Keyframe[] keys2 = this.alphaCurve.keys;
		Keyframe[] keys3 = this.scaleCurve.keys;
		Keyframe[] keys4 = this.RotateCurve.keys;
		float time = keys[keys.Length - 1].time;
		float time2 = keys2[keys2.Length - 1].time;
		float time3 = keys3[keys3.Length - 1].time;
		float num = Mathf.Max(time3, Mathf.Max(time, time2));
		int i = this.mList.Count;
		while (i > 0)
		{
			CavasAnimation.Entry entry = this.mList[--i];
			float num2 = realtimeSinceStartup - entry.movementStart;
			entry.offset = this.offsetCurve.Evaluate(num2);
			entry.label.alpha = this.alphaCurve.Evaluate(num2);
			entry.rotate = this.RotateCurve.Evaluate(num2);
			float num3 = this.scaleCurve.Evaluate(realtimeSinceStartup - entry.time) * entry.size;
			if (num3 < 0.001f)
			{
				num3 = 0.001f;
			}
			entry.label.cachedTransform.localScale = new Vector3(num3, num3, num3);
			if (num2 > num)
			{
				this.Delete(entry);
			}
			else
			{
				entry.label.enabled = true;
			}
		}
		float num4 = 0f;
		int j = this.mList.Count;
		while (j > 0)
		{
			CavasAnimation.Entry entry2 = this.mList[--j];
			num4 = Mathf.Max(num4, entry2.offset);
			entry2.label.cachedTransform.localPosition = new Vector3(0f, num4, 0f);
			num4 += Mathf.Round(entry2.label.cachedTransform.localScale.y);
			entry2.label.transform.localRotation = Quaternion.AngleAxis(entry2.rotate, Vector3.forward);
		}
	}

	public UIFont font;

	public UILabel.Effect effect;

	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(3f, 40f)
	});

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(1f, 1f),
		new Keyframe(3f, 0f)
	});

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.25f, 1f)
	});

	public AnimationCurve RotateCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.25f, 1f)
	});

	public Vector3 ItemSize;

	private List<CavasAnimation.Entry> mList = new List<CavasAnimation.Entry>();

	private List<CavasAnimation.Entry> mUnused = new List<CavasAnimation.Entry>();

	private int counter;

	protected class Entry
	{
		public float movementStart
		{
			get
			{
				return this.time + this.stay;
			}
		}

		public float time;

		public float stay;

		public float offset;

		public float val;

		public UIPanel label;

		public float size;

		public float rotate;
	}
}
