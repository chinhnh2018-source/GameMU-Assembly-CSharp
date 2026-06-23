using System;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluTianshengXilianPart : UserControl
{
	public bool Enchancing
	{
		get
		{
			return this._Enchancing;
		}
		set
		{
			this._Enchancing = value;
		}
	}

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
		Canvas.SetLeft(this.Equip, 127);
		Canvas.SetTop(this.Equip, 114);
		this.Equip.Background = new SolidColorBrush(16777215U);
		this.Equip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
		};
		this.Container.Children.Add(this.Rock);
		this.Rock.Width = 32.0;
		this.Rock.Height = 32.0;
		Canvas.SetLeft(this.Rock, 126);
		Canvas.SetTop(this.Rock, 189);
		this.Rock.Background = new SolidColorBrush(16777215U);
		this.TongqianText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.TongqianText.TextFontColor = new SolidColorBrush(3669815U);
		this.TongqianText.Text = "0";
		this.TongqianText.Height = 14.0;
		this.TongqianText.TextSize = 12.0;
		this.TongqianText.Width = 59.0;
		Canvas.SetLeft(this.TongqianText, 168);
		Canvas.SetTop(this.TongqianText, 235);
		this.Container.Children.Add(this.TongqianText);
		this.XilianJieguoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.XilianJieguoText.TextFontColor = new SolidColorBrush(16777080U);
		this.XilianJieguoText.Text = string.Empty;
		this.XilianJieguoText.Height = 14.0;
		this.XilianJieguoText.TextSize = 12.0;
		Canvas.SetLeft(this.XilianJieguoText, 90);
		Canvas.SetTop(this.XilianJieguoText, 5);
		this.Container.Children.Add(this.XilianJieguoText);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("洗 炼");
		gicon.TextColor = new SolidColorBrush(10551295U);
		Canvas.SetLeft(gicon, 104);
		Canvas.SetTop(gicon, 254);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartXiLian();
		};
		this.XiLianBtn = gicon;
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[2];
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.Rock.ItemsSource;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TianshengJihuoGoodsID", ',');
		if (systemParamIntArrayByName.Length == 2)
		{
			this.TianshengJihuoGoodsID = systemParamIntArrayByName[0];
			this.TianshengXilianGoodsID = systemParamIntArrayByName[1];
		}
		this.NeedYinLiang = (int)ConfigSystemParam.GetSystemParamIntByName("TianshengXilianMoney");
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.TongqianText.Text = "0";
		this.XilianJieguoText.Text = string.Empty;
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 2)
		{
			this.equipIcon[index].Clear();
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			if (gd.BornIndex == 0)
			{
				this.XiLianBtn.Text = Global.GetLang("激 活");
				this.AddGoodsIcon(this.TianshengJihuoGoodsID, 1, 1);
			}
			else
			{
				this.XiLianBtn.Text = Global.GetLang("洗 炼");
				this.AddGoodsIcon(this.TianshengXilianGoodsID, 1, 1);
			}
			this.ShowBornAttributeStr(gd.BornIndex);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 0
			});
			this.TongqianText.Text = this.NeedYinLiang.ToString();
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedYinLiang)
			{
				this.TongqianText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
			}
			else
			{
				this.TongqianText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
			}
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

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
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
			gicon.Text = iNeedNub.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(gicon);
		}
	}

	private void StartXiLian()
	{
		if (this.equipIcon[0].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要洗炼的装备放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		if (Global.GetGoodsDataByDbID(goodsData.Id, null) == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗炼的装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedYinLiang)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		int itemCode = U3DUtils.AS<GIcon>(this.equipIcon[1][0]).ItemCode;
		if (Global.GetGoodsDataByID(itemCode) == null)
		{
			if (Super.ShowGoodsGuide(itemCode, null) == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有{0}"), new object[]
				{
					Global.GetGoodsNameByID(itemCode, false)
				}), 19, -1, -1, itemCode);
			}
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteUpdateGoodsBornIndex((U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, itemCode, 0);
	}

	public void OnBornIndexUpdateCompleted(GoodsData goodsData, int result, int dbID, int goodsOldBornIndex, int goodsThisTimeUpdateBornIndex, int goodsNowBornIndex, int binding)
	{
		this.CloseModalDialog();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		string title = goodsXmlNodeByID.Title;
		if (result < 1)
		{
			if (result != 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗练【{0}】时发生错误:{1}"), new object[]
				{
					title,
					result
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio(this.equip_failed, false);
			return;
		}
		this.ShowBornAttributeStr(goodsNowBornIndex);
		Global.PlaySoundAudio(this.equip_ok, false);
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1,
			PlayID = 1
		});
		this.AddEquip(goodsData);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
		}
	}

	private void ShowBornAttributeStr(int bornIndex)
	{
		if (bornIndex == 0)
		{
			this.XilianJieguoText.Text = string.Empty;
		}
		else
		{
			int bornAttackValue = Global.GetBornAttackValue(bornIndex, 0);
			int bornAttackValue2 = Global.GetBornAttackValue(bornIndex, 1);
			int bornAttackValue3 = Global.GetBornAttackValue(bornIndex, 2);
			string text = string.Empty;
			if (bornAttackValue > 0)
			{
				text = text + Global.GetLang("最大物理攻击 +") + bornAttackValue.ToString() + "\n";
			}
			if (bornAttackValue2 > 0)
			{
				text = text + Global.GetLang("最大魔法攻击 +") + bornAttackValue2.ToString() + "\n";
			}
			if (bornAttackValue3 > 0)
			{
				text = text + Global.GetLang("最大道术攻击 +") + bornAttackValue3.ToString() + "\n";
			}
			this.XilianJieguoText.Text = text;
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

	private GTextBlockOutLine TongqianText;

	private GTextBlockOutLine XilianJieguoText;

	private GIcon XiLianBtn;

	private ListBox Equip = new ListBox();

	private ListBox Rock = new ListBox();

	private int TianshengJihuoGoodsID = -1;

	private int TianshengXilianGoodsID = -1;

	private int NeedYinLiang;

	public GDecoration DecoFangruZhuangbei;

	private bool _Enchancing;

	private ObservableCollection[] _equipIcon;

	private string equip_ok = StringUtil.substitute("Media/Music/UI/equip_ok.mp3", new object[0]);

	private string equip_failed = StringUtil.substitute("Media/Music/UI/equip_failed.mp3", new object[0]);
}
