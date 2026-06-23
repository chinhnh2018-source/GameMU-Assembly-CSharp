using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class MarkHotPart : UserControl
{
	public MarkHotPart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 608.0;
		this.listBox.Height = 335.0;
		Canvas.SetLeft(this.listBox, 12);
		Canvas.SetTop(this.listBox, 37);
		this.listBox.ItemMargin = new Thickness(3.0, 3.0, 0.0, 0.0);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
		this.RefreshData();
	}

	public void RefreshData()
	{
		this.ShowGoodsList();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void ShowGoodsList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Hot.Xml");
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "IDS", "ID", "1");
		if (xelement == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Value");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return;
		}
		string[] array = xelementAttributeStr.Split(new char[]
		{
			','
		});
		this.ItemCollection.Clear();
		GIcon gicon = null;
		for (int i = 0; i < array.Length; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(array[i]));
			if (goodsXmlNodeByID != null)
			{
				gicon = U3DUtils.NEW<GIcon>();
				gicon.Width = 32.0;
				gicon.Height = 32.0;
				gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
				gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
				gicon.TipType = 1;
				gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					Global.GetXElementAttributeStr(xelement, "ID"),
					0,
					-1,
					-1
				});
				gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
						{
							ID = U3DUtils.AS<Mark1ListItem>(this.listBox.SelectedItem).GoodsID,
							IDType = 0
						});
					}
				};
			}
			Mark1ListItem mark1ListItem = U3DUtils.NEW<Mark1ListItem>();
			mark1ListItem.BodyWidth = 118.0;
			mark1ListItem.BodyHeight = 80.0;
			mark1ListItem.Width = 118.0;
			mark1ListItem.Height = 80.0;
			mark1ListItem.ContentBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));
			mark1ListItem.ContentWidth = 40.0;
			mark1ListItem.ContentHeight = 40.0;
			mark1ListItem.GoodsName = Global.GetGoodsNameByID(Convert.ToInt32(array[i]), false);
			mark1ListItem.GoodsImg = gicon;
			mark1ListItem.GoodsID = Convert.ToInt32(array[i]);
			this.ItemCollection.AddNoUpdate(mark1ListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
	}

	private SpriteSL thisCtrl;

	private ListBox listBox = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection _ItemCollection;

	private GTabControl _tc;
}
