using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class RoleAttributePart3 : UserControl
{
	public RoleAttributePart3()
	{
		this.thisCtrl = this;
		this.gtbDescribe.BodyWidth = 240.0;
		this.gtbDescribe.TextFontWrapping = TextWrapping.Wrap;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.cv1);
		this.cv1.Visibility = true;
		this.cv1.Children.Add(this.gtbName);
		this.gtbName.Text = Global.GetLang("物攻");
		this.gtbName.FontSize = HSTextField.defaultFontSize;
		this.gtbName.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbName.TextColor = new SolidColorBrush(10626862U);
		Canvas.SetLeft(this.gtbName, 120);
		Canvas.SetTop(this.gtbName, 51);
		this.cv1.Children.Add(this.gtbAttack);
		this.gtbAttack.Text = Global.GetLang("物攻值");
		this.gtbAttack.FontSize = HSTextField.defaultFontSize;
		this.gtbAttack.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbAttack, 150);
		Canvas.SetTop(this.gtbAttack, 51);
		this.cv1.Children.Add(this.gtbDefense);
		this.gtbDefense.Text = Global.GetLang("物防");
		this.gtbDefense.FontSize = HSTextField.defaultFontSize;
		this.gtbDefense.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbDefense, 150);
		Canvas.SetTop(this.gtbDefense, 75);
		this.cv1.Children.Add(this.gtbBurstHit);
		this.gtbBurstHit.Text = Global.GetLang("暴击");
		this.gtbBurstHit.FontSize = HSTextField.defaultFontSize;
		this.gtbBurstHit.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbBurstHit, 218);
		Canvas.SetTop(this.gtbBurstHit, 51);
		this.cv1.Children.Add(this.gtbMagicDefense);
		this.gtbMagicDefense.Text = Global.GetLang("魔防");
		this.gtbMagicDefense.FontSize = HSTextField.defaultFontSize;
		this.gtbMagicDefense.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbMagicDefense, 218);
		Canvas.SetTop(this.gtbMagicDefense, 75);
		this.cv1.Children.Add(this.gtbDodge);
		this.gtbDodge.Text = Global.GetLang("闪避");
		this.gtbDodge.FontSize = HSTextField.defaultFontSize;
		this.gtbDodge.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbDodge, 284);
		Canvas.SetTop(this.gtbDodge, 51);
		this.cv1.Children.Add(this.gtbHit);
		this.gtbHit.Text = Global.GetLang("命中");
		this.gtbHit.FontSize = HSTextField.defaultFontSize;
		this.gtbHit.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbHit, 284);
		Canvas.SetTop(this.gtbHit, 75);
		this.cv1.Children.Add(this.gtbLifeLimit);
		this.gtbLifeLimit.Text = Global.GetLang("生命上限");
		this.gtbLifeLimit.FontSize = HSTextField.defaultFontSize;
		this.gtbLifeLimit.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbLifeLimit, 374);
		Canvas.SetTop(this.gtbLifeLimit, 51);
		this.cv1.Children.Add(this.gtbMagicLimit);
		this.gtbMagicLimit.Text = Global.GetLang("暴抗");
		this.gtbMagicLimit.FontSize = HSTextField.defaultFontSize;
		this.gtbMagicLimit.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbMagicLimit, 374);
		Canvas.SetTop(this.gtbMagicLimit, 75);
		this.cv1.Children.Add(this.gtbJiaCheng);
		this.gtbJiaCheng.Text = Global.GetLang("加成");
		this.gtbJiaCheng.FontSize = HSTextField.defaultFontSize;
		this.gtbJiaCheng.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbJiaCheng, 380);
		Canvas.SetTop(this.gtbJiaCheng, 200);
		this.cv1.Children.Add(this.gtbExperience);
		this.gtbExperience.Text = Global.GetLang("经验");
		this.gtbExperience.FontSize = HSTextField.defaultFontSize;
		this.gtbExperience.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbExperience, 219);
		Canvas.SetTop(this.gtbExperience, 226);
		this.cv1.Children.Add(this.gtbLeftChongXueNum);
		this.gtbLeftChongXueNum.Text = Global.GetLang("剩余冲穴次数");
		this.gtbLeftChongXueNum.FontSize = HSTextField.defaultFontSize;
		this.gtbLeftChongXueNum.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbLeftChongXueNum, 247);
		Canvas.SetTop(this.gtbLeftChongXueNum, 180);
		this.Container.Children.Add(this.cv2);
		this.cv2.Visibility = false;
		this.cv2.Children.Add(this.gtbQiJingBaMai);
		this.gtbQiJingBaMai.Text = Global.GetLang("奇经八脉 * ");
		this.gtbQiJingBaMai.FontSize = HSTextField.defaultFontSize;
		this.gtbQiJingBaMai.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbQiJingBaMai.TextColor = new SolidColorBrush(2736991U);
		Canvas.SetLeft(this.gtbQiJingBaMai, 242);
		Canvas.SetTop(this.gtbQiJingBaMai, -14);
		this.cv2.Children.Add(this.gtbJingMaiBodyLevelName);
		this.gtbJingMaiBodyLevelName.Text = Global.GetLang("未知");
		this.gtbJingMaiBodyLevelName.FontSize = HSTextField.defaultFontSize;
		this.gtbJingMaiBodyLevelName.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbJingMaiBodyLevelName.TextColor = new SolidColorBrush(16766720U);
		Canvas.SetLeft(this.gtbJingMaiBodyLevelName, 305);
		Canvas.SetTop(this.gtbJingMaiBodyLevelName, -14);
		this.cv2.Children.Add(this.img);
		this.img.Width = 307.0;
		this.img.Height = 281.0;
		Canvas.SetLeft(this.img, 110);
		Canvas.SetTop(this.img, -24);
		this.img.Children.Add(this.imgJM);
		this.cv2.Children.Add(this.gtbJMName);
		this.gtbJMName.Text = Global.GetLang("阳维脉 ：");
		this.gtbJMName.FontSize = HSTextField.defaultFontSize;
		this.gtbJMName.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbJMName.TextColor = new SolidColorBrush(2736991U);
		Canvas.SetLeft(this.gtbJMName, 23);
		Canvas.SetTop(this.gtbJMName, 278);
		this.cv2.Children.Add(this.gtbDescribe);
		this.gtbDescribe.Text = Global.GetLang("人体的奇经八脉之一，贯通之意");
		this.gtbDescribe.TextColor = new SolidColorBrush(14407004U);
		this.gtbDescribe.FontSize = HSTextField.defaultFontSize;
		this.gtbDescribe.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbDescribe.Width = 230.0;
		Canvas.SetLeft(this.gtbDescribe, 23);
		Canvas.SetTop(this.gtbDescribe, 295);
		this.cv2.Children.Add(this.gtbJiaChengEX);
		this.gtbJiaChengEX.Text = Global.GetLang("附加加成");
		this.gtbJiaChengEX.FontSize = HSTextField.defaultFontSize;
		this.gtbJiaChengEX.Foreground = new SolidColorBrush(uint.MaxValue);
		this.gtbJiaChengEX.TextColor = new SolidColorBrush(10626862U);
		Canvas.SetLeft(this.gtbJiaChengEX, 158);
		Canvas.SetTop(this.gtbJiaChengEX, 333);
		this.cv2.Children.Add(this.gtbXueName);
		this.gtbXueName.Text = string.Empty;
		this.gtbXueName.FontSize = HSTextField.defaultFontSize;
		this.gtbXueName.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbXueName, 340);
		Canvas.SetTop(this.gtbXueName, 280);
		this.cv2.Children.Add(this.gtbJiaChengAttr);
		this.gtbJiaChengAttr.Text = string.Empty;
		this.gtbJiaChengAttr.FontSize = HSTextField.defaultFontSize;
		this.gtbJiaChengAttr.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbJiaChengAttr, 340);
		Canvas.SetTop(this.gtbJiaChengAttr, 303);
		this.cv2.Children.Add(this.gtbNeedNeiLi);
		this.gtbNeedNeiLi.Text = string.Empty;
		this.gtbNeedNeiLi.FontSize = HSTextField.defaultFontSize;
		this.gtbNeedNeiLi.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbNeedNeiLi, 340);
		Canvas.SetTop(this.gtbNeedNeiLi, 327);
		this.Container.Children.Add(this.cvXueweiDian);
		this.cvXueweiDian.Visibility = false;
		this.cvXueweiDian.Width = 307.0;
		this.cvXueweiDian.Height = 281.0;
		Canvas.SetLeft(this.cvXueweiDian, 110);
		Canvas.SetTop(this.cvXueweiDian, 0);
		this.Container.Children.Add(this.cvZhenLong);
		this.cvZhenLong.Visibility = false;
		this.cvZhenLong.Width = 394.0;
		this.cvZhenLong.Height = 76.0;
		Canvas.SetLeft(this.cvZhenLong, 20);
		Canvas.SetTop(this.cvZhenLong, 272);
	}

	private int FindTabIndex(string text)
	{
		for (int i = 0; i < this.TabNames.Length; i++)
		{
			if (this.TabNames[i] == text)
			{
				return i;
			}
		}
		return -1;
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private void AddChongXueHint()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
		if (Global.Data.roleData.JingMaiXueWeiNum > 0)
		{
			return;
		}
		Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("经脉UI"), 630002, 0, 1);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "Total";
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("总  缆");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Cursor = Cursors.Hand;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconCX_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, -25);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "YangWei";
		gicon.ItemCode = 0;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("阳维脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconYangwei_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 7);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "YinWei";
		gicon.ItemCode = 1;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("阴维脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconYinwei_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 39);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "YangQiao";
		gicon.ItemCode = 2;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("阳跷脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconYangqiao_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 71);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "YinQiao";
		gicon.ItemCode = 3;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("阴跷脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconYinqiao_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 103);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "DaiMai";
		gicon.ItemCode = 4;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("带 脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconDaimai_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 135);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "ChongMai";
		gicon.ItemCode = 5;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("冲 脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconChongmai_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 167);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "RenMai";
		gicon.ItemCode = 6;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("任 脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.AddChongXueHint();
			this.iconRenmai_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 199);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "DuMai";
		gicon.ItemCode = 7;
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Cursor = Cursors.Hand;
		gicon.Text = Global.GetLang("督 脉");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
			this.iconDumai_Click();
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 231);
		this.Container.Children.Add(gicon);
		this.AddTimesIcon = U3DUtils.NEW<GIcon>();
		this.AddTimesIcon.Width = 80.0;
		this.AddTimesIcon.Height = 21.0;
		this.AddTimesIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.AddTimesIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.AddTimesIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.AddTimesIcon.Text = Global.GetLang("增加次数");
		this.AddTimesIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.AddTimesIcon.DisableTextColor = new SolidColorBrush(ColorSL.FromArgb(255, 40, 54, 63));
		this.AddTimesIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.AddTimesIcon.EnableIcon)
			{
				return;
			}
			this.AddTimesOk(-1, -1, -1);
		};
		Canvas.SetLeft(this.AddTimesIcon, 282);
		Canvas.SetTop(this.AddTimesIcon, 177);
		this.cv1.Children.Add(this.AddTimesIcon);
		this.progressBar = U3DUtils.NEW<GProgressBar>();
		this.progressBar.BodyWidth = 282.0;
		this.progressBar.BodyHeight = 5.0;
		this.progressBar.ForeBrush = new ImageBrush(Global.GetGameResImage("Images/Plate/cm_progressBar.png"));
		this.progressBar.RadiusX = 0.0;
		this.progressBar.RadiusY = 0.0;
		this.progressBar.ProgessTextColor = new SolidColorBrush(4294901760U);
		this.cvZhenLong.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/zhenlong.png"), false, 0);
		this.cvZhenLong.Visibility = true;
		this.progressBar.Visibility = false;
		Canvas.SetLeft(this.progressBar, 123);
		Canvas.SetTop(this.progressBar, 13);
		this.Container.Children.Add(this.progressBar);
	}

	public void InitPartData()
	{
		if (this.FirstInitPartData)
		{
			this.FirstInitPartData = false;
			this.SelectIcon("Total", null, null);
			this.SetJingMaiProgress();
		}
	}

	public void GetNewData()
	{
		this.SelectIcon("Total", null, null);
		if (Global.Data.roleData.JingMaiXueWeiNum <= 0)
		{
			Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("经脉UI"), 630001, 0, 1);
		}
		if (!this.FirstGetNewData && this.FirstGetNewDataTime == Global.GetCorrectDateTime().ToString("yyyy-MM-dd"))
		{
			return;
		}
		this.FirstGetNewData = false;
		this.FirstGetNewDataTime = DateTime.Now.ToString("yyyy-MM-dd");
		GameInstance.Game.SpriteGetJingMaiInfo();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
	}

	public void CleanUpChildWindows()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void InitTotalData()
	{
		if (Global.Data.roleData.Occupation == 0)
		{
			this.gtbName.Text = Global.GetLang("物攻");
			this.gtbAttack.Text = this.GetJingMaiInfoItem("Attack").ToString();
		}
		else
		{
			this.gtbName.Text = Global.GetLang("魔攻");
			this.gtbAttack.Text = this.GetJingMaiInfoItem("MAttack").ToString();
		}
		this.gtbDefense.Text = this.GetJingMaiInfoItem("PhyDefense").ToString();
		this.gtbBurstHit.Text = this.GetJingMaiInfoItem("BurstV").ToString();
		this.gtbMagicDefense.Text = this.GetJingMaiInfoItem("MagicDefense").ToString();
		this.gtbDodge.Text = this.GetJingMaiInfoItem("DodgeV").ToString();
		this.gtbHit.Text = this.GetJingMaiInfoItem("HitV").ToString();
		this.gtbLifeLimit.Text = this.GetJingMaiInfoItem("MaxLifeV").ToString();
		this.gtbMagicLimit.Text = this.GetJingMaiInfoItem("BurstPercentV").ToString();
		this.gtbJiaCheng.Text = this.GetJingMaiInfoItem("JiaChengCiShu").ToString();
		this.gtbExperience.Text = this.GetJingMaiInfoItem("LeiJiJingYan").ToString();
		this.RefresDailyJingMaiData();
		this.RefreshBodyLevelName();
	}

	private void SetJingMaiProgress()
	{
		int jingMaiXueWeiNum = Global.Data.roleData.JingMaiXueWeiNum;
		int num = ((jingMaiXueWeiNum - 1) / (Global.MaxJingMaiLevel * 8) + 1) * (Global.MaxJingMaiLevel * 8);
		this.progressBar.Percent = (double)jingMaiXueWeiNum / ((double)num * 1.0);
		this.progressBar.ProgessText = StringUtil.substitute("{0}/{1}", new object[]
		{
			jingMaiXueWeiNum,
			num
		});
	}

	private int GetJingMaiInfoItem(string itemName)
	{
		if (this.JingMaiInfoDict == null)
		{
			return 0;
		}
		int result = 0;
		if (!this.JingMaiInfoDict.ContainsKey(itemName))
		{
			return 0;
		}
		this.JingMaiInfoDict.TryGetValue(itemName, ref result);
		return result;
	}

	public void RefreshJingMaiInfo(Dictionary<string, int> jingMaiInfoDict)
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.JingMaiInfoDict = jingMaiInfoDict;
		this.InitTotalData();
		if (!this.GetJingMaiDataList())
		{
			return;
		}
	}

	public void RefreshData()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (null != this.SelectedIcon)
		{
			this.ReseXueWeiDiansProgress(this.SelectedIcon.ItemCode);
		}
		this.ResetZhenLongDiansProgress(false);
	}

	public void RefresDailyJingMaiData()
	{
		this.gtbLeftChongXueNum.Text = Global.TodayChongXueNum().ToString();
		if (null != this.r3_ChongXue)
		{
			this.r3_ChongXue.ChongXueNum = this.gtbLeftChongXueNum.Text;
		}
	}

	public void RefreshOtherJingMaiExp(int canGetExpNum, int totalJingMaiExp)
	{
		this.gtbJiaCheng.Text = canGetExpNum.ToString();
		this.gtbExperience.Text = totalJingMaiExp.ToString();
	}

	public void RefreshBodyLevelName()
	{
		this.gtbJingMaiBodyLevelName.Text = Global.GetJingMaiBodyLevelName(Global.Data.roleData.JingMaiBodyLevel - 1);
	}

	public void UpJingMaiLevel(int ret, int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		int jingMaiBodyLevel2 = Global.Data.roleData.JingMaiBodyLevel;
		if (Global.Data.JingMaiDataList == null)
		{
			Global.Data.JingMaiDataList = new List<JingMaiData>();
		}
		JingMaiData jingMaiDataByJMID = Global.GetJingMaiDataByJMID(jingMaiBodyLevel2, jingMaiID, Global.Data.JingMaiDataList);
		if (jingMaiDataByJMID != null)
		{
			if (ret > 0)
			{
				jingMaiDataByJMID.JingMaiLevel = jingMaiLevel;
			}
			else
			{
				jingMaiLevel = jingMaiDataByJMID.JingMaiLevel + 1;
			}
		}
		else if (ret > 0)
		{
			JingMaiData jingMaiData = new JingMaiData();
			jingMaiData.DbID = ret;
			jingMaiData.JingMaiID = jingMaiID;
			jingMaiData.JingMaiLevel = jingMaiLevel;
			jingMaiData.JingMaiBodyLevel = jingMaiBodyLevel2;
			Global.Data.JingMaiDataList.Add(jingMaiData);
		}
		else
		{
			jingMaiLevel = 1;
		}
		if (ret < 0)
		{
			if (ret >= -4 && ret <= -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("冲击穴位时失败, 你的灵力不足"), new object[]
				{
					3
				}), 0, -1, -1, 0);
			}
			else if (ret == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("运气不佳, 冲击穴位时失败"), new object[0]), 0, -1, -1, 0);
			}
			else if (ret == -10000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("经脉的重数已经到了最高层，无法再继续冲脉"), new object[0]), 0, -1, -1, 0);
			}
			else if (ret == -11000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("冲击穴位时失败, 你今日的剩余冲穴次数已经为0"), new object[]
				{
					2
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("冲击穴位时失败, 错误:{0}"), new object[]
				{
					ret
				}), 0, -1, -1, 0);
			}
			this.ShowChongXueWindow(jingMaiBodyLevel2, jingMaiID, jingMaiLevel);
			return;
		}
		Global.Data.roleData.JingMaiBodyLevel = jingMaiBodyLevel;
		this.ReseXueWeiDiansProgress(jingMaiID);
		Global.Data.roleData.JingMaiXueWeiNum = Global.CalcJingMaiXueWeiNum(Global.Data.JingMaiDataList);
		int jingMaiOkNum = Global.Data.roleData.JingMaiOkNum;
		Global.Data.roleData.JingMaiOkNum = Global.CalcJingMaiOkNum(Global.Data.JingMaiDataList);
		if (jingMaiOkNum != Global.Data.roleData.JingMaiOkNum)
		{
			Global.Data.GameScene.ReloadJingMaiUpDeco();
		}
		this.SetJingMaiProgress();
		this.ResetZhenLongDiansProgress(jingMaiBodyLevel2 != Global.Data.roleData.JingMaiBodyLevel);
		this.RefreshBodyLevelName();
		Global.Data.GameScene.UpdateLeaderJingMaiWord();
		Global.Data.GameScene.ExternalPlayDeco(90000, 0, 1);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你, 冲击穴位成功!"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 1,
				Tag = null
			});
		}
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.1;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetTop(this.PlaceHolder, -36);
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Root.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Root.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void ShowChongXueWindow(int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
	{
		if (null != this.chongXueWindow)
		{
			this.CloseChildWindow(this.chongXueWindow);
			this.chongXueWindow = null;
			if (this.r3_ChongXue != null)
			{
				this.r3_ChongXue.Destroy();
				this.r3_ChongXue = null;
			}
		}
		this.chongXueWindow = U3DUtils.NEW<GChildWindow>();
		this.chongXueWindow.Left = (double)Super.GetChildLeft(480, 308);
		this.chongXueWindow.Top = (double)Super.GetChildTop(306, 286);
		this.chongXueWindow.HeadLeft = 0.0;
		this.chongXueWindow.HeadTop = 0.0;
		this.chongXueWindow.HeadWidth = 308.0;
		this.chongXueWindow.HeadHeight = 46.0;
		this.chongXueWindow.BodyLeft = 0.0;
		this.chongXueWindow.BodyTop = 46.0;
		this.chongXueWindow.BodyWidth = 308.0;
		this.chongXueWindow.BodyHeight = 219.0;
		this.InitChildWindow1(this.chongXueWindow, Global.GetLang("冲 穴"), false);
		this.chongXueWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.chongXueWindow);
			this.chongXueWindow = null;
			if (this.r3_ChongXue != null)
			{
				this.r3_ChongXue.Destroy();
				this.r3_ChongXue = null;
			}
			return true;
		};
		Canvas.SetZIndex(this.chongXueWindow, 9001.0);
		this.Root.Children.Add(this.chongXueWindow);
		this.r3_ChongXue = U3DUtils.NEW<RoleAttributePart3_ChongXue>();
		this.r3_ChongXue.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/jm_cx_bak.png"), false, 0);
		this.r3_ChongXue.InitPartSize((int)this.chongXueWindow.BodyWidth - 18, (int)this.chongXueWindow.BodyHeight - 9);
		this.r3_ChongXue.InitPartData(jingMaiBodyLevel, jingMaiID, jingMaiLevel);
		this.r3_ChongXue.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.r3_ChongXue_DPSelectedItem(jingMaiID, s, e);
		};
		this.chongXueWindow.SetContent(this.chongXueWindow.BodyPresenter, this.r3_ChongXue, 9.0, 0.0, true);
	}

	private void r3_ChongXue_DPSelectedItem(int jingMaiID, object s, DPSelectedItemEventArgs e)
	{
		this.CloseModalDialog();
		if (e.ID == 1)
		{
			GameInstance.Game.SpriteUpJingMai(Global.Data.roleData.JingMaiBodyLevel, jingMaiID, e.IDType);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.tc.Container.Children.Add(this.LoadingWin);
		}
		else if (e.ID == 2)
		{
			this.AddTimesOk(this.r3_ChongXue.GetJingMaiBodyLevel(), this.r3_ChongXue.GetJingMaiID(), this.r3_ChongXue.GetJingMaiLevel());
		}
		this.CloseChildWindow(this.chongXueWindow);
		this.chongXueWindow = null;
		if (this.r3_ChongXue != null)
		{
			this.r3_ChongXue.Destroy();
			this.r3_ChongXue = null;
		}
	}

	private void AddTimesOk(int jingMaiBodyLevel = -1, int jingMaiID = -1, int jingMaiLevel = -1)
	{
		if (null != this.AddTimesWindow)
		{
			this.CloseChildWindow(this.AddTimesWindow);
			this.AddTimesWindow = null;
			this.addTimesPart = null;
			return;
		}
		this.AddTimesWindow = U3DUtils.NEW<GChildWindow>();
		this.AddTimesWindow.Left = (double)Super.GetChildLeft(480, 308);
		this.AddTimesWindow.Top = (double)Super.GetChildTop(306, 166);
		this.AddTimesWindow.HeadLeft = 0.0;
		this.AddTimesWindow.HeadTop = 0.0;
		this.AddTimesWindow.HeadWidth = 308.0;
		this.AddTimesWindow.HeadHeight = 46.0;
		this.AddTimesWindow.BodyLeft = 0.0;
		this.AddTimesWindow.BodyTop = 46.0;
		this.AddTimesWindow.BodyWidth = 308.0;
		this.AddTimesWindow.BodyHeight = 166.0;
		this.InitChildWindow1(this.AddTimesWindow, Global.GetLang("增加冲穴次数"), false);
		this.AddTimesWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.AddTimesWindow = null;
			this.addTimesPart = null;
			return true;
		};
		Canvas.SetZIndex(this.AddTimesWindow, 9001.0);
		this.Root.Children.Add(this.AddTimesWindow);
		this.addTimesPart = U3DUtils.NEW<AddTimesPart>();
		this.addTimesPart.JingMaiBodyLevel = jingMaiBodyLevel;
		this.addTimesPart.JingMaiID = jingMaiID;
		this.addTimesPart.JingMaiLevel = jingMaiLevel;
		this.addTimesPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/jm_zjcxcs_bak.png"), false, 0);
		this.addTimesPart.InitPartSize((int)this.AddTimesWindow.BodyWidth - 18, (int)this.AddTimesWindow.BodyHeight - 9);
		this.addTimesPart.InitPartData();
		this.addTimesPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseModalDialog();
			if (this.addTimesPart.JingMaiBodyLevel >= 0)
			{
				this.ShowChongXueWindow(this.addTimesPart.JingMaiBodyLevel, this.addTimesPart.JingMaiID, this.addTimesPart.JingMaiLevel);
			}
			this.CloseChildWindow(this.AddTimesWindow);
			this.AddTimesWindow = null;
			this.addTimesPart = null;
		};
		this.AddTimesWindow.SetContent(this.AddTimesWindow.BodyPresenter, this.addTimesPart, 9.0, 0.0, true);
	}

	public void RefreshGoodsCount()
	{
		if (null != this.addTimesPart)
		{
			this.addTimesPart.RefreshGoodsCount();
		}
		if (null != this.r3_ChongXue)
		{
			this.r3_ChongXue.RefreshGoodsCount();
		}
	}

	private bool GetJingMaiDataList()
	{
		if (!this.FirstGetJingMaiList)
		{
			return true;
		}
		this.FirstGetJingMaiList = false;
		GameInstance.Game.SpriteGetJingMaiList();
		return false;
	}

	private void SelectIcon(string iconName, int[,] pointArr, string[] nameArry)
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName(iconName));
		if (gicon == this.SelectedIcon)
		{
			return;
		}
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 255, 255));
		if (null != this.SelectedIcon)
		{
			this.SelectedIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
			this.SelectedIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		}
		if ("Total" == iconName)
		{
			this.cv1.Visibility = true;
			this.progressBar.Visibility = true;
			this.cv2.Visibility = false;
			this.cvZhenLong.Visibility = true;
		}
		else
		{
			this.cv1.Visibility = false;
			this.progressBar.Visibility = false;
			this.cv2.Visibility = true;
			this.cvZhenLong.Visibility = false;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				Tag = iconName
			});
		}
		if (iconName != "Total")
		{
			this.imgJM.URL = Global.GetGameResImageURL(StringUtil.substitute("Images/Plate/jm_{0}_text.png", new object[]
			{
				iconName.ToLower()
			}));
		}
		this.gtbJMName.Text = this.GetJMNameByIconName(gicon.ItemCode);
		this.gtbDescribe.Text = this.GetDescribeByIcon(gicon.ItemCode);
		this.gtbJiaChengEX.Text = this.GetJiaChengExByIcon(gicon.ItemCode);
		if ("Total" == iconName)
		{
			this.cvXueweiDian.Visibility = false;
			this.CreateZhenLongDians();
		}
		else
		{
			this.cvXueweiDian.Visibility = true;
			this.CreateXueWeiDians();
			this.ResetWueWeiDiansCoordinate(pointArr, nameArry, this.LeftOffsets[gicon.ItemCode], this.TopOffsets[gicon.ItemCode]);
			this.ReseXueWeiDiansProgress(gicon.ItemCode);
		}
		this.SelectedIcon = gicon;
	}

	private string GetJMNameByIconName(int JingMaiID)
	{
		return Global.GetLang(Global.JingMaiNames[JingMaiID]);
	}

	private string GetDescribeByIcon(int JingMaiID)
	{
		return Global.GetLang(this.JingMaiDescs[JingMaiID]);
	}

	private string GetJiaChengExByIcon(int jingMaiID)
	{
		return string.Empty;
	}

	private void CreateXueWeiDians()
	{
		if (!this.FirstCreateXueWeiDians)
		{
			return;
		}
		this.FirstCreateXueWeiDians = false;
		for (int i = 0; i < Global.MaxJingMaiLevel; i++)
		{
			string uri = string.Empty;
			if (i <= 0)
			{
				uri = "Images/Plate/jm_btn_enabled.png";
			}
			else
			{
				uri = "Images/Plate/jm_btn_disable.png";
			}
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Name = StringUtil.substitute("XueWeiD{0}", new object[]
			{
				i
			});
			gicon.Width = 10.0;
			gicon.Height = 9.0;
			gicon.BodySource = new ImageBrush(Global.GetGameResImage(uri));
			gicon.Cursor = Cursors.Hand;
			gicon.ItemCode = i;
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				Super.RemoveSystemNaviBox(this.Container, Global.GetLang("经脉UI"), null);
				if (null != this.SelectedIcon)
				{
					JingMaiData jingMaiDataByJMID = Global.GetJingMaiDataByJMID(Global.Data.roleData.JingMaiBodyLevel, this.SelectedIcon.ItemCode, Global.Data.JingMaiDataList);
					int num = 0;
					if (jingMaiDataByJMID != null)
					{
						num = jingMaiDataByJMID.JingMaiLevel;
					}
					if (num == (s as GIcon).ItemCode)
					{
						this.ShowChongXueWindow(Global.Data.roleData.JingMaiBodyLevel, this.SelectedIcon.ItemCode, (s as GIcon).ItemCode + 1);
					}
				}
			};
			gicon.addEventListener("ROLL_OVER", new MouseEventHandler(this.icon_MouseEnter));
			gicon.addEventListener("ROLL_OUT", new MouseEventHandler(this.icon_MouseLeave));
			this.cvXueweiDian.Children.Add(gicon);
			this.XueWeiDianList.Add(gicon);
		}
	}

	private void icon_MouseEnter(MouseEvent e)
	{
		object currentTarget = e.currentTarget;
		if (null != this.SelectedIcon)
		{
			int itemCode = (currentTarget as GIcon).ItemCode;
			string text = ((currentTarget as GIcon).ItemObject as string[])[itemCode];
			this.gtbXueName.Text = text;
			XElement jingMaiXmlItem = Global.GetJingMaiXmlItem(this.SelectedIcon.ItemCode, Global.Data.roleData.Occupation, Global.Data.roleData.JingMaiBodyLevel - 1);
			int xelementAttributeInt = Global.GetXElementAttributeInt(jingMaiXmlItem, "Property");
			this.gtbJiaChengAttr.Text = StringUtil.substitute("{0} +{1}", new object[]
			{
				Global.GetXElementAttributeStr(jingMaiXmlItem, "PropName"),
				xelementAttributeInt
			});
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(jingMaiXmlItem, "LingLi");
			this.gtbNeedNeiLi.Text = xelementAttributeInt2.ToString();
		}
	}

	private void icon_MouseLeave(MouseEvent e)
	{
		this.gtbXueName.Text = string.Empty;
		this.gtbJiaChengAttr.Text = string.Empty;
		this.gtbNeedNeiLi.Text = string.Empty;
	}

	private void ResetWueWeiDiansCoordinate(int[,] pointArr, string[] nameArr, int offsetLeft, int offsetTop)
	{
		for (int i = 0; i < this.XueWeiDianList.Count; i++)
		{
			this.XueWeiDianList[i].ItemObject = nameArr;
			Canvas.SetLeft(this.XueWeiDianList[i], pointArr[i, 0] - 110 + offsetLeft);
			Canvas.SetTop(this.XueWeiDianList[i], pointArr[i, 1] - 40 + offsetTop);
		}
	}

	private void ReseXueWeiDiansProgress(int jingMaiID)
	{
		if (Global.Data.JingMaiDataList == null)
		{
			return;
		}
		if (this.XueWeiDianList.Count <= 0)
		{
			return;
		}
		JingMaiData jingMaiDataByJMID = Global.GetJingMaiDataByJMID(Global.Data.roleData.JingMaiBodyLevel, jingMaiID, Global.Data.JingMaiDataList);
		int num = 0;
		if (jingMaiDataByJMID != null)
		{
			num = jingMaiDataByJMID.JingMaiLevel;
		}
		if (num == this.LastXueWeiProgress)
		{
			return;
		}
		if (num < this.LastXueWeiProgress)
		{
			this.XueWeiDianList[num].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_enabled.png"));
			if (this.LastXueWeiProgress >= Global.MaxJingMaiLevel)
			{
				this.LastXueWeiProgress--;
			}
			for (int i = num + 1; i <= this.LastXueWeiProgress; i++)
			{
				this.XueWeiDianList[i].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_disable.png"));
			}
		}
		else
		{
			if (num < Global.MaxJingMaiLevel)
			{
				this.XueWeiDianList[num].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_enabled.png"));
			}
			for (int j = this.LastXueWeiProgress; j < num; j++)
			{
				this.XueWeiDianList[j].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_ok.png"));
			}
		}
		this.LastXueWeiProgress = num;
	}

	private void iconCX_Click()
	{
		this.SelectIcon("Total", null, null);
	}

	private void iconYangwei_Click()
	{
		this.SelectIcon("YangWei", this.arr1, this.arr1s);
	}

	private void iconYinwei_Click()
	{
		this.SelectIcon("YinWei", this.arr2, this.arr2s);
	}

	private void iconYangqiao_Click()
	{
		this.SelectIcon("YangQiao", this.arr3, this.arr3s);
	}

	private void iconYinqiao_Click()
	{
		this.SelectIcon("YinQiao", this.arr4, this.arr4s);
	}

	private void iconDaimai_Click()
	{
		this.SelectIcon("DaiMai", this.arr5, this.arr5s);
	}

	private void iconChongmai_Click()
	{
		this.SelectIcon("ChongMai", this.arr6, this.arr6s);
	}

	private void iconRenmai_Click()
	{
		this.SelectIcon("RenMai", this.arr7, this.arr7s);
	}

	private void iconDumai_Click()
	{
		this.SelectIcon("DuMai", this.arr8, this.arr8s);
	}

	private void CreateZhenLongDians()
	{
		if (!this.FirstCreateZhenLongDians)
		{
			return;
		}
		this.FirstCreateZhenLongDians = false;
		for (int i = 0; i < Global.JingMaiNames.Length; i++)
		{
			string uri = "Images/Plate/jm_btn_disable.png";
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Name = StringUtil.substitute("ZhenLong{0}", new object[]
			{
				i
			});
			gicon.Width = 10.0;
			gicon.Height = 9.0;
			gicon.BodySource = new ImageBrush(Global.GetGameResImage(uri));
			gicon.ItemCode = i;
			gicon.Tip = Global.GetLang(Global.JingMaiNames[i]);
			Canvas.SetLeft(gicon, this.ZhenLongDianXYs[i, 0]);
			Canvas.SetTop(gicon, this.ZhenLongDianXYs[i, 1]);
			this.cvZhenLong.Children.Add(gicon);
			this.ZhenLongDianList.Add(gicon);
		}
	}

	private void ResetZhenLongDiansProgress(bool forceReset = false)
	{
		if (Global.Data.JingMaiDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.JingMaiDataList.Count; i++)
		{
			JingMaiData jingMaiData = Global.Data.JingMaiDataList[i];
			if (Global.Data.roleData.JingMaiBodyLevel != Global.Data.JingMaiDataList[i].JingMaiBodyLevel)
			{
				if (forceReset)
				{
					this.ZhenLongDianList[jingMaiData.JingMaiID].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_disable.png"));
				}
			}
			else if (jingMaiData.JingMaiLevel >= Global.MaxJingMaiLevel)
			{
				this.ZhenLongDianList[jingMaiData.JingMaiID].BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/jm_btn_ok.png"));
			}
		}
	}

	private int[] LeftOffsets = new int[8];

	private int[] TopOffsets = new int[]
	{
		-24,
		-24,
		-14,
		-20,
		-24,
		-18,
		-24,
		-24
	};

	private LoadingWindow LoadingWin;

	private GIcon SelectedIcon;

	private GProgressBar progressBar;

	private string[] TabNames = new string[]
	{
		"Total",
		"YangWei",
		"YinWei",
		"YangQiao",
		"YinQiao",
		"DaiMai",
		"ChongMai",
		"RenMai",
		"DuMai"
	};

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private string FirstGetNewDataTime = string.Empty;

	private GIcon AddTimesIcon;

	private Dictionary<string, int> JingMaiInfoDict = new Dictionary<string, int>();

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private Canvas PlaceHolder;

	private GChildWindow chongXueWindow;

	private RoleAttributePart3_ChongXue r3_ChongXue;

	private GChildWindow AddTimesWindow;

	private AddTimesPart addTimesPart;

	private bool FirstGetJingMaiList = true;

	private string[] JingMaiDescs = new string[]
	{
		Global.GetLang("维，有维系之意，阳维脉的功能是“维络诸阳”。"),
		Global.GetLang("维，有维系之意，阴维脉的功能是“维络诸阴”。"),
		Global.GetLang("跷，有轻健跷捷之意。有濡养眼目、司眼睑开合和下肢运动的功能。"),
		Global.GetLang("跷，有轻健跷捷之意。有濡养眼目、司眼睑开合和下肢运动的功能。"),
		Global.GetLang("起于季胁，斜向下行到带脉穴，绕身一周，如腰带，能约束纵行的诸脉。"),
		Global.GetLang("上至于头，下至于足，贯穿全身,能调节十二经气血故称“十二经脉之海”。"),
		Global.GetLang("行于腹面正中线，其脉多与手足三阴及阴维脉交会，能总任一身之阴经，故称“阴脉之海”。"),
		Global.GetLang("行于背部正中，其脉多与手足三阳经及阳维脉交会，能总督一身之阳经，故称“阳脉之海”。 ")
	};

	private bool FirstCreateXueWeiDians = true;

	private List<GIcon> XueWeiDianList = new List<GIcon>();

	private int LastXueWeiProgress;

	private int[,] arr1 = new int[,]
	{
		{
			201,
			110
		},
		{
			193,
			93
		},
		{
			174,
			85
		},
		{
			155,
			92
		},
		{
			145,
			109
		},
		{
			151,
			129
		},
		{
			166,
			142
		},
		{
			185,
			152
		},
		{
			199,
			168
		},
		{
			193,
			187
		},
		{
			180,
			206
		},
		{
			179,
			227
		},
		{
			193,
			246
		},
		{
			215,
			254
		},
		{
			236,
			249
		},
		{
			255,
			237
		},
		{
			275,
			236
		},
		{
			291,
			251
		},
		{
			312,
			255
		},
		{
			330,
			240
		},
		{
			333,
			219
		},
		{
			336,
			199
		},
		{
			341,
			178
		},
		{
			355,
			163
		},
		{
			372,
			154
		}
	};

	private string[] arr1s = Global.GetLang("金门,阳交,臑俞,天髎,肩井,头维,阳本神,阳白,阳临泣,阳目窗,阳正营,阳承灵,阳脑空,阳风池,风府,哑门,云门,侠白,尺泽,孔最,列缺,经渠,太渊,鱼际,少商").Split(new char[]
	{
		','
	});

	private int[,] arr2 = new int[,]
	{
		{
			284,
			95
		},
		{
			264,
			90
		},
		{
			244,
			97
		},
		{
			230,
			112
		},
		{
			228,
			132
		},
		{
			242,
			148
		},
		{
			264,
			151
		},
		{
			280,
			138
		},
		{
			295,
			125
		},
		{
			315,
			121
		},
		{
			335,
			128
		},
		{
			337,
			150
		},
		{
			328,
			166
		},
		{
			313,
			178
		},
		{
			307,
			195
		},
		{
			305,
			213
		},
		{
			289,
			225
		},
		{
			267,
			224
		},
		{
			250,
			211
		},
		{
			233,
			201
		},
		{
			214,
			201
		},
		{
			200,
			217
		},
		{
			204,
			239
		},
		{
			219,
			256
		},
		{
			234,
			269
		}
	};

	private string[] arr2s = Global.GetLang("冲门,府舍,大横,阴阳交,腹哀,期门,廉泉,极泉,青灵,少海,灵道,通里,阴郄,神门,少府,少冲,筑宾,中冲,劳宫,大陵,内关,间使,郄门,曲泽,天泉").Split(new char[]
	{
		','
	});

	private int[,] arr3 = new int[,]
	{
		{
			154,
			160
		},
		{
			144,
			176
		},
		{
			146,
			195
		},
		{
			158,
			210
		},
		{
			177,
			213
		},
		{
			197,
			202
		},
		{
			207,
			185
		},
		{
			214,
			166
		},
		{
			220,
			148
		},
		{
			237,
			135
		},
		{
			257,
			141
		},
		{
			278,
			146
		},
		{
			295,
			135
		},
		{
			303,
			118
		},
		{
			317,
			101
		},
		{
			339,
			101
		},
		{
			354,
			118
		},
		{
			355,
			137
		},
		{
			342,
			151
		},
		{
			330,
			166
		},
		{
			335,
			184
		},
		{
			348,
			197
		},
		{
			361,
			211
		},
		{
			372,
			228
		},
		{
			365,
			248
		}
	};

	private string[] arr3s = Global.GetLang("申脉,仆参,跗阳,居髎,大臑俞,肩髃,巨骨,秉风,曲垣,地仓,巨髎,承泣,跷风池,攒竹,眉冲,曲差,五处,承光,通天,络却,玉枕,天柱,承山,飞扬,昆仑").Split(new char[]
	{
		','
	});

	private int[,] arr4 = new int[,]
	{
		{
			214,
			111
		},
		{
			200,
			95
		},
		{
			180,
			91
		},
		{
			159,
			100
		},
		{
			147,
			118
		},
		{
			152,
			139
		},
		{
			170,
			155
		},
		{
			184,
			175
		},
		{
			193,
			196
		},
		{
			208,
			213
		},
		{
			230,
			219
		},
		{
			251,
			208
		},
		{
			264,
			189
		},
		{
			275,
			170
		},
		{
			293,
			158
		},
		{
			315,
			152
		},
		{
			337,
			159
		},
		{
			354,
			170
		},
		{
			364,
			186
		},
		{
			357,
			203
		},
		{
			340,
			216
		},
		{
			320,
			220
		},
		{
			319,
			240
		},
		{
			339,
			249
		},
		{
			349,
			267
		}
	};

	private string[] arr4s = Global.GetLang("然谷,照海,交信,阴谷,横谷,阴气冲,乳根,乳中,庸窗,天溪,胸乡,周荣,库房,气户,盆缺,人迎,晴明,不容,梁门,横鼻,足三里,丰隆,解溪,冲阳,属兑").Split(new char[]
	{
		','
	});

	private int[,] arr5 = new int[,]
	{
		{
			185,
			80
		},
		{
			178,
			99
		},
		{
			187,
			115
		},
		{
			182,
			134
		},
		{
			169,
			148
		},
		{
			156,
			163
		},
		{
			153,
			181
		},
		{
			165,
			198
		},
		{
			187,
			203
		},
		{
			207,
			192
		},
		{
			218,
			174
		},
		{
			234,
			158
		},
		{
			256,
			157
		},
		{
			270,
			172
		},
		{
			267,
			191
		},
		{
			262,
			208
		},
		{
			257,
			229
		},
		{
			268,
			246
		},
		{
			289,
			250
		},
		{
			306,
			237
		},
		{
			317,
			220
		},
		{
			332,
			209
		},
		{
			352,
			211
		},
		{
			363,
			226
		},
		{
			361,
			246
		}
	};

	private string[] arr5s = Global.GetLang("带穴,五枢,维道,天冲,浮白,头窍阴,完骨,带本神,带阳白,带临泣,带目窗,带正营,带承灵,带脑空,环跳,风市,中渎,膝阳关,外丘,光明,阳辅,悬钟,丘墟,足临泣,侠溪").Split(new char[]
	{
		','
	});

	private int[,] arr6 = new int[,]
	{
		{
			216,
			168
		},
		{
			200,
			155
		},
		{
			192,
			138
		},
		{
			196,
			118
		},
		{
			208,
			99
		},
		{
			224,
			88
		},
		{
			242,
			97
		},
		{
			258,
			83
		},
		{
			281,
			87
		},
		{
			290,
			101
		},
		{
			281,
			118
		},
		{
			277,
			136
		},
		{
			292,
			147
		},
		{
			312,
			134
		},
		{
			333,
			134
		},
		{
			342,
			154
		},
		{
			329,
			171
		},
		{
			308,
			178
		},
		{
			289,
			190
		},
		{
			273,
			201
		},
		{
			264,
			218
		},
		{
			274,
			234
		},
		{
			295,
			240
		},
		{
			316,
			245
		},
		{
			333,
			254
		}
	};

	private string[] arr6s = Global.GetLang("会阴,阴交,气冲,横骨,冲大赫,气穴,四满,中注,盲俞,商曲,石关,阴都,通谷,幽门,承满,关门,天枢,太乙,大巨,归来,滑肉门,外陵,水道,水突,气舍").Split(new char[]
	{
		','
	});

	private int[,] arr7 = new int[,]
	{
		{
			299,
			78
		},
		{
			313,
			89
		},
		{
			323,
			103
		},
		{
			311,
			115
		},
		{
			293,
			120
		},
		{
			274,
			116
		},
		{
			255,
			116
		},
		{
			237,
			121
		},
		{
			220,
			131
		},
		{
			207,
			143
		},
		{
			199,
			158
		},
		{
			208,
			174
		},
		{
			228,
			182
		},
		{
			245,
			171
		},
		{
			261,
			163
		},
		{
			279,
			160
		},
		{
			299,
			168
		},
		{
			310,
			180
		},
		{
			308,
			196
		},
		{
			293,
			203
		},
		{
			277,
			209
		},
		{
			265,
			226
		},
		{
			268,
			243
		},
		{
			279,
			259
		},
		{
			295,
			269
		}
	};

	private string[] arr7s = Global.GetLang("曲骨,中极,关元,下脘,中脘,上脘,天突,任廉泉,承浆,大钟,水泉,任大赫,灵墟,箕门,涌泉,神封,地机,血海,腹通谷,步廊,神藏,俞府,复溜,阴陵泉,漏谷").Split(new char[]
	{
		','
	});

	private int[,] arr8 = new int[,]
	{
		{
			161,
			109
		},
		{
			179,
			102
		},
		{
			198,
			112
		},
		{
			205,
			128
		},
		{
			196,
			146
		},
		{
			183,
			164
		},
		{
			179,
			184
		},
		{
			186,
			203
		},
		{
			196,
			218
		},
		{
			208,
			232
		},
		{
			226,
			244
		},
		{
			247,
			249
		},
		{
			267,
			247
		},
		{
			287,
			238
		},
		{
			300,
			226
		},
		{
			311,
			211
		},
		{
			309,
			190
		},
		{
			301,
			174
		},
		{
			291,
			160
		},
		{
			293,
			141
		},
		{
			310,
			129
		},
		{
			327,
			128
		},
		{
			340,
			116
		},
		{
			344,
			103
		},
		{
			334,
			84
		}
	};

	private string[] arr8s = Global.GetLang("兑端,龈交,水沟,素,神庭,上星,通卤会,百会,后顶排,强间,脑户,连风府,大椎,陶道开,神柱,神道,归灵台,至阳,筋缩,至中枢,脊中,悬枢,入命门,腰俞,长强").Split(new char[]
	{
		','
	});

	private int[,] ZhenLongDianXYs = new int[,]
	{
		{
			131,
			30
		},
		{
			76,
			61
		},
		{
			39,
			31
		},
		{
			74,
			5
		},
		{
			153,
			54
		},
		{
			206,
			54
		},
		{
			248,
			28
		},
		{
			307,
			19
		}
	};

	private bool FirstCreateZhenLongDians = true;

	private List<GIcon> ZhenLongDianList = new List<GIcon>();

	private GTabControl _tc = U3DUtils.NEW<GTabControl>();

	private Canvas Root;

	private Canvas cv1 = new Canvas();

	private GTextBlockOutLine gtbName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbAttack = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbDefense = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbBurstHit = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbMagicDefense = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbDodge = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbHit = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbLifeLimit = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbMagicLimit = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbJiaCheng = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbExperience = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbLeftChongXueNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas cv2 = new Canvas();

	private GTextBlockOutLine gtbQiJingBaMai = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbJingMaiBodyLevelName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas img = new Canvas();

	private URLImage imgJM = new URLImage();

	private GTextBlockOutLine gtbJMName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbDescribe = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbJiaChengEX = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbXueName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbJiaChengAttr = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbNeedNeiLi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas cvXueweiDian = new Canvas();

	private Canvas cvZhenLong = new Canvas();

	private UserControl thisCtrl;
}
