using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSmashSkill : ManualUpdateBehaviour
{
	private void Start()
	{
		this.EffectMgr = base.GetComponent<EffectManager>();
	}

	public override void ManualUpdate()
	{
		if (this.play)
		{
			this.play = false;
			this.DragonSmash();
		}
	}

	private void DragonSmash()
	{
		if (this.isPlaySkill)
		{
			return;
		}
		this.isPlaySkill = true;
		this.startTimeStamp = Time.time;
		this.dragonHeadMaterial = new Material(this.dragonPrefab.GetComponent<Renderer>().sharedMaterial);
		this.dragonTailMaterial = new Material(this.dragonPrefab.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial);
		float num = 0f;
		if (this.dragonCount != 0)
		{
			num = 360f / (float)this.dragonCount;
		}
		float y = base.transform.eulerAngles.y;
		for (int i = 0; i < this.dragonCount; i++)
		{
			GameObject gameObject = Object.Instantiate(this.dragonPrefab, base.transform.position, Quaternion.Euler(0f, y + num * (float)i, 0f)) as GameObject;
			EffectManager effectManager = gameObject.AddComponent<EffectManager>();
			effectManager.OwnerName = this.EffectMgr.OwnerName;
			effectManager.TriggerType = this.EffectMgr.TriggerType;
			foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
			{
				if (renderer.transform == gameObject.transform)
				{
					renderer.material = this.dragonHeadMaterial;
				}
				else
				{
					renderer.material = this.dragonTailMaterial;
				}
			}
			FlyDragon component = gameObject.GetComponent<FlyDragon>();
			component.range = this.skillRange;
			this.dragonInstances.Add(gameObject);
		}
		base.StartCoroutine(this.FadeOut());
	}

	private IEnumerator FadeOut()
	{
		Color headTintColor = this.dragonHeadMaterial.GetColor("_MainColor");
		float headInitialAlpha = headTintColor.a;
		Color tailTintColor = this.dragonTailMaterial.GetColor("_MainColor");
		float tailInitialAlpha = tailTintColor.a;
		while (this.isPlaySkill)
		{
			float duration = Time.time - this.startTimeStamp;
			if (duration > this.lifeTime)
			{
				break;
			}
			if (duration > this.lifeTime - this.fadeoutLength)
			{
				if (this.fadeoutLength != 0f)
				{
					float lerp = 1f - (this.lifeTime - duration) / this.fadeoutLength;
					headTintColor.a = Mathf.Lerp(headInitialAlpha, 0f, lerp);
					tailTintColor.a = Mathf.Lerp(tailInitialAlpha, 0f, lerp);
				}
				this.dragonHeadMaterial.SetColor("_MainColor", headTintColor);
				this.dragonTailMaterial.SetColor("_MainColor", tailTintColor);
			}
			yield return new WaitForSeconds(0.1f);
		}
		this.isPlaySkill = false;
		for (int i = 0; i < this.dragonInstances.Count; i++)
		{
			Object.Destroy(this.dragonInstances[i]);
		}
		this.dragonInstances.Clear();
		yield break;
	}

	public GameObject dragonPrefab;

	public int dragonCount = 6;

	public bool play;

	public float skillRange;

	public float lifeTime;

	public float fadeoutLength;

	private float startTimeStamp;

	private bool isPlaySkill;

	private Material dragonHeadMaterial;

	private Material dragonTailMaterial;

	private List<GameObject> dragonInstances = new List<GameObject>();

	private EffectManager EffectMgr;
}
