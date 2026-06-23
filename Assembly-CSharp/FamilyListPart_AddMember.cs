using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class FamilyListPart_AddMember : UserControl
{
	protected override void InitializeComponent()
	{
		this.ItemCollection = this.lbPlayers.ItemsSource;
		this.lbPlayers.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbPlayers_SelectionChanged);
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

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.InitCtr();
	}

	private void InitCtr()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = Global.StringTrim(string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				if (text.get_Chars(0) == '[' && text.IndexOf(Global.GetLang("区]")) != -1)
				{
					try
					{
						text = text.Substring(text.IndexOf(']') + 1);
					}
					catch (Exception ex)
					{
						MUDebug.LogException(ex);
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.SearchPlayers(text);
				}
			}
		};
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			if (this.CurrentSelectedItem == null)
			{
				return;
			}
			if (this.CurrentSelectedItem.RoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.CurrentSelectedItem.RoleID);
			}
		};
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			if (this.CurrentSelectedItem == null)
			{
				return;
			}
			if (this.CurrentSelectedItem.RoleID != Global.Data.roleData.RoleID)
			{
				if (this.CurrentSelectedItem.RoleLevel < Global.JoinBangHuiNeedLevel)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("【{0}】已经是其他战盟的成员，无法再邀请!"), new object[]
					{
						this.CurrentSelectedItem.RoleName
					}), 10, 3);
					return;
				}
				if (this.CurrentSelectedItem.RoleLevel < Global.JoinBangHuiNeedLevel)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("角色等级小于【{0}】的无法成为战盟成员"), new object[]
					{
						Global.JoinBangHuiNeedLevel
					}), 10, 3);
					return;
				}
				if (Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData) > 0)
				{
					GameInstance.Game.SpriteAddBHMember(Global.Data.roleData.Faction, this.CurrentSelectedItem.RoleID, this.CurrentSelectedItem.RoleName, 1);
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
						{
							ID = 0,
							IDType = 0
						});
					}
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("普通成员无法执行添加成员的操作!"), 0, -1, -1, 0);
				}
			}
		};
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
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
	}

	private void SearchPlayers(string key)
	{
		if (key.Length < 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要搜索的关键字不能少于2个字"), 0, -1, -1, 0);
			return;
		}
		if (key.Length > 10)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要搜索的关键字不能超过7个字"), 0, -1, -1, 0);
			return;
		}
		this.ItemCollection.Clear();
		this.CurrentSelectedItem = null;
		this.SetButtonLight();
		string text = Global.StringReplaceAll(key, "'", string.Empty);
		text = Global.StringReplaceAll(text, "|", string.Empty);
		text = Global.StringReplaceAll(text, "$", string.Empty);
		text = Global.StringReplaceAll(text, ":", string.Empty);
		GameInstance.Game.SpriteSearchRolesFromDB(text, 0);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void lbPlayers_SelectionChanged(object sender, object e)
	{
		if (null != this.CurrentSelectedItem)
		{
			this.CurrentSelectedItem.BodyBackground = null;
		}
		FamilyListPart_AddMember_Item familyListPart_AddMember_Item = this.lbPlayers.SelectedItem.SafeGetComponent<FamilyListPart_AddMember_Item>();
		this.CurrentSelectedItem = familyListPart_AddMember_Item;
		this.SetButtonLight();
		if (null == familyListPart_AddMember_Item)
		{
			return;
		}
		familyListPart_AddMember_Item.BodyHeight = 20.0;
		familyListPart_AddMember_Item.BodyWidth = 230.0;
		familyListPart_AddMember_Item.BodyBackground = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 230.0, 20.0, 5.0, 5.0));
	}

	private void SetButtonLight()
	{
		if (this.CurrentSelectedItem != null)
		{
			this.iconShowInfo.EnableIcon = true;
			this.iconToMember.EnableIcon = true;
		}
		else
		{
			this.iconShowInfo.EnableIcon = false;
			this.iconToMember.EnableIcon = false;
		}
	}

	public void RefreshSearchData(List<SearchRoleData> searchRoleDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (searchRoleDataList == null)
		{
			return;
		}
		for (int i = 0; i < searchRoleDataList.Count; i++)
		{
			FamilyListPart_AddMember_Item familyListPart_AddMember_Item = U3DUtils.NEW<FamilyListPart_AddMember_Item>();
			familyListPart_AddMember_Item.gtbPlayerName.Text = searchRoleDataList[i].RoleName;
			familyListPart_AddMember_Item.gtbLevel.Text = searchRoleDataList[i].Level.ToString();
			familyListPart_AddMember_Item.gtbWork.Text = Global.GetOccupationStr(searchRoleDataList[i].Occupation);
			familyListPart_AddMember_Item.RoleID = searchRoleDataList[i].RoleID;
			familyListPart_AddMember_Item.RoleName = searchRoleDataList[i].RoleName;
			familyListPart_AddMember_Item.RoleLevel = searchRoleDataList[i].Level;
			familyListPart_AddMember_Item.Faction = searchRoleDataList[i].Faction;
			familyListPart_AddMember_Item.BodyHeight = 20.0;
			familyListPart_AddMember_Item.BodyWidth = 230.0;
			this.ItemCollection.AddNoUpdate(familyListPart_AddMember_Item);
		}
		this.ItemCollection.DelayUpdate();
	}

	public GButton btnQuite;

	public GButton btnAddMember;

	public GButton btnVerifyList;

	public UICheckbox chkAutoVerify;

	public ListBox listBox;

	private LoadingWindow LoadingWin;

	private FamilyListPart_AddMember_Item CurrentSelectedItem;

	private GIcon iconShowInfo;

	private GIcon iconToMember;

	private BangHuiDetailData MyBangHuiDetailData;

	private ListBox lbPlayers;

	private SpriteSL thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection _ItemCollection;
}
