using System;
using System.Collections;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class GImgProgressBar : UISlider
{
	protected GameObject MyGameObject
	{
		get
		{
			if (null == this._MyGameObject)
			{
				this._MyGameObject = base.gameObject;
			}
			return this._MyGameObject;
		}
	}

	protected Transform MyTransform
	{
		get
		{
			if (null == this._MyTransform)
			{
				this._MyTransform = base.transform;
			}
			return this._MyTransform;
		}
	}

	public double Percent
	{
		get
		{
			return this._Percent;
		}
		set
		{
			this._Percent = value;
			base.sliderValue = (float)this._Percent;
		}
	}

	public bool Visibility
	{
		get
		{
			return this.MyGameObject.activeSelf;
		}
		set
		{
			if (this.MyGameObject.activeSelf != value)
			{
				this.MyGameObject.SetActive(value);
			}
		}
	}

	public float PartNum
	{
		set
		{
			this.PartWidth = 1f / value;
		}
	}

	public int Level
	{
		get
		{
			return this.PartLevel;
		}
		set
		{
			this.PartLevel = value;
			if (this.PartWidth != 0f)
			{
				base.sliderValue = this.PartWidth * (float)value;
			}
			else if (this._ItemWidth != 0f)
			{
				Vector3 localScale = this.foreground.localScale;
				this.foreground.localScale = new Vector3(this._ItemWidth * (float)value, localScale.y, localScale.z);
			}
		}
	}

	public int MaxLevel
	{
		set
		{
			Vector3 localScale = this.background.localScale;
			this.background.localScale = new Vector3(this._ItemWidth * (float)value, localScale.y, localScale.z);
		}
	}

	public float ItemWidth
	{
		set
		{
			this._ItemWidth = value;
		}
	}

	public ImageBrush BodyBackground { get; set; }

	public double BodyWidth { get; set; }

	public double BodyHeight { get; set; }

	public BitmapData ProgressBar_Source { get; set; }

	public SizeSL ProgressBar_Size { get; set; }

	public Point ProgressBar_pos { get; set; }

	public int EffectID { get; set; }

	public Point EffectPos { get; set; }

	public string ProgessText
	{
		get
		{
			if (this.uiLabel != null)
			{
				return this.uiLabel.text;
			}
			return this._ProgessText;
		}
		set
		{
			this._ProgessText = value;
			if (this.uiLabel != null)
			{
				this.uiLabel.text = this._ProgessText;
			}
		}
	}

	public int RadiusX { get; set; }

	public int RadiusY { get; set; }

	public SolidColorBrush ProgessTextColor { get; set; }

	public Thickness ProgTextMargin { get; set; }

	public void AddEffect()
	{
	}

	public double PercentByStopTween
	{
		set
		{
			this.StopTween();
			this.Percent = value;
		}
	}

	public void TweenPercent(double startPersent, double endPersent, double timeBySec = 1.0)
	{
		this.TotalTicks = timeBySec * 1000.0;
		this.StartPersent = startPersent;
		this.EndPersent = endPersent;
		this.LastTicks = (double)Global.GetCorrectLocalTime();
		this.ElapsedNumTicks = 0.0;
		base.StartCoroutine("TweenPercent_Tick");
	}

	private IEnumerator TweenPercent_Tick()
	{
		for (;;)
		{
			double ticks = (double)Global.GetCorrectLocalTime();
			double subTicks = ticks - this.LastTicks;
			this.LastTicks = ticks;
			this.ElapsedNumTicks += subTicks;
			if (this.TotalTicks != 0.0)
			{
				double persent = this.ElapsedNumTicks / this.TotalTicks;
				this.SetTweenPersent(persent);
				if (persent >= 1.0)
				{
					this.StopTween();
					this.Percent = this.EndPersent % 1.0;
				}
			}
			yield return new WaitForSeconds(0.01f);
		}
		yield break;
	}

	private void StopTween()
	{
		base.StopCoroutine("TweenPercent_Tick");
	}

	private void SetTweenPersent(double persent)
	{
		double num = (this.EndPersent - this.StartPersent) * persent + this.StartPersent;
		num %= 1.0;
		this.Percent = (double)((float)num);
	}

	private GameObject _MyGameObject;

	protected Transform _MyTransform;

	private double _Percent;

	private float PartWidth;

	private int PartLevel;

	private float _ItemWidth;

	public UILabel uiLabel;

	private string _ProgessText = string.Empty;

	private double TotalTicks = 1000.0;

	private double LastTicks;

	private double ElapsedNumTicks;

	private double StartPersent;

	private double EndPersent;
}
