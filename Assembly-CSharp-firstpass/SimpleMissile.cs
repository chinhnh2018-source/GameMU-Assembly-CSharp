using System;
using UnityEngine;

public class SimpleMissile : MonoBehaviour
{
	private void Start()
	{
		this.Xffect = base.transform.GetComponent<XffectComponent>();
		this.Reset();
	}

	private void Update()
	{
		this.ElapsedTime += Time.deltaTime;
		if (this.ElapsedTime > this.Life)
		{
			this.ElapsedTime = 0f;
			this.Xffect.DeActive();
			if (this.ExplodeXftName != string.Empty)
			{
				this.EffectCache.ReleaseEffect(this.ExplodeXftName, base.transform.position);
			}
		}
		this.Velocity += this.Accelaration * Time.deltaTime;
		base.transform.position -= base.transform.forward * this.Velocity * Time.deltaTime;
	}

	public void Reset()
	{
		this.Velocity = this.OriVelocity;
		this.ElapsedTime = 0f;
	}

	public float Life = 3f;

	public float OriVelocity = 50f;

	public float Accelaration = 10f;

	public string ExplodeXftName = string.Empty;

	public XffectCache EffectCache;

	protected float ElapsedTime;

	protected XffectComponent Xffect;

	protected float Velocity;
}
