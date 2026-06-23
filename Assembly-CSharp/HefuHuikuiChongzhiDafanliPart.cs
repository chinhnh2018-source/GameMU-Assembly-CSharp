using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;

public class HefuHuikuiChongzhiDafanliPart : UserControl
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
		this.listBox.Width = 245.0;
		this.listBox.Height = 99.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 1.0);
		Canvas.SetLeft(this.listBox, 163);
		Canvas.SetTop(this.listBox, 20);
		this.ItemCollection = this.listBox.ItemsSource;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("领取奖励");
		gicon.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(gicon, 310);
		Canvas.SetTop(gicon, 118);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteFetchActivityAward(23, 0);
		};
		this.BtnArr[0] = gicon;
		this.Container.Children.Add(this.txtTotalValue);
		Canvas.SetLeft(this.txtTotalValue, 186);
		Canvas.SetTop(this.txtTotalValue, 126);
		this.txtTotalValue.TextColor = new SolidColorBrush(16777215U);
	}

	public void InitPartData()
	{
	}

	private void LoadTopChongzhiData()
	{
		if (this.state == null)
		{
			return;
		}
		this.txtTotalValue.Text = string.Empty + this.state.YuanBao;
		this.ItemCollection.Clear();
		for (int i = 0; i < 5; i++)
		{
			HefuHuikuiChongzhiDafanliListItem hefuHuikuiChongzhiDafanliListItem = U3DUtils.NEW<HefuHuikuiChongzhiDafanliListItem>();
			hefuHuikuiChongzhiDafanliListItem.txtTopRoleNameYesterday.text = this.GetKingNameByPaiHang(i + 1, 0);
			hefuHuikuiChongzhiDafanliListItem.txtTopRoleNameToday.text = this.GetKingNameByPaiHang(i + 1, 1);
			hefuHuikuiChongzhiDafanliListItem.Width = 245.0;
			hefuHuikuiChongzhiDafanliListItem.Height = 19.0;
			this.ItemCollection.Add(hefuHuikuiChongzhiDafanliListItem);
		}
	}

	public void GetData(bool isInLingquTime)
	{
		this.IsInLingquTime = isInLingquTime;
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryHeFuFanLi();
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
			this.LoadTopChongzhiData();
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了昨日充值排名大返利"), 0, -1, -1, 0);
			this.SetBtnState(0, result);
			this.state.State = result;
		}
	}

	private bool IsMeByPaiHang()
	{
		bool result = false;
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHangYestoday = this.state.ListPaiHangYestoday;
		if (listPaiHangYestoday == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHangYestoday.Count; i++)
		{
			if (listPaiHangYestoday[i] != null && Global.Data.UserID == listPaiHangYestoday[i].UserID)
			{
				result = true;
			}
		}
		return result;
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

	private string GetKingNameByPaiHang(int paiHang, int day)
	{
		string result = Global.GetLang("无");
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> list = null;
		if (day == 0)
		{
			list = this.state.ListPaiHangYestoday;
		}
		else if (day == 1)
		{
			list = this.state.ListPaiHang;
		}
		if (list == null)
		{
			return result;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && paiHang == list[i].PaiHang)
			{
				result = Global.FormatRoleName(list[i].MaxLevelRoleZoneID, list[i].MaxLevelRoleName);
				break;
			}
		}
		return result;
	}

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private GIcon[] BtnArr = new GIcon[0];

	private JieriCZKingData state;

	private bool IsInLingquTime;

	public GTextBlockOutLine txtTotalValue = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ObservableCollection _ItemCollection;
}
