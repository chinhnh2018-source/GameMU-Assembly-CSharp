using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class Team_Part_NearPlayer : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnRefresh.Text = Global.GetLang("刷新列表");
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

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.lbPlayer.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbPlayer_SelectionChanged);
		this.ItemCollection = this.lbPlayer.ItemsSource;
		this.InitCtr();
	}

	public void ResetGetNewData()
	{
	}

	public void GetNewData()
	{
		this.ListMapRoles(0);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitCtr()
	{
		this.btnRefresh.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ListMapRoles(0);
		};
	}

	private void ListMapRoles(int startIndex)
	{
		this.ItemCollection.Clear();
		GameInstance.Game.SpriteListMapRoles(startIndex);
		Super.ShowNetWaiting(string.Empty);
	}

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection.Clear();
		if (this.CurrentListRolesData != null)
		{
			List<SearchRoleData> searchRoleDataList = this.CurrentListRolesData.SearchRoleDataList;
			if (searchRoleDataList != null)
			{
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < searchRoleDataList.Count; j++)
					{
						if (searchRoleDataList[j].RoleID != Global.Data.roleData.RoleID)
						{
							TeamPartItem teamPartItem = U3DUtils.NEW<TeamPartItem>();
							teamPartItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
							{
								Global.CalcOriginalOccupationID(searchRoleDataList[j].Occupation),
								searchRoleDataList[j].RoleSex
							});
							teamPartItem.RoleID = searchRoleDataList[j].RoleID;
							teamPartItem.PersonName = searchRoleDataList[j].RoleName;
							teamPartItem.PersonLevel = string.Concat(new object[]
							{
								"LV",
								searchRoleDataList[j].Level,
								"[",
								searchRoleDataList[j].ChangeLifeLev,
								Global.GetLang("转]")
							});
							teamPartItem.PersonForce = Global.GetLang("战力：") + searchRoleDataList[j].CombatForce;
							teamPartItem.TeamName = ConfigSettings.GetMapNameByCode(searchRoleDataList[j].MapCode, false);
							teamPartItem.PageType = 2;
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
				}
				this.ItemCollection.DelayUpdate();
			}
		}
		if (this.ItemCollection.Count > 0)
		{
			this.lbPlayer.SelectedIndex = 0;
		}
	}

	private void lbPlayer_SelectionChanged(object sender, EventArgs e)
	{
		if (null == this.lbPlayer.LastSelectedItem && this.lbPlayer.LastSelectedItem != this.lbPlayer.SelectedItem)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbPlayer.SelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = true;
			}
			return;
		}
		if (null != this.lbPlayer.LastSelectedItem && null != this.lbPlayer.SelectedItem && this.lbPlayer.LastSelectedItem != this.lbPlayer.SelectedItem)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbPlayer.LastSelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = false;
			}
			teamPartPageItem = U3DUtils.AS<TeamPartPageItem>(this.lbPlayer.SelectedItem);
			if (null != teamPartPageItem)
			{
				teamPartPageItem.SelectedState = true;
			}
		}
	}

	public void RefreshListData(ListRolesData listRolesData)
	{
		Super.HideNetWaiting();
		this.CurrentListRolesData = listRolesData;
		this.RefreshItemsListPerpage();
	}

	public void RefreshItemsListPerpage()
	{
		this.ItemCollection.Clear();
		if (this.CurrentListRolesData == null || this.CurrentListRolesData.SearchRoleDataList == null)
		{
			return;
		}
		List<SearchRoleData> searchRoleDataList = this.CurrentListRolesData.SearchRoleDataList;
		for (int i = 0; i < searchRoleDataList.Count; i++)
		{
			if (searchRoleDataList[i].RoleID == Global.Data.roleData.RoleID)
			{
				searchRoleDataList.RemoveAt(i);
				break;
			}
		}
		int num = (int)Mathf.Ceil((float)searchRoleDataList.Count / 5f);
		for (int j = 0; j < num; j++)
		{
			TeamPartPageItem teamPartPageItem = U3DUtils.NEW<TeamPartPageItem>();
			this.ItemCollection.AddNoUpdate(teamPartPageItem);
			teamPartPageItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(this, e);
				}
			};
			int num2 = (searchRoleDataList.Count - j * 5 < 5) ? (searchRoleDataList.Count - j * 5) : 5;
			teamPartPageItem.AddItems(searchRoleDataList.GetRange(j * 5, num2));
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

	public GButton btnRefresh;

	public ListBox lbPlayer;

	private bool FirstInitPartData = true;

	private ListRolesData CurrentListRolesData;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public UICenterOnChild centerController;

	public UIPanel panel;

	public GameObject radioGroup;

	public UISprite radioBackground;

	public UISprite radioForeground;

	private int pageCount;

	private ObservableCollection _ItemCollection;
}
