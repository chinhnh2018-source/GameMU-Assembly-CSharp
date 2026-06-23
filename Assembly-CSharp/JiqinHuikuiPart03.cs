using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JiqinHuikuiPart03 : UserControl
{
	public JiqinHuikuiPart03()
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
		this.listBox.Width = 200.0;
		this.listBox.Height = 32.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 14.0, 0.0);
		Canvas.SetLeft(this.listBox, 122);
		Canvas.SetTop(this.listBox, 113);
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.Children.Add(this.txtItem);
		this.txtItem.BodyWidth = 86.0;
		Canvas.SetLeft(this.txtItem, 220);
		Canvas.SetTop(this.txtItem, 163);
		this.txtItem.TextColor = new SolidColorBrush(16777080U);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("领取奖励");
		gicon.TextColor = new SolidColorBrush(16777080U);
		gicon.Visibility = false;
		Canvas.SetLeft(gicon, 165);
		Canvas.SetTop(gicon, 195);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteGetDayChongZhiDaLi(Global.Data.roleData.RoleID, 29, (s as GIcon).ItemCode);
		};
		this.BtnArr[0] = gicon;
	}

	public void InitPartData(List<XElement> xml)
	{
		XElement xelement = xml[0];
		this.minge = Global.GetXElementAttributeInt(xelement, "Roles");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
		string text = StringUtil.trim(xelementAttributeStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 6)
			{
				this.AddGoodsIcon(i, Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[5]), -1);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int i, int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataEx = Global.GetDummyGoodsDataEx(goodsID, forgeLevel, quality, binding, gcount, born);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			string goodsImageURLFromIconCodeEx = Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID));
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = ((i != 0) ? new ImageURL(goodsImageURLFromIconCode, false, 0) : new ImageURL(goodsImageURLFromIconCodeEx, false, 2));
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				default(object),
				-1
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = dummyGoodsDataEx;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			if (Global.IsForgeRockGoodsID(goodsID))
			{
				ggoodIcon.STextVisibility = true;
				ggoodIcon.SText = StringUtil.substitute("{0}", new object[]
				{
					Global.GetForgeRockLevelNames(goodsID)
				});
				ggoodIcon.STextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.STextVerticalAlignment = global::Layout.Top;
				ggoodIcon.STextColor = uint.MaxValue;
				ggoodIcon.STextShadowColor = 24831U;
			}
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataEx, true, IconTextTypes.Qianghua);
			if (i == 0)
			{
				this.Container.Children.Add(ggoodIcon);
				Canvas.SetLeft(ggoodIcon, 183);
				Canvas.SetTop(ggoodIcon, 51);
				return;
			}
			this.ItemCollection.AddNoUpdate(ggoodIcon);
		}
	}

	public bool IsLingquDengluDali(int index)
	{
		int resource = (this.state != null) ? this.state.ShenZhuangHuiZengState : 0;
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
			this.state = new StateObject03();
			this.state.ShenZhuangHuiZengState = result.ShenZhuangHuiZengState;
			this.state.ShengyuMinge = result.ShenZhuangHuiZengQuoto;
			this.txtItem.Text = StringUtil.substitute("{0}/{1}", new object[]
			{
				this.state.ShengyuMinge,
				this.minge
			});
			this.SetBtnState(0);
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
			else if (result == -102)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您尚未从NPC·神兵神甲处购买任意一件装备,无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -103)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("对不起 您已经领取过神装回馈大礼"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -104)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("神器回馈奖励名额已满，无法进行领取"), new object[0]), 0, -1, -1, 0);
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
			this.state = new StateObject03();
			this.state.ShenZhuangHuiZengState = resdata.ShenZhuangHuiZengState;
			this.state.ShengyuMinge = resdata.ShenZhuangHuiZengQuoto;
			this.txtItem.Text = StringUtil.substitute("{0}/{1}", new object[]
			{
				this.state.ShengyuMinge,
				this.minge
			});
			this.SetBtnState(0);
		}
	}

	private void SetBtnState(int id)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].Visibility = true;
			if (this.state != null)
			{
				this.BtnArr[id].Text = Global.GetLang("领取");
				if (this.state.ShengyuMinge < this.minge && !this.IsLingquDengluDali(0))
				{
					this.BtnArr[id].EnableIcon = true;
				}
				else
				{
					this.BtnArr[id].EnableIcon = false;
				}
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine txtItem = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int minge;

	private StateObject03 state;

	private List<GIcon> BtnArr = new List<GIcon>();

	private ObservableCollection _ItemCollection;
}
