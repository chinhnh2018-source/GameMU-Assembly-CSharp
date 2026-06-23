using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GWuxueImg : UserControl
{
	public GWuxueImg()
	{
		this.Container.Width = 197.0;
		this.Container.Height = 197.0;
		this.Container.Children.Add(this.ImgBak);
		this.ImgBak.Width = 197.0;
		this.ImgBak.Height = 197.0;
		this.Container.Children.Add(this.ImgPhoto);
		this.ImgPhoto.Width = 128.0;
		this.ImgPhoto.Height = 128.0;
		Canvas.SetLeft(this.ImgPhoto, 35);
		Canvas.SetTop(this.ImgPhoto, 35);
	}

	public BitmapData ImgBak_Source
	{
		set
		{
			this.ImgBak.Source = new ImageBrush(value);
		}
	}

	public BitmapData ImgPhoto_Source
	{
		set
		{
			this.ImgPhoto.Source = new ImageBrush(value);
		}
	}

	public bool Jihuo
	{
		set
		{
			this._jihuo = value;
			if (value)
			{
				this.AddEffect();
			}
			else
			{
				this.RemoveEffect();
			}
		}
	}

	private void AddEffect()
	{
		this.WuXueEffect = Global.GetDecoration(505, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.WuXueEffect.Coordinate = new Point(233, 315);
	}

	public void RemoveEffect()
	{
		if (this.WuXueEffect != null)
		{
			Global.RemoveObject(this.WuXueEffect, true);
		}
	}

	public void PauseEffect(bool pause)
	{
		if (this.WuXueEffect != null)
		{
			this.WuXueEffect.Pause = pause;
		}
	}

	private Image ImgBak = new Image();

	private Image ImgPhoto = new Image();

	public GDecoration WuXueEffect;

	private bool _jihuo;
}
