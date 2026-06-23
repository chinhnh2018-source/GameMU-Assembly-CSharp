using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GImgScrollShowWindow : GBitmapWindow
{
	public GImgScrollShowWindow()
	{
		this.Visibility = false;
	}

	public bool Angle90
	{
		set
		{
			this._Angle90 = value;
		}
	}

	private void DrawMask()
	{
		if (null != this.maskLayer)
		{
			this.InitLayerPos();
			base.mask = this.maskWarpper.getChildAt(0);
			return;
		}
		this.maskLayer = new Sprite();
		this.maskLayer.cacheAsBitmap = true;
		this.maskWarpper.Children.Add(this.maskLayer);
		this.Container.Children.Add(this.maskWarpper);
		this.InitLayerPos();
		base.mask = this.maskWarpper.getChildAt(0);
	}

	private void InitLayerPos()
	{
		if (this._Angle90)
		{
			Canvas.SetTop(this.maskWarpper, -base.BodyHeight - (double)this.OffsetValue_1 - (double)this.OffsetValue_2);
		}
		else
		{
			Canvas.SetLeft(this.maskWarpper, -base.BodyWidth - (double)this.OffsetValue_1 - (double)this.OffsetValue_2);
		}
	}

	public void ScrollShowWindow()
	{
		this.maskWarpper.visible = true;
	}

	public int EffectID
	{
		set
		{
			this._effectID = value;
		}
	}

	public Point EffectPos
	{
		set
		{
			this._effectPos = value;
		}
	}

	private void AddEffect()
	{
		if (this.ScrollEffect == null)
		{
			this.ScrollEffect = Global.GetDecoration(this._effectID, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
			this.ScrollEffect.Coordinate = this._effectPos;
		}
	}

	private void RemoveEffect()
	{
		this.maskLayer.visible = false;
		base.mask = null;
		if (this.ScrollEffect != null)
		{
		}
	}

	private Sprite maskWarpper = new Sprite();

	private Sprite maskLayer;

	private GDecoration ScrollEffect;

	private bool _Angle90;

	private int OffsetValue_1 = 20;

	private int OffsetValue_2 = 10;

	private int _effectID;

	private Point _effectPos = default(Point);
}
