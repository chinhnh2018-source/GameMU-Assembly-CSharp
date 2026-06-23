using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenShiPartShenShiItem : UserControl
{
	public int Lev
	{
		get
		{
			return this.lev;
		}
		set
		{
			this.lev = value;
		}
	}

	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	public bool IsJiHuo
	{
		get
		{
			return this.isJiHuo;
		}
		set
		{
			this.isJiHuo = value;
			this.isGray = !value;
		}
	}

	public bool IsZhuangBei
	{
		get
		{
			return this.isZhuangBei;
		}
		set
		{
			this.isZhuangBei = value;
		}
	}

	public int goodID
	{
		get
		{
			return this.goodid;
		}
		set
		{
			this.shenshiItem.URL = string.Format("NetImages/GameRes/Images/FuWenGods/God_{0}.png.qj", value);
			this.goodid = value;
		}
	}

	public bool isSelect
	{
		set
		{
			this.xuanzhongkuang.GetComponent<UISprite>().enabled = value;
		}
	}

	public bool openBoxC0llider
	{
		set
		{
			base.GetComponent<BoxCollider>().enabled = value;
		}
	}

	public bool openBtnClose
	{
		set
		{
			this.BtnClose.gameObject.SetActive(!value);
			this.isAddShenShiItem = !value;
		}
	}

	public bool isGray
	{
		set
		{
			if (value)
			{
				UITexture component = this.shenshiItem.GetComponent<UITexture>();
				component.shader = Shader.Find("Unlit/Gray");
			}
		}
	}

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs
			{
				ID = this.type * 100 + this.Lev,
				Index = this.Index
			});
		};
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject ss)
		{
			if (this.isAddShenShiItem)
			{
				this.Handler(this, new DPSelectedItemEventArgs
				{
					Type = this.Type
				});
			}
		};
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler Handler;

	public GButton BtnClose;

	public ShowNetImage shenshiItem;

	public GameObject xuanzhongkuang;

	public UILabel ShowLevel;

	private int lev;

	private int type;

	private bool isJiHuo;

	private bool isZhuangBei;

	private int goodid;

	private bool isAddShenShiItem;

	private int index;
}
