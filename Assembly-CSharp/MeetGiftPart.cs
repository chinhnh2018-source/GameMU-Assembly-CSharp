using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class MeetGiftPart : UserControl
{
	public MeetGiftPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.info1.Text = Global.GetLang("恭喜您穿越到了隋唐末年，进放到了梦幻般的隋唐世界！");
		this.info2.Text = Global.GetLang("我们为您精心准备了在这个世界里需要的多种实用奖品。");
		this.info3.Text = Global.GetLang("如果您还没有领走这些奖品就下线了，我想大家都会感到遗憾的！");
		this.info1.TextLineHeight = 3.0;
		this.info2.TextLineHeight = 3.0;
		this.info3.TextLineHeight = 3.0;
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container2.Background = new SolidColorBrush(16777215U);
		this.Container2.HorizontalAlignment = global::Layout.Left;
		this.Container2.VerticalAlignment = global::Layout.Top;
		this.Container2.Orientation = global::Layout.Vertical;
		this.Container2.Children.Add(this.info1);
		this.info1.Text = Global.GetLang("恭喜您穿越到了隋唐末年，进放到了梦幻般的隋唐世界！");
		Canvas.SetLeft(this.info1, 11);
		this.info1.Width = 480.0;
		Canvas.SetTop(this.info1, 8);
		this.info1.Height = 23.0;
		this.info1.FontSize = HSTextField.defaultFontSize;
		this.info1.TextColor = new SolidColorBrush(10626862U);
		this.Container2.Children.Add(this.info2);
		this.info2.Text = Global.GetLang("我们为您精心准备了在这个世界里需要的多种实用奖品。");
		Canvas.SetLeft(this.info2, 11);
		this.info2.Width = 480.0;
		Canvas.SetTop(this.info2, 28);
		this.info2.Height = 23.0;
		this.info2.FontSize = HSTextField.defaultFontSize;
		this.info2.TextColor = new SolidColorBrush(1999025U);
		this.Container2.Children.Add(this.info3);
		this.info3.Text = Global.GetLang("如果您还没有领走这些奖品就下线了，我想大家都会感到遗憾的！");
		Canvas.SetLeft(this.info3, 11);
		this.info3.Width = 480.0;
		Canvas.SetTop(this.info3, 56);
		this.info3.Height = 23.0;
		this.info3.FontSize = HSTextField.defaultFontSize;
		this.info3.TextColor = new SolidColorBrush(13277735U);
		this.Container2.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 6);
		Canvas.SetTop(this.listBox, 78);
		this.listBox.Width = 476.0;
		this.listBox.Height = 288.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		this.scrollViewer1 = new GScrollView(514, 325, 0);
		Canvas.SetLeft(this.scrollViewer1, 25);
		Canvas.SetTop(this.scrollViewer1, 12);
		this.Container2.Width = this.scrollViewer1.Width;
		this.scrollViewer1.Viewer = this.Container2;
		this.scrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.Container.Children.Add(this.scrollViewer1);
		this.scrollViewer1.ResetScrollView();
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.BuildDataList();
	}

	private void BuildDataList()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/NewRoleGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					','
				});
				if (array.Length == 6)
				{
					int[] array2 = Global.StringArray2IntArray(array);
					MeetGiftItem meetGiftItem = U3DUtils.NEW<MeetGiftItem>();
					meetGiftItem.TextInfo = Global.GetXElementAttributeStr(xelement, "Description");
					meetGiftItem.GoodsNmaeColor = Global.GetEnchanceColor(array2[2]);
					meetGiftItem.GoodsNmae = this.GetGoodsinfo(array2[0], array2[1], array2[2], array2[3], array2[4]);
					meetGiftItem.TimeTexts = Global.GetLang("只需 ") + Global.GetXElementAttributeStr(xelement, "TimeSecs") + Global.GetLang(" 分钟");
					meetGiftItem.IconListBoxBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/xtsl_rec2.png"));
					GIcon gicon = U3DUtils.NEW<GIcon>();
					gicon.Width = 48.0;
					gicon.Height = 48.0;
					gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/libao.png"));
					meetGiftItem.BagBoxIcons = gicon;
					this.ItemCollection.AddNoUpdate(meetGiftItem);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
		this.scrollViewer1.ResetScrollView();
	}

	private string GetGoodsinfo(int goodsID, int gcount, int quality, int forgeLevel, int binding)
	{
		string text = Global.GetGoodsNameByID(goodsID, false);
		if (forgeLevel > 0)
		{
			text = StringUtil.substitute("{0}(+{1})x{2}", new object[]
			{
				text,
				forgeLevel,
				gcount
			});
		}
		else
		{
			text = StringUtil.substitute("{0}x{1}", new object[]
			{
				text,
				gcount
			});
		}
		return text;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		this.scrollViewer1.ResetScrollView();
	}

	public void RefreshHuodongData()
	{
	}

	private GScrollView scrollViewer1;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private StackPanel Container2 = new StackPanel();

	private GTextBlockOutLine info1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info3 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	public ObservableCollection ItemCollection;

	private GTabControl _tc;
}
