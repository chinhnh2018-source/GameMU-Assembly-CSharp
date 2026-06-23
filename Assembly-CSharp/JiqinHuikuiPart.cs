using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JiqinHuikuiPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.partCanvas);
		this.partCanvas.Background = new SolidColorBrush(16777215U);
		this.partCanvas.Width = 414.0;
		this.partCanvas.Height = 248.0;
		Canvas.SetLeft(this.partCanvas, 132);
		Canvas.SetTop(this.partCanvas, 118);
		this.Container.Children.Add(this.partCanvas2);
		this.partCanvas2.Background = new SolidColorBrush(16777215U);
		this.partCanvas2.Width = 414.0;
		this.partCanvas2.Height = 313.0;
		Canvas.SetLeft(this.partCanvas2, 132);
		Canvas.SetTop(this.partCanvas2, 53);
		this.HuodongIntro = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongIntro.TextFontColor = new SolidColorBrush(16777215U);
		this.HuodongIntro.TextSize = 12.0;
		this.HuodongIntro.mouseEnabled = false;
		this.HuodongIntro.TextWidth = 388.0;
		this.HuodongIntro.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.HuodongIntro, 145);
		Canvas.SetTop(this.HuodongIntro, 60);
		this.Container.Children.Add(this.HuodongIntro);
		this.HuodongTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongTitle.TextFontColor = new SolidColorBrush(16777080U);
		this.HuodongTitle.TextSize = 14.0;
		this.HuodongTitle.fontBold = true;
		this.HuodongTitle.Width = 125.0;
		this.HuodongTitle.Height = 25.0;
		Canvas.SetLeft(this.HuodongTitle, 295);
		Canvas.SetTop(this.HuodongTitle, 19);
		this.Container.Children.Add(this.HuodongTitle);
	}

	public void InitPartData(int pageID = 1)
	{
		string iconName = null;
		if (this.ItemsDict.TryGetValue(pageID, ref iconName))
		{
			this.SelectIcon(iconName);
		}
	}

	public void LoadItemByConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/RiChangGifts/JieRiType.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		if (xelementList == null)
		{
			return;
		}
		int num = 10;
		string text = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "LiuShuiID");
				text = Global.GetXElementAttributeStr(xelement, "PeiZhi");
				this.AddIcon(text, xelementAttributeInt, Global.GetXElementAttributeStr(xelement, "Name"), num);
				this.ItemsDict[xelementAttributeInt] = text;
				num += 41;
			}
		}
		this.InitPartData(1);
	}

	public void AddIcon(string namex, int flag, string text, int topY)
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = namex;
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang(text);
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = flag;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon(namex);
		};
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, topY);
		this.Container.Children.Add(gicon);
	}

	private void SelectIcon(string iconName)
	{
		this.SetBtnState(iconName);
		this.SetBackground(this.SelectedIcon.ItemCode);
		this.SetPart(this.SelectedIcon.ItemCode);
	}

	private void SetBtnState(string iconName)
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName(iconName));
		if (gicon == this.SelectedIcon)
		{
			return;
		}
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.TextColor = new SolidColorBrush(16777080U);
		this.HuodongTitle.Text = gicon.Text;
		if (null != this.SelectedIcon)
		{
			this.SelectedIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
			this.SelectedIcon.TextColor = new SolidColorBrush(8418620U);
		}
		this.SelectedIcon = gicon;
	}

	private void SetBackground(int showPage)
	{
		if (showPage != 4)
		{
			this.partCanvas.Visibility = true;
			this.partCanvas2.Visibility = false;
			this.HuodongIntro.Visibility = true;
		}
		else
		{
			this.partCanvas.Visibility = false;
			this.partCanvas2.Visibility = true;
			this.HuodongIntro.Visibility = false;
		}
		if (showPage == 1 || showPage == 2)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JiqinKuizheng_bak2.png"), false, 0);
		}
		else if (showPage == 3)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JiqinKuizheng_bak4.png"), false, 0);
		}
		else if (showPage == 4)
		{
			this.partCanvas2.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JiqinKuizheng_bak5.png"), false, 0);
		}
	}

	private void SetPart(int showPage)
	{
		List<XElement> list = null;
		if (showPage != 4)
		{
			XElement gameResXml = Global.GetGameResXml(StringUtil.substitute("Config/RiChangGifts/{0}", new object[]
			{
				this.SelectedIcon.Name
			}));
			if (gameResXml == null)
			{
				return;
			}
			if (Global.GetXElementList(gameResXml, "Activities") == null)
			{
				return;
			}
			list = Global.GetXElementList(gameResXml, "GiftList");
			if (list == null)
			{
				return;
			}
			XElement xelement = list[0];
			if (xelement != null)
			{
				this.HuodongIntro.Text = Global.GetXElementAttributeStr(xelement, "Description");
			}
			list = Global.GetXElementList(gameResXml, "Award");
		}
		this.partCanvas.Clear();
		this.partCanvas2.Clear();
		switch (showPage)
		{
		case 1:
			if (null == this.jiqinHuikuiPart01)
			{
				this.jiqinHuikuiPart01 = U3DUtils.NEW<JiqinHuikuiPart01>();
				this.jiqinHuikuiPart01.InitPartSize(414, 248);
				this.jiqinHuikuiPart01.InitPartData(list);
			}
			this.jiqinHuikuiPart01.GetData();
			this.partCanvas.Add(this.jiqinHuikuiPart01);
			Canvas.SetLeft(this.jiqinHuikuiPart01, 0);
			Canvas.SetTop(this.jiqinHuikuiPart01, 0);
			break;
		case 2:
			if (null == this.jiqinHuikuiPart02)
			{
				this.jiqinHuikuiPart02 = U3DUtils.NEW<JiqinHuikuiPart02>();
				this.jiqinHuikuiPart02.InitPartSize(414, 248);
				this.jiqinHuikuiPart02.InitPartData(list);
			}
			this.jiqinHuikuiPart02.GetData();
			this.partCanvas.Add(this.jiqinHuikuiPart02);
			Canvas.SetLeft(this.jiqinHuikuiPart02, 0);
			Canvas.SetTop(this.jiqinHuikuiPart02, 0);
			break;
		case 3:
			if (null == this.jiqinHuikuiPart03)
			{
				this.jiqinHuikuiPart03 = U3DUtils.NEW<JiqinHuikuiPart03>();
				this.jiqinHuikuiPart03.InitPartSize(414, 248);
				this.jiqinHuikuiPart03.InitPartData(list);
			}
			this.jiqinHuikuiPart03.GetData();
			this.partCanvas.Add(this.jiqinHuikuiPart03);
			Canvas.SetLeft(this.jiqinHuikuiPart03, 0);
			Canvas.SetTop(this.jiqinHuikuiPart03, 0);
			break;
		case 4:
			if (null == this.jiqinHuikuiPart04)
			{
				this.jiqinHuikuiPart04 = U3DUtils.NEW<JiqinHuikuiPart04>();
				this.jiqinHuikuiPart04.InitPartSize(414, 313);
				this.jiqinHuikuiPart04.InitGoodsGicon();
			}
			this.jiqinHuikuiPart04.GetData();
			this.partCanvas2.Add(this.jiqinHuikuiPart04);
			Canvas.SetLeft(this.jiqinHuikuiPart04, 0);
			Canvas.SetTop(this.jiqinHuikuiPart04, 0);
			this.jiqinHuikuiPart04.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0 && this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = 0
					});
				}
			};
			break;
		}
		if (this.jiqinHuikuiDate != null)
		{
			this.GetInfoResultData(this.jiqinHuikuiDate, showPage);
		}
	}

	public void GetInfoResultData(JiQingHuiKuiData data, int page = 1)
	{
		this.jiqinHuikuiDate = data;
		if (page == 1)
		{
			if (null != this.jiqinHuikuiPart01)
			{
				this.jiqinHuikuiPart01.OnGetDataCompleted(data);
			}
		}
		else if (page == 2)
		{
			if (null != this.jiqinHuikuiPart02)
			{
				this.jiqinHuikuiPart02.OnGetDataCompleted(data);
			}
		}
		else if (page == 3)
		{
			if (null != this.jiqinHuikuiPart03)
			{
				this.jiqinHuikuiPart03.OnGetDataCompleted(data);
			}
		}
		else if (page == 4 && null != this.jiqinHuikuiPart04)
		{
			this.jiqinHuikuiPart04.OnGetDataCompleted(data);
		}
	}

	public void GetData()
	{
		GameInstance.Game.SpriteQueryActivityInfo();
	}

	public void LingQuResult(int result, int activityType, int retValue, int retIndex)
	{
		if (result >= 0)
		{
			this.RefreshData(activityType, retValue, retIndex);
		}
		if (activityType == 27)
		{
			this.jiqinHuikuiPart01.OnLingquCompleted(result, this.jiqinHuikuiDate);
		}
		else if (activityType == 28)
		{
			this.jiqinHuikuiPart02.OnLingquCompleted(result, this.jiqinHuikuiDate);
		}
		else if (activityType == 29)
		{
			this.jiqinHuikuiPart03.OnLingquCompleted(result, this.jiqinHuikuiDate);
		}
		else if (activityType == 31)
		{
			this.jiqinHuikuiPart04.OnLingquCompleted(result, this.jiqinHuikuiDate);
		}
	}

	public void RefreshData(int activityType, int retValue, int retIndex = 1)
	{
		if (this.jiqinHuikuiDate == null)
		{
			return;
		}
		if (activityType == 27)
		{
			this.jiqinHuikuiDate.TodayYuanBaoState = Global.SetIntSomeBit(retIndex - 1, this.jiqinHuikuiDate.TodayYuanBaoState, true);
		}
		else if (activityType == 28)
		{
			this.jiqinHuikuiDate.ChongJiQingQuShenZhuangQuota[retIndex - 1] = retValue;
			this.jiqinHuikuiDate.ChongJiLingQuShenZhuangState = Global.SetIntSomeBit(retIndex - 1, this.jiqinHuikuiDate.ChongJiLingQuShenZhuangState, true);
		}
		else if (activityType == 29)
		{
			this.jiqinHuikuiDate.ShenZhuangHuiZengQuoto = retValue;
			this.jiqinHuikuiDate.ShenZhuangHuiZengState = Global.SetIntSomeBit(retIndex, this.jiqinHuikuiDate.ShenZhuangHuiZengState, true);
		}
		else if (activityType == 31)
		{
			this.jiqinHuikuiDate.XingYunChouJiangCount = retValue;
		}
	}

	public void LingQuResult2(int result, int roleID, int retValue, int nYuanBao)
	{
		if (result >= 0)
		{
			this.jiqinHuikuiDate.XingYunChouJiangCount = retValue;
			this.jiqinHuikuiDate.XingYunChouJiangYuanBao = nYuanBao;
		}
		this.jiqinHuikuiPart04.OnLingquCompleted(result, this.jiqinHuikuiDate);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GIcon SelectedIcon;

	private Canvas partCanvas = new Canvas();

	private Canvas partCanvas2 = new Canvas();

	private GTextBlockOutLine HuodongTitle;

	private GTextBlockOutLine HuodongIntro;

	private Dictionary<int, string> ItemsDict = new Dictionary<int, string>();

	public JiqinHuikuiPart01 jiqinHuikuiPart01;

	public JiqinHuikuiPart02 jiqinHuikuiPart02;

	public JiqinHuikuiPart03 jiqinHuikuiPart03;

	public JiqinHuikuiPart04 jiqinHuikuiPart04;

	private JiQingHuiKuiData jiqinHuikuiDate;
}
