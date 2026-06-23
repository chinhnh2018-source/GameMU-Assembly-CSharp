using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDTextCustom : MonoBehaviour
{
	public static string GetAnimationName(HUDTextCustom.TextType type)
	{
		string result = "shanghaishuzi_putong";
		switch (type)
		{
		case HUDTextCustom.TextType.Normal:
			result = "shanghaishuzi_putong";
			break;
		case HUDTextCustom.TextType.Special:
			result = "shanghaishuzi_baoji";
			break;
		case HUDTextCustom.TextType.Miss:
			result = "shanghaishuzi_miss";
			break;
		case HUDTextCustom.TextType.Miss2:
			result = "shanghaishuzi_miss2";
			break;
		}
		return result;
	}

	private void Start()
	{
		this.LoadPreLoadEntrys();
	}

	private void LoadPreLoadEntrys()
	{
		for (int i = 0; i < HUDTextCustom.CacheEntryCount; i++)
		{
			HUDTextCustom.Entry entry = this.Create();
		}
		for (int j = this.mList.Count - 1; j >= 0; j--)
		{
			HUDTextCustom.Entry ent = this.mList[j];
			this.Delete(ent);
		}
	}

	private static int Comparison(HUDTextCustom.Entry a, HUDTextCustom.Entry b)
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

	private HUDTextCustom.Entry Create()
	{
		if (this.mUnused.Count > 0)
		{
			HUDTextCustom.Entry entry = this.mUnused[this.mUnused.Count - 1];
			this.mUnused.RemoveAt(this.mUnused.Count - 1);
			entry.time = Time.realtimeSinceStartup;
			entry.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
			entry.label.enabled = true;
			entry.offset = 0f;
			this.mList.Add(entry);
			return entry;
		}
		Transform transform = base.transform;
		GameObject gameObject = new GameObject("HUDLabel" + this.counter);
		gameObject.transform.parent = transform.parent;
		gameObject.transform.localPosition = transform.localPosition;
		gameObject.transform.localRotation = transform.localRotation;
		gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		gameObject.layer = base.gameObject.layer;
		HUDTextCustom.Entry entry2 = new HUDTextCustom.Entry();
		entry2.time = Time.realtimeSinceStartup;
		entry2.label = NGUITools.AddWidget<UILabel>(gameObject);
		entry2.label.name = this.counter.ToString();
		entry2.label.effectStyle = this.effect;
		entry2.label.font = this.font;
		entry2.anchor = gameObject.AddComponent<UIFollowTarget>();
		GameObject gameObject2 = new GameObject("HUDTarget");
		Object.DontDestroyOnLoad(gameObject2);
		gameObject2.layer = base.gameObject.layer;
		entry2.anchor.target = gameObject2.transform;
		Animation animation = entry2.label.gameObject.AddComponent<Animation>();
		animation.wrapMode = 1;
		if (HUDTextCustom.NormalClip == null)
		{
			HUDTextCustom.NormalClip = (AnimationClip)Resources.Load("Anim/" + HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Normal));
		}
		if (HUDTextCustom.NormalClip != null)
		{
			animation.AddClip(HUDTextCustom.NormalClip, HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Normal));
		}
		if (HUDTextCustom.SpecialClip == null)
		{
			HUDTextCustom.SpecialClip = (AnimationClip)Resources.Load("Anim/" + HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Special));
		}
		if (HUDTextCustom.SpecialClip != null)
		{
			animation.AddClip(HUDTextCustom.SpecialClip, HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Special));
		}
		if (HUDTextCustom.MissClip == null)
		{
			HUDTextCustom.MissClip = (AnimationClip)Resources.Load("Anim/" + HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Miss));
		}
		if (HUDTextCustom.MissClip != null)
		{
			animation.AddClip(HUDTextCustom.MissClip, HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Miss));
		}
		if (HUDTextCustom.Miss2Clip == null)
		{
			HUDTextCustom.Miss2Clip = (AnimationClip)Resources.Load("Anim/" + HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Miss2));
		}
		if (HUDTextCustom.Miss2Clip != null)
		{
			animation.AddClip(HUDTextCustom.Miss2Clip, HUDTextCustom.GetAnimationName(HUDTextCustom.TextType.Miss2));
		}
		entry2.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		this.mList.Add(entry2);
		this.counter++;
		return entry2;
	}

	private void Delete(HUDTextCustom.Entry ent)
	{
		this.mList.Remove(ent);
		if (this.mUnused.Count > HUDTextCustom.CacheEntryCount)
		{
			Object.Destroy(ent.anchor.target.gameObject);
			Object.Destroy(ent.anchor.gameObject);
		}
		else
		{
			this.mUnused.Add(ent);
			ent.anchor.enabled = false;
			ent.label.transform.localPosition = HUDTextCustom.disablePosition;
		}
	}

	public void Add(object obj, Vector3 worldPosition, Color c, float stayDuration, float offsetX = 0f, float fontSize = 0f, HUDTextCustom.TextType textType = HUDTextCustom.TextType.Normal)
	{
		if (!base.enabled)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float val = 0f;
		HUDTextCustom.Entry entry = this.Create();
		entry.stay = stayDuration;
		entry.label.color = c;
		entry.val = val;
		entry.anchor.target.position = worldPosition;
		entry.anchor.enabled = true;
		entry.offsetX = offsetX;
		entry.fontSize = ((fontSize <= 0f) ? 50f : fontSize);
		entry.label.text = obj.ToString();
		Animation component = entry.label.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			component.Play(HUDTextCustom.GetAnimationName(textType));
		}
	}

	private void OnDisable()
	{
		int count = this.mList.Count;
		int i = count;
		while (i > 0)
		{
			HUDTextCustom.Entry entry = this.mList[--i];
			this.mList.RemoveAt(i);
			if (entry.label != null)
			{
				GameObject gameObject = entry.label.transform.parent.gameObject;
				entry.label.transform.parent = null;
				if (gameObject != null)
				{
					UIFollowTarget component = gameObject.GetComponent<UIFollowTarget>();
					if (component != null && component.target != null)
					{
						Object.Destroy(component.target.gameObject);
					}
					Object.Destroy(gameObject);
				}
				Object.Destroy(entry.label.gameObject);
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
		int count = this.mList.Count;
		int i = count;
		while (i > 0)
		{
			HUDTextCustom.Entry entry = this.mList[--i];
			float num = realtimeSinceStartup - entry.movementStart;
			if (num > this.totalEnd)
			{
				this.Delete(entry);
			}
			else
			{
				entry.label.enabled = true;
			}
		}
	}

	public static AnimationClip NormalClip = null;

	public static AnimationClip SpecialClip = null;

	public static AnimationClip MissClip = null;

	public static AnimationClip Miss2Clip = null;

	public UIFont font;

	public UILabel.Effect effect;

	private List<HUDTextCustom.Entry> mList = new List<HUDTextCustom.Entry>();

	private List<HUDTextCustom.Entry> mUnused = new List<HUDTextCustom.Entry>();

	private static int CacheEntryCount = 40;

	private static Vector3 disablePosition = new Vector3(1000f, 1000f, 0f);

	private int counter;

	private float totalEnd = 1f;

	public enum TextType
	{
		Normal,
		Special,
		Miss,
		Miss2
	}

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

		public UIFollowTarget anchor;

		public float offsetX;

		public float fontSize;
	}
}
