using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JiqinHuikuiPart01 : UserControl
{
	public JiqinHuikuiPart01()
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
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 414.0;
		this.listBox.Height = 248.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 14.0, 0.0);
		this.Container.Children.Add(this.listBox);
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
		int num = (this.state != null) ? this.state.AwardID : 0;
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
				if (!(s as GIcon).EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteGetDayChongZhiDaLi(Global.Data.roleData.RoleID, 27, (s as GIcon).ItemCode);
			};
			if (this.IsLingquDengluDali(i))
			{
				gicon.EnableIcon = false;
				gicon.Text = Global.GetLang("已领取");
			}
			else if (num >= Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinYuanBao")))
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
			JiqinHuikuiliListItem01 jiqinHuikuiliListItem = U3DUtils.NEW<JiqinHuikuiliListItem01>();
			jiqinHuikuiliListItem.TextColor = textColor;
			jiqinHuikuiliListItem.TxtItem = StringUtil.substitute(Global.GetLang("每日充值{0}钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			jiqinHuikuiliListItem.BtnLingQu = gicon;
			jiqinHuikuiliListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			this.ItemCollection.Add(jiqinHuikuiliListItem);
		}
	}

	public bool IsLingquDengluDali(int index)
	{
		int resource = (this.state != null) ? this.state.LingquState : 0;
		return Convert.ToBoolean(Global.GetIntSomeBit(resource, index));
	}

	public void GetData()
	{
	}

	public void OnGetDataCompleted(JiQingHuiKuiData result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result != null)
		{
			this.state = new StateObject01();
			this.state.AwardID = result.TodayYuanBao;
			this.state.LingquState = result.TodayYuanBaoState;
			this.LoadDengluDaliJiangLi(this.xmlList);
		}
	}

	public void OnLingquCompleted(int result, JiQingHuiKuiData resdata)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			if (result == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今日累计充值尚未达到1000钻石，无法领取！"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请清理出空格后再领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1111)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日充值大礼包已经领取过了，请明天再来领取！"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2222)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本账号的其他角色已经领取过今日充值大礼包"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取每日充值大礼包时，未知错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了大礼包"), 0, -1, -1, 0);
			this.state = new StateObject01();
			this.state.AwardID = resdata.TodayYuanBao;
			this.state.LingquState = resdata.TodayYuanBaoState;
			this.LoadDengluDaliJiangLi(this.xmlList);
		}
	}

	private ListBox listBox = new ListBox();

	private LoadingWindow LoadingWin;

	private List<XElement> xmlList;

	private StateObject01 state;

	private ObservableCollection _ItemCollection;
}
