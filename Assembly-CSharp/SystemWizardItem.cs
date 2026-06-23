using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class SystemWizardItem : UserControl
{
	public GoodsData MyGoodsData
	{
		get
		{
			return this.m_GoodsData;
		}
	}

	public string ButtonName
	{
		set
		{
			this._Button.Text = value;
		}
	}

	public new string Name
	{
		set
		{
			this._Name.text = value;
		}
	}

	public string Desc
	{
		set
		{
			this._Desc.text = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Icon.EnableEvent = false;
		if (null == this._Icon)
		{
			this._Icon = UIHelper.AddGoodsIcon(this, null, null, false);
			this._Icon.OutSizeX = 92;
			this._Icon.OutSizeY = 92;
			this._Icon.BackSpriteName0 = "bagGrid2_bak";
		}
		if (null == this._TweenAlpha)
		{
			this._TweenAlpha = TweenAlpha.Begin(base.gameObject, 0f, 1f);
		}
		this._Icon.Width = 78.0;
		this._Icon.Height = 78.0;
	}

	public void InitTween(float delay, float during)
	{
		this.Delay = delay;
		this.During = during;
		if (delay >= 0f && during > 0f)
		{
			this._TweenAlpha.Reset();
			this._TweenAlpha.from = 1f;
			this._TweenAlpha.to = 0f;
			this._TweenAlpha.alpha = 1f;
			this._TweenAlpha.duration = this.During;
			this._TweenAlpha.delay = this.Delay;
			this._TweenAlpha.onFinished = new UITweener.OnFinished(this.OnFinished);
			this._TweenAlpha.Play(true);
		}
	}

	public void InitPartData(int IconCode, string text)
	{
		this._Name.text = text;
		this._Icon.BodyURL = new ImageURL(Global.GetGoodsIconString(IconCode), false, 0);
	}

	public void InitPartData(GoodsData goodsData)
	{
		this.m_GoodsData = goodsData;
		if (goodsData.Id > 0)
		{
			int goodsIconCodeByID = Global.GetGoodsIconCodeByID(this.m_GoodsData.GoodsID);
			this._Icon.Width = 78.0;
			this._Icon.Height = 78.0;
			this._Icon.BodyURL = new ImageURL(Global.GetGoodsIconString(goodsIconCodeByID), false, 0);
			Super.InitGoodsGIcon(this._Icon, goodsData, true, IconTextTypes.Qianghua);
		}
		else
		{
			this._Icon.Width = 86.667;
			this._Icon.Height = 86.667;
			this._Count.Text = goodsData.GCount.ToString();
			this._Icon.BodyURL = new ImageURL(Global.GetGoodsIconString(goodsData.GoodsID), false, 0);
		}
	}

	private void GoodsTipsParcelHandler(object sender, MouseEvent e)
	{
		GoodsData goodsData = this.m_GoodsData;
		if (this.Handler != null)
		{
			this.Handler(this, this.args);
		}
		if (goodsData != null)
		{
			GTipServiceEx.SelfBagOnly = true;
			GTipServiceEx.ShowTip(this._Icon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
	}

	private void OnFinished(UITweener tween)
	{
		if (this.Handler != null)
		{
			this.args.IDType = 1;
			this.Handler(this, this.args);
		}
	}

	public void CleanUpChildWindows()
	{
	}

	public static SystemWizardItem AddWizardItem(ObservableCollection ItemsSource, DPSelectedItemEventHandler handler)
	{
		SystemWizardItem item = U3DUtils.NEW<SystemWizardItem>();
		ItemsSource.AddNoUpdate(item);
		ItemsSource.DelayUpdate();
		if (handler != null)
		{
			item.Handler = handler;
			item._Icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				handler(item, item.args);
			};
			item._Button.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				handler(item, item.args);
			};
		}
		else
		{
			item._Icon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(item.GoodsTipsParcelHandler);
			item._Button.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(item.GoodsTipsParcelHandler);
		}
		return item;
	}

	public DPSelectedItemEventArgs args = new DPSelectedItemEventArgs();

	public DPSelectedItemEventHandler Handler;

	public GGoodIcon _Icon;

	public TextBlock _Name;

	public TextBlock _Desc;

	public TextBlock _Count;

	public GButton _Button;

	public TweenAlpha _TweenAlpha;

	private float Delay = -1f;

	private float During = -1f;

	private GoodsData m_GoodsData;
}
