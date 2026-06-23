using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class PetResurrectionPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 38);
		Canvas.SetTop(this.ScrollImg, 44);
	}

	public int PetDbID
	{
		get
		{
			return this._petDbID;
		}
		set
		{
			this._petDbID = value;
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
		gicon.Text = Global.GetLang("复活");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PetResurrectionOk();
		};
		Canvas.SetLeft(gicon, 13);
		Canvas.SetTop(gicon, 89);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		if (this._petDbID == -1)
		{
			return;
		}
		PetData petDataByDbID = Global.GetPetDataByDbID(this._petDbID);
		if (petDataByDbID == null)
		{
			return;
		}
		if (petDataByDbID.PetType == 0)
		{
			this.ShowGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("NoramlPetRealiveGoodsID"));
		}
		else if (petDataByDbID.PetType == 1)
		{
			this.ShowGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("AdvancePetRealiveGoodsID"));
		}
	}

	private bool PetResurrectionOk()
	{
		if (this._petDbID == -1)
		{
			return false;
		}
		PetData petDataByDbID = Global.GetPetDataByDbID(this._petDbID);
		if (petDataByDbID == null)
		{
			return false;
		}
		if (Global.IsPetDead(petDataByDbID))
		{
			if (petDataByDbID.PetType == 0)
			{
				if (Global.GetTotalGoodsCountByID((int)ConfigSystemParam.GetSystemParamIntByName("NoramlPetRealiveGoodsID")) <= 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的宠物已经死亡, 复活宠物需要{0}!"), new object[]
					{
						Global.GetGoodsNameByID((int)ConfigSystemParam.GetSystemParamIntByName("NoramlPetRealiveGoodsID"), false)
					}), 0, -1, -1, 0);
					return false;
				}
			}
			else if (petDataByDbID.PetType == 1 && Global.GetTotalGoodsCountByID((int)ConfigSystemParam.GetSystemParamIntByName("AdvancePetRealiveGoodsID")) <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的宠物已经死亡, 复活宠物需要{0}!"), new object[]
				{
					Global.GetGoodsNameByID((int)ConfigSystemParam.GetSystemParamIntByName("AdvancePetRealiveGoodsID"), false)
				}), 0, -1, -1, 0);
				return false;
			}
			GameInstance.Game.SpriteModPet(this._petDbID, 3, string.Empty);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
			return true;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的宠物不需要复活!"), new object[0]), 0, -1, -1, 0);
		return true;
	}

	private void ShowGoodsIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = Global.GetTotalGoodsCountByID(goodsID).ToString();
			gicon.TextHorizontalAlignment = global::Layout.Center;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			if (Global.GetTotalGoodsCountByID(goodsID) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			this.ScrollImg.Children.Add(gicon);
		}
	}

	private Canvas ScrollImg = new Canvas();

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _petDbID = -1;
}
