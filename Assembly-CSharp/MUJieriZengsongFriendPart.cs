using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class MUJieriZengsongFriendPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemsSource = this.friendList.ItemsSource;
		this.friendList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		GameInstance.Game.SpriteGetFriends();
	}

	public void LoadFriendlist()
	{
		List<FriendData> friendDataList = Global.Data.FriendDataList;
		if (friendDataList == null)
		{
			return;
		}
		for (int i = 0; i < friendDataList.Count; i++)
		{
			MUJieriZengsongFriendItem mujieriZengsongFriendItem = U3DUtils.NEW<MUJieriZengsongFriendItem>();
			FriendData friendData = friendDataList[i];
			mujieriZengsongFriendItem.RoleID = friendData.OtherRoleID;
			mujieriZengsongFriendItem.RoleName = friendData.OtherRoleName;
			mujieriZengsongFriendItem.RoleIcon = Global.CalcOriginalOccupationID(friendData.Occupation).ToString();
			this.ItemsSource.Add(mujieriZengsongFriendItem);
			UIPanel component = mujieriZengsongFriendItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		MUJieriZengsongFriendItem mujieriZengsongFriendItem = U3DUtils.AS<MUJieriZengsongFriendItem>(this.friendList.SelectedItem);
		if (mujieriZengsongFriendItem == null)
		{
			return;
		}
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = mujieriZengsongFriendItem.RoleID,
			Title = mujieriZengsongFriendItem.RoleName
		});
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox friendList;

	private ObservableCollection ItemsSource;
}
