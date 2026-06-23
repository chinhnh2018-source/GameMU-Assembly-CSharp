using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChangeZwPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnAppOk.Text = Global.GetLang("确定");
		this.btnAppNo.Text = Global.GetLang("取消");
		this.btnAppCommander.Text = Global.GetLang("首领");
		this.btnAppVCommander.Text = Global.GetLang("副首领");
		this.btnAppGeneralL.Text = Global.GetLang("左将军");
		this.btnAppGeneralR.Text = Global.GetLang("右将军");
		this.btnMember.Text = Global.GetLang("普通成员");
		if (this.ConstTexts != null && this.ConstTexts.Length == 3)
		{
			this.ConstTexts[0].Text = Global.GetLang("玩家当前职务为：");
			this.ConstTexts[1].Text = Global.GetLang("请选择职务：");
			this.ConstTexts[2].Text = Global.GetLang("首领职能：任命职位、升级战盟、修改公告、添加成员、删除成员");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.btnAppCommander.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.lblAppDuty.text = Global.GetLang("首领");
			this.lblDutyDesc.text = Global.GetLang("首领职能：任命职位、升级战盟、修改公告、添加成员、删除成员");
			this.SelectedMenuItemID = 1;
		};
		this.btnAppVCommander.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.lblAppDuty.text = Global.GetLang("副首领");
			this.lblDutyDesc.text = Global.GetLang("副首领职能：升级战盟、修改公告、添加成员、删除成员");
			this.SelectedMenuItemID = 2;
		};
		this.btnAppGeneralL.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.lblAppDuty.text = Global.GetLang("左将军");
			this.lblDutyDesc.text = Global.GetLang("左将军职能：修改公告、添加成员、删除成员");
			this.SelectedMenuItemID = 3;
		};
		this.btnAppGeneralR.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.lblAppDuty.text = Global.GetLang("右将军");
			this.lblDutyDesc.text = Global.GetLang("右将军职能：修改公告、添加成员、删除成员");
			this.SelectedMenuItemID = 4;
		};
		this.btnMember.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.lblAppDuty.text = Global.GetLang("普通成员");
			this.lblDutyDesc.text = Global.GetLang("普通成员职能：无");
			this.SelectedMenuItemID = 0;
		};
		this.btnAppOk.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OkZw();
		};
		this.btnAppNo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.transform.parent = null;
			Object.Destroy(base.transform.gameObject);
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.transform.parent = null;
			Object.Destroy(base.transform.gameObject);
		};
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._roleID;
		}
		set
		{
			this._roleID = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this._roleName;
		}
		set
		{
			this._roleName = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		if (this._roleID == -1)
		{
			return;
		}
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.SelectedMenuItemID = -1;
		this.lblCurDuty.text = Global.GetBHZhiWu(Global.GetBangHuiZhiWuByRoleID(this.RoleID, this.MyBangHuiDetailData));
	}

	public void RefreshPopupMenuByZhiwuID(int zhiwuId)
	{
		if (zhiwuId == 1)
		{
			this.btnAppCommander.gameObject.SetActive(true);
			this.btnAppVCommander.gameObject.SetActive(true);
		}
		else if (zhiwuId == 2)
		{
			this.btnAppCommander.gameObject.SetActive(false);
			this.btnAppVCommander.gameObject.SetActive(false);
		}
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

	public void ShowMenuWindow(int px, int py, int[] ids, string[] names)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
		}
		this.menuPart.RenderMenu(21);
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
		this.SelectedMenuItemID = id;
		for (int i = 0; i < this.MenuItemsIDs.Length; i++)
		{
			if (this.MenuItemsIDs[i] == id)
			{
				this.txtShowItem.EditText = this.MenusItemsNames[i];
				break;
			}
		}
		if (this.SelectedMenuItemID > 0)
		{
			BangHuiMgrItemData bangHuiMgrItemDataByZhiWu = Global.GetBangHuiMgrItemDataByZhiWu(this.SelectedMenuItemID, this.MyBangHuiDetailData);
			if (bangHuiMgrItemDataByZhiWu != null && bangHuiMgrItemDataByZhiWu.RoleID != this.RoleID)
			{
				this.txtHint_1.Text = StringUtil.substitute(Global.GetLang("{0}的目前担任者为：{1}"), new object[]
				{
					this.txtShowItem.EditText,
					Global.FormatRoleName(bangHuiMgrItemDataByZhiWu.ZoneID, bangHuiMgrItemDataByZhiWu.RoleName)
				});
				this.txtHint_2.Text = StringUtil.substitute(Global.GetLang("您确定要指定新的担任者：【{0}】吗？"), new object[]
				{
					this.RoleName
				});
			}
			else
			{
				this.txtHint_1.Text = string.Empty;
				this.txtHint_2.Text = string.Empty;
			}
		}
		else
		{
			this.txtHint_1.Text = string.Empty;
			this.txtHint_2.Text = string.Empty;
		}
	}

	private void OkZw()
	{
		if (this.SelectedMenuItemID < 0)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("请为【{0}】选择一个新的职务"), new object[]
			{
				this.RoleName
			}), 10, 3);
			return;
		}
		if (Global.Data.roleData.Faction != this.MyBangHuiDetailData.BHID)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("非【{0}】战盟成员无法操作"), new object[]
			{
				this.MyBangHuiDetailData.BHName
			}), 10, 3);
			return;
		}
		int bangHuiZhiWuByRoleID = Global.GetBangHuiZhiWuByRoleID(Global.Data.RoleID, this.MyBangHuiDetailData);
		if (bangHuiZhiWuByRoleID == 1)
		{
			if (this.SelectedMenuItemID != 1)
			{
				GameInstance.Game.SpriteUpdateBHMemberZhiWu(this.MyBangHuiDetailData.BHID, this.RoleID, this.SelectedMenuItemID);
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = this.SelectedMenuItemID
					});
				}
			}
			else
			{
				if (Global.IsHuangDi(Global.Data.roleData))
				{
					Super.HintMainText("您目前是本服的城主，无法将首领位置让出!", 10, 3);
					return;
				}
				if (Global.Data.roleData.HuangDiRoleID == Global.Data.roleData.RoleID)
				{
					Super.HintMainText("您目前是【舍利之源】持有者，无法将首领位置让出!", 10, 3);
					return;
				}
				GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("确认要将首领的位置让出吗？"), new object[0]), 0, 0, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteUpdateBHMemberZhiWu(this.MyBangHuiDetailData.BHID, this.RoleID, this.SelectedMenuItemID);
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
							{
								ID = -1
							});
						}
					}
					return true;
				};
			}
		}
		else
		{
			Super.HintMainText("只有首领才能执行修改职务操作!", 10, 3);
		}
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
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

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	public GameObject appMemberWin;

	public GButton btnAppCommander;

	public GButton btnAppVCommander;

	public GButton btnAppGeneralL;

	public GButton btnAppGeneralR;

	public GButton btnMember;

	public UILabel lblCurDuty;

	public UILabel lblAppDuty;

	public UILabel lblDutyDesc;

	public GButton btnAppOk;

	public GButton btnAppNo;

	public GButton btnClose;

	private int SelectedMenuItemID = -1;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	public int[] MenuItemsIDs = new int[]
	{
		-1,
		0,
		1,
		2,
		3,
		4
	};

	public string[] MenusItemsNames = new string[]
	{
		Global.GetLang("选择职务"),
		Global.GetLang("普通成员"),
		Global.GetLang("首领"),
		Global.GetLang("副首领"),
		Global.GetLang("左长老"),
		Global.GetLang("右长老")
	};

	public GTextBlock txtShowItem;

	private int _roleID = -1;

	private string _roleName = string.Empty;

	private BangHuiDetailData MyBangHuiDetailData;

	private Canvas PlaceHolder;

	private Canvas Root;

	private GTextBlockOutLine txtHint_1;

	private GTextBlockOutLine txtHint_2;

	private SpriteSL thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock[] ConstTexts;
}
