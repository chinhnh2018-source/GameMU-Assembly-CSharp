using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class GGoodsCard : GTipSprite
{
	public new double Width
	{
		get
		{
			return this._Width;
		}
		set
		{
			this._Width = value;
			this.UpdateLayout();
			this.ReSize();
		}
	}

	public new double Height
	{
		get
		{
			return this._Height;
		}
		set
		{
			this._Height = value;
			this.UpdateLayout();
			this.ReSize();
		}
	}

	public double InnerWidth
	{
		get
		{
			return this._InnerWidth;
		}
		set
		{
			this._InnerWidth = value;
			this.ReSize();
		}
	}

	public double InnerHeight
	{
		get
		{
			return this._InnerHeight;
		}
		set
		{
			this._InnerHeight = value;
			this.ReSize();
		}
	}

	public double OuterWidth
	{
		get
		{
			return this._OuterWidth;
		}
		set
		{
			this._OuterWidth = value;
			this.ReSize();
		}
	}

	public double OuterHeight
	{
		get
		{
			return this._OuterHeight;
		}
		set
		{
			this._OuterHeight = value;
			this.UpdateLayout();
			this.ReSize();
		}
	}

	public new string Name
	{
		set
		{
			if (value != null)
			{
				this._Name.text = value;
			}
		}
	}

	public int NameSize
	{
		set
		{
			this._Name.transform.localScale = new Vector3((float)value, (float)value, 0f);
		}
	}

	public bool IsShow { get; protected set; }

	public int TagCode { get; set; }

	public int TagIndex { get; set; }

	public string BodyURL
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
				this._GoodsImg.URL = this._BodyURL;
			}
		}
	}

	protected override void InitializeComponent()
	{
		this._FrontImage.URL = Global.GetGameResImageString("richangHuodongTongguanJiangliItem1_bak.png");
		this._BackImage.URL = Global.GetGameResImageString("richangHuodongTongguanJiangliItem2_bak.png");
	}

	protected virtual void ReSize()
	{
		if (!double.IsNaN(this._Width) && !double.IsNaN(this._Height))
		{
			this._GoodsImg.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			if (this._OuterWidth == 0.0 && this._OuterHeight == 0.0)
			{
				this._FrontImage.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
				this._BackImage.transform.localScale = new Vector3((float)this._Width, (float)this._Height, 1f);
			}
			else
			{
				this._FrontImage.transform.localScale = new Vector3((float)this._OuterWidth, (float)this._OuterHeight, 1f);
				this._BackImage.transform.localScale = new Vector3((float)this._OuterWidth, (float)this._OuterHeight, 1f);
			}
			return;
		}
	}

	public void TurnCard()
	{
		if (null != this._Card && null != this._Card.GetComponent<Animation>())
		{
			this._Card.GetComponent<Animation>().Play();
			this.IsShow = true;
		}
	}

	public ShowNetImage _GoodsImg;

	public ShowNetImage _FrontImage;

	public ShowNetImage _BackImage;

	public UILabel _Level;

	public UILabel _Name;

	public Animation _Card;

	private double _InnerWidth;

	private double _InnerHeight;

	private double _OuterWidth;

	private double _OuterHeight;

	private string _BodyURL;
}
