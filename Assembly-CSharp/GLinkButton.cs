using System;
using UnityEngine;

public class GLinkButton : UserControl
{
	public string Text
	{
		set
		{
			this._Label.text = value;
			this.ReSize();
		}
	}

	public int FontSize
	{
		set
		{
			this._Label.transform.localScale = new Vector3((float)value, (float)value, 0f);
			this.ReSize();
		}
	}

	public bool UnderLine
	{
		set
		{
			this._Background.gameObject.SetActive(value);
		}
	}

	protected void ReSize()
	{
		this._Width = (double)(this._Label.transform.localScale.x * this._Label.relativeSize.x);
		this._Height = (double)(this._Label.transform.localScale.y * this._Label.relativeSize.y);
		this.Size = new Vector3((float)this._Width, (float)this._Height, 0f);
		if (null != this._Background)
		{
			this._Background.transform.localScale = this.Size;
		}
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (null != boxCollider)
		{
			boxCollider.center = new Vector3(this.Size.x / 2f, 0f, 0f);
			boxCollider.size = this.Size * this.ColiderSizeModule;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Background = base.GetComponentInChildren<UISprite>();
		this._Label = base.GetComponentInChildren<UILabel>();
		this.ReSize();
	}

	private UISprite _Background;

	private UILabel _Label;

	public float ColiderSizeModule = 1f;

	private Vector3 Size;
}
