using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluQianghuaFenliPart : UserControl
{
	public ObservableCollection[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 32.0;
		this.Equip.Height = 32.0;
		this.Equip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Equip, 126);
		Canvas.SetTop(this.Equip, 114);
		this.Equip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
		};
		this.YinLiangText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.YinLiangText.TextFontColor = new SolidColorBrush(3669815U);
		this.YinLiangText.Text = "0";
		this.YinLiangText.Height = 14.0;
		this.YinLiangText.TextSize = 12.0;
		this.YinLiangText.Width = 30.0;
		Canvas.SetLeft(this.YinLiangText, 168);
		Canvas.SetTop(this.YinLiangText, 235);
		this.Container.Children.Add(this.YinLiangText);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("分 离");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartFenli();
		};
		Canvas.SetLeft(gicon, 104);
		Canvas.SetTop(gicon, 254);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[1];
		this.equipIcon[0] = this.Equip.ItemsSource;
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.YinLiangText.Text = "0";
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 1)
		{
			this.equipIcon[index].Clear();
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 0
			});
			if (gd.Forge_level > 0)
			{
				this.NeedYinLiang = Global.GetQianghuashiFenliMoney(gd.Forge_level);
			}
			else
			{
				this.NeedYinLiang = 0;
			}
			this.YinLiangText.Text = this.NeedYinLiang.ToString();
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = 12;
			ggoodIcon.Text = ((gd.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
			{
				gd.Forge_level.ToString()
			}));
			ggoodIcon.TextSize = 12;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 4294901760U;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool liuguang = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitEquipGIcon(ggoodIcon, gd, liuguang, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(ggoodIcon);
		}
	}

	private void StartFenli()
	{
		if (this.equipIcon[0].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要分离装备放到装备框中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要分离的装备不在背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (goodsData.Forge_level <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要分离的装备强化等级为0"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (!Global.CanTakeNewGoodsByGridNum(goodsData.Forge_level))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包空格不足, 无法分离装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedYinLiang)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteEquipmentFenJie(goodsData.Id);
	}

	public void OnQianghuaFenliCompleted(int result, int dbID)
	{
		this.CloseModalDialog();
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		if (result < 1)
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("分离失败，要分离的装备强化等级为0"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("分离失败，要分离的装备不在背包中"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			else if (result == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("分离时扣除{0}金币失败"), new object[]
				{
					this.NeedYinLiang
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("分离时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio(this.equip_failed, false);
			return;
		}
		Global.PlaySoundAudio(this.equip_ok, false);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功地将强化石分离了出来"), new object[0]), 0, -1, -1, 0);
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1,
			PlayID = 1
		});
		this.AddEquip(goodsDataByDbID);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
		}
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	private Canvas PlaceHolder;

	private ListBox Equip = new ListBox();

	private GTextBlockOutLine YinLiangText;

	private int NeedYinLiang;

	private ObservableCollection[] _equipIcon;

	private string equip_ok = StringUtil.substitute("Media/Music/UI/equip_ok.mp3", new object[0]);

	private string equip_failed = StringUtil.substitute("Media/Music/UI/equip_failed.mp3", new object[0]);
}
