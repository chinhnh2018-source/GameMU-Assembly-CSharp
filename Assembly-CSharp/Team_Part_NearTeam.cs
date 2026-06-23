using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class Team_Part_NearTeam : UserControl
{
	private void InitTextInPrefabs()
	{
		this.RefreshBtn.Text = Global.GetLang("刷新列表");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.centerController.onFinished = delegate()
		{
			if (this.pageCount > 1)
			{
				int radioPos = Mathf.RoundToInt(Math.Abs(this.panel.transform.localPosition.x)) / Mathf.RoundToInt(this.panel.clipRange.z) + 1;
				this.SetRadioPos(radioPos);
			}
		};
	}

	public ObservableCollection ItemCollection1
	{
		get
		{
			return this._ItemCollection1;
		}
		set
		{
			this._ItemCollection1 = value;
		}
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.InitCtr();
		this.lbTeam.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbTeam_SelectionChanged);
		this.ItemCollection1 = this.lbTeam.ItemsSource;
		if (this.FirstInitPartData)
		{
			this.FirstInitPartData = false;
		}
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		this.ListAllTeams(0);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitCtr()
	{
		this.RefreshBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int startIndex = 0;
			if (this.CurrentSearchTeamData != null)
			{
				startIndex = this.CurrentSearchTeamData.StartIndex;
			}
			this.ListAllTeams(startIndex);
		};
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

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection1.Clear();
		if (this.CurrentSearchTeamData != null)
		{
			List<TeamData> teamDataList = this.CurrentSearchTeamData.TeamDataList;
			if (teamDataList != null)
			{
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
						teamPartItem.PersonForce = Global.GetLang("人数：") + teamDataList.Count + "/5";
						teamPartItem.TeamName = Global.GetLang("位置:") + ConfigSettings.GetMapNameByCode(teamMemberData.MapCode, false);
						teamPartItem.PageType = 1;
						this.ItemCollection1.AddNoUpdate(teamPartItem);
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
				this.ItemCollection1.DelayUpdate();
				if (this.ItemCollection1.Count > 0)
				{
					this.lbTeam.SelectedIndex = 0;
				}
			}
		}
	}

	private void SetButtonLight(GIcon icon, bool enabled)
	{
		icon.EnableIcon = enabled;
	}

	private void lbTeam_SelectionChanged(object sender, EventArgs e)
	{
		if (null == this.lbTeam.LastSelectedItem && this.lbTeam.LastSelectedItem != this.lbTeam.SelectedItem)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbTeam.SelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = true;
			}
			return;
		}
		if (null != this.lbTeam.LastSelectedItem && null != this.lbTeam.SelectedItem && this.lbTeam.LastSelectedItem != this.lbTeam.SelectedItem)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbTeam.LastSelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = false;
			}
			teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbTeam.SelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = true;
			}
		}
	}

	private void ClearAllResult()
	{
		if (this.ItemCollection1 != null)
		{
			this.ItemCollection1.Clear();
		}
	}

	private void ListAllTeams(int startIndex)
	{
		this.ClearAllResult();
		GameInstance.Game.SpriteListAllTeams(startIndex);
		Super.ShowNetWaiting(string.Empty);
	}

	public void RefreshTeamDataList(SearchTeamData searchTeamData = null)
	{
		Super.HideNetWaiting();
		this.CurrentSearchTeamData = searchTeamData;
		this.RefreshItemsListPerpage();
	}

	public void RefreshItemsListPerpage()
	{
		this.ItemCollection1.Clear();
		if (this.CurrentSearchTeamData == null || this.CurrentSearchTeamData.TeamDataList == null)
		{
			return;
		}
		List<TeamData> teamDataList = this.CurrentSearchTeamData.TeamDataList;
		int num = (int)Mathf.Ceil((float)teamDataList.Count / 5f);
		for (int i = 0; i < num; i++)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.NEW<TeamPartPageItem>();
			this.ItemCollection1.AddNoUpdate(teamPartPageItem);
			teamPartPageItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(this, e);
				}
			};
			int num2 = (teamDataList.Count - i * 5 < 5) ? (teamDataList.Count - i * 5) : 5;
			teamPartPageItem.AddItems(teamDataList.GetRange(i * 5, num2));
		}
		this.pageCount = num;
		if (this.pageCount > 1)
		{
			Vector3 localScale = this.radioForeground.gameObject.transform.localScale;
			localScale.x *= (float)num;
			this.radioBackground.transform.localScale = localScale;
			this.SetRadioPos(1);
			this.radioGroup.SetActive(true);
		}
		else
		{
			this.radioGroup.SetActive(false);
		}
	}

	private void SetRadioPos(int currentPage)
	{
		float x = ((float)currentPage - (float)(this.pageCount + 1) / 2f) * this.radioForeground.gameObject.transform.localScale.x;
		Vector3 localPosition = this.radioForeground.gameObject.transform.localPosition;
		localPosition.x = x;
		this.radioForeground.gameObject.transform.localPosition = localPosition;
	}

	public GButton RefreshBtn;

	public ListBox lbTeam;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private SearchTeamData CurrentSearchTeamData;

	private GTextBlockOutLine Page = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public UICenterOnChild centerController;

	public UIPanel panel;

	public GameObject radioGroup;

	public UISprite radioBackground;

	public UISprite radioForeground;

	private int pageCount;

	private ObservableCollection _ItemCollection1;
}
