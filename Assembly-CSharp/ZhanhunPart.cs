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

public class ZhanhunPart : UserControl
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
		this.listBox.Width = 300.0;
		this.listBox.Height = 48.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 10.0, 0.0);
		Canvas.SetLeft(this.listBox, 87);
		Canvas.SetTop(this.listBox, 288 - num);
		this.ItemCollection = this.listBox.ItemsSource;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 97);
		Canvas.SetTop(gtextBlockOutLine, 40 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TotalZhanhunText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777080U);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 14.0;
		gtextBlockOutLine.fontBold = true;
		Canvas.SetLeft(gtextBlockOutLine, 300);
		Canvas.SetTop(gtextBlockOutLine, 37 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjieText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 389);
		Canvas.SetTop(gtextBlockOutLine, 84 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WushiWuliFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 389);
		Canvas.SetTop(gtextBlockOutLine, 109 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WushiMofaFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 363);
		Canvas.SetTop(gtextBlockOutLine, 133 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 363);
		Canvas.SetTop(gtextBlockOutLine, 157 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 363);
		Canvas.SetTop(gtextBlockOutLine, 181 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_DaoshuGongjiText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 363);
		Canvas.SetTop(gtextBlockOutLine, 205 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_WuliFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 363);
		Canvas.SetTop(gtextBlockOutLine, 229 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.DangqianJingjie_MofaFangyuText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 116);
		Canvas.SetTop(gtextBlockOutLine, 356 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.LevelText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(16777215U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 244);
		Canvas.SetTop(gtextBlockOutLine, 356 - num);
		this.Container.Children.Add(gtextBlockOutLine);
		this.ZhanhunText = gtextBlockOutLine;
		this.PrevPageIcon = U3DUtils.NEW<GIcon>();
		this.PrevPageIcon.Width = 51.0;
		this.PrevPageIcon.Height = 51.0;
		this.PrevPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_normal.png"));
		this.PrevPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_hover.png"));
		this.PrevPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxuePrePage_nouse.png"));
		this.PrevPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PrevPageIcon.EnableIcon)
			{
				this.PrevPage();
			}
		};
		Canvas.SetLeft(this.PrevPageIcon, 17);
		Canvas.SetTop(this.PrevPageIcon, 287 - num);
		this.Container.Children.Add(this.PrevPageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 51.0;
		this.NextPageIcon.Height = 51.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/wuxueNextPage_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NextPageIcon.EnableIcon)
			{
				this.NextPage();
			}
		};
		Canvas.SetLeft(this.NextPageIcon, 389);
		Canvas.SetTop(this.NextPageIcon, 287 - num);
		this.Container.Children.Add(this.NextPageIcon);
		this.JihuoIcon = U3DUtils.NEW<GIcon>();
		this.JihuoIcon.Width = 81.0;
		this.JihuoIcon.Height = 21.0;
		this.JihuoIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.JihuoIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.JihuoIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.JihuoIcon.Text = Global.GetLang("立即提升");
		this.JihuoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartJihuo();
		};
		Canvas.SetLeft(this.JihuoIcon, 190);
		Canvas.SetTop(this.JihuoIcon, 385 - num);
		this.Container.Children.Add(this.JihuoIcon);
		this.Container.Children.Add(this.SelectedImg);
		this.SelectedImg.IsHitTestVisible = false;
	}

	public void InitPartData()
	{
		this.currentZhanhunJibie = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhanHunLevel);
		this.SetSelectImg(this.currentZhanhunJibie);
		this.SetZhanhun();
		if (this.currentZhanhunJibie <= 0)
		{
			this.SetShuxin(this.currentZhanhunJibie + 1);
		}
		else
		{
			this.SetShuxin(this.currentZhanhunJibie);
		}
		if (this.currentZhanhunJibie >= Global.MaxZhanhun)
		{
			this.SetTiaojian(this.currentZhanhunJibie);
		}
		else
		{
			this.SetTiaojian(this.currentZhanhunJibie + 1);
		}
		this.SetEffect(this.currentZhanhunJibie);
		this.InitZhanhunIconArr();
	}

	public void GetNewData()
	{
		this.InitPartData();
	}

	private void SetSelectImg(int level)
	{
		if (level >= 0)
		{
			int num = 83;
			int value = 220;
			Canvas.SetLeft(this.SelectedImg, level % this.PageSize * 58 + num);
			Canvas.SetTop(this.SelectedImg, value);
			this.SelectedImg.IsHitTestVisible = true;
			this.CurrentSelectedPage = (int)Math.Floor((double)level / (double)this.PageSize);
		}
	}

	public void SetZhanhun()
	{
		this.TotalZhanhunText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhanHun).ToString();
	}

	private void SetZhanhunJingjieName(int level)
	{
		if (this.currentZhanhunJibie >= level)
		{
			this.DangqianJingjieText.TextColor = new SolidColorBrush(16777080U);
			this.DangqianJingjieText.Text = StringUtil.substitute(Global.GetLang("{0}"), new object[]
			{
				this.GetZhanhunName(level)
			});
		}
		else
		{
			this.DangqianJingjieText.TextColor = new SolidColorBrush(8421504U);
			this.DangqianJingjieText.Text = StringUtil.substitute(Global.GetLang("{0}"), new object[]
			{
				this.GetZhanhunName(level)
			});
		}
	}

	private string GetZhanhunName(int index)
	{
		int zhanhunBuffID = Global.GetZhanhunBuffID(index);
		if (zhanhunBuffID != 0)
		{
			return Global.GetGoodsNameByID(zhanhunBuffID, false);
		}
		return string.Empty;
	}

	private double[] GetRolePropList(int index)
	{
		int zhanhunBuffID = Global.GetZhanhunBuffID(index);
		return Global.GetGoodsEquipPropsDoubleList(zhanhunBuffID);
	}

	private void SetShuxin(int level)
	{
		if (level <= 0)
		{
			this.DangqianJingjieText.Text = string.Empty;
			this.DangqianJingjie_WushiWuliFangyuText.Text = string.Empty;
			this.DangqianJingjie_WushiMofaFangyuText.Text = string.Empty;
			this.DangqianJingjie_WuliGongjiText.Text = string.Empty;
			this.DangqianJingjie_MofaGongjiText.Text = string.Empty;
			this.DangqianJingjie_DaoshuGongjiText.Text = string.Empty;
			this.DangqianJingjie_WuliFangyuText.Text = string.Empty;
			this.DangqianJingjie_MofaFangyuText.Text = string.Empty;
		}
		else
		{
			this.SetZhanhunJingjieName(level);
			double[] rolePropList = this.GetRolePropList(level);
			this.DangqianJingjie_WushiWuliFangyuText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Math.Round(rolePropList[28] * 100.0)
			});
			this.DangqianJingjie_WushiMofaFangyuText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Math.Round(rolePropList[255] * 100.0)
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
	}

	private void SetTiaojian(int level)
	{
		if (level <= 0)
		{
			this.LevelText.Text = string.Empty;
			this.ZhanhunText.Text = string.Empty;
		}
		else
		{
			XElement xelement = null;
			if (xelement == null)
			{
				return;
			}
			XElement xelement2 = Global.GetXElement(xelement, "ZhanHun", "ID", level.ToString());
			if (xelement2 == null)
			{
				return;
			}
			this.needZhanhun = Global.GetXElementAttributeInt(xelement2, "ZhanHun");
			this.needLevel = Global.GetXElementAttributeInt(xelement2, "LevelLimit");
			this.LevelText.Text = this.needLevel.ToString();
			this.ZhanhunText.Text = this.needZhanhun.ToString();
			if (Global.Data.roleData.Level < this.needLevel)
			{
				this.LevelText.TextColor = new SolidColorBrush(16711680U);
			}
			else
			{
				this.LevelText.TextColor = new SolidColorBrush(16777215U);
			}
			if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhanHun) < this.needZhanhun)
			{
				this.ZhanhunText.TextColor = new SolidColorBrush(16711680U);
			}
			else
			{
				this.ZhanhunText.TextColor = new SolidColorBrush(16777215U);
			}
		}
	}

	public void SetEffect(int level)
	{
		level = Math.Min(level, Global.MaxZhanhun - 1);
		if (this.DecoArr[level] == null)
		{
			GDecoration decoration = Global.GetDecoration(this.DecoZhanhunIDs[level], GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
			decoration.Coordinate = new Point(this.DecoZhanhunPos[level, 0], this.DecoZhanhunPos[level, 1]);
			this.DecoArr[level] = decoration;
		}
		for (int i = 0; i < this.DecoArr.Count; i++)
		{
			if (this.DecoArr[i] != null)
			{
				if (i == level)
				{
					this.DecoArr[i].Pause = false;
				}
				else
				{
					this.DecoArr[i].Pause = true;
				}
			}
		}
	}

	public void PauseAllEffect(bool pause)
	{
		for (int i = 0; i < this.DecoArr.Count; i++)
		{
			if (this.DecoArr[i] != null)
			{
				this.DecoArr[i].Pause = pause;
			}
		}
	}

	public void delEffect()
	{
	}

	private void InitZhanhunIconArr()
	{
		if (this.ItemsList == null)
		{
			this.ItemsList = new List<GIcon>();
			for (int i = 0; i < Global.MaxZhanhun; i++)
			{
				GIcon icon = U3DUtils.NEW<GIcon>();
				icon.Width = 48.0;
				icon.Height = 48.0;
				icon.BodySource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Zhanhun/0/{0}.png", new object[]
				{
					i + 1
				})));
				icon.NewSource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Zhanhun/1/{0}.png", new object[]
				{
					i + 1
				})));
				icon.DisableBodySource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Zhanhun/2/{0}.png", new object[]
				{
					i + 1
				})));
				icon.ItemCode = i + 1;
				icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.SetSelectImg(icon.ItemCode - 1);
					this.SetEffect(icon.ItemCode - 1);
					this.SetShuxin(icon.ItemCode);
					this.SetTiaojian(icon.ItemCode);
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
			this.ItemsList[num2].EnableIcon = (num2 < this.currentZhanhunJibie);
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

	private void StartJihuo()
	{
		if (this.currentZhanhunJibie >= Global.MaxZhanhun)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战魂境界已达到最高级，无法提升"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.Level < this.needLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("角色等级太低，无法激活，请升级后再次进行激活"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhanHun) < this.needZhanhun)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("角色战魂值不足，无法激活。"), new object[0]), 19, -1, -1, 36090);
			return;
		}
		GameInstance.Game.SpriteActiveNextLevelZhanHun();
	}

	public void NotifyZhanhunResult(int result, int oldLevel, int nowLevel)
	{
		if (result < 1)
		{
			if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("激活下一级武魂等级出错,错误码:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功将战魂从{0}级提升到{1}级"), new object[]
			{
				oldLevel,
				nowLevel
			}), 0, -1, -1, 0);
			this.InitPartData();
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int currentZhanhunJibie;

	private GTextBlockOutLine TotalZhanhunText;

	private GTextBlockOutLine DangqianJingjieText;

	private int needZhanhun;

	private int needLevel;

	private GTextBlockOutLine LevelText;

	private GTextBlockOutLine ZhanhunText;

	private GTextBlockOutLine DangqianJingjie_WushiWuliFangyuText;

	private GTextBlockOutLine DangqianJingjie_WushiMofaFangyuText;

	private GTextBlockOutLine DangqianJingjie_WuliGongjiText;

	private GTextBlockOutLine DangqianJingjie_MofaGongjiText;

	private GTextBlockOutLine DangqianJingjie_DaoshuGongjiText;

	private GTextBlockOutLine DangqianJingjie_WuliFangyuText;

	private GTextBlockOutLine DangqianJingjie_MofaFangyuText;

	private GIcon JihuoIcon;

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private ListBox listBox = new ListBox();

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private int PageSize = 5;

	private List<GIcon> ItemsList;

	private Image SelectedImg = new Image();

	private GTabControl _tc = U3DUtils.NEW<GTabControl>();

	private ObservableCollection _ItemCollection;

	public int[] DecoZhanhunIDs = new int[]
	{
		601,
		602,
		603,
		604,
		605,
		606,
		607,
		608,
		609,
		610,
		611,
		612
	};

	public int[,] DecoZhanhunPos = new int[,]
	{
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			92
		},
		{
			125,
			93
		},
		{
			125,
			95
		},
		{
			126,
			83
		}
	};

	public List<GDecoration> DecoArr = new List<GDecoration>();
}
