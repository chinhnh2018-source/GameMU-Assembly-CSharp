using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class CTongGuanScore : MonoBehaviour
{
	public string SpriteName
	{
		set
		{
			if (null != this._Sprite)
			{
				this._Sprite.spriteName = value;
				this._Sprite.MakePixelPerfect();
			}
		}
	}

	public void Play()
	{
		base.transform.localScale = new Vector3(3f, 3f, 1f);
		this.panel.alpha = 0f;
		Vector3 vector;
		vector..ctor(2.363f, 2.363f, 1f);
		this.tweener = TweenScale.Begin(base.gameObject, this.timePerClip, vector);
		this.tweener.onFinished = new UITweener.OnFinished(this.TweenEnd);
		TweenAlpha.Begin(base.gameObject, this.timePerClip * 5f, 1f);
	}

	private void TweenEnd(UITweener tweener)
	{
		Vector3 vector;
		vector..ctor(2.009f, 2.009f, 1f);
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip * 2f, vector);
		tweener.onFinished = new UITweener.OnFinished(this.TweenEnd0);
	}

	private void TweenEnd0(UITweener tweener)
	{
		Vector3 vector;
		vector..ctor(1f, 1f, 1f);
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip, vector);
		tweener.onFinished = new UITweener.OnFinished(this.TweenEnd2);
	}

	private void TweenEnd1(UITweener tweener)
	{
		Vector3 vector;
		vector..ctor(1f, 1f, 1f);
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip, vector);
		tweener.onFinished = new UITweener.OnFinished(this.TweenEnd2);
		this._FlashAnim.Reset();
	}

	private void TweenEnd2(UITweener tweener)
	{
		Vector3 vector;
		vector..ctor(1.2f, 1.2f, 1f);
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip, vector);
		tweener.onFinished = new UITweener.OnFinished(this.TweenEnd3);
	}

	private void TweenEnd3(UITweener tweener)
	{
		Vector3 vector;
		vector..ctor(0.8f, 0.8f, 1f);
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip, vector);
		tweener.onFinished = new UITweener.OnFinished(this.TweenEnd4);
	}

	private void TweenEnd4(UITweener tweener)
	{
		tweener = TweenScale.Begin(base.gameObject, this.timePerClip, Vector3.one);
		tweener.onFinished = null;
	}

	public void SetSprite(string atlas, string name)
	{
		if (null != this._Sprite)
		{
			this._Sprite.atlas = U3DUtils.LoadAtlas(atlas);
			this._Sprite.spriteName = name;
			this._Sprite.MakePixelPerfect();
		}
	}

	public float timePerClip = 0.15f;

	public UITweener tweener;

	public UIPanel panel;

	public UISprite _Sprite;

	public CAnimation _FlashAnim;
}
