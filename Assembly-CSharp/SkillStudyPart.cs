using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class SkillStudyPart : UserControl
{
	public SkillStudyPart()
	{
		this.thisCtrl = this;
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtName);
		Canvas.SetLeft(this.txtName, 190);
		Canvas.SetTop(this.txtName, 59);
		this.txtName.FontSize = HSTextField.defaultFontSize;
		this.txtName.TextColor = new SolidColorBrush(4278236930U);
		this.Container.Children.Add(this.txtLevel);
		Canvas.SetLeft(this.txtLevel, 277);
		Canvas.SetTop(this.txtLevel, 76);
		this.txtLevel.FontSize = HSTextField.defaultFontSize;
		this.txtLevel.TextColor = new SolidColorBrush(4288720307U);
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 113);
		Canvas.SetTop(this.ScrollImg, 59);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("确定");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null == this.SelectedSkillIcon)
			{
				return;
			}
			int itemCode = this.SelectedSkillIcon.ItemCode;
			GameInstance.Game.SpriteChangeNumSkillID(itemCode);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 80);
		Canvas.SetTop(gicon, 220);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取消");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 179);
		Canvas.SetTop(gicon, 220);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 12.0;
		gicon.Height = 44.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn8_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn8_hover.png"));
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.XYMouseLeftButtonUp);
		Canvas.SetLeft(gicon, 151);
		Canvas.SetTop(gicon, 53);
		this.Container.Children.Add(gicon);
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "AutoAddLevel";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("技能熟练度 100% 时自动升级");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		gcheckBox.Check = Super.GData.AutoUpgradeSkillLevel;
		gcheckBox.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			Super.GData.AutoUpgradeSkillLevel = (sender as GCheckBox).Check;
		};
		Canvas.SetLeft(gcheckBox, 87);
		Canvas.SetTop(gcheckBox, 188);
		this.Container.Children.Add(gcheckBox);
	}

	public void InitPartData()
	{
	}

	public void GetNewData()
	{
		this.InitPartMenuItem();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	private void HideWindow()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	private List<MagicInfoVO> GetSkillListByOccupation()
	{
		if (!ConfigMagicInfos.IsValid())
		{
			return null;
		}
		List<MagicInfoVO> list = new List<MagicInfoVO>();
		foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
		{
			MagicInfoVO value = keyValuePair.Value;
			if (value.ToOcuupation == Global.Data.roleData.Occupation)
			{
				list.Add(value);
			}
		}
		return list;
	}

	private void InitPartMenuItem()
	{
		List<MagicInfoVO> skillListByOccupation = this.GetSkillListByOccupation();
		if (skillListByOccupation == null || skillListByOccupation.Count <= 0)
		{
			return;
		}
		this.ItemIconList.Clear();
		this.ItemMenuNamesList.Clear();
		for (int i = 0; i < skillListByOccupation.Count; i++)
		{
			MagicInfoVO magicInfoVO = skillListByOccupation[i];
			if (magicInfoVO.MagicType == -1)
			{
				int id = magicInfoVO.ID;
				SkillData skillDataByID = Global.GetSkillDataByID(id);
				if (skillDataByID != null)
				{
					int num = magicInfoVO.MagicIcon;
					if (num < 0)
					{
						num = 0;
					}
					GIcon gicon = U3DUtils.NEW<GIcon>();
					gicon.Width = 32.0;
					gicon.Height = 32.0;
					gicon.BodyURL = new ImageURL(Super.GetSkillImageURLFromIconCode(num.ToString(), string.Empty), false, 1);
					gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
					gicon.TipType = 2;
					gicon.Tip = StringUtil.substitute("{0},{1}", new object[]
					{
						id,
						(skillDataByID != null) ? skillDataByID.SkillLevel : 1
					});
					gicon.ItemCategory = 0;
					gicon.ItemCode = id;
					gicon.ItemObject = skillDataByID;
					gicon.BoxTypes = 2;
					gicon.Text = ((skillDataByID != null) ? skillDataByID.SkillLevel.ToString() : "1");
					gicon.TextHorizontalAlignment = global::Layout.Right;
					gicon.TextVerticalAlignment = global::Layout.Bottom;
					gicon.TextShadowColor = 4278190080U;
					gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
					this.ItemIconList.Add(gicon);
					this.ItemMenuNamesList.Add(magicInfoVO.Name);
				}
			}
		}
		for (int j = 0; j < this.ItemIconList.Count; j++)
		{
			if (this.ItemIconList[j].ItemCode == Global.GetNumSkillID())
			{
				this.ProcessMenuClick(j + 1);
				break;
			}
		}
	}

	private void XYMouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.ShowMenuWindow((int)Canvas.GetLeft(this.ScrollImg), (int)Canvas.GetTop(this.ScrollImg) + 50);
	}

	public void ShowMenuWindow(int px, int py)
	{
		this.HideWindow();
		int count = this.ItemIconList.Count;
		if (count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您还没有学会技能，无法修炼"), 0, -1, -1, 0);
			return;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)px;
		this.MenuWindow.Top = (double)py;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 180.0;
		this.MenuWindow.BodyHeight = (double)(count * 40 + 25);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < count; i++)
		{
			this.menuPart.AddMenuItem(i + 1, imageFileName, this.ItemMenuNamesList[i], this.ItemIconList[i]);
		}
		this.menuPart.RenderMenu(40);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.ProcessMenuClick(gmenuItem.MenuItemID);
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void ProcessMenuClick(int id)
	{
		this.ScrollImg.Clear();
		this.SelectedSkillIcon = this.ItemIconList[id - 1].Clone();
		this.ScrollImg.Children.Add(this.SelectedSkillIcon);
		this.txtName.Text = this.ItemMenuNamesList[id - 1];
		this.txtLevel.Text = this.ItemIconList[id - 1].Text;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private SpriteSL thisCtrl;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private List<GIcon> ItemIconList = new List<GIcon>();

	private List<string> ItemMenuNamesList = new List<string>();

	private GIcon SelectedSkillIcon;

	private Canvas Root;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas ScrollImg = new Canvas();
}
