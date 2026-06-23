using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamPartPageItem : UserControl
{
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

	protected override void InitializeComponent()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListSelectHandle);
		base.GetComponent<BoxCollider>().enabled = false;
	}

	public void AddItems(List<TeamData> teamDataList)
	{
		this.itemCount = teamDataList.Count;
		this.ItemCollection.Clear();
		for (int i = 0; i < teamDataList.Count; i++)
		{
			TeamMemberData teamMemberData = this.FindLeaderTeamData(teamDataList[i]);
			if (teamMemberData != null)
			{
				TeamPartItem teamPartItem = U3DUtils.NEW<TeamPartItem>();
				teamPartItem.IsCaptaion = true;
				teamPartItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
				{
					Global.CalcOriginalOccupationID(teamMemberData.Occupation),
					teamMemberData.RoleSex
				});
				teamPartItem.RoleID = teamMemberData.RoleID;
				teamPartItem.PersonName = teamMemberData.RoleName;
				teamPartItem.PersonLevel = string.Concat(new object[]
				{
					"LV",
					teamMemberData.Level,
					"[",
					teamMemberData.ChangeLifeLev,
					Global.GetLang("转]")
				});
				teamPartItem.PersonForce = Global.GetLang("人数：") + ((teamDataList[i].TeamRoles != null) ? teamDataList[i].TeamRoles.Count : 0) + "/5";
				teamPartItem.TeamName = Global.GetLang("位置:") + ConfigSettings.GetMapNameByCode(teamMemberData.MapCode, false);
				teamPartItem.PageType = 1;
				teamPartItem.gameObject.AddComponent<BoxCollider>().size = new Vector3(172f, 258f, 1f);
				this.ItemCollection.AddNoUpdate(teamPartItem);
				teamPartItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.ShowChatBoxCallback != null)
					{
						this.ShowChatBoxCallback(s, e);
					}
				};
				UIPanel component = teamPartItem.transform.gameObject.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	public void AddItems(List<SearchRoleData> searchRoleDataList)
	{
		this.itemCount = searchRoleDataList.Count;
		this.ItemCollection.Clear();
		for (int i = 0; i < searchRoleDataList.Count; i++)
		{
			TeamPartItem teamPartItem = U3DUtils.NEW<TeamPartItem>();
			this.ItemCollection.AddNoUpdate(teamPartItem);
			teamPartItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(searchRoleDataList[i].Occupation),
				searchRoleDataList[i].RoleSex
			});
			teamPartItem.RoleID = searchRoleDataList[i].RoleID;
			teamPartItem.PersonName = searchRoleDataList[i].RoleName;
			teamPartItem.PersonLevel = string.Concat(new object[]
			{
				"LV",
				searchRoleDataList[i].Level,
				"[",
				searchRoleDataList[i].ChangeLifeLev,
				Global.GetLang("转]")
			});
			teamPartItem.PersonForce = Global.GetLang("战力：") + searchRoleDataList[i].CombatForce;
			teamPartItem.TeamName = ConfigSettings.GetMapNameByCode(searchRoleDataList[i].MapCode, false);
			teamPartItem.gameObject.AddComponent<BoxCollider>().size = new Vector3(172f, 258f, 1f);
			teamPartItem.PageType = 2;
			teamPartItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(s, e);
				}
			};
			UIPanel component = teamPartItem.transform.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	public void ListSelectHandle(object s, MouseEvent e)
	{
		int selectedIndex = this.listBox.SelectedIndex;
		if (selectedIndex != -1)
		{
			TeamPartItem teamPartItem = U3DUtils.AS<TeamPartItem>(this.ItemCollection[selectedIndex]);
			if (teamPartItem.RoleID != Global.Data.roleData.RoleID)
			{
				teamPartItem.CreateMenuWindow();
			}
			this.UnSelectedItem = this.SelectedItem;
			this.SelectedItem = teamPartItem;
			if (this.UnSelectedItem != this.SelectedItem)
			{
				if (this.UnSelectedItem != null)
				{
					this.UnSelectedItem.SelectedState = false;
				}
				if (this.SelectedItem != null)
				{
					this.SelectedItem.SelectedState = true;
				}
			}
			else if (this.SelectedItem != null)
			{
				this.SelectedItem.SelectedState = true;
			}
		}
		else if (this.SelectedItem != null)
		{
			this.SelectedItem.SelectedState = false;
		}
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (!this._SelectedState)
			{
				if (this.SelectedItem != null)
				{
					this.SelectedItem.SelectedState = false;
				}
				this.UnSelectedItem = null;
				this.SelectedItem = null;
			}
		}
	}

	private int CalculateClickArea()
	{
		int num = -1;
		if (UICamera.lastTouchPosition.y / Global.Data.ScreenScaleY < 398f && UICamera.lastTouchPosition.y / Global.Data.ScreenScaleY > 145f)
		{
			if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 40f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 211f)
			{
				num = 0;
			}
			else if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 215f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 389f)
			{
				num = 1;
			}
			else if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 392f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 569f)
			{
				num = 2;
			}
			else if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 571f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 747f)
			{
				num = 3;
			}
			else if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 750f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 923f)
			{
				num = 4;
			}
		}
		return (num >= this.itemCount) ? -1 : num;
	}

	private TeamMemberData FindLeaderTeamData(TeamData teamData)
	{
		if (teamData == null || Global.Data.roleData.RoleID == teamData.LeaderRoleID)
		{
			return null;
		}
		List<TeamMemberData> teamRoles = teamData.TeamRoles;
		if (teamRoles == null)
		{
			return null;
		}
		for (int i = 0; i < teamRoles.Count; i++)
		{
			if (teamRoles[i].RoleID == teamData.LeaderRoleID)
			{
				return teamRoles[i];
			}
		}
		return null;
	}

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public DPSelectedItemEventHandler ShowConfirmWinCallback;

	private TeamPartItem UnSelectedItem;

	private TeamPartItem SelectedItem;

	private new float scaleX = (float)Screen.width / 960f;

	private new float scaleY = (float)Screen.height / 540f;

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	private int itemCount = -1;

	private bool _SelectedState;
}
