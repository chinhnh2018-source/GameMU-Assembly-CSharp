using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RongyaoPart : UserControl
{
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
		int num = 64;
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 62.0;
		this.listBox.Height = 300.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 8.0);
		Canvas.SetLeft(this.listBox, 15);
		Canvas.SetTop(this.listBox, 94 - num);
		this.ItemCollection = this.listBox.ItemsSource;
		this.PrevPageIcon = U3DUtils.NEW<GIcon>();
		this.PrevPageIcon.Width = 18.0;
		this.PrevPageIcon.Height = 14.0;
		this.PrevPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_PrePage_normal.png"));
		this.PrevPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_PrePage_hover.png"));
		this.PrevPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_PrePage_nouse.png"));
		this.PrevPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PrevPageIcon.EnableIcon)
			{
				this.PrevPage();
			}
		};
		Canvas.SetLeft(this.PrevPageIcon, 37);
		Canvas.SetTop(this.PrevPageIcon, 72 - num);
		this.Container.Children.Add(this.PrevPageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 18.0;
		this.NextPageIcon.Height = 14.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_NextPage_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_NextPage_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/Rongyao_NextPage_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NextPageIcon.EnableIcon)
			{
				this.NextPage();
			}
		};
		Canvas.SetLeft(this.NextPageIcon, 37);
		Canvas.SetTop(this.NextPageIcon, 402 - num);
		this.Container.Children.Add(this.NextPageIcon);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 174);
		Canvas.SetTop(gtextBlockOutLine, 119 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_KaiqiZhufuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 174 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_ShengmingShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 196 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 218 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 240 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 261 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 283 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 179);
		Canvas.SetTop(gtextBlockOutLine, 305 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 102 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_RoleLevelText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 119 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_ShengjiXiaohaoText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 135 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_KaiqiZhufuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 174 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_ShengmingShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 196 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_MofaShangxianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 218 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 240 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 261 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 283 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_WuliFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 362);
		Canvas.SetTop(gtextBlockOutLine, 305 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XiayiJingjie_MofaFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 101);
		Canvas.SetTop(gtextBlockOutLine, 39 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TotalRongyaoText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 174);
		Canvas.SetTop(gtextBlockOutLine, 102 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjieText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 174);
		Canvas.SetTop(gtextBlockOutLine, 135 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.ZhufuShengyuTimeText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777015U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 14.0;
		gtextBlockOutLine.fontBold = true;
		Canvas.SetLeft(gtextBlockOutLine, 288);
		Canvas.SetTop(gtextBlockOutLine, 73 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.NextRongyaoJibeiText = gtextBlockOutLine;
		this.LinkText = U3DUtils.NEW<HyperLinkText>();
		this.Container.Children.Add(this.LinkText);
		Canvas.SetLeft(this.LinkText, 243);
		Canvas.SetTop(this.LinkText, 37 - num);
		TextFormat textFormat = new TextFormat();
		textFormat.font = HSTextField.fontName;
		textFormat.color = 3669815U;
		textFormat.size = 12;
		textFormat.underline = true;
		this.LinkText.TxtFormat = textFormat;
		this.LinkText.HtmlText = Global.GetLang("荣耀值如何获取？");
		this.LinkText.MouseLeftButtonUp = delegate(object s, EventArgs e)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("荣耀可通过战盟贡献、战盟日常、血战地府、战旗战、砸金蛋等途径获取"), new object[0]), 0, -1, -1, 0);
		};
		this.KaiqiZhufuIcon = U3DUtils.NEW<GIcon>();
		this.KaiqiZhufuIcon.Width = 81.0;
		this.KaiqiZhufuIcon.Height = 21.0;
		this.KaiqiZhufuIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.KaiqiZhufuIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.KaiqiZhufuIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.KaiqiZhufuIcon.Text = Global.GetLang("开启祝福");
		this.KaiqiZhufuIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.KaiqiZhufu();
		};
		Canvas.SetLeft(this.KaiqiZhufuIcon, 122);
		Canvas.SetTop(this.KaiqiZhufuIcon, 345 - num);
		this.Container.Children.Add(this.KaiqiZhufuIcon);
		this.TishengRongyaoIcon = U3DUtils.NEW<GIcon>();
		this.TishengRongyaoIcon.Width = 81.0;
		this.TishengRongyaoIcon.Height = 21.0;
		this.TishengRongyaoIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.TishengRongyaoIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.TishengRongyaoIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.TishengRongyaoIcon.Text = Global.GetLang("提升荣耀");
		this.TishengRongyaoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TishengRongyao();
		};
		Canvas.SetLeft(this.TishengRongyaoIcon, 322);
		Canvas.SetTop(this.TishengRongyaoIcon, 345 - num);
		this.Container.Children.Add(this.TishengRongyaoIcon);
		this.Container.Children.Add(this.SelectedImg);
		this.SelectedImg.IsHitTestVisible = false;
	}

	public void InitPartData()
	{
		this.currentRongyaoJibie = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYuLevel);
		this.SetSelectImg(this.currentRongyaoJibie);
		this.SetRongyao();
		this.SetCurrentShuxin();
		this.SetNextShuxin(this.currentRongyaoJibie + 1);
		this.SetCurrentTiaojian(false);
		this.SetNextTiaojian(this.currentRongyaoJibie + 1, false);
		this.InitRongyaoIconArr();
	}

	private void SetSelectImg(int level)
	{
		if (level >= 0)
		{
			int value = 11;
			int num = 26;
			if (level >= Global.MaxRongyao)
			{
				level = Global.MaxRongyao - 1;
			}
			Canvas.SetLeft(this.SelectedImg, value);
			Canvas.SetTop(this.SelectedImg, level % this.PageSize * 31 + num);
			this.SelectedImg.Visibility = true;
			this.CurrentSelectedPage = (int)Math.Floor((double)level / (double)this.PageSize);
		}
	}

	public void SetRongyao()
	{
		this.TotalRongyaoText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu).ToString();
	}

	public void SetZhufuShengyuTime()
	{
		BufferData bufferDataByID = Global.GetBufferDataByID(51);
		if (bufferDataByID != null)
		{
			double num = (double)bufferDataByID.BufferSecs * 1000.0;
			double num2 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num3 = Math.Max((num - num2) / 1000.0, 0.0);
			this.ZhufuShengyuTimeText.Text = StringUtil.substitute(Global.GetLang("{0}分钟"), new object[]
			{
				(int)(num3 / 60.0)
			});
		}
	}

	private void SetCurrentShuxin()
	{
		if (this.currentRongyaoJibie <= 0)
		{
			this.KaiqiZhufuIcon.Visibility = false;
			this.DangqianJingjieText.Text = string.Empty;
			this.ZhufuShengyuTimeText.Text = string.Empty;
			this.DangqianJingjie_KaiqiZhufuText.Text = string.Empty;
			this.DangqianJingjie_ShengmingShangxianText.Text = string.Empty;
			this.DangqianJingjie_MofaShangxianText.Text = string.Empty;
			this.DangqianJingjie_WuliGongjiText.Text = string.Empty;
			this.DangqianJingjie_MofaGongjiText.Text = string.Empty;
			this.DangqianJingjie_DaoshuGongjiText.Text = string.Empty;
			this.DangqianJingjie_WuliFangyuText.Text = string.Empty;
			this.DangqianJingjie_MofaFangyuText.Text = string.Empty;
			return;
		}
		this.KaiqiZhufuIcon.Visibility = true;
		this.SetZhufuShengyuTime();
		this.DangqianJingjieText.Text = this.currentRongyaoJibie.ToString();
		double[] rolePropList = this.GetRolePropList(this.currentRongyaoJibie);
		this.DangqianJingjie_ShengmingShangxianText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[14] * 100.0)
		});
		this.DangqianJingjie_MofaShangxianText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Math.Round(rolePropList[16] * 100.0)
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
		this.DangqianJingjie_WuliFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[3],
			rolePropList[4]
		});
		this.DangqianJingjie_MofaFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
		{
			rolePropList[5],
			rolePropList[6]
		});
	}

	private void SetNextShuxin(int level)
	{
		if (level - 1 != this.currentRongyaoJibie)
		{
			this.TishengRongyaoIcon.Visibility = false;
		}
		else
		{
			this.TishengRongyaoIcon.Visibility = true;
		}
		if (level <= Global.MaxRongyao)
		{
			this.SetRongyaoJingjieName(level);
			double[] rolePropList = this.GetRolePropList(level);
			this.XiayiJingjie_ShengmingShangxianText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Math.Round(rolePropList[14] * 100.0)
			});
			this.XiayiJingjie_MofaShangxianText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Math.Round(rolePropList[16] * 100.0)
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
			this.XiayiJingjie_WuliFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
			{
				rolePropList[3],
				rolePropList[4]
			});
			this.XiayiJingjie_MofaFangyuText.Text = StringUtil.substitute("{0}-{1}", new object[]
			{
				rolePropList[5],
				rolePropList[6]
			});
		}
		else
		{
			this.XiayiJingjie_ShengmingShangxianText.Text = string.Empty;
			this.XiayiJingjie_MofaShangxianText.Text = string.Empty;
			this.XiayiJingjie_WuliGongjiText.Text = string.Empty;
			this.XiayiJingjie_MofaGongjiText.Text = string.Empty;
			this.XiayiJingjie_DaoshuGongjiText.Text = string.Empty;
			this.XiayiJingjie_WuliFangyuText.Text = string.Empty;
			this.XiayiJingjie_MofaFangyuText.Text = string.Empty;
		}
	}

	private void SetRongyaoJingjieName(int level)
	{
		if (level > Global.MaxRongyao)
		{
			this.NextRongyaoJibeiText.Text = string.Empty;
		}
		else
		{
			this.NextRongyaoJibeiText.Text = level.ToString();
		}
	}

	private double[] GetRolePropList(int index)
	{
		int rongyaoBuffID = Global.GetRongyaoBuffID(index);
		return Global.GetGoodsEquipPropsDoubleList(rongyaoBuffID);
	}

	private void SetCurrentTiaojian(bool onlyColor = false)
	{
		if (!onlyColor)
		{
			XElement gameResXml = Global.GetGameResXml("Config/RongYu.xml");
			if (gameResXml == null)
			{
				return;
			}
			if (this.currentRongyaoJibie <= 0)
			{
				this.DangqianJingjie_KaiqiZhufuText.Text = string.Empty;
			}
			else
			{
				XElement xelement = Global.GetXElement(gameResXml, "RongYu", "ID", this.currentRongyaoJibie.ToString());
				if (xelement == null)
				{
					return;
				}
				this.DangqianJingjie_KaiqiZhufu = Global.GetXElementAttributeInt(xelement, "KaiQiXiaoHao");
				this.DangqianJingjie_KaiqiZhufuText.Text = this.DangqianJingjie_KaiqiZhufu.ToString();
			}
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu) < this.DangqianJingjie_KaiqiZhufu)
		{
			this.DangqianJingjie_KaiqiZhufuText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.DangqianJingjie_KaiqiZhufuText.TextColor = new SolidColorBrush(16777215U);
		}
	}

	private void SetNextTiaojian(int level, bool onlyColor = false)
	{
		if (!onlyColor)
		{
			if (this.currentRongyaoJibie >= Global.MaxRongyao)
			{
				this.TishengRongyaoIcon.Visibility = false;
			}
			else
			{
				this.TishengRongyaoIcon.Visibility = true;
			}
			XElement gameResXml = Global.GetGameResXml("Config/RongYu.xml");
			if (gameResXml == null)
			{
				return;
			}
			if (level <= Global.MaxRongyao)
			{
				XElement xelement = Global.GetXElement(gameResXml, "RongYu", "ID", level.ToString());
				if (xelement == null)
				{
					return;
				}
				this.XiayiJingjie_RoleLevel = Global.GetXElementAttributeInt(xelement, "LevelLimit");
				this.XiayiJingjie_ShengjiXiaohao = Global.GetXElementAttributeInt(xelement, "RongYu");
				this.XiayiJingjie_KaiqiZhufu = Global.GetXElementAttributeInt(xelement, "KaiQiXiaoHao");
				this.XiayiJingjie_RoleLevelText.Text = this.XiayiJingjie_RoleLevel.ToString();
				this.XiayiJingjie_ShengjiXiaohaoText.Text = this.XiayiJingjie_ShengjiXiaohao.ToString();
				this.XiayiJingjie_KaiqiZhufuText.Text = this.XiayiJingjie_KaiqiZhufu.ToString();
			}
			else
			{
				this.XiayiJingjie_RoleLevelText.Text = string.Empty;
				this.XiayiJingjie_ShengjiXiaohaoText.Text = string.Empty;
				this.XiayiJingjie_KaiqiZhufuText.Text = string.Empty;
			}
		}
		if (Global.Data.roleData.Level < this.XiayiJingjie_RoleLevel)
		{
			this.XiayiJingjie_RoleLevelText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.XiayiJingjie_RoleLevelText.TextColor = new SolidColorBrush(16777215U);
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu) < this.XiayiJingjie_ShengjiXiaohao)
		{
			this.XiayiJingjie_ShengjiXiaohaoText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.XiayiJingjie_ShengjiXiaohaoText.TextColor = new SolidColorBrush(16777215U);
		}
	}

	private void InitRongyaoIconArr()
	{
		if (this.ItemsList == null)
		{
			ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 60.0, 23.0, 3.0, 2.0));
			ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 60.0, 23.0, 3.0, 2.0));
			ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 60.0, 23.0, 3.0, 2.0));
			this.ItemsList = new List<GIcon>();
			for (int i = 0; i < Global.MaxRongyao; i++)
			{
				GIcon icon = U3DUtils.NEW<GIcon>();
				icon.Width = 60.0;
				icon.Height = 23.0;
				icon.BodySource = bodySource;
				icon.NewSource = newSource;
				icon.DisableBodySource = disableBodySource;
				icon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
				icon.Text = StringUtil.substitute(Global.GetLang("{0}级荣耀"), new object[]
				{
					i + 1
				});
				icon.ItemCode = i + 1;
				icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.SetSelectImg(icon.ItemCode - 1);
					this.SetNextShuxin(icon.ItemCode);
					this.SetNextTiaojian(icon.ItemCode, false);
				};
				this.ItemsList[i] = icon;
			}
		}
		this.MaxPageCount = (this.ItemsList.Count - 1) / this.PageSize + 1;
		this.ShowPage(this.CurrentSelectedPage);
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection.Clear();
		int num = pageIndex * this.PageSize;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + this.PageSize)
		{
			this.ItemsList[num2].EnableIcon = (num2 < this.currentRongyaoJibie);
			this.ItemsList[num2].buttonMode = true;
			this.ItemCollection.AddNoUpdate(this.ItemsList[num2]);
			num2++;
		}
		this.ItemCollection.DelayUpdate();
		if (pageIndex <= 0)
		{
			this.PrevPageIcon.EnableIcon = false;
			this.CurrentSelectedPage = 0;
		}
		else
		{
			this.PrevPageIcon.EnableIcon = true;
		}
		if (pageIndex >= this.MaxPageCount - 1)
		{
			this.NextPageIcon.EnableIcon = false;
			this.CurrentSelectedPage = this.MaxPageCount - 1;
		}
		else
		{
			this.NextPageIcon.EnableIcon = true;
		}
	}

	public void GetNewData()
	{
	}

	public void KaiqiZhufu()
	{
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu) < this.DangqianJingjie_KaiqiZhufu)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("荣耀值不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		BufferData bufferDataByID = Global.GetBufferDataByID(51);
		if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L, false))
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("荣耀buffer已经存在，使用\n\n后将覆盖原有状态，继续使用吗？"), new object[0]), (int)((this.Width - 253.0) / 2.0), (int)((this.Height - 171.0) / 2.0), (int)this.Width, (int)this.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SpriteActiveRongYuBuffer();
				}
				return true;
			};
			return;
		}
		GameInstance.Game.SpriteActiveRongYuBuffer();
	}

	public void NotifyKaiqiZhufuResult(int result)
	{
		if (result < 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("开启荣耀祝福失败"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.SetCurrentTiaojian(false);
		this.SetNextTiaojian(this.currentRongyaoJibie + 1, false);
	}

	public void TishengRongyao()
	{
		if (this.currentRongyaoJibie >= Global.MaxRongyao)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("荣耀已达到最大级别，无法提升"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.Level < this.XiayiJingjie_RoleLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级不够"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu) < this.XiayiJingjie_ShengjiXiaohao)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("荣耀值不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GameInstance.Game.SpriteActiveNextLevelRongYu();
	}

	public void NotifyTishengRongyaoResult(int result, int oldLevel, int nowLevel)
	{
		if (result < 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提升荣耀失败"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.InitPartData();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int currentRongyaoJibie;

	private GTextBlockOutLine TotalRongyaoText;

	private HyperLinkText LinkText;

	private GTextBlockOutLine DangqianJingjieText;

	private GTextBlockOutLine ZhufuShengyuTimeText;

	private GTextBlockOutLine DangqianJingjie_KaiqiZhufuText;

	private GTextBlockOutLine DangqianJingjie_ShengmingShangxianText;

	private GTextBlockOutLine DangqianJingjie_MofaShangxianText;

	private GTextBlockOutLine DangqianJingjie_WuliGongjiText;

	private GTextBlockOutLine DangqianJingjie_MofaGongjiText;

	private GTextBlockOutLine DangqianJingjie_DaoshuGongjiText;

	private GTextBlockOutLine DangqianJingjie_WuliFangyuText;

	private GTextBlockOutLine DangqianJingjie_MofaFangyuText;

	private GTextBlockOutLine NextRongyaoJibeiText;

	private GTextBlockOutLine XiayiJingjie_RoleLevelText;

	private GTextBlockOutLine XiayiJingjie_ShengjiXiaohaoText;

	private GTextBlockOutLine XiayiJingjie_KaiqiZhufuText;

	private GTextBlockOutLine XiayiJingjie_ShengmingShangxianText;

	private GTextBlockOutLine XiayiJingjie_MofaShangxianText;

	private GTextBlockOutLine XiayiJingjie_WuliGongjiText;

	private GTextBlockOutLine XiayiJingjie_MofaGongjiText;

	private GTextBlockOutLine XiayiJingjie_DaoshuGongjiText;

	private GTextBlockOutLine XiayiJingjie_WuliFangyuText;

	private GTextBlockOutLine XiayiJingjie_MofaFangyuText;

	private GIcon KaiqiZhufuIcon;

	private GIcon TishengRongyaoIcon;

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private ListBox listBox = new ListBox();

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private int PageSize = 10;

	private List<GIcon> ItemsList;

	private int DangqianJingjie_KaiqiZhufu;

	private int XiayiJingjie_RoleLevel;

	private int XiayiJingjie_ShengjiXiaohao;

	private int XiayiJingjie_KaiqiZhufu;

	private Image SelectedImg = new Image();

	private GTabControl _tc = U3DUtils.NEW<GTabControl>();

	private ObservableCollection _ItemCollection;
}
