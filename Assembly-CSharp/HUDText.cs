using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDText : MonoBehaviour
{
	private static int Comparison(HUDText.Entry a, HUDText.Entry b)
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

	private HUDText.Entry Create()
	{
		if (this.mUnused.Count > 0)
		{
			HUDText.Entry entry = this.mUnused[this.mUnused.Count - 1];
			this.mUnused.RemoveAt(this.mUnused.Count - 1);
			entry.time = Time.realtimeSinceStartup;
			entry.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
			entry.label.enabled = true;
			entry.offset = 0f;
			this.mList.Add(entry);
			return entry;
		}
		HUDText.Entry entry2 = new HUDText.Entry();
		entry2.time = Time.realtimeSinceStartup;
		entry2.label = NGUITools.AddWidget<UILabel>(base.gameObject);
		entry2.label.name = this.counter.ToString();
		entry2.label.effectStyle = this.effect;
		entry2.label.font = this.font;
		entry2.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		this.mList.Add(entry2);
		this.counter++;
		return entry2;
	}

	private void Delete(HUDText.Entry ent)
	{
		this.mList.Remove(ent);
		this.mUnused.Add(ent);
		ent.label.enabled = false;
	}

	public void Add(object obj, Color c, float stayDuration, float offsetX = 0f, float fontSize = 0f)
	{
		if (!base.enabled)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = false;
		float num = 0f;
		if (obj is float)
		{
			flag = true;
			num = (float)obj;
		}
		else if (obj is int)
		{
			flag = true;
			num = (float)((int)obj);
		}
		if (flag)
		{
			if (num == 0f)
			{
				return;
			}
			int i = this.mList.Count;
			while (i > 0)
			{
				HUDText.Entry entry = this.mList[--i];
				if (entry.time + 1f >= realtimeSinceStartup)
				{
					if (entry.val != 0f)
					{
						if (entry.val < 0f && num < 0f)
						{
							entry.val += num;
							entry.label.text = Mathf.RoundToInt(entry.val).ToString();
							return;
						}
						if (entry.val > 0f && num > 0f)
						{
							entry.val += num;
							entry.label.text = "+" + Mathf.RoundToInt(entry.val);
							return;
						}
					}
				}
			}
		}
		HUDText.Entry entry2 = this.Create();
		entry2.stay = stayDuration;
		entry2.label.color = c;
		entry2.val = num;
		entry2.offsetX = offsetX;
		entry2.fontSize = ((fontSize <= 0f) ? 50f : fontSize);
		if (flag)
		{
			entry2.label.text = ((num >= 0f) ? ("+" + Mathf.RoundToInt(entry2.val)) : Mathf.RoundToInt(entry2.val).ToString());
		}
		else
		{
			entry2.label.text = obj.ToString();
		}
		if (this.MaxCount != -1 && this.mList.Count > this.MaxCount)
		{
			for (int j = 0; j < this.mList.Count - this.MaxCount; j++)
			{
				HUDText.Entry ent = this.mList[0];
				this.Delete(ent);
			}
		}
		this.mList.Sort(new Comparison<HUDText.Entry>(HUDText.Comparison));
	}

	private void OnDisable()
	{
		int i = this.mList.Count;
		while (i > 0)
		{
			HUDText.Entry entry = this.mList[--i];
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

	private void Update()
	{
		if (this.mList.Count == 0)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Keyframe[] keys = this.offsetCurve.keys;
		Keyframe[] keys2 = this.alphaCurve.keys;
		Keyframe[] keys3 = this.scaleCurve.keys;
		float time = keys[keys.Length - 1].time;
		float time2 = keys2[keys2.Length - 1].time;
		float time3 = keys3[keys3.Length - 1].time;
		float num = Mathf.Max(time3, Mathf.Max(time, time2));
		int i = this.mList.Count;
		while (i > 0)
		{
			HUDText.Entry entry = this.mList[--i];
			float num2 = realtimeSinceStartup - entry.movementStart;
			entry.offset = this.offsetCurve.Evaluate(num2);
			entry.label.alpha = this.alphaCurve.Evaluate(num2);
			float num3 = this.scaleCurve.Evaluate(realtimeSinceStartup - entry.time) * entry.fontSize;
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
			HUDText.Entry entry2 = this.mList[--j];
			num4 = Mathf.Max(num4, entry2.offset);
			entry2.label.cachedTransform.localPosition = new Vector3(entry2.offsetX, num4, 0f);
			num4 += Mathf.Round(entry2.label.cachedTransform.localScale.y);
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

	public int MaxCount = -1;

	private List<HUDText.Entry> mList = new List<HUDText.Entry>();

	private List<HUDText.Entry> mUnused = new List<HUDText.Entry>();

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

		public const float defaultFontSize = 50f;

		public float time;

		public float stay;

		public float offset;

		public float val;

		public UILabel label;

		public float offsetX;

		public float fontSize;
	}
}
