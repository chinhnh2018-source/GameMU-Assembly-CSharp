using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class GButton : UIImageButton
{
	public bool IsPressed { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.Btn_Click);
		UIEventListener.Get(base.gameObject).onPress = new UIEventListener.BoolDelegate(this.Btn_Press);
		if (this.Label != null)
		{
		}
	}

	private void Btn_Press(GameObject go, bool state)
	{
		if (this.OnPress != null)
		{
			try
			{
				this.IsPressed = true;
				this.OnPress.Invoke(go, state);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}
		else
		{
			this.IsPressed = false;
		}
	}

	private void Btn_Click(GameObject go)
	{
		try
		{
			if (this.MouseLeftButtonUp != null)
			{
				this.MouseLeftButtonUp(go, new MouseEvent("mouseUp", null));
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public string Text
	{
		get
		{
			if (this.Label == null)
			{
				return string.Empty;
			}
			return this.Label.text;
		}
		set
		{
			if (this.Label != null)
			{
				this.Label.text = value;
			}
			else if (base.GetComponentInChildren<UILabel>())
			{
				base.GetComponentInChildren<UILabel>().text = value;
			}
		}
	}

	public bool EnableHint
	{
		set
		{
		}
	}

	public bool Pressed
	{
		get
		{
			return null != this.OldNormalSprite;
		}
		set
		{
			if (value && string.IsNullOrEmpty(this.OldNormalSprite))
			{
				this.OldNormalSprite = this.normalSprite;
				this.normalSprite = this.pressedSprite;
				base.Refresh();
			}
			else if (!value && !string.IsNullOrEmpty(this.OldNormalSprite))
			{
				this.normalSprite = this.OldNormalSprite;
				this.OldNormalSprite = null;
				base.Refresh();
			}
		}
	}

	public Color TextColor
	{
		get
		{
			if (null != this.Label)
			{
				return this.Label.color;
			}
			return Color.black;
		}
		set
		{
			if (null != this.Label)
			{
				this.Label.color = value;
			}
		}
	}

	public bool isEnabled
	{
		get
		{
			return base.isEnabled;
		}
		set
		{
			base.isEnabled = value;
			if (this.OldTextColor.grayscale == 0f)
			{
				this.OldTextColor = this.TextColor;
			}
			if (value)
			{
				this.TextColor = this.OldTextColor;
			}
			else
			{
				this.TextColor = ColorCode.GetGrayColor(this.OldTextColor, 0.5f);
			}
		}
	}

	public void SetSpriteAnimationVisible(bool state)
	{
		if (null == this.spriteAnimation)
		{
			return;
		}
		if (state)
		{
			this.spriteAnimation.Reset();
		}
		else
		{
			this.spriteAnimation.Stop();
		}
		NGUITools.SetActive(this.spriteAnimation.gameObject, state);
	}

	public UILabel Label;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUp;

	public UIEventListener.BoolDelegate OnPress;

	public UISpriteAnimation spriteAnimation;

	public CAnimation Anim;

	public string BtnTag;

	public int TagIndex;

	public GameObject BtnState;

	private string OldNormalSprite;

	private Color OldTextColor;
}
