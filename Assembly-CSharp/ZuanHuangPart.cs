using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ZuanHuangPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	protected override void InitializeComponent()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 87.0;
		gicon.Height = 40.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/cz_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/cz_hover.png"));
		Canvas.SetLeft(gicon, 114);
		Canvas.SetTop(gicon, 22);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
		this.leveText.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.leveText, 125);
		Canvas.SetTop(this.leveText, 133);
		this.leveText.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.infoText);
		this.infoText.FontSize = HSTextField.defaultFontSize;
		this.infoText.BodyWidth = 190.0;
		this.infoText.TextWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.infoText, 30);
		Canvas.SetTop(this.infoText, 83);
		this.infoText.TextColor = new SolidColorBrush(16777080U);
		Super.FormatTextBlockEx2(this.infoText, StringUtil.substitute(Global.GetLang("您只需再充值 ｛color=#0x3ace00 uline=false tag= text={0}｝ 钻石即可成为钻皇 ｛color=#0x3ace00 uline=false tag= text={1}｝ 级用户。"), new object[]
		{
			0,
			0
		}));
		this.leveProgressBar = new GImgProgressBar();
		this.leveProgressBar.BodyWidth = 138.0;
		this.leveProgressBar.BodyHeight = 10.0;
		this.leveProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/zuanhuangBar.png");
		this.leveProgressBar.ProgressBar_Size = new SizeSL(138.0, 10.0);
		this.leveProgressBar.ProgressBar_pos = new Point(0, 0);
		this.Container.Children.Add(this.leveProgressBar);
		Canvas.SetLeft(this.leveProgressBar, 91);
		Canvas.SetTop(this.leveProgressBar, 67);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 92);
		Canvas.SetTop(gtextBlockOutLine, 171);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiajieZhuanhuang_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 92);
		Canvas.SetTop(gtextBlockOutLine, 195);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiajieZhuanhuang_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 92);
		Canvas.SetTop(gtextBlockOutLine, 219);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiajieZhuanhuang_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 92);
		Canvas.SetTop(gtextBlockOutLine, 243);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiajieZhuanhuang_WuliFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 92);
		Canvas.SetTop(gtextBlockOutLine, 267);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiajieZhuanhuang_MofaFangyuText = gtextBlockOutLine;
	}

	public void RefreshData()
	{
		GameInstance.Game.SpriteFetchZuanHuangWeekAward(1);
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZuanHuangLevel);
		this.SetJingLi(roleCommonUseParamsValue);
	}

	private void SetJingLi(int leve)
	{
		this.iLeve = leve;
		XElement xelement = Global.GetXElement(this.xmlRoot, "Level", "ID", this.iLeve.ToString());
		if (xelement == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinYuanBao");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "WeekBindYuanBao");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "WeekGoods");
		if (this.iLeve >= this.iMaxLeve)
		{
			return;
		}
		int index = leve + 1;
		XElement xelement2 = Global.GetXElement(this.xmlRoot, "Level", "ID", index.ToString());
		if (xelement2 == null)
		{
			return;
		}
		this.iNextMinYuanBao = Global.GetXElementAttributeInt(xelement2, "MinYuanBao");
		double[] rolePropList = this.GetRolePropList(index);
		this.XiajieZhuanhuang_WuliGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[7],
			rolePropList[8]
		});
		this.XiajieZhuanhuang_MofaGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
		this.XiajieZhuanhuang_DaoshuGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
		this.XiajieZhuanhuang_WuliFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[3],
			rolePropList[4]
		});
		this.XiajieZhuanhuang_MofaFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[5],
			rolePropList[6]
		});
	}

	private GGoodIcon Geticon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int bornInex)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		GoodsData goodsData = Global.AddGiftGoodsData(goodsID, forgeLevel, quality, binding, gcount, bornInex);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			12
		});
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	protected void UpdateTotalInputYuanBao(double totalYuanBao)
	{
		if (this.iLeve >= this.iMaxLeve)
		{
			this.iLeve = this.iMaxLeve;
			this.leveProgressBar.Percent = 1.0;
			this.infoText.Text = StringUtil.substitute(Global.GetLang("敬爱的玩家，您目前的钻皇等级已达顶级！"), new object[0]);
		}
		else
		{
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZuanHuangLevel);
			Super.FormatTextBlockEx2(this.infoText, StringUtil.substitute(Global.GetLang("您当前钻皇等级为 ｛color=#0x3ace00 uline=false tag= text={0}｝ 级只需再充值 ｛color=#0x3ace00 uline=false tag= text={1}｝ 钻石即可成为钻皇 ｛color=#0x3ace00 uline=false tag= text={2}｝ 级用户。"), new object[]
			{
				Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZuanHuangLevel),
				(int)((double)this.iNextMinYuanBao - totalYuanBao),
				this.iLeve + 1
			}));
			if (this.iNextMinYuanBao != 0)
			{
				this.leveProgressBar.Percent = totalYuanBao / (double)this.iNextMinYuanBao;
			}
		}
	}

	public void OnZuanHuangWeekAwardGetCompleted(int result, double totalYuanBao)
	{
		this.UpdateTotalInputYuanBao(totalYuanBao);
		if (result >= 0)
		{
			if (result == 6000)
			{
				if (this.LingQuIcon != null)
				{
					this.LingQuIcon.EnableIcon = true;
				}
				return;
			}
			if (result != 7000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取成功"), new object[0]), 0, -1, -1, 0);
			}
			if (this.LingQuIcon != null)
			{
				this.LingQuIcon.EnableIcon = false;
			}
		}
		else if (result == -3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本周已经领取过了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -31)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包空间不够"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的钻皇等级为0，不能领取"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励时发生错误{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	private double[] GetRolePropList(int index)
	{
		int zhuanhuangBuffID = Global.GetZhuanhuangBuffID(index);
		return Global.GetGoodsEquipPropsDoubleList(zhuanhuangBuffID);
	}

	private GTextBlockEx leveText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GTextBlockEx infoText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GTextBlockOutLine bdYbText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GImgProgressBar leveProgressBar = new GImgProgressBar();

	private GIcon LingQuIcon;

	private int iLeve;

	private int iNextMinYuanBao;

	private int iMaxLeve = Global.MaxZhuanhuangLevel;

	private GTextBlockOutLine XiajieZhuanhuang_WuliGongjiText;

	private GTextBlockOutLine XiajieZhuanhuang_MofaGongjiText;

	private GTextBlockOutLine XiajieZhuanhuang_DaoshuGongjiText;

	private GTextBlockOutLine XiajieZhuanhuang_WuliFangyuText;

	private GTextBlockOutLine XiajieZhuanhuang_MofaFangyuText;

	private XElement xmlRoot = Global.GetGameResXml("Config/ZuanHuang.Xml");
}
