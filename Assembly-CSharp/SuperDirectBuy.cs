using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class SuperDirectBuy : UserControl
{
	public override void Destroy()
	{
		base.Destroy();
		if (this.m_timer != null)
		{
			this.m_timer.Tick = null;
			this.m_timer.Stop();
			this.m_timer = null;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.m_timer = new DispatcherTimer("SuperDirectBuy");
		this.m_timer.Tick = delegate(object s, EventArgs e)
		{
			this.TimeTick();
		};
		this.m_timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_timer.Start();
		this.HaveBuy = false;
		this.BgImageURL = "ad";
		this.TitleImage = "-1";
	}

	private void InitPrefabText()
	{
		this.NumberLift = -1;
		this._BuyBtn.Label.text = Global.GetLang("购买");
		this.ObjPrice.transform.localPosition = new Vector3(280f, 130f, this.ObjPrice.transform.localPosition.z);
		this.UINowPrice.pivot = 5;
		this.UINowPrice.transform.localPosition = new Vector3(-15f, 0f, this.UINowPrice.transform.localPosition.z);
		this._NowPrice.Pivot = 5;
		this._NowPrice.transform.localPosition = new Vector3(170f, 0f, this._NowPrice.transform.localPosition.z);
	}

	private void InitTexture()
	{
		this.SetTextBlockText(this._BugCount, string.Empty, "fffffe");
		this.SetTextBlockText(this._TimeLift, string.Empty, "fffffe");
		this.SetTextBlockText(this._NumberLift, string.Empty, "fffffe");
	}

	private void InitHandler()
	{
		this.m_GoodsObservableCollection = this._ListBox.ItemsSource;
		this._BuyBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (0 < this.m_BtnCd)
			{
				return;
			}
			this.m_BtnCd = 1;
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					ID = this.m_ID
				});
			}
		};
	}

	private void SetTextBlockText(TextBlock block, string content, string color = "fffffe")
	{
		if (content == null)
		{
			content = string.Empty;
		}
		if (null != block)
		{
			if (null != block.Label)
			{
				block.Text = Global.GetColorStringForNGUIText(new object[]
				{
					color,
					content
				});
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("请为{0}的label赋值"),
					block.gameObject.name
				});
			}
		}
	}

	private void TimeTick()
	{
		this.m_TimeLift -= 10000000L;
		this.SetTime(new TimeSpan(this.m_TimeLift));
		if (0 < this.m_BtnCd)
		{
			this.m_BtnCd--;
		}
	}

	private void SetTime(TimeSpan time)
	{
		if (0.0 < time.TotalSeconds)
		{
			int num = this.MyDoubleToInt(time.TotalDays);
			int num2 = this.MyDoubleToInt(time.TotalHours);
			int num3 = this.MyDoubleToInt(time.TotalMinutes);
			int num4 = this.MyDoubleToInt(time.TotalSeconds);
			string text = (0 >= num) ? string.Empty : string.Format(Global.GetLang("{0}天"), num);
			string text2 = (0 >= num2) ? string.Empty : string.Format(Global.GetLang("{0}时"), this.MyDoubleToInt((double)time.Hours));
			string text3 = (0 >= num3) ? string.Empty : string.Format(Global.GetLang("{0}分"), this.MyDoubleToInt((double)time.Minutes));
			string text4 = string.Format(Global.GetLang("{0}秒"), this.MyDoubleToInt((double)time.Seconds));
			this.SetTextBlockText(this._TimeLift, string.Format("{0}{1}{2}{3}", new object[]
			{
				text,
				text2,
				text3,
				text4
			}), "fd010c");
		}
		else
		{
			string lang = Global.GetLang("0天0时0分0秒");
			this.SetTextBlockText(this._TimeLift, lang, "fd010c");
		}
	}

	private int MyDoubleToInt(double value)
	{
		string text = value.ToString();
		if (string.IsNullOrEmpty(text))
		{
			return Mathf.FloorToInt((float)value);
		}
		string[] array = text.Split(new char[]
		{
			'.'
		});
		return int.Parse(array[0]);
	}

	private void ChongZhi(int money, string productId = "")
	{
		PlatSDKMgr.Pay(money, productId, 0);
	}

	private void SetActivityTime(DateTime BeginTime, DateTime EndTime, string day)
	{
		string[] array = day.Split(new char[]
		{
			','
		});
		if (array.Length == 2)
		{
			int num = int.Parse(array[0]);
			int addDays = int.Parse(array[1]);
			this.m_TimeLift = this.GetAddDaysDataTime(BeginTime, addDays, true).Ticks - Global.GetCorrectDateTime().Ticks;
		}
		else if (EndTime >= BeginTime)
		{
			this.m_TimeLift = EndTime.Ticks - Global.GetCorrectDateTime().Ticks;
		}
	}

	public DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
	{
		if (roundDay)
		{
			dateTime..ctor(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
		}
		return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
	}

	public void RefreshContent(JieRiChongZhiQiangGouData3 data, JieRiChongZhiQiangGouData1 data1)
	{
		this.SetActivityTime(data1.FromDate, data1.ToDate, data.Day);
		Super.LoadGoodsList(data.goodsid, this.m_GoodsObservableCollection);
		if (this.m_GoodsObservableCollection.Count == 1)
		{
			this._ListBox.transform.localPosition = new Vector3(76f, this._ListBox.transform.localPosition.y, 0f);
		}
		this.NowPrice = data.XianJia;
		this.OldPrice = data.YuanJia;
		this.m_ID = data.ID;
		this.BuyCount = data.XianGouLeftNum;
		for (int i = 0; i < this.m_GoodsObservableCollection.Count; i++)
		{
			GameObject gameObject = this.m_GoodsObservableCollection[i];
			if (null != gameObject)
			{
				GGoodIcon component = gameObject.GetComponent<GGoodIcon>();
				BoxCollider boxCollider = component.transform.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = component.gameObject.AddComponent<BoxCollider>();
				}
				Vector3 center = boxCollider.center;
				center.z = -0.5f;
				boxCollider.center = center;
				if (null != component)
				{
					component.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						if (this.GoodsTipsCanShow)
						{
							GGoodIcon ggoodIcon = e as GGoodIcon;
							GTipServiceEx.SelfBagOnly = false;
							GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, ggoodIcon.ItemObject as GoodsData);
						}
					};
				}
			}
		}
	}

	public void DestoryPanel()
	{
		UIPanel component = base.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.size = this._BgImage.transform.localScale;
		boxCollider.center = this._BgImage.transform.parent.localPosition;
	}

	public UIDraggablePanel DragPanelContents
	{
		get
		{
			if (null == this.m_DragPanelContents)
			{
				this.m_DragPanelContents = base.GetComponent<UIDragPanelContents>();
				if (null == this.m_DragPanelContents)
				{
					this.m_DragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
				}
			}
			return this.m_DragPanelContents.draggablePanel;
		}
		set
		{
			if (null == this.m_DragPanelContents)
			{
				this.m_DragPanelContents = base.GetComponent<UIDragPanelContents>();
				if (null == this.m_DragPanelContents)
				{
					this.m_DragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
				}
			}
			this.m_DragPanelContents.draggablePanel = value;
		}
	}

	public bool HaveBuy
	{
		get
		{
			return this.m_HaveBuy;
		}
		set
		{
			this.m_HaveBuy = value;
			NGUITools.SetActive(this._HaveBuy, this.m_HaveBuy);
			NGUITools.SetActive(this._BuyBtn.gameObject, !this.m_HaveBuy);
		}
	}

	public int ChongZhiID
	{
		get
		{
			return this.m_ChongZhiID;
		}
		set
		{
			this.m_ChongZhiID = value;
		}
	}

	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
		}
	}

	public int Effect
	{
		set
		{
			if (value == 1)
			{
				GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Chongzhi/chaoji_zhigou_kuang", this._BgImage.transform.parent);
				gameObject.transform.localPosition = new Vector3(-11f, -24f, 31.1f);
			}
		}
	}

	public string TitleImage
	{
		set
		{
			this.m_TitleImage = value;
			if (this.m_TitleImage.Equals("-1") || string.IsNullOrEmpty(this.m_TitleImage))
			{
				this._NmaeTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("超级直购")
				});
				this._NmaeTitleYinYing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"000000",
					Global.GetLang("超级直购")
				});
			}
			else
			{
				this._NmaeTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang(this.m_TitleImage)
				});
				this._NmaeTitleYinYing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"000000",
					Global.GetLang(this.m_TitleImage)
				});
			}
		}
	}

	public string BgImageURL
	{
		get
		{
			return this.m_BgImageURL;
		}
		set
		{
			this.m_BgImageURL = value;
			this.HaveFailed = false;
			if (this.m_BgImageURL.SafeToInt32(0) >= 4)
			{
				this._BgImage.URL = "NetImages/GameRes/Images/SuperDirectBuy/" + string.Format("{0}-1", this.m_BgImageURL) + ".png.qj";
				this._BgImageBack.gameObject.SetActive(true);
				this._BgImageBack.URL = "NetImages/GameRes/Images/SuperDirectBuy/" + string.Format("{0}-2", this.m_BgImageURL) + ".png.qj";
			}
			else
			{
				this._BgImage.URL = "NetImages/GameRes/Images/SuperDirectBuy/" + this.m_BgImageURL + ".png.qj";
			}
			this._BgImage.ImageDownloadedErr = delegate(object e)
			{
				if (!this.HaveFailed)
				{
					this._BgImage.URL = "NetImages/GameRes/Images/SuperDirectBuy/-1.png.qj";
				}
				this.HaveFailed = true;
			};
		}
	}

	public int SinglePurchase
	{
		get
		{
			return this.m_SinglePurchase;
		}
		set
		{
			this.m_SinglePurchase = value;
		}
	}

	public long TimeLift
	{
		get
		{
			return this.m_TimeLift;
		}
		set
		{
			this.m_TimeLift = value;
		}
	}

	public int BuyCount
	{
		get
		{
			return this.m_BuyCount;
		}
		set
		{
			this.m_BuyCount = value;
			if (0 < this.m_BuyCount)
			{
				this.HaveBuy = false;
				this.SetTextBlockText(this._BugCount, Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format(Global.GetLang("个人限购：{0}"), this.m_BuyCount)
				}), "dac7ae");
			}
			else
			{
				this.HaveBuy = true;
				this.SetTextBlockText(this._BugCount, Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format(Global.GetLang("个人限购：{0}"), this.m_BuyCount)
				}), "dac7ae");
			}
		}
	}

	public int OldPrice
	{
		get
		{
			return this.m_OldPrice;
		}
		set
		{
		}
	}

	public int NowPrice
	{
		get
		{
			return this.m_NowPrice;
		}
		set
		{
			this.m_NowPrice = value;
			this.SetTextBlockText(this._NowPrice, string.Format("{0}{1}", this.m_NowPrice, this.m_MoneyType), "fac60d");
		}
	}

	public int NumberLift
	{
		get
		{
			return this.m_NumberLift;
		}
		set
		{
			this.m_NumberLift = value;
			if (0 > this.m_NumberLift)
			{
				this.SetTextBlockText(this._NumberLift, string.Empty, "fffffe");
			}
			else
			{
				this.SetTextBlockText(this._NumberLift, string.Format(Global.GetLang("剩余次数：{0}"), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.m_NumberLift.ToString()
				})), "e3b36c");
			}
		}
	}

	public ShowNetImage _BgImage;

	public ShowNetImage _BgImageBack;

	public ListBox _ListBox;

	public TextBlock _TimeLift;

	public TextBlock _NumberLift;

	public TextBlock _OldPrice;

	public TextBlock _NowPrice;

	public TextBlock _BugCount;

	public GButton _BuyBtn;

	public GameObject _HaveBuy;

	public UILabel _NmaeTitle;

	public UILabel _NmaeTitleYinYing;

	public GameObject ObjPrice;

	public UISprite UIOldPrice;

	public UISprite UINowPrice;

	[HideInInspector]
	public bool GoodsTipsCanShow;

	private long m_TimeLift;

	private int m_BuyCount;

	private int m_OldPrice;

	private int m_NowPrice;

	private int m_NumberLift;

	private int m_ID;

	private int m_ChongZhiID;

	private int m_SinglePurchase;

	private string m_MoneyType = string.Empty;

	private string m_BgImageURL = string.Empty;

	private ObservableCollection m_GoodsObservableCollection;

	private bool m_HaveBuy;

	private UIDragPanelContents m_DragPanelContents;

	private DispatcherTimer m_timer;

	private int m_BtnCd;

	private string m_TitleImage = string.Empty;

	public DPSelectedItemEventHandler Hander;

	private bool HaveFailed_Title = true;

	private bool HaveFailed;
}
