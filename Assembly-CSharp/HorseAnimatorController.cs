using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class HorseAnimatorController : MonoBehaviour
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
			}
		}
	}

	private void Start()
	{
		this.InitController(base.gameObject);
	}

	private void OnEnable()
	{
		this.Action = GHorseEXActions.Idle;
	}

	public void InitController(GameObject go)
	{
		this.GameObjectInstance = go;
		if (this.GameObjectInstance != null)
		{
			this.AnimatorInstance = this.GameObjectInstance.GetComponent<Animator>();
			if (null != this.AnimatorInstance)
			{
				Global.PlayAnimatorClip(this.AnimatorInstance, "Relax");
				this.StandLength = Global.GetClipLength(this.AnimatorInstance, "Stand");
				this.IdleLength = Global.GetClipLength(this.AnimatorInstance, "Relax");
				this.RunLength = Global.GetClipLength(this.AnimatorInstance, "Walk");
			}
		}
	}

	public GHorseEXActions Action
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
				GHorseEXActions action = this._Action;
				this._LastAction = action;
				this._Action = value;
				if (this.AnimatorInstance != null)
				{
					if (this._Action == GHorseEXActions.Stand)
					{
						Global.PlayAnimatorClip(this.AnimatorInstance, "Stand");
					}
					else if (this._Action == GHorseEXActions.Idle)
					{
						Global.PlayAnimatorClip(this.AnimatorInstance, "Relax");
					}
					else if (this._Action == GHorseEXActions.Walk)
					{
						Global.PlayAnimatorClip(this.AnimatorInstance, "Walk");
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
		if (this._Action == GHorseEXActions.Stand)
		{
			this.RemainTime += Time.deltaTime;
			if (this.StandLength != 0f && this.RemainTime / (this.StandLength * (1f - this.CrossFadeTime)) >= 5f)
			{
				this.Action = GHorseEXActions.Idle;
				this.RemainTime = 0f;
			}
		}
		else if (this._Action == GHorseEXActions.Idle)
		{
			this.RemainTime += Time.deltaTime;
			if (this.IdleLength != 0f && this.RemainTime / (this.IdleLength * (1f - this.CrossFadeTime)) >= 1f)
			{
				this.Action = GHorseEXActions.Stand;
				this.RemainTime = 0f;
			}
		}
		else if (this._Action == GHorseEXActions.Walk)
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

	private Animator AnimatorInstance;

	private float StandLength;

	private float IdleLength;

	private float RunLength;

	private float RemainTime;

	private GHorseEXActions _Action;

	private GHorseEXActions _LastAction;

	public float CrossFadeTime = 0.15f;

	public GameObject GameObjectInstance;

	private Transform _Target;

	public int UseType;

	public string LoaderURL = string.Empty;
}
