using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JiqinHuikuiPart02 : UserControl
{
	public JiqinHuikuiPart02()
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
		uint textColor = 16777080U;
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Roles");
			int num = (this.state != null) ? this.state.ShengyuMinge[i] : 0;
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
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteGetDayChongZhiDaLi(Global.Data.roleData.RoleID, 28, (s as GIcon).ItemCode);
			};
			if (this.IsLingquDengluDali(i))
			{
				gicon.EnableIcon = false;
				gicon.Text = Global.GetLang("已领取");
			}
			else if (Global.Data.roleData.Level >= xelementAttributeInt2 && num < xelementAttributeInt3)
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
			JiqinHuikuiliListItem02 jiqinHuikuiliListItem = U3DUtils.NEW<JiqinHuikuiliListItem02>();
			jiqinHuikuiliListItem.TextColor = textColor;
			jiqinHuikuiliListItem.TxtItem = StringUtil.substitute(Global.GetLang("等级达到{0}级"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinLevel")
			});
			jiqinHuikuiliListItem.TxtItem2 = StringUtil.substitute(Global.GetLang("剩余名额：{0}/{1}"), new object[]
			{
				num,
				Global.GetXElementAttributeStr(xelement, "Roles")
			});
			jiqinHuikuiliListItem.BtnLingQu = gicon;
			jiqinHuikuiliListItem.GoodsList = StringUtil.substitute("{0}|{1}", new object[]
			{
				Global.GetXElementAttributeStr(xelement, "GoodsOne"),
				this.GetItemByGoodsIDs(Global.GetXElementAttributeStr(xelement, "GoodsTwo"))
			});
			this.ItemCollection.Add(jiqinHuikuiliListItem);
		}
	}

	public string GetItemByGoodsIDs(string goodsList)
	{
		string text = StringUtil.trim(goodsList);
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return string.Empty;
		}
		return array[Global.Data.roleData.Occupation];
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
			this.state = new StateObject02();
			this.state.ShengyuMinge = result.ChongJiQingQuShenZhuangQuota;
			this.state.LingquState = result.ChongJiLingQuShenZhuangState;
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
			if (result == -20)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请清理出空格后再领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您当前的等级尚未达到领取等级要求"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1111)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已领取过该等级段奖励,无法再次进行领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -101)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该等级段的奖励名额已满，无法进行领取"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，失败原因{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了大礼包"), 0, -1, -1, 0);
			this.state = new StateObject02();
			this.state.ShengyuMinge = resdata.ChongJiQingQuShenZhuangQuota;
			this.state.LingquState = resdata.ChongJiLingQuShenZhuangState;
			this.LoadDengluDaliJiangLi(this.xmlList);
		}
	}

	private ListBox listBox = new ListBox();

	private LoadingWindow LoadingWin;

	private List<XElement> xmlList;

	private StateObject02 state;

	private ObservableCollection _ItemCollection;
}
