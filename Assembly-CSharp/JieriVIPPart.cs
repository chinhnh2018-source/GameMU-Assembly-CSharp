using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JieriVIPPart : UserControl
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
		this.listBox.Width = 200.0;
		this.listBox.Height = 32.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 14.0, 0.0);
		Canvas.SetLeft(this.listBox, 115);
		Canvas.SetTop(this.listBox, 74);
		this.ItemCollection = this.listBox.ItemsSource;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("领取奖励");
		gicon.TextColor = new SolidColorBrush(16777080U);
		gicon.Visibility = false;
		Canvas.SetLeft(gicon, 155);
		Canvas.SetTop(gicon, 115);
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
				GameInstance.Game.SpriteFetchActivityAward(11, 0);
				return;
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不在领取时间范围内，无法领取"), 0, -1, -1, 0);
		};
		this.BtnArr[0] = gicon;
	}

	public void InitPartData(List<XElement> xmlList)
	{
		XElement xelement = xmlList[0];
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
				this.AddGoodsIcon(i, int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), -1);
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
				Canvas.SetLeft(ggoodIcon, 176);
				Canvas.SetTop(ggoodIcon, 12);
				return;
			}
			this.ItemCollection.AddNoUpdate(ggoodIcon);
		}
	}

	public void GetData(bool isInLingquTime)
	{
		this.IsInLingquTime = isInLingquTime;
		if (this.state < 0)
		{
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteQueryJieriVIP();
		}
		else
		{
			this.SetBtnState(0);
		}
	}

	public void OnGetDataCompleted(int result, int isVip)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			this.state = result;
			this.SetBtnState(0);
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了VIP大礼包"), 0, -1, -1, 0);
			this.state = result;
			this.SetBtnState(0);
		}
	}

	private void SetBtnState(int id)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].Visibility = true;
			if (this.state > 0)
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
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private int state = -1;

	private bool IsInLingquTime;

	private GIcon[] BtnArr = new GIcon[0];

	private ObservableCollection _ItemCollection;
}
