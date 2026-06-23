using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class YangZhouShouShuiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtZuoRi);
		this.txtZuoRi.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtZuoRi, 149);
		Canvas.SetTop(this.txtZuoRi, 47);
		this.Container.Children.Add(this.txtCount);
		this.txtCount.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtCount, 149);
		Canvas.SetTop(this.txtCount, 71);
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("设置");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		Canvas.SetLeft(gicon, 163);
		Canvas.SetTop(gicon, 13);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiLingDiInfosDict == null)
			{
				return;
			}
			if (!Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有本服城主才能设置扬州城的税率"), 0, -1, -1, 0);
				return;
			}
			int num = Global.SafeConvertToInt32(this.rateGtext.EditText);
			if (num < 0 || num > 10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("扬州城的税率只能设置在【0~10】之间!"), 0, -1, -1, 0);
				return;
			}
			num = Global.GMin(10, num);
			num = Global.GMax(0, num);
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			if (!this.MyBangHuiLingDiInfosDict.TryGetValue(1, ref bangHuiLingDiInfoData))
			{
				return;
			}
			if (bangHuiLingDiInfoData != null && bangHuiLingDiInfoData.LingDiTax != num)
			{
				GameInstance.Game.SpriteSetLingDiTax(bangHuiLingDiInfoData.LingDiID, num);
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
			}
		};
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Width = 81.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.Text = Global.GetLang("取消");
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		Canvas.SetLeft(gicon2, 148);
		Canvas.SetTop(gicon2, 278);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Width = 66.0;
		gicon3.Height = 21.0;
		gicon3.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon3.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon3, 163);
		Canvas.SetTop(gicon3, 115);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiLingDiInfosDict == null)
			{
				return;
			}
			if (!Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有本服城主才能领取扬州城的税收"), 0, -1, -1, 0);
				return;
			}
			int num = Global.SafeConvertToInt32(this.incomeGtext.EditText);
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			if (!this.MyBangHuiLingDiInfosDict.TryGetValue(1, ref bangHuiLingDiInfoData))
			{
				return;
			}
			if (bangHuiLingDiInfoData != null && num > 0 && (double)num <= (double)bangHuiLingDiInfoData.TotalTax * 0.25)
			{
				GameInstance.Game.SpriteTakeTaxMoney(bangHuiLingDiInfoData.LingDiID, num);
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
			}
		};
		this.rateGtext = U3DUtils.NEW<GTextBlock>();
		this.rateGtext.BodyWidth = 43.0;
		this.rateGtext.BodyHeight = 21.0;
		this.rateGtext.EditText = "0";
		this.rateGtext.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 43.0, 21.0, 3.0, 2.0));
		this.rateGtext.ReadOnly = false;
		this.rateGtext.Onlydouble = true;
		Canvas.SetLeft(this.rateGtext, 88);
		Canvas.SetTop(this.rateGtext, 15);
		this.Container.Children.Add(this.rateGtext);
		this.rateGtext.Text.TextChanged = new EventHandler(this.TextChanged);
		this.incomeGtext = U3DUtils.NEW<GTextBlock>();
		this.incomeGtext.BodyWidth = 117.0;
		this.incomeGtext.BodyHeight = 21.0;
		this.incomeGtext.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 117.0, 21.0, 3.0, 2.0));
		this.incomeGtext.ReadOnly = false;
		this.incomeGtext.Onlydouble = true;
		Canvas.SetLeft(this.incomeGtext, 29);
		Canvas.SetTop(this.incomeGtext, 117);
		this.Container.Children.Add(this.incomeGtext);
		this.incomeGtext.Text.TextChanged = new EventHandler(this.TextChanged2);
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteGetBHLingDiInfoDictByBHID(Global.Data.roleData.Faction);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void TextChanged(object sender, object e)
	{
	}

	private void TextChanged2(object sender, object e)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.incomeGtext.Text.Text);
			int num2 = Convert.ToInt32((double)Convert.ToInt32(this.txtCount.Text) * 0.25);
			if (num >= num2)
			{
				num = num2;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 0;
		}
		this.incomeGtext.Text.Text = num.ToString();
	}

	public void NotifyBangHuiLingDiInfosDict(Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfosDict)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.MyBangHuiLingDiInfosDict = bangHuiLingDiInfosDict;
		this.LoadList();
	}

	private int GetMapCodeByLingDiID(int lingDiID, int[] lingDiIDs2MapCodes)
	{
		if (lingDiIDs2MapCodes == null)
		{
			return 0;
		}
		if (lingDiID < 0 || lingDiID >= lingDiIDs2MapCodes.Length)
		{
			return 0;
		}
		return lingDiIDs2MapCodes[lingDiID - 1];
	}

	private void LoadList()
	{
		this.rateGtext.Text.Text = "0";
		this.txtZuoRi.Text = "0";
		this.txtCount.Text = "0";
		this.incomeGtext.Text.Text = "0";
		if (this.MyBangHuiLingDiInfosDict != null)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiIDs2MapCodes", ',');
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			if (this.MyBangHuiLingDiInfosDict.TryGetValue(1, ref bangHuiLingDiInfoData))
			{
				int mapCodeByLingDiID = this.GetMapCodeByLingDiID(bangHuiLingDiInfoData.LingDiID, systemParamIntArrayByName);
				string mapNameByCode = ConfigSettings.GetMapNameByCode(mapCodeByLingDiID, false);
				this.rateGtext.Text.Text = bangHuiLingDiInfoData.LingDiTax.ToString();
				this.txtZuoRi.Text = bangHuiLingDiInfoData.YestodayTax.ToString();
				this.txtCount.Text = bangHuiLingDiInfoData.TotalTax.ToString();
				this.incomeGtext.Text.Text = "0";
			}
		}
	}

	public void NotifySetLingDiTaxResult(int retCode, int roleID, int bhid, int lingDiID, int newLingDiTax)
	{
		if (lingDiID != 1)
		{
			return;
		}
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (retCode < 0)
		{
			if (retCode == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在！"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有首领才能设置领地税率"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1010)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有首领才能设置领地税率"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("设置领地税率时发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
		if (!this.MyBangHuiLingDiInfosDict.TryGetValue(lingDiID, ref bangHuiLingDiInfoData))
		{
			return;
		}
		if (bangHuiLingDiInfoData != null)
		{
			bangHuiLingDiInfoData.LingDiTax = newLingDiTax;
			this.LoadList();
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiIDs2MapCodes", ',');
			int mapCodeByLingDiID = this.GetMapCodeByLingDiID(lingDiID, systemParamIntArrayByName);
			string mapNameByCode = ConfigSettings.GetMapNameByCode(mapCodeByLingDiID, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("为【{0}】设置税率【{1}】成功"), new object[]
			{
				mapNameByCode,
				newLingDiTax
			}), 0, -1, -1, 0);
		}
	}

	public void NotifyTakeTaxMoneyResult(int retCode, int roleID, int bhid, int lingDiID, int takeTaxMoney)
	{
		if (lingDiID != 1)
		{
			return;
		}
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (retCode < 0)
		{
			if (retCode == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在！"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有首领才能设置领地税率"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1020)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不能设置其他战盟的领地税率"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1030)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("每天提取的税收不能超过总税收的25%"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1040)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("每个领地每天只能提取一次税收"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("设置领地税率时发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
		if (!this.MyBangHuiLingDiInfosDict.TryGetValue(lingDiID, ref bangHuiLingDiInfoData))
		{
			return;
		}
		if (bangHuiLingDiInfoData != null)
		{
			bangHuiLingDiInfoData.TotalTax -= takeTaxMoney;
			this.LoadList();
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiIDs2MapCodes", ',');
			int mapCodeByLingDiID = this.GetMapCodeByLingDiID(lingDiID, systemParamIntArrayByName);
			string mapNameByCode = ConfigSettings.GetMapNameByCode(mapCodeByLingDiID, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("从【{0}】提取了【{1}】税收"), new object[]
			{
				mapNameByCode,
				takeTaxMoney
			}), 0, -1, -1, 0);
		}
	}

	private GTextBlock rateGtext;

	private GTextBlock incomeGtext;

	private LoadingWindow LoadingWin;

	private Dictionary<int, BangHuiLingDiInfoData> MyBangHuiLingDiInfosDict;

	private Canvas Root;

	private GTextBlockOutLine txtZuoRi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtCount = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;
}
