using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenShiPartFuWenZongLanItem : UserControl
{
	public int goodID
	{
		get
		{
			return this.goodid;
		}
		set
		{
			this.GoodsIcon.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", value);
			this.goodid = value;
		}
	}

	public bool isUsed
	{
		get
		{
			return this.isused;
		}
		set
		{
			this.isused = value;
			this.xiangqian.GetComponent<UISprite>().enabled = value;
		}
	}

	public bool isGet
	{
		set
		{
			this.isGray = value;
		}
	}

	public bool isGray
	{
		set
		{
			this.MengHui.GetComponent<UITexture>().enabled = !value;
			this.BG.spriteName = "1jigezidi";
		}
	}

	public new string Name
	{
		get
		{
			return this.name;
		}
		set
		{
			this.goodsName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				value
			});
			this.name = value;
		}
	}

	public new int Count
	{
		get
		{
			return this.count;
		}
		set
		{
			this.goodsCount.text = value.ToString();
			this.count = value;
		}
	}

	public string Attr
	{
		get
		{
			return this.attr;
		}
		set
		{
			this.goodsAttr.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
			this.attr = value;
		}
	}

	public bool XuanZhong
	{
		get
		{
			return this.select;
		}
		set
		{
			this.XuanZhongFenJie.Check = value;
			this.MengHui.GetComponent<UITexture>().enabled = value;
			this.select = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.MengHui.URL = "NetImages/GameRes/Images/shenshiTexture/hui.png.qj";
		if (null != this.XuanZhongFenJie)
		{
			this.XuanZhongFenJie.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.MengHui.GetComponent<UITexture>().enabled = this.XuanZhongFenJie.Check;
				this.select = this.XuanZhongFenJie.Check;
				this.Handler(this, new DPSelectedItemEventArgs());
			};
		}
	}

	public DPSelectedItemEventHandler Handler;

	public UISprite BG;

	public GameObject xiangqian;

	public ShowNetImage MengHui;

	public ShowNetImage GoodsIcon;

	public GCheckBox XuanZhongFenJie;

	public UILabel goodsName;

	public UILabel goodsCount;

	public UILabel goodsAttr;

	private int goodid;

	private bool isused;

	private string name;

	private int count;

	private string attr;

	private bool select;
}
