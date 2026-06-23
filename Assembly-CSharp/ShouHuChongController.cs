using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShouHuChongController : MonoBehaviour
{
	public Transform Target
	{
		get
		{
			return this._Target;
		}
		set
		{
			if (value != this._Target)
			{
				this._Target = value;
				if (this.GameObjectInstance != null && this.UseType != 1)
				{
					this.GameObjectInstance.transform.localRotation = this._Target.localRotation;
					PetFollow petFollow = this.GameObjectInstance.AddComponent<PetFollow>();
					petFollow.target = this._Target;
					this.GameObjectInstance.transform.localPosition = this._Target.localPosition;
				}
			}
		}
	}

	private void OnEnable()
	{
		this.Action = GPetActions.Stand;
	}

	public void InitController(GameObject go, Transform target)
	{
		this.GameObjectInstance = go;
		if (this.GameObjectInstance != null)
		{
			this.AnimationInstance = this.GameObjectInstance.GetComponent<Animation>();
			if (null != this.AnimationInstance)
			{
				this.AnimationInstance.wrapMode = 2;
				this.AnimationInstance.CrossFade("Stand");
				this.StandLength = this.AnimationInstance["Stand"].length;
				this.IdleLength = this.AnimationInstance["Relax"].length;
				this.RunLength = this.AnimationInstance["Walk"].length;
			}
			this.Target = target;
		}
	}

	public GPetActions Action
	{
		get
		{
			return this._Action;
		}
		set
		{
			this.RemainTime = 0f;
			if (this._Action != value)
			{
				GPetActions action = this._Action;
				this._LastAction = action;
				this._Action = value;
				if (this.AnimationInstance != null)
				{
					if (this._Action == GPetActions.Stand)
					{
						this.AnimationInstance.wrapMode = 2;
						this.AnimationInstance.CrossFade("Stand", 0.5f);
					}
					else if (this._Action == GPetActions.Idle)
					{
						this.AnimationInstance.wrapMode = 1;
						this.AnimationInstance.CrossFade("Relax", 0.5f);
					}
					else if (this._Action == GPetActions.Walk)
					{
						this.AnimationInstance.wrapMode = 2;
						this.AnimationInstance.CrossFade("Walk", 0.5f);
					}
				}
			}
		}
	}

	private void Update()
	{
		if (this.GameObjectInstance == null)
		{
			return;
		}
		if (this._Action == GPetActions.Stand)
		{
			this.RemainTime += Time.deltaTime;
			if (this.StandLength != 0f && this.RemainTime / this.StandLength >= 5f)
			{
				this.Action = GPetActions.Idle;
				this.RemainTime = 0f;
			}
		}
		else if (this._Action == GPetActions.Idle)
		{
			this.RemainTime += Time.deltaTime;
			if (this.IdleLength != 0f && this.RemainTime / this.IdleLength >= 1f)
			{
				this.Action = GPetActions.Stand;
				this.RemainTime = 0f;
			}
		}
		else if (this._Action == GPetActions.Walk)
		{
		}
	}

	public void Dispose()
	{
		if (this.GameObjectInstance != null)
		{
			Object.Destroy(this.GameObjectInstance);
			this.GameObjectInstance = null;
		}
	}

	private void OnDestroy()
	{
		this.Dispose();
	}

	private Animation AnimationInstance;

	private float StandLength;

	private float IdleLength;

	private float RunLength;

	private float RemainTime;

	private GPetActions _Action;

	private GPetActions _LastAction;

	public GameObject GameObjectInstance;

	public GameObject GameJueXing;

	private Transform _Target;

	public int UseType;

	public string LoaderURL = string.Empty;

	public ItemCategories Categoriy = ItemCategories.ChongWu;
}
