using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class GetThingPart : UserControl
{
	public GetThingPart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 171.0;
		this.listBox.Height = 175.0;
		Canvas.SetLeft(this.listBox, 10);
		Canvas.SetTop(this.listBox, 10);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.ItemMargin = new Thickness(3.0, 2.0, 0.0, 1.0);
		this.Container.Children.Add(this.Page);
		this.Page.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Page, 39);
		Canvas.SetTop(this.Page, 202);
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.HitBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("全部拾取");
		gicon.TextColor = new SolidColorBrush(10551295U);
		Canvas.SetLeft(gicon, 94);
		Canvas.SetTop(gicon, 195);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = -1,
					ID = 0
				});
			}
		};
		this.PrePageIcon = U3DUtils.NEW<GIcon>();
		this.PrePageIcon.Width = 16.0;
		this.PrePageIcon.Height = 21.0;
		this.PrePageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_normal.png"));
		this.PrePageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_hover.png"));
		this.PrePageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_nouse.png"));
		this.PrePageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PrePageIcon.EnableIcon)
			{
				this.PrevPage();
			}
		};
		Canvas.SetLeft(this.PrePageIcon, 16);
		Canvas.SetTop(this.PrePageIcon, 197);
		this.Container.Children.Add(this.PrePageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 16.0;
		this.NextPageIcon.Height = 21.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NextPageIcon.EnableIcon)
			{
				this.NextPage();
			}
		};
		Canvas.SetLeft(this.NextPageIcon, 65);
		Canvas.SetTop(this.NextPageIcon, 197);
		this.Container.Children.Add(this.NextPageIcon);
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount - 1)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void SetBtnState(bool state)
	{
		this.PrePageIcon.EnableIcon = state;
		this.NextPageIcon.EnableIcon = state;
	}

	public void AutoGetAllThing()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -1,
				ID = 0
			});
		}
	}

	public void InitPartData(int autoID)
	{
		this.AutoID = autoID;
		this.StartedTicks = (double)Global.GetCorrectLocalTime();
	}

	public void RunByTimer()
	{
		if (Super.GData.CurrentGoodsPackListData == null)
		{
			return;
		}
		if (Super.GData.CurrentGoodsPackListData.PackTicks <= 0L)
		{
			return;
		}
		if ((double)Global.GetCorrectLocalTime() - this.StartedTicks >= (double)Super.GData.CurrentGoodsPackListData.PackTicks && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -1,
				ID = 1
			});
		}
	}

	public void ItemSelected(object sender, object e)
	{
		this.CurrentSelectedItem = (sender as GetThingItem);
		if (null == this.CurrentSelectedItem)
		{
			return;
		}
		if (this.ItemCollection.Count > 0)
		{
			GameInstance.Game.SpriteGetThing(this.AutoID, this.CurrentSelectedItem.DbID);
		}
	}

	public void DeleteList(int goodsDbID)
	{
		if (goodsDbID < 0)
		{
			for (int i = 0; i < this.ItemsList.Count; i++)
			{
				this.ItemsList[i].ItemSelected = null;
			}
			this.ItemCollection.Clear();
			this.ItemsList.RemoveRange(0, this.ItemsList.Count);
		}
		else
		{
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				if (goodsDbID == this.ItemCollection[j].SafeGetComponent<GetThingItem>().DbID)
				{
					this.ItemCollection.RemoveAt(j);
					break;
				}
			}
			for (int k = 0; k < this.ItemsList.Count; k++)
			{
				if (goodsDbID == this.ItemsList[k].DbID)
				{
					this.ItemsList[k].ItemSelected = null;
					this.ItemsList.RemoveRange(k, 1);
					break;
				}
			}
		}
		if (this.ItemsList.Count <= 0)
		{
			this.SetBtnState(false);
			this.CurrentSelectedPage = 0;
			this.MaxPageCount = 0;
		}
		else
		{
			this.SetBtnState(true);
			this.MaxPageCount = (this.ItemsList.Count - 1) / 4 + 1;
			if (this.ItemCollection.Count > 0)
			{
				this.ShowPage(this.CurrentSelectedPage);
			}
			else
			{
				this.CurrentSelectedPage = 0;
				this.ShowPage(this.CurrentSelectedPage);
			}
		}
	}

	public void RefreshList(GoodsPackListData goodsPackListData)
	{
		if (this.AutoID != goodsPackListData.AutoID)
		{
			return;
		}
		this.SetBtnState(false);
		this.CurrentSelectedPage = 0;
		this.MaxPageCount = 0;
		this.ItemCollection.Clear();
		this.ItemsList.RemoveRange(0, this.ItemsList.Count);
		if (goodsPackListData.GoodsDataList == null)
		{
			return;
		}
		Super.GData.CurrentGoodsPackListData = goodsPackListData;
		if (Super.GData.CurrentGoodsPackListData == null)
		{
			return;
		}
		for (int i = 0; i < Super.GData.CurrentGoodsPackListData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData = Super.GData.CurrentGoodsPackListData.GoodsDataList[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					0,
					goodsData.Id,
					2
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = goodsData.GoodsID;
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.BoxTypes = -1;
				ggoodIcon.Text = ((goodsData.GCount <= 1) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsData.GCount.ToString()
				}));
				ggoodIcon.TextSize = 12;
				ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4278222848U;
				Super.InitEquipGIcon(ggoodIcon, goodsData, false, IconTextTypes.Qianghua);
				GetThingItem getThingItem = U3DUtils.NEW<GetThingItem>();
				getThingItem.BodyWidth = 165.0;
				getThingItem.BodyHeight = 40.0;
				getThingItem.LeftPanelWidth = 40.0;
				getThingItem.RightPanelWidth = 95.0;
				getThingItem.GoodsIcon = ggoodIcon;
				getThingItem.Text = goodsXmlNodeByID.Title;
				getThingItem.TextColor = Global.GetEnchanceColor(goodsData.Quality);
				getThingItem.DbID = goodsData.Id;
				getThingItem.ItemSelected = new EventHandler(this.ItemSelected);
				this.ItemsList.Add(getThingItem);
			}
		}
		if (this.ItemsList.Count > 0)
		{
			this.SetBtnState(true);
			this.CurrentSelectedPage = 0;
			this.MaxPageCount = (this.ItemsList.Count - 1) / 4 + 1;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void ShowPage(int pageIndex)
	{
		this.Page.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			pageIndex + 1,
			this.MaxPageCount
		});
		this.ItemCollection.Clear();
		int num = pageIndex * 4;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + 4)
		{
			this.ItemCollection.AddNoUpdate(this.ItemsList[num2]);
			num2++;
		}
		this.ItemCollection.DelayUpdate();
		if (pageIndex <= 0)
		{
			this.PrePageIcon.EnableIcon = false;
		}
		else
		{
			this.PrePageIcon.EnableIcon = true;
		}
		if (pageIndex >= this.MaxPageCount - 1)
		{
			this.NextPageIcon.EnableIcon = false;
		}
		else
		{
			this.NextPageIcon.EnableIcon = true;
		}
	}

	private SpriteSL thisCtrl;

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine Page = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<GetThingItem> ItemsList = new List<GetThingItem>();

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private GIcon PrePageIcon;

	private GIcon NextPageIcon;

	public int AutoID = -1;

	private double StartedTicks;

	private GetThingItem CurrentSelectedItem;

	private ObservableCollection _ItemCollection;
}
