using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class GIcon : GTipSprite
{
	public GIcon(IconTypes type, int shadow = -1) : this((int)type, shadow)
	{
	}

	public GIcon(int type = 0, int shadow = -1)
	{
	}

	public bool TextShadow { get; set; }

	public new double Height
	{
		get
		{
			return (double)this.Texture.transform.localScale.y;
		}
		set
		{
			this.Texture.transform.localScale = new Vector2(this.Texture.transform.localScale.x, (float)value);
		}
	}

	public new double Width
	{
		get
		{
			return (double)this.Texture.transform.localScale.x;
		}
		set
		{
			this.Texture.transform.localScale = new Vector2((float)value, this.Texture.transform.localScale.y);
		}
	}

	public float Progress
	{
		get
		{
			return this.__Progress.fillAmount;
		}
		set
		{
			this.__Progress.fillAmount = Mathf.Clamp01(value);
		}
	}

	public IconTypes IconType
	{
		get
		{
			return this._IconType;
		}
		set
		{
			this._IconType = value;
			if (this._IconType == IconTypes.HitModes)
			{
				base.GetComponent<UIButtonOffset>().enabled = true;
			}
			else
			{
				base.GetComponent<UIButtonOffset>().enabled = false;
			}
			if (this._IconType == IconTypes.Transform)
			{
				base.GetComponent<UIButtonScale>().enabled = true;
			}
			else
			{
				base.GetComponent<UIButtonScale>().enabled = false;
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.InitIcon((int)this.IconType, 0);
	}

	private IEnumerator LoadImage(string url, ImageBrush imageBrush)
	{
		WWW www = new WWW(PathUtils.WebPath(url));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			yield break;
		}
		imageBrush.ImageSource.TextureData = www.textureNonReadable;
		www.Dispose();
		yield break;
	}

	private IEnumerable TickProc(bool init = false)
	{
		for (;;)
		{
			if (this._Changed)
			{
				yield return this.LoadImage(null, null);
			}
			else
			{
				yield return new WaitForFixedUpdate();
			}
		}
		yield break;
	}

	protected void InitIcon(int type, int shadow)
	{
		GTipService.HookTip(this, this);
		this.TextShadow = (shadow > 0);
		this.IconType = (IconTypes)type;
		if (this.IconType == IconTypes.Highlights)
		{
			this.HilightMask = null;
		}
		base.addEventListener("ROLL_OVER", new MouseEventHandler(this.mouseEventRollOver));
		base.addEventListener("ROLL_OUT", new MouseEventHandler(this.mouseEventRollOut));
		base.addEventListener("mouseDown", new MouseEventHandler(this.mouseEventLeftButtonDown));
		base.addEventListener("mouseUp", new MouseEventHandler(this.mouseEventLeftButtonUp));
		base.addEventListener("mouseMove", new MouseEventHandler(this.mouseEventLeftButtonMove));
	}

	public double LastMouseDownTicks
	{
		get
		{
			return this._LastMouseDownTicks;
		}
		set
		{
			this._LastMouseDownTicks = value;
		}
	}

	private void mouseEventLeftButtonMove(MouseEvent e)
	{
		if (ObjectClickGetingMgr.IsInClickGetThing())
		{
			return;
		}
		if (GoodsDragingMgr.IsGoodsMoving())
		{
			return;
		}
	}

	private void mouseEventRollOver(MouseEvent evt)
	{
		if (this.IconType == IconTypes.Transform)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			if (this._NewSource != null)
			{
				this.Texture.mainTexture = this._NewSource;
			}
			this.MouseEnterState = true;
			if (!this.DisableHandCursor && Global.Data.GameCursorImageID < 100)
			{
				base.buttonMode = true;
			}
		}
		else if (this.IconType == IconTypes.Highlights)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			if (!this._Hit && this._NewSource != null)
			{
				this.HilightMask.bitmapData = this._NewSource.ImageSource;
				if (this._BodySource != null)
				{
				}
			}
			this.MouseEnterState = true;
			if (!this.DisableHandCursor && Global.Data.GameCursorImageID < 100)
			{
				base.buttonMode = true;
			}
		}
		else if (this.IconType == IconTypes.HitModes)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			this.Texture.mainTexture = ((!this.Hit) ? this._NewSource : this.HitNewBodySource);
			this.MouseEnterState = true;
			if (!this.DisableHandCursor && Global.Data.GameCursorImageID < 100)
			{
				base.buttonMode = true;
			}
		}
		else if (this.IconType != IconTypes.RepeatButton)
		{
			if (this.IconType != IconTypes.BodyShadow)
			{
				if (this.IconType == IconTypes.ThreeState)
				{
					if (!this.EnableIcon)
					{
						return;
					}
					this.Texture.mainTexture = this._NewSource;
					this.MouseEnterState = true;
					if (!this.DisableHandCursor && Global.Data.GameCursorImageID < 100)
					{
						base.buttonMode = true;
					}
				}
				else if (this.IconType == IconTypes.Composite)
				{
					if (!this.EnableIcon)
					{
						return;
					}
					if (!this.DisableHandCursor && Global.Data.GameCursorImageID < 100)
					{
						base.buttonMode = true;
					}
				}
			}
		}
	}

	private void mouseEventRollOut(MouseEvent evt)
	{
		bool buttonPressed = this.ButtonPressed;
		this.ButtonPressed = false;
		if (this.IconType == IconTypes.Transform)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			this.Texture.mainTexture = this._BodySource;
			this.MouseEnterState = false;
			if (!this.DisableHandCursor)
			{
				base.buttonMode = false;
			}
		}
		else if (this.IconType == IconTypes.Highlights)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			if (!this._Hit)
			{
				this.HilightMask.bitmapData = null;
			}
			this.MouseEnterState = false;
			if (!this.DisableHandCursor)
			{
				base.buttonMode = false;
			}
		}
		else if (this.IconType == IconTypes.HitModes)
		{
			if (!this.EnableIcon)
			{
				return;
			}
			this.Texture.mainTexture = ((!this.Hit) ? this._BodySource : this.HitBodySource);
			this.MouseEnterState = false;
			if (!this.DisableHandCursor)
			{
				base.buttonMode = false;
			}
		}
		else if (this.IconType != IconTypes.RepeatButton)
		{
			if (this.IconType != IconTypes.BodyShadow)
			{
				if (this.IconType == IconTypes.ThreeState)
				{
					if (!this.EnableIcon)
					{
						return;
					}
					this.Texture.mainTexture = this._BodySource;
					this.MouseEnterState = false;
					if (!this.DisableHandCursor)
					{
						base.buttonMode = false;
					}
				}
				else if (this.IconType == IconTypes.Composite)
				{
					if (!this.EnableIcon)
					{
						return;
					}
					if (!this.DisableHandCursor)
					{
						base.buttonMode = false;
					}
				}
			}
		}
		if (buttonPressed)
		{
			this.startDragIcon(evt);
		}
	}

	private void startDragIcon(MouseEvent evt)
	{
		if (FixedListBoxDragDropTarget.Draging)
		{
			return;
		}
	}

	private void mouseEventLeftButtonDown(MouseEvent evt)
	{
		this.ButtonPressed = true;
		if (this.IconType == IconTypes.Transform)
		{
			if (this.MouseLeftButtonDown != null)
			{
				this.MouseLeftButtonDown(this, evt);
			}
		}
		else if (this.IconType != IconTypes.Highlights)
		{
			if (this.IconType == IconTypes.HitModes)
			{
				if (!this.EnableIcon)
				{
					return;
				}
				this.Hit = !this.Hit;
			}
			else if (this.IconType != IconTypes.RepeatButton)
			{
				if (this.IconType != IconTypes.BodyShadow)
				{
					if (this.IconType == IconTypes.ThreeState)
					{
						if (!this.EnableIcon)
						{
							return;
						}
						this.Texture.mainTexture = this.HitBodySource;
					}
					else if (this.IconType == IconTypes.Composite && this.MouseLeftButtonDown != null)
					{
						this.MouseLeftButtonDown(this, evt);
					}
				}
			}
		}
	}

	private void mouseEventLeftButtonUp(MouseEvent evt)
	{
		if (FixedListBoxDragDropTarget.Draging)
		{
			return;
		}
		this.ClearHintDecoration();
		if (this.IconType == IconTypes.Transform)
		{
			if (this.ButtonPressed && this.MouseLeftButtonUp != null)
			{
				this.MouseLeftButtonUp(this, evt);
			}
		}
		else if (this.IconType == IconTypes.Highlights)
		{
			if (this.ButtonPressed && this.MouseLeftButtonUp != null)
			{
				this.MouseLeftButtonUp(this, evt);
			}
		}
		else if (this.IconType == IconTypes.HitModes)
		{
			if (this.ButtonPressed && this.MouseLeftButtonUp != null)
			{
				this.MouseLeftButtonUp(this, evt);
			}
		}
		else if (this.IconType != IconTypes.RepeatButton)
		{
			if (this.IconType != IconTypes.BodyShadow)
			{
				if (this.IconType == IconTypes.ThreeState)
				{
					if (!this.EnableIcon)
					{
						return;
					}
					this.Texture.mainTexture = this._NewSource;
					if (this.ButtonPressed && this.MouseLeftButtonUp != null)
					{
						this.MouseLeftButtonUp(this, evt);
					}
				}
				else if (this.IconType == IconTypes.Composite && this.ButtonPressed && this.MouseLeftButtonUp != null)
				{
					this.MouseLeftButtonUp(this, evt);
				}
			}
		}
		this.ButtonPressed = false;
	}

	private void mouseEventDblClick(MouseEvent evt)
	{
		if (!this.EnableIcon)
		{
			return;
		}
		if (!this._DisableMovingEnd && GoodsMovingMgr.ProcessGoodsMovingEnd(evt))
		{
			return;
		}
		this.ClearHintDecoration();
		if (this.MouseLeftButtonDoubleClick != null)
		{
			this.MouseLeftButtonDoubleClick(this, evt);
		}
	}

	public GIcon Clone()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = this.Width;
		gicon.Height = this.Height;
		gicon.BodySource = this.BodySource;
		gicon.NewSource = this.NewSource;
		gicon.HitBodySource = this.HitBodySource;
		gicon.HitNewBodySource = this.HitBodySource;
		gicon.DisableBodySource = this.HitBodySource;
		gicon.EnableIcon = this.EnableIcon;
		gicon.Tip = base.Tip;
		gicon.TipType = base.TipType;
		gicon.ItemCategory = this.ItemCategory;
		gicon.Tag = this.Tag;
		gicon.ItemCode = this.ItemCode;
		gicon.ItemObject = this.ItemObject;
		gicon.BoxTypes = this.BoxTypes;
		gicon.Text = this.Text;
		gicon.TextHorizontalAlignment = this.TextHorizontalAlignment;
		gicon.TextVerticalAlignment = this.TextVerticalAlignment;
		gicon.TextShadowColor = this.TextShadowColor;
		gicon.TextColor = this.TextColor;
		return gicon;
	}

	public ImageBrush BodySource
	{
		get
		{
			return this._BodySource;
		}
		set
		{
			this._BodySource = value;
			this.Texture.mainTexture = this._BodySource;
			this.RepositionContentText();
		}
	}

	public ImageURL BodyURL
	{
		get
		{
			return this._BodyURL;
		}
		set
		{
			this._BodyURL = value;
			if (value != null)
			{
				if (value.ImageType == 0)
				{
					GoodsImageMgr.GetGoods32x32ImageFromURL(value.ImageSource, this);
				}
				else if (value.ImageType == 1)
				{
					SkillImageMgr.GetSkill32x32ImageFromURL(value.ImageSource, this);
				}
				else if (value.ImageType == 2)
				{
					GoodsImageMgr.GetSales48x48ImageFromURL(value.ImageSource, this);
				}
			}
			else
			{
				this.BodySource = null;
			}
		}
	}

	public ImageBrush NewSource
	{
		get
		{
			return this._NewSource;
		}
		set
		{
			this._NewSource = value;
		}
	}

	public Brush BodyBackground
	{
		get
		{
			return base.Background;
		}
		set
		{
			base.Background = value;
		}
	}

	public bool Hit
	{
		get
		{
			return this._Hit;
		}
		set
		{
			this._Hit = value;
			if (this.IconType == IconTypes.HitModes)
			{
				this.Texture.mainTexture = ((!this.Hit) ? this._BodySource : this.HitBodySource);
			}
			else if (this.IconType == IconTypes.Highlights)
			{
				if (this._Hit)
				{
					this.HilightMask.bitmapData = this._NewSource.ImageSource;
				}
				else
				{
					this.HilightMask.bitmapData = null;
				}
			}
		}
	}

	public int Delay
	{
		get
		{
			return this._Delay;
		}
		set
		{
			this._Delay = value;
		}
	}

	public int Interval
	{
		get
		{
			return this._Interval;
		}
		set
		{
			this._Interval = value;
		}
	}

	public ImageBrush HitBodySource
	{
		get
		{
			return new ImageBrush(this._HitBodySource);
		}
		set
		{
			this._HitBodySource = value.ImageSource;
		}
	}

	public ImageBrush HitNewBodySource
	{
		get
		{
			return new ImageBrush(this._HitNewBodySource);
		}
		set
		{
			this._HitNewBodySource = value.ImageSource;
		}
	}

	public void RepositonStatePanel()
	{
		this.StatePanel.X = 1.0;
		this.StatePanel.Y = Math.Floor(this._BodySource.ImageSource.height - this.StatePanel.Height) - 1.0;
	}

	public void RepositionContentText()
	{
		if (this._BodySource != null && this._BodySource.ImageSource != null)
		{
			if (!string.IsNullOrEmpty(this.ContentText.text))
			{
				if (this._TextHorizontalAlignment == global::Layout.Left)
				{
					this.ContentText.X = -Math.Floor(this._BodySource.ImageSource.width / 2.0);
				}
				else if (this._TextHorizontalAlignment == global::Layout.Center)
				{
					this.ContentText.X = 0.0;
				}
				else
				{
					this.ContentText.X = Math.Floor(this._BodySource.ImageSource.width / 2.0);
				}
				if (this._TextVerticalAlignment == global::Layout.Top)
				{
					this.ContentText.Y = this._BodySource.ImageSource.height / 2.0;
				}
				else if (this._TextVerticalAlignment == global::Layout.Center)
				{
					this.ContentText.Y = 0.0;
				}
				else if (this._TextVerticalAlignment == global::Layout.Bottom)
				{
					this.ContentText.Y = Math.Floor(-this._BodySource.ImageSource.height / 2.0);
				}
			}
			this.__Progress.transform.localScale = new Vector3((float)this._BodySource.ImageSource.width, (float)this._BodySource.ImageSource.height, 0f);
			this._Composite2BodyRectangle.transform.localScale = new Vector3((float)this._BodySource.ImageSource.width, (float)this._BodySource.ImageSource.height, 0f);
			this.Background.transform.localScale = new Vector3((float)this._BodySource.ImageSource.width, (float)this._BodySource.ImageSource.height, 0f);
			this.HilightMask.transform.localScale = new Vector3((float)this._BodySource.ImageSource.width, (float)this._BodySource.ImageSource.height, 0f);
			this.mBoxCollider.size = this.__Progress.transform.localScale;
			float num = (float)this._BodySource.ImageSource.height;
			this.ContentText.FontSize = (int)(num / 6f);
			this.SecondText.Y = this._BodySource.ImageSource.height / 2.0;
			this.SecondText.FontSize = (int)(num / 5f);
		}
	}

	public string Text
	{
		get
		{
			return this.ContentText.text;
		}
		set
		{
			this.ContentText.text = value;
			this.RepositionContentText();
		}
	}

	public uint TextShadowColor
	{
		get
		{
			return this.ContentText.borderColor;
		}
		set
		{
			this.ContentText.borderColor = value;
		}
	}

	public SolidColorBrush TextColor
	{
		get
		{
			return new SolidColorBrush(this.ContentText.textColor);
		}
		set
		{
			this.ContentText.textColor = value.Color;
			this._NormalTextColor = value.Color;
		}
	}

	public SolidColorBrush DisableTextColor
	{
		set
		{
			this._DisableTextColor = value.Color;
		}
	}

	public double TextSize
	{
		set
		{
		}
	}

	public string TextHorizontalAlignment
	{
		get
		{
			return this._TextHorizontalAlignment;
		}
		set
		{
			this._TextHorizontalAlignment = value;
			this.RepositionContentText();
		}
	}

	public string TextVerticalAlignment
	{
		get
		{
			return this._TextVerticalAlignment;
		}
		set
		{
			this._TextVerticalAlignment = value;
			this.RepositionContentText();
		}
	}

	public Thickness TextMargin
	{
		set
		{
		}
	}

	public SizeSL TextBodySize
	{
		get
		{
			return new SizeSL(0.0, 0.0);
		}
		set
		{
			this.ContentText.wordWrap = true;
		}
	}

	public int ItemCategory
	{
		get
		{
			return this._ItemCategory;
		}
		set
		{
			this._ItemCategory = value;
		}
	}

	public int ItemCode
	{
		get
		{
			return this._ItemCode;
		}
		set
		{
			this._ItemCode = value;
		}
	}

	public object ItemObject
	{
		get
		{
			return this._ItemObject;
		}
		set
		{
			this._ItemObject = value;
		}
	}

	public int BoxTypes
	{
		get
		{
			return this._BoxTypes;
		}
		set
		{
			this._BoxTypes = value;
		}
	}

	public new bool EnableIcon
	{
		get
		{
			return this._EnableIcon;
		}
		set
		{
			this._EnableIcon = value;
			if (this._EnableIcon)
			{
				this.Texture.mainTexture = this._BodySource;
				this.ContentText.textColor = this._NormalTextColor;
			}
			else
			{
				this.Texture.mainTexture = this.DisableBodySource;
				this.ContentText.textColor = this._DisableTextColor;
			}
		}
	}

	public bool DisableMovingEnd
	{
		get
		{
			return this._DisableMovingEnd;
		}
		set
		{
			this._DisableMovingEnd = value;
		}
	}

	public ImageBrush DisableBodySource
	{
		get
		{
			return this._DisableBodySource;
		}
		set
		{
			this._DisableBodySource = value;
		}
	}

	public BitmapData Composite1BodySource
	{
		get
		{
			return this._Composite1BodySource;
		}
		set
		{
			if (this.IconType != IconTypes.Composite)
			{
				return;
			}
			this._Composite1BodySource = value;
			if (this.Composite1BodySource != null)
			{
				if (null == this._Composite1BodyRectangle)
				{
					this._Composite1BodyRectangle = null;
				}
			}
			else
			{
				if (null != this._Composite1BodyRectangle)
				{
				}
				this._Composite1BodyRectangle = null;
			}
		}
	}

	public string Composite2BodyPath
	{
		get
		{
			return this._Composite2BodyPath;
		}
		set
		{
			this._Composite2BodyPath = value;
		}
	}

	public int MaxComposite2BodyCount
	{
		get
		{
			return this._MaxComposite2BodyCount;
		}
		set
		{
			if (this.IconType != IconTypes.Composite)
			{
				return;
			}
			this._MaxComposite2BodyCount = value;
			if (this._MaxComposite2BodyCount > 0)
			{
				if (null == this._Composite2BodyRectangle)
				{
				}
			}
			else
			{
				if (null != this._Composite2BodyRectangle)
				{
				}
				this._Composite2BodyRectangle = null;
			}
		}
	}

	public BitmapData EndTimeBodySource
	{
		get
		{
			return this._EndTimeBodySource;
		}
		set
		{
			if (this.IconType != IconTypes.Composite)
			{
				return;
			}
			this._EndTimeBodySource = value;
			if (this._EndTimeBodySource != null)
			{
				if (null == this._EndTimeBodyRectangle)
				{
					this._EndTimeBodyRectangle = new RectangleSL();
					this._EndTimeBodyRectangle.Width = this._EndTimeBodySource.width;
					this._EndTimeBodyRectangle.Height = this._EndTimeBodySource.height;
				}
				this._EndTimeBodyRectangle.Fill = new ImageBrush(this._EndTimeBodySource);
				this.StatePanel.Children.Add(this._EndTimeBodyRectangle);
				this.StatePanel.Height = (double)(this.StatePanel.Children.Count() * 9);
				this.RepositonStatePanel();
			}
			else
			{
				if (null != this._EndTimeBodyRectangle)
				{
					this.StatePanel.Children.Remove(this._EndTimeBodyRectangle, true);
				}
				this._EndTimeBodyRectangle = null;
			}
		}
	}

	public BitmapData BindingBodySource
	{
		get
		{
			return this._BindingBodySource;
		}
		set
		{
			if (this.IconType != IconTypes.Composite)
			{
				return;
			}
			this._BindingBodySource = value;
			if (this._BindingBodySource != null)
			{
				if (null == this._BindingBodyRectangle)
				{
					this._BindingBodyRectangle = new RectangleSL();
					this._BindingBodyRectangle.Width = this._BindingBodySource.width;
					this._BindingBodyRectangle.Height = this._BindingBodySource.height;
				}
				this._BindingBodyRectangle.Fill = new ImageBrush(this._BindingBodySource);
				this.StatePanel.Children.Add(this._BindingBodyRectangle);
				this.StatePanel.Height = (double)(this.StatePanel.Children.Count() * 9);
				this.RepositonStatePanel();
			}
			else
			{
				if (null != this._BindingBodyRectangle)
				{
					this.StatePanel.Children.Remove(this._BindingBodyRectangle, true);
				}
				this._BindingBodyRectangle = null;
			}
		}
	}

	public BitmapData NoUseBodySource
	{
		get
		{
			return this._NoUseBodySource;
		}
		set
		{
			if (this.IconType != IconTypes.Composite)
			{
				return;
			}
			this._NoUseBodySource = value;
			if (this._NoUseBodySource != null)
			{
				if (null == this._NoUseBodyRectangle)
				{
					this._NoUseBodyRectangle = new RectangleSL();
					this._NoUseBodyRectangle.Width = this._NoUseBodySource.width;
					this._NoUseBodyRectangle.Height = this._NoUseBodySource.height;
				}
				this._NoUseBodyRectangle.Fill = new ImageBrush(this._NoUseBodySource);
				this.StatePanel.Children.Add(this._NoUseBodyRectangle);
				this.StatePanel.Height = (double)(this.StatePanel.Children.Count() * 9);
				this.RepositonStatePanel();
			}
			else
			{
				if (null != this._NoUseBodyRectangle)
				{
					this.StatePanel.Children.Remove(this._NoUseBodyRectangle, true);
				}
				this._NoUseBodyRectangle = null;
			}
		}
	}

	public int TimerTicks
	{
		get
		{
			return this._TimerTicks;
		}
		set
		{
			this._TimerTicks = value;
		}
	}

	public bool DisableHandCursor
	{
		get
		{
			return this._DisableHandCursor;
		}
		set
		{
			this._DisableHandCursor = value;
		}
	}

	public bool OldEnableHint
	{
		get
		{
			return this._OldEnableHint;
		}
		set
		{
			this._OldEnableHint = value;
		}
	}

	public bool EnableHint
	{
		get
		{
			return this._EnableHint;
		}
		set
		{
			this._EnableHint = value;
			this.HintState = 0;
			if (this._EnableHint)
			{
				if (this._HintTimer != null)
				{
					this._HintTimer.Tick = null;
					this._HintTimer.Stop();
					this._HintTimer = null;
				}
				this._HintTimer = new DispatcherTimer("GIcon_HintTimer");
				if (this._TimerTicks == 0)
				{
					this._TimerTicks = 400;
				}
				this._HintTimer.Interval = TimeSpan.FromMilliseconds((double)this._TimerTicks);
				this._HintTimer.Tick = new DispatcherTimerEventHandler(this.HintTimer_Tick);
				this._HintTimer.Start();
			}
			else if (this._HintTimer != null)
			{
				this._HintTimer.Tick = null;
				this._HintTimer.Stop();
				this._HintTimer = null;
			}
		}
	}

	private void HintTimer_Tick(object sender, object e)
	{
		this.ProcessHint();
	}

	private void ProcessHint()
	{
		if (!this.EnableHint)
		{
			return;
		}
		if (this.MouseEnterState)
		{
			return;
		}
		if (!this.EnableIcon)
		{
			return;
		}
		switch (this.IconType)
		{
		case IconTypes.Transform:
			if (this.HintState == 0)
			{
				this.Texture.mainTexture = this._NewSource;
			}
			else
			{
				this.Texture.mainTexture = this._BodySource;
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.Highlights:
			if (this.HintState == 0)
			{
				this.HilightMask.bitmapData = this._NewSource.ImageSource;
				if (this._BodySource != null)
				{
				}
			}
			else
			{
				this.HilightMask.bitmapData = null;
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.HitModes:
			if (this.HintState == 0)
			{
				this.Texture.mainTexture = this._NewSource;
			}
			else
			{
				this.Texture.mainTexture = this._BodySource;
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.RepeatButton:
			if (this.HintState == 0)
			{
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.BodyShadow:
			if (this.HintState == 0)
			{
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.ThreeState:
			if (this.HintState == 0)
			{
				this.Texture.mainTexture = this._NewSource;
			}
			else
			{
				this.Texture.mainTexture = this._BodySource;
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
			break;
		case IconTypes.Composite:
			if (this._Composite2BodyRectangle != null && !string.IsNullOrEmpty(this.Composite2BodyPath))
			{
				this._Composite2BodyRectangle.bitmapData = GIcon.GetHintBitmapImage(StringUtil.substitute("{0}{1}.png", new object[]
				{
					this.Composite2BodyPath,
					this.HintState
				}));
			}
			this.HintState++;
			this.HintState = ((this.HintState < this._MaxComposite2BodyCount) ? this.HintState : 0);
			break;
		}
	}

	private static BitmapData GetHintBitmapImage(string name)
	{
		if (GIcon._HintImageDict.ContainsKey(name))
		{
			return GIcon._HintImageDict.GetValue(name);
		}
		BitmapData gameResImage = Global.GetGameResImage(name);
		GIcon._HintImageDict[name] = gameResImage;
		return gameResImage;
	}

	public bool STextVisibility
	{
		get
		{
			return this.SecondText.Visibility;
		}
		set
		{
			this.SecondText.Visibility = value;
		}
	}

	public string SText
	{
		get
		{
			return this.SecondText.Text;
		}
		set
		{
			this.SecondText.Text = value;
		}
	}

	public uint STextShadowColor
	{
		set
		{
			this.SecondText.fontBorder = value;
		}
	}

	public SolidColorBrush STextColor
	{
		set
		{
			this.SecondText.TextColor = value;
		}
	}

	public double STextSize
	{
		set
		{
			this.SecondText.TextSize = value;
		}
	}

	public string STextHorizontalAlignment
	{
		set
		{
			this.SecondText.HorizontalAlignment = value;
		}
	}

	public string STextVerticalAlignment
	{
		set
		{
			this.SecondText.VerticalAlignment = value;
		}
	}

	protected void InitHintDecoration(int decoCode, Point pos)
	{
		this.ClearHintDecoration();
		this.HintDeco = Global.GetDecoration(decoCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
		this.Container.Children.Add(this.HintDeco.The3DGameObject);
		this.HintDeco.Start();
	}

	protected new void ClearHintDecoration()
	{
		if (this.HintDeco != null)
		{
			this.HintDeco.Destroy();
			this.HintDeco = null;
		}
	}

	public int HintDecoType
	{
		get
		{
			return this.CurrentHintDecoType;
		}
		set
		{
			this.CurrentHintDecoType = 0;
			if (this.CurrentHintDecoType == 0)
			{
				this.InitHintDecoration(50005, new Point((int)this.Width / 2, 0));
			}
			else if (this.CurrentHintDecoType == 1)
			{
				this.InitHintDecoration(50004, new Point((int)this.Width / 2, (int)this.Height));
			}
			else if (this.CurrentHintDecoType == 2)
			{
				this.InitHintDecoration(50007, new Point(0, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 3)
			{
				this.InitHintDecoration(50006, new Point((int)this.Width, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 4)
			{
				this.InitHintDecoration(518, new Point(30, -85));
			}
			else if (this.CurrentHintDecoType == 5)
			{
				this.InitHintDecoration(533, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 6)
			{
				this.InitHintDecoration(531, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 7)
			{
				this.InitHintDecoration(532, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 8)
			{
				this.InitHintDecoration(560, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 9)
			{
				this.InitHintDecoration(534, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 10)
			{
				this.InitHintDecoration(537, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else
			{
				this.ClearHintDecoration();
			}
		}
	}

	public bool isEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			return component && component.enabled;
		}
		set
		{
			Collider component = base.GetComponent<Collider>();
			if (!component)
			{
				return;
			}
			if (component.enabled != value)
			{
				component.enabled = value;
				this.UpdateImage();
			}
		}
	}

	private void Btn_Click(GameObject go)
	{
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp(go, new MouseEvent("mouseUp", null));
		}
	}

	protected new virtual void Awake()
	{
		base.Awake();
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<UITexture>();
		}
		UIEventListener.Get(base.gameObject).onPress = delegate(GameObject go, bool b)
		{
			if (b)
			{
				this.Btn_Click(go);
			}
		};
	}

	private void OnEnable()
	{
		this.UpdateImage();
	}

	private void UpdateImage()
	{
		if (this.target != null)
		{
			if (this.isEnabled)
			{
			}
			this.target.MakePixelPerfect();
		}
	}

	protected void Destory()
	{
		if (this._HintTimer != null)
		{
			DispatcherTimerDriver.RemoveTimer(this._HintTimer);
		}
	}

	public void Refresh()
	{
		this.UpdateImage();
	}

	public UISprite __Progress;

	public BoxCollider mBoxCollider;

	private bool ButtonPressed;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUp;

	public MouseLeftButtonUpEventHandler MouseLeftButtonDown;

	public MouseLeftButtonUpEventHandler MouseLeftButtonDoubleClick;

	public Canvas IconCanvas;

	public IconTypes _IconType;

	public GTextBlockOutLine SecondText;

	public HSTextField ContentText;

	private StackPanel StatePanel;

	public UITexture Texture;

	private bool _Changed = true;

	private double _LastMouseDownTicks;

	private ImageBrush _BodySource;

	private ImageURL _BodyURL;

	private ImageBrush _NewSource;

	public new Bitmap Background;

	private bool _Hit;

	public Bitmap HilightMask;

	private int _Delay;

	private int _Interval;

	private BitmapData _HitBodySource;

	private BitmapData _HitNewBodySource;

	private uint _NormalTextColor = 16777215U;

	private uint _DisableTextColor = 8421504U;

	private string _TextHorizontalAlignment = global::Layout.Center;

	private string _TextVerticalAlignment = global::Layout.Center;

	private int _ItemCategory = 50;

	private int _ItemCode;

	private object _ItemObject;

	private int _BoxTypes = -1;

	private bool _EnableIcon = true;

	private bool _DisableMovingEnd;

	private ImageBrush _DisableBodySource;

	private Bitmap _Composite1BodyRectangle;

	private BitmapData _Composite1BodySource;

	private string _Composite2BodyPath;

	public Bitmap _Composite2BodyRectangle;

	private int _MaxComposite2BodyCount;

	private RectangleSL _EndTimeBodyRectangle;

	private BitmapData _EndTimeBodySource;

	private RectangleSL _BindingBodyRectangle;

	private BitmapData _BindingBodySource;

	private RectangleSL _NoUseBodyRectangle;

	private BitmapData _NoUseBodySource;

	private bool MouseEnterState;

	private int _TimerTicks = 400;

	private bool _DisableHandCursor;

	private int HintState;

	private bool _EnableHint;

	private bool _OldEnableHint;

	private DispatcherTimer _HintTimer;

	private static Dictionary<string, BitmapData> _HintImageDict = new Dictionary<string, BitmapData>();

	protected new GDecoration HintDeco;

	protected int CurrentHintDecoType = -1;

	private UITexture target;

	public string normalSprite;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;
}
