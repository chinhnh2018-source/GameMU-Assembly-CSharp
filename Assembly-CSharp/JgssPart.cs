using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class JgssPart : UserControl
{
	public JgssPart()
	{
		this.ItemCollection = this.lbMenu.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.lbMenu);
		Canvas.SetLeft(this.lbMenu, 6);
		Canvas.SetTop(this.lbMenu, 6);
		this.lbMenu.Width = 81.0;
		this.lbMenu.Height = 153.0;
		this.lbMenu.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
		this.lbMenu.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbMenu_SelectionChanged);
		this.lbMenu.BorderThickness = 0;
		this.Container.Children.Add(this.txtNewTax);
		this.txtNewTax.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtNewTax, 203);
		Canvas.SetTop(this.txtNewTax, 47);
		this.Container.Children.Add(this.txtTotalTax);
		this.txtTotalTax.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtTotalTax, 203);
		Canvas.SetTop(this.txtTotalTax, 71);
		this.thisCtrl = this;
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
		this.CancleIcon = U3DUtils.NEW<GIcon>();
		this.CancleIcon.Width = 81.0;
		this.CancleIcon.Height = 21.0;
		this.CancleIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.CancleIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.CancleIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.CancleIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.CancleIcon.Text = Global.GetLang("取消");
		this.CancleIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(this.CancleIcon, 196);
		Canvas.SetTop(this.CancleIcon, 299);
		this.Container.Children.Add(this.CancleIcon);
		this.MaxTaxRateIcon = U3DUtils.NEW<GIcon>();
		this.MaxTaxRateIcon.Width = 66.0;
		this.MaxTaxRateIcon.Height = 21.0;
		this.MaxTaxRateIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxRateIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxRateIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxRateIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.MaxTaxRateIcon.Text = Global.GetLang("设置");
		this.MaxTaxRateIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiLingDiInfosDict == null)
			{
				return;
			}
			if (!Global.IsBangHuiLeader(Global.Data.roleData, this.MyBangHuiDetailData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能设置领地的税率"), 0, -1, -1, 0);
				return;
			}
			if (null == this.SelectedListItem)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请选择要操作的领地！"), 0, -1, -1, 0);
				return;
			}
			int num = Global.SafeConvertToInt32(this.txtTaxRate.EditText);
			if (num < 0 || num > 10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("领地的税率只能设置在【0~10】之间!"), 0, -1, -1, 0);
				return;
			}
			num = Global.GMin(10, num);
			num = Global.GMax(0, num);
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			if (!this.MyBangHuiLingDiInfosDict.TryGetValue(this.SelectedListItem.LingDiID, ref bangHuiLingDiInfoData))
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
		Canvas.SetLeft(this.MaxTaxRateIcon, 211);
		Canvas.SetTop(this.MaxTaxRateIcon, 13);
		this.Container.Children.Add(this.MaxTaxRateIcon);
		this.MaxTaxIcon = U3DUtils.NEW<GIcon>();
		this.MaxTaxIcon.Width = 66.0;
		this.MaxTaxIcon.Height = 21.0;
		this.MaxTaxIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.MaxTaxIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.MaxTaxIcon.Text = Global.GetLang("领取");
		this.MaxTaxIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiLingDiInfosDict == null)
			{
				return;
			}
			if (!Global.IsBangHuiLeader(Global.Data.roleData, this.MyBangHuiDetailData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能领取税收!"), 0, -1, -1, 0);
				return;
			}
			if (null == this.SelectedListItem)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请选择要操作的领地！"), 0, -1, -1, 0);
				return;
			}
			int num = Global.SafeConvertToInt32(this.txtTax.EditText);
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			if (!this.MyBangHuiLingDiInfosDict.TryGetValue(this.SelectedListItem.LingDiID, ref bangHuiLingDiInfoData))
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
		Canvas.SetLeft(this.MaxTaxIcon, 211);
		Canvas.SetTop(this.MaxTaxIcon, 115);
		this.Container.Children.Add(this.MaxTaxIcon);
		this.txtTaxRate = U3DUtils.NEW<GTextBlock>();
		this.txtTaxRate.BodyWidth = 38.0;
		this.txtTaxRate.BodyHeight = 21.0;
		this.txtTaxRate.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 38.0, 21.0, 3.0, 2.0));
		this.txtTaxRate.FontSize = 12;
		this.txtTaxRate.Onlydouble = true;
		this.txtTaxRate.EditText = "0";
		this.txtTaxRate.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		this.txtTaxRate.Text.TextChanged = new EventHandler(this.txtTaxRate_TextChanged);
		Canvas.SetLeft(this.txtTaxRate, 150);
		Canvas.SetTop(this.txtTaxRate, 15);
		this.Container.Children.Add(this.txtTaxRate);
		this.txtTax = U3DUtils.NEW<GTextBlock>();
		this.txtTax.BodyWidth = 117.0;
		this.txtTax.BodyHeight = 21.0;
		this.txtTax.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 117.0, 21.0, 3.0, 2.0));
		this.txtTax.FontSize = 12;
		this.txtTax.Onlydouble = true;
		this.txtTax.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		this.txtTax.Text.TextChanged = new EventHandler(this.txtTax_TextChanged);
		Canvas.SetLeft(this.txtTax, 91);
		Canvas.SetTop(this.txtTax, 117);
		this.Container.Children.Add(this.txtTax);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.GetData(bangHuiDetailData.BHID);
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

	private void GetData(int bhid)
	{
		this.BHID = bhid;
		GameInstance.Game.SpriteGetBHLingDiInfoDictByBHID(this.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void GetItemData()
	{
		if (null == this.SelectedListItem)
		{
			this.txtNewTax.Text = string.Empty;
			this.txtTotalTax.Text = string.Empty;
			this.txtTaxRate.EditText = string.Empty;
			return;
		}
		if (this.MyBangHuiLingDiInfosDict == null)
		{
			return;
		}
		BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
		if (!this.MyBangHuiLingDiInfosDict.TryGetValue(this.SelectedListItem.LingDiID, ref bangHuiLingDiInfoData))
		{
			return;
		}
		if (bangHuiLingDiInfoData != null)
		{
			this.txtNewTax.Text = bangHuiLingDiInfoData.YestodayTax.ToString();
			this.txtTotalTax.Text = bangHuiLingDiInfoData.TotalTax.ToString();
			this.txtTaxRate.EditText = bangHuiLingDiInfoData.LingDiTax.ToString();
		}
	}

	private void lbMenu_SelectionChanged(object sender, MouseEvent e)
	{
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.BodyBackground = null;
		}
		if (this.lbMenu.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem = U3DUtils.AS<JgssListItem>(this.lbMenu.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.GetItemData();
		this.txtTax.EditText = ((double)Convert.ToInt32(this.txtTotalTax.Text) * 0.25).ToString();
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.lbMenu.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	private void txtTaxRate_TextChanged(object sender, object e)
	{
	}

	private void txtTax_TextChanged(object sender, object e)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.txtTax.Text.Text);
			int num2 = (int)((double)Convert.ToInt32(this.txtTotalTax.Text) * 0.25);
			if (num >= num2)
			{
				num = num2;
			}
		}
		catch (Exception)
		{
			num = 0;
		}
		this.txtTax.Text.Text = num.ToString();
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
		if (lingDiID - 1 < 0 || lingDiID - 1 >= lingDiIDs2MapCodes.Length)
		{
			return 0;
		}
		return lingDiIDs2MapCodes[lingDiID - 1];
	}

	private void LoadList()
	{
		this.ItemCollection.Clear();
		if (this.MyBangHuiLingDiInfosDict != null)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiIDs2MapCodes", ',');
			foreach (KeyValuePair<int, BangHuiLingDiInfoData> keyValuePair in this.MyBangHuiLingDiInfosDict)
			{
				if (keyValuePair.Value.LingDiID != 1)
				{
					int mapCodeByLingDiID = this.GetMapCodeByLingDiID(keyValuePair.Value.LingDiID, systemParamIntArrayByName);
					string mapNameByCode = ConfigSettings.GetMapNameByCode(mapCodeByLingDiID, false);
					JgssListItem jgssListItem = U3DUtils.NEW<JgssListItem>();
					jgssListItem.BodyWidth = 81.0;
					jgssListItem.BodyHeight = 25.0;
					jgssListItem.BodyWidth = 81.0;
					jgssListItem.BodyHeight = 25.0;
					jgssListItem.Width = 81.0;
					jgssListItem.Height = 25.0;
					jgssListItem.MapName = mapNameByCode;
					jgssListItem.MapID = mapCodeByLingDiID;
					jgssListItem.LingDiID = keyValuePair.Value.LingDiID;
					this.ItemCollection.AddNoUpdate(jgssListItem);
				}
			}
			this.ItemCollection.DelayUpdate();
		}
		if (this.ItemCollection.Count > 0)
		{
			this.SelectListBox(0);
		}
	}

	public void NotifySetLingDiTaxResult(int retCode, int roleID, int bhid, int lingDiID, int newLingDiTax)
	{
		if (lingDiID == 1)
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
		if (!this.MyBangHuiLingDiInfosDict.TryGetValue(this.SelectedListItem.LingDiID, ref bangHuiLingDiInfoData))
		{
			return;
		}
		if (bangHuiLingDiInfoData != null)
		{
			bangHuiLingDiInfoData.LingDiTax = newLingDiTax;
			this.GetItemData();
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
		if (lingDiID == 1)
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
		if (!this.MyBangHuiLingDiInfosDict.TryGetValue(this.SelectedListItem.LingDiID, ref bangHuiLingDiInfoData))
		{
			return;
		}
		if (bangHuiLingDiInfoData != null)
		{
			bangHuiLingDiInfoData.TotalTax -= takeTaxMoney;
			this.GetItemData();
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

	private LoadingWindow LoadingWin;

	private GIcon CancleIcon;

	private GIcon MaxTaxRateIcon;

	private GIcon MaxTaxIcon;

	private GTextBlock txtTaxRate;

	private GTextBlock txtTax;

	private BangHuiDetailData MyBangHuiDetailData;

	private int BHID;

	private JgssListItem SelectedListItem;

	private Canvas Root;

	private ListBox lbMenu = new ListBox();

	private GTextBlockOutLine txtNewTax = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTotalTax = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private SpriteSL thisCtrl = new SpriteSL();

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 81.0, 25.0, 5.0, 5.0));

	private Dictionary<int, BangHuiLingDiInfoData> MyBangHuiLingDiInfosDict;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
