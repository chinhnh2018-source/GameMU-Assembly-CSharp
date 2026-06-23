using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class JianMianYouLiPart : UserControl
{
	public JianMianYouLiPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.info1);
		this.info1.Text = Global.GetLang("欢迎您来到《怒斩》！");
		Canvas.SetLeft(this.info1, 18);
		this.info1.Width = 480.0;
		Canvas.SetTop(this.info1, 18);
		this.info1.Height = 23.0;
		this.info1.TextColor = new SolidColorBrush(4294967040U);
		this.info1.fontBold = true;
		this.Container.Children.Add(this.info2);
		this.info2.Text = Global.GetLang("我们为您精心准备了在这个世界里需要的多种实用奖品。如果您还没有领走这些奖品就下线了，我想大家都会感到遗憾的！");
		this.info2.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.info2, 18);
		Canvas.SetTop(this.info2, 49);
		this.info2.Width = 480.0;
		this.info2.BodyWidth = 480.0;
		this.info2.Height = 40.0;
		this.info2.TextColor = new SolidColorBrush(11394222U);
		this.Container.Children.Add(this.info3);
		this.info3.Text = Global.GetLang("温馨提示：每隔几分钟，您就能领到丰厚的奖品！");
		Canvas.SetLeft(this.info3, 18);
		this.info3.Width = 480.0;
		Canvas.SetTop(this.info3, 127);
		this.info3.Height = 23.0;
		this.info3.TextColor = new SolidColorBrush(4294967040U);
		this.info3.fontBold = true;
		this.Container.Children.Add(this.timeGtext);
		this.timeGtext.htmlText = "<b>" + Global.GetColorStringForHtmlText(new object[]
		{
			"#ffffff37",
			Global.GetLang("活动时间："),
			"#ff37ff37",
			Global.GetLang("【永久】")
		}) + "</b>";
		Canvas.SetLeft(this.timeGtext, 380);
		this.timeGtext.Width = 480.0;
		Canvas.SetTop(this.timeGtext, 127);
		this.timeGtext.Height = 23.0;
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 15);
		Canvas.SetTop(this.listBox, 165);
		this.listBox.Width = 570.0;
		this.listBox.Height = 288.0;
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
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
					meetGiftItem.BindYuanbao = StringUtil.substitute(Global.GetLang("绑定钻石:{0}"), new object[]
					{
						Global.GetXElementAttributeStr(xelement, "BindYuanBao")
					});
					meetGiftItem.BindTongqian = StringUtil.substitute(Global.GetLang("绑定金币:{0}"), new object[]
					{
						Global.GetXElementAttributeStr(xelement, "BindMoney")
					});
					meetGiftItem.TimeTexts = Global.GetLang("只需登陆 ") + Global.GetXElementAttributeStr(xelement, "TimeSecs") + Global.GetLang(" 分钟即可领取！");
					meetGiftItem.IconListBoxBackground = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/rm_listItem.png"), 60.0, 64.0, 3.0, 2.0));
					meetGiftItem.TimeTextsLeft = 350;
					meetGiftItem.TimeTextsTop = 25;
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
			this.Container.Children.Remove(this.LoadingWin, true);
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
	}

	public ObservableCollection ItemCollection;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GTextBlockOutLine info1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info3 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine timeGtext = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();
}
