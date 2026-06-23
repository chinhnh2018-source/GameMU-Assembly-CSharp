using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class RoleAttributePart3_ChongXue : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.gtbXueName);
		this.gtbXueName.Text = "1";
		this.gtbXueName.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbXueName, 170);
		Canvas.SetTop(this.gtbXueName, 18);
		this.Container.Children.Add(this.gtbGain);
		this.gtbGain.Text = "2";
		this.gtbGain.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbGain, 170);
		Canvas.SetTop(this.gtbGain, 41);
		this.Container.Children.Add(this.gtbHave);
		this.gtbHave.Text = "3";
		this.gtbHave.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbHave, 170);
		Canvas.SetTop(this.gtbHave, 66);
		this.Container.Children.Add(this.gtbConsume);
		this.gtbConsume.Text = "4";
		this.gtbConsume.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbConsume, 205);
		Canvas.SetTop(this.gtbConsume, 110);
		this.Container.Children.Add(this.gtbLeftChongXueNum);
		this.gtbLeftChongXueNum.TextColor = new SolidColorBrush(16711680U);
		this.gtbLeftChongXueNum.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.gtbLeftChongXueNum, 205);
		Canvas.SetTop(this.gtbLeftChongXueNum, 131);
		this.Container.Children.Add(this.Good);
		this.Good.Width = 32.0;
		this.Good.Height = 32.0;
		Canvas.SetLeft(this.Good, 34);
		Canvas.SetTop(this.Good, 112);
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public string ChongXueNum
	{
		get
		{
			return this.gtbLeftChongXueNum.Text;
		}
		set
		{
			this.gtbLeftChongXueNum.Text = value;
		}
	}

	public string ChongXueName
	{
		get
		{
			return this.gtbXueName.Text;
		}
		set
		{
			this.gtbXueName.Text = value;
		}
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.InitCtr();
	}

	public int GetJingMaiBodyLevel()
	{
		return this.JingMaiBodyLevel;
	}

	public int GetJingMaiID()
	{
		return this.JingMaiID;
	}

	public int GetJingMaiLevel()
	{
		return this.JingMaiLevel;
	}

	public void InitPartData(int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
	{
		this.RefreshData(jingMaiBodyLevel, jingMaiID, jingMaiLevel);
	}

	private string GetXueWeiName(int jingMaiID, int jingMaiLevel)
	{
		if (jingMaiID == 0)
		{
			return this.arr1s[jingMaiLevel];
		}
		if (jingMaiID == 1)
		{
			return this.arr2s[jingMaiLevel];
		}
		if (jingMaiID == 2)
		{
			return this.arr3s[jingMaiLevel];
		}
		if (jingMaiID == 3)
		{
			return this.arr4s[jingMaiLevel];
		}
		if (jingMaiID == 4)
		{
			return this.arr5s[jingMaiLevel];
		}
		if (jingMaiID == 5)
		{
			return this.arr6s[jingMaiLevel];
		}
		if (jingMaiID == 6)
		{
			return this.arr7s[jingMaiLevel];
		}
		if (jingMaiID == 7)
		{
			return this.arr8s[jingMaiLevel];
		}
		return Global.GetLang("未知穴位");
	}

	public void RefreshData(int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
	{
		this.JingMaiBodyLevel = jingMaiBodyLevel;
		this.JingMaiID = jingMaiID;
		this.JingMaiLevel = jingMaiLevel;
		XElement jingMaiXmlItem = Global.GetJingMaiXmlItem(this.JingMaiID, Global.Data.roleData.Occupation, Global.Data.roleData.JingMaiBodyLevel - 1);
		if (jingMaiXmlItem != null)
		{
			this.ChongXueGoodsID = Global.GetXElementAttributeInt(jingMaiXmlItem, "GoodsID");
			this.ShowGoodsIcon(this.Good, this.ChongXueGoodsID, 1);
			this.gtbXueName.Text = this.GetXueWeiName(this.JingMaiID, this.JingMaiLevel - 1);
			int xelementAttributeInt = Global.GetXElementAttributeInt(jingMaiXmlItem, "Property");
			this.gtbGain.Text = StringUtil.substitute("{0} +{1}", new object[]
			{
				Global.GetXElementAttributeStr(jingMaiXmlItem, "PropName"),
				xelementAttributeInt
			});
			this.UseLingLi = Global.GetXElementAttributeInt(jingMaiXmlItem, "LingLi");
			this.NeedRoleLevel = Global.GetXElementAttributeInt(jingMaiXmlItem, "LevelLimit");
			this.gtbConsume.Text = this.UseLingLi.ToString();
			if (Global.Data.roleData.InterPower < this.UseLingLi)
			{
				this.gtbConsume.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
			}
			else
			{
				this.gtbConsume.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
			}
			this.gtbHave.Text = Global.Data.roleData.InterPower.ToString();
			this.OldSucceedRecte = 100;
			this.NewSucceedRecte = this.OldSucceedRecte;
		}
		this.gtbLeftChongXueNum.Text = Global.TodayChongXueNum().ToString();
	}

	private void InitCtr()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("运功冲穴");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DoChongXue(this.gtbXueName.Text);
		};
		Canvas.SetLeft(gicon, 50);
		Canvas.SetTop(gicon, 180);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 112.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("增加冲穴次数");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.AddTimesOk();
		};
		Canvas.SetLeft(gicon, 150);
		Canvas.SetTop(gicon, 180);
		this.Container.Children.Add(gicon);
	}

	private bool CanOpenPulse()
	{
		if (this.JingMaiBodyLevel > Global.MaxJingMaiBodyLevel)
		{
			return false;
		}
		if (this.JingMaiLevel > Global.MaxJingMaiLevel)
		{
			return false;
		}
		if (Global.Data.roleData.Level < this.NeedRoleLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("角色级别达到【{0}】级后，才能冲击本重经脉的穴位"), new object[]
			{
				this.NeedRoleLevel
			}), 0, -1, -1, 0);
			return false;
		}
		if (Global.Data.roleData.InterPower < this.UseLingLi)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您当前的灵力不足，无法冲穴"), 3, -1, -1, 0);
			return false;
		}
		if (!Global.TodayCanChongXue())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您今日剩余的冲穴次数已经为0，无法冲穴"), 2, -1, -1, 0);
			return false;
		}
		if (Global.GetTotalGoodsCountByID(this.ChongXueGoodsID) < 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有找到【{0}】，无法冲穴"), new object[]
			{
				Global.GetGoodsNameByID(this.ChongXueGoodsID, false)
			}), 19, -1, -1, this.ChongXueGoodsID);
			return false;
		}
		return true;
	}

	private void DoChongXue(string name)
	{
		if (!this.CanOpenPulse())
		{
			return;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = this.NewSucceedRecte
			});
		}
	}

	private void AddTimesOk()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2,
				IDType = 0
			});
		}
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.alpha = 0.01;
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

	private void InitChildWindow1(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void ShowSucceedRecte()
	{
		if (null != this.AddSucceedRecteWindow)
		{
			this.CloseChildWindow(this.AddSucceedRecteWindow);
			this.AddSucceedRecteWindow = null;
			this.addSucceedRectePart = null;
			this.CloseModalDialog();
			return;
		}
		this.ShowModalDialog();
		this.AddSucceedRecteWindow = U3DUtils.NEW<GChildWindow>();
		this.AddSucceedRecteWindow.Left = (double)(Super.GetChildLeft(480, 308) - 93);
		this.AddSucceedRecteWindow.Top = (double)(Super.GetChildTop(306, 166) - 77);
		this.AddSucceedRecteWindow.HeadLeft = 0.0;
		this.AddSucceedRecteWindow.HeadTop = 0.0;
		this.AddSucceedRecteWindow.HeadWidth = 308.0;
		this.AddSucceedRecteWindow.HeadHeight = 46.0;
		this.AddSucceedRecteWindow.BodyLeft = 0.0;
		this.AddSucceedRecteWindow.BodyTop = 46.0;
		this.AddSucceedRecteWindow.BodyWidth = 308.0;
		this.AddSucceedRecteWindow.BodyHeight = 166.0;
		this.InitChildWindow1(this.AddSucceedRecteWindow, Global.GetLang("提高成功率"), false);
		this.AddSucceedRecteWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.AddSucceedRecteWindow = null;
			this.addSucceedRectePart = null;
			return true;
		};
		Canvas.SetZIndex(this.AddSucceedRecteWindow, 9001.0);
		this.Root.Children.Add(this.AddSucceedRecteWindow);
		this.addSucceedRectePart = U3DUtils.NEW<AddSucceedRectePart>();
		this.addSucceedRectePart.SucceeRect = this.OldSucceedRecte;
		this.addSucceedRectePart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cmcgl_bak.png"), false, 0);
		this.addSucceedRectePart.InitPartSize((int)this.AddSucceedRecteWindow.BodyWidth - 18, (int)this.AddSucceedRecteWindow.BodyHeight - 9);
		this.addSucceedRectePart.InitPartData();
		this.addSucceedRectePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.NewSucceedRecte = this.OldSucceedRecte + Global.JingMaiLingZhiLuckyNum * e.IDType;
			this.CloseChildWindow(this.AddSucceedRecteWindow);
			this.AddSucceedRecteWindow = null;
			this.addSucceedRectePart = null;
			this.CloseModalDialog();
		};
		this.AddSucceedRecteWindow.SetContent(this.AddSucceedRecteWindow.BodyPresenter, this.addSucceedRectePart, 9.0, 0.0, true);
	}

	public void RefreshGoodsCount()
	{
		if (null != this.addSucceedRectePart)
		{
			this.addSucceedRectePart.RefreshGoodsCount();
		}
	}

	private void ShowGoodsIcon(Canvas img, int goodsID, int goodsNum)
	{
		img.Children.Clear();
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
			img.Children.Add(gicon);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int JingMaiBodyLevel = 1;

	private int JingMaiID;

	private int JingMaiLevel = 1;

	private int UseLingLi;

	private int NeedRoleLevel;

	private int OldSucceedRecte;

	private int NewSucceedRecte;

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private GChildWindow AddSucceedRecteWindow;

	private AddSucceedRectePart addSucceedRectePart;

	private Canvas Root;

	private GTextBlockOutLine gtbXueName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbGain = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbHave = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbConsume = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gtbLeftChongXueNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas Good = new Canvas();

	private int ChongXueGoodsID = -1;

	private string[] arr1s = Global.GetLang("金门,阳交,臑俞,天髎,肩井,头维,阳本神,阳白,阳临泣,阳目窗,阳正营,阳承灵,阳脑空,阳风池,风府,哑门,云门,侠白,尺泽,孔最,列缺,经渠,太渊,鱼际,少商").Split(new char[]
	{
		','
	});

	private string[] arr2s = Global.GetLang("冲门,府舍,大横,阴阳交,腹哀,期门,廉泉,极泉,青灵,少海,灵道,通里,阴郄,神门,少府,少冲,筑宾,中冲,劳宫,大陵,内关,间使,郄门,曲泽,天泉").Split(new char[]
	{
		','
	});

	private string[] arr3s = Global.GetLang("申脉,仆参,跗阳,居髎,大臑俞,肩髃,巨骨,秉风,曲垣,地仓,巨髎,承泣,跷风池,攒竹,眉冲,曲差,五处,承光,通天,络却,玉枕,天柱,承山,飞扬,昆仑").Split(new char[]
	{
		','
	});

	private string[] arr4s = Global.GetLang("然谷,照海,交信,阴谷,横谷,阴气冲,乳根,乳中,庸窗,天溪,胸乡,周荣,库房,气户,盆缺,人迎,晴明,不容,梁门,横鼻,足三里,丰隆,解溪,冲阳,属兑").Split(new char[]
	{
		','
	});

	private string[] arr5s = Global.GetLang("带穴,五枢,维道,天冲,浮白,头窍阴,完骨,带本神,带阳白,带临泣,带目窗,带正营,带承灵,带脑空,环跳,风市,中渎,膝阳关,外丘,光明,阳辅,悬钟,丘墟,足临泣,侠溪").Split(new char[]
	{
		','
	});

	private string[] arr6s = Global.GetLang("会阴,阴交,气冲,横骨,冲大赫,气穴,四满,中注,盲俞,商曲,石关,阴都,通谷,幽门,承满,关门,天枢,太乙,大巨,归来,滑肉门,外陵,水道,水突,气舍").Split(new char[]
	{
		','
	});

	private string[] arr7s = Global.GetLang("曲骨,中极,关元,下脘,中脘,上脘,天突,任廉泉,承浆,大钟,水泉,任大赫,灵墟,箕门,涌泉,神封,地机,血海,腹通谷,步廊,神藏,俞府,复溜,阴陵泉,漏谷").Split(new char[]
	{
		','
	});

	private string[] arr8s = Global.GetLang("兑端,龈交,水沟,素,神庭,上星,通卤会,百会,后顶排,强间,脑户,连风府,大椎,陶道开,神柱,神道,归灵台,至阳,筋缩,至中枢,脊中,悬枢,入命门,腰俞,长强").Split(new char[]
	{
		','
	});
}
