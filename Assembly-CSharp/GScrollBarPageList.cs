using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class GScrollBarPageList : UIScrollBar, ISilverLight
{
	public int Page
	{
		get
		{
			return this._Page;
		}
		set
		{
			this._Page = value;
			if (this._ItemCount > this._ItemPerPage)
			{
				base.scrollValue = (float)(this._ItemCount - (this._PageCount - this._Page) * this._ItemPerPage) / (float)(this._ItemCount - this._ItemPerPage);
			}
			else
			{
				base.scrollValue = 0f;
			}
		}
	}

	public int ItemPerPage
	{
		get
		{
			return this._ItemPerPage;
		}
		set
		{
			this._ItemPerPage = value;
			this.Resize();
		}
	}

	public int ItemCount
	{
		get
		{
			return this._ItemCount;
		}
		set
		{
			this._ItemCount = value;
			this.Resize();
		}
	}

	[SerializeField]
	public bool IsEnabled
	{
		set
		{
			U3DUtils.EnableCollider(base.gameObject, value);
		}
	}

	public int PageCount
	{
		get
		{
			return this._PageCount;
		}
		protected set
		{
			this._PageCount = value;
			if (this._PageCount >= 1)
			{
				base.transform.localPosition = new Vector3(this._LocalLocation.x - (float)(this._PageCount - 1) / 2f * this.ItemSize.x, this._LocalLocation.y, this._LocalLocation.z);
			}
		}
	}

	public Vector3 LocalLocation
	{
		set
		{
			base.transform.localPosition += value - this._LocalLocation;
			this._LocalLocation = value;
		}
	}

	protected virtual void Awake()
	{
		this._LocalLocation = base.transform.localPosition;
		foreach (UISprite uisprite in base.GetComponentsInChildren<UISprite>())
		{
			if (uisprite.name == "Background")
			{
				base.background = uisprite;
			}
			else if (uisprite.name == "Foreground")
			{
				base.foreground = uisprite;
				uisprite.MakePixelPerfect();
			}
		}
		this.Resize();
	}

	public override float scrollValue
	{
		get
		{
			return base.scrollValue;
		}
		set
		{
			if (this.PageCount != 0)
			{
				this.barSize = 1f / (float)this.PageCount;
			}
			this.Page = Mathf.RoundToInt(Mathf.Clamp01(value) * (float)(this._PageCount - 1));
			if (this._ItemCount > this._ItemPerPage)
			{
				base.scrollValue = (float)(this._ItemCount - (this._PageCount - this._Page) * this._ItemPerPage) / (float)(this._ItemCount - this._ItemPerPage);
			}
			else
			{
				base.scrollValue = 0f;
			}
		}
	}

	public void GotoPage(int page)
	{
		this.Page = Mathf.Clamp(page, 0, this._PageCount - 1);
	}

	public void Resize()
	{
		if (this._ItemPerPage == 0)
		{
			return;
		}
		this.PageCount = (this._ItemCount - 1) / this._ItemPerPage + 1;
		if (this._ItemCount > this._ItemPerPage)
		{
			this.scrollValue = 0f;
			if (null != base.foreground)
			{
				if (this.ScrollType == GScrollBarPageList.GScrollType.Hide)
				{
					base.transform.localScale = Vector3.zero;
				}
				else if (this.ScrollType == GScrollBarPageList.GScrollType.PageRadio)
				{
					base.background.transform.localScale = new Vector3(this.ItemSize.x * (float)this.PageCount, this.ItemSize.y, this.ItemSize.z);
					base.foreground.transform.localScale = this.ItemSize;
					if (this.PageCount != 0)
					{
						this.barSize = 1f / (float)this.PageCount;
					}
					base.foreground.MakePixelPerfect();
					base.transform.localScale = Vector3.one;
				}
			}
			return;
		}
	}

	public double Width { get; set; }

	public double Height { get; set; }

	public double ActualWidth
	{
		get
		{
			return 0.0;
		}
	}

	public double ActualHeight
	{
		get
		{
			return 0.0;
		}
	}

	public string Name { get; set; }

	public bool Visibility { get; set; }

	public object Tag { get; set; }

	public bool IsHitTestVisible { get; set; }

	public GameObject Parent { get; set; }

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public Thickness Margin { get; set; }

	public void UpdateLayout()
	{
	}

	public GScrollBarPageList.GScrollType ScrollType = GScrollBarPageList.GScrollType.PageRadio;

	private int _Page;

	[SerializeField]
	protected int _ItemPerPage = 3;

	[SerializeField]
	protected int _ItemCount;

	[SerializeField]
	protected int _PageCount = 5;

	[SerializeField]
	protected Vector3 ItemSize;

	[SerializeField]
	private Vector3 _LocalLocation;

	public enum GScrollType
	{
		Hide,
		PageRadio
	}
}
