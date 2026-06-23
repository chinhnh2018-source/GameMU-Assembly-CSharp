using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class JieriLeijiPart : UserControl
{
	public JieriLeijiPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 415.0;
		this.listBox.Height = 161.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 14.0, 0.0);
		this.listBox.ScrollBarBak = Global.GetLoginResImage("Images/Plate/ScrollBar2.png");
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.listBox, 0);
		Canvas.SetTop(this.listBox, 0);
	}

	public void InitPartData(List<XElement> xml)
	{
		this.xmlList = xml;
	}

	private void LoadDengluDaliJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_normal.png"), 50.0, 29.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_hover.png"), 50.0, 29.0, 3.0, 2.0));
		ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_nouse.png"), 50.0, 29.0, 3.0, 2.0));
		int num = (this.state != null) ? this.state.roleYuanBaoInPeriod : 0;
		uint textColor = 16777080U;
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 50.0;
			gicon.Height = 29.0;
			gicon.BodySource = bodySource;
			gicon.NewSource = newSource;
			gicon.DisableBodySource = disableBodySource;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Name = "LingquBtn_" + xelementAttributeInt;
			gicon.ItemCode = xelementAttributeInt;
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				if (this.IsInLingquTime)
				{
					this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
					this.Container.Children.Add(this.LoadingWin);
					GameInstance.Game.SpriteFetchActivityAward(13, (s as GIcon).ItemCode);
					return;
				}
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不在领取时间范围内，无法领取"), 0, -1, -1, 0);
			};
			this.BtnArr[xelementAttributeInt] = gicon;
			if (this.IsLingquDengluDali(i))
			{
				gicon.EnableIcon = false;
				gicon.Text = Global.GetLang("已领取");
			}
			else if (Global.GetXElementAttributeInt(xelement, "MinYuanBao") <= num)
			{
				gicon.Text = Global.GetLang("领取");
				gicon.EnableIcon = true;
			}
			else
			{
				textColor = 8421504U;
				gicon.Text = Global.GetLang("未领取");
				gicon.EnableIcon = false;
			}
			JieriDengluDaliJiangliListItem jieriDengluDaliJiangliListItem = U3DUtils.NEW<JieriDengluDaliJiangliListItem>();
			jieriDengluDaliJiangliListItem.TxtDayNumWidth = 120;
			jieriDengluDaliJiangliListItem.TextColor = textColor;
			jieriDengluDaliJiangliListItem.DayNum = StringUtil.substitute(Global.GetLang("累计 {0} 钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			jieriDengluDaliJiangliListItem.BtnLingQu = gicon;
			jieriDengluDaliJiangliListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			this.ItemCollection.Add(jieriDengluDaliJiangliListItem);
		}
	}

	public bool IsLingquDengluDali(int index)
	{
		int resource = (this.state != null) ? this.state.GiftState : 0;
		return bool.Parse(string.Empty + Global.GetIntSomeBit(resource, index));
	}

	public void GetData(bool isInLingquTime)
	{
		this.IsInLingquTime = isInLingquTime;
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryLeijiCZ();
	}

	public void OnGetDataCompleted(int roleYuanBaoInPeriod, int giftState)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		JieriLeijiPart.JustForParamsTrans justForParamsTrans = new JieriLeijiPart.JustForParamsTrans
		{
			roleYuanBaoInPeriod = roleYuanBaoInPeriod,
			GiftState = giftState
		};
		if (justForParamsTrans != null)
		{
			this.state = justForParamsTrans;
			this.LoadDengluDaliJiangLi(this.xmlList);
		}
	}

	public void OnLingquCompleted(int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了大礼包"), 0, -1, -1, 0);
			this.state.GiftState = result;
			this.LoadDengluDaliJiangLi(this.xmlList);
		}
	}

	private ListBox listBox = new ListBox();

	private List<XElement> xmlList;

	private JieriLeijiPart.JustForParamsTrans state;

	private bool IsInLingquTime;

	private GIcon[] BtnArr = new GIcon[0];

	private LoadingWindow LoadingWin;

	private ObservableCollection _ItemCollection;

	public class JustForParamsTrans
	{
		public int GiftState;

		public int roleYuanBaoInPeriod;
	}
}
