using System;
using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox")]
public class UICheckbox : MonoBehaviour
{
	public bool isChecked
	{
		get
		{
			return this.mChecked;
		}
		set
		{
			if (this.radioButtonRoot == null || value || this.optionCanBeNone || !this.mStarted)
			{
				this.Set(value);
			}
		}
	}

	private void Awake()
	{
		this.mTrans = base.transform;
		if (this.checkSprite != null)
		{
			this.checkSprite.alpha = ((!this.startsChecked) ? 0f : 1f);
		}
		if (this.option)
		{
			this.option = false;
			if (this.radioButtonRoot == null)
			{
				this.radioButtonRoot = this.mTrans.parent;
			}
		}
		if (null != this._Lable)
		{
			this._TextColor = this._Lable.color;
		}
	}

	private void Start()
	{
		if (this.eventReceiver == null)
		{
			this.eventReceiver = base.gameObject;
		}
		this.mChecked = !this.startsChecked;
		this.mStarted = true;
		this.Set(this.startsChecked);
	}

	private void OnClick()
	{
		if (base.enabled)
		{
			this.isChecked = !this.isChecked;
		}
	}

	private void Set(bool state)
	{
		if (!this.mStarted)
		{
			this.mChecked = state;
			this.startsChecked = state;
			if (this.checkSprite != null)
			{
				this.checkSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else if (this.mChecked != state)
		{
			if (this.radioButtonRoot != null && state)
			{
				UICheckbox[] componentsInChildren = this.radioButtonRoot.GetComponentsInChildren<UICheckbox>(true);
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UICheckbox uicheckbox = componentsInChildren[i];
					if (uicheckbox != this && uicheckbox.radioButtonRoot == this.radioButtonRoot)
					{
						uicheckbox.Set(false);
					}
					i++;
				}
			}
			this.mChecked = state;
			if (this.checkSprite != null)
			{
				if (this.instantTween)
				{
					this.checkSprite.alpha = ((!this.mChecked) ? 0f : 1f);
				}
				else
				{
					TweenAlpha.Begin(this.checkSprite.gameObject, 0.15f, (!this.mChecked) ? 0f : 1f);
				}
			}
			UICheckbox.current = this;
			if (this.onStateChange != null)
			{
				this.onStateChange(this.mChecked);
			}
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mChecked, 1);
			}
			UICheckbox.current = null;
			if (this.checkAnimation != null)
			{
				ActiveAnimation.Play(this.checkAnimation, (!state) ? Direction.Reverse : Direction.Forward);
			}
		}
		if (null != this._Lable)
		{
			if (this._TextColor == Color.clear)
			{
				return;
			}
			if (this.IsKeepOriginalTextStyle)
			{
				this._Lable.color = this._TextColor;
			}
			else
			{
				this._Lable.color = ((!this.mChecked) ? ColorCode.GetGrayColor(this._TextColor, 0.6f) : this._TextColor);
			}
		}
	}

	public bool GrayTextOnDisable = true;

	public UILabel _Lable;

	public Color _TextColor;

	public static UICheckbox current;

	public UISprite checkSprite;

	public Animation checkAnimation;

	public bool instantTween;

	public bool startsChecked = true;

	public bool IsKeepOriginalTextStyle = true;

	public Transform radioButtonRoot;

	public bool optionCanBeNone;

	public GameObject eventReceiver;

	public string functionName = "OnActivate";

	public UICheckbox.OnStateChange onStateChange;

	[HideInInspector, SerializeField]
	private bool option;

	private bool mChecked = true;

	private bool mStarted;

	private Transform mTrans;

	public delegate void OnStateChange(bool state);
}
