using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class CAnimation : UserControl
{
	public bool Loop
	{
		get
		{
			return null != this._Anim && this._Anim.loop;
		}
		set
		{
			if (null != this._Anim)
			{
				this._Anim.loop = value;
			}
		}
	}

	public int FrameRate
	{
		set
		{
			if (null != this._Anim)
			{
				this._Anim.framesPerSecond = value;
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (this.AutoUnload)
		{
			this.ReloadPrefab();
		}
	}

	private void ReloadPrefab()
	{
		if (this.AnimationType == CAnimation.CAnimationTypes.SpriteAnimation)
		{
			if (null == this.Prefab)
			{
				this.Prefab = base.transform;
			}
			if (null == this._Anim)
			{
				this._Anim = base.GetComponentInChildren<UISpriteAnimation>();
			}
			if (null == this._Sprite)
			{
				this._Sprite = base.GetComponentInChildren<UISprite>();
			}
			if (string.IsNullOrEmpty(this.AtlasName) && this._Sprite != null)
			{
				this.AtlasName = this._Sprite.atlas.name;
			}
			if (null == this._Sprite.atlas && !string.IsNullOrEmpty(this.AtlasName))
			{
				this.SetSprite(this.AtlasName, null);
			}
			if (!string.IsNullOrEmpty(this.AtlasName))
			{
				base.InvokeRepeating("TickProc", 0f, 10f);
			}
		}
		else if (this.AnimationType == CAnimation.CAnimationTypes.UnityAnimation)
		{
			if (null == this.Prefab)
			{
				this.Prefab = U3DUtils.NEW<Transform>(this.AtlasName);
				if (null != this.Prefab)
				{
					U3DUtils.AddChild(base.gameObject, this.Prefab.gameObject, false);
				}
			}
			this._Animations = base.GetComponentsInChildren<Animation>();
		}
		else if (this.AnimationType == CAnimation.CAnimationTypes.UnityAnimator)
		{
			if (null == this.Prefab)
			{
				this.Prefab = U3DUtils.NEW<Transform>(this.AtlasName);
				if (null != this.Prefab)
				{
					U3DUtils.AddChild(base.gameObject, this.Prefab.gameObject, false);
				}
			}
			this._Animators = base.GetComponentsInChildren<Animator>();
		}
	}

	private void UnloadPrefab()
	{
		if (this.AnimationType == CAnimation.CAnimationTypes.SpriteAnimation)
		{
			this.Prefab = null;
			if (null != this._Sprite.atlas)
			{
				this._Sprite.atlas = null;
			}
		}
		else if (this.AnimationType == CAnimation.CAnimationTypes.UnityAnimation)
		{
			if (null != this.Prefab)
			{
				Object.Destroy(this.Prefab.gameObject);
			}
			this.Prefab = null;
			this._Animations = null;
		}
		else if (this.AnimationType == CAnimation.CAnimationTypes.UnityAnimator)
		{
			if (null != this.Prefab)
			{
				Object.Destroy(this.Prefab.gameObject);
			}
			this.Prefab = null;
			this._Animators = null;
		}
		else if (this.TimeOutHandler != null)
		{
			this.TimeOutHandler.Invoke(this, EventArgs.Empty);
		}
	}

	protected virtual void OnEnable()
	{
		if (this.AutoUnload && !string.IsNullOrEmpty(this.AtlasName))
		{
			this.TickInterval = 0f;
			if (!base.IsInvoking("TickProc"))
			{
				base.InvokeRepeating("TickProc", 0f, 10f);
			}
			if (null == this.Prefab)
			{
				this.ReloadPrefab();
			}
		}
	}

	private void TickProc()
	{
		if (this.AutoUnload && !base.gameObject.activeInHierarchy)
		{
			if (this.TickInterval >= (float)this.KeepIfHide)
			{
				this.TickInterval = 0f;
				this.UnloadPrefab();
			}
			else
			{
				this.TickInterval += 10f;
			}
		}
	}

	private void CalculateSize()
	{
		if (null != this._Sprite)
		{
			Vector2 zero = Vector2.zero;
			foreach (UIAtlas.Sprite sprite in this._Sprite.atlas.spriteList)
			{
				if (zero.x < sprite.outer.width)
				{
					zero.x = sprite.outer.width;
				}
				if (zero.y < sprite.outer.height)
				{
					zero.y = sprite.outer.height;
				}
			}
		}
	}

	public void Reset()
	{
		if (null != this._Anim)
		{
			this._Anim.Reset();
		}
	}

	public void SetSprite(string atlas, string name = null)
	{
		if (null != this._Sprite)
		{
			string path;
			if (atlas.StartsWith("Prefabs/"))
			{
				path = atlas;
			}
			else
			{
				path = Global.GetPrefabString(atlas, true);
			}
			this._Sprite.atlas = U3DUtils.LoadAtlas(path);
			this._Sprite.spriteName = name;
		}
	}

	public static CAnimation Attach(GameObject go, CAnimation old, string name)
	{
		if (null != old)
		{
			Object.Destroy(old.gameObject);
		}
		CAnimation canimation = U3DUtils.NEW<CAnimation>(name);
		if (canimation)
		{
			U3DUtils.AddChild(go, canimation.gameObject, true);
		}
		return canimation;
	}

	public void SetAutoHide(float time)
	{
		base.StartCoroutine_Auto(this.AutoHide(time));
	}

	private IEnumerator AutoHide(float time)
	{
		yield return new WaitForSeconds(time);
		base.gameObject.SetActive(false);
		yield break;
	}

	public UISpriteAnimation _Anim;

	public UISprite _Sprite;

	public Animation[] _Animations;

	public Animator[] _Animators;

	public Transform Prefab;

	public EventHandler TimeOutHandler;

	public CAnimation.CAnimationTypes AnimationType;

	public bool AutoUnload;

	public string AtlasName;

	public int KeepIfHide = 15;

	private float TickInterval;

	public enum CAnimationTypes
	{
		None,
		SpriteAnimation,
		UnityAnimation,
		UnityAnimator,
		Other
	}
}
