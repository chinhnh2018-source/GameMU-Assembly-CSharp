using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class TianShengXiLianPart : UserControl
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

	public void InitPartData(GoodsData goodsData)
	{
		int oldBornIndex = 0;
		if (goodsData != null)
		{
			oldBornIndex = goodsData.BornIndex;
		}
		int num;
		int needNum;
		int needYinLiang;
		if (!Global.GetEquipBornUpdateParams(oldBornIndex, out num, out needNum, out needYinLiang))
		{
			return;
		}
		this.NeedNum = needNum;
		this.NeedYinLiang = needYinLiang;
		int num2 = num;
		this.needGoodsID = num2;
		if (num2 < 0)
		{
			return;
		}
		this.needTianshi.text = needNum.ToString();
		this.needYingLiang.text = needYinLiang.ToString();
		this.AddGoodsIcon(num2, needNum);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 23);
		Canvas.SetTop(gicon, 17);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("洗炼");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		Canvas.SetLeft(gicon, 242);
		Canvas.SetTop(gicon, 380);
		this.Container.Children.Add(gicon);
		this.XiLianBtn = gicon;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartXiLian();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("替换");
		gicon.TextColor = new SolidColorBrush(10551295U);
		Canvas.SetLeft(gicon, 242);
		Canvas.SetTop(gicon, 380);
		this.Container.Children.Add(gicon);
		gicon.Visibility = false;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartTiHuan();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 51.0;
		gicon.Height = 29.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/zbtp_qualityPointer.png"));
		gicon.DisableHandCursor = true;
		Canvas.SetLeft(gicon, 80);
		Canvas.SetTop(gicon, 56);
		this.Container.Children.Add(gicon);
		this.tianshenLiangIcon = gicon;
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "UsingRocks";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("材料不足时自动购买");
		gcheckBox.Check = false;
		gcheckBox.TextColor = new SolidColorBrush(7448500U);
		Canvas.SetLeft(gcheckBox, 33);
		Canvas.SetTop(gcheckBox, 383);
		this.Container.Children.Add(gcheckBox);
		this.AutoBuyCheckBox = gcheckBox;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("0%");
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 93);
		Canvas.SetTop(gtextBlockOutLine, 267);
		this.Container.Children.Add(gtextBlockOutLine);
		this.gTianshenOld = gtextBlockOutLine;
		this.gTianshenOld.Visibility = false;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("0%");
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 150);
		Canvas.SetTop(gtextBlockOutLine, 255);
		this.Container.Children.Add(gtextBlockOutLine);
		this.gTianshenNew = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("0");
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 263);
		Canvas.SetTop(gtextBlockOutLine, 302);
		this.Container.Children.Add(gtextBlockOutLine);
		this.needTianshi = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("50");
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 263);
		Canvas.SetTop(gtextBlockOutLine, 326);
		this.Container.Children.Add(gtextBlockOutLine);
		this.needYingLiang = gtextBlockOutLine;
	}

	public override void Destroy()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("天生洗炼UI"), null);
		ObjectClickGetingMgr.CancelClickGetThing(13);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ObjectClickEvent objectClickEvent = evt.Tag as ObjectClickEvent;
		if (objectClickEvent.ClickGetThingType != ClickGetThingTypes.TianSheng)
		{
			return;
		}
		if (this.Enchancing)
		{
			return;
		}
		int clickGetThingDbID = objectClickEvent.ClickGetThingDbID;
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		this.selectedGoods = goodsDataByDbID;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsDataByDbID.Id,
					0
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
				ggoodIcon.ItemObject = goodsDataByDbID;
				ggoodIcon.BoxTypes = 6;
				ggoodIcon.Text = ((goodsDataByDbID.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsDataByDbID.Forge_level.ToString()
				}));
				ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.TextVerticalAlignment = global::Layout.Top;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4294901760U;
				Canvas.SetLeft(this.tianshenLiangIcon, 80 + 50 * Global.GetBornIndexNameLevel(goodsDataByDbID));
				Super.InitEquipGIcon(ggoodIcon, goodsDataByDbID, false, IconTextTypes.Qianghua);
				Canvas.SetLeft(ggoodIcon, 40);
				Canvas.SetTop(ggoodIcon, 55);
				this.Container.Children.Add(ggoodIcon);
				this.gTianshenNew.Text = goodsDataByDbID.BornIndex + "%";
				this.gTianshenOld.text = goodsDataByDbID.BornIndex + "%";
				this.InitPartData(goodsDataByDbID);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有装备才能洗炼"), new object[0]), 0, -1, -1, 0);
			}
		}
		if (this.ToHintStartEnchance)
		{
			Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("天生洗炼UI"), 650000, 0, 1);
		}
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (Super.RemoveSystemNaviBox(this.Container, Global.GetLang("天生洗炼UI"), null))
		{
			this.ToHintStartEnchance = true;
			string taskPropNameByID = Global.GetTaskPropNameByID(170403);
			string title = null;
			int num = 0;
			int num2 = 0;
			Global.ParsePropNameInfo(taskPropNameByID, out title, out num, out num2);
			int num3 = ConfigGoods.FindGoodsIDByName(title);
		}
		(sender as GIcon).Cursor = Cursors.Auto;
		ObjectClickGetingMgr.StartClickGetThing(13, e);
	}

	private void AddGoodsIcon(int goodsID, int needNum)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = needNum.ToString();
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
			Canvas.SetLeft(gicon, 21);
			Canvas.SetTop(gicon, 305);
			this.Container.Children.Add(gicon);
		}
	}

	private void StartXiLian()
	{
		if (this.selectedGoods != null)
		{
			this.XiLianBtn.EnableIcon = false;
			GameInstance.Game.SpriteUpdateGoodsBornIndex(this.selectedGoods.Id, this.needGoodsID, (!this.AutoBuyCheckBox.Check) ? 0 : 1);
		}
	}

	private void StartTiHuan()
	{
	}

	public void OnBornIndexUpdateCompleted(GoodsData goodsData, int result, int dbID, int goodsOldBornIndex, int goodsThisTimeUpdateBornIndex, int goodsNowBornIndex, int binding)
	{
		this.XiLianBtn.EnableIcon = true;
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
				if (result == -4)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要洗练的【{0}】不在背包中"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -6)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的天缘石数量不足, 需要{0}块天缘石"), new object[]
					{
						this.NeedNum
					}), 19, -1, -1, this.needGoodsID);
				}
				else if (result == -7)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币数量不足, 需要{0}金币"), new object[]
					{
						this.NeedYinLiang
					}), 0, -1, -1, 0);
				}
				else if (result == -8)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别，无法洗练"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -11)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗练时扣除{0}两银子失败"), new object[]
					{
						this.NeedYinLiang
					}), 0, -1, -1, 0);
				}
				else if (result == -2300)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("自动购物时钻石不足"), new object[0]), 0, -1, -1, 0);
				}
				else if (result == -5)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】佩戴在身上时无法洗练"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗练【{0}】时发生错误:{1}"), new object[]
					{
						title,
						result
					}), 0, -1, -1, 0);
				}
			}
			return;
		}
		this.gTianshenNew.Text = goodsData.BornIndex + "%";
		if (goodsOldBornIndex < goodsNowBornIndex)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功地将【{0}】的天生属性洗练到了{1}%"), new object[]
			{
				title,
				goodsData.BornIndex
			}), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】的天生属性洗练到{1}%, 比原值低，保留原值"), new object[]
			{
				title,
				goodsThisTimeUpdateBornIndex
			}), 0, -1, -1, 0);
		}
		Canvas.SetLeft(this.tianshenLiangIcon, 80 + 50 * Global.GetBornIndexNameLevel(goodsData));
		this.InitPartData(goodsData);
	}

	private GIcon tianshenLiangIcon;

	private GTextBlockOutLine gTianshenOld;

	private GTextBlockOutLine gTianshenNew;

	private GTextBlockOutLine needTianshi;

	private GTextBlockOutLine needYingLiang;

	private GoodsData selectedGoods;

	private int needGoodsID = -1;

	private GCheckBox AutoBuyCheckBox;

	private int NeedNum;

	private int NeedYinLiang;

	private GIcon XiLianBtn;

	private bool _Enchancing;

	private bool ToHintStartEnchance;
}
