using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JieriChongzhikingPart : UserControl
{
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
		this.listBox.Width = 421.0;
		this.listBox.Height = 119.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 7.0);
		Canvas.SetLeft(this.listBox, 0);
		Canvas.SetTop(this.listBox, 6);
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.Children.Add(this.txtTotal);
		Canvas.SetLeft(this.txtTotal, 44);
		Canvas.SetTop(this.txtTotal, 128);
		this.txtTotal.TextColor = new SolidColorBrush(3669815U);
		this.txtTotal.Text = Global.GetLang("您活动期间累计充值：");
		this.Container.Children.Add(this.txtTotalValue);
		Canvas.SetLeft(this.txtTotalValue, 166);
		Canvas.SetTop(this.txtTotalValue, 126);
		this.txtTotalValue.TextColor = new SolidColorBrush(16777215U);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("领取奖励");
		gicon.TextColor = new SolidColorBrush(16777080U);
		gicon.Visibility = false;
		Canvas.SetLeft(gicon, 290);
		Canvas.SetTop(gicon, 120);
		this.Container.Children.Add(gicon);
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
				GameInstance.Game.SpriteFetchActivityAward(16, 0);
				return;
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不在领取时间范围内，无法领取"), 0, -1, -1, 0);
		};
		this.BtnArr[0] = gicon;
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
		this.txtTotalValue.Text = string.Empty + this.state.YuanBao;
		this.ItemCollection.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			JieriKingListItem jieriKingListItem = U3DUtils.NEW<JieriKingListItem>();
			jieriKingListItem.txtRoleName.text = this.GetKingNameByPaiHang(i + 1);
			jieriKingListItem.txtCondition.text = StringUtil.substitute(Global.GetLang("最低充值 {0} 钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			jieriKingListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			jieriKingListItem.Width = 414.0;
			jieriKingListItem.Height = 32.0;
			this.ItemCollection.Add(jieriKingListItem);
		}
	}

	public void GetData(bool isInLingquTime)
	{
		this.IsInLingquTime = isInLingquTime;
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryJieriCZKing();
	}

	public void OnGetDataCompleted(JieriCZKingData result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result != null)
		{
			this.state = result;
			this.SetBtnState(0, this.state.State);
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了充值王大礼包"), 0, -1, -1, 0);
			this.SetBtnState(0, result);
		}
	}

	private void SetBtnState(int id, int flag)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].Visibility = true;
			if (this.IsMeByPaiHang())
			{
				if (flag > 0)
				{
					this.BtnArr[id].Text = Global.GetLang("已领取");
					this.BtnArr[id].EnableIcon = false;
				}
				else
				{
					this.BtnArr[id].Text = Global.GetLang("领取");
					this.BtnArr[id].EnableIcon = true;
				}
			}
			else
			{
				this.BtnArr[id].Text = Global.GetLang("领取");
				this.BtnArr[id].EnableIcon = false;
			}
		}
	}

	private bool IsMeByPaiHang()
	{
		bool result = false;
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHang = this.state.ListPaiHang;
		if (listPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHang.Count; i++)
		{
			if (listPaiHang[i] != null && Global.Data.UserID == listPaiHang[i].UserID)
			{
				result = true;
			}
		}
		return result;
	}

	private string GetKingNameByPaiHang(int paiHang)
	{
		string result = Global.GetLang("无");
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHang = this.state.ListPaiHang;
		if (listPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHang.Count; i++)
		{
			if (listPaiHang[i] != null && paiHang == listPaiHang[i].PaiHang)
			{
				result = Global.FormatRoleName(listPaiHang[i].MaxLevelRoleZoneID, listPaiHang[i].MaxLevelRoleName);
				break;
			}
		}
		return result;
	}

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private GIcon[] BtnArr = new GIcon[0];

	private List<XElement> xmlList;

	private JieriCZKingData state;

	private bool IsInLingquTime;

	public GTextBlockOutLine txtTotal = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtTotalValue = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ObservableCollection _ItemCollection;
}
