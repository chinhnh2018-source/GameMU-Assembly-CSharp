using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class WuxuePart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.AddEffect();
		this.WuxueJihuoTiaojianBak = new Canvas();
		this.WuxueJihuoTiaojianBak.Width = 158.0;
		this.WuxueJihuoTiaojianBak.Height = 153.0;
		this.WuxueJihuoTiaojianBak.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/wuxueJihuoTiaojian_bak.png"), false, 0);
		Canvas.SetLeft(this.WuxueJihuoTiaojianBak, 180);
		Canvas.SetTop(this.WuxueJihuoTiaojianBak, 234);
		this.Container.Children.Add(this.WuxueJihuoTiaojianBak);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 265);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliMianshangText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 289);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaMianshangText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 313);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_ShengmingShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 338);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 362);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 77);
		Canvas.SetTop(gtextBlockOutLine, 386);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 265);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_WuliMianshangText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 289);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_MofaMianshangText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 313);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_ShengmingShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 338);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 362);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 411);
		Canvas.SetTop(gtextBlockOutLine, 386);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 234);
		Canvas.SetTop(gtextBlockOutLine, 311);
		this.Container.Children.Add(gtextBlockOutLine);
		this.JihuoTiaojianLevelText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 234);
		Canvas.SetTop(gtextBlockOutLine, 335);
		this.Container.Children.Add(gtextBlockOutLine);
		this.JihuoTiaojianZhengqiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 195);
		Canvas.SetTop(gtextBlockOutLine, 205);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjieText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(11394222U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.fontBold = true;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 189);
		Canvas.SetTop(gtextBlockOutLine, 240);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianRealJingjieText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(11394222U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.fontBold = true;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 286);
		Canvas.SetTop(gtextBlockOutLine, 240);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiRealJingjieText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 184);
		Canvas.SetTop(gtextBlockOutLine, 257);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianRealJingjieNameText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 282);
		Canvas.SetTop(gtextBlockOutLine, 257);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiRealJingjieNameText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 75);
		Canvas.SetTop(gtextBlockOutLine, 22);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TotalWuxingText = gtextBlockOutLine;
		this.JihuoIcon = U3DUtils.NEW<GIcon>();
		this.JihuoIcon.Width = 81.0;
		this.JihuoIcon.Height = 21.0;
		this.JihuoIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.JihuoIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.JihuoIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.JihuoIcon.Text = Global.GetLang("立即激活");
		this.JihuoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartJihuo();
		};
		Canvas.SetLeft(this.JihuoIcon, 220);
		Canvas.SetTop(this.JihuoIcon, 380);
		this.Container.Children.Add(this.JihuoIcon);
		this.PrevPageIcon = U3DUtils.NEW<GIcon>();
		this.PrevPageIcon.Width = 16.0;
		this.PrevPageIcon.Height = 21.0;
		this.PrevPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_normal.png"));
		this.PrevPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_hover.png"));
		this.PrevPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_nouse.png"));
		this.PrevPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PrevWuxueImg();
		};
		Canvas.SetLeft(this.PrevPageIcon, 26);
		Canvas.SetTop(this.PrevPageIcon, 84);
		this.Container.Children.Add(this.PrevPageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 51.0;
		this.NextPageIcon.Height = 51.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NextWuxueImg();
		};
		Canvas.SetLeft(this.NextPageIcon, 442);
		Canvas.SetTop(this.NextPageIcon, 84);
		this.Container.Children.Add(this.NextPageIcon);
		this.TianrenHeyiIcon = U3DUtils.NEW<GIcon>();
		this.TianrenHeyiIcon.Width = 81.0;
		this.TianrenHeyiIcon.Height = 21.0;
		this.TianrenHeyiIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.TianrenHeyiIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.TianrenHeyiIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.TianrenHeyiIcon.Text = Global.GetLang("武学巅峰");
		this.TianrenHeyiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowTianrenHeyiWindow();
		};
		Canvas.SetLeft(this.TianrenHeyiIcon, 220);
		Canvas.SetTop(this.TianrenHeyiIcon, 380);
		this.Container.Children.Add(this.TianrenHeyiIcon);
		this.TianrenHeyiIcon.Visibility = false;
		this.Container.Children.Add(this.WuxueDianfengBak);
		this.WuxueDianfengBak.Width = 113.0;
		this.WuxueDianfengBak.Height = 138.0;
		Canvas.SetLeft(this.WuxueDianfengBak, 208);
		Canvas.SetTop(this.WuxueDianfengBak, 239);
		this.wuxueCanvas = new Sprite();
		this.wuxueCanvas.Width = 360.0;
		this.wuxueCanvas.Height = 200.0;
		this.wuxueCanvas.X = 79.0;
		this.wuxueCanvas.Y = 11.0;
		this.Container.Children.Add(this.wuxueCanvas);
	}

	private void AddEffect()
	{
		this.WuXueBakEffect = Global.GetDecoration(513, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.WuXueBakEffect.Coordinate = new Point(258, 118);
	}

	public void InitPartData()
	{
		this.currentRealJingjie = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.WuXueLevel);
		this.currentJingjie = this.currentRealJingjie;
		if (this.currentJingjie == 0)
		{
			this.currentJingjie = 1;
		}
		else if (this.currentJingjie > Global.MaxWuxueLevel)
		{
			this.currentJingjie = Global.MaxWuxueLevel;
		}
		this.InitWuxueImgArr();
		this.InitWuxueCanvas();
		this.SetPos();
		this.SetJihuoTiaojian();
		this.SetShuxin();
		this.SetWuxueJingjieName();
		this.SetWuxing();
	}

	public void ClearWuxueCanvas()
	{
		while (this.wuxueCanvas.numChildren > 0)
		{
			this.wuxueCanvas.removeChildAt(this.wuxueCanvas.numChildren - 1);
		}
	}

	public void InitWuxueImgArr()
	{
		this.wuxueArray.RemoveRange(0, this.wuxueArray.Count);
		BitmapData gameResImage = Global.GetGameResImage("Images/Plate/wuxueJinjie.png");
		for (int i = 0; i < 12; i++)
		{
			GWuxueImg gwuxueImg = U3DUtils.NEW<GWuxueImg>();
			gwuxueImg.ImgBak_Source = gameResImage;
			gwuxueImg.ImgPhoto_Source = Global.GetGameResImage(StringUtil.substitute("Images/Wuxue/{0}.png", new object[]
			{
				i
			}));
			this.wuxueArray[i] = gwuxueImg;
			if (i > 0 && i < 11 && this.currentRealJingjie >= i)
			{
				gwuxueImg.Jihuo = true;
			}
		}
	}

	public void InitWuxueCanvas()
	{
		this.ClearWuxueCanvas();
		for (int i = 0; i < this.wuxueArray.Count; i++)
		{
			this.wuxueCanvas.Children.Add(this.wuxueArray[i]);
			this.SetProp(this.wuxueArray[i], 0.0, 0.0, 0);
		}
	}

	public void SetPos()
	{
		GWuxueImg obj = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie - 1));
		this.SetProp(obj, 0.7, 0.7, 1);
		obj = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie));
		this.SetProp(obj, 1.0, 1.0, 2);
		obj = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie + 1));
		this.SetProp(obj, 0.7, 0.7, 3);
		this.wuxueCanvas.swapChildrenAt(this.currentJingjie, this.currentJingjie + 1);
		this.Swap = true;
	}

	public void SetProp(GWuxueImg obj, double scale, double alpha, int posIndex)
	{
		if (null != obj)
		{
			obj.scaleX = scale;
			obj.scaleY = scale;
			obj.alpha = alpha;
			obj.X = (double)this.posArr[posIndex, 0];
			obj.Y = (double)this.posArr[posIndex, 1];
		}
	}

	public void NextWuxueImg()
	{
		if (this.currentJingjie >= this.wuxueArray.Count - 2)
		{
			return;
		}
		this.currentJingjie++;
		if (this.Swap)
		{
			this.wuxueCanvas.swapChildrenAt(this.currentJingjie - 1, this.currentJingjie);
			this.Swap = false;
		}
		GWuxueImg gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie - 2));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie - 1));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie + 1));
		this.wuxueCanvas.swapChildrenAt(this.currentJingjie, this.currentJingjie + 1);
		this.Swap = true;
		this.SetWuxueJingjieName();
	}

	public void PrevWuxueImg()
	{
		if (this.currentJingjie <= 1)
		{
			return;
		}
		this.currentJingjie--;
		if (this.Swap)
		{
			this.wuxueCanvas.swapChildrenAt(this.currentJingjie + 1, this.currentJingjie + 2);
			this.Swap = false;
		}
		GWuxueImg gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie + 2));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie + 1));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie));
		gwuxueImg = U3DUtils.AS<GWuxueImg>(this.wuxueCanvas.getChildAt(this.currentJingjie - 1));
		this.wuxueCanvas.swapChildrenAt(this.currentJingjie, this.currentJingjie + 1);
		this.Swap = true;
		this.SetWuxueJingjieName();
	}

	public void SetWuxing()
	{
		this.TotalWuxingText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.WuXingZhi).ToString();
	}

	private string GetWuxueJingjieName(int index)
	{
		int wuxueJingjieBuffID = Global.GetWuxueJingjieBuffID(index);
		if (wuxueJingjieBuffID != 0)
		{
			return Global.GetGoodsNameByID(wuxueJingjieBuffID, false);
		}
		return string.Empty;
	}

	private double[] GetRolePropList(int index)
	{
		int wuxueJingjieBuffID = Global.GetWuxueJingjieBuffID(index);
		return Global.GetGoodsEquipPropsDoubleList(wuxueJingjieBuffID);
	}

	private void SetWuxueJingjieName()
	{
		if (this.currentRealJingjie >= this.currentJingjie)
		{
			this.DangqianJingjieText.TextColor = new SolidColorBrush(3669815U);
			this.DangqianJingjieText.Text = StringUtil.substitute(Global.GetLang("第{0}重：{1}【已激活】"), new object[]
			{
				this.currentJingjie,
				this.GetWuxueJingjieName(this.currentJingjie)
			});
		}
		else
		{
			this.DangqianJingjieText.TextColor = new SolidColorBrush(8421504U);
			this.DangqianJingjieText.Text = StringUtil.substitute(Global.GetLang("第{0}重：{1}【未激活】"), new object[]
			{
				this.currentJingjie,
				this.GetWuxueJingjieName(this.currentJingjie)
			});
		}
	}

	private void SetJihuoTiaojian()
	{
		if (this.currentRealJingjie >= this.wuxueArray.Count - 2)
		{
			this.JihuoTiaojianLevelText.Text = string.Empty;
			this.JihuoTiaojianZhengqiText.Text = string.Empty;
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/WuXue.Xml");
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "WuXue", "ID", (this.currentRealJingjie + 1).ToString());
		if (xelement == null)
		{
			return;
		}
		this.needLevel = Global.GetXElementAttributeInt(xelement, "LevelLimit");
		this.needWuxin = Global.GetXElementAttributeInt(xelement, "WuXing");
		this.JihuoTiaojianLevelText.Text = this.needLevel.ToString();
		this.JihuoTiaojianZhengqiText.Text = this.needWuxin.ToString();
		if (Global.Data.roleData.Level < this.needLevel)
		{
			this.JihuoTiaojianLevelText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.JihuoTiaojianLevelText.TextColor = new SolidColorBrush(3669815U);
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.WuXingZhi) < this.needWuxin)
		{
			this.JihuoTiaojianZhengqiText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.JihuoTiaojianZhengqiText.TextColor = new SolidColorBrush(3669815U);
		}
	}

	private void SetTxtShuxin(GTextBlockOutLine obj, int x, int y, string text)
	{
		Canvas.SetLeft(obj, x);
		Canvas.SetTop(obj, y);
		obj.Text = text;
	}

	private void SetShuxin()
	{
		if (this.currentRealJingjie <= 0)
		{
			this.JihuoIcon.Visibility = true;
			this.WuxueJihuoTiaojianBak.Visibility = true;
			this.TianrenHeyiIcon.Visibility = false;
			this.WuxueDianfengBak.Source = null;
			this.SetTxtShuxin(this.DangqianRealJingjieText, 185, 240, Global.GetLang("武学境界"));
			this.SetTxtShuxin(this.DangqianRealJingjieNameText, 204, 257, Global.GetLang("无"));
			this.XiayiRealJingjieText.Text = StringUtil.substitute(Global.GetLang("第{0}重"), new object[]
			{
				this.currentRealJingjie + 1
			});
			this.XiayiRealJingjieNameText.Text = this.GetWuxueJingjieName(this.currentRealJingjie + 1);
		}
		else if (this.currentRealJingjie >= this.wuxueArray.Count - 2)
		{
			this.JihuoIcon.Visibility = false;
			this.WuxueJihuoTiaojianBak.Visibility = false;
			this.TianrenHeyiIcon.Visibility = true;
			this.WuxueDianfengBak.URL = Global.GetGameResImageURL(StringUtil.substitute("Images/Plate/wuxueDianfeng_bak.png", new object[0]));
			this.SetTxtShuxin(this.DangqianRealJingjieText, 239, 290, StringUtil.substitute(Global.GetLang("第{0}重"), new object[]
			{
				this.currentRealJingjie
			}));
			this.SetTxtShuxin(this.DangqianRealJingjieNameText, 234, 307, this.GetWuxueJingjieName(this.currentRealJingjie));
			this.XiayiRealJingjieText.Text = string.Empty;
			this.XiayiRealJingjieNameText.Text = string.Empty;
		}
		else
		{
			this.TianrenHeyiIcon.Visibility = false;
			this.JihuoIcon.Visibility = true;
			this.WuxueJihuoTiaojianBak.Visibility = true;
			this.SetTxtShuxin(this.DangqianRealJingjieText, 189, 240, StringUtil.substitute(Global.GetLang("第{0}重"), new object[]
			{
				this.currentRealJingjie
			}));
			this.SetTxtShuxin(this.DangqianRealJingjieNameText, 184, 257, this.GetWuxueJingjieName(this.currentRealJingjie));
			this.XiayiRealJingjieText.Text = StringUtil.substitute(Global.GetLang("第{0}重"), new object[]
			{
				this.currentRealJingjie + 1
			});
			this.XiayiRealJingjieNameText.Text = this.GetWuxueJingjieName(this.currentRealJingjie + 1);
		}
		double[] rolePropList = this.GetRolePropList(this.currentRealJingjie);
		this.DangqianJingjie_WuliMianshangText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[24] * 100.0)
		});
		this.DangqianJingjie_MofaMianshangText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[255] * 100.0)
		});
		this.DangqianJingjie_ShengmingShangxianText.Text = StringUtil.substitute("{0}", new object[]
		{
			rolePropList[13]
		});
		this.DangqianJingjie_WuliGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[7],
			rolePropList[8]
		});
		this.DangqianJingjie_MofaGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
		this.DangqianJingjie_DaoshuGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
		rolePropList = this.GetRolePropList(this.currentRealJingjie + 1);
		this.XiayiJingjie_WuliMianshangText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[24] * 100.0)
		});
		this.XiayiJingjie_MofaMianshangText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[255] * 100.0)
		});
		this.XiayiJingjie_ShengmingShangxianText.Text = StringUtil.substitute("{0}", new object[]
		{
			rolePropList[13]
		});
		this.XiayiJingjie_WuliGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[7],
			rolePropList[8]
		});
		this.XiayiJingjie_MofaGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
		this.XiayiJingjie_DaoshuGongjiText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[9],
			rolePropList[10]
		});
	}

	private void StartJihuo()
	{
		if (this.currentRealJingjie >= Global.MaxWuxueLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("武学境界已达到最高级，无法提升"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.Level < this.needLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("角色等级太低，无法激活，请升级后再次进行激活"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.WuXingZhi) < this.needWuxin)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("角色悟性值不足，无法激活。做【武学日常】任务可以获得悟性值"), new object[0]), 19, -1, -1, 36060);
			return;
		}
		GameInstance.Game.SpriteActivateNextWuXueLevel();
	}

	public void NotifyWuxueResult(int result, int oldLevel, int nowLevel)
	{
		if (result < 1)
		{
			if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("激活下一级武学等级出错,错误码:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			this.InitPartData();
		}
	}

	protected void CloseWuxueTianrenHeyiWindow()
	{
		if (null != this.WuxueTianrenHeyiWindow)
		{
			this.WuxueTianrenHeyiWindow.Visibility = false;
			this.wuxueTianrenHeyiPart.Destroy();
		}
	}

	public void ShowTianrenHeyiWindow()
	{
		if (null != this.WuxueTianrenHeyiWindow)
		{
			if (this.WuxueTianrenHeyiWindow.Visibility)
			{
				this.CloseWuxueTianrenHeyiWindow();
			}
			else
			{
				this.WuxueTianrenHeyiWindow.Left = (double)((int)((this.Container.Width - 492.0) / 2.0));
				this.WuxueTianrenHeyiWindow.Top = (double)((int)((this.Container.Height - 256.0) / 2.0));
				this.WuxueTianrenHeyiWindow.Visibility = true;
				this.wuxueTianrenHeyiPart.InitPartData(this.currentRealJingjie);
			}
			return;
		}
		this.WuxueTianrenHeyiWindow = U3DUtils.NEW<GBitmapWindow>();
		this.WuxueTianrenHeyiWindow.BodyLeft = 88.0;
		this.WuxueTianrenHeyiWindow.BodyTop = 4.0;
		this.WuxueTianrenHeyiWindow.BodyWidth = 315.0;
		this.WuxueTianrenHeyiWindow.BodyHeight = 199.0;
		this.WuxueTianrenHeyiWindow.WinBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/wuxue1112_bak.png"), false, 0);
		this.WuxueTianrenHeyiWindow.CloseButtonWidth = 13.0;
		this.WuxueTianrenHeyiWindow.CloseButtonHeight = 13.0;
		this.WuxueTianrenHeyiWindow.CloseButtonLeft = 430.0;
		this.WuxueTianrenHeyiWindow.CloseButtonTop = 50.0;
		this.WuxueTianrenHeyiWindow.CloseButtonFill = Global.GetGameResImage("Images/Plate/x1.png");
		this.WuxueTianrenHeyiWindow.CloseButtonTransformFill = Global.GetGameResImage("Images/Plate/x0.png");
		this.WuxueTianrenHeyiWindow.Left = (double)((int)((this.Container.Width - 492.0) / 2.0));
		this.WuxueTianrenHeyiWindow.Top = (double)((int)((this.Container.Height - 256.0) / 2.0));
		this.WuxueTianrenHeyiWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseWuxueTianrenHeyiWindow();
			return true;
		};
		Canvas.SetZIndex(this.WuxueTianrenHeyiWindow, 9001.0);
		this.Container.Children.Add(this.WuxueTianrenHeyiWindow);
		this.wuxueTianrenHeyiPart = U3DUtils.NEW<WuxueTianrenHeyiPart>();
		this.wuxueTianrenHeyiPart.InitPartSize((int)this.WuxueTianrenHeyiWindow.BodyWidth, (int)this.WuxueTianrenHeyiWindow.BodyHeight);
		this.wuxueTianrenHeyiPart.InitPartData(this.currentRealJingjie);
		this.wuxueTianrenHeyiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
		};
		this.WuxueTianrenHeyiWindow.SetContent(this.WuxueTianrenHeyiWindow.Body, this.wuxueTianrenHeyiPart);
	}

	public void PauseAllEffect(bool pause)
	{
		if (this.WuXueBakEffect != null)
		{
			this.WuXueBakEffect.Pause = pause;
		}
		for (int i = 0; i < this.wuxueArray.Count; i++)
		{
			this.wuxueArray[i].PauseEffect(pause);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine DangqianJingjie_WuliMianshangText;

	private GTextBlockOutLine DangqianJingjie_MofaMianshangText;

	private GTextBlockOutLine DangqianJingjie_ShengmingShangxianText;

	private GTextBlockOutLine DangqianJingjie_WuliGongjiText;

	private GTextBlockOutLine DangqianJingjie_MofaGongjiText;

	private GTextBlockOutLine DangqianJingjie_DaoshuGongjiText;

	private GTextBlockOutLine XiayiJingjie_WuliMianshangText;

	private GTextBlockOutLine XiayiJingjie_MofaMianshangText;

	private GTextBlockOutLine XiayiJingjie_ShengmingShangxianText;

	private GTextBlockOutLine XiayiJingjie_WuliGongjiText;

	private GTextBlockOutLine XiayiJingjie_MofaGongjiText;

	private GTextBlockOutLine XiayiJingjie_DaoshuGongjiText;

	private GTextBlockOutLine TotalWuxingText;

	private Canvas WuxueJihuoTiaojianBak;

	private URLImage WuxueDianfengBak = new URLImage();

	private GTextBlockOutLine JihuoTiaojianLevelText;

	private GTextBlockOutLine JihuoTiaojianZhengqiText;

	private int needWuxin;

	private int needLevel;

	private GTextBlockOutLine DangqianJingjieText;

	private GTextBlockOutLine DangqianRealJingjieText;

	private GTextBlockOutLine XiayiRealJingjieText;

	private GTextBlockOutLine DangqianRealJingjieNameText;

	private GTextBlockOutLine XiayiRealJingjieNameText;

	private GIcon JihuoIcon;

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private GIcon TianrenHeyiIcon;

	private Sprite wuxueCanvas;

	private int currentJingjie;

	private int currentRealJingjie;

	public GDecoration WuXueBakEffect;

	private List<GWuxueImg> wuxueArray = new List<GWuxueImg>();

	private int[,] posArr = new int[,]
	{
		{
			168,
			88
		},
		{
			0,
			35
		},
		{
			80,
			2
		},
		{
			220,
			35
		}
	};

	private bool Swap;

	protected GBitmapWindow WuxueTianrenHeyiWindow;

	protected WuxueTianrenHeyiPart wuxueTianrenHeyiPart;
}
